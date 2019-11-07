// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario1_ExtractText.xaml.h"
#include <map>
#include <robuffer.h>
#include <ppltasks.h>
#include <vector>

using namespace OCR;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::Storage::FileProperties;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace WindowsPreview::Media::Ocr;
using namespace concurrency;
using namespace std;

ExtractText::ExtractText() : rootPage(MainPage::Current)
{
    InitializeComponent();

    ocrEngine = ref new OcrEngine(OcrLanguage::English);

    // Initialize language name mapping.
    languages[OcrLanguage::ChineseSimplified.ToString()]  = OcrLanguage::ChineseSimplified;
    languages[OcrLanguage::ChineseTraditional.ToString()] = OcrLanguage::ChineseTraditional;
    languages[OcrLanguage::Czech.ToString()]              = OcrLanguage::Czech;
    languages[OcrLanguage::Danish.ToString()]             = OcrLanguage::Danish;
    languages[OcrLanguage::Dutch.ToString()]              = OcrLanguage::Dutch;
    languages[OcrLanguage::English.ToString()]            = OcrLanguage::English;
    languages[OcrLanguage::Finnish.ToString()]            = OcrLanguage::Finnish;
    languages[OcrLanguage::French.ToString()]             = OcrLanguage::French;
    languages[OcrLanguage::German.ToString()]             = OcrLanguage::German;
    languages[OcrLanguage::Greek.ToString()]              = OcrLanguage::Greek;
    languages[OcrLanguage::Hungarian.ToString()]          = OcrLanguage::Hungarian;
    languages[OcrLanguage::Italian.ToString()]            = OcrLanguage::Italian;
    languages[OcrLanguage::Japanese.ToString()]           = OcrLanguage::Japanese;
    languages[OcrLanguage::Korean.ToString()]             = OcrLanguage::Korean;
    languages[OcrLanguage::Norwegian.ToString()]          = OcrLanguage::Norwegian;
    languages[OcrLanguage::Polish.ToString()]             = OcrLanguage::Polish;
    languages[OcrLanguage::Portuguese.ToString()]         = OcrLanguage::Portuguese;
    languages[OcrLanguage::Russian.ToString()]            = OcrLanguage::Russian;
    languages[OcrLanguage::Spanish.ToString()]            = OcrLanguage::Spanish;
    languages[OcrLanguage::Swedish.ToString()]            = OcrLanguage::Swedish;
    languages[OcrLanguage::Turkish.ToString()]            = OcrLanguage::Turkish;

    // Load all available languages from OcrLanguage enum in combo box.
    vector<Platform::String^> languageNames;
    for (auto lang : languages)
    {
        languageNames.push_back(lang.first);
    }
    LanguageList->ItemsSource = ref new Vector<Platform::String^>(languageNames.begin(), languageNames.end());
    LanguageList->SelectedItem = ocrEngine->Language.ToString();
    LanguageList->SelectionChanged += ref new SelectionChangedEventHandler(this, &ExtractText::LanguageList_SelectionChanged);
}

/// <summary>
/// This is selection changed handler for language list.
/// Tries to change language for Optical Character Recognition.
/// If language resources are not present reverts selected language.
/// Check MSDN docs or readme.txt in NuGet Package to learn how to produce 
/// resource file that contains language specific resources.
/// </summary>
void ExtractText::LanguageList_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
    String^ languageName = (String^)LanguageList->SelectedItem;

    try
    {
        // Try to set new language.
        ocrEngine->Language = languages[languageName];

        rootPage->NotifyUser(
            "OCR engine set to extract text in " + languageName + "language.",
            NotifyType::StatusMessage);

        ClearResults();
    }
    catch (InvalidArgumentException^)
    {
        auto replacedLanguage = e->RemovedItems->First()->Current->ToString();

        LanguageList->SelectedItem = replacedLanguage;

        rootPage->NotifyUser(
            "Resource file 'MsOcrRes.opr' does not contain required resources for " + languageName + " language. \n" +
            "Check MSDN docs or readme.txt in NuGet Package to learn how to produce resource file" +
            "that contains " + languageName + " language specific resources.\n" +
            "OCR language is now reverted to " + replacedLanguage + " language.",
            NotifyType::ErrorMessage);
    }
}

/// <summary>
/// Invoked when the user clicks on the Load button.
/// </summary>
void ExtractText::Load_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    FileOpenPicker^ picker = ref new FileOpenPicker();
    picker->FileTypeFilter->Append(".jpg");
    picker->FileTypeFilter->Append(".jpeg");
    picker->FileTypeFilter->Append(".png");
    picker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;

    // On Windows Phone, after the picker is launched the app is closed.
#if WINAPI_FAMILY==WINAPI_FAMILY_PHONE_APP

    picker->PickSingleFileAndContinue();

#else
    auto pickFileOp = picker->PickSingleFileAsync();
    create_task(pickFileOp).then([this](StorageFile^ file)
    {
        if (file != nullptr)
        {
            LoadImage(file);
        }
    });
#endif
}

#if WINAPI_FAMILY==WINAPI_FAMILY_PHONE_APP

/// <summary>
/// Handle the returned files from file picker
/// This method is triggered by ContinuationManager based on ActivationKind
/// </summary>
/// <param name="args">File open picker continuation activation argument. It contains the list of files user selected with file open picker </param>
void ExtractText::ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs^ args)
{
    if (args->Files->Size > 0)
    {
        LoadImage(args->Files->GetAt(0));
    }
}
#endif

/// <summary>
/// Invoked when the user clicks on the Sample button.
/// </summary>
void ExtractText::Sample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Load sample "Hello World" image.
    auto getFileOp = Windows::ApplicationModel::Package::Current->InstalledLocation->GetFileAsync("sample\\sample.png");
    create_task(getFileOp).then([this](StorageFile^ file)
    {
        LoadImage(file);
    });
}

/// <summary>
/// Loads image from file to bitmap and displays it in UI.
/// </summary>
void ExtractText::LoadImage(StorageFile^ file)
{
    auto getImgPropsOp = file->Properties->GetImagePropertiesAsync();

    create_task(getImgPropsOp).then([this, file](ImageProperties^ imgProp) -> IAsyncOperation<IRandomAccessStream^>^
    {
        bitmap = ref new WriteableBitmap(imgProp->Width, imgProp->Height);

        return file->OpenAsync(FileAccessMode::Read);

    }).then([this, file](IRandomAccessStream^ stream)
    {
        bitmap->SetSource(stream);

        PreviewImage->Source = bitmap;

        ClearResults();

        rootPage->NotifyUser(
            "Loaded image from file: " + file->Name + " (" + bitmap->PixelWidth + "x" + bitmap->PixelHeight + ").",
            NotifyType::StatusMessage);
    });
}

/// <summary>
/// Clears extracted text results.
/// Removes extracted text and text overlay.
/// </summary>
void ExtractText::ClearResults()
{
    // Retrieve initial state.
    PreviewImage->RenderTransform = nullptr;
    ImageText->Text = "Text not extracted.";
    ImageText->Style = (Windows::UI::Xaml::Style^) Application::Current->Resources->Lookup("YellowTextStyle");

    ExtractTextButton->IsEnabled = true;
    LanguageList->IsEnabled = true;

    // Clear text overlay from image.
    TextOverlay->Children->Clear();
}

/// <summary>
/// This is click handler for Extract Text button.
/// If image size is supported text is extracted and overlaid over displayed image.
/// Supported image dimensions are between 40 and 2600 pixels.
/// </summary>
void ExtractText::ExtractText_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Prevent another OCR request, since only image can be processed at the time at same OCR engine instance.
    ExtractTextButton->IsEnabled = false;

    // Check whether loaded image is supported for processing.
    // Supported image dimensions are between 40 and 2600 pixels.
    if (bitmap->PixelHeight < 40 ||
        bitmap->PixelHeight > 2600 ||
        bitmap->PixelWidth < 40 ||
        bitmap->PixelWidth > 2600)
    {
        ImageText->Text = "Image size is not supported.\n" +
            "Loaded image size is " + bitmap->PixelWidth + "x" + bitmap->PixelHeight + ".\n" +
            "Supported image dimensions are between 40 and 2600 pixels.";
        ImageText->Style = (Windows::UI::Xaml::Style^) Application::Current->Resources->Lookup("RedTextStyle");

        rootPage->NotifyUser(
            "OCR was attempted on image with unsupported size.\n" +
            "Supported image dimensions are between \n 40 and 2600 pixels.",
            NotifyType::ErrorMessage);

        return;
    }

    Platform::Array<byte>^ imageByteArray = ref new Platform::Array<byte>(GetPointerToPixelData(bitmap->PixelBuffer), bitmap->PixelBuffer->Length);

    // This is main API call to extract text from an image.
    auto recognizeOp = ocrEngine->RecognizeAsync((unsigned int)bitmap->PixelHeight, (unsigned int)bitmap->PixelWidth, imageByteArray);

    create_task(recognizeOp).then([this](OcrResult^ ocrResult)
    {
        // OCR result does not contain any lines, no text was recognized. 
        if (ocrResult->Lines != nullptr)
        {
            // Used for text overlay.
            // Prepare scale transform for words since image is not displayed in original format.
            ScaleTransform^ scaleTrasform = ref new ScaleTransform();
            scaleTrasform->CenterX = 0;
            scaleTrasform->CenterY = 0;
            scaleTrasform->ScaleX = PreviewImage->ActualWidth / bitmap->PixelWidth;
            scaleTrasform->ScaleY = PreviewImage->ActualHeight / bitmap->PixelHeight;

            if (ocrResult->TextAngle != nullptr)
            {
                double zxc = (double)ocrResult->TextAngle->Value;

                // If text is detected under some angle then
                // apply a transform rotate on image around center.
                RotateTransform^ rotateTransform = ref new RotateTransform();
                rotateTransform->Angle = (double)ocrResult->TextAngle->Value;
                rotateTransform->CenterX = PreviewImage->ActualWidth / 2;
                rotateTransform->CenterY = PreviewImage->ActualHeight / 2;
                PreviewImage->RenderTransform = rotateTransform;
            }

            String^ extractedText = "";

            // Iterate over recognized lines of text.
            for (auto line : ocrResult->Lines)
            {
                // Iterate over words in line.
                for (auto word : line->Words)
                {
                    Rect* originalRect = new Rect((float)word->Left, (float)word->Top, (float)word->Width, (float)word->Height);
                    Rect overlayRect = scaleTrasform->TransformBounds(*originalRect);

                    auto wordTextBlock = ref new TextBlock();

                    // Define the TextBlock.
                    wordTextBlock->Height = overlayRect.Height;
                    wordTextBlock->Width = overlayRect.Width;
                    wordTextBlock->FontSize = wordTextBlock->Height * 0.8;
                    wordTextBlock->Text = word->Text;
                    wordTextBlock->Style = (Windows::UI::Xaml::Style^) Application::Current->Resources->Lookup("ExtractedWordTextStyle");

                    // Define position, background, etc.
                    Border^ border = ref new Border();
                    border->Margin = *(ref new Thickness((double)overlayRect.Left, (double)overlayRect.Top, 0.0, 0.0));
                    border->Height = overlayRect.Height;
                    border->Width = overlayRect.Width;
                    border->Child = wordTextBlock;
                    border->Style = (Windows::UI::Xaml::Style^) Application::Current->Resources->Lookup("ExtractedWordBorderStyle");

                    // Put the filled TextBlock in the results grid.
                    TextOverlay->Children->Append(border);

                    extractedText += word->Text + " ";
                }
                extractedText += "\n";
            }

            ImageText->Text = extractedText;
            ImageText->Style = (Windows::UI::Xaml::Style^) Application::Current->Resources->Lookup("GreenTextStyle");
        }
        else
        {
            ImageText->Text = "No text.";
            ImageText->Style = (Windows::UI::Xaml::Style^) Application::Current->Resources->Lookup("RedTextStyle");
        }

        rootPage->NotifyUser(
            "Image successfully processed in " + ocrEngine->Language.ToString() + " language.",
            NotifyType::StatusMessage);
    });
}

/// <summary>
/// Get pointer to byte array from IBuffer.
/// </summary>
byte* ExtractText::GetPointerToPixelData(IBuffer^ buffer)
{
    // Cast to Object^, then to its underlying IInspectable interface.
    Object^ obj = buffer;
    Microsoft::WRL::ComPtr<IInspectable> insp(reinterpret_cast<IInspectable*>(obj));

    // Query the IBufferByteAccess interface.
    Microsoft::WRL::ComPtr<IBufferByteAccess> bufferByteAccess;
    insp.As(&bufferByteAccess);

    // Retrieve the buffer data.
    byte* pixels = nullptr;
    bufferByteAccess->Buffer(&pixels);

    return pixels;
}

/// <summary>
/// This is click event handler for Overlay button.
/// Check state of this this control determines whether extracted text will be overlaid over image. 
/// </summary>
void ExtractText::Overlay_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    TextOverlay->Visibility = OverlayResults->IsChecked->Value ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Collapsed;
}
