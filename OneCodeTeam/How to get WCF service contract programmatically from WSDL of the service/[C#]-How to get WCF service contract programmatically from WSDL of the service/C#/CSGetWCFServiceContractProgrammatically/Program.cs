/******************************* Module Header ******************************\
* Module Name:  Program.cs
* Project:      CSGetWCFServiceContractProgrammatically
* Copyright (c) Microsoft Corporation.
*
* ​The sample demonstrates how to get WCF Service contract programmatically. 
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
****************************************************************************/
using System;
using System.Web.Services.Description;
using System.Net;
using System.IO;

namespace CSGetWCFServiceContractProgrammatically
{

    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Enter address for the service wsdl");
            string str = Console.ReadLine();
            Console.WriteLine("Address of service is {0}", str);

            // Create a webrequest to the particular address as specified 
            WebRequest request = WebRequest.Create(str);
            request.UseDefaultCredentials = true;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream associated with the response. 
            Stream receiveStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(receiveStream);

            System.Web.Services.Description.ServiceDescription wsdl = new System.Web.Services.Description.ServiceDescription();
            wsdl = ServiceDescription.Read(reader);

            foreach (PortType pt in wsdl.PortTypes)
            {
                Console.WriteLine("ServiceContract : {0}", pt.Name);
                Console.ReadLine();
            }

        }
    }
}

