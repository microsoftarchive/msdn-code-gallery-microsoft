// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="LoadCameraTask.cs">
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
    /// Initializes the camera device.
    /// </summary>
    public class LoadCameraTask : CameraTaskBase
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadCameraTask"/> class.
        /// </summary>
        /// <param name="cameraController">Current camera controller.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cameraController"/> is <see langword="null"/>.</exception>
        public LoadCameraTask(CameraController cameraController)
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
                return CameraTaskType.Initialization;
            }
        }

        #endregion // Properties

        #region Protected methods

        /// <summary>
        /// Loads and initializes the <see cref="CameraController"/> object.
        /// </summary>
        protected override void DoWork()
        {
            // Must be done in the UI thread context.
            DispatcherHelper.BeginInvokeOnUIThread(async () =>
            {
                await this.CameraController.InitializeAsync();

                Tracing.Trace("LoadCameraTask: Camera loaded.");
                this.NotifyCompleted();
            });
        }

        #endregion // Protected methods
    }
}
