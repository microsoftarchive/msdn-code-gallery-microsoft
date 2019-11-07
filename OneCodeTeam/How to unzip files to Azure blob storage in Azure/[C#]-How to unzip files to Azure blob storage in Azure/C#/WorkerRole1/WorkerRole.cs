
/****************************** Module Header ******************************\
*Module Name:  WorkerRole.cs
*Project:      CSAzureUnzipFilesToBlobStorage
*Copyright (c) Microsoft Corporation.
* 
*For users with large amounts of unstructured data to store in the cloud, Blob storage offers a cost-effective and 
*scalable solution ,users can store documents ,social data ,images and text etc.
*
*This project  demonstrates how to unzip files to Azure blob storage in Azure.
*Uploading thousands of small files one-by-one is very slow. 
*It would be great if we could upload a zip file to Azure and unzip it directly into blob storage in Azure.
* 
*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
* 
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.ServiceModel;
using UnZipWCFService;
using System.IO;
using Microsoft.Runtime.Hosting;
using System.Text;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

            try
            {
                // Retrieve an object that points to the local storage resource
                LocalResource localResource = RoleEnvironment.GetLocalResource("LocalStorage1");
                UnZipService.strLoacalStorage = localResource.RootPath;
            }
            catch(Exception ex)
            {
                Trace.TraceInformation(ex.Message);
            }
           
            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();
            Trace.TraceInformation("WorkerRole1 has been started");

            try
            { 
                // Start the WCF host.
                ServiceHost host = new ServiceHost(typeof(UnZipService));
                host.Open();
                Trace.TraceInformation("ServiceHost has been started");
            }
            catch(Exception ex)
            {
                Trace.TraceInformation("ServiceHost failed "+ex.Message);
            }

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
