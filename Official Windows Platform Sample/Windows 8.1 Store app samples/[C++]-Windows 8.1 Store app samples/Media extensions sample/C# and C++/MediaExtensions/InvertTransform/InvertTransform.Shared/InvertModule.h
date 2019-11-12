// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once

#include "DirectXVideoTransform.h"

ref class CInvertModule sealed: public DirectXVideoTransform 
{
internal:
    void Invalidate() override;
    void Initialize(ID3D11Device *pDevice, UINT uiWidth, UINT uiHeight) override;
    void ProcessFrame(ID3D11Device *pDevice, ID3D11Texture2D *pInput, UINT uiInIndex, ID3D11Texture2D *pOutput, UINT uiOutIndex) override;

private:
    UINT m_uiWidth;
    UINT m_uiHeight;
    ComPtr<ID3D11Buffer> m_spScreenQuadVB;
    ComPtr<ID3D11SamplerState> m_spSampleStateLinear;
    ComPtr<ID3D11InputLayout> m_spQuadLayout;
    ComPtr<ID3D11VertexShader> m_spVertexShader;
    ComPtr<ID3D11PixelShader> m_spPixelShader;
};
