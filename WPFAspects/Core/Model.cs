using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPFAspects.Utils;

namespace WPFAspects.Core;

/// <summary>
/// Base class for View model classes.
/// </summary>
public abstract class Model : INotifyPropertyChanging, INotifyPropertyChanged
{
	/// <summary>
	/// Get the properties which should be considered for dirty determination by default.
	/// </summary>
	public virtual HashSet<string> DefaultTrackedProperties => s_defaultTrackedProperties;

	public event EventHandler<PropertyChangingWithValueEventArgs> PropertyChangingFromValue;
	public event PropertyChangingEventHandler PropertyChanging;

	public event EventHandler<PropertyChangedWithValueEventArgs> PropertyChangedToValue;
	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanging(object previousValue, [CallerMemberName] string propertyName = null)
	{
		PropertyChangingFromValue?.Invoke(this, new PropertyChangingWithValueEventArgs(propertyName, previousValue));
		PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
	}

	protected virtual void OnPropertyChanged(object newValue, [CallerMemberName] string propertyName = null)
	{
		PropertyChangedToValue?.Invoke(this, new PropertyChangedWithValueEventArgs(propertyName, newValue));
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	/// <summary>
	/// Check if execution is currently on the main thread; an exception will be thrown if it's not.
	/// </summary>
	protected static TValue CheckIsOnMainThread<TValue>(TValue value)
	{
		if (!DispatcherHelper.IsOnMainThread())
			throw new InvalidOperationException("Cannot access from background thread.");

		return value;
	}

	/// <summary>
	/// Update a property's backing field value if it has actually changed and raise all appropriate events.
	/// </summary>
	/// <exception>An exception will be thrown if the code is not called from the main thread.</exception>
	protected bool SetPropertyBackingValue<TValue>(TValue newValue, ref TValue field, [CallerMemberName] string propertyName = null)
	{
		if (!DispatcherHelper.IsOnMainThread())
			throw new InvalidOperationException($"Cannot set property \"{propertyName}\" from a background thread.");

		if (!Equals(field, newValue))
		{
			OnPropertyChanging(field, propertyName);
			field = newValue;
			OnPropertyChanged(field, propertyName);
			return true;
		}
		else
		{
			return false;
		}
	}

	private static readonly HashSet<string> s_defaultTrackedProperties = new HashSet<string>();
}
