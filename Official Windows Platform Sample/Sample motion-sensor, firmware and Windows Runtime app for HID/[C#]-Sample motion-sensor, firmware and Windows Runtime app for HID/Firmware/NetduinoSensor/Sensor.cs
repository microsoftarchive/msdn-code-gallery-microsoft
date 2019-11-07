/* Custom USB HID Communication Device - Echo Device
 * Copyright (c) Secret Labs LLC. All Rights Reserved.
 * 
 * Licensed under the Apache 2.0 open source license
 */

// 
// The starting point for this firmware was the UsbHidEchoNetduinoApp sample that is found on the Netduino forum
// see:  http://forums.netduino.com/index.php?/topic/1514-driverless-pc-netduino-communication-using-usb/
// For license terms see: http://www.apache.org/licenses/
//
// The PIR class (below) is based on a code snippet also found on the Netduino forum
// see: http://forums.netduino.com/index.php?/topic/1246-parallax-pir-sensor-class/
// For license terms see: http://creativecommons.org/licenses/by-sa/3.0/
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
using Microsoft.SPOT;
using System.Threading;
using Microsoft.SPOT.Hardware.UsbClient;
using Microsoft.SPOT.Hardware; 

namespace NetduinoSensor
{
    struct InputReport
    {
        public byte Presence; // 1 if presence detected; 0 otherwise
        public byte Interval; // report interval (or frequency) in seconds
    }

    struct OutputReport
    {
        public byte Interval; // report interval (or frequency) in seconds
    }

    public delegate void PIRTriggeredEventHandler(uint triggered, DateTime time);

    public class PIR{
        private InterruptPort _sensor;        
        public event PIRTriggeredEventHandler SensorTriggered;
        
        /// <summary>        
        /// Initializes a new instance of the PIR class.        
        /// </summary>        
        /// <param name="portId">The port id.</param>        
        public PIR(Cpu.Pin portId){
            _sensor = new InterruptPort(
                portId,                     
                false,                     
                Port.ResistorMode.Disabled,                     
                Port.InterruptMode.InterruptEdgeBoth);
            
            _sensor.OnInterrupt += new NativeEventHandler(
                (port, motion, time) =>
            {
                OnSensorTriggered(port, motion, time);                    
            }            
            );        
        }  
      
        /// <summary>        
        /// Invoked when the sensor changes state  
        /// port identifies the port; we ignore as it's not used
        /// motion specifies the sensor data; we capture it
        /// time specifies the event time; we ignore it
        /// </summary>        
  
        protected void OnSensorTriggered(uint port, uint motion, DateTime time)        
        {
            var evt = SensorTriggered;
            if (evt != null)
                 evt.Invoke(motion, time); 
         }    
    }
 
    class Sensor
    {
        protected const int WRITE_ENDPOINT = 1;
        protected const int READ_ENDPOINT = 2; 
        protected const int MAXPOWER_MILLIAMPS = 200;
        protected const int HID_DESCRIPTOR_TYPE = 0x21;

        public UsbStream stream;
        public UsbController usbController;

        protected bool started;

        protected byte[] hidGenericReportDescriptorPayload;

        public Sensor()
        {

            hidGenericReportDescriptorPayload = new byte[]
            {
                0x06,0x55,0xFF,     //HID_USAGE_PAGE_VENDOR_DEFINED
                0x09,0xA5,          //HID_USAGE (vendor_defined)

                0xA1,0x01,          //HID_COLLECTION(Application),

                // Input report (device-transmits)
                0x09,0xA7,          //HID_USAGE (vendor_defined)
                0x15,0x00,          //HID_LOGICAL_MIN_8(0), // False = not present
                0x25,0x01,          //HID_LOGICAL_MAX_8(1), // True = present
                0x75,0x08,          //HID_REPORT_SIZE(8),
                0x95,0x01,          //HID_REPORT_COUNT(1),
                0x81,0x02,          //HID_INPUT(Data_Var_Abs),

                0x09,0xA8,          //HID_USAGE (vendor_defined)
                0x15,0x01,          //HID_LOGICAL_MIN_8(1), // minimum 1-second
                0x25,0x3C,          //HID_LOGICAL_MAX_8(60), // maximum 60-seconds
                0x75,0x08,          //HID_REPORT_SIZE(8), 
                0x95,0x01,          //HID_REPORT_COUNT(1), 
                0x81,0x02,          //HID_INPUT(Data_Var_Abs),

                // Output report (device-receives)
                0x09,0xA9,          //HID_USAGE (vendor_defined)
                0x15,0x01,          //HID_LOGICAL_MIN_8(1), // minimum 1-second
                0x25,0x3C,          //HID_LOGICAL_MAX_8(60), // maximum 60-seconds
                0x75,0x08,          //HID_REPORT_SIZE(8), 
                0x95,0x01,          //HID_REPORT_COUNT(1), 
                0x91,0x02,          //HID_OUTPUT(Data_Var_Abs),

                0xC0                //HID_END_COLLECTION

            };
        }

        public bool Open()
        {
            bool succeed = true;

            started = false;

            UsbController[] usbControllers = UsbController.GetControllers();

            if (usbControllers.Length < 1)
            {
                succeed = false;
            }

            if (succeed)
            {
                usbController = usbControllers[0];

                try
                {
 
                    succeed = ConfigureHID();

                    if (succeed)
                    {
                        succeed = usbController.Start();
                    }

                    if (succeed)
                    {
                        stream = usbController.CreateUsbStream(WRITE_ENDPOINT, READ_ENDPOINT);
                    }
                }
                catch (Exception)
                {
                    succeed = false;
                }
            }

            started = true;
            return succeed;
        }

        public bool Close()
        {
            bool succeed = true;

            if (started)
            {
                succeed = usbController.Stop();

                if (succeed)
                {
                    started = false;
                }
            }

            return succeed;
        }

        protected bool ConfigureHID()
        {
            Configuration configuration = new Configuration();
            Configuration.DeviceDescriptor deviceDescriptor = new Configuration.DeviceDescriptor(0x16C0, 0x0012, 0x0100);
            deviceDescriptor.bcdUSB = 0x110;
            deviceDescriptor.bDeviceClass = 0;  
            deviceDescriptor.bDeviceSubClass = 0; 
            deviceDescriptor.bMaxPacketSize0 = 8;
            deviceDescriptor.iManufacturer = 1;
            deviceDescriptor.iProduct = 2;
            deviceDescriptor.iSerialNumber = 0;

            byte descriptorLength;

            if (hidGenericReportDescriptorPayload.Length == 0 || hidGenericReportDescriptorPayload.Length <= byte.MaxValue)
            {
                descriptorLength = (byte)hidGenericReportDescriptorPayload.Length;
            }
            else
            {
                return false;
            }

            byte[] hidClassDescriptorPayload = new byte[]
            {
                0x01, 0x11,     // bcdHID (v1.01) || 0x01, 0x01, changed to 0x01, 0x11, 
                0x00,           // bCountryCode
                0x01,           // bNumDescriptors
                0x22,           // bDescriptorType (report)
                descriptorLength, 0x00      // wDescriptorLength (report descriptor size in bytes)
            };

            // HID class descriptor
            Configuration.ClassDescriptor hidClassDescriptor = new Configuration.ClassDescriptor(HID_DESCRIPTOR_TYPE, hidClassDescriptorPayload);
            // write endpoint
            Configuration.Endpoint writeEndpoint = new Configuration.Endpoint(WRITE_ENDPOINT, 
                Configuration.Endpoint.ATTRIB_Interrupt | Configuration.Endpoint.ATTRIB_Write);
            writeEndpoint.wMaxPacketSize = 16;   // packet size: 16 bytes
            writeEndpoint.bInterval = 10;         // interval: 10ms

            // read endpoint
            Configuration.Endpoint readEndpoint = new Configuration.Endpoint(READ_ENDPOINT, 
                Configuration.Endpoint.ATTRIB_Interrupt | Configuration.Endpoint.ATTRIB_Read);
            readEndpoint.wMaxPacketSize = 16;   // packet size: 16 bytes
            readEndpoint.bInterval = 10;         // interval: 10ms

            Configuration.UsbInterface usbInterface = new Configuration.UsbInterface(0, new Configuration.Endpoint[] { writeEndpoint, readEndpoint });
            usbInterface.classDescriptors = new Configuration.ClassDescriptor[] { hidClassDescriptor };
            usbInterface.bInterfaceClass = 0x03; // Must be 0x03 for any HID device. hid1_11.pdf pp.7 
            usbInterface.bInterfaceSubClass = 0x00; // Sensors do not support a boot device, must be zero. pp. 8
            usbInterface.bInterfaceProtocol = 0x00; // No interface protocol is supported. hid1_11.pdf pp. 9

            // configuration descriptor
            Configuration.ConfigurationDescriptor configurationDescriptor = new Configuration.ConfigurationDescriptor(MAXPOWER_MILLIAMPS, new Configuration.UsbInterface[] { usbInterface });
            configurationDescriptor.bmAttributes = Configuration.ConfigurationDescriptor.ATTRIB_Base | Configuration.ConfigurationDescriptor.ATTRIB_SelfPowered;

            Configuration.GenericDescriptor hidGenericReportDescriptor = new Configuration.GenericDescriptor(0x81, 0x2200, hidGenericReportDescriptorPayload);
            hidGenericReportDescriptor.bRequest = 0x06; // GET_DESCRIPTOR
            hidGenericReportDescriptor.wIndex = 0x00; // INTERFACE 0 (zero)

            Configuration.StringDescriptor manufacturerNameStringDescriptor = new Configuration.StringDescriptor(1, "Microsoft Corporation");
            Configuration.StringDescriptor productNameStringDescriptor = new Configuration.StringDescriptor(2, "Custom Device");
            Configuration.StringDescriptor displayNameStringDescriptor = new Configuration.StringDescriptor(4, "Netduino Infrared Sensor");
            Configuration.StringDescriptor friendlyNameStringDescriptor = new Configuration.StringDescriptor(5, "Netduino IR Sensor");

            configuration.descriptors = new Configuration.Descriptor[]
            {
                deviceDescriptor,
                configurationDescriptor, 
                manufacturerNameStringDescriptor, 
                productNameStringDescriptor,
                displayNameStringDescriptor,
                friendlyNameStringDescriptor,
                hidGenericReportDescriptor
            };

            usbController.Configuration = configuration;

            if (usbController.ConfigurationError != UsbController.ConfigError.ConfigOK)
                return false;

            return true;
        }

        public int Update(int iPresence, int iInterval)
        {
            InputReport inputReport = new InputReport();
            byte Interval = 0;
       
            inputReport.Presence = (byte)iPresence;
            inputReport.Interval = (byte)iInterval;

            SendInputReport(inputReport);

            Interval = GetOutputReport();
            return (int)Interval;
        }

        protected byte GetOutputReport()
        {

            byte[] outputReport = new byte[1];
            int bytesRead = 0;

            if (stream.CanRead)
            {
                bytesRead = stream.Read(outputReport, 0, 1);
            }

            if (bytesRead > 0)
                return outputReport[0];
            else
                return 0;
        }

        protected void SendInputReport(InputReport report)
        {
 
            byte[] inputReport = new byte[2];

            inputReport[0] = (byte)report.Presence;
            inputReport[1] = (byte)report.Interval;

            stream.Write(inputReport, 0, 2);
        }

    }
}
