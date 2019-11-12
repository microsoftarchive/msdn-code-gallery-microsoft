#pragma once
#include <D3D11.h>

class DirectXVideoTransform
{
public:
    // Called when resources should be invalidated
    virtual void Invalidate() = 0;
    
    // Called to create resources
    virtual HRESULT Initialize(ID3D11Device *pDevice, UINT uiWidth, UINT uiHeight) = 0;

    // Called to process video data
    // pInput and pOutput are both DXGI_FORMAT_B8G8R8X8_UNORM textures with uiWidth,uiHeight as given by Initialize
    // uiInIndex and uiOutIndex are the subresource indices
    virtual HRESULT ProcessFrame(ID3D11Device *pDevice, ID3D11Texture2D *pInput, UINT uiInIndex, ID3D11Texture2D *pOutput, UINT uiOutIndex) = 0;

    virtual ~DirectXVideoTransform(){}
};
