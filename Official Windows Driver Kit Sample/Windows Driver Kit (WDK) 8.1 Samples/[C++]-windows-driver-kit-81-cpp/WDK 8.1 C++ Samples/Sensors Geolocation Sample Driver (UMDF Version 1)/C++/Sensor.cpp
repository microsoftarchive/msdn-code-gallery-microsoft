 /*

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


 Module:
      Sensor.cpp

 Description:
      Implements the CSensor container class

--*/

#include "internal.h"
#include "SensorDdi.h"
#include "SensorManager.h"

#include "Sensor.h"


#include "Sensor.tmh"

// TODO Set the follow values so they are appropriate for the device.
#define LONG_REPORT_INTERVAL_MS 5 * 60000   // Number of miliseconds where a report interval
                                            // is long enough that the device should be
                                            // powered down.
#define DEVICE_STARTUP_MS 60000             // Average number of miliseconds it takes the
                                            // device to find a location fix from a cold
                                            // start with no assistance data.

/////////////////////////////////////////////////////////////////////////
//
// CSensor::CSensor
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CSensor::CSensor()
{
    m_spSensorPropertyValues = nullptr;
    m_spSensorDataFieldValues = nullptr;
    m_fSensorInitialized = FALSE;
    m_fValidDataEvent = FALSE;

    m_fSensorUpdated = FALSE; // flag checks to see if data report was received
    m_fInitialDataReceived = FALSE;  // flag checks to see if inital poll request was fulfilled
    m_fReportingState = FALSE; // tracks whether sensor reporting is turned on or off
    m_ulPowerState = 0;    // tracks in what state sensor power should be

    m_spSupportedSensorDataFields = nullptr;
    m_spSensorPropertyValues = nullptr;

    m_spSupportedSensorProperties = nullptr;
    m_spSettableSensorProperties = nullptr;
    m_spRequiredDataFields = nullptr;

    m_spSensorDataFieldValues = nullptr;

    m_fSensorInitialized = FALSE;    // flag checks if we have been initialized

    m_fValidDataEvent = FALSE;         // flag that indicates an event needs to be fired

#if (NTDDI_VERSION >= NTDDI_WIN8)
    m_ulCurrentGeolocationRadioState = DRS_RADIO_ON;
    m_ulRequiredGeolocationRadioState = DRS_RADIO_ON;
    m_ulPreviousGeolocationRadioState = DRS_RADIO_ON;
#endif

    m_spWdfDevice2 = nullptr;

    m_pThreadpool = nullptr;
    SecureZeroMemory(&m_ThreadpoolEnvironment, sizeof(TP_CALLBACK_ENVIRON));
    m_pLongReportIntervalTimer = nullptr;
    m_fUsingLongReportIntervalTimer = false;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensor::~CSensor
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CSensor::~CSensor()
{
    Uninitialize();
}

/////////////////////////////////////////////////////////////////////////
//
// CSensor::Initialize
//
// Initializes the PROPERTYKEY/PROPVARIANT values for
// the Supported Properties & Supported Data
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensor::InitializeSensor(
    _In_ SensorType sensType, 
    _In_ DWORD sensNum, 
    _In_ LPWSTR pwszManufacturer,
    _In_ LPWSTR pwszProduct,
    _In_ LPWSTR pwszSerialNumber,
    _In_ LPWSTR pwszSensorID,
    _In_ IWDFDevice* pWdfDevice)
{
    //CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    // Check if we are already initialized
    HRESULT hr = (TRUE == IsInitialized()) ? E_UNEXPECTED : S_OK;

    if (SUCCEEDED(hr))
    {
        hr = pWdfDevice->QueryInterface(IID_PPV_ARGS(&m_spWdfDevice2));
    }

    if(SUCCEEDED(hr))
    {
        if(NULL == m_spSupportedSensorProperties)
        {
            // Create a new PortableDeviceKeyCollection to store the supported property KEYS
            hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_IPortableDeviceKeyCollection,
                                  (VOID**)&m_spSupportedSensorProperties);
        }

        if(NULL == m_spSensorPropertyValues)
        {
            // Create a new PortableDeviceValues to store the property VALUES
            hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_IPortableDeviceValues,
                                  (VOID**)&m_spSensorPropertyValues);
        }
        
        if(NULL == m_spSupportedSensorDataFields)
        {
            // Create a new PortableDeviceValues to store the supported datafield KEYS
            hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_IPortableDeviceKeyCollection,
                                  (VOID**)&m_spSupportedSensorDataFields);
        }     

        if(NULL == m_spRequiredDataFields)
        {
            // Create a new PortableDeviceValues to store the supported datafield KEYS
            hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_IPortableDeviceKeyCollection,
                                  (VOID**)&m_spRequiredDataFields);
        }

        if(NULL == m_spSensorDataFieldValues)
        {
            // Create a new PortableDeviceValues to store the datafield VALUES
            hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_IPortableDeviceValues,
                                  (VOID**)&m_spSensorDataFieldValues);
        }

        if(NULL == m_spSettableSensorProperties)
        {
            // Create a new PortableDeviceValues to store the settable property keys
            hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_IPortableDeviceKeyCollection,
                                  (VOID**)&m_spSettableSensorProperties);
        }
    }

    if(SUCCEEDED(hr))
    {
        m_SensorType = sensType;
        m_SensorNum = sensNum;
        hr = StringCchCopy(m_pwszManufacturer, DESCRIPTOR_MAX_LENGTH, pwszManufacturer);

        if (SUCCEEDED(hr))
        {
            hr = StringCchCopy(m_pwszProduct, DESCRIPTOR_MAX_LENGTH, pwszProduct);
        }

        if (SUCCEEDED(hr))
        {
            hr = StringCchCopy(m_pwszSerialNumber, DESCRIPTOR_MAX_LENGTH, pwszSerialNumber);
        }

        if (SUCCEEDED(hr))
        {
            hr = StringCchCopy(m_SensorID, DESCRIPTOR_MAX_LENGTH, pwszSensorID);
        }
    }
    
    // Initialize the Report Interval and Change Sensitivities
    if (SUCCEEDED(hr))
    {
        m_ulLowestClientReportInterval = ULONG_MAX;

        for (int idx = 0; idx < MAX_NUM_DATA_FIELDS; idx++)
        {
            m_fltLowestClientChangeSensitivities[idx] = FLT_MAX;
        }
    }

    if(SUCCEEDED(hr))
    {
        m_fSensorInitialized = TRUE;
    }

    // Create threadpool for long report interval timer
    if (SUCCEEDED(hr))
    {
        m_pThreadpool = CreateThreadpool(nullptr);
        if (nullptr != m_pThreadpool)
        {
            SetThreadpoolThreadMaximum(m_pThreadpool, 1);
            if (TRUE == SetThreadpoolThreadMinimum(m_pThreadpool, 1))
            {
                InitializeThreadpoolEnvironment(&m_ThreadpoolEnvironment);
                SetThreadpoolCallbackPool(&m_ThreadpoolEnvironment, m_pThreadpool);
            }
            else
            {
                CloseThreadpool(m_pThreadpool);
                m_pThreadpool = nullptr;
                hr = HRESULT_FROM_WIN32(GetLastError());
                Trace(TRACE_LEVEL_ERROR, "%!FUNC! Failed SetThreadpoolThreadMinimum, hr = %!HRESULT!", hr);
            }
        }
        else
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
            Trace(TRACE_LEVEL_ERROR, "%!FUNC! Failed to create thread pool, hr = %!HRESULT!", hr);
        }

        if (SUCCEEDED(hr))
        {
            m_pLongReportIntervalTimer = CreateThreadpoolTimer(
                LongReportIntervalTimerCallback,    // Timer callback
                this,                               // Optional data to pass to callback
                &m_ThreadpoolEnvironment            // Callback environment
                );
            if (nullptr == m_pLongReportIntervalTimer)
            {
                hr = HRESULT_FROM_WIN32(GetLastError());
                Trace(TRACE_LEVEL_ERROR, "%!FUNC! Failed to create thread pool timer, hr = %!HRESULT!", hr);
            }
        }
    }


    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CSensor::Uninitialize
//
// Initializes the PROPERTYKEY/PROPVARIANT values for
// the Supported Properties & Supported Data
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensor::Uninitialize()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    // Check if we are already initialized
    HRESULT hr = (FALSE == IsInitialized()) ? E_UNEXPECTED : S_OK;

    // Clean up threadpool
    if (SUCCEEDED(hr))
    {
        if (nullptr != m_pLongReportIntervalTimer)
        {
            // Stop timer, cancel any pending events, and close timer
            SetThreadpoolTimer(m_pLongReportIntervalTimer, nullptr, 0, 0);
            WaitForThreadpoolTimerCallbacks(m_pLongReportIntervalTimer, true);
            CloseThreadpoolTimer(m_pLongReportIntervalTimer);
        }

        if (nullptr != m_pThreadpool)
        {
            CloseThreadpool(m_pThreadpool);
        }

        DestroyThreadpoolEnvironment(&m_ThreadpoolEnvironment);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSupportedSensorProperties->Clear();

        if(SUCCEEDED(hr))
        {
            hr = m_spSettableSensorProperties->Clear();
        }

        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorDataFields->Clear();
        }

        if(SUCCEEDED(hr))
        {
            hr = m_spRequiredDataFields->Clear();
        }

        if(SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->Clear();
        }

        if(SUCCEEDED(hr))
        {
            hr = m_spSensorDataFieldValues->Clear();
        }

        m_spSensorPropertyValues = NULL;
        m_spSensorDataFieldValues = NULL;
        m_spSupportedSensorProperties = NULL;
        m_spSettableSensorProperties = NULL;
        m_spSupportedSensorDataFields = NULL;
        m_spRequiredDataFields = NULL;

        m_pClientMap.RemoveAll();
        m_pSubscriberMap.RemoveAll();

        m_fSensorInitialized = FALSE;
    }

    return hr;
}
///////////////////////////////////////////////////////////////////////////
//
// CSensor::GetSettableProperties
//
// Gets the list of SettableProperties for the device
// 
// Returns a collection of PROPERTYKEYS
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensor::GetSettableProperties(IPortableDeviceKeyCollection **ppKeys)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    if(NULL == ppKeys)
    {
        hr = E_INVALIDARG;
    }

    // Make sure we have been Initialized
    if((FALSE == IsInitialized()) || (NULL == m_spSettableSensorProperties))
    {
        hr = E_UNEXPECTED;
    }
    
    if(SUCCEEDED(hr))
    {
        hr = m_spSettableSensorProperties.CopyTo(ppKeys);
    }

    return hr;
}


///////////////////////////////////////////////////////////////////////////
//
// CSensor::GetSupportedProperties
//
// Gets the list of SupportedProperties for the device
// 
// Returns a collection of PROPERTYKEYS
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensor::GetSupportedProperties(IPortableDeviceKeyCollection **ppKeys)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    if(NULL == ppKeys)
    {
        hr = E_INVALIDARG;
    }

    // Make sure we have been Initialized
    if((FALSE == IsInitialized()) || (NULL == m_spSupportedSensorProperties))
    {
        hr = E_UNEXPECTED;
    }
    
    if(SUCCEEDED(hr))
    {
        hr = m_spSupportedSensorProperties.CopyTo(ppKeys);
    }

    return hr;
}

//////////////////////////////////////////////////////////////////////////////////////
//
// CSensor::GetSupportedDataFields
//
// Gets the list of SupportedDataFields for the device
// 
// Returns a collection of PROPERTYKEYS
//
//////////////////////////////////////////////////////////////////////////////////////
HRESULT CSensor::GetSupportedDataFields(IPortableDeviceKeyCollection **ppKeys)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    if(NULL == ppKeys)
    {
        hr = E_INVALIDARG;
    }

    // Make sure we have been Initialized
    if((FALSE == IsInitialized()) || (NULL == m_spSupportedSensorDataFields))
    {
        hr = E_UNEXPECTED;
    }
    
    if(SUCCEEDED(hr))
    {
        hr = m_spSupportedSensorDataFields.CopyTo(ppKeys);
    }
    
    return hr;
}

//////////////////////////////////////////////////////////////////////////////////////
//
// CSensor::GetRequiredDataFields
//
// Gets the list of RequiredDataFields for the Gps sensor
// 
// Returns a collection of PROPERTYKEYS
//
//////////////////////////////////////////////////////////////////////////////////////
HRESULT CSensor::GetRequiredDataFields(IPortableDeviceKeyCollection **ppKeys)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    if(NULL == ppKeys)
    {
        hr = E_INVALIDARG;
    }

    // Make sure we have been Initialized
    if((FALSE == IsInitialized()) || (NULL == m_spRequiredDataFields))
    {
        hr = E_UNEXPECTED;
    }
    
    if(SUCCEEDED(hr))
    {
        hr = m_spRequiredDataFields.CopyTo(ppKeys);
    }
    
    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
// CSensor::GetProperty
//
// Gets the Property Value for a given Property key.
// 
// Called to retrive a property of the device
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensor::GetProperty(REFPROPERTYKEY key, PROPVARIANT *pValue)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    if(NULL == m_spSensorPropertyValues)
    {
        hr = E_UNEXPECTED;
    }
    else
    {
        // Retrieve the value
        hr = m_spSensorPropertyValues->GetValue(key, pValue);
    }

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
// CSensor::GetDataField
//
// Gets the Data Field Value for a given Property key.
// 
// Called to retrieve a data field of the device
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensor::GetDataField(REFPROPERTYKEY key, PROPVARIANT *pValue)
{ 
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    if(NULL == m_spSensorDataFieldValues)
    {
        hr = E_UNEXPECTED;
    }
    else
    {
        // Retrieve the value
        hr = m_spSensorDataFieldValues->GetValue(key, pValue);
    }

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
// CSensor::GetAllDataFieldValues
//
// Gets the all the Data  values 
// 
// Called to retrieve all data field values of the device
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensor::GetAllDataFieldValues(IPortableDeviceValues* pValues)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    if(NULL == m_spSensorDataFieldValues)
    {
        hr = E_UNEXPECTED;
    }
    else
    {
        DWORD dwCount = 0;
        hr = m_spSensorDataFieldValues->GetCount( &dwCount );

        if( SUCCEEDED(hr) )
        {
            for( DWORD i = 0 ; i < dwCount ; i++ )
            {
                PROPERTYKEY key;
                PROPVARIANT var;
                PropVariantInit( &var );
                hr = m_spSensorDataFieldValues->GetAt( i, &key, &var );
                
                if( SUCCEEDED(hr) )
                {
                    hr = pValues->SetValue( key, &var );
                }

                PropVariantClear(&var);
            }
        }

    }

    return hr;
}


///////////////////////////////////////////////////////////////////////////
//
// CSensor::SetProperty
//
// Sets the Property Value for a given Property key.
// 
// Called during Initialize and when a property is read from the device
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensor::SetProperty(REFPROPERTYKEY key, const PROPVARIANT *pValue, IPortableDeviceValues* spDfSensVals)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe
    
    HRESULT hr = S_OK;

    if (nullptr != pValue) 
    {
        if (pValue->vt == VT_EMPTY)
        {
            // special case for initializing the property value
            hr = m_spSensorPropertyValues->SetValue(key, pValue);
        }

        else if (TRUE == IsEqualPropertyKey(key, SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL))
        {
            // Validate Range for Report Interval 
            if (VT_UI4 == V_VT(pValue))
            {
                ULONG ulReportInterval = V_UI4(pValue);
                if (ulReportInterval < m_ulDefaultMinimumReportInterval)
                {
                    hr = E_INVALIDARG;
                    Trace(TRACE_LEVEL_ERROR, "Desired report interval < current minimum for %s: input, hr = %!HRESULT!", m_SensorName, hr);
                }
            }

            if(SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetValue(key, pValue);
            }
        }

        else if (TRUE == IsEqualPropertyKey(key, SENSOR_PROPERTY_LOCATION_DESIRED_ACCURACY))
        {
            // Validate Range for Location Desired Accuracy 
            if (VT_UI4 == V_VT(pValue))
            {
                ULONG ulLocationDesiredAccuracy = V_UI4(pValue);
                if (ulLocationDesiredAccuracy > LOCATION_DESIRED_ACCURACY_HIGH)
                {
                    hr = E_INVALIDARG;
                    Trace(TRACE_LEVEL_ERROR, "Desired location desired accuracy > maximum for %s: input, hr = %!HRESULT!", m_SensorName, hr);
                }
            }

            if(SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetValue(key, pValue);
            }
        }

        else
        {
            hr = m_spSensorPropertyValues->SetValue(key, pValue);
        }
    }

    else if (TRUE == IsEqualPropertyKey(key, SENSOR_PROPERTY_CHANGE_SENSITIVITY) && (nullptr != spDfSensVals))
    {
        DWORD cDfVals = 0;

        if (SUCCEEDED(hr))
        {
            hr = spDfSensVals->GetCount(&cDfVals);

            if (SUCCEEDED(hr))
            {
                PROPERTYKEY pkDfKey;
                PROPVARIANT var;
                FLOAT fltSensitivity = 0;

                for (DWORD dwIdx = 0; dwIdx < cDfVals; dwIdx++)
                {
                    if (SUCCEEDED(hr))
                    {
                        PropVariantInit( &var );
                        hr = spDfSensVals->GetAt(dwIdx, &pkDfKey, &var);
                        if (SUCCEEDED(hr))
                        {
                            fltSensitivity = var.fltVal;
                        }
                        PropVariantClear(&var);
                    }

                    if (SUCCEEDED(hr))
                    {
                        // Validate sensitivity for the specific data field

                        if (fltSensitivity < 0.0F)
                        {
                            hr = E_INVALIDARG;
                            Trace(TRACE_LEVEL_ERROR, "Desired change sensitivity < 0.0 for %s: input, hr = %!HRESULT!", m_SensorName, hr);
                        }
                        else
                        {
                            hr = spDfSensVals->SetFloatValue(pkDfKey, fltSensitivity);
                        }
                    }
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_CHANGE_SENSITIVITY, spDfSensVals);
        }
    }
    else
    {
        hr = E_UNEXPECTED;
    }

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
// CSensor::SetDataField
//
// Sets the Data Field Value for a given DataField key.
// 
// Called during Initialize and when data is read from the device
//
///////////////////////////////////////////////////////////////////////////
HRESULT CSensor::SetDataField(REFPROPERTYKEY key, const PROPVARIANT *pValue)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    if(NULL == pValue)
    {
        hr = E_INVALIDARG;
    }

    if(NULL == m_spSensorDataFieldValues)
    {
        hr = E_POINTER;
    }
    
    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(key, pValue);
    }

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
// CSensor::Subscribe
//
// Sets the status of event subscribers
// 
// 
///////////////////////////////////////////////////////////////////////////
HRESULT CSensor::Subscribe(_In_ IWDFFile* appID)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = S_OK;

    if (nullptr != m_pSensorManager)
    {
        if (nullptr != m_pSensorManager->m_pSensorDDI)
        {
            BOOL fClientIsSubscribed = FALSE;

            hr = m_pSensorManager->m_pSensorDDI->CheckForSubscriber(this, appID, &fClientIsSubscribed);

            if (SUCCEEDED(hr) && (FALSE == fClientIsSubscribed))
            {
                SUBSCRIBER_ENTRY entry;

                entry.fSubscribed = TRUE;
                m_pSubscriberMap[appID] = entry;
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Subscriber %p is already present in %s, hr = %!HRESULT!", appID, m_SensorName, hr);
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "m_pSensorDDI is NULL in %s, hr = %!HRESULT!", m_SensorName, hr);
        }
    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "m_pSensorManager is NULL in %s, hr = %!HRESULT!", m_SensorName, hr);
    }

    return hr;
}


///////////////////////////////////////////////////////////////////////////
//
// CSensor::Unsubscribe
//
// Sets the status of event subscribers
// 
// 
///////////////////////////////////////////////////////////////////////////
HRESULT CSensor::Unsubscribe(_In_ IWDFFile* appID)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = S_OK;

    if (nullptr != m_pSensorManager)
    {
        if (nullptr != m_pSensorManager->m_pSensorDDI)
        {
            BOOL fSubscriberIsPresent = FALSE;

            hr = m_pSensorManager->m_pSensorDDI->CheckForSubscriber(this, appID, &fSubscriberIsPresent);

            if (SUCCEEDED(hr) && (TRUE == fSubscriberIsPresent))
            {
                if (SUCCEEDED(hr))
                {
                    size_t cEventSubscribers = m_pSubscriberMap.GetCount();
                    if (cEventSubscribers > 0)
                    {
                        hr = m_pSensorManager->m_pSensorDDI->RemoveSubscriber(this, appID);
                    }
                    else
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Attempt to reduce subscriber count below 0 in %s, AppID %p", m_SensorName, appID);
                    }
                }
                else
                {
                    hr = E_UNEXPECTED;
                    Trace(TRACE_LEVEL_ERROR, "Failed to remove subscriber %p from %s, hr = %!HRESULT!", appID, m_SensorName, hr);
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Subscriber %p is not present in %s, hr = %!HRESULT!", appID, m_SensorName, hr);
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "m_pSensorDDI is NULL in %s, hr = %!HRESULT!", m_SensorName, hr);
        }
    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "m_pSensorManager is NULL in %s, hr = %!HRESULT!", m_SensorName, hr);
    }

    return hr;
}

///////////////////////////////////////////////////////////////////////////
//
// CSensor::GetSubscriberCount
//
// Retrieves the status of event subscribers
// 
// 
///////////////////////////////////////////////////////////////////////////
DWORD CSensor::GetSubscriberCount(void)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    return (DWORD)m_pSubscriberMap.GetCount();
}

/////////////////////////////////////////////////////////////////////////
//
// CSensor::RemoveProperty
//
// Removes the PROPERTYKEY passed in from the list of Supported Properties
//
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensor::RemoveProperty(REFPROPERTYKEY key)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = S_OK;
    DWORD cKeys = 0;
    PROPERTYKEY tempKey;

    if (NULL != m_spSupportedSensorProperties)
    {
        hr = m_spSupportedSensorProperties->GetCount(&cKeys);

        if (SUCCEEDED(hr))
        {
            for (DWORD dwIndex = 0; dwIndex < cKeys; ++dwIndex)
            {
                hr = m_spSupportedSensorProperties->GetAt(dwIndex, &tempKey);

                if (SUCCEEDED(hr))
                {
                    if (IsEqualPropertyKey(tempKey, key))
                    {
                        hr = m_spSupportedSensorProperties->RemoveAt(dwIndex);

                        if (SUCCEEDED(hr))
                            hr = m_spSensorPropertyValues->RemoveValue(key); 

                        break;
                    }
                }
            }
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensor::RemoveDataField
//
// Removes the PROPERTYKEY passed in from the list of Supported DataFields
//
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensor::RemoveDataField(REFPROPERTYKEY key)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = S_OK;
    DWORD cKeys = 0;
    PROPERTYKEY tempKey;

    if (NULL != m_spSupportedSensorDataFields)
    {
        hr = m_spSupportedSensorDataFields->GetCount(&cKeys);

        if (SUCCEEDED(hr))
        {
            for (DWORD dwIndex = 0; dwIndex < cKeys; ++dwIndex)
            {
                hr = m_spSupportedSensorDataFields->GetAt(dwIndex, &tempKey);

                if (SUCCEEDED(hr))
                {
                    if (IsEqualPropertyKey(tempKey, key))
                    {
                        hr = m_spSupportedSensorDataFields->RemoveAt(dwIndex);

                        if (SUCCEEDED(hr))
                            hr = m_spSensorDataFieldValues->RemoveValue(key); 

                        break;
                    }
                }
            }
        }
    }

    return hr;
}


// Sets the timestamp when new data is updated
HRESULT CSensor::SetTimeStamp()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    HRESULT hr = S_OK;

    PROPVARIANT var;

    // Get the current time as FILETIME format
    FILETIME ft;

#if (NTDDI_VERSION >= NTDDI_WIN8)
    GetSystemTimePreciseAsFileTime(&ft); // Use the higher resolution timer when available
#else
    GetSystemTimeAsFileTime(&ft); // API not available, fallback to lower resolution timer
#endif

    hr = InitPropVariantFromFileTime(&ft, &var);

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_TIMESTAMP, &var);
    }

    PropVariantClear( &var );

    return hr;
}

// Called when new data is updated from device
VOID CSensor::RaiseDataEvent()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    size_t cSubscribers = m_pSubscriberMap.GetCount();

// no wdk content

    // Check if there are any subscribers
    if( 0 < cSubscribers )
    {
        // Set the data event flag 
        m_fValidDataEvent = TRUE;
        
        if( NULL != m_hSensorEvent )
        {
            SetEvent(m_hSensorEvent);
        }
    }
}

// Checks if there is a valid data event
// If so returns TRUE and resets the internal flag
BOOL CSensor::HasValidDataEvent()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);

    BOOL fDataEvent = FALSE;

    // Check if there is a valid data event to post
    if(TRUE == m_fValidDataEvent)
    {
        fDataEvent = m_fValidDataEvent;
        m_fValidDataEvent = FALSE;
    }

    return fDataEvent;
}

// Sets the persistent unique ID property
// called if no overloaded version
HRESULT CSensor::SetUniqueID(_In_ IWDFDevice* pWdfDevice)
{
    HRESULT hr = S_OK;

    CComPtr<IWDFNamedPropertyStore> spPropStore;
    if (SUCCEEDED(hr))
    {
        hr = pWdfDevice->RetrieveDevicePropertyStore(NULL, WdfPropertyStoreCreateIfMissing, &spPropStore, NULL);
    }

    if(SUCCEEDED(hr))
    {
        GUID idGuid;
        
        CComBSTR bstr(GetSensorObjectID());
        LPCWSTR lpcszKeyName = bstr;
        
        PROPVARIANT vID;
        PropVariantInit(&vID);
        hr = spPropStore->GetNamedValue(lpcszKeyName, &vID);
        if (SUCCEEDED(hr))
        {
            hr = ::CLSIDFromString(vID.bstrVal, &idGuid);
        }
        else
        {
            hr = ::CoCreateGuid(&idGuid);
            if (SUCCEEDED(hr))
            {
                LPOLESTR lpszGUID = NULL;
                hr = ::StringFromCLSID(idGuid, &lpszGUID);
                if (SUCCEEDED(hr))
                {
                    vID.vt = VT_LPWSTR;
                    vID.pwszVal = lpszGUID;
                    hr = spPropStore->SetNamedValue(lpcszKeyName, &vID);
                }
            }
        }

        PropVariantClear(&vID);

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_PERSISTENT_UNIQUE_ID, idGuid);
        }
    }

    return hr;
}

HRESULT CSensor::HandleReportIntervalUpdate()
{
    HRESULT hr = S_OK;

    ULONG ulRequestedReportInterval = 0;
    ULONG ulDeviceReportInterval = 0;
    ULONG ulRequiredReportIntervalAtApi = 0;

    BOOL fReportIntervalSupported = TRUE;

    PROPVARIANT var;

    PropVariantInit(&var);

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->GetValue(SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, &var);
    }

    if (SUCCEEDED(hr))
    {
        if (VT_EMPTY == var.vt)
        {
            if (fReportIntervalSupported)
            {
                if (ulRequestedReportInterval == 0)
                {
                    ulRequestedReportInterval = m_ulDefaultCurrentReportInterval;
                }
            }
            else
            {
                ulRequestedReportInterval = m_ulDefaultCurrentReportInterval;
            }
        }
        else
        {
            hr = m_spSensorPropertyValues->GetUnsignedIntegerValue(SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, &ulRequestedReportInterval);
        }
    }

    if (SUCCEEDED(hr))
    {
        // '0' to the sensor API means the default value
        if (ulRequestedReportInterval == 0)
        {
            ulRequestedReportInterval = m_ulDefaultCurrentReportInterval;
        }
        else
        {
            ulRequestedReportInterval = m_ulLowestClientReportInterval;
        }

        //pulReportInterval is the value we request from the device
        if (ulRequestedReportInterval < 0xFFFFFFFF)
        {
            if (m_pSensorManager->m_fDeviceActive)
            {
                // Write the report interval value to the device and then read back what the device has set the report interval to before
                // setting this value for the DeviceProperties and at the API
                Trace(TRACE_LEVEL_INFORMATION, "Requesting Report Interval = %i on %s", ulRequestedReportInterval, m_SensorName);

                ulDeviceReportInterval = ulRequestedReportInterval;
                Trace(TRACE_LEVEL_INFORMATION, "Device Report Interval = %i on %s", ulDeviceReportInterval, m_SensorName);

                // check to see this is valid and set it at the api
                ulRequiredReportIntervalAtApi = ulDeviceReportInterval;

                if (SUCCEEDED(hr))
                {
                    // Set this possible changed value in DeviceProperties and at the API

                    if (SUCCEEDED(hr))
                    {
                        Trace(TRACE_LEVEL_INFORMATION, "Setting API Report Interval = %i on %s", ulRequiredReportIntervalAtApi, m_SensorName);
                        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, ulRequiredReportIntervalAtApi);
                    }
                    else
                    {
                        Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValue = Sensor ReportInterval in %s, hr = %!HRESULT!", m_SensorName, hr);
                    }
                }
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed to set then get %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                }
            }
            else
            {
                Trace(TRACE_LEVEL_WARNING, "%!FUNC! Not updating report interval as device is not active");
            }
        }
    }

    PropVariantClear(&var);

    return hr;
}

HRESULT CSensor::HandleLocationDesiredAccuracyUpdate()
{
    HRESULT hr = S_OK;

    ULONG ulRequestedLocationDesiredAccuracy = 0;
    ULONG ulDeviceLocationDesiredAccuracy = 0;
    ULONG ulRequiredLocationDesiredAccuracyAtApi = 0;

    BOOL fLocationDesiredAccuracySupported = TRUE;

    PROPVARIANT var;

    PropVariantInit(&var);

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->GetValue(SENSOR_PROPERTY_LOCATION_DESIRED_ACCURACY, &var);
    }

    if (SUCCEEDED(hr))
    {
        if (VT_EMPTY == var.vt)
        {
            if (fLocationDesiredAccuracySupported)
            {
                if (ulRequestedLocationDesiredAccuracy == 0)
                {
                    ulRequestedLocationDesiredAccuracy = LOCATION_DESIRED_ACCURACY_DEFAULT;
                }
            }
            else
            {
                ulRequestedLocationDesiredAccuracy = LOCATION_DESIRED_ACCURACY_DEFAULT;
            }
        }
        else
        {
            hr = m_spSensorPropertyValues->GetUnsignedIntegerValue(SENSOR_PROPERTY_LOCATION_DESIRED_ACCURACY, &ulRequestedLocationDesiredAccuracy);
        }
    }

    if (SUCCEEDED(hr))
    {
        // '0' to the sensor API means the default value
        if (ulRequestedLocationDesiredAccuracy > LOCATION_DESIRED_ACCURACY_HIGH)
        {
            ulRequestedLocationDesiredAccuracy = LOCATION_DESIRED_ACCURACY_DEFAULT;
        }
        else
        {
            ulRequestedLocationDesiredAccuracy = m_ulLowestClientLocationDesiredAccuracy;
        }

        //pulLocationDesiredAccuracy is the value we request from the device
        if (ulRequestedLocationDesiredAccuracy <= LOCATION_DESIRED_ACCURACY_HIGH)
        {
            // Only use the device if powered on
            if (m_pSensorManager->m_fDeviceActive)
            {
                // Write the location desired accuracy value to the device and then read back what the device has set the report interval to before
                // setting this value for the DeviceProperties and at the API
                Trace(TRACE_LEVEL_INFORMATION, "Requesting Location Desired Accuracy = %i on %s", ulRequestedLocationDesiredAccuracy, m_SensorName);

                ulDeviceLocationDesiredAccuracy = ulRequestedLocationDesiredAccuracy;
                Trace(TRACE_LEVEL_INFORMATION, "Device Location Desired Accuracy = %i on %s", ulDeviceLocationDesiredAccuracy, m_SensorName);

                // check to see this is valid and set it at the api
                ulRequiredLocationDesiredAccuracyAtApi = ulDeviceLocationDesiredAccuracy;

                if (SUCCEEDED(hr))
                {
                    // Set this possible changed value in DeviceProperties and at the API

                    if (SUCCEEDED(hr))
                    {
                        Trace(TRACE_LEVEL_INFORMATION, "Setting API Location Desired Accuracy = %i on %s", ulRequiredLocationDesiredAccuracyAtApi, m_SensorName);
                        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_LOCATION_DESIRED_ACCURACY, ulRequiredLocationDesiredAccuracyAtApi);
                    }
                    else
                    {
                        Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValue = Sensor LocationDesiredAccuracy in %s, hr = %!HRESULT!", m_SensorName, hr);
                    }
                }
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed to set then get %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                }
            }
            else
            {
                Trace(TRACE_LEVEL_WARNING, "%!FUNC! Not updating desired accuracy as device is not active");
            }
        }
    }

    PropVariantClear(&var);

    return hr;
}


HRESULT CSensor::HandleGeolocationRadioStateUpdate()
{
    HRESULT hr = S_OK;

#if (NTDDI_VERSION >= NTDDI_WIN8)
    bool fStateChanged = false;
    /*******************************************************************************
    NOTE:

    It is at this point that the GPS implementer must handle the power setting of
    the GPS device. The state of m_ulRequiredGeolocationRadioState can be either:
    - DRS_SW_RADIO_OFF, in which case the power to the GPS radio should be removed
    - DRS_RADIO_ON, in which case the power to the GPS radio should be restored

    *******************************************************************************/
    if (m_ulCurrentGeolocationRadioState != m_ulRequiredGeolocationRadioState)
    {
        m_ulCurrentGeolocationRadioState = m_ulRequiredGeolocationRadioState;

        size_t cClients = m_pClientMap.GetCount();

        if (DRS_RADIO_ON == m_ulRequiredGeolocationRadioState)
        {
            Trace(TRACE_LEVEL_INFORMATION, "Turning GPS device radio state on %s", m_SensorName);
            m_pSensorManager->SetState(this, SENSOR_STATE_INITIALIZING, &fStateChanged);

            if (cClients > 0)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Calling StopIdle so the device will remain in D0 as long as there are connected clients and the radio is on");
                m_spWdfDevice2->StopIdle(FALSE);
            }
        }
        else if (DRS_SW_RADIO_OFF == m_ulRequiredGeolocationRadioState)
        {
            Trace(TRACE_LEVEL_INFORMATION, "Turning GPS device radio state off %s", m_SensorName);
            m_pSensorManager->SetState(this, SENSOR_STATE_NOT_AVAILABLE, &fStateChanged);

            if (cClients > 0)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Calling ResumeIdle so the device will remain in Dx as long as there are no connected clients or the radio is off");
                m_spWdfDevice2->ResumeIdle();
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_INFORMATION, "Unsupported radio state requested for %s", m_SensorName);
        }

        if (SUCCEEDED(hr))
        {
            Trace(TRACE_LEVEL_INFORMATION, "Setting Radio State = %i on %s", m_ulRequiredGeolocationRadioState, m_SensorName);
            hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_RADIO_STATE, m_ulRequiredGeolocationRadioState);

            if (FAILED(hr))
            {
                Trace(TRACE_LEVEL_ERROR, "Failed to set Radio State %s, hr = %!HRESULT!", m_SensorName, hr);
            }
            else
            {
                // Update the store
                CComPtr<IWDFNamedPropertyStore> spPropStore;
                hr = m_spWdfDevice2->RetrieveDevicePropertyStore(NULL, WdfPropertyStoreCreateIfMissing, &spPropStore, NULL);
                if (SUCCEEDED(hr))
                {
                    PROPVARIANT var;
                    PropVariantInit(&var);
                    var.vt = VT_UI4;
                    var.ulVal = m_ulRequiredGeolocationRadioState;
                    hr = spPropStore->SetNamedValue(PROP_STORE_KEY_RADIO_STATE, &var);
                    PropVariantClear(&var);
                }
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "RetrieveDevicePropertyStore failed for radio state, hr = %!HRESULT!", hr);
                }
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        Trace(TRACE_LEVEL_INFORMATION, "Setting Previous Radio State = %i on %s", m_ulPreviousGeolocationRadioState, m_SensorName);
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_RADIO_STATE_PREVIOUS, m_ulPreviousGeolocationRadioState);

        if (FAILED(hr))
        {
            Trace(TRACE_LEVEL_ERROR, "Failed to set Previous Radio State %s, hr = %!HRESULT!", m_SensorName, hr);
        }
        else
        {
            // Update the store
            CComPtr<IWDFNamedPropertyStore> spPropStore;
            hr = m_spWdfDevice2->RetrieveDevicePropertyStore(NULL, WdfPropertyStoreCreateIfMissing, &spPropStore, NULL);
            if (SUCCEEDED(hr))
            {
                PROPVARIANT var;
                PropVariantInit(&var);
                var.vt = VT_UI4;
                var.ulVal = m_ulPreviousGeolocationRadioState;
                hr = spPropStore->SetNamedValue(PROP_STORE_KEY_PREVIOUS_RADIO_STATE, &var);
                PropVariantClear(&var);
            }
            else
            {
                Trace(TRACE_LEVEL_ERROR, "RetrieveDevicePropertyStore failed for previous radio state, hr = %!HRESULT!", hr);
            }
        }
    }
#endif

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CCompass::GetPropertyValuesForSensorObject
//
//  This method is called to populate property values for the object specified.
//
//  The parameters sent to us are:
//  wszObjectID - the object whose properties are being requested.
//  pKeys - the list of property keys of the properties to request from the object
//  pValues - an IPortableDeviceValues which will contain the property values retreived from the object
//
//  The driver should:
//  Read the specified properties for the specified object and populate pValues with the
//  results.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensor::GetPropertyValuesForSensorObject(
    _In_ LPCWSTR                        wszObjectID,
    _In_ IPortableDeviceKeyCollection*  pKeys,
    _In_ IPortableDeviceValues*         pValues,
    _In_ LPCWSTR                        wszSensorName,
    _In_ GUID                           guidSensorCategory,
    _Out_ BOOL*                         pfError
    )
{
    HRESULT     hr          = S_OK;
    CAtlStringW strObjectID = wszObjectID;
    DWORD       cKeys       = 0;

    if ((wszObjectID == NULL) ||
        (pKeys       == NULL) ||
        (pValues     == NULL))
    {
        hr = E_INVALIDARG;
        return hr;
    }

    *pfError = FALSE;

    hr = pKeys->GetCount(&cKeys);

    if((NULL == m_spSensorPropertyValues) || (NULL == m_spSensorDataFieldValues))
    {
        hr = E_POINTER;
    }

    if (hr == S_OK)
    {
        for (DWORD dwIndex = 0; dwIndex < cKeys; dwIndex++)
        {
            PROPERTYKEY Key = WPD_PROPERTY_NULL;
            hr = pKeys->GetAt(dwIndex, &Key);

            if (hr == S_OK && !IsEqualPropertyKey(Key, WPD_PROPERTY_NULL))
            {
                // Preset the property value to 'error not supported'.  The actual value
                // will replace this value, if read from the device.
                pValues->SetErrorValue(Key, HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED));

                // Set general properties for sensor
                if (IsEqualPropertyKey(Key, WPD_OBJECT_ID))
                {
                    hr = pValues->SetStringValue(WPD_OBJECT_ID, m_SensorID);
                }
                else
                if (IsEqualPropertyKey(Key, WPD_OBJECT_NAME))
                {
                    hr = pValues->SetStringValue(WPD_OBJECT_NAME, wszSensorName);
                }
                else
                if (IsEqualPropertyKey(Key, WPD_OBJECT_PERSISTENT_UNIQUE_ID))
                {
                    hr = pValues->SetStringValue(WPD_OBJECT_PERSISTENT_UNIQUE_ID, m_SensorID);
                }
                else
                if (IsEqualPropertyKey(Key, WPD_OBJECT_PARENT_ID))
                {
                    hr = pValues->SetStringValue(WPD_OBJECT_PARENT_ID, WPD_DEVICE_OBJECT_ID);
                }
                else
                if (IsEqualPropertyKey(Key, WPD_OBJECT_FORMAT))
                {
                    hr = pValues->SetGuidValue(WPD_OBJECT_FORMAT, WPD_OBJECT_FORMAT_UNSPECIFIED);
                }
                else
                if (IsEqualPropertyKey(Key, WPD_OBJECT_CONTENT_TYPE))
                {
                    hr = pValues->SetGuidValue(WPD_OBJECT_CONTENT_TYPE, WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT);
                }
                else
                if (IsEqualPropertyKey(Key, WPD_OBJECT_CAN_DELETE))
                {
                    hr = pValues->SetBoolValue(WPD_OBJECT_CAN_DELETE, FALSE);
                }
                else
                if (IsEqualPropertyKey(Key, WPD_FUNCTIONAL_OBJECT_CATEGORY))
                {
                    hr = pValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, guidSensorCategory);
                }
                else
                {   
                    // Get sensor properties
                    PROPVARIANT value;
                    PropVariantInit(&value);

                    HRESULT hrTemp = GetProperty(Key, &value);

                    if (SUCCEEDED(hrTemp))
                    {
                        pValues->SetValue(Key, &value);
                    }
                    else
                    {
                        // Failed to find the requested property, convey the hr back to the caller
                        pValues->SetErrorValue(Key, hrTemp);
                        *pfError = TRUE;
                    }

                    PropVariantClear(&value);

                }
            }
        }
    }

    return hr;
}

HRESULT CSensor::InitPerDataFieldProperties(_In_ PROPERTYKEY pkDataField)
{
    HRESULT hr = S_OK;

    //------------------Change sensitivity

    if (SUCCEEDED(hr))
    {
        CComPtr<IPortableDeviceValues>  spSensitivityValues;
        PROPERTYKEY datakey;
        DWORD  uDatafieldCount = 0;

        hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_CHANGE_SENSITIVITY, &spSensitivityValues);

        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorDataFields->GetCount(&uDatafieldCount);
        }

        if(SUCCEEDED(hr))
        {
            // Only set the default if the data field is supported
            for (DWORD j = 0; j < uDatafieldCount; j++)
            {
                if (SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->GetAt(j, &datakey);
                }

                if (SUCCEEDED(hr))
                {
                    if (pkDataField == datakey)
                    {
                        hr = spSensitivityValues->SetFloatValue(datakey, m_fltDefaultChangeSensitivity);                           
                    }
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_CHANGE_SENSITIVITY, spSensitivityValues);
        }
    }
    else
    {
        hr = E_POINTER;
    }

    //---------------Range Maximum
    if (SUCCEEDED(hr))
    {
        CComPtr<IPortableDeviceValues>  spMaximumValues;
        PROPERTYKEY datakey;
        DWORD  uDatafieldCount = 0;

        hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MAXIMUM, &spMaximumValues);

        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorDataFields->GetCount(&uDatafieldCount);
        }

        if(SUCCEEDED(hr))
        {
            // Only set the default if the data field is supported
            for (DWORD j = 0; j < uDatafieldCount; j++)
            {
                if (SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->GetAt(j, &datakey);
                }

                if (SUCCEEDED(hr))
                {
                    if (pkDataField == datakey)
                    {
                        hr = spMaximumValues->SetFloatValue(datakey, m_fltDefaultRangeMaximum);                           
                    }
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MAXIMUM, spMaximumValues);
        }
    }

    if (SUCCEEDED(hr))
    {
        CComPtr<IPortableDeviceValues>  spMinimumValues;
        PROPERTYKEY datakey;
        DWORD  uDatafieldCount = 0;

        hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MINIMUM, &spMinimumValues);

        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorDataFields->GetCount(&uDatafieldCount);
        }

        if(SUCCEEDED(hr))
        {
            // Only set the default if the data field is supported
            for (DWORD j = 0; j < uDatafieldCount; j++)
            {
                if (SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->GetAt(j, &datakey);
                }

                if (SUCCEEDED(hr))
                {
                    if (pkDataField == datakey)
                    {
                        hr = spMinimumValues->SetFloatValue(datakey, m_fltDefaultRangeMinimum);                           
                    }
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MINIMUM, spMinimumValues);
        }
    }

    if (SUCCEEDED(hr))
    {
        CComPtr<IPortableDeviceValues>  spAccuracyValues;
        PROPERTYKEY datakey;
        DWORD  uDatafieldCount = 0;

        hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_ACCURACY, &spAccuracyValues);

        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorDataFields->GetCount(&uDatafieldCount);
        }

        if(SUCCEEDED(hr))
        {
            // Only set the default if the data field is supported
            for (DWORD j = 0; j < uDatafieldCount; j++)
            {
                if (SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->GetAt(j, &datakey);
                }

                if (SUCCEEDED(hr))
                {
                    if (pkDataField == datakey)
                    {
                        hr = spAccuracyValues->SetFloatValue(datakey, m_fltDefaultAccuracy);
                    }
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_ACCURACY, spAccuracyValues);
        }
    }

    if (SUCCEEDED(hr))
    {
        CComPtr<IPortableDeviceValues>  spResolutionValues;
        PROPERTYKEY datakey;
        DWORD  uDatafieldCount = 0;

        hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_RESOLUTION, &spResolutionValues);

        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorDataFields->GetCount(&uDatafieldCount);
        }

        if(SUCCEEDED(hr))
        {
            // Only set the default if the data field is supported
            for (DWORD j = 0; j < uDatafieldCount; j++)
            {
                if (SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->GetAt(j, &datakey);
                }

                if (SUCCEEDED(hr))
                {
                    if (pkDataField == datakey)
                    {
                        hr = spResolutionValues->SetFloatValue(datakey, m_fltDefaultResolution);                           
                    }
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RESOLUTION, spResolutionValues);
        }
    }

    return hr;
}

HRESULT CSensor::HandleChangeSensitivityUpdate()
{
    HRESULT hr = S_OK;

    FLOAT  fltDesiredDatafieldSensitivity = 0.0F;
    FLOAT  fltDeviceDatafieldSensitivity = 0.0F;
    FLOAT  fltRequiredDatafieldSensitivityAtApi = 0.0F;

    BOOL fDatafieldSupported = TRUE;

    CComPtr<IPortableDeviceValues>  spSensitivityValues;

    if (TRUE == fDatafieldSupported)
    {
        hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_CHANGE_SENSITIVITY, &spSensitivityValues);

        if (SUCCEEDED(hr))
        {
            DWORD cDatafields = 0;
            PROPERTYKEY pkDatafield;
            
            hr = m_spSupportedSensorDataFields->GetCount(&cDatafields);

            if (SUCCEEDED(hr))
            {
                for(DWORD idx = 1; idx < cDatafields; idx++) //start with 1 to bypass timestamp
                {
                    if (SUCCEEDED(hr))
                    {
                        fltDesiredDatafieldSensitivity = m_fltLowestClientChangeSensitivities[idx];
                        m_spSupportedSensorDataFields->GetAt(idx, &pkDatafield);

                        if (m_pSensorManager->m_fDeviceActive)
                        {
                            // Write the change sensitivity value to the device and then read back what the device has set the report interval to before
                            Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Change Sensitivity for Key = %!GUID!-%i was set = %f", m_SensorName, &pkDatafield.fmtid, pkDatafield.pid, fltDesiredDatafieldSensitivity);
                            fltDeviceDatafieldSensitivity = fltDesiredDatafieldSensitivity;

                            // device responds with value it can be set to
                            fltRequiredDatafieldSensitivityAtApi = fltDeviceDatafieldSensitivity;
                            Trace(TRACE_LEVEL_INFORMATION, "%s set Change Sensitivity for Key = %!GUID!-%i was set = %f", m_SensorName, &pkDatafield.fmtid, pkDatafield.pid, fltDesiredDatafieldSensitivity);

                            if ((SUCCEEDED(hr) && (TRUE == fDatafieldSupported)))
                            {
                                Trace(TRACE_LEVEL_INFORMATION, "%s Change Sensitivity for Key = %!GUID!-%i was set = %f", m_SensorName, &pkDatafield.fmtid, pkDatafield.pid, fltRequiredDatafieldSensitivityAtApi);
                                hr = spSensitivityValues->SetFloatValue(pkDatafield, fltRequiredDatafieldSensitivityAtApi);
                            }
                            else
                            {
                                Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValue = Sensor Sensitivity in %s, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }
                        else
                        {
                            Trace(TRACE_LEVEL_WARNING, "%!FUNC! Not updating change sensitivity as device is not active");
                        }

                    }
                }
            }

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_CHANGE_SENSITIVITY, spSensitivityValues);
            }
        }
    }
                 
    return hr;
}

 HRESULT CSensor::HandleSetReportingAndPowerStates()
 {
    HRESULT hr = S_OK;

    BOOL fReportingStateSupported = TRUE;
    BOOL fPowerStateSupported = TRUE;

    if (m_pSensorManager->m_fDeviceActive)
    {
        if (SUCCEEDED(hr) && (TRUE == fReportingStateSupported))
        {
            if (TRUE == m_fReportingState)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Reporting State = ALL_EVENTS", m_SensorName);
                //turn on event reporting at the device
                Trace(TRACE_LEVEL_INFORMATION, "%s Reporting State = ALL_EVENTS", m_SensorName);
            }
            else
            {
                Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Reporting State = NO_EVENTS", m_SensorName);
                //turn off event reporting at the device
                Trace(TRACE_LEVEL_INFORMATION, "%s Reporting State = NO_EVENTS", m_SensorName);
            }
        }

        if (SUCCEEDED(hr) && (TRUE == fPowerStateSupported))
        {
            if (SENSOR_POWER_STATE_FULL_POWER == m_ulPowerState)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Power State = FULL_POWER", m_SensorName);
                // set power state to full power at the device
                Trace(TRACE_LEVEL_INFORMATION, "%s Power State = FULL_POWER", m_SensorName);
            }
            else if (SENSOR_POWER_STATE_LOW_POWER == m_ulPowerState)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Power State = LOW_POWER", m_SensorName);
                // set power state to low power at the device
                Trace(TRACE_LEVEL_INFORMATION, "%s Power State = LOW_POWER", m_SensorName);
            }
            else if (SENSOR_POWER_STATE_POWER_OFF == m_ulPowerState)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Power State = POWER_OFF", m_SensorName);
                // set power state to power off at the device
                Trace(TRACE_LEVEL_INFORMATION, "%s Power State = POWER_OFF", m_SensorName);
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Unrecognized %s power state, hr = %!HRESULT!", m_SensorName, hr);
            }
        }
    }
    else
    {
        Trace(TRACE_LEVEL_WARNING, "%!FUNC! Not updating reporting or power states as device is not active");
    }

    if (SUCCEEDED(hr))
    {
        hr = S_OK;
    }

    return hr;
}


FLOAT CSensor::GetRangeMaximumValue(
        _In_ FLOAT fltDefaultRangeMaximum,
        _In_ BOOL fSpecificMaximumSupported,
        _In_ FLOAT fltSpecificMaximum,
        _In_ BOOL fBulkMaximumSupported,
        _In_ FLOAT fltBulkMaximum,
        _In_ BOOL fGlobalMaximumSupported,
        _In_ FLOAT fltGlobalMaximum)
{
    FLOAT fltMaximum = fltDefaultRangeMaximum;

    //range check against max usage specified by sensor
    if (TRUE == fSpecificMaximumSupported)
    {
        fltMaximum = fltSpecificMaximum;
    }
    else if (TRUE == fBulkMaximumSupported)
    {
        fltMaximum = fltBulkMaximum;
    }
    else if (TRUE == fGlobalMaximumSupported)
    {
        fltMaximum = fltGlobalMaximum;
    }
    else
    {
        //do nothing
    }

    return fltMaximum;
}


FLOAT CSensor::GetRangeMinimumValue(
        _In_ FLOAT fltDefaultRangeMinimum,
        _In_ BOOL fSpecificMinimumSupported,
        _In_ FLOAT fltSpecificMinimum,
        _In_ BOOL fBulkMinimumSupported,
        _In_ FLOAT fltBulkMinimum,
        _In_ BOOL fGlobalMinimumSupported,
        _In_ FLOAT fltGlobalMinimum)
{
    FLOAT fltMinimum = fltDefaultRangeMinimum;

    //range check against min usage specified by sensor
    if (TRUE == fSpecificMinimumSupported)
    {
        fltMinimum = fltSpecificMinimum;
    }
    else if (TRUE == fBulkMinimumSupported)
    {
        fltMinimum = fltBulkMinimum;
    }
    else if (TRUE == fGlobalMinimumSupported)
    {
        fltMinimum = fltGlobalMinimum;
    }
    else
    {
        //do nothing
    }

    return fltMinimum;
}



VOID CSensor::CheckLongReportIntervalTimer()
{
    if (m_ulLowestClientReportInterval >= LONG_REPORT_INTERVAL_MS)
    {
        StartLongReportIntervalTimer();
    }
    else
    {
        StopLongReportIntervalTimer();
    }
}

VOID CSensor::StartLongReportIntervalTimer()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSectionLongReportInterval);

    if (FALSE == IsThreadpoolTimerSet(m_pLongReportIntervalTimer))
    {
        m_fUsingLongReportIntervalTimer = true;

        m_spWdfDevice2->ResumeIdle();
        Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Calling ResumeIdle");

        // Convert miliseconds to filetime. Filetime is in 100ns units.
        // Negative means relative future time.
        FILETIME fileTimeDue = {0};
        *reinterpret_cast<PLONGLONG>(&fileTimeDue) = -static_cast<LONGLONG>((LONGLONG)10000 * (LONGLONG)(m_ulLowestClientReportInterval - DEVICE_STARTUP_MS));

        SetThreadpoolTimer(
            m_pLongReportIntervalTimer,     // Thread pool timer
            &fileTimeDue,                   // Due time for timer
            0,                              // Period time, 0 is only signaled once
            100                             // Allow delay of up to this many miliseconds on the timer callback for performance savings
            );
    }
}

VOID CSensor::StopLongReportIntervalTimer()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSectionLongReportInterval);

    // Always stop timer
    SetThreadpoolTimer(m_pLongReportIntervalTimer, nullptr, 0, 0);

    if (m_fUsingLongReportIntervalTimer)
    {
        StopIdleForLongReportIntervalTimer();
    }
}

VOID CSensor::StopIdleForLongReportIntervalTimer()
{
    m_spWdfDevice2->StopIdle(FALSE);
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Calling StopIdle");

    m_fUsingLongReportIntervalTimer = false;
}

VOID CALLBACK CSensor::LongReportIntervalTimerCallback(
        _Inout_      PTP_CALLBACK_INSTANCE pInstance,
        _Inout_opt_  PVOID pContext,
        _Inout_      PTP_TIMER pTimer
        )
{
    UNREFERENCED_PARAMETER(pInstance);
    UNREFERENCED_PARAMETER(pTimer);

    CSensor* pThis = static_cast<CSensor*>(pContext);
    if (nullptr != pThis)
    {
        CComCritSecLock<CComAutoCriticalSection> scopeLock(pThis->m_CriticalSectionLongReportInterval);

        Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Hit timer callback");
        pThis->StopIdleForLongReportIntervalTimer();
        SetThreadpoolTimer(pThis->m_pLongReportIntervalTimer, nullptr, 0, 0);
    }
}

