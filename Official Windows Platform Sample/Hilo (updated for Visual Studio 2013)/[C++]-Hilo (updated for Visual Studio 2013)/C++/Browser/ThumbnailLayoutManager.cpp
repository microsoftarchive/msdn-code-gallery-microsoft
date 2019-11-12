//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "ThumbnailLayoutManager.h"

//
// Initialization of constant values
//
const float ThumbnailLayoutManager::DefaultThumbnailSize = 128.0f;
const float ThumbnailLayoutManager::MaxThumbnailSize = 256.0f;
const float ThumbnailLayoutManager::MinThumbnailSize = 48.0f;
const float ThumbnailLayoutManager::MaxThumbnailSizeWithNoText = 64.0f;
const float ThumbnailLayoutManager::MinSwitchImageDistance = 5.0f;
const float ThumbnailLayoutManager::XGapRatio = 0.15f;
const float ThumbnailLayoutManager::YGapRatio = 0.10f;
const float ThumbnailLayoutManager::XSelectionBoxDeltaRatio = 0.04f;
const float ThumbnailLayoutManager::YSelectionBoxDeltaRatio = 0.04f;
const float ThumbnailLayoutManager::ShadowSizeRatio = 0.05f;

//
// Constructor
//
ThumbnailLayoutManager::ThumbnailLayoutManager(Hilo::AsyncLoader::IAsyncLoaderHelper *asyncLoader):
    m_isSlideShow(false),
    m_slideShowCurrentPanPositionX(0),
    m_slideShowMaxThumbnailSize(MaxThumbnailSize),
    m_slideShowZoomFactor(1),
    m_previousThumbnailSize(DefaultThumbnailSize),
    m_asyncLoaderHelper(asyncLoader),
    m_renderTargetSize(D2D1::SizeF(0, 0)),
    m_renderingStartPoint(D2D1::Point2F(0, 0)),
    m_imageCount(0),
    m_rowCount(1),
    m_columnCount(1),
    m_renderingStartImageIndex(0),
    m_renderingEndImageIndex(0),
    m_thumbnailSize(DefaultThumbnailSize),
    m_textBoxHeight(0),
    m_xGap(0),
    m_yGap(0),
    m_imageGridOffsetX(0),
    m_imageGridOffsetY(0),
    m_arrowGutterSize(0),
    m_currentPanPositionX(0),
    m_panBoundaryLeft(0),
    m_panBoundaryRight(0)
{
}

//
// Destructor
//
ThumbnailLayoutManager::~ThumbnailLayoutManager()
{
}

//
// Retrieves information about which pan states are available.
//
HRESULT ThumbnailLayoutManager::GetPanState(int *panState)
{
    assert(panState);
    *panState = NoPanning;

    if (m_currentPanPositionX < (m_panBoundaryLeft - m_arrowGutterSize))
    {
        (*panState) |= PanLeft;
    }

    if ((m_currentPanPositionX - m_arrowGutterSize) > m_panBoundaryRight)
    {
        (*panState) |= PanRight;
    }

    return S_OK;
}

//
// Retreives the distance needed to pan in order to get the current image centered
// in the display. This method should only be used in slide show mode when the user
// finishes a pan gesture
//
HRESULT ThumbnailLayoutManager::EndSlideShowPan(float *distance)
{
    // Determine direction to pan based on delta between begin/end pan
    float directionDelta = m_slideShowCurrentPanPositionX - m_currentPanPositionX;

    // Determine direction and distance to pan
    if (directionDelta < -MinSwitchImageDistance)
    {
        // Pan left
        *distance = -m_renderTargetSize.width - directionDelta;
    }
    else if (directionDelta > MinSwitchImageDistance)
    {
        // Pan right
        *distance = m_renderTargetSize.width - directionDelta;
    }
    else
    {
        // No panning
        *distance = directionDelta;
    }

    return S_OK;
}

//
// Retrieves the current thumbnail size
//
HRESULT ThumbnailLayoutManager::GetThumbnailSize(float* thumbnailSize)
{
    assert(thumbnailSize);
    *thumbnailSize = m_thumbnailSize;

    return S_OK;
}

//
// Retrieves the currently visible range of thumbnail cells. It is important to precede this call
// with at least one call to UpdateVisibleThumbnailControls so that the layout manager can determine which
// thumbnails are visible and update the drawing rectangle for each of those cells
//
HRESULT ThumbnailLayoutManager::GetVisibleThumbnailRange(int *rangeStart, int *rangeEnd)
{
    if (nullptr != rangeStart)
    {
        *rangeStart = m_renderingStartImageIndex;
    }

    if (nullptr != rangeEnd)
    {
        *rangeEnd = m_renderingEndImageIndex;
    }

    return S_OK;
}

//
// Sets the width of the arrow gutter
//
HRESULT ThumbnailLayoutManager::SetArrowGutterSize(float arrowGutterSize)
{
    m_arrowGutterSize = arrowGutterSize;
    return S_OK;
}

//
// Calculates the best pan position to center the specified image in the viewing area.
//
HRESULT ThumbnailLayoutManager::SetCurrentImage(unsigned int currentImageIndex)
{
    // Calculate delta between current pan position and specified index
    float targetPositionX = m_panBoundaryLeft - (currentImageIndex / m_rowCount) * m_imageGridOffsetX;
    float delta = m_currentPanPositionX - targetPositionX;

    // Pan based on the specified delta
    Pan(-delta);

    return S_OK;
}

//
// Sets the current render target size
//
HRESULT ThumbnailLayoutManager::SetRenderTargetSize(D2D1_SIZE_F renderTargetSize)
{
    m_renderTargetSize = renderTargetSize;

    if (m_isSlideShow)
    {
        int currentImageIndex = m_renderingStartImageIndex;
        SetSlideShowMode(m_isSlideShow);
        SetCurrentImage(currentImageIndex);
    }

    // Recalculate layout items that are dependent on the render target size
    UpdateLayout();

    return S_OK;
}

//
// Sets the maximum text box height
//
HRESULT ThumbnailLayoutManager::SetTextBoxHeight(float textBoxHeight)
{
    m_textBoxHeight = textBoxHeight;
    return S_OK;
}

//
// Sets whether or not the layout manager is in slideshow mode
//
HRESULT ThumbnailLayoutManager::SetSlideShowMode(bool isSlideShow)
{
    // Store the previous thumbnail size if switching to slide show mode.
    if (isSlideShow && !m_isSlideShow)
    {
        m_previousThumbnailSize = m_thumbnailSize;
        m_slideShowZoomFactor = 1;
    }

    // Set slide show mode on the layout manager
    m_isSlideShow = isSlideShow;

    if (isSlideShow)
    {
        m_slideShowMaxThumbnailSize = std::min(m_renderTargetSize.width - m_arrowGutterSize * 2, m_renderTargetSize.height * (1.0f - ShadowSizeRatio));
        m_thumbnailSize = m_slideShowMaxThumbnailSize * m_slideShowZoomFactor;
    }
    else
    {
        m_thumbnailSize = m_previousThumbnailSize;
    }

    UpdateLayout();

    // Reset panning for current image
    m_currentPanPositionX = m_panBoundaryLeft;
    Pan(0.0f);

    return S_OK;
}

//
// Locks in the current image index so that the layout manager can correctly calculate the distance
// to pan when panning ends
//
HRESULT ThumbnailLayoutManager::BeginSlideShowPan()
{
    m_slideShowCurrentPanPositionX = m_currentPanPositionX;
    return S_OK;
}


//
// Creates a thumbnail layout based on the specified vector of images using the current render target size and specified
// textbox height.
//
HRESULT ThumbnailLayoutManager::CreateLayout(int imageCount, bool resetPanPosition)
{
    // Capture image count
    m_imageCount = imageCount;

    // Update layout based on current render size
    UpdateLayout();

    // Reset panning to initial value
    if (resetPanPosition)
    {
        m_currentPanPositionX = m_panBoundaryLeft;
    }

    return S_OK;
}

//
// Updates the layout for thumbnails that are currently visible based on the current render target size and pan position
// this method should only be called when it is expected that the layout will be changed (after zoom and panning).
//
HRESULT ThumbnailLayoutManager::UpdateVisibleThumbnailControls(const std::vector<ComPtr<IThumbnail>> &cells)
{
    int index = m_renderingStartImageIndex;

    D2D1_RECT_F drawingRect;
    drawingRect.left = m_renderingStartPoint.x;
    drawingRect.top = m_renderingStartPoint.y;
    drawingRect.right = drawingRect.left + m_thumbnailSize;
    drawingRect.bottom = drawingRect.top + m_thumbnailSize;

    for (int column = 0; drawingRect.left < m_renderTargetSize.width && index < static_cast<int>(cells.size()); column++)
    {
        for (int row = 0; row < m_rowCount && index < static_cast<int>(cells.size()); row++, index++)
        {
            cells[index]->SetRect(drawingRect);
            cells[index]->SetIsFullImage(m_isSlideShow);

            drawingRect.top += m_imageGridOffsetY;
            drawingRect.bottom = drawingRect.top + m_thumbnailSize;
        }

        drawingRect.left += m_imageGridOffsetX;
        drawingRect.right = drawingRect.left + m_thumbnailSize;
        drawingRect.top = m_renderingStartPoint.y;
        drawingRect.bottom = drawingRect.top + m_thumbnailSize;
    }

    m_renderingEndImageIndex = index;

    return S_OK;
}

//
// Updates the current pan position and calculates values that are based on the pan position
//
HRESULT ThumbnailLayoutManager::Pan(float offset)
{
    m_currentPanPositionX += offset;

    if (m_currentPanPositionX > m_panBoundaryLeft)
    {
        m_currentPanPositionX = m_panBoundaryLeft;
    }
    else if (m_currentPanPositionX < m_panBoundaryRight)
    {
        m_currentPanPositionX = m_panBoundaryRight;
    }

    // Calculate index of first visible image
    if (m_panBoundaryLeft == m_panBoundaryRight)
    {
        m_renderingStartImageIndex = 0;
    }
    else
    {
        m_renderingStartImageIndex = m_rowCount * (
            static_cast<int>(std::abs(m_currentPanPositionX - m_panBoundaryLeft)) /
            static_cast<int>(m_imageGridOffsetX));
    }

    // Calculate the starting point to render images
    m_renderingStartPoint.x = m_currentPanPositionX + (m_renderingStartImageIndex / m_rowCount) * m_imageGridOffsetX;

    if (m_isSlideShow)
    {
        m_renderingStartPoint.y = m_renderTargetSize.height / 2 - m_thumbnailSize / 2;
    }
    else
    {
        m_renderingStartPoint.y = ShadowSizeRatio * m_thumbnailSize;
    }

    // Update asynchrouse loader with current start index
    if (m_renderingStartImageIndex != m_previousRenderingStartImageIndex)
    {
        m_asyncLoaderHelper->SetCurrentPagePivot(m_renderingStartImageIndex);
        m_previousRenderingStartImageIndex = m_renderingStartImageIndex;
    }

    return S_OK;
}

//
// Update the current thumbnail size based on the specified zoom factor
//
HRESULT ThumbnailLayoutManager::Zoom(float factor)
{
    HRESULT hr = S_OK;

    m_thumbnailSize *= factor;

    if (m_isSlideShow)
    {
        m_slideShowZoomFactor *= factor;

        if (m_slideShowZoomFactor > 1.0)
        {
            m_slideShowZoomFactor = 1.0;
        }
        else if (m_slideShowZoomFactor < 0.5)
        {
            m_slideShowZoomFactor = 0.5;
            hr = S_FALSE;
        }

        int currentIndex = m_renderingStartImageIndex;
        SetSlideShowMode(m_isSlideShow);
        SetCurrentImage(currentIndex);
    }
    else
    {
        if (m_thumbnailSize > MaxThumbnailSize)
        {
            m_thumbnailSize = MaxThumbnailSize;
        }
        else if (m_thumbnailSize < MinThumbnailSize)
        {
            m_thumbnailSize = MinThumbnailSize;
        }

        UpdateLayout();
    }

    return hr;
}

//
// Updates the value of all layout elements
//
void ThumbnailLayoutManager::UpdateLayout()
{
    if (m_isSlideShow)
    {
        m_xGap = m_renderTargetSize.width;
        m_yGap = 0;
        m_imageGridOffsetX = m_renderTargetSize.width;
        m_imageGridOffsetY = 0;
        m_rowCount = 1;
    }
    else
    {
        // Calculate gaps between images in the layout
        m_xGap = m_thumbnailSize * XGapRatio;
        m_yGap = m_thumbnailSize * (YGapRatio + ShadowSizeRatio) + m_textBoxHeight;

        // Update image layout information
        m_imageGridOffsetX = m_thumbnailSize + m_xGap;
        m_imageGridOffsetY = m_thumbnailSize + m_yGap;

        // Calculate number of rows per column
        m_rowCount = std::max(1, static_cast<int>(m_renderTargetSize.height / m_imageGridOffsetY));
    }

    // Determine the number of columns
    m_columnCount = static_cast<int>(ceil(static_cast<float>(m_imageCount) / static_cast<float>(m_rowCount)));

    // Calculate total width of the area that images will occupy
    float totalWidth = m_columnCount * m_imageGridOffsetX - m_xGap;

    // Update pan boundaries based on layout objects
    if (totalWidth + m_arrowGutterSize * 2 < m_renderTargetSize.width)
    {
        m_panBoundaryLeft = m_renderTargetSize.width / 2 - totalWidth / 2;
        m_panBoundaryRight = m_panBoundaryLeft;
    }
    else
    {
        if (m_isSlideShow)
        {
            m_panBoundaryLeft = m_renderTargetSize.width / 2 - m_thumbnailSize / 2;
            m_panBoundaryRight = m_panBoundaryLeft - totalWidth;
        }
        else
        {
            m_panBoundaryLeft = m_arrowGutterSize;
            m_panBoundaryRight = std::min(m_panBoundaryLeft, -totalWidth + m_renderTargetSize.width - m_arrowGutterSize);
        }
    }

    // Simulate pan in order to update calculations based on panning
    Pan(0.0f);

    // Update asynchrous loader helper
    int itemCount = static_cast<int>(m_renderTargetSize.width / m_imageGridOffsetX * m_rowCount);

    if (itemCount != m_previousItemCount)
    {
        m_asyncLoaderHelper->SetPageItemCount(itemCount);
        m_previousItemCount = itemCount;
    }
}