using System.ComponentModel;
using WPFAspects.Core;
using Xunit;

namespace WpfAspects.Tests;

/// <summary>
/// Test core view model functionality.
/// </summary>
public class ModelTests
{
	private class TestModel : Model
	{
		public string PropertyOne
		{
			get => CheckIsOnMainThread(m_propertyOne);
			set => SetPropertyBackingValue(value, ref m_propertyOne);
		}

		public string PropertyTwo
		{
			get => CheckIsOnMainThread(m_propertyTwo);
			set => SetPropertyBackingValue(value, ref m_propertyTwo);
		}

		private string m_propertyOne;
		private string m_propertyTwo;
	}

	[Fact]
	public void TestSetPropertyBackingValue()
	{
		var testModel = new TestModel();

		testModel.PropertyOne = "Hi!";
		Assert.Equal("Hi!", testModel.PropertyOne);
	}

	[Fact]
	public void TestPropertyChangingEvents()
	{
		var testModel = new TestModel();

		EventHandler<PropertyChangingWithValueEventArgs> changingWithValueHandler = (object sender, PropertyChangingWithValueEventArgs args) =>
		{
			Assert.Null(args.PreviousValue);
			Assert.Equal(nameof(TestModel.PropertyOne), args.PropertyName);
		};
		PropertyChangingEventHandler changingHandler = (object sender, PropertyChangingEventArgs args) =>
		{
			Assert.Null(testModel.PropertyOne);
			Assert.Equal(nameof(TestModel.PropertyOne), args.PropertyName);
		};

		testModel.PropertyChangingFromValue += changingWithValueHandler;
		testModel.PropertyChanging += changingHandler;

		testModel.PropertyOne = "Hi!";

		testModel.PropertyChangingFromValue -= changingWithValueHandler;
		testModel.PropertyChanging -= changingHandler;

		changingWithValueHandler = (object sender, PropertyChangingWithValueEventArgs args) =>
		{
			Assert.Equal("Hi!", args.PreviousValue);
			Assert.Equal(nameof(TestModel.PropertyOne), args.PropertyName);
		};
		changingHandler = (object sender, PropertyChangingEventArgs args) =>
		{
			Assert.Equal("Hi!", testModel.PropertyOne);
			Assert.Equal(nameof(TestModel.PropertyOne), args.PropertyName);
		};

		testModel.PropertyOne = null;
	}

	[Fact]
	public void TestPropertyChangedEvents()
	{
		var testModel = new TestModel();

		EventHandler<PropertyChangedWithValueEventArgs> changedWithValueHandler = (object sender, PropertyChangedWithValueEventArgs args) =>
		{
			Assert.Equal("Hi!", args.NewValue);
			Assert.Equal(nameof(TestModel.PropertyOne), args.PropertyName);
		};
		PropertyChangedEventHandler changedHandler = (object sender, PropertyChangedEventArgs args) =>
		{
			Assert.Equal("Hi!", testModel.PropertyOne);
			Assert.Equal(nameof(TestModel.PropertyOne), args.PropertyName);
		};

		testModel.PropertyChangedToValue += changedWithValueHandler;
		testModel.PropertyChanged += changedHandler;

		testModel.PropertyOne = "Hi!";
	}
}
