 /*

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


 Module:
      Gyrometer.cpp

 Description:
      Implements the CGyrometer container class

--*/

#include "internal.h"
#include "SensorDdi.h"
#include "SensorManager.h"

#include "Sensor.h"
#include "Gyrometer.h"

#include "Gyrometer.tmh"


const PROPERTYKEY g_RequiredSupportedGyrometerProperties[] =
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

const PROPERTYKEY g_OptionalSupportedGyrometerProperties[] =
{
    SENSOR_PROPERTY_RANGE_MAXIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RANGE_MINIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_ACCURACY,                   //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RESOLUTION,                 //[VT_UNKNOWN], IPortableDeviceValues
};

const PROPERTYKEY g_RequiredSettableGyrometerProperties[] =
{
    SENSOR_PROPERTY_CHANGE_SENSITIVITY,         //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
};

const PROPERTYKEY g_SupportedGyrometer1DDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                                 //[VT_FILETIME]
    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND,     //[VT_R8]
};

const PROPERTYKEY g_SupportedGyrometer2DDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                                 //[VT_FILETIME]
    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND,     //[VT_R8]
    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND,     //[VT_R8]
};

const PROPERTYKEY g_SupportedGyrometer3DDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                                 //[VT_FILETIME]
    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND,     //[VT_R8]
    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND,     //[VT_R8]
    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND,     //[VT_R8]
};

const PROPERTYKEY g_SupportedGyrometerEvents[] =
{
    SENSOR_EVENT_DATA_UPDATED, 0,
    SENSOR_EVENT_STATE_CHANGED, 0,
};

/////////////////////////////////////////////////////////////////////////
//
// CGyrometer::CGyrometer
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CGyrometer::CGyrometer()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CGyrometer::~CGyrometer
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CGyrometer::~CGyrometer()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CGyrometer::Initialize
//
// Initializes the PROPERTYKEY/PROPVARIANT values for
// the Supported Properties & Supported Data
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGyrometer::Initialize(
    _In_ SensorType sensType,
    _In_ ULONG sensUsage,
    _In_ USHORT sensLinkCollection,
    _In_ DWORD sensNum, 
    _In_ LPWSTR pwszManufacturer,
    _In_ LPWSTR pwszProduct,
    _In_ LPWSTR pwszSerialNumber,
    _In_ LPWSTR sensorID,
    _In_ CSensorManager* pSensorManager)
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

    strcpy_s(m_SensorName, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_GYROMETER_TRACE_NAME);

    if(SUCCEEDED(hr))
    {
        hr = InitializeGyrometer();
    }

    if(SUCCEEDED(hr))
    {
        m_fSensorInitialized = TRUE;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGyrometer::InitializeGyrometer
//
// Initializes the Gyrometer PropertyKeys and DataFieldKeys 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGyrometer::InitializeGyrometer( )
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    ZeroMemory(&m_DeviceProperties, sizeof(m_DeviceProperties));

    hr = AddGyrometerPropertyKeys();

    if (SUCCEEDED(hr))
    {
        hr = AddGyrometerSettablePropertyKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = AddGyrometerDataFieldKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = SetGyrometerDefaultValues();
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CGyrometer::AddGyrometerPropertyKeys
//
// Copies the PROPERTYKEYS for Gyrometer Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGyrometer::AddGyrometerPropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSupportedGyrometerProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSupportedGyrometerProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_RequiredSupportedGyrometerProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_OptionalSupportedGyrometerProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_OptionalSupportedGyrometerProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_OptionalSupportedGyrometerProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGyrometer::AddGyrometerSettablePropertyKeys
//
// Copies the PROPERTYKEYS for Gyrometer Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGyrometer::AddGyrometerSettablePropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSettableGyrometerProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSettableGyrometerProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSettableSensorProperties->Add(g_RequiredSettableGyrometerProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGyrometer::AddGyrometerDataFieldKeys
//
// Copies the PROPERTYKEYS for Gyrometer Supported DataFields 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGyrometer::AddGyrometerDataFieldKeys()
{
    HRESULT hr = S_OK;

    PROPVARIANT var;
    PropVariantInit(&var);

    switch(m_SensorUsage)
    {
    case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_1D:
        for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedGyrometer1DDataFields); dwIndex++)
        {
            // Initialize all the PROPERTYKEY values to VT_EMPTY
            hr = SetDataField(g_SupportedGyrometer1DDataFields[dwIndex], &var);

            // Also add the PROPERTYKEY to the list of supported data fields
            if(SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorDataFields->Add(g_SupportedGyrometer1DDataFields[dwIndex]);
            }
        }
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_2D:
        for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedGyrometer2DDataFields); dwIndex++)
        {
            // Initialize all the PROPERTYKEY values to VT_EMPTY
            hr = SetDataField(g_SupportedGyrometer2DDataFields[dwIndex], &var);

            // Also add the PROPERTYKEY to the list of supported data fields
            if(SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorDataFields->Add(g_SupportedGyrometer2DDataFields[dwIndex]);
            }
        }
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_3D:
        for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedGyrometer3DDataFields); dwIndex++)
        {
            // Initialize all the PROPERTYKEY values to VT_EMPTY
            hr = SetDataField(g_SupportedGyrometer3DDataFields[dwIndex], &var);

            // Also add the PROPERTYKEY to the list of supported data fields
            if(SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorDataFields->Add(g_SupportedGyrometer3DDataFields[dwIndex]);
            }
        }
        break;
    default:
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "Unsupported usage of supported sensor, hr = %!HRESULT!", hr);
        break;
    }

    PropVariantClear(&var);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGyrometer::SetGyrometerDefaultValues
//
// Fills in default values for most Gyrometer Properties and 
// Data Fields.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGyrometer::SetGyrometerDefaultValues()
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

    m_ulDefaultCurrentReportInterval = DEFAULT_GYROMETER_CURRENT_REPORT_INTERVAL;
    m_ulDefaultMinimumReportInterval = DEFAULT_GYROMETER_MIN_REPORT_INTERVAL;

    m_fltDefaultChangeSensitivity = DEFAULT_GYROMETER_SENSITIVITY;

    m_fltDefaultRangeMaximum = DEFAULT_GYROMETER_MAXIMUM;
    m_fltDefaultRangeMinimum = DEFAULT_GYROMETER_MINIMUM;
    m_fltDefaultAccuracy = DEFAULT_GYROMETER_ACCURACY;
    m_fltDefaultResolution = DEFAULT_GYROMETER_RESOLUTION;

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, SENSOR_CATEGORY_MOTION);
    }

    if(SUCCEEDED(hr))
    {
        switch(m_SensorUsage)
        {
        case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_1D:
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_GYROMETER_1D);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_2D:
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_GYROMETER_2D);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_3D:
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_GYROMETER_3D);
            break;
        default:
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Unsupported usage of gyrometer sensor, hr = %!HRESULT!", hr);
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
            wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_GYROMETER_NAME);
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, tempStr);
        }
        else
        {
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, m_pSensorManager->m_wszDeviceName);
        }
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_GYROMETER_DESCRIPTION);
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
                            if ((SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND == datakey) ||
                                (SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND == datakey) ||
                                (SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND == datakey))
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
                            if ((SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND == datakey) ||
                                (SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND == datakey) ||
                                (SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND == datakey))
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
                            if ((SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND == datakey) ||
                                (SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND == datakey) ||
                                (SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND == datakey))
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
                            if ((SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND == datakey) ||
                                (SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND == datakey) ||
                                (SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND == datakey))
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
                            if ((SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND == datakey) ||
                                (SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND == datakey) ||
                                (SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND == datakey))
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
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND, &value);

    if(SUCCEEDED(hr) && ((m_SensorUsage == HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_2D) || (m_SensorUsage == HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_3D)))
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND, &value);

    if(SUCCEEDED(hr) && (m_SensorUsage == HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_3D))
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND, &value);

    PropVariantClear( &value );

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CGyrometer::GetPropertyValuesForGyrometerObject
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
HRESULT CGyrometer::GetPropertyValuesForGyrometerObject(
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

    hr = GetPropertyValuesForSensorObject(wszObjectID, pKeys, pValues, SENSOR_GYROMETER_NAME, SENSOR_CATEGORY_MOTION, &fError);

    return (FALSE == fError) ? hr : S_FALSE;
}

/////////////////////////////////////////////////////////////////////////
//
// CGyrometer::ProcessGyrometerAsyncRead
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGyrometer::ProcessGyrometerAsyncRead( BYTE* buffer, ULONG length )
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
                DOUBLE dblGyrometerX = 0.0F, dblGyrometerY = 0.0F, dblGyrometerZ = 0.0F;

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
                        UnitsExp = m_InputValueCapsNodes[idx].UnitsExp;
                        BitSize = m_InputValueCapsNodes[idx].BitSize;
                        Units = m_InputValueCapsNodes[idx].Units;
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
                                case HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_X_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        m_DeviceProperties.fGyrometerXAxisSupported = TRUE;
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            dblGyrometerX = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fGyrometerXMaximumSupported,
                                                            m_DeviceProperties.fltGyrometerXMaximum,
                                                            m_DeviceProperties.fGyrometerMaximumSupported,
                                                            m_DeviceProperties.fltGyrometerMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fGyrometerXMinimumSupported,
                                                            m_DeviceProperties.fltGyrometerXMinimum,
                                                            m_DeviceProperties.fGyrometerMinimumSupported,
                                                            m_DeviceProperties.fltGyrometerMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((dblGyrometerX > fltMax) || (dblGyrometerX < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R8;
                                                value.dblVal = dblGyrometerX;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND, &value);
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Y_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        m_DeviceProperties.fGyrometerYAxisSupported = TRUE;
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            dblGyrometerY = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fGyrometerYMaximumSupported,
                                                            m_DeviceProperties.fltGyrometerYMaximum,
                                                            m_DeviceProperties.fGyrometerMaximumSupported,
                                                            m_DeviceProperties.fltGyrometerMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fGyrometerYMinimumSupported,
                                                            m_DeviceProperties.fltGyrometerYMinimum,
                                                            m_DeviceProperties.fGyrometerMinimumSupported,
                                                            m_DeviceProperties.fltGyrometerMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((dblGyrometerY > fltMax) || (dblGyrometerY < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R8;
                                                value.dblVal = dblGyrometerY;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND, &value);
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;
                                case HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Z_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        m_DeviceProperties.fGyrometerZAxisSupported = TRUE;
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            dblGyrometerZ = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fGyrometerZMaximumSupported,
                                                            m_DeviceProperties.fltGyrometerZMaximum,
                                                            m_DeviceProperties.fGyrometerMaximumSupported,
                                                            m_DeviceProperties.fltGyrometerMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fGyrometerZMinimumSupported,
                                                            m_DeviceProperties.fltGyrometerZMinimum,
                                                            m_DeviceProperties.fGyrometerMinimumSupported,
                                                            m_DeviceProperties.fltGyrometerMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((dblGyrometerZ > fltMax) || (dblGyrometerZ < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R8;
                                                value.dblVal = dblGyrometerZ;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND, &value);
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
// CGyrometer::UpdateGyrometerDeviceValues
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGyrometer::UpdateGyrometerPropertyValues(BYTE* pFeatureReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported)
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
                                case HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltGyrometerSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerMaximumSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerMinimumSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerAccuracySupported = TRUE;
                                            m_DeviceProperties.fltGyrometerAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerResolutionSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_X_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerXSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltGyrometerXSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerXMaximumSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerXMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerXMinimumSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerXMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerXAccuracySupported = TRUE;
                                            m_DeviceProperties.fltGyrometerXAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerXResolutionSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerXResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Y_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerYSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltGyrometerYSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerYMaximumSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerYMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerYMinimumSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerYMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerYAccuracySupported = TRUE;
                                            m_DeviceProperties.fltGyrometerYAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerYResolutionSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerYResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Z_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerZSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltGyrometerZSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerZMaximumSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerZMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerZMinimumSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerZMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerZAccuracySupported = TRUE;
                                            m_DeviceProperties.fltGyrometerZAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fGyrometerZResolutionSupported = TRUE;
                                            m_DeviceProperties.fltGyrometerZResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fGyrometerSensitivitySupported,
                                                                            m_DeviceProperties.fGyrometerXSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                            m_DeviceProperties.fGyrometerXAxisSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_X_AXIS,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                            SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltGyrometerSensitivity, 
                                                                            &m_DeviceProperties.fltGyrometerXSensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fGyrometerSensitivitySupported,
                                                                            m_DeviceProperties.fGyrometerYSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                            m_DeviceProperties.fGyrometerYAxisSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Y_AXIS,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                            SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltGyrometerSensitivity, 
                                                                            &m_DeviceProperties.fltGyrometerYSensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fGyrometerSensitivitySupported,
                                                                            m_DeviceProperties.fGyrometerZSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                            m_DeviceProperties.fGyrometerZAxisSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Z_AXIS,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                            SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltGyrometerSensitivity, 
                                                                            &m_DeviceProperties.fltGyrometerZSensitivity, 
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fGyrometerMaximumSupported,
                                                                    m_DeviceProperties.fGyrometerXMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerXAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_X_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltGyrometerMaximum, 
                                                                    &m_DeviceProperties.fltGyrometerXMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fGyrometerMaximumSupported,
                                                                    m_DeviceProperties.fGyrometerYMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerYAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Y_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltGyrometerMaximum, 
                                                                    &m_DeviceProperties.fltGyrometerYMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fGyrometerMaximumSupported,
                                                                    m_DeviceProperties.fGyrometerZMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerZAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Z_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltGyrometerMaximum, 
                                                                    &m_DeviceProperties.fltGyrometerZMaximum, 
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fGyrometerMinimumSupported,
                                                                    m_DeviceProperties.fGyrometerXMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerXAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_X_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltGyrometerMinimum, 
                                                                    &m_DeviceProperties.fltGyrometerXMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fGyrometerMinimumSupported,
                                                                    m_DeviceProperties.fGyrometerYMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerYAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Y_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltGyrometerMinimum, 
                                                                    &m_DeviceProperties.fltGyrometerYMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fGyrometerMinimumSupported,
                                                                    m_DeviceProperties.fGyrometerZMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerZAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Z_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltGyrometerMinimum, 
                                                                    &m_DeviceProperties.fltGyrometerZMinimum, 
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fGyrometerAccuracySupported,
                                                                    m_DeviceProperties.fGyrometerXAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerXAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_X_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltGyrometerAccuracy, 
                                                                    &m_DeviceProperties.fltGyrometerXAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fGyrometerAccuracySupported,
                                                                    m_DeviceProperties.fGyrometerYAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerYAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Y_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltGyrometerAccuracy, 
                                                                    &m_DeviceProperties.fltGyrometerYAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fGyrometerAccuracySupported,
                                                                    m_DeviceProperties.fGyrometerZAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerZAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Z_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltGyrometerAccuracy, 
                                                                    &m_DeviceProperties.fltGyrometerZAccuracy, 
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fGyrometerResolutionSupported,
                                                                    m_DeviceProperties.fGyrometerXResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerXAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_X_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltGyrometerResolution, 
                                                                    &m_DeviceProperties.fltGyrometerXResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fGyrometerResolutionSupported,
                                                                    m_DeviceProperties.fGyrometerYResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerYAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Y_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltGyrometerResolution, 
                                                                    &m_DeviceProperties.fltGyrometerYResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fGyrometerResolutionSupported,
                                                                    m_DeviceProperties.fGyrometerZResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY,
                                                                    m_DeviceProperties.fGyrometerZAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Z_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND,
                                                                    SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltGyrometerResolution, 
                                                                    &m_DeviceProperties.fltGyrometerZResolution, 
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

