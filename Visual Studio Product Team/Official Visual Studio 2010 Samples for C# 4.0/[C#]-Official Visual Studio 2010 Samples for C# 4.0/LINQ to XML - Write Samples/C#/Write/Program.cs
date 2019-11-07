using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace Write
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq15();    // Write an XElement to XmlWriter using the WriteTo method
            //samples.XLinq16();    // Write the content of XDocument to XmlWriter using the WriteTo method
            //samples.XLinq17();    // Save XDocument using XmlWriter/TextWriter/File
        }

        private class LinqSamples
        {
            [Category("Write")]
            [Description("Write an XElement to XmlWriter using the WriteTo method")]
            public void XLinq15()
            {
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
            [Description("Write the content of XDocument to XmlWriter using the WriteTo method")]
            public void XLinq16()
            {
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
            [Description("Save XDocument using XmlWriter/TextWriter/File")]
            public void XLinq17()
            {
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
        }
    }
}
