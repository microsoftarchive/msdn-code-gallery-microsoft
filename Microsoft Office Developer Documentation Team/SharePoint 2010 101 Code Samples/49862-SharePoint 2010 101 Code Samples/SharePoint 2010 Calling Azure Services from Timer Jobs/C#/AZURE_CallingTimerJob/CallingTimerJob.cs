using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.ServiceModel;

namespace AZURE_CallingTimerJob
{
    //This timer job calls the DayInfoService.svc WCF service in Windows Azure
    //Make sure you package and publish the service in your Window Azure account
    //before you run this web part.
    public class CallingTimerJob : SPJobDefinition
    {

        //Constructors
        public CallingTimerJob()
            : base()
        {

        }

        public CallingTimerJob(string jobName, SPService service, SPServer server, SPJobLockType targetType)
            : base(jobName, service, server, targetType)
        {

        }

        public CallingTimerJob(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.ContentDatabase)
        {
            this.Title = "Azure Calling Timer Job";
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

        //This method calls the WCF service hosted in Azure
        private string getToday()
        {
            //I used svcutil.exe to generate the proxy class for the service
            //in the generatedDayNamerProxy.cs file. I'm going to configure this
            //in code by using a channel factory.

            string today = string.Empty;
            //Create the channel factory with a Uri, binding and endpoint
            //Change the Uri to match the location in Azure where you published the service
            Uri serviceUri = new Uri("http://daynamercs.cloudapp.net/dayinfoservice.svc");
            BasicHttpBinding serviceBinding = new BasicHttpBinding();
            EndpointAddress dayNamerEndPoint = new EndpointAddress(serviceUri);
            ChannelFactory<IDayInfo> channelFactory = new ChannelFactory<IDayInfo>(serviceBinding, dayNamerEndPoint);
            //Create a channel
            IDayInfo dayNamer = channelFactory.CreateChannel();
            //Now we can call the TodayIs method
            today = dayNamer.TodayIs();
            //close the factory with all its channels
            channelFactory.Close();
            //Return the name
            return today;
        }
    }
}
