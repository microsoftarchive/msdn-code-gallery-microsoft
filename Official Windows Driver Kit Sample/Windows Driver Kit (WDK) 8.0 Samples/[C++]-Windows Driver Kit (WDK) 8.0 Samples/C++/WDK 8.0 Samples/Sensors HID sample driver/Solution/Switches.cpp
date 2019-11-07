 /*

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


 Module:
      Switches.cpp

 Description:
      Implements the CSwitch container class

--*/

#include "internal.h"
#include "SensorDdi.h"
#include "SensorManager.h"

#include "Sensor.h"
#include "Switches.h"

#include "Switches.tmh"


const PROPERTYKEY g_RequiredSupportedSwitchProperties[] =
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
    WPD_FUNCTIONAL_OBJECT_CATEGORY,             //[VT_CLSID]
};

const PROPERTYKEY g_OptionalSupportedSwitchMultivalProperties[] =
{
    SENSOR_PROPERTY_RANGE_MAXIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RANGE_MINIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
};

// Settable properties

const PROPERTYKEY g_RequiredSettableSwitchProperties[] =
{
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
};

const PROPERTYKEY g_RequiredSettableSwitchArrayProperties[] =
{
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
};

const PROPERTYKEY g_RequiredSettableSwitchMultivalProperties[] =
{
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
};

const PROPERTYKEY g_RequiredSettableTouchProperties[] =
{
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
};

const PROPERTYKEY g_RequiredSettableMotionProperties[] =
{
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
};

// datafields

const PROPERTYKEY g_SupportedSwitchDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                 //[VT_FILETIME]
    SENSOR_DATA_TYPE_BOOLEAN_SWITCH_STATE,      //[VT_BOOL]
};

const PROPERTYKEY g_SupportedSwitchArrayDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                     //[VT_FILETIME]
    SENSOR_DATA_TYPE_BOOLEAN_SWITCH_ARRAY_STATES,   //[VT_UI4]
};

const PROPERTYKEY g_SupportedSwitchMultivalDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                 //[VT_FILETIME]
    SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE,   //[VT_R8]
};

const PROPERTYKEY g_SupportedTouchDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                 //[VT_FILETIME]
    SENSOR_DATA_TYPE_TOUCH_STATE,               //[VT_BOOL]
};

const PROPERTYKEY g_SupportedMotionDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                 //[VT_FILETIME]
    SENSOR_DATA_TYPE_MOTION_STATE,              //[VT_BOOL]
};

const PROPERTYKEY g_SupportedSwitchEvents[] =
{
    SENSOR_EVENT_DATA_UPDATED, 0,
    SENSOR_EVENT_STATE_CHANGED, 0,
};

/////////////////////////////////////////////////////////////////////////
//
// CSwitch::CSwitch
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CSwitch::CSwitch()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CSwitch::~CSwitch
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CSwitch::~CSwitch()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CSwitch::Initialize
//
// Initializes the PROPERTYKEY/PROPVARIANT values for
// the Supported Properties & Supported Data
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSwitch::Initialize(
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

    switch(m_SensorUsage)
    {
    case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH:
        strcpy_s(m_SensorName, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_SWITCH_TRACE_NAME);
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY:
        strcpy_s(m_SensorName, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_SWITCH_ARRAY_TRACE_NAME);
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH:
        strcpy_s(m_SensorName, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_SWITCH_MULTIVAL_TRACE_NAME);
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR:
        strcpy_s(m_SensorName, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_MOTION_TRACE_NAME);
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH:
        strcpy_s(m_SensorName, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_TOUCH_TRACE_NAME);
        break;
    default:
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "Unsupported usage of switch sensor, Usage = %i, hr = %!HRESULT!", m_SensorUsage, hr);
        break;
    }

    if(SUCCEEDED(hr))
    {
        hr = InitializeSwitch();
    }

    if(SUCCEEDED(hr))
    {
        m_fSensorInitialized = TRUE;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSwitch::InitializeSwitch
//
// Initializes the Switch PropertyKeys and DataFieldKeys 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSwitch::InitializeSwitch( )
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    ZeroMemory(&m_DeviceProperties, sizeof(m_DeviceProperties));

    hr = AddSwitchPropertyKeys();

    if (SUCCEEDED(hr))
    {
        hr = AddSwitchSettablePropertyKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = AddSwitchDataFieldKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = SetSwitchDefaultValues();
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CSwitch::AddSwitchPropertyKeys
//
// Copies the PROPERTYKEYS for Switch Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSwitch::AddSwitchPropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSupportedSwitchProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSupportedSwitchProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_RequiredSupportedSwitchProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    if ((SUCCEEDED(hr) && (HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH == m_SensorUsage)))
    {
        for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_OptionalSupportedSwitchMultivalProperties); dwIndex++)
        {
            PROPVARIANT var;
            PropVariantInit(&var);

            // Initialize all the PROPERTYKEY values to VT_EMPTY
            hr = SetProperty(g_OptionalSupportedSwitchMultivalProperties[dwIndex], &var, nullptr);

            // Also add the PROPERTYKEY to the list of supported properties
            if(SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorProperties->Add(g_OptionalSupportedSwitchMultivalProperties[dwIndex]);
            }

            PropVariantClear(&var);
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSwitch::AddSwitchSettablePropertyKeys
//
// Copies the PROPERTYKEYS for Switch Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSwitch::AddSwitchSettablePropertyKeys()
{
    HRESULT hr = S_OK;

    if(SUCCEEDED(hr))
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        switch(m_SensorUsage)
        {
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH:
            for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSettableSwitchProperties); dwIndex++)
            {
                // Initialize all the PROPERTYKEY values to VT_EMPTY
                hr = SetProperty(g_RequiredSettableSwitchProperties[dwIndex], &var, nullptr);

                // Also add the PROPERTYKEY to the list of supported properties
                if(SUCCEEDED(hr))
                {
                    hr = m_spSettableSensorProperties->Add(g_RequiredSettableSwitchProperties[dwIndex]);
                }

            }
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY:
            for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSettableSwitchArrayProperties); dwIndex++)
            {
                // Initialize all the PROPERTYKEY values to VT_EMPTY
                hr = SetProperty(g_RequiredSettableSwitchArrayProperties[dwIndex], &var, nullptr);

                // Also add the PROPERTYKEY to the list of supported properties
                if(SUCCEEDED(hr))
                {
                    hr = m_spSettableSensorProperties->Add(g_RequiredSettableSwitchArrayProperties[dwIndex]);
                }

            }
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH:
            for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSettableSwitchMultivalProperties); dwIndex++)
            {
                // Initialize all the PROPERTYKEY values to VT_EMPTY
                hr = SetProperty(g_RequiredSettableSwitchMultivalProperties[dwIndex], &var, nullptr);

                // Also add the PROPERTYKEY to the list of supported properties
                if(SUCCEEDED(hr))
                {
                    hr = m_spSettableSensorProperties->Add(g_RequiredSettableSwitchMultivalProperties[dwIndex]);
                }

            }
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR:
            for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSettableMotionProperties); dwIndex++)
            {
                // Initialize all the PROPERTYKEY values to VT_EMPTY
                hr = SetProperty(g_RequiredSettableMotionProperties[dwIndex], &var, nullptr);

                // Also add the PROPERTYKEY to the list of supported properties
                if(SUCCEEDED(hr))
                {
                    hr = m_spSettableSensorProperties->Add(g_RequiredSettableMotionProperties[dwIndex]);
                }

            }
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH:
            for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSettableTouchProperties); dwIndex++)
            {
                // Initialize all the PROPERTYKEY values to VT_EMPTY
                hr = SetProperty(g_RequiredSettableTouchProperties[dwIndex], &var, nullptr);

                // Also add the PROPERTYKEY to the list of supported properties
                if(SUCCEEDED(hr))
                {
                    hr = m_spSettableSensorProperties->Add(g_RequiredSettableTouchProperties[dwIndex]);
                }

            }
            break;
        default:
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Unsupported usage of switch sensor, hr = %!HRESULT!", hr);
            break;
        }
        
        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSwitch::AddSwitchDataFieldKeys
//
// Copies the PROPERTYKEYS for Switch Supported DataFields 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSwitch::AddSwitchDataFieldKeys()
{
    HRESULT hr = S_OK;

    if(SUCCEEDED(hr))
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        switch(m_SensorUsage)
        {
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH:
            for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedSwitchDataFields); dwIndex++)
            {
                // Initialize all the PROPERTYKEY values to VT_EMPTY
                hr = SetProperty(g_SupportedSwitchDataFields[dwIndex], &var, nullptr);

                // Also add the PROPERTYKEY to the list of supported properties
                if(SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->Add(g_SupportedSwitchDataFields[dwIndex]);
                }

            }
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY:
            for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedSwitchArrayDataFields); dwIndex++)
            {
                // Initialize all the PROPERTYKEY values to VT_EMPTY
                hr = SetProperty(g_SupportedSwitchArrayDataFields[dwIndex], &var, nullptr);

                // Also add the PROPERTYKEY to the list of supported properties
                if(SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->Add(g_SupportedSwitchArrayDataFields[dwIndex]);
                }

            }
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH:
            for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedSwitchMultivalDataFields); dwIndex++)
            {
                // Initialize all the PROPERTYKEY values to VT_EMPTY
                hr = SetProperty(g_SupportedSwitchMultivalDataFields[dwIndex], &var, nullptr);

                // Also add the PROPERTYKEY to the list of supported properties
                if(SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->Add(g_SupportedSwitchMultivalDataFields[dwIndex]);
                }

            }
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR:
            for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedMotionDataFields); dwIndex++)
            {
                // Initialize all the PROPERTYKEY values to VT_EMPTY
                hr = SetProperty(g_SupportedMotionDataFields[dwIndex], &var, nullptr);

                // Also add the PROPERTYKEY to the list of supported properties
                if(SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->Add(g_SupportedMotionDataFields[dwIndex]);
                }

            }
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH:
            for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedTouchDataFields); dwIndex++)
            {
                // Initialize all the PROPERTYKEY values to VT_EMPTY
                hr = SetProperty(g_SupportedTouchDataFields[dwIndex], &var, nullptr);

                // Also add the PROPERTYKEY to the list of supported properties
                if(SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->Add(g_SupportedTouchDataFields[dwIndex]);
                }

            }
            break;
        default:
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Unsupported usage of switch sensor, hr = %!HRESULT!", hr);
            break;
        }
        
        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSwitch::SetSwitchDefaultValues
//
// Fills in default values for most Switch Properties and 
// Data Fields.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSwitch::SetSwitchDefaultValues()
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

    m_ulDefaultCurrentReportInterval = DEFAULT_SWITCH_CURRENT_REPORT_INTERVAL;
    m_ulDefaultMinimumReportInterval = DEFAULT_SWITCH_MIN_REPORT_INTERVAL;

    m_fltDefaultChangeSensitivity = DEFAULT_SWITCH_SENSITIVITY;

    m_fltDefaultRangeMaximum = DEFAULT_SWITCH_MAXIMUM;
    m_fltDefaultRangeMinimum = DEFAULT_SWITCH_MINIMUM;
    m_fltDefaultAccuracy = DEFAULT_SWITCH_ACCURACY;
    m_fltDefaultResolution = DEFAULT_SWITCH_RESOLUTION;

    if(SUCCEEDED(hr))
    {
        switch(m_SensorUsage)
        {
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH:
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY:
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH:
            hr = m_spSensorPropertyValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, SENSOR_CATEGORY_MECHANICAL);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR:
            hr = m_spSensorPropertyValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, SENSOR_CATEGORY_MOTION);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH:
            hr = m_spSensorPropertyValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, SENSOR_CATEGORY_BIOMETRIC);
            break;
        default:
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Unsupported usage of switch sensor, hr = %!HRESULT!", hr);
            break;
        }
    }

    if(SUCCEEDED(hr))
    {
        switch(m_SensorUsage)
        {
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH:
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_BOOLEAN_SWITCH);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY:
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_BOOLEAN_SWITCH_ARRAY);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH:
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_MULTIVALUE_SWITCH);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR:
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_MOTION_DETECTOR);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH:
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_TOUCH);
            break;
        default:
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Unsupported usage of switch sensor, hr = %!HRESULT!", hr);
            break;
        }
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
            switch (m_SensorUsage)
            {
            case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH:
                wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_SWITCH_NAME);
                break;
            case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY:
                wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_SWITCH_ARRAY_NAME);
                break;
            case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH:
                wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_SWITCH_MULTIVAL_NAME);
                break;
            case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR:
                wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_MOTION_NAME);
                break;
            case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH:
                wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_TOUCH_NAME);
                break;
            default:
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Unsupported usage of switch sensor, hr = %!HRESULT!", hr);
                break;
            }
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, tempStr);
        }
        else
        {
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, m_pSensorManager->m_wszDeviceName);
        }
    }

    if(SUCCEEDED(hr))
    {
        switch(m_SensorUsage)
        {
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH:
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_SWITCH_DESCRIPTION);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY:
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_SWITCH_ARRAY_DESCRIPTION);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH:
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_SWITCH_MULTIVAL_DESCRIPTION);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR:
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_MOTION_DESCRIPTION);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH:
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_TOUCH_DESCRIPTION);
            break;
        default:
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Unsupported usage of switch sensor, hr = %!HRESULT!", hr);
            break;
        }
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CONNECTION_TYPE, SENSOR_CONNECTION_TYPE_PC_ATTACHED);
    }

    // *****************************************************************************************
    // Default values for SENSOR PROPERTIES
    // *****************************************************************************************

    DWORD dwPropertyCount = 0;
    PROPERTYKEY propkey;

    if (SUCCEEDED(hr))
    {
        hr = m_spSupportedSensorProperties->GetCount(&dwPropertyCount);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, m_ulDefaultCurrentReportInterval);
    }
    
    if (SUCCEEDED(hr))
    {
        for (DWORD i = 0; i < dwPropertyCount; i++)
        {
            if (SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorProperties->GetAt(i, &propkey);
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

                        if (SUCCEEDED(hr) && (SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE == datakey))
                        {
                            hr = spMaximumValues->SetFloatValue(SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE, m_fltDefaultRangeMaximum);
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

                        if (SUCCEEDED(hr) && (SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE == datakey))
                        {
                            hr = spMinimumValues->SetFloatValue(SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE, m_fltDefaultRangeMinimum);
                        }
                    }
                }
                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MINIMUM, spMinimumValues);
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
        switch(m_SensorUsage)
        {
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH:
            hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_BOOLEAN_SWITCH_STATE, &value);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY:
            hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_BOOLEAN_SWITCH_ARRAY_STATES, &value);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH:
            hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE, &value);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR:
            hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_MOTION_STATE, &value);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH:
            hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_TOUCH_STATE, &value);
            break;
        default:
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Unsupported usage of switch sensor, hr = %!HRESULT!", hr);
            break;
        }
    }

    PropVariantClear( &value );
    
    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CSwitch::GetPropertyValuesForSwitchObject
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
HRESULT CSwitch::GetPropertyValuesForSwitchObject(
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

    switch(m_SensorUsage)
    {
    case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH:
        hr = GetPropertyValuesForSensorObject(wszObjectID, pKeys, pValues, SENSOR_SWITCH_NAME, SENSOR_CATEGORY_MECHANICAL, &fError);
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY:
        hr = GetPropertyValuesForSensorObject(wszObjectID, pKeys, pValues, SENSOR_SWITCH_ARRAY_NAME, SENSOR_CATEGORY_MECHANICAL, &fError);
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH:
        hr = GetPropertyValuesForSensorObject(wszObjectID, pKeys, pValues, SENSOR_SWITCH_MULTIVAL_NAME, SENSOR_CATEGORY_MECHANICAL, &fError);
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR:
        hr = GetPropertyValuesForSensorObject(wszObjectID, pKeys, pValues, SENSOR_MOTION_NAME, SENSOR_CATEGORY_MOTION, &fError);
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH:
        hr = GetPropertyValuesForSensorObject(wszObjectID, pKeys, pValues, SENSOR_TOUCH_NAME, SENSOR_CATEGORY_BIOMETRIC, &fError);
        break;
    default:
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "Unsupported usage of presence/proximity sensor, hr = %!HRESULT!", hr);
        break;
    }

    return (FALSE == fError) ? hr : S_FALSE;
}

/////////////////////////////////////////////////////////////////////////
//
// CSwitch::ProcessSwitchAsyncRead
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSwitch::ProcessSwitchAsyncRead( BYTE* buffer, ULONG length )
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
            UCHAR  reportID = 0;
            USAGE  Usage = 0;
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

                USHORT  sensorState = SENSOR_STATE_NOT_AVAILABLE;
                USHORT  eventType = SENSOR_EVENT_TYPE_UNKNOWN;
                DWORD   dwSwitch = 0;
                DOUBLE  dblSwitch = 0.0F;

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
                        UnitsExp = m_InputValueCapsNodes[idx].UnitsExp;
                        BitSize = m_InputValueCapsNodes[idx].BitSize;
                        ReportCount = m_InputValueCapsNodes[idx].ReportCount;
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
                                FLOAT fltMax;
                                FLOAT fltMin;

                                switch(Usage)
                                {
                                case HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_BOOLEAN_SWITCH_STATE:
                                    m_DeviceProperties.fSwitchSupported = TRUE;
                                    value.vt = VT_BOOL;
                                    dwSwitch = (DWORD)UsageUValue;
                                    //force to bool
                                    dwSwitch = (dwSwitch) > 0 ? 1 : 0;
                                    value.boolVal = (dwSwitch > 0) ? VARIANT_TRUE : VARIANT_FALSE;
                                    hr = SetDataField(SENSOR_DATA_TYPE_BOOLEAN_SWITCH_STATE, &value);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_BOOLEAN_SWITCH_ARRAY_STATES:
                                    m_DeviceProperties.fSwitchArraySupported = TRUE;
                                    value.vt = VT_UI4;
                                    dwSwitch = (DWORD)UsageUValue;
                                    value.uintVal = dwSwitch;
                                    hr = SetDataField(SENSOR_DATA_TYPE_BOOLEAN_SWITCH_ARRAY_STATES, &value);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_MULTIVAL_SWITCH_VALUE:
                                    m_DeviceProperties.fSwitchMultivalSupported = TRUE;
                                    dblSwitch = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                    fltMax = GetRangeMaximumValue(
                                                    m_fltDefaultRangeMaximum,
                                                    m_DeviceProperties.fSwitchMaximumSupported,
                                                    m_DeviceProperties.fltSwitchMaximum,
                                                    m_DeviceProperties.fSwitchMaximumSupported,
                                                    m_DeviceProperties.fltSwitchMaximum,
                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                    m_DeviceProperties.fltGlobalMaximum);

                                    fltMin = GetRangeMinimumValue( 
                                                    m_fltDefaultRangeMinimum,
                                                    m_DeviceProperties.fSwitchMinimumSupported,
                                                    m_DeviceProperties.fltSwitchMinimum,
                                                    m_DeviceProperties.fSwitchMinimumSupported,
                                                    m_DeviceProperties.fltSwitchMinimum,
                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                    m_DeviceProperties.fltGlobalMinimum);

                                    if ((dblSwitch > fltMax) || (dblSwitch < fltMin))
                                    {
                                        value.vt = VT_NULL;
                                    }
                                    else
                                    {
                                        value.vt = VT_R8;
                                        value.dblVal = dblSwitch;
                                    }
                                    hr = SetDataField(SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE, &value);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_MOTION_INTENSITY:
                                    m_DeviceProperties.fSwitchMotionSupported = TRUE;
                                    value.vt = VT_BOOL;
                                    dwSwitch = (DWORD)UsageUValue;
                                    //force to bool
                                    dwSwitch = (dwSwitch) > 0 ? 1 : 0;
                                    value.boolVal = (dwSwitch > 0) ? VARIANT_TRUE : VARIANT_FALSE;
                                    hr = SetDataField(SENSOR_DATA_TYPE_MOTION_STATE, &value);
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC_HUMAN_TOUCH_STATE:
                                    m_DeviceProperties.fSwitchTouchSupported = TRUE;
                                    value.vt = VT_BOOL;
                                    dwSwitch = (DWORD)UsageUValue;
                                    //force to bool
                                    dwSwitch = (dwSwitch) > 0 ? 1 : 0;
                                    value.boolVal = (dwSwitch > 0) ? VARIANT_TRUE : VARIANT_FALSE;
                                    hr = SetDataField(SENSOR_DATA_TYPE_TOUCH_STATE, &value);
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
// CSwitch::UpdateSwitchDeviceValues
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSwitch::UpdateSwitchPropertyValues(BYTE* pFeatureReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported)
{
    UNREFERENCED_PARAMETER(fSettableOnly);

    DWORD   cValues = 0;
    HRESULT hr = m_spSupportedSensorProperties->GetCount(&cValues);
    UCHAR   reportID = 0;
    ULONG   uReportSize = m_pSensorManager->m_HidCaps.FeatureReportByteLength;
    
    HIDP_REPORT_TYPE ReportType = HidP_Feature;
    USAGE  UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
    USHORT LinkCollection = 0;
    LONG   UsageValue = 0;
    ULONG  UsageUValue = 0;
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
            USAGE Usage = 0;
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
                                case HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_BOOLEAN_SWITCH_STATE:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY: //not supported
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION: //not supported
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_MULTIVAL_SWITCH_VALUE:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fSwitchMaximumSupported = TRUE;
                                            m_DeviceProperties.fltSwitchMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units)
                                        {
                                            m_DeviceProperties.fSwitchMinimumSupported = TRUE;
                                            m_DeviceProperties.fltSwitchMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY: //not supported
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION: //not supported
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_BOOLEAN_SWITCH_ARRAY_STATES:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY: //not supported
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION: //not supported
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC_HUMAN_TOUCH_STATE:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY: //not supported
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION: //not supported
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_MOTION_STATE:
                                case HID_DRIVER_USAGE_SENSOR_DATA_MOTION_INTENSITY:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY: //not supported
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION: //not supported
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

                
                    else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RANGE_MAXIMUM))
                    {
                        DWORD cDfKeys = 0;
                        PROPERTYKEY pkDfKey = {0};

                        hr = m_spSupportedSensorDataFields->GetCount(&cDfKeys);

                        if (SUCCEEDED(hr))
                        {
                            for (DWORD dwDfIdx = 1; dwDfIdx < cDfKeys; dwDfIdx++) //ignore the timestamp
                            {
                                hr = m_spSupportedSensorDataFields->GetAt(dwDfIdx, &pkDfKey);

                                if (SUCCEEDED(hr))
                                {
                                    if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                reportID, 
                                                                m_DeviceProperties.fGlobalMaximumSupported,
                                                                FALSE,
                                                                m_DeviceProperties.fSwitchMaximumSupported,
                                                                HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_MULTIVAL_SWITCH_VALUE,
                                                                m_DeviceProperties.fSwitchMaximumSupported,
                                                                HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_MULTIVAL_SWITCH_VALUE,
                                                                HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE,
                                                                dwDfIdx,
                                                                &m_DeviceProperties.fltGlobalMaximum, 
                                                                &m_DeviceProperties.fltSwitchMinimum, 
                                                                &m_DeviceProperties.fltSwitchMinimum, 
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
                            for (DWORD dwDfIdx = 1; dwDfIdx < cDfKeys; dwDfIdx++) //ignore the timestamp
                            {
                                hr = m_spSupportedSensorDataFields->GetAt(dwDfIdx, &pkDfKey);

                                if (SUCCEEDED(hr))
                                {
                                    if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                reportID, 
                                                                m_DeviceProperties.fGlobalMinimumSupported,
                                                                FALSE,
                                                                m_DeviceProperties.fSwitchMinimumSupported,
                                                                HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_MULTIVAL_SWITCH_VALUE,
                                                                m_DeviceProperties.fSwitchMinimumSupported,
                                                                HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_MULTIVAL_SWITCH_VALUE,
                                                                HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED,
                                                                SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE,
                                                                dwDfIdx,
                                                                &m_DeviceProperties.fltGlobalMinimum, 
                                                                &m_DeviceProperties.fltSwitchMinimum, 
                                                                &m_DeviceProperties.fltSwitchMinimum, 
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
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_CHANGE_SENSITIVITY))
                            //|| (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RANGE_MAXIMUM))
                            //|| (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RANGE_MINIMUM))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_ACCURACY))
                            || (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RESOLUTION))
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
