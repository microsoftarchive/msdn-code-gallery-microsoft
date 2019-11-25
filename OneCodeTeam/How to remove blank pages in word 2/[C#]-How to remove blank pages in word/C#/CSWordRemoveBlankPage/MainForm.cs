﻿/****************************** Module Header ******************************\ 
* Module Name:   MainForm.cs
* Project:       CSWordRemoveBlankPage
* Copyright (c)  Microsoft Corporation. 
*  
* The Class is Main Form
* Customers can manipulate the form to remove blank page of word document
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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace CSWordRemoveBlankPage
{
    public partial class MainForm : Form
    {
        //the path of word document
        string wordPath = null; 
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open word document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenWord_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog openfileDialog=new OpenFileDialog())
            {
                openfileDialog.Filter="Word document(*.doc,*.docx)|*.doc;*.docx";
                if (openfileDialog.ShowDialog() == DialogResult.OK)
                {
                    txbWordPath.Text = openfileDialog.FileName;
                    wordPath = openfileDialog.FileName;
                }
            }
        }

        /// <summary>
        /// Click event of Remove Blank Page button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txbWordPath.Text))
            {
                MessageBox.Show("Please Select valid path of word document!","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            // Remove blank Page in word document
            if (RemoveBlankPage()==true)
            {
                MessageBox.Show("Remove blank page successfully!");
            }
        }

        /// <summary>
        /// Remove Blank Page in Word document
        /// </summary>
        private bool RemoveBlankPage()
        {
            Word.Application wordapp = null;
            Word.Document doc = null;
            Word.Paragraphs paragraphs=null;
            try
            {
                // Start Word APllication and set it be invisible
                wordapp = new Word.Application();
                wordapp.Visible = false;
                doc = wordapp.Documents.Open(wordPath);
                paragraphs = doc.Paragraphs;
                foreach (Word.Paragraph paragraph in paragraphs)
                {
                    if (paragraph.Range.Text.Trim() == string.Empty)
                    {
                        paragraph.Range.Select();
                        wordapp.Selection.Delete();
                    }
                }

                // Save the document and close document
                doc.Save();
                ((Word._Document)doc).Close();

                // Quit the word application
                ((Word._Application)wordapp).Quit();

            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception Occur, error message is: "+ex.Message);
                return false;
            }
            finally
            { 
                // Clean up the unmanaged Word COM resources by explicitly
                // call Marshal.FinalReleaseComObject on all accessor objects
                if (paragraphs != null)
                {
                    Marshal.FinalReleaseComObject(paragraphs);
                    paragraphs = null;
                }
                if (doc != null)
                {
                    Marshal.FinalReleaseComObject(doc);
                    doc = null;
                }
                if (wordapp != null)
                {
                    Marshal.FinalReleaseComObject(wordapp);
                    wordapp = null;
                }
            }

            return true;
        }
    }
}
