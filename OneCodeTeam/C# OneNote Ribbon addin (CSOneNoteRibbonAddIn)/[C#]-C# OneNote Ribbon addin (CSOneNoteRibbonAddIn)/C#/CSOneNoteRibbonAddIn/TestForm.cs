/****************************** Module Header ******************************\
Module Name:  TestForm.cs
Project:      CSOneNoteRibbonAddIn
Copyright (c) Microsoft Corporation.

The Window Form for testing to open

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Imports directives
using System;
using System.Windows.Forms;
using OneNote = Microsoft.Office.Interop.OneNote;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.InteropServices;
#endregion

namespace CSOneNoteRibbonAddIn
{
    [ComVisible(false)] 
    public partial class TestForm : Form
    {
        private OneNote.Application oneNoteApp = null;
        public TestForm(OneNote.Application oneNote)
        {
            oneNoteApp = oneNote;
            InitializeComponent();
        }

        private void btnClick_Click(object sender, EventArgs e)
        {
            MessageBox.Show(GetPageTitle());
        }

        /// <summary>
        /// Get the title of the page
        /// </summary>
        /// <returns>string</returns>
        private string GetPageTitle()
        {
            string pageXmlOut = GetActivePageContent();        
            var doc = XDocument.Parse(pageXmlOut);
            string pageTitle = "";
            pageTitle = doc.Descendants().FirstOrDefault().Attribute("ID").NextAttribute.Value;

            return pageTitle;
        }

        /// <summary>
        /// Get active page content and output the xml string
        /// </summary>
        /// <returns>string</returns>
        private string GetActivePageContent()
        {
            string activeObjectID = this.GetActiveObjectID(_ObjectType.Page);
            string pageXmlOut = "";
            oneNoteApp.GetPageContent(activeObjectID,out pageXmlOut);

            return pageXmlOut;
        }

        /// <summary>
        /// Get ID of current page 
        /// </summary>
        /// <param name="obj">_Object Type</param>
        /// <returns>current page Id</returns>
        private string GetActiveObjectID(_ObjectType obj)
        {
            string currentPageId = "";
            uint count = oneNoteApp.Windows.Count;
            foreach (OneNote.Window window in oneNoteApp.Windows)
            {
                if (window.Active)
                {
                    switch (obj)
                    {
                        case _ObjectType.Notebook:
                            currentPageId = window.CurrentNotebookId;
                            break; 
                        case _ObjectType.Section:
                            currentPageId = window.CurrentSectionId;
                            break; 
                        case _ObjectType.SectionGroup:
                            currentPageId = window.CurrentSectionGroupId;
                            break; 
                    }

                    currentPageId = window.CurrentPageId;
                }
            }

            return currentPageId;

        }

        /// <summary>
        /// Nested types
        /// </summary>
        private enum _ObjectType
        {
            Notebook,
            Section,
            SectionGroup,
            Page,
            SelectedPages,
            PageObject
        }
    }
}
