/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:      CSOpenXmlCreateTable
* Copyright(c)  Microsoft Corporation.
* 
* This is the main form of this application. 
* It is used to initialize the UI and handle the events.
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
using System.IO;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;

namespace CSOpenXmlCreateTable
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.btnInserTable.Enabled = false;
        }

        /// <summary>
        /// Handle the Click event of Open button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            SelectPowerPointFile();

            // After select an existing PowerPoint file, make "Inser Table" button to be enable
            this.btnInserTable.Enabled = true;
        }

        /// <summary>
        /// Show an OpenFileDialog to select a Word document.
        /// </summary>
        /// <returns>
        /// The file name.
        /// </returns>
        private string SelectPowerPointFile()
        {
            string fileName = null;
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "PowerPoint document (*.pptx)|*.pptx";
                dialog.InitialDirectory = Environment.CurrentDirectory;

                // Retore the directory before closing
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = dialog.FileName;
                    this.txbSource.Text = dialog.FileName;
                }
            }

            return fileName;
        }

        private void btnInserTable_Click(object sender, EventArgs e)
        {
            string filePath=txbSource.Text.Trim();
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                MessageBox.Show("The File is invalid,Please select an existing file again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Open the source document as read/write
                // In the Open XML SDK, the PresentationDocument class represents a presentation document package.
                // To work with a presentation document, first create an instance of the Presentation Class
                // and then work with the instance.
                // To create the class instance from the document call the Open(string, boolean) method.
                using (PresentationDocument presentationDocument = PresentationDocument.Open(filePath, true))
                {
                    // Start create table with rows in powerPoint document
                    // If create successfully, we can see a table in the last slide of the powerpoint
                    InsertTableToPowerPoint.CreateTableInLastSlide(presentationDocument);
                }

                MessageBox.Show("Insert Table successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
