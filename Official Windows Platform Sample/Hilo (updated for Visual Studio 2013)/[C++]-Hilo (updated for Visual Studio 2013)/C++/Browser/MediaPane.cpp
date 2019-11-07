//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "MediaPane.h"
#include "FlyerAnimation.h"
#include "MoverAnimation.h"
#include "ShellItemsLoader.h"
#include "WindowLayout.h"
#include "ShareDialog.h"

using namespace Hilo::AnimationHelpers;
using namespace Hilo::AsyncLoader;
using namespace Hilo::Direct2DHelpers;
using namespace Hilo::WindowApiHelpers;

const float MediaPaneMessageHandler::ArrowGutterSize = 48.0f;
const float MediaPaneMessageHandler::FontSizeRatio = 0.1f;

MediaPaneMessageHandler::MediaPaneMessageHandler() :
    m_windowLayout(nullptr),
    m_leftArrowSelected(false),
    m_rightArrowSelected(false),
    m_leftArrowClicked(false),
    m_rightArrowClicked(false),
    m_enablePanAnimation(false),
    m_enableAnimation(false),
    m_disableAnimation(false),
    m_currentAnimation(FlyIn),
    m_updatingFolder(false),
    m_isSlideShowMode(false),
    m_mouseDownSlideShowMode(false),
    m_inertiaHandled(false),
    m_previousRenderTargetSize(D2D1::SizeF(0.0f, 0.0f))
{
#ifdef _MEASURE_FPS
    m_logFile.open("mediapane-fps.log", std::ofstream::app);

    SYSTEMTIME currentTime;
    ::GetLocalTime(&currentTime);
    m_logFile << "MediaPane FPS Logging started @ " << currentTime.wHour << ":" << currentTime.wMinute << ":" << currentTime.wSecond <<"\r\n" ;
#endif
}

MediaPaneMessageHandler::~MediaPaneMessageHandler()
{
}

HRESULT MediaPaneMessageHandler::Initialize()
{
    HRESULT hr = CreateDeviceIndependentResources();

    if (SUCCEEDED(hr))
    {
        hr = SharedObject<AsyncLoaderHelper>::Create(&m_AsyncLoaderHelper);
    }

    if (SUCCEEDED(hr))
    {
        hr = SharedObject<ThumbnailLayoutManager>::Create(m_AsyncLoaderHelper, &m_thumbnailLayoutManager);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_thumbnailLayoutManager->SetArrowGutterSize(ArrowGutterSize);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_AsyncLoaderHelper->ConnectClient(this);
    }

    return hr;
}

//
//  This method creates resources which are bound to a particular
//  Direct2D render target.
//
HRESULT MediaPaneMessageHandler::CreateDeviceResources()
{
    HRESULT hr = S_OK;

    if (nullptr == m_renderTarget)
    {
        ComPtr<IWindow> window;

        hr = GetWindow(&window);
        if (SUCCEEDED(hr))
        {
            RECT rect;

            hr = window->GetClientRect(&rect);
            if (SUCCEEDED(hr))
            {
                HWND hWnd;

                hr = window->GetWindowHandle(&hWnd);
                if (SUCCEEDED(hr))
                {
                    // Create a Direct2D render target
                    hr = m_d2dFactory->CreateHwndRenderTarget(
                        RenderTargetProperties(),
                        HwndRenderTargetProperties(hWnd, SizeU(rect.right, rect.bottom)),
                        &m_renderTarget);
                }
            }
        }

        D2D1_GRADIENT_STOP gradientStops[2];
        gradientStops[0].color = ColorF(ColorF::White);
        gradientStops[0].position = 0.75f;
        gradientStops[1].color = ColorF(BackgroundColor);
        gradientStops[1].position = 1;

        ComPtr<ID2D1GradientStopCollection> gradientStopCollection;
        if (SUCCEEDED(hr))
        {
            hr = m_renderTarget->CreateGradientStopCollection(
                gradientStops,
                2,
                D2D1_GAMMA_2_2,
                D2D1_EXTEND_MODE_CLAMP,
                &gradientStopCollection);
        };

        D2D1_SIZE_F renderTargetSize = m_renderTarget->GetSize();

        if (SUCCEEDED(hr))
        {
            hr = m_renderTarget->CreateLinearGradientBrush(
                LinearGradientBrushProperties(
                Point2F(renderTargetSize.width, 0),
                Point2F(renderTargetSize.width, renderTargetSize.height)),
                gradientStopCollection,
                &m_backgroundLinearGradientBrush);
        }

        //Create gradient stop collection for both left and right gradients
        ComPtr<ID2D1GradientStopCollection> fadeoutGradientStopCollection;
        if (SUCCEEDED(hr))
        {
            D2D1_GRADIENT_STOP fadeoutGradientStops[3];
            fadeoutGradientStops[0].color = D2D1::ColorF(ColorF::White, 1);
            fadeoutGradientStops[0].position = 0.0f;
            fadeoutGradientStops[1].color = D2D1::ColorF(ColorF::White, 1);
            fadeoutGradientStops[1].position = 0.3f;
            fadeoutGradientStops[2].color = D2D1::ColorF(ColorF::White, 0);
            fadeoutGradientStops[2].position = 1.0f;

            hr = m_renderTarget->CreateGradientStopCollection(
                fadeoutGradientStops,
                3,
                D2D1_GAMMA_2_2,
                D2D1_EXTEND_MODE_CLAMP,
                &fadeoutGradientStopCollection);

            if (SUCCEEDED(hr))
            {
                // Create brush for left gradient
                hr = m_renderTarget->CreateLinearGradientBrush(
                    D2D1::LinearGradientBrushProperties(
                    D2D1::Point2F(0, 0),
                    D2D1::Point2F(ArrowGutterSize, 0)),
                    fadeoutGradientStopCollection,
                    &m_foregroundGradientBrushLeft);
            }

            // Create brush for right gradient
            if (SUCCEEDED(hr))
            {
                hr = m_renderTarget->CreateLinearGradientBrush(
                    D2D1::LinearGradientBrushProperties(
                    D2D1::Point2F(m_renderTarget->GetSize().width, 0),
                    D2D1::Point2F(m_renderTarget->GetSize().width - ArrowGutterSize, 0)),
                    fadeoutGradientStopCollection,
                    &m_foregroundGradientBrushRight);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = m_renderTarget->CreateSolidColorBrush(ColorF(ColorF::Black), &m_solidBrush);
        }

        if (SUCCEEDED(hr))
        {
            hr = Direct2DUtility::LoadBitmapFromResource(m_renderTarget, L"ArrowImage", L"PNG", 0, 0, &m_arrowBitmap);
        }

        if (SUCCEEDED(hr))
        {
            hr = Direct2DUtility::LoadBitmapFromResource(m_renderTarget, L"DefaultThumbNailImage", L"PNG", 0, 0, &m_defaultThumbnailBitmap);
        }

        if (SUCCEEDED(hr))
        {
            // Update thumbnail layout manager with current render target size
            hr = m_thumbnailLayoutManager->SetRenderTargetSize(renderTargetSize);
        }

        if (SUCCEEDED(hr))
        {
            CreateThumbnailCells(true);
            CalculateArrowRectangles();

            // Set current render parameters
            m_renderingParameters.solidBrush = m_solidBrush;
            m_renderingParameters.renderTarget = m_renderTarget;

            // Make sure rendering parameters are updated in all thumbnails if they've already been created
            for (auto thumbnail = m_thumbnailControls.begin() ; thumbnail != m_thumbnailControls.end(); thumbnail++)
            {
                (*thumbnail)->SetRenderingParameters(m_renderingParameters);
            }

            // Start background loading of thumbnails
            hr = m_AsyncLoaderHelper->StartBackgroundLoading();
        }
    }

    return hr;
}

HRESULT MediaPaneMessageHandler::DiscardDeviceResources()
{
    for (auto thumbnail = m_thumbnailControls.begin(); thumbnail != m_thumbnailControls.end(); thumbnail++)
    {
        (*thumbnail)->DiscardResources();
    }

    m_arrowBitmap = nullptr;
    m_solidBrush = nullptr;
    m_backgroundLinearGradientBrush = nullptr;
    m_renderTarget = nullptr;

    return S_OK;
}

HRESULT MediaPaneMessageHandler::CreateDeviceIndependentResources()
{
    static const wchar_t initialText [] = L"Some Static Arbitary Text Here. This will be used as a file title placeholder to determine the text height.";

    HRESULT hr = Direct2DUtility::GetD2DFactory(&m_d2dFactory);
    if (SUCCEEDED(hr))
    {
        hr = Direct2DUtility::GetDWriteFactory(&m_dWriteFactory);
    }

    if (SUCCEEDED(hr))
    {
        // Create text format
        hr = m_dWriteFactory->CreateTextFormat(
            L"Arial",
            nullptr, // Use Windows fonts
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            10, // An arbitary font size that'll be overriden by the textLayout
            L"en-us",
            &m_textFormat);
    }

    if (SUCCEEDED(hr))
    {
        // Create an arbitary text layout object
        // Width and height will be determined later.
        hr = m_dWriteFactory->CreateTextLayout(
            initialText,
            ARRAYSIZE(initialText) - 1,
            m_textFormat,
            50,
            50,
            &m_textLayout);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_textLayout->SetWordWrapping(DWRITE_WORD_WRAPPING_WRAP);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_textLayout->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_textLayout->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);
    }

    if (SUCCEEDED(hr))
    {
        // Initialize animation controller with flyer animation
        hr = SharedObject<FlyerAnimation>::Create(&m_animationController);
    }

    if (SUCCEEDED(hr))
    {
        m_renderingParameters.textLayout = m_textLayout;
        m_renderingParameters.textFormat = m_textFormat;
    }

    return hr;
}

HRESULT MediaPaneMessageHandler::OnEraseBackground()
{
    // To prevent flickering, returning Success would indicate
    // this message has been handled, so background will not be erased
    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnRender()
{
    HRESULT hr = S_OK;

    if (m_enablePanAnimation || m_enableAnimation)
    {
        // Update animation manager with current time
        hr = AnimationUtility::UpdateAnimationManagerTime();

        // Invalidate the window now regardless of animation status to make
        // sure that the last frame of animation is drawn
        InvalidateWindow();

        // Render the client area
        DrawClientArea();

        if (SUCCEEDED(hr))
        {
            // Check if the animation manager is still processing storyboards
            // and continue rendering until the animation manager is idle
            bool isBusy = false;

            AnimationUtility::IsAnimationManagerBusy(&isBusy);
            if (!isBusy)
            {
                // Disable all animation flags
                m_enableAnimation = false;
                m_enablePanAnimation = false;
                m_updatingFolder = false;

                // Detect if we need to trigger the MoveAround animation
                if (!m_isSlideShowMode)
                {
                    if (m_renderTarget->GetSize().width != m_lastAnimationSize.width ||
                        m_renderTarget->GetSize().height != m_lastAnimationSize.height)
                    {
                        SetupAnimation(MoveAround, m_renderTarget->GetSize());
                    }
                }

#ifdef _MEASURE_FPS
                if (m_totalFramesRendered > 0)
                {
                    // Calculate elapsed time
                    SYSTEMTIME currentTime;
                    ::GetSystemTime(&currentTime);

                    unsigned long totalAnimationTime = (currentTime.wSecond * 1000 + currentTime.wMilliseconds) - (m_startAnimationTime.wSecond * 1000 + m_startAnimationTime.wMilliseconds);

                    double fps = 0;
                    if (totalAnimationTime > 0)
                    {
                        fps = m_totalFramesRendered / (totalAnimationTime / 1000.0);
                    }

                    m_logFile << "Frames per second for last media pane animation = ( " << m_totalFramesRendered << " frames /" << totalAnimationTime << " millis) = " << fps << " fps\r\n";
                    m_totalFramesRendered = 0;
                }
            }
            else // isBusy
            {
                m_totalFramesRendered++;
#endif
            }
        }
    }
    else
    {
        // Simply render the client area
        DrawClientArea();
    }

    return hr;
}

void MediaPaneMessageHandler::InvalidateWindow()
{
    // Setting this window to the bottom of the Z order gives the chance for other sibling windows to redraw if
    // necessary. Removing this call would prevent other windows from rendering animation until this window's
    // animation is complete
    ComPtr<IWindow> window;

    if (SUCCEEDED(GetWindow(&window)))
    {
        window->SetZOrder(Bottom);
        window->RedrawWindow();
    }
}

void MediaPaneMessageHandler::DrawClientArea()
{
    HRESULT hr = CreateDeviceResources();

    if (!(m_renderTarget->CheckWindowState() & D2D1_WINDOW_STATE_OCCLUDED))
    {
        m_renderTarget->BeginDraw();

        D2D1_MATRIX_3X2_F identity = Matrix3x2F::Identity();
        m_renderTarget->SetTransform(identity);

        D2D1_SIZE_F size = m_renderTarget->GetSize();

        // Paint background
        m_renderTarget->FillRectangle(RectF(0, 0, size.width, size.height), m_backgroundLinearGradientBrush);

        // Update panning for animation
        if (m_enablePanAnimation)
        {
            double currentPosition;
            if (SUCCEEDED(m_panAnimation->GetValue(&currentPosition)))
            {
                // Determine delta between this value and the previous value
                m_thumbnailLayoutManager->Pan(m_previousPanPositionX - static_cast<float>(currentPosition));

                // Save current pan position to calculate delta for next pan animation step
                m_previousPanPositionX = static_cast<float>(currentPosition);

                // Update thumbnail positions
                m_thumbnailLayoutManager->UpdateVisibleThumbnailControls(m_thumbnailControls);
            }
        }

        // Render current images
        if (m_enableAnimation)
        {
            DrawAnimatedThumbnailCells();
        }
        else
        {
            int currentIndex;
            int endIndex;

            hr = m_thumbnailLayoutManager->GetVisibleThumbnailRange(&currentIndex, &endIndex);
            if (SUCCEEDED(hr))
            {
                while (currentIndex < endIndex)
                {
                    m_thumbnailControls[currentIndex]->Draw();
                    currentIndex++;
                }
            }
        }

        if (!m_isSlideShowMode)
        {
            // Draw left gradient
            m_renderTarget->FillRectangle(
                D2D1::RectF(0, 0, ArrowGutterSize, m_renderTarget->GetSize().height),
                m_foregroundGradientBrushLeft);

            // Draw right gradient
            m_renderTarget->FillRectangle(
                D2D1::RectF(
                m_renderTarget->GetSize().width - ArrowGutterSize,
                0,
                m_renderTarget->GetSize().width,
                m_renderTarget->GetSize().height),
                m_foregroundGradientBrushRight);
        }

        // Draw the arrows
        DrawArrows();

        hr = m_renderTarget->EndDraw();
        if (hr == D2DERR_RECREATE_TARGET)
        {
            DiscardDeviceResources();
        }
    }
}

void MediaPaneMessageHandler::DrawArrows()
{
    int panState;

    HRESULT hr = m_thumbnailLayoutManager->GetPanState(&panState);
    if (SUCCEEDED(hr))
    {
        if ((panState & PanLeft) == PanLeft)
        {
            // Draw left arrow
            m_renderTarget->DrawBitmap(m_arrowBitmap, m_leftArrowRect, m_leftArrowSelected || m_leftArrowClicked ? 1.0f : 0.5f);
        }

        if ((panState & PanRight) == PanRight)
        {
            // Rotate the arrow 180 deg
            m_renderTarget->SetTransform(
                D2D1::Matrix3x2F::Rotation(
                180.0f,
                D2D1::Point2F(
                m_rightArrowRect.left + (m_rightArrowRect.right - m_rightArrowRect.left) / 2.0f, 
                m_rightArrowRect.top + (m_rightArrowRect.bottom - m_rightArrowRect.top) / 2.0f)));

            m_renderTarget->DrawBitmap(m_arrowBitmap, m_rightArrowRect, m_rightArrowSelected || m_rightArrowClicked ? 1.0f : 0.5f);
        }
    }
}

void MediaPaneMessageHandler::DrawAnimatedThumbnailCells()
{
    std::vector<AnimatedThumbnailCell> thumbnails;
    m_animationController->GetAnimatedThumbnailCells(thumbnails);

    for (auto thumbnail = thumbnails.begin(); thumbnail != thumbnails.end(); ++thumbnail)
    {
        D2D1_POINT_2F center;

        if (SUCCEEDED(m_animationController->GetAnimationPosition(thumbnail->cell.control, &center)))
        {
            float offsetX = Direct2DUtility::GetRectWidth(thumbnail->cell.position) / 2;
            float offsetY = Direct2DUtility::GetRectHeight(thumbnail->cell.position) / 2;

            D2D1_RECT_F position = {center.x - offsetX, center.y - offsetY, center.x + offsetX, center.y + offsetY };

            thumbnail->cell.control->SetRect(position);
            thumbnail->cell.control->SetIsFullImage(GetThumbnailSize() > ThumbnailLayoutManager::MaxThumbnailSize);
            thumbnail->cell.control->Draw();
        }
    }
}

HRESULT MediaPaneMessageHandler::OnSize(unsigned int width, unsigned int height)
{
    HRESULT hr = S_OK;

    if (width == 0 || height == 0)
    {
        // Ignore this message if either width or height equal zero. This occurs when the media pane has
        // no room to show or the main application has been minimized.
        return hr;
    }

    if (m_renderTarget)
    {
        // Capture previous size
        m_previousRenderTargetSize = m_renderTarget->GetSize();

        // Resize the render target
        m_renderTarget->Resize(D2D1::SizeU(width, height));

        if (!m_updatingFolder)
        {
            if (m_disableAnimation)
            {
                m_thumbnailLayoutManager->SetRenderTargetSize(m_renderTarget->GetSize());
                m_thumbnailLayoutManager->UpdateVisibleThumbnailControls(m_thumbnailControls);
            }
            else
            {
                SetupAnimation(MoveAround, m_renderTarget->GetSize());
            }
        }

        PrepareBackgroundBrush(m_renderTarget->GetSize());
        CalculateArrowRectangles();
    }

    return hr;
}

HRESULT MediaPaneMessageHandler::RenderScrollingAnimation(float distance)
{
    HRESULT hr = S_OK;

    if (m_enablePanAnimation)
    {
        // Don't kick off another pan animation until the current one is complete
        return hr;
    }

    if (nullptr == m_panAnimation)
    {
        hr = SharedObject<PanAnimation>::Create(&m_panAnimation);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_panAnimation->Setup(0, distance, 0.25);

        m_previousPanPositionX = 0;
        m_enablePanAnimation = true;

#ifdef _MEASURE_FPS
        // Setup animation time
        ::GetSystemTime(&m_startAnimationTime);
        m_totalFramesRendered = 0;
#endif
    }

    InvalidateWindow();

    return hr;
}

HRESULT MediaPaneMessageHandler::OnLeftMouseButtonDown(D2D1_POINT_2F mousePosition)
{
    // Capture mouse
    ComPtr<IWindow> window;
    if (SUCCEEDED(GetWindow(&window)))
    {
        window->SetCapture();
    }

    // Capture which mode the media pane is in when the mouse button is pressed. This is
    // needed in order to ignore pan requests when the mode is switch via double-click
    m_mouseDownSlideShowMode = m_isSlideShowMode;

    if (!m_enablePanAnimation)
    {
        PanImage(mousePosition, GF_BEGIN);
    }

    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnLeftMouseButtonUp(D2D1_POINT_2F mousePosition)
{
    bool redraw = false;

    // Release mouse capture
    ::ReleaseCapture();

    if (Direct2DUtility::HitTest(m_leftArrowRect, mousePosition))
    {
        PreviousPage();
        redraw = true;
    }
    else if (Direct2DUtility::HitTest(m_rightArrowRect, mousePosition))
    {
        NextPage();
        redraw = true;
    }
    else if (m_isSlideShowMode)
    {
        if (m_mouseDownSlideShowMode == m_isSlideShowMode)
        {
            PanImage(mousePosition, GF_END);
        }
    }
    else
    {
        int currentIndex;
        int endIndex;

        HRESULT hr = m_thumbnailLayoutManager->GetVisibleThumbnailRange(&currentIndex, &endIndex);
        if (SUCCEEDED(hr))
        {
            // Determine if one of the visible thumbnail controls was clicked
            for(currentIndex; currentIndex < endIndex; currentIndex++)
            {
                D2D1_RECT_F positionRect;
                hr = m_thumbnailControls[currentIndex]->GetRect(&positionRect);
                if (SUCCEEDED(hr))
                {
                    // Adjust rectangle for textbox
                    positionRect.bottom += m_textLayout->GetMaxHeight();
                }

                ThumbnailSelectionState selectionState = SelectionStateNone;
                if (SUCCEEDED(hr))
                {
                    hr = m_thumbnailControls[currentIndex]->GetSelectionState(&selectionState);
                }

                if (SUCCEEDED(hr))
                {
                    if (Direct2DUtility::HitTest(positionRect, mousePosition))
                    {
                        if ((selectionState & SelectionStateSelected) == SelectionStateSelected)
                        {
                            selectionState = static_cast<ThumbnailSelectionState>(selectionState & ~SelectionStateSelected);
                            m_thumbnailControls[currentIndex]->SetSelectionState(selectionState);
                        }
                        else
                        {
                            selectionState = static_cast<ThumbnailSelectionState>(selectionState | SelectionStateSelected);
                            m_thumbnailControls[currentIndex]->SetSelectionState(selectionState);
                        }

                        redraw = true;
                        break;
                    }
                }
            }
        }
    }

    if (redraw)
    {
        InvalidateWindow();
    }

    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnLeftMouseButtonDoubleClick(D2D1_POINT_2F mousePos)
{
    HRESULT hr = S_OK;

    if (m_isSlideShowMode)
    {
        // Clicking anywhere except in arrows will switch to the normal view
        if (!Direct2DUtility::HitTest(m_leftArrowRect, mousePos) &&
            !Direct2DUtility::HitTest(m_rightArrowRect, mousePos))
        {
            SetSlideShowMode(false, 0);
        }
    }
    else
    {
        int currentIndex;
        int endIndex;

        hr = m_thumbnailLayoutManager->GetVisibleThumbnailRange(&currentIndex, &endIndex);
        if (SUCCEEDED(hr))
        {
            // Determine if one of the visible thumbnail controls was clicked
            for (currentIndex; currentIndex < endIndex; currentIndex++)
            {
                D2D1_RECT_F rect;

                hr = m_thumbnailControls[currentIndex]->GetRect(&rect);
                if (SUCCEEDED(hr))
                {
                    if (Direct2DUtility::HitTest(rect, mousePos))
                    {
                        SetSlideShowMode(true, currentIndex);
                        break;
                    }
                }
            }
        }
    }

    return hr;
}

//
// Sets the slide show mode specifying the image that should have the current focus when entering/leaving
// slideshow mode
//
void MediaPaneMessageHandler::SetSlideShowMode(bool isSlideShow, unsigned int imageIndex)
{
    m_isSlideShowMode = isSlideShow;

    if (isSlideShow)
    {
        // Disable all non-pan animations
        m_disableAnimation = true;
    }
    else
    {
        // Make sure memory is released for stored full images
        for (auto thumbnail = m_thumbnailControls.begin(); thumbnail != m_thumbnailControls.end(); thumbnail++)
        {
            (*thumbnail)->ReleaseFullImage();
        }
    }

    // Update image index if leaving slide show mode
    if (!m_isSlideShowMode)
    {
        int currentIndex;

        if (SUCCEEDED(m_thumbnailLayoutManager->GetVisibleThumbnailRange(&currentIndex, nullptr)))
        {
            imageIndex = currentIndex;
        }
    }

    // Updates the layout manager so that it no longer shows the carousel pane during slide show mode
    m_windowLayout->SwitchDisplayMode(m_isSlideShowMode);

    // Updates the layout manager so that is displays full size images during slide show mode
    m_thumbnailLayoutManager->SetSlideShowMode(m_isSlideShowMode);

    if (!m_isSlideShowMode)
    {
        m_disableAnimation = false;
        CreateThumbnailCells(true);
    }

    // Sets the current image index
    m_thumbnailLayoutManager->SetCurrentImage(imageIndex);
    m_thumbnailLayoutManager->UpdateVisibleThumbnailControls(m_thumbnailControls);

    InvalidateWindow();
}

HRESULT MediaPaneMessageHandler::OnMouseWheel(D2D1_POINT_2F /*mousePosition*/, short delta, int keys) 
{
    if ((keys & MK_CONTROL) == MK_CONTROL)
    {
        int steps = std::max(1, std::abs(delta / WHEEL_DELTA));

        for (int i = 0; i < steps; i++)
        {
            if (delta > 0)
            {
                Zoom(1.1f);
            }
            else
            {
                Zoom(0.9f);
            }
        }
    }
    else // scroll to the next page
    {
        if (delta < 0)
        {
            NextPage();
        }
        else
        {
            PreviousPage();
        }
    }

    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnMouseMove(D2D1_POINT_2F mousePosition)
{
    // Don't do calculations while animation is taking place
    if (m_enableAnimation)
    {
        return S_OK;
    }

    bool redraw = false;
    m_leftArrowSelected = false;
    m_rightArrowSelected = false;

    // Check if we're panning
    ComPtr<IWindow> window;
    if (SUCCEEDED(GetWindow(&window)))
    {
        bool isMouseCaptured;
        if SUCCEEDED(window->IsMouseCaptured(&isMouseCaptured))
        {
            if (isMouseCaptured)
            {
                PanImage(mousePosition, 0);
            }
        }
    }

    if (Direct2DUtility::HitTest(m_leftArrowRect, mousePosition))
    {
        m_leftArrowSelected = true;
        redraw = true;
    }
    else if (Direct2DUtility::HitTest(m_rightArrowRect, mousePosition))
    {
        m_rightArrowSelected = true;
        redraw = true;
    }
    else
    {
        int currentIndex;
        int endIndex;

        HRESULT hr = m_thumbnailLayoutManager->GetVisibleThumbnailRange(&currentIndex, &endIndex);
        if (SUCCEEDED(hr))
        {
            // Determine if one of the visible thumbnail controls
            for (;currentIndex < endIndex; currentIndex++)
            {
                D2D1_RECT_F positionRect;

                hr = m_thumbnailControls[currentIndex]->GetRect(&positionRect);
                if (SUCCEEDED(hr))
                {
                    // Adjust for textbox
                    positionRect.bottom += m_textLayout->GetMaxHeight();
                }

                ThumbnailSelectionState selectionState = SelectionStateNone;
                if (SUCCEEDED(hr))
                {
                    hr = m_thumbnailControls[currentIndex]->GetSelectionState(&selectionState);
                }

                if (SUCCEEDED(hr))
                {
                    if (Direct2DUtility::HitTest(positionRect, mousePosition))
                    {
                        // Only toggle hover if necessary
                        if ((selectionState & SelectionStateHoverOn) != SelectionStateHoverOn)
                        {
                            selectionState = static_cast<ThumbnailSelectionState>(selectionState | SelectionStateHoverOn);
                            m_thumbnailControls[currentIndex]->SetSelectionState(selectionState);
                            redraw = true;
                        }
                    }
                    else
                    {
                        // Only toggle hover if necessary
                        if ((selectionState & SelectionStateHoverOn) == SelectionStateHoverOn)
                        {
                            selectionState = static_cast<ThumbnailSelectionState>(selectionState & ~SelectionStateHoverOn);
                            m_thumbnailControls[currentIndex]->SetSelectionState(selectionState);
                            redraw = true;
                        }
                    }
                }
            }
        }
    }

    if (redraw)
    {
        InvalidateWindow();
    }

    return S_OK;
}

//
// Event that is fired when the mouse enters the client area
//
HRESULT MediaPaneMessageHandler::OnMouseEnter(D2D1_POINT_2F /*mousePosition*/)
{
    ::SetCursor(::LoadCursor(nullptr, IDC_ARROW));
    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnKeyDown(unsigned int vKey)
{
    if (vKey == VK_ADD || vKey == VK_OEM_PLUS)
    {
        Zoom(1.1f);
    }
    else if (vKey == VK_SUBTRACT || vKey == VK_OEM_MINUS)
    {
        Zoom(0.9f);
    }
    else if (vKey == VK_LEFT)
    {
        PreviousPage();
    }
    else if (vKey == VK_RIGHT)
    {
        NextPage();
    }

    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnAppCommandBrowserBackward()
{
    PreviousPage();
    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnAppCommandBrowserForward()
{
    NextPage();
    return S_OK;
}

void MediaPaneMessageHandler::PrepareBackgroundBrush(D2D1_SIZE_F size)
{
    m_backgroundLinearGradientBrush->SetStartPoint(Point2F(size.width, 0));
    m_backgroundLinearGradientBrush->SetEndPoint(Point2F(size.width, size.height));
    m_foregroundGradientBrushRight->SetStartPoint(Point2F(size.width, 0));
    m_foregroundGradientBrushRight->SetEndPoint(Point2F(size.width - ArrowGutterSize, 0));
}

void MediaPaneMessageHandler::CreateThumbnailCells(bool resetPanPosition)
{
    UpdateTextLayout();
    m_thumbnailLayoutManager->SetTextBoxHeight(m_textLayout->GetMaxHeight());
    m_thumbnailLayoutManager->CreateLayout(static_cast<int>(m_thumbnailControls.size()), resetPanPosition);
}

void MediaPaneMessageHandler::UpdateTextLayout()
{
    static DWRITE_TEXT_RANGE defaultTextRange = {0, _MAX_PATH};

    HRESULT hr = m_textLayout->SetMaxWidth(GetThumbnailSize());
    if (SUCCEEDED(hr))
    {
        m_textLayout->SetFontSize(GetFontSize(), defaultTextRange);
    }

    DWRITE_TEXT_METRICS textMetrics = { };
    if (SUCCEEDED(hr))
    {
        hr = m_textLayout->GetMetrics(&textMetrics);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_textLayout->SetMaxHeight(2 * (textMetrics.height / static_cast<float>(textMetrics.lineCount)));
    }
}

void MediaPaneMessageHandler::PreviousPage()
{
    RenderScrollingAnimation(-m_renderTarget->GetSize().width);
}

void MediaPaneMessageHandler::NextPage()
{
    RenderScrollingAnimation(m_renderTarget->GetSize().width);
}

void MediaPaneMessageHandler::Zoom(float zoomFactor)
{
    HRESULT hr = m_thumbnailLayoutManager->Zoom(zoomFactor);
    if (hr == S_FALSE)
    {
        // Return value of S_FALSE indicates that the thumbnail layout manager has reached
        // the minimum zoom size for slide show mode. Switch back to browser mode
        SetSlideShowMode(false, 0);
    }

    UpdateTextLayout();
    m_thumbnailLayoutManager->SetTextBoxHeight(m_textLayout->GetMaxHeight());
    m_thumbnailLayoutManager->UpdateVisibleThumbnailControls(m_thumbnailControls);
}

void MediaPaneMessageHandler::PanImage(D2D1_POINT_2F panLocation, unsigned long flags)
{
    // Don't update panning when the pan gesture is beginning. Simply capture current
    // pan position.
    if (GF_BEGIN == flags)
    {
        m_previousPanPositionX = panLocation.x;
        m_inertiaHandled = false;

        if (m_isSlideShowMode)
        {
            m_thumbnailLayoutManager->BeginSlideShowPan();
        }
    }
    else
    {
        if (m_isSlideShowMode && (GF_END == flags || GF_INERTIA == flags))
        {
            if (!m_inertiaHandled && !m_enablePanAnimation)
            {
                float distance;

                if (SUCCEEDED(m_thumbnailLayoutManager->EndSlideShowPan(&distance)))
                {
                    RenderScrollingAnimation(distance);
                }
            }

            // Panning during slideshow mode doesn't allow inertia since we're animating
            // to the nearest image (previous/next) based on distance panned
            m_inertiaHandled = true;
        }
        else if (!m_inertiaHandled)
        {
            m_thumbnailLayoutManager->Pan(panLocation.x - m_previousPanPositionX);

            // Store current pan postion for next pan message
            m_previousPanPositionX = panLocation.x;
        }
    }

    // Recalculate thumbnail layout
    m_thumbnailLayoutManager->UpdateVisibleThumbnailControls(m_thumbnailControls);

    // Invalidate window
    InvalidateWindow();
}

HRESULT MediaPaneMessageHandler::RemoveAllItems()
{
    m_AsyncLoaderHelper->ClearItems();
    m_thumbnailControls.clear();

    return S_OK;
}

HRESULT MediaPaneMessageHandler::SetCurrentLocation(IShellItem* shellFolder, bool recursive)
{
    HRESULT hr = S_OK;

    // Capture currently visible thumbnails for animation of previous thumbnails
    std::vector<ThumbnailCell> previousThumbnails;

    // Update layout manager with previous thumbnail layout size
    // This is needed for when the carousel pane adjusts it's height based on
    // the number of icons on the orbit. We want the previous thumbnail cells to be based on the
    // previous size and the current thumbnails to be based on the updated size
    if (m_previousRenderTargetSize.width > 0 || m_previousRenderTargetSize.height > 0)
    {
        hr = m_thumbnailLayoutManager->SetRenderTargetSize(m_previousRenderTargetSize);

        if (SUCCEEDED(hr))
        {
            hr = m_thumbnailLayoutManager->UpdateVisibleThumbnailControls(m_thumbnailControls);
        }

        if (SUCCEEDED(hr))
        {
            hr = GetVisibleThumbnailCells(previousThumbnails);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_thumbnailLayoutManager->SetRenderTargetSize(m_renderTarget->GetSize());
        }
    }
    else
    {
        hr = GetVisibleThumbnailCells(previousThumbnails);
    }

    if (SUCCEEDED(hr))
    {
        hr = RemoveAllItems();
    }

    // Create render target if necessary. This is needed to set default bitmaps and setup flyer animation
    if (SUCCEEDED(hr))
    {
        if (!m_renderTarget)
        {
            hr = CreateDeviceResources();
        }
    }

    // Get list of ShellItems for current directory
    std::vector<ComPtr<IShellItem> > shellItems;

    // Ignore the returned HRESULT because it's possible no elements are found in this folder
    ShellItemsLoader::EnumerateFolderItems(shellFolder, FileTypeImage, recursive, shellItems);

    for (auto shellItem = shellItems.begin(); shellItem != shellItems.end(); shellItem++)
    {
        ThumbnailInfo info(*shellItem);
        info.fileType = FileTypeImage;

        ComPtr<IThumbnail> imageThumbnailControl;
        if (SUCCEEDED(SharedObject<ImageThumbnail>::Create(info, &imageThumbnailControl)))
        {
            imageThumbnailControl->SetDefaultBitmap(m_defaultThumbnailBitmap);
            imageThumbnailControl->SetRenderingParameters(m_renderingParameters);
            imageThumbnailControl->SetParentWindow(this);

            m_thumbnailControls.push_back(imageThumbnailControl);

            if (SUCCEEDED(hr))
            {
                hr = m_AsyncLoaderHelper->ConnectItem(
                    imageThumbnailControl,
                    static_cast<unsigned int>(m_thumbnailControls.size()) - 1);
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        // Initialize thumbnail layout manager with new cells
        CreateThumbnailCells(true);
        hr = m_thumbnailLayoutManager->UpdateVisibleThumbnailControls(m_thumbnailControls);
    }

    std::vector<ThumbnailCell> currentThumbnails;
    if (SUCCEEDED(hr))
    {
        // Get currently visible thumbnails
        hr = GetVisibleThumbnailCells(currentThumbnails);
    }

    if (SUCCEEDED(hr))
    {
        // Initialize FlyIn animation
        m_enableAnimation = false;
        hr = SetupAnimation(FlyIn, m_renderTarget->GetSize());
    }

    if (SUCCEEDED(hr))
    {
        // Setup the FlyIn animation
        m_animationController->Setup(previousThumbnails, currentThumbnails, m_renderTarget->GetSize());
        m_updatingFolder = true;
    }

#ifdef _MEASURE_FPS
    // Setup animation time
    ::GetSystemTime(&m_startAnimationTime);
    m_totalFramesRendered = 0;
#endif

    return hr;
}

HRESULT MediaPaneMessageHandler::SetupAnimation(AnimationType animationType, D2D1_SIZE_F size)
{
    HRESULT hr = S_OK;

    // Don't start another animation if one if already running or we are in slide show mode
    if (m_enableAnimation || m_isSlideShowMode)
    {
        return hr;
    }

    if (m_currentAnimation != animationType)
    {
        if (nullptr != m_animationController)
        {
            m_animationController = nullptr;
        }

        m_currentAnimation = animationType;

        if (animationType == FlyIn)
        {
            hr = SharedObject<FlyerAnimation>::Create(&m_animationController);
        }
        else if (animationType == MoveAround)
        {
            hr = SharedObject<MoverAnimation>::Create(&m_animationController);
        }
    }

    // Capture current size when starting the current animation. This allows us to check if a MoveAround
    // animation is needed after animation is complete
    m_lastAnimationSize = size;

    if (SUCCEEDED(hr))
    {
        if (animationType == MoveAround)
        {
            std::vector<ThumbnailCell> thumbnailCells;
            std::vector<ThumbnailCell> prevThumbnailCells;

            // Capture currently visible thumbnails
            hr = GetVisibleThumbnailCells(prevThumbnailCells);
            if (SUCCEEDED(hr))
            {
                hr = m_thumbnailLayoutManager->SetRenderTargetSize(m_renderTarget->GetSize());
            }

            if (SUCCEEDED(hr))
            {
                // Update thumbnail cells
                CreateThumbnailCells(false);
                m_thumbnailLayoutManager->UpdateVisibleThumbnailControls(m_thumbnailControls);
            }

            if (SUCCEEDED(hr))
            {
                // Get updated thumbnail cells
                hr = GetVisibleThumbnailCells(thumbnailCells);
            }

            if (SUCCEEDED(hr))
            {
                hr = m_animationController->Setup(prevThumbnailCells, thumbnailCells, size);
            }
        }
    }

    m_enableAnimation = true;

    InvalidateWindow();

    return hr;
}

HRESULT MediaPaneMessageHandler::GetVisibleThumbnailCells(std::vector<ThumbnailCell> &cells)
{
    HRESULT hr = S_OK;
    int currentIndex;
    int endIndex;

    if (!m_thumbnailControls.empty())
    {
        hr = m_thumbnailLayoutManager->GetVisibleThumbnailRange(&currentIndex, &endIndex);

        if (SUCCEEDED(hr))
        {
            for (; currentIndex < endIndex; currentIndex++)
            {
                D2D1_RECT_F rect;
                hr = m_thumbnailControls[currentIndex]->GetRect(&rect);

                if (SUCCEEDED(hr))
                {
                    cells.push_back(ThumbnailCell(m_thumbnailControls[currentIndex], rect));
                }
            }
        }
    }

    return hr;
}

//
// IAsyncLoaderMemoryManagerClient
//
HRESULT MediaPaneMessageHandler::GetClientItemSize(unsigned int* clientItemSize)
{
    // Locking is not important for this resource
    *clientItemSize = static_cast<unsigned int>(GetThumbnailSize());
    return S_OK;
}

//
// IChildNotificationHandler
//
HRESULT MediaPaneMessageHandler::OnChildChanged() 
{
    InvalidateWindow();
    return S_OK;
}

HRESULT MediaPaneMessageHandler::Finalize()
{
    HRESULT hr = RemoveAllItems();
    if (SUCCEEDED(hr))
    {
        hr = m_AsyncLoaderHelper->Shutdown();
    }

    return hr;
}

//
// Helper functions
//
float MediaPaneMessageHandler::GetThumbnailSize()
{
    float thumbnailSize = 0;

    if (m_thumbnailLayoutManager)
    {
        m_thumbnailLayoutManager->GetThumbnailSize(&thumbnailSize);
    }

    return thumbnailSize;
}

float MediaPaneMessageHandler::GetFontSize()
{
    return FontSizeRatio * GetThumbnailSize();
}

void MediaPaneMessageHandler::CalculateArrowRectangles()
{
    // Calculate left arrow
    m_leftArrowRect.left = 0;
    m_leftArrowRect.right = ArrowGutterSize;
    m_leftArrowRect.top = m_renderTarget->GetSize().height / 2 - ArrowGutterSize / 2;
    m_leftArrowRect.bottom = m_leftArrowRect.top + ArrowGutterSize;

    // Calculate right arrow
    m_rightArrowRect.left = m_renderTarget->GetSize().width - ArrowGutterSize;
    m_rightArrowRect.right = m_rightArrowRect.left + ArrowGutterSize;
    m_rightArrowRect.top = m_leftArrowRect.top;
    m_rightArrowRect.bottom = m_leftArrowRect.bottom;
}

std::vector<std::wstring> MediaPaneMessageHandler::GetSelectedImageList()
{
    std::vector<std::wstring> imageList;

    for(int i = 0; i < static_cast<int>(m_thumbnailControls.size()); i++)
    {
        ThumbnailSelectionState selectionState;
        if (SUCCEEDED(m_thumbnailControls[i]->GetSelectionState(&selectionState)))
        {
            if ((selectionState & SelectionStateSelected) == SelectionStateSelected)
            {
                ThumbnailInfo info;
                if (SUCCEEDED(m_thumbnailControls[i]->GetThumbnailInfo(&info)))
                {
                    imageList.push_back(info.GetFileName());
                }
            }
        }
    }

    return imageList;
}

HRESULT MediaPaneMessageHandler::LaunchAnnotator()
{
    // Don't launch annotator if the current directory is empty
    if (m_thumbnailControls.empty())
    {
        return S_OK;
    }

    std::vector<std::wstring> images = GetSelectedImageList();
    std::wstring imageList;

    // Flatten the file names of all selected image
    for each (std::wstring image in images)
    {
        if (!imageList.empty())
        {
            imageList += ' ';
        }
        imageList += '"';
        imageList += image;
        imageList += '"';
    }

    // If no images are selected, then pass the directory of the first image
    if (imageList.empty())
    {
        ThumbnailInfo info;
        if (SUCCEEDED(m_thumbnailControls[0]->GetThumbnailInfo(&info)))
        {
            std::wstring fileName = info.GetFileName();
            imageList += '"';
            imageList += fileName.substr(0, fileName.find_last_of(L"\\"));
            imageList += '"';
        }
    }

    // Get the path of target exe
    wchar_t currentFileName[FILENAME_MAX];

    if ( !GetModuleFileName(nullptr, currentFileName, FILENAME_MAX) )
    {
        return S_OK;
    }

    // Annotator should be found in the same directory as this binary
    std::wstring currentDirectory = std::wstring(currentFileName);
    std::wstring externalFileName = currentDirectory.substr(0, currentDirectory.find_last_of(L"\\") + 1);
    externalFileName += L"annotator.exe";

    STARTUPINFO startInfo;
    PROCESS_INFORMATION processInfo;

    // Initialize startup and process info structures
    ZeroMemory(&startInfo, sizeof(startInfo));
    startInfo.cb = sizeof(startInfo);
    ZeroMemory(&processInfo, sizeof(processInfo));

    // Create command line parameter list
    std::wstring commandLaneParameters;
    commandLaneParameters += L"\"" + externalFileName + L"\" " + imageList;

    // Copy command line parameter list into buffer
    size_t length = commandLaneParameters.length() + 1;
    wchar_t * buffer = new wchar_t[length];
    swprintf_s(buffer, length, L"%s", commandLaneParameters.c_str());

    // Create annotator process
    ::CreateProcess(nullptr, buffer, nullptr, nullptr, false, 0, nullptr, nullptr, &startInfo, &processInfo);

    // Release memory
    delete [] buffer;
    ::CloseHandle(processInfo.hProcess);
    ::CloseHandle(processInfo.hThread);

    return S_OK;
}

HRESULT MediaPaneMessageHandler::ShareImages()
{
    // Don't open the share dialog for empty directories
    if (!m_thumbnailControls.empty())
    {
        ComPtr<IWindow> window;
        if (SUCCEEDED(GetWindow(&window)))
        {
            ShareDialog::Show(window, &m_thumbnailControls);
        }
    }

    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnPan(D2D1_POINT_2F panLocation, unsigned long flags)
{
    PanImage(panLocation, flags);
    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnZoom(float zoomFactor)
{
    Zoom(zoomFactor);
    return S_OK;
}
