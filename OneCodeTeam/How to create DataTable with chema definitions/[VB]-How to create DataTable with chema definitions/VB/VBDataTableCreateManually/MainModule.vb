'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBDataTableCreateManually
' Copyright (c) Microsoft Corporation.
' 
' Sometimes we may need to create the DataTable manually and some specific 
' schema definitions, such as ForeignKey constraints, expression columns and 
' so on.
' In this sample, we will demonstrate how to create the DataTable manually 
' with specific schema definitions:
' 1. Create multiple DataTable and define the initial columns.
' 2. Create the constraints on the tables.
' 3. Insert the values and show the tables.
' 4. Create the expression columns and show the tables.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/


Namespace VBDataTableCreateManually
    Friend Class MainModule
        Shared Sub Main(ByVal args() As String)
            ' Create two tables and add them into the DataSet
            Dim orderTable As DataTable = CreateOrderTable()
            Dim orderDetailTable As DataTable = CreateOrderDetailTable()
            Dim salesSet As New DataSet()
            salesSet.Tables.Add(orderTable)
            salesSet.Tables.Add(orderDetailTable)

            ' Set the relations between the tables and create the related constraint.
            salesSet.Relations.Add("OrderOrderDetail", orderTable.Columns("OrderId"),
                                   orderDetailTable.Columns("OrderId"), True)

            Console.WriteLine("After create the foreign key constriant," &
                "we will get the following error if inserting order detail with the wrong OrderId: ")
            Try
                Dim errorRow As DataRow = orderDetailTable.NewRow()
                errorRow(0) = 1
                errorRow(1) = "O0007"
                orderDetailTable.Rows.Add(errorRow)
            Catch e As Exception
                Console.WriteLine(e.Message)
            End Try
            Console.WriteLine()

            ' Insert the rows into the table
            InsertOrders(orderTable)
            InsertOrderDetails(orderDetailTable)

            Console.WriteLine("Following is the initial Order table.")
            ShowTable(orderTable)
            Console.WriteLine("Following is the OrderDetail table.")
            ShowTable(orderDetailTable)

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

            Dim row As DataRow = orderTable.NewRow()
            row("OrderId") = "Total"
            orderTable.Rows.Add(row)

            Console.WriteLine("Following is the Order table with the expression columns.")
            ShowTable(orderTable)

            Console.WriteLine("Please press any key to exit.....")
            Console.ReadKey()
        End Sub

        Private Shared Function CreateOrderTable() As DataTable
            Dim orderTable As New DataTable("Order")

            ' Define one column once.
            Dim colId As New DataColumn("OrderId", GetType(String))
            orderTable.Columns.Add(colId)

            Dim colDate As New DataColumn("OrderDate", GetType(Date))
            orderTable.Columns.Add(colDate)

            ' Set the OrderId column as the primary key.
            orderTable.PrimaryKey = New DataColumn() {colId}

            Return orderTable
        End Function

        Private Shared Function CreateOrderDetailTable() As DataTable
            Dim orderDetailTable As New DataTable("OrderDetail")

            ' Define all the columns once.
            Dim cols() As DataColumn = {
                                        New DataColumn("OrderDetailId", GetType(Int32)),
                                        New DataColumn("OrderId", GetType(String)),
                                        New DataColumn("Product", GetType(String)),
                                        New DataColumn("UnitPrice", GetType(Decimal)),
                                        New DataColumn("OrderQty", GetType(Int32)),
                                        New DataColumn("LineTotal", GetType(Decimal),
                                                       "UnitPrice*OrderQty")
                                    }
            orderDetailTable.Columns.AddRange(cols)

            orderDetailTable.PrimaryKey = New DataColumn() {orderDetailTable.Columns("OrderDetailId")}

            Return orderDetailTable
        End Function

        Private Shared Sub InsertOrders(ByVal orderTable As DataTable)
            ' Add one row once.
            Dim row1 As DataRow = orderTable.NewRow()
            row1("OrderId") = "O0001"
            row1("OrderDate") = New Date(2013, 3, 1)
            orderTable.Rows.Add(row1)

            Dim row2 As DataRow = orderTable.NewRow()
            row2("OrderId") = "O0002"
            row2("OrderDate") = New Date(2013, 3, 12)
            orderTable.Rows.Add(row2)

            Dim row3 As DataRow = orderTable.NewRow()
            row3("OrderId") = "O0003"
            row3("OrderDate") = New Date(2013, 3, 20)
            orderTable.Rows.Add(row3)
        End Sub

        Private Shared Sub InsertOrderDetails(ByVal orderDetailTable As DataTable)
            ' Use Object array to insert all the rows once.
            ' Note that values in the array are matched sequentially to the columns, 
            ' based on the order in which they appear in the table.
            Dim rows() As Object = {
                                    New Object() {1, "O0001", "Mountain Bike", 1419.5, 36},
                                    New Object() {2, "O0001", "Road Bike", 1233.6, 16},
                                    New Object() {3, "O0001", "Touring Bike", 1653.3, 32},
                                    New Object() {4, "O0002", "Mountain Bike", 1419.5, 24},
                                    New Object() {5, "O0002", "Road Bike", 1233.6, 12},
                                    New Object() {6, "O0003", "Mountain Bike", 1419.5, 48},
                                    New Object() {7, "O0003", "Touring Bike", 1653.3, 8}
                                   }

            For Each row As Object() In rows
                orderDetailTable.Rows.Add(row)
            Next row
        End Sub

        Private Shared Sub ShowTable(ByVal table As DataTable)
            For Each col As DataColumn In table.Columns
                Console.Write("{0,-14}", col.ColumnName)
            Next col
            Console.WriteLine()

            For Each row As DataRow In table.Rows
                For Each col As DataColumn In table.Columns
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
