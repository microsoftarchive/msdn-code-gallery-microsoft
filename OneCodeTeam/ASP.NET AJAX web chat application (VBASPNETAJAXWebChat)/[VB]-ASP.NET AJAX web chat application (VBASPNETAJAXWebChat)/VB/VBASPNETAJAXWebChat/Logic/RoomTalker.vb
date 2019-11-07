'***************************** Module Header ******************************\
'* Module Name:    RoomTalker.vb
'* Project:        VBASPNETAJAXWebChat
'* Copyright (c) Microsoft Corporation
'*
'* The project illustrates how to design a simple AJAX web chat application. 
'* We use jQuery, ASP.NET AJAX at client side and Linq to SQL at server side.
'* In this sample, we could create a chat room and invite someone
'* else to join in the room and start to chat.
'* 
'* In this file, we create a DataContract class which used to serialize the
'* talker data to the client side.
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'\****************************************************************************

Imports System.Runtime.Serialization

<DataContract()> _
Public Class RoomTalker
    <DataMember()> _
    Public Property TalkerAlias() As String
        Get
            Return m_TalkerAlias
        End Get
        Private Set(ByVal value As String)
            m_TalkerAlias = value
        End Set
    End Property
    Private m_TalkerAlias As String

    <DataMember()> _
    Public Property TalkerSession() As String
        Get
            Return m_TalkerSession
        End Get
        Private Set(ByVal value As String)
            m_TalkerSession = value
        End Set
    End Property
    Private m_TalkerSession As String

    <DataMember()> _
    Public Property TalkerIP() As String
        Get
            Return m_TalkerIP
        End Get
        Private Set(ByVal value As String)
            m_TalkerIP = value
        End Set
    End Property
    Private m_TalkerIP As String

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

    Public Sub New(ByVal talker As tblTalker, ByVal context As HttpContext)
        TalkerAlias = talker.tblSession.UserAlias
        TalkerIP = talker.tblSession.IP
        TalkerSession = talker.tblSession.SessionID
        IsFriend = (TalkerSession <> context.Session.SessionID)
    End Sub
End Class
