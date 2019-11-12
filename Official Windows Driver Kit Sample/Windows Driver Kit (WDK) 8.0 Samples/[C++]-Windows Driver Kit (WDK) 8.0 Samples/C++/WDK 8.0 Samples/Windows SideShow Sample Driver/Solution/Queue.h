//-----------------------------------------------------------------------
// <copyright file="Queue.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      Queue.h
//
// Description:
//      Declaration of the I/O queue for UMDF requests.
//
//-----------------------------------------------------------------------

#pragma once

#include "WSSDevice.h"

class ATL_NO_VTABLE CQueue :
    public CComObjectRootEx<CComMultiThreadModel>,
    public IQueueCallbackDeviceIoControl,
    public IQueueCallbackCreate,
    public IQueueCallbackRead,
    public IQueueCallbackWrite
{
public:
    DECLARE_NOT_AGGREGATABLE(CQueue)

    BEGIN_COM_MAP(CQueue)
        COM_INTERFACE_ENTRY(IQueueCallbackDeviceIoControl)
        COM_INTERFACE_ENTRY(IQueueCallbackCreate)
        COM_INTERFACE_ENTRY(IQueueCallbackRead)
        COM_INTERFACE_ENTRY(IQueueCallbackWrite)
    END_COM_MAP()

public:
    static HRESULT CreateInstance(WSSDevicePtr parentDevice, IUnknown **ppUkwn);

    // IQueueCallbackCreate
    //
    STDMETHOD_ (void, OnCreateFile)(
        /*[in]*/ IWDFIoQueue *pQueue,
        /*[in]*/ IWDFIoRequest *pRequest,
        /*[in]*/ IWDFFile *pFileObject
        );

    //
    // IQueueCallbackDeviceIoControl
    //
    STDMETHOD_ (void, OnDeviceIoControl)(
        IWDFIoQueue*    pQueue,
        IWDFIoRequest*  pRequest,
        ULONG           ControlCode,
        SIZE_T          InputBufferSizeInBytes,
        SIZE_T          OutputBufferSizeInBytes
        );

    //
    // IQueueCallbackRead
    //
    STDMETHOD_ (void, OnRead)(
        IWDFIoQueue*    pQueue,
        IWDFIoRequest*  pRequest,
        SIZE_T          NumOfBytesToRead
        );

    //
    // IQueueCallbackWrite
    //
    STDMETHOD_ (void, OnWrite)(
       IWDFIoQueue*    pQueue,
       IWDFIoRequest*  pRequest,
       SIZE_T          NumOfBytesToWrite
       );

    virtual ~CQueue();

protected:
    CQueue();

    WSSDevicePtr                    m_parentDevice;
};
