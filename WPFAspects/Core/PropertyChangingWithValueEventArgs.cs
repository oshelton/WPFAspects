using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAspects.Core;

/// <summary>
/// Event args for property changing that also supplies the previous value.
/// </summary>
public sealed class PropertyChangingWithValueEventArgs : PropertyChangingEventArgs
{
	public PropertyChangingWithValueEventArgs(string propertyName, object previousValue)
		: base(propertyName)
	{
		PreviousValue = previousValue;
	}

	/// <summary>
	/// Previous (current) value of the property.
	/// </summary>
	public object PreviousValue { get; init; }
}
