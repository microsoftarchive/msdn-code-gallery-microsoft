Imports Windows.ApplicationModel.Background

'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "Network Status Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Network Status with Internet Present ", .ClassType = GetType(NetworkStatusApp.NetworkStatusWithInternetPresent)}
                                                                 }
    End Class

    Public Class Scenario
        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType As Type
            Get
                Return m_ClassType
            End Get
            Set(value As Type)
                m_ClassType = value
            End Set
        End Property
        Private m_ClassType As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace



Class BackgroundTaskSample
    Public Const SampleBackgroundTaskEntryPoint As String = "NetworkStatusTask.NetworkStatusBackgroundTask"
    Public Const SampleBackgroundTaskWithConditionName As String = "NetworkStatusWithInternetPresent"

    ''' <summary>
    ''' Register a background task with the specified taskEntryPoint, name, trigger,
    ''' and condition (optional).
    ''' </summary>
    ''' <param name="taskEntryPoint">Task entry point for the background task.</param>
    ''' <param name="name">A name for the background task.</param>
    ''' <param name="trigger">The trigger for the background task.</param>
    ''' <param name="condition">An optional conditional event that must be true for the task to fire.</param>
    Public Shared Function RegisterBackgroundTask(taskEntryPoint As [String], name As [String], trigger As IBackgroundTrigger, condition As IBackgroundCondition) As BackgroundTaskRegistration
        Dim builder = New BackgroundTaskBuilder()

        builder.Name = name
        builder.TaskEntryPoint = taskEntryPoint
        builder.SetTrigger(trigger)

        If condition IsNot Nothing Then
            builder.AddCondition(condition)
        End If

        Dim task As BackgroundTaskRegistration = builder.Register()

        Return task
    End Function

    ''' <summary>
    ''' Unregister background tasks with specified name.
    ''' </summary>
    ''' <param name="name">Name of the background task to unregister.</param>
    Public Shared Sub UnregisterBackgroundTasks(name As String)
        '
        ' Loop through all background tasks and unregister any with SampleBackgroundTaskName or
        ' SampleBackgroundTaskWithConditionName.
        '
        For Each cur In BackgroundTaskRegistration.AllTasks
            If cur.Value.Name = name Then
                cur.Value.Unregister(True)
            End If
        Next
    End Sub
End Class


