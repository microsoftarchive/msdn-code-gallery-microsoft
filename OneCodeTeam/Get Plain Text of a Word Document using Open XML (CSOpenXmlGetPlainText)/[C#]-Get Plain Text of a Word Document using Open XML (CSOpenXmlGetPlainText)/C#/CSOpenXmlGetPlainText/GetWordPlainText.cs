/****************************** Module Header ******************************\
* Module Name:  GetWordPlainText.cs
* Project:      CSOpenXmlGetPlainText
* Copyright(c) Microsoft Corporation.
* 
* The Class is used to read plain text from word document.
* Microsoft Word *.docx is an Open XML document combining texts, stytle,grapyhics 
* and so on into a single ZIP archive. 
* The Class uses Open XML SDK API to read XML element and filter the text. 
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
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace CSUsingOpenXmlPlainText
{
    public class GetWordPlainText : IDisposable
    {
        // Specify whether the instance is disposed.
        private bool disposed = false;

        // The word package
        private WordprocessingDocument package = null;

        /// <summary>
        ///  Get the file name
        /// </summary>
        private string FileName = string.Empty;

        /// <summary>
        ///  Initialize the WordPlainTextManager instance
        /// </summary>
        /// <param name="filepath"></param>
        public GetWordPlainText(string filepath)
        {
            this.FileName = filepath;
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
            {
                throw new Exception("The file is invalid. Please select an existing file again");
            }

            this.package = WordprocessingDocument.Open(filepath, true);
        }

        /// <summary>
        ///  Read Word Document
        /// </summary>
        /// <returns>Plain Text in document </returns>
        public string ReadWordDocument()
        {
            StringBuilder sb = new StringBuilder();
            OpenXmlElement element = package.MainDocumentPart.Document.Body;
            if (element == null)
            {
                return string.Empty;
            }

            sb.Append(GetPlainText(element));
            return sb.ToString();
        }

        /// <summary>
        ///  Read Plain Text in all XmlElements of word document
        /// </summary>
        /// <param name="element">XmlElement in document</param>
        /// <returns>Plain Text in XmlElement</returns>
        public string GetPlainText(OpenXmlElement element)
        {
            StringBuilder PlainTextInWord = new StringBuilder();
            foreach (OpenXmlElement section in element.Elements())
            {
                switch (section.LocalName)
                {
                    // Text
                    case "t":
                        PlainTextInWord.Append(section.InnerText);
                        break;

                    case "cr":                          // Carriage return
                    case "br":                          // Page break
                        PlainTextInWord.Append(Environment.NewLine);
                        break;

                    // Tab
                    case "tab":
                        PlainTextInWord.Append("\t");
                        break;

                    // Paragraph
                    case "p":
                        PlainTextInWord.Append(GetPlainText(section));
                        PlainTextInWord.AppendLine(Environment.NewLine);
                        break;

                    default:
                        PlainTextInWord.Append(GetPlainText(section));
                        break;
                }
            }

            return PlainTextInWord.ToString();
        }

        #region IDisposable interface

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Protect from being called multiple times.
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Clean up all managed resources.
                if (this.package != null)
                {
                    this.package.Dispose();
                }
            }

            disposed = true;
        }
        #endregion
    }
}
