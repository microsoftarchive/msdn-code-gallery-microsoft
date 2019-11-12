/****************************** Module Header ******************************\
* Module Name:    Program.cs
* Project:        CSEmailScheduler
* Copyright (c) Microsoft Corporation
*
* The project shows you how to send email at particular date and time.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;

namespace CSEmailScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            var emailSender = new EmailSender();
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var mailSettings = config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;

            // Create a Mail Message
            var mailMessage = new MailMessage();

            // Receiver’s E-Mail address. 
            mailMessage.To.Add("pathakajay@live.com");
            
            // Subject of Email
            mailMessage.Subject = "Send Email OneCode Sample"; 

            mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            mailMessage.Body = "This is a test mail comes from OneCode Team."; // Message Body
            mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            mailMessage.IsBodyHtml = true;
            mailMessage.Priority = MailPriority.Normal; // Email priority
            mailMessage.From = new MailAddress(mailSettings.Smtp.From, "OneCode", System.Text.Encoding.UTF8);

            var mailMessages = new List<MailMessage>();
            mailMessages.Add(mailMessage);

            // Send email in Synchronous manner
            emailSender.SendEmail(mailMessages);

            // Send Email Asynchronously
            emailSender.SendEmailAsync(mailMessages);

            // Remove this line, after adding CSEmailScheduler.exe to Windows Task Scheduler
            Console.ReadLine(); 

        }
    }
}
