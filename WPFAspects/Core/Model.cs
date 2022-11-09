using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPFAspects.Utils;

namespace WPFAspects.Core;

/// <summary>
/// Base class for Data and View model classes.
/// </summary>
public abstract class Model : INotifyPropertyChanging, INotifyPropertyChanged
{
	public virtual HashSet<string> DefaultTrackedProperties => s_defaultTrackedProperties;

	protected static TValue CheckIsOnMainThread<TValue>(TValue value)
	{
		if (!DispatcherHelper.IsOnMainThread())
			throw new InvalidOperationException("Cannot access from background thread.");

		return value;
	}

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

	public event EventHandler<PropertyChangingWithValueEventArgs> PropertyChangingFromValue;
	public event PropertyChangingEventHandler PropertyChanging;

	protected virtual void OnPropertyChanging(object previousValue, [CallerMemberName] string propertyName = null)
	{
		PropertyChangingFromValue?.Invoke(this, new PropertyChangingWithValueEventArgs(propertyName, previousValue));
		PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
	}

	public event EventHandler<PropertyChangedWithValueEventArgs> PropertyChangedToValue;
	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged(object newValue, [CallerMemberName] string propertyName = null)
	{
		PropertyChangedToValue?.Invoke(this, new PropertyChangedWithValueEventArgs(propertyName, newValue));
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	private static readonly HashSet<string> s_defaultTrackedProperties = new HashSet<string>();
}
