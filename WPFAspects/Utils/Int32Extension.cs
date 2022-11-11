using System.Windows.Markup;

namespace WPFAspects.Utils;

/// <summary>
/// Markup Extension for int values (say if you need to pass one to a converter).
/// </summary>
public class Int32Extension : MarkupExtension
{
	public Int32Extension(int value) => Value = value;

	public int Value { get; set; }
	public override object ProvideValue(IServiceProvider serviceProvider) => Value;
}
