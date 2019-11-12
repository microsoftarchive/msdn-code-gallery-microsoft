
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WCF_CallingWorkflow

    'NOTE: The following procedure is required by the Workflow Designer
    'It can be modified using the Workflow Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Private Sub InitializeComponent()
        Me.CanModifyActivities = True
        Dim correlationtoken1 As System.Workflow.Runtime.CorrelationToken = New System.Workflow.Runtime.CorrelationToken()
        Dim activitybind1 As System.Workflow.ComponentModel.ActivityBind = New System.Workflow.ComponentModel.ActivityBind()
        Me.onWorkflowActivated1 = New Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated()
        '
        'onWorkflowActivated1
        '
        correlationtoken1.Name = "workflowToken"
        correlationtoken1.OwnerActivityName = "WCF_CallingWorkflow"
        Me.onWorkflowActivated1.CorrelationToken = correlationtoken1
        Me.onWorkflowActivated1.EventName = "OnWorkflowActivated"
        Me.onWorkflowActivated1.Name = "onWorkflowActivated1"
        activitybind1.Name = "WCF_CallingWorkflow"
        activitybind1.Path = "workflowProperties"
        AddHandler Me.onWorkflowActivated1.Invoked, AddressOf Me.onWorkflowActivated1_Invoked
        Me.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, CType(activitybind1, System.Workflow.ComponentModel.ActivityBind))
        '
        'WCF_CallingWorkflow
        '
        Me.Activities.Add(Me.onWorkflowActivated1)
        Me.Name = "WCF_CallingWorkflow"
        Me.CanModifyActivities = False

    End Sub
    Private onWorkflowActivated1 As Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated


End Class
