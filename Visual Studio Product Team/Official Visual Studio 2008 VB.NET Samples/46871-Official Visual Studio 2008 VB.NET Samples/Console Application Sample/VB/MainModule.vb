' Copyright (c) Microsoft Corporation. All rights reserved.
Module MainModule

    Private dsProducts As New DataSet
    Private dvProducts As DataView

    Private promptMessage As String = "Enter a product ID to display information or 'QUIT' to end this application."
    Private welcomeMessage As String = "Welcome to How-To Work with the Console Product Finder"


    Sub Main()

        Dim strInput As String
        Dim indexData As Integer

        Console.WriteLine(welcomeMessage)

        ' Populate the dataset, dsProducts, with the products table
        ' and create a sorted view of the products table on the product ID field.
        CreateDataSet()

        Console.WriteLine()
        Console.WriteLine(promptMessage)

        strInput = UCase(Console.ReadLine())
        While strInput <> "QUIT"

            ' Check to ensure that a number was entered as product ID.
            While Not (IsNumeric(strInput) Or UCase(strInput) = "QUIT")
                Console.WriteLine("A numeric product ID is required.")
                Console.WriteLine("Please reenter the Product ID or QUIT to continue.")
                strInput = Console.ReadLine()
            End While

            ' Exit on "quit".
            If UCase(strInput) = "QUIT" Then
                End
            End If

            ' Display the product information if a valid product id was found.
            ' If the product id was not found, display an exception message.
            indexData = dvProducts.Find(strInput)
            If indexData = -1 Then
                Console.WriteLine("No product found.")
            Else
                Console.Write("Product Name: ")
                Console.WriteLine(dvProducts(indexData)("ProductName"))
                Console.Write("Quantity Per Unit: ")
                Console.WriteLine(dvProducts(indexData)("QuantityPerUnit"))
                Console.Write("Unit Price: ")
                Console.WriteLine(dvProducts(indexData)("UnitPrice"))
                Console.Write("Units In Stock: ")
                Console.WriteLine(dvProducts(indexData)("UnitsInStock"))
                Console.Write("Units on Order: ")
                Console.WriteLine(dvProducts(indexData)("UnitsOnOrder"))
                Console.Write("Reorder Level: ")
                Console.WriteLine(dvProducts(indexData)("ReorderLevel"))
                Console.Write("Discontinued: ")
                If CBool(dvProducts(indexData)("Discontinued")) = False Then
                    Console.WriteLine("False")
                Else
                    Console.WriteLine("True")
                End If
            End If
            Console.WriteLine()
            Console.WriteLine(promptMessage)
            strInput = UCase(Console.ReadLine())
        End While
        End

    End Sub

    ''' <summary>
    ''' Reads the Products.xml dataset from the embedded resources of the project.
    ''' </summary>
    Private Sub CreateDataSet()
        Dim thisExe As System.Reflection.Assembly
        thisExe = System.Reflection.Assembly.GetExecutingAssembly()
        Dim xmlFile As System.IO.Stream
        xmlFile = thisExe.GetManifestResourceStream("ConsoleApp.Products.xml")
        dsProducts.ReadXml(xmlFile)
        dvProducts = New DataView(dsProducts.Tables(0))
        dvProducts.Sort = "ProductID"
    End Sub
End Module
