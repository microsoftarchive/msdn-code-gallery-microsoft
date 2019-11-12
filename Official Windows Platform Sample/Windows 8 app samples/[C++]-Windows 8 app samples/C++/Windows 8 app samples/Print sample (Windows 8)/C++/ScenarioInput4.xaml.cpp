// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ScenarioInput4.xaml.cpp
// Implementation of the ScenarioInput4 class
//

#include "pch.h"
#include "ScenarioOutput4.xaml.h"
#include "ScenarioInput4.xaml.h"

using namespace PrintSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Graphics::Printing;
using namespace Windows::Graphics::Printing::OptionDetails;
using namespace Windows::UI;
using namespace Windows::UI::Text;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Documents;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Printing;

DisplayContent GetDisplayContent(int option)
{
    DisplayContent displayContent;
    switch (option)
    {
    case 1:
        displayContent = DisplayContent::Text;
        break;
    case 2:
        displayContent = DisplayContent::Images;
        break;
    default:
        displayContent = DisplayContent::TextAndImages;
        break;
    }

    return displayContent;
}

ScenarioInput4::ScenarioInput4()
{
    InitializeComponent();
}

/// <summary>
/// Provide print content for scenario 4 first page
/// </summary>
void ScenarioInput4::PreparetPrintContent()
{
    if (FirstPage == nullptr)
    {
        FirstPage = ref new ScenarioOutput4();
        StackPanel^ header = safe_cast<StackPanel^>(FirstPage->FindName("header"));
        header->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }

    // Add the (newley created) page to the printing root which is part of the visual tree and force it to go
    // through layout so that the linked containers correctly distribute the content inside them.
    PrintingRoot->Children->Append(FirstPage);
    PrintingRoot->InvalidateMeasure();
    PrintingRoot->UpdateLayout();
}

/// <summary>
/// This is the event handler for PrintManager.PrintTaskRequested.
/// In order to ensure a good user experience, the system requires that the app handle the PrintTaskRequested event within the time specified by PrintTaskRequestedEventArgs->Request->Deadline.
/// Therefore, we use this handler to only create the print task.
/// The print settings customization can be done when the print document source is requested.
/// </summary>
/// <param name="sender">PrintManager</param>
/// <param name="e">PrintTaskRequestedEventArgs</param>
void ScenarioInput4::PrintTaskRequested(PrintManager^ sender, PrintTaskRequestedEventArgs^ e)
{
    auto printTaskRef = std::make_shared<PrintTask^>(nullptr);
    *printTaskRef = e->Request->CreatePrintTask("C++ Printing SDK Sample", 
                                                ref new PrintTaskSourceRequestedHandler([this, printTaskRef](PrintTaskSourceRequestedArgs^ args)
                                                {
                                                    PrintTask^ printTask = *printTaskRef;
                                                    IVector<String^>^ displayedOptions = printTask->Options->DisplayedOptions;

                                                    // clear the print options that are displayed beacuse adding the same one multiple times will throw an exception
                                                    displayedOptions->Clear();

                                                    PrintTaskOptionDetails^ printDetailedOptions = PrintTaskOptionDetails::GetFromPrintTaskOptions(printTask->Options);

                                                    // Choose the printer options to be shown.
                                                    // The order in which the options are appended determines the order in which they appear in the UI
                                                    displayedOptions->Clear();

                                                    displayedOptions->Append(StandardPrintTaskOptions::Copies);
                                                    displayedOptions->Append(StandardPrintTaskOptions::Orientation);
                                                    displayedOptions->Append(StandardPrintTaskOptions::ColorMode);
            
                                                    // Create a new list option

                                                    PrintCustomItemListOptionDetails^ pageFormat = printDetailedOptions->CreateItemListOption(L"PageContent", L"Pictures");
                                                    pageFormat->AddItem(L"PicturesText", L"Pictures and text");
                                                    pageFormat->AddItem(L"PicturesOnly", L"Pictures only");
                                                    pageFormat->AddItem(L"TextOnly", L"Text only");

                                                    // Add the custom option to the option list
                                                    displayedOptions->Append(L"PageContent");

                                                    printDetailedOptions->OptionChanged += ref new TypedEventHandler<PrintTaskOptionDetails^, PrintTaskOptionChangedEventArgs^>(this, &ScenarioInput4::printDetailedOptions_OptionChanged);

                                                    // Print Task event handler is invoked when the print job is completed.
                                                    printTask->Completed += ref new TypedEventHandler<PrintTask^, PrintTaskCompletedEventArgs^>(
                                                    [=](PrintTask^ sender, PrintTaskCompletedEventArgs^ e)
                                                    {
                                                        // Notify the user when the print operation fails.
                                                        if (e->Completion == Windows::Graphics::Printing::PrintTaskCompletion::Failed)
                                                        {
                                                            auto callback = ref new Windows::UI::Core::DispatchedHandler(
                                                            [=]()
                                                            {
                                                                RootPage->NotifyUser(ref new String(L"Failed to print."), NotifyType::ErrorMessage);
                                                            });

                                                            Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, callback);
                                                        }
                                                    });

                                                    args->SetSource(PrintDocumentSource);
                                                }));
}

/// <summary>
/// Option change event handler
/// </summary>
/// <param name="sender">PrintTaskOptionsDetails object</param>
/// <param name="args">the event arguments containing the changed option id</param>
void ScenarioInput4::printDetailedOptions_OptionChanged(PrintTaskOptionDetails^ sender, PrintTaskOptionChangedEventArgs^ args)
{
    // Listen for PageContent changes
    String^ optionId = safe_cast<String^>(args->OptionId);

    if (optionId != nullptr && optionId == L"PageContent")
    {
        RefreshPreview();
    }
}

/// <summary>
/// Helper function used to triger a preview refresh
/// </summary>
void ScenarioInput4::RefreshPreview()
{
    auto callback = ref new Windows::UI::Core::DispatchedHandler([this]()
                                                                  {
                                                                    CurrentPrintDocument->InvalidatePreview();
                                                                  });
    
    Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, callback);
}

/// <summary>
/// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
/// </summary>
/// <param name="sender"></param>
/// <param name="e">Paginate Event Arguments</param>
void ScenarioInput4::CreatePrintPreviewPages(Object^ sender, PaginateEventArgs^ e)
{
    // Get PageContent property
    PrintTaskOptionDetails^ printDetailedOptions = PrintTaskOptionDetails::GetFromPrintTaskOptions(e->PrintTaskOptions);
    IPrintOptionDetails^ printOptionDetails = printDetailedOptions->Options->Lookup(ref new String(L"PageContent"));
    String^ pageContent = safe_cast<String^>(printOptionDetails->Value);

    // Set the text & image display flag
    int result = (wcsstr(pageContent->Data(), L"Pictures")!=nullptr) << 1 | (wcsstr(pageContent->Data(), L"Text")!=nullptr);
    m_imageText = GetDisplayContent(result);

    BasePrintPage::CreatePrintPreviewPages(sender, e);
}

/// <summary>
/// This function creates and adds one print preview page to the internal cache of print preview
/// pages stored in m_printPreviewPages.
/// </summary>
/// <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
/// <param name="printPageDescription">Printer's page description</param>
RichTextBlockOverflow^ ScenarioInput4::AddOnePrintPreviewPage(RichTextBlockOverflow^ lastRTBOAdded, PrintPageDescription printPageDescription)
{
    // Check if we need to hide/show text & images for this scenario
    // Since all is rulled by the first page (page flow), here is where we must start
    if (lastRTBOAdded == nullptr)
    {
        // Get a refference to page objects
        Grid^ pageContent = safe_cast<Grid^>(FirstPage->FindName("printableArea"));
        Image^ scenarioImage = safe_cast<Image^>(FirstPage->FindName("scenarioImage"));
        RichTextBlock^ mainText = safe_cast<RichTextBlock^>(FirstPage->FindName("textContent"));
        RichTextBlockOverflow^ firstLink = safe_cast<RichTextBlockOverflow^>(FirstPage->FindName("firstLinkedContainer"));
        RichTextBlockOverflow^ continuationLink = safe_cast<RichTextBlockOverflow^>(FirstPage->FindName("continuationPageLinkedContainer"));

        // Hide(collapse) and move elements in different grid cells depending by the viewable content(only text, only pictures)

        scenarioImage->Visibility = ShowImage ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Collapsed;
        firstLink->SetValue(Grid::ColumnSpanProperty, ShowImage ? 1 : 2);

        scenarioImage->SetValue(Grid::RowProperty, ShowText ? 2 : 1);
        scenarioImage->SetValue(Grid::ColumnProperty, ShowText ? 1 : 0);

        pageContent->ColumnDefinitions->GetAt(0)->Width = Windows::UI::Xaml::GridLengthHelper::FromValueAndType(ShowText ? 6 : 4, GridUnitType::Star);
        pageContent->ColumnDefinitions->GetAt(1)->Width = Windows::UI::Xaml::GridLengthHelper::FromValueAndType(ShowText ? 4 : 6, GridUnitType::Star);

        // Break the text flow if the app is not printing text in order not to spawn pages with blank content
        mainText->Visibility = ShowText ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Collapsed;
        continuationLink->Visibility = ShowText ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Collapsed;

        // Hide header if printing only images
        StackPanel^ header = safe_cast<StackPanel^>(FirstPage->FindName("header"));
        header->Visibility = ShowText ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Collapsed;
    }

    //Continue with the rest of the base printing layout processing (paper size, printable page size)
    return BasePrintPage::AddOnePrintPreviewPage(lastRTBOAdded, printPageDescription);
}

void ScenarioInput4::OnNavigatedTo(NavigationEventArgs^ e)
{
    BasePrintPage::OnNavigatedTo(e);

    RegisterForPrinting();
}

void ScenarioInput4::OnNavigatedFrom(NavigationEventArgs^ e)
{
    UnregisterForPrinting() ;
}