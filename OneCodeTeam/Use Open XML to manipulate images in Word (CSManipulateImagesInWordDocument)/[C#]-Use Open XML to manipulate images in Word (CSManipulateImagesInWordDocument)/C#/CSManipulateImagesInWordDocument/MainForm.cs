/****************************** Module Header ******************************\
 Module Name:  MainForm.cs
 Project:      CSManipulateImagesInWordDocument
 Copyright (c) Microsoft Corporation.
 
 The Main UI of the application. 
 
 
 This source is subject to the Microsoft Public License.
 See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 All other rights reserved.
 
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Drawing;

namespace CSManipulateImagesInWordDocument
{
    public partial class MainForm : Form
    {
        WordDocumentImageManipulator documentManipulator;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handle the btnOpenFile click event.
        /// </summary>
        private void btnOpenFile_Click(object sender, EventArgs e)
        {

            // Open an OpenFileDialog instance.
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Word document (*.docx)|*.docx";

                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        lstImage.Items.Clear();
                        if (picView.Image != null)
                        {
                            picView.Image.Dispose();
                        }
                        picView.Image = null;
                        lbFileName.Text = string.Empty;


                        // Initialize a WordDocumentImageManipulator instance.
                        OpenWordDocument(dialog.FileName);

                        // Update the lstImage listbox.
                        UpdateImageList();

                        lbFileName.Text = dialog.FileName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Initialize a WordDocumentImageManipulator instance.
        /// </summary>
        void OpenWordDocument(string filepath)
        {
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
            {
                throw new ArgumentException("filepath");
            }

            FileInfo file = new FileInfo(filepath);

            // Dispose the previous instance.
            if (documentManipulator != null)
            {
                documentManipulator.Dispose();
            }

            documentManipulator = new WordDocumentImageManipulator(file);

            // Register the ImagesChanged event.
            documentManipulator.ImagesChanged += new EventHandler(documentManipulator_ImagesChanged);

        }

        /// <summary>
        /// Update the lstImage listbox.
        /// </summary>
        void UpdateImageList()
        {
            if (picView.Image != null)
            {
                picView.Image.Dispose();
            }
            picView.Image = null;

            lstImage.Items.Clear();

            // Display the "Embed" property of the Blip element. This property is the 
            // reference ID of the ImagePart.
            lstImage.DisplayMember = "Embed";
            foreach (var blip in documentManipulator.GetAllImages())
            {
                lstImage.Items.Add(blip);
            }
        }

        /// <summary>
        /// Handle the ImagesChanged event.
        /// </summary>
        void documentManipulator_ImagesChanged(object sender, EventArgs e)
        {
            UpdateImageList();
        }

        /// <summary>
        /// Handle the lstImage SelectedIndexChanged event to display the image in 
        /// picView.
        /// </summary>
        private void lstImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            var imgBlip = lstImage.SelectedItem as Blip;
            if (imgBlip == null)
            {
                return;
            }

            // Dispose the previous image in the picView.
            if (picView.Image != null)
            {
                picView.Image.Dispose();
                picView.Image = null;
            }

            try
            {
                var newImg = documentManipulator.GetImageInBlip(imgBlip);
                picView.Image = new Bitmap(newImg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Handle the btnDelete Click SelectedIndexChanged event.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstImage.SelectedItem != null)
            {
                var result = MessageBox.Show(
                    "Do you want to delete this image?",
                    "Delete Image",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        documentManipulator.DeleteImage(lstImage.SelectedItem as Blip);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Handle the btnReplace Click SelectedIndexChanged event.
        /// </summary>
        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (lstImage.SelectedItem != null)
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Filter = "Image File (*.jpeg;*.jpg;*.png)|*.jpeg;*.jpg;*.png";

                    var result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        var confirm = MessageBox.Show(
                            "Do you want to replace this image?",
                            "Replace Image",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        if (confirm == System.Windows.Forms.DialogResult.Yes)
                        {
                            try
                            {
                                documentManipulator.ReplaceImage(
                                    lstImage.SelectedItem as Blip,
                                    new FileInfo(dialog.FileName));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Handle the btnExport Click SelectedIndexChanged event.
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (lstImage.SelectedItem != null && picView.Image != null)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = "Image File (*.jpeg)|*.jpeg";
                    var result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        picView.Image.Save(dialog.FileName, ImageFormat.Jpeg);
                    }
                }
            }
        }
    }
}
