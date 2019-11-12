/****************************** Module Header ******************************\
* Module Name:  XMLSerialization.cs
* Project:	    CSWebBrowserAutomation
* Copyright (c) Microsoft Corporation.
* 
* This class is used to serialize an object to an XML file or deserialize an XML
* file to an object.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.IO;
using System.Xml.Serialization;

namespace CSWebBrowserAutomation
{
    public class XMLSerialization<T>
    {

        /// <summary>
        /// Serialize an object to an XML file. 
        /// </summary>
        public static bool SerializeFromObjectToXML(T obj, string filepath)
        {
            if (obj == null)
            {
                throw new ArgumentException("The object to serialize could not be null!");
            }

            bool successed = false;
            Type objType = obj.GetType();
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite))
            {
                XmlSerializer xs = new XmlSerializer(objType);
                xs.Serialize(fs, obj);
                successed = true;
            }

            return successed;
        }

        /// <summary>
        /// Deserialize an XML file to an object.
        /// </summary>
        public static T DeserializeFromXMLToObject(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new ArgumentException("The file does not exist!");
            }

            T obj;
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                obj = (T)xs.Deserialize(fs);
            }
            return obj;
        }

    }
}
