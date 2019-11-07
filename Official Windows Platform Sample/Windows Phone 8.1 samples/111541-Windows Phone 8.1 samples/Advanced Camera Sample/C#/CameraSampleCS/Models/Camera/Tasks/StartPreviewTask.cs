// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="StartPreviewTask.cs">
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
    using System.Windows;
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Models.Camera;

    /// <summary>
    /// Starts the video preview.
    /// </summary>
    public class StartPreviewTask : CameraTaskBase
    {
        #region Fields

        /// <summary>
        /// The desired preview size.
        /// </summary>
        private readonly Size previewSize;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StartPreviewTask"/> class.
        /// </summary>
        /// <param name="cameraController">Current camera controller.</param>
        /// <param name="previewSize">The desired preview size.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cameraController"/> is <see langword="null"/>.</exception>
        public StartPreviewTask(CameraController cameraController, Size previewSize)
            : base(cameraController)
        {
            this.previewSize = previewSize;
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
                return CameraTaskType.Initialization;
            }
        }

        #endregion // Properties

        #region Protected methods

        /// <summary>
        /// Initializes and starts the video preview.
        /// </summary>
        protected override void DoWork()
        {
            DispatcherHelper.BeginInvokeOnUIThread(async () =>
            {
                await this.CameraController.StartPreviewAsync(this.previewSize);

                Tracing.Trace("StartPreviewTask: Camera preview started.");
                this.NotifyCompleted();
            });
        }

        #endregion // Protected methods
    }
}
