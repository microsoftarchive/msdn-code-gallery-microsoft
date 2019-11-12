'***************************** Module Header ******************************\
'* Module Name:    RoomTalker.svc.vb
'* Project:        VBASPNETAJAXWebChat
'* Copyright (c) Microsoft Corporation
'*
'* The project illustrates how to design a simple AJAX web chat application. 
'* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
'* In this sample, we could create a chat room and invite someone
'* else to join in the room and start to chat.
'* 
'* In this file, we create an Ajax-enabled WCF service which used to be called
'* from the client side.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'\****************************************************************************

Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.ServiceModel.Web

<ServiceContract([Namespace]:="http://VBASPNETAJAXWebChat",
    SessionMode:=SessionMode.Allowed)> _
<AspNetCompatibilityRequirements(
    RequirementsMode:=AspNetCompatibilityRequirementsMode.Allowed)> _
Public Class Transition

    <OperationContract()> _
    Public Sub CreateChatRoom(ByVal useralias As String,
                              ByVal roomName As String,
                              ByVal password As String,
                              ByVal maxUser As Integer,
                              ByVal needPassword As Boolean)

        If maxUser < 2 Then
            maxUser = 2
        End If

        Dim roomid As Guid = ChatManager.CreateChatRoom(roomName,
                                                        password,
                                                        False,
                                                        maxUser,
                                                        needPassword)

    End Sub

    <OperationContract()> _
    Public Function JoinChatRoom(ByVal roomid As String,
                                 ByVal [alias] As String) As ChatRoom
        Dim rid As Guid
        If Guid.TryParse(roomid, rid) Then
            ChatManager.JoinChatRoom(rid, HttpContext.Current, [alias])

            Return New ChatRoom(rid)
        Else
            Return Nothing
        End If

    End Function

    <OperationContract()> _
    Public Sub LeaveChatRoom(ByVal roomid As String)
        If roomid Is Nothing Then
            roomid = GetGUIDFromQuery(
                HttpContext.Current.Request.UrlReferrer.Query).ToString()
        End If
        Dim rid As Guid
        If Guid.TryParse(roomid, rid) Then
            ChatManager.LeaveChatRoom(rid, HttpContext.Current)
        Else
            Return
        End If
    End Sub

    <OperationContract()> _
    Public Function GetChatRoomList() As List(Of ChatRoom)
        Dim list As List(Of tblChatRoom) = ChatManager.GetChatRoomList()
        Dim result As New List(Of ChatRoom)()
        For Each room As tblChatRoom In list
            result.Add(New ChatRoom(room.ChatRoomID))
        Next
        Return result
    End Function

    <OperationContract()> _
    Public Function GetChatRoomInfo(ByVal RoomID As String) As ChatRoom
        Dim rim As Guid
        If Guid.TryParse(RoomID, rim) Then
            Return New ChatRoom(rim)
        Else
            Return Nothing
        End If

    End Function

    <OperationContract()> _
    Public Function GetRoomTalkerList() As List(Of RoomTalker)

        Dim result As New List(Of RoomTalker)()
        Dim roomid As Guid = GetGUIDFromQuery(
            HttpContext.Current.Request.UrlReferrer.Query)
        If roomid <> Guid.Empty Then
            Dim talkerList As List(Of tblTalker) =
                ChatManager.GetRoomTalkerList(roomid)
            For Each talker As tblTalker In talkerList
                result.Add(New RoomTalker(talker, HttpContext.Current))
            Next
        End If
        Return result

    End Function

    <OperationContract()> _
    Public Function SendMessage(ByVal message As String) As Boolean
        Dim roomid As Guid = GetGUIDFromQuery(
            HttpContext.Current.Request.UrlReferrer.Query)

        If roomid <> Guid.Empty Then
            Dim talker As tblTalker =
                ChatManager.FindTalker(roomid, HttpContext.Current)
            ChatManager.SendMessage(talker, message)
            Return True
        Else

            Return False
        End If
    End Function

    <OperationContract()> _
    Public Function RecieveMessage() As List(Of Message)
        Dim result As New List(Of Message)()
        Dim roomid As Guid = GetGUIDFromQuery(
            HttpContext.Current.Request.UrlReferrer.Query)

        If roomid <> Guid.Empty Then
            Dim messageList As List(Of tblMessagePool) =
                ChatManager.RecieveMessage(ChatManager.GetChatRoom(roomid))

            For Each msg As tblMessagePool In messageList
                result.Add(New Message(msg, HttpContext.Current))
            Next
        End If
        Return result

    End Function

    Private Function GetGUIDFromQuery(ByVal query As String) As Guid
        Dim rim As Guid
        If String.IsNullOrEmpty(query) Then
            Return Guid.Empty
        End If

        Dim reg As New Regex(
        "i=([0-9a-z]{8}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{12})")
        Dim gid As String = reg.Match(query).Groups(1).Value
        If Guid.TryParse(gid, rim) Then
            Return rim
        Else
            Return Guid.Empty
        End If

    End Function
End Class
