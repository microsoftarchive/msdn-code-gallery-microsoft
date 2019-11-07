/******************************** Module Header ********************************\
* Module Name:  Solution1.cs
* Project:      CSAutomateWord
* Copyright (c) Microsoft Corporation.
* 
* Solution1.AutomateWord demonstrates automating Microsoft Word application by 
* using Microsoft Word Primary Interop Assembly (PIA) and explicitly assigning 
* each COM accessor object to a new variable that you would explicitly call 
* Marshal.FinalReleaseComObject to release it at the end. When you use this 
* solution, it is important to avoid calls that tunnel into the object model 
* because they will orphan Runtime Callable Wrapper (RCW) on the heap that you 
* will not be able to access in order to call Marshal.ReleaseComObject. You need 
* to be very careful. For example, 
* 
*   Word.Document oDoc = oWord.Documents.Add(ref missing, ref missing,
*     ref missing, ref missing);
*  
* Calling oWord.Documents.Add creates an RCW for the Documents object. If you 
* invoke these accessors via tunneling as this code does, the RCW for 
* Documents is created on the GC heap, but the reference is created under the 
* hood on the stack and are then discarded. As such, there is no way to call 
* MarshalFinalReleaseComObject on this RCW. To get such kind of RCWs released, 
* you would either need to force a garbage collection as soon as the calling 
* function is off the stack (see Solution2.AutomateWord), or you would need to 
* explicitly assign each accessor object to a variable and free it.
* 
*   Word.Documents oDocs = oWord.Documents;
*   Word.Document oDoc = oDocs.Add(ref missing, ref missing, ref missing, 
*     ref missing);
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
using System.IO;
using System.Text;
using System.Reflection;

using Word = Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
#endregion


namespace CSAutomateWord
{
    static class Solution1
    {
        public static void AutomateWord()
        {
            object missing = Type.Missing;
            object notTrue = false;

            Word.Application oWord = null;
            Word.Documents oDocs = null;
            Word.Document oDoc = null;
            Word.Paragraphs oParas = null;
            Word.Paragraph oPara = null;
            Word.Range oParaRng = null;
            Word.Font oFont = null;

            try
            {
                // Create an instance of Microsoft Word and make it invisible.

                oWord = new Word.Application();
                oWord.Visible = false;
                Console.WriteLine("Word.Application is started");

                // Create a new Document and add it to document collection.               
                oDoc = oWord.Documents.Add(ref missing, ref missing, ref missing, ref missing);
                Console.WriteLine("A new document is created");

                // Insert a paragraph.

                Console.WriteLine("Insert a paragraph");

                oParas = oDoc.Paragraphs;
                oPara = oParas.Add(ref missing);
                oParaRng = oPara.Range;
                oParaRng.Text = "Heading 1";
                oFont = oParaRng.Font;
                oFont.Bold = 1;
                oParaRng.InsertParagraphAfter();

                // Save the document as a docx file and close it.

                Console.WriteLine("Save and close the document");

                object fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample1.docx";
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
                Console.WriteLine("Solution1.AutomateWord throws the error: {0}",
                       ex.Message);
            }
            finally
            {
                // Clean up the unmanaged Word COM resources by explicitly 
                // calling Marshal.FinalReleaseComObject on all accessor objects. 
                // See http://support.microsoft.com/kb/317109.

                if (oFont != null)
                {
                    Marshal.FinalReleaseComObject(oFont);
                    oFont = null;
                }
                if (oParaRng != null)
                {
                    Marshal.FinalReleaseComObject(oParaRng);
                    oParaRng = null;
                }
                if (oPara != null)
                {
                    Marshal.FinalReleaseComObject(oPara);
                    oPara = null;
                }
                if (oParas != null)
                {
                    Marshal.FinalReleaseComObject(oParas);
                    oParas = null;
                }
                if (oDoc != null)
                {
                    Marshal.FinalReleaseComObject(oDoc);
                    oDoc = null;
                }
                if (oDocs != null)
                {
                    Marshal.FinalReleaseComObject(oDocs);
                    oDocs = null;
                }
                if (oWord != null)
                {
                    Marshal.FinalReleaseComObject(oWord);
                    oWord = null;
                }
            }
        }
    }
}
