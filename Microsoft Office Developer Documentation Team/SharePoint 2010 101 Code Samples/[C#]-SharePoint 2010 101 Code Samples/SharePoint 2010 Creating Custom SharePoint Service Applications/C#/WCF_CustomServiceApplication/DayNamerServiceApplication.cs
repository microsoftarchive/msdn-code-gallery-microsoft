using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

namespace WCF_CustomServiceApplication.Server
{
    
    //The SPIisWebServiceApplication class runs on Application servers
    //and holds references to all the endpoints. It is also persisted
    //in the Config database. Finally it's where you write code that 
    //implements the service application's functionality. Make sure the 
    //GUID below matches the one referred to in your service proxy class
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        IncludeExceptionDetailInFaults = true)]
    [System.Runtime.InteropServices.Guid("4D45F3BB-5996-4EF1-855E-BD68D918A3A6")]
    public class DayNamerServiceApplication : SPIisWebServiceApplication, IDayNamerContract
    {
        [Persisted]
        private int _settings;
        public int Setting
        {
            get { return _settings; }
            set { _settings = value; }
        }

        #region Constructors

        public DayNamerServiceApplication()
            : base() { }
        
        private DayNamerServiceApplication(string name, DayNamerService service, SPIisWebServiceApplicationPool appPool)
            : base(name, service, appPool) { }
        
        #endregion

        public static DayNamerServiceApplication Create(string name, DayNamerService service, SPIisWebServiceApplicationPool appPool)
        {
            //This method creates the custom service application

            #region validation checks

            if (name == null) throw new ArgumentNullException("name");
            if (service == null) throw new ArgumentNullException("service");
            if (appPool == null) throw new ArgumentNullException("appPool");
            
            #endregion

            //Create the service application
            DayNamerServiceApplication serviceApplication = new DayNamerServiceApplication(name, service, appPool);
            serviceApplication.Update();

            //Register the supported endpoints
            serviceApplication.AddServiceEndpoint("http", SPIisWebServiceBindingType.Http);
            serviceApplication.AddServiceEndpoint("https", SPIisWebServiceBindingType.Https, "secure");

            return serviceApplication;
        }

        #region service application properties

        protected override string DefaultEndpointName
        {
            get { return "http"; }
        }

        public override string TypeName
        {
            get { return "Day Namer Service Application"; }
        }

        protected override string InstallPath
        {
            get { return SPUtility.GetGenericSetupPath(@"WebServices\DayNamer"); }
        }

        protected override string VirtualPath
        {
            get { return "DayNamer.svc"; }
        }

        public override Guid ApplicationClassId
        {
            //This GUID matches the one for the class above
            get { return new Guid("4D45F3BB-5996-4EF1-855E-BD68D918A3A6"); }
        }

        public override Version ApplicationVersion
        {
            get { return new Version("1.0.0.0"); }
        }

        #endregion

        #region service application admin pages

        //These two properties point to the Manage.aspx page in the ADMIN folder
        //In fact that page is blank for this service application, but these properties are required.
        public override SPAdministrationLink ManageLink
        {
            get
            { return new SPAdministrationLink("/_admin/DayNamerService/Manage.aspx"); }
        }
        public override SPAdministrationLink PropertiesLink
        {
            get
            { return new SPAdministrationLink("/_admin/DayNamerService/Manage.aspx"); }
        }

        #endregion

        #region IDayNamerContract implementation

        //This is where the service application actually does something.
        //In this case, it's very simple by way of demonstration
        
        //This simple method returns today's name
        public string TodayIs()
        {
            //Find out what today is
            DayOfWeek today = DateTime.Today.DayOfWeek;
            //Return today's name
            return today.ToString();
        }

        //This method adds the requested number of days to today and returns the name of that day
        public string TodayAdd(int dayToAdd)
        {
            //Add the requested number to today
            DateTime requestedDateTime = DateTime.Today.AddDays(dayToAdd);
            DayOfWeek requestedDay = requestedDateTime.DayOfWeek;
            //Return the day's name
            return requestedDay.ToString();
        }

        #endregion
    }
}
