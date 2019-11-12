/****************************** Module Header ******************************\
* Module Name:  GlobleData.cs
* Project:      CSWinFormSingleInstanceApp
* Copyright (c) Microsoft Corporation.
* 
* The  sample demonstrates how to achieve the goal that only 
* one instance of the application is allowed in Windows Forms application..
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSWinFormSingleInstanceApp
{
    public class GlobleData
    {
        static bool isUserLoggedIn;

        public static bool IsUserLoggedIn
        {
            get { return GlobleData.isUserLoggedIn; }
            set { GlobleData.isUserLoggedIn = value; }
        }

        static string userName;

        public static string UserName
        {
            get { return GlobleData.userName; }
            set { GlobleData.userName = value; }
        }
    }
}
