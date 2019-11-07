using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace Grouping
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq53();    // Group orders by customer
            //samples.XLinq54();    // Create a directory of customers grouped by country and city
            //samples.XLinq55();    // Group orders by customer and return all customers (+ orders) for customers who have more than 25 orders
        }

        private class LinqSamples
        {
            [Category("Grouping")]
            [Description("Group orders by customer")]
            public void XLinq53()
            {
                XDocument doc = XDocument.Load("nw_orders.xml");
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
            [Description("Create a directory of customers grouped by country and city")]
            public void XLinq54()
            {
                XDocument customers = XDocument.Load("nw_customers.xml");
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
            [Description("Group orders by customer and return all customers (+ orders) for customers who have more than 25 orders")]
            public void XLinq55()
            {
                XDocument customers = XDocument.Load("nw_customers.xml");
                XDocument orders = XDocument.Load("nw_orders.xml");
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
        }
    }
}
