using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel;
using Microsoft.SharePoint;

namespace WCF_CustomServiceApplication.Server
{
    //This class enables SharePoint to create multiple service
    //hosts for the service application
    internal sealed class DayNamerServiceHostFactory : ServiceHostFactory
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            ServiceHost serviceHost = new ServiceHost(typeof(DayNamerServiceApplication), baseAddresses);

            // configure the service for claims
            serviceHost.Configure(SPServiceAuthenticationMode.Claims);

            return serviceHost;
        }
    }
}