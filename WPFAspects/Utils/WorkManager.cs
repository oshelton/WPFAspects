using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace WPFAspects.Utils
{
    /// <summary>
    /// Static class for anaging some scoped and application blocking work.
    /// </summary>
    public static class WorkManager
    {
        /// <summary>
        /// Simple class for representing scoped work; ie, work that needs to do something when it is done.
        /// </summary>
        public sealed class ScopedWork: IDisposable
        {
            public ScopedWork(Action workStartsAction, Action workDoneAction)
            {
                workStartsAction?.Invoke();

                WorkDoneAction = workDoneAction;
            }

            public void Dispose()
            {
                WorkDoneAction?.Invoke();
            }
            
            private readonly Action WorkDoneAction;
        }

        private static bool _IsApplicationBlockingWorkInProgress = false;
        /// <summary>
        /// Whether or not work that is blocking is currently in progress.
        /// </summary>
        public static bool IsApplicationBlockingWorkInProgress
        {
            get => _IsApplicationBlockingWorkInProgress;
            set
            {
                if (value != _IsApplicationBlockingWorkInProgress)
                {
                    _IsApplicationBlockingWorkInProgress = value;
                    IsApplicationBlockingWorkInProgressChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        private static string _ApplicationBlockingWorkTitle;
        /// <summary>
        /// Title to use for the progress idicator.
        /// </summary>
        public static string ApplicationBlockingWorkTitle
        {
            get => _ApplicationBlockingWorkTitle;
            set
            {
                if (_ApplicationBlockingWorkTitle != value)
                {
                    _ApplicationBlockingWorkTitle = value;
                    ApplicationBlockingWorkTitleChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Start scoped work, should be called in a using ().
        /// </summary>
        /// <param name="workStartAction">Work to be done to kick things off.</param>
        /// <param name="workDoneAction">Work to be done when the scope is left/disposed of.</param>
        /// <returns></returns>
        public static ScopedWork StartScopedWork(Action workStartAction, Action workDoneAction)
        {
            return new ScopedWork(workStartAction, workDoneAction);
        }

        /// <summary>
        /// Start application blocking work.
        /// </summary>
        public static async Task StartApplicationBlockingWork(Action work, string workTitle)
        {
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
            if (IsApplicationBlockingWorkInProgress)
                throw new InvalidOperationException("Application blocking work is already in progress; more cannot be started until this finishes.");

            IsApplicationBlockingWorkInProgress = true;
            ApplicationBlockingWorkTitle = workTitle;

            await work();

            Dispatcher.CurrentDispatcher.Invoke(() => IsApplicationBlockingWorkInProgress = false);
        }
        
        public static event EventHandler IsApplicationBlockingWorkInProgressChanged;
        public static event EventHandler ApplicationBlockingWorkTitleChanged;
    }
}
