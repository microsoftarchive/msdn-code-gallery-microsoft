/****************************** Module Header ******************************\
* Module Name: Default.aspx.cs
* Project:     CSASPNETShowSpinnerImage
* Copyright (c) Microsoft Corporation
*
* This project illustrates how to show spinner image while retrieving huge of 
* data. As we know, handle a time-consuming operate always requiring a long 
* time, we need to show a spinner image for better user experience.
* 
* This page is used to retrieve data from XML file, and includes PopupProgeress
* user control. 
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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;
using System.Data;

namespace CSASPNETShowSpinnerImage
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            // Here we use Thread.Sleep() to suspends the thread for 10 seconds for imitating
            // an expensive, time-consuming operate of retrieve data. (Such as connect network
            // database to retrieve mass data.)
            // So in practice application, you can remove this line. 
            System.Threading.Thread.Sleep(10000);

            // Retrieve data from XML file as sample data.
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "XMLFile/XMLData.xml");
            DataTable tabXML = new DataTable();
            DataColumn columnName = new DataColumn("Name", Type.GetType("System.String"));
            DataColumn columnAge = new DataColumn("Age", Type.GetType("System.Int32"));
            DataColumn columnCountry = new DataColumn("Country", Type.GetType("System.String"));
            DataColumn columnComment = new DataColumn("Comment", Type.GetType("System.String"));
            tabXML.Columns.Add(columnName);
            tabXML.Columns.Add(columnAge);
            tabXML.Columns.Add(columnCountry);
            tabXML.Columns.Add(columnComment);
            XmlNodeList nodeList = xmlDocument.SelectNodes("Root/Person");
            foreach (XmlNode node in nodeList)
            {
                DataRow row = tabXML.NewRow();
                row["Name"] = node.SelectSingleNode("Name").InnerText;
                row["Age"] = node.SelectSingleNode("Age").InnerText;
                row["Country"] = node.SelectSingleNode("Country").InnerText;
                row["Comment"] = node.SelectSingleNode("Comment").InnerText;
                tabXML.Rows.Add(row);
            }
            gvwXMLData.DataSource = tabXML;
            gvwXMLData.DataBind();
        }


    }
}