//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include <wrl.h>
#include <d3d11_1.h>
#include <d2d1_1.h>
#include <d2d1effects.h>
#include <d2d1_1helper.h>
#include <dwrite_1.h>
#include <wincodec.h>
#include <agile.h>
#define _USE_MATH_DEFINES
#include <math.h>
#include <strsafe.h>

template <class Interface>
inline void
SafeRelease(
    Interface **ppInterfaceToRelease
    )
{
    if (*ppInterfaceToRelease != nullptr)
    {
        (*ppInterfaceToRelease)->Release();

        (*ppInterfaceToRelease) = nullptr;
    }
}

template <class DestInterface, class SourceInterace>
inline void
SafeReplace(
    DestInterface **ppDestInterface,
    SourceInterace *pSourceInterface
    )
{
    if (*ppDestInterface != nullptr)
    {
        (*ppDestInterface)->Release();
    }

    *ppDestInterface = pSourceInterface;

    if (pSourceInterface)
    {
        (*ppDestInterface)->AddRef();
    }
}