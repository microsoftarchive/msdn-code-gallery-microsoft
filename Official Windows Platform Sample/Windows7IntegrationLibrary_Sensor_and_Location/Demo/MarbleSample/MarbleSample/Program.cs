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
using Windows7.Sensors;

namespace Microsoft.Sensors.Samples
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GameMarbleBounce game = new GameMarbleBounce())
            {
                game.Run();
            }
        }

        static void OnSensorDeatched(Sensor sensor)
        {
            Console.WriteLine("Sensor detached.");
        }

        static void OnSensorStateChanged(Sensor sensor, SensorState state)
        {
            Console.WriteLine("Sensor {0} state changed to {1}", sensor.FriendlyName, state);
        }

        static void OnSensorAttached(Sensor sensor, SensorState state)
        {
            Console.WriteLine("Sensor {0} attached in state {1}", sensor.FriendlyName, state);
        }
    }
}