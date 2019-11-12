/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:	    CSMonitorRegistryChange
* Copyright (c) Microsoft Corporation.
* 
* This is the main form of this application. It is used to initialize the UI and 
* handle the events.
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
using System.Windows.Forms;
using Microsoft.Win32;

namespace CSMonitorRegistryChange
{
    public partial class MainForm : Form
    {

        #region Fields

        // Current status
        bool isMonitoring = false;

        RegistryWatcher watcher = null;

        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            // Initialize the data source of cmbHives. Changes to the HKEY_CLASSES_ROOT 
            // and HKEY_CURRENT_USER hives are not supported by RegistryEvent or classes
            // derived from it, such as RegistryKeyChangeEvent.        
            cmbHives.DataSource = RegistryWatcher.SupportedHives;

        }

        /// <summary>
        /// Handle the click event of btnMonitor.
        /// </summary>
        private void btnMonitor_Click(object sender, EventArgs e)
        {

            // If this application is monitoring the registry key, then stop monitoring 
            // and enable the editors.
            if (isMonitoring)
            {
                bool success = StopMonitor();
                if (success)
                {
                    btnMonitor.Text = "Start Monitor";
                    cmbHives.Enabled = true;
                    tbRegkeyPath.ReadOnly = false;
                    isMonitoring = false;
                    lstChanges.Items.Add(string.Format("{0} Stop monitoring", DateTime.Now));
                }
            }

            // If this application is idle, then start to monitor and disable the editors.
            else
            {
                bool success = StartMonitor();
                if (success)
                {
                    btnMonitor.Text = "Stop Monitor";
                    cmbHives.Enabled = false;
                    tbRegkeyPath.ReadOnly = true;
                    isMonitoring = true;
                    lstChanges.Items.Add(string.Format("{0} Start monitoring", DateTime.Now));
                }
            }

        }

        /// <summary>
        /// Check whether the key to be monitored exists, and then 
        /// start ManagementEventWatcher to watch the RegistryKeyChangeEvent
        /// </summary>
        /// <returns>True if the ManagementEventWatcher starts successfully.</returns>
        bool StartMonitor()
        {
            RegistryKey hive = cmbHives.SelectedValue as RegistryKey;
            var keyPath = tbRegkeyPath.Text.Trim();
            
            try
            {
                watcher = new RegistryWatcher(hive, keyPath);
            }
            
            // The constructor of RegistryWatcher may throw a SecurityException when
            // the key to monitor does not exist. 
            catch (System.ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
           
            // The constructor of RegistryWatcher may throw a SecurityException when
            // current user does not have the permission to access the key to monitor. 
            catch (System.Security.SecurityException)
            {
                string message = string.Format(
                    @"You do not have permission to access the key {0}\{1}.",
                    hive.Name,
                    keyPath);
                MessageBox.Show(message);
                return false;
            }
                      
            try
            {
                          
                // Set up the handler that will handle the change event.
                watcher.RegistryKeyChangeEvent += new EventHandler<RegistryKeyChangeEventArgs>(
                    watcher_RegistryKeyChangeEvent);

                // Start listening for events.
                watcher.Start();
                return true;
            }
            catch (System.Runtime.InteropServices.COMException comException)
            {
                MessageBox.Show("An error occurred: " + comException.Message);
                return false;
            }
            catch (ManagementException managementException)
            {
                MessageBox.Show("An error occurred: " + managementException.Message);
                return false;
            }

        }
     
        /// <summary>
        /// Stop listening for events.
        /// </summary>
        /// <returns>True if ManagementEventWatcher stops successfully.</returns>
        bool StopMonitor()
        {
            try
            {
                watcher.Stop();
                return true;
            }
            catch (ManagementException managementException)
            {
                MessageBox.Show("An error occurred: " + managementException.Message);
                return false;
            }
            finally
            {
                watcher.Dispose();
            }
        }

        /// <summary>
        /// Handle the RegistryKeyChangeEvent.
        /// </summary>
        void watcher_RegistryKeyChangeEvent(object sender, RegistryKeyChangeEventArgs e)
        {         
            string newEventMessage = string.Format(@"{0} The key {1}\{2} changed",
                e.TIME_CREATED.ToLocalTime(), e.Hive, e.KeyPath);
            lstChanges.Items.Add(newEventMessage);
        }
    }
}
