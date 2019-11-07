/****************************** Module Header ******************************\
* Module Name:  MainForm.cs
* Project:      CSOpenXmlExcelToXml
* Copyright(c)  Microsoft Corporation.
* 
* This is Main Form.
* Uers can manipulate the form to convert excel docuemnt to XML format string. 
* Then Show the xml in TextBox control
* Users also can save the xml string as xml file on client 
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

namespace CSOpenXmlExcelToXml
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.btnSaveAs.Enabled = false;
        }

        /// <summary>
        ///  Open an dialog to let users select Excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            // Initializes a OpenFileDialog instance 
            using (OpenFileDialog openfileDialog = new OpenFileDialog())
            {
                openfileDialog.RestoreDirectory = true;
                openfileDialog.Filter = "Excel files(*.xlsx;*.xls)|*.xlsx;*.xls";

                if (openfileDialog.ShowDialog() == DialogResult.OK)
                {
                    tbExcelName.Text = openfileDialog.FileName;
                }
            }
        }

        /// <summary>
        ///  Convert Excel file to Xml format and view in Listbox control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConvert_Click(object sender, EventArgs e)
        {
            tbXmlView.Clear();
            string excelfileName = tbExcelName.Text;

            if (string.IsNullOrEmpty(excelfileName) || !File.Exists(excelfileName))
            {
                MessageBox.Show("The Excel file is invalid! Please select a valid file.");
                return;
            }

            try
            {
                string xmlFormatstring = new ConvertExcelToXml().GetXML(excelfileName);
                if (string.IsNullOrEmpty(xmlFormatstring))
                {
                    MessageBox.Show("The content of Excel file is Empty!");
                    return;
                }

                tbXmlView.Text = xmlFormatstring;

                // If txbXmlView has text, set btnSaveAs button to be enable
                btnSaveAs.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurs! The error message is: " +ex.Message);
            }
        }

        /// <summary>
        ///  Save the XMl format string as Xml file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            // Initializes a SaveFileDialog instance 
            using (SaveFileDialog savefiledialog = new SaveFileDialog())
            {
                savefiledialog.RestoreDirectory = true;
                savefiledialog.DefaultExt = "xml";
                savefiledialog.Filter = "All Files(*.xml)|*.xml";
                if (savefiledialog.ShowDialog() == DialogResult.OK)
                {
                    Stream filestream = savefiledialog.OpenFile();
                    StreamWriter streamwriter = new StreamWriter(filestream);
                    streamwriter.Write("<?xml version='1.0'?>" +
                        Environment.NewLine + tbXmlView.Text);
                    streamwriter.Close();
                }
            }
        }
    }
}
