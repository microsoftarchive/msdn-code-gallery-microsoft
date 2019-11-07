' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System

#If Not WINRT_NOT_PRESENT Then
Imports Windows.Data.Xml.Dom
Imports Windows.UI.Notifications
#End If

Namespace BadgeContent
    ''' <summary>
    ''' Notification content object to display a glyph on a tile's badge.
    ''' </summary>
    Public NotInheritable Class BadgeGlyphNotificationContent
        Implements IBadgeNotificationContent

        ''' <summary>
        ''' Default constructor to create a glyph badge content object.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Constructor to create a glyph badge content object with a glyph.
        ''' </summary>
        ''' <param name="glyph">The glyph to be displayed on the badge.</param>
        Public Sub New(ByVal glyph As GlyphValue)
            m_Glyph = glyph
        End Sub

        ''' <summary>
        ''' The glyph to be displayed on the badge.
        ''' </summary>
        Public Property Glyph() As GlyphValue
            Get
                Return m_Glyph
            End Get
            Set(ByVal value As GlyphValue)
                If Not System.Enum.IsDefined(GetType(GlyphValue), value) Then
                    Throw New ArgumentOutOfRangeException("value")
                End If
                m_Glyph = value
            End Set
        End Property

        ''' <summary>
        ''' Retrieves the notification Xml content as a string.
        ''' </summary>
        ''' <returns>The notification Xml content as a string.</returns>
        Public Function GetContent() As String Implements NotificationsExtensions.INotificationContent.GetContent
            If Not System.Enum.IsDefined(GetType(GlyphValue), m_Glyph) Then
                Throw New NotificationContentValidationException("The badge glyph property was left unset.")
            End If

            Dim glyphString As String = m_Glyph.ToString()
            ' lower case the first character of the enum value to match the Xml schema
            glyphString = String.Format("{0}{1}", Char.ToLowerInvariant(glyphString.Chars(0)), glyphString.Substring(1))
            Return String.Format("<badge version='{0}' value='{1}'/>", Util.NOTIFICATION_CONTENT_VERSION, glyphString)
        End Function

        ''' <summary>
        ''' Retrieves the notification Xml content as a string.
        ''' </summary>
        ''' <returns>The notification Xml content as a string.</returns>
        Public Overrides Function ToString() As String
            Return GetContent()
        End Function

#If Not WINRT_NOT_PRESENT Then
        ''' <summary>
        ''' Retrieves the notification Xml content as a WinRT Xml document.
        ''' </summary>
        ''' <returns>The notification Xml content as a WinRT Xml document.</returns>
        Public Function GetXml() As XmlDocument Implements NotificationsExtensions.INotificationContent.GetXml
            Dim xml As New XmlDocument()
            xml.LoadXml(GetContent())
            Return xml
        End Function

        ''' <summary>
        ''' Creates a WinRT BadgeNotification object based on the content.
        ''' </summary>
        ''' <returns>A WinRT BadgeNotification object based on the content.</returns>
        Public Function CreateNotification() As BadgeNotification Implements IBadgeNotificationContent.CreateNotification
            Dim xmlDoc As New XmlDocument()
            xmlDoc.LoadXml(GetContent())
            Return New BadgeNotification(xmlDoc)
        End Function
#End If

        Private m_Glyph As GlyphValue = CType(-1, GlyphValue)
    End Class

    ''' <summary>
    ''' Notification content object to display a number on a tile's badge.
    ''' </summary>
    Public NotInheritable Class BadgeNumericNotificationContent
        Implements IBadgeNotificationContent

        ''' <summary>
        ''' Default constructor to create a numeric badge content object.
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Constructor to create a numeric badge content object with a number.
        ''' </summary>
        ''' <param name="number">
        ''' The number that will appear on the badge.  If the number is 0, the badge
        ''' will be removed.  The largest value that will appear on the badge is 99.
        ''' Numbers greater than 99 are allowed, but will be displayed as "99+".
        ''' </param>
        Public Sub New(ByVal number As UInteger)
            m_Number = number
        End Sub

        ''' <summary>
        ''' The number that will appear on the badge.  If the number is 0, the badge
        ''' will be removed.  The largest value that will appear on the badge is 99.
        ''' Numbers greater than 99 are allowed, but will be displayed as "99+".
        ''' </summary>
        Public Property Number() As UInteger
            Get
                Return m_Number
            End Get
            Set(ByVal value As UInteger)
                m_Number = value
            End Set
        End Property

        ''' <summary>
        ''' Retrieves the notification Xml content as a string.
        ''' </summary>
        ''' <returns>The notification Xml content as a string.</returns>
        Public Function GetContent() As String Implements NotificationsExtensions.INotificationContent.GetContent
            Return String.Format("<badge version='{0}' value='{1}'/>", Util.NOTIFICATION_CONTENT_VERSION, m_Number)
        End Function

        ''' <summary>
        ''' Retrieves the notification Xml content as a string.
        ''' </summary>
        ''' <returns>The notification Xml content as a string.</returns>
        Public Overrides Function ToString() As String
            Return GetContent()
        End Function

#If Not WINRT_NOT_PRESENT Then
        ''' <summary>
        ''' Retrieves the notification Xml content as a WinRT Xml document.
        ''' </summary>
        ''' <returns>The notification Xml content as a WinRT Xml document.</returns>
        Public Function GetXml() As XmlDocument Implements NotificationsExtensions.INotificationContent.GetXml
            Dim xml As New XmlDocument()
            xml.LoadXml(GetContent())
            Return xml
        End Function

        ''' <summary>
        ''' Creates a WinRT BadgeNotification object based on the content.
        ''' </summary>
        ''' <returns>A WinRT BadgeNotification object based on the content.</returns>
        Public Function CreateNotification() As BadgeNotification Implements IBadgeNotificationContent.CreateNotification
            Dim xmlDoc As New XmlDocument()
            xmlDoc.LoadXml(GetContent())
            Return New BadgeNotification(xmlDoc)
        End Function
#End If

        Private m_Number As UInteger = 0
    End Class
End Namespace
