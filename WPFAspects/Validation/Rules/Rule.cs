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
	/// Base class for validation rules; should not be used directly.
	/// <remarks>
	/// Validation rule implementation classes should inherit from Rule T,U and not this class.
	/// </remarks>
	public abstract class Rule<TValue>
		where TValue : Core.Model
	{
		// Constructor; just sets what property the rule is for.
		protected Rule(string forProperty)
		{
			ForProperty = forProperty;
		}

		// Property the rule is for.
		public string ForProperty { get; init; }

		// Indicate when the validation rule applies.
		public Rule<TValue> AppliesWhen(Func<TValue, bool> func)
		{
			if (m_appliesWhenFunction is not null)
				throw new InvalidOperationException("This validation rule already has an applies when function.");

			m_appliesWhenFunction = func;

			return this;
		}

		// Get whether or not the rule currently applies.
		public bool RuleApplies(TValue toValidate) => m_appliesWhenFunction?.Invoke(toValidate) ?? true;

		// Set the message to be displayed when validation fails.
		public Rule<TValue> WithOnFailMessage(string message) => WithOnFailMessage((TValue t) => message);

		// Set the message to be displayed when validation fails.
		public Rule<TValue> WithOnFailMessage(Func<TValue, string> failMessageFunc)
		{
			if (m_onFailMessage != null)
				throw new InvalidOperationException("This validation rule already has a failure message.");

			m_onFailMessage = failMessageFunc;

			return this;
		}

		// Add another property to check this rule on.
		public Rule<TValue> AlsoCheckOn(string otherProperty)
		{
			m_alsoCheckOnProperties.Add(otherProperty);

			return this;
		}

		// Get whether or not this rule should be checked when the passed in property changes.
		public bool RuleShouldBeCheckedOnChanged(string otherProperty) => m_alsoCheckOnProperties.Contains(otherProperty);

		// Check the rule's logic.
		public string CheckRule(TValue toValidate, object value)
		{
			if (m_onFailMessage == null)
				throw new InvalidOperationException("Validation rule must have failure message assigned to it.");

			return PerformRuleLogic(toValidate, value) ? null : m_onFailMessage(toValidate);
		}

		// Method that implements the validation logic for this validation rule.
		// <remarks>True indicates that the rule passes; false indicates failure.</remarks>
		protected abstract bool PerformRuleLogic(TValue toValidate, object value);

		// Function invoked to generate the validation failed message.
		private Func<TValue, string> m_onFailMessage;

		// Function defining when this function applies.
		private Func<TValue, bool> m_appliesWhenFunction;

		// List of other properties on the validated object that should trigger this rule when they change.
		private readonly HashSet<string> m_alsoCheckOnProperties = new HashSet<string>();
	}
}
