//-----------------------------------------------------------------------
// <copyright file="WSSDevice.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      WSSDevice.h
//
// Description:
//      This class implements the IPnpCallbackHardware
//      interface.  It represents the UMDF device instance, and allows
//      access to the device.
//
//-----------------------------------------------------------------------

#pragma once

#include "BasicDDI.h"

class IWSSTransport;

class ATL_NO_VTABLE CWSSDevice :
    public CComObjectRootEx<CComMultiThreadModel>,
    public IPnpCallbackHardware,
    public IFileCallbackCleanup
{
public:
    virtual ~CWSSDevice();

public:
    DECLARE_NOT_AGGREGATABLE(CWSSDevice)

    BEGIN_COM_MAP(CWSSDevice)
        COM_INTERFACE_ENTRY(IPnpCallbackHardware)
        COM_INTERFACE_ENTRY(IFileCallbackCleanup)
    END_COM_MAP()

    //
    // IPnpCallbackHardware
    //
    STDMETHOD (OnPrepareHardware)(/*[in]*/ IWDFDevice* pWdfDevice);

    STDMETHOD (OnReleaseHardware)(/*[in]*/ IWDFDevice* pWdfDevice);

    //
    // IFileCallbackCleanup
    //
    STDMETHOD_ (void, OnCleanupFile)(/*[in]*/ IWDFFile *pFileObject);

public:
    static HRESULT CreateDeviceInstance(
        WSSDevicePtr*                   ppDevice,
        IWDFDeviceInitialize*           pDeviceInit
        );

    HRESULT FinalConstruct();
    void    FinalRelease();

    HRESULT InitializeClassExtension();
    HRESULT ProcessIoControl(
        IWDFIoQueue*     pQueue,
        IWDFIoRequest*   pRequest,
        ULONG            ControlCode,
        SIZE_T           InputBufferSizeInBytes,
        SIZE_T           OutputBufferSizeInBytes,
        DWORD*           pcbWritten,
        BOOL*            pfCompleteRequest
        );

protected:
    CWSSDevice();

    //
    // Device helper methods
    //
    HRESULT BasicDriverInitialization(void);
    HRESULT BasicDriverShutdown(void);

private:
    // If m_pClassExtension2.p is not NULL, then ISideShowClassExtension2 is available, and the driver
    // should follow Windows 7 SideShow driver guidelines during use.
    //
    // If m_pClassExtension2.p is NULL, then m_pClassExtension1.p will not be NULL and ISideShowClassExtension
    // will be available. In this case, the driver should follow Windows Vista SideShow driver guidelines
    // during use.
    CComPtr<ISideShowClassExtension>    m_pClassExtension1;
    CComPtr<ISideShowClassExtension2>   m_pClassExtension2;

    CComObject<CWssBasicDDI>*           m_pBasicDriver;

    CComPtr<IWDFDevice>                 m_pWdfDevice;
};
