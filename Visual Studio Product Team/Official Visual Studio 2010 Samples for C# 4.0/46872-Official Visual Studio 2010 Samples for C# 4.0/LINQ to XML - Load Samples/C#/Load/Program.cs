using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Xml.Linq;

namespace Load
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq1();
            //samples.XLinq2();
            //samples.XLinq3();
            //samples.XLinq4();
            //samples.XLinq5();
        }

        private class LinqSamples
        {
            [Category("Load")]
            [Description("Load an XML document from a file")]
            public void XLinq1()
            {
                XDocument doc = XDocument.Load("bib.xml");
                Console.WriteLine(doc);
            }

            [Category("Load")]
            [Description("Load document from string")]
            public void XLinq2()
            {
                string xml = "<book price='100' isbn='1002310'>" +
                                "<title>XClarity Samples</title>" +
                                "<author>Matt</author>" +
                             "</book>";
                XDocument doc = XDocument.Parse(xml);
                Console.WriteLine(doc);

            }

            //load an XML document from XmlReader
            [Category("Load")]
            [Description("Load an XML document from XmlReader")]
            public void XLinq3()
            {
                XmlReader reader = XmlReader.Create("bib.xml");
                XDocument doc = XDocument.Load(reader);
                Console.WriteLine(doc);

            }

            [Category("Load")]
            [Description("Construct XElement from XmlReader positioned on an element")]
            public void XLinq4()
            {
                XmlReader reader = XmlReader.Create("nw_customers.xml");
                reader.Read();//move to root
                reader.Read(); // move to fist customer
                XElement c = (XElement)XNode.ReadFrom(reader);
                Console.WriteLine(c);

            }

            [Category("Load")]
            [Description("Read XElement content from XmlReader")]
            public void XLinq5()
            {
                XmlReader reader = XmlReader.Create("config.xml");
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
        }
    }
}
