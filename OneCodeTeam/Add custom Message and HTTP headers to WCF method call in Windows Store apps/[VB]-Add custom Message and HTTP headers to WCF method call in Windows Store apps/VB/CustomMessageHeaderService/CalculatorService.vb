'****************************** Module Header ******************************\
' Module Name:  CalculatorService.vb
' Project:      CustomMessageHeaderService
' Copyright (c) Microsoft Corporation.
' 
' This class is the Service contract realization of a simple Calculator.
' It also deals with the UserInfo object instance from IncomingMessageHeaders.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Imports System.ServiceModel
Imports System.ServiceModel.Channels


Public Class CalculatorService
    Implements ICalculatorService

    ' This instance is used to store the received SOAP Header information.
    Private _incomingUserInfo As UserInfo
    ' It's used to store the received HTTP Header information.
    Private strIncomingHTTPHeader As String
    Public Sub New()
        Dim intHeaderIndex As Integer = OperationContext.Current.IncomingMessageHeaders.FindHeader("UserInfo", "http://tempuri.org")
        If intHeaderIndex <> -1 Then
            _incomingUserInfo = OperationContext.Current.IncomingMessageHeaders.GetHeader(Of UserInfo)(intHeaderIndex)
        End If

        strIncomingHTTPHeader = DirectCast(OperationContext.Current.IncomingMessageProperties("httpRequest"), HttpRequestMessageProperty).Headers("MyHttpHeader")
    End Sub

    ''' <summary>
    ''' Add the two number and output the IncomingMessageHeaders infomation.
    ''' </summary>
    ''' <param name="n1">number 1</param>
    ''' <param name="n2">number 2</param>
    ''' <returns></returns>
    Public Function Add(n1 As Double, n2 As Double) As Double Implements ICalculatorService.Add

        If _incomingUserInfo IsNot Nothing Then
            Console.WriteLine("Incoming User Info: FirstName:{0}, LastName:{1}, Age:{2}", _incomingUserInfo.FirstName, _incomingUserInfo.LastName, _incomingUserInfo.Age)
        Else
            Console.WriteLine("There was no incoming user information")
        End If

        Console.WriteLine("Incoming HTTP Header Value: {0}", If((strIncomingHTTPHeader IsNot Nothing), strIncomingHTTPHeader, "null"))
        Return n1 + n2
    End Function

End Class
