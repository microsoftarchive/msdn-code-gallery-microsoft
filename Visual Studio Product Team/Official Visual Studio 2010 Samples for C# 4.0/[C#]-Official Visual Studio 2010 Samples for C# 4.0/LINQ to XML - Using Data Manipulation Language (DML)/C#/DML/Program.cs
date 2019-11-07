using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace DML
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq60();  // Add an element as the last child
            //samples.XLinq61();  // Add an element as the first child
            //samples.XLinq62();  // Add multiple elements as children
            //samples.XLinq63();  // Add an attribute to an element
            //samples.XLinq64();  // Add attributes and elements
            //samples.XLinq65();  // Replace content to an existing element
            //samples.XLinq66();  // Remove content of an element
            //samples.XLinq67();  // Remove all content and attributes of an element
            //samples.XLinq68();  // Remove all attributes of an element
            //samples.XLinq69();  // Remove an attribute of an element
            //samples.XLinq70();  // Update the value of an attribute
            //samples.XLinq71();  // Remove a child element by name
            //samples.XLinq72();  // Update a child element by name
            //samples.XLinq73();  // Remove a list of elements
            //samples.XLinq74();  // Remove a list of attributes
            //samples.XLinq75();  // Add an un-parented element to an element
            //samples.XLinq76();  // Adding a parented element to another container clones it
        }

        private class LinqSamples
        {
            [Category("DML")]
            [Description("Add an element as the last child")]
            public void XLinq60()
            {
                XDocument doc = XDocument.Load("config.xml");
                XElement config = doc.Element("config");
                config.Add(new XElement("logFolder", "c:\\log"));
                Console.WriteLine(config);

            }

            [Category("DML")]
            [Description("Add an element as the first child")]
            public void XLinq61()
            {
                XDocument doc = XDocument.Load("config.xml");
                XElement config = doc.Element("config");
                config.AddFirst(new XElement("logFolder", "c:\\log"));
                Console.WriteLine(config);

            }

            [Category("DML")]
            [Description("Add multiple elements as children")]
            public void XLinq62()
            {
                XDocument doc = XDocument.Load("config.xml");
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
            [Description("Add an attribute to an element")]
            public void XLinq63()
            {
                XElement elem = new XElement("customer",
                                             "this is an XElement",
                                             new XAttribute("id", "abc"));
                Console.WriteLine("Original element {0}", elem);

                elem.Add(new XAttribute("name", "Jack"));
                Console.WriteLine("Updated element {0}", elem);
            }


            [Category("DML")]
            [Description("Add attributes and elements")]
            public void XLinq64()
            {
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
            [Description("Replace content to an existing element")]
            public void XLinq65()
            {
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
            [Description("Remove content of an element")]
            public void XLinq66()
            {
                XElement elem = new XElement("customer",
                                             "this is an XElement",
                                             new XAttribute("id", "abc"));
                Console.WriteLine("Original element {0}", elem);

                elem.RemoveNodes();
                Console.WriteLine("Updated element {0}", elem);

            }

            [Category("DML")]
            [Description("Remove all content and attributes of an element")]
            public void XLinq67()
            {
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
            [Description("Remove all attributes of an element")]
            public void XLinq68()
            {
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
            [Description("Remove an attribute of an element")]
            public void XLinq69()
            {
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
            [Description("Update the value of an attribute")]
            public void XLinq70()
            {
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
            [Description("Remove a child element by name")]
            public void XLinq71()
            {
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
            [Description("Update a child element by name")]
            public void XLinq72()
            {
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
            [Description("Remove a list of elements")]
            public void XLinq73()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                var elems = doc.Descendants("Customers");
                Console.WriteLine("Before count {0}", elems.Count());

                elems.Take(15).Remove();
                Console.WriteLine("After count {0}", elems.Count());
            }

            [Category("DML")]
            [Description("Remove a list of attributes")]
            public void XLinq74()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                var attrs = doc.Descendants("Customers")
                               .Attributes();
                Console.WriteLine("Before count {0}", attrs.Count());

                attrs.Take(15).Remove();
                Console.WriteLine("After count {0}", attrs.Count());
            }


            [Category("DML")]
            [Description("Add an un-parented element to an element")]
            public void XLinq75()
            {
                XElement e = new XElement("foo",
                                          "this is an element");
                Console.WriteLine("Parent : " +
                    (e.Parent == null ? "null" : e.Parent.Value));

                XElement p = new XElement("bar", e); //add to document
                Console.WriteLine("Parent : " +
                    (e.Parent == null ? "null" : e.Parent.Name));
            }

            [Category("DML")]
            [Description("Adding a parented element to another container clones it")]
            public void XLinq76()
            {
                XElement e = new XElement("foo",
                                          "this is an element");
                XElement p1 = new XElement("p1", e);
                Console.WriteLine("Parent : " + e.Parent.Name);

                XElement p2 = new XElement("p2", e);
                Console.WriteLine("Parent : " + e.Parent.Name);
            }
        }
    }
}
