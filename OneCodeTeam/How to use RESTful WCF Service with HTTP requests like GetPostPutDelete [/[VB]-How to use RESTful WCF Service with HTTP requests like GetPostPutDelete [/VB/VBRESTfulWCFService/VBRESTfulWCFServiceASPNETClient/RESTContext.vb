'****************************** Module Header ******************************\
' Module Name:  RESTContext.vb
' Project:      VBRESTfulWCFServiceASPNETClient
' Copyright (c) Microsoft Corporation.
'
' REST context class to address Get/Post/Delete/Put
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Net
Imports System.IO

''' <summary>
''' REST context class
''' </summary>
''' <typeparam name="T">Object like: User</typeparam>
Public Class RESTContext(Of T)

#Region "Fields"

    Private ReadOnly basicUrl As String
    Private m_StrMessage As String
    Private httpRequest As HttpWebRequest
    Private httpResponse As HttpWebResponse
    Private dataStream As Stream
    Private streamReader As StreamReader

#End Region

#Region "Constructor"

    Friend Sub New(ByVal url As String)
        basicUrl = url & "/{0}/{1}"
    End Sub

#End Region

#Region "Properties"

    ''' <summary>
    ''' Message property
    ''' </summary>
    Public Property StrMessage() As String
        Get
            Return m_StrMessage
        End Get
        Private Set(ByVal value As String)
            m_StrMessage = value
        End Set
    End Property

#End Region

#Region "Methods"

    ''' <summary>
    ''' Send the request to WCF Service
    ''' </summary>
    ''' <param name="template">Object template like: User</param>
    ''' <param name="action">Action like: Delete</param>
    ''' <param name="method">Request method</param>
    ''' <param name="t">Object like: User</param>
    Private Sub SendRequest(ByVal template As String,
                            ByVal action As String,
                            ByVal method As HttpMethod,
                            ByVal t As T)
        Dim jsonData As String = JsonHelp.JsonSerialize(Of T)(t)
        If String.IsNullOrEmpty(jsonData) Then
            Return
        End If

        Dim data As Byte() = UnicodeEncoding.UTF8.GetBytes(jsonData)

        httpRequest = DirectCast(WebRequest.Create(String.Format(basicUrl, template, action)), HttpWebRequest)
        httpRequest.Method = method.ToString()
        httpRequest.ContentType = "application/json"
        httpRequest.ContentLength = data.Length

        Try
            Using dataStream = httpRequest.GetRequestStream()
                dataStream.Write(data, 0, data.Length)
            End Using
        Catch we As WebException
            StrMessage = we.Message
        End Try
    End Sub


    ''' <summary>
    ''' Get the response from WCF Service
    ''' </summary>
    ''' <param name="template">Object template like: User</param>
    ''' <param name="action">Action like: Delete</param>
    ''' <param name="method">Request method</param>
    ''' <returns>Return the result from WCF Service</returns>
    Private Function GetResponse(ByVal template As String,
                                 ByVal action As String,
                                 ByVal method As HttpMethod) As String
        Dim responseData As String = String.Empty

        httpRequest = DirectCast(WebRequest.Create(String.Format(basicUrl, template, action)), HttpWebRequest)
        httpRequest.Method = method.ToString()

        Try
            Using httpResponse = TryCast(httpRequest.GetResponse(), HttpWebResponse)
                dataStream = httpResponse.GetResponseStream()

                Using streamReader = New StreamReader(dataStream)
                    responseData = streamReader.ReadToEnd()
                End Using
            End Using
        Catch we As WebException
            StrMessage = we.Message
        Catch pve As ProtocolViolationException
            StrMessage = pve.Message
        End Try

        Return responseData
    End Function

    ''' <summary>
    ''' Get all
    ''' </summary>
    ''' <returns>Return a object list like: List(User)</returns>
    Friend Function GetAll() As List(Of T)
        Dim data As String = GetResponse("User", "All", HttpMethod.[GET])
        If String.IsNullOrEmpty(data) Then
            Return Nothing
        End If

        Return JsonHelp.JsonDeserialize(Of List(Of T))(data)
    End Function

    ''' <summary>
    ''' Create a object like: User
    ''' </summary>
    ''' <param name="t">Object like User</param>
    Friend Sub Create(ByVal t As T)
        SendRequest("User", "Create", HttpMethod.POST, t)
    End Sub


    ''' <summary>
    ''' Update a object like: User
    ''' </summary>
    ''' <param name="t">Object like User</param>
    Friend Sub Update(ByVal t As T)
        SendRequest("User", "Edit", HttpMethod.PUT, t)
    End Sub

    ''' <summary>
    ''' Delete a object like: User
    ''' </summary>
    ''' <typeparam name="S">Type of object member like: int</typeparam>
    ''' <param name="id">Object member like: User's id</param>
    Friend Sub Delete(Of S)(ByVal id As S)
        GetResponse("User", String.Format("Delete/{0}", id), HttpMethod.DELETE)
    End Sub

#End Region

End Class

#Region "HttpMethod Class"

''' <summary>
''' Class to simulate an enum
''' </summary>
Class HttpMethod
    Private method As String

    Public Sub New(ByVal method As String)
        Me.method = method
    End Sub

    Public Shared ReadOnly [GET] As New HttpMethod("GET")
    Public Shared ReadOnly POST As New HttpMethod("POST")
    Public Shared ReadOnly PUT As New HttpMethod("PUT")
    Public Shared ReadOnly DELETE As New HttpMethod("DELETE")

    Public Overrides Function ToString() As String
        Return method
    End Function
End Class

#End Region

