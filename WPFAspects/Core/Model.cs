/*
 * Copyright 2017 Jack owen Shelton
 * Licensed under the terms of the MIT license.
 * Part of the WPFAspects project. 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

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
    public abstract class Model : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public Model() {}

        private static HashSet<string> _defaultUntrackedProperties = new HashSet<string>();
        public virtual HashSet<string> DefaultUntrackedProperties => _defaultUntrackedProperties;

        protected T CheckIsOnMainThread<T>(T value)
        {
            if (Application.Current != null && !Application.Current.Dispatcher.CheckAccess())
                throw new InvalidOperationException($"Cannot access from background thread.");

            return value;
        }

        protected bool SetPropertyBackingValue<T>(T newValue, ref T field, [CallerMemberName] string propertyName = null)
        {
            if (Application.Current != null && !Application.Current.Dispatcher.CheckAccess())
                throw new InvalidOperationException($"Cannot set property \"{propertyName}\" from a background thread.");

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
