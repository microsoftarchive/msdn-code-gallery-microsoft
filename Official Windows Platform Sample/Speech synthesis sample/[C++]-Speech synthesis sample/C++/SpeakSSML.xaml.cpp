//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "SpeakSSML.xaml.h"
#include "MainPage.xaml.h"


using namespace SDKSample;
using namespace SDKSample::VoiceSynthesisCPP;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Platform;
using namespace Platform::Collections;
using namespace Concurrency;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::Storage::BulkAccess;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Media::SpeechSynthesis;

Scenario2::Scenario2()
{
    InitializeComponent();
    //creating a new speech synthesizer object 
    this->synthesizer = ref new Windows::Media::SpeechSynthesis::SpeechSynthesizer();
    //loads voices into the listbox for user to choose from
    this->ListboxVoiceChooser_LoadVoices();

    this->UpdateSSMLText();
}

//this function handles the click on the speak button. Input SSML is converted to stream and then spoken via a media element
void VoiceSynthesisCPP::Scenario2::BtnSpeak_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    HRESULT hr = S_OK;
    String^ text = this->tbData->Text;
    this->btnSpeak->IsEnabled = false;

    try
    {
        auto speakTask = this->GetSpeechStreamTask(text);
        speakTask.then([this, text](SpeechSynthesisStream ^speechStream)
        {
            // start this audio stream playing
            this->media->SetSource(speechStream, speechStream->ContentType);
            this->media->AutoPlay = true;
            this->media->Play();
        });
    }
    catch (Exception ^ex)
    {
        hr = ex->HResult;
    }

    // we can't use the showAsync method from inside the exception
    // so check here to see if the exception was thrown (hr was set) and if so show the user message
    if (FAILED(hr))
    {
        this->btnSpeak->IsEnabled = true;
        ShowFailureMessage(hr);
    }
}

//if media state changes to stop or pause, re-enable the speak button for audio control
void VoiceSynthesisCPP::Scenario2::media_CurrentStateChanged(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    MediaElement ^element = (MediaElement ^)sender;

    if (element->CurrentState == MediaElementState::Stopped ||
        element->CurrentState == MediaElementState::Paused)
    {
        this->btnSpeak->IsEnabled = true;
    }
}
//actual creation of the stream using the new voice synthesis stream
task<SpeechSynthesisStream ^> VoiceSynthesisCPP::Scenario2::GetSpeechStreamTask(String ^ text)
{
    task<SpeechSynthesisStream ^>markersStream;
    markersStream = create_task(this->synthesizer->SynthesizeSsmlToStreamAsync(text));
    return markersStream;
}
//implementing the saving to file functionality 
void VoiceSynthesisCPP::Scenario2::BtnSaveToFile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    String^ text = this->tbData->Text;

    // select the file to save this data to
    FileSavePicker ^savePicker = ref new FileSavePicker();
    savePicker->DefaultFileExtension = ".wav";
    String^ FileTypeName = "Audio File";
    Vector<String^> ^extensions = ref new Vector<String ^>();
    extensions->Append(".wav");
    savePicker->FileTypeChoices->Insert(FileTypeName, extensions);

    create_task(savePicker->PickSaveFileAsync()).then([this, text](StorageFile ^file)
    {
        // check that the user picked a file.
        if (file != nullptr)
        {
            HRESULT hr = S_OK;
            String ^error = nullptr;

            try
            {
                create_task(this->GetSpeechStreamTask(text)).then([this, file](SpeechSynthesisStream ^markersStream)
                {
                    // open the output stream
                    create_task(file->OpenAsync(FileAccessMode::ReadWrite)).then([this, file, markersStream](IRandomAccessStream ^writeStream)
                    {
                        Windows::Storage::Streams::Buffer ^buffer = ref new Windows::Storage::Streams::Buffer(4096);
                        IOutputStream ^outputStream = writeStream->GetOutputStreamAt(0);
                        DataWriter ^dw = ref new DataWriter(outputStream);

                        // copy the stream data into the file
                        while (markersStream->Position < markersStream->Size)
                        {
                            create_task(markersStream->ReadAsync(buffer, 4096, InputStreamOptions::None)).wait();
                            dw->WriteBuffer(buffer);
                        }

                        create_task(dw->StoreAsync()).then([outputStream](size_t unused)
                        {
                            outputStream->FlushAsync();
                        });
                    });
                });
            }
            catch (Exception ^ex)
            {
                // we can't call the messageDialog->ShowAsync from inside an exception handler, so just
                // set HR to indicate failure
                hr = ex->HResult;
            }

            // check HR here to see if we need to display a user message indicating failure
            if (FAILED(hr))
            {
                ShowFailureMessage(hr);
            }
        }
    });
}
//listbox option changed, changes the voice for synthesis and speaking. This function implements the handling of the change
void VoiceSynthesisCPP::Scenario2::ListboxVoiceChooser_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
    ComboBoxItem ^item = (ComboBoxItem^)(this->listboxVoiceChooser->SelectedItem);
    if (nullptr != item)
    {
        VoiceInformation ^voice = (VoiceInformation^)item->Tag;
        this->synthesizer->Voice = voice;

        this->UpdateSSMLText();
    }
}
//SSML language tag needs to be updated to speak the language of users choice. this function handles the language update for ssml
void VoiceSynthesisCPP::Scenario2::UpdateSSMLText()
{
    try
    {
        String ^text = this->tbData->Text;
        String ^language = this->synthesizer->Voice->Language;
        //parse ssml into an xml doc and update language
        XmlDocument ^doc = ref new XmlDocument();
        doc->LoadXml(text);

        auto LangAttribute = doc->DocumentElement->GetAttributeNode("xml:lang");
        LangAttribute->InnerText = language;

        this->tbData->Text = doc->GetXml();
    }
    catch (Exception ^ex)
    {
        // this may happen if the user is currently editing the SSML, and it's not curretnly valid.
        // so don't fail, just can't update the SSML
    }
}
//listbox when double tapped behavior
void VoiceSynthesisCPP::Scenario2::ListboxVoiceChooser_DoubleTapped(Platform::Object^ sender, Windows::UI::Xaml::Input::DoubleTappedRoutedEventArgs^ e)
{
    return this->ListboxVoiceChooser_SelectionChanged(sender, nullptr);
}

void VoiceSynthesisCPP::Scenario2::ListboxVoiceChooser_LoadVoices()
{
    // get all of the installed voices
    IVectorView<VoiceInformation ^> ^voices = Windows::Media::SpeechSynthesis::SpeechSynthesizer::AllVoices;

    // get the currently selected voice
    VoiceInformation ^currentVoice = this->synthesizer->Voice;

    for (unsigned int index = 0; index < voices->Size; index++)
    {
        // create a combo box item, and copy the voice data into that
        VoiceInformation ^voice = voices->GetAt(index);
        ComboBoxItem ^item = ref new ComboBoxItem();
        item->Name = voice->DisplayName;
        item->Tag = voice;
        item->Content = voice->DisplayName;
        this->listboxVoiceChooser->Items->Append(item);

        // check to see if this is the current voice, so that we can set it to be selected
        if (currentVoice->Id == voice->Id)
        {
            item->IsSelected = true;
            this->listboxVoiceChooser->SelectedItem = item;
        }
    }
}
/// <summary>
/// displays a message to the user that the text they asked to synthesize has failed
/// </summary>
/// <remarks>
/// This will be called on an asynchronous thread. this attempts to collect
/// we will enable this button.
/// </remarks>
/// <param name="hr">the HRESULT that needs to be displayed</param>
void VoiceSynthesisCPP::Scenario2::ShowFailureMessage(HRESULT hr)
{
    WCHAR errMessage[1024];

    if (!FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM |  FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL,
        hr,
        MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // default language
        (LPWSTR) &errMessage,
        0,
        NULL))
    {
        // failed to format this message
        // so use a default
        StringCchPrintf(errMessage, 1024, L"Could not synthesize text, HRESULT : 0x%08X", hr);
    }

    StringReference message(errMessage);

    auto dialog = ref new Windows::UI::Popups::MessageDialog(message);
    dialog->ShowAsync();
}
