using Xunit;
using WPFAspects.Core;

namespace UtilTests
{
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
        
        [Fact]
        public void TestBasicTracking()
        {
            var testModel = new TestModel();
            var tracker = new DirtyTracker(testModel);

            Assert.False(tracker.IsDirty);

            testModel.PropertyOne = "Hello!";

            Assert.True(tracker.IsDirty);
        }

        [Fact]
        public void TestObjectReset()
        {
            var testModel = new TestModel();
            var tracker = new DirtyTracker(testModel);

            Assert.False(tracker.IsDirty);

            testModel.PropertyOne = "Hello!";

            Assert.True(tracker.IsDirty);

            tracker.ResetToInitialState();

            Assert.False(tracker.IsDirty);
            Assert.Null(testModel.PropertyOne);

            testModel.PropertyOne = "Hi!";
            tracker.SetInitialState();

            Assert.Equal("Hi!", testModel.PropertyOne);
            Assert.False(false);
        }

        [Fact]
        public void TestPropertyReset()
        {
            var testModel = new TestModel();
            var tracker = new DirtyTracker(testModel);

            Assert.False(tracker.IsDirty);

            testModel.PropertyOne = "Hello!";
            testModel.PropertyTwo = "Prop 2";

            Assert.True(tracker.IsDirty);

            tracker.ResetPropertyToInitialSTate(nameof(TestModel.PropertyOne));

            Assert.False(tracker.IsPropertyDirty(nameof(TestModel.PropertyOne)));
            Assert.True(tracker.IsPropertyDirty(nameof(TestModel.PropertyTwo)));
            Assert.True(tracker.IsDirty);
            Assert.Null(testModel.PropertyOne);
            Assert.Equal("Prop 2", testModel.PropertyTwo);

            tracker.ResetPropertyToInitialSTate(nameof(TestModel.PropertyTwo));

            Assert.False(tracker.IsDirty);
            Assert.Null(testModel.PropertyOne);
            Assert.Null(testModel.PropertyTwo);
        }

        [Fact]
        public void TestPropertyGroups()
        {
            var testModel = new TestModel();
            var tracker = new DirtyTracker(testModel);

            var group1 = tracker.CreateDirtyTrackingGroup("group", nameof(TestModel.PropertyOne));

            testModel.PropertyOne = "New Value";

            Assert.True(group1.IsDirty);
            Assert.True(tracker.IsDirty);

            testModel.PropertyTwo = "Other Value";

            Assert.True(group1.IsDirty);
            Assert.True(tracker.IsDirty);

            testModel.PropertyOne = null;

            Assert.False(group1.IsDirty);
            Assert.True(tracker.IsDirty);

            testModel.PropertyTwo = null;

            Assert.False(group1.IsDirty);
            Assert.False(tracker.IsDirty);
        }

        [Fact]
        public void TestResetPropertyGroups()
        {
            var testModel = new TestModel();
            var tracker = new DirtyTracker(testModel);

            var group1 = tracker.CreateDirtyTrackingGroup("group", nameof(TestModel.PropertyOne));

            testModel.PropertyOne = "New Value";

            Assert.True(group1.IsDirty);
            Assert.True(tracker.IsDirty);

            tracker.ResetToInitialState();

            Assert.False(group1.IsDirty);
            Assert.False(tracker.IsDirty);
            
        }
    }
}
