// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CameraTaskBase.cs">
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

namespace CameraSampleCS.Models.Camera.Tasks
{
    using System;
    using CameraSampleCS.Models.Camera;

    /// <summary>
    /// Basic <see cref="ICameraTask"/> implementation that helps creating new camera tasks.
    /// </summary>
    public abstract class CameraTaskBase : ICameraTask
    {
        #region Fields

        /// <summary>
        /// Whether the task cancellation is requested.
        /// </summary>
        /// <seealso cref="Cancel"/>
        private volatile bool cancellationRequested;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraTaskBase"/> class.
        /// </summary>
        /// <param name="cameraController">Current camera controller.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cameraController"/> is <see langword="null"/>.</exception>
        protected CameraTaskBase(CameraController cameraController)
        {
            if (cameraController == null)
            {
                throw new ArgumentNullException("cameraController");
            }

            this.CameraController = cameraController;
        }

        #endregion // Constructor

        #region Events

        /// <summary>
        /// Occurs when the current task finishes its execution.
        /// </summary>
        public event EventHandler Complete;

        #endregion // Events

        #region Properties

        /// <summary>
        /// Gets the type of the task.
        /// </summary>
        public abstract CameraTaskType Type { get; }

        /// <summary>
        /// Gets a value indicating whether the current task can be cancelled.
        /// </summary>
        public virtual bool IsCancellable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the current camera controller.
        /// </summary>
        protected CameraController CameraController { get; private set; }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Starts the task execution.
        /// </summary>
        public virtual void Start()
        {
            if (this.cancellationRequested)
            {
                this.NotifyCompleted();
                return;
            }

            // Exceptions thrown by this methods will be handled by the execution engine.
            this.DoWork();
        }

        /// <summary>
        /// Cancels the current task.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current task is not cancellable.</exception>
        /// <seealso cref="IsCancellable"/>
        public virtual void Cancel()
        {
            if (!this.IsCancellable)
            {
                throw new InvalidOperationException("The current task is not cancellable.");
            }

            this.cancellationRequested = true;
        }

        #endregion // Public methods

        #region Protected methods

        /// <summary>
        /// Allows inherited tasks to do their job.
        /// </summary>
        protected abstract void DoWork();

        /// <summary>
        /// Notifies the <see cref="Complete"/> event listeners.
        /// </summary>
        protected void NotifyCompleted()
        {
            EventHandler completeHandler = this.Complete;
            if (completeHandler != null)
            {
                completeHandler.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion // Protected methods
    }
}
