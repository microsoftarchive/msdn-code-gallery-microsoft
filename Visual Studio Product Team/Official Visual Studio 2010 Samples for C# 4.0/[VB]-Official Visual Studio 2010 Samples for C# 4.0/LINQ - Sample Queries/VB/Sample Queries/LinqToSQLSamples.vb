' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
' Copyright (c) Microsoft Corporation.  All rights reserved.
Option Infer On
Option Strict On

Imports SampleQueries.SampleSupport
Imports SampleQueries.NorthwindInheritance
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.Linq
Imports System.Data.Linq.Mapping
Imports System.Data.Linq.SqlClient
Imports System.Data.SqlClient
Imports System.IO
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Transactions
Imports System.Windows.Forms

<Title("LINQ to SQL Samples")> <Prefix("LinqToSql")> _
Public Class LinqToSQLSamples
    Inherits SampleHarness
    'The connection string is stored in the Sample Queries application settings. To open, Double-click "My Project" in the Solution Explorer for the Sample Queries project. 
    'The default connect string for the samples should work without modification if you have Sql Server Express installed on your development machine. There are, however, a few cases in which trouble might occur.
    '	On some SQL Express installations, you may not have the rights to start a user instance. If you get an error message to that effect, try removing the clause ";user instance = true" from the connection string.
    '	Be sure that the copies of the northwind database that you are accessing are not marked read only. If necessary, browse to the copy of NORTHWIND.MDF you are trying to access, right click on the file, chose properties. Clear the Read-only attribute.
    '	On some slow machines, or when using a virtual pc, users have reported that some database applications give errors the first time they run. Try running the sample two or three times.
    '	If you are using SQL Server instead of SQL Server Express, you will need to change the connect string. Here is an alternate connect string that you can modify for your own purposes if you are not using Sql Express but have Sql Server available.

    '   Server=[SERVER_NAME];Database=northwind;user id=[USER_NAME];password=[PASSWORD]

    '   You will probably need to modify the words SERVER_NAME, USER_NAME, PASSWORD wherever it appears in the sample connect string.  For example, your connect string might look like this:

    '  Server=MyServer;Database=northwind;user id=sa;password=MyPassword"

    Private db As NorthwindDataContext
    Private newDB As NorthwindInheritance



    <Category("Your First LINQ Query")> _
    <Title("Select 2 columns")> _
    <Description("This sample selects 2 columns and returns the data from the database.")> _
    Public Sub LinqToSqlFirst01()

        'Instead of returning the entire Customers table, just return the
        'CompanyName and Country
        Dim londonCustomers = From cust In db.Customers _
                              Select cust.CompanyName, cust.Country

        'Execute the query and print out the results
        For Each custRow In londonCustomers
            Console.WriteLine("Company: " & custRow.CompanyName & vbTab & _
                              "Country: " & custRow.Country)
        Next
    End Sub

    <Category("Your First LINQ Query")> _
    <Title("Simple Filtering")> _
    <Description("This sample uses a Where clause to filter for Customers in London.")> _
    Public Sub LinqToSqlFirst02()

        'Only return customers from London
        Dim londonCustomers = From cust In db.Customers _
                              Where cust.City = "London" _
                              Select cust.CompanyName, cust.City, cust.Country

        'Execute the query and print out the results
        For Each custRow In londonCustomers
            Console.WriteLine("Company: " & custRow.CompanyName & vbTab & _
                              "City: " & custRow.City & vbTab & _
                              "Country: " & custRow.Country)
        Next
    End Sub


    <Category("WHERE")> _
    <Title("Where - 1")> _
    <Description("This sample uses a Where clause to filter for Employees hired " & _
                 "during or after 1994.")> _
    Public Sub LinqToSqlWhere01()
        Dim hiredAfter1994 = From emp In db.Employees _
                             Where emp.HireDate >= #1/1/1994# _
                             Select emp

        ObjectDumper.Write(hiredAfter1994)
    End Sub

    <Category("WHERE")> _
    <Title("Where - 2")> _
    <Description("This sample uses a Where clause to filter for Products that have stock below their " & _
                 "reorder level and are not discontinued.")> _
    Public Sub LinqToSqlWhere02()
        Dim needToOrder = From prod In db.Products _
                          Where prod.UnitsInStock <= prod.ReorderLevel _
                                AndAlso Not prod.Discontinued _
                          Select prod

        ObjectDumper.Write(needToOrder)
    End Sub

    <Category("WHERE")> _
    <Title("Where - 3")> _
    <Description("This sample uses a Where clause to filter out Products that are either " & _
                 "discontinued or that have a UnitPrice greater than 10.")> _
    Public Sub LinqToSqlWhere03()
        Dim prodQuery = From prod In db.Products _
                        Where prod.UnitPrice > 10.0# OrElse prod.Discontinued

        ObjectDumper.Write(prodQuery, 0)
    End Sub

    <Category("WHERE")> _
    <Title("Where - 4")> _
    <Description("This sample uses two Where clauses to filter out Products that are discontinued " & _
                 "and with UnitPrice greater than 10")> _
    Public Sub LinqToSqlWhere04()

        Dim prodQuery = From prod In db.Products _
                        Where prod.UnitPrice > 10D _
                        Where prod.Discontinued

        ObjectDumper.Write(prodQuery, 0)
    End Sub


    <Category("WHERE")> _
    <Title("First - Simple")> _
    <Description("This sample uses First to select the first Shipper in the table.")> _
    Public Sub LinqToSqlWhere05()
        Dim shipper = db.Shippers.First()

        ObjectDumper.Write(shipper, 0)
    End Sub


    <Category("WHERE")> _
    <Title("First - Element")> _
    <Description("This sample uses Take to select the first Customer with CustomerID 'BONAP'.")> _
    Public Sub LinqToSqlWhere06()
        Dim customer = From cust In db.Customers _
                       Where cust.CustomerID = "BONAP" _
                       Take 1

        ObjectDumper.Write(customer, 0)
    End Sub

    <Category("WHERE")> _
    <Title("First - Condition")> _
    <Description("This sample uses First to select an Order with freight greater than 10.00.")> _
    Public Sub LinqToSqlWhere07()
        Dim firstOrd = (From ord In db.Orders _
                        Where ord.Freight > 10D _
                        Select ord).First()

        ObjectDumper.Write(firstOrd, 0)
    End Sub

    <Category("SELECT/DISTINCT")> _
    <Title("Select - Simple")> _
    <Description("This sample uses Select to return a sequence of just the " & _
                 "Customers' contact names.")> _
    Public Sub LinqToSqlSelect01()
        Dim contactList = From cust In db.Customers _
                          Select cust.ContactName

        ObjectDumper.Write(contactList)
    End Sub

    <Category("SELECT/DISTINCT")> _
    <Title("Select - Anonymous Type 1")> _
    <Description("This sample uses Select and anonymous types to return " & _
                 "a sequence of just the Customers' contact names and phone numbers.")> _
    Public Sub LinqToSqlSelect02()
        Dim nameAndNumber = From cust In db.Customers _
                            Select cust.ContactName, cust.Phone

        ObjectDumper.Write(nameAndNumber)
    End Sub


    <Category("SELECT/DISTINCT")> _
    <Title("Select - Anonymous Type 2")> _
    <Description("This sample uses Select and anonymous types to return " & _
                 "a sequence of just the Employees' names and phone numbers, " & _
                 "with the FirstName and LastName fields combined into a single field, 'Name', " & _
                 "and the HomePhone field renamed to Phone in the resulting sequence.")> _
    Public Sub LinqToSqlSelect03()
        Dim nameAndNumber = From emp In db.Employees _
                            Select Name = emp.FirstName & " " & emp.LastName, _
                                   Phone = emp.HomePhone

        ObjectDumper.Write(nameAndNumber, 1)
    End Sub

    <Category("SELECT/DISTINCT")> _
    <Title("Select - Anonymous Type 3")> _
    <Description("This sample uses Select and anonymous types to return " & _
                 "a sequence of all Products' IDs and a calculated value " & _
                 "called HalfPrice which is set to the Product's UnitPrice " & _
                 "divided by 2.")> _
    Public Sub LinqToSqlSelect04()
        Dim prices = From prod In db.Products _
                     Select prod.ProductID, HalfPrice = prod.UnitPrice / 2

        ObjectDumper.Write(prices, 1)
    End Sub

    <Category("SELECT/DISTINCT")> _
    <Title("Select - Conditional ")> _
    <Description("This sample uses Select and a conditional statment to return a sequence of product " & _
                 " name and product availability.")> _
    Public Sub LinqToSqlSelect05()
        Dim inStock = From prod In db.Products _
                      Select prod.ProductName, _
                             Availability = If((prod.UnitsInStock - prod.UnitsOnOrder) < 0, _
                                               "Out Of Stock", _
                                               "In Stock")
        ObjectDumper.Write(inStock, 1)
    End Sub

    <Category("SELECT/DISTINCT")> _
    <Title("Select - Named Type")> _
    <Description("This sample uses Select and a known type to return a sequence of employee names.")> _
    Public Sub LinqToSqlSelect06()
        Dim names = From emp In db.Employees _
                    Select New Name With {.FirstName = emp.FirstName, _
                                          .LastName = emp.LastName}

        ObjectDumper.Write(names, 1)
    End Sub

    <Category("SELECT/DISTINCT")> _
    <Title("Select - Filtered")> _
    <Description("This sample uses Select and Where clauses to return a sequence of " & _
                 "just the London Customers' contact names.")> _
    Public Sub LinqToSqlSelect07()
        Dim londonNames = From cust In db.Customers _
                          Where cust.City = "London" _
                          Select cust.ContactName

        ObjectDumper.Write(londonNames)
    End Sub

    <Category("SELECT/DISTINCT")> _
    <Title("Select - Shaped")> _
    <Description("This sample uses Select and anonymous types to return " & _
                 "a shaped subset of the data about Customers.")> _
    Public Sub LinqToSqlSelect08()
        Dim customers = From cust In db.Customers _
                        Select cust.CustomerID, CompanyInfo = New With {cust.CompanyName, _
                                                                        cust.City, _
                                                                        cust.Country}, _
                                                ContactInfo = New With {cust.ContactName, _
                                                                        cust.ContactTitle}

        ObjectDumper.Write(customers, 1)
    End Sub

    <Category("SELECT/DISTINCT")> _
    <Title("Select - Nested")> _
    <Description("This sample uses nested queries to return a sequence of " & _
                 "all orders containing their OrderID, a subsequence of the " & _
                 "items in the order where there is a discount, and the money " & _
                 "saved if shipping is not included.")> _
    Public Sub LinqToSqlSelect09()
        Dim orders = From ord In db.Orders _
                     Select ord.OrderID, DiscountedProducts = (From od In ord.Order_Details _
                                                               Where od.Discount > 0.0), _
                                         FreeShippingDiscount = ord.Freight

        ObjectDumper.Write(orders, 1)
    End Sub

    '' Phone converter that converts a phone number to 
    '' an international format based on its country.
    '' This sample only supports USA and UK formats, for 
    '' phone numbers from the Northwind database.
    Public Function PhoneNumberConverter(ByVal Country As String, ByVal Phone As String) As String
        Phone = Phone.Replace(" ", "").Replace(")", ")-")
        Select Case Country
            Case "USA"
                Return "1-" & Phone
            Case "UK"
                Return "44-" & Phone
            Case Else
                Return Phone
        End Select
    End Function

    <Category("SELECT/DISTINCT")> _
    <Title("Select - Local Method Call 1")> _
    <Description("This sample uses a Local Method Call to " & _
                 "'PhoneNumberConverter' to convert Phone number " & _
                 "to an international format.")> _
    Public Sub LinqToSqlLocalMethodCall01()

        Dim q = From c In db.Customers _
                Where c.Country = "UK" Or c.Country = "USA" _
                Select c.CustomerID, c.CompanyName, Phone = c.Phone, InternationalPhone = PhoneNumberConverter(c.Country, c.Phone)

        ObjectDumper.Write(q)
    End Sub

    <Category("SELECT/DISTINCT")> _
    <Title("Select - Local Method Call 2")> _
    <Description("This sample uses a Local Method Call to " & _
                 "convert phone numbers to an international format " & _
                 "and create XDocument.")> _
    Public Sub LinqToSqlLocalMethodCall02()

        Dim doc = <Customers>
                      <%= From c In db.Customers _
                          Where c.Country = "UK" Or c.Country = "USA" _
                          Select <Customer CustomerID=<%= c.CustomerID %>
                                     CompanyName=<%= c.CompanyName %>
                                     InternationalPhone=<%= PhoneNumberConverter(c.Country, c.Phone) %>/> %>
                  </Customers>

        Console.WriteLine(doc.ToString())
    End Sub


    <Category("SELECT/DISTINCT")> _
    <Title("Distinct")> _
    <Description("This sample uses Distinct to select a sequence of the unique cities " & _
                 "that have Customers.")> _
    Public Sub LinqToSqlSelect10()
        Dim cities = From cust In db.Customers _
                     Select cust.City _
                     Distinct

        ObjectDumper.Write(cities)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Count - Simple")> _
    <Description("This sample uses Count to find the number of Customers in the database.")> _
    Public Sub LinqToSqlCount01()
        Dim custCount = db.Customers.Count()
        Console.WriteLine(custCount)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Count - Conditional")> _
    <Description("This sample uses Count to find the number of Products in the database " & _
                 "that are not discontinued.")> _
    Public Sub LinqToSqlCount02()
        Dim activeProducts = Aggregate prod In db.Products _
                             Into Count(Not prod.Discontinued)

        'Alternative Syntax
        'Dim activeProducts = (From prod In db.Products _
        '                      Where Not prod.Discontinued _
        '                      Select prod).Count()

        Console.WriteLine(activeProducts)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Sum - Simple")> _
    <Description("This sample uses Sum to find the total freight over all Orders.")> _
    Public Sub LinqToSqlCount03()

        Dim totalFreight = Aggregate ord In db.Orders _
                           Into Sum(ord.Freight)

        'Alternative Syntax
        'Dim totalFreight = (From ord In db.Orders _
        '                    Select ord.Freight).Sum()

        Console.WriteLine(totalFreight)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Sum - Mapped")> _
    <Description("This sample uses Sum to find the total number of units on order over all Products.")> _
    Public Sub LinqToSqlCount04()
        Dim totalUnits = (From prod In db.Products _
                          Select CInt(prod.UnitsOnOrder.Value)).Sum()

        Console.WriteLine(totalUnits)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Min - Simple")> _
    <Description("This sample uses Min to find the lowest unit price of any Product.")> _
    Public Sub LinqToSqlCount05()
        Dim lowestPrice = Aggregate prod In db.Products _
                          Into Min(prod.UnitPrice)

        Console.WriteLine(lowestPrice)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Min - Mapped")> _
    <Description("This sample uses Min to find the lowest freight of any Order.")> _
    Public Sub LinqToSqlCount06()
        Dim lowestFreight = Aggregate ord In db.Orders _
                            Into Min(ord.Freight)

        Console.WriteLine(lowestFreight)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Min - Elements")> _
    <Description("This sample uses Min to find the Products that have the lowest unit price " & _
                 "in each category.")> _
    Public Sub LinqToSqlCount07()
        Dim categories = From prod In db.Products _
                         Group prod By prod.CategoryID Into g = Group _
                         Select CategoryID = g, _
                                CheapestProducts = _
                                    From p2 In g _
                                    Where p2.UnitPrice = g.Min(Function(p3) p3.UnitPrice) _
                                    Select p2

        ObjectDumper.Write(categories, 1)
    End Sub


    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Max - Simple")> _
    <Description("This sample uses Max to find the latest hire date of any Employee.")> _
    Public Sub LinqToSqlCount08()
        Dim latestHire = Aggregate emp In db.Employees _
                         Into Max(emp.HireDate)

        Console.WriteLine(latestHire)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Max - Mapped")> _
    <Description("This sample uses Max to find the most units in stock of any Product.")> _
    Public Sub LinqToSqlCount09()
        Dim mostInStock = Aggregate prod In db.Products _
                          Into Max(prod.UnitsInStock)

        Console.WriteLine(mostInStock)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Max - Elements")> _
    <Description("This sample uses Max to find the Products that have the highest unit price " & _
                 "in each category.")> _
    Public Sub LinqToSqlCount10()
        Dim categories = From prod In db.Products _
                         Group prod By prod.CategoryID Into g = Group _
                         Select CategoryGroup = g, _
                                MostExpensiveProducts = _
                                    From p2 In g _
                                    Where p2.UnitPrice = g.Max(Function(p3) p3.UnitPrice)

        ObjectDumper.Write(categories, 1)
    End Sub


    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Average - Simple")> _
    <Description("This sample uses Average to find the average freight of all Orders.")> _
    Public Sub LinqToSqlCount11()
        Dim avgFreight = Aggregate ord In db.Orders _
                         Into Average(ord.Freight)

        Console.WriteLine(avgFreight)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Average - Mapped")> _
    <Description("This sample uses Average to find the average unit price of all Products.")> _
    Public Sub LinqToSqlCount12()
        Dim avgPrice = Aggregate prod In db.Products _
                       Into Average(prod.UnitPrice)

        Console.WriteLine(avgPrice)
    End Sub

    <Category("COUNT/SUM/MIN/MAX/AVG")> _
    <Title("Average - Elements")> _
    <Description("This sample uses Average to find the Products that have unit price higher than " & _
                 "the average unit price of the category for each category.")> _
    Public Sub LinqToSqlCount13()
        Dim categories = From prod In db.Products _
                         Group prod By prod.CategoryID Into g = Group _
                         Select g, _
                                ExpensiveProducts = _
                                    From prod2 In g _
                                    Where (prod2.UnitPrice > g.Average(Function(p3) p3.UnitPrice))

        ObjectDumper.Write(categories, 1)
    End Sub


    <Category("JOIN")> _
    <Title("SelectMany - 1 to Many - 1")> _
    <Description("This sample uses foreign key navigation in the " & _
                 "From clause to select all orders for customers in London.")> _
    Public Sub LinqToSqlJoin01()
        Dim ordersInLondon = From cust In db.Customers, ord In cust.Orders _
                             Where cust.City = "London"

        ObjectDumper.Write(ordersInLondon, 1)
    End Sub

    <Category("JOIN")> _
    <Title("SelectMany - 1 to Many - 2")> _
    <Description("This sample uses foreign key navigation in the " & _
                 "Where clause to filter for Products whose Supplier is in the USA " & _
                 "that are out of stock.")> _
    Public Sub LinqToSqlJoin02()
        Dim outOfStock = From prod In db.Products _
                         Where prod.Supplier.Country = "USA" AndAlso prod.UnitsInStock = 0

        ObjectDumper.Write(outOfStock)
    End Sub

    <Category("JOIN")> _
    <Title("SelectMany - Many to Many")> _
    <Description("This sample uses foreign key navigation in the " & _
                 "from clause to filter for employees in Seattle, " & _
                 "and also list their territories.")> _
    Public Sub LinqToSqlJoin03()
        Dim seattleEmployees = From emp In db.Employees, et In emp.EmployeeTerritories _
                               Where emp.City = "Seattle" _
                               Select emp.FirstName, emp.LastName, et.Territory.TerritoryDescription

        ObjectDumper.Write(seattleEmployees)
    End Sub

    <Category("JOIN")> _
    <Title("SelectMany - Self-Join")> _
    <Description("This sample uses foreign key navigation in the " & _
                 "Select clause to filter for pairs of employees where " & _
                 "one employee reports to the other and where " & _
                 "both employees are from the same City.")> _
    Public Sub LinqToSqlJoin04()
        Dim empQuery = From emp1 In db.Employees, emp2 In emp1.Employees _
                       Where emp1.City = emp2.City _
                       Select FirstName1 = emp1.FirstName, LastName1 = emp1.LastName, _
                              FirstName2 = emp2.FirstName, LastName2 = emp2.LastName, emp1.City

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("JOIN")> _
    <Title("GroupJoin - Two way join")> _
    <Description("This sample explictly joins two tables and projects results from both tables.")> _
    Public Sub LinqToSqlJoin05()
        Dim ordCount = From cust In db.Customers _
                       Group Join ord In db.Orders On cust.CustomerID Equals ord.CustomerID _
                       Into orders = Group _
                       Select cust.ContactName, OrderCount = orders.Count()

        ObjectDumper.Write(ordCount)
    End Sub
    <Category("JOIN")> _
    <Title("GroupJoin - Three way join")> _
    <Description("This sample explictly joins three tables and projects results from each of them.")> _
    Public Sub LinqToSqlJoin06()
        Dim joinQuery = From cust In db.Customers _
                        Group Join ord In db.Orders On cust.CustomerID Equals ord.CustomerID _
                              Into ords = Group _
                        Group Join emp In db.Employees On cust.City Equals emp.City _
                              Into emps = Group _
                        Select cust.ContactName, ords = ords.Count(), emps = emps.Count()

        ObjectDumper.Write(joinQuery)
    End Sub

    <Category("JOIN")> _
    <Title("GroupJoin - LEFT OUTER JOIN")> _
    <Description("This sample shows how to get LEFT OUTER JOIN by using DefaultIfEmpty(). " & _
                 "The DefaultIfEmpty() method returns Nothing when there is no Order for the Employee.")> _
    Public Sub LinqToSqlJoin07()
        Dim empQuery = From emp In db.Employees _
                       Group Join ord In db.Orders On emp Equals ord.Employee _
                             Into ords = Group _
                       From ord2 In ords.DefaultIfEmpty _
                       Select emp.FirstName, emp.LastName, Order = ord2

        ObjectDumper.Write(empQuery, 1)
    End Sub

    <Category("JOIN")> _
    <Title("GroupJoin - Projected let assignment")> _
    <Description("This sample projects a 'Let' expression resulting from a join.")> _
    Public Sub LinqToSqlJoin08()
        Dim ordQuery = From cust In db.Customers _
                       Group Join ord In db.Orders On cust.CustomerID Equals ord.CustomerID _
                       Into ords = Group _
                       Let Location = cust.City + cust.Country _
                       From ord2 In ords _
                       Select cust.ContactName, ord2.OrderID, Location

        ObjectDumper.Write(ordQuery)
    End Sub

    <Category("JOIN")> _
    <Title("GroupJoin - Composite Key")> _
    <Description("This sample shows a join with a composite key.")> _
    Public Sub LinqToSqlJoin09()

        'The Key keyword means that when the anonymous types are tested for equality,
        'only the OrderID field will be compared
        Dim ordQuery = From ord In db.Orders _
                       From prod In db.Products _
                       Group Join details In db.Order_Details _
                           On New With {Key ord.OrderID, prod.ProductID} _
                           Equals New With {Key details.OrderID, details.ProductID} _
                       Into details = Group _
                       From d In details _
                       Select ord.OrderID, prod.ProductID, d.UnitPrice

        ObjectDumper.Write(ordQuery)
    End Sub

    <Category("JOIN")> _
    <Title("GroupJoin - Nullable\\Nonnullable Key Relationship")> _
    <Description("This sample shows how to construct a join where one side is nullable and the other isn't.")> _
    Public Sub LinqToSqlJoin10()
        Dim ordQuery = From ord In db.Orders _
                       Group Join emp In db.Employees _
                           On ord.EmployeeID Equals CType(emp.EmployeeID, Integer?) _
                       Into emps = Group _
                       From emp2 In emps _
                       Select ord.OrderID, emp2.FirstName

        ObjectDumper.Write(ordQuery)
    End Sub

    <Category("ORDER BY")> _
    <Title("OrderBy - Simple")> _
    <Description("This sample uses Order By to sort Employees " & _
                 "by hire date.")> _
    Public Sub LinqToSqlOrderBy01()
        Dim empQuery = From emp In db.Employees _
                       Order By emp.HireDate

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("ORDER BY")> _
    <Title("OrderBy - With Where")> _
    <Description("This sample uses Where and Order By to sort Orders " & _
                 "shipped to London by freight.")> _
    Public Sub LinqToSqlOrderBy02()
        Dim londonOrders = From ord In db.Orders _
                           Where ord.ShipCity = "London" _
                           Order By ord.Freight

        ObjectDumper.Write(londonOrders)
    End Sub

    <Category("ORDER BY")> _
    <Title("OrderByDescending")> _
    <Description("This sample uses Order By to sort Products " & _
                 "by unit price from highest to lowest.")> _
    Public Sub LinqToSqlOrderBy03()
        Dim sortedProducts = From prod In db.Products _
                             Order By prod.UnitPrice Descending

        ObjectDumper.Write(sortedProducts)
    End Sub

    <Category("ORDER BY")> _
    <Title("ThenBy")> _
    <Description("This sample uses a compound Order By to sort Customers " & _
                 "by city and then contact name.")> _
    Public Sub LinqToSqlOrderBy04()
        Dim custQuery = From cust In db.Customers _
                        Select cust _
                        Order By cust.City, cust.ContactName

        ObjectDumper.Write(custQuery)
    End Sub

    <Category("ORDER BY")> _
    <Title("ThenByDescending")> _
    <Description("This sample uses Order By to sort Orders from EmployeeID 1 " & _
                 "by ship-to country, and then by freight from highest to lowest.")> _
    Public Sub LinqToSqlOrderBy05()
        Dim ordQuery = From ord In db.Orders _
                       Where ord.EmployeeID = 1 _
                       Order By ord.ShipCountry, ord.Freight Descending

        ObjectDumper.Write(ordQuery)
    End Sub

    <Category("ORDER BY")> _
    <Title("OrderBy - Group By")> _
    <Description("This sample uses Order By, Max and Group By to find the Products that have " & _
                 "the highest unit price in each category, and sorts the group by category id.")> _
    Public Sub LinqToSqlOrderBy06()
        Dim categories = From prod In db.Products _
                         Group prod By prod.CategoryID Into Group _
                         Order By CategoryID _
                         Select Group, _
                                MostExpensiveProducts = _
                                    From prod2 In Group _
                                    Where prod2.UnitPrice = _
                                        Group.Max(Function(prod3) prod3.UnitPrice)

        ObjectDumper.Write(categories, 1)
    End Sub

    <Category("GROUP BY/HAVING")> _
    <Title("GroupBy - Simple")> _
    <Description("This sample uses Group By to partition Products by " & _
                 "CategoryID.")> _
    Public Sub LinqToSqlGroupBy01()
        Dim categorizedProducts = From prod In db.Products _
                                  Group prod By prod.CategoryID Into prodGroup = Group _
                                  Select prodGroup

        ObjectDumper.Write(categorizedProducts, 1)
    End Sub

    <Category("GROUP BY/HAVING")> _
    <Title("GroupBy - Max")> _
    <Description("This sample uses Group By and Max " & _
                 "to find the maximum unit price for each CategoryID.")> _
    Public Sub LinqToSqlGroupBy02()
        Dim maxPrices = From prod In db.Products _
                        Group prod By prod.CategoryID _
                        Into prodGroup = Group, MaxPrice = Max(prod.UnitPrice) _
                        Select prodGroup, MaxPrice

        ObjectDumper.Write(maxPrices, 1)
    End Sub

    <Category("GROUP BY/HAVING")> _
    <Title("GroupBy - Min")> _
    <Description("This sample uses Group By and Min " & _
                 "to find the minimum unit price for each CategoryID.")> _
    Public Sub LinqToSqlGroupBy03()
        Dim minPrices = From prod In db.Products _
                        Group prod By prod.CategoryID _
                        Into prodGroup = Group, MinPrice = Min(prod.UnitPrice)

        ObjectDumper.Write(minPrices, 1)
    End Sub

    <Category("GROUP BY/HAVING")> _
    <Title("GroupBy - Average")> _
    <Description("This sample uses Group By and Average " & _
                 "to find the average UnitPrice for each CategoryID.")> _
    Public Sub LinqToSqlGroupBy04()
        Dim avgPrices = From prod In db.Products _
                        Group prod By prod.CategoryID _
                        Into prodGroup = Group, AveragePrice = Average(prod.UnitPrice)

        ObjectDumper.Write(avgPrices, 1)
    End Sub

    <Category("GROUP BY/HAVING")> _
    <Title("GroupBy - Sum")> _
    <Description("This sample uses Group By and Sum " & _
                 "to find the total UnitPrice for each CategoryID.")> _
    Public Sub LinqToSqlGroupBy05()
        Dim totalPrices = From prod In db.Products _
                          Group prod By prod.CategoryID _
                          Into prodGroup = Group, TotalPrice = Sum(prod.UnitPrice)

        ObjectDumper.Write(totalPrices, 1)
    End Sub

    <Category("GROUP BY/HAVING")> _
    <Title("GroupBy - Count")> _
    <Description("This sample uses Group By and Count " & _
                 "to find the number of Products in each CategoryID.")> _
    Public Sub LinqToSqlGroupBy06()
        Dim prodQuery = From prod In db.Products _
                        Group prod By prod.CategoryID _
                        Into prodGroup = Group _
                        Select prodGroup, NumProducts = prodGroup.Count()

        ObjectDumper.Write(prodQuery, 1)
    End Sub

    <Category("GROUP BY/HAVING")> _
    <Title("GroupBy - Count - Conditional")> _
    <Description("This sample uses Group By and Count " & _
                 "to find the number of Products in each CategoryID " & _
                 "that are discontinued.")> _
    Public Sub LinqToSqlGroupBy07()

        Dim prodQuery = From prod In db.Products _
                        Group prod By prod.CategoryID _
                        Into prodGroup = Group, NumProducts = Count(prod.Discontinued)

        'Alternative Syntax
        'Dim prodQuery = From prod In db.Products _
        '                Group prod By prod.CategoryID Into prodGroup = Group _
        '                Select prodGroup, _
        '                       NumProducts = prodGroup.Count(Function(prod2) prod2.Discontinued)

        ObjectDumper.Write(prodQuery, 1)
    End Sub

    <Category("GROUP BY/HAVING")> _
    <Title("GroupBy - followed by Where")> _
    <Description("This sample uses a Where clause after a Group By clause " & _
                 "to find all categories that have at least 10 products.")> _
    Public Sub LinqToSqlGroupBy08()
        Dim bigCategories = From prod In db.Products _
                            Group prod By prod.CategoryID _
                            Into ProdGroup = Group, ProdCount = Count() _
                            Where ProdCount >= 10 _
                            Select ProdGroup, ProdCount

        ObjectDumper.Write(bigCategories, 1)
    End Sub

    <Category("GROUP BY/HAVING")> _
    <Title("GroupBy - Multiple Columns")> _
    <Description("This sample uses Group By to group products by CategoryID and SupplierID.")> _
    Public Sub LinqToSqlGroupBy09()
        Dim categories = From prod In db.Products _
                         Group By Key = New With {prod.CategoryID, prod.SupplierID} _
                         Into prodGroup = Group _
                         Select Key, prodGroup

        ObjectDumper.Write(categories, 1)
    End Sub

    <Category("GROUP BY/HAVING")> _
    <Title("GroupBy - Expression")> _
    <Description("This sample uses Group By to return two sequences of products. " & _
                 "The first sequence contains products with unit price " & _
                 "greater than 10. The second sequence contains products " & _
                 "with unit price less than or equal to 10.")> _
    Public Sub LinqToSqlGroupBy10()
        Dim categories = From prod In db.Products _
                         Group prod By Key = New With {.Criterion = prod.UnitPrice > 10} _
                         Into ProductGroup = Group

        ObjectDumper.Write(categories, 1)
    End Sub

    <Category("EXISTS/IN/ANY/ALL")> _
    <Title("Any - Simple")> _
    <Description("This sample uses the Any operator to return only Customers that have no Orders.")> _
    Public Sub LinqToSqlExists01()
        Dim custQuery = From cust In db.Customers _
                        Where Not cust.Orders.Any()

        ObjectDumper.Write(custQuery)
    End Sub

    <Category("EXISTS/IN/ANY/ALL")> _
    <Title("Any - Conditional")> _
    <Description("This sample uses Any to return only Categories that have " & _
                 "at least one Discontinued product.")> _
    Public Sub LinqToSqlExists02()
        Dim prodQuery = From cust In db.Categories _
                        Where (From prod In cust.Products Where prod.Discontinued).Any()

        ObjectDumper.Write(prodQuery)
    End Sub

    <Category("EXISTS/IN/ANY/ALL")> _
    <Title("All - Conditional")> _
    <Description("This sample uses All to return Customers whom all of their orders " & _
                 "have been shipped to their own city or whom have no orders.")> _
    Public Sub LinqToSqlExists03()
        Dim ordQuery = From cust In db.Customers _
                       Where cust.Orders.All(Function(ord) ord.ShipCity = cust.City)

        ObjectDumper.Write(ordQuery)
    End Sub

    <Category("Exists/In/Any/All/Contains")> _
    <Title("Contains - One Object")> _
    <Description("This sample uses Contain to find which Customer contains an order with OrderID 10248.")> _
    Public Sub LinqToSqlExists04()

        Dim order = (From o In db.Orders _
                     Where o.OrderID = 10248).First()

        Dim q = db.Customers.Where(Function(p) p.Orders.Contains(order)).ToList()

        For Each cust In q
            For Each ord In cust.Orders

                Console.WriteLine("Customer {0} has OrderID {1}.", _
                                  cust.CustomerID, ord.OrderID)
            Next
        Next

    End Sub

    <Category("Exists/In/Any/All/Contains")> _
    <Title("Contains - Multiple values")> _
    <Description("This sample uses Contains to find customers whose city is Seattle, London, Paris or Vancouver.")> _
    Public Sub LinqToSqlExists05()
        Dim cities = New String() {"Seattle", "London", "Vancouver", "Paris"}

        Dim q = db.Customers.Where(Function(p) cities.Contains(p.City)).ToList()

        ObjectDumper.Write(q)
    End Sub

    <Category("UNION ALL/UNION/INTERSECT")> _
    <Title("Concat - Simple")> _
    <Description("This sample uses Concat to return a sequence of all Customer and Employee " & _
                 "phone/fax numbers.")> _
    Public Sub LinqToSqlUnion01()
        Dim phoneNumbers = (From cust In db.Customers Select cust.Phone).Concat( _
                            From cust In db.Customers Select cust.Fax).Concat( _
                            From emp In db.Employees Select emp.HomePhone)

        ObjectDumper.Write(phoneNumbers)
    End Sub

    <Category("UNION ALL/UNION/INTERSECT")> _
    <Title("Concat - Compound")> _
    <Description("This sample uses Concat to return a sequence of all Customer and Employee " & _
                 "name and phone number mappings.")> _
    Public Sub LinqToSqlUnion02()
        Dim custPhones = From cust In db.Customers _
                         Select Name = cust.CompanyName, _
                                Phone = cust.Phone

        Dim phoneNumbers = custPhones.Concat(From emp In db.Employees _
                                             Select Name = emp.FirstName & " " & emp.LastName, _
                                                    Phone = emp.HomePhone)

        ObjectDumper.Write(phoneNumbers)
    End Sub

    <Category("UNION ALL/UNION/INTERSECT")> _
    <Title("Union")> _
    <Description("This sample uses Union to return a sequence of all countries that either " & _
                 "Customers or Employees live in.")> _
    Public Sub LinqToSqlUnion03()
        Dim countries = (From cust In db.Customers _
                         Select cust.Country).Union(From emp In db.Employees _
                                                    Select emp.Country)

        ObjectDumper.Write(countries)
    End Sub

    <Category("UNION ALL/UNION/INTERSECT")> _
    <Title("Intersect")> _
    <Description("This sample uses Intersect to return a sequence of all countries that both " & _
                 "Customers and Employees live in.")> _
    Public Sub LinqToSqlUnion04()
        Dim countries = (From cust In db.Customers _
                         Select cust.Country).Intersect(From emp In db.Employees _
                                                        Select emp.Country)

        ObjectDumper.Write(countries)
    End Sub

    <Category("UNION ALL/UNION/INTERSECT")> _
    <Title("Except")> _
    <Description("This sample uses Except to return a sequence of all countries that " & _
                 "Customers live in but no Employees live in.")> _
    Public Sub LinqToSqlUnion05()
        Dim countries = (From cust In db.Customers _
                         Select cust.Country).Except(From emp In db.Employees _
                                                     Select emp.Country)

        ObjectDumper.Write(countries)
    End Sub

    <Category("TOP/BOTTOM")> _
    <Title("Take")> _
    <Description("This sample uses Take to select the first 5 Employees hired.")> _
    Public Sub LinqToSqlTop01()
        Dim first5Employees = From emp In db.Employees _
                              Order By emp.HireDate _
                              Take 5

        ObjectDumper.Write(first5Employees)
    End Sub

    <Category("TOP/BOTTOM")> _
    <Title("Skip")> _
    <Description("This sample uses Skip to select all but the 10 most expensive Products.")> _
    Public Sub LinqToSqlTop02()
        Dim expensiveProducts = From prod In db.Products _
                                Order By prod.UnitPrice Descending _
                                Skip 10

        ObjectDumper.Write(expensiveProducts)
    End Sub

    <Category("Paging")> _
    <Title("Paging - Index")> _
    <Description("This sample uses the Skip and Take operators to do paging by " & _
                 "skipping the first 50 records and then returning the next 10, thereby " & _
                 "providing the data for page 6 of the Products table.")> _
    Public Sub LinqToSqlPaging01()
        Dim productPage = From cust In db.Customers _
                          Order By cust.ContactName _
                          Skip 50 _
                          Take 10

        ObjectDumper.Write(productPage)
    End Sub

    <Category("Paging")> _
    <Title("Paging - Ordered Unique Key")> _
    <Description("This sample uses a Where clause and the Take operator to do paging by, " & _
                 "first filtering to get only the ProductIDs above 50 (the last ProductID " & _
                 "from page 5), then ordering by ProductID, and finally taking the first 10 results, " & _
                 "thereby providing the data for page 6 of the Products table.  " & _
                 "Note that this method only works when ordering by a unique key.")> _
    Public Sub LinqToSqlPaging02()
        Dim productPage = From prod In db.Products _
                          Where prod.ProductID > 50 _
                          Select prod _
                          Order By prod.ProductID _
                          Take 10

        ObjectDumper.Write(productPage)
    End Sub

    <Category("SqlMethods")> _
    <Title("SqlMethods - Like")> _
    <Description("This sample uses SqlMethods to filter for Customers with CustomerID that starts with 'C'.")> _
    Public Sub LinqToSqlSqlMethods01()


        Dim q = From c In db.Customers _
                Where SqlMethods.Like(c.CustomerID, "C%") _
                Select c

        ObjectDumper.Write(q)

    End Sub

    <Category("SqlMethods")> _
    <Title("SqlMethods - DateDiffDay")> _
    <Description("This sample uses SqlMethods to find all orders which shipped within 10 days the order created")> _
    Public Sub LinqToSqlSqlMethods02()

        Dim orderQuery = From o In db.Orders _
                         Where SqlMethods.DateDiffDay(o.OrderDate, o.ShippedDate) < 10

        ObjectDumper.Write(orderQuery)
    End Sub


    <Category("Compiled Query")> _
    <Title("Compiled Query - 1")> _
    <Description("This sample create a compiled query and then use it to retrieve customers of the input city")> _
    Public Sub LinqToSqlCompileQuery01()

        '' Create compiled query
        Dim fn = CompiledQuery.Compile( _
                Function(db2 As NorthwindDataContext, city As String) _
                    From c In db2.Customers _
                    Where c.City = city _
                    Select c)


        Console.WriteLine("****** Call compiled query to retrieve customers from London ******")
        Dim LonCusts = fn(db, "London")
        ObjectDumper.Write(LonCusts)

        Console.WriteLine()

        Console.WriteLine("****** Call compiled query to retrieve customers from Seattle ******")
        Dim SeaCusts = fn(db, "Seattle")
        ObjectDumper.Write(SeaCusts)

    End Sub


    <Category("INSERT/UPDATE/DELETE")> _
    <Title("Insert - Simple")> _
    <Description("This sample uses the Add method to add a new Customer to the " & _
                 "Customers Table object.  The call to SubmitChanges persists this " & _
                 "new Customer to the database.")> _
    Public Sub LinqToSqlInsert01()
        Dim custQuery = From cust In db.Customers _
                        Where cust.Region = "WA"

        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(custQuery)


        Console.WriteLine()
        Console.WriteLine("*** INSERT ***")
        Dim newCustomer = New Customer With {.CustomerID = "MCSFT", .CompanyName = "Microsoft", .ContactName = "John Doe", .ContactTitle = "Sales Manager", .Address = "1 Microsoft Way", .City = "Redmond", .Region = "WA", .PostalCode = "98052", .Country = "USA", .Phone = "(425) 555-1234", .Fax = Nothing}
        db.Customers.InsertOnSubmit(newCustomer)
        db.SubmitChanges()


        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        ObjectDumper.Write(custQuery)

        Cleanup56()  ' Restore previous database state
    End Sub

    Private Sub Cleanup56()
        setLogging(False)

        db.Customers.DeleteAllOnSubmit(From cust In db.Customers _
                                       Where cust.CustomerID = "MCSFT")
        db.SubmitChanges()
    End Sub

    <Category("INSERT/UPDATE/DELETE")> _
    <Title("Insert - 1-to-Many")> _
    <Description("This sample uses the Add method to add a new Category to the " & _
                 "Categories table object, and a new Product to the Products Table " & _
                 "object with a foreign key relationship to the new Category.  The call " & _
                 "to SubmitChanges persists these new objects and their relationships " & _
                 "to the database.")> _
    Public Sub LinqToSqlInsert02()

        Dim db2 = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

        Dim ds = New DataLoadOptions()

        ds.LoadWith(Of Category)(Function(p) p.Products)
        db2.LoadOptions = ds

        Dim q = From category In db2.Categories _
                Where category.CategoryName = "Widgets"


        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(q, 1)


        Console.WriteLine()
        Console.WriteLine("*** INSERT ***")
        Dim newCategory = New Category With _
                            {.CategoryName = "Widgets", _
                             .Description = "Widgets are the customer-facing analogues " & _
                                                       "to sprockets and cogs."}

        Dim newProduct = New Product With {.ProductName = "Blue Widget", _
                                           .UnitPrice = New Decimal?(34.56D), _
                                           .Category = newCategory}
        db2.Categories.InsertOnSubmit(newCategory)
        db2.SubmitChanges()


        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        ObjectDumper.Write(q, 1)

        Cleanup65()  'Restore previous database state
    End Sub

    Private Sub Cleanup65()
        setLogging(False)

        db.Products.DeleteAllOnSubmit(From prod In db.Products _
                                      Where prod.Category.CategoryName = "Widgets")

        db.Categories.DeleteAllOnSubmit(From category In db.Categories _
                                        Where category.CategoryName = "Widgets")

        db.SubmitChanges()
    End Sub

    <Category("INSERT/UPDATE/DELETE")> _
    <Title("Insert - Many-to-Many")> _
    <Description("This sample uses the Add method to add a new Employee to the " & _
                 "Employees table object, a new Territory to the Territories table " & _
                 "object, and a new EmployeeTerritory to the EmployeeTerritories table " & _
                 "object with foreign key relationships to the new Employee and Territory.  " & _
                 "The call to SubmitChanges persists these new objects and their " & _
                 "relationships to the database.")> _
    Public Sub LinqToSqlInsert03()

        Dim db2 = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

        Dim ds = New DataLoadOptions()
        ds.LoadWith(Of Employee)(Function(p) p.EmployeeTerritories)
        ds.LoadWith(Of EmployeeTerritory)(Function(p) p.Territory)

        db2.LoadOptions = ds
        Dim empQuery = From emp In db.Employees _
                       Where emp.FirstName = "Nancy"

        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(empQuery, 1)


        Console.WriteLine()
        Console.WriteLine("*** INSERT ***")
        Dim newEmployee = New Employee With {.FirstName = "Kira", .LastName = "Smith"}
        Dim newTerritory = New Territory With {.TerritoryID = "12345", _
                                               .TerritoryDescription = "Anytown", _
                                               .Region = db.Regions.First()}

        Dim newEmployeeTerritory = New EmployeeTerritory With _
                                    {.Employee = newEmployee, .Territory = newTerritory}
        db.Employees.InsertOnSubmit(newEmployee)
        db.Territories.InsertOnSubmit(newTerritory)
        db.EmployeeTerritories.InsertOnSubmit(newEmployeeTerritory)
        db.SubmitChanges()

        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        ObjectDumper.Write(empQuery, 2)

        Cleanup166()  ' Restore previous database state
    End Sub

    Private Sub Cleanup166()
        setLogging(False)

        db.EmployeeTerritories.DeleteAllOnSubmit(From et In db.EmployeeTerritories _
                                                 Where et.TerritoryID = "12345")
        db.Employees.DeleteAllOnSubmit(From emp In db.Employees _
                                       Where emp.FirstName = "Kira" AndAlso emp.LastName = "Smith")

        db.Territories.DeleteAllOnSubmit(From terr In db.Territories _
                                         Where terr.TerritoryID = "12345")
        db.SubmitChanges()
    End Sub


    <Category("INSERT/UPDATE/DELETE")> _
    <Title("Update - Simple")> _
    <Description("This sample uses SubmitChanges to persist an update made to a retrieved " & _
                 "Customer object back to the database.")> _
    Public Sub LinqToSqlInsert04()
        Dim custQuery = From cust In db.Customers _
                        Where cust.CustomerID = "ALFKI"

        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(custQuery)

        Console.WriteLine()
        Console.WriteLine("*** UPDATE ***")

        Dim custToUpdate = (From cust In db.Customers _
                            Where cust.CustomerID = "ALFKI").First()
        custToUpdate.ContactTitle = "Vice President"

        db.SubmitChanges()

        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        ObjectDumper.Write(custQuery)

        Cleanup59()  ' Restore previous database state
    End Sub

    Private Sub Cleanup59()
        setLogging(False)

        Dim custToUpdate = (From cust In db.Customers _
                            Where cust.CustomerID = "ALFKI").First()

        custToUpdate.ContactTitle = "Sales Representative"
        db.SubmitChanges()
    End Sub

    <Category("INSERT/UPDATE/DELETE")> _
    <Title("Update - Multiple and Show Changes")> _
    <Description("This sample uses SubmitChanges to persist updates made to multiple retrieved " & _
             "Product objects back to the database. Also demonstartes how to determine which " & _
             "how many objects changed, which objects changed, and which object members changed.")> _
    Public Sub LinqToSqlInsert05()

        Dim query = From p In db.Products _
                    Where p.CategoryID = 1

        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(query)


        Console.WriteLine()
        Console.WriteLine("*** UPDATE ***")
        For Each p In query
            p.UnitPrice += 1D
        Next

        Dim cs As ChangeSet = db.GetChangeSet()

        Console.WriteLine("*** CHANGE SET ***")
        Console.WriteLine("Number of entities inserted: {0}", cs.Inserts.Count)
        Console.WriteLine("Number of entities updated:  {0}", cs.Updates.Count)
        Console.WriteLine("Number of entities deleted:  {0}", cs.Deletes.Count)
        Console.WriteLine()

        Console.WriteLine("*** GetOriginalEntityState ***")
        For Each o In cs.Updates
            Dim p = TryCast(o, Product)

            If p IsNot Nothing Then
                Dim oldP As Product = db.Products.GetOriginalEntityState(p)

                Console.WriteLine("** Current **")
                ObjectDumper.Write(p)

                Console.WriteLine("** Original **")
                ObjectDumper.Write(oldP)

                Console.WriteLine()
            End If
        Next

        Console.WriteLine()

        Console.WriteLine("*** GetModifiedMembers ***")
        For Each o In cs.Updates

            Dim p = TryCast(o, Product)
            If p IsNot Nothing Then

                For Each mmi In db.Products.GetModifiedMembers(p)
                    Console.WriteLine("Member {0}", mmi.Member.Name)
                    Console.WriteLine(vbTab & "Original value: {0}", mmi.OriginalValue)
                    Console.WriteLine(vbTab & "\tCurrent  value: {0}", mmi.CurrentValue)
                Next

            End If
        Next

        db.SubmitChanges()

        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        ObjectDumper.Write(query)

        Cleanup68()  '' Restore previous database state
    End Sub


    Private Sub Cleanup68()
        setLogging(False)

        Dim query = From p In db.Products _
                    Where p.CategoryID = 1

        For Each p In query
            p.UnitPrice -= 1D
        Next
        db.SubmitChanges()
    End Sub

    <Category("INSERT/UPDATE/DELETE")> _
    <Title("Delete - Simple")> _
    <Description("This sample uses the Remove method to delete an OrderDetail from the " & _
                 "OrderDetails Table object.  The call to SubmitChanges persists this " & _
                 "deletion to the database.")> _
    Public Sub LinqToSqlInsert06()
        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(From cust In db.Order_Details _
                           Where cust.OrderID = 10255)

        Console.WriteLine()
        Console.WriteLine("*** DELETE ***")
        Dim order = (From cust In db.Order_Details _
                     Where cust.OrderID = 10255 AndAlso cust.ProductID = 36).First()

        db.Order_Details.DeleteOnSubmit(order)
        db.SubmitChanges()


        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        clearDBCache()
        ObjectDumper.Write(From cust In db.Order_Details _
                           Where cust.OrderID = 10255 _
                           Select cust)

        Cleanup61()  ' Restore previous database state
    End Sub

    Private Sub Cleanup61()
        setLogging(False)

        Dim order = New Order_Detail With {.OrderID = 10255, .ProductID = 36, _
                                           .UnitPrice = 15.2D, .Quantity = 25, _
                                           .Discount = 0.0F}

        db.Order_Details.InsertOnSubmit(order)

        db.SubmitChanges()
    End Sub


    <Category("INSERT/UPDATE/DELETE")> _
    <Title("Delete - One-to-Many")> _
    <Description("This sample uses the Remove method to delete an Order and Order Detail  " & _
                 "from the Order Details and Orders tables. First deleting Order Details and then " & _
                 "deleting from Orders. The call to SubmitChanges persists this deletion to the database.")> _
    Public Sub LinqToSqlInsert07()

        Dim orderDetails = From ordDetail In db.Order_Details _
                           Where ordDetail.Order.CustomerID = "WARTH" _
                                 AndAlso ordDetail.Order.EmployeeID = 3

        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(orderDetails)

        Console.WriteLine()
        Console.WriteLine("*** DELETE ***")
        Dim order = (From ord In db.Orders _
                     Where ord.CustomerID = "WARTH" AndAlso ord.EmployeeID = 3).First()

        For Each od As Order_Detail In orderDetails
            db.Order_Details.DeleteOnSubmit(od)
        Next

        db.Orders.DeleteOnSubmit(order)
        db.SubmitChanges()

        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        ObjectDumper.Write(orderDetails)

        Cleanup62()  ' Restore previous database state

    End Sub

    Private Sub Cleanup62()
        setLogging(False)

        Dim order = New Order With { _
                                    .CustomerID = "WARTH", _
                                    .EmployeeID = 3, _
                                    .OrderDate = New DateTime(1996, 7, 26), _
                                    .RequiredDate = New DateTime(1996, 9, 6), _
                                    .ShippedDate = New DateTime(1996, 7, 31), _
                                    .ShipVia = 3, _
                                    .Freight = 25.73D, _
                                    .ShipName = "Wartian Herkku", _
                                    .ShipAddress = "Torikatu 38", _
                                    .ShipCity = "Oulu", _
                                    .ShipPostalCode = "90110", _
                                    .ShipCountry = "Finland"}

        Dim orderDetail = New Order_Detail With { _
                                                 .ProductID = 12, _
                                                 .UnitPrice = 30.4D, _
                                                 .Quantity = 12, _
                                                 .Discount = 0.0F}

        order.Order_Details.Add(orderDetail)
        db.Orders.InsertOnSubmit(order)
        db.SubmitChanges()
    End Sub

    <Category("INSERT/UPDATE/DELETE")> _
    <Title("Insert - Override using Dynamic CUD")> _
    <Description("This sample uses partial method InsertRegion provided by the DataContext to insert a Region. " & _
                 "The call to SubmitChanges calls InsertRegion override which uses Dynamic CUD to run " & _
                 "the default Linq To SQL generated SQL Query")> _
    Public Sub LinqToSqlInsert09()

        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(From c In db.Regions _
                           Where c.RegionID = 32)

        Console.WriteLine()
        Console.WriteLine("*** INSERT OVERRIDE ***")

        ''Beverages
        Dim nwRegion As New Region() With {.RegionID = 32, .RegionDescription = "Rainy"}

        db.Regions.InsertOnSubmit(nwRegion)
        db.SubmitChanges()


        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        clearDBCache()

        ObjectDumper.Write(From c In db.Regions _
                           Where c.RegionID = 32)

        CleanupInsert09() '' Restore previous database state
    End Sub

    Private Sub CleanupInsert09()


        setLogging(False)

        db.Regions.DeleteAllOnSubmit(From r In db.Regions _
                                     Where r.RegionID = 32)
        db.SubmitChanges()
    End Sub

    <Category("INSERT/UPDATE/DELETE")> _
    <Title("Update with Attach")> _
    <Description("This sample takes entities from one context, uses Attach and AttachAll " & _
                 "to attach the entities from another context, and then updates the entities. " & _
                 "Changes are submitted to the database.")> _
    Public Sub LinqToSqlInsert10()

        '' Typically you would get entities to attach from deserializing XML from another tier.
        Dim c1 As Customer
        Dim c2Orders As List(Of Order)

        Using tempdb As New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

            c1 = tempdb.Customers.Single(Function(c) c.CustomerID = "ALFKI")
            Console.WriteLine("Customer {0}'s original address {1}", c1.CustomerID, c1.Address)
            Console.WriteLine()

            Dim tempcust = tempdb.Customers.Single(Function(c) c.CustomerID = "ANTON")
            c2Orders = tempcust.Orders.ToList()
            For Each o In c2Orders
                Console.WriteLine("Order {0} belongs to customer {1}", o.OrderID, o.CustomerID)
            Next

            Console.WriteLine()

            Dim tempcust2 = tempdb.Customers.Single(Function(c) c.CustomerID = "CHOPS")
            Dim c3Orders = tempcust2.Orders.ToList()
            For Each o In c3Orders
                Console.WriteLine("Order {0} belongs to customer {1}", o.OrderID, o.CustomerID)
            Next

            Console.WriteLine()
        End Using

        Using db2 As New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

            '' Attach the first entity to the current data context, to track changes.
            db2.Customers.Attach(c1)
            Console.WriteLine("***** Update Customer ALFKI's address ******")
            Console.WriteLine()

            '' Change the entity that is tracked.
            c1.Address = "123 First Ave"

            '' Attach all entities in the orders list.
            db2.Orders.AttachAll(c2Orders)

            '' Update the orders to belong to another customer.
            Console.WriteLine("****** Assign all Orders belong to ANTON to CHOPS ******")
            Console.WriteLine()

            For Each o In c2Orders
                o.CustomerID = "CHOPS"
            Next


            '' Submit the changes in the current data context.
            db2.SubmitChanges()
        End Using

        '' Check that the orders were submitted as expected.
        Using db3 As New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

            Dim dbC1 = db3.Customers.Single(Function(c) c.CustomerID = "ALFKI")

            Console.WriteLine("Customer {0}'s new address {1}", dbC1.CustomerID, dbC1.Address)
            Console.WriteLine()

            Dim dbC2 = db3.Customers.Single(Function(c) c.CustomerID = "CHOPS")

            For Each o In dbC2.Orders
                Console.WriteLine("Order {0} belongs to customer {1}", o.OrderID, o.CustomerID)
            Next

        End Using

        CleanupInsert10()
    End Sub


    Private Sub CleanupInsert10()

        Dim c2OrderIDs = New Integer() {10365, 10507, 10535, 10573, 10677, 10682, 10856}

        Using tempdb As New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

            Dim c1 = tempdb.Customers.Single(Function(c) c.CustomerID = "ALFKI")
            c1.Address = "Obere Str. 57"

            For Each o In tempdb.Orders.Where(Function(p) c2OrderIDs.Contains(p.OrderID))
                o.CustomerID = "ANTON"
            Next
            tempdb.SubmitChanges()
        End Using

    End Sub

    <Category("INSERT/UPDATE/DELETE")> _
    <Title("Update and Delete with Attach")> _
    <Description("This sample takes entities from one context and uses Attach and AttachAll " & _
                 "to attach the entities from another context. Then, two entities are updated, " & _
                 "one entity is deleted, and another entity is added. Changes are submitted to " & _
                 "the database")> _
    Public Sub LinqToSqlInsert11()

        '' Typically you would get entities to attach from deserializing
        '' XML from another tier.
        '' This sample uses LoadWith to eager load customer and orders
        '' in one query and disable deferred loading.
        Dim cust As Customer = Nothing
        Using tempdb As NorthwindDataContext = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

            Dim shape As New DataLoadOptions()
            shape.LoadWith(Of Customer)(Function(c) c.Orders)
            '' Load the first customer entity and its orders.
            tempdb.LoadOptions = shape
            tempdb.DeferredLoadingEnabled = False
            cust = tempdb.Customers.First(Function(x) x.CustomerID = "ALFKI")
        End Using

        Console.WriteLine("Customer {0}'s original phone number {1}", cust.CustomerID, cust.Phone)
        Console.WriteLine()

        For Each o In cust.Orders
            Console.WriteLine("Customer {0} has order {1} for city {2}", _
                              o.CustomerID, o.OrderID, o.ShipCity)
        Next

        Dim orderA = cust.Orders.First()
        Dim orderB = cust.Orders.First(Function(x) x.OrderID > orderA.OrderID)

        Using db2 As New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

            '' Attach the first entity to the current data context, to track changes.
            db2.Customers.Attach(cust)
            '' Attach the related orders for tracking; otherwise they will be inserted on submit.
            db2.Orders.AttachAll(cust.Orders.ToList())

            '' Update the customer.
            cust.Phone = "2345 5436"
            '' Update the first order.
            orderA.ShipCity = "Redmond"
            '' Remove the second order.
            cust.Orders.Remove(orderB)
            '' Create a new order and add it to the customer.
            Dim orderC = New Order() With {.ShipCity = "New York"}
            Console.WriteLine("Adding new order")
            cust.Orders.Add(orderC)

            '' Now submit the all changes
            db2.SubmitChanges()
        End Using


        '' Verify that the changes were applied a expected.
        Using db3 As New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

            Dim newCust = db3.Customers.First(Function(x) x.CustomerID = "ALFKI")
            Console.WriteLine("Customer {0}'s new phone number {1}", newCust.CustomerID, newCust.Phone)
            Console.WriteLine()

            For Each o In newCust.Orders
                Console.WriteLine("Customer {0} has order {1} for city {2}", o.CustomerID, o.OrderID, o.ShipCity)
            Next

        End Using


        CleanupInsert11()
    End Sub


    Private Sub CleanupInsert11()
        Dim alfkiOrderIDs = New Integer() {10643, 10692, 10702, 10835, 10952, 11011}

        Using tempdb As New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
            Dim c1 = tempdb.Customers.Single(Function(c) c.CustomerID = "ALFKI")
            c1.Phone = "030-0074321"

            Dim oa = tempdb.Orders.Single(Function(o) o.OrderID = 10643)
            oa.ShipCity = "Berlin"

            Dim ob = tempdb.Orders.Single(Function(o) o.OrderID = 10692)
            ob.CustomerID = "ALFKI"

            For Each o In c1.Orders.Where(Function(p) Not alfkiOrderIDs.Contains(p.OrderID))
                tempdb.Orders.DeleteOnSubmit(o)
            Next
            tempdb.SubmitChanges()
        End Using

    End Sub

    <Category("Simultaneous Changes")> _
    <Title("Optimistic Concurrency - 1")> _
    <Description("This and the following sample demonstrate optimistic concurrency.  In this sample, " & _
                 "the other user makes and commits his update to Product 1 before you read the data " & _
                 "so no conflict occurs.")> _
    Public Sub LinqToSqlSimultaneous01()
        Console.WriteLine("OTHER USER: ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")

        ' Open a second connection to the database to simulate another user
        ' who is going to make changes to the Products table                
        Dim otherUser_db As NorthwindDataContext = New NorthwindDataContext()
        otherUser_db.Log = db.Log

        Dim otherUser_product = (From prod In otherUser_db.Products _
                                 Where prod.ProductID = 1).First()

        otherUser_product.UnitPrice = 999.99D
        otherUser_db.SubmitChanges()

        Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")


        Console.WriteLine()
        Console.WriteLine("YOU:  ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")

        Dim product = (From prod In db.Products _
                       Where prod.ProductID = 1).First()

        product.UnitPrice = 777.77D

        Dim conflict As Boolean = False
        Try
            db.SubmitChanges()
        Catch x As DBConcurrencyException
            conflict = True
        End Try

        Console.WriteLine()
        If (conflict) Then
            Console.WriteLine("* * * OPTIMISTIC CONCURRENCY EXCEPTION * * *")
            Console.WriteLine("Another user has changed Product 1 since it was first requested.")
            Console.WriteLine("Backing out changes.")
        Else
            Console.WriteLine("* * * COMMIT SUCCESSFUL * * *")
            Console.WriteLine("Changes to Product 1 saved.")
        End If

        Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")



        Cleanup63()  ' Restore previous database state
    End Sub

    Private Sub Cleanup63()
        clearDBCache()
        setLogging(False)

        Dim product = (From prod In db.Products _
                       Where prod.ProductID = 1).First()

        product.UnitPrice = 18D
        db.SubmitChanges()
    End Sub

    <Category("Simultaneous Changes")> _
    <Title("Optimistic Concurrency - 2")> _
    <Description("This and the previous sample demonstrate optimistic concurrency.  In this sample, " & _
                 "the other user makes and commits his update to Product 1 after you read the data, " & _
                 "but before completing your update, causing an optimistic concurrency conflict.  " & _
                 "Your changes are rolled back, allowing you to retrieve the newly updated data " & _
                 "from the database and decide how to proceed with your own update.")> _
    Public Sub LinqToSqlSimultaneous02()
        Console.WriteLine("YOU:  ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")

        Dim product = (From prod In db.Products _
                       Where prod.ProductID = 1 _
                       Select prod).First()

        Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")


        Console.WriteLine()
        Console.WriteLine("OTHER USER: ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")

        ' Open a second connection to the database to simulate another user
        ' who is going to make changes to the Products table                
        Dim otherUser_db As NorthwindDataContext = New NorthwindDataContext()
        otherUser_db.Log = db.Log

        Dim otherUser_product = (From prod In otherUser_db.Products _
                                 Where prod.ProductID = 1 _
                                 Select prod).First()
        otherUser_product.UnitPrice = 999.99D
        otherUser_db.SubmitChanges()

        Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")


        Console.WriteLine()
        Console.WriteLine("YOU (continued): ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")

        product.UnitPrice = 777.77D

        Dim conflict As Boolean = False
        Try
            db.SubmitChanges()
        Catch x As DBConcurrencyException
            conflict = True
        End Try

        Console.WriteLine()
        If (conflict) Then
            Console.WriteLine("* * * OPTIMISTIC CONCURRENCY EXCEPTION * * *")
            Console.WriteLine("Another user has changed Product 1 since it was first requested.")
            Console.WriteLine("Backing out changes.")
        Else
            Console.WriteLine("* * * COMMIT SUCCESSFUL * * *")
            Console.WriteLine("Changes to Product 1 saved.")
        End If

        Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ")



        Cleanup64()  ' Restore previous database state
    End Sub

    Private Sub Cleanup64()
        clearDBCache()
        setLogging(False)

        ' transaction failure will roll data back automatically
    End Sub

    <Category("Simultaneous Changes")> _
    <Title("Transactions - Implicit")> _
    <Description("This sample demonstrates the implicit transaction created by " & _
                 "SubmitChanges.  The update to prod2's UnitsInStock field " & _
                 "makes its value negative, which violates a check constraint " & _
                 "on the server.  This causes the transaction that is updating " & _
                 "both Products to fail, which rolls back all changes.")> _
    Public Sub LinqToSqlSimultaenous03()
        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(From prod In db.Products _
                           Where prod.ProductID = 4)

        ObjectDumper.Write(From prod In db.Products _
                           Where prod.ProductID = 5)

        Console.WriteLine()
        Console.WriteLine("*** UPDATE WITH IMPLICIT TRANSACTION ***")
        Try
            Dim prod1 As Product = (From prod In db.Products _
                                    Where prod.ProductID = 4 _
                                    Select prod).First()
            Dim prod2 As Product = (From prod In db.Products _
                                    Where prod.ProductID = 5 _
                                    Select prod).First()
            prod1.UnitsInStock = prod1.UnitsInStock - 3S
            prod2.UnitsInStock = prod2.UnitsInStock - 5S    ' ERROR: this will make the units in stock negative
            ' db.SubmitChanges implicitly uses a transaction so that
            ' either both updates are accepted or both are rejected
            db.SubmitChanges()
        Catch e As Exception
            Console.WriteLine(e.Message)
        End Try


        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        clearDBCache()
        ObjectDumper.Write(From prod In db.Products _
                           Where prod.ProductID = 4 _
                           Select prod)
        ObjectDumper.Write(From prod In db.Products _
                           Where prod.ProductID = 5 _
                           Select prod)



        Cleanup95()  ' Restore previous database state
    End Sub

    Private Sub Cleanup95()
        setLogging(False)

        ' transaction failure will roll data back automatically
    End Sub

    <Category("Simultaneous Changes")> _
    <Title("Transactions - Explicit")> _
    <Description("This sample demonstrates using an explicit transaction.  This " & _
                 "provides more protection by including the reading of the data in the " & _
                 "transaction to help prevent optimistic concurrency exceptions.  " & _
                 "As in the previous query, the update to prod2's UnitsInStock field " & _
                 "makes the value negative, which violates a check constraint within " & _
                 "the database.  This causes the transaction that is updating both " & _
                 "Products to fail, which rolls back all changes.")> _
    Public Sub LinqToSqlSimultaneous04()
        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(From prod In db.Products _
                           Where prod.ProductID = 4 _
                           Select prod)
        ObjectDumper.Write(From prod In db.Products _
                           Where prod.ProductID = 5 _
                           Select prod)


        Console.WriteLine()
        Console.WriteLine("*** UPDATE WITH EXPLICIT TRANSACTION ***")
        ' Explicit use of TransactionScope ensures that
        ' the data will not change in the database between
        ' read and write

        Using ts As TransactionScope = New TransactionScope()
            Try
                Dim prod1 As Product = (From p In db.Products _
                                        Where p.ProductID = 4 _
                                        Select p).First()
                Dim prod2 As Product = (From prod In db.Products _
                                        Where prod.ProductID = 5 _
                                        Select prod).First()
                prod1.UnitsInStock = prod1.UnitsInStock - 3S
                prod2.UnitsInStock = prod2.UnitsInStock - 5S    ' ERROR: this will make the units in stock negative
                db.SubmitChanges()
            Catch e As Exception
                Console.WriteLine(e.Message)
            End Try
        End Using

        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        clearDBCache()
        ObjectDumper.Write(From prod In db.Products _
                           Where prod.ProductID = 4 _
                           Select prod)
        ObjectDumper.Write(From prod In db.Products _
                           Where prod.ProductID = 5 _
                           Select prod)

        Cleanup66()  ' Restore previous database state
    End Sub

    Private Sub Cleanup66()
        setLogging(False)

        ' transaction failure will roll data back automatically
    End Sub

    <Category("NULL")> _
    <Title("Handling NULL (Nothing in VB)")> _
    <Description("This sample uses the Nothing value to find Employees " & _
                 "that do not report to another Employee.")> _
    Public Sub LinqToSqlNull01()
        Dim empQuery = From emp In db.Employees _
                       Where emp.ReportsTo Is Nothing

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("NULL")> _
    <Title("Nullable(Of T).HasValue")> _
    <Description("This sample uses Nullable(Of T).HasValue to find Employees " & _
                 "that do not report to another Employee.")> _
    Public Sub LinqToSqlNull02()
        Dim empQuery = From emp In db.Employees _
                       Where Not emp.ReportsTo.HasValue _
                       Select emp

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("NULL")> _
    <Title("Nullable(Of T).Value")> _
    <Description("This sample uses Nullable(Of T).Value for Employees " & _
                 "that report to another Employee to return the " & _
                 "EmployeeID number of that employee.  Note that " & _
                 "the .Value is optional.")> _
    Public Sub LinqToSqlNull03()
        Dim empQuery = From emp In db.Employees _
                       Where emp.ReportsTo.HasValue _
                       Select emp.FirstName, emp.LastName, ReportsTo = emp.ReportsTo.Value

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String Concatenation")> _
    <Description("This sample uses the & operator to concatenate string fields " & _
                 "and string literals in forming the Customers' calculated " & _
                 "Location value.")> _
    Public Sub LinqToSqlString01()
        Dim custQuery = From cust In db.Customers _
                        Select cust.CustomerID, _
                               Location = cust.City & ", " & cust.Country

        ObjectDumper.Write(custQuery, 1)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.Length")> _
    <Description("This sample uses the Length property to find all Products whose " & _
                  "name is shorter than 10 characters.")> _
     Public Sub LinqToSqlString02()
        Dim shortProducts = From prod In db.Products _
                            Where prod.ProductName.Length < 10

        ObjectDumper.Write(shortProducts)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.Contains(substring)")> _
    <Description("This sample uses the Contains method to find all Customers whose " & _
                 "contact name contains 'Anders'.")> _
    Public Sub LinqToSqlString03()
        Dim custQuery = From cust In db.Customers _
                        Where cust.ContactName.Contains("Anders")

        ObjectDumper.Write(custQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.IndexOf(substring)")> _
    <Description("This sample uses the IndexOf method to find the first instance of " & _
                 "a space in each Customer's contact name.")> _
    Public Sub LinqToSqlString04()
        Dim custQuery = From cust In db.Customers _
                        Select cust.ContactName, SpacePos = cust.ContactName.IndexOf(" ")

        ObjectDumper.Write(custQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.StartsWith(prefix)")> _
    <Description("This sample uses the StartsWith method to find Customers whose " & _
                 "contact name starts with 'Maria'.")> _
    Public Sub LinqToSqlString05()
        Dim custQuery = From cust In db.Customers _
                        Where cust.ContactName.StartsWith("Maria")

        ObjectDumper.Write(custQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.EndsWith(suffix)")> _
    <Description("This sample uses the StartsWith method to find Customers whose " & _
                 "contact name ends with 'Anders'.")> _
    Public Sub LinqToSqlString06()
        Dim custQuery = From cust In db.Customers _
                        Where cust.ContactName.EndsWith("Anders")

        ObjectDumper.Write(custQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.Substring(start)")> _
    <Description("This sample uses the Substring method to return Product names starting " & _
                 "from the fourth letter.")> _
    Public Sub LinqToSqlString07()
        Dim prodQuery = From prod In db.Products _
                        Select prod.ProductName.Substring(3)

        ObjectDumper.Write(prodQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.Substring(start, length)")> _
    <Description("This sample uses the Substring method to find Employees whose " & _
                 "home phone numbers have '555' as the seventh through ninth digits.")> _
    Public Sub LinqToSqlString08()
        Dim empQuery = From emp In db.Employees _
                       Where emp.HomePhone.Substring(6, 3) = "555"

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.ToUpper()")> _
    <Description("This sample uses the ToUpper method to return Employee names " & _
                 "where the last name has been converted to uppercase.")> _
    Public Sub LinqToSqlString09()
        Dim empQuery = From emp In db.Employees _
                       Select LastName = emp.LastName.ToUpper(), emp.FirstName

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.ToLower()")> _
    <Description("This sample uses the ToLower method to return Category names " & _
                 "that have been converted to lowercase.")> _
    Public Sub LinqToSqlString10()
        Dim categoryQuery = From category In db.Categories _
                            Select category.CategoryName.ToLower()

        ObjectDumper.Write(categoryQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.Trim()")> _
    <Description("This sample uses the Trim method to return the first five " & _
                 "digits of Employee home phone numbers, with leading and " & _
                 "trailing spaces removed.")> _
    Public Sub LinqToSqlString11()
        Dim empQuery = From emp In db.Employees _
                       Select emp.HomePhone.Substring(0, 5).Trim()

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.Insert(pos, str)")> _
    <Description("This sample uses the Insert method to return a sequence of " & _
                 "employee phone numbers that have a ) in the fifth position, " & _
                 "inserting a : after the ).")> _
    Public Sub LinqToSqlString12()
        Dim empQuery = From emp In db.Employees _
                       Where emp.HomePhone.Substring(4, 1) = ")" _
                       Select emp.HomePhone.Insert(5, ":")

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.Remove(start)")> _
    <Description("This sample uses the Insert method to return a sequence of " & _
                 "employee phone numbers that have a ) in the fifth position, " & _
                 "removing all characters starting from the tenth character.")> _
    Public Sub LinqToSqlString13()
        Dim empQuery = From emp In db.Employees _
                       Where emp.HomePhone.Substring(4, 1) = ")" _
                       Select emp.HomePhone.Remove(9)

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.Remove(start, length)")> _
    <Description("This sample uses the Insert method to return a sequence of " & _
                 "employee phone numbers that have a ) in the fifth position, " & _
                 "removing the first six characters.")> _
    Public Sub LinqToSqlString14()
        Dim empQuery = From emp In db.Employees _
                       Where emp.HomePhone.Substring(4, 1) = ")" _
                       Select emp.HomePhone.Remove(0, 6)

        ObjectDumper.Write(empQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("String.Replace(find, replace)")> _
    <Description("This sample uses the Replace method to return a sequence of " & _
                 "Supplier information where the Country field has had " & _
                 "UK replaced with United Kingdom and USA replaced with " & _
                 "United States of America.")> _
    Public Sub LinqToSqlString15()
        Dim supplierQuery = From supplier In db.Suppliers _
                            Select supplier.CompanyName, _
                                   Country = supplier.Country.Replace("UK", "United Kingdom") _
                                                             .Replace("USA", "United States of America")

        ObjectDumper.Write(supplierQuery)
    End Sub

    <Category("String/Date Functions")> _
    <Title("DateTime.Year")> _
    <Description("This sample uses the DateTime's Year property to " & _
                 "find Orders placed in 1997.")> _
    Public Sub LinqToSqlString16()
        Dim ordersIn97 = From ord In db.Orders _
                         Where ord.OrderDate.Value.Year = 1997

        ObjectDumper.Write(ordersIn97)
    End Sub

    <Category("String/Date Functions")> _
    <Title("DateTime.Month")> _
    <Description("This sample uses the DateTime's Month property to " & _
                 "find Orders placed in December.")> _
    Public Sub LinqToSqlString17()
        Dim decemberOrders = From ord In db.Orders _
                             Where ord.OrderDate.Value.Month = 12

        ObjectDumper.Write(decemberOrders)
    End Sub

    <Category("String/Date Functions")> _
    <Title("DateTime.Day")> _
    <Description("This sample uses the DateTime's Day property to " & _
                 "find Orders placed on the 31st day of the month.")> _
    Public Sub LinqToSqlString18()
        Dim ordQuery = From ord In db.Orders _
                       Where ord.OrderDate.Value.Day = 31

        ObjectDumper.Write(ordQuery)
    End Sub

    <Category("Object Identity")> _
    <Title("Object Caching - 1")> _
    <Description("This sample demonstrates how, upon executing the same query twice, " & _
                 "you will receive a reference to the same object in memory each time.")> _
    Public Sub LinqToSqlObjectIdentity01()
        Dim cust1 = db.Customers.First(Function(cust) cust.CustomerID = "BONAP")
        Dim cust2 = (From cust In db.Customers _
                                 Where cust.CustomerID = "BONAP").First()

        Console.WriteLine("cust1 and cust2 refer to the same object in memory: " & _
                          Object.ReferenceEquals(cust1, cust2))
    End Sub

    <Category("Object Identity")> _
    <Title("Object Caching - 2")> _
    <Description("This sample demonstrates how, upon executing different queries that " & _
                 "return the same row from the database, you will receive a " & _
                 "reference to the same object in memory each time.")> _
    Public Sub LinqToSqlObjectIdentity02()
        Dim cust1 = db.Customers.First(Function(cust) cust.CustomerID = "BONAP")
        Dim cust2 = (From ord In db.Orders _
                     Where ord.Customer.CustomerID = "BONAP").First().Customer

        Console.WriteLine("cust1 and cust2 refer to the same object in memory: " & _
                          Object.ReferenceEquals(cust1, cust2))
    End Sub

    <Category("Object Loading")> _
    <Title("Deferred Loading - 1")> _
    <Description("This sample demonstrates how navigating through relationships in " & _
                 "retrieved objects can end up triggering new queries to the database " & _
                 "if the data was not requested by the original query.")> _
    Public Sub LinqToSqlObject01()
        Dim custs = From cust In db.Customers _
                    Where cust.City = "Sao Paulo" _
                    Select cust

        For Each cust In custs
            For Each ord In cust.Orders
                Console.WriteLine("CustomerID " & cust.CustomerID & " has an OrderID " & ord.OrderID)
            Next
        Next
    End Sub

    <Category("Object Loading")> _
    <Title("LoadWith - Eager Loading - 1")> _
    <Description("This sample demonstrates how to use LoadWith to request related " & _
                 "data during the original query so that additional roundtrips to the " & _
                 "database are not required later when navigating through " & _
                 "the retrieved objects.")> _
    Public Sub LinqToSqlObject02()

        Dim db2 = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        db2.Log = Me.OutputStreamWriter

        Dim ds = New DataLoadOptions()
        ds.LoadWith(Of Customer)(Function(cust) cust.Orders)

        db2.LoadOptions = ds

        Dim custs = From cust In db.Customers _
                    Where cust.City = "Sao Paulo"

        For Each cust In custs
            For Each ord In cust.Orders
                Console.WriteLine("CustomerID " & cust.CustomerID & " has an OrderID " & ord.OrderID)
            Next
        Next

    End Sub

    <Category("Object Loading")> _
    <Title("Deferred Loading + AssociateWith")> _
    <Description("This sample demonstrates how navigating through relationships in " & _
                 "retrieved objects can end up triggering new queries to the database " & _
                 "if the data was not requested by the original query. Also this sample shows relationship " & _
                 "objects can be filtered using AssoicateWith when they are deferred loaded.")> _
    Public Sub LinqToSqlObject03()

        Dim db2 As New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        db2.Log = Me.OutputStreamWriter

        Dim ds As New DataLoadOptions()
        ds.AssociateWith(Of Customer)(Function(p) p.Orders.Where(Function(o) o.ShipVia.Value > 1))

        db2.LoadOptions = ds

        Dim custs = From cust In db2.Customers _
                    Where cust.City = "London"

        For Each cust In custs
            For Each ord In cust.Orders
                For Each orderDetail In ord.Order_Details
                    Console.WriteLine("CustomerID {0} has an OrderID {1} that ShipVia is {2} with ProductID {3} that has name {4}.", _
                        cust.CustomerID, ord.OrderID, ord.ShipVia, orderDetail.ProductID, orderDetail.Product.ProductName)
                Next
            Next
        Next
    End Sub

    <Category("Object Loading")> _
    <Title("LoadWith - Eager Loading + Associate With")> _
    <Description("This sample demonstrates how to use LoadWith to request related " & _
                 "data during the original query so that additional roundtrips to the " & _
                 "database are not required later when navigating through " & _
                 "the retrieved objects. Also this sample shows relationship" & _
                 "objects can be ordered by using Assoicate With when they are eager loaded.")> _
    Public Sub LinqToSqlObject04()

        Dim db2 = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        db2.Log = Me.OutputStreamWriter

            
        Dim ds As New DataLoadOptions()
        ds.LoadWith(Of Customer)(Function(cust) cust.Orders)
        ds.LoadWith(Of Order)(Function(ord) ord.Order_Details)

        ds.AssociateWith(Of Order)(Function(p) p.Order_Details.OrderBy(Function(o) o.Quantity))

        db2.LoadOptions = ds

        Dim custs = From cust In db.Customers _
                    Where cust.City = "London"

        For Each cust In custs
            For Each ord In cust.Orders
                For Each orderDetail In ord.Order_Details
                    Console.WriteLine("CustomerID {0} has an OrderID {1} with ProductID {2} that has quantity {3}.", _
                        cust.CustomerID, ord.OrderID, orderDetail.ProductID, orderDetail.Quantity)
                Next
            Next
        Next

    End Sub


    Private Function isValidProduct(ByVal prod As Product) As Boolean
        Return (prod.ProductName.LastIndexOf("C") = 0)
    End Function

    <Category("Object Loading")> _
    <Title("Deferred Loading - (1:M)")> _
    <Description("This sample demonstrates how navigating through relationships in " & _
                 "retrieved objects can result in triggering new queries to the database " & _
                 "if the data was not requested by the original query.")> _
    Public Sub LinqToSqlObject05()
        Dim emps = db.Employees

        For Each emp In emps
            For Each man In emp.Employees
                Console.WriteLine("Employee " & emp.FirstName & " reported to Manager " & man.FirstName)
            Next
        Next
    End Sub

    <Category("Object Loading")> _
    <Title("Deferred Loading - (BLOB)")> _
    <Description("This sample demonstrates how navigating through Link in " & _
                 "retrieved objects can end up triggering new queries to the database " & _
                 "if the data type is Link.")> _
    Public Sub LinqToSqlObject06()
        Dim emps = db.Employees

        For Each emp In emps
            Console.WriteLine(emp.Notes)
        Next

    End Sub


    <Category("Object Loading")> _
    <Title("Load Override")> _
    <Description("This samples overrides the partial method LoadProducts in Category class. When products of a category are being loaded, " & _
                 "LoadProducts is being called to load products that are not discontinued in this category. ")> _
    Public Sub LinqToSqlObject07()

        Dim db2 As New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

        Dim ds As New DataLoadOptions()

        ds.LoadWith(Of Category)(Function(p) p.Products)
        db2.LoadOptions = ds

        Dim q = From c In db2.Categories _
                Where c.CategoryID < 3

        For Each cat In q
            For Each prod In cat.Products
                Console.WriteLine("Category {0} has a ProductID {1} that Discontined = {2}.", _
                                  cat.CategoryID, prod.ProductID, prod.Discontinued)
            Next
        Next

    End Sub


    <Category("Conversion Operators")> _
    <Title("AsEnumerable")> _
    <Description("This sample uses ToArray so that the client-side IEnumerable(Of T) " & _
                 "implementation of Where is used, instead of the default Query(Of T) " & _
                 "implementation which would be converted to SQL and executed " & _
                 "on the server.  This is necessary because the where clause " & _
                 "references a user-defined client-side method, isValidProduct, " & _
                 "which cannot be converted to SQL.")> _
    <LinkedFunction("isValidProduct")> _
    Public Sub LinqToSqlConversions01()
        Dim prodQuery = From prod In db.Products.AsEnumerable() _
                        Where isValidProduct(prod)

        ObjectDumper.Write(prodQuery)
    End Sub

    <Category("Conversion Operators")> _
    <Title("ToArray")> _
    <Description("This sample uses ToArray to immediately evaluate a query into an array " & _
                 "and get the 3rd element.")> _
    Public Sub LinqToSqlConversions02()
        Dim londonCustomers = From cust In db.Customers _
                              Where cust.City = "London"

        Dim custArray = londonCustomers.ToArray()
        ObjectDumper.Write(custArray(3), 0)
    End Sub

    <Category("Conversion Operators")> _
    <Title("ToList")> _
    <Description("This sample uses ToList to immediately evaluate a query into a List(Of T).")> _
    Public Sub LinqToSqlConversions03()
        Dim hiredAfter1994 = From emp In db.Employees _
                             Where emp.HireDate >= #1/1/1994#

        Dim qList = hiredAfter1994.ToList()
        ObjectDumper.Write(qList, 0)
    End Sub

    <Category("Conversion Operators")> _
    <Title("ToDictionary")> _
    <Description("This sample uses ToDictionary to immediately evaluate a query and " & _
                 "a key expression into an Dictionary(Of K, T).")> _
    Public Sub LinqToSqlConversion04()
        Dim prodQuery = From prod In db.Products _
                        Where prod.UnitsInStock <= prod.ReorderLevel _
                              AndAlso Not prod.Discontinued

        Dim qDictionary = prodQuery.ToDictionary(Function(prod) prod.ProductID)

        For Each key In qDictionary.Keys
            Console.WriteLine("Key " & key & ":")
            ObjectDumper.Write(qDictionary(key))
            Console.WriteLine()
        Next
    End Sub


    <Category("Direct SQL")> _
    <Title("SQL Query")> _
    <Description("This sample uses ExecuteQuery(Of T) to execute an arbitrary SQL query, " & _
                 "mapping the resulting rows to a sequence of Product objects.")> _
    Public Sub LinqToSqlDirect01()
        Dim products = db.ExecuteQuery(Of Product)( _
            "SELECT [Product List].ProductID, [Product List].ProductName " & _
            "FROM Products AS [Product List] " & _
            "WHERE [Product List].Discontinued = 0 " & _
            "ORDER BY [Product List].ProductName;")

        ObjectDumper.Write(products)
    End Sub

    <Category("Direct SQL")> _
    <Title("SQL Command")> _
    <Description("This sample uses ExecuteCommand to execute an arbitrary SQL command, " & _
                 "in this case a mass update to increase all Products' unit price by 1.00.")> _
    Public Sub LinqToSqlDirect02()
        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(From prod In db.Products _
                           Select prod)


        Console.WriteLine()
        Console.WriteLine("*** UPDATE ***")
        db.ExecuteQuery(Of Object)("UPDATE Products SET UnitPrice = UnitPrice + 1.00")

        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        clearDBCache()
        ObjectDumper.Write(From prod In db.Products _
                           Select prod)

        Cleanup99()  ' Restore previous database state
    End Sub

    Private Sub Cleanup99()
        setLogging(False)
    End Sub

    <Category("ADO.NET Interop")> _
    <Title("Connection Interop")> _
    <Description("This sample uses a pre-existing ADO.NET connection to create a Northwind " & _
                 "object that can be used to perform queries, in this case a query to return " & _
                 "all orders with freight of at least 500.00.")> _
    Public Sub LinqToSqlAdo01()
        ' Create a standard ADO.NET connection:
        Dim nwindConn As SqlConnection = New SqlConnection(My.Settings.NORTHWINDConnectionString1)
        nwindConn.Open()

        ' ... other ADO.NET database access code ... '

        ' Use pre-existing ADO.NET connection to create DataContext:
        Dim interop_db = New NorthwindDataContext(nwindConn) With {.Log = db.Log}

        Dim orders = From ord In interop_db.Orders _
                     Where ord.Freight > 500D _
                     Select ord

        ObjectDumper.Write(orders)

        nwindConn.Close()
    End Sub

    <Category("ADO.NET Interop")> _
    <Title("Transaction Interop")> _
    <Description("This sample uses a pre-existing ADO.NET connection to create a Northwind " & _
                 "object and then shares an ADO.NET transaction with it.  The transaction is " & _
                 "used both to execute SQLCommands through the ADO.NET connection and to submit " & _
                 "changes through the Northwind object.  When the transaction aborts due to a " & _
                 "violated check constraint, all changes are rolled back, including both the " & _
                 "changes made through the SqlCommand and the changes made through the " & _
                 "Northwind object.")> _
    Public Sub LinqToSqlAdo02()
        Dim prodQuery = From prod In db.Products _
                        Where prod.ProductID = 3

        Console.WriteLine("*** BEFORE ***")
        ObjectDumper.Write(prodQuery)


        Console.WriteLine()
        Console.WriteLine("*** INSERT ***")

        ' Create a standard ADO.NET connection:
        Dim nwindConn As SqlConnection = New SqlConnection(My.Settings.NORTHWINDConnectionString1)
        nwindConn.Open()

        ' Use pre-existing ADO.NET connection to create DataContext:
        Dim interop_db As NorthwindDataContext = New NorthwindDataContext(nwindConn)
        interop_db.Log = db.Log

        Dim nwindTxn As SqlTransaction = nwindConn.BeginTransaction()

        Try
            Dim cmd As SqlCommand = New SqlCommand("UPDATE Products SET QuantityPerUnit = 'single item' WHERE ProductID = 3")
            cmd.Connection = nwindConn
            cmd.Transaction = nwindTxn
            cmd.ExecuteNonQuery()

            interop_db.Transaction = nwindTxn

            Dim prod1 = (From prod In interop_db.Products _
                         Where prod.ProductID = 4).First()
            Dim prod2 = (From prod In interop_db.Products _
                         Where prod.ProductID = 5).First()

            prod1.UnitsInStock = prod1.UnitsInStock - 3S
            prod2.UnitsInStock = prod2.UnitsInStock - 5S    ' ERROR: this will make the units in stock negative

            interop_db.SubmitChanges()

            nwindTxn.Commit()
        Catch e As Exception
            ' If there is a transaction error, all changes are rolled back,
            ' including any changes made directly through the ADO.NET connection
            Console.WriteLine(e.Message)
            Console.WriteLine("Error submitting changes... all changes rolled back.")
        End Try

        nwindConn.Close()


        Console.WriteLine()
        Console.WriteLine("*** AFTER ***")
        clearDBCache()
        ObjectDumper.Write(prodQuery)



        Cleanup101()  ' Restore previous database state
    End Sub


    Private Sub Cleanup101()
        setLogging(False)

        ' transaction failure will roll data back automatically
    End Sub

    <Category("Stored Procedures")> _
    <Title("Scalar Return")> _
    <Description("This sample uses a stored procedure to return the number of Customers in the 'WA' Region.")> _
    Public Sub LinqToSqlStoredProc01()
        Dim count = db.Customers_Count_By_Region("WA")

        Console.WriteLine(count)
    End Sub

    <Category("Stored Procedures")> _
    <Title("Single Resultset")> _
    <Description("This sample uses a method mapped to the 'Customers By City' stored procedure " & _
                 "in Northwind database to return customers from 'London'.  " & _
                 "Methods can be created by dragging stored procedures from the Server " & _
                 "Explorer onto the O/R Designer which can be accessed by double-clicking " & _
                 "on .DBML file in the Solution Explorer.")> _
    Public Sub LinqToSqlStoredProc02()
        Dim custQuery = db.Customers_By_City("London")

        ObjectDumper.Write(custQuery, 0)
    End Sub

    <Category("Stored Procedures")> _
    <Title("Multiple Result-Sets")> _
    <Description("This sample uses a stored procedure to return the Customer 'SEVES' and all it's Orders.")> _
    Public Sub LinqToSqlStoredProc04()
        Dim result = db.Get_Customer_And_Orders("SEVES")

        Console.WriteLine("********** Customer Result-set ***********")
        Dim customer As IEnumerable(Of Get_Customer_And_OrdersResult) = result
        ObjectDumper.Write(customer)
        Console.WriteLine()

    End Sub

    <Category("Stored Procedures")> _
    <Title("Out parameters")> _
    <Description("This sample uses a stored procedure that returns an out parameter.")> _
    Public Sub LinqToSqlStoredProc05()
        Dim totalSales? = 0@

        Dim customerID = "ALFKI"

        ' Out parameters are passed by ByRef, to support scenarios where
        ' the parameter is In or Out.  In this case, the parameter is only
        ' out.
        db.CustOrderTotal(customerID, totalSales)

        Console.WriteLine("Total Sales for Customer '{0}' = {1:C}", customerID, totalSales)
    End Sub

    <Category("Stored Procedures")> _
    <Title("Function")> _
    <Description("This sample uses a method mapped to the 'ProductsUnderThisUnitPrice' function " & _
                 "in Northwind database to return products with unit price less than $10.00. " & _
                 "Methods can be created by dragging database functions from the Server " & _
                 "Explorer onto the O/R Designer which can be accessed by double-clicking " & _
                 "on the .DBML file in the Solution Explorer.")> _
    Public Sub LinqToSqlStoredProc06()
        Dim cheapProducts = db.ProductsUnderThisUnitPrice(10D)

        ObjectDumper.Write(cheapProducts, 0)
    End Sub

    <Category("Stored Procedures")> _
    <Title("Query over methods")> _
    <Description("This sample queries against a collection of products returned by " & _
                 "'ProductsUnderThisUnitPrice' method. The method was created from the database  " & _
                 "function 'ProductsUnderThisUnitPrice' in Northwind database. ")> _
    Public Sub LinqToSqlStoredProc07()
        Dim cheapProducts = From prod In db.ProductsUnderThisUnitPrice(10D) _
                            Where prod.Discontinued = True

        ObjectDumper.Write(cheapProducts, 0)
    End Sub

    <Category("User-Defined Functions")> _
    <Title("Scalar Function - Select")> _
    <Description("This sample demonstrates using a scalar user-defined function in a projection.")> _
    Public Sub LinqToSqlUserDefined01()
        Dim catQuery = From category In db.Categories _
                       Select category.CategoryID, _
                              TotalUnitPrice = db.TotalProductUnitPriceByCategory(category.CategoryID)

        ObjectDumper.Write(catQuery)
    End Sub

    <Category("User-Defined Functions")> _
    <Title("Scalar Function - Where")> _
    <Description("This sample demonstrates using a scalar user-defined function in a Where clause.")> _
    Public Sub LinqToSqlUserDefined02()

        Dim prodQuery = From prod In db.Products _
                        Where prod.UnitPrice = db.MinUnitPriceByCategory(prod.CategoryID)

        ObjectDumper.Write(prodQuery)
    End Sub

    <Category("User-Defined Functions")> _
    <Title("Table-Valued Function")> _
    <Description("This sample demonstrates selecting from a table-valued user-defined function.")> _
    Public Sub LinqToSqlUserDefined03()

        Dim prodQuery = From p In db.ProductsUnderThisUnitPrice(10.25D) _
                        Where Not p.Discontinued

        ObjectDumper.Write(prodQuery)
    End Sub

    <Category("User-Defined Functions")> _
    <Title("Table-Valued Function - Join")> _
    <Description("This sample demonstrates joining to the results of a table-valued user-defined function.")> _
    Public Sub LinqToSqlUserDefined04()

        Dim q = From category In db.Categories _
                Group Join prod In db.ProductsUnderThisUnitPrice(8.5D) _
                      On category.CategoryID Equals prod.CategoryID _
                Into prods = Group _
                From prod2 In prods _
                Select category.CategoryID, category.CategoryName, _
                       prod2.ProductName, prod2.UnitPrice

        ObjectDumper.Write(q)
    End Sub

    <Category("DataContext Functions")> _
    <Title("CreateDatabase() and DeleteDatabase() ")> _
    <Description("This sample uses CreateDatabase() to create a new database based on the NewCreateDB Schema in Mapping.cs,  " & _
                 "and DeleteDatabase() to delete the newly created database.")> _
    Public Sub LinqToSqlDataContext01()

        ' Create a temp folder to store the new created Database 
        Dim userTempFolder = Environment.GetEnvironmentVariable("SystemDrive") + "\LinqToSqlSamplesTemp"
        Directory.CreateDirectory(userTempFolder)

        Console.WriteLine("********** Create NewCreateDB ***********")
        Dim userMDF = System.IO.Path.Combine(userTempFolder, "NewCreateDB.mdf")
        Dim connStr = "Data Source=.\SQLEXPRESS;AttachDbFilename=" & userMDF & ";Integrated Security=True;Connect Timeout=30;User Instance=True; Integrated Security = SSPI;"
        Dim newDB = New NewCreateDB(connStr)
        Try
            newDB.CreateDatabase()
        Catch ex As SqlException
            Console.WriteLine(ex.Message)
        End Try

        If (newDB.DatabaseExists() AndAlso File.Exists(Path.Combine(userTempFolder, "NewCreateDB.mdf"))) Then
            Console.WriteLine("NewCreateDB is created")
        Else
            Console.WriteLine("Error: NewCreateDB is not successfully created")
        End If


        Console.WriteLine()
        Console.WriteLine("************ Delete NewCreateDB **************")
        newDB.DeleteDatabase()

        If (File.Exists(Path.Combine(userTempFolder, "NewCreateDB.mdf"))) Then
            Console.WriteLine("Error: NewCreateDB is not yet deleted")
        Else
            Console.WriteLine("NewCreateDB is deleted")
        End If
        ' Delete the temp folder created for this testcase 
        Directory.Delete(userTempFolder)

    End Sub

    <Category("DataContext Functions")> _
    <Title("DatabaseExists()")> _
    <Description("This sample uses DatabaseExists() to verify whether a database exists or not.")> _
    Public Sub LinqToSqlDataContext02()

        Console.WriteLine("*********** Verify Northwind DB exists ***********")
        If (db.DatabaseExists()) Then
            Console.WriteLine("Northwind DB exists")
        Else
            Console.WriteLine("Error: Northwind DB does not exist")

            Console.WriteLine()
            Console.WriteLine("********* Verify NewCreateDB does not exist **********")
            Dim userTempFolder = Environment.GetEnvironmentVariable("Temp")
            Dim userMDF = System.IO.Path.Combine(userTempFolder, "NewCreateDB.mdf")
            Dim newDB = New NewCreateDB(userMDF)

            If (newDB.DatabaseExists()) Then
                Console.WriteLine("Error: NewCreateDB DB exists")
            Else
                Console.WriteLine("NewCreateDB DB does not exist")
            End If
        End If
    End Sub

    <Category("DataContext Functions")> _
    <Title("SubmitChanges()")> _
    <Description("This sample demonstrates that SubmitChanges() must be called in order to  " & _
                 "submit any update to the actual database.")> _
    Public Sub LinqToSql1DataContext03()
        Dim customer = db.Customers.First(Function(cust) cust.CustomerID = "ALFKI")

        Console.WriteLine("********** Original Customer CompanyName **********")
        Dim custQuery = From cust In db.Customers _
                        Where cust.CustomerID = "ALFKI" _
                        Select cust.CompanyName

        Console.WriteLine()
        ObjectDumper.Write(custQuery)

        Console.WriteLine()
        Console.WriteLine("*********** Update and call SubmitChanges() **********")

        customer.CompanyName = "VB Programming Firm"
        db.SubmitChanges()

        Console.WriteLine()
        ObjectDumper.Write(custQuery)

        Console.WriteLine()
        Console.WriteLine("*********** Update and did not call SubmitChanges() **********")

        customer.CompanyName = "LinqToSql Programming Firm"

        Console.WriteLine()
        ObjectDumper.Write(custQuery)

        Cleanup122()  ' Restore previous database state      
    End Sub

    Private Sub Cleanup122()
        setLogging(False)
        Dim cust = db.Customers.First(Function(c) c.CustomerID = "ALFKI")
        cust.CompanyName = "Alfreds Futterkiste"
        db.SubmitChanges()
    End Sub

    <Category("DataContext Functions")> _
    <Title("CreateQuery()")> _
    <Description("This sample uses CreateQuery() to create an IQueryable(Of T) out of an Expression.")> _
    Public Sub LinqToSqlDataContext04()

        Dim custParam = Expression.Parameter(GetType(Customer), "c")
        Dim city = GetType(Customer).GetProperty("City")

        Dim pred = Expression.Lambda(Of Func(Of Customer, Boolean))( _
            Expression.Equal( _
                             Expression.Property(custParam, city), Expression.Constant("Seattle") _
                             ), custParam)

        Dim custs As IQueryable = db.Customers
        Dim expr = Expression.Call(GetType(Queryable), "Where", _
                                   New Type() {custs.ElementType}, custs.Expression, pred)

        Dim custQuery = db.Customers.AsQueryable().Provider.CreateQuery(Of Customer)(expr)

        ObjectDumper.Write(custQuery)
    End Sub

    <Category("DataContext Functions")> _
    <Title("Log")> _
    <Description("This sample uses Db.Log to turn off and turn on the database logging display.")> _
    Public Sub LinqToSqlDataContext05()
        Console.WriteLine("**************** Turn off DB Log Display *****************")
        db.Log = Nothing
        Dim londonCustomers = From cust In db.Customers _
                              Where cust.City = "London"

        ObjectDumper.Write(londonCustomers)

        Console.WriteLine()
        Console.WriteLine("**************** Turn on DB Log Display  *****************")

        db.Log = Me.OutputStreamWriter
        ObjectDumper.Write(londonCustomers)


    End Sub

    <Category("Advanced")> _
    <Title("Dynamic query - Select")> _
    <Description("This sample builds a query dynamically to return the contact name of each customer.")> _
    Public Sub LinqToSqlAdvanced01()
        Dim param = Expression.Parameter(GetType(Customer), "c")
        Dim selector = Expression.Property(param, GetType(Customer).GetProperty("ContactName"))
        Dim pred = Expression.Lambda(selector, param)

        Dim custs = db.Customers
        Dim expr = Expression.Call(GetType(Queryable), "Select", New Type() {GetType(Customer), GetType(String)}, Expression.Constant(custs), pred)
        Dim query = custs.AsQueryable().Provider.CreateQuery(Of String)(expr)

        Dim cmd = db.GetCommand(query)
        Console.WriteLine("Generated T-SQL:")
        Console.WriteLine(cmd.CommandText)
        Console.WriteLine()


        ObjectDumper.Write(query)
    End Sub

    <Category("Advanced")> _
    <Title("Dynamic query - Where")> _
    <Description("This sample builds a query dynamically to filter for Customers in London.")> _
    Public Sub LinqToSqlAdvanced02()

        Dim custs = db.Customers
        Dim param = Expression.Parameter(GetType(Customer), "c")
        Dim right = Expression.Constant("London")
        Dim left = Expression.Property(param, GetType(Customer).GetProperty("City"))
        Dim filter = Expression.Equal(left, right)
        Dim pred = Expression.Lambda(filter, param)

        Dim expr = Expression.Call(GetType(Queryable), "Where", New Type() {GetType(Customer)}, Expression.Constant(custs), pred)
        Dim query = db.Customers.AsQueryable().Provider.CreateQuery(Of Customer)(expr)
        ObjectDumper.Write(query)
    End Sub


    <Category("Advanced")> _
    <Title("Dynamic query - OrderBy")> _
    <Description("This sample builds a query dynamically to filter for Customers in London" & _
                 " and order them by ContactName.")> _
    Public Sub LinqToSqlAdvanced03()

        Dim param = Expression.Parameter(GetType(Customer), "c")

        Dim left = Expression.Property(param, GetType(Customer).GetProperty("City"))
        Dim right = Expression.Constant("London")
        Dim filter = Expression.Equal(left, right)
        Dim pred = Expression.Lambda(filter, param)

        Dim custs As IQueryable = db.Customers

        Dim expr = Expression.Call(GetType(Queryable), "Where", _
                                   New Type() {GetType(Customer)}, _
                                   Expression.Constant(custs), pred)

        expr = Expression.Call(GetType(Queryable), "OrderBy", _
                               New Type() {GetType(Customer), GetType(String)}, _
                               custs.Expression, _
                               Expression.Lambda(Expression.Property(param, "ContactName"), param))


        Dim query = db.Customers.AsQueryable().Provider.CreateQuery(Of Customer)(expr)

        ObjectDumper.Write(query)
    End Sub

    <Category("Advanced")> _
    <Title("Dynamic query - Union")> _
    <Description("This sample dynamically builds a Union to return a sequence of all countries where either " & _
                 "a customer or an employee live.")> _
    Public Sub LinqToSqlAdvanced04()

        Dim custs = db.Customers
        Dim param1 = Expression.Parameter(GetType(Customer), "c")
        Dim left1 = Expression.Property(param1, GetType(Customer).GetProperty("City"))
        Dim pred1 = Expression.Lambda(left1, param1)

        Dim employees = db.Employees
        Dim param2 = Expression.Parameter(GetType(Employee), "e")
        Dim left2 = Expression.Property(param2, GetType(Employee).GetProperty("City"))
        Dim pred2 = Expression.Lambda(left2, param2)

        Dim expr1 = Expression.Call(GetType(Queryable), "Select", New Type() {GetType(Customer), GetType(String)}, Expression.Constant(custs), pred1)
        Dim expr2 = Expression.Call(GetType(Queryable), "Select", New Type() {GetType(Employee), GetType(String)}, Expression.Constant(employees), pred2)

        Dim custQuery1 = db.Customers.AsQueryable().Provider.CreateQuery(Of String)(expr1)
        Dim empQuery1 = db.Employees.AsQueryable().Provider.CreateQuery(Of String)(expr2)

        Dim finalQuery = custQuery1.Union(empQuery1)

        ObjectDumper.Write(finalQuery)
    End Sub

    <Category("Advanced")> _
    <Title("Identity")> _
    <Description("This sample demonstrates how we insert a new Contact and retrieve the " & _
                 "newly assigned ContactID from the database.")> _
    Public Sub LinqToSqlAdvanced05()

        Console.WriteLine("ContactID is marked as an identity column")
        Dim con = New Contact() With {.CompanyName = "New Era", .Phone = "(123)-456-7890"}

        db.Contacts.InsertOnSubmit(con)
        db.SubmitChanges()

        Console.WriteLine()
        Console.WriteLine("The ContactID of the new record is " & con.ContactID)

        cleanup130(con.ContactID)

    End Sub
    Sub cleanup130(ByVal contactID As Integer)
        setLogging(False)
        Dim con = db.Contacts.Where(Function(c) c.ContactID = contactID).First()
        db.Contacts.DeleteOnSubmit(con)
        db.SubmitChanges()
    End Sub


    <Category("Advanced")> _
    <Title("Nested in FROM")> _
    <Description("This sample uses OrderByDescending and Take to return the " & _
                 "discontinued products of the top 10 most expensive products.")> _
    Public Sub LinqToSqlAdvanced06()
        Dim prods = From prod In db.Products.OrderByDescending(Function(p) p.UnitPrice) _
                    Take 10 _
                    Where prod.Discontinued

        ObjectDumper.Write(prods, 0)
    End Sub

    <Category("Advanced")> _
    <Title("Mutable/Immutable Anonymous Types")> _
    <Description("" & _
                 "")> _
    Public Sub LinqToSqlAdvanced07()

        'Generates an immutable anonymous type (FirstName and LastName will be ReadOnly)
        Dim empQuery1 = From emp In db.Employees _
                        Where emp.City = "Seattle" _
                        Select emp.FirstName, emp.LastName

        For Each row In empQuery1
            'These lines would be compile errors because the projected type is immutable
            'row.FirstName = "John"
            'row.LastName = "Doe"
        Next

        'Generates a mutable anonymous type (FirstName and LastName are Read/Write)
        Dim empQuery2 = From emp In db.Employees _
                        Where emp.City = "Seattle" _
                        Select New With {emp.FirstName, emp.LastName}

        For Each row In empQuery2
            'These lines work because the projected type is mutable
            row.FirstName = "John"
            row.LastName = "Doe"
        Next

        'Generates a partially mutable anonymous type (FirstName is ReadOnly, LastName is Read/Write)
        Dim empQuery3 = From emp In db.Employees _
                        Where emp.City = "Seattle" _
                        Select New With {Key emp.FirstName, emp.LastName}

        For Each row In empQuery3
            'Only the second line works because the type is partially mutable; you can
            'only write to the LastName field
            'row.FirstName = "John"
            row.LastName = "Doe"
        Next

        'Generates an immutable anonymous type (FirstName and LastName will be ReadOnly)
        Dim empQuery4 = From emp In db.Employees _
                        Where emp.City = "Seattle" _
                        Select New With {Key emp.FirstName, Key emp.LastName}

        For Each row In empQuery4
            'These lines would be compile errors because the projected type is immutable
            'row.FirstName = "John"
            'row.LastName = "Doe"
        Next

        Console.WriteLine("By default the Select keyword generates types that are " & _
                          "immutable, however you can use the New With {...} syntax " & _
                          "to generate a mutable anonymous type.  To make individual " & _
                          "fields ReadOnly, use the Key modifier.")
    End Sub

    <Category("View")> _
    <Title("Query - Anonymous Type")> _
    <Description("This sample uses Select and Where to return a sequence of invoices " & _
                 "where the shipping city is London.")> _
    Public Sub LinqToSqlView01()
        Dim shipToLondon = From inv In db.Invoices _
                           Where inv.ShipCity = "London" _
                           Select inv.OrderID, inv.ProductName, inv.Quantity, inv.CustomerName

        ObjectDumper.Write(shipToLondon, 1)
    End Sub

    <Category("View")> _
    <Title("Query - Identity mapping")> _
    <Description("This sample uses Select to query QuarterlyOrders.")> _
    Public Sub LinqToSqlView02()
        Dim quarterlyOrders = From qo In db.Quarterly_Orders _
                              Select qo

        ObjectDumper.Write(quarterlyOrders, 1)
    End Sub

    <Category("Inheritance")> _
    <Title("Simple")> _
    <Description("This sample returns all contacts where the city is London.")> _
    Public Sub LinqToSqlInheritance01()

        Dim cons = From contact In newDB.Contacts _
                   Select contact

        For Each con In cons
            Console.WriteLine("Company name: " & con.CompanyName)
            Console.WriteLine("Phone: " & con.Phone)
            Console.WriteLine("This is a " & con.GetType().ToString)
            Console.WriteLine()
        Next

    End Sub

    <Category("Inheritance")> _
    <Title("TypeOf")> _
    <Description("This sample uses OfType to return all customer contacts.")> _
    Public Sub LinqToSqlInheritance02()

        Dim cons = From contact In newDB.Contacts.OfType(Of CustomerContact)() _
                   Select contact

        ObjectDumper.Write(cons, 0)
    End Sub

    <Category("Inheritance")> _
    <Title("IS")> _
    <Description("This sample uses IS to return all shipper contacts.")> _
    Public Sub LinqToSqlInheritance03()

        Dim cons = From contact In newDB.Contacts _
                   Where TypeOf contact Is ShipperContact _
                   Select contact

        ObjectDumper.Write(cons, 0)
    End Sub


    <Category("Inheritance")> _
    <Title("CType")> _
    <Description("This sample uses CType to return FullContact or Nothing.")> _
    Public Sub LinqToSqlInheritance04()
        Dim cons = From contact In newDB.Contacts _
                   Select CType(contact, FullContact)

        ObjectDumper.Write(cons, 0)
    End Sub

    <Category("Inheritance")> _
    <Title("Cast")> _
    <Description("This sample uses a cast to retrieve customer contacts who live in London.")> _
    Public Sub LinqToSqlInheritance05()
        Dim cons = From contact In newDB.Contacts _
                   Where contact.ContactType = "Customer" _
                         AndAlso (DirectCast(contact, CustomerContact)).City = "London"

        ObjectDumper.Write(cons, 0)
    End Sub

    <Category("Inheritance")> _
    <Title("UseAsDefault")> _
    <Description("This sample demonstrates that an unknown contact type  " & _
                 "will be automatically converted to the default contact type.")> _
    Public Sub LinqToSqlInheritance06()

        Console.WriteLine("***** INSERT Unknown Contact using normal mapping *****")
        Dim contact = New Contact() With {.ContactType = Nothing, _
                                          .CompanyName = "Unknown Company", _
                                          .City = "London", _
                                          .Phone = "333-444-5555"}
        db.Contacts.InsertOnSubmit(contact)
        db.SubmitChanges()

        Console.WriteLine()
        Console.WriteLine("***** Query Unknown Contact using inheritance mapping *****")
        Dim con = (From cont In db.Contacts _
                   Where cont.CompanyName = "Unknown Company" AndAlso _
                         cont.Phone = "333-444-5555").First()

        Console.WriteLine("The base class nwind.BaseContact had been used as default fallback type")
        Console.WriteLine("The discriminator value for con is unknown. So, its type should be " & con.GetType().ToString())

        cleanup140(contact.ContactID)

    End Sub

    Sub cleanup140(ByVal contactID As Integer)
        setLogging(False)
        Dim con = db.Contacts.Where(Function(cont) cont.ContactID = contactID).First()
        db.Contacts.DeleteOnSubmit(con)
        db.SubmitChanges()
    End Sub


    <Category("Inheritance")> _
    <Title("Insert New Record")> _
    <Description("This sample demonstrates how to create a new shipper contact.")> _
    Public Sub LinqToSqlInheritance07()

        Console.WriteLine("****** Before Insert Record ******")
        Dim ShipperContacts = From sc In newDB.Contacts.OfType(Of ShipperContact)() _
                              Where sc.CompanyName = "Northwind Shipper"

        Console.WriteLine()
        Console.WriteLine("There are " & ShipperContacts.Count() & " Shipper Contacts that matched our requirement")

        Dim nsc = New ShipperContact() With {.CompanyName = "Northwind Shipper", _
                                             .Phone = "(123)-456-7890"}
        newDB.Contacts.InsertOnSubmit(nsc)
        newDB.SubmitChanges()

        Console.WriteLine()
        Console.WriteLine("****** After Insert Record ******")
        ShipperContacts = From sc In newDB.Contacts.OfType(Of ShipperContact)() _
                          Where sc.CompanyName = "Northwind Shipper"

        Console.WriteLine()
        Console.WriteLine("There are " & ShipperContacts.Count() & " Shipper Contacts that matched our requirement")
        newDB.Contacts.DeleteOnSubmit(nsc)
        newDB.SubmitChanges()

    End Sub


    <Category("External Mapping")> _
    <Title("Load and use an External Mapping")> _
    <Description("This sample demonstrates how to create a data context that uses an external XML mapping source.")> _
    Public Sub LinqToSqlExternal01()

        ' load the mapping source
        Dim path2 = Path.Combine(Application.StartupPath, "..\..\SampleData\NorthwindMapped.map")

        Dim mappingSource As XmlMappingSource = XmlMappingSource.FromXml(File.ReadAllText(path2))


        'Notice that each type in the NorthwindMapped.map file contains the fully-qualified
        'name (i.e. it *includes* the Root Namespace).  So for example, the following element in the
        'mapping file:
        '    <Type Name="Mapped.AddressSplit">
        '
        'becomes:
        '    <Type Name="SampleQueries.Mapped.AddressSplit">
        '
        'since SampleQueries is the Root Namespace defined for this project

        ' create context using mapping source
        Dim nw As New Mapped.NorthwindMapped(db.Connection, mappingSource)

        ' demonstrate use of an externally-mapped entity 
        Console.WriteLine("****** Externally-mapped entity ******")
        Dim order As Mapped.Order = nw.Orders.First()
        ObjectDumper.Write(order, 1)

        ' demonstrate use of an externally-mapped inheritance hierarchy
        Dim contacts = From c In nw.Contacts _
                       Where TypeOf c Is Mapped.EmployeeContact _
                       Select c

        Console.WriteLine()
        Console.WriteLine("****** Externally-mapped inheritance hierarchy ******")

        For Each contact In contacts
            Console.WriteLine("Company name: {0}", contact.CompanyName)
            Console.WriteLine("Phone: {0}", contact.Phone)
            Console.WriteLine("This is a {0}", contact.GetType())
            Console.WriteLine()
        Next

        ' demonstrate use of an externally-mapped stored procedure
        Console.WriteLine()
        Console.WriteLine("****** Externally-mapped stored procedure ******")
        For Each result In nw.CustomerOrderHistory("ALFKI")
            ObjectDumper.Write(result, 0)
        Next

        ' demonstrate use of an externally-mapped scalar user defined function
        Console.WriteLine()
        Console.WriteLine("****** Externally-mapped scalar UDF ******")
        Dim totals = From c In nw.Categories _
                     Select c.CategoryID, TotalUnitPrice = nw.TotalProductUnitPriceByCategory(c.CategoryID)
        ObjectDumper.Write(totals)

        ' demonstrate use of an externally-mapped table-valued user-defined function
        Console.WriteLine()
        Console.WriteLine("****** Externally-mapped table-valued UDF ******")
        Dim products = From p In nw.ProductsUnderThisUnitPrice(9.75D) _
                       Where p.SupplierID = 8 _
                       Select p
        ObjectDumper.Write(products)
    End Sub


    <Category("Optimistic Concurrency")> _
    <Title("Get conflict information")> _
    <Description("This sample demonstrates how to retrieve the changes that lead to an optimistic concurrency exception.")> _
    Public Sub LinqToSqlOptimistic01()
        Console.WriteLine("YOU:  ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")
        Dim product = db.Products.First(Function(prod) prod.ProductID = 1)
        Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")
        Console.WriteLine()
        Console.WriteLine("OTHER USER: ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")

        ' Open a second connection to the database to simulate another user
        ' who is going to make changes to the Products table                
        Dim otherUser_db = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1) With {.Log = db.Log}
        Dim otherUser_product = otherUser_db.Products.First(Function(p) p.ProductID = 1)

        otherUser_product.UnitPrice = 999.99D
        otherUser_product.UnitsOnOrder = 10
        otherUser_db.SubmitChanges()

        Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")
        Console.WriteLine("YOU (continued): ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~")
        product.UnitPrice = 777.77D

        Dim conflictOccurred = False
        Try
            db.SubmitChanges(ConflictMode.ContinueOnConflict)
        Catch c As ChangeConflictException
            Console.WriteLine("* * * OPTIMISTIC CONCURRENCY EXCEPTION * * *")
            For Each aConflict In db.ChangeConflicts
                Dim prod = CType(aConflict.Object, Product)
                Console.WriteLine("The conflicting product has ProductID " & prod.ProductID)
                Console.WriteLine()
                Console.WriteLine("Conflicting members:")
                Console.WriteLine()
                For Each memConflict In aConflict.MemberConflicts
                    Dim name = memConflict.Member.Name
                    Dim yourUpdate = memConflict.CurrentValue.ToString()
                    Dim original = memConflict.OriginalValue.ToString()
                    Dim theirUpdate = memConflict.DatabaseValue.ToString()
                    If (memConflict.IsModified) Then

                        Console.WriteLine("'{0}' was updated from {1} to {2} while you updated it to {3}", _
                                              name, original, theirUpdate, yourUpdate)
                    Else
                        Console.WriteLine("'{0}' was updated from {1} to {2}, you did not change it.", _
                                                                            name, original, theirUpdate)
                    End If
                    Console.WriteLine()
                Next
                conflictOccurred = True
            Next

            Console.WriteLine()
            If (Not conflictOccurred) Then

                Console.WriteLine("* * * COMMIT SUCCESSFUL * * *")
                Console.WriteLine("Changes to Product 1 saved.")
            End If
            Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ")

            ResetProducts() ' clean up
        End Try
    End Sub

    Private Sub ResetProducts()
        clearDBCache()
        Dim dbClean = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        Dim prod(4) As Product
        Dim price = New Decimal() {18D, 19D, 10D, 22D}
        For i = 0 To 3
            Dim tmp = i
            prod(i) = dbClean.Products.First(Function(p) p.ProductID = tmp + 1)
            prod(i).UnitPrice = price(i)
        Next
        prod(0).UnitsInStock = 39
        prod(0).UnitsOnOrder = 0
        dbClean.SubmitChanges()
    End Sub

    'OptimisticConcurrencyConflict
    Private Sub WriteConflictDetails(ByVal conflicts As IEnumerable(Of ObjectChangeConflict))
        'OptimisticConcurrencyConflict
        For Each conflict In conflicts
            DescribeConflict(conflict)
        Next
    End Sub

    Private Sub DescribeConflict(ByVal conflict As ObjectChangeConflict)
        Dim prod = DirectCast(conflict.Object, Product)
        Console.WriteLine("Optimistic Concurrency Conflict in product " & prod.ProductID)

        'OptimisticConcurrencyMemberConflict
        For Each memConflict In conflict.MemberConflicts
            Dim name = memConflict.Member.Name
            Dim yourUpdate = memConflict.CurrentValue.ToString()
            Dim original = memConflict.OriginalValue.ToString()
            Dim theirUpdate = memConflict.DatabaseValue.ToString()
            If (memConflict.IsModified) Then
                Console.WriteLine("'{0}' was updated from {1} to {2} while you updated it to {3}", _
                                  name, original, theirUpdate, yourUpdate)
            Else
                Console.WriteLine("'{0}' was updated from {1} to {2}, you did not change it.", _
                    name, original, theirUpdate)
            End If
        Next
    End Sub

    <Category("Optimistic Concurrency")> _
    <Title("Resolve conflicts: Overwrite current values")> _
    <Description("This sample demonstrates how to automatically resolve concurrency conflicts." & vbNewLine & _
                 "The 'overwrite current values' option writes the new database values to the client objects.")> _
    Public Sub LinqToSqlOptimistic02()

        Dim otherUser_db = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        db.Log = Nothing

        Dim product = db.Products.First(Function(prod) prod.ProductID = 1)
        Console.WriteLine("You retrieve product number 1, it costs " & product.UnitPrice)
        Console.WriteLine("There are {0} units in stock, {1} units on order", product.UnitsInStock, product.UnitsOnOrder)
        Console.WriteLine()

        Console.WriteLine("Another user changes the price to 22.22 and UnitsInStock to 22")
        Dim otherUser_product = otherUser_db.Products.First(Function(prod) prod.ProductID = 1)
        otherUser_product.UnitPrice = 22.22D
        otherUser_product.UnitsInStock = 22
        otherUser_db.SubmitChanges()

        Console.WriteLine("You set the price of product 1 to 1.01 and UnitsOnOrder to 11")
        product.UnitPrice = 1.01D
        product.UnitsOnOrder = 11

        Try
            Console.WriteLine("You submit")
            Console.WriteLine()
            db.SubmitChanges()
        Catch c As ChangeConflictException

            WriteConflictDetails(db.ChangeConflicts)  ' write changed objects / members to console
            Console.WriteLine()
            Console.WriteLine("Resolve by overwriting current values")

            db.ChangeConflicts.ResolveAll(RefreshMode.OverwriteCurrentValues)
            db.SubmitChanges()
        End Try

        Console.WriteLine()
        Dim dbResult = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        Dim result = dbResult.Products.First(Function(prod) prod.ProductID = 1)
        Console.WriteLine("Now product 1 has price={0}, UnitsInStock={1}, UnitsOnOrder={2}", _
                          result.UnitPrice, result.UnitsInStock, result.UnitsOnOrder)
        Console.WriteLine()
        ResetProducts() ' clean up
    End Sub

    <Category("Optimistic Concurrency")> _
    <Title("Resolve conflicts: Keep current values")> _
    <Description("This sample demonstrates how to automatically resolve concurrency conflicts. " & vbNewLine & _
                 "The 'keep current values' option changes everything to the values of this client.")> _
    Public Sub LinqToSqlOptimistic03()
        Dim otherUser_db = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        db.Log = Nothing

        Dim prod = db.Products.First(Function(p) p.ProductID = 1)
        Console.WriteLine("You retrieve the product 1, it costs " & prod.UnitPrice)
        Console.WriteLine("There are {0} units in stock, {1} units on order", prod.UnitsInStock, prod.UnitsOnOrder)
        Console.WriteLine()

        Console.WriteLine("Another user changes the price to 22.22 and UnitsInStock to 22")
        Dim otherUser_product = otherUser_db.Products.First(Function(p) p.ProductID = 1)
        otherUser_product.UnitPrice = 22.22D
        otherUser_product.UnitsInStock = 22
        otherUser_db.SubmitChanges()

        Console.WriteLine("You set the price of product 1 to 1.01 and UnitsOnOrder to 11")
        prod.UnitPrice = 1.01D
        prod.UnitsOnOrder = 11

        Try
            Console.WriteLine("You submit")
            Console.WriteLine()
            db.SubmitChanges()
        Catch c As ChangeConflictException
            WriteConflictDetails(db.ChangeConflicts) ' write changed objects / members to console

            Console.WriteLine()
            Console.WriteLine("Resolve by keeping current values")

            db.ChangeConflicts.ResolveAll(RefreshMode.KeepCurrentValues)
            db.SubmitChanges()
        End Try

        Console.WriteLine()
        Dim dbResult = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        Dim result = dbResult.Products.First(Function(p) p.ProductID = 1)
        Console.WriteLine("Now product 1 has price={0}, UnitsInStock={1}, UnitsOnOrder={2}", _
                          result.UnitPrice, result.UnitsInStock, result.UnitsOnOrder)
        Console.WriteLine()
        ResetProducts() ' clean up
    End Sub

    <Category("Optimistic Concurrency")> _
    <Title("Resolve conflicts: Keep changes")> _
    <Description("This sample demonstrates how to automatically resolve concurrency conflicts. " & vbNewLine & _
                 "The 'keep changes' option keeps all changes from the current user " & _
                 "and merges changes from other users if the corresponding field was not changed by the current user.")> _
    Public Sub LinqToSqlOptimistic04()

        Dim otherUser_db = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        db.Log = Nothing

        Dim prod = db.Products.First(Function(p) p.ProductID = 1)
        Console.WriteLine("You retrieve the product 1, it costs " & prod.UnitPrice)
        Console.WriteLine("There are {0} units in stock, {1} units on order", prod.UnitsInStock, prod.UnitsOnOrder)
        Console.WriteLine()

        Console.WriteLine("Another user changes the price to 22.22 and UnitsInStock to 22")
        Dim otherUser_product = otherUser_db.Products.First(Function(p) p.ProductID = 1)
        otherUser_product.UnitPrice = 22.22D
        otherUser_product.UnitsInStock = 22
        otherUser_db.SubmitChanges()

        Console.WriteLine("You set the price of product 1 to 1.01 and UnitsOnOrder to 11")
        prod.UnitPrice = 1.01D
        prod.UnitsOnOrder = 11S

        Try
            Console.WriteLine("You submit")
            Console.WriteLine()
            db.SubmitChanges()
        Catch c As ChangeConflictException
            WriteConflictDetails(db.ChangeConflicts) 'write changed objects / members to console

            Console.WriteLine()
            Console.WriteLine("Resolve by keeping changes")

            db.ChangeConflicts.ResolveAll(RefreshMode.KeepChanges)
            db.SubmitChanges()
        End Try

        Console.WriteLine()

        Dim dbResult = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        Dim result = dbResult.Products.First(Function(p) p.ProductID = 1)

        Console.WriteLine("Now product 1 has price={0}, UnitsInStock={1}, UnitsOnOrder={2}", _
            result.UnitPrice, result.UnitsInStock, result.UnitsOnOrder)
        Console.WriteLine()
        ResetProducts() ' clean up
    End Sub

    <Category("Optimistic Concurrency")> _
    <Title("Custom resolve rule")> _
    <Description("Demonstrates using MemberConflict.Resolve to write a custom resolve rule.")> _
    Public Sub LinqToSqlOptimistic05()

        Dim otherUser_db = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        db.Log = Nothing

        Dim prod = db.Products.First(Function(p) p.ProductID = 1)
        Console.WriteLine("You retrieve the product 1, it costs " & prod.UnitPrice)
        Console.WriteLine("There are {0} units in stock, {1} units on order", prod.UnitsInStock, prod.UnitsOnOrder)
        Console.WriteLine()

        Console.WriteLine("Another user changes the price to 22.22 and UnitsOnOrder to 2")
        Dim otherUser_product = otherUser_db.Products.First(Function(p) p.ProductID = 1)
        otherUser_product.UnitPrice = 22.22D
        otherUser_product.UnitsOnOrder = 2
        otherUser_db.SubmitChanges()

        Console.WriteLine("You set the price of product 1 to 1.01 and UnitsOnOrder to 11")
        prod.UnitPrice = 1.01D
        prod.UnitsOnOrder = 11
        Dim needsSubmit = True
        While needsSubmit
            Try
                Console.WriteLine("You submit")
                Console.WriteLine()
                needsSubmit = False
                db.SubmitChanges()
            Catch c As ChangeConflictException
                needsSubmit = True
                WriteConflictDetails(db.ChangeConflicts) ' write changed objects / members to console
                Console.WriteLine()
                Console.WriteLine("Resolve by higher price / order")
                For Each conflict In db.ChangeConflicts
                    conflict.Resolve(RefreshMode.KeepChanges)
                    For Each memConflict In conflict.MemberConflicts
                        If (memConflict.Member.Name = "UnitPrice") Then
                            'always use the highest price
                            Dim theirPrice = CDec(memConflict.DatabaseValue)
                            Dim yourPrice = CDec(memConflict.CurrentValue)
                            memConflict.Resolve(Math.Max(theirPrice, yourPrice))
                        ElseIf (memConflict.Member.Name = "UnitsOnOrder") Then
                            'always use higher order
                            Dim theirOrder = CShort(memConflict.DatabaseValue)
                            Dim yourOrder = CShort(memConflict.CurrentValue)
                            memConflict.Resolve(Math.Max(theirOrder, yourOrder))
                        End If
                    Next
                Next
            End Try
        End While
        Dim dbResult = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        Dim result = dbResult.Products.First(Function(p) p.ProductID = 1)
        Console.WriteLine("Now product 1 has price={0}, UnitsOnOrder={1}", _
                          result.UnitPrice, result.UnitsOnOrder)
        Console.WriteLine()
        ResetProducts() 'clean up
    End Sub

    <Category("Optimistic Concurrency")> _
    <Title("Submit with FailOnFirstConflict")> _
    <Description("Submit(FailOnFirstConflict) throws an Optimistic Concurrency Exception when the first conflict is detected." & vbNewLine & _
                 "Only one exception is handled at a time, you have to submit for each conflict.")> _
    Public Sub LinqToSqlOptimistic06()

        db.Log = Nothing
        Dim otherUser_db = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

        'you load 3 products
        Dim prod() = db.Products.OrderBy(Function(p) p.ProductID).Take(3).ToArray()
        For i = 0 To 2
            Console.WriteLine("You retrieve the product {0}, it costs {1}", i + 1, prod(i).UnitPrice)
        Next
        'other user changes these products
        Dim otherUserProd() = otherUser_db.Products.OrderBy(Function(p) p.ProductID).Take(3).ToArray()
        For i = 0 To 2
            Dim otherPrice = (i + 1) * 111.11D
            Console.WriteLine("Other user changes the price of product {0} to {1}", i + 1, otherPrice)
            otherUserProd(i).UnitPrice = otherPrice
        Next
        otherUser_db.SubmitChanges()
        Console.WriteLine("Other user submitted changes")

        'you change your loaded products
        For i = 0 To 2
            Dim yourPrice = (i + 1) * 1.01D
            Console.WriteLine("You set the price of product {0} to {1}", i + 1, yourPrice)
            prod(i).UnitPrice = yourPrice
        Next

        ' submit
        Dim needsSubmit = True
        While needsSubmit
            Try
                Console.WriteLine("======= You submit with FailOnFirstConflict =======")
                needsSubmit = False
                db.SubmitChanges(ConflictMode.FailOnFirstConflict)
            Catch c As ChangeConflictException
                For Each conflict In db.ChangeConflicts

                    DescribeConflict(conflict) 'write changes to console
                    Console.WriteLine("Resolve conflict with KeepCurrentValues")
                    conflict.Resolve(RefreshMode.KeepCurrentValues)
                Next
                needsSubmit = True
            End Try
        End While

        Dim dbResult = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

        For i = 0 To 2
            'Creating a temporary since this will be used in a lambda
            Dim tmp = i
            Dim result = dbResult.Products.First(Function(p) p.ProductID = tmp + 1)
            Console.WriteLine("Now the product {0} has price {1}", i + 1, result.UnitPrice)
        Next
        ResetProducts() 'clean up
    End Sub

    <Category("Optimistic Concurrency")> _
    <Title("Submit with ContinueOnConflict")> _
    <Description("Submit(ContinueOnConflict) collects all concurrency conflicts and throws an exception when the last conflict is detected.\r\n" & _
                 "All conflicts are handled in one catch statement." & vbNewLine & _
                 "It is still possible that another user updated the same objects before this update, so it is possible that another Optimistic Concurrency Exception is thrown which would need to be handled again.")> _
    Public Sub LinqToSqlOptimistic07()
        db.Log = Nothing
        Dim otherUser_db = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)

        ' you load 3 products
        Dim prod() = db.Products.OrderBy(Function(p) p.ProductID).Take(3).ToArray()
        For i = 0 To 2
            Console.WriteLine("You retrieve the product {0}, it costs {1}", i + 1, prod(i).UnitPrice)
        Next

        ' other user changes these products
        Dim otherUserProd() = otherUser_db.Products.OrderBy(Function(p) p.ProductID).Take(3).ToArray()
        For i = 0 To 2
            Dim otherPrice = (i + 1) * 111.11D
            Console.WriteLine("Other user changes the price of product {0} to {1}", i + 1, otherPrice)
            otherUserProd(i).UnitPrice = otherPrice
        Next
        otherUser_db.SubmitChanges()
        Console.WriteLine("Other user submitted changes")

        ' you change your loaded products
        For i = 0 To 2
            Dim yourPrice = (i + 1) * 1.01D
            Console.WriteLine("You set the price of product {0} to {1}", i + 1, yourPrice)
            prod(i).UnitPrice = yourPrice
        Next

        ' submit
        Dim needsSubmit = True
        While needsSubmit

            Try
                Console.WriteLine("======= You submit with ContinueOnConflict =======")
                needsSubmit = False
                db.SubmitChanges(ConflictMode.ContinueOnConflict)

            Catch c As ChangeConflictException

                For Each conflict In db.ChangeConflicts
                    DescribeConflict(conflict) ' write changes to console
                    Console.WriteLine("Resolve conflict with KeepCurrentValues")
                    conflict.Resolve(RefreshMode.KeepCurrentValues)
                Next

                needsSubmit = True
            End Try
        End While

        Dim dbResult = New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        For i = 0 To 2
            Dim tmp = i
            Dim result = dbResult.Products.First(Function(p) p.ProductID = tmp + 1)
            Console.WriteLine("Now the product {0} has price {1}", i + 1, result.UnitPrice)
        Next

        ResetProducts() 'clean up
    End Sub

    <Category("Extensibility Partial Methods")> _
    <Title("Update with OnValidate")> _
    <Description("This sample overrides the OnValidate partial method for Order Class. When an Order is being updated, it validates " & _
                 "ShipVia cannot be greater than 100 else exception throws and no update is sent to database.")> _
    Public Sub LinqToSqlExtensibility01()

        Dim order = (From o In db.Orders _
                     Where o.OrderID = 10355).First()

        ObjectDumper.Write(order)
        Console.WriteLine()

        Console.WriteLine("***** Update Order to set ShipVia to 120 and submit changes ******")
        Console.WriteLine()

        order.ShipVia = 120
        Try
            db.SubmitChanges()
        Catch e As Exception
            Console.WriteLine("****** Catch exception throw by OnValidate() ******")
            Console.WriteLine(e.Message)
        End Try

        Console.WriteLine()
        Console.WriteLine("****** verify that order's ShipVia didn't get changed in db. ******")
        Dim db2 As New NorthwindDataContext(My.Settings.NORTHWINDConnectionString1)
        Dim order2 = (From o In db2.Orders _
                      Where o.OrderID = 10355).First()

        ObjectDumper.Write(order2)
    End Sub


    Public Overrides Sub HandleException(ByVal e As Exception)
        Console.WriteLine("Unable to connect to Northwind database: " & My.Settings.NORTHWINDConnectionString1)
        Console.WriteLine("Check the connection string in the Application Settings or try restarting SQL Server.")
        Console.WriteLine()
        Console.WriteLine("If the problem persists, see the Troubleshooting section of the Readme for tips.")
        Console.WriteLine()
        Console.WriteLine()
        Console.WriteLine("Exception:")
        MyBase.HandleException(e)
    End Sub

    Public Overrides Sub InitSample()
        clearDBCache()
        setLogging(True)
    End Sub

    Public Sub clearDBCache()
        ' Creates a new Northwind object to start fresh with an empty object cache
        ' Active ADO.NET connection will be reused by new Northwind object

        Dim oldLog As TextWriter

        If (db Is Nothing) Then
            oldLog = Nothing
        Else
            oldLog = db.Log
        End If

        db = New NorthwindDataContext() With {.Log = oldLog}
        newDB = New NorthwindInheritance(My.Settings.NORTHWINDConnectionString1) With {.Log = oldLog}
    End Sub

    Public Sub setLogging(ByVal dologging As Boolean)
        If dologging Then
            db.Log = Me.OutputStreamWriter
            'newDB.Log = Me.OutputStreamWriter
        Else
            db.Log = Nothing
            'newDB.Log = Nothing
        End If
    End Sub

    Public Class Name
        Public FirstName As String
        Public LastName As String
    End Class

End Class


Partial Public Class NewCreateDB : Inherits DataContext

    Public Persons As Table(Of Person)

    Public Sub New(ByVal connection As String)
        MyBase.New(connection)
    End Sub

    Public Sub New(ByVal connection As System.Data.IDbConnection)
        MyBase.New(connection)
    End Sub

End Class

<Table(Name:="_Person")> _
Partial Public Class Person : Implements System.ComponentModel.INotifyPropertyChanged

    Private _PersonID As Integer

    Private _PersonName As String

    Private _Age As Nullable(Of Integer)

    Public Sub New()

    End Sub

    <Column(Storage:="_PersonID", DbType:="INT", IsPrimaryKey:=True)> _
    Public Property PersonID() As Integer
        Get
            Return Me._PersonID
        End Get
        Set(ByVal value As Integer)
            If (Me._PersonID <> value) Then
                Me.OnPropertyChanged("PersonID")
                Me._PersonID = value
                Me.OnPropertyChanged("PersonID")
            End If
        End Set
    End Property

    <Column(Storage:="_PersonName", DbType:="NVarChar(30)")> _
    Public Property PersonName() As String
        Get
            Return Me._PersonName
        End Get
        Set(ByVal value As String)
            If (Me._PersonName <> value) Then
                Me.OnPropertyChanged("PersonName")
                Me._PersonName = value
                Me.OnPropertyChanged("PersonName")
            End If
        End Set
    End Property

    <Column(Storage:="_Age", DbType:="INT")> _
    Public Property Age() As Nullable(Of Integer)
        Get
            Return Me._Age
        End Get
        Set(ByVal value As Nullable(Of Integer))
            If (Me._Age <> value) Then
                Me.OnPropertyChanged("Age")
                Me._Age = value
                Me.OnPropertyChanged("Age")
            End If
        End Set
    End Property

    Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

    Protected Overridable Sub OnPropertyChanged(ByVal PropertyName As String)
        RaiseEvent PropertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(PropertyName))
    End Sub
End Class

Partial Public Class NorthwindInheritance : Inherits DataContext
    Public Contacts As Table(Of BaseContact)

    Public Sub New(ByVal connection As String)
        MyBase.New(connection)
    End Sub

    Public Sub New(ByVal connection As System.Data.IDbConnection)
        MyBase.New(connection)
    End Sub

    '
    '                  Contact
    '                 /      \
    '       ShipperContact   FullContact (abstract)
    '                       /     |     \
    '               Customer   Employee  Supplier
    '               Contact    Contact   Contact
    '
    <Table(Name:="Contacts")> _
    <InheritanceMapping(Code:="Unknown", Type:=GetType(BaseContact), IsDefault:=True)> _
    <InheritanceMapping(Code:="Customer", Type:=GetType(CustomerContact))> _
    <InheritanceMapping(Code:="Supplier", Type:=GetType(SupplierContact))> _
    <InheritanceMapping(Code:="Employee", Type:=GetType(EmployeeContact))> _
    <InheritanceMapping(Code:="Shipper", Type:=GetType(ShipperContact))> _
    Public Class BaseContact
        <Column(DbType:="INT NOT NULL", IsPrimaryKey:=True, IsDbGenerated:=True)> _
        Public ContactID As Integer

        <Column(DbType:="NVARCHAR(50)", IsDiscriminator:=True)> _
        Public ContactType As String

        <Column(DbType:="NVARCHAR(40)")> _
        Public CompanyName As String

        <Column(DbType:="NVARCHAR(24)")> _
        Public Phone As String
    End Class

    Public Class ShipperContact : Inherits BaseContact
    End Class

    Public MustInherit Class FullContact : Inherits BaseContact
        <Column(DbType:="NVARCHAR(40)")> _
        Public ContactName As String

        <Column(DbType:="NVARCHAR(30)")> _
        Public ContactTitle As String

        <Column(DbType:="NVARCHAR(60)")> _
        Public Address As String

        <Column(DbType:="NVARCHAR(15)")> _
        Public City As String

        <Column(DbType:="NVARCHAR(15)")> _
        Public Region As String

        <Column(DbType:="NVARCHAR(10)")> _
        Public PostalCode As String

        <Column(DbType:="NVARCHAR(15)")> _
        Public Country As String

        <Column(DbType:="NVARCHAR(24)")> _
        Public Fax As String
    End Class

    Public Class SupplierContact : Inherits FullContact
        <Column(DbType:="NTEXT")> _
        Public HomePage As String
    End Class

    Public Class CustomerContact : Inherits FullContact
    End Class


    Public Class EmployeeContact : Inherits FullContact
        <Column(DbType:="NVARCHAR(4)")> _
        Public Extension As String

        <Column(DbType:="NVARCHAR(255)")> _
        Public PhotoPath As String

        <Column(DbType:="IMAGE")> _
        Public Photo As Byte()
    End Class
End Class