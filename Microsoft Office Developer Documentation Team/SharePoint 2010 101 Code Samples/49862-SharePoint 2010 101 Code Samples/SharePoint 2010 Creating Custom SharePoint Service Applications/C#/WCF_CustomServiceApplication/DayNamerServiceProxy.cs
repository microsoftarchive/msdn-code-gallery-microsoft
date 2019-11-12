using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace WCF_CustomServiceApplication.Client
{
    //This class runs on the Web-Front End servers and 
    //connects to the WCF service on the application servers
    //This is why web parts etc. don't need to know where
    //the service application actually runs. Make sure that
    //the SupportedServiceApplication GUID matches the one
    //in the DayNamerServiceApplication class.
    [System.Runtime.InteropServices.Guid("841D1124-83CD-483A-9043-09B7A1CB7505")]
    [SupportedServiceApplication("4D45F3BB-5996-4EF1-855E-BD68D918A3A6", 
                                "1.0.0.0", 
                                typeof(DayNamerServiceApplicationProxy))]
    public class DayNamerServiceProxy: SPIisWebServiceProxy, IServiceProxyAdministration
    {
        #region Constructors

        public DayNamerServiceProxy()
            : base() { }
        public DayNamerServiceProxy(SPFarm farm)
            : base(farm) { }

        #endregion

        public SPServiceApplicationProxy CreateProxy(Type serviceApplicationProxyType, string name, Uri serviceApplicationUri, SPServiceProvisioningContext provisioningContext)
        {
            if (serviceApplicationProxyType != typeof(DayNamerServiceApplicationProxy))
                throw new NotSupportedException();

            return new DayNamerServiceApplicationProxy(name, this, serviceApplicationUri);
        }

        public SPPersistedTypeDescription GetProxyTypeDescription(Type serviceApplicationProxyType)
        {
            return new SPPersistedTypeDescription("Day Namer Service Proxy", "Custom service application proxy providing simple day names.");
        }

        public Type[] GetProxyTypes()
        {
            return new Type[] { typeof(DayNamerServiceApplicationProxy) };
        }
    }
}
