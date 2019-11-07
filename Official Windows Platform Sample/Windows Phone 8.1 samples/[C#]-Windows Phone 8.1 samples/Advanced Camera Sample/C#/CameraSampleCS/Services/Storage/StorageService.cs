// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="StorageService.cs">
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
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using CameraSampleCS.Helpers;
    using CameraSampleCS.Models.Camera.Capture;
    using Microsoft.Phone.Controls;
    using Microsoft.Xna.Framework.Media;
    using Windows.Graphics.Imaging;
    using Windows.Storage.Streams;

    /// <summary>
    /// Basic <see cref="IStorageService"/> implementation.
    /// </summary>
    public sealed class StorageService : IStorageService
    {
        #region Fields

        /// <summary>
        /// Lazily-created media library.
        /// </summary>
        private readonly Lazy<MediaLibrary> lazyMediaLibrary = new Lazy<MediaLibrary>(() => new MediaLibrary());

        #endregion // Fields

        #region Public methods

        /// <summary>
        /// Asynchronously saves the <paramref name="photo" /> given to the camera roll album.
        /// </summary>
        /// <param name="photo">Photo to save.</param>
        /// <returns>Image with thumbnail.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="photo"/> is <see langword="null"/>.</exception>
        public async Task<IThumbnailedImage> SaveResultToCameraRollAsync(ICapturedPhoto photo)
        {
            if (photo == null)
            {
                throw new ArgumentNullException("photo");
            }

            Tracing.Trace("StorageService: Trying to save picture to the Camera Roll.");

            Picture cameraRollPicture;
            string name = this.GeneratePhotoName();

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                // Convert image to JPEG format and rotate in accordance with the original photo orientation.
                Tracing.Trace("StorageService: Converting photo to JPEG format. Size: {0}x{1}, Rotation: {2}", photo.Width, photo.Height, photo.Rotation);
                byte[] pixelData = await photo.DetachPixelDataAsync();

                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.BitmapTransform.Rotation = photo.Rotation;
                encoder.BitmapTransform.Flip     = photo.Flip;
                encoder.IsThumbnailGenerated     = true;

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, photo.Width, photo.Height, Constants.DefaultDpiX, Constants.DefaultDpiY, pixelData);
                await encoder.FlushAsync();

                cameraRollPicture = this.lazyMediaLibrary.Value.SavePictureToCameraRoll(name, stream.AsStream());
            }

            Tracing.Trace("StorageService: Saved to Camera Roll as {0}", name);

            return new MediaLibraryThumbnailedImage(cameraRollPicture);
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        /// Generates the unique name photo name.
        /// </summary>
        /// <returns>Photo name.</returns>
        private string GeneratePhotoName()
        {
            return string.Format(CultureInfo.InvariantCulture, "_cs_{0:yyyy.MM.dd.HH.mm.ss}", DateTime.UtcNow);
        }

        #endregion // Private methods
    }
}
