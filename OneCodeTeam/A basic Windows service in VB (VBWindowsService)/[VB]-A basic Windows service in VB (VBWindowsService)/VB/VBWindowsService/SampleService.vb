'****************************** Module Header ******************************'
' Module Name:  SampleService.vb
' Project:      VBWindowsService
' Copyright (c) Microsoft Corporation.
' 
' Provides a sample service class that derives from the service base class - 
' System.ServiceProcess.ServiceBase. The sample service logs the service 
' start and stop information to the Application event log, and shows how to 
' run the main function of the service in a thread pool worker thread. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

Imports System.Threading


Public Class SampleService

    Public Sub New()
        InitializeComponent()

        Me.stopping = False
        Me.stoppedEvent = New ManualResetEvent(False)
    End Sub


    ''' <summary>
    ''' The function is executed when a Start command is sent to the service
    ''' by the SCM or when the operating system starts (for a service that 
    ''' starts automatically). It specifies actions to take when the service 
    ''' starts. In this code sample, OnStart logs a service-start message to 
    ''' the Application log, and queues the main service function for 
    ''' execution in a thread pool worker thread.
    ''' </summary>
    ''' <param name="args">Command line arguments</param>
    ''' <remarks>
    ''' A service application is designed to be long running. Therefore, it 
    ''' usually polls or monitors something in the system. The monitoring is 
    ''' set up in the OnStart method. However, OnStart does not actually do 
    ''' the monitoring. The OnStart method must return to the operating 
    ''' system after the service's operation has begun. It must not loop 
    ''' forever or block. To set up a simple monitoring mechanism, one 
    ''' general solution is to create a timer in OnStart. The timer would 
    ''' then raise events in your code periodically, at which time your 
    ''' service could do its monitoring. The other solution is to spawn a 
    ''' new thread to perform the main service functions, which is 
    ''' demonstrated in this code sample.
    ''' </remarks>
    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Log a service start message to the Application log.
        Me.EventLog1.WriteEntry("VBWindowsService in OnStart.")

        ' Queue the main service function for execution in a worker thread.
        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ServiceWorkerThread))
    End Sub


    ''' <summary>
    ''' The method performs the main function of the service. It runs on a 
    ''' thread pool worker thread.
    ''' </summary>
    ''' <param name="state"></param>
    Private Sub ServiceWorkerThread(ByVal state As Object)
        ' Periodically check if the service is stopping.
        Do While Not Me.stopping
            ' Perform main service function here...

            Thread.Sleep(2000)  ' Simulate some lengthy operations.
        Loop

        ' Signal the stopped event.
        Me.stoppedEvent.Set()
    End Sub


    ''' <summary>
    ''' The function is executed when a Stop command is sent to the service 
    ''' by SCM. It specifies actions to take when a service stops running. In 
    ''' this code sample, OnStop logs a service-stop message to the 
    ''' Application log, and waits for the finish of the main service 
    ''' function.
    ''' </summary>
    Protected Overrides Sub OnStop()
        ' Log a service stop message to the Application log.
        Me.EventLog1.WriteEntry("VBWindowsService in OnStop.")

        ' Indicate that the service is stopping and wait for the finish of 
        ' the main service function (ServiceWorkerThread).
        Me.stopping = True
        Me.stoppedEvent.WaitOne()
    End Sub


    Private stopping As Boolean
    Private stoppedEvent As ManualResetEvent

End Class
