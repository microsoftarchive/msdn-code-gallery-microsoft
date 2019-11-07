' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Collections.ObjectModel
Imports System.Runtime.InteropServices
Imports System.Text
#If Not WINRT_NOT_PRESENT Then
Imports Windows.Data.Xml.Dom
#End If

Friend NotInheritable Class NotificationContentText
    Implements INotificationContentText

    Friend Sub New()
    End Sub

    Public Property Text() As String Implements INotificationContentText.Text
        Get
            Return m_Text
        End Get
        Set(value As String)
            m_Text = value
        End Set
    End Property

    Public Property Lang() As String Implements INotificationContentText.Lang
        Get
            Return m_Lang
        End Get
        Set(value As String)
            m_Lang = value
        End Set
    End Property

    Private m_Text As String
    Private m_Lang As String
End Class

Friend NotInheritable Class NotificationContentImage
    Implements INotificationContentImage

    Friend Sub New()
    End Sub

    Public Property Src() As String Implements INotificationContentImage.Src
        Get
            Return m_Src
        End Get
        Set(value As String)
            m_Src = value
        End Set
    End Property

    Public Property Alt() As String Implements INotificationContentImage.Alt
        Get
            Return m_Alt
        End Get
        Set(value As String)
            m_Alt = value
        End Set
    End Property

    Private m_Src As String
    Private m_Alt As String
End Class

Friend NotInheritable Class Util
    Private Sub New()
    End Sub
    Public Const NOTIFICATION_CONTENT_VERSION As Integer = 1

    Public Shared Function HttpEncode(value As String) As String
        Return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;").Replace("'", "&apos;")
    End Function
End Class

''' <summary>
''' Base class for the notification content creation helper classes.
''' </summary>
#If Not WINRT_NOT_PRESENT Then
Friend MustInherit Class NotificationBase
#Else
    Friend partial class NotificationBase
#End If
    Protected Sub New(templateName As String, imageCount As Integer, textCount As Integer)
        m_TemplateName = templateName

        m_Images = New INotificationContentImage(imageCount - 1) {}
        For i As Integer = 0 To m_Images.Length - 1
            m_Images(i) = New NotificationContentImage()
        Next

        m_TextFields = New INotificationContentText(textCount - 1) {}
        For i As Integer = 0 To m_TextFields.Length - 1
            m_TextFields(i) = New NotificationContentText()
        Next
    End Sub

    Public Property StrictValidation() As Boolean
        Get
            Return m_StrictValidation
        End Get
        Set(value As Boolean)
            m_StrictValidation = value
        End Set
    End Property
    Public MustOverride Function GetContent() As String

    Public Overrides Function ToString() As String
        Return GetContent()
    End Function

#If Not WINRT_NOT_PRESENT Then
    Public Overridable Function GetXml() As XmlDocument
        Dim xml As New XmlDocument()
        xml.LoadXml(GetContent())
        Return xml
    End Function
#End If

    ''' <summary>
    ''' Retrieves the list of images that can be manipulated on the notification content.
    ''' </summary>
    Public ReadOnly Property Images() As INotificationContentImage()
        Get
            Return m_Images
        End Get
    End Property

    ''' <summary>
    ''' Retrieves the list of text fields that can be manipulated on the notification content.
    ''' </summary>
    Public ReadOnly Property TextFields() As INotificationContentText()
        Get
            Return m_TextFields
        End Get
    End Property

    ''' <summary>
    ''' The base Uri path that should be used for all image references in the notification.
    ''' </summary>
    Public Property BaseUri() As String
        Get
            Return m_BaseUri
        End Get
        Set(value As String)
            Dim goodPrefix As Boolean = Me.StrictValidation OrElse value Is Nothing
            goodPrefix = goodPrefix OrElse value.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            goodPrefix = goodPrefix OrElse value.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
            goodPrefix = goodPrefix OrElse value.StartsWith("ms-appx:///", StringComparison.OrdinalIgnoreCase)
            goodPrefix = goodPrefix OrElse value.StartsWith("ms-appdata:///local/", StringComparison.OrdinalIgnoreCase)
            If Not goodPrefix Then
                Throw New ArgumentException("The BaseUri must begin with http://, https://, ms-appx:///, or ms-appdata:///local/.", "value")
            End If
            m_BaseUri = value
        End Set
    End Property

    Public Property Lang() As String
        Get
            Return m_Lang
        End Get
        Set(value As String)
            m_Lang = value
        End Set
    End Property

    Protected Function SerializeProperties(globalLang As String, globalBaseUri As String) As String
        globalLang = If((globalLang IsNot Nothing), globalLang, String.Empty)
        globalBaseUri = If(String.IsNullOrWhiteSpace(globalBaseUri), Nothing, globalBaseUri)

        Dim builder As New StringBuilder(String.Empty)
        For i As Integer = 0 To m_Images.Length - 1
            If Not String.IsNullOrEmpty(m_Images(i).Src) Then
                Dim escapedSrc As String = Util.HttpEncode(m_Images(i).Src)
                If Not String.IsNullOrWhiteSpace(m_Images(i).Alt) Then
                    Dim escapedAlt As String = Util.HttpEncode(m_Images(i).Alt)
                    builder.AppendFormat("<image id='{0}' src='{1}' alt='{2}'/>", i + 1, escapedSrc, escapedAlt)
                Else
                    builder.AppendFormat("<image id='{0}' src='{1}'/>", i + 1, escapedSrc)
                End If
            End If
        Next

        For i As Integer = 0 To m_TextFields.Length - 1
            If Not String.IsNullOrWhiteSpace(m_TextFields(i).Text) Then
                Dim escapedValue As String = Util.HttpEncode(m_TextFields(i).Text)
                If Not String.IsNullOrWhiteSpace(m_TextFields(i).Lang) AndAlso Not m_TextFields(i).Lang.Equals(globalLang) Then
                    Dim escapedLang As String = Util.HttpEncode(m_TextFields(i).Lang)
                    builder.AppendFormat("<text id='{0}' lang='{1}'>{2}</text>", i + 1, escapedLang, escapedValue)
                Else
                    builder.AppendFormat("<text id='{0}'>{1}</text>", i + 1, escapedValue)
                End If
            End If
        Next

        Return builder.ToString
    End Function

    Public ReadOnly Property TemplateName() As String
        Get
            Return m_TemplateName
        End Get
    End Property

    Private m_StrictValidation As Boolean = True
    Private m_Images As INotificationContentImage()
    Private m_TextFields As INotificationContentText()
    ' Remove when "lang" is not required on the visual element
    Private m_Lang As String = "en-US"
    Private m_BaseUri As String
    Private m_TemplateName As String
End Class

''' <summary>
''' Exception returned when invalid notification content is provided.
''' </summary>
Friend NotInheritable Class NotificationContentValidationException
    Inherits COMException
    Public Sub New(message As String)
        MyBase.New(message, CInt(&H80070057))
    End Sub
End Class
