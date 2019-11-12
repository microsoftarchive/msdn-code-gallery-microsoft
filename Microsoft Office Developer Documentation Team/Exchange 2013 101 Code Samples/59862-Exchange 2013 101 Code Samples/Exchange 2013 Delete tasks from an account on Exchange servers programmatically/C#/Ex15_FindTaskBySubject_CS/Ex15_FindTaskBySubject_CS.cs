using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exchange101;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    public class Ex15_FindTaskBySubject_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        static void Main(string[] args)
        {
            // Try to retrieve a task item with the subject "Custom Task" (located in the Tasks folder).
            Task TaskItem = FindTaskBySubject(service, "Custom Task");
            if (TaskItem != null)
            {
                Console.WriteLine("The task item with the subject 'Custom Task' was found.");
            }
            else
            {
                Console.WriteLine("The task item with the subject 'Custom Task' was NOT found.");
            }

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }


        public static Task FindTaskBySubject(ExchangeService service, string Subject)
        {
            // Specify the folder to search, and limit the properties returned in the result.
            TasksFolder tasksfolder = TasksFolder.Bind(service,
                                                       WellKnownFolderName.Tasks,
                                                       new PropertySet(BasePropertySet.IdOnly, FolderSchema.TotalCount));
            
            // Set the number of items to the smaller of the number of items in the Contacts folder or 1000.
            int numItems = tasksfolder.TotalCount < 1000 ? tasksfolder.TotalCount : 1000;
            
            // Instantiate the item view with the number of items to retrieve from the contacts folder.
            ItemView view = new ItemView(numItems);
            
            // To keep the request smaller, send only the display name.
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, TaskSchema.Subject);
            
            // Create a searchfilter to check the subject of the tasks.
            SearchFilter.IsEqualTo filter = new SearchFilter.IsEqualTo(TaskSchema.Subject, Subject);
            
            // Retrieve the items in the Tasks folder with the properties you selected.
            FindItemsResults<Item> taskItems = service.FindItems(WellKnownFolderName.Tasks, filter, view);
            
            // If the subject of the task matches only one item, return that task item.
            if (taskItems.Count() == 1)
            {
                return (Task)taskItems.Items[0];
            }
            // No tasks, or more than one, were found.
            else 
            {
                return null;
            }
        }

    }
}
