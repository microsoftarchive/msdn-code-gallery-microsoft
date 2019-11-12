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
using namespace SDKSample::BarcodeScannerCPP;

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
/// Creates the default barcode scanner.
/// </summary>
task<void> Scenario1::CreateDefaultScannerObject()
{
    return create_task(BarcodeScanner::GetDefaultAsync()).then([this] (BarcodeScanner^ _scanner)
    {
        this->scanner = _scanner;
        if (this->scanner != nullptr)
        {			
            UpdateOutput("Default Barcode Scanner created.");
            UpdateOutput("Device Id is:" + this->scanner->DeviceId);
        }
        else
        {
            UpdateOutput("Barcode Scanner not found. Please connect a Barcode Scanner.");
        }
    });

}

/// <summary>
/// Update the status in the UI with the string passed.
/// </summary>
/// <param name="message"></param>
void Scenario1::UpdateOutput(Platform::String^ strMessage)
{
   StatusBlock->Text +=  strMessage;
   StatusBlock->Text +=  "\r";
}

/// <summary>
/// This method claims the barcode scanner
/// </summary>
task<void> Scenario1::ClaimScanner()
{
    // claim the barcode scanner
    return create_task(scanner->ClaimScannerAsync()).then([this] (ClaimedBarcodeScanner^ _claimedScanner)
    {
        this->claimedScanner = _claimedScanner;
        // enable the claimed barcode scanner
        if (claimedScanner != nullptr)
        {
            UpdateOutput("Claim Barcode Scanner succeeded.");		
        }
        else
        {
            UpdateOutput("Claim Barcode Scanner failed.");  
        }
    });
}

/// <summary>
/// This method enables the barcode scanner so that datareceieved event is received.
/// </summary>
task<void> Scenario1::EnableScanner()
{
    // enable the barcode scanner
    return create_task(claimedScanner->EnableAsync()).then([this](void)
    {
        UpdateOutput("Enable Barcode Scanner succeeded.");
    });
}	

 // <summary>
/// Setup the barcode scanner to be ready to receive the data events from the scan.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void Scenario1::ScenarioStartScanButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
     // create the barcode scanner. 
    create_task(CreateDefaultScannerObject()).then([this](void)
    {
        if (scanner != nullptr)
        {
            // after successful creation, claim the scanner for exclusive use and enable it so that data reveived events are received.
            create_task(ClaimScanner()).then([this](void)
            {
                if (claimedScanner)
                {
                        
                    // It is always a good idea to have a release device requested event handler. If this event is not handled, there are chances of another app can 
                    // claim ownsership of the barcode scanner.
                    releaseDeviceRequestedToken = claimedScanner->ReleaseDeviceRequested::add(ref new EventHandler<ClaimedBarcodeScanner^>(this, &Scenario1::OnReleaseDeviceRequested));

                    // after successfully claiming, attach the datareceived event handler.
                    dataReceivedToken =  claimedScanner->DataReceived::add(ref new TypedEventHandler<ClaimedBarcodeScanner^, BarcodeScannerDataReceivedEventArgs^>(this, &Scenario1::OnDataReceived));
                    UpdateOutput("Attached the DataReceived Event handler.");

                    // Ask the API to decode the data by default. By setting this, API will decode the raw data from the barcode scanner and 
                    // send the ScanDataLabel and ScanDataType in the DataReceived event
                    claimedScanner->IsDecodeDataEnabled = true;

                    // enable the scanner.
                    // Note: If the scanner is not enabled (i.e. EnableAsync not called), attaching the event handler will not be any useful because the API will not fire the event 
                    // if the claimedScanner has not beed Enabled
                    create_task(EnableScanner()).then([this](void)
                    {
                        UpdateOutput("Ready to Scan.");

                        // reset the button state
                        ScenarioEndScanButton->IsEnabled = true;
                        ScenarioStartScanButton->IsEnabled = false;
                    });
                }
            });
        }
    });

}

/// <summary>
/// Event handler for the Release Device Requested event fired when barcode scanner receives Claim request from another application
/// </summary>
/// <param name="sender"></param>
/// <param name="args"> Contains the ClamiedBarcodeScanner that is sending this request</param>
void Scenario1::OnReleaseDeviceRequested(Platform::Object ^sender, Windows::Devices::PointOfService::ClaimedBarcodeScanner ^args)
{
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler( 
        [this,args]() 
    {
        // let us retain the device always. If it is not retained, this exclusive claim will be lost.
        args->RetainDevice(); 
        UpdateOutput("Event ReleaseDeviceRequest received. Retaining the Barcode Scanner.");
    }));
}

/// <summary>
/// Event handler for the DataReceived event fired when a barcode is scanned by the barcode scanner 
/// </summary>
/// <param name="sender"></param>
/// <param name="args"> Contains the BarcodeScannerReport which contains the data obtained in the scan</param>
void Scenario1::OnDataReceived(Windows::Devices::PointOfService::ClaimedBarcodeScanner ^sender, Windows::Devices::PointOfService::BarcodeScannerDataReceivedEventArgs ^args)
{
    // read the data from the buffer and convert to string.
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler( 
        [this,args]() 
    {
        ScenarioOutputScanDataLabel->Text = GetDataLabelString(args->Report->ScanDataLabel, args->Report->ScanDataType);

        ScenarioOutputScanData->Text = GetDataString(args->Report->ScanData);

        ScenarioOutputScanDataType->Text = BarcodeSymbologies::GetName(args->Report->ScanDataType);
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
/// Event handler for End Scan Button Click. 
/// Releases the Barcode Scanner and resets the text in the UI
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void Scenario1::ScenarioEndScanButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ResetTheScenarioState();
}

/// <summary>
/// Reset the Scenario state
/// </summary>
void Scenario1::ResetTheScenarioState()
{
    if (claimedScanner != nullptr)
    {
        // Detach the event handlers
        claimedScanner->DataReceived::remove(dataReceivedToken);
        claimedScanner->ReleaseDeviceRequested::remove(releaseDeviceRequestedToken);
        
        // release the Barcode Scanner and set to null
        delete claimedScanner;		
        claimedScanner = nullptr;
    }

    if (scanner != nullptr)
    {
        delete scanner;
        scanner = nullptr;
    }

    // Reset the strings in the UI
    UpdateOutput("Click the Start Scanning Button.");
    ScenarioOutputScanData->Text = "No data";
    ScenarioOutputScanDataLabel->Text = "No data";
    ScenarioOutputScanDataType->Text = "No data";

    // reset the button state
    ScenarioEndScanButton->IsEnabled = false;
    ScenarioStartScanButton->IsEnabled = true;
}
