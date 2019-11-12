'**************************** Module Header ******************************/
' Module Name:  MainModule.vb
' Project:      VBDataTableQuery
' Copyright (c) Microsoft Corporation.
' 
' We can use traditional SQL queries to get data from the data source, but we 
' cannot use SQL to query in DataSet. In this sample, we will demonstrate how 
' to use Expression to query in a DataSet.
' 1. Create a DataSet with two DataTables;
' 2. Create the constraints between the tables;
' 3. Use DataTable.Select Method to get rows from the tables;
' 4. Use DataTable.Compute Method to compute the specified rows;
' 5. Use Expression in the above methods to query. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/


Namespace VBDataTableQuery
    Friend Class MainModule
        Shared Sub Main(ByVal args() As String)
            ' Create two tables and add them into the DataSet
            Dim salesPersonTable As DataTable = CreateSalesPersonTable()
            Dim orderTable As DataTable = CreateOrderTable()
            Dim orderDetailTable As DataTable = CreateOrderDetailTable()
            Dim salesSet As New DataSet()
            salesSet.Tables.Add(salesPersonTable)
            salesSet.Tables.Add(orderTable)
            salesSet.Tables.Add(orderDetailTable)


            ' Set the relations between the tables and create the related constraints.
            salesSet.Relations.Add("OrderOrderDetail", orderTable.Columns("OrderId"),
                                   orderDetailTable.Columns("OrderId"), True)

            salesSet.Relations.Add("SalesPersonOrder", salesPersonTable.Columns("PersonId"),
                                   orderTable.Columns("SalesPerson"), True)

            ' Insert the rows into the table
            InsertPerson(salesPersonTable)
            InsertOrders(orderTable)
            InsertOrderDetails(orderDetailTable)

            ' Use the Aggregate-Sum on the child table column to get the result.
            Dim colSub As New DataColumn("SubTotal", GetType(Decimal), "Sum(Child.LineTotal)")
            orderTable.Columns.Add(colSub)

            ' Compute the tax by referencing the SubTotal expression column.
            Dim colTax As New DataColumn("Tax", GetType(Decimal), "SubTotal*0.1")
            orderTable.Columns.Add(colTax)

            ' If the OrderId is 'Total', compute the due on all orders; or compute the due on this order.
            Dim colTotal As New DataColumn("TotalDue", GetType(Decimal),
                                           "IIF(OrderId='Total',Sum(SubTotal)+Sum(Tax),SubTotal+Tax)")
            orderTable.Columns.Add(colTotal)

            Dim totalRow As DataRow = orderTable.NewRow()
            totalRow("OrderId") = "Total"
            orderTable.Rows.Add(totalRow)

            Console.WriteLine("Following is the SalesPerson table.")
            ShowTable(salesPersonTable)
            Console.WriteLine("Following is the Order table.")
            ShowTable(orderTable)
            Console.WriteLine("Following is the OrderDetail table.")
            ShowTable(orderDetailTable)

            Dim territories() As String = {"Europe", "North America"}
            Console.WriteLine("Following is the sales information for every territories.")
            For Each territory As String In territories
                Dim expression As String = String.Format("Parent.Territory='{0}'", territory)
                Dim total As Object = orderTable.Compute("Sum(TotalDue)", expression)
                Console.WriteLine("Sales information in {0}(Total:{1:C}):", territory, total)
                Dim territoryRows() As DataRow = orderTable.Select(expression)
                ShowRows(territoryRows)
            Next territory

            Console.WriteLine("Following is the sales information for all the bikes.")
            Dim bikeRows() As DataRow = orderDetailTable.Select("Product like '*Bike'")
            ShowRows(bikeRows)

            Console.WriteLine("Please press any key to exit.....")
            Console.ReadKey()
        End Sub

        Private Shared Function CreateSalesPersonTable() As DataTable
            Dim salesPersonTable As New DataTable("SalesPerson")


            Dim colId As New DataColumn("PersonId", GetType(String))
            salesPersonTable.Columns.Add(colId)

            Dim colFirstName As New DataColumn("FirstName", GetType(String))
            salesPersonTable.Columns.Add(colFirstName)

            Dim colLastName As New DataColumn("LastName", GetType(String))
            salesPersonTable.Columns.Add(colLastName)

            Dim colTerritory As New DataColumn("Territory", GetType(String))
            salesPersonTable.Columns.Add(colTerritory)

            salesPersonTable.PrimaryKey = New DataColumn() {colId}

            Return salesPersonTable
        End Function

        Private Shared Function CreateOrderTable() As DataTable
            Dim orderTable As New DataTable("Order")

            ' Define one column once.
            Dim colId As New DataColumn("OrderId", GetType(String))
            orderTable.Columns.Add(colId)

            Dim colDate As New DataColumn("OrderDate", GetType(Date))
            orderTable.Columns.Add(colDate)

            Dim colSalesPerson As New DataColumn("SalesPerson", GetType(String))
            orderTable.Columns.Add(colSalesPerson)

            ' Set the OrderId column as the primary key.
            orderTable.PrimaryKey = New DataColumn() {colId}

            Return orderTable
        End Function

        Private Shared Function CreateOrderDetailTable() As DataTable
            Dim orderDetailTable As New DataTable("OrderDetail")

            ' Define all the columns once.
            Dim cols() As DataColumn = {New DataColumn("OrderDetailId", GetType(Int32)),
                                        New DataColumn("OrderId", GetType(String)),
                                        New DataColumn("Product", GetType(String)),
                                        New DataColumn("UnitPrice", GetType(Decimal)),
                                        New DataColumn("OrderQty", GetType(Int32)),
                                        New DataColumn("LineTotal", GetType(Decimal),
                                                       "UnitPrice*OrderQty")}
            orderDetailTable.Columns.AddRange(cols)

            orderDetailTable.PrimaryKey = New DataColumn() {orderDetailTable.Columns("OrderDetailId")}

            Return orderDetailTable
        End Function

        Private Shared Sub InsertPerson(ByVal SalesPersonTable As DataTable)
            ' Use Object array to insert all the rows once.
            ' Note that values in the array are matched sequentially to the columns, 
            ' based on the order in which they appear in the table.
            Dim rows() As Object = {New Object() {"P0001", "Rob", "Walters", "Europe"},
                                    New Object() {"P0005", "Mary", "Gibson", "North America"}}

            For Each row As Object() In rows
                SalesPersonTable.Rows.Add(row)
            Next row
        End Sub

        Private Shared Sub InsertOrders(ByVal orderTable As DataTable)
            ' Add one row once.
            Dim row1 As DataRow = orderTable.NewRow()
            row1("OrderId") = "O0001"
            row1("OrderDate") = New Date(2013, 3, 1)
            row1("SalesPerson") = "P0001"
            orderTable.Rows.Add(row1)

            Dim row2 As DataRow = orderTable.NewRow()
            row2("OrderId") = "O0002"
            row2("OrderDate") = New Date(2013, 3, 12)
            row2("SalesPerson") = "P0005"
            orderTable.Rows.Add(row2)

            Dim row3 As DataRow = orderTable.NewRow()
            row3("OrderId") = "O0003"
            row3("OrderDate") = New Date(2013, 3, 20)
            row3("SalesPerson") = "P0001"
            orderTable.Rows.Add(row3)
        End Sub

        Private Shared Sub InsertOrderDetails(ByVal orderDetailTable As DataTable)
            ' Use Object array to insert all the rows once.
            ' Note that values in the array are matched sequentially to the columns, 
            ' based on the order in which they appear in the table.
            Dim rows() As Object = {New Object() {1, "O0001", "Mountain Bike", 1419, 36},
                                    New Object() {2, "O0001", "Road Bike", 1233, 16},
                                    New Object() {3, "O0001", "Touring Bike", 1653, 32},
                                    New Object() {4, "O0002", "Mountain Bike", 1419, 24},
                                    New Object() {5, "O0002", "Road Bike", 1233, 12},
                                    New Object() {6, "O0003", "Helmet", 129.5, 56},
                                    New Object() {7, "O0003", "Mountain Bike", 1419, 48},
                                    New Object() {8, "O0003", "Touring Bike", 1653, 8}}

            For Each row As Object() In rows
                orderDetailTable.Rows.Add(row)
            Next row
        End Sub

        Private Shared Sub ShowTable(ByVal table As DataTable)
            For Each col As DataColumn In table.Columns
                Console.Write("{0,-14}", col.ColumnName)
            Next col
            Console.WriteLine()

            ShowTableRows(table.Select())
        End Sub

        Private Shared Sub ShowRows(ByVal rows() As DataRow)
            If rows.Length > 0 AndAlso rows(0).Table IsNot Nothing Then
                For Each col As DataColumn In rows(0).Table.Columns
                    Console.Write("{0,-14}", col.ColumnName)
                Next col
                Console.WriteLine()

                ShowTableRows(rows)
            End If
        End Sub

        Private Shared Sub ShowTableRows(ByVal rows() As DataRow)
            For Each row As DataRow In rows
                For Each col As DataColumn In row.Table.Columns
                    If col.DataType.Equals(GetType(Date)) Then
                        Console.Write("{0,-14:d}", row(col))
                    ElseIf col.DataType.Equals(GetType(Decimal)) Then
                        Console.Write("{0,-14:C}", row(col))
                    Else
                        Console.Write("{0,-14}", row(col))
                    End If
                Next col
                Console.WriteLine()
            Next row
            Console.WriteLine()
        End Sub
    End Class
End Namespace
