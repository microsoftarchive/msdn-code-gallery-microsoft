/****************************** Module Header ******************************\
* Module Name:  UserControl2.ascx.cs
* Project:      CSASPNETUserControlPassData
* Copyright (c) Microsoft Corporation
*
* This project illustrates how to pass data from one user control to another.
* A user control can contain other controls like TextBoxes or Labels, These 
* control are declared as protected members, We cannot get the use control from
* another one directly.
* 
* The control is use to call UserControl1 user control's property.
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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSASPNETUserControlPassData
{
    public partial class UserControl2 : System.Web.UI.UserControl
    {
        private string strCaller = "I come from UserControl2.";
        private UserControl1 userControl1;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Output UserControl2 user control message.
                lbPublicVariable2.Text = strCaller;

                // Find UserControl1 user control.
                Control control = Page.FindControl("UserControl1ID");
                userControl1= (UserControl1)control;
                if (userControl1 != null)
                {
                    // Output UserControl1 user control message.
                    Label lbUserControl1 = userControl1.FindControl("lbPublicVariable") as Label;
                    if (lbUserControl1 != null)
                    {
                        lbUserControl1.Text = userControl1.StrCallee;
                        tbModifyUserControl1.Text = userControl1.StrCallee;
                    }
                }
            }
        }

        /// <summary>
        /// UserControl2 message.
        /// </summary>
        public string StrCaller
        {
            get
            {
                return strCaller;
            }
            set
            {
                strCaller = value;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!tbModifyUserControl1.Text.Trim().Equals(string.Empty))
            {
                Control control = Session["UserControl1"] as Control;
                userControl1 = control as UserControl1;
                if (userControl1 != null)
                {
                    // Set UserControl1 user control message.
                    lbFormatMessage.Text = string.Format("forward message: {0} ", userControl1.StrCallee);
                    userControl1.StrCallee = tbModifyUserControl1.Text;
                    Session["UserControl1"] = userControl1;
                    UserControl1 pageUserControl1 = Page.FindControl("UserControl1ID") as UserControl1;
                    Label lbUserControl1 = pageUserControl1.FindControl("lbPublicVariable") as Label;
                    lbUserControl1.Text = tbModifyUserControl1.Text;
                }
                else
                {
                    control = Page.FindControl("UserControl1ID");
                    userControl1 = (UserControl1)control;
                    userControl1.StrCallee = tbModifyUserControl1.Text.Trim();
                    Label lbUserControl1 = userControl1.FindControl("lbPublicVariable") as Label;
                    lbUserControl1.Text = userControl1.StrCallee;
                    Session["UserControl1"] = userControl1;
                }
            }
            else
            {
                lbMessage.Text = "The message can not be null.";
            }
        }
    }
}