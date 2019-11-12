using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Administration;
using WCF_CustomServiceApplication.Server;
using WCF_CustomServiceApplication.Client;

namespace WCF_CustomServiceApplication.Features.Feature1
{

    [Guid("b8663c97-18cf-44ef-ab9a-42f04d64d7fd")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            // install the service
            DayNamerService service = SPFarm.Local.Services.GetValue<DayNamerService>();
            if (service == null)
            {
                service = new DayNamerService(SPFarm.Local);
                service.Update();
            }

            // install the service proxy
            DayNamerServiceProxy serviceProxy = SPFarm.Local.ServiceProxies.GetValue<DayNamerServiceProxy>();
            if (serviceProxy == null)
            {
                serviceProxy = new DayNamerServiceProxy(SPFarm.Local);
                serviceProxy.Update(true);
            }

            // with service added to the farm, install instance
            DayNamerServiceInstance serviceInstance = new DayNamerServiceInstance(SPServer.Local, service);
            serviceInstance.Update(true);

        }


        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            // uninstall the instance
            DayNamerServiceInstance serviceInstance = SPFarm.Local.Services.GetValue<DayNamerServiceInstance>();
            if (serviceInstance != null)
                SPServer.Local.ServiceInstances.Remove(serviceInstance.Id);

            // uninstall the service proxy
            DayNamerServiceProxy serviceProxy = SPFarm.Local.ServiceProxies.GetValue<DayNamerServiceProxy>();
            if (serviceProxy != null)
            {
                SPFarm.Local.ServiceProxies.Remove(serviceProxy.Id);
            }

            // uninstall the service
            DayNamerService service = SPFarm.Local.Services.GetValue<DayNamerService>();
            if (service != null)
                SPFarm.Local.Services.Remove(service.Id);

        }
    }
}
