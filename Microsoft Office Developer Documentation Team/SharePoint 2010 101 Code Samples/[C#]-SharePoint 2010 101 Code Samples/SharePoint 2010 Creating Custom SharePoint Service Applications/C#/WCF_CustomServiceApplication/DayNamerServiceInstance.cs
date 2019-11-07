using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace WCF_CustomServiceApplication.Server
{
    //This class enables SharePoint to create instances of the 
    //custom service application on one or more application servers
    //in the farm. When a client calls the service application
    //they don't need to know which servers it runs on.
    public class DayNamerServiceInstance : SPIisWebServiceInstance
    {
        #region Constructors

        public DayNamerServiceInstance() { }
        public DayNamerServiceInstance(SPServer server, DayNamerService service)
            : base(server, service) { }

        #endregion

        #region Properties

        public override string DisplayName
        {
            get { return "Day Namer Service"; }
        }
        public override string Description
        {
            get { return "Day Namer service application providing simple day names."; }
        }
        public override string TypeName
        {
            get { return "Day Namer Service"; }
        }

        #endregion
    }
}
