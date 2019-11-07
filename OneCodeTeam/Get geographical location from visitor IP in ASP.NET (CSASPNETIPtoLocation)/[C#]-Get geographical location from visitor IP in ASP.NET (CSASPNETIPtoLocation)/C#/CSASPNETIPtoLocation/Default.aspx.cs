/**************************** Module Header ********************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETIPtoLocation
* Copyright (c) Microsoft Corporation
*
* This project illustrates how to get the geographical location from a db file.
* You need install Sqlserver Express for run the web applicaiton. The code-sample
* only support Internet Protocol version 4.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/


using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace CSASPNETIPtoLocation
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string ipAddress;

            // Get the client's IP address. If you get the result of "::1", it's the IPv6 version
            // of your IP address, you can disable IPv6 components if you want to get IPv4 version.
            // And you need change the registry key when you disable it, check this link:
            // http://support.microsoft.com/kb/929852
            ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            }

            lbIPAddress.Text = "Your IP Address is: [" + ipAddress + "].";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string ipAddress = tbIPAddress.Text.Trim();
            Location locationInfo = new Location();
            if (String.IsNullOrEmpty(ipAddress.Trim()))
            {
                Response.Write("<strong>Please input an IP address</strong>");
                return;
            }

            // Get the IP address string and calculate IP number.
            string ipRange = IPConvert.ConvertToIPRange(ipAddress);
            DataTable tabLocation = new DataTable();

            // Create a connection to Sqlserver
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConectString"].ToString()))
            {
                string selectCommand = "select * from IPtoLocation where CAST(" + ipRange + " as bigint) between BeginingIP and EndingIP";
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(selectCommand, sqlConnection);
                sqlConnection.Open();
                sqlAdapter.Fill(tabLocation);
            }

            // Store IP infomation into Location entity class
            if (tabLocation.Rows.Count == 1)
            {
                locationInfo.BeginIP = tabLocation.Rows[0][0].ToString();
                locationInfo.EndIP = tabLocation.Rows[0][1].ToString();
                locationInfo.CountryTwoCode = tabLocation.Rows[0][2].ToString();
                locationInfo.CountryThreeCode = tabLocation.Rows[0][3].ToString();
                locationInfo.CountryName = tabLocation.Rows[0][4].ToString();
            }
            else
            {
                Response.Write("<strong>Cannot find the location based on the IP address [" + ipAddress + "].</strong> ");
                return;
            }

            // Output.
            Response.Write("<strong>Country Code(Two):</strong> ");
            Response.Write(locationInfo.CountryTwoCode + "<br />");

            Response.Write("<strong>Country Code(Three):</strong> ");
            Response.Write(locationInfo.CountryThreeCode + "<br />");

            Response.Write("<strong>Country Name:</strong> ");
            Response.Write(locationInfo.CountryName + "<br />");
            
            lbIPAddress.Visible = false;
        }

    }
}