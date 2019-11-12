/*++

Copyright (c) Microsoft Corporation, All Rights Reserved

Module Name:

    umdf_vdev_sequentialqueue.h

Abstract:

    This file defines the queue callback object for handling serialized device I/O
    control requests. 

Environment:

    Windows User-Mode Driver Framework (WUDF)


--*/

#pragma once

class CVDevSequentialQueue : 
    public CUnknown,
    public IQueueCallbackDeviceIoControl
{

private:

    //
    // Request input memory block
    //
    PVOID m_InputMemory;

    // 
    // Request output memory block
    //
    PVOID m_OutputMemory;

    //
    // Weak reference to the critical section object
    //
    CRITICAL_SECTION m_Crit;

    //
    // Weak reference to queue object
    //
    IWDFIoQueue * m_FxQueue;

    //
    // Reference to the device class
    // 
    IWDFDevice * m_FxDevice;

    //
    // Reference to the Lower IO Target
    //
    IWDFIoTarget * m_kmdfIoTarget;

    HRESULT
        Initialize();

    __inline 
        _Acquires_lock_(m_Crit)
        void
        Lock ()
    {
        ::EnterCriticalSection(&m_Crit);
    }

    __inline 
        _Releases_lock_(m_Crit)
        void 
        Unlock ()
    {
        ::LeaveCriticalSection(&m_Crit);
    }

public:


    CVDevSequentialQueue::CVDevSequentialQueue (
        IWDFDevice * FxDevice):
    m_FxDevice(FxDevice)
    {
    }

    CVDevSequentialQueue::~CVDevSequentialQueue()
    {
        DeleteCriticalSection (&this->m_Crit);
    }

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
        _Out_ PCVDevSequentialQueue *Queue);



    IQueueCallbackDeviceIoControl *
        DeviceIoControl (VOID)
    {
        AddRef();
        return static_cast<IQueueCallbackDeviceIoControl *>(this);
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
        HRESULT
        STDMETHODCALLTYPE
        SubmitRequestToLower (
        _In_ IWDFIoRequest * Request);
};


