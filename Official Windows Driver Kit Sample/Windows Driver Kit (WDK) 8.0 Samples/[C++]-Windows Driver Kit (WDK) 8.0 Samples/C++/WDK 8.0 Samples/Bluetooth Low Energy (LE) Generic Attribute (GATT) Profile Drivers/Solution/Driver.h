/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Driver.h

Abstract:

--*/

#pragma once

#include "resource.h"

class ATL_NO_VTABLE CDriver :
    public CComObjectRootEx<CComMultiThreadModel>,
    public CComCoClass<CDriver, &CLSID_WpdBluetoothGattServiceSampleDriver>,
    public IDriverEntry,
    public IObjectCleanup
{
public:
    CDriver();

    DECLARE_REGISTRY_RESOURCEID(IDR_WpdBluetoothGattServiceSampleDriver)

    DECLARE_NOT_AGGREGATABLE(CDriver)

    BEGIN_COM_MAP(CDriver)
        COM_INTERFACE_ENTRY(IDriverEntry)
    END_COM_MAP()

public:
    //
    // IDriverEntry
    //
    STDMETHOD (OnInitialize)(
        _In_ IWDFDriver* pDriver
        );
    STDMETHOD (OnDeviceAdd)(
        _In_ IWDFDriver*             pDriver,
        _In_ IWDFDeviceInitialize*   pDeviceInit
        );
    STDMETHOD_ (void, OnDeinitialize)(
        _In_ IWDFDriver* pDriver
        );

    //
    // IObjectCleanup
    //
    STDMETHOD_ (void, OnCleanup)(
        _In_ IWDFObject* pWdfObject
        );
};

OBJECT_ENTRY_AUTO(__uuidof(WpdBluetoothGattServiceSampleDriver), CDriver)


