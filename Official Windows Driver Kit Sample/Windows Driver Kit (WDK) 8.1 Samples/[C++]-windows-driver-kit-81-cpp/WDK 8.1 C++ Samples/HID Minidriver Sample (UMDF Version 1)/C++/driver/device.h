/*++

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

    Device.h

Abstract:

    This module contains the type definitions for the UMDF 
    driver's device callback class.

Environment:

    Windows User-Mode Driver Framework (WUDF)

--*/

#pragma once

//
// Misc definitions
//
#define CONTROL_FEATURE_REPORT_ID   0x01

typedef UCHAR HID_REPORT_DESCRIPTOR, *PHID_REPORT_DESCRIPTOR;

//
// These are the device attributes returned by the mini driver in response
// to IOCTL_HID_GET_DEVICE_ATTRIBUTES.
//
#define HIDMINI_PID              0xFEED
#define HIDMINI_VID              0xBEEF
#define HIDMINI_VERSION          0x0101

//
// Class declaration
//
class CMyDevice : 
    public CUnknown
{

//
// Private data members.
//
private:

    IWDFDevice *m_FxDevice;

public:

    PCMyManualQueue m_ManualQueue;

    HID_DEVICE_ATTRIBUTES m_Attributes;

    BYTE m_DeviceData;

//
// Private methods.
//

private:

    CMyDevice(
        VOID
        ) :
        m_FxDevice(NULL),
        m_ManualQueue(NULL)
    {
        ZeroMemory(&m_Attributes, sizeof(HID_DEVICE_ATTRIBUTES));
        m_Attributes.Size = sizeof(HID_DEVICE_ATTRIBUTES);
        m_Attributes.VendorID = HIDMINI_VID;
        m_Attributes.ProductID = HIDMINI_PID;
        m_Attributes.VersionNumber = HIDMINI_VERSION;
        m_DeviceData = 'K';
    }

    HRESULT
    Initialize(
        _In_ IWDFDriver *FxDriver,
        _In_ IWDFDeviceInitialize *FxDeviceInit
        );

//
// Public methods
//
public:

    //
    // The factory method used to create an instance of this driver.
    //
    
    static
    HRESULT
    CreateInstance(
        _In_ IWDFDriver *FxDriver,
        _In_ IWDFDeviceInitialize *FxDeviceInit,
        _Out_ PCMyDevice *Device
        );

    HRESULT
    Configure(
        VOID
        );

    IWDFDevice *
    GetFxDevice(
        VOID
        )
    {
        return m_FxDevice;
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
};


