// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo 
{
    // ExifRotations values represent the codes used by JPEG image files to indicated how an image should be rotated for display.
    public enum class ExifRotations
    {
        NotRotated = 1,
        RotatedDown = 3,
        RotatedLeft = 6,
        RotatedRight = 8
    };

    // ExifExtensions is a helper class that converts image rotation angles between degrees and 
    // Exchangeable image file format (Exif) codes that are used by JPEG images.
    class ExifExtensions 
    {

    public:
        static Windows::Foundation::Rect RotateClockwise(Windows::Foundation::Rect rect, Windows::Foundation::Rect bitmapSize, float64 degrees);
        static unsigned int ConvertExifOrientationToDegreesRotation(ExifRotations exifOrientationFlag);
        static ExifRotations ConvertDegreesRotationToExifOrientation(unsigned int angle);

    };
}