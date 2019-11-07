// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// MainPage.xaml.cpp
// Implementation of the MainPage.xaml class.
//

#include "pch.h"
#include "MainPage.xaml.h"
#include "App.xaml.h"

using namespace ShareTarget;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::DataTransfer;
using namespace Windows::ApplicationModel::DataTransfer::ShareTarget;
using namespace Windows::Data::Json;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::UI;
using namespace Windows::UI::Core;
using namespace Windows::UI::Text;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Documents;
using namespace Windows::UI::Xaml::Input;

MainPage::MainPage()
{
    InitializeComponent();
}

MainPage::~MainPage()
{
}

void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // It is recommended to only retrieve the ShareOperation object in the activation handler, return as
    // quickly as possible, and retrieve all data from the share target asynchronously.

    shareOperation = dynamic_cast<ShareOperation^>(e->Parameter);

    create_task([this]()
    {
        // Retrieve the data package properties.
        sharedDataTitle = shareOperation->Data->Properties->Title;
        sharedDataDescription = shareOperation->Data->Properties->Description;
        sharedThumbnailStreamRef = shareOperation->Data->Properties->Thumbnail;
        shareQuickLinkId = shareOperation->QuickLinkId;

        // Retrieve the data package content.
        std::vector<task<void>> dataPackageTasks;
        if (shareOperation->Data->Contains(StandardDataFormats::Uri))
        {
            // The GetUriAsync(), GetTextAsync(), GetStorageItemsAsync(), etc. APIs will throw if there was an error retrieving the data from the source app.
            // In this sample, we just display the error. It is recommended that a share target app handles these in a way appropriate for that particular app.
            dataPackageTasks.push_back(create_task(shareOperation->Data->GetUriAsync()).then([this](task<Uri^> uriTask)
            {
                try
                {
                    sharedUri = uriTask.get();
                }
                catch (Exception^ ex)
                {
                    NotifyUserBackgroundThread("Failed GetUriAsync - " + ex->Message, NotifyType::ErrorMessage);
                }
            }));
        }
        if (shareOperation->Data->Contains(StandardDataFormats::Text))
        {
            dataPackageTasks.push_back(create_task(shareOperation->Data->GetTextAsync()).then([this](task<String^> textTask)
            {
                try
                {
                    sharedText = textTask.get();
                }
                catch (Exception^ ex)
                {
                    NotifyUserBackgroundThread("Failed GetTextAsync - " + ex->Message, NotifyType::ErrorMessage);
                }
            }));
        }
        if (shareOperation->Data->Contains(StandardDataFormats::StorageItems))
        {
            dataPackageTasks.push_back(create_task(shareOperation->Data->GetStorageItemsAsync()).then(
                [this](task<IVectorView<IStorageItem^>^> itemsTask)
            {
                try
                {
                    sharedStorageItems = itemsTask.get();
                }
                catch (Exception^ ex)
                {
                    NotifyUserBackgroundThread("Failed GetStorageItemsAsync - " + ex->Message, NotifyType::ErrorMessage);
                }
            }));
        }
        if (shareOperation->Data->Contains(DATA_FORMAT_NAME))
        {
            dataPackageTasks.push_back(create_task(shareOperation->Data->GetTextAsync(DATA_FORMAT_NAME)).then(
                [this](task<String^> customTextTask)
            {
                try
                {
                    sharedCustomData = customTextTask.get();
                }
                catch (Exception^ ex)
                {
                    NotifyUserBackgroundThread("Failed GetTextAsync(" + DATA_FORMAT_NAME + ") - "  + ex->Message, NotifyType::ErrorMessage);
                }
            }));
        }
        if (shareOperation->Data->Contains(StandardDataFormats::Html))
        {
            dataPackageTasks.push_back(create_task(shareOperation->Data->GetHtmlFormatAsync()).then([this](task<String^> htmlTask)
            {
                try
                {
                    sharedHtmlFormat = htmlTask.get();
                }
                catch (Exception^ ex)
                {
                    NotifyUserBackgroundThread("Failed GetHtmlFormatAsync - " + ex->Message, NotifyType::ErrorMessage);
                }
            }));

            dataPackageTasks.push_back(create_task(shareOperation->Data->GetResourceMapAsync()).then(
                [this](task<IMapView<String^, RandomAccessStreamReference^>^> resourceMapTask)
            {
                try
                {
                    sharedResourceMap = resourceMapTask.get();
                }
                catch (Exception^ ex)
                {
                    NotifyUserBackgroundThread("Failed GetResourceMapAsync - " + ex->Message, NotifyType::ErrorMessage);
                }
            }));
        }
        if (shareOperation->Data->Contains(StandardDataFormats::Bitmap))
        {
            dataPackageTasks.push_back(create_task(shareOperation->Data->GetBitmapAsync()).then(
                [this](task<RandomAccessStreamReference^> bitmapTask)
            {
                try
                {
                    sharedBitmapStreamRef = bitmapTask.get();
                }
                catch (Exception^ ex)
                {
                    NotifyUserBackgroundThread("Failed GetBitmapAsync - " + ex->Message, NotifyType::ErrorMessage);
                }
            }));
        }

        return when_all(dataPackageTasks.begin(), dataPackageTasks.end());
    }).then([this] ()
    {
        // In this sample, we just display the shared data content.

        DataPackageTitle->Text = sharedDataTitle;
        DataPackageDescription->Text = sharedDataDescription;

        if (sharedThumbnailStreamRef != nullptr)
        {
            create_task(sharedThumbnailStreamRef->OpenReadAsync()).then(
                [this](IRandomAccessStreamWithContentType^ thumbnailStream)
            {
                auto bitmapImage = ref new BitmapImage();
                bitmapImage->SetSource(thumbnailStream);
                ThumbnailHolder->Source = bitmapImage;
                ThumbnailArea->Visibility = Xaml::Visibility::Visible;
            });
        }

        if (shareQuickLinkId != nullptr)
        {
            SelectedQuickLinkId->Text = shareQuickLinkId;
        }

        if (sharedUri != nullptr)
        {
            AddContentValue("Uri: ", sharedUri->AbsoluteUri);
        }
        if (sharedText != nullptr)
        {
            AddContentValue("Text: ", sharedText);
        }
        if (sharedStorageItems != nullptr)
        {
            // Display the name of the files being shared.
            std::wstringstream fileNames;
            for (unsigned int i = 0; i < sharedStorageItems->Size; i++)
            {
                fileNames << sharedStorageItems->GetAt(i)->Name->Data();
                if (i < sharedStorageItems->Size - 1)
                {
                    fileNames << L", ";
                }
            }
            fileNames << L".";

            AddContentValue("StorageItems: ", ref new String(fileNames.str().c_str()));
        }
        if (sharedCustomData != nullptr)
        {
            // This is an area to be especially careful parsing data from the source app to avoid buffer overruns.
            // This sample doesn't perform data validation but will catch any exceptions thrown.
            try
            {
                auto customObject = JsonObject::Parse(sharedCustomData);
                if (customObject->HasKey("type"))
                {
                    auto typeString = customObject->GetNamedString("type");
                    if (typeString == "http://schema.org/Book")
                    {
                        // This sample expects the custom format to be of type http://schema.org/Book.
                        String^ receivedStrings = "Type: " + typeString + "\n";
                        auto properties = customObject->GetNamedObject("properties");
                        double numberOfPages = properties->GetNamedNumber("numberOfPages");
                        wchar_t pagesBuffer[11];
                        swprintf_s(pagesBuffer, ARRAYSIZE(pagesBuffer), L"%d", (int) min(numberOfPages, INT_MAX));

                        receivedStrings += "Image: " + properties->GetNamedString("image")
                            + "\nName: " + properties->GetNamedString("name")
                            + "\nBook Format: " + properties->GetNamedString("bookFormat")
                            + "\nAuthor: " + properties->GetNamedString("author")
                            + "\nNumber of Pages: " + ref new String(pagesBuffer)
                            + "\nPublisher: " + properties->GetNamedString("publisher")
                            + "\nDate Published: " + properties->GetNamedString("datePublished")
                            + "\nIn Language: " + properties->GetNamedString("inLanguage")
                            + "\nISBN: " + properties->GetNamedString("isbn");

                        AddContentValue("Custom format data:\n", receivedStrings);
                    }
                    else
                    {
                        NotifyUser("The custom format from the source app is not of type http://schema.org/Book", NotifyType::ErrorMessage);
                    }
                }
                else
                {
                    NotifyUser("The custom format from the source app doesn't contain a type", NotifyType::ErrorMessage);
                }
            }
            catch (Exception^ ex)
            {
                NotifyUser("Failed to parse the custom data - " + ex->Message, NotifyType::ErrorMessage);
            }
        }
        if (sharedHtmlFormat != nullptr)
        {
            String^ htmlFragment = HtmlFormatHelper::GetStaticFragment(sharedHtmlFormat);
            if (htmlFragment != nullptr)
            {
                AddContentValue("HTML: ");
                ShareWebView->Visibility = Xaml::Visibility::Visible;
                ShareWebView->NavigateToString("<html><body>" + htmlFragment + "</body></html>");
            }
            else
            {
                NotifyUser("GetStaticFragment failed to parse the HTML from the source app", NotifyType::ErrorMessage);
            }

            // Check if there are any local images in the resource map.
            if (sharedResourceMap->Size > 0)
            {
                ResourceMapValue->Text = "";
                std::for_each(begin(sharedResourceMap), end(sharedResourceMap), [this](IKeyValuePair<String^, RandomAccessStreamReference^>^ item)
                {
                    ResourceMapValue->Text += "\nKey: " + item->Key;
                });
                ResourceMapArea->Visibility = Xaml::Visibility::Visible;
            }
        }
        if (sharedBitmapStreamRef != nullptr)
        {
            create_task(sharedBitmapStreamRef->OpenReadAsync()).then(
                [this](IRandomAccessStreamWithContentType^ sharedStream)
            {
                auto bitmapImage = ref new BitmapImage();
                bitmapImage->SetSource(sharedStream);
                ImageHolder->Source = bitmapImage;
                ImageArea->Visibility = Xaml::Visibility::Visible;
            });
        }
    }, task_continuation_context::use_current()); // Run the continuation on the UI thread as it updates the UI.
}

void MainPage::QuickLinkSectionLabel_Tapped(Object^ sender, TappedRoutedEventArgs^ e)
{
    // Trigger the appropriate Checked/Unchecked event if the user taps on the text instead of the checkbox.
    AddQuickLink->IsChecked = !AddQuickLink->IsChecked->Value;
}

void MainPage::AddQuickLink_Checked(Object^ sender, RoutedEventArgs^ e)
{
    QuickLinkCustomization->Visibility = Xaml::Visibility::Visible;
}

void MainPage::AddQuickLink_Unchecked(Object^ sender, RoutedEventArgs^ e)
{
    QuickLinkCustomization->Visibility = Xaml::Visibility::Collapsed;
}

void MainPage::ReportCompleted_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (AddQuickLink->IsChecked->Value)
    {
        Agile<QuickLink> quickLinkInfo(ref new QuickLink());
        quickLinkInfo->Id = QuickLinkId->Text;
        quickLinkInfo->Title = QuickLinkTitle->Text;

        // For QuickLinks, the supported FileTypes and DataFormats are set independently from the manifest.
        quickLinkInfo->SupportedFileTypes->Append("*");
        quickLinkInfo->SupportedDataFormats->Append(StandardDataFormats::Text);
        quickLinkInfo->SupportedDataFormats->Append(StandardDataFormats::Uri);
        quickLinkInfo->SupportedDataFormats->Append(StandardDataFormats::Bitmap);
        quickLinkInfo->SupportedDataFormats->Append(StandardDataFormats::Html);
        quickLinkInfo->SupportedDataFormats->Append(DATA_FORMAT_NAME);

        create_task(Package::Current->InstalledLocation->CreateFileAsync("assets\\user.png", CreationCollisionOption::OpenIfExists))
            .then([this, quickLinkInfo](task<StorageFile^> completedTask)
        {
            try
            {
                auto iconFile = completedTask.get();
                quickLinkInfo->Thumbnail = RandomAccessStreamReference::CreateFromFile(iconFile);
                shareOperation->ReportCompleted(quickLinkInfo.Get());
            }
            catch (Exception^)
            {
                // Even if the QuickLink cannot be created it is important to call ReportCompleted. Otherwise, if this is a long-running share,
                // the app will stick around in the long-running share progress list.
                shareOperation->ReportCompleted();
                throw;
            }
        });
    }
    else
    {
        shareOperation->ReportCompleted();
    }
}

void MainPage::LongRunningShareLabel_Tapped(Object^ sender, TappedRoutedEventArgs^ e)
{
    // Trigger the appropriate Checked/Unchecked event if the user taps on the text instead of the checkbox.
    ExpandLongRunningShareSection->IsChecked = !ExpandLongRunningShareSection->IsChecked->Value;
}

void MainPage::ExpandLongRunningShareSection_Checked(Object^ sender, RoutedEventArgs^ e)
{
    ExtendedSharingArea->Visibility = Xaml::Visibility::Visible;
}

void MainPage::ExpandLongRunningShareSection_Unchecked(Object^ sender, RoutedEventArgs^ e)
{
    ExtendedSharingArea->Visibility = Xaml::Visibility::Collapsed;
}

void MainPage::ReportStarted_Click(Object^ sender, RoutedEventArgs^ e)
{
    shareOperation->ReportStarted();
    NotifyUser("Started...", NotifyType::StatusMessage);
}

void MainPage::ReportDataRetrieved_Click(Object^ sender, RoutedEventArgs^ e)
{
    shareOperation->ReportDataRetrieved();
    NotifyUser("Data retrieved...", NotifyType::StatusMessage);
}

void MainPage::ReportSubmittedBackgroundTask_Click(Object^ sender, RoutedEventArgs^ e)
{
    shareOperation->ReportSubmittedBackgroundTask();
    NotifyUser("Submitted background task...", NotifyType::StatusMessage);
}

void MainPage::ReportErrorButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    shareOperation->ReportError(ReportError->Text);
}

void MainPage::Footer_Click(Object^ sender, RoutedEventArgs^ e)
{
    auto uri = ref new Uri(safe_cast<String^>(safe_cast<HyperlinkButton^>(sender)->Tag));
    Windows::System::Launcher::LaunchUriAsync(uri);
}

void MainPage::NotifyUser(String^ message, NotifyType type)
{
    switch (type)
    {
    case NotifyType::StatusMessage:
        // Use the status message style.
        StatusBlock->Style = safe_cast<Windows::UI::Xaml::Style^>(this->Resources->Lookup("StatusStyle"));
        break;
    case NotifyType::ErrorMessage:
        // Use the error message style.
        StatusBlock->Style = safe_cast<Windows::UI::Xaml::Style^>(this->Resources->Lookup("ErrorStyle"));
        break;
    default:
        break;
    }
    StatusBlock->Text = message;
}

void MainPage::NotifyUserBackgroundThread(String^ message, NotifyType type)
{
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, message, type]()
    {
        NotifyUser(message, type);
    }));
}

void MainPage::AddContentValue(String^ title)
{
    AddContentValue(title, nullptr);
}

void MainPage::AddContentValue(String^ title, String^ description)
{
    Run^ contentType = ref new Run();
    contentType->FontWeight = FontWeights::Bold;
    contentType->Text = title;
    ContentValue->Inlines->Append(contentType);

    if (description != nullptr)
    {
        Run^ contentValue = ref new Run();
        contentValue->Text = description + "\n";
        ContentValue->Inlines->Append(contentValue);
    }
}