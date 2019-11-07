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
#include "D2DPageRenderer.h"

class CDocumentSource : public Microsoft::WRL::RuntimeClass<Microsoft::WRL::RuntimeClassFlags<Microsoft::WRL::WinRtClassicComMix>,
                                                            IPrintDocumentPageSource,
                                                            IPrintPreviewPageCollection,
                                                            ABI::Windows::Graphics::Printing::IPrintDocumentSource>
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

            // Cast d2dRender back to PageRenderer object.
            m_renderer = reinterpret_cast<PageRenderer^>(pageRenderer);
        }

        return hr;
    }

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
    float
    TransformedPageSize(
        _In_  float                         desiredWidth,
        _In_  float                         desiredHeight,
        _Out_ Windows::Foundation::Size*    previewSize
        );

    uint32                                                  m_totalPages;
    bool                                                    m_paginateCalled;
    float                                                   m_height;
    float                                                   m_width;
    D2D1_RECT_F                                             m_imageableRect;
    PageRenderer^                                           m_renderer;
    Microsoft::WRL::ComPtr<IPrintPreviewDxgiPackageTarget>  m_dxgiPreviewTarget;
};
