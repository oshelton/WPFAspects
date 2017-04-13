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
    /// Base class for Data and View model classes.
    /// </summary>
    public abstract class Model : INotifyDataErrorInfo, INotifyPropertyChanging, INotifyPropertyChanged
    {
        Model()
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

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Subclasses should override this method in order to provide an instance of a Validator object appropriate
        /// to the type.
        /// </summary>
        /// <remarks>
        /// Null can be returned for objects that don't really need validation.
        /// </remarks>
        protected virtual Validation.Validator GetValidator() { return null; }
        public Validation.Validator Validator { get; protected set; }
        #endregion

        #region INotifyPropertyChanging related items.
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void OnThisPropertyChanging([CallerMemberName] string propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
        #endregion

        #region INotifyPropertyChanged related items.
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected virtual void OnThisPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
