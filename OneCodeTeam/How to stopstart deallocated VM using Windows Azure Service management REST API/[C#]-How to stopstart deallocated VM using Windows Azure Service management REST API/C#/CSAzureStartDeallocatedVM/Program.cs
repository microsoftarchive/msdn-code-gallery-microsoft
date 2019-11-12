/****************************** Module Header ******************************\
* Module Name: Program.cs
* Project:     CSAzureStartDeallocatedVM
* Copyright (c) Microsoft Corporation.
* 
* This sample will show you how to stop VM(Deallocated) and start it.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 * \***************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace CSAzureStartDeallocatedVM
{
    class Program
    {

        public static string SubscriptionID = "<Subscription ID>";

        // A VM need a host service, you can create it in your Azure portal.
        public static string VMName = "<VM Name>";

        // You need to make sure this certificate is in your Azure management certificate pool.
        // And it's also in your local computer personal certificate pool.
        public static string CertificateThumbprint = "<Certificate Thumbprint>";
        public static X509Certificate2 Certificate;

        static void Main(string[] args)
        {
            X509Store certificateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certificateStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certs = certificateStore.Certificates.Find(
                X509FindType.FindByThumbprint,
                CertificateThumbprint,
                false);
            if (certs.Count == 0)
            {
                Console.WriteLine("Can't find the certificate in your local computer.");
                Console.ReadKey();
                return;
            }
            else
            {
                Certificate = certs[0];
            }

            //StartVirtualMachine(SubscriptionID, Certificate, VMName, VMName, VMName);
            StopVirtualMachine(SubscriptionID, Certificate, VMName, VMName, VMName, false);

        }

        static void StartVirtualMachine(string subscriptionID, X509Certificate2 cer, string serviceName, string deploymentsName, string vmName)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri("https://management.core.windows.net/" + SubscriptionID
            + "/services/hostedservices/" + serviceName + "/deployments/" + deploymentsName + "/roleinstances/" + vmName + "/Operations"));

            request.Method = "POST";
            request.ClientCertificates.Add(Certificate);
            request.ContentType = "application/xml";
            request.Headers.Add("x-ms-version", "2013-06-01");

            // Add body to the request
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("..\\..\\StartVM.xml");

            Stream requestStream = request.GetRequestStream();
            StreamWriter streamWriter = new StreamWriter(requestStream, System.Text.UTF8Encoding.UTF8);
            xmlDoc.Save(streamWriter);

            streamWriter.Close();
            requestStream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                Console.WriteLine("Operation succeed!");
                Console.ReadKey();
            }
            catch (WebException ex)
            {

                Console.Write(ex.Response.Headers.ToString());
                Console.Read();
            }
        }

        static void StopVirtualMachine(string subscriptionID, X509Certificate2 cer, string serviceName,string deploymentsName,string vmName, bool Deallocated)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri("https://management.core.windows.net/" + subscriptionID
                + "/services/hostedservices/" + serviceName + "/deployments/" + deploymentsName + "/roleinstances/" + vmName + "/Operations"));

            request.Method = "POST";
            request.ClientCertificates.Add(cer);
            request.ContentType = "application/xml";
            request.Headers.Add("x-ms-version", "2013-06-01");

            //Add body to the reqeust 
            XmlDocument xmlDoc = new XmlDocument();
            if (Deallocated)
            {
                xmlDoc.Load("..\\..\\StopVM_Deallocated.xml");
            }
            else
            {
                xmlDoc.Load("..\\..\\StopVM.xml");  
            }

            Stream requestStream = request.GetRequestStream();
            StreamWriter streamWriter = new StreamWriter(requestStream, System.Text.UTF8Encoding.UTF8);
            xmlDoc.Save(streamWriter);

            streamWriter.Close();
            requestStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
                Console.WriteLine("Operation succeed!");
                Console.ReadKey();
            }
            catch (WebException ex)
            {
                
                 Console.Write(ex.Response.Headers.ToString());
                Console.Read();
            }

        }
    }
}
