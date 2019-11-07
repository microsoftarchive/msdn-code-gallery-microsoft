//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "OutlineRenderer.h"
#include "Extruder.h"

// describes the constant buffer that will be used to draw the cube for extruded text
struct ConstantBuffer
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;
};

inline static float Clamp(float value, float low, float high)
{
    return (value < low) ? low : (value > high) ? high : value;
}

ref class ExtrudedTextRenderer
{
internal:
    ExtrudedTextRenderer();

    void CreateDeviceIndependentResources(
        Microsoft::WRL::ComPtr<ID2D1Factory1> d2dFactory,
        Microsoft::WRL::ComPtr<IDWriteFactory1> dwriteFactory
        );

    void CreateDeviceResources(
        Microsoft::WRL::ComPtr<ID3D11Device1> d3dDevice,
        Microsoft::WRL::ComPtr<ID3D11DeviceContext1> d3dContext
        );

    void CreateWindowSizeDependentResources(
        float dpi,
        Microsoft::WRL::ComPtr<ID3D11RenderTargetView> renderTargetView,
        Microsoft::WRL::ComPtr<ID3D11DepthStencilView> depthStencilView,
        Windows::Foundation::Size renderTargetSize
        );

    void DrawExtrudedText();

    void SetExtrudedText(Platform::String^ text);
    void Reset();
    void ReleaseBufferResources();

    void OnManipulationDelta(
        Windows::Foundation::Point position,
        Windows::Foundation::Point positionDelta,
        float zoomDelta,
        float rotation
        );

private:
    void UpdateModelAndCamera();
    void UpdateTextGeometry();
    void GenerateTextOutline(ID2D1Geometry** ppGeometry);

    void CreateBuffers(
        uint32 numVertices,
        uint32 numIndices,
        BasicVertex* vertexData,
        unsigned short* indexData,
        ID3D11Buffer** vertexBuffer,
        ID3D11Buffer** indexBuffer
        );

    // Resources shared with PostcardRenderer.
    Microsoft::WRL::ComPtr<ID2D1Factory1>                   m_d2dFactory;
    Microsoft::WRL::ComPtr<IDWriteFactory1>                 m_dwriteFactory;
    Microsoft::WRL::ComPtr<ID3D11Device1>                   m_d3dDevice;
    Microsoft::WRL::ComPtr<ID3D11DeviceContext1>            m_d3dContext;
    Microsoft::WRL::ComPtr<ID3D11RenderTargetView>          m_d3dRenderTargetView;
    Microsoft::WRL::ComPtr<ID3D11DepthStencilView>          m_d3dDepthStencilView;
    Windows::Foundation::Size                               m_renderTargetSize;
    float                                                   m_dpi;

    // Resources specific to this renderer.
    Microsoft::WRL::ComPtr<ID3D11InputLayout>               m_inputLayout;          // cube vertex input layout
    Microsoft::WRL::ComPtr<ID3D11Buffer>                    m_vertexBuffer;         // cube vertex buffer
    Microsoft::WRL::ComPtr<ID3D11Buffer>                    m_indexBuffer;          // cube index buffer
    Microsoft::WRL::ComPtr<ID3D11VertexShader>              m_vertexShader;         // cube vertex shader
    Microsoft::WRL::ComPtr<ID3D11PixelShader>               m_pixelShader;          // cube pixel shader
    Microsoft::WRL::ComPtr<ID3D11Buffer>                    m_constantBuffer;       // constant buffer resource

    unsigned int                                            m_indexCount;           // cube index count
    ConstantBuffer                                          m_constantBufferData;   // constant buffer resource data
    BasicCamera^                                            m_camera;               // scene camera

    Platform::String^                                       m_characters;           // the text to be extruded
    Windows::Foundation::Point                              m_panPosition;          // position of the text
    float                                                   m_zoom;                 // zoom size of the text

    Microsoft::WRL::ComPtr<ID2D1Geometry>                   m_textGeometry;
    Microsoft::WRL::ComPtr<IDWriteTextLayout>               m_textLayout;
};
