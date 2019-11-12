/****************************** Module Header ******************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETVerificationImage
* Copyright (c) Microsoft Corporation
*
* This is the test page.
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

namespace CSASPNETVerificationImage
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Compare the value in session and type. If equal, set a success to the text of Literal, otherwise failed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnOK_Click(object sender, EventArgs e)
        {
            if (tbCode.Text.Trim().ToLower().Equals(Session["ValidateCode"].ToString().ToLower()))
            {
                ltrMessage.Text = "success";
            }
            else
            {
                ltrMessage.Text = "failed";
            }
        }
    }
}
