#pragma once

#include "DirectXVideoTransform.h"

class CCopyModule : public DirectXVideoTransform 
{
    virtual void Invalidate(){}

    virtual HRESULT Initialize(ID3D11Device *pDevice, UINT uiWidth, UINT uiHeight){ return S_OK; }

    virtual HRESULT ProcessFrame(ID3D11Device *pDevice, ID3D11Texture2D *pInput, UINT uiInIndex, ID3D11Texture2D *pOutput, UINT uiOutIndex)
    {
        ComPtr<ID3D11DeviceContext> pContext;
        pDevice->GetImmediateContext(&pContext);

        pContext->CopySubresourceRegion(pOutput, uiOutIndex, 0, 0, 0, pInput, uiInIndex, NULL);

        return S_OK;
    }
};