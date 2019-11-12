//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

namespace Stsp {
template <typename TDelegateInterface>
class CDelegateWrapper :
    public IUnknown
{
public:
    static HRESULT CreateInstance(_In_ TDelegateInterface *pDelegate, _Outptr_ CDelegateWrapper<TDelegateInterface> **ppDelegateWrapper)
    {
        if (pDelegate == nullptr || ppDelegateWrapper == nullptr)
        {
            return E_INVALIDARG;
        }

        ComPtr<CDelegateWrapper<typename TDelegateInterface> > spWrapper;
        HRESULT hr = S_OK;

        spWrapper.Attach(new CDelegateWrapper<typename TDelegateInterface>());
        if (!spWrapper)
        {
            hr =  E_OUTOFMEMORY;
        }

        if (SUCCEEDED(hr))
        {
            hr = spWrapper->Initialize(pDelegate);
        }

        if (SUCCEEDED(hr))
        {
            *ppDelegateWrapper = spWrapper.Detach();
        }

        return hr;
    }

    // IUnknown
    IFACEMETHODIMP_(ULONG) AddRef() { 
        return InterlockedIncrement(&_cRef);
    }

    IFACEMETHODIMP_(ULONG) Release() { 
        LONG cRef = InterlockedDecrement(&_cRef);

        if (cRef == 0)
        {
            delete this;
        }
        return cRef;
    }

    IFACEMETHODIMP QueryInterface(REFIID iid, void** ppv)
    {
        if (!ppv)
        {
            return E_POINTER;
        }
        if (iid == __uuidof(IUnknown))
        {
            *ppv = static_cast<IUnknown*>(this);
        }
        else
        {
            *ppv = NULL;
            return E_NOINTERFACE;
        }
        AddRef();
        return S_OK;
    }

    template < typename T0, typename T1 > HRESULT Invoke(T0 arg0, T1 arg1) throw()
    {
        Microsoft::WRL::ComPtr<TDelegateInterface> spDelegate;

        if (_spGIT && SUCCEEDED(_spGIT->GetInterfaceFromGlobal(_delegateCookie, IID_PPV_ARGS(spDelegate.GetAddressOf()))))
        {
            // We have both interfaces marshalled, now we can call the completion handler.
            return spDelegate->Invoke(arg0, arg1);
        }
        return E_FAIL;
    }

private:
    CDelegateWrapper() 
        : _delegateCookie(0)
        , _cRef(1)
    {
    }

    virtual ~CDelegateWrapper()
    {
        _spGIT->RevokeInterfaceFromGlobal(_delegateCookie);
    }

    HRESULT Initialize(_In_ TDelegateInterface *pDelegate)
    {
        HRESULT hr = CoCreateInstance(
                CLSID_StdGlobalInterfaceTable,
                nullptr,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(&_spGIT)
                );

        if (SUCCEEDED(hr))
        {   
            hr = _spGIT->RegisterInterfaceInGlobal(
                pDelegate, __uuidof(TDelegateInterface), &_delegateCookie);
        }

        if (SUCCEEDED(hr))
        {
            _spDelegate = pDelegate;
        }

        return hr;
    }

protected:
    LONG                                _cRef;
    DWORD                               _delegateCookie;
    ComPtr<IGlobalInterfaceTable>       _spGIT;
    ComPtr<TDelegateInterface>          _spDelegate;
};

template <typename TDelegateInterface>
class CEventSource : 
     private Microsoft::WRL::EventSource<CDelegateWrapper<TDelegateInterface> >
{
public:
    typedef CDelegateWrapper<TDelegateInterface> _Wrapper;
    typedef Microsoft::WRL::EventSource<_Wrapper > _Base;

    HRESULT Add(_In_opt_ TDelegateInterface* delegateInterface, _Out_ EventRegistrationToken* token) throw()
    {
        ComPtr<_Wrapper> spWrapper;
        if (delegateInterface == nullptr)
        {
            return E_INVALIDARG;
        }
        
        HRESULT hr = _Wrapper::CreateInstance(delegateInterface, spWrapper.GetAddressOf());
        if (SUCCEEDED(hr))
        {
            hr = _Base::Add(spWrapper.Get(), token);
        }

        return hr;
    }

    HRESULT Remove(EventRegistrationToken token) throw()
    {
        return _Base::Remove(token);
    }

    HRESULT RemoveAll()
    {
        ComPtr<Details::EventTargetArray> pTempList;
        {
            // The addRemoveLock_ prevents multiple threads from doing simultaneous adds/removes.
            // An invoke may be occurring during an add or remove operation.
            Microsoft::WRL::Wrappers::SRWLock::SyncLockExclusive addRemoveLock = addRemoveLock_.LockExclusive();
            pTempList = Microsoft::WRL::Details::Move(targets_);
        }

        return S_OK;
    }


    template < typename T0, typename T1 > void InvokeAll(T0 &&arg0, T1 &&arg1) throw()
    {
        _Base::InvokeAll(Microsoft::WRL::Details::Forward<T0>(arg0), Microsoft::WRL::Details::Forward<T1>(arg1));
    }

    size_t GetSize() const throw()
    {
        Microsoft::WRL::Wrappers::SRWLock::SyncLockExclusive addRemoveLock = addRemoveLock_.LockExclusive();
        return targets_ == nullptr ? 0 : targets_->Length();
    }
};
} // namespace Stsp
