/****************************** Module Header ******************************\
*Module Name:  GetCustomer.aspx.cs
*Project:      SalesSendMessagesViaTopic
*Copyright (c) Microsoft Corporation.
* 
* Gets the data of the customer from database based on the customer name value input
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
    public partial class GetCustomer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string jsonResult = string.Empty;
            DALClass dalClass = new DALClass();

            try
            {
                string strCustomerName = Request["strCustomerName"];
                DataTable dtCustomer = dalClass.GetCustomerByName(strCustomerName);
                if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                {

                    string strAddress = string.Empty;
                    string strTelePhone = string.Empty;

                    if (!(dtCustomer.Rows[0]["CustomerAddress"] is DBNull))
                    {
                        strAddress = dtCustomer.Rows[0]["CustomerAddress"].ToString();
                    }

                    if (!(dtCustomer.Rows[0]["CustomerTelePhone"] is DBNull))
                    {
                        strTelePhone = dtCustomer.Rows[0]["CustomerTelePhone"].ToString();
                    }
                    jsonResult = "{\"Customer\":" +
                         "[" +
                         "{\"Address\":\"" + strAddress + "\",\"TelePhone\":\"" + strTelePhone + "\"}" +
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