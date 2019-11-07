// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ScreenFormat.cs">
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

namespace CameraSampleCS.Models.Camera
{
    /// <summary>
    /// Constains screen formats known by the application.
    /// </summary>
    public enum ScreenFormat
    {
        /// <summary>
        /// Unknown or unsupported screen format.
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// <c>4:3</c> screen format.
        /// </summary>
        FourByThree = 0,

        /// <summary>
        /// <c>16:9</c> screen format.
        /// </summary>
        SixteenByNine
    }
}
