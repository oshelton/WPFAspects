using System.Collections;
using System.ComponentModel;

namespace WPFAspects.Core;

/// <summary>
/// Base class for view models that need validation.
/// </summary>
public abstract class ValidatedModel : Model, INotifyDataErrorInfo
{
	public bool HasErrors => Validator?.HasErrors ?? false;

	public IEnumerable GetErrors(string propertyName) => GetErrorsList(propertyName);

	public IReadOnlyList<string> GetErrorsList(string propertyName)
	{
		if (string.IsNullOrWhiteSpace(propertyName))
			throw new ArgumentNullException(propertyName);

		return Validator?.GetErrorsForProperty(propertyName) ?? new List<string>();
	}

	// Event triggered when the validation state of an object's property changes.
	public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

	public virtual Validation.ValidatorBase Validator { get; protected set; }

	internal void InvokeErrorsChanged(string propertyName = null)
	{
		OnPropertyChanged(nameof(HasErrors));
		ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
	}
}
