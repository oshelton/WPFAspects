using System.Collections.Generic;
using WPFAspects.Core;
using WPFAspects.Validation;
using WPFAspects.Validation.Rules;
using Xunit;

namespace UtilTests
{
    public class ModelValidationTests
    {
        private class TestModel : ValidatedModel
        {
            #region Validation stuff.
            private class TestValidator: Validator<TestModel>
            {
                private static IEnumerable<Rule<TestModel>> _Rules = null;
                private static IEnumerable<Rule<TestModel>> GetRules()
                {
                    if (_Rules == null)
                    {
                        _Rules = new Rule<TestModel>[]
                        {
                            new CustomRule<TestModel>(nameof(PropertyOne), x => !string.IsNullOrEmpty(x.PropertyOne))
                                .AlsoCheckOn(nameof(TestModel.PropertyTwo))
                                .WithOnFailMessage("Property One must have a value.")
                        };
                    }

                    return _Rules;
                }

                public TestValidator(TestModel model): base(model)
                {
                    foreach (var rule in GetRules())
                    {
                        AddRule(rule);
                    }
                }
            }

            protected override Validator GetValidator()
            {
                return new TestValidator(this);
            }
            #endregion

            private string _PropertyOne = null;
            public string PropertyOne
            {
                get => _PropertyOne;
                set => SetPropertyBackingValue(value, ref _PropertyOne);
            }

            private string _PropertyTwo = null;
            public string PropertyTwo
            {
                get => _PropertyTwo;
                set => SetPropertyBackingValue(value, ref _PropertyTwo);
            }
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

            bool receivedErrorNotification = false;
            testModel.ErrorsChanged += (sender, args) =>
            {
                Assert.Equal(nameof(TestModel.PropertyOne), args.PropertyName);
                receivedErrorNotification = true;
            };

            bool receivedValidationFailed = false;
            testModel.Validator.ValidationFail += (sender) =>
            {
                receivedValidationFailed = true;
            };

            bool receivedPropertyValidationFailed = false;
            testModel.Validator.PropertyValidationFail += (sender, propertyName, value) =>
            {
                Assert.Equal(nameof(TestModel.PropertyOne), propertyName);
                receivedPropertyValidationFailed = true;
            };

            bool receivedValidationSuccess = false;
            testModel.Validator.ValidationSuccess += (sender) =>
            {
                receivedValidationSuccess = true;
            };

            bool receivedPropertyValidationSuccess = false;
            testModel.Validator.PropertyValidationSuccess += (sender, propertyName, value) =>
            {
                Assert.Equal(nameof(TestModel.PropertyOne), propertyName);
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
}
