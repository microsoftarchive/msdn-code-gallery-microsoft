// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ScenarioInput5.xaml.cpp
// Implementation of the ScenarioInput5 class
//

#include "pch.h"
#include "ScenarioInput5.xaml.h"
#include "ScenarioOutput5.xaml.h"
#include <algorithm>
#include <regex>

using namespace PrintSample;

using namespace Windows::Foundation::Collections;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::Graphics::Printing;
using namespace Windows::Graphics::Printing::OptionDetails;
using namespace Windows::UI;
using namespace Windows::UI::Text;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Documents;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;


ScenarioInput5::ScenarioInput5()
    :m_totalPages(0)
{
    InitializeComponent();
}

/// <summary>
/// Provide print content for scenario 5 first page
/// </summary>
void ScenarioInput5::PreparetPrintContent()
{
    if (FirstPage == nullptr)
    {
        FirstPage = ref new ScenarioOutput5();
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
/// Removes pages that are not in the given range
/// </summary>
/// <param name="sender">The list of preview pages</param>
/// <param name="e"></param>
/// <note> Handling preview for page range
/// Developers have the control over how the preview should look when the user specifies a valid page range.
/// There are three common ways to handle this:
/// 1) Preview remains unaffected by the page range and all the pages are shown independent of the specified page range.
/// 2) Preview is changed and only the pages specified in the range are shown to the user.
/// 3) Preview is changed, showing all the pages and graying out the pages not in page range.
/// We chose option (2) for this sample, developers can choose their preview option.
/// </note>
void ScenarioInput5::OnContentCreated()
{
    m_totalPages = PrintPreviewPages;

    if (m_pageRangeEditVisible)
    {
        // ignore page range if there are any invalid pages regarding current context
        if(m_pageList.size() > 0 &&
           std::find_if(m_pageList.begin(), m_pageList.end(), 
                        [this](int page)
                        { 
                            return page > static_cast<int>(m_totalPages);
                        }
                       ) == m_pageList.end())
        {
            for(int it = static_cast<int>(PrintPreviewPages) - 1; it >= 0; --it)
            {                
                if(m_pageList.end() == std::find(m_pageList.begin(), m_pageList.end(), it + 1))
                {
                    RemovePageFromPrintPreviewCollection(it);
                }
            }
        }
    }
}

/// <summary>
/// This is the event handler for PrintManager.PrintTaskRequested.
/// In order to ensure a good user experience, the system requires that the app handle the PrintTaskRequested event within the time specified by PrintTaskRequestedEventArgs->Request->Deadline. 
/// Therefore, we use this handler to only create the print task.
/// The print settings customization can be done when the print document source is requested.
/// </summary>
/// <param name="sender">PrintManager</param>
/// <param name="e">PrintTaskRequestedEventArgs</param>
void ScenarioInput5::PrintTaskRequested(PrintManager^ sender, PrintTaskRequestedEventArgs^ e)
{
    auto printTaskRef = std::make_shared<PrintTask^>(nullptr);
    *printTaskRef = e->Request->CreatePrintTask(L"C++ Printing SDK Sample", 
                                                ref new PrintTaskSourceRequestedHandler([this, printTaskRef](PrintTaskSourceRequestedArgs^ args)
                                                {
                                                    PrintTask^ printTask = *printTaskRef;
                                                    PrintTaskOptionDetails^ printDetailedOptions = PrintTaskOptionDetails::GetFromPrintTaskOptions(printTask->Options);
                                                    IVector<String^>^ displayedOptions = printDetailedOptions->DisplayedOptions;

                                                    // Choose the printer options to be shown.
                                                    // The order in which the options are appended determines the order in which they appear in the UI
                                                    displayedOptions->Clear();

                                                    displayedOptions->Append(StandardPrintTaskOptions::Copies);
                                                    displayedOptions->Append(StandardPrintTaskOptions::Orientation);
                                                    displayedOptions->Append(StandardPrintTaskOptions::ColorMode);

                                                    // Create a new list option

                                                    PrintCustomItemListOptionDetails^ pageFormat = printDetailedOptions->CreateItemListOption(L"PageRange", L"Page Range");
                                                    pageFormat->AddItem(L"PrintAll", L"Print all");
                                                    pageFormat->AddItem(L"PrintSelection", L"Print Selection");
                                                    pageFormat->AddItem(L"PrintRange", L"Print Range");

                                                    // Add the custom option to the option list
                                                    displayedOptions->Append(L"PageRange");

                                                    // Create new edit option
                                                    PrintCustomTextOptionDetails^ pageRangeEdit = printDetailedOptions->CreateTextOption(L"PageRangeEdit", L"Range");

                                                    // Register the handler for the option change event
                                                    printDetailedOptions->OptionChanged += ref new TypedEventHandler<PrintTaskOptionDetails^, PrintTaskOptionChangedEventArgs^>(this, &ScenarioInput5::printDetailedOptions_OptionChanged);

                                                    // Print Task event handler is invoked when the print job is completed.
                                                    printTask->Completed += ref new TypedEventHandler<PrintTask^, PrintTaskCompletedEventArgs^>(
                                                    [=](PrintTask^ sender, PrintTaskCompletedEventArgs^ e)
                                                    {
                                                        m_pageRangeEditVisible = false;
                                                        m_selectionMode = false;
                                                        m_pageList.clear();

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

                                                        auto callback = ref new Windows::UI::Core::DispatchedHandler(
                                                        [=]()
                                                        {
                                                            // Restore first page to its default layout
                                                            // Undo any changes made by a text selection
                                                            ShowContent(nullptr);
                                                        });

                                                        Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, callback);
                                                    });

                                                    args->SetSource(PrintDocumentSource);
                                                }));
}

/// <summary>
/// Option change event handler
/// </summary>
/// <param name="sender">PrintTaskOptionsDetails object</param>
/// <param name="args">the event arguments containing the changed option id</param>
void ScenarioInput5::printDetailedOptions_OptionChanged(PrintTaskOptionDetails^ sender, PrintTaskOptionChangedEventArgs^ args)
{
    if (args->OptionId == nullptr)
        return;
            
    String^ optionId = args->OptionId->ToString();

    // Handle change in Page Range Option

    if (optionId == L"PageRange")
    {
        IPrintOptionDetails^ pageRange = sender->Options->Lookup(optionId);
        String^ pageRangeValue = pageRange->Value->ToString();

        m_selectionMode = false;

        if(pageRangeValue == L"PrintRange")
        {
            // Add PageRangeEdit custom option to the option list
            sender->DisplayedOptions->Append(L"PageRangeEdit");
            m_pageRangeEditVisible = true;

            auto callback = ref new Windows::UI::Core::DispatchedHandler(
            [=]()
            {
                // Restore first page to its default layout
                // Undo any changes made by a text selection
                ShowContent(nullptr);
            });

            Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, callback);
        }
        else if(pageRangeValue == L"PrintSelection")
        {
            Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal,
                                 ref new Windows::UI::Core::DispatchedHandler([this]()
                                                            {
                                                                ScenarioOutput5^ outputScenario = safe_cast<ScenarioOutput5^>(RootPage->OutputFrame->Content);

                                                                ShowContent(outputScenario->SelectedText);
                                                            }));
            RemovePageRangeEdit(sender);
        }
        else
        {
            auto callback = ref new Windows::UI::Core::DispatchedHandler(
            [=]()
            {
                // Restore first page to its default layout
                // Undo any changes made by a text selection
                ShowContent(nullptr);
            });

            Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, callback);
            RemovePageRangeEdit(sender);
        }

        RefreshPreview();

    }
    else if (optionId == L"PageRangeEdit")
    {
        IPrintOptionDetails^ pageRange = sender->Options->Lookup(optionId);

        // Expected range format (p1,p2...)*, (p3-p9)* ...
        std::wregex rangePattern(L"^\\s*\\d+\\s*(\\-\\s*\\d+\\s*)?(\\,\\s*\\d+\\s*(\\-\\s*\\d+\\s*)?)*$");
        std::wstring pageRangeValue(pageRange->Value->ToString()->Data());

        if(!std::regex_match(pageRangeValue.begin(), pageRangeValue.end(), rangePattern))
        {
            pageRange->ErrorText = L"Invalid Page Range (eg: 1-3, 5)";
        }
        else
        {
            pageRange->ErrorText = L"";
            try
            {
                GetPagesInRange(pageRange->Value->ToString());
            }
            catch(InvalidPageException* rangeException)
            {
                pageRange->ErrorText = ref new String(rangeException->get_DisplayMessage().data());

                delete rangeException;
            }

            RefreshPreview();
        }
    }
}

/// <summary>
/// Helper function used to split a string based on some delimiters
/// </summary>
/// <param name="string">String to be splited</param>
/// <param name="delimiter">Delimiter character used to split the string</param>
/// <param name="words">Splited words</param>
void ScenarioInput5::SplitString(String^ string, wchar_t delimiter, std::vector<std::wstring>& words)
{
    std::wistringstream iss(string->Data());

    std::wstring part;
    while(std::getline(iss, part, delimiter))
    {
        words.push_back(part);
    };
}

/// <summary>
/// This is where we parse the range field
/// </summary>
/// <param name="pageRange">the page range value</param>
void ScenarioInput5::GetPagesInRange(String^ pageRange)
{
    std::vector<std::wstring> vector_range;
    SplitString(pageRange, ',', vector_range);

    // Clear the previous values
    m_pageList.clear();

    for(std::vector<std::wstring>::iterator it = vector_range.begin(); it != vector_range.end(); ++ it)
    {
        int intervalPos = static_cast<int>((*it).find('-'));
        if( intervalPos != -1)
        {
            int start = _wtoi((*it).substr(0, intervalPos).data());
            int end = _wtoi((*it).substr(intervalPos + 1, (*it).length() - intervalPos - 1).data());

            if ((start < 1) || (end > static_cast<int>(m_totalPages)) || (start >= end))
            {
                std::wstring message(L"Invalid page(s) in range ");

                message.append(std::to_wstring(start));
                message.append(L" - ");
                message.append(std::to_wstring(end));

                throw new InvalidPageException(message);
            }

            for(int intervalPage=start; intervalPage <= end; ++intervalPage)
            {
                m_pageList.push_back(intervalPage);
            }
        }
        else
        {
            int pageNr = _wtoi((*it).data());
            std::wstring message(L"Invalid page ");

            if (pageNr < 1)
            {
                message.append(std::to_wstring(pageNr));
                throw new InvalidPageException(message);
            }

            if (pageNr > static_cast<int>(m_totalPages))
            {
                message.append(std::to_wstring(pageNr));
                throw new InvalidPageException(message);
            }

            m_pageList.push_back(pageNr);
        }
    }

    // sort in decreasing order
    std::sort(m_pageList.begin(), m_pageList.end(), std::greater<int>());
    // remove duplicates
    std::unique(m_pageList.begin(), m_pageList.end());
}

/// <summary>
/// Helper function used to triger a preview refresh
/// </summary>
void ScenarioInput5::RefreshPreview()
{
    auto callback = ref new Windows::UI::Core::DispatchedHandler([this]()
                                                                  {
                                                                    CurrentPrintDocument->InvalidatePreview();
                                                                  });

    Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, callback);
}

/// <summary>
/// Removes the PageRange edit from the charm window
/// </summary>
/// <param name="printTaskOptionDetails">Details regarding PrintTaskOptions</param>
void ScenarioInput5::RemovePageRangeEdit(PrintTaskOptionDetails^ printTaskOptionDetails)
{
    if (m_pageRangeEditVisible)
    {
        unsigned int index;
        if(printTaskOptionDetails->DisplayedOptions->IndexOf(ref new String(L"PageRangeEdit"), &index))
        {
            printTaskOptionDetails->DisplayedOptions->RemoveAt(index);
        }
        m_pageRangeEditVisible = false;
    }
}

void ScenarioInput5::ShowContent(String^ selectionText)
{
    bool hasSelection = selectionText != nullptr && !selectionText->IsEmpty();
    m_selectionMode = hasSelection;

    // Hide/show images depending by the selected text
    StackPanel^ header = safe_cast<StackPanel^>(FirstPage->FindName("header"));
    header->Visibility = hasSelection ? Windows::UI::Xaml::Visibility::Collapsed : Windows::UI::Xaml::Visibility::Visible;
    Grid^ pageContent = safe_cast<Grid^>(FirstPage->FindName("printableArea"));
    pageContent->RowDefinitions->GetAt(0)->Height = GridLengthHelper::Auto;

    Image^ scenarioImage = safe_cast<Image^>(FirstPage->FindName("scenarioImage"));
    scenarioImage->Visibility = hasSelection ? Windows::UI::Xaml::Visibility::Collapsed : Windows::UI::Xaml::Visibility::Visible;

    // Expand the middle paragraph on the full page if printing only selected text
    RichTextBlockOverflow^ firstLink = safe_cast<RichTextBlockOverflow^>(FirstPage->FindName("firstLinkedContainer"));
    firstLink->SetValue(Grid::ColumnSpanProperty, hasSelection ? 2 : 1);

    // Clear(hide) current text and add only the selection if a selection exists
    RichTextBlock^ mainText = safe_cast<RichTextBlock^>(FirstPage->FindName("textContent"));
    RichTextBlock^ textSelection = safe_cast<RichTextBlock^>(FirstPage->FindName("textSelection"));

    // Main (default) scenario text
    mainText->Visibility = hasSelection ? Windows::UI::Xaml::Visibility::Collapsed : Windows::UI::Xaml::Visibility::Visible;
    mainText->OverflowContentTarget = hasSelection ? nullptr : firstLink;

    // Scenario text-blocks used for displaying selection
    textSelection->OverflowContentTarget = hasSelection ? firstLink : nullptr;
    textSelection->Visibility = hasSelection ? Windows::UI::Xaml::Visibility::Visible : Windows::UI::Xaml::Visibility::Collapsed;
    textSelection->Blocks->Clear();

    // Force the visual root to go through layout so that the linked containers correctly distribute the content inside them.
    PrintingRoot->InvalidateArrange();
    PrintingRoot->InvalidateMeasure();
    PrintingRoot->UpdateLayout();

    // Add the selection if any
    if (hasSelection)
    {
        Run^ inlineText = ref new Run();
        inlineText->Text = selectionText;

        Paragraph^ paragraph = ref new Paragraph();
        paragraph->Inlines->Append(inlineText);

        textSelection->Blocks->Append(paragraph);
    }
}

/// <summary>
/// This function creates and adds one print preview page to the internal cache of print preview
/// pages stored in m_printPreviewPages.
/// </summary>
/// <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
/// <param name="printPageDescription">Printer's page description</param>
RichTextBlockOverflow^ ScenarioInput5::AddOnePrintPreviewPage(RichTextBlockOverflow^ lastRTBOAdded, Windows::Graphics::Printing::PrintPageDescription printPageDescription)
{
    RichTextBlockOverflow^ textLink = BasePrintPage::AddOnePrintPreviewPage(lastRTBOAdded, printPageDescription);

    ScenarioOutput5^ outputScenario = safe_cast<ScenarioOutput5^>(RootPage->OutputFrame->Content);

    // Don't show footer in selection mode
    if(m_selectionMode)
    {
        FrameworkElement^ page = safe_cast<FrameworkElement^>(PrintingRoot->Children->GetAt(PrintingRoot->Children->Size -1));
        StackPanel^ footer = safe_cast<StackPanel^>(page->FindName("footer"));
        footer->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    }

    return textLink;
}

void ScenarioInput5::OnNavigatedTo(NavigationEventArgs^ e)
{
    BasePrintPage::OnNavigatedTo(e);

    RegisterForPrinting();
}

void ScenarioInput5::OnNavigatedFrom(NavigationEventArgs^ e)
{
    UnregisterForPrinting() ;
}