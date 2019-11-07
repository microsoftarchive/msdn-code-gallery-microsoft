// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "BasePrintPage.h"
#include "ContinuationPage.xaml.h"

using namespace PrintSample;

using namespace Windows::Foundation::Collections;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::Graphics::Printing;
using namespace Windows::UI;
using namespace Windows::UI::Text;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Documents;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;

/// <summary>
/// Factory method for every scenario that will create/generate print content specific to each scenario
/// For scenarios 1-5: it will create the first page from which content will flow
/// </summary>
void BasePrintPage::PreparetPrintContent()
{
}

/// <summary>
/// This is the click handler for the 'Register' button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void BasePrintPage::RegisterForPrintingButtonClick(Object^ sender, RoutedEventArgs^ e)
{
    Button^ clickedButton = safe_cast<Button^>(sender);

    // check to see if the application is registered for printing
    if (m_isRegisteredForPrinting)
    {
        // unregister for printing 
        UnregisterForPrinting();

        // change the text on the button
        clickedButton->Content = "Register";

        // tell the user that the print contract has been unregistered
        m_rootPage->NotifyUser("Print contract unregistered.", NotifyType::StatusMessage);
    }
    else
    {
        // register for printing
        RegisterForPrinting();

        // change the text on the button
        clickedButton->Content = "Unregister";

        // tell the user that the print contract has been registered
        m_rootPage->NotifyUser("Print contract registered, use the Charms Bar to print.", NotifyType::StatusMessage);
    }

    // toggel the value of isRegisteredForPrinting
    m_isRegisteredForPrinting = !m_isRegisteredForPrinting;
}

void BasePrintPage::RegisterForPrinting()
{
    // Create the PrintDocument.
    m_printDocument = ref new PrintDocument();

    // Save the DocumentSource.
    m_printDocumentSource = m_printDocument->DocumentSource;

    // Add an event handler which creates preview pages.
    m_printDocument->Paginate += ref new Windows::UI::Xaml::Printing::PaginateEventHandler(this, &BasePrintPage::CreatePrintPreviewPages);

    // Add an event handler which provides a specified preview page.
    m_printDocument->GetPreviewPage += ref new Windows::UI::Xaml::Printing::GetPreviewPageEventHandler(this, &BasePrintPage::GetPrintPreviewPage);

    // Add an event handler which provides all final print pages.
    m_printDocument->AddPages += ref new Windows::UI::Xaml::Printing::AddPagesEventHandler(this, &BasePrintPage::AddPrintPages);

    // Create a PrintManager and add a handler for printing initialization.
    PrintManager^ printMan = PrintManager::GetForCurrentView();

    m_printTaskRequestedEventToken = printMan->PrintTaskRequested += ref new TypedEventHandler<PrintManager^, PrintTaskRequestedEventArgs^>(this, &BasePrintPage::PrintTaskRequested);        

    // Initialize print content for this scenario
    PreparetPrintContent();
}

void BasePrintPage::UnregisterForPrinting()
{
    // Remove the handler for printing initialization.
    PrintManager^ printMan = PrintManager::GetForCurrentView();

    printMan->PrintTaskRequested -= m_printTaskRequestedEventToken;

    PrintingRoot->Children->Clear();
}

/// <summary>
/// This is the event handler for PrintManager.PrintTaskRequested.
/// In order to ensure a good user experience, the system requires that the app handle the PrintTaskRequested event within the time specified by PrintTaskRequestedEventArgs->Request->Deadline.
/// Therefore, we use this handler to only create the print task.
/// The print settings customization can be done when the print document source is requested.
/// </summary>
/// <param name="sender">PrintManager</param>
/// <param name="e">PrintTaskRequestedEventArgs</param>
void BasePrintPage::PrintTaskRequested(PrintManager^ sender, PrintTaskRequestedEventArgs^ e)
{
    auto printTaskRef = std::make_shared<PrintTask^>(nullptr);
    *printTaskRef = e->Request->CreatePrintTask("C++ Printing SDK Sample", ref new Windows::Graphics::Printing::PrintTaskSourceRequestedHandler(
                               [=](PrintTaskSourceRequestedArgs^ args)
                               {
                                   PrintTask^ printTask = *printTaskRef;

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
                                                m_rootPage->NotifyUser(ref new String(L"Failed to print."), NotifyType::ErrorMessage);
                                            });

                                            Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal, callback);
                                        }
                                   });

                                   args->SetSource(m_printDocumentSource);
                               }));
}

/// <summary>
/// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
/// </summary>
/// <param name="sender">PrintDocument</param>
/// <param name="e">Paginate Event Arguments</param>
void BasePrintPage::CreatePrintPreviewPages(Object^ sender, PaginateEventArgs^ e)
{
    // Clear the cache of preview pages 
    m_printPreviewPages.clear();

    // Clear the printing root of preview pages
    PrintingRoot->Children->Clear();

    // This variable keeps track of the last RichTextBlockOverflow element that was added to a page which will be printed
    RichTextBlockOverflow^ lastRTBOOnPage;

    // Get the PrintTaskOptions
    PrintTaskOptions^ printingOptions = safe_cast<PrintTaskOptions^>(e->PrintTaskOptions);

    // Get the page description to deterimine how big the page is
    PrintPageDescription pageDescription = printingOptions->GetPageDescription(0);

    // We know there is at least one page to be printed. passing null as the first parameter to
    // AddOnePrintPreviewPage tells the function to add the first page.
    lastRTBOOnPage = AddOnePrintPreviewPage(nullptr, pageDescription);

    // We know there are more pages to be added as long as the last RichTextBoxOverflow added to a print preview
    // page has extra content
    while (lastRTBOOnPage->HasOverflowContent && lastRTBOOnPage->Visibility == Windows::UI::Xaml::Visibility::Visible)
    {
        lastRTBOOnPage = AddOnePrintPreviewPage(lastRTBOOnPage, pageDescription);
    }

    OnContentCreated();

    PrintDocument^ printDocument = safe_cast<PrintDocument^>(sender);

    // Report the number of preview pages created
    printDocument->SetPreviewPageCount(static_cast<int>(m_printPreviewPages.size()), PreviewPageCountType::Intermediate);
}

void BasePrintPage::GetPrintPreviewPage(Object^ sender, GetPreviewPageEventArgs^ e)
{
    PrintDocument^ localprintDocument = safe_cast<PrintDocument^>(sender);
    localprintDocument->SetPreviewPage(e->PageNumber, m_printPreviewPages[e->PageNumber - 1]);
}

/// <summary>
/// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
/// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
/// into a pages that the Windows print system can deal with.
/// </summary>
/// <param name="sender"></param>
/// <param name="e">Add page event arguments containing a print task options reference</param>
void BasePrintPage::AddPrintPages(Object^ sender, AddPagesEventArgs^ e)
{
    PrintDocument^ printDocument = safe_cast<PrintDocument^>(sender);

    // Loop over all of the preview pages and add each one to  add each page to be printied
    for (size_t i = 0; i < m_printPreviewPages.size(); i++)
    {
        printDocument->AddPage(m_printPreviewPages[i]);
    }

    // Indicate that all of the print pages have been provided
    printDocument->AddPagesComplete();
}

/// <summary>
/// This function creates and adds one print preview page to the internal cache of print preview
/// pages stored in m_printPreviewPages.
/// </summary>
/// <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
/// <param name="printPageDescription">Printer's page description</param>
RichTextBlockOverflow^ BasePrintPage::AddOnePrintPreviewPage(RichTextBlockOverflow^ lastRTBOAdded, PrintPageDescription printPageDescription)
{
    // XAML element that is used to represent to "printing page"
    FrameworkElement^ page;

    // The link container for text overflowing in this page
    RichTextBlockOverflow^ textLink;

    // Check if this is the first page ( no previous RichTextBlockOverflow)
    if (lastRTBOAdded == nullptr)
    {
        // If this is the first page add the specific scenario content
        page = m_firstPage;
        //Hide footer since we don't know yet if it will be displayed (this might not be the last page) - wait for layout
        StackPanel^ footer = safe_cast<StackPanel^>(page->FindName("footer"));
        footer->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    }
    else
    {
        // Flow content (text) from previous pages
        page = ref new ContinuationPage(lastRTBOAdded);
    }

    // Set "paper" width
    page->Width = printPageDescription.PageSize.Width;
    page->Height = printPageDescription.PageSize.Height;

    Grid^ printableArea = safe_cast<Grid^>(page->FindName("printableArea"));

    // Get the margins size
    // If the ImageableRect is smaller than the app provided margins use the ImageableRect
    double marginWidth = (std::max)(printPageDescription.PageSize.Width - printPageDescription.ImageableRect.Width, printPageDescription.PageSize.Width * ApplicationContentMarginLeft * 2);
    double marginHeight = (std::max)(printPageDescription.PageSize.Height - printPageDescription.ImageableRect.Height, printPageDescription.PageSize.Height * ApplicationContentMarginTop * 2);

    // Set-up "printable area" on the "paper"
    printableArea->Width = m_firstPage->Width - marginWidth;
    printableArea->Height = m_firstPage->Height - marginHeight;

    // Add the (newley created) page to the printing root which is part of the visual tree and force it to go
    // through layout so that the linked containers correctly distribute the content inside them.            
    PrintingRoot->Children->Append(page);
    PrintingRoot->InvalidateMeasure();
    PrintingRoot->UpdateLayout();

    // Find the last text container and see if the content is overflowing
    textLink = safe_cast<RichTextBlockOverflow^>(page->FindName("continuationPageLinkedContainer"));

    // Check if this is the last page
    if (!textLink->HasOverflowContent && textLink->Visibility == Windows::UI::Xaml::Visibility::Visible)
    {
        StackPanel^ footer = safe_cast<StackPanel^>(page->FindName("footer"));
        footer->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }

    // Add the page to the page preview collection
    m_printPreviewPages.push_back(page);

    return textLink;
}

void BasePrintPage::RemovePageFromPrintPreviewCollection(int pageNumber)
{
    m_printPreviewPages.erase(m_printPreviewPages.begin() + pageNumber);
}

void BasePrintPage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    m_rootPage = safe_cast<MainPage^>(e->Parameter);
}

void BasePrintPage::OnNavigatedFrom(NavigationEventArgs^ e)
{
    UnregisterForPrinting() ;
}