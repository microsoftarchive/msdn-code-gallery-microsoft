//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "SimpleImage.h"
#include "ImageTransformationOperation.h"
#include "ImageClippingOperation.h"

using namespace Hilo::AnimationHelpers;
using namespace Hilo::Direct2DHelpers;

const float SimpleImage::ShadowDepth = 8;

//
// Constructor
//
SimpleImage::SimpleImage(ImageInfo info) :
    m_imageInfo(info),
    m_isHorizontal(true) // by default images are loaded as horizontal
{
}

//
// Destructor
//
SimpleImage::~SimpleImage()
{
}

//
// Retrieves the current rectangle used for drawing this image
//
HRESULT SimpleImage::GetDrawingRect(D2D1_RECT_F* rect)
{
    if (rect == nullptr)
    {
        return E_POINTER;
    }

    (*rect) = m_drawingRect;

    return S_OK;
}


//
// Updates the current rectangle used for drawing this image
//
HRESULT SimpleImage::SetDrawingRect(const D2D1_RECT_F& rect)
{
    m_drawingRect = rect;
    m_originalDrawingRect = rect;
    return S_OK;
}

//
// Retrieves the current crop rectangle of this image
//
HRESULT SimpleImage::GetClipRect(D2D1_RECT_F* rect)
{
    if (rect == nullptr)
    {
        return E_POINTER;
    }

    (*rect) = m_clipRect;

    return S_OK;
}

//
// Retrieves the current ImageInfo struct for this image
HRESULT SimpleImage::GetImageInfo(ImageInfo* info)
{
    if (nullptr == info)
    {
        return E_POINTER;
    }

    (*info) = m_imageInfo;

    return S_OK;
}

//
// Specifies the bounding rectangle used to calculate the drawing rectangle for this image
//
HRESULT SimpleImage::SetBoundingRect(const D2D1_RECT_F& rect)
{
    m_boundingRect = rect;
    CalculateDrawingRect();

    return S_OK;

}

//
// Specifies the current rendering parameters for this image
//
HRESULT SimpleImage::SetRenderingParameters(const RenderingParameters& renderingParameters)
{
    m_renderingParameters = renderingParameters;

    return S_OK;
}

//
// Draws the image using the current drawing rectangle
//
HRESULT SimpleImage::Draw()
{
    HRESULT hr = S_OK;

    if (nullptr == m_bitmap)
    {
        // Load the image if needed
        hr = LoadBitmapFromShellItem();
    }

    if (SUCCEEDED(hr))
    {
        m_currentRenderTarget = m_renderingParameters.renderTarget;
        hr = DrawImage(m_drawingRect, m_clipRect, false);
    }

    return hr;
}

//
// Draws the image or portion of it using the given rectangle
//
HRESULT SimpleImage::DrawImage(const D2D1_RECT_F& drawingRect, const D2D1_RECT_F& imageRect, bool isSaving)
{
    D2D1_MATRIX_3X2_F originalTransform;
    m_currentRenderTarget->GetTransform(&originalTransform);

    D2D1_POINT_2F midPoint = Direct2DUtility::GetMidPoint(drawingRect);
    m_currentRenderTarget->SetTransform(GetTransformationsReversed(midPoint) * originalTransform);

    if (!isSaving)
    {
        DrawShadow(drawingRect);
    }

    m_currentRenderTarget->DrawBitmap(m_bitmap, drawingRect, 1, D2D1_BITMAP_INTERPOLATION_MODE_NEAREST_NEIGHBOR, imageRect);

    // Only draw within the image rectangle
    ComPtr<ID2D1Layer> layer;
    m_currentRenderTarget->CreateLayer(D2D1::SizeF(Direct2DUtility::GetRectWidth(drawingRect), Direct2DUtility::GetRectHeight(drawingRect)), &layer);
    m_currentRenderTarget->PushLayer(D2D1::LayerParameters(drawingRect), layer);

    // The scale is relative to the full bitmap size
    float scale = GetCurrentImageScale();

    for (auto iter = m_imageOperations.begin() ; iter != m_imageOperations.end(); iter++)
    {
        ComPtr<IDrawGeometryOperation> drawOperation;
        if (SUCCEEDED((*iter).QueryInterface(&drawOperation)))
        {
            D2D1_MATRIX_3X2_F transform;
            if (isSaving)
            {
                drawOperation->DiscardResources();

                transform = 
                    D2D1::Matrix3x2F::Translation(-m_clipRect.left, -m_clipRect.top) * 
                    GetTransformationsReversed(midPoint) * 
                    originalTransform;
            }
            else
            {
                transform = 
                    D2D1::Matrix3x2F::Translation(-m_clipRect.left, -m_clipRect.top) * 
                    D2D1::Matrix3x2F::Scale(1 / scale, 1 / scale) * 
                    D2D1::Matrix3x2F::Translation(drawingRect.left, drawingRect.top) * 
                    GetTransformationsReversed(midPoint) * 
                    originalTransform;
            }

            m_currentRenderTarget->SetTransform(transform);
            drawOperation->DrawToRenderTarget(m_currentRenderTarget, drawingRect);

            if (isSaving)
            {
                drawOperation->DiscardResources();
            }
        }
    }

    m_currentRenderTarget->PopLayer();
    m_currentRenderTarget->SetTransform(originalTransform);

    return S_OK;
}

//
// Loads the current image if necessary
//
HRESULT SimpleImage::Load()
{
    return LoadBitmapFromShellItem();
}

//
// Saves the current image
//
HRESULT SimpleImage::Save(__in IShellItem *saveAsItem)
{
    ComPtr<IWICImagingFactory> wicFactory;
    ComPtr<ID2D1Factory> d2dFactory;
    ComPtr<IWICBitmap> wicBitmap;
    ComPtr<ID2D1RenderTarget> wicRenderTarget;

    // Clear backup information from previous save
    m_imageInfo.backupFileName.clear();

    // Don't save if there are no image operations applied to this image unless the user specifed 'Save As'
    if (m_imageOperations.empty() && nullptr == saveAsItem)
    {
        return S_OK;
    }

    HRESULT hr = Direct2DUtility::GetWICFactory(&wicFactory);
    if (SUCCEEDED(hr))
    {
        hr = Direct2DUtility::GetD2DFactory(&d2dFactory);
    }

    // Get the original bitmap rectangle in terms of the current crop
    D2D1_RECT_F originalBitmapRect =
        D2D1::RectF(0, 0, Direct2DUtility::GetRectWidth(m_clipRect), Direct2DUtility::GetRectHeight(m_clipRect));

    // Adjust height and width based on current orientation and clipping rectangle
    float width = m_isHorizontal ? Direct2DUtility::GetRectWidth(m_clipRect) : Direct2DUtility::GetRectHeight(m_clipRect);
    float height = m_isHorizontal ? Direct2DUtility::GetRectHeight(m_clipRect) : Direct2DUtility::GetRectWidth(m_clipRect);

    if (SUCCEEDED(hr))
    {
        // Create WIC bitmap for rendering
        hr = wicFactory->CreateBitmap(
            static_cast<unsigned int>(width),
            static_cast<unsigned int>(height),
            GUID_WICPixelFormat32bppBGR,
            WICBitmapCacheOnLoad,
            &wicBitmap);
    }

    if (SUCCEEDED(hr))
    {
        hr = d2dFactory->CreateWicBitmapRenderTarget(wicBitmap, D2D1::RenderTargetProperties(), &wicRenderTarget);
    }

    if (SUCCEEDED(hr))
    {
        // Replace current bitmap with one that's compatible with the WIC render target
        if (m_bitmap)
        {
            m_bitmap = nullptr;
        }
        
        hr = Direct2DUtility::LoadBitmapFromFile(wicRenderTarget, m_imageInfo.fileName.c_str(), 0, 0, &m_bitmap);
    }

    if (SUCCEEDED(hr))
    {
        // When rotating images make sure that the point around which rotation occurs lines
        // up with the center of the rotated render target
        if (false == m_isHorizontal)
        {
            float offsetX;
            float offsetY;

            if (width > height)
            {
                offsetX = (width - height) / 2;
                offsetY = -offsetX;
            }
            else
            {
                offsetY = (height - width) / 2;
                offsetX = - offsetY;
            }

            D2D1_MATRIX_3X2_F translation = D2D1::Matrix3x2F::Translation(offsetX, offsetY);
            wicRenderTarget->SetTransform(translation);
        }

        // Update current render target to point to WIC render target
        m_currentRenderTarget = wicRenderTarget;

        // Draw updated image to WIC render target
        wicRenderTarget->BeginDraw();
        DrawImage(originalBitmapRect, m_clipRect, true);
        wicRenderTarget->EndDraw();
    }

    if (SUCCEEDED(hr) && !m_imageOperations.empty())
    {
        // Create copy of original image unless the user is simply using 'Save As'
        std::wstring backupPath(m_imageInfo.fileName);
        backupPath.insert(backupPath.find_last_of('\\'), L"\\AnnotatorBackup");

        std::wstring backupDirectory(backupPath.substr(0, backupPath.find_last_of('\\')));

        // Create backup directory if needed
        if (false == ::CreateDirectoryW(backupDirectory.c_str(), nullptr))
        {
            hr = (GetLastError() == ERROR_ALREADY_EXISTS) ? S_OK : E_FAIL;
        }
    
        if (SUCCEEDED(hr))
        {
            // Do not copy if the backup file already exists
            if (false == ::CopyFile(m_imageInfo.fileName.c_str(), backupPath.c_str(), true))
            {
                hr = (GetLastError() == ERROR_FILE_EXISTS) ? S_OK : E_FAIL;
            }
            else
            {
                // Capture name of backup file
                m_imageInfo.backupFileName.assign(backupPath);
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        if (nullptr == saveAsItem)
        {
            // Save updated file
            hr = Direct2DUtility::SaveBitmapToFile(wicBitmap, m_imageInfo.fileName.c_str());
        }
        else
        {
            // Save updated file as the specifed shell item
            wchar_t * saveAsFileName;
            hr = saveAsItem->GetDisplayName(SIGDN_FILESYSPATH, &saveAsFileName);

            if (SUCCEEDED(hr))
            {
                hr = Direct2DUtility::SaveBitmapToFile(wicBitmap, m_imageInfo.fileName.c_str(), saveAsFileName);

                ::CoTaskMemFree(saveAsFileName);
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        // Force image to reload
        hr = DiscardResources();
        m_isHorizontal = true;

        if (nullptr == saveAsItem)
        {
            // Clear all undo operations
            m_imageOperations.clear();

            // Empty redo stack
            while (!m_redoStack.empty())
            {
                m_redoStack.pop();
            }
        }
    }

    return hr;
}

//
// Discards Direct2D resources associated with this image
//
HRESULT SimpleImage::DiscardResources()
{
    for (auto operation = m_imageOperations.begin(); operation != m_imageOperations.end(); operation++)
    {
        ComPtr<IDrawGeometryOperation> drawOperation;
        if (SUCCEEDED((*operation).QueryInterface(&drawOperation)))
        {
            drawOperation->DiscardResources();
        }
    }

    m_bitmap = nullptr;
    m_wicBitmap = nullptr;
    m_currentRenderTarget = nullptr;

    return S_OK;
}

//
// Loads the specified image using the specified shell item
//
HRESULT SimpleImage::LoadBitmapFromShellItem()
{
    HRESULT hr = S_OK;

    if (m_bitmap == nullptr)
    {
        hr = Direct2DUtility::LoadBitmapFromFile(
            m_renderingParameters.renderTarget,
            m_imageInfo.fileName.c_str(),
            0,
            0,
            &m_bitmap);

        if (SUCCEEDED(hr))
        {
            m_clipRect = D2D1::RectF(0, 0, m_bitmap->GetSize().width, m_bitmap->GetSize().height);
        }
    }
    return hr;
}

//
// Calculates the current drawing rectangle based on the orignial image size and the specified boundary rectangle
//
void SimpleImage::CalculateDrawingRect()
{
    // Load our bitmap if necessary
    if (nullptr == m_bitmap)
    {
        if (FAILED(LoadBitmapFromShellItem()))
        {
            return;
        }
    }

    // Calculate bitmap rectangle
    float boundingWidth = m_boundingRect.right - m_boundingRect.left;
    float boundingHeight = m_boundingRect.bottom - m_boundingRect.top;

    float width = Direct2DUtility::GetRectWidth(m_clipRect);
    float height = Direct2DUtility::GetRectHeight(m_clipRect);

    if (!m_isHorizontal)
    {
        // Swap width and height to calculate boundaries
        float widthTemp = width;
        width = height;
        height = widthTemp;
    }

    if (width > boundingWidth)
    {
        // Width is larger than bounding box. Scale width to fit
        float scale = boundingWidth / width;
        width *= scale;
        height *= scale;
    }

    if (height > boundingHeight)
    {
        // Height is larger than bounding box. Scale height to fit
        float scale = boundingHeight / height;
        width *= scale;
        height *= scale;
    }

    if (!m_isHorizontal)
    {
        // Swap width and height to calculate boundaries
        float widthTemp = width;
        width = height;
        height = widthTemp;
    }

    // Update drawing rect
    m_drawingRect.left = m_boundingRect.left + boundingWidth / 2 - width / 2;
    m_drawingRect.top = m_boundingRect.top + boundingHeight / 2 - height / 2;
    m_drawingRect.right = m_drawingRect.left + width;
    m_drawingRect.bottom = m_drawingRect.top + height;
}

//
// Draws a drop shadow around the specified rectangle
//
void SimpleImage::DrawShadow(const D2D1_RECT_F& bitmapRect)
{
    float savedOpacity = m_renderingParameters.solidBrush->GetOpacity();
    D2D1_COLOR_F savedColor = m_renderingParameters.solidBrush->GetColor();

    float opacity = 0.25f;
    float opacityStep = 0.25f / ShadowDepth;

    m_renderingParameters.solidBrush->SetColor(D2D1::ColorF(D2D1::ColorF::Black));

    // First draw the bounding rect
    m_currentRenderTarget->DrawRectangle(bitmapRect, m_renderingParameters.solidBrush);

    for (int i = 0; i < static_cast<int>(ShadowDepth); i++)
    {
        m_renderingParameters.solidBrush->SetOpacity(opacity);

        // Draw right shadow
        m_renderingParameters.renderTarget->DrawLine(
            D2D1::Point2F(bitmapRect.right + i, bitmapRect.top + ShadowDepth),
            D2D1::Point2F(bitmapRect.right + i, bitmapRect.bottom + i),
            m_renderingParameters.solidBrush);

        // Draw bottom shadow
        m_renderingParameters.renderTarget->DrawLine(
            D2D1::Point2F(bitmapRect.left + ShadowDepth , bitmapRect.bottom + i),
            D2D1::Point2F(bitmapRect.right + i, bitmapRect.bottom + i),
            m_renderingParameters.solidBrush);

        opacity -= opacityStep;
    }

    // Restore brush opacity
    m_renderingParameters.solidBrush->SetOpacity(savedOpacity);

    // Restore brush color
    m_renderingParameters.solidBrush->SetColor(savedColor);

}

HRESULT SimpleImage::PushImageOperation(IImageOperation* imageOperation)
{
    if (nullptr == imageOperation)
    {
        return E_INVALIDARG; // a null operation is not allowed
    }

    m_imageOperations.push_back(imageOperation);

    ComPtr<IImageTransformationOperation> transformation;
    ComPtr<IImageClippingOperation> clip;

    if (SUCCEEDED(imageOperation->QueryInterface(IID_PPV_ARGS(&transformation))))
    {
        ImageOperationType transformationType;
        if (SUCCEEDED(transformation->GetTransformationType(&transformationType)) && IsRotation(transformationType))
        {
            m_isHorizontal = !m_isHorizontal;
        }
    }
    else if (SUCCEEDED(imageOperation->QueryInterface(IID_PPV_ARGS(&clip))))
    {
        // Save the current scale
        float scale = Direct2DUtility::GetRectWidth(m_clipRect) / Direct2DUtility::GetRectWidth(m_drawingRect);

        clip->GetClippingRect(&m_clipRect);
        m_clipRect = Direct2DUtility::FixRect(m_clipRect);

        // Save current clip rect
        clip->SetClippingRect(m_clipRect);

        m_drawingRect.left = m_drawingRect.left + 0.5f * (Direct2DUtility::GetRectWidth(m_drawingRect) - (Direct2DUtility::GetRectWidth(m_clipRect) / scale));
        m_drawingRect.right = m_drawingRect.left + Direct2DUtility::GetRectWidth(m_clipRect) / scale;
        
        m_drawingRect.top = m_drawingRect.top + 0.5f * (Direct2DUtility::GetRectHeight(m_drawingRect) - (Direct2DUtility::GetRectHeight(m_clipRect) / scale));
        m_drawingRect.bottom = m_drawingRect.top + Direct2DUtility::GetRectHeight(m_clipRect) / scale;

        // Save new drawing rect
        clip->SetDrawingRect(m_drawingRect);
    }

    // Since we can't redo anymore, empty redo stack
    while (!m_redoStack.empty())
    {
        m_redoStack.pop();
    }

    return S_OK;
}

HRESULT SimpleImage::ContainsPoint(D2D1_POINT_2F point, bool *doesImageContainPoint)
{    
    if (nullptr == doesImageContainPoint)
    {
        return E_POINTER;
    }

    if (nullptr == m_renderingParameters.renderTarget)
    {
        return E_FAIL;
    }

    *doesImageContainPoint = Direct2DUtility::HitTest(GetTransformedRect(Direct2DUtility::GetMidPoint(m_drawingRect), m_drawingRect), point);

    return S_OK;
}

HRESULT SimpleImage::TranslateToAbsolutePoint(D2D1_POINT_2F point, D2D1_POINT_2F *translatedPoint)
{
    if (nullptr == translatedPoint)
    {
        return E_POINTER;
    }

    D2D1::Matrix3x2F invertedMatrix = GetInverseTransformations(Direct2DUtility::GetMidPoint(m_drawingRect));
    *translatedPoint = invertedMatrix.TransformPoint(point);

    (*translatedPoint).x += m_clipRect.left / GetCurrentImageScale();
    (*translatedPoint).y += m_clipRect.top  / GetCurrentImageScale();

    return S_OK;
}

HRESULT SimpleImage::CanUndo(__out bool* canUndo)
{
    if (nullptr == canUndo)
    {
        return E_POINTER;
    }
    
    (*canUndo) = !m_imageOperations.empty();

    return S_OK;
}

HRESULT SimpleImage::CanRedo(__out bool* canRedo)
{
    if (nullptr == canRedo)
    {
        return E_POINTER;
    }
    
    (*canRedo) = !m_redoStack.empty();

    return S_OK;
}

HRESULT SimpleImage::UndoImageOperation()
{
    if (m_imageOperations.empty())
    {
        return E_FAIL;
    }
    
    ComPtr<IImageOperation> operation = m_imageOperations.back();
    m_imageOperations.pop_back();
    m_redoStack.push(operation);

    if (IsRotationOperation(operation))
    {
        m_isHorizontal = !m_isHorizontal;
    }

    RecalculateClipRect();

    return S_OK;
}

HRESULT SimpleImage::RedoImageOperation()
{
    if (m_redoStack.empty())
    {
        return E_FAIL;
    }
    
    ComPtr<IImageOperation> operation = m_redoStack.top();
    m_redoStack.pop();
    m_imageOperations.push_back(operation);

    if (IsRotationOperation(operation))
    {
        m_isHorizontal = !m_isHorizontal;
    }

    RecalculateClipRect();

    return S_OK;
}

bool SimpleImage::IsRotationOperation(IImageOperation* operation)
{
    bool isRotation = false;
    ComPtr<IImageTransformationOperation> transformation;

    if (SUCCEEDED(operation->QueryInterface(IID_PPV_ARGS(&transformation))))
    {
        ImageOperationType transformationType;
        if (SUCCEEDED(transformation->GetTransformationType(&transformationType)) && IsRotation(transformationType))
        {
            isRotation = true;
        }
    }

    return isRotation;
}

//
// Get the transofmrations of this image in reverese order, up to a given index 
// and using a given midPoint
//
D2D1::Matrix3x2F SimpleImage::GetTransformationsReversed(D2D1_POINT_2F midPoint, int upTo)
{
    D2D1::Matrix3x2F transform = D2D1::Matrix3x2F::Identity();

    for (int i = static_cast<int>(m_imageOperations.size()) - 1; i >= upTo ; i--)
    {
        ComPtr<IImageTransformationOperation> transformOperation;
        if (SUCCEEDED(m_imageOperations[i].QueryInterface(&transformOperation)))
        {
            D2D1::Matrix3x2F operationTransform;
            transformOperation->GetTransformationMatrix(midPoint, &operationTransform);
            transform = operationTransform * transform;
        }
    }

    return transform;
}

//
// Get the inverse of the transformations of this image, allowing to trasnform
// a given point to its original locaiton in a geometry
//
D2D1::Matrix3x2F SimpleImage::GetInverseTransformations(D2D1_POINT_2F midPoint)
{
    D2D1::Matrix3x2F transform = D2D1::Matrix3x2F::Identity();

    for (int i = 0 ; i < static_cast<int>(m_imageOperations.size()) ; i++)
    {
        ComPtr<IImageTransformationOperation> transformOperation;
        if (SUCCEEDED(m_imageOperations[i].QueryInterface(&transformOperation)))
        {
            D2D1::Matrix3x2F operationTransform;
            transformOperation->GetInverseTransformationMatrix(midPoint, &operationTransform);
            transform = operationTransform * transform;
        }
    }

    return transform;
}

HRESULT SimpleImage::GetTransformedRect(D2D1_POINT_2F midPoint, __out D2D1_RECT_F* rect)
{
    if (nullptr == rect)
    {
        return E_POINTER;
    }

    *rect = GetTransformedRect(midPoint, m_drawingRect);

    return S_OK;
}

D2D1_RECT_F SimpleImage::GetTransformedRect(D2D1_POINT_2F midPoint, const D2D1_RECT_F& rect)
{
    D2D1::Matrix3x2F transform = GetTransformationsReversed(midPoint);

    D2D1_POINT_2F upperLeft = transform.TransformPoint(D2D1::Point2F(rect.left, rect.top));
    D2D1_POINT_2F lowerRight = transform.TransformPoint(D2D1::Point2F(rect.right, rect.bottom));

    return Direct2DUtility::FixRect(D2D1::RectF(upperLeft.x, upperLeft.y, lowerRight.x, lowerRight.y));
}

void SimpleImage::RecalculateClipRect()
{
    m_clipRect = D2D1::RectF(0, 0, m_bitmap->GetSize().width, m_bitmap->GetSize().height); 
    m_drawingRect = m_originalDrawingRect;

    for (int i = static_cast<int>(m_imageOperations.size()) - 1; i >= 0 ; i--)
    {
        ComPtr<IImageClippingOperation> clip;
        if (SUCCEEDED(m_imageOperations[i].QueryInterface(&clip)))
        {
            clip->GetClippingRect(&m_clipRect);
            clip->GetDrawingRect(&m_drawingRect);
            // We need only the last clip
            break;
        }
    }
}

HRESULT SimpleImage::GetScale(float* scale)
{
    if (scale == nullptr)
    {
        return E_POINTER;
    }

    *scale = GetCurrentImageScale();

    return S_OK;
}