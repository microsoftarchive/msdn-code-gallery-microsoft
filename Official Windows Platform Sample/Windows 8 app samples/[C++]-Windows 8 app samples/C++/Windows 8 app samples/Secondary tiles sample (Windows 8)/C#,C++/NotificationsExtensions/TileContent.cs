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

namespace NotificationsExtensions.TileContent
{
    internal abstract class TileNotificationBase : NotificationBase
    {
        public TileNotificationBase(string templateName, int imageCount, int textCount) : base(templateName, imageCount, textCount)
        {
        }

        public TileBranding Branding
        {
            get { return m_Branding; }
            set
            {
                if (!Enum.IsDefined(typeof(TileBranding), value))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                m_Branding = value;
            }
        }
        
#if !WINRT_NOT_PRESENT
        public TileNotification CreateNotification()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(GetContent());
            return new TileNotification(xmlDoc);
        }
#endif

        private TileBranding m_Branding = TileBranding.Logo;
    }

    internal interface ISquareTileInternal
    {
        string SerializeBinding(string globalLang, string globalBaseUri, TileBranding globalBranding, bool globalAddImageQuery);
    }

    internal class TileSquareBase : TileNotificationBase, ISquareTileInternal
    {
        public TileSquareBase(string templateName, int imageCount, int textCount) : base(templateName, imageCount, textCount)
        {
        }               

        public override string GetContent()
        {
            StringBuilder builder = new StringBuilder(String.Empty);
            builder.AppendFormat("<tile><visual version='{0}'", Util.NOTIFICATION_CONTENT_VERSION);
            if (!String.IsNullOrWhiteSpace(Lang))
            {
                builder.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang));
            }
            if (Branding != TileBranding.Logo)
            {
                builder.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant());
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
            builder.Append(SerializeBinding(Lang, BaseUri, Branding, AddImageQuery));
            builder.Append("</visual></tile>");
            return builder.ToString();
        }

        public string SerializeBinding(string globalLang, string globalBaseUri, TileBranding globalBranding, bool globalAddImageQuery)
        {
            StringBuilder bindingNode = new StringBuilder(String.Empty);
            bindingNode.AppendFormat("<binding template='{0}'", TemplateName);
            if (!String.IsNullOrWhiteSpace(Lang) && !Lang.Equals(globalLang))
            {
                bindingNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang));
                globalLang = Lang;
            }
            if (Branding != TileBranding.Logo && Branding != globalBranding)
            {
                bindingNode.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant());
            }
            if (!String.IsNullOrWhiteSpace(BaseUri) && !BaseUri.Equals(globalBaseUri))
            {
                bindingNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri));
                globalBaseUri = BaseUri;
            }
            if (AddImageQueryNullable != null && AddImageQueryNullable != globalAddImageQuery)
            {
                bindingNode.AppendFormat(" addImageQuery='{0}'", AddImageQuery.ToString().ToLowerInvariant());
                globalAddImageQuery = AddImageQuery;
            }           
            bindingNode.AppendFormat(">{0}</binding>", SerializeProperties(globalLang, globalBaseUri, globalAddImageQuery));

            return bindingNode.ToString();
        }
    }

    internal class TileWideBase : TileNotificationBase
    {
        public TileWideBase(string templateName, int imageCount, int textCount) : base(templateName, imageCount, textCount)
        {
        }

        public ISquareTileNotificationContent SquareContent
        {
            get { return m_SquareContent; }
            set { m_SquareContent = value; }
        }

        public bool RequireSquareContent
        {
            get { return m_RequireSquareContent; }
            set { m_RequireSquareContent = value; }
        }

        public override string GetContent()
        {
            if (RequireSquareContent && SquareContent == null)
            {
                throw new NotificationContentValidationException(
                    "Square tile content should be included with each wide tile. " +
                    "If this behavior is undesired, use the RequireSquareContent property.");
            }

            StringBuilder visualNode = new StringBuilder(String.Empty);
            visualNode.AppendFormat("<visual version='{0}'", Util.NOTIFICATION_CONTENT_VERSION);
            if (!String.IsNullOrWhiteSpace(Lang))
            {
                visualNode.AppendFormat(" lang='{0}'", Util.HttpEncode(Lang));
            }
            if (Branding != TileBranding.Logo)
            {
                visualNode.AppendFormat(" branding='{0}'", Branding.ToString().ToLowerInvariant());
            }
            if (!String.IsNullOrWhiteSpace(BaseUri))
            {
                visualNode.AppendFormat(" baseUri='{0}'", Util.HttpEncode(BaseUri));
            }
            if (AddImageQuery)
            {
                visualNode.AppendFormat(" addImageQuery='true'");
            }
            visualNode.Append(">");

            StringBuilder builder = new StringBuilder(String.Empty);
            builder.AppendFormat("<tile>{0}<binding template='{1}'>{2}</binding>", visualNode, TemplateName, SerializeProperties(Lang, BaseUri, AddImageQuery));
            if (SquareContent != null)
            {
                ISquareTileInternal squareBase = SquareContent as ISquareTileInternal;
                if (squareBase == null)
                {
                    throw new NotificationContentValidationException("The provided square tile content class is unsupported.");
                }
                builder.Append(squareBase.SerializeBinding(Lang, BaseUri, Branding, AddImageQuery));
            }
            builder.Append("</visual></tile>");

            return builder.ToString();
        }

        private ISquareTileNotificationContent m_SquareContent = null;
        private bool m_RequireSquareContent = true;
    }

    internal class TileSquareBlock : TileSquareBase, ITileSquareBlock
    {
        public TileSquareBlock() : base(templateName: "TileSquareBlock", imageCount: 0, textCount: 2)
        {
        }

        public INotificationContentText TextBlock { get { return TextFields[0]; } }
        public INotificationContentText TextSubBlock { get { return TextFields[1]; } }
    }

    internal class TileSquareImage : TileSquareBase, ITileSquareImage
    {
        public TileSquareImage() : base(templateName: "TileSquareImage", imageCount: 1, textCount: 0)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }
    }

    internal class TileSquarePeekImageAndText01 : TileSquareBase, ITileSquarePeekImageAndText01
    {
        public TileSquarePeekImageAndText01() : base(templateName: "TileSquarePeekImageAndText01", imageCount: 1, textCount: 4)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }
        
        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return TextFields[3]; } }
    }

    internal class TileSquarePeekImageAndText02 : TileSquareBase, ITileSquarePeekImageAndText02
    {
        public TileSquarePeekImageAndText02() : base(templateName: "TileSquarePeekImageAndText02", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class TileSquarePeekImageAndText03 : TileSquareBase, ITileSquarePeekImageAndText03
    {
        public TileSquarePeekImageAndText03() : base(templateName: "TileSquarePeekImageAndText03", imageCount: 1, textCount: 4)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextBody1 { get { return TextFields[0]; } }
        public INotificationContentText TextBody2 { get { return TextFields[1]; } }
        public INotificationContentText TextBody3 { get { return TextFields[2]; } }
        public INotificationContentText TextBody4 { get { return TextFields[3]; } }
    }

    internal class TileSquarePeekImageAndText04 : TileSquareBase, ITileSquarePeekImageAndText04
    {
        public TileSquarePeekImageAndText04() : base(templateName: "TileSquarePeekImageAndText04", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextBodyWrap { get { return TextFields[0]; } }
    }

    internal class TileSquareText01 : TileSquareBase, ITileSquareText01
    {
        public TileSquareText01() : base(templateName: "TileSquareText01", imageCount: 0, textCount: 4)
        {
        }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return TextFields[3]; } }
    }

    internal class TileSquareText02 : TileSquareBase, ITileSquareText02
    {
        public TileSquareText02() : base(templateName: "TileSquareText02", imageCount: 0, textCount: 2)
        {
        }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class TileSquareText03 : TileSquareBase, ITileSquareText03
    {
        public TileSquareText03() : base(templateName: "TileSquareText03", imageCount: 0, textCount: 4) 
        {
        }

        public INotificationContentText TextBody1 { get { return TextFields[0]; } }
        public INotificationContentText TextBody2 { get { return TextFields[1]; } }
        public INotificationContentText TextBody3 { get { return TextFields[2]; } }
        public INotificationContentText TextBody4 { get { return TextFields[3]; } }
    }

    internal class TileSquareText04 : TileSquareBase, ITileSquareText04
    {
        public TileSquareText04() : base(templateName: "TileSquareText04", imageCount: 0, textCount: 1)
        {
        }

        public INotificationContentText TextBodyWrap { get { return TextFields[0]; } }
    }

    internal class TileWideBlockAndText01 : TileWideBase, ITileWideBlockAndText01
    {
        public TileWideBlockAndText01() : base(templateName: "TileWideBlockAndText01", imageCount: 0, textCount: 6)
        {
        }

        public INotificationContentText TextBody1 { get { return TextFields[0]; } }
        public INotificationContentText TextBody2 { get { return TextFields[1]; } }
        public INotificationContentText TextBody3 { get { return TextFields[2]; } }
        public INotificationContentText TextBody4 { get { return TextFields[3]; } }
        public INotificationContentText TextBlock { get { return TextFields[4]; } }
        public INotificationContentText TextSubBlock { get { return TextFields[5]; } }
    }

    internal class TileWideBlockAndText02 : TileWideBase, ITileWideBlockAndText02
    {
        public TileWideBlockAndText02() : base(templateName: "TileWideBlockAndText02", imageCount: 0, textCount: 6)
        {
        }

        public INotificationContentText TextBodyWrap { get { return TextFields[0]; } }
        public INotificationContentText TextBlock { get { return TextFields[1]; } }
        public INotificationContentText TextSubBlock { get { return TextFields[2]; } }
    }

    internal class TileWideImage : TileWideBase, ITileWideImage
    {
        public TileWideImage() : base(templateName: "TileWideImage", imageCount: 1, textCount: 0)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }
    }

    internal class TileWideImageAndText01 : TileWideBase, ITileWideImageAndText01
    {
        public TileWideImageAndText01() : base(templateName: "TileWideImageAndText01", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextCaptionWrap { get { return TextFields[0]; } }
    }

    internal class TileWideImageAndText02 : TileWideBase, ITileWideImageAndText02
    {
        public TileWideImageAndText02() : base(templateName: "TileWideImageAndText02", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextCaption1 { get { return TextFields[0]; } }
        public INotificationContentText TextCaption2 { get { return TextFields[1]; } }
    }

    internal class TileWideImageCollection : TileWideBase, ITileWideImageCollection
    {
        public TileWideImageCollection() : base(templateName: "TileWideImageCollection", imageCount: 5, textCount: 0)
        {
        }

        public INotificationContentImage ImageMain { get { return Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return Images[4]; } }
    }

    internal class TileWidePeekImage01 : TileWideBase, ITileWidePeekImage01
    {
        public TileWidePeekImage01() : base(templateName: "TileWidePeekImage01", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class TileWidePeekImage02 : TileWideBase, ITileWidePeekImage02
    {
        public TileWidePeekImage02() : base(templateName: "TileWidePeekImage02", imageCount: 1, textCount: 5)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return TextFields[3]; } }
        public INotificationContentText TextBody4 { get { return TextFields[4]; } }
    }

    internal class TileWidePeekImage03 : TileWideBase, ITileWidePeekImage03
    {
        public TileWidePeekImage03() : base(templateName: "TileWidePeekImage03", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeadingWrap { get { return TextFields[0]; } }
    }

    internal class TileWidePeekImage04 : TileWideBase, ITileWidePeekImage04
    {
        public TileWidePeekImage04() : base(templateName: "TileWidePeekImage04", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextBodyWrap { get { return TextFields[0]; } }
    }

    internal class TileWidePeekImage05 : TileWideBase, ITileWidePeekImage05
    {
        public TileWidePeekImage05() : base(templateName: "TileWidePeekImage05", imageCount: 2, textCount: 2)
        {
        }

        public INotificationContentImage ImageMain { get { return Images[0]; } }
        public INotificationContentImage ImageSecondary { get { return Images[1]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class TileWidePeekImage06 : TileWideBase, ITileWidePeekImage06
    {
        public TileWidePeekImage06() : base(templateName: "TileWidePeekImage06", imageCount: 2, textCount: 1)
        {
        }

        public INotificationContentImage ImageMain { get { return Images[0]; } }
        public INotificationContentImage ImageSecondary { get { return Images[1]; } }

        public INotificationContentText TextHeadingWrap { get { return TextFields[0]; } }
    }

    internal class TileWidePeekImageAndText01 : TileWideBase, ITileWidePeekImageAndText01
    {
        public TileWidePeekImageAndText01() : base(templateName: "TileWidePeekImageAndText01", imageCount: 1, textCount: 1) 
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextBodyWrap { get { return TextFields[0]; } }
    }

    internal class TileWidePeekImageAndText02 : TileWideBase, ITileWidePeekImageAndText02
    {
        public TileWidePeekImageAndText02() : base(templateName: "TileWidePeekImageAndText02", imageCount: 1, textCount: 5) 
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextBody1 { get { return TextFields[0]; } }
        public INotificationContentText TextBody2 { get { return TextFields[1]; } }
        public INotificationContentText TextBody3 { get { return TextFields[2]; } }
        public INotificationContentText TextBody4 { get { return TextFields[3]; } }
        public INotificationContentText TextBody5 { get { return TextFields[4]; } }
    }

    internal class TileWidePeekImageCollection01 : TileWideBase, ITileWidePeekImageCollection01
    {
        public TileWidePeekImageCollection01() : base(templateName: "TileWidePeekImageCollection01", imageCount: 5, textCount: 2)
        {
        }

        public INotificationContentImage ImageMain { get { return Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return Images[4]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class TileWidePeekImageCollection02 : TileWideBase, ITileWidePeekImageCollection02
    {
        public TileWidePeekImageCollection02() : base(templateName: "TileWidePeekImageCollection02", imageCount: 5, textCount: 5)
        {
        }

        public INotificationContentImage ImageMain { get { return Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return Images[4]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return TextFields[3]; } }
        public INotificationContentText TextBody4 { get { return TextFields[4]; } }
    }

    internal class TileWidePeekImageCollection03 : TileWideBase, ITileWidePeekImageCollection03
    {
        public TileWidePeekImageCollection03() : base(templateName: "TileWidePeekImageCollection03", imageCount: 5, textCount: 1)
        {
        }

        public INotificationContentImage ImageMain { get { return Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return Images[4]; } }

        public INotificationContentText TextHeadingWrap { get { return TextFields[0]; } }
    }

    internal class TileWidePeekImageCollection04 : TileWideBase, ITileWidePeekImageCollection04
    {
        public TileWidePeekImageCollection04() : base(templateName: "TileWidePeekImageCollection04", imageCount: 5, textCount: 1)
        {
        }

        public INotificationContentImage ImageMain { get { return Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return Images[4]; } }

        public INotificationContentText TextBodyWrap { get { return TextFields[0]; } }
    }

    internal class TileWidePeekImageCollection05 : TileWideBase, ITileWidePeekImageCollection05
    {
        public TileWidePeekImageCollection05() : base(templateName: "TileWidePeekImageCollection05", imageCount: 6, textCount: 2)
        {
        }

        public INotificationContentImage ImageMain { get { return Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return Images[4]; } }
        public INotificationContentImage ImageSecondary { get { return Images[5]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class TileWidePeekImageCollection06 : TileWideBase, ITileWidePeekImageCollection06
    {
        public TileWidePeekImageCollection06() : base(templateName: "TileWidePeekImageCollection06", imageCount: 6, textCount: 1)
        {
        }

        public INotificationContentImage ImageMain { get { return Images[0]; } }
        public INotificationContentImage ImageSmallColumn1Row1 { get { return Images[1]; } }
        public INotificationContentImage ImageSmallColumn2Row1 { get { return Images[2]; } }
        public INotificationContentImage ImageSmallColumn1Row2 { get { return Images[3]; } }
        public INotificationContentImage ImageSmallColumn2Row2 { get { return Images[4]; } }
        public INotificationContentImage ImageSecondary { get { return Images[5]; } }

        public INotificationContentText TextHeadingWrap { get { return TextFields[0]; } }
    }

    internal class TileWideSmallImageAndText01 : TileWideBase, ITileWideSmallImageAndText01
    {
        public TileWideSmallImageAndText01() : base(templateName: "TileWideSmallImageAndText01", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeadingWrap { get { return TextFields[0]; } }
    }
    
    internal class TileWideSmallImageAndText02 : TileWideBase, ITileWideSmallImageAndText02
    {
        public TileWideSmallImageAndText02() : base(templateName: "TileWideSmallImageAndText02", imageCount: 1, textCount: 5)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return TextFields[3]; } }
        public INotificationContentText TextBody4 { get { return TextFields[4]; } }
    }

    internal class TileWideSmallImageAndText03 : TileWideBase, ITileWideSmallImageAndText03
    {
        public TileWideSmallImageAndText03() : base(templateName: "TileWideSmallImageAndText03", imageCount: 1, textCount: 1)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextBodyWrap { get { return TextFields[0]; } }
    }

    internal class TileWideSmallImageAndText04 : TileWideBase, ITileWideSmallImageAndText04
    {
        public TileWideSmallImageAndText04() : base(templateName: "TileWideSmallImageAndText04", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class TileWideSmallImageAndText05 : TileWideBase, ITileWideSmallImageAndText05
    {
        public TileWideSmallImageAndText05() : base(templateName: "TileWideSmallImageAndText05", imageCount: 1, textCount: 2)
        {
        }

        public INotificationContentImage Image { get { return Images[0]; } }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class TileWideText01 : TileWideBase, ITileWideText01
    {
        public TileWideText01() : base(templateName: "TileWideText01", imageCount: 0, textCount: 5)
        {
        }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBody1 { get { return TextFields[1]; } }
        public INotificationContentText TextBody2 { get { return TextFields[2]; } }
        public INotificationContentText TextBody3 { get { return TextFields[3]; } }
        public INotificationContentText TextBody4 { get { return TextFields[4]; } }
    }

    internal class TileWideText02 : TileWideBase, ITileWideText02
    {
        public TileWideText02() : base(templateName: "TileWideText02", imageCount: 0, textCount: 9)
        {
        }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextColumn1Row1 { get { return TextFields[1]; } }
        public INotificationContentText TextColumn2Row1 { get { return TextFields[2]; } }
        public INotificationContentText TextColumn1Row2 { get { return TextFields[3]; } }
        public INotificationContentText TextColumn2Row2 { get { return TextFields[4]; } }
        public INotificationContentText TextColumn1Row3 { get { return TextFields[5]; } }
        public INotificationContentText TextColumn2Row3 { get { return TextFields[6]; } }
        public INotificationContentText TextColumn1Row4 { get { return TextFields[7]; } }
        public INotificationContentText TextColumn2Row4 { get { return TextFields[8]; } }
    }

    internal class TileWideText03 : TileWideBase, ITileWideText03
    {
        public TileWideText03() : base(templateName: "TileWideText03", imageCount: 0, textCount: 1)
        {
        }

        public INotificationContentText TextHeadingWrap { get { return TextFields[0]; } }
    }

    internal class TileWideText04 : TileWideBase, ITileWideText04
    {
        public TileWideText04() : base(templateName: "TileWideText04", imageCount: 0, textCount: 1)
        {
        }

        public INotificationContentText TextBodyWrap { get { return TextFields[0]; } }
    }

    internal class TileWideText05 : TileWideBase, ITileWideText05
    {
        public TileWideText05() : base(templateName: "TileWideText05", imageCount: 0, textCount: 5)
        {
        }

        public INotificationContentText TextBody1 { get { return TextFields[0]; } }
        public INotificationContentText TextBody2 { get { return TextFields[1]; } }
        public INotificationContentText TextBody3 { get { return TextFields[2]; } }
        public INotificationContentText TextBody4 { get { return TextFields[3]; } }
        public INotificationContentText TextBody5 { get { return TextFields[4]; } }
    }

    internal class TileWideText06 : TileWideBase, ITileWideText06
    {
        public TileWideText06() : base(templateName: "TileWideText06", imageCount: 0, textCount: 10)
        {
        }

        public INotificationContentText TextColumn1Row1 { get { return TextFields[0]; } }
        public INotificationContentText TextColumn2Row1 { get { return TextFields[1]; } }
        public INotificationContentText TextColumn1Row2 { get { return TextFields[2]; } }
        public INotificationContentText TextColumn2Row2 { get { return TextFields[3]; } }
        public INotificationContentText TextColumn1Row3 { get { return TextFields[4]; } }
        public INotificationContentText TextColumn2Row3 { get { return TextFields[5]; } }
        public INotificationContentText TextColumn1Row4 { get { return TextFields[6]; } }
        public INotificationContentText TextColumn2Row4 { get { return TextFields[7]; } }
        public INotificationContentText TextColumn1Row5 { get { return TextFields[8]; } }
        public INotificationContentText TextColumn2Row5 { get { return TextFields[9]; } }
    }

    internal class TileWideText07 : TileWideBase, ITileWideText07
    {
        public TileWideText07() : base(templateName: "TileWideText07", imageCount: 0, textCount: 9)
        {
        }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextShortColumn1Row1 { get { return TextFields[1]; } }
        public INotificationContentText TextColumn2Row1 { get { return TextFields[2]; } }
        public INotificationContentText TextShortColumn1Row2 { get { return TextFields[3]; } }
        public INotificationContentText TextColumn2Row2 { get { return TextFields[4]; } }
        public INotificationContentText TextShortColumn1Row3 { get { return TextFields[5]; } }
        public INotificationContentText TextColumn2Row3 { get { return TextFields[6]; } }
        public INotificationContentText TextShortColumn1Row4 { get { return TextFields[7]; } }
        public INotificationContentText TextColumn2Row4 { get { return TextFields[8]; } }
    }

    internal class TileWideText08 : TileWideBase, ITileWideText08
    {
        public TileWideText08() : base(templateName: "TileWideText08", imageCount: 0, textCount: 10)
        {
        }

        public INotificationContentText TextShortColumn1Row1 { get { return TextFields[0]; } }
        public INotificationContentText TextColumn2Row1 { get { return TextFields[1]; } }
        public INotificationContentText TextShortColumn1Row2 { get { return TextFields[2]; } }
        public INotificationContentText TextColumn2Row2 { get { return TextFields[3]; } }
        public INotificationContentText TextShortColumn1Row3 { get { return TextFields[4]; } }
        public INotificationContentText TextColumn2Row3 { get { return TextFields[5]; } }
        public INotificationContentText TextShortColumn1Row4 { get { return TextFields[6]; } }
        public INotificationContentText TextColumn2Row4 { get { return TextFields[7]; } }
        public INotificationContentText TextShortColumn1Row5 { get { return TextFields[8]; } }
        public INotificationContentText TextColumn2Row5 { get { return TextFields[9]; } }
    }

    internal class TileWideText09 : TileWideBase, ITileWideText09
    {
        public TileWideText09() : base(templateName: "TileWideText09", imageCount: 0, textCount: 2)
        {
        }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextBodyWrap { get { return TextFields[1]; } }
    }

    internal class TileWideText10 : TileWideBase, ITileWideText10
    {
        public TileWideText10() : base(templateName: "TileWideText10", imageCount: 0, textCount: 9)
        {
        }

        public INotificationContentText TextHeading { get { return TextFields[0]; } }
        public INotificationContentText TextPrefixColumn1Row1 { get { return TextFields[1]; } }
        public INotificationContentText TextColumn2Row1 { get { return TextFields[2]; } }
        public INotificationContentText TextPrefixColumn1Row2 { get { return TextFields[3]; } }
        public INotificationContentText TextColumn2Row2 { get { return TextFields[4]; } }
        public INotificationContentText TextPrefixColumn1Row3 { get { return TextFields[5]; } }
        public INotificationContentText TextColumn2Row3 { get { return TextFields[6]; } }
        public INotificationContentText TextPrefixColumn1Row4 { get { return TextFields[7]; } }
        public INotificationContentText TextColumn2Row4 { get { return TextFields[8]; } }
    }

    internal class TileWideText11 : TileWideBase, ITileWideText11
    {
        public TileWideText11() : base(templateName: "TileWideText11", imageCount: 0, textCount: 10)
        {
        }

        public INotificationContentText TextPrefixColumn1Row1 { get { return TextFields[0]; } }
        public INotificationContentText TextColumn2Row1 { get { return TextFields[1]; } }
        public INotificationContentText TextPrefixColumn1Row2 { get { return TextFields[2]; } }
        public INotificationContentText TextColumn2Row2 { get { return TextFields[3]; } }
        public INotificationContentText TextPrefixColumn1Row3 { get { return TextFields[4]; } }
        public INotificationContentText TextColumn2Row3 { get { return TextFields[5]; } }
        public INotificationContentText TextPrefixColumn1Row4 { get { return TextFields[6]; } }
        public INotificationContentText TextColumn2Row4 { get { return TextFields[7]; } }
        public INotificationContentText TextPrefixColumn1Row5 { get { return TextFields[8]; } }
        public INotificationContentText TextColumn2Row5 { get { return TextFields[9]; } }
    }

    /// <summary>
    /// A factory which creates tile content objects for all of the toast template types.
    /// </summary>
    public sealed class TileContentFactory
    {
        /// <summary>
        /// Creates a tile content object for the TileSquareBlock template.
        /// </summary>
        /// <returns>A tile content object for the TileSquareBlock template.</returns>
        public static ITileSquareBlock CreateTileSquareBlock()
        {
            return new TileSquareBlock();
        }

        /// <summary>
        /// Creates a tile content object for the TileSquareImage template.
        /// </summary>
        /// <returns>A tile content object for the TileSquareImage template.</returns>
        public static ITileSquareImage CreateTileSquareImage()
        {
            return new TileSquareImage();
        }

        /// <summary>
        /// Creates a tile content object for the TileSquarePeekImageAndText01 template.
        /// </summary>
        /// <returns>A tile content object for the TileSquarePeekImageAndText01 template.</returns>
        public static ITileSquarePeekImageAndText01 CreateTileSquarePeekImageAndText01()
        {
            return new TileSquarePeekImageAndText01();
        }

        /// <summary>
        /// Creates a tile content object for the TileSquarePeekImageAndText02 template.
        /// </summary>
        /// <returns>A tile content object for the TileSquarePeekImageAndText02 template.</returns>
        public static ITileSquarePeekImageAndText02 CreateTileSquarePeekImageAndText02()
        {
            return new TileSquarePeekImageAndText02();
        }

        /// <summary>
        /// Creates a tile content object for the TileSquarePeekImageAndText03 template.
        /// </summary>
        /// <returns>A tile content object for the TileSquarePeekImageAndText03 template.</returns>
        public static ITileSquarePeekImageAndText03 CreateTileSquarePeekImageAndText03()
        {
            return new TileSquarePeekImageAndText03();
        }

        /// <summary>
        /// Creates a tile content object for the TileSquarePeekImageAndText04 template.
        /// </summary>
        /// <returns>A tile content object for the TileSquarePeekImageAndText04 template.</returns>
        public static ITileSquarePeekImageAndText04 CreateTileSquarePeekImageAndText04()
        {
            return new TileSquarePeekImageAndText04();
        }

        /// <summary>
        /// Creates a tile content object for the TileSquareText01 template.
        /// </summary>
        /// <returns>A tile content object for the TileSquareText01 template.</returns>
        public static ITileSquareText01 CreateTileSquareText01()
        {
            return new TileSquareText01();
        }

        /// <summary>
        /// Creates a tile content object for the TileSquareText02 template.
        /// </summary>
        /// <returns>A tile content object for the TileSquareText02 template.</returns>
        public static ITileSquareText02 CreateTileSquareText02()
        {
            return new TileSquareText02();
        }

        /// <summary>
        /// Creates a tile content object for the TileSquareText03 template.
        /// </summary>
        /// <returns>A tile content object for the TileSquareText03 template.</returns>
        public static ITileSquareText03 CreateTileSquareText03()
        {
            return new TileSquareText03();
        }

        /// <summary>
        /// Creates a tile content object for the TileSquareText04 template.
        /// </summary>
        /// <returns>A tile content object for the TileSquareText04 template.</returns>
        public static ITileSquareText04 CreateTileSquareText04()
        {
            return new TileSquareText04();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideBlockAndText01 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideBlockAndText01 template.</returns>
        public static ITileWideBlockAndText01 CreateTileWideBlockAndText01()
        {
            return new TileWideBlockAndText01();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideBlockAndText02 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideBlockAndText02 template.</returns>
        public static ITileWideBlockAndText02 CreateTileWideBlockAndText02()
        {
            return new TileWideBlockAndText02();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideImage template.
        /// </summary>
        /// <returns>A tile content object for the TileWideImage template.</returns>
        public static ITileWideImage CreateTileWideImage()
        {
            return new TileWideImage();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideImageAndText01 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideImageAndText01 template.</returns>
        public static ITileWideImageAndText01 CreateTileWideImageAndText01()
        {
            return new TileWideImageAndText01();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideImageAndText02 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideImageAndText02 template.</returns>
        public static ITileWideImageAndText02 CreateTileWideImageAndText02()
        {
            return new TileWideImageAndText02();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideImageCollection template.
        /// </summary>
        /// <returns>A tile content object for the TileWideImageCollection template.</returns>
        public static ITileWideImageCollection CreateTileWideImageCollection()
        {
            return new TileWideImageCollection();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImage01 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImage01 template.</returns>
        public static ITileWidePeekImage01 CreateTileWidePeekImage01()
        {
            return new TileWidePeekImage01();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImage02 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImage02 template.</returns>
        public static ITileWidePeekImage02 CreateTileWidePeekImage02()
        {
            return new TileWidePeekImage02();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImage03 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImage03 template.</returns>
        public static ITileWidePeekImage03 CreateTileWidePeekImage03()
        {
            return new TileWidePeekImage03();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImage04 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImage04 template.</returns>
        public static ITileWidePeekImage04 CreateTileWidePeekImage04()
        {
            return new TileWidePeekImage04();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImage05 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImage05 template.</returns>
        public static ITileWidePeekImage05 CreateTileWidePeekImage05()
        {
            return new TileWidePeekImage05();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImage06 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImage06 template.</returns>
        public static ITileWidePeekImage06 CreateTileWidePeekImage06()
        {
            return new TileWidePeekImage06();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImageAndText01 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImageAndText01 template.</returns>
        public static ITileWidePeekImageAndText01 CreateTileWidePeekImageAndText01()
        {
            return new TileWidePeekImageAndText01();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImageAndText02 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImageAndText02 template.</returns>
        public static ITileWidePeekImageAndText02 CreateTileWidePeekImageAndText02()
        {
            return new TileWidePeekImageAndText02();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImageCollection01 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImageCollection01 template.</returns>
        public static ITileWidePeekImageCollection01 CreateTileWidePeekImageCollection01()
        {
            return new TileWidePeekImageCollection01();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImageCollection02 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImageCollection02 template.</returns>
        public static ITileWidePeekImageCollection02 CreateTileWidePeekImageCollection02()
        {
            return new TileWidePeekImageCollection02();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImageCollection03 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImageCollection03 template.</returns>
        public static ITileWidePeekImageCollection03 CreateTileWidePeekImageCollection03()
        {
            return new TileWidePeekImageCollection03();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImageCollection04 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImageCollection04 template.</returns>
        public static ITileWidePeekImageCollection04 CreateTileWidePeekImageCollection04()
        {
            return new TileWidePeekImageCollection04();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImageCollection05 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImageCollection05 template.</returns>
        public static ITileWidePeekImageCollection05 CreateTileWidePeekImageCollection05()
        {
            return new TileWidePeekImageCollection05();
        }

        /// <summary>
        /// Creates a tile content object for the TileWidePeekImageCollection06 template.
        /// </summary>
        /// <returns>A tile content object for the TileWidePeekImageCollection06 template.</returns>
        public static ITileWidePeekImageCollection06 CreateTileWidePeekImageCollection06()
        {
            return new TileWidePeekImageCollection06();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideSmallImageAndText01 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideSmallImageAndText01 template.</returns>
        public static ITileWideSmallImageAndText01 CreateTileWideSmallImageAndText01()
        {
            return new TileWideSmallImageAndText01();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideSmallImageAndText02 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideSmallImageAndText02 template.</returns>
        public static ITileWideSmallImageAndText02 CreateTileWideSmallImageAndText02()
        {
            return new TileWideSmallImageAndText02();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideSmallImageAndText03 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideSmallImageAndText03 template.</returns>

        public static ITileWideSmallImageAndText03 CreateTileWideSmallImageAndText03()
        {
            return new TileWideSmallImageAndText03();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideSmallImageAndText04 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideSmallImageAndText04 template.</returns>
        public static ITileWideSmallImageAndText04 CreateTileWideSmallImageAndText04()
        {
            return new TileWideSmallImageAndText04();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideSmallImageAndText05 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideSmallImageAndText05 template.</returns>
        public static ITileWideSmallImageAndText05 CreateTileWideSmallImageAndText05()
        {
            return new TileWideSmallImageAndText05();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText01 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText01 template.</returns>
        public static ITileWideText01 CreateTileWideText01()
        {
            return new TileWideText01();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText02 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText02 template.</returns>
        public static ITileWideText02 CreateTileWideText02()
        {
            return new TileWideText02();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText03 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText03 template.</returns>
        public static ITileWideText03 CreateTileWideText03()
        {
            return new TileWideText03();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText04 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText04 template.</returns>
        public static ITileWideText04 CreateTileWideText04()
        {
            return new TileWideText04();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText05 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText05 template.</returns>
        public static ITileWideText05 CreateTileWideText05()
        {
            return new TileWideText05();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText06 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText06 template.</returns>
        public static ITileWideText06 CreateTileWideText06()
        {
            return new TileWideText06();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText07 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText07 template.</returns>
        public static ITileWideText07 CreateTileWideText07()
        {
            return new TileWideText07();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText08 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText08 template.</returns>
        public static ITileWideText08 CreateTileWideText08()
        {
            return new TileWideText08();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText09 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText09 template.</returns>
        public static ITileWideText09 CreateTileWideText09()
        {
            return new TileWideText09();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText10 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText10 template.</returns>
        public static ITileWideText10 CreateTileWideText10()
        {
            return new TileWideText10();
        }

        /// <summary>
        /// Creates a tile content object for the TileWideText11 template.
        /// </summary>
        /// <returns>A tile content object for the TileWideText11 template.</returns>
        public static ITileWideText11 CreateTileWideText11()
        {
            return new TileWideText11();
        }
    }
}
