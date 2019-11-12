//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "BasicMath.h"
#include "BasicCamera.h"
#include "SampleOverlay.h"

// describes the constant buffer that will be used to draw the cube
struct ConstantBuffer
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;
};

static const int  numberOfGlowTextures = 2;

// Stuff used for drawing the "full screen quad"
struct ScreenQuadVertex
{
    float4 pos;
    float2 tex;
};

// Constant buffer layout for transferring pixel size to downScaleBrightPass9 pixel shader (DX9 version)

struct ConstantBufferLayoutForDX9BrightPassPixelShader
{
    float pixelWidth;   // Pixel width: will be 1.0 / intermediate render target width
    float pixelHeight;  // Pixel height will be 1.0 / intermediate render target height
};

static const int blurKernelSpan = 15;
static const int blurKernelMidPoint = 7;

// Constant buffer layout for transferring data to the Pixel Shader for glow effect
struct ConstantBufferLayoutForGlowPixelShader
{
    float4 sampleOffsets[blurKernelSpan];
    float4 sampleWeights[blurKernelSpan];
};


ref class D3DPostProcessing : public DirectXBase
{
internal:
    D3DPostProcessing();

    virtual void HandleDeviceLost() override;
    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;

    void Update(float timeTotal, float timeDelta);

private:
    SampleOverlay^                                      m_sampleOverlay;

    Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_inputLayout;                // cube vertex input layout
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_vertexBuffer;               // cube vertex buffer
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_indexBuffer;                // cube index buffer
    Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_vertexShader;               // cube vertex shader
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_pixelShader;                // cube pixel shader
    Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_texture;                    // cube texture
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_textureShaderResourceView;  // cube texture view
    Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_sampler;                    // cube texture sampler
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBuffer;             // constant buffer resource

    uint                                                m_indexCount;                  // cube index count
    ConstantBuffer                                      m_constantBufferData;          // constant buffer resource data
    BasicCamera^                                        m_camera;                      // scene camera

    // Post Processing sample-specific members
    Microsoft::WRL::ComPtr<ID3D11RenderTargetView>      m_intermediateTextureRenderTargetView;    // render target view
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_intermediateTextureShaderResourceView;  // shader resource view
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_quadVertexBuffer;                       // vertex buffer for full window quad
    Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_quadLayout;                             // input layout for full window quad
    Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_quadVertexShader;                       // vertex shader for full window quad
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_brightPassConstantBuffer;               // constant buffer for DX9 brightness threshold pass pixel shader
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_glowConstantBuffer;                     // constant buffer for Glow pixel shader
    Microsoft::WRL::ComPtr<ID3D11RenderTargetView>      m_brightPassTextureRenderTargetView;      // render target view for brightPass texture
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_brightPassTextureShaderResourceView;    // shader resource view for brightPass texture
    Microsoft::WRL::ComPtr<ID3D11RenderTargetView>      m_glowTextureRenderTargetViews[numberOfGlowTextures];   // render target views
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_glowTextureShaderResourceViews[numberOfGlowTextures]; // shader resource views
    uint                                                m_intermediateRenderTargetWidth;          // will be backbuffer width * 0.6f
    uint                                                m_intermediateRenderTargetHeight;         // will be backbuffer height * 0.6f
    uint                                                m_glowTextureWidth;                       // will be intermediate render target width / 4
    uint                                                m_glowTextureHeight;                      // will be intermediate render target height / 4

    // Pixel shaders for Post Processing
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_downScaleBrightPassPixelShader;         // brightness threshold pass pixel shader
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_downScaleBrightPass9PixelShader;        // brightness threshold pass pixel shader for DX9 level hardware
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_glowPixelShader;                        // glow/blur pixel shader
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_finalPassPixelShader;                   // pixel shader to combine 3D cube scene and glow
    // Samplers for Post Processing
    Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_pointSampler;                 // point sampler state for post processing shaders
    Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_linearSampler;                // linear sampler state for post processing shaders

    // Post Processing-specific private methods
    void       DrawQuad(ID3D11PixelShader* pixelShader, uint width, uint height);       // Use a pixel shader to render a quad of width and height
    void       BrightPassDownFilter();                                                  // Apply brightness threshold down scaling pixel shader on 3d cube scene
    void       RenderGlow();                                                            // Apply horizontal and vertical blur to brightness thresholded image -> glow
    void       CombineGlow();                                                           // Use the finalPassPixelShader to combine glow and 3d cube scene into backbuffer

};
