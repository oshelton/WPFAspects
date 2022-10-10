/*
 * Copyright 2017 Jack owen Shelton
 * Licensed under the terms of the MIT license.
 * Part of the WPFAspects project.
 */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WPFAspects.Utils;

namespace WPFAspects.Validation
{
	/// <summary>
	/// Base class for all object validators.  Implements just the common validation properties, events, and methods.
	/// </summary>
	/// <remarks>
	/// This class should not be inherited from in client code.  Instead use the generic subclass of this class.
	/// </remarks>
	public abstract class ValidatorBase : Core.Model
	{
		protected ValidatorBase(Core.Model validatedObject)
		{
			if (validatedObject is null)
				throw new ArgumentNullException(nameof(validatedObject));

			validatedObject.PropertyChanged += (s, args) => OnValidatedObjectPropertyChanged(args.PropertyName);
		}

		// Get whether or not the validated object has any validation errors.
		public abstract bool HasErrors
		{
			get;
			protected set;
		}

		// Dictionary containing the current validation state of the object.
		protected ConcurrentDictionary<string, List<string>> ValidationErrorMessages { get; init; } = new ConcurrentDictionary<string, List<string>>();

		// Get validation error messages related to a specific property.
		public IReadOnlyList<string> GetErrorsForProperty(string propertyName)
		{
			if (ValidationErrorMessages.ContainsKey(propertyName))
				return ValidationErrorMessages[propertyName];
			else
				return new List<string>();
		}

		// Handle actual validation as properties change on the validated object.
		public void OnValidatedObjectPropertyChanged(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
				CheckObject();
			else
				CheckProperty(propertyName);
		}

		public abstract void CheckObject();
		public abstract void CheckProperty(string propertyName);

		public event EventHandler<ValidationEventArgs> ValidationFail;
		public event EventHandler<ValidationEventArgs> ValidationSuccess;

		protected void InvokeValidationFailed(Core.Model model) => ValidationFail?.Invoke(this, new ValidationEventArgs(model));
		protected void InvokeValidationSuccess(Core.Model model) => ValidationSuccess?.Invoke(this, new ValidationEventArgs(model));

		public event EventHandler<PropertyValidationEventArgs> PropertyValidationFail;
		public event EventHandler<PropertyValidationEventArgs> PropertyValidationSuccess;

		protected void InvokePropertyValidationFailed(Core.Model model, string propertyName, object value) => PropertyValidationFail?.Invoke(this, new PropertyValidationEventArgs(model, propertyName, value));
		protected void InvokePropertyValidationSuccess(Core.Model model, string propertyName, object value) => PropertyValidationSuccess?.Invoke(this, new PropertyValidationEventArgs(model, propertyName, value));
	}
}
