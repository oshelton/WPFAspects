using System;
using System.ComponentModel;
using WPFAspects.Core;
using Xunit;

namespace UtilTests
{
    public class ModelTests
    {
        private class TestModel : Model
        {
            public TestModel() : base() { }

            private string _PropertyOne = null;
            public string PropertyOne
            {
                get => CheckIsOnMainThread(_PropertyOne);
                set => SetPropertyBackingValue(value, ref _PropertyOne);
            }

            private string _PropertyTwo = null;
            public string PropertyTwo
            {
                get => CheckIsOnMainThread(_PropertyTwo);
                set => SetPropertyBackingValue(value, ref _PropertyTwo); 
            }
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
}
