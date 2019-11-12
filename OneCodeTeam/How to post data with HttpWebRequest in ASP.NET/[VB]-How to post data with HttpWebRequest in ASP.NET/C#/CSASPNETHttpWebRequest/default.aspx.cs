/***************************** Module Header ******************************\
* Module Name:	default.aspx.cs
* Project:		CSASPNETHttpWebRequest
* Copyright (c) Microsoft Corporation.
* 
* This sample will show you how to create HTTPWebReqeust, and get HTTPWebResponse.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Xml;

namespace CSASPNETHttpWebRequest
{
    public partial class _default : System.Web.UI.Page
    {
        private const string url = "http://localhost:25794/RESTAPI.svc/json";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSendRequest_Click(object sender, EventArgs e)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(new Uri(url+"/1"));
            req.Method = "Get";
            try
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Stream sw = res.GetResponseStream();
                StreamReader reader = new StreamReader(sw);
                Response.Write("Your GET Request response XML value:"+reader.ReadToEnd());
                res.Close();
                sw.Close();
                reader.Close();
            
            }
            catch (Exception)
            {  
                throw;
            }
        }

        protected void btnSendPostRequest_Click(object sender, EventArgs e)
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;
            try
            {
                req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/xml; charset=utf-8";
                // req.Timeout = 30000;

                var xmlDoc = new XmlDocument { XmlResolver = null };
                xmlDoc.Load(Server.MapPath("PostData.xml"));
                string sXml = xmlDoc.InnerXml;
                req.ContentLength = sXml.Length;
                var sw = new StreamWriter(req.GetRequestStream());
                sw.Write(sXml);
                sw.Close();

                res = (HttpWebResponse)req.GetResponse();
                Stream responseStream = res.GetResponseStream();
                var streamReader = new StreamReader(responseStream);

                // Read the response into an xml document
                var xml = new XmlDocument();
                xml.LoadXml(streamReader.ReadToEnd());

                // return only the xml representing the response details (inner request)
                Response.Write("Your POST Request response XML value:"+xml.InnerXml);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            } 
        }
    }
}