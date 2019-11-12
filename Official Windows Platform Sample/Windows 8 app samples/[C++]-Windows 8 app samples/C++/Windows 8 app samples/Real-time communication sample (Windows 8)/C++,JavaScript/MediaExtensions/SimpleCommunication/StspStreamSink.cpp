//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "StdAfx.h"
#include "StspStreamSink.h"
#include "StspMediaSink.h"
#include "Marker.h"

using namespace Stsp;

#define SET_SAMPLE_FLAG(dest, destMask, pSample, flagName) \
    { \
        UINT32 unValue; \
        if (SUCCEEDED(pSample->GetUINT32(MFSampleExtension_##flagName, &unValue))) \
        { \
            dest |= (unValue != FALSE) ? StspSampleFlag_##flagName : 0; \
            destMask |= StspSampleFlag_##flagName; \
        } \
    }

CStreamSink::CStreamSink(DWORD dwIdentifier)
    : _cRef(1)
    , _dwIdentifier(dwIdentifier)
    , _state(State_TypeNotSet)
    , _IsShutdown(false)
    , _Connected(false)
    , _fIsVideo(false)
    , _fGetStartTimeFromSample(false)
    , _fWaitingForFirstSample(false)
    , _fFirstSampleAfterConnect(false)
    , _StartTime(0)
    , _WorkQueueId(0)
    , _pParent(nullptr)
#pragma warning(push)
#pragma warning(disable:4355)
    , _WorkQueueCB(this, &CStreamSink::OnDispatchWorkItem)
    , _OnFirstVideoSampleSentCB(this, &CStreamSink::OnFirstVideoSampleSent)
    , _OnSampleSentCB(this, &CStreamSink::OnSampleSent)
#pragma warning(pop)
{
    ZeroMemory(&_guiCurrentSubtype, sizeof(_guiCurrentSubtype));
}

CStreamSink::~CStreamSink()
{
    assert(_IsShutdown);
}


// IUnknown methods

IFACEMETHODIMP CStreamSink::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    (*ppv) = nullptr;

    HRESULT hr = S_OK;
    if (riid == IID_IUnknown || 
        riid == IID_IMFStreamSink ||
        riid == IID_IMFMediaEventGenerator)
    {
        (*ppv) = static_cast<IMFStreamSink*>(this);
        AddRef();
    }
    else if (riid == IID_IMFMediaTypeHandler)
    {
        (*ppv) = static_cast<IMFMediaTypeHandler*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    if (FAILED(hr) && riid == IID_IMarshal)
    {
        if (_spFTM == nullptr)
        {
            AutoLock lock(_critSec);
            if (_spFTM == nullptr)
            {
                hr = CoCreateFreeThreadedMarshaler(static_cast<IMFStreamSink*>(this), &_spFTM);
            }
        }

        if (SUCCEEDED(hr))
        {
            if (_spFTM == nullptr)
            {
                hr = E_UNEXPECTED;
            }
            else
            {
                hr = _spFTM.Get()->QueryInterface(riid, ppv);
            }
        }
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CStreamSink::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CStreamSink::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

// IMFMediaEventGenerator methods.
// Note: These methods call through to the event queue helper object.

IFACEMETHODIMP CStreamSink::BeginGetEvent(IMFAsyncCallback* pCallback, IUnknown* punkState)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->BeginGetEvent(pCallback, punkState);
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CStreamSink::EndGetEvent(IMFAsyncResult* pResult, IMFMediaEvent** ppEvent)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->EndGetEvent(pResult, ppEvent);
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CStreamSink::GetEvent(DWORD dwFlags, IMFMediaEvent** ppEvent)
{
    // NOTE:
    // GetEvent can block indefinitely, so we don't hold the lock.
    // This requires some juggling with the event queue pointer.

    HRESULT hr = S_OK;

    IMFMediaEventQueue *pQueue = NULL;

    {
        AutoLock lock(_critSec);

        // Check shutdown
        hr = CheckShutdown();

        // Get the pointer to the event queue.
        if (SUCCEEDED(hr))
        {
            pQueue = _spEventQueue.Get();
            pQueue->AddRef();
        }
    }

    // Now get the event.
    if (SUCCEEDED(hr))
    {
        hr = pQueue->GetEvent(dwFlags, ppEvent);
    }

    SafeRelease(&pQueue);

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CStreamSink::QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, PROPVARIANT const *pvValue)
{
    HRESULT hr = S_OK;

    AutoLock lock(_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->QueueEventParamVar(met, guidExtendedType, hrStatus, pvValue);
    }

    TRACEHR_RET(hr);
}

/// IMFStreamSink methods

IFACEMETHODIMP CStreamSink::GetMediaSink(IMFMediaSink **ppMediaSink)
{
    if (ppMediaSink == NULL)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        _spSink.Get()->QueryInterface(IID_IMFMediaSink, (void**)ppMediaSink);
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CStreamSink::GetIdentifier(DWORD *pdwIdentifier)
{
    if (pdwIdentifier == NULL)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        *pdwIdentifier = _dwIdentifier;
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CStreamSink::GetMediaTypeHandler(IMFMediaTypeHandler **ppHandler)
{
    if (ppHandler == NULL)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    // This stream object acts as its own type handler, so we QI ourselves.
    if (SUCCEEDED(hr))
    {
        hr = QueryInterface(IID_IMFMediaTypeHandler, (void**)ppHandler);
    }

    TRACEHR_RET(hr);
}

// We received a sample from an upstream component
IFACEMETHODIMP CStreamSink::ProcessSample(IMFSample *pSample)
{
    if (pSample == NULL)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;

    AutoLock lock(_critSec);

    hr = CheckShutdown();

    // Validate the operation.
    if (SUCCEEDED(hr))
    {
        hr = ValidateOperation(OpProcessSample);
    }

    if (SUCCEEDED(hr) && _fWaitingForFirstSample && !_Connected)
    {
        _spFirstVideoSample = pSample;
        _fWaitingForFirstSample = false;

        hr = QueueEvent(MEStreamSinkRequestSample, GUID_NULL, hr, NULL);
    }
    else if (SUCCEEDED(hr))
    {
        // Add the sample to the sample queue.
        if (SUCCEEDED(hr))
        {
            hr = _SampleQueue.InsertBack(pSample);
        }

        // Unless we are paused, start an async operation to dispatch the next sample.
        if (SUCCEEDED(hr))
        {
            if (_state != State_Paused)
            {
                // Queue the operation.
                hr = QueueAsyncOperation(OpProcessSample);
            }
        }
    }

    TRACEHR_RET(hr);
}

// The client can call PlaceMarker at any time. In response,
// we need to queue an MEStreamSinkMarker event, but not until
// *after* we have processed all samples that we have received
// up to this point.
//
// Also, in general you might need to handle specific marker
// types, although this sink does not.

IFACEMETHODIMP CStreamSink::PlaceMarker(
    MFSTREAMSINK_MARKER_TYPE eMarkerType,
    const PROPVARIANT *pvarMarkerValue,
    const PROPVARIANT *pvarContextValue)
{
    AutoLock lock(_critSec);

    HRESULT hr = S_OK;
    ComPtr<IMarker> spMarker;

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = ValidateOperation(OpPlaceMarker);
    }

    if (SUCCEEDED(hr))
    {
        hr = CMarker::Create(eMarkerType, pvarMarkerValue, pvarContextValue, &spMarker);
    }

    if (SUCCEEDED(hr))
    {
        hr = _SampleQueue.InsertBack(spMarker.Get());
    }

    // Unless we are paused, start an async operation to dispatch the next sample/marker.
    if (SUCCEEDED(hr))
    {
        if (_state != State_Paused)
        {
            // Queue the operation.
            hr = QueueAsyncOperation(OpPlaceMarker); // Increments ref count on pOp.
        }
    }

    TRACEHR_RET(hr);
}

// Discards all samples that were not processed yet.
IFACEMETHODIMP CStreamSink::Flush()
{
    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        // Note: Even though we are flushing data, we still need to send
        // any marker events that were queued.
        hr = DropSamplesFromQueue();
    }

    TRACEHR_RET(hr);
}


/// IMFMediaTypeHandler methods

// Check if a media type is supported.
IFACEMETHODIMP CStreamSink::IsMediaTypeSupported(
    /* [in] */ IMFMediaType *pMediaType,
    /* [out] */ IMFMediaType **ppMediaType)
{
    if (pMediaType == nullptr)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);

    GUID majorType = GUID_NULL;
    UINT cbSize = 0;

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = pMediaType->GetGUID(MF_MT_MAJOR_TYPE, &majorType);
    }

    // First make sure it's video or audio type.
    if (SUCCEEDED(hr))
    {
        if (majorType != MFMediaType_Video && majorType != MFMediaType_Audio)
        {
            hr = MF_E_INVALIDTYPE;
        }
    }

    if (SUCCEEDED(hr) && _spCurrentType != nullptr)
    {
        GUID guiNewSubtype;
        if (FAILED(pMediaType->GetGUID(MF_MT_SUBTYPE, &guiNewSubtype)) || 
            guiNewSubtype != _guiCurrentSubtype)
        {
            hr = MF_E_INVALIDTYPE;
        }
    }

    // We don't return any "close match" types.
    if (ppMediaType)
    {
        *ppMediaType = nullptr;
    }

    TRACEHR_RET(hr);
}


// Return the number of preferred media types.
IFACEMETHODIMP CStreamSink::GetMediaTypeCount(DWORD *pdwTypeCount)
{
    if (pdwTypeCount == nullptr)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        // We've got only one media type
        *pdwTypeCount = 1;
    }

    TRACEHR_RET(hr);
}


// Return a preferred media type by index.
IFACEMETHODIMP CStreamSink::GetMediaTypeByIndex(
    /* [in] */ DWORD dwIndex,
    /* [out] */ IMFMediaType **ppType)
{
    if (ppType == NULL)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if ( dwIndex > 0 )
    {
        hr = MF_E_NO_MORE_TYPES;
    }
    else
    {
        *ppType = _spCurrentType.Get();
        if (*ppType != nullptr)
        {
            (*ppType)->AddRef();
        }
    }

    TRACEHR_RET(hr);
}


// Set the current media type.
IFACEMETHODIMP CStreamSink::SetCurrentMediaType(IMFMediaType *pMediaType)
{
    if (pMediaType == NULL)
    {
        return E_INVALIDARG;
    }
    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    // We don't allow format changes after streaming starts.
    if (SUCCEEDED(hr))
    {
        hr = ValidateOperation(OpSetMediaType);
    }

    // We set media type already
    if (_state >= State_Ready)
    {
        if (SUCCEEDED(hr))
        {
            hr = IsMediaTypeSupported(pMediaType, NULL);
        }
    }

    if (SUCCEEDED(hr))
    {
        GUID guiMajorType;
        pMediaType->GetMajorType(&guiMajorType);
        _fIsVideo = (guiMajorType == MFMediaType_Video);

        _spCurrentType.ReleaseAndGetAddressOf();
        hr = MFCreateMediaType(&_spCurrentType);
        if (SUCCEEDED(hr))
        {
            hr = pMediaType->CopyAllItems(_spCurrentType.Get());
        }
        if (SUCCEEDED(hr))
        {
            hr = _spCurrentType->GetGUID(MF_MT_SUBTYPE, &_guiCurrentSubtype);
        }
        if (SUCCEEDED(hr))
        {
            _state = State_Ready;
        }
    }

    TRACEHR_RET(hr);
}

// Return the current media type, if any.
IFACEMETHODIMP CStreamSink::GetCurrentMediaType(IMFMediaType **ppMediaType)
{
    if (ppMediaType == NULL)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_spCurrentType == nullptr)
        {
            hr = MF_E_NOT_INITIALIZED;
        }
    }

    if (SUCCEEDED(hr))
    {
        *ppMediaType = _spCurrentType.Get();
        (*ppMediaType)->AddRef();
    }

    TRACEHR_RET(hr);
}


// Return the major type GUID.
IFACEMETHODIMP CStreamSink::GetMajorType(GUID *pguidMajorType)
{
    if (pguidMajorType == nullptr)
    {
        return E_INVALIDARG;
    }

    if (!_spCurrentType)
    {
        return MF_E_NOT_INITIALIZED;
    }
    
    *pguidMajorType = (_fIsVideo) ? MFMediaType_Video : MFMediaType_Audio;

    return S_OK;
}


// private methods
HRESULT CStreamSink::Initialize(CMediaSink *pParent, INetworkChannel *pNetworkSender)
{
    assert(pParent != NULL);

    HRESULT hr = S_OK;

    // Create the event queue helper.
    hr = MFCreateEventQueue(&_spEventQueue);

    // Allocate a new work queue for async operations.
    if (SUCCEEDED(hr))
    {
        hr = MFAllocateSerialWorkQueue(MFASYNC_CALLBACK_QUEUE_STANDARD, &_WorkQueueId);
    }

    if (SUCCEEDED(hr))
    {
        _spSink = pParent;
        _pParent = pParent;
        _spNetworkSender = pNetworkSender;
    }

    TRACEHR_RET(hr);
}


// Called when the presentation clock starts.
HRESULT CStreamSink::Start(MFTIME start)
{
    AutoLock lock(_critSec);

    HRESULT hr = S_OK;

    hr = ValidateOperation(OpStart);

    if (SUCCEEDED(hr))
    {
        if (start != PRESENTATION_CURRENT_POSITION)
        {
            _StartTime = start;        // Cache the start time.
            _fGetStartTimeFromSample = false;
        }
        else
        {
            _fGetStartTimeFromSample = true;
        }
        _state = State_Started;
        _fWaitingForFirstSample = _fIsVideo;
        hr = QueueAsyncOperation(OpStart);
    }

    TRACEHR_RET(hr);
}

// Called when the presentation clock stops.
HRESULT CStreamSink::Stop()
{
    AutoLock lock(_critSec);

    HRESULT hr = S_OK;

    hr = ValidateOperation(OpStop);

    if (SUCCEEDED(hr))
    {
        _state = State_Stopped;
        hr = QueueAsyncOperation(OpStop);
    }

    TRACEHR_RET(hr);
}

// Called when the presentation clock pauses.
HRESULT CStreamSink::Pause()
{
    AutoLock lock(_critSec);

    HRESULT hr = S_OK;

    hr = ValidateOperation(OpPause);

    if (SUCCEEDED(hr))
    {
        _state = State_Paused;
        hr = QueueAsyncOperation(OpPause);
    }

    TRACEHR_RET(hr);
}

// Called when the presentation clock restarts.
HRESULT CStreamSink::Restart()
{
    AutoLock lock(_critSec);

    HRESULT hr = S_OK;

    hr = ValidateOperation(OpRestart);

    if (SUCCEEDED(hr))
    {
        _state = State_Started;
        hr = QueueAsyncOperation(OpRestart);
    }

    TRACEHR_RET(hr);
}

// Class-static matrix of operations vs states.
// If an entry is TRUE, the operation is valid from that state.
BOOL CStreamSink::ValidStateMatrix[CStreamSink::State_Count][CStreamSink::Op_Count] =
{
// States:    Operations:
//            SetType   Start     Restart   Pause     Stop      Sample    Marker   
/* NotSet */  TRUE,     FALSE,    FALSE,    FALSE,    FALSE,    FALSE,    FALSE,   

/* Ready */   TRUE,     TRUE,     FALSE,    TRUE,     TRUE,     FALSE,    TRUE,    

/* Start */   FALSE,    TRUE,     FALSE,    TRUE,     TRUE,     TRUE,     TRUE,    

/* Pause */   FALSE,    TRUE,     TRUE,     TRUE,     TRUE,     TRUE,     TRUE,    

/* Stop */    FALSE,    TRUE,     FALSE,    FALSE,    TRUE,     FALSE,    TRUE,    

};

// Checks if an operation is valid in the current state.
HRESULT CStreamSink::ValidateOperation(StreamOperation op)
{
    assert(!_IsShutdown);

    HRESULT hr = S_OK;

    if (ValidStateMatrix[_state][op])
    {
        return S_OK;
    }
    else if (_state == State_TypeNotSet)
    {
        return MF_E_NOT_INITIALIZED;
    }
    else
    {
        return MF_E_INVALIDREQUEST;
    }
}

// Shuts down the stream sink.
HRESULT CStreamSink::Shutdown()
{
    assert(!_IsShutdown);

    if (_spEventQueue)
    {
        _spEventQueue->Shutdown();
    }

    MFUnlockWorkQueue(_WorkQueueId);

    _SampleQueue.Clear();

    _spSink.ReleaseAndGetAddressOf();
    _spEventQueue.ReleaseAndGetAddressOf();
    _spByteStream.ReleaseAndGetAddressOf();
    _spCurrentType.ReleaseAndGetAddressOf();
    _spNetworkSender.ReleaseAndGetAddressOf();

    _IsShutdown = TRUE;

    return S_OK;
}


// Puts an async operation on the work queue.
HRESULT CStreamSink::QueueAsyncOperation(StreamOperation op)
{
    HRESULT hr = S_OK;
    ComPtr<CAsyncOperation> spOp;
    spOp.Attach(new CAsyncOperation(op)); // Created with ref count = 1
    if (!spOp)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = MFPutWorkItem2(_WorkQueueId, 0, &_WorkQueueCB, spOp.Get());
    }

    TRACEHR_RET(hr);
}

HRESULT CStreamSink::OnDispatchWorkItem(IMFAsyncResult* pAsyncResult)
{
    // Called by work queue thread. Need to hold the critical section.
    AutoLock lock(_critSec);

    HRESULT hr = S_OK;

    ComPtr<IUnknown> spState;

    hr = pAsyncResult->GetState(&spState);

    if (SUCCEEDED(hr))
    {
        // The state object is a CAsncOperation object.
        CAsyncOperation *pOp = static_cast<CAsyncOperation *>(spState.Get());
        StreamOperation op = pOp->m_op;

        switch (op)
        {
        case OpStart:
        case OpRestart:
            // Send MEStreamSinkStarted.
            hr = QueueEvent(MEStreamSinkStarted, GUID_NULL, hr, NULL);

            // There might be samples queue from earlier (ie, while paused).
            if (SUCCEEDED(hr))
            {
                if (!_Connected)
                {
                    // Just drop samples if we are not connected
                    hr = DropSamplesFromQueue();
                }
                else
                {
                    hr = SendSampleFromQueue();
                }
                if (hr == S_FALSE)
                {
                    // If false there is no samples in the queue now so request one
                    hr = QueueEvent(MEStreamSinkRequestSample, GUID_NULL, hr, NULL);
                }
            }
            break;

        case OpStop:
            // Drop samples from queue.
            hr = DropSamplesFromQueue();

            // Send the event even if the previous call failed.
            hr = QueueEvent(MEStreamSinkStopped, GUID_NULL, hr, NULL);
            break;

        case OpPause:
            hr = QueueEvent(MEStreamSinkPaused, GUID_NULL, hr, NULL);
            break;

        case OpProcessSample:
        case OpPlaceMarker:
            hr = DispatchProcessSample(pOp);
            break;
        }
    }

    TRACEHR_RET(hr);
}

// Complete a ProcessSample or PlaceMarker request.
HRESULT CStreamSink::DispatchProcessSample(CAsyncOperation* pOp)
{
    HRESULT hr = S_OK;
    assert(pOp != NULL);

    if (!_Connected)
    {
        hr = DropSamplesFromQueue();
    }
    else
    {
        hr = SendSampleFromQueue();
    }

    // Ask for another sample
    if (hr == S_FALSE)
    {
        if (pOp->m_op == OpProcessSample)
        {
            hr = QueueEvent(MEStreamSinkRequestSample, GUID_NULL, S_OK, NULL);
        }
    }

    // We are in the middle of an asynchronous operation, so if something failed, send an error.
    if (FAILED(hr))
    {
        hr = QueueEvent(MEError, GUID_NULL, hr, NULL);
    }

    TRACEHR_RET(hr);
}

// Drop samples in the queue
HRESULT CStreamSink::DropSamplesFromQueue()
{
    ProcessSamplesFromQueue(TRUE);

    return S_FALSE;
}

// Send sample from the queue
HRESULT CStreamSink::SendSampleFromQueue()
{
    return ProcessSamplesFromQueue(FALSE);
}

HRESULT CStreamSink::ProcessSamplesFromQueue(BOOL fFlush)
{
    HRESULT hr = S_OK;

    ComPtr<IUnknown> spunkSample;

    hr = _SampleQueue.RemoveFront(&spunkSample);
    bool fSendSamples = true;
    bool fSendEOS = false;
    
    if (FAILED(hr))
    {
        hr = S_FALSE;
        fSendSamples = false;
    }

    while (SUCCEEDED(hr) && fSendSamples)
    {
        ComPtr<IMFSample> spSample;
        ComPtr<IBufferPacket> spPacket;
        assert(spunkSample); 

        // Figure out if this is a marker or a sample.
        // If this is a sample, write it to the file.
        hr = spunkSample.As(&spSample);

        // Now handle the sample/marker appropriately.
        if (SUCCEEDED(hr))
        {
            assert(spSample);    // Not a marker, must be a sample

            if (!fFlush)
            {
                // Prepare sample for sending
                hr = PrepareSample(spSample.Get(), false, &spPacket);
            }

            if (hr == S_FALSE || fFlush)
            {
                spunkSample.ReleaseAndGetAddressOf();
                hr = _SampleQueue.RemoveFront(&spunkSample);
                if (FAILED(hr))
                {
                    hr = S_FALSE;
                    fSendSamples = false;
                }
            }
        }
        else
        {
            ComPtr<IMarker> spMarker;
            hr = spunkSample.As(&spMarker);
            if (SUCCEEDED(hr))
            {                
                MFSTREAMSINK_MARKER_TYPE markerType;
                PROPVARIANT var;
                ZeroMemory(&var, sizeof(var));
                hr = spMarker->GetMarkerType(&markerType);
                // Get the context data.
                if (SUCCEEDED(hr))
                {
                    hr = spMarker->GetContext(&var);
                }

                if (SUCCEEDED(hr))
                {
                    hr = QueueEvent(MEStreamSinkMarker, GUID_NULL, S_OK, &var);
                }

                if (SUCCEEDED(hr))
                {
                    spunkSample.ReleaseAndGetAddressOf();
                    hr = _SampleQueue.RemoveFront(&spunkSample);
                    // No more samples in the queue.
                    if (FAILED(hr))
                    {
                        hr = S_FALSE;
                        fSendSamples = false;
                    }

                    if (markerType == MFSTREAMSINK_MARKER_ENDOFSEGMENT)
                    {
                        fSendEOS = true;
                    }
                }
            }
        }

        if (SUCCEEDED(hr) && spPacket)
        {
            // Send the sample
            hr = _spNetworkSender->BeginSend(spPacket.Get(), &_OnSampleSentCB, nullptr);
            fSendSamples = false;
        }
    }     

    if (fSendEOS)
    {
        _pParent->ReportEndOfStream();
    }

    TRACEHR_RET(hr);
}

// Set the information if we are connected to a client or not
HRESULT CStreamSink::SetConnected(bool fConnected, LONGLONG llCurrentTime)
{
    AutoLock lock(_critSec);
    HRESULT hr = S_OK;

    if (_spCurrentType == nullptr)
    {
        return MF_E_NOT_INITIALIZED;
    }

    _StartTime = llCurrentTime;
    _Connected = fConnected;
    if (fConnected)
    {
        if (_spFirstVideoSample)
        {
            ComPtr<IBufferPacket> spPacket;
            hr = PrepareSample(_spFirstVideoSample.Get(), true, &spPacket);

            if (SUCCEEDED(hr) && spPacket)
            {
                // Send the sample
                hr = _spNetworkSender->BeginSend(spPacket.Get(), &_OnFirstVideoSampleSentCB, nullptr);
            }
            _spFirstVideoSample.ReleaseAndGetAddressOf();
        }


        TRACE(TRACE_LEVEL_LOW, L"SetConnected start=%I64d\n", _StartTime);

        _fFirstSampleAfterConnect = true;
    }

    return hr;
}

// First video sample has been sent
HRESULT CStreamSink::OnFirstVideoSampleSent(IMFAsyncResult* pAsyncResult)
{
    AutoLock lock(_critSec);

    HRESULT hr = _spNetworkSender->EndSend(pAsyncResult);
    
    if (FAILED(hr))
    {
        hr = QueueEvent(MEError, GUID_NULL, hr, NULL);
    }

    TRACEHR_RET(hr);
}

// Sample has been sent
HRESULT CStreamSink::OnSampleSent(IMFAsyncResult* pAsyncResult)
{
    AutoLock lock(_critSec);

    HRESULT hr = _spNetworkSender->EndSend(pAsyncResult);
    
    if (_state == State_Started)
    {
        // If we are still in started state request another sample
        hr = QueueEvent(MEStreamSinkRequestSample, GUID_NULL, hr, NULL);
    }

    TRACEHR_RET(hr);
}

// Prepare sample for sending over the network by serializing it to a network packet
HRESULT CStreamSink::PrepareSample(IMFSample *pSample, bool fForce, IBufferPacket **ppPacket)
{
    assert(pSample);
    assert(ppPacket);

    HRESULT hr = S_OK;
    ComPtr<IMediaBufferWrapper> spBuffer;
    ComPtr<IBufferPacket> spPacket;
    const size_t c_cbHeaderSize = sizeof(StspOperationHeader) + sizeof(StspSampleHeader);
    DWORD cbTotalSampleLength = 0;
    LONGLONG llSampleTime;

    hr = pSample->GetSampleTime(&llSampleTime);
    llSampleTime -= _StartTime;    

    if (SUCCEEDED(hr) && llSampleTime < 0 && !fForce)
    {
        return S_FALSE;
    }

    if (llSampleTime < 0)// && !IsVideo())
    {
        llSampleTime = 0;
    }

    if (SUCCEEDED(hr))
    {
        // Create packet and initialize it with current sample
        hr = CreateBufferPacketFromMFSample(pSample, &spPacket);
    }

    if (SUCCEEDED(hr))
    {
        hr = pSample->GetTotalLength(&cbTotalSampleLength);
    }

    if (SUCCEEDED(hr))
    {
        hr = CreateMediaBufferWrapper(c_cbHeaderSize, &spBuffer);
    }

    if (SUCCEEDED(hr))
    {
        // Prepare the headers
        BYTE *pBuf = spBuffer->GetBuffer();
        // Operation header
        StspOperationHeader *pOpHeader = reinterpret_cast<StspOperationHeader *>(pBuf);
        pOpHeader->eOperation = StspOperation_ServerSample;
        pOpHeader->cbDataSize = sizeof(StspSampleHeader) + cbTotalSampleLength;

        // Sample header
        StspSampleHeader *pSampleHeader = reinterpret_cast<StspSampleHeader *>(pBuf + sizeof(StspOperationHeader));
        ZeroMemory(pSampleHeader, sizeof(*pSampleHeader));
        GetIdentifier(&pSampleHeader->dwStreamId);
        if (_fGetStartTimeFromSample)
        {
            pSample->GetSampleTime(&_StartTime);
            _fGetStartTimeFromSample = false;
            llSampleTime = 0;
        }
        pSampleHeader->ullTimestamp = llSampleTime;
        if (FAILED(pSample->GetSampleDuration(&pSampleHeader->ullDuration)))
        {
            pSampleHeader->ullDuration = 0;
        }

        // Set up video samples
        if (IsVideo())
        {
            SET_SAMPLE_FLAG(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, BottomFieldFirst);
            SET_SAMPLE_FLAG(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, CleanPoint);
            SET_SAMPLE_FLAG(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, DerivedFromTopField);
            SET_SAMPLE_FLAG(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, Discontinuity);
            SET_SAMPLE_FLAG(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, Interlaced);
            SET_SAMPLE_FLAG(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, RepeatFirstField);
            SET_SAMPLE_FLAG(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, SingleField);
        }

        hr = spBuffer->SetCurrentLength(c_cbHeaderSize);

        if (SUCCEEDED(hr))
        {
            // Put headers before sample
            hr = spPacket->InsertBuffer(0, spBuffer.Get());
        }

        _fFirstSampleAfterConnect = false;
    }

    if (SUCCEEDED(hr))
    {
        *ppPacket = spPacket.Detach();
    }

    
    TRACEHR_RET(hr);
}

CStreamSink::CAsyncOperation::CAsyncOperation(StreamOperation op)
    : _cRef(1)
    , m_op(op)
{
}

CStreamSink::CAsyncOperation::~CAsyncOperation()
{
    assert(_cRef == 0);
}

ULONG CStreamSink::CAsyncOperation::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

ULONG CStreamSink::CAsyncOperation::Release()
{
    ULONG cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }

    return cRef; 
}

HRESULT CStreamSink::CAsyncOperation::QueryInterface(REFIID iid, void** ppv)
{
    if (!ppv)
    {
        return E_POINTER;
    }
    if (iid == IID_IUnknown)
    {
        *ppv = static_cast<IUnknown*>(this);
    }
    else
    {
        *ppv = NULL;
        return E_NOINTERFACE;
    }
    AddRef();
    return S_OK;
}

HRESULT CStreamSink::AppendParameterSets(IMFSample *pCopyFrom, IBufferPacket *pPacket)
{
    static const BYTE rgbStartCode[] = {0x00, 0x00, 0x00, 0x01};
    static const size_t cbStartCode = _countof(rgbStartCode);

    ComPtr<IMediaBufferWrapper> spStartCodeBuffer;
    ComPtr<IMFMediaBuffer> spMediaBuffer;
    ComPtr<IMediaBufferWrapper> spParameterSetsBuffer;
    DWORD dwMediaBufferLen = 0;

    HRESULT hr = CreateMediaBufferWrapper(cbStartCode, &spStartCodeBuffer);

    if (SUCCEEDED(hr))
    {
        CopyMemory(spStartCodeBuffer->GetBuffer(), rgbStartCode, cbStartCode);
        hr = pPacket->InsertBuffer(0, spStartCodeBuffer.Get());
    }

    if (SUCCEEDED(hr))
    {
        hr = pCopyFrom->ConvertToContiguousBuffer(&spMediaBuffer);
        if (SUCCEEDED(hr))
        {
            spMediaBuffer->GetCurrentLength(&dwMediaBufferLen);
            if (dwMediaBufferLen < sizeof(DWORD))
            {
                hr = E_UNEXPECTED;
            }
        }
        
    }

    if (SUCCEEDED(hr))
    {
        hr = CreateMediaBufferWrapper(spMediaBuffer.Get(), &spParameterSetsBuffer);
    }

    if (SUCCEEDED(hr))
    {
        DWORD dwParameterSetLen;
        CopyMemory(&dwParameterSetLen, spParameterSetsBuffer->GetBuffer(), sizeof(DWORD));

        if (dwMediaBufferLen < dwParameterSetLen)
        {
            hr = E_UNEXPECTED;
        }

        if (SUCCEEDED(hr))
        {
            hr = spParameterSetsBuffer->SetCurrentLength(dwParameterSetLen);
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = pPacket->InsertBuffer(0, spParameterSetsBuffer.Get());
    }

    return hr;
}
