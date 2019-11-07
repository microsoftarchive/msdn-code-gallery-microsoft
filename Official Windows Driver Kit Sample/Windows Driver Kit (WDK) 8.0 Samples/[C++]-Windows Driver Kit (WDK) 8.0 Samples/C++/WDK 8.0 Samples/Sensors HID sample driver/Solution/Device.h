/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    Device.h

Abstract:

    This module contains the type definitions for the HID sensor class driver
    device callback class.

--*/

#pragma once

#include "SensorManager.h"

class ATL_NO_VTABLE CMyDevice :
    public CComObjectRootEx<CComMultiThreadModel>,
    public IPnpCallback,
    public IPnpCallbackHardware,
    public IFileCallbackCleanup
{
public:
    ~CMyDevice();

    DECLARE_NOT_AGGREGATABLE(CMyDevice)

    BEGIN_COM_MAP(CMyDevice)
        COM_INTERFACE_ENTRY(IPnpCallback)
        COM_INTERFACE_ENTRY(IPnpCallbackHardware)
        COM_INTERFACE_ENTRY(IFileCallbackCleanup)
    END_COM_MAP()

    volatile DWORD64 m_dwShutdownControlFlags;

protected:
    
    CMyDevice();
    HRESULT ConfigureQueue();

// COM Interface methods
public:

    //
    // IPnpCallbackHardware
    //
    STDMETHOD_ (HRESULT, OnPrepareHardware)(_In_ IWDFDevice* pWdfDevice);
    STDMETHOD_ (HRESULT, OnReleaseHardware)(_In_ IWDFDevice* pWdfDevice);

    //
    // IPnpCallback
    //
    STDMETHOD (OnD0Entry)( _In_ IWDFDevice* pWdfDevice,  _In_ WDF_POWER_DEVICE_STATE previousState);
    STDMETHOD (OnD0Exit)( _In_ IWDFDevice* pWdfDevice,  _In_ WDF_POWER_DEVICE_STATE newState);
    STDMETHOD_ (VOID, OnSurpriseRemoval)( _In_ IWDFDevice* pWdfDevice);
    STDMETHOD_ (HRESULT, OnQueryRemove)(  _In_ IWDFDevice* pWdfDevice);
    STDMETHOD_ (HRESULT, OnQueryStop)( _In_ IWDFDevice* pWdfDevice);

    //
    // IFileCallbackCleanup
    //
    STDMETHOD_ (VOID, OnCleanupFile)(_In_ IWDFFile *pWdfFile);

public:

    // The factory method used to create an instance of this device
    static HRESULT CreateInstance(_In_ IWDFDriver* pDriver, _In_ IWDFDeviceInitialize*  pDeviceInit);

    HRESULT ProcessIoControl(_In_ IWDFIoQueue*     pQueue,
                             _In_ IWDFIoRequest*   pRequest,
                             _In_ ULONG            ControlCode,
                                  SIZE_T           InputBufferSizeInBytes,
                                  SIZE_T           OutputBufferSizeInBytes,
                                  DWORD*           pcbWritten);

    inline HRESULT EnterProcessing(DWORD64 dwControlFlag);
    inline void    ExitProcessing(DWORD64 dwControlFlag);

private:
    inline void    EnterShutdown();

    CComPtr<IWDFDevice>                 m_spWdfDevice;
    CComObject<CSensorManager>*         m_pSensorManager;
};
