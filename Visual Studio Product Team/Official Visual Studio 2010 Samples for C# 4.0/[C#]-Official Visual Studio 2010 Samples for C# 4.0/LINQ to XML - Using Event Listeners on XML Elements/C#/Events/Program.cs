using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.ComponentModel;

namespace Events
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq102(); // Attach event listners to the root element and add/remove child element
            //samples.XLinq103(); // Attach event listners to the root element and change the element name/value
            //samples.XLinq104(); // Attach event listners to multiple elements in the tree
            //samples.XLinq105(); // Log orders as they are added
        }

        private class LinqSamples
        {
            [Category("Events")]
            [Description("Attach event listners to the root element and add/remove child element")]
            public void XLinq102()
            {
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
            [Description("Attach event listners to the root element and change the element name/value")]
            public void XLinq103()
            {
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
            [Description("Attach event listners to multiple elements in the tree")]
            public void XLinq104()
            {
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

            public void OnChanging(object sender, XObjectChangeEventArgs e)
            {
                Console.WriteLine();
                Console.WriteLine("OnChanging:");
                Console.WriteLine("EventType: {0}", e.ObjectChange);
                Console.WriteLine("Object: {0}", ((XNode)sender).ToString(SaveOptions.None));
            }

            public void OnChanged(object sender, XObjectChangeEventArgs e)
            {
                Console.WriteLine();
                Console.WriteLine("OnChanged:");
                Console.WriteLine("EventType: {0}", e.ObjectChange);
                Console.WriteLine("Object: {0}", ((XNode)sender).ToString(SaveOptions.None));
                Console.WriteLine();
            }

            [Category("Events")]
            [Description("Log orders as they are added")]
            public void XLinq105()
            {
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
                        delegate(object sender, XObjectChangeEventArgs e)
                        {
                            orderCount++;
                            log.WriteLine(((XNode)sender).ToString(SaveOptions.None));
                        });

                for (int i = 0; i < 100; i++)
                {
                    order.Add(new XElement("item", new XAttribute("price", 350), "Printer"));
                }

                Console.WriteLine("Orders Received: {0}", orderCount);
                Console.WriteLine("Orders Log:");
                Console.WriteLine(log.ToString());
            }
        }
    }
}
