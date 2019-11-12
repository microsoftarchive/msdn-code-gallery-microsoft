/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSDataTableQuery
Copyright (c) Microsoft Corporation.

We can use traditional SQL queries to get data from the data source, but we 
cannot use SQL to query in DataSet. In this sample, we will demonstrate how 
to use Expression to query in a DataSet.
1. Create a DataSet with two DataTables;
2. Create the constraints between the tables;
3. Use DataTable.Select Method to get rows from the tables;
4. Use DataTable.Compute Method to compute the specified rows;
5. Use Expression in the above methods to query. 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Data;

namespace CSDataTableQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create two tables and add them into the DataSet
            DataTable salesPersonTable = CreateSalesPersonTable();
            DataTable orderTable = CreateOrderTable();
            DataTable orderDetailTable = CreateOrderDetailTable();
            DataSet salesSet = new DataSet();
            salesSet.Tables.Add(salesPersonTable);
            salesSet.Tables.Add(orderTable);
            salesSet.Tables.Add(orderDetailTable);


            // Set the relations between the tables and create the related constraints.
            salesSet.Relations.Add("OrderOrderDetail",
                orderTable.Columns["OrderId"], orderDetailTable.Columns["OrderId"], true);

            salesSet.Relations.Add("SalesPersonOrder", 
                salesPersonTable.Columns["PersonId"], orderTable.Columns["SalesPerson"], true);

            // Insert the rows into the table
            InsertPerson(salesPersonTable);
            InsertOrders(orderTable);
            InsertOrderDetails(orderDetailTable);

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

            DataRow totalRow = orderTable.NewRow();
            totalRow["OrderId"] = "Total";
            orderTable.Rows.Add(totalRow);

            Console.WriteLine("Following is the SalesPerson table.");
            ShowTable(salesPersonTable);
            Console.WriteLine("Following is the Order table.");
            ShowTable(orderTable);
            Console.WriteLine("Following is the OrderDetail table.");
            ShowTable(orderDetailTable);

            String[] territories = { "Europe", "North America"};
            Console.WriteLine("Following is the sales information for every territories.");
            foreach(String territory in territories)
            {
                String expression = String.Format("Parent.Territory='{0}'",
                    territory);
                Object total = orderTable.Compute("Sum(TotalDue)", expression);
                Console.WriteLine("Sales information in {0}(Total:{1:C}):", territory, total);
                DataRow[] territoryRows = orderTable.Select(expression);
                ShowRows(territoryRows);
            }

            Console.WriteLine("Following is the sales information for all the bikes.");
            DataRow[] bikeRows = orderDetailTable.Select("Product like '*Bike'");
            ShowRows(bikeRows);

            Console.WriteLine("Please press any key to exit.....");
            Console.ReadKey();
        }

        private static DataTable CreateSalesPersonTable()
        {
            DataTable salesPersonTable = new DataTable("SalesPerson");


            DataColumn colId = new DataColumn("PersonId", typeof(String));
            salesPersonTable.Columns.Add(colId);

            DataColumn colFirstName = new DataColumn("FirstName", typeof(String));
            salesPersonTable.Columns.Add(colFirstName);

            DataColumn colLastName = new DataColumn("LastName", typeof(String));
            salesPersonTable.Columns.Add(colLastName);

            DataColumn colTerritory = new DataColumn("Territory",typeof(String));
            salesPersonTable.Columns.Add(colTerritory);

            salesPersonTable.PrimaryKey = new DataColumn[] { colId };

            return salesPersonTable;
        }

        private static DataTable CreateOrderTable()
        {
            DataTable orderTable = new DataTable("Order");

            // Define one column once.
            DataColumn colId = new DataColumn("OrderId", typeof(String));
            orderTable.Columns.Add(colId);

            DataColumn colDate = new DataColumn("OrderDate", typeof(DateTime));
            orderTable.Columns.Add(colDate);

            DataColumn colSalesPerson = new DataColumn("SalesPerson", typeof(String));
            orderTable.Columns.Add(colSalesPerson);

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

        private static void InsertPerson(DataTable SalesPersonTable)
        {
            // Use Object array to insert all the rows once.
            // Note that values in the array are matched sequentially to the columns, 
            // based on the order in which they appear in the table.
            Object[] rows = { 
                              new Object[]{"P0001","Rob","Walters","Europe"},
                              new Object[]{"P0005","Mary","Gibson","North America"}
                            };

            foreach (Object[] row in rows)
            {
                SalesPersonTable.Rows.Add(row);
            }
        }

        private static void InsertOrders(DataTable orderTable)
        {
            // Add one row once.
            DataRow row1 = orderTable.NewRow();
            row1["OrderId"] = "O0001";
            row1["OrderDate"] = new DateTime(2013, 3, 1);
            row1["SalesPerson"] = "P0001";
            orderTable.Rows.Add(row1);

            DataRow row2 = orderTable.NewRow();
            row2["OrderId"] = "O0002";
            row2["OrderDate"] = new DateTime(2013, 3, 12);
            row2["SalesPerson"] = "P0005";
            orderTable.Rows.Add(row2);

            DataRow row3 = orderTable.NewRow();
            row3["OrderId"] = "O0003";
            row3["OrderDate"] = new DateTime(2013, 3, 20);
            row3["SalesPerson"] = "P0001";
            orderTable.Rows.Add(row3);
        }

        private static void InsertOrderDetails(DataTable orderDetailTable)
        {
            // Use Object array to insert all the rows once.
            // Note that values in the array are matched sequentially to the columns, 
            // based on the order in which they appear in the table.
            Object[] rows = {
                                 new Object[]{1,"O0001","Mountain Bike",1419,36},
                                 new Object[]{2,"O0001","Road Bike",1233,16},
                                 new Object[]{3,"O0001","Touring Bike",1653,32},
                                 new Object[]{4,"O0002","Mountain Bike",1419,24},
                                 new Object[]{5,"O0002","Road Bike",1233,12},
                                 new Object[]{6,"O0003","Helmet",129.5,56},
                                 new Object[]{7,"O0003","Mountain Bike",1419,48},
                                 new Object[]{8,"O0003","Touring Bike",1653,8}
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

            ShowTableRows(table.Select());
        }

        private static void ShowRows(DataRow[] rows)
        {
            if (rows.Length > 0&&rows[0].Table!=null)
            {
                foreach (DataColumn col in rows[0].Table.Columns)
                {
                    Console.Write("{0,-14}", col.ColumnName);
                }
                Console.WriteLine();

                ShowTableRows(rows);
            }
        }

        private static void ShowTableRows(DataRow[] rows)
        {
            foreach (DataRow row in rows)
            {
                foreach (DataColumn col in row.Table.Columns)
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
