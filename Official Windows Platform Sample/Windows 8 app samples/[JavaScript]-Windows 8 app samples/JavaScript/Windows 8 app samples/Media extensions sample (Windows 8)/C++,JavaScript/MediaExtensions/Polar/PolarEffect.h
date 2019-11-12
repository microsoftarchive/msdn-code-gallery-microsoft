// Defines the transform class.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

#ifndef POLAREFFECT_H
#define POLAREFFECT_H

#include <new>
#include <mfapi.h>
#include <mftransform.h>
#include <mfidl.h>
#include <mferror.h>
#include <strsafe.h>
#include <assert.h>
#include <math.h>

// Note: The Direct2D helper library is included for its 2D matrix operations.
#include <D2d1helper.h>

#include <wrl\implements.h>
#include <wrl\module.h>
#include <windows.media.h>
#include <wrl\wrappers\corewrappers.h>

#include "PolarTransform.h"

using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Wrappers;

void DllAddRef();
void DllRelease();

template <class T> void SafeRelease(T **ppT)
{
    if (*ppT)
    {
        (*ppT)->Release();
        *ppT = NULL;
    }
}

template <class T> void SafeArrayDelete(T **ppT)
{
    if (*ppT)
    {
        delete [] (*ppT);
        *ppT = NULL;
    }
}


// Function pointer for the function that transforms the image.
typedef void (*IMAGE_TRANSFORM_FN)(
    const UINT32*           pLookup,         // Lookup buffer.
    BYTE*                   pDest,           // Destination buffer.
    LONG                    lDestStride,     // Destination stride.
    const BYTE*             pSrc,            // Source buffer.
    LONG                    lSrcStride,      // Source stride.
    DWORD                   dwWidthInPixels, // Image width in pixels.
    DWORD                   dwHeightInPixels // Image height in pixels.
    );

typedef DOUBLE (*PolarTransformer)(DOUBLE, DOUBLE, DOUBLE);


// CPolarEffect class:
// Implements a polar transformation video effect.

class CPolarEffect
    : public Microsoft::WRL::RuntimeClass<
           Microsoft::WRL::RuntimeClassFlags< Microsoft::WRL::RuntimeClassType::WinRtClassicComMix >, 
           ABI::Windows::Media::IMediaExtension,
           IMFTransform >
{
    InspectableClass(RuntimeClass_PolarTransform_PolarEffect,BaseTrust)

public:
    CPolarEffect();

    ~CPolarEffect();

    __forceinline static DOUBLE DefaultRadius(DOUBLE r, DOUBLE R, DOUBLE theta) { return r; }
    __forceinline static DOUBLE DefaultTheta(DOUBLE r, DOUBLE R, DOUBLE theta) { return theta; };

    __forceinline static DOUBLE PinchRadius(DOUBLE r, DOUBLE R, DOUBLE theta) { return sqrt(r*R); };
    __forceinline static DOUBLE FisheyeRadius(DOUBLE r, DOUBLE R, DOUBLE theta) { return (r*r)/R; };
    __forceinline static DOUBLE WarpTheta(DOUBLE r, DOUBLE R, DOUBLE theta) { return theta + r / R; };

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

    STDMETHODIMP GetAttributes(IMFAttributes** pAttributes);

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
    BOOL HasPendingOutput() const { return m_pSample != NULL; }

    // IsValidInputStream: Returns TRUE if dwInputStreamID is a valid input stream identifier.
    BOOL IsValidInputStream(DWORD dwInputStreamID) const
    {
        return dwInputStreamID == 0;
    }

    // IsValidOutputStream: Returns TRUE if dwOutputStreamID is a valid output stream identifier.
    BOOL IsValidOutputStream(DWORD dwOutputStreamID) const
    {
        return dwOutputStreamID == 0;
    }

    HRESULT OnGetPartialType(DWORD dwTypeIndex, IMFMediaType **ppmt);
    HRESULT OnCheckInputType(IMFMediaType *pmt);
    HRESULT OnCheckOutputType(IMFMediaType *pmt);
    HRESULT OnCheckMediaType(IMFMediaType *pmt);
    void    OnSetInputType(IMFMediaType *pmt);
    void    OnSetOutputType(IMFMediaType *pmt);
    HRESULT BeginStreaming();
    HRESULT EndStreaming();
    HRESULT OnProcessOutput(IMFMediaBuffer *pIn, IMFMediaBuffer *pOut);
    HRESULT OnFlush();
    HRESULT UpdateFormatInfo();
    void GeneratePolarLookup( UINT32 unWidth, 
                                   UINT32 unHeight, 
                                   UINT32 *pIndexLookup );


    CRITICAL_SECTION            m_critSec;

    // Streaming
    bool                        m_bStreamingInitialized;
    IMFSample                   *m_pSample;                 // Input sample.
    IMFMediaType                *m_pInputType;              // Input media type.
    IMFMediaType                *m_pOutputType;             // Output media type.

    // Fomat information
    UINT32                      m_imageWidthInPixels;
    UINT32                      m_imageHeightInPixels;
    DWORD                       m_cbImageSize;              // Image size, in bytes.

    IMFAttributes               *m_pAttributes;

    // Image transform function. (Changes based on the media type.)
    IMAGE_TRANSFORM_FN          m_pTransformFn;

    // Lookup table to store the result of polar transformation
    UINT32                      *m_pIndexLookup;

    // Polar transform function for specific effect
    PolarTransformer            m_pRadiusTransformFn;       // Radius transformation
    PolarTransformer            m_pThetaTransformFn;        // Angle transformation
};

#endif
