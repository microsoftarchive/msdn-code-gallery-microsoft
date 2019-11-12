using System;
using System.Collections.Generic;
using System.Linq;
/***************************** Module Header ******************************\
* Module Name:	Program.cs
* Project:		CSWordDocCompare2012
* Copyright (c) Microsoft Corporation.
* 
* This code sample shows how to compare two word documents and show the difference.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSWordDocCompare2012
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
