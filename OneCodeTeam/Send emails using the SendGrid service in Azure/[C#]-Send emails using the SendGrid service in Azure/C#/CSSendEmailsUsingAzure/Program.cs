/****************************** Module Header ******************************\
* Module Name: Program.cs
* Project:     CSSendEmailsUsingAzure
* Copyright (c) Microsoft Corporation.
* 
* This sample will show how to send emails using Azure and SendGrid 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Net;
using System.Net.Mail;
using SendGrid;
using System.Collections.Generic;

namespace CSSendEmailsUsingAzure
{
    class Program
    {
        static void Main(string[] args)
        {
            //Message properties, add your values below
            string fromProperty = "noreply@example.com";
            
            List<String> recipientsProperty = new List<String>
            {
                @"John Smith <john@example.com>",
                @"Jane Smith <jane@example.com>"
            };

            string subjectProperty = "Title of email";

            string HTMLContentProperty = "<p>Your email message in HTML format</p>";

            //SendGrid credentials
            string username = "Username";
            string password = "Password";

            //The email object
            var message = new SendGridMessage();

            message.From = new MailAddress(fromProperty);
            message.AddTo(recipientsProperty);
            message.Subject = subjectProperty;

            //Add the HTML and Text bodies
            message.Html = HTMLContentProperty;

            var credentials = new NetworkCredential(username, password);
            var transportWeb = new Web(credentials);
            transportWeb.DeliverAsync(message).Wait();
        }
    }
}
