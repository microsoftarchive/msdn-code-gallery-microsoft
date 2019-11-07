using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CScallASMXlocalWsdl
{
    //****************************** Module Header ******************************\
    //Module Name:    Program.cs
    //Project:        CScallASMXlocalWsdl
    //Copyright (c) Microsoft Corporation

    // The project illustrates how to check whether a file is in use or not.

    //This source is subject to the Microsoft Public License.
    //See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
    //All other rights reserved.

    //*****************************************************************************/
    class Program
    {
        static void Main(string[] args)
        {
            using (var webService = new WebReference.WebService1())
            {
                // invoke the web method
                // pass different parameter (integer) to test the application
                var result = webService.GetData(10);

                Console.WriteLine(result);
            }
            Console.ReadLine();
        }
    }
}
