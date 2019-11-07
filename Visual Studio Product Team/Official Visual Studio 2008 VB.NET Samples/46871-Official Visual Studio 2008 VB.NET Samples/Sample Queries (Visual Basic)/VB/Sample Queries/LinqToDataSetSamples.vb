' Copyright (c) Microsoft Corporation.  All rights reserved.
Option Infer On

Imports System.IO
Imports System.Data.Common
Imports System.Data.DataTableExtensions
Imports System.Text
Imports System.Linq
Imports System.Xml.Linq
Imports SampleQueries.SampleSupport
Imports SampleQueries.CustomSequenceOperators

<SampleSupport.Title("LINQ To DataSet Samples"), _
Prefix("DataSetLinq")> _
Public Class LinqToDataSetSamples
    Inherits SampleHarness

    Private dataPath As String = System.Windows.Forms.Application.StartupPath & "\..\..\SampleData\"

#Region "Restriction Operators"

    <Category("Filtering (Where)"), _
     Title("Where - Simple 1"), _
     Description("This sample uses a Where clause to find all DataRows of an DataTable less than 5.")> _
    Public Sub DataSetLinq1()
        Dim numbers = TestDS.Tables("Numbers")

        Dim lowNums = From row In numbers _
                      Where row!number < 5 _
                      Select row

        'If coding with Option Strict On, use the following:
        'Dim lowNums = From row In numbers _
        '              Where row.Field(Of Integer)("number") < 5 _
        '              Select row

        Console.WriteLine("Numbers < 5:")

        For Each x In lowNums
            Console.WriteLine(x!number)
        Next
    End Sub

    <Category("Filtering (Where)")> _
    <Title("Where - Simple 2")> _
    <Description("This sample uses a Where clause to find all DataRows that are out of stock.")> _
    Public Sub DataSetLinq2()
        Dim products = TestDS.Tables("Products")


        Dim soldOutProducts = From prod In products _
                              Where prod!UnitsInStock = 0 _
                              Select prod

        Console.WriteLine("Sold out products:")
        For Each p In soldOutProducts
            Console.WriteLine(p!ProductName & " is sold out!")
        Next
    End Sub

    <Category("Filtering (Where)")> _
    <Title("Where - Simple 3")> _
    <Description("This sample uses a Where clause to find all DataRows that are in stock and " & _
                 "cost more than 3.00 per unit")> _
    Public Sub DataSetLinq3()
        Dim products = TestDS.Tables("Products")

        Dim expensiveInStockProducts = From prod In products _
                                       Where prod!UnitsInStock > 0 AndAlso _
                                             prod!UnitPrice > 3.0# _
                                       Select prod

        Console.WriteLine("In-stock products that cost more than 3.00:")
        For Each product In expensiveInStockProducts
            Console.WriteLine(product!ProductName & "is in stock and costs more than 3.00.")
        Next
    End Sub

    <Category("Filtering (Where)")> _
    <Title("Where - Drilldown")> _
    <Description("This sample uses a Where clause to find all customers in France " & _
                 "and then uses the resulting sequence to drill down into their " & _
                 "orders.")> _
    Public Sub DataSetLinq4()
        Dim customers = TestDS.Tables("Customers")

        Dim franceCustomers = From cust In customers _
                              Where cust!Country = "France" _
                              Select cust

        Console.WriteLine("Customers from France and their orders:")
        For Each customer In franceCustomers

            Console.WriteLine("Customer {0}: {1}", customer!CustomerID, customer!CompanyName)

            For Each order In customer.GetChildRows("CustomersOrders")
                Console.WriteLine("  Order {0}: {1}", order!OrderID, order!OrderDate)
            Next
        Next
    End Sub

    <Category("Filtering (Where)")> _
    <Title("Where - Indexed")> _
    <Description("This sample demonstrates an indexed Where clause that returns digits whose name is " & _
                 "shorter than their value.")> _
    Public Sub DataSetLinq5()

        Dim digits = TestDS.Tables("Digits").AsEnumerable()

        Dim shortDigits = digits.Where(Function(digit, index) digit.Field(Of String)(0).Length < index)

        Console.WriteLine("Short digits:")
        For Each d In shortDigits
            Console.WriteLine("The word " & d!digit & " is shorter than its value.")
        Next
    End Sub

    <Category("Filtering (Where)")> _
    <Title("Single - Simple")> _
    <Description("Turns a seqeunce into a single result")> _
    Public Sub DataSetLinq106()

        'create a table with a single row
        Dim singleRowTable = New DataTable("SingleRowTable")
        singleRowTable.Columns.Add("id", GetType(Integer))
        singleRowTable.Rows.Add(New Object() {1})

        Dim singleRow = singleRowTable.AsEnumerable().Single()

        Console.WriteLine(singleRow IsNot Nothing)
    End Sub

    <Category("Filtering (Where)")> _
    <Title("Single - with Predicate")> _
    <Description("Returns a single element based on the specified predicate")> _
    Public Sub DataSetLinq107()

        'create an table with a two rows
        Dim table = New DataTable("MyTable")
        table.Columns.Add("id", GetType(Integer))
        table.Rows.Add(New Object() {1})
        table.Rows.Add(New Object() {2})

        Dim singleRow = table.AsEnumerable().Single(Function(row) row!id = 1)

        Console.WriteLine(singleRow IsNot Nothing)
    End Sub

#End Region

#Region "Projection Operators"

    <Category("Projecting (Select)")> _
    <Title("Select - Simple 1")> _
    <Description("This sample uses Select to produce a sequence of ints one higher than " & _
                 "those in an existing DataTable.")> _
    Public Sub DataSetLinq6()
        Dim numbers = TestDS.Tables("Numbers")

        Dim numsPlusOne = From row In numbers _
                          Select row!number + 1

        Console.WriteLine("Numbers + 1:")
        For Each i In numsPlusOne
            Console.WriteLine(i)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Simple 2")> _
    <Description("This sample uses Select to return a sequence of just the names of a list of products.")> _
    Public Sub DataSetLinq7()
        Dim products = TestDS.Tables("Products")

        Dim productNames = From prod In products _
                           Select prod!ProductName

        Console.WriteLine("Product Names:")
        For Each productName In productNames
            Console.WriteLine(productName)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Transformation")> _
    <Description("This sample uses Select to produce a sequence of strings representing " & _
                 "the text version of a sequence of ints.")> _
    Public Sub DataSetLinq8()
        Dim numbers = TestDS.Tables("Numbers")
        Dim strings As String() = {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}

        Dim textNums = From row In numbers _
                       Select strings(row!number)

        Console.WriteLine("Number strings:")
        For Each s In textNums
            Console.WriteLine(s)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Anonymous Types 1")> _
    <Description("This sample uses Select to produce a sequence of the uppercase " & _
                 "and lowercase versions of each word in the original DataTable.")> _
    Public Sub DataSetLinq9()
        Dim words = TestDS.Tables("Words")

        Dim upperLowerWords = From row In words _
                              Select Upper = CStr(row!word).ToUpper(), _
                                     Lower = CStr(row!word).ToLower()

        For Each ul In upperLowerWords
            Console.WriteLine("Uppercase: " & ul.Upper & ", Lowercase: " & ul.Lower)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Anonymous Types 2")> _
    <Description("This sample uses Select to produce a sequence containing text " & _
                 "representations of digits and whether their length is even or odd.")> _
    Public Sub DataSetLinq10()
        Dim numbers = TestDS.Tables("Numbers")
        Dim stringNames() As String = {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}

        Dim digitOddEvens = From row In numbers _
                            Select Digit = stringNames(row("number")), _
                                   Even = (row!number Mod 2 = 0)

        For Each d In digitOddEvens
            Console.WriteLine("The digit " & d.Digit & " is " & If(d.Even, "even.", "odd."))
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Anonymous Types 3")> _
    <Description("This sample uses Select to produce a sequence containing some column values " & _
                 "from Product DataRows, including UnitPrice which is renamed to Price " & _
                 "in the resulting type.")> _
    Public Sub DataSetLinq11()
        Dim products = TestDS.Tables("Products")

        Dim productInfos = From prod In products _
                           Select prod!ProductName, prod!Category, prod!UnitPrice


        'To get strongly-typed results, you can use the .Field(Of T) extension method to
        'explicitly specify the data types of your columns

        'Dim productInfo2 = From prod In products _
        '                   Select ProductName = prod.Field(Of String)("ProductName"), _
        '                          Category = prod.Field(Of String)("Category"), _
        '                          UnitPrice = prod.Field(Of Decimal)("UnitPrice")

        Console.WriteLine("Product Info:")
        For Each productInfo In productInfos
            Console.WriteLine(productInfo.ProductName & " is in the category " & _
                              productInfo.Category & " and costs " & _
                              productInfo.UnitPrice & " per unit")
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Indexed")> _
    <Description("This sample uses an indexed Select clause to determine if the value of Integers " & _
                 "in an array match their position in the array.")> _
    Public Sub DataSetLinq12()


        Dim numbers = TestDS.Tables("Numbers").AsEnumerable()

        Dim numsInPlace = numbers.Select( _
                            Function(num, index) New With {.Num = num!number, _
                                                           .InPlace = (num!number = index)})

        Console.WriteLine("Number: In-place?")
        For Each n In numsInPlace
            Console.WriteLine(n.Num & ": " & n.InPlace)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Filtered")> _
    <Description("This sample combines Select and Where clauses to make a simple query that returns " & _
                 "the text form of each digit less than 5.")> _
    Public Sub DataSetLinq13()
        Dim numbers = TestDS.Tables("Numbers")
        Dim digits = TestDS.Tables("Digits")

        Dim lowNums = From num In numbers _
                      Where num!number < 5 _
                      Select digits.Rows(num!number)!digit

        Console.WriteLine("Numbers < 5:")
        For Each num In lowNums
            Console.WriteLine(num)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - Compound from 1")> _
    <Description("This sample uses a compound From clause to make a query that returns all pairs " & _
                 "of numbers from both DataTables such that the number from numbersA is less than the number " & _
                 "from numbersB.")> _
    Public Sub DataSetLinq14()
        Dim numbersA = TestDS.Tables("NumbersA")
        Dim numbersB = TestDS.Tables("NumbersB")

        Dim pairs = From numA In numbersA, numB In numbersB _
                    Where numA!number < numB!number _
                    Select numberA = numA!number, numberB = numB!number

        Console.WriteLine("Pairs where a < b:")
        For Each pair In pairs
            Console.WriteLine(pair.numberA & " is less than " & pair.numberB)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - Compound from 2")> _
    <Description("This sample uses a compound From clause to select all orders where the " & _
                 "order total is less than 500.00.")> _
    Public Sub DataSetLinq15()
        Dim customers = TestDS.Tables("Customers")

        'We use the ! syntax to automatically infer the column names for the anonymous type
        Dim orders = From cust In customers, ord In cust.GetChildRows("CustomersOrders") _
                     Where ord!Total < 500.0 _
                     Select cust!CustomerID, ord!OrderID, ord!Total

        For Each x In orders
            Console.WriteLine("CustomerID: {0}, OrderID: {1}, Total: {2}", x.CustomerID, x.OrderID, x.Total)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - Compound from 3")> _
    <Description("This sample uses a compound From clause to select all orders where the " & _
                 "order was made in 1998 or later.")> _
    Public Sub DataSetLinq16()
        Dim customers = TestDS.Tables("Customers")

        Dim orders = From cust In customers, ord In cust.GetChildRows("CustomersOrders") _
                     Where ord!OrderDate >= #1/1/1998# _
                     Select cust!CustomerID, ord!OrderID, ord!OrderDate

        For Each ord In orders
            Console.WriteLine("CustomerID: {0}, OrderID: {1}, OrderDate: {2}", _
                              ord.CustomerID, ord.OrderID, ord.OrderDate)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - from Assignment")> _
    <Description("This sample uses a compound From clause to select all orders where the " & _
                 "order total is greater than 2000.00 and uses ""From assignment"" to avoid " & _
                 "requesting the total twice.")> _
    Public Sub DataSetLinq17()

        Dim customers = TestDS.Tables("Customers")
        Dim orders = TestDS.Tables("Orders")


        Dim myOrders = From cust In customers, ord In orders _
                       Let Total = ord!Total _
                       Let CustomerID = cust!CustomerID _
                       Let ordCustomerID = ord!CustomerID _
                       Where CustomerID = ordCustomerID AndAlso Total >= 2000 _
                       Select CustomerID, OrderID = ord!OrderID, Total

        'To get Strongly-typed results, use the .Field(Of T) extension method:
        'Dim myOrders = From cust In customers, ord In orders _
        '               Let Total = ord.Field(Of Decimal)("Total") _
        '               Let CustomerID = cust.Field(Of String)("CustomerID") _
        '               Let ordCustomerID = ord.Field(Of String)("CustomerID") _
        '               Where CustomerID = ordCustomerID AndAlso Total >= 2000 _
        '               Select CustomerID, OrderID = ord.Field(Of Integer)("OrderID"), Total

        ObjectDumper.Write(myOrders, 1)
    End Sub


    <Category("Projecting (Select)")> _
    <Title("SelectMany - Multiple From")> _
    <Description("This sample uses multiple From clauses so that filtering on customers can " & _
                 "be done before selecting their orders.  This makes the query more efficient by " & _
                 "not selecting and then discarding orders for customers outside of France.")> _
    Public Sub DataSetLinq18()
        Dim customers = TestDS.Tables("Customers")

        Dim cutoffDate = #1/1/1997#

        Dim orders = From cust In customers, ord In cust.GetChildRows("CustomersOrders") _
                     Where cust!Country = "France" And ord!OrderDate >= cutoffDate _
                     Select cust!CustomerID, ord!OrderID

        For Each ord In orders
            Console.WriteLine("CustomerID: " & ord.CustomerID & ", OrderID: " & ord.OrderID)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - Indexed")> _
    <Description("This sample uses an indexed SelectMany clause to select all orders, " & _
                 "while referring to customers by the order in which they are returned " & _
                 "from the query.")> _
    Public Sub DataSetLinq19()

        Dim customers = TestDS.Tables("Customers").AsEnumerable()
        Dim orders = TestDS.Tables("Orders").AsEnumerable()

        Dim customerOrders = customers.SelectMany(Function(cust, cIndex) _
                                From ord In orders _
                                Where cust!CustomerID = ord!CustomerID _
                                Select CustomerIndex = cIndex + 1, OrderID = ord!OrderID)

        For Each c In customerOrders
            Console.WriteLine("Customer Index: " & c.CustomerIndex & _
                              " has an order with OrderID " & c.OrderID)
        Next
    End Sub

#End Region

#Region "Partitioning Operators"

    <Category("Partitioning (Skip/Take)")> _
    <Title("Take - Simple")> _
    <Description("This sample uses Take to get only the first 3 elements of " & _
                 "the source DataTable.")> _
    Public Sub DataSetLinq20()
        Dim numbers = TestDS.Tables("Numbers")

        Dim first3Numbers = From num In numbers Take 3

        Console.WriteLine("First 3 numbers:")
        For Each n In first3Numbers
            Console.WriteLine(n!number)
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Take - Nested")> _
    <Description("This sample uses Take to get the first 3 orders from customers " & _
                 "in Finland.")> _
    Public Sub DataSetLinq21()
        Dim customers = TestDS.Tables("Customers")

        Dim first3FinlandOrders = From cust In customers, ord In cust.GetChildRows("CustomersOrders") _
                                  Where cust!Country = "Finland" _
                                  Select cust!CustomerID, ord!OrderID, ord!OrderDate _
                                  Take 3

        Console.WriteLine("First 3 orders in Finland:")
        For Each ord In first3FinlandOrders
            Console.WriteLine("CustomerID: {0}, OrderID: {1}, OrderDate: {2}", _
                              ord.CustomerID, ord.OrderID, ord.OrderDate)
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Skip - Simple")> _
    <Description("This sample uses Skip to get all but the first 4 rows of " & _
                 "the source DataTable.")> _
    Public Sub DataSetLinq22()
        Dim numbers = TestDS.Tables("Numbers")

        Dim allButFirst4Numbers = From num In numbers Skip 4

        Console.WriteLine("All but first 4 numbers:")
        For Each n In allButFirst4Numbers
            Console.WriteLine(n("number"))
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Skip - Nested")> _
    <Description("This sample uses Take to get all but the first 2 orders from customers " & _
                 "in the USA.")> _
    Public Sub DataSetLinq23()
        Dim customers = TestDS.Tables("Customers")

        Dim usaOrders = From cust In customers, ord In cust.GetChildRows("CustomersOrders") _
                        Where cust!Country = "USA" _
                        Select cust!CustomerID, ord!OrderID, ord!OrderDate

        Dim allButFirst2Orders = usaOrders.Skip(2)

        Console.WriteLine("All but first 2 orders in USA:")
        For Each ord In allButFirst2Orders
            Console.WriteLine("CustomerID: {0}, OrderID: {1}, OrderDate: {2}", _
                              ord.CustomerID, ord.OrderID, ord.OrderDate)
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("TakeWhile - Simple")> _
    <Description("This sample uses Take While to return rows starting from the " & _
                 "beginning of the DataTable until a number is hit that is not less than 6.")> _
    Public Sub DataSetLinq24()
        Dim numbers = TestDS.Tables("Numbers")

        Dim firstNumbersLessThan6 = From n In numbers _
                                    Take While n!number < 6

        Console.WriteLine("First numbers less than 6:")
        For Each num In firstNumbersLessThan6
            Console.WriteLine(num!number)
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("TakeWhile - Indexed")> _
    <Description("This sample uses TakeWhile to return elements starting from the " & _
                "beginning of the array until a number is hit that is less than its position " & _
                "in the array.")> _
    Public Sub DataSetLinq25()

        Dim numbers = TestDS.Tables("Numbers").AsEnumerable()

        Dim firstSmallNumbers = numbers.TakeWhile(Function(n, index) n!number >= index)

        Console.WriteLine("First numbers not less than their position:")
        For Each num In firstSmallNumbers
            Console.WriteLine(num!number)
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("SkipWhile - Simple")> _
    <Description("This sample uses Skip While to get the elements of the array " & _
                "starting from the first element divisible by 3.")> _
    Public Sub DataSetLinq26()

        Dim numbers = TestDS.Tables("Numbers")

        Dim allButFirst3Numbers = From num In numbers _
                                  Skip While num!number Mod 3 <> 0

        Console.WriteLine("All elements starting from first element divisible by 3:")
        For Each num In allButFirst3Numbers
            Console.WriteLine(num("number"))
        Next
    End Sub


    <Category("Partitioning (Skip/Take)")> _
    <Title("SkipWhile - Indexed")> _
    <Description("This sample uses SkipWhile to get the elements of the array " & _
                "starting from the first element less than its position.")> _
    Public Sub DataSetLinq27()

        Dim numbers = TestDS.Tables("Numbers").AsEnumerable()

        Dim laterNumbers = numbers.SkipWhile(Function(n, index) n!number >= index)

        Console.WriteLine("All elements starting from first element less than its position:")
        For Each num In laterNumbers
            Console.WriteLine(num!number)
        Next
    End Sub

#End Region

#Region "Ordering Operators"

    <Category("Ordering (Order By)")> _
    <Title("OrderBy - Simple 1")> _
    <Description("This sample uses Order By to sort a DataTable of words alphabetically.")> _
    Public Sub DataSetLinq28()
        Dim words = TestDS.Tables("Words2")

        Dim sortedWords = From row In words _
                          Order By row!word

        Console.WriteLine("The sorted list of words:")
        For Each w In sortedWords
            Console.WriteLine(w!word)
        Next
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("OrderBy - Simple 2")> _
    <Description("This sample uses Order By to sort a DataTable of words by length.")> _
    Public Sub DataSetLinq29()
        Dim words = TestDS.Tables("Words2")

        Dim sortedWords = From row In words _
                          Order By row.Field(Of String)("word").Length

        Console.WriteLine("The sorted list of words (by length):")
        For Each word In sortedWords
            Console.WriteLine(word!word)
        Next

    End Sub

    <Category("Ordering (Order By)")> _
    <Title("OrderBy - Simple 3")> _
    <Description("This sample uses Order By to sort a DataTable of products by name.")> _
    Public Sub DataSetLinq30()
        Dim products = TestDS.Tables("Products")

        Dim sortedProducts = From prod In products _
                             Order By prod!ProductName

        For Each r In sortedProducts
            Console.WriteLine("Product ID: " & r!ProductId & " ProductName: " & r!ProductName)
        Next
    End Sub

    Private Class CaseInsensitiveComparer : Implements IComparer(Of String)

        Public Function Compare(ByVal x As String, ByVal y As String) As Integer Implements IComparer(Of String).Compare
            Return String.Compare(x, y, True)
        End Function

    End Class


    <Category("Ordering (Order By)")> _
    <Title("OrderBy - Comparer")> _
    <Description("This sample uses an Order By clause with a custom comparer to " & _
                 "do a case-insensitive sort of the words in an array.")> _
    <LinkedClass("CaseInsensitiveComparer")> _
    Public Sub DataSetLinq31()

        Dim words3 = TestDS.Tables("Words3").AsEnumerable()

        Dim sortedWords = words3.OrderBy(Function(row) row("word"), _
                                         New CaseInsensitiveComparer())

        For Each row In sortedWords
            Console.WriteLine(row("word"))
        Next
    End Sub


    <Category("Ordering (Order By)")> _
    <Title("OrderByDescending - Simple 1")> _
    <Description("This sample uses Order By and Descending to sort a DataTable of " & _
                 "doubles from highest to lowest.")> _
    Public Sub DataSetLinq32()
        Dim doubles = TestDS.Tables("Doubles")

        Dim sortedDoubles = From dbl In doubles _
                            Order By dbl!double Descending

        Console.WriteLine("The doubles from highest to lowest:")
        For Each dbl In sortedDoubles
            Console.WriteLine(dbl("double"))
        Next
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("OrderByDescending - Simple 2")> _
    <Description("This sample uses Order By to sort a DataTable of products by units in stock " & _
                 "from highest to lowest.")> _
    Public Sub DataSetLinq33()
        Dim products = TestDS.Tables("Products")

        Dim sortedProducts = From prod In products _
                             Order By prod!UnitsInStock Descending

        For Each row In sortedProducts
            Console.WriteLine("Product ID: " & row!ProductID & " UnitsInStock: " & row!UnitsInStock)
        Next
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("OrderByDescending - Comparer")> _
    <Description("This sample uses an OrderByDescending clause with a custom comparer to " & _
                 "do a case-insensitive descending sort of the words in an array.")> _
    <LinkedClass("CaseInsensitiveComparer")> _
    Public Sub DataSetLinq34()

        Dim words3 = TestDS.Tables("Words3").AsEnumerable()

        Dim sortedWords = words3.OrderByDescending( _
                                 Function(row) row!word, New CaseInsensitiveComparer())

        For Each dr In sortedWords
            Console.WriteLine(dr("word"))
        Next
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("ThenBy - Simple")> _
    <Description("This sample uses a compound Order By clause to sort a DataTable of digits, " & _
                 "first by length of their name, and then alphabetically by the name itself.")> _
    Public Sub DataSetLinq35()
        Dim digits = TestDS.Tables("Digits")

        Dim sortedDigits = From row In digits _
                           Select Digit = row!digit, _
                                  DigLength = CStr(row!digit).Length _
                           Order By DigLength, Digit

        Console.WriteLine("Sorted digits:")
        For Each d In sortedDigits
            Console.WriteLine(d.Digit)
        Next
    End Sub


    <Category("Ordering (Order By)")> _
    <Title("ThenBy - Comparer")> _
    <Description("This sample uses an OrderBy and a ThenBy clause with a custom comparer to " & _
                 "sort first by word length and then by a case-insensitive sort of the words in an array.")> _
    <LinkedClass("CaseInsensitiveComparer")> _
    Public Sub DataSetLinq36()

        Dim words3 = TestDS.Tables("Words3").AsEnumerable()

        Dim sortedWords = words3.OrderBy( _
                              Function(row) row.Field(Of String)("word").Length).ThenBy( _
                              Function(row) row.Field(Of String)("word"), _
                                         New CaseInsensitiveComparer())

        For Each row In sortedWords
            Console.WriteLine(row("word"))
        Next
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("ThenByDescending - Simple")> _
    <Description("This sample uses a compound Order By to sort a DataTable of products, " & _
                 "first by category, and then by unit price, from highest to lowest.")> _
    Public Sub DataSetLinq37()
        Dim products = TestDS.Tables("Products")

        Dim sortedProducts = From prod In products _
                             Order By prod!Category, prod!UnitPrice Descending

        For Each row In sortedProducts
            Console.WriteLine("Product ID: " & row!ProductID & _
                              " Category: " & row!Category & _
                              " UnitsInStock: " & row!UnitsInStock)
        Next
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("ThenByDescending - Comparer")> _
    <Description("This sample uses an OrderBy and a ThenBy clause with a custom comparer to " & _
                 "sort first by word length and then by a case-insensitive descending sort " & _
                 "of the words in an array.")> _
    <LinkedClass("CaseInsensitiveComparer")> _
    Public Sub DataSetLinq38()

        Dim words3 = TestDS.Tables("Words3").AsEnumerable()

        'The call to .Length is latebound, but can be made early-bound using the 
        '.Field(Of T) extension method
        Dim sortedWords = words3.OrderBy(Function(row) row!word.Length).ThenByDescending( _
                                         Function(row) row!word, New CaseInsensitiveComparer())

        For Each row In sortedWords
            Console.WriteLine(row("word"))
        Next
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("Reverse")> _
    <Description("This sample uses Reverse to create a list of all digits in the DataTable whose " & _
                 "second letter is 'i' that is reversed from the order in the original array.")> _
    Public Sub DataSetLinq39()
        Dim digits = TestDS.Tables("Digits")

        Dim IDigits = From row In digits _
                      Where row("digit")(1) = "i"

        Dim ReverseIDigits = IDigits.Reverse()

        Console.WriteLine("A backwards list of the digits with a second character of 'i':")
        For Each row In ReverseIDigits
            Console.WriteLine(row!digit)
        Next
    End Sub

    <Category("Grouping (Group By)")> _
    <Title("GroupBy - Simple 1")> _
    <Description("This sample uses Group By to partition a list of numbers by " & _
                 "their remainder when divided by 5.")> _
    Public Sub DataSetLinq40()

        Dim numbers = TestDS.Tables("Numbers")
        Dim numGroups = From num In numbers _
                        Group num By Remainder = num!number Mod 5 _
                        Into NumberGroup = Group

        For Each group In numGroups
            Console.WriteLine("Numbers with a remainder of " & group.Remainder & " when divided by 5:")
            For Each num In group.NumberGroup
                Console.WriteLine(num!number)
            Next
        Next
    End Sub

    <Category("Grouping (Group By)")> _
    <Title("GroupBy - Simple 2")> _
    <Description("This sample uses Group By to partition a list of words by " & _
                 "their first letter.")> _
    Public Sub DataSetLinq41()

        Dim words4 = TestDS.Tables("Words4")

        Dim wordGroups = From word In words4 _
                         Group word By FirstLetter = word.Field(Of String)("word")(0) _
                         Into Words = Group

        For Each group In wordGroups
            Console.WriteLine("Words that start with the letter '" & group.FirstLetter & "':")
            For Each word In group.Words
                Console.WriteLine(word!word)
            Next
        Next
    End Sub

    <Category("Grouping (Group By)")> _
    <Title("GroupBy - Simple 3")> _
    <Description("This sample uses Group By to partition a list of products by category.")> _
    Public Sub DataSetLinq42()

        Dim products = TestDS.Tables("Products")

        Dim productGroups = From prod In products _
                            Group prod By prod!Category _
                            Into ProductGroup = Group

        For Each group In productGroups
            Console.WriteLine("Category: " & group.Category)

            For Each row In group.ProductGroup
                Console.WriteLine(vbTab & row!ProductName)
            Next
        Next

    End Sub

    <Category("Grouping (Group By)")> _
    <Title("GroupBy - Nested")> _
    <Description("This sample uses Group By to partition a list of each customer's orders, " & _
                 "first by year, and then by month.")> _
    Public Sub DataSetLinq43()

        Dim customers = TestDS.Tables("Customers")

        Dim custOrdersGroups = From cust In customers _
                               Select CompanyName = cust!CompanyName, _
                                      YearGroups = From ord In cust.GetChildRows("CustomersOrders") _
                                                   Group ord By Key = ord.Field(Of DateTime)("OrderDate").Year _
                                                   Into Group _
                                                   Select Year = Key, _
                                                          MonthGroups = From ord In Group _
                                                                        Group ord By MonthKey = ord.Field(Of DateTime)("OrderDate").Month _
                                                                        Into MonthGroup = Group _
                                                                        Select Month = MonthKey, Orders = MonthGroup

        For Each cog In custOrdersGroups
            Console.WriteLine("CompanyName= " & cog.CompanyName)

            For Each yg In cog.YearGroups
                Console.WriteLine(vbTab & " Year= " & yg.Year)

                For Each mg In yg.MonthGroups
                    Console.WriteLine(vbTab & vbTab & " Month= " & mg.Month)

                    For Each order In mg.Orders
                        Console.WriteLine(vbTab & vbTab & vbTab & " OrderID= " & order("OrderID"))
                        Console.WriteLine(vbTab & vbTab & vbTab & " OrderDate= " & order("OrderDate"))
                    Next
                Next
            Next
        Next
    End Sub

    Private Class AnagramEqualityComparer : Implements IEqualityComparer(Of String)

        Public Overloads Function Equals(ByVal x As String, ByVal y As String) As Boolean Implements IEqualityComparer(Of String).Equals
            Return getCanonicalString(x) = getCanonicalString(y)
        End Function


        Public Overloads Function GetHashCode(ByVal obj As String) As Integer Implements IEqualityComparer(Of String).GetHashCode
            Return getCanonicalString(obj).GetHashCode()
        End Function

        Private Function getCanonicalString(ByVal word As String) As String
            Dim wordChars = word.ToCharArray()
            Array.Sort(wordChars)
            Return New String(wordChars)
        End Function

    End Class

    <Category("Grouping (Group By)")> _
    <Title("GroupBy - Comparer")> _
    <Description("This sample uses GroupBy to partition trimmed elements of an array using " & _
                 "a custom comparer that matches words that are anagrams of each other.")> _
    <LinkedClass("AnagramEqualityComparer")> _
    Public Sub DataSetLinq44()

        Dim anagrams = TestDS.Tables("Anagrams").AsEnumerable()

        Dim orderGroups = anagrams.GroupBy(Function(w) w.Field(Of String)("anagram").Trim(), _
                                           New AnagramEqualityComparer())

        For Each group In orderGroups
            Console.WriteLine("Key: " & group.Key)
            For Each word In group
                Console.WriteLine(vbTab & word!anagram)
            Next
        Next

    End Sub

    <Category("Grouping (Group By)")> _
    <Title("GroupBy - Comparer, Mapped")> _
    <Description("This sample uses GroupBy to partition trimmed elements of an array using " & _
                 "a custom comparer that matches words that are anagrams of each other, " & _
                 "and then converts the results to uppercase.")> _
    <LinkedClass("AnagramEqualityComparer")> _
    Public Sub DataSetLinq45()

        Dim anagrams = TestDS.Tables("Anagrams").AsEnumerable()

        Dim orderGroups = anagrams.GroupBy(Function(row) row("anagram").Trim(), _
                                           Function(row) row("anagram").ToUpper(), _
                                           New AnagramEqualityComparer())

        For Each group In orderGroups
            Console.WriteLine("Key: " & group.Key)
            For Each word In group
                Console.WriteLine(vbTab & word)
            Next
        Next

    End Sub

#End Region

#Region "Set Operators"

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("DistinctRows")> _
    <Description("This sample uses Distincts to remove duplicate DataRows in a DataTable of " & _
                 "factors of 300.")> _
    Public Sub DataSetLinq46()
        Dim factorsOf300 = TestDS.Tables("FactorsOf300")

        Dim uniqueFactors = factorsOf300.AsEnumerable().Distinct()

        Console.WriteLine("Prime factors of 300:")
        For Each factor In uniqueFactors
            Console.WriteLine(factor("factor"))
        Next
    End Sub

    <SampleSupport.Category("Set Operators (Distinct/Union...)")> _
    <SampleSupport.Title("Distinct")> _
    <SampleSupport.Description("This sample uses Distinct to find the unique Category names.")> _
    Public Sub DataSetLinq47()
        Dim products = TestDS.Tables("Products")

        Dim categoryNames = From prod In products _
                            Select prod!Category _
                            Distinct

        Console.WriteLine("Category names:")
        For Each name In categoryNames
            Console.WriteLine(name)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("UnionRows")> _
    <Description("This sample uses Union to create one sequence that contains the unique values " & _
                 "from both arrays.")> _
    Public Sub DataSetLinq48()
        Dim numbersA = TestDS.Tables("NumbersA").AsEnumerable()
        Dim numbersB = TestDS.Tables("NumbersB").AsEnumerable()

        Dim uniqueNumbers = numbersA.Union(numbersB)

        Console.WriteLine("Unique numbers from both DataTables:")
        For Each num In uniqueNumbers
            Console.WriteLine(num!number)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Union")> _
    <Description("This sample uses Union to create one sequence that contains the unique first letter " & _
                 "from both product and customer names.")> _
    Public Sub DataSetLinq49()

        Dim products = TestDS.Tables("Products")
        Dim customers = TestDS.Tables("Customers")

        Dim productFirstChars = From prod In products _
                                Select prod.Field(Of String)("ProductName")(0)

        Dim customerFirstChars = From cust In customers _
                                 Select cust.Field(Of String)("CompanyName")(0)

        Dim uniqueFirstChars = productFirstChars.Union(customerFirstChars)

        Console.WriteLine("Unique first letters from Product names and Customer names:")
        For Each ch In uniqueFirstChars
            Console.WriteLine(ch)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("IntersectRows")> _
    <Description("This sample uses Intersect to create one sequence that contains the common values " & _
                "shared by both source DataTables.")> _
    Public Sub DataSetLinq50()
        Dim numbersA = TestDS.Tables("NumbersA").AsEnumerable()
        Dim numbersB = TestDS.Tables("NumbersB").AsEnumerable()

        Dim commonNumbers = numbersA.Intersect(numbersB, DataRowComparer.Default)

        Console.WriteLine("Common numbers shared by both DataTables:")
        For Each num In commonNumbers
            Console.WriteLine(num!number)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Intersect")> _
    <Description("This sample uses Intersect to create one sequence that contains the common first letter " & _
                 "from both product and customer names.")> _
    Public Sub DataSetLinq51()

        Dim products = TestDS.Tables("Products")
        Dim customers = TestDS.Tables("Customers")

        Dim productFirstChars = From prod In products _
                                Select prod.Field(Of String)("ProductName")(0)

        Dim customerFirstChars = From cust In customers _
                                 Select cust.Field(Of String)("CompanyName")(0)

        Dim commonFirstChars = productFirstChars.Intersect(customerFirstChars)

        Console.WriteLine("Common first letters from Product names and Customer names:")
        For Each ch In commonFirstChars
            Console.WriteLine(ch)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("ExceptRows")> _
    <Description("This sample uses Except to create a sequence that contains the values from numbersA" & _
                 "DataTable that are not also in numbersB DataTable.")> _
    Public Sub DataSetLinq52()
        Dim numbersA = TestDS.Tables("NumbersA").AsEnumerable()
        Dim numbersB = TestDS.Tables("NumbersB").AsEnumerable()

        Dim aOnlyNumbers = numbersA.Except(numbersB)

        Console.WriteLine("Numbers in first DataTable but not second DataTable:")
        For Each num In aOnlyNumbers
            Console.WriteLine(num!number)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Except")> _
    <Description("This sample uses Except to create one sequence that contains the first letters " & _
                 "of product names that are not also first letters of customer names.")> _
    Public Sub DataSetLinq53()

        Dim products = TestDS.Tables("Products")
        Dim customers = TestDS.Tables("Customers")

        Dim productFirstChars = From prod In products _
                                Select prod.Field(Of String)("ProductName")(0)

        Dim customerFirstChars = From cust In customers _
                                 Select cust.Field(Of String)("CompanyName")(0)

        Dim productOnlyFirstChars = productFirstChars.Except(customerFirstChars)

        Console.WriteLine("First letters from Product names, but not from Customer names:")
        For Each ch In productOnlyFirstChars
            Console.WriteLine(ch)
        Next
    End Sub

#End Region

#Region "Conversion Operators"

    <Category("Converting (ToArray/ToList...)")> _
    <Title("ToArray")> _
    <Description("This sample uses ToArray to immediately evaluate a sequence into an array.")> _
    Public Sub DataSetLinq54()
        Dim doublesDataTable = TestDS.Tables("Doubles")

        Dim doubles = From dbl In doublesDataTable _
                      Select dbl!double

        Dim doublesArray = doubles.ToArray()

        Console.WriteLine("Every other double from highest to lowest:")
        For d = 0 To doublesArray.Length
            Console.WriteLine(doublesArray(d))
            d += 1
        Next
    End Sub

    <Category("Converting (ToArray/ToList...)")> _
    <Title("ToList")> _
    <Description("This sample uses ToList to immediately evaluate a sequence into a List(Of T).")> _
    Public Sub DataSetLinq55()
        Dim wordsTable = TestDS.Tables("Words")

        Dim words = From word In wordsTable _
                    Select word!word

        Dim wordList = words.ToList()

        Console.WriteLine("The sorted word list:")
        For Each word In wordList
            Console.WriteLine(word)
        Next
    End Sub

    <Category("Converting (ToArray/ToList...)")> _
    <Title("ToDictionary")> _
    <Description("This sample uses ToDictionary to immediately evaluate a sequence and a " & _
                 "related key expression into a dictionary.")> _
    Public Sub DataSetLinq56()

        Dim scoreRecords = TestDS.Tables("ScoreRecords").AsEnumerable()

        'Makes name the key for the dictionary
        Dim scoreRecordsDict = scoreRecords.ToDictionary(Function(sr) sr!Name)
        Console.WriteLine("Bob's score: " & scoreRecordsDict("Bob")("Score"))
    End Sub

    <Category("Converting (ToArray/ToList...)")> _
    <Title("TypeOf")> _
    <Description("This sample uses TypeOf to return only the rows of the DataTable containing a value of type double.")> _
    Public Sub DataSetLinq57()
        Dim numbers = TestDS.Tables("MixedNumbers")

        Dim doubles = From num In numbers _
                      Where TypeOf num!number Is Double

        Console.WriteLine("Numbers stored as doubles:")
        For Each dbl In doubles
            Console.WriteLine(dbl!number)
        Next
    End Sub

#End Region

#Region "Element Operators"

    <Category("Element Operators (First/ElementAt...)")> _
    <Title("First - Simple")> _
    <Description("This sample uses First to return the first matching DataRow, " & _
                 "instead of as a sequence of DataRows.")> _
    Public Sub DataSetLinq58()
        Dim products = TestDS.Tables("Products")

        Dim product12 = (From prod In products _
                         Where prod!ProductID = 12).First

        Console.WriteLine("Product ID: " & product12!ProductId & " UnitsInStock: " & product12!UnitsInStock)

    End Sub

    <Category("Element Operators (First/ElementAt...)")> _
    <Title("First - Condition")> _
    <Description("This sample uses First to find the first DataRow in the DataTable that contains a value that starts with 'o'.")> _
    Public Sub DataSetLinq59()

        Dim strings = TestDS.Tables("Digits").AsEnumerable()

        Dim startsWithO = strings.First(Function(s) s.Field(Of String)("digit")(0) = "o"c)

        Console.WriteLine("A string starting with 'o': " & startsWithO!digit)

    End Sub

    <Category("Element Operators (First/ElementAt...)")> _
    <Title("FirstOrDefault - Simple")> _
    <Description("This sample uses FirstOrDefault to try to return the first DataRow of the source DataTable, " & _
                 "unless there are no DataROws, in which case the default value of nothing is returned ")> _
    Public Sub DataSetLinq61()
        Dim emptyTable = TestDS.Tables("EmptyDataTable").AsEnumerable()

        Dim firstNumOrDefault = emptyTable.FirstOrDefault()

        Console.WriteLine(firstNumOrDefault Is Nothing)
    End Sub

    <Category("Element Operators (First/ElementAt...)")> _
    <Title("ElementAt")> _
    <Description("This sample uses ElementAt to retrieve the second number greater than 5 " & _
                 "from an DataTable.")> _
    Public Sub DataSetLinq64()
        Dim numbers = TestDS.Tables("Numbers")

        ' second number is index 1 because sequences use 0-based indexing
        Dim fourthLowNum = (From num In numbers _
                            Where num!number > 5).ElementAt(1)

        Console.WriteLine("Second number > 5: " & fourthLowNum!number)
    End Sub

#End Region

#Region "Generation Operators"

    <Category("Generation Operators (Range/Repeat)")> _
    <Title("Range")> _
    <Description("This sample uses Range to generate a sequence of numbers from 100 to 149 " & _
                 "that is used to find which numbers in that range are odd and even.")> _
    Public Sub DataSetLinq65()

        Dim numbers = From num In Enumerable.Range(100, 50) _
                      Select Number = num, OddEven = If(num Mod 2 = 1, "odd", "even")

        For Each num In numbers
            Console.WriteLine("The number " & num.Number & " is " & num.OddEven)
        Next
    End Sub


    <Category("Generation Operators (Range/Repeat)")> _
    <Title("Repeat")> _
    <Description("This sample uses Repeat to generate a sequence that contains the number 7 ten times.")> _
    Public Sub DataSetLinq66()

        Dim numbers = Enumerable.Repeat(7, 10)

        For Each num In numbers
            Console.WriteLine(num)
        Next
    End Sub

#End Region

#Region "Quantifiers"

    <Category("Quantifiers (Any/All)")> _
    <Title("Any - Simple")> _
    <Description("This sample uses Any to determine if any of the words in the array " & _
                 "contain the substring 'ei'.")> _
    Public Sub DataSetLinq67()

        Dim words2 = TestDS.Tables("Words2").AsEnumerable()
        Dim iAfterE = words2.Any(Function(word) word.Field(Of String)("word").Contains("ei"))

        Console.WriteLine("There is a word that contains in the list that contains 'ei': " & iAfterE)
    End Sub



    <Category("Quantifiers (Any/All)")> _
    <Title("Any - Grouped")> _
    <Description("This sample uses Any to return a grouped a list of products only for categories " & _
                 "that have at least one product that is out of stock.")> _
    Public Sub DataSetLinq69()

        Dim products = TestDS.Tables("Products")

        Dim productGroups = From prod In products _
                            Group prod By prod!Category _
                            Into ProductGroup = Group _
                            Where ProductGroup.Any(Function(p) p!UnitsInStock = 0) _
                            Select Category, ProductGroup

        For Each pg In productGroups
            Console.WriteLine(pg.Category)

            For Each prod In pg.ProductGroup
                Console.WriteLine(vbTab & prod("ProductName"))
            Next
        Next
    End Sub


    <Category("Quantifiers (Any/All)")> _
    <Title("All - Simple")> _
    <Description("This sample uses All to determine whether an array contains " & _
                 "only odd numbers.")> _
    Public Sub DataSetLinq70()

        Dim numbers = TestDS.Tables("Numbers")

        Dim onlyOdd = Aggregate num In numbers _
                      Into All(num!number Mod 2 = 1)

        Console.WriteLine("The list contains only odd numbers: " & onlyOdd)
    End Sub

    <Category("Quantifiers (Any/All)")> _
    <Title("All - Grouped")> _
    <Description("This sample uses All to return a grouped a list of products only for categories " & _
                 "that have all of their products in stock.")> _
    Public Sub DataSetLinq72()

        Dim products = TestDS.Tables("Products")

        Dim productGroups = From prod In products _
                            Group By Category = prod("Category") _
                            Into ProductGroup = Group, _
                                 AllUnitsInStock = All(prod!UnitsInStock > 0) _
                            Where AllUnitsInStock = True

        For Each pg In productGroups
            Console.WriteLine(pg.Category)

            For Each prod In pg.ProductGroup
                Console.WriteLine(vbTab & prod!ProductName)
            Next
        Next
    End Sub

    <Category("Quantifiers (Any/All)")> _
    <Title("Contains")> _
    <Description("Uses the Contains method to find the row with number = 3")> _
    Public Sub DataSetLinq102()

        Dim numbers = TestDS.Tables("Numbers")

        'Find DataRow with number = 3
        Dim rowToFind As DataRow = Nothing
        For Each row As DataRow In numbers.Rows
            If row!number = 3 Then
                rowToFind = row
                Exit For
            End If
        Next

        Dim foundRow = numbers.AsEnumerable().Contains(rowToFind)

        Console.WriteLine("Found Row: " & foundRow)
    End Sub

#End Region

#Region "Aggregate Operators"

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Count - Simple")> _
    <Description("This sample uses Count to get the number of unique factors of 300.")> _
    Public Sub DataSetLinq73()
        Dim factorsOf300 = TestDS.Tables("FactorsOF300").AsEnumerable()

        Dim uniqueFactors = factorsOf300.Distinct().Count()

        Console.WriteLine("There are " & uniqueFactors & " unique factors of 300.")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Count - Conditional")> _
    <Description("This sample uses Count to get the number of odd Integers in the array.")> _
    Public Sub DataSetLinq74()

        Dim numbers = TestDS.Tables("Numbers")

        Dim oddNumbers = Aggregate num In numbers _
                         Into Count(num!number Mod 2 = 1)

        Console.WriteLine("There are " & oddNumbers & " odd numbers in the list.")
    End Sub


    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Count - Nested")> _
    <Description("This sample uses Count to return a list of customers and how many orders " & _
                 "each has.")> _
    Public Sub DataSetLinq76()
        Dim customers = TestDS.Tables("Customers")

        Dim orderCounts = From cust In customers _
                          Select OrderCount = cust.GetChildRows("CustomersOrders").Count()

        For Each x In orderCounts
            Console.WriteLine("OrderCount: " & x)
        Next
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Count - Grouped")> _
    <Description("This sample uses Count to return a list of categories and how many products " & _
                 "each has.")> _
    Public Sub DataSetLinq77()

        Dim products = TestDS.Tables("Products")

        Dim categoryCounts = From prod In products _
                             Group prod By prod!Category _
                             Into ProductCount = Count() _
                             Select Category, ProductCount

        ObjectDumper.Write(categoryCounts, 1)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Long Count Simple")> _
    <Description("Gets the count as a Long")> _
    Public Sub DataSetLinq103()

        Dim products = TestDS.Tables("Products").AsEnumerable()

        Dim numberOfProducts = products.LongCount()
        Console.WriteLine("There are " & numberOfProducts & " products")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Long Count Conditional")> _
    <Description("This sample uses Count to get the number of odd Integers in the array as a Long")> _
    Public Sub DataSetLinq104()

        Dim numbers = TestDS.Tables("Numbers")

        Dim oddNumbers = Aggregate num In numbers _
                         Into LongCount(num!number Mod 2 = 1)

        Console.WriteLine("There are " & oddNumbers & " odd numbers in the list.")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Sum - Simple")> _
    <Description("This sample uses Sum to get the total of the numbers in an DataTable.")> _
    Public Sub DataSetLinq78()
        Dim numbers = TestDS.Tables("Numbers")

        Dim numSum = Aggregate num In numbers _
                     Into Sum(CInt(num!number))

        Console.WriteLine("The sum of the numbers is " & numSum)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Sum - Projection")> _
    <Description("This sample uses Sum to get the total number of characters of all words " & _
                 "in the array.")> _
    Public Sub DataSetLinq79()

        Dim words = TestDS.Tables("Words").AsEnumerable()

        Dim totalChars = words.Sum(Function(word) CStr(word("word")).Length)
        Console.WriteLine("There are a total of " & totalChars & " characters in these words.")
    End Sub


    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Sum - Grouped")> _
    <Description("This sample uses Sum to get the total units in stock for each product category.")> _
    Public Sub DataSetLinq80()

        Dim products = TestDS.Tables("Products")

        Dim categories = From prod In products _
                         Group prod By prod!Category _
                         Into TotalUnitsInStock = Sum(CDec(prod!UnitsInStock))

        'Dim categories = From prod In products _
        '         Group prod By Key = prod.Field(Of String)("Category") _
        '         Into Group _
        '         Select Category = Key, _
        '                TotalUnitsInStock = Aggregate prod In Group _
        '                                    Into Sum(CDec(prod("UnitsInStock")))

        ObjectDumper.Write(categories, 1)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Min - Simple")> _
    <Description("This sample uses Min to get the lowest number in the DataTable.")> _
    Public Sub DataSetLinq81()

        Dim numbers = TestDS.Tables("Numbers")

        Dim minNum = Aggregate num In numbers _
                     Into Min(CInt(num!number))

        Console.WriteLine("The minimum number is " & minNum & ".")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Min - Grouped")> _
    <Description("This sample uses Min to get the cheapest price among each category's products.")> _
    Public Sub DataSetLinq83()

        Dim products = TestDS.Tables("Products")

        Dim categories = From prod In products _
                         Group prod By Key = prod.Field(Of String)("Category") _
                         Into CheapestPrice = Min(prod.Field(Of Decimal)("UnitPrice"))

        ObjectDumper.Write(categories)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Min - Elements")> _
    <Description("This sample uses Min to get the products with the cheapest price in each category.")> _
    Public Sub DataSetLinq84()

        Dim products = TestDS.Tables("Products")

        Dim categories = From prod In products _
                         Group prod By Category = prod("Category") _
                         Into minPrice = Min(prod.Field(Of Decimal)("UnitPrice")), _
                              Prods = Group _
                         Let CheapestProducts = (From prod In products _
                                                 Where prod("UnitPrice") = minPrice) _
                         Select Category, CheapestProducts

        For Each group In categories
            Console.WriteLine("Category: " & group.Category)
            Console.WriteLine("CheapestProducts:")
            For Each word In group.CheapestProducts
                Console.WriteLine(vbTab & word("ProductName"))
            Next
        Next
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Max - Simple")> _
    <Description("This sample uses Max to get the highest number in an DataTable.")> _
    Public Sub DataSetLinq85()

        Dim numbers = TestDS.Tables("Numbers")

        Dim maxNum = Aggregate row In numbers _
                     Into Max(CInt(row!number))

        'Alternative Syntax
        'Dim maxNum = Aggregate row In numbers _
        '             Into Max(row.Field(Of Integer)("number"))

        Console.WriteLine("The maximum number is " & maxNum & ".")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Max - Projection")> _
    <Description("This sample uses Max to get the length of the longest word in an array.")> _
    Public Sub DataSetLinq86()

        Dim words = TestDS.Tables("Words")

        Dim longestLength = Aggregate word In words _
                            Into Max(word.Field(Of String)("word").Length)
        'Alternative Syntax
        'Dim longestLength = words.Max(Function(w) w("word").Length)

        Console.WriteLine("The longest word is " & longestLength & " characters long.")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Max - Grouped")> _
    <Description("This sample uses Max to get the most expensive price among each category's products.")> _
    Public Sub DataSetLinq87()

        Dim products = TestDS.Tables("Products")
        Dim categories = From prod In products _
                         Group prod By prod!Category _
                         Into Max(CDbl(prod!UnitPrice))

        'Alternative Syntax
        'Dim categories = From prod In products _
        '                 Group prod By Category = prod.Field(Of String)("Category") _
        '                 Into Group _
        '                 Select Category, MostExpensivePrice = CDbl(Group.Max(Function(p) p("UnitPrice")))

        For Each row In categories
            Console.WriteLine(row.Category & ": " & row.Max)
        Next
    End Sub


    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Max - Elements")> _
    <Description("This sample uses Max to get the products with the most expensive price in each category.")> _
    Public Sub DataSetLinq88()

        Dim products = TestDS.Tables("Products")

        Dim categories = From prod In products _
                         Group prod By prod!Category _
                         Into MaxPrice = Max(CDbl(prod!UnitPrice)), Group _
                         Select Category, MostExpensive = From prod In Group _
                                                          Where prod!UnitPrice = MaxPrice

        'Alternative Syntax
        'Dim categories = From prod In products _
        '                 Group prod By Category = prod("Category") Into Group _
        '                 Let maxPrice = Group.Max(Function(p) p("UnitPrice")) _
        '                 Select Category, _
        '                        MostExpensive = Group.Where(Function(p) p("UnitPrice") = maxPrice)

        For Each group In categories
            Console.WriteLine("Category: " & group.Category)
            Console.WriteLine("MostExpensiveProducts:")

            For Each prod In group.MostExpensive
                Console.WriteLine(vbTab & prod("ProductName"))
            Next
        Next

    End Sub


    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Average - Simple")> _
    <Description("This sample uses Average to get the average of all numbers in a DataTable.")> _
    Public Sub DataSetLinq89()
        Dim numbers = TestDS.Tables("Numbers")

        Dim averageNum = (From num In numbers _
                          Select num.Field(Of Integer)("number")).Average()

        Console.WriteLine("The average number is " & averageNum & ".")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Average - Projection")> _
    <Description("This sample uses Average to get the average length of the words in the array.")> _
    Public Sub DataSetLinq90()

        Dim words = TestDS.Tables("Words").AsEnumerable()

        Dim averageLength = words.Average(Function(row) row("word").Length)
        Console.WriteLine("The average word length is " & averageLength & " characters.")
    End Sub


    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Average - Grouped")> _
    <Description("This sample uses Average to get the average price of each category's products.")> _
    Public Sub DataSetLinq91()

        Dim products = TestDS.Tables("Products")

        'Strongly-typed
        Dim categories = From prod In products _
                         Group prod By prod!Category _
                         Into Average(CDbl(prod!UnitPrice))

        'Weakly-typed
        'Dim categories = From prod In products _
        '                 Group prod By prod!Category _
        '                 Into Average(CDbl(prod!UnitPrice))

        For Each row In categories
            Console.WriteLine(row.Category & ": " & row.Average)
        Next
    End Sub


    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Aggregate - Seed")> _
    <Description("This sample uses Aggregate to create a running account balance that " & _
                 "subtracts each withdrawal from the initial balance of 100, as long as " & _
                 "the balance never drops below 0.")> _
    Public Sub DataSetLinq93()

        Dim attemptedWithdrawals = TestDS.Tables("AttemptedWithdrawals").AsEnumerable()

        Dim startBalance = 100.0

        Dim endBalance = attemptedWithdrawals.Aggregate(startBalance, Function(balance, nextWithdrawal) _
                            If(nextWithdrawal("withdrawal") <= balance, _
                               balance - nextWithdrawal("withdrawal"), _
                               balance))

        Console.WriteLine("Ending balance: " & endBalance)
    End Sub

#End Region

#Region "Miscellaneous Operators"

    <Category("Miscellaneous (Concat/SequenceEqual...)")> _
    <Title("Concat - 1")> _
    <Description("This sample uses Concat to create one sequence of DataRows that contains each DataTables's " & _
                 "DataRows, one after the other.")> _
    Public Sub DataSetLinq94()
        Dim numbersA = TestDS.Tables("NumbersA").AsEnumerable()
        Dim numbersB = TestDS.Tables("NumbersB").AsEnumerable()

        Dim allNumbers = numbersA.Concat(numbersB)

        Console.WriteLine("All numbers from both arrays:")
        For Each num In allNumbers
            Console.WriteLine(num!number)
        Next
    End Sub

    <Category("Miscellaneous (Concat/SequenceEqual...)")> _
    <Title("Concat - 2")> _
    <Description("This sample uses Concat to create one sequence that contains the names of " & _
                 "all customers and products, including any duplicates.")> _
    Public Sub DataSetLinq95()

        Dim products = TestDS.Tables("Products")
        Dim customers = TestDS.Tables("Customers")

        Dim customerNames = From cust In customers _
                            Select cust!CompanyName

        Dim productNames = From prod In products _
                           Select prod!ProductName

        Dim allNames = customerNames.Concat(productNames)

        Console.WriteLine("Customer and product names:")
        For Each name In allNames
            Console.WriteLine(name)
        Next
    End Sub

    <Category("Miscellaneous (Concat/SequenceEqual...)")> _
    <Title("SequenceEqual - 1")> _
    <Description("This sample uses EqualAllRows to see if two DataTables match on all Rows " & _
                 "in the same order.")> _
    Public Sub DataSetLinq96()
        Dim wordsA = TestDS.Tables("Words").AsEnumerable()
        Dim wordsB = TestDS.Tables("Words").AsEnumerable()

        Dim match = wordsA.SequenceEqual(wordsB)

        Console.WriteLine("The sequences match: " & match)
    End Sub

    <Category("Miscellaneous (Concat/SequenceEqual...)")> _
    <Title("SequenceEqual - 2")> _
    <Description("This sample uses EqualAll to see if two DataTables match on all Rows " & _
                 "in the same order.")> _
    Public Sub DataSetLinq97()
        Dim wordsA = TestDS.Tables("Words").AsEnumerable()
        Dim wordsB = TestDS.Tables("Words2").AsEnumerable()

        Dim match = wordsA.SequenceEqual(wordsB)

        Console.WriteLine("The sequences match: " & match)
    End Sub


    Function Increment(ByRef i As Integer) As Integer
        i += 1
        Return i
    End Function
    <Category("Custom Sequence Operators")> _
    <Title("Combine")> _
    <Description("This sample uses a user-created sequence operator, Combine, to calculate the " & _
                 "dot product of two vectors.")> _
    <LinkedClass("CustomSequenceOperators")> _
    Public Sub DataSetLinq98()

        Dim numbersA = TestDS.Tables("NumbersA").AsEnumerable()
        Dim numbersB = TestDS.Tables("NumbersB").AsEnumerable()

        Dim dotProduct = numbersA.Combine(numbersB, _
                                          Function(a, b) a.Field(Of Integer)("number") _
                                                       * b.Field(Of Integer)("number")).Sum()
        Console.WriteLine("Dot product: " & dotProduct)
    End Sub


    <Category("Query Execution")> _
    <Title("ToLookup - element selector")> _
    <Description("The following sample shows how queries can be executed immediately with operators " & _
                 "such as ToLookup().")> _
    Public Sub DataSetLinq108()

        Dim products = TestDS.Tables("Products").AsEnumerable()

        Dim productsLookup = products.ToLookup(Function(prod) prod!Category)

        Dim confections = productsLookup("Confections")

        Console.WriteLine("Number of categories: " & productsLookup.Count)
        For Each product In confections
            Console.WriteLine("ProductName: " & product!ProductName)
        Next
    End Sub


    <Category("Query Execution")> _
    <Title("Immediate Execution")> _
    <Description("The following sample shows how queries can be executed immediately with operators " & _
                 "such as ToList().")> _
    Public Sub DataSetLinq100()
        ' Methods like ToList() cause the query to be
        ' executed immediately, caching the results.

        Dim numbers = TestDS.Tables("Numbers")

        Dim i As Integer = 0
        Dim numQuery = (From num In numbers _
                        Select Increment(i)).ToList()

        ' The local variable i has already been fully
        ' incremented before we iterate the results:
        For Each num In numQuery
            Console.WriteLine("v = " & num & ", i = " & i)
        Next
    End Sub

    <Category("Query Execution")> _
    <Title("Query Reuse")> _
    <Description("The following sample shows how, because of deferred execution, queries can be used " & _
                 "again after data changes and will then operate on the new data.")> _
    Public Sub DataSetLinq101()
        ' Deferred execution lets us define a query once
        ' and then reuse it later after data changes.

        Dim numbers = TestDS.Tables("Numbers")
        Dim lowNumbers = From row In numbers _
                         Where row!number <= 3

        Console.WriteLine("First run numbers <= 3:")
        For Each num In lowNumbers
            Console.WriteLine(num!number)
        Next

        For i As Integer = 0 To TestDS.Tables("Numbers").Rows.Count - 1
            TestDS.Tables("Numbers").Rows(i)!number -= 1
        Next

        ' During this second run, the same query object,
        ' lowNumbers, will be iterating over the new state
        ' of numbers(), producing different results:
        Console.WriteLine("Second run numbers <= 3:")
        For Each num In lowNumbers
            Console.WriteLine(num!number)
        Next

    End Sub

#End Region

#Region "DataSet Custom Operators"

    <Category("DataSet Custom Operators")> _
    <Title("ToDataTable - Load existing DataTable")> _
    <Description("Load an existing table from a sequence.")> _
    Public Sub DataSetLinq105()
        Dim customers = TestDS.Tables("Customers")
        Dim orders = TestDS.Tables("Orders")

        Dim myOrders = New DataTable("MyOrders")

        myOrders.Columns.Add("CustomerID", GetType(String))
        myOrders.Columns.Add("OrderID", GetType(Integer))
        myOrders.Columns.Add("Total", GetType(Decimal))

        Dim smallOrders = From cust In customers, ord In orders _
                          Where cust!CustomerID = ord!CustomerID And _
                                ord!Total < 500 _
                          Select cust!CustomerID, ord!OrderID, ord!Total _
                          Take 10

        For Each result In smallOrders
            myOrders.Rows.Add(New Object() {result.CustomerID, result.OrderID, result.Total})
        Next

        PrettyPrintDataTable(myOrders)

    End Sub

#End Region

#Region "DataSet Loading Examples"

    <Category("DataSet Loading examples")> _
    <Title("Using CopyToDataTable")> _
    <Description("Load an existing DataTable with query results")> _
    Public Sub DataSetLinq117a()

        Dim customers = TestDS.Tables("Customers")
        Dim orders = TestDS.Tables("Orders")

        Dim ordQuery = From ord In orders _
                       Where ord!Total < 500.0#


        Dim results = ordQuery.CopyToDataTable()

        PrettyPrintDataTable(results)
    End Sub

    Private Sub PrettyPrintDataTable(ByRef table As DataTable)
        Console.WriteLine("Table: " & table.TableName)

        For Each row In table.Rows
            Dim sb = New StringBuilder()

            For Each dc In table.Columns
                Dim value = row(dc)

                If row.IsNull(dc) Then
                    value = "nothing"
                End If

                sb.AppendFormat("{0} = {1}  ", dc.ColumnName, value)
            Next
            Console.WriteLine(sb.ToString())
        Next
    End Sub

    Private Sub PrettyPrintDataReader(ByRef reader As DbDataReader)
        While reader.Read()
            Dim sb = New StringBuilder()

            For ii As Integer = 0 To reader.FieldCount - 1
                Dim value = reader(ii)

                If reader.IsDBNull(ii) Then
                    value = "nothing"
                End If

                sb.AppendFormat("{0} = {1}  ", reader.GetName(ii), value)
            Next

            Console.WriteLine(sb.ToString())
        End While
    End Sub

#End Region

#Region "LINQ over Typed DataSet"

    <Category("LINQ over TypedDataSet")> _
    <Title("TypedDataSet - Simple")> _
    <Description("Simple query using typed DataSet.")> _
    Public Sub DataSetLinq109()
        Dim taEmployees As New NorthwindDataSetTableAdapters.EmployeesTableAdapter
        Dim employees = taEmployees.GetData()

        Dim results = From emp In employees _
                      Where emp.HireDate.Year > 1993 _
                      Select emp _
                      Order By emp.LastName

        For Each emp In results
            Console.WriteLine("Id = {0}, Name = {1},{2}, Hire Date = {3}", emp.EmployeeID, emp.LastName, emp.FirstName, emp.HireDate)
        Next

    End Sub

    <Category("LINQ over TypedDataSet")> _
    <Title("TypedDataSet - Projection")> _
    <Description("Projection using typed DataSet.")> _
    Public Sub DataSetLinq110()

        Dim taEmployees As New NorthwindDataSetTableAdapters.EmployeesTableAdapter
        Dim employees = taEmployees.GetData()

        Dim results = From emp In employees _
                      Where emp.HireDate.Year > 1993 _
                      Select emp.EmployeeID, Name = emp.LastName & ", " & emp.FirstName, _
                             emp.HireDate

        For Each emp In results
            Console.WriteLine("Id = {0}, Name = {1}, Hire Date = {2}", emp.EmployeeID, emp.Name, emp.HireDate)
        Next

    End Sub

    <Category("LINQ over TypedDataSet")> _
    <Title("TypedDataSet - Load existing DataTable ")> _
    <Description("Load a existing DataTable with results from a Linq query over Typed DataTable ")> _
    Public Sub DataSetLinq111()

        Dim taEmployees As New NorthwindDataSetTableAdapters.EmployeesTableAdapter
        Dim employees = taEmployees.GetData()

        Dim query = From emp In employees _
                    Where emp.HireDate.Year > 1993

        Dim tableWithFilterResults = query.CopyToDataTable()

        PrettyPrintDataTable(tableWithFilterResults)

    End Sub

    <Category("LINQ over TypedDataSet")> _
    <Title("TypedDataSet - Checking for NULL")> _
    <Description("Check for null values before referencing nullable columns in Typed DataSet")> _
    Public Sub DataSetLinq112()

        Dim taOrders As New NorthwindDataSetTableAdapters.OrdersTableAdapter
        Dim orders = taOrders.GetData()


        Dim query = From ord In orders _
                    Where Not (ord.IsShippedDateNull) AndAlso _
                    DateDiff(DateInterval.Day, ord.OrderDate, ord.ShippedDate) > 7

        Dim tableWithFilterResults = query.CopyToDataTable()

        PrettyPrintDataTable(tableWithFilterResults)

    End Sub
    <Category("LINQ over TypedDataSet")> _
    <Title("TypedDataSet - simple")> _
    <Description("Simple queries using typed dataset")> _
    Public Sub DataSetLinq115()

        Dim employees As New EmployeesTable()
        employees.AddEmployeesRow(5, "Jeff Jones", 60000)
        employees.AddEmployeesRow(6, "Geoff Webber", 85000)
        employees.AddEmployeesRow(7, "Alan Fox", 85000)
        employees.AddEmployeesRow(8, "Dwight Schute", 101000)
        employees.AddEmployeesRow(9, "Chaz Hoover", 99999)

        Dim empQuery = From emp In employees _
                       Where emp.Salary >= 85000 _
                       Order By emp.Name

        For Each emp In empQuery
            Console.WriteLine("Id = " & emp.ID & ", Name = " & emp.Name)
        Next
    End Sub


    <Category("LINQ over TypedDataSet")> _
    <Title("TypedDataSet - projection ")> _
    <Description("Projection using typed dataset")> _
    Public Sub DataSetLinq116()

        Dim employees As New EmployeesTable()
        employees.AddEmployeesRow(5, "Jeff Jones", 60000)
        employees.AddEmployeesRow(6, "Geoff Webber", 85000)
        employees.AddEmployeesRow(7, "Alan Fox", 85000)
        employees.AddEmployeesRow(8, "Dwight Schute", 101000)
        employees.AddEmployeesRow(9, "Chaz Hoover", 99999)

        Dim empQuery = From emp In employees _
                       Select EmployeeID = emp.ID, EmployeeName = emp.Name, Employee = emp

        For Each ord In empQuery
            Console.WriteLine("Id = " & ord.EmployeeID & ", Name = " & ord.EmployeeName)
        Next

    End Sub

    <Category("LINQ over TypedDataSet")> _
    <Title("TypedDataSet - load DataTable ")> _
    <Description("Load an existing DataTable with query results")> _
    Public Sub DataSetLinq117()

        Dim employees As New EmployeesTable()
        employees.AddEmployeesRow(5, "Jeff Jones", 60000)
        employees.AddEmployeesRow(6, "Geoff Webber", 85000)
        employees.AddEmployeesRow(7, "Alan Fox", 85000)
        employees.AddEmployeesRow(8, "Dwight Schute", 101000)
        employees.AddEmployeesRow(9, "Chaz Hoover", 99999)

        Dim filteredEmployees As New EmployeesTable()

        Dim empQuery = From emp In employees _
                       Where emp.ID > 7

        empQuery.CopyToDataTable(filteredEmployees, LoadOption.OverwriteChanges)

        PrettyPrintDataTable(filteredEmployees)
    End Sub

#End Region

#Region "Join Operators"

    <Category("Joining (Join/Group Join)")> _
    <Title("Join - Using Nested Loops")> _
    <Description("This query joins two DataTables using nested loops.")> _
    Public Sub DataSetLinq1062()
        Dim customers = TestDS.Tables("Customers")
        Dim orders = TestDS.Tables("Orders")

        Dim joinedResults = From cust In customers, ord In orders _
                            Where cust!CustomerID = ord!CustomerID _
                            Select cust!CustomerID, ord!OrderID, ord!Total

        For Each row In joinedResults
            Console.WriteLine("CustomerID: {0}, OrderID: {1}, OrderDate: {2}", _
                              row.CustomerID, row.OrderID, row.Total)
        Next

    End Sub

    <Category("Joining (Join/Group Join)")> _
    <Title("Join - simple")> _
    <Description("Simple join over two tables")> _
    Public Sub DataSetLinq118()
        Dim customers = TestDS.Tables("Customers").AsEnumerable()
        Dim orders = TestDS.Tables("Orders").AsEnumerable()


        Dim smallOrders = From cust In customers _
                          Join ord In orders _
                              On ord.Field(Of String)("CustomerID") _
                              Equals cust.Field(Of String)("CustomerID") _
                          Select cust!CustomerID, _
                                 ord!OrderID, _
                                 ord!Total

        'Alternative syntax (strongly-typed)
        'Dim smallOrders = From cust In customers _
        '                  Join ord In orders _
        '                  On ord.Field(Of String)("CustomerID") Equals cust.Field(Of String)("CustomerID") _
        '                  Select CustomerID = cust.Field(Of String)("CustomerID"), _
        '                         OrderID = ord.Field(Of Integer)("OrderID"), _
        '                         Total = ord.Field(Of Decimal)("Total")

        ObjectDumper.Write(smallOrders, 1)
    End Sub

    <Category("Joining (Join/Group Join)")> _
    <Title("Join with grouped results")> _
    <Description("Join with grouped results")> _
    Public Sub DataSetLinq119()

        Dim customers = TestDS.Tables("Customers").AsEnumerable()
        Dim orders = TestDS.Tables("Orders").AsEnumerable()

        Dim groupedOrders = From cust In customers _
                            Join ord In orders _
                            On ord.Field(Of String)("CustomerID") Equals cust.Field(Of String)("CustomerID") _
                            Select CustomerID = cust.Field(Of String)("CustomerID"), _
                                   OrderID = ord.Field(Of Integer)("OrderID"), _
                                   Total = ord.Field(Of Decimal)("Total") _
                            Group By OrderID _
                            Into OrderGroup = Group

        For Each group In groupedOrders
            For Each order In group.OrderGroup
                ObjectDumper.Write(order)
            Next
        Next
    End Sub

    <Category("Joining (Join/Group Join)")> _
    <Title("Group Join")> _
    <Description("Simple group join")> _
    Public Sub DataSetLinq120()
        Dim customers = TestDS.Tables("Customers")
        Dim orders = TestDS.Tables("Orders")

        Dim custQuery = From cust In customers _
                        Group Join ord In orders On cust!CustomerID Equals ord!CustomerID _
                        Into Count() _
                        Select cust!CustomerID, ords = Count

        For Each row In custQuery
            Console.WriteLine("CustomerID: {0}  Orders Count: {1}", _
                              row.CustomerID, row.ords)
        Next
    End Sub

#End Region

    Private Sub CreateCustomersTable()
        ' Customer/order data read into memory from XML file using XLinq:
        Dim customerListPath = Path.GetFullPath(Path.Combine(dataPath, "customers.xml"))


        Dim list = System.Xml.Linq.XDocument.Load(customerListPath).Root.Elements("customer")
        Dim customerList = _
            (From cust In list _
             Select New Customer With {.CustomerID = CStr(cust.<id>.Value), _
                                       .CompanyName = CStr(cust.<name>.Value), _
                                       .Address = CStr(cust.<address>.Value), _
                                       .City = CStr(cust.<city>.Value), _
                                       .PostalCode = CStr(cust.<postalcode>.Value), _
                                       .Country = CStr(cust.<country>.Value), _
                                       .Phone = CStr(cust.<phone>.Value), _
                                       .Fax = CStr(cust.<fax>.Value), _
                                       .MyOrders = (From o In cust.<orders>.<order>).ToArray()} _
            ).ToList()


        Dim customers = New DataTable("Customers")
        customers.Columns.Add("CustomerID", GetType(String))
        customers.Columns.Add("CompanyName", GetType(String))
        customers.Columns.Add("Address", GetType(String))
        customers.Columns.Add("City", GetType(String))
        customers.Columns.Add("PostalCode", GetType(String))
        customers.Columns.Add("Country", GetType(String))
        customers.Columns.Add("Phone", GetType(String))
        customers.Columns.Add("Fax", GetType(String))

        TestDS.Tables.Add(customers)

        Dim orders = New DataTable("Orders")

        orders.Columns.Add("OrderID", GetType(Integer))
        orders.Columns.Add("CustomerID", GetType(String))
        orders.Columns.Add("OrderDate", GetType(DateTime))
        orders.Columns.Add("Total", GetType(Decimal))

        TestDS.Tables.Add(orders)

        For Each cust In customerList
            customers.Rows.Add(New Object() {cust.CustomerID, cust.CompanyName, cust.Address, cust.City, _
                                                cust.PostalCode, cust.Country, cust.Phone, cust.Fax})
            For Each e In cust.MyOrders
                orders.Rows.Add(e.<id>.Value, cust.CustomerID, e.<orderdate>.Value, e.<total>.Value)
            Next
        Next

        Dim co = New DataRelation("CustomersOrders", customers.Columns("CustomerID"), orders.Columns("CustomerID"), True)
        TestDS.Relations.Add(co)

    End Sub

    Private Sub CreateProductsTable()

        Dim table As New DataTable("Products")

        table.Columns.Add("ProductID", GetType(Integer))
        table.Columns.Add("ProductName", GetType(String))
        table.Columns.Add("Category", GetType(String))
        table.Columns.Add("UnitPrice", GetType(Decimal))
        table.Columns.Add("UnitsInStock", GetType(Integer))

        Dim productList = New List(Of Product)
        productList.Add(New Product With {.ProductID = 1, .ProductName = "Chai", .Category = "Beverages", .UnitPrice = 18.0, .UnitsInStock = 39})
        productList.Add(New Product With {.ProductID = 2, .ProductName = "Chang", .Category = "Beverages", .UnitPrice = 19.0, .UnitsInStock = 17})
        productList.Add(New Product With {.ProductID = 3, .ProductName = "Aniseed Syrup", .Category = "Condiments", .UnitPrice = 10.0, .UnitsInStock = 13})
        productList.Add(New Product With {.ProductID = 4, .ProductName = "Chef Anton's Cajun Seasoning", .Category = "Condiments", .UnitPrice = 22.0, .UnitsInStock = 53})
        productList.Add(New Product With {.ProductID = 5, .ProductName = "Chef Anton's Gumbo Mix", .Category = "Condiments", .UnitPrice = 21.35, .UnitsInStock = 0})
        productList.Add(New Product With {.ProductID = 6, .ProductName = "Grandma's Boysenberry Spread", .Category = "Condiments", .UnitPrice = 25.0, .UnitsInStock = 120})
        productList.Add(New Product With {.ProductID = 7, .ProductName = "Uncle Bob's Organic Dried Pears", .Category = "Produce", .UnitPrice = 30.0, .UnitsInStock = 15})
        productList.Add(New Product With {.ProductID = 8, .ProductName = "Northwoods Cranberry Sauce", .Category = "Condiments", .UnitPrice = 40.0, .UnitsInStock = 6})
        productList.Add(New Product With {.ProductID = 9, .ProductName = "Mishi Kobe Niku", .Category = "Meat/Poultry", .UnitPrice = 97.0, .UnitsInStock = 29})
        productList.Add(New Product With {.ProductID = 10, .ProductName = "Ikura", .Category = "Seafood", .UnitPrice = 31.0, .UnitsInStock = 31})
        productList.Add(New Product With {.ProductID = 11, .ProductName = "Queso Cabrales", .Category = "Dairy Products", .UnitPrice = 21.0, .UnitsInStock = 22})
        productList.Add(New Product With {.ProductID = 12, .ProductName = "Queso Manchego La Pastora", .Category = "Dairy Products", .UnitPrice = 38.0, .UnitsInStock = 86})
        productList.Add(New Product With {.ProductID = 13, .ProductName = "Konbu", .Category = "Seafood", .UnitPrice = 6.0, .UnitsInStock = 24})
        productList.Add(New Product With {.ProductID = 14, .ProductName = "Tofu", .Category = "Produce", .UnitPrice = 23.25, .UnitsInStock = 35})
        productList.Add(New Product With {.ProductID = 15, .ProductName = "Genen Shouyu", .Category = "Condiments", .UnitPrice = 15.5, .UnitsInStock = 39})
        productList.Add(New Product With {.ProductID = 16, .ProductName = "Pavlova", .Category = "Confections", .UnitPrice = 17.45, .UnitsInStock = 29})
        productList.Add(New Product With {.ProductID = 17, .ProductName = "Alice Mutton", .Category = "Meat/Poultry", .UnitPrice = 39.0, .UnitsInStock = 0})
        productList.Add(New Product With {.ProductID = 18, .ProductName = "Carnarvon Tigers", .Category = "Seafood", .UnitPrice = 62.5, .UnitsInStock = 42})
        productList.Add(New Product With {.ProductID = 19, .ProductName = "Teatime Chocolate Biscuits", .Category = "Confections", .UnitPrice = 9.2, .UnitsInStock = 25})
        productList.Add(New Product With {.ProductID = 20, .ProductName = "Sir Rodney's Marmalade", .Category = "Confections", .UnitPrice = 81.0, .UnitsInStock = 40})
        productList.Add(New Product With {.ProductID = 21, .ProductName = "Sir Rodney's Scones", .Category = "Confections", .UnitPrice = 10.0, .UnitsInStock = 3})
        productList.Add(New Product With {.ProductID = 22, .ProductName = "Gustaf's Knäckebröd", .Category = "Grains/Cereals", .UnitPrice = 21.0, .UnitsInStock = 104})
        productList.Add(New Product With {.ProductID = 23, .ProductName = "Tunnbröd", .Category = "Grains/Cereals", .UnitPrice = 9.0, .UnitsInStock = 61})
        productList.Add(New Product With {.ProductID = 24, .ProductName = "Guaraná Fantástica", .Category = "Beverages", .UnitPrice = 4.5, .UnitsInStock = 20})
        productList.Add(New Product With {.ProductID = 25, .ProductName = "NuNuCa Nuß-Nougat-Creme", .Category = "Confections", .UnitPrice = 14.0, .UnitsInStock = 76})
        productList.Add(New Product With {.ProductID = 26, .ProductName = "Gumbär Gummibärchen", .Category = "Confections", .UnitPrice = 31.23, .UnitsInStock = 15})
        productList.Add(New Product With {.ProductID = 27, .ProductName = "Schoggi Schokolade", .Category = "Confections", .UnitPrice = 43.9, .UnitsInStock = 49})
        productList.Add(New Product With {.ProductID = 28, .ProductName = "Rössle Sauerkraut", .Category = "Produce", .UnitPrice = 45.6, .UnitsInStock = 26})
        productList.Add(New Product With {.ProductID = 29, .ProductName = "Thüringer Rostbratwurst", .Category = "Meat/Poultry", .UnitPrice = 123.79, .UnitsInStock = 0})
        productList.Add(New Product With {.ProductID = 30, .ProductName = "Nord-Ost Matjeshering", .Category = "Seafood", .UnitPrice = 25.89, .UnitsInStock = 10})
        productList.Add(New Product With {.ProductID = 31, .ProductName = "Gorgonzola Telino", .Category = "Dairy Products", .UnitPrice = 12.5, .UnitsInStock = 0})
        productList.Add(New Product With {.ProductID = 32, .ProductName = "Mascarpone Fabioli", .Category = "Dairy Products", .UnitPrice = 32.0, .UnitsInStock = 9})
        productList.Add(New Product With {.ProductID = 33, .ProductName = "Geitost", .Category = "Dairy Products", .UnitPrice = 2.5, .UnitsInStock = 112})
        productList.Add(New Product With {.ProductID = 34, .ProductName = "Sasquatch Ale", .Category = "Beverages", .UnitPrice = 14.0, .UnitsInStock = 111})
        productList.Add(New Product With {.ProductID = 35, .ProductName = "Steeleye Stout", .Category = "Beverages", .UnitPrice = 18.0, .UnitsInStock = 20})
        productList.Add(New Product With {.ProductID = 36, .ProductName = "Inlagd Sill", .Category = "Seafood", .UnitPrice = 19.0, .UnitsInStock = 112})
        productList.Add(New Product With {.ProductID = 37, .ProductName = "Gravad lax", .Category = "Seafood", .UnitPrice = 26.0, .UnitsInStock = 11})
        productList.Add(New Product With {.ProductID = 38, .ProductName = "Côte de Blaye", .Category = "Beverages", .UnitPrice = 263.5, .UnitsInStock = 17})
        productList.Add(New Product With {.ProductID = 39, .ProductName = "Chartreuse verte", .Category = "Beverages", .UnitPrice = 18.0, .UnitsInStock = 69})
        productList.Add(New Product With {.ProductID = 40, .ProductName = "Boston Crab Meat", .Category = "Seafood", .UnitPrice = 18.4, .UnitsInStock = 123})
        productList.Add(New Product With {.ProductID = 41, .ProductName = "Jack's New England Clam Chowder", .Category = "Seafood", .UnitPrice = 9.65, .UnitsInStock = 85})
        productList.Add(New Product With {.ProductID = 42, .ProductName = "Singaporean Hokkien Fried Mee", .Category = "Grains/Cereals", .UnitPrice = 14.0, .UnitsInStock = 26})
        productList.Add(New Product With {.ProductID = 43, .ProductName = "Ipoh Coffee", .Category = "Beverages", .UnitPrice = 46.0, .UnitsInStock = 17})
        productList.Add(New Product With {.ProductID = 44, .ProductName = "Gula Malacca", .Category = "Condiments", .UnitPrice = 19.45, .UnitsInStock = 27})
        productList.Add(New Product With {.ProductID = 45, .ProductName = "Rogede sild", .Category = "Seafood", .UnitPrice = 9.5, .UnitsInStock = 5})
        productList.Add(New Product With {.ProductID = 46, .ProductName = "Spegesild", .Category = "Seafood", .UnitPrice = 12.0, .UnitsInStock = 95})
        productList.Add(New Product With {.ProductID = 47, .ProductName = "Zaanse koeken", .Category = "Confections", .UnitPrice = 9.5, .UnitsInStock = 36})
        productList.Add(New Product With {.ProductID = 48, .ProductName = "Chocolade", .Category = "Confections", .UnitPrice = 12.75, .UnitsInStock = 15})
        productList.Add(New Product With {.ProductID = 49, .ProductName = "Maxilaku", .Category = "Confections", .UnitPrice = 20.0, .UnitsInStock = 10})
        productList.Add(New Product With {.ProductID = 50, .ProductName = "Valkoinen suklaa", .Category = "Confections", .UnitPrice = 16.25, .UnitsInStock = 65})
        productList.Add(New Product With {.ProductID = 51, .ProductName = "Manjimup Dried Apples", .Category = "Produce", .UnitPrice = 53.0, .UnitsInStock = 20})
        productList.Add(New Product With {.ProductID = 52, .ProductName = "Filo Mix", .Category = "Grains/Cereals", .UnitPrice = 7.0, .UnitsInStock = 38})
        productList.Add(New Product With {.ProductID = 53, .ProductName = "Perth Pasties", .Category = "Meat/Poultry", .UnitPrice = 32.8, .UnitsInStock = 0})
        productList.Add(New Product With {.ProductID = 54, .ProductName = "Tourtière", .Category = "Meat/Poultry", .UnitPrice = 7.45, .UnitsInStock = 21})
        productList.Add(New Product With {.ProductID = 55, .ProductName = "Pâté chinois", .Category = "Meat/Poultry", .UnitPrice = 24.0, .UnitsInStock = 115})
        productList.Add(New Product With {.ProductID = 56, .ProductName = "Gnocchi di nonna Alice", .Category = "Grains/Cereals", .UnitPrice = 38.0, .UnitsInStock = 21})
        productList.Add(New Product With {.ProductID = 57, .ProductName = "Ravioli Angelo", .Category = "Grains/Cereals", .UnitPrice = 19.5, .UnitsInStock = 36})
        productList.Add(New Product With {.ProductID = 58, .ProductName = "Escargots de Bourgogne", .Category = "Seafood", .UnitPrice = 13.25, .UnitsInStock = 62})
        productList.Add(New Product With {.ProductID = 59, .ProductName = "Raclette Courdavault", .Category = "Dairy Products", .UnitPrice = 55.0, .UnitsInStock = 79})
        productList.Add(New Product With {.ProductID = 60, .ProductName = "Camembert Pierrot", .Category = "Dairy Products", .UnitPrice = 34.0, .UnitsInStock = 19})
        productList.Add(New Product With {.ProductID = 61, .ProductName = "Sirop d'érable", .Category = "Condiments", .UnitPrice = 28.5, .UnitsInStock = 113})
        productList.Add(New Product With {.ProductID = 62, .ProductName = "Tarte au sucre", .Category = "Confections", .UnitPrice = 49.3, .UnitsInStock = 17})
        productList.Add(New Product With {.ProductID = 63, .ProductName = "Vegie-spread", .Category = "Condiments", .UnitPrice = 43.9, .UnitsInStock = 24})
        productList.Add(New Product With {.ProductID = 64, .ProductName = "Wimmers gute Semmelknödel", .Category = "Grains/Cereals", .UnitPrice = 33.25, .UnitsInStock = 22})
        productList.Add(New Product With {.ProductID = 65, .ProductName = "Louisiana Fiery Hot Pepper Sauce", .Category = "Condiments", .UnitPrice = 21.05, .UnitsInStock = 76})
        productList.Add(New Product With {.ProductID = 66, .ProductName = "Louisiana Hot Spiced Okra", .Category = "Condiments", .UnitPrice = 17.0, .UnitsInStock = 4})
        productList.Add(New Product With {.ProductID = 67, .ProductName = "Laughing Lumberjack Lager", .Category = "Beverages", .UnitPrice = 14.0, .UnitsInStock = 52})
        productList.Add(New Product With {.ProductID = 68, .ProductName = "Scottish Longbreads", .Category = "Confections", .UnitPrice = 12.5, .UnitsInStock = 6})
        productList.Add(New Product With {.ProductID = 69, .ProductName = "Gudbrandsdalsost", .Category = "Dairy Products", .UnitPrice = 36.0, .UnitsInStock = 26})
        productList.Add(New Product With {.ProductID = 70, .ProductName = "Outback Lager", .Category = "Beverages", .UnitPrice = 15.0, .UnitsInStock = 15})
        productList.Add(New Product With {.ProductID = 71, .ProductName = "Flotemysost", .Category = "Dairy Products", .UnitPrice = 21.5, .UnitsInStock = 26})
        productList.Add(New Product With {.ProductID = 72, .ProductName = "Mozzarella di Giovanni", .Category = "Dairy Products", .UnitPrice = 34.8, .UnitsInStock = 14})
        productList.Add(New Product With {.ProductID = 73, .ProductName = "Röd Kaviar", .Category = "Seafood", .UnitPrice = 15.0, .UnitsInStock = 101})
        productList.Add(New Product With {.ProductID = 74, .ProductName = "Longlife Tofu", .Category = "Produce", .UnitPrice = 10.0, .UnitsInStock = 4})
        productList.Add(New Product With {.ProductID = 75, .ProductName = "Rhönbräu Klosterbier", .Category = "Beverages", .UnitPrice = 7.75, .UnitsInStock = 125})
        productList.Add(New Product With {.ProductID = 76, .ProductName = "Lakkalikööri", .Category = "Beverages", .UnitPrice = 18.0, .UnitsInStock = 57})
        productList.Add(New Product With {.ProductID = 77, .ProductName = "Original Frankfurter grüne Soße", .Category = "Condiments", .UnitPrice = 13.0, .UnitsInStock = 32})
        For Each x In productList
            table.Rows.Add(New Object() {x.ProductID, x.ProductName, x.Category, x.UnitPrice, x.UnitsInStock})
        Next

        TestDS.Tables.Add(table)


    End Sub

    Public Class Product
        Public ProductID As Integer
        Public ProductName As String
        Public Category As String
        Public UnitPrice As Decimal
        Public UnitsInStock As Integer
    End Class

    Public Class Customer
        Public CustomerID As String
        Public CompanyName As String
        Public Address As String
        Public City As String
        Public Region As String
        Public PostalCode As String
        Public Country As String
        Public Phone As String
        Public Fax As String
        Public MyOrders As XElement()
    End Class

    Public Class Order
        Public OrderID As Integer
        Public CustomerID As String
        Public OrderDate As DateTime
        Public Total As Decimal
    End Class

    Public Class Pet
        Private _name As String
        Private _owner As String

        Public Sub New(ByVal name As String, ByVal owner As String)
            _name = name
            _owner = owner

        End Sub

        Public Property Name() As String
            Set(ByVal value As String)
                _name = value
            End Set

            Get
                Return _name
            End Get

        End Property

        Public Property Owner() As String
            Set(ByVal value As String)
                _owner = value
            End Set

            Get
                Return _owner
            End Get

        End Property

    End Class

    Public Class Dog
        Inherits Pet

        Private _breed As String

        Public Sub New(ByVal name As String, ByVal owner As String, ByVal breed As String)
            MyBase.New(name, owner)
            _breed = breed

        End Sub

        Public Property Breed() As String
            Set(ByVal value As String)
                _breed = value
            End Set

            Get
                Return _breed
            End Get

        End Property

    End Class

    Public Class ShowDog
        Inherits Dog

        Private _ranking As Integer

        Public Sub New(ByVal name As String, ByVal owner As String, ByVal breed As String, ByVal ranking As Integer)
            MyBase.New(name, owner, breed)
            _ranking = ranking
        End Sub


        Public Property Ranking() As String
            Set(ByVal value As String)
                _ranking = value
            End Set

            Get
                Return _ranking
            End Get

        End Property

    End Class


    Public Class EmployeesTable
        Inherits TypedTableBase(Of EmployeesRow)

        Public ColumnId = New DataColumn("id", GetType(Integer))
        Public ColumnName = New DataColumn("name", GetType(String))
        Public ColumnSalary = New DataColumn("salary", GetType(Decimal))

        Public Sub New()
            Me.TableName = "Employees"
            Me.Columns.Add(ColumnId)
            Me.Columns.Add(ColumnName)
            Me.Columns.Add(ColumnSalary)
            Me.AcceptChanges()
        End Sub

        Public Function AddEmployeesRow(ByVal id As Integer, ByVal name As String, ByVal salary As Decimal) As EmployeesRow
            Dim rowEmployeesRow = Me.NewRow()
            rowEmployeesRow.ItemArray = New Object() {id, name, salary}
            Me.Rows.Add(rowEmployeesRow)
            Return rowEmployeesRow
        End Function

        Protected Overrides Function NewRowFromBuilder(ByVal builder As DataRowBuilder) As DataRow
            Return New EmployeesRow(builder)
        End Function

    End Class

    Public Class EmployeesRow
        Inherits DataRow

        Private employeesTable As EmployeesTable

        Public Sub New(ByVal builder As DataRowBuilder)
            MyBase.New(builder)
            Me.employeesTable = Me.Table
        End Sub

        Public Property ID() As String
            Set(ByVal value As String)
                Me(employeesTable.ColumnId) = value
            End Set

            Get
                Return Me(employeesTable.ColumnId)
            End Get

        End Property


        Public Property Salary() As Decimal
            Set(ByVal value As Decimal)
                Me(employeesTable.ColumnSalary) = value
            End Set

            Get
                Return Me(employeesTable.ColumnSalary)
            End Get

        End Property

        Public Property Name() As String
            Set(ByVal value As String)
                Me(employeesTable.ColumnName) = value
            End Set

            Get
                Return Me(employeesTable.ColumnName)
            End Get

        End Property

    End Class
    Private TestDS As DataSet

    Public Sub New()
        MyBase.New()
        Init()
    End Sub

    Private Sub Init()
        TestDS = New DataSet()

        CreateNumbersTable()
        CreateProductsTable()
        CreateCustomersTable()
        CreateDigitsTable()
        CreateWordsTable()
        CreateNumbersATable()
        CreateNumbersBTable()
        CreateWords2Table()
        CreateDoublesTable()
        CreateWords3Table()
        CreateFactorsOf300Table()
        CreateMixedNumberTable()
        CreateEmptyTable()
        CreateWords4Table()
        CreateAnagramsTable()
        CreateScoreRecordsTable()
        CreateAttemptedWithdrawalsTable()
    End Sub

    Private Function MakeList(Of T)(ByVal ParamArray list() As T) As IEnumerable(Of T)
        Return list
    End Function

    Private Sub CreateScoreRecordsTable()
        Dim scoreRecords = MakeList(New With {.Name = "Alice", .Score = 50}, _
                                    New With {.Name = "Bob", .Score = 40}, _
                                    New With {.Name = "Cathy", .Score = 45})

        Dim table = New DataTable("ScoreRecords")
        table.Columns.Add("Name", GetType(String))
        table.Columns.Add("Score", GetType(Integer))

        For Each r In scoreRecords
            table.Rows.Add(New Object() {r.Name, r.Score})
        Next
        TestDS.Tables.Add(table)
    End Sub

    Private Sub CreateAnagramsTable()
        Dim anagrams = New String() {"from   ", " salt", " earn ", "  last   ", " near ", " form  "}
        Dim table As New DataTable("Anagrams")
        table.Columns.Add("anagram", GetType(String))

        For Each word In anagrams
            table.Rows.Add(New Object() {word})
        Next
        TestDS.Tables.Add(table)
    End Sub

    Private Sub CreateNumbersTable()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}
        Dim table As New DataTable("Numbers")
        table.Columns.Add("number", GetType(Integer))

        For Each n In numbers
            table.Rows.Add(New Object() {n})
        Next
        TestDS.Tables.Add(table)
    End Sub

    Private Sub CreateDigitsTable()
        Dim digits() As String = {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}
        Dim table As New DataTable("Digits")
        table.Columns.Add("digit", GetType(String))

        For Each n In digits
            table.Rows.Add(New Object() {n})
        Next
        TestDS.Tables.Add(table)

    End Sub

    Private Sub CreateWordsTable()
        Dim words() As String = {"aPPLE", "BlUeBeRrY", "cHeRry"}
        Dim table As New DataTable("Words")
        table.Columns.Add("word", GetType(String))

        For Each n In words
            table.Rows.Add(New Object() {n})
        Next
        TestDS.Tables.Add(table)

    End Sub

    Private Sub CreateWords2Table()
        Dim words() As String = {"cherry", "apple", "blueberry"}
        Dim table As New DataTable("Words2")
        table.Columns.Add("word", GetType(String))

        For Each n In words
            table.Rows.Add(New Object() {n})
        Next
        TestDS.Tables.Add(table)

    End Sub

    Private Sub CreateWords3Table()
        Dim words() As String = {"aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry"}
        Dim table As New DataTable("Words3")
        table.Columns.Add("word", GetType(String))

        For Each n In words
            table.Rows.Add(New Object() {n})
        Next
        TestDS.Tables.Add(table)

    End Sub

    Private Sub CreateNumbersATable()
        Dim numbersA() As Integer = {0, 2, 4, 5, 6, 8, 9}
        Dim table As New DataTable("NumbersA")
        table.Columns.Add("number", GetType(Integer))

        For Each n In numbersA
            table.Rows.Add(New Object() {n})
        Next
        TestDS.Tables.Add(table)

    End Sub


    Private Sub CreateNumbersBTable()
        Dim numbersB() As Integer = {1, 3, 5, 7, 8}
        Dim table As New DataTable("NumbersB")
        table.Columns.Add("number", GetType(Integer))

        For Each n In numbersB
            table.Rows.Add(New Object() {n})
        Next
        TestDS.Tables.Add(table)

    End Sub

    Private Sub CreateMixedNumberTable()
        Dim mn() As Object = {Nothing, 1.0, "two", 3, "four", 5, "six", 7.0}
        Dim table As New DataTable("MixedNumbers")
        table.Columns.Add("number", GetType(Object))

        For Each n In mn
            table.Rows.Add(New Object() {n})
        Next
        TestDS.Tables.Add(table)

    End Sub

    Private Sub CreateFactorsOf300Table()
        Dim factors() As Integer = {2, 2, 3, 5, 5}
        Dim table As New DataTable("FactorsOf300")
        table.Columns.Add("factor", GetType(Integer))

        For Each n In factors
            table.Rows.Add(New Object() {n})
        Next
        TestDS.Tables.Add(table)

    End Sub

    Private Sub CreateDoublesTable()
        Dim doubles() As Integer = {1.7, 2.3, 1.9, 4.1, 2.9}
        Dim table As New DataTable("Doubles")
        table.Columns.Add("double", GetType(Double))

        For Each n In doubles
            table.Rows.Add(New Object() {n})
        Next
        TestDS.Tables.Add(table)

    End Sub

    Private Sub CreateEmptyTable()
        Dim table As New DataTable("EmptyDataTable")
        table.Columns.Add("number", GetType(Integer))
        TestDS.Tables.Add(table)

    End Sub

    Private Sub CreateWords4Table()
        Dim words = New String() {"blueberry", "chimpanzee", "abacus", "banana", "apple", "cheese"}
        Dim table As New DataTable("Words4")
        table.Columns.Add("word", GetType(String))

        For Each word In words
            table.Rows.Add(New Object() {word})
        Next
        TestDS.Tables.Add(table)
    End Sub

    Private Sub CreateAttemptedWithdrawalsTable()
        Dim attemptedWithdrawals = New Integer() {20, 10, 40, 50, 10, 70, 30}
        Dim table As New DataTable("AttemptedWithdrawals")
        table.Columns.Add("withdrawal", GetType(Integer))

        For Each r In attemptedWithdrawals
            table.Rows.Add(New Object() {r})
        Next
        TestDS.Tables.Add(table)

    End Sub

End Class

Public Module CustomSequenceOperators
    '        Dim dotProduct = numbersA.Combine(numbersB, Function(a, b) a("number") * b("number")).Sum()
    <Runtime.CompilerServices.Extension()> _
    Public Function Combine(Of T)(ByVal first As IEnumerable(Of DataRow), ByVal second As IEnumerable(Of DataRow), ByVal func As Func(Of DataRow, DataRow, T)) As IEnumerable(Of T)
        Dim combinedList As New List(Of T)
        Using e1 As IEnumerator(Of DataRow) = first.GetEnumerator(), e2 = second.GetEnumerator()
            While e1.MoveNext() AndAlso e2.MoveNext()
                combinedList.Add(func(e1.Current, e2.Current))
            End While
        End Using
        Return combinedList
    End Function
End Module

