 /*

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved


 Module:
      Generic.cpp

 Description:
      Implements the CGeneric container class

--*/

#include "internal.h"
#include "SensorDdi.h"
#include "SensorManager.h"

#include "Sensor.h"
#include "Generic.h"

#include "Generic.tmh"


const PROPERTYKEY g_RequiredSupportedGenericProperties[] =
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

const PROPERTYKEY g_OptionalSupportedGenericProperties[] =
{
    SENSOR_PROPERTY_RANGE_MAXIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RANGE_MINIMUM,              //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_ACCURACY,                   //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_RESOLUTION,                 //[VT_UNKNOWN], IPortableDeviceValues
};

const PROPERTYKEY g_RequiredSettableGenericProperties[] =
{
    SENSOR_PROPERTY_CHANGE_SENSITIVITY,         //[VT_UNKNOWN], IPortableDeviceValues
    SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL,    //[VT_UI4]
};

const PROPERTYKEY g_SupportedGenericDataFields[] =
{
    SENSOR_DATA_TYPE_TIMESTAMP,                 //[VT_FILETIME]
};

const PROPERTYKEY g_SupportedGenericEvents[] =
{
    SENSOR_EVENT_DATA_UPDATED, 0,
    SENSOR_EVENT_STATE_CHANGED, 0,
};

/////////////////////////////////////////////////////////////////////////
//
// CGeneric::CGeneric
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CGeneric::CGeneric()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CGeneric::~CGeneric
//
// Object destructor function
//
/////////////////////////////////////////////////////////////////////////
CGeneric::~CGeneric()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CGeneric::Initialize
//
// Initializes the PROPERTYKEY/PROPVARIANT values for
// the Supported Properties & Supported Data
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeneric::Initialize(
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

    strcpy_s(m_SensorName, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_GENERIC_TRACE_NAME);

    if(SUCCEEDED(hr))
    {
        hr = InitializeGeneric();
    }

    if(SUCCEEDED(hr))
    {
        m_fSensorInitialized = TRUE;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeneric::InitializeGeneric
//
// Initializes the Generic PropertyKeys and DataFieldKeys 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeneric::InitializeGeneric( )
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); // Make this call thread safe

    HRESULT hr = S_OK;

    ZeroMemory(&m_DeviceProperties, sizeof(m_DeviceProperties));

    hr = AddGenericPropertyKeys();

    if (SUCCEEDED(hr))
    {
        hr = AddGenericSettablePropertyKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = AddGenericDataFieldKeys();
    }

    if(SUCCEEDED(hr))
    {
        hr = SetGenericDefaultValues();
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CGeneric::AddGenericPropertyKeys
//
// Copies the PROPERTYKEYS for Generic Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeneric::AddGenericPropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSupportedGenericProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSupportedGenericProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_RequiredSupportedGenericProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }


    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_OptionalSupportedGenericProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_OptionalSupportedGenericProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorProperties->Add(g_OptionalSupportedGenericProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }
    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeneric::AddGenericSettablePropertyKeys
//
// Copies the PROPERTYKEYS for Generic Supported Properties 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeneric::AddGenericSettablePropertyKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_RequiredSettableGenericProperties); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetProperty(g_RequiredSettableGenericProperties[dwIndex], &var, nullptr);

        // Also add the PROPERTYKEY to the list of supported properties
        if(SUCCEEDED(hr))
        {
            hr = m_spSettableSensorProperties->Add(g_RequiredSettableGenericProperties[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeneric::AddGenericDataFieldKeys
//
// Copies the PROPERTYKEYS for Generic Supported DataFields 
// and sets the Values to VT_EMPTY
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeneric::AddGenericDataFieldKeys()
{
    HRESULT hr = S_OK;

    for (DWORD dwIndex = 0; dwIndex < ARRAY_SIZE(g_SupportedGenericDataFields); dwIndex++)
    {
        PROPVARIANT var;
        PropVariantInit(&var);

        // Initialize all the PROPERTYKEY values to VT_EMPTY
        hr = SetDataField(g_SupportedGenericDataFields[dwIndex], &var);

        // Also add the PROPERTYKEY to the list of supported data fields
        if(SUCCEEDED(hr))
        {
            hr = m_spSupportedSensorDataFields->Add(g_SupportedGenericDataFields[dwIndex]);
        }

        PropVariantClear(&var);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeneric::SetGenericDefaultValues
//
// Fills in default values for most Generic Properties and 
// Data Fields.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeneric::SetGenericDefaultValues()
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

    m_ulDefaultCurrentReportInterval = DEFAULT_GENERIC_CURRENT_REPORT_INTERVAL;
    m_ulDefaultMinimumReportInterval = DEFAULT_GENERIC_MIN_REPORT_INTERVAL;

    m_fltDefaultChangeSensitivity = DEFAULT_GENERIC_SENSITIVITY;

    m_fltDefaultRangeMaximum = DEFAULT_GENERIC_MAXIMUM;
    m_fltDefaultRangeMinimum = DEFAULT_GENERIC_MINIMUM;
    m_fltDefaultAccuracy = DEFAULT_GENERIC_ACCURACY;
    m_fltDefaultResolution = DEFAULT_GENERIC_RESOLUTION;

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, SENSOR_CATEGORY_OTHER);
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, SENSOR_TYPE_UNKNOWN);
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
            wcscat_s(tempStr, HID_USB_DESCRIPTOR_MAX_LENGTH, SENSOR_GENERIC_NAME);
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, tempStr);
        }
        else
        {
            hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, m_pSensorManager->m_wszDeviceName);
        }
    }

    if(SUCCEEDED(hr))
    {
        hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, SENSOR_GENERIC_DESCRIPTION);
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

                hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(&spChangeSensitivityValues));

                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_CHANGE_SENSITIVITY, spChangeSensitivityValues);
                }
            }

            if(SUCCEEDED(hr) && (SENSOR_PROPERTY_RANGE_MAXIMUM == propkey))
            {
                CComPtr<IPortableDeviceValues>  spMaximumValues;

                hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(&spMaximumValues));

                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MAXIMUM, spMaximumValues);
                }
            }

            if(SUCCEEDED(hr) && (SENSOR_PROPERTY_RANGE_MINIMUM == propkey))
            {
                CComPtr<IPortableDeviceValues>  spMinimumValues;

                hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(&spMinimumValues));

                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MINIMUM, spMinimumValues);
                }
            }

            if(SUCCEEDED(hr) && (SENSOR_PROPERTY_ACCURACY == propkey))
            {
                CComPtr<IPortableDeviceValues>  spAccuracyValues;

                hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(&spAccuracyValues));

                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_ACCURACY, spAccuracyValues);
                }
            }

            if(SUCCEEDED(hr) && (SENSOR_PROPERTY_RESOLUTION == propkey))
            {
                CComPtr<IPortableDeviceValues>  spResolutionValues;

                hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(&spResolutionValues));

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

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CGeneric::GetPropertyValuesForGenericObject
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
HRESULT CGeneric::GetPropertyValuesForGenericObject(
    LPCWSTR                        wszObjectID,
    IPortableDeviceKeyCollection*  pKeys,
    IPortableDeviceValues*         pValues)
{
    HRESULT     hr          = S_OK;
    BOOL        fError      = FALSE;
    GUID        guidCategory = {0};

    if ((wszObjectID == NULL) ||
        (pKeys       == NULL) ||
        (pValues     == NULL))
    {
        hr = E_INVALIDARG;
        return hr;
    }

    hr = m_spSensorPropertyValues->GetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, &guidCategory);

    if (SUCCEEDED(hr))
    {
        hr = GetPropertyValuesForSensorObject(wszObjectID, pKeys, pValues, SENSOR_GENERIC_NAME, guidCategory, &fError);
    }

    return (FALSE == fError) ? hr : S_FALSE;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeneric::ProcessGenericAsyncRead
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeneric::ProcessGenericAsyncRead( BYTE* pInputReport, ULONG uReportSize )
{
    HRESULT             hr = S_OK;

    if ((NULL == pInputReport) || (uReportSize == 0))
    {
        hr = E_UNEXPECTED;
    }

    if (SUCCEEDED(hr) && (m_fSensorInitialized))
    {
        if (uReportSize == m_pSensorManager->m_HidCaps.InputReportByteLength)
        {
            //Handle input report

            HIDP_REPORT_TYPE ReportType = HidP_Input;
            USAGE  UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
            USHORT LinkCollection = 0;
            UCHAR  reportID = 0;
            USHORT ReportCount = 0;
            USAGE  Usage = 0;
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

                USHORT  sensorState = SENSOR_STATE_NOT_AVAILABLE;
                USHORT  eventType = SENSOR_EVENT_TYPE_UNKNOWN;

                GUID   guidProperty;
                PROPERTYKEY_DWVALUE_PAIR PropkeyDwValuePair = { {GUID_NULL, 0}, 0};
                PROPERTYKEY_USVALUE_PAIR PropkeyUsValuePair = { {GUID_NULL, 0}, 0};

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
                        pInputReport, 
                        uReportSize);

                if (SUCCEEDED(hr))
                {
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
                            UsageValue = 0;

                            if (ReportCount > 1)
                            {
                                guidProperty = GUID_NULL;
                                PropkeyDwValuePair.propkeyProperty.fmtid = GUID_NULL;
                                PropkeyDwValuePair.propkeyProperty.pid = 0;
                                PropkeyDwValuePair.dwPropertyValue = 0;

                                if ((HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_CATEGORY_GUID == Usage) || (HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_TYPE_GUID == Usage))
                                {
                                    ZeroMemory(&guidProperty, sizeof(guidProperty));
                                    hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, (PCHAR)&guidProperty, sizeof(guidProperty), m_pSensorManager->m_pPreparsedData, (PCHAR)pInputReport, uReportSize);
                                }
                                else if ((HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_PROPERTY_PROPERTYKEY == Usage) || (HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_EVENT_PROPERTYKEY == Usage) || (HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_DATAFIELD_PROPERTYKEY == Usage))
                                {
                                    if (sizeof(PropkeyUsValuePair) == ReportCount)
                                    {
                                        PropkeyUsValuePair.propkeyProperty.fmtid = GUID_NULL;
                                        PropkeyUsValuePair.propkeyProperty.pid = 0;
                                        PropkeyUsValuePair.usPropertyValue = 0;

                                        ZeroMemory(&PropkeyUsValuePair, sizeof(PropkeyUsValuePair));
                                        hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, (PCHAR)&PropkeyUsValuePair, sizeof(PropkeyUsValuePair), m_pSensorManager->m_pPreparsedData, (PCHAR)pInputReport, uReportSize);
   
                                        if (SUCCEEDED(hr))
                                        {
                                            UsageUValue = PropkeyUsValuePair.usPropertyValue;
                                        }

                                        PropkeyDwValuePair.propkeyProperty = PropkeyUsValuePair.propkeyProperty;
                                        PropkeyDwValuePair.dwPropertyValue = PropkeyUsValuePair.usPropertyValue;
                                    }
                                    else if (sizeof(PropkeyDwValuePair) == ReportCount)
                                    {
                                        ZeroMemory(&PropkeyDwValuePair, sizeof(PropkeyDwValuePair));
                                        hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, (PCHAR)&PropkeyDwValuePair, sizeof(PropkeyDwValuePair), m_pSensorManager->m_pPreparsedData, (PCHAR)pInputReport, uReportSize);
   
                                        if (SUCCEEDED(hr))
                                        {
                                            UsageUValue = PropkeyDwValuePair.dwPropertyValue;
                                        }
                                    }
                                }
                                else
                                {
                                    //assume this is a string descriptor
                                    ZeroMemory(UsageArray, HID_FEATURE_REPORT_STRING_MAX_LENGTH*2);
                                    hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, UsageArray, HID_FEATURE_REPORT_STRING_MAX_LENGTH*2, m_pSensorManager->m_pPreparsedData, (PCHAR)pInputReport, uReportSize);
                                }
                            }
                            else if (ReportCount == 1)
                            {
                                UsageUValue = 0;
                                hr = HidP_GetUsageValue(ReportType, UsagePage, LinkCollection, Usage, &UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pInputReport, uReportSize);
                            }
                            else
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Input report count == 0, hr = %!HRESULT!", hr);
                            }

                            if(SUCCEEDED(hr))
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
                                    case HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_EVENT_PROPERTYKEY:
                                        //not supported
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_DATAFIELD_PROPERTYKEY:
                                        //NOTE: only the properties statically defined in g_RequiredSupportedGenericProperties[] are treated here as their types are known 
                                        if (TRUE == IsEqualPropertyKey(PropkeyDwValuePair.propkeyProperty, SENSOR_PROPERTY_STATE))
                                        {
                                        }
                                        else
                                        {
                                            if ((sizeof(PROPERTYKEY) + sizeof(USHORT) == ReportCount) || (sizeof(PROPERTYKEY) + sizeof(DWORD) == ReportCount))
                                            {
                                                ReportCount = 1;
                                            }
                                            else
                                            {
                                                ReportCount = ReportCount - sizeof(PROPERTYKEY);
                                            }

                                            hr = HandleArbitraryDynamicDatafield(PropkeyDwValuePair.propkeyProperty, VT_UNKNOWN, ReportCount, UnitsExp, UsageValue, UsageArray);
                                        }
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_GUID_OR_PROPERTYKEY:
                                        //not supported
                                        break;

                                    default:
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
            Trace(TRACE_LEVEL_ERROR, "%s Input report is incorrect length, is = %i, should be = %i, hr = %!HRESULT!", m_SensorName, uReportSize,  m_pSensorManager->m_HidCaps.InputReportByteLength, hr);
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CGeneric::UpdateGenericDeviceValues
//
//  This method parses the content in the buffer and updates cached data vlaues.
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CGeneric::UpdateGenericPropertyValues(BYTE* pFeatureReport, ULONG *pReportSize, BOOL fSettableOnly, BOOL *pfFeatureReportSupported)
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
    CHAR   UsageArray[HID_FEATURE_REPORT_STRING_MAX_LENGTH*2] = {0};

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
            GUID   guidProperty = GUID_NULL;
            PROPERTYKEY_DWVALUE_PAIR PropkeyDwValuePair = { {GUID_NULL, 0}, 0};

            PROPVARIANT value;
            PropVariantInit( &value );

            if (uReportSize == m_pSensorManager->m_HidCaps.FeatureReportByteLength)
            {
                USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;

                LONG   UnitsExp = 0;
                ULONG  BitSize = 0;

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

                if (SUCCEEDED(hr))
                {
                    for(ULONG idx = 0; idx < numNodes; idx++)
                    {
                        if (reportID == m_FeatureValueCapsNodes[idx].ReportID)
                        {
                            UsagePage = m_FeatureValueCapsNodes[idx].UsagePage;
                            Usage = m_FeatureValueCapsNodes[idx].NotRange.Usage;
                            UsageDataModifier = (USHORT)Usage & 0xF000; //extract the data modifier
                            ReportCount = m_FeatureValueCapsNodes[idx].ReportCount;
                            UnitsExp = m_FeatureValueCapsNodes[idx].UnitsExp;
                            BitSize = m_FeatureValueCapsNodes[idx].BitSize;
                            TranslateHidUnitsExp(&UnitsExp);

                            if (ReportCount > 1)
                            {
                                guidProperty = GUID_NULL;
                                PropkeyDwValuePair.propkeyProperty.fmtid = GUID_NULL;
                                PropkeyDwValuePair.propkeyProperty.pid = 0;
                                PropkeyDwValuePair.dwPropertyValue = 0;

                                if ((HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_CATEGORY_GUID == Usage) || (HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_TYPE_GUID == Usage))
                                {
                                    ZeroMemory(&guidProperty, sizeof(guidProperty));
                                    hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, (PCHAR)&guidProperty, sizeof(guidProperty), m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                                }
                                else if ((HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_PROPERTY_PROPERTYKEY == Usage) || (HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_EVENT_PROPERTYKEY == Usage) || (HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_DATAFIELD_PROPERTYKEY == (Usage & 0x0FFF)))
                                {
                                    ZeroMemory(&PropkeyDwValuePair, sizeof(PropkeyDwValuePair));
                                    hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, (PCHAR)&PropkeyDwValuePair, sizeof(PropkeyDwValuePair), m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                                }
                                else
                                {
                                    //assume this is a string descriptor
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
                                    case HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_CATEGORY_GUID:
                                        m_DeviceProperties.fSensorCategorySupported = TRUE;

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = m_spSensorPropertyValues->SetGuidValue(WPD_FUNCTIONAL_OBJECT_CATEGORY, guidProperty);
                                        }
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_TYPE_GUID:
                                        m_DeviceProperties.fSensorTypeSupported = TRUE;
                                        hr = m_spSensorPropertyValues->SetGuidValue(SENSOR_PROPERTY_TYPE, guidProperty);
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_DATAFIELD_PROPERTYKEY:
                                        hr = HandleArbitraryDynamicDatafieldProperty(PropkeyDwValuePair.propkeyProperty, UnitsExp, PropkeyDwValuePair.dwPropertyValue, UsageDataModifier);
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_PROPERTY_PROPERTYKEY:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_EVENT_PROPERTYKEY:
                                    case HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_GUID_OR_PROPERTYKEY:
                                        //not supported
                                        break;

                                    default:
                                        //unknown
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
                            for (DWORD dwDfIdx = 1; dwDfIdx < cDfKeys; dwDfIdx++) //ignore the timestamp
                            {
                                hr = m_spSupportedSensorDataFields->GetAt(dwDfIdx, &pkDfKey);

                                if (SUCCEEDED(hr))
                                {
                                    hr = HandleChangeSensitivityUpdate(
                                                                        reportID, 
                                                                        fSettableOnly,
                                                                        m_DeviceProperties.fGlobalSensitivitySupported,
                                                                        FALSE,
                                                                        m_DynamicDatafieldSensitivitySupported[dwDfIdx],
                                                                        m_DynamicDatafieldUsages[dwDfIdx],
                                                                        m_DynamicDatafieldSupported[dwDfIdx],
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
                                    hr = HandleMaximumUpdate(
                                                            reportID, 
                                                            m_DeviceProperties.fGlobalMaximumSupported,
                                                            FALSE,
                                                            m_DynamicDatafieldMaximumSupported[dwDfIdx],
                                                            m_DynamicDatafieldUsages[dwDfIdx],
                                                            m_DynamicDatafieldSupported[dwDfIdx],
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
                                    hr = HandleMinimumUpdate(
                                                            reportID, 
                                                            m_DeviceProperties.fGlobalMinimumSupported,
                                                            FALSE,
                                                            m_DynamicDatafieldMinimumSupported[dwDfIdx],
                                                            m_DynamicDatafieldUsages[dwDfIdx],
                                                            m_DynamicDatafieldSupported[dwDfIdx],
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

                    else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_ACCURACY))
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
                                    hr = HandleAccuracyUpdate(
                                                            reportID, 
                                                            m_DeviceProperties.fGlobalAccuracySupported,
                                                            FALSE,
                                                            m_DynamicDatafieldAccuracySupported[dwDfIdx],
                                                            m_DynamicDatafieldUsages[dwDfIdx],
                                                            m_DynamicDatafieldSupported[dwDfIdx],
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

                    else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_RESOLUTION))
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
                                    hr = HandleResolutionUpdate(
                                                            reportID, 
                                                            m_DeviceProperties.fGlobalResolutionSupported,
                                                            FALSE,
                                                            m_DynamicDatafieldResolutionSupported[dwDfIdx],
                                                            m_DynamicDatafieldUsages[dwDfIdx],
                                                            m_DynamicDatafieldSupported[dwDfIdx],
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

