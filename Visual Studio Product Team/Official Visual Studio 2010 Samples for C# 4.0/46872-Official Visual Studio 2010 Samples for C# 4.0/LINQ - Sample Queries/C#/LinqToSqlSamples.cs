// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using nwind;
using SampleSupport;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Transactions;
using System.Linq.Expressions;
using System.Data.Linq.Provider;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.Data.Linq.SqlClient;
using System.Xml.Linq;

namespace SampleQueries {
    [Title("101+ Linq To Sql Query Samples")]
    [Prefix("LinqToSql")]
    public class LinqToSqlSamples : SampleHarness {
        
        private readonly static string dbPath = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\Data\NORTHWND.MDF"));
        private readonly static string sqlServerInstance = @".\SQLEXPRESS";
        private readonly static string connString = "AttachDBFileName='" + dbPath + "';Server='" + sqlServerInstance + "';user instance=true;Integrated Security=SSPI;Connection Timeout=60";

        private Northwind db;

        [Category("Where")]
        [Title("Where - 1")]
        [Description("This sample uses WHERE to filter for Customers in London.")]
        public void LinqToSqlWhere01() {
            var q =
                from c in db.Customers
                where c.City == "London"
                select c;
            ObjectDumper.Write(q);
        }

        [Category("Where")]
        [Title("Where - 2")]
        [Description("This sample uses WHERE to filter for Employees hired " +
                     "during or after 1994.")]
        public void LinqToSqlWhere02() {
            var q =
                from e in db.Employees
                where e.HireDate >= new DateTime(1994, 1, 1)
                select e;

            ObjectDumper.Write(q);
        }

        [Category("Where")]
        [Title("Where - 3")]
        [Description("This sample uses WHERE to filter for Products that have stock below their " +
                     "reorder level and are not discontinued.")]
        public void LinqToSqlWhere03() {
            var q =
                from p in db.Products
                where p.UnitsInStock <= p.ReorderLevel && !p.Discontinued
                select p;

            ObjectDumper.Write(q);
        }

        [Category("Where")]
        [Title("Where - 4")]
        [Description("This sample uses WHERE to filter out Products that are either " +
                     "UnitPrice is greater than 10 or is discontinued.")]
        public void LinqToSqlWhere04() {
            var q =
                from p in db.Products
                where p.UnitPrice > 10m || p.Discontinued
                select p;

            ObjectDumper.Write(q, 0);
        }

        [Category("Where")]
        [Title("Where - 5")]
        [Description("This sample calls WHERE twice to filter out Products that UnitPrice is greater than 10" +
                     " and is discontinued.")]
        public void LinqToSqlWhere05() {
            var q =
                db.Products.Where(p=>p.UnitPrice > 10m).Where(p=>p.Discontinued);

            ObjectDumper.Write(q, 0);
        }

        [Category("Where")]
        [Title("First - Simple")]
        [Description("This sample uses First to select the first Shipper in the table.")]
        public void LinqToSqlWhere06() {
            Shipper shipper = db.Shippers.First();
            ObjectDumper.Write(shipper, 0);
        }

        [Category("Where")]
        [Title("First - Element")]
        [Description("This sample uses First to select the single Customer with CustomerID 'BONAP'.")]
        public void LinqToSqlWhere07() {
            Customer cust = db.Customers.First(c => c.CustomerID == "BONAP");
            ObjectDumper.Write(cust, 0);
        }

        [Category("Where")]
        [Title("First - Condition")]
        [Description("This sample uses First to select an Order with freight greater than 10.00.")]
        public void LinqToSqlWhere08() {
            Order ord = db.Orders.First(o => o.Freight > 10.00M);
            ObjectDumper.Write(ord, 0);
        }

        [Category("Select/Distinct")]
        [Title("Select - Simple")]
        [Description("This sample uses SELECT to return a sequence of just the " +
                     "Customers' contact names.")]
        public void LinqToSqlSelect01() {
            var q =
                from c in db.Customers
                select c.ContactName;

            ObjectDumper.Write(q);
        }

        [Category("Select/Distinct")]
        [Title("Select - Anonymous Type 1")]
        [Description("This sample uses SELECT and anonymous types to return " +
                     "a sequence of just the Customers' contact names and phone numbers.")]
        public void LinqToSqlSelect02() {
            var q =
                from c in db.Customers
                select new {c.ContactName, c.Phone};

            ObjectDumper.Write(q);
        }

        [Category("Select/Distinct")]
        [Title("Select - Anonymous Type 2")]
        [Description("This sample uses SELECT and anonymous types to return " +
                     "a sequence of just the Employees' names and phone numbers, " +
                     "with the FirstName and LastName fields combined into a single field, 'Name', " +
                     "and the HomePhone field renamed to Phone in the resulting sequence.")]
        public void LinqToSqlSelect03() {
            var q =
                from e in db.Employees
                select new {Name = e.FirstName + " " + e.LastName, Phone = e.HomePhone};

            ObjectDumper.Write(q, 1);
        }

        [Category("Select/Distinct")]
        [Title("Select - Anonymous Type 3")]
        [Description("This sample uses SELECT and anonymous types to return " +
                     "a sequence of all Products' IDs and a calculated value " +
                     "called HalfPrice which is set to the Product's UnitPrice " +
                     "divided by 2.")]
        public void LinqToSqlSelect04() {
            var q =
                from p in db.Products
                select new {p.ProductID, HalfPrice = p.UnitPrice / 2};
            ObjectDumper.Write(q, 1);
        }

        [Category("Select/Distinct")]
        [Title("Select - Conditional ")]
        [Description("This sample uses SELECT and a conditional statement to return a sequence of product " +
                     " name and product availability.")]
        public void LinqToSqlSelect05() {
            var q =
                from p in db.Products
                select new {p.ProductName, Availability = p.UnitsInStock - p.UnitsOnOrder < 0 ? "Out Of Stock": "In Stock"};

            ObjectDumper.Write(q, 1);
        }

        [Category("Select/Distinct")]
        [Title("Select - Named Type")]
        [Description("This sample uses SELECT and a known type to return a sequence of employees' names.")]
        public void LinqToSqlSelect06() {
            var q =
                from e in db.Employees                
                select new Name {FirstName = e.FirstName, LastName = e.LastName};

            ObjectDumper.Write(q, 1);
        }

        [Category("Select/Distinct")]
        [Title("Select - Filtered")]
        [Description("This sample uses SELECT and WHERE to return a sequence of " +
                     "just the London Customers' contact names.")]
        public void LinqToSqlSelect07() {
            var q =
                from c in db.Customers
                where c.City == "London"
                select c.ContactName;

            ObjectDumper.Write(q);
        }

        [Category("Select/Distinct")]
        [Title("Select - Shaped")]
        [Description("This sample uses SELECT and anonymous types to return " +
                     "a shaped subset of the data about Customers.")]
        public void LinqToSqlSelect08() {
            var q =
                from c in db.Customers
                select new {
                    c.CustomerID,
                    CompanyInfo = new {c.CompanyName, c.City, c.Country},
                    ContactInfo = new {c.ContactName, c.ContactTitle}
                };

            ObjectDumper.Write(q, 1);
        }

        [Category("Select/Distinct")]
        [Title("Select - Nested ")]
        [Description("This sample uses nested queries to return a sequence of " +
                     "all orders containing their OrderID, a subsequence of the " +
                     "items in the order where there is a discount, and the money " +
                     "saved if shipping is not included.")]
        public void LinqToSqlSelect09() {
            var q =
                from o in db.Orders
                select new {
                    o.OrderID,
                    DiscountedProducts =
                        from od in o.OrderDetails
                        where od.Discount > 0.0
                        select od,
                    FreeShippingDiscount = o.Freight
                };

            ObjectDumper.Write(q, 1);
        }

        // Phone converter that converts a phone number to 
        // an international format based on its country.
        // This sample only supports USA and UK formats, for 
        // phone numbers from the Northwind database.
        public string PhoneNumberConverter(string Country, string Phone)
        {
            Phone = Phone.Replace(" ", "").Replace(")", ")-");
            switch (Country)
            {
                case "USA":
                    return "1-" + Phone;
                case "UK":
                    return "44-" + Phone;
                default:
                    return Phone;
            }
        }

        [Category("Select/Distinct")]
        [Title("Select - Local Method Call 1")]
        [Description("This sample uses a Local Method Call to " + 
                     "'PhoneNumberConverter' to convert Phone number " + 
                     "to an international format.")]
        public void LinqToSqlLocalMethodCall01()
        {
            var q = from c in db.Customers
                    where c.Country == "UK" || c.Country == "USA"
                    select new { c.CustomerID, c.CompanyName, Phone = c.Phone, InternationalPhone = PhoneNumberConverter(c.Country, c.Phone) };

            ObjectDumper.Write(q);
        }

        [Category("Select/Distinct")]
        [Title("Select - Local Method Call 2")]
        [Description("This sample uses a Local Method Call to " + 
                     "convert phone numbers to an international format " + 
                     "and create XDocument.")]
        public void LinqToSqlLocalMethodCall02()
        {
            XDocument doc = new XDocument(
                new XElement("Customers", from c in db.Customers
                                          where c.Country == "UK" || c.Country == "USA"
                                          select (new XElement("Customer",
                                              new XAttribute("CustomerID", c.CustomerID),
                                              new XAttribute("CompanyName", c.CompanyName),
                                              new XAttribute("InterationalPhone", PhoneNumberConverter(c.Country, c.Phone))
                                              ))));

            Console.WriteLine(doc.ToString());
        }


        [Category("Select/Distinct")]
        [Title("Distinct")]
        [Description("This sample uses Distinct to select a sequence of the unique cities " +
                     "that have Customers.")]
        public void LinqToSqlSelect10() {
            var q = (
                from c in db.Customers
                select c.City )
                .Distinct();

            ObjectDumper.Write(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Count - Simple")]
        [Description("This sample uses Count to find the number of Customers in the database.")]
        public void LinqToSqlCount01() {
            var q = db.Customers.Count();
            Console.WriteLine(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Count - Conditional")]
        [Description("This sample uses Count to find the number of Products in the database " +
                     "that are not discontinued.")]
        public void LinqToSqlCount02() {
            var q = db.Products.Count(p => !p.Discontinued);
            Console.WriteLine(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Sum - Simple")]
        [Description("This sample uses Sum to find the total freight over all Orders.")]
        public void LinqToSqlCount03() {
            var q = db.Orders.Select(o => o.Freight).Sum();
            Console.WriteLine(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Sum - Mapped")]
        [Description("This sample uses Sum to find the total number of units on order over all Products.")]
        public void LinqToSqlCount04() {
            var q = db.Products.Sum(p => p.UnitsOnOrder);
            Console.WriteLine(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Min - Simple")]
        [Description("This sample uses Min to find the lowest unit price of any Product.")]
        public void LinqToSqlCount05() {
            var q = db.Products.Select(p => p.UnitPrice).Min();
            Console.WriteLine(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Min - Mapped")]
        [Description("This sample uses Min to find the lowest freight of any Order.")]
        public void LinqToSqlCount06() {
            var q = db.Orders.Min(o => o.Freight);
            Console.WriteLine(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Min - Elements")]
        [Description("This sample uses Min to find the Products that have the lowest unit price " +
                     "in each category.")]
        public void LinqToSqlCount07() {
            var categories =
                from p in db.Products
                group p by p.CategoryID into g
                select new {
                    CategoryID = g.Key,
                    CheapestProducts =
                        from p2 in g
                        where p2.UnitPrice == g.Min(p3 => p3.UnitPrice)
                        select p2
                };

            ObjectDumper.Write(categories, 1);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Max - Simple")]
        [Description("This sample uses Max to find the latest hire date of any Employee.")]
        public void LinqToSqlCount08() {
            var q = db.Employees.Select(e => e.HireDate).Max();
            Console.WriteLine(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Max - Mapped")]
        [Description("This sample uses Max to find the most units in stock of any Product.")]
        public void LinqToSqlCount09() {
            var q = db.Products.Max(p => p.UnitsInStock);
            Console.WriteLine(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Max - Elements")]
        [Description("This sample uses Max to find the Products that have the highest unit price " +
                     "in each category.")]
        public void LinqToSqlCount10() {
            var categories =
                from p in db.Products
                group p by p.CategoryID into g
                select new {
                    g.Key,
                    MostExpensiveProducts =
                        from p2 in g
                        where p2.UnitPrice == g.Max(p3 => p3.UnitPrice)
                        select p2
                };

            ObjectDumper.Write(categories, 1);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Average - Simple")]
        [Description("This sample uses Average to find the average freight of all Orders.")]
        public void LinqToSqlCount11() {
            var q = db.Orders.Select(o => o.Freight).Average();
            Console.WriteLine(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Average - Mapped")]
        [Description("This sample uses Average to find the average unit price of all Products.")]
        public void LinqToSqlCount12() {
            var q = db.Products.Average(p => p.UnitPrice);
            Console.WriteLine(q);
        }

        [Category("Count/Sum/Min/Max/Avg")]
        [Title("Average - Elements")]
        [Description("This sample uses Average to find the Products that have unit price higher than " +
                     "the average unit price of the category for each category.")]
        public void LinqToSqlCount13() {
            var categories =
                from p in db.Products
                group p by p.CategoryID into g
                select new {
                    g.Key, 
                    ExpensiveProducts =
                        from p2 in g
                        where p2.UnitPrice > g.Average(p3 => p3.UnitPrice)
                        select p2
                };

            ObjectDumper.Write(categories, 1);
        }

        [Category("Join")]
        [Title("SelectMany - 1 to Many - 1")]
        [Description("This sample uses foreign key navigation in the second " +
                     "from clause to produce a flat sequence of all orders for customers in London.")]
        public void LinqToSqlJoin01() {
            var q =
                from c in db.Customers
                where c.City == "London"
                from o in c.Orders
                select o;

            ObjectDumper.Write(q);
        }

        [Category("Join")]
        [Title("SelectMany - 1 to Many - 2")]
        [Description("This sample uses foreign key navigation in the " +
                     "where clause to filter for Products whose Supplier is in the USA " +
                     "that are out of stock.")]
        public void LinqToSqlJoin02() {
            var q =
                from p in db.Products
                where p.Supplier.Country == "USA" && p.UnitsInStock == 0
                select p;

            ObjectDumper.Write(q);
        }

        [Category("Join")]
        [Title("SelectMany - Many to Many")]
        [Description("This sample uses foreign key navigation in the " +
                     "from clause to filter for employees in Seattle, " +
                     "and also list their territories.")]
        public void LinqToSqlJoin03() {
            var q =
                from e in db.Employees
                from et in e.EmployeeTerritories
                where e.City == "Seattle"
                select new {e.FirstName, e.LastName, et.Territory.TerritoryDescription};

            ObjectDumper.Write(q);
        }

        [Category("Join")]
        [Title("SelectMany - Self-Join")]
        [Description("This sample uses foreign key navigation in the " +
                     "select clause to filter for pairs of employees where " +
                     "one employee reports to the other and where " +
                     "both employees are from the same City.")]
        public void LinqToSqlJoin04() {
            var q =
                from e1 in db.Employees
                from e2 in e1.Employees
                where e1.City == e2.City
                select new {
                    FirstName1 = e1.FirstName, LastName1 = e1.LastName,
                    FirstName2 = e2.FirstName, LastName2 = e2.LastName,
                    e1.City
                };

            ObjectDumper.Write(q);
        }

        [Category("Join")]
        [Title("GroupJoin - Two way join")]
        [Description("This sample explicitly joins two tables and projects results from both tables.")]
        public void LinqToSqlJoin05() {
            var q =
                from c in db.Customers
                join o in db.Orders on c.CustomerID equals o.CustomerID into orders
                select new {c.ContactName, OrderCount = orders.Count()};

            ObjectDumper.Write(q);
        }

        [Category("Join")]
        [Title("GroupJoin - Three way join")]
        [Description("This sample explicitly joins three tables and projects results from each of them.")]
        public void LinqToSqlJoin06() {
            var q =
                from c in db.Customers
                join o in db.Orders on c.CustomerID equals o.CustomerID into ords
                join e in db.Employees on c.City equals e.City into emps
                select new {c.ContactName, ords=ords.Count(), emps=emps.Count()};

            ObjectDumper.Write(q);
        }

        [Category("Join")]
        [Title("GroupJoin - Left Outer Join")] 
        [Description("This sample shows how to get LEFT OUTER JOIN by using DefaultIfEmpty(). The DefaultIfEmpty() method returns null when there is no Order for the Employee." )]
        public void LinqToSqlJoin07() {
            var q =
                from e in db.Employees
                join o in db.Orders on e equals o.Employee into ords
                from o in ords.DefaultIfEmpty()
                select new {e.FirstName, e.LastName, Order = o};

            ObjectDumper.Write(q);
        }

        [Category("Join")]
        [Title("GroupJoin - Projected let assignment")]
        [Description("This sample projects a 'let' expression resulting from a join.")]
        public void LinqToSqlJoin08() {
            var q = 
                from c in db.Customers
                join o in db.Orders on c.CustomerID equals o.CustomerID into ords
                let z = c.City + c.Country
                from o in ords                  
                select new {c.ContactName, o.OrderID, z};

            ObjectDumper.Write(q);
        }

        [Category("Join")]
        [Title("GroupJoin - Composite Key")]
        [Description("This sample shows a join with a composite key.")]
        public void LinqToSqlJoin09() {
            var q =
                from o in db.Orders
                from p in db.Products
                join d in db.OrderDetails 
                    on new {o.OrderID, p.ProductID} equals new {d.OrderID, d.ProductID}
                    into details
                from d in details
                select new {o.OrderID, p.ProductID, d.UnitPrice};

            ObjectDumper.Write(q);
        }

        [Category("Join")]
        [Title("GroupJoin - Nullable\\Nonnullable Key Relationship")]
        [Description("This sample shows how to construct a join where one side is nullable and the other is not.")]
        public void LinqToSqlJoin10() {
            var q =
                from o in db.Orders
                join e in db.Employees 
                    on o.EmployeeID equals (int?)e.EmployeeID into emps
                from e in emps
                select new {o.OrderID, e.FirstName};

            ObjectDumper.Write(q);
        }

        [Category("Order By")]
        [Title("OrderBy - Simple")]
        [Description("This sample uses orderby to sort Employees " +
                     "by hire date.")]
        public void LinqToSqlOrderBy01() {
            var q =
                from e in db.Employees
                orderby e.HireDate
                select e;

            ObjectDumper.Write(q);
        }

        [Category("Order By")]
        [Title("OrderBy - With Where")]
        [Description("This sample uses where and orderby to sort Orders " +
                     "shipped to London by freight.")]
        public void LinqToSqlOrderBy02() {
            var q =
                from o in db.Orders
                where o.ShipCity == "London"
                orderby o.Freight
                select o;

            ObjectDumper.Write(q);
        }

        [Category("Order By")]
        [Title("OrderByDescending")]
        [Description("This sample uses orderby to sort Products " +
                     "by unit price from highest to lowest.")]
        public void LinqToSqlOrderBy03() {
            var q =
                from p in db.Products
                orderby p.UnitPrice descending
                select p;

            ObjectDumper.Write(q);
        }

        [Category("Order By")]
        [Title("ThenBy")]
        [Description("This sample uses a compound orderby to sort Customers " +
                     "by city and then contact name.")]
        public void LinqToSqlOrderBy04() {
            var q =
                from c in db.Customers
                orderby c.City, c.ContactName
                select c;

            ObjectDumper.Write(q);
        }

        [Category("Order By")]
        [Title("ThenByDescending")]
        [Description("This sample uses orderby to sort Orders from EmployeeID 1 " +
                     "by ship-to country, and then by freight from highest to lowest.")]
        public void LinqToSqlOrderBy05() {
            var q =
                from o in db.Orders
                where o.EmployeeID == 1
                orderby o.ShipCountry, o.Freight descending
                select o;

            ObjectDumper.Write(q);
        }


        [Category("Order By")]
        [Title("OrderBy - Group By")]
        [Description("This sample uses orderby, Max and Group By to find the Products that have " +
                     "the highest unit price in each category, and sorts the group by category id.")]
        public void LinqToSqlOrderBy06() {
            var categories =
                from p in db.Products
                group p by p.CategoryID into g
                orderby g.Key
                select new {
                    g.Key,
                    MostExpensiveProducts =
                        from p2 in g
                        where p2.UnitPrice == g.Max(p3 => p3.UnitPrice)
                        select p2
                };

            ObjectDumper.Write(categories, 1);
        }

        [Category("Group By/Having")]
        [Title("GroupBy - Simple")]
        [Description("This sample uses group by to partition Products by " +
                     "CategoryID.")]
        public void LinqToSqlGroupBy01() {
            var q =
                from p in db.Products
                group p by p.CategoryID into g
                select g;

            ObjectDumper.Write(q, 1);
        }

        [Category("Group By/Having")]
        [Title("GroupBy - Max")]
        [Description("This sample uses group by and Max " +
                     "to find the maximum unit price for each CategoryID.")]
        public void LinqToSqlGroupBy02() {
            var q =
                from p in db.Products
                group p by p.CategoryID into g
                select new {
                    g.Key,
                    MaxPrice = g.Max(p => p.UnitPrice)
                };

            ObjectDumper.Write(q, 1);
        }

        [Category("Group By/Having")]
        [Title("GroupBy - Min")]
        [Description("This sample uses group by and Min " +
                     "to find the minimum unit price for each CategoryID.")]
        public void LinqToSqlGroupBy03() {
            var q =
                from p in db.Products
                group p by p.CategoryID into g
                select new {
                    g.Key,
                    MinPrice = g.Min(p => p.UnitPrice)
                };

            ObjectDumper.Write(q, 1);
        }

        [Category("Group By/Having")]
        [Title("GroupBy - Average")]
        [Description("This sample uses group by and Average " +
                     "to find the average UnitPrice for each CategoryID.")]
        public void LinqToSqlGroupBy04() {
            var q =
                from p in db.Products
                group p by p.CategoryID into g
                select new {
                    g.Key,
                    AveragePrice = g.Average(p => p.UnitPrice)
                };

            ObjectDumper.Write(q, 1);
        }

        [Category("Group By/Having")]
        [Title("GroupBy - Sum")]
        [Description("This sample uses group by and Sum " +
                     "to find the total UnitPrice for each CategoryID.")]
        public void LinqToSqlGroupBy05() {
            var q =
                from p in db.Products
                group p by p.CategoryID into g
                select new {
                    g.Key,
                    TotalPrice = g.Sum(p => p.UnitPrice)
                };

            ObjectDumper.Write(q, 1);
        }

        [Category("Group By/Having")]
        [Title("GroupBy - Count")]
        [Description("This sample uses group by and Count " +
                     "to find the number of Products in each CategoryID.")]
        public void LinqToSqlGroupBy06() {
            var q =
                from p in db.Products
                group p by p.CategoryID into g
                select new {
                    g.Key,
                    NumProducts = g.Count()
                };

            ObjectDumper.Write(q, 1);
        }

        [Category("Group By/Having")]
        [Title("GroupBy with Conditional Count")]
        [Description("This sample uses group by and Count " +
                     "to find the number of Products in each CategoryID " +
                     "that are discontinued.")]
        public void LinqToSqlGroupBy07() {
            var q =
                from p in db.Products
                group p by p.CategoryID into g
                select new {
                    g.Key,
                    NumProducts = g.Count(p => p.Discontinued)
                };

            ObjectDumper.Write(q, 1);
        }

        [Category("Group By/Having")]
        [Title("GroupBy - followed by Where")]
        [Description("This sample uses a where clause after a group by clause " +
                     "to find all categories that have at least 10 products.")]
        public void LinqToSqlGroupBy08() {
            var q =
                from p in db.Products
                group p by p.CategoryID into g
                where g.Count() >= 10
                select new {
                    g.Key,
                    ProductCount = g.Count()
                };

            ObjectDumper.Write(q, 1);
        }

        [Category("Group By/Having")]
        [Title("GroupBy - Multiple Columns")]
        [Description("This sample uses Group By to group products by CategoryID and SupplierID.")]
        public void LinqToSqlGroupBy09() {
            var categories =
                from p in db.Products
                group p by new { p.CategoryID, p.SupplierID } into g
                select new {g.Key, g};

            ObjectDumper.Write(categories, 1);
        }

        [Category("Group By/Having")]
        [Title("GroupBy - Expression")]
        [Description("This sample uses Group By to return two sequences of products. " +
                     "The first sequence contains products with unit price " +
                     "greater than 10. The second sequence contains products " +
                     "with unit price less than or equal to 10.")]
        public void LinqToSqlGroupBy10() {
            var categories =
                from p in db.Products
                group p by new { Criterion = p.UnitPrice > 10 } into g
                select g;

            ObjectDumper.Write(categories, 1);
        }

        [Category("Exists/In/Any/All/Contains")]
        [Title("Any - Simple")]
        [Description("This sample uses Any to return only Customers that have no Orders.")]
        public void LinqToSqlExists01() {
            var q =
                from c in db.Customers
                where !c.Orders.Any()
                select c;

            ObjectDumper.Write(q);
        }

        [Category("Exists/In/Any/All/Contains")]
        [Title("Any - Conditional")]
        [Description("This sample uses Any to return only Categories that have " +
                     "at least one Discontinued product.")]
        public void LinqToSqlExists02() {
            var q =
                from c in db.Categories
                where c.Products.Any(p => p.Discontinued)
                select c;

            ObjectDumper.Write(q);
        }

        [Category("Exists/In/Any/All/Contains")]
        [Title("All - Conditional")]
        [Description("This sample uses All to return Customers whom all of their orders " +
                     "have been shipped to their own city or whom have no orders.")]
        public void LinqToSqlExists03() {
            var q =
                from c in db.Customers
                where c.Orders.All(o => o.ShipCity == c.City)
                select c;

            ObjectDumper.Write(q);
        }

        [Category("Exists/In/Any/All/Contains")]
        [Title("Contains - One Object")]
        [Description("This sample uses Contain to find which Customer contains an order with OrderID 10248.")] 
        public void LinqToSqlExists04()
        {
            var order = (from o in db.Orders
                         where o.OrderID == 10248
                         select o).First();

            var q = db.Customers.Where(p => p.Orders.Contains(order)).ToList();

            foreach (var cust in q)
            {
                foreach (var ord in cust.Orders)
                {
                    Console.WriteLine("Customer {0} has OrderID {1}.", cust.CustomerID, ord.OrderID);
                }
            }
        }

        [Category("Exists/In/Any/All/Contains")]
        [Title("Contains - Multiple values")]
        [Description("This sample uses Contains to find customers whose city is Seattle, London, Paris or Vancouver.")]
        public void LinqToSqlExists05()
        {
            string[] cities = new string[] { "Seattle", "London", "Vancouver", "Paris" };
            var q = db.Customers.Where(p=>cities.Contains(p.City)).ToList();

            ObjectDumper.Write(q);
        }


        [Category("Union All/Union/Intersect")]
        [Title("Concat - Simple")]
        [Description("This sample uses Concat to return a sequence of all Customer and Employee " +
                     "phone/fax numbers.")]
        public void LinqToSqlUnion01() {
            var q = (
                     from c in db.Customers
                     select c.Phone
                    ).Concat(
                     from c in db.Customers
                     select c.Fax
                    ).Concat(
                     from e in db.Employees
                     select e.HomePhone
                    );

            ObjectDumper.Write(q);
        }

        [Category("Union All/Union/Intersect")]
        [Title("Concat - Compound")]
        [Description("This sample uses Concat to return a sequence of all Customer and Employee " +
                     "name and phone number mappings.")]
        public void LinqToSqlUnion02() {
            var q = (
                     from c in db.Customers
                     select new {Name = c.CompanyName, c.Phone}
                    ).Concat(
                     from e in db.Employees
                     select new {Name = e.FirstName + " " + e.LastName, Phone = e.HomePhone}
                    );

            ObjectDumper.Write(q);
        }

        [Category("Union All/Union/Intersect")]
        [Title("Union")]
        [Description("This sample uses Union to return a sequence of all countries that either " +
                     "Customers or Employees are in.")]
        public void LinqToSqlUnion03() {
            var q = (
                     from c in db.Customers
                     select c.Country
                    ).Union(
                     from e in db.Employees
                     select e.Country
                    );

            ObjectDumper.Write(q);
        }

        [Category("Union All/Union/Intersect")]
        [Title("Intersect")]
        [Description("This sample uses Intersect to return a sequence of all countries that both " +
                     "Customers and Employees live in.")]
        public void LinqToSqlUnion04() {
            var q = (
                     from c in db.Customers
                     select c.Country
                    ).Intersect(
                     from e in db.Employees
                     select e.Country
                    );

            ObjectDumper.Write(q);
        }

        [Category("Union All/Union/Intersect")]
        [Title("Except")]
        [Description("This sample uses Except to return a sequence of all countries that " +
                     "Customers live in but no Employees live in.")]
        public void LinqToSqlUnion05() {
            var q = (
                     from c in db.Customers
                     select c.Country
                    ).Except(
                     from e in db.Employees
                     select e.Country
                    );

            ObjectDumper.Write(q);
        }

        [Category("Top/Bottom")]
        [Title("Take")]
        [Description("This sample uses Take to select the first 5 Employees hired.")]
        public void LinqToSqlTop01() {
            var q = (
                from e in db.Employees
                orderby e.HireDate
                select e)
                .Take(5);

            ObjectDumper.Write(q);
        }

        [Category("Top/Bottom")]
        [Title("Skip")]
        [Description("This sample uses Skip to select all but the 10 most expensive Products.")]
        public void LinqToSqlTop02() {
            var q = (
                from p in db.Products
                orderby p.UnitPrice descending
                select p)
                .Skip(10);

            ObjectDumper.Write(q);
        }

        [Category("Paging")]
        [Title("Paging - Index")]
        [Description("This sample uses the Skip and Take operators to do paging by " +
                     "skipping the first 50 records and then returning the next 10, thereby " +
                     "providing the data for page 6 of the Products table.")]
        public void LinqToSqlPaging01() {
            var q = (
                from c in db.Customers
                orderby c.ContactName
                select c)
                .Skip(50)
                .Take(10);

            ObjectDumper.Write(q);
        }

        [Category("Paging")]
        [Title("Paging - Ordered Unique Key")]
        [Description("This sample uses a where clause and the Take operator to do paging by, " +
                     "first filtering to get only the ProductIDs above 50 (the last ProductID " +
                     "from page 5), then ordering by ProductID, and finally taking the first 10 results, " +
                     "thereby providing the data for page 6 of the Products table.  " +
                     "Note that this method only works when ordering by a unique key.")]
        public void LinqToSqlPaging02() {
            var q = (
                from p in db.Products
                where p.ProductID > 50
                orderby p.ProductID
                select p)
                .Take(10);

            ObjectDumper.Write(q);
        }
        [Category("SqlMethods")]
        [Title("SqlMethods - Like")]
        [Description("This sample uses SqlMethods to filter for Customers with CustomerID that starts with 'C'.")]
        public void LinqToSqlSqlMethods01()
        {

            var q = from c in db.Customers
                    where SqlMethods.Like(c.CustomerID, "C%")
                    select c;

            ObjectDumper.Write(q);

        }

        [Category("SqlMethods")]
        [Title("SqlMethods - DateDiffDay")]
        [Description("This sample uses SqlMethods to find all orders which shipped within 10 days the order created")]
        public void LinqToSqlSqlMethods02()
        {

            var q = from o in db.Orders
                    where SqlMethods.DateDiffDay(o.OrderDate, o.ShippedDate) < 10
                    select o;

            ObjectDumper.Write(q);

        }

        [Category("Compiled Query")]
        [Title("Compiled Query - 1")]
        [Description("This sample create a compiled query and then use it to retrieve customers of the input city")]
        public void LinqToSqlCompileQuery01()
        {
            //Create compiled query
            var fn = CompiledQuery.Compile((Northwind db2, string city) =>
                from c in db2.Customers
                where c.City == city
                select c);

            Console.WriteLine("****** Call compiled query to retrieve customers from London ******");
            var LonCusts = fn(db, "London");
            ObjectDumper.Write(LonCusts);

            Console.WriteLine();

            Console.WriteLine("****** Call compiled query to retrieve customers from Seattle ******");
            var SeaCusts = fn(db, "Seattle");
            ObjectDumper.Write(SeaCusts);

        }


        [Category("Insert/Update/Delete")]
        [Title("Insert - Simple")]
        [Description("This sample uses the InsertOnSubmit method to add a new Customer to the " +
                     "Customers Table object.  The call to SubmitChanges persists this " +
                     "new Customer to the database.")]
        public void LinqToSqlInsert01() {
            var q =
                from c in db.Customers
                where c.Region == "WA"
                select c;

            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(q);


            Console.WriteLine();
            Console.WriteLine("*** INSERT ***");
            var newCustomer = new Customer { CustomerID = "MCSFT",
                                             CompanyName = "Microsoft",
                                             ContactName = "John Doe",
                                             ContactTitle = "Sales Manager",
                                             Address = "1 Microsoft Way",
                                             City = "Redmond",
                                             Region = "WA",
                                             PostalCode = "98052",
                                             Country = "USA",
                                             Phone = "(425) 555-1234",
                                             Fax = null
                                           };
            db.Customers.InsertOnSubmit(newCustomer);
            db.SubmitChanges();


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ObjectDumper.Write(q);



            Cleanup64();  // Restore previous database state
        }

        private void Cleanup64() {
            SetLogging(false);

            db.Customers.DeleteAllOnSubmit(from c in db.Customers where c.CustomerID == "MCSFT" select c);
            db.SubmitChanges();
        }

        [Category("Insert/Update/Delete")]
        [Title("Insert - 1 to-Many")]
        [Description("This sample uses the InsetOnSubmit method to add a new Category to the " +
                     "Categories table object, and a new Product to the Products Table " +
                     "object with a foreign key relationship to the new Category.  The call " +
                     "to SubmitChanges persists these new objects and their relationships " +
                     "to the database.")]
        public void LinqToSqlInsert02() {

            Northwind db2 = new Northwind(connString);

            DataLoadOptions ds = new DataLoadOptions();

            ds.LoadWith<nwind.Category>(p => p.Products);
            db2.LoadOptions = ds;

            var q = (
                from c in db2.Categories
                where c.CategoryName == "Widgets"
                select c);


            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(q, 1);


            Console.WriteLine();
            Console.WriteLine("*** INSERT ***");
            var newCategory = new Category { CategoryName = "Widgets",
                                             Description = "Widgets are the customer-facing analogues " +
                                                           "to sprockets and cogs."
                                           };
            var newProduct = new Product { ProductName = "Blue Widget",
                                           UnitPrice = 34.56M,
                                           Category = newCategory
                                         };
            db2.Categories.InsertOnSubmit(newCategory);
            db2.SubmitChanges();


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ObjectDumper.Write(q, 1);

            Cleanup65();  // Restore previous database state
        }

        private void Cleanup65() {
            SetLogging(false);

            db.Products.DeleteAllOnSubmit(from p in db.Products where p.Category.CategoryName == "Widgets" select p);
            db.Categories.DeleteAllOnSubmit(from c in db.Categories where c.CategoryName == "Widgets" select c);
            db.SubmitChanges();
        }

        [Category("Insert/Update/Delete")]
        [Title("Insert - Many to Many")]
        [Description("This sample uses the InsertOnSubmit method to add a new Employee to the " +
                     "Employees table object, a new Territory to the Territories table " +
                     "object, and a new EmployeeTerritory to the EmployeeTerritories table " +
                     "object with foreign key relationships to the new Employee and Territory.  " +
                     "The call to SubmitChanges persists these new objects and their " +
                     "relationships to the database.")]
        public void LinqToSqlInsert03() {

            Northwind db2 = new Northwind(connString);

            DataLoadOptions ds = new DataLoadOptions();
            ds.LoadWith<nwind.Employee>(p => p.EmployeeTerritories);
            ds.LoadWith<nwind.EmployeeTerritory>(p => p.Territory);

            db2.LoadOptions = ds;
            var q = (
                from e in db.Employees
                where e.FirstName == "Nancy"
                select e);



            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(q, 1);


            Console.WriteLine();
            Console.WriteLine("*** INSERT ***");
            var newEmployee = new Employee { FirstName = "Kira",
                                             LastName = "Smith"
                                           };
            var newTerritory = new Territory { TerritoryID = "12345",
                                               TerritoryDescription = "Anytown",
                                               Region = db.Regions.First()
                                             };
            var newEmployeeTerritory = new EmployeeTerritory { Employee = newEmployee,
                                                               Territory = newTerritory
                                                             };
            db.Employees.InsertOnSubmit(newEmployee);
            db.Territories.InsertOnSubmit(newTerritory);
            db.EmployeeTerritories.InsertOnSubmit(newEmployeeTerritory);
            db.SubmitChanges();


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ObjectDumper.Write(q, 2);



            Cleanup66();  // Restore previous database state
        }

        private void Cleanup66() {
            SetLogging(false);

            db.EmployeeTerritories.DeleteAllOnSubmit(from et in db.EmployeeTerritories where et.TerritoryID == "12345" select et);
            db.Employees.DeleteAllOnSubmit(from e in db.Employees where e.FirstName == "Kira" && e.LastName == "Smith" select e);
            db.Territories.DeleteAllOnSubmit(from t in db.Territories where t.TerritoryID == "12345" select t);
            db.SubmitChanges();
        }

        [Category("Insert/Update/Delete")]
        [Title("Update - Simple")]
        [Description("This sample uses SubmitChanges to persist an update made to a retrieved " +
                     "Customer object back to the database.")]
        public void LinqToSqlInsert04() {
            var q =
                from c in db.Customers
                where c.CustomerID == "ALFKI"
                select c;

            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(q);


            Console.WriteLine();
            Console.WriteLine("*** UPDATE ***");
            Customer cust = db.Customers.First(c => c.CustomerID == "ALFKI");
            cust.ContactTitle = "Vice President";
            db.SubmitChanges();


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ObjectDumper.Write(q);

            Cleanup67();  // Restore previous database state
        }

        private void Cleanup67() {
            SetLogging(false);

            Customer cust = db.Customers.First(c => c.CustomerID == "ALFKI");
            cust.ContactTitle = "Sales Representative";
            db.SubmitChanges();
        }

        [Category("Insert/Update/Delete")]
        [Title("Update - Multiple and Show Changes")]
        [Description("This sample uses SubmitChanges to persist updates made to multiple retrieved " +
                     "Product objects back to the database. Also demonstartes how to determine which " +
                     "how many objects changed, which objects changed, and which object members changed.")]
        public void LinqToSqlInsert05() {
            var q = from p in db.Products
                    where p.CategoryID == 1
                    select p;

            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(q);


            Console.WriteLine();
            Console.WriteLine("*** UPDATE ***");
            foreach (var p in q)
            {
                p.UnitPrice += 1.00M;
            }

            //
            ChangeSet cs = db.GetChangeSet();
            Console.WriteLine("*** CHANGE SET ***");
            Console.WriteLine("Number of entities inserted: {0}", cs.Inserts.Count);
            Console.WriteLine("Number of entities updated:  {0}", cs.Updates.Count);
            Console.WriteLine("Number of entities deleted:  {0}", cs.Deletes.Count);
            Console.WriteLine();

            Console.WriteLine("*** GetOriginalEntityState ***");
            foreach (object o in cs.Updates)
            {
                Product p = o as Product;
                if (p != null)
                {
                    Product oldP = db.Products.GetOriginalEntityState(p);
                    Console.WriteLine("** Current **");
                    ObjectDumper.Write(p);
                    Console.WriteLine("** Original **");
                    ObjectDumper.Write(oldP);
                    Console.WriteLine();
                }
            }
            Console.WriteLine();

            Console.WriteLine("*** GetModifiedMembers ***");
            foreach (object o in cs.Updates)
            {
                Product p = o as Product;
                if (p != null)
                {
                    foreach (ModifiedMemberInfo mmi in db.Products.GetModifiedMembers(p))
                    {
                        Console.WriteLine("Member {0}", mmi.Member.Name);
                        Console.WriteLine("\tOriginal value: {0}", mmi.OriginalValue);
                        Console.WriteLine("\tCurrent  value: {0}", mmi.CurrentValue);
                    }
                }
            }

            db.SubmitChanges();

            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ObjectDumper.Write(q);

            Cleanup68();  // Restore previous database state
        }

        private void Cleanup68() {
            SetLogging(false);

            var q =
                from p in db.Products
                where p.CategoryID == 1
                select p;

            foreach (var p in q) {
                p.UnitPrice -= 1.00M;
            }
            db.SubmitChanges();
        }

        [Category("Insert/Update/Delete")]
        [Title("Delete - Simple")]
        [Description("This sample uses the DeleteOnSubmit method to delete a OrderDetail from the " +
                     "OrderDetail Table object.  The call to SubmitChanges persists this " +
                     "deletion to the database.")]
        public void LinqToSqlInsert06() {
            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(from c in db.OrderDetails where c.OrderID == 10255 select c);


            Console.WriteLine();
            Console.WriteLine("*** DELETE ***");
            //Beverages
            OrderDetail orderDetail = db.OrderDetails.First(c => c.OrderID == 10255 && c.ProductID == 36);

            db.OrderDetails.DeleteOnSubmit(orderDetail);
            db.SubmitChanges();


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ClearDBCache();
            ObjectDumper.Write(from c in db.OrderDetails where c.OrderID == 10255 select c);



            Cleanup69();  // Restore previous database state
        }

        private void Cleanup69() {
            SetLogging(false);

            OrderDetail orderDetail = new OrderDetail()
                                      {
                                          OrderID = 10255,
                                          ProductID = 36,
                                          UnitPrice = 15.200M,
                                          Quantity = 25,
                                          Discount = 0.0F
                                      };
            db.OrderDetails.InsertOnSubmit(orderDetail);

            db.SubmitChanges();
        }

        [Category("Insert/Update/Delete")]
        [Title("Delete - One to Many")]
        [Description("This sample uses the DeleteOnSubmit method to delete a Order and Order Detail " +
                     "from Order Details and Orders table. First deleting Order Details and then " +
                     "deleting from Orders. The call to SubmitChanges persists this deletion to the database.")]
        public void LinqToSqlInsert07() {
            var orderDetails =
                from o in db.OrderDetails
                where o.Order.CustomerID == "WARTH" && o.Order.EmployeeID == 3
                select o;

            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(orderDetails);

            Console.WriteLine();
            Console.WriteLine("*** DELETE ***");
            var order =
                (from o in db.Orders
                 where o.CustomerID == "WARTH" && o.EmployeeID == 3
                 select o).First();

            foreach (OrderDetail od in orderDetails)
            {
                db.OrderDetails.DeleteOnSubmit(od);
            }

            db.Orders.DeleteOnSubmit(order);

            db.SubmitChanges();


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ObjectDumper.Write(orderDetails);



            Cleanup70();  // Restore previous database state
        }

        private void Cleanup70() {
            SetLogging(false);

            Order order = new Order()
                          {
                              CustomerID = "WARTH",
                              EmployeeID = 3,
                              OrderDate = new DateTime(1996, 7, 26),
                              RequiredDate = new DateTime(1996, 9, 6),
                              ShippedDate = new DateTime(1996, 7, 31),
                              ShipVia = 3,
                              Freight = 25.73M,
                              ShipName = "Wartian Herkku",
                              ShipAddress = "Torikatu 38",
                              ShipCity = "Oulu",
                              ShipPostalCode="90110",
                              ShipCountry = "Finland"
                          };

                              //Order, Cus, Emp, OrderD, ReqD, ShiD, ShipVia, Frei, ShipN, ShipAdd, ShipCi, ShipReg, ShipPostalCost, ShipCountry
                              //10266	WARTH	3	1996-07-26 00:00:00.000	1996-09-06 00:00:00.000	1996-07-31 00:00:00.000	3	25.73	Wartian Herkku	Torikatu 38	Oulu	NULL	90110	Finland

            OrderDetail orderDetail = new OrderDetail()
                                      {
                                          ProductID = 12,
                                          UnitPrice = 30.40M,
                                          Quantity = 12,
                                          Discount = 0.0F
                                      };
            order.OrderDetails.Add(orderDetail);

            db.Orders.InsertOnSubmit(order);
            db.SubmitChanges();
        }


        [Category("Insert/Update/Delete")]
        [Title("Delete - Inferred Delete")]
        [Description("This sample demonstrates how inferred delete causes an actual delete operation " +
                     "on an entity object when its referencing entity removes the object from its EntitySet. " +
                     "The inferred delete behavior only happens when the entity's association mapping has" +
                     " DeleteOnNull set to true and the CanBeNull is false.")]
        public void LinqToSqlInsert08()
        {
            Console.WriteLine("*** BEFORE ***");
       
            ObjectDumper.Write(from o in db.Orders where o.OrderID == 10248 select o);
            ObjectDumper.Write(from d in db.OrderDetails where d.OrderID == 10248 select d);

            Console.WriteLine();
            Console.WriteLine("*** INFERRED DELETE ***");
            Order order = db.Orders.First(x => x.OrderID == 10248);
            OrderDetail od = order.OrderDetails.First(d => d.ProductID == 11);
            order.OrderDetails.Remove(od);

            db.SubmitChanges();

            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ClearDBCache();
            ObjectDumper.Write(from o in db.Orders where o.OrderID == 10248 select o);
            ObjectDumper.Write(from d in db.OrderDetails where d.OrderID == 10248 select d);
            CleanupInsert08();  // Restore previous database state
        }

        private void CleanupInsert08()
        {
            SetLogging(false);
            OrderDetail od = new OrderDetail() { ProductID = 11, Quantity = 12, UnitPrice = 14, OrderID = 10248, Discount = 0 };
            db.OrderDetails.InsertOnSubmit(od);
            db.SubmitChanges();
        }

        [Category("Insert/Update/Delete")]
        [Title("Insert - Override using Dynamic CUD")]
        [Description("This sample uses partial method InsertRegion provided by the DataContext to insert a Region. " +
                     "The call to SubmitChanges calls InsertRegion override which uses Dynamic CUD to run " +
                     "the default Linq To SQL generated SQL Query")]
        public void LinqToSqlInsert09()
        {
            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(from c in db.Regions where c.RegionID == 32 select c);


            Console.WriteLine();
            Console.WriteLine("*** INSERT OVERRIDE ***");
            //Beverages
            Region nwRegion = new Region() { RegionID = 32, RegionDescription = "Rainy" };

            db.Regions.InsertOnSubmit(nwRegion);
            db.SubmitChanges();


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ClearDBCache();
            ObjectDumper.Write(from c in db.Regions where c.RegionID == 32 select c);

            CleanupInsert09();  // Restore previous database state
        }

        private void CleanupInsert09()
        {
            SetLogging(false);

            db.Regions.DeleteAllOnSubmit(from r in db.Regions where r.RegionID == 32 select r);
            db.SubmitChanges();
        }

        [Category("Insert/Update/Delete")]
        [Title("Update with Attach" )]
        [Description("This sample takes entities from another tier, uses Attach and AttachAll " +
                     "to attach the deserialized entities to a data context, and then updates the entities. " +
                     "Changes are submitted to the database.")]
        public void LinqToSqlInsert10()
        {
           
            // Typically you would get entities to attach from deserializing XML from another tier.
            // It is not supported to attach entities from one DataContext to another DataContext.  
            // So to duplicate deserializing the entities, the entities will be recreated here.
            Customer c1;
            List<Order> deserializedOrders = new List<Order>();
            Customer deserializedC1;

            using (Northwind tempdb = new Northwind(connString))
            {
                c1 = tempdb.Customers.Single(c => c.CustomerID == "ALFKI");
                Console.WriteLine("Customer {0}'s original address {1}", c1.CustomerID, c1.Address);
                Console.WriteLine();
                deserializedC1 = new Customer { Address = c1.Address, City = c1.City,
                                                CompanyName=c1.CompanyName, ContactName=c1.ContactName,
                                                ContactTitle=c1.ContactTitle, Country=c1.Country,
                                                CustomerID=c1.CustomerID, Fax=c1.Fax,
                                                Phone=c1.Phone, PostalCode=c1.PostalCode,
                                                Region=c1.Region};
                Customer tempcust = tempdb.Customers.Single(c => c.CustomerID == "ANTON");
                foreach (Order o in tempcust.Orders)
                {
                    Console.WriteLine("Order {0} belongs to customer {1}", o.OrderID, o.CustomerID);
                    deserializedOrders.Add(new Order {CustomerID=o.CustomerID, EmployeeID=o.EmployeeID,
                                                      Freight=o.Freight, OrderDate=o.OrderDate, OrderID=o.OrderID,
                                                      RequiredDate=o.RequiredDate, ShipAddress=o.ShipAddress,
                                                      ShipCity=o.ShipCity, ShipName=o.ShipName,
                                                      ShipCountry=o.ShipCountry, ShippedDate=o.ShippedDate,
                                                      ShipPostalCode=o.ShipPostalCode, ShipRegion=o.ShipRegion,
                                                      ShipVia=o.ShipVia});
                }
                
                Console.WriteLine();

                Customer tempcust2 = tempdb.Customers.Single(c => c.CustomerID == "CHOPS");
                var c3Orders = tempcust2.Orders.ToList();
                foreach (Order o in c3Orders)
                {
                    Console.WriteLine("Order {0} belongs to customer {1}", o.OrderID, o.CustomerID);
                }
                Console.WriteLine();
            }

            using (Northwind db2 = new Northwind(connString))
            {
                // Attach the first entity to the current data context, to track changes.
                db2.Customers.Attach(deserializedC1);
                Console.WriteLine("***** Update Customer ALFKI's address ******");
                Console.WriteLine();
                // Change the entity that is tracked.
                deserializedC1.Address = "123 First Ave";

                // Attach all entities in the orders list.
                db2.Orders.AttachAll(deserializedOrders);
                // Update the orders to belong to another customer.
                Console.WriteLine("****** Assign all Orders belong to ANTON to CHOPS ******");
                Console.WriteLine();

                foreach (Order o in deserializedOrders)
                {
                    o.CustomerID = "CHOPS";
                }

                // Submit the changes in the current data context.
                db2.SubmitChanges();
            }

            // Check that the orders were submitted as expected.
            using (Northwind db3 = new Northwind(connString))
            {
                Customer dbC1 = db3.Customers.Single(c => c.CustomerID == "ALFKI");

                Console.WriteLine("Customer {0}'s new address {1}", dbC1.CustomerID, dbC1.Address);
                Console.WriteLine();

                Customer dbC2 = db3.Customers.Single(c => c.CustomerID == "CHOPS");

                foreach (Order o in dbC2.Orders)
                {
                    Console.WriteLine("Order {0} belongs to customer {1}", o.OrderID, o.CustomerID);
                }
              
            }

            CleanupInsert10();
        }

        private void CleanupInsert10()
        {
            int[] c2OrderIDs = { 10365, 10507, 10535, 10573, 10677, 10682, 10856 };
            using (Northwind tempdb = new Northwind(connString))
            {
                Customer c1 = tempdb.Customers.Single(c => c.CustomerID == "ALFKI");
                c1.Address = "Obere Str. 57";
                foreach (Order o in tempdb.Orders.Where(p => c2OrderIDs.Contains(p.OrderID)))
                    o.CustomerID = "ANTON";
                tempdb.SubmitChanges();
            }
        }

        [Category("Insert/Update/Delete")]
        [Title("Update and Delete with Attach")]
        [Description("This sample takes entities from one context and uses Attach and AttachAll " +
                     "to attach the entities from another context. Then, two entities are updated, " +
                     "one entity is deleted, and another entity is added. Changes are submitted to " +
                     "the database")]
        public void LinqToSqlInsert11()
        {
            // Typically you would get entities to attach from deserializing
            // XML from another tier.
            // This sample uses LoadWith to eager load customer and orders
            // in one query and disable deferred loading.
            Customer cust = null;
            using (Northwind tempdb = new Northwind(connString))
            {
                DataLoadOptions shape = new DataLoadOptions();
                shape.LoadWith<Customer>(c => c.Orders);
                // Load the first customer entity and its orders.
                tempdb.LoadOptions = shape;
                tempdb.DeferredLoadingEnabled = false;
                cust = tempdb.Customers.First(x => x.CustomerID == "ALFKI");
            }

            Console.WriteLine("Customer {0}'s original phone number {1}", cust.CustomerID, cust.Phone);
            Console.WriteLine();

            foreach (Order o in cust.Orders)
            {
                Console.WriteLine("Customer {0} has order {1} for city {2}", o.CustomerID, o.OrderID, o.ShipCity);
            }

            Order orderA = cust.Orders.First();
            Order orderB = cust.Orders.First(x => x.OrderID > orderA.OrderID);

            using (Northwind db2 = new Northwind(connString))
            {
                // Attach the first entity to the current data context, to track changes.
                db2.Customers.Attach(cust);
                // Attach the related orders for tracking; otherwise they will be inserted on submit.
                db2.Orders.AttachAll(cust.Orders.ToList());

                // Update the customer.
                cust.Phone = "2345 5436";
                // Update the first order.
                orderA.ShipCity = "Redmond";
                // Remove the second order.
                cust.Orders.Remove(orderB);
                // Create a new order and add it to the customer.
                Order orderC = new Order() { ShipCity = "New York" };
                Console.WriteLine("Adding new order");
                cust.Orders.Add(orderC);

                //Now submit the all changes
                db2.SubmitChanges();
            }

            // Verify that the changes were applied a expected.
            using (Northwind db3 = new Northwind(connString))
            {
                Customer newCust = db3.Customers.First(x => x.CustomerID == "ALFKI");
                Console.WriteLine("Customer {0}'s new phone number {1}", newCust.CustomerID, newCust.Phone);
                Console.WriteLine();

                foreach (Order o in newCust.Orders)
                {
                    Console.WriteLine("Customer {0} has order {1} for city {2}", o.CustomerID, o.OrderID, o.ShipCity);
                }
            }

            CleanupInsert11();
        }

        private void CleanupInsert11()
        {
            int[] alfkiOrderIDs = { 10643, 10692, 10702, 10835, 10952, 11011 };

            using (Northwind tempdb = new Northwind(connString))
            {
                Customer c1 = tempdb.Customers.Single(c => c.CustomerID == "ALFKI");
                c1.Phone = "030-0074321";
                Order oa = tempdb.Orders.Single(o => o.OrderID == 10643);
                oa.ShipCity = "Berlin";
                Order ob = tempdb.Orders.Single(o => o.OrderID == 10692);
                ob.CustomerID = "ALFKI";
                foreach (Order o in c1.Orders.Where(p => !alfkiOrderIDs.Contains(p.OrderID)))
                    tempdb.Orders.DeleteOnSubmit(o);

                tempdb.SubmitChanges();
            }
        }
        [Category("Simultaneous Changes")]
        [Title("Optimistic Concurrency - 1")]
        [Description("This and the following sample demonstrates optimistic concurrency.  In this sample, " +
                     "the other user makes and commits his update to Product 1 before you read the data " +
                     "so no conflict occurs.")]
        public void LinqToSqlSimultaneous01() {
            Console.WriteLine("OTHER USER: ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");

            // Open a second connection to the database to simulate another user
            // who is going to make changes to the Products table
            Northwind otherUser_db = new Northwind(connString) { Log = db.Log };

            var otherUser_product = otherUser_db.Products.First(p => p.ProductID == 1);
            otherUser_product.UnitPrice = 999.99M;
            otherUser_db.SubmitChanges();

            Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");


            Console.WriteLine();
            Console.WriteLine("YOU:  ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");

            var product = db.Products.First(p => p.ProductID == 1);
            product.UnitPrice = 777.77M;

            bool conflict = false;
            try {
                db.SubmitChanges();
            }
            //OptimisticConcurrencyException
            catch (ChangeConflictException) {
                conflict = true;
            }

            Console.WriteLine();
            if (conflict) {
                Console.WriteLine("* * * OPTIMISTIC CONCURRENCY EXCEPTION * * *");
                Console.WriteLine("Another user has changed Product 1 since it was first requested.");
                Console.WriteLine("Backing out changes.");
            }
            else {
                Console.WriteLine("* * * COMMIT SUCCESSFUL * * *");
                Console.WriteLine("Changes to Product 1 saved.");
            }

            Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");



            Cleanup71();  // Restore previous database state
        }

        private void Cleanup71() {
            ClearDBCache();
            SetLogging(false);

            var product = db.Products.First(p => p.ProductID == 1);
            product.UnitPrice = 18.00M;
            db.SubmitChanges();
        }

        [Category("Simultaneous Changes")]
        [Title("Optimistic Concurrency - 2")]
        [Description("This and the previous sample demonstrates optimistic concurrency.  In this sample, " +
                     "the other user makes and commits his update to Product 1 after you read the data, " +
                     "but before completing your update, causing an optimistic concurrency conflict.  " +
                     "Your changes are rolled back, allowing you to retrieve the newly updated data " +
                     "from the database and decide how to proceed with your own update.")]
        public void LinqToSqlSimultaneous02()
        {
            Console.WriteLine("YOU:  ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");

            var product = db.Products.First(p => p.ProductID == 1);

            Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");


            Console.WriteLine();
            Console.WriteLine("OTHER USER: ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");

            // Open a second connection to the database to simulate another user
            // who is going to make changes to the Products table.
            Northwind otherUser_db = new Northwind(connString) { Log = db.Log };

            var otherUser_product = otherUser_db.Products.First(p => p.ProductID == 1);
            otherUser_product.UnitPrice = 999.99M;
            otherUser_db.SubmitChanges();

            Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");


            Console.WriteLine();
            Console.WriteLine("YOU (continued): ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");

            product.UnitPrice = 777.77M;

            bool conflict = false;
            try {
                db.SubmitChanges();
            }
            //OptimisticConcurrencyException
            catch (ChangeConflictException) {
                conflict = true;
            }

            Console.WriteLine();
            if (conflict) {
                Console.WriteLine("* * * OPTIMISTIC CONCURRENCY EXCEPTION * * *");
                Console.WriteLine("Another user has changed Product 1 since it was first requested.");
                Console.WriteLine("Backing out changes.");
            }
            else {
                Console.WriteLine("* * * COMMIT SUCCESSFUL * * *");
                Console.WriteLine("Changes to Product 1 saved.");
            }

            Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ");



            Cleanup72();  // Restore previous database state
        }

        private void Cleanup72() {
            ClearDBCache();
            SetLogging(false);

            // transaction failure will roll data back automatically
        }

        [Category("Simultaneous Changes")]
        [Title("Transactions - Implicit")]
        [Description("This sample demonstrates the implicit transaction created by " +
                     "SubmitChanges.  The update to prod2's UnitsInStock field " +
                     "makes its value negative, which violates a check constraint " +
                     "on the server.  This causes the transaction that is updating " +
                     "both Products to fail, which rolls back all changes.")]
        public void LinqToSqlSimultaneous03()
        {
            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(from p in db.Products where p.ProductID == 4 select p);
            ObjectDumper.Write(from p in db.Products where p.ProductID == 5 select p);


            Console.WriteLine();
            Console.WriteLine("*** UPDATE WITH IMPLICIT TRANSACTION ***");
            try {
                Product prod1 = db.Products.First(p => p.ProductID == 4);
                Product prod2 = db.Products.First(p => p.ProductID == 5);
                prod1.UnitsInStock -= 3;
                prod2.UnitsInStock -= 5;    // ERROR: this will make the units in stock negative
                // db.SubmitChanges implicitly uses a transaction so that
                // either both updates are accepted or both are rejected
                db.SubmitChanges();
            }
            catch (System.Data.SqlClient.SqlException e) {
                Console.WriteLine(e.Message);
            }


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ClearDBCache();
            ObjectDumper.Write(from p in db.Products where p.ProductID == 4 select p);
            ObjectDumper.Write(from p in db.Products where p.ProductID == 5 select p);



            Cleanup73();  // Restore previous database state
        }

        private void Cleanup73() {
            SetLogging(false);

            // transaction failure will roll data back automatically
        }

        [Category("Simultaneous Changes")]
        [Title("Transactions - Explicit")]
        [Description("This sample demonstrates using an explicit transaction.  This " +
                     "provides more protection by including the reading of the data in the " +
                     "transaction to help prevent optimistic concurrency exceptions.  " +
                     "As in the previous query, the update to prod2's UnitsInStock field " +
                     "makes the value negative, which violates a check constraint within " +
                     "the database.  This causes the transaction that is updating both " +
                     "Products to fail, which rolls back all changes.")]
        public void LinqToSqlSimultaneous04()
        {
            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(from p in db.Products where p.ProductID == 4 select p);
            ObjectDumper.Write(from p in db.Products where p.ProductID == 5 select p);


            Console.WriteLine();
            Console.WriteLine("*** UPDATE WITH EXPLICIT TRANSACTION ***");
            // Explicit use of TransactionScope ensures that
            // the data will not change in the database between
            // read and write
            using (TransactionScope ts = new TransactionScope()) {
                try {
                    Product prod1 = db.Products.First(p => p.ProductID == 4);
                    Product prod2 = db.Products.First(p => p.ProductID == 5);
                    prod1.UnitsInStock -= 3;
                    prod2.UnitsInStock -= 5;    // ERROR: this will make the units in stock negative
                    db.SubmitChanges();
                }
                catch (System.Data.SqlClient.SqlException e) {
                    Console.WriteLine(e.Message);
                }
            }


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ClearDBCache();
            ObjectDumper.Write(from p in db.Products where p.ProductID == 4 select p);
            ObjectDumper.Write(from p in db.Products where p.ProductID == 5 select p);



            Cleanup74();  // Restore previous database state
        }

        private void Cleanup74() {
            SetLogging(false);

            // transaction failure will roll data back automatically
        }

        [Category("Null")]
        [Title("null")]
        [Description("This sample uses the null value to find Employees " +
                     "that do not report to another Employee.")]
        public void LinqToSqlNull01() {
            var q =
                from e in db.Employees
                where e.ReportsToEmployee == null
                select e;

            ObjectDumper.Write(q);
        }

        [Category("Null")]
        [Title("Nullable<T>.HasValue")]
        [Description("This sample uses Nullable<T>.HasValue to find Employees " +
                     "that do not report to another Employee.")]
        public void LinqToSqlNull02()
        {
            var q =
                from e in db.Employees
                where !e.ReportsTo.HasValue
                select e;

            ObjectDumper.Write(q);
        }

        [Category("Null")]
        [Title("Nullable<T>.Value")]
        [Description("This sample uses Nullable<T>.Value for Employees " +
                     "that report to another Employee to return the " +
                     "EmployeeID number of that employee.  Note that " +
                     "the .Value is optional.")]
        public void LinqToSqlNull03()
        {
            var q =
                from e in db.Employees
                where e.ReportsTo.HasValue
                select new {e.FirstName, e.LastName, ReportsTo = e.ReportsTo.Value};

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String Concatenation")]
        [Description("This sample uses the + operator to concatenate string fields " +
                     "and string literals in forming the Customers' calculated " +
                     "Location value.")]
        public void LinqToSqlString01()
        {
            var q =
                from c in db.Customers
                select new { c.CustomerID, Location = c.City + ", " + c.Country };

            ObjectDumper.Write(q, 1);
        }

        [Category("String/Date Functions")]
        [Title("String.Length")]
        [Description("This sample uses the Length property to find all Products whose " +
                     "name is shorter than 10 characters.")]
        public void LinqToSqlString02()
        {
            var q =
                from p in db.Products
                where p.ProductName.Length < 10
                select p;

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.Contains(substring)")]
        [Description("This sample uses the Contains method to find all Customers whose " +
                     "contact name contains 'Anders'.")]
        public void LinqToSqlString03()
        {
            var q =
                from c in db.Customers
                where c.ContactName.Contains("Anders")
                select c;

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.IndexOf(substring)")]
        [Description("This sample uses the IndexOf method to find the first instance of " +
                     "a space in each Customer's contact name.")]
        public void LinqToSqlString04()
        {
            var q =
                from c in db.Customers
                select new {c.ContactName, SpacePos = c.ContactName.IndexOf(" ")};

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.StartsWith(prefix)")]
        [Description("This sample uses the StartsWith method to find Customers whose " +
                     "contact name starts with 'Maria'.")]
        public void LinqToSqlString05()
        {
            var q =
                from c in db.Customers
                where c.ContactName.StartsWith("Maria")
                select c;

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.EndsWith(suffix)")]
        [Description("This sample uses the EndsWith method to find Customers whose " +
                     "contact name ends with 'Anders'.")]
        public void LinqToSqlString06()
        {
            var q =
                from c in db.Customers
                where c.ContactName.EndsWith("Anders")
                select c;

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.Substring(start)")]
        [Description("This sample uses the Substring method to return Product names starting " +
                     "from the fourth letter.")]
        public void LinqToSqlString07()
        {
            var q =
                from p in db.Products
                select p.ProductName.Substring(3);

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.Substring(start, length)")]
        [Description("This sample uses the Substring method to find Employees whose " +
                     "home phone numbers have '555' as the seventh through ninth digits.")]
        public void LinqToSqlString08()
        {
            var q =
                from e in db.Employees
                where e.HomePhone.Substring(6, 3) == "555"
                select e;

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.ToUpper()")]
        [Description("This sample uses the ToUpper method to return Employee names " +
                     "where the last name has been converted to uppercase.")]
        public void LinqToSqlString09()
        {
            var q =
                from e in db.Employees
                select new {LastName = e.LastName.ToUpper(), e.FirstName};

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.ToLower()")]
        [Description("This sample uses the ToLower method to return Category names " +
                     "that have been converted to lowercase.")]
        public void LinqToSqlString10()
        {
            var q =
                from c in db.Categories
                select c.CategoryName.ToLower();

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.Trim()")]
        [Description("This sample uses the Trim method to return the first five " +
                     "digits of Employee home phone numbers, with leading and " +
                     "trailing spaces removed.")]
        public void LinqToSqlString11() {
            var q =
                from e in db.Employees
                select e.HomePhone.Substring(0, 5).Trim();

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.Insert(pos, str)")]
        [Description("This sample uses the Insert method to return a sequence of " +
                     "employee phone numbers that have a ) in the fifth position, " +
                     "inserting a : after the ).")]
        public void LinqToSqlString12() {
            var q =
                from e in db.Employees
                where e.HomePhone.Substring(4, 1) == ")"
                select e.HomePhone.Insert(5, ":");

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.Remove(start)")]
        [Description("This sample uses the Remove method to return a sequence of " +
                     "employee phone numbers that have a ) in the fifth position, " +
                     "removing all characters starting from the tenth character.")]
        public void LinqToSqlString13() {
            var q =
                from e in db.Employees
                where e.HomePhone.Substring(4, 1) == ")"
                select e.HomePhone.Remove(9);

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.Remove(start, length)")]
        [Description("This sample uses the Remove method to return a sequence of " +
                     "employee phone numbers that have a ) in the fifth position, " +
                     "removing the first six characters.")]
        public void LinqToSqlString14() {
            var q =
                from e in db.Employees
                where e.HomePhone.Substring(4, 1) == ")"
                select e.HomePhone.Remove(0, 6);

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("String.Replace(find, replace)")]
        [Description("This sample uses the Replace method to return a sequence of " +
                     "Supplier information where the Country field has had " +
                     "UK replaced with United Kingdom and USA replaced with " +
                     "United States of America.")]
        public void LinqToSqlString15() {
            var q =
                from s in db.Suppliers
                select new {
                    s.CompanyName,
                    Country = s.Country.Replace("UK", "United Kingdom")
                                       .Replace("USA", "United States of America")
                };

            ObjectDumper.Write(q);
        }

        
        [Category("String/Date Functions")]
        [Title("DateTime.Year")]
        [Description("This sample uses the DateTime's Year property to " +
                     "find Orders placed in 1997.")]
        public void LinqToSqlString16() {
            var q =
                from o in db.Orders
                where o.OrderDate.Value.Year == 1997
                select o;

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("DateTime.Month")]
        [Description("This sample uses the DateTime's Month property to " +
                     "find Orders placed in December.")]
        public void LinqToSqlString17() {
            var q =
                from o in db.Orders
                where o.OrderDate.Value.Month == 12
                select o;

            ObjectDumper.Write(q);
        }

        [Category("String/Date Functions")]
        [Title("DateTime.Day")]
        [Description("This sample uses the DateTime's Day property to " +
                     "find Orders placed on the 31st day of the month.")]
        public void LinqToSqlString18() {
            var q =
                from o in db.Orders
                where o.OrderDate.Value.Day == 31
                select o;

            ObjectDumper.Write(q);
        }

        [Category("Object Identity")]
        [Title("Object Caching - 1")]
        [Description("This sample demonstrates how, upon executing the same query twice, " +
                     "you will receive a reference to the same object in memory each time.")]
        public void LinqToSqlObjectIdentity01() {
            Customer cust1 = db.Customers.First(c => c.CustomerID == "BONAP");
            Customer cust2 = db.Customers.First(c => c.CustomerID == "BONAP");

            Console.WriteLine("cust1 and cust2 refer to the same object in memory: {0}",
                              Object.ReferenceEquals(cust1, cust2));
        }

        [Category("Object Identity")]
        [Title("Object Caching - 2")]
        [Description("This sample demonstrates how, upon executing different queries that " +
                     "return the same row from the database, you will receive a " +
                     "reference to the same object in memory each time.")]
        public void LinqToSqlObjectIdentity02() {
            Customer cust1 = db.Customers.First(c => c.CustomerID == "BONAP");
            Customer cust2 = (
                from o in db.Orders
                where o.Customer.CustomerID == "BONAP"
                select o )
                .First()
                .Customer;

            Console.WriteLine("cust1 and cust2 refer to the same object in memory: {0}",
                              Object.ReferenceEquals(cust1, cust2));
        }

        [Category("Object Loading")]
        [Title("Deferred Loading - 1")]
        [Description("This sample demonstrates how navigating through relationships in " +
                     "retrieved objects can end up triggering new queries to the database " +
                     "if the data was not requested by the original query.")]
        public void LinqToSqlObject01() {
            var custs =
                from c in db.Customers
                where c.City == "Sao Paulo"
                select c;

            foreach (var cust in custs) {
                foreach (var ord in cust.Orders) {
                    Console.WriteLine("CustomerID {0} has an OrderID {1}.", cust.CustomerID, ord.OrderID);
                }
            }
        }

        [Category("Object Loading")]
        [Title("LoadWith - Eager Loading - 1")]
        [Description("This sample demonstrates how to use LoadWith to request related " +
                     "data during the original query so that additional roundtrips to the " +
                     "database are not required later when navigating through " +
                     "the retrieved objects.")]
        public void LinqToSqlObject02() {

            Northwind db2 = new Northwind(connString);
            db2.Log = this.OutputStreamWriter;

            DataLoadOptions ds = new DataLoadOptions();
            ds.LoadWith<nwind.Customer>(p => p.Orders);

            db2.LoadOptions = ds;

            var custs = (
                from c in db2.Customers
                where c.City == "Sao Paulo"
                select c);

            foreach (var cust in custs) {
                foreach (var ord in cust.Orders) {
                    Console.WriteLine("CustomerID {0} has an OrderID {1}.", cust.CustomerID, ord.OrderID);
                }
            }
        }

        [Category("Object Loading")]
        [Title("Deferred Loading + AssociateWith")]
        [Description("This sample demonstrates how navigating through relationships in " +
                     "retrieved objects can end up triggering new queries to the database " +
                     "if the data was not requested by the original query. Also this sample shows relationship " +
                     "objects can be filtered using Assoicate With when they are deferred loaded.")]
        public void LinqToSqlObject03() {

            Northwind db2 = new Northwind(connString);
            db2.Log = this.OutputStreamWriter;

            DataLoadOptions ds = new DataLoadOptions();
            ds.AssociateWith<nwind.Customer>(p => p.Orders.Where(o=>o.ShipVia > 1));

            db2.LoadOptions = ds;
            var custs =
                from c in db2.Customers
                where c.City == "London"
                select c;

            foreach (var cust in custs) {
                foreach (var ord in cust.Orders) {
                    foreach (var orderDetail in ord.OrderDetails) {
                        Console.WriteLine("CustomerID {0} has an OrderID {1} that ShipVia is {2} with ProductID {3} that has name {4}.",
                            cust.CustomerID, ord.OrderID, ord.ShipVia, orderDetail.ProductID, orderDetail.Product.ProductName);
                    }
                }
            }
        }

        [Category("Object Loading")]
        [Title("LoadWith - Eager Loading + Associate With")]
        [Description("This sample demonstrates how to use LoadWith to request related " +
                     "data during the original query so that additional roundtrips to the " +
                     "database are not required later when navigating through " +
                     "the retrieved objects. Also this sample shows relationship" +
                     "objects can be ordered by using Assoicate With when they are eager loaded.")]
        public void LinqToSqlObject04() {

            Northwind db2 = new Northwind(connString);
            db2.Log = this.OutputStreamWriter;

            DataLoadOptions ds = new DataLoadOptions();
            ds.LoadWith<Customer>(p => p.Orders);
            ds.LoadWith<Order>(p => p.OrderDetails);
            ds.AssociateWith<Order>(p=>p.OrderDetails.OrderBy(o=>o.Quantity));

            db2.LoadOptions = ds;
         
            var custs = (
                from c in db2.Customers
                where c.City == "London"
                select c );

            foreach (var cust in custs) {
                foreach (var ord in cust.Orders) {
                    foreach (var orderDetail in ord.OrderDetails) {
                        Console.WriteLine("CustomerID {0} has an OrderID {1} with ProductID {2} that has Quantity {3}.",
                            cust.CustomerID, ord.OrderID, orderDetail.ProductID, orderDetail.Quantity );
                    }
                }
            }
        }

        private bool isValidProduct(Product p) {
            return p.ProductName.LastIndexOf('C') == 0;
        }

        [Category("Object Loading")]
        [Title("Deferred Loading - (1:M)")]
        [Description("This sample demonstrates how navigating through relationships in " +
             "retrieved objects can result in triggering new queries to the database " +
             "if the data was not requested by the original query.")]
        public void LinqToSqlObject05() {
            var emps = from e in db.Employees
                       select e;

            foreach (var emp in emps) {
                foreach (var man in emp.Employees) {
                    Console.WriteLine("Employee {0} reported to Manager {1}.", emp.FirstName, man.FirstName);
                }
            }
        }

        [Category("Object Loading")]
        [Title("Deferred Loading - (Blob)")]
        [Description("This sample demonstrates how navigating through Link in " +
             "retrieved objects can end up triggering new queries to the database " +
             "if the data type is Link.")]
        public void LinqToSqlObject06() {
            var emps = from c in db.Employees
                       select c;

            foreach (Employee emp in emps) {
                Console.WriteLine("{0}", emp.Notes);
            }
        }


        [Category("Object Loading")]
        [Title("Load Override")]
        [Description("This samples overrides the partial method LoadProducts in Category class. When products of a category are being loaded,"+ 
        " LoadProducts is being called to load products that are not discontinued in this category. ")]
        public void LinqToSqlObject07()
        {
            Northwind db2 = new Northwind(connString);

            DataLoadOptions ds = new DataLoadOptions();

            ds.LoadWith<nwind.Category>(p => p.Products);
            db2.LoadOptions = ds;

            var q = (
                from c in db2.Categories
                where c.CategoryID < 3
                select c);

            foreach (var cat in q)
            {
                foreach (var prod in cat.Products)
                {
                    Console.WriteLine("Category {0} has a ProductID {1} that Discontined = {2}.", cat.CategoryID, prod.ProductID, prod.Discontinued);
                }
            }
        }

        [Category("Conversion Operators")]
        [Title("AsEnumerable")]
        [Description("This sample uses AsEnumerable so that the client-side IEnumerable<T> " +
                     "implementation of Where is used, instead of the default IQueryable<T> " +
                     "implementation which would be converted to SQL and executed " +
                     "on the server.  This is necessary because the where clause " +
                     "references a user-defined client-side method, isValidProduct, " +
                     "which cannot be converted to SQL.")]
        [LinkedMethod("isValidProduct")]
        public void LinqToSqlConversion01() {
            var q =
                from p in db.Products.AsEnumerable()
                where isValidProduct(p)
                select p;

            ObjectDumper.Write(q);
        }

        [Category("Conversion Operators")]
        [Title("ToArray")]
        [Description("This sample uses ToArray to immediately evaluate a query into an array " +
                     "and get the 3rd element.")]
        public void LinqToSqlConversion02() {
            var q =
                from c in db.Customers
                where c.City == "London"
                select c;

            Customer[] qArray = q.ToArray();
            ObjectDumper.Write(qArray[3], 0);
        }

        [Category("Conversion Operators")]
        [Title("ToList")]
        [Description("This sample uses ToList to immediately evaluate a query into a List<T>.")]
        public void LinqToSqlConversion03() {
            var q =
                from e in db.Employees
                where e.HireDate >= new DateTime(1994, 1, 1)
                select e;

            List<Employee> qList = q.ToList();
            ObjectDumper.Write(qList, 0);
        }

        [Category("Conversion Operators")]
        [Title("ToDictionary")]
        [Description("This sample uses ToDictionary to immediately evaluate a query and " +
                     "a key expression into an Dictionary<K, T>.")]
        public void LinqToSqlConversion04() {
            var q =
                from p in db.Products
                where p.UnitsInStock <= p.ReorderLevel && !p.Discontinued
                select p;

            Dictionary<int, Product> qDictionary = q.ToDictionary(p => p.ProductID);

            foreach (int key in qDictionary.Keys) {
                Console.WriteLine("Key {0}:", key);
                ObjectDumper.Write(qDictionary[key]);
                Console.WriteLine();
            }
        }

        [Category("Direct SQL")]
        [Title("SQL Query")]
        [Description("This sample uses ExecuteQuery<T> to execute an arbitrary SQL query, " +
                     "mapping the resulting rows to a sequence of Product objects.")]
        public void LinqToSqlDirect01() {
            var products = db.ExecuteQuery<Product>(
                "SELECT [Product List].ProductID, [Product List].ProductName " +
                "FROM Products AS [Product List] " +
                "WHERE [Product List].Discontinued = 0 " +
                "ORDER BY [Product List].ProductName;"
            );

            ObjectDumper.Write(products);
        }

        [Category("Direct SQL")]
        [Title("SQL Command")]
        [Description("This sample uses ExecuteCommand to execute an arbitrary SQL command, " +
                     "in this case a mass update to increase all Products' unit price by 1.00.")]
        public void LinqToSqlDirect02() {
            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(from p in db.Products select p);


            Console.WriteLine();
            Console.WriteLine("*** UPDATE ***");
            db.ExecuteCommand("UPDATE Products SET UnitPrice = UnitPrice + 1.00");


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ClearDBCache();
            ObjectDumper.Write(from p in db.Products select p);



            Cleanup110();  // Restore previous database state
        }

        private void Cleanup110() {
            SetLogging(false);

            db.ExecuteCommand("UPDATE Products SET UnitPrice = UnitPrice - 1.00");
        }

        [Category("ADO.NET Interop")]
        [Title("Connection Interop")]
        [Description("This sample uses a pre-existing ADO.NET connection to create a Northwind " +
                     "object that can be used to perform queries, in this case a query to return " +
                     "all orders with freight of at least 500.00.")]
        public void LinqToSqlAdo01() {
            // Create a standard ADO.NET connection:
            SqlConnection nwindConn = new SqlConnection(connString);
            nwindConn.Open();

            // ... other ADO.NET database access code ... //

            // Use pre-existing ADO.NET connection to create DataContext:
            Northwind interop_db = new Northwind(nwindConn) { Log = db.Log };

            var orders =
                from o in interop_db.Orders
                where o.Freight > 500.00M
                select o;

            ObjectDumper.Write(orders);

            nwindConn.Close();
        }

        [Category("ADO.NET Interop")]
        [Title("Transaction Interop")]
        [Description("This sample uses a pre-existing ADO.NET connection to create a Northwind " +
                     "object and then shares an ADO.NET transaction with it.  The transaction is " +
                     "used both to execute SQLCommands through the ADO.NET connection and to submit " +
                     "changes through the Northwind object.  When the transaction aborts due to a " +
                     "violated check constraint, all changes are rolled back, including both the " +
                     "changes made through the SqlCommand and the changes made through the " +
                     "Northwind object.")]
        public void LinqToSqlAdo02() {
            var q =
                from p in db.Products
                where p.ProductID == 3
                select p;

            Console.WriteLine("*** BEFORE ***");
            ObjectDumper.Write(q);


            Console.WriteLine();
            Console.WriteLine("*** INSERT ***");

            // Create a standard ADO.NET connection:
            SqlConnection nwindConn = new SqlConnection(connString);
            nwindConn.Open();

            // Use pre-existing ADO.NET connection to create DataContext:
            Northwind interop_db = new Northwind(nwindConn) { Log = db.Log };

            SqlTransaction nwindTxn = nwindConn.BeginTransaction();

            try {
                SqlCommand cmd = new SqlCommand("UPDATE Products SET QuantityPerUnit = 'single item' WHERE ProductID = 3");
                cmd.Connection = nwindConn;
                cmd.Transaction = nwindTxn;
                cmd.ExecuteNonQuery();

                // Share pre-existing ADO.NET transaction:
                //interop_db.LocalTransaction = nwindTxn;
                interop_db.Transaction = nwindTxn;

                Product prod1 = interop_db.Products.First(p => p.ProductID == 4);
                Product prod2 = interop_db.Products.First(p => p.ProductID == 5);
                prod1.UnitsInStock -= 3;
                prod2.UnitsInStock -= 5;    // ERROR: this will make the units in stock negative

                interop_db.SubmitChanges();

                nwindTxn.Commit();
            }
            catch (Exception e) {
                // If there is a transaction error, all changes are rolled back,
                // including any changes made directly through the ADO.NET connection
                Console.WriteLine(e.Message);
                Console.WriteLine("Error submitting changes... all changes rolled back.");
            }

            nwindConn.Close();


            Console.WriteLine();
            Console.WriteLine("*** AFTER ***");
            ClearDBCache();
            ObjectDumper.Write(q);



            Cleanup112();  // Restore previous database state
        }
        private void Cleanup112() {
            SetLogging(false);

            // transaction failure will roll data back automatically
        }

        [Category("Stored Procedures")]
        [Title("Scalar Return")]
        [Description("This sample uses a stored procedure to return the number of Customers in the 'WA' Region.")]
        public void LinqToSqlStoredProc01() {
            int count = db.CustomersCountByRegion("WA");

            Console.WriteLine(count);
        }

        [Category("Stored Procedures")]
        [Title("Single Result-Set")]
        [Description("This sample uses a stored procedure to return the CustomerID, ContactName, CompanyName" +
        " and City of customers who are in London.")]
        public void LinqToSqlStoredProc02() {
            ISingleResult<CustomersByCityResult> result = db.CustomersByCity("London");

            ObjectDumper.Write(result);
        }

        [Category("Stored Procedures")]
        [Title("Single Result-Set - Multiple Possible Shapes")]
        [Description("This sample uses a stored procedure to return a set of " +
        "Customers in the 'WA' Region.  The result set-shape returned depends on the parameter passed in. " +
        "If the parameter equals 1, all Customer properties are returned. " +
        "If the parameter equals 2, the CustomerID, ContactName and CompanyName properties are returned.")]
        public void LinqToSqlStoredProc03() {
            Console.WriteLine("********** Whole Customer Result-set ***********");
            IMultipleResults result = db.WholeOrPartialCustomersSet(1);
            IEnumerable<WholeCustomersSetResult> shape1 = result.GetResult<WholeCustomersSetResult>();

            ObjectDumper.Write(shape1);

            Console.WriteLine();
            Console.WriteLine("********** Partial Customer Result-set ***********");
            result = db.WholeOrPartialCustomersSet(2);
            IEnumerable<PartialCustomersSetResult> shape2 = result.GetResult<PartialCustomersSetResult>();

            ObjectDumper.Write(shape2);
        }

        [Category("Stored Procedures")]
        [Title("Multiple Result-Sets")]
        [Description("This sample uses a stored procedure to return the Customer 'SEVES' and all their Orders.")]
        public void LinqToSqlStoredProc04() {
            IMultipleResults result = db.GetCustomerAndOrders("SEVES");

            Console.WriteLine("********** Customer Result-set ***********");
            IEnumerable<CustomerResultSet> customer = result.GetResult<CustomerResultSet>();
            ObjectDumper.Write(customer);
            Console.WriteLine();

            Console.WriteLine("********** Orders Result-set ***********");
            IEnumerable<OrdersResultSet> orders = result.GetResult<OrdersResultSet>();
            ObjectDumper.Write(orders);
        }

        [Category("Stored Procedures")]
        [Title("Out parameters")]
        [Description("This sample uses a stored procedure that returns an out parameter.")]
        public void LinqToSqlStoredProc05() {
            decimal? totalSales = 0;
            string customerID = "ALFKI";

            // Out parameters are passed by ref, to support scenarios where
            // the parameter is 'in/out'.  In this case, the parameter is only
            // 'out'.
            db.CustomerTotalSales(customerID, ref totalSales);

            Console.WriteLine("Total Sales for Customer '{0}' = {1:C}", customerID, totalSales);
        }


        [Category("User-Defined Functions")]
        [Title("Scalar Function - Select")]
        [Description("This sample demonstrates using a scalar user-defined function in a projection.")]
        public void LinqToSqlUserDefined01() {
            var q = from c in db.Categories
                    select new {c.CategoryID, TotalUnitPrice = db.TotalProductUnitPriceByCategory(c.CategoryID)};

            ObjectDumper.Write(q);
        }

        [Category("User-Defined Functions")]
        [Title("Scalar Function - Where")]
        [Description("This sample demonstrates using a scalar user-defined function in a where clause.")]
        public void LinqToSqlUserDefined02()
        {
            var q = from p in db.Products
                    where p.UnitPrice == db.MinUnitPriceByCategory(p.CategoryID)
                    select p;

            ObjectDumper.Write(q);
        }

        [Category("User-Defined Functions")]
        [Title("Table-Valued Function")]
        [Description("This sample demonstrates selecting from a table-valued user-defined function.")]
        public void LinqToSqlUserDefined03()
        {
            var q = from p in db.ProductsUnderThisUnitPrice(10.25M)
                    where !(p.Discontinued ?? false)
                    select p;

            ObjectDumper.Write(q);
        }

        [Category("User-Defined Functions")]
        [Title("Table Valued Function - Join")]
        [Description("This sample demonstrates joining to the results of a table-valued user-defined function.")]
        public void LinqToSqlUserDefined04()
        {
            var q = from c in db.Categories
                    join p in db.ProductsUnderThisUnitPrice(8.50M) on c.CategoryID equals p.CategoryID into prods
                    from p in prods
                    select new {c.CategoryID, c.CategoryName, p.ProductName, p.UnitPrice};

            ObjectDumper.Write(q);
        }

        [Category("DataContext Functions")]
        [Title("CreateDatabase() and DeleteDatabase() ")]
        [Description("This sample uses CreateDatabase() to create a new database based on the NewCreateDB Schema in Mapping.cs,  " +
                     "and DeleteDatabase() to delete the newly created database.")]
        public void LinqToSqlDataContext01() {

            // Create a temp folder to store the new created Database 
            string userTempFolder = Environment.GetEnvironmentVariable("SystemDrive") + @"\LinqToSqlSamplesTemp";
            Directory.CreateDirectory(userTempFolder);

            Console.WriteLine("********** Create NewCreateDB ***********");
            string userMDF = System.IO.Path.Combine(userTempFolder, @"NewCreateDB.mdf");
            string connStr = String.Format(@"Data Source=.\SQLEXPRESS;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30;User Instance=True; Integrated Security = SSPI;", userMDF);
            NewCreateDB newDB = new NewCreateDB(connStr);

            newDB.CreateDatabase();

            if (newDB.DatabaseExists() && File.Exists(Path.Combine(userTempFolder, @"NewCreateDB.mdf")))
                Console.WriteLine("NewCreateDB is created");
            else
                Console.WriteLine("Error: NewCreateDB is not successfully created");

            Console.WriteLine();
            Console.WriteLine("********* Insert data and query *********");

            var newRow = new Person { PersonID = 1, PersonName = "Peter", Age = 28 };

            newDB.Persons.InsertOnSubmit(newRow);
            newDB.SubmitChanges();

            var q = from x in newDB.Persons
                    select x;

            ObjectDumper.Write(q);

            Console.WriteLine();
            Console.WriteLine("************ Delete NewCreateDB **************");
            newDB.DeleteDatabase();

            if (File.Exists(Path.Combine(userTempFolder, @"NewCreateDB.mdf")))
                Console.WriteLine("Error: NewCreateDB is not yet deleted");
            else
                Console.WriteLine("NewCreateDB is deleted");

            // Delete the temp folder created for this testcase 
            Directory.Delete(userTempFolder);

        }

        [Category("DataContext Functions")]
        [Title("DatabaseExists()")]
        [Description("This sample uses DatabaseExists() to verify whether a database exists or not.")]
        public void LinqToSqlDataContext02() {

            Console.WriteLine("*********** Verify Northwind DB exists ***********");
            if (db.DatabaseExists())
                Console.WriteLine("Northwind DB exists");
            else
                Console.WriteLine("Error: Northwind DB does not exist");

            Console.WriteLine();
            Console.WriteLine("********* Verify NewCreateDB does not exist **********");
            string userTempFolder = Environment.GetEnvironmentVariable("Temp");
            string userMDF = System.IO.Path.Combine(userTempFolder, @"NewCreateDB.mdf");
            NewCreateDB newDB = new NewCreateDB(userMDF);

            if (newDB.DatabaseExists())
                Console.WriteLine("Error: NewCreateDB DB exists");
            else
                Console.WriteLine("NewCreateDB DB does not exist");
        }

        [Category("DataContext Functions")]
        [Title("SubmitChanges()")]
        [Description("This sample demonstrates that SubmitChanges() must be called in order to  " +
         "submit any update to the actual database.")]
        public void LinqToSql1DataContext03() {
            Customer cust = db.Customers.First(c=>c.CustomerID == "ALFKI");

            Console.WriteLine("********** Original Customer CompanyName **********");
            var q = from x in db.Customers
                     where x.CustomerID == "ALFKI"
                     select x.CompanyName;

            Console.WriteLine();
            ObjectDumper.Write(q);

            Console.WriteLine();
            Console.WriteLine("*********** Update and call SubmitChanges() **********");

            cust.CompanyName = "CSharp Programming Firm";
            db.SubmitChanges();

            Console.WriteLine();
            ObjectDumper.Write(q);

            Console.WriteLine();
            Console.WriteLine("*********** Update and did not call SubmitChanges() **********");

            cust.CompanyName = "LinqToSql Programming Firm";

            Console.WriteLine();
            ObjectDumper.Write(q);

            Cleanup122();  // Restore previous database state      
        }

        private void Cleanup122() {
            SetLogging(false);
            Customer cust = db.Customers.First(c=>c.CustomerID == "ALFKI");
            cust.CompanyName = "Alfreds Futterkiste";
            db.SubmitChanges();
        }
 
        [Category("DataContext Functions")]
        [Title("CreateQuery()")]
        [Description("This sample uses CreateQuery() to create an IQueryable<T> out of Expression.")]
        public void LinqToSqlDataContext04() {

            var c1 = Expression.Parameter(typeof(Customer), "c");
            PropertyInfo City = typeof(Customer).GetProperty("City");

            var pred = Expression.Lambda<Func<Customer, bool>>(
                Expression.Equal(
                Expression.Property(c1, City),
                  Expression.Constant("Seattle")
               ), c1
            );

            IQueryable custs = db.Customers;
            Expression expr = Expression.Call(typeof(Queryable), "Where",
                new Type[] { custs.ElementType }, custs.Expression, pred);
            IQueryable<Customer> q = db.Customers.AsQueryable().Provider.CreateQuery<Customer>(expr);

            ObjectDumper.Write(q);
        }

        [Category("DataContext Functions")]
        [Title("Log")]
        [Description("This sample uses Db.Log to turn off and turn on the database logging display.")]
        public void LinqToSqlDataContext05() {
            Console.WriteLine("**************** Turn off DB Log Display *****************");
            db.Log = null;
            var q = from c in db.Customers
                    where c.City == "London"
                    select c;

            ObjectDumper.Write(q);

            Console.WriteLine();
            Console.WriteLine("**************** Turn on DB Log Display  *****************");

            db.Log = this.OutputStreamWriter;
            ObjectDumper.Write(q);


        }

        [Category("Advanced")]
        [Title("Dynamic query - Select")]
        [Description("This sample builds a query dynamically to return the contact name of each customer. " + 
                     "The GetCommand method is used to get the generated T-SQL of the query.")]
        public void LinqToSqlAdvanced01()
        {
            ParameterExpression param = Expression.Parameter(typeof(Customer), "c");
            Expression selector = Expression.Property(param, typeof(Customer).GetProperty("ContactName"));
            Expression pred = Expression.Lambda(selector, param);

            IQueryable<Customer> custs = db.Customers;
            Expression expr = Expression.Call(typeof(Queryable), "Select", new Type[] { typeof(Customer), typeof(string) }, Expression.Constant(custs), pred);
            IQueryable<string> query = db.Customers.AsQueryable().Provider.CreateQuery<string>(expr);

            System.Data.Common.DbCommand cmd = db.GetCommand(query);
            Console.WriteLine("Generated T-SQL:");
            Console.WriteLine(cmd.CommandText);
            Console.WriteLine();

            ObjectDumper.Write(query);
        }

        [Category("Advanced")]
        [Title("Dynamic query - Where")]
        [Description("This sample builds a query dynamically to filter for Customers in London.")]
        public void LinqToSqlAdvanced02()
        {
            IQueryable<Customer> custs = db.Customers;
            ParameterExpression param = Expression.Parameter(typeof(Customer), "c");
            Expression right = Expression.Constant("London");
            Expression left = Expression.Property(param, typeof(Customer).GetProperty("City"));
            Expression filter = Expression.Equal(left, right);
            Expression pred = Expression.Lambda(filter, param);

            Expression expr = Expression.Call(typeof(Queryable), "Where", new Type[] { typeof(Customer) }, Expression.Constant(custs), pred);
            IQueryable<Customer> query = db.Customers.AsQueryable().Provider.CreateQuery<Customer>(expr);
            ObjectDumper.Write(query);
        }

        [Category("Advanced")]
        [Title("Dynamic query - OrderBy")]
        [Description("This sample builds a query dynamically to filter for Customers in London" +
                     " and order them by ContactName.")]
        public void LinqToSqlAdvanced03()
        {
            ParameterExpression param = Expression.Parameter(typeof(Customer), "c");

            Expression left = Expression.Property(param, typeof(Customer).GetProperty("City"));
            Expression right = Expression.Constant("London");
            Expression filter = Expression.Equal(left, right);
            Expression pred = Expression.Lambda(filter, param);

            IQueryable custs = db.Customers;

            Expression expr = Expression.Call(typeof(Queryable), "Where",
                new Type[] { typeof(Customer) }, Expression.Constant(custs), pred);

            expr = Expression.Call(typeof(Queryable), "OrderBy",
                new Type[] { typeof(Customer), typeof(string) }, custs.Expression, Expression.Lambda(Expression.Property(param, "ContactName"), param));


            IQueryable<Customer> query = db.Customers.AsQueryable().Provider.CreateQuery<Customer>(expr);

            ObjectDumper.Write(query);
        }


        [Category("Advanced")]
        [Title("Dynamic query - Union")]
        [Description("This sample dynamically builds a Union to return a sequence of all countries where either " +
                     "a customer or an employee live.")]
        public void LinqToSqlAdvanced04()
        {
            IQueryable<Customer> custs = db.Customers;
            ParameterExpression param1 = Expression.Parameter(typeof(Customer), "e");
            Expression left1 = Expression.Property(param1, typeof(Customer).GetProperty("City"));
            Expression pred1 = Expression.Lambda(left1, param1);

            IQueryable<Employee> employees = db.Employees;
            ParameterExpression param2 = Expression.Parameter(typeof(Employee), "c");
            Expression left2 = Expression.Property(param2, typeof(Employee).GetProperty("City"));
            Expression pred2 = Expression.Lambda(left2, param2);

            Expression expr1 = Expression.Call(typeof(Queryable), "Select", new Type[] { typeof(Customer), typeof(string) }, Expression.Constant(custs), pred1);
            Expression expr2 = Expression.Call(typeof(Queryable), "Select", new Type[] { typeof(Employee), typeof(string) }, Expression.Constant(employees), pred2);

            IQueryable<string> q1 = db.Customers.AsQueryable().Provider.CreateQuery<string>(expr1);
            IQueryable<string> q2 = db.Employees.AsQueryable().Provider.CreateQuery<string>(expr2);

            var q3 = q1.Union(q2);

            ObjectDumper.Write(q3);
        }

        [Category("Advanced")]
        [Title("Identity")]
        [Description("This sample demonstrates how we insert a new Contact and retrieve the " +
                     "newly assigned ContactID from the database.")]
        public void LinqToSqlAdvanced05() {

            Console.WriteLine("ContactID is marked as an identity column");
            Contact con = new Contact() { CompanyName = "New Era", Phone = "(123)-456-7890" };

            db.Contacts.InsertOnSubmit(con);
            db.SubmitChanges();

            Console.WriteLine();
            Console.WriteLine("The ContactID of the new record is {0}", con.ContactID);

            cleanup130(con.ContactID);

        }
        void cleanup130(int contactID) {
            SetLogging(false);
            Contact con = db.Contacts.Where(c=>c.ContactID == contactID).First();
            db.Contacts.DeleteOnSubmit(con);
            db.SubmitChanges();
        }

        [Category("Advanced")]
        [Title("Nested in From")]
        [Description("This sample uses orderbyDescending and Take to return the " +
                     "discontinued products of the top 10 most expensive products.")]
        public void LinqToSqlAdvanced06() {
            var prods = from p in db.Products.OrderByDescending(p => p.UnitPrice).Take(10)
                    where p.Discontinued
                    select p;

            ObjectDumper.Write(prods, 0);
        }

        [Category("View")]
        [Title("Query - Anonymous Type")]
        [Description("This sample uses SELECT and WHERE to return a sequence of invoices " +
                     " where shipping city is London.")]
        public void LinqToSqlView01() {
            var q =
                from i in db.Invoices
                where i.ShipCity == "London"
                select new {i.OrderID, i.ProductName, i.Quantity, i.CustomerName};

            ObjectDumper.Write(q, 1);
        }

        [Category("View")]
        [Title("Query - Identity mapping")]
        [Description("This sample uses SELECT to query QuarterlyOrders.")]
        public void LinqToSqlView02() {
            var q =
                from qo in db.QuarterlyOrders
                select qo;

            ObjectDumper.Write(q, 1);
        }

        [Category("Inheritance")]
        [Title("Simple")]
        [Description("This sample returns all contacts where the city is London.")]
        public void LinqToSqlInheritance01()
        {
            var cons = from c in db.Contacts                       
                       select c;

            foreach (var con in cons) {
                Console.WriteLine("Company name: {0}", con.CompanyName);
                Console.WriteLine("Phone: {0}", con.Phone);
                Console.WriteLine("This is a {0}", con.GetType());
                Console.WriteLine();
            }

        }

        [Category("Inheritance")]
        [Title("OfType")]
        [Description("This sample uses OfType to return all customer contacts.")]
        public void LinqToSqlInheritance02()
        {
            var cons = from c in db.Contacts.OfType<CustomerContact>()
                       select c;

            ObjectDumper.Write(cons, 0);
        }

        [Category("Inheritance")]
        [Title("IS")]
        [Description("This sample uses IS to return all shipper contacts.")]
        public void LinqToSqlInheritance03()
        {
            var cons = from c in db.Contacts
                       where c is ShipperContact
                       select c;

            ObjectDumper.Write(cons, 0);
        }

        [Category("Inheritance")]
        [Title("AS")]
        [Description("This sample uses AS to return FullContact or null.")]
        public void LinqToSqlInheritance04()
        {
            var cons = from c in db.Contacts
                       select c as FullContact;

            ObjectDumper.Write(cons, 0);
        }

        [Category("Inheritance")]
        [Title("Cast")]
        [Description("This sample uses a cast to retrieve customer contacts who live in London.")]
        public void LinqToSqlInheritance05()
        {
            var cons = from c in db.Contacts
                       where c.ContactType == "Customer" && ((CustomerContact)c).City == "London"
                       select c;

            ObjectDumper.Write(cons, 0);
        }

        [Category("Inheritance")]
        [Title("UseAsDefault")]
        [Description("This sample demonstrates that an unknown contact type  " +
                     "will be automatically converted to the default contact type.")]
        public void LinqToSqlInheritance06()
        {
            Console.WriteLine("***** INSERT Unknown Contact using normal mapping *****");
            Contact contact = new Contact() { ContactType = null, CompanyName = "Unknown Company", Phone = "333-444-5555" };
            db.Contacts.InsertOnSubmit(contact);
            db.SubmitChanges();

            Console.WriteLine();
            Console.WriteLine("***** Query Unknown Contact using inheritance mapping *****");
            var con = (from c in db.Contacts
                       where c.CompanyName == "Unknown Company" && c.Phone == "333-444-5555"
                       select c).First();

            Console.WriteLine("The base class nwind.BaseContact had been used as default fallback type");
            Console.WriteLine("The discriminator value for con is unknown. So, its type should be {0}", con.GetType().ToString());

            cleanup140(contact.ContactID);

        }

        void cleanup140(int contactID) {
            SetLogging(false);
            Contact con = db.Contacts.Where(c=>c.ContactID == contactID).First();
            db.Contacts.DeleteOnSubmit(con);
            db.SubmitChanges();

        }

        [Category("Inheritance")]
        [Title("Insert New Record")]
        [Description("This sample demonstrates how to create a new shipper contact.")]
        public void LinqToSqlInheritance07()
        {
            Console.WriteLine("****** Before Insert Record ******");
            var ShipperContacts = from sc in db.Contacts.OfType<ShipperContact>()
                                     where sc.CompanyName == "Northwind Shipper"
                                     select sc;

            Console.WriteLine();
            Console.WriteLine("There is {0} Shipper Contact matched our requirement", ShipperContacts.Count());

            ShipperContact nsc = new ShipperContact() { CompanyName = "Northwind Shipper", Phone = "(123)-456-7890" };
            db.Contacts.InsertOnSubmit(nsc);
            db.SubmitChanges();

            Console.WriteLine();
            Console.WriteLine("****** After Insert Record ******");
            ShipperContacts = from sc in db.Contacts.OfType<ShipperContact>()
                               where sc.CompanyName == "Northwind Shipper"
                               select sc;

            Console.WriteLine();
            Console.WriteLine("There is {0} Shipper Contact matched our requirement", ShipperContacts.Count());
            db.Contacts.DeleteOnSubmit(nsc);
            db.SubmitChanges();

        }

        [Category("External Mapping")]
        [Title("Load and use an External Mapping")]
        [Description("This sample demonstrates how to create a data context that uses an external XML mapping source.")]
        public void LinqToSqlExternal01()
        {
            // load the mapping source
            string path = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\Data\NorthwindMapped.map"));

            XmlMappingSource mappingSource = XmlMappingSource.FromXml(File.ReadAllText(path));

            // create context using mapping source
            Mapped.NorthwindMapped nw = new Mapped.NorthwindMapped(db.Connection, mappingSource);

            // demonstrate use of an externally-mapped entity 
            Console.WriteLine("****** Externally-mapped entity ******");
            Mapped.Order order = nw.Orders.First();
            ObjectDumper.Write(order, 1);

            // demonstrate use of an externally-mapped inheritance hierarchy
            var contacts = from c in nw.Contacts
                           where c is Mapped.EmployeeContact
                           select c;
            Console.WriteLine();
            Console.WriteLine("****** Externally-mapped inheritance hierarchy ******");
            foreach (var contact in contacts)
            {
                Console.WriteLine("Company name: {0}", contact.CompanyName);
                Console.WriteLine("Phone: {0}", contact.Phone);
                Console.WriteLine("This is a {0}", contact.GetType());
                Console.WriteLine();
            }

            // demonstrate use of an externally-mapped stored procedure
            Console.WriteLine();
            Console.WriteLine("****** Externally-mapped stored procedure ******");
            foreach (Mapped.CustOrderHistResult result in nw.CustomerOrderHistory("ALFKI"))
            {
                ObjectDumper.Write(result, 0);
            }

            // demonstrate use of an externally-mapped scalar user defined function
            Console.WriteLine();
            Console.WriteLine("****** Externally-mapped scalar UDF ******");
            var totals = from c in nw.Categories
                         select new { c.CategoryID, TotalUnitPrice = nw.TotalProductUnitPriceByCategory(c.CategoryID) };
            ObjectDumper.Write(totals);

            // demonstrate use of an externally-mapped table-valued user-defined function
            Console.WriteLine();
            Console.WriteLine("****** Externally-mapped table-valued UDF ******");
            var products = from p in nw.ProductsUnderThisUnitPrice(9.75M)
                           where p.SupplierID == 8
                           select p;
            ObjectDumper.Write(products);
        }

        [Category("Optimistic Concurrency")]
        [Title("Get conflict information")]
        [Description("This sample demonstrates how to retrieve the changes that lead to an optimistic concurrency exception.")]
        public void LinqToSqlOptimistic01() {
            Console.WriteLine("YOU:  ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");
            Product product = db.Products.First(p => p.ProductID == 1);
            Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");
            Console.WriteLine();
            Console.WriteLine("OTHER USER: ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");
            // Open a second connection to the database to simulate another user
            // who is going to make changes to the Products table                
            Northwind otherUser_db = new Northwind(connString) { Log = db.Log };
            Product otherUser_product = otherUser_db.Products.First(p => p.ProductID == 1);
            otherUser_product.UnitPrice = 999.99M;
            otherUser_product.UnitsOnOrder = 10;
            otherUser_db.SubmitChanges();
            Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");
            Console.WriteLine("YOU (continued): ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~");
            product.UnitPrice = 777.77M;

            bool conflictOccurred = false;
            try {
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            } catch (ChangeConflictException) {
                Console.WriteLine("* * * OPTIMISTIC CONCURRENCY EXCEPTION * * *");
                foreach (ObjectChangeConflict aConflict in /*ex.Conflicts*/db.ChangeConflicts) {
                    Product prod = (Product)aConflict.Object;
                    Console.WriteLine("The conflicting product has ProductID {0}", prod.ProductID);
                    Console.WriteLine();
                    Console.WriteLine("Conflicting members:");
                    Console.WriteLine();
                    foreach (MemberChangeConflict memConflict in aConflict.MemberConflicts) {
                        string name = memConflict.Member/*MemberInfo*/.Name;
                        string yourUpdate = memConflict.CurrentValue.ToString();
                        string original = memConflict.OriginalValue.ToString();
                        string theirUpdate = memConflict.DatabaseValue.ToString();
                        if (memConflict.IsModified/*HaveModified*/) {
                            Console.WriteLine("'{0}' was updated from {1} to {2} while you updated it to {3}",
                                              name, original, theirUpdate, yourUpdate);
                        } else {
                            Console.WriteLine("'{0}' was updated from {1} to {2}, you did not change it.",
                                name, original, theirUpdate);
                        }
                    }
                    Console.WriteLine();
                }
                conflictOccurred = true;
            }

            Console.WriteLine();
            if (!conflictOccurred) {

                Console.WriteLine("* * * COMMIT SUCCESSFUL * * *");
                Console.WriteLine("Changes to Product 1 saved.");
            }
            Console.WriteLine("~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ~ ");

            ResetProducts(); // clean up
        }

        private void ResetProducts() {
            ClearDBCache();
            Northwind dbClean = new Northwind(connString);
            Product[] prod = new Product[4];
            decimal[] price = new decimal[4] { 18.00M, 19.00M, 10.00M, 22.00M };
            for (int i = 0; i < 4; i++) {
                prod[i] = dbClean.Products.First(p => p.ProductID == i + 1);
                prod[i].UnitPrice = price[i];
            }
            prod[0].UnitsInStock = 39;
            prod[0].UnitsOnOrder = 0;
            dbClean.SubmitChanges();
        }

        //OptimisticConcurrencyConflict
        private void WriteConflictDetails(IEnumerable<ObjectChangeConflict> conflicts) {
            //OptimisticConcurrencyConflict
            foreach (ObjectChangeConflict conflict in conflicts)
            {
                DescribeConflict(conflict);
            }
        }

        private void DescribeConflict(ObjectChangeConflict conflict) {
            Product prod = (Product)conflict.Object;
            Console.WriteLine("Optimistic Concurrency Conflict in product {0}", prod.ProductID);
            //OptimisticConcurrencyMemberConflict
            foreach (MemberChangeConflict memConflict in conflict.MemberConflicts/*GetMemberConflicts()*/) {
                string name = memConflict.Member.Name;
                string yourUpdate = memConflict.CurrentValue.ToString();
                string original = memConflict.OriginalValue.ToString();
                string theirUpdate = memConflict.DatabaseValue.ToString();
                if (memConflict.IsModified) {
                    Console.WriteLine("'{0}' was updated from {1} to {2} while you updated it to {3}",
                                      name, original, theirUpdate, yourUpdate);
                } else {
                    Console.WriteLine("'{0}' was updated from {1} to {2}, you did not change it.",
                        name, original, theirUpdate);
                }
            }
        }

        [Category("Optimistic Concurrency")]
        [Title("Resolve conflicts: Overwrite current values")]
        [Description("This sample demonstrates how to automatically resolve concurrency conflicts.\r\num"
               + "The 'overwrite current values' option writes the new database values to the client objects.")]
        public void LinqToSqlOptimistic02()
        {
            Northwind otherUser_db = new Northwind(connString);
            db.Log = null;

            Product product = db.Products.First(p => p.ProductID == 1);
            Console.WriteLine("You retrieve the product 1, it costs {0}", product.UnitPrice);
            Console.WriteLine("There are {0} units in stock, {1} units on order", product.UnitsInStock, product.UnitsOnOrder);
            Console.WriteLine();

            Console.WriteLine("Another user changes the price to 22.22 and UnitsInStock to 22");
            Product otherUser_product = otherUser_db.Products.First(p => p.ProductID == 1);
            otherUser_product.UnitPrice = 22.22M;
            otherUser_product.UnitsInStock = 22;
            otherUser_db.SubmitChanges();

            Console.WriteLine("You set the price of product 1 to 1.01 and UnitsOnOrder to 11");
            product.UnitPrice = 1.01M;
            product.UnitsOnOrder = 11;
            try {
                Console.WriteLine("You submit");
                Console.WriteLine();
                db.SubmitChanges();
            } catch (ChangeConflictException) {
                WriteConflictDetails(db.ChangeConflicts); // write changed objects / members to console
                Console.WriteLine();
                Console.WriteLine("Resolve by overwriting current values");
                db.ChangeConflicts.ResolveAll(RefreshMode.OverwriteCurrentValues);
                db.SubmitChanges();
            }
            Console.WriteLine();
            Northwind dbResult = new Northwind(connString);
            Product result = dbResult.Products.First(p => p.ProductID == 1);
            Console.WriteLine("Now product 1 has price={0}, UnitsInStock={1}, UnitsOnOrder={2}",
                result.UnitPrice, result.UnitsInStock, result.UnitsOnOrder);
            Console.WriteLine();
            ResetProducts(); // clean up
        }

        [Category("Optimistic Concurrency")]
        [Title("Resolve conflicts: Keep current values")]
        [Description("This sample demonstrates how to automatically resolve concurrency conflicts.\r\num"
             + "The 'keep current values' option changes everything to the values of this client.")]
        public void LinqToSqlOptimistic03()
        {
            Northwind otherUser_db = new Northwind(connString);
            db.Log = null;

            Product product = db.Products.First(p => p.ProductID == 1);
            Console.WriteLine("You retrieve the product 1, it costs {0}", product.UnitPrice);
            Console.WriteLine("There are {0} units in stock, {1} units on order", product.UnitsInStock, product.UnitsOnOrder);
            Console.WriteLine();

            Console.WriteLine("Another user changes the price to 22.22 and UnitsInStock to 22");
            Product otherUser_product = otherUser_db.Products.First(p => p.ProductID == 1);
            otherUser_product.UnitPrice = 22.22M;
            otherUser_product.UnitsInStock = 22;
            otherUser_db.SubmitChanges();

            Console.WriteLine("You set the price of product 1 to 1.01 and UnitsOnOrder to 11");
            product.UnitPrice = 1.01M;
            product.UnitsOnOrder = 11;
            try {
                Console.WriteLine("You submit");
                Console.WriteLine();
                db.SubmitChanges();
            } catch (ChangeConflictException) {
                WriteConflictDetails(db.ChangeConflicts); // write changed objects / members to console
                Console.WriteLine();
                Console.WriteLine("Resolve by keeping current values");
                db.ChangeConflicts.ResolveAll(RefreshMode.KeepCurrentValues);
                db.SubmitChanges();
            }
            Console.WriteLine();
            Northwind dbResult = new Northwind(connString);
            Product result = dbResult.Products.First(p => p.ProductID == 1);
            Console.WriteLine("Now product 1 has price={0}, UnitsInStock={1}, UnitsOnOrder={2}",
                result.UnitPrice, result.UnitsInStock, result.UnitsOnOrder);
            Console.WriteLine();
            ResetProducts(); // clean up
        }

        [Category("Optimistic Concurrency")]
        [Title("Resolve conflicts: Keep changes")]
        [Description("This sample demonstrates how to automatically resolve concurrency conflicts.\r\num"
           + "The 'keep changes' option keeps all changes from the current user "
           + "and merges changes from other users if the corresponding field was not changed by the current user.")]
        public void LinqToSqlOptimistic04()
        {
            Northwind otherUser_db = new Northwind(connString);
            db.Log = null;

            Product product = db.Products.First(p => p.ProductID == 1);
            Console.WriteLine("You retrieve the product 1, it costs {0}", product.UnitPrice);
            Console.WriteLine("There are {0} units in stock, {1} units on order", product.UnitsInStock, product.UnitsOnOrder);
            Console.WriteLine();

            Console.WriteLine("Another user changes the price to 22.22 and UnitsInStock to 22");
            Product otherUser_product = otherUser_db.Products.First(p => p.ProductID == 1);
            otherUser_product.UnitPrice = 22.22M;
            otherUser_product.UnitsInStock = 22;
            otherUser_db.SubmitChanges();

            Console.WriteLine("You set the price of product 1 to 1.01 and UnitsOnOrder to 11");
            product.UnitPrice = 1.01M;
            product.UnitsOnOrder = 11;
            try {
                Console.WriteLine("You submit");
                Console.WriteLine();
                db.SubmitChanges();
            } catch (ChangeConflictException) {
                WriteConflictDetails(db.ChangeConflicts); // write changed objects / members to console
                Console.WriteLine();
                Console.WriteLine("Resolve by keeping changes");
                db.ChangeConflicts.ResolveAll(RefreshMode.KeepChanges);
                db.SubmitChanges();
            }
            Console.WriteLine();
            Northwind dbResult = new Northwind(connString);
            Product result = dbResult.Products.First(p => p.ProductID == 1);
            Console.WriteLine("Now product 1 has price={0}, UnitsInStock={1}, UnitsOnOrder={2}",
                result.UnitPrice, result.UnitsInStock, result.UnitsOnOrder);
            Console.WriteLine();
            ResetProducts(); // clean up
        }

        [Category("Optimistic Concurrency")]
        [Title("Custom resolve rule")]
        [Description("Demonstrates using MemberConflict.Resolve to write a custom resolve rule.\r\num")]
        public void LinqToSqlOptimistic05()
        {
            Northwind otherUser_db = new Northwind(connString);
            db.Log = null;

            Product product = db.Products.First(p => p.ProductID == 1);
            Console.WriteLine("You retrieve the product 1, it costs {0}", product.UnitPrice);
            Console.WriteLine("There are {0} units in stock, {1} units on order", product.UnitsInStock, product.UnitsOnOrder);
            Console.WriteLine();

            Console.WriteLine("Another user changes the price to 22.22 and UnitsOnOrder to 2");
            Product otherUser_product = otherUser_db.Products.First(p => p.ProductID == 1);
            otherUser_product.UnitPrice = 22.22M;
            otherUser_product.UnitsOnOrder = 2;
            otherUser_db.SubmitChanges();

            Console.WriteLine("You set the price of product 1 to 1.01 and UnitsOnOrder to 11");
            product.UnitPrice = 1.01M;
            product.UnitsOnOrder = 11;
            bool needsSubmit = true;
            while (needsSubmit) {
                try {
                    Console.WriteLine("You submit");
                    Console.WriteLine();
                    needsSubmit = false;
                    db.SubmitChanges();
                } catch (ChangeConflictException) {
                    needsSubmit = true;
                    WriteConflictDetails(db.ChangeConflicts); // write changed objects / members to console
                    Console.WriteLine();
                    Console.WriteLine("Resolve by higher price / order");
                    foreach (ObjectChangeConflict conflict in db.ChangeConflicts) {
                        conflict.Resolve(RefreshMode.KeepChanges);
                        foreach (MemberChangeConflict memConflict in conflict.MemberConflicts) {
                            if (memConflict.Member.Name == "UnitPrice") {
                                //always use the highest price
                                decimal theirPrice = (decimal)memConflict.DatabaseValue;
                                decimal yourPrice = (decimal)memConflict.CurrentValue;
                                memConflict.Resolve(Math.Max(theirPrice, yourPrice));
                            } else if (memConflict.Member.Name == "UnitsOnOrder") {
                                //always use higher order
                                short theirOrder = (short)memConflict.DatabaseValue;
                                short yourOrder = (short)memConflict.CurrentValue;
                                memConflict.Resolve(Math.Max(theirOrder, yourOrder));
                            }
                        }
                    }
                }
            }
            Northwind dbResult = new Northwind(connString);
            Product result = dbResult.Products.First(p => p.ProductID == 1);
            Console.WriteLine("Now product 1 has price={0}, UnitsOnOrder={1}",
                result.UnitPrice, result.UnitsOnOrder);
            Console.WriteLine();
            ResetProducts(); // clean up
        }

        [Category("Optimistic Concurrency")]
        [Title("Submit with FailOnFirstConflict")]
        [Description("Submit(FailOnFirstConflict) throws an Optimistic Concurrency Exception when the first conflict is detected.\r\num"
           + "Only one exception is handled at a time, you have to submit for each conflict.")]
        public void LinqToSqlOptimistic06()
        {
            db.Log = null;
            Northwind otherUser_db = new Northwind(connString);

            // you load 3 products
            Product[] prod = db.Products.OrderBy(p => p.ProductID).Take(3).ToArray();
            for (int i = 0; i < 3; i++) {
                Console.WriteLine("You retrieve the product {0}, it costs {1}", i + 1, prod[i].UnitPrice);
            }
            // other user changes these products
            Product[] otherUserProd = otherUser_db.Products.OrderBy(p => p.ProductID).Take(3).ToArray();
            for (int i = 0; i < 3; i++) {
                decimal otherPrice = (i + 1) * 111.11M;
                Console.WriteLine("Other user changes the price of product {0} to {1}", i + 1, otherPrice);
                otherUserProd[i].UnitPrice = otherPrice;
            }
            otherUser_db.SubmitChanges();
            Console.WriteLine("Other user submitted changes");

            // you change your loaded products
            for (int i = 0; i < 3; i++) {
                decimal yourPrice = (i + 1) * 1.01M;
                Console.WriteLine("You set the price of product {0} to {1}", i + 1, yourPrice);
                prod[i].UnitPrice = yourPrice;
            }

            // submit
            bool needsSubmit = true;
            while (needsSubmit) {
                try {
                    Console.WriteLine("======= You submit with FailOnFirstConflict =======");
                    needsSubmit = false;
                    db.SubmitChanges(ConflictMode.FailOnFirstConflict);
                } catch (ChangeConflictException) {
                    foreach (ObjectChangeConflict conflict in db.ChangeConflicts) {
                        DescribeConflict(conflict); // write changes to console
                        Console.WriteLine("Resolve conflict with KeepCurrentValues");
                        conflict.Resolve(RefreshMode.KeepCurrentValues);
                    }
                    needsSubmit = true;
                }
            }
            Northwind dbResult = new Northwind(connString);
            for (int i = 0; i < 3; i++) {
                Product result = dbResult.Products.First(p => p.ProductID == i + 1);
                Console.WriteLine("Now the product {0} has price {1}", i + 1, result.UnitPrice);
            }
            ResetProducts(); // clean up
        }

        [Category("Optimistic Concurrency")]
        [Title("Submit with ContinueOnConflict")]
        [Description("Submit(ContinueOnConflict) collects all concurrency conflicts and throws an exception when the last conflict is detected.\r\num"
           + "All conflicts are handled in one catch statement.\r\num"
           + "It is still possible that another user updated the same objects before this update, so it is possible that another Optimistic Concurrency Exception is thrown which would need to be handled again.")]
        public void LinqToSqlOptimistic07()
        {
            db.Log = null;
            Northwind otherUser_db = new Northwind(connString);

            // you load 3 products
            Product[] prod = db.Products.OrderBy(p => p.ProductID).Take(3).ToArray();
            for (int i = 0; i < 3; i++) {
                Console.WriteLine("You retrieve the product {0}, it costs {1}", i + 1, prod[i].UnitPrice);
            }
            // other user changes these products
            Product[] otherUserProd = otherUser_db.Products.OrderBy(p => p.ProductID).Take(3).ToArray();
            for (int i = 0; i < 3; i++) {
                decimal otherPrice = (i + 1) * 111.11M;
                Console.WriteLine("Other user changes the price of product {0} to {1}", i + 1, otherPrice);
                otherUserProd[i].UnitPrice = otherPrice;
            }
            otherUser_db.SubmitChanges();
            Console.WriteLine("Other user submitted changes");

            // you change your loaded products
            for (int i = 0; i < 3; i++) {
                decimal yourPrice = (i + 1) * 1.01M;
                Console.WriteLine("You set the price of product {0} to {1}", i + 1, yourPrice);
                prod[i].UnitPrice = yourPrice;
            }

            // submit
            bool needsSubmit = true;
            while (needsSubmit) {
                try {
                    Console.WriteLine("======= You submit with ContinueOnConflict =======");
                    needsSubmit = false;
                    db.SubmitChanges(ConflictMode.ContinueOnConflict);
                } catch (ChangeConflictException) {
                    foreach (ObjectChangeConflict conflict in db.ChangeConflicts) {
                        DescribeConflict(conflict); // write changes to console
                        Console.WriteLine("Resolve conflict with KeepCurrentValues");
                        conflict.Resolve(RefreshMode.KeepCurrentValues);
                    }
                    needsSubmit = true;
                }
            }
            Northwind dbResult = new Northwind(connString);
            for (int i = 0; i < 3; i++) {
                Product result = dbResult.Products.First(p => p.ProductID == i + 1);
                Console.WriteLine("Now the product {0} has price {1}", i + 1, result.UnitPrice);
            }
            ResetProducts(); // clean up
        }


      
        [Category("Extensibility Partial Methods")]
        [Title("Update with OnValidate")]
        [Description("This sample overrides the OnValidate partial method for Order Class. When an Order is being updated, it validates"
        +" ShipVia cannot be greater than 100 else exception throws and no update is sent to database.")]
        public void LinqToSqlExtensibility01()
        {

            var order = (from o in db.Orders
                         where o.OrderID == 10355
                         select o).First();
            ObjectDumper.Write(order);
            Console.WriteLine();

            Console.WriteLine("***** Update Order to set ShipVia to 120 and submit changes ******");
            Console.WriteLine();

            order.ShipVia = 120;
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine("****** Catch exception throw by OnValidate() ******");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine();
            Console.WriteLine("****** verify that order's ShipVia didn't get changed in db. ******");
            Northwind db2 = new Northwind(connString);
            var order2 = (from o in db2.Orders
                          where o.OrderID == 10355
                          select o).First();

            ObjectDumper.Write(order2);

             
        }

       

        public override void InitSample() {
            ClearDBCache();
            SetLogging(true);
        }

        public void ClearDBCache() {
            // Creates a new Northwind object to start fresh with an empty object cache
            // Active ADO.NET connection will be reused by new Northwind object

            TextWriter oldLog;
            if (db == null)
                oldLog = null;
            else
                oldLog = db.Log;

            db = new Northwind(connString) { Log = oldLog };
        }

        public void SetLogging(bool logging) {
            if (logging) {
                db.Log = this.OutputStreamWriter;
            }
            else {
                db.Log = null;
            }
        }

        public override void HandleException(Exception e) {
            Console.WriteLine("Unable to connect to the Northwind database on SQL Server instance.");
            Console.WriteLine("Try restarting SQL Server or your computer.");
            Console.WriteLine();
            Console.WriteLine("If the problem persists, see the Troubleshooting section of the Readme for tips.");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Exception:");
            base.HandleException(e);
        }

        public class Name {
            public string FirstName;
            public string LastName;
        }
    }
}
