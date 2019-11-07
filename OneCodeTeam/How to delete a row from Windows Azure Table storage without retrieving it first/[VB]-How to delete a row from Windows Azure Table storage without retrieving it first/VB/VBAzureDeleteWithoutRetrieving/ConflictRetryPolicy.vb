'***************************** Module Header ******************************\
' Module Name:	ConflictRetryPolicy.vb
' Project:		VBAzureDeleteWithoutRetrieving
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to reduce the request to Azure storage service.
'
' This class implements IRetryPolicy. It will handle the conflict error when 
' Azure Data Center has transient fault, and retry the request later.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/
Imports Microsoft.WindowsAzure.Storage.RetryPolicies
Imports System.ServiceModel

Public Class ConflictRetryPolicy
    Implements IRetryPolicy

    Private maxRetryAttemps As Integer = 10
    Private defaultRetryInterval As TimeSpan = TimeSpan.FromSeconds(5)

    Public Sub New(deltaBackoff As TimeSpan, retryAttempts As Integer)
        maxRetryAttemps = retryAttempts
        defaultRetryInterval = deltaBackoff
    End Sub

    Public Function CreateInstance() As IRetryPolicy Implements IRetryPolicy.CreateInstance
        Return New ConflictRetryPolicy(TimeSpan.FromSeconds(5), 10)
    End Function

    Public Function ShouldRetry(currentRetryCount As Integer, statusCode As Integer, lastException As Exception, ByRef retryInterval As TimeSpan, operationContext As Microsoft.WindowsAzure.Storage.OperationContext) As Boolean Implements IRetryPolicy.ShouldRetry
        retryInterval = defaultRetryInterval
        If currentRetryCount >= maxRetryAttemps Then
            Return False
        End If
        If statusCode = 409 Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
