//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "Scenario1.g.h"

namespace SDKSample
{
    namespace PDFAPICPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
 public enum class  RENDEROPTIONS
{
    NORMAL,
    ZOOM,
    PORTION
};

        public ref class Scenario1 sealed
        {
        public:
            Scenario1();

        private:

            void RenderPage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);            
            void RenderPageZoom_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);            
            void RenderPagePortion_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e); 
            void DisplayImageFileAsync(Windows::Storage::StorageFile^ file, RENDEROPTIONS);
            void RenderPDFPage(Platform::String^ pdfFileName,RENDEROPTIONS renderOptions);
            void RenderDocument(RENDEROPTIONS renderOptions);
            void EnableRenderingButtons();
            void DisableRenderingButtons();
            Windows::Foundation::IAsyncAction^ PDFRenderToStreamAsync(Windows::Data::Pdf::PdfPage^ pdfPage,Windows::Storage::Streams::IRandomAccessStream^ randomStream, RENDEROPTIONS renderOptions);
        };
    }
}
