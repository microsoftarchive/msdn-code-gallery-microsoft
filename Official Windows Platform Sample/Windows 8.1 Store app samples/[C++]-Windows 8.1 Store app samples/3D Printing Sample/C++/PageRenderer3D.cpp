//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "PageRenderer3D.h"
#include "BasicShapes.h"
#include "BasicLoader.h"
#include <collection.h>
#include <dxgiformat.h>
#include <d2d1Helper.h>
#include <shcore.h>
#include <xmllite.h>

// Needed for print functionality
using namespace Concurrency;
using namespace Windows::UI::Core;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Microsoft::WRL;
using namespace D2D1;
using namespace Windows::Storage::Streams;

extern BasicVertex cubeVertices[24];
extern unsigned short cubeIndices[36];
extern float3 cubeVertexData[8];
extern unsigned short cubeIndexData[36];

PageRenderer3D::PageRenderer3D()
{
}

void PageRenderer3D::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();
}

void PageRenderer3D::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    BasicLoader^ loader = ref new BasicLoader(m_d3dDevice.Get());

    loader->LoadShader(
        L"SimpleVertexShader.cso",
        nullptr,
        0,
        &m_vertexShader,
        &m_inputLayout
        );

    // create the vertex and index buffers for drawing the cube

    BasicShapes^ shapes = ref new BasicShapes(m_d3dDevice.Get());

    shapes->CreateCube(
        &m_vertexBuffer,
        &m_indexBuffer,
        &m_vertexCount,
        &m_indexCount
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

    loader->LoadTexture(
        L"reftexture.dds",
        &m_texture,
        &m_textureSRV
        );

    // create the sampler
    D3D11_SAMPLER_DESC samplerDesc;
    ZeroMemory(&samplerDesc, sizeof(D3D11_SAMPLER_DESC));
    samplerDesc.Filter = D3D11_FILTER_ANISOTROPIC;
    samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
    samplerDesc.MipLODBias = 0.0f;
    // use 4x on feature level 9.2 and above, otherwise use only 2x
    samplerDesc.MaxAnisotropy = m_featureLevel > D3D_FEATURE_LEVEL_9_1 ? 4 : 2;
    samplerDesc.ComparisonFunc = D3D11_COMPARISON_NEVER;
    samplerDesc.BorderColor[0] = 0.0f;
    samplerDesc.BorderColor[1] = 0.0f;
    samplerDesc.BorderColor[2] = 0.0f;
    samplerDesc.BorderColor[3] = 0.0f;
    // allow use of all mip levels
    samplerDesc.MinLOD = 0;
    samplerDesc.MaxLOD = D3D11_FLOAT32_MAX;

    DX::ThrowIfFailed(
        m_d3dDevice->CreateSamplerState(
        &samplerDesc,
        &m_sampler)
        );

    m_camera = ref new BasicCamera();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "3D printing sample for Windows Store app using C++ and Direct3D"
        );
}

void PageRenderer3D::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // update the 3D projection matrix
    m_camera->SetProjectionParameters(
        70.0f,                                                // use a 70-degree vertical field of view
        m_renderTargetSize.Width / m_renderTargetSize.Height, // specify the aspect ratio of the window
        0.01f,                                                // specify the nearest Z-distance at which to draw vertices
        100.0f                                                // specify the farthest Z-distance at which to draw vertices
        );

    // save the new projection matrix to the constant buffer data struct
    m_camera->GetProjectionMatrix(&m_constantBufferData.projection);

    m_sampleOverlay->UpdateForWindowSizeChange();
}

void PageRenderer3D::Update(float timeTotal, float timeDelta)
{
    // Lock the D2D factory to avoid thread contention between main UI thread and preview UI thread
    D2DFactoryLock factoryLock(m_d2dFactory.Get());

    // update the model matrix based on the time
    m_constantBufferData.model = rotationY(-timeTotal * 60.0f);

    // update the view matrix based on the camera position
    // note that for this sample, the camera position is fixed
    m_camera->SetViewParameters(
        float3(0, 10.0f, 20.0f),
        float3(0, 0, 0),
        float3(0, 1, 0)
        );

    m_camera->GetViewMatrix(&m_constantBufferData.view);

    // update the constant buffer with the new data
    m_d3dContext->UpdateSubresource(
        m_constantBuffer.Get(),
        0,
        nullptr,
        &m_constantBufferData,
        0,
        0
        );
}

void PageRenderer3D::Render()
{
    // Lock the D2D factory to avoid thread contention between main UI thread and preview UI thread
    D2DFactoryLock factoryLock(m_d2dFactory.Get());

    m_d3dContext->OMSetRenderTargets(
        1,
        m_d3dRenderTargetView.GetAddressOf(),
        m_d3dDepthStencilView.Get()
        );

    // clear both the render target and depth stencil to default values
    const float ClearColor[4] = { 0.071f, 0.040f, 0.561f, 1.0f };
    m_d3dContext->ClearRenderTargetView(
        m_d3dRenderTargetView.Get(),
        ClearColor
        );

    m_d3dContext->ClearDepthStencilView(
        m_d3dDepthStencilView.Get(),
        D3D11_CLEAR_DEPTH,
        1.0f,
        0
        );

    m_d3dContext->IASetInputLayout(m_inputLayout.Get());

    // set the vertex and index buffers, and specify the way they define geometry
    UINT stride = sizeof(BasicVertex);
    UINT offset = 0;
    m_d3dContext->IASetVertexBuffers(
        0,                              // starting at the first vertex buffer slot
        1,                              // set one vertex buffer binding
        m_vertexBuffer.GetAddressOf(),
        &stride,                        // specify the size in bytes of a single vertex
        &offset                         // specify the base vertex in the buffer
        );

    // set the index buffer
    m_d3dContext->IASetIndexBuffer(
        m_indexBuffer.Get(),
        DXGI_FORMAT_R16_UINT,   // unsigned short index format
        0                       // specify the base index in the buffer
        );

    // specify the way the vertex and index buffers define geometry
    m_d3dContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

    // set the vertex shader stage state
    m_d3dContext->VSSetShader(
        m_vertexShader.Get(),
        nullptr,                // don't use shader linkage
        0                       // don't use shader linkage
        );

    m_d3dContext->VSSetConstantBuffers(
        0,                              // starting at the first constant buffer slot
        1,                              // set one constant buffer binding
        m_constantBuffer.GetAddressOf()
        );

    // set the pixel shader stage state
    m_d3dContext->PSSetShader(
        m_pixelShader.Get(),
        nullptr,                // don't use shader linkage
        0                       // don't use shader linkage
        );

    m_d3dContext->PSSetShaderResources(
        0,                          // starting at the first shader resource slot
        1,                          // set one shader resource binding
        m_textureSRV.GetAddressOf()
        );

    m_d3dContext->PSSetSamplers(
        0,                          // starting at the first sampler slot
        1,                          // set one sampler binding
        m_sampler.GetAddressOf()
        );

    // draw the cube
    m_d3dContext->DrawIndexed(
        m_indexCount,   // draw all created vertices
        0,              // starting with the first vertex
        0               // and the first index
        );

    // render the overlay text
    m_sampleOverlay->Render();

    // show the results
    Present();
}

//
// The output IStream represents the 3D model of the scene. The XML follows the 3MF format.
//
void
PageRenderer3D::Generate3dModelXml(
_Out_ ComPtr<IStream>& spOutStream
)
{
    HRESULT hr = S_OK;
    int idCounter = 0;
    String ^ texturePath = "/3D/Textures/tex0.texture";
    int textureWidth = 100;
    int textureHeight = 100;
    int outputObjectId = 0;
    int textureId = 0;
    int colorId = 0;

    ComPtr<IXmlWriter> spWriter;
    LPCWSTR pwszPrefix = nullptr;
    LPCWSTR pwszLocalName = nullptr;

    // create the XML writer
    InMemoryRandomAccessStream^ memStream = ref new InMemoryRandomAccessStream();
    DX::ThrowIfFailed(::CreateStreamOverRandomAccessStream(memStream, IID_PPV_ARGS(&spOutStream)));
    DX::ThrowIfFailed(::CreateXmlWriter(IID_PPV_ARGS(&spWriter), nullptr));
    DX::ThrowIfFailed(spWriter->SetOutput(spOutStream.Get()));
    DX::ThrowIfFailed(spWriter->SetProperty(XmlWriterProperty_Indent, TRUE));

    // write model data with the XML writer
    DX::ThrowIfFailed(spWriter->WriteStartDocument(XmlStandalone_No));
    DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"model", L"http://schemas.microsoft.com/3dmanufacturing/2013/01"));
    DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"unit", nullptr, L"millimeter"));

    DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"resources", nullptr));

    DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"texture", nullptr));
    DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"id", nullptr, idCounter.ToString()->Data()));
    DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"path", nullptr, texturePath->Data()));
    DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"width", nullptr, textureWidth.ToString()->Data()));
    DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"height", nullptr, textureHeight.ToString()->Data()));
    DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"contenttype", nullptr, L"image/png"));
    DX::ThrowIfFailed(spWriter->WriteEndElement());//</texture>

    textureId = idCounter;
    idCounter++;
    String^ color = L"tex(" + textureId.ToString() + L")";
    DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"color", nullptr));
    DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"id", nullptr, idCounter.ToString()->Data()));
    DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"value", nullptr, color->Data()));
    DX::ThrowIfFailed(spWriter->WriteEndElement());//</color>

    // Output object
    colorId = idCounter;
    idCounter++;
    outputObjectId = idCounter;
    DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"object", nullptr));
    DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"id", nullptr, outputObjectId.ToString()->Data()));

    DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"mesh", nullptr));

    // Calculate offsets to get vertices into positive x,y,z quadrant
    float32 xOffset = 0.0f;
    float32 yOffset = 0.0f;
    float32 zOffset = 0.0f;
    for (unsigned int i = 0; i < ARRAYSIZE(cubeVertexData); i++)
    {
        if (cubeVertexData[i].x < xOffset)
        {
            xOffset = cubeVertexData[i].x;
        }
        if (cubeVertexData[i].y < yOffset)
        {
            yOffset = cubeVertexData[i].y;
        }
        if (cubeVertexData[i].z < zOffset)
        {
            zOffset = cubeVertexData[i].z;
        }
    }

    // Output vertices
    DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"vertices", nullptr));

    for (unsigned int i = 0; i < ARRAYSIZE(cubeVertexData); i++)
    {
        DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"vertex", nullptr));
        DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"x", nullptr, (cubeVertexData[i].x - xOffset).ToString()->Data()));
        DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"y", nullptr, (cubeVertexData[i].y - yOffset).ToString()->Data()));
        DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"z", nullptr, (cubeVertexData[i].z - zOffset).ToString()->Data()));
        DX::ThrowIfFailed(spWriter->WriteEndElement());//</vertex>
    }

    DX::ThrowIfFailed(spWriter->WriteEndElement());//</vertices>

    // Output triangles
    DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"triangles", nullptr));

    for (unsigned int i = 0; i < ARRAYSIZE(cubeIndexData); i+=3)
    {
        //
        // Change the order of the triangle indices because D3D uses a left-handed coordinate system
        // and the 3MF format uses a right-handed coordinate system.
        //

        DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"triangle", nullptr));
        DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"v1", nullptr, cubeIndexData[i].ToString()->Data()));
        DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"v2", nullptr, cubeIndexData[i+2].ToString()->Data()));
        DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"v3", nullptr, cubeIndexData[i+1].ToString()->Data()));

        DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"colorid", nullptr, colorId.ToString()->Data()));

        String^ cv1 = (cubeVertices[cubeIndices[i]].tex.x*textureWidth).ToString() + L","+(cubeVertices[cubeIndices[i]].tex.y*textureHeight).ToString();
        DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"cv1", nullptr, cv1->Data()));
        String^ cv2 = (cubeVertices[cubeIndices[i+2]].tex.x*textureWidth).ToString() + L","+(cubeVertices[cubeIndices[i+1]].tex.y*textureHeight).ToString();
        DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"cv2", nullptr, cv2->Data()));
        String^ cv3 = (cubeVertices[cubeIndices[i+1]].tex.x*textureWidth).ToString() + L","+(cubeVertices[cubeIndices[i+2]].tex.y*textureHeight).ToString();
        DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"cv3", nullptr, cv3->Data()));

        DX::ThrowIfFailed(spWriter->WriteEndElement());//</triangle>
    }

    DX::ThrowIfFailed(spWriter->WriteEndElement());//</triangles>

    DX::ThrowIfFailed(spWriter->WriteEndElement());//</mesh>
    DX::ThrowIfFailed(spWriter->WriteEndElement());//</object>
    DX::ThrowIfFailed(spWriter->WriteEndElement());//</resources>

    // Build
    DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"build", nullptr));
    DX::ThrowIfFailed(spWriter->WriteStartElement(nullptr, L"item", nullptr));
    DX::ThrowIfFailed(spWriter->WriteAttributeString(nullptr, L"objectid", nullptr, outputObjectId.ToString()->Data()));
    DX::ThrowIfFailed(spWriter->WriteEndElement());//</item>
    DX::ThrowIfFailed(spWriter->WriteEndElement());//</build>

    DX::ThrowIfFailed(spWriter->WriteEndElement());//</model>

    DX::ThrowIfFailed(spWriter->WriteEndDocument());
    DX::ThrowIfFailed(spWriter->Flush());
}

void PageRenderer3D::DrawPreviewSurface(
    _In_  float                             width,
    _In_  float                             height,
    _In_  float                             scale,
    _In_  D2D1_RECT_F                       contentBox,
    _In_  uint32                            desiredJobPage,
    _In_  IPrintPreviewDxgiPackageTarget*   previewTarget
    )
{
    // Lock the D2D factory to avoid contention between main UI thread and preview UI thread
    D2DFactoryLock factoryLock(m_d2dFactory.Get());

    // If we have not previously generated a preview image, generate it now
    if (m_previewBitmap == NULL)
    {
        PreparePageImage();
    }

    CD3D11_TEXTURE2D_DESC textureDesc(
        DXGI_FORMAT_B8G8R8A8_UNORM,
        static_cast<uint32>(ceil(width  * m_dpi / 96)),
        static_cast<uint32>(ceil(height * m_dpi / 96)),
        1,
        1,
        D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE
        );

    ComPtr<ID3D11Texture2D> texture;
    DX::ThrowIfFailed(
        m_d3dDevice->CreateTexture2D(
        &textureDesc,
        nullptr,
        &texture
        )
        );

    // Create a preview DXGI surface with given size
    ComPtr<IDXGISurface> dxgiSurface;
    DX::ThrowIfFailed(
        texture.As<IDXGISurface>(&dxgiSurface)
        );

    // Create a new D2D device context for rendering the preview surface. D2D
    // device contexts are stateful, and hence a unique device context must be
    // used on each thread.
    ComPtr<ID2D1DeviceContext> d2dContext;
    DX::ThrowIfFailed(
        m_d2dDevice->CreateDeviceContext(
        D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
        &d2dContext
        )
        );

    // Update DPI for preview surface as well
    d2dContext->SetDpi(m_dpi, m_dpi);

    // Recommend using the screen DPI for better fidelity and better performance in the print preview
    D2D1_BITMAP_PROPERTIES1 bitmapProperties =
        D2D1::BitmapProperties1(
        D2D1_BITMAP_OPTIONS_TARGET | D2D1_BITMAP_OPTIONS_CANNOT_DRAW,
        D2D1::PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED)
        );

    // Create surface bitmap on which page content is drawn
    ComPtr<ID2D1Bitmap1> d2dSurfaceBitmap;
    DX::ThrowIfFailed(
        d2dContext->CreateBitmapFromDxgiSurface(
        dxgiSurface.Get(),
        &bitmapProperties,
        &d2dSurfaceBitmap
        )
        );

    ComPtr<ID2D1DrawingStateBlock>  stateBlock;
    DX::ThrowIfFailed(
        m_d2dFactory->CreateDrawingStateBlock(&stateBlock)
        );

    d2dContext->SaveDrawingState(stateBlock.Get());

    d2dContext->BeginDraw();
    d2dContext->SetTarget(d2dSurfaceBitmap.Get());

    d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::White));

    // We use scale matrix to shrink the size for preview
    d2dContext->SetTransform(D2D1::Matrix3x2F(1/scale, 0, 0, 1/scale, 0, 0));

    D2D_RECT_F drawRect = D2D1::RectF((width * scale / 2.0f) - (m_previewSize.width / 2.0f),
        (height * scale / 2.0f) - (m_previewSize.height / 2.0f),
        (width * scale / 2.0f) - (m_previewSize.width / 2.0f) + m_previewSize.width,
        (height * scale / 2.0f) - (m_previewSize.height / 2.0f) + m_previewSize.height);
    d2dContext->DrawBitmap(m_previewBitmap.Get(), &drawRect);

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    d2dContext->RestoreDrawingState(stateBlock.Get());

    // Must pass the same DPI as used to create the DXGI surface for the correct print preview
    DX::ThrowIfFailed(
        previewTarget->DrawPage(
        desiredJobPage,
        dxgiSurface.Get(),
        m_dpi,
        m_dpi
        )
        );
}

//
// Please prepare your own image for the print preview.
//
InMemoryRandomAccessStream^ PageRenderer3D::GetPageImage()
{
    if (m_previewBitmap == NULL)
    {
        PreparePageImage();
    }
    return m_previewImageStream;
}

void PageRenderer3D::PreparePageImage()
{
    ComPtr<IWICBitmapDecoder> wicBitmapDecoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
        L"preview.png",
        nullptr,
        GENERIC_READ,
        WICDecodeMetadataCacheOnDemand,
        &wicBitmapDecoder
        )
        );

    ComPtr<IWICBitmapFrameDecode> wicBitmapFrame;
    DX::ThrowIfFailed(
        wicBitmapDecoder->GetFrame(0, &wicBitmapFrame)
        );

    ComPtr<IWICFormatConverter> wicFormatConverter;
    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&wicFormatConverter)
        );

    DX::ThrowIfFailed(
        wicFormatConverter->Initialize(
        wicBitmapFrame.Get(),
        GUID_WICPixelFormat32bppPBGRA,
        WICBitmapDitherTypeNone,
        nullptr,
        0.0,
        WICBitmapPaletteTypeCustom  // the BGRA format has no palette so this value is ignored
        )
        );

    double dpi = m_dpi;
    DX::ThrowIfFailed(
        wicFormatConverter->GetResolution(&dpi, &dpi)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmapFromWicBitmap(
        wicFormatConverter.Get(),
        BitmapProperties(
        PixelFormat(DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED),
        m_dpi,
        m_dpi
        ),
        &m_previewBitmap
        )
        );

    m_previewSize = m_previewBitmap->GetSize();

    //
    // Now convert the D2D bitmap image to a PNG stream
    //
    ComPtr<IWICBitmap> pWICBitmap = reinterpret_cast<IWICBitmap *>(wicFormatConverter.Get());
    ComPtr<IWICBitmapEncoder> pEncoder;
    ComPtr<IWICBitmapFrameEncode> pFrameEncode;
    ComPtr<IWICStream> pStream;
    ComPtr<IStream> pIStream;

    // Create a WIC stream to write our PNG to.
    DX::ThrowIfFailed(
        m_wicFactory->CreateStream(&pStream)
        );

    // Create an IStream to ultimately store the PNG
    m_previewImageStream = ref new InMemoryRandomAccessStream();
    m_previewImageStream->Seek(0);

    DX::ThrowIfFailed(
        CreateStreamOverRandomAccessStream( reinterpret_cast<IUnknown*>(m_previewImageStream), IID_PPV_ARGS(&pIStream))
        );

    // Bridge the WIC stream to the IStream
    DX::ThrowIfFailed(
        pStream->InitializeFromIStream(pIStream.Get())
        );

    // Create a PNG encoder
    DX::ThrowIfFailed(
        m_wicFactory->CreateEncoder(GUID_ContainerFormatPng, NULL, &pEncoder)
        );

    // Initialize the encoder
    DX::ThrowIfFailed(
        pEncoder->Initialize(pStream.Get(), WICBitmapEncoderNoCache)
        );

    // Create a frame encoder
    DX::ThrowIfFailed(
        pEncoder->CreateNewFrame(&pFrameEncode, NULL)
        );

    // Initialize the frame encoder
    DX::ThrowIfFailed(
        pFrameEncode->Initialize(NULL)
        );

    // Set the size of the frame to match our image size
    DX::ThrowIfFailed(
        pFrameEncode->SetSize(static_cast<UINT>(m_previewSize.width), static_cast<UINT>(m_previewSize.height))
        );

    // Set the pixel format
    WICPixelFormatGUID pixelFormat = GUID_WICPixelFormat32bppPBGRA;
    DX::ThrowIfFailed(
        pFrameEncode->SetPixelFormat(&pixelFormat)
        );

    // Encode the WIC bitmap to the PNG frame
    DX::ThrowIfFailed(
        pFrameEncode->WriteSource(pWICBitmap.Get(), NULL)
        );

    // Commit the frame
    DX::ThrowIfFailed(
        pFrameEncode->Commit()
        );

    // Commit the image
    DX::ThrowIfFailed(
        pEncoder->Commit()
        );
}

void PageRenderer3D::GetPageImageSize(
    _Out_ float32* width,
    _Out_ float32* height
    )
{
    *width = m_previewSize.width;
    *height = m_previewSize.height;
}
