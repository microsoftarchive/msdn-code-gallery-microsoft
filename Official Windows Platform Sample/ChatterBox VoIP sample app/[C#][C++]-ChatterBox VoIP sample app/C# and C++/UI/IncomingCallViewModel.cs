/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
namespace PhoneVoIPApp.UI
{
    /// <summary>
    /// The view model for the Incoming Call page
    /// </summary>
    public class IncomingCallViewModel : BaseViewModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public IncomingCallViewModel()
            : base()
        {
            this.CallerName = "Scott MacDonald";
            this.CallerNumber = "(555) 555 1111";
            this.Result = string.Empty;
        }

        private string callerName;
        public string CallerName
        {
            get
            {
                return this.callerName;
            }

            set
            {
                if (value == null)
                    value = string.Empty;

                this.callerName = value;
                this.OnPropertyChanged("CallerName");

                // Update the simulate button state whenever the caller name changes
                this.OnCallerNameChanged(this.callerName);
            }
        }

        private string callerNumber;
        public string CallerNumber
        {
            get
            {
                return this.callerNumber;
            }

            set
            {
                if (value == null)
                    value = string.Empty;

                this.callerNumber = value;
                this.OnPropertyChanged("CallerNumber");
            }
        }

        private string result;
        public string Result
        {
            get
            {
                return this.result;
            }

            set
            {
                if (value == null)
                    value = string.Empty;

                this.result = value;
                this.OnPropertyChanged("Result");
            }
        }

        private bool isSimulateButtonEnabled;
        public bool IsSimulateButtonEnabled
        {
            get
            {
                return this.isSimulateButtonEnabled;
            }

            set
            {
                if (this.isSimulateButtonEnabled != value)
                {
                    this.isSimulateButtonEnabled = value;
                    this.OnPropertyChanged("IsSimulateButtonEnabled");
                }
            }
        }

        /// <summary>
        /// The text in the caller name text box has changed
        /// </summary>
        public void OnCallerNameChanged(string callerName)
        {
            // The simulate button is enabled only if there is a non-empty caller name.
            this.IsSimulateButtonEnabled = !string.IsNullOrEmpty(callerName);
        }

        /// <summary>
        /// Simulate an incoming call
        /// </summary>
        public void Simulate()
        {
            // Send a push notification to the push channel URI of this app to simulate an incoming call
            try
            {
                // Create an HTTPWebRequest that posts the raw notification to the Microsoft Push Notification Service.
                // HTTP POST is the only method allowed to send the notification.
                HttpWebRequest sendNotificationRequest = (HttpWebRequest)WebRequest.Create(((App)App.Current).PushChannelUri);
                sendNotificationRequest.Method = "POST";

                // Create the raw message.
                byte[] notificationMessage = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    Agents.Notification notification = new Agents.Notification();
                    notification.Name = this.CallerName;
                    notification.Number = this.CallerNumber;

                    XmlSerializer xs = new XmlSerializer(typeof(Agents.Notification));
                    xs.Serialize(ms, notification);

                    notificationMessage = ms.ToArray();
                }

                // Set the required web request headers
                sendNotificationRequest.ContentLength = notificationMessage.Length;
                sendNotificationRequest.ContentType = "text/xml";
                sendNotificationRequest.Headers["X-NotificationClass"] = "4"; // Class 4 indicates an incoming VoIP call

                // Write the request body
                sendNotificationRequest.BeginGetRequestStream((IAsyncResult arRequest) =>
                {
                    try
                    {
                        using (Stream requestStream = sendNotificationRequest.EndGetRequestStream(arRequest))
                        {
                            requestStream.Write(notificationMessage, 0, notificationMessage.Length);
                        }

                        // Get the response.
                        sendNotificationRequest.BeginGetResponse((IAsyncResult arResponse) =>
                        {
                            try
                            {
                                HttpWebResponse response = (HttpWebResponse)sendNotificationRequest.EndGetResponse(arResponse);
                                string notificationStatus = response.Headers["X-NotificationStatus"];
                                string subscriptionStatus = response.Headers["X-SubscriptionStatus"];
                                string deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];

                                // The push notification was sent
                                this.ShowResult(string.Format("Notification: {0}\r\nSubscription: {1}\r\nDevice: {2}", notificationStatus, subscriptionStatus, deviceConnectionStatus));
                            }
                            catch (Exception ex)
                            {
                                this.ShowResult(ex);
                            }
                        }, null);
                    }
                    catch (Exception ex)
                    {
                        this.ShowResult(ex);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                this.ShowResult(ex);
            }
        }

        /// <summary>
        /// Show the result of sending a push message
        /// </summary>
        private void ShowResult(object result)
        {
            // Note, this call is called on some thread-pool thread - dispatch to the UI thread before doing anything else
            this.Page.Dispatcher.BeginInvoke(() =>
            {
                Exception ex = result as Exception;
                if (ex == null)
                {
                    this.Result = string.Format("{0}\r\n{1}", result, DateTime.Now);
                }
                else
                {
                    this.Result = string.Format("An error has occurred\r\n{0}", DateTime.Now);
                }
            });
        }
    }
}
