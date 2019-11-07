using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace Sort
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq56();  // Sort customers by name in ascending order
            //samples.XLinq57();  // Sort orders by date in ascending order
            //samples.XLinq58();  // Sort customers by name in descending order
            //samples.XLinq59();  // Sort customers by country and city
        }

        private class LinqSamples
        {
            [Category("Sort")]
            [Description("Sort customers by name in ascending order")]
            public void XLinq56()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
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
            [Description("Sort orders by date in ascending order")]
            public void XLinq57()
            {
                XDocument doc = XDocument.Load("nw_orders.xml");
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
            [Description("Sort customers by name in descending order")]
            public void XLinq58()
            {
                XDocument doc = XDocument.Load("nw_Customers.xml");
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
            [Description("Sort customers by country and city")]
            public void XLinq59()
            {
                XDocument doc = XDocument.Load("nw_Customers.xml");
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
        }
    }
}
