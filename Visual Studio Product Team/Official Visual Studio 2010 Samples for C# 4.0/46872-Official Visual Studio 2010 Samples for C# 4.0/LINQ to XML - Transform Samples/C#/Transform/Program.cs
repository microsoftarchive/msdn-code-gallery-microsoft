using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace Transform
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq77();  // Generate html with a list of customers that is numbered
            //samples.XLinq78();  // Generate a html tables of books by authors
        }

        private class LinqSamples
        {
            [Category("Transform")]
            [Description("Generate html with a list of customers that is numbered")]
            public void XLinq77()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");

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
            [Description("Generate a html tables of books by authors")]
            public void XLinq78()
            {
                XDocument doc = XDocument.Load("bib.xml");
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

            static XElement GetBooksTable(IEnumerable<XElement> books)
            {
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
        }
    }
}
