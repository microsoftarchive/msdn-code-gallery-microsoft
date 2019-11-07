// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <pch.h>
#include "MainPage.g.h"

namespace PdfShowcase
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MainPage sealed
    {
    public:
        MainPage();
        void OnSuspending(
            _In_ Object^ sender,
            _In_ Windows::ApplicationModel::SuspendingEventArgs^ args
            );

    protected:
        // Event Handler for ViewChange event from scroll viewer
        void EventHandlerViewChanged(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::Controls::ScrollViewerViewChangedEventArgs^ e);

        // Event Handler for ViewChangeStarted for SemanticZoom
        void EventHandlerViewChangeStarted(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::Controls::SemanticZoomViewChangedEventArgs^ e);

        void InitializeZoomedInView();

        void InitializeZoomedOutView();

        void OnOpenFileClick(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ e);

        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        void LoadDefaultFile();

        void LoadPDF(_In_ Windows::Storage::StorageFile^ pdfFile);


    private:
        PdfViewModel::PdfDocViewModel^ pdfDataSourceZoomedInView;
        PdfViewModel::PdfDocViewModel^ pdfDataSourceZoomedOutView;
        Windows::Data::Pdf::PdfDocument^ pdfDocument;
        Windows::Storage::StorageFile^ loadedFile;
        Windows::UI::Xaml::Controls::GridView^ zoomedOutView;
        Windows::UI::Xaml::Controls::ListView^ zoomedInView;
    };
}
