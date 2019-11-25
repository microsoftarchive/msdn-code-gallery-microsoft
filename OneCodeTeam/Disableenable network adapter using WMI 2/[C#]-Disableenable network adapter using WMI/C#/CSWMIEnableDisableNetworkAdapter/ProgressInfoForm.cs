/****************************** Module Header ******************************\
* Module Name:  ProgressInfo.cs
* Project:	    CSWMIEnableDisableNetworkAdapter
* Copyright (c) Microsoft Corporation.
* 
* This is a form that shows the progress information while enabling or 
* disabling a Network Adapter.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Drawing;
using System.Windows.Forms;


namespace CSWMIEnableDisableNetworkAdapter
{
    public partial class ProgressInfoForm : Form
    {

        #region Construct ProgressInfo

        public ProgressInfoForm()
        {
            InitializeComponent();
        }

        #endregion 

        #region Properties

        /// <summary>
        /// Form.Locattion.X
        /// </summary>
        public int LocationX
        {
            get;
            set;
        }

        /// <summary>
        /// Form.Locattion.Y
        /// </summary>
        public int LocationY
        {
            get;
            set;
        }

        #endregion

        #region Event Handler

        private void ProgressInfoLoad(object sender, System.EventArgs e)
        {
            Location = new Point(LocationX, LocationY);
        }

        #endregion
    }
}
