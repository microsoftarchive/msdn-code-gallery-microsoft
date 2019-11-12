//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "SampleGenerator.h"
#include "BasicReaderWriter.h"

using namespace DXSurfaceGenerator;
using namespace Platform;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::Media::Core;
using namespace Windows::Media::MediaProperties;
using namespace DirectX;

SampleGenerator::SampleGenerator() :
    _hDevice(nullptr),
    _ulVideoTimestamp(0),
    _iVideoFrameNumber(0),
    _rgCubeIndexes(nullptr)
{
    // Create the View Matrix
    // Create the constant buffer data in system memory.
    XMVECTOR eye = XMVectorSet(0.0f, 0.7f, 1.5f, 0.0f);
    XMVECTOR at = XMVectorSet(0.0f, -0.1f, 0.0f, 0.0f);
    XMVECTOR up = XMVectorSet(0.0f, 1.0f, 0.0f, 0.0f);

    XMStoreFloat4x4(
        &_constantBufferData.view,
        XMMatrixTranspose(XMMatrixLookAtRH(eye, at, up))
        );
}

SampleGenerator::~SampleGenerator()
{
    if (_spDeviceManager.Get() != nullptr && _hDevice != nullptr)
    {
        _spDeviceManager->CloseDeviceHandle(_hDevice);
        _hDevice = nullptr;
    }

    if (_spSampleAllocator != nullptr)
    {
        _spSampleAllocator->UninitializeSampleAllocator();
    }
}

void SampleGenerator::Initialize(MediaStreamSource ^ mss, VideoStreamDescriptor ^ videoDesc)
{
    ComPtr<IMFDXGIDeviceManagerSource> spDeviceManagerSource;

    HRESULT hr = reinterpret_cast<IInspectable*>(mss)->QueryInterface(spDeviceManagerSource.GetAddressOf());

    if (_spDeviceManager == nullptr)
    {

        if (SUCCEEDED(hr))
        {
            hr = spDeviceManagerSource->GetManager(_spDeviceManager.ReleaseAndGetAddressOf());
            if (FAILED(hr))
            {
                UINT uiToken = 0;
                hr = MFCreateDXGIDeviceManager(&uiToken, _spDeviceManager.ReleaseAndGetAddressOf());
                if (SUCCEEDED(hr))
                {
                    ComPtr<ID3D11Device> spDevice;
                    D3D_FEATURE_LEVEL maxSupportedLevelByDevice = D3D_FEATURE_LEVEL_9_1;
                    D3D_FEATURE_LEVEL rgFeatureLevels[] = {
                        D3D_FEATURE_LEVEL_11_1,
                        D3D_FEATURE_LEVEL_11_0,
                        D3D_FEATURE_LEVEL_10_1,
                        D3D_FEATURE_LEVEL_10_0,
                        D3D_FEATURE_LEVEL_9_3,
                        D3D_FEATURE_LEVEL_9_2,
                        D3D_FEATURE_LEVEL_9_1
                    };

                    hr = D3D11CreateDevice(nullptr,
                        D3D_DRIVER_TYPE_WARP,
                        nullptr,
                        D3D11_CREATE_DEVICE_BGRA_SUPPORT,
                        rgFeatureLevels,
                        ARRAYSIZE(rgFeatureLevels),
                        D3D11_SDK_VERSION,
                        spDevice.ReleaseAndGetAddressOf(),
                        &maxSupportedLevelByDevice,
                        nullptr);

                    if (SUCCEEDED(hr))
                    {
                        hr = _spDeviceManager->ResetDevice(spDevice.Get(), uiToken);
                    }
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = _spDeviceManager->OpenDeviceHandle(&_hDevice);
        }

        if (SUCCEEDED(hr))
        {
            BasicReaderWriter^ readerWriter = ref new BasicReaderWriter();

            // Load the Compiled Shader Data
            _vertexShaderData = readerWriter->ReadData("CubeVertexShader.cso");
            _pixelShaderData = readerWriter->ReadData("CubePixelShader.cso");
            hr = InitializeDirectXObjects();
        }
    }

    if (SUCCEEDED(hr) && _spSampleAllocator == nullptr)
    {
        ComPtr<IMFMediaType> spVideoMT;
        if (SUCCEEDED(hr))
        {
            hr = ConvertPropertiesToMediaType(videoDesc->EncodingProperties, spVideoMT.GetAddressOf());
        }

        if (SUCCEEDED(hr))
        {
            hr = MFCreateVideoSampleAllocatorEx(IID_PPV_ARGS(_spSampleAllocator.ReleaseAndGetAddressOf()));
        }

        if (SUCCEEDED(hr))
        {
            hr = _spSampleAllocator->SetDirectXManager(_spDeviceManager.Get());
        }

        if (SUCCEEDED(hr))
        {
            hr = _spSampleAllocator->InitializeSampleAllocator(60, spVideoMT.Get());
        }
    }

    // We reset each time we are initialized
    _ulVideoTimestamp = 0;

    if (FAILED(hr))
    {
        throw Platform::Exception::CreateException(hr, L"Unable to initialize resources for the sample generator.");
    }
}

HRESULT SampleGenerator::AddAttribute(_In_ GUID guidKey, _In_ IPropertyValue ^value, _In_ IMFAttributes *pAttr)
{
    HRESULT hr = (value != nullptr && pAttr != nullptr) ? S_OK : E_INVALIDARG;
    if (SUCCEEDED(hr))
    {
        PropertyType type = value->Type;
        switch (type)
        {
        case PropertyType::UInt8Array:
        {
            Platform::Array<BYTE>^ arr;
            value->GetUInt8Array(&arr);

            hr = (pAttr->SetBlob(guidKey, arr->Data, arr->Length));
        }
            break;

        case PropertyType::Double:
        {
            hr = (pAttr->SetDouble(guidKey, value->GetDouble()));
        }
            break;

        case PropertyType::Guid:
        {
            hr = (pAttr->SetGUID(guidKey, value->GetGuid()));
        }
            break;

        case PropertyType::String:
        {
            hr = (pAttr->SetString(guidKey, value->GetString()->Data()));
        }
            break;

        case PropertyType::UInt32:
        {
            hr = (pAttr->SetUINT32(guidKey, value->GetUInt32()));
        }
            break;

        case PropertyType::UInt64:
        {
            hr = (pAttr->SetUINT64(guidKey, value->GetUInt64()));
        }
            break;

            // ignore unknown values
        }
    }
    return hr;
}

HRESULT SampleGenerator::ConvertPropertiesToMediaType(_In_ IMediaEncodingProperties ^mep, _Outptr_ IMFMediaType **ppMT)
{
    HRESULT hr = (mep != nullptr && ppMT != nullptr) ? S_OK : E_INVALIDARG;
    ComPtr<IMFMediaType> spMT;
    if (SUCCEEDED(hr))
    {
        *ppMT = nullptr;
        hr = MFCreateMediaType(&spMT);
    }

    if (SUCCEEDED(hr))
    {
        auto it = mep->Properties->First();

        while (SUCCEEDED(hr) && it->HasCurrent)
        {
            auto currentValue = it->Current;
            hr = AddAttribute(currentValue->Key, safe_cast<IPropertyValue^>(currentValue->Value), spMT.Get());
            it->MoveNext();
        }

        if (SUCCEEDED(hr))
        {
            GUID guiMajorType = safe_cast<IPropertyValue^>(mep->Properties->Lookup(MF_MT_MAJOR_TYPE))->GetGuid();

            if (guiMajorType != MFMediaType_Video && guiMajorType != MFMediaType_Audio)
            {
                hr = E_UNEXPECTED;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        *ppMT = spMT.Detach();
    }

    return hr;
}

void SampleGenerator::GenerateSample(MediaStreamSourceSampleRequest ^ request)
{
    ComPtr<IMFMediaStreamSourceSampleRequest> spRequest;
    VideoEncodingProperties^ spEncodingProperties;
    HRESULT hr = (request != nullptr) ? S_OK : E_POINTER;
    UINT32 ui32Height = 0;
    UINT32 ui32Width = 0;
    ULONGLONG ulTimeSpan = 0;

    if (SUCCEEDED(hr))
    {
        hr = reinterpret_cast<IInspectable*>(request)->QueryInterface(spRequest.ReleaseAndGetAddressOf());
    }

    // Make sure we are on the same device if not then reset the device
    if (SUCCEEDED(hr))
    {
        if (FAILED(_spDeviceManager->TestDevice(_hDevice)))
        {
            _spDeviceManager->CloseDeviceHandle(_hDevice);
            hr = _spDeviceManager->OpenDeviceHandle(&_hDevice);
            if (SUCCEEDED(hr))
            {
                hr = InitializeDirectXObjects();
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        VideoStreamDescriptor^ spVideoStreamDescriptor;
        spVideoStreamDescriptor = dynamic_cast<VideoStreamDescriptor^>(request->StreamDescriptor);
        if (spVideoStreamDescriptor != nullptr)
        {
            spEncodingProperties = spVideoStreamDescriptor->EncodingProperties;
        }
        else
        {
            throw Platform::Exception::CreateException(E_INVALIDARG, L"Media Request is not for an video sample.");
        }
    }

    if (SUCCEEDED(hr))
    {
        ui32Height = spEncodingProperties->Height;
    }

    if (SUCCEEDED(hr))
    {
        ui32Width = spEncodingProperties->Width;
    }

    if (SUCCEEDED(hr))
    {
        MediaRatio^ spRatio = spEncodingProperties->FrameRate;
        if (SUCCEEDED(hr))
        {
            UINT32 ui32Numerator = spRatio->Numerator;
            UINT32 ui32Denominator = spRatio->Denominator;
            if (ui32Numerator != 0)
            {
                ulTimeSpan = ((ULONGLONG)ui32Denominator) * 10000000 / ui32Numerator;
            }
            else
            {
                hr = E_INVALIDARG;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        ComPtr<IMFMediaBuffer> spBuffer;
        ComPtr<IMFSample> spSample;
        hr = _spSampleAllocator->AllocateSample(spSample.GetAddressOf());

        if (SUCCEEDED(hr))
        {
            hr = spSample->SetSampleDuration(ulTimeSpan);
        }

        if (SUCCEEDED(hr))
        {
            hr = spSample->SetSampleTime((LONGLONG)_ulVideoTimestamp);
        }

        if (SUCCEEDED(hr))
        {
            hr = spSample->GetBufferByIndex(0, spBuffer.GetAddressOf());
        }

        if (SUCCEEDED(hr))
        {
            hr = GenerateCubeFrame(ui32Width, ui32Height, _iVideoFrameNumber, spBuffer.Get());
        }

        if (SUCCEEDED(hr))
        {
            hr = spRequest->SetSample(spSample.Get());
        }

        if (SUCCEEDED(hr))
        {
            ++_iVideoFrameNumber;
            _ulVideoTimestamp += ulTimeSpan;
        }
    }

    if (FAILED(hr))
    {
        throw Platform::Exception::CreateException(hr);
    }
}

HRESULT SampleGenerator::GenerateCubeFrame(UINT32 ui32Width, UINT32 ui32Height, int iFrameRotation, IMFMediaBuffer * pBuffer)
{
    ComPtr<IMFDXGIBuffer> spDXGIBuffer;
    HRESULT hr = (pBuffer != nullptr) ? S_OK : E_INVALIDARG;

    if (SUCCEEDED(hr))
    {
        hr = pBuffer->QueryInterface(spDXGIBuffer.GetAddressOf());
    }

    if (SUCCEEDED(hr))
    {
        hr = _spDeviceManager->LockDevice(_hDevice, IID_PPV_ARGS(_spD3DDevice.ReleaseAndGetAddressOf()), TRUE);

        if (SUCCEEDED(hr))
        {
            ComPtr<ID3D11Texture2D> spTex2DOutputBuffer;

            _spD3DDevice->GetImmediateContext(_spD3DContext.ReleaseAndGetAddressOf());
            hr = spDXGIBuffer->GetResource(IID_PPV_ARGS(spTex2DOutputBuffer.GetAddressOf()));

            if (SUCCEEDED(hr))
            {
                CD3D11_VIEWPORT viewport(
                    0.0f,
                    0.0f,
                    static_cast<float>(ui32Width),
                    static_cast<float>(ui32Height)
                    );

                _spD3DContext->RSSetViewports(1, &viewport);

                // Set up the perspective matrix
                float aspectRatio = (float)ui32Width / (float)ui32Height;
                float fovAngleY = 70.0f * XM_PI / 180.0f;

                XMStoreFloat4x4(
                    &_constantBufferData.projection,
                    XMMatrixTranspose(
                    XMMatrixPerspectiveFovRH(
                    fovAngleY,
                    aspectRatio,
                    0.01f,
                    100.0f
                    )
                    )
                    );


                ComPtr<ID3D11RenderTargetView> spD3DRenderTarget;
                XMStoreFloat4x4(
                    &_constantBufferData.model,
                    XMMatrixTranspose(XMMatrixRotationY(((float)(iFrameRotation % 360)) * XM_PI / 180.f))
                    );

                hr = _spD3DDevice->CreateRenderTargetView(spTex2DOutputBuffer.Get(), nullptr, spD3DRenderTarget.GetAddressOf());
                if (SUCCEEDED(hr))
                {
                    const float midnightBlue[] = { 0.098f, 0.098f, 0.439f, 1.000f };
                    _spD3DContext->ClearRenderTargetView(spD3DRenderTarget.Get(), midnightBlue);
                    _spD3DContext->OMSetRenderTargets(1, spD3DRenderTarget.GetAddressOf(), nullptr);
                    _spD3DContext->UpdateSubresource(_spConstantBuffer.Get(), 0, nullptr, &_constantBufferData, 0, 0);

                    UINT stride = sizeof(VertexPositionColor);
                    UINT offset = 0;

                    _spD3DContext->IASetVertexBuffers(
                        0,  // start with the first vertex buffer
                        1,  // one vertex buffer
                        _spVertexBuffer.GetAddressOf(),
                        &stride,
                        &offset
                        );

                    _spD3DContext->IASetIndexBuffer(
                        _spIndexBuffer.Get(),
                        DXGI_FORMAT_R16_UINT,
                        0   // no offset
                        );

                    _spD3DContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
                    _spD3DContext->IASetInputLayout(_spInputLayout.Get());


                    // Set the vertex shader.
                    _spD3DContext->VSSetShader(
                        _spVertexShader.Get(),
                        nullptr,
                        0
                        );

                    // Set the vertex shader constant buffer data.
                    _spD3DContext->VSSetConstantBuffers(
                        0,  // register 0
                        1,  // one constant buffer
                        _spConstantBuffer.GetAddressOf()
                        );


                    // Set the pixel shader.
                    _spD3DContext->PSSetShader(
                        _spPixelShader.Get(),
                        nullptr,
                        0
                        );


                    // Draw the cube.
                    _spD3DContext->DrawIndexed(
                        _rgCubeIndexes->Length,
                        0,  // start with index 0
                        0   // start with vertex 0
                        );

                }
            }
            _spDeviceManager->UnlockDevice(_hDevice, TRUE);
        }
    }

    return hr;
}


HRESULT SampleGenerator::InitializeDirectXObjects()
{
    // Load the resources
    
    // Cube position index buffer:
    unsigned short cubeIndexes[] = {
        0, 2, 1, // -x
        1, 2, 3,

        4, 5, 6, // +x
        5, 7, 6,

        0, 1, 5, // -y
        0, 5, 4,

        2, 6, 7, // +y
        2, 7, 3,

        0, 4, 6, // -z
        0, 6, 2,

        1, 3, 7, // +z
        1, 7, 5,
    };

    HRESULT hr = S_OK;

    _rgCubeIndexes = ref new Platform::Array<unsigned short>(ARRAYSIZE(cubeIndexes));
    if (_rgCubeIndexes != nullptr)
    {
        memcpy(_rgCubeIndexes->Data, cubeIndexes, sizeof(cubeIndexes));
    }
    else
    {
        hr = E_OUTOFMEMORY;
    }
    
    if (SUCCEEDED(hr))
    {
        hr = _spDeviceManager->LockDevice(_hDevice, IID_PPV_ARGS(_spD3DDevice.ReleaseAndGetAddressOf()), TRUE);

        if (SUCCEEDED(hr))
        {
            hr = _spD3DDevice->CreateVertexShader(_vertexShaderData->Data, _vertexShaderData->Length, nullptr, _spVertexShader.ReleaseAndGetAddressOf());
            if (SUCCEEDED(hr))
            {
                hr = _spD3DDevice->CreatePixelShader(_pixelShaderData->Data, _pixelShaderData->Length, nullptr, _spPixelShader.ReleaseAndGetAddressOf());
            }

            if (SUCCEEDED(hr))
            {
                const D3D11_INPUT_ELEMENT_DESC vertexDesc[] =
                {
                    { "POSITION", 0, DXGI_FORMAT_R32G32B32_FLOAT,
                    0, 0, D3D11_INPUT_PER_VERTEX_DATA, 0 },

                    { "COLOR", 0, DXGI_FORMAT_R32G32B32_FLOAT,
                    0, 12, D3D11_INPUT_PER_VERTEX_DATA, 0 },
                };

                hr = _spD3DDevice->CreateInputLayout(vertexDesc, ARRAYSIZE(vertexDesc), _vertexShaderData->Data, _vertexShaderData->Length, _spInputLayout.ReleaseAndGetAddressOf());
            }

            if (SUCCEEDED(hr))
            {
                CD3D11_BUFFER_DESC constantBufferDesc(
                    sizeof(ModelViewProjectionConstantBuffer),
                    D3D11_BIND_CONSTANT_BUFFER
                    );

                hr = _spD3DDevice->CreateBuffer(&constantBufferDesc, nullptr, _spConstantBuffer.ReleaseAndGetAddressOf());
            }

            // Create the cube
            if (SUCCEEDED(hr))
            {
                // Create vertex buffer to store cube geometry:
                VertexPositionColor CubeVertices[] =
                {
                    { XMFLOAT3(-0.5f, -0.5f, -0.5f), XMFLOAT3(0.0f, 0.0f, 0.0f) },
                    { XMFLOAT3(-0.5f, -0.5f, 0.5f), XMFLOAT3(0.0f, 0.0f, 1.0f) },
                    { XMFLOAT3(-0.5f, 0.5f, -0.5f), XMFLOAT3(0.0f, 1.0f, 0.0f) },
                    { XMFLOAT3(-0.5f, 0.5f, 0.5f), XMFLOAT3(0.0f, 1.0f, 1.0f) },

                    { XMFLOAT3(0.5f, -0.5f, -0.5f), XMFLOAT3(1.0f, 0.0f, 0.0f) },
                    { XMFLOAT3(0.5f, -0.5f, 0.5f), XMFLOAT3(1.0f, 0.0f, 1.0f) },
                    { XMFLOAT3(0.5f, 0.5f, -0.5f), XMFLOAT3(1.0f, 1.0f, 0.0f) },
                    { XMFLOAT3(0.5f, 0.5f, 0.5f), XMFLOAT3(1.0f, 1.0f, 1.0f) },
                };

                // The buffer description tells Direct3D how to create this buffer.
                D3D11_SUBRESOURCE_DATA vertexBufferData = { 0 };
                vertexBufferData.pSysMem = CubeVertices;
                vertexBufferData.SysMemPitch = 0;
                vertexBufferData.SysMemSlicePitch = 0;
                CD3D11_BUFFER_DESC vertexBufferDesc(
                    sizeof(CubeVertices),
                    D3D11_BIND_VERTEX_BUFFER);

                // This call allocates a device resource for the vertex buffer and copies
                // in the data.
                hr = _spD3DDevice->CreateBuffer(&vertexBufferDesc, &vertexBufferData, _spVertexBuffer.ReleaseAndGetAddressOf());
            }

            if (SUCCEEDED(hr))
            {
                // Create index buffer:
                D3D11_SUBRESOURCE_DATA indexBufferData = { 0 };
                indexBufferData.pSysMem = _rgCubeIndexes->Data;
                indexBufferData.SysMemPitch = 0;
                indexBufferData.SysMemSlicePitch = 0;
                CD3D11_BUFFER_DESC indexBufferDesc(
                    _rgCubeIndexes->Length * sizeof(unsigned short),
                    D3D11_BIND_INDEX_BUFFER);

                // This call allocates a device resource for the index buffer and copies
                // in the data.
                hr = _spD3DDevice->CreateBuffer(
                    &indexBufferDesc,
                    &indexBufferData,
                    _spIndexBuffer.ReleaseAndGetAddressOf()
                    );
            }

            _spDeviceManager->UnlockDevice(_hDevice, TRUE);
        }
    }

    return hr;
}