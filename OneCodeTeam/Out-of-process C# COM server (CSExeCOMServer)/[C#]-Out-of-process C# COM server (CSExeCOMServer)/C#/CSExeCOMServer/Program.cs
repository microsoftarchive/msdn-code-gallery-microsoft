/****************************** Module Header ******************************\
* Module Name:  Program.cs
* Project:      CSExeCOMServer
* Copyright (c) Microsoft Corporation.
* 
* The main entry point for the application. It is responsible for starting  
* the out-of-proc COM server registered in the executable.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion


namespace CSExeCOMServer
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            // Run the out-of-process COM server
            ExeCOMServer.Instance.Run();
        }
    }
}
