//////////////////////////////////////////////////////////////////////////
//
// decoder.h
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

#pragma once
#include <CritSec.h>

const DWORD MPEG1_VIDEO_SEQ_HEADER_MIN_SIZE = 12;       // Minimum length of the video sequence header.
static const REFERENCE_TIME INVALID_TIME = _I64_MAX;    //  Not really invalid but unlikely enough for sample code.

//-------------------------------------------------------------------
// CStreamState class.
// Provides state machine for picture start codes and timestamps.
//
//  We need to know:
//
//    - The time stamp to associate with a picture start code
//    - The next time stamp
//
//-------------------------------------------------------------------

class CStreamState sealed
{
private:

    DWORD m_cbBytes;    // Number of valid bytes of start code so far

    struct
    {
        bool           bValid;
        REFERENCE_TIME rt;
    } m_arTS[4];        // Array of all the timestamps.
                        // m_cbBytes+1 entries are valid.

    REFERENCE_TIME m_rt;

    BYTE m_bGOPData[4];

    DWORD m_dwTimeCode;
    DWORD m_dwNextTimeCode;

public:
    CStreamState()
    {
        Reset();
    }

    //  Returns true if a start code was identifed
    bool NextByte(BYTE bData);
    void TimeStamp(REFERENCE_TIME rt);
    void Reset();

    REFERENCE_TIME PictureTime(DWORD *pdwTimeCode) const
    {
        *pdwTimeCode = m_dwTimeCode;
        return m_rt;
    }
};



//-------------------------------------------------------------------
// CDecoder
//
// Implements the MPEG-1 "decoder" MFT.
//
// The decoder outputs RGB-32 only.
//
// Note: This MFT is derived from a sample that used to ship in the
// DirectX SDK.
//-------------------------------------------------------------------

class CDecoder WrlSealed
    : public Microsoft::WRL::RuntimeClass<
           Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >, 
           ABI::Windows::Media::IMediaExtension,
           IMFTransform>
{
    InspectableClass(L"MPEG1Decoder.MPEG1Decoder", BaseTrust)

public:
    CDecoder();
    ~CDecoder();

    // IMediaExtension
    IFACEMETHOD (SetProperties) (ABI::Windows::Foundation::Collections::IPropertySet *pConfiguration);

    // IMFTransform methods
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

    STDMETHODIMP GetStreamIDs(
        DWORD   dwInputIDArraySize,
        DWORD   *pdwInputIDs,
        DWORD   dwOutputIDArraySize,
        DWORD   *pdwOutputIDs
    );

    STDMETHODIMP GetInputStreamInfo(
        DWORD                     dwInputStreamID,
        MFT_INPUT_STREAM_INFO *   pStreamInfo
    );

    STDMETHODIMP GetOutputStreamInfo(
        DWORD                     dwOutputStreamID,
        MFT_OUTPUT_STREAM_INFO *  pStreamInfo
    );

    STDMETHODIMP GetAttributes(IMFAttributes **pAttributes);

    STDMETHODIMP GetInputStreamAttributes(
        DWORD           dwInputStreamID,
        IMFAttributes   **ppAttributes
    );

    STDMETHODIMP GetOutputStreamAttributes(
        DWORD           dwOutputStreamID,
        IMFAttributes   **ppAttributes
    );

    STDMETHODIMP DeleteInputStream(DWORD dwStreamID);

    STDMETHODIMP AddInputStreams(
        DWORD   cStreams,
        DWORD   *adwStreamIDs
    );

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

    STDMETHODIMP SetOutputBounds(
        LONGLONG        hnsLowerBound,
        LONGLONG        hnsUpperBound
    );

    STDMETHODIMP ProcessEvent(
        DWORD              dwInputStreamID,
        IMFMediaEvent      *pEvent
    );

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

protected:

    // HasPendingOutput: Returns TRUE if the MFT is holding an input sample.
    bool HasPendingOutput() const { return m_spBuffer != nullptr; }

    // IsValidInputStream: Returns TRUE if dwInputStreamID is a valid input stream identifier.
    static bool IsValidInputStream(DWORD dwInputStreamID)
    {
        return dwInputStreamID == 0;
    }

    // IsValidOutputStream: Returns TRUE if dwOutputStreamID is a valid output stream identifier.
    static bool IsValidOutputStream(DWORD dwOutputStreamID)
    {
        return dwOutputStreamID == 0;
    }

    //  Internal processing routine
    void InternalProcessOutput(IMFSample *pSample, IMFMediaBuffer *pOutputBuffer);
    void Process();
    void OnCheckInputType(IMFMediaType *pmt);
    void OnCheckOutputType(IMFMediaType *pmt);
    void OnSetInputType(IMFMediaType *pmt);
    void OnSetOutputType(IMFMediaType *pmt);
    void OnDiscontinuity();
    void AllocateStreamingResources();
    void FreeStreamingResources();
    void OnFlush();


protected:

    CritSec m_critSec;

    //  Streaming locals
    ComPtr<IMFMediaType> m_spInputType;     // Input media type.
    ComPtr<IMFMediaType> m_spOutputType;    // Output media type.

    ComPtr<IMFMediaBuffer> m_spBuffer;
    BYTE *m_pbData;
    DWORD m_cbData;

    // Fomat information
    UINT32 m_imageWidthInPixels;
    UINT32 m_imageHeightInPixels;
    MFRatio m_frameRate;
    DWORD m_cbImageSize;                    // Image size, in bytes.

    //  Fabricate timestamps based on the average time per from if there isn't one in the stream
    REFERENCE_TIME m_rtFrame;
    UINT64 m_rtLength;

    //  Current state info
    CStreamState m_StreamState;
    bool m_fPicture;
    bool m_fLowLatencyMode;
};
