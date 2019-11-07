/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    Driver.h

Abstract:

    This module contains the type definitions for the sensors service
    driver callback class.

--*/

#pragma once

//
// This class handles driver events for the sensors service driver.
// It supports the OnDeviceAdd event, which occurs when the driver is called
// to setup per-device handlers for a new device stack.
//

class ATL_NO_VTABLE CMyDriver :
    public CComObjectRootEx<CComMultiThreadModel>,
    public CComCoClass<CMyDriver, &CLSID_SensorsGeolocationDriverSample>,
    public IDriverEntry
{
public:
    CMyDriver();

    DECLARE_NO_REGISTRY()
    DECLARE_CLASSFACTORY()
    DECLARE_NOT_AGGREGATABLE(CMyDriver)

    BEGIN_COM_MAP(CMyDriver)
        COM_INTERFACE_ENTRY(IDriverEntry)
    END_COM_MAP()

public:
    //
    // IDriverEntry
    //
    STDMETHOD  (OnInitialize)(_In_ IWDFDriver* pDriver);
    STDMETHOD  (OnDeviceAdd)(_In_ IWDFDriver* pDriver, _In_ IWDFDeviceInitialize* pDeviceInit);
    STDMETHOD_ (void, OnDeinitialize)(_In_ IWDFDriver* pDriver);
};

OBJECT_ENTRY_AUTO(__uuidof(SensorsGeolocationDriverSample), CMyDriver)
