/*++

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

    Queue.h

Abstract:

    This module contains the type definitions for the driver's queue callback 
    class.

Environment:

    Windows User-Mode Driver Framework (WUDF)

--*/

#pragma once

#define MILLI_SECOND_TO_NANO100(x)  (x * 1000 * 10)

//
// Class for the queue callbacks.
// It implements
//        IQueueCallbackDeviceIoControl
//
class CMyQueue :
        public CUnknown,
        public IQueueCallbackDeviceIoControl
{

//
// Private data members.
//
private:

    //
    // Weak reference to framework Queue object which this object implements callbacks for
    // This is kept as a weak reference to avoid circular reference
    // This object's lifetime is contained within framework Queue object's lifetime
    //
    
    IWDFIoQueue     *m_FxQueue;

    //
    // Unreferenced pointer to the parent device.
    //

    PCMyDevice m_Device;

    UCHAR m_OutputReport;

//
// Private methods.
//

private:

    CMyQueue(
        _In_ PCMyDevice Device
        ) : 
        m_FxQueue(NULL),
        m_Device(Device),
        m_OutputReport(0)
    {
    }

    virtual ~CMyQueue()
    {
    }

    //
    // Initialize
    //

    HRESULT
    Initialize(
        _In_ IWDFDevice *FxDevice
        );

//
// Public methods
//
public:

    //
    // The factory method used to create an instance of this class
    //
    
    static
    HRESULT
    CreateInstance(
        _In_ PCMyDevice Device,
        _Out_ CMyQueue **Queue
        );


    HRESULT
    Configure(
        VOID
        )
    {
        return S_OK;
    }

//
// COM methods
//
public:

    //
    // IUnknown methods.
    //

    virtual
    ULONG
    STDMETHODCALLTYPE
    AddRef(
        VOID
        )
    {
        return __super::AddRef();
    }

    _At_(this, __drv_freesMem(object))
    virtual
    ULONG
    STDMETHODCALLTYPE
    Release(
        VOID
       )
    {
        return __super::Release();
    }

    virtual
    HRESULT
    STDMETHODCALLTYPE
    QueryInterface(
        _In_ REFIID InterfaceId,
        _Out_ PVOID *Object
        );

    //
    // IQueueCallbackDeviceIoControl
    //
    STDMETHOD_ (void, OnDeviceIoControl)( 
        _In_ IWDFIoQueue *pWdfQueue,
        _In_ IWDFIoRequest *pWdfRequest,
        _In_ ULONG ControlCode,
        _In_ SIZE_T InputBufferSizeInBytes,
        _In_ SIZE_T OutputBufferSizeInBytes
           );
  
    HRESULT
    GetHidDescriptor(
      _In_ IWDFIoRequest2 *FxRequest
      );

    HRESULT
    GetReportDescriptor(
      _In_ IWDFIoRequest2 *FxRequest
      );

    HRESULT
    GetDeviceAttributes(
      _In_ IWDFIoRequest2 *FxRequest
      );

    HRESULT
    ReadReport(
        _In_ IWDFIoRequest2 *FxRequest,
        _Out_ BOOLEAN* CompleteRequest
        );

    HRESULT
    GetFeature(
      _In_ IWDFIoRequest2 *FxRequest
      );

    HRESULT
    SetFeature(
      _In_ IWDFIoRequest2 *FxRequest
      );

    HRESULT
    GetInputReport(
        _In_ IWDFIoRequest2 *FxRequest
        );

    HRESULT
    SetOutputReport(
        _In_ IWDFIoRequest2 *FxRequest
        );

    HRESULT
    WriteReport(
        _In_ IWDFIoRequest2 *FxRequest
        );

    HRESULT
    GetString(
        _In_ IWDFIoRequest2 *FxRequest
        );
 
    static 
    VOID 
    CALLBACK 
    _TimerCallback(
      _Inout_      PTP_CALLBACK_INSTANCE Instance,
      _Inout_opt_  PVOID Context,
      _Inout_      PTP_TIMER Timer
      );

};

class CMyManualQueue :
      public CUnknown,
      public IObjectCleanup
{
private:

    IWDFIoQueue *m_FxQueue;

    PCMyDevice m_Device;

    //
    // threadpool timer
    //
    PTP_TIMER m_Timer;

private:

    CMyManualQueue(
        PCMyDevice Device
        ) : 
        m_FxQueue(NULL),
        m_Timer(NULL),
        m_Device(Device)
    {
    }

    virtual ~CMyManualQueue()
    {
    }

    //
    // Initialize
    //

    HRESULT
    Initialize(
        _In_ IWDFDevice *FxDevice
        );

public:

    IWDFIoQueue*
    GetQueue(
        )
    {
        return m_FxQueue;
    }

    PTP_TIMER
    GetTimer(
        )
    {
        return m_Timer;
    }

    //
    // The factory method used to create an instance of this class
    //
    static
    HRESULT
    CreateInstance(
        _In_ PCMyDevice Device,
        _Out_ CMyManualQueue **Queue
        );

    //
    // COM methods
    //
public:

    //
    // IUnknown methods.
    //
    virtual
    ULONG
    STDMETHODCALLTYPE
    AddRef(
      VOID
      )
    {
        return __super::AddRef();
    }

    _At_(this, __drv_freesMem(object))
    virtual
    ULONG
    STDMETHODCALLTYPE
    Release(
        VOID
     )
    {
        return __super::Release();
    }

    //
    // IObjectCleanup .
    //
    virtual
    void
    STDMETHODCALLTYPE
    OnCleanup(
        IN IWDFObject*  pWdfObject
        );

    virtual
    HRESULT
    STDMETHODCALLTYPE
    QueryInterface(
        _In_ REFIID InterfaceId,
        _Out_ PVOID *Object
        );

    static 
    VOID 
    CALLBACK 
    _TimerCallback(
        _Inout_      PTP_CALLBACK_INSTANCE Instance,
        _Inout_      PVOID Context,
        _Inout_      PTP_TIMER Timer
        );
};

