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
''' This is an example state machine workflow with four states, conditional state
''' changes, SharePoint task creation, and event driven activities, and history list
''' logging.
''' </summary>
''' <remarks>
''' This workflow requires a document library called Projects. When a document is 
''' created in the library, a task item is created called "Finish Document" and 
''' stateInProgress is entered. When that task is marked 100% complete, another 
''' task is created called "Review Document" and stateReview is entered.
''' When the second task is marked 100% complete, if the Description is "Approved", 
''' stateFinished is entered and the workflow is complete. Otherwise the workflow
''' returns to stateInProgress and another "Finish Document" task item is created.
''' </remarks>
Public Class Workflow1
    Inherits StateMachineWorkflowActivity

    Public workflowProperties As New SPWorkflowActivationProperties

    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub

    Public createTask1_TaskId1 As System.Guid = Nothing
    Public createTask1_TaskProperties1 As SPWorkflowTaskProperties = New Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties

    'This method sets properties for the first task item
    Private Sub createTask1_MethodInvoking(ByVal sender As System.Object, ByVal e As System.EventArgs)
        createTask1_TaskId1 = Guid.NewGuid()
        createTask1_TaskProperties1.Title = "Finish Document"
        createTask1_TaskProperties1.AssignedTo = "CONTOSO\aris"
        createTask1_TaskProperties1.DueDate = Date.Now.AddDays(1.0)
    End Sub

    Public onTaskChanged1_AfterProperties1 As SPWorkflowTaskProperties = New Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties
    Public onTaskChanged1_BeforeProperties1 As SPWorkflowTaskProperties = New Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties

    Private Sub onTaskChanged1_Invoked(ByVal sender As System.Object, ByVal e As System.Workflow.Activities.ExternalDataEventArgs)
        onTaskChanged1_AfterProperties1 = onTaskChanged1.AfterProperties
        onTaskChanged1_BeforeProperties1 = onTaskChanged1.BeforeProperties
    End Sub

    'This is the code condition for ifElseBranchActivity1
    'It checks if the task is complete
    Private Sub ReadyForReview(ByVal sender As Object, ByVal e As ConditionalEventArgs)
        If onTaskChanged1_AfterProperties1.PercentComplete = 1.0 Then
            e.Result = True
        Else
            e.Result = False
        End If
    End Sub

    Public createReviewTask_TaskId1 As System.Guid = Nothing
    Public createReviewTask_TaskProperties1 As SPWorkflowTaskProperties = New Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties

    'This method sets properties for the Review Document task item
    Private Sub createReviewTask_MethodInvoking(ByVal sender As System.Object, ByVal e As System.EventArgs)
        createReviewTask_TaskId1 = Guid.NewGuid()
        createReviewTask_TaskProperties1.Title = "Review Document"
        createReviewTask_TaskProperties1.AssignedTo = "CONTOSO\danj"
        createReviewTask_TaskProperties1.DueDate = Date.Now.AddDays(1.0)
    End Sub

    Public onTaskChanged2_AfterProperties1 As SPWorkflowTaskProperties = New Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties
    Public onTaskChanged2_BeforeProperties1 As SPWorkflowTaskProperties = New Microsoft.SharePoint.Workflow.SPWorkflowTaskProperties

    Private Sub onTaskChanged2_Invoked(ByVal sender As System.Object, ByVal e As System.Workflow.Activities.ExternalDataEventArgs)
        onTaskChanged1_AfterProperties1 = onTaskChanged2.AfterProperties
        onTaskChanged2_BeforeProperties1 = onTaskChanged2.BeforeProperties
    End Sub

    'This is the code condition for ifElseBranchActivity3
    'It checks if the "Review Document" task is 100% complete
    Private Sub ReviewFinished(ByVal sender As Object, ByVal e As ConditionalEventArgs)
        If onTaskChanged2_AfterProperties1.PercentComplete = 1.0 Then
            e.Result = True
        Else
            e.Result = False
        End If
    End Sub

    'This is the code condition for ifElseBranchActivity5
    'It checks if the Description field in the "Review Document" task is "Approved"
    Private Sub DocApproved(ByVal sender As Object, ByVal e As ConditionalEventArgs)
        If onTaskChanged2_AfterProperties1.Description = "<DIV>Approved</DIV>" Then
            e.Result = True
        Else
            e.Result = False
        End If
    End Sub
End Class