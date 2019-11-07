// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

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

namespace SequentialWorkflow.Workflow1
{
    public sealed partial class Workflow1 : SequentialWorkflowActivity
    {
        public Workflow1()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public Guid taskId1 = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();
        public SPWorkflowTaskProperties taskProperties = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties afterProperties = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        public SPWorkflowTaskProperties beforeProperties1 = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
        private bool taskCompleted;

        private void createTask1_MethodInvoking(object sender, EventArgs e)
        {
            //Task must have a guid
            taskId1 = Guid.NewGuid();
            //Setting up the basic task properties
            taskProperties.PercentComplete = (float)0.0;
            taskProperties.AssignedTo = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            //This is really useful when incorporating InfoPath forms
            taskProperties.TaskType = 0;
            taskProperties.DueDate = DateTime.Now.AddDays(7);
            taskProperties.StartDate = DateTime.Now;
            taskProperties.Title = "SharePoint Workflow Task";
        }

        private void notCompleted(object sender, ConditionalEventArgs e)
        {
            //The result must be 1.0 for the task to be complete
            e.Result = !taskCompleted;
        }

        private void onTaskChanged1_Invoked(object sender, ExternalDataEventArgs e)
        {
            //Checking the task properties after the change.
            //Looking for 1.0 to reflect a completed task.
            if (afterProperties.PercentComplete == 1.0)
            {
                taskCompleted = true;
            }
        }
    }
}
