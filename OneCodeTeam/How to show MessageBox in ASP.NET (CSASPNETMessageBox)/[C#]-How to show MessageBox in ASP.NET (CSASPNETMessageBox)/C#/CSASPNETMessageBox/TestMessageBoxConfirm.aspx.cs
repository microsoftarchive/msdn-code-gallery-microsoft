/****************************** Module Header ******************************\
Module Name:  TestMessageBoxConfirm.aspx.cs
Project:      CSASPNETIntelligentErrorPage
Copyright (c) Microsoft Corporation.
 
The sample code demonstrates how to create a MessageBox in asp.net, usually we
often use JavaScript functions "alert()" or "confirm()" to show simple messages
and make a simple choice with customers, but these dialog boxes is very simple,
we can not add any different and complicate controls, images or styles. As we know,
good web sites always have their own web styles, such as typeface and colors, 
and in this situation, JavaScript dialog boxes looks not very well. So this sample
shows how to make an Asp.net MessageBox.

This page defines a customize MessageBox class with some necessary properties.
For example, title, text, icons, buttons, events, etc. The MessageBox class looks
like a windows form MessageBox but not the same, because these two application's working
mechanism is different, we need display the MessageBox in current web page, rather 
than open a new page for displaying messages, so we need a Literal control for
receive HTML code and use web service for getting different results.
 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
All other rights reserved.
 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

namespace CSASPNETMessageBox
{
    public partial class TestMessageBoxConfirm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnInvokeConfirm_Click(object sender, EventArgs e)
        {
            string title = "Confirm";
            string text = @"Hello everyone, I am an Asp.net MessageBox. You can set MessageBox.SuccessEvent and MessageBox.FailedEvent and Click Yes(OK) or No(Cancel) buttons for calling them. The Methods must be a WebMethod because client-side application will call web services.";
            MessageBox messageBox = new MessageBox(text, title, MessageBox.MessageBoxIcons.Question, MessageBox.MessageBoxButtons.OKCancel, MessageBox.MessageBoxStyle.StyleB);
            messageBox.SuccessEvent.Add("OkClick");
            messageBox.SuccessEvent.Add("OkClick");
            messageBox.FailedEvent.Add("CancalClick");
            Literal1.Text = messageBox.Show(this);
        }

        [WebMethod]
        public static string OkClick(object sender, EventArgs e)
        {
            return "You have clicked Ok button";
        }

        [WebMethod]
        public static string CancalClick(object sender, EventArgs e)
        {
            return "You have clicked Cancel button.";
        }
    }
}