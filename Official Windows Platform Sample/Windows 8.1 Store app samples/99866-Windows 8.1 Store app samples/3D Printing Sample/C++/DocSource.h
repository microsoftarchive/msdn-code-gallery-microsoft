//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <windows.graphics.printing.h>
#include <printpreview.h>
#include <documentsource.h>
#include <xpsobjectmodel_2.h>
#include "PageRenderer3D.h"
#include <wrl.h>

using namespace Microsoft::WRL;

class CDocumentSource : public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::WinRtClassicComMix>,
                                               ABI::Windows::Graphics::Printing::IPrintDocumentSource,
                                               IPrintDocumentPageSource,
                                               IPrintPreviewPageCollection>
{
private:
    InspectableClass(L"Windows.Graphics.Printing.IPrintDocumentSource", BaseTrust);

public:
    HRESULT RuntimeClassInitialize(
        _In_ IUnknown* pageRenderer
        )
    {
        HRESULT hr = (pageRenderer != nullptr) ? S_OK : E_INVALIDARG;

        if (SUCCEEDED(hr))
        {
            m_paginateCalled = false;
            m_totalPages     = 1;
            m_height         = 0.f;
            m_width          = 0.f;

            // Cast object to correct type
            m_renderer = reinterpret_cast<PageRenderer3D^>(pageRenderer);
        }

        return hr;
    }

    //
    // App-specific methods
    //
    void InvalidatePreview(void);

    void Set3dModelXml(
        _In_ ComPtr<IStream>& modelStream
        );

    void SetPageImage(
        _In_ Windows::Storage::Streams::InMemoryRandomAccessStream^ pageImageStream,
        _In_ float32 pageImageWidth,
        _In_ float32 pageImageHeight
        );

    //
    // classic COM interface IDocumentPageSource methods
    //
    IFACEMETHODIMP
    GetPreviewPageCollection(
        _In_  IPrintDocumentPackageTarget*  docPackageTarget,
        _Out_ IPrintPreviewPageCollection** docPageCollection
        );

    IFACEMETHODIMP
    MakeDocument(
        _In_ IInspectable*                  docOptions,
        _In_ IPrintDocumentPackageTarget*   docPackageTarget
        );

    //
    // classic COM interface IPrintPreviewPageCollection methods
    //
    IFACEMETHODIMP
    Paginate(
        _In_   uint32           currentJobPage,
        _In_   IInspectable*    docOptions
        );

    IFACEMETHODIMP
    MakePage(
        _In_ uint32 desiredJobPage,
        _In_ float  width,
        _In_ float  height
        );

private:
    Platform::String^ 
    MakePrintTicket(
        _In_ IInspectable*                docOptions
        );

    float
    TransformedPageSize(
        _In_  float                         desiredWidth,
        _In_  float                         desiredHeight,
        _Out_ Windows::Foundation::Size*    previewSize
        );

    HRESULT
    Add2DPayload(
        ComPtr<IXpsOMObjectFactory>& xpsFactory,
        ComPtr<IXpsOMObjectFactory1>& xpsFactory1,
        ComPtr<IXpsOMPackageWriter>& packageWriter2D,
        XPS_SIZE& pageSize
        );

    ComPtr<IStream>                                         m_modelStream;
    Platform::Object^                                       m_texture;
    uint32                                                  m_totalPages;
    bool                                                    m_paginateCalled;
    float                                                   m_height;
    float                                                   m_width;
    D2D1_RECT_F                                             m_imageableRect;
    PageRenderer3D^                                         m_renderer;
    Microsoft::WRL::ComPtr<IPrintPreviewDxgiPackageTarget>  m_dxgiPreviewTarget;
    Windows::Storage::Streams::InMemoryRandomAccessStream^  m_pageImageStream;
    float                                                   m_pageImageWidth;
    float                                                   m_pageImageHeight;
};
