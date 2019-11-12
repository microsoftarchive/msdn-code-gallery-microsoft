/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    WpdBaseDriver.h

Abstract:

--*/

#pragma once

class WpdBaseDriver :
    public IUnknown
{
public:
    WpdBaseDriver();
    virtual ~WpdBaseDriver();

    HRESULT Initialize(_In_ IWDFDevice* pDevice, _In_ PCWSTR DeviceFileName);
    VOID    Uninitialize();

    HRESULT DispatchClientArrival();

    HRESULT DispatchClientDeparture();

    HRESULT DispatchWpdMessage(_In_    IPortableDeviceValues* pParams,
                               _Inout_   IPortableDeviceValues* pResults);

private:
    HRESULT OnGetObjectIDsFromPersistentUniqueIDs(_In_    IPortableDeviceValues* pParams,
                                                  _Inout_ IPortableDeviceValues* pResults);
    

    HRESULT OnSaveClientInfo(_In_   IPortableDeviceValues* pParams,
                             _Inout_  IPortableDeviceValues* pResults);

public: // IUnknown
    ULONG __stdcall AddRef();

    _At_(this, __drv_freesMem(Mem)) 
    ULONG __stdcall Release();

    HRESULT __stdcall QueryInterface(REFIID riid, void** ppv);

public:
    WpdObjectEnumerator     m_ObjectEnum;
    WpdObjectProperties     m_ObjectProperties;
    WpdCapabilities         m_Capabilities;
    WpdService              m_Service;

private:
    BthLEDevice             m_Device;
    ULONG                   m_cRef;
};


