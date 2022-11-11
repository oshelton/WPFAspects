using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Markup;

namespace WPFAspects.Utils;

/// <summary>
/// Class that allows binding XAML events to view model methods.
/// </summary>
/// <remarks>Code is from here: https://thomaslevesque.com/2011/09/23/wpf-4-5-subscribing-to-an-event-using-a-markup-extension/. </remarks>
public class EventBindingExtension : MarkupExtension
{
	public EventBindingExtension()
	{
	}

	public EventBindingExtension(string eventHandlerName) => EventHandlerName = eventHandlerName;

	public EventBindingExtension(string eventHandlerName, DependencyObject source)
	{
		EventHandlerName = eventHandlerName;
		Source = source;
	}

	/// <summary>
	/// Name of themethod.
	/// </summary>
	[ConstructorArgument("eventHandlerName")]
	public string EventHandlerName { get; set; }

	/// <summary>
	/// Source object that provides the method.
	/// </summary>
	[ConstructorArgument("Source")]
	public DependencyObject Source { get; set; }

	public override object ProvideValue(IServiceProvider serviceProvider)
	{
		if (string.IsNullOrEmpty(EventHandlerName))
			throw new InvalidOperationException("The EventHandlerName property is not set");

		var target = (IProvideValueTarget) serviceProvider.GetService(typeof(IProvideValueTarget));

		if (target.TargetObject is not DependencyObject targetObj)
			throw new InvalidOperationException("The target object must be a DependencyObject");

		m_eventInfo = target.TargetProperty as EventInfo;
		if (m_eventInfo is null)
			throw new InvalidOperationException("The target property must be an event");

		var dataContext = Source is not null ? GetDataContext(Source) : GetDataContext(targetObj);
		if (dataContext is null)
		{
			SubscribeToDataContextChanged(targetObj);
			return GetDummyHandler(m_eventInfo.EventHandlerType);
		}

		var handler = GetHandler(dataContext, m_eventInfo, EventHandlerName);
		if (handler is null)
		{
			Trace.TraceError(
				"EventBinding: no suitable method named '{0}' found in type '{1}' to handle event '{2'}",
				EventHandlerName,
				dataContext.GetType(),
				m_eventInfo);
			return GetDummyHandler(m_eventInfo.EventHandlerType);
		}

		return handler;
	}

	private static Delegate GetHandler(object dataContext, EventInfo eventInfo, string eventHandlerName)
	{
		var dcType = dataContext.GetType();

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

	private static Type[] GetParameterTypes(Type delegateType)
	{
		var invokeMethod = delegateType.GetMethod("Invoke");
		return invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();
	}

	private static object GetDataContext(DependencyObject target)
	{
		return target.GetValue(FrameworkElement.DataContextProperty)
			?? target.GetValue(FrameworkContentElement.DataContextProperty);
	}

	private static Delegate GetDummyHandler(Type eventHandlerType)
	{
		Delegate handler;
		if (!s_dummyHandlers.TryGetValue(eventHandlerType, out handler))
		{
			handler = CreateDummyHandler(eventHandlerType);
			s_dummyHandlers[eventHandlerType] = handler;
		}
		return handler;
	}

	private static Delegate CreateDummyHandler(Type eventHandlerType)
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

	private void SubscribeToDataContextChanged(DependencyObject targetObj) => DependencyPropertyDescriptor
			.FromProperty(FrameworkElement.DataContextProperty, targetObj.GetType())
			.AddValueChanged(targetObj, TargetObject_DataContextChanged);

	private void UnsubscribeFromDataContextChanged(DependencyObject targetObj) => DependencyPropertyDescriptor
			.FromProperty(FrameworkElement.DataContextProperty, targetObj.GetType())
			.RemoveValueChanged(targetObj, TargetObject_DataContextChanged);

	private void TargetObject_DataContextChanged(object sender, EventArgs e)
	{
		if (sender is not DependencyObject targetObj)
			return;

		var dataContext = GetDataContext(targetObj);
		if (dataContext is null)
			return;

		var handler = GetHandler(dataContext, m_eventInfo, EventHandlerName);
		if (handler is not null)
			m_eventInfo.AddEventHandler(targetObj, handler);

		UnsubscribeFromDataContextChanged(targetObj);
	}

	private static readonly Dictionary<Type, Delegate> s_dummyHandlers = new Dictionary<Type, Delegate>();

	private EventInfo m_eventInfo;
}
