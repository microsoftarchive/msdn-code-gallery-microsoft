/*=====================================================================
  This file is part of the Microsoft Unified Communications Code Samples.

  Copyright (C) 2012 Microsoft Corporation.  All rights reserved.

This source code is intended only as a supplement to Microsoft
Development Tools and/or on-line documentation.  See these other
materials for detailed information regarding Microsoft code samples.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
=====================================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Lync.Model;

namespace PresencePublication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        // Current dispatcher reference for changes in the user interface.
        private Dispatcher dispatcher;
        private LyncClient lyncClient;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            //Save the current dispatcher to use it for changes in the user interface.
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        #region Handlers for user interface controls events
        /// <summary>
        /// Handler for the Loaded event of the Window.
        /// Used to initialize the values shown in the user interface (e.g. availability values), get the Lync client instance
        /// and start listening for events of changes in the client state.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Add the availability values to the ComboBox
            availabilityComboBox.Items.Add(ContactAvailability.Free);
            availabilityComboBox.Items.Add(ContactAvailability.Busy);
            availabilityComboBox.Items.Add(ContactAvailability.DoNotDisturb);
            availabilityComboBox.Items.Add(ContactAvailability.Away);

            //Listen for events of changes in the state of the client
            try
            {
                lyncClient = LyncClient.GetClient();
            }
            catch (ClientNotFoundException clientNotFoundException)
            {
                Console.WriteLine(clientNotFoundException);
                return;
            }
            catch (NotStartedByUserException notStartedByUserException)
            {
                Console.Out.WriteLine(notStartedByUserException);
                return;
            }
            catch (LyncClientException lyncClientException)
            {
                Console.Out.WriteLine(lyncClientException);
                return;
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                    return;
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }

            lyncClient.StateChanged +=
                new EventHandler<ClientStateChangedEventArgs>(Client_StateChanged);

            //Update the user interface
            UpdateUserInterface(lyncClient.State);
        }

        /// <summary>
        /// Handler for the SelectionChanged event of the Availability ComboBox. Used to publish the selected availability value in Lync
        /// </summary>
        private void AvailabilityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Add the availability to the contact information items to be published
            Dictionary<PublishableContactInformationType, object> newInformation =
                new Dictionary<PublishableContactInformationType, object>();
            newInformation.Add(PublishableContactInformationType.Availability, availabilityComboBox.SelectedItem);

            //Publish the new availability value
            try
            {
                lyncClient.Self.BeginPublishContactInformation(newInformation,PublishContactInformationCallback, null);
            }
            catch (LyncClientException lyncClientException)
            {
                Console.WriteLine(lyncClientException);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }

        }

        /// <summary>
        /// Handler for the Click event of the Note Button. Used to publish a new personal note value in Lync
        /// </summary>
        private void SetNoteButton_Click(object sender, RoutedEventArgs e)
        {
            //Add the personal note to the contact information items to be published
            Dictionary<PublishableContactInformationType, object> newInformation =
                new Dictionary<PublishableContactInformationType, object>();
            newInformation.Add(PublishableContactInformationType.PersonalNote, personalNoteTextBox.Text);

            //Publish the new personal note value
            try
            {
                lyncClient.Self.BeginPublishContactInformation(newInformation,PublishContactInformationCallback, null);
            }
            catch (LyncClientException lyncClientException)
            {
                Console.WriteLine(lyncClientException);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }
        }

        /// <summary>
        /// Handler for the Click event of the SignInOut Button. Used to sign in or out Lync depending on the current client state.
        /// </summary>
        private void SignInOutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lyncClient.State == ClientState.SignedIn)
                {
                    //Sign out If the current client state is Signed In
                    lyncClient.BeginSignOut(SignOutCallback, null);
                }
                else if (lyncClient.State == ClientState.SignedOut)
                {
                    //Sign in If the current client state is Signed Out
                    lyncClient.BeginSignIn(null, null, null, SignInCallback, null);
                }
            }
            catch (LyncClientException lyncClientException)
            {
                Console.WriteLine(lyncClientException);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }

        }
        #endregion

        #region Handlers for Lync events
        /// <summary>
        /// Handler for the ContactInformationChanged event of the contact. Used to update the contact's information in the user interface.
        /// </summary>
        private void SelfContact_ContactInformationChanged(object sender, ContactInformationChangedEventArgs e)
        {
            //Only update the contact information in the user interface if the client is signed in.
            //Ignore other states including transitions (e.g. signing in or out).
            if (lyncClient.State == ClientState.SignedIn)
            {
                //Get from Lync only the contact information that changed.

                if (e.ChangedContactInformation.Contains(ContactInformationType.DisplayName))
                {
                    //Use the current dispatcher to update the contact's name in the user interface.
                    dispatcher.BeginInvoke(new Action(SetName));
                }
                if (e.ChangedContactInformation.Contains(ContactInformationType.Availability))
                {
                    //Use the current dispatcher to update the contact's availability in the user interface.
                    dispatcher.BeginInvoke(new Action(SetAvailability));
                }
                if (e.ChangedContactInformation.Contains(ContactInformationType.PersonalNote))
                {
                    //Use the current dispatcher to update the contact's personal note in the user interface.
                    dispatcher.BeginInvoke(new Action(SetPersonalNote));
                }
                if (e.ChangedContactInformation.Contains(ContactInformationType.Photo))
                {
                    //Use the current dispatcher to update the contact's photo in the user interface.
                    dispatcher.BeginInvoke(new Action(SetContactPhoto));
                }
            }
        }

        /// <summary>
        /// Handler for the StateChanged event of the contact. Used to update the user interface with the new client state.
        /// </summary>
        private void Client_StateChanged(object sender, ClientStateChangedEventArgs e)
        {
            //Use the current dispatcher to update the user interface with the new client state.
            dispatcher.BeginInvoke(new Action<ClientState>(UpdateUserInterface), e.NewState);
        }
        #endregion

        #region Callbacks
        /// <summary>
        /// Callback invoked when LyncClient.BeginSignIn is completed
        /// </summary>
        /// <param name="result">The status of the asynchronous operation</param>
        private void SignInCallback(IAsyncResult result)
        {
            try
            {
                lyncClient.EndSignIn(result);
            }
            catch (LyncClientException e)
            {
                Console.WriteLine(e);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }

        }

        /// <summary>
        /// Callback invoked when LyncClient.BeginSignOut is completed
        /// </summary>
        /// <param name="result">The status of the asynchronous operation</param>
        private void SignOutCallback(IAsyncResult result)
        {
            try
            {
                lyncClient.EndSignOut(result);
            }
            catch (LyncClientException e)
            {
                Console.WriteLine(e);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }

        }

        /// <summary>
        /// Callback invoked when Self.BeginPublishContactInformation is completed
        /// </summary>
        /// <param name="result">The status of the asynchronous operation</param>
        private void PublishContactInformationCallback(IAsyncResult result)
        {
            lyncClient.Self.EndPublishContactInformation(result);
        }
        #endregion

        /// <summary>
        /// Updates the user interface
        /// </summary>
        /// <param name="currentState"></param>
        private void UpdateUserInterface(ClientState currentState)
        {
            //Update the client state in the user interface
            clientStateTextBox.Text = currentState.ToString();

            if (currentState == ClientState.SignedIn)
            {
                //Listen for events of changes of the contact's information
                lyncClient.Self.Contact.ContactInformationChanged +=
                    new EventHandler<ContactInformationChangedEventArgs>(SelfContact_ContactInformationChanged);

                //Get the contact's information from Lync and update with it the corresponding elements of the user interface.
                SetName();
                SetAvailability();
                SetPersonalNote();
                SetContactPhoto();

                //Update the SignInOut button content
                signInOutButton.Content = "Sign Out";

                //Enable elements in the user interface
                personalNoteTextBox.IsEnabled = true;
                availabilityComboBox.IsEnabled = true;
                setNoteButton.IsEnabled = true;
            }
            else
            {
                //Update the SignInOut button content
                signInOutButton.Content = "Sign In";

                //Disable elements in the user interface
                personalNoteTextBox.IsEnabled = false;
                availabilityComboBox.IsEnabled = false;
                setNoteButton.IsEnabled = false;

                //Change the color of the border containing the contact's photo to match the contact's offline status
                availabilityBorder.Background = Brushes.LightSlateGray;
            }
        }

        /// <summary>
        /// Gets the contact's current availability value from Lync and updates the corresponding elements in the user interface
        /// </summary>
        private void SetAvailability()
        {
            //Get the current availability value from Lync
            ContactAvailability currentAvailability = 0;
            try
            {
                currentAvailability = (ContactAvailability)
                                                          lyncClient.Self.Contact.GetContactInformation(ContactInformationType.Availability);
            }
            catch (LyncClientException e)
            {
                Console.WriteLine(e);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }


            if (currentAvailability != 0)
            {
                //Update the availability ComboBox with the contact's current availability.
                availabilityComboBox.SelectedValue = currentAvailability;

                //Choose a color to match the contact's current availability and update the border area containing the contact's photo
                Brush availabilityColor;
                switch (currentAvailability)
                {
                    case ContactAvailability.Away:
                        availabilityColor = Brushes.Yellow;
                        break;
                    case ContactAvailability.Busy:
                        availabilityColor = Brushes.Red;
                        break;
                    case ContactAvailability.BusyIdle:
                        availabilityColor = Brushes.Red;
                        break;
                    case ContactAvailability.DoNotDisturb:
                        availabilityColor = Brushes.DarkRed;
                        break;
                    case ContactAvailability.Free:
                        availabilityColor = Brushes.LimeGreen;
                        break;
                    case ContactAvailability.FreeIdle:
                        availabilityColor = Brushes.LimeGreen;
                        break;
                    case ContactAvailability.Offline:
                        availabilityColor = Brushes.LightSlateGray;
                        break;
                    case ContactAvailability.TemporarilyAway:
                        availabilityColor = Brushes.Yellow;
                        break;
                    default:
                        availabilityColor = Brushes.LightSlateGray;
                        break;
                }
                availabilityBorder.Background = availabilityColor;
            }
        }

        /// <summary>
        /// Gets the contact's name from Lync and updates the corresponding element in the user interface
        /// </summary>
        private void SetName()
        {
            string text = string.Empty;
            try
            {
                text = lyncClient.Self.Contact.GetContactInformation(ContactInformationType.DisplayName)
                              as string;
            }
            catch (LyncClientException e)
            {
                Console.WriteLine(e);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }

            nameTextBlock.Text = text;
        }

        /// <summary>
        /// Gets the contact's personal note from Lync and updates the corresponding element in the user interface
        /// </summary>
        private void SetPersonalNote()
        {
            string text = string.Empty;
            try
            {
                text = lyncClient.Self.Contact.GetContactInformation(ContactInformationType.PersonalNote)
                              as string;
            }
            catch (LyncClientException e)
            {
                Console.WriteLine(e);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }

            personalNoteTextBox.Text = text;
        }

        /// <summary>
        /// Gets the contact's photo from Lync and updates the corresponding element in the user interface
        /// </summary>
        private void SetContactPhoto()
        {
            try
            {
                using (Stream photoStream =
                    lyncClient.Self.Contact.GetContactInformation(ContactInformationType.Photo) as Stream)
                {
                    if (photoStream != null)
                    {
                        BitmapImage bm = new BitmapImage();
                        bm.BeginInit();
                        bm.StreamSource = photoStream;
                        bm.EndInit();
                        photoImage.Source = bm;
                    }
                }
            }
            catch (LyncClientException e)
            {
                Console.WriteLine(e);
            }
            catch (SystemException systemException)
            {
                if (IsLyncException(systemException))
                {
                    // Log the exception thrown by the Lync Model API.
                    Console.WriteLine("Error: " + systemException);
                }
                else
                {
                    // Rethrow the SystemException which did not come from the Lync Model API.
                    throw;
                }
            }
        }

        /// <summary>
        /// Identify if a particular SystemException is one of the exceptions which may be thrown
        /// by the Lync Model API.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsLyncException(SystemException ex)
        {
            return
                ex is NotImplementedException ||
                ex is ArgumentException ||
                ex is NullReferenceException ||
                ex is NotSupportedException ||
                ex is ArgumentOutOfRangeException ||
                ex is IndexOutOfRangeException ||
                ex is InvalidOperationException ||
                ex is TypeLoadException ||
                ex is TypeInitializationException ||
                ex is InvalidComObjectException ||
                ex is InvalidCastException;
        }
    }
}
