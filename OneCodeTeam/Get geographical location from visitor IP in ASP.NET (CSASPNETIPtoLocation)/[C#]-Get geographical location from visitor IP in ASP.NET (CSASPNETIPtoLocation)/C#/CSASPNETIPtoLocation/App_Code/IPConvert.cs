/**************************** Module Header ********************************\
* Module Name:    IPConvert.cs
* Project:        CSASPNETIPtoLocation
* Copyright (c) Microsoft Corporation
*
* This project illustrates how to get the geographical location from a db file.
* You need install Sqlserver Express for run the web applicaiton. The code-sample
* only support Internet Protocol version 4.
* 
* This class use to calculate the IP number from IP address.
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
using System.Text;

namespace CSASPNETIPtoLocation
{
    public class IPConvert
    {
        public static string ConvertToIPRange(string ipAddress)
        {
            try
            {
                string[] ipArray = ipAddress.Split('.');
                int number = ipArray.Length;
                double ipRange = 0;
                if (number != 4)
                {
                    return "error ipAddress";
                }
                for (int i = 0; i < 4; i++)
                {
                    int numPosition = int.Parse(ipArray[3 - i].ToString());
                    if (i == 4)
                    {
                        ipRange += numPosition;
                    }
                    else
                    {
                        ipRange += ((numPosition % 256) * (Math.Pow(256, (i))));
                    }
                }
                return ipRange.ToString();
            }
            catch (Exception)
            {
                return "error";
            }
        }

    }
}