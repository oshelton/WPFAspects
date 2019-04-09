using System;
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
    }
}
