Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Administration
Imports Microsoft.SharePoint.Security

''' <summary>
''' For a custom timer job, you must use this feature receiver to install and configure
''' the SPJobDefinition class
''' </summary>
''' <remarks>
''' The GUID attached to this class may be used during packaging and should not be modified.
''' </remarks>

<GuidAttribute("d7faf5a5-9d5e-41c3-b768-8a30c6136990")> _
Public Class CustomTimerJobFeatureEventReceiver 
    Inherits SPFeatureReceiver

    Const TIMER_JOB_NAME As String = "DemoTimerJob"

    Public Overrides Sub FeatureActivated(ByVal properties As SPFeatureReceiverProperties)
        'In this event we create and install the timer job
        'Start by finding the SPSite.
        Dim site As SPSite = DirectCast(properties.Feature.Parent, SPSite)
        'Make sure the timer job isn't already registered
        For Each job As SPJobDefinition In site.WebApplication.JobDefinitions
            If job.Name = TIMER_JOB_NAME Then
                job.Delete()
            End If
        Next
        'Create a new Timer job
        Dim newTimerJob As GENERAL_CustomTimerJob = New GENERAL_CustomTimerJob(TIMER_JOB_NAME, site.WebApplication)
        'Configure the schedule and save it
        Dim jobSchedule As SPMinuteSchedule = New SPMinuteSchedule()
        jobSchedule.BeginSecond = 0
        jobSchedule.EndSecond = 59
        jobSchedule.Interval = 5
        newTimerJob.Schedule = jobSchedule
        newTimerJob.Update()
    End Sub

    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)
        'In this event we must clean up by deleting the timer job
        Dim site As SPSite = DirectCast(properties.Feature.Parent, SPSite)
        'Locate the right timer job
        For Each job As SPJobDefinition In site.WebApplication.JobDefinitions
            If job.Name = TIMER_JOB_NAME Then
                'This one is the right job. Delete it.
                job.Delete()
            End If
        Next
    End Sub

End Class
