using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFAspects.Converters;

/// <summary>
/// Class for simple converters that can be implemented via a function passed in.
/// </summary>
/// <typeparam name="TValue">Desired type of the values to convert.</typeparam>
/// <typeparam name="TParameter">Desired typ of the conversion parameter.</typeparam>
public class MultiValueGenericConverter<TValue, TParameter> : IMultiValueConverter
{
	public MultiValueGenericConverter(Func<TValue[], TParameter, object> converterFunction, bool passThroughNull = false, object defaultValue = null)
	{
		m_converterFunction = converterFunction ?? throw new ArgumentNullException(nameof(converterFunction));

		m_passThroughNull = passThroughNull;
		if (defaultValue == null)
			m_defaultValue = DependencyProperty.UnsetValue;
		else
			m_defaultValue = defaultValue;
	}

	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
	{
		if ((values is not null || m_passThroughNull) && (parameter is null || parameter is TParameter))
			return m_converterFunction(values.Cast<TValue>().ToArray(), (TParameter) parameter);
		else
			return m_defaultValue;
	}

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

	private readonly Func<TValue[], TParameter, object> m_converterFunction;
	private readonly bool m_passThroughNull;
	private readonly object m_defaultValue;
}
