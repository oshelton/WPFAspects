using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFAspects.Utils;

namespace WPFAspects.Validation;

/// <summary>
/// Base class for object validators.  This is what you want to subclass in client code.
/// </summary>
/// <typeparam name="TValue">The type this validator validates.</typeparam>
public abstract class Validator<TValue> : ValidatorBase
	where TValue : Core.ValidatedModel
{
	// Constructor, just invokes the base class constructor (as all subclasses should also do.
	protected Validator(TValue model)
		: base(model)
	{
		ValidatedObject = model;
	}

	// Get the object this validator validates.
	public TValue ValidatedObject { get; init; }

	// Get whether or not the validator has any errors.
	public override bool HasErrors
	{
		get => CheckIsOnMainThread(m_hasErrors);
		protected set
		{
			if (SetPropertyBackingValue(value, ref m_hasErrors))
			{
				if (value)
					InvokeValidationFailed(ValidatedObject);
				else
					InvokeValidationSuccess(ValidatedObject);
			}
		}
	}

	// Add a rule to the passed in property to this validator.
	public Rules.Rule<TValue> AddRule(Rules.Rule<TValue> newRule)
	{
		List<Rules.Rule<TValue>> ruleList = null;
		string forProperty = newRule.ForProperty;
		if (m_validationRules.TryGetValue(forProperty, out ruleList))
			ruleList.Add(newRule);
		else
			m_validationRules.Add(forProperty, new List<Rules.Rule<TValue>>() { newRule });

		return newRule;
	}

	// Validate all properties on this object.
	public override void CheckObject()
	{
		foreach (var ruleGroup in m_validationRules)
		{
			CheckPropertyInternal(ruleGroup.Key);
		}
	}

	// Validate a specific property.
	public override void CheckProperty(string propertyName)
	{
		HashSet<string> processed = new HashSet<string>();
		if (m_validationRules.ContainsKey(propertyName))
			CheckPropertyInternal(propertyName);
		processed.Add(propertyName);

		// Also check properties for whom the property is set up as an also check on property.
		foreach (var otherToCheck in m_validationRules.Where(v => v.Value.Any(r => r.RuleShouldBeCheckedOnChanged(propertyName))))
		{
			if (!processed.Contains(otherToCheck.Key))
			{
				processed.Add(otherToCheck.Key);
				CheckPropertyInternal(otherToCheck.Key);
			}
		}
	}

	// Underlying logic for validating a property.
	protected void CheckPropertyInternal(string propertyName)
	{
		var rulesForThisProperty = m_validationRules[propertyName];
		bool anyRuleFailed = false;

		// Reset the validation state for this property.
		if (ValidationErrorMessages.ContainsKey(propertyName))
			ValidationErrorMessages[propertyName].Clear();
		else
			ValidationErrorMessages.TryAdd(propertyName, new List<string>());
		var errorList = ValidationErrorMessages[propertyName];

		// Go ahead and grab the value of the property for more efficient access.
		object propertyValue = ValidatedObject.GetPropertyValue<object>(propertyName);
		foreach (var rule in rulesForThisProperty)
		{
			// Confirm the rule applies before checking it.
			if (rule.RuleApplies(ValidatedObject))
			{
				// Check the property's value against the rule.
				var result = rule.CheckRule(ValidatedObject, propertyValue);

				// A non-null empty string value indicates the rule failed.
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

			// Only set the object as being free of validation errors if none of its properties have errors.
			if (ValidationErrorMessages.Values.All(v => !v.Any()))
				HasErrors = false;
		}

		// No matter what, indicate that the object's validation errors have changed.
		ValidatedObject.InvokeErrorsChanged(propertyName);
	}

	private bool m_hasErrors;

	// Dictionary of validation rules attached to this validator.
	private readonly Dictionary<string, List<Rules.Rule<TValue>>> m_validationRules = new Dictionary<string, List<Rules.Rule<TValue>>>();
}
