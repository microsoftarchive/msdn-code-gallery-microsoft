using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Ex15_RespondToMeetingInvite_CS
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());

        static void Main(string[] args)
        {
            RespondToMeetingInvite(service);
            Console.WriteLine("Press or select Enter...");
            Console.Read();
        }

        /// <summary>
        /// Reply to a meeting request. You can either reply to a meeting request received in your Inbox or
        /// you can reply to the appointment in your calendar if the calendar item is automatically created 
        /// in your calendar folder. This sample identifies the different options for replying
        /// to a meeting request.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        private static void RespondToMeetingInvite(ExchangeService service)
        {
            // Specify a view that returns a single item.
            ItemView view = new ItemView(1);

            bool replyToMeetingRequest = true; // Indicates whether you will act on the meeting request or the calendar item.
            bool sendResponse = true;
            //bool tentative = true;
            //bool replyAll = true;

            string querystringMeetingRequest = "Subject:'Status update - planning meeting' Kind:meetings";
            string querystringAppointment = "Subject:'Status update - planning meeting'";

            if (replyToMeetingRequest)
            {
                // Find the first meeting request in the Inbox with 'Status update - planning meeting' set for the subject property.
                // This results in a FindItem operation call to EWS.
                FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, querystringMeetingRequest, view);

                if (results.TotalCount > 0) 
                {
                    if (results.Items[0] is MeetingRequest)
                    {
                        MeetingRequest request = results.Items[0] as MeetingRequest;

                        // Accept the meeting invitation. This results in a CreateItem operation call to EWS 
                        // with the AcceptItem response object. If the Exchange server created a tentatively 
                        // accepted calendar item for this request, Exchange will change the response value for the
                        // calendar item to accepted. Exchange will move the meeting request to the Deleted Items
                        // folder.
                        CalendarActionResults responseResults = request.Accept(sendResponse);
                        
                        // The following are other options for responding to meeting requests:
                        // CalendarActionResults responseResults = request.AcceptTentatively(sendResponse);
                        // CalendarActionResults responseResults = request.Decline(sendResponse);

                        // The following options for responding to meeting requests create a local copy of the meeting response
                        // so that you can set properties on the response before sending the meeting response message
                        // to the organizer:
                        // CalendarActionResults responseResults = request.CreateAcceptMessage(tentative); 
                        // CalendarActionResults responseResults = request.CreateDeclineMessage();

                        // The following options for acting on a meeting request change the item class of the response to an
                        // email message. This allows you to send an email response that does not affect the calendar workflow.
                        // You can update the ResponseMessage object properties and then use the ResponseMessage.Send
                        // method to send the email response.
                        // ResponseMessage response = request.CreateForward();
                        // ResponseMessage response = request.CreateReply(replyAll); // Creates a reply email message to all attendees.

                        // Get the appointment that was created in response to the accepted meeting request.
                        Appointment myAppointment = responseResults.Appointment;

                        // Gets the meeting request that was moved to the Deleted Items folder after the meeting request was
                        // used to make a response to the organizer. If the meeting request was responded to from
                        // the Deleted Items folder, the value of responseResults.MeetingRequest will be null.
                        if (responseResults.MeetingRequest != null)
                        {
                            MeetingRequest deletedRequest = responseResults.MeetingRequest;
                        }
                        // Gets a copy of the response sent to the organizer. If the attendee chose not to send a 
                        // response, the value of responseResults.MeetingResponse will be null.
                        if (responseResults.MeetingResponse != null)
                        {
                            MeetingResponse meetingReponse = responseResults.MeetingResponse;
                        }
                        // Gets a copy of the meeting cancellation if the organizer canceled the meeting. This will
                        // be null if the organizer has not canceled the meeting.
                        if (responseResults.MeetingCancellation != null)
                        {
                            MeetingCancellation meetingCancel = responseResults.MeetingCancellation;
                        }

                    }
                }
            }
            else // Find the automatically created calendar item in your calendar.
            {
                // Find the first appointment in the calendar with 'Status update - planning meeting' set for the subject property.
                // This results in a FindItem operation call to EWS.
                FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Calendar, querystringAppointment, view);

                if (results.TotalCount > 0)
                {
                    if (results.Items[0] is Appointment)
                    {
                        Appointment appointment = results.Items[0] as Appointment;

                        // Accept the calendar item. This results in a CreateItem operation call to EWS
                        // with the AcceptItem response object. Exchange will move the meeting 
                        // request to the Deleted Items folder.
                        CalendarActionResults responseResults = appointment.Accept(sendResponse);

                        // The following are other options for responding to the meeting organizer:
                        // CalendarActionResults responseResults = appointment.AcceptTentatively(sendResponse);
                        // CalendarActionResults responseResults = appointment.Decline(sendResponse);
                        // CalendarActionResults responseResults = appointment.CreateAcceptMessage(tentative); 
                        // CalendarActionResults responseResults = appointment.CreateDeclineMessage();
                        // appointment.Delete(DeleteMode.MoveToDeletedItems); // No response is sent to the organizer.
                        // ResponseMessage response = appointment.CreateForward();
                        // ResponseMessage response = appointment.CreateReply(replyAll);

                        // Gets the meeting request from the Deleted Items folder if it is present.
                        if (responseResults.MeetingRequest != null)
                        {
                            MeetingRequest deletedRequest = responseResults.MeetingRequest;
                        }
                        // Gets a copy of the response sent to the organizer. If the attendee chose not to send a 
                        // response, the value of responseResults.MeetingResponse will be null.
                        if (responseResults.MeetingResponse != null)
                        {
                            MeetingResponse meetingReponse = responseResults.MeetingResponse;
                        }
                        // Gets a copy of the meeting cancellation if the organizer canceled the meeting. This will
                        // be null if the organizer has not canceled the meeting.
                        if (responseResults.MeetingCancellation != null)
                        {
                            MeetingCancellation meetingCancel = responseResults.MeetingCancellation;
                        }
                    }
                }
            }
        }
    }
}
