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
    public abstract class Validator : INotifyPropertyChanged
    {
        public Validator(Core.Model validatedObject)
        {
            validatedObject.PropertyChanged += (s, args) => OnValidatedObjectPropertyChanged(args.PropertyName);
        }

        ///Get whether or not the validated object has any validation errors.
        public abstract bool HasErrors
        {
            get;
            protected set;
        }

        ///Dictionary containing the current validation state of the object.
        protected ConcurrentDictionary<string, List<string>> ValidationErrorMessages = new ConcurrentDictionary<string, List<string>>();

        ///Get validation error messages related to a specific property.
        public List<string> GetErrorsForProperty(string propertyName)
        {
            if (ValidationErrorMessages.ContainsKey(propertyName))
                return ValidationErrorMessages[propertyName];
            else
                return new List<string>();
        }

        #region Core validation methods.
        ///
        public Task OnValidatedObjectPropertyChanged(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return CheckObject();
            else
                return CheckProperty(propertyName);
        }

        public abstract Task CheckObject();
        public abstract Task CheckProperty(string propertyName);
        #endregion

        #region Validation related events
        public delegate void ObjectValidationHandler(Core.Model validatedObject);
        public event ObjectValidationHandler ValidationFail;
        public event ObjectValidationHandler ValidationSuccess;

        protected void InvokeValidationFailed(Core.Model model) { ValidationFail?.Invoke(model); }
        protected void InvokeValidationSuccess(Core.Model model) { ValidationSuccess?.Invoke(model); }

        public delegate void PropertyValidationHandler(Core.Model validatedObject, string propertyName, object value);
        public event PropertyValidationHandler PropertyValidationFail;
        public event PropertyValidationHandler PropertyValidationSuccess;

        protected void InvokePropertyValidationFailed(Core.Model model, string propertyName, object value) { PropertyValidationFail?.Invoke(model, propertyName, value); }
        protected void InvokePropertyValidationSuccess(Core.Model model, string propertyName, object value) { PropertyValidationSuccess?.Invoke(model, propertyName, value); }
        #endregion

        #region INotifyPropertyChanged related items.
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnThisPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    /// <summary>
    /// Base class for object validators.  This is what you want to subclass in client code.
    /// </summary>
    /// <typeparam name="T">The type this validator validates.</typeparam>
    public abstract class Validator<T> : Validator where T : Core.Model
    {
        ///Dictionary of validation rules attached to this validator.
        private Dictionary<string, List<Rules.Rule<T>>> ValidationRules = new Dictionary<string, List<Rules.Rule<T>>>();

        ///Get the object this validator validates.
        public T ValidatedObject { get; private set; }

        ///Constructor, just invokes the base class constructor (as all subclasses should also do.
        public Validator(T model): base(model)
        {
            ValidatedObject = model;
        }

        private bool _HasErrors = false;
        ///Get whether or not the validator has any errors.
        public override bool HasErrors
        {
            get { return _HasErrors; }
            protected set
            {
                if (value != _HasErrors)
                {
                    _HasErrors = value;
                    OnThisPropertyChanged();
                    if (!value)
                        InvokeValidationFailed(ValidatedObject);
                    else
                        InvokeValidationSuccess(ValidatedObject);

                    ValidatedObject.RaisePropertyChanged(nameof(Core.Model.HasErrors));
                }
            }
        }

        ///Add a rule to the passed in property to this validator.
        public Rules.Rule<T> AddRule(Rules.Rule<T> newRule)
        {
            List<Rules.Rule<T>> ruleList = null;
            string forProperty = newRule.ForProperty;
            if (ValidationRules.TryGetValue(forProperty, out ruleList))
                ruleList.Add(newRule);
            else
                ValidationRules.Add(forProperty, new List<Rules.Rule<T>>() { newRule });

            return newRule;
        }

        #region Validation related methods.
        ///Validate all properties on this object.
        public override Task CheckObject()
        {
            return Task.Run(() =>
            {
                foreach (var ruleGroup in ValidationRules)
                {
                    CheckPropertyInternal(ruleGroup.Key);
                }
            });
        }

        ///Validate a specific property.
        public override Task CheckProperty(string propertyName)
        {
            return Task.Run(() =>
            {
                HashSet<string> processed = new HashSet<string>();
                if (ValidationRules.ContainsKey(propertyName))
                    CheckPropertyInternal(propertyName);
                processed.Add(propertyName);

                //Also check properties for whom the property is set up as an also check on property.
                foreach (var otherToCheck in ValidationRules.Where(v => v.Value.Any(r => r.RuleShouldBeCheckedOnChanged(propertyName))))
                {
                    if (!processed.Contains(otherToCheck.Key))
                    {
                        processed.Add(otherToCheck.Key);
                        CheckPropertyInternal(otherToCheck.Key);
                    }
                }
            });
        }

        ///Underlying logic for validating a property.
        protected void CheckPropertyInternal(string propertyName)
        {
            var rulesForThisProperty = ValidationRules[propertyName];
            bool anyRuleFailed = false;

            //Reset the validation state for this property.
            if (ValidationErrorMessages.ContainsKey(propertyName))
                ValidationErrorMessages[propertyName].Clear();
            else
                ValidationErrorMessages.TryAdd(propertyName, new List<string>());
            var errorList = ValidationErrorMessages[propertyName];

            //Go ahead and grab the value of the property for more efficient access.
            object propertyValue = ValidatedObject.GetPropertyValue<object>(propertyName);
            foreach (var rule in rulesForThisProperty)
            {
                //Confirm the rule applies before checking it.
                if (rule.RuleApplies(ValidatedObject))
                {
                    //Check the property's value against the rule.
                    var result = rule.CheckRule(ValidatedObject, propertyValue);
                    //A non-null empty string value indicates the rule failed.
                    if (!string.IsNullOrEmpty(result))
                    {
                        anyRuleFailed = true;
                        errorList.Add(result);
                    }
                }
            }

            if (anyRuleFailed)
            {
                InvokePropertyValidationFailed(ValidatedObject, propertyName, propertyValue);
                HasErrors = true;
            }
            else
            {
                InvokePropertyValidationSuccess(ValidatedObject, propertyName, propertyValue);
                //Only set the object as being free of validation errors if none of its properties have errors.
                if (ValidationErrorMessages.Values.All(v => !v.Any()))
                    HasErrors = false;
            }

            //No matter what, indicate that the object's validation errors have changed.
            ValidatedObject.InvokeErrorsChanged(propertyName);
        }
        #endregion
    }
}
