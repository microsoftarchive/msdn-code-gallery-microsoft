 /*

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


 Module:
      Custom.cpp

 Description:
      Implements the CCustom container class

--*/

#include "internal.h"
#include "SensorDdi.h"
#include "SensorManager.h"

#include "Sensor.h"
#include "Custom.h"

#include "Custom.tmh"


const PROPERTYKEY g_RequiredSupportedCustomProperties[] =
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

const PROPERTYKEY g_OptionalSupportedCustomProperties[] =
{
    SENSOR_PROPERTY_RANGE_MAXIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RANGE_MINIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_ACCURACY,                   //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RESOLUTION,                 //[VT_UNKNOWN], IPortableDeviceValues
};

const PROPERTYKEY g_RequiredSettableCustomProperties[] =
{
    SENSOR_PROPERTY_CHANGE_SENSITIVITY,         //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
};

const PROPERTYKEY g_SupportedCustomDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                 //[VT_FILETIME]
};

const PROPERTYKEY g_OptionalCustomDataFields[] =
{
    SENSOR_DATA_TYPE_CUSTOM_USAGE,              //[VT_UI4]
    SENSOR_DATA_TYPE_CUSTOM_BOOLEAN_ARRAY,      //[VT_UI4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE1,             //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE2,             //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE3,             //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE4,             //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE5,             //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE6,             //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE7,             //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE8,             //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE9,             //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE10,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE11,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE12,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE13,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE14,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE15,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE16,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE17,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE18,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE19,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE20,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE21,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE22,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE23,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE24,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE25,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE26,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE27,            //[VT_UI4]|[VT_R4]
    SENSOR_DATA_TYPE_CUSTOM_VALUE28,            //[VT_UI4]|[VT_R4]
};

const PROPERTYKEY g_SupportedCustomEvents[] =
{
    SENSOR_EVENT_DATA_UPDATED, 0,
    SENSOR_EVENT_STATE_CHANGED, 0,
};

/////////////////////////////////////////////////////////////////////////
//
// CCustom::CCustom
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CCustom::CCustom()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CCustom::~CCustom
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CCustom::~CCustom()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CCustom::Initialize
//
// Initializes the PROPERTYKEY/PROPVARIANT values for
// the Supported Properties & Supported Data
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCustom::Initialize(
    _In_ SensorType sensType,
    _In_ ULONG sensUsage,
    _In_ USHORT sensLinkCollection,
    _In_ DWORD sensNum, 
    _In_ LPWSTR pwszManufacturer,
    _In_ LPWSTR pwszProduct,
    _In_ LPWSTR pwszSerialNumber,
    _In_ LPWSTR pwszSensorID,
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
                                pwszSensorID);
    }

    strcpy_s(m_SensorName, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_CUSTOM_TRACE_NAME);

    if(SUCCEEDED(hr))
    {
        hr = InitializeCustom();
    }

    if(SUCCEEDED(hr))
    {
        m_fSensorInitialized = TRUE;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CCustom::InitializeCustom
//
// Initializes the Custom PropertyKeys and DataFieldKeys 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCustom::InitializeCustom( )
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    hr = AddCustomPropertyKeys();

    if (SUCCEEDED(hr))
    {
        hr = AddCustomSettablePropertyKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = AddCustomDataFieldKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = SetCustomDefaultValues();
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CCustom::AddCustomPropertyKeys
//
// Copies the PROPERTYKEYS for Custom Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCustom::AddCustomPropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSupportedCustomProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSupportedCustomProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_RequiredSupportedCustomProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_OptionalSupportedCustomProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_OptionalSupportedCustomProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_OptionalSupportedCustomProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CCustom::AddCustomSettablePropertyKeys
//
// Copies the PROPERTYKEYS for Custom Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCustom::AddCustomSettablePropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSettableCustomProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSettableCustomProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSettableSensorProperties->Add(g_RequiredSettableCustomProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CCustom::AddCustomDataFieldKeys
//
// Copies the PROPERTYKEYS for Custom Supported DataFields 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCustom::AddCustomDataFieldKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedCustomDataFields); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetDataField(g_SupportedCustomDataFields[dwIndex], &var);

        // Also add the PROPERTYKEY to the list of supported data fields
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorDataFields->Add(g_SupportedCustomDataFields[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CCustom::SetCustomDefaultValues
//
// Fills in default values for most Custom Properties and 
// Data Fields.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCustom::SetCustomDefaultValues()
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

    m_ulDefaultCurrentReportInterval = DEFAULT_CUSTOM_CURRENT_REPORT_INTERVAL;
    m_ulDefaultMinimumReportInterval = DEFAULT_CUSTOM_MIN_REPORT_INTERVAL;

    m_fltDefaultChangeSensitivity = DEFAULT_CUSTOM_SENSITIVITY;

    m_fltDefaultRangeMaximum = DEFAULT_CUSTOM_MAXIMUM;
    m_fltDefaultRangeMinimum = DEFAULT_CUSTOM_MINIMUM;
    m_fltDefaultAccuracy = DEFAULT_CUSTOM_ACCURACY;
    m_fltDefaultResolution = DEFAULT_CUSTOM_RESOLUTION;

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, SENSOR_CATEGORY_OTHER);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_CUSTOM);
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
            wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_CUSTOM_NAME);
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, tempStr);
        }
        else
        {
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, m_pSensorManager->m_wszDeviceName);
        }
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_CUSTOM_DESCRIPTION);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CONNECTION_TYPE, SENSOR_CONNECTION_TYPE_PC_ATTACHED);
    }

    // *****************************************************************************************
    // Default values for SENSOR PER-DATAFIELD PROPERTIES
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

                        if (SUCCEEDED(hr))
                        {
                            if ((SENSOR_DATA_TYPE_CUSTOM_VALUE1 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE2 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE3 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE4 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE5 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE6 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE7 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE8 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE9 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE10 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE11 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE12 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE13 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE14 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE15 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE16 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE17 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE18 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE19 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE20 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE21 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE22 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE23 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE24 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE25 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE26 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE27 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE28 == datakey))
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
                            if ((SENSOR_DATA_TYPE_CUSTOM_VALUE1 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE2 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE3 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE4 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE5 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE6 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE7 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE8 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE9 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE10 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE11 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE12 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE13 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE14 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE15 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE16 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE17 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE18 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE19 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE20 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE21 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE22 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE23 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE24 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE25 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE26 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE27 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE28 == datakey))
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
                            if ((SENSOR_DATA_TYPE_CUSTOM_VALUE1 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE2 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE3 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE4 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE5 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE6 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE7 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE8 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE9 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE10 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE11 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE12 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE13 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE14 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE15 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE16 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE17 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE18 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE19 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE20 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE21 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE22 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE23 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE24 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE25 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE26 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE27 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE28 == datakey))
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
                            if ((SENSOR_DATA_TYPE_CUSTOM_VALUE1 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE2 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE3 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE4 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE5 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE6 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE7 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE8 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE9 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE10 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE11 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE12 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE13 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE14 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE15 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE16 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE17 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE18 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE19 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE20 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE21 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE22 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE23 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE24 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE25 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE26 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE27 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE28 == datakey))
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
                            if ((SENSOR_DATA_TYPE_CUSTOM_VALUE1 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE2 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE3 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE4 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE5 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE6 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE7 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE8 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE9 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE10 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE11 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE12 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE13 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE14 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE15 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE16 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE17 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE18 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE19 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE20 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE21 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE22 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE23 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE24 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE25 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE26 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE27 == datakey) ||
                                (SENSOR_DATA_TYPE_CUSTOM_VALUE28 == datakey))
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
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_USAGE, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_BOOLEAN_ARRAY, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE1, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE2, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE3, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE4, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE5, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE6, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE7, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE8, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE9, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE10, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE11, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE12, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE13, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE14, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE15, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE16, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE17, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE18, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE19, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE20, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE21, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE22, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE23, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE24, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE25, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE26, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE27, &value);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_CUSTOM_VALUE28, &value);
    }

    PropVariantClear( &value );

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CCustom::GetPropertyValuesForCustomObject
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
HRESULT CCustom::GetPropertyValuesForCustomObject(
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

    hr = GetPropertyValuesForSensorObject(wszObjectID, pKeys, pValues, SENSOR_CUSTOM_NAME, SENSOR_CATEGORY_OTHER, &fError);

    return (FALSE == fError) ? hr : S_FALSE;
}

/////////////////////////////////////////////////////////////////////////
//
// CCustom::ProcessCustomAsyncRead
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCustom::ProcessCustomAsyncRead( BYTE* buffer, ULONG length )
{
    HRESULT             hr = S_OK;

    if ((NULL == buffer) || (length == 0))
    {
        hr = E_UNEXPECTED;
    }

    if (SUCCEEDED(hr) && (m_fSensorInitialized))
    {
        if (length == m_pSensorManager->m_HidCaps.InputReportByteLength)
        {
            //Handle input report

            HIDP_REPORT_TYPE ReportType = HidP_Input;
            USAGE UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
            USHORT LinkCollection = 0;
            UCHAR reportID = 0;
            USAGE  Usage = 0;
            USHORT UsageDataModifier = 0;
            LONG  UsageValue = 0;
            ULONG UsageUValue = 0;
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
                LONG    nCustomBooleanArray = 0;
                LONG    nCustomValue1 = 0, nCustomValue2 = 0, nCustomValue3 = 0, nCustomValue4 = 0, nCustomValue5 = 0, nCustomValue6 = 0, nCustomValue7 = 0;
                LONG    nCustomValue8 = 0, nCustomValue9 = 0, nCustomValue10 = 0, nCustomValue11 = 0, nCustomValue12 = 0, nCustomValue13 = 0, nCustomValue14 = 0;
                LONG    nCustomValue15 = 0, nCustomValue16 = 0, nCustomValue17 = 0, nCustomValue18 = 0, nCustomValue19 = 0, nCustomValue20 = 0, nCustomValue21 = 0;
                LONG    nCustomValue22 = 0, nCustomValue23 = 0, nCustomValue24 = 0, nCustomValue25 = 0, nCustomValue26 = 0, nCustomValue27 = 0, nCustomValue28 = 0;
                FLOAT   fltCustomValue1 = 0.0F, fltCustomValue2 = 0.0F, fltCustomValue3 = 0.0F, fltCustomValue4 = 0.0F, fltCustomValue5 = 0.0F, fltCustomValue6 = 0.0F, fltCustomValue7 = 0.0F;
                FLOAT   fltCustomValue8 = 0.0F, fltCustomValue9 = 0.0F, fltCustomValue10 = 0.0F, fltCustomValue11 = 0.0F, fltCustomValue12 = 0.0F, fltCustomValue13 = 0.0F, fltCustomValue14 = 0.0F;
                FLOAT   fltCustomValue15 = 0.0F, fltCustomValue16 = 0.0F, fltCustomValue17 = 0.0F, fltCustomValue18 = 0.0F, fltCustomValue19 = 0.0F, fltCustomValue20 = 0.0F, fltCustomValue21 = 0.0F;
                FLOAT   fltCustomValue22 = 0.0F, fltCustomValue23 = 0.0F, fltCustomValue24 = 0.0F, fltCustomValue25 = 0.0F, fltCustomValue26 = 0.0F, fltCustomValue27 = 0.0F, fltCustomValue28 = 0.0F;

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
                        UsageValue = 0;

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
                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_BOOLEAN_ARRAY:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomBooleanArraySupported)
                                        {
                                            m_DeviceProperties.fCustomBooleanArraySupported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_BOOLEAN_ARRAY);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_BOOLEAN_ARRAY);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_BOOLEAN_ARRAY, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            value.vt = VT_UI4;
                                            nCustomBooleanArray = UsageUValue;
                                            value.intVal = nCustomBooleanArray;
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_BOOLEAN_ARRAY, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_1:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue1Supported)
                                        {
                                            m_DeviceProperties.fCustomValue1Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE1);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE1);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE1, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue1MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue1Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue1MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue1Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue1 = UsageUValue;
                                                if (((FLOAT)nCustomValue1 > fltMax) || ((FLOAT)nCustomValue1 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue1;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue1 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue1 > fltMax) || (fltCustomValue1 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue1;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE1, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_2:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue2Supported)
                                        {
                                            m_DeviceProperties.fCustomValue2Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE2);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE2);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE2, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue2MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue2Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue2MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue2Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue2 = UsageUValue;
                                                if (((FLOAT)nCustomValue2 > fltMax) || ((FLOAT)nCustomValue2 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue2;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue2 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue2 > fltMax) || (fltCustomValue2 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue2;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE2, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_3:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue3Supported)
                                        {
                                            m_DeviceProperties.fCustomValue3Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE3);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE3);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE3, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue3MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue3Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue3MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue3Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue3 = UsageUValue;
                                                if (((FLOAT)nCustomValue3 > fltMax) || ((FLOAT)nCustomValue3 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue3;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue3 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue3 > fltMax) || (fltCustomValue3 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue3;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE3, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_4:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue4Supported)
                                        {
                                            m_DeviceProperties.fCustomValue4Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE4);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE4);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE4, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue4MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue4Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue4MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue4Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue4 = UsageUValue;
                                                if (((FLOAT)nCustomValue4 > fltMax) || ((FLOAT)nCustomValue4 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue4;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue4 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue4 > fltMax) || (fltCustomValue4 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue4;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE4, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_5:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue5Supported)
                                        {
                                            m_DeviceProperties.fCustomValue5Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE5);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE5);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE5, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue5MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue5Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue5MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue5Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue5 = UsageUValue;
                                                if (((FLOAT)nCustomValue5 > fltMax) || ((FLOAT)nCustomValue5 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue5;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue5 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue5 > fltMax) || (fltCustomValue5 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue5;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE5, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_6:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue6Supported)
                                        {
                                            m_DeviceProperties.fCustomValue6Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE6);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE6);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE6, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue6MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue6Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue6MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue6Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue6 = UsageUValue;
                                                if (((FLOAT)nCustomValue6 > fltMax) || ((FLOAT)nCustomValue6 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue6;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue6 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue6 > fltMax) || (fltCustomValue6 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue6;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE6, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_7:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue7Supported)
                                        {
                                            m_DeviceProperties.fCustomValue7Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE7);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE7);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE7, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue7MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue7Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue7MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue7Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue7 = UsageUValue;
                                                if (((FLOAT)nCustomValue7 > fltMax) || ((FLOAT)nCustomValue7 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue7;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue7 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue7 > fltMax) || (fltCustomValue7 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue7;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE7, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_8:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue8Supported)
                                        {
                                            m_DeviceProperties.fCustomValue8Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE8);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE8);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE8, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue8MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue8Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue8MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue8Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue8 = UsageUValue;
                                                if (((FLOAT)nCustomValue8 > fltMax) || ((FLOAT)nCustomValue8 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue8;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue8 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue8 > fltMax) || (fltCustomValue8 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue8;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE8, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_9:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue9Supported)
                                        {
                                            m_DeviceProperties.fCustomValue9Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE9);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE9);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE9, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue9MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue9Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue9MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue9Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue9 = UsageUValue;
                                                if (((FLOAT)nCustomValue9 > fltMax) || ((FLOAT)nCustomValue9 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue9;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue9 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue9 > fltMax) || (fltCustomValue9 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue9;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE9, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_10:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue10Supported)
                                        {
                                            m_DeviceProperties.fCustomValue10Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE10);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE10);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE10, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue10MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue10Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue10MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue10Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue10 = UsageUValue;
                                                if (((FLOAT)nCustomValue10 > fltMax) || ((FLOAT)nCustomValue10 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue10;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue10 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue10 > fltMax) || (fltCustomValue10 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue10;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE10, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_11:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue11Supported)
                                        {
                                            m_DeviceProperties.fCustomValue11Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE11);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE11);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE11, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue11MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue11Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue11MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue11Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue11 = UsageUValue;
                                                if (((FLOAT)nCustomValue11 > fltMax) || ((FLOAT)nCustomValue11 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue11;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue11 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue11 > fltMax) || (fltCustomValue11 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue11;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE11, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_12:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue12Supported)
                                        {
                                            m_DeviceProperties.fCustomValue12Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE12);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE12);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE12, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue12MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue12Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue12MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue12Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue12 = UsageUValue;
                                                if (((FLOAT)nCustomValue12 > fltMax) || ((FLOAT)nCustomValue12 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue12;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue12 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue12 > fltMax) || (fltCustomValue12 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue12;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE12, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_13:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue13Supported)
                                        {
                                            m_DeviceProperties.fCustomValue13Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE13);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE13);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE13, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue13MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue13Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue13MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue13Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue13 = UsageUValue;
                                                if (((FLOAT)nCustomValue13 > fltMax) || ((FLOAT)nCustomValue13 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue13;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue13 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue13 > fltMax) || (fltCustomValue13 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue13;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE13, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_14:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue14Supported)
                                        {
                                            m_DeviceProperties.fCustomValue14Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE14);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE14);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE14, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue14MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue14Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue14MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue14Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue14 = UsageUValue;
                                                if (((FLOAT)nCustomValue14 > fltMax) || ((FLOAT)nCustomValue14 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue14;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue14 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue14 > fltMax) || (fltCustomValue14 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue14;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE14, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_15:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue15Supported)
                                        {
                                            m_DeviceProperties.fCustomValue15Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE15);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE15);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE15, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue15MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue15Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue15MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue15Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue15 = UsageUValue;
                                                if (((FLOAT)nCustomValue15 > fltMax) || ((FLOAT)nCustomValue15 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue15;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue15 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue15 > fltMax) || (fltCustomValue15 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue15;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE15, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_16:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue16Supported)
                                        {
                                            m_DeviceProperties.fCustomValue16Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE16);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE16);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE16, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue16MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue16Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue16MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue16Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue16 = UsageUValue;
                                                if (((FLOAT)nCustomValue16 > fltMax) || ((FLOAT)nCustomValue16 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue16;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue16 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue16 > fltMax) || (fltCustomValue16 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue16;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE16, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_17:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue17Supported)
                                        {
                                            m_DeviceProperties.fCustomValue17Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE17);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE17);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE17, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue17MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue17Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue17MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue17Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue17 = UsageUValue;
                                                if (((FLOAT)nCustomValue17 > fltMax) || ((FLOAT)nCustomValue17 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue17;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue17 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue17 > fltMax) || (fltCustomValue17 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue17;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE17, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_18:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue18Supported)
                                        {
                                            m_DeviceProperties.fCustomValue18Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE18);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE18);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE18, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue18MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue18Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue18MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue18Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue18 = UsageUValue;
                                                if (((FLOAT)nCustomValue18 > fltMax) || ((FLOAT)nCustomValue18 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue18;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue18 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue18 > fltMax) || (fltCustomValue18 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue18;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE18, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_19:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue19Supported)
                                        {
                                            m_DeviceProperties.fCustomValue19Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE19);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE19);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE19, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue19MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue19Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue19MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue19Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue19 = UsageUValue;
                                                if (((FLOAT)nCustomValue19 > fltMax) || ((FLOAT)nCustomValue19 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue19;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue19 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue19 > fltMax) || (fltCustomValue19 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue19;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE19, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_20:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue20Supported)
                                        {
                                            m_DeviceProperties.fCustomValue20Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE20);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE20);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE20, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue20MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue20Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue20MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue20Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue20 = UsageUValue;
                                                if (((FLOAT)nCustomValue20 > fltMax) || ((FLOAT)nCustomValue20 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue20;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue20 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue20 > fltMax) || (fltCustomValue20 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue20;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE20, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_21:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue21Supported)
                                        {
                                            m_DeviceProperties.fCustomValue21Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE21);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE21);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE21, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue21MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue21Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue21MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue21Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue21 = UsageUValue;
                                                if (((FLOAT)nCustomValue21 > fltMax) || ((FLOAT)nCustomValue21 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue21;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue21 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue21 > fltMax) || (fltCustomValue21 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue21;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE21, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_22:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue22Supported)
                                        {
                                            m_DeviceProperties.fCustomValue22Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE22);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE22);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE22, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue22MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue22Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue22MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue22Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue22 = UsageUValue;
                                                if (((FLOAT)nCustomValue22 > fltMax) || ((FLOAT)nCustomValue22 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue22;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue22 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue22 > fltMax) || (fltCustomValue22 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue22;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE22, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_23:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue23Supported)
                                        {
                                            m_DeviceProperties.fCustomValue23Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE23);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE23);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE23, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue23MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue23Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue23MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue23Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue23 = UsageUValue;
                                                if (((FLOAT)nCustomValue23 > fltMax) || ((FLOAT)nCustomValue23 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue23;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue23 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue23 > fltMax) || (fltCustomValue23 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue23;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE23, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_24:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue24Supported)
                                        {
                                            m_DeviceProperties.fCustomValue24Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE24);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE24);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE24, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue24MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue24Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue24MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue24Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue24 = UsageUValue;
                                                if (((FLOAT)nCustomValue24 > fltMax) || ((FLOAT)nCustomValue24 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue24;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue24 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue24 > fltMax) || (fltCustomValue24 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue24;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE24, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_25:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue25Supported)
                                        {
                                            m_DeviceProperties.fCustomValue25Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE25);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE25);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE25, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue25MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue25Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue25MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue25Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue25 = UsageUValue;
                                                if (((FLOAT)nCustomValue25 > fltMax) || ((FLOAT)nCustomValue25 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue25;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue25 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue25 > fltMax) || (fltCustomValue25 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue25;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE25, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_26:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue26Supported)
                                        {
                                            m_DeviceProperties.fCustomValue26Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE26);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE26);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE26, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue26MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue26Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue26MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue26Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue26 = UsageUValue;
                                                if (((FLOAT)nCustomValue26 > fltMax) || ((FLOAT)nCustomValue26 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue26;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue26 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue26 > fltMax) || (fltCustomValue26 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue26;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE26, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_27:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue27Supported)
                                        {
                                            m_DeviceProperties.fCustomValue27Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE27);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE27);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE27, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue27MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue27Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue27MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue27Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue27 = UsageUValue;
                                                if (((FLOAT)nCustomValue27 > fltMax) || ((FLOAT)nCustomValue27 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue27;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue27 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue27 > fltMax) || (fltCustomValue27 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue27;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE27, &value);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_28:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCustomValue28Supported)
                                        {
                                            m_DeviceProperties.fCustomValue28Supported = TRUE;

                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_VALUE28);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_VALUE28);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE28, &value);
                                            }
                                        }
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCustomValue28MaximumSupported,
                                                            m_DeviceProperties.fltCustomValue28Maximum,
                                                            m_DeviceProperties.fCustomMaximumSupported,
                                                            m_DeviceProperties.fltCustomMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCustomValue28MinimumSupported,
                                                            m_DeviceProperties.fltCustomValue28Minimum,
                                                            m_DeviceProperties.fCustomMinimumSupported,
                                                            m_DeviceProperties.fltCustomMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if (0 == UnitsExp)
                                            {
                                                nCustomValue28 = UsageUValue;
                                                if (((FLOAT)nCustomValue28 > fltMax) || ((FLOAT)nCustomValue28 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_UI4;
                                                    value.intVal = nCustomValue28;
                                                }
                                            }
                                            else
                                            {
                                                fltCustomValue28 = (FLOAT)ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                                if ((fltCustomValue28 > fltMax) || (fltCustomValue28 < fltMin))
                                                {
                                                    value.vt = VT_NULL;
                                                }
                                                else
                                                {
                                                    value.vt = VT_R4;
                                                    value.fltVal = fltCustomValue28;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        if (SUCCEEDED(hr))
                                        {
                                            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_VALUE28, &value);
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
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CCustom::UpdateCustomDeviceValues
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCustom::UpdateCustomPropertyValues(BYTE* pFeatureReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported)
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
                            ZeroMemory(UsageArray, HID_FEATURE_REPORT_STRING_MAX_LENGTH*2);
                            hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, UsageArray, HID_FEATURE_REPORT_STRING_MAX_LENGTH*2, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
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
                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                            //set the values for each of the axes
                                            m_DeviceProperties.fltCustomValue1Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue2Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue3Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue4Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue5Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue6Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue7Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue8Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue9Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue10Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue11Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue12Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue13Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue14Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue15Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue16Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue17Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue18Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue19Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue20Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue21Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue22Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue23Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue24Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue25Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue26Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue27Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                            m_DeviceProperties.fltCustomValue28Sensitivity = m_DeviceProperties.fltCustomSensitivity;
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomMaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                            //set the values for each of the axes
                                            m_DeviceProperties.fltCustomValue1Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue2Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue3Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue4Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue5Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue6Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue7Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue8Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue9Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue10Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue11Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue12Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue13Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue14Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue15Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue16Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue17Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue18Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue19Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue20Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue21Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue22Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue23Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue24Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue25Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue26Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue27Maximum = m_DeviceProperties.fltCustomMaximum;
                                            m_DeviceProperties.fltCustomValue28Maximum = m_DeviceProperties.fltCustomMaximum;
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomMinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                            //set the values for each of the axes
                                            m_DeviceProperties.fltCustomValue1Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue2Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue3Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue4Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue5Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue6Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue7Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue8Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue9Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue10Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue11Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue12Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue13Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue14Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue15Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue16Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue17Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue18Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue19Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue20Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue21Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue22Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue23Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue24Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue25Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue26Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue27Minimum = m_DeviceProperties.fltCustomMinimum;
                                            m_DeviceProperties.fltCustomValue28Minimum = m_DeviceProperties.fltCustomMinimum;
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomAccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                            //set the values for each of the axes
                                            m_DeviceProperties.fltCustomValue1Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue2Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue3Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue4Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue5Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue6Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue7Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue8Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue9Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue10Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue11Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue12Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue13Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue14Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue15Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue16Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue17Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue18Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue19Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue20Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue21Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue22Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue23Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue24Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue25Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue26Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue27Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                            m_DeviceProperties.fltCustomValue28Accuracy = m_DeviceProperties.fltCustomAccuracy;
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                            //set the values for each of the axes
                                            m_DeviceProperties.fltCustomValue1Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue2Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue3Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue4Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue5Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue6Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue7Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue8Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue9Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue10Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue11Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue12Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue13Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue14Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue15Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue16Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue17Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue18Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue19Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue20Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue21Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue22Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue23Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue24Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue25Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue26Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue27Resolution = m_DeviceProperties.fltCustomResolution;
                                            m_DeviceProperties.fltCustomValue28Resolution = m_DeviceProperties.fltCustomResolution;
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_1:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue1SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue1Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue1MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue1Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue1MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue1Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue1AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue1Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue1ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue1Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_2:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue2SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue2Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue2MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue2Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue2MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue2Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue2AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue2Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue2ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue2Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_3:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue3SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue3Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue3MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue3Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue3MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue3Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue3AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue3Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue3ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue3Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_4:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue4SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue4Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue4MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue4Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue4MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue4Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue4AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue4Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue4ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue4Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_5:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue5SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue5Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue5MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue5Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue5MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue5Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue5AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue5Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue5ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue5Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_6:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue6SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue6Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue6MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue6Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue6MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue6Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue6AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue6Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue6ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue6Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_7:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue7SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue7Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue7MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue7Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue7MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue7Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue7AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue7Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue7ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue7Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_8:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue8SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue8Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue8MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue8Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue8MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue8Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue8AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue8Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue8ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue8Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_9:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue9SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue9Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue9MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue9Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue9MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue9Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue9AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue9Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue9ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue9Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_10:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue10SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue10Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue10MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue10Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue10MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue10Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue10AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue10Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue10ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue10Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_11:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue11SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue11Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue11MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue11Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue11MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue11Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue11AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue11Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue11ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue11Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_12:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue12SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue12Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue12MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue12Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue12MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue12Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue12AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue12Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue12ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue12Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_13:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue13SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue13Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue13MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue13Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue13MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue13Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue13AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue13Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue13ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue13Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_14:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue14SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue14Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue14MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue14Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue14MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue14Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue14AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue14Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue14ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue14Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_15:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue15SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue15Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue15MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue15Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue15MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue15Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue15AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue15Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue15ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue15Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_16:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue16SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue16Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue16MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue16Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue16MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue16Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue16AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue16Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue16ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue16Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_17:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue17SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue17Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue17MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue17Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue17MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue17Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue17AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue17Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue17ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue17Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_18:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue18SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue18Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue18MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue18Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue18MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue18Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue18AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue18Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue18ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue18Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_19:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue19SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue19Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue19MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue19Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue19MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue19Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue19AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue19Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue19ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue19Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_20:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue20SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue20Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue20MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue20Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue20MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue20Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue20AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue20Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue20ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue20Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_21:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue21SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue21Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue21MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue21Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue21MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue21Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue21AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue21Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue21ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue21Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_22:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue22SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue22Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue22MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue22Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue22MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue22Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue22AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue22Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue22ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue22Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_23:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue23SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue23Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue23MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue23Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue23MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue23Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue23AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue23Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue23ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue23Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_24:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue24SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue24Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue24MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue24Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue24MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue24Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue24AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue24Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue24ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue24Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_25:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue25SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue25Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue25MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue25Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue25MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue25Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue25AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue25Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue25ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue25Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_26:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue26SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue26Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue26MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue26Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue26MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue26Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue26AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue26Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue26ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue26Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_27:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue27SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue27Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue27MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue27Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue27MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue27Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue27AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue27Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue27ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue27Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_28:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue28SensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue28Sensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue28MaximumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue28Maximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue28MinimumSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue28Minimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue28AccuracySupported = TRUE;
                                            m_DeviceProperties.fltCustomValue28Accuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fCustomValue28ResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCustomValue28Resolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE1))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue1SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue1Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_1,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE1,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue1Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE2))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue2SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue2Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_2,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE2,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue2Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE3))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue3SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue3Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_3,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE3,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue3Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE4))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue4SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue4Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_4,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE4,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue4Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE5))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue5SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue5Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_5,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE5,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue5Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE6))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue6SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue6Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_6,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE6,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue6Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE7))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue7SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue7Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_7,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE7,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue7Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE8))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue8SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue8Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_8,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE8,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue8Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE9))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue9SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue9Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_9,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE9,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue9Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE10))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue10SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue10Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_10,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE10,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue10Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE11))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue11SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue11Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_11,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE11,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue11Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE12))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue12SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue12Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_12,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE12,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue12Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE13))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue13SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue13Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_13,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE13,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue13Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE14))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue14SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue14Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_14,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE14,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue14Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE15))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue15SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue15Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_15,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE15,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue15Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE16))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue16SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue16Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_16,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE16,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue16Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE17))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue17SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue17Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_17,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE17,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue17Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE18))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue18SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue18Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_18,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE18,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue18Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE19))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue19SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue19Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_19,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE19,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue19Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE20))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue20SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue20Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_20,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE20,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue20Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE21))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue21SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue21Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_21,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE21,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue21Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE22))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue22SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue22Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_22,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE22,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue22Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE23))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue23SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue23Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_23,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE23,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue23Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE24))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue24SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue24Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_24,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE24,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue24Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE25))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue25SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue25Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_25,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE25,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue25Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE26))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue26SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue26Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_26,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE26,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue26Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE27))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue27SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue27Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_27,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE27,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue27Sensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE28))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCustomSensitivitySupported,
                                                                            m_DeviceProperties.fCustomValue28SensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                            m_DeviceProperties.fCustomValue28Supported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_28,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                            SENSOR_DATA_TYPE_CUSTOM_VALUE28,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCustomSensitivity, 
                                                                            &m_DeviceProperties.fltCustomValue28Sensitivity, 
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE1))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue1MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue1Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_1,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE1,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue1Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE2))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue2MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue2Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_2,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE2,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue2Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE3))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue3MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue3Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_3,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE3,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue3Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE4))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue4MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue4Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_4,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE4,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue4Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE5))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue5MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue5Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_5,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE5,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue5Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE6))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue6MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue6Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_6,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE6,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue6Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE7))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue7MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue7Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_7,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE7,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue7Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE8))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue8MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue8Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_8,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE8,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue8Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE9))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue9MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue9Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_9,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE9,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue9Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE10))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue10MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue10Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_10,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE10,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue10Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE11))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue11MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue11Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_11,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE11,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue11Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE12))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue12MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue12Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_12,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE12,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue12Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE13))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue13MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue13Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_13,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE13,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue13Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE14))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue14MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue14Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_14,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE14,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue14Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE15))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue15MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue15Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_15,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE15,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue15Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE16))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue16MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue16Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_16,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE16,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue16Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE17))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue17MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue17Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_17,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE17,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue17Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE18))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue18MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue18Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_18,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE18,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue18Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE19))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue19MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue19Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_19,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE19,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue19Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE20))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue20MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue20Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_20,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE20,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue20Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE21))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue21MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue21Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_21,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE21,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue21Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE22))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue22MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue22Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_22,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE22,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue22Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE23))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue23MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue23Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_23,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE23,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue23Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE24))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue24MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue24Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_24,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE24,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue24Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE25))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue25MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue25Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_25,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE25,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue25Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE26))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue26MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue26Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_26,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE26,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue26Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE27))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue27MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue27Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_27,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE27,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue27Maximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE28))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCustomMaximumSupported,
                                                                    m_DeviceProperties.fCustomValue28MaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue28Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_28,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE28,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCustomMaximum, 
                                                                    &m_DeviceProperties.fltCustomValue28Maximum, 
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE1))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue1MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue1Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_1,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE1,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue1Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE2))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue2MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue2Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_2,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE2,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue2Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE3))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue3MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue3Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_3,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE3,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue3Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE4))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue4MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue4Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_4,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE4,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue4Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE5))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue5MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue5Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_5,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE5,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue5Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE6))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue6MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue6Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_6,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE6,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue6Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE7))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue7MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue7Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_7,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE7,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue7Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE8))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue8MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue8Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_8,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE8,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue8Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE9))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue9MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue9Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_9,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE9,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue9Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE10))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue10MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue10Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_10,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE10,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue10Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE11))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue11MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue11Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_11,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE11,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue11Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE12))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue12MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue12Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_12,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE12,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue12Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE13))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue13MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue13Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_13,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE13,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue13Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE14))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue14MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue14Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_14,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE14,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue14Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE15))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue15MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue15Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_15,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE15,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue15Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE16))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue16MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue16Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_16,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE16,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue16Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE17))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue17MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue17Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_17,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE17,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue17Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE18))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue18MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue18Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_18,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE18,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue18Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE19))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue19MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue19Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_19,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE19,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue19Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE20))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue20MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue20Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_20,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE20,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue20Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE21))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue21MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue21Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_21,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE21,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue21Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE22))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue22MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue22Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_22,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE22,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue22Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE23))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue23MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue23Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_23,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE23,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue23Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE24))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue24MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue24Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_24,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE24,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue24Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE25))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue25MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue25Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_25,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE25,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue25Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE26))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue26MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue26Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_26,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE26,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue26Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE27))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue27MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue27Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_27,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE27,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue27Minimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE28))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCustomMinimumSupported,
                                                                    m_DeviceProperties.fCustomValue28MinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue28Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_28,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE28,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCustomMinimum, 
                                                                    &m_DeviceProperties.fltCustomValue28Minimum, 
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE1))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue1AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue1Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_1,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE1,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue1Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE2))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue2AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue2Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_2,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE2,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue2Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE3))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue3AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue3Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_3,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE3,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue3Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE4))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue4AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue4Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_4,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE4,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue4Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE5))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue5AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue5Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_5,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE5,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue5Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE6))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue6AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue6Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_6,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE6,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue6Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE7))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue7AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue7Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_7,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE7,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue7Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE8))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue8AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue8Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_8,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE8,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue8Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE9))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue9AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue9Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_9,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE9,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue9Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE10))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue10AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue10Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_10,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE10,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue10Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE11))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue11AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue11Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_11,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE11,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue11Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE12))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue12AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue12Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_12,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE12,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue12Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE13))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue13AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue13Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_13,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE13,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue13Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE14))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue14AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue14Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_14,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE14,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue14Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE15))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue15AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue15Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_15,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE15,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue15Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE16))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue16AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue16Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_16,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE16,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue16Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE17))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue17AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue17Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_17,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE17,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue17Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE18))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue18AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue18Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_18,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE18,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue18Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE19))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue19AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue19Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_19,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE19,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue19Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE20))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue20AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue20Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_20,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE20,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue20Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE21))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue21AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue21Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_21,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE21,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue21Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE22))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue22AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue22Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_22,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE22,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue22Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE23))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue23AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue23Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_23,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE23,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue23Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE24))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue24AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue24Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_24,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE24,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue24Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE25))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue25AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue25Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_25,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE25,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue25Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE26))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue26AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue26Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_26,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE26,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue26Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE27))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue27AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue27Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_27,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE27,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue27Accuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE28))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCustomAccuracySupported,
                                                                    m_DeviceProperties.fCustomValue28AccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue28Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_28,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE28,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCustomAccuracy, 
                                                                    &m_DeviceProperties.fltCustomValue28Accuracy, 
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE1))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue1ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue1Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_1,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE1,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue1Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE2))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue2ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue2Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_2,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE2,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue2Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE3))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue3ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue3Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_3,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE3,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue3Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE4))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue4ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue4Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_4,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE4,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue4Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE5))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue5ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue5Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_5,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE5,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue5Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE6))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue6ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue6Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_6,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE6,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue6Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE7))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue7ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue7Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_7,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE7,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue7Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE8))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue8ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue8Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_8,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE8,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue8Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE9))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue9ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue9Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_9,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE9,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue9Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE10))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue10ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue10Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_10,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE10,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue10Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE11))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue11ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue11Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_11,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE11,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue11Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE12))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue12ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue12Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_12,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE12,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue12Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE13))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue13ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue13Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_13,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE13,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue13Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE14))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue14ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue14Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_14,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE14,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue14Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE15))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue15ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue15Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_15,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE15,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue15Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE16))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue16ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue16Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_16,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE16,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue16Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE17))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue17ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue17Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_17,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE17,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue17Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE18))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue18ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue18Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_18,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE18,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue18Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE19))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue19ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue19Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_19,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE19,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue19Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE20))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue20ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue20Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_20,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE20,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue20Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE21))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue21ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue21Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_21,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE21,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue21Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE22))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue22ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue22Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_22,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE22,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue22Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE23))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue23ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue23Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_23,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE23,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue23Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE24))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue24ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue24Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_24,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE24,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue24Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE25))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue25ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue25Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_25,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE25,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue25Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE26))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue26ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue26Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_26,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE26,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue26Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE27))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue27ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue27Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_27,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE27,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue27Resolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_CUSTOM_VALUE28))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCustomResolutionSupported,
                                                                    m_DeviceProperties.fCustomValue28ResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE,
                                                                    m_DeviceProperties.fCustomValue28Supported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_28,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                    SENSOR_DATA_TYPE_CUSTOM_VALUE28,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCustomResolution, 
                                                                    &m_DeviceProperties.fltCustomValue28Resolution, 
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

