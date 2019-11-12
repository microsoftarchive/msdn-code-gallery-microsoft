// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once

ref class DirectXVideoTransform abstract
{
internal:
    // Called when resources should be invalidated
    virtual void Invalidate() = 0;
    
    // Called to create resources
    virtual void Initialize(ID3D11Device *pDevice, UINT uiWidth, UINT uiHeight) = 0;

    // Called to process video data
    // pInput and pOutput are both DXGI_FORMAT_B8G8R8X8_UNORM textures with uiWidth,uiHeight as given by Initialize
    // uiInIndex and uiOutIndex are the subresource indices
    virtual void ProcessFrame(ID3D11Device *pDevice, ID3D11Texture2D *pInput, UINT uiInIndex, ID3D11Texture2D *pOutput, UINT uiOutIndex) = 0;
};
