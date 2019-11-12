#pragma once

#include "DirectXVideoTransform.h"
#include <wrl\client.h>
using namespace Microsoft::WRL;

class CInvertModule : public DirectXVideoTransform 
{
public:
    virtual ~CInvertModule();
    virtual void Invalidate();
    virtual HRESULT Initialize(ID3D11Device *pDevice, UINT uiWidth, UINT uiHeight);
    virtual HRESULT ProcessFrame(ID3D11Device *pDevice, ID3D11Texture2D *pInput, UINT uiInIndex, ID3D11Texture2D *pOutput, UINT uiOutIndex);

private:
    UINT m_uiWidth, m_uiHeight;
    ComPtr<ID3D11Buffer> m_pScreenQuadVB;
    ComPtr<ID3D11SamplerState> m_pSampleStateLinear;
    ComPtr<ID3D11InputLayout> m_spQuadLayout;
    ComPtr<ID3D11VertexShader> m_pVertexShader;
    ComPtr<ID3D11PixelShader> m_pPixelShader;
};
