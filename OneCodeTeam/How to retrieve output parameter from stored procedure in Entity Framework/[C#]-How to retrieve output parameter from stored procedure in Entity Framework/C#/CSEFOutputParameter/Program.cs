/****************************** Module Header ******************************\
* Module Name:    Program.cs
* Project:        CSEFOutputParameter
* Copyright (c) Microsoft Corporation
*
* This sample demonstrates how to use ObjectParameter instance to get the value
* of output parameter.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Data.Objects;

namespace CSEFOutputParameter
{
    class Program
    {
        static void Main(string[] args)
        {
            string name;
            string description;

            do
            {
                Console.WriteLine("Please input a name: ");
                name = Console.ReadLine();

                while(string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Name can't be empty, please input again:");
                    name = Console.ReadLine();
                }

                Console.WriteLine();
                Console.WriteLine("Please input the description: ");
                description = Console.ReadLine();
                Console.WriteLine();

                using (EFDemoDBEntities context = new EFDemoDBEntities())
                {
                    // Create an ObjectParameter instance to retrieve output parameter from stored procedure.
                    ObjectParameter Output = new ObjectParameter("ID", typeof(Int32));
                    context.InsertPerson(name, description, Output);

                    Console.Write("ID: {0}\n", Output.Value);
                    Console.WriteLine("Press any key to continue, press 'Q' to exit.\n");
                }
            }
            while (Console.ReadKey(true).Key != ConsoleKey.Q);
        }
    }
}
