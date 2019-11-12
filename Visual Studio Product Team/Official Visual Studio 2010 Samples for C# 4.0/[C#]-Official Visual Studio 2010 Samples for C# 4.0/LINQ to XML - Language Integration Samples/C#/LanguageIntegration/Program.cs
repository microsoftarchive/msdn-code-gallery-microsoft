using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;
using System.Globalization;

namespace LanguageIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq79();  // Find all orders for customers in a List
            //samples.XLinq80();  // Find sum of items in a shopping cart
            //samples.XLinq81();  // Load and use a config file
            //samples.XLinq82();  // Convert a Sequence of nodes to Array
            //samples.XLinq83();  // Convert a Sequence of nodes to List
            //samples.XLinq84();  // Create a dictionary of customers
            //samples.XLinq85();  // Number all the countries and list them
        }

        private class LinqSamples
        {
            [Category("Language Integration")]
            [Description("Find all orders for customers in a List")]
            public void XLinq79()
            {
                XDocument doc = XDocument.Load("nw_orders.xml");
                List<Customer> customers = SomeMethodToGetCustomers();
                var result =
                    from customer in customers
                    from order in doc.Descendants("Orders")
                    where
                        customer.id == (string)order.Element("CustomerID")
                    select
                        new { custid = (string)customer.id, orderdate = (string)order.Element("OrderDate") };
                foreach (var tuple in result)
                    Console.WriteLine("Customer id = {0}, Order Date = {1}",
                                        tuple.custid, tuple.orderdate);

            }
            class Customer
            {
                public Customer(string id) { this.id = id; }
                public string id;
            }
            static List<Customer> SomeMethodToGetCustomers()
            {
                List<Customer> customers = new List<Customer>();
                customers.Add(new Customer("VINET"));
                customers.Add(new Customer("TOMSP"));
                return customers;
            }

            [Category("Language Integration")]
            [Description("Find sum of items in a shopping cart")]
            public void XLinq80()
            {
                XDocument doc = XDocument.Load("inventory.xml");
                List<Item> cart = GetShoppingCart();
                var subtotal =
                    from item in cart
                    from inventory in doc.Descendants("item")
                    where item.id == (string)inventory.Attribute("id")
                    select (double)item.quantity * (double)inventory.Element("price");
                Console.WriteLine("Total payment = {0}", subtotal.Sum());
            }
            class Item
            {
                public Item(string id, int quantity)
                {
                    this.id = id;
                    this.quantity = quantity;
                }
                public int quantity;
                public string id;
            }
            static List<Item> GetShoppingCart()
            {
                List<Item> items = new List<Item>();
                items.Add(new Item("1", 10));
                items.Add(new Item("5", 5));
                return items;
            }

            [Category("Language Integration")]
            [Description("Load and use a config file")]
            public void XLinq81()
            {
                XElement config = XDocument.Load("config.xml").Element("config");
                Initialize((string)config.Element("rootFolder"),
                           (int)config.Element("iterations"),
                           (double)config.Element("maxMemory"),
                           (string)config.Element("tempFolder"));

            }
            static void Initialize(string root, int iter, double mem, string temp)
            {
                Console.WriteLine("Application initialized to root folder: " +
                                  "{0}, iterations: {1}, max memory {2}, temp folder: {3}",
                                  root, iter, Convert.ToString(mem, CultureInfo.InvariantCulture), temp);
            }

            [Category("Language Integration")]
            [Description("Convert a Sequence of nodes to Array")]
            public void XLinq82()
            {
                XDocument doc = XDocument.Load("nw_Customers.xml");
                XElement[] custArray = doc.Descendants("Customers").ToArray();
                foreach (var c in custArray)
                    Console.WriteLine(c);
            }

            [Category("Language Integration")]
            [Description("Convert a Sequence of nodes to List")]
            public void XLinq83()
            {
                XDocument doc = XDocument.Load("nw_Customers.xml");
                List<XElement> clist = doc.Descendants("Customers").ToList();
                foreach (var c in clist)
                    Console.WriteLine(c);
            }

            [Category("Language Integration")]
            [Description("Create a dictionary of customers")]
            public void XLinq84()
            {
                XDocument doc = XDocument.Load("nw_Customers.xml");
                var dictionary = doc.Descendants("Customers")
                                    .ToDictionary(c => (string)c.Attribute("CustomerID"));
                Console.WriteLine(dictionary["ALFKI"]);
            }


            [Category("Language Integration")]
            [Description("Number all the countries and list them")]
            public void XLinq85()
            {
                XDocument doc = XDocument.Load("nw_Customers.xml");
                var countries = doc.Descendants("Country")
                                    .Select(c => (string)c)
                                    .Distinct()
                                    .Select((c, index) => new { i = index, name = c });
                foreach (var c in countries)
                    Console.WriteLine(c.i + " " + c.name);

            }
        }
    }
}
