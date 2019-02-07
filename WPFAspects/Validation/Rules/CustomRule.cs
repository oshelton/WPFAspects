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
    /// <typeparam name="T"></typeparam>
    public class CustomRule<T> : Rule<T> where T : Core.ValidatedModel
    {
        /// <param name="ruleLogic">Bool returning function (should return true if it passes).</param>
        public CustomRule(string propertyName, Func<T, bool> ruleLogic): base(propertyName)
        {
            RuleLogic = ruleLogic ?? throw new ArgumentException(nameof(ruleLogic));
        }

        private Func<T, bool> RuleLogic = null;

        protected override bool PerformRuleLogic(T toValidate, object value)
        {
            return RuleLogic(toValidate);
        }
    }
}
