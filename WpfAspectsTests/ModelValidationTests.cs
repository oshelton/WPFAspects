using WPFAspects.Core;
using WPFAspects.Validation;
using WPFAspects.Validation.Rules;
using Xunit;

namespace WpfAspects.Tests;

/// <summary>
/// Test ModelValidation behavior.
/// </summary>
public class ModelValidationTests
{
	private class TestModel : ValidatedModel
	{
		public TestModel()
		{
			Validator = new TestValidator(this);
		}

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

		private class TestValidator : Validator<TestModel>
		{
			public TestValidator(TestModel model)
				: base(model)
			{
				foreach (var rule in Rules)
				{
					AddRule(rule);
				}
			}

			private static IEnumerable<Rule<TestModel>> Rules { get; } = new Rule<TestModel>[]
			{
				new CustomRule<TestModel>(nameof(PropertyOne), x => !string.IsNullOrEmpty(x.PropertyOne))
					.AlsoCheckOn(nameof(PropertyTwo))
					.WithOnFailMessage("Property One must have a value."),
			};
		}

		private string m_propertyOne;
		private string m_propertyTwo;
	}

	[Fact]
	public void TestSimplePropertyValidation()
	{
		var testModel = new TestModel();

		Assert.False(testModel.HasErrors);
		Assert.Equal(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
		testModel.Validator.CheckProperty(nameof(TestModel.PropertyOne));
		Assert.True(testModel.HasErrors);
		Assert.Equal(1, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
		Assert.Equal("Property One must have a value.", testModel.GetErrorsList(nameof(TestModel.PropertyOne))[0]);

		testModel.PropertyOne = "Hi!";
		Assert.False(testModel.HasErrors);
		Assert.Equal(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
	}

	[Fact]
	public void TestSimpleObjectValidation()
	{
		var testModel = new TestModel();

		Assert.False(testModel.HasErrors);
		Assert.Equal(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
		testModel.Validator.CheckObject();
		Assert.True(testModel.HasErrors);
		Assert.Equal(1, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
		Assert.Equal("Property One must have a value.", testModel.GetErrorsList(nameof(TestModel.PropertyOne))[0]);

		testModel.PropertyOne = "Hi!";
		Assert.False(testModel.HasErrors);
		Assert.Equal(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
	}

	[Fact]
	public void TestSimplePropertyDependency()
	{
		var testModel = new TestModel();

		Assert.False(testModel.HasErrors);
		Assert.Equal(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
		testModel.Validator.CheckProperty(nameof(TestModel.PropertyTwo));
		Assert.True(testModel.HasErrors);
		Assert.Equal(1, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
		Assert.Equal("Property One must have a value.", testModel.GetErrorsList(nameof(TestModel.PropertyOne))[0]);

		testModel.PropertyOne = "Hi!";
		testModel.Validator.CheckProperty(nameof(TestModel.PropertyTwo));
		Assert.False(testModel.HasErrors);
		Assert.Equal(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
	}

	[Fact]
	public void TestValidationEvents()
	{
		var testModel = new TestModel();

		var receivedErrorNotification = false;
		testModel.ErrorsChanged += (sender, args) =>
		{
			Assert.Equal(nameof(TestModel.PropertyOne), args.PropertyName);
			receivedErrorNotification = true;
		};

		var receivedValidationFailed = false;
		testModel.Validator.ValidationFail += (sender, args) =>
		{
			receivedValidationFailed = true;
		};

		var receivedPropertyValidationFailed = false;
		testModel.Validator.PropertyValidationFail += (sender, args) =>
		{
			Assert.Equal(nameof(TestModel.PropertyOne), args.PropertyName);
			receivedPropertyValidationFailed = true;
		};

		var receivedValidationSuccess = false;
		testModel.Validator.ValidationSuccess += (sender, args) =>
		{
			receivedValidationSuccess = true;
		};

		var receivedPropertyValidationSuccess = false;
		testModel.Validator.PropertyValidationSuccess += (sender, args) =>
		{
			Assert.Equal(nameof(TestModel.PropertyOne), args.PropertyName);
			receivedPropertyValidationSuccess = true;
		};

		Assert.False(receivedErrorNotification);
		Assert.False(receivedValidationFailed);
		Assert.False(receivedPropertyValidationFailed);
		testModel.Validator.CheckProperty(nameof(TestModel.PropertyTwo));
		Assert.True(receivedErrorNotification);
		Assert.True(receivedValidationFailed);
		Assert.True(receivedPropertyValidationFailed);

		testModel.PropertyOne = "Hi!";
		Assert.True(receivedValidationSuccess);
		Assert.True(receivedPropertyValidationSuccess);
	}
}
