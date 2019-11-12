/****************************** Module Header ******************************\
Module Name:  Form1.cs
Project:      CSOfficeDocumentFileExtrator
Copyright (c) Microsoft Corporation.

The MainForm.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Windows.Forms;

using System.IO.Packaging;
using System.IO;

namespace CSOfficeDocumentFileExtractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string embeddingPartString; 

        /// <summary>
        /// This method extracts the files to the folder mentioned in the Destination Folder text box.
        /// If the extracted file is a structured storage, it will be sent to Ole10Native.ExtractFile() method 
        /// to extract the actual contents.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExtractSelectedFiles_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSourceFile.Text) || string.IsNullOrWhiteSpace(txtDestinationFolder.Text))
            {
                MessageBox.Show("The source file and destination folder cannot be empty");
                return;
            }
            if (!File.Exists(txtSourceFile.Text))
            {
                MessageBox.Show("The file does not exist");
                return;
            }
            // Open the package and loop through parts 
            // Check if the part uri to find if it contains the selected items in checked list box
            Package pkg = Package.Open(txtSourceFile.Text);
            foreach (PackagePart pkgPart in pkg.GetParts())
            {
                for (int i = 0; i < chkdLstEmbeddedFiles.CheckedItems.Count; i++)
                {
                    object chkditem = chkdLstEmbeddedFiles.CheckedItems[i];
                
                    if (pkgPart.Uri.ToString().Contains(embeddingPartString + chkdLstEmbeddedFiles.GetItemText(chkditem)))
                    {
                        // Get the file name
                        string fileName1 = pkgPart.Uri.ToString().Remove(0, embeddingPartString.Length);

                        // Get the stream from the part
                        System.IO.Stream partStream = pkgPart.GetStream();
                        string filePath = txtDestinationFolder.Text + "\\" + fileName1;

                        // Write the steam to the file.
                        System.IO.FileStream writeStream = new System.IO.FileStream(filePath, FileMode.Create, FileAccess.Write);
                        ReadWriteStream(pkgPart.GetStream(), writeStream);

                        // If the file is a structured storage file stored as a oleObjectXX.bin file
                        // Use Ole10Native class to extract the contents inside it.
                        if (fileName1.Contains("oleObject"))
                        {
                            // The Ole10Native class is defined in Ole10Native.cs file
                            Ole10Native.ExtractFile(filePath, txtDestinationFolder.Text);
                        }
                    }
                }
                
            }
            pkg.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Office Files | *.docx;*.docm;*.dotx;*.dotm;*.xlsx;*.xlsm;*.xltx;*.xltm;*.pptx;*.pptm";
            ofd.RestoreDirectory = true;
            DialogResult dr = ofd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                txtSourceFile.Text = ofd.FileName;
            }
        }
        /// <summary>
        /// This event scans through the file to check if there is any files embedded in it.
        /// If there is any, it will add the name of the file in the checked list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScan_Click(object sender, EventArgs e)
        {
            string fileName = txtSourceFile.Text; 
            if (txtSourceFile.Text == string.Empty || !System.IO.File.Exists(fileName))
            {
                MessageBox.Show("File does not exist.", "Invalid file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the package file
            Package pkg = Package.Open(fileName);

            System.IO.FileInfo fi = new System.IO.FileInfo(fileName);
            
            string extension = fi.Extension.ToLower();
           
            if ((extension == ".docx") || (extension == ".dotx") || (extension == ".docm") || (extension == ".dotm"))
            {
                embeddingPartString = "/word/embeddings/";
            }
            else if ((extension == ".xlsx") || (extension == ".xlsm") || (extension == ".xltx") || (extension == ".xltm"))
            {
                embeddingPartString = "/excel/embeddings/";
            }
            else
            {
                embeddingPartString = "/ppt/embeddings/";
            }

            // Get the embedded files names.
            foreach(PackagePart pkgPart in pkg.GetParts())
            {
                if (pkgPart.Uri.ToString().StartsWith(embeddingPartString))
                {
                    string fileName1 = pkgPart.Uri.ToString().Remove(0, embeddingPartString.Length);
                    chkdLstEmbeddedFiles.Items.Add(fileName1);
                }
            }
            pkg.Close();
            if (chkdLstEmbeddedFiles.Items.Count == 0)
                MessageBox.Show("The file does not contain any embedded files.");
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the folder to save the extracted files";
            DialogResult dr = fbd.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                txtDestinationFolder.Text = fbd.SelectedPath;
            }
        }

        /// <summary>
        /// ReadWriteStream method is used to extract the files from the document to a temporary location
        /// If the extracted file is a structured storage, it will be sent to the ExtractFile method to extract the actual content
        /// </summary>
        /// <param name="readStream"></param>
        /// <param name="writeStream"></param>
        private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        } 

        
                
    }
}
