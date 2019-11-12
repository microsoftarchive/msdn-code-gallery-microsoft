using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
//This namespace is used for the SPJobDefinition class
using Microsoft.SharePoint.Administration;
//This is the main WCF namespace
using System.ServiceModel;

namespace WCF_CallServiceFromTimerJob
{
    class WCF_CallingTimerJob : SPJobDefinition
    {
        //Constructors
        public WCF_CallingTimerJob()
            : base()
        {

        }

        public WCF_CallingTimerJob(string jobName, SPService service, SPServer server, SPJobLockType targetType)
            : base(jobName, service, server, targetType)
        {

        }

        public WCF_CallingTimerJob(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "WCF Calling Timer Job";
        }

        public override void Execute(Guid targetInstanceId)
        {
            //Get the Web Application in which this Timer Job runs
            SPWebApplication webApp = this.Parent as SPWebApplication;
            SPContentDatabase contentDB = webApp.ContentDatabases[targetInstanceId];
            SPSiteCollection timerSiteCollection = webApp.ContentDatabases[targetInstanceId].Sites;
            SPList timerJobList = null;
            foreach (SPSite site in timerSiteCollection)
            {
                timerJobList = site.RootWeb.Lists.TryGetList("Announcements");
                if (timerJobList != null)
                {
                    SPListItem newItem = timerJobList.Items.Add();
                    newItem["Title"] = "Today is " + getToday();
                    newItem.Update();
                }
            }
        }

        //THis method calls the WCF service
        private string getToday()
        {
            //I used svcutil.exe to generate the proxy class for the service
            //in the generatedDayNamerProxy.cs file. I'm going to configure this
            //in code by using a channel factory.

            string today = string.Empty;
            //Create the channel factory with a Uri, binding and endpoint
            Uri serviceUri = new Uri("http://localhost:8088/WCF_ExampleService/Service/DayNamerService");
            WSHttpBinding serviceBinding = new WSHttpBinding();
            EndpointAddress dayNamerEndPoint = new EndpointAddress(serviceUri);
            ChannelFactory<httpWCF_ExampleService> channelFactory = new ChannelFactory<httpWCF_ExampleService>(serviceBinding, dayNamerEndPoint);
            //Create a channel
            httpWCF_ExampleService dayNamer = channelFactory.CreateChannel();
            //Now we can call the TodayIs method
            today = dayNamer.TodayIs();
            //close the factory with all its channels
            channelFactory.Close();
            //Return the name
            return today;
        }
    }
}
