#include "StdAfx.h"
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
IFACEMETHODIMP CSourceOperation::QueryInterface(REFIID riid, void** ppv)
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
#pragma warning(push)
#pragma warning(disable: 4355)
    , _OnFinishOpenCB(this, &CGeometricMediaSource::OnFinishOpen)
#pragma warning(pop)
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

HRESULT CGeometricMediaSource::CreateInstance(CGeometricMediaSource **ppSource)
{
    HRESULT hr = S_OK;

    if (ppSource == nullptr)
    {
        return E_INVALIDARG;
    }

    ComPtr<CGeometricMediaSource> spSource;
    spSource.Attach(new(std::nothrow) CGeometricMediaSource());
    if (!spSource)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = spSource->Initialize();
    }

    if (SUCCEEDED(hr))
    {
        *ppSource = spSource.Detach();
    }

    return hr;
}

// IUnknown methods

IFACEMETHODIMP CGeometricMediaSource::QueryInterface(REFIID riid, void** ppv)
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
IFACEMETHODIMP CGeometricMediaSource::BeginGetEvent(IMFAsyncCallback* pCallback, IUnknown* punkState)
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

IFACEMETHODIMP CGeometricMediaSource::EndGetEvent(IMFAsyncResult* pResult, IMFMediaEvent** ppEvent)
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

IFACEMETHODIMP CGeometricMediaSource::GetEvent(DWORD dwFlags, IMFMediaEvent** ppEvent)
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
    IMFPresentationDescriptor** ppPresentationDescriptor
    )
{
    if (ppPresentationDescriptor == NULL)
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

IFACEMETHODIMP CGeometricMediaSource::GetCharacteristics(DWORD* pdwCharacteristics)
{
    if (pdwCharacteristics == NULL)
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
        IMFPresentationDescriptor* pPresentationDescriptor,
        const GUID* pguidTimeFormat,
        const PROPVARIANT* pvarStartPos
    )
{
    HRESULT hr = S_OK;

    // Check parameters.

    // Start position and presentation descriptor cannot be NULL.
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
    HRESULT hr = CheckShutdown();
    if (SUCCEEDED(hr))
    {
        _spDeviceManager.ReleaseAndGetAddressOf();
        if (pManager != nullptr)
        {
            hr = pManager->QueryInterface(IID_PPV_ARGS(&_spDeviceManager));
        }
    }

    if (SUCCEEDED(hr) && _spStream)
    {
        hr = _spStream->SetDXGIDeviceManager(_spDeviceManager.Get());
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

HRESULT CGeometricMediaSource::BeginOpen(LPCWSTR pszUrl, IMFAsyncCallback *pCB, IUnknown *pUnkState)
{
    if (pszUrl == nullptr || pCB == nullptr)
    {
        return E_INVALIDARG;
    }

    AutoLock lock(_critSec);
    HRESULT hr = S_OK;
    GeometricShape eShape = GeometricShape_Square;

    if (_eSourceState != SourceState_Invalid)
    {
        return MF_E_INVALIDREQUEST;
    }
    
    if (SUCCEEDED(hr))
    {
        // Parse url (to obtain host name and port)
        hr = ParseServerUrl(pszUrl, &eShape);
    }
    
    if (SUCCEEDED(hr))
    {
        hr = CGeometricMediaStream::CreateInstance(this, eShape, &_spStream);
    }

    if (SUCCEEDED(hr) && _spDeviceManager)
    {
        hr = _spStream->SetDXGIDeviceManager(_spDeviceManager.Get());
    }

    if (SUCCEEDED(hr))
    {
        hr = MFCreateAsyncResult(nullptr, pCB, pUnkState, &_spOpenResult);
    }

    if (SUCCEEDED(hr))
    {
        hr = MFPutWorkItem2(MFASYNC_CALLBACK_QUEUE_STANDARD, 0, &_OnFinishOpenCB, _spOpenResult.Get());
    }

    if (SUCCEEDED(hr))
    {
        // If everything is ok now we are waiting for network client to connect. 
        // Change state to opening.
        _eSourceState = SourceState_Opening;
    }

    return hr;
}

HRESULT CGeometricMediaSource::EndOpen(IMFAsyncResult *pResult)
{
    HRESULT hr = pResult->GetStatus();

    if (FAILED(hr))
    {
        Shutdown();
    }

    return hr;
}

HRESULT CGeometricMediaSource::Initialize()
{
    HRESULT hr = S_OK;

    // Create the event queue helper.
    hr = MFCreateEventQueue(&_spEventQueue);

    if (SUCCEEDED(hr))
    {
        hr = MFCreateAttributes(&_spAttributes, 1);
    }

    if (FAILED(hr))
    {
        Shutdown();
    }

    return hr;
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


HRESULT CGeometricMediaSource::CompleteOpen(HRESULT hResult)
{
    assert(_spOpenResult);
    HRESULT hr = _spOpenResult->SetStatus(hResult);
    if (SUCCEEDED(hr))
    {
        // Invoke the user's callback 
        hr = MFInvokeCallback(_spOpenResult.Get());
    }

    _spOpenResult.ReleaseAndGetAddressOf();

    return hr;
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
    (void)_spEventQueue->QueueEventParamVar(MESourceStopped, GUID_NULL, hr, NULL);

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
    (void)_spEventQueue->QueueEventParamVar(MESourceRateChanged, GUID_NULL, hr, NULL);

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

HRESULT CGeometricMediaSource::ParseServerUrl(LPCWSTR pszUrl, GeometricShape *pShape)
{
    if (pszUrl == nullptr || pShape == nullptr)
    {
        return E_INVALIDARG;
    }
    ComPtr<ABI::Windows::Foundation::IUriRuntimeClassFactory> spUriFactory;
    ComPtr<ABI::Windows::Foundation::IUriRuntimeClass> spUri;
    HString strUrl;
    strUrl.Set((wchar_t*)pszUrl);
    HRESULT hr = ABI::Windows::Foundation::GetActivationFactory(HStringReference(RuntimeClass_Windows_Foundation_Uri).Get(), &spUriFactory);

    if (SUCCEEDED(hr))
    {
        hr = spUriFactory->CreateUri(strUrl.Get(), &spUri);
    }

    if (SUCCEEDED(hr))
    {
        HString strScheme;
        LPCWSTR pszScheme = nullptr;
        hr = spUri->get_SchemeName(strScheme.GetAddressOf());

        if (SUCCEEDED(hr))
        {
            pszScheme = WindowsGetStringRawBuffer(strScheme.Get(), nullptr);
        }

        // It is not a scheme we support
        if (SUCCEEDED(hr) && (pszScheme == nullptr || wcscmp(pszScheme, c_szGeometricScheme) != 0))
        {
            hr = MF_E_UNSUPPORTED_SCHEME;
        }
    }
    if (SUCCEEDED(hr))
    {
        HString strHost;
        LPCWSTR pszHost = nullptr;
        hr = spUri->get_Host(strHost.GetAddressOf());

        if (SUCCEEDED(hr))
        {
            pszHost = WindowsGetStringRawBuffer(strHost.Get(), nullptr);
            hr = MF_E_INVALIDNAME;
            for (DWORD dwIndex = 0; dwIndex < GeometricShape_Count; ++dwIndex)
            {
                if (_wcsicmp(c_arrShapeNames[dwIndex], pszHost) == 0)
                {
                    *pShape = static_cast<GeometricShape>(dwIndex);
                    hr = S_OK;
                    break;
                }
            }
        }
    }

    return hr;
}

HRESULT CGeometricMediaSource::OnFinishOpen(IMFAsyncResult *pResult)
{
    HRESULT hr = pResult->GetStatus();
    ComPtr<IMFStreamDescriptor> spStreamDesc;
    
    if (SUCCEEDED(hr))
    {
        hr = _spStream->GetStreamDescriptor(&spStreamDesc);
    }

    if (SUCCEEDED(hr))
    {
        hr = MFCreatePresentationDescriptor(1, spStreamDesc.GetAddressOf(), _spPresentationDescriptor.ReleaseAndGetAddressOf());
    }

    if (SUCCEEDED(hr))
    {
        hr = _spPresentationDescriptor->SelectStream(0);
    }

    if (SUCCEEDED(hr))
    {
        _eSourceState = SourceState_Stopped;
    }

    hr = CompleteOpen(hr);
    
    return hr;
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
