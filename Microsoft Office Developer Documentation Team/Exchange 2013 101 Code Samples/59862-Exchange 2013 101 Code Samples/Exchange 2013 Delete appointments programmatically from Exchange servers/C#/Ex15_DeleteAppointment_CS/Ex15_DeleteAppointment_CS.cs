using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_DeleteAppointment_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            DeleteAppointment(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Delete an appointment.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void DeleteAppointment(ExchangeService service)
        {
            // Specify a view that returns a single item.
            ItemView view = new ItemView(1);

            string querystring = "Subject:'Play date at the park'";

            try
            {
                // Find the first appointment in the calendar with 'Play date at the park' set for the subject property.
                // This results in a FindItem operation call to EWS.
                FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Calendar, querystring, view);

                if (results.TotalCount > 0)
                {
                    if (results.Items[0] is Appointment)
                    {
                        Appointment appointment = results.Items[0] as Appointment;

                        // Determine whether a meeting request was sent. Meetings must be canceled. Appointments
                        // can be deleted. This sample shows how to handle appointments.
                        if (!appointment.MeetingRequestWasSent)
                        {
                            // Delete the appointment from your calendar. This results in a DeleteItem operation call to EWS.
                            appointment.Delete(DeleteMode.HardDelete);
                        }
                        else
                        {
                            Console.WriteLine("This is a meeting. Cancel the meeting by using Appointment.CancelMeeting.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No appointment was found with your search criteria.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
