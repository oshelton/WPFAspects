/*
 * Copyright 2017 Jack owen Shelton
 * Licensed under the terms of the MIT license.
 * Part of the WPFAspects project. 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Core
{
    /// <summary>
    /// Base class for view models that need validation.
    /// </summary>
    public abstract class ValidatedModel : Model, INotifyDataErrorInfo
    {
        public ValidatedModel()
        {
            Validator = GetValidator();
        }
        
        public bool HasErrors => Validator?.HasErrors ?? false;

        public IEnumerable GetErrors(string propertyName)
        {
            return GetErrorsList(propertyName);
        }

        public IReadOnlyList<string> GetErrorsList(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException(propertyName);

            return Validator?.GetErrorsForProperty(propertyName) ?? new List<string>();
        }

        ///Event triggered when the validation state of an object's property changes.
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        internal void InvokeErrorsChanged(string propertyName = null)
        {
            OnPropertyChanged(nameof(HasErrors));
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
    }
}
