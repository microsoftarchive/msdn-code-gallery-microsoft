using System;
using System.Collections.ObjectModel;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_CreateMeeting_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            CreateMeeting(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Create meetings either one at a time or batch request the creation of meetings. Meetings
        /// are appointments that include attendees.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void CreateMeeting(ExchangeService service)
        {
            bool demoBatchCreateMeeting = true;

            Appointment meeting1 = new Appointment(service);
            meeting1.Subject = "Status Meeting";
            meeting1.Body = "The purpose of this meeting is to discuss status.";
            meeting1.Start = new DateTime(2013, 6, 1, 9, 0, 0);
            meeting1.End = meeting1.Start.AddHours(2);
            meeting1.Location = "Conf Room";
            meeting1.RequiredAttendees.Add("user1@contoso.com");
            meeting1.RequiredAttendees.Add("user2@contoso.com");
            meeting1.OptionalAttendees.Add("user3@contoso.com");

            Appointment meeting2 = new Appointment(service);
            meeting2.Subject = "Lunch";
            meeting2.Body = "The purpose of this meeting is to eat and be merry.";
            meeting2.Start = new DateTime(2013, 6, 1, 12, 0, 0);
            meeting2.End = meeting2.Start.AddHours(2);
            meeting2.Location = "Contoso cafe";
            meeting2.RequiredAttendees.Add("user1@contoso.com");
            meeting2.RequiredAttendees.Add("user2@contoso.com");
            meeting2.OptionalAttendees.Add("user3@contoso.com");

            try
            {
                if (demoBatchCreateMeeting) // Show batch.
                {
                    Collection<Appointment> meetings = new Collection<Appointment>();
                    meetings.Add(meeting1);
                    meetings.Add(meeting2);

                    // Create the batch of meetings. This results in a CreateItem operation call to EWS.
                    ServiceResponseCollection<ServiceResponse> responses = service.CreateItems(meetings, 
                                                                                              WellKnownFolderName.Calendar, 
                                                                                              MessageDisposition.SendOnly, 
                                                                                              SendInvitationsMode.SendToAllAndSaveCopy);

                    if (responses.OverallResult == ServiceResult.Success)
                    {
                        Console.WriteLine("You've successfully created a couple of meetings in a single call.");
                    }
                    else if (responses.OverallResult == ServiceResult.Warning)
                    {
                        Console.WriteLine("There are some issues with your batch request.");

                        foreach (ServiceResponse response in responses)
                        {
                            if (response.Result == ServiceResult.Error)
                            {
                                Console.WriteLine("Error code: " + response.ErrorCode.ToString());
                                Console.WriteLine("Error message: " + response.ErrorMessage);
                            }
                        }
                    }
                    else // responses.OverallResult == ServiceResult.Error
                    {
                        Console.WriteLine("There are errors with your batch request.");

                        foreach (ServiceResponse response in responses)
                        {
                            if (response.Result == ServiceResult.Error)
                            {
                                Console.WriteLine("Error code: " + response.ErrorCode.ToString());
                                Console.WriteLine("Error message: " + response.ErrorMessage);
                            }
                        }
                    }
                }
                else // Show creation of a single meeting.
                {
                    // Create a single meeting. This results in a CreateItem operation call to EWS.
                    meeting1.Save(SendInvitationsMode.SendToAllAndSaveCopy);
                    Console.WriteLine("You've successfully created a single meeting.");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception info: " +ex.Message);
            }
        }
    }
}
