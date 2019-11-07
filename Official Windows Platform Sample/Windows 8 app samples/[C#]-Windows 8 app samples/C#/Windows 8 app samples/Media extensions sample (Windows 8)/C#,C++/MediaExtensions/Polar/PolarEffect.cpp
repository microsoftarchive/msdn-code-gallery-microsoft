// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#include "PolarEffect.h"
#include "bufferlock.h"

#pragma comment(lib, "d2d1")

/*

This sample implements video effects as a Media Foundation transform (MFT).

Windows Store apps using JavaScript can select one of the following effects
by using PropertySet argument of msInsertVideoEffect.

Fisheye

    var effect = new Windows.Foundation.Collections.PropertySet();
    effect["effect"] = "Fisheye";

Pinch

    var effect = new Windows.Foundation.Collections.PropertySet();
    effect["effect"] = "Pinch";

Warp

    var effect = new Windows.Foundation.Collections.PropertySet();
    effect["effect"] = "Warp";


These effects are based on polar transformation.

Each pixel in a YUV image is represented as radius (r) and angle (theta) from
the center of the image. If R is the maximum distance from the center of the
image, the new pixel location is calculated by the following rules:

Fisheye

    r := r*(r/R)

Pinch

    r := sqrt(r/R)

Warp

    theta := theta + r/R


Before the first video frame is processed, a lookup table is created to store
the result of the polar transformation. Then, the same lookup table is used
until the end of the streaming session.


NOTES ON THE MFT IMPLEMENTATION

1. The MFT has fixed streams: One input stream and one output stream. 

2. The MFT supports the following formats: UYVY, YUY2, NV12.

3. If the MFT is holding an input sample, SetInputType and SetOutputType both fail.

4. The input and output types must be identical.

5. If both types are set, no type can be set until the current type is cleared.

6. Preferred input types:
 
     (a) If the output type is set, that's the preferred type.
     (b) Otherwise, the preferred types are partial types, constructed from the 
         list of supported subtypes.
 
7. Preferred output types: As above.

8. Streaming: 
 
    The private BeingStreaming() method is called in response to the 
    MFT_MESSAGE_NOTIFY_BEGIN_STREAMING message. 

    If the client does not send MFT_MESSAGE_NOTIFY_BEGIN_STREAMING, the MFT calls
    BeginStreaming inside the first call to ProcessInput or ProcessOutput. 

    This is a good approach for allocating resources that your MFT requires for
    streaming. 
    
9. The configuration attributes are applied in the BeginStreaming method. If the 
   client changes the attributes during streaming, the change is ignored until 
   streaming is stopped (either by changing the media types or by sending the 
   MFT_MESSAGE_NOTIFY_END_STREAMING message) and then restarted.
   
*/

// Video FOURCC codes.
const DWORD FOURCC_YUY2 = '2YUY'; 
const DWORD FOURCC_UYVY = 'YVYU'; 
const DWORD FOURCC_NV12 = '21VN'; 

// Static array of media types (preferred and accepted).
const GUID g_MediaSubtypes[] =
{
    MFVideoFormat_NV12,
    MFVideoFormat_YUY2,
    MFVideoFormat_UYVY
};

HRESULT GetImageSize(DWORD fcc, UINT32 width, UINT32 height, DWORD* pcbImage);
HRESULT GetDefaultStride(IMFMediaType *pType, LONG *plStride);

//-------------------------------------------------------------------
// Functions to apply the lookup table to YUV images.
//
// In all cases, the same transformation is applied to the YUV
// image, but the pixel layout in memory differs. The transformation
// is applied to the pair of pixels which share the UV component.
//
// The image conversion functions take the following parameters:
//
// pLookup          Pointer to the lookup table.
// pDest             Pointer to the destination buffer.
// lDestStride       Stride of the destination buffer, in bytes.
// pSrc              Pointer to the source buffer.
// lSrcStride        Stride of the source buffer, in bytes.
// dwWidthInPixels   Frame width in pixels.
// dwHeightInPixels  Frame height, in pixels.
//-------------------------------------------------------------------

// Convert UYVY image.

void TransformImage_UYVY(
    _In_reads_(_Inexpressible_(dwWidthInPixels * dwHeightInPixels)) const UINT32* pLookup,
    _Inout_updates_(_Inexpressible_(lDestStride * dwHeightInPixels)) BYTE *pDest, 
    _In_ LONG lDestStride, 
    _In_reads_(_Inexpressible_(lSrcStride * dwHeightInPixels)) const BYTE* pSrc,
    _In_ LONG lSrcStride, 
    _In_ DWORD dwWidthInPixels, 
    _In_ DWORD dwHeightInPixels)
{
    DWORD *pSrc_UYVY = (DWORD*) pSrc;

    // Byte order is U0 Y0 V0 Y1
    // Windows is little-endian so the order appears reversed.

    pSrc_UYVY[0] = 0x00800080;  // black

    for (DWORD y = 0; y < dwHeightInPixels; y++)
    {
        DWORD *pDest_UYVY = (DWORD*) pDest;

        for (DWORD x = 0; (x + 1) < dwWidthInPixels; x += 2)
        {
#pragma warning(push)
#pragma warning(disable: 6385) 
#pragma warning(disable: 6386) 
            UINT32 uOffset = (pLookup[x] % dwWidthInPixels) * sizeof(WORD) + (pLookup[x] / dwWidthInPixels) * lSrcStride;
            pDest_UYVY[x/2] = pSrc_UYVY[uOffset/sizeof(DWORD)];
#pragma warning(pop)
        }
        pDest += lDestStride;
        pLookup += dwWidthInPixels;
    }
}


// Convert YUY2 image.

void TransformImage_YUY2(
    _In_reads_(_Inexpressible_(dwWidthInPixels * dwHeightInPixels)) const UINT32* pLookup,
    _Inout_updates_(_Inexpressible_(lDestStride * dwHeightInPixels)) BYTE *pDest, 
    _In_ LONG lDestStride, 
    _In_reads_(_Inexpressible_(lSrcStride * dwHeightInPixels)) const BYTE* pSrc,
    _In_ LONG lSrcStride, 
    _In_ DWORD dwWidthInPixels, 
    _In_ DWORD dwHeightInPixels)
{
    DWORD *pSrc_UYVY = (DWORD*) pSrc;

    // Byte order is Y0 U0 Y1 V0
    // Windows is little-endian so the order appears reversed.

    pSrc_UYVY[0] = 0x80008000;  // black

    for (DWORD y = 0; y < dwHeightInPixels; y++)
    {
        DWORD *pDest_UYVY = (DWORD*) pDest;

        for (DWORD x = 0; (x + 1) < dwWidthInPixels; x += 2)
        {
#pragma warning(push)
#pragma warning(disable: 6385) 
#pragma warning(disable: 6386) 
            UINT32 uOffset = (pLookup[x] % dwWidthInPixels) * sizeof(WORD) + (pLookup[x] / dwWidthInPixels) * lSrcStride;
            pDest_UYVY[x/2] = pSrc_UYVY[uOffset/sizeof(DWORD)];
#pragma warning(pop)
        }
        pDest += lDestStride;
        pLookup += dwWidthInPixels;
    }
}


// Convert NV12 image

void TransformImage_NV12(
    _In_reads_(_Inexpressible_(dwWidthInPixels * dwHeightInPixels)) const UINT32* pLookup,
    _Inout_updates_(_Inexpressible_(lDestStride * dwHeightInPixels)) BYTE *pDest, 
    _In_ LONG lDestStride, 
    _In_reads_(_Inexpressible_(lSrcStride * dwHeightInPixels)) const BYTE* pSrc,
    _In_ LONG lSrcStride, 
    _In_ DWORD dwWidthInPixels, 
    _In_ DWORD dwHeightInPixels)
{
    // NV12 is planar: Y plane, followed by packed U-V plane.

    // Y plane

    UINT32 *pLookup_Y = (UINT32*) pLookup;
    WORD *pSrc_Y = (WORD*) pSrc;
    pSrc_Y[0] = 0x0000;    // black

    for (DWORD y = 0; y < dwHeightInPixels; y++)
    {
        WORD *pDest_Y = (WORD*) pDest;

        for (DWORD x = 0; (x + 1) < dwWidthInPixels; x += 2)
        {
#pragma warning(push)
#pragma warning(disable: 6385) 
#pragma warning(disable: 6386) 
            UINT32 uOffset = (pLookup_Y[x] % dwWidthInPixels) + (pLookup_Y[x] / dwWidthInPixels) * lSrcStride;
            pDest_Y[x/2] = pSrc_Y[uOffset/2];
#pragma warning(pop)
        }
        pDest += lDestStride;
        pLookup_Y += dwWidthInPixels;
    }

    // U-V plane

    // NOTE: The U-V plane has 1/2 the number of lines as the Y plane.

    UINT32 *pLookup_UV = (UINT32*) pLookup;
    WORD *pSrc_UV = (WORD*) (pSrc + lSrcStride * dwHeightInPixels);
    pSrc_UV[0] = 0x8080;    // black

    for (DWORD y = 0; y < dwHeightInPixels; y += 2)
    {
        WORD *pDest_UV = (WORD*) pDest;

        for (DWORD x = 0; x < dwWidthInPixels; x += 2)
        {
#pragma warning(push)
#pragma warning(disable: 6385) 
            UINT32 uOffset = (pLookup_UV[x] % dwWidthInPixels) + ((pLookup_UV[x] / dwWidthInPixels) / 2) * lSrcStride;
#pragma warning(pop)
            pDest_UV[x/2] = pSrc_UV[uOffset/2];
        }
        pDest += lDestStride;
        pLookup_UV += dwWidthInPixels * 2;
    }
}



CPolarEffect::CPolarEffect() :
    m_pSample(NULL), m_pInputType(NULL), m_pOutputType(NULL), m_pTransformFn(NULL),
    m_imageWidthInPixels(0), m_imageHeightInPixels(0), m_cbImageSize(0),
    m_pIndexLookup(NULL), m_bStreamingInitialized(false), m_pAttributes(NULL),
    m_pRadiusTransformFn(CPolarEffect::DefaultRadius),
    m_pThetaTransformFn(CPolarEffect::DefaultTheta)
{
    InitializeCriticalSectionEx(&m_critSec, 3000, 0);
}

CPolarEffect::~CPolarEffect()
{
    SafeArrayDelete(&m_pIndexLookup);
    SafeRelease(&m_pInputType);
    SafeRelease(&m_pOutputType);
    SafeRelease(&m_pSample);
    SafeRelease(&m_pAttributes);
    DeleteCriticalSection(&m_critSec);
}

// Initialize the instance.
STDMETHODIMP CPolarEffect::RuntimeClassInitialize()
{
    // Create the attribute store.
    return MFCreateAttributes(&m_pAttributes, 3);
}

// IMediaExtension methods

//-------------------------------------------------------------------
// SetProperties
// Sets the configuration of the effect
//-------------------------------------------------------------------
HRESULT CPolarEffect::SetProperties(ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration)
{
    HRESULT hr = S_OK;

    if(pConfiguration)
    {
        Microsoft::WRL::ComPtr<IInspectable> spInsp;
        Microsoft::WRL::ComPtr<ABI::Windows::Foundation::Collections::IMap<HSTRING, IInspectable *>> spSetting;
        Microsoft::WRL::ComPtr<ABI::Windows::Foundation::IPropertyValue> spPropVal;

        hr = pConfiguration->QueryInterface(IID_PPV_ARGS(&spSetting));
        if (FAILED(hr))
        {
            hr = E_FAIL;
            goto done;
        }

        HStringReference strKey(L"effect");
        hr = spSetting->Lookup(strKey.Get(), spInsp.ReleaseAndGetAddressOf());
        if(FAILED(hr))
        {
            hr = E_INVALIDARG;
            goto done;
        }

        hr = spInsp.As(&spPropVal);
        if(FAILED(hr))
        {
            hr = E_INVALIDARG;
            goto done;
        }

        HString strEffectName;
        hr = spPropVal->GetString(strEffectName.GetAddressOf());
        if(FAILED(hr))
        {
            hr = E_INVALIDARG;
            goto done;
        }

        LPCWSTR pszEffectName = WindowsGetStringRawBuffer(strEffectName.Get(), nullptr);
        if (pszEffectName == nullptr)
        {
            hr = E_INVALIDARG;
            goto done;
        }

        if (wcscmp(pszEffectName, L"Fisheye") == 0)
        {
            m_pRadiusTransformFn = CPolarEffect::FisheyeRadius;
            m_pThetaTransformFn = CPolarEffect::DefaultTheta;
        }
        else if (wcscmp(pszEffectName, L"Pinch") == 0)
        {
            m_pRadiusTransformFn = CPolarEffect::PinchRadius;
            m_pThetaTransformFn = CPolarEffect::DefaultTheta;
        }
        else if (wcscmp(pszEffectName, L"Warp") == 0)
        {
            m_pRadiusTransformFn = CPolarEffect::DefaultRadius;
            m_pThetaTransformFn = CPolarEffect::WarpTheta;
        }
        else
        {
            hr = E_INVALIDARG;
            goto done;
        }
    }

done:
    return hr;
}

// IMFTransform methods. Refer to the Media Foundation SDK documentation for details.

//-------------------------------------------------------------------
// GetStreamLimits
// Returns the minimum and maximum number of streams.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetStreamLimits(
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

HRESULT CPolarEffect::GetStreamCount(
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
// GetStreamIDs
// Returns stream IDs for the input and output streams.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetStreamIDs(
    DWORD   dwInputIDArraySize,
    DWORD   *pdwInputIDs,
    DWORD   dwOutputIDArraySize,
    DWORD   *pdwOutputIDs
)
{
    // It is not required to implement this method if the MFT has a fixed number of
    // streams AND the stream IDs are numbered sequentially from zero (that is, the
    // stream IDs match the stream indexes).

    // In that case, it is OK to return E_NOTIMPL.
    return E_NOTIMPL;
}


//-------------------------------------------------------------------
// GetInputStreamInfo
// Returns information about an input stream.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetInputStreamInfo(
    DWORD                     dwInputStreamID,
    MFT_INPUT_STREAM_INFO *   pStreamInfo
)
{
    if (pStreamInfo == NULL)
    {
        return E_POINTER;
    }

    EnterCriticalSection(&m_critSec);

    if (!IsValidInputStream(dwInputStreamID))
    {
        LeaveCriticalSection(&m_critSec);
        return MF_E_INVALIDSTREAMNUMBER;
    }

    // NOTE: This method should succeed even when there is no media type on the
    //       stream. If there is no media type, we only need to fill in the dwFlags
    //       member of MFT_INPUT_STREAM_INFO. The other members depend on having a
    //       a valid media type.

    pStreamInfo->hnsMaxLatency = 0;
    pStreamInfo->dwFlags = MFT_INPUT_STREAM_WHOLE_SAMPLES | MFT_INPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER;

    if (m_pInputType == NULL)
    {
        pStreamInfo->cbSize = 0;
    }
    else
    {
        pStreamInfo->cbSize = m_cbImageSize;
    }

    pStreamInfo->cbMaxLookahead = 0;
    pStreamInfo->cbAlignment = 0;

    LeaveCriticalSection(&m_critSec);
    return S_OK;
}

//-------------------------------------------------------------------
// GetOutputStreamInfo
// Returns information about an output stream.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetOutputStreamInfo(
    DWORD                     dwOutputStreamID,
    MFT_OUTPUT_STREAM_INFO *  pStreamInfo
)
{
    if (pStreamInfo == NULL)
    {
        return E_POINTER;
    }

    EnterCriticalSection(&m_critSec);

    if (!IsValidOutputStream(dwOutputStreamID))
    {
        LeaveCriticalSection(&m_critSec);
        return MF_E_INVALIDSTREAMNUMBER;
    }

    // NOTE: This method should succeed even when there is no media type on the
    //       stream. If there is no media type, we only need to fill in the dwFlags
    //       member of MFT_OUTPUT_STREAM_INFO. The other members depend on having a
    //       a valid media type.

    pStreamInfo->dwFlags =
        MFT_OUTPUT_STREAM_WHOLE_SAMPLES |
        MFT_OUTPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER |
        MFT_OUTPUT_STREAM_FIXED_SAMPLE_SIZE ;

    if (m_pOutputType == NULL)
    {
        pStreamInfo->cbSize = 0;
    }
    else
    {
        pStreamInfo->cbSize = m_cbImageSize;
    }

    pStreamInfo->cbAlignment = 0;

    LeaveCriticalSection(&m_critSec);
    return S_OK;
}


//-------------------------------------------------------------------
// GetAttributes
// Returns the attributes for the MFT.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetAttributes(IMFAttributes** ppAttributes)
{
    if (ppAttributes == NULL)
    {
        return E_POINTER;
    }

    EnterCriticalSection(&m_critSec);

    *ppAttributes = m_pAttributes;
    (*ppAttributes)->AddRef();

    LeaveCriticalSection(&m_critSec);
    return S_OK;
}


//-------------------------------------------------------------------
// GetInputStreamAttributes
// Returns stream-level attributes for an input stream.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetInputStreamAttributes(
    DWORD           dwInputStreamID,
    IMFAttributes   **ppAttributes
)
{
    // This MFT does not support any stream-level attributes, so the method is not implemented.
    return E_NOTIMPL;
}


//-------------------------------------------------------------------
// GetOutputStreamAttributes
// Returns stream-level attributes for an output stream.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetOutputStreamAttributes(
    DWORD           dwOutputStreamID,
    IMFAttributes   **ppAttributes
)
{
    // This MFT does not support any stream-level attributes, so the method is not implemented.
    return E_NOTIMPL;
}


//-------------------------------------------------------------------
// DeleteInputStream
//-------------------------------------------------------------------

HRESULT CPolarEffect::DeleteInputStream(DWORD dwStreamID)
{
    // This MFT has a fixed number of input streams, so the method is not supported.
    return E_NOTIMPL;
}


//-------------------------------------------------------------------
// AddInputStreams
//-------------------------------------------------------------------

HRESULT CPolarEffect::AddInputStreams(
    DWORD   cStreams,
    DWORD   *adwStreamIDs
)
{
    // This MFT has a fixed number of output streams, so the method is not supported.
    return E_NOTIMPL;
}


//-------------------------------------------------------------------
// GetInputAvailableType
// Returns a preferred input type.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetInputAvailableType(
    DWORD           dwInputStreamID,
    DWORD           dwTypeIndex, // 0-based
    IMFMediaType    **ppType
)
{
    if (ppType == NULL)
    {
        return E_INVALIDARG;
    }

    EnterCriticalSection(&m_critSec);

    if (!IsValidInputStream(dwInputStreamID))
    {
        LeaveCriticalSection(&m_critSec);
        return MF_E_INVALIDSTREAMNUMBER;
    }

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
        *ppType = m_pOutputType;
        (*ppType)->AddRef();
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}



//-------------------------------------------------------------------
// GetOutputAvailableType
// Returns a preferred output type.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetOutputAvailableType(
    DWORD           dwOutputStreamID,
    DWORD           dwTypeIndex, // 0-based
    IMFMediaType    **ppType
)
{
    if (ppType == NULL)
    {
        return E_INVALIDARG;
    }

    EnterCriticalSection(&m_critSec);

    if (!IsValidOutputStream(dwOutputStreamID))
    {
        LeaveCriticalSection(&m_critSec);
        return MF_E_INVALIDSTREAMNUMBER;
    }

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
        *ppType = m_pInputType;
        (*ppType)->AddRef();
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// SetInputType
//-------------------------------------------------------------------

HRESULT CPolarEffect::SetInputType(
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

    EnterCriticalSection(&m_critSec);

    if (!IsValidInputStream(dwInputStreamID))
    {
        LeaveCriticalSection(&m_critSec);
        return MF_E_INVALIDSTREAMNUMBER;
    }

    HRESULT hr = S_OK;

    // Does the caller want us to set the type, or just test it?
    BOOL bReallySet = ((dwFlags & MFT_SET_TYPE_TEST_ONLY) == 0);

    // If we have an input sample, the client cannot change the type now.
    if (HasPendingOutput())
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
    if (bReallySet)
    {
        OnSetInputType(pType);

        // When the type changes, end streaming.
        hr = EndStreaming();
    }

done:
    LeaveCriticalSection(&m_critSec);
    return hr;
}



//-------------------------------------------------------------------
// SetOutputType
//-------------------------------------------------------------------

HRESULT CPolarEffect::SetOutputType(
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

    EnterCriticalSection(&m_critSec);

    if (!IsValidOutputStream(dwOutputStreamID))
    {
        LeaveCriticalSection(&m_critSec);
        return MF_E_INVALIDSTREAMNUMBER;
    }

    HRESULT hr = S_OK;

    // Does the caller want us to set the type, or just test it?
    BOOL bReallySet = ((dwFlags & MFT_SET_TYPE_TEST_ONLY) == 0);

    // If we have an input sample, the client cannot change the type now.
    if (HasPendingOutput())
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
    if (bReallySet)
    {
        OnSetOutputType(pType);

        // When the type changes, end streaming.
        hr = EndStreaming();
    }

done:
    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// GetInputCurrentType
// Returns the current input type.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetInputCurrentType(
    DWORD           dwInputStreamID,
    IMFMediaType    **ppType
)
{
    if (ppType == NULL)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;

    EnterCriticalSection(&m_critSec);

    if (!IsValidInputStream(dwInputStreamID))
    {
        hr = MF_E_INVALIDSTREAMNUMBER;
    }
    else if (!m_pInputType)
    {
        hr = MF_E_TRANSFORM_TYPE_NOT_SET;
    }
    else
    {
        *ppType = m_pInputType;
        (*ppType)->AddRef();
    }
    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// GetOutputCurrentType
// Returns the current output type.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetOutputCurrentType(
    DWORD           dwOutputStreamID,
    IMFMediaType    **ppType
)
{
    if (ppType == NULL)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;

    EnterCriticalSection(&m_critSec);

    if (!IsValidOutputStream(dwOutputStreamID))
    {
        hr = MF_E_INVALIDSTREAMNUMBER;
    }
    else if (!m_pOutputType)
    {
        hr = MF_E_TRANSFORM_TYPE_NOT_SET;
    }
    else
    {
        *ppType = m_pOutputType;
        (*ppType)->AddRef();
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// GetInputStatus
// Query if the MFT is accepting more input.
//-------------------------------------------------------------------

HRESULT CPolarEffect::GetInputStatus(
    DWORD           dwInputStreamID,
    DWORD           *pdwFlags
)
{
    if (pdwFlags == NULL)
    {
        return E_POINTER;
    }

    EnterCriticalSection(&m_critSec);

    if (!IsValidInputStream(dwInputStreamID))
    {
        LeaveCriticalSection(&m_critSec);
        return MF_E_INVALIDSTREAMNUMBER;
    }

    // If an input sample is already queued, do not accept another sample until the 
    // client calls ProcessOutput or Flush.

    // NOTE: It is possible for an MFT to accept more than one input sample. For 
    // example, this might be required in a video decoder if the frames do not 
    // arrive in temporal order. In the case, the decoder must hold a queue of 
    // samples. For the video effect, each sample is transformed independently, so
    // there is no reason to queue multiple input samples.

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

HRESULT CPolarEffect::GetOutputStatus(DWORD *pdwFlags)
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
// SetOutputBounds
// Sets the range of time stamps that the MFT will output.
//-------------------------------------------------------------------

HRESULT CPolarEffect::SetOutputBounds(
    LONGLONG        hnsLowerBound,
    LONGLONG        hnsUpperBound
)
{
    // Implementation of this method is optional.
    return E_NOTIMPL;
}


//-------------------------------------------------------------------
// ProcessEvent
// Sends an event to an input stream.
//-------------------------------------------------------------------

HRESULT CPolarEffect::ProcessEvent(
    DWORD              dwInputStreamID,
    IMFMediaEvent      *pEvent
)
{
    // This MFT does not handle any stream events, so the method can
    // return E_NOTIMPL. This tells the pipeline that it can stop
    // sending any more events to this MFT.
    return E_NOTIMPL;
}


//-------------------------------------------------------------------
// ProcessMessage
//-------------------------------------------------------------------

HRESULT CPolarEffect::ProcessMessage(
    MFT_MESSAGE_TYPE    eMessage,
    ULONG_PTR           ulParam
)
{
    EnterCriticalSection(&m_critSec);

    HRESULT hr = S_OK;

    switch (eMessage)
    {
    case MFT_MESSAGE_COMMAND_FLUSH:
        // Flush the MFT.
        hr = OnFlush();
        break;

    case MFT_MESSAGE_COMMAND_DRAIN:
        // Drain: Tells the MFT to reject further input until all pending samples are
        // processed. That is our default behavior already, so there is nothing to do.
        //
        // For a decoder that accepts a queue of samples, the MFT might need to drain
        // the queue in response to this command.
    break;

    case MFT_MESSAGE_SET_D3D_MANAGER:
        // Sets a pointer to the IDirect3DDeviceManager9 interface.

        // The pipeline should never send this message unless the MFT sets the MF_SA_D3D_AWARE 
        // attribute set to TRUE. Because this MFT does not set MF_SA_D3D_AWARE, it is an error
        // to send the MFT_MESSAGE_SET_D3D_MANAGER message to the MFT. Return an error code in
        // this case.

        // NOTE: If this MFT were D3D-enabled, it would cache the IDirect3DDeviceManager9 
        // pointer for use during streaming.

        hr = E_NOTIMPL;
        break;

    case MFT_MESSAGE_NOTIFY_BEGIN_STREAMING:
        hr = BeginStreaming();
        break;

    case MFT_MESSAGE_NOTIFY_END_STREAMING:
        hr = EndStreaming();
        break;

    // The next two messages do not require any action from this MFT.

    case MFT_MESSAGE_NOTIFY_END_OF_STREAM:
        break;

    case MFT_MESSAGE_NOTIFY_START_OF_STREAM:
        break;
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// ProcessInput
// Process an input sample.
//-------------------------------------------------------------------

HRESULT CPolarEffect::ProcessInput(
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

    HRESULT hr = S_OK;

    EnterCriticalSection(&m_critSec);

    // Validate the input stream number.
    if (!IsValidInputStream(dwInputStreamID))
    {
        hr = MF_E_INVALIDSTREAMNUMBER;
        goto done;
    }

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
    pSample->AddRef();  // Hold a reference count on the sample.

done:
    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// ProcessOutput
// Process an output sample.
//-------------------------------------------------------------------

HRESULT CPolarEffect::ProcessOutput(
    DWORD                   dwFlags,
    DWORD                   cOutputBufferCount,
    MFT_OUTPUT_DATA_BUFFER  *pOutputSamples, // one per stream
    DWORD                   *pdwStatus
)
{
    // Check input parameters...

    // This MFT does not accept any flags for the dwFlags parameter.

    // The only defined flag is MFT_PROCESS_OUTPUT_DISCARD_WHEN_NO_BUFFER. This flag 
    // applies only when the MFT marks an output stream as lazy or optional. But this
    // MFT has no lazy or optional streams, so the flag is not valid.

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
    if (pOutputSamples[0].pSample == NULL)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;

    IMFMediaBuffer *pInput = NULL;
    IMFMediaBuffer *pOutput = NULL;

    EnterCriticalSection(&m_critSec);

    // There must be an input sample available for processing.
    if (m_pSample == NULL)
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

    // Get the input buffer.
    hr = m_pSample->ConvertToContiguousBuffer(&pInput);
    if (FAILED(hr))
    {
        goto done;
    }

    // Get the output buffer.
    hr = pOutputSamples[0].pSample->ConvertToContiguousBuffer(&pOutput);
    if (FAILED(hr))
    {
        goto done;
    }

    hr = OnProcessOutput(pInput, pOutput);
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

done:
    SafeRelease(&m_pSample);   // Release our input sample.
    SafeRelease(&pInput);
    SafeRelease(&pOutput);
    LeaveCriticalSection(&m_critSec);
    return hr;
}

// PRIVATE METHODS

// All methods that follow are private to this MFT and are not part of the IMFTransform interface.

// Create a partial media type from our list.
//
// dwTypeIndex: Index into the list of peferred media types.
// ppmt:        Receives a pointer to the media type.

HRESULT CPolarEffect::OnGetPartialType(DWORD dwTypeIndex, IMFMediaType **ppmt)
{
    if (dwTypeIndex >= ARRAYSIZE(g_MediaSubtypes))
    {
        return MF_E_NO_MORE_TYPES;
    }

    IMFMediaType *pmt = NULL;

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

    hr = pmt->SetGUID(MF_MT_SUBTYPE, g_MediaSubtypes[dwTypeIndex]);
    if (FAILED(hr))
    {
        goto done;
    }

    *ppmt = pmt;
    (*ppmt)->AddRef();

done:
    SafeRelease(&pmt);
    return hr;
}


// Validate an input media type.

HRESULT CPolarEffect::OnCheckInputType(IMFMediaType *pmt)
{
    assert(pmt != NULL);

    HRESULT hr = S_OK;

    // If the output type is set, see if they match.
    if (m_pOutputType != NULL)
    {
        DWORD flags = 0;
        hr = pmt->IsEqual(m_pOutputType, &flags);

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

HRESULT CPolarEffect::OnCheckOutputType(IMFMediaType *pmt)
{
    assert(pmt != NULL);

    HRESULT hr = S_OK;

    // If the input type is set, see if they match.
    if (m_pInputType != NULL)
    {
        DWORD flags = 0;
        hr = pmt->IsEqual(m_pInputType, &flags);

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

HRESULT CPolarEffect::OnCheckMediaType(IMFMediaType *pmt)
{
    BOOL bFoundMatchingSubtype = FALSE;

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

    // Subtype must be one of the subtypes in our global list.

    // Get the subtype GUID.
    GUID subtype;
    hr = pmt->GetGUID(MF_MT_SUBTYPE, &subtype);
    if (FAILED(hr))
    {
        goto done;
    }

    // Look for the subtype in our list of accepted types.
    for (DWORD i = 0; i < ARRAYSIZE(g_MediaSubtypes); i++)
    {
        if (subtype == g_MediaSubtypes[i])
        {
            bFoundMatchingSubtype = TRUE;
            break;
        }
    }

    if (!bFoundMatchingSubtype)
    {
        hr = MF_E_INVALIDMEDIATYPE; // The MFT does not support this subtype.
        goto done;
    }

    // Reject single-field media types. 
    UINT32 interlace = MFGetAttributeUINT32(pmt, MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive);
    if (interlace == MFVideoInterlace_FieldSingleUpper  || interlace == MFVideoInterlace_FieldSingleLower)
    {
        hr = MF_E_INVALIDMEDIATYPE;
    }

done:
    return hr;
}


// Set or clear the input media type.
//
// Prerequisite: The input type was already validated.

void CPolarEffect::OnSetInputType(IMFMediaType *pmt)
{
    // if pmt is NULL, clear the type.
    // if pmt is non-NULL, set the type.

    SafeRelease(&m_pInputType);
    m_pInputType = pmt;
    if (m_pInputType)
    {
        m_pInputType->AddRef();
    }

    // Update the format information.
    UpdateFormatInfo();
}


// Set or clears the output media type.
//
// Prerequisite: The output type was already validated.

void CPolarEffect::OnSetOutputType(IMFMediaType *pmt)
{
    // If pmt is NULL, clear the type. Otherwise, set the type.

    SafeRelease(&m_pOutputType);
    m_pOutputType = pmt;
    if (m_pOutputType)
    {
        m_pOutputType->AddRef();
    }
}


// Initialize streaming parameters.
//
// This method is called if the client sends the MFT_MESSAGE_NOTIFY_BEGIN_STREAMING
// message, or when the client processes a sample, whichever happens first.

HRESULT CPolarEffect::BeginStreaming()
{
    HRESULT hr = S_OK;

    if (m_pIndexLookup == NULL)
    {
        UINT32 unVideoArea = m_imageWidthInPixels * m_imageHeightInPixels;

        m_pIndexLookup = new UINT32[unVideoArea];

        if (m_pIndexLookup != NULL)
        {
            ZeroMemory( m_pIndexLookup, unVideoArea * sizeof(UINT32) );

            GeneratePolarLookup( m_imageWidthInPixels, 
                                 m_imageHeightInPixels, 
                                 m_pIndexLookup );
        }
        else
        {
            hr = E_OUTOFMEMORY;
        }
    }

    if (SUCCEEDED(hr))
    {
        m_bStreamingInitialized = true;
    }

    return hr;
}


// End streaming. 

// This method is called if the client sends an MFT_MESSAGE_NOTIFY_END_STREAMING
// message, or when the media type changes. In general, it should be called whenever
// the streaming parameters need to be reset.

HRESULT CPolarEffect::EndStreaming()
{
    SafeArrayDelete(&m_pIndexLookup);
    m_bStreamingInitialized = false;
    return S_OK;
}



// Generate output data.

HRESULT CPolarEffect::OnProcessOutput(IMFMediaBuffer *pIn, IMFMediaBuffer *pOut)
{
    BYTE *pDest = NULL;         // Destination buffer.
    LONG lDestStride = 0;       // Destination stride.

    BYTE *pSrc = NULL;          // Source buffer.
    LONG lSrcStride = 0;        // Source stride.

    // Helper objects to lock the buffers.
    VideoBufferLock inputLock(pIn);
    VideoBufferLock outputLock(pOut);

    // Stride if the buffer does not support IMF2DBuffer
    LONG lDefaultStride = 0;

    HRESULT hr = GetDefaultStride(m_pInputType, &lDefaultStride);
    if (FAILED(hr))
    {
        goto done;
    }

    // Lock the input buffer.
    hr = inputLock.LockBuffer(lDefaultStride, m_imageHeightInPixels, &pSrc, &lSrcStride);
    if (FAILED(hr))
    {
        goto done;
    }

    // Lock the output buffer.
    hr = outputLock.LockBuffer(lDefaultStride, m_imageHeightInPixels, &pDest, &lDestStride);
    if (FAILED(hr))
    {
        goto done;
    }

    // Invoke the image transform function.
    assert (m_pTransformFn != NULL);
    if (m_pTransformFn)
    {
        (*m_pTransformFn)(m_pIndexLookup, pDest, lDestStride, pSrc, lSrcStride, m_imageWidthInPixels, m_imageHeightInPixels);
    }
    else
    {
        hr = E_UNEXPECTED;
        goto done;
    }


    // Set the data size on the output buffer.
    hr = pOut->SetCurrentLength(m_cbImageSize);

    // The VideoBufferLock class automatically unlocks the buffers.
done:
    return hr;
}


// Flush the MFT.

HRESULT CPolarEffect::OnFlush()
{
    // For this MFT, flushing just means releasing the input sample.
    SafeRelease(&m_pSample);
    return S_OK;
}


// Update the format information. This method is called whenever the
// input type is set.

HRESULT CPolarEffect::UpdateFormatInfo()
{
    HRESULT hr = S_OK;

    GUID subtype = GUID_NULL;

    m_imageWidthInPixels = 0;
    m_imageHeightInPixels = 0;
    m_cbImageSize = 0;

    m_pTransformFn = NULL;

    if (m_pInputType != NULL)
    {
        hr = m_pInputType->GetGUID(MF_MT_SUBTYPE, &subtype);
        if (FAILED(hr))
        {
            goto done;
        }
        if (subtype == MFVideoFormat_YUY2)
        {
            m_pTransformFn = TransformImage_YUY2;
        }
        else if (subtype == MFVideoFormat_UYVY)
        {
            m_pTransformFn = TransformImage_UYVY;
        }
        else if (subtype == MFVideoFormat_NV12)
        {
            m_pTransformFn = TransformImage_NV12;
        }
        else
        {
            hr = E_UNEXPECTED;
            goto done;
        }

        hr = MFGetAttributeSize(m_pInputType, MF_MT_FRAME_SIZE, &m_imageWidthInPixels, &m_imageHeightInPixels);
        if (FAILED(hr))
        {
            goto done;
        }

        // Calculate the image size (not including padding)
        hr = GetImageSize(subtype.Data1, m_imageWidthInPixels, m_imageHeightInPixels, &m_cbImageSize);
    }

done:
    return hr;
}


// Create a lookup table which stores the result of polar transformation

// By controlling the radius (m_pRadiusTransformFn) and angle
// (m_pThetaTransformFn) functions, different effects can be created

void CPolarEffect::GeneratePolarLookup(UINT32 unWidth, 
                                            UINT32 unHeight, 
                                            UINT32 *pIndexLookup )
{
    LONG x, y;
    UINT32 *pIndex;
    LONG lMaxX, lMaxY;
    LONG lHalfWidth, lHalfHeight;
    LONG lTransDx, lTransDy;
    DOUBLE dRadius, dMaxRadius, dNewRadius;
    DOUBLE dTheta, dNewTheta;

    assert(m_pRadiusTransformFn != NULL);
    assert(m_pThetaTransformFn != NULL);

    lMaxX = (LONG)unWidth;
    lMaxY = (LONG)unHeight;
    lHalfWidth = unWidth / 2;
    lHalfHeight = unHeight / 2;
    dMaxRadius = floor( sqrtf( static_cast<float>(lHalfWidth*lHalfWidth+lHalfHeight*lHalfHeight) ) );

    pIndex = pIndexLookup;

    for(y = 0; y < lMaxY; y++)
    {
        for(x = 0; x < lMaxX; x++)
        {
            lTransDx = lHalfWidth - x;
            lTransDy = lHalfHeight - y;

            dRadius = ceil( sqrtf( static_cast<float>(lTransDx*lTransDx + lTransDy*lTransDy) ) );

            if( dRadius < dMaxRadius )
            {
                dTheta = atan2f( static_cast<float> (lTransDy), static_cast<float> (lTransDx) );

                dNewRadius = m_pRadiusTransformFn(dRadius, dMaxRadius, dTheta);
                dNewTheta = m_pThetaTransformFn(dRadius, dMaxRadius, dTheta);

                lTransDx = lHalfWidth - (LONG)(dNewRadius * cos(dNewTheta));
                lTransDy = lHalfHeight - (LONG)(dNewRadius * sin(dNewTheta));

                if( 0 <= lTransDx && lTransDx < lMaxX &&
                    0 <= lTransDy && lTransDy < lMaxY )
                {
                    *pIndex++ = lTransDy * lMaxX + lTransDx;
                }
                else
                {
                    *pIndex++ = 0;
                }
            }
            else
            {
                *pIndex++ = 0;
            }
        }
    }
}


// Calculate the size of the buffer needed to store the image.

// fcc: The FOURCC code of the video format.

HRESULT GetImageSize(DWORD fcc, UINT32 width, UINT32 height, DWORD* pcbImage)
{
    HRESULT hr = S_OK;

    switch (fcc)
    {
    case FOURCC_YUY2:
    case FOURCC_UYVY:
        // check overflow
        if ((width > MAXDWORD / 2) || (width * 2 > MAXDWORD / height))
        {
            hr = E_INVALIDARG;
        }
        else
        {   
            // 16 bpp
            *pcbImage = width * height * 2;
        }
        break;

    case FOURCC_NV12:
        // check overflow
        if ((height/2 > MAXDWORD - height) || ((height + height/2) > MAXDWORD / width))
        {
            hr = E_INVALIDARG;
        }
        else
        {
            // 12 bpp
            *pcbImage = width * (height + (height/2));
        }
        break;

    default:
        hr = E_FAIL;    // Unsupported type.
    }
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
            if (subtype == MFVideoFormat_NV12)
            {
                lStride = width;
            }
            else if (subtype == MFVideoFormat_YUY2 || subtype == MFVideoFormat_UYVY)
            {
                lStride = ((width * 2) + 3) & ~3;
            }
            else
            {
                hr = E_INVALIDARG;
            }
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

