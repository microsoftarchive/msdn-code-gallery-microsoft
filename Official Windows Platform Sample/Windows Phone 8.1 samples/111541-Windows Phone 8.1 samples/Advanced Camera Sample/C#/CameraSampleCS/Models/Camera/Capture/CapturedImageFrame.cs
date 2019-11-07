// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="CapturedImageFrame.cs">
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

namespace CameraSampleCS.Models.Camera.Capture
{
    using System;
    using System.Threading.Tasks;
    using Windows.Graphics.Imaging;
    using CameraSampleCS.Helpers;
    using Microsoft.Devices;
    using Microsoft.Phone.Controls;
    using Windows.Media.Capture;
    using Windows.Media.MediaProperties;
    using Windows.Storage.Streams;

    /// <summary>
    /// Contains information about an image frame captured by the camera device.
    /// </summary>
    public class CapturedImageFrame : ICapturedPhoto
    {
        #region Constuctor

        /// <summary>
        /// Prevents a default instance of the <see cref="CapturedImageFrame"/> class from being created.
        /// </summary>
        private CapturedImageFrame()
        {
            // Do nothing.
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CapturedImageFrame"/> class.
        /// </summary>
        ~CapturedImageFrame()
        {
            this.Dispose(false);
        }

        #endregion // Constuctor

        #region Properties

        /// <summary>
        /// Gets the image height in pixels.
        /// </summary>
        public uint Height { get; private set; }

        /// <summary>
        /// Gets the image width in pixels.
        /// </summary>
        public uint Width { get; private set; }

        /// <summary>
        /// Gets the frame rotation.
        /// </summary>
        public BitmapRotation Rotation { get; private set; }

        /// <summary>
        /// Gets the frame flip.
        /// </summary>
        public BitmapFlip Flip { get; private set; }

        /// <summary>
        /// Gets or sets the stream containing frame data.
        /// </summary>
        internal IRandomAccessStream Stream { get; private set; }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Creates a new <see cref="CapturedImageFrame"/> instance from the <paramref name="frame"/> specified.
        /// </summary>
        /// <param name="frame">Captured frame.</param>
        /// <param name="orientation">Camera orientation.</param>
        /// <param name="cameraType">Camera type.</param>
        /// <returns>Captured image frame.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="frame"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="frame"/> stream cannot be read.
        ///     <para>-or-</para>
        /// <paramref name="frame"/> stream is empty.
        /// </exception>
        public static CapturedImageFrame CreateFromCapturedFrame(CapturedFrame frame, PageOrientation orientation, CameraType cameraType)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }

            if (!frame.CanRead)
            {
                throw new ArgumentException("Frame stream cannot be read.", "frame");
            }

            if (frame.Size == 0)
            {
                throw new ArgumentException("Frame stream is empty.", "frame");
            }

            return new CapturedImageFrame
            {
                Rotation = OrientationHelper.ConvertOrientationToRotation(orientation, cameraType),
                Flip     = OrientationHelper.ConvertOrientationToFlip(orientation, cameraType),
                Stream   = frame,
                Height   = frame.Height,
                Width    = frame.Width
            };
        }

        /// <summary>
        /// Creates a new <see cref="CapturedImageFrame"/> instance from the <paramref name="stream"/> specified.
        /// </summary>
        /// <param name="stream">Captured image stream.</param>
        /// <param name="encoding">Encoding used to capture the frame <paramref name="stream"/></param>
        /// <param name="orientation">Camera orientation.</param>
        /// <param name="cameraType">Camera type.</param>
        /// <returns>Captured image frame.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="stream"/> is <see langword="null"/>.
        ///     <para>-or-</para>
        /// <paramref name="encoding"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="stream"/> cannot be read.
        ///     <para>-or-</para>
        /// <paramref name="stream"/> is empty.
        /// </exception>
        public static CapturedImageFrame CreateFromStream(IRandomAccessStream stream, ImageEncodingProperties encoding, PageOrientation orientation, CameraType cameraType)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream cannot be read.", "stream");
            }

            if (stream.Size == 0)
            {
                throw new ArgumentException("Stream is empty.", "stream");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            return new CapturedImageFrame
            {
                Rotation = OrientationHelper.ConvertOrientationToRotation(orientation, cameraType),
                Flip     = OrientationHelper.ConvertOrientationToFlip(orientation, cameraType),
                Stream   = stream,
                Height   = encoding.Height,
                Width    = encoding.Width
            };
        }

        /// <summary>
        /// Returns the internal frame pixel data as a byte array.
        /// </summary>
        /// <returns>Byte array containing raw pixel data.</returns>
        public async Task<byte[]> DetachPixelDataAsync()
        {
            if (this.Stream == null)
            {
                return null;
            }

            byte[] bytes = new byte[this.Stream.Size];

            using (DataReader reader = new DataReader(this.Stream.GetInputStreamAt(0)))
            {
                await reader.LoadAsync((uint)this.Stream.Size);
                reader.ReadBytes(bytes);
            }

            this.Stream.Dispose();
            this.Stream = null;

            return bytes;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // Public methods

        #region Protected methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Stream != null)
                {
                    this.Stream.Dispose();
                    this.Stream = null;
                }
            }
        }

        #endregion // Protected methods
    }
}
