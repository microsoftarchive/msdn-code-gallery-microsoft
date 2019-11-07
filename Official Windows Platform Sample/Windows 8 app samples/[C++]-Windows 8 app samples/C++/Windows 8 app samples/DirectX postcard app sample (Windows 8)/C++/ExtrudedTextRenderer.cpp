//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ExtrudedTextRenderer.h"

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::Foundation;

ExtrudedTextRenderer::ExtrudedTextRenderer() :
    m_zoom(1.0f)
{
}

void ExtrudedTextRenderer::CreateDeviceIndependentResources(
    ComPtr<ID2D1Factory1> d2dFactory,
    ComPtr<IDWriteFactory1> dwriteFactory
    )
{
    // Save the shared factories.
    m_d2dFactory = d2dFactory;
    m_dwriteFactory = dwriteFactory;
}

void ExtrudedTextRenderer::CreateDeviceResources(
    ComPtr<ID3D11Device1> d3dDevice,
    ComPtr<ID3D11DeviceContext1> d3dContext
    )
{
    // Save the shared Direct3D resources.
    m_d3dDevice = d3dDevice;
    m_d3dContext = d3dContext;

    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    loader->LoadShader(
        L"SimpleVertexShader.cso",
        nullptr,
        0,
        &m_vertexShader,
        &m_inputLayout
        );

    // create the constant buffer for updating model and camera data
    CD3D11_BUFFER_DESC constantBufferDesc(sizeof(ConstantBuffer), D3D11_BIND_CONSTANT_BUFFER);
    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &constantBufferDesc,
            nullptr,             // leave the buffer uninitialized
            &m_constantBuffer
            )
        );

    loader->LoadShader(
        L"SimplePixelShader.cso",
        &m_pixelShader
        );

    m_camera = ref new BasicCamera();
}

void ExtrudedTextRenderer::CreateWindowSizeDependentResources(
    float dpi,
    ComPtr<ID3D11RenderTargetView> renderTargetView,
    ComPtr<ID3D11DepthStencilView> depthStencilView,
    Size renderTargetSize
    )
{
    // Save the shared resources.
    m_dpi = dpi;
    m_d3dRenderTargetView = renderTargetView;
    m_d3dDepthStencilView = depthStencilView;
    m_renderTargetSize = renderTargetSize;

    // update the 3D projection matrix
    m_camera->SetProjectionParameters(
        70.0f,                                                // use a 70-degree vertical field of view
        m_renderTargetSize.Width / m_renderTargetSize.Height, // specify the aspect ratio of the window
        0.01f,                                                // specify the nearest Z-distance at which to draw vertices
        10000.0f                                              // specify the farthest Z-distance at which to draw vertices
        );

    // save the new projection matrix to the constant buffer data struct
    m_camera->GetProjectionMatrix(&m_constantBufferData.projection);
}

void ExtrudedTextRenderer::DrawExtrudedText()
{
    if (m_characters != nullptr && m_characters->Length() > 0)
    {
        // Get the outline of the current text as a geometry.
        ComPtr<ID2D1Geometry> textOutlineGeometry;
        GenerateTextOutline(&textOutlineGeometry);

        // Extrude the geometry into a vector of SimpleVertex.
        std::vector<SimpleVertex> vertices;
        Extruder::ExtrudeGeometry(
            textOutlineGeometry.Get(),
            24.0f, // height
            &vertices
            );

        m_indexCount = vertices.size();

        // Convert the vector of SimpleVertex into an array of BasicVertex.
        std::unique_ptr<BasicVertex[]> basicVertices(new BasicVertex[m_indexCount]);
        for (unsigned int i = 0; i < m_indexCount; i++)
        {
            SimpleVertex v = vertices[i];
            basicVertices[i].pos = v.Pos;
            basicVertices[i].norm = v.Norm;
            // basicVertices[i].tex is not used
        }

        // Build the index buffer. It's reversed so that handedness is correct.
        std::unique_ptr<unsigned short[]> indices(new unsigned short[m_indexCount]);
        for (unsigned int i = 0; i < m_indexCount; i++)
        {
            indices[i] = m_indexCount - i - 1;
        }

        CreateBuffers(
            m_indexCount,
            m_indexCount,
            basicVertices.get(),
            indices.get(),
            &m_vertexBuffer,
            &m_indexBuffer
            );

        uint stride = sizeof(BasicVertex);
        uint offset = 0;

        // Bind the render targets.
        m_d3dContext->OMSetRenderTargets(
            1,
            m_d3dRenderTargetView.GetAddressOf(),
            m_d3dDepthStencilView.Get()
            );

        m_d3dContext->ClearDepthStencilView(
            m_d3dDepthStencilView.Get(),
            D3D11_CLEAR_DEPTH,
            1.0f,
            0
            );

        m_d3dContext->IASetInputLayout(m_inputLayout.Get());

        // Set the vertex buffer.
        m_d3dContext->IASetVertexBuffers(
            0,                              // starting at the first vertex buffer slot
            1,                              // set one vertex buffer binding
            m_vertexBuffer.GetAddressOf(),
            &stride,                        // specify the size in bytes of a single vertex
            &offset                         // specify the base vertex in the buffer
            );

        // Set the index buffer.
        m_d3dContext->IASetIndexBuffer(
            m_indexBuffer.Get(),
            DXGI_FORMAT_R16_UINT,   // unsigned short index format
            0                       // specify the base index in the buffer
            );

        // Specify the way the vertex and index buffers define geometry.
        m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

        // Set the vertex shader stage state.
        m_d3dContext->VSSetShader(
            m_vertexShader.Get(),
            nullptr,                // don't use shader linkage
            0                       // don't use shader linkage
            );

        // Set the constant buffer.
        m_d3dContext->VSSetConstantBuffers(
            0,                              // starting at the first constant buffer slot
            1,                              // set one constant buffer binding
            m_constantBuffer.GetAddressOf()
            );

        // Create a viewport descriptor of the full window size. The
        // panning position is used to offset the rendering.
        CD3D11_VIEWPORT viewport(
            m_panPosition.X,
            m_panPosition.Y,
            m_renderTargetSize.Width,
            m_renderTargetSize.Height
            );

        // Set the current viewport using the descriptor.
        m_d3dContext->RSSetViewports(1, &viewport);

        // Set the pixel shader stage state.
        m_d3dContext->PSSetShader(
            m_pixelShader.Get(),
            nullptr,                // don't use shader linkage
            0                       // don't use shader linkage
            );

        // Draw the vertices.
        m_d3dContext->DrawIndexed(
            m_indexCount,   // draw all created vertices
            0,              // starting with the first vertex
            0               // and the first index
            );
    }
}

void ExtrudedTextRenderer::SetExtrudedText(String^ text)
{
    m_characters = text;
    UpdateTextGeometry();
    UpdateModelAndCamera();
}

void ExtrudedTextRenderer::Reset()
{
    m_characters = nullptr;
    m_panPosition = Point(0.0f, 0.0f);
    m_zoom = 1.0f;
}

void ExtrudedTextRenderer::ReleaseBufferResources()
{
    m_d3dRenderTargetView = nullptr;
    m_d3dDepthStencilView = nullptr;
}

void ExtrudedTextRenderer::OnManipulationDelta(
    Point position,
    Point positionDelta,
    float zoomDelta,
    float rotation
    )
{
    // Update the transformation variables to reflect how the user is manipulating the text.
    m_panPosition.X += positionDelta.X;
    m_panPosition.Y += positionDelta.Y;

    m_zoom *= zoomDelta;
    m_zoom = Clamp(m_zoom, 0.75, 2.0);

    UpdateModelAndCamera();
}

void ExtrudedTextRenderer::UpdateModelAndCamera()
{
    // Update the model based on the current zoom level.
    float4x4 model = identity();
    model = mul(model, scale(m_zoom, m_zoom, m_zoom));

    m_constantBufferData.model = model;

    // Update the view matrix based on the camera position.
    // For this sample, the camera position is fixed.
    m_camera->SetViewParameters(
        float3(0, -20, -500.0f),
        float3(0, -20.0f, 0),
        float3(0, -1, 0)
        );

    m_camera->GetViewMatrix(&m_constantBufferData.view);

    // Update the constant buffer with the new data.
    m_d3dContext->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        nullptr,
        &m_constantBufferData,
        0,
        0
        );
}

// Generates a layout object and a geometric outline
// corresponding to the current text string and stores the
// results in m_pTextLayout and m_pTextGeometry.
void ExtrudedTextRenderer::UpdateTextGeometry()
{
    if (m_characters != nullptr && m_characters->Length() > 0)
    {
        const float fontSize = 72.0f;

        ComPtr<IDWriteTextFormat> textFormat;
        DX::ThrowIfFailed(
            m_dwriteFactory->CreateTextFormat(
                L"Gabriola",
                nullptr,
                DWRITE_FONT_WEIGHT_EXTRA_BOLD,
                DWRITE_FONT_STYLE_NORMAL,
                DWRITE_FONT_STRETCH_NORMAL,
                fontSize,
                L"en-us",
                &textFormat
                )
            );

        DX::ThrowIfFailed(
            m_dwriteFactory->CreateTextLayout(
                m_characters->Data(),
                m_characters->Length(),
                textFormat.Get(),
                0.0f,           // lineWidth (ignored because of NO_WRAP)
                fontSize,       // lineHeight
                &m_textLayout
                )
            );

        DX::ThrowIfFailed(
            m_textLayout->SetWordWrapping(DWRITE_WORD_WRAPPING_NO_WRAP)
            );

        ComPtr<IDWriteTypography> typography;
        DX::ThrowIfFailed(
            m_dwriteFactory->CreateTypography(&typography)
            );

        DWRITE_FONT_FEATURE fontFeature =
        {
            DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_7,
            1
        };

        DX::ThrowIfFailed(
            typography->AddFontFeature(fontFeature)
            );

        DWRITE_TEXT_RANGE textRange = {0, m_characters->Length()};

        DX::ThrowIfFailed(
            m_textLayout->SetTypography(typography.Get(), textRange)
            );

        // Create a custom text renderer, then pass the text through it via
        // the Draw method in order to compute the text's outline.
        ComPtr<OutlineRenderer> renderer;
        OutlineRenderer::CreateOutlineRenderer(m_d2dFactory.Get(), &renderer);

        DX::ThrowIfFailed(
            m_textLayout->Draw(
                nullptr,    // clientDrawingContext
                renderer.Get(),
                0.0f,       // originX
                0.0f        // originY
                )
            );

        // Retrieve the ID2D1Geometry representing the text's outline.
        renderer->GetGeometry(&m_textGeometry);
    }
}

// Returns a version of the text geometry that is horizontally
// centered and vertically positioned so (0, 0) is on the baseline.
void ExtrudedTextRenderer::GenerateTextOutline(ID2D1Geometry** ppGeometry)
{
    DWRITE_LINE_METRICS lineMetrics;
    DWRITE_TEXT_METRICS textMetrics;
    UINT actualNumLines;

    //
    // We're assuming here that the text doesn't wrap and doesn't contain
    // newlines.
    //
    DX::ThrowIfFailed(
        m_textLayout->GetLineMetrics(
            &lineMetrics,
            1,                  // maxLineCount
            &actualNumLines     // ignored
            )
        );

    DX::ThrowIfFailed(
        m_textLayout->GetMetrics(&textMetrics)
        );

    float offsetY = -lineMetrics.baseline;
    float offsetX = -textMetrics.widthIncludingTrailingWhitespace / 2;

    ComPtr<ID2D1TransformedGeometry> pTransformedGeometry;
    DX::ThrowIfFailed(
        m_d2dFactory->CreateTransformedGeometry(
            m_textGeometry.Get(),
            D2D1::Matrix3x2F::Translation(offsetX, offsetY),
            &pTransformedGeometry
            )
        );

    // Transfer reference.
    *ppGeometry = pTransformedGeometry.Get();
    (*ppGeometry)->AddRef();
}

void ExtrudedTextRenderer::CreateBuffers(
    uint32 numVertices,
    uint32 numIndices,
    BasicVertex* vertexData,
    unsigned short* indexData,
    ID3D11Buffer** vertexBuffer,
    ID3D11Buffer** indexBuffer
    )
{
    *vertexBuffer = nullptr;
    *indexBuffer = nullptr;
    ComPtr<ID3D11Buffer> vertexBufferInternal;
    ComPtr<ID3D11Buffer> indexBufferInternal;

    D3D11_BUFFER_DESC VertexBufferDesc;
    VertexBufferDesc.ByteWidth = sizeof(BasicVertex) * numVertices;
    VertexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    VertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    VertexBufferDesc.CPUAccessFlags = 0;
    VertexBufferDesc.MiscFlags = 0;
    VertexBufferDesc.StructureByteStride = 0;

    D3D11_SUBRESOURCE_DATA VertexBufferData;
    VertexBufferData.pSysMem = vertexData;
    VertexBufferData.SysMemPitch = 0;
    VertexBufferData.SysMemSlicePitch = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &VertexBufferDesc,
            &VertexBufferData,
            &vertexBufferInternal
            )
        );

    D3D11_BUFFER_DESC IndexBufferDesc;
    IndexBufferDesc.ByteWidth = sizeof(unsigned short) * numIndices;
    IndexBufferDesc.Usage = D3D11_USAGE_DEFAULT;
    IndexBufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
    IndexBufferDesc.CPUAccessFlags = 0;
    IndexBufferDesc.MiscFlags = 0;
    IndexBufferDesc.StructureByteStride = 0;

    D3D11_SUBRESOURCE_DATA IndexBufferData;
    IndexBufferData.pSysMem = indexData;
    IndexBufferData.SysMemPitch = 0;
    IndexBufferData.SysMemSlicePitch = 0;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateBuffer(
            &IndexBufferDesc,
            &IndexBufferData,
            &indexBufferInternal
            )
        );

    *vertexBuffer = vertexBufferInternal.Detach();
    *indexBuffer = indexBufferInternal.Detach();
}
