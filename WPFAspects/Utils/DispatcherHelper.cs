using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WPFAspects.Utils
{
    public static class DispatcherHelper
    {
        public static bool IsOnMainThread()
        {
            return Application.Current == null || Application.Current.Dispatcher.CheckAccess();
        }

        public static void InvokeIfNecessary(Action toExecute)
        {
            if (!IsOnMainThread())
                Application.Current.Dispatcher.Invoke(toExecute);
            else
                toExecute();
        }

        public static Task InvokeAsyncIfNecessary(Action toExecute)
        {
            if (!IsOnMainThread())
                return Application.Current.Dispatcher.InvokeAsync(toExecute).Task;
            else
            {
                toExecute();
                return Task.CompletedTask;
            }
        }
    }
}
