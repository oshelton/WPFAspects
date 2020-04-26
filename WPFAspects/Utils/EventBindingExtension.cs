using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace WPFAspects.Utils
{
    /// <summary>
    /// Class that allows binding XAML events to view model methods.
    /// </summary>
    /// <remarks>Code is from here: https://thomaslevesque.com/2011/09/23/wpf-4-5-subscribing-to-an-event-using-a-markup-extension/. </remarks>
    public class EventBindingExtension : MarkupExtension
    {
        private EventInfo _eventInfo;

        public EventBindingExtension() { }

        public EventBindingExtension(string eventHandlerName)
        {
            this.EventHandlerName = eventHandlerName;
        }

        public EventBindingExtension(string eventHandlerName, DependencyObject source)
        {
            this.EventHandlerName = eventHandlerName;
            this.Source = source;
        }

        [ConstructorArgument("eventHandlerName")]
        public string EventHandlerName { get; set; }

        [ConstructorArgument("Source")]
        public DependencyObject Source { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(EventHandlerName))
                throw new ArgumentException("The EventHandlerName property is not set", "EventHandlerName");

            var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            var targetObj = target.TargetObject as DependencyObject;
            if (targetObj == null)
                throw new InvalidOperationException("The target object must be a DependencyObject");

            _eventInfo = target.TargetProperty as EventInfo;
            if (_eventInfo == null)
                throw new InvalidOperationException("The target property must be an event");

            object dataContext = null;
            if (Source is object)
                dataContext = GetDataContext(Source);
            else
                dataContext = GetDataContext(targetObj);

            if (dataContext == null)
            {
                SubscribeToDataContextChanged(targetObj);
                return GetDummyHandler(_eventInfo.EventHandlerType);
            }

            var handler = GetHandler(dataContext, _eventInfo, EventHandlerName);
            if (handler == null)
            {
                Trace.TraceError(
                    "EventBinding: no suitable method named '{0}' found in type '{1}' to handle event '{2'}",
                    EventHandlerName,
                    dataContext.GetType(),
                    _eventInfo);
                return GetDummyHandler(_eventInfo.EventHandlerType);
            }

            return handler;

        }

        #region Helper methods

        static Delegate GetHandler(object dataContext, EventInfo eventInfo, string eventHandlerName)
        {
            Type dcType = dataContext.GetType();

            var method = dcType.GetMethod(
                eventHandlerName,
                GetParameterTypes(eventInfo.EventHandlerType));
            if (method != null)
            {
                if (method.IsStatic)
                    return Delegate.CreateDelegate(eventInfo.EventHandlerType, method);
                else
                    return Delegate.CreateDelegate(eventInfo.EventHandlerType, dataContext, method);
            }

            return null;
        }

        static Type[] GetParameterTypes(Type delegateType)
        {
            var invokeMethod = delegateType.GetMethod("Invoke");
            return invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        static object GetDataContext(DependencyObject target)
        {
            return target.GetValue(FrameworkElement.DataContextProperty)
                ?? target.GetValue(FrameworkContentElement.DataContextProperty);
        }

        static readonly Dictionary<Type, Delegate> _dummyHandlers = new Dictionary<Type, Delegate>();

        static Delegate GetDummyHandler(Type eventHandlerType)
        {
            Delegate handler;
            if (!_dummyHandlers.TryGetValue(eventHandlerType, out handler))
            {
                handler = CreateDummyHandler(eventHandlerType);
                _dummyHandlers[eventHandlerType] = handler;
            }
            return handler;
        }

        static Delegate CreateDummyHandler(Type eventHandlerType)
        {
            var parameterTypes = GetParameterTypes(eventHandlerType);
            var returnType = eventHandlerType.GetMethod("Invoke").ReturnType;
            var dm = new DynamicMethod("DummyHandler", returnType, parameterTypes);
            var il = dm.GetILGenerator();
            if (returnType != typeof(void))
            {
                if (returnType.IsValueType)
                {
                    var local = il.DeclareLocal(returnType);
                    il.Emit(OpCodes.Ldloca_S, local);
                    il.Emit(OpCodes.Initobj, returnType);
                    il.Emit(OpCodes.Ldloc_0);
                }
                else
                {
                    il.Emit(OpCodes.Ldnull);
                }
            }
            il.Emit(OpCodes.Ret);
            return dm.CreateDelegate(eventHandlerType);
        }

        private void SubscribeToDataContextChanged(DependencyObject targetObj)
        {
            DependencyPropertyDescriptor
                .FromProperty(FrameworkElement.DataContextProperty, targetObj.GetType())
                .AddValueChanged(targetObj, TargetObject_DataContextChanged);
        }

        private void UnsubscribeFromDataContextChanged(DependencyObject targetObj)
        {
            DependencyPropertyDescriptor
                .FromProperty(FrameworkElement.DataContextProperty, targetObj.GetType())
                .RemoveValueChanged(targetObj, TargetObject_DataContextChanged);
        }

        private void TargetObject_DataContextChanged(object sender, EventArgs e)
        {
            DependencyObject targetObj = sender as DependencyObject;
            if (targetObj == null)
                return;

            object dataContext = GetDataContext(targetObj);
            if (dataContext == null)
                return;

            var handler = GetHandler(dataContext, _eventInfo, EventHandlerName);
            if (handler != null)
            {
                _eventInfo.AddEventHandler(targetObj, handler);
            }
            UnsubscribeFromDataContextChanged(targetObj);
        }

        #endregion
    }
}
