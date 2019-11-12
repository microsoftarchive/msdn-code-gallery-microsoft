using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_FindAllContactsAndContactGroups_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            FindAllContactsAndContactGroups(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Finds all the contacts and contact groups within the default Contacts folder,
        /// including within each child folder of the default Contacts folder.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void FindAllContactsAndContactGroups(ExchangeService service)
        {
            int contactOffset = 0;
            const int contactPageSize = 200;
            bool contactMoreItems = true;
            ItemView view = new ItemView(contactPageSize, contactOffset);

            foreach (ContactsFolder folder in GetAllContactsFolders(service))
            {
                Console.WriteLine("Folder named {0} has {1} contacts or contact groups.", folder.DisplayName, folder.TotalCount.ToString());

                while (contactMoreItems)
                {
                    try
                    {
                        FindItemsResults<Item> results = folder.FindItems(view);

                        foreach (Item item in results.Items)
                        {
                            ContactOrContactGroupHelper(item);
                        }

                        if (results.MoreAvailable == false)
                        {
                            contactMoreItems = false;
                        }

                        else
                        {
                            view.Offset = view.Offset + contactPageSize;
                            Console.WriteLine("Page #{0}", view.Offset / contactPageSize);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: {0}", ex.Message);
                    }
                }

                // Reset the view for the next FindItem operation call.
                view.Offset = 0;
                contactMoreItems = true;
            }
        }

        private static Collection<Folder> GetAllContactsFolders(ExchangeService service)
        {
            // The collection will contain all contact folders. 
            Collection<Folder> folders = new Collection<Folder>();
            
            // Get the root Contacts folder and load all properties. This results in a GetFolder call to EWS.
            ContactsFolder rootContactFolder = ContactsFolder.Bind(service, WellKnownFolderName.Contacts);
            folders.Add(rootContactFolder);
            Console.WriteLine("Added the default Contacts folder to the collection of contact folders.");

            // Find all child folders of the root Contacts folder.
            int initialFolderSearchOffset = 0;
            const int folderSearchPageSize = 100;
            bool AreMoreFolders = true;
            FolderView folderView = new FolderView(folderSearchPageSize, initialFolderSearchOffset);
            folderView.Traversal = FolderTraversal.Deep;
            folderView.PropertySet = new PropertySet(BasePropertySet.IdOnly);

            while (AreMoreFolders)
            {
                try
                {
                    // Find all the child folders of the default Contacts folder. This results in a FindFolder
                    // operation call to EWS.
                    FindFoldersResults childrenOfContactsFolderResults = rootContactFolder.FindFolders(folderView);
                    if (folderView.Offset == 0)
                    {
                        Console.WriteLine("Found {0} child folders of the default Contacts folder.", childrenOfContactsFolderResults.TotalCount);
                    }

                    foreach (Folder f in childrenOfContactsFolderResults.Folders)
                    {
                        ContactsFolder contactFolder = (ContactsFolder)f;
                        // Loads all the properties for the folder. This results in a GetFolder operation call to EWS.
                        contactFolder.Load();
                        Console.WriteLine("Loaded a folder named {0} and added it to the collection of contact folders.", contactFolder.DisplayName);
                        // Add the folder to the collection of contact folders.
                        folders.Add(contactFolder);
                    }

                    // Turn off paged searches if there are no more folders to return.
                    if (childrenOfContactsFolderResults.MoreAvailable == false)
                    {
                        AreMoreFolders = false;
                    }
                    else // Increment the paging offset.
                    {
                        folderView.Offset = folderView.Offset + folderSearchPageSize;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: {0}", ex.Message);
                }
            }

            return folders;
        }

        private static void ContactOrContactGroupHelper(Item item)
        {
            if (item is Contact)
            {
                PrintContactHelper(item);
            }

            if (item is ContactGroup)
            {
                PrintContactGroupHelper(item);
            }
        }

        private static void PrintContactHelper(Item item)
        {                                
            Contact contact = item as Contact;

            if (contact.DisplayName != null)
            {
                Console.WriteLine("Contact display name: " + contact.DisplayName);
            }
            else
            {
                Console.WriteLine("Contact identifier: " + contact.Id.UniqueId);
            }
        }

        private static void PrintContactGroupHelper(Item item)
        {
            ContactGroup contactGroup = item as ContactGroup;

            // This results in a GetItem operation call to EWS. This loads all the properties, 
            // including the members of the ContactGroup.
            contactGroup.Load();

            Console.WriteLine("Contact group name ({0} members): " + contactGroup.DisplayName, contactGroup.Members.Count.ToString());

            if (contactGroup.Members.Count > 0)
            {
                ExpandGroupResults expandResults = service.ExpandGroup(contactGroup.Id);

                foreach (EmailAddress address in expandResults.Members)
                {
                    try
                    {
                        Item expandedItem = Item.Bind(service, address.Id);
                        ContactOrContactGroupHelper(expandedItem);
                    }
                    catch (ServiceResponseException ex)
                    {
                        Console.WriteLine("Found a reference to contact item that does not exist in your mailbox. You might want to delete this entry from your contact group.");
                    }
                }
            }
        }
    }
}
