//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "CarouselThumbnail.h"
#include "Animation.h"
#include "AsyncLoaderHelper.h"
#include "WindowLayout.h"
#include "WindowLayoutChildInterface.h"
#include "CarouselPaneWindowInterface.h"
#include "WindowMessageHandlerImpl.h"

struct CarouselHistoryItem
{
    ComPtr<IThumbnail> Thumbnail;
    ComPtr<ICarouselThumbnailAnimation> ThumbnailAnimation;
    ComPtr<IOrbitAnimation> OrbitAnimation;
};

struct MouseMoveInfo
{
    D2D1_POINT_2F point;
    UI_ANIMATION_SECONDS time;
};

enum MouseMoveDirection
{
    None = 0,
    Left = 1,
    Right = 2
};

class CarouselPaneMessageHandler :
    public IPane,
    public IWindowLayoutChild,
    public ICarouselPaneWindow,
    public Hilo::WindowApiHelpers::IChildNotificationHandler,
    public Hilo::AsyncLoader::IAsyncLoaderMemoryManagerClient,
    public IInitializable,
    public Hilo::WindowApiHelpers::WindowMessageHandler
{
public:

    // Constants
    static const float MaxThumbnailSize;
    static const float KeyRotateSize;

    // IPane implementation
    HRESULT __stdcall SetCurrentLocation(IShellItem* shellFolder, bool recursive);

    // IWindowLayoutChild implementation
    HRESULT __stdcall SetWindowLayout(IWindowLayout* layout);
    HRESULT __stdcall Finalize();

    // ICarouselPane implementation
    HRESULT __stdcall SetMediaPane(IPane* mediaPane);

    // IChildNotificationHandler
    HRESULT __stdcall OnChildChanged();

    // IAsyncLoaderMemoryManagerClient
    HRESULT __stdcall GetClientItemSize(unsigned int* clientItemSize);

protected:
    // Contructors/Destructor
    CarouselPaneMessageHandler();
    virtual ~CarouselPaneMessageHandler();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return 
            CastHelper<IPane>::CastTo(iid, this, object) ||
            CastHelper<IWindowLayoutChild>::CastTo(iid, this, object) ||
            CastHelper<ICarouselPaneWindow>::CastTo(iid, this, object) ||
            CastHelper<Hilo::WindowApiHelpers::IChildNotificationHandler>::CastTo(iid, this, object) ||
            CastHelper<Hilo::AsyncLoader::IAsyncLoaderMemoryManagerClient>::CastTo(iid, this, object) ||
            CastHelper<IInitializable>::CastTo(iid, this, object) ||
            Hilo::WindowApiHelpers::WindowMessageHandler::QueryInterfaceHelper(iid, object);
    }

    // IInitiliazable
    HRESULT __stdcall Initialize();

    // WindowMessageHandler Events
    HRESULT OnCreate();
    HRESULT OnEraseBackground();
    HRESULT OnRender();
    HRESULT OnSize(unsigned int width, unsigned int height);
    HRESULT OnLeftMouseButtonDown(D2D1_POINT_2F mousePosition);
    HRESULT OnLeftMouseButtonUp(D2D1_POINT_2F mousePosition);
    HRESULT OnMouseMove(D2D1_POINT_2F mousePosition);
    HRESULT OnMouseEnter(D2D1_POINT_2F mousePosition);
    HRESULT OnMouseWheel(D2D1_POINT_2F mousePosition, short delta, int keys);
    HRESULT OnKeyDown(unsigned int vKey);

private:
#ifdef _MEASURE_FPS
    // timer calculations
    SYSTEMTIME  m_startAnimationTime;
    unsigned short m_totalFramesRendered;
    std::ofstream m_logFile;
#endif

    // Constants
    static const float ThumbnailWidth;
    static const float ThumbnailHeight;
    static const float ApplicationButtonSize;
    static const float ApplicationButtonMargin;
    static const float ApplicationButtonSelectionMargin;
    static const float HistoryThumbnailWidth;
    static const float HistoryThumbnailHeight;
    static const float CarouselPaneMarginSize;
    static const float OrbitMarginXSize;
    static const float OrbitMarginYSize;

    static const int MaxHistoryIncrement;
    static const int CarouselSpeedFactor;
    static const int BackgroundColor;

    static const double InnerOrbitOpacity;
    static const double HistoryExpansionTime;

    static const float BackButtonWidth;
    static const float BackButtonHeight;
    static const float HistoryThumbnailMarginTop;
    static const float HistoryThumbnailMarginLeft;
    static const float MaxInnerOrbitHeight;
    static const float ThumbnailTextHeight;

    // Asynchronous loading support
    ComPtr<Hilo::AsyncLoader::IAsyncLoaderHelper> m_AsyncLoaderHelper;

    MouseMoveDirection m_MouseDirection;
    bool m_isRotationClockwise;
    D2D1_POINT_2F m_mouseDownPoint;
    float m_carouselSpinValue;
    std::deque<MouseMoveInfo> m_mouseMovePoints;

    // Animation variables
    ComPtr<IOrbitAnimation> m_innerOrbitAnimation;
    ComPtr<ICarouselAnimation> m_carouselAnimation;

    // Window state/objects
    ComPtr<IWindowLayout> m_windowLayout;
    bool m_isHistoryExpanded;
    bool m_updatingFolder;
    D2D1_RECT_F m_backButtonRect;
    D2D1_RECT_F m_annotateButtonImageRect;
    D2D1_ROUNDED_RECT m_annotateButtonSelectionRect;
    D2D1_RECT_F m_shareButtonImageRect;
    D2D1_ROUNDED_RECT m_shareButtonSelectionRect;

    // Application button states
    bool m_isAnnotatorButtonMouseHover;
    bool m_isSharingButtonMouseHover;

    // Geometry / Direct2d render resources
    ComPtr<ID2D1Factory> m_d2dFactory;
    ComPtr<IDWriteFactory> m_dWriteFactory;
    ComPtr<ID2D1HwndRenderTarget> m_renderTarget;
    ComPtr<ID2D1RadialGradientBrush> m_radialGradientBrush;
    ComPtr<ID2D1LinearGradientBrush> m_backgroundLinearGradientBrush;
    ComPtr<ID2D1SolidColorBrush> m_fontBrush;
    ComPtr<ID2D1SolidColorBrush> m_selectionBrush;
    ComPtr<IDWriteTextFormat> m_textFormat;
    ComPtr<IDWriteTextLayout> m_textLayoutAnnotate;
    ComPtr<IDWriteTextLayout> m_textLayoutShare;
    ComPtr<ID2D1Bitmap> m_defaultFolderBitmap;
    ComPtr<ID2D1Bitmap> m_arrowBitmap;
    ComPtr<ID2D1Bitmap> m_annotatorButtonImage;
    ComPtr<ID2D1Bitmap> m_sharingButtonImage;

    RenderingParameters m_renderingParameters;

    // Carousel Items
    std::vector<ComPtr<IThumbnail> > m_carouselItems;
    std::vector<CarouselHistoryItem> m_carouselHistoryItems;

    // Media Pane
    ComPtr<IPane> m_mediaPane;

    // Item list maintenance
    HRESULT RemoveAllItems();

    // Animation methods
    HRESULT AnimateHistoryAddition(bool isUpdatingWindowSize);
    HRESULT AnimateHistoryExpansion();
    HRESULT AnimateNewFolderSet();
    HRESULT RotateCarousel(double speed);
    HRESULT UpdateCarouselLocation(double rotationLocation);
    HRESULT NavigateBack();
    HRESULT NavigateToHistoryItem(unsigned int historyItemIndex);

    // Direct2D methods
    HRESULT CreateDeviceResources();
    HRESULT CreateDeviceIndependentResources();
    HRESULT DiscardDeviceResources();

    // Rendering methods
    HRESULT DrawClientArea();
    void DrawHistoryItems();
    void DrawOrbitItems();
    void DrawSelectionBox(D2D1_ROUNDED_RECT rect);

    HRESULT ResetOrbitValues();

    // Calculations
    D2D1_ELLIPSE CalculateInnerOrbit();
    D2D1_ELLIPSE CalculateHistoryOrbit(unsigned int index);
    double CalculateHistoryOrbitOpacity(unsigned int index);
    double CalculateHistoryThumbnailOpacity(unsigned int index);
    D2D1_POINT_2F CalculateHistoryThumbnailPoint(unsigned int index);
    D2D1_POINT_2F CalculatePointAtAngle(D2D1_ELLIPSE* ellipse, double angle);
    void CalculateApplicationButtonRects();

    // Carousel add/remove methods
    HRESULT AddOrbitItem(IThumbnail* item);
    HRESULT AddHistoryItem(IThumbnail* item);
    HRESULT AddHistoryItem(unsigned int orbitItemIndex);
    void InvalidateWindow();

    // Mouse methods
    HRESULT CheckForMouseHover(D2D1_POINT_2F mousePosition);
    HRESULT ClearMouseHover();
};
