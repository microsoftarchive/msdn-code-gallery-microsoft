/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:      CSOpenXmlInsertImageToPPT
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
using DocumentFormat.OpenXml.Presentation;

namespace CSOpenXmlInsertImageToPPT
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handle client event of Insert button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsert_Click(object sender, EventArgs e)
        {
            string pptFilePath = txbPPtPath.Text.Trim();
            string imagefilePath = txbImagePath.Text.Trim();
            string imageExt =Path.GetExtension(imagefilePath);
            if (imageExt.Equals("jpg", StringComparison.OrdinalIgnoreCase))
            {
                imageExt = "image/jpeg";
            }
            else
            {
                imageExt = "image/png";
            }
            bool condition =string.IsNullOrEmpty(pptFilePath)
                ||!File.Exists(pptFilePath)
                ||string.IsNullOrEmpty(imagefilePath)
                ||!File.Exists(imagefilePath);
            if (condition)
            {
                MessageBox.Show("The PowerPoint or iamge file is invalid,Please select an existing file again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (PresentationDocument presentationDocument = PresentationDocument.Open(pptFilePath, true))
                {
                    // Get the presentation Part of the presentation document
                    PresentationPart presentationPart = presentationDocument.PresentationPart;
                    Slide slide = new InsertImage().InsertSlide(presentationPart, "Title Only");
                    new InsertImage().InsertImageInLastSlide(slide, imagefilePath, imageExt);
                    slide.Save();
                    presentationDocument.PresentationPart.Presentation.Save();
                    MessageBox.Show("Insert Image Successfully,you can check with opening PowerPoint");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handle Click events of Open button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenPPt_Click(object sender, EventArgs e)
        {
             using (OpenFileDialog dialog = new OpenFileDialog())
             {
                dialog.Filter = "PowerPoint document (*.pptx)|*.pptx";
                dialog.InitialDirectory = Environment.CurrentDirectory;

                // Retore the directory before closing
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.txbPPtPath.Text = dialog.FileName;
                }
             }
        }

        /// <summary>
        /// Handle Click events of Select button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectImg_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Pictures(*.jpg;*.png)|*.jpg;*.png";
                dialog.InitialDirectory = Environment.CurrentDirectory;

                // Retore the directory before closing
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.txbImagePath.Text = dialog.FileName;
                }
            }
        }
    }
}
