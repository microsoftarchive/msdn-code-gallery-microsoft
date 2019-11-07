// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="IStorageService.cs">
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

namespace CameraSampleCS.Services.Storage
{
    using System.Threading.Tasks;
    using CameraSampleCS.Models.Camera.Capture;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Defines methods to work with media library and isolated application storage.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Asynchronously saves the <paramref name="photo"/> given to the camera roll album.
        /// </summary>
        /// <param name="photo">Photo to save.</param>
        /// <returns>Image with thumbnail.</returns>
        Task<IThumbnailedImage> SaveResultToCameraRollAsync(ICapturedPhoto photo);
    }
}
