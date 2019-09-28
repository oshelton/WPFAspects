using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFAspects.Converters
{
    /// <summary>
    /// Class to provide simple access to a host of commonly used value converters.
    /// </summary>
    public static class CommonConverters
    {
        /// Convert boolean false values to Visibility.Hidden and true to Visibility.Visible.
        public static GenericConverter<bool, object> BooleanFalseToVisibilityHidden { get; } =
            new GenericConverter<bool, object>((value, param) => value ? Visibility.Visible : Visibility.Hidden);

        /// Convert boolean false values to Visibility.Collapsed and true to Visibility.Visible.
        public static GenericConverter<bool, object> BooleanFalseToVisibilityCollapsed { get; } =
            new GenericConverter<bool, object>((value, param) => value ? Visibility.Visible : Visibility.Collapsed);

        /// Convert boolean false values to Visibility.Visible and true to Visibility.Hidden.
        public static GenericConverter<bool, object> BooleanTrueToVisibilityHidden { get; } =
            new GenericConverter<bool, object>((value, param) => value ? Visibility.Hidden : Visibility.Visible);

        /// Convert boolean false values to Visibility.Visible and true to Visibility.Collapsed.
        public static GenericConverter<bool, object> BooleanTrueToVisibilityCollapsed { get; } =
            new GenericConverter<bool, object>((value, param) => value ? Visibility.Collapsed : Visibility.Visible);

        /// Convert boolean false values to Visibility.Visible and true to Visibility.Collapsed.
        public static GenericConverter<bool, object> InvertBoolean { get; } =
            new GenericConverter<bool, object>((value, param) => !value);

        /// Returns true if the passed in objects are equal.
        public static GenericConverter<object, object> ObjectsAreEqual { get; } =
            new GenericConverter<object, object>((value, param) => value.Equals(param));

        /// Returns Visible if the passed in objects are equal or Collapsed.
        public static GenericConverter<object, object> ObjectsAreEqualOrCollapsed { get; } =
            new GenericConverter<object, object>((value, param) => value.Equals(param) ? Visibility.Visible : Visibility.Collapsed);

        /// Returns Visible if the passed in objects are equal or Hidden.
        public static GenericConverter<object, object> ObjectsAreEqualOrHidden { get; } =
            new GenericConverter<object, object>((value, param) => value.Equals(param) ? Visibility.Visible : Visibility.Hidden);

        /// Returns Visible if the passed in objects are not equal or Collapsed.
        public static GenericConverter<object, object> ObjectsNotEqualOrCollapsed { get; } =
            new GenericConverter<object, object>((value, param) => !value.Equals(param) ? Visibility.Visible : Visibility.Collapsed);

        /// Returns Visible if the passed in objects are not equal or Hidden.
        public static GenericConverter<object, object> ObjectsNotEqualOrHidden { get; } =
            new GenericConverter<object, object>((value, param) => !value.Equals(param) ? Visibility.Visible : Visibility.Hidden);

        /// Returns visibility collapsed if the input value is equal to null.
        public static GenericConverter<object, object> NullToVisibilityCollapsed { get; } =
            new GenericConverter<object, object>((value, param) => value is null ? Visibility.Collapsed : Visibility.Visible, true);

        /// Returns visibility collapsed if the input value is null or empty if it is an IEnumerable.
        public static GenericConverter<IEnumerable, object> NullEmptyEnumerableToVisibilityCollapsed { get; } =
            new GenericConverter<IEnumerable, object>((value, param) =>
            {
                if (value is null)
                    return Visibility.Collapsed;
                return value.Cast<object>().Any() ? Visibility.Visible : Visibility.Collapsed;
            }, true);

        /// Returns visibility collapsed if the input value is a non-empty enumerable.
        public static GenericConverter<IEnumerable, object> NonEmptyEnumerableToVisibilityCollapsed { get; } =
            new GenericConverter<IEnumerable, object>((value, param) =>
            {
                if (value is null)
                    return Visibility.Visible;
                return !value.Cast<object>().Any() ? Visibility.Visible : Visibility.Collapsed;
            }, true);

        /// Returns visibility hidden if the input value is equal to null.
        public static GenericConverter<object, object> NullToVisibilityHidden { get; } =
            new GenericConverter<object, object>((value, param) => value is null ? Visibility.Hidden : Visibility.Visible, true);

        /// Returns visibility hidden if the input value is null or empty if it is an IEnumerable.
        public static GenericConverter<IEnumerable, object> NullEmptyEnumerableToVisibilityHidden { get; } =
            new GenericConverter<IEnumerable, object>((value, param) =>
            {
                if (value is null)
                    return Visibility.Hidden;
                return value.Cast<object>().Any() ? Visibility.Visible : Visibility.Hidden;
            }, true);

        /// Returns visibility collapsed if the input value is a non-empty enumerable.
        public static GenericConverter<IEnumerable, object> NonEmptyEnumerableToVisibilityHidden { get; } =
            new GenericConverter<IEnumerable, object>((value, param) =>
            {
                if (value is null)
                    return Visibility.Visible;
                return !value.Cast<object>().Any() ? Visibility.Visible : Visibility.Hidden;
            }, true);

        /// Multiply a double by -1.
        public static GenericConverter<double, object> MultiplyDoubleByNegativeOne { get; } =
			new GenericConverter<double, object>((value, param) => value * -1);

        /// And together visbility values or return visibility.collapsed if all are not visible.
        public static MultiValueGenericConverter<Visibility, object> VisibilityAndOrCollapsed { get; } =
            new MultiValueGenericConverter<Visibility, object>((values, parameter) => values.Any() && values.All(x => x == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed, false, Visibility.Collapsed);

        /// And together visbility values or return visibility.hidden if all are not visible.
        public static MultiValueGenericConverter<Visibility, object> VisibilityAndOrHidden { get; } =
            new MultiValueGenericConverter<Visibility, object>((values, parameter) => values.Any() && values.All(x => x == Visibility.Visible) ? Visibility.Visible : Visibility.Hidden, false, Visibility.Hidden);

        /// Or together visbility values or return visibility.collapsed if none are visible.
        public static MultiValueGenericConverter<Visibility, object> VisibilityOrOrCollapsed { get; } =
            new MultiValueGenericConverter<Visibility, object>((values, parameter) => values.Any() && values.Any(x => x == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed, false, Visibility.Collapsed);

        /// Or together visbility values or return visibility.hidden if none are visible.
        public static MultiValueGenericConverter<Visibility, object> VisibilityOrOrHidden { get; } =
            new MultiValueGenericConverter<Visibility, object>((values, parameter) => values.Any() && values.Any(x => x == Visibility.Visible) ? Visibility.Visible : Visibility.Hidden, false, Visibility.Hidden);
    }
}
