'***************************** Module Header ******************************\
'* Module Name:    ChatManager.vb
'* Project:        VBASPNETAJAXWebChat
'* Copyright (c) Microsoft Corporation
'*
'* The project illustrates how to design a simple AJAX web chat application. 
'* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
'* In this sample, we could create a chat room and invite someone
'* else to join in the room and start to chat.
'* 
'* In this file, we use Linq to control the data in the database.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'\****************************************************************************

Public Class ChatManager

#Region "Send & Recieve Message"

    Public Shared Function SendMessage(ByVal talker As tblTalker,
                                       ByVal message As String) As Boolean
        Try

            Dim db As New SessionDBDataContext()
            Dim msgpool As New tblMessagePool()
            msgpool.message = message
            msgpool.SendTime = DateTime.Now
            msgpool.talkerID = talker.TalkerID
            db.tblMessagePools.InsertOnSubmit(msgpool)
            db.SubmitChanges()
            Return True
        Catch
            Return False
        End Try
    End Function

    Public Shared Function RecieveMessage(ByVal room As tblChatRoom) _
                                As List(Of tblMessagePool)

        Dim db As New SessionDBDataContext()
        If db.tblMessagePools.Count(
            Function(msg) room.tblTalkers.Contains(msg.tblTalker)) > 0 Then
            Return (From messages In db.tblMessagePools
                    Where messages.tblTalker.ChatRoomID = room.ChatRoomID) _
                    .ToList()
        Else
            Return Nothing
        End If
    End Function

    Private Shared Sub TryToDeleteChatMessageList(ByVal roomid As Guid)
        Dim db As New SessionDBDataContext()
        Dim chatroom = GetChatRoom(roomid)
        If (From t In chatroom.tblTalkers
                         Where t.CheckOutTime Is Nothing
                         Select t).Count() = 0 Then
            Dim list = From m In db.tblMessagePools
                       Where m.tblTalker.ChatRoomID = roomid
            db.tblMessagePools.DeleteAllOnSubmit(list)
            db.SubmitChanges()
        End If
    End Sub

#End Region

#Region "ChatRoom Management"

    Public Shared Function CreateChatRoom(ByVal roomName As String,
                                          ByVal password As String,
                                          ByVal isLock As Boolean,
                                          ByVal maxUserNumber As Integer,
                                          ByVal needPassword As Boolean) As Guid

        Dim db As New SessionDBDataContext()
        Dim room As New tblChatRoom()
        room.ChatRoomID = Guid.NewGuid()
        room.ChatRoomName = roomName
        room.ChatRoomPassword = password
        room.IsLock = isLock
        room.MaxUserNumber = maxUserNumber
        room.NeedPassword = needPassword
        db.tblChatRooms.InsertOnSubmit(room)
        db.SubmitChanges()
        Return room.ChatRoomID
    End Function

    Public Shared Function GetChatRoom(ByVal roomid As Guid) As tblChatRoom
        Dim db As New SessionDBDataContext()
        Return db.tblChatRooms.SingleOrDefault(Function(r) r.ChatRoomID = roomid)

    End Function

    Public Shared Function IsRoomFull(ByVal roomID As Guid) As Boolean
        Dim db As New SessionDBDataContext()
        Dim rsl = db.tblChatRooms.SingleOrDefault(
            Function(room) room.ChatRoomID = roomID)
        If rsl IsNot Nothing Then
            Return rsl.MaxUserNumber = (From t In rsl.tblTalkers
                         Where t.CheckOutTime Is Nothing
                         Select t).Count()
        Else
            Return False
        End If

    End Function

    Public Shared Function GetChatRoomList() As List(Of tblChatRoom)
        Dim db As New SessionDBDataContext()
        Return db.tblChatRooms.ToList()
    End Function

    Public Shared Function JoinChatRoom(ByVal ChatRoomID As Guid,
                                        ByVal context As HttpContext,
                                        ByVal [alias] As String) As Boolean
        If Not ChatManager.IsRoomFull(ChatRoomID) Then
            Dim db As New SessionDBDataContext()
            If db.tblSessions.Count(
                Function(s) s.SessionID = context.Session.SessionID) = 0 Then
                ChatManager.CreateSession(context, [alias])
            End If
            Dim session = ChatManager.GetSession(context)
            If db.tblTalkers.Count(
                Function(t) t.ChatRoomID = ChatRoomID AndAlso
                    t.SessionID = session.UID AndAlso
                    t.CheckOutTime Is Nothing) > 0 Then
                Return False
            Else
                Dim talker As New tblTalker()
                talker.ChatRoomID = ChatRoomID
                talker.CheckInTime = DateTime.Now
                talker.CheckOutTime = Nothing
                talker.SessionID = session.UID
                db.tblTalkers.InsertOnSubmit(talker)
                db.SubmitChanges()
                Return True
            End If
        Else
            Return False
        End If
    End Function

    Public Shared Function FindTalker(ByVal ChatRoomID As Guid,
                                      ByVal context As HttpContext) As tblTalker
        Dim db As New SessionDBDataContext()
        Dim rsl = db.tblTalkers.FirstOrDefault(
            Function(t) t.ChatRoomID = ChatRoomID AndAlso
                t.SessionID = ChatManager.GetSession(context).UID)
        Return rsl

    End Function

    Public Shared Function GetRoomTalkerList(ByVal ChatRoomID As Guid) _
                                As List(Of tblTalker)

        Dim db As New SessionDBDataContext()
        Dim rsl = From d In db.tblTalkers
                  Where d.CheckOutTime Is Nothing AndAlso
                  d.ChatRoomID = ChatRoomID
        Return rsl.ToList()

    End Function

    Public Shared Sub LeaveChatRoom(ByVal ChatRoomID As Guid,
                                    ByVal context As HttpContext)
        Dim db As New SessionDBDataContext()
        Dim session As tblSession = ChatManager.GetSession(context)
        If session IsNot Nothing Then
            Dim talker = db.tblTalkers.FirstOrDefault(
                Function(t) t.ChatRoomID = ChatRoomID AndAlso
                    t.SessionID = session.UID AndAlso
                    t.CheckOutTime Is Nothing)

            If talker IsNot Nothing Then
                talker.CheckOutTime = DateTime.Now
                db.SubmitChanges()
            End If
        End If
        TryToDeleteChatMessageList(ChatRoomID)
    End Sub
#End Region

#Region "Chat Session Management"

    Public Shared Function GetSession(ByVal context As HttpContext) As tblSession
        Dim db As New SessionDBDataContext()
        Dim session = db.tblSessions.FirstOrDefault(
            Function(s) s.SessionID = context.Session.SessionID)
        Return session
    End Function

    Public Shared Function SessionExist(ByVal context As HttpContext) As Boolean
        Return ChatManager.GetSession(context) IsNot Nothing
    End Function

    Public Shared Function CreateSession(ByVal context As HttpContext,
                                         ByVal userAlias As String) As Boolean
        Try
            Dim db As New SessionDBDataContext()

            Dim session As New tblSession()
            session.SessionID = context.Session.SessionID
            session.IP = context.Request.UserHostAddress
            If String.IsNullOrEmpty(userAlias) Then
                userAlias = session.IP
            End If
            session.UserAlias = userAlias
            db.tblSessions.InsertOnSubmit(session)
            db.SubmitChanges()
            Return True
        Catch
            Return False
        End Try
    End Function

#End Region

End Class
