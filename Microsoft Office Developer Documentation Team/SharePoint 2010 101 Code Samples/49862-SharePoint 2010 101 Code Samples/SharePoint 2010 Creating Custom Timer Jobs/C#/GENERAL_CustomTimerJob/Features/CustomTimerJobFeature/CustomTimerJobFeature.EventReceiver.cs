using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

namespace GENERAL_CustomTimerJob.Features.CustomTimerJobFeature
{
    /// <summary>
    /// For a custom timer job, you must use this feature receiver to install and configure
    /// the SPJobDefinition class
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("1b45bf8b-f5e2-4155-8f7f-999b1163c3d1")]
    public class CustomTimerJobFeatureEventReceiver : SPFeatureReceiver
    {
        const string TIMER_JOB_NAME = "DemoTimerJob";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //In this event we create and install the timer job
            //Start by finding the SPSite.
            SPSite site = (SPSite)properties.Feature.Parent;
            //Make sure the timer job isn't already registered
            foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
            {
                if (job.Name == TIMER_JOB_NAME)
                {
                    job.Delete();
                }
            }
            //Create a new Timer job
            GENERAL_CustomTimerJob newTimerJob = new GENERAL_CustomTimerJob(TIMER_JOB_NAME, site.WebApplication);
            //Configure the schedule and save it
            SPMinuteSchedule jobSchedule = new SPMinuteSchedule();
            jobSchedule.BeginSecond = 0;
            jobSchedule.EndSecond = 59;
            jobSchedule.Interval = 5;
            newTimerJob.Schedule = jobSchedule;
            newTimerJob.Update();
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
