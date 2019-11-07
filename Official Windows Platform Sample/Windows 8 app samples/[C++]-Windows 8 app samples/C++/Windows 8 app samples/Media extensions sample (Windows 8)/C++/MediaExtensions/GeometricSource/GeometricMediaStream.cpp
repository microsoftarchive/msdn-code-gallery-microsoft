#include "StdAfx.h"
#include "GeometricMediaStream.h"
#include <initguid.h>
#include <math.h>
#include <wrl\module.h>

namespace
{
    const DWORD c_dwOutputImageHeight = 256;
    const DWORD c_dwOutputImageWidth = 320;
    const DWORD c_cbOutputSamplenNumPixels = c_dwOutputImageWidth * c_dwOutputImageHeight;
    const DWORD c_cbOutputSampleSize = c_cbOutputSamplenNumPixels * 4;
    const DWORD c_dwOutputFrameRateNumerator = 1;
    const DWORD c_dwOutputFrameRateDenominator = 1;
    const LONGLONG c_llOutputFrameDuration = 1000000ll;
}

class CGeometricMediaStream::CSourceLock
{
public:
    _Acquires_lock_(_spSource)
        CSourceLock(CGeometricMediaSource *pSource)
        : _spSource(pSource)
    {
        if (_spSource)
        {
            _spSource->Lock();
        }
    }

    _Releases_lock_(_spSource)
        ~CSourceLock()
    {
        if (_spSource)
        {
            _spSource->Unlock();
        }
    }

private:
    ComPtr<CGeometricMediaSource> _spSource;
};

BYTE Clip(int i)
{
    return (i > 255 ? 255 : (i < 0 ? 0 : i));
}

DWORD YUVToRGB( BYTE Y, BYTE U, BYTE V)
{
    int C = INT(Y) - 16;
    int D = INT(U) - 128;
    int E = INT(V) - 128;

    INT R = ( 298 * C           + 409 * E + 128) >> 8;
    INT G = ( 298 * C - 100 * D - 208 * E + 128) >> 8;
    INT B = ( 298 * C + 516 * D           + 128) >> 8;

    DWORD ret = 0xff000000;
    BYTE *bCols = reinterpret_cast<BYTE*>(&ret);

    bCols[2] = Clip(R);
    bCols[1] = Clip(G);
    bCols[0] = Clip(B);

    return ret;
}

class CFrameGenerator
{
public:
    CFrameGenerator() { }
    virtual ~CFrameGenerator() { }

    HRESULT PrepareFrame(BYTE *pBuf, LONGLONG llTimestamp, LONG lPitch)
    {
        ZeroMemory(pBuf, lPitch * c_dwOutputImageHeight);

        sin((llTimestamp/100000.0)*3.1415);
        return DrawFrame(pBuf, YUVToRGB(128, 128 + BYTE(127 * sin(llTimestamp/10000000.0)), 128 + BYTE(127 * cos(llTimestamp/3300000.0))), lPitch);
    }

protected:
    virtual HRESULT DrawFrame(BYTE *pBuf, DWORD dwColor, LONG lPitch) = 0;

    HRESULT SetColor(_In_reads_bytes_(cElements) DWORD *pBuf, DWORD dwColor, DWORD cElements)
    {
        for(DWORD nIndex = 0; nIndex < cElements; ++nIndex)
        {
            pBuf[nIndex] = dwColor;
        }
        return S_OK;
    }

private:
    CFrameGenerator(const CFrameGenerator& nonCopyable);
    CFrameGenerator& operator=(const CFrameGenerator& nonCopyable);
};

class CSquareDrawer : public CFrameGenerator
{
protected:
    HRESULT DrawFrame(BYTE *pBuf, DWORD dwColor, LONG lPitch)
    {
        HRESULT hr = S_OK;

        const DWORD dwDimension = min(c_dwOutputImageWidth, c_dwOutputImageHeight);
        const int nFirstLine = (c_dwOutputImageHeight-dwDimension)/2;    
        const int nStartPos = (c_dwOutputImageWidth-dwDimension)/2;

        for (int nLine = 0; nLine < dwDimension; ++nLine, pBuf += lPitch)
        {
            DWORD* pLine = reinterpret_cast<DWORD *>(pBuf) + nStartPos;
            hr = SetColor(pLine, dwColor, dwDimension);
            if (FAILED(hr))
            {
                break;
            }
        }

        return hr;
    }
};

class CCircleDrawer : public CFrameGenerator
{
protected:
    HRESULT DrawFrame(BYTE *pBuf, DWORD dwColor, LONG lPitch)
    {
        HRESULT hr = S_OK;

        const int dwDimension = min(c_dwOutputImageWidth, c_dwOutputImageHeight);
        const int dwRadius = dwDimension/2;
        const int nFirstLine = (c_dwOutputImageHeight-dwDimension)/2;    
        const int nStartPos = (c_dwOutputImageWidth-dwDimension)/2;

        for (int nLine = -dwRadius; nLine < dwRadius; ++nLine, pBuf += lPitch)
        {
            const int nXPos = (int)sqrt(dwRadius*(double)dwRadius - nLine*(double)nLine);
            const int nStartPos = (c_dwOutputImageWidth / 2) - nXPos;
            const int cPixels = nXPos * 2;
            DWORD* pLine = reinterpret_cast<DWORD *>(pBuf) + nStartPos;

            hr = SetColor(pLine, dwColor, cPixels);
            if (FAILED(hr))
            {
                break;
            }
        }

        return hr;
    }
};

class CTriangleDrawer : public CFrameGenerator
{
protected:
    HRESULT DrawFrame(BYTE *pBuf, DWORD dwColor, LONG lPitch)
    {
        HRESULT hr = S_OK;

        const DWORD dwDimension = min(c_dwOutputImageWidth, c_dwOutputImageHeight);

        const int nFirstLine = (c_dwOutputImageHeight-dwDimension)/2;    
        const int nStartPos = (c_dwOutputImageWidth-dwDimension)/2;

        int nLeft = c_dwOutputImageWidth / 2;
        int nRight = nLeft + 1;
        const int cLinesPerPixel = 2;

        for (int nLine = 0, nLinesToGrow = 1; nLine < dwDimension; ++nLine, pBuf += lPitch, --nLinesToGrow)
        {
            if (nLinesToGrow == 0)
            {
                nLinesToGrow = cLinesPerPixel;
                --nLeft;
                ++nRight;
            }

            DWORD* pLine = reinterpret_cast<DWORD *>(pBuf) + nLeft;
            hr = SetColor(pLine, dwColor, nRight - nLeft);
            if (FAILED(hr))
            {
                break;
            }
        }

        return hr;
    }
};

HRESULT CreateFrameGenerator(GeometricShape eShape, CFrameGenerator **ppFrameGenerator)
{
    HRESULT hr = S_OK;

    switch(eShape)
    {
    case GeometricShape_Square:
        {
            *ppFrameGenerator = new CSquareDrawer();
            break;
        }
    case GeometricShape_Circle:
        {
            *ppFrameGenerator = new CCircleDrawer();
            break;
        }
    case GeometricShape_Triangle:
        {
            *ppFrameGenerator = new CTriangleDrawer();
            break;
        }
    default:
        {
            hr = E_INVALIDARG;
            break;
        }
    }

    if (SUCCEEDED(hr) && *ppFrameGenerator == nullptr)
    {
        hr = E_OUTOFMEMORY;
    }

    return hr;
}

HRESULT CGeometricMediaStream::CreateInstance(CGeometricMediaSource *pSource, GeometricShape eShape, CGeometricMediaStream **ppStream)
{
    if (pSource == nullptr || ppStream == nullptr)
    {
        return E_INVALIDARG;
    }

    HRESULT hr = S_OK;
    ComPtr<CGeometricMediaStream> spStream;
    spStream.Attach(new(std::nothrow) CGeometricMediaStream(pSource, eShape));
    if (!spStream)
    {
        hr = E_OUTOFMEMORY;
    }

    if (SUCCEEDED(hr))
    {
        hr = spStream->Initialize();
    }

    if (SUCCEEDED(hr))
    {
        (*ppStream) = spStream.Detach();
    }

    return hr;
}

CGeometricMediaStream::CGeometricMediaStream(CGeometricMediaSource *pSource, GeometricShape eShape)
    : _cRef(1)
    , _spSource(pSource)
    , _eSourceState(SourceState_Invalid)
    , _eShape(eShape)
    , _flRate(1.0f)
{
    auto module = ::Microsoft::WRL::GetModuleBase();
    if (module != nullptr)
    {
        module->IncrementObjectCount();
    }
}


CGeometricMediaStream::~CGeometricMediaStream(void)
{
    auto module = ::Microsoft::WRL::GetModuleBase();
    if (module != nullptr)
    {
        module->DecrementObjectCount();
    }
}

// IUnknown methods

IFACEMETHODIMP CGeometricMediaStream::QueryInterface(REFIID riid, void** ppv)
{
    if (ppv == nullptr)
    {
        return E_POINTER;
    }
    HRESULT hr = E_NOINTERFACE;
    (*ppv) = nullptr;
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

IFACEMETHODIMP_(ULONG) CGeometricMediaStream::AddRef()
{
    return InterlockedIncrement(&_cRef);
}

IFACEMETHODIMP_(ULONG) CGeometricMediaStream::Release()
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

IFACEMETHODIMP CGeometricMediaStream::BeginGetEvent(IMFAsyncCallback* pCallback, IUnknown* punkState)
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

IFACEMETHODIMP CGeometricMediaStream::EndGetEvent(IMFAsyncResult* pResult, IMFMediaEvent** ppEvent)
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

IFACEMETHODIMP CGeometricMediaStream::GetEvent(DWORD dwFlags, IMFMediaEvent** ppEvent)
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

IFACEMETHODIMP CGeometricMediaStream::QueueEvent(MediaEventType met, REFGUID guidExtendedType, HRESULT hrStatus, PROPVARIANT const *pvValue)
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

IFACEMETHODIMP CGeometricMediaStream::GetMediaSource(IMFMediaSource** ppMediaSource)
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

IFACEMETHODIMP CGeometricMediaStream::GetStreamDescriptor(IMFStreamDescriptor** ppStreamDescriptor)
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

IFACEMETHODIMP CGeometricMediaStream::RequestSample(IUnknown* pToken)
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
        // Trigger sample delivery
        hr = DeliverSample(pToken);
    }

    if (FAILED(hr))
    {
        hr = HandleError(hr);
    }

    return hr;
}

HRESULT CGeometricMediaStream::Start()
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_eSourceState == SourceState_Stopped ||
            _eSourceState == SourceState_Started)
        {
            if (SUCCEEDED(hr))
            {
                if (_eSourceState == SourceState_Stopped)
                {
                    _llCurrentTimestamp = 0;
                }
                _eSourceState = SourceState_Started;

                // Inform the client that we've started
                hr = QueueEvent(MEStreamStarted, GUID_NULL, S_OK, nullptr);
            }
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

HRESULT CGeometricMediaStream::Stop()
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_eSourceState == SourceState_Started)
        {
            _eSourceState = SourceState_Stopped;
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

    return hr;
}

HRESULT CGeometricMediaStream::SetRate(float flRate)
{
    HRESULT hr = S_OK;
    CSourceLock lock(_spSource.Get());

    hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        _flRate = flRate;
    }

    if (FAILED(hr))
    {
        hr = HandleError(hr);
    }

    return hr;
}

HRESULT CGeometricMediaStream::Shutdown()
{
    CSourceLock lock(_spSource.Get());

    HRESULT hr = CheckShutdown();

    if (SUCCEEDED(hr))
    {
        if (_spEventQueue)
        {
            _spEventQueue->Shutdown();
            _spEventQueue.ReleaseAndGetAddressOf();
        }

        if (_spAllocEx != nullptr)
        {
            _spAllocEx->UninitializeSampleAllocator();
        }

        _spStreamDescriptor.ReleaseAndGetAddressOf();
        _spDeviceManager.ReleaseAndGetAddressOf();
        _eSourceState = SourceState_Shutdown;

        delete _pFrameGenerator;
        _pFrameGenerator = nullptr;
    }

    return hr;
}

HRESULT CGeometricMediaStream::SetDXGIDeviceManager(IMFDXGIDeviceManager *pManager)
{
    HRESULT hr = S_OK;

    _spDeviceManager = pManager;

    if (_spDeviceManager)
    {
        hr = CreateVideoSampleAllocator();
    }

    return hr;
}

HRESULT CGeometricMediaStream::Initialize()
{
    // Create the media event queue.
    HRESULT hr = MFCreateEventQueue(&_spEventQueue);
    ComPtr<IMFStreamDescriptor> spSD;
    ComPtr<IMFMediaTypeHandler> spMediaTypeHandler;

    if (SUCCEEDED(hr))
    {
        // Create a media type object.
        hr = CreateMediaType(&_spMediaType);
    }

    if (SUCCEEDED(hr))
    {
        // Now we can create MF stream descriptor.
        hr = MFCreateStreamDescriptor(c_dwGeometricStreamId, 1, _spMediaType.GetAddressOf(), &spSD);
    }

    if (SUCCEEDED(hr))
    {
        hr = spSD->GetMediaTypeHandler(&spMediaTypeHandler);
    }

    if (SUCCEEDED(hr))
    {
        // Set current media type
        hr = spMediaTypeHandler->SetCurrentMediaType(_spMediaType.Get());
    }

    if (SUCCEEDED(hr))
    {
        _spStreamDescriptor = spSD;
        // State of the stream is started.
        _eSourceState = SourceState_Stopped;
    }

    if (SUCCEEDED(hr))
    {
        hr = CreateFrameGenerator(_eShape, &_pFrameGenerator);
    }

    return hr;
}

HRESULT CGeometricMediaStream::CreateMediaType(IMFMediaType **ppMediaType)
{
    ComPtr<IMFMediaType> spOutputType;
    HRESULT hr = MFCreateMediaType(&spOutputType);

    if (SUCCEEDED(hr))
    {
        hr = spOutputType->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Video);
    }

    if (SUCCEEDED(hr))
    {
        hr = spOutputType->SetGUID(MF_MT_SUBTYPE, MFVideoFormat_ARGB32);
    }

    if (SUCCEEDED(hr))
    {
        hr = spOutputType->SetUINT32(MF_MT_FIXED_SIZE_SAMPLES, TRUE);
    }

    if (SUCCEEDED(hr))
    {
        hr = spOutputType->SetUINT32(MF_MT_ALL_SAMPLES_INDEPENDENT, TRUE);
    }

    if (SUCCEEDED(hr))
    {
        hr = spOutputType->SetUINT32(MF_MT_SAMPLE_SIZE, c_cbOutputSampleSize);
    }

    if (SUCCEEDED(hr))
    {
        hr = MFSetAttributeSize(spOutputType.Get(), MF_MT_FRAME_SIZE, c_dwOutputImageWidth, c_dwOutputImageHeight);
    }

    if (SUCCEEDED(hr))
    {
        hr = MFSetAttributeRatio(spOutputType.Get(), MF_MT_FRAME_RATE, c_dwOutputFrameRateNumerator, c_dwOutputFrameRateDenominator);
    }

    if (SUCCEEDED(hr))
    {
        hr = spOutputType->SetUINT32(MF_MT_INTERLACE_MODE, MFVideoInterlace_Progressive);
    }

    if (SUCCEEDED(hr))
    {
        hr = MFSetAttributeRatio(spOutputType.Get(), MF_MT_PIXEL_ASPECT_RATIO, 1, 1);
    }

    if (SUCCEEDED(hr))
    {
        *ppMediaType = spOutputType.Detach();
    }

    return hr;
}

HRESULT CGeometricMediaStream::DeliverSample(IUnknown *pToken)
{
    ComPtr<IMFSample> spSample;

    HRESULT hr = CreateImage(_eShape, &spSample);

    if (SUCCEEDED(hr))
    {
        hr = spSample->SetSampleTime(_llCurrentTimestamp);
        _llCurrentTimestamp += c_llOutputFrameDuration;
    }

    if (SUCCEEDED(hr))
    {
        hr = spSample->SetSampleDuration(c_llOutputFrameDuration);
    }

    if (SUCCEEDED(hr) && pToken != nullptr)
    {
        // If token was not null set the sample attribute.
        hr = spSample->SetUnknown(MFSampleExtension_Token, pToken);
    }

    if (SUCCEEDED(hr))
    {
        // Send a sample event.
        hr = _spEventQueue->QueueEventParamUnk(MEMediaSample, GUID_NULL, S_OK, spSample.Get());
    }

    return hr;
}

HRESULT CGeometricMediaStream::CreateImage(GeometricShape eShape, IMFSample **ppSample)
{
    ComPtr<IMFMediaBuffer> spOutputBuffer;
    ComPtr<IMF2DBuffer2> sp2DBuffer;
    ComPtr<IMFSample> spSample;
    ComPtr<ID3D11Texture2D> spTexture2D;
    BYTE *pBuf = nullptr;
    LONG pitch = c_dwOutputImageWidth * sizeof(DWORD);

    HRESULT hr = S_OK;

    if (_pFrameGenerator == nullptr)
    {
        hr = E_UNEXPECTED;
    }

    if (SUCCEEDED(hr))
    {
        if (_spDeviceManager)
        {
            hr = _spAllocEx->AllocateSample(&spSample);

            if (SUCCEEDED(hr))
            {
                hr = spSample->GetBufferByIndex(0, &spOutputBuffer);
            }
        }
        else
        {
            hr = MFCreateMemoryBuffer(c_cbOutputSampleSize, &spOutputBuffer);

            if (SUCCEEDED(hr))
            {
                hr = MFCreateSample(&spSample);
            }

            if (SUCCEEDED(hr))
            {
                hr = spSample->AddBuffer(spOutputBuffer.Get());
            }
        }
    }
    
    if (SUCCEEDED(hr))
    {
        spOutputBuffer.As(&sp2DBuffer);
    }

    if (SUCCEEDED(hr))
    {
        BYTE* pBufferStart;
        DWORD cLen;
        if (sp2DBuffer)
        {
            hr = sp2DBuffer->Lock2DSize(MF2DBuffer_LockFlags_Write, &pBuf, &pitch, &pBufferStart, &cLen);
        }
        else
        {
            hr = spOutputBuffer->Lock(&pBuf, nullptr, nullptr);
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = _pFrameGenerator->PrepareFrame(pBuf, _llCurrentTimestamp, pitch);
    }

    if (SUCCEEDED(hr) && !sp2DBuffer)
    {
        hr = spOutputBuffer->SetCurrentLength(c_cbOutputSampleSize);
    }

    if (pBuf != nullptr)
    {
        if (sp2DBuffer)
        {
            sp2DBuffer->Unlock2D();
        }
        else
        {
            spOutputBuffer->Unlock();
        }
    }

    if (SUCCEEDED(hr))
    {
        *ppSample = spSample.Detach();
    }

    return hr;
}

HRESULT CGeometricMediaStream::HandleError(HRESULT hErrorCode)
{
    if (hErrorCode != MF_E_SHUTDOWN)
    {
        // Send MEError to the client
        hErrorCode = QueueEvent(MEError, GUID_NULL, hErrorCode, nullptr);
    }

    return hErrorCode;
}

HRESULT CGeometricMediaStream::CreateVideoSampleAllocator()
{
    HRESULT hr = S_OK;
    ComPtr<IMFAttributes> spAttributes;

    hr = MFCreateAttributes(&spAttributes, 1);

    if (SUCCEEDED(hr))
    {
        spAttributes->SetUINT32( MF_SA_D3D11_BINDFLAGS, D3D11_BIND_SHADER_RESOURCE );
        hr = MFCreateVideoSampleAllocatorEx(IID_IMFVideoSampleAllocatorEx, (void**)&_spAllocEx);
    }
    if (SUCCEEDED(hr))
    {
        hr = _spAllocEx->SetDirectXManager(_spDeviceManager.Get());
    }
    if (SUCCEEDED(hr))
    {
        hr = _spAllocEx->InitializeSampleAllocatorEx(1, 4, spAttributes.Get(), _spMediaType.Get());
    }

    return hr;
}
