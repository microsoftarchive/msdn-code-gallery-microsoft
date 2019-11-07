#include "pch.h"
#include "MediaStreamSourceStream.h"

namespace
{
class CSourceLock
{
public:
    CSourceLock(CMediaStreamSourceService *pSource)
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
    ComPtr<CMediaStreamSourceService> _spSource;
};

    HRESULT GetUint32FromMap(
        ABI::Windows::Foundation::Collections::IMap<HSTRING, IInspectable *> *pMap,
        LPCWSTR lpszValueName,
        UINT32 *pValue)
    {
        HString strKey;
        strKey.Set(lpszValueName);
        ComPtr<IInspectable> spInsp;
        ComPtr<ABI::Windows::Foundation::IPropertyValue> spPropVal;
        HRESULT hr = pMap->Lookup(strKey.Get(), &spInsp);

        if (SUCCEEDED(hr))
        {
            hr = spInsp.As(&spPropVal);
        }

        if (SUCCEEDED(hr))
        {
            hr = spPropVal->GetUInt32(pValue);
        }

        return hr;
    }
}

HRESULT CMediaStreamSourceStream::CreateInstance(
    _In_ CMediaStreamSourceService *pSource, 
    _In_ ABI::Windows::Foundation::Collections::IPropertySet *pStreamAttributes,
    _Out_ CMediaStreamSourceStream **ppStream)
{
    if (pSource == nullptr || pStreamAttributes == nullptr || ppStream == nullptr)
    {
        return E_POINTER;
    }

    ComPtr<CMediaStreamSourceStream> spStream;
    HRESULT hr = S_OK;
    
    spStream.Attach(new CMediaStreamSourceStream(pSource));

    if (spStream == nullptr)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = spStream->Initialize(pStreamAttributes);
    }

    if (SUCCEEDED(hr))
    {
        *ppStream = spStream.Detach();
    }

    return hr;
}

CMediaStreamSourceStream::CMediaStreamSourceStream(_In_ CMediaStreamSourceService *pSource)
    : _cRef(1)
    , _eSourceState(SourceState_Invalid)
    , _spSource(pSource)
{
}

CMediaStreamSourceStream::~CMediaStreamSourceStream(void)
{
}

HRESULT CMediaStreamSourceStream::Initialize(_In_ ABI::Windows::Foundation::Collections::IPropertySet *pStreamAttributes)
{
    // Create the media event queue.
    HRESULT hr = MFCreateEventQueue(&_spEventQueue);
    ComPtr<IMFMediaType> spMediaType;
    ComPtr<IMFStreamDescriptor> spSD;
    ComPtr<IMFMediaTypeHandler> spMediaTypeHandler;
    UINT32 cChannels = 0;
    UINT32 bitsPerSample = 0;
    UINT32 sampleRate = 0;
    
    ComPtr<ABI::Windows::Foundation::Collections::IMap<HSTRING, IInspectable *>> spSetting;

    if (SUCCEEDED(hr))
    {
        hr = pStreamAttributes->QueryInterface(IID_PPV_ARGS(&spSetting));
    }

    if (SUCCEEDED(hr))
    {
        hr = GetUint32FromMap(spSetting.Get(), L"Channels", &cChannels);
    }
    
    if (SUCCEEDED(hr))
    {
        hr = GetUint32FromMap(spSetting.Get(), L"BitsPerSample", &bitsPerSample);
    }

    if (SUCCEEDED(hr))
    {
        hr = GetUint32FromMap(spSetting.Get(), L"SampleRate", &sampleRate);
    }

    UINT32 blockAlign = cChannels * (bitsPerSample / 8);
    UINT32 bytesPerSecond = blockAlign * sampleRate;

    if (SUCCEEDED(hr))
    {
        // Create a media type object.
        hr = MFCreateMediaType(&spMediaType);
    }

    if (SUCCEEDED(hr))
    {
        hr = spMediaType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
    }

    if (SUCCEEDED(hr))
    {
        hr = spMediaType->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_PCM);
    }

    if (SUCCEEDED(hr))
    {
        hr = spMediaType->SetUINT32(MF_MT_AUDIO_NUM_CHANNELS, cChannels);
    }

    if (SUCCEEDED(hr))
    {
        hr = spMediaType->SetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, sampleRate);
    }

    if (SUCCEEDED(hr))
    {
        hr = spMediaType->SetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, blockAlign);
    }

    if (SUCCEEDED(hr))
    {
        hr = spMediaType->SetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, bytesPerSecond);
    }

    if (SUCCEEDED(hr))
    {
        hr = spMediaType->SetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, bitsPerSample);
    }

    if (SUCCEEDED(hr))
    {
        hr = spMediaType->SetUINT32(MF_MT_ALL_SAMPLES_INDEPENDENT, TRUE);
    }

    if (SUCCEEDED(hr))
    {
        // Now we can create MF stream descriptor.
        hr = MFCreateStreamDescriptor(AUDIO_STREAM_ID, 1, spMediaType.GetAddressOf(), &spSD);
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
        // State of the stream is started.
        _eSourceState = SourceState_Stopped;
    }

    return hr;
}

// IUnknown methods

IFACEMETHODIMP CMediaStreamSourceStream::QueryInterface(REFIID riid, void** ppv)
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
    else
    {
        hr = E_NOINTERFACE;
    }

    return hr;
}

IFACEMETHODIMP_(ULONG) CMediaStreamSourceStream::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CMediaStreamSourceStream::Release()
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

IFACEMETHODIMP CMediaStreamSourceStream::BeginGetEvent(IMFAsyncCallback* pCallback, IUnknown* punkState)
{
    HRESULT hr = S_OK;

    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->BeginGetEvent(pCallback, punkState);
    }

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceStream::EndGetEvent(IMFAsyncResult* pResult, IMFMediaEvent** ppEvent)
{
    HRESULT hr = S_OK;

    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->EndGetEvent(pResult, ppEvent);
    }

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceStream::GetEvent(DWORD dwFlags, IMFMediaEvent** ppEvent)
{
    // NOTE:
    // GetEvent can block indefinitely, so we don't hold the lock.
    // This requires some juggling with the event queue pointer.

    HRESULT hr = S_OK;

    ComPtr<IMFMediaEventQueue> spQueue;

    {
        CSourceLock lock(_spSource.Get());

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

IFACEMETHODIMP CMediaStreamSourceStream::QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, PROPVARIANT const *pvValue)
{
    HRESULT hr = S_OK;

    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        hr = _spEventQueue->QueueEventParamVar(met, guidExtendedType, hrStatus, pvValue);
    }

    return hr;
}

// IMFMediaStream methods

IFACEMETHODIMP CMediaStreamSourceStream::GetMediaSource(IMFMediaSource** ppMediaSource)
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

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceStream::GetStreamDescriptor(IMFStreamDescriptor** ppStreamDescriptor)
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

    return hr;
}

IFACEMETHODIMP CMediaStreamSourceStream::RequestSample(IUnknown* pToken)
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

    if (SUCCEEDED(hr))
    {
        hr = _spSource->GetSampleAsync(ABI::MSSWinRTExtension::MediaStreamType_Audio);
    }

    if (FAILED(hr))
    {
        hr = HandleError(hr);
    }

    return hr;
}

HRESULT CMediaStreamSourceStream::ReportSample(IMFSample *pSample)
{
    HRESULT hr = S_OK;

    CSourceLock lock(_spSource.Get());

    if (_samples.GetCount() >= _tokens.GetCount())
    {
        hr = MF_E_UNEXPECTED;
    }

    if (SUCCEEDED(hr)) 
    {
        if (_eSourceState == SourceState_Started)
        {
            hr = _samples.InsertBack(pSample);

            if (SUCCEEDED(hr))
            {
                // Trigger sample delivery
                hr = DeliverSamples();
            }
        }
        else
        {
            hr = MF_E_INVALIDREQUEST;
        }
    }

    if (FAILED(hr))
    {
        HandleError(hr);
    }

    return hr;
}

HRESULT CMediaStreamSourceStream::Start()
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_eSourceState == SourceState_Stopped)
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

    return hr;
}

HRESULT CMediaStreamSourceStream::Stop()
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

    return hr;;
}

HRESULT CMediaStreamSourceStream::Shutdown()
{
    CSourceLock lock(_spSource.Get());

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        _tokens.Clear();
        _samples.Clear();

        if (_spEventQueue)
        {
            _spEventQueue->Shutdown();
            _spEventQueue.ReleaseAndGetAddressOf();
        }

        _spStreamDescriptor.ReleaseAndGetAddressOf();
        _eSourceState = SourceState_Shutdown;

        _spSource.ReleaseAndGetAddressOf();
    }

    return hr;
}

// Deliver samples for every request client made
HRESULT CMediaStreamSourceStream::DeliverSamples()
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

        // Send a sample event.
        hr = _spEventQueue->QueueEventParamUnk(MEMediaSample, GUID_NULL, S_OK, spSample.Get());
        if (FAILED(hr))
        {
            break;
        }
    }

    return hr;
}

HRESULT CMediaStreamSourceStream::HandleError(HRESULT hrError)
{
    return QueueEvent(MEError, GUID_NULL, hrError, nullptr);
}