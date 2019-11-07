/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSDataTableCreateManually
Copyright (c) Microsoft Corporation.

Sometimes we may need to create the DataTable manually and some specific 
schema definitions, such as ForeignKey constraints, expression columns and 
so on.
In this sample, we will demonstrate how to create the DataTable manually 
with specific schema definitions:
1. Create multiple DataTable and define the initial columns.
2. Create the constraints on the tables.
3. Insert the values and show the tables.
4. Create the expression columns and show the tables.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data;

namespace CSDataTableCreateManually
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create two tables and add them into the DataSet
            DataTable orderTable = CreateOrderTable();
            DataTable orderDetailTable = CreateOrderDetailTable();
            DataSet salesSet = new DataSet();
            salesSet.Tables.Add(orderTable);
            salesSet.Tables.Add(orderDetailTable);

            // Set the relations between the tables and create the related constraint.
            salesSet.Relations.Add("OrderOrderDetail",
                orderTable.Columns["OrderId"], orderDetailTable.Columns["OrderId"], true);

            Console.WriteLine("After create the foreign key constriant," +
          "we will get the following error if inserting order detail with the wrong OrderId: ");
            try
            {
                DataRow errorRow = orderDetailTable.NewRow();
                errorRow[0] = 1;
                errorRow[1] = "O0007";
                orderDetailTable.Rows.Add(errorRow);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine();

            // Insert the rows into the table
            InsertOrders(orderTable);
            InsertOrderDetails(orderDetailTable);

            Console.WriteLine("Following is the initial Order table.");
            ShowTable(orderTable);
            Console.WriteLine("Following is the OrderDetail table.");
            ShowTable(orderDetailTable);

            // Use the Aggregate-Sum on the child table column to get the result.
            DataColumn colSub = new DataColumn("SubTotal", typeof(Decimal), "Sum(Child.LineTotal)");
            orderTable.Columns.Add(colSub);

            // Compute the tax by referencing the SubTotal expression column.
            DataColumn colTax = new DataColumn("Tax", typeof(Decimal), "SubTotal*0.1");
            orderTable.Columns.Add(colTax);

            // If the OrderId is 'Total', compute the due on all orders; or compute the due on this order.
            DataColumn colTotal = new DataColumn("TotalDue", typeof(Decimal),
                "IIF(OrderId='Total',Sum(SubTotal)+Sum(Tax),SubTotal+Tax)");
            orderTable.Columns.Add(colTotal);

            DataRow row = orderTable.NewRow();
            row["OrderId"] = "Total";
            orderTable.Rows.Add(row);

            Console.WriteLine("Following is the Order table with the expression columns.");
            ShowTable(orderTable);

            Console.WriteLine("Please press any key to exit.....");
            Console.ReadKey();
        }

        private static DataTable CreateOrderTable()
        {
            DataTable orderTable = new DataTable("Order");

            // Define one column once.
            DataColumn colId = new DataColumn("OrderId", typeof(String));
            orderTable.Columns.Add(colId);

            DataColumn colDate = new DataColumn("OrderDate", typeof(DateTime));
            orderTable.Columns.Add(colDate);

            // Set the OrderId column as the primary key.
            orderTable.PrimaryKey = new DataColumn[] { colId };

            return orderTable;
        }

        private static DataTable CreateOrderDetailTable()
        {
            DataTable orderDetailTable = new DataTable("OrderDetail");

            // Define all the columns once.
            DataColumn[] cols ={
                                  new DataColumn("OrderDetailId",typeof(Int32)),
                                  new DataColumn("OrderId",typeof(String)),
                                  new DataColumn("Product",typeof(String)),
                                  new DataColumn("UnitPrice",typeof(Decimal)),
                                  new DataColumn("OrderQty",typeof(Int32)),
                                  new DataColumn("LineTotal",typeof(Decimal),"UnitPrice*OrderQty")
                              };
            orderDetailTable.Columns.AddRange(cols);

            orderDetailTable.PrimaryKey = new DataColumn[] { orderDetailTable.Columns["OrderDetailId"] };

            return orderDetailTable;
        }

        private static void InsertOrders(DataTable orderTable)
        {
            // Add one row once.
            DataRow row1 = orderTable.NewRow();
            row1["OrderId"] = "O0001";
            row1["OrderDate"] = new DateTime(2013, 3, 1);
            orderTable.Rows.Add(row1);

            DataRow row2 = orderTable.NewRow();
            row2["OrderId"] = "O0002";
            row2["OrderDate"] = new DateTime(2013, 3, 12);
            orderTable.Rows.Add(row2);

            DataRow row3 = orderTable.NewRow();
            row3["OrderId"] = "O0003";
            row3["OrderDate"] = new DateTime(2013, 3, 20);
            orderTable.Rows.Add(row3);
        }

        private static void InsertOrderDetails(DataTable orderDetailTable)
        {
            // Use Object array to insert all the rows once.
            // Note that values in the array are matched sequentially to the columns, 
            // based on the order in which they appear in the table.
            Object[] rows = {
                                 new Object[]{1,"O0001","Mountain Bike",1419.5,36},
                                 new Object[]{2,"O0001","Road Bike",1233.6,16},
                                 new Object[]{3,"O0001","Touring Bike",1653.3,32},
                                 new Object[]{4,"O0002","Mountain Bike",1419.5,24},
                                 new Object[]{5,"O0002","Road Bike",1233.6,12},
                                 new Object[]{6,"O0003","Mountain Bike",1419.5,48},
                                 new Object[]{7,"O0003","Touring Bike",1653.3,8},
                             };

            foreach (Object[] row in rows)
            {
                orderDetailTable.Rows.Add(row);
            }
        }

        private static void ShowTable(DataTable table)
        {
            foreach (DataColumn col in table.Columns)
            {
                Console.Write("{0,-14}", col.ColumnName);
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType.Equals(typeof(DateTime)))
                    {
                        Console.Write("{0,-14:d}", row[col]);
                    }
                    else if (col.DataType.Equals(typeof(Decimal)))
                    {
                        Console.Write("{0,-14:C}", row[col]);
                    }
                    else
                    {
                        Console.Write("{0,-14}", row[col]);
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
