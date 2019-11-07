using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace Query
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            // Comment or uncomment the method calls below to run or not
            samples.XLinq18();    // Select all the customers in the xml document
            //samples.XLinq19();    // Select all the child elements of the first customer
            //samples.XLinq20();    // Select the first customer in the document
            //samples.XLinq21();    // Query for one child element on a sequence of elements
            //samples.XLinq22();    // Selects all the CustomerIDs in the xml document
            //samples.XLinq23();    // Find orders with price > 100
            //samples.XLinq24();    // Get the root element of a document
            //samples.XLinq25();    // Filter query results using where
            //samples.XLinq26();    // Select all ContactName elements in the document
            //samples.XLinq27();    // Select all text in the document
            //samples.XLinq28();    // Check if two nodes belong to the same document
            //samples.XLinq29();    // Query for parent of an Element
            //samples.XLinq30();    // Add customer company info to orders of the first customer
            //samples.XLinq31();    // Query content of a given type of an existing element
            //samples.XLinq32();    // Query for all Swedish customer orders and Swedish orders whose freight is > 250
            //samples.XLinq33();    // Query the 3rd customer in the document
            //samples.XLinq34();    // Union books authored by two authors: Serge and Peter
            //samples.XLinq35();    // Intersect books that are common for both authors
            //samples.XLinq36();    // Find books that are authored by Peter and did not have Serge as co-author
            //samples.XLinq37();    // Display the path to a node
            //samples.XLinq38();    // Check if 2 sequences of nodes are equal. Did Serge and Peter co-author all of their the books?
            //samples.XLinq39();    // List books until total price is less that $150
            //samples.XLinq40();    // Create 5 new customers with different IDs
            //samples.XLinq41();    // Initialize new orders with items
            //samples.XLinq42();    // Check if there are any customers in Argentina
            //samples.XLinq43();    // Check if all books have at least one author
            //samples.XLinq44();    // Find the number of orders for a customer
            //samples.XLinq45();    // Find tax on an order
            //samples.XLinq46();    // Find all the countries where there is a customer
            //samples.XLinq47();    // List all books by Serge and Peter with co-authored books repeated
            //samples.XLinq48();    // Query the first two customers
            //samples.XLinq49();    // Skip the first 3 books
            //samples.XLinq50();    // Print items that dont fit in budget
            //samples.XLinq51();    // Get all books authored by Serge and Peter
            //samples.XLinq52();    // Find the container document of an element
        }

        private class LinqSamples
        {
            [Category("Query")]
            [Description("Select all the customers in the xml document")]
            public void XLinq18()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                foreach (XElement result in doc.Elements("Root")
                                               .Elements("Customers"))
                    Console.WriteLine(result);

            }

            [Category("Query")]
            [Description("Select all the child elements of the first customer")]
            public void XLinq19()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                var query = doc.Element("Root")
                               .Element("Customers")
                               .Elements();
                foreach (XElement result in query)
                    Console.WriteLine(result);
            }

            [Category("Query")]
            [Description("Select the first customer in the document")]
            public void XLinq20()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                var result = doc.Element("Root")
                                .Element("Customers");
                Console.WriteLine(result);
            }

            [Category("Query")]
            [Description("Query for one child element on a sequence of elements")]
            public void XLinq21()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                var result = doc.Elements()
                                .Elements("Customers")
                                .First()
                                .Element("CompanyName");
                Console.WriteLine(result);
            }

            [Category("Query")]
            [Description("Selects all the CustomerIDs in the xml document")]
            public void XLinq22()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                var query = doc.Element("Root")
                               .Elements("Customers")
                               .Attributes("CustomerID");
                foreach (XAttribute result in query)
                    Console.WriteLine(result.Name + " = " + result.Value);

            }

            [Category("Query")]
            [Description("Find orders with price > 100")]
            public void XLinq23()
            {
                string xml = "<order >" +
                                "<item price='150'>Motor</item>" +
                                "<item price='50'>Cable</item>" +
                                "<item price='50'>Modem</item>" +
                                "<item price='250'>Monitor</item>" +
                                "<item price='10'>Mouse</item>" +
                            "</order>";
                XElement order = XElement.Parse(xml);
                var query =
                    from
                        i in order.Elements("item")
                    where
                        (int)i.Attribute("price") > 100
                    select i;
                foreach (var result in query)
                    Console.WriteLine("Expensive Item {0} costs {1}",
                                      (string)result,
                                      (string)result.Attribute("price"));
            }

            [Category("Query")]
            [Description("Get the root element of a document")]
            public void XLinq24()
            {
                XElement root = XDocument.Load("config.xml")
                                         .Root;
                Console.WriteLine("Name of root element is {0}", root.Name);
            }

            [Category("Query")]
            [Description("Filter query results using where")]
            public void XLinq25()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                var query =
                        from
                            c in doc.Element("Root")
                                    .Elements("Customers")
                        where
                            c.Element("FullAddress")
                             .Element("Country")
                             .Value == "Germany"
                        select c;

                foreach (XElement result in query)
                    Console.WriteLine(result);
            }

            [Category("Query")]
            [Description("Select all ContactName elements in the document")]
            public void XLinq26()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                var query = doc.Descendants("ContactName");
                foreach (XElement result in query)
                    Console.WriteLine(result);
            }

            [Category("Query")]
            [Description("Select all text in the document")]
            public void XLinq27()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                var query = doc.DescendantNodes().OfType<XText>().Select(t => t.Value);
                foreach (string result in query)
                    Console.WriteLine(result);
            }

            [Category("Query")]
            [Description("Check if two nodes belong to the same document")]
            public void XLinq28()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                XElement element1 = doc.Element("Root");
                XElement element2 = doc.Descendants("Customers")
                                    .ElementAt(3);
                var query = from a in element1.AncestorsAndSelf()
                            from b in element2.AncestorsAndSelf()
                            where a == b
                            select a;
                Console.WriteLine(query.Any());
            }

            [Category("Query")]
            [Description("Query for parent of an Element")]
            public void XLinq29()
            {
                XElement item = new XElement("item-01",
                                             "Computer");
                XElement order = new XElement("order", item);
                XElement p = item.Parent;
                Console.WriteLine(p.Name);
            }

            [Category("Query")]
            [Description("Add customer company info to orders of the first customer")]
            public void XLinq30()
            {
                XDocument customers = XDocument.Load("nw_customers.xml");
                XDocument orders = XDocument.Load("nw_orders.xml");

                var query =
                    from customer in customers.Descendants("Customers").Take(1)
                    join order in orders.Descendants("Orders")
                               on (string)customer.Attribute("CustomerID") equals
                                  (string)order.Element("CustomerID")
                    select
                        new XElement("Order",
                                    order.Nodes(),
                                    customer.Element("CompanyName"));

                foreach (var result in query)
                    Console.WriteLine(result);

            }

            [Category("Query")]
            [Description("Query content of a given type of an existing element")]
            public void XLinq31()
            {
                XElement elem =
                  new XElement("customer",
                               new XElement("name",
                                            "jack"),
                               "some text",
                               new XComment("new customer"),
                               new XAttribute("id",
                                              "abc"));

                //string content
                foreach (XText s in elem.Nodes().OfType<XText>())
                    Console.WriteLine("String content: {0}", s);

                //element content
                foreach (XElement e in elem.Elements())
                    Console.WriteLine("Element content: {0}", e);

                //comment content
                foreach (XComment c in elem.Nodes().OfType<XComment>())
                    Console.WriteLine("Comment content: {0}", c);

            }

            [Category("Query")]
            [Description("Query for all Swedish customer orders and Swedish orders whose freight is > 250")]
            public void XLinq32()
            {
                XDocument customers = XDocument.Load("nw_customers.xml");
                XDocument orders = XDocument.Load("nw_orders.xml");
                XStreamingElement summary = new XStreamingElement("Summary",
                    new XAttribute("Country", "Sweden"),
                    new XStreamingElement("SwedishCustomerOrders", GetSwedishCustomerOrders(customers, orders)),
                    new XStreamingElement("Orders", GetSwedishFreightProfile(orders)));

                Console.WriteLine(summary);
                //DML operation, which adds a new order for customer BERGS freight > 250  
                AddNewOrder(orders);
                Console.WriteLine("****XStreaming Output after DML reflects new order added!!****");
                Console.WriteLine(summary);
            }

            static void AddNewOrder(XDocument orders)
            {
                string order = @"<Orders>
                                   <CustomerID>BERGS</CustomerID>
                                   <ShipInfo ShippedDate='1997-12-09T00:00:00'>
                                   <Freight>301</Freight>
                                   <ShipCountry>Sweden</ShipCountry>
                                   </ShipInfo>
                                   </Orders>";
                XElement newOrder = XElement.Parse(order);
                orders.Root.Add(newOrder);
            }

            private static IEnumerable<XElement> GetSwedishFreightProfile(XDocument orders)
            {
                return
                   from
                    order in orders.Descendants("Orders")
                   where
                    (string)order.Element("ShipInfo").Element("ShipCountry") == "Sweden"
                      && (float)order.Element("ShipInfo").Element("Freight") > 250
                   select
                      new XElement("Order",
                      new XAttribute("Freight",
                                     (string)order.Element("ShipInfo").Element("Freight")));

            }

            private static IEnumerable<XElement> GetSwedishCustomerOrders(XDocument customers, XDocument orders)
            {
                return
                   from
                    customer in customers.Descendants("Customers")
                   where
                    (string)customer.Element("FullAddress").Element("Country") == "Sweden"
                   select
                     new XElement("Customer",
                     new XAttribute("Name",
                                    (string)customer.Element("CompanyName")),
                     new XAttribute("OrderCount",
                                    (from
                                      order in orders.Descendants("Orders")
                                     where
                                       (string)order.Element("CustomerID") == (string)customer.Attribute("CustomerID")
                                     select
                                       order).Count()));

            }

            [Category("Query")]
            [Description("Query the 3rd customer in the document")]
            public void XLinq33()
            {
                XDocument doc = XDocument.Load("nw_customers.xml");
                var c = doc.Descendants("Customers")
                           .ElementAt(2);
                Console.WriteLine(c);
            }

            [Category("Query")]
            [Description("Union books authored by two authors: Serge and Peter")]
            public void XLinq34()
            {
                XDocument doc = XDocument.Load("bib.xml");
                var b1 = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                         .Elements("first")
                                         .Any(f => (string)f == "Serge"));
                var b2 = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                         .Elements("first")
                                         .Any(f => (string)f == "Peter"));
                var books = b1.Union(b2);
                foreach (var b in books)
                    Console.WriteLine(b);
            }

            [Category("Query")]
            [Description("Intersect books that are common for both authors")]
            public void XLinq35()
            {
                XDocument doc = XDocument.Load("bib.xml");
                var b1 = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                            .Elements("first")
                                            .Any(f => (string)f == "Serge"));
                var b2 = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                            .Elements("first")
                                            .Any(f => (string)f == "Peter"));
                var books = b1.Intersect(b2);
                foreach (var b in books)
                    Console.WriteLine(b);
            }

            [Category("Query")]
            [Description("Find books that are authored by Peter and did not have Serge as co-author")]
            public void XLinq36()
            {
                XDocument doc = XDocument.Load("bib.xml");
                var b1 = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                         .Elements("first")
                                         .Any(f => (string)f == "Serge"));
                var b2 = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                         .Elements("first")
                                         .Any(f => (string)f == "Peter"));
                var books = b2.Except(b1);
                foreach (var b in books)
                    Console.WriteLine(b);
            }

            [Category("Query")]
            [Description("Display the path to a node")]
            public void XLinq37()
            {
                XDocument doc = XDocument.Load("bib.xml");
                XElement e = doc.Descendants("last")
                                .First();
                PrintPath(e);
            }

            static void PrintPath(XElement e)
            {
                var nodes = e.AncestorsAndSelf()
                                .Reverse();
                foreach (var n in nodes)
                    Console.Write(n.Name + (n == e ? "" : "->"));
            }


            [Category("Query")]
            [Description("Check if 2 sequences of nodes are equal. " +
                         "Did Serge and Peter co-author all of their the books?")]
            public void XLinq38()
            {
                XDocument doc = XDocument.Load("bib.xml");
                var b1 = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                            .Elements("first")
                                            .Any(f => (string)f == "Serge"));
                var b2 = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                            .Elements("first")
                                            .Any(f => (string)f == "Peter"));
                bool result = b2.SequenceEqual(b1);
                Console.WriteLine(result);
            }


            [Category("Query")]
            [Description("List books until total price is less that $150")]
            public void XLinq39()
            {
                XDocument doc = XDocument.Load("bib.xml");
                double sum = 0;
                var query = doc.Descendants("book")
                               .TakeWhile(c => (sum += (double)c.Element("price")) <= 150);
                foreach (var result in query)
                    Console.WriteLine(result);
            }

            [Category("Query")]
            [Description("Create 5 new customers with different IDs")]
            public void XLinq40()
            {
                var query = from
                               i in Enumerable.Range(1, 5)
                            select
                                new XElement("Customer",
                                             new XAttribute("id", i),
                                             "New customer");
                foreach (var result in query)
                    Console.WriteLine(result);
            }

            [Category("Query")]
            [Description("Initialize new orders with items")]
            public void XLinq41()
            {
                var orders = new XElement[] {
                new XElement("order", new XAttribute("itemCount",5)),
                new XElement("order", new XAttribute("itemCount",2)),
                new XElement("order", new XAttribute("itemCount",3))};

                //add empty items
                foreach (var order in orders)
                    order.Add(Enumerable.Repeat(new XElement("item", "New item"),
                                              (int)order.Attribute("itemCount")));

                foreach (var o in orders)
                    Console.WriteLine(o);

            }

            [Category("Query")]
            [Description("Check if there are any customers in Argentina")]
            public void XLinq42()
            {
                XDocument doc = XDocument.Load("nw_Customers.xml");
                if (doc.Descendants("Country").Any(c => (string)c == "Argentina"))
                    Console.WriteLine("There are cusotmers in Argentina");
                else
                    Console.WriteLine("There are no cusotmers in Argentina");
            }

            [Category("Query")]
            [Description("Check if all books have at least one author")]
            public void XLinq43()
            {
                XDocument doc = XDocument.Load("bib.xml");
                bool query = doc.Descendants("book")
                                .All(b => b.Descendants("author").Count() > 0);
                if (query)
                    Console.WriteLine("All books have authors");
                else
                    Console.WriteLine("Some books dont have authors");
            }

            [Category("Query")]
            [Description("Find the number of orders for a customer")]
            public void XLinq44()
            {
                XDocument doc = XDocument.Load("nw_Orders.xml");
                var query = doc.Descendants("Orders")
                               .Where(o => (string)o.Element("CustomerID") == "VINET");
                Console.WriteLine("Customer has {0} orders", query.Count());
            }

            [Category("Query")]
            [Description("Find tax on an order")]
            public void XLinq45()
            {
                string xml = "<order >" +
                                "<item price='150'>Motor</item>" +
                                "<item price='50'>Cable</item>" +
                             "</order>";

                XElement order = XElement.Parse(xml);
                double tax = order.Elements("item")
                                  .Aggregate((double)0, Tax);

                Console.WriteLine("The total tax on the order @10% is ${0}", tax);

            }
            static double Tax(double seed, XElement item)
            {
                return seed + (double)item.Attribute("price") * 0.1;
            }

            [Category("Query")]
            [Description("Find all the countries where there is a customer")]
            public void XLinq46()
            {
                XDocument doc = XDocument.Load("nw_Customers.xml");
                var countries = doc.Descendants("Country")
                                   .Select(c => (string)c)
                                   .Distinct()
                                   .OrderBy(c => c);
                foreach (var c in countries)
                    Console.WriteLine(c);
            }

            [Category("Query")]
            [Description("List all books by Serge and Peter with co-authored books repeated")]
            public void XLinq47()
            {
                XDocument doc = XDocument.Load("bib.xml");
                var b1 = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                         .Elements("first")
                                         .Any(f => (string)f == "Serge"));
                var b2 = doc.Descendants("book")
                            .Where(b => b.Elements("author")
                                         .Elements("first")
                                         .Any(f => (string)f == "Peter"));
                var books = b1.Concat(b2);
                foreach (var b in books)
                    Console.WriteLine(b);

            }
            [Category("Query")]
            [Description("Query the first two customers")]
            public void XLinq48()
            {
                XDocument doc = XDocument.Load("nw_Customers.xml");
                var customers = doc.Descendants("Customers").Take(2);
                foreach (var c in customers)
                    Console.WriteLine(c);
            }

            [Category("Query")]
            [Description("Skip the first 3 books")]
            public void XLinq49()
            {
                XDocument doc = XDocument.Load("bib.xml");
                var books = doc.Descendants("book").Skip(3);
                foreach (var b in books)
                    Console.WriteLine(b);
            }

            [Category("Query")]
            [Description("Print items that dont fit in budget")]
            public void XLinq50()
            {
                string xml = "<order >" +
                                "<item price='150'>Motor</item>" +
                                "<item price='50'>Cable</item>" +
                                "<item price='50'>Modem</item>" +
                                "<item price='250'>Monitor</item>" +
                                "<item price='10'>Mouse</item>" +
                            "</order>";
                XElement order = XElement.Parse(xml);
                int sum = 0;
                var items = order.Descendants("item")
                                 .SkipWhile(i => (sum += (int)i.Attribute("price")) < 300);
                foreach (var i in items)
                    Console.WriteLine("{0} does not fit in you budget", (string)i);
            }

            [Category("Query")]
            [Description("Get all books authored by Serge and Peter")]
            public void XLinq51()
            {
                string[] authors = { "Serge", "Peter" };
                var books = authors.SelectMany(a => GetBooks(a));
                foreach (var b in books)
                    Console.WriteLine(b);

            }

            public IEnumerable<XElement> GetBooks(string author)
            {
                XDocument doc = XDocument.Load("bib.xml");
                var query = doc.Descendants("book")
                                .Where(b => b.Elements("author")
                                             .Elements("first")
                                             .Any(f => (string)f == author));

                return query;
            }

            [Category("Query")]
            [Description("Find the container document of an element")]
            public void XLinq52()
            {
                XElement c = XDocument.Load("bib.xml").Descendants("book").First();
                XDocument container = c.Document;
                Console.WriteLine(container);
            }
        }
    }
}
