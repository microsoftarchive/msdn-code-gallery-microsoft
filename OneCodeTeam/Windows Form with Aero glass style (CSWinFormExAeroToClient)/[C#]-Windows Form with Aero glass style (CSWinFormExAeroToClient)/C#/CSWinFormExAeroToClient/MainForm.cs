/****************************** Module Header ******************************\
 * Module Name:  MainForm.cs
 * Project:      CSWinFormExAeroToClient
 * Copyright (c) Microsoft Corporation.
 * 
 * This is the Main UI of this application. The user could set the Aero effect
 * of the demo Form.
 * 
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
using System.Drawing;
using System.Windows.Forms;

namespace CSWinFormExAeroToClient
{
    public partial class MainForm : Form
    {
        GlassForm demoForm = null;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Show the Demo Form or change the style of the Demo Form.
        /// </summary>
        private void btnApply_Click(object sender, EventArgs e)
        {

            if (demoForm == null || demoForm.IsDisposed)
            {
                demoForm = new GlassForm();
                demoForm.Text = "Demo Form";
                demoForm.DWMCompositionChanged += demoForm_DWMCompositionChanged;
                demoForm.Show();
            }

            try
            {
                demoForm.ExtendFrameEnabled = chkExtendFrame.Checked;
                if (demoForm.ExtendFrameEnabled)
                {
                    if (chkEntendToEntireClientArea.Checked)
                    {
                        demoForm.GlassMargins = new NativeMethods.MARGINS(-1);
                    }
                    else
                    {
                        demoForm.GlassMargins = new NativeMethods.MARGINS(
                            int.Parse(tbLeft.Text),
                            int.Parse(tbRight.Text),
                            int.Parse(tbTop.Text),
                            int.Parse(tbBottom.Text));
                    }
                }

                demoForm.BlurBehindWindowEnabled = chkBlurBehindWindow.Checked;
                if (demoForm.BlurBehindWindowEnabled)
                {
                    if (chkEnableEntireFormBlurEffect.Checked)
                    {
                        demoForm.BlurRegion = null;
                    }
                    else
                    {
                        demoForm.BlurRegion = new Region(new Rectangle(
                            int.Parse(tbX.Text),
                            int.Parse(tbY.Text),
                            int.Parse(tbWidth.Text),
                            int.Parse(tbHeight.Text)));
                    }
                }

                demoForm.Invalidate();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Display whether the Aero Glass Style is enabled.
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateUI(GlassForm.IsAeroGlassStyleSupported());            
        }

        /// <summary>
        /// Update the UI if the DWMComposition Changed.
        /// </summary>
        private void demoForm_DWMCompositionChanged(object sender, EventArgs e)
        {
            UpdateUI(GlassForm.IsAeroGlassStyleSupported());
        }

        /// <summary>
        /// Enable or disable some controls if the DWMComposition Changed.
        /// </summary>
        /// <param name="isAeroGlassStyleSupported"></param>
        private void UpdateUI(bool isAeroGlassStyleSupported)
        {
            this.lbAeroGlassStyleSupported.Text = string.Format(
               "Aero GlassStyle Supported : {0} ",
               isAeroGlassStyleSupported);

            if (isAeroGlassStyleSupported)
            {
                chkExtendFrame.Enabled = true;
                chkBlurBehindWindow.Enabled = true;
                chkEntendToEntireClientArea.Enabled = true;
                chkEnableEntireFormBlurEffect.Enabled = true;
                btnApply.Enabled = true;
            }
            else
            {
                chkExtendFrame.Enabled = false;
                chkBlurBehindWindow.Enabled = false;
                chkEntendToEntireClientArea.Enabled = false;
                chkEnableEntireFormBlurEffect.Enabled = false;
                btnApply.Enabled = false;

                chkExtendFrame.Checked = false;
                chkBlurBehindWindow.Checked = false;
                chkEntendToEntireClientArea.Checked = false;
                chkEnableEntireFormBlurEffect.Checked = false;
            }
        }
    }
}
