/****************************** Module Header ******************************\
* Module Name:  UCNetworkAdapter.cs
* Project:	    CSWMIEnableDisableNetworkAdapter
* Copyright (c) Microsoft Corporation.
* 
* This is the user control which shows information of a Network Adapter in the main form.
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
using System.Drawing;
using System.Windows.Forms;
using CSWMIEnableDisableNetworkAdapter.Properties;

namespace CSWMIEnableDisableNetworkAdapter
{
    internal partial class UcNetworkAdapter : UserControl
    {

        #region Constuct UCNetworkAdapter

        public UcNetworkAdapter()
        {
            InitializeComponent();
        }

        /// <summary>
        /// An User Control handle a Network Adapter Information
        /// </summary>
        /// <param name="networkAdapter">The NetworkAdapter object 
        /// showed in the UCNetworkAdapter</param>
        /// <param name="eventHandler">The EventHandler for btnEnableDisable 
        /// in the UCNetworkAdapter</param>
        /// <param name="point">The location of the UCNetworkAdapter</param>
        /// <param name="parent">The parent control which contained 
        /// the UCNetworkAdapter</param>
        public UcNetworkAdapter(NetworkAdapter networkAdapter, 
            EventHandler eventHandler,
            Point point,
            Control parent)
        {
            InitializeComponent();
            pctNetworkAdapter.Image = (networkAdapter.NetEnabled > 0) 
                ? Resources.ImgEnabledNetworkAdapter 
                : Resources.ImgDisabledNetworkAdapter;
            lbProductName.Text = networkAdapter.Name;
            lbConnectionStatus.Text = 
                NetworkAdapter.SaNetConnectionStatus[networkAdapter.NetConnectionStatus];
            btnEnableDisable.Text = (networkAdapter.NetEnabled > 0) 
                ? Resources.BtnText_Disable : Resources.BtnText_Enable;
            btnEnableDisable.Tag = 
                new[]{networkAdapter.DeviceId,networkAdapter.NetEnabled};
            btnEnableDisable.Click += eventHandler;
            Location = point;
            Parent = parent;
        }

        #endregion 

    }
}
