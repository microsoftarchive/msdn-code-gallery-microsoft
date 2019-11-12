/****************************** Module Header ******************************\
* Module Name: Default.aspx.cs
* Project:     CSASPNETDragItemInListView
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to drag and drop items in ListView using JQuery.
* In this page, bind two xml data files to ListView and use ItemTemplate to display
* them, cite JQuery javascript library to implements these functions in 
* Default.aspx page.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/


using System;
using System.Xml;
using System.Data;
namespace CSASPNETDragItemInListView
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Bind two xml data file to ListView control, actually you can change the "open" property to "0",
            // In that way, it will not display in ListView control.
            XmlDocument xmlDocument = new XmlDocument();
            using (DataTable tabListView1 = new DataTable())
            {
                tabListView1.Columns.Add("value", Type.GetType("System.String"));
                xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "/XmlFile/ListView1.xml");
                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("root/data[@open='1']");
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    DataRow dr = tabListView1.NewRow();
                    dr["value"] = xmlNode.InnerText;
                    tabListView1.Rows.Add(dr);
                }
                ListView1.DataSource = tabListView1;
                ListView1.DataBind();
            }

            XmlDocument xmlDocument2 = new XmlDocument();
            using (DataTable tabListView2 = new DataTable())
            {
                tabListView2.Columns.Add("value2", Type.GetType("System.String"));
                xmlDocument2.Load(AppDomain.CurrentDomain.BaseDirectory + "/XmlFile/ListView2.xml");
                XmlNodeList xmlNodeList2 = xmlDocument2.SelectNodes("root/data[@open='1']");
                foreach (XmlNode xmlNode in xmlNodeList2)
                {
                    DataRow dr = tabListView2.NewRow();
                    dr["value2"] = xmlNode.InnerText;
                    tabListView2.Rows.Add(dr);
                }
                ListView2.DataSource = tabListView2;
                ListView2.DataBind();
            }
        }
    }
}