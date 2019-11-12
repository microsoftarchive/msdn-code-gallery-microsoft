/****************************** Module Header ******************************\
* Module Name:  RegistryWatcher.cs
* Project:	    CSMonitorRegistryChange
* Copyright (c) Microsoft Corporation.
* 
* This class derived from ManagementEventWatcher. It is used to 
* 1. Supply the supported hives.
* 2. Construct a WqlEventQuery from Hive and KeyPath.
* 3. Wrap the EventArrivedEventArgs to RegistryKeyChangeEventArg.
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
using System.Collections.ObjectModel;
using System.Management;
using Microsoft.Win32;

namespace CSMonitorRegistryChange
{
    class RegistryWatcher : ManagementEventWatcher, IDisposable
    {

        static ReadOnlyCollection<RegistryKey> supportedHives = null;

        /// <summary>
        /// Changes to the HKEY_CLASSES_ROOT and HKEY_CURRENT_USER hives are not supported
        /// by RegistryEvent or classes derived from it, such as RegistryKeyChangeEvent. 
        /// </summary>
        public static ReadOnlyCollection<RegistryKey> SupportedHives
        {
            get
            {
                if (supportedHives == null)
                {
                    RegistryKey[] hives = new RegistryKey[] 
                    {
                        Registry.LocalMachine,
                        Registry.Users,
                        Registry.CurrentConfig
                    };
                    supportedHives = Array.AsReadOnly<RegistryKey>(hives);
                }
                return supportedHives;
            }
        }

        public RegistryKey Hive { get; private set; }
        public string KeyPath { get; private set; }
        public RegistryKey KeyToMonitor { get; private set; }

        public event EventHandler<RegistryKeyChangeEventArgs> RegistryKeyChangeEvent;

        /// <exception cref="System.Security.SecurityException">
        /// Thrown when current user does not have the permission to access the key 
        /// to monitor.
        /// </exception> 
        /// <exception cref="System.ArgumentException">
        /// Thrown when the key to monitor does not exist.
        /// </exception> 
        public RegistryWatcher(RegistryKey hive, string keyPath)
        {
            this.Hive = hive;
            this.KeyPath = keyPath;

            // If you set the platform of this project to x86 and run it on a 64bit 
            // machine, you will get the Registry Key under 
            // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node when the key path is
            // HKEY_LOCAL_MACHINE\SOFTWARE
            this.KeyToMonitor = hive.OpenSubKey(keyPath);

            if (KeyToMonitor != null)
            {
                // Construct the query string.
                string queryString = string.Format(@"SELECT * FROM RegistryKeyChangeEvent 
                   WHERE Hive = '{0}' AND KeyPath = '{1}' ", this.Hive.Name, this.KeyPath);

                WqlEventQuery query = new WqlEventQuery();
                query.QueryString = queryString;
                query.EventClassName = "RegistryKeyChangeEvent";
                query.WithinInterval = new TimeSpan(0, 0, 0, 1);
                this.Query = query;

                this.EventArrived += new EventArrivedEventHandler(RegistryWatcher_EventArrived);
            }
            else
            {
                string message = string.Format(
                    @"The registry key {0}\{1} does not exist",
                    hive.Name,
                    keyPath);
                throw new ArgumentException(message);
            }
        }

        void RegistryWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (RegistryKeyChangeEvent != null)
            {
                // Get RegistryKeyChangeEventArgs from EventArrivedEventArgs.NewEvent.Properties.
                RegistryKeyChangeEventArgs args = new RegistryKeyChangeEventArgs(e.NewEvent);

                // Raise the event handler. 
                RegistryKeyChangeEvent(sender, args);
            }
        }

        /// <summary>
        /// Dispose the RegistryKey.
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            if (this.KeyToMonitor != null)
            {
                this.KeyToMonitor.Dispose();
            }
        }
    }
}
