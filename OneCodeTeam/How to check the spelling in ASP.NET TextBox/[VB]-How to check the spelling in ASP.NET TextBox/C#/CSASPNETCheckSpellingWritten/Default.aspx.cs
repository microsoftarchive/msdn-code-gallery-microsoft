/****************************** Module Header ******************************\
* Module Name: Default.aspx.cs
* Project:     CSASPNETCheckSpellingWritten
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to check whether the spelling written in TextBox
* is correct or not. This sample code checks the user's input words
* via MS Word CheckSpelling component.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Reflection;
using Microsoft.Office.Interop.Word;

namespace CSASPNETCheckSpellingWritten
{
    public partial class Default : System.Web.UI.Page
    {
        // Define MS Word application.
        public Microsoft.Office.Interop.Word.Application applicationWord;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                tbInput.Text = "  This article describes a All-In-One framewok sample that demonstrates a step-by-step guide " +
                "illustrating how to strip and parsse the Html code. You can download the sample packge from the donload icons below.";
            }
        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            // Prevent multiple checker windows.
            if (applicationWord != null)
            {
                return;
            }

            applicationWord = new Microsoft.Office.Interop.Word.Application();
            int errors = 0;
            if (tbInput.Text.Length > 0)
            {
                object template = Missing.Value;
                object newTemplate = Missing.Value;
                object documentType = Missing.Value;
                object visible = true;

                // Define a MS Word Document, then we use this document to calculate errors number and
                // invoke document's CheckSpelling method.
                Microsoft.Office.Interop.Word._Document documentCheck = applicationWord.Documents.Add(ref template,
                    ref newTemplate, ref documentType, ref visible);
                applicationWord.Visible = false;
                documentCheck.Words.First.InsertBefore(tbInput.Text);
                Microsoft.Office.Interop.Word.ProofreadingErrors spellErrorsColl = documentCheck.SpellingErrors;
                errors = spellErrorsColl.Count;

                object optional = Missing.Value;
                documentCheck.Activate();
                documentCheck.CheckSpelling(ref optional, ref optional, ref optional, ref optional, ref optional, ref optional,
                    ref optional, ref optional, ref optional, ref optional, ref optional, ref optional);
                documentCheck.LanguageDetected = true;


                // When users close the dialog, the error message will be displayed.
                if (errors == 0)
                {
                    lbMessage.Text = "No errors";
                }
                else
                {
                    lbMessage.Text = "Total errors num:" + errors;
                }

                // Replace misspelled words of TextBox.
                object first = 0;
                object last = documentCheck.Characters.Count - 1;
                tbInput.Text = documentCheck.Range(ref first, ref last).Text;
            }

            object saveChanges = false;
            object originalFormat = Missing.Value;
            object routeDocument = Missing.Value;
            ((_Application)applicationWord).Quit(ref saveChanges, ref originalFormat, ref routeDocument);
            applicationWord = null;
        }
    }
}