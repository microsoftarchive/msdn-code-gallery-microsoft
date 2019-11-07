// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ICameraTask.cs">
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

    /// <summary>
    /// Defines common camera task methods.
    /// </summary>
    public interface ICameraTask
    {
        /// <summary>
        /// Occurs when the current task finishes its execution.
        /// </summary>
        event EventHandler Complete;

        /// <summary>
        /// Gets the type of the task.
        /// </summary>
        CameraTaskType Type { get; }

        /// <summary>
        /// Gets a value indicating whether the current task can be cancelled.
        /// </summary>
        bool IsCancellable { get; }

        /// <summary>
        /// Starts the task execution.
        /// </summary>
        void Start();

        /// <summary>
        /// Cancels the task.
        /// </summary>
        void Cancel();
    }
}
