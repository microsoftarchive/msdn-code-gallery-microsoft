/*++
 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

    SensorDdi.cpp

Abstract:

    This module implements the ISensorDriver interface which is used
    by the Sensor Class Extension.
--*/


#include "Internal.h"
#include "Adxl345.h"
#include "ClientManager.h"
#include "ReportManager.h"

#include "SensorDdi.h"
#include "SensorDdi.tmh"

//
// Supported accelerometer properties, data fields,
// and events
//

const PROPERTYKEY g_SupportedAccelerometerProperties[] =
{
    WPD_OBJECT_ID,
    SENSOR_PROPERTY_TYPE,
    SENSOR_PROPERTY_PERSISTENT_UNIQUE_ID,
    SENSOR_PROPERTY_MANUFACTURER, 
    SENSOR_PROPERTY_MODEL,
    SENSOR_PROPERTY_SERIAL_NUMBER,
    SENSOR_PROPERTY_FRIENDLY_NAME,
    SENSOR_PROPERTY_DESCRIPTION, 
    SENSOR_PROPERTY_CONNECTION_TYPE,
    SENSOR_PROPERTY_RANGE_MINIMUM,
    SENSOR_PROPERTY_RANGE_MAXIMUM,
    SENSOR_PROPERTY_RESOLUTION,
    SENSOR_PROPERTY_STATE,
    SENSOR_PROPERTY_MIN_REPORT_INTERVAL,
    WPD_FUNCTIONAL_OBJECT_CATEGORY,
};

const PROPERTYKEY g_SupportedPerDataFieldProperties[] =
{
    SENSOR_PROPERTY_RANGE_MINIMUM,
    SENSOR_PROPERTY_RANGE_MAXIMUM,
    SENSOR_PROPERTY_RESOLUTION,
};

const PROPERTYKEY g_SettableAccelerometerProperties[] =
{
    SENSOR_PROPERTY_CHANGE_SENSITIVITY,
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,
};

const PROPERTYKEY g_SupportedAccelerometerDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,
    SENSOR_DATA_TYPE_ACCELERATION_X_G,
    SENSOR_DATA_TYPE_ACCELERATION_Y_G,
    SENSOR_DATA_TYPE_ACCELERATION_Z_G,
};

const PROPERTYKEY g_SupportedAccelerometerEvents[] =
{
    SENSOR_EVENT_DATA_UPDATED, 0,
    SENSOR_EVENT_STATE_CHANGED, 0,
};


/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::CSensorDdi()
//
//  Constructor
//
/////////////////////////////////////////////////////////////////////////
CSensorDdi::CSensorDdi() :
    m_spSupportedSensorProperties(nullptr),
    m_spSupportedSensorDataFields(nullptr),
    m_spSensorPropertyValues(nullptr),
    m_spSensorDataFieldValues(nullptr),
    m_spClassExtension(nullptr),
    m_spSensorDevice(nullptr),
    m_spWdfDevice2(nullptr),
    m_pAccelerometerDevice(nullptr),
    m_pClientManager(nullptr),
    m_pReportManager(nullptr),
    m_fStateChanged(FALSE),
    m_fSensorInitialized(FALSE),
    m_DataUpdateMode(DataUpdateModeOff)
{

}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::~CSensorDdi()
//
//  Destructor
//
/////////////////////////////////////////////////////////////////////////
CSensorDdi::~CSensorDdi()
{
    // Release object references
    SAFE_RELEASE(m_pAccelerometerDevice);
    SAFE_RELEASE(m_pClientManager);
    SAFE_RELEASE(m_pReportManager);
    
    {
        // Synchronize access to the property cache
        CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CacheCriticalSection);

        // Clear the cache
        if (m_spSupportedSensorProperties != nullptr)
        {
            m_spSupportedSensorProperties->Clear();
            m_spSupportedSensorProperties = nullptr;
        }

        if (m_spSensorPropertyValues != nullptr)
        {
            m_spSensorPropertyValues->Clear();
            m_spSensorPropertyValues = nullptr;
        }

        if (m_spSupportedSensorDataFields != nullptr)
        {
            m_spSupportedSensorDataFields->Clear();
            m_spSupportedSensorDataFields = nullptr;
        }

        if (m_spSensorDataFieldValues != nullptr)
        {
            m_spSensorDataFieldValues->Clear();
            m_spSensorDataFieldValues = nullptr;
        }
    }

    m_fSensorInitialized = FALSE;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::Initialize()
//
//  Initialize function that will set up the sensor device and the sensor
//  driver interface.
//
//  Parameters:
//      pWdfDevice - pointer to a device object
//      pWdfResourcesRaw - pointer the raw resource list
//      pWdfResourcesTranslated - pointer to the translated resource list
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::Initialize(
    _In_ IWDFDevice* pWdfDevice,
    _In_ IWDFCmResourceList * pWdfResourcesRaw,
    _In_ IWDFCmResourceList * pWdfResourcesTranslated
    )
{
    FuncEntry();

    // Check if we are already initialized
    HRESULT hr = S_OK;

    if (m_fSensorInitialized == FALSE)
    {
        // Initialize the sensor device object
        hr = InitializeSensorDevice(
            pWdfDevice,
            pWdfResourcesRaw,
            pWdfResourcesTranslated);

        if (SUCCEEDED(hr))
        {
            // Initialize the SLP sensor driver interface
            hr = InitializeSensorDriverInterface(pWdfDevice);
        }

        if (SUCCEEDED(hr))
        {
            // Create the client manager
            hr = CComObject<CClientManager>::CreateInstance(&m_pClientManager);
            if ((SUCCEEDED(hr)) && (m_pClientManager != nullptr))
            {
                m_pClientManager->AddRef();

                // Initialize the client manager
                hr = m_pClientManager->Initialize();
            }
        }

        if (SUCCEEDED(hr))
        {
            // Create the report manager
            hr = CComObject<CReportManager>::CreateInstance(&m_pReportManager);
            if ((SUCCEEDED(hr)) && (m_pReportManager != nullptr))
            {
                m_pReportManager->AddRef();

                // Initialize the report manager with the
                // default report interval
                m_pReportManager->Initialize(
                    this,
                    DEFAULT_ACCELEROMETER_CURRENT_REPORT_INTERVAL);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = pWdfDevice->QueryInterface(IID_PPV_ARGS(&m_spWdfDevice2));

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to query IWDFDevice2 interface from IWDFDevice %p, "
                    "%!HRESULT!", 
                    pWdfDevice,
                    hr);
            }
        }

        if (SUCCEEDED(hr))
        {
            m_fSensorInitialized = TRUE;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::InitializeSensorDevice()
//
//  This method creates the sensor device object and initializes
//  the callback interface.
//
//  Parameters:
//      pWdfDevice - pointer to a device object
//      pWdfResourcesRaw - pointer the raw resource list
//      pWdfResourcesTranslated - pointer to the translated resource list
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::InitializeSensorDevice(
    _In_ IWDFDevice* pWdfDevice,
    _In_ IWDFCmResourceList * pWdfResourcesRaw,
    _In_ IWDFCmResourceList * pWdfResourcesTranslated
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    // Create the sensor device object
    hr = CComObject<CAccelerometerDevice>::CreateInstance(
        &m_pAccelerometerDevice); 
        
    if (SUCCEEDED(hr))
    {
        m_pAccelerometerDevice->AddRef();

        // Save the ISensorDevice pointer
        hr = m_pAccelerometerDevice->QueryInterface(
            IID_PPV_ARGS(&m_spSensorDevice));
    }    

    // TODO: CoCreateInstance rather than calling
    //       CreateInstance on the class and querying
    //       the required interface.

    //// Create the sensor device
    //hr = CoCreateInstance(
    //    __uuidof(AccelerometerDevice), // CLSID_AccelerometerDevice
    //    nullptr,
    //    CLSCTX_INPROC_SERVER,
    //    IID_PPV_ARGS(&m_spSensorDevice));

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to create the sensor device, %!HRESULT!", 
            hr);
    }
        
    if (SUCCEEDED(hr))
    {
        // Get the ISensorDeviceCallback pointer
        CComPtr<ISensorDeviceCallback> spSensorDeviceCallback;
        hr = QueryInterface(
            IID_PPV_ARGS(&spSensorDeviceCallback));

        if (SUCCEEDED(hr))
        {
            // Initialize the sensor device object
            hr = m_spSensorDevice->Initialize(
                pWdfDevice,
                pWdfResourcesRaw, 
                pWdfResourcesTranslated,
                spSensorDeviceCallback);
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::InitializeSensorDriverInterface()
//
//  Initialize function that will set up all the propertykeys
//
//  Parameters:
//      pWdfDevice - pointer to a device object
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::InitializeSensorDriverInterface(
    _In_ IWDFDevice* pWdfDevice
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if (m_spSupportedSensorProperties == nullptr)
    {
        // Create a new PortableDeviceKeyCollection to store the supported 
        // property KEYS
        hr = CoCreateInstance(
            CLSID_PortableDeviceKeyCollection,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&m_spSupportedSensorProperties));
    }
        
    if (SUCCEEDED(hr))
    {
        if (m_spSensorPropertyValues == nullptr)
        {
            // Create a new PortableDeviceValues to store the property VALUES
            hr = CoCreateInstance(
                CLSID_PortableDeviceValues,
                nullptr,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(&m_spSensorPropertyValues));
        }
    }
        
    if (SUCCEEDED(hr))
    {        
        if (m_spSupportedSensorDataFields == nullptr)
        {
            // Create a new PortableDeviceValues to store the supported 
            // datafield KEYS
            hr = CoCreateInstance(
                CLSID_PortableDeviceKeyCollection,
                nullptr,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(&m_spSupportedSensorDataFields));
        }
    }
        
    if (SUCCEEDED(hr))
    {        
        if (m_spSensorDataFieldValues == nullptr)
        {
            // Create a new PortableDeviceValues to store the datafield VALUES
            hr = CoCreateInstance(
                CLSID_PortableDeviceValues,
                nullptr,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(&m_spSensorDataFieldValues));
        }
    }

    if (SUCCEEDED(hr))
    {
        // Add supported property keys and initialize values
        hr = AddPropertyKeys();
    }

    if (SUCCEEDED(hr))
    {
        // Add supported data field keys and initialize values
        hr = AddDataFieldKeys();
    }

    if (SUCCEEDED(hr))
    {
        // Set the unique ID of the sensor
        hr = SetUniqueID(pWdfDevice);
    }

    if (SUCCEEDED(hr))
    {
        // Set the default property values
        hr = SetDefaultPropertyValues();
    }

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to initialize the sensor driver interface, %!HRESULT!", 
            hr);
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::Uninitialize()
//
//  Uninitialize function that will tear down the sensor device and 
//  report manager.
//
//  Parameters:
//
//  Return Values:
//      none
//
/////////////////////////////////////////////////////////////////////////
VOID CSensorDdi::Uninitialize()
{
    FuncEntry();

    // Uninitialize the report manager
    if (m_pReportManager != nullptr)
    {
        m_pReportManager->Uninitialize();
    }

    // The sensor device has already been stopped
    // in D0Exit.  No further uninitialization
    // is necessary.

    FuncExit();
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::SetSensorClassExtension
//
//  Custom method to receive a pointer to the sensor class extension. This 
//  pointer is provided from CMyDevice::OnPrepareHardware after the
//  sensor class extension is created.
//
//  Parameters:
//      pClassExtension - interface pointer to the sensor class extension
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::SetSensorClassExtension(
    _In_ ISensorClassExtension* pClassExtension
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if (pClassExtension == nullptr)
    {
        hr = E_POINTER;
    }
    else
    {
        // Save the class extension interface pointer
        m_spClassExtension = pClassExtension;
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::Start()
//
//  This method configures the sensor device and places it in standby mode.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::Start()
{
    FuncEntry();

    // Check if we are already initialized
    HRESULT hr = m_fSensorInitialized ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr))
    {
        // Configure the sensor device. The sensor state should
        // already be SENSOR_STATE_NO_DATA, which will be updated when 
        // the first data bytes are received.
        hr = m_spSensorDevice->ConfigureHardware();

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "The hardware could not be configured, %!HRESULT!", 
                hr);
        }
    }

    if (SUCCEEDED(hr))
    {
        if (m_pClientManager->GetClientCount() > 0)
        {
            // Restore previous configuration. This function
            // will apply the current report interval and change
            // sensitivity and set the reporting mode based on  
            // client connectivity and subscription.
            hr = ApplyUpdatedProperties();

            if (SUCCEEDED(hr))
            {
                // Poll for initial data
                hr = PollForData();
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::Stop()
//
//  This method disables the sensor device.
//
//  Parameters:
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::Stop()
{
    FuncEntry();

    HRESULT hr = S_OK;

    // Indicate that the sensor no longer has valid data
    hr = SetState(SENSOR_STATE_NO_DATA);

    if (SUCCEEDED(hr))
    {
        hr = SetDataUpdateMode(DataUpdateModeOff);
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::SetDataUpdateMode()
//
//  This method sets the data update mode.
//
//  Parameters:
//      Mode - new data update mode
//
//  Return Values:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::SetDataUpdateMode(
    _In_  DATA_UPDATE_MODE Mode
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    hr = m_spSensorDevice->SetDataUpdateMode(Mode);

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to set data update mode %d, %!HRESULT!",
            Mode,
            hr);
    }

    if (SUCCEEDED(hr))
    {
        m_DataUpdateMode = Mode;
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::GetSensorObjectID
//
//  This method returns the sensor's object ID.
//  
//  Parameters:
//
//  Return Value:
//      Object ID 
//
/////////////////////////////////////////////////////////////////////////
LPWSTR CSensorDdi::GetSensorObjectID()
{
    return (LPWSTR)SENSOR_ACCELEROMETER_ID;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnGetSupportedSensorObjects
//
//  This method is called by Sensor Class Extension during the initialize
//  procedure to get the list of sensor objects and their supported properties.
//  
//  Parameters:
//      ppPortableDeviceValuesCollection - a double IPortableDeviceValuesCollection
//          pointer that receives the set of Sensor property values
//
//  Return Value:
//      status 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnGetSupportedSensorObjects(
        _Out_ IPortableDeviceValuesCollection** ppPortableDeviceValuesCollection
        )
{
    FuncEntry();

    HRESULT hr = S_OK;
    
    // CoCreate a collection to store the sensor object identifiers.
    hr = CoCreateInstance(
        CLSID_PortableDeviceValuesCollection,
        nullptr,
        CLSCTX_INPROC_SERVER,
        IID_PPV_ARGS(ppPortableDeviceValuesCollection));

    CComPtr<IPortableDeviceKeyCollection> spKeys;
    CComPtr<IPortableDeviceValues> spValues;

    if (SUCCEEDED(hr))
    {
        // Get the list of supported property keys
        hr = OnGetSupportedProperties(
            GetSensorObjectID(), 
            &spKeys);
    }

    if (SUCCEEDED(hr))
    {
        CComPtr<IWDFFile> spTemp;

        // Get the property values
        hr = OnGetProperties(
            spTemp, 
            GetSensorObjectID(), 
            spKeys, 
            &spValues);
    }

    if (SUCCEEDED(hr))
    {
        hr = (*ppPortableDeviceValuesCollection)->Add(spValues);
    }

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to get the supported sensor objects, %!HRESULT!", 
            hr);
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnGetSupportedProperties
//
//  This method is called by Sensor Class Extension to get the list of 
//  supported properties for a particular Sensor.
//  
//  Parameters:
//      pwszObjectID - string that represents the object whose supported 
//          property keys are being requested
//      ppKeys - an IPortableDeviceKeyCollection to be populated with 
//          supported PROPERTYKEYs
//
//  Return Value:
//      status 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnGetSupportedProperties(
        _In_  LPWSTR pwszObjectID,
        _Out_ IPortableDeviceKeyCollection** ppKeys
        )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(pwszObjectID);

    HRESULT hr = S_OK;

    // CoCreate a collection to store the supported property keys.
    hr = CoCreateInstance(
        CLSID_PortableDeviceKeyCollection,
        nullptr,
        CLSCTX_INPROC_SERVER,
        IID_PPV_ARGS(ppKeys));

    // Add supported property keys for the specified object to the collection
    if (SUCCEEDED(hr))
    {
        hr = CopyKeys(m_spSupportedSensorProperties, *ppKeys);
    }

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to get the supported sensor properties, %!HRESULT!", 
            hr);
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnGetSupportedDataFields
//
//  This method is called by Sensor Class Extension to get the list of supported data fields
//  for a particular Sensor.
//  
//  Parameters:
//      wszObjectID - string that represents the object whose supported 
//          property keys are being requested
//      ppKeys - An IPortableDeviceKeyCollection to be populated with 
//          supported PROPERTYKEYs
//
// Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnGetSupportedDataFields(
        _In_  LPWSTR wszObjectID,
        _Out_ IPortableDeviceKeyCollection** ppKeys
        )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(wszObjectID);

    HRESULT hr = S_OK;

    // CoCreate a collection to store the supported property keys.
    hr = CoCreateInstance(
        CLSID_PortableDeviceKeyCollection,
        nullptr,
        CLSCTX_INPROC_SERVER,
        IID_PPV_ARGS(ppKeys));

    // Add supported property keys for the specified object to the collection
    if (SUCCEEDED(hr) && ppKeys != nullptr)
    {
        hr = CopyKeys(m_spSupportedSensorDataFields, *ppKeys);
    }

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to get the supported sensor data fields, %!HRESULT!", 
            hr);
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnGetSupportedEvents
//
//  This method is called by Sensor Class Extension to get the list of
//  supported events for a particular Sensor.
//  
//  Parameters:
//      wszObjectID - String that represents the object whose supported 
//          property keys are being requested
//      ppSupportedEvents - A set of GUIDs that represent the supported events
//      pulEventCount - Count of the number of events supported
//
//  Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnGetSupportedEvents(
        _In_  LPWSTR wszObjectID,
        _Out_ GUID **ppSupportedEvents,
        _Out_ ULONG *pulEventCount
        )
{    
    FuncEntry();

    UNREFERENCED_PARAMETER(wszObjectID);

    HRESULT hr = S_OK;
    ULONG count = ARRAY_SIZE(g_SupportedAccelerometerEvents);
    
    // Allocate memory for the list of supported events
    GUID* pBuf = (GUID*)CoTaskMemAlloc(sizeof(GUID) * count);

    if (pBuf != nullptr)
    {
        for (DWORD i = 0; i < count; count++)
        {
            *(pBuf + i) = g_SupportedAccelerometerEvents[i].fmtid;
        }

        *ppSupportedEvents = pBuf;
        *pulEventCount = count;
    }
    else
    {
        hr = E_OUTOFMEMORY;

        *ppSupportedEvents = nullptr;
        *pulEventCount = 0;
    }

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to get the supported sensor events, %!HRESULT!", 
            hr);
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnGetProperties
//
//  This method is called by Sensor Class Extension to get Sensor
//  property values for a particular Sensor.
//  
//  Parameters:
//      appId - pinter to an IWDFFile interface that represents the file 
//          object for the application requesting property values
//      wszObjectID - string that represents the object whose property
//          key values are being requested
//      pKeys - an IPortableDeviceKeyCollection containing the list of 
//          properties being requested
//      ppValues - an IPortableDeviceValues pointer that receives the 
//          requested property values
//
//  Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnGetProperties(
        _In_  IWDFFile* appId,
        _In_  LPWSTR wszObjectID,
        _In_  IPortableDeviceKeyCollection* pKeys,
        _Out_ IPortableDeviceValues** ppValues
        )
{
    FuncEntry();
    
    UNREFERENCED_PARAMETER(appId);
    UNREFERENCED_PARAMETER(wszObjectID);

    HRESULT hr = S_OK;
    DWORD keyCount = 0;
    BOOL fError = FALSE;
    IPortableDeviceValues*  pValues = nullptr;

    if ((pKeys == nullptr) ||
        (ppValues == nullptr))
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {        
        // CoCreate an object to store the property values
        hr = CoCreateInstance(
            CLSID_PortableDeviceValues,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(ppValues));
    }

    if (SUCCEEDED(hr))
    {
        pValues = *ppValues;
        
        if (pValues == nullptr)
        {
            hr = E_INVALIDARG;
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = pKeys->GetCount(&keyCount);
    }

    if (SUCCEEDED(hr))
    {
        // Loop through each key and get the property value
        for (DWORD i = 0; i < keyCount; i++)
        {
            PROPERTYKEY key;
            hr = pKeys->GetAt(i, &key);

            if (SUCCEEDED(hr))
            {
                PROPVARIANT var;
                PropVariantInit(&var);

                HRESULT hrTemp = GetProperty(key, &var);

                if (SUCCEEDED(hrTemp))
                {
                   pValues->SetValue(key, &var);
                }
                else
                {
                    // Failed to find the requested property, 
                    // convey the hr back to the caller
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to get the sensor property value, "
                        "%!HRESULT!", 
                        hrTemp);

                    pValues->SetErrorValue(key, hrTemp);
                    fError = TRUE;     
                }

                if ((var.vt & VT_VECTOR) == 0)
                {
                    // For a VT_VECTOR type, PropVariantClear()
                    // frees all underlying elements. Note pValues
                    // now has a pointer to the vector structure
                    // and is responsible for freeing it.

                    // If var is not a VT_VECTOR, clear it.
                    PropVariantClear(&var);
                }
            }
            else
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to get property key, %!HRESULT!", 
                    hr);

                break;
            }
        }

        if (fError)
        {
            hr = S_FALSE;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnGetDataFields
//
//  This method is called by Sensor Class Extension to get Sensor data fields
//  for a particular Sensor.
//  
//  Parameters:
//      appId - pinter to an IWDFFile interface that represents the file 
//          object for the application requesting data field values
//      wszObjectID - string that represents the object whose data field 
//          values are being requested
//      pKeys - an IPortableDeviceKeyCollection containing the list of 
//          data fields being requested
//      ppValues - an IPortableDeviceValues pointer that receives the 
//          requested data field values
//
//  Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnGetDataFields(
        _In_  IWDFFile* appId,
        _In_  LPWSTR wszObjectID,
        _In_  IPortableDeviceKeyCollection* pKeys,
        _Out_ IPortableDeviceValues** ppValues
        )
{    
    FuncEntry();

    UNREFERENCED_PARAMETER(appId);
    UNREFERENCED_PARAMETER(wszObjectID);

    HRESULT hr = S_OK;
    DWORD keyCount = 0;
    BOOL fError = FALSE;
    IPortableDeviceValues*  pValues = nullptr;

    if ((pKeys == nullptr) ||
        (ppValues == nullptr))
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {        
        // CoCreate an object to store the data field values
        hr = CoCreateInstance(
            CLSID_PortableDeviceValues,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(ppValues));
    }

    if (SUCCEEDED(hr))
    {
        pValues = *ppValues;
        
        if (pValues == nullptr)
        {
            hr = E_INVALIDARG;
        }
    }

    if (SUCCEEDED(hr))
    {
        DWORD value;
        SensorState currentState = SENSOR_STATE_NOT_AVAILABLE;
        PROPVARIANT var;
        PropVariantInit(&var);

        // Get current state
        hr = GetProperty(SENSOR_PROPERTY_STATE, &var);

        if (SUCCEEDED(hr)) 
        {
            PropVariantToUInt32(var, &value);
            currentState = (SensorState)value;
        }

        if ((m_DataUpdateMode == DataUpdateModePolling) ||
            (currentState != SENSOR_STATE_READY))
        {
            hr = PollForData();
        }

        PropVariantClear(&var);
    }

    if (SUCCEEDED(hr))
    {
        hr = pKeys->GetCount(&keyCount);
    }

    if (SUCCEEDED(hr))
    {
        // Loop through each key and get the data field value
        for (DWORD i = 0; i < keyCount; i++)
        {
            PROPERTYKEY key;
            hr = pKeys->GetAt(i, &key);

            if (SUCCEEDED(hr))
            {
                PROPVARIANT var;
                PropVariantInit(&var);

                HRESULT hrTemp = GetDataField(key, &var);

                if (SUCCEEDED(hrTemp))
                {
                   pValues->SetValue(key, &var);
                }
                else
                {
                    // Failed to find the requested property, 
                    // convey the hr back to the caller
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to get the sensor data field value, "
                        "%!HRESULT!", 
                        hrTemp);

                    pValues->SetErrorValue(key, hrTemp);
                    fError = TRUE;
                }

                PropVariantClear(&var);
            }
            else
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to get property key, %!HRESULT!", 
                    hr);

                break;
            }
        }

        if (fError)
        {
            hr = S_FALSE;
        }
    }

    FuncExit();

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnSetProperties
//
//  This method is called by Sensor Class Extension to set Sensor properties
//  for a particular Sensor.
//  
//  Parameters:
//      appId - pinter to an IWDFFile interface that represents the file 
//          object for the application specifying property values
//      wszObjectID - string that represents the object whose property
//          key values are being specified
//      pValues - an IPortableDeviceValues containing the list of 
//          properties being specified
//      ppResults - an IPortableDeviceValues pointer that receives the 
//          list of specified property values if successful or an error code
//
//  Return Value:
//      status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnSetProperties(
        _In_ IWDFFile* appId,
        _In_ LPWSTR ObjectID,
        _In_ IPortableDeviceValues* pValues,
        _Out_ IPortableDeviceValues** ppResults
        )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(ObjectID);

    HRESULT hr = S_OK;

    if ((appId == nullptr) ||
        (pValues == nullptr) ||
        (ppResults == nullptr))
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        DWORD count = 0;
        BOOL fError = FALSE;
        
        // CoCreate an object to store the property value results
        hr = CoCreateInstance(
            CLSID_PortableDeviceValues,
            NULL,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(ppResults));

        if (SUCCEEDED(hr))
        {
            hr = pValues->GetCount(&count);
        }

        if (SUCCEEDED(hr))
        {
            // Loop through each key and get the 
            // property key and value
            for (DWORD i = 0; i < count; i++)
            {
                PROPERTYKEY key = WPD_PROPERTY_NULL;
                PROPVARIANT var;

                PropVariantInit( &var );

                hr = pValues->GetAt(i, &key, &var);

                if (SUCCEEDED(hr))
                {
                    HRESULT hrTemp = S_OK;

                    PROPVARIANT varResult;
                    PropVariantInit(&varResult);

                    // Check if this is one of the test properties
                    if (IsTestProperty(key))
                    {
                        hrTemp = m_spSensorDevice->SetTestProperty(
                            key, 
                            &var);

                        // Test does not care about the property
                        // result.  No need to set varResult.
                    }
                    // Else assume this is one of the settable
                    // properties, which are mainted by the client
                    // manager
                    else
                    {
                        hrTemp = m_pClientManager->SetDesiredProperty(
                            appId,
                            key, 
                            &var,
                            &varResult);
                    }

                    if (SUCCEEDED(hrTemp))
                    {
                        (*ppResults)->SetValue(key, &varResult);

                        // Check if one of the change sensitivity
                        // values failed.  Convey this back to the
                        // client
                        if (hrTemp != S_OK)
                        {
                            fError = TRUE;
                        }
                    }
                    else
                    {
                        Trace(
                            TRACE_LEVEL_ERROR,
                            "Failed to set property value, "
                            "%!HRESULT!",
                            hrTemp);
                                    
                        fError = TRUE;
                        (*ppResults)->SetErrorValue(key, hrTemp);
                    }

                    PropVariantClear(&varResult);
                }

                PropVariantClear(&var);
                
                if (FAILED(hr))
                {
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to get property key and value, %!HRESULT!", 
                        hr);

                    break;
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            // Successfully setting a property may have
            // caused the mimimum properties to change.
            // Be safe and reapply the updated values.
            hr = ApplyUpdatedProperties();
        }

        if (SUCCEEDED(hr) && (fError == TRUE))
        {
            hr = S_FALSE;
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnClientConnect
//
//  This method is called by Sensor Class Extension when a client app connects
//  to a Sensor
//  
//  Parameters:
//      pClientFile - file object for the application requesting the conenction
//      pwszObjectID - the ID for the sensor to which the client application 
//          is connecting
//
//  Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnClientConnect(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(pwszObjectID);

    HRESULT hr = S_OK;

    if (pClientFile == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        ULONG clientCount = 0;

        {
            // Synchronize access to the client manager so that after
            // the client connects it can query the new client
            // count atomically
            CComCritSecLock<CComAutoCriticalSection> 
                scopeLock(m_ClientCriticalSection);

            // Inform the client manager of the new client
            hr  = m_pClientManager->Connect(pClientFile);

            if (SUCCEEDED(hr))
            {
                // Save the client count
                clientCount = m_pClientManager->GetClientCount();
            }
        }

        if (SUCCEEDED(hr))
        {
            // The mimimum properties may have changed, reapply
            hr = ApplyUpdatedProperties();
        }

        if (SUCCEEDED(hr))
        {
            // Stop idle detection if this is the first client
            if (clientCount == 1)
            {
                Trace(
                    TRACE_LEVEL_INFORMATION,
                    "First client, stop idle detection");

                // When using a power managed queue we are guaranteed
                // to be in D0 during OnClientConnect, so there is no need
                // to block on this call. It's safe to touch hardware at 
                // this point. There is potential, however, to temporarily 
                // transition from D0->Dx->D0 after this call returns, so be 
                // sure to reconfigure the hardware in D0Enty.
                hr = m_spWdfDevice2->StopIdle(false);

                if (FAILED(hr))
                {
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to stop idle detection for IWDFDevice2 %p, "
                        "%!HRESULT!", 
                        m_spWdfDevice2,
                        hr);
                }

                if (SUCCEEDED(hr))
                {
                    hr = SetDataUpdateMode(DataUpdateModePolling);
                }

                if (SUCCEEDED(hr))
                {
                    // Poll for initial data
                    hr = PollForData();
                }
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnClientDisconnect
//
//  This method is called by Sensor Class Extension when a client app 
//  disconnects from a Sensor
//  
//  Parameters:
//      pClientFile - file object for the application requesting 
//          the disconnection
//      pwszObjectID - the ID for the sensor from which the client 
//          application is disconnecting
//
//  Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnClientDisconnect(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(pwszObjectID);

    HRESULT hr = S_OK;

    if (pClientFile == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        ULONG clientCount = 0;

        {
            // Synchronize access to the client manager so that after
            // the client disconnects it can query the new client
            // count atomically
            CComCritSecLock<CComAutoCriticalSection> 
                scopeLock(m_ClientCriticalSection);

            // Inform the client manager that the client
            // is leaving
            hr  = m_pClientManager->Disconnect(pClientFile);

            if (SUCCEEDED(hr))
            {
                // Save the client count
                clientCount = m_pClientManager->GetClientCount();
            }
        }

        if (SUCCEEDED(hr))
        {
            // The mimimum properties may have changed, reapply
            hr = ApplyUpdatedProperties();
        }

        if (SUCCEEDED(hr))
        {
            // Resume idle detection if there are no more clients
            if (clientCount == 0)
            {
                Trace(
                    TRACE_LEVEL_INFORMATION,
                    "No clients, resume idle detection");

                m_spWdfDevice2->ResumeIdle();

                if (SUCCEEDED(hr))
                {
                    hr = SetDataUpdateMode(DataUpdateModeOff);
                }
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnClientSubscribeToEvents
//
//  This method is called by Sensor Class Extension when a client subscribes to
//  events by calling SetEventSink
//  
//  Parameters:
//      pClientFile - file object for the application subscribing to events
//      pwszObjectID - the ID for the sensor from which the client 
//          application is subscribing to events
//
// Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnClientSubscribeToEvents(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(pwszObjectID);

    HRESULT hr = S_OK;

    if (pClientFile == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        // Let the client manager know that the client
        // has subscribed
        hr = m_pClientManager->Subscribe(pClientFile);

        if (SUCCEEDED(hr))
        {
            // The mimimum properties may have changed, reapply
            hr = ApplyUpdatedProperties();
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnClientUnsubscribeFromEvents
//
//  This method is called by Sensor Class Extension when a client unsubscribes
//  from events by calling SetEventSink(nullptr)
//  
//  Parameters:
//      pClientFile - file object for the application unsubscribing from events
//      pwszObjectID - the ID for the sensor from which the client 
//          application is unsubscribing from events
//
// Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnClientUnsubscribeFromEvents(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(pwszObjectID);

    HRESULT hr = S_OK;

    if (pClientFile == nullptr)
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        // Let the client manager know that the client
        // has subscribed
        hr = m_pClientManager->Unsubscribe(pClientFile);

        if (SUCCEEDED(hr))
        {
            // The mimimum properties may have changed, reapply
            hr = ApplyUpdatedProperties();
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnProcessWpdMessage
//
//  This method handles Windows Portable Device (WPD) commands that the 
//  ISensorClassExtension::ProcessIoControl method does not handle internally
//  
//  Parameters:
//      pPortableDeviceValuesParamsUnknown - the object that is associated 
//          with this IUnknown interface contains the parameters for the 
//          WPD command
//      pPortableDeviceValuesResultsUnknown - the object that is associated 
//          with this IUnknown interface stores the results for the WPD command
//
//  Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnProcessWpdMessage(
        _In_ IUnknown* pPortableDeviceValuesParamsUnknown,
        _In_ IUnknown* pPortableDeviceValuesResultsUnknown
        )
{
    FuncEntry();

    UNREFERENCED_PARAMETER(pPortableDeviceValuesParamsUnknown);
    UNREFERENCED_PARAMETER(pPortableDeviceValuesResultsUnknown);

    FuncExit();

    return E_NOTIMPL;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::AddPropertyKeys
//
//  This methods populates the supported properties list and initializes
//  their values to VT_EMPTY.
//
//  Parameters: 
//
//  Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::AddPropertyKeys()
{
    FuncEntry();

    // Synchronize access to the property cache
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CacheCriticalSection);

    HRESULT hr = S_OK;

    if (m_spSupportedSensorProperties == nullptr)
    {
        hr = E_POINTER;
    }

    if (SUCCEEDED(hr))
    {
        for (DWORD i = 0; i < ARRAY_SIZE(g_SupportedAccelerometerProperties); i++)
        {
            PROPVARIANT var;
            PropVariantInit(&var);

            // Add the PROPERTYKEY to the list of supported properties
            if (SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorProperties->Add(
                    g_SupportedAccelerometerProperties[i]);
            }

            // Initialize the PROPERTYKEY value to VT_EMPTY
            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetValue(
                    g_SupportedAccelerometerProperties[i], 
                    &var);
            }

            PropVariantClear(&var);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to add the sensor property key, %!HRESULT!,",
                    hr);
            }
        }

        for (DWORD i = 0; i < ARRAY_SIZE(g_SettableAccelerometerProperties); i++)
        {
            // Add the PROPERTYKEY to the list of supported properties.
            // We do not need to add the settable property values to the cache
            // because these are maintained by the client manager
            if (SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorProperties->Add(
                    g_SettableAccelerometerProperties[i]);

                if (FAILED(hr))
                {
                    Trace(
                        TRACE_LEVEL_ERROR,
                        "Failed to add the sensor property key, %!HRESULT!,",
                        hr);
                }
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::AddDataFieldKeys
//
//  This methods populates the supported data fields list and initializes
//  their values to VT_EMPTY.
//
//  Parameters: 
//
//  Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::AddDataFieldKeys()
{
    FuncEntry();

    // Synchronize access to the property cache
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CacheCriticalSection);

    HRESULT hr = S_OK;

    if (m_spSupportedSensorDataFields == nullptr)
    {
        hr = E_POINTER;
    }

    if (SUCCEEDED(hr))
    {
        for (DWORD i = 0; i < ARRAY_SIZE(g_SupportedAccelerometerDataFields); i++)
        {
            PROPVARIANT var;
            PropVariantInit(&var);

            // Add the PROPERTYKEY to the list of supported properties
            if (SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorDataFields->Add(
                    g_SupportedAccelerometerDataFields[i]);
            }

            // Initialize the PROPERTYKEY value to VT_EMPTY
            if (SUCCEEDED(hr))
            {
                hr = m_spSensorDataFieldValues->SetValue(
                    g_SupportedAccelerometerDataFields[i], 
                    &var);
            }

            PropVariantClear(&var);

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to add the sensor data field key, %!HRESULT!,",
                    hr);
            }
        }
    }

    FuncExit();

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::SetDefaultPropertyValues
//
//  This methods sets the property values to their defaults.
//
//  Parameters: 
//
//  Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::SetDefaultPropertyValues()
{
    FuncEntry();

    // Synchronize access to the property cache
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CacheCriticalSection);

    HRESULT hr = S_OK;

    if (m_spSensorPropertyValues == nullptr)
    {
        hr = E_POINTER;
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(
            WPD_OBJECT_ID, 
            (LPCWSTR)SENSOR_ACCELEROMETER_ID);

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetGuidValue(
                WPD_FUNCTIONAL_OBJECT_CATEGORY, 
                SENSOR_CATEGORY_MOTION);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetGuidValue(
                SENSOR_PROPERTY_TYPE, 
                SENSOR_TYPE_ACCELEROMETER_3D);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetStringValue(
                SENSOR_PROPERTY_MANUFACTURER, 
                (LPCWSTR)SENSOR_ACCELEROMETER_MANUFACTURER);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetStringValue(
                SENSOR_PROPERTY_MODEL, 
                (LPCWSTR)SENSOR_ACCELEROMETER_MODEL);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetStringValue(
                SENSOR_PROPERTY_SERIAL_NUMBER, 
                (LPCWSTR)SENSOR_ACCELEROMETER_SERIAL_NUMBER);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetStringValue(
                SENSOR_PROPERTY_FRIENDLY_NAME, 
                (LPCWSTR)SENSOR_ACCELEROMETER_NAME);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetStringValue(
                SENSOR_PROPERTY_DESCRIPTION, 
                (LPCWSTR)SENSOR_ACCELEROMETER_DESCRIPTION);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(
                SENSOR_PROPERTY_CONNECTION_TYPE, 
                SENSOR_CONNECTION_TYPE_PC_INTEGRATED);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(
                SENSOR_PROPERTY_STATE, SENSOR_STATE_NO_DATA);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(
                SENSOR_PROPERTY_MIN_REPORT_INTERVAL, 
                ACCELEROMETER_MIN_REPORT_INTERVAL);
        }
        
        // The following properties are per data field

        if (SUCCEEDED(hr))
        {
            // Create an IPortableDeviceValues to
            // store the per data field minimum
            // range values
            CComPtr<IPortableDeviceValues> spRangeMinValues;
            hr = CoCreateInstance(
                CLSID_PortableDeviceValues,
                nullptr,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(&spRangeMinValues));

            if (SUCCEEDED(hr))
            {
                PROPVARIANT var;
                PropVariantInit(&var);

                var.vt = VT_R8;
                var.dblVal = ACCELEROMETER_MIN_ACCELERATION_G;
                
                hr = spRangeMinValues->SetValue(
                    SENSOR_DATA_TYPE_ACCELERATION_X_G,
                    &var);

                if (SUCCEEDED(hr))
                {
                    hr = spRangeMinValues->SetValue(
                        SENSOR_DATA_TYPE_ACCELERATION_Y_G,
                        &var);
                }

                if (SUCCEEDED(hr))
                {
                    hr = spRangeMinValues->SetValue(
                        SENSOR_DATA_TYPE_ACCELERATION_Z_G,
                        &var);
                }

                if (SUCCEEDED(hr))
                {
                    // Add to the property cache
                    hr = m_spSensorPropertyValues->
                        SetIPortableDeviceValuesValue(
                            SENSOR_PROPERTY_RANGE_MINIMUM, 
                            spRangeMinValues);
                }

                PropVariantClear(&var);
            }
        }

        if (SUCCEEDED(hr))
        {
            // Create an IPortableDeviceValues to
            // store the per data field maximum
            // range values
            CComPtr<IPortableDeviceValues> spRangeMaxValues;
            hr = CoCreateInstance(
                CLSID_PortableDeviceValues,
                nullptr,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(&spRangeMaxValues));

            if (SUCCEEDED(hr))
            {
                PROPVARIANT var;
                PropVariantInit(&var);

                var.vt = VT_R8;
                var.dblVal = ACCELEROMETER_MAX_ACCELERATION_G;
                
                hr = spRangeMaxValues->SetValue(
                    SENSOR_DATA_TYPE_ACCELERATION_X_G,
                    &var);

                if (SUCCEEDED(hr))
                {
                    hr = spRangeMaxValues->SetValue(
                        SENSOR_DATA_TYPE_ACCELERATION_Y_G,
                        &var);
                }

                if (SUCCEEDED(hr))
                {
                    hr = spRangeMaxValues->SetValue(
                        SENSOR_DATA_TYPE_ACCELERATION_Z_G,
                        &var);
                }

                if (SUCCEEDED(hr))
                {
                    // Add to the property cache
                    hr = m_spSensorPropertyValues->
                        SetIPortableDeviceValuesValue(
                            SENSOR_PROPERTY_RANGE_MAXIMUM, 
                            spRangeMaxValues);
                }

                PropVariantClear(&var);
            }
        }

        if (SUCCEEDED(hr))
        {
            // Create an IPortableDeviceValues to
            // store the per data field resolution values
            CComPtr<IPortableDeviceValues> spResolutionValues;
            hr = CoCreateInstance(
                CLSID_PortableDeviceValues,
                nullptr,
                CLSCTX_INPROC_SERVER,
                IID_PPV_ARGS(&spResolutionValues));

            if (SUCCEEDED(hr))
            {
                PROPVARIANT var;
                PropVariantInit(&var);

                var.vt = VT_R8;
                var.dblVal = ACCELEROMETER_RESOLUTION_ACCELERATION_G;
                
                hr = spResolutionValues->SetValue(
                    SENSOR_DATA_TYPE_ACCELERATION_X_G,
                    &var);

                if (SUCCEEDED(hr))
                {
                    hr = spResolutionValues->SetValue(
                        SENSOR_DATA_TYPE_ACCELERATION_Y_G,
                        &var);
                }

                if (SUCCEEDED(hr))
                {
                    hr = spResolutionValues->SetValue(
                        SENSOR_DATA_TYPE_ACCELERATION_Z_G,
                        &var);
                }

                if (SUCCEEDED(hr))
                {
                    // Add to the property caches
                    hr = m_spSensorPropertyValues->
                        SetIPortableDeviceValuesValue(
                            SENSOR_PROPERTY_RESOLUTION, 
                            spResolutionValues);
                }

                PropVariantClear(&var);
            }
        }

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failure while setting defualt property keys, %!HRESULT!,",
                hr);
        }
    }

    FuncExit();
    
    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::SetUniqueID
//
//  This methods sets the persistent unique ID property.
//
//  Parameters: 
//      pWdfDevice - pointer to a device object
//
//  Return Value:
//      status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::SetUniqueID(_In_ IWDFDevice* pWdfDevice)
{
    FuncEntry();

    // Synchronize access to the property cache
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CacheCriticalSection);

    HRESULT hr = S_OK;
    
    if (pWdfDevice == nullptr)
    {
        hr = E_INVALIDARG;
    }

    CComPtr<IWDFNamedPropertyStore> spPropStore;
    if (SUCCEEDED(hr))
    {
        hr = pWdfDevice->RetrieveDevicePropertyStore(
            nullptr, 
            WdfPropertyStoreCreateIfMissing, 
            &spPropStore, 
            nullptr);
    }

    if (SUCCEEDED(hr))
    {
        GUID idGuid;
        LPCWSTR lpcszKeyName = (LPCWSTR)SENSOR_ACCELEROMETER_ID;
        
        PROPVARIANT var;
        PropVariantInit(&var);
        hr = spPropStore->GetNamedValue(lpcszKeyName, &var);
        if (SUCCEEDED(hr))
        {
            hr = ::CLSIDFromString(var.bstrVal, &idGuid);
        }
        else
        {
            hr = ::CoCreateGuid(&idGuid);
            if (SUCCEEDED(hr))
            {
                LPOLESTR lpszGUID = nullptr;
                hr = ::StringFromCLSID(idGuid, &lpszGUID);
                if (SUCCEEDED(hr))
                {
                    var.vt = VT_LPWSTR;
                    var.pwszVal = lpszGUID;
                    hr = spPropStore->SetNamedValue(lpcszKeyName, &var);
                }
            }
        }

        PropVariantClear(&var);

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetGuidValue(
                SENSOR_PROPERTY_PERSISTENT_UNIQUE_ID, 
                idGuid);
        }
    }

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to set the sensor's unique ID, %!HRESULT!", 
            hr);
    }

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::GetProperty
//
//  Gets the property value for a given property key.
//
//  Parameters:
//      key - the requested property key
//      pVar - location for the value of the requested property key
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::GetProperty(
        _In_  REFPROPERTYKEY key, 
        _Out_ PROPVARIANT* pVar
        )
{
    HRESULT hr = S_OK;

    // Check if this is a test property
    if (IsTestProperty(key))
    {
        hr = m_spSensorDevice->GetTestProperty(key, pVar);
    }
    // Settable properties are managed by the client manager
    else if (IsEqualPropertyKey(key, SENSOR_PROPERTY_CHANGE_SENSITIVITY) ||
        IsEqualPropertyKey(key, SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL))
    {
        hr = m_pClientManager->GetArbitratedProperty(key, pVar);
    }
    // Other property key
    else
    {
        // Synchronize access to the property cache
        CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CacheCriticalSection);
            

        if (m_spSensorPropertyValues == nullptr)
        {
            hr = E_POINTER;
        }
        else
        {
            // Retrieve property value from cache
            hr = m_spSensorPropertyValues->GetValue(key, pVar);
        }
    }

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR,
            "Failed to get the sensor property value, %!HRESULT!", 
            hr);
    }

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::SetState
//
//  Sets the property value for a given property key.
//
//  Parameters: 
//      newState - the new state of the sensor
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::SetState(
    _In_ SensorState newState
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    DWORD value;
    SensorState currentState;

    PROPVARIANT var;
    PropVariantInit(&var);    

    // Get current state
    hr = GetProperty(SENSOR_PROPERTY_STATE, &var);

    if (SUCCEEDED(hr)) 
    {
        PropVariantToUInt32(var, &value);
        currentState = (SensorState)value;

        if (currentState != newState)
        {
            // Synchronize access to the property cache
            CComCritSecLock<CComAutoCriticalSection> 
                scopeLock(m_CacheCriticalSection);

            // State has changed, update property
            // value in the cache

            Trace(
                TRACE_LEVEL_INFORMATION,
                "State has changed, now %d",
                newState);

            PropVariantClear(&var);
            InitPropVariantFromUInt32(newState, &var);

            hr = m_spSensorPropertyValues->SetValue(
                SENSOR_PROPERTY_STATE,
                &var);

            if (SUCCEEDED(hr))
            {
                m_fStateChanged = TRUE;
            }
        }
    }

    PropVariantClear(&var);

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::HasStateChanged
//
//  Checks if the state has changed since the last data event.
//
//  Parameters: 
//
//  Return Value:
//      TRUE if it has changed and clears the flag
//
///////////////////////////////////////////////////////////////////////////
BOOL CSensorDdi::HasStateChanged()
{
    FuncEntry();

    // Synchronize access to the property cache
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CacheCriticalSection);

    BOOL fStateEvent = FALSE;

    // Check if there is a valid data event to post
    if(TRUE == m_fStateChanged)
    {
        fStateEvent = m_fStateChanged;
        m_fStateChanged = FALSE;
    }

    FuncExit();

    return fStateEvent;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::SetTimeStamp
//
//  Sets the timestamp when the data cache is updated with new data.
//
//  Parameters:
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::SetTimeStamp()
{
    FuncEntry();

    // Synchronize access to the data field cache
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CacheCriticalSection);

    HRESULT hr = S_OK;

    PROPVARIANT var;
    PropVariantInit( &var );

    //Get the current time in SYSTEMTIME format
    SYSTEMTIME st;
    ::GetSystemTime(&st);

    // Convert the SYSTEMTIME into FILETIME
    FILETIME ft;
    if(FALSE == ::SystemTimeToFileTime(&st, &ft))
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
    }
    if (S_OK == hr)
    {
        var.vt                      = VT_FILETIME;
        var.filetime.dwLowDateTime  = ft.dwLowDateTime;
        var.filetime.dwHighDateTime = ft.dwHighDateTime;

        m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_TIMESTAMP, &var);
    }

    PropVariantClear( &var );

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::GetDataField
//
//  Gets the data field value for a given data field key.
//
//  Parameters: 
//      key - the requested data field key
//      pVar - location for the value of the requested data field key
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::GetDataField(
        _In_  REFPROPERTYKEY key, 
        _Out_ PROPVARIANT* pVar
        )
{ 
    FuncEntry();

    // Synchronize access to the data field cache
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CacheCriticalSection);

    HRESULT hr = S_OK;

    if (m_spSensorDataFieldValues == nullptr)
    {
        hr = E_POINTER;
    }
    else
    {
        // Retrieve the value
        hr = m_spSensorDataFieldValues->GetValue(key, pVar);

        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to get the sensor data field value, %!HRESULT!", 
                hr);
        }
    }

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::GetAllDataFields
//
//  Gets all of the data field values.
//
//  Parameters: 
//      pValues - pointer to the IPortableDeviceValues structure to place all
//          of the data field values
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::GetAllDataFields(
    _Inout_ IPortableDeviceValues* pValues)
{
    FuncEntry();

    // Synchronize access to the data field cache
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CacheCriticalSection);

    HRESULT hr = S_OK;

    if (m_spSensorDataFieldValues == nullptr)
    {
        hr = E_POINTER;
    }
    else
    {
        DWORD count = 0;
        hr = m_spSensorDataFieldValues->GetCount(&count);

        if (SUCCEEDED(hr))
        {
            // Loop through each data field and add
            // its value to the list
            for (DWORD i = 0; i < count; i++)
            {
                PROPERTYKEY key;
                PROPVARIANT var;
                PropVariantInit( &var );
                hr = m_spSensorDataFieldValues->GetAt(i, &key, &var);
                
                if (SUCCEEDED(hr))
                {
                    hr = pValues->SetValue(key, &var);
                }

                PropVariantClear(&var);
            }
        }

    }

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::CopyKeys
//
//  Copies keys from the source list to the target list.
//
//  Parameters: 
//      pSourceKeys - an IPortableDeviceKeyCollection containing the list
//          of source keys
//      pTargetKeys - an IPortableDeviceKeyCollection to contain the list
//          the copied keys
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::CopyKeys(
    _In_    IPortableDeviceKeyCollection *pSourceKeys,
    _Inout_ IPortableDeviceKeyCollection *pTargetKeys)
{
    FuncEntry();

    HRESULT hr = S_OK;
    DWORD cKeys = 0;
    PROPERTYKEY key = {0};

    if ((pSourceKeys == nullptr) ||
        (pTargetKeys == nullptr))
    {
        hr = E_INVALIDARG;
    }

    if (SUCCEEDED(hr))
    {
        hr = pSourceKeys->GetCount(&cKeys);
        
        if (SUCCEEDED(hr))
        {
            // Loop through each source key and copy to the
            // destination collection
            for (DWORD dwIndex = 0; dwIndex < cKeys; ++dwIndex)
            {
                hr = pSourceKeys->GetAt(dwIndex, &key);
                
                if (SUCCEEDED(hr))
                {
                    hr = pTargetKeys->Add(key);
                }
            }

            if (FAILED(hr))
            {
                Trace(
                    TRACE_LEVEL_ERROR,
                    "Failed to copy keys, %!HRESULT!", 
                    hr);
            }
        }
    }

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::IsPerDataFieldProperty
//
//  This method is used to determine if the specified property key
//      has per data field values
//
//  Parameters:
//      key - property key in question
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
BOOL CSensorDdi::IsPerDataFieldProperty(PROPERTYKEY key)
{
    FuncEntry();

    BOOL fPdfkey = FALSE;

    // Loop through the per data field values and see if the key matches
    for (int i = 0; i < ARRAY_SIZE(g_SupportedPerDataFieldProperties); i++)
    {
        if (IsEqualPropertyKey(key, g_SupportedPerDataFieldProperties[i]))
        {
            fPdfkey = TRUE;
            break;
        }
    }

    FuncExit();

    return fPdfkey;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::IsTestProperty
//
//  This method is used to determine if the key is a test property key.
//
//  Parameters:
//      key - property key in question
//
//  Return Value:
//      TRUE if test property key
//
///////////////////////////////////////////////////////////////////////////
BOOL CSensorDdi::IsTestProperty(PROPERTYKEY key)
{
    FuncEntry();

    BOOL fTestkey = FALSE;

    if (IsEqualPropertyKey(key, SENSOR_PROPERTY_TEST_REGISTER) ||
        IsEqualPropertyKey(key, SENSOR_PROPERTY_TEST_DATA_SIZE) ||
        IsEqualPropertyKey(key, SENSOR_PROPERTY_TEST_DATA))
    {
        fTestkey = TRUE;
    }

    FuncExit();

    return fTestkey;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::OnNewData
//
//  Callback method used to pass new data from the sensor device.
//
//  Parameters:
//      pValues - an IPortableDeviceValues collection containing the list of 
//          new data field values
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::OnNewData(
    _In_ IPortableDeviceValues* pValues
    )
{
    FuncEntry();

    HRESULT hr = S_OK;

    if (pValues == nullptr)
    {
        hr = E_INVALIDARG;
    }

    // Update the timestamp
    if (SUCCEEDED(hr))
    {
        hr = SetTimeStamp();
    }
    
    // Update cache with the new data
    if (SUCCEEDED(hr))
    {
        DWORD valueCount = 0;
        hr = pValues->GetCount(&valueCount);

        if (SUCCEEDED(hr))
        {
            // Synchronize access to the property cache
            CComCritSecLock<CComAutoCriticalSection> 
                scopeLock(m_CacheCriticalSection);

            // Loop through each data field value
            // and update cache
            for (DWORD i = 0; i < valueCount; i++)
            {
                PROPERTYKEY key = WPD_PROPERTY_NULL;
                PROPVARIANT var;

                PropVariantInit(&var);

                hr = pValues->GetAt(i, &key, &var);

                if (SUCCEEDED(hr))
                {
                    // Data value was already validated. Go
                    // ahead and update cache
                    hr = m_spSensorDataFieldValues->SetValue(key, &var);
                }

                PropVariantClear(&var);
                
                if (FAILED(hr))
                {
                    break;
                }
            }
        }
    }

    // Mark sensor state as ready.
    if (SUCCEEDED(hr))
    {
        hr = SetState(SENSOR_STATE_READY);
    }

    if (SUCCEEDED(hr))
    {
        Trace(
            TRACE_LEVEL_VERBOSE,
            "New data received from the device");
        
        // Raise an event to signal new data
        RaiseDataEvent();
    }

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::RaiseDataEvent
//
//  This method is called when new data is received from the device.  It only
//  raises an event if there are clients subscribed.
//
//  Parameters:
//
//  Return Value:
//      none
//
///////////////////////////////////////////////////////////////////////////
VOID CSensorDdi::RaiseDataEvent()
{
    FuncEntry();

    // Check if there are any subscribers
    if(m_pClientManager->GetSubscriberCount() > 0)
    {
        // Inform the report manager when new data is available.
        // It will determine when a data event should be
        // posted to the class extension.
        m_pReportManager->NewDataAvailable();
    }

    FuncExit();
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::PostDataEvent
//
//  This method is called to post a new data event to the class extension.
//
//  Parameters:
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::PostDataEvent(
    _In_ IPortableDeviceValues *pValues)
{
    FuncEntry();

    HRESULT hr = S_OK;

    CComPtr<IPortableDeviceValuesCollection> spEventCollection;
    
    if (spEventCollection == nullptr)
    {
        // CoCreate a collection to hold the data field values
        hr = CoCreateInstance( 
            CLSID_PortableDeviceValuesCollection,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&spEventCollection));

        if (SUCCEEDED(hr))
        {
            // Add the values to the collection
            hr = spEventCollection->Add(pValues);
            if (SUCCEEDED(hr) && (m_spClassExtension != nullptr))
            {
                Trace(
                    TRACE_LEVEL_VERBOSE,
                    "Posting new data event to the class extension");

                // Post the data event to the class extension
                hr = m_spClassExtension->PostEvent( 
                    GetSensorObjectID(), 
                    spEventCollection);            
            }
        }
    }

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::PollForData
//
//  This method is called to synchronously poll the device for new data
//  and update the data cache.
//
//  Parameters:
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::PollForData()
{
    CComPtr<IPortableDeviceValues> spValues = nullptr;
    HRESULT hr;

    // Create an IPortableDeviceValues to hold the data
    hr = CoCreateInstance(
        CLSID_PortableDeviceValues,
        nullptr,
        CLSCTX_INPROC_SERVER,
        IID_PPV_ARGS(&spValues));
                
    if (SUCCEEDED(hr))
    {
        hr = m_spSensorDevice->RequestNewData(spValues);
    
        if (FAILED(hr))
        {
            Trace(
                TRACE_LEVEL_ERROR,
                "Failed to poll for new data, %!HRESULT!",
                hr);
        }
    }
    
    // Update cache with the new data
    if (SUCCEEDED(hr))
    {
        DWORD valueCount = 0;
        hr = spValues->GetCount(&valueCount);

        if (SUCCEEDED(hr))
        {
            // Synchronize access to the property cache
            CComCritSecLock<CComAutoCriticalSection> 
                scopeLock(m_CacheCriticalSection);

            // Loop through each data field value
            // and update cache
            for (DWORD i = 0; i < valueCount; i++)
            {
                PROPERTYKEY key = WPD_PROPERTY_NULL;
                PROPVARIANT var;

                PropVariantInit(&var);

                hr = spValues->GetAt(i, &key, &var);

                if (SUCCEEDED(hr))
                {
                    // Data value was already validated. Go
                    // ahead and update cache
                    hr = m_spSensorDataFieldValues->SetValue(key, &var);
                }

                PropVariantClear(&var);
                
                if (FAILED(hr))
                {
                    break;
                }
            }
        }
    }

    // Update the timestamp
    if (SUCCEEDED(hr))
    {
        hr = SetTimeStamp();
    }

    // Mark sensor state as ready.
    if (SUCCEEDED(hr))
    {
        hr = SetState(SENSOR_STATE_READY);
    }

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::ApplyUpdatedProperties
//
//  This method retrieves the settable values from the client manager
//  and applies them to the driver.  This method must be called each time
//  the properties are changed or a client changes its subscription status.
//
//  Parameters:
//
//  Return Value:
//      status
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::ApplyUpdatedProperties(
    )
{
    FuncEntry();

    HRESULT hr = (m_fSensorInitialized == TRUE) ? S_OK : E_UNEXPECTED;

    if (SUCCEEDED(hr))
    {
        PROPVARIANT var;
        PropVariantInit(&var);
        
        // Update the report interval
        hr = m_pClientManager->GetArbitratedProperty(
            SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,
            &var);
        
        if (SUCCEEDED(hr))
        {
            // Update device with report interval
            hr = m_spSensorDevice->SetReportInterval(var.ulVal);

            if (SUCCEEDED(hr))
            {
                m_pReportManager->SetReportInterval(var.ulVal);
            }
        }
        
        if (SUCCEEDED(hr))
        {
            // Update the change sensitivity
            hr = m_pClientManager->GetArbitratedProperty(
                SENSOR_PROPERTY_CHANGE_SENSITIVITY,
                &var);

            if (SUCCEEDED(hr))
            {
                // Update device with change sensitivity
                hr = m_spSensorDevice->SetChangeSensitivity(&var);
            }
        }
        
        PropVariantClear(&var);
    }

    if (SUCCEEDED(hr))
    {
        DATA_UPDATE_MODE newMode = m_pClientManager->GetDataUpdateMode();

        if (newMode != m_DataUpdateMode)
        {
            Trace(
                TRACE_LEVEL_INFORMATION,
                "Data update mode has changed to %d",
                newMode);

            hr = SetDataUpdateMode(newMode);
        }
    }

    FuncExit();

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
//  CSensorDdi::ReportIntervalExpired
//
//  This method is called when the report interval has expired and new
//  data has been recieved.
//
//  Parameters:
//
//  Return Value:
//      none
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensorDdi::ReportIntervalExpired()
{
    FuncEntry();

    HRESULT hr = S_OK;
    
    // Create the event parameters collection if it doesn't exist
    CComPtr<IPortableDeviceValues> spEventParams;
    if (spEventParams == nullptr)
    {
        hr = CoCreateInstance(
            CLSID_PortableDeviceValues,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&spEventParams));
    }

    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR, 
            "Failed to CoCreateInstance for Event Parameters, %!HRESULT!", 
            hr);

        return 0;
    }

    if (SUCCEEDED(hr))
    {
        // Initialize the event parameters
        spEventParams->Clear();

        // Populate the event type
        hr = spEventParams->SetGuidValue(
            SENSOR_EVENT_PARAMETER_EVENT_ID, 
            SENSOR_EVENT_DATA_UPDATED);
    }

    if (SUCCEEDED(hr))
    {
        // Get the All the Data Field values
        // Populate the event parameters
        if (SUCCEEDED(hr)) {
            hr = GetAllDataFields(spEventParams);
        }

        if (SUCCEEDED(hr) && HasStateChanged())
        {
            PROPVARIANT var;
            ULONG value = 0;
            SensorState currentState;

            PropVariantInit(&var);
            
            hr = GetProperty(SENSOR_PROPERTY_STATE, &var);

            if (SUCCEEDED(hr))
            {
                hr = PropVariantToUInt32(var, &value);
            }

            if (SUCCEEDED(hr))
            {
                currentState = (SensorState)value;

                Trace(
                    TRACE_LEVEL_INFORMATION,
                    "Posting state change, now %d",
                    currentState);

                // Post a state change event
                hr = m_spClassExtension->PostStateChange(
                    GetSensorObjectID(), 
                    currentState);
            }

            PropVariantClear(&var);
        }

        if( SUCCEEDED(hr) )
        {
            Trace(
                TRACE_LEVEL_VERBOSE,
                "Posting data event");

            hr = PostDataEvent(spEventParams);
        }
    }

    FuncExit();

    return hr;
}
