 /*

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


 Module:
      AmbientLight.cpp

 Description:
      Implements the CAmbientLight container class

--*/

#include "internal.h"
#include "SensorDdi.h"
#include "SensorManager.h"

#include "Sensor.h"
#include "AmbientLight.h"

#include "AmbientLight.tmh"


const PROPERTYKEY g_RequiredSupportedAmbientLightProperties[] =
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
    WPD_FUNCTIONAL_OBJECT_CATEGORY,             //[VT_CLSID]
};

const PROPERTYKEY g_OptionalSupportedAmbientLightProperties[] =
{
    SENSOR_PROPERTY_RANGE_MAXIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RANGE_MINIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_ACCURACY,                   //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RESOLUTION,                 //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_LIGHT_RESPONSE_CURVE,       //[VT_VECTOR|VT_UI1] 
};

const PROPERTYKEY g_RequiredSettableAmbientLightProperties[] =
{
    SENSOR_PROPERTY_CHANGE_SENSITIVITY,         //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
};

const PROPERTYKEY g_SupportedAmbientLightDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,
    SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX,           //[VT_R4]
};

const PROPERTYKEY g_OptionalAmbientLightDataFields[] =
{
    SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN,  //[VT_R4]
    SENSOR_DATA_TYPE_LIGHT_CHROMACITY,          //[VT_VECTOR|VT_UI1]
};

const PROPERTYKEY g_SupportedAmbientLightEvents[] =
{
    SENSOR_EVENT_DATA_UPDATED, 0,
    SENSOR_EVENT_STATE_CHANGED, 0,
};

/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::CAmbientLight
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CAmbientLight::CAmbientLight()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::~CAmbientLight
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CAmbientLight::~CAmbientLight()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::Initialize
//
// Initializes the PROPERTYKEY/PROPVARIANT values for
// the Supported Properties & Supported Data
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAmbientLight::Initialize(
    _In_ SensorType sensType,
    _In_ ULONG sensUsage,
    _In_ USHORT sensLinkCollection,
    _In_ DWORD sensNum, 
    _In_ LPWSTR pwszManufacturer,
    _In_ LPWSTR pwszProduct,
    _In_ LPWSTR pwszSerialNumber,
    _In_ LPWSTR sensorID,
    _In_ CSensorManager* pSensorManager
    )
{
    // Check if we are already initialized
    HRESULT hr = (TRUE == IsInitialized()) ? E_UNEXPECTED : S_OK;

    if(SUCCEEDED(hr))
    {
        m_pSensorManager = pSensorManager;

        InitializeSensor(sensType, 
                                sensUsage,
                                sensLinkCollection,
                                sensNum, 
                                pwszManufacturer,
                                pwszProduct,
                                pwszSerialNumber,
                                sensorID);
    }

    strcpy_s(m_SensorName, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_AMBIENTLIGHT_TRACE_NAME);

    if(SUCCEEDED(hr))
    {
        hr = InitializeAmbientLight();
    }

    if(SUCCEEDED(hr))
    {
        m_fSensorInitialized = TRUE;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::InitializeAmbientLight
//
// Initializes the AmbientLight PropertyKeys and DataFieldKeys 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAmbientLight::InitializeAmbientLight( )
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    ZeroMemory(&m_DeviceProperties, sizeof(m_DeviceProperties));

    hr = AddAmbientLightPropertyKeys();

    if (SUCCEEDED(hr))
    {
        hr = AddAmbientLightSettablePropertyKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = AddAmbientLightDataFieldKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = SetAmbientLightDefaultValues();
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::AddAmbientLightPropertyKeys
//
// Copies the PROPERTYKEYS for AmbientLight Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAmbientLight::AddAmbientLightPropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSupportedAmbientLightProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSupportedAmbientLightProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_RequiredSupportedAmbientLightProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_OptionalSupportedAmbientLightProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_OptionalSupportedAmbientLightProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_OptionalSupportedAmbientLightProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::AddAmbientLightSettablePropertyKeys
//
// Copies the PROPERTYKEYS for AmbientLight Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAmbientLight::AddAmbientLightSettablePropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSettableAmbientLightProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSettableAmbientLightProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSettableSensorProperties->Add(g_RequiredSettableAmbientLightProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::AddAmbientLightDataFieldKeys
//
// Copies the PROPERTYKEYS for AmbientLight Supported DataFields 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAmbientLight::AddAmbientLightDataFieldKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedAmbientLightDataFields); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetDataField(g_SupportedAmbientLightDataFields[dwIndex], &var);

        // Also add the PROPERTYKEY to the list of supported data fields
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorDataFields->Add(g_SupportedAmbientLightDataFields[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::SetAmbientLightDefaultValues
//
// Fills in default values for most AmbientLight Properties and 
// Data Fields.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAmbientLight::SetAmbientLightDefaultValues()
{
    HRESULT hr = S_OK;
    WCHAR  tempStr[HID_USB_DESCRIPTOR_MAX_LENGTH];

    if((NULL == m_spSensorPropertyValues) || (NULL == m_spSensorDataFieldValues))
    {
        hr = E_POINTER;
    }

    // *****************************************************************************************
    // Default values for SENSOR PROPERTIES
    // *****************************************************************************************

    m_ulDefaultCurrentReportInterval = DEFAULT_AMBIENTLIGHT_CURRENT_REPORT_INTERVAL;
    m_ulDefaultMinimumReportInterval = DEFAULT_AMBIENTLIGHT_MIN_REPORT_INTERVAL;

    m_fltDefaultChangeSensitivity = DEFAULT_AMBIENTLIGHT_ILLUMINANCE_SENSITIVITY;
    m_fltDefaultColorTempSensitivity = DEFAULT_AMBIENTLIGHT_COLORTEMP_SENSITIVITY;
    m_fltDefaultChromaticitySensitivity = DEFAULT_AMBIENTLIGHT_CHROMATICITY_SENSITIVITY;

    m_fltDefaultRangeMaximum = DEFAULT_AMBIENTLIGHT_ILLUMINANCE_MAXIMUM;
    m_fltDefaultColorTempMaximum = DEFAULT_AMBIENTLIGHT_COLORTEMP_MAXIMUM;
    m_fltDefaultChromaticityMaximum = DEFAULT_AMBIENTLIGHT_CHROMATICITY_MAXIMUM;

    m_fltDefaultRangeMinimum = DEFAULT_AMBIENTLIGHT_ILLUMINANCE_MINIMUM;
    m_fltDefaultColorTempMinimum = DEFAULT_AMBIENTLIGHT_COLORTEMP_MINIMUM;
    m_fltDefaultChromaticityMinimum = DEFAULT_AMBIENTLIGHT_CHROMATICITY_MINIMUM;

    m_fltDefaultAccuracy = DEFAULT_AMBIENTLIGHT_ILLUMINANCE_ACCURACY;
    m_fltDefaultColorTempAccuracy = DEFAULT_AMBIENTLIGHT_COLORTEMP_ACCURACY;
    m_fltDefaultChromaticityAccuracy = DEFAULT_AMBIENTLIGHT_CHROMATICITY_ACCURACY;

    m_fltDefaultResolution = DEFAULT_AMBIENTLIGHT_ILLUMINANCE_RESOLUTION;
    m_fltDefaultColorTempResolution = DEFAULT_AMBIENTLIGHT_COLORTEMP_RESOLUTION;
    m_fltDefaultChromaticityResolution = DEFAULT_AMBIENTLIGHT_CHROMATICITY_RESOLUTION;

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, SENSOR_CATEGORY_LIGHT);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_AMBIENT_LIGHT);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_STATE, SENSOR_STATE_NO_DATA);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_MIN_REPORT_INTERVAL, m_ulDefaultMinimumReportInterval);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_MANUFACTURER, m_pwszManufacturer);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_MODEL, m_pwszProduct);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_SERIAL_NUMBER, m_pwszSerialNumber);
    }

    if(SUCCEEDED(hr))
    {
        if (m_pSensorManager->m_NumMappedSensors > 1)
        {
            wcscpy_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, m_pSensorManager->m_wszDeviceName);
            wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, L": ");
            wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_AMBIENTLIGHT_NAME);
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, tempStr);
        }
        else
        {
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, m_pSensorManager->m_wszDeviceName);
        }
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_AMBIENTLIGHT_DESCRIPTION);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CONNECTION_TYPE, SENSOR_CONNECTION_TYPE_PC_ATTACHED);
    }

    // *****************************************************************************************
    // Default values for SENSOR SETTABLE PROPERTIES
    // *****************************************************************************************

    DWORD uPropertyCount = 0;
    PROPERTYKEY propkey;

    if (SUCCEEDED(hr))
    {
        hr = m_spSupportedSensorProperties->GetCount(&uPropertyCount);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, m_ulDefaultCurrentReportInterval);
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

                        if (SUCCEEDED(hr) && (SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX == datakey))
                        {
                            hr = spChangeSensitivityValues->SetFloatValue(SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX, m_fltDefaultChangeSensitivity);
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

                        if (SUCCEEDED(hr) && (SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX == datakey))
                        {
                            hr = spMaximumValues->SetFloatValue(SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX, m_fltDefaultRangeMaximum);
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

                        if (SUCCEEDED(hr) && (SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX == datakey))
                        {
                            hr = spMinimumValues->SetFloatValue(SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX, m_fltDefaultRangeMinimum);
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

                        if (SUCCEEDED(hr) && (SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX == datakey))
                        {
                            hr = spAccuracyValues->SetFloatValue(SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX, m_fltDefaultAccuracy);
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

                        if (SUCCEEDED(hr) && (SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX == datakey))
                        {
                            hr = spResolutionValues->SetFloatValue(SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX, m_fltDefaultResolution);
                        }
                    }
                }
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RESOLUTION, spResolutionValues);
                }
            }

            if(SUCCEEDED(hr) && (SENSOR_PROPERTY_LIGHT_RESPONSE_CURVE == propkey))
            {
                PROPVARIANT value;
                PropVariantInit( &value );
                value.vt = VT_EMPTY;

                hr = InitPropVariantFromBuffer(DEFAULT_RESPONSE_CURVE_X_Y_PAIRS, sizeof(DWORD)*AMBIENTLIGHT_MAX_RESPONSE_CURVE_XY_PAIRS*2, &value);

                if (SUCCEEDED(hr))
                {
                    hr = SetProperty(SENSOR_PROPERTY_LIGHT_RESPONSE_CURVE, &value, nullptr);
                }
                else
                {
                    value.vt = VT_EMPTY;
                }

                PropVariantClear( &value );
            }
        }
    }
    // *****************************************************************************************
    // Default values for SENSOR DATA FIELDS
    // *****************************************************************************************
    
    if(SUCCEEDED(hr))
    {
        PROPVARIANT var;
        PropVariantInit( &var );

        //Get the current time in  SYSTEMTIME format
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
            hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_TIMESTAMP, &var);
        }

        PropVariantClear( &var );
    }

    PROPVARIANT value;
    PropVariantInit( &value );
    value.vt = VT_EMPTY;

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_LIGHT_CHROMACITY, &value);
    }

    PropVariantClear( &value );

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::GetPropertyValuesForAmbientLightObject
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
HRESULT CAmbientLight::GetPropertyValuesForAmbientLightObject(
    LPCWSTR                        wszObjectID,
    IPortableDeviceKeyCollection*  pKeys,
    IPortableDeviceValues*         pValues)
{
    HRESULT     hr          = S_OK;
    BOOL        fError      = FALSE;

    if ((wszObjectID == NULL) ||
        (pKeys       == NULL) ||
        (pValues     == NULL))
    {
        hr = E_INVALIDARG;
        return hr;
    }

    hr = GetPropertyValuesForSensorObject(wszObjectID, pKeys, pValues, SENSOR_AMBIENTLIGHT_NAME, SENSOR_CATEGORY_LIGHT, &fError);

    return (FALSE == fError) ? hr : S_FALSE;
}

/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::ProcessAmbientLightAsyncRead
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAmbientLight::ProcessAmbientLightAsyncRead( BYTE* buffer, ULONG length )
{
    HRESULT             hr = S_OK;

    if ((NULL == buffer) || (length == 0))
    {
        hr = E_UNEXPECTED;
    }

    if (SUCCEEDED(hr) && (m_fSensorInitialized))
    {
        if (length >= m_pSensorManager->m_HidCaps.InputReportByteLength)
        {
            //Handle input report

            HIDP_REPORT_TYPE ReportType = HidP_Input;
            USAGE UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
            USHORT LinkCollection = 0;
            UCHAR reportID = 0;
            USAGE Usage = 0;
            USHORT UsageDataModifier = 0;
            LONG   UsageValue = 0;
            ULONG  UsageUValue = 0;
            USHORT ReportCount = 0;
            CHAR   UsageArray[HID_FEATURE_REPORT_STRING_MAX_LENGTH*2] = {0};

            ULONG numUsages = MAX_NUM_HID_USAGES;
            USAGE UsageList[MAX_NUM_HID_USAGES] = {0};

            if (m_pSensorManager->m_NumMappedSensors > 1)
            {
                reportID = (UCHAR)(m_StartingInputReportID + m_SensorNum); 
                LinkCollection = m_SensorLinkCollection; 
            }

            if(SUCCEEDED(hr))
            {
                hr = SetTimeStamp();
            }

            if (SUCCEEDED(hr))
            {
                USHORT numNodes = m_pSensorManager->m_HidCaps.NumberInputValueCaps;
                PROPVARIANT value;
                PropVariantInit( &value );

                LONG   UnitsExp = 0;
                ULONG  BitSize = 0;
                ULONG  Units = 0;

                USHORT  sensorState = SENSOR_STATE_NOT_AVAILABLE;
                USHORT  eventType = SENSOR_EVENT_TYPE_UNKNOWN;
                FLOAT   fltIntensity = 0, fltColorTemp = 0, fltChromaticityX = 0, fltChromaticityY = 0;
                BOOL    fChromaticityXAvailable = FALSE, fChromaticityYAvailable = FALSE;
                FLOAT   fltChromaticityBuffer[2] = {0.0};

                hr = HandleGetHidInputSelectors(
                        &m_DeviceProperties.fSensorStateSelectorSupported,
                        &sensorState,
                        &m_DeviceProperties.fEventTypeSelectorSupported,
                        &eventType,
                        ReportType, 
                        UsagePage, 
                        LinkCollection, 
                        UsageList, 
                        &numUsages,
                        buffer, 
                        length);

                for(ULONG idx = 0; idx < numNodes; idx++)
                {
                    if (reportID == m_InputValueCapsNodes[idx].ReportID)
                    {
                        UsagePage = m_InputValueCapsNodes[idx].UsagePage;
                        Usage = m_InputValueCapsNodes[idx].NotRange.Usage;
                        UsageDataModifier = (USHORT)Usage & 0xF000; //extract the data modifier
                        ReportCount = m_InputValueCapsNodes[idx].ReportCount;
                        Units = m_InputValueCapsNodes[idx].Units;
                        UnitsExp = m_InputValueCapsNodes[idx].UnitsExp;
                        BitSize = m_InputValueCapsNodes[idx].BitSize;
                        TranslateHidUnitsExp(&UnitsExp);

                        UsageUValue = 0;

                        if (ReportCount > 1)
                        {
                            ZeroMemory(UsageArray, HID_FEATURE_REPORT_STRING_MAX_LENGTH*2);
                            hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, UsageArray, HID_FEATURE_REPORT_STRING_MAX_LENGTH*2, m_pSensorManager->m_pPreparsedData, (PCHAR)buffer, length);
                        }
                        else if (ReportCount == 1)
                        {
                            UsageUValue = 0;
                            hr = HidP_GetUsageValue(ReportType, UsagePage, LinkCollection, Usage, &UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)buffer, length);
                        }
                        else
                        {
                            hr = E_UNEXPECTED;
                            Trace(TRACE_LEVEL_ERROR, "Input report count == 0, hr = %!HRESULT!", hr);
                        }

                        if (SUCCEEDED(hr))
                        {
                            BOOL fInputHandled = FALSE;

                            Usage = Usage & 0x0FFF; //remove the data modifier
                            UsageValue = ExtractValueFromUsageUValue(m_InputValueCapsNodes[idx].LogicalMin, BitSize, UsageUValue);

                            hr = HandleCommonInputValues(
                                    idx,
                                    &m_DeviceProperties.fSensorStateSupported,
                                    &sensorState,
                                    &m_DeviceProperties.fEventTypeSupported,
                                    &eventType,
                                    Usage,
                                    UsageValue,
                                    UsageUValue,
                                    UnitsExp,
                                    UsageArray,
                                    &fInputHandled);

                            if (SUCCEEDED(hr) && (FALSE == fInputHandled))
                            {
                                switch(Usage)
                                {
                                case HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        m_DeviceProperties.fAmbientLightIlluminanceSupported = TRUE;
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            fltIntensity = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fAmbientLightIlluminanceMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightIlluminanceMaximum,
                                                            m_DeviceProperties.fAmbientLightIlluminanceMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightIlluminanceMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fAmbientLightIlluminanceMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightIlluminanceMinimum,
                                                            m_DeviceProperties.fAmbientLightMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((fltIntensity > fltMax) || (fltIntensity < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R4;
                                                value.fltVal = fltIntensity;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX, &value);
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fAmbientLightColorTempSupported)
                                        {
                                            m_DeviceProperties.fAmbientLightColorTempSupported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN, &value);
                                            }
                                        }
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            fltColorTemp = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fAmbientLightColorTempMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightColorTempMaximum,
                                                            m_DeviceProperties.fAmbientLightColorTempMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightColorTempMaximum,
                                                            m_DeviceProperties.fAmbientLightColorTempMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightColorTempMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fAmbientLightColorTempMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightColorTempMinimum,
                                                            m_DeviceProperties.fAmbientLightColorTempMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightColorTempMinimum,
                                                            m_DeviceProperties.fAmbientLightColorTempMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightColorTempMinimum);

                                            if ((fltColorTemp > fltMax) || (fltColorTemp < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R4;
                                                value.fltVal = fltColorTemp;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY_X:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fAmbientLightChromaticityXSupported)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityXSupported = TRUE;

                                            if (FALSE == m_DeviceProperties.fAmbientLightChromaticityYSupported)
                                            {
                                                hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_LIGHT_CHROMACITY);

                                                if (SUCCEEDED(hr))
                                                {
                                                    hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_LIGHT_CHROMACITY);
                                                }

                                                if (SUCCEEDED(hr))
                                                {
                                                    value.vt = VT_EMPTY;
                                                    hr = SetDataField(SENSOR_DATA_TYPE_LIGHT_CHROMACITY, &value);
                                                }
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            fltChromaticityX = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                            fChromaticityXAvailable = TRUE;
                                        }
                                        if (fChromaticityXAvailable && fChromaticityYAvailable)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticitySupported = TRUE;

                                            FLOAT fltMaxX = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fAmbientLightChromaticityXMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityXMaximum,
                                                            m_DeviceProperties.fAmbientLightChromaticityMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityMaximum,
                                                            m_DeviceProperties.fAmbientLightChromaticityMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityMaximum);

                                            FLOAT fltMinX = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fAmbientLightChromaticityXMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityXMaximum,
                                                            m_DeviceProperties.fAmbientLightChromaticityMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityMinimum,
                                                            m_DeviceProperties.fAmbientLightChromaticityMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityMinimum);

                                            FLOAT fltMaxY = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fAmbientLightChromaticityYMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityYMaximum,
                                                            m_DeviceProperties.fAmbientLightChromaticityMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityMaximum,
                                                            m_DeviceProperties.fAmbientLightChromaticityMaximumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityMaximum);

                                            FLOAT fltMinY = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fAmbientLightChromaticityYMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityYMaximum,
                                                            m_DeviceProperties.fAmbientLightChromaticityMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityMinimum,
                                                            m_DeviceProperties.fAmbientLightChromaticityMinimumSupported,
                                                            m_DeviceProperties.fltAmbientLightChromaticityMinimum);

                                            fltChromaticityBuffer[0] = fltChromaticityX;
                                            fltChromaticityBuffer[1] = fltChromaticityY;

                                            if ((fltChromaticityX > fltMaxX) || (fltChromaticityX < fltMinX) || (fltChromaticityY > fltMaxY) || (fltChromaticityY < fltMinY))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                hr = InitPropVariantFromBuffer(fltChromaticityBuffer, sizeof(fltChromaticityBuffer), &value);
                                            }
                                            if (SUCCEEDED(hr))
                                            {
                                                hr = SetDataField(SENSOR_DATA_TYPE_LIGHT_CHROMACITY, &value);
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY_Y:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fAmbientLightChromaticityYSupported)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityYSupported = TRUE;

                                            if (FALSE == m_DeviceProperties.fAmbientLightChromaticityXSupported)
                                            {
                                                hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_LIGHT_CHROMACITY);

                                                if (SUCCEEDED(hr))
                                                {
                                                    hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_LIGHT_CHROMACITY);
                                                }

                                                if (SUCCEEDED(hr))
                                                {
                                                    value.vt = VT_EMPTY;
                                                    hr = SetDataField(SENSOR_DATA_TYPE_LIGHT_CHROMACITY, &value);
                                                }
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            fltChromaticityY = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                            fChromaticityYAvailable = TRUE;
                                        }
                                        if (fChromaticityXAvailable && fChromaticityYAvailable)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticitySupported = TRUE;
                                            fltChromaticityBuffer[0] = fltChromaticityX;
                                            fltChromaticityBuffer[1] = fltChromaticityY;
                                            hr = InitPropVariantFromBuffer(fltChromaticityBuffer, sizeof(fltChromaticityBuffer), &value);
                                            if (SUCCEEDED(hr))
                                            {
                                                hr = SetDataField(SENSOR_DATA_TYPE_LIGHT_CHROMACITY, &value);
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                default:
                                    hr = HandleDefinedDynamicDatafield(Usage, ReportCount, UnitsExp, UsageValue, UsageArray);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);
                        }
                    }
                }

                PropVariantClear( &value );
            }

            if( SUCCEEDED(hr))
            {
                RaiseDataEvent();

                if (FALSE == m_fInformedCommonInputReportConditions)
                {
                    ReportCommonInputReportDescriptorConditions(
                        m_DeviceProperties.fSensorStateSelectorSupported,
                        m_DeviceProperties.fEventTypeSelectorSupported,
                        m_DeviceProperties.fSensorStateSupported,
                        m_DeviceProperties.fEventTypeSupported 
                    );

                    //input conditions specific to this sensor

                }
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "%s Input report is incorrect length, is = %i, should be = %i, hr = %!HRESULT!", m_SensorName, length,  m_pSensorManager->m_HidCaps.InputReportByteLength, hr);

            Trace(TRACE_LEVEL_ERROR, "%s Input report failure count = %i, content = 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", 
                m_SensorName, ++m_InputReportFailureCount,
                buffer[0], buffer[1], buffer[2], buffer[3],
                buffer[4], buffer[5], buffer[6], buffer[7],
                buffer[8], buffer[9], buffer[10], buffer[11],
                buffer[12], buffer[13], buffer[14], buffer[15], 
                buffer[16], buffer[17], buffer[18], buffer[19],
                buffer[20], buffer[21], buffer[22], buffer[23],
                buffer[24], buffer[25], buffer[26], buffer[27],
                buffer[28], buffer[29]); 
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CAmbientLight::UpdateAmbientLightPropertyValues
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CAmbientLight::UpdateAmbientLightPropertyValues(BYTE* pFeatureReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported)
{
    UNREFERENCED_PARAMETER(fSettableOnly);

    DWORD   cValues = 0;
    HRESULT hr = m_spSupportedSensorProperties->GetCount(&cValues);
    UCHAR   reportID = 0;
    ULONG   uReportSize = m_pSensorManager->m_HidCaps.FeatureReportByteLength;
    
    HIDP_REPORT_TYPE ReportType = HidP_Feature;
    USAGE UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
    USHORT LinkCollection = 0;
    LONG  UsageValue = 0;
    ULONG UsageUValue = 0;
    CHAR  UsageArray[HID_FEATURE_REPORT_STRING_MAX_LENGTH*2] = {0};

    ULONG numUsages = MAX_NUM_HID_USAGES;
    USAGE UsageList[MAX_NUM_HID_USAGES] = {0};

    if (m_pSensorManager->m_NumMappedSensors > 1)
    {
        reportID = (UCHAR)(m_StartingFeatureReportID + m_SensorNum); 
        LinkCollection = m_SensorLinkCollection; 
    }
    
    //Get the properties from the device using Feature report
    //Synchronously get the current device configuration
    *pfFeatureReportSupported = m_fFeatureReportSupported;

    if(SUCCEEDED(hr) && (TRUE == m_fFeatureReportSupported))
    {
        hr = GetSensorPropertiesFromDevice(reportID, pFeatureReport, uReportSize);
    }

    if (TRUE == m_fFeatureReportSupported)
    {
        //Extract the properties from the report buffer
        if(SUCCEEDED(hr))
        {
            USHORT ReportCount = 0;
            USAGE  Usage = 0;
            USHORT UsageDataModifier = 0;
            USHORT usResponseCurveX = 0, usResponseCurveY = 0;

            DWORD  dwResponseCurveX = 0, dwResponseCurveY = 0;
            DWORD  dwResponseCurveXYPairs[AMBIENTLIGHT_MAX_RESPONSE_CURVE_XY_PAIRS*2] = {0};
            //The following sets default values if no values are retrieved from device
            USHORT usResponseCurveXYPairs[AMBIENTLIGHT_MAX_RESPONSE_CURVE_XY_PAIRS*2] = {
                3, 34,
                39, 49,
                299, 74,
                799, 99,
                1274, 174,
                1799, 199
            };

            if (uReportSize == m_pSensorManager->m_HidCaps.FeatureReportByteLength)
            {
                USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;

                LONG   UnitsExp = 0;
                ULONG  BitSize = 0;
                ULONG  Units = 0;

                hr = HandleGetHidFeatureSelectors(
                        &m_DeviceProperties.fReportingStateSelectorSupported,
                        &m_DeviceProperties.ulReportingStateSelector,
                        &m_DeviceProperties.fPowerStateSelectorSupported,
                        &m_DeviceProperties.ulPowerStateSelector,
                        &m_DeviceProperties.fSensorStatusSelectorSupported,
                        &m_DeviceProperties.ulSensorStatusSelector,
                        &m_DeviceProperties.fConnectionTypeSelectorSupported,
                        &m_DeviceProperties.ulConnectionTypeSelector,
                        ReportType, 
                        UsagePage, 
                        LinkCollection, 
                        UsageList, 
                        &numUsages,
                        pFeatureReport, 
                        uReportSize);

                for(ULONG idx = 0; idx < numNodes; idx++)
                {
                    if (reportID == m_FeatureValueCapsNodes[idx].ReportID)
                    {
                        UsagePage = m_FeatureValueCapsNodes[idx].UsagePage;
                        Usage = m_FeatureValueCapsNodes[idx].NotRange.Usage;
                        UsageDataModifier = (USHORT)Usage & 0xF000; //extract the data modifier
                        ReportCount = m_FeatureValueCapsNodes[idx].ReportCount;
                        Units = m_FeatureValueCapsNodes[idx].Units;
                        UnitsExp = m_FeatureValueCapsNodes[idx].UnitsExp;
                        BitSize = m_FeatureValueCapsNodes[idx].BitSize;
                        TranslateHidUnitsExp(&UnitsExp);

                        if (ReportCount > 1)
                        {
                            if (HID_DRIVER_USAGE_SENSOR_PROPERTY_RESPONSE_CURVE == Usage)
                            {
                                ZeroMemory(usResponseCurveXYPairs, sizeof(usResponseCurveXYPairs));
                                hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, (PCHAR)usResponseCurveXYPairs, sizeof(usResponseCurveXYPairs), m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                            }
                            else
                            {
                                ZeroMemory(UsageArray, HID_FEATURE_REPORT_STRING_MAX_LENGTH*2);
                                hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, UsageArray, HID_FEATURE_REPORT_STRING_MAX_LENGTH*2, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                            }
                        }
                        else if (ReportCount == 1)
                        {
                            UsageUValue = 0;
                            hr = HidP_GetUsageValue(ReportType, UsagePage, LinkCollection, Usage, &UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                        }
                        else
                        {
                            hr = E_UNEXPECTED;
                            Trace(TRACE_LEVEL_ERROR, "Feature Report Count == 0, hr = %!HRESULT!", hr);
                        }

                        if(SUCCEEDED(hr))
                        {
                            BOOL fFeatureHandled = FALSE;

                            Usage = Usage & 0x0FFF; //remove the data modifier
                            UsageValue = ExtractValueFromUsageUValue(m_FeatureValueCapsNodes[idx].LogicalMin, BitSize, UsageUValue);

                            hr = HandleGetCommonFeatureValues(
                                    idx,
                                    &m_DeviceProperties.fReportingStateSupported,
                                    &m_DeviceProperties.ulReportingState,
                                    &m_DeviceProperties.fPowerStateSupported,
                                    &m_DeviceProperties.ulPowerState,
                                    &m_DeviceProperties.fSensorStatusSupported,
                                    &m_DeviceProperties.ulSensorStatus,
                                    &m_DeviceProperties.fConnectionTypeSupported,
                                    &m_DeviceProperties.ulConnectionType,
                                    &m_DeviceProperties.fReportIntervalSupported,
                                    &m_DeviceProperties.ulReportInterval,
                                    &m_DeviceProperties.fGlobalSensitivitySupported,
                                    &m_DeviceProperties.fltGlobalSensitivity,
                                    &m_DeviceProperties.fGlobalMaximumSupported,
                                    &m_DeviceProperties.fltGlobalMaximum,
                                    &m_DeviceProperties.fGlobalMinimumSupported,
                                    &m_DeviceProperties.fltGlobalMinimum,
                                    &m_DeviceProperties.fGlobalAccuracySupported,
                                    &m_DeviceProperties.fltGlobalAccuracy,
                                    &m_DeviceProperties.fGlobalResolutionSupported,
                                    &m_DeviceProperties.fltGlobalResolution,
                                    &m_DeviceProperties.fMinimumReportIntervalSupported,
                                    &m_DeviceProperties.ulMinimumReportInterval,
                                    &m_DeviceProperties.fFriendlyNameSupported,
                                    m_DeviceProperties.wszFriendlyName,
                                    &m_DeviceProperties.fPersistentUniqueIDSupported,
                                    m_DeviceProperties.wszPersistentUniqueID,
                                    &m_DeviceProperties.fManufacturerSupported,
                                    m_DeviceProperties.wszManufacturer,
                                    &m_DeviceProperties.fModelSupported,
                                    m_DeviceProperties.wszModel,
                                    &m_DeviceProperties.fSerialNumberSupported,
                                    m_DeviceProperties.wszSerialNumber,
                                    &m_DeviceProperties.fDescriptionSupported,
                                    m_DeviceProperties.wszDescription,
                                    Usage,
                                    UsageValue,
                                    UsageUValue,
                                    UnitsExp,
                                    UsageArray,
                                    &fFeatureHandled);

                            if (SUCCEEDED(hr) && (FALSE == fFeatureHandled))
                            {
                                switch(Usage)
                                {
                                case HID_DRIVER_USAGE_SENSOR_PROPERTY_RESPONSE_CURVE:
                                    m_DeviceProperties.fAmbientLightResponseCurveSupported = TRUE;
                                    if (SUCCEEDED(hr))
                                    {
                                        PROPVARIANT value;
                                        PropVariantInit( &value );
                                        value.vt = VT_EMPTY;

                                        m_DeviceProperties.fAmbientLightResponseCurveSupported = TRUE;
                                        //there may be fewer pairs of response curve values than the max, so choose the lesser of the possible values
                                        m_DeviceProperties.ulAmbientLightResponseCurveCount = ((ReportCount/2) <= AMBIENTLIGHT_MAX_RESPONSE_CURVE_XY_PAIRS) \
                                                                                                    ? (ReportCount/2)                                       \
                                                                                                    : AMBIENTLIGHT_MAX_RESPONSE_CURVE_XY_PAIRS;

                                        for (ULONG i = 0; i <  m_DeviceProperties.ulAmbientLightResponseCurveCount; i++)
                                        {
                                            if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                            {
                                                TranslateHidUnitsExp(&UnitsExp);

                                                //get value for percent
                                                usResponseCurveX = usResponseCurveXYPairs[i*2];
                                                dwResponseCurveX = (DWORD)((usResponseCurveX * pow( 10.0, (double)((1.0) * UnitsExp ))) / 1.0F);
                                                dwResponseCurveXYPairs[i*2] = dwResponseCurveX;
                                                m_DeviceProperties.dwAmbientLightResponseCurveXYPairs[i*2] = dwResponseCurveX;

                                                //get value for illuminance
                                                usResponseCurveY = usResponseCurveXYPairs[(i*2)+1];
                                                //NOTE: the scale factor needs to be adjusted as this is a percent and not a LUX value as is X
                                                dwResponseCurveY = (DWORD)((usResponseCurveY * pow( 10.0, (double)((1.0) * UnitsExp ))) / 1.0F);
                                                dwResponseCurveXYPairs[(i*2)+1] = dwResponseCurveY;
                                                m_DeviceProperties.dwAmbientLightResponseCurveXYPairs[(i*2)+1] = dwResponseCurveY;
                                            }
                                        }
                                        //using min here to prevent OACR complaint
                                        hr = InitPropVariantFromBuffer(dwResponseCurveXYPairs, sizeof(DWORD)*m_DeviceProperties.ulAmbientLightResponseCurveCount*2, &value);
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetProperty(SENSOR_PROPERTY_LIGHT_RESPONSE_CURVE, &value, nullptr);
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }

                                        PropVariantClear( &value );
                                    }
                                    break;
                                case HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fUsingSensitivityAbs = TRUE;
                                            m_DeviceProperties.fAmbientLightIlluminanceSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightIlluminanceSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_REL_PCT:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_PERCENT == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fUsingSensitivityRelPct = TRUE;
                                            m_DeviceProperties.fAmbientLightIlluminanceSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightIlluminanceSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fAmbientLightIlluminanceMaximumSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightIlluminanceMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fAmbientLightIlluminanceMinimumSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightIlluminanceMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fAmbientLightIlluminanceAccuracySupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightIlluminanceAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fAmbientLightIlluminanceResolutionSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightIlluminanceResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fAmbientLightColorTempSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightColorTempSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fAmbientLightColorTempMaximumSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightColorTempMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fAmbientLightColorTempMinimumSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightColorTempMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fAmbientLightColorTempAccuracySupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightColorTempAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fAmbientLightColorTempResolutionSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightColorTempResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY_X:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityXSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightChromaticityXSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityXMaximumSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightChromaticityXMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityXMinimumSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightChromaticityXMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityXAccuracySupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightChromaticityXAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityXResolutionSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightChromaticityXResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY_Y:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityYSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightChromaticityYSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityYMaximumSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightChromaticityYMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityYMinimumSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightChromaticityYMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityYAccuracySupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightChromaticityYAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fAmbientLightChromaticityYResolutionSupported = TRUE;
                                            m_DeviceProperties.fltAmbientLightChromaticityYResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                default:
                                    hr = HandleDefinedDynamicDatafieldProperty(Usage, UnitsExp, UsageValue, UsageDataModifier);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);
                        }
                    }
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Feature report is incorrect length, hr = %!HRESULT!", hr);
            }
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Failed to get configuration from %s, hr = %!HRESULT!", m_SensorName, hr);
        }
    }


    // Report the Feature report conditions for this sensor
    if (FALSE == m_fInformedCommonFeatureReportConditions)
    {
        ReportCommonFeatureReportDescriptorConditions(
            m_fFeatureReportSupported,
            m_DeviceProperties.fReportingStateSelectorSupported,
            m_DeviceProperties.fPowerStateSelectorSupported,
            m_DeviceProperties.fSensorStatusSelectorSupported,
            m_DeviceProperties.fConnectionTypeSelectorSupported,
            m_DeviceProperties.fReportingStateSupported,
            m_DeviceProperties.fPowerStateSupported,
            m_DeviceProperties.fSensorStatusSupported,
            m_DeviceProperties.fConnectionTypeSupported,
            m_DeviceProperties.fReportIntervalSupported,
            m_DeviceProperties.fGlobalSensitivitySupported,
            m_DeviceProperties.fGlobalMaximumSupported,
            m_DeviceProperties.fGlobalMinimumSupported,
            m_DeviceProperties.fGlobalAccuracySupported,
            m_DeviceProperties.fGlobalResolutionSupported,
            m_DeviceProperties.fMinimumReportIntervalSupported,
            m_DeviceProperties.fFriendlyNameSupported,
            m_DeviceProperties.fPersistentUniqueIDSupported,
            m_DeviceProperties.fManufacturerSupported,
            m_DeviceProperties.fModelSupported,
            m_DeviceProperties.fSerialNumberSupported,
            m_DeviceProperties.fDescriptionSupported
            );

        //Property conditions specific to this sensor

    }

    if (TRUE == m_fFeatureReportSupported)
    {
        // Update the local properties and write changes back to the device
        if (SUCCEEDED(hr))
        {
            for (DWORD dwIndex = 0; SUCCEEDED(hr) && dwIndex < cValues; dwIndex++)
            {
                PROPERTYKEY Key = WPD_PROPERTY_NULL;
                PROPVARIANT var;

                PropVariantInit( &var );
                if (SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorProperties->GetAt(dwIndex, &Key);
                }

                if(SUCCEEDED(hr))
                {
                    if ((TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL)) && (TRUE == m_DeviceProperties.fReportIntervalSupported))
                    {
                        hr = HandleReportIntervalUpdate(reportID, m_DeviceProperties.fReportIntervalSupported, &m_DeviceProperties.ulReportInterval, pFeatureReport, uReportSize);

                        if (FAILED(hr))
                        {
                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Report Interval in property update, hr = %!HRESULT!", hr);
                        }
                    }

                    else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_CHANGE_SENSITIVITY))
                    {
                        DWORD cDfKeys = 0;
                        PROPERTYKEY pkDfKey = {0};

                        hr = m_spSupportedSensorDataFields->GetCount(&cDfKeys);

                        if (SUCCEEDED(hr))
                        {
                            for (DWORD dwDfIdx = 1; dwDfIdx < cDfKeys; dwDfIdx++)
                            {
                                hr = m_spSupportedSensorDataFields->GetAt(dwDfIdx, &pkDfKey);

                                if (SUCCEEDED(hr))
                                {
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fAmbientLightIlluminanceSensitivitySupported,
                                                                            m_DeviceProperties.fAmbientLightIlluminanceSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE,
                                                                            m_DeviceProperties.fAmbientLightIlluminanceSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX,
                                                                            SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltAmbientLightIlluminanceSensitivity, 
                                                                            &m_DeviceProperties.fltAmbientLightIlluminanceSensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fAmbientLightColorTempSensitivitySupported,
                                                                            m_DeviceProperties.fAmbientLightColorTempSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE,
                                                                            m_DeviceProperties.fAmbientLightColorTempSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN,
                                                                            SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltAmbientLightColorTempSensitivity, 
                                                                            &m_DeviceProperties.fltAmbientLightColorTempSensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_CHROMACITY))
                                    {
                                        if (m_DeviceProperties.fAmbientLightChromaticityXSensitivitySupported && m_DeviceProperties.fAmbientLightChromaticityYSensitivitySupported)
                                        {
                                            m_DeviceProperties.fltAmbientLightChromaticitySensitivity = (m_DeviceProperties.fltAmbientLightChromaticityXSensitivity + m_DeviceProperties.fltAmbientLightChromaticityYSensitivity) / 2.0F;
                                        }

                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fAmbientLightChromaticitySensitivitySupported,
                                                                            m_DeviceProperties.fAmbientLightChromaticitySensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY,
                                                                            m_DeviceProperties.fAmbientLightChromaticitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_LIGHT_CHROMACITY,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltAmbientLightChromaticitySensitivity, 
                                                                            &m_DeviceProperties.fltAmbientLightChromaticitySensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else //handle dynamic datafield
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            FALSE,
                                                                            FALSE,
                                                                            m_DynamicDatafieldSensitivitySupported[dwDfIdx],
                                                                            m_DynamicDatafieldUsages[dwDfIdx],
                                                                            (BOOL)m_DynamicDatafieldUsages[dwDfIdx],
                                                                            m_DynamicDatafieldUsages[dwDfIdx],
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            pkDfKey,
                                                                            dwDfIdx,
                                                                            &m_DynamicDatafieldSensitivity[dwDfIdx], 
                                                                            &m_DynamicDatafieldSensitivity[dwDfIdx], 
                                                                            &m_DynamicDatafieldSensitivity[dwDfIdx], 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RANGE_MAXIMUM)) 
                    {
                        DWORD cDfKeys = 0;
                        PROPERTYKEY pkDfKey = {0};

                        hr = m_spSupportedSensorDataFields->GetCount(&cDfKeys);

                        if (SUCCEEDED(hr))
                        {
                            for (DWORD dwDfIdx = 1; dwDfIdx < cDfKeys; dwDfIdx++)
                            {
                                hr = m_spSupportedSensorDataFields->GetAt(dwDfIdx, &pkDfKey);

                                if (SUCCEEDED(hr))
                                {
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceMaximumSupported,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX,
                                                                    SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltAmbientLightIlluminanceMaximum, 
                                                                    &m_DeviceProperties.fltAmbientLightIlluminanceMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fAmbientLightColorTempMaximumSupported,
                                                                    m_DeviceProperties.fAmbientLightColorTempMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE,
                                                                    m_DeviceProperties.fAmbientLightColorTempSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN,
                                                                    SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltAmbientLightColorTempMaximum, 
                                                                    &m_DeviceProperties.fltAmbientLightColorTempMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_CHROMACITY))
                                    {
                                        if (m_DeviceProperties.fAmbientLightChromaticityXMaximumSupported && m_DeviceProperties.fAmbientLightChromaticityYMaximumSupported)
                                        {
                                            m_DeviceProperties.fltAmbientLightChromaticityMaximum = (m_DeviceProperties.fltAmbientLightChromaticityXMaximum + m_DeviceProperties.fltAmbientLightChromaticityYMaximum) / 2.0F;
                                        }

                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fAmbientLightChromaticityMaximumSupported,
                                                                    m_DeviceProperties.fAmbientLightChromaticityMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY,
                                                                    m_DeviceProperties.fAmbientLightChromaticitySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_LIGHT_CHROMACITY,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltAmbientLightChromaticityMaximum, 
                                                                    &m_DeviceProperties.fltAmbientLightChromaticityMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else //handle dynamic datafield
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    FALSE,
                                                                    FALSE,
                                                                    m_DynamicDatafieldMaximumSupported[dwDfIdx],
                                                                    m_DynamicDatafieldUsages[dwDfIdx],
                                                                    (BOOL)m_DynamicDatafieldUsages[dwDfIdx],
                                                                    m_DynamicDatafieldUsages[dwDfIdx],
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    pkDfKey,
                                                                    dwDfIdx,
                                                                    &m_DynamicDatafieldMaximum[dwDfIdx], 
                                                                    &m_DynamicDatafieldMaximum[dwDfIdx], 
                                                                    &m_DynamicDatafieldMaximum[dwDfIdx], 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RANGE_MINIMUM))
                    {
                        DWORD cDfKeys = 0;
                        PROPERTYKEY pkDfKey = {0};

                        hr = m_spSupportedSensorDataFields->GetCount(&cDfKeys);

                        if (SUCCEEDED(hr))
                        {
                            for (DWORD dwDfIdx = 1; dwDfIdx < cDfKeys; dwDfIdx++)
                            {
                                hr = m_spSupportedSensorDataFields->GetAt(dwDfIdx, &pkDfKey);

                                if (SUCCEEDED(hr))
                                {
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceMinimumSupported,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX,
                                                                    SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltAmbientLightIlluminanceMinimum, 
                                                                    &m_DeviceProperties.fltAmbientLightIlluminanceMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fAmbientLightColorTempMinimumSupported,
                                                                    m_DeviceProperties.fAmbientLightColorTempMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE,
                                                                    m_DeviceProperties.fAmbientLightColorTempSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN,
                                                                    SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltAmbientLightColorTempMinimum, 
                                                                    &m_DeviceProperties.fltAmbientLightColorTempMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_CHROMACITY))
                                    {
                                        if (m_DeviceProperties.fAmbientLightChromaticityXMinimumSupported && m_DeviceProperties.fAmbientLightChromaticityYMinimumSupported)
                                        {
                                            m_DeviceProperties.fltAmbientLightChromaticityMinimum = (m_DeviceProperties.fltAmbientLightChromaticityXMinimum + m_DeviceProperties.fltAmbientLightChromaticityYMinimum) / 2.0F;
                                        }

                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fAmbientLightChromaticityMinimumSupported,
                                                                    m_DeviceProperties.fAmbientLightChromaticityMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY,
                                                                    m_DeviceProperties.fAmbientLightChromaticitySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_LIGHT_CHROMACITY,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltAmbientLightChromaticityMinimum, 
                                                                    &m_DeviceProperties.fltAmbientLightChromaticityMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else //handle dynamic datafield
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    FALSE,
                                                                    FALSE,
                                                                    m_DynamicDatafieldMinimumSupported[dwDfIdx],
                                                                    m_DynamicDatafieldUsages[dwDfIdx],
                                                                    (BOOL)m_DynamicDatafieldUsages[dwDfIdx],
                                                                    m_DynamicDatafieldUsages[dwDfIdx],
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    pkDfKey,
                                                                    dwDfIdx,
                                                                    &m_DynamicDatafieldMinimum[dwDfIdx], 
                                                                    &m_DynamicDatafieldMinimum[dwDfIdx], 
                                                                    &m_DynamicDatafieldMinimum[dwDfIdx], 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_ACCURACY))
                    {
                        DWORD cDfKeys = 0;
                        PROPERTYKEY pkDfKey = {0};

                        hr = m_spSupportedSensorDataFields->GetCount(&cDfKeys);

                        if (SUCCEEDED(hr))
                        {
                            for (DWORD dwDfIdx = 1; dwDfIdx < cDfKeys; dwDfIdx++)
                            {
                                hr = m_spSupportedSensorDataFields->GetAt(dwDfIdx, &pkDfKey);

                                if (SUCCEEDED(hr))
                                {
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceAccuracySupported,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX,
                                                                    SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltAmbientLightIlluminanceAccuracy, 
                                                                    &m_DeviceProperties.fltAmbientLightIlluminanceAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fAmbientLightColorTempAccuracySupported,
                                                                    m_DeviceProperties.fAmbientLightColorTempAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE,
                                                                    m_DeviceProperties.fAmbientLightColorTempSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN,
                                                                    SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltAmbientLightColorTempAccuracy, 
                                                                    &m_DeviceProperties.fltAmbientLightColorTempAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_CHROMACITY))
                                    {
                                        if (m_DeviceProperties.fAmbientLightChromaticityXAccuracySupported && m_DeviceProperties.fAmbientLightChromaticityYAccuracySupported)
                                        {
                                            m_DeviceProperties.fltAmbientLightChromaticityAccuracy = (m_DeviceProperties.fltAmbientLightChromaticityXAccuracy + m_DeviceProperties.fltAmbientLightChromaticityYAccuracy) / 2.0F;
                                        }

                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fAmbientLightChromaticityAccuracySupported,
                                                                    m_DeviceProperties.fAmbientLightChromaticityAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY,
                                                                    m_DeviceProperties.fAmbientLightChromaticitySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_LIGHT_CHROMACITY,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltAmbientLightChromaticityAccuracy, 
                                                                    &m_DeviceProperties.fltAmbientLightChromaticityAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else //handle dynamic datafield
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    FALSE,
                                                                    FALSE,
                                                                    m_DynamicDatafieldAccuracySupported[dwDfIdx],
                                                                    m_DynamicDatafieldUsages[dwDfIdx],
                                                                    (BOOL)m_DynamicDatafieldUsages[dwDfIdx],
                                                                    m_DynamicDatafieldUsages[dwDfIdx],
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    pkDfKey,
                                                                    dwDfIdx,
                                                                    &m_DynamicDatafieldAccuracy[dwDfIdx], 
                                                                    &m_DynamicDatafieldAccuracy[dwDfIdx], 
                                                                    &m_DynamicDatafieldAccuracy[dwDfIdx], 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RESOLUTION))
                    {
                        DWORD cDfKeys = 0;
                        PROPERTYKEY pkDfKey = {0};

                        hr = m_spSupportedSensorDataFields->GetCount(&cDfKeys);

                        if (SUCCEEDED(hr))
                        {
                            for (DWORD dwDfIdx = 1; dwDfIdx < cDfKeys; dwDfIdx++)
                            {
                                hr = m_spSupportedSensorDataFields->GetAt(dwDfIdx, &pkDfKey);

                                if (SUCCEEDED(hr))
                                {
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceResolutionSupported,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE,
                                                                    m_DeviceProperties.fAmbientLightIlluminanceSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX,
                                                                    SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltAmbientLightIlluminanceResolution, 
                                                                    &m_DeviceProperties.fltAmbientLightIlluminanceResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fAmbientLightColorTempResolutionSupported,
                                                                    m_DeviceProperties.fAmbientLightColorTempResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE,
                                                                    m_DeviceProperties.fAmbientLightColorTempSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN,
                                                                    SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltAmbientLightColorTempResolution, 
                                                                    &m_DeviceProperties.fltAmbientLightColorTempResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_LIGHT_CHROMACITY))
                                    {
                                        if (m_DeviceProperties.fAmbientLightChromaticityXResolutionSupported && m_DeviceProperties.fAmbientLightChromaticityYResolutionSupported)
                                        {
                                            m_DeviceProperties.fltAmbientLightChromaticityResolution = (m_DeviceProperties.fltAmbientLightChromaticityXResolution + m_DeviceProperties.fltAmbientLightChromaticityYResolution) / 2.0F;
                                        }

                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fAmbientLightChromaticityResolutionSupported,
                                                                    m_DeviceProperties.fAmbientLightChromaticityResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY,
                                                                    m_DeviceProperties.fAmbientLightChromaticitySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_LIGHT_CHROMACITY,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltAmbientLightChromaticityResolution, 
                                                                    &m_DeviceProperties.fltAmbientLightChromaticityResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else //handle dynamic datafield
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    FALSE,
                                                                    FALSE,
                                                                    m_DynamicDatafieldResolutionSupported[dwDfIdx],
                                                                    m_DynamicDatafieldUsages[dwDfIdx],
                                                                    (BOOL)m_DynamicDatafieldUsages[dwDfIdx],
                                                                    m_DynamicDatafieldUsages[dwDfIdx],
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    pkDfKey,
                                                                    dwDfIdx,
                                                                    &m_DynamicDatafieldResolution[dwDfIdx], 
                                                                    &m_DynamicDatafieldResolution[dwDfIdx], 
                                                                    &m_DynamicDatafieldResolution[dwDfIdx], 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else if (  (TRUE == IsEqualPropertyKey(Key, WPD_FUNCTIONAL_OBJECT_CATEGORY))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_TYPE))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_STATE))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_MIN_REPORT_INTERVAL))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_PERSISTENT_UNIQUE_ID))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_MANUFACTURER))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_MODEL))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_SERIAL_NUMBER))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_FRIENDLY_NAME))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_DESCRIPTION))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_CONNECTION_TYPE))
                            //|| (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL))
                            //|| (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_CHANGE_SENSITIVITY))
                            //|| (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RANGE_MAXIMUM))
                            //|| (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RANGE_MINIMUM))
                            //|| (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_ACCURACY))
                            //|| (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RESOLUTION))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_HID_USAGE))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_LIGHT_RESPONSE_CURVE))
                            )
                    {
                        //no action - updates not supported for these properties
                    }

                    else
                    {
                        Trace(TRACE_LEVEL_ERROR, "Failed to find update code for %s property, Key.fmtid = %!GUID!-%i", m_SensorName, &Key.fmtid, Key.pid);
                    }
                }

                PropVariantClear( &var );
            }
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Failed to get extract properties from %s feature report, hr = %!HRESULT!", m_SensorName, hr);
        }

        if (SUCCEEDED(hr))
        {
            hr = HandleSetReportingAndPowerStates(
                m_DeviceProperties.fReportingStateSupported,
                m_DeviceProperties.fReportingStateSelectorSupported,
                m_fReportingState,
                m_DeviceProperties.fPowerStateSupported,
                m_DeviceProperties.fPowerStateSelectorSupported,
                m_ulPowerState,
                ReportType, 
                UsagePage, 
                m_SensorLinkCollection, 
                UsageList, 
                &numUsages, 
                pFeatureReport, 
                uReportSize
                );
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Failed to update %s device properties, hr = %!HRESULT!", m_SensorName, hr);
        }

        // Send the Write Request down the stack
        if(SUCCEEDED(hr))
        {
            *pReportSize = m_pSensorManager->m_HidCaps.FeatureReportByteLength;
            Trace(TRACE_LEVEL_INFORMATION, "%s device properties updated, hr = %!HRESULT!", m_SensorName, hr);
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Failed to update %s device reporting and power states, hr = %!HRESULT!", m_SensorName, hr);
        }
    }

    return hr;
}

