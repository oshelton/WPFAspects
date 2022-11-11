using System.Linq;
using System.Reflection;
using WPFAspects.Utils;
using Xunit;

namespace WpfAspects.Tests;

/// <summary>
/// Tesst property getter behavior.
/// </summary>
public class PropertyGetterTests
{
	private class TestObject
	{
		public static bool StaticVarOne { get; set; } = true;
		private static bool StaticVarTwo { get; set; }

		public int VarOne { get; set; }
		internal int VarTwo { get; set; } = 1;
		protected int VarThree { get; set; } = 2;
		private int VarFour { get; set; } = 3;
	}

	// Test default PropertyGetter.GetProperties behavior; should only return the VarOne property.
	[Fact]
	public void TestDefaultBindingFlags()
	{
		var testObject = new TestObject();
		var properties = testObject.GetProperties();

		Assert.Single(properties);
		Assert.Equal("VarOne", properties.First().Name);
	}

	// Test more advanced PropertyGetter.GetProperties behavior; should just return the StaticVarOne property.
	[Fact]
	public void TestAdvancedBindingFlags()
	{
		var testObject = new TestObject();
		var properties = testObject.GetProperties(BindingFlags.Static | BindingFlags.Public);

		Assert.Single(properties);
		Assert.Contains(properties, p => p.Name == nameof(TestObject.StaticVarOne));
	}

	[Fact]
	public void TestGetValue()
	{
		var testObject = new TestObject();

		Assert.True(testObject.GetPropertyValue<bool>(nameof(TestObject.StaticVarOne), BindingFlags.Static | BindingFlags.Public));
		Assert.Equal(0, testObject.GetPropertyValue<int>(nameof(TestObject.VarOne)));
	}

	[Fact]
	public void TestSetValue()
	{
		var testObject = new TestObject();

		testObject.SetPropertyValue(nameof(TestObject.VarOne), 2);
		Assert.Equal(2, testObject.VarOne);
		testObject.SetPropertyValue(nameof(testObject.VarTwo), 3, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.Equal(3, testObject.VarTwo);

		TestObject.StaticVarOne = true;
		Assert.True(testObject.GetPropertyValue<bool>(nameof(TestObject.StaticVarOne), BindingFlags.Static | BindingFlags.Public));
		testObject.SetPropertyValue(nameof(TestObject.StaticVarOne), false, BindingFlags.Static | BindingFlags.Public);
		Assert.False(testObject.GetPropertyValue<bool>(nameof(TestObject.StaticVarOne), BindingFlags.Static | BindingFlags.Public));
		testObject.SetPropertyValue(nameof(TestObject.StaticVarOne), true, BindingFlags.Static | BindingFlags.Public);
		Assert.True(testObject.GetPropertyValue<bool>(nameof(TestObject.StaticVarOne), BindingFlags.Static | BindingFlags.Public));
	}
}
