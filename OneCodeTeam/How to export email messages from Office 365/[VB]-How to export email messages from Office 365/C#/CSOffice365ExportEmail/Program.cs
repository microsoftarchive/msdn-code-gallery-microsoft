/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSOffice365ExportEmail
Copyright (c) Microsoft Corporation.

In this sample, we will demonstrate how to export the emails form the office 
365.
If we export the emails, we can read them offline. We can follow these steps
to implement it:
1. Create a search directory to collect the emails;
2. Get the directory;
3. Export the emails;

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
using System.Linq;
using System.IO;

namespace CSOffice365ExportEmail
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

            String subjectString = InputSubjectString();

            // Use the EmailMessageSchema.Subject to filter the emails.
            Dictionary<PropertyDefinition, String> filters = new Dictionary<PropertyDefinition, string>();
            filters[EmailMessageSchema.Subject] = subjectString;

            String folderName = "Subject contains for export email";

            // Delete the duplicate folder.
            DeleteFolder(service, WellKnownFolderName.SearchFolders, folderName);

            // Create the search folder named "Subject contains" to get the emails that received in last 30 days
            CreateSearchFolder(service, filters, folderName);
            Console.WriteLine("Create the search folder.");
            Console.WriteLine();

            // Get the search folder.
            SearchFilter filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName);
            SearchFolder searchFolder = GetFolder(service, filter,
                WellKnownFolderName.SearchFolders) as SearchFolder;
            Console.WriteLine("Get the specific search folder.");
            Console.WriteLine();

            // Export the email messages to the application directory.
            Console.WriteLine("Begin to export the emails:");
            ExportEmailMessages(searchFolder, Environment.CurrentDirectory);
            Console.WriteLine();

            Console.WriteLine("Press any key to exit......");
            Console.ReadKey();
        }

        /// <summary>
        /// This method creates and sets the search folder.
        /// </summary>
        private static SearchFolder CreateSearchFolder(ExchangeService service,
            Dictionary<PropertyDefinition, String> filters, String displayName)
        {
            if (service == null)
            {
                return null;
            }

            SearchFilter.SearchFilterCollection filterCollection =
                new SearchFilter.SearchFilterCollection(LogicalOperator.And);

            // We only search the nearest 30 days emails.
            DateTime startDate = DateTime.Now.AddDays(-30);
            DateTime endDate = DateTime.Now;
            SearchFilter startDateFilter =
                new SearchFilter.IsGreaterThanOrEqualTo(EmailMessageSchema.DateTimeCreated, startDate);
            SearchFilter endDateFilter =
                new SearchFilter.IsLessThanOrEqualTo(EmailMessageSchema.DateTimeCreated, endDate);
            filterCollection.Add(startDateFilter);
            filterCollection.Add(endDateFilter);

            SearchFilter itemClassFilter = 
                new SearchFilter.IsEqualTo(EmailMessageSchema.ItemClass, "IPM.Note");
            filterCollection.Add(itemClassFilter);

            // Set the other filters.
            if (filters != null)
            {
                foreach (PropertyDefinition property in filters.Keys)
                {
                    SearchFilter searchFilter = 
                        new SearchFilter.ContainsSubstring(property, filters[property]);
                    filterCollection.Add(searchFilter);
                }
            }

            FolderId folderId = new FolderId(WellKnownFolderName.Inbox);

            Boolean isDuplicateFoler = true;
            SearchFilter duplicateFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, displayName);
            SearchFolder searchFolder =
                GetFolder(service, duplicateFilter, WellKnownFolderName.SearchFolders) as SearchFolder;

            // If there isn't the specific search folder, we create a new one.
            if (searchFolder == null)
            {
                searchFolder = new SearchFolder(service);
                isDuplicateFoler = false;
            }
            searchFolder.SearchParameters.RootFolderIds.Add(folderId);
            searchFolder.SearchParameters.Traversal = SearchFolderTraversal.Shallow;
            searchFolder.SearchParameters.SearchFilter = filterCollection;

            if (isDuplicateFoler)
            {
                searchFolder.Update();
            }
            else
            {
                searchFolder.DisplayName = displayName;

                searchFolder.Save(WellKnownFolderName.SearchFolders);
            }

            return searchFolder;
        }

        /// <summary>
        /// Export the eamil messges form the specific search folder.
        /// </summary>
        private static void ExportEmailMessages(SearchFolder searchFolder, String filePath)
        {
            if (searchFolder == null)
            {
                return;
            }

            String[] invalidStings = { "\\", ",", ":", "*", "?", "\"", "<", ">", "|" };

            PropertySet itemPorpertySet = new PropertySet(BasePropertySet.FirstClassProperties,
                EmailMessageSchema.MimeContent);

            const Int32 pageSize = 50;
            ItemView itemView = new ItemView(pageSize);

            FindItemsResults<Item> findResults = null;
            do
            {
                findResults = searchFolder.FindItems(itemView);

                foreach (Item item in findResults.Items)
                {
                    if (item is EmailMessage)
                    {
                        EmailMessage email = item as EmailMessage;
                        email.Load(itemPorpertySet);

                        Byte[] content = email.MimeContent.Content;
                        String fileName = email.Subject;

                        // Replace all the invaild strings.
                        foreach (String str in invalidStings)
                        {
                            fileName = fileName.Replace(str, "");
                        }

                        // Export the emails to the .eml files.
                        fileName = Path.Combine(filePath, fileName + ".eml");
                        File.WriteAllBytes(fileName, content);
                        Console.WriteLine("Export the email:{0}", email.Subject);
                    }
                }

                itemView.Offset += pageSize;
            } while (findResults.MoreAvailable);
        }

        private static String InputSubjectString()
        {
            Console.WriteLine("Please input the string that the email's subject contains to filter the emails:");
            String subjectString = Console.ReadLine();

            if (String.IsNullOrWhiteSpace(subjectString))
            {
                Console.WriteLine("Please input the vaild strings");
                subjectString = InputSubjectString();
            }

            return subjectString;
        }

        private static Folder GetFolder(ExchangeService service, SearchFilter filter,
            WellKnownFolderName folder)
        {
            if (service == null)
            {
                return null;
            }

            PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly);

            FolderView folderView = new FolderView(5);
            folderView.PropertySet = propertySet;

            FindFoldersResults searchResults = service.FindFolders(folder,
                filter, folderView);

            return searchResults.FirstOrDefault();
        }

        private static void DeleteFolder(ExchangeService service, WellKnownFolderName parentFolder,
            String folderName)
        {
            SearchFilter searchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName);

            Folder folder = GetFolder(service, searchFilter, parentFolder);

            if (folder != null)
            {
                Console.WriteLine("Delete the folder '{0}'", folderName);
                folder.Delete(DeleteMode.HardDelete);
                Console.WriteLine();
            }
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
