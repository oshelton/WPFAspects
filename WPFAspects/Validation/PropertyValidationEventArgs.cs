using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Validation;
public sealed class PropertyValidationEventArgs : EventArgs
{
	public PropertyValidationEventArgs(Core.Model validatedObject, string propertyName, object value)
	{
		ValidatedObject = validatedObject;
		PropertyName = propertyName;
		Value = value;
	}

	public Core.Model ValidatedObject { get; init; }
	public string PropertyName { get; init; }
	public object Value { get; init; }
}
