/*
 * Copyright 2017 Jack owen Shelton
 * Licensed under the terms of the MIT license.
 * Part of the WPFAspects project. 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Core
{
    /// <summary>
    /// Event args for property changing that also supplies the previous value.
    /// </summary>
    public class PropertyChangingWithValueEventArgs : PropertyChangingEventArgs
    {
        public PropertyChangingWithValueEventArgs(string propertyName, object previousValue)
            : base(propertyName)
        {
            PreviousValue = previousValue;
        }

        /// <summary>
        /// Previous (current) value of the property.
        /// </summary>
        public object PreviousValue { get; private set; }
    }

    /// <summary>
    /// Event args for property changing that also supplies the previous value.
    /// </summary>
    public class PropertyChangedWithValueEventArgs : PropertyChangedEventArgs
    {
        public PropertyChangedWithValueEventArgs(string propertyName, object newValue)
            : base(propertyName)
        {
            NewValue = newValue;
        }

        /// <summary>
        /// New value of the property.
        /// </summary>
        public object NewValue { get; private set; }
    }

    /// <summary>
    /// Base class for Data and View model classes.
    /// </summary>
    public abstract class Model : INotifyDataErrorInfo, INotifyPropertyChanging, INotifyPropertyChanged
    {
        public Model()
        {
            Validator = GetValidator();
        }

        #region Validation related items.
        public bool HasErrors => Validator?.HasErrors ?? false;
        //TODO: Implement me! I should not be abstract.  I need stuff from the Validator class that doesn't exist yet though.
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }

        ///Event triggered when the validation state of an object's property changes.
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        internal void InvokeErrorsChanged(string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Subclasses should override this method in order to provide an instance of a Validator object appropriate
        /// to the type.
        /// </summary>
        /// <remarks>
        /// Null can be returned for objects that don't really need validation.
        /// </remarks>
        protected virtual Validation.Validator GetValidator() { return null; }
        public Validation.Validator Validator { get; private set; }
        #endregion

        private static HashSet<string> _defaultUntrackedProperties = new HashSet<string>();
        public virtual HashSet<string> DefaultUntrackedProperties => _defaultUntrackedProperties;

        protected bool SetPropertyBackingValue<T>(T newValue, ref T field, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                OnPropertyChanging(field, propertyName);
                field = newValue;
                OnPropertyChanged(field, propertyName);
                return true;
            }
            else
                return false;
        }

        #region INotifyPropertyChanging related items.
        public event EventHandler<PropertyChangingWithValueEventArgs> PropertyChangingFromValue;
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnPropertyChanging(object previousValue, [CallerMemberName] string propertyName = null)
        {
            PropertyChangingFromValue?.Invoke(this, new PropertyChangingWithValueEventArgs(propertyName, previousValue));
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
        #endregion

        #region INotifyPropertyChanged related items.
        public event EventHandler<PropertyChangedWithValueEventArgs> PropertyChangedToValue;
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnPropertyChanged(object newValue, [CallerMemberName] string propertyName = null)
        {
            PropertyChangedToValue?.Invoke(this, new PropertyChangedWithValueEventArgs(propertyName, newValue));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
