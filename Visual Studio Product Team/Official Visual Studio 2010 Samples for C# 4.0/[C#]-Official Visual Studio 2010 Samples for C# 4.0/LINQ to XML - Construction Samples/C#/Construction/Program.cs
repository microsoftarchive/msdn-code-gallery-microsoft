using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;
using System.IO;

namespace Construction
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq6();     // Construct an XElement from string
            //samples.XLinq7();     // Add XML declaration to a document
            //samples.XLinq8();     // Computed element name
            //samples.XLinq9();     // Create a simple config file
            //samples.XLinq10();    // Create an XmlSchema
            //samples.XLinq11();    // Create an XML document with an XSLT PI
            //samples.XLinq12();    // XML comment construction
            //samples.XLinq13();    // Create a CData section
            //samples.XLinq14();    // Create a sequence of customer elements
        }

        private class LinqSamples
        {
            [Category("Construction")]
            [Description("Construct an XElement from string")]
            public void XLinq6()
            {
                string xml = "<purchaseOrder price='100'>" +
                                "<item price='50'>Motor</item>" +
                                "<item price='50'>Cable</item>" +
                              "</purchaseOrder>";
                XElement po = XElement.Parse(xml);
                Console.WriteLine(po);

            }

            [Category("Construction")]
            [Description("Add XML declaration to a document")]
            public void XLinq7()
            {
                XDocument doc = new XDocument(new XDeclaration("1.0", "UTF-16", "Yes"),
                                              new XElement("foo"));
                StringWriter sw = new StringWriter();
                doc.Save(sw);
                Console.WriteLine(sw);

            }

            [Category("Construction")]
            [Description("Computed element name")]
            public void XLinq8()
            {
                XDocument customers = XDocument.Load("nw_customers.xml");
                string name = (string)customers.Elements("Root")
                                               .Elements("Customers")
                                               .First()
                                               .Attribute("CustomerID");
                XElement result = new XElement(name,
                                                "Element with a computed name");
                Console.WriteLine(result);

            }

            [Category("Construction")]
            [Description("Create a simple config file")]
            public void XLinq9()
            {
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
            [Description("Create an XmlSchema")]
            public void XLinq10()
            {
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
            [Description("Create an XML document with an XSLT PI")]
            public void XLinq11()
            {
                XDocument result = new XDocument(
                                    new XProcessingInstruction("xml-stylesheet",
                                                               "type='text/xsl' href='diff.xsl'"),
                                    new XElement("foo"));
                Console.WriteLine(result);
            }

            [Category("Construction")]
            [Description("XML comment construction")]
            public void XLinq12()
            {
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
            [Description("Create a CData section")]
            public void XLinq13()
            {
                XElement e = new XElement("Dump",
                              new XCData("<dump>this is some xml</dump>"),
                              new XText("some other text"));
                Console.WriteLine("Element Value: {0}", e.Value);
                Console.WriteLine("Text nodes collapsed!: {0}", e.Nodes().First());
                Console.WriteLine("CData preserved on serialization: {0}", e);
            }

            [Category("Construction")]
            [Description("Create a sequence of customer elements")]
            public void XLinq14()
            {
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
        }
    }
}
