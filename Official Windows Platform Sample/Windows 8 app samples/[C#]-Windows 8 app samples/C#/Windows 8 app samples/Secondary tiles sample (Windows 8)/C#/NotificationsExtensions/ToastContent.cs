// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Text;
#if !WINRT_NOT_PRESENT
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
#endif

namespace NotificationsExtensions.ToastContent
{
    internal sealed class ToastAudio : IToastAudio
    {
        internal ToastAudio() { }

        public ToastAudioContent Content
        {
            get { return m_Content; }
            set
            {
                if (!Enum.IsDefined(typeof(ToastAudioContent), value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                m_Content = value;
            }
        }

        public bool Loop
        {
            get { return m_Loop; }
            set { m_Loop = value; }
        }

        private ToastAudioContent m_Content = ToastAudioContent.Default;
        private bool m_Loop = false;
    }

    internal class ToastNotificationBase : NotificationBase, IToastNotificationContent
    {
        public ToastNotificationBase(string templateName, int imageCount, int textCount) : base(templateName, imageCount, textCount)
        {
        }

        private bool AudioSrcIsLooping()
        {
            return (Audio.Content == ToastAudioContent.LoopingAlarm) || (Audio.Content == ToastAudioContent.LoopingAlarm2) ||
                (Audio.Content == ToastAudioContent.LoopingCall) || (Audio.Content == ToastAudioContent.LoopingCall2);
        }

        private void ValidateAudio()
        {
            if (StrictValidation)
            {
                if (Audio.Loop && Duration != ToastDuration.Long)
                {
                    throw new NotificationContentValidationException("Looping audio is only available for long duration toasts.");
                }
                if (Audio.Loop && !AudioSrcIsLooping())
                {
                    throw new NotificationContentValidationException(
                        "A looping audio src must be chosen if the looping audio property is set.");
                }
                if (!Audio.Loop && AudioSrcIsLooping())
                {
                    throw new NotificationContentValidationException(
                        "The looping audio property needs to be set if a looping audio src is chosen.");
                }
            }
        }

        private void AppendAudioTag(StringBuilder builder)
        {
            if (Audio.Content != ToastAudioContent.Default)
            {
                builder.Append("<audio");
                if (Audio.Content == ToastAudioContent.Silent)
                {
                    builder.Append(" silent='true'/>");
                }
                else
                {
                    if (Audio.Loop == true)
                    {
                        builder.Append(" loop='true'");
                    }

                    // The default looping sound is LoopingCall - save size by not adding it
                    if (Audio.Content != ToastAudioContent.LoopingCall)
                    {
                        string audioSrc = null;
                        switch (Audio.Content)
                        {
                            case ToastAudioContent.IM:
                                audioSrc = "ms-winsoundevent:Notification.IM";
                                break;
                            case ToastAudioContent.Mail:
                                audioSrc = "ms-winsoundevent:Notification.Mail";
                                break;
                            case ToastAudioContent.Reminder:
                                audioSrc = "ms-winsoundevent:Notification.Reminder";
                                break;
                            case ToastAudioContent.SMS:
                                audioSrc = "ms-winsoundevent:Notification.SMS";
                                break;
                            case ToastAudioContent.LoopingAlarm:
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm";
                                break;
                            case ToastAudioContent.LoopingAlarm2:
                                audioSrc = "ms-winsoundevent:Notification.Looping.Alarm2";
                                break;
                            case ToastAudioContent.LoopingCall:
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call";
                                break;
                            case ToastAudioContent.LoopingCall2:
                                audioSrc = "ms-winsoundevent:Notification.Looping.Call2";
                                break;
                        }
                        builder.AppendFormat(" src='{0}'", audioSrc);
                    }
                }
                builder.Append("/>");
            }
        }

        public override string GetContent()
        {
            ValidateAudio();

            StringBuilder builder = new StringBuilder("<toast");
            if (!String.IsNullOrEmpty(Launch))
            {
                builder.AppendFormat(" launch='{0}'", Util.HttpEncode(Launch));
            }
            if (Duration != ToastDuration.Short)
            {
                builder.AppendFormat(" duration='{0}'", Duration.ToString().ToLowerInvariant());
            }
            builder.Append(">");

            builder.AppendFormat("<visual version='{0}'", Util.NOTIFICATION_CONTENT_VERSION);
            if (!String.IsNullOrWhiteSpace(Lang))
            {
                builder.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang));
            }
            if (!String.IsNullOrWhiteSpace(BaseUri))
            {
                builder.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri));
            }
            if (AddImageQuery)
            {
                builder.AppendFormat(" addImageQuery='true'");
            }
            builder.Append(">");
            
            builder.AppendFormat("<binding template='{0}'>{1}</binding>", TemplateName, SerializeProperties(Lang, BaseUri, AddImageQuery));
            builder.Append("</visual>");

            AppendAudioTag(builder);
            
            builder.Append("</toast>");

            return builder.ToString();
        }
        
#if !WINRT_NOT_PRESENT
        public ToastNotification CreateNotification()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(GetContent());
            return new ToastNotification(xmlDoc);
        }
#endif

        public IToastAudio Audio
        {
            get { return m_Audio; }
        }

        public string Launch
        {
            get { return m_Launch; }
            set { m_Launch = value; }
        }

        public ToastDuration Duration
        {
            get { return m_Duration; }
            set
            {
                if (!Enum.IsDefined(typeof(ToastDuration), value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                m_Duration = value;
            }
        }

        private string m_Launch;
        private IToastAudio m_Audio = new ToastAudio();
        private ToastDuration m_Duration = ToastDuration.Short;
    }

    internal class ToastImageAndText01 : ToastNotificationBase, IToastImageAndText01
    {
        public ToastImageAndText01() : base(templateName: "ToastImageAndText01", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextBodyWrap { get { return TextFields[0]; } }
    }

    internal class ToastImageAndText02 : ToastNotificationBase, IToastImageAndText02
    {
        public ToastImageAndText02() : base(templateName: "ToastImageAndText02", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class ToastImageAndText03 : ToastNotificationBase, IToastImageAndText03
    {
        public ToastImageAndText03() : base(templateName: "ToastImageAndText03", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeadingWrap { get { return TextFields[0]; } }
        public INotificationContentText TextBody { get { return TextFields[1]; } }
    }

    internal class ToastImageAndText04 : ToastNotificationBase, IToastImageAndText04
    {
        public ToastImageAndText04() : base(templateName: "ToastImageAndText04", imageCount: 1, textCount: 3)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return TextFields[2]; } }
    }

    internal class ToastText01 : ToastNotificationBase, IToastText01
    {
        public ToastText01() : base(templateName: "ToastText01", imageCount: 0, textCount: 1)
        {
        }

        public INotificationContentText TextBodyWrap { get { return TextFields[0]; } }
    }

    internal class ToastText02 : ToastNotificationBase, IToastText02
    {
        public ToastText02() : base(templateName: "ToastText02", imageCount: 0, textCount: 2)
        {
        }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class ToastText03 : ToastNotificationBase, IToastText03
    {
        public ToastText03() : base(templateName: "ToastText03", imageCount: 0, textCount: 2)
        {
        }

        public INotificationContentText TextHeadingWrap { get { return TextFields[0]; } }
        public INotificationContentText TextBody { get { return TextFields[1]; } }
    }

    internal class ToastText04 : ToastNotificationBase, IToastText04
    {
        public ToastText04() : base(templateName: "ToastText04", imageCount: 0, textCount: 3)
        {
        }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return TextFields[2]; } }
    }

    /// <summary>
    /// A factory which creates toast content objects for all of the toast template types.
    /// </summary>
    public static class ToastContentFactory
    {
        /// <summary>
        /// Creates a ToastImageAndText01 template content object.
        /// </summary>
        /// <returns>A ToastImageAndText01 template content object.</returns>
        public static IToastImageAndText01 CreateToastImageAndText01()
        {
            return new ToastImageAndText01();
        }

        /// <summary>
        /// Creates a ToastImageAndText02 template content object.
        /// </summary>
        /// <returns>A ToastImageAndText02 template content object.</returns>
        public static IToastImageAndText02 CreateToastImageAndText02()
        {
            return new ToastImageAndText02();
        }

        /// <summary>
        /// Creates a ToastImageAndText03 template content object.
        /// </summary>
        /// <returns>A ToastImageAndText03 template content object.</returns>
        public static IToastImageAndText03 CreateToastImageAndText03()
        {
            return new ToastImageAndText03();
        }

        /// <summary>
        /// Creates a ToastImageAndText04 template content object.
        /// </summary>
        /// <returns>A ToastImageAndText04 template content object.</returns>
        public static IToastImageAndText04 CreateToastImageAndText04()
        {
            return new ToastImageAndText04();
        }

        /// <summary>
        /// Creates a ToastText01 template content object.
        /// </summary>
        /// <returns>A ToastText01 template content object.</returns>
        public static IToastText01 CreateToastText01()
        {
            return new ToastText01();
        }

        /// <summary>
        /// Creates a ToastText02 template content object.
        /// </summary>
        /// <returns>A ToastText02 template content object.</returns>
        public static IToastText02 CreateToastText02()
        {
            return new ToastText02();
        }

        /// <summary>
        /// Creates a ToastText03 template content object.
        /// </summary>
        /// <returns>A ToastText03 template content object.</returns>
        public static IToastText03 CreateToastText03()
        {
            return new ToastText03();
        }

        /// <summary>
        /// Creates a ToastText04 template content object.
        /// </summary>
        /// <returns>A ToastText04 template content object.</returns>
        public static IToastText04 CreateToastText04()
        {
            return new ToastText04();
        }
    }
}
