/* Custom USB HID Communication Device - Echo Device
 * Copyright (c) Secret Labs LLC. All Rights Reserved.
 * 
 * Licensed under the Apache 2.0 open source license
 */

// 
// This firmware is based on the UsbHidEchoNetduinoApp that is found on the Netduino forum
// see:  http://forums.netduino.com/index.php?/topic/1514-driverless-pc-netduino-communication-using-usb/
// For license terms see: http://www.apache.org/licenses/
//
// This firmware is licensed under the Creative Commons Attribution-ShareAlike 3.0 Unported License. 
// To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/3.0/ or send a letter to 
// Creative Commons, 444 Castro Street, Suite 900, Mountain View, California, 94041, USA. 
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY 
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR 
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT. 
// 

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO.Ports;


namespace NetduinoSensor
{
    public class Program
    {
        public static void Main()
        {
            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
            Sensor sensor = new Sensor();
            int i = 0;
            int State = 0; // This variable is set hi or lo in response to the sensor
            int CurrentInterval = 1; // 1 second intervals is our default
            int NetduinoInterval = CurrentInterval * 1000; 
            int RequestedInterval = 0; // received from host
 
            // The PIR output is attached to PIN 13 on the Netduino Plus
            var PIR = new PIR(Pins.GPIO_PIN_D13);
            PIR.SensorTriggered += new PIRTriggeredEventHandler(
                (triggered, time) =>
                {
                    // Each time the PIR toggles state
                    // we set the State variable
                    State = (int)triggered;
                }            
                );

            if (sensor.Open())
            {
                for (i = 0; i < 5; i++)
                {
                    led.Write(true);
                    Thread.Sleep(1000);
                    led.Write(false);
                }
            }

            while (true)
            {
                // We invoke the Update method every CurrentInterval seconds
                // and pass the current value of State
 
                RequestedInterval = sensor.Update(State, CurrentInterval);

                // Check for a possible new interval requested via an
                // output report.

                if (RequestedInterval != 0)
                {
                    NetduinoInterval = RequestedInterval * 1000;
                    CurrentInterval = RequestedInterval;
                }
                else
                    NetduinoInterval = CurrentInterval * 1000;

                // Toggle the LED every iNetuinoInterval seconds

                led.Write(true);
                Thread.Sleep(NetduinoInterval);
                led.Write(false);
            }
        }

    }
}
