using System.ComponentModel;

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
	public object NewValue { get; }
}
