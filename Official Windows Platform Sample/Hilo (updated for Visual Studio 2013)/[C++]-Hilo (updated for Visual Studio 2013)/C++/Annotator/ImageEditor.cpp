//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "ImageEditor.h"

#include "DrawGeometryOperation.h"
#include "ImageTransformationOperation.h"
#include "ImageClippingOperation.h"
#include "PropertySet.h"
#include "Resource.h"
#include "RibbonIds.h"
#include "ShellFileDialog.h"
#include "Taskbar.h"

const int ImageEditorHandler::BackgroundColor = 0xFFFFFF;
const int ImageEditorHandler::PreviousNextImageRangeCount = 2;

const float ImageEditorHandler::ImageMargin = 30;
const float ImageEditorHandler::KeyboardPanDistance = 25;
const float ImageEditorHandler::PreviousNextImageMargin = 60;
const float ImageEditorHandler::SlideAnimationDuration = 0.25f;
const float ImageEditorHandler::TransformationAnimationDuration = 0.25f;
const float ImageEditorHandler::ZoomMinimum = 1.0;
const float ImageEditorHandler::ZoomMaximum = 4.0;
const float ImageEditorHandler::ZoomStep = 0.25f;

using namespace Hilo::AnimationHelpers;
using namespace Hilo::Direct2DHelpers;
using namespace Hilo::WindowApiHelpers;

//
// Constructor
//
ImageEditorHandler::ImageEditorHandler() :
    m_animationEnabled(false),
    m_switchingImages(false),
    m_currentIndex(0),
    m_currentRangeStart(-1),
    m_currentRangeEnd(-1),
    m_currentZoom(1),
    m_maxSlideDistance(0),
    m_currentPanPoint(D2D1::Point2F(0, 0)),
    m_currentPanBoundary(D2D1::RectF(0, 0, 0, 0)),
    m_currentDrawingOperationType(ImageOperationTypeNone),
    m_isDrawing(false),
    m_isClipping(false),
    m_isRotation(false),
    m_isFlip(false),
    m_penSize(2),
    m_penColor(D2D1::ColorF(D2D1::ColorF::Black))
{
#ifdef _MEASURE_FPS
    m_logFile.open("annotator-fps.log", std::ofstream::app);

    SYSTEMTIME currentTime;
    ::GetLocalTime(&currentTime);
    m_logFile << "Annotator FPS Logging started @ " << currentTime.wHour << ":" << currentTime.wMinute << ":" << currentTime.wSecond <<"\r\n" ;

#endif
}

//
// Destructor
//
ImageEditorHandler::~ImageEditorHandler()
{
}

//
// Reset the state of the image editor
//
HRESULT ImageEditorHandler::Reset()
{
    m_images.clear();
    m_currentIndex = 0;
    m_currentRangeStart = -1;
    m_currentRangeEnd = -1;
    return S_OK;
}

//
// Loads the images in the specifed list
//
HRESULT ImageEditorHandler::LoadShellItems(const std::vector<ComPtr<IShellItem> >* shellItems, IShellItem* currentItem)
{
    HRESULT hr = Reset();
    if (SUCCEEDED(hr))
    {
        for (auto shellItem = shellItems->begin() ; shellItem != shellItems->end(); shellItem++)
        {
            ImageInfo info(*shellItem);
            ImageItem newItem;

            if (FAILED(SharedObject<SimpleImage>::Create(info, &newItem.Image)))
            {
                continue;
            }

            // For now do not create animation objects. These are managed when animation begins/ends
            m_images.push_back(newItem);

            if (nullptr != currentItem)
            {
                int compareResult;

                if (SUCCEEDED(currentItem->Compare(*shellItem, SICHINT_DISPLAY,  &compareResult)))
                {
                    if (0 == compareResult)
                    {
                        m_currentIndex = static_cast<int>(m_images.size()) - 1;
                    }
                }
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = ManageImageResources();
    }

    if (SUCCEEDED(hr))
    {
        hr = CalculateImagePositions();
    }

    InvalidateWindow();

    return hr;
}

//
// Loads the images in the specifed folder
//
HRESULT ImageEditorHandler::SetCurrentLocation(IShellItem* shellItem, bool isSingleImage)
{
    // Clear current list of items
    HRESULT hr = Reset();

    // List of shell items from which to load our images
    std::vector<ComPtr<IShellItem> > shellItems;

    // Current item
    ComPtr<IShellItem> currentItem = shellItem;

    if (isSingleImage)
    {
        // Parse for directory name since we want to load the specified directory
        wchar_t * buffer;
        hr = shellItem->GetDisplayName(SIGDN_FILESYSPATH, &buffer);

        if (SUCCEEDED(hr))
        {
            std::wstring imageFileName(buffer);
            std::wstring directoryName = imageFileName.substr(0, imageFileName.find_last_of(L"\\"));

            ::SHCreateItemFromParsingName(directoryName.c_str(), nullptr, IID_PPV_ARGS(&shellItem));

            ::CoTaskMemFree(buffer);
        }
    }

    // Ignore the returned HRESULT because it's possible no elements are found in this folder
    ShellItemsLoader::EnumerateFolderItems(shellItem, FileTypeImage, false, shellItems);

    if (!shellItems.empty())
    {
        hr = LoadShellItems(&shellItems, isSingleImage ? currentItem : nullptr);
    }
    else
    {
        hr = OpenFile();
    }

    return hr;
}

//
// Sets the current location using the specified command line arguments
//
HRESULT ImageEditorHandler::SetCurrentLocationFromCommandLine()
{
    HRESULT hr = S_OK;

    // Indicates whether or not a directory was found on the command line
    bool foundDirectory = false;

    // List of shell items to load
    std::vector<ComPtr<IShellItem> > shellItems;

    // Process command line
    int argumentCount;
    wchar_t ** commandArgumentList = CommandLineToArgvW(GetCommandLineW(), &argumentCount);

    for (int i = 1; i < argumentCount; i++)
    {
        ComPtr<IShellItem> item;

        // Attempt create a shell item from the current command line argument
        if (SUCCEEDED(::SHCreateItemFromParsingName(commandArgumentList[i], nullptr, IID_PPV_ARGS(&item))))
        {
            // Check if this item is a folder
            SFGAOF attributes;
            if (SUCCEEDED(item->GetAttributes(SFGAO_FOLDER, &attributes)))
            {
                if ((attributes & SFGAO_FOLDER) == SFGAO_FOLDER)
                {
                    // Only one location can be set so we are done parsing command line arguments once we find a
                    // valid directory
                    hr = SetCurrentLocation(item, false);
                    foundDirectory = true;
                    break;
                }
            }

            shellItems.push_back(item);
        }
    }

    if (!foundDirectory)
    {
        hr = LoadShellItems(&shellItems, nullptr);
    }

    return m_images.empty() ? S_FALSE : hr;
}

//
// Specifies a message to be shown in the image editor until the next window redraw
//
void ImageEditorHandler::SetMessage(std::wstring message)
{
    if (m_textLayout)
    {
        m_textLayout = nullptr;
    }

    // Create text layout for new message
    m_dWriteFactory->CreateTextLayout(
        message.c_str(),
        static_cast<unsigned int>(message.length()),
        m_textFormat,
        Direct2DUtility::GetRectWidth(m_imageBoundaryRect),
        16,
        &m_textLayout);
}

//
//  This method creates resources which are not bound to a particular Direct2D render target
//
HRESULT ImageEditorHandler::CreateDeviceIndependentResources()
{
    HRESULT hr = Direct2DUtility::GetD2DFactory(&m_d2dFactory);

    if (SUCCEEDED(hr))
    {
        hr = Direct2DUtility::GetDWriteFactory(&m_dWriteFactory);
    }

    if (SUCCEEDED(hr))
    {
        float dashes[] = {1.0f, 2.0f, 2.0f, 1.0f, 2.0f, 2.0f};
        // Stroke Style with custom Dash Style
        hr = m_d2dFactory->CreateStrokeStyle(
            D2D1::StrokeStyleProperties(
                D2D1_CAP_STYLE_FLAT,
                D2D1_CAP_STYLE_FLAT,
                D2D1_CAP_STYLE_ROUND,
                D2D1_LINE_JOIN_MITER,
                10.0f,
                D2D1_DASH_STYLE_CUSTOM,
                0.0f),
            dashes,
            ARRAYSIZE(dashes),
            &strokeStyleCustomOffsetZero);
    }

    if (SUCCEEDED(hr))
    {
        // Create text format
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

    // Create animation objects
    if (SUCCEEDED(hr))
    {
        hr = AnimationUtility::GetAnimationManager(&m_animationManager);
    }

    if (SUCCEEDED(hr))
    {
        hr = AnimationUtility::GetTransitionLibrary(&m_transitionLibrary);
    }

    return hr;
}

//
//  This method creates resources which are bound to a particular Direct2D render target
//
HRESULT ImageEditorHandler::CreateDeviceResources()
{
    HRESULT hr = S_OK;

    if (m_renderTarget == nullptr)
    {
        ComPtr<IWindow> window;
        hr = GetWindow(&window);

        if (SUCCEEDED(hr))
        {
            HWND hWnd;
            hr = window->GetWindowHandle(&hWnd);

            if (SUCCEEDED(hr))
            {
                RECT rect;
                if (GetClientRect(hWnd, &rect))
                {
                    D2D1_SIZE_U size = D2D1::SizeU(rect.right - rect.left, rect.bottom - rect.top);

                    // Create a Direct2D render target.
                    hr = m_d2dFactory->CreateHwndRenderTarget(
                        D2D1::RenderTargetProperties(),
                        D2D1::HwndRenderTargetProperties(hWnd, size),
                        &m_renderTarget);
                }
            }
        }

        // Create gradient stop collection for both left and right gradients
        ComPtr<ID2D1GradientStopCollection> gradientStopCollection;

        if (SUCCEEDED(hr))
        {
            D2D1_GRADIENT_STOP gradientStops[2];
            gradientStops[0].color = D2D1::ColorF(BackgroundColor, 1);
            gradientStops[0].position = 0.0f;
            gradientStops[1].color = D2D1::ColorF(BackgroundColor, 0);
            gradientStops[1].position = 1.0f;

            hr = m_renderTarget->CreateGradientStopCollection(
                gradientStops,
                2,
                D2D1_GAMMA_2_2,
                D2D1_EXTEND_MODE_CLAMP,
                &gradientStopCollection);

            // Create brush for left gradient
            if (SUCCEEDED(hr))
            {
                hr = m_renderTarget->CreateLinearGradientBrush(
                    D2D1::LinearGradientBrushProperties(
                    D2D1::Point2F(0, 0),
                    D2D1::Point2F(PreviousNextImageMargin, 0)),
                    gradientStopCollection,
                    &m_foregroundGradientBrushLeft);
            }

            // Create brush for right gradient
            if (SUCCEEDED(hr))
            {
                hr = m_renderTarget->CreateLinearGradientBrush(
                    D2D1::LinearGradientBrushProperties(
                    D2D1::Point2F(m_renderTarget->GetSize().width - PreviousNextImageMargin, 0),
                    D2D1::Point2F(m_renderTarget->GetSize().width, 0)),
                    gradientStopCollection,
                    &m_foregroundGradientBrushRight);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = m_renderTarget->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Black), &m_solidBrush);
        }

        if (SUCCEEDED(hr))
        {
            m_renderingParameters.renderTarget = m_renderTarget;
            m_renderingParameters.solidBrush = m_solidBrush;
        }
    }

    return hr;
}

//
// Discard any Direct2D resources which are no longer bound to a particular Direct2D render target
//
HRESULT ImageEditorHandler::DiscardDeviceResources()
{
    // Discard image resources
    for (auto image = m_images.begin(); image != m_images.end(); ++image)
    {
        (*image).Image->DiscardResources();
    }

    // Discard local Direct2D resources
    m_renderTarget = nullptr;
    m_foregroundGradientBrushLeft = nullptr;
    m_foregroundGradientBrushRight = nullptr;
    m_solidBrush = nullptr;

    return S_OK;
}

//
// Manage the currently loaded images. This method calculates the effective range based on the current image index.
// Images that have just moved outside the valid range will be discards. Images that have just moved into the valid
// range will be loaded.
//
HRESULT ImageEditorHandler::ManageImageResources()
{
    if (m_images.empty())
    {
        return S_OK;
    }

    // Calculate new range
    int rangeStart = m_currentIndex - PreviousNextImageRangeCount;
    int rangeEnd = m_currentIndex + PreviousNextImageRangeCount;

    // Update range based on valid values
    rangeStart = std::max(0, rangeStart);
    rangeEnd = std::min(static_cast<int>(m_images.size()) - 1, rangeEnd);

    // Discard resources for any images no longer in range
    for (int i = m_currentRangeStart; i < m_currentRangeEnd + 1; i++)
    {
        if (i >= 0 && (i < rangeStart || i > rangeEnd))
        {
            m_images[i].Image->DiscardResources();
        }
    }

    // Load resources as necessary
    for (int i = rangeStart; i < rangeEnd + 1; i++)
    {
        m_images[i].Image->SetRenderingParameters(m_renderingParameters);
        m_images[i].Image->Load();
        m_images[i].Image->SetBoundingRect(m_imageBoundaryRect);
    }

    // Update current range values
    m_currentRangeStart = rangeStart;
    m_currentRangeEnd = rangeEnd;

    return S_OK;
}

//
// Calculate the pan boundary based on the current zoom and screen size
//
HRESULT ImageEditorHandler::CalculatePanBoundary()
{
    if (m_images.empty())
    {
        return S_OK;
    }

    D2D1_RECT_F rect;
    HRESULT hr = m_images[m_currentIndex].Image->GetTransformedRect(GetCenter(), &rect);

    if (SUCCEEDED(hr))
    {
        float imageWidth = (rect.right - rect.left) * m_currentZoom;
        float imageHeight = (rect.bottom - rect.top) * m_currentZoom;

        float clientWidth = m_renderTarget->GetSize().width;
        float clientHeight = m_renderTarget->GetSize().height;

        if (clientWidth > imageWidth)
        {
            // Width is smaller than the client area. Don't allow panning in the x-axis
            m_currentPanBoundary.left = 0;
            m_currentPanBoundary.right = 0;
        }
        else
        {
            m_currentPanBoundary.left = (clientWidth / 2 ) - (imageWidth / 2);
            m_currentPanBoundary.right = m_currentPanBoundary.left * -1;
        }

        if (clientHeight > imageHeight)
        {
            // Height is smaller than the client area. Don't allow panning in the y-axis
            m_currentPanBoundary.top = 0;
            m_currentPanBoundary.bottom = 0;
        }
        else
        {
            m_currentPanBoundary.top = (clientHeight / 2) - (imageHeight / 2);
            m_currentPanBoundary.bottom = m_currentPanBoundary.top * -1;
        }
    }

    return hr;
}

//
// Calculate the visible image positions based on screen size
//
HRESULT ImageEditorHandler::CalculateImagePositions()
{
    HRESULT hr = S_OK;

    if (m_images.empty())
    {
        return S_OK;
    }

    for (int i = m_currentRangeStart; i < m_currentRangeEnd + 1; i++)
    {
        D2D1_RECT_F currentRect;

        if (SUCCEEDED(m_images[i].Image->GetDrawingRect(&currentRect)))
        {
            float width = currentRect.right - currentRect.left;

            switch(m_currentIndex - i)
            {
            case 2:
                {
                    // Far left image
                    D2D1_RECT_F siblingRect;
                    hr = m_images[i + 1].Image->GetDrawingRect(&siblingRect);

                    if (SUCCEEDED(hr))
                    {
                        float siblingWidth = siblingRect.right - siblingRect.left;

                        currentRect.right = PreviousNextImageMargin - siblingWidth / 2 - m_renderTarget->GetSize().width / 2;
                        currentRect.left = currentRect.right - width;
                    }

                    break;
                }
            case 1:
                {
                    // Left image
                    currentRect.right = PreviousNextImageMargin;
                    currentRect.left = currentRect.right - width;
                    break;
                }
            case 0:
                {
                    // Center image
                    currentRect.left = PreviousNextImageMargin + ImageMargin + (m_imageBoundaryRect.right - m_imageBoundaryRect.left) / 2 - width / 2;
                    currentRect.right = currentRect.left + width;
                    break;
                }
            case -1:
                {
                    // Right image
                    currentRect.left = m_renderTarget->GetSize().width - PreviousNextImageMargin;
                    currentRect.right = currentRect.left + width;
                    break;
                }
            case -2:
                {
                    // Far right image
                    D2D1_RECT_F siblingRect;
                    hr = m_images[i - 1].Image->GetDrawingRect(&siblingRect);

                    if (SUCCEEDED(hr))
                    {
                        float siblingWidth = siblingRect.right - siblingRect.left;

                        currentRect.left = m_renderTarget->GetSize().width * 1.5f - PreviousNextImageMargin + siblingWidth / 2;
                        currentRect.right = currentRect.left + width;
                        break;
                    }
                }
            }

            m_images[i].Image->SetDrawingRect(currentRect);
        }
    }

    return hr;
}

//
// Navigate to the previous image
//
bool ImageEditorHandler::PreviousImage()
{
    bool isPrevImage = false;

    // Ignore additional request to goto previous image while animation is active
    if (!m_animationEnabled)
    {
        if (m_currentIndex > 0)
        {
            --m_currentIndex;
            isPrevImage = true;
            m_switchingImages = true;
        }

        SetupAnimation();
    }

    return isPrevImage;
}

//
// Navigate to the next image
//
bool ImageEditorHandler::NextImage()
{
    bool isNextImage = false;

    // Ignore additional request to goto next image while animation is active
    if (!m_animationEnabled)
    {
        if (m_currentIndex < static_cast<int>(m_images.size()) - 1)
        {
            ++m_currentIndex;
            isNextImage = true;
            m_switchingImages = true;
        }

        SetupAnimation();
    }

    return isNextImage;
}

//
// Zoom in on the current image
//
HRESULT ImageEditorHandler::ZoomIn()
{
    if (m_currentZoom < ZoomMaximum)
    {
        m_currentZoom = std::min(ZoomMaximum, m_currentZoom + ZoomStep);
    }

    // Update pan boundaries based on current zoom
    HRESULT hr = CalculatePanBoundary();

    // Check for bounds
    PanImage(D2D1::Point2F(0, 0), true);

    return hr;
}

//
// Zoom out on the current image
//
HRESULT ImageEditorHandler::ZoomOut()
{
    if (m_currentZoom > ZoomMinimum)
    {
        m_currentZoom = std::max(ZoomMinimum, m_currentZoom - ZoomStep);
    }

    // Update pan boundaries based on current zoom
    HRESULT hr = CalculatePanBoundary();

    // Check for bounds
    PanImage(D2D1::Point2F(0, 0), true);

    return hr;
}

//
// Zoom out to zoom minimum on the current image
//
HRESULT ImageEditorHandler::ZoomFull()
{
    m_currentZoom = ZoomMinimum;

    return ZoomOut();
}

//
// Pans the current image by the specified offset and snaps the image back to it's pan boundary
//
void ImageEditorHandler::PanImage(D2D1_POINT_2F offset, bool snapToBounds)
{
    // Update pan values and then check for bounds
    m_currentPanPoint.x += offset.x;

    if (m_currentZoom > ZoomMinimum)
    {
        m_currentPanPoint.y += offset.y;
    }

    if (snapToBounds)
    {
        // Panning left
        if (m_currentPanPoint.x > m_currentPanBoundary.right)
        {
            m_currentPanPoint.x = m_currentPanBoundary.right;
        }

        // Panning right
        if (m_currentPanPoint.x < m_currentPanBoundary.left)
        {
            m_currentPanPoint.x = m_currentPanBoundary.left;
        }

        // Panning up
        if (m_currentPanPoint.y > m_currentPanBoundary.bottom)
        {
            m_currentPanPoint.y = m_currentPanBoundary.bottom;
        }

        // Panning down
        if (m_currentPanPoint.y < m_currentPanBoundary.top)
        {
            m_currentPanPoint.y = m_currentPanBoundary.top;
        }
    }

    InvalidateWindow();
}

//
// Draws the client area
//
void ImageEditorHandler::DrawClientArea()
{
    // Don't draw if window is occulded
    if (m_renderTarget->CheckWindowState() & D2D1_WINDOW_STATE_OCCLUDED)
    {
        return;
    }

    m_renderTarget->BeginDraw();

    // Apply rendering transform based on current pan point and zoom
    D2D1_MATRIX_3X2_F scale = D2D1::Matrix3x2F::Scale(m_currentZoom, m_currentZoom, GetCenter());
    D2D1_MATRIX_3X2_F translation = D2D1::Matrix3x2F::Translation(m_currentPanPoint.x, m_currentPanPoint.y);
    m_renderTarget->SetTransform(scale * translation);

    // Paint background
    m_renderTarget->Clear(D2D1::ColorF(BackgroundColor));

    if (!m_images.empty())
    {
        // Draw images in the current range
        for (int i = m_currentRangeStart; i < m_currentRangeEnd + 1; i++)
        {
            if (m_animationEnabled)
            {
                DrawAnimatedImages(i);
            }
            else
            {
                DrawImages(i);
            }
        }
    }

    // Draw gradients
    if (m_currentZoom <= 1.0)
    {
        m_renderTarget->SetTransform(D2D1::Matrix3x2F::Identity());

        DrawForeground();
    }

    // Drawing is done
    HRESULT hr = m_renderTarget->EndDraw();
    if (hr == D2DERR_RECREATE_TARGET)
    {
        DiscardDeviceResources();
        InvalidateWindow();
    }

    UpdateUIFramework();
}

HRESULT ImageEditorHandler::UpdateUIFramework()
{
    // After we're done drawing make sure to update framework buttons as necessary
    if (m_framework)
    {
        m_framework->InvalidateUICommand(ID_BUTTON_UNDO, UI_INVALIDATIONS_STATE, nullptr);
        m_framework->InvalidateUICommand(ID_BUTTON_REDO, UI_INVALIDATIONS_STATE, nullptr);
        m_framework->InvalidateUICommand(ID_FILE_SAVE, UI_INVALIDATIONS_STATE, nullptr);

        m_framework->InvalidateUICommand(pencilButton, UI_INVALIDATIONS_VALUE, nullptr);
        m_framework->InvalidateUICommand(cropButton, UI_INVALIDATIONS_VALUE, nullptr);
    }

    return S_OK;
}

void ImageEditorHandler::DrawAnimatedImages(int imageIndex)
{
    // Get drawing rectangle
    D2D1_RECT_F rect;
    m_images[imageIndex].Image->GetDrawingRect(&rect);

    // Update transforms based on animation type
    if (m_isRotation || m_isFlip)
    {
        // Only apply rotation/flip to current image
        if (imageIndex == m_currentIndex)
        {
            // Store current transition
            D2D1_MATRIX_3X2_F originalTransform;
            m_renderTarget->GetTransform(&originalTransform);

            // Calculate rotation point for animation
            D2D1_POINT_2F midPoint = D2D1::Point2F(
                m_currentPanPoint.x + rect.left + (rect.right - rect.left) / 2, 
                m_currentPanPoint.y + rect.top + (rect.bottom - rect.top) / 2);

            DOUBLE value = 0;
            m_transformationAnimationVariable->GetValue(&value);

            if (m_isRotation)
            {
                m_renderTarget->SetTransform(
                    originalTransform * D2D1::Matrix3x2F::Rotation(static_cast<float>(value), midPoint));
            }
            else // m_isFlip is true
            {
                bool isHorizontalFlip = m_currentDrawingOperationType == ImageOperationTypeFlipHorizontal;
                m_renderTarget->SetTransform(
                    originalTransform *
                    D2D1::Matrix3x2F::Skew(
                    isHorizontalFlip ? std::min<float>(10, 10 * std::sin(static_cast<float>(PI * std::abs(value)))) : 0, 
                    isHorizontalFlip ? 0 : std::min<float>(10, 10 * std::sin(static_cast<float>(PI * std::abs(value)))), 
                    midPoint) * 
                    D2D1::Matrix3x2F::Scale(
                    isHorizontalFlip ? -(static_cast<float>(value)) : 1.0f,
                    isHorizontalFlip ? 1.0f : -(static_cast<float>(value)),
                    midPoint));
            }

            // Draw image
            m_images[imageIndex].Image->Draw();

            // Restore previous tranform
            m_renderTarget->SetTransform(originalTransform);
        } // if (i == m_currentIndex)
        else if (m_currentZoom <= ZoomMaximum)
        {
            // Simply draw the image
            m_images[imageIndex].Image->Draw();
        }
    }
    // not a rotation or flip animation
    else
    {
        D2D1_POINT_2F point;
        if (SUCCEEDED(m_images[imageIndex].Animation->GetCurrentPoint(&point)))
        {
            // Use translation matrix to draw this image at the specified point.
            // Scale should not play a factor here since the slide animation
            // should only be kicked off when zoomed all the way out
            m_renderTarget->SetTransform(D2D1::Matrix3x2F::Translation(point.x- rect.left, point.y - rect.top));

            // Draw the image
            m_images[imageIndex].Image->Draw();
        }
    }
}

void ImageEditorHandler::DrawImages(int imageIndex)
{
    // Simply draw image
    if (imageIndex == m_currentIndex || m_currentZoom <= ZoomMinimum)
    {
        m_images[imageIndex].Image->Draw();

        // If cropping
        if (imageIndex == m_currentIndex && m_currentDrawingOperationType == ImageOperationTypeCrop)
        {
            D2D1_RECT_F imageRect;
            m_images[imageIndex].Image->GetTransformedRect(GetCenter(), &imageRect);

            D2D1_COLOR_F savedColor = m_solidBrush->GetColor();
            float savedOpacity = m_solidBrush->GetOpacity();

            m_solidBrush->SetColor(D2D1::ColorF(D2D1::ColorF::White));
            m_solidBrush->SetOpacity(0.75f);

            if (m_isClipping)
            {
                D2D1_RECT_F selectionRect = Direct2DUtility::FixRect(m_currentClipDrawBox);

                // Wash out unneeded areas
                m_renderTarget->FillRectangle(D2D1::RectF(imageRect.left, imageRect.top,  imageRect.right, selectionRect.top),  m_solidBrush);
                m_renderTarget->FillRectangle(D2D1::RectF(imageRect.left, selectionRect.top,  selectionRect.left, selectionRect.bottom),  m_solidBrush);
                m_renderTarget->FillRectangle(D2D1::RectF(selectionRect.right, selectionRect.top, imageRect.right, selectionRect.bottom),  m_solidBrush);
                m_renderTarget->FillRectangle(D2D1::RectF(imageRect.left, selectionRect.bottom,  imageRect.right, imageRect.bottom),  m_solidBrush);
            }
            else if (m_startClipping) // Not yet clipping
            {
                // Wash out the whole image
                m_renderTarget->FillRectangle(imageRect,  m_solidBrush);
                m_currentClipDrawBox = imageRect;
            }

            if (m_isClipping || m_startClipping)
            {
                m_solidBrush->SetOpacity(1);
                m_solidBrush->SetColor(D2D1::ColorF(D2D1::ColorF::Black));

                // The boundary box of the clipping rectangle
                m_renderTarget->DrawRectangle(m_currentClipDrawBox, m_solidBrush, 1, strokeStyleCustomOffsetZero);

                m_solidBrush->SetColor(savedColor);
                m_solidBrush->SetOpacity(savedOpacity);
                m_startClipping = false;
            }
        }
    }
}

//
// Draws the gradients in the foreground
//
void ImageEditorHandler::DrawForeground()
{
    // Draw left gradient
    m_renderTarget->FillRectangle(
        D2D1::RectF(0, 0, PreviousNextImageMargin, m_renderTarget->GetSize().height),
        m_foregroundGradientBrushLeft);

    // Draw right gradient
    m_renderTarget->FillRectangle(
        D2D1::RectF(
        m_renderTarget->GetSize().width - PreviousNextImageMargin,
        0,
        m_renderTarget->GetSize().width, m_renderTarget->GetSize().height),
        m_foregroundGradientBrushRight);

    // Draw message
    if (nullptr != m_textLayout)
    {
        m_solidBrush->SetColor(D2D1::ColorF(D2D1::ColorF(0.5f, 0.5f, 0.5f)));

        m_renderTarget->DrawTextLayout(
            D2D1::Point2F(PreviousNextImageMargin / 4, m_imageBoundaryRect.bottom + 5),
            m_textLayout,
            m_solidBrush);

        m_textLayout = nullptr;
    }
}

//
// Invalidates the current window which will cause a WM_PAINT to be generated
//
void ImageEditorHandler::InvalidateWindow()
{
    ComPtr<IWindow> window;
    if (SUCCEEDED(GetWindow(&window)))
    {
        window->RedrawWindow();
    }
}

//
// Setup all the necessary transitions for the slide animation
//
void ImageEditorHandler::SetupAnimation()
{
    // Only allow one animation at a time
    if (m_animationEnabled)
    {
        return;
    }

    // Initialize Animation
    m_animationEnabled = true;

    if (m_isRotation || m_isFlip)
    {
        // Setup transformtion animations
        SetupTransformationAnimation();
    }
    else
    {
        // Setup slide animation
        for(int i = m_currentRangeStart; i < m_currentRangeEnd + 1; i++)
        {
            D2D1_RECT_F rect;

            if (SUCCEEDED(m_images[i].Image->GetDrawingRect(&rect)))
            {
                D2D1_POINT_2F initialPoint;
                initialPoint.x = rect.left + m_currentPanPoint.x;
                initialPoint.y = rect.top + m_currentPanPoint.y;

                // Initialize animation with initial point
                SharedObject<PointAnimation>::Create(initialPoint, &m_images[i].Animation);
            }
        }

        // Recaulate image positions
        CalculateImagePositions();

        // Animate to target location
        for(int i = m_currentRangeStart; i < m_currentRangeEnd + 1; i++)
        {
            D2D1_RECT_F rect;

            if (SUCCEEDED(m_images[i].Image->GetDrawingRect(&rect)))
            {
                if (m_images[i].Animation)
                {
                    // Setup animation to animate to end location
                    m_images[i].Animation->Setup(D2D1::Point2F(rect.left,rect.top), SlideAnimationDuration);
                }
            }
        }
    }

#ifdef _MEASURE_FPS
    // Setup animation time
    ::GetSystemTime(&m_startAnimationTime);
    m_totalFramesRendered = 0;
#endif
    InvalidateWindow();
}

//
// Setup animation for rotation and flip transformations
//
void ImageEditorHandler::SetupTransformationAnimation()
{
    HRESULT hr = S_OK;

    m_transformationAnimationVariable = nullptr;

    if (m_isRotation)
    {
        hr = m_animationManager->CreateAnimationVariable(0, &m_transformationAnimationVariable);
    }
    else if (m_isFlip)
    {
        hr = m_animationManager->CreateAnimationVariable(-1, &m_transformationAnimationVariable);
    }
    else
    {
        return /* Unexpected */;
    }

    ComPtr<IUIAnimationStoryboard> storyboard;
    ComPtr<IUIAnimationTransition> transition;

    if (SUCCEEDED(hr))
    {
        // Initialize storyboard
        hr = m_animationManager->CreateStoryboard(&storyboard);
    }

    if (SUCCEEDED(hr))
    {
        // Create rotation transition
        hr = m_transitionLibrary->CreateAccelerateDecelerateTransition(
            TransformationAnimationDuration,
            m_isFlip ? 1 : (m_currentDrawingOperationType == ImageOperationTypeRotateClockwise ? 90 /* degrees */: -90),
            0.5,
            0.5,
            &transition);
    }

    if (SUCCEEDED(hr))
    {
        hr = storyboard->AddTransition(m_transformationAnimationVariable, transition);
    }

    AnimationUtility::ScheduleStoryboard(storyboard);
}

//
// Disposes of all the animation objects that are done animating
//
void ImageEditorHandler::CleanupAnimation()
{
#ifdef _MEASURE_FPS
    if (m_animationEnabled)
    {
        // Calauclate elapsed time
        SYSTEMTIME currentTime;
        ::GetSystemTime(&currentTime);

        unsigned long totalAnimationTime = (currentTime.wSecond * 1000 + currentTime.wMilliseconds) - (m_startAnimationTime.wSecond * 1000 + m_startAnimationTime.wMilliseconds);

        double fps = 0;
        if (totalAnimationTime > 0)
        {
            fps = m_totalFramesRendered / (totalAnimationTime / 1000.0);
        }

        m_logFile << "Frames per second for " << (m_isRotation ? "rotation " : (m_isFlip ? "flip " : "slide ")) << "animation = ( " << m_totalFramesRendered << " frames /" << totalAnimationTime << " millis) = " << fps << " fps\r\n";
    }
#endif
    // Disable animation
    m_animationEnabled = false;

    if (m_isRotation || m_isFlip)
    {
        ComPtr<IImageOperation> operation;
        if (SUCCEEDED(SharedObject<ImageTransformationOperation>::Create(m_currentDrawingOperationType, &operation)))
        {
            m_images[m_currentIndex].Image->PushImageOperation(operation);
        }

        m_currentDrawingOperationType = m_prevDrawingOperationType;

        m_isRotation = false;
        m_isFlip = false;
    }

    for (int i = m_currentRangeStart; i < m_currentRangeEnd + 1; i++)
    {
        m_images[i].Animation = nullptr;
    }

    m_images[m_currentIndex].Image->SetBoundingRect(m_imageBoundaryRect);

    // Redraw client area since animation uses a faster method of drawing bitmaps that is not
    // as crisp as a static image
    InvalidateWindow();
}

//
// Event that is fired when the user clicks the File|Open menu item.
//
HRESULT ImageEditorHandler::OpenFile()
{
    // Save current image if needed
    if (!m_images.empty())
    {
        SaveFiles();
    }

    ShellFileDialog openDialog;
    HRESULT hr = openDialog.SetDefaultFolder(FOLDERID_Pictures);

    ComPtr<IWindow> window;
    if (SUCCEEDED(hr))
    {
        hr = GetWindow(&window);
    }

    std::vector<ComPtr<IShellItem> > shellItems;
    if (SUCCEEDED(hr))
    {
        hr = openDialog.ShowOpenDialog(window, &shellItems);
    }

    if (SUCCEEDED(hr))
    {
        hr = LoadShellItems(&shellItems, nullptr);
    }

    return hr;
}

//
// Saves the current image to file
//
HRESULT ImageEditorHandler::SaveFiles()
{
    HCURSOR savedCursor = ::SetCursor(::LoadCursor(nullptr, IDC_WAIT));

    if (m_switchingImages)
    {
        // Save the image to the left and right of the current image
        if (m_currentIndex > 0)
        {
            SaveFileAtIndex(m_currentIndex - 1);
        }

        if (m_currentIndex < static_cast<int>(m_images.size()) - 1)
        {
            SaveFileAtIndex(m_currentIndex + 1);
        }
    }
    else
    {
        SaveFileAtIndex(m_currentIndex);
    }

    // Reload images and calculate updates positions as needed
    ManageImageResources();
    CalculateImagePositions();
    InvalidateWindow();

    ::SetCursor(savedCursor);

    return S_OK;
}

HRESULT ImageEditorHandler::SaveFileAs()
{
    ComPtr<IWindow> window;
    HRESULT hr = GetWindow(&window);

    ImageInfo info;
    if (SUCCEEDED(hr))
    {
        hr = m_images[m_currentIndex].Image->GetImageInfo(&info);
    }

    ComPtr<IShellItem> outputItem;
    if (SUCCEEDED(hr))
    {
        ShellFileDialog saveDialog;
        hr = saveDialog.ShowSaveDialog(window, info.shellItem, &outputItem);
    }

    if (SUCCEEDED(hr))
    {
        // Check if user requested to save to the same image
        int result = 0;
        info.shellItem->Compare(outputItem, SICHINT_DISPLAY, &result);

        if (0 == result)
        {
            hr = SaveFileAtIndex(m_currentIndex);
        }
        else
        {
            hr = m_images[m_currentIndex].Image->Save(outputItem);
            if (FAILED(hr))
            {
                ShowSaveFailure(m_currentIndex);
            }
        }
    }

    return hr;
}

HRESULT ImageEditorHandler::SaveFileAtIndex(int index)
{
    // Save the current image
    if (FAILED(m_images[index].Image->Save()))
    {
        ShowSaveFailure(index);
    }
    else
    {
        ImageInfo info;
        if (SUCCEEDED(m_images[index].Image->GetImageInfo(&info)))
        {
            // Add this file to recent docs so that it can show up in the jump list
            if (!info.fileName.empty())
            {
                SHAddToRecentDocs(SHARD_PATH, info.fileName.c_str());
            }
            // Display back up message
            if (!info.backupFileName.empty())
            {
                std::wstring saveMessage = L"Original copy of ";

                if (index == m_currentIndex)
                {
                    saveMessage += L"current";
                }
                else
                {
                    saveMessage += L"previous";
                }

                saveMessage += L" image backed up to: ";

                // Get the relative path before '\AnnotatorBackup'
                unsigned int stringIndex = static_cast<unsigned int>(info.backupFileName.find_last_of('\\'));
                stringIndex = static_cast<unsigned int>(info.backupFileName.find_last_of('\\', stringIndex - 1));

                if (stringIndex > 0)
                {
                    saveMessage += L"." + info.backupFileName.substr(stringIndex);
                }
                else
                {
                    saveMessage += info.backupFileName;
                }

                SetMessage(saveMessage);
            }
        }
    }

    return S_OK;
}

//
// Event that is fired when the user submits the 'Backward' application command. This is usually done via a flick or mouse
//
HRESULT ImageEditorHandler::OnAppCommandBrowserBackward() 
{
    PreviousImage();

    return S_OK;
}

//
// Event that is fired when the user submits the 'Forward' application command. This is usually done via a flick
//
HRESULT ImageEditorHandler::OnAppCommandBrowserForward() 
{
    NextImage();

    return S_OK;
}

//
// Event that is fired when the user click the backward or forward button in the thumbnail toolbar of taskbar
//
HRESULT ImageEditorHandler::OnCommand(WPARAM wParam, LPARAM /*lParam*/)
{
    unsigned int const cmdId = LOWORD(wParam);
    if (cmdId == APPCOMMAND_BROWSER_FORWARD)
    {
        NextImage();
    }
    else if (cmdId == APPCOMMAND_BROWSER_BACKWARD)
    {
        PreviousImage();
    }

    return S_OK;
}

//
// Event that is called when the corresponding window is created
//
HRESULT ImageEditorHandler::OnCreate()
{
    HRESULT hr = CreateDeviceIndependentResources();

    if (SUCCEEDED(hr))
    {
        CreateDeviceResources();
    }

    return hr;
}

//
// Event for erasing the background.
//
HRESULT ImageEditorHandler::OnEraseBackground()
{
    // Simply return S_OK since the background is cleared via Direct2D rather than Windows    
    return S_OK;
}

//
// Event that is fired when a key on the keyboard is pressed
//
HRESULT ImageEditorHandler::OnKeyDown(unsigned int vKey)
{
    if (vKey == VK_LEFT)
    {
        if (m_currentZoom <= ZoomMinimum)
        {
            PreviousImage();
        }
        else
        {
            PanImage(D2D1::Point2F(KeyboardPanDistance, 0), true);
        }
    }
    else if (vKey == VK_RIGHT)
    {
        if (m_currentZoom <= ZoomMinimum)
        {
            NextImage();
        }
        else
        {
            PanImage(D2D1::Point2F(-KeyboardPanDistance, 0), true);
        }
    }
    else if (vKey == VK_UP)
    {
        if (m_currentZoom > ZoomMinimum)
        {
            PanImage(D2D1::Point2F(0, KeyboardPanDistance), true);
        }
    }
    else if (vKey == VK_DOWN)
    {
        if (m_currentZoom > ZoomMinimum)
        {
            PanImage(D2D1::Point2F(0, -KeyboardPanDistance), true);
        }
    }
    else if (vKey == VK_ADD || vKey == VK_OEM_PLUS)
    {
        ZoomIn();
    }
    else if (vKey == VK_SUBTRACT || vKey == VK_OEM_MINUS)
    {
        ZoomOut();
    }
    else if (vKey == VK_ESCAPE)
    {
        m_currentZoom = ZoomMinimum;
        ZoomOut();
    }

    return S_OK;
}

//
// Event that is called when the left mouse button is pressed
//
HRESULT ImageEditorHandler::OnLeftMouseButtonDown(D2D1_POINT_2F mousePosition)
{
    HRESULT hr = S_OK;
    bool isHit = IsImageHit(mousePosition);

    if (!m_isDrawing && m_currentDrawingOperationType == ImageOperationTypePen && isHit)
    {
        hr = SharedObject<DrawGeometryOperation>::Create(&m_currentOperation);

        ComPtr<IDrawGeometryOperation> drawGeometry;
        if (SUCCEEDED(m_currentOperation.QueryInterface(&drawGeometry)))
        {
            drawGeometry->SetBrushColor(m_penColor);
            drawGeometry->SetStrokeSize(m_penSize);

            drawGeometry->AppendPoint(m_renderTarget, GetAbsolutePosition(mousePosition));

            m_images[m_currentIndex].Image->PushImageOperation(m_currentOperation);

        }

        m_isDrawing = true;
        InvalidateWindow();

    }
    else if (m_currentDrawingOperationType == ImageOperationTypeCrop && isHit)
    {
        m_currentClipBoundary.left = m_currentClipBoundary.right = mousePosition.x;
        m_currentClipBoundary.top = m_currentClipBoundary.bottom = mousePosition.y;

        // First transform back the point (disregarding current translation and scale)
        mousePosition = RemoveRenderingTransformations(mousePosition);

        // Adjust for drawing
        m_currentClipDrawBox.left = m_currentClipDrawBox.right = mousePosition.x;
        m_currentClipDrawBox.top = m_currentClipDrawBox.bottom = mousePosition.y;

        m_isClipping = true;
        InvalidateWindow();
    }
    else
    {
        // Store mouse point for panning
        m_previousMousePosition = mousePosition;

        // Store mouse point for mouse up
        m_mouseDownPosition = mousePosition;
    }

    // Capture the mouse cursor
    ComPtr<IWindow> window;
    if (SUCCEEDED(GetWindow(&window)))
    {
        window->SetCapture();
    }

    return hr;
}

//
// Event that is fired when the left mouse button is released
//
HRESULT ImageEditorHandler::OnLeftMouseButtonUp(D2D1_POINT_2F mousePosition)
{
    HRESULT hr = S_OK;

    // Release mouse capture
    ::ReleaseCapture();

    if (m_isDrawing)
    {
        // Reset the current image operation
        m_currentOperation = nullptr;
        m_isDrawing = false;

        InvalidateWindow();
    }
    else if (m_isClipping)
    {
        // Crop only if the crop area is large enough
        if (Direct2DUtility::GetRectWidth(m_currentClipBoundary) > 0 && Direct2DUtility::GetRectHeight(m_currentClipBoundary) > 0)
        {
            D2D1_POINT_2F clipStart = GetAbsolutePosition(D2D1::Point2F(m_currentClipBoundary.left, m_currentClipBoundary.top));
            D2D1_POINT_2F clipEnd = GetAbsolutePosition(mousePosition);

            ComPtr<IImageOperation> clip;
            hr = SharedObject<ImageClippingOperation>::Create(
                D2D1::RectF(clipStart.x, clipStart.y, clipEnd.x, clipEnd.y),
                &clip);

            m_images[m_currentIndex].Image->PushImageOperation(clip);

            // Reset the pan points
            m_currentPanPoint.x = 0;
            m_currentPanPoint.y = 0;
        }

        m_isClipping = false;
        InvalidateWindow();
    }
    else if (m_currentZoom <= ZoomMinimum)
    {
        // Determine if user has clicked on left or right image
        if (mousePosition.x < PreviousNextImageMargin && m_mouseDownPosition.x < PreviousNextImageMargin)
        {
            PreviousImage();
        }
        else if (mousePosition.x > m_renderTarget->GetSize().width - PreviousNextImageMargin &&
            m_mouseDownPosition.x > m_renderTarget->GetSize().width - PreviousNextImageMargin)
        {
            NextImage();
        }
        else
        {
            // Determine if user has panned far enough to go to the next/previous image. The trigger
            // distance is 1/4 of the main image area
            float triggerDistance = (m_renderTarget->GetSize().width - PreviousNextImageMargin * 2) / 4;

            if (std::fabs(m_currentPanPoint.x) > triggerDistance)
            {
                if (m_currentPanPoint.x > 0)
                {
                    PreviousImage();
                }
                else
                {
                    NextImage();
                }
            }
            else
            {
                SetupAnimation();
            }
        }
    }

    PanImage(D2D1::Point2F(0,0), true);

    return hr;
}

//
// Event that is fired when the mouse is moved
//
HRESULT ImageEditorHandler::OnMouseMove(D2D1_POINT_2F mousePosition)
{
    HRESULT hr = S_OK;

    UpdateMouseCursor(mousePosition);

    if (m_isDrawing)
    {
        ComPtr<IDrawGeometryOperation> drawGeometry;
        if (SUCCEEDED(m_currentOperation.QueryInterface(&drawGeometry)))
        {
            drawGeometry->AppendPoint(m_renderTarget, GetAbsolutePosition(mousePosition));
            InvalidateWindow();
        }
    }
    else if (m_isClipping)
    {
        m_currentClipBoundary.right = mousePosition.x;
        m_currentClipBoundary.bottom = mousePosition.y;

        mousePosition = RemoveRenderingTransformations(mousePosition);

        D2D1_RECT_F rect;
        m_images[m_currentIndex].Image->GetTransformedRect(GetCenter(), &rect);

        m_currentClipDrawBox.right = std::max(rect.left, std::min(rect.right, mousePosition.x));
        m_currentClipDrawBox.bottom = std::max(rect.top, std::min(rect.bottom, mousePosition.y));


        InvalidateWindow();
    }
    else
    {
        ComPtr<IWindow> window;
        hr = GetWindow(&window);

        if (SUCCEEDED(hr))
        {
            bool isMouseCaptured = false;
            window->IsMouseCaptured(&isMouseCaptured);
            if (isMouseCaptured)
            {
                // Update pan based on delta
                D2D1_POINT_2F delta;
                delta.x = (mousePosition.x - m_previousMousePosition.x);
                delta.y = (mousePosition.y - m_previousMousePosition.y);

                if (!(delta.x == 0 && delta.y == 0))
                {
                    PanImage(delta, false);

                    // Save current mouse position for next mouse message
                    m_previousMousePosition = mousePosition;

                    // Keep pan within the boundaries on the maximum slide distance
                    if (std::abs(m_currentPanPoint.x) > m_maxSlideDistance)
                    {
                        if (m_currentPanPoint.x > 0)
                        {
                            m_currentPanPoint.x = m_maxSlideDistance;
                        }
                        else
                        {
                            m_currentPanPoint.x = -m_maxSlideDistance;
                        }
                    }
                }
            }
        }
    }
    return hr;
}

//
// Event that is fired when the mouse enters the client area
//
HRESULT ImageEditorHandler::OnMouseEnter(D2D1_POINT_2F /*mousePosition*/)
{
    ::SetCursor(::LoadCursor(nullptr, IDC_ARROW));
    return S_OK;
}

//
// Event that is fired when the mousewheel is moved
//
HRESULT ImageEditorHandler::OnMouseWheel(D2D1_POINT_2F /*mousePosition*/, short delta, int keys) 
{
    if ((keys & MK_CONTROL) == MK_CONTROL)
    {
        if (delta > 0)
        {
            ZoomIn();
        }
        else
        {
            ZoomOut();
        }
    }

    return S_OK;
}

//
// Event for rendering to the client area. During animation, checks the animation manager
// to see if the client are needs to be invalidated to continue rendering the next frame
//
HRESULT ImageEditorHandler::OnRender()
{
    HRESULT hr = CreateDeviceResources();

    if (!m_animationEnabled)
    {
        DrawClientArea();
        // Update taskbar thumbnail
        hr = UpdateTaskbarThumbnail();
    }
    else
    {
        // Update the animation manager with the current time
        hr = AnimationUtility::UpdateAnimationManagerTime();

        // Draw the client area
        DrawClientArea();

        // Continue drawing as long as there are animations scheduled
        bool isBusy;

        if (SUCCEEDED(AnimationUtility::IsAnimationManagerBusy(&isBusy)))
        {
            if (isBusy)
            {
                InvalidateWindow();
#ifdef _MEASURE_FPS
                m_totalFramesRendered++;
#endif            
            }
            else
            {
                // Cleanup animation objects
                CleanupAnimation();
                // Update taskbar thumbnail
                hr = UpdateTaskbarThumbnail();
                // Save the previous/next images if the user has recently switched images
                if (m_switchingImages)
                {
                    SaveFiles();
                    m_switchingImages = false;
                }
            }
        }
    }

    return hr;
}

//
// Update the taskbar thumbnail after drawing and animation is done
//
HRESULT ImageEditorHandler::UpdateTaskbarThumbnail()
{
    // Don't update the taskbar thumbnail if there are no images. This will be the case while the
    // annotator application is in the middle of initialization and the 'Pictures' or current directory
    // has no pictures.
    if (m_images.empty())
    {
        return S_OK;
    }

    ComPtr<IWindow> window;
    HRESULT hr = GetWindow(&window);
    if (SUCCEEDED(hr))
    {
        HWND hWndChild, hWndParent;
        window->GetWindowHandle(&hWndChild);
        // Get the handle of the top-level window, because taskbar only communicates with top-level window
        window->GetParentWindowHandle(&hWndParent);
        D2D1_RECT_F drawingRect;
        m_images[m_currentIndex].Image->GetTransformedRect(GetCenter(), &drawingRect);
        RECT currentWindowRect, parentWindowRect;
        window->GetRect(&currentWindowRect); // the rectangle dimensions of curent window relative to screen coordinates
        window->GetParentWindowRect(&parentWindowRect); // the rectangle dimensions of curent window's parent window relative to screen coordinates
        // Calculate the rectangle dimentions of current image relative to the main window's coordinates
        RECT rect;
        rect.left = static_cast<long>(drawingRect.left);
        rect.top = static_cast<long>(drawingRect.top) + (currentWindowRect.top - parentWindowRect.top);
        rect.right = rect.left + static_cast<long>(drawingRect.right - drawingRect.left);
        rect.bottom = rect.top + static_cast<long>(drawingRect.bottom - drawingRect.top);
        static Taskbar taskbar(hWndParent);
        // Zoom the image to the thumbnail
        hr = taskbar.SetThumbnailClip(&rect);
        // Update thumbnail toolbar buttons
        if (SUCCEEDED(hr))
        {
            ThumbnailToobarButton backButton = {APPCOMMAND_BROWSER_BACKWARD, m_currentIndex > 0};
            ThumbnailToobarButton nextButton = {APPCOMMAND_BROWSER_FORWARD, m_currentIndex < (int)m_images.size() - 1};
            hr = taskbar.EnableThumbnailToolbarButtons(backButton, nextButton);
        }
    }
    return hr;
}

//
// Event that is called when the corresponding window is resized
//
HRESULT ImageEditorHandler::OnSize(unsigned int width, unsigned int height)
{
    HRESULT hr = S_OK;
    if (m_renderTarget)
    {
        D2D1_SIZE_U size = {width, height};
        hr = m_renderTarget->Resize(size);
    }

    if (SUCCEEDED(hr))
    {
        D2D1_SIZE_F size = m_renderTarget->GetSize();

        // Update boundary rectangle for images
        m_imageBoundaryRect.left = PreviousNextImageMargin + ImageMargin;
        m_imageBoundaryRect.top = ImageMargin;
        m_imageBoundaryRect.right = size.width - PreviousNextImageMargin - ImageMargin;
        m_imageBoundaryRect.bottom = size.height - ImageMargin;

        // Update right gradient brush
        D2D1_POINT_2F startPoint = D2D1::Point2F(size.width - PreviousNextImageMargin, 0);
        D2D1_POINT_2F endPoint = D2D1::Point2F(size.width, 0);
        m_foregroundGradientBrushRight->SetStartPoint(endPoint);
        m_foregroundGradientBrushRight->SetEndPoint(startPoint);

        // Set maximum slide distance
        m_maxSlideDistance = size.width;;

        hr = ManageImageResources();

        if (SUCCEEDED(hr))
        {
            hr = CalculateImagePositions();
        }
    }

    return hr;
}

HRESULT ImageEditorHandler::SetDrawingOperation(__in ImageOperationType imageDrawingOperation)
{
    // Save the previous drawing operation
    m_prevDrawingOperationType = m_currentDrawingOperationType;

    switch (imageDrawingOperation)
    {
    case ImageOperationTypeRotateClockwise:
    case ImageOperationTypeRotateCounterClockwise:
        {
            m_isRotation = true;
            m_currentDrawingOperationType = imageDrawingOperation;
            SetupAnimation();
            break;
        }
    case ImageOperationTypeFlipVertical:
    case ImageOperationTypeFlipHorizontal:
        {
            m_currentDrawingOperationType = imageDrawingOperation;
            m_isFlip = true;
            SetupAnimation();
            break;
        }
    case ImageOperationTypeCrop:
        {
            // flip pen drawing operation based on toggle button input
            if (m_currentDrawingOperationType == ImageOperationTypeCrop)
            {
                m_currentDrawingOperationType = ImageOperationTypeNone;
                m_startClipping = false;
                InvalidateWindow();
            }
            else
            {
                m_currentDrawingOperationType = ImageOperationTypeCrop;
                m_images[m_currentIndex].Image->GetTransformedRect(GetCenter(), &m_currentClipBoundary);
                m_startClipping = true;
                InvalidateWindow();
            }
            break;
        }
    case ImageOperationTypePen:
        {
            // flip pen drawing operation based on toggle button input
            if (m_currentDrawingOperationType == ImageOperationTypePen)
            {
                m_currentDrawingOperationType = ImageOperationTypeNone;
            }
            else
            {
                m_currentDrawingOperationType = ImageOperationTypePen;
            }
            break;
        }
    default:
        {
            m_currentDrawingOperationType = imageDrawingOperation;
        }
    }

    return S_OK;
}

HRESULT ImageEditorHandler::SetPenColor(__in D2D1_COLOR_F penColor)
{
    m_penColor = penColor;
    return S_OK;
}

HRESULT ImageEditorHandler::SetPenSize(__in float penSize)
{
    m_penSize = penSize;
    return S_OK;
}

HRESULT ImageEditorHandler::CanUndo(__out bool* canUndo)
{
    if (nullptr == canUndo)
    {
        return E_POINTER;
    }

    if (m_images.empty())
    {
        *canUndo = false;
        return E_FAIL;
    }

    return m_images[m_currentIndex].Image->CanUndo(canUndo);
}

HRESULT ImageEditorHandler::CanRedo(__out bool* canRedo)
{
    if (nullptr == canRedo)
    {
        return E_POINTER;
    }

    if (m_images.empty())
    {
        *canRedo = false;
        return E_FAIL;
    }

    return m_images[m_currentIndex].Image->CanRedo(canRedo);
}

HRESULT ImageEditorHandler::Undo()
{
    HRESULT hr = m_images[m_currentIndex].Image->UndoImageOperation();

    if (SUCCEEDED(hr))
    {
        InvalidateWindow();
    }

    return hr;
}

HRESULT ImageEditorHandler::Redo()
{
    HRESULT hr = m_images[m_currentIndex].Image->RedoImageOperation();

    if (SUCCEEDED(hr))
    {
        InvalidateWindow();
    }

    return hr;
}

//
// Displays a TaskDialog that shows the user which files failed to save
// 
void ImageEditorHandler::ShowSaveFailure(int imageIndex)
{
    ComPtr<IWindow> window;
    GetWindow(&window);

    HWND hWnd;
    window->GetWindowHandle(&hWnd);

    // Get file name of file that failed to save
    ImageInfo info;
    if (SUCCEEDED(m_images[imageIndex].Image->GetImageInfo(&info)))
    {
        ::TaskDialog(
            hWnd,
            nullptr, 
            MAKEINTRESOURCE(IDS_APP_TITLE),
            MAKEINTRESOURCE(IDS_SAVE_FAILED_TITLE),
            info.fileName.c_str(),
            TDCBF_OK_BUTTON,
            TD_WARNING_ICON, 
            nullptr);
    }

    m_images[imageIndex].Image->DiscardResources();
}

//
// Set the Ribbon UI framework, allowing the editor to update its properties/buttons as needed
// 
HRESULT ImageEditorHandler::SetUIFramework(IUIFramework* framework)
{
    m_framework = framework;

    return S_OK;
}

//
// Return the current drawing operation
// 
HRESULT ImageEditorHandler::GetDrawingOperation(ImageOperationType* imageDrawingOperation)
{
    if (nullptr == imageDrawingOperation)
    {
        return E_POINTER;
    }

    *imageDrawingOperation = m_currentDrawingOperationType;

    return S_OK;
}

//
// Translate any given point (mostly mouse clicks) to an absolute position within
// the currently active image
//
D2D1_POINT_2F ImageEditorHandler::GetAbsolutePosition(D2D1_POINT_2F mousePosition)
{
    // First transform back the point (disregarding current translation and scale)
    mousePosition = RemoveRenderingTransformations(mousePosition);

    // Translate to an absolute point within the image current drawing rect
    D2D1_POINT_2F absPoint;
    m_images[m_currentIndex].Image->TranslateToAbsolutePoint(mousePosition, &absPoint);

    D2D1_RECT_F rect;
    m_images[m_currentIndex].Image->GetDrawingRect(&rect);

    // Scale to actual point relative to the original bitmap
    float scale;
    m_images[m_currentIndex].Image->GetScale(&scale);

    return AdjustToClipRect(
        D2D1::Point2F(
        scale * (absPoint.x - rect.left),
        scale * (absPoint.y - rect.top)));
}

bool ImageEditorHandler::IsImageHit(D2D1_POINT_2F mousePosition)
{
    // First transform back the point (disregarding current translation and scale)
    mousePosition = RemoveRenderingTransformations(mousePosition);

    bool isHit = false;
    m_images[m_currentIndex].Image->ContainsPoint(mousePosition, &isHit);

    return isHit;
}

D2D1_POINT_2F ImageEditorHandler::RemoveRenderingTransformations(D2D1_POINT_2F mousePosition)
{
    mousePosition = D2D1::Matrix3x2F::Translation(-m_currentPanPoint.x, -m_currentPanPoint.y).TransformPoint(mousePosition);
    mousePosition = D2D1::Matrix3x2F::Scale(1/ m_currentZoom, 1 / m_currentZoom, GetCenter()).TransformPoint(mousePosition);

    return mousePosition;
}

D2D1_POINT_2F ImageEditorHandler::AdjustToClipRect(D2D1_POINT_2F absPoint)
{
    D2D1_RECT_F clipRect;
    m_images[m_currentIndex].Image->GetClipRect(&clipRect);

    return D2D1::Point2F(
        std::max(clipRect.left, std::min(clipRect.right, absPoint.x)),
        std::max(clipRect.top, std::min(clipRect.bottom, absPoint.y)));
}

void ImageEditorHandler::UpdateMouseCursor(D2D1_POINT_2F mousePosition)
{
    bool isHit = IsImageHit(mousePosition);

    if (m_currentDrawingOperationType == ImageOperationTypePen)
    {
        if (isHit || m_isDrawing)
        {
            ::SetCursor(::LoadCursor(nullptr, IDC_PEN));
        }
        else
        {
            ::SetCursor(::LoadCursor(nullptr, IDC_ARROW));
        }
    } 
    else if (m_currentDrawingOperationType == ImageOperationTypeCrop)
    {
        if (isHit || m_isClipping)
        {
            ::SetCursor(::LoadCursor(nullptr, IDC_CROSS));
        }
        else
        {
            ::SetCursor(::LoadCursor(nullptr, IDC_ARROW));
        }
    }
}
