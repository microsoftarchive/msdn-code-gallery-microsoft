// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="LocalFolderThumbnailedImage.cs">
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
    using System.IO.IsolatedStorage;

    /// <summary>
    /// This code is taken from the Code Samples for Windows Phone (http://code.msdn.microsoft.com/wpapps).
    /// </summary>
    public class LocalFolderThumbnailedImage: IThumbnailedImage
    {
        /// <summary>
        /// The path and file name of the full resolution image file.
        /// </summary>
        public string ImageFileName { get; private set; }
        /// <summary>
        /// The path and file name of the thumbnail resolution image file.
        /// </summary>
        public string ThumbnailFileName { get; private set; }

        private DateTime? _dateTaken = null;

        public LocalFolderThumbnailedImage(string imageFileName, string thumbnailFileName)
        {
            this.ImageFileName = imageFileName;
            this.ThumbnailFileName = thumbnailFileName;
        }

        /// <summary>
        /// Returns a Stream representing the thumbnail image.
        /// </summary>
        /// <returns>Stream of the thumbnail image.</returns>
        public Stream GetThumbnailImage()
        {
            Stream thumbnailFileStream;

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                thumbnailFileStream = store.OpenFile(
                    this.ThumbnailFileName,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Delete | FileShare.Read);
            }
            thumbnailFileStream.Seek(0, SeekOrigin.Begin);

            return thumbnailFileStream;
        }

        /// <summary>
        /// Returns a Stream representing the full resolution image.
        /// </summary>
        /// <returns>Stream of the full resolution image.</returns>
        public Stream GetImage()
        {
            Stream imageFileStream;

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                imageFileStream = store.OpenFile(
                    this.ImageFileName, 
                    FileMode.Open, 
                    FileAccess.Read, 
                    FileShare.Delete | FileShare.Read);
            }
            imageFileStream.Seek(0, SeekOrigin.Begin);

            return imageFileStream;
        }

        /// <summary>
        /// Represents the time the photo was taken, useful for sorting photos.
        /// </summary>
        public DateTime DateTaken
        {
            get
            {
                if (this._dateTaken == null)
                {
                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        this._dateTaken = store.GetCreationTime(this.ImageFileName).DateTime;
                    }
                }

                return this._dateTaken.Value;
            }
        }

        public string Path
        {
            get
            {
                return this.ImageFileName;
            }
        }
    }
}
