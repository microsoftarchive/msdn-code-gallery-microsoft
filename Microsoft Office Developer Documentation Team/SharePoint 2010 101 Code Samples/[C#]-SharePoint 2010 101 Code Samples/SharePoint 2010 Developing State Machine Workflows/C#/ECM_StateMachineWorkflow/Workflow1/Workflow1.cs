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

namespace ECM_StateMachineWorkflow.Workflow1
{
    /// <summary>
    /// This is an example state machine workflow with four states, conditional state
    /// changes, SharePoint task creation, and event driven activities, and history list
    /// logging.
    /// </summary>
    /// <remarks>
    /// This workflow requires a document library called Projects. When a document is 
    /// created in the library, a task item is created called "Finish Document" and 
    /// stateInProgress is entered. When that task is marked 100% complete, another 
    /// task is created called "Review Document" and stateReview is entered.
    /// When the second task is marked 100% complete, if the Description is "Approved", 
    /// stateFinished is entered and the workflow is complete. Otherwise the workflow
    /// returns to stateInProgress and another "Finish Document" task item is created.
    /// </remarks>
    public sealed partial class Workflow1 : StateMachineWorkflowActivity
    {
        public Workflow1()
        {
            InitializeComponent();
        }

        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        public Guid createTask1_TaskId1 = default(System.Guid);

        //This method sets properties for the first task item
        private void createTask1_MethodInvoking(object sender, EventArgs e)
        {
            createTask1_TaskId1 = Guid.NewGuid();
            createTask1_TaskProperties1.Title = "Finish Document";
            createTask1_TaskProperties1.AssignedTo = @"CONTOSO\aris";
            createTask1_TaskProperties1.DueDate = DateTime.Now.AddDays(1.0);
        }

        public SPWorkflowTaskProperties createTask1_TaskProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties onTaskChanged1_AfterProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties onTaskChanged1_BeforeProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();

        private void OnTaskChanged1_Invoked(object sender, ExternalDataEventArgs e)
        {
            onTaskChanged1_AfterProperties1 = onTaskChanged1.AfterProperties;
            onTaskChanged1_BeforeProperties1 = onTaskChanged1.BeforeProperties;
        }

        //This is the code condition for ifElseBranchActivity1
        //It checks if the task is complete
        private void ReadyForReview(object sender, ConditionalEventArgs e)
        {
            if (onTaskChanged1_AfterProperties1.PercentComplete == 1.0)
            {
                e.Result = true;
            }
            else
            {
                e.Result = false;
            }
        }

        public Guid createReviewTask_TaskId1 = default(System.Guid);
        public SPWorkflowTaskProperties createReviewTask_TaskProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();

        //This method sets properties for the Review Document task item
        private void createReviewTask_MethodInvoking(object sender, EventArgs e)
        {
            createReviewTask_TaskId1 = Guid.NewGuid();
            createReviewTask_TaskProperties1.Title = "Review Document";
            createReviewTask_TaskProperties1.AssignedTo = @"CONTOSO\danj";
            createReviewTask_TaskProperties1.DueDate = DateTime.Now.AddDays(1.0);
        }

        public SPWorkflowTaskProperties onTaskChanged2_AfterProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties onTaskChanged2_BeforeProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();

        private void onTaskChanged2_Invoked(object sender, ExternalDataEventArgs e)
        {
            onTaskChanged2_AfterProperties1 = onTaskChanged2.AfterProperties;
            onTaskChanged2_BeforeProperties1 = onTaskChanged2.BeforeProperties;
        }

        //This is the code condition for ifElseBranchActivity3
        //It checks if the "Review Document" task is 100% complete
        private void ReviewFinished(object sender, ConditionalEventArgs e)
        {
            if (onTaskChanged2_AfterProperties1.PercentComplete == 1.0)
            {
                e.Result = true;
            }
            else
            {
                e.Result = false;
            }
        }

        //This is the code condition for ifElseBranchActivity5
        //It checks if the Description field in the "Review Document" task is "Approved"
        private void DocApproved(object sender, ConditionalEventArgs e)
        {
            if (onTaskChanged2_AfterProperties1.Description == "<DIV>Approved</DIV>")
            {
                e.Result = true;
            }
            else
            {
                e.Result = false;
            }
        }
    }
}
