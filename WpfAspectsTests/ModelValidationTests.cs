using System;
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
                public TestValidator(TestModel model): base(model)
                {
                    AddRule(new CustomRule<TestModel>(nameof(PropertyOne), x => !string.IsNullOrEmpty(x.PropertyOne)))
                        .AlsoCheckOn(nameof(TestModel.PropertyTwo))
                        .WithOnFailMessage("Property One must have a value.");
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
    }
}
