using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using WPFAspects.Converters;
using WPFAspects.Utils;
using Xunit;

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
			Assert.True((bool) CommonConverters.InvertBoolean.Convert(false, typeof(bool), null, CultureInfo.CurrentCulture));
			Assert.False((bool) CommonConverters.InvertBoolean.Convert(true, typeof(bool), null, CultureInfo.CurrentCulture));
			Assert.Equal(DependencyProperty.UnsetValue, CommonConverters.InvertBoolean.Convert(null, typeof(bool), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestObjectsAreEqual()
		{
			Assert.False((bool) CommonConverters.ObjectsAreEqual.Convert(0, typeof(object), 5, CultureInfo.CurrentCulture));
			Assert.True((bool) CommonConverters.ObjectsAreEqual.Convert(0, typeof(object), 0, CultureInfo.CurrentCulture));
			Assert.False((bool) CommonConverters.ObjectsAreEqual.Convert("Hi!", typeof(object), "Bye!", CultureInfo.CurrentCulture));
			Assert.True((bool) CommonConverters.ObjectsAreEqual.Convert("One", typeof(object), "One", CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestObjectsAreEqualOrCollapsed()
		{
			Assert.Equal(Visibility.Collapsed, CommonConverters.ObjectsAreEqualOrCollapsed.Convert(0, typeof(object), 5, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.ObjectsAreEqualOrCollapsed.Convert(0, typeof(object), 0, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Collapsed, CommonConverters.ObjectsAreEqualOrCollapsed.Convert("Hi!", typeof(object), "Bye!", CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.ObjectsAreEqualOrCollapsed.Convert("One", typeof(object), "One", CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestObjectsAreEqualOrHiden()
		{
			Assert.Equal(Visibility.Hidden, CommonConverters.ObjectsAreEqualOrHidden.Convert(0, typeof(object), 5, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.ObjectsAreEqualOrHidden.Convert(0, typeof(object), 0, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Hidden, CommonConverters.ObjectsAreEqualOrHidden.Convert("Hi!", typeof(object), "Bye!", CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.ObjectsAreEqualOrHidden.Convert("One", typeof(object), "One", CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestObjectsAreNotEqualOrCollapsed()
		{
			Assert.Equal(Visibility.Visible, CommonConverters.ObjectsAreNotEqualOrCollapsed.Convert(0, typeof(object), 5, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Collapsed, CommonConverters.ObjectsAreNotEqualOrCollapsed.Convert(0, typeof(object), 0, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.ObjectsAreNotEqualOrCollapsed.Convert("Hi!", typeof(object), "Bye!", CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Collapsed, CommonConverters.ObjectsAreNotEqualOrCollapsed.Convert("One", typeof(object), "One", CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestObjectsAreNotEqualOrHidden()
		{
			Assert.Equal(Visibility.Visible, CommonConverters.ObjectsAreNotEqualOrHidden.Convert(0, typeof(object), 5, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Hidden, CommonConverters.ObjectsAreNotEqualOrHidden.Convert(0, typeof(object), 0, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.ObjectsAreNotEqualOrHidden.Convert("Hi!", typeof(object), "Bye!", CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Hidden, CommonConverters.ObjectsAreNotEqualOrHidden.Convert("One", typeof(object), "One", CultureInfo.CurrentCulture));
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
			Assert.Equal(Visibility.Collapsed, CommonConverters.NullEmptyEnumerableToVisibilityCollapsed.Convert(Array.Empty<object>(), typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Collapsed, CommonConverters.NullEmptyEnumerableToVisibilityCollapsed.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.NullEmptyEnumerableToVisibilityCollapsed.Convert(new object[] { "Hi!" }, typeof(object), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestNonEmptyEnumerableToVisibilityCollapsed()
		{
			Assert.Equal(Visibility.Visible, CommonConverters.NonEmptyEnumerableToVisibilityCollapsed.Convert(Array.Empty<object>(), typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.NonEmptyEnumerableToVisibilityCollapsed.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Collapsed, CommonConverters.NonEmptyEnumerableToVisibilityCollapsed.Convert(new object[] { "Hi!" }, typeof(object), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestNonEmptyEnumerableToVisibilityHidden()
		{
			Assert.Equal(Visibility.Visible, CommonConverters.NonEmptyEnumerableToVisibilityHidden.Convert(Array.Empty<object>(), typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.NonEmptyEnumerableToVisibilityHidden.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Hidden, CommonConverters.NonEmptyEnumerableToVisibilityHidden.Convert(new object[] { "Hi!" }, typeof(object), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestNullEmptyEnumerableToVisibilityHidden()
		{
			Assert.Equal(Visibility.Hidden, CommonConverters.NullEmptyEnumerableToVisibilityHidden.Convert(Array.Empty<object>(), typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Hidden, CommonConverters.NullEmptyEnumerableToVisibilityHidden.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.NullEmptyEnumerableToVisibilityHidden.Convert(new object[] { "Hi!" }, typeof(object), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestMultiplyDoubleByNegativeOne()
		{
			Assert.Equal(10.0, CommonConverters.MultiplyDoubleByNegativeOne.Convert(-10.0, typeof(double), null, CultureInfo.CurrentCulture));
			Assert.Equal(-10.0, CommonConverters.MultiplyDoubleByNegativeOne.Convert(10.0, typeof(double), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestNullToFalse()
		{
			Assert.Equal(true, CommonConverters.NullToFalse.Convert("A", typeof(string), null, CultureInfo.CurrentCulture));
			Assert.Equal(false, CommonConverters.NullToFalse.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestNullToTrue()
		{
			Assert.Equal(false, CommonConverters.NullToTrue.Convert("A", typeof(string), null, CultureInfo.CurrentCulture));
			Assert.Equal(true, CommonConverters.NullToTrue.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestVisibilityAndOrCollapsed()
		{
			Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityAndOrCollapsed.Convert(Array.Empty<object>(), typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityAndOrCollapsed.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityAndOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Hidden }, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityAndOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.VisibilityAndOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Visible }, typeof(object), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestVisibilityAndOrHidden()
		{
			Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityAndOrHidden.Convert(Array.Empty<object>(), typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityAndOrHidden.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityAndOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Hidden }, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityAndOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.VisibilityAndOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Visible }, typeof(object), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestVisibilityOrOrCollapsed()
		{
			Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityOrOrCollapsed.Convert(Array.Empty<object>(), typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityOrOrCollapsed.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Collapsed, CommonConverters.VisibilityOrOrCollapsed.Convert(new object[] { Visibility.Hidden, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Hidden }, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrCollapsed.Convert(new object[] { Visibility.Visible, Visibility.Visible }, typeof(object), null, CultureInfo.CurrentCulture));
		}

		[Fact]
		public void TestVisibilityOrOrHidden()
		{
			Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityOrOrHidden.Convert(Array.Empty<object>(), typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityOrOrHidden.Convert(null, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Hidden, CommonConverters.VisibilityOrOrHidden.Convert(new object[] { Visibility.Hidden, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Hidden }, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Collapsed }, typeof(object), null, CultureInfo.CurrentCulture));
			Assert.Equal(Visibility.Visible, CommonConverters.VisibilityOrOrHidden.Convert(new object[] { Visibility.Visible, Visibility.Visible }, typeof(object), null, CultureInfo.CurrentCulture));
		}
	}
}
