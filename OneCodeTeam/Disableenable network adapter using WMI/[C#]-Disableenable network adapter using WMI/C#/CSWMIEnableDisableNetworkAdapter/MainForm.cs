/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:	    CSWMIEnableDisableNetworkAdapter
* Copyright (c) Microsoft Corporation.
* 
* This is the main form of this application. It is used to initialize the UI  
* and handle the events. 
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
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Principal;
using CSWMIEnableDisableNetworkAdapter.Properties;


namespace CSWMIEnableDisableNetworkAdapter
{
    public partial class MainForm : Form
    {

        #region Private Properties

        /// <summary>
        /// All Network Adapters in the machine
        /// </summary>
        private List<NetworkAdapter> _allNetworkAdapters = new List<NetworkAdapter>();

        /// <summary>
        /// A ProgressInfo form
        /// </summary>
        private ProgressInfoForm _progressInfoForm = new ProgressInfoForm();
        
        /// <summary>
        /// The Current Operation Network Adapter
        /// </summary>
        private NetworkAdapter _currentNetworkAdapter = null;

        #endregion

        #region Construct EnableDisableNetworkAdapter

        /// <summary>
        /// Construct an EnableDisableNetworkAdapter
        /// </summary>
        public MainForm()
        {
            if (isAdministrator())
            {
                InitializeComponent();
                ShowAllNetworkAdapters();
                tsslbResult.Text = string.Format("{0}[{1}]", 
                    Resources.StatusTextInitial, 
                    _allNetworkAdapters.Count);
            }
            else
            {
                MessageBox.Show(Resources.MsgElevatedRequire, 
                    Resources.OneCodeCaption, 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                Environment.Exit(1);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// You need to run this sample as Administrator
        /// Check whether the application is run as administrator
        /// </summary>
        /// <returns>Whether the application is run as administrator</returns>      
        private bool isAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        } 


        /// <summary>
        /// Show all Network Adapters in the Enable\DisableNetworkAdapter window
        /// </summary>
        private void ShowAllNetworkAdapters()
        {
            grpNetworkAdapters.Controls.Clear();
            
            _allNetworkAdapters = NetworkAdapter.GetAllNetworkAdapter();
            int i = 0;

            foreach (NetworkAdapter networkAdapter in _allNetworkAdapters)
            {
                i++;
                UcNetworkAdapter ucNetworkAdapter = new UcNetworkAdapter(
                    networkAdapter, 
                    BtnEnableDisableNetworAdaptetClick, 
                    new Point(10, 30 * i), 
                    grpNetworkAdapters);
            }
        }

        /// <summary>
        /// Show progress info while enabling or disabling a Network Adapter.
        /// </summary>
        private void ShowProgressInfo()
        {
            tsslbResult.Text = string.Empty;
            foreach (Control c in _progressInfoForm.Controls)
            {
                if (c is Label)
                {
                    c.Text = string.Format("{0}[{1}] ({2}) {3}", 
                        Resources.StatusTextBegin, 
                        _currentNetworkAdapter.DeviceId, 
                        _currentNetworkAdapter.Name, 
                        ((_currentNetworkAdapter.GetNetEnabled() != 1) 
                        ? Resources.ProgressTextEnableEnd 
                        : Resources.ProgressTextDisableEnd));
                }
            }

            _progressInfoForm.LocationX = Location.X
                + (Width - _progressInfoForm.Width) / 2;
            _progressInfoForm.LocationY = Location.Y
                + (Height - _progressInfoForm.Height) / 2;

            _progressInfoForm.ShowDialog();
        }

        #endregion

        #region Event Handler

        /// <summary>
        /// Button on click event handler
        /// Click enable or disable the network adapter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BtnEnableDisableNetworAdaptetClick(object sender, EventArgs e)
        {
            Button btnEnableDisableNetworkAdapter = (Button)sender;

            // The result of enable or disable Network Adapter
            // result ={-1: Fail;0:Unknow;1:Success}
            int result = -1;
            int deviceId = ((int[])btnEnableDisableNetworkAdapter.Tag)[0];
            
            Thread showProgressInfoThreadProc = 
                    new Thread(ShowProgressInfo);
            try
            {
                _currentNetworkAdapter = new NetworkAdapter(deviceId);

                // To avoid the condition of the network adapter netenable change caused 
                // by any other tool or operation (ex. Disconnected the Media) after you 
                // start the sample and before you click the enable\disable button.
                // In this case, the network adapter status shown in windows form is not
                // meet the real status.

                // If the network adapters' status is shown corrected,just to enable or 
                // disable it as usual.
                if (((int[]) btnEnableDisableNetworkAdapter.Tag)[1]
                    == _currentNetworkAdapter.NetEnabled)
                {
                    // If Network Adapter NetConnectionStatus in ("Hardware not present",
                    // Hardware disabled","Hardware malfunction","Media disconnected"), 
                    // it will can not be enabled.
                    if (_currentNetworkAdapter.NetEnabled == -1
                        && (_currentNetworkAdapter.NetConnectionStatus >= 4
                            && _currentNetworkAdapter.NetConnectionStatus <= 7))
                    {
                        string error =
                            String.Format("{0}({1}) [{2}] {3} [{4}]",
                                          Resources.StatusTextBegin,
                                          _currentNetworkAdapter.DeviceId,
                                          Name,
                                          Resources.CanNotEnableMsg,
                                          NetworkAdapter.SaNetConnectionStatus
                                              [_currentNetworkAdapter
                                              .NetConnectionStatus]);

                        MessageBox.Show(error,
                                        Resources.OneCodeCaption,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                    }
                    else
                    {
                        showProgressInfoThreadProc.Start();

                        result = _currentNetworkAdapter.EnableOrDisableNetworkAdapter(
                            (_currentNetworkAdapter.NetEnabled == 1) 
                            ? "Disable" : "Enable");

                        showProgressInfoThreadProc.Abort();
                    }
                }
                // If the network adapter status are not shown corrected, just to refresh
                // the form to show the correct network adapter status.
                else
                {
                    ShowAllNetworkAdapters();
                    result = 1;
                }
            }
            catch(NullReferenceException)
            {
                // If failed to construct _currentNetworkAdapter the result will be fail.
            }

            // If successfully enable or disable the Network Adapter
            if (result > 0)
            {
                ShowAllNetworkAdapters();
                tsslbResult.Text = 
                    string.Format("{0}[{1}] ({2}) {3}", 
                    Resources.StatusTextBegin, 
                    _currentNetworkAdapter.DeviceId, 
                    _currentNetworkAdapter.Name,
                    ((((int[])btnEnableDisableNetworkAdapter.Tag)[1] == 1)
                    ? Resources.StatusTextSuccessDisableEnd 
                    : Resources.StatusTextSuccessEnableEnd)) ;

                tsslbResult.ForeColor = Color.Green;
            }
            else
            {
                tsslbResult.Text = 
                    string.Format("{0}[{1}] ({2}) {3}", 
                    Resources.StatusTextBegin, 
                    _currentNetworkAdapter.DeviceId, 
                    _currentNetworkAdapter.Name,
                    ((((int[])btnEnableDisableNetworkAdapter.Tag)[1] == 1) 
                    ? Resources.StatusTextFailDisableEnd 
                    : Resources.StatusTextFailEnableEnd));

                tsslbResult.ForeColor = Color.Red;
            }
        }

        #endregion 
    }
}
