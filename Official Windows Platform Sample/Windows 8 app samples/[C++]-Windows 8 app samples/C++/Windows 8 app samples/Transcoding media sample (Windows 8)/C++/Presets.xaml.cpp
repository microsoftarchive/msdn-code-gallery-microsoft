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
// Presets.xaml.cpp
// Implementation of the Presets class
//

#include "pch.h"
#include "Presets.xaml.h"

using namespace SDKSample::Transcode;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage::Streams;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Media;
using namespace Windows::Media::MediaProperties;
using namespace Windows::Media::Transcoding;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;
using namespace concurrency;

Presets::Presets()
{
    InitializeComponent();

    // Hook up UI
    PickFileButton->Click += ref new RoutedEventHandler(this, &Presets::PickFile);
    TargetFormat->SelectionChanged += ref new SelectionChangedEventHandler(this, &Presets::OnTargetFormatChanged);
    Transcode->Click += ref new RoutedEventHandler(this, &Presets::TranscodePreset);
    Cancel->Click += ref new RoutedEventHandler(this, &Presets::TranscodeCancel);  

    // Media Controls
    InputPlayButton->Click += ref new RoutedEventHandler(this, &Presets::InputPlayButton_Click);
    InputPauseButton->Click += ref new RoutedEventHandler(this, &Presets::InputPauseButton_Click);
    InputStopButton->Click += ref new RoutedEventHandler(this, &Presets::InputStopButton_Click);
    OutputPlayButton->Click += ref new RoutedEventHandler(this, &Presets::OutputPlayButton_Click);
    OutputPauseButton->Click += ref new RoutedEventHandler(this, &Presets::OutputPauseButton_Click);
    OutputStopButton->Click += ref new RoutedEventHandler(this, &Presets::OutputStopButton_Click);

    // File is not selected, disable all buttons but PickFileButton
    DisableButtons();
    SetPickFileButton(true);
    SetCancelButton(false);

    // Initialize Objects
    _Transcoder = ref new MediaTranscoder();   
    _Profile = nullptr;
    _InputFile = nullptr;
    _OutputFile = nullptr;
    _OutputFileName = "TranscodeSampleOutput.mp4";
    _CTS = cancellation_token_source();

    _UseMp4 = true;
    
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Presets::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in Presets such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void Presets::GetPresetProfile(ComboBox^ comboBox)
{
    _Profile = nullptr;
    VideoEncodingQuality videoEncodingProfile = VideoEncodingQuality::Wvga;
    switch (comboBox->SelectedIndex)
    {
    case 0:
        videoEncodingProfile = VideoEncodingQuality::HD1080p;
        break;
    case 1:
        videoEncodingProfile = VideoEncodingQuality::HD720p;
        break;
    case 2:
        videoEncodingProfile = VideoEncodingQuality::Wvga;
        break;
    case 3:
        videoEncodingProfile = VideoEncodingQuality::Ntsc;
        break;
    case 4:
        videoEncodingProfile = VideoEncodingQuality::Pal;
        break;
    case 5:
        videoEncodingProfile = VideoEncodingQuality::Vga;
        break;
    case 6:
        videoEncodingProfile = VideoEncodingQuality::Qvga;
        break;
    }

    if(_UseMp4)
    {
        _Profile = MediaEncodingProfile::CreateMp4(videoEncodingProfile);
    }
    else
    {
        _Profile = MediaEncodingProfile::CreateWmv(videoEncodingProfile);
    }
}

void Presets::TranscodePreset(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    StopPlayers();
    DisableButtons();
    GetPresetProfile(ProfileSelect);

    // Clear messages
    StatusMessage->Text = "";

    try
    {
        if (_InputFile != nullptr)
        {
            auto videoLibrary = KnownFolders::VideosLibrary;
            create_task(videoLibrary->CreateFileAsync(_OutputFileName, CreationCollisionOption::GenerateUniqueName), _CTS.get_token()).then(
                [this](StorageFile^ destinationFile)
            {
                try
                {
                    _OutputFile = destinationFile;
                    return _Transcoder->PrepareFileTranscodeAsync(_InputFile, _OutputFile, _Profile);
                }
                catch (Platform::Exception^ exception)
                {
                    TranscodeError(exception->Message);
                }

                cancel_current_task();
            }).then(
                [this](PrepareTranscodeResult^ transcode)
            {
                try
                {
                    if(transcode->CanTranscode)
                    {
                        OutputMsg->Text = "";
                        SetCancelButton(true);
                        Windows::Foundation::IAsyncActionWithProgress<double>^ transcodeOp = transcode->TranscodeAsync();

                        // Set Progress Callback
                        transcodeOp->Progress = ref new AsyncActionProgressHandler<double>(
                        [this](IAsyncActionWithProgress<double>^ asyncInfo, double percent){
                            TranscodeProgress(asyncInfo, percent);
                        }, Platform::CallbackContext::Same);

                        return transcodeOp;
                    }
                    else
                    {
                        TranscodeFailure(transcode->FailureReason);
                    }
                }
                catch (Platform::Exception^ exception)
                {
                    TranscodeError(exception->Message);
                }

                cancel_current_task();
            }).then(
                [this](task<void> transcodeTask)
            {
                try
                {
                    transcodeTask.get();
                    OutputMsg->Text = "Transcode Completed.";
                    OutputPath->Text = "Output (" + _OutputFile->Path + ")";
                    PlayFile(_OutputFile);
                }
                catch (task_canceled&)
                {
                    OutputMsg->Foreground = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Red);
                    OutputMsg->Text = "Transcode Cancelled.";
                }
                catch(Exception^ exception)
                {
                    TranscodeError(exception->Message);
                }

                EnableButtons();
                SetCancelButton(false);
            });
        }
    }
    catch (Platform::Exception^ exception)
    {
        TranscodeError(exception->Message);
    }
}

void Presets::TranscodeCancel(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        _CTS.cancel();
        _CTS = cancellation_token_source();

        if(_OutputFile != nullptr)
        {
            create_task(_OutputFile->DeleteAsync()).then(
                [this](task<void> deleteTask)
            {
                try
                {
                    deleteTask.get();
                }
                catch (Platform::Exception^ exception)
                {
                    TranscodeError(exception->Message);
                }   
            });
        }
    }
    catch (Platform::Exception^ exception)
    {
        TranscodeError(exception->Message);
    }
}

void Presets::TranscodeProgress(IAsyncActionWithProgress<double>^ asyncInfo, double percent)
{
    OutputMsg->Foreground = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Green);
    OutputMsg->Text = "Progress:  " + ((int)percent).ToString() + "%";
}

void Presets::TranscodeError(Platform::String^ error)
{
    StatusMessage->Foreground = ref new Windows::UI::Xaml::Media::SolidColorBrush(Windows::UI::Colors::Red);
    StatusMessage->Text = error;
    EnableButtons();
    SetCancelButton(false); 
}

void Presets::TranscodeFailure(TranscodeFailureReason reason)
{
    try
    {
        if(_OutputFile != nullptr)
        {
            create_task(_OutputFile->DeleteAsync()).then(
                [this](task<void> deleteTask)
            {
                try
                {
                    deleteTask.get();
                }
                catch (Platform::Exception^ exception)
                {
                    TranscodeError(exception->Message);
                }   
            });
        }
    }
    catch(Platform::Exception^ exception)
    {
        TranscodeError(exception->Message);
    }

    switch (reason)
    {
    case TranscodeFailureReason::CodecNotFound:
        TranscodeError("Codec not found.");
        break;
    case TranscodeFailureReason::InvalidProfile:
        TranscodeError("Invalid profile.");
        break;
    default:
        TranscodeError("Unknown failure.");
    }
}

void Presets::SetCancelButton(bool isEnabled)
{
    Cancel->IsEnabled = isEnabled;
}

void Presets::EnableButtons()
{
    PickFileButton->IsEnabled = true;
    TargetFormat->IsEnabled = true;
    ProfileSelect->IsEnabled = true;
    Transcode->IsEnabled = true;
}

void Presets::DisableButtons()
{
    ProfileSelect->IsEnabled = false;
    Transcode->IsEnabled = false;
    PickFileButton->IsEnabled = false;
    TargetFormat->IsEnabled = false;
}

void Presets::PickFile(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
		Windows::UI::ViewManagement::ApplicationViewState currentState = Windows::UI::ViewManagement::ApplicationView::Value;
		if(currentState == Windows::UI::ViewManagement::ApplicationViewState::Snapped && !Windows::UI::ViewManagement::ApplicationView::TryUnsnap())
		{
			TranscodeError("Cannot pick files while application is in snapped view");
		}
		else
		{
			FileOpenPicker^ picker = ref new FileOpenPicker();
			picker->SuggestedStartLocation = PickerLocationId::VideosLibrary;
			picker->FileTypeFilter->Append(".wmv");
			picker->FileTypeFilter->Append(".mp4");

			create_task(picker->PickSingleFileAsync()).then(
				[this](StorageFile^ inputFile)
			{
				try
				{
					_InputFile = inputFile;
            
					if(_InputFile != nullptr)
					{
						return inputFile->OpenAsync(FileAccessMode::Read);
					}
				}
				catch (Platform::Exception^ exception)
				{
					TranscodeError(exception->Message);
				}

				cancel_current_task();
			}).then(
				[this](IRandomAccessStream^ inputStream)
			{
				try
				{
					InputVideo->SetSource(inputStream, _InputFile->ContentType);;
					InputVideo->Play();

					//Enable buttons
					EnableButtons();
				}
				catch (Platform::Exception^ exception)
				{
					TranscodeError(exception->Message);
				}
			});
		}
    }
    catch(Exception^ exception)
    {
        TranscodeError(exception->Message);
    }
}

void Presets::OnTargetFormatChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
    if (TargetFormat->SelectedIndex > 0)
    {
        _OutputFileName = "TranscodeSampleOutput.wmv";
        _UseMp4 = false;
    }
    else
    {
        _OutputFileName = "TranscodeSampleOutput.mp4";
        _UseMp4 = true;
    }
}

void Presets::InputPlayButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (InputVideo->DefaultPlaybackRate == 0)
    {
        InputVideo->DefaultPlaybackRate = 1.0;
        InputVideo->PlaybackRate = 1.0;
    }

    InputVideo->Play(); 
}

void Presets::InputStopButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    InputVideo->Stop();
}

void Presets::InputPauseButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    InputVideo->Pause();
}

void Presets::OutputPlayButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (OutputVideo->DefaultPlaybackRate == 0)
    {
        OutputVideo->DefaultPlaybackRate = 1.0;
        OutputVideo->PlaybackRate = 1.0;
    }

    OutputVideo->Play(); 
}

void Presets::OutputStopButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OutputVideo->Stop();
}

void Presets::OutputPauseButton_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OutputVideo->Pause();
}

void Presets::SetPickFileButton(bool isEnabled)
{
    PickFileButton->IsEnabled = isEnabled;
}


void Presets::StopPlayers()
{
    if(InputVideo->CurrentState != Media::MediaElementState::Paused)
    {
        InputVideo->Pause();
    }
    if(OutputVideo->CurrentState != Media::MediaElementState::Paused)
    {
        OutputVideo->Pause();
    }
}

void Presets::PlayFile(Windows::Storage::StorageFile^ MediaFile)
{
    try
    {
        create_task(MediaFile->OpenAsync(FileAccessMode::Read)).then(
            [&, this, MediaFile](IRandomAccessStream^ outputStream)
        {
            try
            {
                OutputVideo->SetSource(outputStream, MediaFile->ContentType);
                OutputVideo->Play();
            }
            catch (Platform::Exception^ exception)
            {
                TranscodeError(exception->Message);
            }
        });
    }
    catch (Exception^ exception)
    {
        TranscodeError(exception->Message);
    }
}
