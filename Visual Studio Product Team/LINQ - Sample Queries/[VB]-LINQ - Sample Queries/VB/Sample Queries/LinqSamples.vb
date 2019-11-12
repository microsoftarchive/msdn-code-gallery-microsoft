' Copyright Â© Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
' Copyright (c) Microsoft Corporation.  All rights reserved.
Option Explicit Off
Option Infer On
Option Strict On

Imports SampleQueries.SampleSupport
Imports System.IO
Imports System.Reflection
Imports System.Linq
Imports System.Xml.Linq


<Title("LINQ Query Samples"), _
Prefix("Linq")> _
Public Class LinqSamples
    Inherits SampleHarness

    Private dataPath As String = System.Windows.Forms.Application.StartupPath & "\..\..\SampleData\"

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
        Public Orders As Order()
    End Class

    Public Class Order
        Public OrderID As Integer
        Public OrderDate As DateTime
        Public Total As Decimal
    End Class

    Public Class Product
        Public ProductID As Integer
        Public ProductName As String
        Public Category As String
        Public UnitPrice As Decimal
        Public UnitsInStock As Integer
    End Class

    Private productList As List(Of Product)
    Private customerList As List(Of Customer)


    <Category("Your First LINQ Query"), _
    Title("Filtering Numbers"), _
    Description("This returns all the numbers in an array that are less than 5.")> _
    Public Sub Linq1()

        'Creates an array of numbers
        Dim numbers = New Integer() {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        'This query will give us all numbers in the array which are less than 5
        'The variable num is called the "range variable" - it will take turns 
        'representing each value in the array.
        Dim lowNums = From num In numbers _
                      Where num < 5 _
                      Select num

        Console.WriteLine("Numbers < 5:")

        'lowNums now contains only the numbers that are less than 5
        'We can use a loop to print out each of the values
        For Each lowNumber In lowNums
            Console.WriteLine(lowNumber)
        Next
    End Sub

    <Category("Filtering (Where)")> _
    <Title("Where - Simple 1")> _
    <Description("This sample uses a Where clause to find all products that are out of stock.")> _
    Public Sub Linq2()
        Dim products = GetProductList()

        'We can leave off the Select statement when we just want the range variable
        Dim soldOutProducts = From prod In products _
                              Where prod.UnitsInStock = 0 _
                              Select prod

        Console.WriteLine("Sold out products: ")
        For Each prod In soldOutProducts
            Console.WriteLine(prod.ProductName & " is sold out!")
        Next
    End Sub

    <Category("Filtering (Where)")> _
    <Title("Where - Simple 2")> _
    <Description("This sample uses a Where clause to find all products that are in stock and " & _
                 "cost more than 3.00 per unit.")> _
    Public Sub Linq3()
        Dim products = GetProductList()

        Dim expensiveProducts = From prod In products _
                                Where prod.UnitsInStock > 0 AndAlso prod.UnitPrice > 3.0

        Console.WriteLine("In-stock products that cost more than 3.00:")
        For Each prod In expensiveProducts
            Console.WriteLine(prod.ProductName & "is in stock and costs more than 3.00.")
        Next
    End Sub

    <Category("Filtering (Where)")> _
    <Title("Where - Drilldown")> _
    <Description("This sample uses a Where clause to find all customers in Washington " & _
                 "and then uses the resulting sequence to drill down into their orders")> _
    Public Sub Linq4()
        Dim customers = GetCustomerList()

        Dim waCustomers = From cust In customers _
                          Where cust.Region = "WA" _
                          Select cust

        Console.WriteLine("Customers from Washington and their orders:")
        For Each cust In waCustomers
            Console.WriteLine("Customer " & cust.CustomerID & ": " & cust.CompanyName)
            For Each ord In cust.Orders
                Console.WriteLine("  Order " & ord.OrderID & ": " & ord.OrderDate)
            Next
        Next
    End Sub

    <Category("Filtering (Where)")> _
    <Title("Where - Indexed")> _
    <Description("This sample demonstrates an indexed Where clause that returns digits whose name is " & _
                 "shorter than their value.")> _
    Public Sub Linq5()
        Dim digits = New String() {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}

        Dim shortDigits = digits.Where(Function(digit, index) digit.Length < index)

        Console.WriteLine("Short digits:")
        For Each d In shortDigits
            Console.WriteLine("The word " & d & " is shorter than its value.")
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Simple 1")> _
    <Description("This sample uses Select to produce a sequence of Integers one higher than " & _
                 "those in an existing array of Integers.")> _
    Public Sub Linq6()
        Dim numbers = New Integer() {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim numsPlusOne = From num In numbers _
                          Select num + 1

        Console.WriteLine("Numbers + 1:")
        For Each i In numsPlusOne
            Console.WriteLine(i)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Simple 2")> _
    <Description("This sample uses Select to return a sequence of just the names of a list of products.")> _
    Public Sub Linq7()
        Dim products = GetProductList()

        Dim productNames = From prod In products _
                           Select prod.ProductName

        Console.WriteLine("Product Names:")
        For Each productName In productNames
            Console.WriteLine(productName)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Transformation")> _
    <Description("This sample uses Select to produce a sequence of strings representing " & _
                 "the text version of a sequence of Integers.")> _
    Public Sub Linq8()
        Dim numbers = New Integer() {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}
        Dim stringNames = New String() {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}

        Dim textNums = From num In numbers _
                       Select stringNames(num)

        Console.WriteLine("Number strings:")
        For Each s In textNums
            Console.WriteLine(s)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Anonymous Types 1")> _
    <Description("This sample uses Select to produce a sequence of the uppercase " & _
                 "and lowercase versions of each word in the original array.")> _
    Public Sub Linq9()
        Dim words = New String() {"aPPLE", "BlUeBeRrY", "cHeRry"}

        Dim upperLowerWords = From word In words _
                              Select Upper = word.ToUpper(), Lower = word.ToLower()

        'Alternate syntax
        'Dim upperLowerWords = From w In words _
        '                      Select New With {.Upper = w.ToUpper(), .Lower = w.ToLower()}

        For Each ul In upperLowerWords
            Console.WriteLine("Uppercase: " & ul.Upper & ", Lowercase: " & ul.Lower)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Anonymous Types 2")> _
    <Description("This sample uses Select to produce a sequence containing text " & _
                 "representations of digits and whether their length is even or odd.")> _
    Public Sub Linq10()
        Dim numbers As Integer() = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}
        Dim stringNames As String() = {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}

        Dim digitOddEvens = From num In numbers _
                            Select Digit = stringNames(num), Even = ((num Mod 2) = 0)

        For Each d In digitOddEvens
            Console.WriteLine("The digit " & d.Digit & " is " & If(d.Even, "even", "odd"))
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Anonymous Types 3")> _
    <Description("This sample uses Select to produce a sequence containing some properties " & _
                 "of Products, including UnitPrice which is renamed to Price " & _
                 "in the resulting type.")> _
    Public Sub Linq11()
        Dim products = GetProductList()

        Dim productInfos = From prod In products _
                           Select prod.ProductName, prod.Category, Price = prod.UnitPrice

        Console.WriteLine("Product Info:")
        For Each prodInfo In productInfos
            Console.WriteLine("{0} is in the category {1} and costs {2} per unit.", _
                              prodInfo.ProductName, prodInfo.Category, prodInfo.Price)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Indexed")> _
    <Description("This sample uses an indexed Select clause to determine if the value of Integers " & _
                 "in an array match their position in the array.")> _
    Public Sub Linq12()
        Dim numbers As Integer() = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim numsInPlace = numbers.Select(Function(num, index) New With {.Num = num, .InPlace = (num = index)})

        Console.WriteLine("Number: In-place?")
        For Each n In numsInPlace
            Console.WriteLine(n.Num & ": " & n.InPlace)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("Select - Filtered")> _
    <Description("This sample combines Select and Where to make a simple query that returns " & _
                 "the text form of each digit less than 5.")> _
    Public Sub Linq13()
        Dim numbers As Integer() = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}
        Dim digits As String() = {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}

        Dim lowNums = From num In numbers _
                      Where num < 5 _
                      Select digits(num)

        Console.WriteLine("Numbers < 5:")
        For Each num In lowNums
            Console.WriteLine(num)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - Compound From 1")> _
    <Description("This sample uses a compound From clause to make a query that returns all pairs " & _
                 "of numbers from both arrays such that the number from numbersA is less than the number " & _
                 "from numbersB.")> _
    Public Sub Linq14()
        Dim numbersA() As Integer = {0, 2, 4, 5, 6, 8, 9}
        Dim numbersB() As Integer = {1, 3, 5, 7, 8}

        Dim pairs = From a In numbersA, b In numbersB _
                    Where a < b _
                    Select a, b

        Console.WriteLine("Pairs where a < b:")
        For Each pair In pairs
            Console.WriteLine(pair.a & " is less than " & pair.b)
        Next
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - Compound From 2")> _
    <Description("This sample uses a compound From clause to select all orders where the " & _
                 "order total is less than 500.00.")> _
    Public Sub Linq15()
        Dim customers = GetCustomerList()

        Dim orders = From cust In customers, ord In cust.Orders _
                     Where ord.Total < 500.0 _
                     Select cust.CustomerID, ord.OrderID, ord.Total

        ObjectDumper.Write(orders)
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - Compound From 3")> _
    <Description("This sample uses a compound From clause to select all orders where the " & _
                 "order was made in 1998 or later.")> _
    Public Sub Linq16()
        Dim customers = GetCustomerList()

        Dim orders = From cust In customers, ord In cust.Orders _
                     Where ord.OrderDate >= #1/1/1998# _
                     Select cust.CustomerID, ord.OrderID, ord.OrderDate

        ObjectDumper.Write(orders)
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - From Assignment")> _
    <Description("This sample uses a compound From clause to select all orders where the " & _
                 "order total is greater than 2000.00 and uses From assignment to avoid " & _
                 "requesting the total twice.")> _
    Public Sub Linq17()

        Dim customers = GetCustomerList()

        Dim orders = From cust In customers _
                     From ord In cust.Orders _
                     Where ord.Total >= 2000.0# _
                     Select cust.CustomerID, ord.OrderID, ord.Total

        ObjectDumper.Write(orders)
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - Multiple From")> _
    <Description("This sample uses multiple From clauses so that filtering on customers can " & _
                 "be done before selecting their orders.  This makes the query more efficient by " & _
                 "not selecting and then discarding orders for customers outside of Washington.")> _
    Public Sub Linq18()
        Dim customers = GetCustomerList()

        Dim cutoffDate = #1/1/1997#

        Dim orders = From cust In customers, ord In cust.Orders _
                     Where cust.Region = "WA" AndAlso ord.OrderDate >= cutoffDate _
                     Select cust.CustomerID, ord.OrderID

        ObjectDumper.Write(orders)
    End Sub

    <Category("Projecting (Select)")> _
    <Title("SelectMany - Indexed")> _
    <Description("This sample uses an indexed SelectMany clause to select all orders, " & _
                 "while referring to customers by the order in which they are returned " & _
                 "from the query.")> _
    Public Sub Linq19()
        Dim customers = GetCustomerList()

        Dim customerOrders = customers.SelectMany(Function(cust, custIndex) _
                                                    From ord In cust.Orders _
                                                    Select "Customer #" & (custIndex + 1) & _
                                                           " has an order with OrderID " & ord.OrderID)
        ObjectDumper.Write(customerOrders)
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Take - Simple")> _
    <Description("This sample uses Take to get only the first 3 elements of " & _
                 "the array.")> _
    Public Sub Linq20()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim first3Numbers = From num In numbers Take 3

        Console.WriteLine("First 3 numbers:")
        For Each n In first3Numbers
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Take - Nested")> _
    <Description("This sample uses Take to get the first 3 orders from customers " & _
                 "in Washington.")> _
    Public Sub Linq21()
        Dim customers = GetCustomerList()

        Dim first3WAOrders = From cust In customers, ord In cust.Orders _
                             Where cust.Region = "WA" _
                             Select cust.CustomerID, ord.OrderID, ord.OrderDate _
                             Take 3

        Console.WriteLine("First 3 orders in WA:")

        ObjectDumper.Write(first3WAOrders)
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Skip - Simple")> _
    <Description("This sample uses Skip to get all but the first 4 elements of " & _
                 "the array.")> _
    Public Sub Linq22()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim allButFirst4Numbers = From num In numbers Skip 4

        Console.WriteLine("All but first 4 numbers:")
        For Each n In allButFirst4Numbers
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Skip - Nested")> _
    <Description("This sample uses Skip to get all but the first 2 orders from customers " & _
                 "in Washington.")> _
    Public Sub Linq23()
        Dim customers = GetCustomerList()

        Dim allButFirst2Orders = From cust In customers, ord In cust.Orders _
                                 Where cust.Region = "WA" _
                                 Select cust.CustomerID, ord.OrderID, ord.OrderDate _
                                 Skip 2


        Console.WriteLine("All but first 2 orders in WA:")

        ObjectDumper.Write(allButFirst2Orders)
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Take While - Simple")> _
    <Description("This sample uses Take While to return elements starting from the " & _
                 "beginning of the array until a number is hit that is not less than 6.")> _
    Public Sub Linq24()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim firstNumbersLessThan6 = From num In numbers Take While num < 6

        'Alternate syntax
        'Dim firstNumbersLessThan6 = numbers.TakeWhile(Function(n) n < 6)

        Console.WriteLine("First numbers less than 6:")
        For Each n In firstNumbersLessThan6
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Take While - Indexed")> _
    <Description("This sample uses TakeWhile to return elements starting from the " & _
                "beginning of the array until a number is hit that is less than its position " & _
                "in the array.")> _
    Public Sub Linq25()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim firstSmallNumbers = numbers.TakeWhile(Function(n, index) n >= index)

        Console.WriteLine("First numbers not less than their position:")
        For Each n In firstSmallNumbers
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Skip While - Simple")> _
    <Description("This sample uses Skip While to get the elements of the array " & _
                 "starting from the first element divisible by 3.")> _
    Public Sub Linq26()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}
        Dim allButFirst3Numbers = From num In numbers _
                                  Skip While num Mod 3 <> 0

        Console.WriteLine("All elements starting from first element divisible by 3:")
        For Each n In allButFirst3Numbers
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Partitioning (Skip/Take)")> _
    <Title("Skip While - Indexed")> _
    <Description("This sample uses SkipWhile to get the elements of the array " & _
                 "starting from the first element less than its position.")> _
    Public Sub Linq27()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim laterNumbers = numbers.SkipWhile(Function(n, index) n >= index)

        Console.WriteLine("All elements starting from first element less than its position:")
        For Each n In laterNumbers
            Console.WriteLine(n)
        Next
    End Sub


    <Category("Ordering (Order By)")> _
    <Title("Order By - Simple 1")> _
    <Description("This sample uses Order By to sort a list of words alphabetically.")> _
    Public Sub Linq28()
        Dim words() As String = {"cherry", "apple", "blueberry"}

        Dim sortedWords = From word In words _
                          Order By word

        Console.WriteLine("The sorted list of words:")
        For Each w In sortedWords
            Console.WriteLine(w)
        Next
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("Order By - Simple 2")> _
    <Description("This sample uses Order By to sort a list of words by length.")> _
    Public Sub Linq29()
        Dim words = New String() {"cherry", "apple", "blueberry"}

        Dim sortedWords = From word In words _
                          Order By word.Length

        Console.WriteLine("The sorted list of words (by length):")
        For Each w In sortedWords
            Console.WriteLine(w)
        Next

    End Sub

    <Category("Ordering (Order By)")> _
    <Title("Order By - Simple 3")> _
    <Description("This sample uses Order By to sort a list of products by name.")> _
    Public Sub Linq30()
        Dim products = GetProductList()

        Dim sortedProducts = From prod In products _
                             Order By prod.ProductName

        ObjectDumper.Write(sortedProducts)
    End Sub

    Public Class CaseInsensitiveComparer : Implements IComparer(Of String)

        Public Function Compare(ByVal x As String, ByVal y As String) As Integer Implements IComparer(Of String).Compare
            Return String.Compare(x, y, True)
        End Function
    End Class

    <Category("Ordering (Order By)")> _
    <Title("Order By - Comparer")> _
    <Description("This sample uses Order By with a custom comparer to " & _
                 "do a case-insensitive sort of the words in an array.")> _
    <LinkedClass("CaseInsensitiveComparer")> _
    Public Sub Linq31()
        Dim words = New String() {"aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry"}

        Dim sortedWords = words.OrderBy(Function(word) word, New CaseInsensitiveComparer())

        ObjectDumper.Write(sortedWords)
    End Sub


    <Category("Ordering (Order By)")> _
    <Title("Order By Descending - Simple 1")> _
    <Description("This sample uses Order By and Descending to sort a list of " & _
                 "doubles from highest to lowest.")> _
    Public Sub Linq32()
        Dim doubles = New Double() {1.7, 2.3, 1.9, 4.1, 2.9}

        Dim sortedDoubles = From dbl In doubles _
                            Order By dbl Descending

        Console.WriteLine("The doubles from highest to lowest:")
        For Each d In sortedDoubles
            Console.WriteLine(d)
        Next
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("Order By Descending - Simple 2")> _
    <Description("This sample uses Order By to sort a list of products by units in stock " & _
                 "from highest to lowest.")> _
    Public Sub Linq33()
        Dim products = GetProductList()

        Dim sortedProducts = From prod In products _
                             Order By prod.UnitsInStock Descending

        ObjectDumper.Write(sortedProducts)
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("Order By Descending - Comparer")> _
    <Description("This sample uses a Order By with a custom comparer to " & _
                 "do a case-insensitive descending sort of the words in an array.")> _
    <LinkedClass("CaseInsensitiveComparer")> _
    Public Sub Linq34()
        Dim words = New String() {"aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry"}

        Dim sortedWords = words.OrderByDescending(Function(word) word, New CaseInsensitiveComparer())

        ObjectDumper.Write(sortedWords)
    End Sub


    <Category("Ordering (Order By)")> _
    <Title("Compound Order By")> _
    <Description("This sample uses a compound Order By to sort a list of digits, " & _
                 "first by length of their name, and then alphabetically by the name itself.")> _
    Public Sub Linq35()
        Dim digits = New String() {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}

        Dim sortedDigits = From dig In digits _
                           Order By dig.Length, dig

        Console.WriteLine("Sorted digits:")
        For Each d In sortedDigits
            Console.WriteLine(d)
        Next
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("Compound Order By - Ascending then Descending")> _
    <Description("This sample uses a compound Order By to sort first by word length " & _
                 "and then descending alphabetically by the name itself.")> _
    <LinkedClass("CaseInsensitiveComparer")> _
    Public Sub Linq36()
        Dim words() As String = {"aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry"}

        Dim sortedWords = From word In words _
                          Order By word.Length, word Descending

        ObjectDumper.Write(sortedWords)
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("Compound Order By - Descending then Ascending")> _
    <Description("This sample uses a compound orderby to sort a list of products, " & _
                 "first by category (descending), and then by unit price.")> _
    Public Sub Linq37()
        Dim products = GetProductList()

        Dim sortedProducts = From prod In products _
                             Order By prod.Category Descending, prod.UnitPrice

        ObjectDumper.Write(sortedProducts)
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("ThenByDescending - Comparer")> _
    <Description("This sample uses an OrderBy and a ThenBy clause with a custom comparer to " & _
                 "sort first by word length and then by a case-insensitive descending sort " & _
                 "of the words in an array.")> _
    <LinkedClass("CaseInsensitiveComparer")> _
    Public Sub Linq38()
        Dim words() As String = {"aPPLE", "AbAcUs", "bRaNcH", "BlUeBeRrY", "ClOvEr", "cHeRry"}

        Dim sortedWords = words.OrderBy(Function(word) word.Length) _
                               .ThenByDescending(Function(word) word, New CaseInsensitiveComparer())

        ObjectDumper.Write(sortedWords)
    End Sub

    <Category("Ordering (Order By)")> _
    <Title("Reverse")> _
    <Description("This sample uses Reverse to create a list of all digits in the array whose " & _
                 "second letter is 'i' that is reversed from the order in the original array.")> _
    Public Sub Linq39()
        Dim digits() As String = {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}

        Dim IDigits = From dig In digits _
                      Where dig(1) = "i"

        Dim reversedIDigits = IDigits.Reverse()

        Console.WriteLine("A backwards list of the digits with a second character of 'i':")
        For Each d In reversedIDigits
            Console.WriteLine(d)
        Next
    End Sub

    Public Class AnagramEqualityComparer
        Implements IEqualityComparer(Of String)

        Public Shadows Function Equals(ByVal x As String, ByVal y As String) As Boolean Implements IEqualityComparer(Of String).Equals
            Return getCanonicalString(x) = getCanonicalString(y)
        End Function

        Public Shadows Function GetHashCode(ByVal obj As String) As Integer Implements IEqualityComparer(Of String).GetHashCode
            Return getCanonicalString(obj).GetHashCode()
        End Function

        Private Function getCanonicalString(ByVal word As String) As String
            Dim wordChars = word.ToCharArray()
            Array.Sort(wordChars)
            Return New String(wordChars)
        End Function

    End Class

    <Category("Grouping (Group By)")> _
    <Title("Group By - Simple 1")> _
    <Description("This sample uses Group By to partition a list of numbers by " & _
                 "their remainder when divided by 5.")> _
    Public Sub Linq40()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim numberGroups = From num In numbers _
                           Group num By Remainder = num Mod 5 Into NumberGroup = Group _
                           Select Remainder, NumberGroup

        For Each g In numberGroups
            Console.WriteLine("Numbers with a remainder of " & g.Remainder & "when divided by 5:")
            For Each n In g.NumberGroup
                Console.WriteLine(n)
            Next
        Next
    End Sub

    <Category("Grouping (Group By)")> _
    <Title("Group By - Simple 2")> _
    <Description("This sample uses Group By to partition a list of words by " & _
                 "their first letter.")> _
    Public Sub Linq41()
        Dim words = New String() {"blueberry", "chimpanzee", "abacus", "banana", "apple", "cheese"}

        Dim wordGroups = From word In words _
                         Group word By Key = word(0) Into Group _
                         Select FirstLetter = Key, WordGroup = Group

        For Each g In wordGroups
            Console.WriteLine("Words that start with the letter '" & g.FirstLetter & "':")
            For Each w In g.WordGroup
                Console.WriteLine(w)
            Next
        Next

    End Sub

    <Category("Grouping (Group By)")> _
    <Title("Group By - Simple 3")> _
    <Description("This sample uses Group By to partition a list of products by category.")> _
    Public Sub Linq42()
        Dim products = GetProductList()

        Dim orderGroups = From prod In products _
                          Group prod By prod.Category Into Group _
                          Select Category, ProductGroup = Group

        ObjectDumper.Write(orderGroups, 1)
    End Sub

    <Category("Grouping (Group By)")> _
    <Title("Group By - Nested")> _
    <Description("This sample uses Group By to partition a list of each customer's orders, " & _
                 "first by year, and then by month.")> _
    Public Sub Linq43()
        Dim customers = GetCustomerList()

        Dim custOrderGroups = From cust In customers _
                              Select cust.CompanyName, _
                                     Groups = (From ord In cust.Orders _
                                               Group By ord.OrderDate.Year, ord.OrderDate.Month _
                                               Into Group)

        ObjectDumper.Write(custOrderGroups, 3)
    End Sub

    <Category("Grouping (Group By)")> _
    <Title("Group By - Comparer")> _
    <Description("This sample uses GroupBy to partition trimmed elements of an array using " & _
                 "a custom comparer that matches words that are anagrams of each other.")> _
    <LinkedClass("AnagramEqualityComparer")> _
    Public Sub Linq44()
        Dim anagrams() As String = {"from   ", " salt", " earn ", "  last   ", " near ", " form  "}

        Dim orderGroups = anagrams.GroupBy(Function(word) word.Trim(), New AnagramEqualityComparer())

        ObjectDumper.Write(orderGroups, 1)
    End Sub

    <Category("Grouping (Group By)")> _
    <Title("Group By - Comparer, Mapped")> _
    <Description("This sample uses GroupBy to partition trimmed elements of an array using " & _
                 "a custom comparer that matches words that are anagrams of each other, " & _
                 "and then converts the results to uppercase.")> _
    <LinkedClass("AnagramEqualityComparer")> _
    Public Sub Linq45()
        Dim anagrams() As String = {"from   ", " salt", " earn ", "  last   ", " near ", " form  "}

        Dim orderGroups = anagrams.GroupBy(Function(word) word.Trim(), _
                                           Function(word) word.ToUpper(), _
                                           New AnagramEqualityComparer())

        ObjectDumper.Write(orderGroups, 1)
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Distinct - 1")> _
    <Description("This sample uses Distinct to remove duplicate elements in a sequence of " & _
                "factors of 300.")> _
    Public Sub Linq46()
        Dim factorsOf300 = New Integer() {2, 2, 3, 5, 5}

        Dim uniqueFactors = From factor In factorsOf300 Distinct

        Console.WriteLine("Prime factors of 300:")
        For Each f In uniqueFactors
            Console.WriteLine(f)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Distinct - 2")> _
    <Description("This sample uses Distinct to find the unique Category names.")> _
    Public Sub Linq47()
        Dim products = GetProductList()

        Dim categoryNames = From prod In products _
                            Select prod.Category _
                            Distinct

        Console.WriteLine("Category names:")
        For Each n In categoryNames
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Distinct with Key")> _
    <Description("This sample uses Distinct to select each country exactly once, and " & _
                 "pick one company.  The Distinct operator will only operate on the " & _
                 "Country field because it has been marked with the 'Key' modifier.")> _
    Public Sub Linq47b()
        Dim customers = GetCustomerList()

        'Marking Country as the Key field means that the anonymous type will override
        'the Equals method to only compare the Country field (instead of all fields)
        'As a result, when Distinct calls Equals, it will only take the Country field
        'into account.
        Dim cities = From cust In customers _
                     Select New With {Key cust.Country, cust.CompanyName} _
                     Distinct

        'Note that you could get the same results by writing your own custom Comparer
        'and passing it in to the Distinct extension method, but using the Key modifier
        'makes things a lot easier

        ObjectDumper.Write(cities)
    End Sub


    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Union - 1")> _
    <Description("This sample uses Union to create one sequence that contains the unique values " & _
                 "from both arrays.")> _
    Public Sub Linq48()
        Dim numbersA() As Integer = {0, 2, 4, 5, 6, 8, 9}
        Dim numbersB() As Integer = {1, 3, 5, 7, 8}

        Dim uniqueNumbers = numbersA.Union(numbersB)

        Console.WriteLine("Unique numbers from both arrays:")
        For Each n In uniqueNumbers
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Union - 2")> _
    <Description("This sample uses Union to create one sequence that contains the unique first letter " & _
                 "from both product and customer names.")> _
    Public Sub Linq49()
        Dim products = GetProductList()
        Dim customers = GetCustomerList()

        Dim productFirstChars = From prod In products _
                                Select prod.ProductName(0)

        Dim customerFirstChars = From cust In customers _
                                 Select cust.CompanyName(0)

        Dim uniqueFirstChars = productFirstChars.Union(customerFirstChars)

        Console.WriteLine("Unique first letters from Product names and Customer names:")
        For Each ch In uniqueFirstChars
            Console.WriteLine(ch)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Intersect - 1")> _
    <Description("This sample uses Intersect to create one sequence that contains the common values " & _
                 "shared by both arrays.")> _
    Public Sub Linq50()
        Dim numbersA() As Integer = {0, 2, 4, 5, 6, 8, 9}
        Dim numbersB() As Integer = {1, 3, 5, 7, 8}

        Dim commonNumbers = numbersA.Intersect(numbersB)

        Console.WriteLine("Common numbers shared by both arrays:")
        For Each n In commonNumbers
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Intersect - 2")> _
    <Description("This sample uses Intersect to create one sequence that contains the common first letter " & _
                 "from both product and customer names.")> _
    Public Sub Linq51()
        Dim products = GetProductList()
        Dim customers = GetCustomerList()

        Dim productFirstChars = From prod In products _
                                Select prod.ProductName(0)

        Dim customerFirstChars = From cust In customers _
                                 Select cust.CompanyName(0)

        Dim commonFirstChars = productFirstChars.Intersect(customerFirstChars)

        Console.WriteLine("Common first letters from Product names and Customer names:")
        For Each ch In commonFirstChars
            Console.WriteLine(ch)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Except - 1")> _
    <Description("This sample uses Except to create a sequence that contains the values from numbersA" & _
                 "that are not also in numbersB.")> _
    Public Sub Linq52()
        Dim numbersA() As Integer = {0, 2, 4, 5, 6, 8, 9}
        Dim numbersB() As Integer = {1, 3, 5, 7, 8}

        Dim aOnlyNumbers = numbersA.Except(numbersB)

        Console.WriteLine("Numbers in first array but not second array:")
        For Each n In aOnlyNumbers
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Set Operators (Distinct/Union...)")> _
    <Title("Except - 2")> _
    <Description("This sample uses Except to create one sequence that contains the first letters " & _
                 "of product names that are not also first letters of customer names.")> _
    Public Sub Linq53()
        Dim products = GetProductList()
        Dim customers = GetCustomerList()

        Dim productFirstChars = From prod In products _
                                Select prod.ProductName(0)

        Dim customerFirstChars = From cust In customers _
                                 Select cust.CompanyName(0)

        Dim productOnlyFirstChars = productFirstChars.Except(customerFirstChars)

        Console.WriteLine("First letters from Product names, but not from Customer names:")
        For Each ch In productOnlyFirstChars
            Console.WriteLine(ch)
        Next
    End Sub

    <Category("Converting (ToArray/ToList...)")> _
    <Title("ToArray")> _
    <Description("This sample uses ToArray to immediately evaluate a sequence into an array.")> _
    Public Sub Linq54()
        Dim doubles() As Double = {1.7, 2.3, 1.9, 4.1, 2.9}

        Dim sortedDoubles = From dbl In doubles _
                            Order By dbl Descending

        Dim doublesArray = sortedDoubles.ToArray()

        Console.WriteLine("Every other double from highest to lowest:")
        For d As Integer = 0 To doublesArray.Length
            Console.WriteLine(doublesArray(d))
            d += 1
        Next
    End Sub

    <Category("Converting (ToArray/ToList...)")> _
    <Title("ToList")> _
    <Description("This sample uses ToList to immediately evaluate a sequence into a List(Of T).")> _
    Public Sub Linq55()
        Dim words() As String = {"cherry", "apple", "blueberry"}

        Dim sortedWords = From word In words _
                          Order By word

        Dim wordList = sortedWords.ToList()

        Console.WriteLine("The sorted word list:")
        For Each w In wordList
            Console.WriteLine(w)
        Next
    End Sub

    <Category("Converting (ToArray/ToList...)")> _
    <Title("ToDictionary")> _
    <Description("This sample uses ToDictionary to immediately evaluate a sequence and a " & _
                 "related key expression into a dictionary.")> _
    Public Sub Linq56()
        Dim scoreRecords = MakeList(New With {.Name = "Alice", .Score = 50}, _
                                    New With {.Name = "Bob", .Score = 40}, _
                                    New With {.Name = "Cathy", .Score = 45})

        Dim scoreRecordsDict = scoreRecords.ToDictionary(Function(sr) sr.Name)

        Console.WriteLine("Bob's score: " & scoreRecordsDict("Bob").ToString)

    End Sub

    <Category("Converting (ToArray/ToList...)")> _
    <Title("TypeOf")> _
    <Description("This sample uses TypeOf to return only the elements of the array that are of type double.")> _
    Public Sub Linq57()
        Dim numbers = New Object() {Nothing, 1.0, "two", 3, "four", 5, "six", 7.0}

        Dim doubles = From num In numbers _
                      Where TypeOf num Is Double

        Console.WriteLine("Numbers stored as doubles:")
        For Each d In doubles
            Console.WriteLine(d)
        Next
    End Sub

    <Category("Element Operators (First/ElementAt...)")> _
    <Title("First - Simple")> _
    <Description("This sample uses First to return the first matching element " & _
                 "as a Product, instead of as a sequence containing a Product.")> _
    Public Sub Linq58()
        Dim products = GetProductList()

        Dim product12 = From prod In products _
                        Where prod.ProductID = 12 _
                        Take 1

        ObjectDumper.Write(product12)
    End Sub

    <Category("Element Operators (First/ElementAt...)")> _
    <Title("First - Condition")> _
    <Description("This sample uses First to find the first element in the array that starts with 'o'.")> _
    Public Sub Linq59()

        Dim strings = New String() {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine"}

        Dim FirstO = Aggregate str In strings _
                     Into First(str(0) = "o"c)

        'Alternative Syntax
        'Dim FirstO = strings.First(Function(s) s(0) = "o"c)

        Console.WriteLine("A string starting with 'o': " & FirstO)
    End Sub

    <Category("Element Operators (First/ElementAt...)")> _
    <Title("FirstOrDefault - Simple")> _
    <Description("This sample uses FirstOrDefault to try to return the first element of the sequence, " & _
                 "unless there are no elements, in which case the default value for that type " & _
                 "is returned.")> _
    Public Sub Linq61()
        Dim numbers As Integer() = {}

        'Since the array has no elements, the default value (zero) is returned
        Dim firstNumOrDefault = numbers.FirstOrDefault()

        Console.WriteLine(firstNumOrDefault)
    End Sub

    <Category("Element Operators (First/ElementAt...)")> _
    <Title("FirstOrDefault - Condition")> _
    <Description("This sample uses FirstOrDefault to return the first product whose ProductID is 789 " & _
                 "as a single Product object, unless there is no match, in which case Nothing is returned.")> _
    Public Sub Linq62()
        Dim products = GetProductList()

        'This will not return any results since there is no product with ID 789
        Dim product789 = Aggregate prod In products _
                         Into FirstOrDefault(prod.ProductID = 789)

        'Alternative Syntax
        'Dim product789 = products.FirstOrDefault(Function(p) p.ProductID = 789)
        Console.WriteLine("Product 789 exists: " & (product789 IsNot Nothing))
    End Sub


    <Category("Element Operators (First/ElementAt...)")> _
    <Title("ElementAt")> _
    <Description("This sample uses ElementAt to retrieve the second number greater than 5 " & _
                 "from an array.")> _
    Public Sub Linq64()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        'second number is index 1 because sequences use 0-based indexing
        Dim fourthLowNum = (From num In numbers Where num > 5).ElementAt(1)

        Console.WriteLine("Second number > 5: " & fourthLowNum)
    End Sub

    <Category("Generation Operators (Range/Repeat)")> _
    <Title("Range")> _
    <Description("This sample uses Range to generate a sequence of numbers from 100 to 149 " & _
                 "that is used to find which numbers in that range are odd and even.")> _
    Public Sub Linq65()

        'Uses the new ternary If operator to return "odd" or "even"
        Dim numbers = From num In Enumerable.Range(100, 50) _
                      Select Number = num, OddEven = If(num Mod 2 = 1, "odd", "even")

        For Each n In numbers
            Console.WriteLine("The number " & n.Number & " is " & n.OddEven)
        Next

    End Sub

    <Category("Generation Operators (Range/Repeat)")> _
    <Title("Repeat")> _
    <Description("This sample uses Repeat to generate a sequence that contains the number 7 ten times.")> _
    Public Sub Linq66()

        Dim numbers = Enumerable.Repeat(7, 10)

        For Each n In numbers
            Console.WriteLine(n)
        Next
    End Sub


    <Category("Quantifiers (Any/All)")> _
    <Title("Any - Simple")> _
    <Description("This sample uses Any to determine if any of the words in the array " & _
                 "contain the substring 'ei'.")> _
    Public Sub Linq67()

        Dim words() As String = {"believe", "relief", "receipt", "field"}

        Dim iAfterE = Aggregate word In words _
                      Into Any(word.Contains("ei"))

        'Alternative Syntax
        'Dim iAfterE = words.Any(Function(w) w.Contains("ei"))

        Console.WriteLine("There is a word that contains in the list that contains 'ei': " & iAfterE)
    End Sub


    <Category("Quantifiers (Any/All)")> _
    <Title("Any - Grouped")> _
    <Description("This sample uses Any to return a grouped a list of products only for categories " & _
                 "that have at least one product that is out of stock.")> _
    Public Sub Linq69()
        Dim products = GetProductList()

        Dim productGroups = From prod In products _
                            Group prod By prod.Category Into Group _
                            Where Group.Any(Function(p) p.UnitsInStock = 0) _
                            Select Category, ProductGroup = Group

        ObjectDumper.Write(productGroups, 1)
    End Sub

    <Category("Quantifiers (Any/All)")> _
    <Title("All - Simple")> _
    <Description("This sample uses All to determine whether an array contains " & _
                 "only odd numbers.")> _
    Public Sub Linq70()
        Dim numbers() As Integer = {1, 11, 3, 19, 41, 65, 19}

        Dim onlyOdd = Aggregate num In numbers _
                      Into All(num Mod 2 = 1)

        Console.WriteLine("The list contains only odd numbers: " & onlyOdd)
    End Sub

    <Category("Quantifiers (Any/All)")> _
    <Title("All - Grouped")> _
    <Description("This sample uses All to return a grouped a list of products only for categories " & _
                 "that have all of their products in stock.")> _
    Public Sub Linq72()

        Dim products = GetProductList()

        Dim productGroups = From prod In products _
                            Group By prod.Category _
                            Into ProductGroup = Group, _
                                 AllUnitsInStock = All(prod.UnitsInStock > 0) _
                            Where AllUnitsInStock = True _
                            Select Category, ProductGroup

        'Alternative syntax
        'Dim productGroups = From p In products _
        '                    Group p By p.Category Into Group _
        '                    Where (Group.All(Function(p) p.UnitsInStock > 0)) _
        '                    Select Category, ProductGroup = Group

        ObjectDumper.Write(productGroups, 1)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Count - Simple")> _
    <Description("This sample uses Count to get the number of unique factors of 300.")> _
    Public Sub Linq73()
        Dim factorsOf300 = New Integer() {2, 2, 3, 5, 5}

        Dim uniqueFactors = factorsOf300.Distinct().Count()

        Console.WriteLine("There are " & uniqueFactors & " unique factors of 300.")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Count - Conditional")> _
    <Description("This sample uses Count to get the number of odd ints in the array.")> _
    Public Sub Linq74()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim oddNumbers = Aggregate num In numbers _
                         Into Count(num Mod 2 = 1)

        Console.WriteLine("There are " & oddNumbers & " odd numbers in the list.")
    End Sub



    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Count - Nested")> _
    <Description("This sample uses Count to return a list of customers and how many orders " & _
                 "each has.")> _
    Public Sub Linq76()
        Dim customers = GetCustomerList()

        Dim orderCounts = From cust In customers _
                          Select cust.CustomerID, OrderCount = cust.Orders.Count()

        ObjectDumper.Write(orderCounts)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Count - Grouped")> _
    <Description("This sample uses Count to return a list of categories and how many products " & _
                 "each has.")> _
    Public Sub Linq77()
        Dim products = GetProductList()

        Dim categoryCounts = From prod In products _
                             Group prod By prod.Category Into Count() _
                             Select Category, ProductCount = Count

        ObjectDumper.Write(categoryCounts)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Sum - Simple")> _
    <Description("This sample uses Sum to get the total of the numbers in an array.")> _
    Public Sub Linq78()

        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim numSum = numbers.Sum()

        Console.WriteLine("The sum of the numbers is " & numSum)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Sum - Projection")> _
    <Description("This sample uses Sum to get the total number of characters of all words " & _
                 "in the array.")> _
    Public Sub Linq79()
        Dim words = New String() {"cherry", "apple", "blueberry"}

        Dim totalChars = Aggregate word In words _
                         Into Sum(word.Length)

        Console.WriteLine("There are a total of " & totalChars & " characters in these words.")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Sum - Grouped")> _
    <Description("This sample uses Sum to get the total units in stock for each product category.")> _
    Public Sub Linq80()
        Dim products = GetProductList()

        Dim categories = From prod In products _
                         Group prod By prod.Category _
                         Into TotalUnitsInStock = Sum(prod.UnitsInStock)

        'Alternative Syntax
        'Dim categories = From p In products _
        '                 Group p By p.Category Into pGroup = Group _
        '                 Select Category, _
        '                        TotalUnitsInStock = pGroup.Sum(Function(p) p.UnitsInStock)

        ObjectDumper.Write(categories)
    End Sub


    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Min - Simple")> _
    <Description("This sample uses Min to get the lowest number in an array.")> _
    Public Sub Linq81()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim minNum = numbers.Min()

        Console.WriteLine("The minimum number is " & minNum & ".")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Min - Projection")> _
    <Description("This sample uses Min to get the length of the shortest word in an array.")> _
    Public Sub Linq82()
        Dim words = New String() {"cherry", "apple", "blueberry"}

        Dim shortestWord = Aggregate word In words _
                           Into Min(word.Length)

        Console.WriteLine("The shortest word is " & shortestWord & " characters long.")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Min - Grouped")> _
    <Description("This sample uses Min to get the cheapest price among each category's products.")> _
    Public Sub Linq83()
        Dim products = GetProductList()

        Dim categories = From prod In products _
                         Group prod By prod.Category _
                         Into CheapestPrice = Min(prod.UnitPrice) _
                         Select Category, CheapestPrice

        ObjectDumper.Write(categories)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Min - Elements")> _
    <Description("This sample uses Min to get the products with the cheapest price in each category.")> _
    Public Sub Linq84()
        Dim products = GetProductList()

        Dim categories = From prod In products _
                         Group prod By prod.Category _
                         Into Prods = Group, minPrice = Min(prod.UnitPrice) _
                         Let CheapestProducts = (From p In Prods _
                                                 Where p.UnitPrice = minPrice) _
                         Select Category, CheapestProducts

        ObjectDumper.Write(categories, 1)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Max - Simple")> _
    <Description("This sample uses Max to get the highest number in an array.")> _
    Public Sub Linq85()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim maxNum = numbers.Max()

        Console.WriteLine("The maximum number is " & maxNum & ".")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Max - Projection")> _
    <Description("This sample uses Max to get the length of the longest word in an array.")> _
    Public Sub Linq86()
        Dim words = New String() {"cherry", "apple", "blueberry"}

        Dim longestLength = Aggregate word In words _
                            Into Max(word.Length)

        Console.WriteLine("The longest word is " & longestLength & " characters long.")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Max - Grouped")> _
    <Description("This sample uses Max to get the most expensive price among each category's products.")> _
    Public Sub Linq87()
        Dim products = GetProductList()

        Dim categories = From prod In products _
                         Group prod By prod.Category _
                         Into Max(prod.UnitPrice)

        'Alternative Syntax
        'Dim categories = From prod In products _
        '                 Group prod By prod.Category Into Group _
        '                 Select Category, _
        '                        MostExpensivePrice = Aggregate prod In Group _
        '                                             Into Max(prod.UnitPrice)

        ObjectDumper.Write(categories)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Max - Elements")> _
    <Description("This sample uses Max to get the products with the most expensive price in each category.")> _
    Public Sub Linq88()
        Dim products = GetProductList()

        Dim categories = From prod In products _
                         Group prod By prod.Category _
                         Into MaxPrice = Max(prod.UnitPrice), Group _
                         Select Category, MostExpensive = From prod In Group _
                                                          Where prod.UnitPrice = MaxPrice

        'Alternative Syntax
        'Dim categories = From prod In products _
        '                 Group prod By prod.Category Into Group _
        '                 Let maxPrice = Group.Max(Function(p) p.UnitPrice) _
        '                 Select Category, MostExpensiveProducts = Group.Where(Function(p) p.UnitPrice = maxPrice)

        ObjectDumper.Write(categories, 1)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Average - Simple")> _
    <Description("This sample uses Average to get the average of all numbers in an array.")> _
    Public Sub Linq89()
        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim averageNum = numbers.Average()

        Console.WriteLine("The average number is " & averageNum & ".")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Average - Projection")> _
    <Description("This sample uses Average to get the average length of the words in the array.")> _
    Public Sub Linq90()
        Dim words() As String = {"cherry", "apple", "blueberry"}

        Dim averageLength = words.Average(Function(w) w.Length)

        Console.WriteLine("The average word length is " & averageLength & " characters.")
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Average - Grouped")> _
    <Description("This sample uses Average to get the average price of each category's products.")> _
    Public Sub Linq91()
        Dim products = GetProductList()

        Dim categories = From prod In products _
                         Group prod By prod.Category _
                         Into Average(prod.UnitPrice)

        ObjectDumper.Write(categories)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Aggregate - Simple")> _
    <Description("This sample uses Aggregate to create a running product on the array that " & _
                 "calculates the total product of all elements.")> _
    Public Sub Linq92()
        Dim doubles = New Double() {1.7, 2.3, 1.9, 4.1, 2.9}

        Dim product = doubles.Aggregate(Function(runningProduct, nextFactor) runningProduct * nextFactor)

        Console.WriteLine("Total product of all numbers: " & product)
    End Sub

    <Category("Aggregating (Min/Max/Count...)")> _
    <Title("Aggregate - Seed")> _
    <Description("This sample uses Aggregate to create a running account balance that " & _
                 "subtracts each withdrawal from the initial balance of 100, as long as " & _
                 "the balance never drops below 0.")> _
    Public Sub Linq93()
        Dim startBalance = 100.0

        Dim attemptedWithdrawals() As Integer = {20, 10, 40, 50, 10, 70, 30}

        Dim endBalance = _
            attemptedWithdrawals.Aggregate(startBalance, _
                                           Function(balance, nextWithdrawal) _
                                            (If(nextWithdrawal <= balance, (balance - nextWithdrawal), balance)))

        Console.WriteLine("Ending balance: " & endBalance)
    End Sub



    <Category("Miscellaneous (Concat/SequenceEqual...)")> _
    <Title("Concat - 1")> _
    <Description("This sample uses Concat to create one sequence that contains each array's " & _
                 "values, one after the other.")> _
    Public Sub Linq94()
        Dim numbersA() As Integer = {0, 2, 4, 5, 6, 8, 9}
        Dim numbersB() As Integer = {1, 3, 5, 7, 8}

        Dim allNumbers = numbersA.Concat(numbersB)

        Console.WriteLine("All numbers from both arrays:")
        For Each n In allNumbers
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Miscellaneous (Concat/SequenceEqual...)")> _
    <Title("Concat - 2")> _
    <Description("This sample uses Concat to create one sequence that contains the names of " & _
                 "all customers and products, including any duplicates.")> _
    Public Sub Linq95()
        Dim customers = GetCustomerList()
        Dim products = GetProductList()

        Dim customerNames = From cust In customers _
                            Select cust.CompanyName

        Dim productNames = From prod In products _
                           Select prod.ProductName

        Dim allNames = customerNames.Concat(productNames)

        Console.WriteLine("Customer and product names:")
        For Each n In allNames
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Miscellaneous (Concat/SequenceEqual...)")> _
    <Title("EqualAll - 1")> _
    <Description("This sample uses EqualAll to see if two sequences match on all elements " & _
                 "in the same order.")> _
    Public Sub Linq96()
        Dim wordsA() As String = {"cherry", "apple", "blueberry"}
        Dim wordsB() As String = {"cherry", "apple", "blueberry"}

        Dim match = wordsA.SequenceEqual(wordsB)

        Console.WriteLine("The sequences match: " & match)
    End Sub

    <Category("Miscellaneous (Concat/SequenceEqual...)")> _
    <Title("EqualAll - 2")> _
    <Description("This sample uses EqualAll to see if two sequences match on all elements " & _
                 "in the same order.")> _
    Public Sub Linq97()
        Dim wordsA() As String = {"cherry", "apple", "blueberry"}
        Dim wordsB() As String = {"apple", "blueberry", "cherry"}

        Dim match = wordsA.SequenceEqual(wordsB)

        Console.WriteLine("The sequences match: " & match)
    End Sub

    Public Class CustomSequenceOperators
        Public Shared Function Combine(Of T)( _
            ByVal first As IEnumerable(Of T), ByVal second As IEnumerable(Of T), ByVal func As Func(Of T, T, T)) As IEnumerable(Of T)

            Dim list As New List(Of T)
            Using e1 = first.GetEnumerator, e2 = second.GetEnumerator
                While (e1.MoveNext AndAlso e2.MoveNext)
                    list.Add(func(e1.Current, e2.Current))
                End While
            End Using
            Return list
        End Function
    End Class


    <Category("Query Execution")> _
    <Title("Deferred Execution")> _
    <Description("The following sample shows how query execution is deferred until the query is " & _
                 "enumerated at a For Each statement.")> _
    <LinkedFunction("Increment")> _
    Public Sub Linq99()
        ' Sequence operators form first-class queries that
        ' are not executed until you enumerate over them.

        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim i As Integer = 0
        Dim numberQuery = From num In numbers _
                          Select Increment(i)

        ' Note, the local variable 'i' is not incremented
        ' until each element is evaluated (as a side-effect):
        For Each number In numberQuery
            Console.WriteLine("number = " & number & ", i = " & i)
        Next
    End Sub

    Function Increment(ByRef i As Integer) As Integer
        i += 1
        Return i
    End Function

    <Category("Query Execution")> _
    <Title("Immediate Execution")> _
    <Description("The following sample shows how queries can be executed immediately with operators " & _
                 "such as ToList().")> _
    <LinkedFunction("Increment")> _
    Public Sub Linq100()
        ' Methods like ToList() cause the query to be
        ' executed immediately, caching the results.

        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}

        Dim i As Integer = 0
        Dim q = (From num In numbers Select Increment(i)).ToList()

        ' The local variable i has already been fully
        ' incremented before we iterate the results:
        For Each v In q
            Console.WriteLine("v = " & v & ", i = " & i)
        Next
    End Sub

    <Category("Query Execution")> _
    <Title("Query Reuse")> _
    <Description("The following sample shows how, because of deferred execution, queries can be used " & _
                 "again after data changes and will then operate on the new data.")> _
    Public Sub Linq101()
        ' Deferred execution lets us define a query once
        ' and then reuse it later after data changes.

        Dim numbers() As Integer = {5, 4, 1, 3, 9, 8, 6, 7, 2, 0}
        Dim lowNumbers = From num In numbers _
                         Where num <= 3

        Console.WriteLine("First run numbers <= 3:")
        For Each n In lowNumbers
            Console.WriteLine(n)
        Next

        For i As Integer = 0 To numbers.Length - 1
            numbers(i) -= 1
        Next

        ' During this second run, the same query object,
        ' lowNumbers, will be iterating over the new state
        ' of numbers(), producing different results:
        Console.WriteLine("Second run numbers <= 3:")
        For Each n In lowNumbers
            Console.WriteLine(n)
        Next
    End Sub

    <Category("Joining (Join/Group Join)")> _
    <Title("Cross Join")> _
    <Description("This sample shows how to efficiently join elements of two sequences based " & _
                 "on equality between key expressions over the two.")> _
    Public Sub Linq102()
        Dim categories() As String = {"Beverages", "Condiments", "Vegetables", "Dairy Products", "Seafood"}

        Dim products = GetProductList()

        Dim categorizedProducts = From cat In categories _
                                  Join prod In products On cat Equals prod.Category _
                                  Select Category = cat, prod.ProductName

        For Each v In categorizedProducts
            Console.WriteLine(v.ProductName & ": " & v.Category)
        Next
    End Sub


    <Category("Joining (Join/Group Join)")> _
    <Title("Group Join")> _
    <Description("Using a group join you can get all the products that match a given category " & _
                 "bundled as a sequence.")> _
    Public Sub Linq103()
        Dim categories As String() = {"Beverages", "Condiments", "Vegetables", "Dairy Products", "Seafood"}

        Dim productList = GetProductList()

        Dim categorizedProducts = From cat In categories _
                                  Group Join prod In productList On cat Equals prod.Category _
                                  Into Products = Group _
                                  Select Category = cat, Products

        For Each v In categorizedProducts
            Console.WriteLine(v.Category & ":")
            For Each p In v.Products
                Console.WriteLine("   " & p.ProductName)
            Next
        Next

    End Sub

    <Category("Joining (Join/Group Join)")> _
    <Title("Cross Join with Group Join")> _
    <Description("The group join operator is more general than join, as this slightly more verbose " & _
                 "version of the cross join sample shows.")> _
    Public Sub Linq104()
        Dim categories() As String = {"Beverages", "Condiments", "Vegetables", "Dairy Products", "Seafood"}

        Dim productList = GetProductList()

        Dim categorizedProducts = From cat In categories _
                                  Group Join prod In productList On cat Equals prod.Category _
                                  Into Group _
                                  From prod2 In Group _
                                  Select Category = cat, prod2.ProductName

        For Each v In categorizedProducts
            Console.WriteLine(v.ProductName & ": " & v.Category)
        Next
    End Sub

    <Category("Joining (Join/Group Join)")> _
    <Title("Left Outer Join")> _
    <Description("A so-called outer join can be expressed with a group join. A left outer join " & _
                 "is like a cross join, except that all the left hand side elements get included at " & _
                 "least once, even if they don't match any right hand side elements. Note how Vegetables " & _
                 "shows up in the output even though it has no matching products.")> _
    Public Sub Linq105()
        Dim categories() As String = {"Beverages", "Condiments", "Vegetables", "Dairy Products", "Seafood"}

        Dim productList = GetProductList()

        Dim joinResults = From cat In categories _
                          Group Join prod In productList On cat Equals prod.Category Into Group _
                          From prod2 In Group.DefaultIfEmpty() _
                          Select Category = cat, _
                                 ProductName = If(prod2 Is Nothing, "(None)", prod2.ProductName)

        For Each v In joinResults
            Console.WriteLine(v.Category & ": " & v.ProductName)
        Next
    End Sub


    <Category("* Sample Data *")> _
    <Title("CustomerList / ProductList")> _
    <Description("This method displays the sample data used by the queries above.  You can also see " & _
                 "the method below that constructs the lists.  ProductList is built using " & _
                 "a object initializers and CustomerList uses XLinq to read its values " & _
                 "into memory from an XML document.")> _
    <LinkedFunction("GetCustomerList")> _
    <LinkedFunction("GetProductList")> _
    <LinkedMethod("createLists")> _
    Public Sub Linq106()
        ObjectDumper.Write(GetCustomerList(), 1)

        Console.WriteLine()

        ObjectDumper.Write(GetProductList())
    End Sub

    <Category("Task-based Samples"), _
    Title("Working with Dates"), _
    Description("Scenario: You need to fit a dentist appointment into an already-packed schedule. " & _
                "The Dentist has given you a list of dates this month where he can fit you in. " & _
                "Use the following query to determine which dates work for both of you.")> _
    Public Sub Linq107()
        Randomize()

        Dim y = Year(Now)
        Dim m = Month(Now)
        Try

            'Randomly pick 15 appointment dates this month
            Dim DentistDates(15) As Date, AvailableDates(8) As Date
            For i = 0 To UBound(DentistDates)
                DentistDates(i) = New Date(y, m, CInt((Date.DaysInMonth(y, m) - 1) * Rnd() + 1))
            Next

            'Randomly pick 8 dates you're free this month
            For i = 0 To UBound(AvailableDates)
                AvailableDates(i) = New Date(y, m, CInt((Date.DaysInMonth(y, m) - 1) * Rnd() + 1))
            Next

            Dim CommonDates = From day In AvailableDates, day2 In DentistDates _
                              Where day.Date = day2.Date _
                              Order By day _
                              Select day Distinct

            'Just to make it easier to see that the query works, we sort the original lists before display
            Array.Sort(DentistDates)
            Array.Sort(AvailableDates)

            Console.WriteLine("Dentist is free on:")
            ObjectDumper.Write(DentistDates)

            Console.WriteLine(vbNewLine & "You are free on:")
            ObjectDumper.Write(AvailableDates)

            Console.WriteLine(vbNewLine & "Appointment dates that work:")
            For Each d In CommonDates
                Console.WriteLine(Format(d, "MM/dd/yy"))
            Next
        Catch ex As Exception
            Stop
        End Try
    End Sub

    <Category("Task-based Samples"), _
    Title("System.Diagnostics"), _
    Description("This query determines the top 5 memory-using applications currently loaded.")> _
    Public Sub Linq108()

        Dim pList = From proc In System.Diagnostics.Process.GetProcesses() _
                    Select ProcessName = proc.ProcessName, _
                           Size = (Format(proc.WorkingSet64 / 1000, "#,##0") & " KB"), _
                           Size64 = proc.WorkingSet64 _
                    Order By Size64 _
                    Take 5

        Console.WriteLine("These 5 processes are using the most memory:")

        For Each p In pList
            Console.WriteLine(p.ProcessName & " - " & p.Size)
        Next

    End Sub

    <Category("Task-based Samples"), _
    Title("Working with the File System - 1"), _
    Description("This query finds files created within the last year.")> _
    Public Sub Linq110()

        Dim fileList = From file In New DirectoryInfo("C:\").GetFiles() _
                       Where file.CreationTime.AddYears(1) > Now _
                       Order By file.Name

        Console.WriteLine("Files created within the last year:")
        For Each f In fileList
            Console.WriteLine(f.Name)
        Next

    End Sub


    <Category("Task-based Samples"), _
    Title("Working with the File System - 2"), _
    Description("This query shows all mapped network drives")> _
    Public Sub Linq111()

        'Note that if you don't have any network drives mapped, the query will not
        'return any results

        'If you want to see other drive types (i.e. CDRom) please change the enum below
        Dim Drives = From drive In My.Computer.FileSystem.Drives() _
                     Where drive.DriveType = System.IO.DriveType.Network

        Console.WriteLine("Network Drives:")
        For Each d In Drives
            Console.WriteLine(d.Name)
        Next

    End Sub

    <Category("Task-based Samples"), _
    Title("Command Line Arguments"), _
    Description("Uses LINQ to check for the /help command line argument.")> _
    Public Sub Linq112()
        'To experiment with this you can double-click My Project, click on Debug and
        'enter "/help" in the command line arguments textbox

        Dim cmdArgs = Aggregate cmd In My.Application.CommandLineArgs() _
                      Into LongCount(cmd = "/help")

        If cmdArgs = 0 Then
            Console.WriteLine("""/help"" was not entered as a command line argument.")
        Else
            Console.WriteLine("""/help"" requested by user on the command line.")
        End If

    End Sub

    <Category("Task-based Samples"), _
    Title("Working with the Registry - 1"), _
    Description("Shows all keys under HKLM\Software that start with the letter C.")> _
    Public Sub Linq113()

        Dim SubKeys = My.Computer.Registry.LocalMachine.OpenSubKey("Software").GetSubKeyNames

        Dim reg = From regKey In SubKeys _
                  Where regKey.StartsWith("C") _
                  Order By regKey

        ObjectDumper.Write(reg)
    End Sub

    <Category("Task-based Samples"), _
    Title("Working with the Registry - 2"), _
    Description("Shows keys common to HKLM\Software and HKCU\Software")> _
    Public Sub Linq114()

        Dim LMKeys = _
            My.Computer.Registry.LocalMachine.OpenSubKey("Software").GetSubKeyNames

        Dim UserKeys = _
            My.Computer.Registry.CurrentUser.OpenSubKey("Software").GetSubKeyNames

        'Performs an intersection on the two arrays
        Dim reg = From LocalMachineKey In LMKeys, UserKey In UserKeys _
                  Where LocalMachineKey = UserKey _
                  Select LocalMachineKey, UserKey _
                  Order By LocalMachineKey

        Console.WriteLine("Keys common to HKLM\Software and HKCU\Software:")

        For Each r In reg
            Console.WriteLine(r.LocalMachineKey)
        Next
    End Sub

    <Category("Task-based Samples"), _
    Title("Recent Document History"), _
    Description("Shows shortcuts to everything under ""My Recent Documents""")> _
    Public Sub Linq115()

        Dim RecentPath As String = _
            Environment.GetFolderPath(Environment.SpecialFolder.Recent)

        'Uses an anonymous type to reduce the amount of information returned
        Dim Recent = From file In New DirectoryInfo(RecentPath).GetFiles _
                     Select file.Name, file.LastAccessTime

        Console.WriteLine("Shortcuts to My Recent Documents:")

        For Each r In Recent
            Console.WriteLine(r.Name & " - " & r.LastAccessTime)
        Next
    End Sub

    <Category("Task-based Samples"), _
    Title("IE Favorites - 1"), _
    Description("Counts how many items are in your Favorites folder")> _
    Public Sub Linq116()

        Dim FavPath As String = Environment.GetFolderPath(Environment.SpecialFolder.Favorites)

        Dim FavSize = Aggregate file In New DirectoryInfo(FavPath).GetFiles() _
                      Into Count()

        Console.WriteLine("There are " & FavSize & " files in your Favorites folder.")
    End Sub

    <Category("Task-based Samples"), _
    Title("Reflection - 1"), _
    Description("Uses reflection to show all System assemblies which are currently loaded")> _
    Public Sub Linq117()

        Dim Assemblies = From assembly In My.Application.Info.LoadedAssemblies _
                         Where assembly.GetName().Name.StartsWith("System.") _
                         Select assembly.GetName().Name

        Console.WriteLine("Currently-loaded System assemblies:")
        For Each a In Assemblies
            Console.WriteLine(a)
        Next
    End Sub

    <Category("Task-based Samples"), _
    Title("Reflection - 2"), _
    Description("Shows all public methods in this assembly.  Notice that there are duplicates. " & _
                "See Reflection - 3 to see a Distinct list.")> _
    Public Sub Linq118()

        Dim MethodList = From type In Assembly.GetExecutingAssembly.GetTypes(), _
                              method In type.GetMembers() _
                         Where method.MemberType = MemberTypes.Method _
                               AndAlso CType(method, MethodInfo).IsPublic _
                         Select Item = CType(method, MethodInfo) _
                         Order By Item.Name

        For Each m In MethodList
            Console.WriteLine(m.Name)
        Next
    End Sub

    <Category("Task-based Samples"), _
    Title("Reflection - 3"), _
    Description("Shows all public methods in this assembly, with duplicates removed. " & _
                "Notice the use of a nested select.")> _
    Public Sub Linq119()

        Dim NameList = From method In _
                           (From type In Assembly.GetExecutingAssembly.GetTypes(), _
                                 method2 In type.GetMembers() _
                            Where method2.MemberType = MemberTypes.Method AndAlso _
                                  CType(method2, MethodInfo).IsPublic _
                            Select Item = CType(method2, MethodInfo) _
                            Order By Item.Name) _
                       Select method.Name _
                       Distinct

        For Each m In NameList
            Console.WriteLine(m)
        Next

    End Sub

    Public Function GetProductList() As List(Of Product)
        If productList Is Nothing Then
            CreateLists()
        End If

        Return productList
    End Function

    Public Function GetCustomerList() As List(Of Customer)
        If customerList Is Nothing Then
            CreateLists()
        End If

        Return customerList
    End Function

    Private Function MakeList(Of T)(ByVal ParamArray list() As T) As IEnumerable(Of T)
        Return list
    End Function

    Private Sub CreateLists()
        ' Product data created in-memory. 
        ' We could also use the MakeList method above to do this all in one line
        productList = New List(Of Product)
        productList.Add(New Product With {.ProductID = 1, .ProductName = "Chai", .Category = "Beverages", .UnitPrice = 18D, .UnitsInStock = 39})
        productList.Add(New Product With {.ProductID = 2, .ProductName = "Chang", .Category = "Beverages", .UnitPrice = 19D, .UnitsInStock = 17})
        productList.Add(New Product With {.ProductID = 3, .ProductName = "Aniseed Syrup", .Category = "Condiments", .UnitPrice = 10D, .UnitsInStock = 13})
        productList.Add(New Product With {.ProductID = 4, .ProductName = "Chef Anton's Cajun Seasoning", .Category = "Condiments", .UnitPrice = 22D, .UnitsInStock = 53})
        productList.Add(New Product With {.ProductID = 5, .ProductName = "Chef Anton's Gumbo Mix", .Category = "Condiments", .UnitPrice = 21.35D, .UnitsInStock = 0})
        productList.Add(New Product With {.ProductID = 6, .ProductName = "Grandma's Boysenberry Spread", .Category = "Condiments", .UnitPrice = 25D, .UnitsInStock = 120})
        productList.Add(New Product With {.ProductID = 7, .ProductName = "Uncle Bob's Organic Dried Pears", .Category = "Produce", .UnitPrice = 30D, .UnitsInStock = 15})
        productList.Add(New Product With {.ProductID = 8, .ProductName = "Northwoods Cranberry Sauce", .Category = "Condiments", .UnitPrice = 40D, .UnitsInStock = 6})
        productList.Add(New Product With {.ProductID = 9, .ProductName = "Mishi Kobe Niku", .Category = "Meat/Poultry", .UnitPrice = 97D, .UnitsInStock = 29})
        productList.Add(New Product With {.ProductID = 10, .ProductName = "Ikura", .Category = "Seafood", .UnitPrice = 31D, .UnitsInStock = 31})
        productList.Add(New Product With {.ProductID = 11, .ProductName = "Queso Cabrales", .Category = "Dairy Products", .UnitPrice = 21D, .UnitsInStock = 22})
        productList.Add(New Product With {.ProductID = 12, .ProductName = "Queso Manchego La Pastora", .Category = "Dairy Products", .UnitPrice = 38D, .UnitsInStock = 86})
        productList.Add(New Product With {.ProductID = 13, .ProductName = "Konbu", .Category = "Seafood", .UnitPrice = 6D, .UnitsInStock = 24})
        productList.Add(New Product With {.ProductID = 14, .ProductName = "Tofu", .Category = "Produce", .UnitPrice = 23.25D, .UnitsInStock = 35})
        productList.Add(New Product With {.ProductID = 15, .ProductName = "Genen Shouyu", .Category = "Condiments", .UnitPrice = 15.5D, .UnitsInStock = 39})
        productList.Add(New Product With {.ProductID = 16, .ProductName = "Pavlova", .Category = "Confections", .UnitPrice = 17.45D, .UnitsInStock = 29})
        productList.Add(New Product With {.ProductID = 17, .ProductName = "Alice Mutton", .Category = "Meat/Poultry", .UnitPrice = 39D, .UnitsInStock = 0})
        productList.Add(New Product With {.ProductID = 18, .ProductName = "Carnarvon Tigers", .Category = "Seafood", .UnitPrice = 62.5D, .UnitsInStock = 42})
        productList.Add(New Product With {.ProductID = 19, .ProductName = "Teatime Chocolate Biscuits", .Category = "Confections", .UnitPrice = 9.2D, .UnitsInStock = 25})
        productList.Add(New Product With {.ProductID = 20, .ProductName = "Sir Rodney's Marmalade", .Category = "Confections", .UnitPrice = 81D, .UnitsInStock = 40})
        productList.Add(New Product With {.ProductID = 21, .ProductName = "Sir Rodney's Scones", .Category = "Confections", .UnitPrice = 10D, .UnitsInStock = 3})
        productList.Add(New Product With {.ProductID = 22, .ProductName = "Gustaf's Knäckebröd", .Category = "Grains/Cereals", .UnitPrice = 21D, .UnitsInStock = 104})
        productList.Add(New Product With {.ProductID = 23, .ProductName = "Tunnbröd", .Category = "Grains/Cereals", .UnitPrice = 9D, .UnitsInStock = 61})
        productList.Add(New Product With {.ProductID = 24, .ProductName = "Guaraná Fantástica", .Category = "Beverages", .UnitPrice = 4.5D, .UnitsInStock = 20})
        productList.Add(New Product With {.ProductID = 25, .ProductName = "NuNuCa Nuß-Nougat-Creme", .Category = "Confections", .UnitPrice = 14D, .UnitsInStock = 76})
        productList.Add(New Product With {.ProductID = 26, .ProductName = "Gumbär Gummibärchen", .Category = "Confections", .UnitPrice = 31.23D, .UnitsInStock = 15})
        productList.Add(New Product With {.ProductID = 27, .ProductName = "Schoggi Schokolade", .Category = "Confections", .UnitPrice = 43.9D, .UnitsInStock = 49})
        productList.Add(New Product With {.ProductID = 28, .ProductName = "Rössle Sauerkraut", .Category = "Produce", .UnitPrice = 45.6D, .UnitsInStock = 26})
        productList.Add(New Product With {.ProductID = 29, .ProductName = "Thüringer Rostbratwurst", .Category = "Meat/Poultry", .UnitPrice = 123.79D, .UnitsInStock = 0})
        productList.Add(New Product With {.ProductID = 30, .ProductName = "Nord-Ost Matjeshering", .Category = "Seafood", .UnitPrice = 25.89D, .UnitsInStock = 10})
        productList.Add(New Product With {.ProductID = 31, .ProductName = "Gorgonzola Telino", .Category = "Dairy Products", .UnitPrice = 12.5D, .UnitsInStock = 0})
        productList.Add(New Product With {.ProductID = 32, .ProductName = "Mascarpone Fabioli", .Category = "Dairy Products", .UnitPrice = 32D, .UnitsInStock = 9})
        productList.Add(New Product With {.ProductID = 33, .ProductName = "Geitost", .Category = "Dairy Products", .UnitPrice = 2.5D, .UnitsInStock = 112})
        productList.Add(New Product With {.ProductID = 34, .ProductName = "Sasquatch Ale", .Category = "Beverages", .UnitPrice = 14D, .UnitsInStock = 111})
        productList.Add(New Product With {.ProductID = 35, .ProductName = "Steeleye Stout", .Category = "Beverages", .UnitPrice = 18D, .UnitsInStock = 20})
        productList.Add(New Product With {.ProductID = 36, .ProductName = "Inlagd Sill", .Category = "Seafood", .UnitPrice = 19D, .UnitsInStock = 112})
        productList.Add(New Product With {.ProductID = 37, .ProductName = "Gravad lax", .Category = "Seafood", .UnitPrice = 26D, .UnitsInStock = 11})
        productList.Add(New Product With {.ProductID = 38, .ProductName = "Côte de Blaye", .Category = "Beverages", .UnitPrice = 263.5D, .UnitsInStock = 17})
        productList.Add(New Product With {.ProductID = 39, .ProductName = "Chartreuse verte", .Category = "Beverages", .UnitPrice = 18D, .UnitsInStock = 69})
        productList.Add(New Product With {.ProductID = 40, .ProductName = "Boston Crab Meat", .Category = "Seafood", .UnitPrice = 18.4D, .UnitsInStock = 123})
        productList.Add(New Product With {.ProductID = 41, .ProductName = "Jack's New England Clam Chowder", .Category = "Seafood", .UnitPrice = 9.65D, .UnitsInStock = 85})
        productList.Add(New Product With {.ProductID = 42, .ProductName = "Singaporean Hokkien Fried Mee", .Category = "Grains/Cereals", .UnitPrice = 14D, .UnitsInStock = 26})
        productList.Add(New Product With {.ProductID = 43, .ProductName = "Ipoh Coffee", .Category = "Beverages", .UnitPrice = 46D, .UnitsInStock = 17})
        productList.Add(New Product With {.ProductID = 44, .ProductName = "Gula Malacca", .Category = "Condiments", .UnitPrice = 19.45D, .UnitsInStock = 27})
        productList.Add(New Product With {.ProductID = 45, .ProductName = "Rogede sild", .Category = "Seafood", .UnitPrice = 9.5D, .UnitsInStock = 5})
        productList.Add(New Product With {.ProductID = 46, .ProductName = "Spegesild", .Category = "Seafood", .UnitPrice = 12D, .UnitsInStock = 95})
        productList.Add(New Product With {.ProductID = 47, .ProductName = "Zaanse koeken", .Category = "Confections", .UnitPrice = 9.5D, .UnitsInStock = 36})
        productList.Add(New Product With {.ProductID = 48, .ProductName = "Chocolade", .Category = "Confections", .UnitPrice = 12.75D, .UnitsInStock = 15})
        productList.Add(New Product With {.ProductID = 49, .ProductName = "Maxilaku", .Category = "Confections", .UnitPrice = 20D, .UnitsInStock = 10})
        productList.Add(New Product With {.ProductID = 50, .ProductName = "Valkoinen suklaa", .Category = "Confections", .UnitPrice = 16.25D, .UnitsInStock = 65})
        productList.Add(New Product With {.ProductID = 51, .ProductName = "Manjimup Dried Apples", .Category = "Produce", .UnitPrice = 53D, .UnitsInStock = 20})
        productList.Add(New Product With {.ProductID = 52, .ProductName = "Filo Mix", .Category = "Grains/Cereals", .UnitPrice = 7D, .UnitsInStock = 38})
        productList.Add(New Product With {.ProductID = 53, .ProductName = "Perth Pasties", .Category = "Meat/Poultry", .UnitPrice = 32.8D, .UnitsInStock = 0})
        productList.Add(New Product With {.ProductID = 54, .ProductName = "Tourtière", .Category = "Meat/Poultry", .UnitPrice = 7.45D, .UnitsInStock = 21})
        productList.Add(New Product With {.ProductID = 55, .ProductName = "Pâté chinois", .Category = "Meat/Poultry", .UnitPrice = 24D, .UnitsInStock = 115})
        productList.Add(New Product With {.ProductID = 56, .ProductName = "Gnocchi di nonna Alice", .Category = "Grains/Cereals", .UnitPrice = 38D, .UnitsInStock = 21})
        productList.Add(New Product With {.ProductID = 57, .ProductName = "Ravioli Angelo", .Category = "Grains/Cereals", .UnitPrice = 19.5D, .UnitsInStock = 36})
        productList.Add(New Product With {.ProductID = 58, .ProductName = "Escargots de Bourgogne", .Category = "Seafood", .UnitPrice = 13.25D, .UnitsInStock = 62})
        productList.Add(New Product With {.ProductID = 59, .ProductName = "Raclette Courdavault", .Category = "Dairy Products", .UnitPrice = 55D, .UnitsInStock = 79})
        productList.Add(New Product With {.ProductID = 60, .ProductName = "Camembert Pierrot", .Category = "Dairy Products", .UnitPrice = 34D, .UnitsInStock = 19})
        productList.Add(New Product With {.ProductID = 61, .ProductName = "Sirop d'érable", .Category = "Condiments", .UnitPrice = 28.5D, .UnitsInStock = 113})
        productList.Add(New Product With {.ProductID = 62, .ProductName = "Tarte au sucre", .Category = "Confections", .UnitPrice = 49.3D, .UnitsInStock = 17})
        productList.Add(New Product With {.ProductID = 63, .ProductName = "Vegie-spread", .Category = "Condiments", .UnitPrice = 43.9D, .UnitsInStock = 24})
        productList.Add(New Product With {.ProductID = 64, .ProductName = "Wimmers gute Semmelknödel", .Category = "Grains/Cereals", .UnitPrice = 33.25D, .UnitsInStock = 22})
        productList.Add(New Product With {.ProductID = 65, .ProductName = "Louisiana Fiery Hot Pepper Sauce", .Category = "Condiments", .UnitPrice = 21.05D, .UnitsInStock = 76})
        productList.Add(New Product With {.ProductID = 66, .ProductName = "Louisiana Hot Spiced Okra", .Category = "Condiments", .UnitPrice = 17D, .UnitsInStock = 4})
        productList.Add(New Product With {.ProductID = 67, .ProductName = "Laughing Lumberjack Lager", .Category = "Beverages", .UnitPrice = 14D, .UnitsInStock = 52})
        productList.Add(New Product With {.ProductID = 68, .ProductName = "Scottish Longbreads", .Category = "Confections", .UnitPrice = 12.5D, .UnitsInStock = 6})
        productList.Add(New Product With {.ProductID = 69, .ProductName = "Gudbrandsdalsost", .Category = "Dairy Products", .UnitPrice = 36D, .UnitsInStock = 26})
        productList.Add(New Product With {.ProductID = 70, .ProductName = "Outback Lager", .Category = "Beverages", .UnitPrice = 15D, .UnitsInStock = 15})
        productList.Add(New Product With {.ProductID = 71, .ProductName = "Flotemysost", .Category = "Dairy Products", .UnitPrice = 21.5D, .UnitsInStock = 26})
        productList.Add(New Product With {.ProductID = 72, .ProductName = "Mozzarella di Giovanni", .Category = "Dairy Products", .UnitPrice = 34.8D, .UnitsInStock = 14})
        productList.Add(New Product With {.ProductID = 73, .ProductName = "Röd Kaviar", .Category = "Seafood", .UnitPrice = 15D, .UnitsInStock = 101})
        productList.Add(New Product With {.ProductID = 74, .ProductName = "Longlife Tofu", .Category = "Produce", .UnitPrice = 10D, .UnitsInStock = 4})
        productList.Add(New Product With {.ProductID = 75, .ProductName = "Rhönbräu Klosterbier", .Category = "Beverages", .UnitPrice = 7.75D, .UnitsInStock = 125})
        productList.Add(New Product With {.ProductID = 76, .ProductName = "Lakkalikööri", .Category = "Beverages", .UnitPrice = 18D, .UnitsInStock = 57})
        productList.Add(New Product With {.ProductID = 77, .ProductName = "Original Frankfurter grüne Soße", .Category = "Condiments", .UnitPrice = 13D, .UnitsInStock = 32})

        ' Customer/order data read into memory from XML file using XLinq:
        Dim customerListPath = Path.GetFullPath(Path.Combine(dataPath, "customers.xml"))

        Dim list = XDocument.Load(customerListPath).Root.Elements("customer")

        customerList = (From e In list _
                        Select New Customer With { _
                            .CustomerID = CStr(e.<id>.Value), _
                            .CompanyName = CStr(e.<name>.Value), _
                            .Address = CStr(e.<address>.Value), _
                            .City = CStr(e.<city>.Value), _
                            .Region = CStr(e.<region>.Value), _
                            .PostalCode = CStr(e.<postalcode>.Value), _
                            .Country = CStr(e.<country>.Value), _
                            .Phone = CStr(e.<phone>.Value), _
                            .Fax = CStr(e.<fax>.Value), _
                            .Orders = ( _
                                       From o In e...<orders>...<order> _
                                       Select New Order With { _
                                            .OrderID = CInt(o.<id>.Value), _
                                            .OrderDate = CDate(o.<orderdate>.Value), _
                                            .Total = CDec(o.<total>.Value)}).ToArray() _
                        }).ToList()
    End Sub

End Class
