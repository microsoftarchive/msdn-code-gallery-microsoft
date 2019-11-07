using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace PartitioningOperators
{
    class Program
    {
        static void Main(string[] args)
        {
            LinqSamples samples = new LinqSamples();

            //Comment or uncomment the method calls below to run or not

              samples.Linq20(); // This sample uses Take to get only the first 3 elements of the array

            //samples.Linq21(); // This sample uses Take to get the first 3 orders from customers in Washington

            //samples.Linq22(); // This sample uses Skip to get all but the first four elements of the array

            //samples.Linq23(); // This sample uses Take to get all but the first 2 orders from customers in Washington

            //samples.Linq24(); // This sample uses TakeWhile to return elements starting from the beginning of the array 
                                // until a number is read whose value is not less than 6

            //samples.Linq25(); // This sample uses TakeWhile to return elements starting from the beginning of the array 
                                // until a number is hit that is less than its position in the array

            //samples.Linq26(); // This sample  uses SkipWhile to get the  elements of the array  starting from the first 
                                // element divisible by 3

            //samples.Linq27(); // This sample  uses SkipWhile to get the  elements of the array  starting from the first 
                                // element less than its position
        }

        public class Order
        {
            public int OrderID { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal Total { get; set; }
        }

        public class Customer
        {
            public string CustomerID { get; set; }
            public string CompanyName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Region { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }
            public string Phone { get; set; }
            public string Fax { get; set; }
            public Order[] Orders { get; set; }
        }

        class LinqSamples
        {
            private List<Customer> customerList;

            [Category("Partitioning Operators")]
            [Description("This sample uses Take to get only the first 3 elements of " +
                         "the array.")]
            public void Linq20()
            {
                int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

                var first3Numbers = numbers.Take(3);

                Console.WriteLine("First 3 numbers:");
                foreach (var n in first3Numbers)
                {
                    Console.WriteLine(n);
                }
            }

            [Category("Partitioning Operators")]
            [Description("This sample uses Take to get the first 3 orders from customers " +
                         "in Washington.")]
            public void Linq21()
            {
                List<Customer> customers = GetCustomerList();

                var first3WAOrders = (
                    from cust in customers
                    from order in cust.Orders
                    where cust.Region == "WA"
                    select new { cust.CustomerID, order.OrderID, order.OrderDate })
                    .Take(3);

                Console.WriteLine("First 3 orders in WA:");
                foreach (var order in first3WAOrders)
                {
                    ObjectDumper.Write(order);
                }
            }

            [Category("Partitioning Operators")]
            [Description("This sample uses Skip to get all but the first four elements of " +
                         "the array.")]
            public void Linq22()
            {
                int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

                var allButFirst4Numbers = numbers.Skip(4);

                Console.WriteLine("All but first 4 numbers:");
                foreach (var n in allButFirst4Numbers)
                {
                    Console.WriteLine(n);
                }
            }

            [Category("Partitioning Operators")]
            [Description("This sample uses Take to get all but the first 2 orders from customers " +
                         "in Washington.")]
            public void Linq23()
            {
                List<Customer> customers = GetCustomerList();

                var waOrders =
                    from cust in customers
                    from order in cust.Orders
                    where cust.Region == "WA"
                    select new { cust.CustomerID, order.OrderID, order.OrderDate };

                var allButFirst2Orders = waOrders.Skip(2);

                Console.WriteLine("All but first 2 orders in WA:");
                foreach (var order in allButFirst2Orders)
                {
                    ObjectDumper.Write(order);
                }
            }

            [Category("Partitioning Operators")]
            [Description("This sample uses TakeWhile to return elements starting from the " +
                         "beginning of the array until a number is read whose value is not less than 6.")]
            public void Linq24()
            {
                int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

                var firstNumbersLessThan6 = numbers.TakeWhile(n => n < 6);

                Console.WriteLine("First numbers less than 6:");
                foreach (var num in firstNumbersLessThan6)
                {
                    Console.WriteLine(num);
                }
            }

            [Category("Partitioning Operators")]
            [Description("This sample uses TakeWhile to return elements starting from the " +
                        "beginning of the array until a number is hit that is less than its position " +
                        "in the array.")]
            public void Linq25()
            {
                int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

                var firstSmallNumbers = numbers.TakeWhile((n, index) => n >= index);

                Console.WriteLine("First numbers not less than their position:");
                foreach (var n in firstSmallNumbers)
                {
                    Console.WriteLine(n);
                }
            }

            [Category("Partitioning Operators")]
            [Description("This sample uses SkipWhile to get the elements of the array " +
                        "starting from the first element divisible by 3.")]
            public void Linq26()
            {
                int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

                // In the lambda expression, 'n' is the input parameter that identifies each
                // element in the collection in succession. It is is inferred to be
                // of type int because numbers is an int array.
                var allButFirst3Numbers = numbers.SkipWhile(n => n % 3 != 0);

                Console.WriteLine("All elements starting from first element divisible by 3:");
                foreach (var n in allButFirst3Numbers)
                {
                    Console.WriteLine(n);
                }
            }

            [Category("Partitioning Operators")]
            [Description("This sample uses SkipWhile to get the elements of the array " +
                        "starting from the first element less than its position.")]
            public void Linq27()
            {
                int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

                var laterNumbers = numbers.SkipWhile((n, index) => n >= index);

                Console.WriteLine("All elements starting from first element less than its position:");
                foreach (var n in laterNumbers)
                {
                    Console.WriteLine(n);
                }
            }

            public List<Customer> GetCustomerList()
            {
                if (customerList == null)
                    createLists();

                return customerList;
            }

            private void createLists()
            {
                // Customer/Order data read into memory from XML file using XLinq:
                customerList = (
                    from e in XDocument.Load("Customers.xml").
                              Root.Elements("customer")
                    select new Customer
                    {
                        CustomerID = (string)e.Element("id"),
                        CompanyName = (string)e.Element("name"),
                        Address = (string)e.Element("address"),
                        City = (string)e.Element("city"),
                        Region = (string)e.Element("region"),
                        PostalCode = (string)e.Element("postalcode"),
                        Country = (string)e.Element("country"),
                        Phone = (string)e.Element("phone"),
                        Fax = (string)e.Element("fax"),
                        Orders = (
                            from o in e.Elements("orders").Elements("order")
                            select new Order
                            {
                                OrderID = (int)o.Element("id"),
                                OrderDate = (DateTime)o.Element("orderdate"),
                                Total = (decimal)o.Element("total")
                            })
                            .ToArray()
                    })
                    .ToList();
            }
        }
    }
}
