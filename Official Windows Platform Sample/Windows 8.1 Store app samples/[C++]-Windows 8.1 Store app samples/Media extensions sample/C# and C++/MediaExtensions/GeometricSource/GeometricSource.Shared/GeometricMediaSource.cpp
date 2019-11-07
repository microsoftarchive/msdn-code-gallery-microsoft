//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include <wrl\module.h>
#include "GeometricMediaSource.h"
#include "GeometricMediaStream.h"

CSourceOperation::CSourceOperation(CSourceOperation::Type opType)
    : _cRef(1)
    , _opType(opType)
{
    ZeroMemory(&_data, sizeof(_data));
}

CSourceOperation::~CSourceOperation()
{
    PropVariantClear(&_data);
}

// IUnknown methods
IFACEMETHODIMP CSourceOperation::QueryInterface(REFIID riid, void **ppv)
{
    return E_NOINTERFACE;
}

IFACEMETHODIMP_(ULONG) CSourceOperation::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CSourceOperation::Release()
{
    long cRef = InterlockedDecrement(&_cRef);
    if (cRef == 0)
    {
        delete this;
    }
    return cRef;
}

HRESULT CSourceOperation::SetData(const PROPVARIANT& varData)
{
    return PropVariantCopy(&_data, &varData);
}

CStartOperation::CStartOperation(IMFPresentationDescriptor *pPD)
    : CSourceOperation(CSourceOperation::Operation_Start)
    , _spPD(pPD)
{
}

CStartOperation::~CStartOperation()
{
}

CSetRateOperation::CSetRateOperation(BOOL fThin, float flRate)
: CSourceOperation(CSourceOperation::Operation_SetRate)
, _fThin(fThin)
, _flRate(flRate)
{
}

CSetRateOperation::~CSetRateOperation()
{
}

CGeometricMediaSource::CGeometricMediaSource(void)
    : OpQueue<CGeometricMediaSource, CSourceOperation>(_critSec.m_criticalSection)
    , _cRef(1)
    , _eSourceState(SourceState_Invalid)
    , _flRate(1.0f)
{
    auto module = ::Microsoft::WRL::GetModuleBase();
    if (module != nullptr)
    {
        module->IncrementObjectCount();
    }
}


CGeometricMediaSource::~CGeometricMediaSource(void)
{
    auto module = ::Microsoft::WRL::GetModuleBase();
    if (module != nullptr)
    {
        module->DecrementObjectCount();
    }
}

ComPtr<CGeometricMediaSource> CGeometricMediaSource::CreateInstance()
{
    ComPtr<CGeometricMediaSource> spSource;
    spSource.Attach(new(std::nothrow) CGeometricMediaSource());
    if (!spSource)
    {
        throw ref new OutOfMemoryException();
    }

    spSource->Initialize();

    return spSource;
}

// IUnknown methods

IFACEMETHODIMP CGeometricMediaSource::QueryInterface(REFIID riid, void **ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    HRESULT hr = E_NOINTERFACE;
    (*ppv) = nullptr;
    if (riid == IID_IUnknown || 
        riid == IID_IMFMediaEventGenerator ||
        riid == IID_IMFMediaSource ||
        riid == IID_IMFMediaSourceEx)
    {
        (*ppv) = static_cast<IMFMediaSourceEx *>(this);
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

IFACEMETHODIMP_(ULONG) CGeometricMediaSource::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CGeometricMediaSource::Release()
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
IFACEMETHODIMP CGeometricMediaSource::BeginGetEvent(IMFAsyncCallback *pCallback, IUnknown *punkState)
{
    HRESULT hr = S_OK;

	AutoLock lock(_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->BeginGetEvent(pCallback, punkState);
    }

    return hr;
}

IFACEMETHODIMP CGeometricMediaSource::EndGetEvent(IMFAsyncResult *pResult, IMFMediaEvent **ppEvent)
{
    HRESULT hr = S_OK;

	AutoLock lock(_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->EndGetEvent(pResult, ppEvent);
    }

    return hr;
}

IFACEMETHODIMP CGeometricMediaSource::GetEvent(DWORD dwFlags, IMFMediaEvent **ppEvent)
{
    // NOTE:
    // GetEvent can block indefinitely, so we don't hold the lock.
    // This requires some juggling with the event queue pointer.

    HRESULT hr = S_OK;

    ComPtr<IMFMediaEventQueue> spQueue;

    {
	    AutoLock lock(_critSec);

        // Check shutdown
        hr = CheckShutdown();

        // Get the pointer to the event queue.
        if (SUCCEEDED(hr))
        {
            spQueue = _spEventQueue;
        }
    }

    // Now get the event.
    if (SUCCEEDED(hr))
    {
        hr = spQueue->GetEvent(dwFlags, ppEvent);
    }

    return hr;
}

IFACEMETHODIMP CGeometricMediaSource::QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, PROPVARIANT const *pvValue)
{
    HRESULT hr = S_OK;

	AutoLock lock(_critSec);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->QueueEventParamVar(met, guidExtendedType, hrStatus, pvValue);
    }

    return hr;
}

// IMFMediaSource methods
IFACEMETHODIMP CGeometricMediaSource::CreatePresentationDescriptor(
    IMFPresentationDescriptor **ppPresentationDescriptor
    )
{
    if (ppPresentationDescriptor == nullptr)
    {
        return E_POINTER;
    }

	AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr) &&
        (_eSourceState == SourceState_Opening || _eSourceState == SourceState_Invalid || !_spPresentationDescriptor))
    {
        hr = MF_E_NOT_INITIALIZED;
    }

    if (SUCCEEDED(hr))
    {
        hr = _spPresentationDescriptor->Clone(ppPresentationDescriptor);
    }

    return hr;
}

IFACEMETHODIMP CGeometricMediaSource::GetCharacteristics(DWORD *pdwCharacteristics)
{
    if (pdwCharacteristics == nullptr)
    {
        return E_POINTER;
    }

	AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        *pdwCharacteristics = MFMEDIASOURCE_IS_LIVE;
    }

    return hr;
}

IFACEMETHODIMP CGeometricMediaSource::Pause()
{
    return MF_E_INVALID_STATE_TRANSITION;
}

IFACEMETHODIMP CGeometricMediaSource::Shutdown()
{
	AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_spEventQueue)
        {
            _spEventQueue->Shutdown();
        }

        if (_spStream)
        {
            _spStream->Shutdown();
        }

         _eSourceState = SourceState_Shutdown;

        _spEventQueue.ReleaseAndGetAddressOf();
        _spStream.ReleaseAndGetAddressOf();
        _spDeviceManager.ReleaseAndGetAddressOf();
    }
    
    return hr;
}

IFACEMETHODIMP CGeometricMediaSource::Start(
        IMFPresentationDescriptor *pPresentationDescriptor,
        const GUID *pguidTimeFormat,
        const PROPVARIANT *pvarStartPos
    )
{
    HRESULT hr = S_OK;

    // Check parameters.

    // Start position and presentation descriptor cannot be nullptr.
    if (pvarStartPos == nullptr || pPresentationDescriptor == nullptr)
    {
        return E_INVALIDARG;
    }

    // Check the time format.
    if ((pguidTimeFormat != nullptr) && (*pguidTimeFormat != GUID_NULL))
    {
        // Unrecognized time format GUID.
        return MF_E_UNSUPPORTED_TIME_FORMAT;
    }

    // Check the data type of the start position.
    if (pvarStartPos->vt != VT_EMPTY && pvarStartPos->vt != VT_I8)
    {
        return MF_E_UNSUPPORTED_TIME_FORMAT;
    }

    AutoLock lock(_critSec);
    ComPtr<CStartOperation> spStartOp;

    if (_eSourceState != SourceState_Stopped &&
        _eSourceState != SourceState_Started)
    {
        hr = MF_E_INVALIDREQUEST;
    }

    if (SUCCEEDED(hr))
    {
        // Check if the presentation description is valid.
        hr = ValidatePresentationDescriptor(pPresentationDescriptor);
    }

    if (SUCCEEDED(hr))
    {
        // Prepare asynchronous operation attributes
        spStartOp = new(std::nothrow) CStartOperation(pPresentationDescriptor);
        if (!spStartOp)
        {
            hr = E_OUTOFMEMORY;
        }

        if (SUCCEEDED(hr))
        {
            hr = spStartOp->SetData(*pvarStartPos);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Queue asynchronous operation
        hr = QueueOperation(spStartOp.Get());
    }

    return hr;
}

IFACEMETHODIMP CGeometricMediaSource::Stop()
{
    HRESULT hr = S_OK;
    ComPtr<CSourceOperation> spStopOp;
    spStopOp.Attach(new(std::nothrow) CSourceOperation(CSourceOperation::Operation_Stop));
    if (!spStopOp)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        // Queue asynchronous stop
        hr = QueueOperation(spStopOp.Get());
    }

    return hr;
}

// IMFMediaSourceEx
IFACEMETHODIMP CGeometricMediaSource::GetSourceAttributes(_Outptr_ IMFAttributes **ppAttributes)
{
    if (ppAttributes == nullptr)
    {
        return E_POINTER;
    }

    *ppAttributes = _spAttributes.Get();
    (*ppAttributes)->AddRef();

    return S_OK;
}

IFACEMETHODIMP CGeometricMediaSource::GetStreamAttributes(_In_ DWORD dwStreamIdentifier, _Outptr_ IMFAttributes **ppAttributes)
{
    if (ppAttributes == nullptr)
    {
        return E_POINTER;
    }

    *ppAttributes = _spAttributes.Get();
    (*ppAttributes)->AddRef();

    return S_OK;
}

IFACEMETHODIMP CGeometricMediaSource::SetD3DManager(_In_opt_ IUnknown *pManager)
{
    HRESULT hr = S_OK;
    try
    {
        ThrowIfError(CheckShutdown());
        _spDeviceManager.Reset();
        if (pManager != nullptr)
        {
            ThrowIfError(pManager->QueryInterface(IID_PPV_ARGS(&_spDeviceManager)));
        }

        if (_spStream != nullptr)
        {
            _spStream->SetDXGIDeviceManager(_spDeviceManager.Get());
        }
    }
    catch (Exception ^exc)
    {
        hr = exc->HResult;
    }

    return hr;
}

IFACEMETHODIMP CGeometricMediaSource::GetService( _In_ REFGUID guidService, _In_ REFIID riid, _Out_opt_ LPVOID *ppvObject)
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

IFACEMETHODIMP CGeometricMediaSource::SetRate(BOOL fThin, float flRate)
{
    if (fThin)
    {
        return MF_E_THINNING_UNSUPPORTED;
    }
    if (!IsRateSupported(flRate, &flRate))
    {
        return MF_E_UNSUPPORTED_RATE;
    }

    AutoLock lock(_critSec);
    HRESULT hr = S_OK;

    if (flRate == _flRate)
    {
        return S_OK;
    }

    ComPtr<CSourceOperation> spSetRateOp;
    spSetRateOp.Attach(new CSetRateOperation(fThin, flRate));
    if (!spSetRateOp)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        // Queue asynchronous stop
        hr = QueueOperation(spSetRateOp.Get());
    }

    return hr;
}

IFACEMETHODIMP CGeometricMediaSource::GetRate(_Inout_opt_ BOOL *pfThin, _Inout_opt_ float *pflRate)
{
    AutoLock lock(_critSec);
    if (pfThin == nullptr || pflRate == nullptr)
    {
        return E_INVALIDARG;
    }

    *pfThin = FALSE;
    *pflRate = _flRate;

    return S_OK;
}

__override HRESULT CGeometricMediaSource::DispatchOperation(CSourceOperation *pOp)
{
	AutoLock lock(_critSec);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        switch(pOp->GetOperationType())
        {
        case CSourceOperation::Operation_Start:
            hr = DoStart(static_cast<CStartOperation *>(pOp));
            break;
        case CSourceOperation::Operation_Stop:
            hr = DoStop(pOp);
            break;
        case CSourceOperation::Operation_SetRate:
            hr = DoSetRate(static_cast<CSetRateOperation *>(pOp));
            break;
        default:
            hr = E_UNEXPECTED;
            break;
        }
    }

    return hr;
}

__override HRESULT CGeometricMediaSource::ValidateOperation(CSourceOperation *pOp)
{
    return S_OK;
}

concurrency::task<void> CGeometricMediaSource::OpenAsync(Platform::String ^url)
{
    if (url == nullptr)
    {
        throw ref new InvalidArgumentException();
    }

    AutoLock lock(_critSec);

    if (_eSourceState != SourceState_Invalid)
    {
        ThrowException(MF_E_INVALIDREQUEST);
    }

    // Parse url (to obtain host name and port)
    GeometricShape eShape = ParseServerUrl(url);
    
    _spStream = CGeometricMediaStream::CreateInstance(this, eShape);

    if (_spDeviceManager != nullptr)
    {
        _spStream->SetDXGIDeviceManager(_spDeviceManager.Get());
    }

    ComPtr<CGeometricMediaSource> spThis = this;
    concurrency::create_task([this, spThis]()
    {
        ComPtr<IMFStreamDescriptor> spStreamDesc;
        HRESULT hr = S_OK;
        try
        {
            ThrowIfError(_spStream->GetStreamDescriptor(&spStreamDesc));
            ThrowIfError(MFCreatePresentationDescriptor(1, spStreamDesc.GetAddressOf(), _spPresentationDescriptor.ReleaseAndGetAddressOf()));
            ThrowIfError(_spPresentationDescriptor->SelectStream(0));
            _eSourceState = SourceState_Stopped;
        }
        catch (Exception ^exc)
        {
            hr = exc->HResult;
        }

        CompleteOpen(hr);
    });

    // If everything is ok now we are waiting for network client to connect. 
    // Change state to opening.
    _eSourceState = SourceState_Opening;

    return create_task(_openedEvent);
}

void CGeometricMediaSource::Initialize()
{
    try
    {
        // Create the event queue helper.
        ThrowIfError(MFCreateEventQueue(&_spEventQueue));

        ThrowIfError(MFCreateAttributes(&_spAttributes, 1));
    }
    catch (Exception ^exc)
    {
        Shutdown();
        throw;
    }
}

// Handle errors
void CGeometricMediaSource::HandleError(HRESULT hResult)
{
    if (_eSourceState == SourceState_Opening)
    {
        // If we have an error during opening operation complete it and pass the error to client.
        CompleteOpen(hResult);
    }
    else if (_eSourceState != SourceState_Shutdown)
    {
        // If we received an error at any other time (except shutdown) send MEError event.
        QueueEvent(MEError, GUID_NULL, hResult, nullptr);
    }
}

void CGeometricMediaSource::CompleteOpen(HRESULT hResult)
{
    assert(!_openedEvent._IsTriggered());
    if (FAILED(hResult))
    {
        Shutdown();
        _openedEvent.set_exception(ref new COMException(hResult));
    }
    else
    {
        _openedEvent.set();
    }
}

HRESULT CGeometricMediaSource::DoStart(CStartOperation *pOp)
{
    assert(pOp->GetOperationType() == CSourceOperation::Operation_Start);
    ComPtr<IMFPresentationDescriptor> spPD = pOp->GetPresentationDescriptor();
    MediaEventType met = MENewStream;
    if (_eSourceState == SourceState_Started)
    {
        met = MEUpdatedStream;
    }
    HRESULT hr = _spEventQueue->QueueEventParamUnk(met, GUID_NULL, S_OK, _spStream.Get());

    if (SUCCEEDED(hr))
    {
        hr = _spStream->Start();
    }

    if (SUCCEEDED(hr))
    {
        _eSourceState = SourceState_Started;
        hr = _spEventQueue->QueueEventParamVar(MESourceStarted, GUID_NULL, hr, &pOp->GetData());
    }

    if (FAILED(hr))
    {
        _eSourceState = SourceState_Stopped;
        hr = _spEventQueue->QueueEventParamVar(MESourceStarted, GUID_NULL, hr, nullptr);
    }

    return hr;
}

HRESULT CGeometricMediaSource::DoStop(CSourceOperation *pOp)
{
    assert(pOp->GetOperationType() == CSourceOperation::Operation_Stop);
    
    HRESULT hr = _spStream->Stop();

    // Send the "stopped" event. This might include a failure code.
    (void)_spEventQueue->QueueEventParamVar(MESourceStopped, GUID_NULL, hr, nullptr);

    return hr;
}

HRESULT CGeometricMediaSource::DoSetRate(CSetRateOperation *pOp)
{
    assert(pOp->GetOperationType() == CSourceOperation::Operation_SetRate);

    HRESULT hr = S_OK;

    if (_spStream == nullptr)
    {
        hr = E_FAIL;
    }

    if (SUCCEEDED(hr))
    {
        hr = _spStream->SetRate(pOp->GetRate());
    }

    if (SUCCEEDED(hr))
    {
        _flRate = pOp->GetRate();
    }

    // Send the "rate changed" event. This might include a failure code.
    (void)_spEventQueue->QueueEventParamVar(MESourceRateChanged, GUID_NULL, hr, nullptr);

    return hr;
}

HRESULT CGeometricMediaSource::ValidatePresentationDescriptor(IMFPresentationDescriptor *pPD)
{
    HRESULT hr = S_OK;
    BOOL fSelected = FALSE;
    DWORD cStreams = 0;

    // The caller's PD must have the same number of streams as ours.
    hr = pPD->GetStreamDescriptorCount(&cStreams);

    if (SUCCEEDED(hr))
    {
        if (cStreams != 1)
        {
            hr = E_INVALIDARG;
        }
    }

    // The caller must select at least one stream.
    if (SUCCEEDED(hr))
    {
        ComPtr<IMFStreamDescriptor> spSD;
        hr = pPD->GetStreamDescriptorByIndex(0, &fSelected, &spSD);

        // As for now all streams have to be selected
        if (SUCCEEDED(hr) && !fSelected)
        {
            hr = E_INVALIDARG;
        }

        if (SUCCEEDED(hr))
        {
            DWORD dwId = 0;
            hr = spSD->GetStreamIdentifier(&dwId);

            if (SUCCEEDED(hr) && dwId != c_dwGeometricStreamId)
            {
                hr = E_INVALIDARG;
            }
        }
    }

    return hr;
}

GeometricShape CGeometricMediaSource::ParseServerUrl(String ^url)
{
    if (url == nullptr)
    {
        throw ref new InvalidArgumentException();
    }
    auto uri = ref new Windows::Foundation::Uri(url);

    String ^scheme = uri->SchemeName;
    if (wcscmp(scheme->Data(), c_szGeometricScheme) != 0)
    {
        throw ref new COMException(MF_E_UNSUPPORTED_SCHEME);
    }

    String ^host = uri->Host;
    GeometricShape result;
    bool fFound = false;
    for (DWORD dwIndex = 0; dwIndex < GeometricShape_Count; ++dwIndex)
    {
        if (_wcsicmp(c_arrShapeNames[dwIndex], host->Data()) == 0)
        {
            result = static_cast<GeometricShape>(dwIndex);
            fFound = true;
            break;
        }
    }

    if (!fFound)
    {
        throw ref new COMException(MF_E_INVALIDNAME);
    }
    return result;
}

BOOL CGeometricMediaSource::IsRateSupported(float flRate, float *pflAdjustedRate)
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
