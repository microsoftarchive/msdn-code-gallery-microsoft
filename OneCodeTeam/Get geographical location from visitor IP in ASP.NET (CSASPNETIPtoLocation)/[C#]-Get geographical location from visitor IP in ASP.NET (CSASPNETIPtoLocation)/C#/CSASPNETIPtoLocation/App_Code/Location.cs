/**************************** Module Header ********************************\
* Module Name:    Location.cs
* Project:        CSASPNETIPtoLocation
* Copyright (c) Microsoft Corporation
*
* This project illustrates how to get the geographical location from a db file.
* You need install Sqlserver Express for run the web applicaiton. The code-sample
* only support Internet Protocol version 4.
* 
* This class is IP location information entity class. 
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
using System.Text;

namespace CSASPNETIPtoLocation
{
    public class Location
    {
        private string beginIP;
        private string endIP;
        private string countryTwoCode;
        private string countryThreeCode;
        private string countryName;

        public string BeginIP
        {
            get
            {
                return beginIP;
            }
            set
            {
                beginIP = value;
            }
        }

        public string EndIP
        {
            get
            {
                return endIP;
            }
            set
            {
                endIP = value;
            }
        }

        public string CountryTwoCode
        {
            get
            {
                return countryTwoCode;
            }
            set
            {
                countryTwoCode = value;
            }
        }

        public string CountryThreeCode
        {
            get
            {
                return countryThreeCode;
            }
            set
            {
                countryThreeCode = value;
            }
        }

        public string CountryName
        {
            get
            {
                return countryName;
            }
            set
            {
                countryName = value;
            }
        }
    }
}
