// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="MainPageViewModelState.cs">
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

namespace CameraSampleCS.ViewModels
{
    /// <summary>
    /// Possible <see cref="MainPageViewModel"/> states.
    /// </summary>
    public enum MainPageViewModelState
    {
        /// <summary>
        /// Camera is unloaded.
        /// </summary>
        Unloaded = 0,

        /// <summary>
        /// Camera is loading.
        /// </summary>
        Loading,

        /// <summary>
        /// Camera is loaded, and video preview is started.
        /// </summary>
        Loaded,

        /// <summary>
        /// Focus operation is in progress.
        /// </summary>
        FocusInProgress,

        /// <summary>
        /// Frame capture is in progress.
        /// </summary>
        CaptureInProgress,

        /// <summary>
        /// Application is saving capture result.
        /// </summary>
        SavingCaptureResult
    }
}
