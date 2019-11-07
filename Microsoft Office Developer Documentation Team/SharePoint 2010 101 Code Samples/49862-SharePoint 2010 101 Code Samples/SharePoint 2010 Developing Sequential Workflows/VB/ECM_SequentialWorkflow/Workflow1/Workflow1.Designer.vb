
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Workflow1

    'NOTE: The following procedure is required by the Workflow Designer
    'It can be modified using the Workflow Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Private Sub InitializeComponent()
        Me.CanModifyActivities = True
        Dim correlationtoken1 As System.Workflow.Runtime.CorrelationToken = New System.Workflow.Runtime.CorrelationToken()
        Dim codecondition1 As System.Workflow.Activities.CodeCondition = New System.Workflow.Activities.CodeCondition()
        Dim activitybind1 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Me.onWorkflowItemChanged1 = New Microsoft.SharePoint.WorkflowActions.OnWorkflowItemChanged()
        Me.whileActivity1 = New System.Workflow.Activities.WhileActivity()
        Me.onWorkflowActivated1 = New Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated()
        '
        'onWorkflowItemChanged1
        '
        Me.onWorkflowItemChanged1.AfterProperties = Nothing
        Me.onWorkflowItemChanged1.BeforeProperties = Nothing
        correlationtoken1.Name = "workflowToken"
        correlationtoken1.OwnerActivityName = "Workflow1"
        Me.onWorkflowItemChanged1.CorrelationToken = correlationtoken1
        Me.onWorkflowItemChanged1.Name = "onWorkflowItemChanged1"
        AddHandler Me.onWorkflowItemChanged1.Invoked, AddressOf Me.onWorkflowItemChanged
        '
        'whileActivity1
        '
        Me.whileActivity1.Activities.Add(Me.onWorkflowItemChanged1)
        AddHandler codecondition1.Condition, AddressOf Me.isWorkflowPending
        Me.whileActivity1.Condition = codecondition1
        Me.whileActivity1.Name = "whileActivity1"
        '
        'onWorkflowActivated1
        '
        Me.onWorkflowActivated1.CorrelationToken = correlationtoken1
        Me.onWorkflowActivated1.EventName = "OnWorkflowActivated"
        Me.onWorkflowActivated1.Name = "onWorkflowActivated1"
        activitybind1.Name = "Workflow1"
        activitybind1.Path = "workflowProperties"
        AddHandler Me.onWorkflowActivated1.Invoked, AddressOf Me.onWorkflowActivated
        Me.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, CType(activitybind1, System.Workflow.ComponentModel.ActivityBind))
        '
        'Workflow1
        '
        Me.Activities.Add(Me.onWorkflowActivated1)
        Me.Activities.Add(Me.whileActivity1)
        Me.Name = "Workflow1"
        Me.CanModifyActivities = False

    End Sub
    Private whileActivity1 As System.Workflow.Activities.WhileActivity
    Private onWorkflowItemChanged1 As Microsoft.SharePoint.WorkflowActions.OnWorkflowItemChanged
    Private onWorkflowActivated1 As Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated


End Class
