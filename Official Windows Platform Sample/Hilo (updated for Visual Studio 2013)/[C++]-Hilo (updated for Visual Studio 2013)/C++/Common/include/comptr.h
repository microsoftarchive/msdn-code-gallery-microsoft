//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

//
// This structs acts as a smart pointer for IUnknown pointers
// making sure to call AddRef() and Release() as needed.
//
template<typename T>
struct ComPtr
{
public:

    ComPtr(T* lComPtr = nullptr) : m_ComPtr(lComPtr)
    {
        static_assert(std::tr1::is_base_of<IUnknown, T>::value, "T needs to be IUnknown based");
        if (m_ComPtr)
        {
            m_ComPtr->AddRef();
        }
    }

    ComPtr(const ComPtr<T>& lComPtrObj)
    {
        static_assert(std::tr1::is_base_of<IUnknown, T>::value, "T needs to be IUnknown based");
        m_ComPtr = lComPtrObj.m_ComPtr;

        if (m_ComPtr)
        {
            m_ComPtr->AddRef();
        }
    }

    ComPtr(ComPtr<T>&& lComPtrObj)
    {
        m_ComPtr = lComPtrObj.m_ComPtr;
        lComPtrObj.m_ComPtr = nullptr;
    }

    T* operator=(T* lComPtr)
    {
        if (m_ComPtr)
        {
            m_ComPtr->Release();
        }

        m_ComPtr = lComPtr;

        if (m_ComPtr)
        {
            m_ComPtr->AddRef();
        }

        return m_ComPtr;
    }

    T* operator=(const ComPtr<T>& lComPtrObj)
    {
        if (m_ComPtr)
        {
            m_ComPtr->Release();
        }

        m_ComPtr = lComPtrObj.m_ComPtr;

        if (m_ComPtr)
        {
            m_ComPtr->AddRef();
        }

        return m_ComPtr;
    }

    ~ComPtr()
    {
        if (m_ComPtr)
        {
            m_ComPtr->Release();
            m_ComPtr = nullptr;
        }
    }

    operator T*() const
    {
        return m_ComPtr;
    }

    T* GetInterface() const
    {
        return m_ComPtr;
    }

    T& operator*() const
    {
        return *m_ComPtr;
    }

    T** operator&()
    {
        //The assert on operator& usually indicates a bug. Could be a potential memory leak.
        // If this really what is needed, however, use GetInterface() explicitly.
        assert(nullptr == m_ComPtr);
        return &m_ComPtr;
    }

    T* operator->() const
    {
        return m_ComPtr;
    }

    bool operator!() const
    {    
        return (nullptr == m_ComPtr);
    }

    bool operator<(T* lComPtr) const
    {
        return m_ComPtr < lComPtr;
    }

    bool operator!=(T* lComPtr) const
    {
        return !operator==(lComPtr);
    }

    bool operator==(T* lComPtr) const
    {
        return m_ComPtr == lComPtr;
    }

    template <typename I>
    HRESULT QueryInterface(I **interfacePtr)
    {
        return m_ComPtr->QueryInterface(IID_PPV_ARGS(interfacePtr));
    }

protected:
    // The internal interface pointer
    T* m_ComPtr;
};
