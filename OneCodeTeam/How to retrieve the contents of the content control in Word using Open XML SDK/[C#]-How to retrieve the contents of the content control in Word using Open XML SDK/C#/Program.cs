//****************************** Module Header ******************************\
//Module Name:  Program.cs
//Project:      ReadContentCtrlFromWordDoc
//Copyright (c) Microsoft Corporation

//The project illustrates how to retrieve the contents of the content control using Open XML SDK

//This source is subject to the Microsoft Public License.
//See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
//All other rights reserved.

//*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ReadContentCtrlFromWordDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string containingDocumentPath = appPath + "\\ContainingDocument.docx";

                WordprocessingDocument document = WordprocessingDocument.Open(containingDocumentPath, true);

                MainDocumentPart mainDocumentPart = document.MainDocumentPart;

                // locate content control collection
                var sdtblocks = mainDocumentPart.Document.Descendants<SdtBlock>();
                string contentCtrlData = string.Empty;

                // Iterate through the content control blocks and append data to a string object
                foreach (var sdtb in sdtblocks)
                {
                    contentCtrlData += sdtb.SdtContentBlock.InnerText;
                }

                // Display the content control data to the user
                Console.WriteLine(contentCtrlData);

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
