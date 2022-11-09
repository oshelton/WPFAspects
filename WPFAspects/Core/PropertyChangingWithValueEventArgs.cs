using System.ComponentModel;

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
	public object PreviousValue { get; }
}
