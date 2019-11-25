/****************************** Module Header ******************************\
* Module Name:  NetworkAdapter.cs
* Project:	    CSWMIEnableDisableNetworkAdapter
* Copyright (c) Microsoft Corporation.
* 
* The class NetworkAdapter supplies following features:
* 1. Get all the NetworkAdapters of the machine
* 2. Get NetEnabled Status of the NetworkAdapter  
* 3. Enable\Disable a NetworkAdapter
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using CSWMIEnableDisableNetworkAdapter.Properties;

namespace CSWMIEnableDisableNetworkAdapter
{
    internal class NetworkAdapter
    {
        #region NetworkAdapter Properties

        /// <summary>
        /// The DeviceID of the NetworkAdapter
        /// </summary>
        public int DeviceId
        {
            get;
            private set;
        }
        
        /// <summary>
        /// The ProductName of the NetworkAdapter
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// The NetEnabled status of the NetworkAdapter
        /// </summary>
        public int NetEnabled
        {
            get;
            private set;
        }

        /// <summary>
        /// The Net Connection Status Value
        /// </summary>
        public int NetConnectionStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// The Net Connection Status Description
        /// </summary>
        public static string[] SaNetConnectionStatus = 
        { 
            Resources.NetConnectionStatus0,
            Resources.NetConnectionStatus1,
            Resources.NetConnectionStatus2,
            Resources.NetConnectionStatus3,
            Resources.NetConnectionStatus4,
            Resources.NetConnectionStatus5,
            Resources.NetConnectionStatus6,
            Resources.NetConnectionStatus7,
            Resources.NetConnectionStatus8,
            Resources.NetConnectionStatus9,
            Resources.NetConnectionStatus10,
            Resources.NetConnectionStatus11,
            Resources.NetConnectionStatus12
        };

        /// <summary>
        /// Enum the NetEnabled Status
        /// </summary>
        private enum EnumNetEnabledStatus
        { 
            Disabled = -1,
            Unknow,
            Enabled
        }

        /// <summary>
        /// Enum the Operation result of Enable and Disable  Network Adapter
        /// </summary>
        private enum EnumEnableDisableResult
        {
            Fail = -1,
            Unknow,
            Success
        }

        #endregion

        #region Construct NetworkAdapter

        public NetworkAdapter(int deviceId,
            string name,  
            int netEnabled, 
            int netConnectionStatus)
        {
            DeviceId = deviceId;
            Name = name;
            NetEnabled = netEnabled;
            NetConnectionStatus = netConnectionStatus;
        }

        public NetworkAdapter(int deviceId)
        {
            ManagementObject crtNetworkAdapter = new ManagementObject();
            string strWQuery = string.Format("SELECT DeviceID, ProductName, "
                + "NetEnabled, NetConnectionStatus "
                + "FROM Win32_NetworkAdapter " 
                + "WHERE DeviceID = {0}", deviceId);
            try
            {
                ManagementObjectCollection networkAdapters 
                    = WMIOperation.WMIQuery(strWQuery);
            
                foreach (ManagementObject networkAdapter in networkAdapters)
                {
                    // Expected to be only one ManagementObject instance.
                    crtNetworkAdapter = networkAdapter;
                    break;
                }
            
                DeviceId = deviceId;
                Name = crtNetworkAdapter["ProductName"].ToString();
                NetEnabled = (
                    Convert.ToBoolean(crtNetworkAdapter["NetEnabled"].ToString())) 
                    ? (int)EnumNetEnabledStatus.Enabled 
                    : (int)EnumNetEnabledStatus.Disabled;

                NetConnectionStatus = Convert.ToInt32(
                    crtNetworkAdapter["NetConnectionStatus"].ToString());
            }
            catch(NullReferenceException)
            {
                // If there is no a network adapter which deviceid equates to the argument 
                // "deviceId" just to construct a none exists network adapter
                DeviceId = -1;
                Name = string.Empty;
                NetEnabled = 0;
                NetConnectionStatus = -1;
            }
        }

        #endregion

        #region Get NetEnabled Status Of The NetworkAdapter

        /// <summary>
        /// Get the NetworkAdapter Netenabled Property
        /// </summary>
        /// <returns>Whether the NetworkAdapter is enabled</returns>
        public int GetNetEnabled()
        {
            int netEnabled = (int)EnumNetEnabledStatus.Unknow;
            string strWQuery =string.Format("SELECT NetEnabled FROM Win32_NetworkAdapter " 
                + "WHERE DeviceID = {0}", DeviceId);
            try
            {
                ManagementObjectCollection networkAdapters = 
                    WMIOperation.WMIQuery(strWQuery);
                foreach (ManagementObject networkAdapter in networkAdapters)
                {
                    netEnabled =
                        (Convert.ToBoolean(networkAdapter["NetEnabled"].ToString()))
                                     ? (int) EnumNetEnabledStatus.Enabled
                                     : (int) EnumNetEnabledStatus.Disabled;
                }
            }
            catch(NullReferenceException)
            {
                // If NullReferenceException return (EnumNetEnabledStatus.Unknow)
            }
            return netEnabled;
        }

        #endregion

        #region Get All NetworkAdapters

        /// <summary>
        /// List all the NetworkAdapters
        /// </summary>
        /// <returns>The list of all NetworkAdapter of the machine</returns>
        public static List<NetworkAdapter> GetAllNetworkAdapter()
        {
            List<NetworkAdapter> allNetworkAdapter = new List<NetworkAdapter>();

            // Manufacturer <> 'Microsoft'to get all none virtual devices.
            // Because the AdapterType property will be null if the NetworkAdapter is 
            // disabled, so we do not use NetworkAdapter = 'Ethernet 802.3' or 
            // NetworkAdapter = 'Wireless’
            string strWQuery = "SELECT DeviceID, ProductName, "
                + "NetEnabled, NetConnectionStatus "
                + "FROM Win32_NetworkAdapter " 
                + "WHERE Manufacturer <> 'Microsoft'";

            ManagementObjectCollection networkAdapters = WMIOperation.WMIQuery(strWQuery);
            foreach (ManagementObject moNetworkAdapter in networkAdapters)
            {
                try
                {
                    allNetworkAdapter.Add(new NetworkAdapter(
                        Convert.ToInt32(moNetworkAdapter["DeviceID"].ToString()),
                        moNetworkAdapter["ProductName"].ToString(),
                        (Convert.ToBoolean(moNetworkAdapter["NetEnabled"].ToString())) 
                            ? (int)EnumNetEnabledStatus.Enabled 
                            : (int)EnumNetEnabledStatus.Disabled,
                        Convert.ToInt32(moNetworkAdapter["NetConnectionStatus"].ToString()
                        )));
                }
                catch(NullReferenceException)
                {
                    // Ignore some other devices (like the bluetooth), that need user 
                    // interaction to enable or disable.
                }
            }

            return allNetworkAdapter;
        }

        #endregion

        #region Enable Or Disable The Network Adapter

        /// <summary>
        /// Enable Or Disable The NetworkAdapter
        /// </summary>
        /// <returns>
        /// Whether the NetworkAdapter was enabled or disabled successfully
        /// </returns>
        public int EnableOrDisableNetworkAdapter(string strOperation)
        {
            int resultEnableDisableNetworkAdapter = (int)EnumEnableDisableResult.Unknow;
            ManagementObject crtNetworkAdapter = new ManagementObject();

            string strWQuery = string.Format("SELECT DeviceID, ProductName, " 
                + "NetEnabled, NetConnectionStatus " 
                + "FROM Win32_NetworkAdapter " + "WHERE DeviceID = {0}", DeviceId);

            try
            {
                ManagementObjectCollection networkAdapters = 
                    WMIOperation.WMIQuery(strWQuery);
                foreach (ManagementObject networkAdapter in networkAdapters)
                {
                    crtNetworkAdapter = networkAdapter;
                }

                crtNetworkAdapter.InvokeMethod(strOperation, null);

                Thread.Sleep(500);
                while (GetNetEnabled() != ((strOperation.Trim() == "Enable")
                                                ? (int) EnumNetEnabledStatus.Enabled
                                                : (int) EnumNetEnabledStatus.Disabled))
                {
                    Thread.Sleep(100);
                }

                resultEnableDisableNetworkAdapter = 
                    (int) EnumEnableDisableResult.Success;
            }
            catch(NullReferenceException)
            {
                // If there is a NullReferenceException, the result of the enable or 
                // disable network adapter operation will be fail
                resultEnableDisableNetworkAdapter = (int)EnumEnableDisableResult.Fail;
            }

            crtNetworkAdapter.Dispose();

            return resultEnableDisableNetworkAdapter;
        }

        #endregion

    }
}
