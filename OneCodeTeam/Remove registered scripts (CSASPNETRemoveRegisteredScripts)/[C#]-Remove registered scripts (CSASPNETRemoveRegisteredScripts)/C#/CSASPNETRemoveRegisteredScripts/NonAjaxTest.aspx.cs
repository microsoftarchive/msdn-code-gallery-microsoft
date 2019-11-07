/****************************** Module Header ******************************\
* Module Name:    NonAjaxTest.aspx.cs
* Project:        CSASPNETRemoveRegisteredScripts
* Copyright (c) Microsoft Corporation
*
* This page shows how to use ClientScript.RegisterClientScriptBlock and 
* ClientScript.RegisterStartupScript  
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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace CSASPNETRemoveRegisteredScripts
{
    public partial class NonAjaxTest : System.Web.UI.Page
    { 
        /// <summary>
        /// Register client script.
        /// RegisterStartupScript puts the javascript before the closing tag of the page
        /// and RegisterClientScriptBlock puts it right after the starting tag of the page 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!ClientScript.IsClientScriptBlockRegistered("RegisterKey"))
            {
                // Client script
                StringBuilder sbAlertScript = new StringBuilder();
                sbAlertScript.Append("<script language='javascript'>\n");
                sbAlertScript.Append("function test()\n");
                sbAlertScript.Append("{\n");
                sbAlertScript.Append("alert(\"test1\");\n");
                sbAlertScript.Append("}\n");
                sbAlertScript.Append("</script>\n");

                // Register the client script
                ClientScript.RegisterClientScriptBlock(this.GetType(), "RegisterKey", sbAlertScript.ToString());

                // You can comment the code above and uncomment the code below
                // ClientScript.RegisterStartupScript(this.GetType(), "RegisterKey", sbAlertScript.ToString());       
            }
        }

        /// <summary>
        /// Remove script: Register a null value to the existing key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRemove_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptBlock(this.GetType(), "RegisterKey", "");

            // You can comment the code above and uncomment the code below
            // ClientScript.RegisterStartupScript(this.GetType(), "RegisterKey", "");    
        }
    }
}
