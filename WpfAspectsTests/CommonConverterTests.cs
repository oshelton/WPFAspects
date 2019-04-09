using System.Linq;
using System.Reflection;
using Xunit;
using WPFAspects.Utils;
using WPFAspects.Converters;
using System.Windows;
using System.Globalization;

namespace UtilTests
{
    public class CommonConverterTests
    {
        [Fact]
        public void TestBooleanFalseToVisibilityHidden()
        {
            Assert.Equal(Visibility.Visible, CommonConverters.BooleanFalseToVisibilityHidden.Convert(true, typeof(Visibility), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Hidden, CommonConverters.BooleanFalseToVisibilityHidden.Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture));
            Assert.Equal(DependencyProperty.UnsetValue, CommonConverters.BooleanFalseToVisibilityHidden.Convert(null, typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestBooleanFalseToVisibilityCollapsed()
        {
            Assert.Equal(Visibility.Visible, CommonConverters.BooleanFalseToVisibilityCollapsed.Convert(true, typeof(Visibility), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Collapsed, CommonConverters.BooleanFalseToVisibilityCollapsed.Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture));
            Assert.Equal(DependencyProperty.UnsetValue, CommonConverters.BooleanFalseToVisibilityCollapsed.Convert(null, typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestBooleanTrueToVisibilityHidden()
        {
            Assert.Equal(Visibility.Hidden, CommonConverters.BooleanTrueToVisibilityHidden.Convert(true, typeof(Visibility), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.BooleanTrueToVisibilityHidden.Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture));
            Assert.Equal(DependencyProperty.UnsetValue, CommonConverters.BooleanTrueToVisibilityHidden.Convert(null, typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestBooleanTrueToVisibilityCollapsed()
        {
            Assert.Equal(Visibility.Collapsed, CommonConverters.BooleanTrueToVisibilityCollapsed.Convert(true, typeof(Visibility), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.BooleanTrueToVisibilityCollapsed.Convert(false, typeof(Visibility), null, CultureInfo.CurrentCulture));
            Assert.Equal(DependencyProperty.UnsetValue, CommonConverters.BooleanTrueToVisibilityCollapsed.Convert(null, typeof(Visibility), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestInvertBoolean()
        {
            Assert.True((bool)CommonConverters.InvertBoolean.Convert(false, typeof(bool), null, CultureInfo.CurrentCulture));
            Assert.False((bool)CommonConverters.InvertBoolean.Convert(true, typeof(bool), null, CultureInfo.CurrentCulture));
            Assert.Equal(DependencyProperty.UnsetValue, CommonConverters.InvertBoolean.Convert(null, typeof(bool), null, CultureInfo.CurrentCulture));
        }
    }
}
