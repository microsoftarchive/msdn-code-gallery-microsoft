using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Globalization;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Miscellaneous
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq90();  // Get the outer XML of a node
            //samples.XLinq91();  // Get the inner text of a node
            //samples.XLinq92();  // Check if an element has attributes
            //samples.XLinq93();  // Check if an element has element children
            //samples.XLinq94();  // Check if an element is empty
            //samples.XLinq95();  // Get the name of an element
            //samples.XLinq96();  // Get the name of an attribute
            //samples.XLinq97();  // Get the XML declaration
            //samples.XLinq98();  // Find the type of the node
            //samples.XLinq99();  // Verify that the phone numbers of the format xxx-xxx-xxxx
            //samples.XLinq100(); // Validate file structure
            //samples.XLinq101(); // Calculate sum, average, min, max of freight of all orders
        }

        private class LinqSamples
        {
            [Category("Misc")]
            [Description("Get the outer XML of a node")]
            public void XLinq90()
            {
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
            [Description("Get the inner text of a node")]
            public void XLinq91()
            {
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
            [Description("Check if an element has attributes")]
            public void XLinq92()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                XElement e = doc.Element("Root")
                                .Element("Customers");
                Console.WriteLine("Customers has attributes? {0}", e.HasAttributes);

            }

            [Category("Misc")]
            [Description("Check if an element has element children")]
            public void XLinq93()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                XElement e = doc.Element("Root")
                                .Element("Customers");
                Console.WriteLine("Customers has elements? {0}", e.HasElements);

            }

            [Category("Misc")]
            [Description("Check if an element is empty")]
            public void XLinq94()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                XElement e = doc.Element("Root")
                                .Element("Customers");
                Console.WriteLine("Customers element is empty? {0}", e.IsEmpty);

            }

            [Category("Misc")]
            [Description("Get the name of an element")]
            public void XLinq95()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                XElement e = doc.Elements()
                                .First();
                Console.WriteLine("Name of element {0}", e.Name);
            }

            [Category("Misc")]
            [Description("Get the name of an attribute")]
            public void XLinq96()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                XAttribute a = doc.Element("Root")
                                  .Element("Customers")
                                  .Attributes().First();
                Console.WriteLine("Name of attribute {0}", a.Name);
            }

            [Category("Misc")]
            [Description("Get the XML declaration")]
            public void XLinq97()
            {
                XDocument doc = XDocument.Load("config.xml");
                Console.WriteLine("Version {0}", doc.Declaration.Version);
                Console.WriteLine("Encoding {0}", doc.Declaration.Encoding);
                Console.WriteLine("Standalone {0}", doc.Declaration.Standalone);
            }


            [Category("Misc")]
            [Description("Find the type of the node")]
            public void XLinq98()
            {
                XNode o = new XElement("foo");
                Console.WriteLine(o.NodeType);
            }

            [Category("Misc")]
            [Description("Verify that the phone numbers of the format xxx-xxx-xxxx")]
            public void XLinq99()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
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
            static XElement CheckPhone(string phone)
            {
                Regex regex = new Regex("([0-9]{3}-)|('('[0-9]{3}')')[0-9]{3}-[0-9]{4}");
                return new XElement("isValidPhone", regex.IsMatch(phone));
            }

            [Category("Misc")]
            [Description("Validate file structure")]
            public void XLinq100()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                foreach (XElement customer in doc.Descendants("Customers"))
                {
                    string err = VerifyCustomer(customer);
                    if (err != "")
                        Console.WriteLine("Customer {0} is invalid. Missing {1}",
                                            (string)customer.Attribute("CustomerID"), err);
                }

            }
            static string VerifyCustomer(XElement c)
            {
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
            [Description("Calculate sum, average, min, max of freight of all orders")]
            public void XLinq101()
            {
                XDocument doc = XDocument.Load("nw_orders.xml");
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
        }
    }
}
