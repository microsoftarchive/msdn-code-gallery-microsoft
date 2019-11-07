//////////////////////////////////////////////////////////////////////////
//
// decoder.cpp
// Implements the MPEG-1 decoder.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//////////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "decoder.h"
#include "VideoBufferLock.h"
#include <wrl\module.h>

ActivatableClass(CDecoder);

void    OurFillRect(PBYTE pbScanline0, DWORD dwWidth, DWORD dwHeight, LONG lStrideInBytes, COLORREF color);

#ifndef LODWORD
#define LODWORD(x)  ((DWORD)(((DWORD_PTR)(x)) & 0xffffffff))
#endif

const UINT32 MAX_VIDEO_WIDTH = 4095;        // per ISO/IEC 11172-2
const UINT32 MAX_VIDEO_HEIGHT = 4095;

//-------------------------------------------------------------------
// CDecoder class
//-------------------------------------------------------------------

CDecoder::CDecoder() :
    m_pbData(nullptr),
    m_cbData(0),
    m_imageWidthInPixels(0),
    m_imageHeightInPixels(0),
    m_cbImageSize(0),
    m_rtFrame(0),
    m_rtLength(0),
    m_fPicture(false),
    m_fLowLatencyMode(false)
{
    m_frameRate.Numerator = m_frameRate.Denominator = 0;

}

CDecoder::~CDecoder()
{
}

// IMediaExtension methods

//-------------------------------------------------------------------
// Name: SetProperties
// Sets the configuration of the decoder
//-------------------------------------------------------------------
IFACEMETHODIMP CDecoder::SetProperties (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration)
{
    return S_OK;
}

// IMFTransform methods. Refer to the Media Foundation SDK documentation for details.

//-------------------------------------------------------------------
// Name: GetStreamLimits
// Returns the minimum and maximum number of streams.
//-------------------------------------------------------------------

HRESULT CDecoder::GetStreamLimits(
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
// Name: GetStreamCount
// Returns the actual number of streams.
//-------------------------------------------------------------------

HRESULT CDecoder::GetStreamCount(
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
// Name: GetStreamIDs
// Returns stream IDs for the input and output streams.
//-------------------------------------------------------------------

HRESULT CDecoder::GetStreamIDs(
    DWORD   /*dwInputIDArraySize*/,
    DWORD   * /*pdwInputIDs*/,
    DWORD   /*dwOutputIDArraySize*/,
    DWORD   * /*pdwOutputIDs*/
)
{
    // Do not need to implement, because this MFT has a fixed number of
    // streams and the stream IDs match the stream indexes.
    return E_NOTIMPL;
}


//-------------------------------------------------------------------
// Name: GetInputStreamInfo
// Returns information about an input stream.
//-------------------------------------------------------------------

HRESULT CDecoder::GetInputStreamInfo(
    DWORD                     dwInputStreamID,
    MFT_INPUT_STREAM_INFO *   pStreamInfo
)
{
    if (pStreamInfo == nullptr)
    {
        return E_POINTER;
    }

    if (!IsValidInputStream(dwInputStreamID))
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    pStreamInfo->hnsMaxLatency = 0;

    //  We can process data on any boundary.
    pStreamInfo->dwFlags = 0;

    pStreamInfo->cbSize = 1;
    pStreamInfo->cbMaxLookahead = 0;
    pStreamInfo->cbAlignment = 1;

    return S_OK;
}



//-------------------------------------------------------------------
// Name: GetOutputStreamInfo
// Returns information about an output stream.
//-------------------------------------------------------------------

HRESULT CDecoder::GetOutputStreamInfo(
    DWORD                     dwOutputStreamID,
    MFT_OUTPUT_STREAM_INFO *  pStreamInfo
)
{
    if (pStreamInfo == nullptr)
    {
        return E_POINTER;
    }

    if (!IsValidOutputStream(dwOutputStreamID))
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    AutoLock lock(m_critSec);

    // NOTE: This method should succeed even when there is no media type on the
    //       stream. If there is no media type, we only need to fill in the dwFlags
    //       member of MFT_OUTPUT_STREAM_INFO. The other members depend on having a
    //       a valid media type.

    pStreamInfo->dwFlags =
        MFT_OUTPUT_STREAM_WHOLE_SAMPLES |
        MFT_OUTPUT_STREAM_SINGLE_SAMPLE_PER_BUFFER |
        MFT_OUTPUT_STREAM_FIXED_SAMPLE_SIZE ;

    if (m_spOutputType == nullptr)
    {
        pStreamInfo->cbSize = 0;
        pStreamInfo->cbAlignment = 0;
    }
    else
    {
        pStreamInfo->cbSize = m_cbImageSize;
        pStreamInfo->cbAlignment = 1;
    }

    return S_OK;
}



//-------------------------------------------------------------------
// Name: GetAttributes
// Returns the attributes for the MFT.
//-------------------------------------------------------------------

HRESULT CDecoder::GetAttributes(IMFAttributes** /*pAttributes*/)
{
    // This MFT does not support any attributes, so the method is not implemented.
    return E_NOTIMPL;
}



//-------------------------------------------------------------------
// Name: GetInputStreamAttributes
// Returns stream-level attributes for an input stream.
//-------------------------------------------------------------------

HRESULT CDecoder::GetInputStreamAttributes(
    DWORD           /*dwInputStreamID*/,
    IMFAttributes   ** /*ppAttributes*/
)
{
    // This MFT does not support any attributes, so the method is not implemented.
    return E_NOTIMPL;
}



//-------------------------------------------------------------------
// Name: GetOutputStreamAttributes
// Returns stream-level attributes for an output stream.
//-------------------------------------------------------------------

HRESULT CDecoder::GetOutputStreamAttributes(
    DWORD           /*dwOutputStreamID*/,
    IMFAttributes   ** /*ppAttributes*/
)
{
    // This MFT does not support any attributes, so the method is not implemented.
    return E_NOTIMPL;
}



//-------------------------------------------------------------------
// Name: DeleteInputStream
//-------------------------------------------------------------------

HRESULT CDecoder::DeleteInputStream(DWORD /*dwStreamID*/)
{
    // This MFT has a fixed number of input streams, so the method is not implemented.
    return E_NOTIMPL;
}



//-------------------------------------------------------------------
// Name: AddInputStreams
//-------------------------------------------------------------------

HRESULT CDecoder::AddInputStreams(
    DWORD   /*cStreams*/,
    DWORD   * /*adwStreamIDs*/
)
{
    // This MFT has a fixed number of output streams, so the method is not implemented.
    return E_NOTIMPL;
}



//-------------------------------------------------------------------
// Name: GetInputAvailableType
// Description: Return a preferred input type.
//-------------------------------------------------------------------

HRESULT CDecoder::GetInputAvailableType(
    DWORD           /*dwInputStreamID*/,
    DWORD           /*dwTypeIndex*/,
    IMFMediaType    ** /*ppType*/
)
{
    return MF_E_NO_MORE_TYPES;
}



//-------------------------------------------------------------------
// Name: GetOutputAvailableType
// Description: Return a preferred output type.
//-------------------------------------------------------------------

HRESULT CDecoder::GetOutputAvailableType(
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

        if (!IsValidOutputStream(dwOutputStreamID))
        {
            ThrowException(MF_E_INVALIDSTREAMNUMBER);
        }

        if (dwTypeIndex != 0)
        {
            return MF_E_NO_MORE_TYPES;
        }

        AutoLock lock(m_critSec);

        ComPtr<IMFMediaType> spOutputType;

        if (m_spInputType == nullptr)
        {
            return MF_E_TRANSFORM_TYPE_NOT_SET;
        }

        ThrowIfError(MFCreateMediaType(&spOutputType));
        ThrowIfError(spOutputType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video));
        ThrowIfError(spOutputType->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_RGB32));
        ThrowIfError(spOutputType->SetUINT32(MF_MT_FIXED_SIZE_SAMPLES, TRUE));
        ThrowIfError(spOutputType->SetUINT32(MF_MT_ALL_SAMPLES_INDEPENDENT, TRUE));
        ThrowIfError(spOutputType->SetUINT32(MF_MT_SAMPLE_SIZE, m_imageHeightInPixels * m_imageWidthInPixels * 4));
        ThrowIfError(MFSetAttributeSize(spOutputType.Get(), MF_MT_FRAME_SIZE, m_imageWidthInPixels, m_imageHeightInPixels));
        ThrowIfError(MFSetAttributeRatio(spOutputType.Get(), MF_MT_FRAME_RATE, m_frameRate.Numerator, m_frameRate.Denominator));
        ThrowIfError(spOutputType->SetUINT32(MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive));
        ThrowIfError(MFSetAttributeRatio(spOutputType.Get(), MF_MT_PIXEL_ASPECT_RATIO, 1, 1));

        *ppType = spOutputType.Detach();
    }
    catch (Exception ^exc)
    {
        hr = exc->HResult;
    }

    return hr;
}



//-------------------------------------------------------------------
// Name: SetInputType
//-------------------------------------------------------------------

HRESULT CDecoder::SetInputType(
    DWORD           dwInputStreamID,
    IMFMediaType    *pType, // Can be nullptr to clear the input type.
    DWORD           dwFlags
)
{
    HRESULT hr = S_OK;

    try
    {
        if (!IsValidInputStream(dwInputStreamID))
        {
            ThrowException(MF_E_INVALIDSTREAMNUMBER);
        }

        // Validate flags.
        if (dwFlags & ~MFT_SET_TYPE_TEST_ONLY)
        {
            throw ref new InvalidArgumentException();
        }

        AutoLock lock(m_critSec);

        // Does the caller want us to set the type, or just test it?
        bool fReallySet = ((dwFlags & MFT_SET_TYPE_TEST_ONLY) == 0);

        // If we have an input sample, the client cannot change the type now.
        if (HasPendingOutput())
        {
            ThrowException(MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING);
        }

        // Validate the type, if non-nullptr.
        if (pType != nullptr)
        {
            OnCheckInputType(pType);
        }

        // The type is OK.
        // Set the type, unless the caller was just testing.
        if (fReallySet)
        {
            OnSetInputType(pType);
        }
    }
    catch (Exception ^exc)
    {
        hr = exc->HResult;
    }
        
    return hr;
}



//-------------------------------------------------------------------
// Name: SetOutputType
//-------------------------------------------------------------------

HRESULT CDecoder::SetOutputType(
    DWORD           dwOutputStreamID,
    IMFMediaType    *pType, // Can be nullptr to clear the output type.
    DWORD           dwFlags
)
{
    HRESULT hr = S_OK;
    try
    {
        if (!IsValidOutputStream(dwOutputStreamID))
        {
            return MF_E_INVALIDSTREAMNUMBER;
        }

        // Validate flags.
        if (dwFlags & ~MFT_SET_TYPE_TEST_ONLY)
        {
            return E_INVALIDARG;
        }

        AutoLock lock(m_critSec);

        // Does the caller want us to set the type, or just test it?
        bool fReallySet = ((dwFlags & MFT_SET_TYPE_TEST_ONLY) == 0);

        // If we have an input sample, the client cannot change the type now.
        if (HasPendingOutput())
        {
            ThrowException(MF_E_TRANSFORM_CANNOT_CHANGE_MEDIATYPE_WHILE_PROCESSING);
        }

        // Validate the type, if non-nullptr.
        if (pType != nullptr)
        {
            OnCheckOutputType(pType);
        }

        if (fReallySet)
        {
            // The type is OK.
            // Set the type, unless the caller was just testing.
            OnSetOutputType(pType);
        }
    }
    catch (Exception ^exc)
    {
        hr = exc->HResult;
    }

    return hr;
}



//-------------------------------------------------------------------
// Name: GetInputCurrentType
// Description: Returns the current input type.
//-------------------------------------------------------------------

HRESULT CDecoder::GetInputCurrentType(
    DWORD           dwInputStreamID,
    IMFMediaType    **ppType
)
{
    if (ppType == nullptr)
    {
        return E_POINTER;
    }

    if (!IsValidInputStream(dwInputStreamID))
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    AutoLock lock(m_critSec);

    HRESULT hr = S_OK;

    if (m_spInputType == nullptr)
    {
        hr = MF_E_TRANSFORM_TYPE_NOT_SET;
    }

    if (SUCCEEDED(hr))
    {
        *ppType = m_spInputType.Get();
        (*ppType)->AddRef();
    }

    return hr;
}



//-------------------------------------------------------------------
// Name: GetOutputCurrentType
// Description: Returns the current output type.
//-------------------------------------------------------------------

HRESULT CDecoder::GetOutputCurrentType(
    DWORD           dwOutputStreamID,
    IMFMediaType    **ppType
)
{
    if (ppType == nullptr)
    {
        return E_POINTER;
    }

    if (!IsValidOutputStream(dwOutputStreamID))
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    AutoLock lock(m_critSec);

    HRESULT hr = S_OK;

    if (m_spOutputType == nullptr)
    {
        hr = MF_E_TRANSFORM_TYPE_NOT_SET;
    }

    if (SUCCEEDED(hr))
    {
        *ppType = m_spOutputType.Get();
        (*ppType)->AddRef();
    }

    return hr;
}



//-------------------------------------------------------------------
// Name: GetInputStatus
// Description: Query if the MFT is accepting more input.
//-------------------------------------------------------------------

HRESULT CDecoder::GetInputStatus(
    DWORD           dwInputStreamID,
    DWORD           *pdwFlags
)
{
    if (pdwFlags == nullptr)
    {
        return E_POINTER;
    }

    if (!IsValidInputStream(dwInputStreamID))
    {
        return MF_E_INVALIDSTREAMNUMBER;
    }

    AutoLock lock(m_critSec);

    // If we already have an input sample, we don't accept
    // another one until the client calls ProcessOutput or Flush.
    if (HasPendingOutput())
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
// Name: GetOutputStatus
// Description: Query if the MFT can produce output.
//-------------------------------------------------------------------

HRESULT CDecoder::GetOutputStatus(DWORD *pdwFlags)
{
    if (pdwFlags == nullptr)
    {
        return E_POINTER;
    }

    AutoLock lock(m_critSec);

    // We can produce an output sample if (and only if)
    // we have an input sample.
    if (HasPendingOutput())
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
// Name: SetOutputBounds
// Sets the range of time stamps that the MFT will output.
//-------------------------------------------------------------------

HRESULT CDecoder::SetOutputBounds(
    LONGLONG        /*hnsLowerBound*/,
    LONGLONG        /*hnsUpperBound*/
)
{
    // Implementation of this method is optional.
    return E_NOTIMPL;
}



//-------------------------------------------------------------------
// Name: ProcessEvent
// Sends an event to an input stream.
//-------------------------------------------------------------------

HRESULT CDecoder::ProcessEvent(
    DWORD              /*dwInputStreamID*/,
    IMFMediaEvent      * /*pEvent */
)
{
    // This MFT does not handle any stream events, so the method can
    // return E_NOTIMPL. This tells the pipeline that it can stop
    // sending any more events to this MFT.
    return E_NOTIMPL;
}



//-------------------------------------------------------------------
// Name: ProcessMessage
//-------------------------------------------------------------------

HRESULT CDecoder::ProcessMessage(
    MFT_MESSAGE_TYPE    eMessage,
    ULONG_PTR           /*ulParam*/
)
{
    AutoLock lock(m_critSec);

    HRESULT hr = S_OK;

    try
    {
        switch (eMessage)
        {
        case MFT_MESSAGE_COMMAND_FLUSH:
            // Flush the MFT.
            OnFlush();
            break;

        case MFT_MESSAGE_COMMAND_DRAIN:
            // Set the discontinuity flag on all of the input.
            OnDiscontinuity();
            break;

        case MFT_MESSAGE_SET_D3D_MANAGER:
            // The pipeline should never send this message unless the MFT
            // has the MF_SA_D3D_AWARE attribute set to TRUE. However, if we
            // do get this message, it's invalid and we don't implement it.
            ThrowException(E_NOTIMPL);
            break;


        case MFT_MESSAGE_NOTIFY_BEGIN_STREAMING:
            AllocateStreamingResources();
            break;

        case MFT_MESSAGE_NOTIFY_END_STREAMING:
            FreeStreamingResources();
            break;

            // These messages do not require a response.
        case MFT_MESSAGE_NOTIFY_START_OF_STREAM:
        case MFT_MESSAGE_NOTIFY_END_OF_STREAM:
            break;

        }
    }
    catch (Exception ^exc)
    {
        hr = exc->HResult;
    }

    return hr;
}



//-------------------------------------------------------------------
// Name: ProcessInput
// Description: Process an input sample.
//-------------------------------------------------------------------

HRESULT CDecoder::ProcessInput(
    DWORD               dwInputStreamID,
    IMFSample           *pSample,
    DWORD               dwFlags
)
{
    HRESULT hr = S_OK;
    try
    {
        if (pSample == nullptr)
        {
            throw ref new InvalidArgumentException();
        }

        if (!IsValidInputStream(dwInputStreamID))
        {
            ThrowException(MF_E_INVALIDSTREAMNUMBER);
        }

        if (dwFlags != 0)
        {
            throw ref new InvalidArgumentException(); // dwFlags is reserved and must be zero.
        }

        AutoLock lock(m_critSec);

        LONGLONG rtTimestamp = 0;

        if (m_spInputType == nullptr || m_spOutputType == nullptr)
        {
            ThrowException(MF_E_NOTACCEPTING);   // Client must set input and output types.
        }
        else if (HasPendingOutput())
        {
            ThrowException(MF_E_NOTACCEPTING);   // We already have an input sample.
        }

        // Convert to a single contiguous buffer.
        // NOTE: This does not cause a copy unless there are multiple buffers
        ThrowIfError(pSample->ConvertToContiguousBuffer(m_spBuffer.ReleaseAndGetAddressOf()));


        ThrowIfError(m_spBuffer->Lock(&m_pbData, nullptr, &m_cbData));

        // Get the time stamp. It is OK if this call fails.
        if (FAILED(pSample->GetSampleTime(&rtTimestamp)))
        {
            rtTimestamp = INVALID_TIME;
        }

        m_StreamState.TimeStamp(rtTimestamp);

        Process();
    }
    catch (Exception ^exc)
    {
        hr = exc->HResult;
    }

    return hr;
}



//-------------------------------------------------------------------
// Name: ProcessOutput
// Description: Process an output sample.
//-------------------------------------------------------------------

HRESULT CDecoder::ProcessOutput(
    DWORD                   dwFlags,
    DWORD                   cOutputBufferCount,
    MFT_OUTPUT_DATA_BUFFER  *pOutputSamples, // one per stream
    DWORD                   *pdwStatus
)
{
    HRESULT hr = S_OK;
    try
    {
        // Check input parameters...

        // There are no flags that we accept in this MFT.
        // The only defined flag is MFT_PROCESS_OUTPUT_DISCARD_WHEN_NO_BUFFER. This
        // flag only applies when the MFT marks an output stream as lazy or optional.
        // However there are no lazy or optional streams on this MFT, so the flag is
        // not valid.
        if (dwFlags != 0)
        {
            throw ref new InvalidArgumentException();
        }

        if (pOutputSamples == nullptr || pdwStatus == nullptr)
        {
            throw ref new InvalidArgumentException();
        }

        // Must be exactly one output buffer.
        if (cOutputBufferCount != 1)
        {
            throw ref new InvalidArgumentException();
        }

        // It must contain a sample.
        if (pOutputSamples[0].pSample == nullptr)
        {
            throw ref new InvalidArgumentException();
        }

        AutoLock lock(m_critSec);

        DWORD cbData = 0;

        ComPtr<IMFMediaBuffer> spOutput;

        // If we don't have an input sample, we need some input before
        // we can generate any output.
        if (!HasPendingOutput())
        {
            return MF_E_TRANSFORM_NEED_MORE_INPUT;
        }

        // Get the output buffer.
        ThrowIfError(pOutputSamples[0].pSample->GetBufferByIndex(0, &spOutput));
        ThrowIfError(spOutput->GetMaxLength(&cbData));

        if (cbData < m_cbImageSize)
        {
            throw ref new InvalidArgumentException();
        }

        InternalProcessOutput(pOutputSamples[0].pSample, spOutput.Get());

        //  Update our state
        m_fPicture = false;

        //  Is there any more data to output at this point?
        try
        {
            Process();
            pOutputSamples[0].dwStatus |= MFT_OUTPUT_DATA_BUFFER_INCOMPLETE;
        }
        catch (Exception^)
        {
        }
    }
    catch (Exception ^exc)
    {
        hr = exc->HResult;
    }

    return hr;
}

// Private class methods


void CDecoder::InternalProcessOutput(IMFSample *pSample, IMFMediaBuffer *pOutputBuffer)
{
    DWORD dwTimeCode = 0;
    LONGLONG rt = 0;
    TCHAR szBuffer[20];

    LONG lDefaultStride = lDefaultStride = m_imageWidthInPixels * 4; // Works only for RGB32
   
    VideoBufferLock buffer(pOutputBuffer, MF2DBuffer_LockFlags_Write, m_imageHeightInPixels, lDefaultStride);
    LONG lActualStride = buffer.GetStride();
    BYTE *pbData = buffer.GetTopRow();

    //  Generate our data
    rt = m_StreamState.PictureTime(&dwTimeCode);

    ThrowIfError(StringCchPrintf(szBuffer, ARRAYSIZE(szBuffer),
        TEXT("%2.2d:%2.2d:%2.2d:%2.2d\0"),
            (dwTimeCode >> 19) & 0x1F,
            (dwTimeCode >> 13) & 0x3F,
            (dwTimeCode >> 6) & 0x3F,
            dwTimeCode & 0x3F
            ));

    // Update our bitmap with turquoise
    OurFillRect(pbData, m_imageWidthInPixels, m_imageHeightInPixels, lActualStride, RGB(0xFF,0x80,0x80));
    OurFillRect(pbData, m_imageWidthInPixels/2, m_imageHeightInPixels/2, lActualStride, RGB(0x00,0xff,0x00));

    ThrowIfError(pOutputBuffer->SetCurrentLength(m_cbImageSize));

    // Clean point / key frame
    ThrowIfError(pSample->SetUINT32(MFSampleExtension_CleanPoint, TRUE));

    //  Set the timestamp
    //  Uncompressed video must always have a timestamp

    rt = m_StreamState.PictureTime(&dwTimeCode);
    if (rt == INVALID_TIME)
    {
        rt = m_rtFrame;
    }
    ThrowIfError(pSample->SetSampleTime(rt));

    ThrowIfError(pSample->SetSampleDuration(m_rtLength));

    m_rtFrame = rt + m_rtLength;
}


void CDecoder::OnCheckInputType(IMFMediaType *pmt)
{
    //  Check if the type is already set and if so reject any type that's not identical.
    if (m_spInputType != nullptr)
    {
        DWORD dwFlags = 0;
        if (S_OK == m_spInputType->IsEqual(pmt, &dwFlags))
        {
            return;
        }
        else
        {
            ThrowException(MF_E_INVALIDTYPE);
        }
    }

    GUID majortype = { 0 };
    GUID subtype = { 0 };
    UINT32 width = 0, height = 0;
    MFRatio fps = { 0 };
    UINT32 cbSeqHeader = 0;

    //  We accept MFMediaType_Video, MEDIASUBTYPE_MPEG1Video

    ThrowIfError(pmt->GetMajorType(&majortype));

    if (majortype != MFMediaType_Video)
    {
        ThrowException(MF_E_INVALIDTYPE);
    }

    ThrowIfError(pmt->GetGUID(MF_MT_SUBTYPE, &subtype));

    if (subtype != MFVideoFormat_MPG1)
    {
        ThrowException(MF_E_INVALIDTYPE);
    }

    // At this point a real decoder would validate the MPEG-1 format.

    // Validate the frame size.

    ThrowIfError(MFGetAttributeSize(pmt, MF_MT_FRAME_SIZE, &width, &height));

    if (width > MAX_VIDEO_WIDTH || height > MAX_VIDEO_HEIGHT)
    {
        ThrowException(MF_E_INVALIDTYPE);
    }

    // Make sure there is a frame rate.
    // *** A real decoder would also validate this. ***

    ThrowIfError(MFGetAttributeRatio(pmt, MF_MT_FRAME_RATE, (UINT32*)&fps.Numerator, (UINT32*)&fps.Denominator));

    // Check for a sequence header.
    // We don't actually look at it for this sample.
    // *** A real decoder would validate the sequence header. ***

    (void)pmt->GetBlobSize(MF_MT_MPEG_SEQUENCE_HEADER, &cbSeqHeader);

    if (cbSeqHeader < MPEG1_VIDEO_SEQ_HEADER_MIN_SIZE)
    {
        ThrowException(MF_E_INVALIDTYPE);
    }
}


void CDecoder::OnSetInputType(IMFMediaType *pmt)
{
    m_spInputType.Reset();

    ThrowIfError(MFGetAttributeSize(pmt, MF_MT_FRAME_SIZE, &m_imageWidthInPixels, &m_imageHeightInPixels));

    ThrowIfError(MFGetAttributeRatio(pmt, MF_MT_FRAME_RATE, (UINT32*)&m_frameRate.Numerator, (UINT32*)&m_frameRate.Denominator));

    // Also store the frame duration, derived from the frame rate.
    if (m_frameRate.Numerator == 0 || m_frameRate.Denominator == 0) {
        m_rtLength = 0;
        ThrowException(MF_E_INVALIDTYPE);
    }

    double fRate = (double)m_frameRate.Numerator / m_frameRate.Denominator;

    m_rtLength = (UINT64)((10000000 / fRate) + 0.5);

    m_cbImageSize = m_imageWidthInPixels * m_imageHeightInPixels * 4;

    m_spInputType = pmt;
}

void CDecoder::OnCheckOutputType(IMFMediaType *pmt)
{
    //  Check if the type is already set and if so reject any type that's not identical.
    if (m_spOutputType != nullptr)
    {
        DWORD dwFlags = 0;
        if (S_OK == m_spOutputType->IsEqual(pmt, &dwFlags))
        {
            return;
        }
        else
        {
            ThrowException(MF_E_INVALIDTYPE);
        }
    }

    if (m_spInputType == nullptr)
    {
        ThrowException(MF_E_TRANSFORM_TYPE_NOT_SET); // Input type must be set first.
    }

    BOOL fMatch = FALSE;

    ComPtr<IMFMediaType> spOurType;

    // Make sure their type is a superset of our proposed output type.
    ThrowIfError(GetOutputAvailableType(0, 0, &spOurType));

    ThrowIfError(spOurType->Compare(pmt, MF_ATTRIBUTES_MATCH_OUR_ITEMS, &fMatch));

    if (!fMatch)
    {
        ThrowException(MF_E_INVALIDTYPE);
    }
}


void CDecoder::OnSetOutputType(IMFMediaType *pmt)
{
    m_spOutputType = pmt;
}


void CDecoder::AllocateStreamingResources()
{
    //  Reinitialize variables
    OnDiscontinuity();
}

void CDecoder::FreeStreamingResources()
{

}

void CDecoder::OnDiscontinuity()
{
    //  Zero our timestamp
    m_rtFrame = 0;

    //  No pictures yet
   m_fPicture   = false;

   //  Reset state machine
   m_StreamState.Reset();
}

void CDecoder::OnFlush()
{
    OnDiscontinuity();

    //  Release buffer
    m_spBuffer.Reset();
}


//  Scan input data until either we're exhausted or we find
//  a picture start code
//  Note GOP time codes as we encounter them so we can
//  output time codes

void CDecoder::Process()
{
    //  Process bytes and update our state machine
    while (m_cbData && !m_fPicture)
    {
        m_fPicture = m_StreamState.NextByte(*m_pbData);
        m_cbData--;
        m_pbData++;
    }

    //  Release buffer if we're done with it
    if (m_cbData == 0)
    {
        m_spBuffer->Unlock();

        m_pbData = nullptr;

        m_spBuffer.Reset();
    }

    //  assert that if have no picture to output then we ate all the data
    assert(m_fPicture || m_cbData == 0);
}


//-------------------------------------------------------------------
// CStreamState
//-------------------------------------------------------------------
void CStreamState::TimeStamp(REFERENCE_TIME rt)
{
    DWORD dwIndex = m_cbBytes >= 4 ? 0 : m_cbBytes;

    m_arTS[dwIndex].bValid = true;
    m_arTS[dwIndex].rt = rt;
}

void CStreamState::Reset()
{
    m_cbBytes = 0;
    m_dwNextTimeCode = 0;

    for (int i = 0; i < 4; i++) {
        m_arTS[i].bValid = false;
    }
}

bool CStreamState::NextByte(BYTE bData)
{
    assert(m_arTS[0].bValid);

    switch (m_cbBytes)
    {
        case 0:
            if (bData == 0) {
                m_cbBytes++;
            }
            return false;

        case 1:
            if (bData == 0) {
                m_cbBytes++;
            } else {
                m_cbBytes = 0;

                //  Pick up new timestamp if there was one
                if (m_arTS[1].bValid) {
                    m_arTS[0].rt = m_arTS[1].rt;
                    m_arTS[1].bValid = false;
                }
            }
            return false;

        case 2:
            if (bData == 1) {
                m_cbBytes++;
            } else {
                if (bData == 0) {
                   if (m_arTS[1].bValid) {
                       m_arTS[0].rt = m_arTS[1].rt;
                       m_arTS[1].bValid = false;
                   }
                   if (m_arTS[2].bValid) {
                       m_arTS[1] = m_arTS[2];
                       m_arTS[2].bValid = false;
                   }
                } else {
                    //  Not 0 or 1, revert
                    m_cbBytes = 0;

                    //  and pick up latest timestamp
                    if (m_arTS[1].bValid) {
                        m_arTS[0].rt = m_arTS[1].rt;
                        m_arTS[1].bValid = false;
                    }
                    if (m_arTS[2].bValid) {
                        m_arTS[0].rt = m_arTS[2].rt;
                        m_arTS[2].bValid = false;
                    }
                }
            }
            return false;

        case 3:
        {
            //  It's a start code
            //  Return the timestamp and reset everything
            m_rt = m_arTS[0].rt;

            //  If it's a picture start code can't use this timestamp again.
            if (0 == bData) {
                m_arTS[0].rt = INVALID_TIME;
                m_cbBytes = 0;
            }

            //  Catch up and clean out timestamps
            for (int i = 1; i < 4; i++) {
                if (m_arTS[i].bValid) {
                    m_arTS[0].rt = m_arTS[i].rt;
                    m_arTS[i].bValid = false;
                }
            }

            // Picture start code?
            if (0 == bData) {
                m_cbBytes = 0;
                m_dwTimeCode = m_dwNextTimeCode;
                m_dwNextTimeCode++;
                return true;
            } else {
                //  Is it a GOP start code?
                if (bData == 0xb8) {
                    m_cbBytes++;
                } else {
                    m_cbBytes = 0;
                }
                return false;
            }
        }

        case 4:
        case 5:
        case 6:
        case 7:
            m_bGOPData[m_cbBytes - 4] = bData;
            m_cbBytes++;

            if (m_cbBytes == 8) {
                m_cbBytes = 0;
                m_dwNextTimeCode = ((DWORD)m_bGOPData[0] << 17) +
                                   ((DWORD)m_bGOPData[1] << 9) +
                                   ((DWORD)m_bGOPData[2] << 1) +
                                   ((DWORD)m_bGOPData[3] >> 7);
            }
            return false;
    }

    // Should never reach here
    return false;
};



//-------------------------------------------------------------------
// Static functions
//-------------------------------------------------------------------


//-------------------------------------------------------------------
// OurFillRect
//
// Fills a bitmap with a specified color.
//
// pvih: Pointer to a VIDEOINFOHEADER structure that describes the
//       bitmap.
// pbData: Pointer to the start of the bitmap.
// color: Fill color.
//-------------------------------------------------------------------

void OurFillRect(PBYTE pbScanline0, DWORD dwWidth, DWORD dwHeight, LONG lStrideInBytes, COLORREF color)
{
    PBYTE pbPixels = pbScanline0;

    // For just filling we don't care which way up the bitmap is -
    // we just start at pbData
    for(DWORD dwCount = 0; dwCount < dwHeight; dwCount++) {
        DWORD *pWord = (DWORD *)pbPixels;

        for(DWORD dwPixel = 0; dwPixel < dwWidth; dwPixel++) {
            pWord[dwPixel] = (DWORD)color;
        }

        //  biWidth is the stride
        pbPixels += lStrideInBytes;
    }
}
