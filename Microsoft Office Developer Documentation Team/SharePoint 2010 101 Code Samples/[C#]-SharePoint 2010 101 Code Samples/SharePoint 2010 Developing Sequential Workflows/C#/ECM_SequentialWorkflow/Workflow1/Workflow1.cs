using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;

namespace ECM_SequentialWorkflow.Workflow1
{
    /// <summary>
    /// This simple workflow demonstrates how to develop a workflow in Visual Studio
    /// and deploy it to a SharePoint list. At each stage in the workflow, the 
    /// checkStatus method checks the value of the Document Status property. If the
    /// value is "Review Complete" the While Activity can stop looping and the workflow
    /// is complete
    /// </summary>
    /// <remarks>
    /// Before you deploy this workflow, you must create a Document Library called 
    /// Projects with a Column called "Document Status". This should be a Choice column
    /// with several values, one of which is "Review Complete".  
    /// </remarks>
    public sealed partial class Workflow1 : SequentialWorkflowActivity
    {
        public Workflow1()
        {
            InitializeComponent();
        }

        bool bIsWorkflowPending = true;

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();

        private void onWorkflowActivated(object sender, ExternalDataEventArgs e)
        {
            checkStatus();
        }

        private void isWorkflowPending(object sender, ConditionalEventArgs e)
        {
            e.Result = bIsWorkflowPending;
        }

        private void onWorkflowItemChanged(object sender, ExternalDataEventArgs e)
        {
            checkStatus();
        }

        private void checkStatus()
        {
            if (workflowProperties.Item["Document Status"].ToString() == "Review Complete")
            {
                bIsWorkflowPending = false;
            }
        }
    }
}
