// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include <wrl\module.h>

#include "VideoBufferLock.h"
#include "Invert.h"
#include "TextureLock.h"

#include "InvertModule.h"

using namespace Microsoft::WRL;

ActivatableClass(CInvert);

LONG GetDefaultStride(IMFMediaType *pType);
ComPtr<ID3D11Texture2D> BufferToDXType(IMFMediaBuffer *pBuffer, _Out_ UINT *uiViewIndex);

CInvert::CInvert() :
    m_imageWidthInPixels(0), 
    m_imageHeightInPixels(0), 
    m_cbImageSize(0),
    m_fStreamingInitialized(false)
{
}

CInvert::~CInvert()
{
}

// Initialize the instance.
STDMETHODIMP CInvert::RuntimeClassInitialize()
{
    HRESULT hr = S_OK;

    try
    {
        // Create the attribute store.
        ThrowIfError(MFCreateAttributes(m_spAttributes.ReleaseAndGetAddressOf(), 3));

        // MFT supports DX11 acceleration
        ThrowIfError(m_spAttributes->SetUINT32(MF_SA_D3D_AWARE, 1));
        ThrowIfError(m_spAttributes->SetUINT32(MF_SA_D3D11_AWARE, 1));
        // output attributes
        ThrowIfError(MFCreateAttributes(m_spOutputAttributes.ReleaseAndGetAddressOf(), 1));
        // Load the transform
        m_transform = ref new CInvertModule();
    }
    catch(Exception ^exc)
    {
        hr = exc->HResult;
    }

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
    if ((pdwInputMinimum == nullptr) ||
        (pdwInputMaximum == nullptr) ||
        (pdwOutputMinimum == nullptr) ||
        (pdwOutputMaximum == nullptr))
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
    if ((pcInputStreams == nullptr) || (pcOutputStreams == nullptr))
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
    if (pStreamInfo == nullptr)
    {
        return E_POINTER;
    }

    if (dwInputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    AutoLock lock(m_critSec);

    pStreamInfo->hnsMaxLatency = 0;
    pStreamInfo->dwFlags = MFT_INPUT_STREAM_WHOLE_SAMPLES | MFT_INPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER;
    pStreamInfo->cbSize = m_spInputType == nullptr ? 0 : m_cbImageSize;
    pStreamInfo->cbMaxLookahead = 0;
    pStreamInfo->cbAlignment = 0;

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
    if (pStreamInfo == nullptr)
    {
        return E_POINTER;
    }

    if (dwOutputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    AutoLock lock(m_critSec);

    pStreamInfo->dwFlags =
        MFT_OUTPUT_STREAM_WHOLE_SAMPLES |
        MFT_OUTPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER |
        MFT_OUTPUT_STREAM_FIXED_SAMPLE_SIZE ;

    if( m_spDX11Manager != nullptr )
    {
        pStreamInfo->dwFlags |= MFT_OUTPUT_STREAM_PROVIDES_SAMPLES;
    }

    pStreamInfo->cbSize = m_spInputType == nullptr ? 0 : m_cbImageSize;
    pStreamInfo->cbAlignment = 0;

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

    if (nullptr == ppAttributes)
    {
        return E_POINTER;
    }

    if (dwOutputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    AutoLock lock(m_critSec);

    m_spOutputAttributes.CopyTo(ppAttributes);

    return( hr );
}

//-------------------------------------------------------------------
// GetAttributes
// Returns the attributes for the MFT.
//-------------------------------------------------------------------

HRESULT CInvert::GetAttributes(IMFAttributes **ppAttributes)
{
    if (ppAttributes == nullptr)
    {
        return E_POINTER;
    }

    AutoLock lock(m_critSec);

    m_spAttributes.CopyTo(ppAttributes);

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
    HRESULT hr = S_OK;

    try
    {
        if (ppType == nullptr)
        {
            throw ref new InvalidArgumentException();
        }

        if (dwInputStreamID != 0)
        {
            ThrowException(MF_E_INVALIDSTREAMNUMBER);
        }

        if (dwTypeIndex > 0)
        {
            return MF_E_NO_MORE_TYPES;
        }

        AutoLock lock(m_critSec);

        // If the output type is set, return that type as our preferred input type.
        if (m_spOutputType == nullptr)
        {
            // The output type is not set. Create a partial media type.
            *ppType = OnGetPartialType(dwTypeIndex).Detach();
        }
        else if (dwTypeIndex > 0)
        {
            return MF_E_NO_MORE_TYPES;
        }
        else
        {
            *ppType = m_spOutputType.Get();
            (*ppType)->AddRef();
        }
    }
    catch(Exception ^exc)
    {
        hr = exc->HResult;
    }

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
    HRESULT hr = S_OK;

    try
    {
        if (ppType == nullptr)
        {
            throw ref new InvalidArgumentException();
        }

        if (dwOutputStreamID != 0)
        {
            ThrowException(MF_E_INVALIDSTREAMNUMBER);
        }

        if (dwTypeIndex > 0)
        {
            return MF_E_NO_MORE_TYPES;
        }

        AutoLock lock(m_critSec);

        if (m_spInputType == nullptr)
        {
            // The input type is not set. Create a partial media type.
            *ppType = OnGetPartialType(dwTypeIndex).Detach();
        }
        else if (dwTypeIndex > 0)
        {
            return MF_E_NO_MORE_TYPES;
        }
        else
        {
            *ppType = m_spInputType.Get();
            (*ppType)->AddRef();
        }
    }
    catch(Exception ^exc)
    {
        hr = exc->HResult;
    }

    return hr;
}


//-------------------------------------------------------------------
// SetInputType
//-------------------------------------------------------------------

HRESULT CInvert::SetInputType(
    DWORD           dwInputStreamID,
    IMFMediaType    *pType, // Can be nullptr to clear the input type.
    DWORD           dwFlags
)
{
    HRESULT hr = S_OK;

    try
    {
        // Validate flags.
        if (dwFlags & ~MFT_SET_TYPE_TEST_ONLY)
        {
            throw ref new InvalidArgumentException();
        }

        if (dwInputStreamID != 0)
        {
            ThrowException(MF_E_INVALIDSTREAMNUMBER);
        }

        AutoLock lock(m_critSec);

        // If we have an input sample, the client cannot change the type now.
        if (m_spSample != nullptr)
        {
            ThrowException(MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING);
        }

        // Validate the type, if non-nullptr.
        if (pType != nullptr)
        {
            OnCheckInputType(pType);
        }

        // The type is OK. Set the type, unless the caller was just testing.
        if ((dwFlags & MFT_SET_TYPE_TEST_ONLY) == 0)
        {
            m_spInputType = nullptr;
            m_spInputType = pType;

            // Update the format information.
            UpdateFormatInfo();

            // When the type changes, end streaming.
            m_fStreamingInitialized = false;
        }
    }
    catch(Exception ^exc)
    {
        hr = exc->HResult;
    }

    return hr;
}



//-------------------------------------------------------------------
// SetOutputType
//-------------------------------------------------------------------

HRESULT CInvert::SetOutputType(
    DWORD           dwOutputStreamID,
    IMFMediaType    *pType, // Can be nullptr to clear the output type.
    DWORD           dwFlags
)
{
    HRESULT hr = S_OK;

    // Validate flags.
    try
    {
        if (dwFlags & ~MFT_SET_TYPE_TEST_ONLY)
        {
            throw ref new InvalidArgumentException();
        }

        if (dwOutputStreamID != 0)
        {
            ThrowException(MF_E_INVALIDSTREAMNUMBER);
        }

        AutoLock lock(m_critSec);

        // If we have an input sample, the client cannot change the type now.
        if (m_spSample != nullptr)
        {
            ThrowException(MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING);
        }

        // Validate the type, if non-nullptr.
        if (pType != nullptr)
        {
            OnCheckOutputType(pType);
        }

        // The type is OK. Set the type, unless the caller was just testing.
        if ((dwFlags & MFT_SET_TYPE_TEST_ONLY) == 0)
        {
            m_spOutputType = pType;

            // When the type changes, end streaming.
            m_fStreamingInitialized = false;
        }
    }
    catch(Exception ^exc)
    {
        hr = exc->HResult;
    }

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
    if (ppType == nullptr)
    {
        return E_POINTER;
    }

    if (dwInputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    HRESULT hr = S_OK;
    AutoLock lock(m_critSec);

    if (!m_spInputType)
    {
        hr = MF_E_TRANSFORM_TYPE_NOT_SET;
    }
    else
    {
        *ppType = m_spInputType.Get();
        (*ppType)->AddRef();
    }

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
    if (ppType == nullptr)
    {
        return E_POINTER;
    }

    if( dwOutputStreamID != 0 )
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    HRESULT hr = S_OK;
    AutoLock lock(m_critSec);

    if (!m_spOutputType)
    {
        hr = MF_E_TRANSFORM_TYPE_NOT_SET;
    }
    else
    {
        m_spOutputType.CopyTo(ppType);
    }

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
    if (pdwFlags == nullptr)
    {
        return E_POINTER;
    }

    if (dwInputStreamID != 0)
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    AutoLock lock(m_critSec);

    if (m_spSample == nullptr)
    {
        *pdwFlags = MFT_INPUT_STATUS_ACCEPT_DATA;
    }
    else
    {
        *pdwFlags = 0;
    }

    return S_OK;
}


//-------------------------------------------------------------------
// GetOutputStatus
// Query if the MFT can produce output.
//-------------------------------------------------------------------

HRESULT CInvert::GetOutputStatus(DWORD *pdwFlags)
{
    if (pdwFlags == nullptr)
    {
        return E_POINTER;
    }

    AutoLock lock(m_critSec);

    // The MFT can produce an output sample if (and only if) there an input sample.
    if (m_spSample != nullptr)
    {
        *pdwFlags = MFT_OUTPUT_STATUS_SAMPLE_READY;
    }
    else
    {
        *pdwFlags = 0;
    }

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
    HRESULT hr = S_OK;

    try
    {
        AutoLock lock(m_critSec);

        ComPtr<IMFDXGIDeviceManager> pDXGIDeviceManager; 

        switch (eMessage)
        {
        case MFT_MESSAGE_COMMAND_FLUSH:
            // Flush the MFT.
            m_spSample = nullptr;
            break;

        case MFT_MESSAGE_SET_D3D_MANAGER:
            if (ulParam != 0)
            {
                ComPtr<IUnknown> spManagerUnk = reinterpret_cast<IUnknown*>(ulParam);

                ThrowIfError(spManagerUnk.As(&pDXGIDeviceManager));

                if (m_spDX11Manager != pDXGIDeviceManager)
                {
                    InvalidateDX11Resources();
                    m_spDX11Manager = nullptr;
                    m_spDX11Manager = pDXGIDeviceManager;

                    UpdateDX11Device();

                    if (m_spOutputSampleAllocator != nullptr)
                    {
                        ThrowIfError(m_spOutputSampleAllocator->SetDirectXManager(spManagerUnk.Get()));
                    }
                }
            }
            else
            {
                InvalidateDX11Resources();
                m_spDX11Manager = nullptr;
            }
            break;

        case MFT_MESSAGE_NOTIFY_BEGIN_STREAMING:
            BeginStreaming();
            break;

        case MFT_MESSAGE_NOTIFY_END_STREAMING:
            m_fStreamingInitialized = false;
            break;

        // The remaining messages require no action
        case MFT_MESSAGE_NOTIFY_END_OF_STREAM:
        case MFT_MESSAGE_NOTIFY_START_OF_STREAM:
        case MFT_MESSAGE_COMMAND_DRAIN:
            break;
        }
    }
    catch(Exception ^exc)
    {
        hr = exc->HResult;
    }

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
    HRESULT hr = S_OK;

    try
    {
        // Check input parameters.
        if (pSample == nullptr)
        {
            throw ref new InvalidArgumentException();
        }

        if (dwFlags != 0)
        {
            throw ref new InvalidArgumentException(); // dwFlags is reserved and must be zero.
        }

        // Validate the input stream number.
        if (dwInputStreamID != 0)
        {
            ThrowException(MF_E_INVALIDSTREAMNUMBER);
        }

        AutoLock lock(m_critSec);

        // Check for valid media types.
        // The client must set input and output types before calling ProcessInput.
        if (!m_spInputType || !m_spOutputType)
        {
            ThrowException(MF_E_NOTACCEPTING);   
        }

        // Check if an input sample is already queued.
        if (m_spSample != nullptr)
        {
            ThrowException(MF_E_NOTACCEPTING);   // We already have an input sample.
        }

        // Initialize streaming.
        BeginStreaming();

        // Cache the sample. We do the actual work in ProcessOutput.
        m_spSample = pSample;
    }
    catch(Exception ^exc)
    {
        hr = exc->HResult;
    }

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
    HRESULT hr = S_OK;
    bool fDeviceLocked = false;
    try
    {
        if (dwFlags != 0)
        {
            throw ref new InvalidArgumentException();
        }

        if (pOutputSamples == nullptr || pdwStatus == nullptr)
        {
            throw ref new InvalidArgumentException();
        }

        // There must be exactly one output buffer.
        if (cOutputBufferCount != 1)
        {
            throw ref new InvalidArgumentException();
        }

        // It must contain a sample.
        if (pOutputSamples[0].pSample == nullptr && m_spDX11Manager == nullptr)
        {
            throw ref new InvalidArgumentException();
        }

        ComPtr<IMFMediaBuffer> spInput;
        ComPtr<IMFMediaBuffer> spOutput;
        ComPtr<ID3D11Device> spDevice;
        ComPtr<IMFMediaBuffer> spMediaBuffer;

        AutoLock lock(m_critSec);

        // There must be an input sample available for processing.
        if (m_spSample == nullptr)
        {
            return MF_E_TRANSFORM_NEED_MORE_INPUT;
        }

        // Initialize streaming.
        BeginStreaming();

        // Check that our device is still good
        CheckDX11Device();

        // When using DX we provide the output samples...
        if (m_spDX11Manager != nullptr)
        {
            ThrowIfError(m_spOutputSampleAllocator->AllocateSample(&(pOutputSamples[0].pSample)));;
        }

        if (pOutputSamples[0].pSample == nullptr)
        {
            throw ref new InvalidArgumentException();
        }

        // Get the input buffer.
        ThrowIfError(m_spSample->GetBufferByIndex(0, &spInput));

        // Get the output buffer.
        ThrowIfError(pOutputSamples[0].pSample->GetBufferByIndex(0, &spOutput));

        // Attempt to lock the device if necessary
        if (m_spDX11Manager != nullptr)
        {
            ThrowIfError(m_spDX11Manager->LockDevice(m_hDeviceHandle, IID_PPV_ARGS(&spDevice), TRUE));
            fDeviceLocked = true;
        }

        OnProcessOutput(spInput.Get(), spOutput.Get());

        // Set status flags.
        pOutputSamples[0].dwStatus = 0;
        *pdwStatus = 0;

        // Copy the duration and time stamp from the input sample, if present.
        LONGLONG hnsDuration = 0;
        LONGLONG hnsTime = 0;

        if (SUCCEEDED(m_spSample->GetSampleDuration(&hnsDuration)))
        {
            ThrowIfError(pOutputSamples[0].pSample->SetSampleDuration(hnsDuration));
        }

        if (SUCCEEDED(m_spSample->GetSampleTime(&hnsTime)))
        {
            ThrowIfError(pOutputSamples[0].pSample->SetSampleTime(hnsTime));
        }

        // Always set the sample size
        ThrowIfError(pOutputSamples[0].pSample->GetBufferByIndex(0, &spMediaBuffer));

        ThrowIfError(spMediaBuffer->SetCurrentLength(m_cbImageSize));

        m_spSample = nullptr;
        if (fDeviceLocked)
        {
            ThrowIfError(m_spDX11Manager->UnlockDevice(m_hDeviceHandle, FALSE));
        }
    }
    catch(Exception ^exc)
    {
        hr = exc->HResult;
    }

    m_spSample = nullptr;
    if (fDeviceLocked)
    {
        m_spDX11Manager->UnlockDevice(m_hDeviceHandle, FALSE);
    }

    return hr;
}

// PRIVATE METHODS

void CInvert::CheckDX11Device()
{
    if(m_spDX11Manager != nullptr && m_hDeviceHandle )
    {
        if(m_spDX11Manager->TestDevice(m_hDeviceHandle) != S_OK)
        {
            InvalidateDX11Resources();

            UpdateDX11Device();

            m_transform->Initialize(m_spDevice.Get(), m_imageWidthInPixels, m_imageHeightInPixels);
        }
    }
}

// Delete any resources dependant on the current device we are using
void CInvert::InvalidateDX11Resources()
{
    m_transform->Invalidate();
    m_spDevice  = nullptr;
    m_spContext = nullptr;
    m_spInBufferTex = nullptr;
    m_spOutBufferTex = nullptr;
    m_spOutBufferStage = nullptr;
}

// Update the directx device
void CInvert::UpdateDX11Device()
{
    try
    {
        if (m_spDX11Manager != nullptr)
        {
            ThrowIfError(m_spDX11Manager->OpenDeviceHandle(&m_hDeviceHandle));

            ThrowIfError(m_spDX11Manager->GetVideoService(m_hDeviceHandle, __uuidof(m_spDevice), (void**)&m_spDevice));
       
            m_spDevice->GetImmediateContext(&m_spContext);
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

            ThrowIfError(D3D11CreateDevice( nullptr, D3D_DRIVER_TYPE_WARP, nullptr, 0 , levelsWanted, numLevelsWanted, 
                                    D3D11_SDK_VERSION, &m_spDevice, &level, &m_spContext));
        }
    }
    catch(Exception^)
    {
        InvalidateDX11Resources();
        throw;
    }
}

// Create a partial media type.
ComPtr<IMFMediaType> CInvert::OnGetPartialType(DWORD dwTypeIndex)
{
    if (dwTypeIndex > 0)
    {
        ThrowException(MF_E_NO_MORE_TYPES);
    }

    ComPtr<IMFMediaType> pmt;

    ComPtr<IMFMediaType> spMT;

    ThrowIfError(MFCreateMediaType(&spMT));

    ThrowIfError(spMT->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video));

    ThrowIfError(spMT->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_ARGB32));

    return spMT;
}

// Validate an input media type.
void CInvert::OnCheckInputType(IMFMediaType *pmt)
{
    assert(pmt != nullptr);

    // If the output type is set, see if they match.
    if (m_spOutputType != nullptr)
    {
        DWORD flags = 0;
        // IsEqual can return S_FALSE. Treat this as failure.
        if(pmt->IsEqual(m_spOutputType.Get(), &flags) != S_OK)
        {
            ThrowException(MF_E_INVALIDMEDIATYPE);
        }
    }
    else
    {
        // Output type is not set. Just check this type.
        OnCheckMediaType(pmt);
    }
}

// Validate an output media type.
void CInvert::OnCheckOutputType(IMFMediaType *pmt)
{
    assert(pmt != nullptr);

    // If the input type is set, see if they match.
    if (m_spInputType != nullptr)
    {
        DWORD flags = 0;
        // IsEqual can return S_FALSE. Treat this as failure.
        if (pmt->IsEqual(m_spInputType.Get(), &flags) != S_OK)
        {
            ThrowException(MF_E_INVALIDMEDIATYPE);
        }
    }
    else
    {
        // Input type is not set. Just check this type.
        OnCheckMediaType(pmt);
    }
}

// Validate a media type (input or output)
void CInvert::OnCheckMediaType(IMFMediaType *pmt)
{
    // Major type must be video.
    GUID major_type;
    ThrowIfError(pmt->GetGUID(MF_MT_MAJOR_TYPE, &major_type));

    if (major_type != MFMediaType_Video)
    {
        ThrowException(MF_E_INVALIDMEDIATYPE);
    }
    // Get the subtype GUID.
    GUID subtype;
    ThrowIfError(pmt->GetGUID(MF_MT_SUBTYPE, &subtype));

    if (subtype != MFVideoFormat_ARGB32)
    {
        ThrowException(MF_E_INVALIDMEDIATYPE);
    }

    // Reject single-field media types. 
    UINT32 interlace = MFGetAttributeUINT32(pmt, MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive);
    if (interlace == MFVideoInterlace_FieldSingleUpper  || interlace == MFVideoInterlace_FieldSingleLower)
    {
        ThrowException(MF_E_INVALIDMEDIATYPE);
    }

    // Reject media types without a frame size
    UINT width, height;
    ThrowIfError(MFGetAttributeSize(pmt, MF_MT_FRAME_SIZE, &width, &height));
}

// Initialize streaming parameters.
void CInvert::BeginStreaming()
{
    if (!m_fStreamingInitialized)
    {
        if( m_spDevice == nullptr )
        {
            UpdateDX11Device();
        }

        m_transform->Initialize(m_spDevice.Get(), m_imageWidthInPixels, m_imageHeightInPixels);

        // if the device is dxman we need to alloc samples...
        if (m_spDX11Manager != nullptr)
        {
            DWORD dwBindFlags = MFGetAttributeUINT32(m_spOutputAttributes.Get(), MF_SA_D3D11_BINDFLAGS, D3D11_BIND_RENDER_TARGET);
            dwBindFlags |= D3D11_BIND_RENDER_TARGET;        // render target binding must always be set
            ThrowIfError(m_spOutputAttributes->SetUINT32(MF_SA_D3D11_BINDFLAGS, dwBindFlags));
            ThrowIfError(m_spOutputAttributes->SetUINT32(MF_SA_BUFFERS_PER_SAMPLE, 1));
            ThrowIfError(m_spOutputAttributes->SetUINT32(MF_SA_D3D11_USAGE, D3D11_USAGE_DEFAULT));

            if (nullptr == m_spOutputSampleAllocator)
            {
                ComPtr<IMFVideoSampleAllocatorEx> spVideoSampleAllocator;
                ComPtr<IUnknown> spDXGIManagerUnk;

                ThrowIfError(MFCreateVideoSampleAllocatorEx(IID_PPV_ARGS( &spVideoSampleAllocator)));
                ThrowIfError(m_spDX11Manager.As(&spDXGIManagerUnk));
                ThrowIfError(spVideoSampleAllocator->SetDirectXManager(spDXGIManagerUnk.Get()));
                m_spOutputSampleAllocator.Attach(spVideoSampleAllocator.Detach());
            }

            HRESULT hr = m_spOutputSampleAllocator->InitializeSampleAllocatorEx(1, 10, m_spOutputAttributes.Get(), m_spOutputType.Get());

            if (FAILED(hr))
            {
                if (dwBindFlags != D3D11_BIND_RENDER_TARGET)
                {
                    // Try again with only the mandatory "render target" binding
                    ThrowIfError(m_spOutputAttributes->SetUINT32(MF_SA_D3D11_BINDFLAGS, D3D11_BIND_RENDER_TARGET));
                    ThrowIfError(m_spOutputSampleAllocator->InitializeSampleAllocatorEx(1, 10, m_spOutputAttributes.Get(), m_spOutputType.Get()));
                }
                else
                {
                    ThrowException(hr);
                }
            }
        }

        m_fStreamingInitialized = true;
    }
}

// Reads DX buffers from IMFMediaBuffer
ComPtr<ID3D11Texture2D> BufferToDXType(IMFMediaBuffer *pBuffer, _Out_ UINT *uiViewIndex)
{
    ComPtr<IMFDXGIBuffer> spDXGIBuffer;
    ComPtr<ID3D11Texture2D> spTexture;

    if (SUCCEEDED(pBuffer->QueryInterface(IID_PPV_ARGS(&spDXGIBuffer))))
    {
        if(SUCCEEDED(spDXGIBuffer->GetResource(IID_PPV_ARGS(&spTexture))))
        {
            spDXGIBuffer->GetSubresourceIndex(uiViewIndex);
        }
    }

    return spTexture;
}

// Generate output data.
void CInvert::OnProcessOutput(IMFMediaBuffer *pIn, IMFMediaBuffer *pOut)
{
    ComPtr<ID3D11Texture2D> spInTex;
    ComPtr<ID3D11Texture2D> spOutTex;
    UINT uiInIndex = 0;
    UINT uiOutIndex = 0;

    // Attempt to convert directly to DX textures
    try
    {
        spInTex = BufferToDXType(pIn, &uiInIndex);
    }
    catch(Exception^)
    {
    }
    try
    {
        spOutTex = BufferToDXType(pOut, &uiOutIndex);
    }
    catch(Exception^)
    {
    }

    bool fNativeIn = spInTex != nullptr;
    bool fNativeOut = spOutTex != nullptr;

    // If the input or output textures' device does not match our device
    // we have to move them to our device
    if (fNativeIn)
    {
        ComPtr<ID3D11Device> spDev;
        spInTex->GetDevice(&spDev);
        if( spDev != m_spDevice )
        {
            fNativeIn = false;
        }
    }
    if (fNativeOut)
    {
        ComPtr<ID3D11Device> spDev;
        spOutTex->GetDevice(&spDev);
        if( spDev != m_spDevice )
        {
            fNativeOut = false;
        }
    }


    if (!fNativeIn)
    {
        // Native DX texture in the buffer failed
        // create a texture we can use and copy the data in
        if (m_spInBufferTex == nullptr)
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

            ThrowIfError(m_spDevice->CreateTexture2D(&desc, nullptr, &m_spInBufferTex));
        }

        // scope the texture lock
        {
            TextureLock tlock(m_spContext.Get(), m_spInBufferTex.Get());
            ThrowIfError(tlock.Map(uiInIndex, D3D11_MAP_WRITE_DISCARD, 0));

            // scope the video buffer lock
            {
                LONG lStride = GetDefaultStride(m_spInputType.Get());

                VideoBufferLock lock(pIn, MF2DBuffer_LockFlags_Read, m_imageHeightInPixels, lStride);

                ThrowIfError(MFCopyImage((BYTE*)tlock.map.pData, tlock.map.RowPitch, lock.GetData(), lock.GetStride(), abs(lStride), m_imageHeightInPixels));
            }
        }
        spInTex = m_spInBufferTex;
    }

    if (!fNativeOut)
    {
        if (m_spOutBufferTex == nullptr)
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
            ThrowIfError(m_spDevice->CreateTexture2D(&desc, nullptr, &m_spOutBufferTex));
        }
        spOutTex = m_spOutBufferTex;
    }

    // do some processing
    m_transform->ProcessFrame(m_spDevice.Get(),spInTex.Get(),uiInIndex,spOutTex.Get(),uiOutIndex);

    // write back pOut if necessary
    if (!fNativeOut)
    {
        if (m_spOutBufferStage == nullptr)
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
            ThrowIfError(m_spDevice->CreateTexture2D(&desc, nullptr, &m_spOutBufferStage));
        }

        m_spContext->CopyResource(m_spOutBufferStage.Get(), m_spOutBufferTex.Get());

        {
            TextureLock tlock(m_spContext.Get(), m_spOutBufferStage.Get());
            ThrowIfError(tlock.Map(uiOutIndex, D3D11_MAP_READ, 0));

            // scope the video buffer lock
            {
                LONG lStride = GetDefaultStride(m_spOutputType.Get());

                VideoBufferLock lock(pOut, MF2DBuffer_LockFlags_Write, m_imageHeightInPixels, lStride);

                ThrowIfError(MFCopyImage(lock.GetData(), lock.GetStride(), (BYTE*)tlock.map.pData, tlock.map.RowPitch, abs(lStride), m_imageHeightInPixels));
            }
        }
    }
}

// Update the format information. This method is called whenever the
// input type is set.
void CInvert::UpdateFormatInfo()
{
    m_imageWidthInPixels = 0;
    m_imageHeightInPixels = 0;
    m_cbImageSize = 0;

    if (m_spInputType != nullptr)
    {
        ThrowIfError(MFGetAttributeSize(m_spInputType.Get(), MF_MT_FRAME_SIZE, &m_imageWidthInPixels, &m_imageHeightInPixels));

        LONG lStride = GetDefaultStride(m_spInputType.Get());
        // Calculate the image size
        m_cbImageSize = abs(lStride) * 4 * m_imageHeightInPixels;
    }
}

// Get the default stride for a video format. 
LONG GetDefaultStride(IMFMediaType *pType)
{
    LONG lStride = 0;

    // Try to get the default stride from the media type.
    if (FAILED(pType->GetUINT32(MF_MT_DEFAULT_STRIDE, (UINT32*)&lStride)))
    {
        UINT32 width = 0;
        UINT32 height = 0;

        // Get the subtype and the image size.
        ThrowIfError(MFGetAttributeSize(pType, MF_MT_FRAME_SIZE, &width, &height));
        lStride = width * 4;

        // Set the attribute for later reference.
        (void)pType->SetUINT32(MF_MT_DEFAULT_STRIDE, UINT32(lStride));
    }

    return lStride;
}