using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WorkflowActions;
using System.Threading;

// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

namespace Microsoft.Samples.SequentialWorkflow
{
    public sealed partial class Workflow1 : SequentialWorkflowActivity
    {
        public Workflow1()
        {
            InitializeComponent();
        }
        #region "Global Declarations"
        public Guid workflowId = default(System.Guid);
        public Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties workflowProperties = new Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties();
        public static DependencyProperty taskPropertiesProperty = DependencyProperty.Register("taskProperties", typeof(Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties), typeof(Microsoft.Samples.SequentialWorkflow.Workflow1));
        private bool taskCompleted;
        #endregion
        #region "TaskProps"
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Misc")]

        public Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties taskProperties
        {
            get
            {
                return ((Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties)(base.GetValue(Microsoft.Samples.SequentialWorkflow.Workflow1.taskPropertiesProperty)));
            }
            set
            {
                base.SetValue(Microsoft.Samples.SequentialWorkflow.Workflow1.taskPropertiesProperty, value);
            }
        }

        public static DependencyProperty TaskIDProperty = DependencyProperty.Register("TaskID", typeof(System.Guid), typeof(Microsoft.Samples.SequentialWorkflow.Workflow1));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Misc")]
        public Guid TaskID
        {
            get
            {
                return ((System.Guid)(base.GetValue(Microsoft.Samples.SequentialWorkflow.Workflow1.TaskIDProperty)));
            }
            set
            {
                base.SetValue(Microsoft.Samples.SequentialWorkflow.Workflow1.TaskIDProperty, value);
            }
        }

        public static DependencyProperty afterTaskPropsProperty = DependencyProperty.Register("afterTaskProps", typeof(Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties), typeof(Microsoft.Samples.SequentialWorkflow.Workflow1));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Misc")]
        public Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties afterTaskProps
        {
            get
            {
                return ((Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties)(base.GetValue(Microsoft.Samples.SequentialWorkflow.Workflow1.afterTaskPropsProperty)));
            }
            set
            {
                base.SetValue(Microsoft.Samples.SequentialWorkflow.Workflow1.afterTaskPropsProperty, value);
            }
        }

        public static DependencyProperty beforeTaskPropsProperty = DependencyProperty.Register("beforeTaskProps", typeof(Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties), typeof(Microsoft.Samples.SequentialWorkflow.Workflow1));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Misc")]
        public Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties beforeTaskProps
        {
            get
            {
                return ((Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties)(base.GetValue(Microsoft.Samples.SequentialWorkflow.Workflow1.beforeTaskPropsProperty)));
            }
            set
            {
                base.SetValue(Microsoft.Samples.SequentialWorkflow.Workflow1.beforeTaskPropsProperty, value);
            }
        }

        #endregion
        #region "Workflow Routines"
        private void notCompleted(object sender, ConditionalEventArgs e)
        {
            //The result must be 1.0 for the task to be complete
            e.Result = !taskCompleted;
        }

        private void onTaskChanged(object sender, ExternalDataEventArgs e)
        {
            //Checking the task properties after the change.
            //Looking for 1.0 to reflect a completed task.
            if (afterTaskProps.PercentComplete == 1.0)
            {
                taskCompleted = true;
            }
        }

        private void TaskCreation(object sender, EventArgs e)
        {
            try
            {
                if (taskProperties == null)
                {
                    taskProperties = new Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties();
                }
                //Task must have a guid
                TaskID = Guid.NewGuid();
                //Setting up the basic task properties
                taskProperties.PercentComplete = (float)0.0;
                taskProperties.AssignedTo = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                //This is really useful when incorporating InfoPath forms
                taskProperties.TaskType = 0;
                taskProperties.DueDate = DateTime.Now.AddDays(7);
                taskProperties.StartDate = DateTime.Now;
                taskProperties.Title = "SharePoint Workflow Task";
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }

}
