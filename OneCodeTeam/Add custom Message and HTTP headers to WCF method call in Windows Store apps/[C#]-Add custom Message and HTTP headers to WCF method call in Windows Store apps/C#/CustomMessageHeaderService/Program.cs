/****************************** Module Header ******************************\
 * Module Name:  Program.cs
 * Project:      CustomMessageHeaderService
 * Copyright (c) Microsoft Corporation.
 * 
 *  This is a server end application listening on a port.
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
using System.ServiceModel;
using System.ServiceModel.Description;

namespace CustomMessageHeaderService
{
    class Program
    {
        // WCF service's address.
        static string strBaseAddress = "http://localhost:8001";
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri(strBaseAddress);

            // Create a ServiceHost instance with the specific service address.
            ServiceHost localHost = new ServiceHost(typeof(CalculatorService), baseAddress);

            try
            {
                // Release the metadata.
                ServiceMetadataBehavior serviceMetadataBehavior = new ServiceMetadataBehavior();
                serviceMetadataBehavior.HttpGetEnabled = true;
                localHost.Description.Behaviors.Add(serviceMetadataBehavior);
                localHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

                // Specify the type of the service contract ICalculator and the binding type WSHttpBinding.
                localHost.AddServiceEndpoint(typeof(ICalculatorService), new BasicHttpBinding(), "");

                // Start the Service.
                localHost.Open();
                Console.WriteLine("Service is listening on {0}", baseAddress.AbsoluteUri);
                Console.ReadLine();
                localHost.Close();
            }
            catch (Exception oEx)
            {
                Console.WriteLine("Exception: {0}", oEx.Message);
            }
        }
    }
}
