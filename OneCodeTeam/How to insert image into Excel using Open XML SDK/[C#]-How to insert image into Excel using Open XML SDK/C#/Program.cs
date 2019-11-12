//****************************** Module Header ******************************\
//Module Name:    Program.cs
//Project:        InsertImageIntoExcel
//Copyright (c) Microsoft Corporation

//The project illustrates how to insert image into Excel using Open XML SDK

//This source is subject to the Microsoft Public License.
//See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
//All other rights reserved.

//*****************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;

namespace InsertImageIntoExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string sFile = appPath + "\\InsertImage.xlsx";

                string imageFile = appPath + "\\SampleImage.jpg";

                // If the file exists, delete it
                if (File.Exists(sFile))
                {
                    File.Delete(sFile);
                }

                Utility.CreatePackage(sFile, imageFile);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}