//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ScenarioScanFromFlatbed.xaml.cpp
// Implementation of the ScenarioScanFromFlatbed class
//

#include "pch.h"
#include "ScenarioScanFromFlatbed.xaml.h"
#include "MainPage.xaml.h"
#include "Utils.h"

using namespace SDKSample;
using namespace SDKSample::ScanRuntimeAPI;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::Devices::Scanners;
using namespace concurrency;
using namespace Windows::Storage::Streams;
using namespace Windows::Storage;
using namespace Windows::UI::Xaml::Controls;

/// <summary>
/// Constructor
/// </summary>
ScenarioScanFromFlatbed::ScenarioScanFromFlatbed()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e"></param>
void ScenarioScanFromFlatbed::OnNavigatedTo(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
    // Start scanner watcher if it was not previously started
    if(!ModelDataContext->ScannerDataContext->WatcherStarted)
    {
        ModelDataContext->ScannerDataContext->StartScannerWatcher();
    }
}

/// <summary>
/// Invoked when user nagivates away from this page
/// </summary>
/// <param name="e"></param>
void ScenarioScanFromFlatbed::OnNavigatedFrom(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
    // If a scan job is in progress, cancel it now
    CancelScanning();
}

/// <summary>
/// Even Handler for click on Start Scenario button. Starts the scenario for scanning from the Faltbed
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void ScenarioScanFromFlatbed::StartScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        DisplayImage->Source = nullptr;
       
        MainPage::Current->NotifyUser("Starting scenario of scanning from Flatbed.", NotifyType::StatusMessage);		
        // Making scenario running as true before start of scenario
        ModelDataContext->ScenarioRunning = true;
        ScanToFolder(ModelDataContext->ScannerDataContext->CurrentScannerDeviceId,  ModelDataContext->DestinationFolder);
    }
}

/// <summary>
/// Event handler for click on  Cancel Scenario button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void ScenarioScanFromFlatbed::CancelScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        CancelScanning();
    }
}

/// <summary>
/// Cancels the current scanning task.
/// </summary>
void ScenarioScanFromFlatbed::CancelScanning()
{
    if (ModelDataContext->ScenarioRunning)
    {
        cancellationToken.cancel();        
        DisplayImage->Source = nullptr;
        ModelDataContext->ScenarioRunning = false;        
    }
}

/// <summary>
/// Scans image from the Flatbed source of the scanner
/// The scanning is allowed only if the selected scanner is equipped with a Flatbed
/// </summary>
/// <param name="deviceId">scanner device id</param>
/// <param name="destinationFolder">the folder that receives the scanned files</param>
void ScenarioScanFromFlatbed::ScanToFolder(_In_ Platform::String^ deviceId, _In_ StorageFolder^ destinationFolder)
{
    
    // Get the scanner object for this device id
    create_task(ImageScanner::FromIdAsync(deviceId)).then([this, destinationFolder](ImageScanner^ myScanner)
    {
        // Check to see if the use has already cancelled the scenario
        if (ModelDataContext->ScenarioRunning)
        {
            if (myScanner->IsScanSourceSupported(ImageScannerScanSource::Flatbed))
            {
                // Set the scan file format to Device Independent Bitmap (DIB)
                myScanner->FlatbedConfiguration->Format = ImageScannerFormat::DeviceIndependentBitmap;
                // Scan API call to start scanning from the Flatbed source of the scanner.
                IAsyncOperationWithProgress<ImageScannerScanResult^, UINT32>^ asyncOp = myScanner->ScanFilesToFolderAsync(ImageScannerScanSource::Flatbed, destinationFolder);

                MainPage::Current->NotifyUser("Scanning", NotifyType::StatusMessage);	
                cancellationToken = Concurrency::cancellation_token_source();
                return create_task(asyncOp, cancellationToken.get_token());
            }
            else
            {
                ModelDataContext->ScenarioRunning = false;
                MainPage::Current->NotifyUser("The selected scanner does not report to be equipped with a Flatbed.", NotifyType::ErrorMessage);		
                return create_task([]()->ImageScannerScanResult^ { return nullptr;});	
            }
        }
        else
        {
            // Scenario has already been canceled; return nullptr so no further action is possible 
            return create_task([]()->ImageScannerScanResult^ { return nullptr;});	
        }
    }).then([this](task<ImageScannerScanResult^> resultTask)
    {
        try
        {
            ImageScannerScanResult^ result = resultTask.get();
            // Check for nullptr to prevent scanning in cases of cancellation and scanner not equipped with flatbed scenarios
            if (result != nullptr)
            {
                ModelDataContext->ScenarioRunning = false;
                if (result->ScannedFiles->Size > 0)
                {
                    Utils::DisplayImageAndScanCompleteMessage(result->ScannedFiles, DisplayImage);
                    if (result->ScannedFiles->Size > 1)
                    {
                        Utils::UpdateFileListData(result->ScannedFiles, ModelDataContext);
                    }
                }
                else
                {
                    MainPage::Current->NotifyUser("There are no files scanned from the Flatbed.", NotifyType::ErrorMessage);		
                }
            }
           
        }
        catch (task_canceled)
        {
            Utils::DisplayScanCancelationMessage();
        }
        catch (Platform::Exception^ e)
        {
            Utils::OnScenarioException(e, ModelDataContext);
        }                   
    });
}
