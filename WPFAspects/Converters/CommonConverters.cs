using System.Collections;
using System.Windows;

namespace WPFAspects.Converters;

/// <summary>
/// Class to provide simple access to a host of commonly used value converters.
/// </summary>
public static class CommonConverters
{
	/// <summary>
	/// Convert boolean false values to Visibility.Hidden and true to Visibility.Visible.
	/// </summary>
	public static GenericConverter<bool, object> BooleanFalseToVisibilityHidden { get; } =
		new GenericConverter<bool, object>((value, param) => value ? Visibility.Visible : Visibility.Hidden);

	/// <summary>
	/// Convert boolean false values to Visibility.Collapsed and true to Visibility.Visible.
	/// </summary>
	public static GenericConverter<bool, object> BooleanFalseToVisibilityCollapsed { get; } =
		new GenericConverter<bool, object>((value, param) => value ? Visibility.Visible : Visibility.Collapsed);

	/// <summary>
	/// Convert boolean false values to Visibility.Visible and true to Visibility.Hidden.
	/// </summary>
	public static GenericConverter<bool, object> BooleanTrueToVisibilityHidden { get; } =
		new GenericConverter<bool, object>((value, param) => value ? Visibility.Hidden : Visibility.Visible);

	/// <summary>
	/// Convert boolean false values to Visibility.Visible and true to Visibility.Collapsed.
	/// </summary>
	public static GenericConverter<bool, object> BooleanTrueToVisibilityCollapsed { get; } =
		new GenericConverter<bool, object>((value, param) => value ? Visibility.Collapsed : Visibility.Visible);

	/// <summary>
	/// Convert boolean false values to Visibility.Visible and true to Visibility.Collapsed.
	/// </summary>
	public static GenericConverter<bool, object> InvertBoolean { get; } =
		new GenericConverter<bool, object>((value, param) => !value);

	/// <summary>
	/// Returns true if the passed in objects are equal.
	/// </summary>
	public static GenericConverter<object, object> ObjectsAreEqual { get; } =
		new GenericConverter<object, object>((value, param) => value.Equals(param));

	/// <summary>
	/// Returns Visible if the passed in objects are equal or Collapsed.
	/// </summary>
	public static GenericConverter<object, object> ObjectsAreEqualOrCollapsed { get; } =
		new GenericConverter<object, object>((value, param) => value.Equals(param) ? Visibility.Visible : Visibility.Collapsed);

	/// <summary>
	/// Returns Visible if the passed in objects are equal or Hidden.
	/// </summary>
	public static GenericConverter<object, object> ObjectsAreEqualOrHidden { get; } =
		new GenericConverter<object, object>((value, param) => value.Equals(param) ? Visibility.Visible : Visibility.Hidden);

	/// <summary>
	/// Returns Visible if the passed in objects are not equal or Collapsed.
	/// </summary>
	public static GenericConverter<object, object> ObjectsAreNotEqualOrCollapsed { get; } =
		new GenericConverter<object, object>((value, param) => !value.Equals(param) ? Visibility.Visible : Visibility.Collapsed);

	/// <summary>
	/// Returns Visible if the passed in objects are not equal or Hidden.
	/// </summary>
	public static GenericConverter<object, object> ObjectsAreNotEqualOrHidden { get; } =
		new GenericConverter<object, object>((value, param) => !value.Equals(param) ? Visibility.Visible : Visibility.Hidden);

	/// <summary>
	/// Returns visibility collapsed if the input value is equal to null.
	/// </summary>
	public static GenericConverter<object, object> NullToVisibilityCollapsed { get; } =
		new GenericConverter<object, object>((value, param) => value is null ? Visibility.Collapsed : Visibility.Visible, true);

	/// <summary>
	/// Returns visibility collapsed if the input value is null or empty if it is an IEnumerable.
	/// </summary>
	public static GenericConverter<IEnumerable, object> NullEmptyEnumerableToVisibilityCollapsed { get; } =
		new GenericConverter<IEnumerable, object>((value, param) =>
		{
			if (value is null)
				return Visibility.Collapsed;
			return value.Cast<object>().Any() ? Visibility.Visible : Visibility.Collapsed;
		}, true);

	/// <summary>
	/// Returns visibility collapsed if the input value is a non-empty enumerable.
	/// </summary>
	public static GenericConverter<IEnumerable, object> NonEmptyEnumerableToVisibilityCollapsed { get; } =
		new GenericConverter<IEnumerable, object>((value, param) =>
		{
			if (value is null)
				return Visibility.Visible;
			return !value.Cast<object>().Any() ? Visibility.Visible : Visibility.Collapsed;
		}, true);

	/// <summary>
	/// Returns visibility hidden if the input value is equal to null.
	/// </summary>
	public static GenericConverter<object, object> NullToVisibilityHidden { get; } =
		new GenericConverter<object, object>((value, param) => value is null ? Visibility.Hidden : Visibility.Visible, true);

	/// <summary>
	/// Returns visibility hidden if the input value is null or empty if it is an IEnumerable.
	/// </summary>
	public static GenericConverter<IEnumerable, object> NullEmptyEnumerableToVisibilityHidden { get; } =
		new GenericConverter<IEnumerable, object>((value, param) =>
		{
			if (value is null)
				return Visibility.Hidden;
			return value.Cast<object>().Any() ? Visibility.Visible : Visibility.Hidden;
		}, true);

	/// <summary>
	/// Returns visibility collapsed if the input value is a non-empty enumerable.
	/// </summary>
	public static GenericConverter<IEnumerable, object> NonEmptyEnumerableToVisibilityHidden { get; } =
		new GenericConverter<IEnumerable, object>((value, param) =>
		{
			if (value is null)
				return Visibility.Visible;
			return !value.Cast<object>().Any() ? Visibility.Visible : Visibility.Hidden;
		}, true);

	/// <summary>
	/// Multiply a double by -1.
	/// </summary>
	public static GenericConverter<double, object> MultiplyDoubleByNegativeOne { get; } =
		new GenericConverter<double, object>((value, param) => value * -1);

	/// <summary>
	/// Map null to false; non-null to true.
	/// </summary>
	public static GenericConverter<object, object> NullToFalse { get; } =
		new GenericConverter<object, object>((value, param) => !(value is null), true);

	/// <summary>
	/// Map null to true; non-null to false.
	/// </summary>
	public static GenericConverter<object, object> NullToTrue { get; } =
		new GenericConverter<object, object>((value, param) => value is null, true);

	/// <summary>
	/// And together visbility values or return visibility.collapsed if all are not visible.
	/// </summary>
	public static MultiValueGenericConverter<Visibility, object> VisibilityAndOrCollapsed { get; } =
		new MultiValueGenericConverter<Visibility, object>((values, parameter) => values.Any() && values.All(x => x == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed, false, Visibility.Collapsed);

	/// <summary>
	/// And together visbility values or return visibility.hidden if all are not visible.
	/// </summary>
	public static MultiValueGenericConverter<Visibility, object> VisibilityAndOrHidden { get; } =
		new MultiValueGenericConverter<Visibility, object>((values, parameter) => values.Any() && values.All(x => x == Visibility.Visible) ? Visibility.Visible : Visibility.Hidden, false, Visibility.Hidden);

	/// <summary>
	/// Or together visbility values or return visibility.collapsed if none are visible.
	/// </summary>
	public static MultiValueGenericConverter<Visibility, object> VisibilityOrOrCollapsed { get; } =
		new MultiValueGenericConverter<Visibility, object>((values, parameter) => values.Any() && values.Any(x => x == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed, false, Visibility.Collapsed);

	/// <summary>
	/// Or together visbility values or return visibility.hidden if none are visible.
	/// </summary>
	public static MultiValueGenericConverter<Visibility, object> VisibilityOrOrHidden { get; } =
		new MultiValueGenericConverter<Visibility, object>((values, parameter) => values.Any() && values.Any(x => x == Visibility.Visible) ? Visibility.Visible : Visibility.Hidden, false, Visibility.Hidden);
}
