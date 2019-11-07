/****************************** Module Header ******************************\
* Module Name:  CharacterEncoder.cs
* Project:	    CSRichTextBoxSyntaxHighlighting 
* Copyright (c) Microsoft Corporation.
* 
* This CharacterEncoder class supplies a static(Shared) method to encode some 
* special characters in Xml and Rtf, such as '<', '>', '"', '&', ''', '\',
* '{' and '}' .
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Text;

namespace CSRichTextBoxSyntaxHighlighting
{
    public class CharacterEncoder
    {
        public static string Encode(string originalText)
        {
            if (string.IsNullOrWhiteSpace(originalText))
            {
                return string.Empty;
            }

            StringBuilder encodedText = new StringBuilder();
            for (int i = 0; i < originalText.Length; i++)
            {
                switch (originalText[i])
                {
                    case '"':
                        encodedText.Append("&quot;");
                        break;
                    case '&':
                        encodedText.Append(@"&amp;");
                        break;
                    case '\'':
                        encodedText.Append(@"&apos;");
                        break;
                    case '<':
                        encodedText.Append(@"&lt;");
                        break;
                    case '>':
                        encodedText.Append(@"&gt;");
                        break;

                    // The character '\' should be converted to @"\\" or "\\\\" 
                    case '\\':
                        encodedText.Append(@"\\");
                        break;

                    // The character '{' should be converted to @"\{" or "\\{" 
                    case '{':
                        encodedText.Append(@"\{");
                        break;

                    // The character '}' should be converted to @"\}" or "\\}" 
                    case '}':
                        encodedText.Append(@"\}");
                        break;
                    default:
                        encodedText.Append(originalText[i]);
                        break;
                }

            }
            return encodedText.ToString();
        }
    }
}
