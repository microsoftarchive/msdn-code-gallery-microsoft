//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include "stdafx.h"

//
// Provides basic information about a given image such as corresponding IShellItem, filename, and title
//
struct ImageInfo
{
public:
    ComPtr<IShellItem> shellItem;
    std::wstring title;
    std::wstring fileName;
    std::wstring backupFileName;

    ImageInfo(IShellItem* shellItemPtr = nullptr) : shellItem(shellItemPtr)
    {
        if (shellItemPtr != nullptr)
        {
            wchar_t* name;
            
            HRESULT hr = shellItemPtr->GetDisplayName(SIGDN_FILESYSPATH, &name);
            if (SUCCEEDED(hr))
            {
                fileName = name;
                title = name;
                ::CoTaskMemFree(name);
            }

            hr = shellItemPtr->GetDisplayName(SIGDN_NORMALDISPLAY, &name);
            if (SUCCEEDED(hr))
            {
                title = name;
                ::CoTaskMemFree(name);
            }
        }
    }
};

//
// Collection of Direct2D resources used in drawing thumbnails
//
struct RenderingParameters
{
    ComPtr<ID2D1HwndRenderTarget> renderTarget;
    ComPtr<ID2D1SolidColorBrush> solidBrush;
};

/// <summary>
/// The interface representing a thumbnail control
/// </summary>
[uuid("B8DE25F6-70CF-4ECA-B9B8-24E9100DDF4A")]
__interface IImage : IUnknown
{
    HRESULT __stdcall Draw();
    HRESULT __stdcall Load();
    HRESULT __stdcall Save(__in IShellItem *saveAsItem = nullptr);

    // Getters and setters
    HRESULT __stdcall GetDrawingRect(__out D2D1_RECT_F* rect);
    HRESULT __stdcall SetDrawingRect(__in const D2D1_RECT_F& rect);
    HRESULT __stdcall GetTransformedRect(D2D1_POINT_2F midPoint, __out D2D1_RECT_F* rect);
    HRESULT __stdcall GetImageInfo(__out ImageInfo* info);
    HRESULT __stdcall SetBoundingRect(__in const D2D1_RECT_F& rect);
    HRESULT __stdcall SetRenderingParameters(__in const RenderingParameters& drawingObjects);
    HRESULT __stdcall GetScale(__out float* scale);
    HRESULT __stdcall GetClipRect(__out D2D1_RECT_F* rect);
    
    HRESULT __stdcall ContainsPoint(__in D2D1_POINT_2F point, __out bool* doesImageContainPoint);
    HRESULT __stdcall TranslateToAbsolutePoint(__in D2D1_POINT_2F point, __out D2D1_POINT_2F *translatedPoint);

    HRESULT __stdcall CanUndo(__out bool* canUndo);
    HRESULT __stdcall CanRedo(__out bool* canRedo);
    HRESULT __stdcall UndoImageOperation();
    HRESULT __stdcall RedoImageOperation();

    HRESULT __stdcall PushImageOperation(__in IImageOperation* imageOperation);

    // Resource management
    HRESULT __stdcall DiscardResources();
};
