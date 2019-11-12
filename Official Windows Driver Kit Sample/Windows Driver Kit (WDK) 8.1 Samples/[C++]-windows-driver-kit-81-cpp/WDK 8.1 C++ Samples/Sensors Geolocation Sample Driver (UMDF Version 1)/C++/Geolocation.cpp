 /*

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


 Module:
      Geolocation.cpp

 Description:
      Implements the CGeolocation container class

--*/

#include "internal.h"
#include "SensorDdi.h"
#include "SensorManager.h"

#include "Sensor.h"
#include "Geolocation.h"

#include "Geolocation.tmh"

#include <strsafe.h>

// TODO Generate a unique GUID to replace this one
#if USE_GEOLOCATION_SPECIFIC_UNIQUE_ID 
// {A3921B4F-D0D8-4686-94DB-47F59CBDE2C9}
static const GUID SENSOR_ID_GEOLOCATION_SAMPLE = 
{ 0xa3921b4f, 0xd0d8, 0x4686, { 0x94, 0xdb, 0x47, 0xf5, 0x9c, 0xbd, 0xe2, 0xc9 } };
#endif

const PROPERTYKEY g_SupportedGeolocationProperties[] =
{
    SENSOR_PROPERTY_TYPE,                       //[VT_CLSID]
    SENSOR_PROPERTY_STATE,                      //[VT_UI4]
    SENSOR_PROPERTY_MIN_REPORT_INTERVAL,        //[VT_UI4]
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
    SENSOR_PROPERTY_PERSISTENT_UNIQUE_ID,       //[VT_CLSID]
    SENSOR_PROPERTY_MANUFACTURER,               //[VT_LPWSTR]
    SENSOR_PROPERTY_MODEL,                      //[VT_LPWSTR]
    SENSOR_PROPERTY_SERIAL_NUMBER,              //[VT_LPWSTR]
    SENSOR_PROPERTY_FRIENDLY_NAME,              //[VT_LPWSTR]
    SENSOR_PROPERTY_DESCRIPTION,                //[VT_LPWSTR]
    SENSOR_PROPERTY_CONNECTION_TYPE,            //[VT_UI4]
    SENSOR_PROPERTY_CHANGE_SENSITIVITY,         //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_LOCATION_DESIRED_ACCURACY,  //[VT_UI4]
    SENSOR_PROPERTY_RADIO_STATE,                //[VT_UI4]
    SENSOR_PROPERTY_RADIO_STATE_PREVIOUS,       //[VT_UI4]
    WPD_FUNCTIONAL_OBJECT_CATEGORY,             //[VT_CLSID]
};

const PROPERTYKEY g_OptionalSupportedGeolocationProperties[] =
{
    SENSOR_PROPERTY_RANGE_MAXIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RANGE_MINIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_ACCURACY,                   //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RESOLUTION,                 //[VT_UNKNOWN], IPortableDeviceValues
};

const PROPERTYKEY g_SettableGeolocationProperties[] =
{
    SENSOR_PROPERTY_CHANGE_SENSITIVITY,         //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
    SENSOR_PROPERTY_LOCATION_DESIRED_ACCURACY,  //[VT_UI4]
};

const PROPERTYKEY g_SupportedGeolocationDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                 //[VT_FILETIME]
    SENSOR_DATA_TYPE_LATITUDE_DEGREES,          //[VT_R8]
    SENSOR_DATA_TYPE_LONGITUDE_DEGREES,         //[VT_R8]
    SENSOR_DATA_TYPE_ERROR_RADIUS_METERS,       //[VT_R8]
    SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS, //[VT_R8]
    SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS, //[VT_R8]
    SENSOR_DATA_TYPE_SPEED_KNOTS,               //[VT_R8]
    SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES,      //[VT_R8]
};

const PROPERTYKEY g_RequiredGeolocationDataFields[] =
{
    SENSOR_DATA_TYPE_LATITUDE_DEGREES,          //[VT_R8]
    SENSOR_DATA_TYPE_LONGITUDE_DEGREES,         //[VT_R8]
    SENSOR_DATA_TYPE_ERROR_RADIUS_METERS,       //[VT_R8]
    SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS,  //[VT_R8]
    SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS, //[VT_R8]
    SENSOR_DATA_TYPE_SPEED_KNOTS,               //[VT_R8]
    SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES,      //[VT_R8]
};

const PROPERTYKEY g_SupportedGeolocationEvents[] =
{
    SENSOR_EVENT_DATA_UPDATED, 0,
    SENSOR_EVENT_STATE_CHANGED, 0,
};

/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::CGeolocation
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CGeolocation::CGeolocation()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::~CGeolocation
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CGeolocation::~CGeolocation()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::Initialize
//
// Initializes the PROPERTYKEY/PROPVARIANT values for
// the Supported Properties & Supported Data
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeolocation::Initialize(
    _In_ SensorType sensType, 
    _In_ DWORD sensNum, 
    _In_ LPWSTR pwszManufacturer,
    _In_ LPWSTR pwszProduct,
    _In_ LPWSTR pwszSerialNumber,
    _In_ LPWSTR sensorID,
    _In_ IWDFDevice* pWdfDevice,
    _In_ CSensorManager* pSensorManager)
{
    // Check if we are already initialized
    HRESULT hr = (TRUE == IsInitialized()) ? E_UNEXPECTED : S_OK;

    if (SUCCEEDED(hr))
    {
        hr = InitializeSensor(sensType, 
                              sensNum, 
                              pwszManufacturer,
                              pwszProduct,
                              pwszSerialNumber,
                              sensorID,
                              pWdfDevice);

        if (SUCCEEDED(hr))
        {
            // reset the initialization flag
            // since there is specialized
            // initialization still to do
            m_fSensorInitialized = FALSE;
        }
    }

    strcpy_s(m_SensorName, DESCRIPTOR_MAX_LENGTH, SENSOR_GEOLOCATION_TRACE_NAME);

    m_pSensorManager = pSensorManager;

    if (SUCCEEDED(hr))
    {
        hr = InitializeGeolocation(pWdfDevice);
    }

    if (SUCCEEDED(hr))
    {
        m_fSensorInitialized = TRUE;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::InitializeGeolocation
//
// Initializes the Geolocation PropertyKeys and DataFieldKeys 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeolocation::InitializeGeolocation(_In_ IWDFDevice* pWdfDevice)
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    hr = AddGeolocationPropertyKeys();

    if (SUCCEEDED(hr))
    {
        hr = AddGeolocationSettablePropertyKeys();
    }

    if (SUCCEEDED(hr))
    {
        hr = AddGeolocationDataFieldKeys();
    }

    if (SUCCEEDED(hr))
    {
        hr = SetGeolocationDefaultValues(pWdfDevice);
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::AddGeolocationPropertyKeys
//
// Copies the PROPERTYKEYS for Geolocation Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeolocation::AddGeolocationPropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedGeolocationProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_SupportedGeolocationProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if (SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_SupportedGeolocationProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_OptionalSupportedGeolocationProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_OptionalSupportedGeolocationProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of settable properties
        if (SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_OptionalSupportedGeolocationProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::AddGeolocationSettablePropertyKeys
//
// Copies the PROPERTYKEYS for Geolocation Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeolocation::AddGeolocationSettablePropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SettableGeolocationProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_SettableGeolocationProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of settable properties
        if (SUCCEEDED(hr))
        {
            hr = m_spSettableSensorProperties->Add(g_SettableGeolocationProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::AddGeolocationDataFieldKeys
//
// Copies the PROPERTYKEYS for Geolocation Supported DataFields 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeolocation::AddGeolocationDataFieldKeys()
{
    HRESULT hr = S_OK;

    DWORD dwIndex;

    for ( dwIndex = 0 ; (dwIndex < ARRAY_SIZE(g_SupportedGeolocationDataFields)) && SUCCEEDED(hr) ; dwIndex++ )
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetDataField(g_SupportedGeolocationDataFields[dwIndex], &var);

        // Also add the PROPERTYKEY to the list of supported data fields
        if (SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorDataFields->Add(g_SupportedGeolocationDataFields[dwIndex]);
        }

        PropVariantClear(&var);
    }

    for ( dwIndex = 0 ; (dwIndex < ARRAY_SIZE(g_RequiredGeolocationDataFields)) && SUCCEEDED(hr) ; dwIndex++ )
    {
        // initialization of the data field was accomplished in the supported data field for loop

        hr = m_spRequiredDataFields->Add(g_RequiredGeolocationDataFields[dwIndex]);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::SetGeolocationDefaultValues
//
// Fills in default values for most Geolocation Properties and 
// Data Fields.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeolocation::SetGeolocationDefaultValues(_In_ IWDFDevice* pWdfDevice)
{
    HRESULT hr = S_OK;
    WCHAR  tempStr[DESCRIPTOR_MAX_LENGTH];

    if ((NULL == m_spSensorPropertyValues) || (NULL == m_spSensorDataFieldValues) || (NULL == m_pSensorManager))
    {
        hr = E_POINTER;
    }


    // *****************************************************************************************
    // Default values for SENSOR PROPERTIES
    // *****************************************************************************************

    m_ulDefaultCurrentReportInterval = DEFAULT_GEOLOCATION_CURRENT_REPORT_INTERVAL;
    m_ulDefaultMinimumReportInterval = GEOLOCATION_MIN_REPORT_INTERVAL;

    m_fltDefaultChangeSensitivity = DEFAULT_GEOLOCATION_CHANGE_SENSITIVITY;

    m_fltDefaultRangeMaximum = DEFAULT_GEOLOCATION_MAXIMUM;
    m_fltDefaultRangeMinimum = DEFAULT_GEOLOCATION_MINIMUM;
    m_fltDefaultAccuracy = DEFAULT_GEOLOCATION_ACCURACY;
    m_fltDefaultResolution = DEFAULT_GEOLOCATION_RESOLUTION;

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, SENSOR_CATEGORY_LOCATION);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_LOCATION_GPS);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_STATE, SENSOR_STATE_MIN);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_MIN_REPORT_INTERVAL, GEOLOCATION_MIN_REPORT_INTERVAL);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_MANUFACTURER, m_pwszManufacturer);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_MODEL, m_pwszProduct);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_SERIAL_NUMBER, m_pwszSerialNumber);
    }

    if (SUCCEEDED(hr))
    {
        if (m_pSensorManager->m_NumMappedSensors > 1)
        {
            StringCchCopy(tempStr, DESCRIPTOR_MAX_LENGTH, m_pSensorManager->m_wszDeviceName);
            StringCchCat(tempStr, DESCRIPTOR_MAX_LENGTH, L": ");
            StringCchCat(tempStr, DESCRIPTOR_MAX_LENGTH, SENSOR_GEOLOCATION_NAME);
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, tempStr);
        }
        else
        {
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, m_pSensorManager->m_wszDeviceName);
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_GEOLOCATION_DESCRIPTION);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CONNECTION_TYPE, SENSOR_CONNECTION_TYPE_PC_INTEGRATED);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_LOCATION_DESIRED_ACCURACY, LOCATION_DESIRED_ACCURACY_DEFAULT);
    }

#if (NTDDI_VERSION >= NTDDI_WIN8)

    // Radio State values are persisted, pull them from property store
    CComPtr<IWDFNamedPropertyStore> spPropStore;
    hr = pWdfDevice->RetrieveDevicePropertyStore(NULL, WdfPropertyStoreCreateIfMissing, &spPropStore, NULL);
    if (SUCCEEDED(hr))
    {
        PROPVARIANT var;

        hr = spPropStore->GetNamedValue(PROP_STORE_KEY_RADIO_STATE, &var);
        if (SUCCEEDED(hr))
        {
            m_ulCurrentGeolocationRadioState = var.ulVal;
            m_ulRequiredGeolocationRadioState = var.ulVal;
        }
        else
        {
            PropVariantInit(&var);
            var.vt = VT_UI4;
            var.ulVal = m_ulCurrentGeolocationRadioState;
            hr = spPropStore->SetNamedValue(PROP_STORE_KEY_RADIO_STATE, &var);
            PropVariantClear(&var);
        }

        if (SUCCEEDED(hr))
        {
            hr = spPropStore->GetNamedValue(PROP_STORE_KEY_PREVIOUS_RADIO_STATE, &var);
            if (SUCCEEDED(hr))
            {
                m_ulPreviousGeolocationRadioState = var.ulVal;
            }
            else
            {
                PropVariantInit(&var);
                var.vt = VT_UI4;
                var.ulVal = m_ulPreviousGeolocationRadioState;
                hr = spPropStore->SetNamedValue(PROP_STORE_KEY_PREVIOUS_RADIO_STATE, &var);
                PropVariantClear(&var);
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_RADIO_STATE, m_ulCurrentGeolocationRadioState);
    }
    
    if (SUCCEEDED(hr))
    {
        hr =  m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_RADIO_STATE_PREVIOUS, m_ulPreviousGeolocationRadioState);
    }
#else
    UNREFERENCED_PARAMETER(pWdfDevice);
#endif

    // *****************************************************************************************
    // Default values for SENSOR PER-DATAFIELD PROPERTIES
    // *****************************************************************************************

    DWORD uPropertyCount = 0;
    PROPERTYKEY propkey;

    if (SUCCEEDED(hr))
    {
        hr = m_spSupportedSensorProperties->GetCount(&uPropertyCount);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, DEFAULT_GEOLOCATION_CURRENT_REPORT_INTERVAL);
    }
    
    if (SUCCEEDED(hr))
    {
        // Only set the defaults if the property is supported
        for (DWORD i = 0; i < uPropertyCount; i++)
        {
            if (SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorProperties->GetAt(i, &propkey);
            }

            if(SUCCEEDED(hr)  && (SENSOR_PROPERTY_CHANGE_SENSITIVITY == propkey))
            {
                CComPtr<IPortableDeviceValues>  spChangeSensitivityValues;
                PROPERTYKEY datakey;
                DWORD  uDatafieldCount = 0;

                hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(&spChangeSensitivityValues));

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
                            if ((SENSOR_DATA_TYPE_LATITUDE_DEGREES == datakey) ||
                                (SENSOR_DATA_TYPE_LONGITUDE_DEGREES == datakey) ||
                                (SENSOR_DATA_TYPE_ERROR_RADIUS_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_SPEED_KNOTS == datakey) ||
                                (SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES == datakey))
                            {
                                hr = spChangeSensitivityValues->SetFloatValue(datakey, m_fltDefaultChangeSensitivity);                           
                            }
                        }
                    }
                }
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_CHANGE_SENSITIVITY, spChangeSensitivityValues);
                }
            }

            if(SUCCEEDED(hr) && (SENSOR_PROPERTY_RANGE_MAXIMUM == propkey))
            {
                CComPtr<IPortableDeviceValues>  spMaximumValues;
                PROPERTYKEY datakey;
                DWORD  uDatafieldCount = 0;

                hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(&spMaximumValues));

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
                            if ((SENSOR_DATA_TYPE_LATITUDE_DEGREES == datakey) ||
                                (SENSOR_DATA_TYPE_LONGITUDE_DEGREES == datakey) ||
                                (SENSOR_DATA_TYPE_ERROR_RADIUS_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_SPEED_KNOTS == datakey) ||
                                (SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES == datakey))
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

            if(SUCCEEDED(hr) && (SENSOR_PROPERTY_RANGE_MINIMUM == propkey))
            {
                CComPtr<IPortableDeviceValues>  spMinimumValues;
                PROPERTYKEY datakey;
                DWORD  uDatafieldCount = 0;

                hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(&spMinimumValues));

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
                            if ((SENSOR_DATA_TYPE_LATITUDE_DEGREES == datakey) ||
                                (SENSOR_DATA_TYPE_LONGITUDE_DEGREES == datakey) ||
                                (SENSOR_DATA_TYPE_ERROR_RADIUS_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_SPEED_KNOTS == datakey) ||
                                (SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES == datakey))
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

            if(SUCCEEDED(hr) && (SENSOR_PROPERTY_ACCURACY == propkey))
            {
                CComPtr<IPortableDeviceValues>  spAccuracyValues;
                PROPERTYKEY datakey;
                DWORD  uDatafieldCount = 0;

                hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(&spAccuracyValues));

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
                            if ((SENSOR_DATA_TYPE_LATITUDE_DEGREES == datakey) ||
                                (SENSOR_DATA_TYPE_LONGITUDE_DEGREES == datakey) ||
                                (SENSOR_DATA_TYPE_ERROR_RADIUS_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_SPEED_KNOTS == datakey) ||
                                (SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES == datakey))
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

            if(SUCCEEDED(hr) && (SENSOR_PROPERTY_RESOLUTION == propkey))
            {
                CComPtr<IPortableDeviceValues>  spResolutionValues;
                PROPERTYKEY datakey;
                DWORD  uDatafieldCount = 0;

                hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(&spResolutionValues));

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
                            if ((SENSOR_DATA_TYPE_LATITUDE_DEGREES == datakey) ||
                                (SENSOR_DATA_TYPE_LONGITUDE_DEGREES == datakey) ||
                                (SENSOR_DATA_TYPE_ERROR_RADIUS_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS == datakey) ||
                                (SENSOR_DATA_TYPE_SPEED_KNOTS == datakey) ||
                                (SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES == datakey))
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
        }
    }

    // *****************************************************************************************
    // Default values for SENSOR DATA FIELDS
    // *****************************************************************************************
    
    if (SUCCEEDED(hr))
    {
        PROPVARIANT var;

        // Get the current time as FILETIME format
        FILETIME ft;

#if (NTDDI_VERSION >= NTDDI_WIN8)
        GetSystemTimePreciseAsFileTime(&ft); // Use the higher resolution timer when available
#else
        GetSystemTimeAsFileTime(&ft); // API not available, fallback to 16ms resolution timer
#endif

        hr = InitPropVariantFromFileTime(&ft, &var);

        if (SUCCEEDED(hr))
        {
            hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_TIMESTAMP, &var);
        }

        PropVariantClear( &var );
    }

    PROPVARIANT value;
    PropVariantInit( &value );

    value.vt = VT_EMPTY;
    
    if (SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_LATITUDE_DEGREES, &value);
    }
    if (SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_LONGITUDE_DEGREES, &value);
    }
    if (SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ERROR_RADIUS_METERS, &value);
    }
    if (SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS, &value);
    }
    if (SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS, &value);
    }
    if (SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_SPEED_KNOTS, &value);
    }
    if (SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES, &value);
    }

    PropVariantClear( &value );

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::GetPropertyValuesForGeolocationObject
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
HRESULT CGeolocation::GetPropertyValuesForGeolocationObject(
    LPCWSTR                        wszObjectID,
    IPortableDeviceKeyCollection*  pKeys,
    IPortableDeviceValues*         pValues)
{
    HRESULT     hr          = S_OK;
    CAtlStringW strObjectID = wszObjectID;
    DWORD       cKeys       = 0;
    BOOL        fError      = FALSE;

    if ((wszObjectID == NULL) ||
        (pKeys       == NULL) ||
        (pValues     == NULL))
    {
        hr = E_INVALIDARG;
        return hr;
    }

    hr = pKeys->GetCount(&cKeys);

    if ((NULL == m_spSensorPropertyValues) || (NULL == m_spSensorDataFieldValues))
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
                    hr = pValues->SetStringValue(WPD_OBJECT_NAME, SENSOR_GEOLOCATION_NAME);
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
                    hr = pValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, SENSOR_CATEGORY_LOCATION);
                }
                else
                {   
                    // Get Geolocation sensor properties
                    PROPVARIANT value;
                    PropVariantInit(&value);

                    HRESULT hrTemp = m_pSensorManager->m_pSensorList.GetAt(m_pSensorManager->m_pSensorList.FindIndex(m_SensorNum))->GetProperty(Key, &value);
                    if (SUCCEEDED(hrTemp))
                    {
                        pValues->SetValue(Key, &value);
                    }
                    else
                    {
                        // Failed to find the requested property, convey the hr back to the caller
                        pValues->SetErrorValue(Key, hrTemp);
                        fError = TRUE;
                    }

                    PropVariantClear(&value);

                }
            }
        }
    }

    return (FALSE == fError) ? hr : S_FALSE;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::UpdateGeolocationPropertyValues
//
//  This method updates the writable properties of the sensor.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeolocation::UpdateGeolocationPropertyValues()
{
    HRESULT hr = E_UNEXPECTED;

    hr = HandleReportIntervalUpdate();

    if (SUCCEEDED(hr))
    {
        HandleLocationDesiredAccuracyUpdate();
    }

    if (SUCCEEDED(hr))
    {
        HandleGeolocationRadioStateUpdate();
    }

    if (SUCCEEDED(hr))
    {
        HandleChangeSensitivityUpdate();
    }

    if (SUCCEEDED(hr))
    {
        HandleSetReportingAndPowerStates();
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeolocation::UpdateGeolocationDataValues
//
//  This method sends a poll data request to the device.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeolocation::UpdateGeolocationDataFieldValues()
{
    HRESULT hr = S_OK;

    // Only use the device if powered on
    if (m_pSensorManager->m_fDeviceActive)
    {
        bool fContinue = true;
#if (NTDDI_VERSION >= NTDDI_WIN8)
        fContinue = DRS_RADIO_ON == m_ulRequiredGeolocationRadioState;
#endif

        if (fContinue)
        {
            PROPVARIANT value;
            PropVariantInit(&value);

            //NOTE: the datafields are all forced to static value only for the purpose of
            //      illustration. Normally, the value is acquired from a device

            const DOUBLE dblNewLatitude         = 87.65;
            const DOUBLE dblNewLongitude        = 123.4;
            const DOUBLE dblNewErrorRadius      = 4.321;
            const DOUBLE dblNewAltitude         = 1234.5;
            const DOUBLE dblNewAltitudeError    = 9.876;
            const DOUBLE dblNewSpeed            = 43.21;
            const DOUBLE dblNewHeading          = 56.78;

            bool fStateChanged = FALSE;
            bool fHasValidData = FALSE;

            if (SUCCEEDED(hr))
            {
                // Get the current time as FILETIME format
                FILETIME ft;

#if (NTDDI_VERSION >= NTDDI_WIN8)
                GetSystemTimePreciseAsFileTime(&ft); // Use the higher resolution timer when available
#else
                GetSystemTimeAsFileTime(&ft); // API not available, fallback to lower resolution timer
#endif

                hr = InitPropVariantFromFileTime(&ft, &value);
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_TIMESTAMP, &value);

                    PropVariantClear(&value);
                }
            }

            if (SUCCEEDED(hr))
            {
                PropVariantInit(&value);

                FLOAT fltMax = GetRangeMaximumValue(
                    m_fltDefaultRangeMaximum,
                    TRUE,
                    90.0F,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum);

                FLOAT fltMin = GetRangeMinimumValue( 
                    m_fltDefaultRangeMinimum,
                    TRUE,
                    -90.0F,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum);

                if ((dblNewLatitude > fltMax) || (dblNewLatitude < fltMin))
                {
                    value.vt = VT_NULL;
                }
                else
                {
                    value.vt = VT_R8;
                    value.dblVal = dblNewLatitude;
                    fHasValidData = TRUE;
                }

                hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_LATITUDE_DEGREES, &value);

                PropVariantClear(&value);
            }
            if (SUCCEEDED(hr))
            {
                PropVariantInit(&value);

                FLOAT fltMax = GetRangeMaximumValue(
                    m_fltDefaultRangeMaximum,
                    TRUE,
                    180.0F,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum);

                FLOAT fltMin = GetRangeMinimumValue( 
                    m_fltDefaultRangeMinimum,
                    TRUE,
                    -180.0F,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum);

                if ((dblNewLongitude > fltMax) || (dblNewLongitude < fltMin))
                {
                    value.vt = VT_NULL;
                }
                else
                {
                    value.vt = VT_R8;
                    value.dblVal = dblNewLongitude;
                    fHasValidData = TRUE;
                }

                hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_LONGITUDE_DEGREES, &value);

                PropVariantClear(&value);
            }

            if (SUCCEEDED(hr))
            {
                PropVariantInit(&value);

                FLOAT fltMax = GetRangeMaximumValue(
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum);

                FLOAT fltMin = GetRangeMinimumValue( 
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum);

                if ((dblNewErrorRadius > fltMax) || (dblNewErrorRadius < fltMin))
                {
                    value.vt = VT_NULL;
                }
                else
                {
                    value.vt = VT_R8;
                    value.dblVal = dblNewErrorRadius;
                    fHasValidData = TRUE;
                }

                hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ERROR_RADIUS_METERS, &value);

                PropVariantClear(&value);
            }

            if (SUCCEEDED(hr))
            {
                PropVariantInit(&value);

                FLOAT fltMax = GetRangeMaximumValue(
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum);

                FLOAT fltMin = GetRangeMinimumValue( 
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum);

                if ((dblNewAltitude > fltMax) || (dblNewAltitude < fltMin))
                {
                    value.vt = VT_NULL;
                }
                else
                {
                    value.vt = VT_R8;
                    value.dblVal = dblNewAltitude;
                    fHasValidData = TRUE;
                }

                hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS, &value);

                PropVariantClear(&value);
            }

            if (SUCCEEDED(hr))
            {
                PropVariantInit(&value);

                FLOAT fltMax = GetRangeMaximumValue(
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum);

                FLOAT fltMin = GetRangeMinimumValue( 
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum);

                if ((dblNewAltitudeError > fltMax) || (dblNewAltitudeError < fltMin))
                {
                    value.vt = VT_NULL;
                }
                else
                {
                    value.vt = VT_R8;
                    value.dblVal = dblNewAltitudeError;
                    fHasValidData = TRUE;
                }

                hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS, &value);

                PropVariantClear(&value);
            }

            if (SUCCEEDED(hr))
            {
                PropVariantInit(&value);

                FLOAT fltMax = GetRangeMaximumValue(
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum);

                FLOAT fltMin = GetRangeMinimumValue( 
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum);

                if ((dblNewSpeed > fltMax) || (dblNewSpeed < fltMin))
                {
                    value.vt = VT_NULL;
                }
                else
                {
                    value.vt = VT_R8;
                    value.dblVal = dblNewSpeed;
                    fHasValidData = TRUE;
                }

                hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_SPEED_KNOTS, &value);

                PropVariantClear(&value);
            }

            if (SUCCEEDED(hr))
            {
                PropVariantInit(&value);

                FLOAT fltMax = GetRangeMaximumValue(
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum,
                    FALSE,
                    m_fltDefaultRangeMaximum);

                FLOAT fltMin = GetRangeMinimumValue( 
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum,
                    FALSE,
                    m_fltDefaultRangeMinimum);

                if ((dblNewHeading > fltMax) || (dblNewHeading < fltMin))
                {
                    value.vt = VT_NULL;
                }
                else
                {
                    value.vt = VT_R8;
                    value.dblVal = dblNewHeading;
                    fHasValidData = TRUE;
                }

                hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES, &value);

                PropVariantClear(&value);
            }

            if (SUCCEEDED(hr))
            {
                m_fInitialDataReceived = TRUE;
            }

            if (TRUE == fHasValidData)
            {
                // NOTE: the sensor state is set to ready so that the values above are visible to clients

                SetState(SENSOR_STATE_READY, &fStateChanged);
            }
            else
            {
                // NOTE: in the event the Geolocation is a device that will actively continue to find location, 
                // like a GPS that has lost the satelite fix, the sensor state must be set to
                // SENSOR_STATE_INITIALIZING and remain in that state until the device once again has a valid
                // location, in which case it can be set back to SENSOR_STATE_READY

                SetState(SENSOR_STATE_INITIALIZING, &fStateChanged);

                // Any sensor that has given up finding location for now will set this state to SENSOR_STATE_NO_DATA
                // and remain in that state until the sensor once again has valid data

                //SetState(SENSOR_STATE_NO_DATA, &fStateChanged);
            }

            // Call RaiseDataEvent to send a data notification to all clients.
            // Do not call RaiseDataEvent from this function as this function
            // is called as a result of a client data request IO.
            // Call RaiseDataEvent when the location device updates data.
            //RaiseDataEvent();
        }
    }
    else
    {
        Trace(TRACE_LEVEL_WARNING, "%!FUNC! Not updating data as device is not active");
    }

    return hr;
}

#if USE_GEOLOCATION_SPECIFIC_UNIQUE_ID 
// Sets the persistent unique ID property
// this overrides the CSensor base class version
HRESULT CGeolocation::SetUniqueID(_In_ IWDFDevice* pWdfDevice)
{
    HRESULT hr = S_OK;

    UNREFERENCED_PARAMETER(pWdfDevice);

    hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_PERSISTENT_UNIQUE_ID, SENSOR_ID_GEOLOCATION_SAMPLE);

    return hr;
}
#endif

HRESULT CGeolocation::SetState(_In_ SensorState newState, _Out_ bool* pfStateChanged)
{    
    Trace(TRACE_LEVEL_VERBOSE, "%!FUNC! Entry");

    HRESULT hr = m_pSensorManager->SetState(this, newState, pfStateChanged);

    if (SUCCEEDED(hr))
    {
        if (true == *pfStateChanged)
        {
            if (SENSOR_STATE_READY != newState)
            {
                PROPVARIANT value;
                PropVariantInit( &value );

                value.vt = VT_EMPTY;
    
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_LATITUDE_DEGREES, &value);
                }
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_LONGITUDE_DEGREES, &value);
                }
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ERROR_RADIUS_METERS, &value);
                }
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS, &value);
                }
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS, &value);
                }
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_SPEED_KNOTS, &value);
                }
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES, &value);
                }

                PropVariantClear( &value );
            }
        }
    }

    return hr;
}
