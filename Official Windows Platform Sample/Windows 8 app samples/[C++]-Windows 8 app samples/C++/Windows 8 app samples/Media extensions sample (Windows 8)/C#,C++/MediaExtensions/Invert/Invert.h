// Defines the transform class.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once

#include <new>
#include <mfapi.h>
#include <mftransform.h>
#include <mfidl.h>
#include <mferror.h>
#include <strsafe.h>
#include <assert.h>
#include <wrl\implements.h>
#include <wrl\module.h>
#include <wrl\client.h>
#include <windows.media.h>
#include <D3D11.h>

#include "DirectXVideoTransform.h"
#include "InvertTransform.h"

using namespace Microsoft::WRL;

// CLSID of the MFT.
// {F1BF74F0-65F4-4F47-925C-E8B564A9A87E}
DEFINE_GUID(CLSID_InvertMFT, 
0xf1bf74f0, 0x65f4, 0x4f47, 0x92, 0x5c, 0xe8, 0xb5, 0x64, 0xa9, 0xa8, 0x7e);

class CInvert
    : public RuntimeClass<
           RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >, 
           ABI::Windows::Media::IMediaExtension,
           IMFTransform >
{
    InspectableClass(RuntimeClass_InvertTransform_InvertEffect, BaseTrust)

public:
    CInvert();
    ~CInvert();

    STDMETHOD(RuntimeClassInitialize)();

    // IMediaExtension
    STDMETHODIMP SetProperties(ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration);

    // IMFTransform
    STDMETHODIMP GetStreamLimits(
        DWORD   *pdwInputMinimum,
        DWORD   *pdwInputMaximum,
        DWORD   *pdwOutputMinimum,
        DWORD   *pdwOutputMaximum
    );

    STDMETHODIMP GetStreamCount(
        DWORD   *pcInputStreams,
        DWORD   *pcOutputStreams
    );

    STDMETHODIMP GetInputStreamInfo(
        DWORD                     dwInputStreamID,
        MFT_INPUT_STREAM_INFO *   pStreamInfo
    );

    STDMETHODIMP GetOutputStreamInfo(
        DWORD                     dwOutputStreamID,
        MFT_OUTPUT_STREAM_INFO *  pStreamInfo
    );

    STDMETHODIMP GetAttributes(IMFAttributes** pAttributes);

    STDMETHODIMP GetInputAvailableType(
        DWORD           dwInputStreamID,
        DWORD           dwTypeIndex, // 0-based
        IMFMediaType    **ppType
    );

    STDMETHODIMP GetOutputAvailableType(
        DWORD           dwOutputStreamID,
        DWORD           dwTypeIndex, // 0-based
        IMFMediaType    **ppType
    );

    STDMETHODIMP SetInputType(
        DWORD           dwInputStreamID,
        IMFMediaType    *pType,
        DWORD           dwFlags
    );

    STDMETHODIMP SetOutputType(
        DWORD           dwOutputStreamID,
        IMFMediaType    *pType,
        DWORD           dwFlags
    );

    STDMETHODIMP GetInputCurrentType(
        DWORD           dwInputStreamID,
        IMFMediaType    **ppType
    );

    STDMETHODIMP GetOutputCurrentType(
        DWORD           dwOutputStreamID,
        IMFMediaType    **ppType
    );

    STDMETHODIMP GetInputStatus(
        DWORD           dwInputStreamID,
        DWORD           *pdwFlags
    );

    STDMETHODIMP GetOutputStatus(DWORD *pdwFlags);

    STDMETHODIMP ProcessMessage(
        MFT_MESSAGE_TYPE    eMessage,
        ULONG_PTR           ulParam
    );

    STDMETHODIMP ProcessInput(
        DWORD               dwInputStreamID,
        IMFSample           *pSample,
        DWORD               dwFlags
    );

    STDMETHODIMP ProcessOutput(
        DWORD                   dwFlags,
        DWORD                   cOutputBufferCount,
        MFT_OUTPUT_DATA_BUFFER  *pOutputSamples, // one per stream
        DWORD                   *pdwStatus
    );

    STDMETHODIMP GetOutputStreamAttributes(
        DWORD           dwOutputStreamID,
        IMFAttributes   **ppAttributes
        );

    // Optional Methods
    STDMETHODIMP GetStreamIDs(
        DWORD   dwInputIDArraySize,
        DWORD   *pdwInputIDs,
        DWORD   dwOutputIDArraySize,
        DWORD   *pdwOutputIDs
        ) { return E_NOTIMPL; }

    STDMETHODIMP GetInputStreamAttributes(
        DWORD           dwInputStreamID,
        IMFAttributes   **ppAttributes
        ) { return E_NOTIMPL; }

    STDMETHODIMP DeleteInputStream(DWORD dwStreamID){ return E_NOTIMPL; }

    STDMETHODIMP AddInputStreams(
        DWORD   cStreams,
        DWORD   *adwStreamIDs
        ) { return E_NOTIMPL; }

    STDMETHODIMP SetOutputBounds(
        LONGLONG        hnsLowerBound,
        LONGLONG        hnsUpperBound
        ) { return E_NOTIMPL; }

    STDMETHODIMP ProcessEvent(
        DWORD              dwInputStreamID,
        IMFMediaEvent      *pEvent
        ) { return E_NOTIMPL; }

private:

    HRESULT OnGetPartialType(DWORD dwTypeIndex, IMFMediaType **ppmt);

    HRESULT OnCheckInputType(IMFMediaType *pmt);
    HRESULT OnCheckOutputType(IMFMediaType *pmt);
    HRESULT OnCheckMediaType(IMFMediaType *pmt);
    HRESULT BeginStreaming();
    HRESULT OnProcessOutput(IMFMediaBuffer *pIn, IMFMediaBuffer *pOut);
    HRESULT UpdateFormatInfo();
    HRESULT UpdateDX11Device();
    HRESULT CheckDX11Device();
    void InvalidateDX11Resources();

    CRITICAL_SECTION            m_critSec;

    // Streaming
    bool                        m_bStreamingInitialized;
    ComPtr<IMFSample>           m_pSample;                 // Input sample.
    ComPtr<IMFMediaType>        m_pInputType;              // Input media type.
    ComPtr<IMFMediaType>        m_pOutputType;             // Output media type.

    // Fomat information
    UINT32                      m_imageWidthInPixels;
    UINT32                      m_imageHeightInPixels;
    DWORD                       m_cbImageSize;              // Image size, in bytes.

    ComPtr<IMFAttributes>       m_pAttributes;

    // DirectX manager
    ComPtr<IMFDXGIDeviceManager> m_pDX11Manager;
    HANDLE m_hDeviceHandle;

    // Device resources
    ComPtr<ID3D11Device> m_pDevice;
    ComPtr<ID3D11DeviceContext> m_pContext;
    ComPtr<ID3D11Texture2D> m_pInBufferTex;
    ComPtr<ID3D11Texture2D> m_pOutBufferTex;
    ComPtr<ID3D11Texture2D> m_pOutBufferStage;

    // Output
    ComPtr<IMFVideoSampleAllocatorEx> m_spOutputSampleAllocator;
    ComPtr<IMFAttributes> m_spOutputAttributes;

    // Transform
    DirectXVideoTransform *m_pTransform;
};
