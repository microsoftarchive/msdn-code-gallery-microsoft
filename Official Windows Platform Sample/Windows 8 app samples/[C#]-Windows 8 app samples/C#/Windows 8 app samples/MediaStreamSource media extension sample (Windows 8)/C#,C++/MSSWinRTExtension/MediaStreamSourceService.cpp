#include "pch.h"
#include "MediaStreamSourceService.h"
#include "MediaStreamSourceStream.h"

CMediaStreamSourceService::CMediaStreamSourceService(void)
    : _eSourceState(SourceState_Invalid)
{
}


CMediaStreamSourceService::~CMediaStreamSourceService(void)
{
}

HRESULT CMediaStreamSourceService::RuntimeClassInitialize(
        _In_ ABI::MSSWinRTExtension::IMediaStreamSourcePlugin *pPlugin, _In_ IMFAsyncCallback *pOpenAsyncCallback, _In_ IUnknown *pOpenState)
{
    HRESULT hr = S_OK;
    ComPtr<IMFAsyncResult> spResult;

    // Create the event queue helper.
    hr = MFCreateEventQueue(&_spEventQueue);

    if (SUCCEEDED(hr))
    {
        ComPtr<IUnknown> spunkSource;
        hr = this->QueryInterface(IID_PPV_ARGS(&spunkSource));
        if (SUCCEEDED(hr))
        {
            hr = MFCreateAsyncResult(spunkSource.Get(), pOpenAsyncCallback, pOpenState, &spResult);
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = pPlugin->SetService(this);
    }

    if (SUCCEEDED(hr))
    {
        _spOpenResult = spResult;
        _spPlugin = pPlugin;
    }

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceService::ErrorOccurred(HSTRING errorCode)
{
    AutoLock lock(_cs);
    
    HandleError(E_FAIL);

    return S_OK;
}

IFACEMETHODIMP CMediaStreamSourceService::ReportGetSampleCompleted( _In_ ABI::MSSWinRTExtension::IMediaStreamSample *pSample)
{
    AutoLock lock(_cs);

    HRESULT hr = S_OK;
    ComPtr<IMFSample> spSample;
    ComPtr<IMFMediaBuffer> spBuffer;
    LONGLONG llDuration, llTimestamp;

    hr = pSample->QueryInterface(IID_PPV_ARGS(&spBuffer));

    if (SUCCEEDED(hr))
    {
        hr = MFCreateSample(&spSample);
    }

    if (SUCCEEDED(hr))
    {
        hr = spSample->AddBuffer(spBuffer.Get());
    }

    if (SUCCEEDED(hr))
    {
        pSample->get_Duration(&llDuration);
        pSample->get_Timestamp(&llTimestamp);
        spSample->SetSampleDuration(llDuration);
        spSample->SetSampleTime(llTimestamp);
    }

    if (SUCCEEDED(hr))
    {
        hr = _spAudioStream->ReportSample(spSample.Get());
    }

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceService::ReportOpenMediaCompleted( 
    _In_ ABI::Windows::Foundation::Collections::IPropertySet *pSourceAttributes,
    _In_ ABI::Windows::Foundation::Collections::IPropertySet *pAudioStreamAttributes)
{
    AutoLock lock(_cs);

    HRESULT hr = S_OK;

    if (_spAudioStream != nullptr && _spOpenResult == nullptr)
    {
        return MF_E_INVALIDREQUEST;
    }

    hr = CMediaStreamSourceStream::CreateInstance(this, pAudioStreamAttributes, &_spAudioStream);

    if (SUCCEEDED(hr))
    {
        hr = InitPresentationDescription();
    }

    if (SUCCEEDED(hr))
    {
        _eSourceState = SourceState_Stopped;
    }

    CompleteOpen(hr);

    return hr;
}

// IMFMediaEventGenerator
IFACEMETHODIMP CMediaStreamSourceService::BeginGetEvent(IMFAsyncCallback* pCallback,IUnknown* punkState)
{
    HRESULT hr = S_OK;

    AutoLock lock(_cs);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->BeginGetEvent(pCallback, punkState);
    }

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceService::EndGetEvent(IMFAsyncResult* pResult, IMFMediaEvent** ppEvent)
{
    HRESULT hr = S_OK;

    AutoLock lock(_cs);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->EndGetEvent(pResult, ppEvent);
    }

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceService::GetEvent(DWORD dwFlags, IMFMediaEvent** ppEvent)
{
    // NOTE:
    // GetEvent can block indefinitely, so we don't hold the lock.
    // This requires some juggling with the event queue pointer.

    HRESULT hr = S_OK;

    ComPtr<IMFMediaEventQueue> spQueue;

    {
        AutoLock lock(_cs);

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

IFACEMETHODIMP CMediaStreamSourceService::QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, const PROPVARIANT* pvValue)
{
    HRESULT hr = S_OK;

    AutoLock lock(_cs);

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->QueueEventParamVar(met, guidExtendedType, hrStatus, pvValue);
    }

    return hr;
}

// IMFMediaSource
IFACEMETHODIMP CMediaStreamSourceService::CreatePresentationDescriptor(IMFPresentationDescriptor** ppPresentationDescriptor)
{
    if (ppPresentationDescriptor == NULL)
    {
        return E_POINTER;
    }

    AutoLock lock(_cs);

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

IFACEMETHODIMP CMediaStreamSourceService::GetCharacteristics(DWORD* pdwCharacteristics)
{
    if (pdwCharacteristics == NULL)
    {
        return E_POINTER;
    }

    AutoLock lock(_cs);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        *pdwCharacteristics = MFMEDIASOURCE_IS_LIVE;
    }

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceService::Pause()
{
    return E_NOTIMPL;
}

IFACEMETHODIMP CMediaStreamSourceService::Shutdown()
{
    AutoLock lock(_cs);

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_spEventQueue)
        {
            _spEventQueue->Shutdown();
        }

        if (_spAudioStream != nullptr)
        {
            _spAudioStream->Shutdown();
            _spAudioStream.ReleaseAndGetAddressOf();
        }

        _eSourceState = SourceState_Shutdown;

        if (_spPlugin != nullptr)
        {
            _spPlugin->CloseMedia();
            _spPlugin.ReleaseAndGetAddressOf();
        }

        _spEventQueue.ReleaseAndGetAddressOf();
    }

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceService::Start(
    IMFPresentationDescriptor* pPresentationDescriptor,
    const GUID* pguidTimeFormat,
    const PROPVARIANT* pvarStartPosition
)
{
    HRESULT hr = S_OK;

    // Check parameters.

    // Start position and presentation descriptor cannot be NULL.
    if (pvarStartPosition == nullptr || pPresentationDescriptor == nullptr)
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
    if (pvarStartPosition->vt != VT_EMPTY && pvarStartPosition->vt != VT_I8)
    {
        return MF_E_UNSUPPORTED_TIME_FORMAT;
    }

    AutoLock lock(_cs);

    if (_eSourceState != SourceState_Stopped)
    {
        hr = MF_E_INVALIDREQUEST;
    }

    if (SUCCEEDED(hr))
    {
        ComPtr<IUnknown> spStreamUnk;
        hr = _spAudioStream.As(&spStreamUnk);
        if (SUCCEEDED(hr))
        {
            hr = _spEventQueue->QueueEventParamUnk(MENewStream, GUID_NULL, hr, spStreamUnk.Get());
        }

        if (SUCCEEDED(hr))
        {
            hr = _spAudioStream->Start();
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->QueueEventParamVar(MESourceStarted, GUID_NULL, hr, pvarStartPosition);
    }

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceService::Stop()
{
    AutoLock lock(_cs);
    HRESULT hr = S_OK;

    if (_eSourceState != SourceState_Stopped)
    {
        hr = MF_E_INVALIDREQUEST;
    }

    if (SUCCEEDED(hr) && _spAudioStream != nullptr)
    {
        hr = _spAudioStream->Stop();
    }

    if (SUCCEEDED(hr) && _spPlugin != nullptr)
    {
        _spPlugin->CloseMedia();
        _spPlugin.ReleaseAndGetAddressOf();
    }

    hr = _spEventQueue->QueueEventParamVar(MESourceStopped, GUID_NULL, hr, NULL);

    return hr;
}

HRESULT CMediaStreamSourceService::HandleError(HRESULT hrError)
{
    HRESULT hr = S_OK;
    if (_spOpenResult != nullptr)
    {
        hr = CompleteOpen(hrError);
    }
    else
    {
        hr = QueueEvent(MEError, GUID_NULL, hrError, nullptr);
    }

    return hr;
}

HRESULT CMediaStreamSourceService::GetSampleAsync(ABI::MSSWinRTExtension::MediaStreamType streamType)
{
    AutoLock lock(_cs);
    HRESULT hr = S_OK;

    if (_spPlugin == nullptr)
    {
        hr = MF_E_INVALIDREQUEST;
    }
    else
    {
        hr = _spPlugin->GetSampleAsync(streamType);
    }

    return hr;
}

HRESULT CMediaStreamSourceService::CompleteOpen(HRESULT hResult)
{
    HRESULT hr = _spOpenResult->SetStatus(hResult);
    if (SUCCEEDED(hr))
    {
        // Invoke the user's callback 
        hr = MFInvokeCallback(_spOpenResult.Get());
    }

    _spOpenResult.ReleaseAndGetAddressOf();

    return hr;
}

HRESULT CMediaStreamSourceService::InitPresentationDescription()
{
    ComPtr<IMFPresentationDescriptor> spPresentationDescriptor;
    ComPtr<IMFStreamDescriptor> spSD;
    HRESULT hr = _spAudioStream->GetStreamDescriptor(&spSD);

    if (SUCCEEDED(hr))
    {
        hr = MFCreatePresentationDescriptor(1, spSD.GetAddressOf(), &spPresentationDescriptor);
    }

    if (SUCCEEDED(hr))
    {
        hr = spPresentationDescriptor->SelectStream(0);
    }

    if (SUCCEEDED(hr))
    {
        _spPresentationDescriptor = spPresentationDescriptor;
    }

    return hr;
}
