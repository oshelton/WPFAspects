using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Validation.Rules
{
	/// <summary>
	/// Simple validation rule that expects validation logic to be provided via another function or lambda.
	/// </summary>
	/// <typeparam name="TValue">Value type the rule validates.</typeparam>
	public class CustomRule<TValue> : Rule<TValue>
		where TValue : Core.ValidatedModel
	{
		/// <param name="ruleLogic">Bool returning function (should return true if it passes).</param>
		public CustomRule(string propertyName, Func<TValue, bool> ruleLogic)
			: base(propertyName)
		{
			m_ruleLogic = ruleLogic ?? throw new ArgumentNullException(nameof(ruleLogic));
		}

		protected override bool PerformRuleLogic(TValue toValidate, object value) => m_ruleLogic(toValidate);

		private readonly Func<TValue, bool> m_ruleLogic;
	}
}
