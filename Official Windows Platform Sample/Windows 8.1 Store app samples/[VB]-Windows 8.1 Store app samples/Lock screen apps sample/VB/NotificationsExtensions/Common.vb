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
        Set(ByVal value As String)
            m_Text = value
        End Set
    End Property

    Public Property Lang() As String Implements INotificationContentText.Lang
        Get
            Return m_Lang
        End Get
        Set(ByVal value As String)
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
        Set(ByVal value As String)
            m_Src = value
        End Set
    End Property

    Public Property Alt() As String Implements INotificationContentImage.Alt
        Get
            Return m_Alt
        End Get
        Set(ByVal value As String)
            m_Alt = value
        End Set
    End Property

    Public Property AddImageQuery() As Boolean Implements INotificationContentImage.AddImageQuery
        Get
            If m_AddImageQueryNullable Is Nothing OrElse m_AddImageQueryNullable.Value = False Then
                Return False
            Else
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            m_AddImageQueryNullable = value
        End Set
    End Property

    Public Property AddImageQueryNullable() As Boolean?
        Get
            Return m_AddImageQueryNullable
        End Get
        Set(ByVal value As Boolean?)
            m_AddImageQueryNullable = value
        End Set
    End Property

    Private m_Src As String
    Private m_Alt As String
    Private m_AddImageQueryNullable? As Boolean
End Class

Friend NotInheritable Class Util

    Private Sub New()
    End Sub
    Public Const NOTIFICATION_CONTENT_VERSION As Integer = 1

    Public Shared Function HttpEncode(ByVal value As String) As String
        Return value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;").Replace("'", "&apos;")
    End Function
End Class

''' <summary>
''' Base class for the notification content creation helper classes.
''' </summary>
#If Not WINRT_NOT_PRESENT Then
Friend MustInherit Class NotificationBase
#Else
    Friend MustInherit Partial Class NotificationBase
#End If
    Protected Sub New(ByVal templateName As String, ByVal fallbackName As String, ByVal imageCount As Integer, ByVal textCount As Integer)
        m_TemplateName = templateName
        m_FallbackName = fallbackName

        m_Images = New NotificationContentImage(imageCount - 1) {}
        For i As Integer = 0 To m_Images.Length - 1
            m_Images(i) = New NotificationContentImage()
        Next i

        m_TextFields = New INotificationContentText(textCount - 1) {}
        For i As Integer = 0 To m_TextFields.Length - 1
            m_TextFields(i) = New NotificationContentText()
        Next i
    End Sub

    Public Property StrictValidation() As Boolean
        Get
            Return m_StrictValidation
        End Get
        Set(ByVal value As Boolean)
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
        Set(ByVal value As String)
            If Me.StrictValidation AndAlso (Not String.IsNullOrEmpty(value)) Then
                Dim uri As Uri
                Try
                    uri = New Uri(value)
                Catch e As Exception
                    Throw New ArgumentException("Invalid URI. Use a valid URI or turn off StrictValidation", e)
                End Try
                If Not (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) OrElse uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) OrElse uri.Scheme.Equals("ms-appx", StringComparison.OrdinalIgnoreCase) OrElse (uri.Scheme.Equals("ms-appdata", StringComparison.OrdinalIgnoreCase) AndAlso (String.IsNullOrEmpty(uri.Authority)) AndAlso (uri.AbsolutePath.StartsWith("/local/") OrElse uri.AbsolutePath.StartsWith("local/")))) Then ' both ms-appdata:local/ and ms-appdata:/local/ are valid -  check to make sure the Uri isn't ms-appdata://foo/local
                    Throw New ArgumentException("The BaseUri must begin with http://, https://, ms-appx:///, or ms-appdata:///local/.", "value")
                End If
            End If
            m_BaseUri = value
        End Set
    End Property

    Public Property Lang() As String
        Get
            Return m_Lang
        End Get
        Set(ByVal value As String)
            m_Lang = value
        End Set
    End Property

    Public Property AddImageQuery() As Boolean
        Get
            If m_AddImageQueryNullable Is Nothing OrElse m_AddImageQueryNullable.Value = False Then
                Return False
            Else
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            m_AddImageQueryNullable = value
        End Set
    End Property

    Public Property AddImageQueryNullable() As Boolean?
        Get
            Return m_AddImageQueryNullable
        End Get
        Set(ByVal value As Boolean?)
            m_AddImageQueryNullable = value
        End Set
    End Property

    Protected Function SerializeProperties(ByVal globalLang As String, ByVal globalBaseUri As String, ByVal globalAddImageQuery As Boolean) As String
        globalLang = If(globalLang IsNot Nothing, globalLang, String.Empty)
        globalBaseUri = If(String.IsNullOrWhiteSpace(globalBaseUri), Nothing, globalBaseUri)

        Dim builder As New StringBuilder(String.Empty)
        For i As Integer = 0 To m_Images.Length - 1
            If Not String.IsNullOrEmpty(m_Images(i).Src) Then
                Dim escapedSrc As String = Util.HttpEncode(m_Images(i).Src)
                If Not String.IsNullOrWhiteSpace(m_Images(i).Alt) Then
                    Dim escapedAlt As String = Util.HttpEncode(m_Images(i).Alt)
                    If m_Images(i).AddImageQueryNullable Is Nothing OrElse m_Images(i).AddImageQueryNullable = globalAddImageQuery Then
                        builder.AppendFormat("<image id='{0}' src='{1}' alt='{2}'/>", i + 1, escapedSrc, escapedAlt)
                    Else
                        builder.AppendFormat("<image addImageQuery='{0}' id='{1}' src='{2}' alt='{3}'/>", m_Images(i).AddImageQuery.ToString().ToLowerInvariant(), i + 1, escapedSrc, escapedAlt)
                    End If
                Else
                    If m_Images(i).AddImageQueryNullable Is Nothing OrElse m_Images(i).AddImageQueryNullable = globalAddImageQuery Then
                        builder.AppendFormat("<image id='{0}' src='{1}'/>", i + 1, escapedSrc)
                    Else
                        builder.AppendFormat("<image addImageQuery='{0}' id='{1}' src='{2}'/>", m_Images(i).AddImageQuery.ToString().ToLowerInvariant(), i + 1, escapedSrc)
                    End If
                End If
            End If
        Next i

        For i As Integer = 0 To m_TextFields.Length - 1
            If Not String.IsNullOrWhiteSpace(m_TextFields(i).Text) Then
                Dim escapedValue As String = Util.HttpEncode(m_TextFields(i).Text)
                If (Not String.IsNullOrWhiteSpace(m_TextFields(i).Lang)) AndAlso (Not m_TextFields(i).Lang.Equals(globalLang)) Then
                    Dim escapedLang As String = Util.HttpEncode(m_TextFields(i).Lang)
                    builder.AppendFormat("<text id='{0}' lang='{1}'>{2}</text>", i + 1, escapedLang, escapedValue)
                Else
                    builder.AppendFormat("<text id='{0}'>{1}</text>", i + 1, escapedValue)
                End If
            End If
        Next i

        Return builder.ToString()
    End Function

    Public ReadOnly Property TemplateName() As String
        Get
            Return m_TemplateName
        End Get
    End Property
    Public ReadOnly Property FallbackName() As String
        Get
            Return m_FallbackName
        End Get
    End Property

    Private m_StrictValidation As Boolean = True
    Private m_Images() As NotificationContentImage
    Private m_TextFields() As INotificationContentText

    Private m_Lang As String
    Private m_BaseUri As String
    Private m_TemplateName As String
    Private m_FallbackName As String
    Private m_AddImageQueryNullable? As Boolean
End Class

''' <summary>
''' Exception returned when invalid notification content is provided.
''' </summary>
Friend NotInheritable Class NotificationContentValidationException
    Inherits COMException

    Public Sub New(ByVal message As String)
        MyBase.New(message, CInt(&H80070057))
    End Sub
End Class
