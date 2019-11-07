// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#include "Invert.h"
#include "VideoBufferLock.h"
#include "TextureLock.h"

#include "InvertModule.h"
#include "CopyModule.h"

using namespace Microsoft::WRL;

HRESULT GetDefaultStride(IMFMediaType *pType, LONG *plStride);
HRESULT BufferToDXType(IMFMediaBuffer *pBuffer, _Out_ UINT *uiViewIndex, _Out_ ID3D11Texture2D **ppTexture);

CInvert::CInvert() :
    m_imageWidthInPixels(0), 
    m_imageHeightInPixels(0), 
    m_cbImageSize(0),
    m_bStreamingInitialized(false)
{
    InitializeCriticalSectionEx(&m_critSec, 3000, 0);
}

CInvert::~CInvert()
{
    DeleteCriticalSection(&m_critSec);
    if( m_pTransform )
    {
        delete m_pTransform;
    }
}

// Initialize the instance.
STDMETHODIMP CInvert::RuntimeClassInitialize()
{
    HRESULT hr = S_OK;

    // Create the attribute store.
    hr = MFCreateAttributes(&m_pAttributes, 3);
    if (FAILED(hr))
    {
        goto done;
    }

    // MFT supports DX11 acceleration
    hr = m_pAttributes->SetUINT32(MF_SA_D3D_AWARE, 1);
    if (FAILED(hr))
    {
        goto done;
    }

    hr = m_pAttributes->SetUINT32(MF_SA_D3D11_AWARE, 1);
    if (FAILED(hr))
    {
        goto done;
    }

    // output attributes
    hr = MFCreateAttributes( &m_spOutputAttributes, 1 );
    if (FAILED(hr))
    {
        goto done;
    }

    // Load the transform
    m_pTransform = new CInvertModule();
    if( m_pTransform == nullptr )
    {
        hr = E_OUTOFMEMORY;
        goto done;
    }

done:
    return hr;
}

// IMediaExtension methods

//-------------------------------------------------------------------
// SetProperties
// Sets the configuration of the effect
//-------------------------------------------------------------------
HRESULT CInvert::SetProperties(ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration)
{
    return S_OK;
}

//-------------------------------------------------------------------
// GetStreamLimits
// Returns the minimum and maximum number of streams.
//-------------------------------------------------------------------

HRESULT CInvert::GetStreamLimits(
    DWORD   *pdwInputMinimum,
    DWORD   *pdwInputMaximum,
    DWORD   *pdwOutputMinimum,
    DWORD   *pdwOutputMaximum
)
{
    if ((pdwInputMinimum == NULL) ||
        (pdwInputMaximum == NULL) ||
        (pdwOutputMinimum == NULL) ||
        (pdwOutputMaximum == NULL))
    {
        return E_POINTER;
    }

    // This MFT has a fixed number of streams.
    *pdwInputMinimum = 1;
    *pdwInputMaximum = 1;
    *pdwOutputMinimum = 1;
    *pdwOutputMaximum = 1;
    return S_OK;
}


//-------------------------------------------------------------------
// GetStreamCount
// Returns the actual number of streams.
//-------------------------------------------------------------------

HRESULT CInvert::GetStreamCount(
    DWORD   *pcInputStreams,
    DWORD   *pcOutputStreams
)
{
    if ((pcInputStreams == NULL) || (pcOutputStreams == NULL))
    {
        return E_POINTER;
    }

    // This MFT has a fixed number of streams.
    *pcInputStreams = 1;
    *pcOutputStreams = 1;
    return S_OK;
}


//-------------------------------------------------------------------
// GetInputStreamInfo
// Returns information about an input stream.
//-------------------------------------------------------------------

HRESULT CInvert::GetInputStreamInfo(
    DWORD                     dwInputStreamID,
    MFT_INPUT_STREAM_INFO *   pStreamInfo
)
{
    if (pStreamInfo == NULL)
    {
        return E_POINTER;
    }

    if (dwInputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    EnterCriticalSection(&m_critSec);

    pStreamInfo->hnsMaxLatency = 0;
    pStreamInfo->dwFlags = MFT_INPUT_STREAM_WHOLE_SAMPLES | MFT_INPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER;
    pStreamInfo->cbSize = m_pInputType == NULL ? 0 : m_cbImageSize;
    pStreamInfo->cbMaxLookahead = 0;
    pStreamInfo->cbAlignment = 0;

    LeaveCriticalSection(&m_critSec);
    return S_OK;
}

//-------------------------------------------------------------------
// GetOutputStreamInfo
// Returns information about an output stream.
//-------------------------------------------------------------------

HRESULT CInvert::GetOutputStreamInfo(
    DWORD                     dwOutputStreamID,
    MFT_OUTPUT_STREAM_INFO *  pStreamInfo
)
{
    if (pStreamInfo == NULL)
    {
        return E_POINTER;
    }

    if (dwOutputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    EnterCriticalSection(&m_critSec);

    pStreamInfo->dwFlags =
        MFT_OUTPUT_STREAM_WHOLE_SAMPLES |
        MFT_OUTPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER |
        MFT_OUTPUT_STREAM_FIXED_SAMPLE_SIZE ;

    if( m_pDX11Manager != nullptr )
    {
        pStreamInfo->dwFlags |= MFT_OUTPUT_STREAM_PROVIDES_SAMPLES;
    }

    pStreamInfo->cbSize = m_pInputType == NULL ? 0 : m_cbImageSize;
    pStreamInfo->cbAlignment = 0;

    LeaveCriticalSection(&m_critSec);
    return S_OK;
}

//-------------------------------------------------------------------
// Name: GetOutputStreamAttributes
// Returns stream-level attributes for an output stream.
//-------------------------------------------------------------------

HRESULT CInvert::GetOutputStreamAttributes(
    DWORD           dwOutputStreamID,
    IMFAttributes   **ppAttributes
    )
{
    HRESULT hr = S_OK;

    if (NULL == ppAttributes)
    {
        return E_POINTER;
    }

    if (dwOutputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    m_spOutputAttributes.CopyTo(ppAttributes);

    return( hr );
}

//-------------------------------------------------------------------
// GetAttributes
// Returns the attributes for the MFT.
//-------------------------------------------------------------------

HRESULT CInvert::GetAttributes(IMFAttributes** ppAttributes)
{
    if (ppAttributes == NULL)
    {
        return E_POINTER;
    }

    EnterCriticalSection(&m_critSec);

    m_pAttributes.CopyTo(ppAttributes);

    LeaveCriticalSection(&m_critSec);
    return S_OK;
}


//-------------------------------------------------------------------
// GetInputAvailableType
// Returns a preferred input type.
//-------------------------------------------------------------------

HRESULT CInvert::GetInputAvailableType(
    DWORD           dwInputStreamID,
    DWORD           dwTypeIndex, // 0-based
    IMFMediaType    **ppType
)
{
    if (ppType == NULL)
    {
        return E_INVALIDARG;
    }

    if (dwInputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    EnterCriticalSection(&m_critSec);
    HRESULT hr = S_OK;

    // If the output type is set, return that type as our preferred input type.
    if (m_pOutputType == NULL)
    {
        // The output type is not set. Create a partial media type.
        hr = OnGetPartialType(dwTypeIndex, ppType);
    }
    else if (dwTypeIndex > 0)
    {
        hr = MF_E_NO_MORE_TYPES;
    }
    else
    {
        *ppType = m_pOutputType.Get();
        (*ppType)->AddRef();
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}



//-------------------------------------------------------------------
// GetOutputAvailableType
// Returns a preferred output type.
//-------------------------------------------------------------------

HRESULT CInvert::GetOutputAvailableType(
    DWORD           dwOutputStreamID,
    DWORD           dwTypeIndex, // 0-based
    IMFMediaType    **ppType
)
{
    if (ppType == NULL)
    {
        return E_INVALIDARG;
    }

    if (dwOutputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    EnterCriticalSection(&m_critSec);
    HRESULT hr = S_OK;

    if (m_pInputType == NULL)
    {
        // The input type is not set. Create a partial media type.
        hr = OnGetPartialType(dwTypeIndex, ppType);
    }
    else if (dwTypeIndex > 0)
    {
        hr = MF_E_NO_MORE_TYPES;
    }
    else
    {
        *ppType = m_pInputType.Get();
        (*ppType)->AddRef();
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// SetInputType
//-------------------------------------------------------------------

HRESULT CInvert::SetInputType(
    DWORD           dwInputStreamID,
    IMFMediaType    *pType, // Can be NULL to clear the input type.
    DWORD           dwFlags
)
{
    // Validate flags.
    if (dwFlags & ~MFT_SET_TYPE_TEST_ONLY)
    {
        return E_INVALIDARG;
    }

    if (dwInputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    EnterCriticalSection(&m_critSec);
    HRESULT hr = S_OK;

    // If we have an input sample, the client cannot change the type now.
    if (m_pSample != NULL)
    {
        hr = MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING;
        goto done;
    }

    // Validate the type, if non-NULL.
    if (pType)
    {
        hr = OnCheckInputType(pType);
        if (FAILED(hr))
        {
            goto done;
        }
    }

    // The type is OK. Set the type, unless the caller was just testing.
    if ( (dwFlags & MFT_SET_TYPE_TEST_ONLY) == 0 )
    {
        m_pInputType = nullptr;
        m_pInputType = pType;

        // Update the format information.
        UpdateFormatInfo();

        // When the type changes, end streaming.
        m_bStreamingInitialized = false;
    }

done:
    LeaveCriticalSection(&m_critSec);
    return hr;
}



//-------------------------------------------------------------------
// SetOutputType
//-------------------------------------------------------------------

HRESULT CInvert::SetOutputType(
    DWORD           dwOutputStreamID,
    IMFMediaType    *pType, // Can be NULL to clear the output type.
    DWORD           dwFlags
)
{
    // Validate flags.
    if (dwFlags & ~MFT_SET_TYPE_TEST_ONLY)
    {
        return E_INVALIDARG;
    }

    if (dwOutputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    EnterCriticalSection(&m_critSec);
    HRESULT hr = S_OK;

    // If we have an input sample, the client cannot change the type now.
    if (m_pSample != NULL)
    {
        hr = MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING;
        goto done;
    }

    // Validate the type, if non-NULL.
    if (pType)
    {
        hr = OnCheckOutputType(pType);
        if (FAILED(hr))
        {
            goto done;
        }
    }

    // The type is OK. Set the type, unless the caller was just testing.
    if ( (dwFlags & MFT_SET_TYPE_TEST_ONLY) == 0 )
    {
        m_pOutputType = pType;

        // When the type changes, end streaming.
        m_bStreamingInitialized = false;
    }

done:
    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// GetInputCurrentType
// Returns the current input type.
//-------------------------------------------------------------------

HRESULT CInvert::GetInputCurrentType(
    DWORD           dwInputStreamID,
    IMFMediaType    **ppType
)
{
    if (ppType == NULL)
    {
        return E_POINTER;
    }

    if (dwInputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    HRESULT hr = S_OK;
    EnterCriticalSection(&m_critSec);

    if (!m_pInputType)
    {
        hr = MF_E_TRANSFORM_TYPE_NOT_SET;
    }
    else
    {
        *ppType = m_pInputType.Get();
        (*ppType)->AddRef();
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// GetOutputCurrentType
// Returns the current output type.
//-------------------------------------------------------------------

HRESULT CInvert::GetOutputCurrentType(
    DWORD           dwOutputStreamID,
    IMFMediaType    **ppType
)
{
    if (ppType == NULL)
    {
        return E_POINTER;
    }

    if( dwOutputStreamID != 0 )
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    HRESULT hr = S_OK;
    EnterCriticalSection(&m_critSec);

    if (!m_pOutputType)
    {
        hr = MF_E_TRANSFORM_TYPE_NOT_SET;
    }
    else
    {
        m_pOutputType.CopyTo(ppType);
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// GetInputStatus
// Query if the MFT is accepting more input.
//-------------------------------------------------------------------

HRESULT CInvert::GetInputStatus(
    DWORD           dwInputStreamID,
    DWORD           *pdwFlags
)
{
    if (pdwFlags == NULL)
    {
        return E_POINTER;
    }

    if (dwInputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    EnterCriticalSection(&m_critSec);

    if (m_pSample == NULL)
    {
        *pdwFlags = MFT_INPUT_STATUS_ACCEPT_DATA;
    }
    else
    {
        *pdwFlags = 0;
    }

    LeaveCriticalSection(&m_critSec);
    return S_OK;
}


//-------------------------------------------------------------------
// GetOutputStatus
// Query if the MFT can produce output.
//-------------------------------------------------------------------

HRESULT CInvert::GetOutputStatus(DWORD *pdwFlags)
{
    if (pdwFlags == NULL)
    {
        return E_POINTER;
    }

    EnterCriticalSection(&m_critSec);

    // The MFT can produce an output sample if (and only if) there an input sample.
    if (m_pSample != NULL)
    {
        *pdwFlags = MFT_OUTPUT_STATUS_SAMPLE_READY;
    }
    else
    {
        *pdwFlags = 0;
    }

    LeaveCriticalSection(&m_critSec);
    return S_OK;
}


//-------------------------------------------------------------------
// ProcessMessage
//-------------------------------------------------------------------

HRESULT CInvert::ProcessMessage(
    MFT_MESSAGE_TYPE    eMessage,
    ULONG_PTR           ulParam
)
{
    EnterCriticalSection(&m_critSec);

    HRESULT hr = S_OK;
    ComPtr<IUnknown> pUnk;
    ComPtr<IMFDXGIDeviceManager> pDXGIDeviceManager; 

    switch (eMessage)
    {
    case MFT_MESSAGE_COMMAND_FLUSH:
        // Flush the MFT.
        m_pSample = nullptr;
        break;

    case MFT_MESSAGE_SET_D3D_MANAGER:
        if (ulParam != NULL)
        {
            pUnk = (IUnknown*)ulParam;
            hr = pUnk.As(&pDXGIDeviceManager);

            if( SUCCEEDED( hr ) && (m_pDX11Manager != pDXGIDeviceManager) )
            {
                InvalidateDX11Resources();
                m_pDX11Manager = nullptr;
                m_pDX11Manager = pDXGIDeviceManager;

                hr = UpdateDX11Device();
                if (FAILED(hr))
                {
                    goto done;
                }

                if( m_spOutputSampleAllocator )
                {
                    hr = m_spOutputSampleAllocator->SetDirectXManager(pUnk.Get());
                    if (FAILED(hr))
                    {
                        goto done;
                    }
                }
            }
        }
        else
        {
            InvalidateDX11Resources();
            m_pDX11Manager = nullptr;
        }
        break;

    case MFT_MESSAGE_NOTIFY_BEGIN_STREAMING:
        hr = BeginStreaming();
        break;

    case MFT_MESSAGE_NOTIFY_END_STREAMING:
        m_bStreamingInitialized = false;
        break;

    // The remaining messages require no action
    case MFT_MESSAGE_NOTIFY_END_OF_STREAM:
    case MFT_MESSAGE_NOTIFY_START_OF_STREAM:
    case MFT_MESSAGE_COMMAND_DRAIN:
        break;
    }

done:
    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// ProcessInput
// Process an input sample.
//-------------------------------------------------------------------

HRESULT CInvert::ProcessInput(
    DWORD               dwInputStreamID,
    IMFSample           *pSample,
    DWORD               dwFlags
)
{
    // Check input parameters.
    if (pSample == NULL)
    {
        return E_POINTER;
    }

    if (dwFlags != 0)
    {
        return E_INVALIDARG; // dwFlags is reserved and must be zero.
    }

    // Validate the input stream number.
    if (dwInputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    HRESULT hr = S_OK;

    EnterCriticalSection(&m_critSec);

    // Check for valid media types.
    // The client must set input and output types before calling ProcessInput.
    if (!m_pInputType || !m_pOutputType)
    {
        hr = MF_E_NOTACCEPTING;   
        goto done;
    }

    // Check if an input sample is already queued.
    if (m_pSample != NULL)
    {
        hr = MF_E_NOTACCEPTING;   // We already have an input sample.
        goto done;
    }

    // Initialize streaming.
    hr = BeginStreaming();
    if (FAILED(hr))
    {
        goto done;
    }

    // Cache the sample. We do the actual work in ProcessOutput.
    m_pSample = pSample;

done:
    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// ProcessOutput
// Process an output sample.
//-------------------------------------------------------------------

HRESULT CInvert::ProcessOutput(
    DWORD                   dwFlags,
    DWORD                   cOutputBufferCount,
    MFT_OUTPUT_DATA_BUFFER  *pOutputSamples, // one per stream
    DWORD                   *pdwStatus
)
{
    if (dwFlags != 0)
    {
        return E_INVALIDARG;
    }

    if (pOutputSamples == NULL || pdwStatus == NULL)
    {
        return E_POINTER;
    }

    // There must be exactly one output buffer.
    if (cOutputBufferCount != 1)
    {
        return E_INVALIDARG;
    }

    // It must contain a sample.
    if (pOutputSamples[0].pSample == NULL && m_pDX11Manager == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;
    ComPtr<IMFMediaBuffer> pInput;
    ComPtr<IMFMediaBuffer> pOutput;
    ComPtr<ID3D11Device> spDevice;
    ComPtr<IMFMediaBuffer> pMediaBuffer;
    bool bDeviceLocked = false;

    EnterCriticalSection(&m_critSec);

    // There must be an input sample available for processing.
    if (m_pSample == nullptr)
    {
        hr = MF_E_TRANSFORM_NEED_MORE_INPUT;
        goto done;
    }

    // Initialize streaming.
    hr = BeginStreaming();
    if (FAILED(hr))
    {
        goto done;
    }

    // Check that our device is still good
    hr = CheckDX11Device();
    if (FAILED(hr))
    {
        goto done;
    }

    // When using DX we provide the output samples...
    if (m_pDX11Manager != nullptr)
    {
        hr = m_spOutputSampleAllocator->AllocateSample( &(pOutputSamples[0].pSample) );
        if (FAILED(hr))
        {
            goto done;
        }
    }

    if (pOutputSamples[0].pSample == nullptr)
    {
        hr = E_INVALIDARG;
        goto done;
    }

    // Get the input buffer.
    hr = m_pSample->GetBufferByIndex(0, &pInput);
    if (FAILED(hr))
    {
        goto done;
    }

    // Get the output buffer.
    hr = pOutputSamples[0].pSample->GetBufferByIndex(0, &pOutput);
    if (FAILED(hr))
    {
        goto done;
    }

    // Attempt to lock the device if necessary
    if (m_pDX11Manager != nullptr)
    {
        hr = m_pDX11Manager->LockDevice(m_hDeviceHandle, IID_PPV_ARGS( &spDevice ), TRUE );
        if (FAILED(hr))
        {
            goto done;
        }
        bDeviceLocked = true;
    }

    hr = OnProcessOutput(pInput.Get(), pOutput.Get());
    if (FAILED(hr))
    {
        goto done;
    }

    // Set status flags.
    pOutputSamples[0].dwStatus = 0;
    *pdwStatus = 0;

    // Copy the duration and time stamp from the input sample, if present.
    LONGLONG hnsDuration = 0;
    LONGLONG hnsTime = 0;

    if (SUCCEEDED(m_pSample->GetSampleDuration(&hnsDuration)))
    {
        hr = pOutputSamples[0].pSample->SetSampleDuration(hnsDuration);
        if (FAILED(hr))
        {
            goto done;
        }
    }

    if (SUCCEEDED(m_pSample->GetSampleTime(&hnsTime)))
    {
        hr = pOutputSamples[0].pSample->SetSampleTime(hnsTime);
    }

    // Always set the sample size
    hr = pOutputSamples[0].pSample->GetBufferByIndex( 0, &pMediaBuffer );
    if (FAILED(hr))
    {
        goto done;
    }

    hr = pMediaBuffer->SetCurrentLength(m_cbImageSize);
    if (FAILED(hr))
    {
        goto done;
    }

done:
    m_pSample = nullptr;

    if (bDeviceLocked)
    {
        hr = m_pDX11Manager->UnlockDevice(m_hDeviceHandle, FALSE);
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}

// PRIVATE METHODS

HRESULT CInvert::CheckDX11Device()
{
    HRESULT hr = S_OK;

    if(m_pDX11Manager != nullptr && m_hDeviceHandle )
    {
        if(m_pDX11Manager->TestDevice(m_hDeviceHandle) != S_OK)
        {
            InvalidateDX11Resources();

            hr = UpdateDX11Device();
            if (FAILED(hr))
            {
                return hr;
            }

            return m_pTransform->Initialize(m_pDevice.Get(), m_imageWidthInPixels, m_imageHeightInPixels);
        }
    }

    return S_OK;
}

// Delete any resources dependant on the current device we are using
void CInvert::InvalidateDX11Resources()
{
    m_pTransform->Invalidate();
    m_pDevice  = nullptr;
    m_pContext = nullptr;
    m_pInBufferTex = nullptr;
    m_pOutBufferTex = nullptr;
    m_pOutBufferStage = nullptr;
}

// Update the directx device
HRESULT CInvert::UpdateDX11Device()
{
    HRESULT hr = S_OK;

    if (m_pDX11Manager != nullptr)
    {
        hr = m_pDX11Manager->OpenDeviceHandle(&m_hDeviceHandle);
        if (FAILED(hr))
        {
            goto done;
        }

        hr = m_pDX11Manager->GetVideoService(m_hDeviceHandle, __uuidof(m_pDevice), (void**)&m_pDevice);
        if (FAILED(hr))
        {
            goto done;
        }
        
        m_pDevice->GetImmediateContext(&m_pContext);
    }
    else
    {
        D3D_FEATURE_LEVEL level;
        D3D_FEATURE_LEVEL levelsWanted[] = 
        { 
            D3D_FEATURE_LEVEL_11_1,
            D3D_FEATURE_LEVEL_11_0, 
            D3D_FEATURE_LEVEL_10_1, 
            D3D_FEATURE_LEVEL_10_0,
        };
        DWORD numLevelsWanted = sizeof( levelsWanted ) / sizeof( levelsWanted[0] );

        hr = D3D11CreateDevice( NULL, D3D_DRIVER_TYPE_WARP, NULL, 0 , levelsWanted, numLevelsWanted, 
                                D3D11_SDK_VERSION, &m_pDevice, &level, &m_pContext);
        if (FAILED(hr))
        {
            goto done;
        }

    }


    return hr;

done:
    InvalidateDX11Resources();
    return hr;
}

// Create a partial media type.
HRESULT CInvert::OnGetPartialType(DWORD dwTypeIndex, IMFMediaType **ppmt)
{
    if (dwTypeIndex > 0)
    {
        return MF_E_NO_MORE_TYPES;
    }

    ComPtr<IMFMediaType> pmt;

    HRESULT hr = MFCreateMediaType(&pmt);
    if (FAILED(hr))
    {
        goto done;
    }

    hr = pmt->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);
    if (FAILED(hr))
    {
        goto done;
    }

    hr = pmt->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_ARGB32);
    if (FAILED(hr))
    {
        goto done;
    }

    pmt.CopyTo(ppmt);

done:
    return hr;
}

// Validate an input media type.
HRESULT CInvert::OnCheckInputType(IMFMediaType *pmt)
{
    assert(pmt != NULL);

    HRESULT hr = S_OK;

    // If the output type is set, see if they match.
    if (m_pOutputType != NULL)
    {
        DWORD flags = 0;
        hr = pmt->IsEqual(m_pOutputType.Get(), &flags);

        // IsEqual can return S_FALSE. Treat this as failure.
        if (hr != S_OK)
        {
            hr = MF_E_INVALIDMEDIATYPE;
        }
    }
    else
    {
        // Output type is not set. Just check this type.
        hr = OnCheckMediaType(pmt);
    }
    return hr;
}

// Validate an output media type.
HRESULT CInvert::OnCheckOutputType(IMFMediaType *pmt)
{
    assert(pmt != NULL);

    HRESULT hr = S_OK;

    // If the input type is set, see if they match.
    if (m_pInputType != NULL)
    {
        DWORD flags = 0;
        hr = pmt->IsEqual(m_pInputType.Get(), &flags);

        // IsEqual can return S_FALSE. Treat this as failure.
        if (hr != S_OK)
        {
            hr = MF_E_INVALIDMEDIATYPE;
        }
    }
    else
    {
        // Input type is not set. Just check this type.
        hr = OnCheckMediaType(pmt);
    }
    return hr;
}

// Validate a media type (input or output)
HRESULT CInvert::OnCheckMediaType(IMFMediaType *pmt)
{
    // Major type must be video.
    GUID major_type;
    HRESULT hr = pmt->GetGUID(MF_MT_MAJOR_TYPE, &major_type);
    if (FAILED(hr))
    {
        goto done;
    }

    if (major_type != MFMediaType_Video)
    {
        hr = MF_E_INVALIDMEDIATYPE;
        goto done;
    }

    // Get the subtype GUID.
    GUID subtype;
    hr = pmt->GetGUID(MF_MT_SUBTYPE, &subtype);
    if (FAILED(hr))
    {
        goto done;
    }

    if(subtype != MFVideoFormat_ARGB32)
    {
        hr = MF_E_INVALIDMEDIATYPE;
        goto done;
    }

    // Reject single-field media types. 
    UINT32 interlace = MFGetAttributeUINT32(pmt, MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive);
    if (interlace == MFVideoInterlace_FieldSingleUpper  || interlace == MFVideoInterlace_FieldSingleLower)
    {
        hr = MF_E_INVALIDMEDIATYPE;
        goto done;
    }

    // Reject media types without a frame size
    UINT width, height;
    hr = MFGetAttributeSize(pmt, MF_MT_FRAME_SIZE, &width, &height);
    if (FAILED(hr))
    {
        goto done;
    }

done:
    return hr;
}

// Initialize streaming parameters.
HRESULT CInvert::BeginStreaming()
{
    HRESULT hr = S_OK;

    if (!m_bStreamingInitialized)
    {
        if( m_pDevice == nullptr )
        {
            hr = UpdateDX11Device();
            if (FAILED(hr))
            {
                goto done;
            }
        }

        hr = m_pTransform->Initialize(m_pDevice.Get(), m_imageWidthInPixels, m_imageHeightInPixels);
        if (FAILED(hr))
        {
            goto done;
        }

        // if the device is dxman we need to alloc samples...
        if( m_pDX11Manager != nullptr )
        {
            DWORD dwBindFlags = MFGetAttributeUINT32( m_spOutputAttributes.Get(), MF_SA_D3D11_BINDFLAGS, D3D11_BIND_RENDER_TARGET );
            dwBindFlags |= D3D11_BIND_RENDER_TARGET;        // render target binding must always be set
            hr = m_spOutputAttributes->SetUINT32( MF_SA_D3D11_BINDFLAGS, dwBindFlags );
            if (FAILED(hr))
            {
                goto done;
            }
            hr = m_spOutputAttributes->SetUINT32( MF_SA_BUFFERS_PER_SAMPLE, 1 );
            if (FAILED(hr))
            {
                goto done;
            }
            hr = m_spOutputAttributes->SetUINT32( MF_SA_D3D11_USAGE, D3D11_USAGE_DEFAULT );
            if (FAILED(hr))
            {
                goto done;
            }

            if ( NULL == m_spOutputSampleAllocator )
            {
                ComPtr<IMFVideoSampleAllocatorEx> spVideoSampleAllocator;
                ComPtr<IUnknown> spDXGIManagerUnk;

                hr = MFCreateVideoSampleAllocatorEx( IID_PPV_ARGS( &spVideoSampleAllocator ) );
                if (FAILED(hr))
                {
                    goto done;
                }
                
                hr = m_pDX11Manager.As( &spDXGIManagerUnk );
                if (FAILED(hr))
                {
                    goto done;
                }

                hr = spVideoSampleAllocator->SetDirectXManager( spDXGIManagerUnk.Get() );
                if (FAILED(hr))
                {
                    goto done;
                }

                m_spOutputSampleAllocator.Attach( spVideoSampleAllocator.Detach() );
            }

            hr = m_spOutputSampleAllocator->InitializeSampleAllocatorEx( 1, 10, m_spOutputAttributes.Get(), m_pOutputType.Get() );

            if ( FAILED(hr) && (dwBindFlags != D3D11_BIND_RENDER_TARGET) )
            {
                // Try again with only the mandatory "render target" binding
                hr = m_spOutputAttributes->SetUINT32( MF_SA_D3D11_BINDFLAGS, D3D11_BIND_RENDER_TARGET );
                if (FAILED(hr))
                {
                    goto done;
                }
                hr = m_spOutputSampleAllocator->InitializeSampleAllocatorEx( 1, 10, m_spOutputAttributes.Get(), m_pOutputType.Get() );
                if (FAILED(hr))
                {
                    goto done;
                }
            }
        }

        m_bStreamingInitialized = true;
    }

done:
    return hr;
}

// Reads DX buffers from IMFMediaBuffer
HRESULT BufferToDXType(IMFMediaBuffer *pBuffer, _Out_ UINT *uiViewIndex, _Out_ ID3D11Texture2D **ppTexture)
{
    HRESULT hr = S_OK;
    ComPtr<IMFDXGIBuffer> spDXGIBuffer;

    hr = pBuffer->QueryInterface(__uuidof(IMFDXGIBuffer), (LPVOID *)(&spDXGIBuffer));
    if (FAILED(hr))
    {
        goto done;
    }

    hr = spDXGIBuffer->GetResource(__uuidof(ID3D11Texture2D), (LPVOID *)(ppTexture));
    if (FAILED(hr))
    {
        goto done;
    }

    hr = spDXGIBuffer->GetSubresourceIndex(uiViewIndex);
    if (FAILED(hr))
    {
        goto done;
    }

done:
    return hr;
}

// Generate output data.
HRESULT CInvert::OnProcessOutput(IMFMediaBuffer *pIn, IMFMediaBuffer *pOut)
{
    HRESULT hr = S_OK;
    ComPtr<ID3D11Texture2D> pInTex;
    ComPtr<ID3D11Texture2D> pOutTex;
    BOOL bNativeOut = FALSE, bNativeIn = FALSE;
    UINT uiInIndex = 0;
    UINT uiOutIndex = 0;

    // Attempt to convert directly to DX textures
    hr = BufferToDXType(pIn, &uiInIndex, &pInTex);
    bNativeIn = SUCCEEDED(hr) && pInTex != nullptr;
    hr = BufferToDXType(pOut, &uiOutIndex, &pOutTex);
    bNativeOut = SUCCEEDED(hr) && pOutTex != nullptr;

    // If the input or output textures' device does not match our device
    // we have to move them to our device
    if (bNativeIn)
    {
        ComPtr<ID3D11Device> pDev;
        pInTex->GetDevice(&pDev);
        if( pDev != m_pDevice )
        {
            bNativeIn = FALSE;
        }
    }
    if (bNativeOut)
    {
        ComPtr<ID3D11Device> pDev;
        pOutTex->GetDevice(&pDev);
        if( pDev != m_pDevice )
        {
            bNativeOut = FALSE;
        }
    }


    if (!bNativeIn)
    {
        // Native DX texture in the buffer failed
        // create a texture we can use and copy the data in
        if (m_pInBufferTex == nullptr)
        {
            D3D11_TEXTURE2D_DESC desc;
            ZeroMemory(&desc,sizeof(desc));
            desc.Width = m_imageWidthInPixels;
            desc.Height = m_imageHeightInPixels;
            desc.ArraySize = 1;
            desc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
            desc.Usage = D3D11_USAGE_DYNAMIC;
            desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
            desc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
            desc.MipLevels = 1;
            desc.SampleDesc.Count = 1;

            hr = m_pDevice->CreateTexture2D(&desc, NULL, &m_pInBufferTex);
            if (FAILED(hr))
            {
                goto done;
            }
        }

        // scope the texture lock
        {
            TextureLock tlock(m_pContext.Get(), m_pInBufferTex.Get());
            hr = tlock.Map(uiInIndex, D3D11_MAP_WRITE_DISCARD, 0);
            if (FAILED(hr))
            {
                goto done;
            }

            // scope the video buffer lock
            {
                VideoBufferLock lock;
                LONG lStride;

                hr = GetDefaultStride(m_pInputType.Get(), &lStride);
                if (FAILED(hr))
                {
                    goto done;
                }

                hr = lock.Lock(pIn, MF2DBuffer_LockFlags_Read, lStride * m_imageHeightInPixels, lStride);
                if (FAILED(hr))
                {
                    goto done;
                }

                hr = MFCopyImage((BYTE*)tlock.map.pData, tlock.map.RowPitch, lock.GetData(), lock.GetStride(), abs(lStride), m_imageHeightInPixels);
                if (FAILED(hr))
                {
                    goto done;
                }
            }
        }
        pInTex = m_pInBufferTex;
    }

    if (!bNativeOut)
    {
        if (m_pOutBufferTex == nullptr)
        {
            D3D11_TEXTURE2D_DESC desc;
            ZeroMemory(&desc,sizeof(desc));
            desc.Width = m_imageWidthInPixels;
            desc.Height = m_imageHeightInPixels;
            desc.ArraySize = 1;
            desc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
            desc.Usage = D3D11_USAGE_DEFAULT;
            desc.BindFlags = D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE;
            desc.MipLevels = 1;
            desc.SampleDesc.Count = 1;
            hr = m_pDevice->CreateTexture2D(&desc, NULL, &m_pOutBufferTex);
            if (FAILED(hr))
            {
                goto done;
            }
        }
        pOutTex = m_pOutBufferTex;
    }

    // do some processing
    m_pTransform->ProcessFrame(m_pDevice.Get(),pInTex.Get(),uiInIndex,pOutTex.Get(),uiOutIndex);

    // write back pOut if necessary
    if (!bNativeOut)
    {
        if (m_pOutBufferStage == nullptr)
        {
            D3D11_TEXTURE2D_DESC desc;
            ZeroMemory(&desc,sizeof(desc));
            desc.Width = m_imageWidthInPixels;
            desc.Height = m_imageHeightInPixels;
            desc.ArraySize = 1;
            desc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
            desc.Usage = D3D11_USAGE_STAGING;
            desc.CPUAccessFlags = D3D11_CPU_ACCESS_READ;
            desc.MipLevels = 1;
            desc.SampleDesc.Count = 1;
            hr = m_pDevice->CreateTexture2D(&desc, NULL, &m_pOutBufferStage);
            if (FAILED(hr))
            {
                goto done;
            }
        }

        m_pContext->CopyResource(m_pOutBufferStage.Get(), m_pOutBufferTex.Get());

        {
            TextureLock tlock(m_pContext.Get(), m_pOutBufferStage.Get());
            hr = tlock.Map(uiOutIndex, D3D11_MAP_READ, 0);
            if (FAILED(hr))
            {
                goto done;
            }

            // scope the video buffer lock
            {
                VideoBufferLock lock;
                LONG lStride;

                hr = GetDefaultStride(m_pOutputType.Get(), &lStride);
                if (FAILED(hr))
                {
                    goto done;
                }

                hr = lock.Lock(pOut, MF2DBuffer_LockFlags_Write, lStride * m_imageHeightInPixels, lStride);
                if (FAILED(hr))
                {
                    goto done;
                }

                hr = MFCopyImage(lock.GetData(), lock.GetStride(), (BYTE*)tlock.map.pData, tlock.map.RowPitch, abs(lStride), m_imageHeightInPixels);
                if (FAILED(hr))
                {
                    goto done;
                }
            }
        }
    }

done:
    return hr;
}

// Update the format information. This method is called whenever the
// input type is set.
HRESULT CInvert::UpdateFormatInfo()
{
    HRESULT hr = S_OK;
    LONG lStride = 0;

    m_imageWidthInPixels = 0;
    m_imageHeightInPixels = 0;
    m_cbImageSize = 0;

    if (m_pInputType != NULL)
    {
        hr = MFGetAttributeSize(m_pInputType.Get(), MF_MT_FRAME_SIZE, &m_imageWidthInPixels, &m_imageHeightInPixels);
        if (FAILED(hr))
        {
            goto done;
        }

        hr = GetDefaultStride(m_pInputType.Get(), &lStride);
        if (FAILED(hr))
        {
            goto done;
        }

        // Calculate the image size
        m_cbImageSize = abs(lStride) * 4 * m_imageHeightInPixels;
    }

done:
    return hr;
}

// Get the default stride for a video format. 
HRESULT GetDefaultStride(IMFMediaType *pType, LONG *plStride)
{
    LONG lStride = 0;

    // Try to get the default stride from the media type.
    HRESULT hr = pType->GetUINT32(MF_MT_DEFAULT_STRIDE, (UINT32*)&lStride);
    if (FAILED(hr))
    {
        // Attribute not set. Try to calculate the default stride.
        GUID subtype = GUID_NULL;

        UINT32 width = 0;
        UINT32 height = 0;

        // Get the subtype and the image size.
        hr = pType->GetGUID(MF_MT_SUBTYPE, &subtype);
        if (SUCCEEDED(hr))
        {
            hr = MFGetAttributeSize(pType, MF_MT_FRAME_SIZE, &width, &height);
        }
        if (SUCCEEDED(hr))
        {
            lStride = width * 4;
        }

        // Set the attribute for later reference.
        if (SUCCEEDED(hr))
        {
            (void)pType->SetUINT32(MF_MT_DEFAULT_STRIDE, UINT32(lStride));
        }
    }
    if (SUCCEEDED(hr))
    {
        *plStride = lStride;
    }
    return hr;
}
