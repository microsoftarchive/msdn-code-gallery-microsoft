/****************************** Module Header ******************************\
* Module Name:    EmailSender.cs
* Project:        CSCheckFileInUse
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
    public class EmailSender
    {
        /// <summary>
        /// Send Email to a list of recipients. 
        /// </summary>
        /// <param name="recipientList">A List of MailMessage object, that contains the list of Email Message</param>
        /// <returns>Returns True, if e-mail is sent successfully otherwise false</returns>
        public bool SendEmail(List<MailMessage> recipientList)
        {
            SmtpClient smtpClient = null;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var mailSettings = config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;
            bool status = false;
            try
            {
                // SMTP settings are defined in app.config file
                smtpClient = new SmtpClient();
                foreach (MailMessage mailMessage in recipientList)
                {
                    smtpClient.Send(mailMessage);
                }

                status = true;
            }
            catch (Exception e)
            {
                string errorMessage = string.Empty;
                while (e != null)
                {
                    errorMessage += e.ToString();
                    e = e.InnerException;
                }
                status = false;

            }
            finally
            {
                if (smtpClient != null)
                {
                    smtpClient.Dispose();
                }
            }
            return status;
        }

        /// <summary>
        /// Asynchronously send Email to a list of recipients. 
        /// </summary>
        /// <param name="recipientList">A List of MailMessage object, that contains the list of Email Message</param>
        public void SendEmailAsync(List<MailMessage> recipientList)
        {
            SmtpClient smtpClient = null;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var mailSettings = config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;
            object userState = null;
            try
            {
                // SMTP settings are defined in app.config file
                smtpClient = new SmtpClient();
                foreach (MailMessage mailMessage in recipientList)
                {
                    userState = mailMessage;
                    smtpClient.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
                    smtpClient.SendAsync(mailMessage, userState);
                }
            }
            catch (Exception e)
            {
                string errorMessage = string.Empty;
                while (e != null)
                {
                    errorMessage += e.ToString();
                    e = e.InnerException;
                }
            }
            finally
            {
                if (smtpClient != null)
                {
                    smtpClient.Dispose();
                }
            }
        }

        void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // Get the Original MailMessage object
            MailMessage mailMessage = (MailMessage)e.UserState;
            string subject = mailMessage.Subject;

            // Write custom logging code here. Currently it is showing error on console.
            if (e.Cancelled)
            {
                Console.WriteLine("Send canceled for [{0}] with subject [{1}] at [{2}].", mailMessage.To, subject, DateTime.Now.ToString());
            }
            if (e.Error != null)
            {
                Console.WriteLine("An error {1} occurred when sending mail [{0}] to [{2}] at [{3}] ", subject, e.Error.ToString(), mailMessage.To, DateTime.Now.ToString());
            }
            else
            {
                Console.WriteLine("Message [{0}] is sent to [{1}] at [{2}].", subject, mailMessage.To, DateTime.Now.ToString());
            }
        }
    }
}
