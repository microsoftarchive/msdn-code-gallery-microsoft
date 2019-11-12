//
//    Copyright (C) Microsoft.  All rights reserved.
//
/*++
 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Module Name:

    Device.cpp

Abstract:

    This module contains the implementation for the HID sensor class driver
    device callback object.

--*/

#include "internal.h"
#include "SensorManager.h"
#include "SensorDdi.h"
#include "Device.h"
#include "Queue.h"


#include "Device.tmh"

#include <setupapi.h>
#include <devpkey.h>
#include <strsafe.h>

const DWORD c_dwInitDelay = 10000; // 10 seconds


/////////////////////////////////////////////////////////////////////////
//
// CMyDevice::CMyDevice
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CMyDevice::CMyDevice() :
    m_spWdfDevice(NULL),
    m_pSensorManager(NULL),
    m_dwShutdownControlFlags(0)
{
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDevice::~CMyDevice
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CMyDevice::~CMyDevice()
{
    SAFE_RELEASE(m_pSensorManager);
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDevice::CreateInstance
//
// This static method is used to create and initialize an instance of
// CMyDevice for use with a given hardware device.
//
// Parameters:
//      
//      pDeviceInit  - pointer to an interface used to intialize the device
//      pDriver      - pointer to an IWDFDriver interface
//
// Return Values:
//      S_OK: object created successfully
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::CreateInstance(
    _In_  IWDFDriver*           pDriver,
    _In_  IWDFDeviceInitialize* pDeviceInit
    )
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    CComObject<CMyDevice>* pMyDevice = NULL;

    HRESULT hr = CComObject<CMyDevice>::CreateInstance(&pMyDevice);

    if (SUCCEEDED(hr) && (nullptr != pMyDevice))
    {
        pMyDevice->AddRef();
    
        // Prepare device parameters
        pDeviceInit->SetLockingConstraint(WdfDeviceLevel);  // The skeleton uses device level locking
        pDeviceInit->SetPowerPolicyOwnership(FALSE);        // Power policy
        // pDeviceInit->SetFilter();                        // If you're writing a filter driver then set this flag
        // pDeviceInit->AutoForwardCreateCleanupClose(WdfTrue);

        CComPtr<IUnknown> spCallback;
        hr = pMyDevice->QueryInterface(IID_IUnknown, (void**)&spCallback);

        CComPtr<IWDFDevice> spIWDFDevice;
        if (SUCCEEDED(hr))
        {
            // Create the IWDFDevice object
            hr = pDriver->CreateDevice(pDeviceInit, spCallback, &spIWDFDevice);
        }

        //Release the pMyDevice pointer when done. Note: UMDF holds a reference to it above
        SAFE_RELEASE(pMyDevice);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDevice::OnPrepareHardware
//
//  Called by UMDF to prepare the hardware for use. In our case
//  we create the SensorDDI object and initialize the Sensor Class Extension
//
// Parameters:
//      pWdfDevice - pointer to an IWDFDevice object representing the
//      device
//
// Return Values:
//      S_OK: success
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::OnPrepareHardware(
        _In_ IWDFDevice* pWdfDevice
        )
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = (NULL != pWdfDevice) ? S_OK : E_UNEXPECTED;


    if (SUCCEEDED(hr))
    {
        hr = EnterProcessing(PROCESSING_IPNPCALLBACKHARDWARE);

        if (SUCCEEDED(hr))
        {
            if( NULL != pWdfDevice )
            {
                // Store the IWDFDevice pointer
                m_spWdfDevice = pWdfDevice;
            }

            // Create & Configure the default IO Queue
            if(SUCCEEDED(hr))
            {
                hr = ConfigureQueue();
            }

            // Create the sensor manager object
            if(SUCCEEDED(hr))
            {
                hr = CComObject<CSensorManager>::CreateInstance(&m_pSensorManager);

                if (SUCCEEDED(hr) && (nullptr != m_pSensorManager))
                {
                    m_pSensorManager->AddRef();

                    // Initialize the sensor manager object
                    if(SUCCEEDED(hr))
                    {
                        hr = m_pSensorManager->Initialize(m_spWdfDevice, this);
                    }

                    if (SUCCEEDED(hr))
                    {
                        hr = StringCchCopy(m_pSensorManager->m_wszDeviceName, MAX_PATH, DEFAULT_DEVICE_MODEL_VALUE);
    
                        if (SUCCEEDED(hr))
                        {
                            ULONG ulCchInstanceId = 0;
                            BOOL  fResult = FALSE;
                            WCHAR* wszInstanceId = nullptr;
                            WCHAR* tempStr = nullptr;

                            try 
                            {
                                wszInstanceId = new WCHAR[MAX_PATH];
                            }
                            catch(...)
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for instance ID string, hr = %!HRESULT!", hr);

                                if (nullptr != wszInstanceId) 
                                {
                                    delete[] wszInstanceId;
                                }
                            }

                            try 
                            {
                                tempStr = new WCHAR[MAX_PATH];
                            }
                            catch(...)
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for instance ID temp string, hr = %!HRESULT!", hr);

                                if (nullptr != tempStr) 
                                {
                                    delete[] tempStr;
                                }
                            }

                            if (SUCCEEDED(hr) && SUCCEEDED(pWdfDevice->RetrieveDeviceInstanceId(NULL, &ulCchInstanceId)))
                            {
                                if (SUCCEEDED(pWdfDevice->RetrieveDeviceInstanceId(wszInstanceId, &ulCchInstanceId)))
                                {
                                    HDEVINFO hDeviceInfo = INVALID_HANDLE_VALUE;
                                    if (INVALID_HANDLE_VALUE != (hDeviceInfo = ::SetupDiCreateDeviceInfoList(NULL, NULL)))
                                    {
                                        SP_DEVINFO_DATA deviceInfo = {sizeof(SP_DEVINFO_DATA)};
                                        if (TRUE == ::SetupDiOpenDeviceInfo(hDeviceInfo, wszInstanceId, NULL, 0, &deviceInfo))
                                        {
                                            DEVPROPTYPE propType;
                                            ULONG ulSize;
                
                                            fResult = ::SetupDiGetDeviceProperty(hDeviceInfo, &deviceInfo, &DEVPKEY_Device_DeviceDesc, &propType, (PBYTE)tempStr, MAX_PATH*sizeof(WCHAR), &ulSize, 0);
                                            if (FALSE == fResult)
                                            {
                                                hr = HRESULT_FROM_WIN32(GetLastError());
                                            }

                                            if (SUCCEEDED(hr) && (wcscmp(tempStr, L"") != 0))
                                            {
                                                wcscpy_s(m_pSensorManager->m_wszDeviceName, MAX_PATH, tempStr);
                                            }

                                            DWORD dwRequiredSize;
                                            fResult = ::SetupDiGetDeviceInstanceId(hDeviceInfo, &deviceInfo, wszInstanceId, MAX_PATH, &dwRequiredSize);
                                            if (FALSE == fResult)
                                            {
                                                hr = HRESULT_FROM_WIN32(GetLastError());
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = m_pSensorManager->StringTrim(wszInstanceId);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                wcscpy_s(m_pSensorManager->m_wszDeviceId, MAX_PATH, wszInstanceId);
                                                hr = m_pSensorManager->Start();

                                                if (FAILED(hr))
                                                {
                                                    Trace(TRACE_LEVEL_CRITICAL, "Failed to initialize sensors, hr = %!HRESULT!", hr);
                                                }
                                            }
                                        }
                
                                        fResult = ::SetupDiDestroyDeviceInfoList(hDeviceInfo);
                                        if (FALSE == fResult)
                                        {
                                            hr = HRESULT_FROM_WIN32(GetLastError());
                                        }
                                    }
                                }

                            }

#pragma warning(suppress: 6001) //using unitialized memory
                            if (nullptr != wszInstanceId) 
                            {
                                delete[] wszInstanceId;
                            }

#pragma warning(suppress: 6001) //using unitialized memory
                            if (nullptr != tempStr) 
                            {
                                delete[] tempStr;
                            }
                        }
                    }
                }
            }

        } // processing in progress

        ExitProcessing(PROCESSING_IPNPCALLBACKHARDWARE);
    }

    if (FAILED(hr))
    {
        Trace(TRACE_LEVEL_CRITICAL, "Abnormal results during hardware initialization, hr = %!HRESULT!", hr);
    }

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDevice::OnReleaseHardware
//
// Called by UMDF to uninitialize the hardware.
//
// Parameters:
//      pWdfDevice - pointer to an IWDFDevice object for the device
//
// Return Values:
//      S_OK:
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::OnReleaseHardware(
        _In_ IWDFDevice* pWdfDevice
        )
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");
    UNREFERENCED_PARAMETER(pWdfDevice);

    EnterShutdown();

    HRESULT hr = S_OK;

    // Uninitialize the sensor manager object
    if (NULL != m_pSensorManager)
    {
        hr = m_pSensorManager->Stop();

        if (SUCCEEDED(hr))
        {
            if (NULL != m_pSensorManager->m_pSensorDDI)
            {
                if (NULL != m_pSensorManager->m_pSensorDDI->m_spHidWdfFile)
                {
                    m_pSensorManager->m_pSensorDDI->m_spHidWdfFile->Close();
                }
            }
        }

        m_pSensorManager->Uninitialize();

        SAFE_RELEASE(m_pSensorManager);
    }

    // Release the IWDFDevice handle, if it matches
    if (pWdfDevice == m_spWdfDevice.p)
    {
        m_spWdfDevice.Release();
    }


    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    Trace(TRACE_LEVEL_CRITICAL, "SensorsHIDClassDriver - Trace log ending for this instance of driver");
    Trace(TRACE_LEVEL_CRITICAL, "------------------------------- END -------------------------------------------");

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDevice::OnD0Entry
//
// This method is called after a new device enters the system
//
// Parameters:
//      pWdfDevice - pointer to a device object
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::OnD0Entry(
        _In_ IWDFDevice* pWdfDevice,
        _In_ WDF_POWER_DEVICE_STATE previousState
        )
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");
    UNREFERENCED_PARAMETER(pWdfDevice);
    UNREFERENCED_PARAMETER(previousState);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_IPNPCALLBACK);

    if (SUCCEEDED(hr) 
        && (previousState == WdfPowerDeviceD3 || previousState == WdfPowerDeviceD3Final))
    {
        if( NULL != m_pSensorManager 
            && NULL != m_pSensorManager->m_pSensorDDI)
        {
            if (TRUE == m_pSensorManager->m_pSensorDDI->m_fDeviceIdle)
            {
                hr = m_pSensorManager->m_pSensorDDI->InitSensorDevice(m_pSensorManager->m_spWdfDevice);
            }

            if (!SUCCEEDED(hr))
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Unable to re-initialize sensor device, hr = %!HRESULT!", hr);
            }
            else
            {
                m_pSensorManager->m_pSensorDDI->OnDeviceReconnect();
            }
        }
    } // processing in progress

    ExitProcessing(PROCESSING_IPNPCALLBACK);

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDevice::OnD0Exit
//
// This method is called when a device leaves the system
//
// Parameters:
//      pWdfDevice - pointer to a device object
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::OnD0Exit(
        _In_ IWDFDevice* pWdfDevice,
        _In_ WDF_POWER_DEVICE_STATE newState
        )
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    UNREFERENCED_PARAMETER(pWdfDevice);
    UNREFERENCED_PARAMETER(newState);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_IPNPCALLBACK);

    if (SUCCEEDED(hr) 
        && (newState == WdfPowerDeviceD3 || newState == WdfPowerDeviceD3Final))
    {
        if( NULL != m_pSensorManager 
            && NULL != m_pSensorManager->m_pSensorDDI)
        {
            m_pSensorManager->m_pSensorDDI->DeInitSensorDevice();
        }

    } // processing in progress

    ExitProcessing(PROCESSING_IPNPCALLBACK);

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}

VOID CMyDevice::OnSurpriseRemoval(
    _In_ IWDFDevice* pWdfDevice
    )
{
    UNREFERENCED_PARAMETER(pWdfDevice);
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    EnterShutdown();

    return;
}

HRESULT CMyDevice::OnQueryRemove(
        _In_ IWDFDevice* pWdfDevice
        )
{
    UNREFERENCED_PARAMETER(pWdfDevice);
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    return S_OK;
}

HRESULT CMyDevice::OnQueryStop(
        _In_ IWDFDevice* pWdfDevice
        )
{
    UNREFERENCED_PARAMETER(pWdfDevice);
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    return S_OK;
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDevice::OnCleanupFile
//
// This method is called when the file handle to the device is closed
//
// Parameters:
//      pWdfFile - pointer to a file object
//
/////////////////////////////////////////////////////////////////////////
VOID CMyDevice::OnCleanupFile(
            _In_ IWDFFile* pWdfFile
            )
{
    //Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_IFILECALLBACKCLEANUP);

    if (SUCCEEDED(hr))
    {
        if (NULL != m_pSensorManager)
        {
            m_pSensorManager->CleanupFile(pWdfFile);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_IFILECALLBACKCLEANUP);


    return;
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDevice::ConfigureQueue
//  
//  This method is called after the device callback object has been initialized 
//  and returned to the driver.  It would setup the device's queues and their 
//  corresponding callback objects.
//
// Parameters:
//
// Return Values:
//      S_OK: success
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::ConfigureQueue()
{
    HRESULT hr = S_OK;
    CComPtr<IWDFIoQueue> spIoQueue;

    if( NULL != m_spWdfDevice )
    {
        m_spWdfDevice->GetDefaultIoQueue(&spIoQueue);
    }
    else
    {
        hr = E_UNEXPECTED;
    }

    if(SUCCEEDED(hr) && ( NULL == spIoQueue ))
    {
        hr = CMyQueue::CreateInstance(m_spWdfDevice, this);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDevice::ProcessIoControl
//
// This method is a helper that takes the incoming IOCTL and forwards
// it to the Windows Sensor Class Extension for processing.
//
// Parameters:
//      pQueue                  - [in] pointer to the UMDF queue that handled the request
//      pRequest                - [in] pointer to the request
//      ControlCode             - [in] the IOCTL code
//      InputBufferSizeInBytes  - [in] size of the incoming IOCTL buffer
//      OutputBufferSizeInBytes - [out] size of the outgoing IOCTL buffer
//      pcbWritten              - pointer to a DWORD containing the number of bytes returned
//
// Return Values:
//      S_OK:
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDevice::ProcessIoControl(_In_ IWDFIoQueue*     pQueue,
                                    _In_ IWDFIoRequest*   pRequest,
                                    _In_ ULONG            ControlCode,
                                         SIZE_T           InputBufferSizeInBytes,
                                         SIZE_T           OutputBufferSizeInBytes,
                                         DWORD*           pcbWritten)
{
    UNREFERENCED_PARAMETER(pQueue);
    UNREFERENCED_PARAMETER(ControlCode);
    UNREFERENCED_PARAMETER(InputBufferSizeInBytes);
    UNREFERENCED_PARAMETER(OutputBufferSizeInBytes);
    UNREFERENCED_PARAMETER(pcbWritten);

    HRESULT hr = S_OK;

    if(NULL != m_pSensorManager)
    {
        hr = m_pSensorManager->ProcessIoControl(pRequest);
    }
    else
    {
        hr = E_UNEXPECTED;
    }

    return hr;
}

inline HRESULT CMyDevice::EnterProcessing(DWORD64 dwControlFlag)
{
    //Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    if ((InterlockedOr (&m_dwShutdownControlFlags, dwControlFlag) & 
            SHUTDOWN_IN_PROGRESS) != 0)
    {
        hr = HRESULT_FROM_WIN32(ERROR_SHUTDOWN_IN_PROGRESS);
    }

    return hr;
}

inline void CMyDevice::ExitProcessing(DWORD64 dwControlFlag)
{
    //Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    InterlockedAnd (&m_dwShutdownControlFlags, ~dwControlFlag);
}

inline void CMyDevice::EnterShutdown()
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");
    //
    //  Begin shutdown.  Spin if control handler is in progress.
    //
    while (((InterlockedOr (&m_dwShutdownControlFlags, SHUTDOWN_IN_PROGRESS) & PROCESSING_IN_PROGRESS) != 0))
    {
        Yield();
    }
}

