'***************************** Module Header ******************************\
'* Module Name:    ChatRoom.vb
'* Project:        VBASPNETAJAXWebChat
'* Copyright (c) Microsoft Corporation
'*
'* The project illustrates how to design a simple AJAX web chat application. 
'* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
'* In this sample, we could create a chat room and invite someone
'* else to join in the room and start to chat.
'* 
'* In this file, we create a DataContract class which used to serialize the
'* ChatRoom data to the client side.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'\****************************************************************************

Imports System.Runtime.Serialization

<DataContract()> _
Public Class ChatRoom

    <DataMember()> _
    Public Property RoomID() As Guid
        Get
            Return m_RoomID
        End Get
        Private Set(ByVal value As Guid)
            m_RoomID = value
        End Set
    End Property
    Private m_RoomID As Guid


    <DataMember()> _
    Public Property RoomName() As String
        Get
            Return m_RoomName
        End Get
        Private Set(ByVal value As String)
            m_RoomName = value
        End Set
    End Property
    Private m_RoomName As String


    <DataMember()> _
    Public Property MaxUser() As Integer
        Get
            Return m_MaxUser
        End Get
        Private Set(ByVal value As Integer)
            m_MaxUser = value
        End Set
    End Property
    Private m_MaxUser As Integer


    <DataMember()> _
    Public Property CurrentUser() As Integer
        Get
            Return m_CurrentUser
        End Get
        Private Set(ByVal value As Integer)
            m_CurrentUser = value
        End Set
    End Property
    Private m_CurrentUser As Integer

    Public Sub New(ByVal id As Guid)
        Dim db As SessionDBDataContext = New SessionDBDataContext()
        Dim room = db.tblChatRooms.SingleOrDefault(Function(r) r.ChatRoomID = id)
        If room IsNot Nothing Then
            RoomID = id
            RoomName = room.ChatRoomName
            MaxUser = room.MaxUserNumber
            CurrentUser = (From t In room.tblTalkers
                         Where t.CheckOutTime Is Nothing
                         Select t).Count()
        End If
    End Sub

End Class
