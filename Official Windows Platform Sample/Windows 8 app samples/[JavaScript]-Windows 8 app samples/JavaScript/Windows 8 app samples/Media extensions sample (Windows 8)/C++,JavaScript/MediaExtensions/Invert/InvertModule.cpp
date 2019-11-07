#pragma once

#include "InvertModule.h"

struct ScreenVertex
{
    FLOAT pos[4];
    FLOAT tex[2];
};

CInvertModule::~CInvertModule()
{
    Invalidate();
}

void CInvertModule::Invalidate()
{
    m_pScreenQuadVB = nullptr;
    m_pSampleStateLinear = nullptr;
    m_spQuadLayout = nullptr;
    m_pVertexShader = nullptr;
    m_pPixelShader = nullptr;
}

HRESULT CInvertModule::Initialize(ID3D11Device *pDevice, UINT uiWidth, UINT uiHeight)
{
    HRESULT hr = S_OK;

    m_uiWidth = uiWidth;
    m_uiHeight = uiHeight;

    if(m_pScreenQuadVB == nullptr)
    {
        // Vertex Buffer Layout
        D3D11_BUFFER_DESC vbdesc;

        // 2 vertices (Triangles), covering the whole Viewport, with the input video mapped as a texture
        const ScreenVertex svDefault[4] = {
            //   x      y     z     w         u     v
            { { -1.0f,  1.0f, 0.5f, 1.0f }, { 0.0f, 0.0f } }, // 0
            { {  1.0f,  1.0f, 0.5f, 1.0f }, { 1.0f, 0.0f } }, // 1
            { { -1.0f, -1.0f, 0.5f, 1.0f }, { 0.0f, 1.0f } }, // 2
            { {  1.0f, -1.0f, 0.5f, 1.0f }, { 1.0f, 1.0f } }  // 3
        };

        ZeroMemory(&vbdesc, sizeof(vbdesc));

        vbdesc.Usage = D3D11_USAGE_DYNAMIC;
        vbdesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
        vbdesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
        vbdesc.MiscFlags = 0;
        vbdesc.ByteWidth = sizeof( svDefault );

        D3D11_SUBRESOURCE_DATA InitData;
        InitData.pSysMem = svDefault;
        InitData.SysMemPitch = 0;
        InitData.SysMemSlicePitch = 0;
        hr = pDevice->CreateBuffer( &vbdesc, &InitData, &m_pScreenQuadVB );
        if( FAILED(hr) )
        {
            goto done;
        }
    }

    // Pixel Shader
    {
        static
#       include "PixelShader.hps"
        if(m_pPixelShader == nullptr)
        {
            hr = pDevice->CreatePixelShader(g_psshader, sizeof(g_psshader), NULL, &m_pPixelShader);
			if(FAILED(hr))
			{
				goto done;
			}
        }
    }

    // Vertex Shader
    {
        static
#       include "VertexShader.hvs"
        if( m_pVertexShader == nullptr )
        {
            const D3D11_INPUT_ELEMENT_DESC quadlayout[] =
            {
                { "POSITION", 0, DXGI_FORMAT_R32G32B32A32_FLOAT, 0,  0, D3D11_INPUT_PER_VERTEX_DATA, 0 },
                { "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT,       0, 16, D3D11_INPUT_PER_VERTEX_DATA, 0 },
            };
            hr = pDevice->CreateVertexShader(g_vsshader, sizeof(g_vsshader), NULL, &m_pVertexShader);
			if(FAILED(hr))
			{
				goto done;
			}

            hr = pDevice->CreateInputLayout(quadlayout, 2, g_vsshader, sizeof(g_vsshader), &m_spQuadLayout);
			if(FAILED(hr))
			{
				goto done;
			}
        }
    }

    if( m_pSampleStateLinear == NULL )
    {
        // Sampler
        D3D11_SAMPLER_DESC SamplerDesc;
        ZeroMemory(&SamplerDesc, sizeof(SamplerDesc));

        SamplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_CLAMP;
        SamplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_CLAMP;
        SamplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_CLAMP;
        SamplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
        SamplerDesc.MaxLOD = D3D11_FLOAT32_MAX;

        hr = pDevice->CreateSamplerState( &SamplerDesc, &m_pSampleStateLinear);
        if( FAILED(hr) )
        {
            goto done;
        }
    }

    return hr;

done:
    m_pScreenQuadVB = nullptr;
    m_pSampleStateLinear = nullptr;
    m_spQuadLayout = nullptr;
    m_pVertexShader = nullptr;
    m_pPixelShader = nullptr;
    return hr;

}

HRESULT CreateShaderInputView(ID3D11Texture2D *pTexture, UINT uiIndex, ID3D11Device *pDevice, ID3D11ShaderResourceView **ppSRV)
{
    D3D11_TEXTURE2D_DESC textureDesc;
    D3D11_SHADER_RESOURCE_VIEW_DESC viewDesc;

    ZeroMemory( &viewDesc, sizeof( viewDesc ) );

    pTexture->GetDesc( &textureDesc );

    viewDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;

    if ( textureDesc.ArraySize > 1 )
    {
        // Texture array, use the subresource index
        viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2DARRAY;
        viewDesc.Texture2DArray.FirstArraySlice = uiIndex;
        viewDesc.Texture2DArray.ArraySize = 1;
        viewDesc.Texture2DArray.MipLevels = 1;
        viewDesc.Texture2DArray.MostDetailedMip = 0;
    }
    else
    {
        viewDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
        viewDesc.Texture2D.MostDetailedMip = 0;
        viewDesc.Texture2D.MipLevels = 1;
    }

    return pDevice->CreateShaderResourceView(pTexture,
                                             &viewDesc,
                                             ppSRV);
}

HRESULT CreateShaderOutputView(ID3D11Texture2D *pTexture, UINT uiIndex, ID3D11Device *pDevice, ID3D11RenderTargetView **ppRTV)
{
    D3D11_TEXTURE2D_DESC textureDesc;
    D3D11_RENDER_TARGET_VIEW_DESC viewDesc;

    ZeroMemory( &viewDesc, sizeof( viewDesc ) );

    pTexture->GetDesc( &textureDesc );

    viewDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;

    if ( textureDesc.ArraySize > 1 )
    {
        // Texture array, use the subresource index
        viewDesc.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2DARRAY;
        viewDesc.Texture2DArray.FirstArraySlice = uiIndex;
        viewDesc.Texture2DArray.ArraySize = 1;
        viewDesc.Texture2DArray.MipSlice = 0;
    }
    else
    {
        viewDesc.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2D;
        viewDesc.Texture2D.MipSlice = 0;
    }

    return pDevice->CreateRenderTargetView(pTexture,
                                           &viewDesc,
                                           ppRTV );

}

HRESULT CInvertModule::ProcessFrame(ID3D11Device *pDevice, ID3D11Texture2D *pInput, UINT uiInIndex, ID3D11Texture2D *pOutput, UINT uiOutIndex)
{

    HRESULT hr = S_OK;
    ComPtr<ID3D11DeviceContext> pd3dImmediateContext;

    // Render targets
    ComPtr<ID3D11RenderTargetView> rgpOrigRTV;
    ComPtr<ID3D11DepthStencilView> pOrigDSV;
    ID3D11RenderTargetView *pRTVs[1] = { nullptr };

    // Shader resources
    ComPtr<ID3D11ShaderResourceView> rgpOrigSRV;
    ID3D11ShaderResourceView *pSRVs[1] = { nullptr };

    // Vertex buffers
    ID3D11Buffer *pBuffers[1] = { m_pScreenQuadVB.Get() };
    UINT vbStrides = sizeof( ScreenVertex );
    UINT vbOffsets = 0;

    // Samplers
    ID3D11SamplerState *pSamplers[1] = { m_pSampleStateLinear.Get() };

    // View port
    D3D11_VIEWPORT vpOld[D3D11_VIEWPORT_AND_SCISSORRECT_MAX_INDEX];
    UINT nViewPorts = 1;
    D3D11_VIEWPORT vp;

    // Get the context
    pDevice->GetImmediateContext(&pd3dImmediateContext);
   
    // Get the resource views
    hr = CreateShaderInputView(pInput, uiInIndex, pDevice, &(pSRVs[0]));
    if( FAILED(hr) )
    {
        goto done;
    }

    hr = CreateShaderOutputView(pOutput, uiOutIndex, pDevice, &(pRTVs[0]));
    if( FAILED(hr) )
    {
        goto done;
    }

    // Setup the Draw call
    pd3dImmediateContext->IASetInputLayout(m_spQuadLayout.Get());

    pd3dImmediateContext->IASetVertexBuffers(0, 1, pBuffers, &vbStrides, &vbOffsets);

    pd3dImmediateContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP);

    pd3dImmediateContext->PSSetSamplers(0, 1, pSamplers);

    pd3dImmediateContext->OMGetRenderTargets(1, &rgpOrigRTV, &pOrigDSV);

    pd3dImmediateContext->OMSetRenderTargets(1, pRTVs, NULL);  

    pd3dImmediateContext->PSGetShaderResources(0, 1, &rgpOrigSRV);

    pd3dImmediateContext->PSSetShaderResources(0, 1, pSRVs);

    pd3dImmediateContext->RSGetViewports( &nViewPorts, vpOld );

    // Setup the viewport to match the backbuffer
    vp.Width = (float)m_uiWidth;
    vp.Height = (float)m_uiHeight;
    vp.MinDepth = 0.0f;
    vp.MaxDepth = 1.0f;
    vp.TopLeftX = 0.0f;
    vp.TopLeftY = 0.0f;

    pd3dImmediateContext->RSSetViewports( 1, &vp );

    pd3dImmediateContext->VSSetShader( m_pVertexShader.Get(), NULL, 0 );

    pd3dImmediateContext->PSSetShader( m_pPixelShader.Get(), NULL, 0 );

    pd3dImmediateContext->Draw( 4, 0 );

    // Restore the Old
    pd3dImmediateContext->RSSetViewports( nViewPorts, vpOld );
    pd3dImmediateContext->PSSetShaderResources(0, 1, rgpOrigSRV.GetAddressOf());
    pd3dImmediateContext->OMSetRenderTargets( 1, rgpOrigRTV.GetAddressOf(), pOrigDSV.Get() ); 

done:
    if(nullptr != pSRVs[0])
    {
        pSRVs[0]->Release();
    }
    if(nullptr != pRTVs[0])
    {
        pRTVs[0]->Release();
    }
    return hr; 

}