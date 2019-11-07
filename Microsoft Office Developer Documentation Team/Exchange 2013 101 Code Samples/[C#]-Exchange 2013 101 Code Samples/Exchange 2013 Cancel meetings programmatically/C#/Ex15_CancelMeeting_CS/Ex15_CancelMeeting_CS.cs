using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_CancelMeeting_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            CancelMeeting(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Find and then cancel a meeting.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void CancelMeeting(ExchangeService service)
        { 
            // Specify a view that returns a single item.
            ItemView view = new ItemView(1);

            string querystring = "Subject:Lunch";
            string organizer = "user@contoso.com"; // This address should represent the caller.

            try
            {
                // Find the first appointment in the calendar with "Lunch" set for the subject property.
                // This results in a FindItem operation call to EWS.
                FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Calendar, querystring, view);

                if (results.TotalCount > 0)
                {
                    if (results.Items[0] is Appointment)
                    {
                        Appointment meeting = results.Items[0] as Appointment;

                        // Determine whether the caller is the organizer. Only organizers can cancel meetings.
                        if (meeting.Organizer.Equals(new EmailAddress(organizer)))
                        {
                            // Cancels the meeting and sends cancellation messages to the attendees.
                            // This results in a call to EWS by means of the CreateItem operation and
                            // a MeetingCancellation response object. Do not delete meetings because
                            // cancellation messages are not sent to attendees. You can also use 
                            // Appointment.Delete(DeleteMode, SendCancellationsMode) to cancel a meeting
                            // but this does not give you an option to send a cancellation message.
                            var cancelResults = meeting.CancelMeeting("This meeting has been canceled");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No meeting was found with your search criteria.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error message: " + ex.Message);
            }
        }
    }
}
