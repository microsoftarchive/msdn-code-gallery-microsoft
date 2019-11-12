//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "CarouselPane.h"
#include "CarouselThumbnail.h"
#include "AsyncLoader\AsyncLoaderItemCache.h"

using namespace Hilo::Direct2DHelpers;
using namespace Hilo::WindowApiHelpers;

CarouselThumbnail::CarouselThumbnail(ThumbnailInfo thumbnailInfo) : Thumbnail(thumbnailInfo)
{
}

CarouselThumbnail::~CarouselThumbnail()
{
}

HRESULT CarouselThumbnail::Initialize()
{
    HRESULT hr = Direct2DUtility::GetDWriteFactory(&m_dWriteFactory);
    if (SUCCEEDED(hr))
    {
        hr = SharedObject<AsyncLoaderItemCache>::Create(&m_asyncLoaderItemCache);
    }

    if (SUCCEEDED(hr))
    {
        m_asyncLoaderItemCache->SetLoadableItem(this);
    }

    return hr;
}

HRESULT CarouselThumbnail::Draw()
{
    // Get the latest bitmap
    ComPtr<ID2D1Bitmap> bitmap;
    HRESULT hr = GetThumbnail(&bitmap);

    // Create a bitmap if needed
    if ((FAILED(hr)) || (nullptr == bitmap))
    {
        hr = CreateBitmap(&bitmap);

        if (SUCCEEDED(hr) && (nullptr != bitmap))
        {
            hr = SetThumbnailIfNull(bitmap);
        }
    }

    // Draw the image
    if (SUCCEEDED(hr))
    {
        if (m_selectionState == SelectionStateHoverOn)
        {
            // Update the size if this item is currently selected
            m_rect.left -= 5;
            m_rect.top -= 10;
            m_rect.right += 5;
        }

        m_renderingParameters.renderTarget->DrawBitmap(bitmap, m_rect, m_opacity);

        if (m_selectionState == SelectionStateHoverOn)
        {
            // Revert to previous size
            m_rect.left += 5;
            m_rect.top += 10;
            m_rect.right -= 5;
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = CreateTextLayout();
    }

    // Calculate actual text height
    DWRITE_TEXT_METRICS textMetrics = { };

    // Font size affects current line size
    m_textLayout->SetFontSize(m_fontSize, m_textRange);

    if (SUCCEEDED(hr))
    {
        hr = m_textLayout->GetMetrics(&textMetrics);
    }

    if (SUCCEEDED(hr) && m_opacity > 0.7f)
    {
        // Update opacity
        m_renderingParameters.solidBrush->SetOpacity(m_opacity);

        // Set maximum width and height
        m_textLayout->SetMaxWidth(m_rect.right - m_rect.left);

        // Only allow two lines of text to be displayed
        if (textMetrics.lineCount > 2)
        {
            m_textLayout->SetMaxHeight(2 * textMetrics.height / textMetrics.lineCount);
        }
        else
        {
            m_textLayout->SetMaxHeight(textMetrics.height);
        }

        // Draw text
        m_renderingParameters.renderTarget->DrawTextLayout(
            D2D1::Point2F(m_rect.left, m_rect.bottom),
            m_textLayout,
            m_renderingParameters.solidBrush,
            D2D1_DRAW_TEXT_OPTIONS_CLIP);
    }

    return hr;
}

HRESULT CarouselThumbnail::CreateBitmap(ID2D1Bitmap** bitmap)
{
    // Check for valid out parameter
    if ( nullptr == bitmap )
    {
        return E_POINTER;
    }

    return LoadBitmapFromShellItem(static_cast<unsigned int>(CarouselPaneMessageHandler::MaxThumbnailSize), bitmap);
}

HRESULT CarouselThumbnail::CreateTextLayout()
{
    HRESULT hr = S_OK;

    // Set the text alignment
    m_renderingParameters.textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);

    if (nullptr == m_textLayout)
    {
        hr = m_dWriteFactory->CreateTextLayout(
            m_thumbnailInfo.title.c_str(),
            static_cast<unsigned int>(m_thumbnailInfo.title.length()),
            m_renderingParameters.textFormat,
            m_rect.right - m_rect.left,
            16,
            &m_textLayout);
    }

    // Set the text range to the entire caption
    m_textRange.startPosition = 0;
    m_textRange.length = static_cast<unsigned int>(m_thumbnailInfo.title.length());

    return hr;
}

HRESULT CarouselThumbnail::LoadBitmapFromShellItem(unsigned int thumbnailSize, ID2D1Bitmap** bitmap)
{
    *bitmap = nullptr;

    HRESULT hr = S_OK;

    m_thumbnailInfo.fileType = FileTypeFolder;

    if (!m_asyncLoadingEnabled )
    {
        ComPtr<ID2D1Bitmap> newBitmap;
        hr = Direct2DUtility::DecodeImageFromThumbCache(m_thumbnailInfo.shellItem, m_renderingParameters.renderTarget, thumbnailSize, &newBitmap);

        if (SUCCEEDED(hr) && (nullptr != newBitmap))
        {
            hr = AssignToOutputPointer(bitmap, newBitmap);
        }
    }
    else
    {
        if (nullptr != m_defaultBitmap)
        {
            hr = AssignToOutputPointer(bitmap, m_defaultBitmap);
        }
    }

    return hr;
}
