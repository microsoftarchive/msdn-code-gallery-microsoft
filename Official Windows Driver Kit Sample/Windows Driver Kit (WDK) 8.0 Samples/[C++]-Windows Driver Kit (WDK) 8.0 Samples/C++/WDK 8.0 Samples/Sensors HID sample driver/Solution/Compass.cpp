 /*

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


 Module:
      Compass.cpp

 Description:
      Implements the CCompass container class

--*/

#include "internal.h"
#include "SensorDdi.h"
#include "SensorManager.h"

#include "Sensor.h"
#include "Compass.h"

#include "Compass.tmh"


const PROPERTYKEY g_RequiredSupportedCompassProperties[] =
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

const PROPERTYKEY g_OptionalSupportedCompassProperties[] =
{
    SENSOR_PROPERTY_RANGE_MAXIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RANGE_MINIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_ACCURACY,                   //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RESOLUTION,                 //[VT_UNKNOWN], IPortableDeviceValues
};

const PROPERTYKEY g_RequiredSettableCompassProperties[] =
{
    SENSOR_PROPERTY_CHANGE_SENSITIVITY,         //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
};

const PROPERTYKEY g_SupportedCompass1dDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                                 //[VT_FILETIME]
    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES,   //[VT_R8]
};

const PROPERTYKEY g_SupportedCompass3dDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                                             //[VT_FILETIME]
    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES,   //[VT_R8]
};

const PROPERTYKEY g_OptionalCompassDataFields[] =
{
    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS,                  //[VT_R8]
    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS,                  //[VT_R8]
    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS,                  //[VT_R8]
    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES,       //[VT_R8]
    SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES,               //[VT_R8]
    SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES,                   //[VT_R8]
};

const PROPERTYKEY g_SupportedCompassEvents[] =
{
    SENSOR_EVENT_DATA_UPDATED, 0,
    SENSOR_EVENT_STATE_CHANGED, 0,
};

/////////////////////////////////////////////////////////////////////////
//
// CCompass::CCompass
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CCompass::CCompass()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CCompass::~CCompass
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CCompass::~CCompass()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CCompass::Initialize
//
// Initializes the PROPERTYKEY/PROPVARIANT values for
// the Supported Properties & Supported Data
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCompass::Initialize(
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

    strcpy_s(m_SensorName, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_COMPASS_TRACE_NAME);

    if(SUCCEEDED(hr))
    {
        hr = InitializeCompass();
    }

    if(SUCCEEDED(hr))
    {
        m_fSensorInitialized = TRUE;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CCompass::InitializeCompass
//
// Initializes the Compass PropertyKeys and DataFieldKeys 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCompass::InitializeCompass( )
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    ZeroMemory(&m_DeviceProperties, sizeof(m_DeviceProperties));

    hr = AddCompassPropertyKeys();

    if (SUCCEEDED(hr))
    {
        hr = AddCompassSettablePropertyKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = AddCompassDataFieldKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = SetCompassDefaultValues();
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CCompass::AddCompassPropertyKeys
//
// Copies the PROPERTYKEYS for Compass Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCompass::AddCompassPropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSupportedCompassProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSupportedCompassProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_RequiredSupportedCompassProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_OptionalSupportedCompassProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_OptionalSupportedCompassProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_OptionalSupportedCompassProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }
    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CCompass::AddCompassSettablePropertyKeys
//
// Copies the PROPERTYKEYS for Compass Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCompass::AddCompassSettablePropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSettableCompassProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSettableCompassProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSettableSensorProperties->Add(g_RequiredSettableCompassProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CCompass::AddCompassDataFieldKeys
//
// Copies the PROPERTYKEYS for Compass Supported DataFields 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCompass::AddCompassDataFieldKeys()
{
    HRESULT hr = S_OK;

    PROPVARIANT var;
    PropVariantInit(&var);

    switch(m_SensorUsage)
    {
    case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_1D:
        for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedCompass1dDataFields); dwIndex++)
        {
            // Initialize all the PROPERTYKEY values to VT_EMPTY
            hr = SetDataField(g_SupportedCompass1dDataFields[dwIndex], &var);

            // Also add the PROPERTYKEY to the list of supported data fields
            if(SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorDataFields->Add(g_SupportedCompass1dDataFields[dwIndex]);
            }
        }
        break;
    case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_3D:
        for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedCompass3dDataFields); dwIndex++)
        {
            // Initialize all the PROPERTYKEY values to VT_EMPTY
            hr = SetDataField(g_SupportedCompass3dDataFields[dwIndex], &var);

            // Also add the PROPERTYKEY to the list of supported data fields
            if(SUCCEEDED(hr))
            {
                hr = m_spSupportedSensorDataFields->Add(g_SupportedCompass3dDataFields[dwIndex]);
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
// CCompass::SetCompassDefaultValues
//
// Fills in default values for most Compass Properties and 
// Data Fields.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCompass::SetCompassDefaultValues()
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

    m_ulDefaultCurrentReportInterval = DEFAULT_COMPASS_CURRENT_REPORT_INTERVAL;
    m_ulDefaultMinimumReportInterval = DEFAULT_COMPASS_MIN_REPORT_INTERVAL;

    m_fltDefaultChangeSensitivity = DEFAULT_COMPASS_SENSITIVITY;

    m_fltDefaultRangeMaximum = DEFAULT_COMPASS_MAXIMUM;
    m_fltDefaultRangeMinimum = DEFAULT_COMPASS_MINIMUM;
    m_fltDefaultAccuracy = DEFAULT_COMPASS_ACCURACY;
    m_fltDefaultResolution = DEFAULT_COMPASS_RESOLUTION;

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, SENSOR_CATEGORY_ORIENTATION);
    }

    if(SUCCEEDED(hr))
    {
        switch(m_SensorUsage)
        {
        case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_1D:
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_COMPASS_1D);
            break;
        case HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_3D:
            hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_COMPASS_3D);
            break;
        default:
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Unsupported usage of compass sensor, hr = %!HRESULT!", hr);
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
            wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_COMPASS_NAME);
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, tempStr);
        }
        else
        {
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, m_pSensorManager->m_wszDeviceName);
        }
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_COMPASS_DESCRIPTION);
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

                        if (SUCCEEDED(hr))
                        {
                            if (SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES == datakey)
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
                            if (SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES == datakey)
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
                            if (SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES == datakey)
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
                            if (SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES == datakey)
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
                            if (SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES == datakey)
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

    if(SUCCEEDED(hr) && ((m_SensorUsage == HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_1D) || (m_SensorUsage == HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_3D)))
    {
        hr = m_spSensorDataFieldValues->SetValue(SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES, &value);
    }

    PropVariantClear( &value );

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CCompass::GetPropertyValuesForCompassObject
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
HRESULT CCompass::GetPropertyValuesForCompassObject(
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

    hr = GetPropertyValuesForSensorObject(wszObjectID, pKeys, pValues, SENSOR_COMPASS_NAME, SENSOR_CATEGORY_ORIENTATION, &fError);

    return (FALSE == fError) ? hr : S_FALSE;
}

/////////////////////////////////////////////////////////////////////////
//
// CCompass::ProcessCompassAsyncRead
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCompass::ProcessCompassAsyncRead( BYTE* buffer, ULONG length )
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
            USAGE  UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
            USHORT LinkCollection = 0;
            UCHAR  reportID = 0;
            USHORT ReportCount = 0;
            USAGE  Usage = 0;
            USHORT UsageDataModifier = 0;
            LONG   UsageValue = 0;
            ULONG  UsageUValue = 0;
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
                DOUBLE  dblCompass = 0.0;

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
                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_X_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCompassXAxisSupported)
                                        {
                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS, &value);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                m_DeviceProperties.fCompassXAxisSupported = TRUE;
                                            }
                                        }
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            dblCompass = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCompassXMaximumSupported,
                                                            m_DeviceProperties.fltCompassXMaximum,
                                                            m_DeviceProperties.fCompassMagneticMaximumSupported,
                                                            m_DeviceProperties.fltCompassMagneticMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCompassXMinimumSupported,
                                                            m_DeviceProperties.fltCompassXMinimum,
                                                            m_DeviceProperties.fCompassMagneticMinimumSupported,
                                                            m_DeviceProperties.fltCompassMagneticMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((dblCompass > fltMax) || (dblCompass < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R8;
                                                value.dblVal = dblCompass;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS, &value);
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Y_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCompassYAxisSupported)
                                        {
                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS, &value);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                m_DeviceProperties.fCompassYAxisSupported = TRUE;
                                            }
                                        }
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            dblCompass = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCompassYMaximumSupported,
                                                            m_DeviceProperties.fltCompassYMaximum,
                                                            m_DeviceProperties.fCompassMagneticMaximumSupported,
                                                            m_DeviceProperties.fltCompassMagneticMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCompassYMinimumSupported,
                                                            m_DeviceProperties.fltCompassYMinimum,
                                                            m_DeviceProperties.fCompassMagneticMinimumSupported,
                                                            m_DeviceProperties.fltCompassMagneticMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((dblCompass > fltMax) || (dblCompass < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R8;
                                                value.dblVal = dblCompass;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS, &value);
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Z_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCompassZAxisSupported)
                                        {
                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS, &value);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                m_DeviceProperties.fCompassZAxisSupported = TRUE;
                                            }
                                        }
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            dblCompass = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCompassZMaximumSupported,
                                                            m_DeviceProperties.fltCompassZMaximum,
                                                            m_DeviceProperties.fCompassMagneticMaximumSupported,
                                                            m_DeviceProperties.fltCompassMagneticMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCompassZMinimumSupported,
                                                            m_DeviceProperties.fltCompassZMinimum,
                                                            m_DeviceProperties.fCompassMagneticMinimumSupported,
                                                            m_DeviceProperties.fltCompassMagneticMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((dblCompass > fltMax) || (dblCompass < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R8;
                                                value.dblVal = dblCompass;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS, &value);
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_MAGNETIC_NORTH:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        m_DeviceProperties.fCompassCompensatedMagneticNorthSupported = TRUE;
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            dblCompass = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCompassCompensatedMagneticNorthMaximumSupported,
                                                            m_DeviceProperties.fltCompassCompensatedMagneticNorthMaximum,
                                                            m_DeviceProperties.fCompassHeadingMaximumSupported,
                                                            m_DeviceProperties.fltCompassHeadingMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCompassCompensatedMagneticNorthMinimumSupported,
                                                            m_DeviceProperties.fltCompassCompensatedMagneticNorthMinimum,
                                                            m_DeviceProperties.fCompassHeadingMinimumSupported,
                                                            m_DeviceProperties.fltCompassHeadingMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((dblCompass > fltMax) || (dblCompass < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R8;
                                                value.dblVal = dblCompass;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES, &value);
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_TRUE_NORTH:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCompassCompensatedTrueNorthSupported)
                                        {
                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES, &value);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                m_DeviceProperties.fCompassCompensatedTrueNorthSupported = TRUE;
                                            }
                                        }
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            dblCompass = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCompassCompensatedTrueNorthMaximumSupported,
                                                            m_DeviceProperties.fltCompassCompensatedTrueNorthMaximum,
                                                            m_DeviceProperties.fCompassHeadingMaximumSupported,
                                                            m_DeviceProperties.fltCompassHeadingMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCompassCompensatedTrueNorthMinimumSupported,
                                                            m_DeviceProperties.fltCompassCompensatedTrueNorthMinimum,
                                                            m_DeviceProperties.fCompassHeadingMinimumSupported,
                                                            m_DeviceProperties.fltCompassHeadingMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((dblCompass > fltMax) || (dblCompass < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R8;
                                                value.dblVal = dblCompass;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES, &value);
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_NORTH:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCompassMagneticNorthSupported)
                                        {
                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES, &value);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                m_DeviceProperties.fCompassMagneticNorthSupported = TRUE;
                                            }
                                        }
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            dblCompass = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCompassMagneticNorthMaximumSupported,
                                                            m_DeviceProperties.fltCompassMagneticNorthMaximum,
                                                            m_DeviceProperties.fCompassHeadingMaximumSupported,
                                                            m_DeviceProperties.fltCompassHeadingMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCompassMagneticNorthMinimumSupported,
                                                            m_DeviceProperties.fltCompassMagneticNorthMinimum,
                                                            m_DeviceProperties.fCompassHeadingMinimumSupported,
                                                            m_DeviceProperties.fltCompassHeadingMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((dblCompass > fltMax) || (dblCompass < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R8;
                                                value.dblVal = dblCompass;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES, &value);
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TRUE_NORTH:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE:
                                        if (FALSE == m_DeviceProperties.fCompassTrueNorthSupported)
                                        {
                                            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES);

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                value.vt = VT_EMPTY;
                                                hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES, &value);
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                m_DeviceProperties.fCompassTrueNorthSupported = TRUE;
                                            }
                                        }
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            dblCompass = ExtractDoubleFromUsageValue(m_InputValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

                                            FLOAT fltMax = GetRangeMaximumValue(
                                                            m_fltDefaultRangeMaximum,
                                                            m_DeviceProperties.fCompassTrueNorthMaximumSupported,
                                                            m_DeviceProperties.fltCompassTrueNorthMaximum,
                                                            m_DeviceProperties.fCompassHeadingMaximumSupported,
                                                            m_DeviceProperties.fltCompassHeadingMaximum,
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            m_DeviceProperties.fltGlobalMaximum);

                                            FLOAT fltMin = GetRangeMinimumValue( 
                                                            m_fltDefaultRangeMinimum,
                                                            m_DeviceProperties.fCompassTrueNorthMinimumSupported,
                                                            m_DeviceProperties.fltCompassTrueNorthMinimum,
                                                            m_DeviceProperties.fCompassHeadingMinimumSupported,
                                                            m_DeviceProperties.fltCompassHeadingMinimum,
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            m_DeviceProperties.fltGlobalMinimum);

                                            if ((dblCompass > fltMax) || (dblCompass < fltMin))
                                            {
                                                value.vt = VT_NULL;
                                            }
                                            else
                                            {
                                                value.vt = VT_R8;
                                                value.dblVal = dblCompass;
                                            }
                                        }
                                        else
                                        {
                                            value.vt = VT_EMPTY;
                                        }
                                        hr = SetDataField(SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES, &value);
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
// CCompass::UpdateCompassDeviceValues
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CCompass::UpdateCompassPropertyValues(BYTE* pFeatureReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported)
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
                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassMagneticSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCompassMagneticSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassMagneticMaximumSupported = TRUE;
                                            m_DeviceProperties.fltCompassMagneticMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassMagneticMinimumSupported = TRUE;
                                            m_DeviceProperties.fltCompassMagneticMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassMagneticAccuracySupported = TRUE;
                                            m_DeviceProperties.fltCompassMagneticAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassMagneticResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCompassMagneticResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassHeadingSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCompassHeadingSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassHeadingMaximumSupported = TRUE;
                                            m_DeviceProperties.fltCompassHeadingMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassHeadingMinimumSupported = TRUE;
                                            m_DeviceProperties.fltCompassHeadingMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassHeadingAccuracySupported = TRUE;
                                            m_DeviceProperties.fltCompassHeadingAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassHeadingResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCompassHeadingResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_X_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassXSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCompassXSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassXMaximumSupported = TRUE;
                                            m_DeviceProperties.fltCompassXMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassXMinimumSupported = TRUE;
                                            m_DeviceProperties.fltCompassXMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassXAccuracySupported = TRUE;
                                            m_DeviceProperties.fltCompassXAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassXResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCompassXResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Y_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassYSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCompassYSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassYMaximumSupported = TRUE;
                                            m_DeviceProperties.fltCompassYMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassYMinimumSupported = TRUE;
                                            m_DeviceProperties.fltCompassYMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassYAccuracySupported = TRUE;
                                            m_DeviceProperties.fltCompassYAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassYResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCompassYResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Z_AXIS:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassZSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCompassZSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassZMaximumSupported = TRUE;
                                            m_DeviceProperties.fltCompassZMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassZMinimumSupported = TRUE;
                                            m_DeviceProperties.fltCompassZMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassZAccuracySupported = TRUE;
                                            m_DeviceProperties.fltCompassZAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassZResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCompassZResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_MAGNETIC_NORTH:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassCompensatedMagneticNorthSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCompassCompensatedMagneticNorthSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassCompensatedMagneticNorthMaximumSupported = TRUE;
                                            m_DeviceProperties.fltCompassCompensatedMagneticNorthMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassCompensatedMagneticNorthMinimumSupported = TRUE;
                                            m_DeviceProperties.fltCompassCompensatedMagneticNorthMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassCompensatedMagneticNorthAccuracySupported = TRUE;
                                            m_DeviceProperties.fltCompassCompensatedMagneticNorthAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassCompensatedMagneticNorthResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCompassCompensatedMagneticNorthResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_TRUE_NORTH:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassCompensatedTrueNorthSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCompassCompensatedTrueNorthSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassCompensatedTrueNorthMaximumSupported = TRUE;
                                            m_DeviceProperties.fltCompassCompensatedTrueNorthMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassCompensatedTrueNorthMinimumSupported = TRUE;
                                            m_DeviceProperties.fltCompassCompensatedTrueNorthMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassCompensatedTrueNorthAccuracySupported = TRUE;
                                            m_DeviceProperties.fltCompassCompensatedTrueNorthAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassCompensatedTrueNorthResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCompassCompensatedTrueNorthResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_NORTH:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassMagneticNorthSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCompassMagneticNorthSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassMagneticNorthMaximumSupported = TRUE;
                                            m_DeviceProperties.fltCompassMagneticNorthMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassMagneticNorthMinimumSupported = TRUE;
                                            m_DeviceProperties.fltCompassMagneticNorthMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassMagneticNorthAccuracySupported = TRUE;
                                            m_DeviceProperties.fltCompassMagneticNorthAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassMagneticNorthResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCompassMagneticNorthResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    default:
                                        //modifier used is not supported for this data field
                                        break;
                                    }
                                    break;

                                case HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TRUE_NORTH:
                                    switch(UsageDataModifier)
                                    {
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassTrueNorthSensitivitySupported = TRUE;
                                            m_DeviceProperties.fltCompassTrueNorthSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassTrueNorthMaximumSupported = TRUE;
                                            m_DeviceProperties.fltCompassTrueNorthMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassTrueNorthMinimumSupported = TRUE;
                                            m_DeviceProperties.fltCompassTrueNorthMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassTrueNorthAccuracySupported = TRUE;
                                            m_DeviceProperties.fltCompassTrueNorthAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
                                        }
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                                        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE == Units) || (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED == Units))
                                        {
                                            m_DeviceProperties.fCompassTrueNorthResolutionSupported = TRUE;
                                            m_DeviceProperties.fltCompassTrueNorthResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCompassMagneticSensitivitySupported,
                                                                            m_DeviceProperties.fCompassXSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                            m_DeviceProperties.fCompassXAxisSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_X_AXIS,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                            SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCompassMagneticSensitivity, 
                                                                            &m_DeviceProperties.fltCompassXSensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCompassMagneticSensitivitySupported,
                                                                            m_DeviceProperties.fCompassYSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                            m_DeviceProperties.fCompassYAxisSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Y_AXIS,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                            SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCompassMagneticSensitivity, 
                                                                            &m_DeviceProperties.fltCompassYSensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCompassMagneticSensitivitySupported,
                                                                            m_DeviceProperties.fCompassZSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                            m_DeviceProperties.fCompassZAxisSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Z_AXIS,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                            SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCompassMagneticSensitivity, 
                                                                            &m_DeviceProperties.fltCompassZSensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCompassHeadingSensitivitySupported,
                                                                            m_DeviceProperties.fCompassCompensatedMagneticNorthSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                            m_DeviceProperties.fCompassCompensatedMagneticNorthSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_MAGNETIC_NORTH,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                            SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCompassMagneticSensitivity, 
                                                                            &m_DeviceProperties.fltCompassCompensatedMagneticNorthSensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCompassHeadingSensitivitySupported,
                                                                            m_DeviceProperties.fCompassCompensatedTrueNorthSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                            m_DeviceProperties.fCompassCompensatedTrueNorthSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_TRUE_NORTH,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                            SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCompassMagneticSensitivity, 
                                                                            &m_DeviceProperties.fltCompassCompensatedTrueNorthSensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCompassHeadingSensitivitySupported,
                                                                            m_DeviceProperties.fCompassMagneticNorthSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                            m_DeviceProperties.fCompassMagneticNorthSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_NORTH,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                            SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCompassMagneticSensitivity, 
                                                                            &m_DeviceProperties.fltCompassMagneticNorthSensitivity, 
                                                                            pFeatureReport, 
                                                                            uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES))
                                    {
                                        hr = HandleChangeSensitivityUpdate(
                                                                            reportID, 
                                                                            fSettableOnly,
                                                                            m_DeviceProperties.fGlobalSensitivitySupported,
                                                                            m_DeviceProperties.fCompassHeadingSensitivitySupported,
                                                                            m_DeviceProperties.fCompassTrueNorthSensitivitySupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                            m_DeviceProperties.fCompassTrueNorthSupported,
                                                                            HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TRUE_NORTH,
                                                                            HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                            SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES,
                                                                            dwDfIdx,
                                                                            &m_DeviceProperties.fltGlobalSensitivity, 
                                                                            &m_DeviceProperties.fltCompassMagneticSensitivity, 
                                                                            &m_DeviceProperties.fltCompassTrueNorthSensitivity, 
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCompassMagneticMaximumSupported,
                                                                    m_DeviceProperties.fCompassXMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassXAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_X_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMaximum, 
                                                                    &m_DeviceProperties.fltCompassXMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCompassMagneticMaximumSupported,
                                                                    m_DeviceProperties.fCompassYMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassYAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Y_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMaximum, 
                                                                    &m_DeviceProperties.fltCompassYMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCompassMagneticMaximumSupported,
                                                                    m_DeviceProperties.fCompassZMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassZAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Z_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMaximum, 
                                                                    &m_DeviceProperties.fltCompassZMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Maximum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCompassHeadingMaximumSupported,
                                                                    m_DeviceProperties.fCompassCompensatedMagneticNorthMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassCompensatedMagneticNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_MAGNETIC_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMaximum, 
                                                                    &m_DeviceProperties.fltCompassCompensatedMagneticNorthMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCompassHeadingMaximumSupported,
                                                                    m_DeviceProperties.fCompassCompensatedTrueNorthMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassCompensatedTrueNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_TRUE_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMaximum, 
                                                                    &m_DeviceProperties.fltCompassCompensatedTrueNorthMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCompassHeadingMaximumSupported,
                                                                    m_DeviceProperties.fCompassMagneticNorthMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassMagneticNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMaximum, 
                                                                    &m_DeviceProperties.fltCompassMagneticNorthMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES))
                                    {
                                        hr = HandleMaximumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMaximumSupported,
                                                                    m_DeviceProperties.fCompassHeadingMaximumSupported,
                                                                    m_DeviceProperties.fCompassTrueNorthMaximumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassTrueNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TRUE_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMaximum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMaximum, 
                                                                    &m_DeviceProperties.fltCompassTrueNorthMaximum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCompassMagneticMinimumSupported,
                                                                    m_DeviceProperties.fCompassXMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassXAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_X_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMinimum, 
                                                                    &m_DeviceProperties.fltCompassXMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCompassMagneticMinimumSupported,
                                                                    m_DeviceProperties.fCompassYMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassYAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Y_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMinimum, 
                                                                    &m_DeviceProperties.fltCompassYMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCompassMagneticMinimumSupported,
                                                                    m_DeviceProperties.fCompassZMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassZAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Z_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMinimum, 
                                                                    &m_DeviceProperties.fltCompassZMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Minimum in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCompassHeadingMinimumSupported,
                                                                    m_DeviceProperties.fCompassCompensatedMagneticNorthMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassCompensatedMagneticNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_MAGNETIC_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMinimum, 
                                                                    &m_DeviceProperties.fltCompassCompensatedMagneticNorthMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCompassHeadingMinimumSupported,
                                                                    m_DeviceProperties.fCompassCompensatedTrueNorthMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassCompensatedTrueNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_TRUE_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMinimum, 
                                                                    &m_DeviceProperties.fltCompassCompensatedTrueNorthMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCompassHeadingMinimumSupported,
                                                                    m_DeviceProperties.fCompassMagneticNorthMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassMagneticNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMinimum, 
                                                                    &m_DeviceProperties.fltCompassMagneticNorthMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES))
                                    {
                                        hr = HandleMinimumUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalMinimumSupported,
                                                                    m_DeviceProperties.fCompassHeadingMinimumSupported,
                                                                    m_DeviceProperties.fCompassTrueNorthMinimumSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassTrueNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TRUE_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalMinimum, 
                                                                    &m_DeviceProperties.fltCompassMagneticMinimum, 
                                                                    &m_DeviceProperties.fltCompassTrueNorthMinimum, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCompassMagneticAccuracySupported,
                                                                    m_DeviceProperties.fCompassXAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassXAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_X_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCompassMagneticAccuracy, 
                                                                    &m_DeviceProperties.fltCompassXAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCompassMagneticAccuracySupported,
                                                                    m_DeviceProperties.fCompassYAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassYAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Y_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCompassMagneticAccuracy, 
                                                                    &m_DeviceProperties.fltCompassYAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCompassMagneticAccuracySupported,
                                                                    m_DeviceProperties.fCompassZAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassZAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Z_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCompassMagneticAccuracy, 
                                                                    &m_DeviceProperties.fltCompassZAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Accuracy in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCompassHeadingAccuracySupported,
                                                                    m_DeviceProperties.fCompassCompensatedMagneticNorthAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassCompensatedMagneticNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_MAGNETIC_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCompassMagneticAccuracy, 
                                                                    &m_DeviceProperties.fltCompassCompensatedMagneticNorthAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCompassHeadingAccuracySupported,
                                                                    m_DeviceProperties.fCompassCompensatedTrueNorthAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassCompensatedTrueNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_TRUE_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCompassMagneticAccuracy, 
                                                                    &m_DeviceProperties.fltCompassCompensatedTrueNorthAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCompassHeadingAccuracySupported,
                                                                    m_DeviceProperties.fCompassMagneticNorthAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassMagneticNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCompassMagneticAccuracy, 
                                                                    &m_DeviceProperties.fltCompassMagneticNorthAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES))
                                    {
                                        hr = HandleAccuracyUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalAccuracySupported,
                                                                    m_DeviceProperties.fCompassHeadingAccuracySupported,
                                                                    m_DeviceProperties.fCompassTrueNorthAccuracySupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassTrueNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TRUE_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalAccuracy, 
                                                                    &m_DeviceProperties.fltCompassMagneticAccuracy, 
                                                                    &m_DeviceProperties.fltCompassTrueNorthAccuracy, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
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
                                    if  (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCompassMagneticResolutionSupported,
                                                                    m_DeviceProperties.fCompassXResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassXAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_X_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCompassMagneticResolution, 
                                                                    &m_DeviceProperties.fltCompassXResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCompassMagneticResolutionSupported,
                                                                    m_DeviceProperties.fCompassYResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassYAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Y_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCompassMagneticResolution, 
                                                                    &m_DeviceProperties.fltCompassYResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCompassMagneticResolutionSupported,
                                                                    m_DeviceProperties.fCompassZResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX,
                                                                    m_DeviceProperties.fCompassZAxisSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Z_AXIS,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCompassMagneticResolution, 
                                                                    &m_DeviceProperties.fltCompassZResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Resolution in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCompassHeadingResolutionSupported,
                                                                    m_DeviceProperties.fCompassCompensatedMagneticNorthResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassCompensatedMagneticNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_MAGNETIC_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCompassMagneticResolution, 
                                                                    &m_DeviceProperties.fltCompassCompensatedMagneticNorthResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCompassHeadingResolutionSupported,
                                                                    m_DeviceProperties.fCompassCompensatedTrueNorthResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassCompensatedTrueNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_TRUE_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCompassMagneticResolution, 
                                                                    &m_DeviceProperties.fltCompassCompensatedTrueNorthResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCompassHeadingResolutionSupported,
                                                                    m_DeviceProperties.fCompassMagneticNorthResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassMagneticNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCompassMagneticResolution, 
                                                                    &m_DeviceProperties.fltCompassMagneticNorthResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
                                        }
                                    }
                                    else if (TRUE == IsEqualPropertyKey(pkDfKey, SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES))
                                    {
                                        hr = HandleResolutionUpdate(
                                                                    reportID, 
                                                                    m_DeviceProperties.fGlobalResolutionSupported,
                                                                    m_DeviceProperties.fCompassHeadingResolutionSupported,
                                                                    m_DeviceProperties.fCompassTrueNorthResolutionSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING,
                                                                    m_DeviceProperties.fCompassTrueNorthSupported,
                                                                    HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TRUE_NORTH,
                                                                    HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE,
                                                                    SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES,
                                                                    dwDfIdx,
                                                                    &m_DeviceProperties.fltGlobalResolution, 
                                                                    &m_DeviceProperties.fltCompassMagneticResolution, 
                                                                    &m_DeviceProperties.fltCompassTrueNorthResolution, 
                                                                    pFeatureReport, 
                                                                    uReportSize);

                                        if (FAILED(hr))
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to Set Change Sensitivity in property update, hr = %!HRESULT!", hr);
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

