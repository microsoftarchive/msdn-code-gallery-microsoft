// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CameraTaskType.cs">
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
    /// <summary>
    /// Types of the tasks supported.
    /// </summary>
    public enum CameraTaskType
    {
        /// <summary>
        /// Tasks that are used to initialize camera or capture objects.
        /// </summary>
        Initialization = 0,

        /// <summary>
        /// Tasks that are used for camera uninitialization.
        /// </summary>
        Uninitialization,

        /// <summary>
        /// Tasks that start photo/video capture.
        /// </summary>
        StartCapture,

        /// <summary>
        /// Tasks that stop photo/video capture.
        /// </summary>
        /// <remarks>
        /// Pending tasks of this type, if in the beginning of the execution queue,
        /// should not be cancelled, because it may leave camera in an invalid state.
        /// </remarks>
        StopCapture,

        /// <summary>
        /// Other tasks. For example, focus.
        /// </summary>
        Other
    }
}
