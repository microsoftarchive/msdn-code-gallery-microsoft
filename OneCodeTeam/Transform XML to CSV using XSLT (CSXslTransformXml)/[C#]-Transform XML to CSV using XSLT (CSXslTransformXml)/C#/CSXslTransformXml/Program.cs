/****************************** Module Header ******************************\
* Module Name:  Program.cs
* Project:      CSXslTransformXml
* Copyright (c) Microsoft Corporation.
* 
* This sample project shows how to use XslCompiledTransform to transform an 
* XML data file to .csv file using an XSLT style sheet.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System.Xml.Xsl;
#endregion


namespace CSXslTransformXml
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize an XslCompiledTransform instance.
            XslCompiledTransform transform = new XslCompiledTransform();

            // Call transform.Load() to load the XSLT style sheet Books.xslt.
            transform.Load("Books.xslt");

            // Call transform.Transform() to transform the source XML file 
            // to a csv file using the XSLT style sheet.
            transform.Transform("Books.xml", "Books.csv");
        }
    }
}
