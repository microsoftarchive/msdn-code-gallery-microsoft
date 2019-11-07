
/****************************** Module Header ******************************\
*Module Name:  BuildSalesOrder.aspx.cs
*Project:      SalesSendMessagesViaTopic
*Copyright (c) Microsoft Corporation.
* 
*In contrast to queues, in which each message is processed by a single consumer,
*topics and subscriptions provide a one-to-many form of communication, in a publish/subscribe pattern.
*
*This sample show that the Sales department is responsible for verifying orders placed by customers, 
*keeping record of the orders and then sending the order information to Production department to make sure that the goods be delivered in time.  

*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
* 
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;

namespace SalesSendMessagesViaTopic
{
    public partial class BuildSalesOrder : System.Web.UI.Page
    {
        static object lockObj = new object();
        string strTopic = DALClass.StrTopic;
        string strServiceBusConnect = DALClass.StrServiceBusConnect;
        DALClass dalClassHelper;

        public DALClass DalClassHelper
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



        protected void Page_Load(object sender, EventArgs e)
        {
            txt_SalesOrderNo.Enabled = false;
            if(!IsPostBack)
            {
                txt_SalesOrderNo.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
                txt_SalesOrderNo.Enabled = false;
                BindSalesman();
            }
            else
            {
                if (Session["RowStatus"] != null)
                {
                    string strRowStatus = Session["RowStatus"].ToString();
                    if(strRowStatus.Length>0)
                    {
                        string[] arrStatus = strRowStatus.Split('_');
                        if(arrStatus.Length>0)
                        {
                            int statusCount = 1;
                            for(int i=1;i<tbl_Production.Rows.Count;i++)
                            {
                                tbl_Production.Rows[i].Style["display"] = arrStatus[statusCount];
                                statusCount++;
                            }

                        }

                    }
                }

            }

        }


        /// <summary>
        ///  Binds the data of the salesman to DropDownList control.
        /// </summary>
        private void BindSalesman()
        {
            dpd_SalesMan.Items.Clear();
            DALClass dalClass = new DALClass();
            DataTable dtSalesMan = dalClass.GetSalesMan();
            if (dtSalesMan != null && dtSalesMan.Rows.Count > 0)
            {
                dpd_SalesMan.DataSource = dtSalesMan;
                dpd_SalesMan.DataValueField = "SalesManId";
                dpd_SalesMan.DataTextField = "SalesManName";
                dpd_SalesMan.DataBind();
            }
        }


        /// <summary>
        /// Saves the data of the sales order into database and then sends the data using Azure Service Bus Topic.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Ok_Click(object sender, EventArgs e)
        {
            SalesOrder salesOrder = new SalesOrder();
            List<SalesOrderProductDetails> lstDetails = new List<SalesOrderProductDetails>();
            string strSalesManId = string.Empty;

            try
            {
                Customer customer = GetCustomerInfo();
                if (dpd_SalesMan.SelectedValue != null)
                {
                    strSalesManId = dpd_SalesMan.SelectedValue.ToString();
                }

                salesOrder.StrSalesOrderNo = txt_SalesOrderNo.Text.Trim();
                salesOrder.StrCustomerId = customer.StrCustomerId;
                salesOrder.StrSalesManId = strSalesManId;
                salesOrder.StrDeliveryDate = txt_DeliveryDate.Text.Trim();
                salesOrder.StrSalesOrderType = dpd_SalesOrderType.Text.Trim();

                lstDetails = GetProductInfo();
                bool blnRet = SaveOrderInfo(salesOrder, lstDetails);
                if (blnRet)
                {
                    blnRet = SendOrderInfoViaTopic(customer, salesOrder, lstDetails);
                    if(blnRet)
                    {
                        Response.Write("<script>alert('" + "Messages are sent successfully! " + "');</script>"); 
                        ClearInPutInfo();
                    }
                }
            }
            catch(Exception ex)
            {
                Response.Write("<script>alert('Error Message:" + ex.Message.ToString() + "');</script>");
            }
           
        }

        /// <summary>
        /// Clears all values input.
        /// </summary>
        private void ClearInPutInfo()
        {
            txt_SalesOrderNo.Text = DateTime.Now.ToString("yyyyMMddHHmmss");
            txt_CustomerName.Text = "";
            txt_DeliveryDate.Text = "";
            txt_ShippingAddress.Text = "";
            txt_TelePhone.Text = "";
            int intcount = tbl_Production.Rows.Count;
            for (int i = 2; i < intcount; i++)
            {
                for (int col = 2; col < tbl_Production.Rows[i].Cells.Count; col++)
                {
                    if (tbl_Production.Rows[i].Cells[col].Controls.Count > 0)
                    {
                        if (tbl_Production.Rows[i].Cells[col].Controls[0] is TextBox)
                        {
                            TextBox txt = (TextBox)tbl_Production.Rows[i].Cells[col].Controls[0];
                            txt.Text = "";
                        }
                    }
                }
            }


        }


        /// <summary>
        /// Saves the data of the sales order into the database.
        /// </summary>
        /// <param name="salesOrder"></param>
        /// <param name="lstDetails"></param>
        /// <returns></returns>
        private bool SaveOrderInfo(SalesOrder salesOrder, List<SalesOrderProductDetails> lstDetails)
        {
            bool blnRet = false;
           try
           {
               if(lstDetails.Count>0)
               {
                   string strSalesOrderId = DalClassHelper.InsertSalesOrder(salesOrder, lstDetails);
                   if (!string.IsNullOrEmpty(strSalesOrderId))
                   {
                       blnRet = true;

                   }
               }
           }
            catch(Exception ex)
           {
               throw ex;
           }
           
            return blnRet;
        }


        /// <summary>
        /// Sends the data of the sales order using Azure Service Bus Topic.
        /// </summary>
        /// <param name="customer"></param>
        /// <param name="salesOrder"></param>
        /// <param name="lstDetails"></param>
        /// <returns></returns>
        private bool SendOrderInfoViaTopic( Customer customer,SalesOrder salesOrder, List<SalesOrderProductDetails> lstDetails)
        {
            bool blnRet = false;

            try
            {
                if(!string.IsNullOrEmpty(strServiceBusConnect)&&!string.IsNullOrEmpty(strTopic))
                {
                    blnRet=CreateSubscription(salesOrder.StrSalesOrderType);
                    if(blnRet)
                    {
                        blnRet = false;
                        TopicClient topiclient = TopicClient.CreateFromConnectionString(strServiceBusConnect, strTopic);
                        BrokeredMessage message = new BrokeredMessage(" Message Date: " + DateTime.Now.ToString());
                        message.Properties.Add("Type", salesOrder.StrSalesOrderType);
                        string strCustomerContent = SerializeClass(typeof(Customer), customer);
                        string stSalesOrderContent = SerializeClass(typeof(SalesOrder), salesOrder);
                        string stSalesOrderDetailsContent = SerializeClass(typeof(List<SalesOrderProductDetails>), lstDetails);
                        message.Properties.Add("Customer", strCustomerContent);
                        message.Properties.Add("SalesOrder", stSalesOrderContent);
                        message.Properties.Add("SalesOrderDetails", stSalesOrderDetailsContent);
                        topiclient.Send(message);
                        blnRet = true;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return blnRet;
        }



        /// <summary>
        ///  Serializes the object to an XML.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private string SerializeClass(Type type, object obj)
        {
            string strContent = string.Empty;
            string strFilePath = AppDomain.CurrentDomain.BaseDirectory + "XMLFile";
            XmlSerializer xs = new XmlSerializer(type);
            try
            {
                lock (lockObj)
                {
                    if (!Directory.Exists(strFilePath))
                    {
                        Directory.CreateDirectory(strFilePath);
                    }
                    strFilePath = strFilePath + "\\Serialize+" + type.Name + ".xml";

                    if (File.Exists(strFilePath))
                    {
                        File.Delete(strFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                using (FileStream stream = new FileStream(strFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    xs.Serialize(stream, obj);
                    stream.Close();                 
                }
                using (StreamReader sReader = new StreamReader(strFilePath, Encoding.GetEncoding("GB2312")))
                {
                    strContent = sReader.ReadToEnd();
                }
                lock (lockObj)
                {
                    if (File.Exists(strFilePath))
                    {
                        File.Delete(strFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strContent;
        }

        /// <summary>
        /// Creates the Subscription based on the SalesOrderType value
        /// </summary>
        /// <param name="StrSalesOrderType"></param>
        /// <returns></returns>
        private bool CreateSubscription(string StrSalesOrderType)
        {
            bool blnRet = false;
            NamespaceManager namespaceManager = NamespaceManager.CreateFromConnectionString(strServiceBusConnect);
            IEnumerable<SubscriptionDescription> lstSubcription = namespaceManager.GetSubscriptions(strTopic);
            foreach (SubscriptionDescription sub in lstSubcription)
            {
                IEnumerable<RuleDescription> lstRules = namespaceManager.GetRules(strTopic, sub.Name);
                foreach (RuleDescription rule in lstRules)
                {
                  string strFilter=  rule.Filter.ToString();
                  if (strFilter.Contains("Type='" + StrSalesOrderType+"'"))
                    {
                        blnRet = true;
                    }
                }
            }
            if(blnRet==false)
            {
                try
                {
                    SubscriptionDescription sub = new SubscriptionDescription(strTopic, "sub_" + StrSalesOrderType);
                    sub.MaxDeliveryCount = 100;
                    SqlFilter filter = new SqlFilter("Type='" + StrSalesOrderType + "'");
                    namespaceManager.CreateSubscription(sub, filter);
                    blnRet = true;
                }
                catch(Exception ex)
                {
                    throw ex;
                }              
            }
            return blnRet;
        }

        /// <summary>
        /// Gets the data of the customer input.
        /// </summary>
        /// <returns></returns>
        private Customer GetCustomerInfo()
        {
            Customer customer = new Customer();
            try
            {
                customer.StrCustomerName = txt_CustomerName.Text.Trim();
                customer.StrCustomerAddress = txt_ShippingAddress.Text.Trim();
                customer.StrCustomerTelephone = txt_TelePhone.Text.Trim();
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
            catch(Exception ex)
            {
                throw ex;
            }
           

            return customer;
        }



        /// <summary>
        /// Gets the all data of the product input
        /// </summary>
        /// <returns></returns>
        private List<SalesOrderProductDetails> GetProductInfo()
        {
            List<SalesOrderProductDetails> lstDetails = new List<SalesOrderProductDetails>();
            try
            {
                int intcount = tbl_Production.Rows.Count;
                for (int i = 2; i < intcount; i++)
                {
                    SalesOrderProductDetails orderDetails = new SalesOrderProductDetails();
                    for (int col = 2; col < tbl_Production.Rows[i].Cells.Count; col++)
                    {
                        if (tbl_Production.Rows[i].Cells[col].Controls.Count > 0)
                        {
                            if (tbl_Production.Rows[i].Cells[col].Controls[0] is TextBox)
                            {
                                TextBox txt = (TextBox)tbl_Production.Rows[i].Cells[col].Controls[0];
                                string strID = txt.ID.ToString();
                                if (strID.Contains("ProductId"))
                                {
                                    orderDetails.StrProductId = txt.Text.Trim();
                                }
                                else if (strID.Contains("Number"))
                                {
                                    orderDetails.StrNumber = txt.Text.Trim();
                                }
                                else if (strID.Contains("Unit"))
                                {
                                    orderDetails.StrUnit = txt.Text.Trim();
                                }


                            }
                        }
                    }
                    if (!lstDetails.Contains(orderDetails) && !string.IsNullOrEmpty(orderDetails.StrProductId))
                    {
                        lstDetails.Add(orderDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstDetails;
        }





        protected void ImageButton_Click(object sender, EventArgs eventArgs)
        {
            calendar.Visible = !calendar.Visible;
        }

        
        protected void RequestedDeliveryDateCalendar_SelectionChanged(object sender, EventArgs eventArgs)
        {
            txt_DeliveryDate.Text = requestedDeliveryDateCalendar.SelectedDate.ToShortDateString();
            calendar.Visible = false;
            txt_DeliveryDate.Focus();
        }


    
    }
}