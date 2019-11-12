//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include "pch.h"

namespace DX
{
    inline void ThrowIfFailed(HRESULT hr)
    {
        if (FAILED(hr))
        {            
            throw Platform::Exception::CreateException(hr);
        }
    }

    inline D2D1_COLOR_F ConvertToColorF(Windows::UI::Color color)
    {
        return D2D1::ColorF(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);            
    }

    inline D2D1_RECT_F ConvertToRectF(Windows::Foundation::Rect source)
    {
        return D2D1::RectF(source.Left, source.Top, source.Right, source.Bottom);
    }
}
