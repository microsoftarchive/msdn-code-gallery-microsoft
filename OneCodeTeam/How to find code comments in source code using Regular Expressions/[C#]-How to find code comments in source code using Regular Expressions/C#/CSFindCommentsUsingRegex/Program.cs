/****************************** Module Header ******************************\
 * Module Name:  Program.cs
 * Project:      CSFindCommentsUsingRegex
 * Copyright (c) Microsoft Corporation.
 * 
 * The Main program.
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CSFindCommentsUsingRegex
{
    class Program
    {
        static void Main(string[] args)
        {
            // file variable points to the .cs file where we want to know the code comments from
            string file = "../../SourceCode.cs";
            string pattern = @"(/\*([^*]|[\r\n]|(\*+([^*/]|[\r\n])))*\*+/)|(//.*)";

            // local variable to get hold of single or multi-line code comments
            string sLine = string.Empty;
            string mLine = string.Empty;
            
            // Read the file stream
            FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(stream);

            while (reader.Peek() != -1)
            {
                // Read a line of the stream reader
                sLine = reader.ReadLine();

                // Trim the space in the start of the line
                sLine = (sLine != null) ? sLine.Trim() : sLine;

                if (sLine.StartsWith(@"/*") && !sLine.EndsWith(@"*/"))
                {
                    mLine = sLine + Environment.NewLine;
                    continue;
                }
                if (mLine.StartsWith(@"/*"))
                {
                    if (sLine.EndsWith(@"*/"))
                    {
                        mLine += sLine;
                    }
                    else
                    {
                        mLine += sLine + Environment.NewLine;
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(mLine))
                {
                    var res = Regex.Matches(mLine, pattern);
                    if(res.Count > 0)
                        Console.WriteLine(mLine);
                    mLine = string.Empty;
                }
                else
                {
                    var res = Regex.Matches(sLine, pattern);
                    if(res.Count > 0)
                        Console.WriteLine(sLine);
                }
            }

            // Dispose the stream objects
            reader.Close();
            stream.Close();

            Console.ReadLine();
        }
    }
}
