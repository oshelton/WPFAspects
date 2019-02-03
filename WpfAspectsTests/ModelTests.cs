using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFAspects.Core;

namespace UtilTests
{
    [TestClass]
    public class ModelTests
    {
        private class TestModel : Model
        {
            public TestModel() : base() { }

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
        public void TestSetPropertyBackingValue()
        {
            var testModel = new TestModel();

            testModel.PropertyOne = "Hi!";
            Assert.AreEqual("Hi!", testModel.PropertyOne);
        }
        
        [TestMethod]
        public void TestPropertyChangingEvents()
        {
            var testModel = new TestModel();

            EventHandler<PropertyChangingWithValueEventArgs> changingWithValueHandler = (object sender, PropertyChangingWithValueEventArgs args) =>
            {
                Assert.AreEqual(null, args.PreviousValue);
                Assert.AreEqual(nameof(TestModel.PropertyOne), args.PropertyName);
            };
            PropertyChangingEventHandler changingHandler = (object sender, PropertyChangingEventArgs args) =>
            {
                Assert.AreEqual(null, testModel.PropertyOne);
                Assert.AreEqual(nameof(TestModel.PropertyOne), args.PropertyName);
            };

            testModel.PropertyChangingFromValue += changingWithValueHandler;
            testModel.PropertyChanging += changingHandler;

            testModel.PropertyOne = "Hi!";

            testModel.PropertyChangingFromValue -= changingWithValueHandler;
            testModel.PropertyChanging -= changingHandler;

            changingWithValueHandler = (object sender, PropertyChangingWithValueEventArgs args) =>
            {
                Assert.AreEqual("Hi!", args.PreviousValue);
                Assert.AreEqual(nameof(TestModel.PropertyOne), args.PropertyName);
            };
            changingHandler = (object sender, PropertyChangingEventArgs args) =>
            {
                Assert.AreEqual("Hi!", testModel.PropertyOne);
                Assert.AreEqual(nameof(TestModel.PropertyOne), args.PropertyName);
            };
        }
        
        [TestMethod]
        public void TestPropertyChangedEvents()
        {
            var testModel = new TestModel();

            EventHandler<PropertyChangedWithValueEventArgs> changedWithValueHandler = (object sender, PropertyChangedWithValueEventArgs args) =>
            {
                Assert.AreEqual("Hi!", args.NewValue);
                Assert.AreEqual(nameof(TestModel.PropertyOne), args.PropertyName);
            };
            PropertyChangedEventHandler changedHandler = (object sender, PropertyChangedEventArgs args) =>
            {
                Assert.AreEqual("Hi!", testModel.PropertyOne);
                Assert.AreEqual(nameof(TestModel.PropertyOne), args.PropertyName);
            };

            testModel.PropertyChangedToValue += changedWithValueHandler;
            testModel.PropertyChanged += changedHandler;

            testModel.PropertyOne = "Hi!";
        }
    }
}
