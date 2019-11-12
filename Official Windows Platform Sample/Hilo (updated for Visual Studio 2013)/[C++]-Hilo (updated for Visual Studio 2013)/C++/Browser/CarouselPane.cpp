//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "resource.h"
#include "CarouselPane.h"
#include "CarouselAnimation.h"
#include "CarouselThumbnailAnimation.h"
#include "OrbitAnimation.h"
#include "ShellItemsLoader.h"
#include "MediaPane.h"
#include "WindowLayout.h"

using namespace Hilo::AnimationHelpers;
using namespace Hilo::AsyncLoader;
using namespace Hilo::Direct2DHelpers;

const float CarouselPaneMessageHandler::ThumbnailWidth = 80;
const float CarouselPaneMessageHandler::ThumbnailHeight = 80;
const float CarouselPaneMessageHandler::ApplicationButtonSize = 32;
const float CarouselPaneMessageHandler::ApplicationButtonMargin = 16;
const float CarouselPaneMessageHandler::ApplicationButtonSelectionMargin = 8;
const float CarouselPaneMessageHandler::HistoryThumbnailWidth = 80;
const float CarouselPaneMessageHandler::HistoryThumbnailHeight = 80;
const float CarouselPaneMessageHandler::CarouselPaneMarginSize = 10;
const float CarouselPaneMessageHandler::OrbitMarginXSize = 60;
const float CarouselPaneMessageHandler::OrbitMarginYSize = 40;
const float CarouselPaneMessageHandler::MaxThumbnailSize = 96;
const int CarouselPaneMessageHandler::MaxHistoryIncrement = 250;
const int CarouselPaneMessageHandler::CarouselSpeedFactor = 500;
const int CarouselPaneMessageHandler::BackgroundColor = 0xB8BEFC;

const double CarouselPaneMessageHandler::InnerOrbitOpacity = 0.6;
const double CarouselPaneMessageHandler::HistoryExpansionTime = 0.25;
const float CarouselPaneMessageHandler::MaxInnerOrbitHeight = 100;
const float CarouselPaneMessageHandler::ThumbnailTextHeight = 36;
const float CarouselPaneMessageHandler::BackButtonWidth = 32;
const float CarouselPaneMessageHandler::BackButtonHeight = 32;
const float CarouselPaneMessageHandler::HistoryThumbnailMarginTop = 20;
const float CarouselPaneMessageHandler::HistoryThumbnailMarginLeft = 20;
const float CarouselPaneMessageHandler::KeyRotateSize = 750;

using namespace Hilo::WindowApiHelpers;

//
// Constructor
//
CarouselPaneMessageHandler::CarouselPaneMessageHandler() :
    m_isHistoryExpanded(false),
    m_isAnnotatorButtonMouseHover(false),
    m_isSharingButtonMouseHover(false),
    m_carouselSpinValue(0),
    m_updatingFolder(false),
    m_MouseDirection(None),
    m_isRotationClockwise(false),
    m_mouseDownPoint(Point2F(0, 0))
{
#ifdef _MEASURE_FPS
    m_logFile.open("carousel-fps.log", std::ofstream::app);

    SYSTEMTIME currentTime;
    ::GetLocalTime(&currentTime);
    m_logFile << "Carousel FPS Logging started @ " << currentTime.wHour << ":" << currentTime.wMinute << ":" << currentTime.wSecond <<"\r\n" ;
#endif
}
//
// Destructor
//
CarouselPaneMessageHandler::~CarouselPaneMessageHandler()
{
}

//
// Initializes a new instance of this object.  Called via SharedObject<>::Create
//
HRESULT CarouselPaneMessageHandler::Initialize()
{
    HRESULT hr = SharedObject<AsyncLoaderHelper>::Create(&m_AsyncLoaderHelper);

    if (SUCCEEDED(hr))
    {
        hr = m_AsyncLoaderHelper->ConnectClient(this);
    }

    return hr;
}

//
// Add the specified orbit item to the history stack
//
HRESULT CarouselPaneMessageHandler::AddHistoryItem(unsigned int carouselItemIndex)
{
    HRESULT hr = S_OK;

    if (carouselItemIndex < m_carouselItems.size())
    {
        hr = AddHistoryItem(m_carouselItems[carouselItemIndex]);
    }
    else
    {
        hr = E_INVALIDARG;
    }

    return hr;
}

//
// Add the specified orbit item to the history stack and kickoff animation for adding an item to the history
//
HRESULT CarouselPaneMessageHandler::AddHistoryItem(IThumbnail* item)
{
    // Create new carousel history item
    CarouselHistoryItem newHistoryItem;

    // Initialize thumbnail
    ThumbnailInfo info(nullptr);
    HRESULT hr = item->GetThumbnailInfo(&info);

    if (SUCCEEDED(hr))
    {
        // Create a new history thumbnail
        hr = SharedObject<CarouselThumbnail>::Create(info, &newHistoryItem.Thumbnail);
    }

    if (SUCCEEDED(hr))
    {
        // Set the rendering parameters for this new thumbnail
        hr = newHistoryItem.Thumbnail->SetRenderingParameters(m_renderingParameters);
    }

    if (SUCCEEDED(hr))
    {
        // Set height and width of new history thumbnail
        newHistoryItem.Thumbnail->SetRect(
            D2D1::RectF(
            0.0f,
            0.0f,
            static_cast<float>(ThumbnailWidth),
            static_cast<float>(ThumbnailHeight)));
    }

    if (SUCCEEDED(hr))
    {
        // Disable asynchronous loading for history thumbnails
        ComPtr<IAsyncLoaderItem> asyncLoaderItem;
        if (SUCCEEDED(newHistoryItem.Thumbnail.QueryInterface(&asyncLoaderItem)))
        {
            asyncLoaderItem->EnableAsyncLoading(false);
        }
    }

    if (SUCCEEDED(hr))
    {
        D2D1_RECT_F currentRect;
        hr = item->GetRect(&currentRect);

        if (SUCCEEDED(hr))
        {
            // Create carousel thumbnail animation
            hr = SharedObject<CarouselThumbnailAnimation>::Create(
                D2D1::Point2F(currentRect.left, currentRect.top),
                1,
                &newHistoryItem.ThumbnailAnimation);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Create new instance of the orbit animation object
        hr = SharedObject<OrbitAnimation>::Create(CalculateInnerOrbit(), &newHistoryItem.OrbitAnimation);
    }

    if (SUCCEEDED(hr))
    {
        // Add history item
        m_carouselHistoryItems.push_back(newHistoryItem);

        // Load new orbit items and kick off animation
        if (m_carouselHistoryItems.size() > 1)
        {
            hr = SetCurrentLocation(info.shellItem, false);
        }
    }

    return hr;
}

//
// Invalidate the current window
//
void CarouselPaneMessageHandler::InvalidateWindow()
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

//
// Sets the specified location as the current location and kicks off the animation for setting a new folder
//
HRESULT CarouselPaneMessageHandler::SetCurrentLocation(IShellItem* shellFolder, bool /*recursive*/)
{
    // Clear inner orbit
    RemoveAllItems();

    // Retrieve list of subfolders
    std::vector<ComPtr<IShellItem> > shellItems;

    HRESULT hr = ShellItemsLoader::EnumerateFolderItems(shellFolder, FileTypeFolder, false, shellItems);
    if (SUCCEEDED(hr))
    {
        // Add each subfolder ShellItem to the inner carousel orbit
        for ( auto ti = shellItems.begin() ; ti != shellItems.end(); ti++ )
        {
            ThumbnailInfo info(*ti);
            info.fileType = FileTypeFolder;

            ComPtr<IThumbnail> thumbnail;
            hr = SharedObject<CarouselThumbnail>::Create(info, &thumbnail);

            if (FAILED(hr))
            {
                // We can ignore the creation error for one of the items.
                hr = S_OK;
                continue;
            }

            // Set default bitmap and parameters
            thumbnail->SetDefaultBitmap(m_defaultFolderBitmap);
            thumbnail->SetRenderingParameters(m_renderingParameters);
            thumbnail->SetParentWindow(this);

            // Add thumbnail to inner orbit
            m_carouselItems.push_back(thumbnail);

            // Initialize asynchronous loader
            ComPtr<IUnknown> itemIUnknown;
            hr = thumbnail.QueryInterface(&itemIUnknown);
            if (SUCCEEDED(hr))
            {
                hr = m_AsyncLoaderHelper->ConnectItem(itemIUnknown, static_cast<unsigned int>(m_carouselItems.size()) - 1);
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        // Update async loader with number of items
        hr = m_AsyncLoaderHelper->SetPageItemCount(static_cast<unsigned int>(m_carouselItems.size()));
    }

    if (SUCCEEDED(hr))
    {
        if (m_carouselHistoryItems.empty())
        {
            // If this is the initial location, add to history stack
            ComPtr<IThumbnail> thumbnail;

            hr = SharedObject<CarouselThumbnail>::Create(ThumbnailInfo(shellFolder), &thumbnail);
            if (SUCCEEDED(hr))
            {
                D2D1_RECT_F initialRect;
                initialRect.left = m_renderTarget->GetSize().width / 2 - ThumbnailWidth / 2;
                initialRect.top = m_renderTarget->GetSize().height / 2 - ThumbnailHeight / 2;
                initialRect.right = initialRect.left + ThumbnailWidth;
                initialRect.bottom = initialRect.top + ThumbnailHeight;

                hr = thumbnail->SetRect(initialRect);
            }

            if (SUCCEEDED(hr))
            {
                hr = AddHistoryItem(thumbnail);
            }
        }
    }

    // Initialize animation of new folder set. This is needed before setting the current location
    // on the media pane to ensure that the media pane's height takes into account the correct
    // height of the carousel pane
    if (SUCCEEDED(hr))
    {
        m_updatingFolder = true;
        hr = AnimateNewFolderSet();
    }

    if (SUCCEEDED(hr))
    {
        // Kick off the animation for adding a history item to the history stack
        hr = AnimateHistoryAddition(false);
    }

    if (m_mediaPane)
    {
        // Update the current location for the media pane
        hr = m_mediaPane->SetCurrentLocation(shellFolder, false);
    }

    return hr;
}

//
// Called during creation of the window
//
HRESULT CarouselPaneMessageHandler::OnCreate()
{
    HRESULT hr = CreateDeviceIndependentResources();
    if (SUCCEEDED(hr))
    {
        hr = CreateDeviceResources();
    }

    return hr;
}

//
// Redraw the current window
//
HRESULT CarouselPaneMessageHandler::OnRender()
{
    // Draw the client area
    DrawClientArea();

    // Update the animation manager with the current time
    AnimationUtility::UpdateAnimationManagerTime();

    // Continue redrawing as long as there are animations scheduled
    bool isBusy;

    if (SUCCEEDED(AnimationUtility::IsAnimationManagerBusy(&isBusy)))
    {
        if (isBusy)
        {
#ifdef _MEASURE_FPS
            m_totalFramesRendered++;
#endif
            InvalidateWindow();
        }
        else
        {
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

                m_logFile << "Frames per second for last carousel animation = ( " << m_totalFramesRendered << " frames /" << totalAnimationTime << " millis) = " << fps << " fps\r\n";
                m_totalFramesRendered = 0;
            }
#endif
            m_updatingFolder = false;
        }
    }

    return S_OK;
}

//
// Draw the client area
//
HRESULT CarouselPaneMessageHandler::DrawClientArea()
{
    HRESULT hr = CreateDeviceResources();
    if (SUCCEEDED(hr) && !(m_renderTarget->CheckWindowState() & D2D1_WINDOW_STATE_OCCLUDED))
    {
        // Size of the current render target
        D2D1_SIZE_F size = m_renderTarget->GetSize();

        // Begin drawing
        m_renderTarget->BeginDraw();
        m_renderTarget->SetTransform(Matrix3x2F::Identity());
        m_renderTarget->FillRectangle(RectF(0, 0, size.width, size.height), m_backgroundLinearGradientBrush);

        // Draw back button
        if (m_carouselHistoryItems.size() > 1 && m_isHistoryExpanded == false)
        {
            m_renderTarget->DrawBitmap(m_arrowBitmap, m_backButtonRect);
        }

        // Draw the carousel pane
        DrawHistoryItems();
        DrawOrbitItems();

        // Draw selection box for share button
        if (m_isSharingButtonMouseHover)
        {
            m_selectionBrush->SetOpacity(1.0f);
            m_renderTarget->DrawRoundedRectangle(m_shareButtonSelectionRect, m_selectionBrush);

            m_selectionBrush->SetOpacity(0.25f);
            m_renderTarget->FillRoundedRectangle(m_shareButtonSelectionRect, m_selectionBrush);

            m_renderTarget->DrawTextLayout(
                D2D1::Point2(m_shareButtonSelectionRect.rect.left, m_shareButtonSelectionRect.rect.bottom),
                m_textLayoutShare,
                m_fontBrush);
        }

        // Draw selection box for annotate button
        if (m_isAnnotatorButtonMouseHover)
        {
            m_selectionBrush->SetOpacity(1.0f);
            m_renderTarget->DrawRoundedRectangle(m_annotateButtonSelectionRect, m_selectionBrush);

            m_selectionBrush->SetOpacity(0.25f);
            m_renderTarget->FillRoundedRectangle(m_annotateButtonSelectionRect, m_selectionBrush);

            m_renderTarget->DrawTextLayout(
                D2D1::Point2(m_annotateButtonSelectionRect.rect.left, m_annotateButtonSelectionRect.rect.bottom),
                m_textLayoutAnnotate,
                m_fontBrush);
        }

        // Draw share/annotate button images
        m_renderTarget->DrawBitmap(m_sharingButtonImage, m_shareButtonImageRect);
        m_renderTarget->DrawBitmap(m_annotatorButtonImage, m_annotateButtonImageRect);

        // End Direct2D rendering
        hr = m_renderTarget->EndDraw();

        if (hr == D2DERR_RECREATE_TARGET)
        {
            hr = S_OK;
            DiscardDeviceResources();
        }
    }

    return hr;
}

//
// Draw all history items and their orbits
//
void CarouselPaneMessageHandler::DrawHistoryItems()
{
    HRESULT hr = S_OK;

    // Draw all history orbits
    for (auto iter = m_carouselHistoryItems.begin(); iter != m_carouselHistoryItems.end(); iter++)
    {
        // Render orbit
        D2D1_ELLIPSE orbit;
        double opacity;

        hr = iter->OrbitAnimation->GetEllipse(&orbit, &opacity);

        if (SUCCEEDED(hr))
        {
            m_radialGradientBrush->SetCenter(orbit.point);
            m_radialGradientBrush->SetRadiusX(orbit.radiusX);
            m_radialGradientBrush->SetRadiusY(orbit.radiusY);
            m_radialGradientBrush->SetOpacity(static_cast<float>(opacity));

            m_renderTarget->FillEllipse(orbit, m_radialGradientBrush);
        }
    }

    // Draw all history thumbnails
    for (auto iter = m_carouselHistoryItems.begin(); iter != m_carouselHistoryItems.end(); iter++)
    {
        // Render thumbnail
        D2D1_POINT_2F point;
        double opacity;

        hr = iter->ThumbnailAnimation->GetInfo(&point, &opacity);

        if (SUCCEEDED(hr))
        {
            iter->Thumbnail->SetRect(
                D2D1::RectF(
                point.x,
                point.y,
                point.x + HistoryThumbnailWidth,
                point.y + HistoryThumbnailHeight));

            iter->Thumbnail->SetFontSize(14);
            iter->Thumbnail->SetOpacity(static_cast<float>(opacity));
            iter->Thumbnail->Draw();
        }
    }
}

//
// Draw all items on the inner orbit
//
void CarouselPaneMessageHandler::DrawOrbitItems()
{
    // Read animation variables
    double rotationLocation = 0;
    double orbitIconsOpacity = 1;
    double orbitIconsScale = 1;

    m_carouselAnimation->GetInfo(&rotationLocation, &orbitIconsScale, &orbitIconsOpacity);

    D2D1_ELLIPSE innerOrbit;
    double opacity;

    m_innerOrbitAnimation->GetEllipse(&innerOrbit, &opacity);

    // Draw inside orbit
    if (m_carouselItems.size() > 0)
    {
        m_radialGradientBrush->SetCenter(innerOrbit.point);
        m_radialGradientBrush->SetRadiusX(innerOrbit.radiusX);
        m_radialGradientBrush->SetRadiusY(innerOrbit.radiusY);
        m_radialGradientBrush->SetOpacity(static_cast<float>(opacity));

        m_renderTarget->FillEllipse(innerOrbit, m_radialGradientBrush);
    }

    // Placement of icons on the orbit are equally spaced (for now)
    double increment = (2 * PI) / m_carouselItems.size();

    // Draw icons on orbit in two passes. The first pass draws the icons
    // in the back. The second pass draws icons in the front.
    for (int pass = 0; pass < 2; pass++)
    {
        // Keep the orignal value of rotation
        double currentRotation = rotationLocation;

        ComPtr<IWindow> window;
        if (SUCCEEDED(GetWindow(&window)))
        {
            bool isMouseCaptured = false;
            if (SUCCEEDED(window->IsMouseCaptured(&isMouseCaptured)))
            {
                if (isMouseCaptured && m_isHistoryExpanded == false)
                {
                    if (m_carouselSpinValue != 0)
                    {
                        // During mouse capture, update the rotation of the carousel
                        UpdateCarouselLocation(rotationLocation + 0.006 * m_carouselSpinValue);
                        m_carouselSpinValue = 0;
                    }
                }
            }
        }

        for (auto iter = m_carouselItems.begin(); iter != m_carouselItems.end(); iter++)
        {
            // Determine when to draw based on relative value of y
            if ((sin(currentRotation) > 0 && pass == 0) || (sin(currentRotation) <= 0 && pass == 1))
            {
                // Get point to draw current orbit
                D2D1_POINT_2F point = CalculatePointAtAngle(&innerOrbit, currentRotation);

                // Calculate scaling based on relative value of y
                float scale = 1.25f - 0.5f * ((1.0f + static_cast<float>(sin(currentRotation))) * 0.5f);
                scale *= static_cast<float>(orbitIconsScale);

                D2D1_RECT_F rect_f;
                rect_f.left = point.x - (ThumbnailWidth * scale * 0.5f);
                rect_f.right = point.x + (ThumbnailWidth * scale * 0.5f);
                rect_f.top = point.y - (ThumbnailHeight * scale) + (ThumbnailHeight - ThumbnailWidth);
                rect_f.bottom = rect_f.top + (ThumbnailHeight * scale);

                // Draw the carousel thumbnail
                (*iter)->SetFontSize(12 * scale);
                (*iter)->SetOpacity(static_cast<float>(orbitIconsOpacity));
                (*iter)->SetRect(rect_f);
                (*iter)->Draw();
            }

            // Increment location to next spot
            currentRotation += increment;
        }
    }
}

//
// Calculates the current ellipse for the inner orbit
//
D2D1_ELLIPSE CarouselPaneMessageHandler::CalculateInnerOrbit()
{
    D2D1_POINT_2F center;
    float radiusX;
    float radiusY;

    float maxRadiusX = (m_renderTarget->GetSize().width / 2) - OrbitMarginXSize * 2;

    radiusX = std::min(maxRadiusX, std::max(1.0f, static_cast<float>(m_carouselItems.size() - 1)) * ThumbnailWidth * 0.60f);
    radiusY = std::min(MaxInnerOrbitHeight, std::max(1.0f, static_cast<float>(m_carouselItems.size()- 1)) * ThumbnailWidth * 0.20f);

    if (m_isHistoryExpanded)
    {
        center.x = m_renderTarget->GetSize().width * (0.875f) - CarouselPaneMarginSize;
        center.y = m_renderTarget->GetSize().height * (0.875f) - CarouselPaneMarginSize;

        radiusX = radiusX / 3;
        radiusY = radiusY / 3;
    }
    else
    {
        center.x = m_renderTarget->GetSize().width / 2;
        center.y = 10 + ThumbnailHeight + radiusY;
    }

    return D2D1::Ellipse(center, radiusX, radiusY);
}

//
// Calculates the history orbit for the specified history item
//
D2D1_ELLIPSE CarouselPaneMessageHandler::CalculateHistoryOrbit(unsigned int index)
{
    D2D1_POINT_2F center;
    float radiusX;
    float radiusY;

    if (m_isHistoryExpanded)
    {
        // Center is bottom right corner of render target
        center.x = m_renderTarget->GetSize().width;
        center.y = m_renderTarget->GetSize().height;

        D2D_POINT_2F point = CalculateHistoryThumbnailPoint(index);

        double a = center.x - point.x;
        double b = center.y - point.y;
        double c = sqrt( a*a + b*b );

        double angle = PI - a / c;

        radiusX = static_cast<float>(abs(a / cos(angle)));
        radiusY = static_cast<float>(abs(b / sin(angle)));

        center.x += ThumbnailWidth * 0.5f;
        center.y += ThumbnailHeight * 0.5f;
    }
    else
    {
        radiusX = m_renderTarget->GetSize().width / 1.0f;
        radiusY = m_renderTarget->GetSize().height / 1.5f;

        center.x = m_renderTarget->GetSize().width / 2;
        center.y = 30 + radiusY - 10 * (m_carouselHistoryItems.size() - index - 1);
    }

    return D2D1::Ellipse(center, radiusX, radiusY);
}

//
// Calculates the current orbit opacity for the specified history item
//
double CarouselPaneMessageHandler::CalculateHistoryOrbitOpacity(unsigned int index)
{
    return (m_isHistoryExpanded) ? 0.6 : std::min(1.0f, std::max(0.4f - 0.1f * (m_carouselHistoryItems.size() - index), 0.0f));
}

//
// Calculates the current icon opacity for the specified history item
//
double CarouselPaneMessageHandler::CalculateHistoryThumbnailOpacity(unsigned int index)
{
    return (m_isHistoryExpanded) ? 1 : std::min(1.0f, std::max(1.0f - 0.33f * (m_carouselHistoryItems.size() - index - 1) , 0.0f));
}

//
// Calculates the icon position for the specified history item
//
D2D1_POINT_2F CarouselPaneMessageHandler::CalculateHistoryThumbnailPoint(unsigned int index)
{
    D2D1_POINT_2F point;

    if (m_isHistoryExpanded)
    {
        // Defines the line that the icons occupy
        D2D_POINT_2F point1;
        D2D_POINT_2F point2;

        // Calculate top left point
        point1.x = ThumbnailWidth * 0.5f + CarouselPaneMarginSize;
        point1.y = ThumbnailHeight * 0.5f + CarouselPaneMarginSize;

        point2.x = m_renderTarget->GetSize().width * 0.75f - CarouselPaneMarginSize - ThumbnailWidth * 0.5f;
        point2.y = m_renderTarget->GetSize().height * 0.875f - CarouselPaneMarginSize - ThumbnailHeight * 0.5f;

        // Calculate increment
        float incrementX = (point2.x - point1.x) / (m_carouselHistoryItems.size() - 1.0f);
        float incrementY = (point2.y - point1.y) / (m_carouselHistoryItems.size() - 1.0f);

        if (incrementX > MaxHistoryIncrement || incrementY > MaxHistoryIncrement)
        {
            float scale = 0.0;

            if (incrementX > MaxHistoryIncrement)
            {
                scale = MaxHistoryIncrement / incrementX;
            }
            else
            {
                scale = MaxHistoryIncrement / incrementY;
            }

            incrementX *= scale;
            incrementY *= scale;
        }

        point.x = point1.x + incrementX * index - static_cast<float>(ThumbnailWidth) * 0.5f;
        point.y = point1.y + incrementY * index - static_cast<float>(ThumbnailHeight) * 0.5f;
    }
    else
    {
        point.x = HistoryThumbnailMarginLeft - 7 * static_cast<float>(m_carouselHistoryItems.size() - index - 1);
        point.y = HistoryThumbnailMarginTop - 7 * static_cast<float>(m_carouselHistoryItems.size() - index - 1);
    }

    return point;
}

//
// Determines where to place an icon on the specified ellipse at the given angle(in radians)
//
D2D1_POINT_2F CarouselPaneMessageHandler::CalculatePointAtAngle(D2D1_ELLIPSE* ellipse, double angle)
{
    return D2D1::Point2F(
        static_cast<float>(ellipse->point.x + ellipse->radiusX * cos(angle)),
        static_cast<float>(ellipse->point.y - ellipse->radiusY * sin(angle)));
}

//
// Calculate the location of the application buttons
//
void CarouselPaneMessageHandler::CalculateApplicationButtonRects()
{
    // Calculate the location of the annotator application button
    m_annotateButtonImageRect.left = m_renderTarget->GetSize().width - ApplicationButtonMargin - ApplicationButtonSize;
    m_annotateButtonImageRect.top = ApplicationButtonMargin;
    m_annotateButtonImageRect.right = m_annotateButtonImageRect.left + ApplicationButtonSize;
    m_annotateButtonImageRect.bottom = m_annotateButtonImageRect.top + ApplicationButtonSize;

    // Calculate the selection rectangle for the annotate button
    m_annotateButtonSelectionRect = D2D1::RoundedRect(m_annotateButtonImageRect, 1, 1);
    m_annotateButtonSelectionRect.rect.left -= ApplicationButtonSelectionMargin;
    m_annotateButtonSelectionRect.rect.right += ApplicationButtonSelectionMargin;
    m_annotateButtonSelectionRect.rect.top -= ApplicationButtonSelectionMargin;
    m_annotateButtonSelectionRect.rect.bottom += ApplicationButtonSelectionMargin;

    // Calculate the location of the sharing application button
    m_shareButtonImageRect.left = m_annotateButtonImageRect.left - ApplicationButtonSize / 2 - ApplicationButtonSize;
    m_shareButtonImageRect.top = ApplicationButtonMargin;
    m_shareButtonImageRect.right = m_shareButtonImageRect.left + ApplicationButtonSize;
    m_shareButtonImageRect.bottom = m_shareButtonImageRect.top + ApplicationButtonSize;

    // Calculate the selection rectangle for the annotate button
    m_shareButtonSelectionRect = D2D1::RoundedRect(m_shareButtonImageRect, 1, 1);
    m_shareButtonSelectionRect.rect.left -= ApplicationButtonSelectionMargin;
    m_shareButtonSelectionRect.rect.right += ApplicationButtonSelectionMargin;
    m_shareButtonSelectionRect.rect.top -= ApplicationButtonSelectionMargin;
    m_shareButtonSelectionRect.rect.bottom += ApplicationButtonSelectionMargin;
}

//
// If the application receives a WM_SIZE message
//
HRESULT CarouselPaneMessageHandler::OnSize(unsigned int width, unsigned int height)
{
    HRESULT hr = S_OK;

    if (width == 0 || height == 0)
    {
        // Ignore the resize message if either width or height equal 0
        return hr;
    }

    if (m_renderTarget)
    {
        D2D1_SIZE_U size = {width,height};
        hr = m_renderTarget->Resize(size);

        if (SUCCEEDED(hr))
        {
            CalculateApplicationButtonRects();

            if (m_updatingFolder == false)
            {
                hr = ResetOrbitValues();
                OnRender();
            }
            else
            {
                hr = AnimateHistoryAddition(true);
            }
        }
    }

    return hr;
}

//
// OnEraseBackground
//
HRESULT CarouselPaneMessageHandler::OnEraseBackground()
{
    // To prevent flickering, returning Success would indicate
    // this message has been handled
    return S_OK;
}

//
// Called whenever the left mouse button is pressed
//
HRESULT CarouselPaneMessageHandler::OnLeftMouseButtonDown(D2D1_POINT_2F mousePosition)
{
    // Clear queue of mouse points
    m_mouseMovePoints.clear();

    // Reset carousel spin value
    m_carouselSpinValue = 0;

    // Store mouse down point for later use
    m_mouseDownPoint = mousePosition;

    // Queue that tracks mouse points for inertia
    MouseMoveInfo info;
    info.point = m_mouseDownPoint;

    // Get time from animation utiity
    HRESULT hr = AnimationUtility::GetAnimationTimerTime(&info.time);

    if (SUCCEEDED(hr))
    {
        m_mouseMovePoints.push_back(info);
    }

    // Capture mouse focus
    ComPtr<IWindow> window;
    if (SUCCEEDED(GetWindow(&window)))
    {
        window->SetCapture();
    }

    return hr;
}

//
// Called whenever the left mouse button is released
//
HRESULT CarouselPaneMessageHandler::OnLeftMouseButtonUp(D2D1_POINT_2F mousePosition)
{
    HRESULT hr = S_OK;
    bool clickProcessed = false;

    // Release mouse capture
    ::ReleaseCapture();

    // Clear mouse direction
    m_MouseDirection = None;

    // Deterine which item was clicked
    if (!m_isHistoryExpanded)
    {
        ClearMouseHover();

        if (m_mouseMovePoints.size() < 3)
        {
            for (auto iter = m_carouselItems.begin(); iter != m_carouselItems.end(); iter++)
            {
                D2D1_RECT_F rect;
                hr = ((*iter)->GetRect(&rect));

                if (SUCCEEDED(hr))
                {
                    if (Direct2DUtility::HitTest(rect, mousePosition))
                    {
                        // Check if this item was the target of MouseDown
                        if (Direct2DUtility::HitTest(rect, m_mouseDownPoint))
                        {
                            // Add item to history
                            ClearMouseHover();
                            AddHistoryItem(*iter);
                            clickProcessed = true;
                            break;
                        }
                    }
                }
            }
        }

        if (clickProcessed == false && m_mouseMovePoints.size() > 0)
        {
            UI_ANIMATION_SECONDS endTime;
            hr = AnimationUtility::GetAnimationTimerTime(&endTime);

            if (SUCCEEDED(hr))
            {
                double distance = fabs(mousePosition.x - m_mouseMovePoints.front().point.x);
                double time = endTime - m_mouseMovePoints.front().time;
                double speed = distance / time;

                if (!m_isRotationClockwise)
                {
                    speed *= -1;
                }

                RotateCarousel(std::min<double>(2000, std::max<double>(-2000, speed)));
            }
        }
    }

    if (!clickProcessed && m_carouselHistoryItems.size() > 1)
    {
        unsigned int index = 0;

        for (auto iter = m_carouselHistoryItems.begin(); iter != m_carouselHistoryItems.end(); iter++)
        {
            D2D1_RECT_F rect;
            hr = (*iter).Thumbnail->GetRect(&rect);

            if (SUCCEEDED(hr))
            {
                if (Direct2DUtility::HitTest(rect, mousePosition))
                {
                    if (m_isHistoryExpanded)
                    {
                        NavigateToHistoryItem(index);
                    }

                    ClearMouseHover();
                    AnimateHistoryExpansion();
                    clickProcessed = true;

                    break;
                }
            }

            index++;
        }
    }

    if (!clickProcessed)
    {
        if (m_isHistoryExpanded)
        {
            // Check if user clicked on inner orbit
            D2D1_ELLIPSE orbit;
            hr = m_innerOrbitAnimation->GetEllipse(&orbit, nullptr);

            if (SUCCEEDED(hr))
            {
                // Check if user clicked in inner orbit
                if (mousePosition.x >= orbit.point.x - orbit.radiusX &&
                    mousePosition.x <= orbit.point.x + orbit.radiusX &&
                    mousePosition.y >= orbit.point.y - orbit.radiusY &&
                    mousePosition.y <= orbit.point.y + orbit.radiusY)
                {
                    hr = AnimateHistoryExpansion();
                }
            }
        }
        else
        {
            // Check if the user clicked the back button
            if (Direct2DUtility::HitTest(m_backButtonRect, mousePosition))
            {
                hr = NavigateBack();
            }

            // Check if the user clicked the share or annotate application button
            if (m_isAnnotatorButtonMouseHover || m_isSharingButtonMouseHover)
            {
                ComPtr<IMediaPane> mediaPane;
                hr = m_mediaPane->QueryInterface(&mediaPane);

                if (SUCCEEDED(hr))
                {
                    if (m_isAnnotatorButtonMouseHover)
                    {
                        mediaPane->LaunchAnnotator();
                    }
                    else
                    {
                        mediaPane->ShareImages();
                    }
                }
            }
        }
    }

    return hr;
}

//
// Called whenever the mouse is moved
//
HRESULT CarouselPaneMessageHandler::OnMouseMove(D2D1_POINT_2F mousePosition)
{
    ComPtr<IWindow> window;
    HRESULT hr = GetWindow(&window);

    if (SUCCEEDED(hr))
    {
        bool isMouseCaptured;
        hr = window->IsMouseCaptured(&isMouseCaptured);

        if (SUCCEEDED(hr))
        {
            // Check if mouse is captured
            if (isMouseCaptured)
            {
                // Don't do anything if no change has occured
                if (m_mouseMovePoints.back().point.x == mousePosition.x && 
                    m_mouseMovePoints.back().point.y == mousePosition.y)
                {
                    return hr;
                }

                // Clear mouse hover
                ClearMouseHover();

                // Allows the carousel to spin during next render
                m_carouselSpinValue = mousePosition.x - m_mouseMovePoints.back().point.x;

                // Limit size of deque of mouse move information
                if (m_mouseMovePoints.size() > 3)
                {
                    m_mouseMovePoints.pop_front();
                }

                // Check for change in direction and update direction of rotation
                switch(m_MouseDirection)
                {
                case Left:
                    {
                        // Indicates mouse is no longer moving left
                        if (m_carouselSpinValue > 0)
                        {
                            m_MouseDirection = Right;
                            m_isRotationClockwise = mousePosition.y < CalculateInnerOrbit().point.y;
                        }

                        break;
                    }

                case Right:
                    {
                        // Indicates mouse is no longer moving right
                        if (m_carouselSpinValue < 0)
                        {
                            m_MouseDirection = Left;
                            m_isRotationClockwise = mousePosition.y > CalculateInnerOrbit().point.y;
                        }

                        break;
                    }

                default:
                    {
                        if (m_carouselSpinValue > 0)
                        {
                            m_MouseDirection = Right;
                            m_isRotationClockwise = mousePosition.y < CalculateInnerOrbit().point.y;
                        }
                        else
                        {
                            m_MouseDirection = Left;
                            m_isRotationClockwise = mousePosition.y > CalculateInnerOrbit().point.y;
                        }
                    }
                }


                if (m_isRotationClockwise)
                {
                    m_carouselSpinValue = fabs(m_carouselSpinValue) * -1;
                }
                else
                {
                    m_carouselSpinValue = fabs(m_carouselSpinValue);
                }

                // Store time and mouse information for intertia calculation
                MouseMoveInfo info;
                info.point = mousePosition;
                hr = AnimationUtility::GetAnimationTimerTime(&info.time);

                if (SUCCEEDED(hr))
                {
                    m_mouseMovePoints.push_back(info);
                }

                InvalidateWindow();
            }
            else
            {
                CheckForMouseHover(mousePosition);
            }
        }
    }

    return hr;
}

//
// Called when the mouse enters the client area
//
HRESULT CarouselPaneMessageHandler::OnMouseEnter(D2D1_POINT_2F /*mousePosition*/)
{
    ::SetCursor(::LoadCursor(nullptr, IDC_ARROW));
    return S_OK;
}

//
// Called when the mouse wheel is moved
//
HRESULT CarouselPaneMessageHandler::OnMouseWheel(D2D1_POINT_2F /*mousePosition*/, short delta, int /*keys*/) 
{
    return RotateCarousel(delta * 5);
}

//
// Called when a key is pressed
//
HRESULT CarouselPaneMessageHandler::OnKeyDown(unsigned int vKey)
{
    HRESULT hr = S_OK;

    if (vKey == VK_LEFT)
    {
        hr = RotateCarousel(KeyRotateSize);
    }
    else if (vKey == VK_RIGHT)
    {
        hr = RotateCarousel(-KeyRotateSize);
    }
    else if (vKey == VK_BACK)
    {
        if (m_carouselHistoryItems.size() > 1)
        {
            // Navigate to penultimate item in the history
            NavigateBack();
        }
    }

    return hr;
}

//
//  This method creates resources which are bound to a particular
//  Direct2D render target. It's all centralized here, in case the resources
//  need to be recreated in case of Direct3D device loss (eg. display
//  change, remoting, removal of video card, etc).
//
HRESULT CarouselPaneMessageHandler::CreateDeviceResources()
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

        if (SUCCEEDED(hr))
        {
            // Load back button image
            hr = Direct2DUtility::LoadBitmapFromResource(
                m_renderTarget,
                L"BackArrowImage",
                L"PNG",
                0,
                0,
                &m_arrowBitmap);
        }

        if (SUCCEEDED(hr))
        {
            // Load default thumbnail image...
            hr = Direct2DUtility::LoadBitmapFromResource(
                m_renderTarget,
                L"DefaultFolderImage",
                L"PNG",
                0,
                0,
                &m_defaultFolderBitmap);
        }

        if (SUCCEEDED(hr))
        {
            // Load annotator button image
            hr = Direct2DUtility::LoadBitmapFromResource(
                m_renderTarget,
                L"AnnotatorButtonImage",
                L"PNG",
                0,
                0,
                &m_annotatorButtonImage);
        }

        if (SUCCEEDED(hr))
        {
            // Load share button image
            hr = Direct2DUtility::LoadBitmapFromResource(
                m_renderTarget,
                L"SharingButtonImage",
                L"PNG",
                0,
                0,
                &m_sharingButtonImage);
        }

        if (SUCCEEDED(hr))
        {
            // Create gradient brush for background
            ComPtr<ID2D1GradientStopCollection> gradientStopCollection;
            D2D1_GRADIENT_STOP gradientStops[3];

            if (SUCCEEDED(hr))
            {
                gradientStops[0].color = ColorF(BackgroundColor);
                gradientStops[0].position = 0.0f;
                gradientStops[1].color = ColorF(ColorF::White);
                gradientStops[1].position = 0.25f;
            }

            if (SUCCEEDED(hr))
            {

                hr = m_renderTarget->CreateGradientStopCollection(
                    gradientStops,
                    2,
                    D2D1_GAMMA_2_2,
                    D2D1_EXTEND_MODE_CLAMP,
                    &gradientStopCollection);
            };

            if (SUCCEEDED(hr))
            {
                hr = m_renderTarget->CreateLinearGradientBrush(
                    LinearGradientBrushProperties(
                    Point2F(m_renderTarget->GetSize().width, 0),
                    Point2F(m_renderTarget->GetSize().width, m_renderTarget->GetSize().height)),
                    gradientStopCollection,
                    &m_backgroundLinearGradientBrush);
            }

            gradientStopCollection = nullptr;

            // Create gradient brush for drawing orbital rings
            gradientStops[0].color = D2D1::ColorF(D2D1::ColorF::White, 1.0f);
            gradientStops[0].position = 0.7f;
            gradientStops[1].color = D2D1::ColorF(D2D1::ColorF::LightBlue, 1.0f);
            gradientStops[1].position = 0.95f;
            gradientStops[2].color = D2D1::ColorF(D2D1::ColorF::White, 1.0f);
            gradientStops[2].position = 1.0f;

            hr = m_renderTarget->CreateGradientStopCollection(
                gradientStops,
                3,
                D2D1_GAMMA_2_2,
                D2D1_EXTEND_MODE_CLAMP,
                &gradientStopCollection);

            if (SUCCEEDED(hr))
            {
                hr = m_renderTarget->CreateRadialGradientBrush(
                    D2D1::RadialGradientBrushProperties(D2D1::Point2F(0, 0), D2D1::Point2F(0, 0), 200, 200),
                    gradientStopCollection,
                    &m_radialGradientBrush);
            }

            if (SUCCEEDED(hr))
            {
                hr = m_renderTarget->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Black), &m_fontBrush);
            }

            if (SUCCEEDED(hr))
            {
                hr = m_renderTarget->CreateSolidColorBrush(ImageThumbnail::SelectionBorderColor, &m_selectionBrush);
            }

            if (SUCCEEDED(hr))
            {
                // Set rendering parameters
                m_renderingParameters.renderTarget = m_renderTarget;
                m_renderingParameters.solidBrush = m_fontBrush;
                m_renderingParameters.textFormat = m_textFormat;
            }

            if (SUCCEEDED(hr))
            {
                hr = m_AsyncLoaderHelper->StartBackgroundLoading();
            }
        }
    }

    return hr;
}

//
// This method discards resources which are bound to a particular Direct2D render target
//
HRESULT CarouselPaneMessageHandler::DiscardDeviceResources()
{
    HRESULT hr = S_OK;

    for (auto iter = m_carouselItems.begin(); iter != m_carouselItems.end(); iter++)
    {
        ComPtr<IThumbnail> thumbnail;

        if (SUCCEEDED(hr))
        {
            hr = (*iter).QueryInterface(&thumbnail);
        }

        thumbnail->DiscardResources();
    }

    for (auto iter = m_carouselHistoryItems.begin(); iter != m_carouselHistoryItems.end(); iter++)
    {
        ComPtr<IThumbnail> thumbnail;

        if (SUCCEEDED(hr))
        {
            hr = (iter)->Thumbnail.QueryInterface(&thumbnail);
        }

        if (SUCCEEDED(hr))
        {
            thumbnail->DiscardResources();
        }
    }

    m_arrowBitmap = nullptr;
    m_fontBrush = nullptr;
    m_backgroundLinearGradientBrush = nullptr;
    m_radialGradientBrush = nullptr;
    m_renderTarget = nullptr;

    return S_OK;
}

//
// Create resources that are not bound to a particular Direct2D render target
//
HRESULT CarouselPaneMessageHandler::CreateDeviceIndependentResources()
{
    HRESULT hr = Direct2DUtility::GetD2DFactory(&m_d2dFactory);

    if (SUCCEEDED(hr))
    {
        hr = Direct2DUtility::GetDWriteFactory(&m_dWriteFactory);
    }

    // Create text format for orbit and history icons
    if (SUCCEEDED(hr))
    {
        hr = m_dWriteFactory->CreateTextFormat(
            L"Arial",
            nullptr,
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            12,
            L"en-us",
            &m_textFormat);
    }

    // Create text layout for share button
    if (SUCCEEDED(hr))
    {
        wchar_t caption[20];
        ::LoadString(HINST_THISCOMPONENT, IDS_SHARE_BUTTON_TEXT, caption, 20);

        hr = m_dWriteFactory->CreateTextLayout(
            caption,
            static_cast<unsigned int>(wcslen(caption)),
            m_textFormat,
            ApplicationButtonSize + ApplicationButtonSelectionMargin * 2,
            ApplicationButtonSelectionMargin * 3,
            &m_textLayoutShare);
    }

    if (SUCCEEDED(hr))
    {
        // Setup text layout properties
        m_textLayoutShare->SetWordWrapping(DWRITE_WORD_WRAPPING_NO_WRAP);
        m_textLayoutShare->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER);
        m_textLayoutShare->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);
    }

    // Create text layout for annotate button
    if (SUCCEEDED(hr))
    {
        wchar_t caption[20];
        ::LoadString(HINST_THISCOMPONENT, IDS_ANNOTATE_BUTTON_TEXT, caption, 20);

        hr = m_dWriteFactory->CreateTextLayout(
            caption,
            static_cast<unsigned int>(wcslen(caption)),
            m_textFormat,
            ApplicationButtonSize + ApplicationButtonSelectionMargin * 2,
            ApplicationButtonSelectionMargin * 3,
            &m_textLayoutAnnotate);
    }

    if (SUCCEEDED(hr))
    {
        // Setup text layout properties
        m_textLayoutAnnotate->SetWordWrapping(DWRITE_WORD_WRAPPING_NO_WRAP);
        m_textLayoutAnnotate->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER);
        m_textLayoutAnnotate->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);
    }

    // Initialize animation controllers
    if (SUCCEEDED(hr))
    {
        hr = SharedObject<OrbitAnimation>::Create(&m_innerOrbitAnimation);
    }

    if (SUCCEEDED(hr))
    {
        hr = SharedObject<CarouselAnimation>::Create(&m_carouselAnimation);
    }

    return hr;
}

//
// Recalculate the current position/size of all orbits
//
HRESULT CarouselPaneMessageHandler::ResetOrbitValues()
{
    // Update inner orbit location
    HRESULT hr = m_innerOrbitAnimation->Setup(CalculateInnerOrbit(), InnerOrbitOpacity, 0);
    if (SUCCEEDED(hr))
    {
        // Update history orbits and thumbnails
        unsigned int index = 0;

        for (auto iter = m_carouselHistoryItems.begin(); iter != m_carouselHistoryItems.end(); iter++, index++)
        {
            if (SUCCEEDED(hr))
            {
                D2D1_ELLIPSE orbit = CalculateHistoryOrbit(index);
                double opacity = CalculateHistoryOrbitOpacity(index);
                hr = iter->OrbitAnimation->Setup(orbit, opacity, 0);
            }

            if (SUCCEEDED(hr))
            {
                D2D1_POINT_2F point = CalculateHistoryThumbnailPoint(index);
                double opacity = CalculateHistoryThumbnailOpacity(index);
                hr = iter->ThumbnailAnimation->Setup(point, opacity, 0);
            }
        }

        InvalidateWindow();
    }

    return hr;
}

//
// Start the animation for adding a new item to the history
//
HRESULT CarouselPaneMessageHandler::AnimateHistoryAddition(bool isUpdatingWindowSize)
{
    HRESULT hr = S_OK;

    // Offset for orbit animation to begin
    const UI_ANIMATION_SECONDS totalTime  = 0.5;

    // Animate each outside history orbit
    unsigned int index = 0;

    for (auto iter = m_carouselHistoryItems.begin(); iter != m_carouselHistoryItems.end(); iter++, index++)
    {
        if (index < m_carouselHistoryItems.size() - 1)
        {
            if (SUCCEEDED(hr))
            {
                // Update orbit
                hr = iter->OrbitAnimation->Setup(
                    CalculateHistoryOrbit(index),
                    CalculateHistoryOrbitOpacity(index),
                    totalTime
                    );
            }

            // Update thumbnail
            if (SUCCEEDED(hr))
            {
                hr = iter->ThumbnailAnimation->Setup(
                    CalculateHistoryThumbnailPoint(index),
                    CalculateHistoryThumbnailOpacity(index),
                    totalTime
                    );
            }
        }
        else
        {
            // Get target orbit for history
            D2D1_ELLIPSE targetOrbit = CalculateHistoryOrbit(index);

            if (SUCCEEDED(hr))
            {
                // Animate history orbit to final location
                hr = iter->OrbitAnimation->Setup(targetOrbit, CalculateHistoryOrbitOpacity(index), totalTime);
            }

            if (SUCCEEDED(hr))
            {
                // Animate history thumbnail to correct location
                D2D1_ELLIPSE startOrbit;
                D2D1_POINT_2F targetPoint = CalculateHistoryThumbnailPoint(index);
                D2D1_POINT_2F keyFramePoint;

                hr = m_innerOrbitAnimation->GetEllipse(&startOrbit, nullptr);

                if (SUCCEEDED(hr))
                {
                    keyFramePoint.x = startOrbit.point.x + startOrbit.radiusX - ThumbnailWidth;
                    keyFramePoint.y = startOrbit.point.y - ThumbnailHeight;

                    m_carouselHistoryItems.back().ThumbnailAnimation->Setup(
                        keyFramePoint,
                        targetPoint,
                        totalTime);
                }
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        D2D1_ELLIPSE targetOrbit = CalculateInnerOrbit();

        // Start inner orbit at center and animate to normal spot. Don't reset to center if we're simply resizing the
        // window during animation
        if (!isUpdatingWindowSize)
        {
            hr = m_innerOrbitAnimation->Setup(D2D1::Ellipse(targetOrbit.point, 0, 0), InnerOrbitOpacity, 0);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_innerOrbitAnimation->Setup(targetOrbit, InnerOrbitOpacity, totalTime);
        }

        if (SUCCEEDED(hr))
        {
            // Update opacity of icons on inner orbit. Don't set the initial opacity if we're simply resizing the
            // window during animation
            if (!isUpdatingWindowSize)
            {
                hr = m_carouselAnimation->SetupOpacity(0, 0);
            }

            hr = m_carouselAnimation->SetupOpacity(1.0, totalTime);
        }

        if (SUCCEEDED(hr))
        {
            // Shrink icons on inner orbit. Don't set the initial opacity if we're simply resizing the
            // window during animation
            if (!isUpdatingWindowSize)
            {
                hr = m_carouselAnimation->SetupScale(0.5, 0);
            }

            hr = m_carouselAnimation->SetupScale(1.0, totalTime);
        }
    }

#ifdef _MEASURE_FPS
    // Setup animation time
    ::GetSystemTime(&m_startAnimationTime);
    m_totalFramesRendered = 0;
#endif

    // Force redraw
    InvalidateWindow();

    return hr;
}

//
// Start the animation for expanding/contracting the list of history items
//
HRESULT CarouselPaneMessageHandler::AnimateHistoryExpansion()
{
    HRESULT hr = S_OK;

    // Toggle expansion
    m_isHistoryExpanded = !m_isHistoryExpanded;

    // Animate each outside history orbit
    unsigned int index = 0;

    for (auto iter = m_carouselHistoryItems.begin(); iter != m_carouselHistoryItems.end(); iter++, index++)
    {
        if (SUCCEEDED(hr))
        {
            // Update orbit
            hr = iter->OrbitAnimation->Setup(
                CalculateHistoryOrbit(index),
                CalculateHistoryOrbitOpacity(index),
                HistoryExpansionTime);
        }

        if (SUCCEEDED(hr))
        {
            // Update target point
            hr = iter->ThumbnailAnimation->Setup(
                CalculateHistoryThumbnailPoint(index),
                CalculateHistoryThumbnailOpacity(index),
                HistoryExpansionTime);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Move inner orbit to bottom-right corner
        hr = m_innerOrbitAnimation->Setup(CalculateInnerOrbit(), InnerOrbitOpacity, HistoryExpansionTime);
    }

    if (SUCCEEDED(hr))
    {
        // Update opacity of icons on inner orbit
        hr = m_carouselAnimation->SetupOpacity(m_isHistoryExpanded ? 0.6 : 1.0, HistoryExpansionTime);
    }

    if (SUCCEEDED(hr))
    {
        // Shrink icons on inner orbit
        hr = m_carouselAnimation->SetupScale(m_isHistoryExpanded ? 0.5 : 1.0, HistoryExpansionTime);
    }

#ifdef _MEASURE_FPS
    // Setup animation time
    ::GetSystemTime(&m_startAnimationTime);
    m_totalFramesRendered = 0;
#endif

    // Force redraw
    InvalidateWindow();

    return hr;
}

//
// Start the animation for going to a new folder
//
HRESULT CarouselPaneMessageHandler::AnimateNewFolderSet()
{
    // For now we are simply snapping the carousel pane to a neutral position
    double targetRotation = 0;

    if (m_carouselItems.size() > 0)
    {
        if (m_carouselItems.size() == 1)
        {
            targetRotation = 5 * PI / 4;
        }
        else
        {
            targetRotation = ((3 * PI) / 2) - (PI / m_carouselItems.size());
        }

        if (m_carouselItems.size() == 4)
        {
            targetRotation += PI / 6;
        }
    }

    HRESULT hr = m_carouselAnimation->SetupRotation(targetRotation, 0);
    if (SUCCEEDED(hr))
    {
        // Calculate location of back button
        if (m_carouselHistoryItems.size() > 0)
        {
            m_backButtonRect.left = HistoryThumbnailMarginLeft + (ThumbnailWidth / 2) - (BackButtonWidth / 2);
            m_backButtonRect.top = CarouselPaneMarginSize + ThumbnailHeight + ThumbnailTextHeight + 2;
            m_backButtonRect.right = m_backButtonRect.left + BackButtonWidth;
            m_backButtonRect.bottom = m_backButtonRect.top + BackButtonHeight;
        }
    }

    if (SUCCEEDED(hr))
    {
        float minWindowHeight = Direct2DUtility::ScaleValueForCurrentDPI(m_backButtonRect.bottom + m_carouselHistoryItems.size());

        if (m_carouselItems.size() == 0)
        {
            m_windowLayout->SetChildWindowLayoutHeight(0, static_cast<unsigned int>(minWindowHeight));
        }
        else
        {
            D2D1_ELLIPSE innerOrbit;

            if (m_isHistoryExpanded)
            {
                m_isHistoryExpanded = false;
                innerOrbit = CalculateInnerOrbit();
                m_isHistoryExpanded = true;
            }
            else
            {
                innerOrbit = CalculateInnerOrbit();
            }

            float height = Direct2DUtility::ScaleValueForCurrentDPI(innerOrbit.point.y + innerOrbit.radiusY + ThumbnailTextHeight);
            height = std::max(minWindowHeight, height);

            m_windowLayout->SetChildWindowLayoutHeight(0, static_cast<unsigned int>(height));
        }

        m_windowLayout->UpdateLayout();
    }

#ifdef _MEASURE_FPS
    // Setup animation time
    ::GetSystemTime(&m_startAnimationTime);
    m_totalFramesRendered = 0;
#endif

    return hr;
}

//
// Kick off the animation for rotating the carousel
//
HRESULT CarouselPaneMessageHandler::RotateCarousel(double speed)
{
    // Get current value of transition and geometry length
    double currentRotation;

    HRESULT hr = m_carouselAnimation->GetInfo(&currentRotation, nullptr, nullptr);
    if (SUCCEEDED(hr))
    {
        // Rotate counter-clockwise
        currentRotation -= PI * (speed / CarouselSpeedFactor);

        hr = m_carouselAnimation->SetupRotation(
            currentRotation,
            std::max(static_cast<float>(speed / CarouselSpeedFactor), 3.0f));
    }

#ifdef _MEASURE_FPS
    // Setup animation time
    ::GetSystemTime(&m_startAnimationTime);
    m_totalFramesRendered = 0;
#endif

    InvalidateWindow();

    return hr;
}

//
// Move the inner orbit to the specified location
//
HRESULT CarouselPaneMessageHandler::UpdateCarouselLocation(double rotationLocation)
{
    return m_carouselAnimation->SetupRotation(rotationLocation, 0);
}

//
// Navigate one level up in the history tree
//
HRESULT CarouselPaneMessageHandler::NavigateBack()
{
    HRESULT hr = S_OK;

    if (m_carouselHistoryItems.size() > 1)
    {
        D2D1_ELLIPSE startOrbit = CalculateHistoryOrbit(static_cast<unsigned int>(m_carouselHistoryItems.size() - 1));

        // Navigate to penultimate item in history
        hr = NavigateToHistoryItem(static_cast<unsigned int>(m_carouselHistoryItems.size() - 2));

        if (SUCCEEDED(hr))
        {
            // Custom animations just for navigating back
            D2D1_ELLIPSE newOrbit = CalculateInnerOrbit();
            newOrbit.radiusX = 0;
            newOrbit.radiusY = 0;

            hr = m_innerOrbitAnimation->Setup(startOrbit, InnerOrbitOpacity, 0);
        }

        if (SUCCEEDED(hr))
        {
            // Update opacity of icons on inner orbit
            hr = m_carouselAnimation->SetupOpacity(0, 0);
        }

        if (SUCCEEDED(hr))
        {
            // Shrink icons on inner orbit
            hr = m_carouselAnimation->SetupScale(0.5, 0);
        }

        if (SUCCEEDED(hr))
        {
            // Force history to not be expanded
            m_isHistoryExpanded = true;

            // Start expansion animation
            hr = AnimateHistoryExpansion();
        }

        if (SUCCEEDED(hr))
        {
            // Make sure nothing is highlighted
            hr = ClearMouseHover();
        }
    }

#ifdef _MEASURE_FPS
    // Setup animation time
    ::GetSystemTime(&m_startAnimationTime);
    m_totalFramesRendered = 0;
#endif

    return hr;
}

//
// Navigate to the specified history item
//
HRESULT CarouselPaneMessageHandler::NavigateToHistoryItem(unsigned int historyItemIndex)
{
    HRESULT hr = S_OK;

    if (historyItemIndex < m_carouselHistoryItems.size() - 1)
    {
        ThumbnailInfo info;
        hr = m_carouselHistoryItems[historyItemIndex].Thumbnail->GetThumbnailInfo(&info);

        if (SUCCEEDED(hr))
        {
            hr = SetCurrentLocation(info.shellItem, false);
        }
    }

    // Update history stack
    while (m_carouselHistoryItems.size() > historyItemIndex + 1)
    {
        m_carouselHistoryItems.pop_back();
    }

#ifdef _MEASURE_FPS
    // Setup animation time
    ::GetSystemTime(&m_startAnimationTime);
    m_totalFramesRendered = 0;
#endif

    return S_OK;
}

//
// Check if the mouse cursor is hovering over any items in the carousel pane and toggle
// hover state as needed
//
HRESULT CarouselPaneMessageHandler::CheckForMouseHover(D2D1_POINT_2F mousePosition)
{
    bool iconFound = false;
    bool needsRedraw = false;

    HRESULT hr = S_OK;

    // Deterine if any inner orbit item has mouse focus
    for (auto iter = m_carouselItems.begin(); iter != m_carouselItems.end(); iter++)
    {
        ThumbnailSelectionState selectionState;
        (*iter)->GetSelectionState(&selectionState);

        if (iconFound || m_isHistoryExpanded)
        {
            // Only allow one item to be selected
            if (selectionState == SelectionStateHoverOn)
            {
                (*iter)->SetSelectionState(SelectionStateNone);
                needsRedraw = true;
            }
        }
        else
        {
            D2D1_RECT_F rect;
            hr = (*iter)->GetRect(&rect);

            if (SUCCEEDED(hr))
            {
                if (Direct2DUtility::HitTest(rect, mousePosition))
                {
                    iconFound = true;

                    if (selectionState == SelectionStateNone)
                    {
                        (*iter)->SetSelectionState(SelectionStateHoverOn);
                        needsRedraw = true;
                    }
                }
                else if (selectionState == SelectionStateHoverOn)
                {
                    (*iter)->SetSelectionState(SelectionStateNone);
                    needsRedraw = true;
                }
            }
        }
    }

    // Determine if any history item has mouse focus
    for (auto iter = m_carouselHistoryItems.begin(); iter != m_carouselHistoryItems.end(); iter++)
    {
        ThumbnailSelectionState selectionState = SelectionStateNone;

        if (SUCCEEDED(hr))
        {
            iter->Thumbnail->GetSelectionState(&selectionState);
        }

        if (needsRedraw || !m_isHistoryExpanded)
        {
            // Only allow one item to be selected
            if (selectionState == SelectionStateHoverOn)
            {
                iter->Thumbnail->SetSelectionState(SelectionStateNone);
                needsRedraw = true;
            }
        }
        else
        {
            D2D1_RECT_F rect;
            hr = iter->Thumbnail->GetRect(&rect);

            if (SUCCEEDED(hr))
            {
                if (Direct2DUtility::HitTest(rect, mousePosition))
                {
                    if (selectionState == SelectionStateNone)
                    {
                        iter->Thumbnail->SetSelectionState(SelectionStateHoverOn);
                        needsRedraw = true;
                    }
                }
                else if (selectionState == SelectionStateHoverOn)
                {
                    iter->Thumbnail->SetSelectionState(SelectionStateNone);
                    needsRedraw = true;
                }
            }
        }
    }

    // Determine if the annotator button has focus
    if (Direct2DUtility::HitTest(m_annotateButtonSelectionRect.rect, mousePosition))
    {
        // Only request redraw if the hover state has changed
        if (!m_isAnnotatorButtonMouseHover)
        {
            needsRedraw = true;
            m_isAnnotatorButtonMouseHover = true;
            CalculateApplicationButtonRects();
        }
    }
    else
    {
        if (m_isAnnotatorButtonMouseHover)
        {
            needsRedraw = true;
            m_isAnnotatorButtonMouseHover = false;
            CalculateApplicationButtonRects();
        }
    }

    // Determine if the sharing application button has focus
    if (Direct2DUtility::HitTest(m_shareButtonSelectionRect.rect, mousePosition))
    {
        // Only request redraw if the hover state has changed
        if (!m_isSharingButtonMouseHover)
        {
            needsRedraw = true;
            m_isSharingButtonMouseHover = true;
            CalculateApplicationButtonRects();
        }
    }
    else
    {
        if (m_isSharingButtonMouseHover)
        {
            needsRedraw = true;
            m_isSharingButtonMouseHover = false;
            CalculateApplicationButtonRects();
        }
    }

    // Redraw
    if (needsRedraw)
    {
        InvalidateWindow();
    }

    return hr;
}

//
// Remove all items from the inner orbit
//
HRESULT CarouselPaneMessageHandler::RemoveAllItems()
{
    m_AsyncLoaderHelper->ClearItems();
    m_carouselItems.clear();

    return S_OK;
}

//
// Turn off all mouse hover states
//
HRESULT CarouselPaneMessageHandler::ClearMouseHover()
{
    HRESULT hr = S_OK;

    for ( auto iter = m_carouselItems.begin(); iter != m_carouselItems.end(); iter++)
    {
        ComPtr<IThumbnail> thumbnail;

        hr = (*iter).QueryInterface(&thumbnail);
        if (SUCCEEDED(hr))
        {
            thumbnail->SetSelectionState(SelectionStateNone);
        }
    }

    for (auto iter = m_carouselHistoryItems.begin(); iter != m_carouselHistoryItems.end(); iter++)
    {
        ComPtr<IThumbnail> thumbnail;

        hr = iter->Thumbnail.QueryInterface(&thumbnail);
        if (SUCCEEDED(hr))
        {
            thumbnail->SetSelectionState(SelectionStateNone);
        }
    }

    return hr;
}

//
// IAsyncLoaderMemoryManagerClient
//
HRESULT CarouselPaneMessageHandler::GetClientItemSize(unsigned int* clientItemSize)
{
    if (nullptr == clientItemSize)
    {
        return E_POINTER;
    }

    // Locking is not important for this resource.
    *clientItemSize = static_cast<unsigned int>(ThumbnailWidth);
    return S_OK;
}

//
// IChildNotificationHandler
//
HRESULT CarouselPaneMessageHandler::OnChildChanged()
{
    InvalidateWindow();
    return S_OK;
}

//
// Called when the carousel pane handler is about to be destroyed
//
HRESULT CarouselPaneMessageHandler::Finalize()
{
    // Remove reference to dependent objects
    m_windowLayout = nullptr;
    m_mediaPane = nullptr;

    // Clear all itms from inner orbit and corresponding asynchronous loader
    HRESULT hr = RemoveAllItems();

    if (SUCCEEDED(hr))
    {
        // Shutdown the asynchronous loader
        hr = m_AsyncLoaderHelper->Shutdown();
    }

    return hr;
}

//
// Set the corresponding window layout that contains the window associated with this message handler
//
HRESULT CarouselPaneMessageHandler::SetWindowLayout(IWindowLayout* layout)
{
    m_windowLayout = layout;
    return S_OK;
}

//
// Set the media pane so that location updates can be communicated to the media pane
//
HRESULT CarouselPaneMessageHandler::SetMediaPane(IPane* mediaPane)
{
    m_mediaPane = mediaPane;
    return S_OK;
}