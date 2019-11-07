//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// PhotoSequence.xaml.cpp
// Implementation of the PhotoSequence class
//

#include "pch.h"
#include "PhotoSequence.xaml.h"
#include "ppl.h"

using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::Storage;
using namespace Windows::Media;
using namespace Windows::Media::MediaProperties;
using namespace Windows::Media::Capture;
using namespace Windows::Storage::Streams;
using namespace Windows::System;
using namespace Windows::UI::Xaml::Media::Imaging;

using namespace SDKSample::MediaCapture;
using namespace concurrency;




PhotoSequence::PhotoSequence()
{
    InitializeComponent();
    ScenarioInit();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void PhotoSequence::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    SystemMediaTransportControls^ systemMediaControls = SystemMediaTransportControls::GetForCurrentView();
    m_eventRegistrationToken = systemMediaControls->PropertyChanged += ref new TypedEventHandler<SystemMediaTransportControls^, SystemMediaTransportControlsPropertyChangedEventArgs^>(this, &PhotoSequence::SystemMediaControlsPropertyChanged);
}

void PhotoSequence::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()

    SystemMediaTransportControls^ systemMediaControls = SystemMediaTransportControls::GetForCurrentView();
    systemMediaControls->PropertyChanged -= m_eventRegistrationToken;
    ScenarioClose();

}

void PhotoSequence::SystemMediaControlsPropertyChanged(SystemMediaTransportControls^ sender, SystemMediaTransportControlsPropertyChangedEventArgs^ e)
{
    switch (e->Property)
    {
    case SystemMediaTransportControlsProperty::SoundLevel:
        create_task(Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High, ref new Windows::UI::Core::DispatchedHandler([this, sender]()
        {
            if (sender->SoundLevel != Windows::Media::SoundLevel::Muted)
            {
                ScenarioInit();
            }
            else
            {
                ScenarioClose();
            }
        })));
        break;

    default:
        break;
    }
}


void  PhotoSequence::ScenarioInit()
{

    btnStartDevice4->IsEnabled = true;
    btnStartPreview4->IsEnabled = false;
    btnStartStopPhotoSequence->IsEnabled = false;
    btnStartStopPhotoSequence->Content = "Prepare PhotoSequence";
    btnSaveToFile->IsEnabled = false;

    m_bPhotoSequence = false;
    m_bPreviewing = false;

    previewElement4->Source = nullptr;

    m_framePtr = ref new Platform::Collections::Vector<Windows::Media::Capture::CapturedFrame^>();

    m_frameNum = 0;
    m_ThumbnailNum = 0;
    m_selectedIndex = -1;
    m_highLighted = false;

    Clear();

}

void  PhotoSequence::ScenarioClose()
{

    if (m_bPhotoSequence)
    {
        ShowStatusMessage("Stopping PhotoSequence");

        create_task(m_photoSequenceCapture->FinishAsync()).then([this](task<void> photoSequenceTask)
        {
            try
            {
                photoSequenceTask.get();
                m_photoSequenceCapture = nullptr;
            }
            catch (Exception ^e)
            {
                ShowExceptionMessage(e);
            }
        });
        m_bPhotoSequence = false;
        m_framePtr = nullptr;
    }

    ShowStatusMessage("Stopping preview and camera");
    try
    {
        if (m_capture.Get())
        {
            previewElement4->Source = nullptr;
            delete(m_capture.Get());
            m_bPreviewing = false;
        }
    }
    catch (Exception ^e)
    {
        ShowExceptionMessage(e);
    }
}



void PhotoSequence::Failed(Windows::Media::Capture::MediaCapture ^currentCaptureObject, Windows::Media::Capture::MediaCaptureFailedEventArgs^ currentFailure)
{
    String ^message = "Fatal error: " + currentFailure->Message;
    create_task(Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::High,
        ref new Windows::UI::Core::DispatchedHandler([this, message]()
    {
        ShowStatusMessage(message);
    })));
}

void PhotoSequence::btnStartDevice_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        btnStartDevice4->IsEnabled = false;
        ShowStatusMessage("Starting device");
        m_capture = ref new Windows::Media::Capture::MediaCapture();

        create_task(m_capture->InitializeAsync()).then([this](task<void> initTask)
        {
            try
            {
                initTask.get();

                auto mediaCapture = m_capture.Get();

                if (mediaCapture->MediaCaptureSettings->VideoDeviceId != nullptr)
                {
                    btnStartPreview4->IsEnabled = true;

                    ShowStatusMessage("Device initialized successful");

                    mediaCapture->Failed += ref new Windows::Media::Capture::MediaCaptureFailedEventHandler(this, &PhotoSequence::Failed);
                }
                else
                {
                    btnStartDevice4->IsEnabled = true;
                    ShowStatusMessage("No Video Device Found");
                }
            }
            catch (Exception ^ e)
            {
                btnStartPreview4->IsEnabled = false;
                btnStartDevice4->IsEnabled = true;
                ShowExceptionMessage(e);
            }
        }
        );
    }
    catch (Platform::Exception^ e)
    {
        btnStartPreview4->IsEnabled = false;
        btnStartDevice4->IsEnabled = true;
        ShowExceptionMessage(e);
    }
}

void PhotoSequence::btnStartPreview_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{

    try
    {
        ShowStatusMessage("Starting preview");

        btnStartPreview4->IsEnabled = false;
        btnStartStopPhotoSequence->IsEnabled = true;

        m_bPreviewing = true;

        previewCanvas4->Visibility = Windows::UI::Xaml::Visibility::Visible;
        previewElement4->Source = m_capture.Get();
        create_task(m_capture.Get()->StartPreviewAsync()).then([this](task<void> previewTask)
        {
            try
            {
                previewTask.get();
                auto mediaCapture = m_capture.Get();

                ShowStatusMessage("Start preview successful");
            }
            catch (Exception ^e)
            {

                previewElement4->Source = nullptr;
                btnStartPreview4->IsEnabled = true;
                btnStartStopPhotoSequence->IsEnabled = false;

                ShowExceptionMessage(e);
            }
        });
    }
    catch (Platform::Exception^ e)
    {

        previewElement4->Source = nullptr;
        btnStartPreview4->IsEnabled = true;
        btnStartStopPhotoSequence->IsEnabled = false;
        m_bPreviewing = false;
        ShowExceptionMessage(e);
    }
}


void PhotoSequence::ShowStatusMessage(Platform::String^ text)
{
    rootPage->NotifyUser(text, NotifyType::StatusMessage);
}

void PhotoSequence::ShowExceptionMessage(Platform::Exception^ ex)
{
    rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
}



void PhotoSequence::Clear()
{
    PhotoGrid->Source = nullptr;
    ThumbnailGrid->Items->Clear();
}




void PhotoSequence::btnStartStopPhotoSequence_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        if (safe_cast<String^>(btnStartStopPhotoSequence->Content) == "Prepare PhotoSequence")
        {
            //user can set the maximum history frame number
            m_pastFrame = 5;
            ////user can set the maximum future frame number
            m_futureFrame = 5;

            if (!m_capture->VideoDeviceController->LowLagPhotoSequence->Supported)
            {
                rootPage->NotifyUser("Photo-sequence is not supported", NotifyType::ErrorMessage);
            }
            else
            {
                if (m_capture->VideoDeviceController->LowLagPhotoSequence->MaxPastPhotos < safe_cast<unsigned int>(m_pastFrame) )
                {
                    m_pastFrame = m_capture->VideoDeviceController->LowLagPhotoSequence->MaxPastPhotos;
                    rootPage->NotifyUser("Photo-sequence histrory frame number exceeds limit", NotifyType::ErrorMessage);
                }

                btnStartStopPhotoSequence->IsEnabled = false;

                m_capture->VideoDeviceController->LowLagPhotoSequence->ThumbnailEnabled = true;
                m_capture->VideoDeviceController->LowLagPhotoSequence->DesiredThumbnailSize = 300;

                m_capture->VideoDeviceController->LowLagPhotoSequence->PhotosPerSecondLimit = 4;
                m_capture->VideoDeviceController->LowLagPhotoSequence->PastPhotoLimit = m_pastFrame;

                create_task(m_capture->PrepareLowLagPhotoSequenceCaptureAsync(ImageEncodingProperties::CreateJpeg())).then([this](LowLagPhotoSequenceCapture ^photoCapture)
                {
                    m_bPhotoSequence = true;

                    photoCapture->PhotoCaptured += ref new TypedEventHandler<LowLagPhotoSequenceCapture ^, PhotoCapturedEventArgs^> (
                        [this](LowLagPhotoSequenceCapture ^senders, PhotoCapturedEventArgs^ args)
                    {
                        create_task(Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, args]()
                        {
                            try
                            {
                                m_framePtr->Append(args->Frame);
                                auto bitmap = ref new BitmapImage();

                                if (m_frameNum == (m_pastFrame + m_futureFrame))
                                {
                                    btnStartStopPhotoSequence->IsEnabled = false;
                                    create_task(m_photoSequenceCapture->StopAsync()).then([this](task<void> StopTask)
                                    {
                                        try
                                        {
                                            StopTask.get();
                                            btnSaveToFile->IsEnabled = true;
                                            ThumbnailGrid->SelectedIndex = m_selectedIndex;
                                            btnStartStopPhotoSequence->IsEnabled = true;
                                        }
                                        catch (Exception ^e)
                                        {
                                            ShowExceptionMessage(e);
                                        }
                                    });
                                }
                                else if (m_frameNum < (m_pastFrame + m_futureFrame))
                                {
                                    bitmap->SetSource(args->Thumbnail);

                                    auto image = ref new Image();
                                    image->Source = bitmap;

                                    image->Width = 160;
                                    image->Height = 120;


                                    auto ThumbnailItem = ref new Windows::UI::Xaml::Controls::GridViewItem;
                                    ThumbnailItem->Content = image;

                                    ThumbnailGrid->Items->Append(ThumbnailItem);

                                    if ((!m_highLighted) && (args->CaptureTimeOffset.Duration > 0))
                                    {
                                        //first picture with timeSpam > 0 get highlighted 
                                        m_highLighted = true;
                                        ThumbnailItem->BorderThickness = 1;
                                        ThumbnailItem->BorderBrush = ref new SolidColorBrush(Colors::Red);
                                        m_selectedIndex = m_ThumbnailNum;
                                    }
                                    m_ThumbnailNum++;
                                }
                                m_frameNum++;
                            }
                            catch (Exception^ e)
                            {
                                ShowExceptionMessage(e);
                            }
                        })));
                    });

                    m_photoSequenceCapture = photoCapture;

                }).then([this]()
                {
                    btnStartStopPhotoSequence->Content = "Take PhotoSequence";
                    btnStartStopPhotoSequence->IsEnabled = true;
                });

            }
        }
        else if (safe_cast<String^>(btnStartStopPhotoSequence->Content) == "Take PhotoSequence")
        {

            btnSaveToFile->IsEnabled = false;
            m_frameNum = 0;
            m_ThumbnailNum = 0;
            m_selectedIndex = -1;
            m_highLighted = false;
            Clear();
            m_framePtr->Clear();

            btnStartStopPhotoSequence->IsEnabled = false;

            m_photoSequenceCapture->StartAsync();

        }
        else
        {
            rootPage->NotifyUser("Bad photo-sequence state", NotifyType::ErrorMessage);
        }
    }
    catch (Exception ^e)
    {
        ShowExceptionMessage(e);
    }
}


void PhotoSequence::ItemSelected_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    m_selectedIndex = ThumbnailGrid->SelectedIndex;

    if (m_selectedIndex > -1)
    {
        auto bitmap = ref new BitmapImage();
        try
        {
            create_task(bitmap->SetSourceAsync(m_framePtr->GetAt(m_selectedIndex)->CloneStream())).then([this, bitmap]()
            {
                PhotoGrid->Source = bitmap;
            });
        }
        catch (Exception ^e)
        {
            rootPage->NotifyUser("Display selected photo fail", NotifyType::ErrorMessage);
        }

    }
}

void PhotoSequence::btnSaveToFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{

    m_selectedIndex = ThumbnailGrid->SelectedIndex;

    if (m_selectedIndex > -1)
    {
        create_task(KnownFolders::PicturesLibrary->CreateFileAsync(PHOTOSEQ_FILE_NAME, CreationCollisionOption::GenerateUniqueName))
            .then([this](task<StorageFile^> file)
        {
            this->m_photoStorageFile = file.get();
            if (nullptr == m_photoStorageFile)
            {
                rootPage->NotifyUser("PhotoFile creation fails", NotifyType::ErrorMessage);
            }

            return this->m_photoStorageFile->OpenAsync(FileAccessMode::ReadWrite);
        }).then([this](task<IRandomAccessStream^> stream)
        {
            auto OutStream = stream.get();
            if (nullptr == OutStream)
            {
                rootPage->NotifyUser("PhotoStream creation fails", NotifyType::ErrorMessage);
            }

            auto ContentStream = m_framePtr->GetAt(m_selectedIndex)->CloneStream();

            RandomAccessStream::CopyAndCloseAsync(ContentStream->GetInputStreamAt(0), OutStream->GetOutputStreamAt(0));
        }).then([this]()
        {
            ShowStatusMessage("Photo save complete");
        });
    }
    else
    {
        rootPage->NotifyUser("Please select a photo to display", NotifyType::ErrorMessage);
    }

}




