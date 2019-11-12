//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1_LoadMP3FileForAudioStream.xaml.cpp
// Implementation of the Scenario1_LoadMP3FileForAudioStream class
//

#include "pch.h"
#include "Scenario1_LoadMP3FileForAudioStream.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::MediaStreamSource;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage; 
using namespace Windows::Storage::FileProperties;
using namespace Windows::Storage::Streams; 
using namespace Windows::Storage::Pickers; 
using namespace Windows::Media::MediaProperties;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections;

Scenario1_LoadMP3FileForAudioStream::Scenario1_LoadMP3FileForAudioStream()
{
    InitializeComponent();

    // MP3 Framesize and length for Layer II and Layer III
    sampleSize = 1152; 
    sampleDuration.Duration = { 70 * 10000 };
}

concurrency::task<void> MediaStreamSource::Scenario1_LoadMP3FileForAudioStream::getMP3FileProperties()
{
    // get the common music properties of the input MP3 file

    create_task(inputMP3File->Properties->GetMusicPropertiesAsync()).then(
        [this](MusicProperties^ mp3FileProperties)
    {
        songDuration = mp3FileProperties->Duration;
        title = mp3FileProperties->Title;
    });

    // get the input MP3 encoding properties

    auto propertiesToRetrieve = ref new Vector<String^>();
    propertiesToRetrieve->Append(StringReference(L"System.Audio.SampleRate"));
    propertiesToRetrieve->Append(StringReference(L"System.Audio.ChannelCount"));
    propertiesToRetrieve->Append(StringReference(L"System.Audio.EncodingBitrate"));

    return create_task(inputMP3File->Properties->RetrievePropertiesAsync(propertiesToRetrieve)).then(
        [this](Windows::Foundation::Collections::IMap<String^, Object^>^ properties)
    {
        int count = properties->Size;
        auto propvalloolup = properties->Lookup(StringReference(L"System.Audio.SampleRate"));
        if (propvalloolup != nullptr)
        {
            nSampleRate = safe_cast<UINT32>(propvalloolup);
        }
        propvalloolup = properties->Lookup(StringReference(L"System.Audio.ChannelCount"));
        if (propvalloolup != nullptr)
        {
            nChannelCount = safe_cast<UINT32>(propvalloolup);
        }
        propvalloolup = properties->Lookup(StringReference(L"System.Audio.EncodingBitrate"));
        if (propvalloolup != nullptr)
        {
            nBitrate = safe_cast<UINT32>(propvalloolup);
        }
    });
}

void MediaStreamSource::Scenario1_LoadMP3FileForAudioStream::InitializeMediaStreamSource()
{
    byteOffset = 0;
    timeOffset.Duration = { 0 };
    songDuration.Duration = { 0 };
    mssStream = nullptr;

    // get the MP3 file properties
    create_task(getMP3FileProperties()).then(
        [this]()
    {

        // creating the AudioEncodingProperties for the MP3 file

        AudioEncodingProperties^ audioProperties = AudioEncodingProperties::CreateMp3(nSampleRate, nChannelCount, nBitrate);

        // creating the AudioStreamDescriptor for the MP3 file

        Windows::Media::Core::AudioStreamDescriptor^ audioDescriptor = ref new Windows::Media::Core::AudioStreamDescriptor(audioProperties);

        // creating the MediaStreamSource for the MP3 file

        MSS = ref new Windows::Media::Core::MediaStreamSource(audioDescriptor);
        MSS->CanSeek = true;
        MSS->MusicProperties->Title = title;
        MSS->Duration = songDuration;

        // hooking up the MediaStreamSource event handlers

        startingRequestedToken = MSS->Starting += ref new TypedEventHandler<Windows::Media::Core::MediaStreamSource ^, Windows::Media::Core::MediaStreamSourceStartingEventArgs ^>(this, &Scenario1_LoadMP3FileForAudioStream::OnStarting);
        sampleRequestedToken = MSS->SampleRequested += ref new TypedEventHandler<Windows::Media::Core::MediaStreamSource ^, Windows::Media::Core::MediaStreamSourceSampleRequestedEventArgs ^>(this, &Scenario1_LoadMP3FileForAudioStream::OnSampleRequested);
        closedRequestedToken = MSS->Closed += ref new TypedEventHandler<Windows::Media::Core::MediaStreamSource ^, Windows::Media::Core::MediaStreamSourceClosedEventArgs ^>(this, &Scenario1_LoadMP3FileForAudioStream::OnClosed);

        // set the MediaStreamSource to MediaElement

        mediaPlayer->SetMediaStreamSource(MSS);

    });
}

void MediaStreamSource::Scenario1_LoadMP3FileForAudioStream::OnStarting(Windows::Media::Core::MediaStreamSource ^sender, Windows::Media::Core::MediaStreamSourceStartingEventArgs ^args)
{

    Windows::Media::Core::MediaStreamSourceStartingRequest^ request = args->Request;

    if ((request->StartPosition) && (request->StartPosition->Value.Duration <= MSS->Duration.Duration))
    {
        UINT64 sampleOffset = (UINT64)request->StartPosition->Value.Duration / (UINT64)sampleDuration.Duration;
        timeOffset.Duration = sampleOffset * sampleDuration.Duration;
        byteOffset = sampleOffset * sampleSize;
    }

    // create the RandomAccessStream for the input file for the first time 

    if (!mssStream)
    {
        Windows::Media::Core::MediaStreamSourceStartingRequestDeferral^ deferal = request->GetDeferral();

        try
        {
            create_task(inputMP3File->OpenAsync(FileAccessMode::Read)).then(
                [this, request, deferal](task<IRandomAccessStream^> stream)
            {
                mssStream = stream.get();
                request->SetActualStartPosition(timeOffset);
                deferal->Complete();
            });
        }
        catch(Exception^ e)
        {
            MSS->NotifyError(Windows::Media::Core::MediaStreamSourceErrorStatus::FailedToOpenFile);
            deferal->Complete();
        }
    }
    else
    {
        request->SetActualStartPosition(timeOffset);
    }

}

void MediaStreamSource::Scenario1_LoadMP3FileForAudioStream::OnSampleRequested(Windows::Media::Core::MediaStreamSource ^sender, Windows::Media::Core::MediaStreamSourceSampleRequestedEventArgs ^args)
{
    Windows::Media::Core::MediaStreamSourceSampleRequest^ request = args->Request;

    // check if the sample requested byte offset is within the file size

    if (byteOffset + sampleSize <= mssStream->Size)
    {
        Windows::Media::Core::MediaStreamSourceSampleRequestDeferral^ deferal = request->GetDeferral();

        IInputStream^ inputStream = mssStream->GetInputStreamAt(byteOffset);

        // create the MediaStreamSample and assign to the request object. 
        // You could also create the MediaStreamSample using createFromBuffer(...)

        create_task(Windows::Media::Core::MediaStreamSample::CreateFromStreamAsync(inputStream, sampleSize, timeOffset)).then(
            [this, request, deferal](Windows::Media::Core::MediaStreamSample^ sample)
        {
            sample->Duration = sampleDuration;
            sample->KeyFrame = true;

            // increment the time and byte offset

            byteOffset += sampleSize;
            timeOffset.Duration = timeOffset.Duration + sampleDuration.Duration;
            request->Sample = sample;
            deferal->Complete();
        });
    }
}

void MediaStreamSource::Scenario1_LoadMP3FileForAudioStream::OnClosed(Windows::Media::Core::MediaStreamSource ^sender, Windows::Media::Core::MediaStreamSourceClosedEventArgs ^args)
{
    // close the MediaStreamSource and remove the MediaStreamSource event handlers

    if (mssStream)
    {
        mssStream = nullptr;
    }

    if (sender == MSS) 
    {
        MSS->Starting -= startingRequestedToken;
        MSS->SampleRequested -= sampleRequestedToken;
        MSS->Closed -= closedRequestedToken;

        MSS = nullptr; 
    }
}

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
void MediaStreamSource::Scenario1_LoadMP3FileForAudioStream::PickMP3_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        FileOpenPicker^ filePicker = ref new FileOpenPicker();
        filePicker->SuggestedStartLocation = PickerLocationId::MusicLibrary;
        filePicker->FileTypeFilter->Append(".mp3");
        filePicker->ViewMode = PickerViewMode::List;
        filePicker->ContinuationData->Insert(StringReference(L"Operation"), StringReference(L"OpenMP3File"));
        filePicker->PickSingleFileAndContinue();
    }
}

void MediaStreamSource::Scenario1_LoadMP3FileForAudioStream::ContinueFileOpenPicker(Windows::ApplicationModel::Activation::FileOpenPickerContinuationEventArgs^ args)
{
    if (dynamic_cast<Platform::String^>(args->ContinuationData->Lookup(L"Operation")) == StringReference(L"OpenMP3File"))
    {
        StorageFile^ localMP3 = nullptr;
        if (args->Files->Size > 0)
        {
            localMP3 = args->Files->GetAt(0);
        }
        
        if (localMP3)
        {
            inputMP3File = localMP3;
            // Initialize the MediaStreamSource and set it to MediaElement

            InitializeMediaStreamSource();

            mediaPlayer->Play();

            MainPage::Current->NotifyUser("Playing MP3 using MediaStreamSource", NotifyType::StatusMessage);
        }
        else
        {
            MainPage::Current->NotifyUser("No file selected or file is invalid", NotifyType::ErrorMessage);
        }
    }
}

#else
void MediaStreamSource::Scenario1_LoadMP3FileForAudioStream::PickMP3_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        FileOpenPicker^ filePicker = ref new FileOpenPicker();
        filePicker->SuggestedStartLocation = PickerLocationId::MusicLibrary;
        filePicker->FileTypeFilter->Append(".mp3");
        filePicker->ViewMode = PickerViewMode::List;

        task<StorageFile^>(filePicker->PickSingleFileAsync()).then(
            [this](StorageFile^ localMP3)
        {
            if (localMP3)
            {
                inputMP3File = localMP3;

                // Initialize the MediaStreamSource and set it to MediaElement

                InitializeMediaStreamSource();

                mediaPlayer->Play();

                MainPage::Current->NotifyUser("Playing MP3 using MediaStreamSource", NotifyType::StatusMessage);
            }
            else
                MainPage::Current->NotifyUser("File is invalid", NotifyType::ErrorMessage);
        });
    }
}

#endif


void MediaStreamSource::Scenario1_LoadMP3FileForAudioStream::playButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        mediaPlayer->Play();
        MainPage::Current->NotifyUser("Playing the selected MP3 file", NotifyType::StatusMessage);
    }
}

void MediaStreamSource::Scenario1_LoadMP3FileForAudioStream::pauseButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        mediaPlayer->Pause();
        MainPage::Current->NotifyUser("Pausing the MP3 playback", NotifyType::StatusMessage);
    }
}
