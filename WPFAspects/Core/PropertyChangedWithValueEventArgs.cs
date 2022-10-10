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
public sealed class PropertyChangedWithValueEventArgs : PropertyChangedEventArgs
{
	public PropertyChangedWithValueEventArgs(string propertyName, object newValue)
		: base(propertyName)
	{
		NewValue = newValue;
	}

	/// <summary>
	/// New value of the property.
	/// </summary>
	public object NewValue { get; init; }
}
