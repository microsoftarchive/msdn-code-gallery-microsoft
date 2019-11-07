' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Text

#If Not WINRT_NOT_PRESENT Then
Imports Windows.Data.Xml.Dom
Imports Windows.UI.Notifications
#End If

Namespace ToastContent
    Friend NotInheritable Class ToastAudio
        Implements IToastAudio

        Friend Sub New()
        End Sub

        Public Property Content() As ToastAudioContent Implements IToastAudio.Content
            Get
                Return m_Content
            End Get
            Set(ByVal value As ToastAudioContent)
                If Not System.Enum.IsDefined(GetType(ToastAudioContent), value) Then
                    Throw New ArgumentOutOfRangeException("value")
                End If
                m_Content = value
            End Set
        End Property

        Public Property [Loop]() As Boolean Implements IToastAudio.Loop
            Get
                Return m_Loop
            End Get
            Set(ByVal value As Boolean)
                m_Loop = value
            End Set
        End Property

        Private m_Content As ToastAudioContent = ToastAudioContent.Default
        Private m_Loop As Boolean = False
    End Class

    Friend NotInheritable Class IncomingCallCommands
        Implements IIncomingCallCommands

        Friend Sub New()
        End Sub

        Public Property ShowVideoCommand() As Boolean Implements IIncomingCallCommands.ShowVideoCommand
            Get
                Return m_Video
            End Get
            Set(ByVal value As Boolean)
                m_Video = value
            End Set
        End Property

        Public Property VideoArgument() As String Implements IIncomingCallCommands.VideoArgument
            Get
                Return m_VideoArgument
            End Get
            Set(ByVal value As String)
                m_VideoArgument = value
            End Set
        End Property

        Public Property ShowVoiceCommand() As Boolean Implements IIncomingCallCommands.ShowVoiceCommand
            Get
                Return m_Voice
            End Get
            Set(ByVal value As Boolean)
                m_Voice = value
            End Set
        End Property

        Public Property VoiceArgument() As String Implements IIncomingCallCommands.VoiceArgument
            Get
                Return m_VoiceArgument
            End Get
            Set(ByVal value As String)
                m_VoiceArgument = value
            End Set
        End Property

        Public Property ShowDeclineCommand() As Boolean Implements IIncomingCallCommands.ShowDeclineCommand
            Get
                Return m_Decline
            End Get
            Set(ByVal value As Boolean)
                m_Decline = value
            End Set
        End Property

        Public Property DeclineArgument() As String Implements IIncomingCallCommands.DeclineArgument
            Get
                Return m_DeclineArgument
            End Get
            Set(ByVal value As String)
                m_DeclineArgument = value
            End Set
        End Property

        Private m_Video As Boolean = False
        Private m_Voice As Boolean = False
        Private m_Decline As Boolean = False

        Private m_VideoArgument As String = String.Empty
        Private m_VoiceArgument As String = String.Empty
        Private m_DeclineArgument As String = String.Empty
    End Class

    Friend NotInheritable Class AlarmCommands
        Implements IAlarmCommands

        Friend Sub New()
        End Sub

        Public Property ShowSnoozeCommand() As Boolean Implements IAlarmCommands.ShowSnoozeCommand
            Get
                Return m_Snooze
            End Get
            Set(ByVal value As Boolean)
                m_Snooze = value
            End Set
        End Property

        Public Property SnoozeArgument() As String Implements IAlarmCommands.SnoozeArgument
            Get
                Return m_SnoozeArgument
            End Get
            Set(ByVal value As String)
                m_SnoozeArgument = value
            End Set
        End Property

        Public Property ShowDismissCommand() As Boolean Implements IAlarmCommands.ShowDismissCommand
            Get
                Return m_Dismiss
            End Get
            Set(ByVal value As Boolean)
                m_Dismiss = value
            End Set
        End Property

        Public Property DismissArgument() As String Implements IAlarmCommands.DismissArgument
            Get
                Return m_DismissArgument
            End Get
            Set(ByVal value As String)
                m_DismissArgument = value
            End Set
        End Property

        Private m_Snooze As Boolean = False
        Private m_Dismiss As Boolean = False

        Private m_SnoozeArgument As String = String.Empty
        Private m_DismissArgument As String = String.Empty
    End Class


    Friend Class ToastNotificationBase
        Inherits NotificationBase
        Implements IToastNotificationContent

        Private Property IToastNotificationContent_StrictValidation() As Boolean Implements IToastNotificationContent.StrictValidation
            Get
                Return MyBase.StrictValidation
            End Get
            Set(value As Boolean)
                MyBase.StrictValidation = value
            End Set
        End Property

        Private Property IToastNotificationContent_Lang() As String Implements IToastNotificationContent.Lang
            Get
                Return MyBase.Lang
            End Get
            Set(value As String)
                MyBase.Lang = value
            End Set
        End Property

        Private Property IToastNotificationContent_BaseUri() As String Implements IToastNotificationContent.BaseUri
            Get
                Return MyBase.BaseUri
            End Get
            Set(value As String)
                MyBase.BaseUri = value
            End Set
        End Property

        Private Property IToastNotificationContent_AddImageQuery() As Boolean Implements IToastNotificationContent.AddImageQuery
            Get
                Return MyBase.AddImageQuery
            End Get
            Set(value As Boolean)
                MyBase.AddImageQuery = value
            End Set
        End Property

        Private Function INotificationContent_GetXml() As XmlDocument Implements INotificationContent.GetXml
            Return MyBase.GetXml()
        End Function

        Public Sub New(ByVal templateName As String, ByVal imageCount As Integer, ByVal textCount As Integer)
            MyBase.New(templateName:=templateName, fallbackName:=Nothing, imageCount:=imageCount, textCount:=textCount)
        End Sub

        Private Function AudioSrcIsLooping() As Boolean
            Return (Audio.Content = ToastAudioContent.LoopingAlarm) OrElse (Audio.Content = ToastAudioContent.LoopingAlarm2) OrElse (Audio.Content = ToastAudioContent.LoopingAlarm3) OrElse (Audio.Content = ToastAudioContent.LoopingAlarm4) OrElse (Audio.Content = ToastAudioContent.LoopingAlarm5) OrElse (Audio.Content = ToastAudioContent.LoopingAlarm6) OrElse (Audio.Content = ToastAudioContent.LoopingAlarm7) OrElse (Audio.Content = ToastAudioContent.LoopingAlarm8) OrElse (Audio.Content = ToastAudioContent.LoopingAlarm9) OrElse (Audio.Content = ToastAudioContent.LoopingAlarm10) OrElse (Audio.Content = ToastAudioContent.LoopingCall) OrElse (Audio.Content = ToastAudioContent.LoopingCall2) OrElse (Audio.Content = ToastAudioContent.LoopingCall3) OrElse (Audio.Content = ToastAudioContent.LoopingCall4) OrElse (Audio.Content = ToastAudioContent.LoopingCall5) OrElse (Audio.Content = ToastAudioContent.LoopingCall6) OrElse (Audio.Content = ToastAudioContent.LoopingCall7) OrElse (Audio.Content = ToastAudioContent.LoopingCall8) OrElse (Audio.Content = ToastAudioContent.LoopingCall9) OrElse (Audio.Content = ToastAudioContent.LoopingCall10)
        End Function

        Private Sub ValidateAudio()
            If StrictValidation Then
                If Audio.Loop AndAlso Duration <> ToastDuration.Long Then
                    Throw New NotificationContentValidationException("Looping audio is only available for long duration toasts.")
                End If
                If Audio.Loop AndAlso (Not AudioSrcIsLooping()) Then
                    Throw New NotificationContentValidationException("A looping audio src must be chosen if the looping audio property is set.")
                End If
                If (Not Audio.Loop) AndAlso AudioSrcIsLooping() Then
                    Throw New NotificationContentValidationException("The looping audio property needs to be set if a looping audio src is chosen.")
                End If
            End If
        End Sub

        Private Sub AppendAudioTag(ByVal builder As StringBuilder)
            If Audio.Content <> ToastAudioContent.Default Then
                builder.Append("<audio")
                If Audio.Content = ToastAudioContent.Silent Then
                    builder.Append(" silent='true'/>")
                Else
                    If Audio.Loop = True Then
                        builder.Append(" loop='true'")
                    End If

                    ' The default looping sound is LoopingCall - save size by not adding it.
                    If Audio.Content <> ToastAudioContent.LoopingCall Then
                        Dim audioSrc As String = Nothing
                        Select Case Audio.Content
                            Case ToastAudioContent.IM
                                audioSrc = "ms-winsoundevent:Notification.IM"
                            Case ToastAudioContent.Mail
                                audioSrc = "ms-winsoundevent:Notification.Mail"
                            Case ToastAudioContent.Reminder
                                audioSrc = "ms-winsoundevent:Notification.Reminder"
                            Case ToastAudioContent.SMS
                                audioSrc = "ms-winsoundevent:Notification.SMS"
                            Case ToastAudioContent.LoopingAlarm
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm"
                            Case ToastAudioContent.LoopingAlarm2
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm2"
                            Case ToastAudioContent.LoopingAlarm3
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm3"
                            Case ToastAudioContent.LoopingAlarm4
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm4"
                            Case ToastAudioContent.LoopingAlarm5
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm5"
                            Case ToastAudioContent.LoopingAlarm6
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm6"
                            Case ToastAudioContent.LoopingAlarm7
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm7"
                            Case ToastAudioContent.LoopingAlarm8
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm8"
                            Case ToastAudioContent.LoopingAlarm9
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm9"
                            Case ToastAudioContent.LoopingAlarm10
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm10"
                            Case ToastAudioContent.LoopingCall
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call"
                            Case ToastAudioContent.LoopingCall2
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call2"
                            Case ToastAudioContent.LoopingCall3
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call3"
                            Case ToastAudioContent.LoopingCall4
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call4"
                            Case ToastAudioContent.LoopingCall5
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call5"
                            Case ToastAudioContent.LoopingCall6
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call6"
                            Case ToastAudioContent.LoopingCall7
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call7"
                            Case ToastAudioContent.LoopingCall8
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call8"
                            Case ToastAudioContent.LoopingCall9
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call9"
                            Case ToastAudioContent.LoopingCall10
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call10"
                        End Select
                        builder.AppendFormat(" src='{0}'", audioSrc)
                    End If
                End If
                builder.Append("/>")
            End If
        End Sub

        Private Function IsIncomingCallToast() As Boolean
            Return (IncomingCallCommands.ShowVideoCommand OrElse IncomingCallCommands.ShowVoiceCommand OrElse IncomingCallCommands.ShowDeclineCommand)
        End Function

        Private Function IsAlarmToast() As Boolean
            Return (AlarmCommands.ShowSnoozeCommand OrElse AlarmCommands.ShowDismissCommand)
        End Function

        Private Sub ValidateCommands()
            If StrictValidation Then
                If IsIncomingCallToast() AndAlso IsAlarmToast() Then
                    Throw New NotificationContentValidationException("IncomingCall and Alarm properties cannot be included on a single toast.")
                End If

                If (Not IncomingCallCommands.ShowVideoCommand) AndAlso (Not String.IsNullOrEmpty(IncomingCallCommands.VideoArgument)) Then
                    Throw New NotificationContentValidationException("Video argument should not be set if the Video button is not shown.")
                End If

                If (Not IncomingCallCommands.ShowVoiceCommand) AndAlso (Not String.IsNullOrEmpty(IncomingCallCommands.VoiceArgument)) Then
                    Throw New NotificationContentValidationException("Voice argument should not be set if the Voice button is not shown.")
                End If

                If (Not IncomingCallCommands.ShowDeclineCommand) AndAlso (Not String.IsNullOrEmpty(IncomingCallCommands.DeclineArgument)) Then
                    Throw New NotificationContentValidationException("Decline argument should not be set if the Video button is not shown.")
                End If

                If (Not AlarmCommands.ShowSnoozeCommand) AndAlso (Not String.IsNullOrEmpty(AlarmCommands.SnoozeArgument)) Then
                    Throw New NotificationContentValidationException("Snooze argument should not be set if the Snooze button is not shown.")
                End If

                If (Not AlarmCommands.ShowDismissCommand) AndAlso (Not String.IsNullOrEmpty(AlarmCommands.DismissArgument)) Then
                    Throw New NotificationContentValidationException("Dismiss argument should not be set if the Dismiss button is not shown.")
                End If
            End If
        End Sub

        Private Sub AppendIncomingCallCommandsTag(ByVal builder As StringBuilder)
            builder.Append("<commands scenario='incomingcall'>")
            Dim CommandIDFormat As String = "<command id='{0}'"
            Dim ArgumentFormat As String = "arguments='{0}'"
            If IncomingCallCommands.ShowVideoCommand Then
                builder.AppendFormat(CommandIDFormat, "video")
                If Not String.IsNullOrEmpty(IncomingCallCommands.VideoArgument) Then
                    builder.AppendFormat(ArgumentFormat, Util.HttpEncode(IncomingCallCommands.VideoArgument))
                End If
                builder.Append("/>")
            End If
            If IncomingCallCommands.ShowVoiceCommand Then
                builder.AppendFormat(CommandIDFormat, "voice")
                If Not String.IsNullOrEmpty(IncomingCallCommands.VoiceArgument) Then
                    builder.AppendFormat(ArgumentFormat, Util.HttpEncode(IncomingCallCommands.VoiceArgument))
                End If
                builder.Append("/>")
            End If
            If IncomingCallCommands.ShowDeclineCommand Then
                builder.AppendFormat(CommandIDFormat, "decline")
                If Not String.IsNullOrEmpty(IncomingCallCommands.DeclineArgument) Then
                    builder.AppendFormat(ArgumentFormat, Util.HttpEncode(IncomingCallCommands.DeclineArgument))
                End If
                builder.Append("/>")
            End If
            builder.Append("</commands>")
        End Sub

        Private Sub AppendAlarmCommandsTag(ByVal builder As StringBuilder)
            builder.Append("<commands scenario='alarm'>")
            Dim CommandIDFormat As String = "<command id='{0}'"
            Dim ArgumentFormat As String = "arguments='{0}'"
            If AlarmCommands.ShowSnoozeCommand Then
                builder.AppendFormat(CommandIDFormat, "snooze")
                If Not String.IsNullOrEmpty(AlarmCommands.SnoozeArgument) Then
                    builder.AppendFormat(ArgumentFormat, Util.HttpEncode(AlarmCommands.SnoozeArgument))
                End If
                builder.Append("/>")
            End If
            If AlarmCommands.ShowDismissCommand Then
                builder.AppendFormat(CommandIDFormat, "dismiss")
                If Not String.IsNullOrEmpty(AlarmCommands.DismissArgument) Then
                    builder.AppendFormat(ArgumentFormat, Util.HttpEncode(AlarmCommands.DismissArgument))
                End If
                builder.Append("/>")
            End If
            builder.Append("</commands>")
        End Sub

        Public Overrides Function GetContent() As String Implements NotificationsExtensions.INotificationContent.GetContent
            ValidateAudio()
            ValidateCommands()

            Dim builder As New StringBuilder("<toast")
            If Not String.IsNullOrEmpty(Launch) Then
                builder.AppendFormat(" launch='{0}'", Util.HttpEncode(Launch))
            End If
            If Duration <> ToastDuration.Short Then
                builder.AppendFormat(" duration='{0}'", Duration.ToString().ToLowerInvariant())
            End If
            builder.Append(">")

            builder.AppendFormat("<visual version='{0}'", Util.NOTIFICATION_CONTENT_VERSION)
            If Not String.IsNullOrWhiteSpace(Lang) Then
                builder.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang))
            End If
            If Not String.IsNullOrWhiteSpace(BaseUri) Then
                builder.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri))
            End If
            If AddImageQuery Then
                builder.AppendFormat(" addImageQuery='true'")
            End If
            builder.Append(">")

            builder.AppendFormat("<binding template='{0}'>{1}</binding>", TemplateName, SerializeProperties(Lang, BaseUri, AddImageQuery))
            builder.Append("</visual>")

            AppendAudioTag(builder)

            If IsIncomingCallToast() Then
                AppendIncomingCallCommandsTag(builder)
            End If

            If IsAlarmToast() Then
                AppendAlarmCommandsTag(builder)
            End If

            builder.Append("</toast>")

            Return builder.ToString()
        End Function

#If Not WINRT_NOT_PRESENT Then
        Public Function CreateNotification() As ToastNotification Implements IToastNotificationContent.CreateNotification
            If StrictValidation Then
                If IsAlarmToast() Then
                    Throw New NotificationContentValidationException("The ToastNotification object should not be used for alarms." & "Use the results of GetContent() to construct a ScheduledToastNotification object.")
                End If
            End If
            Dim xmlDoc As New XmlDocument()
            xmlDoc.LoadXml(GetContent())
            Return New ToastNotification(xmlDoc)
        End Function
#End If

        Public ReadOnly Property Audio() As IToastAudio Implements IToastNotificationContent.Audio
            Get
                Return m_Audio
            End Get
        End Property

        Public ReadOnly Property IncomingCallCommands() As IIncomingCallCommands Implements IToastNotificationContent.IncomingCallCommands
            Get
                Return m_IncomingCallCommands
            End Get
        End Property

        Public ReadOnly Property AlarmCommands() As IAlarmCommands Implements IToastNotificationContent.AlarmCommands
            Get
                Return m_AlarmCommands
            End Get
        End Property

        Public Property Launch() As String Implements IToastNotificationContent.Launch
            Get
                Return m_Launch
            End Get
            Set(ByVal value As String)
                m_Launch = value
            End Set
        End Property

        Public Property Duration() As ToastDuration Implements IToastNotificationContent.Duration
            Get
                Return m_Duration
            End Get
            Set(ByVal value As ToastDuration)
                If Not System.Enum.IsDefined(GetType(ToastDuration), value) Then
                    Throw New ArgumentOutOfRangeException("value")
                End If
                m_Duration = value
            End Set
        End Property

        Private m_Launch As String
        Private m_Audio As IToastAudio = New ToastAudio()
        Private m_IncomingCallCommands As IIncomingCallCommands = New IncomingCallCommands()
        Private m_AlarmCommands As IAlarmCommands = New AlarmCommands()
        Private m_Duration As ToastDuration = ToastDuration.Short
    End Class

    Friend Class ToastImageAndText01
        Inherits ToastNotificationBase
        Implements IToastImageAndText01

        Private Property IToastNotificationContent_StrictValidation() As Boolean
            Get
                Return MyBase.StrictValidation
            End Get
            Set(value As Boolean)
                MyBase.StrictValidation = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Lang() As String
            Get
                Return MyBase.Lang
            End Get
            Set(value As String)
                MyBase.Lang = Value
            End Set
        End Property

        Private Property IToastNotificationContent_BaseUri() As String
            Get
                Return MyBase.BaseUri
            End Get
            Set(value As String)
                MyBase.BaseUri = Value
            End Set
        End Property

        Private Property IToastNotificationContent_AddImageQuery() As Boolean
            Get
                Return MyBase.AddImageQuery
            End Get
            Set(value As Boolean)
                MyBase.AddImageQuery = value
            End Set
        End Property

        Private Property IToastNotificationContent_Launch() As String
            Get
                Return MyBase.Launch
            End Get
            Set(value As String)
                MyBase.Launch = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_Audio() As IToastAudio
            Get
                Return MyBase.Audio
            End Get
        End Property

        Private Property IToastNotificationContent_Duration() As ToastDuration
            Get
                Return MyBase.Duration
            End Get
            Set(value As ToastDuration)
                MyBase.Duration = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_IncomingCallCommands() As IIncomingCallCommands
            Get
                Return MyBase.IncomingCallCommands
            End Get
        End Property

        Private ReadOnly Property IToastNotificationContent_AlarmCommands() As IAlarmCommands
            Get
                Return MyBase.AlarmCommands
            End Get
        End Property

        Private Function IToastNotificationContent_CreateNotification() As ToastNotification
            Return MyBase.CreateNotification()
        End Function

        Public Sub New()
            MyBase.New(templateName:="ToastImageAndText01", imageCount:=1, textCount:=1)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements IToastImageAndText01.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements IToastImageAndText01.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property
    End Class

    Friend Class ToastImageAndText02
        Inherits ToastNotificationBase
        Implements IToastImageAndText02

        Private Property IToastNotificationContent_StrictValidation() As Boolean
            Get
                Return MyBase.StrictValidation
            End Get
            Set(value As Boolean)
                MyBase.StrictValidation = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Lang() As String
            Get
                Return MyBase.Lang
            End Get
            Set(value As String)
                MyBase.Lang = Value
            End Set
        End Property

        Private Property IToastNotificationContent_BaseUri() As String
            Get
                Return MyBase.BaseUri
            End Get
            Set(value As String)
                MyBase.BaseUri = Value
            End Set
        End Property

        Private Property IToastNotificationContent_AddImageQuery() As Boolean
            Get
                Return MyBase.AddImageQuery
            End Get
            Set(value As Boolean)
                MyBase.AddImageQuery = value
            End Set
        End Property

        Private Property IToastNotificationContent_Launch() As String
            Get
                Return MyBase.Launch
            End Get
            Set(value As String)
                MyBase.Launch = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_Audio() As IToastAudio
            Get
                Return MyBase.Audio
            End Get
        End Property

        Private Property IToastNotificationContent_Duration() As ToastDuration
            Get
                Return MyBase.Duration
            End Get
            Set(value As ToastDuration)
                MyBase.Duration = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_IncomingCallCommands() As IIncomingCallCommands
            Get
                Return MyBase.IncomingCallCommands
            End Get
        End Property

        Private ReadOnly Property IToastNotificationContent_AlarmCommands() As IAlarmCommands
            Get
                Return MyBase.AlarmCommands
            End Get
        End Property

        Private Function IToastNotificationContent_CreateNotification() As ToastNotification
            Return MyBase.CreateNotification()
        End Function

        Public Sub New()
            MyBase.New(templateName:="ToastImageAndText02", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements IToastImageAndText02.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements IToastImageAndText02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements IToastImageAndText02.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property
    End Class

    Friend Class ToastImageAndText03
        Inherits ToastNotificationBase
        Implements IToastImageAndText03

        Private Property IToastNotificationContent_StrictValidation() As Boolean
            Get
                Return MyBase.StrictValidation
            End Get
            Set(value As Boolean)
                MyBase.StrictValidation = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Lang() As String
            Get
                Return MyBase.Lang
            End Get
            Set(value As String)
                MyBase.Lang = Value
            End Set
        End Property

        Private Property IToastNotificationContent_BaseUri() As String
            Get
                Return MyBase.BaseUri
            End Get
            Set(value As String)
                MyBase.BaseUri = Value
            End Set
        End Property

        Private Property IToastNotificationContent_AddImageQuery() As Boolean
            Get
                Return MyBase.AddImageQuery
            End Get
            Set(value As Boolean)
                MyBase.AddImageQuery = value
            End Set
        End Property

        Private Property IToastNotificationContent_Launch() As String
            Get
                Return MyBase.Launch
            End Get
            Set(value As String)
                MyBase.Launch = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_Audio() As IToastAudio
            Get
                Return MyBase.Audio
            End Get
        End Property

        Private Property IToastNotificationContent_Duration() As ToastDuration
            Get
                Return MyBase.Duration
            End Get
            Set(value As ToastDuration)
                MyBase.Duration = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_IncomingCallCommands() As IIncomingCallCommands
            Get
                Return MyBase.IncomingCallCommands
            End Get
        End Property

        Private ReadOnly Property IToastNotificationContent_AlarmCommands() As IAlarmCommands
            Get
                Return MyBase.AlarmCommands
            End Get
        End Property

        Private Function IToastNotificationContent_CreateNotification() As ToastNotification
            Return MyBase.CreateNotification()
        End Function

        Public Sub New()
            MyBase.New(templateName:="ToastImageAndText03", imageCount:=1, textCount:=2)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements IToastImageAndText03.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements IToastImageAndText03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody() As INotificationContentText Implements IToastImageAndText03.TextBody
            Get
                Return TextFields(1)
            End Get
        End Property
    End Class

    Friend Class ToastImageAndText04
        Inherits ToastNotificationBase
        Implements IToastImageAndText04

        Private Property IToastNotificationContent_StrictValidation() As Boolean
            Get
                Return MyBase.StrictValidation
            End Get
            Set(value As Boolean)
                MyBase.StrictValidation = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Lang() As String
            Get
                Return MyBase.Lang
            End Get
            Set(value As String)
                MyBase.Lang = Value
            End Set
        End Property

        Private Property IToastNotificationContent_BaseUri() As String
            Get
                Return MyBase.BaseUri
            End Get
            Set(value As String)
                MyBase.BaseUri = Value
            End Set
        End Property

        Private Property IToastNotificationContent_AddImageQuery() As Boolean
            Get
                Return MyBase.AddImageQuery
            End Get
            Set(value As Boolean)
                MyBase.AddImageQuery = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Launch() As String
            Get
                Return MyBase.Launch
            End Get
            Set(value As String)
                MyBase.Launch = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_Audio() As IToastAudio
            Get
                Return MyBase.Audio
            End Get
        End Property

        Private Property IToastNotificationContent_Duration() As ToastDuration
            Get
                Return MyBase.Duration
            End Get
            Set(value As ToastDuration)
                MyBase.Duration = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_IncomingCallCommands() As IIncomingCallCommands
            Get
                Return MyBase.IncomingCallCommands
            End Get
        End Property

        Private ReadOnly Property IToastNotificationContent_AlarmCommands() As IAlarmCommands
            Get
                Return MyBase.AlarmCommands
            End Get
        End Property

        Private Function IToastNotificationContent_CreateNotification() As ToastNotification
            Return MyBase.CreateNotification()
        End Function

        Public Sub New()
            MyBase.New(templateName:="ToastImageAndText04", imageCount:=1, textCount:=3)
        End Sub

        Public ReadOnly Property Image() As INotificationContentImage Implements IToastImageAndText04.Image
            Get
                Return Images(0)
            End Get
        End Property

        Public ReadOnly Property TextHeading() As INotificationContentText Implements IToastImageAndText04.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements IToastImageAndText04.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements IToastImageAndText04.TextBody2
            Get
                Return TextFields(2)
            End Get
        End Property
    End Class

    Friend Class ToastText01
        Inherits ToastNotificationBase
        Implements IToastText01

        Private Property IToastNotificationContent_StrictValidation() As Boolean
            Get
                Return MyBase.StrictValidation
            End Get
            Set(value As Boolean)
                MyBase.StrictValidation = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Lang() As String
            Get
                Return MyBase.Lang
            End Get
            Set(value As String)
                MyBase.Lang = Value
            End Set
        End Property

        Private Property IToastNotificationContent_BaseUri() As String
            Get
                Return MyBase.BaseUri
            End Get
            Set(value As String)
                MyBase.BaseUri = Value
            End Set
        End Property

        Private Property IToastNotificationContent_AddImageQuery() As Boolean
            Get
                Return MyBase.AddImageQuery
            End Get
            Set(value As Boolean)
                MyBase.AddImageQuery = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Launch() As String
            Get
                Return MyBase.Launch
            End Get
            Set(value As String)
                MyBase.Launch = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_Audio() As IToastAudio
            Get
                Return MyBase.Audio
            End Get
        End Property

        Private Property IToastNotificationContent_Duration() As ToastDuration
            Get
                Return MyBase.Duration
            End Get
            Set(value As ToastDuration)
                MyBase.Duration = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_IncomingCallCommands() As IIncomingCallCommands
            Get
                Return MyBase.IncomingCallCommands
            End Get
        End Property

        Private ReadOnly Property IToastNotificationContent_AlarmCommands() As IAlarmCommands
            Get
                Return MyBase.AlarmCommands
            End Get
        End Property

        Private Function IToastNotificationContent_CreateNotification() As ToastNotification
            Return MyBase.CreateNotification()
        End Function

        Public Sub New()
            MyBase.New(templateName:="ToastText01", imageCount:=0, textCount:=1)
        End Sub

        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements IToastText01.TextBodyWrap
            Get
                Return TextFields(0)
            End Get
        End Property
    End Class

    Friend Class ToastText02
        Inherits ToastNotificationBase
        Implements IToastText02

        Private Property IToastNotificationContent_StrictValidation() As Boolean
            Get
                Return MyBase.StrictValidation
            End Get
            Set(value As Boolean)
                MyBase.StrictValidation = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Lang() As String
            Get
                Return MyBase.Lang
            End Get
            Set(value As String)
                MyBase.Lang = Value
            End Set
        End Property

        Private Property IToastNotificationContent_BaseUri() As String
            Get
                Return MyBase.BaseUri
            End Get
            Set(value As String)
                MyBase.BaseUri = Value
            End Set
        End Property

        Private Property IToastNotificationContent_AddImageQuery() As Boolean
            Get
                Return MyBase.AddImageQuery
            End Get
            Set(value As Boolean)
                MyBase.AddImageQuery = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Launch() As String
            Get
                Return MyBase.Launch
            End Get
            Set(value As String)
                MyBase.Launch = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_Audio() As IToastAudio
            Get
                Return MyBase.Audio
            End Get
        End Property

        Private Property IToastNotificationContent_Duration() As ToastDuration
            Get
                Return MyBase.Duration
            End Get
            Set(value As ToastDuration)
                MyBase.Duration = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_IncomingCallCommands() As IIncomingCallCommands
            Get
                Return MyBase.IncomingCallCommands
            End Get
        End Property

        Private ReadOnly Property IToastNotificationContent_AlarmCommands() As IAlarmCommands
            Get
                Return MyBase.AlarmCommands
            End Get
        End Property

        Private Function IToastNotificationContent_CreateNotification() As ToastNotification
            Return MyBase.CreateNotification()
        End Function

        Public Sub New()
            MyBase.New(templateName:="ToastText02", imageCount:=0, textCount:=2)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements IToastText02.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBodyWrap() As INotificationContentText Implements IToastText02.TextBodyWrap
            Get
                Return TextFields(1)
            End Get
        End Property
    End Class

    Friend Class ToastText03
        Inherits ToastNotificationBase
        Implements IToastText03

        Private Property IToastNotificationContent_StrictValidation() As Boolean
            Get
                Return MyBase.StrictValidation
            End Get
            Set(value As Boolean)
                MyBase.StrictValidation = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Lang() As String
            Get
                Return MyBase.Lang
            End Get
            Set(value As String)
                MyBase.Lang = Value
            End Set
        End Property

        Private Property IToastNotificationContent_BaseUri() As String
            Get
                Return MyBase.BaseUri
            End Get
            Set(value As String)
                MyBase.BaseUri = Value
            End Set
        End Property

        Private Property IToastNotificationContent_AddImageQuery() As Boolean
            Get
                Return MyBase.AddImageQuery
            End Get
            Set(value As Boolean)
                MyBase.AddImageQuery = Value
            End Set
        End Property

        Private Property IToastNotificationContent_Launch() As String
            Get
                Return MyBase.Launch
            End Get
            Set(value As String)
                MyBase.Launch = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_Audio() As IToastAudio
            Get
                Return MyBase.Audio
            End Get
        End Property

        Private Property IToastNotificationContent_Duration() As ToastDuration
            Get
                Return MyBase.Duration
            End Get
            Set(value As ToastDuration)
                MyBase.Duration = Value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_IncomingCallCommands() As IIncomingCallCommands
            Get
                Return MyBase.IncomingCallCommands
            End Get
        End Property

        Private ReadOnly Property IToastNotificationContent_AlarmCommands() As IAlarmCommands
            Get
                Return MyBase.AlarmCommands
            End Get
        End Property

        Private Function IToastNotificationContent_CreateNotification() As ToastNotification
            Return MyBase.CreateNotification()
        End Function

        Public Sub New()
            MyBase.New(templateName:="ToastText03", imageCount:=0, textCount:=2)
        End Sub

        Public ReadOnly Property TextHeadingWrap() As INotificationContentText Implements IToastText03.TextHeadingWrap
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody() As INotificationContentText Implements IToastText03.TextBody
            Get
                Return TextFields(1)
            End Get
        End Property
    End Class

    Friend Class ToastText04
        Inherits ToastNotificationBase
        Implements IToastText04

        Private Property IToastNotificationContent_StrictValidation() As Boolean
            Get
                Return MyBase.StrictValidation
            End Get
            Set(value As Boolean)
                MyBase.StrictValidation = value
            End Set
        End Property

        Private Property IToastNotificationContent_Lang() As String
            Get
                Return MyBase.Lang
            End Get
            Set(value As String)
                MyBase.Lang = value
            End Set
        End Property

        Private Property IToastNotificationContent_BaseUri() As String
            Get
                Return MyBase.BaseUri
            End Get
            Set(value As String)
                MyBase.BaseUri = value
            End Set
        End Property

        Private Property IToastNotificationContent_AddImageQuery() As Boolean
            Get
                Return MyBase.AddImageQuery
            End Get
            Set(value As Boolean)
                MyBase.AddImageQuery = value
            End Set
        End Property

        Private Property IToastNotificationContent_Launch() As String
            Get
                Return MyBase.Launch
            End Get
            Set(value As String)
                MyBase.Launch = value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_Audio() As IToastAudio
            Get
                Return MyBase.Audio
            End Get
        End Property

        Private Property IToastNotificationContent_Duration() As ToastDuration
            Get
                Return MyBase.Duration
            End Get
            Set(value As ToastDuration)
                MyBase.Duration = value
            End Set
        End Property

        Private ReadOnly Property IToastNotificationContent_IncomingCallCommands() As IIncomingCallCommands
            Get
                Return MyBase.IncomingCallCommands
            End Get
        End Property

        Private ReadOnly Property IToastNotificationContent_AlarmCommands() As IAlarmCommands
            Get
                Return MyBase.AlarmCommands
            End Get
        End Property

        Private Function IToastNotificationContent_CreateNotification() As ToastNotification
            Return MyBase.CreateNotification()
        End Function

        Public Sub New()
            MyBase.New(templateName:="ToastText04", imageCount:=0, textCount:=3)
        End Sub

        Public ReadOnly Property TextHeading() As INotificationContentText Implements IToastText04.TextHeading
            Get
                Return TextFields(0)
            End Get
        End Property
        Public ReadOnly Property TextBody1() As INotificationContentText Implements IToastText04.TextBody1
            Get
                Return TextFields(1)
            End Get
        End Property
        Public ReadOnly Property TextBody2() As INotificationContentText Implements IToastText04.TextBody2
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
            Return New ToastImageAndText01()
        End Function

        ''' <summary>
        ''' Creates a ToastImageAndText02 template content object.
        ''' </summary>
        ''' <returns>A ToastImageAndText02 template content object.</returns>
        Public Shared Function CreateToastImageAndText02() As IToastImageAndText02
            Return New ToastImageAndText02()
        End Function

        ''' <summary>
        ''' Creates a ToastImageAndText03 template content object.
        ''' </summary>
        ''' <returns>A ToastImageAndText03 template content object.</returns>
        Public Shared Function CreateToastImageAndText03() As IToastImageAndText03
            Return New ToastImageAndText03()
        End Function

        ''' <summary>
        ''' Creates a ToastImageAndText04 template content object.
        ''' </summary>
        ''' <returns>A ToastImageAndText04 template content object.</returns>
        Public Shared Function CreateToastImageAndText04() As IToastImageAndText04
            Return New ToastImageAndText04()
        End Function

        ''' <summary>
        ''' Creates a ToastText01 template content object.
        ''' </summary>
        ''' <returns>A ToastText01 template content object.</returns>
        Public Shared Function CreateToastText01() As IToastText01
            Return New ToastText01()
        End Function

        ''' <summary>
        ''' Creates a ToastText02 template content object.
        ''' </summary>
        ''' <returns>A ToastText02 template content object.</returns>
        Public Shared Function CreateToastText02() As IToastText02
            Return New ToastText02()
        End Function

        ''' <summary>
        ''' Creates a ToastText03 template content object.
        ''' </summary>
        ''' <returns>A ToastText03 template content object.</returns>
        Public Shared Function CreateToastText03() As IToastText03
            Return New ToastText03()
        End Function

        ''' <summary>
        ''' Creates a ToastText04 template content object.
        ''' </summary>
        ''' <returns>A ToastText04 template content object.</returns>
        Public Shared Function CreateToastText04() As IToastText04
            Return New ToastText04()
        End Function
    End Class
End Namespace
