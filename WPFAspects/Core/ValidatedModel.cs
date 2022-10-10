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
		public bool HasErrors => Validator?.HasErrors ?? false;

		public IEnumerable GetErrors(string propertyName) => GetErrorsList(propertyName);

		public IReadOnlyList<string> GetErrorsList(string propertyName)
		{
			if (string.IsNullOrWhiteSpace(propertyName))
				throw new ArgumentException(propertyName);

			return Validator?.GetErrorsForProperty(propertyName) ?? new List<string>();
		}

		// Event triggered when the validation state of an object's property changes.
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public Validation.ValidatorBase Validator { get; protected set; }

		internal void InvokeErrorsChanged(string propertyName = null)
		{
			OnPropertyChanged(nameof(HasErrors));
			ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
		}
	}
}
