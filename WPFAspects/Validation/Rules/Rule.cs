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

        ///Indicate when the validation rule applies.
        public Rule<T> AppliesWhen(Func<T, bool> func)
        {
            if (_AppliesWhenFunction != null)
                throw new InvalidOperationException("This validation rule already has an applies when function.");

            _AppliesWhenFunction = func;

            return this;
        }

        ///Get whether or not the rule currently applies.
        public bool RuleApplies(T toValidate)
        {
            return _AppliesWhenFunction?.Invoke(toValidate) ?? true;
        }

        ///Set the message to be displayed when validation fails.
        public Rule<T> WithOnFailMessage(string message)
        {
            return WithOnFailMessage((T t) => message);
        }
        ///Set the message to be displayed when validation fails.
        public Rule<T> WithOnFailMessage(Func<T, string> failMessageFunc)
        {
            if (_OnFailMessage != null)
                throw new InvalidOperationException("This validation rule already has a failure message.");

            _OnFailMessage = failMessageFunc;

            return this;
        }

        ///Add another property to check this rule on.
        public Rule<T> AlsoCheckOn(string otherProperty)
        {
            _AlsoCheckOnProperties.Add(otherProperty);

            return this;
        }

        ///Get whether or not this rule should be checked when the passed in property changes.
        public bool RuleShouldBeCheckedOnChanged(string otherProperty)
        {
            return _AlsoCheckOnProperties.Contains(otherProperty);
        }

        ///Check the rule's logic.
        public string CheckRule(T toValidate, object value)
        {
            if (_OnFailMessage == null)
                throw new InvalidOperationException("Validation rule must have failure message assigned to it.");

            return PerformRuleLogic(toValidate, value) ? null : _OnFailMessage(toValidate);
        }

        ///Method that implements the validation logic for this validation rule.
        ///<remarks>True indicates that the rule passes; false indicates failure.</remarks>
        protected abstract bool PerformRuleLogic(T toValidate, object value);

        ///Function invoked to generate the validation failed message.
        private Func<T, string> _OnFailMessage = null;

        ///Function defining when this function applies.
        private Func<T, bool> _AppliesWhenFunction = null;

        ///List of other properties on the validated object that should trigger this rule when they change.
        private HashSet<string> _AlsoCheckOnProperties = new HashSet<string>();
    }
}
