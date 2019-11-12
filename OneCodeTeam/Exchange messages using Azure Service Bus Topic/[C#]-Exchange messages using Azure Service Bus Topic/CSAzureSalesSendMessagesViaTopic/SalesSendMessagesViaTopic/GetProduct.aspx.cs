/****************************** Module Header ******************************\
*Module Name:  GetProduct.aspx.cs
*Project:      SalesSendMessagesViaTopic
*Copyright (c) Microsoft Corporation.
* 
* Gets the data of the product from database based on the productno value input
* 
*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
* 
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SalesSendMessagesViaTopic
{
    public partial class GetProduct : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string jsonResult = string.Empty;
            DALClass dalClass = new DALClass();
            try
            {
                string strProductNo = Request["strProductNo"];
                DataTable dtProduct = dalClass.GetProductByNo(strProductNo);
                if (dtProduct != null && dtProduct.Rows.Count > 0)
                {
                    string strProductName = string.Empty;
                    string strProductColor = string.Empty;
                    string strProductSize = string.Empty;
                    string strProductPrice = string.Empty;
                    string strProductId = string.Empty;
                    if (!(dtProduct.Rows[0]["ProductName"] is DBNull))
                    {
                        strProductName = dtProduct.Rows[0]["ProductName"].ToString();
                    }
                    if (!(dtProduct.Rows[0]["ProductColor"] is DBNull))
                    {
                        strProductColor = dtProduct.Rows[0]["ProductColor"].ToString();
                    }
                    if (!(dtProduct.Rows[0]["ProductSize"] is DBNull))
                    {
                        strProductSize = dtProduct.Rows[0]["ProductSize"].ToString();
                    }
                    if (!(dtProduct.Rows[0]["ProductPrice"] is DBNull))
                    {
                        strProductPrice = dtProduct.Rows[0]["ProductPrice"].ToString();
                    }
                    if (!(dtProduct.Rows[0]["ProductId"] is DBNull))
                    {
                        strProductId = dtProduct.Rows[0]["ProductId"].ToString();
                    }
                    jsonResult = "{\"Product\":" +
                         "[" +
                            "{\"ProductName\":\"" + strProductName + "\",\"ProductColor\":\"" + strProductColor + "\"," +
                            "\"ProductSize\":\"" + strProductSize + "\"," + "\"ProductPrice\":\"" + strProductPrice + "\"," +
                            "\"ProductId\":\"" + strProductId + "\"}" +
                         "]" +
                        "}";
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
            Response.Write(jsonResult);
            Response.End();

        }
    }
}