// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "pch.h"
#include "MainPage.xaml.h"


using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Printing;

namespace PrintSample
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class BasePrintPage : Page
    {
    private:
        /// <summary>
        /// Print task requested event registration token
        /// </summary>
        EventRegistrationToken                              m_printTaskRequestedEventToken;

        /// <summary>
        /// a boolean which tracks whether the app has been registered for printing
        /// </summary>        
        bool                                                m_isRegisteredForPrinting;

        /// <summary>
        /// A pointer back to the main page which is used to gain access to the input and output frames and their content. 
        /// </summary>
        MainPage^                                           m_rootPage;
        
        /// <summary>
        /// PrintDocument is used to prepare the pages for printing. 
        /// Prepare the pages to print in the handlers for the Paginate, GetPreviewPage, and AddPages events.
        /// </summary>
        PrintDocument^                                      m_printDocument;

        /// <summary>
        /// Marker interface for document source
        /// </summary>
        Windows::Graphics::Printing::IPrintDocumentSource^  m_printDocumentSource;

        /// <summary>
        /// A list of UIElements used to store the print preview pages.  This gives easy access
        /// to any desired preview page.
        /// </summary>        
        std::vector<UIElement^>                             m_printPreviewPages;

        /// <summary>
        /// Current requeted page for preview.
        /// </summary>
        LONG                                                m_currentPage;

        /// <summary>
        /// First page in the printing-content series
        /// From this "virtual sized" paged content is split(text is flowing) to "printing pages"
        /// </summary>
        FrameworkElement^ m_firstPage;
        
    protected:

        /// <summary>
        /// The percent of app's margin width, content is set at 85% (0.85) of the area's width
        /// </summary>        
        property float ApplicationContentMarginLeft
        {
            float get()
            {
                return 0.075f;
            }
        }

        /// <summary>
        /// The percent of app's margin height, content is set at 94% (0.94) of tha area's height
        /// </summary>
        property float ApplicationContentMarginTop
        {
            float get()
            {
                return 0.03f;
            }
        }

        property Canvas^ PrintingRoot
        {
            Canvas^ get()
            {
                return safe_cast<Canvas^>(FindName("printingRoot"));
            }
        }

        property MainPage^ RootPage
        {
            MainPage^ get()
            {
                return m_rootPage;
            }
        }

        property bool RegisteredForPrinting
        {
            bool get()
            {
                return m_isRegisteredForPrinting;
            }

            void set(bool value)
            {
                m_isRegisteredForPrinting = value;
            }
        }

        property FrameworkElement^ FirstPage
        {
            FrameworkElement^ get()
            {
                return m_firstPage;
            }
            void set(FrameworkElement^ value)
            {
                m_firstPage = value;
            }
        }

        property Windows::Graphics::Printing::IPrintDocumentSource^ PrintDocumentSource
        {
            Windows::Graphics::Printing::IPrintDocumentSource^ get()
            {
                return m_printDocumentSource;
            }
        }

        property PrintDocument^ CurrentPrintDocument
        {
            PrintDocument^ get()
            {
                return m_printDocument;
            }
        }

        property size_t PrintPreviewPages
        {
            size_t get()
            {
                return m_printPreviewPages.size();
            }
        }

        /// <summary>
        /// Factory method for every scenario that will create/generate print content specific to each scenario
        /// For scenarios 1-5: it will create the first page from which content will flow
        /// </summary>
        virtual void PreparetPrintContent();

        /// <summary>
        /// Register for print button handler
        /// </summary>
        void RegisterForPrintingButtonClick(Object^ sender, RoutedEventArgs^ e);

        /// <summary>
        /// This is the event handler for PrintManager.PrintTaskRequested.
        /// In order to ensure a good user experience, the system requires that the app handle the PrintTaskRequested event within the time specified by PrintTaskRequestedEventArgs->Request->Deadline.
        /// Therefore, we use this handler to only create the print task.
        /// The print settings customization can be done when the print document source is requested.
        /// </summary>
        /// <param name="sender">PrintManager</param>
        /// <param name="e">PrintTaskRequestedEventArgs</param>
        void virtual PrintTaskRequested(Windows::Graphics::Printing::PrintManager^ sender, Windows::Graphics::Printing::PrintTaskRequestedEventArgs^ e);

        /// <summary>
        /// This function registers the app for printing with Windows and sets up the necessary event handlers for the print process.
        /// </summary>
        void RegisterForPrinting();

        /// <summary>
        /// This function unregisters the app for printing with Windows.
        /// </summary>
        void UnregisterForPrinting();

        /// <summary>
        /// This is the event handler for PrintDocument.Paginate. It creates print preview pages for the app.
        /// </summary>
        /// <param name="sender">PrintDocument</param>
        /// <param name="e">Paginate Event Arguments</param>
        virtual void CreatePrintPreviewPages(Object^ sender, PaginateEventArgs^ e);

        virtual void OnContentCreated() {};

        /// <summary>
        /// This is the event handler for PrintDocument.GetPrintPreviewPage. It provides a specific print preview page,
        /// in the form of an UIElement, to an instance of PrintDocument. PrintDocument subsequently converts the UIElement
        /// into a page that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender">The Print Document</param>
        /// <param name="e">Arguments containing the preview requested page</param>
        virtual void GetPrintPreviewPage(Object^ sender, GetPreviewPageEventArgs^ e);

        /// <summary>
        /// This is the event handler for PrintDocument.AddPages. It provides all pages to be printed, in the form of
        /// UIElements, to an instance of PrintDocument. PrintDocument subsequently converts the UIElements
        /// into a pages that the Windows print system can deal with.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Add page event arguments containing a print task options reference</param>
        virtual void AddPrintPages(Object^ sender, AddPagesEventArgs^ e);

        /// <summary>
        /// This function creates and adds one print preview page to the internal cache of print preview
        /// pages stored in m_printPreviewPages.
        /// </summary>
        /// <param name="lastRTBOAdded">Last RichTextBlockOverflow element added in the current content</param>
        /// <param name="printPageDescription">Printer's page description</param>
        virtual RichTextBlockOverflow^ AddOnePrintPreviewPage(RichTextBlockOverflow^ lastRTBOAdded, Windows::Graphics::Printing::PrintPageDescription printPageDescription);

        /// <summary>
        /// Removes a page from the printprreview collection (generated content)
        /// </summary>
        /// <param name="pageNumber">The page number to remove</param>
        void RemovePageFromPrintPreviewCollection(int pageNumber);

    public:
        virtual void OnNavigatedTo(NavigationEventArgs^ e) override;

        virtual void OnNavigatedFrom(NavigationEventArgs^ e) override;
    };

};