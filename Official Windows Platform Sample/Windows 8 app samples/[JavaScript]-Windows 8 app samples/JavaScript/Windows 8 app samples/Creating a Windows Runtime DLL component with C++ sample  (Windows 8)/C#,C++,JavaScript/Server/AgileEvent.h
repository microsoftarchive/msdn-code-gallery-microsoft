//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

namespace Microsoft { 
namespace SDKSamples {
namespace Details {

class GlobalInterfaceTable
{
public:
    GlobalInterfaceTable()
    {
        ::InitOnceInitialize(&_initOnce);
    }

    HRESULT Initialize()
    {
        HRESULT hr = S_OK;
        if(!::InitOnceExecuteOnce(&_initOnce, &CreateGlobalInterfaceTable, this, nullptr))
        {
            hr = HRESULT_FROM_WIN32(::GetLastError());
        }
        return hr;
    }

    operator IGlobalInterfaceTable* () const { return _spGlobalInterfaceTable.Get(); }
    IGlobalInterfaceTable* operator->() const { return _spGlobalInterfaceTable.Get(); }

private:
    static BOOL WINAPI CreateGlobalInterfaceTable(_Inout_ PINIT_ONCE, _Inout_ PVOID context, _Unreferenced_parameter_ PVOID*)
    {
        HRESULT hr = ::CoCreateInstance(CLSID_StdGlobalInterfaceTable, nullptr, CLSCTX_INPROC, IID_PPV_ARGS(reinterpret_cast<GlobalInterfaceTable*>(context)->_spGlobalInterfaceTable.ReleaseAndGetAddressOf()));
        return SUCCEEDED(hr);
    }

    INIT_ONCE _initOnce;
    ::Microsoft::WRL::ComPtr<IGlobalInterfaceTable> _spGlobalInterfaceTable;
};

__declspec(selectany) GlobalInterfaceTable _gGlobalInterfaceTable;

class AgilePtr
{
public:
    AgilePtr() : 
        _bread(0)
    {
        _hrInit = Details::_gGlobalInterfaceTable.Initialize();
    }

    virtual ~AgilePtr()
    {
        if (_bread != 0)
        {
            Details::_gGlobalInterfaceTable->RevokeInterfaceFromGlobal(_bread);
        }
    }

    template <typename U>
    HRESULT Initialize(_In_ U* ptr)
    {
        if (_bread != 0)
        {
            return E_UNEXPECTED;
        }

        HRESULT hr = _hrInit;
        if (ptr != nullptr)
        {
            hr = Register(ptr);
        }
        return hr;
    }

    template <typename U>
    HRESULT CopyLocal(_Out_ ::Microsoft::WRL::Details::ComPtrRef<::Microsoft::WRL::ComPtr<U>> ptr)
    {
        return Localize(__uuidof(U), ptr);
    }

protected:
    template <typename U>
    HRESULT Register(_In_ U* ptr)
    {
        HRESULT hr = _hrInit;
        if (SUCCEEDED(hr))
        {
            hr = Details::_gGlobalInterfaceTable->RegisterInterfaceInGlobal(ptr, __uuidof(U), &_bread);
        }
        return hr;
    }

    HRESULT Localize(REFIID riid, _COM_Outptr_ void** ptr)
    {
        HRESULT hr = S_OK; 
        if (_bread != 0)
        {
            hr = _hrInit;
            if (SUCCEEDED(hr))
            {
                hr = Details::_gGlobalInterfaceTable->GetInterfaceFromGlobal(_bread, riid, ptr);
            }
        }
        else
        {
            hr = E_INVALIDARG;
        }

        if (FAILED(hr))
        {
            *ptr = nullptr;
        }

        return hr;
    }

private:
    AgilePtr(const AgilePtr& rhs);
    AgilePtr& operator=(const AgilePtr& rhs);
    DWORD _bread;
    HRESULT _hrInit;
};

template<typename TDelegateInterface, unsigned int argCount>
struct AgileInvokeHelper;

template<typename TDelegateInterface>
struct AgileInvokeHelper<TDelegateInterface, 2> : public ::Microsoft::WRL::RuntimeClass<
    ::Microsoft::WRL::RuntimeClassFlags<::Microsoft::WRL::Delegate>, TDelegateInterface>
{
    typedef typename ::Microsoft::WRL::Details::ArgTraitsHelper<TDelegateInterface>::Traits Traits;

public:
    HRESULT RuntimeClassInitialize(_In_ TDelegateInterface *delegateInterface)
    {
        return _spDelegate.Initialize(delegateInterface);
    }

    virtual HRESULT STDMETHODCALLTYPE Invoke(
        typename Traits::Arg1Type arg1,
        typename Traits::Arg2Type arg2)
    {
        ComPtr<TDelegateInterface> spLocalDelegate;
        HRESULT hr = _spDelegate.CopyLocal(&spLocalDelegate);
        if (SUCCEEDED(hr))
        {
            spLocalDelegate->Invoke(arg1, arg2);
        }
        return hr;
    }

private:
    AgilePtr _spDelegate;
};

template<typename TDelegateInterface>
HRESULT CreateAgileHelper(_In_ TDelegateInterface *delegateInterface, _COM_Outptr_ TDelegateInterface** wrapper)
{
    *wrapper = nullptr;
    ComPtr<AgileInvokeHelper<TDelegateInterface, ::Microsoft::WRL::Details::ArgTraitsHelper<TDelegateInterface>::args>> spInvokeHelper;

    static_assert(__is_base_of(IUnknown, TDelegateInterface) && !__is_base_of(IInspectable, TDelegateInterface), "Delegates objects must be 'IUnknown' base and not 'IInspectable'");

    HRESULT hr = ::Microsoft::WRL::MakeAndInitialize<AgileInvokeHelper<TDelegateInterface, ::Microsoft::WRL::Details::ArgTraitsHelper<TDelegateInterface>::args>>(&spInvokeHelper, delegateInterface);

    if (SUCCEEDED(hr))
    {
        hr = spInvokeHelper.CopyTo(wrapper);
    }
    return hr;
}

} // Details

template<typename TDelegateInterface>
class AgileEventSource : public ::Microsoft::WRL::EventSource<TDelegateInterface>
{
public:
    HRESULT Add(_In_opt_ TDelegateInterface* delegateInterface, _Out_ EventRegistrationToken* token)
    {
        if (delegateInterface == nullptr)
        {
            return E_INVALIDARG;
        }

        ::Microsoft::WRL::ComPtr<TDelegateInterface> spAgileCallback;
        HRESULT hr = Details::CreateAgileHelper<TDelegateInterface>(delegateInterface, &spAgileCallback);
        if (SUCCEEDED(hr))
        {
              hr = EventSource<TDelegateInterface>::Add(spAgileCallback.Get(), token);
        }
        return hr;
    }
};

}
}
