
/****************************** Module Header ******************************\
*Module Name: DALClass.cs
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
using System.Data;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace SalesSendMessagesViaTopic
{
    public class DALClass
    {
        static string strDatabaseConnection;

        public static string StrDataBaseConnection
        {
            get
            {
                strDatabaseConnection = "Server=tcp:pmyfuasi88.database.windows.net,1433;Database=ContosoSales;User ID=tjh@pmyfuasi88;Password=Dfshqi123;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;";
                return DALClass.strDatabaseConnection;
            }
        }

        static string strTopic;
        public static string StrTopic
        {
            get
            {
                strTopic = "topic1";
                return DALClass.strTopic;
            }
            set { DALClass.strTopic = value; }
        }

        static string strServiceBusConnection;
        public static string StrServiceBusConnect
        {
            get
            {
                strServiceBusConnection = "ConnectionString";
                return DALClass.strServiceBusConnection;
            }
            set { DALClass.strServiceBusConnection = value; }
        }

        /// <summary>
        /// Gets the data of customer from the Customer table based on the customer name value. 
        /// </summary>
        /// <param name="strCustomerName"></param>
        /// <returns></returns>
      public DataTable GetCustomerByName(string strCustomerName)
      {
          DataTable dtCustomer = new DataTable();
          SqlConnection sqlConnection=new SqlConnection ();
          string strSql = string.Format("select * from customer where CustomerName='{0}' ", strCustomerName);
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
          catch(Exception ex)
          {
              throw ex;
          }
          finally
          {
              if(sqlConnection.State!=ConnectionState.Closed)
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
                                        " values ('{0}','{1}','{2}') SELECT @@IDENTITY AS Id",
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
        /// Updates the Customer table based on new data of customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
      public bool UpdateCustomer(Customer customer)
      {
          bool blnRet = false;
          SqlConnection sqlConnection = new SqlConnection();
          string strSql = string.Format(" update Customer set CustomerName='{0}',CustomerAddress='{1}',CustomerTelePhone='{2}' " +
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
        ///  Gets all the data of the salesman from the Salesman table.
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
        /// Inserts the data of the  sales order into the SaleOrder table and the SalesOrderDetails table.
        /// </summary>
        /// <param name="salesOrder"></param>
        /// <param name="lstsalesDetails"></param>
        /// <returns></returns>
      public string InsertSalesOrder(SalesOrder salesOrder, List<SalesOrderProductDetails>lstsalesDetails)
      {
          string strCustomerId = string.Empty;
          SqlConnection sqlConnection = new SqlConnection(StrDataBaseConnection);
          
          string strSql = string.Format("insert into SaleOrder(OrderNo,OrderType,CreatedDate,CustomerId, SalesManId,DeliveryProductDate) " +
                                        " values ('{0}','{1}','{2}','{3}','{4}','{5}')SELECT @@IDENTITY AS Id ",
                                        salesOrder.StrSalesOrderNo, salesOrder.StrSalesOrderType, DateTime.Now.ToString(), salesOrder.StrCustomerId,
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
                      strCustomerId = sqlDataReader["Id"].ToString();
                  }
              }
              if (!string.IsNullOrEmpty(strCustomerId))
              {
                  foreach(SalesOrderProductDetails salesDetails in lstsalesDetails )
                  {
                      strSql = string.Format("  insert into SalesOrderDetails(OrderId,ProductId,ProductNumber,ProductUnit) " +
                                             " values ('{0}','{1}','{2}','{3}')SELECT @@IDENTITY AS Id",
                                              strCustomerId, salesDetails.StrProductNo, 
                                              salesDetails.StrNumber,salesDetails.StrUnit);
                      sqlCommand.CommandText = strSql;
                      sqlCommand.ExecuteNonQuery();
                  }
              }
              
              transaction.Commit();
          }
          catch(Exception ex)
          {
              strCustomerId = string.Empty;
              transaction.Rollback();
          }
         
          sqlConnection.Close();
          sqlConnection.Dispose();
          return strCustomerId;
      }



    }
}