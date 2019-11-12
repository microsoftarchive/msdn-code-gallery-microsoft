/****************************** Module Header ******************************\
Module Name:  Program.cs
Project:      CSO365ImportVCardFiles
Copyright (c) Microsoft Corporation.

The vCard file format is supported by many email clients and email services. 
Now Outlook Web App supports to import the single .CSV file only. In this 
application, we will demonstrate how to import multiple vCard files in 
Office 365 Exchange Online.
1. Get a single file or all the vCard files in the folder;
2. Read the contact information from the vCard file;
3. Create a new contact and set the properties;
4. Save the contact
5. Process 2-4 steps for all the vCard files.

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

namespace CSO365ImportVCardFiles
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

            List<String> files = GetFiles();
            Console.WriteLine();

            CreateContacts(service, files);
            Console.WriteLine();

            Console.WriteLine("Press any key to exit......");
            Console.ReadKey();
        }


        /// <summary>
        /// Get the vCard file or all the vCard file in the folder
        /// </summary>
        /// <returns></returns>
        private static List<String> GetFiles()
        {
            Console.WriteLine("Input the file/floder path:");

            String path = Console.ReadLine();
            List<String> files = new List<String>();
            if (Directory.Exists(path))
            {
                files.AddRange(Directory.GetFiles(path, "*.vcf"));
                Console.WriteLine("Get files.");
            }
            else
            {
                if (File.Exists(path) && path.ToLower().EndsWith(".vcf"))
                {
                    files.Add(path);
                    Console.WriteLine("Get the file.");
                }
            }

            return files;
        }

        /// <summary>
        /// Read the contact information from vCard files and creat a new contact in Exchange Online.
        /// </summary>
        private static void CreateContacts(ExchangeService service, List<String> files)
        {
            foreach (String file in files)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    String line;
                    // Store the contact information
                    Dictionary<ContactSchemaProperties, String> contactInfo = new Dictionary<ContactSchemaProperties, String>();
                    Boolean isSupport = true;
                    do
                    {
                        line = reader.ReadLine();

                        if (String.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        Int32 firstColonIndex = line.IndexOf(":");

                        if (firstColonIndex < 0)
                        {
                            continue;
                        }

                        String keyName = null, keyValue = null;
                       
                        // Read the contact information
                        if (line.StartsWith("N:") || line.StartsWith("N;LANGUAGE"))
                        {
                            keyName = "Names";
                            keyValue = line.Substring(firstColonIndex + 1);
                        }
                        else
                        {
                            keyName = line.Substring(0, firstColonIndex);
                            keyValue = line.Substring(firstColonIndex + 1);
                        }

                        if (keyName.StartsWith("VERSION") && !keyValue.StartsWith("2.1"))
                        {
                            isSupport = false;
                            Console.WriteLine("This application only supports VCard Version 2.1 files.");
                            break;
                        }

                        ImportContactDetail(contactInfo, keyName, keyValue, reader);

                    } while (line != null);

                    if (isSupport)
                    {
                        CreateContact(service, contactInfo);
                        Console.WriteLine("Import the contact {0}.", file);
                    }
                }
            }
        }

        /// <summary>
        /// Read the information and store it.
        /// </summary>
        private static void ImportContactDetail(Dictionary<ContactSchemaProperties, String> contactInfo, String keyName, String keyValue, StreamReader reader)
        {
            if (keyName.StartsWith("Names"))
            {
                String[] names = keyValue.Split(';');
                if (names.Length >= 2)
                {
                    contactInfo.Add(ContactSchemaProperties.Surname, names[0]);
                    contactInfo.Add(ContactSchemaProperties.GivenName, names[1]);
                }
            }
            else if (keyName.StartsWith("FN"))
            {
                contactInfo.Add(ContactSchemaProperties.DisplayName, keyValue);
            }
            else if (keyName.StartsWith("ORG"))
            {
                String[] comDep = keyValue.Split(';');

                contactInfo.Add(ContactSchemaProperties.CompanyName, comDep[0]);
                contactInfo.Add(ContactSchemaProperties.Companies, comDep[0]);
                contactInfo.Add(ContactSchemaProperties.Department, comDep[1]);
            }
            else if (keyName.StartsWith("TITLE"))
            {
                contactInfo.Add(ContactSchemaProperties.JobTitle, keyValue);
            }
            else if (keyName.StartsWith("PHOTO"))
            {
                ImportContactDetails.ImportPhoto(contactInfo, keyName, reader);
            }
            else if (keyName.StartsWith("TEL"))
            {
                ImportContactDetails.ImportTelephone(contactInfo, keyName, keyValue);
            }
            else if (keyName.StartsWith("ADR"))
            {
                ImportContactDetails.ImportAddress(contactInfo, keyName, keyValue);
            }
            else if (keyName.StartsWith("EMAIL"))
            {
                ImportContactDetails.ImportEmail(contactInfo, keyName, keyValue);
            }
        }

        /// <summary>
        /// Create a new contact and save it.
        /// </summary>
        private static void CreateContact(ExchangeService service, Dictionary<ContactSchemaProperties, String> contactInfo)
        {
            Contact newContact = new Contact(service);
            PhysicalAddressEntry businessAddressEntry = null;
            PhysicalAddressEntry homeAddressEntry = null;
            PhysicalAddressEntry otherAddressEntry = null;

            foreach (ContactSchemaProperties key in contactInfo.Keys)
            {
                switch (key)
                {
                    case ContactSchemaProperties.Surname:
                        newContact.Surname = contactInfo[key];
                        break;
                    case ContactSchemaProperties.GivenName:
                        newContact.GivenName = contactInfo[key];
                        break;
                    case ContactSchemaProperties.DisplayName:
                        newContact.DisplayName = contactInfo[key];
                        break;
                    case ContactSchemaProperties.JobTitle:
                        newContact.JobTitle = contactInfo[key];
                        break;
                    case ContactSchemaProperties.Birthday:
                        {
                            DateTime birthday;
                            newContact.Birthday = DateTime.TryParse(contactInfo[key], out birthday) ? (DateTime?)birthday : null;
                        }
                        break;
                    case ContactSchemaProperties.CompanyName:
                        newContact.CompanyName = contactInfo[key];
                        break;
                    case ContactSchemaProperties.Companies:
                        {
                            StringList stringList = new StringList();
                            stringList.Add(contactInfo[key]);
                            newContact.Companies = stringList;
                        }
                        break;
                    case ContactSchemaProperties.Department:
                        newContact.Department = contactInfo[key];
                        break;
                    case ContactSchemaProperties.EmailAddress1:
                        newContact.EmailAddresses[EmailAddressKey.EmailAddress1] = contactInfo[key];
                        break;
                    case ContactSchemaProperties.EmailAddress2:
                        newContact.EmailAddresses[EmailAddressKey.EmailAddress2] = contactInfo[key];
                        break;
                    case ContactSchemaProperties.EmailAddress3:
                        newContact.EmailAddresses[EmailAddressKey.EmailAddress3] = contactInfo[key];
                        break;
                    case ContactSchemaProperties.BusinessAddressStreet:
                    case ContactSchemaProperties.BusinessAddressCity:
                    case ContactSchemaProperties.BusinessAddressState:
                    case ContactSchemaProperties.BusinessAddressPostalCode:
                    case ContactSchemaProperties.BusinessAddressCountryOrRegion:
                        {
                            if (businessAddressEntry == null)
                            {
                                businessAddressEntry = new PhysicalAddressEntry();
                            }

                            SetContactDetails.SetAddress(key, contactInfo[key], businessAddressEntry);
                        }
                        break;
                    case ContactSchemaProperties.HomeAddressStreet:
                    case ContactSchemaProperties.HomeAddressCity:
                    case ContactSchemaProperties.HomeAddressState:
                    case ContactSchemaProperties.HomeAddressPostalCode:
                    case ContactSchemaProperties.HomeAddressCountryOrRegion:
                        {
                            if (homeAddressEntry == null)
                            {
                                homeAddressEntry = new PhysicalAddressEntry();
                            }
                            SetContactDetails.SetAddress(key, contactInfo[key], homeAddressEntry);
                        }
                        break;
                    case ContactSchemaProperties.OtherAddressStreet:
                    case ContactSchemaProperties.OtherAddressCity:
                    case ContactSchemaProperties.OtherAddressState:
                    case ContactSchemaProperties.OtherAddressPostalCode:
                    case ContactSchemaProperties.OtherAddressCountryOrRegion:
                        {
                            if (otherAddressEntry == null)
                            {
                                otherAddressEntry = new PhysicalAddressEntry();
                            }
                            SetContactDetails.SetAddress(key, contactInfo[key], otherAddressEntry);
                        }
                        break;
                    case ContactSchemaProperties.BusinessPhone:
                        newContact.PhoneNumbers[PhoneNumberKey.BusinessPhone] = contactInfo[key];
                        break;
                    case ContactSchemaProperties.BusinessPhone2:
                        newContact.PhoneNumbers[PhoneNumberKey.BusinessPhone2] = contactInfo[key];
                        break;
                    case ContactSchemaProperties.HomePhone:
                        newContact.PhoneNumbers[PhoneNumberKey.HomePhone] = contactInfo[key];
                        break;
                    case ContactSchemaProperties.HomePhone2:
                        newContact.PhoneNumbers[PhoneNumberKey.HomePhone2] = contactInfo[key];
                        break;
                    case ContactSchemaProperties.MobilePhone:
                        newContact.PhoneNumbers[PhoneNumberKey.MobilePhone] = contactInfo[key];
                        break;
                    case ContactSchemaProperties.Photo:
                        {
                            Byte[] picture = Convert.FromBase64String(contactInfo[key]);
                            newContact.SetContactPicture(picture);
                        }
                        break;
                    default:
                        break;
                }
            }

            // We'll add the addresses if they exist.
            if (businessAddressEntry != null)
            {
                newContact.PhysicalAddresses[PhysicalAddressKey.Business] = businessAddressEntry;
            }
            if (homeAddressEntry != null)
            {
                newContact.PhysicalAddresses[PhysicalAddressKey.Home] = homeAddressEntry;
            }
            if (otherAddressEntry != null)
            {
                newContact.PhysicalAddresses[PhysicalAddressKey.Other] = otherAddressEntry;
            }

            newContact.FileAsMapping = FileAsMapping.GivenNameSpaceSurname;

            newContact.Save(WellKnownFolderName.Contacts);
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
