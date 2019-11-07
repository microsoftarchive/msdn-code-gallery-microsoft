Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Drawing
Imports System.Workflow.ComponentModel.Compiler
Imports System.Workflow.ComponentModel.Serialization
Imports System.Workflow.ComponentModel
Imports System.Workflow.ComponentModel.Design
Imports System.Workflow.Runtime
Imports System.Workflow.Activities
Imports System.Workflow.Activities.Rules
Imports Microsoft.SharePoint.Workflow
Imports Microsoft.SharePoint.WorkflowActions

''' <summary>
''' This simple workflow demonstrates how to develop a workflow in Visual Studio
''' and deploy it to a SharePoint list. At each stage in the workflow, the 
''' checkStatus method checks the value of the Document Status property. If the
''' value is "Review Complete" the While Activity can stop looping and the workflow
''' is complete
''' </summary>
''' <remarks>
''' Before you deploy this workflow, you must create a Document Library called 
''' Projects with a Column called "Document Status". This should be a Choice column
''' with several values, one of which is "Review Complete".
''' </remarks>
Public Class Workflow1
    Inherits SequentialWorkflowActivity

    Public workflowProperties As New SPWorkflowActivationProperties

    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub

    Private bIsWorkflowPending As Boolean = True

    Private Sub onWorkflowActivated(ByVal sender As System.Object, ByVal e As System.Workflow.Activities.ExternalDataEventArgs)
        checkStatus()
    End Sub

    Private Sub isWorkflowPending(ByVal sender As System.Object, ByVal e As System.Workflow.Activities.ConditionalEventArgs)
        e.Result = bIsWorkflowPending
    End Sub

    Private Sub onWorkflowItemChanged(ByVal sender As System.Object, ByVal e As System.Workflow.Activities.ExternalDataEventArgs)
        checkStatus()
    End Sub

    Private Sub checkStatus()
        If workflowProperties.Item("Document Status").ToString() = "Review Complete" Then
            bIsWorkflowPending = False
        End If
    End Sub
End Class