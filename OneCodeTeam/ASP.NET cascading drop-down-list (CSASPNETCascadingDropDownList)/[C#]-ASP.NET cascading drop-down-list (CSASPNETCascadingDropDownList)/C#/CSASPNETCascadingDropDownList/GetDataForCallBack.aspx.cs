/****************************** Module Header ******************************\
Module Name:  GetDataForCallBack.aspx.cs
Project:      CSASPNETCascadingDropDown
Copyright (c) Microsoft Corporation.
 
This page is used to retrieve data in callback and write data to client.
 
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
using System.Text;

namespace CSASPNETCascadingDropDownList
{
    public partial class GetDataForCallBack : System.Web.UI.Page
    {
        /// <summary>
        /// Page Load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get querystring from URL and retrieve data based on it
            if (Request.QueryString.Count > 0)
            {
                string strValue = Request.QueryString[0];
                if (Request.QueryString["country"] != null)
                {
                    RetrieveRegionByCountry(strValue);
                }
                else
                {
                    RetrieveCityByRegion(strValue);
                }
            }
        }

        /// <summary>
        /// Get region based on country value
        /// </summary>
        /// <param name="strValue">The country value</param>
        public void RetrieveRegionByCountry(string strValue)
        {
            List<string> list = RetrieveDataFromXml.GetRegionByCountry(strValue);
            WriteData(list);
        }

        /// <summary>
        /// Get city based on region value
        /// </summary>
        /// <param name="strValue">The region value</param>
        public void RetrieveCityByRegion(string strValue)
        {
            List<string> list = RetrieveDataFromXml.GetCityByRegion(strValue);
            WriteData(list);
        }

        /// <summary>
        /// Write data to client
        /// </summary>
        /// <param name="list">The list contains value </param>
        public void WriteData(List<string> list)
        {
            int iCount = list.Count;
            StringBuilder builder = new StringBuilder();
            if (iCount > 0)
            {
                for (int i = 0; i < iCount - 1; i++)
                {
                    builder.Append(list[i] + ",");
                }
                builder.Append(list[iCount - 1]);
            }

            Response.ContentType = "text/xml";
            // Write data in string format "###,###,###" to client
            Response.Write(builder.ToString());
            Response.End();
        }
    }
}