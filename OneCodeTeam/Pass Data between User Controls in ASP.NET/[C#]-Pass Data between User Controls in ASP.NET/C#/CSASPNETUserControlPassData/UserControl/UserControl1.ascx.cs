/****************************** Module Header ******************************\
* Module Name:  UserControl1.ascx.cs
* Project:      CSASPNETUserControlPassData
* Copyright (c) Microsoft Corporation
*
* This project illustrates how to pass data from one user control to another.
* A user control can contain other controls like TextBoxes or Labels, These 
* control are declared as protected members, We cannot get the use control from
* another one directly.
* 
* The control is use to be called by UserControl2 user control.
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
    public partial class UserControl1 : System.Web.UI.UserControl
    {
        private string strCallee;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                strCallee = "I come from UserControl1.";
                lbPublicVariable.Text = strCallee;
            }
            
        }

        /// <summary>
        /// UserControl1 message.
        /// </summary>
        public string StrCallee
        {
            get
            {
                return strCallee;
            }
            set
            {
                strCallee = value;
            }
        }
    }
}