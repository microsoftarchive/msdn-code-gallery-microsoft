'***************************** Module Header ******************************\
'* Module Name:    Message.vb
'* Project:        VBASPNETAJAXWebChat
'* Copyright (c) Microsoft Corporation
'*
'* The project illustrates how to design a simple AJAX web chat application. 
'* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
'* In this sample, we could create a chat room and invite someone
'* else to join in the room and start to chat.
'* 
'* In this file, we create a DataContract class which used to serialize the
'* Message data to the client side.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'\****************************************************************************

Imports System.Runtime.Serialization

<DataContract()> _
Public Class Message
    <DataMember()> _
    Public Property Talker() As String
        Get
            Return m_Talker
        End Get
        Private Set(ByVal value As String)
            m_Talker = value
        End Set
    End Property
    Private m_Talker As String

    <DataMember()> _
    Public Property MessageData() As String
        Get
            Return m_MessageData
        End Get
        Private Set(ByVal value As String)
            m_MessageData = value
        End Set
    End Property
    Private m_MessageData As String

    <DataMember()> _
    Public Property SendTime() As DateTime
        Get
            Return m_SendTime
        End Get
        Private Set(ByVal value As DateTime)
            m_SendTime = value
        End Set
    End Property
    Private m_SendTime As DateTime

    <DataMember()> _
    Public Property IsFriend() As Boolean
        Get
            Return m_IsFriend
        End Get
        Private Set(ByVal value As Boolean)
            m_IsFriend = value
        End Set
    End Property
    Private m_IsFriend As Boolean

    Public Sub New(ByVal message__1 As tblMessagePool, ByVal session As HttpContext)
        Talker = message__1.tblTalker.tblSession.UserAlias
        MessageData = message__1.message
        SendTime = message__1.SendTime
        IsFriend = (message__1.tblTalker.tblSession.SessionID _
                    <> session.Session.SessionID)
    End Sub
End Class

