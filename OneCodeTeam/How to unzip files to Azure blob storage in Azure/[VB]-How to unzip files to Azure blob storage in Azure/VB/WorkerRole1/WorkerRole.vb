'/****************************** Module Header ******************************\
'Module Name:  WorkerRole.vb
'Project:      VBAzureUnzipFilesToBlobStorage
'Copyright (c) Microsoft Corporation.
' 
'For users with large amounts of unstructured data to store in the cloud, Blob storage offers a cost-effective and 
'scalable solution ,users can store documents ,social data ,images and text etc.
'
'This project  demonstrates how to unzip files to Azure blob storage in Azure.
'Uploading thousands of small files one-by-one is very slow. 
'It would be great if we could upload a zip file to Azure and unzip it directly into blob storage in Azure.
' 
'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
'All other rights reserved.
' 
'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
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
Imports UnZipWCFService

Public Class WorkerRole
    Inherits RoleEntryPoint

    Private cancellationTokenSource As CancellationTokenSource = New CancellationTokenSource
    Private runCompleteEvent As ManualResetEvent = New ManualResetEvent(False)

    Public Overrides Sub Run()
        Trace.TraceInformation("WorkerRole1 is running")
        Try
            Dim localResource As LocalResource = RoleEnvironment.GetLocalResource("LocalStorage1")

            UnZipService.strLoacalStorage = localResource.RootPath
        Catch ex As Exception
            Trace.TraceInformation(ex.Message)
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

        Trace.TraceInformation("WorkerRole1 has been started")
        Try
            ' Start the WCF host.
            Dim host As New ServiceHost(GetType(UnZipService))
            host.Open()

            Trace.TraceInformation("ServiceHost has been started")
        Catch ex As Exception
            Trace.TraceInformation("ServiceHost failed " & ex.Message)
        End Try
        Return result
    End Function

    Public Overrides Sub OnStop()

        Trace.TraceInformation("WorkerRole1 is stopping")

        Me.cancellationTokenSource.Cancel()
        Me.runCompleteEvent.WaitOne()

        MyBase.OnStop()

        Trace.TraceInformation("WorkerRole1 has stopped")

    End Sub

    Private Async Function RunAsync(ByVal cancellationToken As CancellationToken) As Task

        ' TODO: Replace the following with your own logic.
        While Not cancellationToken.IsCancellationRequested
            Trace.TraceInformation("Working")
            Await Task.Delay(1000)
        End While

    End Function
End Class
