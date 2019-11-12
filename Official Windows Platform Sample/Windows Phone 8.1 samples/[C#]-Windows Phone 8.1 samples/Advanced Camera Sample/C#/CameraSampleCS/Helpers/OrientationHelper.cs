// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="OrientationHelper.cs">
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

namespace CameraSampleCS.Helpers
{
    using Windows.Graphics.Imaging;
    using Microsoft.Devices;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Contains helper methods to work with <see cref="PageOrientation"/>.
    /// </summary>
    public static class OrientationHelper
    {
        /// <summary>
        /// Determines whether the <paramref name="orientation"/> specified is a landscape one.
        /// </summary>
        /// <param name="orientation">Page orientation.</param>
        /// <returns>
        /// <see langword="true"/>, if <paramref name="orientation"/> is landcape; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsLandscape(PageOrientation orientation)
        {
            return orientation == PageOrientation.Landscape || orientation == PageOrientation.LandscapeLeft || orientation == PageOrientation.LandscapeRight;
        }

        /// <summary>
        /// Determines whether the <paramref name="orientation"/> specified is a portrait one.
        /// </summary>
        /// <param name="orientation">Page orientation.</param>
        /// <returns>
        /// <see langword="true"/>, if <paramref name="orientation"/> is portait; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsPortrait(PageOrientation orientation)
        {
            return orientation == PageOrientation.Portrait || orientation == PageOrientation.PortraitDown || orientation == PageOrientation.PortraitUp;
        }

        /// <summary>
        /// Converts the <paramref name="orientation"/> given to a proper page rotation angle, in degrees.
        /// </summary>
        /// <param name="orientation">Page orientation.</param>
        /// <returns>Rotation angle.</returns>
        public static int GetRotationAngle(PageOrientation orientation)
        {
            int angle = 0;

            switch (orientation)
            {
                case PageOrientation.None:
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                    angle = 90;
                    break;
                case PageOrientation.PortraitDown:
                    angle = 270;
                    break;
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                    angle = 0;
                    break;
                case PageOrientation.LandscapeRight:
                    angle = 180;
                    break;
            }

            return angle;
        }

        /// <summary>
        /// Converts the <see cref="PageOrientation"/> to the according <see cref="BitmapRotation"/> value.
        /// </summary>
        /// <param name="orientation"><see cref="PageOrientation"/> to convert.</param>
        /// <param name="cameraType">Camera type.</param>
        /// <returns>Conversion result.</returns>
        /// <remarks>
        /// This method assumes that the preview is horizontally flipped for the front-facing camera.
        /// </remarks>
        public static BitmapRotation ConvertOrientationToRotation(PageOrientation orientation, CameraType cameraType)
        {
            BitmapRotation rotation = BitmapRotation.None;

            switch (orientation)
            {
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                    rotation = cameraType == CameraType.Primary ? BitmapRotation.None : BitmapRotation.Clockwise180Degrees;
                    break;
                case PageOrientation.LandscapeRight:
                    rotation = cameraType == CameraType.Primary ? BitmapRotation.Clockwise180Degrees : BitmapRotation.None;
                    break;
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                    rotation = cameraType == CameraType.Primary ? BitmapRotation.Clockwise90Degrees : BitmapRotation.Clockwise270Degrees;
                    break;
                case PageOrientation.PortraitDown:
                    rotation = cameraType == CameraType.Primary ? BitmapRotation.Clockwise270Degrees : BitmapRotation.Clockwise90Degrees;
                    break;
            }

            return rotation;
        }

        /// <summary>
        /// Converts the <see cref="PageOrientation"/> to the according <see cref="BitmapFlip"/> value.
        /// </summary>
        /// <param name="orientation"><see cref="PageOrientation"/> to convert.</param>
        /// <param name="cameraType">Camera type.</param>
        /// <returns>Conversion result.</returns>
        /// <remarks>
        /// This method assumes that the preview is horizontally flipped for the front-facing camera.
        /// </remarks>
        public static BitmapFlip ConvertOrientationToFlip(PageOrientation orientation, CameraType cameraType)
        {
            return cameraType == CameraType.Primary ? BitmapFlip.None : BitmapFlip.Vertical;
        }
    }
}
