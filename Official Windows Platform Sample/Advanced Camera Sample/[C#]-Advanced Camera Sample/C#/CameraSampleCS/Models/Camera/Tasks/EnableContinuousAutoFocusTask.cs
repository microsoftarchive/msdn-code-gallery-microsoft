// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="FocusTask.cs">
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
    using Windows.Foundation;
    using CameraSampleCS.Helpers;

    /// <summary>
    /// Enables the continous automatic focus.
    /// </summary>
    public class EnableContinuousAutoFocusTask : CameraTaskBase
    {
        #region Fields

        /// <summary>
        /// Synchronization root.
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// Asynchronous focus operation.
        /// </summary>
        private IAsyncAction focusAction;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableContinuousAutoFocusTask"/> class.
        /// </summary>
        /// <param name="cameraController">Current camera controller.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cameraController"/> is <see langword="null"/>.</exception>
        public EnableContinuousAutoFocusTask(CameraController cameraController)
            : base(cameraController)
        {
            // Do nothing.
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Gets the type of the task.
        /// </summary>
        public override CameraTaskType Type
        {
            get
            {
                return CameraTaskType.Other;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current task can be cancelled.
        /// </summary>
        public override bool IsCancellable
        {
            get
            {
                return true;
            }
        }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Cancels the focus task.
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();

            lock (this.syncRoot)
            {
                if (this.focusAction != null)
                {
                    this.focusAction.Cancel();
                    this.focusAction = null;
                }
            }
        }

        #endregion // Public methods

        #region Protected methods

        /// <summary>
        /// Starts the asynchronous focus action.
        /// </summary>
        protected override async void DoWork()
        {
            IAsyncAction action = await this.CameraController.EnableContinuousAutoFocusAsync();

            lock (this.syncRoot)
            {
                this.focusAction = action;
            }

            try
            {
                await action;
            }
            catch (AggregateException)
            {
                // Ignore.
            }

            Tracing.Trace("EnableContinuousAutoFocusTask: Continuous auto-focus enabled.");
            this.NotifyCompleted();
        }

        #endregion // Protected methods
    }
}
