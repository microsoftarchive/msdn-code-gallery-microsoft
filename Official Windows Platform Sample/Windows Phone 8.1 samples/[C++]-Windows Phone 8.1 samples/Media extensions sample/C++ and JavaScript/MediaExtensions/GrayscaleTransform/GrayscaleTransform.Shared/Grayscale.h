// Defines the transform class.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once
#include "CritSec.h"

// Function pointer for the function that transforms the image.
typedef void (*IMAGE_TRANSFORM_FN)(
    const D2D_RECT_U&       rcDest,          // Destination rectangle for the transformation.
    BYTE*                   pDest,           // Destination buffer.
    LONG                    lDestStride,     // Destination stride.
    const BYTE*             pSrc,            // Source buffer.
    LONG                    lSrcStride,      // Source stride.
    DWORD                   dwWidthInPixels, // Image width in pixels.
    DWORD                   dwHeightInPixels // Image height in pixels.
    );

// CGrayscale class:
// Implements a grayscale video effect.

class CGrayscale WrlSealed
    : public Microsoft::WRL::RuntimeClass<
           Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >, 
           ABI::Windows::Media::IMediaExtension,
           IMFTransform >
{
    InspectableClass(L"GrayscaleTransform.GrayscaleEffect", BaseTrust)

public:
    CGrayscale();

    ~CGrayscale();

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


private:
    // HasPendingOutput: Returns TRUE if the MFT is holding an input sample.
    bool HasPendingOutput() const { return m_spSample != nullptr; }

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

    ComPtr<IMFMediaType> OnGetPartialType(DWORD dwTypeIndex);
    void OnCheckInputType(IMFMediaType *pmt);
    void OnCheckOutputType(IMFMediaType *pmt);
    void OnCheckMediaType(IMFMediaType *pmt);
    void OnSetInputType(IMFMediaType *pmt);
    void OnSetOutputType(IMFMediaType *pmt);
    void BeginStreaming();
    void EndStreaming();
    void OnProcessOutput(IMFMediaBuffer *pIn, IMFMediaBuffer *pOut);
    void OnFlush();
    void UpdateFormatInfo();

    CritSec m_critSec;

    // Transformation parameters
    D2D_RECT_U m_rcDest;                    // Destination rectangle for the effect.

    // Streaming
    bool m_fStreamingInitialized;
    ComPtr<IMFSample> m_spSample;           // Input sample.
    ComPtr<IMFMediaType> m_spInputType;     // Input media type.
    ComPtr<IMFMediaType> m_spOutputType;    // Output media type.

    // Fomat information
    UINT32 m_imageWidthInPixels;
    UINT32 m_imageHeightInPixels;
    DWORD m_cbImageSize;                    // Image size, in bytes.

    ComPtr<IMFAttributes> m_spAttributes;

    // Image transform function. (Changes based on the media type.)
    IMAGE_TRANSFORM_FN  m_pTransformFn;
};