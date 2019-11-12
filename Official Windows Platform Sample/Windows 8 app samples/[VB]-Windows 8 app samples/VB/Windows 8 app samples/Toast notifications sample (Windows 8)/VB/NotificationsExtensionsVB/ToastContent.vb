' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Text
#If Not WINRT_NOT_PRESENT Then
Imports Windows.Data.Xml.Dom
Imports Windows.UI.Notifications
#End If
Imports NotificationsExtensionsVB.ToastContent

Namespace NotificationsExtensions.ToastContent
    Friend NotInheritable Class ToastAudio
        Implements IToastAudio

        Friend Sub New()
        End Sub

        Public Property Content As ToastAudioContent Implements IToastAudio.Content
            Get
                Return m_Content
            End Get
            Set(value As ToastAudioContent)
                If Not [Enum].IsDefined(GetType(ToastAudioContent), value) Then
                    Throw New ArgumentOutOfRangeException("value")
                End If
                m_Content = value
            End Set
        End Property

        Public Property [Loop] As Boolean Implements IToastAudio.Loop
            Get
                Return m_Loop
            End Get
            Set(value As Boolean)
                m_Loop = value
            End Set
        End Property

        Private m_Content As ToastAudioContent = ToastAudioContent.Default
        Private m_Loop As Boolean = False
    End Class

    Friend Class ToastNotificationBase
        Inherits NotificationBase
        Implements IToastNotificationContent

        Public Sub New(templateName As String, imageCount As Integer, textCount As Integer)
            MyBase.New(templateName, imageCount, textCount)
        End Sub

        Private Function AudioSrcIsLooping() As Boolean
            Return (Audio.Content = ToastAudioContent.LoopingAlarm) OrElse (Audio.Content = ToastAudioContent.LoopingAlarm2) OrElse (Audio.Content = ToastAudioContent.LoopingCall) OrElse (Audio.Content = ToastAudioContent.LoopingCall2)
        End Function

        Private Sub ValidateAudio()
            If StrictValidation Then
                If Audio.[Loop] AndAlso Duration <> ToastDuration.Long Then
                    Throw New NotificationContentValidationException("Looping audio is only available for long duration toasts.")
                End If
                If Audio.[Loop] AndAlso Not AudioSrcIsLooping() Then
                    Throw New NotificationContentValidationException("A looping audio src must be chosen if the looping audio property is set.")
                End If
                If Not Audio.[Loop] AndAlso AudioSrcIsLooping() Then
                    Throw New NotificationContentValidationException("The looping audio property needs to be set if a looping audio src is chosen.")
                End If
            End If
        End Sub

        Private Sub AppendAudioTag(builder As StringBuilder)
            If Audio.Content <> ToastAudioContent.Default Then
                builder.Append("<audio")
                If Audio.Content = ToastAudioContent.Silent Then
                    builder.Append(" silent='true'/>")
                Else
                    If Audio.[Loop] = True Then
                        builder.Append(" loop='true'")
                    End If

                    ' The default looping sound is LoopingCall - save size by not adding it
                    If Audio.Content <> ToastAudioContent.LoopingCall Then
                        Dim audioSrc As String = Nothing
                        Select Case Audio.Content
                            Case ToastAudioContent.IM
                                audioSrc = "ms-winsoundevent:Notification.IM"
                                Exit Select
                            Case ToastAudioContent.Mail
                                audioSrc = "ms-winsoundevent:Notification.Mail"
                                Exit Select
                            Case ToastAudioContent.Reminder
                                audioSrc = "ms-winsoundevent:Notification.Reminder"
                                Exit Select
                            Case ToastAudioContent.SMS
                                audioSrc = "ms-winsoundevent:Notification.SMS"
                                Exit Select
                            Case ToastAudioContent.LoopingAlarm
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm"
                                Exit Select
                            Case ToastAudioContent.LoopingAlarm2
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm2"
                                Exit Select
                            Case ToastAudioContent.LoopingCall
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call"
                                Exit Select
                            Case ToastAudioContent.LoopingCall2
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call2"
                                Exit Select
                        End Select
                        builder.AppendFormat(" src='{0}'", audioSrc)
                    End If
                End If
                builder.Append("/>")
            End If
        End Sub

        Public Overrides Function GetContent() As String Implements IToastNotificationContent.GetContent
            ValidateAudio()

            Dim builder As New StringBuilder("<toast")
            If Not String.IsNullOrEmpty(Launch) Then
                builder.AppendFormat(" launch='{0}'", Util.HttpEncode(Launch))
            End If
            If Duration <> ToastDuration.Short Then
                builder.AppendFormat(" duration='{0}'", Duration.ToString.ToLowerInvariant)
            End If
            builder.Append(">")

            builder.AppendFormat("<visual version='{0}'", Util.NOTIFICATION_CONTENT_VERSION)
            If Not String.IsNullOrWhiteSpace(Lang) Then
                builder.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) Then
                builder.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
            End If
            builder.Append(">")

            builder.AppendFormat("<binding template='{0}'>{1}</binding>", TemplateName, SerializeProperties(Lang, BaseUri))
            builder.Append("</visual>")

            AppendAudioTag(builder)

            builder.Append("</toast>")

            Return builder.ToString
        End Function

#If Not WINRT_NOT_PRESENT Then
        Public Function CreateNotification() As ToastNotification Implements IToastNotificationContent.CreateNotification
            Dim xmlDoc As New XmlDocument
            xmlDoc.LoadXml(GetContent)
            Return New ToastNotification(xmlDoc)
        End Function
#End If

        Public ReadOnly Property Audio As IToastAudio Implements IToastNotificationContent.Audio
            Get
                Return m_Audio
            End Get
        End Property

        Public Property Launch As String
            Get
                Return m_Launch
            End Get
            Set(value As String)
                m_Launch = value
            End Set
        End Property

        Public Property Duration As ToastDuration Implements IToastNotificationContent.Duration
            Get
                Return m_Duration
            End Get
            Set(value As ToastDuration)
                If Not [Enum].IsDefined(GetType(ToastDuration), value) Then
                    Throw New ArgumentOutOfRangeException("value")
                End If
                m_Duration = value
            End Set
        End Property

        Public Overrides Function GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml
        End Function

        Private m_Launch As String
        Private m_Audio As IToastAudio = New ToastAudio
        Private m_Duration As ToastDuration = ToastDuration.Short

        Public Property BaseUri1 As String Implements IToastNotificationContent.BaseUri
        Public Property Lang1 As String Implements IToastNotificationContent.Lang
        Public Property Launch1 As String Implements IToastNotificationContent.Launch
        Public Property StrictValidation1 As Boolean Implements IToastNotificationContent.StrictValidation
    End Class

    Friend Class ToastImageAndText01
        Inherits ToastNotificationBase
        Implements IToastImageAndText01

        Public Sub New()
            MyBase.New(templateName:="ToastImageAndText01", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image As INotificationContentImage Implements IToastImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap As INotificationContentText Implements IToastImageAndText01.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property
    End Class

    Friend Class ToastImageAndText02
        Inherits ToastNotificationBase
        Implements IToastImageAndText02

        Public Sub New()
            MyBase.New(templateName:="ToastImageAndText02", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image As INotificationContentImage Implements IToastImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading As INotificationContentText Implements IToastImageAndText02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap As INotificationContentText Implements IToastImageAndText02.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property
    End Class

    Friend Class ToastImageAndText03
        Inherits ToastNotificationBase
        Implements IToastImageAndText03

        Public Sub New()
            MyBase.New(templateName:="ToastImageAndText03", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image As INotificationContentImage Implements IToastImageAndText03.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap As INotificationContentText Implements IToastImageAndText03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public ReadOnly Property TextBody As INotificationContentText Implements IToastImageAndText03.TextBody
            Get
                Return TextFields(1)
            End Get
        End Property
    End Class

    Friend Class ToastImageAndText04
        Inherits ToastNotificationBase
        Implements IToastImageAndText04

        Public Sub New()
            MyBase.New(templateName:="ToastImageAndText04", imageCount:=1, textCount:=3)
        End Sub

        Public ReadOnly Property Image As INotificationContentImage Implements IToastImageAndText04.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading As INotificationContentText Implements IToastImageAndText04.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property

        Public ReadOnly Property TextBody1 As INotificationContentText Implements IToastImageAndText04.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property

        Public ReadOnly Property TextBody2 As INotificationContentText Implements IToastImageAndText04.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
    End Class

    Friend Class ToastText01
        Inherits ToastNotificationBase
        Implements IToastText01

        Public Sub New()
            MyBase.New(templateName:="ToastText01", imageCount:=0, textCount:=1)
        End Sub

        Public ReadOnly Property TextBodyWrap As INotificationContentText Implements IToastText01.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property
    End Class

    Friend Class ToastText02
        Inherits ToastNotificationBase
        Implements IToastText02

        Public Sub New()
            MyBase.New(templateName:="ToastText02", imageCount:=0, textCount:=2)
        End Sub

        Public ReadOnly Property TextHeading As INotificationContentText Implements IToastText02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap As INotificationContentText Implements IToastText02.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property
    End Class

    Friend Class ToastText03
        Inherits ToastNotificationBase
        Implements IToastText03

        Public Sub New()
            MyBase.New(templateName:="ToastText03", imageCount:=0, textCount:=2)
        End Sub

        Public ReadOnly Property TextHeadingWrap As INotificationContentText Implements IToastText03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property

        Public ReadOnly Property TextBody As INotificationContentText Implements IToastText03.TextBody
            Get
                Return TextFields(1)
            End Get
        End Property
    End Class

    Friend Class ToastText04
        Inherits ToastNotificationBase
        Implements IToastText04

        Public Sub New()
            MyBase.New(templateName:="ToastText04", imageCount:=0, textCount:=3)
        End Sub

        Public ReadOnly Property TextHeading As INotificationContentText Implements IToastText04.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property

        Public ReadOnly Property TextBody1 As INotificationContentText Implements IToastText04.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property

        Public ReadOnly Property TextBody2 As INotificationContentText Implements IToastText04.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
    End Class

    ''' <summary>
    ''' A factory which creates toast content objects for all of the toast template types.
    ''' </summary>
    Public NotInheritable Class ToastContentFactory
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Creates a ToastImageAndText01 template content object.
        ''' </summary>
        ''' <returns>A ToastImageAndText01 template content object.</returns>
        Public Shared Function CreateToastImageAndText01() As IToastImageAndText01
            Return New ToastImageAndText01
        End Function

        ''' <summary>
        ''' Creates a ToastImageAndText02 template content object.
        ''' </summary>
        ''' <returns>A ToastImageAndText02 template content object.</returns>
        Public Shared Function CreateToastImageAndText02() As IToastImageAndText02
            Return New ToastImageAndText02
        End Function

        ''' <summary>
        ''' Creates a ToastImageAndText03 template content object.
        ''' </summary>
        ''' <returns>A ToastImageAndText03 template content object.</returns>
        Public Shared Function CreateToastImageAndText03() As IToastImageAndText03
            Return New ToastImageAndText03
        End Function

        ''' <summary>
        ''' Creates a ToastImageAndText04 template content object.
        ''' </summary>
        ''' <returns>A ToastImageAndText04 template content object.</returns>
        Public Shared Function CreateToastImageAndText04() As IToastImageAndText04
            Return New ToastImageAndText04
        End Function

        ''' <summary>
        ''' Creates a ToastText01 template content object.
        ''' </summary>
        ''' <returns>A ToastText01 template content object.</returns>
        Public Shared Function CreateToastText01() As IToastText01
            Return New ToastText01
        End Function

        ''' <summary>
        ''' Creates a ToastText02 template content object.
        ''' </summary>
        ''' <returns>A ToastText02 template content object.</returns>
        Public Shared Function CreateToastText02() As IToastText02
            Return New ToastText02
        End Function

        ''' <summary>
        ''' Creates a ToastText03 template content object.
        ''' </summary>
        ''' <returns>A ToastText03 template content object.</returns>
        Public Shared Function CreateToastText03() As IToastText03
            Return New ToastText03
        End Function

        ''' <summary>
        ''' Creates a ToastText04 template content object.
        ''' </summary>
        ''' <returns>A ToastText04 template content object.</returns>
        Public Shared Function CreateToastText04() As IToastText04
            Return New ToastText04
        End Function
    End Class
End Namespace
