/*++

Copyright (c) Microsoft Corporation, All Rights Reserved

Module Name:

    umdf_vdev_parallelqueue.h

Abstract:

    This file defines the queue callback object for handling parallel device I/O
    control requests.

Environment:

    Windows User-Mode Driver Framework (WUDF)


--*/

#pragma once

class CVDevParallelQueue : 
    public CUnknown,
    public IQueueCallbackDeviceIoControl,
    public IRequestCallbackRequestCompletion
{

private:

    //
    // Weak reference to the critical section object
    //
    CRITICAL_SECTION m_Crit;

    //
    // Weak reference to queue object
    //
    IWDFIoQueue * m_FxQueue;

    //
    // Reference to the FX device class
    // 
    IWDFDevice * m_FxDevice;

    //
    // Reference to our Device Class
    //
    PCVDevDevice m_VdevDevice;

    HRESULT
        Initialize();

    __inline 
        _Acquires_lock_(m_Crit)
        void
        Lock ()
    {
        ::EnterCriticalSection (&m_Crit);
    }

    __inline 
        _Releases_lock_(m_Crit)
        void
        Unlock ()
    {
        ::LeaveCriticalSection (&m_Crit);
    }


public:


    CVDevParallelQueue::CVDevParallelQueue (
        PCVDevDevice VdevDevice,
        IWDFDevice * FxDevice) : 
        m_FxDevice (FxDevice),
        m_FxQueue (NULL),
        m_VdevDevice (VdevDevice)
    {

    }

    virtual ~CVDevParallelQueue ();

    IWDFIoQueue *
        GetFxQueue (
        VOID)
    {
        return m_FxQueue;
    }

    IWDFDevice * 
        GetFxDevice (
        VOID)
    {
        return m_FxDevice;
    }

    static 
        HRESULT 
        CreateInstance ( 
        _In_ PCVDevDevice Device,
        _In_ IWDFDevice * FxDevice,
        _Out_ PCVDevParallelQueue *Queue);

    HRESULT
        Configure (
        VOID)
    {
        return S_OK;
    }

    IQueueCallbackDeviceIoControl *
        DeviceIoControl (
        VOID)
    {
        AddRef();
        return static_cast<IQueueCallbackDeviceIoControl *>(this);
    }


    IRequestCallbackRequestCompletion *
        RequestCompletion (
        VOID)
    {
        AddRef();
        return static_cast <IRequestCallbackRequestCompletion *>(this);
    }


    virtual
        ULONG
        STDMETHODCALLTYPE
        AddRef (VOID) 
    {
        return CUnknown::AddRef();
    }
    
    virtual
        ULONG
        STDMETHODCALLTYPE
        Release (VOID) 
    {
        return CUnknown::Release();
    }

    virtual
        HRESULT
        STDMETHODCALLTYPE
        QueryInterface (
        _In_ REFIID InterfaceId, 
        _Out_ PVOID *Object);

    virtual
        VOID
        STDMETHODCALLTYPE
        OnDeviceIoControl ( 
        _In_ IWDFIoQueue *pWdfQueue,
        _In_ IWDFIoRequest *pWdfRequest,
        _In_ ULONG ControlCode,
        _In_ SIZE_T InputBufferSizeInBytes,
        _In_ SIZE_T OutputBufferSizeInBytes);

    virtual
        VOID
        STDMETHODCALLTYPE
        OnCompletion (  
        _In_ IWDFIoRequest*  pWdfRequest,
        _In_ IWDFIoTarget*  pIoTarget,
        _In_ IWDFRequestCompletionParams*  pParams,
        _In_ PVOID  pContext);

    virtual
        void
        STDMETHODCALLTYPE
        OnAllocateAddrRangeCompletion (
        _In_ IWDFIoRequest *  pWdfRequest,
        _In_ IWDFRequestCompletionParams*  pParams);

    virtual
        VOID
        STDMETHODCALLTYPE
        OnFreeAddrRangeCompletion (
        _In_ IWDFIoRequest *  pWdfRequest,
        _In_ IWDFRequestCompletionParams*  pParams);


    virtual
        HRESULT 
        STDMETHODCALLTYPE
        SubmitAsyncRequestToLower (
        _In_ IWDFIoRequest * Request);

};


