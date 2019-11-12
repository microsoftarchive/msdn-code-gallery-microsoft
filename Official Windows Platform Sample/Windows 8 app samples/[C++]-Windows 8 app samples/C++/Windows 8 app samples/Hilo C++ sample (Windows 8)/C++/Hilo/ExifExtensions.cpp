// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#include "pch.h"
#include "ExifExtensions.h"

#define _USE_MATH_DEFINES
#include <math.h>
#include <algorithm>

using namespace std;
using namespace Hilo;
using namespace Windows::Foundation;

Rect ExifExtensions::RotateClockwise(Rect rect, Rect bitmapSize, float64 degrees)
{
    auto radians = (M_PI / 180.0) * degrees;

    auto angleSin = sin(radians);
    auto angleCos = cos(radians);

    auto width = bitmapSize.Width;
    auto height = bitmapSize.Height;

    // calculate rotated translation point for image
    auto x1 = width * angleCos;
    auto y1 = width * angleSin;
    auto x2 = width * angleCos - height*angleSin;
    auto y2 = width * angleSin + height*angleCos;
    auto x3 = -(height * angleSin);
    auto y3 = height * angleCos ;

    auto minX = min(x1, min(x2, x3));
    auto minY = min(y1, min(y2, y3));

    // Adjust rotate and adjust original rect bounding box
    auto xOrigin = (rect.X * angleCos - rect.Y * angleSin) - minX;
    auto yOrigin = (rect.X * angleSin + rect.Y * angleCos) - minY;

    auto xOther = (rect.Right * angleCos - rect.Bottom * angleSin) - minX;
    auto yOther = (rect.Right * angleSin + rect.Bottom * angleCos) - minY;

    Point newOrigin(static_cast<float>(min(xOrigin, xOther)), 
        static_cast<float>(min(yOrigin, yOther)));
    Point newOther(static_cast<float>(max(xOrigin, xOther)), 
        static_cast<float>(max(yOrigin, yOther)));

    return Rect(newOrigin, newOther);
}

unsigned int ExifExtensions::ConvertExifOrientationToDegreesRotation(ExifRotations exifOrientationFlag)
{
    switch (exifOrientationFlag)
    {
    case ExifRotations::NotRotated:
        return 0;
    case ExifRotations::RotatedLeft:
        return 90;
    case ExifRotations::RotatedDown:
        return 180;
    case ExifRotations::RotatedRight:
        return 270;
    default:
        // Ignore flip/mirroring values (2, 4, 5, 7)
        return 0;
    }
}

ExifRotations ExifExtensions::ConvertDegreesRotationToExifOrientation(unsigned int angle)
{
    switch (angle)
    {
    case 0:
        return ExifRotations::NotRotated;
    case 90:
        return ExifRotations::RotatedLeft;
    case 180:
        return ExifRotations::RotatedDown;
    case 270:
        return ExifRotations::RotatedRight;
    default:
        // Ignore flip/mirroring values (2, 4, 5, 7)
        return ExifRotations::NotRotated;
    }

}

