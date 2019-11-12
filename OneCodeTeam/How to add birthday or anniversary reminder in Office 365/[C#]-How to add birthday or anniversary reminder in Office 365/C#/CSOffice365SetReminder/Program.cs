/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSOffice365SetReminder
Copyright (c) Microsoft Corporation.

In this sample, we will demonstrate how to add birthday or anniversary 
reminder for the contacts.
We can search a contact folder to find the contacts that have birthdays or
anniversaries, and then create the reminders for them. Additionally, we can 
import a comma-separated values(CSV) file for creating the reminders.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.WebServices.Data;
using System.Collections.Generic;

namespace CSOffice365SetReminder
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


            #region Create Reminders for the contacts folder
            Folder contactsFolder = ReminderHelper.GetContactsFolder(service, @"\");
            Console.WriteLine("It's success to get the contact folder.");
            Console.WriteLine();

            List<Contact> contactList = ReminderHelper.GetContactsByBirthday(contactsFolder, null);
            Console.WriteLine("Get the contacts that have the Birthday.");
            // The user can specify any TimeZone to set the reminder.
            ReminderHelper.SetReminder(service, contactList,TimeZoneInfo.Local);
            Console.WriteLine();

            contactList = ReminderHelper.GetContactsByAnniversary(contactsFolder, null);
            Console.WriteLine("Get the contacts that have the Anniversary.");
            // The user can specify any TimeZone to set the reminder.
            ReminderHelper.SetReminder(service, contactList,TimeZoneInfo.Local);
            Console.WriteLine(); 
            #endregion

            #region Create Reminders for the CSV file.
            String filePath = "contacts365.csv";

            // We first import the CSV file and get the contacts.
            List<Contact> contactListFromCSV = ReminderHelper.ImportContactsFromCSV(service, filePath);
            Console.WriteLine("It's success to import the CSV file that has the contacts.");
            // Then we set the reminders for the contacts.
            // The user can specify any TimeZone to set the reminder.
            ReminderHelper.SetReminder(service, contactListFromCSV, TimeZoneInfo.Local);
            Console.WriteLine(); 
            #endregion

            Console.WriteLine("Press any key to exit......");
            Console.ReadKey();
        }

        private static Boolean AutodiscoverUrl(ExchangeService service, UserInfo user)
        {
            Boolean isSuccess = false;

            try
            {
                Console.WriteLine("Connecting the Exchange Online......");
                service.AutodiscoverUrl(user.Account, CallbackMethods.RedirectionUrlValidationCallback);
                Console.WriteLine();
                Console.WriteLine("It's success to connect the Exchange Online.");

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
