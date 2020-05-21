// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

namespace Microsoft.Sensors.Samples
{
    /// <summary>
    /// Specifies the angle of the accelerometer.
    /// </summary>
    public enum Orientation : int
    {
        /// <summary>
        /// The accelerometer is oriented at 0 degrees.
        /// </summary>
        Angle0 = 0,
        /// <summary>
        /// The accelerometer is oriented at 90 degrees
        /// </summary>
        Angle90 = 1,
        /// <summary>
        /// The accelerometer is oriented at 180 degrees
        /// </summary>
        Angle180 = 2,
        /// <summary>
        /// The accelerometer is oriented at 270 degrees
        /// </summary>
        Angle270 = 3
    }
}