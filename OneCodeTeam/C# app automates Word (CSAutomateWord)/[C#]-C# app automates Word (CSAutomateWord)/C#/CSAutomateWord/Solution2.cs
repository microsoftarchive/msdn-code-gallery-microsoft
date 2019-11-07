/******************************** Module Header ********************************\
* Module Name:  Solution2.cs
* Project:      CSAutomateWord
* Copyright (c) Microsoft Corporation.
* 
* Solution2.AutomateWord demonstrates automating Microsoft Word application by 
* using Microsoft Word Primary Interop Assembly (PIA) and forcing a garbage 
* collection as soon as the automation function is off the stack (at which point 
* the Runtime Callable Wrapper (RCW) objects are no longer rooted) to clean up 
* RCWs and release COM objects.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*******************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

using Word = Microsoft.Office.Interop.Word;
#endregion


namespace CSAutomateWord
{
    static class Solution2
    {
        public static void AutomateWord()
        {
            AutomateWordImpl();


            // Clean up the unmanaged Word COM resources by forcing a garbage 
            // collection as soon as the calling function is off the stack (at 
            // which point these objects are no longer rooted).

            GC.Collect();
            GC.WaitForPendingFinalizers();
            // GC needs to be called twice in order to get the Finalizers called 
            // - the first time in, it simply makes a list of what is to be 
            // finalized, the second time in, it actually is finalizing. Only 
            // then will the object do its automatic ReleaseComObject.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void AutomateWordImpl()
        {
            object missing = Type.Missing;
            object oEndOfDoc = @"\endofdoc";    // A predefined bookmark 
            object notTrue = false;

            try
            {
                // Create an instance of Microsoft Excel and make it invisible.
                Word.Application oWord = new Word.Application();
                oWord.Visible = false;
                Console.WriteLine("Word.Application is started");

                // Create a new Document and add it to document collection.
                Word.Document oDoc = oWord.Documents.Add(ref missing, ref missing,
                    ref missing, ref missing);

                // Insert a paragraph.
                Console.WriteLine("Insert a paragraph");

                Word.Paragraph oPara = oDoc.Paragraphs.Add(ref missing);
                oPara.Range.Text = "Heading 1";
                oPara.Range.Font.Bold = 1;
                oPara.Range.InsertParagraphAfter();

                // Insert a table.
                Console.WriteLine("Insert a table");

                Word.Range oBookmarkRng = oDoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                Word.Table oTable = oDoc.Tables.Add(oBookmarkRng, 5, 2,
                    ref missing, ref missing);
                oTable.Range.ParagraphFormat.SpaceAfter = 6;
                for (int r = 1; r <= 5; r++)
                {
                    for (int c = 1; c <= 2; c++)
                    {
                        oTable.Cell(r, c).Range.Text = "r" + r + "c" + c;
                    }
                }

                // Change width of columns 1 & 2
                oTable.Columns[1].Width = oWord.InchesToPoints(2);
                oTable.Columns[2].Width = oWord.InchesToPoints(3);

                // Save the document as a docx file and close it.
                Console.WriteLine("Save and close the document");

                object fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample2.docx";
                object fileFormat = Word.WdSaveFormat.wdFormatXMLDocument;

                // Saves the document with a new name or format. 
                // Some of the arguments for this method correspond to 
                // the options in the Save As dialog box. 
                // For details,please refer to
                // :http://msdn.microsoft.com/en-us/library/microsoft.office.tools.word.document.saveas(VS.80).aspx
                oDoc.SaveAs(ref fileName, ref fileFormat, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing);
                ((Word._Document)oDoc).Close(ref missing, ref missing,
                    ref missing);

                // Quit the Word application.
                Console.WriteLine("Quit the Word application");
                ((Word._Application)oWord).Quit(ref notTrue, ref missing,
                    ref missing);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Solution2.AutomateWord throws the error: {0}",
                       ex.Message);
            }
        }
    }
}