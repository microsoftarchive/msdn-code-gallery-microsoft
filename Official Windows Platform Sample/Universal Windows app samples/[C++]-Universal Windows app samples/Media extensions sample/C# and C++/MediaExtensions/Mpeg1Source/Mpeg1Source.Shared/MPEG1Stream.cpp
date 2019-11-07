//////////////////////////////////////////////////////////////////////////
//
// MPEG1Stream.cpp
// Implements the stream object (IMFMediaStream) for the MPEG-1 source.
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
#include "MPEG1Source.h"

#pragma warning( push )
#pragma warning( disable : 4355 )  // 'this' used in base member initializer list


/* CMPEG1Stream::SourceLock class methods */

//-------------------------------------------------------------------
// CMPEG1Stream::SourceLock constructor - locks the source
//-------------------------------------------------------------------

_Acquires_lock_(m_pSource)
CMPEG1Stream::SourceLock::SourceLock(CMPEG1Source *pSource)
    : m_pSource(nullptr)
{
    if (pSource)
    {
        m_pSource = pSource;
        m_pSource->AddRef();
        m_pSource->Lock();
    }
}

//-------------------------------------------------------------------
// CMPEG1Stream::SourceLock destructor - unlocks the source
//-------------------------------------------------------------------

_Releases_lock_(m_pSource)
CMPEG1Stream::SourceLock::~SourceLock()
{
    if (m_pSource)
    {
        m_pSource->Unlock();
        m_pSource->Release();
    }
}



/* Public class methods */

//-------------------------------------------------------------------
// IUnknown methods
//-------------------------------------------------------------------

ULONG CMPEG1Stream::AddRef()
{
    return _InterlockedIncrement(&m_cRef);
}

ULONG CMPEG1Stream::Release()
{
    LONG cRef = _InterlockedDecrement(&m_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

HRESULT CMPEG1Stream::QueryInterface(REFIID riid, void **ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }

    HRESULT hr = E_NOINTERFACE;
    if (riid == IID_IUnknown ||
        riid == IID_IMFMediaEventGenerator ||
        riid == IID_IMFMediaStream)
    {
        (*ppv) = static_cast<IMFMediaStream *>(this);
        AddRef();
        hr = S_OK;
    }

    return hr;
}


//-------------------------------------------------------------------
// IMFMediaEventGenerator methods
//
// For remarks, see MPEG1Source.cpp
//-------------------------------------------------------------------

HRESULT CMPEG1Stream::BeginGetEvent(IMFAsyncCallback *pCallback,IUnknown *punkState)
{
    HRESULT hr = S_OK;

    SourceLock lock(m_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = m_spEventQueue->BeginGetEvent(pCallback, punkState);
    }

    return hr;
}

HRESULT CMPEG1Stream::EndGetEvent(IMFAsyncResult *pResult, IMFMediaEvent **ppEvent)
{
    HRESULT hr = S_OK;

    SourceLock lock(m_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = m_spEventQueue->EndGetEvent(pResult, ppEvent);
    }

    return hr;
}

HRESULT CMPEG1Stream::GetEvent(DWORD dwFlags, IMFMediaEvent **ppEvent)
{
    HRESULT hr = S_OK;

    ComPtr<IMFMediaEventQueue> spQueue;

    { // scope for lock

        SourceLock lock(m_spSource.Get());

        // Check shutdown
        hr = CheckShutdown();

        // Cache a local pointer to the queue.
        if (SUCCEEDED(hr))
        {
            spQueue = m_spEventQueue.Get();
        }
    }   // release lock

    // Use the local pointer to call GetEvent.
    if (SUCCEEDED(hr))
    {
        hr = spQueue->GetEvent(dwFlags, ppEvent);
    }

    return hr;
}

HRESULT CMPEG1Stream::QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT *pvValue)
{
    HRESULT hr = S_OK;

    SourceLock lock(m_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = m_spEventQueue->QueueEventParamVar(met, guidExtendedType, hrStatus, pvValue);
    }

    return hr;
}

//-------------------------------------------------------------------
// IMFMediaStream methods
//-------------------------------------------------------------------


//-------------------------------------------------------------------
// GetMediaSource:
// Returns a pointer to the media source.
//-------------------------------------------------------------------

HRESULT CMPEG1Stream::GetMediaSource(IMFMediaSource **ppMediaSource)
{
    SourceLock lock(m_spSource.Get());

    if (ppMediaSource == nullptr)
    {
        return E_POINTER;
    }

    if (m_spSource == nullptr)
    {
        return E_UNEXPECTED;
    }

    HRESULT hr = S_OK;

    hr = CheckShutdown();

    // QI the source for IMFMediaSource.
    // (Does not hold the source's critical section.)
    if (SUCCEEDED(hr))
    {
        hr = m_spSource.CopyTo(IID_PPV_ARGS(ppMediaSource));
    }
    return hr;
}


//-------------------------------------------------------------------
// GetStreamDescriptor:
// Returns a pointer to the stream descriptor for this stream.
//-------------------------------------------------------------------

HRESULT CMPEG1Stream::GetStreamDescriptor(IMFStreamDescriptor **ppStreamDescriptor)
{
    SourceLock lock(m_spSource.Get());

    if (ppStreamDescriptor == nullptr)
    {
        return E_POINTER;
    }

    if (m_spStreamDescriptor == nullptr)
    {
        return E_UNEXPECTED;
    }

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        *ppStreamDescriptor = m_spStreamDescriptor.Get();
        (*ppStreamDescriptor)->AddRef();

    };
    return hr;
}


//-------------------------------------------------------------------
// RequestSample:
// Requests data from the stream.
//
// pToken: Token used to track the request. Can be nullptr.
//-------------------------------------------------------------------

HRESULT CMPEG1Stream::RequestSample(IUnknown *pToken)
{
    HRESULT hr = S_OK;
    IMFMediaSource *pSource = nullptr;

    // Hold the media source object's critical section.
    SourceLock lock(m_spSource.Get());

    hr = CheckShutdown();
    if (FAILED(hr))
    {
        goto done;
    }

    if (m_state == STATE_STOPPED)
    {
        hr = MF_E_INVALIDREQUEST;
        goto done;
    }

    if (!m_fActive)
    {
        // If the stream is not active, it should not get sample requests.
        hr = MF_E_INVALIDREQUEST;
        goto done;
    }

    if (m_fEOS && m_Samples.IsEmpty())
    {
        // This stream has already reached the end of the stream, and the
        // sample queue is empty.
        hr = MF_E_END_OF_STREAM;
        goto done;
    }

    hr = m_Requests.InsertBack(pToken);
    if (FAILED(hr))
    {
        goto done;
    }

    // Dispatch the request.
    DispatchSamples();

done:
    if (FAILED(hr) && (m_state != STATE_SHUTDOWN))
    {
        // An error occurred. Send an MEError even from the source,
        // unless the source is already shut down.
        hr = m_spSource->QueueEvent(MEError, GUID_NULL, hr, nullptr);
    }
    return hr;
}



//-------------------------------------------------------------------
// Public non-interface methods
//-------------------------------------------------------------------


CMPEG1Stream::CMPEG1Stream(CMPEG1Source *pSource, IMFStreamDescriptor *pSD) :
    m_cRef(1),
    m_state(STATE_STOPPED),
    m_fActive(false),
    m_fEOS(false),
    m_flRate(1.0f),
    m_spSource(pSource),
    m_spStreamDescriptor(pSD)
{
    auto module = ::Microsoft::WRL::GetModuleBase();
    if (module != nullptr)
    {
        module->IncrementObjectCount();
    }

	assert(pSource != nullptr);
    assert(pSD != nullptr);
}

CMPEG1Stream::~CMPEG1Stream()
{
    assert(m_state == STATE_SHUTDOWN);

    auto module = ::Microsoft::WRL::GetModuleBase();
    if (module != nullptr)
    {
        module->DecrementObjectCount();
    }
}

void CMPEG1Stream::Initialize()
{
    // Create the media event queue.
    ThrowIfError(MFCreateEventQueue(&m_spEventQueue));
}

//-------------------------------------------------------------------
// Activate
// Activates or deactivates the stream. Called by the media source.
//-------------------------------------------------------------------

void CMPEG1Stream::Activate(bool fActive)
{
    if (fActive == m_fActive)
    {
        return;
    }

    m_fActive = fActive;

    if (!fActive)
    {
        m_Samples.Clear();
        m_Requests.Clear();
    }
}


//-------------------------------------------------------------------
// Start
// Starts the stream. Called by the media source.
//
// varStart: Starting position.
//-------------------------------------------------------------------

void CMPEG1Stream::Start(const PROPVARIANT &varStart)
{

    ThrowIfError(CheckShutdown());

    // Queue the stream-started event.
    ThrowIfError(QueueEvent(MEStreamStarted, GUID_NULL, S_OK, &varStart));

    m_state = STATE_STARTED;

    // If we are restarting from paused, there may be
    // queue sample requests. Dispatch them now.
    DispatchSamples();
}


//-------------------------------------------------------------------
// Pause
// Pauses the stream. Called by the media source.
//-------------------------------------------------------------------

void CMPEG1Stream::Pause()
{
    ThrowIfError(CheckShutdown());

    m_state = STATE_PAUSED;

    ThrowIfError(QueueEvent(MEStreamPaused, GUID_NULL, S_OK, nullptr));
}


//-------------------------------------------------------------------
// Stop
// Stops the stream. Called by the media source.
//-------------------------------------------------------------------

void CMPEG1Stream::Stop()
{
    ThrowIfError(CheckShutdown());

    m_Requests.Clear();
    m_Samples.Clear();

    m_state = STATE_STOPPED;
       
    ThrowIfError(QueueEvent(MEStreamStopped, GUID_NULL, S_OK, nullptr));
}

//-------------------------------------------------------------------
// SetRate
// Sets rate of the stream. Called by the media source.
//-------------------------------------------------------------------

void CMPEG1Stream::SetRate(float flRate)
{
    ThrowIfError(CheckShutdown());

    m_flRate = flRate;
}

//-------------------------------------------------------------------
// EndOfStream
// Notifies the stream that the source reached the end of the MPEG-1
// stream. For more information, see CMPEG1Source::EndOfMPEGStream().
//-------------------------------------------------------------------

void CMPEG1Stream::EndOfStream()
{
    m_fEOS = true;

    DispatchSamples();
}


//-------------------------------------------------------------------
// Shutdown
// Shuts down the stream and releases all resources.
//-------------------------------------------------------------------

void CMPEG1Stream::Shutdown()
{
    if (SUCCEEDED(CheckShutdown()))
    {
        m_state = STATE_SHUTDOWN;

        // Shut down the event queue.
        if (m_spEventQueue)
        {
            m_spEventQueue->Shutdown();
        }

        // Release objects.
        m_Samples.Clear();
        m_Requests.Clear();

        m_spStreamDescriptor.Reset();
        m_spEventQueue.Reset();

        // NOTE:
        // Do NOT release the source pointer here, because the stream uses
        // it to hold the critical section. In particular, the stream must
        // hold the critical section when checking the shutdown status,
        // which obviously can occur after the stream is shut down.

        // It is OK to hold a ref count on the source after shutdown,
        // because the source releases its ref count(s) on the streams,
        // which breaks the circular ref count.
    }
}


//-------------------------------------------------------------------
// NeedsData
// Returns TRUE if the stream needs more data.
//-------------------------------------------------------------------

bool CMPEG1Stream::NeedsData()
{
    // Note: The stream tries to keep a minimum number of samples
    // queued ahead.

    return (m_fActive && !m_fEOS && (m_Samples.GetCount() < SAMPLE_QUEUE));
}


//-------------------------------------------------------------------
// DeliverPayload
// Delivers a sample to the stream.
//-------------------------------------------------------------------

void CMPEG1Stream::DeliverPayload(IMFSample *pSample)
{
    // Queue the sample.
    ThrowIfError(m_Samples.InsertBack(pSample));

    // Deliver the sample if there is an outstanding request.
    DispatchSamples();
}

/* Private methods */

//-------------------------------------------------------------------
// DispatchSamples
// Dispatches as many pending sample requests as possible.
//-------------------------------------------------------------------

void CMPEG1Stream::DispatchSamples() throw()
{
    // An I/O request can complete after the source is paused, stopped, or
    // shut down. Do not deliver samples unless the source is running.
    if (m_state != STATE_STARTED)
    {
        return;
    }

    try
    {
        // Deliver as many samples as we can.
        while (!m_Samples.IsEmpty() && !m_Requests.IsEmpty())
        {
            ComPtr<IMFSample> spSample;
            ComPtr<IUnknown> spToken;

            // Pull the next sample from the queue.
            ThrowIfError(m_Samples.RemoveFront(&spSample));

            // Pull the next request token from the queue. Tokens can be nullptr.
            ThrowIfError(m_Requests.RemoveFront(&spToken));

            if (spToken != nullptr)
            {
                // Set the token on the sample.
                ThrowIfError(spSample->SetUnknown(MFSampleExtension_Token, spToken.Get()));
            }

            // Send an MEMediaSample event with the sample.
            ThrowIfError(m_spEventQueue->QueueEventParamUnk(MEMediaSample, GUID_NULL, S_OK, spSample.Get()));
        }

        if (m_Samples.IsEmpty() && m_fEOS)
        {
            // The sample queue is empty AND we have reached the end of the source
            // stream. Notify the pipeline by sending the end-of-stream event.

            ThrowIfError(m_spEventQueue->QueueEventParamVar(
                MEEndOfStream, GUID_NULL, S_OK, nullptr));

            // Notify the source. It will send the end-of-presentation event.
            ThrowIfError(m_spSource->QueueAsyncOperation(SourceOp::OP_END_OF_STREAM));
        }
        else if (NeedsData())
        {
            // The sample queue is empty; the request queue is not empty; and we
            // have not reached the end of the stream. Ask for more data.
            ThrowIfError(m_spSource->QueueAsyncOperation(SourceOp::OP_REQUEST_DATA));
        }
    }
    catch (Exception ^exc)
    {
        if (m_state != STATE_SHUTDOWN)
        {
            // An error occurred. Send an MEError even from the source,
            // unless the source is already shut down.
            m_spSource->QueueEvent(MEError, GUID_NULL, exc->HResult, nullptr);
        }
    }
}

#pragma warning( pop )