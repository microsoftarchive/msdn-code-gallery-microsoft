/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


Module Name:

    Queue.h

Abstract:

    This module contains the type definitions for the sensors service driver 
    queue callback class.
--*/

#pragma once

class ATL_NO_VTABLE CMyQueue :
    public CComObjectRootEx<CComMultiThreadModel>,
    public IQueueCallbackDeviceIoControl
{
public:
    virtual ~CMyQueue();

    DECLARE_NOT_AGGREGATABLE(CMyQueue)

    BEGIN_COM_MAP(CMyQueue)
        COM_INTERFACE_ENTRY(IQueueCallbackDeviceIoControl)
    END_COM_MAP()

    static HRESULT CreateInstance(_In_ IWDFDevice* pWdfDevice, CMyDevice* pMyDevice, IWDFIoQueue** ppQueue);

protected:
   CMyQueue();

// COM Interface methods
public:

    // IQueueCallbackDeviceIoControl
    STDMETHOD_ (void, OnDeviceIoControl)(   _In_ IWDFIoQueue*    pQueue,
                                            _In_ IWDFIoRequest*  pRequest,
                                            _In_ ULONG           ControlCode,
                                                 SIZE_T          InputBufferSizeInBytes,
                                                 SIZE_T          OutputBufferSizeInBytes
                                            );

private:
    inline HRESULT EnterProcessing(DWORD64 dwControlFlag);
    inline void    ExitProcessing(DWORD64 dwControlFlag);

    CMyDevice*          m_pParentDevice; // Parent device object
};