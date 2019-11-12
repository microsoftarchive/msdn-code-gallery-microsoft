/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSO365ExportGroup
Copyright (c) Microsoft Corporation.

Sometimes we need to export the distribution groups and their members, but 
Outlook Web App (OWA) doesn’t provide the function. In this application, we 
will demonstrate how to export the Distribution Groups and their members.
1. We get the members of the root group.
2. We export all the mailbox in the group.
3. We can choose to process the following up steps recursively for the nested 
groups.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CSO365ExportGroup
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback =
CallbackMethods.CertificateValidationCallBack;
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP2);

            // Get the information of the account.
            UserInfo user = new UserInfo();
            service.Credentials = new WebCredentials(user.Account, user.Pwd);

            // Set the url of server.
            if (!AutodiscoverUrl(service, user))
            {
                return;
            }
            Console.WriteLine();

            String groupAddress = GetGroupAddress();
            Console.WriteLine();
            String filePath = GetFilePath();
            Console.WriteLine();
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                Console.WriteLine(groupAddress);
                writer.WriteLine("\"{0}\",\"{1}\"", "DistributionGroupAddress", "MemberAddresss");

                ExportGroup(service, groupAddress, null, true, writer);
            }
            Console.WriteLine();

            Console.WriteLine("Press any key to exit......");
            Console.ReadKey();
        }

        /// <summary>
        /// We will export the members of the group.
        /// </summary>
        private static void ExportGroup(ExchangeService service, String groupAddress, String pad,
            Boolean isRecursive, StreamWriter writer)
        {
            ExpandGroupResults groupMembers = service.ExpandGroup(groupAddress);

            if (String.IsNullOrEmpty(pad))
            {
                pad = "";
            }

            // Add spaces for view
            pad += "   ";
            foreach (EmailAddress member in groupMembers)
            {
                // If we need recursion, and the member is group, we will process the method recursively.
                if (isRecursive & (member.MailboxType == MailboxType.ContactGroup ||
                    member.MailboxType == MailboxType.PublicGroup))
                {
                    Console.WriteLine(pad + "{0,-50}{1,-11}", member.Address, member.MailboxType);
                    ExportGroup(service, member, pad, isRecursive, writer);
                }
                else
                {
                    Console.WriteLine(pad + "{0,-50}{1,-11}", member.Address, member.MailboxType);
                    writer.WriteLine("\"{0}\",\"{1}\"", groupAddress, member.Address);
                }
            }
        }

        private static void ExportGroup(ExchangeService service, EmailAddress groupAddress, String pad,
            Boolean isRecursive, StreamWriter writer)
        {
            if (groupAddress.MailboxType == MailboxType.ContactGroup ||
                groupAddress.MailboxType == MailboxType.PublicGroup)
            {
                ExportGroup(service, groupAddress.Address, pad, isRecursive, writer);
            }
        }

        /// <summary>
        /// Check the file path 
        /// </summary>
        private static String GetFilePath()
        {
            do
            {
                Console.Write("Please input the file path:");

                String filePath = Console.ReadLine();
                String directoryPath = Path.GetDirectoryName(filePath);
                if (Directory.Exists(directoryPath))
                {
                    return filePath;
                }

                Console.WriteLine("The path is invaild.");
            } while (true);
        }

        /// <summary>
        /// Check the email address
        /// </summary>
        private static String GetGroupAddress()
        {
            String pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            Regex regex = new Regex(pattern);
            do
            {
                Console.Write("Please input the Distribution Group Address:");

                String address = Console.ReadLine();
                if (regex.IsMatch(address))
                {
                    return address;
                }

                Console.WriteLine("The Email address is invaild.");
            } while (true);
        }

        private static Boolean AutodiscoverUrl(ExchangeService service, UserInfo user)
        {
            Boolean isSuccess = false;

            try
            {
                Console.WriteLine("Connecting the Exchange Online......");
                service.AutodiscoverUrl(user.Account, CallbackMethods.RedirectionUrlValidationCallback);
                Console.WriteLine();
                Console.WriteLine("Connected the Exchange Online successfully.");

                isSuccess = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("There's an error.");
                Console.WriteLine(e.Message);
            }

            return isSuccess;
        }
    }
}
