using System;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure that the code meets the coding requirements of your organization.
    class Email
    {
        static ExchangeService service = Service.ConnectToService(UserDataFromConsole.GetUserData(), new TraceListener());
        
        [STAThreadAttribute]
        static void Main(string[] args)
        {
           PlayEmailOnPhone(service);
           Console.WriteLine("\r\n");
           Console.WriteLine("Press or select Enter..."); 
           Console.Read();
        }

        /// <summary>
        /// Finds the first email message and initiates an attempt to call a phone number and 
        /// dictate the contents of the email message. This sample requires that Unified Messaging
        /// is enabled for the caller.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        static void PlayEmailOnPhone(ExchangeService service)
        {
           // Find the first email message in the Inbox folder.
           ItemView view = new ItemView(1);
           view.PropertySet = new PropertySet(BasePropertySet.IdOnly);

           // Find the first email message in the Inbox. This results in a FindItem operation call to EWS. 
           FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, view);

           string itemId = results.Items[0].Id.UniqueId;
           string dialstring = "4255551212";

           // Initiate a call to dictate an email message over a phone call. This results in a PlayOnPhone operation
           // call to EWS.
           PhoneCall call = service.UnifiedMessaging.PlayOnPhone(itemId, dialstring);
           Console.WriteLine("Call Number: " + dialstring);
           Console.WriteLine(DateTime.Now + " - Call Status: " + call.State + "\n\r");

           // Create a timer that will start immediately. Timer will call callback every 2 seconds.
           using (System.Threading.Timer timer = new System.Threading.Timer(RefreshPhoneCallState, call, 0, 2000))
           {
              Console.WriteLine("PRESS ENTER TO END THE PHONE CALL AND CALL STATUS UPDATES");
              Console.ReadLine();

              // Disconnect the phone call if it is not already disconnected.
              if (call.State != PhoneCallState.Disconnected)
              {
                 call.Disconnect();
              }
           }

           Console.WriteLine("PRESS ENTER TO END CLOSE THIS WINDOW");
           Console.ReadLine();
        }

        /// <summary>
        /// Callback method for refreshing call state for the phone call.
        /// </summary>
        /// <param name="pCall">The PhoneCall object that contains the call state.</param>
        static void RefreshPhoneCallState(object pCall)
        {
           PhoneCall call = (PhoneCall)pCall;

           // Update the phone call state. This results in a GetPhoneCallInformation operation call to EWS.
           call.Refresh();
           
           Console.WriteLine(DateTime.Now + " - Call Status: " + call.State);
        }
    }   
}
