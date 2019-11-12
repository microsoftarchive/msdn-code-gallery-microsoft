//////////////////////////////////////////////////////////////////////////
//
// CMPEG1Source.h
// Implements the MPEG-1 media source object.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//////////////////////////////////////////////////////////////////////////


#include "MPEG1Src.h"

//-------------------------------------------------------------------
//
// Notes:
// This sample contains an MPEG-1 source.
//
// - The source parses MPEG-1 systems-layer streams and generates
//   samples that contain MPEG-1 payloads.
// - The source does not support files that contain a raw MPEG-1
//   video or audio stream.
// - The source does not support seeking.
//
//-------------------------------------------------------------------

#pragma warning( push )
#pragma warning( disable : 4355 )  // 'this' used in base member initializer list

HRESULT CreateVideoMediaType(const MPEG1VideoSeqHeader& videoSeqHdr, IMFMediaType **ppType);
HRESULT CreateAudioMediaType(const MPEG1AudioFrameHeader& audioHeader, IMFMediaType **ppType);
HRESULT GetStreamMajorType(IMFStreamDescriptor *pSD, GUID *pguidMajorType);
BOOL    SampleRequestMatch(SourceOp *pOp1, SourceOp *pOp2);


/* Public class methods */

//-------------------------------------------------------------------
// Name: CreateInstance
// Static method to create an instance of the source.
//
// ppSource:    Receives a ref-counted pointer to the source.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::CreateInstance(CMPEG1Source **ppSource)
{
    if (ppSource == NULL)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;
    CMPEG1Source *pSource = new (std::nothrow) CMPEG1Source(hr);
    if (pSource == NULL)
    {
        return E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        *ppSource = pSource;
        (*ppSource)->AddRef();
    }

    SafeRelease(&pSource);
    return hr;
}


//-------------------------------------------------------------------
// IUnknown methods
//-------------------------------------------------------------------

HRESULT CMPEG1Source::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }

    HRESULT hr = E_NOINTERFACE;
    if (riid == IID_IUnknown ||
        riid == IID_IMFMediaEventGenerator ||
        riid == IID_IMFMediaSource)
    {
        (*ppv) = static_cast<IMFMediaSource *>(this);
        AddRef();
        hr = S_OK;
    }
    else if (riid == IID_IMFGetService)
    {
        (*ppv) = static_cast<IMFGetService*>(this);
        AddRef();
        hr = S_OK;
    }
    else if (riid == IID_IMFRateControl)
    {
        (*ppv) = static_cast<IMFRateControl*>(this);
        AddRef();
        hr = S_OK;
    }

    return hr;
}

ULONG CMPEG1Source::AddRef()
{
    return _InterlockedIncrement(&m_cRef);
}

ULONG CMPEG1Source::Release()
{
    LONG cRef = _InterlockedDecrement(&m_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

//-------------------------------------------------------------------
// IMFMediaEventGenerator methods
//
// All of the IMFMediaEventGenerator methods do the following:
// 1. Check for shutdown status.
// 2. Call the event queue helper object.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::BeginGetEvent(IMFAsyncCallback* pCallback,IUnknown* punkState)
{
    HRESULT hr = S_OK;

    EnterCriticalSection(&m_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = m_pEventQueue->BeginGetEvent(pCallback, punkState);
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}

HRESULT CMPEG1Source::EndGetEvent(IMFAsyncResult* pResult, IMFMediaEvent** ppEvent)
{
    HRESULT hr = S_OK;

    EnterCriticalSection(&m_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = m_pEventQueue->EndGetEvent(pResult, ppEvent);
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}

HRESULT CMPEG1Source::GetEvent(DWORD dwFlags, IMFMediaEvent** ppEvent)
{
    // NOTE:
    // GetEvent can block indefinitely, so we don't hold the critical
    // section. Therefore we need to use a local copy of the event queue
    // pointer, to make sure the pointer remains valid.

    HRESULT hr = S_OK;

    IMFMediaEventQueue *pQueue = NULL;

    EnterCriticalSection(&m_critSec);

    // Check shutdown
    hr = CheckShutdown();

    // Cache a local pointer to the queue.
    if (SUCCEEDED(hr))
    {
        pQueue = m_pEventQueue;
        pQueue->AddRef();
    }

    LeaveCriticalSection(&m_critSec);

    // Use the local pointer to call GetEvent.
    if (SUCCEEDED(hr))
    {
        hr = pQueue->GetEvent(dwFlags, ppEvent);
    }

    SafeRelease(&pQueue);
    return hr;
}

HRESULT CMPEG1Source::QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT* pvValue)
{
    HRESULT hr = S_OK;

    EnterCriticalSection(&m_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = m_pEventQueue->QueueEventParamVar(met, guidExtendedType, hrStatus, pvValue);
    }

    LeaveCriticalSection(&m_critSec);

    return hr;
}

//-------------------------------------------------------------------
// IMFMediaSource methods
//-------------------------------------------------------------------


//-------------------------------------------------------------------
// CreatePresentationDescriptor
// Returns a shallow copy of the source's presentation descriptor.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::CreatePresentationDescriptor(
    IMFPresentationDescriptor** ppPresentationDescriptor
    )
{
    if (ppPresentationDescriptor == NULL)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;

    EnterCriticalSection(&m_critSec);

    // Fail if the source is shut down.
    hr = CheckShutdown();

    // Fail if the source was not initialized yet.
    if (SUCCEEDED(hr))
    {
        hr = IsInitialized();
    }

    // Do we have a valid presentation descriptor?
    if (SUCCEEDED(hr))
    {
        if (m_pPresentationDescriptor == NULL)
        {
            hr = MF_E_NOT_INITIALIZED;
        }
    }

    // Clone our presentation descriptor.
    if (SUCCEEDED(hr))
    {
        hr = m_pPresentationDescriptor->Clone(ppPresentationDescriptor);
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// GetCharacteristics
// Returns capabilities flags.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::GetCharacteristics(DWORD* pdwCharacteristics)
{
    if (pdwCharacteristics == NULL)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;

    EnterCriticalSection(&m_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        *pdwCharacteristics =  MFMEDIASOURCE_CAN_PAUSE;
    }

    // NOTE: This sample does not implement seeking, so we do not
    // include the MFMEDIASOURCE_CAN_SEEK flag.

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// Pause
// Pauses the source.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::Pause()
{
    EnterCriticalSection(&m_critSec);

    HRESULT hr = S_OK;

    // Fail if the source is shut down.
    hr = CheckShutdown();

    // Queue the operation.
    if (SUCCEEDED(hr))
    {
        hr = QueueAsyncOperation(SourceOp::OP_PAUSE);
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}

//-------------------------------------------------------------------
// Shutdown
// Shuts down the source and releases all resources.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::Shutdown()
{
    EnterCriticalSection(&m_critSec);

    HRESULT hr = S_OK;

    CMPEG1Stream *pStream = NULL;

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        // Shut down the stream objects.

        for (DWORD i = 0; i < m_streams.GetCount(); i++)
        {
            (void)m_streams[i]->Shutdown();
        }

        // Shut down the event queue.
        if (m_pEventQueue)
        {
            (void)m_pEventQueue->Shutdown();
        }

        // Release objects.

        SafeRelease(&m_pEventQueue);
        SafeRelease(&m_pPresentationDescriptor);
        SafeRelease(&m_pBeginOpenResult);
        SafeRelease(&m_pByteStream);
        SafeRelease(&m_pCurrentOp);

        CoTaskMemFree(m_pHeader);
        m_pHeader = NULL;

        delete m_pParser;
        m_pParser = NULL;

        // Set the state.
        m_state = STATE_SHUTDOWN;
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// Start
// Starts or seeks the media source.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::Start(
        IMFPresentationDescriptor* pPresentationDescriptor,
        const GUID* pguidTimeFormat,
        const PROPVARIANT* pvarStartPos
    )
{

    HRESULT hr = S_OK;
    SourceOp *pAsyncOp = NULL;

    // Check parameters.

    // Start position and presentation descriptor cannot be NULL.
    if (pvarStartPos == NULL || pPresentationDescriptor == NULL)
    {
        return E_INVALIDARG;
    }

    // Check the time format.
    if ((pguidTimeFormat != NULL) && (*pguidTimeFormat != GUID_NULL))
    {
        // Unrecognized time format GUID.
        return MF_E_UNSUPPORTED_TIME_FORMAT;
    }

    // Check the data type of the start position.
    if ((pvarStartPos->vt != VT_I8) && (pvarStartPos->vt != VT_EMPTY))
    {
        return MF_E_UNSUPPORTED_TIME_FORMAT;
    }

    EnterCriticalSection(&m_critSec);

    // Check if this is a seek request. This sample does not support seeking.

    if (pvarStartPos->vt == VT_I8)
    {
        // If the current state is STOPPED, then position 0 is valid.
        // Otherwise, the start position must be VT_EMPTY (current position).

        if ((m_state != STATE_STOPPED) || (pvarStartPos->hVal.QuadPart != 0))
        {
            hr = MF_E_INVALIDREQUEST;
            goto done;
        }
    }

    // Fail if the source is shut down.
    hr = CheckShutdown();
    if (FAILED(hr))
    {
        goto done;
    }

    // Fail if the source was not initialized yet.
    hr = IsInitialized();
    if (FAILED(hr))
    {
        goto done;
    }

    // Perform a sanity check on the caller's presentation descriptor.
    hr = ValidatePresentationDescriptor(pPresentationDescriptor);
    if (FAILED(hr))
    {
        goto done;
    }

    // The operation looks OK. Complete the operation asynchronously.

    hr = SourceOp::CreateStartOp(pPresentationDescriptor, &pAsyncOp);
    if (FAILED(hr))
    {
        goto done;
    }

    hr = pAsyncOp->SetData(*pvarStartPos);
    if (FAILED(hr))
    {
        goto done;
    }

    hr = QueueOperation(pAsyncOp);

done:
    SafeRelease(&pAsyncOp);
    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// Stop
// Stops the media source.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::Stop()
{
    EnterCriticalSection(&m_critSec);

    HRESULT hr = S_OK;

    // Fail if the source is shut down.
    hr = CheckShutdown();

    // Fail if the source was not initialized yet.
    if (SUCCEEDED(hr))
    {
        hr = IsInitialized();
    }

    // Queue the operation.
    if (SUCCEEDED(hr))
    {
        hr = QueueAsyncOperation(SourceOp::OP_STOP);
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}

//-------------------------------------------------------------------
// IMFMediaSource methods
//-------------------------------------------------------------------

//-------------------------------------------------------------------
// GetService
// Returns a service
//-------------------------------------------------------------------

HRESULT CMPEG1Source::GetService( _In_ REFGUID guidService, _In_ REFIID riid, _Out_opt_ LPVOID *ppvObject)
{
    HRESULT hr = MF_E_UNSUPPORTED_SERVICE;

    if (ppvObject == nullptr)
    {
        return E_POINTER;
    }

    if (guidService == MF_RATE_CONTROL_SERVICE)
    {
        hr = QueryInterface(riid, ppvObject);
    }

    return hr;
}

//-------------------------------------------------------------------
// IMFRateControl methods
//-------------------------------------------------------------------

//-------------------------------------------------------------------
// SetRate
// Sets a rate on the source. Only supported rates are 0 and 1.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::SetRate(BOOL fThin, float flRate)
{
    if (fThin)
    {
        return MF_E_THINNING_UNSUPPORTED;
    }
    if (!IsRateSupported(flRate, &flRate))
    {
        return MF_E_UNSUPPORTED_RATE;
    }

    EnterCriticalSection(&m_critSec);
    HRESULT hr = S_OK;
    SourceOp *pAsyncOp = nullptr;

    if (flRate == m_flRate)
    {
        goto done;
    }

    hr = SourceOp::CreateSetRateOp(fThin, flRate, &pAsyncOp);

    if (SUCCEEDED(hr))
    {
        // Queue asynchronous stop
        hr = QueueOperation(pAsyncOp);
    }

done:
    SafeRelease(&pAsyncOp);
    LeaveCriticalSection(&m_critSec);
    return hr;
}

//-------------------------------------------------------------------
// GetRate
// Returns a current rate.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::GetRate(_Inout_opt_ BOOL *pfThin, _Inout_opt_ float *pflRate)
{
    if (pfThin == nullptr || pflRate == nullptr)
    {
        return E_INVALIDARG;
    }

    EnterCriticalSection(&m_critSec);
    *pfThin = FALSE;
    *pflRate = m_flRate;
    LeaveCriticalSection(&m_critSec);

    return S_OK;
}


//-------------------------------------------------------------------
// Public non-interface methods
//-------------------------------------------------------------------

//-------------------------------------------------------------------
// BeginOpen
// Begins reading the byte stream to initialize the source.
// Called by the byte-stream handler when it creates the source.
//
// This method is asynchronous. When the operation completes,
// the callback is invoked and the byte-stream handler calls
// EndOpen.
//
// pStream: Pointer to the byte stream for the MPEG-1 stream.
// pCB: Pointer to the byte-stream handler's callback.
// pState: State object for the async callback. (Can be NULL.)
//
// Note: The source reads enough data to find one packet header
// for each audio or video stream. This enables the source to
// create a presentation descriptor that describes the format of
// each stream. The source queues the packets that it reads during
// BeginOpen.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::BeginOpen(IMFByteStream *pStream, IMFAsyncCallback *pCB, IUnknown *pState)
{
    if (pStream == NULL || pCB == NULL)
    {
        return E_POINTER;
    }

    if (m_state != STATE_INVALID)
    {
        return MF_E_INVALIDREQUEST;
    }

    HRESULT hr = S_OK;
    DWORD dwCaps = 0;

    EnterCriticalSection(&m_critSec);

    // Cache the byte-stream pointer.
    m_pByteStream = pStream;
    m_pByteStream->AddRef();

    // Validate the capabilities of the byte stream.
    // The byte stream must be readable and seekable.
    hr = pStream->GetCapabilities(&dwCaps);

    if (SUCCEEDED(hr))
    {
        if ((dwCaps & MFBYTESTREAM_IS_SEEKABLE) == 0)
        {
            hr = MF_E_BYTESTREAM_NOT_SEEKABLE;
        }
        else if ((dwCaps & MFBYTESTREAM_IS_READABLE) == 0)
        {
            hr = E_FAIL;
        }
    }

    // Reserve space in the read buffer.
    if (SUCCEEDED(hr))
    {
        hr = m_ReadBuffer.Initalize(INITIAL_BUFFER_SIZE);
    }

    // Create the MPEG-1 parser.
    if (SUCCEEDED(hr))
    {
        m_pParser = new (std::nothrow) Parser();
        if (m_pParser == NULL)
        {
            hr = E_OUTOFMEMORY;
        }
    }

    // Create an async result object. We'll use it later to invoke the callback.
    if (SUCCEEDED(hr))
    {
        hr = MFCreateAsyncResult(NULL, pCB, pState, &m_pBeginOpenResult);
    }

    // Start reading data from the stream.
    if (SUCCEEDED(hr))
    {
        hr = RequestData(READ_SIZE);
    }

    // At this point, we now guarantee to invoke the callback.
    if (SUCCEEDED(hr))
    {
        m_state = STATE_OPENING;
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// EndOpen
// Completes the BeginOpen operation.
// Called by the byte-stream handler when it creates the source.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::EndOpen(IMFAsyncResult *pResult)
{
    EnterCriticalSection(&m_critSec);

    HRESULT hr = S_OK;

    hr = pResult->GetStatus();

    if (FAILED(hr))
    {
        // The source is not designed to recover after failing to open.
        // Switch to shut-down state.
        Shutdown();
    }
    LeaveCriticalSection(&m_critSec);
    return hr;
}



//-------------------------------------------------------------------
// OnByteStreamRead
// Called when an asynchronous read completes.
//
// Read requests are issued in the RequestData() method.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::OnByteStreamRead(IMFAsyncResult *pResult)
{
    EnterCriticalSection(&m_critSec);

    HRESULT hr = S_OK;
    DWORD cbRead = 0;

    IUnknown *pState = NULL;

    if (m_state == STATE_SHUTDOWN)
    {
        // If we are shut down, then we've already released the
        // byte stream. Nothing to do.
        LeaveCriticalSection(&m_critSec);
        return S_OK;
    }

    // Get the state object. This is either NULL or the most
    // recent OP_REQUEST_DATA operation.
    (void)pResult->GetState(&pState);

    // Complete the read opertation.
    hr = m_pByteStream->EndRead(pResult, &cbRead);

    // If the source stops and restarts in rapid succession, there is
    // a chance this is a "stale" read request, initiated before the
    // stop/restart.

    // To ensure that we don't deliver stale data, we store the
    // OP_REQUEST_DATA operation as a state object in pResult, and compare
    // this against the current value of m_cRestartCounter.

    // If they don't match, we discard the data.

    // NOTE: During BeginOpen, pState is NULL

    if (SUCCEEDED(hr))
    {
        if ((pState == NULL) || ( ((SourceOp*)pState)->Data().ulVal == m_cRestartCounter) )
        {
            // This data is OK to parse.

            if (cbRead == 0)
            {
                // There is no more data in the stream. Signal end-of-stream.
                hr = EndOfMPEGStream();
            }
            else
            {
                // Update the end-position of the read buffer.
                hr = m_ReadBuffer.MoveEnd(cbRead);

                // Parse the new data.
                if (SUCCEEDED(hr))
                {
                    hr = ParseData();
                }
            }
        }
    }

    if (FAILED(hr))
    {
        StreamingError(hr);
    }
    SafeRelease(&pState);
    LeaveCriticalSection(&m_critSec);
    return hr;
}



/* Private methods */

CMPEG1Source::CMPEG1Source(HRESULT& hr) :
    OpQueue(m_critSec),
    m_cRef(1),
    m_pEventQueue(NULL),
    m_pPresentationDescriptor(NULL),
    m_pBeginOpenResult(NULL),
    m_pParser(NULL),
    m_pByteStream(NULL),
    m_pHeader(NULL),
    m_state(STATE_INVALID),
    m_pCurrentOp(NULL),
    m_pSampleRequest(NULL),
    m_cRestartCounter(0),
    m_OnByteStreamRead(this, &CMPEG1Source::OnByteStreamRead),
    m_flRate(1.0f)
{
    auto module = ::Microsoft::WRL::GetModuleBase();
    if (module != nullptr)
    {
        module->IncrementObjectCount();
    }

	InitializeCriticalSectionEx(&m_critSec, 3000, 0);

    // Create the media event queue.
    hr = MFCreateEventQueue(&m_pEventQueue);
}

CMPEG1Source::~CMPEG1Source()
{
    if (m_state != STATE_SHUTDOWN)
    {
        Shutdown();
    }

    DeleteCriticalSection(&m_critSec);

	auto module = ::Microsoft::WRL::GetModuleBase();
    if (module != nullptr)
    {
        module->DecrementObjectCount();
    }
}


//-------------------------------------------------------------------
// CompleteOpen
//
// Completes the asynchronous BeginOpen operation.
//
// hrStatus: Status of the BeginOpen operation.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::CompleteOpen(HRESULT hrStatus)
{
    HRESULT hr = S_OK;

    if (m_pBeginOpenResult)
    {
        hr = m_pBeginOpenResult->SetStatus(hrStatus);
        if (SUCCEEDED(hr))
        {
            hr = MFInvokeCallback(m_pBeginOpenResult);
        }
    }

    SafeRelease(&m_pBeginOpenResult);
    return hr;
}


//-------------------------------------------------------------------
// IsInitialized:
// Returns S_OK if the source is correctly initialized with an
// MPEG-1 byte stream. Otherwise, returns MF_E_NOT_INITIALIZED.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::IsInitialized() const
{
    if (m_state == STATE_OPENING || m_state == STATE_INVALID)
    {
        return MF_E_NOT_INITIALIZED;
    }
    else
    {
        return S_OK;
    }
}


//-------------------------------------------------------------------
// IsStreamTypeSupported:
// Returns TRUE if the source supports the specified MPEG-1 stream
// type.
//-------------------------------------------------------------------

BOOL CMPEG1Source::IsStreamTypeSupported(StreamType type) const
{
    // We support audio and video streams.
    return (type == StreamType_Video || type == StreamType_Audio);
}

//-------------------------------------------------------------------
// IsStreamActive:
// Returns TRUE if the source should deliver a payload, whose type
// is indicated by the specified packet header.
//
// Note: This method does not test the started/paused/stopped state
//       of the source.
//-------------------------------------------------------------------

BOOL CMPEG1Source::IsStreamActive(const MPEG1PacketHeader& packetHdr)
{
    if (m_state == STATE_OPENING)
    {
        // The source is still opening.
        // Deliver payloads for every supported stream type.
        return IsStreamTypeSupported(packetHdr.type);
    }
    else
    {
        // The source is already opened. Check if the stream is active.
        CMPEG1Stream *pStream = m_streams.Find(packetHdr.stream_id);

        if (pStream == NULL)
        {
            return FALSE;
        }
        else
        {
            return pStream->IsActive();
        }
    }
}


//-------------------------------------------------------------------
// InitPresentationDescriptor
//
// Creates the source's presentation descriptor, if possible.
//
// During the BeginOpen operation, the source reads packets looking
// for headers for each stream. This enables the source to create the
// presentation descriptor, which describes the stream formats.
//
// This method tests whether the source has seen enough packets
// to create the PD. If so, it invokes the callback to complete
// the BeginOpen operation.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::InitPresentationDescriptor()
{
    HRESULT hr = S_OK;
    DWORD cStreams = 0;

    assert(m_pPresentationDescriptor == NULL);
    assert(m_state == STATE_OPENING);

    if (m_pHeader == NULL)
    {
        return E_FAIL;
    }

    // Get the number of streams, as declared in the MPEG-1 header, skipping
    // any streams with an unsupported format.
    for (DWORD i = 0; i < m_pHeader->cStreams; i++)
    {
        if (IsStreamTypeSupported(m_pHeader->streams[i].type))
        {
            cStreams++;
        }
    }

    // How many streams do we actually have?
    if (cStreams > m_streams.GetCount())
    {
        // Keep reading data until we have seen a packet for each stream.
        return S_OK;
    }

    // We should never create a stream we don't support.
    assert(cStreams == m_streams.GetCount());

    // Ready to create the presentation descriptor.

    // Create an array of IMFStreamDescriptor pointers.
    IMFStreamDescriptor **ppSD =
            new (std::nothrow) IMFStreamDescriptor*[cStreams];

    if (ppSD == NULL)
    {
        hr = E_OUTOFMEMORY;
        goto done;
    }

    ZeroMemory(ppSD, cStreams * sizeof(IMFStreamDescriptor*));

    // Fill the array by getting the stream descriptors from the streams.
    for (DWORD i = 0; i < cStreams; i++)
    {
        hr = m_streams[i]->GetStreamDescriptor(&ppSD[i]);
        if (FAILED(hr))
        {
            goto done;
        }
    }

    // Create the presentation descriptor.
    hr = MFCreatePresentationDescriptor(cStreams, ppSD,
        &m_pPresentationDescriptor);

    if (FAILED(hr))
    {
        goto done;
    }

    // Select the first video stream (if any).
    for (DWORD i = 0; i < cStreams; i++)
    {
        GUID majorType = GUID_NULL;

        hr = GetStreamMajorType(ppSD[i], &majorType);
        if (FAILED(hr))
        {
            goto done;
        }

        hr = m_pPresentationDescriptor->SelectStream(i);
        if (FAILED(hr))
        {
            goto done;
        }
    }

    // Switch state from "opening" to stopped.
    m_state = STATE_STOPPED;

    // Invoke the async callback to complete the BeginOpen operation.
    hr = CompleteOpen(S_OK);

done:
    // clean up:
    if (ppSD)
    {
        for (DWORD i = 0; i < cStreams; i++)
        {
            SafeRelease(&ppSD[i]);
        }
        delete [] ppSD;
    }
    return hr;
}


//-------------------------------------------------------------------
// QueueAsyncOperation
// Queue an asynchronous operation.
//
// OpType: Type of operation to queue.
//
// Note: If the SourceOp object requires additional information, call
// OpQueue<SourceOp>::QueueOperation, which takes a SourceOp pointer.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::QueueAsyncOperation(SourceOp::Operation OpType)
{
    HRESULT hr = S_OK;
    SourceOp *pOp = NULL;

    hr = SourceOp::CreateOp(OpType, &pOp);

    if (SUCCEEDED(hr))
    {
        hr = QueueOperation(pOp);
    }

    SafeRelease(&pOp);
    return hr;
}

//-------------------------------------------------------------------
// BeginAsyncOp
//
// Starts an asynchronous operation. Called by the source at the
// begining of any asynchronous operation.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::BeginAsyncOp(SourceOp *pOp)
{
    // At this point, the current operation should be NULL (the
    // previous operation is NULL) and the new operation (pOp)
    // must not be NULL.

    if (pOp == NULL || m_pCurrentOp != NULL)
    {
        assert(FALSE);
        return E_FAIL;
    }

    // Store the new operation as the current operation.

    m_pCurrentOp = pOp;
    m_pCurrentOp->AddRef();

    return S_OK;
}

//-------------------------------------------------------------------
// CompleteAsyncOp
//
// Completes an asynchronous operation. Called by the source at the
// end of any asynchronous operation.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::CompleteAsyncOp(SourceOp *pOp)
{
    HRESULT hr = S_OK;

    // At this point, the current operation (m_pCurrentOp)
    // must match the operation that is ending (pOp).

    if (pOp == NULL || m_pCurrentOp == NULL)
    {
        assert(FALSE);
        return E_FAIL;
    }

    if (m_pCurrentOp != pOp)
    {
        assert(FALSE);
        return E_FAIL;
    }

    // Release the current operation.
    SafeRelease(&m_pCurrentOp);

    // Process the next operation on the queue.
    hr = ProcessQueue();

    return hr;
}

//-------------------------------------------------------------------
// DispatchOperation
//
// Performs the asynchronous operation indicated by pOp.
//
// NOTE:
// This method implements the pure-virtual OpQueue::DispatchOperation
// method. It is always called from a work-queue thread.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::DispatchOperation(SourceOp *pOp)
{
    EnterCriticalSection(&m_critSec);

    HRESULT hr = S_OK;

    if (m_state == STATE_SHUTDOWN)
    {
        LeaveCriticalSection(&m_critSec);

        return S_OK; // Already shut down, ignore the request.
    }

    switch (pOp->Op())
    {

    // IMFMediaSource methods:

    case SourceOp::OP_START:
        hr = DoStart((StartOp*)pOp);
        break;

    case SourceOp::OP_STOP:
        hr = DoStop(pOp);
        break;

    case SourceOp::OP_PAUSE:
        hr = DoPause(pOp);
        break;

    case SourceOp::OP_SETRATE:
        hr = DoSetRate(pOp);
        break;

    // Operations requested by the streams:

    case SourceOp::OP_REQUEST_DATA:
        hr = OnStreamRequestSample(pOp);
        break;

    case SourceOp::OP_END_OF_STREAM:
        hr = OnEndOfStream(pOp);
        break;

    default:
        hr = E_UNEXPECTED;
    }

    if (FAILED(hr))
    {
        StreamingError(hr);
    }

    LeaveCriticalSection(&m_critSec);
    return hr;
}


//-------------------------------------------------------------------
// ValidateOperation
//
// Checks whether the source can perform the operation indicated
// by pOp at this time.
//
// If the source cannot perform the operation now, the method
// returns MF_E_NOTACCEPTING.
//
// NOTE:
// Implements the pure-virtual OpQueue::ValidateOperation method.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::ValidateOperation(SourceOp *pOp)
{
    if (m_pCurrentOp != NULL)
    {
        return MF_E_NOTACCEPTING;
    }
    return S_OK;
}



//-------------------------------------------------------------------
// DoStart
// Perform an async start operation (IMFMediaSource::Start)
//
// pOp: Contains the start parameters.
//
// Note: This sample currently does not implement seeking, and the
// Start() method fails if the caller requests a seek.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::DoStart(StartOp *pOp)
{
    assert(pOp->Op() == SourceOp::OP_START);

    IMFPresentationDescriptor *pPD = NULL;
    IMFMediaEvent  *pEvent = NULL;

    HRESULT     hr = S_OK;
    LONGLONG    llStartOffset = 0;
    BOOL        bRestartFromCurrentPosition = FALSE;
    BOOL        bSentEvents = FALSE;

    hr = BeginAsyncOp(pOp);

    // Get the presentation descriptor from the SourceOp object.
    // This is the PD that the caller passed into the Start() method.
    // The PD has already been validated.
    if (SUCCEEDED(hr))
    {
        hr = pOp->GetPresentationDescriptor(&pPD);
    }
    // Because this sample does not support seeking, the start
    // position must be 0 (from stopped) or "current position."

    // If the sample supported seeking, we would need to get the
    // start position from the PROPVARIANT data contained in pOp.

    if (SUCCEEDED(hr))
    {
        // Select/deselect streams, based on what the caller set in the PD.
        // This method also sends the MENewStream/MEUpdatedStream events.
        hr = SelectStreams(pPD, pOp->Data());
    }

    if (SUCCEEDED(hr))
    {
        m_state = STATE_STARTED;

        // Queue the "started" event. The event data is the start position.
        hr = m_pEventQueue->QueueEventParamVar(
            MESourceStarted,
            GUID_NULL,
            S_OK,
            &pOp->Data()
            );
    }

    if (FAILED(hr))
    {
        // Failure. Send the error code to the application.

        // Note: It's possible that QueueEvent itself failed, in which case it
        // is likely to fail again. But there is no good way to recover in
        // that case.

        (void)m_pEventQueue->QueueEventParamVar(
            MESourceStarted, GUID_NULL, hr, NULL);
    }

    CompleteAsyncOp(pOp);

    SafeRelease(&pEvent);
    SafeRelease(&pPD);
    return hr;
}


//-------------------------------------------------------------------
// DoStop
// Perform an async stop operation (IMFMediaSource::Stop)
//-------------------------------------------------------------------

HRESULT CMPEG1Source::DoStop(SourceOp *pOp)
{
    HRESULT hr = S_OK;
    QWORD qwCurrentPosition = 0;

    hr = BeginAsyncOp(pOp);

    // Stop the active streams.
    if (SUCCEEDED(hr))
    {
        for (DWORD i = 0; i < m_streams.GetCount(); i++)
        {
            if (m_streams[i]->IsActive())
            {
                hr = m_streams[i]->Stop();
            }
            if (FAILED(hr))
            {
                break;
            }
        }
    }

    // Seek to the start of the file. If we restart after stopping,
    // we will start from the beginning of the file again.
    if (SUCCEEDED(hr))
    {
        hr = m_pByteStream->Seek(
            msoBegin,
            0,
            MFBYTESTREAM_SEEK_FLAG_CANCEL_PENDING_IO,
            &qwCurrentPosition
            );
    }

    // Increment the counter that tracks "stale" read requests.
    if (SUCCEEDED(hr))
    {
        ++m_cRestartCounter; // This counter is allowed to overflow.
    }

    SafeRelease(&m_pSampleRequest);

    m_state = STATE_STOPPED;

    // Send the "stopped" event. This might include a failure code.
    (void)m_pEventQueue->QueueEventParamVar(MESourceStopped, GUID_NULL, hr, NULL);

    CompleteAsyncOp(pOp);

    return hr;
}


//-------------------------------------------------------------------
// DoPause
// Perform an async pause operation (IMFMediaSource::Pause)
//-------------------------------------------------------------------

HRESULT CMPEG1Source::DoPause(SourceOp *pOp)
{
    HRESULT hr = S_OK;

    hr = BeginAsyncOp(pOp);

    // Pause is only allowed while running.
    if (SUCCEEDED(hr))
    {
        if (m_state != STATE_STARTED)
        {
            hr = MF_E_INVALID_STATE_TRANSITION;
        }
    }

    // Pause the active streams.
    if (SUCCEEDED(hr))
    {
        for (DWORD i = 0; i < m_streams.GetCount(); i++)
        {
            if (m_streams[i]->IsActive())
            {
                hr = m_streams[i]->Pause();
            }
            if (FAILED(hr))
            {
                break;
            }
        }
    }

    m_state = STATE_PAUSED;


    // Send the "paused" event. This might include a failure code.
    (void)m_pEventQueue->QueueEventParamVar(MESourcePaused, GUID_NULL, hr, NULL);

    CompleteAsyncOp(pOp);

    return hr;
}

//-------------------------------------------------------------------
// DoSetRate
// Perform an async set rate operation (IMFRateControl::SetRate)
//-------------------------------------------------------------------

HRESULT CMPEG1Source::DoSetRate(SourceOp *pOp)
{
    HRESULT hr = S_OK;
    SetRateOp *pSetRateOp = static_cast<SetRateOp*>(pOp);

    hr = BeginAsyncOp(pOp);

    // Set rate on active streams.
    if (SUCCEEDED(hr))
    {
        for (DWORD i = 0; i < m_streams.GetCount(); i++)
        {
            if (m_streams[i]->IsActive())
            {
                hr = m_streams[i]->SetRate(pSetRateOp->GetRate());
            }
            if (FAILED(hr))
            {
                break;
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        m_flRate = pSetRateOp->GetRate();
    }

    // Send the "rate changted" event. This might include a failure code.
    (void)m_pEventQueue->QueueEventParamVar(MESourceRateChanged, GUID_NULL, hr, NULL);

    CompleteAsyncOp(pOp);

    return hr;
}

//-------------------------------------------------------------------
// StreamRequestSample
// Called by streams when they need more data.
//
// Note: This is an async operation. The stream requests more data
// by queueing an OP_REQUEST_DATA operation.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::OnStreamRequestSample(SourceOp *pOp)
{
    HRESULT hr = S_OK;

    hr = BeginAsyncOp(pOp);

    // Ignore this request if we are already handling an earlier request.
    // (In that case m_pSampleRequest will be non-NULL.)

    if (SUCCEEDED(hr))
    {
        if (m_pSampleRequest == NULL)
        {
            // Add the request counter as data to the operation.
            // This counter tracks whether a read request becomes "stale."

            PROPVARIANT var;
            var.vt = VT_UI4;
            var.ulVal = m_cRestartCounter;

            hr = pOp->SetData(var);

            if (SUCCEEDED(hr))
            {
                // Store this while the request is pending.
                m_pSampleRequest = pOp;
                m_pSampleRequest->AddRef();

                // Try to parse data - this will invoke a read request if needed.
                ParseData();
            }
        }

        CompleteAsyncOp(pOp);
    }

    return hr;
}


//-------------------------------------------------------------------
// OnEndOfStream
// Called by each stream when it sends the last sample in the stream.
//
// Note: When the media source reaches the end of the MPEG-1 stream,
// it calls EndOfStream on each stream object. The streams might have
// data still in their queues. As each stream empties its queue, it
// notifies the source through an async OP_END_OF_STREAM operation.
//
// When every stream notifies the source, the source can send the
// "end-of-presentation" event.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::OnEndOfStream(SourceOp *pOp)
{
    HRESULT hr = S_OK;

    hr = BeginAsyncOp(pOp);

    // Decrement the count of end-of-stream notifications.
    if (SUCCEEDED(hr))
    {
        --m_cPendingEOS;
        if (m_cPendingEOS == 0)
        {
            // No more streams. Send the end-of-presentation event.
            hr = m_pEventQueue->QueueEventParamVar(MEEndOfPresentation, GUID_NULL, S_OK, NULL);
        }

    }

    if (SUCCEEDED(hr))
    {
        hr = CompleteAsyncOp(pOp);
    }

    return hr;
}



//-------------------------------------------------------------------
// SelectStreams
// Called during START operations to select and deselect streams.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::SelectStreams(
    IMFPresentationDescriptor *pPD,   // Presentation descriptor.
    const PROPVARIANT varStart        // New start position.
    )
{
    HRESULT hr = S_OK;
    BOOL    fSelected = FALSE;
    BOOL    fWasSelected = FALSE;
    DWORD   stream_id = 0;

    IMFStreamDescriptor *pSD = NULL;

    CMPEG1Stream *pStream = NULL; // Not add-ref'd

    // Reset the pending EOS count.
    m_cPendingEOS = 0;

    // Loop throught the stream descriptors to find which streams are active.
    for (DWORD i = 0; i < m_streams.GetCount(); i++)
    {
        hr = pPD->GetStreamDescriptorByIndex(i, &fSelected, &pSD);
        if (FAILED(hr))
        {
            goto done;
        }

        hr = pSD->GetStreamIdentifier(&stream_id);
        if (FAILED(hr))
        {
            goto done;
        }

        pStream = m_streams.Find((BYTE)stream_id);
        if (pStream == NULL)
        {
            hr = E_INVALIDARG;
            goto done;
        }

        // Was the stream active already?
        fWasSelected = pStream->IsActive();

        // Activate or deactivate the stream.
        hr = pStream->Activate(fSelected);
        if (FAILED(hr))
        {
            goto done;
        }

        if (fSelected)
        {
            m_cPendingEOS++;

            // If the stream was previously selected, send an "updated stream"
            // event. Otherwise, send a "new stream" event.
            MediaEventType met = fWasSelected ? MEUpdatedStream : MENewStream;

            hr = m_pEventQueue->QueueEventParamUnk(met, GUID_NULL, hr, pStream);
            if (FAILED(hr))
            {
                goto done;
            }

            // Start the stream. The stream will send the appropriate event.
            hr = pStream->Start(varStart);
            if (FAILED(hr))
            {
                goto done;
            }
        }
        SafeRelease(&pSD);
    }

done:
    SafeRelease(&pSD);
    return hr;
}


//-------------------------------------------------------------------
// RequestData
// Request the next batch of data.
//
// cbRequest: Amount of data to read, in bytes.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::RequestData(DWORD cbRequest)
{
    HRESULT hr = S_OK;

    // Reserve a sufficient read buffer.
    hr = m_ReadBuffer.Reserve(cbRequest);

    // Submit the async read request.
    // When it completes, our OnByteStreamRead method will be invoked.

    if (SUCCEEDED(hr))
    {
        hr = m_pByteStream->BeginRead(
            m_ReadBuffer.DataPtr() + m_ReadBuffer.DataSize(),
            cbRequest,
            &m_OnByteStreamRead,
            m_pSampleRequest
            );
    }

    return hr;
}


//-------------------------------------------------------------------
// ParseData
// Parses the next batch of data.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::ParseData()
{
    HRESULT hr = S_OK;

    DWORD cbNextRequest = 0;
    BOOL  bNeedMoreData = FALSE;

    // Keep processing data until
    // (a) All streams have enough samples, or
    // (b) The parser needs more data in the buffer.

    while ( StreamsNeedData() )
    {
        DWORD cbAte = 0;    // How much data we consumed from the read buffer.

        // Check if we got the first system header.
        if (m_pHeader == NULL && m_pParser->HasSystemHeader())
        {
            hr = m_pParser->GetSystemHeader(&m_pHeader);

            if (FAILED(hr)) { break; }
        }

        if (m_pParser->IsEndOfStream())
        {
            // The parser reached the end of the MPEG-1 stream. Notify the streams.
            hr = EndOfMPEGStream();
        }
        else if (m_pParser->HasPacket())
        {
            // The parser reached the start of a new packet.
            hr = ReadPayload(&cbAte, &cbNextRequest);
        }
        else
        {
            // Parse more data.
            hr = m_pParser->ParseBytes(m_ReadBuffer.DataPtr(), m_ReadBuffer.DataSize(), &cbAte);
        }

        // Parser::ParseBytes() or ReadPayload can return S_FALSE, meaning "Need more data"
        if (hr == S_FALSE)
        {
            bNeedMoreData = TRUE;
        }

        if (FAILED(hr)) { break; }

        // Advance the start of the read buffer by the amount consumed.
        hr = m_ReadBuffer.MoveStart(cbAte);

        if (FAILED(hr)) { break; }

        // If we need more data, start an async read operation.
        if (bNeedMoreData)
        {
            hr = RequestData( max(READ_SIZE, cbNextRequest) );

            // Break from the loop because we need to wait for the async read to complete.
            break;
        }
    }

    // Flag our state. If a stream requests more data while we are waiting for an async
    // read to complete, we can ignore the stream's request, because the request will be
    // dispatched as soon as we get more data.

    if (SUCCEEDED(hr))
    {
        if (!bNeedMoreData)
        {
            SafeRelease(&m_pSampleRequest);
        }
    }

    return hr;
}

//-------------------------------------------------------------------
// ReadPayload
// Read the next MPEG-1 payload.
//
// When this method is called:
// - The read position has reached the beginning of a payload.
// - We have the packet header, but not necessarily the entire payload.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::ReadPayload(DWORD *pcbAte, DWORD *pcbNextRequest)
{
    assert(m_pParser != NULL);
    assert(m_pParser->HasPacket());

    HRESULT hr = S_OK;

    DWORD cbPayloadRead = 0;
    DWORD cbPayloadUnread = 0;

    // At this point, the read buffer might be larger or smaller than the payload.
    // Calculate which portion of the payload has been read.
    if (m_pParser->PayloadSize() > m_ReadBuffer.DataSize())
    {
        cbPayloadUnread = m_pParser->PayloadSize() - m_ReadBuffer.DataSize();
    }

    cbPayloadRead = m_pParser->PayloadSize() - cbPayloadUnread;

    // Do we need to deliver this payload?
    if ( !IsStreamActive(m_pParser->PacketHeader()) )
    {
        QWORD qwCurrentPosition = 0;

        // Skip this payload. Seek past the unread portion of the payload.
        hr = m_pByteStream->Seek(
            msoCurrent,
            cbPayloadUnread,
            MFBYTESTREAM_SEEK_FLAG_CANCEL_PENDING_IO,
            &qwCurrentPosition
            );

        // Advance the data buffer to the end of payload, or the portion
        // that has been read.

        if (SUCCEEDED(hr))
        {
            *pcbAte = cbPayloadRead;

            // Tell the parser that we are done with this packet.
            m_pParser->ClearPacket();
        }

    }
    else if (cbPayloadUnread > 0)
    {
        // Some portion of this payload has not been read. Schedule a read.
        *pcbNextRequest = cbPayloadUnread;

        *pcbAte = 0;

        hr = S_FALSE; // Need more data.
    }
    else
    {
        // The entire payload is in the data buffer. Deliver the packet.
        hr = DeliverPayload();

        if (SUCCEEDED(hr))
        {
            *pcbAte = cbPayloadRead;

            // Tell the parser that we are done with this packet.
            m_pParser->ClearPacket();
        }
    }

    return hr;
}

//-------------------------------------------------------------------
// EndOfMPEGStream:
// Called when the parser reaches the end of the MPEG1 stream.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::EndOfMPEGStream()
{
    // Notify the streams. The streams might have pending samples.
    // When each stream delivers the last sample, it will send the
    // end-of-stream event to the pipeline and then notify the
    // source.

    // When every stream is done, the source sends the end-of-
    // presentation event.

    HRESULT hr = S_OK;

    for (DWORD i = 0; i < m_streams.GetCount(); i++)
    {
        if (m_streams[i]->IsActive())
        {
            hr = m_streams[i]->EndOfStream();
        }
        if (FAILED(hr))
        {
            break;
        }
    }

    return hr;
}



//-------------------------------------------------------------------
// StreamsNeedData:
// Returns TRUE if any streams need more data.
//-------------------------------------------------------------------

BOOL CMPEG1Source::StreamsNeedData() const
{
    BOOL bNeedData = FALSE;

    switch (m_state)
    {
    case STATE_OPENING:
        // While opening, we always need data (until we get enough
        // to complete the open operation).
        return TRUE;

    case STATE_SHUTDOWN:
        // While shut down, we never need data.
        return FALSE;

    default:
        // If none of the above, ask the streams.
        for (DWORD i = 0; i < m_streams.GetCount(); i++)
        {
            if (m_streams[i]->NeedsData())
            {
                bNeedData = TRUE;
                break;
            }
        }
        return bNeedData;
    }
}


//-------------------------------------------------------------------
// DeliverPayload:
// Delivers an MPEG-1 payload.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::DeliverPayload()
{
    // When this method is called, the read buffer contains a complete
    // payload, and the payload belongs to a stream whose type we support.

    assert(m_pParser->HasPacket());

    HRESULT             hr = S_OK;
    MPEG1PacketHeader   packetHdr;
    CMPEG1Stream        *pStream = NULL;    // not AddRef'd

    IMFMediaBuffer      *pBuffer = NULL;
    IMFSample           *pSample = NULL;
    BYTE                *pData = NULL;      // Pointer to the IMFMediaBuffer data.

    packetHdr = m_pParser->PacketHeader();

    if (packetHdr.cbPayload > m_ReadBuffer.DataSize())
    {
        assert(FALSE);
        hr = E_UNEXPECTED;
    }

    // If we are still opening the file, then we might need to create this stream.
    if (SUCCEEDED(hr))
    {
        if (m_state == STATE_OPENING)
        {
            hr = CreateStream(packetHdr);
        }
    }

    if (SUCCEEDED(hr))
    {
        pStream = m_streams.Find(packetHdr.stream_id);
        assert(pStream != NULL);
    }

    // Create a media buffer for the payload.
    if (SUCCEEDED(hr))
    {
        hr = MFCreateMemoryBuffer(packetHdr.cbPayload, &pBuffer);
    }

    if (SUCCEEDED(hr))
    {
        hr = pBuffer->Lock(&pData, NULL, NULL);
    }

    if (SUCCEEDED(hr))
    {
        CopyMemory(pData, m_ReadBuffer.DataPtr(), packetHdr.cbPayload);
    }

    if (SUCCEEDED(hr))
    {
        hr = pBuffer->Unlock();
    }

    if (SUCCEEDED(hr))
    {
        hr = pBuffer->SetCurrentLength(packetHdr.cbPayload);
    }

    // Create a sample to hold the buffer.
    if (SUCCEEDED(hr))
    {
        hr = MFCreateSample(&pSample);
    }
    if (SUCCEEDED(hr))
    {
        hr = pSample->AddBuffer(pBuffer);
    }

    // Time stamp the sample.
    if (SUCCEEDED(hr))
    {
        if (packetHdr.bHasPTS)
        {
            LONGLONG hnsStart = packetHdr.PTS * 10000 / 90;

            hr = pSample->SetSampleTime(hnsStart);
        }
    }

    // Deliver the payload to the stream.
    if (SUCCEEDED(hr))
    {
        hr = pStream->DeliverPayload(pSample);
    }

    // If the open operation is still pending, check if we're done.
    if (SUCCEEDED(hr))
    {
        if (m_state == STATE_OPENING)
        {
            hr = InitPresentationDescriptor();
        }
    }

    SafeRelease(&pBuffer);
    SafeRelease(&pSample);
    return hr;
}



//-------------------------------------------------------------------
// CreateStream:
// Creates a media stream, based on a packet header.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::CreateStream(const MPEG1PacketHeader& packetHdr)
{
    // We validate the stream type before calling this method.
    assert(IsStreamTypeSupported(packetHdr.type));


    // First see if the stream already exists.
    if ( m_streams.Find(packetHdr.stream_id) != NULL )
    {
        // The stream already exists. Nothing to do.
        return S_OK;
    }

    HRESULT hr = S_OK;
    DWORD cbAte = 0;
    BYTE *pPayload = NULL;

    IMFMediaType *pType = NULL;
    IMFStreamDescriptor *pSD = NULL;
    CMPEG1Stream *pStream = NULL;
    IMFMediaTypeHandler *pHandler = NULL;

    MPEG1VideoSeqHeader videoSeqHdr;
    MPEG1AudioFrameHeader audioFrameHeader;

    // Get a pointer to the start of the payload.
    pPayload = m_ReadBuffer.DataPtr();

    // Create a media type, based on the packet type (audio/video)
    switch (packetHdr.type)
    {
    case StreamType_Video:
        // Video: Read the sequence header and use it to create a media type.
        hr = ReadVideoSequenceHeader(pPayload, packetHdr.cbPayload, videoSeqHdr, &cbAte);
        if (SUCCEEDED(hr))
        {
            hr = CreateVideoMediaType(videoSeqHdr, &pType);
        }
        break;

    case StreamType_Audio:
        // Audio: Read the frame header and use it to create a media type.
        hr = ReadAudioFrameHeader(pPayload, packetHdr.cbPayload, audioFrameHeader, &cbAte);
        if (SUCCEEDED(hr))
        {
            hr = CreateAudioMediaType(audioFrameHeader, &pType);
        }
        break;

    default:
        assert(false); // If this case occurs, then IsStreamTypeSupported() is wrong.
        hr = E_UNEXPECTED;
    }

    if (SUCCEEDED(hr))
    {
        // Create the stream descriptor from the media type.
        hr = MFCreateStreamDescriptor(packetHdr.stream_id, 1, &pType, &pSD);
    }

    // Set the default media type on the stream handler.
    if (SUCCEEDED(hr))
    {
        hr = pSD->GetMediaTypeHandler(&pHandler);
    }
    if (SUCCEEDED(hr))
    {
        hr = pHandler->SetCurrentMediaType(pType);
    }

    // Create the new stream.
    if (SUCCEEDED(hr))
    {
        pStream = new (std::nothrow) CMPEG1Stream(this, pSD, hr);
        if (pStream == NULL)
        {
            hr = E_OUTOFMEMORY;
        }
    }

    // Add the stream to the array.
    if (SUCCEEDED(hr))
    {
        hr = m_streams.AddStream(packetHdr.stream_id, pStream);
    }

    SafeRelease(&pSD);
    SafeRelease(&pStream);
    return hr;
}


//-------------------------------------------------------------------
// ValidatePresentationDescriptor:
// Validates the presentation descriptor that the caller specifies
// in IMFMediaSource::Start().
//
// Note: This method performs a basic sanity check on the PD. It is
// not intended to be a thorough validation.
//-------------------------------------------------------------------

HRESULT CMPEG1Source::ValidatePresentationDescriptor(IMFPresentationDescriptor *pPD)
{
    HRESULT hr = S_OK;
    BOOL fSelected = FALSE;
    DWORD cStreams = 0;

    IMFStreamDescriptor *pSD = NULL;

    if (m_pHeader == NULL)
    {
        return E_UNEXPECTED;
    }

    // The caller's PD must have the same number of streams as ours.
    hr = pPD->GetStreamDescriptorCount(&cStreams);

    if (SUCCEEDED(hr))
    {
        if (cStreams != m_pHeader->cStreams)
        {
            hr = E_INVALIDARG;
        }
    }

    // The caller must select at least one stream.
    if (SUCCEEDED(hr))
    {
        for (DWORD i = 0; i < cStreams; i++)
        {
            hr = pPD->GetStreamDescriptorByIndex(i, &fSelected, &pSD);
            if (FAILED(hr))
            {
                break;
            }
            if (fSelected)
            {
                break;
            }
            SafeRelease(&pSD);
        }
    }

    if (SUCCEEDED(hr))
    {
        if (!fSelected)
        {
            hr = E_INVALIDARG;
        }
    }

    SafeRelease(&pSD);
    return hr;
}


//-------------------------------------------------------------------
// StreamingError:
// Handles an error that occurs duing an asynchronous operation.
//
// hr: Error code of the operation that failed.
//-------------------------------------------------------------------

void CMPEG1Source::StreamingError(HRESULT hr)
{
    if (m_state == STATE_OPENING)
    {
        // An error happened during BeginOpen.
        // Invoke the callback with the status code.

        CompleteOpen(hr);
    }
    else if (m_state != STATE_SHUTDOWN)
    {
        // An error occurred during streaming. Send the MEError event
        // to notify the pipeline.

        QueueEvent(MEError, GUID_NULL, hr, NULL);
    }
}

BOOL CMPEG1Source::IsRateSupported(float flRate, float *pflAdjustedRate)
{
    if (flRate < 0.00001f && flRate > -0.00001f)
    {
        *pflAdjustedRate = 0.0f;
        return TRUE;
    }
    else if(flRate < 1.0001f && flRate > 0.9999f)
    {
        *pflAdjustedRate = 1.0f;
        return TRUE;
    }
    return FALSE;
}


/* SourceOp class */


//-------------------------------------------------------------------
// CreateOp
// Static method to create a SourceOp instance.
//
// op: Specifies the async operation.
// ppOp: Receives a pointer to the SourceOp object.
//-------------------------------------------------------------------

HRESULT SourceOp::CreateOp(SourceOp::Operation op, SourceOp **ppOp)
{
    if (ppOp == NULL)
    {
        return E_POINTER;
    }

    SourceOp *pOp = new (std::nothrow) SourceOp(op);
    if (pOp  == NULL)
    {
        return E_OUTOFMEMORY;
    }
    *ppOp = pOp;

    return S_OK;
}

//-------------------------------------------------------------------
// CreateStartOp:
// Static method to create a SourceOp instance for the Start()
// operation.
//
// pPD: Presentation descriptor from the caller.
// ppOp: Receives a pointer to the SourceOp object.
//-------------------------------------------------------------------

HRESULT SourceOp::CreateStartOp(IMFPresentationDescriptor *pPD, SourceOp **ppOp)
{
    if (ppOp == NULL)
    {
        return E_POINTER;
    }

    SourceOp *pOp = new (std::nothrow) StartOp(pPD);
    if (pOp == NULL)
    {
        return E_OUTOFMEMORY;
    }

    *ppOp = pOp;
    return S_OK;
}

//-------------------------------------------------------------------
// CreateSetRateOp:
// Static method to create a SourceOp instance for the SetRate()
// operation.
//
// fThin: TRUE - thinning is on, FALSE otherwise
// flRate: New rate
// ppOp: Receives a pointer to the SourceOp object.
//-------------------------------------------------------------------

HRESULT SourceOp::CreateSetRateOp(BOOL fThin, float flRate, SourceOp **ppOp)
{
    if (ppOp == NULL)
    {
        return E_POINTER;
    }

    SourceOp *pOp = new (std::nothrow) SetRateOp(fThin, flRate);
    if (pOp == NULL)
    {
        return E_OUTOFMEMORY;
    }

    *ppOp = pOp;
    return S_OK;
}

ULONG SourceOp::AddRef()
{
    return _InterlockedIncrement(&m_cRef);
}

ULONG SourceOp::Release()
{
    LONG cRef = _InterlockedDecrement(&m_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

HRESULT SourceOp::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }

    HRESULT hr = E_NOINTERFACE;
    if (riid == IID_IUnknown)
    {
        (*ppv) = static_cast<IUnknown *>(this);
        AddRef();
        hr = S_OK;
    }

    return hr;
}

SourceOp::SourceOp(Operation op) : m_cRef(1), m_op(op)
{
    ZeroMemory(&m_data, sizeof(m_data));
}

SourceOp::~SourceOp()
{
    PropVariantClear(&m_data);
}

HRESULT SourceOp::SetData(const PROPVARIANT& var)
{
    return PropVariantCopy(&m_data, &var);
}


StartOp::StartOp(IMFPresentationDescriptor *pPD) : SourceOp(SourceOp::OP_START), m_pPD(pPD)
{
    if (m_pPD)
    {
        m_pPD->AddRef();
    }
}

StartOp::~StartOp()
{
    SafeRelease(&m_pPD);
}


HRESULT StartOp::GetPresentationDescriptor(IMFPresentationDescriptor **ppPD)
{
    if (ppPD == NULL)
    {
        return E_POINTER;
    }
    if (m_pPD == NULL)
    {
        return MF_E_INVALIDREQUEST;
    }
    *ppPD = m_pPD;
    (*ppPD)->AddRef();
    return S_OK;
}

SetRateOp::SetRateOp(BOOL fThin, float flRate)
: SourceOp(SourceOp::OP_SETRATE)
, m_fThin(fThin)
, m_flRate(flRate)
{
}

SetRateOp::~SetRateOp()
{
}

/*  Static functions */


//-------------------------------------------------------------------
// CreateVideoMediaType:
// Create a media type from an MPEG-1 video sequence header.
//-------------------------------------------------------------------

HRESULT CreateVideoMediaType(const MPEG1VideoSeqHeader& videoSeqHdr, IMFMediaType **ppType)
{
    HRESULT hr = S_OK;

    IMFMediaType *pType = NULL;

    hr = MFCreateMediaType(&pType);

    if (SUCCEEDED(hr))
    {
        hr = pType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);
    }

    // Subtype = MPEG-1 payload
    if (SUCCEEDED(hr))
    {
        hr = pType->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_MPG1);
    }

    // Format details.
    if (SUCCEEDED(hr))
    {
        // Frame size

        hr = MFSetAttributeSize(
            pType,
            MF_MT_FRAME_SIZE,
            videoSeqHdr.width,
            videoSeqHdr.height
            );
    }
    if (SUCCEEDED(hr))
    {
        // Frame rate

        hr = MFSetAttributeRatio(
            pType,
            MF_MT_FRAME_RATE,
            videoSeqHdr.frameRate.Numerator,
            videoSeqHdr.frameRate.Denominator
            );
    }
    if (SUCCEEDED(hr))
    {
        // Pixel aspect ratio

        hr = MFSetAttributeRatio(
            pType,
            MF_MT_PIXEL_ASPECT_RATIO,
            videoSeqHdr.pixelAspectRatio.Numerator,
            videoSeqHdr.pixelAspectRatio.Denominator
            );
    }
    if (SUCCEEDED(hr))
    {
        // Average bit rate

        hr = pType->SetUINT32(MF_MT_AVG_BITRATE, videoSeqHdr.bitRate);
    }
    if (SUCCEEDED(hr))
    {

        // Interlacing (progressive frames)

        hr = pType->SetUINT32(MF_MT_INTERLACE_MODE, (UINT32)MFVideoInterlace_Progressive);
    }
    if (SUCCEEDED(hr))
    {
        // Sequence header.

        hr = pType->SetBlob(
            MF_MT_MPEG_SEQUENCE_HEADER,
            videoSeqHdr.header,             // Byte array
            videoSeqHdr.cbHeader            // Size
            );
    }

    if (SUCCEEDED(hr))
    {
        *ppType = pType;
        (*ppType)->AddRef();
    }

    SafeRelease(&pType);
    return hr;
}


//-------------------------------------------------------------------
// CreateAudioMediaType:
// Create a media type from an MPEG-1 audio frame header.
//
// Note: This function fills in an MPEG1WAVEFORMAT structure and then
// converts the structure to a Media Foundation media type
// (IMFMediaType). This is somewhat roundabout but it guarantees
// that the type can be converted back to an MPEG1WAVEFORMAT by the
// decoder if need be.
//
// The WAVEFORMATEX portion of the MPEG1WAVEFORMAT structure is
// converted into attributes on the IMFMediaType object. The rest of
// the struct is stored in the MF_MT_USER_DATA attribute.
//-------------------------------------------------------------------

HRESULT CreateAudioMediaType(const MPEG1AudioFrameHeader& audioHeader, IMFMediaType **ppType)
{
    HRESULT hr = S_OK;
    IMFMediaType *pType = NULL;

    MPEG1WAVEFORMAT format;
    ZeroMemory(&format, sizeof(format));

    format.wfx.wFormatTag = WAVE_FORMAT_MPEG;
    format.wfx.nChannels = audioHeader.nChannels;
    format.wfx.nSamplesPerSec = audioHeader.dwSamplesPerSec;
    if (audioHeader.dwBitRate > 0)
    {
        format.wfx.nAvgBytesPerSec = (audioHeader.dwBitRate * 1000) / 8;
    }
    format.wfx.nBlockAlign = audioHeader.nBlockAlign;
    format.wfx.wBitsPerSample = 0; // Not used.
    format.wfx.cbSize = sizeof(MPEG1WAVEFORMAT) - sizeof(WAVEFORMATEX);

    // MPEG-1 audio layer.
    switch (audioHeader.layer)
    {
    case MPEG1_Audio_Layer1:
        format.fwHeadLayer = ACM_MPEG_LAYER1;
        break;

    case MPEG1_Audio_Layer2:
        format.fwHeadLayer = ACM_MPEG_LAYER2;
        break;

    case MPEG1_Audio_Layer3:
        format.fwHeadLayer = ACM_MPEG_LAYER3;
        break;
    };

    format.dwHeadBitrate = audioHeader.dwBitRate * 1000;

    // Mode
    switch (audioHeader.mode)
    {
    case MPEG1_Audio_Stereo:
        format.fwHeadMode = ACM_MPEG_STEREO;
        break;

    case MPEG1_Audio_JointStereo:
        format.fwHeadMode = ACM_MPEG_JOINTSTEREO;
        break;

    case MPEG1_Audio_DualChannel:
        format.fwHeadMode = ACM_MPEG_DUALCHANNEL;
        break;

    case MPEG1_Audio_SingleChannel:
        format.fwHeadMode = ACM_MPEG_SINGLECHANNEL;
        break;
    };

    if (audioHeader.mode == ACM_MPEG_JOINTSTEREO)
    {
        // Convert the 'mode_extension' field to the correct MPEG1WAVEFORMAT value.
        if (audioHeader.modeExtension <= 0x03)
        {
            format.fwHeadModeExt = 0x01 << audioHeader.modeExtension;
        }
    }

    // Convert the 'emphasis' field to the correct MPEG1WAVEFORMAT value.
    if (audioHeader.emphasis <= 0x03)
    {
        format.wHeadEmphasis = audioHeader.emphasis + 1;
    }

    // The flags translate directly.
    format.fwHeadFlags = audioHeader.wFlags;
    // Add the "MPEG-1" flag, although it's somewhat redundant.
    format.fwHeadFlags |= ACM_MPEG_ID_MPEG1;

    // Use the structure to initialize the Media Foundation media type.
    hr = MFCreateMediaType(&pType);
    if (SUCCEEDED(hr))
    {
        hr = MFInitMediaTypeFromWaveFormatEx(pType, (const WAVEFORMATEX*)&format, sizeof(format));
    }

    if (SUCCEEDED(hr))
    {
        *ppType = pType;
        (*ppType)->AddRef();
    }

    SafeRelease(&pType);
    return hr;
}

// Get the major media type from a stream descriptor.
HRESULT GetStreamMajorType(IMFStreamDescriptor *pSD, GUID *pguidMajorType)
{
    if (pSD == NULL) { return E_POINTER; }
    if (pguidMajorType == NULL) { return E_POINTER; }

    HRESULT hr = S_OK;
    IMFMediaTypeHandler *pHandler = NULL;

    hr = pSD->GetMediaTypeHandler(&pHandler);
    if (SUCCEEDED(hr))
    {
        hr = pHandler->GetMajorType(pguidMajorType);
    }
    SafeRelease(&pHandler);
    return hr;
}


BOOL SampleRequestMatch(SourceOp *pOp1, SourceOp *pOp2)
{
    if ((pOp1 == NULL) && (pOp2 == NULL))
    {
        return TRUE;
    }
    else if ((pOp1 == NULL) || (pOp2 == NULL))
    {
        return FALSE;
    }
    else
    {
        return (pOp1->Data().ulVal == pOp2->Data().ulVal);
    }
}

#pragma warning( pop )
