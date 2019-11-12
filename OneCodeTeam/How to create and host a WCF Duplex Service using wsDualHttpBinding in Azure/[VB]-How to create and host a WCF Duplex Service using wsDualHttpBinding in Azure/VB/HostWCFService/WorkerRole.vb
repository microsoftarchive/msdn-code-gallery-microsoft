'/****************************** Module Header ******************************\
'* Module Name:  WorkerRole.vb
'* Project:      HostWCFService
'* Copyright (c) Microsoft Corporation.

'* WSDualHttpBinding supports duplex services. A duplex service is a service that uses duplex message patterns.
'* These patterns provide the ability for a service to communicate back to the client via a callback.
'* 
'* A workrole holds the WCF service .
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/



Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Net
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.WindowsAzure
Imports Microsoft.WindowsAzure.Diagnostics
Imports Microsoft.WindowsAzure.ServiceRuntime
Imports Microsoft.WindowsAzure.Storage
Imports System.ServiceModel

Public Class WorkerRole
    Inherits RoleEntryPoint

    Private cancellationTokenSource As CancellationTokenSource = New CancellationTokenSource
    Private runCompleteEvent As ManualResetEvent = New ManualResetEvent(False)
    Private serviceHost As ServiceHost
    Public Overrides Sub Run()
        Trace.TraceInformation("HostWCFService is running")

        Trace.TraceInformation(" Try to start WCF service host...")
        Me.serviceHost = New ServiceHost(GetType(ServiceCalculate.ServiceCalculate))

        Try

            Me.serviceHost.Open()
            Trace.TraceInformation("WCF service hosting started successfully.")
        Catch timeoutException As TimeoutException
            Trace.TraceError("The service operation timed out. {0}", timeoutException.Message)
        Catch communicationException As CommunicationException
            Trace.TraceError("Could not start WCF service host. {0}", communicationException.Message)
        End Try

        Try
            Me.RunAsync(Me.cancellationTokenSource.Token).Wait()
        Finally
            Me.runCompleteEvent.Set()
        End Try
    End Sub

    Public Overrides Function OnStart() As Boolean
        ' Set the maximum number of concurrent connections
        ServicePointManager.DefaultConnectionLimit = 12

        ' For information on handling configuration changes
        ' see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
        Dim result As Boolean = MyBase.OnStart()

        Trace.TraceInformation("HostWCFService has been started")

        Return result
    End Function

    Public Overrides Sub OnStop()

        Trace.TraceInformation("HostWCFService is stopping")

        Me.cancellationTokenSource.Cancel()
        Me.runCompleteEvent.WaitOne()

        MyBase.OnStop()

        Trace.TraceInformation("HostWCFService has stopped")

    End Sub

    Private Async Function RunAsync(ByVal cancellationToken As CancellationToken) As Task

        ' TODO: Replace the following with your own logic.
        While Not cancellationToken.IsCancellationRequested
            Trace.TraceInformation("Working")
            Await Task.Delay(1000)
        End While

    End Function
End Class
