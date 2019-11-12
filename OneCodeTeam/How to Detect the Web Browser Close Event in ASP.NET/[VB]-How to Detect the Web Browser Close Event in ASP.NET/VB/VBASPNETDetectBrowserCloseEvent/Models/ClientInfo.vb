'**************************** Module Header ********************************\
' Module Name:    ClientInfo.vb
' Project:        VBASPNETDetectBrowserCloseEvent
' Copyright (c) Microsoft Corporation
'
' As we know, HTTP is a stateless protocol, so the browser doesn't keep connecting 
' to the server. When users try to close the browser using alt-f4, browser close(X) 
' and right click on browser and close, all these methods are working fine, 
' but it's not possible to tell the server that the browser is closed.
'
' This class is the client entity.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Public Class ClientInfo
    Private Shared filePath As String = HttpContext.Current.Server.MapPath("~/App_Data/ClientInfos.xml")
    Public Shared ClientInfoList As New List(Of ClientInfo)()

    ' ClientID
    Private _clientID As String

    ' Last ActiveTime of the client
    Private _activeTime As DateTime

    ' Last RefreshTime of the iframe
    Private _refreshTime As DateTime

    Public Property ClientID() As String
        Get
            Return _clientID
        End Get
        Set(value As String)
            _clientID = value
        End Set
    End Property

    Public Property ActiveTime() As DateTime
        Get
            Return _activeTime
        End Get
        Set(value As DateTime)
            _activeTime = value
        End Set
    End Property

    Public Property RefreshTime() As DateTime
        Get
            Return _refreshTime
        End Get
        Set(value As DateTime)
            _refreshTime = value
        End Set
    End Property

    ''' <summary>
    ''' Search the client by clientID
    ''' </summary>
    ''' <param name="clientList">ClientList</param>
    ''' <param name="strClientID">ClientID</param>
    Public Shared Function GetClinetInfoByClientID(clientList As List(Of ClientInfo), strClientID As String) As ClientInfo
        For i As Integer = 0 To clientList.Count - 1
            If clientList(i).ClientID = strClientID Then
                Return clientList(i)
            End If
        Next
        Dim clientInfo As New ClientInfo()
        ClientInfoList.Add(clientInfo)
        Return clientInfo
    End Function

    Public Sub InsertClientInfo()
        Dim xDoc As XDocument = XDocument.Load(filePath)
        xDoc.Root.Add(New XElement("ClientInfo", New XElement("ClientID", Me.ClientID), New XElement("ActiveTime", Me.ActiveTime), New XElement("RefreshTime", Me.RefreshTime)))
        xDoc.Save(filePath)
    End Sub

    Public Shared Function GetClientInfoByID(ClientID As String) As ClientInfo
        Dim xDoc As XDocument = XDocument.Load(filePath)
        Dim query = From clientInfo In xDoc.Root.Elements() Where clientInfo.Element("ClientID").Value = ClientID
        If query.Count() > 0 Then
            Dim clientInfo As New ClientInfo()
            clientInfo.ClientID = query.First().Element("ClientID").Value
            clientInfo.ActiveTime = DateTime.Parse(query.First().Element("ActiveTime").Value)
            clientInfo.RefreshTime = DateTime.Parse(query.First().Element("RefreshTime").Value)
            Return clientInfo
        End If
        Return Nothing
    End Function
End Class