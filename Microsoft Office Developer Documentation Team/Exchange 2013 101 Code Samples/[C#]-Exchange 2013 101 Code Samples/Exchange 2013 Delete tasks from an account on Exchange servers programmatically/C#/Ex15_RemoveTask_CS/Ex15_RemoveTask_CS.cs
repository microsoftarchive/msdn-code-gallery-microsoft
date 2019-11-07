using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exchange101;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_RemoveTask_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        static void Main(string[] args)
        {
            DeleteTask(service, "Custom Task", DeleteMode.HardDelete);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        private static void DeleteTask(ExchangeService service, string Subject, DeleteMode deleteMode)
        {
            // Try to retrieve the task specified by the subject.
            Task task = Ex15_FindTaskBySubject_CS.FindTaskBySubject(service, Subject);
            
            // If the task if found, delete it with the specified deleteMode.
            if (task != null)
            {
                task.Delete(deleteMode);
                Console.WriteLine("Task deleted.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }
    }
}
