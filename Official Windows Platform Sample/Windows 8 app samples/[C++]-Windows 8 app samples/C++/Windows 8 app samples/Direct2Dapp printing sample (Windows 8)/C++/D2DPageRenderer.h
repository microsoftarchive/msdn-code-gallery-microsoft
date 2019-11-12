//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <DirectXBase.h>
#include <printpreview.h>
#include "SampleOverlay.h"

ref class PageRendererContext;

// This class is shared between the display, print, and print-preview threads.
// To ensure thread-safety, it should contain only immutable resources,
// factories, and devices.
//
// For performance, we also cache the display PageRendererContext on the
// PageRenderer.
ref class PageRenderer : public DirectXBase
{
internal:
    PageRenderer();

    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void Render() override;
    virtual void UpdateForWindowSizeChange() override;

    void SetSnappedMode(_In_ bool isSnapped);

    // Print out one page.
    void PrintPage(
        _In_ uint32                 pageNumber,
        _In_ D2D1_RECT_F            imageableArea,
        _In_ D2D1_SIZE_F            pageSize,
        _In_opt_ IStream*           pagePrintTicketStream
        );

    // Create DXGI surface for preview.
    void DrawPreviewSurface(
        _In_  float                             width,
        _In_  float                             height,
        _In_  float                             scale,
        _In_  D2D1_RECT_F                       contentBox,
        _In_  uint32                            desiredJobPage,
        _In_  IPrintPreviewDxgiPackageTarget*   previewTarget
        );

    // Create Direct2D print control for caller, who only needs to
    // specify IPrintDocumentPackageTarget and print control property.
    void CreatePrintControl(
        _In_  IPrintDocumentPackageTarget*      docPackageTarget,
        _In_  D2D1_PRINT_CONTROL_PROPERTIES*    printControlProperties
        );

    // Close print control with protection.
    HRESULT ClosePrintControl();

    IDWriteTextFormat* GetTextFormatNoRef();
    IDWriteTextFormat* GetMessageFormatNoRef();

private:
    Microsoft::WRL::ComPtr<ID2D1PrintControl>       m_d2dPrintControl;

    // Overlay for sample title.
    SampleOverlay^                                  m_sampleOverlay;

    // Whether the app is in the snapped mode.
    bool                                            m_isSnapped;

    // Page renderer context for use for display. Note that this object is mutable
    // and hence should *only* be used by the display thread.
    PageRendererContext^                            m_displayPageRendererContext;

    // Immutable resources
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_textFormat;
    Microsoft::WRL::ComPtr<IDWriteTextFormat>       m_messageFormat;
};
