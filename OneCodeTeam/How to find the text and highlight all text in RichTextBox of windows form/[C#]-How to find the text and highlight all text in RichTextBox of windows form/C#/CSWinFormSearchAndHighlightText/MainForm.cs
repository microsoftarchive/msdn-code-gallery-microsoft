/****************************** Module Header ******************************\
* Module Name: MainForm.cs
* Project:     CSWinFormSearchAndHighlightText
* Copyright(c) Microsoft Corporation.
* 
* The class is the main form.
* The users can manipulate the form to find the searched text and highlight them
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System;
using System.Windows.Forms;

namespace CSWinFormSearchAndHighlightText
{
    /// <summary>
    ///  Main Form 
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            // Initialize the state of SearchAndHighlight button
            this.btnSearchAndHighlight.Enabled = false;
        }

        /// <summary>
        ///  Select Highlight color
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panelColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = this.panelColor.BackColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    this.panelColor.BackColor = colorDialog.Color;
                }
            }
        }

        /// <summary>
        ///  Search the text and Highlight them in RichTextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchAndHighlight_Click(object sender, EventArgs e)
        {
            bool isexist = rtbSource.Highlight(cboSearch.Text, panelColor.BackColor, chkMatchCase.Checked, chkMatchWholeWord.Checked);

            if (!isexist)
            {
                string format = string.Format("Can't find the \"{0}\" in RichTextBox control", cboSearch.Text);
                MessageBox.Show(format);
            }
            if (!cboSearch.Items.Contains(cboSearch.Text))
            {
                this.cboSearch.Items.Add(this.cboSearch.Text);
            }
        }

        /// <summary>
        ///  The Event for Text change in ComboBox Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboSearch_TextChanged(object sender, EventArgs e)
        {
            // Disable the SearchAndHightlight button
            if (cboSearch.Text.Length == 0)
            {
                this.btnSearchAndHighlight.Enabled = false;
            }
            else
            {
                // Enable the SearchAndHightlight button
                this.btnSearchAndHighlight.Enabled = true;
            }
        }

        /// <summary>
        ///  Key Press event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                btnSearchAndHighlight.PerformClick();
            }
        }
    }
}
