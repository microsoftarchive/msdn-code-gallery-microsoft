/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

    Device.cpp

Abstract:

    This module contains the implementation of the SPB accelerometer's
    device callback object.

--*/

#include "Internal.h"

#include "Queue.h"
#include "SensorDdi.h"

#include "Device.h"
#include "Device.tmh"

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::CMyDevice
//
//  Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CMyDevice::CMyDevice() :
    m_spWdfDevice(nullptr),
    m_spClassExtension(nullptr),
    m_pSensorDdi(nullptr)
{

}

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::~CMyDevice
//
//  Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CMyDevice::~CMyDevice()
{

}

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::CreateInstance
//
//  This static method is used to create and initialize an instance of
//  CMyDevice for use with a given hardware device.
//
//  Parameters:
//      pDriver     - pointer to an IWDFDriver interface
//      pDeviceInit - pointer to an interface used to intialize the device
//      ppMyDevice  - pointer to a location to place the CMyDevice instance
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::CreateInstance(
    _In_  IWDFDriver*              pDriver,
    _In_  IWDFDeviceInitialize*    pDeviceInit,
    _Out_ CComObject<CMyDevice>**  ppMyDevice
    )
{
    FuncEntry();

    CComObject<CMyDevice>* pMyDevice = nullptr;

    HRESULT hr = CComObject<CMyDevice>::CreateInstance(&pMyDevice);

    if (SUCCEEDED(hr))
    {
        pMyDevice->AddRef();

        hr = pMyDevice->Initialize(pDriver, pDeviceInit);

        if (SUCCEEDED(hr))
        {
            *ppMyDevice = pMyDevice;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::Initialize
//
//  This method initializes the device callback object and creates the
//  partner device object.
//
//  Parameters:
//      pDriver     - pointer to an IWDFDriver interface
//      pDeviceInit - pointer to an interface used to intialize the device
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::Initialize(
    _In_  IWDFDriver*              pDriver,
    _In_  IWDFDeviceInitialize*    pDeviceInit
    )
{
    FuncEntry();
    
    CComPtr<IUnknown> spCallback;
    CComPtr<IWDFDevice> spIWDFDevice;
    CComPtr<IWDFDevice3> spIWDFDevice3;
    HRESULT hr;
    
    // Prepare device parameters
    pDeviceInit->SetLockingConstraint(None); 
    pDeviceInit->SetPowerPolicyOwnership(TRUE);

    hr = QueryInterface(IID_PPV_ARGS(&spCallback));

    if (SUCCEEDED(hr))
    {
        // Create the IWDFDevice object
        hr = pDriver->CreateDevice(pDeviceInit, spCallback, &spIWDFDevice);

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to create the IWDFDevice object, %!HRESULT!",
                hr);
        }

        if (SUCCEEDED(hr))
        {   
            // Assign context
            hr = spIWDFDevice->AssignContext(nullptr, (void*)this);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to assign context, %!HRESULT!",
                    hr);
            }
        }
    }
        
    if (SUCCEEDED(hr))
    {
        WUDF_DEVICE_POWER_POLICY_IDLE_SETTINGS idleSettings;
        WUDF_DEVICE_POWER_POLICY_IDLE_SETTINGS_INIT(
            &idleSettings,
            IdleCannotWakeFromS0
            );

        // Set delay timeout value. This specifies the time
        // delay between WDF detecting the device is idle
        // and WDF requesting a Dx power transition on the 
        // device's behalf.
        idleSettings.IdleTimeout = 100;
        
        // Opt-in to D3Cold to allow the platform to remove 
        // power when the device is idle and enters D3.
        idleSettings.ExcludeD3Cold = WdfFalse;

        // Get a pointer to the IWDFDevice3 interface and
        // assign the idle settings.
        hr = spIWDFDevice->QueryInterface(IID_PPV_ARGS(&spIWDFDevice3));

        if (SUCCEEDED(hr)) 
        {
            hr = spIWDFDevice3->AssignS0IdleSettingsEx(&idleSettings);
        }
    }
        
    if (SUCCEEDED(hr))
    {
        // Ensure device is disable-able
        spIWDFDevice->SetPnpState(WdfPnpStateNotDisableable, WdfFalse);
        spIWDFDevice->CommitPnpState();

        // Store the IWDFDevice pointer
        m_spWdfDevice = spIWDFDevice;
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::Configure
//  
//  This method is called after the device callback object has been initialized 
//  and returned to the driver.  It would setup the device's queues and their 
//  corresponding callback objects.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::Configure()
{
    FuncEntry();

    HRESULT hr = CMyQueue::CreateInstance(m_spWdfDevice, this);

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to create instance of CMyQueue, %!HRESULT!",
            hr);
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::OnPrepareHardware
//
//  Called by UMDF to prepare the hardware for use. In our case
//  we create the SensorDdi object and initialize the Sensor Class Extension
//
//  Parameters:
//      pWdfDevice - pointer to an IWDFDevice object representing the
//          device
//      pWdfResourcesRaw - pointer the raw resource list
//      pWdfResourcesTranslated - pointer to the translated resource list
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::OnPrepareHardware(
    _In_ IWDFDevice3 * pWdfDevice,
    _In_ IWDFCmResourceList * pWdfResourcesRaw,
    _In_ IWDFCmResourceList * pWdfResourcesTranslated
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    // Create the SensorDDI
    hr = CComObject<CSensorDdi>::CreateInstance(&m_pSensorDdi);

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to create the sensor DDI, %!HRESULT!", 
            hr);
    }

    if (SUCCEEDED(hr))
    {
        // AddRef after CreateInstance
        m_pSensorDdi->AddRef();

        // Initialize the DDI
        hr = m_pSensorDdi->Initialize(
            pWdfDevice,
            pWdfResourcesRaw, 
            pWdfResourcesTranslated);

        CComPtr<IUnknown> spUnknown;
        if (SUCCEEDED(hr))
        {
            hr = m_pSensorDdi->QueryInterface(IID_PPV_ARGS(&spUnknown));
        }

        if (SUCCEEDED(hr))
        {
            // Create and initialize the sensor class extension
            hr = CoCreateInstance(
                CLSID_SensorClassExtension,
                nullptr,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(&m_spClassExtension));
                
            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Could not create the sensor class extension, "
                    "%!HRESULT!", 
                    hr);
            }
                
            if (SUCCEEDED(hr))
            {
                // Initialize the sensor class extension
                hr = m_spClassExtension->Initialize(pWdfDevice, spUnknown);
            }  

            if (SUCCEEDED(hr))
            {
                // Pass a pointer to the class extension to the DDI
                hr = m_pSensorDdi->SetSensorClassExtension(m_spClassExtension);
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::OnReleaseHardware
//
//  Called by WUDF to uninitialize the hardware.
//
//  Parameters:
//      pWdfDevice - pointer to an IWDFDevice object for the device
//      pWdfResourcesTranslated - pointer to the translated resource list
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::OnReleaseHardware(
    _In_ IWDFDevice3 * pWdfDevice,
    _In_ IWDFCmResourceList * pWdfResourcesTranslated
    )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(pWdfDevice);
    UNREFERENCED_PARAMETER(pWdfResourcesTranslated);

    HRESULT hr = S_OK;

    // Uninitialize and release the class extension.
    if (m_spClassExtension != nullptr)
    {
        hr = m_spClassExtension->Uninitialize();
    }
    
    // Uninitialize the sensor DDI
    if (m_pSensorDdi != nullptr)
    {
        m_pSensorDdi->Uninitialize();
        SAFE_RELEASE(m_pSensorDdi);
    }

    // Release the IWDFDevice handle, if it matches
    if (pWdfDevice == m_spWdfDevice.p)
    {
        m_spWdfDevice.Release();
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::OnD0Entry
//
//  This method is called after a new device enters the system
//
//  Parameters:
//      pWdfDevice    - pointer to a device object
//      previousState - previous WDF power state
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::OnD0Entry(
    _In_ IWDFDevice* pWdfDevice,
    _In_ WDF_POWER_DEVICE_STATE previousState
    )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(pWdfDevice);
    UNREFERENCED_PARAMETER(previousState);

    HRESULT hr = m_pSensorDdi->Start();

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to start the sensor DDI, %!HRESULT!",
            hr);
    }

    FuncExit();
    
    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::OnD0Exit
//
//  This method is called when a device leaves the system
//
//  Parameters:
//      pWdfDevice - pointer to a device object
//      newState   - new WDF power state
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::OnD0Exit(
    _In_ IWDFDevice* pWdfDevice,
    _In_ WDF_POWER_DEVICE_STATE newState
    )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(pWdfDevice);
    UNREFERENCED_PARAMETER(newState);

    HRESULT hr = m_pSensorDdi->Stop();

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to stop the sensor DDI, %!HRESULT!",
            hr);
    }
    
    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::OnCleanupFile
//
//  This method is called when the file handle to the device is closed
//
//  Parameters:
//      pWdfFile - pointer to a file object
//
//  Return Values:
//      none
//
/////////////////////////////////////////////////////////////////////////
VOID CMyDevice::OnCleanupFile(
    _In_ IWDFFile* pWdfFile
    )
{
    FuncEntry();

    if (m_spClassExtension != nullptr)
    {
        m_spClassExtension->CleanupFile(pWdfFile);
    }

    FuncExit();

    return;
}

/////////////////////////////////////////////////////////////////////////
//
//  CMyDevice::ProcessIoControl
//
//  This method is a helper that takes the incoming IOCTL and forwards
//  it to the Windows Sensor Class Extension for processing.
//
//  Parameters:
//      pQueue                  - pointer to the UMDF queue that
//                                handled the request
//      pRequest                - pointer to the request
//      ControlCode             - the IOCTL code
//      InputBufferSizeInBytes  - size of the incoming IOCTL buffer
//      OutputBufferSizeInBytes - size of the outgoing IOCTL buffer
//      pcbWritten              - pointer to a DWORD containing the number
//                                of bytes returned
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::ProcessIoControl(
    _In_ IWDFIoQueue*     pQueue,
    _In_ IWDFIoRequest*   pRequest,
    _In_ ULONG            ControlCode,
         SIZE_T           InputBufferSizeInBytes,
         SIZE_T           OutputBufferSizeInBytes,
         DWORD*           pcbWritten)
{
    FuncEntry();

    UNREFERENCED_PARAMETER(pQueue);
    UNREFERENCED_PARAMETER(ControlCode);
    UNREFERENCED_PARAMETER(InputBufferSizeInBytes);
    UNREFERENCED_PARAMETER(OutputBufferSizeInBytes);
    UNREFERENCED_PARAMETER(pcbWritten);

    HRESULT hr = S_OK;
    
    if (m_spClassExtension == nullptr)
    {
        hr = E_POINTER;
    }
    
    if (SUCCEEDED(hr))
    {
        hr = m_spClassExtension->ProcessIoControl(pRequest);
    }

    FuncExit();

    return hr;
}
