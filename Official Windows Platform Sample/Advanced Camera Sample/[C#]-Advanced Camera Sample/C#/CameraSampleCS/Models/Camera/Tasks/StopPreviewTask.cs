// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="StopPreviewTask.cs">
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
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Models.Camera;

    /// <summary>
    /// Stops the video preview.
    /// </summary>
    public class StopPreviewTask : CameraTaskBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StopPreviewTask"/> class.
        /// </summary>
        /// <param name="cameraController">Current camera controller.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cameraController"/> is <see langword="null"/>.</exception>
        public StopPreviewTask(CameraController cameraController)
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
                return CameraTaskType.Uninitialization;
            }
        }

        #endregion // Properties

        #region Protected methods

        /// <summary>
        /// Stops the video preview.
        /// </summary>
        protected override void DoWork()
        {
            DispatcherHelper.BeginInvokeOnUIThread(async () =>
            {
                await this.CameraController.StopPreviewAsync();

                Tracing.Trace("StopPreviewTask: Camera preview stopped.");
                this.NotifyCompleted();
            });
        }

        #endregion // Protected methods
    }
}
