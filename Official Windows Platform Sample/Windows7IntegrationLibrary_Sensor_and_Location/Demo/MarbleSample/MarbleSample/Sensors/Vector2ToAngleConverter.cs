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

using System;
using Microsoft.Xna.Framework;

namespace Microsoft.Sensors.Samples
{
    /// <summary>
    /// Converts a Vector2 to an angle relative to a specific plane.
    /// </summary>
    public static class Vector2ToAngleConverter
    {
        /// <summary>
        /// The gravitational tolerance for gravity variance from 1.0G.
        /// </summary>
        private const double GravitationalTolerance = 0.2;

        /// <summary>
        /// Converts a double radian value to degrees
        /// </summary>
        private static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        /// <summary>
        /// Determines if the 3d vector represents a normalized gravitational with respect to 1 G.
        /// </summary>
        /// <param name="vectorLength">A vector length representing gravitational force, with 1.0 representing 1G.</param>
        /// <returns>true if the vector is within tolerance, otherwise false.</returns>
        private static bool IsNormalGravitationalForce(double vectorLength)
        {
            return (vectorLength > 1.0 - GravitationalTolerance) && (vectorLength < 1.0 + GravitationalTolerance);
        }

        /// <summary>
        /// Converts a 2D vector to an angle
        /// </summary>
        /// <param name="vector">The 2d gravitational vector to convert</param>
        /// <returns>The angle of the vector around  </returns>
        internal static double? Vector2DToAngle(Vector2 vector)
        {
            // Let's only check vector lengths that are about 1G, otherwise the accelerometer
            // isn't a static value, and we can't really measure gravitational effects (rather
            // we're detecting motion on the system, which isn't good).
            if (IsNormalGravitationalForce(vector.Length()))
            {
                vector.Normalize();
                double radians = Math.Atan2(vector.X, vector.Y);
                double degrees = RadiansToDegrees(radians);
                return degrees;
            }
            return null;
        }
    }
}