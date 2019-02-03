using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WPFAspects.Core;

namespace UtilTests
{
    [TestClass]
    public class DirtyTrackerTests
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
        public void TestBasicTracking()
        {
            var testModel = new TestModel();
            var tracker = new DirtyTracker(testModel);

            Assert.AreEqual(false, tracker.IsDirty);

            testModel.PropertyOne = "Hello!";

            Assert.AreEqual(true, tracker.IsDirty);
        }

        [TestMethod]
        public void TestObjectReset()
        {
            var testModel = new TestModel();
            var tracker = new DirtyTracker(testModel);

            Assert.AreEqual(false, tracker.IsDirty);

            testModel.PropertyOne = "Hello!";

            Assert.AreEqual(true, tracker.IsDirty);

            tracker.ResetToInitialState();

            Assert.AreEqual(false, tracker.IsDirty);
            Assert.AreEqual(null, testModel.PropertyOne);

            testModel.PropertyOne = "Hi!";
            tracker.SetInitialState();

            Assert.AreEqual("Hi!", testModel.PropertyOne);
            Assert.AreEqual(false, tracker.IsDirty);
        }

        [TestMethod]
        public void TestPropertyReset()
        {
            var testModel = new TestModel();
            var tracker = new DirtyTracker(testModel);

            Assert.AreEqual(false, tracker.IsDirty);

            testModel.PropertyOne = "Hello!";
            testModel.PropertyTwo = "Prop 2";

            Assert.AreEqual(true, tracker.IsDirty);

            tracker.ResetPropertyToInitialSTate(nameof(TestModel.PropertyOne));

            Assert.AreEqual(true, tracker.IsDirty);
            Assert.AreEqual(null, testModel.PropertyOne);
            Assert.AreEqual("Prop 2", testModel.PropertyTwo);

            tracker.ResetPropertyToInitialSTate(nameof(TestModel.PropertyTwo));

            Assert.AreEqual(false, tracker.IsDirty);
            Assert.AreEqual(null, testModel.PropertyOne);
            Assert.AreEqual(null, testModel.PropertyTwo);
        }
    }
}
