/****************************** Module Header ******************************\
*Module Name: HelperClass.cs
*Project:      SalesSendMessagesViaTopic
*Copyright (c) Microsoft Corporation.
* 
*This class provides methods for many common operations on Azure database tables. 
*For example, insert data into the table or select data from the table.
*
*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
* 
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SalesSendMessagesViaTopic
{

  [Serializable]  
    public class Customer
    {       
        public double Value { get; set; }

        string strCustomerId;
       
        public string StrCustomerId
        {
            get { return strCustomerId; }
            set { strCustomerId = value; }
        }
        string strCustomerName;
        
        public string StrCustomerName
        {
            get { return strCustomerName; }
            set { strCustomerName = value; }
        }
        string strCustomerTelephone;
       
        public string StrCustomerTelephone
        {
            get { return strCustomerTelephone; }
            set { strCustomerTelephone = value; }
        }
      
        string strCustomerAddress;
          
        public string StrCustomerAddress
        {
            get { return strCustomerAddress; }
            set { strCustomerAddress = value; }
        }
    }

    [Serializable]  
    public class SalesOrder
    {
        string strSalesOrderNo;

        public string StrSalesOrderNo
        {
            get { return strSalesOrderNo; }
            set { strSalesOrderNo = value; }
        }
        string strSalesOrderType;

        public string StrSalesOrderType
        {
            get { return strSalesOrderType; }
            set { strSalesOrderType = value; }
        }
       
        string strCustomerId;

        public string StrCustomerId
        {
            get { return strCustomerId; }
            set { strCustomerId = value; }
        }
         
        string strDeliveryDate;

        public string StrDeliveryDate
        {
            get { return strDeliveryDate; }
            set { strDeliveryDate = value; }
        }

        string strSalesManId;

        public string StrSalesManId
        {
            get { return strSalesManId; }
            set { strSalesManId = value; }
        }
       
    }

    [Serializable]  
    public class SalesOrderProductDetails
    {
        
        string strProductId;

        public string StrProductId
        {
            get { return strProductId; }
            set { strProductId = value; }
        }
        string strProductNo;

        public string StrProductNo
        {
            get { return strProductNo; }
            set { strProductNo = value; }
        }
        
        string strNumber;

        public string StrNumber
        {
            get { return strNumber; }
            set { strNumber = value; }
        }
        string strUnit;

        public string StrUnit
        {
            get { return strUnit; }
            set { strUnit = value; }
        }
    }
}