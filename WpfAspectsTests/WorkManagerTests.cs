using System.Linq;
using System.Reflection;
using WPFAspects.Utils;
using Xunit;

namespace WpfAspects.Tests;

/// <summary>
/// Test WorkManager behavior.
/// </summary>
public class WorkManagerTests
{
	[Fact]
	public void TestInitialBehavior()
	{
		Assert.False(WorkManager.IsApplicationBlockingWorkInProgress);
		Assert.Null(WorkManager.ApplicationBlockingWorkTitle);
	}

	[Fact]
	public async void TestApplicationBlockingWork()
	{
		void TestAction()
		{
			Task.Delay(5000);
		}

		var workTask = WorkManager.StartApplicationBlockingWork(TestAction, "work item 1");
		Assert.True(WorkManager.IsApplicationBlockingWorkInProgress);
		Assert.Equal("work item 1", WorkManager.ApplicationBlockingWorkTitle);

		await workTask.ConfigureAwait(false);

		Assert.False(WorkManager.IsApplicationBlockingWorkInProgress);
		Assert.Null(WorkManager.ApplicationBlockingWorkTitle);

		Task TestFunc()
		{
			return Task.Delay(5000);
		}

		workTask = WorkManager.StartApplicationBlockingWork(TestFunc, "work item 2");
		Assert.True(WorkManager.IsApplicationBlockingWorkInProgress);
		Assert.Equal("work item 2", WorkManager.ApplicationBlockingWorkTitle);

		await workTask.ConfigureAwait(false);

		Assert.False(WorkManager.IsApplicationBlockingWorkInProgress);
		Assert.Null(WorkManager.ApplicationBlockingWorkTitle);
	}

	[Fact]
	public void TestStartScopedWork()
	{
		var scopedVar = 0;
		using (WorkManager.StartScopedWork(() => scopedVar = 1, () => scopedVar = -1))
		{
			Assert.Equal(1, scopedVar);
		}
		Assert.Equal(-1, scopedVar);
	}
}
