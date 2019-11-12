// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CameraTaskEngine.cs">
//   Copyright (c) 2014 Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Use of this sample source code is subject to the terms of the Microsoft license
//   agreement under which you licensed this sample source code and is provided AS-IS.
//   If you did not accept the terms of the license agreement, you are not authorized
//   to use this sample source code. For the terms of the license, please see the
//   license agreement between you and Microsoft.<br/><br/>
//   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604.
// </summary>
// ----------------------------------------------------------------------------

namespace CameraSampleCS.Models.Camera
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Models.Camera.Tasks;

    /// <summary>
    /// Provides methods for camera tasks scheduling and execution.
    /// </summary>
    public sealed class CameraTaskEngine
    {
        #region Fields

        /// <summary>
        /// Synchronization root.
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// Contains pending tasks.
        /// </summary>
        private readonly List<QueuedCameraTask> pendingTasks = new List<QueuedCameraTask>();

        /// <summary>
        /// Active task.
        /// </summary>
        private volatile QueuedCameraTask currentTask;

        #endregion // Fields

        #region Public methods

        /// <summary>
        /// Synchronously adds the <paramref name="cameraTask"/> given to an execution queue
        /// and returns awaitable task to wait for the <paramref name="cameraTask"/> completion.
        /// </summary>
        /// <param name="cameraTask">Task to enqueue.</param>
        /// <returns>Awaitable task.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="cameraTask"/> is <see langword="null"/>.</exception>
        public Task EnqueueTaskAsync(ICameraTask cameraTask)
        {
            if (cameraTask == null)
            {
                throw new ArgumentNullException("cameraTask");
            }

            QueuedCameraTask task = this.AddTaskToQueue(cameraTask);

            return Task.Run(() => task.WaitForComplete(Timeout.InfiniteTimeSpan));
        }

        /// <summary>
        /// Tries to cancel the current task, if possible.
        /// </summary>
        /// <param name="predicate">
        /// Predicate function that defines whether the current task should actually be cancelled.<br/>
        /// It's only called, if the current task is cancellable.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public void CancelCurrentTask(Predicate<ICameraTask> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            lock (this.syncRoot)
            {
                if (this.currentTask != null && this.currentTask.Task.IsCancellable && predicate(this.currentTask.Task))
                {
                    try
                    {
                        this.currentTask.Task.Cancel();
                    }
                    catch (Exception e)
                    {
                        Tracing.Trace("CameraTaskEngine: Unable to cancel task: {0}", e);
                        this.CameraTaskCompleted(this.currentTask.Task, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Removes all pending camera tasks that match the <paramref name="predicate"/> from the execution queue.
        /// </summary>
        /// <param name="predicate">Predicate that defines whether the current pending task should be cancelled.</param>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Pending tasks are not <i>cancelled</i>, just removed from the queue.
        /// </remarks>
        public void CancelPendingTasks(Predicate<ICameraTask> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            lock (this.syncRoot)
            {
                if (!this.pendingTasks.Any())
                {
                    return;
                }

                for (int i = 0; i < this.pendingTasks.Count; i++)
                {
                    if (predicate(this.pendingTasks[i].Task))
                    {
                        this.pendingTasks[i].Dispose();
                        this.pendingTasks.RemoveAt(i);

                        i--;
                    }
                }
            }
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        /// Adds the <paramref name="cameraTask"/> specified to the execution queue.
        /// </summary>
        /// <param name="cameraTask">The camera task to add to the queue.</param>
        /// <returns>Execution wrapper for the <paramref name="cameraTask"/> given.</returns>
        private QueuedCameraTask AddTaskToQueue(ICameraTask cameraTask)
        {
            QueuedCameraTask queuedTask = new QueuedCameraTask(cameraTask);

            lock (this.syncRoot)
            {
                this.pendingTasks.Add(queuedTask);

                this.ScheduleNextTask();
            }

            return queuedTask;
        }

        /// <summary>
        /// Schedules the next pending task for execution, if needed.
        /// </summary>
        private void ScheduleNextTask()
        {
            lock (this.syncRoot)
            {
                if (this.currentTask != null || !this.pendingTasks.Any())
                {
                    return;
                }

                this.currentTask = this.pendingTasks[0];
                this.currentTask.Task.Complete += this.CameraTaskCompleted;

                this.pendingTasks.RemoveAt(0);

                ThreadPool.QueueUserWorkItem(state => this.StartCurrentTask());
            }
        }

        /// <summary>
        /// Starts the current camera task.
        /// </summary>
        private void StartCurrentTask()
        {
            lock (this.syncRoot)
            {
                if (this.currentTask == null)
                {
                    return;
                }

                try
                {
                    this.currentTask.Task.Start();
                }
                catch (Exception e)
                {
                    Tracing.Trace("CameraTaskEngine: Unable to start task: {0}", e);
                    this.CameraTaskCompleted(this.currentTask.Task, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// The <see cref="ICameraTask.Complete"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CameraTaskCompleted(object sender, EventArgs e)
        {
            lock (this.syncRoot)
            {
                // Makes sure that the current method isn't called for the second
                // time for the same task.
                if (this.currentTask == null || this.currentTask.Task != sender)
                {
                    return;
                }

                this.currentTask.Task.Complete -= this.CameraTaskCompleted;
                this.currentTask.Dispose();
                this.currentTask = null;

                this.ScheduleNextTask();
            }
        }

        #endregion // Private methods

        #region Classes

        /// <summary>
        /// Represents an executing or pending camera task.
        /// </summary>
        private class QueuedCameraTask : IDisposable
        {
            /// <summary>
            /// Wait handle that is set on the <see cref="Task"/> completion.
            /// </summary>
            private ManualResetEventSlim taskWaitHandle;

            /// <summary>
            /// Whether the current object is disposed.
            /// </summary>
            private volatile bool disposed;

            /// <summary>
            /// Initializes a new instance of the <see cref="QueuedCameraTask"/> class.
            /// </summary>
            /// <param name="task">The actual camera task object this class wraps.</param>
            public QueuedCameraTask(ICameraTask task)
            {
                this.Task           = task;
                this.taskWaitHandle = new ManualResetEventSlim(false);

                this.Task.Complete += this.TaskCompleted;
            }

            /// <summary>
            /// Finalizes an instance of the <see cref="QueuedCameraTask"/> class.
            /// </summary>
            ~QueuedCameraTask()
            {
                this.Dispose(false);
            }

            /// <summary>
            /// Gets or sets the task instance.
            /// </summary>
            public ICameraTask Task { get; private set; }

            /// <summary>
            /// Waits for the <see cref="Task"/> to complete.
            /// </summary>
            /// <param name="timeout">Timeout to wait for.</param>
            /// <returns>
            /// <see langword="true"/>, if the <see cref="Task"/> completed; otherwise, <see langword="false"/>.
            /// </returns>
            public bool WaitForComplete(TimeSpan timeout)
            {
                // If the current object is already (or about to be) disposed,
                // we don't want to wait for the event.
                return !this.disposed && this.taskWaitHandle.Wait(timeout);
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// The <see cref="ICameraTask.Complete"/> event handler.
            /// </summary>
            /// <param name="sender">Event sender.</param>
            /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
            private void TaskCompleted(object sender, EventArgs eventArgs)
            {
                this.Task.Complete -= this.TaskCompleted;
                this.taskWaitHandle.Set();
            }

            /// <summary>
            /// Releases unmanaged and - optionally - managed resources.
            /// </summary>
            /// <param name="disposing">
            /// <see langword="true"/> to release both managed and unmanaged resources;
            /// <see langword="false"/> to release only unmanaged resources.
            /// </param>
            private void Dispose(bool disposing)
            {
                if (this.disposed)
                {
                    return;
                }

                // Mark this class as disposed early, so the method(s) relying on
                // the disposable objects (eg, taskWaitHandle) will know that
                // those objects are no longer valid.
                this.disposed = true;

                if (disposing)
                {
                    this.Task.Complete -= this.TaskCompleted;

                    if (this.taskWaitHandle != null)
                    {
                        // Set the event in case someone is waiting for it.
                        this.taskWaitHandle.Set();
                        this.taskWaitHandle.Dispose();
                        this.taskWaitHandle = null;
                    }

                    IDisposable disposableTask = this.Task as IDisposable;
                    if (disposableTask != null)
                    {
                        disposableTask.Dispose();
                    }

                    this.Task = null;
                }
            }
        }

        #endregion // Classes
    }
}
