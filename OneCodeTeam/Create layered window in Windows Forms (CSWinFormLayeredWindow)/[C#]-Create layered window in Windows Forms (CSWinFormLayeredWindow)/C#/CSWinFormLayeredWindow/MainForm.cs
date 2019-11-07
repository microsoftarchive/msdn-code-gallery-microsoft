/******************************** Module Header ********************************\
Module Name:  MainForm.cs
Project:      CSWinFormLayeredWindow
Copyright (c) Microsoft Corporation.



This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*******************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CSWinFormLayeredWindow.Properties;
#endregion


namespace CSWinFormLayeredWindow
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        PerPixelAlphaForm alphaWindow = null;


        private void btnShowAlphaWindow_Click(object sender, EventArgs e)
        {
            if (!OSFeature.Feature.IsPresent(OSFeature.LayeredWindows))
            {
                MessageBox.Show("Layered window is not supported in the current system");
                return;
            }

            if (alphaWindow == null)
            {
                alphaWindow = new PerPixelAlphaForm();
            }

            alphaWindow.Show();
            alphaWindow.SelectBitmap(Resources.Ring, this.trackBarOpacity.Value);
        }


        private void trackBarOpacity_ValueChanged(object sender, EventArgs e)
        {
            this.lbOpacity.Text = this.trackBarOpacity.Value.ToString();

            if (alphaWindow != null)
            {
                alphaWindow.SelectBitmap(Resources.Ring, this.trackBarOpacity.Value);
            }
        }
    }
}