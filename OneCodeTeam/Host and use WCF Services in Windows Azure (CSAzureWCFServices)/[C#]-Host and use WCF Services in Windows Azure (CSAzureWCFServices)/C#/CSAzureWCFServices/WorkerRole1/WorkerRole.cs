using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.ServiceModel;
using WCFContract;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private ServiceHost serviceHost;
      

        public override void Run()
        {
           
            this.StartWCFService();
            while (true)
            {
                Thread.Sleep(10000);
                Trace.WriteLine("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }

        /// <summary>
        /// Starts the service host object for the internal 
        /// and external endpoints of the wcf service.
        /// </summary>
        /// <param name="retries">Specifies the number of retries to 
        /// start the service in case of failure.</param>
        private void StartWCFService()
        {
          
            Trace.TraceInformation("Starting WCF service host...");
            this.serviceHost = new ServiceHost(typeof(WCFService));
  
            // Use NetTcpBinding with no security
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            
            // Define an external endpoint for client traffic
            RoleInstanceEndpoint externalEndPoint =
                RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["External"];
            this.serviceHost.AddServiceEndpoint(
               typeof(IContract),
               binding,
               String.Format("net.tcp://{0}/External", externalEndPoint.IPEndpoint));

            // Define an internal endpoint for inter-role traffic
            RoleInstanceEndpoint internalEndPoint =
                RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["Internal"];
            this.serviceHost.AddServiceEndpoint(
               typeof(IContract),
               binding,
               String.Format("net.tcp://{0}/Internal", internalEndPoint.IPEndpoint));
        

            try
            {
                this.serviceHost.Open();
                Trace.TraceInformation("WCF service host started successfully.");
            }
            catch (TimeoutException timeoutException)
            {
                Trace.TraceError("The service operation timed out. {0}",
                                 timeoutException.Message);
            }
            catch (CommunicationException communicationException)
            {
                Trace.TraceError("Could not start WCF service host. {0}",
                                 communicationException.Message);
            }
        }
    }
}
