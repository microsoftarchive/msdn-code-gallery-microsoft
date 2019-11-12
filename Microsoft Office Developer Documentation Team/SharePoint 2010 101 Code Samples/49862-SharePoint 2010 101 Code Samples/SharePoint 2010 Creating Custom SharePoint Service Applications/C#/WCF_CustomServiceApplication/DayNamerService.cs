using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using WCF_CustomServiceApplication.Client;

namespace WCF_CustomServiceApplication.Server
{
    //This class creates a link to create a new Day Namer service
    //application in the Manage Service Applications page ribbon
    //in Central Administration. It also creates the service application
    //and its proxy.

    [System.Runtime.InteropServices.Guid("F4E0F76F-96C6-49D3-9475-E2127FEBCC68")]
    public class DayNamerService : SPIisWebService, IServiceAdministration
    {
        public DayNamerService() { }
        public DayNamerService(SPFarm farm)
            : base(farm) { }

        //This method creates the service application
        public SPServiceApplication CreateApplication(string name, Type serviceApplicationType, SPServiceProvisioningContext provisioningContext)
        {
            #region validation
            if (serviceApplicationType != typeof(DayNamerServiceApplication))
                throw new NotSupportedException();
            if (provisioningContext == null)
                throw new ArgumentNullException("provisioningContext");
            #endregion

            // if the service doesn't already exist, create it
            DayNamerServiceApplication serviceApp = this.Farm.GetObject(name, this.Id, serviceApplicationType) as DayNamerServiceApplication;
            if (serviceApp == null)
                serviceApp = DayNamerServiceApplication.Create(name, this, provisioningContext.IisWebServiceApplicationPool);

            return serviceApp;
        }

        //This method creates the service application proxy
        public SPServiceApplicationProxy CreateProxy(string name, SPServiceApplication serviceApplication, SPServiceProvisioningContext provisioningContext)
        {
            #region validation
            if (serviceApplication.GetType() != typeof(DayNamerServiceApplication))
                throw new NotSupportedException();
            if (serviceApplication == null)
                throw new ArgumentNullException("serviceApplication");
            #endregion

            // verify the service proxy exists
            DayNamerServiceProxy serviceProxy = (DayNamerServiceProxy)this.Farm.GetObject(name, this.Farm.Id, typeof(DayNamerServiceProxy));
            if (serviceProxy == null)
                throw new InvalidOperationException("DayNamerServiceProxy does not exist in the farm.");

            // if the app proxy doesn't exist, create it
            DayNamerServiceApplicationProxy applicationProxy = serviceProxy.ApplicationProxies.GetValue<DayNamerServiceApplicationProxy>(name);
            if (applicationProxy == null)
            {
                Uri serviceAppAddress = ((DayNamerServiceApplication)serviceApplication).Uri;
                applicationProxy = new DayNamerServiceApplicationProxy(name, serviceProxy, serviceAppAddress);
            }

            return applicationProxy;
        }

        //This method returns a description of the service application
        public SPPersistedTypeDescription GetApplicationTypeDescription(Type serviceApplicationType)
        {
            if (serviceApplicationType != typeof(DayNamerServiceApplication))
                throw new NotSupportedException();

            return new SPPersistedTypeDescription("Day Namer Service", "Custom service application providing simple date lookups.");
        }

        public Type[] GetApplicationTypes()
        {
            return new Type[] { typeof(DayNamerServiceApplication) };
        }

        public override SPAdministrationLink GetCreateApplicationLink(Type serviceApplicationType)
        {
            //Make sure this links to your own Create.aspx page. 
            return new SPAdministrationLink("/_admin/DayNamerService/Create.aspx");
        }
    }
}
