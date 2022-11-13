using System.Collections;
using System.ComponentModel;

namespace WPFAspects.Core;

/// <summary>
/// Base class for view models that need validation.
/// </summary>
public abstract class ValidatedModel : Model, INotifyDataErrorInfo
{
	/// <summary>
	/// Get whether or not the model has any validation errors.
	/// </summary>
	public bool HasErrors => Validator?.HasErrors ?? false;

	/// <summary>
	/// Get an IEnumerable of all errors for the passed in property.
	/// </summary>
	public IEnumerable GetErrors(string propertyName) => GetErrorsList(propertyName);

	/// <summary>
	/// Get a strongly typed IEnumerable of all errors for the passed in property.
	/// </summary>
	public IReadOnlyList<string> GetErrorsList(string propertyName)
	{
		if (string.IsNullOrWhiteSpace(propertyName))
			return Validator.GetErrors();
		else
			return Validator?.GetErrorsForProperty(propertyName) ?? new List<string>();
	}

	/// <summary>
	/// Event triggered when the validation state of an object's property changes.
	/// </summary>
	public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

	/// <summary>
	/// Get the validator to be used by the model.
	/// </summary>
	public virtual Validation.ValidatorBase Validator { get; protected set; }

	/// <summary>
	/// Invoked when the errored sstate of a property changes.
	/// </summary>
	internal void InvokeErrorsChanged(string propertyName = null)
	{
		OnPropertyChanged(nameof(HasErrors));
		ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
	}
}
