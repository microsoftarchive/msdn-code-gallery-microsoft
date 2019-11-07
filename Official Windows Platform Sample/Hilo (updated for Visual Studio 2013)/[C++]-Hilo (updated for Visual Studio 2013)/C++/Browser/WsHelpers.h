//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

class WsError
{
    WS_ERROR* m_errorHandle;

public:

    WsError() :
        m_errorHandle(0)
    {
        // Do nothing
    }

    WsError(WS_ERROR* errorHandle) :
        m_errorHandle(errorHandle)
    {
        // Do nothing
    }

    ~WsError()
    {
        if (0 != m_errorHandle)
        {
            WsFreeError(m_errorHandle);
        }
    }

    HRESULT Create(__in_ecount_opt(propertyCount) const WS_ERROR_PROPERTY* properties, __in ULONG propertyCount)
    {
        return WsCreateError(properties, propertyCount, &m_errorHandle);
    }

    HRESULT GetProperty(__in WS_ERROR_PROPERTY_ID id, __out_bcount(bufferSize) void* buffer, __in ULONG bufferSize)
    {
        return WsGetErrorProperty(m_errorHandle, id, buffer, bufferSize);
    }

    template <typename T>
    HRESULT GetProperty(__in WS_ERROR_PROPERTY_ID id, __out T* buffer)
    {
        return GetProperty(id, buffer, sizeof(T));
    }

    HRESULT GetString(__in ULONG index, __out WS_STRING* string)
    {
        return WsGetErrorString(m_errorHandle, index, string);
    }

    operator WS_ERROR*() const
    {
        return m_errorHandle;
    }
};

class WsHeap
{
    WS_HEAP* m_heapHandle;

public:

    WsHeap() :
        m_heapHandle(0)
    {
        // Do nothing
    }

    ~WsHeap()
    {
        if (0 != m_heapHandle)
        {
            WsFreeHeap(m_heapHandle);
        }
    }

    HRESULT Create(__in SIZE_T maxSize,
                   __in SIZE_T trimSize,
                   __in_opt const WS_HEAP_PROPERTY* properties,
                   __in ULONG propertyCount,
                   __in_opt WS_ERROR* error)
    {
        return WsCreateHeap(maxSize, trimSize, properties, propertyCount, &m_heapHandle, error);
    }

    operator WS_HEAP*() const
    {
        return m_heapHandle;
    }

};

class WsServiceProxy
{
    WS_SERVICE_PROXY* m_serviceProxyHandle;
    bool m_open;

public:

    WsServiceProxy() :
        m_serviceProxyHandle(0),
        m_open(false)
    {
        // Do nothing
    }

    ~WsServiceProxy()
    {
        if (0 != m_serviceProxyHandle)
        {
            if (m_open)
            {
                Close(/*async context*/0, /*error*/0);
            }

            WsFreeServiceProxy(m_serviceProxyHandle);
        }
    }

    HRESULT Open(__in const WS_ENDPOINT_ADDRESS* address, 
                 __in_opt const WS_ASYNC_CONTEXT* asyncContext, 
                 __in_opt WS_ERROR* error)
    {
        HRESULT hr = WsOpenServiceProxy(m_serviceProxyHandle, address, asyncContext, error);
        m_open = SUCCEEDED(hr);
        return hr;
    }

    HRESULT Close(__in_opt const WS_ASYNC_CONTEXT* asyncContext, __in_opt WS_ERROR* error)
    {
        HRESULT hr = WsCloseServiceProxy(m_serviceProxyHandle, asyncContext, error);

        m_open = FAILED(hr);
        return hr;
    }

    WS_SERVICE_PROXY** operator&()
    {
        return &m_serviceProxyHandle;
    }

    operator WS_SERVICE_PROXY*() const
    {
        return m_serviceProxyHandle;
    }
};

class WsServiceHost
{
    WS_SERVICE_HOST* m_serviceHostHandle;
    bool m_open;

public:

    WsServiceHost() :
        m_serviceHostHandle(0),
        m_open(false)
    {
        // Do nothing
    }

    ~WsServiceHost()
    {
        if (0 != m_serviceHostHandle)
        {
            if (m_open)
            {
                Close(/*async context*/0, /*error*/0);
            }

            WsFreeServiceHost(m_serviceHostHandle);
        }
    }

    HRESULT Create(__in_ecount_opt(endpointCount) const WS_SERVICE_ENDPOINT** endpoints,
                   __in const USHORT endpointCount,
                   __in_ecount_opt(servicePropertyCount) const WS_SERVICE_PROPERTY* properties,
                   __in ULONG propertyCount,
                   __in_opt WS_ERROR* error)
    {
        return WsCreateServiceHost(endpoints, endpointCount, properties, propertyCount, &m_serviceHostHandle, error);
    }

    HRESULT Open(__in_opt const WS_ASYNC_CONTEXT* asyncContext, __in_opt WS_ERROR* error)
    {
        HRESULT hr = WsOpenServiceHost(m_serviceHostHandle, asyncContext, error);
        m_open = SUCCEEDED(hr);
        return hr;
    }

    HRESULT Close(__in_opt const WS_ASYNC_CONTEXT* asyncContext, __in_opt WS_ERROR* error)
    {
        HRESULT hr = WsCloseServiceHost(m_serviceHostHandle, asyncContext, error);

        m_open = FAILED(hr);
        return hr;
    }

    operator WS_SERVICE_HOST*() const
    {
        return m_serviceHostHandle;
    }
};
