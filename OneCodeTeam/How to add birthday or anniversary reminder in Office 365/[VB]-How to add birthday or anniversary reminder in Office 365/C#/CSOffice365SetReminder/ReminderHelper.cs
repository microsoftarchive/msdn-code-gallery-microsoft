/****************************** Module Header ******************************\
Module Name:  ReminderHelper.cs
Project:      CSOffice365SetReminder
Copyright (c) Microsoft Corporation.

In this sample, we will demonstrate how to add birthday or anniversary 
reminder for the contacts.
This file contains all the methods that set the reminders for the contacts.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using System.IO;
using System.Data;
using System.Globalization;
using System.Collections.ObjectModel;

namespace CSOffice365SetReminder
{
    public static class ReminderHelper
    {
        /// <summary>
        /// This method creates the birthday or anniversary reminders for the contacts.
        /// </summary>
        public static void SetReminder(ExchangeService service, List<Contact> contactList,TimeZoneInfo timeZone)
        {
            if (service == null || contactList == null)
            {
                return;
            }

            foreach (Contact contact in contactList)
            {
                Collection<PropertyDefinitionBase> properties=contact.GetLoadedPropertyDefinitions();

                // If the contact gets the PropertyDefinition of Birthday, the methode will create the 
                // birthday appointment for the contact.
                if (properties.Contains(ContactSchema.Birthday))
                {
                    if (CreateBirthdayAppointment(service, contact, timeZone))
                    {
                        Console.WriteLine("It's success to create the birthday appointment of " 
                            + contact.DisplayName);
                    }
                    else
                    {
                        Console.WriteLine("It's failed to create the birthday appointment of " 
                            + contact.DisplayName);
                    }
                }

                // If the contact gets the PropertyDefinition of WeddingAnniversary, the methode will 
                // create the anniversary appointment for the contact.
                if (properties.Contains(ContactSchema.WeddingAnniversary))
                {
                    if (CreateAnniversaryAppointment(service, contact, timeZone))
                    {
                        Console.WriteLine("It's success to create the anniversary appointment of " 
                            + contact.DisplayName);
                    }
                    else
                    {
                        Console.WriteLine("It's failed to create the anniversary appointment of " 
                            + contact.DisplayName);
                    }
                }
            }
        }

        /// <summary>
        /// This method gets the contacts folder basing the folder path.
        /// </summary>
        public static Folder GetContactsFolder(ExchangeService service, String path)
        {
            if (service == null || path == null)
            {
                return null;
            }

            Folder contactsFolder = null;

            // Searching the folder starts from the Contacts folder.
            SearchFilter.RelationalFilter searchFilter =
new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "Contacts");
            contactsFolder = GetFolder(service, searchFilter);

            path = path.TrimStart('\\').TrimEnd('\\');

            if (String.IsNullOrWhiteSpace(path))
            {
                return contactsFolder;
            }
            else
            {
                String[] pathList = path.Split('\\');
                const Int32 pageSize = 10;

                foreach (String name in pathList)
                {
                    searchFilter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, name);

                    FolderView folderView = new FolderView(pageSize);
                    PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly);
                    folderView.PropertySet = propertySet;

                    FindFoldersResults folderResults = null;
                    do
                    {
                        folderResults = contactsFolder.FindFolders(searchFilter, folderView);
                        folderView.Offset += pageSize;

                        // If the folder we find is the part of the parth, we will set the folder
                        // as parent folder and search the next node in it.
                        if (folderResults.TotalCount == 1)
                        {
                            contactsFolder = folderResults.Folders[0];
                        }

                    } while (folderResults.MoreAvailable);
                }
            }

            return contactsFolder;
        }

        /// <summary>
        /// This method gets the contacts that have the Birthday property definition in the 
        /// special folder.
        /// </summary>
        public static List<Contact> GetContactsByBirthday(Folder contactsFolder, String name)
        {
            if (contactsFolder == null)
            {
                return null;
            }

            return GetContacts(contactsFolder, name, ContactSchema.Birthday);
        }

        /// <summary>
        /// This method gets the contacts that have the WeddingAnniversary property definition  
        /// in the special folder.
        /// </summary>
        public static List<Contact> GetContactsByAnniversary(Folder contactsFolder, String name)
        {
            if (contactsFolder == null)
            {
                return null;
            }

            return GetContacts(contactsFolder, name, ContactSchema.WeddingAnniversary);
        }

        /// <summary>
        /// This method gets the contacts that have the Birthday and WeddingAnniversary property   
        /// definition in the special folder.
        /// </summary>
        public static List<Contact> GetContactsByBirthdayAndAnniversary(Folder contactsFolder, String name)
        {
            if (contactsFolder == null)
            {
                return null;
            }

            return GetContacts(contactsFolder, name, ContactSchema.Birthday, ContactSchema.WeddingAnniversary);
        }

        /// <summary>
        /// This method gets the contacts that have the special DisplayName and property  
        /// definitions in the special folder.
        /// </summary>
        private static List<Contact> GetContacts(Folder contactsFolder, String name, 
            params PropertyDefinition[] schemas)
        {
            if (contactsFolder == null)
            {
                return null;
            }

            List<Contact> contacts = new List<Contact>();

            SearchFilter.SearchFilterCollection filters = 
                new SearchFilter.SearchFilterCollection(LogicalOperator.And);

            if (!String.IsNullOrWhiteSpace(name))
            {
                SearchFilter searchFilter = new SearchFilter.ContainsSubstring(ContactSchema.DisplayName, name);
                filters.Add(searchFilter);
            }

            if (schemas != null)
            {
                foreach (PropertyDefinition schema in schemas)
                {
                    SearchFilter searchFilter = new SearchFilter.Exists(schema);
                    filters.Add(searchFilter);
                }
            }

            const Int32 pageSize = 10;
            ItemView itemView = new ItemView(pageSize);
            PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly, schemas);
            propertySet.Add(ContactSchema.DisplayName);
            itemView.PropertySet = propertySet;

            FindItemsResults<Item> findResults = null;
            do
            {
                findResults = contactsFolder.FindItems(filters, itemView);
                itemView.Offset += pageSize;

                contacts.AddRange(findResults.Cast<Contact>());
            } while (findResults.MoreAvailable);


            return contacts;
        }

        /// <summary>
        /// Create the birthday appointment for the contact.
        /// </summary>
        private static Boolean CreateBirthdayAppointment(ExchangeService service, Contact contact,TimeZoneInfo timeZone)
        {
            if (service == null || contact == null)
            {
                return false;
            }

            String subject = "It's the birthday of";
            return CreateAppointment(service, contact, subject, timeZone,c => c.Birthday);
        }

        /// <summary>
        /// Create the anniversary appointment for the contact.
        /// </summary>
        private static Boolean CreateAnniversaryAppointment(ExchangeService service, Contact contact,TimeZoneInfo timeZone)
        {
            if (service == null || contact == null)
            {
                return false;
            }

            String subject = "It's the anniversary of";
            return CreateAppointment(service, contact, subject,timeZone, c => c.WeddingAnniversary);
        }

        /// <summary>
        /// Create the appointment for the contact.
        /// </summary>
        private static Boolean CreateAppointment(ExchangeService service, Contact contact, 
            String subject, TimeZoneInfo timeZone,Func<Contact, DateTime?> getDate)
        {
            if (service == null || contact == null||getDate==null)
            {
                return false;
            }

            DateTime? date = getDate(contact);

            if (date == null)
            {
                return false;
            }

            String appointmentSubject = subject + " " + contact.DisplayName;

            // Check if there's the duplicate appointment.
            if (HaveDuplicateAppointment(service, appointmentSubject))
            {
                Console.WriteLine("There's a duplicate appointment of " + contact.DisplayName);
                return false;
            }

            Appointment appointment = new Appointment(service);
            appointment.Subject = appointmentSubject;
            appointment.LegacyFreeBusyStatus = LegacyFreeBusyStatus.Free;
            appointment.IsAllDayEvent = true;
            appointment.StartTimeZone = timeZone;
            appointment.EndTimeZone = timeZone;

            Recurrence recurrence = new Recurrence.YearlyPattern(date.Value, 
                (Month)date.Value.Month, date.Value.Day);
            appointment.Recurrence = recurrence;

            appointment.Save(SendInvitationsMode.SendToNone);

            return true;
        }

        /// <summary>
        /// Import the contacts from the CSV file.
        /// </summary>
        public static List<Contact> ImportContactsFromCSV(ExchangeService service, String filepath)
        {
            if (service == null || String.IsNullOrWhiteSpace(filepath))
            {
                return null;
            }

            List<Contact> contactsList = new List<Contact>();

            // Get the DataTable that contains the value of the contacts.
            DataTable contactsTable = new DataTable();
            ImportCSVFile(contactsTable, filepath);

            var properties = new { 
                FirstName = "First Name", 
                LastName = "Last Name", 
                Anniversary = "Anniversary", 
                Birthday = "Birthday" };

            foreach (DataRow row in contactsTable.Rows)
            {
                Contact contact = new Contact(service);

                if (contactsTable.Columns.Contains(properties.FirstName))
                {
                    contact.GivenName = row[properties.FirstName].ToString();
                    contact.DisplayName = contact.GivenName;
                    contact.FileAs = contact.DisplayName;
                }

                if (contactsTable.Columns.Contains(properties.LastName))
                {
                    contact.Surname = row[properties.LastName].ToString();
                    if (!String.IsNullOrWhiteSpace(contact.GivenName))
                    {
                        contact.DisplayName = contact.GivenName + " " + contact.Surname;
                    }
                    contact.FileAs = contact.DisplayName;
                }

                CultureInfo provider = new CultureInfo("en-US");
                DateTime date;
                if (contactsTable.Columns.Contains(properties.Anniversary))
                {
                    contact.WeddingAnniversary = DateTime.TryParseExact(
                        row[properties.Anniversary].ToString(), "d", provider, DateTimeStyles.None, out date) ? 
                        (DateTime?)date : null;
                }

                if (contactsTable.Columns.Contains(properties.Birthday))
                {
                    contact.Birthday = DateTime.TryParseExact(
                        row[properties.Birthday].ToString(), "d", provider, DateTimeStyles.None, out date) ? 
                        (DateTime?)date : null;
                }

                contact.Save();
                contactsList.Add(contact);

                // Load the properties that we can use when creating the appointments.
                PropertySet propertySet = new PropertySet(ContactSchema.DisplayName, 
                    ContactSchema.Birthday, ContactSchema.WeddingAnniversary);
                contact.Load(propertySet);
            }

            return contactsList;
        }

        /// <summary>
        /// Import the CSV file into a DataTable.
        /// </summary>
        private static void ImportCSVFile(DataTable contactsTable, String filepath)
        {
            if (String.IsNullOrWhiteSpace(filepath))
            {
                return;
            }

            if (contactsTable == null)
            {
                contactsTable = new DataTable();
            }

            if (File.Exists(filepath))
            {
                using (StreamReader reader = new StreamReader(filepath, Encoding.Unicode))
                {
                    String strLine = null;
                    Boolean isColumn = true;

                    while (!String.IsNullOrWhiteSpace(strLine = reader.ReadLine()))
                    {
                        String[] strings = strLine.Replace("\"", "").Split(',');

                        if (isColumn)
                        {
                            isColumn = false;

                            foreach (String str in strings)
                            {
                                String columnName = str.TrimStart('"').TrimEnd('"');
                                DataColumn column = new DataColumn(columnName, typeof(String));

                                contactsTable.Columns.Add(column);
                            }
                        }
                        else
                        {
                            DataRow row = contactsTable.NewRow();

                            for (int i = 0; i < contactsTable.Columns.Count; i++)
                            {
                                String rowValue = null;

                                if (!String.IsNullOrWhiteSpace(strings[i]))
                                {
                                    rowValue = strings[i].TrimStart('"').TrimEnd('"');
                                }

                                row[i] = rowValue;
                            }

                            contactsTable.Rows.Add(row);
                        }
                    }
                }
            }
        }

        private static Folder GetFolder(ExchangeService service, SearchFilter.RelationalFilter filter)
        {
            if (service == null)
            {
                return null;
            }

            PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly);

            FolderView folderView = new FolderView(5);
            folderView.PropertySet = propertySet;

            FindFoldersResults searchResults = service.FindFolders(WellKnownFolderName.MsgFolderRoot, 
                filter, folderView);

            return searchResults.FirstOrDefault();
        }

        /// <summary>
        /// Check if there's the duplicate appointment basing the Subject.
        /// </summary>
        private static Boolean HaveDuplicateAppointment(ExchangeService service, String appointmentSubject)
        {
            if (service == null || String.IsNullOrWhiteSpace(appointmentSubject))
            {
                return true;
            }

            SearchFilter.RelationalFilter searchFilter =
            new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "Calendar");
            Folder calendar = GetFolder(service, searchFilter);

            const Int32 pageSize = 10;
            ItemView itemView = new ItemView(pageSize);
            PropertySet propertySet = new PropertySet(BasePropertySet.IdOnly);
            itemView.PropertySet = propertySet;

            searchFilter = new SearchFilter.IsEqualTo(AppointmentSchema.Subject, appointmentSubject);

            FindItemsResults<Item> findResults = calendar.FindItems(searchFilter, itemView);

            if (findResults.TotalCount > 0)
            {
                return true;
            }

            return false;
        }
    }
}
