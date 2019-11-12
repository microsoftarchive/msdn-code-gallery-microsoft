//-----------------------------------------------------------------------
// <copyright file="DriverEntry.h" company="Microsoft">
//      Copyright (c) 2005 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      DriverEntry.h
//
// Description:
//      This is the main entrypoint for the UMDF driver.  It supports
//      the IDriverEntry interface and is CoCreate'd by the framework
//      to initialize the driver to device add/remove.
//
//-----------------------------------------------------------------------

#pragma once

class ATL_NO_VTABLE CDriverEntry : 
    public CComObjectRootEx<CComMultiThreadModel>,
    public CComCoClass<CDriverEntry, &CLSID_WSSDisplayDriver>,
    public IDriverEntry
{
public:
    CDriverEntry();

    DECLARE_NO_REGISTRY()
        
    DECLARE_NOT_AGGREGATABLE(CDriverEntry)

    BEGIN_COM_MAP(CDriverEntry)
        COM_INTERFACE_ENTRY(IDriverEntry)
    END_COM_MAP()

public:
    //        
    // IDriverEntry
    //
    STDMETHOD(OnInitialize)(
        IWDFDriver* pDriver
        );

    STDMETHOD(OnDeviceAdd)(
        IWDFDriver*             pDriver,
        IWDFDeviceInitialize*   pDeviceInit
        );

    STDMETHOD_(void, OnDeinitialize)(
        IWDFDriver* pDriver
        );
};

//
// Define the Object Map array using the CDriverEntry class.
//
BEGIN_OBJECT_MAP(ObjectMapArray)
    OBJECT_ENTRY(__uuidof(WSSDisplayDriver), CDriverEntry)
END_OBJECT_MAP()

ATL::_ATL_OBJMAP_ENTRY* ObjectMap = ObjectMapArray;
