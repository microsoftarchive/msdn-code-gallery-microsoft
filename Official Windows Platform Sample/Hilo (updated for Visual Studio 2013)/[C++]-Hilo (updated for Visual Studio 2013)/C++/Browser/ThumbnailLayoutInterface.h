//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
//
// Defines the available pan directions that are available. Use an int in order to
// allow multiple pan states at the same time and check for that state by doing 
// the following: if ((panValue & PanLeft) == PanLeft) { /*  Pan left */ }
//
enum PanState
{
    NoPanning = 0x0,
    PanLeft = 0x1,
    PanRight = 0x2
};

struct ThumbnailCell
{
    ComPtr<IThumbnail> control;
    D2D1_RECT_F position;
    ThumbnailSelectionState selectionState;

    ThumbnailCell (IThumbnail* thumbnailControl, D2D1_RECT_F thumbnailPosition) :
        control (thumbnailControl),
        position (thumbnailPosition)
    {
    }
};


[uuid("8EA37C96-7F73-427C-9C52-0C068EAA1676")]
__interface IThumbnailLayoutManager : public IUnknown
{
    HRESULT __stdcall GetPanState(__out int *panState);
    HRESULT __stdcall GetThumbnailSize(__out float* thumbnailSize);
    HRESULT __stdcall GetVisibleThumbnailRange(__out int *rangeStart, __out int *rangeEnd);
    HRESULT __stdcall SetArrowGutterSize(__in float arrowGutterSize);
    HRESULT __stdcall SetCurrentImage(__in unsigned int currentImageIndex);
    HRESULT __stdcall SetRenderTargetSize(__in D2D1_SIZE_F renderTargetSize);
    HRESULT __stdcall SetTextBoxHeight(__in float textBoxHeight);
    HRESULT __stdcall SetSlideShowMode(__in bool isSlideShow);
    HRESULT __stdcall BeginSlideShowPan();
    HRESULT __stdcall EndSlideShowPan(__out float *distance);
    HRESULT __stdcall UpdateVisibleThumbnailControls(__in const std::vector<ComPtr<IThumbnail>> &cells);
    HRESULT __stdcall CreateLayout(__in const int imageCount, __in const bool resetPanPosition);
    HRESULT __stdcall Pan(__in float offset);
    HRESULT __stdcall Zoom (__in float factor);
};
