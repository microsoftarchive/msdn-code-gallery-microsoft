//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Magazine;
using namespace concurrency;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::ApplicationModel;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::Storage;
using namespace Windows::Storage::Search;
using namespace Windows::Graphics::Display;

MainPage::MainPage() :
    m_document(nullptr)
{
    InitializeComponent();

    // Initialize DirectX renderer
    auto renderer = ref new Renderer(
        Window::Current->Bounds,
        DisplayProperties::LogicalDpi
        );

    // Start loading the document content
    auto document = ref new Document(Package::Current->InstalledLocation, "sample.story", renderer);
    task<void>(document->LoadAsync()).then([=]()
    {
        DocumentLoaded(document);

    }, task_continuation_context::use_current());

    renderer->DeviceLost += ref new DeviceLostEventHandler(this, &MainPage::OnDeviceLost);

    Window::Current->SizeChanged +=
        ref new WindowSizeChangedEventHandler(this, &MainPage::OnWindowSizeChanged);
}

void MainPage::LoadState(_In_ IPropertySet^ localSettings)
{
    if (localSettings->HasKey("Document.Page"))
    {
        int page = safe_cast<IPropertyValue^>(localSettings->Lookup("Document.Page"))->GetInt32();

        FlipView->SelectedIndex = page;
    }
}

void MainPage::SaveState(_In_ IPropertySet^ localSettings)
{
    if (localSettings->HasKey("Document.Page"))
    {
        localSettings->Remove("Document.Page");
    }

    localSettings->Insert(
        "Document.Page",
        PropertyValue::CreateInt32(FlipView->SelectedIndex)
        );
}

void MainPage::DocumentLoaded(_In_ Document^ document)
{
    // Parse the document into an element tree.
    document->Parse();

    auto contentRoot = document->GetContentRoot();

    if (contentRoot != nullptr)
    {
        // Create a collection of content element to bind to the view
        auto items = ref new Platform::Collections::Vector<Platform::Object^>();

        auto pageContent = contentRoot->GetFirstChild();

        while (pageContent != nullptr)
        {
            items->Append(ref new PageModel(pageContent, document));

            pageContent = pageContent->GetNextSibling();
        }

        FlipView->ItemsSource = items;

        // Load the saved document state if any
        LoadState(ApplicationData::Current->LocalSettings->Values);

        m_document = document;
    }
}

void MainPage::OnDeviceLost()
{
    // Save the application state and reload the document so that all elements will
    // be recreated with the new device.

    SaveState(ApplicationData::Current->LocalSettings->Values);

    DocumentLoaded(m_document);
}

void MainPage::OnWindowSizeChanged(
    _In_ Platform::Object^ sender,
    _In_ Windows::UI::Core::WindowSizeChangedEventArgs^ args
    )
{
    if (m_document != nullptr)
    {
        // Update renderer with the new window size
        auto renderer = m_document->GetRenderer();
        renderer->UpdateWindowSize();

        // Window size change could mean changing display orientation. When this happens the display
        // content may change to suit the new display layout. We need to update the page model with
        // all the new content.
        auto contentRoot = m_document->GetContentRoot();
        auto pageContent = contentRoot->GetFirstChild();

        auto currentItems = dynamic_cast<Platform::Collections::Vector<Platform::Object^>^>(FlipView->ItemsSource);

        for (uint32 i = 0; i != currentItems->Size && pageContent != nullptr; ++i)
        {
            // Refresh the current item with the new page content
            PageModel^ pageModel = dynamic_cast<PageModel^>(currentItems->GetAt(i));
            pageModel->UpdateContent(pageContent);

            pageContent = pageContent->GetNextSibling();
        }
    }
}
