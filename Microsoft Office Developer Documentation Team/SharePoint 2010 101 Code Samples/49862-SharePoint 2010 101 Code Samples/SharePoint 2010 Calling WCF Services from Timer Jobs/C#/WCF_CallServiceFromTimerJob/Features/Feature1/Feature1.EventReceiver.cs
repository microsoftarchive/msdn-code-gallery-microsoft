using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

namespace WCF_CallServiceFromTimerJob.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("683e7fb9-f5ab-44e7-ad1c-4bdbf0338352")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        const string TIMER_JOB_NAME = "WCFCaller";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //In this event we create and install the timer job
            //Start by finding the SPSite.
            SPSite site = properties.Feature.Parent as SPSite;
            //Make sure the timer job isn't already registered
            foreach(SPJobDefinition job in site.WebApplication.JobDefinitions)
            {
                if(job.Name == TIMER_JOB_NAME)
                {
                    job.Delete();
                }
            }
            //Create a configure the new Timer job
            WCF_CallingTimerJob wcfTimerJob = new WCF_CallingTimerJob(TIMER_JOB_NAME, site.WebApplication);
            SPMinuteSchedule jobSchedule = new SPMinuteSchedule();
            jobSchedule.BeginSecond = 0;
            jobSchedule.EndSecond = 59;
            jobSchedule.Interval = 5;
            wcfTimerJob.Schedule = jobSchedule;
            wcfTimerJob.Update();
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //In this event we must clean up by deleting the timer job
            SPSite site = properties.Feature.Parent as SPSite;
            //Locate the right timer job
            foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
            {
                if (job.Name == TIMER_JOB_NAME)
                {
                    //This one is the right job. Delete it.
                    job.Delete();
                }
            }
        }

    }
}
