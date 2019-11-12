Option Explicit On
Option Strict On

Imports System
Imports System.Runtime.InteropServices
Imports System.Security.Permissions
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Administration
Imports Microsoft.SharePoint.Security

''' <summary>
''' This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
''' </summary>
''' <remarks>
''' The GUID attached to this class may be used during packaging and should not be modified.
''' </remarks>

<GuidAttribute("96d253cd-9870-432c-ae85-c567db41e397")> _
Public Class Feature1EventReceiver 
    Inherits SPFeatureReceiver

    Const TIMER_JOB_NAME As String = "AZURECaller"

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
        'Create a configure the new Timer job
        Dim azureTimerJob As CallingTimerJob = New CallingTimerJob(TIMER_JOB_NAME, site.WebApplication)
        Dim jobSchedule As SPMinuteSchedule = New SPMinuteSchedule()
        jobSchedule.BeginSecond = 0
        jobSchedule.EndSecond = 59
        jobSchedule.Interval = 5
        azureTimerJob.Schedule = jobSchedule
        azureTimerJob.Update()
    End Sub

    Public Overrides Sub FeatureDeactivating(ByVal properties As SPFeatureReceiverProperties)
        Dim site As SPSite = DirectCast(properties.Feature.Parent, SPSite)
        'Make sure the timer job isn't already registered
        For Each job As SPJobDefinition In site.WebApplication.JobDefinitions
            If job.Name = TIMER_JOB_NAME Then
                job.Delete()
            End If
        Next
    End Sub
End Class
