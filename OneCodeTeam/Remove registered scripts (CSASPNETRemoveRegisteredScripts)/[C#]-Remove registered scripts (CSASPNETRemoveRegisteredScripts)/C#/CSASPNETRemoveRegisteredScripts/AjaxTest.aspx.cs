/****************************** Module Header ******************************\
* Module Name:    AjaxTest.aspx.cs
* Project:        CSASPNETRemoveRegisteredScripts
* Copyright (c) Microsoft Corporation
*
* This page shows how to use ScriptManager.RegisterClientScriptBlock and 
* ScriptManager.RegisterStartupScript  
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

namespace CSASPNETRemoveRegisteredScripts
{
    public partial class AjaxTest : System.Web.UI.Page
    {
        // Client script
        string script = @"
        function ToggleItem(id)
          {
            var elem = $get('div'+id);
            if (elem) 
            {
              if (elem.style.display != 'block') 
              {
                elem.style.display = 'block';
                elem.style.visibility = 'visible';
              } 
              else
              {
                elem.style.display = 'none';
                elem.style.visibility = 'hidden';
              }
            }
          }
        ";

        /// <summary>
        /// Register client script.        
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this,typeof(Page),"ToggleScript",script,true);

            // You can comment the code above and uncomment the code below
            // ScriptManager.RegisterStartupScript(this,typeof(Page),"ToggleScript",script,true);
        }

        /// <summary>
        /// Remove script: Register a null value to the existing key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRemove_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptBlock(this,typeof(Page),"ToggleScript","",true);

            // You can comment the code above and uncomment the code below
            // ScriptManager.RegisterStartupScript(this,typeof(Page),"ToggleScript","",true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
