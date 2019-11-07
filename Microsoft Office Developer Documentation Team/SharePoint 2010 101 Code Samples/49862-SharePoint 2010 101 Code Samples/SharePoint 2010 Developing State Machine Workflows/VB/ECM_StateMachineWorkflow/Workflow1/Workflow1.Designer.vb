
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Workflow1

    'NOTE: The following procedure is required by the Workflow Designer
    'It can be modified using the Workflow Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Private Sub InitializeComponent()
        Me.CanModifyActivities = True
        Dim codecondition1 As System.Workflow.Activities.CodeCondition = New System.Workflow.Activities.CodeCondition()
        Dim codecondition2 As System.Workflow.Activities.CodeCondition = New System.Workflow.Activities.CodeCondition()
        Dim codecondition3 As System.Workflow.Activities.CodeCondition = New System.Workflow.Activities.CodeCondition()
        Dim activitybind1 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Dim activitybind2 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Dim correlationtoken1 As System.Workflow.Runtime.CorrelationToken = New System.Workflow.Runtime.CorrelationToken()
        Dim activitybind3 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Dim activitybind4 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Dim activitybind5 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Dim activitybind6 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Dim activitybind7 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Dim correlationtoken2 As System.Workflow.Runtime.CorrelationToken = New System.Workflow.Runtime.CorrelationToken()
        Dim activitybind8 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Dim activitybind9 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Dim activitybind10 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Dim correlationtoken3 As System.Workflow.Runtime.CorrelationToken = New System.Workflow.Runtime.CorrelationToken()
        Dim activitybind11 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Me.setStateActivity4 = New System.Workflow.Activities.SetStateActivity()
        Me.logToHistoryListActivity4 = New Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity()
        Me.setStateActivity3 = New System.Workflow.Activities.SetStateActivity()
        Me.logToHistoryListActivity3 = New Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity()
        Me.ifElseBranchActivity6 = New System.Workflow.Activities.IfElseBranchActivity()
        Me.ifElseBranchActivity5 = New System.Workflow.Activities.IfElseBranchActivity()
        Me.ifElseActivity3 = New System.Workflow.Activities.IfElseActivity()
        Me.logToHistoryListActivity2 = New Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity()
        Me.setStateActivity2 = New System.Workflow.Activities.SetStateActivity()
        Me.ifElseBranchActivity4 = New System.Workflow.Activities.IfElseBranchActivity()
        Me.ifElseBranchActivity3 = New System.Workflow.Activities.IfElseBranchActivity()
        Me.ifElseBranchActivity2 = New System.Workflow.Activities.IfElseBranchActivity()
        Me.ifElseBranchActivity1 = New System.Workflow.Activities.IfElseBranchActivity()
        Me.ifElseActivity2 = New System.Workflow.Activities.IfElseActivity()
        Me.onTaskChanged2 = New Microsoft.SharePoint.WorkflowActions.OnTaskChanged()
        Me.createReviewTask = New Microsoft.SharePoint.WorkflowActions.CreateTask()
        Me.ifElseActivity1 = New System.Workflow.Activities.IfElseActivity()
        Me.onTaskChanged1 = New Microsoft.SharePoint.WorkflowActions.OnTaskChanged()
        Me.createTask1 = New Microsoft.SharePoint.WorkflowActions.CreateTask()
        Me.setStateActivity1 = New System.Workflow.Activities.SetStateActivity()
        Me.logToHistoryListActivity1 = New Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity()
        Me.onWorkflowActivated1 = New Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated()
        Me.eventDrivenActivity3 = New System.Workflow.Activities.EventDrivenActivity()
        Me.stateInitializationActivity1 = New System.Workflow.Activities.StateInitializationActivity()
        Me.eventDrivenActivity2 = New System.Workflow.Activities.EventDrivenActivity()
        Me.stateInProgressInitialization = New System.Workflow.Activities.StateInitializationActivity()
        Me.eventDrivenActivity1 = New System.Workflow.Activities.EventDrivenActivity()
        Me.stateFinished = New System.Workflow.Activities.StateActivity()
        Me.stateReview = New System.Workflow.Activities.StateActivity()
        Me.stateInProgress = New System.Workflow.Activities.StateActivity()
        Me.InitialState = New System.Workflow.Activities.StateActivity()
        '
        'setStateActivity4
        '
        Me.setStateActivity4.Name = "setStateActivity4"
        Me.setStateActivity4.TargetStateName = "stateInProgress"
        '
        'logToHistoryListActivity4
        '
        Me.logToHistoryListActivity4.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808")
        Me.logToHistoryListActivity4.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment
        Me.logToHistoryListActivity4.HistoryDescription = "A finished project document was not approved"
        Me.logToHistoryListActivity4.HistoryOutcome = ""
        Me.logToHistoryListActivity4.Name = "logToHistoryListActivity4"
        Me.logToHistoryListActivity4.OtherData = ""
        Me.logToHistoryListActivity4.UserId = -1
        '
        'setStateActivity3
        '
        Me.setStateActivity3.Name = "setStateActivity3"
        Me.setStateActivity3.TargetStateName = "stateFinished"
        '
        'logToHistoryListActivity3
        '
        Me.logToHistoryListActivity3.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808")
        Me.logToHistoryListActivity3.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment
        Me.logToHistoryListActivity3.HistoryDescription = "A project document was approved"
        Me.logToHistoryListActivity3.HistoryOutcome = ""
        Me.logToHistoryListActivity3.Name = "logToHistoryListActivity3"
        Me.logToHistoryListActivity3.OtherData = ""
        Me.logToHistoryListActivity3.UserId = -1
        '
        'ifElseBranchActivity6
        '
        Me.ifElseBranchActivity6.Activities.Add(Me.logToHistoryListActivity4)
        Me.ifElseBranchActivity6.Activities.Add(Me.setStateActivity4)
        Me.ifElseBranchActivity6.Name = "ifElseBranchActivity6"
        '
        'ifElseBranchActivity5
        '
        Me.ifElseBranchActivity5.Activities.Add(Me.logToHistoryListActivity3)
        Me.ifElseBranchActivity5.Activities.Add(Me.setStateActivity3)
        AddHandler codecondition1.Condition, AddressOf Me.DocApproved
        Me.ifElseBranchActivity5.Condition = codecondition1
        Me.ifElseBranchActivity5.Name = "ifElseBranchActivity5"
        '
        'ifElseActivity3
        '
        Me.ifElseActivity3.Activities.Add(Me.ifElseBranchActivity5)
        Me.ifElseActivity3.Activities.Add(Me.ifElseBranchActivity6)
        Me.ifElseActivity3.Name = "ifElseActivity3"
        '
        'logToHistoryListActivity2
        '
        Me.logToHistoryListActivity2.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808")
        Me.logToHistoryListActivity2.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment
        Me.logToHistoryListActivity2.HistoryDescription = "A project document was sent for review"
        Me.logToHistoryListActivity2.HistoryOutcome = ""
        Me.logToHistoryListActivity2.Name = "logToHistoryListActivity2"
        Me.logToHistoryListActivity2.OtherData = ""
        Me.logToHistoryListActivity2.UserId = -1
        '
        'setStateActivity2
        '
        Me.setStateActivity2.Name = "setStateActivity2"
        Me.setStateActivity2.TargetStateName = "stateReview"
        '
        'ifElseBranchActivity4
        '
        Me.ifElseBranchActivity4.Name = "ifElseBranchActivity4"
        '
        'ifElseBranchActivity3
        '
        Me.ifElseBranchActivity3.Activities.Add(Me.ifElseActivity3)
        AddHandler codecondition2.Condition, AddressOf Me.ReviewFinished
        Me.ifElseBranchActivity3.Condition = codecondition2
        Me.ifElseBranchActivity3.Name = "ifElseBranchActivity3"
        '
        'ifElseBranchActivity2
        '
        Me.ifElseBranchActivity2.Name = "ifElseBranchActivity2"
        '
        'ifElseBranchActivity1
        '
        Me.ifElseBranchActivity1.Activities.Add(Me.setStateActivity2)
        Me.ifElseBranchActivity1.Activities.Add(Me.logToHistoryListActivity2)
        AddHandler codecondition3.Condition, AddressOf Me.ReadyForReview
        Me.ifElseBranchActivity1.Condition = codecondition3
        Me.ifElseBranchActivity1.Name = "ifElseBranchActivity1"
        '
        'ifElseActivity2
        '
        Me.ifElseActivity2.Activities.Add(Me.ifElseBranchActivity3)
        Me.ifElseActivity2.Activities.Add(Me.ifElseBranchActivity4)
        Me.ifElseActivity2.Name = "ifElseActivity2"
        '
        'onTaskChanged2
        '
        activitybind1.Name = "Workflow1"
        activitybind1.Path = "onTaskChanged2_AfterProperties1"
        activitybind2.Name = "Workflow1"
        activitybind2.Path = "onTaskChanged2_BeforeProperties1"
        correlationtoken1.Name = "ReviewStateToken"
        correlationtoken1.OwnerActivityName = "stateReview"
        Me.onTaskChanged2.CorrelationToken = correlationtoken1
        Me.onTaskChanged2.Executor = Nothing
        Me.onTaskChanged2.Name = "onTaskChanged2"
        activitybind3.Name = "Workflow1"
        activitybind3.Path = "createReviewTask_TaskId1"
        AddHandler Me.onTaskChanged2.Invoked, AddressOf Me.onTaskChanged2_Invoked
        Me.onTaskChanged2.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.TaskIdProperty, CType(activitybind3, System.Workflow.ComponentModel.ActivityBind))
        Me.onTaskChanged2.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.AfterPropertiesProperty, CType(activitybind1, System.Workflow.ComponentModel.ActivityBind))
        Me.onTaskChanged2.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.BeforePropertiesProperty, CType(activitybind2, System.Workflow.ComponentModel.ActivityBind))
        '
        'createReviewTask
        '
        Me.createReviewTask.CorrelationToken = correlationtoken1
        Me.createReviewTask.ListItemId = -1
        Me.createReviewTask.Name = "createReviewTask"
        Me.createReviewTask.SpecialPermissions = Nothing
        activitybind4.Name = "Workflow1"
        activitybind4.Path = "createReviewTask_TaskId1"
        activitybind5.Name = "Workflow1"
        activitybind5.Path = "createReviewTask_TaskProperties1"
        AddHandler Me.createReviewTask.MethodInvoking, AddressOf Me.createReviewTask_MethodInvoking
        Me.createReviewTask.SetBinding(Microsoft.SharePoint.WorkflowActions.CreateTask.TaskIdProperty, CType(activitybind4, System.Workflow.ComponentModel.ActivityBind))
        Me.createReviewTask.SetBinding(Microsoft.SharePoint.WorkflowActions.CreateTask.TaskPropertiesProperty, CType(activitybind5, System.Workflow.ComponentModel.ActivityBind))
        '
        'ifElseActivity1
        '
        Me.ifElseActivity1.Activities.Add(Me.ifElseBranchActivity1)
        Me.ifElseActivity1.Activities.Add(Me.ifElseBranchActivity2)
        Me.ifElseActivity1.Name = "ifElseActivity1"
        '
        'onTaskChanged1
        '
        activitybind6.Name = "Workflow1"
        activitybind6.Path = "onTaskChanged1_AfterProperties1"
        activitybind7.Name = "Workflow1"
        activitybind7.Path = "onTaskChanged1_BeforeProperties1"
        correlationtoken2.Name = "InProgressToken"
        correlationtoken2.OwnerActivityName = "stateInProgress"
        Me.onTaskChanged1.CorrelationToken = correlationtoken2
        Me.onTaskChanged1.Executor = Nothing
        Me.onTaskChanged1.Name = "onTaskChanged1"
        activitybind8.Name = "Workflow1"
        activitybind8.Path = "createTask1_TaskId1"
        AddHandler Me.onTaskChanged1.Invoked, AddressOf Me.onTaskChanged1_Invoked
        Me.onTaskChanged1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.TaskIdProperty, CType(activitybind8, System.Workflow.ComponentModel.ActivityBind))
        Me.onTaskChanged1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.AfterPropertiesProperty, CType(activitybind6, System.Workflow.ComponentModel.ActivityBind))
        Me.onTaskChanged1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnTaskChanged.BeforePropertiesProperty, CType(activitybind7, System.Workflow.ComponentModel.ActivityBind))
        '
        'createTask1
        '
        Me.createTask1.CorrelationToken = correlationtoken2
        Me.createTask1.ListItemId = -1
        Me.createTask1.Name = "createTask1"
        Me.createTask1.SpecialPermissions = Nothing
        activitybind9.Name = "Workflow1"
        activitybind9.Path = "createTask1_TaskId1"
        activitybind10.Name = "Workflow1"
        activitybind10.Path = "createTask1_TaskProperties1"
        AddHandler Me.createTask1.MethodInvoking, AddressOf Me.createTask1_MethodInvoking
        Me.createTask1.SetBinding(Microsoft.SharePoint.WorkflowActions.CreateTask.TaskIdProperty, CType(activitybind9, System.Workflow.ComponentModel.ActivityBind))
        Me.createTask1.SetBinding(Microsoft.SharePoint.WorkflowActions.CreateTask.TaskPropertiesProperty, CType(activitybind10, System.Workflow.ComponentModel.ActivityBind))
        '
        'setStateActivity1
        '
        Me.setStateActivity1.Name = "setStateActivity1"
        Me.setStateActivity1.TargetStateName = "stateInProgress"
        '
        'logToHistoryListActivity1
        '
        Me.logToHistoryListActivity1.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808")
        Me.logToHistoryListActivity1.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment
        Me.logToHistoryListActivity1.HistoryDescription = "A new project document was created"
        Me.logToHistoryListActivity1.HistoryOutcome = ""
        Me.logToHistoryListActivity1.Name = "logToHistoryListActivity1"
        Me.logToHistoryListActivity1.OtherData = ""
        Me.logToHistoryListActivity1.UserId = -1
        '
        'onWorkflowActivated1
        '
        correlationtoken3.Name = "workflowToken"
        correlationtoken3.OwnerActivityName = "Workflow1"
        Me.onWorkflowActivated1.CorrelationToken = correlationtoken3
        Me.onWorkflowActivated1.EventName = "OnWorkflowActivated"
        Me.onWorkflowActivated1.Name = "onWorkflowActivated1"
        activitybind11.Name = "Workflow1"
        activitybind11.Path = "workflowProperties"
        Me.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, CType(activitybind11, System.Workflow.ComponentModel.ActivityBind))
        '
        'eventDrivenActivity3
        '
        Me.eventDrivenActivity3.Activities.Add(Me.onTaskChanged2)
        Me.eventDrivenActivity3.Activities.Add(Me.ifElseActivity2)
        Me.eventDrivenActivity3.Name = "eventDrivenActivity3"
        '
        'stateInitializationActivity1
        '
        Me.stateInitializationActivity1.Activities.Add(Me.createReviewTask)
        Me.stateInitializationActivity1.Name = "stateInitializationActivity1"
        '
        'eventDrivenActivity2
        '
        Me.eventDrivenActivity2.Activities.Add(Me.onTaskChanged1)
        Me.eventDrivenActivity2.Activities.Add(Me.ifElseActivity1)
        Me.eventDrivenActivity2.Name = "eventDrivenActivity2"
        '
        'stateInProgressInitialization
        '
        Me.stateInProgressInitialization.Activities.Add(Me.createTask1)
        Me.stateInProgressInitialization.Name = "stateInProgressInitialization"
        '
        'eventDrivenActivity1
        '
        Me.eventDrivenActivity1.Activities.Add(Me.onWorkflowActivated1)
        Me.eventDrivenActivity1.Activities.Add(Me.logToHistoryListActivity1)
        Me.eventDrivenActivity1.Activities.Add(Me.setStateActivity1)
        Me.eventDrivenActivity1.Name = "eventDrivenActivity1"
        '
        'stateFinished
        '
        Me.stateFinished.Name = "stateFinished"
        '
        'stateReview
        '
        Me.stateReview.Activities.Add(Me.stateInitializationActivity1)
        Me.stateReview.Activities.Add(Me.eventDrivenActivity3)
        Me.stateReview.Name = "stateReview"
        '
        'stateInProgress
        '
        Me.stateInProgress.Activities.Add(Me.stateInProgressInitialization)
        Me.stateInProgress.Activities.Add(Me.eventDrivenActivity2)
        Me.stateInProgress.Name = "stateInProgress"
        '
        'InitialState
        '
        Me.InitialState.Activities.Add(Me.eventDrivenActivity1)
        Me.InitialState.Name = "InitialState"
        '
        'Workflow1
        '
        Me.Activities.Add(Me.InitialState)
        Me.Activities.Add(Me.stateInProgress)
        Me.Activities.Add(Me.stateReview)
        Me.Activities.Add(Me.stateFinished)
        Me.CompletedStateName = "stateFinished"
        Me.DynamicUpdateCondition = Nothing
        Me.InitialStateName = "InitialState"
        Me.Name = "Workflow1"
        Me.CanModifyActivities = False

    End Sub
    Private setStateActivity4 As System.Workflow.Activities.SetStateActivity
    Private setStateActivity3 As System.Workflow.Activities.SetStateActivity
    Private logToHistoryListActivity4 As Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity
    Private logToHistoryListActivity3 As Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity
    Private ifElseBranchActivity6 As System.Workflow.Activities.IfElseBranchActivity
    Private ifElseBranchActivity5 As System.Workflow.Activities.IfElseBranchActivity
    Private ifElseActivity3 As System.Workflow.Activities.IfElseActivity
    Private ifElseBranchActivity4 As System.Workflow.Activities.IfElseBranchActivity
    Private ifElseBranchActivity3 As System.Workflow.Activities.IfElseBranchActivity
    Private ifElseActivity2 As System.Workflow.Activities.IfElseActivity
    Private onTaskChanged2 As Microsoft.SharePoint.WorkflowActions.OnTaskChanged
    Private eventDrivenActivity3 As System.Workflow.Activities.EventDrivenActivity
    Private setStateActivity2 As System.Workflow.Activities.SetStateActivity
    Private logToHistoryListActivity2 As Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity
    Private createReviewTask As Microsoft.SharePoint.WorkflowActions.CreateTask
    Private stateInitializationActivity1 As System.Workflow.Activities.StateInitializationActivity
    Private ifElseBranchActivity2 As System.Workflow.Activities.IfElseBranchActivity
    Private ifElseBranchActivity1 As System.Workflow.Activities.IfElseBranchActivity
    Private ifElseActivity1 As System.Workflow.Activities.IfElseActivity
    Private onTaskChanged1 As Microsoft.SharePoint.WorkflowActions.OnTaskChanged
    Private eventDrivenActivity2 As System.Workflow.Activities.EventDrivenActivity
    Private onWorkflowActivated1 As Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated
    Private eventDrivenActivity1 As System.Workflow.Activities.EventDrivenActivity
    Private createTask1 As Microsoft.SharePoint.WorkflowActions.CreateTask
    Private setStateActivity1 As System.Workflow.Activities.SetStateActivity
    Private logToHistoryListActivity1 As Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity
    Private stateInProgressInitialization As System.Workflow.Activities.StateInitializationActivity
    Private stateFinished As System.Workflow.Activities.StateActivity
    Private stateReview As System.Workflow.Activities.StateActivity
    Private stateInProgress As System.Workflow.Activities.StateActivity
    Private InitialState As System.Workflow.Activities.StateActivity


End Class
