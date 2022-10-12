using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WPFAspects.Utils;

/// <summary>
/// Static class for managing some scoped and application blocking work.
/// </summary>
public static class WorkManager
{
	/// <summary>
	/// Whether or not work that is blocking is currently in progress.
	/// </summary>
	public static bool IsApplicationBlockingWorkInProgress
	{
		get => s_isApplicationBlockingWorkInProgress;
		set
		{
			if (value != s_isApplicationBlockingWorkInProgress)
			{
				s_isApplicationBlockingWorkInProgress = value;
				IsApplicationBlockingWorkInProgressChanged?.Invoke(null, EventArgs.Empty);
			}
		}
	}

	/// <summary>
	/// Title to use for the progress idicator.
	/// </summary>
	public static string ApplicationBlockingWorkTitle
	{
		get => s_applicationBlockingWorkTitle;
		set
		{
			if (s_applicationBlockingWorkTitle != value)
			{
				s_applicationBlockingWorkTitle = value;
				ApplicationBlockingWorkTitleChanged?.Invoke(null, EventArgs.Empty);
			}
		}
	}

	/// <summary>
	/// Start scoped work, should be called in a using ().
	/// </summary>
	/// <param name="workStartAction">Work to be done to kick things off.</param>
	/// <param name="workDoneAction">Work to be done when the scope is left/disposed of.</param>
	public static ScopedWork StartScopedWork(Action workStartAction, Action workDoneAction)
	{
		return new ScopedWork(workStartAction, workDoneAction);
	}

	/// <summary>
	/// Start application blocking work.
	/// </summary>
	public static async Task StartApplicationBlockingWork(Action work, string workTitle)
	{
		if (work is null)
			throw new ArgumentNullException(nameof(work));
		if (string.IsNullOrWhiteSpace(workTitle))
			throw new ArgumentNullException(nameof(workTitle));

		if (IsApplicationBlockingWorkInProgress)
			throw new InvalidOperationException("Application blocking work is already in progress; more cannot be started until this finishes.");

		IsApplicationBlockingWorkInProgress = true;
		ApplicationBlockingWorkTitle = workTitle;

		await Task.Run(work).ConfigureAwait(false);

		Dispatcher.CurrentDispatcher.Invoke(() => IsApplicationBlockingWorkInProgress = false);
	}

	/// <summary>
	/// Start application blocking work with a method that returns a Task.
	/// </summary>
	public static async Task StartApplicationBlockingWork(Func<Task> work, string workTitle)
	{
		if (work is null)
			throw new ArgumentNullException(nameof(work));
		if (string.IsNullOrWhiteSpace(workTitle))
			throw new ArgumentNullException(nameof(workTitle));

		if (IsApplicationBlockingWorkInProgress)
			throw new InvalidOperationException("Application blocking work is already in progress; more cannot be started until this finishes.");

		IsApplicationBlockingWorkInProgress = true;
		ApplicationBlockingWorkTitle = workTitle;

		await work().ConfigureAwait(false);

		Dispatcher.CurrentDispatcher.Invoke(() => IsApplicationBlockingWorkInProgress = false);
	}

	public static event EventHandler IsApplicationBlockingWorkInProgressChanged;
	public static event EventHandler ApplicationBlockingWorkTitleChanged;

	private static bool s_isApplicationBlockingWorkInProgress;
	private static string s_applicationBlockingWorkTitle;
}
