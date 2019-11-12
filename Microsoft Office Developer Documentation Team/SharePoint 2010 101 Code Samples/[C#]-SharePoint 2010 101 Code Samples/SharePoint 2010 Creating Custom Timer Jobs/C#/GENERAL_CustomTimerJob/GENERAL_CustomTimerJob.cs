using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
//This namespace is used for the SPJobDefinition class
using Microsoft.SharePoint.Administration;

namespace GENERAL_CustomTimerJob
{
    //To create a custom timer job, first add a class to your SharePoint project and 
    //inherit from SPJobDefinition. Implement the constructors and override the Execute
    //method as shown below. To install your timer job, and set the schedule, you must 
    //add a Feature and a Feature receiver. 
    class GENERAL_CustomTimerJob : SPJobDefinition
    {

#region Constructors

        //You must implement all three constructors
        public GENERAL_CustomTimerJob()
            : base()
        {

        }

        public GENERAL_CustomTimerJob(string jobName, SPService service, SPServer server, SPJobLockType targetType)
            : base(jobName, service, server, targetType)
        {

        }

        public GENERAL_CustomTimerJob(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.ContentDatabase)
        {
            //Set the title of the job, which will be shown in the Central Admin UI
            this.Title = "Simple Example Timer Job";
        }

#endregion

        //Override the Execute method to run code.
        public override void Execute(Guid targetInstanceId)
        {
            //Get the Web Application in which this Timer Job runs
            SPWebApplication webApp = this.Parent as SPWebApplication;
            //Get the site collection
            SPSiteCollection timerSiteCollection = webApp.ContentDatabases[targetInstanceId].Sites;
            //Get the Announcements list in the RootWeb of each SPSite
            SPList timerJobList = null;
            foreach (SPSite site in timerSiteCollection)
            {
                timerJobList = site.RootWeb.Lists.TryGetList("Announcements");
                if (timerJobList != null)
                {
                    SPListItem newItem = timerJobList.Items.Add();
                    newItem["Title"] = "Today is " + DateTime.Today.ToLongDateString();
                    newItem.Update();
                }
            }
        }

    }
}
