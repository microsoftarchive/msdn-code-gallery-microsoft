using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace ECM_StateMachineWorkflow.Workflow1
{
    public sealed partial class Workflow1
    {
        #region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.Activities.CodeCondition codecondition1 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.Activities.CodeCondition codecondition2 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.Activities.CodeCondition codecondition3 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind3 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind4 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind5 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind6 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind7 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken2 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind8 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind9 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind10 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken3 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind11 = new System.Workflow.ComponentModel.ActivityBind();
            this.setStateActivity4 = new System.Workflow.Activities.SetStateActivity();
            this.logToHistoryListActivity4 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.setStateActivity3 = new System.Workflow.Activities.SetStateActivity();
            this.logToHistoryListActivity3 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.ifElseBranchActivity6 = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifElseBranchActivity5 = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifElseActivity3 = new System.Workflow.Activities.IfElseActivity();
            this.logToHistoryListActivity2 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.setStateActivity2 = new System.Workflow.Activities.SetStateActivity();
            this.ifElseBranchActivity4 = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifElseBranchActivity3 = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifElseBranchActivity2 = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifElseBranchActivity1 = new System.Workflow.Activities.IfElseBranchActivity();
            this.ifElseActivity2 = new System.Workflow.Activities.IfElseActivity();
            this.onTaskChanged2 = new Microsoft.SharePoint.WorkflowActions.OnTaskChanged();
            this.createReviewTask = new Microsoft.SharePoint.WorkflowActions.CreateTask();
            this.ifElseActivity1 = new System.Workflow.Activities.IfElseActivity();
            this.onTaskChanged1 = new Microsoft.SharePoint.WorkflowActions.OnTaskChanged();
            this.createTask1 = new Microsoft.SharePoint.WorkflowActions.CreateTask();
            this.setStateActivity1 = new System.Workflow.Activities.SetStateActivity();
            this.logToHistoryListActivity1 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            this.eventDrivenActivity3 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInitializationActivity1 = new System.Workflow.Activities.StateInitializationActivity();
            this.eventDrivenActivity2 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateInProgressInitialization = new System.Workflow.Activities.StateInitializationActivity();
            this.eventDrivenActivity1 = new System.Workflow.Activities.EventDrivenActivity();
            this.stateFinished = new System.Workflow.Activities.StateActivity();
            this.stateReview = new System.Workflow.Activities.StateActivity();
            this.stateInProgress = new System.Workflow.Activities.StateActivity();
            this.InitialState = new System.Workflow.Activities.StateActivity();
            // 
            // setStateActivity4
            // 
            this.setStateActivity4.Name = "setStateActivity4";
            this.setStateActivity4.TargetStateName = "stateInProgress";
            // 
            // logToHistoryListActivity4
            // 
            this.logToHistoryListActivity4.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity4.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity4.HistoryDescription = "A finished project document was not approved";
            this.logToHistoryListActivity4.HistoryOutcome = "";
            this.logToHistoryListActivity4.Name = "logToHistoryListActivity4";
            this.logToHistoryListActivity4.OtherData = "";
            this.logToHistoryListActivity4.UserId = -1;
            // 
            // setStateActivity3
            // 
            this.setStateActivity3.Name = "setStateActivity3";
            this.setStateActivity3.TargetStateName = "stateFinished";
            // 
            // logToHistoryListActivity3
            // 
            this.logToHistoryListActivity3.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity3.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity3.HistoryDescription = "A project document was approved";
            this.logToHistoryListActivity3.HistoryOutcome = "";
            this.logToHistoryListActivity3.Name = "logToHistoryListActivity3";
            this.logToHistoryListActivity3.OtherData = "";
            this.logToHistoryListActivity3.UserId = -1;
            // 
            // ifElseBranchActivity6
            // 
            this.ifElseBranchActivity6.Activities.Add(this.logToHistoryListActivity4);
            this.ifElseBranchActivity6.Activities.Add(this.setStateActivity4);
            this.ifElseBranchActivity6.Name = "ifElseBranchActivity6";
            // 
            // ifElseBranchActivity5
            // 
            this.ifElseBranchActivity5.Activities.Add(this.logToHistoryListActivity3);
            this.ifElseBranchActivity5.Activities.Add(this.setStateActivity3);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.DocApproved);
            this.ifElseBranchActivity5.Condition = codecondition1;
            this.ifElseBranchActivity5.Name = "ifElseBranchActivity5";
            // 
            // ifElseActivity3
            // 
            this.ifElseActivity3.Activities.Add(this.ifElseBranchActivity5);
            this.ifElseActivity3.Activities.Add(this.ifElseBranchActivity6);
            this.ifElseActivity3.Name = "ifElseActivity3";
            // 
            // logToHistoryListActivity2
            // 
            this.logToHistoryListActivity2.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity2.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity2.HistoryDescription = "A project document was sent for review";
            this.logToHistoryListActivity2.HistoryOutcome = "";
            this.logToHistoryListActivity2.Name = "logToHistoryListActivity2";
            this.logToHistoryListActivity2.OtherData = "";
            this.logToHistoryListActivity2.UserId = -1;
            // 
            // setStateActivity2
            // 
            this.setStateActivity2.Name = "setStateActivity2";
            this.setStateActivity2.TargetStateName = "stateReview";
            // 
            // ifElseBranchActivity4
            // 
            this.ifElseBranchActivity4.Name = "ifElseBranchActivity4";
            // 
            // ifElseBranchActivity3
            // 
            this.ifElseBranchActivity3.Activities.Add(this.ifElseActivity3);
            codecondition2.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.ReviewFinished);
            this.ifElseBranchActivity3.Condition = codecondition2;
            this.ifElseBranchActivity3.Name = "ifElseBranchActivity3";
            // 
            // ifElseBranchActivity2
            // 
            this.ifElseBranchActivity2.Name = "ifElseBranchActivity2";
            // 
            // ifElseBranchActivity1
            // 
            this.ifElseBranchActivity1.Activities.Add(this.setStateActivity2);
            this.ifElseBranchActivity1.Activities.Add(this.logToHistoryListActivity2);
            codecondition3.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.ReadyForReview);
            this.ifElseBranchActivity1.Condition = codecondition3;
            this.ifElseBranchActivity1.Name = "ifElseBranchActivity1";
            // 
            // ifElseActivity2
            // 
            this.ifElseActivity2.Activities.Add(this.ifElseBranchActivity3);
            this.ifElseActivity2.Activities.Add(this.ifElseBranchActivity4);
            this.ifElseActivity2.Name = "ifElseActivity2";
            // 
            // onTaskChanged2
            // 
            activitybind1.Name = "Workflow1";
            activitybind1.Path = "onTaskChanged2_AfterProperties1";
            activitybind2.Name = "Workflow1";
            activitybind2.Path = "onTaskChanged2_BeforeProperties1";
            correlationtoken1.Name = "reviewStateToken";
            correlationtoken1.OwnerActivityName = "stateReview";
            this.onTaskChanged2.CorrelationToken = correlationtoken1;
            this.onTaskChanged2.Executor = null;
            this.onTaskChanged2.Name = "onTaskChanged2";
            activitybind3.Name = "Workflow1";
            activitybind3.Path = "createReviewTask_TaskId1";
            this.onTaskChanged2.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onTaskChanged2_Invoked);
            this.onTaskChanged2.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.TaskIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind3)));
            this.onTaskChanged2.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.AfterPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            this.onTaskChanged2.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.BeforePropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            // 
            // createReviewTask
            // 
            this.createReviewTask.CorrelationToken = correlationtoken1;
            this.createReviewTask.ListItemId = -1;
            this.createReviewTask.Name = "createReviewTask";
            this.createReviewTask.SpecialPermissions = null;
            activitybind4.Name = "Workflow1";
            activitybind4.Path = "createReviewTask_TaskId1";
            activitybind5.Name = "Workflow1";
            activitybind5.Path = "createReviewTask_TaskProperties1";
            this.createReviewTask.MethodInvoking += new System.EventHandler(this.createReviewTask_MethodInvoking);
            this.createReviewTask.SetBinding(Microsoft.SharePoint.WorkflowActions.CreateTask.TaskIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind4)));
            this.createReviewTask.SetBinding(Microsoft.SharePoint.WorkflowActions.CreateTask.TaskPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind5)));
            // 
            // ifElseActivity1
            // 
            this.ifElseActivity1.Activities.Add(this.ifElseBranchActivity1);
            this.ifElseActivity1.Activities.Add(this.ifElseBranchActivity2);
            this.ifElseActivity1.Name = "ifElseActivity1";
            // 
            // onTaskChanged1
            // 
            activitybind6.Name = "Workflow1";
            activitybind6.Path = "onTaskChanged1_AfterProperties1";
            activitybind7.Name = "Workflow1";
            activitybind7.Path = "onTaskChanged1_BeforeProperties1";
            correlationtoken2.Name = "InProgressToken";
            correlationtoken2.OwnerActivityName = "stateInProgress";
            this.onTaskChanged1.CorrelationToken = correlationtoken2;
            this.onTaskChanged1.Executor = null;
            this.onTaskChanged1.Name = "onTaskChanged1";
            activitybind8.Name = "Workflow1";
            activitybind8.Path = "createTask1_TaskId1";
            this.onTaskChanged1.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.OnTaskChanged1_Invoked);
            this.onTaskChanged1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.TaskIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind8)));
            this.onTaskChanged1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.AfterPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind6)));
            this.onTaskChanged1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.BeforePropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind7)));
            // 
            // createTask1
            // 
            this.createTask1.CorrelationToken = correlationtoken2;
            this.createTask1.ListItemId = -1;
            this.createTask1.Name = "createTask1";
            this.createTask1.SpecialPermissions = null;
            activitybind9.Name = "Workflow1";
            activitybind9.Path = "createTask1_TaskId1";
            activitybind10.Name = "Workflow1";
            activitybind10.Path = "createTask1_TaskProperties1";
            this.createTask1.MethodInvoking += new System.EventHandler(this.createTask1_MethodInvoking);
            this.createTask1.SetBinding(Microsoft.SharePoint.WorkflowActions.CreateTask.TaskIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind9)));
            this.createTask1.SetBinding(Microsoft.SharePoint.WorkflowActions.CreateTask.TaskPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind10)));
            // 
            // setStateActivity1
            // 
            this.setStateActivity1.Name = "setStateActivity1";
            this.setStateActivity1.TargetStateName = "stateInProgress";
            // 
            // logToHistoryListActivity1
            // 
            this.logToHistoryListActivity1.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity1.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity1.HistoryDescription = "A new project document was created";
            this.logToHistoryListActivity1.HistoryOutcome = "";
            this.logToHistoryListActivity1.Name = "logToHistoryListActivity1";
            this.logToHistoryListActivity1.OtherData = "";
            this.logToHistoryListActivity1.UserId = -1;
            // 
            // onWorkflowActivated1
            // 
            correlationtoken3.Name = "workflowToken";
            correlationtoken3.OwnerActivityName = "Workflow1";
            this.onWorkflowActivated1.CorrelationToken = correlationtoken3;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind11.Name = "Workflow1";
            activitybind11.Path = "workflowProperties";
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind11)));
            // 
            // eventDrivenActivity3
            // 
            this.eventDrivenActivity3.Activities.Add(this.onTaskChanged2);
            this.eventDrivenActivity3.Activities.Add(this.ifElseActivity2);
            this.eventDrivenActivity3.Name = "eventDrivenActivity3";
            // 
            // stateInitializationActivity1
            // 
            this.stateInitializationActivity1.Activities.Add(this.createReviewTask);
            this.stateInitializationActivity1.Name = "stateInitializationActivity1";
            // 
            // eventDrivenActivity2
            // 
            this.eventDrivenActivity2.Activities.Add(this.onTaskChanged1);
            this.eventDrivenActivity2.Activities.Add(this.ifElseActivity1);
            this.eventDrivenActivity2.Name = "eventDrivenActivity2";
            // 
            // stateInProgressInitialization
            // 
            this.stateInProgressInitialization.Activities.Add(this.createTask1);
            this.stateInProgressInitialization.Name = "stateInProgressInitialization";
            // 
            // eventDrivenActivity1
            // 
            this.eventDrivenActivity1.Activities.Add(this.onWorkflowActivated1);
            this.eventDrivenActivity1.Activities.Add(this.logToHistoryListActivity1);
            this.eventDrivenActivity1.Activities.Add(this.setStateActivity1);
            this.eventDrivenActivity1.Name = "eventDrivenActivity1";
            // 
            // stateFinished
            // 
            this.stateFinished.Name = "stateFinished";
            // 
            // stateReview
            // 
            this.stateReview.Activities.Add(this.stateInitializationActivity1);
            this.stateReview.Activities.Add(this.eventDrivenActivity3);
            this.stateReview.Name = "stateReview";
            // 
            // stateInProgress
            // 
            this.stateInProgress.Activities.Add(this.stateInProgressInitialization);
            this.stateInProgress.Activities.Add(this.eventDrivenActivity2);
            this.stateInProgress.Name = "stateInProgress";
            // 
            // InitialState
            // 
            this.InitialState.Activities.Add(this.eventDrivenActivity1);
            this.InitialState.Name = "InitialState";
            // 
            // Workflow1
            // 
            this.Activities.Add(this.InitialState);
            this.Activities.Add(this.stateInProgress);
            this.Activities.Add(this.stateReview);
            this.Activities.Add(this.stateFinished);
            this.CompletedStateName = "stateFinished";
            this.DynamicUpdateCondition = null;
            this.InitialStateName = "InitialState";
            this.Name = "Workflow1";
            this.CanModifyActivities = false;

        }

        #endregion

        private SetStateActivity setStateActivity4;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity4;

        private SetStateActivity setStateActivity3;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity3;

        private IfElseBranchActivity ifElseBranchActivity6;

        private IfElseBranchActivity ifElseBranchActivity5;

        private IfElseActivity ifElseActivity3;

        private IfElseBranchActivity ifElseBranchActivity4;

        private IfElseBranchActivity ifElseBranchActivity3;

        private IfElseActivity ifElseActivity2;

        private Microsoft.SharePoint.WorkflowActions.OnTaskChanged onTaskChanged2;

        private EventDrivenActivity eventDrivenActivity3;

        private SetStateActivity setStateActivity2;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity2;

        private Microsoft.SharePoint.WorkflowActions.CreateTask createReviewTask;

        private StateInitializationActivity stateInitializationActivity1;

        private IfElseBranchActivity ifElseBranchActivity2;

        private IfElseBranchActivity ifElseBranchActivity1;

        private IfElseActivity ifElseActivity1;

        private Microsoft.SharePoint.WorkflowActions.OnTaskChanged onTaskChanged1;

        private EventDrivenActivity eventDrivenActivity2;

        private StateActivity stateFinished;

        private StateActivity stateReview;

        private StateActivity stateInProgress;

        private SetStateActivity setStateActivity1;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity1;

        private Microsoft.SharePoint.WorkflowActions.CreateTask createTask1;

        private StateInitializationActivity stateInProgressInitialization;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;

        private EventDrivenActivity eventDrivenActivity1;

        private StateActivity InitialState;





































    }
}
