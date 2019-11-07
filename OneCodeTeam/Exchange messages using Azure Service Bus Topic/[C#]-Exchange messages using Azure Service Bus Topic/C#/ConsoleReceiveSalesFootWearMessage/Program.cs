/****************************** Module Header ******************************\
*Module Name:  Program.cs
*Project:      ConsoleReceiveSalesFootWearMessage
*Copyright (c) Microsoft Corporation.
* 
*In contrast to queues, in which each message is processed by a single consumer,
*topics and subscriptions provide a one-to-many form of communication, in a publish/subscribe pattern.
*
*This project will automatically receive messages that the sales department send when a Footwear sales order is built.
*
*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
* 
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using CSAzureReceiveSalesMessageViaTopic;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleReceiveSalesFootWearMessage
{
    class Program
    {
        static DALClass dalClassHelper;

        public static DALClass DalClassHelper
        {
            get
            {
                if (dalClassHelper == null)
                {
                    dalClassHelper = new DALClass();
                }
                return dalClassHelper;
            }

        }
        static string strTopic = DALClass.StrTopic;
        static string strServiceBusConnect =DALClass.StrServiceBusConnect;

        static void Main(string[] args)
        {
            try
            {
                ReceiveMessage();

            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Receives messages that the sales department send when a Footwear sales order is built .
        /// </summary>
        /// <returns></returns>
        static bool ReceiveMessage()
        {
            bool blnRet = false;
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(strServiceBusConnect);
            IEnumerable<SubscriptionDescription> lstSub = namespaceManager.GetSubscriptions(strTopic);
            foreach (SubscriptionDescription sub in lstSub)
            {
                if (sub.Name.Contains("Footwear"))
                {

                    SubscriptionClient subClient = SubscriptionClient.CreateFromConnectionString(strServiceBusConnect, strTopic, sub.Name);
                    while (true)
                    {
                        try
                        {
                            BrokeredMessage message = null;
                            message = subClient.Receive();
                            if (message != null)
                            {
                                string strCustomer = "";
                                string strSalesOrder = "";
                                string strSalesOrderDetails = "";
                                if (message.Properties.ContainsKey("Customer") && message.Properties["Customer"] != null)
                                {
                                    strCustomer = message.Properties["Customer"].ToString();
                                }
                                if (message.Properties.ContainsKey("SalesOrder") && message.Properties["SalesOrder"] != null)
                                {
                                    strSalesOrder = message.Properties["SalesOrder"].ToString();
                                }
                                if (message.Properties.ContainsKey("SalesOrderDetails") && message.Properties["SalesOrderDetails"] != null)
                                {
                                    strSalesOrderDetails = message.Properties["SalesOrderDetails"].ToString();
                                }
                                if (!string.IsNullOrEmpty(strCustomer) && !string.IsNullOrEmpty(strSalesOrder) && !string.IsNullOrEmpty(strSalesOrderDetails))
                                {
                                    Customer customer = (Customer)Deserialize(strCustomer, typeof(Customer));
                                    SalesOrder salesOrder = (SalesOrder)Deserialize(strSalesOrder, typeof(SalesOrder));
                                    List<SalesOrderProductDetails> lstOrderDetails = (List<SalesOrderProductDetails>)Deserialize(strSalesOrderDetails, typeof(List<SalesOrderProductDetails>));

                                    if (SaveMessage(customer, salesOrder, lstOrderDetails))
                                    {
                                        message.Complete();
                                    }
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            //throw ex;
                        }
                    }
                }
            }

            return blnRet;
        }

        /// <summary>
        /// Deserializes the XML document to an object.
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        static object Deserialize(string strContent, Type type)
        {
            object obj = null;
            try
            {
                byte[] array = Encoding.UTF8.GetBytes(strContent);
                MemoryStream stream = new MemoryStream(array);
                StreamReader reader = new StreamReader(stream);
                XmlSerializer xs = new XmlSerializer(type);
                if (type.Name == "Customer")
                {
                    obj = xs.Deserialize(reader) as Customer;
                }
                else if (type.Name == "SalesOrder")
                {
                    obj = xs.Deserialize(reader) as SalesOrder;
                }
                else
                {
                    obj = xs.Deserialize(reader) as List<SalesOrderProductDetails>;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return obj;
        }

        /// <summary>
        /// Saves messages revceived into the specified table.
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="salesOrder"></param>
        /// <param name="lstOrderDetails"></param>
        /// <returns></returns>
        static bool SaveMessage(Customer customer, SalesOrder salesOrder, List<SalesOrderProductDetails> lstOrderDetails)
        {
            bool blnRet = false;
            try
            {
                Customer customerNew = SaveCustomerInfo(customer);
                if (!string.IsNullOrEmpty(customerNew.StrCustomerId))
                {
                    salesOrder.StrCustomerId = customerNew.StrCustomerId;
                   blnRet= SaveFootWearOrderInfo(salesOrder, lstOrderDetails);
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }

            return blnRet;
        }

        /// <summary>
        /// Saves new data of the sales order into the ProductFootwearOrder and ProductFootWearOrderDetails table.
        /// </summary>
        /// <param name="salesOrder"></param>
        /// <param name="lstOrderDetails"></param>
       /// <returns></returns>
        static bool SaveFootWearOrderInfo(SalesOrder salesOrder, List<SalesOrderProductDetails> lstOrderDetails)
        {
            bool blnRet = false;
            try
            {
                string strOrderId = DalClassHelper.InsertProductFootwearOrder(salesOrder, lstOrderDetails);
                if (!string.IsNullOrEmpty(strOrderId))
                {
                    blnRet = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }

            return blnRet;
        }
       
        /// <summary>
        /// Saves new data of the customer into the Customer table.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        static Customer SaveCustomerInfo(Customer customer)
        {
            try
            {
                DataTable dtCustomer = DalClassHelper.GetCustomerByName(customer.StrCustomerName);
                if (dtCustomer != null && dtCustomer.Rows.Count > 0)
                {
                    string strCustomerId = string.Empty;
                    string strCustomerName = string.Empty;
                    string strCustomerAddress = string.Empty;
                    string strCustomerTelephone = string.Empty;
                    bool blnCompareRet = false;
                    if (!(dtCustomer.Rows[0]["CustomerId"] is DBNull))
                    {
                        strCustomerId = dtCustomer.Rows[0]["CustomerId"].ToString().Trim();
                        customer.StrCustomerId = strCustomerId;
                    }
                    if (!(dtCustomer.Rows[0]["CustomerName"] is DBNull))
                    {
                        strCustomerName = dtCustomer.Rows[0]["CustomerName"].ToString().Trim();
                    }
                    if (!(dtCustomer.Rows[0]["CustomerAddress"] is DBNull))
                    {
                        strCustomerAddress = dtCustomer.Rows[0]["CustomerAddress"].ToString().Trim();
                        if (strCustomerAddress != customer.StrCustomerAddress)
                        {
                            blnCompareRet = true;
                        }
                    }
                    if (!(dtCustomer.Rows[0]["CustomerTelephone"] is DBNull))
                    {
                        strCustomerTelephone = dtCustomer.Rows[0]["CustomerTelephone"].ToString().Trim();
                        if (strCustomerTelephone != customer.StrCustomerTelephone)
                        {
                            blnCompareRet = true;
                        }
                    }
                    if (blnCompareRet)
                    {
                        DalClassHelper.UpdateCustomer(customer);
                    }
                }
                else
                {
                    customer.StrCustomerId = DalClassHelper.InsertCustomer(customer);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return customer;
        }



    }
}
