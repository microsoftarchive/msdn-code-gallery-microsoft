
/****************************** Module Header ******************************\
*Module Name: DALClass.cs
*Project:      CSAzureReceiveSalesMessageViaTopic
*Copyright (c) Microsoft Corporation.
* 
*This project provides methods for many common operations on Azure database tables. 
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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSAzureReceiveSalesMessageViaTopic
{
    public class DALClass
    {

        static string strDatabaseConnection;
        static string strTopic;

        public static string StrTopic
        {
            get
            {
                strTopic = "TopicName";
                return strTopic;
            }
            set { strTopic = value; }
        }
        static string strServiceBusConnection;

        public static string StrServiceBusConnect
        {
            get
            {
                strServiceBusConnection = "ConnectionString";
                return strServiceBusConnection;
            }
            set { strServiceBusConnection = value; }
        }

        public static string StrDataBaseConnection
        {
            get
            {
                strDatabaseConnection = "ConnectionString";
                return strDatabaseConnection;
                

            }
        }


        /// <summary>
        /// Gets the data of the customer from the Customer table based on the customer name value. 
        /// </summary>
        /// <param name="strCustomerName"></param>
        /// <returns></returns>
        public DataTable GetCustomerByName(string strCustomerName)
        {
            DataTable dtCustomer = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();
            string strSql = string.Format("select * from customer where CustomerName=N'{0}' ", strCustomerName);
            try
            {
                sqlConnection.ConnectionString = StrDataBaseConnection;
                SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    dtCustomer.Load(sqlDataReader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return dtCustomer;

        }


        /// <summary>
        /// Inserts new data of the customer into the Customer table. 
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public string InsertCustomer(Customer customer)
        {
            string strCustomerId = string.Empty;
            SqlConnection sqlConnection = new SqlConnection();
            string strSql = string.Format("insert into Customer(CustomerName,CustomerAddress, CustomerTelePhone) " +
                                      " values (N'{0}',N'{1}','{2}') SELECT @@IDENTITY AS Id",
                                      customer.StrCustomerName, customer.StrCustomerAddress, customer.StrCustomerTelephone);
            try
            {
                sqlConnection.ConnectionString = StrDataBaseConnection;
                SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        strCustomerId = sqlDataReader["Id"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return strCustomerId;
        }

        /// <summary>
        /// Updates the Customer table based on new data of the customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool UpdateCustomer(Customer customer)
        {
            bool blnRet = false;
            SqlConnection sqlConnection = new SqlConnection();
            string strSql = string.Format(" update Customer set CustomerName=N'{0}',CustomerAddress=N'{1}',CustomerTelePhone='{2}' " +
                                       " where CustomerId={3}",
                                         customer.StrCustomerName, customer.StrCustomerAddress, customer.StrCustomerTelephone,
                                         customer.StrCustomerId);
            try
            {
                sqlConnection.ConnectionString = StrDataBaseConnection;
                SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                sqlConnection.Open();
                int retCount = sqlCommand.ExecuteNonQuery();
                if (retCount > 0)
                {
                    blnRet = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return blnRet;
        }


        /// <summary>
        /// Gets all the data of the salesman from the Salesman table.
        /// </summary>
        /// <returns></returns>
        public DataTable GetSalesMan()
        {
            DataTable dtSalesMan = new DataTable();
            SqlConnection sqlConnection = new SqlConnection();
            string strSql = string.Format("select SalesManId,SalesManName from Salesman ");

            try
            {
                sqlConnection.ConnectionString = StrDataBaseConnection;
                SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    dtSalesMan.Load(sqlDataReader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }

            return dtSalesMan;
        }

        /// <summary>
        /// Gets the data of the product from the Product table based on the productno value.
        /// </summary>
        /// <param name="strProductNo"></param>
        /// <returns></returns>
        public DataTable GetProductByNo(string strProductNo)
        {
            DataTable dtSalesMan = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(StrDataBaseConnection);

            string strSql = string.Format("select * from  Product where ProductNo='{0}'", strProductNo);

            try
            {
                sqlConnection.ConnectionString = StrDataBaseConnection;
                SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    dtSalesMan.Load(sqlDataReader);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }

            return dtSalesMan;
        }


        /// <summary>
        /// Inserts the data of the sales order into the ProductClothesOrder table and the ProductClothesOrderDetails table.
        /// </summary>
        /// <param name="salesOrder"></param>
        /// <param name="lstsalesDetails"></param>
        /// <returns></returns>
        public string InsertProductClothesOrder(SalesOrder salesOrder, List<SalesOrderProductDetails> lstsalesDetails)
        {
            string strId = string.Empty;
            SqlConnection sqlConnection = new SqlConnection(StrDataBaseConnection);

            string strSql = string.Format("insert into ProductClothesOrder(ProductClothesOrderNo,SaleOrderNo,CreatedDate,CustomerId, SalesManId,DeliveryProductDate) " +
                                          " values ('{0}','{1}','{2}','{3}','{4}','{5}')SELECT @@IDENTITY AS Id ",
                                         DateTime.Now.ToString("yyyyMMddHHmmss"), salesOrder.StrSalesOrderNo, DateTime.Now.ToString(), salesOrder.StrCustomerId,
                                          salesOrder.StrSalesManId, salesOrder.StrDeliveryDate);

            sqlConnection.ConnectionString = StrDataBaseConnection;
            SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
            sqlConnection.Open();
            SqlTransaction transaction;
            transaction = sqlConnection.BeginTransaction("SampleTransaction");
            sqlCommand.Transaction = transaction;

            try
            {
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        strId = sqlDataReader["Id"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(strId))
                {
                    foreach (SalesOrderProductDetails salesDetails in lstsalesDetails)
                    {
                        strSql = string.Format("  insert into ProductClothesOrderDetails(ProductClothesOrderId,ProductId,ProductNumber,ProductUnit) " +
                                               " values ('{0}','{1}','{2}',N'{3}')SELECT @@IDENTITY AS Id",
                                                strId, salesDetails.StrProductId,
                                                salesDetails.StrNumber, salesDetails.StrUnit);
                        sqlCommand.CommandText = strSql;
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                strId = string.Empty;
                transaction.Rollback();
            }

            sqlConnection.Close();
            sqlConnection.Dispose();
            return strId;
        }

        /// <summary>
        /// Inserts the data of the sales order into the ProductFootwearOrder table and the ProductFootWearOrderDetails table.
        /// </summary>
        /// <param name="salesOrder"></param>
        /// <param name="lstsalesDetails"></param>
        /// <returns></returns>
        public string InsertProductFootwearOrder(SalesOrder salesOrder, List<SalesOrderProductDetails> lstsalesDetails)
        {
            string strId = string.Empty;
            SqlConnection sqlConnection = new SqlConnection(StrDataBaseConnection);

            string strSql = string.Format("insert into ProductFootwearOrder(ProductFootwearOrderNo,SaleOrderNo,CreatedDate,CustomerId, SalesManId,DeliveryProductDate) " +
                                          " values ('{0}','{1}','{2}','{3}','{4}','{5}')SELECT @@IDENTITY AS Id ",
                                          DateTime.Now.ToString("yyyyMMddHHmmss"), salesOrder.StrSalesOrderNo, DateTime.Now.ToString(), salesOrder.StrCustomerId,
                                          salesOrder.StrSalesManId, salesOrder.StrDeliveryDate);

            sqlConnection.ConnectionString = StrDataBaseConnection;
            SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
            sqlConnection.Open();
            SqlTransaction transaction;
            transaction = sqlConnection.BeginTransaction("SampleTransaction");
            sqlCommand.Transaction = transaction;

            try
            {
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        strId = sqlDataReader["Id"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(strId))
                {
                    foreach (SalesOrderProductDetails salesDetails in lstsalesDetails)
                    {
                        strSql = string.Format("  insert into ProductFootWearOrderDetails(ProductFootWearOrderId,ProductId,ProductNumber,ProductUnit) " +
                                               " values ('{0}','{1}','{2}',N'{3}')SELECT @@IDENTITY AS Id",
                                                strId, salesDetails.StrProductId,
                                                salesDetails.StrNumber, salesDetails.StrUnit);
                        sqlCommand.CommandText = strSql;
                        sqlCommand.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                strId = string.Empty;
                transaction.Rollback();
            }

            sqlConnection.Close();
            sqlConnection.Dispose();
            return strId;
        }

        /// <summary>
        /// Gets the data of production order from the ProductClothesOrder based on that the order created date value.
        /// </summary>
        /// <param name="strFromDate"></param>
        /// <param name="strToDate"></param>
        /// <returns></returns>
        public DataTable GetProductClothesOrder(string strFromDate, string strToDate)
        {
            DataTable dtProductClothesOrder = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(StrDataBaseConnection);

            string strSql = string.Format(" select p.*,c.CustomerName,c.CustomerAddress,s.SalesManName " +
                                          " from ProductClothesOrder p,Customer c,SalesMan s " +
                                          " where p.CustomerId=c.CustomerId and p.SalesManId=s.SalesManId " +
                                          " and  createddate>='{0}' and createddate<='{1}' ", strFromDate, strToDate);
            try
            {
                sqlConnection.ConnectionString = StrDataBaseConnection;
                SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    dtProductClothesOrder.Load(sqlDataReader);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }

            return dtProductClothesOrder;
        }

        /// <summary>
        /// Gets the data of production order details from the ProductClothesOrderDetails based on the ProductClothesOrderId value.
        /// </summary>
        /// <param name="strProductOrderId"></param>
        /// <returns></returns>
        public DataTable GetProductClothesDetailsOrder(string strProductOrderId)
        {
            DataTable dtProductClothesOrder = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(StrDataBaseConnection);

            string strSql = string.Format(" select d.ProductClothesOrderDetailsId,d.ProductClothesOrderId, d.ProductNumber,d.ProductUnit," +
                                          " p.ProductNo,p.ProductName,p.ProductColor,p.ProductSize,p.ProductPrice  " +
                                          " from ProductClothesOrderDetails d,Product p  " +
                                          "  where d.ProductId=p.ProductId and ProductClothesOrderId={0}", strProductOrderId);
            try
            {
                sqlConnection.ConnectionString = StrDataBaseConnection;
                SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    dtProductClothesOrder.Load(sqlDataReader);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }

            return dtProductClothesOrder;
        }

        /// <summary>
        ///  Gets the data of production order from the ProductFootwearOrder table based on the created date value.
        /// </summary>
        /// <param name="strFromDate"></param>
        /// <param name="strToDate"></param>
        /// <returns></returns>
        public DataTable GetProductFootWearOrder(string strFromDate, string strToDate)
        {
            DataTable dtProductClothesOrder = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(StrDataBaseConnection);

            string strSql = string.Format(" select p.*,c.CustomerName,c.CustomerAddress,s.SalesManName " +
                                          " from ProductFootwearOrder p,Customer c,SalesMan s " +
                                          " where p.CustomerId=c.CustomerId and p.SalesManId=s.SalesManId " +
                                          " and  createddate>='{0}' and createddate<='{1}' ", strFromDate, strToDate);
            try
            {
                sqlConnection.ConnectionString = StrDataBaseConnection;
                SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    dtProductClothesOrder.Load(sqlDataReader);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }

            return dtProductClothesOrder;
        }

        /// <summary>
        /// Gets the data of production order details from the ProductFootWearOrderDetails table based on the ProductFootWearOrderId value.
        /// </summary>
        /// <param name="strProductOrderId"></param>
        /// <returns></returns>
        public DataTable GetProductFootWearDetailsOrder(string strProductOrderId)
        {
            DataTable dtProductClothesOrder = new DataTable();
            SqlConnection sqlConnection = new SqlConnection(StrDataBaseConnection);

            string strSql = string.Format(" select d.ProductFootWearOrderDetailsId,d.ProductFootWearOrderId, d.ProductNumber,d.ProductUnit," +
                                          " p.ProductNo,p.ProductName,p.ProductColor,p.ProductSize,p.ProductPrice  " +
                                          " from ProductFootWearOrderDetails d,Product p  " +
                                          "  where d.ProductId=p.ProductId and ProductFootWearOrderId={0}", strProductOrderId);
            try
            {
                sqlConnection.ConnectionString = StrDataBaseConnection;
                SqlCommand sqlCommand = new SqlCommand(strSql, sqlConnection);
                sqlConnection.Open();
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    dtProductClothesOrder.Load(sqlDataReader);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }

            return dtProductClothesOrder;
        }
    }
}
