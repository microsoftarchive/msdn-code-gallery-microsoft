using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exchange101;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_CreateRecurringTask_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        static void Main(string[] args)
        {
            CreateRecurringTask(service, "Custom Recurring Task", WellKnownFolderName.Tasks);

            Console.WriteLine("\r\n");
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        private static void CreateRecurringTask(ExchangeService service, string Subject, WellKnownFolderName DestinationFolder)
        {
            // Instaniate the Task object.
            Task task = new Task(service);

            // Specify the subject and body of the new task.
            task.Subject = Subject;
            task.Body = new MessageBody(BodyType.Text, "This is an example of a recurring task created using EWS Managed API.");

            // Set the recurrance pattern for the new task.
            DayOfTheWeek[] days = new DayOfTheWeek[] { DayOfTheWeek.Friday };
            task.Recurrence = new Recurrence.WeeklyPattern(DateTime.Today, 1, days);
            task.Recurrence.StartDate = DateTime.Today;
            task.Recurrence.NeverEnds();

            // Create the new task in the specified destination folder.
            task.Save(DestinationFolder);

            Console.WriteLine("Recurring task created.");
        }
    }
}
