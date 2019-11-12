//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************
#pragma once

//-----------------------------------------------------------------------------
// Constant buffer data
//-----------------------------------------------------------------------------
struct ModelViewProjectionConstantBuffer
{
    DirectX::XMFLOAT4X4 model;
    DirectX::XMFLOAT4X4 view;
    DirectX::XMFLOAT4X4 projection;
};

//-----------------------------------------------------------------------------
// Per-vertex data
//-----------------------------------------------------------------------------
struct VertexPositionColor
{
    DirectX::XMFLOAT3 pos;
    DirectX::XMFLOAT3 color;
};


namespace DXSurfaceGenerator
{
    public ref class SampleGenerator sealed
    {
    public:
        SampleGenerator();
        virtual ~SampleGenerator();
        void Initialize(Windows::Media::Core::MediaStreamSource ^ mss, Windows::Media::Core::VideoStreamDescriptor ^ videoDesc);
        void GenerateSample(Windows::Media::Core::MediaStreamSourceSampleRequest ^ request);

    private:
        HRESULT GenerateCubeFrame(UINT32 ui32Width, UINT32 ui32Height, int iFrameRotation, IMFMediaBuffer * ppBuffer);
        HRESULT ConvertPropertiesToMediaType(_In_ Windows::Media::MediaProperties::IMediaEncodingProperties ^mep, _Outptr_ IMFMediaType **ppMT);
        static HRESULT AddAttribute(_In_ GUID guidKey, _In_ Windows::Foundation::IPropertyValue ^value, _In_ IMFAttributes *pAttr);
        HRESULT InitializeDirectXObjects();

        Microsoft::WRL::ComPtr<IMFVideoSampleAllocator> _spSampleAllocator;
        Microsoft::WRL::ComPtr<IMFDXGIDeviceManager> _spDeviceManager;
        Microsoft::WRL::ComPtr<ID3D11Device> _spD3DDevice;
        Microsoft::WRL::ComPtr<ID3D11DeviceContext> _spD3DContext;
        HANDLE _hDevice;
        Platform::Array<byte>^ _vertexShaderData;
        Platform::Array<byte>^ _pixelShaderData;
        ULONGLONG _ulVideoTimestamp;
        int _iVideoFrameNumber;
        Platform::Array<unsigned short>^ _rgCubeIndexes;

        // DirectX Objects
        ModelViewProjectionConstantBuffer _constantBufferData;
        Microsoft::WRL::ComPtr<ID3D11Buffer> _spConstantBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer> _spVertexBuffer;
        Microsoft::WRL::ComPtr<ID3D11Buffer> _spIndexBuffer;
        Microsoft::WRL::ComPtr<ID3D11InputLayout> _spInputLayout;
        Microsoft::WRL::ComPtr<ID3D11PixelShader> _spPixelShader;
        Microsoft::WRL::ComPtr<ID3D11VertexShader> _spVertexShader;
    };
}