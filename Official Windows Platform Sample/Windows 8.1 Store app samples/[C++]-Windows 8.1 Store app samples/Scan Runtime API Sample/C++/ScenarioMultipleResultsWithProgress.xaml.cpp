//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ScenarioMultipleResultsWithProgress.xaml.cpp
// Implementation of the ScenarioMultipleResultsWithProgress class
//

#include "pch.h"
#include "ScenarioMultipleResultsWithProgress.xaml.h"
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
using namespace Windows::Foundation::Collections;

/// <summary>
/// Constructor
/// </summary>
ScenarioMultipleResultsWithProgress::ScenarioMultipleResultsWithProgress()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e"></param>
void ScenarioMultipleResultsWithProgress::OnNavigatedTo(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
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
void ScenarioMultipleResultsWithProgress::OnNavigatedFrom(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
    // If a scan job is in progress, cancel it now
    CancelScanning();
}

/// <summary>
/// Even Handler for click on Start Scenario button. Starts the scenario for scanning from the Feeder and getting multiple results with progress
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void ScenarioMultipleResultsWithProgress::StartScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        DisplayImage->Source = nullptr;
        ModelDataContext->ClearFileList();

        MainPage::Current->NotifyUser("Starting scenario of scanning from Feeder and getting multiple results with progress.", NotifyType::StatusMessage);		

        ModelDataContext->ScenarioRunning = true;
        ScanToFolder(ModelDataContext->ScannerDataContext->CurrentScannerDeviceId,  ModelDataContext->DestinationFolder);
    }
}

/// <summary>
/// Event handler for click on  Cancel Scenario button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void ScenarioMultipleResultsWithProgress::CancelScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e)
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
void ScenarioMultipleResultsWithProgress::CancelScanning()
{
    if (ModelDataContext->ScenarioRunning)
    {
        cancellationToken.cancel();
        DisplayImage->Source = nullptr;
        ModelDataContext->ScenarioRunning = false;
        ModelDataContext->ClearFileList();
    }
}


/// <summary>
/// Event Handler for progress of scanning 
/// </summary>
/// <param name="operation">Async operation containing the results</param>
/// <param name="numberOfScannedFiles">Number of files scanned so far</param>
void ScenarioMultipleResultsWithProgress::Progress(_In_ IAsyncOperationWithProgress<ImageScannerScanResult^, UINT32>^ operation, UINT32 numberOfScannedFiles)
{
    ImageScannerScanResult^ result = nullptr;
    try
    {
        result = operation->GetResults();
    }
    catch (task_canceled)
    {
        // The try catch is placed here for scenarios in which operation has already been cancelled when progress call is made
        Utils::DisplayScanCancelationMessage();
    }    

    if (result && result->ScannedFiles->Size > 0)
    {
        IVectorView<StorageFile ^>^ fileStorageList = result->ScannedFiles;							

        MainPage::Current->Dispatcher->RunAsync(
            CoreDispatcherPriority::Normal,
            ref new DispatchedHandler(
                [this, numberOfScannedFiles, fileStorageList] () {		
                    
                    StorageFile^ file =  fileStorageList->GetAt(0);

                    MainPage::Current->NotifyUser("Scanning is in progress. The number of files scanned so far: " + numberOfScannedFiles + ". Below is the latest scanned image. \n" +
                        "All the files that have been scanned are saved to local My Pictures folder.", NotifyType::StatusMessage);

                    Utils::SetImageSourceFromFile(file, DisplayImage);
                    Utils::UpdateFileListData(fileStorageList, ModelDataContext);
                }
            )
        );
    }
}


/// <summary>
/// Scans all the images from Feeder source of the scanner
/// The scanning is allowed only if the selected scanner is equipped with a Feeder
/// <summary>
/// <param name="deviceId">scanner device id</param>
/// <param name="destinationFolder">the folder that receives the scanned files</param>

void ScenarioMultipleResultsWithProgress::ScanToFolder(_In_ Platform::String^ deviceId, _In_ StorageFolder^ destinationFolder)
{
    // Get the scanner object for this device id
    create_task(ImageScanner::FromIdAsync(deviceId)).then([this, destinationFolder](ImageScanner^ myScanner)
    {
        // Check to see if the user has already canceled the scenario
        if (ModelDataContext->ScenarioRunning)
        {
            // Check if the feeder is supported by the given scanner
            if (myScanner->IsScanSourceSupported(ImageScannerScanSource::Feeder))
            {
                // Set MaxNumberOfPages to zero to scanning all the pages that are present in the feeder
                myScanner->FeederConfiguration->MaxNumberOfPages = 0;
                // Scan API call to start scanning from scanning from the Feeder source of the scanner.
                auto progress = ref new AsyncOperationProgressHandler<ImageScannerScanResult^, UINT32>(this, &SDKSample::ScanRuntimeAPI::ScenarioMultipleResultsWithProgress::Progress);
        
                // Scan API call to start scanning from the Feeder source of the scanner.
                IAsyncOperationWithProgress<ImageScannerScanResult^, UINT32>^ asyncOp = myScanner->ScanFilesToFolderAsync(ImageScannerScanSource::Feeder, destinationFolder);
                asyncOp->Progress = progress;
                    
                MainPage::Current->NotifyUser("Scanning", NotifyType::StatusMessage);	

                cancellationToken = Concurrency::cancellation_token_source();

                return create_task(asyncOp, cancellationToken.get_token());
            }
            else
            {
                ModelDataContext->ScenarioRunning = false;
                MainPage::Current->NotifyUser("The selected scanner does not report to be equipped with a Feeder.", NotifyType::ErrorMessage);
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
            // Check for nullptr to prevent scanning in cases of cancellation and not equipped with feeder scenarios
            if (result != nullptr)
            {
                // Number of scanned files should be zero here since we already processed during scan progress notifications all the files that have been scanned
                MainPage::Current->NotifyUser("Scanning is complete.", NotifyType::StatusMessage);	
                ModelDataContext->ScenarioRunning = false;
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


