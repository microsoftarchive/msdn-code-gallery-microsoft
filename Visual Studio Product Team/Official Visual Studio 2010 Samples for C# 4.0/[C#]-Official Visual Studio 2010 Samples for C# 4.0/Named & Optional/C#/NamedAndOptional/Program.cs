using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NamedAndOptional
{
    // This program demonstrates how to declare a method with named
    // and optional parameters, and how to call the method while
    // making explicit use of the named and optional parameters.
    class Program
    {
        // A method with nameed and optional parameters
        public static void Search(string name, int age = 21, string city = "Pueblo")
        {
            Console.WriteLine("Name = {0} - Age = {1} - City = {2}", name, age, city);
        }

        static void Main(string[] args)
        {
            // Standard call
            Search("Sue", 22, "New York");

            // Omit city parameter
            Search("Mark", 23);

            // Explicitly name the city parameter
            Search("Lucy", city: "Cairo");

            // Use named parameters in reverse order
            Search("Pedro", age: 45, city: "Saigon");
        }
    }
}
