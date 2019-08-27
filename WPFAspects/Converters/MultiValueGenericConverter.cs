﻿using System;
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
    /// <typeparam name="T">Desired type of the values to convert.</typeparam>
    /// <typeparam name="U">Desired typ of the conversion parameter.</typeparam>
    public class MultiValueGenericConverter<T, U> : IMultiValueConverter
    {
        public MultiValueGenericConverter(Func<T[], U, object> converterFunction, bool passThroughNull = false, object defaultValue = null)
        {
            _ConverterFunction = converterFunction ?? throw new ArgumentException(nameof(converterFunction));

            _PassThroughNull = passThroughNull;
            if (defaultValue == null)
                _DefaultValue = DependencyProperty.UnsetValue;
            else
                _DefaultValue = defaultValue;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((!(values is null) || _PassThroughNull) && (parameter == null || parameter is U))
                return _ConverterFunction(values.Cast<T>().ToArray(), (U)parameter);
            else
                return _DefaultValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Func<T[], U, object> _ConverterFunction = null;
        private bool _PassThroughNull;
        private object _DefaultValue;
    }
}
