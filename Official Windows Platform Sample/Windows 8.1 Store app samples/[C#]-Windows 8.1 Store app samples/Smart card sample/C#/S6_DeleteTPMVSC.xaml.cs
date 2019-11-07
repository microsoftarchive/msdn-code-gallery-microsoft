//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using SDKTemplate;
using System;
using System.Threading;
using Windows.Devices.SmartCards;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SmartCardSample
{

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Scenario6 : SDKTemplate.Common.LayoutAwarePage
{
    // A pointer back to the main page.  This is needed if you want to call
    // methods in MainPage such as NotifyUser()
    MainPage rootPage = MainPage.Current;

    private SmartCardReader reader;
    private SynchronizationContext uiContext = null;

    public Scenario6()
    {
        this.InitializeComponent();

        // Since this class is a page, it will be instantiated on the UI
        // thread.  We need to keep a reference to the UI thread's
        // synchronization context as a member so that we can access it
        // from other threads for event handlers.  See the
        // HandleCardRemoved method for more information.
        uiContext = SynchronizationContext.Current;
    }

    /// <summary>
    /// Invoked when this page is about to be displayed in a Frame.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.
    /// The Parameter property is typically used to configure the page.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
    }

    /// <summary>
    /// Click handler for the 'Delete' button.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (!rootPage.ValidateTPMSmartCard())
        {
            rootPage.NotifyUser(
                "Use Scenario One to create a TPM virtual smart card.",
                NotifyType.ErrorMessage);
            return;
        }

        Button b = sender as Button;
        b.IsEnabled = false;
        rootPage.NotifyUser("Deleting the TPM virtual smart card...",
                            NotifyType.ErrorMessage);

        try
        {
            SmartCard card = await rootPage.GetSmartCard();

            // The following two lines are not directly related to TPM virtual
            // smart card creation, but are used to demonstrate how to handle
            // CardRemoved events by registering an event handler with a
            // SmartCardReader object.  Since we are using a TPM virtual smart
            // card in this case, the card cannot actually be added to or
            // removed from the reader, but a CardRemoved event will fire
            // when the reader and card are deleted.
            //
            // We must retain a reference to the SmartCardReader object to
            // which we are adding the event handler.  We use += to add the
            // HandleCardRemoved method as an event handler.  The function
            // will be automatically boxed in a TypedEventHandler, but
            // the function signature match the template arguments for
            // the specific event - in this case,
            // <SmartCardReader, CardRemovedEventArgs>
            reader = card.Reader;
            reader.CardRemoved += HandleCardRemoved;

            bool result = await SmartCardProvisioning
                                .RequestVirtualSmartCardDeletionAsync(card);

            if (result)
            {
                rootPage.NotifyUser(
                    "TPM virtual smart card deletion completed.",
                    NotifyType.StatusMessage);
                rootPage.SmartCardReaderDeviceId = null;
            }
            else
            {
                rootPage.NotifyUser(
                    "TPM virtual smart card deletion was canceled by "
                    + "the user.",
                    NotifyType.StatusMessage);
            }
        }
        catch (Exception ex)
        {
            rootPage.NotifyUser(
                "TPM virtual smartcard deletion failed with exception: " +
                ex.ToString(),
                NotifyType.ErrorMessage);
        }
        finally
        {
            b.IsEnabled = true;
        }
    }

    void HandleCardRemoved(SmartCardReader evtReader, CardRemovedEventArgs args)
    {
        // This event handler will not be invoked on the UI thread.  Hence,
        // to perform UI operations we need to post a lambda to be executed
        // back on the UI thread; otherwise we may access objects which
        // are not marshalled for the current thread, which will result in an
        // exception due to RPC_E_WRONG_THREAD.
        uiContext.Post((object ignore) =>
        {
            rootPage.NotifyUser(
                "Card removed from reader " + evtReader.Name + ".",
                NotifyType.StatusMessage);
        }, null);
    }
}

}
