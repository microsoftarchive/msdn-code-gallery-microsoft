//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include <Unknwn.h>
#include "ComPtr.h"

// Used for initialization of a COM Object
// Implement this interface if you require your COMObject 
// to do initialization after it's created.
[uuid("0B3B5912-31A9-49C2-B195-DCABB870AB59")]
__interface IInitializable : public IUnknown
{
    // This function is called right after the object is constructed
    // WARNING: Don't call AddRef() or Release() for the current object
    // inside this function 
    HRESULT __stdcall Initialize();
};

//
// This class encapsulates an IUnknown based object,
// and implements AddRef, Relase and QueryInterface
// However, QueryInterface requires a helper function: T::QueryInterfaceHelper(IID const&, void**)
// that must be implented by all classes using the COMObject
// WARNING: Don't call AddRef() or Release() in the current object's constructor
//
template <class T>
class SharedObject: public T
{
public:
    template <class I>
    static HRESULT Create(__out I** object)
    {
        static_assert(std::tr1::is_base_of<I, T>::value, "Template arg T must derive from I");

        ComPtr<I> comPtr = new (std::nothrow) SharedObject<T>();
        if (!comPtr)
        {
            return E_FAIL;
        }

        return InitializeHelper(comPtr, object);
    }

    template <class I, class TArg1>
    static HRESULT Create(TArg1 &&arg1, __out I** object)
    {
        static_assert(std::tr1::is_base_of<I, T>::value, "Template arg T must derive from I");

        ComPtr<I> comPtr = new (std::nothrow) SharedObject<T>(arg1);
        if (!comPtr)
        {
            return E_FAIL;
        }

        return InitializeHelper(comPtr, object);
    }

    template <class I, class TArg1, class TArg2>
    static HRESULT Create(TArg1 &&arg1, TArg2 &&arg2, __out I** object)
    {
        static_assert(std::tr1::is_base_of<I, T>::value, "Template arg T must derive from I");

        ComPtr<I> comPtr = new (std::nothrow) SharedObject<T>(arg1, arg2);
        if (!comPtr)
        {
            return E_FAIL;
        }

        return InitializeHelper(comPtr, object);
    }

private:
    template <class I>
    static HRESULT InitializeHelper(const ComPtr<I>& comPtr, __out I** object)
    {
        static_assert(std::tr1::is_base_of<I, T>::value, "Template arg T must derive from I");

        assert(object);
        *object = nullptr;

        HRESULT hr = S_OK;
        ComPtr<IInitializable> initializer;

        if (SUCCEEDED(comPtr->QueryInterface(IID_PPV_ARGS(&initializer))))
        {
            hr = initializer->Initialize();
        }

        if (SUCCEEDED(hr))
        {
            hr = AssignToOutputPointer(object, comPtr);
        }

        assert(SUCCEEDED(hr));

        return hr;
    }

    SharedObject(): T(), m_refCount(0)
    {
    }

    template <class TArg1>
    SharedObject(TArg1 &&arg1)
        : T(arg1), m_refCount(0)
    {
    }

    template <class TArg1, class TArg2>
    SharedObject(TArg1 &&arg1, TArg2 &&arg2)
        : T(arg1, arg2), m_refCount(0)
    {
    }

    HRESULT __stdcall QueryInterface(const IID &iid, void **object)
    {
        if (nullptr == object)
        {
            return E_POINTER;
        }

        *object = nullptr;

        if (T::QueryInterfaceHelper(iid, object))
        {
            static_cast<IUnknown*>(*object)->AddRef();
            return S_OK;
        }

        return E_NOINTERFACE;
    }


    unsigned long __stdcall AddRef()
    {
        return static_cast<unsigned long>(
            InterlockedIncrement(reinterpret_cast<long*>(&m_refCount)));
    }

    unsigned long __stdcall Release()
    {
        unsigned long refCount = static_cast<unsigned long>(
            InterlockedDecrement(reinterpret_cast<long*>(&m_refCount)));

        if (refCount == 0)
        {
            delete this;
        }

        return refCount;
    }

private:
    // The variable holding the ref count
    unsigned long m_refCount;
};

template <typename I>
struct CastHelper
{
    template <typename T>
    static bool CastTo(const IID &iid, T* objPtr, void **interfacePtr)
    {
        if (__uuidof(I) == iid || IID_IUnknown == iid)
        {
            *interfacePtr = static_cast<I*>(objPtr);
            return true;
        }
        return false;
    }
};
