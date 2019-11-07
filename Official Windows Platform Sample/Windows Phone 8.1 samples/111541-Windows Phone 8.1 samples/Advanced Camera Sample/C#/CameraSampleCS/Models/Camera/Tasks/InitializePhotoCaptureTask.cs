// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="InitializePhotoCaptureTask.cs">
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
    using Windows.Media.Capture;
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Models.Camera;
    using CameraSampleCS.Models.Camera.Capture;

    /// <summary>
    /// Initializes photo capture object.
    /// </summary>
    public class InitializePhotoCaptureTask : CameraTaskBase
    {
        #region Fields

        /// <summary>
        /// Photo capture to be initialized.
        /// </summary>
        private readonly IPhotoCapture photoCapture;

        /// <summary>
        /// Camera capture parameters.
        /// </summary>
        private readonly CaptureParameters captureParameters;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializePhotoCaptureTask"/> class.
        /// </summary>
        /// <param name="cameraController">Current camera controller.</param>
        /// <param name="photoCapture">Photo capture to initialize.</param>
        /// <param name="captureParameters">Photo capture parameters.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cameraController"/> is <see langword="null"/>.
        ///     <para>-or-</para>
        /// <paramref name="photoCapture"/> is <see langword="null"/>.
        ///     <para>-or-</para>
        /// <paramref name="captureParameters"/> is <see langword="null"/>.
        /// </exception>
        public InitializePhotoCaptureTask(CameraController cameraController, IPhotoCapture photoCapture, CaptureParameters captureParameters)
            : base(cameraController)
        {
            if (photoCapture == null)
            {
                throw new ArgumentNullException("photoCapture");
            }

            if (captureParameters == null)
            {
                throw new ArgumentNullException("captureParameters");
            }

            this.photoCapture      = photoCapture;
            this.captureParameters = captureParameters;
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
        /// Initializes photo capture.
        /// </summary>
        protected override async void DoWork()
        {
            // Make sure we use the proper media format for the photo capture.
            await this.CameraController.SetMediaFormatAsync(MediaStreamType.Photo, new Size(this.captureParameters.ImageEncoding.Width, this.captureParameters.ImageEncoding.Width));

            await this.photoCapture.InitializeAsync(this.captureParameters);

            Tracing.Trace("InitializePhotoCaptureTask: Capture initialized.");
            this.NotifyCompleted();
        }

        #endregion // Protected methods
    }
}
