using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

namespace ECM_WorkflowActivity.Features.ExampleWorkflowActivityFeature
{

    [Guid("5e6a4744-0442-47ea-b249-2eb209d5f9ba")]
    public class ExampleWorkflowActivityFeatureEventReceiver : SPFeatureReceiver
    {

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Get the current web application
            SPWebApplication currentWebApp = (SPWebApplication)properties.Feature.Parent;
            
            //Create a config modification object and set its properties
            SPWebConfigModification modification = new SPWebConfigModification();
            modification.Name = "AuthType";
            modification.Owner = "ExampleActivityLibrary";
            modification.Path = "configuration/System.Workflow.ComponentModel.WorkflowCompiler/authorizedTypes";
            modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
            modification.Value = "<authorizedType Assembly=\"ExampleActivityLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3a18e08921ea71f7\" " +
                "Namespace=\"ExampleActivityLibrary\" TypeName=\"*\" Authorized=\"True\" />";

            //Add and apply the modification
            currentWebApp.WebConfigModifications.Add(modification);
            currentWebApp.WebService.ApplyWebConfigModifications();
        }

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}

    }
}
