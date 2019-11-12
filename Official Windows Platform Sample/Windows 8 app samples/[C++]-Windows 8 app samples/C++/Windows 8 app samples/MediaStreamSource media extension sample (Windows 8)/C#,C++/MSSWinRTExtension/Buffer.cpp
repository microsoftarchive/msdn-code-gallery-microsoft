#include "pch.h"
#include "Buffer.h"

CBuffer::CBuffer(void)
    : _pbBuffer(nullptr)
    , _cbCapacity(0)
    , _cbLength(0)
{
}

CBuffer::~CBuffer(void)
{
    if (_pbBuffer != nullptr)
    {
        delete[] _pbBuffer;
        _pbBuffer = nullptr;
    }
}

HRESULT CBuffer::RuntimeClassInitialize(
    UINT cbCapacity)
{
    _pbBuffer = new BYTE[cbCapacity];
    if (_pbBuffer == nullptr)
    {
        return E_OUTOFMEMORY;
    }

    _cbCapacity = cbCapacity;
    _cbLength = 0;

    return S_OK;
}

// IBuffer
IFACEMETHODIMP CBuffer::get_Capacity(_Out_ UINT *pcbCapacity)
{
    if (pcbCapacity == nullptr)
    {
        return E_POINTER;
    }

    *pcbCapacity = _cbCapacity;

    return S_OK;
}

IFACEMETHODIMP CBuffer::get_Length(_Out_ UINT *pcbLength)
{
    if (pcbLength == nullptr)
    {
        return E_POINTER;
    }

    *pcbLength = _cbLength;

    return S_OK;
}

IFACEMETHODIMP CBuffer::put_Length(UINT cbLength)
{
    if (cbLength > _cbCapacity)
    {
        return E_INVALIDARG;
    }

    _cbLength = cbLength;

    return S_OK;
}

// IBufferByteAccess
IFACEMETHODIMP CBuffer::Buffer(_Out_ BYTE **ppBuffer)
{
    if (ppBuffer == nullptr)
    {
        return E_POINTER;
    }

    *ppBuffer = _pbBuffer;

    return S_OK;
}

// IMarshal
IFACEMETHODIMP CBuffer::GetUnmarshalClass(REFIID riid, _In_opt_ void *pv, DWORD dwDestContext, 
    _Reserved_ void *pvDestContext, DWORD mshlflags, _Out_ CLSID *pclsid)
{
    HRESULT hr = CheckMarshal();

    if (SUCCEEDED(hr))
    {
        hr = _spBufferMarshal->GetUnmarshalClass(riid, pv, dwDestContext, pvDestContext, mshlflags, pclsid);
    }

    return hr;
}

IFACEMETHODIMP CBuffer::GetMarshalSizeMax(REFIID riid, _In_opt_ void *pv, DWORD dwDestContext,
    _Reserved_ void *pvDestContext, DWORD mshlflags, _Out_ DWORD *pcbSize)
{
    HRESULT hr = CheckMarshal();

    if (SUCCEEDED(hr))
    {
        hr = _spBufferMarshal->GetMarshalSizeMax(riid, pv, dwDestContext, pvDestContext, mshlflags, pcbSize);
    }

    return hr;
}

IFACEMETHODIMP CBuffer::MarshalInterface(_In_ IStream *pStm, REFIID riid, _In_opt_ void *pv, DWORD dwDestContext,
    _Reserved_ void *pvDestContext, DWORD mshlflags)
{
    HRESULT hr = CheckMarshal();

    if (SUCCEEDED(hr))
    {
        hr = _spBufferMarshal->MarshalInterface(pStm,riid, pv, dwDestContext, pvDestContext, mshlflags);
    }

    return hr;
}

IFACEMETHODIMP CBuffer::UnmarshalInterface(_In_ IStream *, REFIID, _Deref_out_ void **)
{
    return E_NOTIMPL; 
}

IFACEMETHODIMP CBuffer::ReleaseMarshalData(_In_ IStream *)
{
    return E_NOTIMPL; 
}

IFACEMETHODIMP CBuffer::DisconnectObject(DWORD)
{
    return E_NOTIMPL; 
}

HRESULT CBuffer::CheckMarshal()
{
    HRESULT hr = S_OK;
    IMarshal **ppMarshal = _spBufferMarshal.GetAddressOf();
    if (*ppMarshal == nullptr)
    {
        IMarshal *pNewMarshal = nullptr;
        hr = RoGetBufferMarshaler(&pNewMarshal);

        if (SUCCEEDED(hr) &&
            nullptr != InterlockedCompareExchangePointer(reinterpret_cast<PVOID*>(ppMarshal), pNewMarshal, nullptr))
        {
            pNewMarshal->Release();
        }
    }

    return hr;
}
