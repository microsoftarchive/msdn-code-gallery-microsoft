/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSO365ExportContacts
Copyright (c) Microsoft Corporation.

Outlook Web App (OWA) allows us to import multiple contacts in a very simple 
way. However, it does not allow to export contacts. In this application, we 
will demonstrate how to export contacts from Office 365 Exchange Online.
1. Get all the contacts from Office 365 Exchange Online.
2. Write the head title to the CSV file.
3. Write the contacts into the CSV file.

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

namespace CSO365ExportContacts
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

            ExportContacts(service);
            Console.WriteLine();

            Console.WriteLine("Press any key to exit......");
            Console.ReadKey();
        }

        /// <summary>
        /// Export all contacts into the CSV file from Office 365 Exchange Online.
        /// </summary>
        private static void ExportContacts(ExchangeService service)
        {
            // Get the properties we need to write.
            PropertySet propertySet = new PropertySet();
            Dictionary<PropertyDefinitionBase, String> schemaList = 
                ContactsHelper.GetSchemaList();
            propertySet.AddRange(schemaList.Keys);

            List<Item> results = GetItems(service, null, WellKnownFolderName.Contacts, 
                propertySet);
            String path = GetFolderPath();
            String filePath = Path.Combine(path, "contacts.csv");

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                Boolean firstCell = true;

                // Write the head title
                foreach (PropertyDefinitionBase head in schemaList.Keys)
                {
                    if (!firstCell)
                    {
                        writer.Write(",");
                    }
                    else
                    {
                        firstCell = false;
                    }

                    writer.Write("\"{0}\"",schemaList[head]);
                }
                writer.WriteLine();
                firstCell = true;

                // Write the contact.
                foreach (Item item in results)
                {
                    Contact contact = item as Contact;

                    foreach (PropertyDefinitionBase proerty in schemaList.Keys)
                    {
                        if (!firstCell)
                        {
                            writer.Write(",");
                        }
                        else
                        {
                            firstCell = false;
                        }

                        ContactsHelper.WriteContacts(writer, proerty, contact);
                    }

                    writer.WriteLine();
                    firstCell = true;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Export the contacts to the file:{0}",filePath); 
        }

        /// <summary>
        /// Ask the path of fodler that stores the CSV file. 
        /// </summary>
        /// <returns>return the folder path</returns>
        private static String GetFolderPath()
        {
            do
            {
                Console.Write("Please input the floder path:");

                String path = Console.ReadLine();
                List<String> files = new List<String>();
                if (Directory.Exists(path))
                {
                    return path;
                }

                Console.WriteLine("The path is invaild.");
            } while (true);
        }

        private static List<Item> GetItems(ExchangeService service, SearchFilter filter,
WellKnownFolderName folder, PropertySet propertySet)
        {
            if (service == null)
            {
                return null;
            }

            List<Item> items = new List<Item>();

            if (propertySet == null)
            {
                propertySet = new PropertySet(BasePropertySet.IdOnly);
            }

            const Int32 pageSize = 10;
            ItemView itemView = new ItemView(pageSize);
            itemView.PropertySet = propertySet;

            FindItemsResults<Item> searchResults = null;
            do
            {
                searchResults = service.FindItems(folder,
                    filter, itemView);
                items.AddRange(searchResults.Items);

                itemView.Offset += pageSize;
            } while (searchResults.MoreAvailable);

            return items;
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
