using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exchange101;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_UpdateTask_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        static void Main(string[] args)
        {
            UpdateTask(service, "Custom Task");

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        static void UpdateTask(ExchangeService service, string Subject)
        {
            // Try to retrieve the task specified by the subject.
            Task FoundTask = Ex15_FindTaskBySubject_CS.FindTaskBySubject(service, Subject);
            
            // If the task if found, set the status to "Completed".
            if (FoundTask != null)
            {
                Task task = Task.Bind(service, FoundTask.Id);
                task.Status = TaskStatus.Completed;
                task.Update(ConflictResolutionMode.AutoResolve);
                Console.WriteLine("Task updated as being completed.");
            }
            else
            {
                Console.WriteLine("Task not found.");
            }
        }
    }
}
