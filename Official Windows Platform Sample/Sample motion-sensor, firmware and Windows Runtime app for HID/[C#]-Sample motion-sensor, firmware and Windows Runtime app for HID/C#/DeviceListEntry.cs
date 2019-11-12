//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using HidInfraredSensor;

namespace HidInfraredSensor
{
    /// <summary>
    /// The class will only expose properties from DeviceInformation that are going to be used
    /// in this sample. Each instance of this class provides information about a single device.
    ///
    /// This class is used by the UI to display device specific information so that
    /// the user can identify which device to use.
    /// </summary>
    public class DeviceListEntry
    {
        private DeviceInformation device;

        public String InstanceId
        {
            get
            {
                return (String)device.Properties[DeviceProperties.DeviceInstanceId];
            }
        }

        /// <summary>
        /// This property returns the DeviceInterfacePath
        /// </summary>
        public String Id
        {
            get
            {
                return device.Id;
            }
        }

        public String Name
        {
            get
            {
                return device.Name;
            }
        }

        /// <summary>
        /// The class is mainly used as a DeviceInformation wrapper so that the UI can bind to a list of these.
        /// </summary>
        /// <param name="deviceInformation"></param>
        public DeviceListEntry(Windows.Devices.Enumeration.DeviceInformation deviceInformation)
        {
            device = deviceInformation;
        }

    }
}
