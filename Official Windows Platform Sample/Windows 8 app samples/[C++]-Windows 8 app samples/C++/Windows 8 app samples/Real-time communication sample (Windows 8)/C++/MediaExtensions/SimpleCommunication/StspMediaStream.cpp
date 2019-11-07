//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "StdAfx.h"
#include "StspMediaStream.h"
#include "StspMediaSource.h"

using namespace Stsp;

#define SET_SAMPLE_ATTRIBUTE(flag, mask, pSample, flagName) \
    if (SUCCEEDED(hr) && (StspSampleFlag_##flagName & mask) == StspSampleFlag_##flagName) \
{ \
    hr = pSample->SetUINT32(MFSampleExtension_##flagName, (StspSampleFlag_##flagName & flag) == StspSampleFlag_##flagName); \
}

// RAII object locking the the source on initialization and unlocks it on deletion
class CMediaStream::CSourceLock
{
public:
    CSourceLock(CMediaSource *pSource)
        : _spSource(pSource)
    {
        if (_spSource)
        {
            _spSource->Lock();
        }
    }

    ~CSourceLock()
    {
        if (_spSource)
        {
            _spSource->Unlock();
        }
    }

private:
    ComPtr<CMediaSource> _spSource;
};

HRESULT CMediaStream::CreateInstance(StspStreamDescription *pStreamDescription, IBufferPacket *pAttributesBuffer, CMediaSource *pSource, CMediaStream **ppStream)
{
    if (pStreamDescription == nullptr || pSource == nullptr || ppStream == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;
    ComPtr<CMediaStream> spStream;
    spStream.Attach(new CMediaStream(pSource));
    if (!spStream)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = spStream->Initialize(pStreamDescription, pAttributesBuffer);
    }

    if (SUCCEEDED(hr))
    {
        (*ppStream) = spStream.Detach();
    }

    TRACEHR_RET(hr);
}

CMediaStream::CMediaStream(CMediaSource *pSource)
    : _cRef(1)
    , _spSource(pSource)
    , _eSourceState(SourceState_Invalid)
    , _fActive(false)
    , _flRate(1.0f)
    , _fVideo(false)
    , _eDropMode(MF_DROP_MODE_NONE)
    , _fDiscontinuity(false)
    , _fDropTime(false)
    , _fInitDropTime(false)
    , _fWaitingForCleanPoint(true)
    , _hnsStartDroppingAt(0)
    , _hnsAmountToDrop(0)
{
}

CMediaStream::~CMediaStream(void)
{
}

// IUnknown methods

IFACEMETHODIMP CMediaStream::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    (*ppv) = nullptr;

    HRESULT hr = S_OK;
    if (riid == IID_IUnknown || 
        riid == IID_IMFMediaStream ||
        riid == IID_IMFMediaEventGenerator) 
    {
        (*ppv) = static_cast<IMFMediaStream*>(this);
        AddRef();
    }
    else if (riid == IID_IMFQualityAdvise || riid == IID_IMFQualityAdvise2)
    {
        (*ppv) = static_cast<IMFQualityAdvise2*>(this);
        AddRef();
    }
    else if (riid == IID_IMFGetService)
    {
        (*ppv) = static_cast<IMFGetService*>(this);
        AddRef();
    }
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CMediaStream::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CMediaStream::Release()
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

IFACEMETHODIMP CMediaStream::BeginGetEvent(IMFAsyncCallback* pCallback, IUnknown* punkState)
{
    HRESULT hr = S_OK;

    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->BeginGetEvent(pCallback, punkState);
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaStream::EndGetEvent(IMFAsyncResult* pResult, IMFMediaEvent** ppEvent)
{
    HRESULT hr = S_OK;

    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->EndGetEvent(pResult, ppEvent);
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaStream::GetEvent(DWORD dwFlags, IMFMediaEvent** ppEvent)
{
    // NOTE:
    // GetEvent can block indefinitely, so we don't hold the lock.
    // This requires some juggling with the event queue pointer.

    HRESULT hr = S_OK;

    IMFMediaEventQueue *pQueue = NULL;

    {
        CSourceLock lock(_spSource.Get());

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

IFACEMETHODIMP CMediaStream::QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, PROPVARIANT const *pvValue)
{
    HRESULT hr = S_OK;

    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->QueueEventParamVar(met, guidExtendedType, hrStatus, pvValue);
    }

    TRACEHR_RET(hr);
}

// IMFMediaStream methods

IFACEMETHODIMP CMediaStream::GetMediaSource(IMFMediaSource** ppMediaSource)
{
    if (ppMediaSource == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;

    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        *ppMediaSource = _spSource.Get();
        (*ppMediaSource)->AddRef();
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaStream::GetStreamDescriptor(IMFStreamDescriptor** ppStreamDescriptor)
{
    if (ppStreamDescriptor == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;

    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        *ppStreamDescriptor = _spStreamDescriptor.Get();
        (*ppStreamDescriptor)->AddRef();
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaStream::RequestSample(IUnknown* pToken)
{
    HRESULT hr = S_OK;

    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr) && _eSourceState != SourceState_Started)
    {
        // We cannot be asked for a sample unless we are in started state
        hr = MF_E_INVALIDREQUEST;
    }

    if (SUCCEEDED(hr))
    {
        // Put token onto the list to return it when we have a sample ready
        hr = _tokens.InsertBack(pToken);
    }

    if (SUCCEEDED(hr) && _eSourceState == SourceState_Started)
    {
        // Trigger sample delivery
        hr = DeliverSamples();
    }

    if (FAILED(hr))
    {
        hr = HandleError(hr);
    }

    TRACEHR_RET(hr);
}

// IMFQualityAdvise methods

IFACEMETHODIMP CMediaStream::SetDropMode(
    _In_ MF_QUALITY_DROP_MODE eDropMode )
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    //
    // Only support one drop mode
    //
    if( SUCCEEDED(hr) &&
        ((eDropMode < MF_DROP_MODE_NONE) || (eDropMode >= MF_DROP_MODE_2)))
    {
        hr = MF_E_NO_MORE_DROP_MODES;
    }

    if (SUCCEEDED(hr) && _eDropMode != eDropMode)
    {
        _eDropMode = eDropMode;
        _fWaitingForCleanPoint = true;
        
        TRACE(TRACE_LEVEL_NORMAL, L"Setting drop mode to %d\n", _eDropMode);
    }

    return( hr );
}

IFACEMETHODIMP CMediaStream::SetQualityLevel(
    _In_ MF_QUALITY_LEVEL eQualityLevel )
{
    return( MF_E_NO_MORE_QUALITY_LEVELS );
}

IFACEMETHODIMP CMediaStream::GetDropMode(
    _Out_  MF_QUALITY_DROP_MODE *peDropMode )
{
    HRESULT hr = S_OK;

    if (peDropMode == nullptr)
    {
        return E_POINTER;
    }

    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        *peDropMode = _eDropMode;
    }

    TRACEHR_RET(hr);
}

IFACEMETHODIMP CMediaStream::GetQualityLevel(
    _Out_  MF_QUALITY_LEVEL *peQualityLevel )
{
    return( E_NOTIMPL );
}

IFACEMETHODIMP CMediaStream::DropTime( _In_ LONGLONG hnsAmountToDrop )
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (hnsAmountToDrop > 0)
        {
            _fDropTime = true;
            _fInitDropTime = true;
            _hnsAmountToDrop = hnsAmountToDrop;
            TRACE(TRACE_LEVEL_NORMAL, L"Dropping time hnsAmountToDrop=%I64d\n", hnsAmountToDrop);
        }
        else if (hnsAmountToDrop == 0)
        {
            // Reset time dropping
            TRACE(TRACE_LEVEL_NORMAL, L"Disabling dropping time\n");
            ResetDropTime();
        }
        else
        {
            hr = E_INVALIDARG;
        }
    }

    TRACEHR_RET(hr);
}

// IMFQualityAdvise2 methods

IFACEMETHODIMP CMediaStream::NotifyQualityEvent(_In_opt_ IMFMediaEvent *pEvent, _Out_ DWORD *pdwFlags)
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());
    
    if (pdwFlags == nullptr || pEvent == nullptr)
    {
        return E_POINTER;
    }

    *pdwFlags = 0;
    
    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        MediaEventType met;
        hr = pEvent->GetType(&met);

        if (SUCCEEDED(hr) && met == MEQualityNotify)
        {
            GUID guiExtendedType;
            hr = pEvent->GetExtendedType(&guiExtendedType);

            if (SUCCEEDED(hr) && guiExtendedType == MF_QUALITY_NOTIFY_SAMPLE_LAG)
            {
                PROPVARIANT var;
                PropVariantInit(&var);

                hr = pEvent->GetValue(&var);
                LONGLONG hnsSampleLatency = var.hVal.QuadPart;

                if (SUCCEEDED(hr))
                {
                    if (_eDropMode == MF_DROP_MODE_NONE && hnsSampleLatency > 30000000)
                    {
                        TRACE(TRACE_LEVEL_NORMAL, L"Entering drop mode\n");
                        hr = SetDropMode(MF_DROP_MODE_1);
                    }
                    else if (_eDropMode == MF_DROP_MODE_1 && hnsSampleLatency < 0)
                    {
                        TRACE(TRACE_LEVEL_NORMAL, L": Leaving drop mode\n");
                        hr = SetDropMode(MF_DROP_MODE_NONE);
                    }
                    else
                    {
                        TRACE(TRACE_LEVEL_NORMAL, L": Sample latency = %I64d\n", hnsSampleLatency);
                    }
                }

                PropVariantClear(&var);
            }
        }
    }

    TRACEHR_RET(hr);
}

// IMFGetService methods

IFACEMETHODIMP CMediaStream::GetService(
    _In_ REFGUID guidService,
    _In_ REFIID riid,
    _Out_ LPVOID *ppvObject )
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());

    if (ppvObject == nullptr)
    {
        return E_POINTER;
    }
    *ppvObject = NULL;

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if( MF_QUALITY_SERVICES == guidService )
        {
            hr = QueryInterface(riid, ppvObject);
        }
        else
        {
            hr = MF_E_UNSUPPORTED_SERVICE;
        }
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaStream::Start()
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_eSourceState == SourceState_Stopped ||
            _eSourceState == SourceState_Started)
        {
            _eSourceState = SourceState_Started;
            // Inform the client that we've started
            hr = QueueEvent(MEStreamStarted, GUID_NULL, S_OK, nullptr);
        }
        else
        {
            hr = MF_E_INVALID_STATE_TRANSITION;    
        }
    }

    if (FAILED(hr))
    {
        hr = HandleError(hr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaStream::Stop()
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_eSourceState == SourceState_Started)
        {
            _eSourceState = SourceState_Stopped;
            _tokens.Clear();
            _samples.Clear();
            // Inform the client that we've stopped.
            hr = QueueEvent(MEStreamStopped, GUID_NULL, S_OK, nullptr);
        }
        else
        {
            hr = MF_E_INVALID_STATE_TRANSITION;    
        }
    }

    if (FAILED(hr))
    {
        hr = HandleError(hr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaStream::SetRate(float flRate)
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        _flRate = flRate;
        if (_flRate != 1.0f)
        {
            hr = CleanSampleQueue();
        }
    }

    if (FAILED(hr))
    {
        hr = HandleError(hr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaStream::Flush()
{
    CSourceLock lock(_spSource.Get());

    _tokens.Clear();
    _samples.Clear();

    _fDiscontinuity = false;
    _eDropMode = MF_DROP_MODE_NONE;
    ResetDropTime();

    return S_OK;
}

HRESULT CMediaStream::Shutdown()
{
    CSourceLock lock(_spSource.Get());

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        Flush();

        if (_spEventQueue)
        {
            _spEventQueue->Shutdown();
            _spEventQueue.ReleaseAndGetAddressOf();
        }

        _spStreamDescriptor.ReleaseAndGetAddressOf();
        _eSourceState = SourceState_Shutdown;
    }

    TRACEHR_RET(hr);
}

// Processes media sampe received from the header
HRESULT CMediaStream::ProcessSample(StspSampleHeader *pSampleHeader, IMFSample *pSample)
{
    assert(pSample);
    CSourceLock lock(_spSource.Get());

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        // Set sample attributes
        hr = SetSampleAttributes(pSampleHeader, pSample);
    }

    if (SUCCEEDED(hr))
    {
        // Check if we are in propper state if so deliver the sample otherwise just skip it and don't treat it as an error.
        if (_eSourceState == SourceState_Started)
        {
            // Put sample on the list
            hr = _samples.InsertBack(pSample);
            if (SUCCEEDED(hr))
            {
                // Deliver samples
                hr = DeliverSamples();
            }
        }
        else
        {
            hr = MF_E_UNEXPECTED;
        }
    }

    if (FAILED(hr))
    {
        hr = HandleError(hr);
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaStream::SetActive(bool fActive)
{
    CSourceLock lock(_spSource.Get());

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_eSourceState != SourceState_Stopped &&
            _eSourceState != SourceState_Started)
        {
            hr = MF_E_INVALIDREQUEST;
        }
    }

    if (SUCCEEDED(hr))
    {
        _fActive = fActive;
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaStream::Initialize(StspStreamDescription *pStreamDescription, IBufferPacket *pAttributesBuffer)
{
    // Create the media event queue.
    HRESULT hr = MFCreateEventQueue(&_spEventQueue);
    ComPtr<IMFMediaType> spMediaType;
    ComPtr<IMFStreamDescriptor> spSD;
    ComPtr<IMFMediaTypeHandler> spMediaTypeHandler;
    BYTE *pAttributes = nullptr;
    DWORD cbAttributesSize = 0;

    _fVideo = (pStreamDescription->guiMajorType == MFMediaType_Video);

    if (SUCCEEDED(hr))
    {
        // Create a media type object.
        hr = MFCreateMediaType(&spMediaType);
    }

    if (SUCCEEDED(hr))
    {
        hr = pAttributesBuffer->GetTotalLength(&cbAttributesSize);
    }

    if (SUCCEEDED(hr) && (cbAttributesSize < pStreamDescription->cbAttributesSize || pStreamDescription->cbAttributesSize == 0))
    {
        // Invalid stream description
        hr = MF_E_UNSUPPORTED_FORMAT;
    }

    if (SUCCEEDED(hr))
    {
        // Prepare buffer where we will copy attributes to
        pAttributes = new BYTE[pStreamDescription->cbAttributesSize];
        if (pAttributes == nullptr)
        {
            hr = E_OUTOFMEMORY;
        }
    }

    if (SUCCEEDED(hr))
    {
        // Move the memory
        hr = pAttributesBuffer->MoveLeft(pStreamDescription->cbAttributesSize, pAttributes);
    }

    if (SUCCEEDED(hr))
    {
        // Initialize media type's attributes
        hr = MFInitAttributesFromBlob(spMediaType.Get(), pAttributes, pStreamDescription->cbAttributesSize);
    }

    if (SUCCEEDED(hr))
    {
        hr = ValidateInputMediaType(pStreamDescription->guiMajorType, pStreamDescription->guiSubType, spMediaType.Get());
    }

    if (SUCCEEDED(hr))
    {
        hr = spMediaType->SetGUID(MF_MT_MAJOR_TYPE, pStreamDescription->guiMajorType);
    }

    if (SUCCEEDED(hr))
    {
        hr = spMediaType->SetGUID(MF_MT_SUBTYPE, pStreamDescription->guiSubType);
    }

    if (SUCCEEDED(hr))
    {
        // Now we can create MF stream descriptor.
        hr = MFCreateStreamDescriptor(pStreamDescription->dwStreamId, 1, spMediaType.GetAddressOf(), &spSD);
    }

    if (SUCCEEDED(hr))
    {
        hr = spSD->GetMediaTypeHandler(&spMediaTypeHandler);
    }

    if (SUCCEEDED(hr))
    {
        // Set current media type
        hr = spMediaTypeHandler->SetCurrentMediaType(spMediaType.Get());
    }

    if (SUCCEEDED(hr))
    {
        _spStreamDescriptor = spSD;
        _dwId = pStreamDescription->dwStreamId;
        // State of the stream is started.
        _eSourceState = SourceState_Stopped;
    }

    if (pAttributes != nullptr) 
    {
        delete[] pAttributes;
    }

    TRACEHR_RET(hr);
}

// Deliver samples for every request client made
HRESULT CMediaStream::DeliverSamples()
{
    HRESULT hr = S_OK;

    // Check if we have both: samples available in the queue and requests in request list.
    while (!_samples.IsEmpty() && !_tokens.IsEmpty())
    {
        ComPtr<IMFSample> spSample;
        ComPtr<IUnknown> spToken;
        BOOL fDrop = FALSE;
        // Get the sample
        hr = _samples.RemoveFront(&spSample);
        if (FAILED(hr))
        {
            break;
        }

        hr = ShouldDropSample(spSample.Get(), &fDrop);
        if (FAILED(hr))
        {
            break;
        }

        if (!fDrop)
        {
            // Get the request token
            hr = _tokens.RemoveFront(&spToken);
            if (FAILED(hr))
            {
                break;
            }

            if (spToken)
            {
                // If token was not null set the sample attribute.
                hr = spSample->SetUnknown(MFSampleExtension_Token, spToken.Get());
                if (FAILED(hr))
                {
                    break;
                }
            }

            if (_fDiscontinuity)
            {
                // If token was not null set the sample attribute.
                hr = spSample->SetUINT32(MFSampleExtension_Discontinuity, TRUE);
                if (FAILED(hr))
                {
                    break;
                }
                _fDiscontinuity = false;
            }

            // Send a sample event.
            hr = _spEventQueue->QueueEventParamUnk(MEMediaSample, GUID_NULL, S_OK, spSample.Get());
            if (FAILED(hr))
            {
                break;
            }
        }
        else
        {
            _fDiscontinuity = true;
        }
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaStream::HandleError(HRESULT hErrorCode)
{
    if (hErrorCode != MF_E_SHUTDOWN)
    {
        // Send MEError to the client
        hErrorCode = QueueEvent(MEError, GUID_NULL, hErrorCode, nullptr);
    }

    return hErrorCode;
}

HRESULT CMediaStream::SetSampleAttributes(StspSampleHeader *pSampleHeader, IMFSample *pSample)
{
    HRESULT hr = S_OK;
    hr = pSample->SetSampleTime(pSampleHeader->ullTimestamp);

    if (SUCCEEDED(hr))
    {
        hr = pSample->SetSampleDuration(pSampleHeader->ullDuration);
    }

    SET_SAMPLE_ATTRIBUTE(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, BottomFieldFirst);
    SET_SAMPLE_ATTRIBUTE(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, CleanPoint);
    SET_SAMPLE_ATTRIBUTE(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, DerivedFromTopField);
    SET_SAMPLE_ATTRIBUTE(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, Discontinuity);
    SET_SAMPLE_ATTRIBUTE(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, Interlaced);
    SET_SAMPLE_ATTRIBUTE(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, RepeatFirstField);
    SET_SAMPLE_ATTRIBUTE(pSampleHeader->dwFlags, pSampleHeader->dwFlagMasks, pSample, SingleField);

    DWORD cbTotalLen;
    pSample->GetTotalLength(&cbTotalLen);
    TRACE(TRACE_LEVEL_LOW, L"Received sample TS-%I64d Duration-%I64d Length-%d %S\n", pSampleHeader->ullTimestamp, pSampleHeader->ullDuration, cbTotalLen, (pSampleHeader->dwFlags & StspSampleFlag_CleanPoint) ? "key frame" : "");
    TRACEHR_RET(hr);
}

HRESULT CMediaStream::ShouldDropSample(IMFSample *pSample, BOOL *pfDrop) 
{
    *pfDrop = FALSE;
    if (!_fVideo)
    {
        return S_OK;
    }

    HRESULT hr = S_OK;
    BOOL fCleanPoint = MFGetAttributeUINT32( pSample, MFSampleExtension_CleanPoint, 0 ) > 0;
    *pfDrop = _flRate != 1.0f && !fCleanPoint;

    LONGLONG hnsTimeStamp = 0;
    hr = pSample->GetSampleTime(&hnsTimeStamp);

    if (!*pfDrop && _fDropTime)
    {
        if (SUCCEEDED(hr))
        {
            if (_fInitDropTime)
            {
                _hnsStartDroppingAt = hnsTimeStamp;
                _fInitDropTime = false;
            }

            *pfDrop = hnsTimeStamp < (_hnsStartDroppingAt + _hnsAmountToDrop);
            if (!*pfDrop)
            {
                TRACE(TRACE_LEVEL_LOW, L"Ending dropping time on sample ts=%I64d _hnsStartDroppingAt=%I64d _hnsAmountToDrop=%I64d\n", 
                    hnsTimeStamp, _hnsStartDroppingAt, _hnsAmountToDrop);
                ResetDropTime();
            }
            else
            {
                TRACE(TRACE_LEVEL_LOW, L": Dropping sample ts=%I64d _hnsStartDroppingAt=%I64d _hnsAmountToDrop=%I64d\n", 
                    hnsTimeStamp, _hnsStartDroppingAt, _hnsAmountToDrop);
            }
        }
    }

    if (SUCCEEDED(hr) && !*pfDrop && (_eDropMode == MF_DROP_MODE_1 || _fWaitingForCleanPoint))
    {
        // Only key frames
        *pfDrop = !fCleanPoint;
        if (fCleanPoint)
        {
            _fWaitingForCleanPoint = false;
        }

        if (*pfDrop)
        {
            TRACE(TRACE_LEVEL_LOW, L": Dropping sample ts=%I64d\n", 
                hnsTimeStamp, _hnsStartDroppingAt, _hnsAmountToDrop);
        }
    }

    TRACEHR_RET(hr);
}

HRESULT CMediaStream::CleanSampleQueue()
{
    auto pos = _samples.FrontPosition();
    ComPtr<IMFSample> spSample;

    if (_fVideo)
    {
        // For video streams leave first key frame.
        for (;SUCCEEDED(_samples.GetItemPos(pos, spSample.ReleaseAndGetAddressOf())); pos = _samples.Next(pos))
        {
            if (MFGetAttributeUINT32(spSample.Get(), MFSampleExtension_CleanPoint, 0))
            {
                break;
            }
        }
    }

    _samples.Clear();

    if (spSample != nullptr)
    {
        return _samples.InsertFront(spSample.Get());
    }
    return S_OK;
}

void CMediaStream::ResetDropTime()
{
    _fDropTime = false;
    _fInitDropTime = false;
    _hnsStartDroppingAt = 0;
    _hnsAmountToDrop = 0;
    _fWaitingForCleanPoint = true;
}
