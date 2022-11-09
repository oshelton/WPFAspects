using WPFAspects.Core;
using Xunit;

namespace UtilTests
{
	public class DirtyTrackerTests
	{
		private sealed class TestModel : Model
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
		public void TestBasicTracking()
		{
			var testModel = new TestModel();
			using var tracker = new DirtyTracker(testModel);

			Assert.False(tracker.IsDirty);

			testModel.PropertyOne = "Hello!";

			Assert.True(tracker.IsDirty);
		}

		[Fact]
		public void TestObjectReset()
		{
			var testModel = new TestModel();
			using var tracker = new DirtyTracker(testModel);

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
			using var tracker = new DirtyTracker(testModel);

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
		public void TestGetInitialValue()
		{
			var testModel = new TestModel();
			testModel.PropertyOne = "Hello!";
			testModel.PropertyTwo = "Prop 2";
			using var tracker = new DirtyTracker(testModel);

			testModel.PropertyOne = "new value";

			Assert.True(tracker.IsDirty);

			Assert.Equal("Hello!", tracker.GetInitialValueForProperty(nameof(TestModel.PropertyOne)));
			Assert.Null(tracker.GetInitialValueForProperty(nameof(TestModel.PropertyTwo)));
		}

		[Fact]
		public void TestPropertyGroups()
		{
			var testModel = new TestModel();
			using var tracker = new DirtyTracker(testModel);

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
			using var tracker = new DirtyTracker(testModel);

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
