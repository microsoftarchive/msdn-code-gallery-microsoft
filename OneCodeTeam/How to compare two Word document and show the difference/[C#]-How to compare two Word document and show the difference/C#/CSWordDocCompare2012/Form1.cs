/***************************** Module Header ******************************\
* Module Name:	Form1.cs
* Project:		CSWordDocCompare2012
* Copyright (c) Microsoft Corporation.
* 
* This code sample shows how to compare two word documents and show the difference.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/
using System;
using System.Windows.Forms;
using MsWord = Microsoft.Office.Interop.Word;

namespace CSWordDocCompare2012
{
    public partial class Form1 : Form
    {
        #region Declare variables

        MsWord.Application wordApp = null;
        object readOnly = null;
        object missing = null;

        MsWord.Document doc1 = null;
        MsWord.Document doc2 = null;
        MsWord.Document doc = null;

        private string documentFilter = "Microsoft Word Document (.doc)|.docx|All files (*.*)|*.*";

        #endregion

        public Form1()
        {
            InitializeComponent();

            // Initialize the private variables
            wordApp = new MsWord.Application();
            readOnly = true;
            missing = System.Reflection.Missing.Value;   
        }

        // Open first word document
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = documentFilter;
            DialogResult result1 = openFileDialog1.ShowDialog();
            if (result1 == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
                textBox1.ReadOnly = true;
                doc1 = wordApp.Documents.Open(textBox1.Text, missing, readOnly, missing, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing);
            }
        }

        // Open second word document
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = documentFilter;
            DialogResult result2 = openFileDialog2.ShowDialog();
            if (result2 == DialogResult.OK)
            {
                textBox2.Text = openFileDialog2.FileName;
                textBox2.ReadOnly = true;
                doc2 = wordApp.Documents.Open(textBox2.Text, missing, readOnly, missing, missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing, missing, missing);
            }
        }

        // Compare the two word documents
        private void button3_Click(object sender, EventArgs e)
        {
            doc = wordApp.CompareDocuments(doc1, doc2, MsWord.WdCompareDestination.wdCompareDestinationNew,
                    MsWord.WdGranularity.wdGranularityWordLevel, true, true, true, true, true, true, true, true, true, true, "", false);

            // Close first document
            doc1.Close(missing, missing, missing);

            // Close second document
            doc2.Close(missing, missing, missing);

            // Show the compared document
            wordApp.Visible = true;

            // Quit the application
            Close();
        }
    }
}
