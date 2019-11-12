//****************************** Module Header ******************************\
//Module Name:    Program.cs
//Project:        EmbedExcelIntoWordDoc
//Copyright (c) Microsoft Corporation

// The project illustrates how to embed excel sheet into word document using using Open XML SDK

//This source is subject to the Microsoft Public License.
//See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
//All other rights reserved.

//*****************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbedExcelIntoWordDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Embed Excel sheet into Word Document by using Open XML SDK \n");
                Console.WriteLine("===========================================================\n");

                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string containingDocumentPath = appPath + "\\ContainingDocument.docx";
                string embeddedExcelPath = appPath + "\\ExbededExcel.xlsx";

                Utility.CreatePackage(containingDocumentPath, embeddedExcelPath);

                Console.WriteLine("Excel file '" + embeddedExcelPath + "' embeded into the word document '" + containingDocumentPath + "'");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
