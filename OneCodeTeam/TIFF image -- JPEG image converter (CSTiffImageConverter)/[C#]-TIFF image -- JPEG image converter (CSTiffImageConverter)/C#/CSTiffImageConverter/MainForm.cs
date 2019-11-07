/****************************** Module Header ******************************\ 
Module Name:    MainForm.cs 
Project:        CSTiffImageConverter
Copyright (c) Microsoft Corporation. 

This sample demonstrates how to convert JPEG images into TIFF images and vice 
versa. This sample also allows to create single multipage TIFF images from 
selected JPEG images.

TIFF (originally standing for Tagged Image File Format) is a flexible, 
adaptable file format for handling images and data within a single file, 
by including the header tags (size, definition, image-data arrangement, 
applied image compression) defining the image's geometry. For example, a 
TIFF file can be a container holding compressed (lossy) JPEG and (lossless) 
PackBits compressed images. A TIFF file also can include a vector-based 
clipping path (outlines, croppings, image frames). 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace CSTiffImageConverter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnConvertToTiff_Click(object sender, EventArgs e)
        {
            dlgOpenFileDialog.Multiselect = true;
            dlgOpenFileDialog.Filter = "Image files (.jpg, .jpeg)|*.jpg;*.jpeg";

            if (dlgOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    TiffImageConverter.ConvertJpegToTiff(dlgOpenFileDialog.FileNames, chkIsMultipage.Checked);
                    MessageBox.Show("Image conversion completed.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error");
                }
            }
        }

        private void btnConvertToJpeg_Click(object sender, EventArgs e)
        {
            dlgOpenFileDialog.Multiselect = false;
            dlgOpenFileDialog.Filter = "Image files (.tif, .tiff)|*.tif;*.tiff";

            if (dlgOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    TiffImageConverter.ConvertTiffToJpeg(dlgOpenFileDialog.FileName);
                    MessageBox.Show("Image conversion completed.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error");
                }
            }
        }
    }
}
