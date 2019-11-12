// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="StartPhotoCaptureTask.cs">
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
    using CameraSampleCS.Models.Camera.Capture;

    /// <summary>
    /// Starts the photo capture operation.
    /// </summary>
    public class StartPhotoCaptureTask : CameraTaskBase
    {
        #region Fields

        /// <summary>
        /// Current photo capture instance.
        /// </summary>
        private readonly IPhotoCapture photoCapture;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StartPhotoCaptureTask"/> class.
        /// </summary>
        /// <param name="cameraController">Current camera controller.</param>
        /// <param name="photoCapture">Photo capture to stop.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cameraController"/> is <see langword="null"/>.
        ///     <para>-or-</para>
        /// <paramref name="photoCapture"/> is <see langword="null"/>.
        /// </exception>
        public StartPhotoCaptureTask(CameraController cameraController, IPhotoCapture photoCapture)
            : base(cameraController)
        {
            if (photoCapture == null)
            {
                throw new ArgumentNullException("photoCapture");
            }

            this.photoCapture = photoCapture;
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
                return CameraTaskType.StartCapture;
            }
        }

        #endregion // Properties

        #region Protected methods

        /// <summary>
        /// Stops the photo capture operation.
        /// </summary>
        protected override async void DoWork()
        {
            await this.photoCapture.StartAsync();

            Tracing.Trace("StartPhotoCaptureTask: Photo capture started.");
            this.NotifyCompleted();
        }

        #endregion // Protected methods
    }
}
