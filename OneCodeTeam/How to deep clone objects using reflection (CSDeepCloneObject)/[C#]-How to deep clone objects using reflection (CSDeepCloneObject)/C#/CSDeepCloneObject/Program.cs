/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSDeepCloneObject
Copyright (c) Microsoft Corporation.

This sample demonstrates how to implement deep clone between objects in C# 
using reflection. 
We can use the MemberwiseClone to get a copy, but the MemberwiseClone method 
creates a shallow copy by creating a new object, and then copying the non-static 
fields of the current object to the new object. If a field is a value type, a   
bit-by-bit copy of the field is performed. If a field is a reference type, the  
reference is copied but the referred object is not. 
In this sample, we make use metadata information to clone a new object and drill 
down each field, ultimately, copy this field. 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;

namespace CSDeepCloneObject
{
    class Program
    {
        static void Main(string[] args)
        {
            // This method demonstrates the different clones of the Employee class.
            CloneEmployee();

            // This method demonstrates the different clones of the Customer struct.
            CloneCustomer();
        }

        /// <summary>
        /// This method demonstrates the difference between the shallow clone and 
        /// the deep clone of the Employee class.
        /// </summary> 
        public static void CloneEmployee()
        {
            Address address = new Address { City = "ShangHai" };
            Employee originalEmployee = new Employee { Id = 101, Name = "Gail Erickson", Address = address };

            // Get a shallow copy of the originalEmployee and set the new values in the copy.
            Employee shallowCloneEmployee = originalEmployee.ShallowCopy();
            shallowCloneEmployee.Id = 102;
            shallowCloneEmployee.Name = "Dylan Miller";
            shallowCloneEmployee.Address.City = "RedMond";  // Change the shallow copy's address information.

            // Show the information of originalEmployee and the shallow copy.
            Console.WriteLine("It is the shallow clone of the Employee class.");
            Console.WriteLine("{0,-5} {1,-25} {2}",
                originalEmployee.Id, originalEmployee.Name, originalEmployee.Address.City);
            Console.WriteLine("{0,-5} {1,-25} {2}",
                shallowCloneEmployee.Id, shallowCloneEmployee.Name, shallowCloneEmployee.Address.City);
            Console.WriteLine();

            // Get a deep copy of the originalEmployee and set the new values in the copy.
            address.City = "ShangHai";
            Employee deepCloneEmployee = DeepCloneHelper.DeepClone(originalEmployee);
            deepCloneEmployee.Id = 102;
            deepCloneEmployee.Name = "Dylan Miller";
            deepCloneEmployee.Address.City = "RedMond";  // Change the deep copy's address information.

            // Show the information of originalEmployee and the deep copy.
            Console.WriteLine("It is the deep clone of the Employee class.");
            Console.WriteLine("{0,-5} {1,-25} {2}",
                originalEmployee.Id, originalEmployee.Name, originalEmployee.Address.City);
            Console.WriteLine("{0,-5} {1,-25} {2}",
                deepCloneEmployee.Id, deepCloneEmployee.Name, deepCloneEmployee.Address.City);
            Console.WriteLine();
        }

        /// <summary>
        /// This method demonstrates the difference between the shallow clone and 
        /// the deep clone of the Customer struct.
        /// </summary>
        public static void CloneCustomer()
        {
            Address address = new Address { City = "Los Angeles" }; 
            Customer originalCustomer = new Customer { Id = 201, Name = "Kevin Brown", Address = address };

            // Get a shallow copy of the originalCustomer and set the new values in the copy.
            Customer shallowCloneCustomer = originalCustomer;
            shallowCloneCustomer.Id = 202;
            shallowCloneCustomer.Name = "John Wood";
            shallowCloneCustomer.Address.City = "Boston";  // Change the shallow copy's address information.

            // Show the information of originalCustomer and the shallow copy.
            Console.WriteLine("It is the shallow clone of the Customer struct.");
            Console.WriteLine("{0,-5} {1,-25} {2}",
                originalCustomer.Id, originalCustomer.Name, originalCustomer.Address.City);
            Console.WriteLine("{0,-5} {1,-25} {2}",
                shallowCloneCustomer.Id, shallowCloneCustomer.Name, originalCustomer.Address.City);
            Console.WriteLine();

            // Get a deep copy of the originalCustomer and set the new values in the copy.
            address.City = "Los Angeles";
            Customer deepCloneCustomer = DeepCloneHelper.DeepClone(originalCustomer);
            deepCloneCustomer.Id = 202;
            deepCloneCustomer.Name = "John Wood";
            deepCloneCustomer.Address.City = "Boston";  // Change the deep copy's address information.

            // Show the information of originalCustomer and the deep copy.
            Console.WriteLine("It is the deep clone of the Customer struct.");
            Console.WriteLine("{0,-5} {1,-25} {2}",
                originalCustomer.Id, originalCustomer.Name, originalCustomer.Address.City);
            Console.WriteLine("{0,-5} {1,-25} {2}",
                deepCloneCustomer.Id, deepCloneCustomer.Name, deepCloneCustomer.Address.City);
            Console.WriteLine();
        }
    }
}
