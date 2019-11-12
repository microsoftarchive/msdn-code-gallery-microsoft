/****************************** Module Header ******************************\
* Module Name:  BlockSites.cs
* Project:	    CSWebBrowserAutomation
* Copyright (c) Microsoft Corporation.
* 
* This BlockSites class includes a list of block sites. The static method GetBlockSites
* deserializes the BlockList.xml to a BlockSites instance.
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
using System.Collections.Generic;

namespace CSWebBrowserAutomation
{
    public class BlockSites
    {
        public List<string> Hosts { get; set; }

        private static readonly BlockSites instance = GetBlockSites();

        public static BlockSites Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Deserialize the BlockList.xml to a BlockSites instance.
        /// </summary>
        static private BlockSites GetBlockSites()
        {
            string path = string.Format(@"{0}\Resources\BlockList.xml",
                Environment.CurrentDirectory);
            return XMLSerialization<BlockSites>.DeserializeFromXMLToObject(path);
        }
    }
}
