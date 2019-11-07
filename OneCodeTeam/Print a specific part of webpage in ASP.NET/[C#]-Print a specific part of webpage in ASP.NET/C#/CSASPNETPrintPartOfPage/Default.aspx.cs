/****************************** Module Header ******************************\
* Module Name: Default.aspx.cs
* Project:     CSASPNETPrintPartOfPage
* Copyright (c) Microsoft Corporation
*
* This project illustrates how to print specific parts of a web page by using Cascading Style
* Sheets and JavaScript. We need to set div layer in the part of web page, and
* use JavaScript code to print the valid part of it.
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

namespace CSASPNETPrintPartOfPage
{
    public partial class Default : System.Web.UI.Page
    {
        // Define some strings, use to add div elements.
        public string PrintImageBegin;
        public string PrintImageEnd;
        public string PrintListBegin;
        public string PrintListEnd;
        public string PrintButtonBegin;
        public string PrintButtonEnd;
        public string PrintSearchBegin;
        public string PrintSearchEnd;
        public const string EnablePirnt = "<div class=nonPrintable>";
        public const string EndDiv = "</div>";
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check the status of CheckBox, set div elements.
            if (chkDisplayImage.Checked)
            { 
                PrintImageBegin = string.Empty;
                PrintImageEnd = string.Empty;
            }
            else
            { 
                PrintImageBegin = EnablePirnt;
                PrintImageEnd = EndDiv;
            }
            if (chkDisplayList.Checked)
            {
                PrintListBegin = string.Empty;
                PrintListEnd = string.Empty;
            }
            else
            { 
                PrintListBegin = EnablePirnt;
                PrintListEnd = EndDiv; 
            }
            if (chkDisplayButton.Checked)
            { 
                PrintButtonBegin = string.Empty;
                PrintButtonEnd = string.Empty;
            }
            else
            {
                PrintButtonBegin = EnablePirnt; 
                PrintButtonEnd = EndDiv; 
            }
            if (chkDisplaySearch.Checked)
            { 
                PrintSearchBegin = string.Empty;
                PrintSearchEnd = string.Empty; 
            }
            else
            {
                PrintSearchBegin = EnablePirnt;
                PrintSearchEnd = EndDiv; 
            }
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PrintOperation", "print()", true);
        }
    }
}