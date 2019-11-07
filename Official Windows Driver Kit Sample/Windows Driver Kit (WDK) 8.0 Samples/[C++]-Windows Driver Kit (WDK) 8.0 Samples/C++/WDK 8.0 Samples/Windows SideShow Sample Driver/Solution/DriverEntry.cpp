//-----------------------------------------------------------------------
// <copyright file="DriverEntry.cpp" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      DriverEntry.cpp
//
// Description:
//      This file implements the CDriverEntry class, which is the UMDF
//      entry point into the driver.
//
//-----------------------------------------------------------------------

#include "Common.h"
#include "Queue.h"
#include "Driver.h"
#include "DriverEntry.h"


/////////////////////////////////////////////////////////////////////////
//
// CDriverEntry::CDriverEntry
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CDriverEntry::CDriverEntry()
{
}

/////////////////////////////////////////////////////////////////////////
//
// CDriverEntry::OnDeviceAdd
//
// The framework call this function when device is detected. This driver
// creates a device callback object and
//
// Parameters:
//      pDriver     - pointer to an IWDFDriver object
//      pDeviceInit - pointer to a device initialization object
//
// Return Values:
//      S_OK: device initialized successfully
//
/////////////////////////////////////////////////////////////////////////
HRESULT CDriverEntry::OnDeviceAdd(
    IWDFDriver* pDriver,
    IWDFDeviceInitialize* pDeviceInit
    )
{
    HRESULT             hr = S_OK;
    WSSDevicePtr        pDevice = NULL;
    CComPtr<IUnknown>   pDeviceCallback = NULL;
    CComPtr<IWDFDevice> pIWDFDevice = NULL;

    //
    // Create device callback object
    //
    hr = CWSSDevice::CreateDeviceInstance(&pDevice, pDeviceInit);

    if (NULL == pDevice)
    {
        return E_UNEXPECTED;
    }

    if (SUCCEEDED(hr))
    {
        pDevice->QueryInterface(__uuidof(IUnknown), (void**)&pDeviceCallback);
    }

    //
    // Create WDFDevice.
    //
    if (S_OK == hr)
    {
        hr = pDriver->CreateDevice(
                pDeviceInit,
                pDeviceCallback,
                &pIWDFDevice);
    }

    //
    // Create queue callback object
    //
    CComPtr<IUnknown> pIUnknown = NULL;
    if (S_OK == hr)
    {
        hr = CQueue::CreateInstance(pDevice, &pIUnknown);
    }

    //
    // Configure the default queue.
    //
    if (S_OK == hr)
    {
        CComPtr<IWDFIoQueue> pDefaultQueue = NULL;
        hr = pIWDFDevice->CreateIoQueue(
                          pIUnknown,
                          TRUE,                         // bDefaultQueue
                          WdfIoQueueDispatchSequential, // Sequential queue handling
                          TRUE,                         // bPowerManaged
                          FALSE,                        // bAllowZeroLengthRequests
                          &pDefaultQueue);
    }

    //
    // Release the reference that we held for this method; note that UMDF still holds a reference from the QI above.
    //
    pDevice->Release();

    return hr;
}

HRESULT
CDriverEntry::OnInitialize(
    IWDFDriver* /*pDriver*/
    )
{
    return S_OK;
}

void
CDriverEntry::OnDeinitialize(
    IWDFDriver* /*pDriver*/
    )
{
    return;
}
