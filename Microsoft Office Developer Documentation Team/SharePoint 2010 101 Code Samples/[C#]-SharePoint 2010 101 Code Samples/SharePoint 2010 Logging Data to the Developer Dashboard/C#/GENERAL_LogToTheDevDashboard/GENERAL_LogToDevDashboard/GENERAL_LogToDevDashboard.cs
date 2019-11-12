using System;
using System.ComponentModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace GENERAL_LogToTheDevDashboard.GENERAL_LogToDevDashboard
{
    //This web part simply displays text in a label after pausing for one second
    //Because the pause in done in a monitored scope, information appears in the
    //developer dashboard when you display it. Look for "Long Procedure" in the 
    //dashboard for timings and other information. Use the two PowerShell scripts
    //in this project to enable and disable the dashboard.
    [ToolboxItemAttribute(false)]
    public class GENERAL_LogToDevDashboard : WebPart
    {
        //Controls
        Label feedbackLabel;

        protected override void CreateChildControls()
        {
            //Set up the feedback label
            feedbackLabel = new Label();
            this.Controls.Add(feedbackLabel);
        }

        protected override void OnPreRender(EventArgs e)
        {
            //To log to the developer dashboard, set up a monitored scope
            using (SPMonitoredScope myScope = new SPMonitoredScope("Long Procedure"))
            {
                feedbackLabel.Text = longProcedure();
            }
            base.OnPreRender(e);
        }

        private string longProcedure()
        {
            //Wait for one second. This should more-or-less match the info in the developer dashboard
            Thread.Sleep(1000);
            return "The Long Procedure has completed!";
        }
    }
}
