/****************************** Module Header ******************************\
* Module Name:  MainWindow.xaml.cs
* Project:               CSVSTOViewWordInWPF
* Copyright(c)      Microsoft Corporation.
* 
* This is the main form of this application. It is used to initialize the control 
* and handle the events
* Users must selected existing word document on local machine firstly.
* Second, Click "View  Word Doc" button to load word document into DocumentViewer control 
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
using System.Windows;
using System.Windows.Xps.Packaging;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using Word = Microsoft.Office.Interop.Word;

namespace CSVSTOViewWordInWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///  Select Word file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectWord_Click(object sender, RoutedEventArgs e)
        {
            // Initialize an OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter and RestoreDirectory
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "Word documents(*.doc;*.docx)|*.doc;*.docx";

            bool? result =openFileDialog.ShowDialog();
            if (result==true)
            {
                if (openFileDialog.FileName.Length > 0)
                {
                    txbSelectedWordFile.Text = openFileDialog.FileName;
                }
            }
        }

        /// <summary>
        ///  Convert the word document to xps document
        /// </summary>
        /// <param name="wordFilename">Word document Path</param>
        /// <param name="xpsFilename">Xps document Path</param>
        /// <returns></returns>
        private XpsDocument ConvertWordToXps(string wordFilename, string xpsFilename)
        {
            // Create a WordApplication and host word document
            Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            try
            {
                wordApp.Documents.Open(wordFilename);
                
                // To Invisible the word document
                wordApp.Application.Visible = false;

                // Minimize the opened word document
                wordApp.WindowState = WdWindowState.wdWindowStateMinimize;

                Document doc = wordApp.ActiveDocument;

                doc.SaveAs(xpsFilename, WdSaveFormat.wdFormatXPS);

                XpsDocument xpsDocument = new XpsDocument(xpsFilename, FileAccess.Read);
                return xpsDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurs, The error message is  " + ex.ToString());
                return null;
            }
            finally
            {
                wordApp.Documents.Close();
                ((_Application)wordApp).Quit(WdSaveOptions.wdDoNotSaveChanges);
            }
        }

        /// <summary>
        ///  View Word Document in WPF DocumentView Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnViewDoc_Click(object sender, RoutedEventArgs e)
        {
            string wordDocument =txbSelectedWordFile.Text;
            if (string.IsNullOrEmpty(wordDocument) || !File.Exists(wordDocument))
            {
                MessageBox.Show("The file is invalid. Please select an existing file again.");
            }
            else
            {
                string convertedXpsDoc = string.Concat(Path.GetTempPath(), "\\", Guid.NewGuid().ToString(), ".xps");
                XpsDocument xpsDocument =ConvertWordToXps(wordDocument, convertedXpsDoc);
                if (xpsDocument == null)
                {
                    return;
                }
   
                documentviewWord.Document = xpsDocument.GetFixedDocumentSequence();
            }
        }
    }
}
