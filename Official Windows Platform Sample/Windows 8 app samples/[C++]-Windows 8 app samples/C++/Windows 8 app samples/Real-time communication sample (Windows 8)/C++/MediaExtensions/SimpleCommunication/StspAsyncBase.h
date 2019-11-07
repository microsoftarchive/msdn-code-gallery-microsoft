//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once
#include <AsyncCB.h>
#include <wrl/async.h>

namespace Stsp {
template <typename TOperation, typename TCompletedEventHandler>
class CAsyncBase :
    public Microsoft::WRL::RuntimeClass<TOperation, Microsoft::WRL::AsyncBase<TCompletedEventHandler>>
{
public:
    CAsyncBase() 
        : _thisCookie(0)
        , _handlerCookie(0)
#pragma warning(push)
#pragma warning(disable: 4355)
        , _OnPostCompletionCB(this, &CAsyncBase<TOperation, TCompletedEventHandler>::OnPostCompletion)
#pragma warning(pop)

    {
    }

    virtual ~CAsyncBase()
    {
    }

    // Complete event handler setter
    IFACEMETHODIMP put_Completed(_In_ TCompletedEventHandler *pCompleteHandler) override
    {
        HRESULT hr = CheckValidStateForDelegateCall();
        DWORD tempCookie = 0;
        ComPtr<TOperation> spThis;
        if (SUCCEEDED(hr))
        {
            hr = EnsureGIT();
        }

        if (SUCCEEDED(hr))
        {
            this->QueryInterface(IID_PPV_ARGS(spThis.ReleaseAndGetAddressOf()));
        }

        // Note that we have to handle the marshaling ourselves as AsyncBase just stores raw pointers and thus
        // ends up smuggling the delegate across threads when we invoke it. Consider switching to the public 
        // utilities when they are fixed in AsyncBase.
        if (SUCCEEDED(hr) && _thisCookie == 0)
        {
            // We only need to marshal the operation pointer once.
            hr = _spGIT->RegisterInterfaceInGlobal(
                spThis.Get(), __uuidof(TOperation), &_thisCookie);
        }

        if (SUCCEEDED(hr))
        {   
            hr = _spGIT->RegisterInterfaceInGlobal(
                pCompleteHandler, __uuidof(TCompletedEventHandler), &tempCookie);
        }

        if (SUCCEEDED(hr) && (InterlockedIncrement(&cCompleteDelegateAssigned_) == 1))
        {
            // The cookie should only be set once
            assert(_handlerCookie == 0);
            _handlerCookie = tempCookie;
        }
        else
        {
            _spGIT->RevokeInterfaceFromGlobal(tempCookie);
        }

        return hr;
    }

    // Complete event handler getter
    IFACEMETHODIMP get_Completed(_Outptr_result_maybenull_ TCompletedEventHandler **ppCompleteHandler) override
    {
        HRESULT hr = S_OK;
        if (ppCompleteHandler == nullptr)
        {
            hr = E_INVALIDARG;
        }
        else
        {
            *ppCompleteHandler = nullptr;
            hr = CheckValidStateForDelegateCall();
        }

        if (SUCCEEDED(hr))
        {
            hr = EnsureGIT();
        }
        if (SUCCEEDED(hr) && _handlerCookie != 0)
        {
            hr = _spGIT->GetInterfaceFromGlobal(_handlerCookie, IID_PPV_ARGS(ppCompleteHandler));
        }

        return hr;
    }

    IFACEMETHODIMP GetResults()
    {
        HRESULT hr = S_OK;
        get_ErrorCode(&hr);
        return hr;
    }

    void HandleAsyncCompletion(HRESULT hrStatus)
    {
        if (FAILED(hrStatus))
        {
            TryTransitionToError(hrStatus);
        }
        PostCompletion();
    }

protected:
    void FireCompletionMarshal()
    {
        TryTransitionToCompleted();

        Microsoft::WRL::ComPtr<TOperation> spOperation;
        Microsoft::WRL::ComPtr<TCompletedEventHandler> spHandler;

        if (SUCCEEDED(EnsureGIT()) && _handlerCookie != 0 && (InterlockedIncrement(&cCallbackMade_) == 1))
        {
            // Make sure _thisCookie is initialized
            assert(_thisCookie != 0);
            if (SUCCEEDED(_spGIT->GetInterfaceFromGlobal(_thisCookie, IID_PPV_ARGS(&spOperation))) &&
                SUCCEEDED(_spGIT->GetInterfaceFromGlobal(_handlerCookie, IID_PPV_ARGS(&spHandler))))
            {
                // We have both interfaces marshalled, now we can call the completion handler.
                Microsoft::WRL::Details::AsyncStatusInternal current = Microsoft::WRL::Details::_Undefined;
                CurrentStatus(&current);
                spHandler->Invoke(spOperation.Get(), static_cast<ABI::Windows::Foundation::AsyncStatus>(current));
            }

            _spGIT->RevokeInterfaceFromGlobal(_thisCookie);
            _spGIT->RevokeInterfaceFromGlobal(_handlerCookie);
            _thisCookie = 0;
            _handlerCookie = 0;
        }
    }

    void PostCompletion()
    {
        (void) MFPutWorkItem2( MFASYNC_CALLBACK_QUEUE_MULTITHREADED, 0, &_OnPostCompletionCB, nullptr );
    }

private:
    HRESULT EnsureGIT()
    {
        if (_spGIT == nullptr)
        {
            return CoCreateInstance(
                CLSID_StdGlobalInterfaceTable,
                nullptr,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(&_spGIT)
                );
        }

        return S_OK;
    }

    HRESULT OnPostCompletion( _In_ IMFAsyncResult *pResult )
    {
        FireCompletionMarshal();
        return S_OK;
    }

protected:
    AsyncCallback<CAsyncBase<TOperation, TCompletedEventHandler> > _OnPostCompletionCB;
    DWORD                               _thisCookie;
    DWORD                               _handlerCookie;
    ComPtr<IGlobalInterfaceTable>       _spGIT;
};
} // namespace Stsp
