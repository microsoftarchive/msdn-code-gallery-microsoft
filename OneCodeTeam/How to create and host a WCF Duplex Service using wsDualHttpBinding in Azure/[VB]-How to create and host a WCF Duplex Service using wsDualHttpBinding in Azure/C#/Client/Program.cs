/****************************** Module Header ******************************\
* Module Name: Program.cs
* Project:     Client
* Copyright (c) Microsoft Corporation.
* 
* 
* WSDualHttpBinding supports duplex services. A duplex service is a service that uses duplex message patterns.
* These patterns provide the ability for a service to communicate back to the client via a callback.
*  
* This is the client side programe. It's used to invoke the WCF service on Azure workrole. 
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Threading;
namespace Client
{
    class Program:CalculatorServiceClient.IServiceCalculateCallback
    {
        static string strSymbol = "{0:C4}";

        static void Main(string[] args)
        {

            InstanceContext instance = new InstanceContext(new Program());
            CalculatorServiceClient.ServiceCalculateClient client = new CalculatorServiceClient.ServiceCalculateClient(instance);

            try
            {
                WSDualHttpBinding binding = client.Endpoint.Binding as WSDualHttpBinding;
                binding.ClientBaseAddress = new Uri("http://localhost:8081/client");

                double dbeOneParameter = 0;
                double dbeTwoParameter = 0;

                Console.WriteLine("Input the first parameter.");
                inputParameter(out dbeOneParameter);

                Console.WriteLine("Input the second parameter.");
                inputParameter(out dbeTwoParameter);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("Starting to call WCF Service method SumByInputValue.");
                client.SumByInputValue(dbeOneParameter, dbeTwoParameter, strSymbol);
                Console.WriteLine("Calling WCF Service method SumByInputValue finished.");

                Console.WriteLine();
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Starting to call WCF Service method SumByInputValueOneway.");
                client.SumByInputValueOneway(dbeOneParameter, dbeTwoParameter, strSymbol);
                Console.WriteLine("Calling WCF Service method SumByInputValueOneway finished.");
            }
            catch (Exception ex)
            {

            }
            Console.ReadLine();
            Console.ResetColor();
        }

        static void inputParameter(out double dbeParameter)
        {
            if (double.TryParse(Console.ReadLine(), out dbeParameter))
            {
            }
            else
            {
                Console.WriteLine(" Input the parameter is wrong! Please input the parameter again. ");
                dbeParameter = 0;
                inputParameter(out dbeParameter);
            }
        }


        public void DisplayResultByOption(string strSymbol, double dbeSumValue)
        {
            Console.Write("Call WCFCallback method DisplayResultByOption: ");
            Console.WriteLine(string.Format(strSymbol, dbeSumValue)+".");
        }

        public void DisPlayResultByOptionOneWay(string strSymbol, double dbeSumValue)
        {
            Console.Write("Call WCFCallback method DisPlayResultByOptionOneWay:");
            Console.WriteLine(string.Format(strSymbol, dbeSumValue) + ".");
        }
    }
}
