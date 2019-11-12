/****************************** Module Header ******************************\
* Module Name:  Default.aspx.cs
* Project:      CSASPNETUserControlPassData
* Copyright (c) Microsoft Corporation
*
* This project illustrates how to pass data from one user control to another.
* A user control can contain other controls like TextBoxes or Labels, These 
* control are declared as protected members, We cannot get the use control from
* another one directly.
* 
* This is the main user page, we load two user controls and host them in this 
* page.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/



using System;

namespace CSASPNETUserControlPassData
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}