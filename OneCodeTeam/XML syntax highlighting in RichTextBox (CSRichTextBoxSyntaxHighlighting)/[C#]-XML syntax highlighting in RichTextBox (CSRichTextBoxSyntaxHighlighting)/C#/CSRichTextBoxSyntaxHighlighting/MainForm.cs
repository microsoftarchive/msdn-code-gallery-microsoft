/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:	    CSRichTextBoxSyntaxHighlighting 
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
using System.Drawing;
using System.Windows.Forms;

namespace CSRichTextBoxSyntaxHighlighting 
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();

            // Initialize the XMLViewerSettings.
            XMLViewerSettings viewerSetting = new XMLViewerSettings
            {
                AttributeKey = Color.Red,
                AttributeValue = Color.Blue,
                Tag = Color.Blue,
                Element = Color.DarkRed,
                Value = Color.Black,
            };

            viewer.Settings = viewerSetting;

        }

        /// <summary>
        /// Handle the Click event of the button "btnProcess".
        /// </summary>
        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                viewer.Process(true);
            }
            catch (ApplicationException appException)
            {
                MessageBox.Show(appException.Message, "ApplicationException");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }

        }

    }
}
