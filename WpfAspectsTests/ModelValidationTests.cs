using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFAspects.Core;
using WPFAspects.Utils;
using WPFAspects.Validation;
using WPFAspects.Validation.Rules;

namespace UtilTests
{
    [TestClass]
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
        
        [TestMethod]
        public void TestSimplePropertyValidation()
        {
            var testModel = new TestModel();

            Assert.AreEqual(false, testModel.HasErrors);
            Assert.AreEqual(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
            testModel.Validator.CheckProperty(nameof(TestModel.PropertyOne));
            Assert.AreEqual(true, testModel.HasErrors);
            Assert.AreEqual(1, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
            Assert.AreEqual("Property One must have a value.", testModel.GetErrorsList(nameof(TestModel.PropertyOne))[0]);

            testModel.PropertyOne = "Hi!";
            Assert.AreEqual(false, testModel.HasErrors);
            Assert.AreEqual(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
        }

        [TestMethod]
        public void TestSimpleObjectValidation()
        {
            var testModel = new TestModel();

            Assert.AreEqual(false, testModel.HasErrors);
            Assert.AreEqual(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
            testModel.Validator.CheckObject();
            Assert.AreEqual(true, testModel.HasErrors);
            Assert.AreEqual(1, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
            Assert.AreEqual("Property One must have a value.", testModel.GetErrorsList(nameof(TestModel.PropertyOne))[0]);

            testModel.PropertyOne = "Hi!";
            Assert.AreEqual(false, testModel.HasErrors);
            Assert.AreEqual(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
        }

        [TestMethod]
        public void TestSimplePropertyDependency()
        {
            var testModel = new TestModel();

            Assert.AreEqual(false, testModel.HasErrors);
            Assert.AreEqual(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
            testModel.Validator.CheckProperty(nameof(TestModel.PropertyTwo));
            Assert.AreEqual(true, testModel.HasErrors);
            Assert.AreEqual(1, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
            Assert.AreEqual("Property One must have a value.", testModel.GetErrorsList(nameof(TestModel.PropertyOne))[0]);

            testModel.PropertyOne = "Hi!";
            testModel.Validator.CheckProperty(nameof(TestModel.PropertyTwo));
            Assert.AreEqual(false, testModel.HasErrors);
            Assert.AreEqual(0, testModel.GetErrorsList(nameof(TestModel.PropertyOne)).Count);
        }

        [TestMethod]
        public void TestValidationEvents()
        {
            var testModel = new TestModel();

            bool receivedErrorNotification = false;
            testModel.ErrorsChanged += (sender, args) =>
            {
                Assert.AreEqual(nameof(TestModel.PropertyOne), args.PropertyName);
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
                Assert.AreEqual(nameof(TestModel.PropertyOne), propertyName);
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
                Assert.AreEqual(nameof(TestModel.PropertyOne), propertyName);
                receivedPropertyValidationSuccess = true;
            };

            Assert.AreEqual(false, receivedErrorNotification);
            Assert.AreEqual(false, receivedValidationFailed);
            Assert.AreEqual(false, receivedPropertyValidationFailed);
            testModel.Validator.CheckProperty(nameof(TestModel.PropertyTwo));
            Assert.AreEqual(true, receivedErrorNotification);
            Assert.AreEqual(true, receivedValidationFailed);
            Assert.AreEqual(true, receivedPropertyValidationFailed);

            testModel.PropertyOne = "Hi!";
            Assert.AreEqual(true, receivedValidationSuccess);
            Assert.AreEqual(true, receivedPropertyValidationSuccess);
        }
    }
}
