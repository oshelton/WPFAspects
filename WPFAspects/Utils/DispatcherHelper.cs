using System;
using System.Windows;

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

        public static void InvokeAsyncIfNecessary(Action toExecute)
        {
            if (!IsOnMainThread())
                Application.Current.Dispatcher.InvokeAsync(toExecute);
            else
                toExecute();
        }
    }
}
