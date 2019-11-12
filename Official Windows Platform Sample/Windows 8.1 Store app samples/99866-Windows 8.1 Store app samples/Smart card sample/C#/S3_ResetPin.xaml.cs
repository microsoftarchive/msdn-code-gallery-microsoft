//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using SDKTemplate;
using System;
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
public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
{
    // A pointer back to the main page.  This is needed if you want to call
    // methods in MainPage such as NotifyUser()
    MainPage rootPage = MainPage.Current;

    public Scenario3()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when this page is about to be displayed in a Frame.
    /// </summary>
    /// <param name="e">Event data that describes how this page was reached.
    /// The Parameter property is typically used to configure the
    /// page.</param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
    }

    /// <summary>
    /// Click handler for the 'ResetPin' button. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ResetPin_Click(object sender, RoutedEventArgs e)
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
        
        try
        {
            SmartCard card = await rootPage.GetSmartCard();
            SmartCardProvisioning provisioning =
                await SmartCardProvisioning.FromSmartCardAsync(card);

            rootPage.NotifyUser("Resetting smart card PIN...",
                    NotifyType.StatusMessage);

            // When requesting a PIN reset, a SmartCardPinResetHandler must be
            // provided as an argument.  This handler must use the challenge
            // it receives and the card's admin key to calculate and set the
            // response.
            bool result = await provisioning.RequestPinResetAsync(
                (pinResetSender, request) =>
                {
                    SmartCardPinResetDeferral deferral =
                        request.GetDeferral();

                    try
                    {
                        IBuffer response =
                            ChallengeResponseAlgorithm.CalculateResponse(
                                request.Challenge,
                                rootPage.AdminKey);
                        request.SetResponse(response);
                    }
                    finally
                    {
                        deferral.Complete();
                    }
                });

            if (result)
            {
                rootPage.NotifyUser(
                    "Smart card PIN reset operation completed.",
                    NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser(
                    "Smart card PIN reset operation was canceled by " +
                    "the user.",
                    NotifyType.StatusMessage);
            }
            
        }
        catch (Exception ex)
        {
            rootPage.NotifyUser(
                "Resetting smart card PIN failed with exception: " +
                ex.ToString(),
                NotifyType.ErrorMessage);
        }
        finally
        {
            b.IsEnabled = true;
        }
    }
}

}
