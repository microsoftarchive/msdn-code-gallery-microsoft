/****************************** Module Header ******************************\
Module Name:  CascadingDropDownListWithCallBack.aspx.cs
Project:      CSASPNETCascadingDropDown
Copyright (c) Microsoft Corporation.
 
This page is used to show the Cascading DropDownList with callback.
  
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

namespace CSASPNETCascadingDropDownList
{
    public partial class CascadingDropDownListWithCallBack : System.Web.UI.Page
    {
        /// <summary>
        /// Page Load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Register client onclick event for submit button
            btnSubmit.Attributes.Add("onclick", "SaveSelectedData();");
            if (!IsPostBack)
            {
                // Bind country dropdownlist
                BindddlCountry();
                // Initialize hide field value
                hdfResult.Value = "";
                hdfCity.Value = "";
                hdfCitySelectValue.Value = "0";
                hdfRegion.Value = "";
                hdfRegionSelectValue.Value = "";
            }
        }

        /// <summary>
        /// Bind country dropdownlist
        /// </summary>
        public void BindddlCountry()
        {
            List<string> list = RetrieveDataFromXml.GetAllCountry();
            ddlCountry.DataSource = list;
            ddlCountry.DataBind();
        }


        /// <summary>
        /// Submit button click event and show select result
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Get result from hide field saved in client
            string strResult = hdfResult.Value;
            lblResult.Text = strResult;
        }
    }
}