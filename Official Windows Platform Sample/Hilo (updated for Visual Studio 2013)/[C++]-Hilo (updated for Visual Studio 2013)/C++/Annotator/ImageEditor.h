//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include "PointAnimation.h"
#include "ShellItemsLoader.h"
#include "SimpleImage.h"
#include "WindowMessageHandlerImpl.h"

struct ImageItem
{
    ComPtr<IImage> Image;
    ComPtr<IPointAnimation> Animation;
};

class ImageEditorHandler :
    public IImageEditor,
    public Hilo::WindowApiHelpers::WindowMessageHandler
{
public:
    // IImageEditor
    HRESULT __stdcall SetCurrentLocation(IShellItem* shellFolder, bool recursive);
    HRESULT __stdcall SetCurrentLocationFromCommandLine();
    HRESULT __stdcall SetDrawingOperation(__in ImageOperationType imageDrawingOperation);
    HRESULT __stdcall GetDrawingOperation(__out ImageOperationType* imageDrawingOperation);
    HRESULT __stdcall SetUIFramework(__in IUIFramework* framework);
    HRESULT __stdcall UpdateUIFramework();
    HRESULT __stdcall SetPenColor(__in D2D1_COLOR_F penColor);
    HRESULT __stdcall SetPenSize(__in float penSize);
    HRESULT __stdcall OpenFile();
    HRESULT __stdcall SaveFiles();
    HRESULT __stdcall SaveFileAs();
    HRESULT __stdcall ZoomIn();
    HRESULT __stdcall ZoomOut();
    HRESULT __stdcall ZoomFull();
    HRESULT __stdcall CanUndo(__out bool* canUndo);
    HRESULT __stdcall CanRedo(__out bool* canRedo);
    HRESULT __stdcall Undo();
    HRESULT __stdcall Redo();

protected:
    // Constructor/destructor
    ImageEditorHandler();
    virtual ~ImageEditorHandler();

    // Interface helpers
    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return
            CastHelper<IImageEditor>::CastTo(iid, this, object) ||
                Hilo::WindowApiHelpers::WindowMessageHandler::QueryInterfaceHelper(iid, object);
    }

    // Events
    HRESULT OnAppCommandBrowserBackward();
    HRESULT OnAppCommandBrowserForward();

    HRESULT OnCreate();
    HRESULT OnEraseBackground();
    HRESULT OnRender();
    HRESULT OnSize(unsigned int width, unsigned int height);

    HRESULT OnKeyDown(unsigned int vKey);
    HRESULT OnLeftMouseButtonDown(D2D1_POINT_2F mousePos);
    HRESULT OnLeftMouseButtonUp(D2D1_POINT_2F mousePosition);
    HRESULT OnMouseWheel(D2D1_POINT_2F mousePosition, short delta, int keys);
    HRESULT OnMouseMove(D2D1_POINT_2F mousePosition);
    HRESULT OnMouseEnter(D2D1_POINT_2F mousePosition);

    HRESULT OnCommand(WPARAM wParam, LPARAM /*lParam*/);
private:
#ifdef _MEASURE_FPS
    // timer calculations
    SYSTEMTIME  m_startAnimationTime;
    unsigned short m_totalFramesRendered;
    std::ofstream m_logFile;// can be merged to std::ofstream file("file.txt");
#endif

    // Constants
    static const int BackgroundColor;
    static const int PreviousNextImageRangeCount;

    static const float ImageMargin;
    static const float KeyboardPanDistance;
    static const float PreviousNextImageMargin;
    static const float SlideAnimationDuration;
    static const float TransformationAnimationDuration;
    static const float ZoomMinimum;
    static const float ZoomMaximum;
    static const float ZoomStep;

    // Factories
    ComPtr<ID2D1Factory> m_d2dFactory;
    ComPtr<IDWriteFactory> m_dWriteFactory;

    // Direct2D rendering resources
    RenderingParameters m_drawingObjects;
    ComPtr<ID2D1HwndRenderTarget> m_renderTarget;
    ComPtr<ID2D1SolidColorBrush> m_solidBrush;
    ComPtr<ID2D1LinearGradientBrush> m_foregroundGradientBrushLeft;
    ComPtr<ID2D1LinearGradientBrush> m_foregroundGradientBrushRight;
    ComPtr<ID2D1StrokeStyle> strokeStyleCustomOffsetZero;
    ComPtr<IDWriteTextFormat> m_textFormat;
    ComPtr<IDWriteTextLayout> m_textLayout;

    // Direct2D rendering parameters
    RenderingParameters m_renderingParameters;

    // Editor image
    std::vector<ImageItem> m_images;

    // Image information
    int m_currentRangeStart;
    int m_currentIndex;
    int m_currentRangeEnd;
    float m_currentZoom;
    float m_maxSlideDistance;

    D2D1_POINT_2F m_currentPanPoint;
    D2D1_RECT_F m_currentPanBoundary;
    D2D1_RECT_F m_imageBoundaryRect;
    D2D1_RECT_F m_currentClipBoundary;
    D2D1_RECT_F m_currentClipDrawBox;
    D2D1_COLOR_F m_penColor;
    float m_penSize;

    // Animation information
    bool m_animationEnabled;
    bool m_switchingImages;
    ComPtr<IUIAnimationManager> m_animationManager;
    ComPtr<IUIAnimationTransitionLibrary> m_transitionLibrary;
    ComPtr<IUIAnimationVariable> m_transformationAnimationVariable;

    // Mouse information
    D2D1_POINT_2F m_mouseDownPosition;
    D2D1_POINT_2F m_previousMousePosition;

    // Drawing operations
    ImageOperationType m_currentDrawingOperationType;
    ImageOperationType m_prevDrawingOperationType;
    ComPtr<IImageOperation> m_currentOperation;
    bool m_isDrawing;
    bool m_isClipping;
    bool m_startClipping;
    bool m_isRotation;
    bool m_isFlip;

    // Ribbon framework
    ComPtr<IUIFramework> m_framework;

    // Message method
    void SetMessage(std::wstring message);

    // Direct2D resources
    HRESULT CreateDeviceIndependentResources();
    HRESULT CreateDeviceResources();
    HRESULT DiscardDeviceResources();
    HRESULT ManageImageResources();

    // Calculate methods
    HRESULT CalculatePanBoundary();
    HRESULT CalculateImagePositions();

    // Scroll/Zoom operations
    bool PreviousImage();
    bool NextImage();
    void PanImage(D2D1_POINT_2F offset, bool snapToBounds);

    // Draw operations
    void DrawClientArea();
    void DrawForeground();
    void InvalidateWindow();

    // Animation methods
    void SetupAnimation();
    void SetupTransformationAnimation();
    void CleanupAnimation();

    // Load/Save operations
    HRESULT Reset();
    HRESULT LoadShellItems(const std::vector<ComPtr<IShellItem> >* shellItems, IShellItem* currentItem);

    // Save methods
    HRESULT SaveFileAtIndex(int index);
    void ShowSaveFailure(int imageIndex);

    // Draw methods
    void DrawAnimatedImages(int imageIndex);
    void DrawImages(int imageIndex);


    // Update Thumbnail on the taskbar
    HRESULT UpdateTaskbarThumbnail();

    D2D1_POINT_2F GetAbsolutePosition(D2D1_POINT_2F mousePosition);

    D2D1_POINT_2F RemoveRenderingTransformations(D2D1_POINT_2F mousePosition);

    inline D2D1_POINT_2F GetCenter()
    {
        return D2D1::Point2F(
            m_renderTarget->GetSize().width / 2,
            m_renderTarget->GetSize().height / 2);
    }

    void ImageEditorHandler::UpdateMouseCursor(D2D1_POINT_2F mousePosition);
    bool IsImageHit(D2D1_POINT_2F mousePoint);
    D2D1_POINT_2F AdjustToClipRect(D2D1_POINT_2F absPoint);
};
