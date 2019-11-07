// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="MediaLibraryThumbnailedImage.cs">
//   Copyright (c) 2011 - 2014 Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Phone.Controls
{
    using System;
    using System.IO;
    using Microsoft.Xna.Framework.Media;
    using Microsoft.Xna.Framework.Media.PhoneExtensions;

    /// <summary>
    /// This code is taken from the Code Samples for Windows Phone (http://code.msdn.microsoft.com/wpapps).
    /// </summary>
    public class MediaLibraryThumbnailedImage : IThumbnailedImage
    {
        /// <summary>
        /// The Picture object that this instance represents.
        /// </summary>
        public Picture Picture { get; private set; }

        public MediaLibraryThumbnailedImage(Picture picture)
        {
            this.Picture = picture;
        }

        /// <summary>
        /// Returns a Stream representing the thumbnail image.
        /// </summary>
        /// <returns>Stream of the thumbnail image.</returns>
        public Stream GetThumbnailImage()
        {
            return this.Picture.GetPreviewImage();
        }

        /// <summary>
        /// Returns a Stream representing the full resolution image.
        /// </summary>
        /// <returns>Stream of the full resolution image.</returns>
        public Stream GetImage()
        {
            return this.Picture.GetImage();
        }

        /// <summary>
        /// Represents the time the photo was taken, useful for sorting photos.
        /// </summary>
        public DateTime DateTaken
        {
            get
            {
                return this.Picture.Date;
            }
        }

        public string Path
        {
            get
            {
                return this.Picture.GetPath();
            }
        }
    }
}
