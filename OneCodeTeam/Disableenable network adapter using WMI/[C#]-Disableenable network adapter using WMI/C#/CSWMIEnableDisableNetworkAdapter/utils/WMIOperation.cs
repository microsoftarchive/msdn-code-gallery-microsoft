/****************************** Module Header ******************************\
* Module Name:  WMIOperation.cs
* Project:	    CSWMIEnableDisableNetworkAdapter
* Copyright (c) Microsoft Corporation.
* 
* This is a class which used to handle some operation of a WMI object.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Management;

namespace CSWMIEnableDisableNetworkAdapter
{
    public class WMIOperation
    {
        public static ManagementObjectCollection WMIQuery(string strwQuery)
        {
            ObjectQuery oQuery = new ObjectQuery(strwQuery);
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oQuery);
            ManagementObjectCollection oReturnCollection = oSearcher.Get();
            return oReturnCollection;
        }
    }
}
