using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.PowerShell;
using System.Management.Automation;
using Microsoft.SharePoint.Administration;
using WCF_CustomServiceApplication.Server;

namespace WCF_CustomServiceApplication.PowerShellRegistration
{
    //This class registers a new PowerShell cmdlet: New_DayNamerServiceApplication
    //You can use this to create the service application, or use Central Administration
    //After you run this cmdlet, be sure to run the New-DayNamerServiceApplicationProxy cmdlet
    [Cmdlet(VerbsCommon.New, "DayNamerServiceApplication", SupportsShouldProcess = true)]
    public class NewDayNamerServiceApplication : SPCmdlet
    {
        #region cmdlet parameters
        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string Name;

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public SPIisWebServiceApplicationPoolPipeBind ApplicationPool;
        #endregion

        protected override bool RequireUserFarmAdmin()
        {
            return true;
        }

        protected override void InternalProcessRecord()
        {
            #region validation checks
            // ensure can hit farm
            SPFarm farm = SPFarm.Local;
            if (farm == null)
            {
                ThrowTerminatingError(new InvalidOperationException("SharePoint farm not found."), ErrorCategory.ResourceUnavailable, this);
                SkipProcessCurrentRecord();
            }

            // ensure can hit local server
            SPServer server = SPServer.Local;
            if (server == null)
            {
                ThrowTerminatingError(new InvalidOperationException("SharePoint local server not found."), ErrorCategory.ResourceUnavailable, this);
                SkipProcessCurrentRecord();
            }

            // ensure can hit service application
            DayNamerService service = farm.Services.GetValue<DayNamerService>();
            if (service == null)
            {
                ThrowTerminatingError(new InvalidOperationException("Day Namer Service not found."), ErrorCategory.ResourceUnavailable, this);
                SkipProcessCurrentRecord();
            }

            // ensure can hit app pool
            SPIisWebServiceApplicationPool appPool = this.ApplicationPool.Read();
            if (appPool == null)
            {
                ThrowTerminatingError(new InvalidOperationException("Application pool not found."), ErrorCategory.ResourceUnavailable, this);
                SkipProcessCurrentRecord();
            }
            #endregion

            // Check a service app doesn't already exist
            DayNamerServiceApplication existingServiceApp = service.Applications.GetValue<DayNamerServiceApplication>();
            if (existingServiceApp != null)
            {
                WriteError(new InvalidOperationException("Day Namer Service Application already exists."),
                    ErrorCategory.ResourceExists,
                    existingServiceApp);
                SkipProcessCurrentRecord();
            }

            // Create & provision the service application
            if (ShouldProcess(this.Name))
            {
                DayNamerServiceApplication serviceApp = DayNamerServiceApplication.Create(
                    this.Name,
                    service,
                    appPool);

                // provision the service app
                serviceApp.Provision();

                // pass service app back to the PowerShell
                WriteObject(serviceApp);
            }


        }
    }
}
