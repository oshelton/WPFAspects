/*
 * Copyright 2017 Jack owen Shelton
 * Licensed under the terms of the MIT license.
 * Part of the WPFAspects project. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Validation.Rules
{
    ///Base class for validation rules; should not be used directly.
    ///<remarks>
    ///Validation rule implementation classes should inherit from Rule T,U and not this class.
    ///</remarks>
    public abstract class Rule<T> where T : Core.Model
    {
        ///Constructor; just sets what property the rule is for.
        public Rule(string forProperty)
        {
            ForProperty = forProperty;
        }

        ///Property the rule is for.
        public string ForProperty { get; private set; }

        ///Get whether or not the rule currently applies.
        public abstract bool RuleApplies(T toValidate);

        ///Get whether or not this rule should be checked when the passed in property changes.
        public abstract bool RuleShouldBeCheckedOnChanged(string otherProperty);

        ///Method that implements the validation logic for this validation rule.
        public abstract string CheckRule(T toValidate, object value);
    }

    ///Core validation rule class; validation rules should inherit from this class.
    ///<remarks>
    ///In sub classes do not assume that an OnFailedMessage function is available; default failed strings should exist.
    ///</remarks>
    public abstract class Rule<T, U> : Rule<T> where T : Core.Model where U : Rule<T, U>
    {
        ///Function invoked to generate the validation failed message.
        private Func<T, U, string> OnFailMessage = null;

        ///Function defining when this function applies.
        private Func<T, bool> AppliesWhenFunction = null;

        ///List of other properties on the validated object that should trigger this rule when they change.
        private HashSet<string> AlsoCheckOnProperties = new HashSet<string>();

        ///Constructor, just calls the base class.
        public Rule(string forProperty) : base(forProperty) { }

        ///Indicate when the validation rule applies.
        public U AppliesWhen(Func<T, bool> func)
        {
            if (AppliesWhenFunction != null)
                throw new InvalidOperationException("This validation rule already has an applies when function.");

            AppliesWhenFunction = func;

            return this as U;
        }
        /// <summary>
        /// Get whether or not the rule currently applies.
        /// </summary>
        /// <returns>True if the AppliesWhen function returns true or the function has not been specified.</returns>
        public override bool RuleApplies(T toValidate)
        {
            return AppliesWhenFunction?.Invoke(toValidate) ?? true;
        }

        ///Set the message to be displayed when validation fails.
        public U WithOnFailMessage(string message)
        {
            return WithOnFailMessage((T t, U u) => message);
        }
        ///Set the message to be displayed when validation fails.
        public U WithOnFailMessage(Func<T, U, string> failMessageFunc)
        {
            if (OnFailMessage != null)
                throw new InvalidOperationException("This validation rule already has a failure message.");

            OnFailMessage = failMessageFunc;

            return this as U;
        }

        ///Add another property to check this rule on.
        public U AlsoCheckOn(string otherProperty)
        {
            AlsoCheckOnProperties.Add(otherProperty);

            return this as U;
        }
        ///Get whether or not the property with the passed in name should also trigger this rule.
        public override bool RuleShouldBeCheckedOnChanged(string otherProperty)
        {
            return AlsoCheckOnProperties.Any(p => p == otherProperty);
        }
    }
}
