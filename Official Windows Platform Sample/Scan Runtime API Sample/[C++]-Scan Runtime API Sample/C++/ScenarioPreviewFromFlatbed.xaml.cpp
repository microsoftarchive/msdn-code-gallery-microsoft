//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ScenarioScanPreview.xaml.cpp
// Implementation of the ScenarioScanPreview class
//

#include "pch.h"
#include "ScenarioPreviewFromFlatbed.xaml.h"
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
ScenarioPreviewFromFlatbed::ScenarioPreviewFromFlatbed()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e"></param>
void ScenarioPreviewFromFlatbed::OnNavigatedTo(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ e)
{
    // Start scanner watcher if it was not previously started
    if(!ModelDataContext->ScannerDataContext->WatcherStarted)
    {
        ModelDataContext->ScannerDataContext->StartScannerWatcher();
    }
}

/// <summary>
/// Even Handler for click on Start Scenario button. Starts the scenario of getting the preview from Faltbed
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void ScenarioPreviewFromFlatbed::StartScenario_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        DisplayImage->Source = nullptr;
        
        MainPage::Current->NotifyUser("Starting scenario of preview scanning from Flatbed.", NotifyType::StatusMessage);

        auto stream = ref new InMemoryRandomAccessStream();        
        ScanPreview(ModelDataContext->ScannerDataContext->CurrentScannerDeviceId, stream);
    }
}

/// <summary>
/// Previews the image from the scanner with given device id
/// The preview is allowed only if the selected scanner is equipped with a Flatbed and supports preview.
/// </summary>
/// <param name="deviceId">scanner device id</param>
/// <param name="stream">RandomAccessStream in which preview given the by the scan runtime API is kept</param>
void ScenarioPreviewFromFlatbed::ScanPreview(_In_ String^ deviceId, _In_ IRandomAccessStream^ stream)
{
    // Get the scanner object for this device id
    create_task(ImageScanner::FromIdAsync(deviceId)).then([this, stream](ImageScanner^ myScanner)
    {      
        if (myScanner->IsScanSourceSupported(ImageScannerScanSource::Flatbed))
        {
            // Check to see if Preview is supported by the scanner
            if (myScanner->IsPreviewSupported(ImageScannerScanSource::Flatbed))
            {
                MainPage::Current->NotifyUser("Scanning", NotifyType::StatusMessage);	
                // Scan API call to get preview from the flatbed
                return create_task(myScanner->ScanPreviewToStreamAsync(ImageScannerScanSource::Flatbed, stream));
            }
            else 
            {
                MainPage::Current->NotifyUser("The selected scanner does not support preview from Flatbed", NotifyType::ErrorMessage);
                return create_task([]()->ImageScannerPreviewResult^ { return nullptr;});	
            }
        } 
        else 
        {
            MainPage::Current->NotifyUser("The selected scanner does not report to be equipped with a Flatbed.", NotifyType::ErrorMessage);
            return create_task([]()->ImageScannerPreviewResult^ { return nullptr;});	
        }
       
    }).then([this, stream](task<ImageScannerPreviewResult^> resultTask)
    {
        try
        {
            // Check for nullptr to prevent scanning in cases of scanner not equipped with flatbed scenarios
            ImageScannerPreviewResult^ result = resultTask.get();
            if (result != nullptr)
            {
                if (result->Succeeded) 
                {
                    Utils::SetImageSourceFromStream(stream, DisplayImage);
                    MainPage::Current->NotifyUser("Scanning is complete. Below is the preview image.", NotifyType::StatusMessage);		
                }
                else
                {
                    MainPage::Current->NotifyUser("Failed to preview from Flatbed.", NotifyType::ErrorMessage);		
                }
            }
        }
        catch (Platform::Exception^ e)
        {
            Utils::OnScenarioException(e, ModelDataContext);
        }                   
    });
}