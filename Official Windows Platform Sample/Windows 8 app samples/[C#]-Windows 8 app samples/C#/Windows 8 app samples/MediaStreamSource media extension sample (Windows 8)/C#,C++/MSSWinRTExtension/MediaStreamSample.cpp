#include "pch.h"
#include "MediaStreamSample.h"
#include "Buffer.h"


CMediaStreamSample::CMediaStreamSample(void)
    : _pLockedBuffer(nullptr)
    , _nOffset(0)
    , _cCount(0)
    , _llTimestamp(0)
    , _llDuration(0)
{
}


CMediaStreamSample::~CMediaStreamSample(void)
{
}

HRESULT CMediaStreamSample::RuntimeClassInitialize(ABI::Windows::Storage::Streams::IBuffer *pBuffer,
    long nOffset,
    long cCount,
    LONGLONG llTimestamp,
    LONGLONG llDuration)
{
    if (pBuffer == nullptr)
    {
        return E_POINTER;
    }

    _spBuffer = pBuffer;
    _nOffset = nOffset;
    _cCount = cCount;
    _llTimestamp = llTimestamp;
    _llDuration = llDuration;

    return S_OK;
}

// IMediaStreamSample
IFACEMETHODIMP CMediaStreamSample::get_Timestamp ( 
    _Out_ LONGLONG *pllTimestamp)
{
    if (pllTimestamp == nullptr)
    {
        return E_POINTER;
    }

    *pllTimestamp = _llTimestamp;

    return S_OK;
}

IFACEMETHODIMP CMediaStreamSample::get_Duration ( 
    _Out_ LONGLONG *pllDuration)
{
    if (pllDuration == nullptr)
    {
        return E_POINTER;
    }

    *pllDuration = _llDuration;

    return S_OK;
}

// IMFMediaBuffer
IFACEMETHODIMP CMediaStreamSample::Lock( 
    _Outptr_result_bytebuffer_to_(*pcbMaxLength, *pcbCurrentLength)  BYTE **ppbBuffer,
    _Out_opt_  DWORD *pcbMaxLength,
    _Out_opt_  DWORD *pcbCurrentLength)
{
    if (ppbBuffer == nullptr)
    {
        return E_POINTER;
    }

    if (_pLockedBuffer != nullptr)
    {
        return MF_E_INVALIDREQUEST;
    }

    ComPtr<Windows::Storage::Streams::IBufferByteAccess> spBuffer;

    HRESULT hr = _spBuffer.As(&spBuffer);
    if (SUCCEEDED(hr))
    {
        hr = spBuffer->Buffer(&_pLockedBuffer); 
    }

    *ppbBuffer = _pLockedBuffer + _nOffset;
    if (pcbMaxLength != nullptr)
    {
        *pcbMaxLength = _cCount;
    }
    if (pcbCurrentLength != nullptr)
    {
        *pcbCurrentLength = _cCount;
    }

    return hr;
}

IFACEMETHODIMP CMediaStreamSample::Unlock(void)
{
    if (_pLockedBuffer == nullptr)
    {
        return MF_E_INVALIDREQUEST;
    }
    _pLockedBuffer = nullptr;

    return S_OK;
}

IFACEMETHODIMP CMediaStreamSample::GetCurrentLength( 
    _Out_  DWORD *pcbCurrentLength)
{
    if (pcbCurrentLength == nullptr)
    {
        return E_POINTER;
    }

    *pcbCurrentLength = _cCount;

    return S_OK;
}

IFACEMETHODIMP CMediaStreamSample::SetCurrentLength( 
    DWORD cbCurrentLength)
{
    return E_NOTIMPL;
}

IFACEMETHODIMP CMediaStreamSample:: GetMaxLength( 
    _Out_  DWORD *pcbMaxLength)
{
    if (pcbMaxLength == nullptr)
    {
        return E_POINTER;
    }

    *pcbMaxLength = _cCount;

    return S_OK;
}

IFACEMETHODIMP CMediaStreamSampleFactory:: ActivateInstance(_Outptr_result_nullonfailure_ IInspectable **ppvObject)
{
    if (ppvObject == nullptr)
    {
        return E_POINTER;
    }
    *ppvObject = nullptr;

    return E_NOTIMPL;
}

IFACEMETHODIMP CMediaStreamSampleFactory:: CreateSample(ABI::Windows::Storage::Streams::IBuffer *pBuffer,//IInspectable *pBuffer,
    long nOffset,
    long cCount,
    LONGLONG llTimestamp,
    LONGLONG llDuration,
    ABI::MSSWinRTExtension::IMediaStreamSample **ppSample)
{
    //ComPtr<IInspectable> spInsp = pBuffer;
    //ComPtr<IBuffer> spBuffer;

    HRESULT hr = S_OK;//spInsp.As(&spBuffer);
    if (SUCCEEDED(hr))
    {
        hr = MakeAndInitialize<CMediaStreamSample>(ppSample,
        pBuffer, nOffset, cCount, llTimestamp, llDuration);
    }

    return hr;
}
