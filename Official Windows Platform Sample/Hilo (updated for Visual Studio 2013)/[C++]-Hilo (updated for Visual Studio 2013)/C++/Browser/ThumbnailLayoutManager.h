//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "AsyncLoaderHelper.h"
#include "ThumbnailControl.h"
#include "ThumbnailLayoutInterface.h"

class ThumbnailLayoutManager : public IThumbnailLayoutManager
{
public:
    // Constants
    static const float MaxThumbnailSize;
    static const float MaxThumbnailSizeWithNoText;
    static const float ShadowSizeRatio;
    static const float XSelectionBoxDeltaRatio;
    static const float YSelectionBoxDeltaRatio;

    // Getters and setters
    HRESULT __stdcall GetPanState(__out int *panState);
    HRESULT __stdcall GetThumbnailSize(__out float *thumbnailSize);
    HRESULT __stdcall GetVisibleThumbnailRange(__out int *rangeStart, __out int *rangeEnd);
    HRESULT __stdcall SetArrowGutterSize(__in float arrowGutterSize);
    HRESULT __stdcall SetCurrentImage(__in unsigned int currentImageIndex);
    HRESULT __stdcall SetRenderTargetSize(__in D2D1_SIZE_F renderTargetSize);
    HRESULT __stdcall SetTextBoxHeight(__in float textBoxHeight);
    HRESULT __stdcall SetSlideShowMode(__in bool isSlideShow);

    // Slideshow helper
    HRESULT __stdcall BeginSlideShowPan();
    HRESULT __stdcall EndSlideShowPan(__out float *distance);

    // Implementation
    HRESULT __stdcall CreateLayout(__in const int imageCount, __in const bool resetPanPosition);

    // Rendering methods
    HRESULT __stdcall UpdateVisibleThumbnailControls(__in const std::vector<ComPtr<IThumbnail>> &cells);

    // Pan and zoom
    HRESULT __stdcall Pan(__in float offset);
    HRESULT __stdcall Zoom (__in float factor);

protected:
    // Constructor / Destructor
    ThumbnailLayoutManager(Hilo::AsyncLoader::IAsyncLoaderHelper *asyncLoader);
    virtual ~ThumbnailLayoutManager();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IThumbnailLayoutManager>::CastTo(iid, this, object);
    }

private:
    // Constants
    static const float DefaultThumbnailSize;
    static const float MinThumbnailSize;
    static const float MinSwitchImageDistance;
    static const float XGapRatio;
    static const float YGapRatio;

    // Asynchronous loader helper
    ComPtr<Hilo::AsyncLoader::IAsyncLoaderHelper> m_asyncLoaderHelper;

    // Rendering information
    D2D1_SIZE_F m_renderTargetSize;
    D2D1_POINT_2F m_renderingStartPoint;

    // Slideshow information
    bool m_isSlideShow;
    float m_slideShowCurrentPanPositionX;
    float m_slideShowMaxThumbnailSize;
    float m_slideShowZoomFactor;
    float m_previousThumbnailSize;

    // Layout/image information
    int m_imageCount;
    int m_rowCount;
    int m_columnCount;
    int m_renderingStartImageIndex;
    int m_renderingEndImageIndex;

    float m_thumbnailSize;
    float m_textBoxHeight;
    float m_xGap;
    float m_yGap;
    float m_imageGridOffsetX;
    float m_imageGridOffsetY;
    float m_arrowGutterSize;
    float m_currentPanPositionX;
    float m_panBoundaryLeft;
    float m_panBoundaryRight;

    // Asynchronous loader information
    int m_previousRenderingStartImageIndex;
    int m_previousItemCount;

    // Update layout based on one or more updated values
    void UpdateLayout();
};