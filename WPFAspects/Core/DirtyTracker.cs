﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPFAspects.Utils;

namespace WPFAspects.Core
{
    /// <summary>
    /// Simple class for tracking changes to models.
    /// </summary>
    /// <remarks>Cannot be used to track collection based properties.</remarks>
    public class DirtyTracker : INotifyPropertyChanged
    {
        public DirtyTracker(Model toTrack)
        {
            _TrackedObject = toTrack ?? throw new ArgumentException(nameof(toTrack));
            _IgnoredProperties = toTrack.DefaultUntrackedProperties;

            AddHandlers();
        }

        private Model _TrackedObject = null;
        public Model TrackedObject => _TrackedObject;

        private HashSet<string> _IgnoredProperties = null;
        /// <summary>
        /// Get/Set the properties changes to should be ignored.
        /// </summary>
        /// <remarks>Defaults to Model.DefaultUntrackedProperties.</remarks>
        public HashSet<string> IgnoredProperties
        {
            get => _IgnoredProperties;
            set => _IgnoredProperties = value ?? throw new ArgumentException("Value cannot be null.");
        }

        private bool _IsDirty = false;
        /// <summary>
        /// Get whether or not the tracked object is dirty (has changes).
        /// </summary>
        public bool IsDirty
        {
            get => _IsDirty;
            set
            {
                if (value != _IsDirty)
                {
                    _IsDirty = value;
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }


        /// <summary>
        /// Add the necessary event handlers.
        /// </summary>
        private void AddHandlers()
        {
            _TrackedObject.PropertyChangingFromValue += OnTrackedObjectPropertyChanging;
            _TrackedObject.PropertyChangedToValue += OnTrackedObjectPropertyChanged;
        }

        /// <summary>
        /// Remove the event handlers.
        /// </summary>
        private void RemoveHandlers()
        {
            _TrackedObject.PropertyChangingFromValue -= OnTrackedObjectPropertyChanging;
            _TrackedObject.PropertyChangedToValue -= OnTrackedObjectPropertyChanged;
        }

        /// <summary>
        /// Reset the object to its initial state.
        /// </summary>
        public void ResetToInitialState()
        {

            RemoveHandlers();
            if (_InitialValues.Count > 0)
            {
                foreach (var pair in _InitialValues)
                    _TrackedObject.SetPropertyValue(pair.Key, pair.Value);
            }
            IsDirty = false;
            AddHandlers();
        }

        /// <summary>
        /// Reset the passed in property to its initial value.
        /// </summary>
        public void ResetPropertyToInitialSTate(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException(propertyName);

            if (_InitialValues.TryGetValue(propertyName, out object initial))
                TrackedObject.SetPropertyValue(propertyName, initial);
        }

        /// <summary>
        /// Set the current state of the object as the initial state.
        /// </summary>
        public void SetInitialState()
        {
            _InitialValues.Clear();
            _NewValues.Clear();
            IsDirty = false;
        }

        /// <summary>
        /// Get whether or not the passed in property has changed.
        /// </summary>
        public bool HasPropertyChanged(string propertyName)
        {
            return _NewValues.ContainsKey(propertyName);
        }

        /// <summary>
        /// Get the initial value for the property whose name is passed in.
        /// </summary>
        public object GetInitialValueForProperty(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException(propertyName);

            return _InitialValues.TryGetValue(propertyName, out object initial) ? initial : null; 
        }

        private void OnTrackedObjectPropertyChanging(object sender, PropertyChangingWithValueEventArgs args)
        {
            if ((args.PreviousValue as IEnumerable) == null && !_InitialValues.ContainsKey(args.PropertyName))
                _InitialValues.Add(args.PropertyName, args.PreviousValue);
        }

        private void OnTrackedObjectPropertyChanged(object sender, PropertyChangedWithValueEventArgs args)
        {
            if (args.NewValue is string || !(args.NewValue is IEnumerable))
            {
                if (!Equals(_InitialValues[args.PropertyName], args.NewValue))
                    _NewValues[args.PropertyName] = args.NewValue;
                else
                    _NewValues.Remove(args.PropertyName);

                if (_NewValues.Count > 0)
                    IsDirty = true;
                else
                    IsDirty = false;
            }
        }
        
        /// <summary>
        /// Initial property values of the object, keyed by name.
        /// </summary>
        private readonly Dictionary<string, object> _InitialValues = new Dictionary<string, object>();
        /// <summary>
        /// New values for properties of the object, keyed by name.
        /// </summary>
        private readonly Dictionary<string, object> _NewValues = new Dictionary<string, object>();

        #region INotifyPropertyChanged related items.
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}