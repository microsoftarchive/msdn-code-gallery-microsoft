using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace XName
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq86();  // Create elements and attributes in a namespace
            //samples.XLinq87();  // Find xsd:element with name=FullAddress
            //samples.XLinq88();  // Create a namespace prefix declaration
            //samples.XLinq89();  // Get the local-name and namespace of an element
        }

        private class LinqSamples
        {
            [Category("XName")]
            [Description("Create elements and attributes in a namespace")]
            public void XLinq86()
            {
                XNamespace ns = "http://myNamespace";
                XElement result = new XElement(ns + "foo",
                                               new XAttribute(ns + "bar", "attribute"));
                Console.WriteLine(result);
            }

            [Category("XName")]
            [Description("Find xsd:element with name=FullAddress")]
            public void XLinq87()
            {
                XDocument doc = XDocument.Load("nw_customers.xsd");
                XNamespace XSD = "http://www.w3.org/2001/XMLSchema";
                XElement result = doc.Descendants(XSD + "element")
                                     .Where(e => (string)e.Attribute("name") == "FullAddress")
                                     .First();
                Console.WriteLine(result);
            }

            [Category("XName")]
            [Description("Create a namespace prefix declaration")]
            public void XLinq88()
            {
                XNamespace myNS = "http://myNamespace";
                XElement result = new XElement("myElement",
                                               new XAttribute(XNamespace.Xmlns + "myPrefix", myNS));
                Console.WriteLine(result);
            }

            [Category("XName")]
            [Description("Get the local-name and namespace of an element")]
            public void XLinq89()
            {
                XNamespace ns = "http://myNamespace";
                XElement e = new XElement(ns + "foo");

                Console.WriteLine("Local name of element: {0}", e.Name.LocalName);
                Console.WriteLine("Namespace of element : {0}", e.Name.Namespace.NamespaceName);

            }
        }
    }
}
