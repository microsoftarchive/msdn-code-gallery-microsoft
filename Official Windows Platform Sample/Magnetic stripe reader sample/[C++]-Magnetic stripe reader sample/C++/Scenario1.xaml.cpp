//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::MagneticStripeReaderSample;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Concurrency;
using namespace Windows::Devices::PointOfService;
using namespace Windows::Foundation;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
	rootPage = MainPage::Current;
}

/// <summary>
/// Creates the default magnetic stripe reader.
/// </summary>
task<void> Scenario1::CreateDefaultReaderObject()
{
    return create_task(MagneticStripeReader::GetDefaultAsync()).then([this] (MagneticStripeReader^ reader)
    {
        _reader = reader;
        if (_reader != nullptr)
        {			
            UpdateReaderStatusTextBlock("Default Magnetic Stripe Reader created.");
            UpdateReaderStatusTextBlock("Device Id is:" + _reader->DeviceId);
        }
        else
        {
            UpdateReaderStatusTextBlock("Magnetic Stripe Reader not found. Please connect a Magnetic Stripe Reader.");
            rootPage->NotifyUser("No Magnetic Stripe Reader found", NotifyType::StatusMessage);
        }
    });

}

/// <summary>
/// Update the status in the UI with the string passed.
/// </summary>
/// <param name="message"></param>
void Scenario1::UpdateReaderStatusTextBlock(Platform::String^ strMessage)
{
    ReaderStatusText->Text += "\r";
    ReaderStatusText->Text += strMessage;
}

/// <summary>
/// This method claims the magnetic stripe reader.
/// </summary>
task<void> Scenario1::ClaimReader()
{
    
    // claim the magnetic stripe reader
    return create_task(_reader->ClaimReaderAsync()).then([this] (ClaimedMagneticStripeReader^ claimedReader)
    {
        _claimedReader = claimedReader;
        // enable the claimed magnetic stripe reader
        if (_claimedReader != nullptr)
        {
            UpdateReaderStatusTextBlock("Claim Magnetic Stripe Reader succeeded.");		
        }
        else
        {
            UpdateReaderStatusTextBlock("Claim Magnetic Stripe Reader failed.");
        }
    });
}

task<void> Scenario1::EnableReader()
{
    
    // enable the magnetic stripe reader
    return create_task(_claimedReader->EnableAsync()).then([this](void)
    {
        UpdateReaderStatusTextBlock("Enable Magnetic Stripe Reader succeeded.");
    });

}

void Scenario1::ScenarioStartReadButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    create_task(CreateDefaultReaderObject()).then([this](void)
    {
        if (_reader != nullptr)
        {
            // after successful creation, claim the reader for exclusive use and enable it so that data reveived events are received.
            create_task(ClaimReader()).then([this](void)
            {
                if (_claimedReader)
                {
                    // It is always a good idea to have a release device requested event handler. If this event is not handled, there are chances of another app can 
                    // claim ownsership of the magnetic stripe reader.
                    _releaseDeviceRequestedToken = _claimedReader->ReleaseDeviceRequested::add(ref new EventHandler<ClaimedMagneticStripeReader^>(this, &Scenario1::OnReleaseDeviceRequested));

                    // after successfully claiming and enabling, attach the BankCardDataReceived event handler.
                    // Note: If the scanner is not enabled (i.e. EnableAsync not called), attaching the event handler will not be any useful because the API will not fire the event 
                    // if the claimedScanner has not beed Enabled
                    _bankCardDataReceivedToken =  _claimedReader->BankCardDataReceived::add(ref new TypedEventHandler<ClaimedMagneticStripeReader^, MagneticStripeReaderBankCardDataReceivedEventArgs^>(this, &Scenario1::OnBankCardDataReceived));
                    UpdateReaderStatusTextBlock("Attached the BankCardDataReceived Event handler..");

                    // Ask the API to decode the data by default. By setting this, API will decode the raw data from the magnetic stripe reader
                    _claimedReader->IsDecodeDataEnabled = true;

                    create_task(EnableReader()).then([this](void)
                    {
                        UpdateReaderStatusTextBlock("Ready to Swipe..");

                        // reset the button state
                        ScenarioEndReadButton->IsEnabled = true;
                        ScenarioStartReadButton->IsEnabled = false;

                    });
                }
            });
        }
    });
}

/// <summary>
/// Event handler for End button
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void Scenario1::ScenarioEndReadButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ResetTheScenarioState();
}

/// <summary>
/// Event handler for the Release Device Requested event fired when magnetic stripe reader receives Claim request from another application
/// </summary>
/// <param name="sender"></param>
/// <param name="args"> Contains the ClaimedMagneticStripeReader that is sending this request</param>
void Scenario1::OnReleaseDeviceRequested(Platform::Object ^sender, Windows::Devices::PointOfService::ClaimedMagneticStripeReader ^args)
{
    // let us retain the device always. If it is not retained, this exclusive claim will be lost.
    args->RetainDevice(); 
}

/// <summary>
/// Event handler for the BankCardDataReceived event fired when a bank card is read by the magnetic stripe reader 
/// </summary>
/// <param name="sender"></param>
/// <param name="args"> Contains the MagneticStripeReaderBankCardDataReceivedEventArgs which contains the data obtained in the swipe</param>
void Scenario1::OnBankCardDataReceived(Windows::Devices::PointOfService::ClaimedMagneticStripeReader^sender, Windows::Devices::PointOfService::MagneticStripeReaderBankCardDataReceivedEventArgs^args)
{
    // read the data and display
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler( 
        [this,args]() 
    {
        ScenarioOutputAccountNumber->Text = args->AccountNumber;
        ScenarioOutputExpirationDate->Text = args->ExpirationDate;
        ScenarioOutputFirstName->Text = args->FirstName;
        ScenarioOutputMiddleInitial->Text = args->MiddleInitial;
        ScenarioOutputServiceCode->Text = args->ServiceCode;
        ScenarioOutputSuffix->Text = args->Suffix;
        ScenarioOutputSurname->Text = args->Surname;
        ScenarioOutputTitle->Text = args->Title;
    }));

}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    ResetTheScenarioState();
}

/// <summary>
/// Invoked when this page is no longer displayed.
/// </summary>
/// <param name="e"></param>
void Scenario1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    ResetTheScenarioState();
}

/// <summary>
/// Reset the Scenario state
/// </summary>
void Scenario1::ResetTheScenarioState()
{
    if (_claimedReader != nullptr)
    {
        // Detach the datareceived event handler and releasedevicerequested event handler
        _claimedReader->BankCardDataReceived::remove(_bankCardDataReceivedToken);
        _claimedReader->ReleaseDeviceRequested::remove(_releaseDeviceRequestedToken);
        
        // release the Claimed Magnetic Stripe Reader and set to null
        delete _claimedReader;		
        _claimedReader = nullptr;
    }

    if (_reader != nullptr)
    {
        // release the Magnetic Stripe Reader and set to null
        delete _reader;
        _reader = nullptr;
    }

    // Reset the strings in the UI
    ReaderStatusText->Text = "Click the Start Receiving Data Button.";
    ScenarioOutputAccountNumber->Text = "No data";
    ScenarioOutputExpirationDate->Text = "No data";
    ScenarioOutputFirstName->Text = "No data";
    ScenarioOutputMiddleInitial->Text = "No data";
    ScenarioOutputServiceCode->Text = "No data";
    ScenarioOutputSuffix->Text = "No data";
    ScenarioOutputSurname->Text = "No data";
    ScenarioOutputTitle->Text = "No data";

    // reset the button state
    ScenarioEndReadButton->IsEnabled = false;
    ScenarioStartReadButton->IsEnabled = true;
}
