using System.Windows;

namespace WPFAspects.Utils;

/// <summary>
/// Helper class for interacting with WPF's dispatcher.
/// </summary>
public static class DispatcherHelper
{
	/// <summary>
	/// Return true if called on the main thread.
	/// </summary>
	public static bool IsOnMainThread()
	{
		return Application.Current == null || Application.Current.Dispatcher.CheckAccess();
	}

	/// <summary>
	/// Blocking invoke the passed in Action if not called from the main thread.
	/// </summary>
	public static void InvokeIfNecessary(Action toExecute)
	{
		if (!IsOnMainThread())
			Application.Current.Dispatcher.Invoke(toExecute);
		else
			toExecute();
	}

	/// <summary>
	/// Invoke asynchronously the passed in Action if not called on the main thread.
	/// </summary>
	public static Task InvokeAsyncIfNecessary(Action toExecute)
	{
		if (!IsOnMainThread())
		{
			return Application.Current.Dispatcher.InvokeAsync(toExecute).Task;
		}
		else
		{
			toExecute();
			return Task.CompletedTask;
		}
	}
}
