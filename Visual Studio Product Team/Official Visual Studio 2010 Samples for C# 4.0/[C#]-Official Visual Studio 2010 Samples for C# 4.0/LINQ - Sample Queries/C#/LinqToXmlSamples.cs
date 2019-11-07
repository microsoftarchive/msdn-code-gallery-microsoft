// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SampleSupport;
using System.Xml;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace SampleQueries {
    [Title("101+ Linq To Xml Query Samples")]
    [Prefix("XLinq")]
    public class LinqToXmlSamples : SampleHarness {
        public string dataPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\Data\"));

        [Category("Load")]
        [Title("Load document from file")]
        [Description("Load an XML document from a file")]
        public void XLinq1() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            Console.WriteLine(doc);
        }

        [Category("Load")]
        [Title("Load document from string")]
        [Description("Load document from string")]
        public void XLinq2() {
            string xml = "<book price='100' isbn='1002310'>" +
                            "<title>XClarity Samples</title>" +
                            "<author>Matt</author>" +
                         "</book>";
            XDocument doc = XDocument.Parse(xml);
            Console.WriteLine(doc);

        }

        //load an XML document from XmlReader
        [Category("Load")]
        [Title("Load document from XmlReader")]
        [Description("Load an XML document from XmlReader")]
        public void XLinq3() {
            XmlReader reader = XmlReader.Create(dataPath + "bib.xml");
            XDocument doc = XDocument.Load(reader);
            Console.WriteLine(doc);

        }

        [Category("Load")]
        [Title("Element from XmlReader - 1")]
        [Description("Construct XElement from XmlReader positioned on an element")]
        public void XLinq4() {
            XmlReader reader = XmlReader.Create(dataPath + "nw_customers.xml");
            reader.Read();//move to root
            reader.Read(); // move to fist customer
            XElement c = (XElement)XNode.ReadFrom(reader);
            Console.WriteLine(c);

        }

        [Category("Load")]
        [Title("Element from XmlReader - 2")]
        [Description("Read XElement content from XmlReader")]
        public void XLinq5() {
            XmlReader reader = XmlReader.Create(dataPath + "config.xml");
            //the file has comments and whitespace at the start
            reader.Read();
            reader.Read();
            XElement config = new XElement("appSettings",
                                           "This content will be replaced");
            config.RemoveAll();
            while (!reader.EOF)
                config.Add(XNode.ReadFrom(reader));
            Console.WriteLine(config);
        }

        [Category("Construction")]
        [Title("Construct an XElement from string")]
        [Description("Construct an XElement from string")]
        public void XLinq6() {
            string xml = "<purchaseOrder price='100'>" +
                            "<item price='50'>Motor</item>" +
                            "<item price='50'>Cable</item>" +
                          "</purchaseOrder>";
            XElement po = XElement.Parse(xml);
            Console.WriteLine(po);

        }

        [Category("Construction")]
        [Title("Add XML declaration to a document")]
        [Description("Add XML declaration to a document")]
        public void XLinq7() {
            XDocument doc = new XDocument(new XDeclaration("1.0", "UTF-16", "Yes"),
                                          new XElement("foo"));
            StringWriter sw = new StringWriter();
            doc.Save(sw);
            Console.WriteLine(sw);

        }

        [Category("Construction")]
        [Title("Computed element name")]
        [Description("Computed element name")]
        public void XLinq8() {
            XDocument customers = XDocument.Load(dataPath + "nw_customers.xml");
            string name = (string)customers.Elements("Root")
                                           .Elements("Customers")
                                           .First()
                                           .Attribute("CustomerID");
            XElement result = new XElement(name,
                                            "Element with a computed name");
            Console.WriteLine(result);

        }
        [Category("Construction")]
        [Title("Document creation")]
        [Description("Create a simple config file")]
        public void XLinq9() {
            XDocument myDocument =
            new XDocument(
              new XElement("configuration",
                new XElement("system.web",
                  new XElement("membership",
                    new XElement("providers",
                      new XElement("add",
                        new XAttribute("name",
                                       "WebAdminMembershipProvider"),
                        new XAttribute("type",
                                       "System.Web.Administration.WebAdminMembershipProvider")))),
                  new XElement("httpModules",
                    new XElement("add",
                      new XAttribute("name",
                                      "WebAdminModule"),
                      new XAttribute("type",
                                      "System.Web.Administration.WebAdminModule"))),
                  new XElement("authentication",
                    new XAttribute("mode", "Windows")),
                  new XElement("authorization",
                    new XElement("deny",
                      new XAttribute("users", "?"))),
                  new XElement("identity",
                    new XAttribute("impersonate", "true")),
                  new XElement("trust",
                    new XAttribute("level", "full")),
                  new XElement("pages",
                    new XAttribute("validationRequest", "true")))));

            Console.WriteLine(myDocument);

        }

        // <xsd:schema xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
        //             xmlns:sql="urn:schemas-microsoft-com:mapping-schema">

        // <xsd:element name="root" sql:is-constant="1">
        // <xsd:complexType>
        // <xsd:sequence>
        // <xsd:element name="Customers" minOccurs="100" maxOccurs="unbounded">
        //     <xsd:complexType>
        //     <xsd:sequence>
        //         <xsd:element name="CompanyName" type="xsd:string" /> 
        //         <xsd:element name="ContactName" type="xsd:string" /> 
        //         <xsd:element name="ContactTitle" type="xsd:string" /> 
        //         <xsd:element name="Phone" type="xsd:string" /> 
        //         <xsd:element name="Fax" type="xsd:string"/> 
        //         <xsd:element ref="FullAddress" maxOccurs="3"/>
        //         <xsd:element name="Date" type="xsd:date"/>
        //     </xsd:sequence>
        //     <xsd:attribute name="CustomerID" type="xsd:integer" /> 
        //     </xsd:complexType>
        // </xsd:element>
        // </xsd:sequence>
        // </xsd:complexType>
        // </xsd:element>
        // <xsd:element name="FullAddress" sql:relation="Customers" sql:relationship="CustAdd" sql:key-fields="CustomerID" >
        //     <xsd:complexType>
        //     <xsd:sequence>
        //         <xsd:element name="Address" type="xsd:string" /> 
        //         <xsd:element name="City" type="xsd:string" /> 
        //         <xsd:element name="Region" type="xsd:string" /> 
        //         <xsd:element name="PostalCode" type="xsd:string" /> 
        //         <xsd:element name="Country" type="xsd:string" /> 
        //     </xsd:sequence>
        //     </xsd:complexType>
        // </xsd:element>       
        //</xsd:schema>

        [Category("Construction")]
        [Title("Create an XmlSchema")]
        [Description("Create an XmlSchema")]
        public void XLinq10() {
            XNamespace XSD = "http://www.w3.org/2001/XMLSchema";
            XNamespace SQL = "urn:schemas-microsoft-com:mapping-schema";
            XElement result =
                new XElement(XSD + "schema",
                        new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"),
                        new XAttribute(XNamespace.Xmlns + "sql", "urn:schemas-microsoft-com:mapping-schema"),
                        new XElement(XSD + "element",
                            new XAttribute("name", "root"),
                            new XAttribute(SQL + "is-constant", "1"),
                            new XElement(XSD + "complexType",
                                new XElement(XSD + "sequence",
                                    new XElement(XSD + "element",
                                        new XAttribute("name", "Customers"),
                                        new XAttribute("minOccurs", "100"),
                                        new XAttribute("maxOccurs", "unbounded"),
                                        new XElement(XSD + "complexType",
                                            new XElement(XSD + "sequence",
                                                new XElement(XSD + "element",
                                                    new XAttribute("name", "CompanyName"),
                                                    new XAttribute("type", "xsd:string")),
                                                new XElement(XSD + "element",
                                                    new XAttribute("name", "ContactName"),
                                                    new XAttribute("type", "xsd:string")),
                                                new XElement(XSD + "element",
                                                    new XAttribute("name", "ContactTitle"),
                                                    new XAttribute("type", "xsd:string")),
                                                new XElement(XSD + "element",
                                                    new XAttribute("name", "Phone"),
                                                    new XAttribute("type", "xsd:string")),
                                                new XElement(XSD + "element",
                                                    new XAttribute("name", "Fax"),
                                                    new XAttribute("type", "xsd:string")),
                                                new XElement(XSD + "element",
                                                    new XAttribute("ref", "FullAddress"),
                                                    new XAttribute("maxOccurs", "3")),
                                                new XElement(XSD + "element",
                                                    new XAttribute("name", "Date"),
                                                    new XAttribute("type", "xsd:date"))),
                                            new XElement(XSD + "attribute",
                                                    new XAttribute("name", "CustomerID"),
                                                    new XAttribute("type", "xsd:integer"))))))),
                        new XElement(XSD + "element",
                                new XAttribute("name", "FullAddress"),
                                new XAttribute(SQL + "relation", "Customers"),
                                new XAttribute(SQL + "relationship", "CustAdd"),
                                new XAttribute(SQL + "key-fields", "CustomerID"),
                                new XElement(XSD + "complexType",
                                    new XElement(XSD + "sequence",
                                        new XElement(XSD + "element",
                                            new XAttribute("name", "Address"),
                                            new XAttribute("type", "xsd:string")),
                                        new XElement(XSD + "element",
                                            new XAttribute("name", "City"),
                                            new XAttribute("type", "xsd:string")),
                                        new XElement(XSD + "element",
                                            new XAttribute("name", "Region"),
                                            new XAttribute("type", "xsd:string")),
                                        new XElement(XSD + "element",
                                            new XAttribute("name", "PostalCode"),
                                            new XAttribute("type", "xsd:string")),
                                        new XElement(XSD + "element",
                                            new XAttribute("name", "Country"),
                                            new XAttribute("type", "xsd:string"))))));

            Console.WriteLine(result);

        }

        [Category("Construction")]
        [Title("Construct a PI")]
        [Description("Create an XML document with an XSLT PI")]
        public void XLinq11() {
            XDocument result = new XDocument(
                                new XProcessingInstruction("xml-stylesheet",
                                                           "type='text/xsl' href='diff.xsl'"),
                                new XElement("foo"));
            Console.WriteLine(result);
        }

        [Category("Construction")]
        [Title("XML comment construction")]
        [Description("XML comment construction")]
        public void XLinq12() {
            XDocument result =
              new XDocument(
                new XComment("My phone book"),
                new XElement("phoneBook",
                  new XComment("My friends"),
                  new XElement("Contact",
                    new XAttribute("name", "Ralph"),
                    new XElement("homephone", "425-234-4567"),
                    new XElement("cellphone", "206-345-75656")),
                  new XElement("Contact",
                    new XAttribute("name", "Dave"),
                    new XElement("homephone", "516-756-9454"),
                    new XElement("cellphone", "516-762-1546")),
                  new XComment("My family"),
                  new XElement("Contact",
                    new XAttribute("name", "Julia"),
                    new XElement("homephone", "425-578-1053"),
                    new XElement("cellphone", "")),
                  new XComment("My team"),
                  new XElement("Contact",
                    new XAttribute("name", "Robert"),
                    new XElement("homephone", "345-565-1653"),
                    new XElement("cellphone", "321-456-2567"))));

            Console.WriteLine(result);
        }

        [Category("Construction")]
        [Title("Create a CData section")]
        [Description("Create a CData section")]
        public void XLinq13() {
            XElement e = new XElement("Dump",
                          new XCData("<dump>this is some xml</dump>"),
                          new XText("some other text"));
            Console.WriteLine("Element Value: {0}", e.Value);
            Console.WriteLine("Text nodes collapsed!: {0}", e.Nodes().First());
            Console.WriteLine("CData preserved on serialization: {0}", e);
        }

        [Category("Construction")]
        [Title("Create a sequence of nodes")]
        [Description("Create a sequence of customer elements")]
        public void XLinq14() {
            var cSequence = new[] {
                    new XElement("customer",
                                 new XAttribute("id","x"),"new customer"),
                    new XElement("customer",
                                 new XAttribute("id","y"),"new customer"),
                    new XElement("customer",
                                 new XAttribute("id","z"),"new customer")};
            foreach (var c in cSequence)
                Console.WriteLine(c);

        }

        [Category("Write")]
        [Title("Write an XElement to XmlWriter")]
        [Description("Write an XElement to XmlWriter using the WriteTo method")]
        public void XLinq15() {
            XElement po1 = new XElement("PurchaseOrder",
                            new XElement("Item", "Motor",
                              new XAttribute("price", "100")));

            XElement po2 = new XElement("PurchaseOrder",
                            new XElement("Item", "Cable",
                              new XAttribute("price", "10")));

            XElement po3 = new XElement("PurchaseOrder",
                            new XElement("Item", "Switch",
                              new XAttribute("price", "10")));

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter w = XmlWriter.Create(sw, settings);
            w.WriteStartElement("PurchaseOrders");

            po1.WriteTo(w);
            po2.WriteTo(w);
            po3.WriteTo(w);

            w.WriteEndElement();
            w.Close();
            Console.WriteLine(sw.ToString());
        }


        [Category("Write")]
        [Title("Write the content of an XDocument to XmlWriter")]
        [Description("Write the content of XDocument to XmlWriter using the WriteTo method")]
        public void XLinq16() {
            XDocument doc1 = new XDocument(
              new XElement("PurchaseOrders",
                new XElement("PurchaseOrder",
                  new XElement("Item", "Motor",
                    new XAttribute("price", "100"))),
                new XElement("PurchaseOrder",
                  new XElement("Item", "Cable",
                    new XAttribute("price", "10")))));
            XDocument doc2 = new XDocument(
              new XElement("PurchaseOrders",
                new XElement("PurchaseOrder",
                  new XElement("Item", "Switch",
                    new XAttribute("price", "10")))));

            StringWriter sw = new StringWriter();

            XmlWriter w = XmlWriter.Create(sw);

            w.WriteStartDocument();
            w.WriteStartElement("AllPurchaseOrders");
            doc1.Root.WriteTo(w);
            doc2.Root.WriteTo(w);
            w.WriteEndElement();
            w.WriteEndDocument();
            w.Close();
            Console.WriteLine(sw.ToString());
        }


        [Category("Write")]
        [Title("Save XDocument")]
        [Description("Save XDocument using XmlWriter/TextWriter/File")]
        public void XLinq17() {
            XDocument doc = new XDocument(
                new XElement("PurchaseOrders",
                    new XElement("PurchaseOrder",
                      new XElement("Item",
                        "Motor",
                        new XAttribute("price", "100"))),
                    new XElement("PurchaseOrder",
                      new XElement("Item",
                        "Switch",
                        new XAttribute("price", "10"))),
                    new XElement("PurchaseOrder",
                      new XElement("Item",
                        "Cable",
                        new XAttribute("price", "10")))));

            StringWriter sw = new StringWriter();
            //save to XmlWriter
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter w = XmlWriter.Create(sw, settings);
            doc.Save(w);
            w.Close();
            Console.WriteLine(sw.ToString());

            //save to file
            doc.Save("out.xml");

        }

        [Category("Query")]
        [Title("Query for child elements")]
        [Description("Select all the customers in the xml document")]
        public void XLinq18() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            foreach (XElement result in doc.Elements("Root")
                                           .Elements("Customers"))
                Console.WriteLine(result);

        }

        [Category("Query")]
        [Title("Query for all child elements")]
        [Description("Select all the child elements of the first customer")]
        public void XLinq19() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var query = doc.Element("Root")
                           .Element("Customers")
                           .Elements();
            foreach (XElement result in query)
                Console.WriteLine(result);
        }

        [Category("Query")]
        [Title("Query for first child element - 1")]
        [Description("Select the first customer in the document")]
        public void XLinq20() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var result = doc.Element("Root")
                            .Element("Customers");
            Console.WriteLine(result);
        }

        [Category("Query")]
        [Title("Query for first child element - 2")]
        [Description("Query for one child element on a sequence of elements")]
        public void XLinq21() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var result = doc.Elements()
                            .Elements("Customers")
                            .First()
                            .Element("CompanyName");
            Console.WriteLine(result);
        }

        [Category("Query")]
        [Title("Query for attributes")]
        [Description("Selects all the CustomerIDs in the xml document")]
        public void XLinq22() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var query = doc.Element("Root")
                           .Elements("Customers")
                           .Attributes("CustomerID");
            foreach (XAttribute result in query)
                Console.WriteLine(result.Name + " = " + result.Value);

        }

        [Category("Query")]
        [Title("Cast an attribute to a number")]
        [Description("Find orders with price > 100")]
        public void XLinq23() {
            string xml = "<order >" +
                            "<item price='150'>Motor</item>" +
                            "<item price='50'>Cable</item>" +
                            "<item price='50'>Modem</item>" +
                            "<item price='250'>Monitor</item>" +
                            "<item price='10'>Mouse</item>" +
                        "</order>";
            XElement order = XElement.Parse(xml);
            var query =
                from
                    i in order.Elements("item")
                where
                    (int)i.Attribute("price") > 100
                select i;
            foreach (var result in query)
                Console.WriteLine("Expensive Item {0} costs {1}",
                                  (string)result,
                                  (string)result.Attribute("price"));
        }

        [Category("Query")]
        [Title("Get the root element of a document")]
        [Description("Get the root element of a document")]
        public void XLinq24() {
            XElement root = XDocument.Load(dataPath + "config.xml")
                                     .Root;
            Console.WriteLine("Name of root element is {0}", root.Name);
        }

        [Category("Query")]
        [Title("Filter query results using where")]
        [Description("Filter query results using where")]
        public void XLinq25() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var query =
                    from
                        c in doc.Element("Root")
                                .Elements("Customers")
                    where
                        c.Element("FullAddress")
                         .Element("Country")
                         .Value == "Germany"
                    select c;

            foreach (XElement result in query)
                Console.WriteLine(result);
        }

        [Category("Query")]
        [Title("Select all descendants of an element")]
        [Description("Select all ContactName elements in the document")]
        public void XLinq26() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var query = doc.Descendants("ContactName");
            foreach (XElement result in query)
                Console.WriteLine(result);
        }

        [Category("Query")]
        [Title("Select all descendants of a given type")]
        [Description("Select all text in the document")]
        public void XLinq27() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var query = doc.DescendantNodes().OfType<XText>().Select(t => t.Value);
            foreach (string result in query)
                Console.WriteLine(result);
        }

        [Category("Query")]
        [Title("Select all ancestors")]
        [Description("Check if two nodes belong to the same document")]
        public void XLinq28() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            XElement element1 = doc.Element("Root");
            XElement element2 = doc.Descendants("Customers")
                                .ElementAt(3);
            var query = from a in element1.AncestorsAndSelf()
                        from b in element2.AncestorsAndSelf()
                        where a == b
                        select a;
            Console.WriteLine(query.Any());
        }

        [Category("Query")]
        [Title("Query for parent")]
        [Description("Query for parent of an Element")]
        public void XLinq29() {
            XElement item = new XElement("item-01",
                                         "Computer");
            XElement order = new XElement("order", item);
            XElement p = item.Parent;
            Console.WriteLine(p.Name);
        }

        [Category("Query")]
        [Title("Join over two sequences")]
        [Description("Add customer company info to orders of the first customer")]
        public void XLinq30() {
            XDocument customers = XDocument.Load(dataPath + "nw_customers.xml");
            XDocument orders = XDocument.Load(dataPath + "nw_orders.xml");

            var query =
                from customer in customers.Descendants("Customers").Take(1)
                join order in orders.Descendants("Orders")
                           on (string)customer.Attribute("CustomerID") equals
                              (string)order.Element("CustomerID")
                select
                    new XElement("Order",
                                order.Nodes(),
                                customer.Element("CompanyName"));

            foreach (var result in query)
                Console.WriteLine(result);

        }

        [Category("Query")]
        [Title("Query content of a type")]
        [Description("Query content of a given type of an existing element")]
        public void XLinq31() {
            XElement elem =
              new XElement("customer",
                           new XElement("name",
                                        "jack"),
                           "some text",
                           new XComment("new customer"),
                           new XAttribute("id",
                                          "abc"));

            //string content
            foreach (XText s in elem.Nodes().OfType<XText>())
                Console.WriteLine("String content: {0}", s);

            //element content
            foreach (XElement e in elem.Elements())
                Console.WriteLine("Element content: {0}", e);

            //comment content
            foreach (XComment c in elem.Nodes().OfType<XComment>())
                Console.WriteLine("Comment content: {0}", c);

        }

        [Category("Query")]
        [Title("Query using XStreamingElement")]
        [Description("Query for all Swedish customer orders and Swedish orders whose freight is > 250")]
        [LinkedMethod("GetSwedishFreightProfile")]
        [LinkedMethod("GetSwedishCustomerOrders")]
        [LinkedMethod("AddNewOrder")]
        public void XLinq32() {
            XDocument customers = XDocument.Load(dataPath + "nw_customers.xml");
            XDocument orders = XDocument.Load(dataPath + "nw_orders.xml");
            XStreamingElement summary = new XStreamingElement("Summary",
                new XAttribute("Country", "Sweden"),
                new XStreamingElement("SwedishCustomerOrders", GetSwedishCustomerOrders(customers, orders)),
                new XStreamingElement("Orders", GetSwedishFreightProfile(orders)));

            Console.WriteLine(summary);
            //DML operation, which adds a new order for customer BERGS freight > 250  
            AddNewOrder(orders);
            Console.WriteLine("****XStreaming Output after DML reflects new order added!!****");
            Console.WriteLine(summary);
        }

        static void AddNewOrder(XDocument orders) {
            string order = @"<Orders>
                                   <CustomerID>BERGS</CustomerID>
                                   <ShipInfo ShippedDate='1997-12-09T00:00:00'>
                                   <Freight>301</Freight>
                                   <ShipCountry>Sweden</ShipCountry>
                                   </ShipInfo>
                                   </Orders>";
            XElement newOrder = XElement.Parse(order);
            orders.Root.Add(newOrder);
        }

        private static IEnumerable<XElement> GetSwedishFreightProfile(XDocument orders) {
            return
               from
                order in orders.Descendants("Orders")
               where
                (string)order.Element("ShipInfo").Element("ShipCountry") == "Sweden"
                  && (float)order.Element("ShipInfo").Element("Freight") > 250
               select
                  new XElement("Order",
                  new XAttribute("Freight",
                                 (string)order.Element("ShipInfo").Element("Freight")));

        }

        private static IEnumerable<XElement> GetSwedishCustomerOrders(XDocument customers, XDocument orders) {
            return
               from
                customer in customers.Descendants("Customers")
               where
                (string)customer.Element("FullAddress").Element("Country") == "Sweden"
               select
                 new XElement("Customer",
                 new XAttribute("Name",
                                (string)customer.Element("CompanyName")),
                 new XAttribute("OrderCount",
                                (from
                                  order in orders.Descendants("Orders")
                                 where
                                   (string)order.Element("CustomerID") == (string)customer.Attribute("CustomerID")
                                 select
                                   order).Count()));

        }

        [Category("Query")]
        [Title("Positional predicate")]
        [Description("Query the 3rd customer in the document")]
        public void XLinq33() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var c = doc.Descendants("Customers")
                       .ElementAt(2);
            Console.WriteLine(c);
        }

        [Category("Query")]
        [Title("Union two sequences of nodes")]
        [Description("Union books authored by two authors: Serge and Peter")]
        public void XLinq34() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            var b1 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                     .Elements("first")
                                     .Any(f => (string)f == "Serge"));
            var b2 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                     .Elements("first")
                                     .Any(f => (string)f == "Peter"));
            var books = b1.Union(b2);
            foreach (var b in books)
                Console.WriteLine(b);
        }

        [Category("Query")]
        [Title("Intersect two sequences of nodes")]
        [Description("Intersect books that are common for both authors")]
        public void XLinq35() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            var b1 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                        .Elements("first")
                                        .Any(f => (string)f == "Serge"));
            var b2 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                        .Elements("first")
                                        .Any(f => (string)f == "Peter"));
            var books = b1.Intersect(b2);
            foreach (var b in books)
                Console.WriteLine(b);
        }

        [Category("Query")]
        [Title("All nodes in sequence 1 except the nodes in sequence 2")]
        [Description("Find books that are authored by Peter and did not have Serge as co-author")]
        public void XLinq36() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            var b1 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                     .Elements("first")
                                     .Any(f => (string)f == "Serge"));
            var b2 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                     .Elements("first")
                                     .Any(f => (string)f == "Peter"));
            var books = b2.Except(b1);
            foreach (var b in books)
                Console.WriteLine(b);
        }

        [Category("Query")]
        [Title("Reverse the order of nodes in a sequence")]
        [Description("Display the path to a node")]
        [LinkedMethod("PrintPath")]
        public void XLinq37() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            XElement e = doc.Descendants("last")
                            .First();
            PrintPath(e);
        }

        static void PrintPath(XElement e) {
            var nodes = e.AncestorsAndSelf()
                            .Reverse();
            foreach (var n in nodes)
                Console.Write(n.Name + (n == e ? "" : "->"));
        }


        [Category("Query")]
        [Title("Equality of sequences")]
        [Description("Check if 2 sequences of nodes are equal. " +
                     "Did Serge and peter co-author all of their the books?")]
        public void XLinq38() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            var b1 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                        .Elements("first")
                                        .Any(f => (string)f == "Serge"));
            var b2 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                        .Elements("first")
                                        .Any(f => (string)f == "Peter"));
            bool result = b2.SequenceEqual(b1);
            Console.WriteLine(result);
        }


        [Category("Query")]
        [Title("TakeWhile operator")]
        [Description("List books until total price is less that $150")]
        public void XLinq39() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            double sum = 0;
            var query = doc.Descendants("book")
                           .TakeWhile(c => (sum += (double)c.Element("price")) <= 150);
            foreach (var result in query)
                Console.WriteLine(result);
        }

        [Category("Query")]
        [Title("Create a list of numbers")]
        [Description("Create 5 new customers with different IDs")]
        public void XLinq40() {
            var query = from
                           i in Enumerable.Range(1, 5)
                        select
                            new XElement("Customer",
                                         new XAttribute("id", i),
                                         "New customer");
            foreach (var result in query)
                Console.WriteLine(result);
        }

        [Category("Query")]
        [Title("Repeat operator")]
        [Description("Initialize new orders with items")]
        public void XLinq41() {
            var orders = new XElement[] {
                new XElement("order", new XAttribute("itemCount",5)),
                new XElement("order", new XAttribute("itemCount",2)),
                new XElement("order", new XAttribute("itemCount",3))};

            //add empty items
            foreach (var order in orders)
                order.Add(Enumerable.Repeat(new XElement("item", "New item"),
                                          (int)order.Attribute("itemCount")));

            foreach (var o in orders)
                Console.WriteLine(o);

        }

        [Category("Query")]
        [Title("Any operator")]
        [Description("Check if there are any customers in Argentina")]
        public void XLinq42() {
            XDocument doc = XDocument.Load(dataPath + "nw_Customers.xml");
            if (doc.Descendants("Country").Any(c => (string)c == "Argentina"))
                Console.WriteLine("There are cusotmers in Argentina");
            else
                Console.WriteLine("There are no cusotmers in Argentina");
        }

        [Category("Query")]
        [Title("All operator")]
        [Description("Check if all books have at least one author")]
        public void XLinq43() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            bool query = doc.Descendants("book")
                            .All(b => b.Descendants("author").Count() > 0);
            if (query)
                Console.WriteLine("All books have authors");
            else
                Console.WriteLine("Some books dont have authors");
        }

        [Category("Query")]
        [Title("Count operartor")]
        [Description("Find the number of orders for a customer")]
        public void XLinq44() {
            XDocument doc = XDocument.Load(dataPath + "nw_Orders.xml");
            var query = doc.Descendants("Orders")
                           .Where(o => (string)o.Element("CustomerID") == "VINET");
            Console.WriteLine("Customer has {0} orders", query.Count());
        }

        [Category("Query")]
        [Title("Aggregate operator")]
        [Description("Find tax on an order")]
        [LinkedMethod("Tax")]
        public void XLinq45() {
            string xml = "<order >" +
                            "<item price='150'>Motor</item>" +
                            "<item price='50'>Cable</item>" +
                         "</order>";

            XElement order = XElement.Parse(xml);
            double tax = order.Elements("item")
                              .Aggregate((double)0, Tax);

            Console.WriteLine("The total tax on the order @10% is ${0}", tax);

        }
        static double Tax(double seed, XElement item) {
            return seed + (double)item.Attribute("price") * 0.1;
        }

        [Category("Query")]
        [Title("Distinct operator")]
        [Description("Find all the countries where there is a customer")]
        public void XLinq46() {
            XDocument doc = XDocument.Load(dataPath + "nw_Customers.xml");
            var countries = doc.Descendants("Country")
                               .Select(c => (string)c)
                               .Distinct()
                               .OrderBy(c => c);
            foreach (var c in countries)
                Console.WriteLine(c);
        }

        [Category("Query")]
        [Title("Concat operator")]
        [Description("List all books by Serge and Peter with co-authored books repeated")]
        public void XLinq47() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            var b1 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                     .Elements("first")
                                     .Any(f => (string)f == "Serge"));
            var b2 = doc.Descendants("book")
                        .Where(b => b.Elements("author")
                                     .Elements("first")
                                     .Any(f => (string)f == "Peter"));
            var books = b1.Concat(b2);
            foreach (var b in books)
                Console.WriteLine(b);

        }
        [Category("Query")]
        [Title("Take operator")]
        [Description("Query the first two customers")]
        public void XLinq48() {
            XDocument doc = XDocument.Load(dataPath + "nw_Customers.xml");
            var customers = doc.Descendants("Customers").Take(2);
            foreach (var c in customers)
                Console.WriteLine(c);
        }

        [Category("Query")]
        [Title("Skip operator")]
        [Description("Skip the first 3 books")]
        public void XLinq49() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            var books = doc.Descendants("book").Skip(3);
            foreach (var b in books)
                Console.WriteLine(b);
        }

        [Category("Query")]
        [Title("Skip nodes based on a condition")]
        [Description("Print items that dont fit in budget")]
        public void XLinq50() {
            string xml = "<order >" +
                            "<item price='150'>Motor</item>" +
                            "<item price='50'>Cable</item>" +
                            "<item price='50'>Modem</item>" +
                            "<item price='250'>Monitor</item>" +
                            "<item price='10'>Mouse</item>" +
                        "</order>";
            XElement order = XElement.Parse(xml);
            int sum = 0;
            var items = order.Descendants("item")
                             .SkipWhile(i => (sum += (int)i.Attribute("price")) < 300);
            foreach (var i in items)
                Console.WriteLine("{0} does not fit in you budget", (string)i);
        }

        [Category("Query")]
        [Title("SelectMany operator")]
        [Description("Get all books authored by Serge and Peter")]
        [LinkedMethod("GetBooks")]
        public void XLinq51() {
            string[] authors = { "Serge", "Peter" };
            var books = authors.SelectMany(a => GetBooks(a));
            foreach (var b in books)
                Console.WriteLine(b);

        }

        public IEnumerable<XElement> GetBooks(string author) {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            var query = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                         .Elements("first")
                                         .Any(f => (string)f == author));

            return query;
        }

        [Category("Query")]
        [Title("Container document")]
        [Description("Find the container document of an element")]
        public void XLinq52() {
            XElement c = XDocument.Load(dataPath + "bib.xml").Descendants("book").First();
            XDocument container = c.Document;
            Console.WriteLine(container);
        }

        [Category("Grouping")]
        [Title("Group orders by customer")]
        [Description("Group orders by customer")]
        public void XLinq53() {
            XDocument doc = XDocument.Load(dataPath + "nw_orders.xml");
            var query =
                from
                    o in doc.Descendants("Orders")
                group o by
                    (string)o.Element("CustomerID") into oGroup
                select
                    oGroup;
            foreach (var result in query.SelectMany(g => g))
                Console.WriteLine(result);
        }

        [Category("Grouping")]
        [Title("Group customers by country and city")]
        [Description("Create a directory of customers grouped by country and city")]
        public void XLinq54() {
            XDocument customers = XDocument.Load(dataPath + "nw_customers.xml");
            XElement directory =
             new XElement("directory",
                from customer in customers.Descendants("Customers")
                from country in customer.Elements("FullAddress").Elements("Country")
                group customer by (string)country into countryGroup
                let country = countryGroup.Key
                select
                    new XElement("Country",
                                 new XAttribute("name",
                                                country),
                                 new XAttribute("numberOfCustomers",
                                                countryGroup.Count()),
                                 from customer in countryGroup
                                 from city in customer.Descendants("City")
                                 group customer by (string)city into cityGroup
                                 let city = cityGroup.Key
                                 select
                                    new XElement("City",
                                                 new XAttribute("name",
                                                                city),
                                                 new XAttribute("numberOfCustomers",
                                                                cityGroup.Count()),
                                                                cityGroup.Elements("ContactName"))));
            Console.WriteLine(directory);

        }

        [Category("Grouping")]
        [Title("Group orders by customer")]
        [Description("Group orders by customer and return all customers (+ orders) for customers who have more than 25 orders ")]
        public void XLinq55() {
            XDocument customers = XDocument.Load(dataPath + "nw_customers.xml");
            XDocument orders = XDocument.Load(dataPath + "nw_orders.xml");
            XElement custOrder = new XElement("CustomerOrders",
                from
                    order in orders.Descendants("Orders")
                group order by
                    order.Element("CustomerID") into cust_orders
                where
                    cust_orders.Count() > 25
                select
                    new XElement("Customer",
                        new XAttribute("CustomerID", cust_orders.Key.Value),
                        from
                            customer in customers.Descendants("Customers")
                        where
                            (string)customer.Attribute("CustomerID") == (string)cust_orders.Key.Value
                        select
                            customer.Nodes(),
                        cust_orders));

            Console.WriteLine(custOrder);
        }

        [Category("Sort")]
        [Title("Sort customers by name")]
        [Description("Sort customers by name in ascending order")]
        public void XLinq56() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var query =
                from
                    customer in doc.Descendants("Customers")
                orderby
                    (string)customer.Elements("ContactName").First()
                select
                    customer.Element("ContactName");
            XElement result =
                new XElement("SortedCustomers", query);
            Console.WriteLine(result);
        }

        [Category("Sort")]
        [Title("Sort orders by date")]
        [Description("Sort orders by date in ascending order")]
        public void XLinq57() {
            XDocument doc = XDocument.Load(dataPath + "nw_orders.xml");
            var query =
                from order in doc.Descendants("Orders")
                let date = (DateTime)order.Element("OrderDate")
                orderby date
                select
                    new XElement("Order",
                                 new XAttribute("date",
                                                date),
                                 new XAttribute("custid",
                                                (string)order.Element("CustomerID")));
            XElement result = new XElement("SortedOrders", query);
            Console.WriteLine(result);

        }

        [Category("Sort")]
        [Title("Descending order")]
        [Description("Sort customers by name in descending order")]
        public void XLinq58() {
            XDocument doc = XDocument.Load(dataPath + "nw_Customers.xml");
            var query =
                from
                    customer in doc.Descendants("Customers")
                orderby
                    (string)customer.Elements("ContactName").First() descending
                select
                    customer.Element("ContactName");
            foreach (var result in query)
                Console.WriteLine(result);

        }

        [Category("Sort")]
        [Title("Multiple sort keys")]
        [Description("Sort customers by country and city")]
        public void XLinq59() {
            XDocument doc = XDocument.Load(dataPath + "nw_Customers.xml");
            var query =
                from
                    customer in doc.Descendants("Customers")
                orderby
                    (string)customer.Descendants("Country").First(),
                    (string)customer.Descendants("City").First()
                select
                    customer;
            foreach (var result in query)
                Console.WriteLine(result);

        }
        [Category("DML")]
        [Title("Add an element as the last child")]
        [Description("Add an element as the last child")]
        public void XLinq60() {
            XDocument doc = XDocument.Load(dataPath + "config.xml");
            XElement config = doc.Element("config");
            config.Add(new XElement("logFolder", "c:\\log"));
            Console.WriteLine(config);

        }

        [Category("DML")]
        [Title("Add an element as the first child")]
        [Description("Add an element as the first child")]
        public void XLinq61() {
            XDocument doc = XDocument.Load(dataPath + "config.xml");
            XElement config = doc.Element("config");
            config.AddFirst(new XElement("logFolder", "c:\\log"));
            Console.WriteLine(config);

        }

        [Category("DML")]
        [Title("Add multiple elements as children")]
        [Description("Add multiple elements as children")]
        public void XLinq62() {
            XDocument doc = XDocument.Load(dataPath + "config.xml");
            XElement[] first = { 
                new XElement("logFolder", "c:\\log"), 
                new XElement("resultsFolders", "c:\\results")};
            XElement[] last = { 
                new XElement("mode", "client"), 
                new XElement("commPort", "2")};
            XElement config = doc.Element("config");
            config.AddFirst(first);
            config.Add(last);
            Console.WriteLine(config);
        }


        [Category("DML")]
        [Title("Add an attribute to an element")]
        [Description("Add an attribute to an element")]
        public void XLinq63() {
            XElement elem = new XElement("customer",
                                         "this is an XElement",
                                         new XAttribute("id", "abc"));
            Console.WriteLine("Original element {0}", elem);

            elem.Add(new XAttribute("name", "Jack"));
            Console.WriteLine("Updated element {0}", elem);
        }


        [Category("DML")]
        [Title("Add content to an existing element")]
        [Description("Add attributes and elements")]
        public void XLinq64() {
            XElement elem = new XElement("customer",
                                         "this is an XElement",
                                         new XAttribute("id", "abc"));
            Console.WriteLine("Original element {0}", elem);

            object[] additionalContent = { 
                new XElement("phone", "555-555-5555"), 
                new XComment("new customer"), 
                new XAttribute("name", "Jack")};

            elem.Add(additionalContent);
            Console.WriteLine("Updated element {0}", elem);
        }

        [Category("DML")]
        [Title("Replace content of a container (element or document")]
        [Description("Replace content to an existing element")]
        public void XLinq65() {
            XElement elem = new XElement("customer",
                                         "this is an XElement",
                                         new XAttribute("id", "abc"));
            Console.WriteLine("Original element {0}", elem);

            elem.ReplaceNodes("this is a coustomer element");
            Console.WriteLine("Updated element {0}", elem);

            object[] newContent = { 
                "this is a customer element", 
                new XElement("phone", "555-555-5555"), 
                new XComment("new customer"), 
                new XAttribute("name", "Jack") };

            elem.ReplaceNodes(newContent);
            Console.WriteLine("Updated element {0}", elem);

        }

        [Category("DML")]
        [Title("Remove content of an element")]
        [Description("Remove content of an element")]
        public void XLinq66() {
            XElement elem = new XElement("customer",
                                         "this is an XElement",
                                         new XAttribute("id", "abc"));
            Console.WriteLine("Original element {0}", elem);

            elem.RemoveNodes();
            Console.WriteLine("Updated element {0}", elem);

        }

        [Category("DML")]
        [Title("Remove all content")]
        [Description("Remove all content and attributes of an element")]
        public void XLinq67() {
            XElement elem = new XElement("customer",
                                         new XElement("name", "jack"),
                                         "this is an XElement",
                                         new XComment("new customer"),
                                         new XAttribute("id", "abc"));
            Console.WriteLine("Original element {0}", elem);

            elem.RemoveAll();
            Console.WriteLine("Stripped element {0}", elem);
        }

        [Category("DML")]
        [Title("Remove all attributes")]
        [Description("Remove all attributes of an element")]
        public void XLinq68() {
            XElement elem = new XElement("customer",
                                         new XAttribute("name", "jack"),
                                         "this is an XElement",
                                         new XComment("new customer"),
                                         new XAttribute("id", "abc"));
            Console.WriteLine("Original element {0}", elem);

            elem.RemoveAttributes();
            Console.WriteLine("Stripped element {0}", elem);

        }

        [Category("DML")]
        [Title("Remove an attribute of an element")]
        [Description("Remove an attribute of an element")]
        public void XLinq69() {
            XElement elem = new XElement("customer",
                                         new XAttribute("name", "jack"),
                                         "this is an XElement",
                                         new XComment("new customer"),
                                         new XAttribute("id", "abc"));
            Console.WriteLine("Original element {0}", elem);

            elem.SetAttributeValue("id", null);
            Console.WriteLine("Updated element {0}", elem);

        }

        [Category("DML")]
        [Title("Update an attribute")]
        [Description("Update the value of an attribute")]
        public void XLinq70() {
            XElement elem = new XElement("customer",
                                         new XAttribute("name", "jack"),
                                         "this is an XElement",
                                         new XComment("new customer"),
                                         new XAttribute("id", "abc"));
            Console.WriteLine("Original element {0}", elem);

            elem.SetAttributeValue("name", "David");
            Console.WriteLine("Updated attribute {0}", elem);

        }


        [Category("DML")]
        [Title("Delete an element by name")]
        [Description("Remove a child element by name")]
        public void XLinq71() {
            XElement elem = new XElement("customer",
                                         new XElement("name", "jack"),
                                         "this is an XElement",
                                         new XComment("new customer"),
                                         new XAttribute("id", "abc"));
            Console.WriteLine("Original element {0}", elem);

            elem.SetElementValue("name", null);
            Console.WriteLine("Updated element {0}", elem);
        }

        [Category("DML")]
        [Title("Update a child element by name")]
        [Description("Update a child element by name")]
        public void XLinq72() {
            XElement elem = new XElement("customer",
                                         new XElement("name", "jack"),
                                         "this is an XElement",
                                         new XComment("new customer"),
                                         new XAttribute("id", "abc"));
            Console.WriteLine("Original element {0}", elem);

            elem.SetElementValue("name", "David");
            Console.WriteLine("Updated element {0}", elem);
        }

        [Category("DML")]
        [Title("Remove a list of elements")]
        [Description("Remove a list of elements")]
        public void XLinq73() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var elems = doc.Descendants("Customers");
            Console.WriteLine("Before count {0}", elems.Count());

            elems.Take(15).Remove();
            Console.WriteLine("After count {0}", elems.Count());
        }

        [Category("DML")]
        [Title("Remove a list of attributes")]
        [Description("Remove a list of attributes")]
        public void XLinq74() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var attrs = doc.Descendants("Customers")
                           .Attributes();
            Console.WriteLine("Before count {0}", attrs.Count());

            attrs.Take(15).Remove();
            Console.WriteLine("After count {0}", attrs.Count());
        }


        [Category("DML")]
        [Title("Add an un-parented element to an element")]
        [Description("Add an un-parented element to an element")]
        public void XLinq75() {
            XElement e = new XElement("foo",
                                      "this is an element");
            Console.WriteLine("Parent : " +
                (e.Parent == null ? "null" : e.Parent.Value));

            XElement p = new XElement("bar", e); //add to document
            Console.WriteLine("Parent : " +
                (e.Parent == null ? "null" : e.Parent.Name));
        }

        [Category("DML")]
        [Title("Add an parented element to a document")]
        [Description("Adding a parented element to another container clones it")]
        public void XLinq76() {
            XElement e = new XElement("foo",
                                      "this is an element");
            XElement p1 = new XElement("p1", e);
            Console.WriteLine("Parent : " + e.Parent.Name);

            XElement p2 = new XElement("p2", e);
            Console.WriteLine("Parent : " + e.Parent.Name);
        }

        [Category("Transform")]
        [Title("Create a table of customers")]
        [Description("Generate html with a list of customers that is numbered")]
        public void XLinq77() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");

            var header = new[]{                 
                    new XElement("th","#"),
                    new XElement("th",
                                 "customer id"),
                    new XElement("th",
                                 "contact name")};
            int index = 0;
            var rows =
                from
                    customer in doc.Descendants("Customers")
                select
                    new XElement("tr",
                                 new XElement("td",
                                              ++index),
                                 new XElement("td",
                                              (string)customer.Attribute("CustomerID")),
                                 new XElement("td",
                                              (string)customer.Element("ContactName")));

            XElement html = new XElement("html",
                                  new XElement("body",
                                    new XElement("table",
                                      header,
                                      rows)));
            Console.Write(html);
        }

        [Category("Transform")]
        [Title("Create html tables of books")]
        [Description("Generate a html tables of books by authors")]
        [LinkedMethod("GetBooksTable")]
        public void XLinq78() {
            XDocument doc = XDocument.Load(dataPath + "bib.xml");
            var content =
                from b in doc.Descendants("book")
                from a in b.Elements("author")
                group b by (string)a.Element("first") + " " + (string)a.Element("last") into authorGroup
                select new XElement("p", "Author: " + authorGroup.Key,
                                           GetBooksTable(authorGroup));

            XElement result =
                new XElement("html",
                            new XElement("body",
                             content));
            Console.WriteLine(result);
        }

        static XElement GetBooksTable(IEnumerable<XElement> books) {
            var header = new XElement[]{
                            new XElement("th","Title"), 
                            new XElement("th", "Year")};
            var rows =
                from
                    b in books
                select
                    new XElement("tr",
                                 new XElement("td",
                                              (string)b.Element("title")),
                                 new XElement("td",
                                              (string)b.Attribute("year")));

            return new XElement("table",
                                header,
                                rows);

        }
        [Category("Language Integration")]
        [Title("Find all orders for customers in a List")]
        [Description("Find all orders for customers in a List")]
        [LinkedMethod("SomeMethodToGetCustomers")]
        [LinkedClass("Customer")]
        public void XLinq79() {
            XDocument doc = XDocument.Load(dataPath + "nw_orders.xml");
            List<Customer> customers = SomeMethodToGetCustomers();
            var result =
                from customer in customers
                from order in doc.Descendants("Orders")
                where
                    customer.id == (string)order.Element("CustomerID")
                select
                    new { custid = (string)customer.id, orderdate = (string)order.Element("OrderDate") };
            foreach (var tuple in result)
                Console.WriteLine("Customer id = {0}, Order Date = {1}",
                                    tuple.custid, tuple.orderdate);

        }
        class Customer {
            public Customer(string id) { this.id = id; }
            public string id;
        }
        static List<Customer> SomeMethodToGetCustomers() {
            List<Customer> customers = new List<Customer>();
            customers.Add(new Customer("VINET"));
            customers.Add(new Customer("TOMSP"));
            return customers;
        }

        [Category("Language Integration")]
        [Title("Find sum of items in a shopping cart")]
        [Description("Find sum of items in a shopping cart")]
        [LinkedMethod("GetShoppingCart")]
        [LinkedClass("Item")]
        public void XLinq80() {
            XDocument doc = XDocument.Load(dataPath + "inventory.xml");
            List<Item> cart = GetShoppingCart();
            var subtotal =
                from item in cart
                from inventory in doc.Descendants("item")
                where item.id == (string)inventory.Attribute("id")
                select (double)item.quantity * (double)inventory.Element("price");
            Console.WriteLine("Total payment = {0}", subtotal.Sum());
        }
        class Item {
            public Item(string id, int quantity) {
                this.id = id;
                this.quantity = quantity;
            }
            public int quantity;
            public string id;
        }
        static List<Item> GetShoppingCart() {
            List<Item> items = new List<Item>();
            items.Add(new Item("1", 10));
            items.Add(new Item("5", 5));
            return items;
        }

        [Category("Language Integration")]
        [Title("Consume a config file")]
        [Description("Load and use a config file")]
        [LinkedMethod("Initialize")]
        public void XLinq81() {
            XElement config = XDocument.Load(dataPath + "config.xml").Element("config");
            Initialize((string)config.Element("rootFolder"),
                       (int)config.Element("iterations"),
                       (double)config.Element("maxMemory"),
                       (string)config.Element("tempFolder"));

        }
        static void Initialize(string root, int iter, double mem, string temp) {
            Console.WriteLine("Application initialized to root folder: " +
                              "{0}, iterations: {1}, max memory {2}, temp folder: {3}",
                              root, iter, Convert.ToString(mem, CultureInfo.InvariantCulture), temp);
        }

        [Category("Language Integration")]
        [Title("Convert a Sequence of nodes to Array")]
        [Description("Convert a Sequence of nodes to Array")]
        public void XLinq82() {
            XDocument doc = XDocument.Load(dataPath + "nw_Customers.xml");
            XElement[] custArray = doc.Descendants("Customers").ToArray();
            foreach (var c in custArray)
                Console.WriteLine(c);
        }

        [Category("Language Integration")]
        [Title("Convert a Sequence of nodes to List")]
        [Description("Convert a Sequence of nodes to List")]
        public void XLinq83() {
            XDocument doc = XDocument.Load(dataPath + "nw_Customers.xml");
            List<XElement> clist = doc.Descendants("Customers").ToList();
            foreach (var c in clist)
                Console.WriteLine(c);
        }

        [Category("Language Integration")]
        [Title("Create a dictionary of customers")]
        [Description("Create a dictionary of customers")]
        public void XLinq84() {
            XDocument doc = XDocument.Load(dataPath + "nw_Customers.xml");
            var dictionary = doc.Descendants("Customers")
                                .ToDictionary(c => (string)c.Attribute("CustomerID"));
            Console.WriteLine(dictionary["ALFKI"]);
        }


        [Category("Language Integration")]
        [Title("Using anonnymous types ")]
        [Description("Number all the countries and list them")]
        public void XLinq85() {
            XDocument doc = XDocument.Load(dataPath + "nw_Customers.xml");
            var countries = doc.Descendants("Country")
                                .Select(c => (string)c)
                                .Distinct()
                                .Select((c, index) => new { i = index, name = c });
            foreach (var c in countries)
                Console.WriteLine(c.i + " " + c.name);

        }

        [Category("XName")]
        [Title("Create elements and attributes in a namespace")]
        [Description("Create elements and attributes in a namespace")]
        public void XLinq86() {
            XNamespace ns = "http://myNamespace";
            XElement result = new XElement(ns + "foo",
                                           new XAttribute(ns + "bar", "attribute"));
            Console.WriteLine(result);
        }

        [Category("XName")]
        [Title("Query for elements in a namespace")]
        [Description("Find xsd:element with name=FullAddress")]
        public void XLinq87() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xsd");
            XNamespace XSD = "http://www.w3.org/2001/XMLSchema";
            XElement result = doc.Descendants(XSD + "element")
                                 .Where(e => (string)e.Attribute("name") == "FullAddress")
                                 .First();
            Console.WriteLine(result);
        }

        [Category("XName")]
        [Title("Create a namespace prefix declaration")]
        [Description("Create a namespace prefix declaration")]
        public void XLinq88() {
            XNamespace myNS = "http://myNamespace";
            XElement result = new XElement("myElement",
                                           new XAttribute(XNamespace.Xmlns + "myPrefix", myNS));
            Console.WriteLine(result);
        }

        [Category("XName")]
        [Title("Local-name and namespace")]
        [Description("Get the local-name and namespace of an element")]
        public void XLinq89() {
            XNamespace ns = "http://myNamespace";
            XElement e = new XElement(ns + "foo");

            Console.WriteLine("Local name of element: {0}", e.Name.LocalName);
            Console.WriteLine("Namespace of element : {0}", e.Name.Namespace.NamespaceName);

        }

        [Category("Misc")]
        [Title("Get the outer XML of a node")]
        [Description("Get the outer XML of a node")]
        public void XLinq90() {
            string xml = "<order >" +
                            "<item price='150'>Motor</item>" +
                            "<item price='50'>Cable</item>" +
                            "<item price='50'>Modem</item>" +
                            "<item price='250'>Monitor</item>" +
                            "<item price='10'>Mouse</item>" +
                         "</order>";
            XElement order = XElement.Parse(xml);
            Console.WriteLine(order.ToString(SaveOptions.DisableFormatting));
        }

        [Category("Misc")]
        [Title("Get the inner text of a node")]
        [Description("Get the inner text of a node")]
        public void XLinq91() {
            string xml = "<order >" +
                            "<item price='150'>Motor</item>" +
                            "<item price='50'>Cable</item>" +
                            "<item price='50'>Modem</item>" +
                            "<item price='250'>Monitor</item>" +
                            "<item price='10'>Mouse</item>" +
                         "</order>";
            XElement order = XElement.Parse(xml);
            Console.WriteLine(order.Value);

        }

        [Category("Misc")]
        [Title("Check if an element has attributes")]
        [Description("Check if an element has attributes")]
        public void XLinq92() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            XElement e = doc.Element("Root")
                            .Element("Customers");
            Console.WriteLine("Customers has attributes? {0}", e.HasAttributes);

        }

        [Category("Misc")]
        [Title("Check if an element has element children")]
        [Description("Check if an element has element children")]
        public void XLinq93() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            XElement e = doc.Element("Root")
                            .Element("Customers");
            Console.WriteLine("Customers has elements? {0}", e.HasElements);

        }

        [Category("Misc")]
        [Title("Check if an element is empty")]
        [Description("Check if an element is empty")]
        public void XLinq94() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            XElement e = doc.Element("Root")
                            .Element("Customers");
            Console.WriteLine("Customers element is empty? {0}", e.IsEmpty);

        }

        [Category("Misc")]
        [Title("Get the name of an element")]
        [Description("Get the name of an element")]
        public void XLinq95() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            XElement e = doc.Elements()
                            .First();
            Console.WriteLine("Name of element {0}", e.Name);
        }

        [Category("Misc")]
        [Title("Get the name of an attribute")]
        [Description("Get the name of an attribute")]
        public void XLinq96() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            XAttribute a = doc.Element("Root")
                              .Element("Customers")
                              .Attributes().First();
            Console.WriteLine("Name of attribute {0}", a.Name);
        }

        [Category("Misc")]
        [Title("Get the XML declaration")]
        [Description("Get the XML declaration")]
        public void XLinq97() {
            XDocument doc = XDocument.Load(dataPath + "config.xml");
            Console.WriteLine("Version {0}", doc.Declaration.Version);
            Console.WriteLine("Encoding {0}", doc.Declaration.Encoding);
            Console.WriteLine("Standalone {0}", doc.Declaration.Standalone);
        }


        [Category("Misc")]
        [Title("Find the type of the node")]
        [Description("Find the type of the node")]
        public void XLinq98() {
            XNode o = new XElement("foo");
            Console.WriteLine(o.NodeType);
        }

        [Category("Misc")]
        [Title("Verify phone numbers")]
        [Description("Verify that the phone numbers of the format xxx-xxx-xxxx")]
        [LinkedMethod("CheckPhone")]
        public void XLinq99() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            var query =
                from customer in doc.Descendants("Customers")
                select
                    new XElement("customer",
                                 customer.Attribute("CustomerID"),
                                 customer.Descendants("Phone").First(),
                                 CheckPhone((string)customer.Descendants("Phone").First()));
            foreach (var result in query)
                Console.WriteLine(result);

        }
        static XElement CheckPhone(string phone) {
            Regex regex = new Regex("([0-9]{3}-)|('('[0-9]{3}')')[0-9]{3}-[0-9]{4}");
            return new XElement("isValidPhone", regex.IsMatch(phone));
        }

        [Category("Misc")]
        [Title("Quick validation")]
        [Description("Validate file structure")]
        [LinkedMethod("VerifyCustomer")]
        public void XLinq100() {
            XDocument doc = XDocument.Load(dataPath + "nw_customers.xml");
            foreach (XElement customer in doc.Descendants("Customers")) {
                string err = VerifyCustomer(customer);
                if (err != "")
                    Console.WriteLine("Cusotmer {0} is invalid. Missing {1}",
                                        (string)customer.Attribute("CustomerID"), err);
            }

        }
        static string VerifyCustomer(XElement c) {
            if (c.Element("CompanyName") == null)
                return "CompanyName";
            if (c.Element("ContactName") == null)
                return "ContactName";
            if (c.Element("ContactTitle") == null)
                return "ContactTitle";
            if (c.Element("Phone") == null)
                return "Phone";
            if (c.Element("Fax") == null)
                return "Fax";
            if (c.Element("FullAddress") == null)
                return "FullAddress";
            return "";
        }


        [Category("Misc")]
        [Title("Aggregate functions")]
        [Description("Calculate sum, average, min, max of freight of all orders")]
        public void XLinq101() {
            XDocument doc = XDocument.Load(dataPath + "nw_orders.xml");
            var query =
                from
                    order in doc.Descendants("Orders")
                where
                    (string)order.Element("CustomerID") == "VINET" &&
                    order.Elements("ShipInfo").Elements("Freight").Any()
                select
                    (double)order.Element("ShipInfo").Element("Freight");

            double sum = query.Sum();
            double average = query.Average();
            double min = query.Min();
            double max = query.Max();

            Console.WriteLine("Sum: {0}, Average: {1}, Min: {2}, Max: {3}", Convert.ToString(sum, CultureInfo.InvariantCulture),
                Convert.ToString(average, CultureInfo.InvariantCulture), Convert.ToString(min, CultureInfo.InvariantCulture),
                Convert.ToString(max, CultureInfo.InvariantCulture));
        }

        [Category("Events")]
        [Title("Add/Remove events")]
        [Description("Attach event listners to the root element and add/remove child element")]
        [LinkedMethod("OnChanging")]
        [LinkedMethod("OnChanged")]
        public void XLinq102() {
            string xml = "<order >" +
                            "<item price='150'>Motor</item>" +
                            "<item price='50'>Cable</item>" +
                            "<item price='50'>Modem</item>" +
                            "<item price='250'>Monitor</item>" +
                            "<item price='10'>Mouse</item>" +
                         "</order>";
            XDocument order = XDocument.Parse(xml);
            // Add event listners to order
            order.Changing += new EventHandler<XObjectChangeEventArgs>(OnChanging);
            order.Changed += new EventHandler<XObjectChangeEventArgs>(OnChanged);
            // Add a new item to order
            Console.WriteLine("Original Order:");
            Console.WriteLine("{0}", order.ToString(SaveOptions.None));
            order.Root.Add(new XElement("item", new XAttribute("price", 350), "Printer"));
            Console.WriteLine("After Add:");
            Console.WriteLine("{0}", order.ToString(SaveOptions.None));
            order.Root.Element("item").Remove();
            Console.WriteLine("After Remove:");
            Console.WriteLine("{0}", order.ToString(SaveOptions.None));
            // Remove event listners
            order.Changing -= new EventHandler<XObjectChangeEventArgs>(OnChanging);
            order.Changed -= new EventHandler<XObjectChangeEventArgs>(OnChanged);
            // Add another element, events should not be raised
            order.Root.Add(new XElement("item", new XAttribute("price", 350), "Printer"));
        }

        [Category("Events")]
        [Title("Name/Value change events")]
        [Description("Attach event listners to the root element and change the element name/value")]
        [LinkedMethod("OnChanging")]
        [LinkedMethod("OnChanged")]
        public void XLinq103() {
            string xml = "<order >" +
                            "<item price='150'>Motor</item>" +
                            "<item price='50'>Cable</item>" +
                            "<item price='50'>Modem</item>" +
                            "<item price='250'>Monitor</item>" +
                            "<item price='10'>Mouse</item>" +
                         "</order>";
            XElement order = XElement.Parse(xml);
            order.Changing += new EventHandler<XObjectChangeEventArgs>(OnChanging);
            order.Changed += new EventHandler<XObjectChangeEventArgs>(OnChanged);
            Console.WriteLine("Original Order:");
            Console.WriteLine("{0}", order.ToString(SaveOptions.None));
            order.Name = "newOrder";
            Console.WriteLine("After Name Change:");
            Console.WriteLine("{0}", order.ToString(SaveOptions.None));
            order.Element("item").Value = "New Item";
            Console.WriteLine("After Value Change:");
            Console.WriteLine("{0}", order.ToString(SaveOptions.None));
        }

        [Category("Events")]
        [Title("Multiple event handlers")]
        [Description("Attach event listners to multiple elements in the tree")]
        [LinkedMethod("OnChanging")]
        [LinkedMethod("OnChanged")]
        public void XLinq104() {
            string xml = "<order >" +
                            "<item price='150'>Motor</item>" +
                            "<item price='50'>Cable</item>" +
                            "<item price='50'>Modem</item>" +
                            "<item price='250'>Monitor</item>" +
                            "<item price='10'>Mouse</item>" +
                         "</order>";
            XDocument order = XDocument.Parse(xml);
            order.Changing += new EventHandler<XObjectChangeEventArgs>(OnChanging);
            order.Changed += new EventHandler<XObjectChangeEventArgs>(OnChanged);
            order.Root.Changing += new EventHandler<XObjectChangeEventArgs>(OnChanging);
            order.Root.Changed += new EventHandler<XObjectChangeEventArgs>(OnChanged);
            // This element should not receive any events since the change will be made to
            // the root element
            order.Root.Element("item").Changing += new EventHandler<XObjectChangeEventArgs>(OnChanging);
            order.Root.Element("item").Changed += new EventHandler<XObjectChangeEventArgs>(OnChanged);
            Console.WriteLine("Original Order:");
            Console.WriteLine("{0}", order.ToString(SaveOptions.None));
            // This should not raise any events for 'order.Root.Element("item")'
            order.Root.Add(new XElement("item", "Printer"));
            Console.WriteLine("After Add:");
            Console.WriteLine("{0}", order.ToString(SaveOptions.None));
        }

        public void OnChanging(object sender, XObjectChangeEventArgs e) {
            Console.WriteLine();
            Console.WriteLine("OnChanging:");
            Console.WriteLine("EventType: {0}", e.ObjectChange);
            Console.WriteLine("Object: {0}", ((XNode)sender).ToString(SaveOptions.None));
        }

        public void OnChanged(object sender, XObjectChangeEventArgs e) {
            Console.WriteLine();
            Console.WriteLine("OnChanged:");
            Console.WriteLine("EventType: {0}", e.ObjectChange);
            Console.WriteLine("Object: {0}", ((XNode)sender).ToString(SaveOptions.None));
            Console.WriteLine();
        }

        [Category("Events")]
        [Title("Orders Log")]
        [Description("Log orders as they are added")]
        public void XLinq105() {
            string xml = "<order >" +
                            "<item price='150'>Motor</item>" +
                            "<item price='50'>Cable</item>" +
                            "<item price='50'>Modem</item>" +
                            "<item price='250'>Monitor</item>" +
                            "<item price='10'>Mouse</item>" +
                         "</order>";
            XElement order = XElement.Parse(xml);
            int orderCount = 0;
            StringWriter log = new StringWriter();

            order.Changed += new EventHandler<XObjectChangeEventArgs>(
                    delegate(object sender, XObjectChangeEventArgs e) {
                        orderCount++;
                        log.WriteLine(((XNode)sender).ToString(SaveOptions.None));
                    });

            for (int i = 0; i < 100; i++) {
                order.Add(new XElement("item", new XAttribute("price", 350), "Printer"));
            }

            Console.WriteLine("Orders Received: {0}", orderCount);
            Console.WriteLine("Orders Log:");
            Console.WriteLine(log.ToString());
        }
    }
}