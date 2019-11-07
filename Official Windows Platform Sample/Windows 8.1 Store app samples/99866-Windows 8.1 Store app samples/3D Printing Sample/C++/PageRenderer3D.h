//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "BasicCamera.h"
#include "SampleOverlay.h"
// Needed for print support
#include "BasicShapes.h"
#include <wrl.h>
#include <ppltasks.h>
#include "DirectXSample.h"
#include <PrintPreview.h>
#include <shcore.h>
#include <wrl.h>

// describes the constant buffer that will be used to draw the cube
struct ConstantBuffer
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;
};

ref class PageRenderer3D : public DirectXBase
{
internal :
    PageRenderer3D();

    virtual void CreateDeviceIndependentResources() override;
    virtual void CreateDeviceResources() override;
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;
    void Update(float timeTotal, float timeDelta);

    // Print support
    void Generate3dModelXml(
        _Out_ Microsoft::WRL::ComPtr<IStream>& spOutStream
        );

    // Create DXGI surface for preview
    void DrawPreviewSurface(
        _In_  float                             width,
        _In_  float                             height,
        _In_  float                             scale,
        _In_  D2D1_RECT_F                       contentBox,
        _In_  uint32                            desiredJobPage,
        _In_  IPrintPreviewDxgiPackageTarget*   previewTarget
        );

    // Retrieve the preview image
    Windows::Storage::Streams::InMemoryRandomAccessStream^ PageRenderer3D::GetPageImage();
    void PreparePageImage();
    void GetPageImageSize(
        _Out_ float32* width, 
        _Out_ float32* height
        );

private:
    SampleOverlay^ m_sampleOverlay;
    Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_inputLayout;                // cube vertex input layout
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_vertexBuffer;               // cube vertex buffer
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_indexBuffer;                // cube index buffer
    Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_vertexShader;               // cube vertex shader
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_pixelShader;                // cube pixel shader
    Microsoft::WRL::ComPtr<ID3D11Texture2D>             m_texture;                    // cube texture
    Microsoft::WRL::ComPtr<ID3D11ShaderResourceView>    m_textureSRV;                 // cube texture view
    Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_sampler;                    // cube texture sampler
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBuffer;             // constant buffer resource
    Microsoft::WRL::ComPtr<ID2D1Bitmap>                 m_previewBitmap;              // print preview image
    D2D1_SIZE_F                                         m_previewSize;                // print preview image size
    Windows::Storage::Streams::InMemoryRandomAccessStream^  m_previewImageStream;     // print preview PNG image as stream

    unsigned int             m_vertexCount;                 // cube vertex count
    unsigned int             m_indexCount;                  // cube index count
    ConstantBuffer           m_constantBufferData;          // constant buffer resource data
    BasicCamera^             m_camera;                      // scene camera
};

// RAII (Resource Acquisition Is Initialization) class for manually
// acquiring/releasing the D2D lock.
class D2DFactoryLock
{
public:
    D2DFactoryLock(_In_ ID2D1Factory* d2dFactory)
    {
        DX::ThrowIfFailed(
            d2dFactory->QueryInterface(IID_PPV_ARGS(&m_d2dMultithread))
            );

        m_d2dMultithread->Enter();
    }

    ~D2DFactoryLock()
    {
        m_d2dMultithread->Leave();
    }

private:
    Microsoft::WRL::ComPtr<ID2D1Multithread> m_d2dMultithread;
};
