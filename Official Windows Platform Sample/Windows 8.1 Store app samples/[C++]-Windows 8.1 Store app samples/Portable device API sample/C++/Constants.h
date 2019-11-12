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

#include <collection.h>

#define NUM_OBJECTS_TO_REQUEST 10

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    partial ref class MainPage
    {
    public:
        static property Platform::String^ FEATURE_NAME
        {
            Platform::String^ get()
            {  
                return ref new Platform::String(L"PortableDevice"); 
            }
        }

        static property Platform::Array<Scenario>^ scenarios
        {
            Platform::Array<Scenario>^ get()
            { 
                return scenariosInner;
            }
        }

        void DispatcherNotifyUser(Platform::String^ message, NotifyType type);

    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };

    // RAII wrapper for CoTaskMemAlloc'ed resources
    struct CoTaskMemFreeStruct
    {
        void operator() (void* ptr) const
        {
            CoTaskMemFree(ptr);
        }
    };
    typedef std::unique_ptr<wchar_t, CoTaskMemFreeStruct> CoTaskMemString;

    // RAII wrapper for Propvariants
    struct PropVariantClearStruct
    {
        void operator() (PROPVARIANT* pvPtr) const
        {
            PropVariantClear(pvPtr);
        }
    };
    typedef std::unique_ptr<PROPVARIANT, PropVariantClearStruct> PropVariantPtr;

    // RAII wrapper for an array of CoTaskMemAlloc'ed resiyrces
    class CoTaskMemFreeArray
    {
    public:
        CoTaskMemFreeArray(_Inout_updates_(count) void** ptr, size_t count) :
          _ptr(ptr), _count(count)
        {
        }

        ~CoTaskMemFreeArray()
        {
            for (size_t i=0; i<_count; i++)
            {
                CoTaskMemFree(_ptr[i]);
            }
        }

        void** _ptr;
        size_t _count;
    };

    // Common helper functions
    Microsoft::WRL::ComPtr<IPortableDeviceValues> GetClientInfo();
    Platform::String^ GetFirstStorageId(_In_ IPortableDevice* device);
    ULONGLONG StreamCopy(_In_ IStream* destStream, _In_ IStream* sourceStream, DWORD transferSizeBytes);

    inline void ThrowIfFailed(HRESULT hr)
    {
        if (FAILED(hr))
        {
            // Set a breakpoint on the line below to catch errors from the Portable Device APIs.
            throw Platform::Exception::CreateException(hr);
        }
    }
}
