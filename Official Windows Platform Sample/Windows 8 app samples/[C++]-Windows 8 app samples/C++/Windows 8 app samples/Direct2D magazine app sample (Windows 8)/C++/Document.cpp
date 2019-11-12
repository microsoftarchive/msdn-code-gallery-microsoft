//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Microsoft::WRL;
using namespace concurrency;
using namespace Windows::Storage;
using namespace Windows::Storage::Search;
using namespace Windows::Storage::Streams;
using namespace Windows::Storage::FileProperties;
using namespace Windows::Data::Xml::Dom;

Document::Document(
    _In_ StorageFolder^ location,
    _In_ Platform::String^ fileName,
    _In_ Renderer^ renderer
    ) :
    m_location(location),
    m_fileName(fileName),
    m_rootXml(nullptr),
    m_rootElement(nullptr),
    m_design(nullptr),
    m_renderer(renderer),
    m_fontCollection(),
    m_fontLoader(),
    m_dwriteFactory()
{
    // Create DirectWrite isolated factory.
    //
    // Unlike a typical shared factory, the isolated factory does not send miss report
    // to the cross-process font cache, and therefore does not interact with DirectWrite's
    // state in other component in the same process. It's designed to be used when the
    // usage of font data within one component is meant to be isolated from the rest of
    // the application.
    //
    DX::ThrowIfFailed(
        DWriteCreateFactory(
            DWRITE_FACTORY_TYPE_ISOLATED,
            __uuidof(IDWriteFactory),
            &m_dwriteFactory
            )
        );
}

Document::~Document()
{
    if (m_fontLoader != nullptr)
    {
        // Unregister the font loader from DirectWrite factory
        DX::ThrowIfFailed(
            m_dwriteFactory->UnregisterFontCollectionLoader(m_fontLoader.Get())
            );

        DX::ThrowIfFailed(
            m_dwriteFactory->UnregisterFontFileLoader(m_fontLoader.Get())
            );
    }
}

task<void> Document::LoadAsync()
{
    // Get the document file from the storage location
    return task<StorageFile^>(m_location->GetFileAsync(m_fileName)).then([=](StorageFile^ file)
    {
        auto xmlDocument = ref new XmlDocument();
        return xmlDocument->LoadFromFileAsync(file);

    }).then([=](XmlDocument^ loadedXml)
    {
        m_rootXml = loadedXml->SelectSingleNode("story");

        // Create a font loader and start loading all the font files in the document location.
        m_fontLoader = new FontLoader(m_location, m_dwriteFactory.Get());
        return m_fontLoader->LoadAsync();

    }).then([=]()
    {
        // Register the font loader to DirectWrite factory
        // The loaders are needed to load custom fonts used within the document.
        DX::ThrowIfFailed(
            m_dwriteFactory->RegisterFontCollectionLoader(m_fontLoader.Get())
            );

        DX::ThrowIfFailed(
            m_dwriteFactory->RegisterFontFileLoader(m_fontLoader.Get())
            );
    });
}

void Document::GetFontCollection(_Outptr_result_maybenull_ IDWriteFontCollection** fontCollection)
{
    if (m_fontCollection != nullptr)
    {
        *fontCollection = ComPtr<IDWriteFontCollection>(m_fontCollection).Detach();
    }
    else if (m_fontLoader != nullptr)
    {
        // The font collection key is simply set to zero because the loader only
        // represents a single collection of fonts used in this document.
        size_t fontCollectionKey = 0;

        DX::ThrowIfFailed(
            m_dwriteFactory->CreateCustomFontCollection(
                m_fontLoader.Get(),
                &fontCollectionKey,
                sizeof(size_t),
                &m_fontCollection
                )
            );

        *fontCollection = ComPtr<IDWriteFontCollection>(m_fontCollection).Detach();
    }
    else
    {
        *fontCollection = nullptr;
    }
}

void Document::GetDWriteFactory(_Outptr_ IDWriteFactory** dwriteFactory)
{
    *dwriteFactory = ComPtr<IDWriteFactory>(m_dwriteFactory).Detach();
}

//  Parse the XML document to an element tree, then traverse the tree
//  to bind resources to the element requiring them.
void Document::Parse()
{
    if (m_rootXml != nullptr)
    {
        Story^ rootElement = Element::Create<Story>(this, m_rootXml);

        if (rootElement != nullptr)
        {
            TreeIterator<Element> it(rootElement);
            do
            {
                if (!(it.GetCurrentNode())->BindResource(rootElement))
                    return;

            } while (++it);

            m_rootElement = rootElement;
        }
    }
}

Element^ Document::GetContentRoot()
{
    // Content root is the direct child of the design element.
    return UpdateDesign()->GetFirstChild();
}

Element^ Document::UpdateDesign()
{
    // Get the design most appropriate for the current display size.
    m_design = m_rootElement->GetDesign(GetRenderer()->GetDisplaySize());

    return m_design;
}

D2D1_SIZE_F Document::GetPageSize()
{
    return m_design != nullptr ? m_design->GetPageSize() : D2D1::SizeF();
}

// Convert a page design width to display width
float Document::DesignToDisplayWidth(float designWidth)
{
    return designWidth / GetPageSize().width * m_renderer->GetDisplayWidth();
}

// Convert a page design height to display height
float Document::DesignToDisplayHeight(float designHeight)
{
    return designHeight / GetPageSize().height * m_renderer->GetDisplayHeight();
}

// Convert a display width to page design width
float Document::DisplayToDesignWidth(float displayWidth)
{
    return displayWidth / m_renderer->GetDisplayWidth() * GetPageSize().width;
}

// Convert a display height to page design height
float Document::DisplayToDesignHeight(float displayHeight)
{
    return displayHeight / m_renderer->GetDisplayHeight() * GetPageSize().height;
}
