//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "docsource.h"

using namespace Microsoft::WRL;
using namespace Windows::Graphics::Printing;

#pragma region IDocumentPageSource Methods

IFACEMETHODIMP
CDocumentSource::GetPreviewPageCollection(
    _In_  IPrintDocumentPackageTarget*  docPackageTarget,
    _Out_ IPrintPreviewPageCollection** docPageCollection
    )
{
    HRESULT hr = (docPackageTarget == nullptr) ? E_INVALIDARG : S_OK;

    // Get for IPrintPreviewDxgiPackageTarget interface.
    if (SUCCEEDED(hr))
    {
        hr = docPackageTarget->GetPackageTarget(
            ID_PREVIEWPACKAGETARGET_DXGI,
            IID_PPV_ARGS(&m_dxgiPreviewTarget)
            );
    }

    ComPtr<IPrintPreviewPageCollection> pageCollection;
    if (SUCCEEDED(hr))
    {
        ComPtr<CDocumentSource> docSource(this);
        hr = docSource.As<IPrintPreviewPageCollection>(&pageCollection);
    }

    if (SUCCEEDED(hr))
    {
        hr = pageCollection.CopyTo(docPageCollection);
    }

    return hr;
}

IFACEMETHODIMP
CDocumentSource::MakeDocument(
    _In_ IInspectable*                docOptions,
    _In_ IPrintDocumentPackageTarget* docPackageTarget
    )
{
    if (docOptions == nullptr || docPackageTarget == nullptr)
    {
        return E_INVALIDARG;
    }

    // Get print settings from PrintTaskOptions for printing, such as page description, which contains page size, imageable area, DPI.
    // User can obtain other print settings in the same way, such as ColorMode, NumberOfCopies, etc., which are not shown in this sample.
    PrintTaskOptions^ option = reinterpret_cast<PrintTaskOptions^>(docOptions);
    PrintPageDescription pageDesc = option->GetPageDescription(1); // Get the description of the first page.

    // Create a print control properties and set DPI from PrintPageDescription.
    D2D1_PRINT_CONTROL_PROPERTIES printControlProperties;

    printControlProperties.rasterDPI  = (float)(min(pageDesc.DpiX, pageDesc.DpiY)); // DPI for rasterization of all unsupported D2D commands or options
    printControlProperties.colorSpace = D2D1_COLOR_SPACE_SRGB;                  // Color space for vector graphics in D2D print control.
    printControlProperties.fontSubset = D2D1_PRINT_FONT_SUBSET_MODE_DEFAULT;    // Subset for used glyphs, send and discard font resource after every five pages

    HRESULT hr = S_OK;

    try
    {
        // Create a new print control linked to the package target.
        m_renderer->CreatePrintControl(
            docPackageTarget,
            &printControlProperties
            );

        // Calculate imageable area and page size from PrintPageDescription.
        D2D1_RECT_F imageableRect = D2D1::RectF(
            pageDesc.ImageableRect.X,
            pageDesc.ImageableRect.Y,
            pageDesc.ImageableRect.X + pageDesc.ImageableRect.Width,
            pageDesc.ImageableRect.Y + pageDesc.ImageableRect.Height
            );

        D2D1_SIZE_F pageSize = D2D1::SizeF(pageDesc.PageSize.Width, pageDesc.PageSize.Height);

        // Loop to add page command list to d2d print control.
        for (uint32 pageNum = 1; pageNum <= m_totalPages; ++pageNum)
        {
            m_renderer->PrintPage(
                pageNum,
                imageableRect,
                pageSize,
                nullptr // If a page-level print ticket is not specified here, the package print ticket is applied for each page.
                );
        }
    }
    catch (Platform::Exception^ e)
    {
        hr = e->HResult;

        if (hr == D2DERR_RECREATE_TARGET)
        {
            // In case of device lost, the whole print job will be aborted, and we should recover
            // so that the device is ready when used again. At the same time, we should propagate
            // this error to the Modern Print Dialog.
            m_renderer->HandleDeviceLost();
        }
    }

    // Make sure to close d2d print control even if AddPage fails.
    HRESULT hrClose = m_renderer->ClosePrintControl();

    if (SUCCEEDED(hr))
    {
        hr = hrClose;
    }

    return hr;
}

#pragma endregion IDocumentPageSource Methods

#pragma region IPrintPreviewPageCollection Methods

IFACEMETHODIMP
CDocumentSource::Paginate(
    _In_   uint32           currentJobPage,
    _In_   IInspectable*    docOptions
    )
{
    HRESULT hr = (docOptions == nullptr) ? E_INVALIDARG : S_OK;

    if (SUCCEEDED(hr))
    {
        // Get print settings from PrintTaskOptions for printing, such as page description, which contains page size, imageable area, DPI.
        // User can obtain other print settings in the same way, such as ColorMode, NumberOfCopies, etc., which are not shown in this sample.
        PrintTaskOptions^ option = reinterpret_cast<PrintTaskOptions^>(docOptions);
        PrintPageDescription pageDesc = option->GetPageDescription(currentJobPage);

        hr = m_dxgiPreviewTarget->InvalidatePreview();

        // Set the total page number.
        if (SUCCEEDED(hr))
        {
            hr = m_dxgiPreviewTarget->SetJobPageCount(PageCountType::FinalPageCount, m_totalPages);
        }

        if (SUCCEEDED(hr))
        {
            m_width = pageDesc.PageSize.Width;
            m_height = pageDesc.PageSize.Height;

            m_imageableRect = D2D1::RectF(
                pageDesc.ImageableRect.X,
                pageDesc.ImageableRect.Y,
                pageDesc.ImageableRect.X + pageDesc.ImageableRect.Width,
                pageDesc.ImageableRect.Y + pageDesc.ImageableRect.Height
                );

            // Now we are ready to let MakePage to be called.
            m_paginateCalled = true;
        }
    }

    return hr;
}

// Here, desiredWidth/desiredHeight is the desired size of preview surface by print mananger in system.
// The final size of the preview surface must have the same proportion as that of the desired width/height.
// In this sample, we just use it as preview size and return the scale variant for surface drawing.
// The size here is in DIPs.
float
CDocumentSource::TransformedPageSize(
    _In_  float                         desiredWidth,
    _In_  float                         desiredHeight,
    _Out_ Windows::Foundation::Size*    previewSize
    )
{
    float scale = 1.0f;

    if (desiredWidth > 0 && desiredHeight > 0)
    {
        previewSize->Width  = desiredWidth;
        previewSize->Height = desiredHeight;

        scale = m_width / desiredWidth;
    }
    else
    {
        previewSize->Width = 0;
        previewSize->Height = 0;
    }

    return scale;
}

// This sample only acts upon orientation setting for an example. The orientation is read from the user selection
// in the Print Experience and is then used to reflow the content in a different way.
IFACEMETHODIMP
CDocumentSource::MakePage(
    _In_ uint32 desiredJobPage,
    _In_ float  width,
    _In_ float  height
    )
{
    HRESULT hr = (width > 0 && height > 0) ? S_OK : E_INVALIDARG;

    // When desiredJobPage is JOB_PAGE_APPLICATION_DEFINED, it means a new preview begins. If the implementation here is by an async way,
    // for example, queue MakePage calls for preview, app needs to clean resources for previous preview before next.
    // In this sample, we will reset page number if Paginate() has been called.
    if (desiredJobPage == JOB_PAGE_APPLICATION_DEFINED && m_paginateCalled)
    {
        desiredJobPage = 1;
    }

    if (SUCCEEDED(hr) && m_paginateCalled)
    {
        // Calculate the size of preview surface, according to desired width and height.
        Windows::Foundation::Size previewSize;
        float scale = TransformedPageSize(width, height, &previewSize);

        try
        {
            m_renderer->DrawPreviewSurface(
                previewSize.Width,
                previewSize.Height,
                scale,
                m_imageableRect,
                desiredJobPage,
                m_dxgiPreviewTarget.Get()
                );
        }
        catch (Platform::Exception^ e)
        {
            hr = e->HResult;

            if (hr == D2DERR_RECREATE_TARGET)
            {
                // In case of device lost, we should recover so that the device is ready to render
                // the next preview page when requested. At the same time, we should propagate this
                // error to the Modern Print Dialog.
                m_renderer->HandleDeviceLost();
            }
        }
    }

    return hr;
}

#pragma region IPrintPreviewPageCollection Methods
