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

        [Fact]
        public void TestObjectsAreEqual()
        {
            Assert.False((bool)CommonConverters.ObjectsAreEqual.Convert(0, typeof(object), 5, CultureInfo.CurrentCulture));
            Assert.True((bool)CommonConverters.ObjectsAreEqual.Convert(0, typeof(object), 0, CultureInfo.CurrentCulture));
            Assert.False((bool)CommonConverters.ObjectsAreEqual.Convert("Hi!", typeof(object), "Bye!", CultureInfo.CurrentCulture));
            Assert.True((bool)CommonConverters.ObjectsAreEqual.Convert("One", typeof(object), "One", CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestNullToCollapsed()
        {
            Assert.Equal(Visibility.Collapsed, CommonConverters.NullToVisibilityCollapsed.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.NullToVisibilityCollapsed.Convert("Word!", typeof(object), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestNullToHidden()
        {
            Assert.Equal(Visibility.Hidden, CommonConverters.NullToVisibilityHidden.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.NullToVisibilityHidden.Convert("Word!", typeof(object), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestNullEmptyEnumerableToVisibilityCollapsed()
        {
            Assert.Equal(Visibility.Collapsed, CommonConverters.NullEmptyEnumerableToVisibilityCollapsed.Convert(new object[0], typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Collapsed, CommonConverters.NullEmptyEnumerableToVisibilityCollapsed.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.NullEmptyEnumerableToVisibilityCollapsed.Convert(new object[] { "Hi!" }, typeof(object), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestNullEmptyEnumerableToVisibilityHidden()
        {
            Assert.Equal(Visibility.Hidden, CommonConverters.NullEmptyEnumerableToVisibilityHidden.Convert(new object[0], typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Hidden, CommonConverters.NullEmptyEnumerableToVisibilityHidden.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.NullEmptyEnumerableToVisibilityHidden.Convert(new object[] { "Hi!" }, typeof(object), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestVisibilityAndOrCollapsed()
        {
            Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityAndOrCollapsed.Convert(new object[0], typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityAndOrCollapsed.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityAndOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Hidden }, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityAndOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.VisibilityAndOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Visible }, typeof(object), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestVisibilityAndOrHidden()
        {
            Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityAndOrHidden.Convert(new object[0], typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityAndOrHidden.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityAndOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Hidden }, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityAndOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.VisibilityAndOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Visible }, typeof(object), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestVisibilityOrOrCollapsed()
        {
            Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityOrOrCollapsed.Convert(new object[0], typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityOrOrCollapsed.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityOrOrCollapsed.Convert(new object[] { Visibility.Hidden, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Hidden }, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Visible }, typeof(object), null, CultureInfo.CurrentCulture));
        }

        [Fact]
        public void TestVisibilityOrOrHidden()
        {
            Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityOrOrHidden.Convert(new object[0], typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityOrOrHidden.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityOrOrHidden.Convert(new object[] { Visibility.Hidden, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Hidden }, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
            Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Visible }, typeof(object), null, CultureInfo.CurrentCulture));
        }
    }
}
