// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SceReader.Converters;
using Microsoft.SceReader.Data;
using System.Windows.Resources;
using System.IO;
using System.Windows;

namespace MsdnReader
{
    /// <summary>
    /// Converter for formatting emails subject and body text
    /// </summary>
    public class MsdnEmailConverter : EmailConverter
    {
        public override string GetStoryEmailSubjectText(Story story)
        {
            string titleText = String.Empty;
            if (story != null)
            {
                titleText = MsdnReaderSettings.ApplicationName + ": " + story.Title; 
            }
            return titleText;
        }

        public override string GetStoryEmailBodyText(Story story, IList<string> attachments, bool htmlSupported)
        {
            string bodyText = String.Empty;
            if (htmlSupported)
            {
                // Attachment file path may be used as image source in HTML mail, e.g. ink note image
                bodyText = GetHtmlBodyText(story, attachments);
            }
            else
            {
                bodyText = GetSimpleBodyText(story);
            }
            return bodyText;
        }

        /// <summary>
        /// Get HTML or plain text mail summary for story
        /// </summary>
        public override string GetStoryEmailSummaryText(Story story, bool htmlSupported)
        {
            string text = string.Empty;
            StreamResourceInfo streamInfo = Application.GetResourceStream(htmlSupported ? _htmlMailSummaryTemplate : _simpleMailTemplate);
            if (streamInfo != null && story != null)
            {
                StreamReader reader = new StreamReader(streamInfo.Stream);
                text = reader.ReadToEnd();
                text = text.Replace("%ArticleTitle%", story.Title);
                text = text.Replace("%ArticleDesc%", story.Description);
                text = text.Replace("%WebLink%", story.WebLink);
            }
            return text;
        }
        
        /// <summary>
        /// Get simple body text string
        /// </summary>
        private string GetSimpleBodyText(Story story)
        {
            string text = string.Empty;
            StreamResourceInfo streamInfo = Application.GetResourceStream(_simpleMailTemplate);
            if (streamInfo != null && story != null)
            {
                StreamReader reader = new StreamReader(streamInfo.Stream);
                text = reader.ReadToEnd();
                text = text.Replace("%ArticleTitle%", story.Title);
                text = text.Replace("%ArticleDesc%", story.Description);
                text = text.Replace("%WebLink%", story.WebLink);
            }
            return text;
        }

        /// <summary>
        /// Get HTML formatted body text
        /// </summary>
        private string GetHtmlBodyText(Story story, IList<string> attachments)
        {
            string text = string.Empty;
            StreamResourceInfo streamInfo = Application.GetResourceStream(_htmlMailTemplate);
            if (streamInfo != null && story != null)
            {
                StreamReader reader = new StreamReader(streamInfo.Stream);
                text = reader.ReadToEnd();
                text = text.Replace("%ArticleTitle%", story.Title);
                text = text.Replace("%ArticleDesc%", story.Description);
                text = text.Replace("%WebLink%", story.WebLink);
            }
            return text;
        }

        private static Uri _htmlMailTemplate = new Uri("pack://application:,,,/Resources/MailTemplates/HTMLMailTemplate.txt");
        private static Uri _simpleMailTemplate = new Uri("pack://application:,,,/Resources/MailTemplates/SimpleMailTemplate.txt");
        private static Uri _htmlMailSummaryTemplate = new Uri("pack://application:,,,/Resources/MailTemplates/HTMLMailSummaryTemplate.txt");
    }
}