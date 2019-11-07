//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "PaneInterface.h"
#include "ImageThumbnailControl.h"
#include "WindowMessageHandlerImpl.h"
#include "AsyncLoaderHelper.h"
#include "ImageThumbnailControl.h"
#include "ThumbnailLayoutInterface.h"
#include "PanAnimation.h"
#include "WindowLayout.h"
#include "WindowLayoutChildInterface.h"
#include "WindowMessageHandlerImpl.h"

class WindowLayout;

class MediaPaneMessageHandler : public IPane,
                                public IMediaPane,
                                public IWindowLayoutChild,
                                public Hilo::WindowApiHelpers::IChildNotificationHandler,
                                public Hilo::AsyncLoader::IAsyncLoaderMemoryManagerClient,
                                public IInitializable,
                                public Hilo::WindowApiHelpers::WindowMessageHandler
{
private:
#ifdef _MEASURE_FPS
    // timer calculations
    SYSTEMTIME  m_startAnimationTime;
    unsigned short m_totalFramesRendered;
    std::ofstream m_logFile;
#endif

    ComPtr<Hilo::AsyncLoader::IAsyncLoaderHelper> m_AsyncLoaderHelper;

    // Thumnails layout manager
    ComPtr<IThumbnailLayoutManager> m_thumbnailLayoutManager;

    // Arrow related variables
    D2D1_RECT_F m_leftArrowRect;
    D2D1_RECT_F m_rightArrowRect;

    bool m_leftArrowSelected;
    bool m_rightArrowSelected;
    bool m_leftArrowClicked;
    bool m_rightArrowClicked;

    // Variables
    bool m_isSlideShowMode;
    bool m_mouseDownSlideShowMode;
    bool m_enableAnimation;
    bool m_enablePanAnimation;
    bool m_disableAnimation;
    bool m_updatingFolder;
    bool m_inertiaHandled;
    float m_previousPanPositionX;

    // The window layout management class
    ComPtr<IWindowLayout> m_windowLayout;

    // Factories
    ComPtr<ID2D1Factory> m_d2dFactory;
    ComPtr<IDWriteFactory> m_dWriteFactory;

    // Direct2D resources
    ComPtr<ID2D1HwndRenderTarget> m_renderTarget;
    ComPtr<ID2D1LinearGradientBrush> m_backgroundLinearGradientBrush;
    ComPtr<ID2D1LinearGradientBrush> m_foregroundGradientBrushLeft;
    ComPtr<ID2D1LinearGradientBrush> m_foregroundGradientBrushRight;

    ComPtr<ID2D1SolidColorBrush> m_solidBrush;
    ComPtr<ID2D1Bitmap> m_arrowBitmap;
    ComPtr<ID2D1Bitmap> m_defaultThumbnailBitmap;
    RenderingParameters m_renderingParameters;

    // DirectWrite resources
    ComPtr<IDWriteTextFormat> m_textFormat;
    ComPtr<IDWriteTextLayout> m_textLayout;

    // Thumbnails
    std::vector<ComPtr<IThumbnail>> m_thumbnailControls;

    // Unique animation controller
    ComPtr<IPanAnimation> m_panAnimation;
    ComPtr<IMediaPaneAnimation> m_animationController;
    
    // Track the current animation type
    AnimationType m_currentAnimation;
    D2D1_SIZE_F m_lastAnimationSize;
    D2D1_SIZE_F m_previousRenderTargetSize;

private:
    // Constants
    static const float ArrowGutterSize;
    static const float FontSizeRatio;

    // Clear thumbnail controls
    HRESULT RemoveAllItems();

    // Create/Descard D2D resources
    HRESULT CreateDeviceIndependentResources();
    HRESULT CreateDeviceResources();
    HRESULT DiscardDeviceResources();
    void PrepareBackgroundBrush(D2D1_SIZE_F size);

    // Layout Operations
    void CreateThumbnailCells(bool resetPanPosition);
    void UpdateTextLayout();
    
    // Basic image operations
    void PreviousPage();
    void NextPage();
    void Zoom(float zoomFactor);
    void PanImage(D2D1_POINT_2F panLocation, unsigned long flags);

    // Draw operations
    void DrawArrows();
    void DrawClientArea();
    void InvalidateWindow();

    // Animation operations:
    void DrawAnimatedThumbnailCells();
    HRESULT RenderScrollingAnimation(float distance);
    HRESULT RenderAnimation();
    HRESULT SetupAnimation(AnimationType animationType, D2D1_SIZE_F size);
    HRESULT GetVisibleThumbnailCells(std::vector<ThumbnailCell> &cells);

    // Helper functions:
    void SetSlideShowMode(bool isSlideShow, unsigned int imageIndex);
    float GetThumbnailSize();
    float GetFontSize();
    void CalculateArrowRectangles();
    std::vector<std::wstring> GetSelectedImageList();

protected:
    // Events
    HRESULT OnRender();
    HRESULT OnEraseBackground();
    HRESULT OnSize(unsigned int width, unsigned int height);
    HRESULT OnKeyDown(unsigned int vKey);
    HRESULT OnLeftMouseButtonDown(D2D1_POINT_2F mousePos);
    HRESULT OnLeftMouseButtonUp(D2D1_POINT_2F mousePosition);
    HRESULT OnLeftMouseButtonDoubleClick(D2D1_POINT_2F mousePos);
    HRESULT OnMouseMove(D2D1_POINT_2F mousePosition);
    HRESULT OnMouseEnter(D2D1_POINT_2F mousePosition);
    HRESULT OnMouseWheel(D2D1_POINT_2F mousePosition, short delta, int keys);
    HRESULT OnAppCommandBrowserBackward();
    HRESULT OnAppCommandBrowserForward();
    HRESULT OnPan(D2D1_POINT_2F location, unsigned long flags);
    HRESULT OnZoom(float zoomFactor);

    MediaPaneMessageHandler();
    virtual ~MediaPaneMessageHandler();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IPane>::CastTo(iid, this, object) ||
            CastHelper<IMediaPane>::CastTo(iid, this, object) ||
            CastHelper<IWindowLayoutChild>::CastTo(iid, this, object) ||
            CastHelper<IChildNotificationHandler>::CastTo(iid, this, object) ||
            CastHelper<Hilo::AsyncLoader::IAsyncLoaderMemoryManagerClient>::CastTo(iid, this, object) ||
            CastHelper<IInitializable>::CastTo(iid, this, object) ||
            Hilo::WindowApiHelpers::WindowMessageHandler::QueryInterfaceHelper(iid, object);
    }

    HRESULT __stdcall Initialize();
public:
    // Constants
    static const int BackgroundColor = 0xB8BEFC;

    // IPane
    HRESULT __stdcall SetCurrentLocation(IShellItem* shellFolder, bool recursive);

    // IMediaPane implemenation
    HRESULT __stdcall LaunchAnnotator();
    HRESULT __stdcall ShareImages();

    // IWindowLayoutChild
    HRESULT __stdcall SetWindowLayout(IWindowLayout* layout)
    {
        m_windowLayout = layout;
        return S_OK;
    }
    HRESULT __stdcall Finalize();

    // IChildNotificationHandler
    HRESULT __stdcall OnChildChanged() ;

    // IAsyncLoaderMemoryManagerClient
    HRESULT __stdcall GetClientItemSize(unsigned int* clientItemSize);
};
