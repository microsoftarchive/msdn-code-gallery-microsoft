//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "ImageThumbnailControl.h"
#include "ThumbnailLayoutManager.h"
#include "AsyncLoader\AsyncLoaderItemCache.h"

using namespace Hilo::Direct2DHelpers;

const D2D1_COLOR_F ImageThumbnail::SelectionBorderColor = ColorF(ColorF::DodgerBlue);

ImageThumbnail::ImageThumbnail(ThumbnailInfo thumbnailInfo) : Thumbnail(thumbnailInfo)
{
}

ImageThumbnail::~ImageThumbnail()
{
}

HRESULT ImageThumbnail::Initialize()
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

HRESULT ImageThumbnail::Draw()
{
    if (nullptr == m_renderingParameters.renderTarget)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;
    ComPtr<ID2D1Bitmap> bitmap;

    if (!m_isFullImage)
    {
        hr = GetThumbnail(&bitmap);

        if ((FAILED(hr)) || (nullptr == bitmap))
        {
            hr = CreateBitmap(&bitmap);

            if (SUCCEEDED(hr) && (nullptr != bitmap))
            {
                hr = SetThumbnailIfNull(bitmap);
            }
        }
    }
    else
    {
        if (nullptr == m_fullBitmap )
        {
            hr = CreateBitmap(&bitmap);

            if (SUCCEEDED(hr) && (nullptr != bitmap))
            {
                m_fullBitmap = bitmap;
            }
        }
        else
        {
            bitmap = m_fullBitmap;
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = CreateTextLayout();
    }

    // Draw the image
    if (SUCCEEDED(hr) && nullptr != bitmap)
    {
        D2D1_RECT_F bitmapRect = CalculateBitmapRect(bitmap);

        // Draw shadow
        DrawShadow(bitmapRect);

        // Selectively draw the various thumbnail components
        if (!m_isFullImage)
        {
            DrawSelectionBorder();
        }

        // Draw the bitmap itself
        m_renderingParameters.renderTarget->DrawBitmap(bitmap, bitmapRect);

        // Draw the text caption
        if (!m_isFullImage)
        {
            DrawText();
        }
    }
    else // in case don't have a bitmap yet, or it's failed
    {
        // Just draw the border around the larger rectangle
        DrawShadow(m_rect);
        DrawSelectionBorder();
    }

    return hr;
}

HRESULT ImageThumbnail::CreateBitmap(ID2D1Bitmap** bitmap)
{
    if ( nullptr == bitmap )
    {
        return E_POINTER;
    }

    HRESULT hr;

    if (m_isFullImage)
    {
        hr = CreateFullBitmap(bitmap);
    }
    else
    {
        hr = CreateThumbnailBitmap(bitmap);
    }

    return hr;
}

HRESULT ImageThumbnail::CreateThumbnailBitmap(ID2D1Bitmap** bitmap)
{
    *bitmap = nullptr;

    return LoadBitmapFromShellItem(static_cast<unsigned int>(ThumbnailLayoutManager::MaxThumbnailSize), bitmap);
}

HRESULT ImageThumbnail::CreateFullBitmap(ID2D1Bitmap** bitmap)
{
    *bitmap = nullptr;

    HRESULT hr = S_OK;

    if (nullptr == m_fullBitmap) // If full bitmap was not yet allocated
    {
        if (m_thumbnailInfo.GetFileName().empty())
        {
            wchar_t* path = nullptr;
            hr = m_thumbnailInfo.shellItem->GetDisplayName(SIGDN_FILESYSPATH, &path);
            if (SUCCEEDED(hr) && nullptr != path)
            {
                m_thumbnailInfo.SetFileName(path);
                ::CoTaskMemFree(path);
            }
        }

        if (SUCCEEDED(hr))
        {
            ComPtr<ID2D1Bitmap> newBitmap;
            hr = Direct2DUtility::LoadBitmapFromFile(m_renderingParameters.renderTarget, m_thumbnailInfo.GetFileName().c_str(), 0, 0, &newBitmap);
            if (SUCCEEDED(hr))
            {
                hr = AssignToOutputPointer(bitmap, newBitmap);
            }
        }
    }

    return hr;
}

HRESULT ImageThumbnail::CreateTextLayout()
{
    if (nullptr == m_renderingParameters.textLayout)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;
    if (nullptr == m_textLayout) // If text layout was not yet allocated
    {
        hr = m_dWriteFactory->CreateTextLayout(
            m_thumbnailInfo.title.c_str(),
            static_cast<unsigned int>(m_thumbnailInfo.title.length()),
            m_renderingParameters.textLayout,
            m_renderingParameters.textLayout->GetMaxWidth(),
            m_renderingParameters.textLayout->GetMaxHeight(),
            &m_textLayout);

        if (SUCCEEDED(hr))
        {
            m_textLayout->SetWordWrapping(m_renderingParameters.textLayout->GetWordWrapping());
            m_textLayout->SetParagraphAlignment(m_renderingParameters.textLayout->GetParagraphAlignment());
            m_textLayout->SetTextAlignment(m_renderingParameters.textLayout->GetTextAlignment());
        }

        m_textRange.startPosition = 0;
        m_textRange.length = static_cast<unsigned int>(m_thumbnailInfo.title.length());
    }

    return hr;
}

void ImageThumbnail::DrawShadow(D2D1_RECT_F bitmapRect)
{
    float savedOpacity = m_renderingParameters.solidBrush->GetOpacity();
    D2D1_COLOR_F savedColor = m_renderingParameters.solidBrush->GetColor();

    float width = bitmapRect.right - bitmapRect.left;
    float height = bitmapRect.bottom - bitmapRect.top;
    int numIterations = static_cast<int>(std::min<float>(width * ThumbnailLayoutManager::ShadowSizeRatio, height * ThumbnailLayoutManager::ShadowSizeRatio));

    float radius = 1;
    float opacity = 0;

    for (int i = 1; i <= numIterations; i++)
    {
        opacity = (m_isFullImage ? 0.3f : 0.5f)* (1.0f - sin(0.5f * static_cast<float>(PI) * (i / static_cast<float>(numIterations))));
        m_renderingParameters.solidBrush->SetOpacity(opacity);

        m_renderingParameters.renderTarget->DrawRoundedRectangle(
            RoundedRect(bitmapRect, radius, radius), m_renderingParameters.solidBrush);

        bitmapRect.bottom += 1;
        bitmapRect.right += 1;
        bitmapRect.top -= 1;
        bitmapRect.left -= 1;
        radius += 1;
    }

    // Restore opactity
    m_renderingParameters.solidBrush->SetColor(savedColor);
    m_renderingParameters.solidBrush->SetOpacity(savedOpacity);
}

void ImageThumbnail::DrawText()
{
    float width = m_rect.right - m_rect.left;
    if (width > ThumbnailLayoutManager::MaxThumbnailSizeWithNoText) // Don't draw text at very small size
    {
        float height = m_rect.bottom - m_rect.top;
        float shadowHeight =  std::min<float>(width * ThumbnailLayoutManager::ShadowSizeRatio, height * ThumbnailLayoutManager::ShadowSizeRatio);

        float fontSize;
        m_renderingParameters.textLayout->GetFontSize(0, &fontSize);
        m_textLayout->SetFontSize(fontSize, m_textRange);

        m_textLayout->SetMaxWidth(m_renderingParameters.textLayout->GetMaxWidth());
        m_textLayout->SetMaxHeight(m_renderingParameters.textLayout->GetMaxHeight());

        m_renderingParameters.renderTarget->DrawTextLayout(
            Point2F(m_rect.left, m_rect.bottom + shadowHeight),
            m_textLayout,
            m_renderingParameters.solidBrush, 
            D2D1_DRAW_TEXT_OPTIONS_CLIP);
    }
}

D2D1_RECT_F ImageThumbnail::CalculateBitmapRect(ID2D1Bitmap* bitmap)
{
    D2D1_RECT_F bitmapRect = m_rect;

    float width = m_rect.right - m_rect.left;
    float height = m_rect.bottom - m_rect.top;

    bool isWideBitmap = bitmap->GetSize().width >= bitmap->GetSize().height;

    if (isWideBitmap) // Use the full width
    {
        float bitmapHeight = bitmap->GetSize().height * ( width / bitmap->GetSize().width);

        if (m_isFullImage)
        {
            bitmapRect.top = m_rect.top + 0.5f * ( (height - bitmapHeight) );
        }
        else
        {
            bitmapRect.top = m_rect.top + (height - bitmapHeight);
        }

        bitmapRect.bottom = bitmapRect.top + bitmapHeight;
    }
    else // narrow, use the full height
    {
        float bitmapWidth = bitmap->GetSize().width * ( height / bitmap->GetSize().height);

        bitmapRect.left = m_rect.left + 0.5f * (width - bitmapWidth);
        bitmapRect.right = bitmapRect.left + bitmapWidth;
    }

    return bitmapRect;
}

void ImageThumbnail::DrawSelectionBorder()
{
    static float radius = 3;

    if ((m_selectionState & SelectionStateHoverOn) == SelectionStateHoverOn || (m_selectionState & SelectionStateSelected) == SelectionStateSelected)
    {
        D2D1_RECT_F selectionRect = m_rect;
        float width = m_rect.right - m_rect.left;
        float height = m_rect.bottom - m_rect.top;

        selectionRect.left -= width * ThumbnailLayoutManager::XSelectionBoxDeltaRatio;
        selectionRect.right += width * ThumbnailLayoutManager::XSelectionBoxDeltaRatio;

        float shadowHeight =  std::min<float>(width * ThumbnailLayoutManager::ShadowSizeRatio, height * ThumbnailLayoutManager::ShadowSizeRatio);
        selectionRect.top -= ThumbnailLayoutManager::YSelectionBoxDeltaRatio * height;
        selectionRect.bottom += (ThumbnailLayoutManager::YSelectionBoxDeltaRatio * height) + m_renderingParameters.textLayout->GetMaxHeight() + shadowHeight;

        D2D1_ROUNDED_RECT selectionRoundedRect = D2D1::RoundedRect(selectionRect, radius, radius);

        ComPtr<ID2D1SolidColorBrush> blueBrush;
        m_renderingParameters.renderTarget->CreateSolidColorBrush(SelectionBorderColor, &blueBrush);

        if ((m_selectionState & SelectionStateSelected) == SelectionStateSelected)
        {
            // Draw selection border and rectangle
            blueBrush->SetOpacity(0.3f);
            m_renderingParameters.renderTarget->FillRoundedRectangle(selectionRoundedRect, blueBrush);
            blueBrush->SetOpacity(1.0f);
            m_renderingParameters.renderTarget->DrawRoundedRectangle(selectionRoundedRect, blueBrush);
        }

        if ((m_selectionState & SelectionStateHoverOn) == SelectionStateHoverOn)
        {
            // Draw hover border and rectangle
            blueBrush->SetOpacity(0.2f);
            m_renderingParameters.renderTarget->FillRoundedRectangle(selectionRoundedRect, blueBrush);
            blueBrush->SetOpacity(0.06f);
            m_renderingParameters.renderTarget->DrawRoundedRectangle(selectionRoundedRect, blueBrush);
        }
    }
}

HRESULT ImageThumbnail::LoadBitmapFromShellItem(unsigned int thumbnailSize, ID2D1Bitmap** bitmap)
{
    assert(bitmap);
    *bitmap = nullptr;

    HRESULT hr = S_OK;

    m_thumbnailInfo.fileType = FileTypeImage;

    if (!m_asyncLoadingEnabled)
    {
        std::wstring fileName = m_thumbnailInfo.GetFileName();

        ComPtr<IShellItem> shellItem;
        hr = SHCreateItemFromParsingName(fileName.c_str(), nullptr /*bindctx*/, IID_PPV_ARGS(&shellItem));

        ComPtr<ID2D1Bitmap> newBitmap;
        if (SUCCEEDED(hr))
        {
            hr = Direct2DUtility::DecodeImageFromThumbCache(shellItem, m_renderingParameters.renderTarget, thumbnailSize, &newBitmap);
        }

        if (SUCCEEDED(hr))
        {
            hr = AssignToOutputPointer(bitmap, newBitmap);
        }
    }
    else
    {
        hr = AssignToOutputPointer(bitmap, m_defaultBitmap);
    }

    return hr;
}
