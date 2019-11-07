//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// pch.cpp
// Include the standard header and generate the precompiled header.
//

#include "pch.h"
#include <robuffer.h>
#include <wrl\client.h>

using namespace Microsoft::WRL;
using namespace Windows::Storage::Streams;

byte* GetArrayFromBuffer(Windows::Storage::Streams::IBuffer^ buffer)
{
    ComPtr<IInspectable> base = reinterpret_cast<IInspectable*>(buffer);
    ComPtr<IBufferByteAccess> access;

    auto hr = base.As(&access);

    if (FAILED(hr))
    {
        throw Platform::Exception::CreateException(hr);
    }

    byte* data;

    hr = access->Buffer(&data);

    if (FAILED(hr))
    {
        throw Platform::Exception::CreateException(hr);
    }

    // The returned buffer is valid as long as the IBuffer passed in
    // remains valid.
    return data;
}