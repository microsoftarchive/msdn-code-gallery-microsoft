/****************************** Module Header ******************************\
Module Name:  CSRetrieveDataFromXml.cs
Project:      CSASPNETCascadingDropDown
Copyright (c) Microsoft Corporation.
 
This class is used to access data source and retrieve data. In this case, 
the data source is XML file.
 
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
using System.Xml;

namespace CSASPNETCascadingDropDownList
{
    /// <summary>
    /// Summary description for CSRetrieveDataFromXml
    /// </summary>
    public class RetrieveDataFromXml
    {
               /// <summary>
        /// Constructor 
        /// </summary>
        public RetrieveDataFromXml()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        /// <summary>
        /// Get the data source path, in this case, it is the XML file in the 
        /// App_Data folder
        /// </summary>
        private static string strXmlPath = HttpContext.Current.Server.MapPath(
            "App_Data") + "\\CustomDataSource.xml";


        /// <summary>
        /// Get all country
        /// </summary>
        /// <returns>The List contains country</returns>
        public static List<string> GetAllCountry()
        {
            XmlDocument document = new XmlDocument();
            document.Load(strXmlPath);
            XmlNodeList nodeList = document.SelectNodes("Countries/Country");
            List<string> list = new List<string>();
            foreach (XmlNode node in nodeList)
            {
                list.Add(node.Attributes["name"].Value);
            }

            return list;
        }


        /// <summary>
        /// Get region based on country
        /// </summary>
        /// <param name="strCountry">The country name</param>
        /// <returns>The list contains region</returns>
        public static List<string> GetRegionByCountry(string strCountry)
        {
            XmlDocument document = new XmlDocument();
            document.Load(strXmlPath);
            XmlNodeList nodeList = document.SelectNodes(
                "Countries/Country[@name='" + strCountry + "']/Region");
            List<string> list = new List<string>();
            foreach (XmlNode node in nodeList)
            {
                list.Add(node.Attributes["name"].Value);
            }

            return list;
        }


        /// <summary>
        ///  Get city based on region
        /// </summary>
        /// <param name="strRegion">The region name</param>
        /// <returns>The list contains city</returns>
        public static List<string> GetCityByRegion(string strRegion)
        {
            XmlDocument document = new XmlDocument();
            document.Load(strXmlPath);
            XmlNodeList nodeList = document.SelectNodes(
                "Countries/Country/Region[@name='" + strRegion + "']/City");
            List<string> list = new List<string>();
            foreach (XmlNode node in nodeList)
            {
                list.Add(node.InnerText.ToString());
            }
            return list;
        }
    }
}