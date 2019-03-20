using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WPFAspects.Converters
{
    /// <summary>
    /// Class for simple converters that can be implemented via a function passed in.
    /// </summary>
    /// <typeparam name="T">Desired type of the value to convert.</typeparam>
    /// <typeparam name="U">Desired typ of the conversion parameter.</typeparam>
    public class GenericConverter<T, U> : IValueConverter
    {
        public GenericConverter(Func<T, U, object> converterFunction, object defaultValue = null)
        {
            if (converterFunction == null)
                throw new ArgumentException(nameof(converterFunction));

            _ConverterFunction = converterFunction;

            if (defaultValue == null)
                _DefaultValue = DependencyProperty.UnsetValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is T && (parameter == null || parameter is U))
                return _ConverterFunction((T)value, (U)parameter);
            else
                return _DefaultValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Func<T, U, object> _ConverterFunction = null;
        private object _DefaultValue;
    }
}
