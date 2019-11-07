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
public sealed partial class Scenario5 : SDKTemplate.Common.LayoutAwarePage
{
    // A pointer back to the main page.  This is needed if you want to call
    // methods in MainPage such as NotifyUser()
    MainPage rootPage = MainPage.Current;

    public Scenario5()
    {
        this.InitializeComponent();
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
    /// Click handler for the 'VerifyResponse' button. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void VerifyResponse_Click(object sender, RoutedEventArgs e)
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
            bool verifyResult = false;
            SmartCard card = await rootPage.GetSmartCard();
            SmartCardProvisioning provisioning =
                await SmartCardProvisioning.FromSmartCardAsync(card);

            rootPage.NotifyUser("Verifying smart card response...",
                                NotifyType.StatusMessage);

            using (SmartCardChallengeContext context =
                   await provisioning.GetChallengeContextAsync())
            {
                IBuffer response = ChallengeResponseAlgorithm.CalculateResponse(
                    context.Challenge,
                    rootPage.AdminKey);

                verifyResult = await context.VerifyResponseAsync(response);
            }

            rootPage.NotifyUser(
                "Smart card response verification completed. Result: " +
                verifyResult.ToString(),
                NotifyType.StatusMessage);
        }
        catch (Exception ex)
        {
            rootPage.NotifyUser(
                "Verifying smart card response operation failed " +
                "with exception: " +
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
