//
//    Copyright (C) Microsoft.  All rights reserved.
//
/*

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Module Name:
    SensorManager.cpp

Abstract:
    Implements the CSensorManager container class

--*/

#include "internal.h"
#include "SensorDDI.h"
#include "SensorManager.h"
#include "Device.h"


#include "Sensor.h"
#include "Geolocation.h"

#include "SensorManager.tmh"

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::CSensorManager
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CSensorManager::CSensorManager() :
    m_spWdfDevice(NULL),
    m_spClassExtension(NULL),
    m_pSensorDDI(NULL),
    m_hSensorEvent(NULL),
    m_hSensorManagerEventingThread(NULL),
    m_fSensorManagerInitialized(FALSE),
    m_fThreadActive(FALSE),
    m_pDevice(NULL),
    m_fDeviceActive(false)
{

}


/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::~CSensorManager
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CSensorManager::~CSensorManager()
{
    SAFE_RELEASE(m_pSensorDDI);
    SAFE_RELEASE(m_pDevice);
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::Initialize
//
// Merely store the device pointer.  The rest of the init will be done
// in Start().  This is because init depends on communication with the
// device.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::Initialize(_In_ IWDFDevice* pWdfDevice, _In_ CMyDevice* pDevice)
{
    HRESULT hr = (FALSE == IsInitialized()) ? S_OK : E_UNEXPECTED;

    if(SUCCEEDED(hr))
    {        
        // Store the IWDF Device pointer
        m_spWdfDevice = pWdfDevice;
        m_fInitializationComplete = FALSE;
        m_NumMappedSensors = 0;
        m_pDevice = pDevice;
        m_pDevice->AddRef();
        m_fDeviceStopped = FALSE;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::Uninitialize
//
//
//
//
/////////////////////////////////////////////////////////////////////////
void CSensorManager::Uninitialize()
{
    // Free all the sensor objects
    for(DWORD i = 0; i < m_pSensorList.GetCount(); i++)
    {
        CSensor *pSensor = m_pSensorList.GetAt(m_pSensorList.FindIndex(i));

        if (nullptr != pSensor)
        {
            pSensor->Uninitialize();

            delete pSensor;
        }
    }

    // Clear the Sensor list and Sensor map
    m_pSensorList.RemoveAll();
    m_AvailableSensorsIDs.RemoveAll();
    m_AvailableSensorsTypes.RemoveAll();

    // Release Sensor Class Extension and Sensor DDI
    if(NULL != m_spClassExtension)
    {
        m_spClassExtension->Uninitialize();
        m_spClassExtension.Release();
    }

    SAFE_RELEASE(m_pSensorDDI);

    m_fSensorManagerInitialized = FALSE;

    return;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::InitializeClassExtension
//
//
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::InitializeClassExtension()
{
    HRESULT hr = (NULL == m_spClassExtension) ? S_OK : E_UNEXPECTED;
    
    if(SUCCEEDED(hr))
    {
        // CoCreate the Sensor ClassExtension
        if(SUCCEEDED(hr))
        {
            hr =  CoCreateInstance(CLSID_SensorClassExtension,
                                    NULL,
                                    CLSCTX_INPROC_SERVER,
                                    IID_PPV_ARGS(&m_spClassExtension));
            
            if (REGDB_E_CLASSNOTREG == hr)
            {
                Trace(TRACE_LEVEL_ERROR, "Class is not registered, hr = %!HRESULT!", hr);
                hr = E_UNEXPECTED;
                m_spClassExtension = NULL;
            }
        }

        // Initialize Sensor ClassExtension
        if(SUCCEEDED(hr))
        {
            CComPtr<IUnknown> spIUnknown;
            hr = m_pSensorDDI->QueryInterface(IID_IUnknown, (void**)&spIUnknown);
            
            if(SUCCEEDED(hr))
            {
                if ( NULL != m_spClassExtension )
                {
                    if (nullptr != m_spWdfDevice)
                    {
                        hr = m_spClassExtension->Initialize(m_spWdfDevice, spIUnknown);
                    }
                    else
                    {
                        hr = E_POINTER;
                    }

                    if (SUCCEEDED(hr))
                    {
                        hr = m_spWdfDevice->CreateDeviceInterface(&GUID_DEVINTERFACE_GPS_RADIO_MANAGEMENT, nullptr);
                        Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! CreateDeviceInterface for Radio management returned hr = %!HRESULT!", hr);

                        if (SUCCEEDED(hr))
                        {
                            hr = m_spWdfDevice->AssignDeviceInterfaceState(&GUID_DEVINTERFACE_GPS_RADIO_MANAGEMENT, nullptr, TRUE);
                        }
                    }
                }
            }
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::Start
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::Start()
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr;

    // Create the sensor DDI if not done already
    if (NULL == m_pSensorDDI)
    {
        //
        // Create the SensorDDI object that implements ISensorDriver
        //
        hr = CComObject<CSensorDDI>::CreateInstance(&m_pSensorDDI);
        if ((SUCCEEDED(hr)) && (NULL != m_pSensorDDI))
        {
            m_pSensorDDI->AddRef();
        }
    }

    // Always initialize the sensor DDI on Start()
    if(NULL != m_pSensorDDI)
    {
        hr = m_pSensorDDI->InitSensorDevice(m_spWdfDevice);
        m_pSensorDDI->m_pSensorManager = this;
    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "SensorDDI pointer is NULL, hr = %!HRESULT!", hr);
    }

    // Init sensor driver the first time Start() is called.  This is done
    // here vs. in Initialize() because we need to contact the device to
    // determine its type before init.
    if (SUCCEEDED(hr) && FALSE == IsInitialized())
    {
        // Get the device's report description to determine
        // the sensor and report type
        SensorType sensType;

        WCHAR* tempStr = nullptr;
        WCHAR* sensorID = nullptr;
        WCHAR* deviceID = nullptr;
        WCHAR* pwszManufacturer = nullptr;
        WCHAR* pwszProduct = nullptr;
        WCHAR* pwszSerialNumber = nullptr;

        try 
        {
            tempStr = new WCHAR[MAX_PATH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for temp string, hr = %!HRESULT!", hr);

            if (nullptr != tempStr) 
            {
                delete[] tempStr;
            }
        }

        try 
        {
            sensorID = new WCHAR[MAX_PATH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for sensor ID string, hr = %!HRESULT!", hr);

            if (nullptr != sensorID) 
            {
                delete[] sensorID;
            }
        }

        try 
        {
            deviceID = new WCHAR[MAX_PATH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for device ID string, hr = %!HRESULT!", hr);

            if (nullptr != deviceID) 
            {
                delete[] deviceID;
            }
        }

        try 
        {
            pwszManufacturer = new WCHAR[MAX_PATH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for manufacturer string, hr = %!HRESULT!", hr);

            if (nullptr != pwszManufacturer) 
            {
                delete[] pwszManufacturer;
            }
        }

        try 
        {
            pwszProduct = new WCHAR[MAX_PATH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for product string, hr = %!HRESULT!", hr);

            if (nullptr != pwszProduct) 
            {
                delete[] pwszProduct;
            }
        }

        try 
        {
            pwszSerialNumber = new WCHAR[MAX_PATH];
        }
        catch(...)
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for serial number string, hr = %!HRESULT!", hr);

            if (nullptr != pwszSerialNumber) 
            {
                delete[] pwszSerialNumber;
            }
        }

        if (SUCCEEDED(hr))
        {
            int numSensors = 0;

#pragma warning(push)
#pragma warning(disable:26035)
            // the OACR warning is being disabled here because it expects a null-terminated string
            // for this particular use. However, string termination happens in the called method
            hr = m_pSensorDDI->RequestDeviceInfo(&sensType, pwszManufacturer, pwszProduct, pwszSerialNumber, deviceID);
#pragma warning(pop)

            if (SUCCEEDED(hr))
            {
                CSensor*            pSensor = NULL;
            CGeolocation* pGeolocation = NULL;

                if (sensType == Collection)
                {
                    numSensors = (int)m_AvailableSensorsTypes.GetCount();
                }
                else
                {
                    numSensors = 1;
                }

                m_NumMappedSensors = numSensors;

                //Build the sensor objects from the sensor map
                if((SUCCEEDED(hr)) && (numSensors > 0))
                {
                    for (int idx = 0; idx < numSensors; idx++)
                    {
                        if (((sensType < FirstSensorType) || (sensType > LastSensorType)) && sensType != Collection)
                        {
                            hr = E_UNEXPECTED;
                            Trace(TRACE_LEVEL_ERROR, "Invalid sensor type, hr = %!HRESULT!", hr);
                        }
                        else
                        {
                            switch (m_AvailableSensorsTypes[idx])
                            {

                            case Geolocation:
                                wcscpy_s(sensorID, DESCRIPTOR_MAX_LENGTH, deviceID);
                                swprintf_s(tempStr, SENSOR_ID_APPENDIX_MAX_LENGTH, L"-%i", idx);
                                wcscat_s(sensorID, DESCRIPTOR_MAX_LENGTH, tempStr);
                                m_AvailableSensorsIDs[idx] = sensorID;
                                pGeolocation = new CGeolocation();
                                if (NULL != pGeolocation)
                                {
                                    hr = pGeolocation->Initialize(
                                            Geolocation, 
                                            idx, 
                                            pwszManufacturer,
                                            pwszProduct,
                                            pwszSerialNumber,
                                            sensorID,
                                            m_spWdfDevice,
                                            this);
                                }
                                else
                                {
                                    hr = E_UNEXPECTED;
                                    Trace(TRACE_LEVEL_ERROR, "Unable to create Geolocation object, hr = %!HRESULT!", hr);
                                }
                                if(SUCCEEDED(hr))
                                {
                                    pSensor = (CSensor*)pGeolocation;
                                    Trace(TRACE_LEVEL_INFORMATION, "Geolocation sensor successfully created, Sensor ID = %ls, hr = %!HRESULT!", sensorID, hr);
                                }
                                break;

                            default:
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Invalid sensor type, hr = %!HRESULT!", hr);
                                break;
                            }
                        }
                    
                        // Set the unique persistent ID
                        if(SUCCEEDED(hr))
                        {
                            hr = pSensor->SetUniqueID(m_spWdfDevice);
                        }
                    
                        if(SUCCEEDED(hr))
                        {
                            m_pSensorList.AddTail(pSensor);
                        }
                    
                        if (SUCCEEDED(hr))
                        {
                            CComPtr<IPortableDeviceValues> spReportInterval = NULL;
                            hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                                  NULL,
                                                  CLSCTX_INPROC_SERVER,
                                                  IID_IPortableDeviceValues,
                                                  (VOID**)&spReportInterval);
                            if (SUCCEEDED(hr))
                            {
                                hr = spReportInterval->SetUnsignedIntegerValue(SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, DEFAULT_SLEEP_REPORT_INTERVAL);
                            }

                            if (SUCCEEDED(hr))
                            {
                                hr = m_pSensorDDI->UpdateSensorPropertyValues(sensorID, FALSE);
                            }

                            spReportInterval.Release();
                        }
                    }
                }

                // Create and initialize the Sensor Class Extension
                if(SUCCEEDED(hr))
                {
                    hr = InitializeClassExtension();
                }

                if(SUCCEEDED(hr))
                {
                    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Sensor Class Extension initialized");
                    m_fSensorManagerInitialized = TRUE;
                    m_fInitializationComplete = TRUE;
                }
            }
            else
            {
                Trace(TRACE_LEVEL_ERROR, "%!FUNC! Failed to get device info");
            }
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != tempStr) 
        {
            delete[] tempStr;
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != sensorID) 
        {
            delete[] sensorID;
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != deviceID) 
        {
            delete[] deviceID;
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != pwszManufacturer) 
        {
            delete[] pwszManufacturer;
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != pwszProduct) 
        {
            delete[] pwszProduct;
        }

#pragma warning(suppress: 6001) //uninitialized memory
        if (nullptr != pwszSerialNumber) 
        {
            delete[] pwszSerialNumber;
        }
    }
    
    // Update each sensor state to SENSOR_STATE_NO_DATA
    // Note: CSensorManager::_SensorEventThreadProc() will change sensor state to SENSOR_STATE_READY
    //   whenever a data field is ready.
    if (SUCCEEDED(hr))
    {
        for(DWORD i = 0; i < m_pSensorList.GetCount(); i++)
        {
            CSensor *pSensor;
            
            POSITION pos = NULL;
            pos = m_pSensorList.FindIndex(i);

            if (NULL != pos)
            {
                pSensor = m_pSensorList.GetAt(pos);

                if(nullptr != pSensor)
                {
                    bool fStateChanged = FALSE;

                    if (Geolocation == pSensor->m_SensorType)
                    {
                        SetState(pSensor, SENSOR_STATE_INITIALIZING, &fStateChanged);
                    }
                    else
                    {
                        SetState(pSensor, SENSOR_STATE_NO_DATA, &fStateChanged);
                    }
                }
                else
                {
                    hr = E_UNEXPECTED;
                }
            }
            else
            {
                hr = E_UNEXPECTED;
            }
        }
    }
    
    if (SUCCEEDED(hr))
    {  
        // Step 1: Create the Data Changed Event Handle
        m_hSensorEvent = ::CreateEvent(  NULL,        // No security attributes
                                        FALSE,       // Automatic-reset event object
                                        FALSE,       // Initial state is non-signaled
                                        NULL  );     // Unnamed object
     
        POSITION pos = NULL;
        CSensor* pSensor = nullptr;

        for(DWORD i = 0; i < m_pSensorList.GetCount(); i++)
        {
            pos = m_pSensorList.FindIndex(i);
            if (NULL != pos)
            {
                pSensor = m_pSensorList.GetAt(pos);
                if (nullptr != pSensor)
                {
                    pSensor->SetDataEventHandle(m_hSensorEvent);
                    pSensor->m_fInitialDataReceived = FALSE;
                }
                else
                {
                    hr = E_UNEXPECTED;
                }
            }
            else
            {
                hr = E_UNEXPECTED;
            }
        }

        // Step 2: Activate & Create and start the eventing thread
        if (SUCCEEDED(hr))
        {
            Activate();

            m_hSensorManagerEventingThread = ::CreateThread(NULL,                                              // Cannot be inherited by child process
                                                         0,                                                   // Default stack size
                                                         &CSensorManager::_SensorEventThreadProc,   // Thread proc
                                                         (LPVOID)this,                                        // Thread proc argument
                                                         0,                                                   // Starting state = running
                                                         NULL);

            if (nullptr == m_hSensorManagerEventingThread)
            {
                Trace(TRACE_LEVEL_ERROR, "%!FUNC! sensor event thread failed to create");
                hr = HRESULT_FROM_WIN32(::GetLastError());
            }
            else
            {
                Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! sensor event thread successfully created");
            }

        }// No thread identifier

        if(SUCCEEDED(hr))
        {
            m_fSensorManagerInitialized = TRUE;
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Sensor initialization completed successfully");
        }
        else
        {
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Sensor initialization failed");
        }
    }

    if (SUCCEEDED(hr))
    {
        m_fDeviceStopped = FALSE;
    }


    return hr;
}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::Stop
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::Stop(_In_ WDF_POWER_DEVICE_STATE newState)
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = S_OK;

    if (FALSE == m_fDeviceStopped)
    {
        if (WdfPowerDeviceD2 != newState)
        {
            // Update each sensor state to SENSOR_STATE_NO_DATA
            for(DWORD i = 0; i < m_pSensorList.GetCount(); i++)
            {
                CSensor *pSensor = nullptr;
                CComBSTR objectId;
                POSITION pos = NULL;

                pos = m_pSensorList.FindIndex(i);
                if (NULL != pos)
                {
                    pSensor = m_pSensorList.GetAt(pos);

                    if(nullptr != pSensor)
                    {
                        bool fStateChanged = FALSE;
                        if (Geolocation == pSensor->m_SensorType)
                        {
                            // NOTE: Only geolocation devices are initialized to 
                            // SENSOR_STATE_INITIALIZING. All other sensors are initialized
                            // to SENSOR_STATE_NO_DATA
                            SetState(pSensor, SENSOR_STATE_INITIALIZING, &fStateChanged);
                        }
                        else
                        {
                            SetState(pSensor, SENSOR_STATE_NO_DATA, &fStateChanged);
                        }
                    }
                    else
                    {
                        hr = E_UNEXPECTED;
                    }
                }
                else
                {
                    hr = E_UNEXPECTED;
                }
            }
        }

        // DeInitialize the sensor device
        if(NULL != m_pSensorDDI)
        {
            hr = m_pSensorDDI->DeInitSensorDevice();
        }

        // Step 1: Stop the eventing thread and Close the handle
        if (NULL != m_hSensorManagerEventingThread)
        {
            // De-activate and close the thread
            DeActivate();
            WaitForSingleObject(m_hSensorManagerEventingThread, INFINITE);
            CloseHandle(m_hSensorManagerEventingThread);
        }
   
        // Step 2: Close the Data Change Event handle
        if (NULL != m_hSensorEvent)
        {
            CloseHandle(m_hSensorEvent);
        }

        m_fDeviceStopped = TRUE;
    }

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;

}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::_SensorEventThreadProc
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
DWORD WINAPI CSensorManager::_SensorEventThreadProc(_In_ LPVOID pvData)
{
    CSensorManager* pParent = (CSensorManager*)pvData;

    if (NULL == pParent)
    {
        return 0;
    }

	// Cast the argument to the correct type.
    CSensorManager* pThis = static_cast<CSensorManager*>(pvData);

    HRESULT hr = CoInitializeEx(NULL, COINIT_MULTITHREADED);

    if (FAILED(hr))
    {
        Trace(TRACE_LEVEL_ERROR, "Failed to call CoInitialize on _SensorEventThreadProc thread, hr = %!HRESULT!", hr);
        return 0;
    }
    
    // Create the event parameters collection if it doesn't exist
    CComPtr<IPortableDeviceValues> spEventParams;
    if (spEventParams == NULL)
    {
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_IPortableDeviceValues,
                              (VOID**)&spEventParams);
    }

    if (FAILED(hr))
    {
        Trace(TRACE_LEVEL_ERROR, "Failed to CoCreateInstance for Event Parameters, hr = %!HRESULT!", hr);
        return 0;
    }
    
    while (pParent->IsActive() &&
           (WAIT_OBJECT_0 == WaitForSingleObject(pParent->GetSensorEventHandle(), INFINITE)))
    {    
        hr = S_OK;

        hr = pThis->EnterProcessing(PROCESSING_ISENSOREVENT);

        if (S_OK == hr)
        {
            // Initialize the event parameters
            spEventParams->Clear();

            // Populate the event type
            hr = spEventParams->SetGuidValue(SENSOR_EVENT_PARAMETER_EVENT_ID, SENSOR_EVENT_DATA_UPDATED);
        }
        // Loop through every sensor and post an event for each one if needed
        if (S_OK == hr)
        {
            for(DWORD i = 0; i < pThis->m_pSensorList.GetCount(); i++)
            {
                CSensor *pSensor;
                CComBSTR objectId;
                
                pSensor = pThis->m_pSensorList.GetAt(pThis->m_pSensorList.FindIndex(i));
                
                // Check if this Sensor has valid data to post
                if( TRUE == pSensor->HasValidDataEvent() )
                {
                    // Update sensor state to ready if needed
                    bool fStateChanged = FALSE;
                    hr = pParent->SetState(pSensor, SENSOR_STATE_READY, &fStateChanged);
                    
                    // Get the All the Data Field values
                    // Populate the event parameters
                    if (SUCCEEDED(hr)) {
                        hr = pSensor->GetAllDataFieldValues(spEventParams);
                    }

                    if( SUCCEEDED(hr) )
                    {                            
                        objectId = pSensor->GetSensorObjectID();
                        pParent->PostDataEvent(objectId, spEventParams);
                        pSensor->CheckLongReportIntervalTimer();
                    }
                }                 
            }
        }   

        pThis->ExitProcessing(PROCESSING_ISENSOREVENT);
    }
    
    CoUninitialize();

    return 0;
}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::SetState
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::SetState(_In_ LPVOID pvData, _In_ SensorState newState, _Out_ bool* pfStateChanged)
{

    HRESULT hr = S_OK;

    *pfStateChanged = FALSE;

    CSensor* pSensor = static_cast<CSensor*>(pvData);

    if (nullptr != pSensor)
    {
        CComBSTR objectId = NULL;
        objectId = pSensor->GetSensorObjectID();

        if (NULL != objectId)
        {
            SensorState currentState;
            DWORD dwValue = 0;

            PROPVARIANT var;
            PropVariantInit(&var);

            hr = pSensor->GetProperty(SENSOR_PROPERTY_STATE, &var);

            if (SUCCEEDED(hr))
            {
                PropVariantToUInt32(var, &dwValue);
                currentState = (SensorState)dwValue;

                if (currentState != newState)
                {
                    PropVariantClear(&var);
                    InitPropVariantFromUInt32(newState, &var);

                    hr = pSensor->SetProperty(SENSOR_PROPERTY_STATE, &var, nullptr);

                    if (SUCCEEDED(hr) && (nullptr != m_spClassExtension))
                    {
                        hr = m_spClassExtension->PostStateChange(objectId, newState);

                        if (SUCCEEDED(hr))
                        {
                            switch(newState)
                            {
                            case SENSOR_STATE_READY:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = READY", pSensor->m_SensorName);
                                break;
                            case SENSOR_STATE_NOT_AVAILABLE:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = NOT_AVAILABLE", pSensor->m_SensorName);
                                break;
                            case SENSOR_STATE_NO_DATA:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = NO_DATA", pSensor->m_SensorName);
                                break;
                            case SENSOR_STATE_INITIALIZING:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = INITIALIZING", pSensor->m_SensorName);
                                break;
                            case SENSOR_STATE_ACCESS_DENIED:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = ACCESS_DENIED", pSensor->m_SensorName);
                                break;
                            case SENSOR_STATE_ERROR:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = ERROR", pSensor->m_SensorName);
                                break;
                            default:
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s state, State = UNKNOWN", pSensor->m_SensorName);
                                break;
                            }

                            if (SENSOR_STATE_READY != newState)
                            {
                                DWORD cDatafields = 0;

                                hr = pSensor->m_spSupportedSensorDataFields->GetCount(&cDatafields);

                                if (SUCCEEDED(hr))
                                {
                                    PROPERTYKEY pkDfKey = {0};
                                    PROPVARIANT value;

                                    Trace(TRACE_LEVEL_INFORMATION, "Setting %s datafield values = VT_EMPTY", pSensor->m_SensorName);
                                    for (DWORD dwIdx = 0; dwIdx < cDatafields; dwIdx++)
                                    {
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = pSensor->m_spSupportedSensorDataFields->GetAt(dwIdx, &pkDfKey);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            PropVariantInit( &value );
                                            value.vt = VT_EMPTY;

                                            pSensor->m_spSensorDataFieldValues->SetValue(pkDfKey, &value);

                                            PropVariantClear( &value );
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            PropVariantClear(&var);
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor ObjectID for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
        }

    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "pSensor == nullptr, hr = %!HRESULT!", hr);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::PostDataEvent
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::PostDataEvent(_In_ LPWSTR objectId, IPortableDeviceValues *pValues)
{

    HRESULT hr = S_OK;

    CComPtr<IPortableDeviceValuesCollection> spEventCollection;

    if(spEventCollection == NULL)
    {
        //*--  CoCreate a collection to store the sensor object identifiers.
        hr = CoCreateInstance( CLSID_PortableDeviceValuesCollection,
                                NULL,
                                CLSCTX_INPROC_SERVER,
                                IID_PPV_ARGS(&spEventCollection));
    }

    if( SUCCEEDED(hr) )
    {
        hr = spEventCollection->Add( pValues );
        if( SUCCEEDED(hr) && (m_spClassExtension != NULL) )
        {
            hr = m_spClassExtension->PostEvent( objectId, spEventCollection );            
        }
    }


    return hr;
}

/////////////////////////////////////////////////////////////////////////////////
//
// CSensorManager::PostStateChange
//
//
//
//
//////////////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::PostStateChange(_In_ LPWSTR objectId, _In_ SensorState newState)
{
    Trace(TRACE_LEVEL_VERBOSE, "%!FUNC! Entry");

    HRESULT hr = E_UNEXPECTED;

    if (NULL != m_spClassExtension)
    {
        hr = m_spClassExtension->PostStateChange( objectId, newState );
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::OnCleanupFile
//
// This method is called when the file handle to the device is closed
//
// Parameters:
//      pWdfFile - pointer to a file object
//
/////////////////////////////////////////////////////////////////////////
void CSensorManager::CleanupFile(
    _In_ IWDFFile* pWdfFile
    )
{
    if (NULL != m_spClassExtension)
    {
        m_spClassExtension->CleanupFile(pWdfFile);
    }

    return;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::ProcessIoControl
//
// This method is called to process a Device IO Control
//
// Parameters:
//      pRequest - [in] pointer to the request
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::ProcessIoControl(
    _In_ IWDFIoRequest*   pRequest
    )
{
    Trace(TRACE_LEVEL_VERBOSE, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    if (NULL != m_spClassExtension)
    {
        hr = m_spClassExtension->ProcessIoControl(pRequest);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorManager::ProcessIoControlRadioManagement
//
// This method is called to process a Device IO Control for Radio
// Management.
//
// Parameters:
//      pRequest - [in] pointer to the request
//      ControlCode - [in] the control code to process
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorManager::ProcessIoControlRadioManagement(
    _In_ IWDFIoRequest*     pRequest,
    _In_ ULONG              ControlCode
    )
{
    Trace(TRACE_LEVEL_VERBOSE, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    if (nullptr != m_pSensorDDI)
    {
        switch (ControlCode)
        {
        case IOCTL_GPS_RADIO_MANAGEMENT_GET_RADIO_STATE:
            hr = m_pSensorDDI->OnGetRadioState(pRequest, false);
            break;
        case IOCTL_GPS_RADIO_MANAGEMENT_GET_PREVIOUS_RADIO_STATE:
            hr = m_pSensorDDI->OnGetRadioState(pRequest, true);
            break;
        case IOCTL_GPS_RADIO_MANAGEMENT_SET_RADIO_STATE:
            hr = m_pSensorDDI->OnSetRadioState(pRequest, false);
            break;
        case IOCTL_GPS_RADIO_MANAGEMENT_SET_PREVIOUS_RADIO_STATE:
            hr = m_pSensorDDI->OnSetRadioState(pRequest, true);
            break;
        }
    }

    return hr;
}

VOID CSensorManager::DeActivate()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    m_fThreadActive = FALSE; 
    SetEvent(m_hSensorEvent);
    return;
}

VOID CSensorManager::Activate()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    m_fThreadActive = TRUE; 

    return;
}

inline HRESULT CSensorManager::EnterProcessing(DWORD64 dwControlFlag)
{
    return m_pDevice->EnterProcessing(dwControlFlag);
}

inline void CSensorManager::ExitProcessing(DWORD64 dwControlFlag)
{
    m_pDevice->ExitProcessing(dwControlFlag);
}

