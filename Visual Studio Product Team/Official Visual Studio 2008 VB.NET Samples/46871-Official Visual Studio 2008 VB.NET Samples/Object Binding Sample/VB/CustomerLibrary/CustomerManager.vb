Public Class CustomerManager
    Public Shared Function GetAllCustomers() As Customers
        Dim customerList As New Customers()
        customerList.Add(New Customer("BOTTM", "Bottom-Dollar Markets", "Tsawassen", "BC", "Canada"))
        customerList(0).Orders.Add(New Order(1, "BOTTM", #1/4/2004#, #1/11/2004#))
        customerList(0).Orders.Add(New Order(2, "BOTTM", #2/9/2004#, #2/16/2004#))
        customerList(0).Orders.Add(New Order(3, "BOTTM", #3/2/2004#, #3/7/2004#))

        customerList.Add(New Customer("GOURL", "Gourmet Lanchonetes", "Campinas", "SP", "Brazil"))
        customerList(1).Orders.Add(New Order(4, "GOURL", #4/7/2004#, #4/14/2004#))
        customerList(1).Orders.Add(New Order(5, "GOURL", #5/1/2004#, #5/14/2004#))
        customerList(1).Orders.Add(New Order(6, "GOURL", #6/20/2004#, #6/23/2004#))


        customerList.Add(New Customer("GREAL", "Great Lakes Food Market", "Eugene", "OR", "USA"))
        customerList(2).Orders.Add(New Order(7, "GREAL", #7/7/2004#, #7/14/2004#))
        customerList(2).Orders.Add(New Order(8, "GREAL", #8/1/2004#, #8/14/2004#))
        customerList(2).Orders.Add(New Order(9, "GREAL", #9/20/2004#, #9/23/2004#))

        Return customerList
    End Function

    Public Shared Function GetNewCustomer() As Customer
        Return New Customer("CustID", "CompanyName", "City", "Region", "Country")
    End Function
End Class
