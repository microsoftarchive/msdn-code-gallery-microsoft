//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "ThumbnailInfo.h"

//
// Current thumbnail selection state
//
enum ThumbnailSelectionState
{
    SelectionStateNone = 0x1,
    SelectionStateHoverOn = 0x2,
    SelectionStateSelected = 0x4
};

/// <summary>
/// The interface representing a thumbnail control
/// </summary>
[uuid("B8DE25F6-70CF-4ECA-B9B8-24E9100DDF4A")]
__interface IThumbnail : IUnknown
{
    HRESULT __stdcall Draw();

    // Getters and setters
    HRESULT __stdcall SetRenderingParameters(__in const RenderingParameters& renderingParams);
    HRESULT __stdcall SetParentWindow(__in Hilo::WindowApiHelpers::IChildNotificationHandler* parentWindowMessageHandler);
    HRESULT __stdcall SetDefaultBitmap(__in ID2D1Bitmap* bitmap);
    HRESULT __stdcall GetRect(__out D2D1_RECT_F* rect);
    HRESULT __stdcall SetRect(__in D2D1_RECT_F rect);
    HRESULT __stdcall GetThumbnail(__out ID2D1Bitmap** bitmap);
    HRESULT __stdcall SetThumbnail(__in ID2D1Bitmap* newBitmap);
    HRESULT __stdcall SetThumbnailIfNull(__in ID2D1Bitmap* newBitmap);
    HRESULT __stdcall GetThumbnailInfo(__out ThumbnailInfo* thumbnailInfo);
    HRESULT __stdcall SetIsFullImage(__in bool isFullImage);
    HRESULT __stdcall GetSelectionState(__out ThumbnailSelectionState* selectionState);
    HRESULT __stdcall SetSelectionState(__in ThumbnailSelectionState selectionState);
    HRESULT __stdcall SetOpacity(float opacity);
    HRESULT __stdcall SetFontSize(float size);

    // Resource management
    HRESULT __stdcall ReleaseThumbnail();
    HRESULT __stdcall ReleaseFullImage();
    HRESULT __stdcall DiscardResources();
};
