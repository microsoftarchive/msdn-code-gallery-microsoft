//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using nwind;

namespace LinqToNorthwind {
    public static class Samples {

        public static void Sample1(Northwind db) 
        { 
            // use Where() to find only elements that match
            IEnumerable<Customer> q = db.Customers.Where(c => c.City == "London");
            ObjectDumper.Write(q, 0);
        }

        public static void Sample2(Northwind db)
        {
            // use First() in place of Where() to find the first or only one 
            Customer cust = db.Customers.First(c => c.CustomerID == "ALFKI");
            ObjectDumper.Write(cust, 0);
        }

        public static void Sample3(Northwind db) 
        {
            // Use Select() to map/project results
            var q = db.Customers.Select(c => c.ContactName);
            ObjectDumper.Write(q, 0);
        }

        public static void Sample4(Northwind db)
        {
            // use Anonymous Type constructors to retrieve only particular data
            var q = db.Customers.Select(c => new {c.ContactName, c.Phone});
            ObjectDumper.Write(q, 0);
        }

        public static void Sample5(Northwind db) 
        {
            // Combine Where() and Select() for common queries
            var q = db.Customers.Where(c => c.City == "London").Select(c => c.ContactName);
            ObjectDumper.Write(q, 0);
        }

        public static void Sample6(Northwind db) 
        { 
            // use SelectMany() to flatten collections
            IEnumerable<Order> q = db.Customers.SelectMany(c => c.Orders);
            ObjectDumper.Write(q, 0);
        }

        public static void Sample7(Northwind db) 
        {
            // use query expressions to simplify common select/where patterns
            var q = from c in db.Customers
                    from o in c.Orders
                    where c.City == "London"
                    select new {c.ContactName, o.OrderDate};
            ObjectDumper.Write(q, 0);
        }

        public static void Sample8(Northwind db) 
        {
            // use orderby to order results
            var q = from c in db.Customers orderby c.City, c.ContactName select c;
            ObjectDumper.Write(q, 0);
        }

        public static void Sample9(Northwind db) 
        {
            // use group x by y to produce a series of group partitions
            var q = from p in db.Products group p by p.CategoryID into Group select new {CategoryID=Group.Key, Group};
            ObjectDumper.Write(q, 1);
        }

        public static void Sample10(Northwind db) 
        {
            // use group-by and aggregates like Min()/Max() to compute values over group partitions
            var q = from p in db.Products
                    group p by p.CategoryID into g
                    select new {
                        Category = g.Key,
                        MinPrice = g.Min(p => p.UnitPrice),
                        MaxPrice = g.Max(p => p.UnitPrice)
                        };
            ObjectDumper.Write(q, 1);
        }

        public static void Sample11(Northwind db) 
        {
            // use Any() to determine if a collection has at least one element, or at least one element matches a condition
            var q = from c in db.Customers
                    where c.Orders.Any()
                    select c;
            ObjectDumper.Write(q, 0);
        }

        public static void Sample12(Northwind db) 
        {
            // use All() to determine if all elements of a collection match a condition (or that the collection is empty!)
            var q = from c in db.Customers
                    where c.Orders.All(o => o.ShipCity == c.City)
                    select c;
            ObjectDumper.Write(q, 0);
        }

        public static void Sample13(Northwind db) 
        {
            // use Take(n) to limit the sequence to only the first n elements
            var q = db.Customers.OrderBy(c => c.ContactName).Take(5);
            ObjectDumper.Write(q, 0);
        }

        public static void Sample14(Northwind db) 
        {
            // use SubmitChanges() to submit all changes back to the database
            Customer cust = db.Customers.First(c => c.CustomerID == "ALFKI");
            cust.ContactTitle = "Sith Lord";
            // other changes ...
            db.SubmitChanges();
        }
        
        public static void Sample15(Northwind db) 
        {
            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(from p in db.Products where p.ProductID == 4 select p);
            ObjectDumper.Write(from p in db.Products where p.ProductID == 5 select p);


            Console.WriteLine();
            Console.WriteLine("*** UPDATE WITH EXPLICIT TRANSACTION ***");
            // Explicit use of TransactionScope ensures that
            // the data will not change in the database between
            // read and write
            using (TransactionScope ts = new TransactionScope())
            {
                try
                {
                    Product prod1 = db.Products.First(p => p.ProductID == 4);
                    Product prod2 = db.Products.First(p => p.ProductID == 5);
                    prod1.UnitsInStock -= 3;
                    prod2.UnitsInStock -= 5;    // ERROR: this will make the units in stock negative
                    db.SubmitChanges();
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    Console.WriteLine(e.Message);
                }
            }


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");

            db = new Northwind(Program.connString);     // reset object cache to prove objects are reset

            ObjectDumper.Write(from p in db.Products where p.ProductID == 4 select p);
            ObjectDumper.Write(from p in db.Products where p.ProductID == 5 select p);
        }        
    }
}
