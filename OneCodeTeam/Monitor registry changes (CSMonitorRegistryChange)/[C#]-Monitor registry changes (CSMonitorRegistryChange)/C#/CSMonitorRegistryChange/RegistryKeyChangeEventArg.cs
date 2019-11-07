/****************************** Module Header ******************************\
* Module Name:  RegistryKeyChangeEventArgs.cs
* Project:	    CSMonitorRegistryChange
* Copyright (c) Microsoft Corporation.
* 
* This class derived from EventArgs. It is used to wrap the ManagementBaseObject of
* EventArrivedEventArgs.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Management;

namespace CSMonitorRegistryChange
{
    class RegistryKeyChangeEventArgs : EventArgs
    {
        public string Hive { get; set; }
        public string KeyPath { get; set; }
        public uint[] SECURITY_DESCRIPTOR { get; set; }
        public DateTime TIME_CREATED { get; set; }

        public RegistryKeyChangeEventArgs(ManagementBaseObject arrivedEvent)
        {

            // Class RegistryKeyChangeEvent has following properties: Hive, KeyPath, 
            // SECURITY_DESCRIPTOR and TIME_CREATED. These properties could get from
            // arrivedEvent.Properties.
            this.Hive = arrivedEvent.Properties["Hive"].Value as string;
            this.KeyPath = arrivedEvent.Properties["KeyPath"].Value as string;

            // The property TIME_CREATED is a unique value that indicates the time 
            // when an event is generated. 
            // This is a 64-bit FILETIME value that represents the number of 
            // 100-nanosecond intervals after January 1, 1601. The information is in
            // the Coordinated Universal Time (UTC) format. 
            this.TIME_CREATED = new DateTime(
                (long)(ulong)arrivedEvent.Properties["TIME_CREATED"].Value,
                DateTimeKind.Utc).AddYears(1600);
        }
    }
}
