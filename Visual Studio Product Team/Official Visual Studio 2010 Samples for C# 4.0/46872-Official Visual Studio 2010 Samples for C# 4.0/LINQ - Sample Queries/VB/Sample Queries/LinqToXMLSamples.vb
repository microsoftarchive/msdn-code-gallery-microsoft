' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
'
' Copyright (c) Microsoft Corporation.  All rights reserved.
Option Infer On
Option Strict On

Imports System.Xml.Linq
Imports System.Xml
Imports System.IO
Imports System.Linq
Imports System.Linq.Enumerable
Imports System.Text.RegularExpressions
Imports SampleQueries.SampleSupport
Imports SampleQueries.LinqToXMLSamples
Imports <xmlns:f="fff">


<Title("LINQ to XML Samples"), _
Prefix("XLinq")> _
Public Class LinqToXMLSamples
    Inherits SampleHarness

    Private Shared ReadOnly dataPath As String = Path.GetFullPath(System.Windows.Forms.Application.StartupPath & "\..\..\SampleData\")

    <Category("Load"), _
    Title("Load document from file"), _
    Description("Load an XML document from a file")> _
    Public Sub XLinq1()
        Dim doc = XDocument.Load(dataPath & "bib.xml")
        Console.WriteLine(doc)
    End Sub

    <Category("Load"), _
    Title("Load document from string"), _
    Description("Load document from literal")> _
    Public Sub XLinq2()
        Dim doc = _
            <book price="100" isbn="1002310">
                <title>XClarity Samples</title>
                <author>Matt</author>
            </book>

        Console.WriteLine(doc)
    End Sub

    <Category("Load"), _
    Title("Load document from XmlReader"), _
    Description("Load an XML document from XmlReader")> _
    Public Sub XLinq3()
        Dim reader = XmlReader.Create(dataPath & "bib.xml")
        Dim doc = XDocument.Load(reader)
        Console.WriteLine(doc)
    End Sub

    <Category("Load"), _
    Title("Element from XmlReader - 1"), _
    Description("Construct XElement from XmlReader positioned on the root")> _
    Public Sub XLinq4()
        Dim reader As XmlReader = XmlReader.Create(dataPath & "nw_customers.xml")
        reader.Read() ' move to root
        Dim c = XElement.Load(reader)
        Console.WriteLine(c)
    End Sub

    <Category("Load"), _
    Title("Element from XmlReader - 2"), _
    Description("Read XElement content from XmlReader not positioned on an Element")> _
    Public Sub XLinq5()
        Dim reader As XmlReader = XmlReader.Create(dataPath & "config.xml") 'the file has comments and whitespace at the start
        reader.Read()
        reader.Read()
        Dim config As XElement = <appSettings>This content will be replaced</appSettings>

        config.Add(XElement.Load(reader))
        Console.WriteLine(config)
    End Sub

    <Category("Construction"), _
    Title("Construct an XElement from string"), _
    Description("Construct an XElement from literal")> _
    Public Sub XLinq6()
        Dim xml = _
            <purchaseOrder price="100">
                <item price="50">Motor</item>
                <item price="50">Cable</item>
            </purchaseOrder>

        Console.WriteLine(xml)
    End Sub

    <Category("Construction"), _
    Title("Add XML declaration to a document"), _
    Description("Add XML declaration to a document")> _
    Public Sub XLinq7()
        Dim doc = <?xml version="1.0" encoding="UTF-16" standalone="yes"?><foo/>

        Dim sw = New StringWriter()
        doc.Save(sw)
        Console.WriteLine(sw)
    End Sub

    <Category("Construction"), _
    Title("Computed element name"), _
    Description("Computed element name")> _
    Public Sub XLinq8()
        Dim customers = XDocument.Load(dataPath & "nw_customers.xml")
        Dim name As String = customers.<Root>.<Customers>.@CustomerID
        Dim result As XElement = <<%= name %>>Element with a computed name</>
        Console.WriteLine(result)
    End Sub

    <Category("Construction"), _
    Title("Document creation"), _
    Description("Create a simple config file")> _
    Public Sub XLinq9()
        Dim myDocument = <?xml version="1.0"?>
                         <configuration>
                             <system.web>
                                 <membership>
                                     <providers>
                                         <add name="WebAdminMembershipProvider" type="System.Web.Administration.WebAdminMembershipProvider"/>
                                     </providers>
                                 </membership>
                                 <httpModules>
                                     <add name="WebAdminModule" type="System.Web.Administration.WebAdminModule"/>
                                 </httpModules>
                                 <authentication mode="Windows"/>
                                 <authorization>
                                     <deny users="?"/>
                                 </authorization>
                                 <identity impersonate="true"/>
                                 <trust level="full"/>
                                 <pages validationRequest="true"/>
                             </system.web>
                         </configuration>

        Console.WriteLine(myDocument)
    End Sub

    <Category("Construction"), _
    Title("Create an XmlSchema"), _
    Description("Create an XmlSchema")> _
    Public Sub XLinq10()
        Dim result = _
        <xsd:schema xmlns:xsd="http://www.w3.org/2001/XMLSchema"
            xmlns:sql="urn:schemas-microsoft-com:mapping-schema">

            <xsd:element name="root" sql:is-constant="1">
                <xsd:complexType>
                    <xsd:sequence>
                        <xsd:element name="Customers" minOccurs="100" maxOccurs="unbounded">
                            <xsd:complexType>
                                <xsd:sequence>
                                    <xsd:element name="CompanyName" type="xsd:string"/>
                                    <xsd:element name="ContactName" type="xsd:string"/>
                                    <xsd:element name="ContactTitle" type="xsd:string"/>
                                    <xsd:element name="Phone" type="xsd:string"/>
                                    <xsd:element name="Fax" type="xsd:string"/>
                                    <xsd:element ref="FullAddress" maxOccurs="3"/>
                                    <xsd:element name="Date" type="xsd:date"/>
                                </xsd:sequence>
                                <xsd:attribute name="CustomerID" type="xsd:integer"/>
                            </xsd:complexType>
                        </xsd:element>
                    </xsd:sequence>
                </xsd:complexType>
            </xsd:element>
            <xsd:element name="FullAddress" sql:relation="Customers" sql:relationship="CustAdd" sql:key-fields="CustomerID">
                <xsd:complexType>
                    <xsd:sequence>
                        <xsd:element name="Address" type="xsd:string"/>
                        <xsd:element name="City" type="xsd:string"/>
                        <xsd:element name="Region" type="xsd:string"/>
                        <xsd:element name="PostalCode" type="xsd:string"/>
                        <xsd:element name="Country" type="xsd:string"/>
                    </xsd:sequence>
                </xsd:complexType>
            </xsd:element>
        </xsd:schema>
        Console.WriteLine(result)
    End Sub

    <Category("Construction"), _
    Title("Construct a PI"), _
    Description("Create an XML document with an XSLT PI")> _
    Public Sub XLinq11()
        Dim result = <?xml version="1.0"?>
                     <?xml-stylesheet type='text/xsl' href='diff.xsl'?>
                     <foo/>

        Console.WriteLine(result)
    End Sub

    <Category("Construction"), _
    Title("XML comment construction"), _
    Description("XML comment construction")> _
    Public Sub XLinq12()
        Dim result = <?xml version="1.0"?>
                     <!--My phone book-->
                     <phoneBook>
                         <!--My friends-->
                         <Contact name="Ralph">
                             <homephone>425-234-4567</homephone>
                             <cellphone>206-345-75656</cellphone>
                         </Contact>
                         <Contact name="Dave">
                             <homephone>516-756-9454</homephone>
                             <cellphone>516-762-1546</cellphone>
                         </Contact>
                         <!--My family-->
                         <Contact name="Julia">
                             <homephone>425-578-1053</homephone>
                             <cellphone></cellphone>
                         </Contact>
                         <!--My team-->
                         <Contact name="Robert">
                             <homephone>345-565-1653</homephone>
                             <cellphone>321-456-2567</cellphone>
                         </Contact>
                     </phoneBook>


        Console.WriteLine(result)
    End Sub

    <Category("Construction"), _
    Title("Create a CData section"), _
    Description("Create a CData section")> _
    Public Sub XLinq13()
        Dim e = <Dump><![CDATA[<dump>this is some xml</dump>]]>some other text</Dump>
        Console.WriteLine("Element Value: " & e.Value)
        Console.WriteLine("Text nodes collapsed!: " & e.Nodes(0).ToString)
        Console.WriteLine("CData preserved on serialization: " & e.ToString)
    End Sub

    <Category("Construction"), _
    Title("Create a sequence of nodes"), _
    Description("Create a sequence of customer elements")> _
    Public Sub XLinq14()

        Dim cSequence = <?xml version="1.0"?>
                        <root>
                            <customer id="x">new customer</customer>
                            <customer id="y">new customer</customer>
                            <customer id="z">new customer</customer>
                        </root>

        For Each c In From cust In cSequence.<root>.<customer>
            Console.WriteLine(c)
        Next
    End Sub


    <Category("Write"), _
    Title("Write an XElement to XmlWriter"), _
    Description("Write an XElement to XmlWriter using the WriteTo method")> _
    Public Sub XLinq15()
        Dim po1 = <PurchaseOrder>
                      <Item price="100">Motor</Item>
                  </PurchaseOrder>

        Dim po2 = <PurchaseOrder>
                      <Item price="10">Cable</Item>
                  </PurchaseOrder>

        Dim po3 = <PurchaseOrder>
                      <Item price="10">Switch</Item>
                  </PurchaseOrder>

        Dim sw = New StringWriter()
        Dim settings = New XmlWriterSettings()
        settings.Indent = True
        Dim writer = XmlWriter.Create(sw, settings)
        writer.WriteStartElement("PurchaseOrders")

        po1.WriteTo(writer)
        po2.WriteTo(writer)
        po3.WriteTo(writer)

        writer.WriteEndElement()
        writer.Close()
        Console.WriteLine(sw.ToString())
    End Sub


    <Category("Write"), _
    Title("Write the contents of a variable to an XmlWriter"), _
    Description("Write the contents of a variable to an XmlWriter using the WriteTo method")> _
    Public Sub XLinq16()
        Dim doc1 = _
            <PurchaseOrders>
                <PurchaseOrder>
                    <Item price="100">Motor</Item>
                </PurchaseOrder>
                <PurchaseOrder>
                    <Item price="10">Cable</Item>
                </PurchaseOrder>
            </PurchaseOrders>

        Dim doc2 = _
            <PurchaseOrders>
                <PurchaseOrder>
                    <Item price="10">Switch</Item>
                </PurchaseOrder>
            </PurchaseOrders>

        Dim sw = New StringWriter()
        Dim writer = XmlWriter.Create(sw)
        With writer
            .WriteStartDocument()
            .WriteStartElement("AllPurchaseOrders")

            doc1.WriteTo(writer)
            doc2.WriteTo(writer)

            .WriteEndElement()
            .WriteEndDocument()
            .Close()
        End With

        Console.WriteLine(sw.ToString())
    End Sub

    <Category("Write"), _
    Title("Save XDocument"), _
    Description("SaveDim using XmlWriter/TextWriter/File")> _
    Public Sub XLinq17()
        Dim doc = _
            <PurchaseOrders>
                <PurchaseOrder>
                    <Item price="100">Motor</Item>
                </PurchaseOrder>
                <PurchaseOrder>
                    <Item price="10">Switch</Item>
                </PurchaseOrder>
                <PurchaseOrder>
                    <Item price="10">Cable</Item>
                </PurchaseOrder>
            </PurchaseOrders>

        Dim sw = New StringWriter()
        'save to XmlWriter
        Dim settings = New XmlWriterSettings()
        settings.Indent = True

        Dim writer = XmlWriter.Create(sw, settings)
        doc.Save(writer)
        writer.Close()

        Console.WriteLine(sw.ToString())

        'save to file
        doc.Save("out.xml")
    End Sub

    <Category("Query"), _
    Title("Query for child elements"), _
    Description("Select all the customers in the xml document")> _
    Public Sub XLinq18()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        For Each result In doc.<Root>.<Customers>
            Console.WriteLine(result.ToString)
        Next
    End Sub


    <Category("Query"), _
    Title("Query for all child elements"), _
    Description("Select all the child elements of the first customer")> _
    Public Sub XLinq19()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim custQuery = doc.<Root>.<Customers>(0).Elements()
        For Each result In custQuery
            Console.WriteLine(result)
        Next
    End Sub

    <Category("Query"), _
    Title("Query for first child element - 1"), _
    Description("Select the first customer in the document")> _
    Public Sub XLinq20()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim result = doc.<Root>.<Customers>(0)
        Console.WriteLine(result)
    End Sub

    <Category("Query"), _
    Title("Query for first child element - 2"), _
    Description("Query for one child element on a sequence of elements")> _
    Public Sub XLinq21()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim result = doc.Elements().<Customers>.<CompanyName>(0)
        Console.WriteLine(result)
    End Sub


    <Category("Query"), _
    Title("Query for attributes"), _
    Description("Selects all the CompanyNames in the xml document")> _
    Public Sub XLinq22()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim query = From cust In doc.<Root>.<Customers> _
                    Select cust.@CustomerID

        For Each result In query
            Console.WriteLine(result)
        Next
    End Sub

    <Category("Query"), _
    Title("Cast an attribute to a number"), _
    Description("Find orders with price > 100")> _
    Public Sub XLinq23()
        Dim myOrder = _
            <order>
                <item price="150">Motor</item>
                <item price="50">Cable</item>
                <item price="50">Modem</item>
                <item price="250">Monitor</item>
                <item price="10">Mouse</item>
            </order>

        Dim query = From i In myOrder.<item> _
                    Where CInt(i.@price) > 100 _
                    Select i

        For Each result In query
            Console.WriteLine("Expensive Item " & result.Value & " costs " & result.@price)
        Next
    End Sub

    <Category("Query"), _
     Title("Get the root element of a document"), _
     Description("Get the root element of a document")> _
    Public Sub XLinq24()
        Dim root = XDocument.Load(dataPath & "config.xml").Root
        Console.WriteLine("Name of root element is " & root.Name.ToString)
    End Sub


    <Category("Query"), _
    Title("Filter query results using where"), _
    Description("Filter query results using where")> _
    Public Sub XLinq25()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim query = From cust In doc.<Root>.<Customers> _
                    Where cust.<FullAddress>.<Country>.Value = "Germany"

        For Each result In query
            Console.WriteLine(result)
        Next
    End Sub

    <Category("Query"), _
    Title("Select all descendants of an element"), _
    Description("Select all ContactName elements in the document")> _
    Public Sub XLinq26()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        For Each result In doc...<ContactName>
            Console.WriteLine(result)
        Next
    End Sub

    <Category("Query"), _
    Title("Select all descendants of a given type"), _
    Description("Select all text in the document")> _
    Public Sub XLinq27()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")

        Dim query = doc.DescendantNodes().OfType(Of XText)()
        For Each result In query
            Console.WriteLine(result.Value)
        Next
    End Sub

    <Category("Query"), _
    Title("Select all ancestors"), _
    Description("Check if two nodes belong to the same document")> _
    Public Sub XLinq28()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim element1 = doc.<Root>
        Dim element2 = doc...<Customers>(3)
        doc.Ancestors()
        Dim query = From a In element1.Ancestors(), b In element2.Ancestors() _
                    Where a Is b _
                    Select a
        Console.WriteLine(query.Any())
    End Sub



    <Category("Query"), _
    Title("Query for parent"), _
    Description("Query for parent of an Element")> _
    Public Sub XLinq29()
        Dim item = <item-01>Computer</item-01>
        Dim myOrder = <order><%= item %></order>
        Dim p = item.Parent
        Console.WriteLine(p.Name)
    End Sub


    <Category("Query"), _
            Title("Join over two sequences"), _
            Description("Add customer company info to orders of the first customer")> _
        Public Sub XLinq30()
        Dim customers = XDocument.Load(dataPath & "nw_customers.xml")
        Dim orders = XDocument.Load(dataPath & "nw_orders.xml")

        Dim orderQuery = From customer In customers...<Customers>, _
                              order In orders...<Orders> _
                         Where customer.@CustomerID = order.<CustomerID>.Value _
                         Select <Order>
                                    <%= order.Nodes %>
                                    <%= customer.<CompanyName> %>
                                </Order>

        For Each result In orderQuery
            Console.WriteLine(result)
        Next
    End Sub

    <Category("Query"), _
    Title("Query content of a type"), _
    Description("Query content of a given type of an existing element")> _
    Public Sub XLinq31()
        Dim elem = _
            <customer id="abc">
                <name>jack</name>some text<!--new customer-->
            </customer>

        'string content               
        For Each s In elem.Nodes().OfType(Of XText)()
            Console.WriteLine("String content: " & s.Value)
        Next

        'element content
        For Each e In elem.Nodes().OfType(Of XElement)()
            Console.WriteLine("Element content: " & e.Value)
        Next

        'comment content
        For Each c In elem.Nodes().OfType(Of XComment)()
            Console.WriteLine("Comment content: " & c.Value)
        Next
    End Sub

    Sub AddNewOrder(ByVal orders As XDocument)
        Dim newOrder = <Orders>
                           <CustomerID>BERGS</CustomerID>
                           <ShipInfo ShippedDate='1997-12-09T00:00:00'>
                               <Freight>301</Freight>
                               <ShipCountry>Sweden</ShipCountry>
                           </ShipInfo>
                       </Orders>

        orders.Add(newOrder)
    End Sub

    Function GetSwedishFreightProfile(ByVal orders As XDocument) As IEnumerable(Of XElement)
        Return _
            From order In orders.Descendants("Orders") _
            Where order.<ShipInfo>.<ShipCountry>.Value = "Sweden" _
                  AndAlso CInt(order.<ShipInfo>.<Freight>.Value) > 250 _
            Select <Order Freight=<%= order.<ShipInfo>.<Freight>.Value %>/>
    End Function

    Function GetSwedishCustomerOrders(ByVal customers As XDocument, ByVal orders As XDocument) As IEnumerable(Of XElement)
        Return _
            From customer In customers...<Customers> _
            Where customer.<FullAddress>.<Country>.Value = "Sweden" _
            Select _
                <Customer Name=<%= customer.<CompanyName>.Value %> OrderCount=<%= _
                                                                                  (From order In orders...<Orders> _
                                                                                  Where order.<CustomerID>.Value = customer.@CustomerID _
                                                                                  Select order).Count() %>>
                </Customer>
    End Function

    <Category("Query")> _
    <Title("Query using XElement")> _
    <Description("Query for all Swedish customer orders and Swedish orders whose freight is > 250")> _
    <LinkedMethod("GetSwedishFreightProfile")> _
    <LinkedMethod("GetSwedishCustomerOrders")> _
    <LinkedMethod("AddNewOrder")> _
    Public Sub XLinq32()

        Dim customers = XDocument.Load(dataPath & "nw_customers.xml")
        Dim orders = XDocument.Load(dataPath & "nw_orders.xml")
        Dim summary = <Summary Country="Sweden">
                          <SwedishCustomerOrders><%= GetSwedishCustomerOrders(customers, orders) %></SwedishCustomerOrders>
                          <Orders><%= GetSwedishFreightProfile(orders) %></Orders>
                      </Summary>

        Console.WriteLine(summary)
    End Sub



    <Category("Query"), _
    Title("Positional predicate"), _
    Description("Query the 3rd customer in the document")> _
    Public Sub XLinq33()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim cust = doc...<Customers>(2)
        Console.WriteLine(cust)
    End Sub


    <Category("Query")> _
    <Title("Union two sequences of nodes")> _
    <Description("Union books authored by two authors: Serge and Peter")> _
    Public Sub XLinq34()

        Dim doc = XDocument.Load(dataPath & "bib.xml")
        Dim b1 = doc...<book>.Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = "Serge"))
        Dim b2 = doc...<book>.Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = "Peter"))

        Dim books = b1.Union(b2)
        For Each b In books
            Console.WriteLine(b)
        Next
    End Sub

    <Category("Query")> _
    <Title("Intersect two sequences of nodes")> _
    <Description("Intersect books that are common for both authors")> _
    Public Sub XLinq35()
        Dim doc = XDocument.Load(dataPath & "bib.xml")
        Dim b1 = doc...<book>.Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = "Serge"))
        Dim b2 = doc...<book>.Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = "Peter"))

        Dim books = b1.Intersect(b2)
        For Each b In books
            Console.WriteLine(b)
        Next
    End Sub


    <Category("Query")> _
    <Title("All nodes in sequence 1 except the nodes in sequence 2")> _
    <Description("Find books that are authored by Peter and did not have Serge as co-author")> _
    Public Sub XLinq36()
        Dim doc = XDocument.Load(dataPath & "bib.xml")
        Dim b1 = doc...<book>.Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = "Serge"))
        Dim b2 = doc...<book>.Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = "Peter"))

        Dim books = b2.Except(b1)
        For Each b In books
            Console.WriteLine(b)
        Next
    End Sub


    <Category("Query"), _
    Title("Reverse the order of nodes in a sequence"), _
    Description("Display the path to a node"), _
    LinkedMethod("PrintPath")> _
    Public Sub XLinq37()
        Dim doc = XDocument.Load(dataPath & "bib.xml")
        Dim e = doc...<last>(0)
        PrintPath(e)
    End Sub

    Public Sub PrintPath(ByRef e As XElement)
        Dim nodes = e.Ancestors().Reverse()
        For Each node In nodes
            If (node Is e) Then
                Console.Write(node.Name)
            Else
                Console.Write(node.Name.ToString() & "->")
            End If
        Next
    End Sub

    <Category("Query")> _
    <Title("Equality of sequences")> _
    <Description("Check if 2 sequences of nodes are equal. " & _
             "Did Serge and peter co-author all of their the books?")> _
    Public Sub XLinq38()
        Dim doc = XDocument.Load(dataPath & "bib.xml")
        Dim b1 = doc.Descendants("book").Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = "Serge"))
        Dim b2 = doc.Descendants("book").Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = "Peter"))

        Dim result = b2.SequenceEqual(b1)
        Console.WriteLine(result)
    End Sub



    '<Category("Query")> _
    '<Title("TakeWhile operator")> _
    '<Description("List books until total price is less that $150")> _
    'Public Sub XLinq39()
    '    Dim doc = XDocument.Load(dataPath & "bib.xml")
    '    Dim sum = 0.0#
    '    dim query = doc...<book>.TakeWhile(function(c) (sum += cdbl(c.Element("price")) <= 150)

    '    For Each result In query
    '        Console.WriteLine(result)
    '    Next

    'End Sub



    <Category("Query"), _
     Title("Create a list of numbers"), _
     Description("Create 5 new customers with different IDs")> _
    Public Sub XLinq40()
        Dim newCustomers = From i In Range(1, 5) _
                    Select <Customer id=<%= i %>>New customer</Customer>

        For Each result In newCustomers
            Console.WriteLine(result)
        Next
    End Sub

    <Category("Query"), _
     Title("Repeat operator"), _
     Description("Initialize new orders with items")> _
    Public Sub XLinq41()
        Dim orders As XElement() = { _
            <order itemCount="5"/>, _
            <order itemCount="2"/>, _
            <order itemCount="3"/>}

        'add empty items
        For Each myOrder In orders
            myOrder.Add(Repeat(<item>New item</item>, CInt(myOrder.@itemCount)))
        Next

        For Each ord In orders
            Console.WriteLine(ord)
        Next

    End Sub

    <Category("Query")> _
    <Title("Any operator")> _
    <Description("Check if there are any customers in Argentina")> _
    Public Sub XLinq42()
        Dim doc = XDocument.Load(dataPath & "nw_Customers.xml")

        If doc...<Country>.Any(Function(c) CStr(c) = "Argentina") Then
            Console.WriteLine("There are customers in Argentina")
        Else
            Console.WriteLine("There are no customers in Argentina")
        End If
    End Sub

    <Category("Query")> _
    <Title("All operator")> _
    <Description("Check if all books have at least one author")> _
    Public Sub XLinq43()
        Dim doc = XDocument.Load(dataPath & "bib.xml")
        Dim bookQuery = Aggregate b In doc...<book> _
                        Into All(b...<author>.Count > 0)

        If bookQuery Then
            Console.WriteLine("All books have authors")
        Else
            Console.WriteLine("Some books dont have authors")
        End If
    End Sub

    <Category("Query")> _
    <Title("Count operartor")> _
    <Description("Find the number of orders for a customer")> _
    Public Sub XLinq44()
        Dim doc = XDocument.Load(dataPath & "nw_Orders.xml")
        Dim query = doc...<Orders>.Where(Function(o) o.<CustomerID>.Value = "VINET")
        Console.WriteLine("Customer has " & query.Count() & " orders")
    End Sub

    <Category("Query")> _
    <Title("Aggregate operator")> _
    <Description("Find tax on an order")> _
    <LinkedMethod("Tax")> _
    Public Sub XLinq45()
        Dim xml = "<order >" & _
                      "<item price='150'>Motor</item>" & _
                      "<item price='50'>Cable</item>" & _
                  "</order>"

        Dim order = XElement.Parse(xml)
        Dim taxTotal = order.Elements("item").Aggregate(0.0#, AddressOf Tax)

        Console.WriteLine("The total tax on the order @10% is $" & taxTotal)

    End Sub

    Shared Function Tax(ByVal seed As Double, ByVal item As XElement) As Double
        Return seed + CDbl(item.@price) * 0.1
    End Function

    <Category("Query")> _
    <Title("Distinct operator")> _
    <Description("Find all the countries where there is a customer")> _
    Public Sub XLinq46()
        Dim doc = XDocument.Load(dataPath & "nw_Customers.xml")
        Dim countries = From cust In doc...<Country> _
                        Select Name = cust.Value _
                        Order By Name _
                        Distinct

        For Each country In countries
            Console.WriteLine(country)
        Next
    End Sub

    <Category("Query")> _
    <Title("Concat operator")> _
    <Description("List all books by Serge and Peter with co-authored books repeated")> _
    Public Sub XLinq47()
        Dim doc = XDocument.Load(dataPath & "bib.xml")

        Dim b1 = doc...<book>.Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = "Serge"))
        Dim b2 = doc...<book>.Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = "Peter"))

        Dim books = b1.Concat(b2)
        For Each b In books
            Console.WriteLine(b)
        Next
    End Sub


    <Category("Query"), _
    Title("Take operator"), _
    Description("Query the first two customers")> _
    Public Sub XLinq48()
        Dim doc = XDocument.Load(dataPath & "nw_Customers.xml")
        Dim customers = doc...<Customers>.Take(2)
        For Each cust In customers
            Console.WriteLine(cust)
        Next
    End Sub

    <Category("Query"), _
    Title("Skip operator"), _
    Description("Skip the first 3 books")> _
    Public Sub XLinq49()
        Dim doc = XDocument.Load(dataPath & "bib.xml")
        Dim books = doc...<book>.Skip(3)
        For Each book In books
            Console.WriteLine(book)
        Next
    End Sub

    <Category("Query")> _
    <Title("SelectMany operator")> _
    <Description("Get all books authored by Serge and Peter")> _
    <LinkedMethod("GetBooks")> _
    Public Sub XLinq51()

        Dim authors = New String() {"Serge", "Peter"}
        Dim books = authors.SelectMany(Function(author) GetBooks(author))

        For Each book In books
            Console.WriteLine(book)
        Next
    End Sub


    Shared Function GetBooks(ByVal author As String) As IEnumerable(Of XElement)
        Dim doc = XDocument.Load(dataPath & "bib.xml")
        Dim query = doc.Descendants("book").Where(Function(b) b.<author>.<first>.Any(Function(f) CStr(f) = author))

        Return query
    End Function

    <Category("Query")> _
    <Title("Container document")> _
    <Description("Find the container document of an element")> _
    Public Sub XLinq52()
        Dim c = XDocument.Load(dataPath & "bib.xml")...<book>.First()
        Dim container = c.Document
        Console.WriteLine(container)
    End Sub


    <Category("Grouping")> _
    <Title("Group orders by customer")> _
    <Description("Group orders by customer")> _
    Public Sub XLinq53()
        Dim doc = XDocument.Load(dataPath & "nw_orders.xml")
        Dim query = From ord In doc...<Orders> _
                    Group ord By Key = CStr(ord.Element("CustomerID")) _
                    Into OrderGroup = Group _
                    Select OrderGroup

        For Each result In query.SelectMany(Function(group) group)
            Console.WriteLine(result)
        Next
    End Sub


    <Category("Grouping")> _
    <Title("Group customers by country and city")> _
    <Description("Create a directory of customers grouped by country and city")> _
    Public Sub XLinq54()
        Dim customers = XDocument.Load(dataPath & "nw_customers.xml")
        Dim dir = <directory>
                      <%= From customer In customers...<Customers> _
                          From country In customer.<FullAddress>.<Country> _
                          Group customer By Key = CStr(country) Into countryGroup = Group _
                          Let country = Key _
                          Select <Country name=<%= country %> numberOfCustomers=<%= countryGroup.Count %>>
                                     <%= From customer In countryGroup _
                                         From city In customer...<City> _
                                         Group customer By CityKey = CStr(city) Into cityGroup = Group _
                                         Let city = CityKey _
                                         Select <City name=<%= city %> numberOfCustomers=<%= cityGroup.Count() %>>
                                                    <%= cityGroup.<ContactName> %>
                                                </City> %>
                                 </Country> %>
                  </directory>
        Console.WriteLine(dir)

    End Sub

    <Category("Grouping")> _
    <Title("Group orders by customer")> _
    <Description("Group orders by customer and return all customers (+ orders) for customers who have more than 15 orders ")> _
    Public Sub XLinq55()
        Dim customers = XDocument.Load(dataPath & "nw_customers.xml")
        Dim orders = XDocument.Load(dataPath & "nw_orders.xml")

        Dim custOrder = <CustomerOrders>
                            <%= From order In orders...<Orders> _
                                Group By Key = order.<CustomerID> _
                                Into cust_orders = Group _
                                Where cust_orders.Count() > 15 _
                                Select <Customer CustomerID=<%= Key.Value %>>
                                           <%= From customer In customers...<Customers> _
                                               Where CStr(customer.@CustomerID) = Key.Value _
                                               Select customer.Nodes() %>
                                           <%= cust_orders %>
                                       </Customer> %>
                        </CustomerOrders>

        Console.WriteLine(custOrder)
    End Sub

    <Category("Sort")> _
    <Title("Sort customers by name")> _
    <Description("Sort customers by name in ascending order")> _
    Public Sub XLinq56()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim query = From customer In doc...<Customers> _
                    Order By CStr(customer.<ContactName>.First()) _
                    Select customer.<ContactName>

        Dim result = <SortedCustomers><%= query %></SortedCustomers>
        Console.WriteLine(result)
    End Sub

    <Category("Sort")> _
    <Title("Sort orders by date")> _
    <Description("Sort orders by date in ascending order")> _
    Public Sub XLinq57()
        Dim doc = XDocument.Load(dataPath & "nw_orders.xml")

        Dim orderQuery = From order In doc...<Orders> _
                         Let orderDate = CDate(order.<OrderDate>.Value) _
                         Order By orderDate _
                         Select <Order date=<%= orderDate %> custid=<%= order.<CustomerID> %>></Order>

        Dim result = New XElement("SortedOrders", orderQuery)
        Console.WriteLine(result)

    End Sub

    <Category("Sort")> _
    <Title("Descending order")> _
    <Description("Sort customers by name in descending order")> _
    Public Sub XLinq58()
        Dim doc = XDocument.Load(dataPath & "nw_Customers.xml")

        Dim custQuery = From customer In doc...<Customers> _
                        Order By CStr(customer.<ContactName>.First()) Descending _
                        Select customer.<ContactName>.Value

        For Each result In custQuery
            Console.WriteLine(result)
        Next
    End Sub

    <Category("Sort")> _
    <Title("Multiple sort keys")> _
    <Description("Sort customers by country and city")> _
    Public Sub XLinq59()
        Dim doc = XDocument.Load(dataPath & "nw_Customers.xml")
        Dim query = From customer In doc...<Customers> _
                    Order By CStr(customer...<Country>.First()), _
                             CStr(customer...<City>.First()) _
                    Select customer

        For Each result In query
            Console.WriteLine(result)
        Next
    End Sub


    <Category("DML"), _
     Title("Add an element as the last child"), _
     Description("Add an element as the last child")> _
    Public Sub XLinq60()
        Dim doc = XDocument.Load(dataPath & "config.xml")

        Dim config = doc.<config>(0)
        config.Add(<logFolder>c:\log</logFolder>)

        Console.WriteLine(config)
    End Sub

    <Category("DML"), _
     Title("Add an element as the first child"), _
     Description("Add an element as the first child")> _
    Public Sub XLinq61()
        Dim doc = XDocument.Load(dataPath & "config.xml")

        Dim config = doc.<config>(0)
        config.AddFirst(<logFolder>c:\log</logFolder>)

        Console.WriteLine(config)
    End Sub

    <Category("DML"), _
     Title("Add multiple elements as children"), _
     Description("Add multiple elements as children")> _
    Public Sub XLinq62()
        Dim doc = XDocument.Load(dataPath & "config.xml")
        Dim first = <temp>
                        <logFolder>c:\log</logFolder>
                        <resultsFolders>c:\results</resultsFolders>
                    </temp>

        Dim last = <temp>
                       <mode>client</mode>
                       <commPort>2</commPort>
                   </temp>

        Dim config = doc.<config>(0)
        config.AddFirst(first.Nodes())
        config.Add(last.Nodes())
        Console.WriteLine(config)
    End Sub

    <Category("DML"), _
     Title("Add an attribute to an element"), _
     Description("Add an attribute to an element")> _
    Public Sub XLinq63()
        Dim elem = <customer id="abc">this is an XElement</customer>
        Console.WriteLine("Original element " & elem.ToString)

        elem.Add(New XAttribute("name", "Jack"))
        Console.WriteLine("Updated element " & elem.ToString)
    End Sub

    <Category("DML"), _
    Title("Add content to an existing element"), _
    Description("Add attributes and elements")> _
    Public Sub XLinq64()
        Dim elem = <customer id="abc">this is an XElement</customer>
        Console.WriteLine("Original element " & elem.ToString)

        Dim additionalContent = <temp>
                                    <phone>555-555-5555</phone>
                                    <!--new customer-->
                                    <%= New XAttribute("name", "Jack") %>
                                </temp>

        elem.Add(additionalContent.Nodes())
        Console.WriteLine("Updated element " & elem.ToString)
    End Sub


    <Category("DML"), _
    Title("Replace content of a container (element or document"), _
    Description("Replace content to an existing element")> _
    Public Sub XLinq65()
        Dim elem = <customer id="abc">this is an XElement</customer>
        Console.WriteLine("Original element " & elem.ToString)

        elem.SetElementValue("id", "this is a customer element")
        Console.WriteLine("Updated element " & elem.ToString)

        Dim newContent = <temp>this is a customer element<phone>555-555-5555</phone>
                             <!-- new customer -->
                             <%= New XAttribute("name", "Jack") %>
                         </temp>

        elem.SetValue(newContent.Nodes())
        Console.WriteLine("Updated element " & elem.ToString)
    End Sub

    <Category("DML"), _
    Title("Remove content of an element"), _
    Description("Remove content of an element")> _
    Public Sub XLinq66()
        Dim elem = <customer id="abc">this is an XElement</customer>
        Console.WriteLine("Original element " & elem.ToString)
        elem.RemoveAll()
        Console.WriteLine("Updated element " & elem.ToString)
    End Sub

    <Category("DML"), _
    Title("Remove all content"), _
    Description("Remove all content and attributes of an element")> _
    Public Sub XLinq67()
        Dim elem = <customer id="abc">
                       <name>jack</name>this is an XElement<!--new customer-->
                   </customer>

        Console.WriteLine("Original element " & elem.ToString)
        elem.RemoveAll()
        Console.WriteLine("Stripped element " & elem.ToString)
    End Sub

    <Category("DML"), _
    Title("Remove all attributes"), _
    Description("Remove all attributes of an element")> _
    Public Sub XLinq68()
        Dim elem = <customer name="jack" id="abc">this is an XElement<!--new customer-->
                   </customer>

        Console.WriteLine("Original element " & elem.ToString)
        elem.RemoveAttributes()
        Console.WriteLine("Stripped element " & elem.ToString)
    End Sub

    <Category("DML"), _
    Title("Remove an attribute of an element"), _
    Description("Remove an attribute of an element")> _
    Public Sub XLinq69()
        Dim elem = <customer name="jack" id="abc">this is an XElement<!--new customer-->
                   </customer>

        Console.WriteLine("Original element " & elem.ToString)
        elem.@id = Nothing
        Console.WriteLine("Updated element " & elem.ToString)
    End Sub


    <Category("DML"), _
    Title("Update an attribute"), _
    Description("Update the value of an attribute")> _
    Public Sub XLinq70()
        Dim elem = <customer name="jack" id="abc">this is an XElement<!--new customer-->
                   </customer>

        Console.WriteLine("Original element " & elem.ToString)
        elem.@name = "David"
        Console.WriteLine("Updated attribute " & elem.ToString)

    End Sub

    <Category("DML"), _
    Title("Delete an element by name"), _
    Description("Remove a child element by name")> _
    Public Sub XLinq71()
        Dim elem = <customer id="abc">
                       <name>jack</name>this is an XElement<!--new customer-->
                   </customer>

        Console.WriteLine("Original element " & elem.ToString)
        elem.SetElementValue("name", Nothing)
        Console.WriteLine("Updated element " & elem.ToString)
    End Sub

    <Category("DML"), _
    Title("Update a child element by name"), _
    Description("Update a child element by name")> _
    Public Sub XLinq72()
        Dim elem = <customer id="abc">
                       <name>jack</name>this is an XElement<!--new customer-->
                   </customer>

        Console.WriteLine("Original element " & elem.ToString)
        elem.<name>.Value = "David"
        Console.WriteLine("Updated element " & elem.ToString)
    End Sub

    <Category("DML"), _
    Title("Remove a list of elements"), _
    Description("Remove a list of elements")> _
    Public Sub XLinq73()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim elems = doc.<Root>.<Customers>
        Console.WriteLine("Before count " & elems.Count())

        elems.Take(15).Remove()
        Console.WriteLine("After count " & elems.Count())
    End Sub

    <Category("DML"), _
    Title("Remove a list of attributes"), _
    Description("Remove a list of attributes")> _
    Public Sub XLinq74()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim attrs = doc...<Customers>.Attributes()
        Console.WriteLine("Before count " & attrs.Count())

        attrs.Take(15).Remove()
        Console.WriteLine("After count " & attrs.Count())
    End Sub

    <Category("DML"), _
    Title("Add an un-parented element to an element"), _
    Description("Add an un-parented element to an element")> _
    Public Sub XLinq75()
        Dim e = <foo>this is an element</foo>
        If e.Parent Is Nothing Then
            Console.WriteLine("Parent : Nothing")
        Else
            Console.WriteLine("Parent : " & e.Parent.Value)
        End If
        Dim p = <bar><%= e %></bar> 'add to document        
        If e.Parent Is Nothing Then
            Console.Write("Parent : Nothing")
        Else
            Console.WriteLine("Parent : " & e.Parent.Name.ToString)
        End If
    End Sub

    <Category("DML"), _
    Title("Add a parented element to a document"), _
    Description("Adding a parented element to another container clones it")> _
    Public Sub XLinq76()
        Dim elem = <foo>this is an element</foo>

        Dim p1 = <p1><%= elem %></p1>
        Console.WriteLine("Parent : " & elem.Parent.Name.ToString())

        Dim p2 = <p2><%= elem %></p2>
        Console.WriteLine("Parent : " & elem.Parent.Name.ToString())
    End Sub

    <Category("Transform"), _
    Title("Create a table of customers"), _
    Description("Generate html with a list of customers that is numbered")> _
    Public Sub XLinq77()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")

        Dim header() As XElement = {<th>#</th>, _
                                  <th>customer id</th>, _
                                  <th>contact name</th>}
        Dim rows = _
            From customer In doc...<Customers> _
            Select <tr><td><%= Increment() %></td><td><%= customer.@CustomerID %></td><td><%= customer.<ContactName>(0).Value %></td></tr>

        Dim html = <html><body><table><%= header %><%= rows %></table></body></html>
        Console.Write(html)
    End Sub

    Public Shared index As Integer = 0
    Public Shared Function Increment() As Integer
        index += 1
        Return index
    End Function


    <Category("Transform")> _
    <Title("Create html tables of books")> _
    <Description("Generate a html tables of books by authors")> _
    <LinkedMethod("GetBooksTable")> _
    Public Sub XLinq78()
        Dim doc = XDocument.Load(dataPath & "bib.xml")
        Dim content = From book In doc...<book> _
                      From author In book.<author> _
                      Group book By Key = author.<first>.Value & " " & author.<last>.Value _
                      Into authorGroup = Group _
                      Select <p><%= "Author: " & Key %><%= GetBooksTable(authorGroup) %></p>

        Dim result = <html><body><%= content %></body></html>

        Console.WriteLine(result)
    End Sub

    Shared Function GetBooksTable(ByVal books As IEnumerable(Of XElement)) As XElement

        Dim header = New XElement() {<th>Title</th>, <th>Year</th>}
        Dim rows = From book In books _
                   Select <tr><td><%= book.@title %></td><td><%= book.@year %></td></tr>

        Return <table><%= header %><%= rows %></table>

    End Function

    <Category("Language Integration")> _
    <Title("Find all orders for customers in a List")> _
    <Description("Find all orders for customers in a List")> _
    <LinkedMethod("SomeMethodToGetCustomers")> _
    <LinkedClass("Customer")> _
    Public Sub XLinq79()
        Dim doc = XDocument.Load(dataPath & "nw_orders.xml")
        Dim customers = SomeMethodToGetCustomers()
        Dim result = From customer In customers _
                     From order In doc...<Orders> _
                     Where customer.id = CStr(order.<CustomerID>.Value) _
                     Select CustID = CStr(customer.id), OrderDate = order.<OrderDate>.Value

        For Each tuple In result
            Console.WriteLine("Customer ID = " & tuple.CustID & ", Order Date = " & tuple.OrderDate)
        Next
    End Sub

    Class Customer
        Public Sub New(ByVal id As String)
            Me.id = id
        End Sub

        Public id As String
    End Class

    Shared Function SomeMethodToGetCustomers() As List(Of Customer)
        Dim customers As New List(Of Customer)
        customers.Add(New Customer("VINET"))
        customers.Add(New Customer("TOMSP"))
        Return customers
    End Function

    <Category("Language Integration")> _
    <Title("Find sum of items in a shopping cart")> _
    <Description("Find sum of items in a shopping cart")> _
    <LinkedMethod("GetShoppingCart")> _
    <LinkedClass("Item")> _
    Public Sub XLinq80()
        Dim doc = XDocument.Load(dataPath & "inventory.xml")
        Dim cart = GetShoppingCart()
        Dim subtotal = From item In cart _
                       From inventory In doc...<item> _
                       Where item.id = CStr(inventory.@id) _
                       Select item.quantity * CDbl(inventory.<price>.Value)

        Console.WriteLine("Total payment = " & subtotal.Sum())
    End Sub

    Shadows Class Item
        Public Sub New(ByVal id As String, ByVal quantity As Integer)
            Me.id = id
            Me.quantity = quantity
        End Sub
        Public quantity As Integer
        Public id As String
    End Class

    Shared Function GetShoppingCart() As List(Of Item)
        Dim items As New List(Of Item)
        items.Add(New Item("1", 10))
        items.Add(New Item("5", 5))
        Return items
    End Function


    <Category("Language Integration"), _
    Title("Consume a config file"), _
    Description("Load and use a config file"), _
    LinkedMethod("Initialize")> _
    Public Sub XLinq81()
        Dim config = XDocument.Load(dataPath & "config.xml").<config>
        Initialize(config.<rootFolder>.Value, _
                   CInt(config.<iterations>.Value), _
                   CDbl(config.<maxMemory>.Value), _
                   config.<tempFolder>.Value)
    End Sub
    Public Sub Initialize(ByVal root As String, ByVal iter As Integer, ByVal mem As Double, ByVal temp As String)
        Console.WriteLine("Application initialized to root folder: " & _
                          "{0}, iterations: {1}, max memory {2}, temp folder: {3}", _
                          root, iter, mem, temp)
    End Sub

    <Category("Language Integration"), _
    Title("Convert a Sequence of nodes to Array"), _
    Description("Convert a Sequence of nodes to Array")> _
    Public Sub XLinq82()
        Dim doc = XDocument.Load(dataPath & "nw_Customers.xml")
        Dim custArray() = doc...<Customers>.ToArray()
        For Each c In custArray
            Console.WriteLine(c)
        Next
    End Sub

    <Category("Language Integration"), _
     Title("Convert a Sequence of nodes to List"), _
     Description("Convert a Sequence of nodes to List")> _
    Public Sub XLinq83()
        Dim doc = XDocument.Load(dataPath & "nw_Customers.xml")
        Dim clist As List(Of XElement) = doc...<Customers>.ToList()
        For Each c In clist
            Console.WriteLine(c)
        Next
    End Sub


    <Category("Language Integration")> _
    <Title("Create a dictionary of customers")> _
    <Description("Create a dictionary of customers")> _
    Public Sub XLinq84()
        Dim doc = XDocument.Load(dataPath & "nw_Customers.xml")
        Dim Dictionary = doc...<Customers>.ToDictionary(Function(c) CStr(c.@CustomerID))
        Console.WriteLine(Dictionary("ALFKI"))
    End Sub


    <Category("Language Integration")> _
    <Title("Using anonymous types ")> _
    <Description("Number all the countries and list them")> _
    Public Sub XLinq85()
        Dim doc = XDocument.Load(dataPath & "nw_Customers.xml")
        Dim countries = doc...<Country>.Select(Function(c) CStr(c)) _
                                       .Distinct() _
                                       .Select(Function(c, index) _
                                                   New With {.i = index, .name = c})
        For Each c In countries
            Console.WriteLine(c.i & " " & c.name)
        Next

    End Sub

    <Category("XName"), _
     Title("Create elements and attributes in a namespace"), _
     Description("Create elements and attributes in a namespace")> _
    Public Sub XLinq86()
        Dim ns As String = "{http://myNamespace}"
        Dim result = <<%= ns & "foo" %> <%= ns & "bar" %>="attribute"/>
        Console.WriteLine(result)
    End Sub

    <Category("XName")> _
    <Title("Query for elements in a namespace")> _
    <Description("Find xsd:element with name=FullAddress")> _
     Public Sub XLinq87()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xsd")
        Dim XSD As XNamespace = "http://www.w3.org/2001/XMLSchema"
        Dim result = doc.Descendants(XSD + "element").Where(Function(e) e.@name = "FullAddress").First()
        Console.WriteLine(result)
    End Sub

    <Category("XName"), _
    Title("Create a namespace prefix declaration"), _
    Description("Create a namespace prefix declaration")> _
    Public Sub XLinq88()
        Dim result = <myElement xmlns:myPrefix="http://myNamespace"/>
        Console.WriteLine(result)
    End Sub

    <Category("XName"), _
    Title("Local-name and namespace"), _
    Description("Get the local-name and namespace of an element")> _
    Public Sub XLinq89()
        Dim e = <ns:foo xmlns:ns="http://myNamespace"/>
        Console.WriteLine("Local name of element: " & e.Name.LocalName)
        Console.WriteLine("Namespace of element : " & e.Name.NamespaceName)
    End Sub

    <Category("Misc"), _
    Title("Get the outer XML of a node"), _
    Description("Get the outer XML of a node")> _
    Public Sub XLinq90()
        Dim myOrder = _
            <order>
                <item price="150">Motor</item>
                <item price="50">Cable</item>
                <item price="50">Modem</item>
                <item price="250">Monitor</item>
                <item price="10">Mouse</item>
            </order>
        Console.WriteLine(myOrder)
    End Sub

    <Category("Misc"), _
    Title("Get the inner text of a node"), _
    Description("Get the inner text of a node")> _
    Public Sub XLinq91()
        Dim myOrder = _
            <order>
                <item price="150">Motor</item>
                <item price="50">Cable</item>
                <item price="50">Modem</item>
                <item price="250">Monitor</item>
                <item price="10">Mouse</item>
            </order>
        Console.WriteLine(myOrder.Value)
    End Sub

    <Category("Misc"), _
    Title("Check if an element has attributes"), _
    Description("Check if an element has attributes")> _
    Public Sub XLinq92()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim e = doc.<Root>.<Customers>(0)
        Console.WriteLine("Customers has attributes? " & e.HasAttributes)
    End Sub

    <Category("Misc"), _
    Title("Check if an element has element children"), _
    Description("Check if an element has element children")> _
    Public Sub XLinq93()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim e = doc.<Root>.<Customers>(0)
        Console.WriteLine("Customers has elements? " & e.HasElements)
    End Sub

    <Category("Misc"), _
    Title("Check if an element is empty"), _
    Description("Check if an element is empty")> _
    Public Sub XLinq94()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim e = doc.<Root>.<Customers>(0)
        Console.WriteLine("Customers element is empty? " & e.IsEmpty)
    End Sub

    <Category("Misc"), _
    Title("Get the name of an element"), _
    Description("Get the name of an element")> _
    Public Sub XLinq95()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim e = doc.Elements()(0)
        Console.WriteLine("Name of element " & e.Name.ToString)
    End Sub

    <Category("Misc"), _
    Title("Get the name of an attribute"), _
    Description("Get the name of an attribute")> _
    Public Sub XLinq96()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim a = doc.<Root>.<Customers>.Attributes()(0)
        Console.WriteLine("Name of attribute " & a.Name.ToString)
    End Sub

    <Category("Misc"), _
    Title("Get the XML declaration"), _
    Description("Get the XML declaration")> _
    Public Sub XLinq97()
        Dim doc = XDocument.Load(dataPath & "config.xml")
        Console.WriteLine("Version " & doc.Declaration.Version)
        Console.WriteLine("Encoding " & doc.Declaration.Encoding)
        Console.WriteLine("Standalone " & doc.Declaration.Standalone)
    End Sub

    <Category("Misc"), _
    Title("Find the type of the node"), _
    Description("Find the type of the node")> _
    Public Sub XLinq98()
        Dim o = <foo/>
        Console.WriteLine(o.NodeType.ToString)
    End Sub

    <Category("Misc"), _
    Title("Verify phone numbers"), _
    Description("Verify that the phone numbers of the format xxx-xxx-xxxx"), _
    LinkedMethod("CheckPhone")> _
    Public Sub XLinq99()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        Dim query1 = _
            From customer In doc...<Customers> _
            Select <customer CustomerID=<%= customer.@CustomerID %>>
                       <%= customer...<Phone>(0) %>
                       <%= CheckPhone(customer...<Phone>.Value) %>
                   </customer>
        For Each result In query1
            Console.WriteLine(result)
        Next
    End Sub

    Public Function CheckPhone(ByVal phone As String) As XElement
        Dim myRegex = New Regex("([0-9]{3}-)|('('[0-9]{3}')')[0-9]{3}-[0-9]{4}")
        CheckPhone = <isValidPhone><%= myRegex.IsMatch(phone) %></isValidPhone>
    End Function

    <Category("Misc"), _
    Title("Quick validation"), _
    Description("Validate file structure"), _
    LinkedMethod("VerifyCustomer")> _
    Public Sub XLinq100()
        Dim doc = XDocument.Load(dataPath & "nw_customers.xml")
        For Each cust In doc...<Customers>
            Dim e = VerifyCustomer(cust)
            If e <> "" Then
                Console.WriteLine("Customer " & cust.@CustomerID & " is invalid. Missing " & e)
            End If
        Next
    End Sub

    Public Function VerifyCustomer(ByVal c As XElement) As String
        If c.<CompanyName>.Count() = 0 Then
            VerifyCustomer = "CompanyName"
        ElseIf c.<ContactName>.Count() = 0 Then
            VerifyCustomer = "ContactName"
        ElseIf c.<ContactTitle>.Count() = 0 Then
            VerifyCustomer = "ContactTitle"
        ElseIf c.<Phone>.Count() = 0 Then
            VerifyCustomer = "Phone"
        ElseIf c.<Fax>.Count() = 0 Then
            VerifyCustomer = "Fax"
        ElseIf c.<FullAddress>.Count() = 0 Then
            VerifyCustomer = "FullAddress"
        Else : VerifyCustomer = ""
        End If
    End Function

    <Category("Misc"), _
    Title("Aggregate functions"), _
    Description("Calculate sum, average, min, max of freight of all orders")> _
    Public Sub XLinq101()
        Dim doc = XDocument.Load(dataPath & "nw_orders.xml")
        Dim result = From myOrder In doc...<Orders> _
            Where myOrder.<CustomerID>(0).Value = "VINET" And myOrder.<ShipInfo>.<Freight>.Any() _
            Select CDbl(myOrder.<ShipInfo>.<Freight>(0))

        Dim sum1 = result.Sum()
        Dim average1 = result.Average()
        Dim min1 = result.Min()
        Dim max1 = result.Max()
        Console.WriteLine("Sum: {0}, Average: {1}, Min: {2}, Max: {3}", sum1, average1, min1, max1)
    End Sub

    <Category("Query"), _
    Title("Fold operator"), _
    Description("Find tax on an order"), _
    LinkedMethod("Tax")> _
    Public Sub XLinq102()
        Dim myOrder = _
            <order>
                <item price="150">Motor</item>
                <item price="50">Cable</item>
            </order>

        Dim tax As Double = myOrder.<item>.Aggregate(0.0, AddressOf CalcTax)
        Console.WriteLine("The total tax on the order @10% is $" & tax)
    End Sub

    Public Function CalcTax(ByVal seed As Double, ByVal item As XElement) As Double
        CalcTax = seed + CDbl(item.@price) * 0.1
    End Function

End Class

