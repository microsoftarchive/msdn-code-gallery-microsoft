//-----------------------------------------------------------------------
// <copyright file="WSSDevice.cpp" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
// </copyright>
//
// Module:
//      WSSDevice.cpp
//
// Description:
//      This file implements the CWSSDevice class.
//
//-----------------------------------------------------------------------

#include "Common.h"
#include "WSSDevice.h"
#include <WindowsSideShowDriverEvents.h>

#include <setupapi.h>
#include <strsafe.h>
#include <devguid.h>

// Basic Driver
#include "DataManager.h"
#include "Renderer.h"
#include "DeviceSpecific.h"

bool g_fDeviceIsValid = false;
CDevice g_Device; // Singleton object
CDataManager* g_pDataManager = NULL;


/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::CWSSDevice
//
// Object constructor function; initializes various members
//
/////////////////////////////////////////////////////////////////////////
CWSSDevice::CWSSDevice() :
m_pBasicDriver(NULL)
{
}

/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::~CWSSDevice
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CWSSDevice::~CWSSDevice()
{
}

/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::FinalRelease
//
// Called by ATL before deleting this object.
//
/////////////////////////////////////////////////////////////////////////
void CWSSDevice::FinalRelease()
{
    if (NULL != m_pBasicDriver)
    {
        m_pBasicDriver->Release();
        m_pBasicDriver = NULL;
    }
}

/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::FinalConstruct()
//
// Called by ATL after creating this object.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWSSDevice::FinalConstruct()
{
    HRESULT hr = S_OK;

    //
    // Create the Driver object which implements ISideShowEnhancedDriver
    //
    hr = CComObject<CWssBasicDDI>::CreateInstance(&m_pBasicDriver);
    if (SUCCEEDED(hr) && (NULL != m_pBasicDriver))
    {
        m_pBasicDriver->AddRef();
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::CreateDeviceInstance
//
// This static method is used to create and initialize an instance of
// CWSSDevice for use with a given hardware device.
//
// Parameters:
//      out_ppDevice - pointer to an instance of CWSSDevice (out ptr)
//      pDeviceInit  - pointer to an interface used to initialize the device
//
// Return Values:
//      S_OK: object created successfully
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWSSDevice::CreateDeviceInstance(
    WSSDevicePtr* out_ppDevice,
    IWDFDeviceInitialize* pDeviceInit
    )
{
    if (NULL == out_ppDevice)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;
    CComObject<CWSSDevice>* pMyDevice = NULL;

    //
    // Set device properties.
    //
    pDeviceInit->SetLockingConstraint(WdfDeviceLevel);

    //
    // Make sure we aren't the power policy owner
    //
    pDeviceInit->SetPowerPolicyOwnership(FALSE);

    //
    // Create the device instance
    //
    hr = CComObject<CWSSDevice>::CreateInstance(&pMyDevice);

    if ((SUCCEEDED(hr)) && (NULL != pMyDevice))
    {
        pMyDevice->AddRef();
        *out_ppDevice = pMyDevice;
    }
    else
    {
        *out_ppDevice = NULL;
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::OnPrepareHardware
//
// Called by UMDF to prepare the hardware for use.
//
// Parameters:
//      pWdfDevice - pointer to an IWDFDevice object representing the
//      device
//
// Return Values:
//      S_OK: success
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWSSDevice::OnPrepareHardware(IWDFDevice* pWdfDevice)
{
    if (NULL == m_pBasicDriver)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;

    //
    // Store the IWDFDevice pointer locally
    //
    m_pWdfDevice = pWdfDevice;

    hr = BasicDriverInitialization();

    if (SUCCEEDED(hr))
    {
        hr = InitializeClassExtension(); // Register the driver with the class extension
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::OnReleaseHardware
//
// Called by WUDF to uninitialize the hardware.
//
// Parameters:
//      pWdfDevice - pointer to an IWDFDevice object for the device
//
// Return Values:
//      S_OK:
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWSSDevice::OnReleaseHardware(IWDFDevice* pWdfDevice)
{
    HRESULT hr = S_OK;

    //
    // Cleanup the DDI object
    //
    if (NULL != m_pBasicDriver)
    {
        hr = m_pBasicDriver->Deinitialize();
    }

    //
    // Shutdown the device specific components
    //
    BasicDriverShutdown();

    //
    // Release the Class Extension
    //
    if (NULL != m_pClassExtension2.p)
    {
        m_pClassExtension2->Uninitialize(pWdfDevice);
        m_pClassExtension2.Release();
    }
    else
    {
        m_pClassExtension1->Uninitialize(pWdfDevice);
        m_pClassExtension1.Release();
    }

    //
    // Release the IWDFDevice handle, if it matches
    //
    if (pWdfDevice == m_pWdfDevice.p)
    {
        m_pWdfDevice.Release();
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::OnCleanupFile
//
// This method is called when the file handle to the device is closed
//
// Parameters:
//      pFileObject - pointer to a file object
//
/////////////////////////////////////////////////////////////////////////
void CWSSDevice::OnCleanupFile(/*[in]*/ IWDFFile* pFileObject)
{
    if (NULL != m_pClassExtension2.p)
    {
        m_pClassExtension2->CleanupFile(pFileObject);
    }
    // else No work if using ISideShowClassExtension

}


/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::InitializeClassExtension
//
// This method initializes the Windows SideShow class extension component.
//
// Return Values:
//      S_OK:
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWSSDevice::InitializeClassExtension()
{
    HRESULT hr = S_OK;

    // Construct an ISideShowClassExtension or ISideShowClassExtension2 object to process IOCTLs.

    hr = m_pClassExtension1.CoCreateInstance(CLSID_SideShowClassExtension);

    if (SUCCEEDED(hr))
    {
        hr = m_pClassExtension1.QueryInterface(&m_pClassExtension2);
        if (E_NOINTERFACE == hr)
        {
            // This is an acceptable error. This means the driver is running on Windows Vista.
            // At this point, m_pClassExtension2.p == NULL. This info will be used to determine
            // which calls should be made on the Class Extension elsewhere in this driver.
            hr = S_OK;
        }
    }

    // Create an instance of our Driver and register it with the Windows SideShow
    // class extension object.  The Windows SideShow class extension Object is now ready to
    // accept forwarded WPD IOCTLs.
    if (SUCCEEDED(hr))
    {
        if (NULL != m_pClassExtension2.p)
        {
            // Register this driver with the ISideShowClassExtension2 object
            hr = m_pClassExtension2->InitializeAsync(m_pWdfDevice, m_pBasicDriver);
        }
        else
        {
            // Register this driver with the ISideShowClassExtension object
            hr = m_pClassExtension1->Initialize(m_pWdfDevice, m_pBasicDriver);
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::ProcessIoControl
//
// This method is a helper that takes the incoming IOCTL and forwards
// it to the Windows SideShow Class Extension for processing.
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
HRESULT CWSSDevice::ProcessIoControl(/* [in] */ IWDFIoQueue*     pQueue,
                                     /* [in] */ IWDFIoRequest*   pRequest,
                                     /* [in] */ ULONG            ControlCode,
                                     /* [in] */ SIZE_T           InputBufferSizeInBytes,
                                     /* [in] */ SIZE_T           OutputBufferSizeInBytes,
                                     /* [out]*/ DWORD*           pcbWritten,
                                     /* [out]*/ BOOL*            pfCompleteRequest)
{
    if ( ((NULL == m_pClassExtension2.p) && (NULL == m_pClassExtension1.p)) ||
         (NULL == pfCompleteRequest) )
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;

    if (NULL != m_pClassExtension2.p)
    {
        hr = m_pClassExtension2->OnProcessIoControl(pQueue,
                                                    pRequest,
                                                    ControlCode,
                                                    InputBufferSizeInBytes,
                                                    OutputBufferSizeInBytes,
                                                    pcbWritten);
        *pfCompleteRequest = FALSE;
    }
    else
    {
        hr = m_pClassExtension1->OnProcessIoControl(pQueue,
                                                    pRequest,
                                                    ControlCode,
                                                    InputBufferSizeInBytes,
                                                    OutputBufferSizeInBytes,
                                                    pcbWritten);
        *pfCompleteRequest = TRUE;
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::BasicDriverInitialization
//
// This method initializes the driver
//
// Parameters:
//      void - (unused argument)
//
// Return Values:
//      S_OK:
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWSSDevice::BasicDriverInitialization(void)
{
    HRESULT hr = S_OK;

    // Initialize the Basic DDI
    CComPtr<IWDFNamedPropertyStore> pStore;

    hr = m_pWdfDevice->RetrieveDevicePropertyStore(NULL, WdfPropertyStoreCreateIfMissing, &pStore, NULL);
    if (SUCCEEDED(hr))
    {
        hr = m_pBasicDriver->Initialize(pStore);
    }

    // Order of initialization: Device display, GDIPlus, DataManager, Device buttons (second thread)
    if (SUCCEEDED(hr))
    {
        if (S_OK == DeviceDisplayInitialization())
        {
            g_fDeviceIsValid = true;
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = CRendererBase::GDIPlusInitialization();
    }

    if (SUCCEEDED(hr))
    {
        g_pDataManager = new(std::nothrow) CDataManager(CNodeDefaultBackground(CRendererBase::GetDefaultBackgroundTitle(), (wcslen(CRendererBase::GetDefaultBackgroundTitle()) + 1),
                                                                               CRendererBase::GetDefaultBackgroundBody(), (wcslen(CRendererBase::GetDefaultBackgroundBody()) + 1),
                                                                               NULL, 0)); // Singleton object
        if (NULL == g_pDataManager)
        {
            // ERROR: new g_pDataManager failed: Critical failure
            hr = E_OUTOFMEMORY;
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = DeviceButtonsInitialization();
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CWSSDevice::BasicDriverShutdown
//
// This method cleans up the driver
//
// Parameters:
//      void - (unused argument)
//
// Return Values:
//      S_OK:
//
/////////////////////////////////////////////////////////////////////////
HRESULT CWSSDevice::BasicDriverShutdown(void)
{
    // Order of shutdown: Device buttons (second thread), DataManager, GDIPlus, Device display
    DeviceButtonsShutdown();

    if (NULL != g_pDataManager)
    {
        delete g_pDataManager;
        g_pDataManager = NULL;
    }

    CRendererBase::GDIPlusShutdown();

    DeviceDisplayShutdown();

    return S_OK;
}

/*
Eventing Example

Changes to Windows 7 SideShow drivers:
If you are writing a Windows 7 SideShow driver, this driver model uses a new eventing model called Authorized Eventing.
This model secures the event data that flows between the driver and the client application. This differs from the Vista
SideShow driver model, where event data is posted to client applications via PnP events. While it is still possible to
use the Vista method to post SideShow event data in Windows 7, it is recommended to use the new Windows 7 model when
running on Windows 7.

In both cases, you first need event data. For example, the following creates an event:

    DWORD dwSizeOfEventData = _Some_Size_Value;
    DWORD dwSizeOfEntireBuffer = offsetof(APPLICATION_EVENT_DATA, bEventData) + dwSizeOfEventData;
    APPLICATION_EVENT_DATA* pApplicationEventData = (APPLICATION_EVENT_DATA*)CoTaskMemAlloc(dwSizeOfEntireBuffer);
    if (NULL != pApplicationEventData)
    {
        pApplicationEventData->cbApplicationEventData = dwSizeOfEntireBuffer;
        pApplicationEventData->ApplicationId = _Some_Application_ID_(GUID);
        pApplicationEventData->EndpointId = _Some_Endpoint_ID_(GUID);
        pApplicationEventData->dwEventId = _Some_Event_ID;
        pApplicationEventData->cbEventData = dwSizeOfEventData;

        // This copies event data starting at bEventData in the structure
        memcpy(pApplicationEventData + offsetof(APPLICATION_EVENT_DATA, bEventData), _Some_Event_Data, dwSizeOfEventData);
    }

Now you have your event "pApplicationEventData". You can post this event by doing the following. Note, this will use
Windows 7 Authorized Eventing when running on Windows 7 and Vista's eventing (PnP events) when running on Vista

    HRESULT hr = S_OK;

    // Check to see if you are using the ISideShowClassExtension2 interface (Windows 7 SideShow driver model)
    if (NULL != m_pClassExtension2.p)
    {
        // Post this event using the Windows 7 SideShow driver model
        hr = m_pClassExtension2->PostEvent(SIDESHOW_APPLICATION_EVENT,
                                           NULL,
                                           (BYTE*)pApplicationEventData,
                                           pApplicationEventData->cbApplicationEventData);
    }
    else // using the ISideShowClassExtension interface (Vista SideShow driver model)
    {
        // Post this event using the Vista SideShow driver model
        hr = m_pWdfDevice->PostEvent(SIDESHOW_APPLICATION_EVENT,
                                     WdfEventBroadcast,
                                     (BYTE*)pApplicationEventData,
                                     pApplicationEventData->cbApplicationEventData);
    }

And finally, release the memory used by the event:

    CoTaskMemFree(pApplicationEventData);

Please see the documentation for ISideShowClassExtension2::PostEvent for more details.
Please see WindowsSideShowDriverEvents.h for more details on how to create an event.
*/
