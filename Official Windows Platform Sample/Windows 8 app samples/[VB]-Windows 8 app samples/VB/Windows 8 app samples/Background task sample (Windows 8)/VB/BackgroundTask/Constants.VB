'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Collections.Generic
Imports Windows.ApplicationModel.Background
Imports Windows.Storage

Namespace Global.SDKTemplate

    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage
        ' Change the string below to reflect the name of your sample.
        ' This is used on the main page as the title of the sample.
        Public Const FEATURE_NAME As String = "Background Task Sample"

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Sample background task in VB", .ClassType = GetType(Global.BackgroundTask.SampleBackgroundTask)},
                                                                   New Scenario With {.Title = "Sample background task in VB with a condition", .ClassType = GetType(Global.BackgroundTask.SampleBackgroundTaskWithCondition)},
                                                                   New Scenario With {.Title = "Servicing complete background task in VB", .ClassType = GetType(Global.BackgroundTask.ServicingCompleteTask)},
                                                                   New Scenario With {.Title = "Sample background task with time trigger", .ClassType = GetType(Global.BackgroundTask.TimeTriggeredTask)}}
    End Class

    Public Class Scenario
        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType() As Type
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
    Public Const SampleBackgroundTaskEntryPoint As String = "Tasks.SampleBackgroundTask"
    Public Const SampleBackgroundTaskName As String = "SampleBackgroundTask"
    Public Shared SampleBackgroundTaskProgress As String = ""
    Public Shared SampleBackgroundTaskRegistered As Boolean = False

    Public Const SampleBackgroundTaskWithConditionName As String = "SampleBackgroundTaskWithCondition"
    Public Shared SampleBackgroundTaskWithConditionProgress As String = ""
    Public Shared SampleBackgroundTaskWithConditionRegistered As Boolean = False

    Public Const ServicingCompleteTaskEntryPoint As String = "Tasks.ServicingComplete"
    Public Const ServicingCompleteTaskName As String = "ServicingCompleteTask"
    Public Shared ServicingCompleteTaskProgress As String = ""
    Public Shared ServicingCompleteTaskRegistered As Boolean = False

    Public Const TimeTriggeredTaskName As String = "TimeTriggeredTask"
    Public Shared TimeTriggeredTaskProgress As String = ""
    Public Shared TimeTriggeredTaskRegistered As Boolean = False

    ''' <summary>
    ''' Register a background task with the specified taskEntryPoint, name, trigger,
    ''' and condition (optional).
    ''' </summary>
    ''' <param name="taskEntryPoint">Task entry point for the background task.</param>
    ''' <param name="name">A name for the background task.</param>
    ''' <param name="trigger">The trigger for the background task.</param>
    ''' <param name="condition">An optional conditional event that must be true for the task to fire.</param>
    Public Shared Function RegisterBackgroundTask(taskEntryPoint As String, name As String, trigger As IBackgroundTrigger, condition As IBackgroundCondition) As BackgroundTaskRegistration
        Dim builder = New BackgroundTaskBuilder()

        builder.Name = name
        builder.TaskEntryPoint = taskEntryPoint
        builder.SetTrigger(trigger)

        If condition IsNot Nothing Then
            builder.AddCondition(condition)
        End If

        Dim task As BackgroundTaskRegistration = builder.Register()

        UpdateBackgroundTaskStatus(name, True)

        '
        ' Remove previous completion status from local settings.
        '
        Dim settings = ApplicationData.Current.LocalSettings
        settings.Values.Remove(name)

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

        UpdateBackgroundTaskStatus(name, False)
    End Sub

    ''' <summary>
    ''' Registers a background task for the servicing-complete system event.
    ''' This event occurs when the application is updated.
    ''' </summary>
    Public Shared Sub RegisterServicingCompleteTask()
        '
        ' Check whether the servicing-complete background task is already registered.
        '
        For Each cur In BackgroundTaskRegistration.AllTasks
            If cur.Value.Name = ServicingCompleteTaskName Then
                '
                ' The task is already registered.
                '
                UpdateBackgroundTaskStatus(ServicingCompleteTaskName, True)
                Return
            End If
        Next

        '
        ' The servicing-complete background task is not already registered.
        '
        RegisterBackgroundTask(ServicingCompleteTaskEntryPoint, ServicingCompleteTaskName, New SystemTrigger(SystemTriggerType.ServicingComplete, False), Nothing)
    End Sub

    ''' <summary>
    ''' Store the registration status of a backgroudn task with a given name.
    ''' </summary>
    ''' <param name="name">Name of background task to store registration status for.</param>
    ''' <param name="registered">TRUE if registered, FALSE if unregistered.</param>
    Public Shared Sub UpdateBackgroundTaskStatus(name As String, registered As Boolean)

        Select Case name
            Case SampleBackgroundTaskName
                SampleBackgroundTaskRegistered = registered
                Exit Select
            Case SampleBackgroundTaskWithConditionName
                SampleBackgroundTaskWithConditionRegistered = registered
                Exit Select
            Case ServicingCompleteTaskName
                ServicingCompleteTaskRegistered = registered
                Exit Select
            Case TimeTriggeredTaskName
                TimeTriggeredTaskRegistered = registered
                Exit Select
        End Select
    End Sub

    ''' <summary>
    ''' Get the registration status of a background task.
    ''' </summary>
    ''' <param name="name">Name of background task to get registration status.</param>
    Public Shared Function GetBackgroundTaskStatus(name As String)

        Dim registered = False
        Select Case name
            Case SampleBackgroundTaskName
                registered = SampleBackgroundTaskRegistered
                Exit Select
            Case SampleBackgroundTaskWithConditionName
                registered = SampleBackgroundTaskWithConditionRegistered
                Exit Select
            Case ServicingCompleteTaskName
                registered = ServicingCompleteTaskRegistered
                Exit Select
            Case TimeTriggeredTaskName
                registered = TimeTriggeredTaskRegistered
                Exit Select
        End Select

        Dim status = If(registered, "Registered", "Unregistered")

        Dim settings = ApplicationData.Current.LocalSettings
        If (settings.Values.ContainsKey(name)) Then
            status += " - " + settings.Values(name).ToString()
        End If

        Return status

    End Function
End Class

