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


/////////////////////////////////////////////////////////////////////////
//
// CSensor::CSensor
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CSensor::CSensor() :
    m_spSensorPropertyValues(nullptr),
    m_spSensorDataFieldValues(nullptr),
    m_fSensorInitialized(FALSE),
    m_fValidDataEvent(FALSE)
{
    m_fSensorUpdated = FALSE; // flag checks to see if data report was received
    m_fInitialDataReceived = FALSE;  // flag checks to see if inital poll request was fulfilled
    m_fReportingState = FALSE; // tracks whether sensor reporting is turned on or off
    m_ulPowerState = 0;    // tracks in what state sensor power should be

    ZeroMemory(&m_DynamicDeviceProperties, sizeof(m_DynamicDeviceProperties));

    for (DWORD idx = 0; idx < MAX_NUM_DATA_FIELDS; idx++)
    {
        m_DynamicDatafieldSupported[idx] = FALSE;
        m_DynamicDatafieldUsages[idx] = 0;
        m_DynamicDatafieldSensitivitySupported[idx] = FALSE;
        m_DynamicDatafieldSensitivity[idx] = FLT_MAX;
        m_DynamicDatafieldMaximumSupported[idx] = FALSE;
        m_DynamicDatafieldMaximum[idx] = FLT_MAX;
        m_DynamicDatafieldMinimumSupported[idx] = FALSE;
        m_DynamicDatafieldMinimum[idx] = -FLT_MAX;
        m_DynamicDatafieldAccuracySupported[idx] = FALSE;
        m_DynamicDatafieldAccuracy[idx] = FLT_MAX;
        m_DynamicDatafieldResolutionSupported[idx] = FALSE;
        m_DynamicDatafieldResolution[idx] = FLT_MAX;

        m_fltLowestClientChangeSensitivities[idx] = FLT_MAX;
    }

    m_spSupportedSensorDataFields = nullptr;
    m_spSensorPropertyValues = nullptr;

    m_spSupportedSensorProperties = nullptr;
    m_spSettableSensorProperties = nullptr;
    m_spRequiredDataFields = nullptr;

    m_spSensorDataFieldValues = nullptr;

    m_fSensorInitialized = FALSE;    // flag checks if we have been initialized
    m_fSensorPropertiesPreviouslyUpdated = FALSE;

    m_fFeatureReportSupported = FALSE;
    m_fHidUsagePropertySupported = FALSE;

    m_fCapsNodesInitialized = FALSE;

    m_InputValueCapsNodes = nullptr;
    m_OutputValueCapsNodes = nullptr;
    m_FeatureValueCapsNodes = nullptr;

    m_InputButtonCapsNodes = nullptr;
    m_OutputButtonCapsNodes = nullptr;
    m_FeatureButtonCapsNodes = nullptr;

    m_fValidDataEvent = FALSE;         // flag that indicates an event needs to be fired

    m_fSensorStateChanged = FALSE;
    m_fInitialPropertiesReceived = FALSE;

    m_fWarnedOnUseOfFeatureConnectionType = FALSE;
    m_fWarnedOnUseOfFeatureReportingState = FALSE;
    m_fWarnedOnUseOfFeaturePowerState = FALSE;
    m_fWarnedOnUseOfFeatureSensorState = FALSE;
    m_fWarnedOnUseOfInputSensorState = FALSE;
    m_fWarnedOnUseOfInputEventType = FALSE;

    m_fInformedCommonInputReportConditions = FALSE;
    m_fInformedCommonFeatureReportConditions = FALSE;
    
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
    _In_ ULONG sensUsage,
    _In_ USHORT sensLinkCollection,
    _In_ DWORD sensNum, 
    _In_ LPWSTR pwszManufacturer,
    _In_ LPWSTR pwszProduct,
    _In_ LPWSTR pwszSerialNumber,
    _In_ LPWSTR pwszSensorID)
{
    // Check if we are already initialized
    HRESULT hr = (TRUE == IsInitialized()) ? E_UNEXPECTED : S_OK;

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
        m_SensorUsage = sensUsage;
        m_SensorLinkCollection = sensLinkCollection;
        hr = StringCchCopy(m_pwszManufacturer, HID_USB_DESCRIPTOR_MAX_LENGTH, pwszManufacturer);

        if (SUCCEEDED(hr))
        {
            hr = StringCchCopy(m_pwszProduct, HID_USB_DESCRIPTOR_MAX_LENGTH, pwszProduct);
        }

        if (SUCCEEDED(hr))
        {
            hr = StringCchCopy(m_pwszSerialNumber, HID_USB_DESCRIPTOR_MAX_LENGTH, pwszSerialNumber);
        }

        if (SUCCEEDED(hr))
        {
            hr = StringCchCopy(m_SensorID, HID_USB_DESCRIPTOR_MAX_LENGTH, pwszSensorID);
        }

        m_ullInitialEventTime = GetTickCount64();
        m_InputReportFailureCount = 0;
    }
    
    // the HID report descriptor does not change over the lifetime of the sensor
    // therefore, to improve performance, preparse the value caps for each supported report type
    
    if (FALSE == m_fCapsNodesInitialized)
    {
        m_InputValueCapsNodes = nullptr;
        m_OutputValueCapsNodes = nullptr;
        m_FeatureValueCapsNodes = nullptr;
        m_InputButtonCapsNodes = nullptr;
        m_OutputButtonCapsNodes = nullptr;
        m_FeatureButtonCapsNodes = nullptr;

        if (SUCCEEDED(hr) && m_pSensorManager->m_HidCaps.NumberInputValueCaps > 0)
        {
            USHORT numNodes = m_pSensorManager->m_HidCaps.NumberInputValueCaps;
            m_InputValueCapsNodes = (new HIDP_VALUE_CAPS[(m_pSensorManager->m_HidCaps.NumberInputValueCaps)*(sizeof(HIDP_VALUE_CAPS))]);
            if (HIDP_STATUS_SUCCESS != HidP_GetValueCaps(HidP_Input, m_InputValueCapsNodes, &numNodes, m_pSensorManager->m_pPreparsedData))
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failure to get value caps type: input, hr = %!HRESULT!", hr);
            }

            if (SUCCEEDED(hr))
            {
                m_StartingInputReportID = m_InputValueCapsNodes[0].ReportID;
            }
        }

        if (SUCCEEDED(hr) && m_pSensorManager->m_HidCaps.NumberOutputValueCaps > 0)
        {
            USHORT numNodes = m_pSensorManager->m_HidCaps.NumberOutputValueCaps;
            m_OutputValueCapsNodes = (new HIDP_VALUE_CAPS[(m_pSensorManager->m_HidCaps.NumberOutputValueCaps)*(sizeof(HIDP_VALUE_CAPS))]);
            if (HIDP_STATUS_SUCCESS != HidP_GetValueCaps(HidP_Output, m_OutputValueCapsNodes, &numNodes, m_pSensorManager->m_pPreparsedData))
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failure to get value caps type: output, hr = %!HRESULT!", hr);
            }

            if (SUCCEEDED(hr))
            {
                m_StartingOutputReportID = m_OutputValueCapsNodes[0].ReportID;
            }
        }

        m_fFeatureReportSupported = FALSE;
        if (SUCCEEDED(hr) && m_pSensorManager->m_HidCaps.NumberFeatureValueCaps > 0)
        {
            USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;
            m_FeatureValueCapsNodes = (new HIDP_VALUE_CAPS[(m_pSensorManager->m_HidCaps.NumberFeatureValueCaps)*(sizeof(HIDP_VALUE_CAPS))]);
            if (HIDP_STATUS_SUCCESS != HidP_GetValueCaps(HidP_Feature, m_FeatureValueCapsNodes, &numNodes, m_pSensorManager->m_pPreparsedData))
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failure to get value caps type: feature, hr = %!HRESULT!", hr);
            }
            if(SUCCEEDED(hr))
            {
                m_StartingFeatureReportID = m_FeatureValueCapsNodes[0].ReportID;
                m_fFeatureReportSupported = TRUE;
            }
        }

        if (SUCCEEDED(hr) && m_pSensorManager->m_HidCaps.NumberInputButtonCaps > 0)
        {
            USHORT numNodes = m_pSensorManager->m_HidCaps.NumberInputButtonCaps;
            m_InputButtonCapsNodes = (new HIDP_BUTTON_CAPS[(m_pSensorManager->m_HidCaps.NumberInputButtonCaps)*(sizeof(HIDP_BUTTON_CAPS))]);
            if (HIDP_STATUS_SUCCESS != HidP_GetButtonCaps(HidP_Input, m_InputButtonCapsNodes, &numNodes, m_pSensorManager->m_pPreparsedData))
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failure to get button caps type: input, hr = %!HRESULT!", hr);
            }
        }

        if (SUCCEEDED(hr) && m_pSensorManager->m_HidCaps.NumberOutputButtonCaps > 0)
        {
            USHORT numNodes = m_pSensorManager->m_HidCaps.NumberOutputButtonCaps;
            m_OutputButtonCapsNodes = (new HIDP_BUTTON_CAPS[(m_pSensorManager->m_HidCaps.NumberOutputButtonCaps)*(sizeof(HIDP_BUTTON_CAPS))]);
            if (HIDP_STATUS_SUCCESS != HidP_GetButtonCaps(HidP_Output, m_OutputButtonCapsNodes, &numNodes, m_pSensorManager->m_pPreparsedData))
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failure to get button caps type: output, hr = %!HRESULT!", hr);
            }
        }

        if (SUCCEEDED(hr) && m_pSensorManager->m_HidCaps.NumberFeatureButtonCaps > 0)
        {
            USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureButtonCaps;
            m_FeatureButtonCapsNodes = (new HIDP_BUTTON_CAPS[(m_pSensorManager->m_HidCaps.NumberFeatureButtonCaps)*(sizeof(HIDP_BUTTON_CAPS))]);
            if (HIDP_STATUS_SUCCESS != HidP_GetButtonCaps(HidP_Feature, m_FeatureButtonCapsNodes, &numNodes, m_pSensorManager->m_pPreparsedData))
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failure to get button caps type: feature, hr = %!HRESULT!", hr);
            }
        }

        if (SUCCEEDED(hr))
        {
            m_fCapsNodesInitialized = TRUE;
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

        if (NULL != m_InputValueCapsNodes) {
            delete[] (BYTE*)m_InputValueCapsNodes;
        }

        if (NULL != m_OutputValueCapsNodes) {
            delete[] (BYTE*)m_OutputValueCapsNodes;
        }

        if (NULL != m_FeatureValueCapsNodes) {
            delete[] (BYTE*)m_FeatureValueCapsNodes;
        }

        if (NULL != m_InputButtonCapsNodes) {
            delete[] (BYTE*)m_InputButtonCapsNodes;
        }

        if (NULL != m_OutputButtonCapsNodes) {
            delete[] (BYTE*)m_OutputButtonCapsNodes;
        }

        if (NULL != m_FeatureButtonCapsNodes) {
            delete[] (BYTE*)m_FeatureButtonCapsNodes;
        }

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
// Gets the list of RequiredDataFields for the sensor
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
                        {
                            hr = m_spSensorPropertyValues->RemoveValue(key); 
                        }

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
                        {
                            hr = m_spSensorDataFieldValues->RemoveValue(key); 
                        }

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
    PropVariantInit( &var );

    //Get the current time in  SYSTEMTIME format
    SYSTEMTIME st;
    ::GetSystemTime(&st);
    m_st = st;

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

    return hr;
}

// Called when new data is updated from device
VOID CSensor::RaiseDataEvent()
{
    CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection);
    size_t cEventSubscribers = 0;

    cEventSubscribers = m_pSubscriberMap.GetCount();

    // Check if there are any subscribers
    if( 0 < cEventSubscribers )
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

// Used to translate HID UnitsExponent from HID form to decimal form
VOID CSensor::TranslateHidUnitsExp(_Inout_ LONG *UnitsExp)
{
    if (*UnitsExp >= 0x08)
    {
        switch(*UnitsExp)
        {
        case 0x08: *UnitsExp = -8; break;
        case 0x09: *UnitsExp = -7; break;
        case 0x0A: *UnitsExp = -6; break;
        case 0x0B: *UnitsExp = -5; break;
        case 0x0C: *UnitsExp = -4; break;
        case 0x0D: *UnitsExp = -3; break;
        case 0x0E: *UnitsExp = -2; break;
        case 0x0F: *UnitsExp = -1; break;
        default: *UnitsExp = 0; break;
        }
    }
}

LONG CSensor::ExtractValueFromUsageUValue(_In_ ULONG LogicalMin, _In_ ULONG BitSize, _In_ ULONG UsageUValue)
{
    LONG UsageValue = 0;
    LONG Sign = 0;

    switch (BitSize)
    {
    case 8:
        if (0 == LogicalMin)
        {
            UsageValue = (LONG)UsageUValue;
        }
        else
        {
            Sign = (UsageUValue & 0x80) ? -1 : 1;
            UsageValue = (Sign >= 0) ? (LONG)UsageUValue : (LONG)((UsageUValue - (UCHAR_MAX)) - 1);
        }
        break;

    case 16:
        if (0 == LogicalMin)
        {
            UsageValue = (LONG)UsageUValue;
        }
        else
        {
            Sign = (UsageUValue & 0x8000) ? -1 : 1;
            UsageValue = (Sign >= 0) ? (LONG)UsageUValue : (LONG)((UsageUValue - (USHRT_MAX)) - 1);
        }
        break;

    case 32:
        if (0 == LogicalMin)
        {
            UsageValue = (LONG)UsageUValue;
        }
        else
        {
            Sign = (UsageUValue & 0x80000000) ? -1 : 1;
            UsageValue = (Sign >= 0) ? (LONG)UsageUValue : (LONG)((UsageUValue - (UINT_MAX)) - 1);
        }
        break;

    default:
        if (0 == LogicalMin)
        {
            UsageValue = (LONG)UsageUValue;
        }
        else
        {
            Sign = (UsageUValue & 0x8000) ? -1 : 1;
            UsageValue = (Sign >= 0) ? (LONG)UsageUValue : (LONG)((UsageUValue - (USHRT_MAX)) - 1);
        }
        break;
    }

    return UsageValue;
}

ULONG CSensor::ExtractUsageUValueFromDouble(_In_ DOUBLE dblValue, _In_ LONG UnitsExp, _In_ ULONG BitSize)
{
    DOUBLE dblScaledValue = dblValue * pow( 10.0, (double)( -1.0 * UnitsExp ));
    LONG   Sign = (dblScaledValue > 0.0) ? 1 : -1;

    LONG   FloorValue = (LONG)floor(dblScaledValue);
    LONG   CeilValue = (LONG)ceil(dblScaledValue);

    DOUBLE FloorValueDelta = dblScaledValue - (double)FloorValue;
    DOUBLE CeilValueDelta = (double)CeilValue - dblScaledValue;

    LONG   UsageValue = FloorValueDelta < CeilValueDelta ? FloorValue : CeilValue;

    ULONG  UsageUValue = 0;

    switch (BitSize)
    {
    case 8:
        UsageUValue = (Sign > 0) ? (ULONG)UsageValue : UsageValue + UCHAR_MAX + 1;
        break;

    case 16: 
        UsageUValue = (Sign > 0) ? (ULONG)UsageValue : UsageValue + USHORT_MAX + 1;
        break;

    case 32:
        UsageUValue = (Sign > 0) ? (ULONG)UsageValue : UsageValue + ULONG_MAX + 1;
        break;

    default:
        UsageUValue = (Sign > 0) ? (ULONG)UsageValue : UsageValue + HID_SIGN_FACTOR;
        break;

    }

    return UsageUValue;
}

DOUBLE CSensor::ExtractDoubleFromUsageValue(_In_ ULONG LogicalMin, _In_ ULONG UsageUValue, _In_ LONG UsageValue, _In_ LONG UnitsExp)
{
    DOUBLE dblValue;

    if (LogicalMin == 0) //unsigned number
    {
        dblValue = (DOUBLE)UsageUValue;
    }
    else //signed number
    {
        dblValue = (DOUBLE)UsageValue;
    }

    dblValue = dblValue * (FLOAT)pow( 10.0, (double)( 1.0 * UnitsExp ));

    return dblValue;
}

LONGLONG CSensor::ExtractLongLongFromUsageValue(_In_ ULONG LogicalMin, _In_ ULONG UsageUValue, _In_ LONG UsageValue, _In_ LONG UnitsExp)
{
    LONGLONG llValue;

    if (LogicalMin == 0) //unsigned number
    {
        llValue = UsageUValue * (ULONG)pow( 10.0, (double)( 1.0 * UnitsExp ));
    }
    else //signed number
    {
        llValue = UsageValue * (ULONG)pow( 10.0, (double)( 1.0 * UnitsExp ));
    }

    return llValue;
}

HRESULT CSensor::TranslateHidUsageToSensorDataTypePropertyKey(_In_ ULONG ulHidUsage, _Out_ PROPERTYKEY* pk, _Out_ VARTYPE *vt)
{
    static const PROPERTYKEY PROPERTYKEY_NULL = 
    { { 0x00000000, 0x0000, 0x0000, { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } }, 0};

    typedef struct _TRANSLATION_TABLE_ENTRY 
    {
        PROPERTYKEY pk;
        VARTYPE     vt;

    } TRANSLATION_TABLE_ENTRY;


    static const TRANSLATION_TABLE_ENTRY SensorDataTypeTranslationTable[] = {

        ////data type usages
        ////data type location
        {{SENSOR_DATA_TYPE_LOCATION_GUID, 0}, VT_EMPTY},                    //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION                                    0x0400
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_DESIRED_ACCURACY                   0x0401
        {SENSOR_DATA_TYPE_ALTITUDE_ANTENNA_SEALEVEL_METERS, VT_R8},         //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ALTITUDE_ANTENNA_SEALEVEL          0x0402
        {SENSOR_DATA_TYPE_DIFFERENTIAL_REFERENCE_STATION_ID, VT_I4},        //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_DIFFERENTIAL_REFERENCE_STATION_ID  0x0403
        {SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_ERROR_METERS, VT_R8},          //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ALTITIDE_ELIPSOID_ERROR            0x0404
        {SENSOR_DATA_TYPE_ALTITUDE_ELLIPSOID_METERS, VT_R8},                //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ALTITIDE_ELIPSOID                  0x0405
        {SENSOR_DATA_TYPE_ALTITUDE_SEALEVEL_ERROR_METERS, VT_R8},           //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ALTITUDE_SEALEVEL_ERROR            0x0406
        {SENSOR_DATA_TYPE_ALTITUDE_SEALEVEL_METERS, VT_R8},                 //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ALTITUDE_SEALEVEL                  0x0407
        {SENSOR_DATA_TYPE_DGPS_DATA_AGE, VT_R8},                            //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_DGPS_DATA_AGE                      0x0408
        {SENSOR_DATA_TYPE_ERROR_RADIUS_METERS, VT_R8},                      //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ERROR_RADIUS                       0x0409
        {SENSOR_DATA_TYPE_FIX_QUALITY, VT_I4},                              //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_FIX_QUALITY                        0x040A
        {SENSOR_DATA_TYPE_FIX_TYPE, VT_I4},                                 //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_FIX_TYPE                           0x040B
        {SENSOR_DATA_TYPE_GEOIDAL_SEPARATION, VT_R8},                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_GEOIDAL_SEPARATION                 0x040C
        {SENSOR_DATA_TYPE_GPS_OPERATION_MODE, VT_I4},                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_GPS_OPERATION_MODE                 0x040D
        {SENSOR_DATA_TYPE_GPS_SELECTION_MODE, VT_I4},                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_GPS_SELECTION_MODE                 0x040E
        {SENSOR_DATA_TYPE_GPS_STATUS, VT_I4},                               //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_GPS_STATUS                         0x040F
        {SENSOR_DATA_TYPE_POSITION_DILUTION_OF_PRECISION, VT_R8},           //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_POSITION_DILUTION_OF_PRECISION     0x0410
        {SENSOR_DATA_TYPE_HORIZONAL_DILUTION_OF_PRECISION, VT_R8},          //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_HORIZONTAL_DILUTION_OF_PRECISION   0x0411
        {SENSOR_DATA_TYPE_VERTICAL_DILUTION_OF_PRECISION, VT_R8},           //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_VERTICAL_DILUTION_OF_PRECISION     0x0412
        {SENSOR_DATA_TYPE_LATITUDE_DEGREES, VT_R8},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_LATITUDE                           0x0413
        {SENSOR_DATA_TYPE_LONGITUDE_DEGREES, VT_R8},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_LONGITUDE                          0x0414
        {SENSOR_DATA_TYPE_TRUE_HEADING_DEGREES, VT_R8},                     //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_TRUE_HEADING                       0x0415
        {SENSOR_DATA_TYPE_MAGNETIC_HEADING_DEGREES, VT_R8},                 //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_MAGNETIC_HEADING                   0x0416
        {SENSOR_DATA_TYPE_MAGNETIC_VARIATION, VT_R8},                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_MAGNETIC_VARIATION                 0x0417
        {SENSOR_DATA_TYPE_SPEED_KNOTS, VT_R8},                              //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SPEED                              0x0418
        {SENSOR_DATA_TYPE_SATELLITES_IN_VIEW, VT_I4},                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW                 0x0419
        {SENSOR_DATA_TYPE_SATELLITES_IN_VIEW_AZIMUTH, VT_VECTOR|VT_UI1},    //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW_AZIMUTH         0x041A
        {SENSOR_DATA_TYPE_SATELLITES_IN_VIEW_ELEVATION, VT_VECTOR|VT_UI1},  //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW_ELEVATION       0x041B
        {SENSOR_DATA_TYPE_SATELLITES_IN_VIEW_ID, VT_VECTOR|VT_UI1},         //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW_ID              0x041C
        {SENSOR_DATA_TYPE_SATELLITES_IN_VIEW_PRNS, VT_VECTOR|VT_UI1},       //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW_PRNs            0x041D
        {SENSOR_DATA_TYPE_SATELLITES_IN_VIEW_STN_RATIO, VT_VECTOR|VT_UI1},  //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW_STN_RATIO       0x041E
        {SENSOR_DATA_TYPE_SATELLITES_USED_COUNT, VT_I4},                    //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_USED_COUNT              0x041F
        {SENSOR_DATA_TYPE_SATELLITES_USED_PRNS, VT_VECTOR|VT_UI1},          //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_USED_PRNs               0x0420
        {SENSOR_DATA_TYPE_NMEA_SENTENCE, VT_LPWSTR},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_NMEA_SENTENCE                      0x0421
        {SENSOR_DATA_TYPE_ADDRESS1, VT_LPWSTR},                             //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ADDRESS_LINE_1                     0x0422
        {SENSOR_DATA_TYPE_ADDRESS2, VT_LPWSTR},                             //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ADDRESS_LINE_2                     0x0423
        {SENSOR_DATA_TYPE_CITY, VT_LPWSTR},                                 //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_CITY                               0x0424
        {SENSOR_DATA_TYPE_STATE_PROVINCE, VT_LPWSTR},                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_STATE_OR_PROVINCE                  0x0425
        {SENSOR_DATA_TYPE_COUNTRY_REGION, VT_LPWSTR},                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_COUNTRY_OR_REGION                  0x0426
        {SENSOR_DATA_TYPE_POSTALCODE, VT_LPWSTR},                           //#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_POSTAL_CODE                        0x0427
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0428
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0429
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x042A
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x042B
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x042C
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x042D
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x042E
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x042F
        ////data type environmental
        {{SENSOR_DATA_TYPE_ENVIRONMENTAL_GUID, 0}, VT_EMPTY},               //#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL                               0x0430
        {SENSOR_DATA_TYPE_ATMOSPHERIC_PRESSURE_BAR, VT_R4},                 //#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_ATMOSPHERIC_PRESSURE          0x0431
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_REFERENCE_PRESSURE            0x0432
        {SENSOR_DATA_TYPE_RELATIVE_HUMIDITY_PERCENT, VT_R4},                //#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_RELATIVE_HUMIDITY             0x0433
        {SENSOR_DATA_TYPE_TEMPERATURE_CELSIUS, VT_R4},                      //#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_TEMPERATURE                   0x0434
        {SENSOR_DATA_TYPE_WIND_DIRECTION_DEGREES_ANTICLOCKWISE, VT_R4},     //#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_WIND_DIRECTION                0x0435
        {SENSOR_DATA_TYPE_WIND_SPEED_METERS_PER_SECOND, VT_R4},             //#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_WIND_SPEED                    0x0436
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0437
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0438
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0439
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x043A
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x043B
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x043C
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x043D
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x043E
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x043F
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_ENVIRONMENTAL                           0x0440 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_ENVIRONMENTAL_REFERENCE_PRESSURE        0x0441 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0442
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0443
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0444
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0445
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0446
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0447
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0448
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0449
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x044A
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x044B
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x044C
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x044D
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x044E
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x044F
        ////data type motion
        {{SENSOR_DATA_TYPE_MOTION_GUID, 0}, VT_EMPTY},                      //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION                                      0x0450
        {SENSOR_DATA_TYPE_MOTION_STATE, VT_BOOL},                           //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_INTENSITY                            0x0451
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ACCELERATION                         0x0452
        {SENSOR_DATA_TYPE_ACCELERATION_X_G, VT_R8},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ACCELERATION_X_AXIS                  0x0453
        {SENSOR_DATA_TYPE_ACCELERATION_Y_G, VT_R8},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ACCELERATION_Y_AXIS                  0x0454
        {SENSOR_DATA_TYPE_ACCELERATION_Z_G, VT_R8},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ACCELERATION_Z_AXIS                  0x0455
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY                     0x0456
        {SENSOR_DATA_TYPE_ANGULAR_VELOCITY_X_DEGREES_PER_SECOND, VT_R8},    //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_ROLL_AXIS           0x0457
        {SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Y_DEGREES_PER_SECOND, VT_R8},    //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_PITCH_AXIS          0x0458
        {SENSOR_DATA_TYPE_ANGULAR_VELOCITY_Z_DEGREES_PER_SECOND, VT_R8},    //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_YAW_AXIS            0x0459
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_POSITION                     0x045A
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_POSITION_ROLL_AXIS           0x045B
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_POSITION_PITCH_AXIS          0x045C
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_POSITION_YAW_AXIS            0x045D
        {SENSOR_DATA_TYPE_SPEED_METERS_PER_SECOND, VT_R8},                  //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_SPEED                                0x045E
        {SENSOR_DATA_TYPE_MOTION_STATE, VT_BOOL},                           //#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_INTENSITY                            0x045F
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0460
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0461
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0462
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0463
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0464
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0465
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0466
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0467
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0468
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0469
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x046A
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x046B
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x046C
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x046D
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x046E
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x046F
        ////data type orientation
        {{SENSOR_DATA_TYPE_ORIENTATION_GUID, 0}, VT_EMPTY},                 //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION                                 0x0470
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING                0x0471
        {SENSOR_DATA_TYPE_MAGNETIC_HEADING_X_DEGREES, VT_R4},               //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING_X              0x0472
        {SENSOR_DATA_TYPE_MAGNETIC_HEADING_Y_DEGREES, VT_R4},               //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING_Y              0x0473
        {SENSOR_DATA_TYPE_MAGNETIC_HEADING_Z_DEGREES, VT_R4},               //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING_Z              0x0474
        {SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_MAGNETIC_NORTH_DEGREES, VT_R8},   //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_MAGNETIC_NORTH    0x0475
        {SENSOR_DATA_TYPE_MAGNETIC_HEADING_COMPENSATED_TRUE_NORTH_DEGREES, VT_R8},       //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_TRUE_NORTH        0x0476
        {SENSOR_DATA_TYPE_MAGNETIC_HEADING_MAGNETIC_NORTH_DEGREES, VT_R8},               //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_NORTH                0x0477
        {SENSOR_DATA_TYPE_MAGNETIC_HEADING_TRUE_NORTH_DEGREES, VT_R8},                   //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TRUE_NORTH                    0x0478
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_DISTANCE                        0x0479
        {SENSOR_DATA_TYPE_DISTANCE_X_METERS, VT_R4},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_DISTANCE_X                      0x047A
        {SENSOR_DATA_TYPE_DISTANCE_Y_METERS, VT_R4},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_DISTANCE_Y                      0x047B
        {SENSOR_DATA_TYPE_DISTANCE_Z_METERS, VT_R4},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_DISTANCE_Z                      0x047C
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_DISTANCE_OUT_OF_RANGE           0x047D
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TILT                            0x047E
        {SENSOR_DATA_TYPE_TILT_X_DEGREES, VT_R4},                           //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TILT_X                          0x047F
        {SENSOR_DATA_TYPE_TILT_Y_DEGREES, VT_R4},                           //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TILT_Y                          0x0480
        {SENSOR_DATA_TYPE_TILT_Z_DEGREES, VT_R4},                           //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TILT_Z                          0x0481
        {SENSOR_DATA_TYPE_ROTATION_MATRIX, VT_VECTOR|VT_UI1},               //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_ROTATION_MATRIX                 0x0482
        {SENSOR_DATA_TYPE_QUATERNION, VT_VECTOR|VT_UI1},                    //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_QUATERNION                      0x0483
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX                   0x0484
        {SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_X_MILLIGAUSS, VT_R8},     //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_X_AXIS            0x0485
        {SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Y_MILLIGAUSS, VT_R8},     //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Y_AXIS            0x0486
        {SENSOR_DATA_TYPE_MAGNETIC_FIELD_STRENGTH_Z_MILLIGAUSS, VT_R8},     //#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Z_AXIS            0x0487
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0488
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0489
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x048A
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x048B
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x048C
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x048D
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x048E
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x048F
        ////data type mechanical
        {{SENSOR_DATA_TYPE_GUID_MECHANICAL_GUID, 0}, VT_EMPTY},             //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL                                  0x0490
        {SENSOR_DATA_TYPE_BOOLEAN_SWITCH_STATE, VT_BOOL},                   //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_BOOLEAN_SWITCH_STATE             0x0491
        {SENSOR_DATA_TYPE_BOOLEAN_SWITCH_ARRAY_STATES, VT_UI4},             //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_BOOLEAN_SWITCH_ARRAY_STATES      0x0492
        {SENSOR_DATA_TYPE_MULTIVALUE_SWITCH_STATE, VT_R8},                  //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_MULTIVAL_SWITCH_VALUE            0x0493
        {SENSOR_DATA_TYPE_FORCE_NEWTONS, VT_R8},                            //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_FORCE                            0x0494
        {SENSOR_DATA_TYPE_ABSOLUTE_PRESSURE_PASCAL, VT_R8},                 //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_ABSOLUTE_PRESSURE                0x0495
        {SENSOR_DATA_TYPE_GAUGE_PRESSURE_PASCAL, VT_R8},                    //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_GAUGE_PRESSURE                   0x0496
        {SENSOR_DATA_TYPE_STRAIN, VT_R8},                                   //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_STRAIN                           0x0497
        {SENSOR_DATA_TYPE_WEIGHT_KILOGRAMS, VT_R8},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_WEIGHT                           0x0498
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_VIBRATION_STATE                  0x0499
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_VIBRATION_SPEED_FORWARD          0x049A
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_VIBRATION_SPEED_BACKWARD         0x049B
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x049C
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x049D
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x049E
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x049F
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_MECHANICAL                              0x04A0 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_MECHANICAL_VIBRATION_STATE              0x04A1 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_VIBRATION_SPEED_FORWARD          0x04A2 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_VIBRATION_SPEED_BACKWARD         0x04A3 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04A4
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04A5
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04A6
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04A7
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04A8
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04A9
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04AA
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04AB
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04AC
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04AD
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04AE
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04AF
        ////data type biometric
        {{SENSOR_DATA_TYPE_BIOMETRIC_GUID, 0}, VT_EMPTY},                   //#define HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC                                   0x04B0
        {SENSOR_DATA_TYPE_HUMAN_PRESENCE, VT_BOOL},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC_HUMAN_PRESENCE                    0x04B1
        {SENSOR_DATA_TYPE_HUMAN_PROXIMITY_METERS, VT_R4},                   //#define HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC_HUMAN_PROXIMITY_RANGE             0x04B2
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC_HUMAN_PROXIMITY_OUT_OF_RANGE      0x04B3
        {SENSOR_DATA_TYPE_TOUCH_STATE, VT_BOOL},                            //#define HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC_HUMAN_TOUCH_STATE                 0x04B4
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04B5
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04B6
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04B7
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04B8
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04B9
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04BA
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04BB
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04BC
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04BD
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04BE
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04BF
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04C0
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04C1
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04C2
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04C3
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04C4
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04C5
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04C6
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04C7
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04C8
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04C9
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04CA
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04CB
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04CC
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04CD
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04CE
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04CF
        ////data type light sensor
        {{SENSOR_DATA_TYPE_LIGHT_GUID, 0}, VT_EMPTY},                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT                                       0x04D0
        {SENSOR_DATA_TYPE_LIGHT_LEVEL_LUX, VT_R4},                          //#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE                           0x04D1
        {SENSOR_DATA_TYPE_LIGHT_TEMPERATURE_KELVIN, VT_R4},                 //#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE                     0x04D2
        {SENSOR_DATA_TYPE_LIGHT_CHROMACITY, VT_VECTOR|VT_UI1},              //#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY                          0x04D3
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY_X                        0x04D4
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY_Y                        0x04D5
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CONSUMER_IR_SENTENCE                  0x04D6
        {PROPERTYKEY_NULL, VT_EMPTY},                  //Ox04D7
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04D8
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04D9
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04DA
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04DB
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04DC
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04DD
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04DE
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04DF
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_LIGHT                                   0x04E0 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_LIGHT_CONSUMER_IR_SENTENCE_SEND         0x04E1 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04E2
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04E3
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04E4
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04E5
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04E6
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04E7
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04E8
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04E9
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04EA
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04EB
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04EC
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04ED
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04EE
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04EF
        ////data type scanner
        {{SENSOR_DATA_TYPE_SCANNER_GUID, 0}, VT_EMPTY},                     //#define HID_DRIVER_USAGE_SENSOR_DATA_SCANNER                                     0x04F0
        {SENSOR_DATA_TYPE_RFID_TAG_40_BIT, VT_UI8},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_SCANNER_RFID_TAG                            0x04F1
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_SCANNER_NFC_SENTENCE_RECEIVE                0x04F2
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04F3
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04F4
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04F5
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04F6
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04F7
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SCANNER                                 0x04F8 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SCANNER_NFC_SENTENCE_SEND               0x04F9 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04FA
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04FB
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04FC
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04FD
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04FE
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x04FF
        ////data type electrical
        {{SENSOR_DATA_TYPE_ELECTRICAL_GUID, 0}, VT_EMPTY},                  //#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL                                  0x0500
        {SENSOR_DATA_TYPE_CAPACITANCE_FARAD, VT_R8},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_CAPACITANCE                      0x0501
        {SENSOR_DATA_TYPE_CURRENT_AMPS, VT_R8},                             //#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_CURRENT                          0x0502
        {SENSOR_DATA_TYPE_ELECTRICAL_POWER_WATTS, VT_R8},                   //#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_POWER                            0x0503
        {SENSOR_DATA_TYPE_INDUCTANCE_HENRY, VT_R8},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_INDUCTANCE                       0x0504
        {SENSOR_DATA_TYPE_RESISTANCE_OHMS, VT_R8},                          //#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_RESISTANCE                       0x0505
        {SENSOR_DATA_TYPE_VOLTAGE_VOLTS, VT_R8},                            //#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_VOLTAGE                          0x0506
        {SENSOR_DATA_TYPE_ELECTRICAL_FREQUENCY_HERTZ, VT_R8},               //#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_FREQUENCY                        0x0507
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_PERIOD                           0x0508
        {SENSOR_DATA_TYPE_ELECTRICAL_PERCENT_OF_RANGE, VT_R8},              //#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_PERCENT_OF_RANGE                 0x0509
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x050A
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x050B
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x050C
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x050D
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x050E
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x050F
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0510
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0511
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0512
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0513
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0514
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0515
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0516
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0517
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0518
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0519
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x051A
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x051B
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x051C
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x051D
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x051E
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x051F
        ////data type time
        {{GUID_NULL, 0}, VT_EMPTY},                                         //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME                                        0x0520
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_YEAR                                   0x0521
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_MONTH                                  0x0522
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_DAY                                    0x0523
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_DAY_OF_WEEK                            0x0524
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_HOUR                                   0x0525
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_MINUTE                                 0x0526
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_SECOND                                 0x0527
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_MILLISECOND                            0x0528
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_TIMESTAMP                              0x0529
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_JULIAN_DAY_OF_YEAR                     0x052A
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x052B
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x052C
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x052D
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x052E
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x052F
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME                                    0x0530 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME_TIME_ZONE_OFFSET_FROM_UTC          0x0531 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME_TIME_ZONE_NAME                     0x0532 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME_DAYLIGHT_SAVINGS_TIME_OBSERVED     0x0533 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME_TIME_TRIM_ADJUSTMENT               0x0534 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME_ARM_ALARM                          0x0535 //property
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0536
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0537
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0538
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x0539
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x053A
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x053B
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x053C
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x053D
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x053E
        {PROPERTYKEY_NULL, VT_EMPTY},                  //0x053F
        ////data type custom
        {{SENSOR_DATA_TYPE_CUSTOM_GUID, 0}, VT_EMPTY},                      //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM                                      0x0540
        {SENSOR_DATA_TYPE_CUSTOM_USAGE, VT_UI4},                            //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_USAGE                                0x0541
        {SENSOR_DATA_TYPE_CUSTOM_BOOLEAN_ARRAY, VT_UI4},                    //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_BOOLEAN_ARRAY                        0x0542
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE                                0x0543
        {SENSOR_DATA_TYPE_CUSTOM_VALUE1, VT_EMPTY},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_1                              0x0544
        {SENSOR_DATA_TYPE_CUSTOM_VALUE2, VT_EMPTY},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_2                              0x0545
        {SENSOR_DATA_TYPE_CUSTOM_VALUE3, VT_EMPTY},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_3                              0x0546
        {SENSOR_DATA_TYPE_CUSTOM_VALUE4, VT_EMPTY},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_4                              0x0547
        {SENSOR_DATA_TYPE_CUSTOM_VALUE5, VT_EMPTY},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_5                              0x0548
        {SENSOR_DATA_TYPE_CUSTOM_VALUE6, VT_EMPTY},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_6                              0x0549
        {SENSOR_DATA_TYPE_CUSTOM_VALUE7, VT_EMPTY},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_7                              0x054A
        {SENSOR_DATA_TYPE_CUSTOM_VALUE8, VT_EMPTY},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_8                              0x054B
        {SENSOR_DATA_TYPE_CUSTOM_VALUE9, VT_EMPTY},                         //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_9                              0x054C
        {SENSOR_DATA_TYPE_CUSTOM_VALUE10, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_10                             0x054D
        {SENSOR_DATA_TYPE_CUSTOM_VALUE11, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_11                             0x054E
        {SENSOR_DATA_TYPE_CUSTOM_VALUE12, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_12                             0x054F
        {SENSOR_DATA_TYPE_CUSTOM_VALUE13, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_13                             0x0550
        {SENSOR_DATA_TYPE_CUSTOM_VALUE14, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_14                             0x0551
        {SENSOR_DATA_TYPE_CUSTOM_VALUE15, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_15                             0x0552
        {SENSOR_DATA_TYPE_CUSTOM_VALUE16, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_16                             0x0553
        {SENSOR_DATA_TYPE_CUSTOM_VALUE17, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_17                             0x0554
        {SENSOR_DATA_TYPE_CUSTOM_VALUE18, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_18                             0x0555
        {SENSOR_DATA_TYPE_CUSTOM_VALUE19, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_19                             0x0556
        {SENSOR_DATA_TYPE_CUSTOM_VALUE20, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_20                             0x0557
        {SENSOR_DATA_TYPE_CUSTOM_VALUE21, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_21                             0x0558
        {SENSOR_DATA_TYPE_CUSTOM_VALUE22, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_22                             0x0559
        {SENSOR_DATA_TYPE_CUSTOM_VALUE23, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_23                             0x055A
        {SENSOR_DATA_TYPE_CUSTOM_VALUE24, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_24                             0x055B
        {SENSOR_DATA_TYPE_CUSTOM_VALUE25, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_25                             0x055C
        {SENSOR_DATA_TYPE_CUSTOM_VALUE26, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_26                             0x055D
        {SENSOR_DATA_TYPE_CUSTOM_VALUE27, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_27                             0x055E
        {SENSOR_DATA_TYPE_CUSTOM_VALUE28, VT_EMPTY},                        //#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_28                             0x055F
        ////data type generic
        {{GUID_NULL, 0}, VT_EMPTY},                                         //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC                                     0x0560
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_GUID_OR_PROPERTYKEY                 0x0561
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_CATEGORY_GUID                       0x0562
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_TYPE_GUID                           0x0563
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_EVENT_PROPERTYKEY                   0x0564
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_PROPERTY_PROPERTYKEY                0x0565
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_DATAFIELD_PROPERTYKEY               0x0566
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_EVENT                               0x0567
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_PROPERTY                            0x0568
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_DATAFIELD                           0x0569
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_ENUMERATOR_TABLE_ROW_INDEX                  0x056A
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_ENUMERATOR_TABLE_ROW_COUNT                  0x056B
        {PROPERTYKEY_NULL, VT_EMPTY},                                       //#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_GUID_OR_PROPERTYKEY_KIND            0x056C // NAry
    };

    ULONG ulUsageIdx = 0;
    HRESULT hr = HRESULT_FROM_WIN32(ERROR_NOT_FOUND);

    if ((HID_DRIVER_USAGE_SENSOR_DATA_LOCATION > ulHidUsage) || (HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_DATAFIELD_PROPERTYKEY < ulHidUsage))
    {
        hr = E_INVALIDARG;
    }
    else
    {
        // The HID datafield usages start at 0x0400, so subtracting this from
        // the ulHidUsage creates an index into the 0-based translation table
        ulUsageIdx = ulHidUsage - 0x0400;

        *pk = SensorDataTypeTranslationTable[ulUsageIdx].pk;
        *vt = SensorDataTypeTranslationTable[ulUsageIdx].vt;

        if (!IsEqualPropertyKey(*pk, PROPERTYKEY_NULL))
        {
            hr = S_OK;
        }
    }

    return hr;
}



HRESULT CSensor::HandleDefinedDynamicDatafield(
    _In_ LONG Usage,
    _In_ USHORT ReportCount,
    _In_ LONG UnitsExp,
    _In_ LONG UsageValue,
    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2) PCHAR UsageArray)
{
    HRESULT hr = S_OK;

    PROPERTYKEY pkDatakey = {0};

    hr = HandleDynamicDatafield(
        pkDatakey,
        VT_UNKNOWN,
        Usage,
        ReportCount,
        UnitsExp,
        UsageValue,
        UsageArray
        );

    return hr;
}

HRESULT CSensor::HandleArbitraryDynamicDatafield(
    _In_ PROPERTYKEY pkDatakey,
    _In_ VARTYPE vtType,
    _In_ USHORT ReportCount,
    _In_ LONG UnitsExp,
    _In_ LONG UsageValue,
    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2) PCHAR UsageArray)
{
    HRESULT hr = S_OK;

    hr = HandleDynamicDatafield(
        pkDatakey,
        vtType,
        NULL,
        ReportCount,
        UnitsExp,
        UsageValue,
        UsageArray
        );

    return hr;
}

// Look in the data field translation table and see if the Usage is defined for Sensors.h
// if so, see if it has already been added to the data field collection and, if so, update the value
// if not, add the new data field to the collection, update the value, and add the datafield to
// the per-datafield properties
HRESULT CSensor::HandleDynamicDatafield(
    _In_ PROPERTYKEY pkDatakey,
    _In_ VARTYPE vtType,
    _In_ LONG Usage,
    _In_ USHORT ReportCount,
    _In_ LONG UnitsExp,
    _In_ LONG UsageValue,
    _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2) PCHAR UsageArray)
{
    HRESULT hr = S_OK;

    if (NULL == UsageArray)
    {
        hr = E_INVALIDARG;
    }

    PROPERTYKEY pkDatafield = {};
    PROPERTYKEY pkPropKey = {};
    PROPVARIANT value;
    VARTYPE     vt = VT_EMPTY;
    LONG        nValue = 0;
    FLOAT       fltValue = 0.0F;
    DOUBLE      dblValue = 0.0;
    ULONG       ulDatafieldCount = 0;
    BOOL        fDatafieldPresent;


    PropVariantInit(&value);

    if (NULL == Usage)
    {
        pkDatafield = pkDatakey;
        if ((vtType == VT_UNKNOWN) || (vtType == VT_EMPTY))
        {
            vt = VT_UNKNOWN;
        }
        else
        {
            vt = vtType;
        }
    }
    else
    {
        hr = TranslateHidUsageToSensorDataTypePropertyKey(Usage, &pkDatafield, &vt);
    }

    if (SUCCEEDED(hr))
    {
        //if not already present, add this property to the list of supported datafields
        ulDatafieldCount = 0;
        fDatafieldPresent = FALSE;
        DWORD dwDfIdx = 0;

        hr = m_spSupportedSensorDataFields->GetCount(&ulDatafieldCount);

        if(SUCCEEDED(hr))
        {
            for (ULONG i = 0; i < ulDatafieldCount; i++)
            {
                if (SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->GetAt(i, &pkPropKey);
                }

                if(SUCCEEDED(hr)  && (TRUE == IsEqualPropertyKey(pkDatafield, pkPropKey)))
                {
                    fDatafieldPresent = TRUE;
                }
            }

            if (FALSE == fDatafieldPresent)
            {
                hr = m_spSupportedSensorDataFields->Add(pkDatafield);
                                            
                // add the datafield to supported datafields
                if(SUCCEEDED(hr))
                {
                    hr = m_spSupportedSensorDataFields->GetCount(&ulDatafieldCount);

                    if (SUCCEEDED(hr))
                    {
                        for (dwDfIdx = 0; dwDfIdx < ulDatafieldCount; dwDfIdx++)
                        {
                            if (SUCCEEDED(hr))
                            {
                                hr = m_spSupportedSensorDataFields->GetAt(dwDfIdx, &pkPropKey);
                            }

                            if(SUCCEEDED(hr)  && (TRUE == IsEqualPropertyKey(pkDatafield, pkPropKey)))
                            {
                                m_DynamicDatafieldUsages[dwDfIdx] = Usage;
                                break;
                            }
                        }
                    }
                }

                // add the datafield to per-datafield properties
                if (SUCCEEDED(hr))
                {
                    DWORD  dwPropertyCount = 0;

                    hr = m_spSupportedSensorProperties->GetCount(&dwPropertyCount);

                    if (SUCCEEDED(hr))
                    {
                        // Only set the defaults if the property is supported
                        for (DWORD i = 0; i < dwPropertyCount; i++)
                        {
                            if (SUCCEEDED(hr))
                            {
                                hr = m_spSupportedSensorProperties->GetAt(i, &pkPropKey);
                            }

                            if(SUCCEEDED(hr)  && (SENSOR_PROPERTY_CHANGE_SENSITIVITY == pkPropKey))
                            {
                                //get the current values
                                CComPtr<IPortableDeviceValues>  spChangeSensitivityValues;
                                hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(pkPropKey, &spChangeSensitivityValues);

                                //add a value for the new datafield to the existing values
                                if(SUCCEEDED(hr))
                                {
                                    hr = spChangeSensitivityValues->SetFloatValue(pkDatafield, m_fltDefaultChangeSensitivity);
                                }

                                //set the property to the modified values
                                if (SUCCEEDED(hr))
                                {
                                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_CHANGE_SENSITIVITY, spChangeSensitivityValues);
                                }

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to add the per-datafield property value to %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                                                        
                            if(SUCCEEDED(hr)  && (SENSOR_PROPERTY_RANGE_MAXIMUM == pkPropKey))
                            {
                                //get the current values
                                CComPtr<IPortableDeviceValues>  spRangeMaximumValues;
                                hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(pkPropKey, &spRangeMaximumValues);

                                //add a value for the new datafield to the existing values
                                if(SUCCEEDED(hr))
                                {
                                    hr = spRangeMaximumValues->SetFloatValue(pkDatafield, m_fltDefaultRangeMaximum);
                                }

                                //set the property to the modified values
                                if (SUCCEEDED(hr))
                                {
                                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MAXIMUM, spRangeMaximumValues);
                                }

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to add the per-datafield property value to %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                                                        
                            if(SUCCEEDED(hr)  && (SENSOR_PROPERTY_RANGE_MINIMUM == pkPropKey))
                            {
                                //get the current values
                                CComPtr<IPortableDeviceValues>  spRangeMinimumValues;
                                hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(pkPropKey, &spRangeMinimumValues);

                                //add a value for the new datafield to the existing values
                                if(SUCCEEDED(hr))
                                {
                                    hr = spRangeMinimumValues->SetFloatValue(pkDatafield, m_fltDefaultRangeMinimum);
                                }

                                //set the property to the modified values
                                if (SUCCEEDED(hr))
                                {
                                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MINIMUM, spRangeMinimumValues);
                                }

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to add the per-datafield property value to %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                                                        
                            if(SUCCEEDED(hr)  && (SENSOR_PROPERTY_ACCURACY == pkPropKey))
                            {
                                //get the current values
                                CComPtr<IPortableDeviceValues>  spAccuracyValues;
                                hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(pkPropKey, &spAccuracyValues);

                                //add a value for the new datafield to the existing values
                                if(SUCCEEDED(hr))
                                {
                                    hr = spAccuracyValues->SetFloatValue(pkDatafield, m_fltDefaultAccuracy);
                                }

                                //set the property to the modified values
                                if (SUCCEEDED(hr))
                                {
                                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_ACCURACY, spAccuracyValues);
                                }

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to add the per-datafield property value to %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                                                        
                            if(SUCCEEDED(hr)  && (SENSOR_PROPERTY_RESOLUTION == pkPropKey))
                            {
                                //get the current values
                                CComPtr<IPortableDeviceValues>  spResolutionValues;
                                hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(pkPropKey, &spResolutionValues);

                                //add a value for the new datafield to the existing values
                                if(SUCCEEDED(hr))
                                {
                                    hr = spResolutionValues->SetFloatValue(pkDatafield, m_fltDefaultResolution);
                                }

                                //set the property to the modified values
                                if (SUCCEEDED(hr))
                                {
                                    hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RESOLUTION, spResolutionValues);
                                }

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to add the per-datafield property value to %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                        }
                    }
                }
            }
        }

        //set the datafield value
        if(SUCCEEDED(hr))
        {
            if (ReportCount == 1)
            {
                FLOAT fltMax = GetRangeMaximumValue(
                                m_fltDefaultRangeMaximum,
                                m_DynamicDatafieldMaximumSupported[dwDfIdx],
                                m_DynamicDatafieldMaximum[dwDfIdx],
                                m_DynamicDatafieldMaximumSupported[dwDfIdx],
                                m_DynamicDatafieldMaximum[dwDfIdx],
                                m_DynamicDatafieldMaximumSupported[dwDfIdx],
                                m_DynamicDatafieldMaximum[dwDfIdx]);

                FLOAT fltMin = GetRangeMinimumValue( 
                                m_fltDefaultRangeMinimum,
                                m_DynamicDatafieldMinimumSupported[dwDfIdx],
                                m_DynamicDatafieldMinimum[dwDfIdx],
                                m_DynamicDatafieldMinimumSupported[dwDfIdx],
                                m_DynamicDatafieldMinimum[dwDfIdx],
                                m_DynamicDatafieldMinimumSupported[dwDfIdx],
                                m_DynamicDatafieldMinimum[dwDfIdx]);

                switch (vt)
                {
                case VT_BOOL:
                    value.vt = VT_BOOL;
                    nValue = (UsageValue > 0) ? 1 : 0;
                    value.boolVal = (VARIANT_BOOL)nValue;
                    break;
                case VT_I4:
                case VT_UI4:
                    nValue = UsageValue;
                    if (((FLOAT)nValue > fltMax) || ((FLOAT)nValue < fltMin))
                    {
                        value.vt = VT_NULL;
                    }
                    else
                    {
                        value.vt = VT_UI4;
                        value.intVal = nValue;
                    }
                    break;
                case VT_R4:
                    fltValue = (FLOAT)ExtractDoubleFromUsageValue(0, UsageValue, UsageValue, UnitsExp);
                    if ((fltValue > fltMax) || (fltValue < fltMin))
                    {
                        value.vt = VT_NULL;
                    }
                    else
                    {
                        value.vt = VT_R4;
                        value.fltVal = fltValue;
                    }
                    break;
                case VT_R8:
                    dblValue = ExtractDoubleFromUsageValue(0, UsageValue, UsageValue, UnitsExp);
                    if ((dblValue > fltMax) || (dblValue < fltMin))
                    {
                        value.vt = VT_NULL;
                    }
                    else
                    {
                        value.vt = VT_R8;
                        value.dblVal = dblValue;
                    }
                    break;
                    break;
                case VT_UNKNOWN:
                case VT_EMPTY:
                default:
                    if (0 == UnitsExp)
                    {
                        nValue = UsageValue;
                        if (((FLOAT)nValue > fltMax) || ((FLOAT)nValue < fltMin))
                        {
                            value.vt = VT_NULL;
                        }
                        else
                        {
                            value.vt = VT_UI4;
                            value.intVal = nValue;
                        }
                    }
                    else
                    {
                        fltValue = (FLOAT)ExtractDoubleFromUsageValue(0, UsageValue, UsageValue, UnitsExp);
                        if ((fltValue > fltMax) || (fltValue < fltMin))
                        {
                            value.vt = VT_NULL;
                        }
                        else
                        {
                            value.vt = VT_R4;
                            value.fltVal = fltValue;
                        }
                    }
                    break;
                }

                hr = m_spSensorDataFieldValues->SetValue(pkDatafield, &value);
            }
            else
            {
                switch (vt)
                {
                case VT_UI8:
                    hr = InitPropVariantFromBuffer(UsageArray, sizeof(DWORD)*2, &value);
                    break;
                case VT_LPWSTR:
#pragma warning(push)
#pragma warning(disable:26035)
                    // the OACR warning is being disabled here because it expects a zero-terminated
                    // array for this particular type. However, above the same array is used for
                    // both VT_UI8 and (VT_VECTOR|VT_UI1) and these types have no such requirement
                    hr = m_spSensorDataFieldValues->SetStringValue(pkDatafield, (WCHAR*)UsageArray);
#pragma warning(pop)
                    break;
                case VT_UNKNOWN:
                    value.vt = VT_VECTOR|VT_UI1;
                    hr = InitPropVariantFromBuffer(UsageArray, ReportCount, &value);
                    break;
                case (VT_VECTOR | VT_UI1):
                case VT_EMPTY:
                default:
                    //treat this like a 
                    hr = InitPropVariantFromBuffer(UsageArray, ReportCount, &value);
                    break;
                }

                if (SUCCEEDED(hr))
                {
                    hr = m_spSensorDataFieldValues->SetValue(pkDatafield, &value);
                }
            }

            if (FAILED(hr))
            {
                Trace(TRACE_LEVEL_ERROR, "Failed to set the datafield value in %s, hr = %!HRESULT!", m_SensorName, hr);
            }
            else
            {
                m_DynamicDatafieldSupported[dwDfIdx] = TRUE;
            }
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Unable to add datafield to %s, hr = %!HRESULT!", m_SensorName, hr);
        }
    }

    PropVariantClear( &value );

    return hr;
}

HRESULT CSensor::HandleArbitraryDynamicDatafieldProperty(
    _In_ PROPERTYKEY pkDatakey,
    _In_ LONG UnitsExp,
    _In_ LONG UsageValue,
    _In_ USHORT UsageDataModifier)
{
    HRESULT hr = S_OK;

    hr = HandleDynamicDatafieldProperty(pkDatakey, NULL, UnitsExp, UsageValue, UsageDataModifier);

    return hr;
}

HRESULT CSensor::HandleDefinedDynamicDatafieldProperty(
    _In_ LONG Usage,
    _In_ LONG UnitsExp,
    _In_ LONG UsageValue,
    _In_ USHORT UsageDataModifier)
{
    HRESULT hr = S_OK;
    PROPERTYKEY pkNull = {0};

    hr = HandleDynamicDatafieldProperty(pkNull, Usage, UnitsExp, UsageValue, UsageDataModifier);

    return hr;
}
    
HRESULT CSensor::HandleDynamicDatafieldProperty(
    _In_ PROPERTYKEY pkDatakey,
    _In_ LONG Usage,
    _In_ LONG UnitsExp,
    _In_ LONG UsageValue,
    _In_ USHORT UsageDataModifier)
{
    HRESULT hr = S_OK;

    PROPERTYKEY pkDfKey = {0};
    VARTYPE vt = VT_R4;

    if (NULL != Usage)
    {
        hr = TranslateHidUsageToSensorDataTypePropertyKey(Usage, &pkDatakey, &vt);
    }

    if (SUCCEEDED(hr))
    {
        DWORD dwDfIdx = 0;
        DWORD cDfKeys = 0;

        hr = m_spSupportedSensorDataFields->GetCount(&cDfKeys);

        if (SUCCEEDED(hr) && (cDfKeys > 1))
        {
            for (dwDfIdx = 1; dwDfIdx < cDfKeys; dwDfIdx++) //ignore the timestamp
            {
                hr = m_spSupportedSensorDataFields->GetAt(dwDfIdx, &pkDfKey);

                if (TRUE == IsEqualPropertyKey(pkDatakey, pkDfKey))
                    break;
            }

            if (dwDfIdx < MAX_NUM_DATA_FIELDS)
            {
                switch(UsageDataModifier)
                {
                case HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS:
                    m_DynamicDatafieldSensitivitySupported[dwDfIdx] = TRUE;
                    m_DynamicDatafieldSensitivity[dwDfIdx] = (FLOAT)ExtractDoubleFromUsageValue(0, UsageValue, UsageValue, UnitsExp);
                    break;
                case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX:
                    m_DynamicDatafieldMaximumSupported[dwDfIdx] = TRUE;
                    m_DynamicDatafieldMaximum[dwDfIdx] = (FLOAT)ExtractDoubleFromUsageValue(0, UsageValue, UsageValue, UnitsExp);
                    break;
                case HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN:
                    m_DynamicDatafieldMinimumSupported[dwDfIdx] = TRUE;
                    m_DynamicDatafieldMinimum[dwDfIdx] = (FLOAT)ExtractDoubleFromUsageValue(0, UsageValue, UsageValue, UnitsExp);
                    break;
                case HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY:
                    m_DynamicDatafieldAccuracySupported[dwDfIdx] = TRUE;
                    m_DynamicDatafieldAccuracy[dwDfIdx] = (FLOAT)ExtractDoubleFromUsageValue(0, UsageValue, UsageValue, UnitsExp);
                    break;
                case HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION:
                    m_DynamicDatafieldResolutionSupported[dwDfIdx] = TRUE;
                    m_DynamicDatafieldResolution[dwDfIdx] = (FLOAT)ExtractDoubleFromUsageValue(0, UsageValue, UsageValue, UnitsExp);
                    break;

                default:
                    //modifier used is not supported for this data field
                    break;
                }
            }
        }
    }

    return hr;
}


HRESULT CSensor::HandleReportIntervalUpdate(
        _In_ UCHAR reportID, 
        _In_ BOOL fReportIntervalSupported, 
        _Out_ ULONG* pulReportInterval, 
        _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
        _In_ ULONG ulReportSize)
{
    HRESULT hr = S_OK;

    *pulReportInterval = 0;

    ULONG idx;
    ULONG ulDeviceReportInterval = 0;
    ULONG ulClientReportInterval = 0;

    USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;
    LONG   UnitsExp = 0;
    ULONG  Units = 0;

    HIDP_REPORT_TYPE ReportType = HidP_Feature;
    USAGE  UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
    USHORT LinkCollection = m_SensorLinkCollection;
    USAGE  Usage = 0;
    USHORT UsageDataModifier = 0;
    LONG   UsageValue = 0;
    ULONG  UsageUValue = 0;

    BYTE* pRecvReport = new BYTE[READ_BUFFER_SIZE];

    PROPVARIANT var;

    PropVariantInit(&var);

    if (nullptr != pRecvReport)
    {
        //extract the units and units exponent
        for(idx = 0; idx < numNodes; idx++)
        {
            if (reportID == m_FeatureValueCapsNodes[idx].ReportID)
            {
                Usage = m_FeatureValueCapsNodes[idx].NotRange.Usage;
                UnitsExp = m_FeatureValueCapsNodes[idx].UnitsExp;
                Units = m_FeatureValueCapsNodes[idx].Units;

                UsageUValue = 0;

                if (HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORT_INTERVAL == Usage)
                    break;
            }
        }
                    
        if ((HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLISECOND != Units) && (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED != Units))
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Incorrect units for ReportInterval in %s, hr = %!HRESULT!", m_SensorName, hr);
        }

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
                    if (*pulReportInterval == 0)
                    {
                        *pulReportInterval = m_ulDefaultCurrentReportInterval;
                    }
                }
                else
                {
                    *pulReportInterval = m_ulDefaultCurrentReportInterval;
                }
            }
            else
            {
                hr = m_spSensorPropertyValues->GetUnsignedIntegerValue(SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, &ulClientReportInterval);
            }
        }

        if (SUCCEEDED(hr))
        {
            Usage = HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORT_INTERVAL;
            UsageDataModifier = HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE;
            Usage = Usage | UsageDataModifier;

            // '0' to the sensor API means the default value
            if (ulClientReportInterval == 0)
            {
                *pulReportInterval = m_ulDefaultCurrentReportInterval;
            }
            else
            {
                *pulReportInterval = m_ulLowestClientReportInterval;
            }

            ulDeviceReportInterval = *pulReportInterval * (ULONG)pow( 10.0, (double)( 1.0 * UnitsExp ));
            UsageUValue = ulDeviceReportInterval;

            if (ulReportSize <= READ_BUFFER_SIZE)
            {
                if (ulDeviceReportInterval < 0xFFFFFFFF)
                {
                    hr = HidP_SetUsageValue(ReportType, UsagePage, LinkCollection, Usage, UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, ulReportSize);

                    // Write the report interval value to the device and then read back what the device has set the report interval to before
                    // setting this value for the DeviceProperties and at the API

                    const BOOL fExactCopyRequired = TRUE;
                    BOOL fIsExactCopy = FALSE;

                    if (SUCCEEDED(hr))
                    {
                        hr = SetThenGetSensorPropertiesFromDevice(pFeatureReport, ulReportSize, fExactCopyRequired, &fIsExactCopy);
                    }

                    if (SUCCEEDED(hr))
                    {
                        if (FALSE == fIsExactCopy)
                        {
                            Trace(TRACE_LEVEL_WARNING, "%s Feature Report returned is not exact copy, hr = %!HRESULT!", m_SensorName, hr);
                        }

                        //Extract the properties from the report buffer
                        if(SUCCEEDED(hr) && (TRUE == m_fFeatureReportSupported))
                        {
                            USHORT ReportCount = 0;

                            if (ulReportSize == m_pSensorManager->m_HidCaps.FeatureReportByteLength)
                            {
                                ULONG  BitSize = 0;

                                numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;

                                for(ULONG jdx = 0; jdx < numNodes; jdx++)
                                {
                                    if (reportID == m_FeatureValueCapsNodes[jdx].ReportID)
                                    {
                                        UsagePage = m_FeatureValueCapsNodes[jdx].UsagePage;
                                        Usage = m_FeatureValueCapsNodes[jdx].NotRange.Usage;
                                        ReportCount = m_FeatureValueCapsNodes[jdx].ReportCount;
                                        UnitsExp = m_FeatureValueCapsNodes[jdx].UnitsExp;
                                        BitSize = m_FeatureValueCapsNodes[jdx].BitSize;
                                        TranslateHidUnitsExp(&UnitsExp);

                                        if (ReportCount == 1)
                                        {
                                            UsageUValue = 0;
                                            hr = HidP_GetUsageValue(ReportType, UsagePage, LinkCollection, Usage, &UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, ulReportSize);

                                            if(SUCCEEDED(hr))
                                            {
                                                Usage = Usage & 0x0FFF; //remove the data modifier

                                                if (HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORT_INTERVAL == Usage)
                                                {
                                                    UsageValue = ExtractValueFromUsageUValue(m_FeatureValueCapsNodes[jdx].LogicalMin, BitSize, UsageUValue);

                                                    Trace(TRACE_LEVEL_INFORMATION, "Device setting Report Interval = %i on %s", *pulReportInterval, m_SensorName);

                                                    *pulReportInterval = (ULONG)ExtractLongLongFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 

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
                                Trace(TRACE_LEVEL_ERROR, "Feature report for %s is incorrect length, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }
                        // Set this possible changed value in DeviceProperties and at the API

                        if (SUCCEEDED(hr))
                        {
                            Trace(TRACE_LEVEL_INFORMATION, "Setting API Report Interval = %i on %s", *pulReportInterval, m_SensorName);
                            hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL, *pulReportInterval);
                        }
                        else
                        {
                            TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);
                            Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValue = Sensor ReportInterval in %s, hr = %!HRESULT!", m_SensorName, hr);
                        }
                    }
                    else
                    {
                        Trace(TRACE_LEVEL_ERROR, "Failed to set then get %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                    }
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Report size larger than buffer in HID feature report for %s, hr = %!HRESULT!", m_SensorName, hr);
            }
        }

        delete[] pRecvReport;
    }

    PropVariantClear(&var);

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

VOID CSensor::TraceHIDParseError(
                                    HRESULT hr, 
                                    SensorType sensType,
                                    HIDP_REPORT_TYPE ReportType,
                                    USHORT LinkCollection)
{
    UNREFERENCED_PARAMETER(sensType);
    UNREFERENCED_PARAMETER(ReportType);
    UNREFERENCED_PARAMETER(LinkCollection);

    if (SUCCEEDED(hr))
    {
        //should not get here
        Trace(TRACE_LEVEL_ERROR, "SUCCEEDED for %s, hr = %!HRESULT!", m_SensorName, hr);
    }
    else
    {
        if (ReportType == HidP_Input)
        {
            switch ((INT)hr)
            {
            case HIDP_STATUS_SUCCESS:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_SUCCESS in %s Input Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_INCOMPATIBLE_REPORT_ID:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_INCOMPATIBLE_REPORT_ID error in %s Input Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_INVALID_PREPARSED_DATA:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_INVALID_PREPARSED_DATA error in %s Input Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_REPORT_DOES_NOT_EXIST:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_REPORT_DOES_NOT_EXIST error in %s Input Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_USAGE_NOT_FOUND:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_USAGE_NOT_FOUND error in %s Input Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_INVALID_REPORT_LENGTH:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_INVALID_REPORT_LENGTH error in %s Input Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_INVALID_REPORT_TYPE:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_INVALID_REPORT_TYPE error in %s Input Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_BUFFER_TOO_SMALL:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_BUFFER_TOO_SMALL error in %s Input Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            default:
                Trace(TRACE_LEVEL_ERROR, "Unrecognized HID Parse error in %s Input Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            }
        }
        else if (ReportType == HidP_Feature)
        {
            switch ((INT)hr)
            {
            case HIDP_STATUS_SUCCESS:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_SUCCESS in %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_INCOMPATIBLE_REPORT_ID:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_INCOMPATIBLE_REPORT_ID error in %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_INVALID_PREPARSED_DATA:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_INVALID_PREPARSED_DATA error in %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_REPORT_DOES_NOT_EXIST:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_REPORT_DOES_NOT_EXIST error in %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_USAGE_NOT_FOUND:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_USAGE_NOT_FOUND error in %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_INVALID_REPORT_LENGTH:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_INVALID_REPORT_LENGTH error in %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_INVALID_REPORT_TYPE:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_INVALID_REPORT_TYPE error in %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            case HIDP_STATUS_BUFFER_TOO_SMALL:
                Trace(TRACE_LEVEL_ERROR, "HIDP_STATUS_BUFFER_TOO_SMALL error in %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            default:
                Trace(TRACE_LEVEL_ERROR, "Unrecognized HID Parse error in %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                break;
            }
        }
        else if (ReportType == HidP_Output)
        {
            Trace(TRACE_LEVEL_ERROR, "Output reports are not supported in %s, hr = %!HRESULT!", m_SensorName, hr);
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Unrecognized HID Parse error in %s, hr = %!HRESULT!", m_SensorName, hr);
        }
    }
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

HRESULT CSensor::HandleChangeSensitivityUpdate(
                                _In_ UCHAR reportID, 
                                _In_ BOOL fWriteToDevice,
                                _In_ BOOL fGlobalSensitivitySupported,
                                _In_ BOOL fBulkSensitivitySupported,
                                _In_ BOOL fDatafieldSensitivitySupported,
                                _In_ ULONG ulBulkUsage,
                                _In_ BOOL fDatafieldSupported,
                                _In_ const ULONG ulDatafieldUsage,
                                _In_ const ULONG ulDatafieldUnits,
                                _In_ PROPERTYKEY pkDatafield,
                                _In_ DWORD dwDfIdx,
                                _Inout_ FLOAT* pfltGlobalSensitivity, 
                                _Inout_ FLOAT* pfltBulkSensitivity, 
                                _Inout_ FLOAT* pfltDatafieldSensitivity, 
                                _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                _In_ ULONG uReportSize)
{
    UNREFERENCED_PARAMETER(pfltGlobalSensitivity);
    UNREFERENCED_PARAMETER(pfltBulkSensitivity);

    HRESULT hr = S_OK;

    ULONG  idx;
    FLOAT  fltDesiredDatafieldSensitivity = 0.0F;
    FLOAT  fltRequiredDatafieldSensitivityAtApi = 0.0F;

    USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;
    USAGE  Usage = 0;
    USHORT UsageModifier = 0;
    LONG   UsageValue = 0;
    ULONG  UsageUValue = 0;
    LONG   UnitsExp = 0;
    ULONG  BitSize = 0;
    ULONG  Units = 0;
    BYTE*  pRecvReport = new BYTE[READ_BUFFER_SIZE];
    PROPERTYKEY_DWVALUE_PAIR PropkeyDwValuePair = { {GUID_NULL, 0}, 0};

    CComPtr<IPortableDeviceValues>  spSensitivityValues;

    if (nullptr != pRecvReport)
    {
        if (TRUE == fDatafieldSupported)
        {
            hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_CHANGE_SENSITIVITY, &spSensitivityValues);

            if (SUCCEEDED(hr))
            {
                //extract the units and units exponent
                for(idx = 0; idx < numNodes; idx++)
                {
                    if (reportID == m_FeatureValueCapsNodes[idx].ReportID)
                    {
                        Usage = m_FeatureValueCapsNodes[idx].NotRange.Usage;
                        Units = m_FeatureValueCapsNodes[idx].Units;
                        UnitsExp = m_FeatureValueCapsNodes[idx].UnitsExp;
                        BitSize = m_FeatureValueCapsNodes[idx].BitSize;
                        TranslateHidUnitsExp(&UnitsExp);

                        UsageValue = 0;
                        UsageUValue = 0;
                        UsageModifier = Usage & 0xF000; //extract the data modifier

                        if ((HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_ABS == Usage) 
                            || ((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS) == Usage)
                            || ((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS) == Usage) //for bulk datafields
                            || (HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_REL_PCT == Usage) 
                            || ((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_REL_PCT) == Usage)
                            || ((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_REL_PCT) == Usage) //for bulk datafields                    
                            || ((HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_ABS) == (Usage & 0xF000)) //for dynamic datafields                    
                            || (5478 == Usage) //for dynamic datafields                    
                            )
                        {
                            //remove the data modifier
                            Usage = Usage & 0x0FFF;

                            if ((ulDatafieldUnits != Units) && (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED != Units))
                            {
                                if ((HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS != UsageModifier) && (HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_REL_PCT != UsageModifier)) //let bulk datafield usage pass
                                {
                                    hr = E_UNEXPECTED;
                                    Trace(TRACE_LEVEL_ERROR, "Incorrect units for Sensitivity in %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }

                            if (SUCCEEDED(hr))
                            {
                                fltDesiredDatafieldSensitivity = m_fltLowestClientChangeSensitivities[dwDfIdx];

                                USAGE  ModifiedUsage = 0;

                                ModifiedUsage = Usage | UsageModifier;

                                UsageUValue = ExtractUsageUValueFromDouble((double)fltDesiredDatafieldSensitivity, UnitsExp, BitSize);
                                if (uReportSize <= READ_BUFFER_SIZE)
                                {
                                    if (TRUE == fWriteToDevice)
                                    {
                                        if (NULL != ulDatafieldUsage)
                                        {
                                            hr = HidP_SetUsageValue(HidP_Feature, HID_DRIVER_USAGE_PAGE_SENSOR, m_SensorLinkCollection, ModifiedUsage, UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                                        }
                                        else
                                        {
                                            if ((HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_ABS == Usage) || (HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_REL_PCT == Usage))
                                            {
                                                hr = HidP_SetUsageValue(HidP_Feature, HID_DRIVER_USAGE_PAGE_SENSOR, m_SensorLinkCollection, ModifiedUsage, UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                                            }
                                            else
                                            {
                                                PropkeyDwValuePair.propkeyProperty = pkDatafield;
                                                PropkeyDwValuePair.dwPropertyValue = UsageUValue;

                                                hr = HidP_SetUsageValueArray(HidP_Feature, HID_DRIVER_USAGE_PAGE_SENSOR, m_SensorLinkCollection, ModifiedUsage, (PCHAR)&PropkeyDwValuePair, sizeof(PropkeyDwValuePair), m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                                            }
                                        }

                                        // Write the change sensitivity value to the device and then read back what the device has set the report interval to before
                                        // setting this value for the DeviceProperties and at the API

                                        const BOOL fExactCopyRequired = TRUE;
                                        BOOL fIsExactCopy = FALSE;

                                        if (SUCCEEDED(hr))
                                        {
                                            Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Change Sensitivity Usage = 0x%x be set = %f", m_SensorName, ModifiedUsage, fltDesiredDatafieldSensitivity);
                                            hr = SetThenGetSensorPropertiesFromDevice(pFeatureReport, uReportSize, fExactCopyRequired, &fIsExactCopy);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            if (FALSE == fIsExactCopy)
                                            {
                                                Trace(TRACE_LEVEL_WARNING, "%s Feature Report returned is not exact copy, hr = %!HRESULT!", m_SensorName, hr);
                                            }

                                            //Extract the properties from the report buffer
                                            if(SUCCEEDED(hr) && (TRUE == m_fFeatureReportSupported))
                                            {
                                                HIDP_REPORT_TYPE ReportType = HidP_Feature;
                                                USAGE UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
                                                USHORT LinkCollection = m_SensorLinkCollection;
                                                USHORT ReportCount = 0;

                                                if (uReportSize == m_pSensorManager->m_HidCaps.FeatureReportByteLength)
                                                {
                                                    numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;

                                                    for(ULONG jdx = 0; jdx < numNodes; jdx++)
                                                    {
                                                        if (reportID == m_FeatureValueCapsNodes[jdx].ReportID)
                                                        {
                                                            UsagePage = m_FeatureValueCapsNodes[jdx].UsagePage;
                                                            Usage = m_FeatureValueCapsNodes[jdx].NotRange.Usage;
                                                            ReportCount = m_FeatureValueCapsNodes[jdx].ReportCount;
                                                            UnitsExp = m_FeatureValueCapsNodes[jdx].UnitsExp;
                                                            BitSize = m_FeatureValueCapsNodes[jdx].BitSize;
                                                            TranslateHidUnitsExp(&UnitsExp);

                                                            if ((HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_ABS == Usage) 
                                                                || ((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS) == Usage)
                                                                || ((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS) == Usage) //for bulk datafields
                                                                || (HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_REL_PCT == Usage) 
                                                                || ((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_REL_PCT) == Usage)
                                                                || ((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_REL_PCT) == Usage) //for bulk datafields                    
                                                                || ((HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_ABS) == (Usage & 0xF000)) //for dynamic datafields                    
                                                                || (HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_DATAFIELD_PROPERTYKEY == (Usage & 0x0FFF)) //for arbitrary dynamic datafields
                                                                )
                                                            {
                                                                if (ReportCount > 1)
                                                                {
                                                                    PropkeyDwValuePair.propkeyProperty.fmtid = GUID_NULL;
                                                                    PropkeyDwValuePair.propkeyProperty.pid = 0;
                                                                    PropkeyDwValuePair.dwPropertyValue = 0;

                                                                    ZeroMemory(&PropkeyDwValuePair, sizeof(PropkeyDwValuePair));
                                                                    hr = HidP_GetUsageValueArray(ReportType, UsagePage, LinkCollection, Usage, (PCHAR)&PropkeyDwValuePair, sizeof(PropkeyDwValuePair), m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                                                                }
                                                                else if (ReportCount == 1)
                                                                {
                                                                    UsageUValue = 0;
                                                                    hr = HidP_GetUsageValue(ReportType, UsagePage, LinkCollection, Usage, &UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                                                                }
                                                                else
                                                                {
                                                                    hr = E_UNEXPECTED;
                                                                    Trace(TRACE_LEVEL_ERROR, "Feature Report Count == 0 in %s, hr = %!HRESULT!", m_SensorName, hr);
                                                                }

                                                                if(SUCCEEDED(hr))
                                                                {

                                                                    if (NULL != ulDatafieldUsage)
                                                                    {
                                                                        UsageValue = ExtractValueFromUsageUValue(m_FeatureValueCapsNodes[jdx].LogicalMin, BitSize, UsageUValue);
                                                                    }
                                                                    else
                                                                    {
                                                                        if (ReportCount > 1)
                                                                        {
                                                                            UsageValue = ExtractValueFromUsageUValue(m_FeatureValueCapsNodes[jdx].LogicalMin, BitSize, PropkeyDwValuePair.dwPropertyValue);
                                                                        }
                                                                        else
                                                                        {
                                                                            UsageValue = ExtractValueFromUsageUValue(m_FeatureValueCapsNodes[jdx].LogicalMin, BitSize, UsageUValue);
                                                                        }
                                                                    }

                                                                    if ((TRUE == fDatafieldSensitivitySupported) 
                                                                        && (((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS) == Usage) || ((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_REL_PCT) == Usage))
                                                                        )
                                                                    {
                                                                        *pfltDatafieldSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                                        fltRequiredDatafieldSensitivityAtApi = *pfltDatafieldSensitivity;
                                                                        Trace(TRACE_LEVEL_INFORMATION, "Device %s Datafield Change Sensitivity Usage = 0x%x was set = %f", m_SensorName, Usage, fltRequiredDatafieldSensitivityAtApi);
                                                                    }
                                                                    else if ((TRUE == fDatafieldSensitivitySupported) 
                                                                        && (HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS == (Usage & 0xF000))
                                                                        )
                                                                    {
                                                                        *pfltDatafieldSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                                        fltRequiredDatafieldSensitivityAtApi = *pfltDatafieldSensitivity;
                                                                        Trace(TRACE_LEVEL_INFORMATION, "Device %s Datafield Change Sensitivity Usage = 0x%x was set = %f", m_SensorName, Usage, fltRequiredDatafieldSensitivityAtApi);
                                                                    }
                                                                    else if ((TRUE == fBulkSensitivitySupported) 
                                                                        && (((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS) == Usage) || ((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_REL_PCT) == Usage))
                                                                        )
                                                                    {
                                                                        *pfltBulkSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                                        fltRequiredDatafieldSensitivityAtApi = *pfltBulkSensitivity;
                                                                        Trace(TRACE_LEVEL_INFORMATION, "Device %s Bulk Change Sensitivity Usage = 0x%x was set = %f", m_SensorName, Usage, fltRequiredDatafieldSensitivityAtApi);
                                                                    }
                                                                    else if ((TRUE == fGlobalSensitivitySupported)
                                                                        && ((HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_ABS == Usage) || (HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_REL_PCT == Usage))
                                                                        )
                                                                    {
                                                                        *pfltGlobalSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                                        fltRequiredDatafieldSensitivityAtApi = *pfltGlobalSensitivity;
                                                                        Trace(TRACE_LEVEL_INFORMATION, "Device %s Global Change Sensitivity Usage = 0x%x was set = %f", m_SensorName, Usage, fltRequiredDatafieldSensitivityAtApi);
                                                                    }
                                                                    else if ((TRUE == fGlobalSensitivitySupported) 
                                                                        && (HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS == (Usage & 0xF000))
                                                                        )
                                                                    {
                                                                        *pfltDatafieldSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                                        fltRequiredDatafieldSensitivityAtApi = *pfltDatafieldSensitivity;
                                                                        Trace(TRACE_LEVEL_INFORMATION, "Device %s Global Change Sensitivity Usage = 0x%x was set = %f", m_SensorName, Usage, fltRequiredDatafieldSensitivityAtApi);
                                                                    }
                                                                    else if ((FALSE == fGlobalSensitivitySupported) && (FALSE == fBulkSensitivitySupported) && (FALSE == fDatafieldSensitivitySupported))
                                                                    {
                                                                        fltRequiredDatafieldSensitivityAtApi = m_fltDefaultChangeSensitivity;
                                                                        Trace(TRACE_LEVEL_INFORMATION, "%s Change Sensitivity was set = %f", m_SensorName, fltRequiredDatafieldSensitivityAtApi);
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
                                                    Trace(TRACE_LEVEL_ERROR, "Feature report is incorrect length in %s, hr = %!HRESULT!", m_SensorName, hr);
                                                }
                                            }

                                            if ((SUCCEEDED(hr) && (TRUE == fDatafieldSupported)))
                                            {
                                                Trace(TRACE_LEVEL_INFORMATION, "%s Change Sensitivity for Key = %!GUID!-%i was set = %f", m_SensorName, &pkDatafield.fmtid, pkDatafield.pid, fltRequiredDatafieldSensitivityAtApi);
                                                hr = spSensitivityValues->SetFloatValue(pkDatafield, fltRequiredDatafieldSensitivityAtApi);
                                            }
                                            else
                                            {
                                                TraceHIDParseError(hr, m_SensorType, HidP_Feature, reportID);
                                                Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValue = Sensor Sensitivity in %s, hr = %!HRESULT!", m_SensorName, hr);
                                            }
                                        }
                                        else
                                        {
                                            Trace(TRACE_LEVEL_ERROR, "Failed to set then get %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                                        }
                                    }
                                }
                                else
                                {
                                    hr = E_UNEXPECTED;
                                    Trace(TRACE_LEVEL_ERROR, "Buffer provided is too large = Sensor Sensitivity in %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
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

        delete[] pRecvReport;
    }
                 
    return hr;
}


HRESULT CSensor::HandleMaximumUpdate(
                                _In_ UCHAR reportID, 
                                _In_ BOOL fGlobalMaximumSupported,
                                _In_ BOOL fBulkMaximumSupported,
                                _In_ BOOL fDatafieldMaximumSupported,
                                _In_ ULONG ulBulkUsage,
                                _In_ BOOL fDatafieldSupported,
                                _In_ const ULONG ulDatafieldUsage,
                                _In_ const ULONG ulDatafieldUnits,
                                _In_ PROPERTYKEY pkDatafield,
                                _In_ DWORD dwDfIdx,
                                _Inout_ FLOAT* pfltGlobalMaximum, 
                                _Inout_ FLOAT* pfltBulkMaximum, 
                                _Inout_ FLOAT* pfltDatafieldMaximum, 
                                _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                _In_ ULONG uReportSize)
{
    UNREFERENCED_PARAMETER(dwDfIdx);

    HRESULT hr = S_OK;

    ULONG  idx;
    FLOAT  fltRequiredDatafieldMaximumAtApi = 0.0F;

    USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;
    USAGE  Usage = 0;
    USHORT UsageModifier = 0;
    LONG   UsageValue = 0;
    ULONG  UsageUValue = 0;
    LONG   UnitsExp = 0;
    ULONG  Units = 0;

    CComPtr<IPortableDeviceValues>  spMaximumValues;

    if (TRUE == fDatafieldSupported)
    {
        hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MAXIMUM, &spMaximumValues);

        if (SUCCEEDED(hr))
        {
            //extract the units and units exponent
            for(idx = 0; idx < numNodes; idx++)
            {
                if (reportID == m_FeatureValueCapsNodes[idx].ReportID)
                {
                    Usage = m_FeatureValueCapsNodes[idx].NotRange.Usage;
                    Units = m_FeatureValueCapsNodes[idx].Units;
                    UnitsExp = m_FeatureValueCapsNodes[idx].UnitsExp;
                    TranslateHidUnitsExp(&UnitsExp);

                    UsageValue = 0;
                    UsageUValue = 0;
                    UsageModifier = Usage & 0xF000; //extract the data modifier

                    if ((HID_DRIVER_USAGE_SENSOR_PROPERTY_RANGE_MAXIMUM == Usage) 
                        || ((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX) == Usage)
                        || ((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX) == Usage) //for bulk datafields
                        )
                    {
                        //remove the data modifier
                        Usage = Usage & 0x0FFF;

                        if ((ulDatafieldUnits != Units) && (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED != Units))
                        {
                            if (HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX != UsageModifier) //let bulk datafield usage pass
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Incorrect units for Maximum in %s, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }

                        if (SUCCEEDED(hr))
                        {
                            if (uReportSize <= READ_BUFFER_SIZE)
                            {
                                //Extract the properties from the report buffer
                                if(SUCCEEDED(hr) && (TRUE == m_fFeatureReportSupported))
                                {
                                    HIDP_REPORT_TYPE ReportType = HidP_Feature;
                                    USAGE UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
                                    USHORT LinkCollection = m_SensorLinkCollection;
                                    USHORT ReportCount = 0;

                                    if (uReportSize == m_pSensorManager->m_HidCaps.FeatureReportByteLength)
                                    {
                                        ULONG  BitSize = 0;

                                        numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;

                                        for(ULONG jdx = 0; jdx < numNodes; jdx++)
                                        {
                                            if (reportID == m_FeatureValueCapsNodes[jdx].ReportID)
                                            {
                                                UsagePage = m_FeatureValueCapsNodes[jdx].UsagePage;
                                                Usage = m_FeatureValueCapsNodes[jdx].NotRange.Usage;
                                                ReportCount = m_FeatureValueCapsNodes[jdx].ReportCount;
                                                UnitsExp = m_FeatureValueCapsNodes[jdx].UnitsExp;
                                                BitSize = m_FeatureValueCapsNodes[jdx].BitSize;
                                                TranslateHidUnitsExp(&UnitsExp);

                                                if (ReportCount == 1)
                                                {
                                                    UsageUValue = 0;
                                                    hr = HidP_GetUsageValue(ReportType, UsagePage, LinkCollection, Usage, &UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

                                                    if(SUCCEEDED(hr))
                                                    {
                                                        UsageValue = ExtractValueFromUsageUValue(m_FeatureValueCapsNodes[jdx].LogicalMin, BitSize, UsageUValue);

                                                        if ((TRUE == fDatafieldMaximumSupported) 
                                                            && (((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX) == Usage))
                                                            )
                                                        {
                                                            *pfltDatafieldMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldMaximumAtApi = *pfltDatafieldMaximum;
                                                        }
                                                        else if ((TRUE == fBulkMaximumSupported) 
                                                            && (((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX) == Usage))
                                                            )
                                                        {
                                                            *pfltBulkMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldMaximumAtApi = *pfltBulkMaximum;
                                                        }
                                                        else if ((TRUE == fGlobalMaximumSupported)
                                                            && ((HID_DRIVER_USAGE_SENSOR_PROPERTY_RANGE_MAXIMUM == Usage))
                                                            )
                                                        {
                                                            *pfltGlobalMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldMaximumAtApi = *pfltGlobalMaximum;
                                                        }
                                                        else if ((FALSE == fGlobalMaximumSupported) && (FALSE == fBulkMaximumSupported) && (FALSE == fDatafieldMaximumSupported))
                                                        {
                                                            fltRequiredDatafieldMaximumAtApi = FLT_MAX;
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
                                        Trace(TRACE_LEVEL_ERROR, "Feature report is incorrect length in %s, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                }

                                if ((SUCCEEDED(hr) && (TRUE == fDatafieldSupported)))
                                {
                                    Trace(TRACE_LEVEL_INFORMATION, "%s Change Maximum for Key = %!GUID!-%i was set = %f", m_SensorName, &pkDatafield.fmtid, pkDatafield.pid, fltRequiredDatafieldMaximumAtApi);
                                    hr = spMaximumValues->SetFloatValue(pkDatafield, fltRequiredDatafieldMaximumAtApi);
                                }
                                else
                                {
                                    TraceHIDParseError(hr, m_SensorType, HidP_Feature, reportID);
                                    Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValue = Sensor Maximum in %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Buffer provided is too large = Sensor Maximum in %s, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }
                    }
                }
            }

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MAXIMUM, spMaximumValues);
            }
        }
    }
                 
    return hr;
 }


 HRESULT CSensor::HandleMinimumUpdate(
                                _In_ UCHAR reportID, 
                                _In_ BOOL fGlobalMinimumSupported,
                                _In_ BOOL fBulkMinimumSupported,
                                _In_ BOOL fDatafieldMinimumSupported,
                                _In_ ULONG ulBulkUsage,
                                _In_ BOOL fDatafieldSupported,
                                _In_ const ULONG ulDatafieldUsage,
                                _In_ const ULONG ulDatafieldUnits,
                                _In_ PROPERTYKEY pkDatafield,
                                _In_ DWORD dwDfIdx,
                                _Inout_ FLOAT* pfltGlobalMinimum, 
                                _Inout_ FLOAT* pfltBulkMinimum, 
                                _Inout_ FLOAT* pfltDatafieldMinimum, 
                                _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                _In_ ULONG uReportSize)
{
    UNREFERENCED_PARAMETER(dwDfIdx);

    HRESULT hr = S_OK;

    ULONG  idx;
    FLOAT  fltRequiredDatafieldMinimumAtApi = 0.0F;

    USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;
    USAGE  Usage = 0;
    USHORT UsageModifier = 0;
    LONG   UsageValue = 0;
    ULONG  UsageUValue = 0;
    LONG   UnitsExp = 0;
    ULONG  Units = 0;

    CComPtr<IPortableDeviceValues>  spMinimumValues;

    if (TRUE == fDatafieldSupported)
    {
        hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MINIMUM, &spMinimumValues);

        if (SUCCEEDED(hr))
        {
            //extract the units and units exponent
            for(idx = 0; idx < numNodes; idx++)
            {
                if (reportID == m_FeatureValueCapsNodes[idx].ReportID)
                {
                    Usage = m_FeatureValueCapsNodes[idx].NotRange.Usage;
                    Units = m_FeatureValueCapsNodes[idx].Units;
                    UnitsExp = m_FeatureValueCapsNodes[idx].UnitsExp;
                    TranslateHidUnitsExp(&UnitsExp);

                    UsageValue = 0;
                    UsageUValue = 0;
                    UsageModifier = Usage & 0xF000; //extract the data modifier

                    if ((HID_DRIVER_USAGE_SENSOR_PROPERTY_RANGE_MINIMUM == Usage) 
                        || ((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN) == Usage)
                        || ((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN) == Usage) //for bulk datafields
                        )
                    {
                        //remove the data modifier
                        Usage = Usage & 0x0FFF;

                        if ((ulDatafieldUnits != Units) && (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED != Units))
                        {
                            if (HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN != UsageModifier) //let bulk datafield usage pass
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Incorrect units for Minimum in %s, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }

                        if (SUCCEEDED(hr))
                        {
                            if (uReportSize <= READ_BUFFER_SIZE)
                            {
                                //Extract the properties from the report buffer
                                if(SUCCEEDED(hr) && (TRUE == m_fFeatureReportSupported))
                                {
                                    HIDP_REPORT_TYPE ReportType = HidP_Feature;
                                    USAGE UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
                                    USHORT LinkCollection = m_SensorLinkCollection;
                                    USHORT ReportCount = 0;

                                    if (uReportSize == m_pSensorManager->m_HidCaps.FeatureReportByteLength)
                                    {
                                        ULONG  BitSize = 0;

                                        numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;

                                        for(ULONG jdx = 0; jdx < numNodes; jdx++)
                                        {
                                            if (reportID == m_FeatureValueCapsNodes[jdx].ReportID)
                                            {
                                                UsagePage = m_FeatureValueCapsNodes[jdx].UsagePage;
                                                Usage = m_FeatureValueCapsNodes[jdx].NotRange.Usage;
                                                ReportCount = m_FeatureValueCapsNodes[jdx].ReportCount;
                                                UnitsExp = m_FeatureValueCapsNodes[jdx].UnitsExp;
                                                BitSize = m_FeatureValueCapsNodes[jdx].BitSize;
                                                TranslateHidUnitsExp(&UnitsExp);

                                                if (ReportCount == 1)
                                                {
                                                    UsageUValue = 0;
                                                    hr = HidP_GetUsageValue(ReportType, UsagePage, LinkCollection, Usage, &UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

                                                    if(SUCCEEDED(hr))
                                                    {
                                                        UsageValue = ExtractValueFromUsageUValue(m_FeatureValueCapsNodes[jdx].LogicalMin, BitSize, UsageUValue);

                                                        if ((TRUE == fDatafieldMinimumSupported) 
                                                            && (((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN) == Usage))
                                                            )
                                                        {
                                                            *pfltDatafieldMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldMinimumAtApi = *pfltDatafieldMinimum;
                                                        }
                                                        else if ((TRUE == fBulkMinimumSupported) 
                                                            && (((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN) == Usage))
                                                            )
                                                        {
                                                            *pfltBulkMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldMinimumAtApi = *pfltBulkMinimum;
                                                        }
                                                        else if ((TRUE == fGlobalMinimumSupported)
                                                            && ((HID_DRIVER_USAGE_SENSOR_PROPERTY_RANGE_MINIMUM == Usage))
                                                            )
                                                        {
                                                            *pfltGlobalMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldMinimumAtApi = *pfltGlobalMinimum;
                                                        }
                                                        else if ((FALSE == fGlobalMinimumSupported) && (FALSE == fBulkMinimumSupported) && (FALSE == fDatafieldMinimumSupported))
                                                        {
                                                            fltRequiredDatafieldMinimumAtApi = FLT_MAX;
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
                                        Trace(TRACE_LEVEL_ERROR, "Feature report is incorrect length in %s, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                }

                                if ((SUCCEEDED(hr) && (TRUE == fDatafieldSupported)))
                                {
                                    Trace(TRACE_LEVEL_INFORMATION, "%s Change Minimum for Key = %!GUID!-%i was set = %f", m_SensorName, &pkDatafield.fmtid, pkDatafield.pid, fltRequiredDatafieldMinimumAtApi);
                                    hr = spMinimumValues->SetFloatValue(pkDatafield, fltRequiredDatafieldMinimumAtApi);
                                }
                                else
                                {
                                    TraceHIDParseError(hr, m_SensorType, HidP_Feature, reportID);
                                    Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValue = Sensor Minimum in %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Buffer provided is too large = Sensor Minimum in %s, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }
                    }
                }
            }

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_RANGE_MINIMUM, spMinimumValues);
            }
        }
    }
                 
    return hr;
 }


HRESULT CSensor::HandleAccuracyUpdate(
                                _In_ UCHAR reportID, 
                                _In_ BOOL fGlobalAccuracySupported,
                                _In_ BOOL fBulkAccuracySupported,
                                _In_ BOOL fDatafieldAccuracySupported,
                                _In_ ULONG ulBulkUsage,
                                _In_ BOOL fDatafieldSupported,
                                _In_ const ULONG ulDatafieldUsage,
                                _In_ const ULONG ulDatafieldUnits,
                                _In_ PROPERTYKEY pkDatafield,
                                _In_ DWORD dwDfIdx,
                                _Inout_ FLOAT* pfltGlobalAccuracy, 
                                _Inout_ FLOAT* pfltBulkAccuracy, 
                                _Inout_ FLOAT* pfltDatafieldAccuracy, 
                                _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                _In_ ULONG uReportSize)
{
    UNREFERENCED_PARAMETER(dwDfIdx);

    HRESULT hr = S_OK;

    ULONG  idx;
    FLOAT  fltRequiredDatafieldAccuracyAtApi = 0.0F;

    USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;
    USAGE  Usage = 0;
    USHORT UsageModifier = 0;
    LONG   UsageValue = 0;
    ULONG  UsageUValue = 0;
    LONG   UnitsExp = 0;
    ULONG  Units = 0;

    CComPtr<IPortableDeviceValues>  spAccuracyValues;

    if (TRUE == fDatafieldSupported)
    {
        hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_ACCURACY, &spAccuracyValues);

        if (SUCCEEDED(hr))
        {
            //extract the units and units exponent
            for(idx = 0; idx < numNodes; idx++)
            {
                if (reportID == m_FeatureValueCapsNodes[idx].ReportID)
                {
                    Usage = m_FeatureValueCapsNodes[idx].NotRange.Usage;
                    Units = m_FeatureValueCapsNodes[idx].Units;
                    UnitsExp = m_FeatureValueCapsNodes[idx].UnitsExp;
                    TranslateHidUnitsExp(&UnitsExp);

                    UsageValue = 0;
                    UsageUValue = 0;
                    UsageModifier = Usage & 0xF000; //extract the data modifier

                    if ((HID_DRIVER_USAGE_SENSOR_PROPERTY_ACCURACY == Usage) 
                        || ((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY) == Usage)
                        || ((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY) == Usage) //for bulk datafields
                        )
                    {
                        //remove the data modifier
                        Usage = Usage & 0x0FFF;

                        if ((ulDatafieldUnits != Units) && (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED != Units))
                        {
                            if (HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY != UsageModifier) //let bulk datafield usage pass
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Incorrect units for Accuracy in %s, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }

                        if (SUCCEEDED(hr))
                        {
                            if (uReportSize <= READ_BUFFER_SIZE)
                            {
                                //Extract the properties from the report buffer
                                if(SUCCEEDED(hr) && (TRUE == m_fFeatureReportSupported))
                                {
                                    HIDP_REPORT_TYPE ReportType = HidP_Feature;
                                    USAGE UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
                                    USHORT LinkCollection = m_SensorLinkCollection;
                                    USHORT ReportCount = 0;

                                    if (uReportSize == m_pSensorManager->m_HidCaps.FeatureReportByteLength)
                                    {
                                        ULONG  BitSize = 0;

                                        numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;

                                        for(ULONG jdx = 0; jdx < numNodes; jdx++)
                                        {
                                            if (reportID == m_FeatureValueCapsNodes[jdx].ReportID)
                                            {
                                                UsagePage = m_FeatureValueCapsNodes[jdx].UsagePage;
                                                Usage = m_FeatureValueCapsNodes[jdx].NotRange.Usage;
                                                ReportCount = m_FeatureValueCapsNodes[jdx].ReportCount;
                                                UnitsExp = m_FeatureValueCapsNodes[jdx].UnitsExp;
                                                BitSize = m_FeatureValueCapsNodes[jdx].BitSize;
                                                TranslateHidUnitsExp(&UnitsExp);

                                                if (ReportCount == 1)
                                                {
                                                    UsageUValue = 0;
                                                    hr = HidP_GetUsageValue(ReportType, UsagePage, LinkCollection, Usage, &UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

                                                    if(SUCCEEDED(hr))
                                                    {
                                                        UsageValue = ExtractValueFromUsageUValue(m_FeatureValueCapsNodes[jdx].LogicalMin, BitSize, UsageUValue);

                                                        if ((TRUE == fDatafieldAccuracySupported) 
                                                            && (((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY) == Usage))
                                                            )
                                                        {
                                                            *pfltDatafieldAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldAccuracyAtApi = *pfltDatafieldAccuracy;
                                                        }
                                                        else if ((TRUE == fBulkAccuracySupported) 
                                                            && (((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY) == Usage))
                                                            )
                                                        {
                                                            *pfltBulkAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldAccuracyAtApi = *pfltBulkAccuracy;
                                                        }
                                                        else if ((TRUE == fGlobalAccuracySupported)
                                                            && ((HID_DRIVER_USAGE_SENSOR_PROPERTY_ACCURACY == Usage))
                                                            )
                                                        {
                                                            *pfltGlobalAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldAccuracyAtApi = *pfltGlobalAccuracy;
                                                        }
                                                        else if ((FALSE == fGlobalAccuracySupported) && (FALSE == fBulkAccuracySupported) && (FALSE == fDatafieldAccuracySupported))
                                                        {
                                                            fltRequiredDatafieldAccuracyAtApi = FLT_MAX;
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
                                        Trace(TRACE_LEVEL_ERROR, "Feature report is incorrect length in %s, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                }

                                if ((SUCCEEDED(hr) && (TRUE == fDatafieldSupported)))
                                {
                                    Trace(TRACE_LEVEL_INFORMATION, "%s Change Accuracy for Key = %!GUID!-%i was set = %f", m_SensorName, &pkDatafield.fmtid, pkDatafield.pid, fltRequiredDatafieldAccuracyAtApi);
                                    hr = spAccuracyValues->SetFloatValue(pkDatafield, fltRequiredDatafieldAccuracyAtApi);
                                }
                                else
                                {
                                    TraceHIDParseError(hr, m_SensorType, HidP_Feature, reportID);
                                    Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValue = Sensor Accuracy in %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Buffer provided is too large = Sensor Accuracy in %s, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }
                    }
                }
            }

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetIPortableDeviceValuesValue(SENSOR_PROPERTY_ACCURACY, spAccuracyValues);
            }
        }
    }
                 
    return hr;
 }


HRESULT CSensor::HandleResolutionUpdate(
                                _In_ UCHAR reportID, 
                                _In_ BOOL fGlobalResolutionSupported,
                                _In_ BOOL fBulkResolutionSupported,
                                _In_ BOOL fDatafieldResolutionSupported,
                                _In_ ULONG ulBulkUsage,
                                _In_ BOOL fDatafieldSupported,
                                _In_ const ULONG ulDatafieldUsage,
                                _In_ const ULONG ulDatafieldUnits,
                                _In_ PROPERTYKEY pkDatafield,
                                _In_ DWORD dwDfIdx,
                                _Inout_ FLOAT* pfltGlobalResolution, 
                                _Inout_ FLOAT* pfltBulkResolution, 
                                _Inout_ FLOAT* pfltDatafieldResolution, 
                                _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                _In_ ULONG uReportSize)
{
    UNREFERENCED_PARAMETER(dwDfIdx);

    HRESULT hr = S_OK;

    ULONG  idx;
    FLOAT  fltRequiredDatafieldResolutionAtApi = 0.0F;

    USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;
    USAGE  Usage = 0;
    USHORT UsageModifier = 0;
    LONG   UsageValue = 0;
    ULONG  UsageUValue = 0;
    LONG   UnitsExp = 0;
    ULONG  Units = 0;

    CComPtr<IPortableDeviceValues>  spResolutionValues;

    if (TRUE == fDatafieldSupported)
    {
        hr = m_spSensorPropertyValues->GetIPortableDeviceValuesValue(SENSOR_PROPERTY_RESOLUTION, &spResolutionValues);

        if (SUCCEEDED(hr))
        {
            //extract the units and units exponent
            for(idx = 0; idx < numNodes; idx++)
            {
                if (reportID == m_FeatureValueCapsNodes[idx].ReportID)
                {
                    Usage = m_FeatureValueCapsNodes[idx].NotRange.Usage;
                    Units = m_FeatureValueCapsNodes[idx].Units;
                    UnitsExp = m_FeatureValueCapsNodes[idx].UnitsExp;
                    TranslateHidUnitsExp(&UnitsExp);

                    UsageValue = 0;
                    UsageUValue = 0;
                    UsageModifier = Usage & 0xF000; //extract the data modifier

                    if ((HID_DRIVER_USAGE_SENSOR_PROPERTY_RESOLUTION == Usage) 
                        || ((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION) == Usage)
                        || ((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION) == Usage) //for bulk datafields
                        )
                    {
                        //remove the data modifier
                        Usage = Usage & 0x0FFF;

                        if ((ulDatafieldUnits != Units) && (HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED != Units))
                        {
                            if (HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION != UsageModifier) //let bulk datafield usage pass
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Incorrect units for Resolution in %s, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }

                        if (SUCCEEDED(hr))
                        {
                            if (uReportSize <= READ_BUFFER_SIZE)
                            {
                                //Extract the properties from the report buffer
                                if(SUCCEEDED(hr) && (TRUE == m_fFeatureReportSupported))
                                {
                                    HIDP_REPORT_TYPE ReportType = HidP_Feature;
                                    USAGE UsagePage = HID_DRIVER_USAGE_PAGE_SENSOR;
                                    USHORT LinkCollection = m_SensorLinkCollection;
                                    USHORT ReportCount = 0;

                                    if (uReportSize == m_pSensorManager->m_HidCaps.FeatureReportByteLength)
                                    {
                                        ULONG  BitSize = 0;

                                        numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;

                                        for(ULONG jdx = 0; jdx < numNodes; jdx++)
                                        {
                                            if (reportID == m_FeatureValueCapsNodes[jdx].ReportID)
                                            {
                                                UsagePage = m_FeatureValueCapsNodes[jdx].UsagePage;
                                                Usage = m_FeatureValueCapsNodes[jdx].NotRange.Usage;
                                                ReportCount = m_FeatureValueCapsNodes[jdx].ReportCount;
                                                UnitsExp = m_FeatureValueCapsNodes[jdx].UnitsExp;
                                                BitSize = m_FeatureValueCapsNodes[jdx].BitSize;
                                                TranslateHidUnitsExp(&UnitsExp);

                                                if (ReportCount == 1)
                                                {
                                                    UsageUValue = 0;
                                                    hr = HidP_GetUsageValue(ReportType, UsagePage, LinkCollection, Usage, &UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

                                                    if(SUCCEEDED(hr))
                                                    {
                                                        UsageValue = ExtractValueFromUsageUValue(m_FeatureValueCapsNodes[jdx].LogicalMin, BitSize, UsageUValue);

                                                        if ((TRUE == fDatafieldResolutionSupported) 
                                                            && (((ulDatafieldUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION) == Usage))
                                                            )
                                                        {
                                                            *pfltDatafieldResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldResolutionAtApi = *pfltDatafieldResolution;
                                                        }
                                                        else if ((TRUE == fBulkResolutionSupported) 
                                                            && (((ulBulkUsage | HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION) == Usage))
                                                            )
                                                        {
                                                            *pfltBulkResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldResolutionAtApi = *pfltBulkResolution;
                                                        }
                                                        else if ((TRUE == fGlobalResolutionSupported)
                                                            && ((HID_DRIVER_USAGE_SENSOR_PROPERTY_RESOLUTION == Usage))
                                                            )
                                                        {
                                                            *pfltGlobalResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[jdx].LogicalMin, UsageUValue, UsageValue, UnitsExp); 
                                                            fltRequiredDatafieldResolutionAtApi = *pfltGlobalResolution;
                                                        }
                                                        else if ((FALSE == fGlobalResolutionSupported) && (FALSE == fBulkResolutionSupported) && (FALSE == fDatafieldResolutionSupported))
                                                        {
                                                            fltRequiredDatafieldResolutionAtApi = FLT_MAX;
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
                                        Trace(TRACE_LEVEL_ERROR, "Feature report is incorrect length in %s, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                }

                                if ((SUCCEEDED(hr) && (TRUE == fDatafieldSupported)))
                                {
                                    Trace(TRACE_LEVEL_INFORMATION, "%s Change Resolution for Key = %!GUID!-%i was set = %f", m_SensorName, &pkDatafield.fmtid, pkDatafield.pid, fltRequiredDatafieldResolutionAtApi);
                                    hr = spResolutionValues->SetFloatValue(pkDatafield, fltRequiredDatafieldResolutionAtApi);
                                }
                                else
                                {
                                    TraceHIDParseError(hr, m_SensorType, HidP_Feature, reportID);
                                    Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValue = Sensor Resolution in %s, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Buffer provided is too large = Sensor Resolution in %s, hr = %!HRESULT!", m_SensorName, hr);
                            }
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
                 
    return hr;
 }

HRESULT CSensor::HandleGetHidInputSelectors(
                                    _Inout_ BOOL* pfSensorStateSelectorSupported,
                                    _Inout_ USHORT* pusSensorState,
                                    _Inout_ BOOL* pfEventTypeSelectorSupported,
                                    _Inout_ USHORT* pusEventType,
                                    _In_ HIDP_REPORT_TYPE ReportType, 
                                    _In_ USAGE UsagePage, 
                                    _In_ USHORT LinkCollection, 
                                    _In_reads_bytes_(MAX_NUM_HID_USAGES*2) USAGE* pUsageList, 
                                    _In_ ULONG* pNumUsages, 
                                    _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pInputReport, 
                                    _In_ ULONG uReportSize)
{
    HRESULT hr = S_OK;

    if ((nullptr == pfSensorStateSelectorSupported)
        || (nullptr == pusSensorState)
        || (nullptr == pfEventTypeSelectorSupported)
        || (nullptr == pusEventType)
        || (nullptr == pUsageList)
        || (nullptr == pNumUsages)
        || (nullptr == pInputReport)
        )
    {
        hr = E_POINTER;
        return hr;
    }

    if (nullptr != m_pSensorManager)
    {
        if (nullptr != m_pSensorManager->m_pLinkCollectionNodes)
        {
            if (m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren > 0)
            {
                if ((uReportSize < READ_BUFFER_SIZE) && (*pNumUsages <= MAX_NUM_HID_USAGES))
                {
                    if (SUCCEEDED(hr))
                    {
                        for (USHORT usIdx = 1; usIdx <= m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren; usIdx++)
                        {
                            ULONG ulNumUsages = *pNumUsages;

                            hr = HidP_GetUsages(ReportType, UsagePage, LinkCollection+usIdx, pUsageList, &ulNumUsages, m_pSensorManager->m_pPreparsedData, (PCHAR)pInputReport, uReportSize);

                            if (SUCCEEDED(hr))
                            {
                                for (USHORT usUdx = 0; usUdx < ulNumUsages; usUdx++)
                                {
                                    USAGE tempUsage = pUsageList[usUdx];

                                    switch(tempUsage)
                                    {

                                    case 0:
                                        //no entry
                                        break;

                                    //Common sensor properties

                                    case HID_DRIVER_USAGE_SENSOR_STATE_UNKNOWN_SEL:
                                        *pfSensorStateSelectorSupported = TRUE;
                                        *pusSensorState = SENSOR_STATE_NOT_AVAILABLE;
                                        hr = m_pSensorManager->SetState(this, SENSOR_STATE_NOT_AVAILABLE);
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_STATE_READY_SEL:
                                        *pfSensorStateSelectorSupported = TRUE;
                                        *pusSensorState = SENSOR_STATE_READY;
                                        hr = m_pSensorManager->SetState(this, SENSOR_STATE_READY);
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_STATE_NOT_AVAILABLE_SEL:
                                        *pfSensorStateSelectorSupported = TRUE;
                                        *pusSensorState = SENSOR_STATE_NOT_AVAILABLE;
                                        hr = m_pSensorManager->SetState(this, SENSOR_STATE_NOT_AVAILABLE);
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_STATE_NO_DATA_SEL:
                                        *pfSensorStateSelectorSupported = TRUE;
                                        *pusSensorState = SENSOR_STATE_NO_DATA;
                                        hr = m_pSensorManager->SetState(this, SENSOR_STATE_NO_DATA);
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_STATE_INITIALIZING_SEL:
                                        *pfSensorStateSelectorSupported = TRUE;
                                        *pusSensorState = SENSOR_STATE_INITIALIZING;
                                        hr = m_pSensorManager->SetState(this, SENSOR_STATE_INITIALIZING);
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_STATE_ACCESS_DENIED_SEL:
                                        *pfSensorStateSelectorSupported = TRUE;
                                        *pusSensorState = SENSOR_STATE_ACCESS_DENIED;
                                        hr = m_pSensorManager->SetState(this, SENSOR_STATE_ACCESS_DENIED);
                                        break;
                                    case HID_DRIVER_USAGE_SENSOR_STATE_ERROR_SEL:
                                        *pfSensorStateSelectorSupported = TRUE;
                                        *pusSensorState = SENSOR_STATE_ERROR;
                                        hr = m_pSensorManager->SetState(this, SENSOR_STATE_ERROR);
                                        break;


                                    case HID_DRIVER_USAGE_SENSOR_EVENT_UNKNOWN_SEL:
                                        *pfEventTypeSelectorSupported = TRUE;
                                        *pusEventType = SENSOR_EVENT_TYPE_UNKNOWN;
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_EVENT_STATE_CHANGED_SEL:
                                        *pfEventTypeSelectorSupported = TRUE;
                                        *pusEventType = SENSOR_EVENT_TYPE_STATE_CHANGED;
                                        m_fSensorStateChanged = TRUE;
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_EVENT_PROPERTY_CHANGED_SEL:
                                        *pfEventTypeSelectorSupported = TRUE;
                                        *pusEventType = SENSOR_EVENT_TYPE_PROPERTY_CHANGED;
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_EVENT_DATA_UPDATED_SEL:
                                        *pfEventTypeSelectorSupported = TRUE;
                                        *pusEventType = SENSOR_EVENT_TYPE_DATA_UPDATED;
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_EVENT_POLL_RESPONSE_SEL:
                                        *pfEventTypeSelectorSupported = TRUE;
                                        *pusEventType = SENSOR_EVENT_TYPE_POLL_RESPONSE;
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_EVENT_CHANGE_SENSITIVITY_SEL:
                                        *pfEventTypeSelectorSupported = TRUE;
                                        *pusEventType = SENSOR_EVENT_TYPE_CHANGE_SENSITIVITY;
                                        break;


                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_INTEGRATED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_ATTACHED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_EXTERNAL_SEL:
                                        //used in feature reports, so no action
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_WAKE_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_WAKE_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_WAKE_SEL:
                                        //used in feature reports, so no action
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_UNDEFINED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D2_STANDBY_WITH_WAKE_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D3_SLEEP_WITH_WAKE_SEL:
                                        //used in feature reports, so no action
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_EVENT_MAX_REACHED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_MIN_REACHED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_UPWARD_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_DOWNWARD_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_UPWARD_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_DOWNWARD_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_UPWARD_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_DOWNWARD_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_PERIOD_EXCEEDED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_FREQUENCY_EXCEEDED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_COMPLEX_TRIGGER_SEL:
                                        Trace(TRACE_LEVEL_ERROR, "Unsupported %s input selector, hr = %!HRESULT!, Usage = %i", m_SensorName, hr, tempUsage);
                                        break;

                                    default:
                                        Trace(TRACE_LEVEL_ERROR, "Unrecognized %s input selector, hr = %!HRESULT!, Usage = %i", m_SensorName, hr, tempUsage);
                                        break;
                                    }
                                }
                            }
                            else if (HIDP_STATUS_USAGE_NOT_FOUND == (INT)hr)
                            {
                                hr = S_OK;
                            }
                            else
                            {
                                TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);

                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Failure to get %s button caps type: feature, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }
                    }
                }
                else
                {
                    hr = E_UNEXPECTED;
                    Trace(TRACE_LEVEL_ERROR, "Input buffer for %s too large, hr = %!HRESULT!", m_SensorName, hr);
                }
            }
        }
        else
        {
            hr = E_POINTER;
            Trace(TRACE_LEVEL_ERROR, "%s m_pLinkCollectionNodes is null, hr = %!HRESULT!", m_SensorName, hr);
        }
    }
    else
    {
        hr = E_POINTER;
        Trace(TRACE_LEVEL_ERROR, "m_pSensorManager is null, hr = %!HRESULT!", hr);
    }

    return hr;
}

 
HRESULT CSensor::HandleCommonInputValues(
                                _In_ ULONG idx,
                                _Inout_ BOOL* pfSensorStateSupported,
                                _Inout_ USHORT* pusSensorState,
                                _Inout_ BOOL* pfEventTypeSupported,
                                _Inout_ USHORT* pusEventType,
                                _In_ USAGE Usage,
                                _In_ LONG UsageValue,
                                _In_ ULONG UsageUValue,
                                _In_ LONG UnitsExp,
                                _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2) CHAR*  pUsageArray,
                                _Out_ BOOL* pfInputHandled)
{
    UNREFERENCED_PARAMETER(idx);
    UNREFERENCED_PARAMETER(UsageUValue);
    UNREFERENCED_PARAMETER(UnitsExp);
    UNREFERENCED_PARAMETER(pUsageArray);

    HRESULT hr = S_OK;

    if ((nullptr == pfSensorStateSupported)
        || (nullptr == pusSensorState)
        || (nullptr == pfEventTypeSupported)
        || (nullptr == pusEventType)
        || (nullptr == pfInputHandled)
        )
    {
        hr = E_POINTER;
        return hr;
    }

    PROPVARIANT value;

    *pfInputHandled = TRUE;

    switch(Usage)
    {

    case HID_DRIVER_USAGE_SENSOR_STATE:
        *pfSensorStateSupported = TRUE;
        *pusSensorState = (USHORT)UsageValue;
        m_SensorState = *pusSensorState;
        if (*pusSensorState >= HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_ERROR)
        {
            hr = m_pSensorManager->SetState(this, SENSOR_STATE_ERROR);
        }
        else
        {
            switch (*pusSensorState)
            {
            case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_READY:
                hr = m_pSensorManager->SetState(this, SENSOR_STATE_READY);
                break;
            case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_NO_DATA:
                hr = m_pSensorManager->SetState(this, SENSOR_STATE_NO_DATA);
                break;
            case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_INITIALIZING:
                hr = m_pSensorManager->SetState(this, SENSOR_STATE_INITIALIZING);
                break;
            case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_ACCESS_DENIED:
                hr = m_pSensorManager->SetState(this, SENSOR_STATE_ACCESS_DENIED);
                break;

            case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_UNKNOWN:
            case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_NOT_AVAILABLE:
            default:
                hr = m_pSensorManager->SetState(this, SENSOR_STATE_NOT_AVAILABLE);
                break;
            }
        }
        if (FALSE == m_fWarnedOnUseOfInputSensorState)
        {
            Trace(TRACE_LEVEL_WARNING, "%s is using deprecated form of Sensor State instead of NAry/Selectors in the Input Report Descriptor", m_SensorName);
            m_fWarnedOnUseOfInputSensorState = TRUE;
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_EVENT:
        *pfEventTypeSupported = TRUE;
        *pusEventType = (USHORT)UsageValue;
        m_EventType = *pusEventType;
        if (FALSE == m_fWarnedOnUseOfInputEventType)
        {
            Trace(TRACE_LEVEL_WARNING, "%s is using deprecated form of Event Type instead of NAry/Selectors in the Input Report Descriptor", m_SensorName);
            m_fWarnedOnUseOfInputEventType = TRUE;
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_USAGE:
        PropVariantInit( &value );
        if (FALSE == m_fHidUsagePropertySupported)
        {
            m_fHidUsagePropertySupported = TRUE;

            hr = m_spSupportedSensorDataFields->Add(SENSOR_DATA_TYPE_CUSTOM_USAGE);

            if (SUCCEEDED(hr))
            {
                hr = InitPerDataFieldProperties(SENSOR_DATA_TYPE_CUSTOM_USAGE);
            }

            if (SUCCEEDED(hr))
            {
                // Also add the PROPERTYKEY to the list of supported properties
                hr = m_spSupportedSensorProperties->Add(SENSOR_PROPERTY_HID_USAGE);

                if(SUCCEEDED(hr))
                {
                    PropVariantClear(&value);
                    value.vt = VT_EMPTY;

                    hr = m_spSensorPropertyValues->SetValue(SENSOR_PROPERTY_HID_USAGE, &value);

                    if (SUCCEEDED(hr))
                    {
                        hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_USAGE, &value);
                    }
                }
            }
        }
        if (SUCCEEDED(hr))
        {
            value.vt = VT_UI4;
            value.intVal = UsageUValue;
        }
        else
        {
            value.vt = VT_EMPTY;
        }
        if (SUCCEEDED(hr))
        {
            hr = SetDataField(SENSOR_DATA_TYPE_CUSTOM_USAGE, &value);

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetValue(SENSOR_PROPERTY_HID_USAGE, &value);
            }
        }
        break;

    default:
        *pfInputHandled = FALSE;
        break;

    }

    return hr;
}

 
HRESULT CSensor::HandleGetHidFeatureSelectors(
                                _Inout_ BOOL* pfReportingStateSelectorSupported,
                                _Inout_ ULONG* pulReportingStateSelector,
                                _Inout_ BOOL* pfPowerStateSelectorSupported,
                                _Inout_ ULONG* pulPowerStateSelector,
                                _Inout_ BOOL* pfSensorStatusSelectorSupported,
                                _Inout_ ULONG* pulSensorStatusSelector,
                                _Inout_ BOOL* pfConnectionTypeSelectorSupported,
                                _Inout_ ULONG* pulConnectionTypeSelector,
                                _In_ HIDP_REPORT_TYPE ReportType, 
                                _In_ USAGE UsagePage, 
                                _In_ USHORT LinkCollection, 
                                _In_reads_bytes_(MAX_NUM_HID_USAGES*2) USAGE* pUsageList, 
                                _In_ ULONG* pNumUsages, 
                                _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                _In_ ULONG uReportSize)
 {
    HRESULT hr = S_OK;

    if ((nullptr == pfReportingStateSelectorSupported)
        || (nullptr == pulReportingStateSelector)
        || (nullptr == pfPowerStateSelectorSupported)
        || (nullptr == pulPowerStateSelector)
        || (nullptr == pfSensorStatusSelectorSupported)
        || (nullptr == pulSensorStatusSelector)
        || (nullptr == pfConnectionTypeSelectorSupported)
        || (nullptr == pulConnectionTypeSelector)
        || (nullptr == pUsageList)
        || (nullptr == pNumUsages)
        || (nullptr == pFeatureReport)
        )
    {
        hr = E_POINTER;
        return hr;
    }

    if (nullptr != m_pSensorManager)
    {
        if (nullptr != m_pSensorManager->m_pLinkCollectionNodes)
        {
            if (m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren > 0)
            {
                if ((uReportSize < READ_BUFFER_SIZE) && (*pNumUsages <= MAX_NUM_HID_USAGES))
                {
                    if (SUCCEEDED(hr))
                    {
                        for (USHORT usIdx = 1; usIdx <= m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren; usIdx++)
                        {
                            ULONG ulNumUsages = *pNumUsages;

                            hr = HidP_GetUsages(ReportType, UsagePage, LinkCollection+usIdx, pUsageList, &ulNumUsages, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

                            if (SUCCEEDED(hr))
                            {
                                for (USHORT usUdx = 0; usUdx < ulNumUsages; usUdx++)
                                {
                                    USAGE tempUsage = pUsageList[usUdx];

                                    switch(tempUsage)
                                    {
                                    case 0:
                                        //no entry
                                        break;

                                    //Common sensor properties
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_WAKE_SEL:
                                        *pfReportingStateSelectorSupported = TRUE;
                                        *pulReportingStateSelector = HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_ENUM;
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_WAKE_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_WAKE_SEL:
                                        *pfReportingStateSelectorSupported = TRUE;
                                        *pulReportingStateSelector = HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_ENUM;
                                        break;


                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_SEL:
                                        *pfPowerStateSelectorSupported = TRUE;
                                        *pulPowerStateSelector = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_ENUM;
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_SEL:
                                        *pfPowerStateSelectorSupported = TRUE;
                                        *pulPowerStateSelector = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_ENUM;
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_SEL:
                                        *pfPowerStateSelectorSupported = TRUE;
                                        *pulPowerStateSelector = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_ENUM;
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_UNDEFINED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D2_STANDBY_WITH_WAKE_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D3_SLEEP_WITH_WAKE_SEL:
                                        *pfPowerStateSelectorSupported = TRUE;
                                        *pulPowerStateSelector = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_UNDEFINED_ENUM;
                                        Trace(TRACE_LEVEL_ERROR, "Unsupported %s feature selector, hr = %!HRESULT!, Usage = %i", m_SensorName, hr, tempUsage);
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_STATE_UNKNOWN_SEL:
                                        *pfSensorStatusSelectorSupported = TRUE;
                                        *pulSensorStatusSelector = HID_DRIVER_USAGE_SENSOR_STATE_UNKNOWN_ENUM;
                                        if (FALSE == m_fInitialPropertiesReceived)
                                        {
                                            hr = m_pSensorManager->SetState(this, SENSOR_STATE_NOT_AVAILABLE);
                                        }
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_STATE_READY_SEL:
                                        *pfSensorStatusSelectorSupported = TRUE;
                                        *pulSensorStatusSelector = HID_DRIVER_USAGE_SENSOR_STATE_READY_ENUM;
                                        if (FALSE == m_fInitialPropertiesReceived)
                                        {
                                            hr = m_pSensorManager->SetState(this, SENSOR_STATE_READY);
                                        }
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_STATE_NOT_AVAILABLE_SEL:
                                        *pfSensorStatusSelectorSupported = TRUE;
                                        *pulSensorStatusSelector = HID_DRIVER_USAGE_SENSOR_STATE_NOT_AVAILABLE_ENUM;
                                        if (FALSE == m_fInitialPropertiesReceived)
                                        {
                                            hr = m_pSensorManager->SetState(this, SENSOR_STATE_NOT_AVAILABLE);
                                        }
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_STATE_NO_DATA_SEL:
                                        *pfSensorStatusSelectorSupported = TRUE;
                                        *pulSensorStatusSelector = HID_DRIVER_USAGE_SENSOR_STATE_NO_DATA_ENUM;
                                        if (FALSE == m_fInitialPropertiesReceived)
                                        {
                                            hr = m_pSensorManager->SetState(this, SENSOR_STATE_NO_DATA);
                                        }
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_STATE_INITIALIZING_SEL:
                                        *pfSensorStatusSelectorSupported = TRUE;
                                        *pulSensorStatusSelector = HID_DRIVER_USAGE_SENSOR_STATE_INITIALIZING_ENUM;
                                        if (FALSE == m_fInitialPropertiesReceived)
                                        {
                                            hr = m_pSensorManager->SetState(this, SENSOR_STATE_INITIALIZING);
                                        }
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_STATE_ACCESS_DENIED_SEL:
                                        *pfSensorStatusSelectorSupported = TRUE;
                                        *pulSensorStatusSelector = HID_DRIVER_USAGE_SENSOR_STATE_ACCESS_DENIED_ENUM;
                                        if (FALSE == m_fInitialPropertiesReceived)
                                        {
                                            hr = m_pSensorManager->SetState(this, SENSOR_STATE_ACCESS_DENIED);
                                        }
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_STATE_ERROR_SEL:
                                        *pfSensorStatusSelectorSupported = TRUE;
                                        *pulSensorStatusSelector = HID_DRIVER_USAGE_SENSOR_STATE_ERROR_ENUM;
                                        if (FALSE == m_fInitialPropertiesReceived)
                                        {
                                            hr = m_pSensorManager->SetState(this, SENSOR_STATE_ERROR);
                                        }
                                        break;


                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_INTEGRATED_SEL:
                                        *pfConnectionTypeSelectorSupported = TRUE;
                                        *pulConnectionTypeSelector = SENSOR_CONNECTION_TYPE_PC_INTEGRATED;
                                        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CONNECTION_TYPE, *pulConnectionTypeSelector);
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_ATTACHED_SEL:
                                        *pfConnectionTypeSelectorSupported = TRUE;
                                        *pulConnectionTypeSelector = SENSOR_CONNECTION_TYPE_PC_ATTACHED;
                                        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CONNECTION_TYPE, *pulConnectionTypeSelector);
                                        if (FALSE == m_fWarnedOnTypeOfFeatureConnectionType)
                                        {
                                            Trace(TRACE_LEVEL_WARNING, "%s ConnectionType is not PC_INTEGRATED - be sure this is correct", m_SensorName);
                                            m_fWarnedOnTypeOfFeatureConnectionType = TRUE;
                                        }
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_EXTERNAL_SEL:
                                        *pfConnectionTypeSelectorSupported = TRUE;
                                        *pulConnectionTypeSelector = SENSOR_CONNECTION_TYPE_PC_EXTERNAL;
                                        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CONNECTION_TYPE, *pulConnectionTypeSelector);
                                        if (FALSE == m_fWarnedOnTypeOfFeatureConnectionType)
                                        {
                                            Trace(TRACE_LEVEL_WARNING, "%s ConnectionType is not PC_INTEGRATED - be sure this is correct", m_SensorName);
                                            m_fWarnedOnTypeOfFeatureConnectionType = TRUE;
                                        }
                                        break;

                                    case HID_DRIVER_USAGE_SENSOR_EVENT_UNKNOWN_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_STATE_CHANGED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_PROPERTY_CHANGED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_DATA_UPDATED_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_POLL_RESPONSE_SEL:
                                    case HID_DRIVER_USAGE_SENSOR_EVENT_CHANGE_SENSITIVITY_SEL:
                                        //used in input reports - no action
                                        break;

                                    default:
                                        Trace(TRACE_LEVEL_ERROR, "Unrecognized feature %s selector, hr = %!HRESULT!, Usage = %i", m_SensorName, hr, tempUsage);
                                        break;
                                    }
                                }
                            }
                            else if (HIDP_STATUS_USAGE_NOT_FOUND == (INT)hr)
                            {
                                hr = S_OK;
                            }
                            else
                            {
                                TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);

                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Failure to get %s button caps type: feature, hr = %!HRESULT!", m_SensorName, hr);
                            }
                        }
                    }
                }
                else
                {
                    hr = E_UNEXPECTED;
                    Trace(TRACE_LEVEL_ERROR, "Feature buffer for %s too large, hr = %!HRESULT!", m_SensorName, hr);
                }
            }
        }
        else
        {
            hr = E_POINTER;
            Trace(TRACE_LEVEL_ERROR, "%s m_pLinkCollectionNodes is null, hr = %!HRESULT!", m_SensorName, hr);
        }
    }
    else
    {
        hr = E_POINTER;
        Trace(TRACE_LEVEL_ERROR, "m_pSensorManager is null, hr = %!HRESULT!", hr);
    }

    if (SUCCEEDED(hr))
    {
        m_fInitialPropertiesReceived = TRUE;
    }

    return hr;
 }


HRESULT CSensor::HandleGetCommonFeatureValues(
                                _In_ ULONG idx,
                                _Inout_ BOOL* pfReportingStateSupported,
                                _Inout_ ULONG* pulReportingState,
                                _Inout_ BOOL* pfPowerStateSupported,
                                _Inout_ ULONG* pulPowerState,
                                _Inout_ BOOL* pfSensorStatusSupported,
                                _Inout_ ULONG* pulSensorStatus,
                                _Inout_ BOOL* pfConnectionTypeSupported,
                                _Inout_ ULONG* pulConnectionType,
                                _Inout_ BOOL* pfReportIntervalSupported,
                                _Inout_ ULONG* pulReportInterval,
                                _Inout_ BOOL* pfGlobalSensitivitySupported,
                                _Inout_ FLOAT* pfltGlobalSensitivity,
                                _Inout_ BOOL* pfGlobalMaximumSupported,
                                _Inout_ FLOAT* pfltGlobalMaximum,
                                _Inout_ BOOL* pfGlobalMinimumSupported,
                                _Inout_ FLOAT* pfltGlobalMinimum,
                                _Inout_ BOOL* pfGlobalAccuracySupported,
                                _Inout_ FLOAT* pfltGlobalAccuracy,
                                _Inout_ BOOL* pfGlobalResolutionSupported,
                                _Inout_ FLOAT* pfltGlobalResolution,
                                _Inout_ BOOL* pfMinimumReportIntervalSupported,
                                _Inout_ ULONG* pulMinimumReportInterval,
                                _Inout_ BOOL* pfFriendlyNameSupported,
                                _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszFriendlyName,
                                _Inout_ BOOL* pfPersistentUniqueIDSupported,
                                _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszPersistentUniqueID,
                                _Inout_ BOOL* pfManufacturerSupported,
                                _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszManufacturer,
                                _Inout_ BOOL* pfModelSupported,
                                _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszModel,
                                _Inout_ BOOL* pfSerialNumberSupported,
                                _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszSerialNumber,
                                _Inout_ BOOL* pfDescriptionSupported,
                                _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH) PWCHAR pwszDescription,
                                _In_ USAGE Usage,
                                _In_ LONG UsageValue,
                                _In_ ULONG UsageUValue,
                                _In_ LONG UnitsExp,
                                _In_reads_bytes_(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2) CHAR*  pUsageArray,
                                _Out_ BOOL* pfFeatureHandled)
 {
    HRESULT hr = S_OK;

    if ((nullptr == pfReportingStateSupported)
        || (nullptr == pulReportingState)
        || (nullptr == pfPowerStateSupported)
        || (nullptr == pulPowerState)
        || (nullptr == pfSensorStatusSupported)
        || (nullptr == pulSensorStatus)
        || (nullptr == pfConnectionTypeSupported)
        || (nullptr == pulConnectionType)
        || (nullptr == pfReportIntervalSupported)
        || (nullptr == pulReportInterval)
        || (nullptr == pfGlobalSensitivitySupported)
        || (nullptr == pfltGlobalSensitivity)
        || (nullptr == pfGlobalMaximumSupported)
        || (nullptr == pfltGlobalMaximum)
        || (nullptr == pfGlobalMinimumSupported)
        || (nullptr == pfltGlobalMinimum)
        || (nullptr == pfGlobalAccuracySupported)
        || (nullptr == pfltGlobalAccuracy)
        || (nullptr == pfGlobalResolutionSupported)
        || (nullptr == pfltGlobalResolution)
        || (nullptr == pfMinimumReportIntervalSupported)
        || (nullptr == pulMinimumReportInterval)
        || (nullptr == pfFriendlyNameSupported)
        || (nullptr == pwszFriendlyName)
        || (nullptr == pfPersistentUniqueIDSupported)
        || (nullptr == pwszPersistentUniqueID)
        || (nullptr == pfManufacturerSupported)
        || (nullptr == pwszManufacturer)
        || (nullptr == pfModelSupported)
        || (nullptr == pwszModel)
        || (nullptr == pfSerialNumberSupported)
        || (nullptr == pwszSerialNumber)
        || (nullptr == pfDescriptionSupported)
        || (nullptr == pwszDescription)
        || (nullptr == pUsageArray)
        || (nullptr == pfFeatureHandled)
        )
    {
        hr = E_POINTER;
        return hr;
    }

    size_t strLen = 0;

    *pfFeatureHandled = TRUE;

    switch(Usage)
    {

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE:
        *pfReportingStateSupported = TRUE;
        *pulReportingState = UsageUValue;
        if (FALSE == m_fWarnedOnUseOfFeatureReportingState)
        {
            Trace(TRACE_LEVEL_WARNING, "%s is using deprecated form of Reporting State instead of NAry/Selectors in the Feature Report Descriptor", m_SensorName);
            m_fWarnedOnUseOfFeatureReportingState = TRUE;
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE:
        *pfPowerStateSupported = TRUE;
        *pulPowerState = UsageUValue;
        if (FALSE == m_fWarnedOnUseOfFeaturePowerState)
        {
            Trace(TRACE_LEVEL_WARNING, "%s is using deprecated form of Power State instead of NAry/Selectors in the Feature Report Descriptor", m_SensorName);
            m_fWarnedOnUseOfFeaturePowerState = TRUE;
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_STATUS:
        *pfSensorStatusSupported = TRUE;
        *pulSensorStatus = UsageUValue;
        if (FALSE == m_fInitialPropertiesReceived)
        {
            if (*pulSensorStatus >= HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_ERROR)
            {
                hr = m_pSensorManager->SetState(this, SENSOR_STATE_ERROR);
            }
            else
            {
                switch (*pulSensorStatus)
                {
                case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_READY:
                    hr = m_pSensorManager->SetState(this, SENSOR_STATE_READY);
                    break;
                case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_NO_DATA:
                    hr = m_pSensorManager->SetState(this, SENSOR_STATE_NO_DATA);
                    break;
                case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_INITIALIZING:
                    hr = m_pSensorManager->SetState(this, SENSOR_STATE_INITIALIZING);
                    break;
                case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_ACCESS_DENIED:
                    hr = m_pSensorManager->SetState(this, SENSOR_STATE_ACCESS_DENIED);
                    break;

                case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_UNKNOWN:
                case HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_NOT_AVAILABLE:
                default:
                    hr = m_pSensorManager->SetState(this, SENSOR_STATE_NOT_AVAILABLE);
                    break;
                }
            }
        }
        if (FALSE == m_fWarnedOnUseOfFeatureSensorState)
        {
            Trace(TRACE_LEVEL_WARNING, "%s is using deprecated form of Sensor State (Sensor Status) instead of NAry/Selectors in the Feature Report Descriptor", m_SensorName);
            m_fWarnedOnUseOfFeatureSensorState = TRUE;
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORT_INTERVAL:
        *pfReportIntervalSupported = TRUE;
        *pulReportInterval = (ULONG)ExtractLongLongFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
        break;

    // Extended properties

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_CONNECTION_TYPE:
        *pfConnectionTypeSupported = TRUE;
        *pulConnectionType = UsageUValue;
        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_CONNECTION_TYPE, *pulConnectionType);
        if (FALSE == m_fWarnedOnUseOfFeatureConnectionType)
        {
            Trace(TRACE_LEVEL_WARNING, "%s is using deprecated form of ConnectionType instead of NAry/Selectors in the Feature Report Descriptor", m_SensorName);
            m_fWarnedOnUseOfFeatureConnectionType = TRUE;
        }
        if (FALSE == m_fWarnedOnTypeOfFeatureConnectionType)
        {
            Trace(TRACE_LEVEL_WARNING, "%s ConnectionType is not PC_INTEGRATED - be sure this is correct", m_SensorName);
            m_fWarnedOnTypeOfFeatureConnectionType = TRUE;
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_MINIMUM_REPORT_INTERVAL:
        *pfMinimumReportIntervalSupported = TRUE;
        *pulMinimumReportInterval = (ULONG)ExtractLongLongFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);

        m_ulDefaultMinimumReportInterval = *pulMinimumReportInterval;

        hr = m_spSensorPropertyValues->SetUnsignedIntegerValue(SENSOR_PROPERTY_MIN_REPORT_INTERVAL, *pulMinimumReportInterval);
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_FRIENDLY_NAME:
        *pfFriendlyNameSupported = TRUE;
        pUsageArray[(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2)-1] = '\0';
        strLen = wcslen((WCHAR*)pUsageArray);
        if (strLen+1 < HID_FEATURE_REPORT_STRING_MAX_LENGTH)
        {
            hr = StringCchCopyW(pwszFriendlyName, strLen+1, (WCHAR*)pUsageArray);

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_FRIENDLY_NAME, pwszFriendlyName);
            }
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_PERSISTENT_UNIQUE_ID:
        *pfPersistentUniqueIDSupported = TRUE;
        pUsageArray[(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2)-1] = '\0';
        strLen = wcslen((WCHAR*)pUsageArray);
        if (strLen+1 < HID_FEATURE_REPORT_STRING_MAX_LENGTH)
        {
            hr = StringCchCopyW(pwszPersistentUniqueID, strLen+1, (WCHAR*)pUsageArray);

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_PERSISTENT_UNIQUE_ID, pwszPersistentUniqueID);
            }
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_MANUFACTURER:
        *pfManufacturerSupported = TRUE;
        pUsageArray[(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2)-1] = '\0';
        strLen = wcslen((WCHAR*)pUsageArray);
        if (strLen+1 < HID_FEATURE_REPORT_STRING_MAX_LENGTH)
        {
            hr = StringCchCopyW(pwszManufacturer, strLen+1, (WCHAR*)pUsageArray);

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_MANUFACTURER, pwszManufacturer);
            }
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_MODEL:
        *pfModelSupported = TRUE;
        pUsageArray[(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2)-1] = '\0';
        strLen = wcslen((WCHAR*)pUsageArray);
        if (strLen+1 < HID_FEATURE_REPORT_STRING_MAX_LENGTH)
        {
            hr = StringCchCopyW(pwszModel, strLen+1, (WCHAR*)pUsageArray);

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_MODEL, pwszModel);
            }
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_SERIAL_NUMBER:
        *pfSerialNumberSupported = TRUE;
        pUsageArray[(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2)-1] = '\0';
        strLen = wcslen((WCHAR*)pUsageArray);
        if (strLen+1 < HID_FEATURE_REPORT_STRING_MAX_LENGTH)
        {
            hr = StringCchCopyW(pwszSerialNumber, strLen+1, (WCHAR*)pUsageArray);

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_SERIAL_NUMBER, pwszSerialNumber);
            }
        }
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_DESCRIPTION:
        *pfDescriptionSupported = TRUE;
        pUsageArray[(HID_FEATURE_REPORT_STRING_MAX_LENGTH*2)-1] = '\0';
        strLen = wcslen((WCHAR*)pUsageArray);
        if (strLen+1 < HID_FEATURE_REPORT_STRING_MAX_LENGTH)
        {
            hr = StringCchCopyW(pwszDescription, strLen+1, (WCHAR*)pUsageArray);

            if (SUCCEEDED(hr))
            {
                hr = m_spSensorPropertyValues->SetStringValue(SENSOR_PROPERTY_DESCRIPTION, pwszDescription);
            }
        }
        break;

    //datafield-related properties

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_ABS:
        *pfGlobalSensitivitySupported = TRUE;
        *pfltGlobalSensitivity = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_ACCURACY:
        *pfGlobalAccuracySupported = TRUE;
        *pfltGlobalAccuracy = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_RESOLUTION:
        *pfGlobalResolutionSupported = TRUE;
        *pfltGlobalResolution = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_RANGE_MAXIMUM:
        *pfGlobalMaximumSupported = TRUE;
        *pfltGlobalMaximum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
        break;

    case HID_DRIVER_USAGE_SENSOR_PROPERTY_RANGE_MINIMUM:
        *pfGlobalMinimumSupported = TRUE;
        *pfltGlobalMinimum = (FLOAT)ExtractDoubleFromUsageValue(m_FeatureValueCapsNodes[idx].LogicalMin, UsageUValue, UsageValue, UnitsExp);
        break;

    default:
        *pfFeatureHandled = FALSE;
        break;

    }

    if (SUCCEEDED(hr))
    {
        m_fInitialPropertiesReceived = TRUE;
    }

    return hr;
}


 HRESULT CSensor::HandleSetReportingAndPowerStates(
                                        _In_ BOOL fReportingStateSupported,
                                        _In_ BOOL fReportingStateSelectorSupported,
                                        _In_ BOOL fReportingState,
                                        _In_ BOOL fPowerStateSupported,
                                        _In_ BOOL fPowerStateSelectorSupported,
                                        _In_ ULONG ulPowerState,
                                        _In_ HIDP_REPORT_TYPE ReportType, 
                                        _In_ USAGE UsagePage, 
                                        _In_ USHORT LinkCollection, 
                                        _In_reads_bytes_(MAX_NUM_HID_USAGES*2) USAGE* pUsageList, 
                                        _In_ ULONG* pNumUsages, 
                                        _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
                                        _In_ ULONG uReportSize)
 {
    HRESULT hr = S_OK;
    USAGE Usage = 0;
    USHORT UsageDataModifier = 0;
    ULONG UsageUValue = 0;

    ULONG ulRequestedReportingState = 0, ulRequestedPowerState = 0;
    ULONG ulCurrentReportingState = 0, ulCurrentPowerState = 0;
    ULONG ulRequestedReportingStateEnum = 0, ulRequestedPowerStateEnum = 0;
    ULONG ulCurrentReportingStateEnum = 0, ulCurrentPowerStateEnum = 0;

    if (SUCCEEDED(hr) && (TRUE == fReportingStateSupported))
    {
        Usage = HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE;
        UsageDataModifier = HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE;

        if (TRUE == fReportingState)
        {
            UsageUValue = TRUE; //reporting on
            Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Reporting State = ALL_EVENTS", m_SensorName);
        }
        else
        {
            UsageUValue = FALSE; //reporting off
            Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Reporting State = NO_EVENTS", m_SensorName);
        }

        if (uReportSize <= READ_BUFFER_SIZE)
        {
            hr = HidP_SetUsageValue(ReportType, UsagePage, LinkCollection, Usage, UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

            if (FAILED(hr))
            {
                TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);
                Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValues = reportingState in %s feature report, hr = %!HRESULT!", m_SensorName, hr);
            }
            else
            {
                ulRequestedReportingState = UsageUValue;
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Feature report for %s is too large, hr = %!HRESULT!", m_SensorName, hr);
        }
    }

    if (SUCCEEDED(hr) && (TRUE == fPowerStateSupported))
    {
        Usage = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE;
        UsageDataModifier = HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE;

        if (SENSOR_POWER_STATE_FULL_POWER == ulPowerState)
        {
            UsageUValue = HID_USAGE_SENSOR_PROPERTY_DEPRECATED_POWER_STATE_D0_FULL_POWER_ENUM;
            Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Power State = FULL_POWER", m_SensorName);
        }
        else if (SENSOR_POWER_STATE_LOW_POWER == ulPowerState)
        {
            UsageUValue = HID_USAGE_SENSOR_PROPERTY_DEPRECATED_POWER_STATE_D1_LOW_POWER_ENUM;
            Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Power State = LOW_POWER", m_SensorName);
        }
        else if (SENSOR_POWER_STATE_POWER_OFF == ulPowerState)
        {
            UsageUValue = HID_USAGE_SENSOR_PROPERTY_DEPRECATED_POWER_STATE_D4_POWER_OFF_ENUM;
            Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Power State = POWER_OFF", m_SensorName);
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Unrecognized %s power state, hr = %!HRESULT!", m_SensorName, hr);
        }

        if (uReportSize <= READ_BUFFER_SIZE)
        {
            hr = HidP_SetUsageValue(ReportType, UsagePage, LinkCollection, Usage, UsageUValue, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

            if (FAILED(hr))
            {
                TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);
                Trace(TRACE_LEVEL_ERROR, "Failed to SetUsageValues = powerState in %s feature report, hr = %!HRESULT!", m_SensorName, hr);
            }
            else
            {
                ulRequestedPowerState = UsageUValue;
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Feature report for %s is too large, hr = %!HRESULT!", m_SensorName, hr);
        }
    }

    if (SUCCEEDED(hr) && (TRUE == fReportingStateSelectorSupported))
    {
        if (nullptr != m_pSensorManager)
        {
            if (nullptr != m_pSensorManager->m_pLinkCollectionNodes)
            {
                if (m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren > 0)
                {
                    if ((uReportSize < READ_BUFFER_SIZE) && (*pNumUsages <= MAX_NUM_HID_USAGES))
                    {
                        if (SUCCEEDED(hr))
                        {
                            for (USHORT usIdx = 1; usIdx <= m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren; usIdx++)
                            {
                                ULONG ulNumUsages = *pNumUsages;

                                hr = HidP_GetUsages(ReportType, UsagePage, LinkCollection+usIdx, pUsageList, &ulNumUsages, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

                                if (SUCCEEDED(hr))
                                {
                                    for (USHORT usUdx = 0; usUdx < ulNumUsages; usUdx++)
                                    {
                                        USAGE tempUsage = pUsageList[usUdx];

                                        switch(pUsageList[usUdx])
                                        {
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_SEL:
                                            hr = HidP_UnsetUsages(ReportType, UsagePage, LinkCollection+usIdx, pUsageList, pNumUsages, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                                            if (SUCCEEDED(hr))
                                            {
                                                if ((TRUE == fReportingState) && (ulNumUsages < MAX_NUM_HID_USAGES))
                                                {
                                                    pUsageList[usUdx] = HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_SEL;  //reporting on
                                                    Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Reporting State = ALL_EVENTS", m_SensorName);
                                                }
                                                else if ((FALSE == fReportingState) && (ulNumUsages < MAX_NUM_HID_USAGES))
                                                {
                                                    pUsageList[usUdx] = HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_SEL;  //reporting off
                                                    Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Reporting State = NO_EVENTS", m_SensorName);
                                                }
                                                else
                                                {
                                                    hr = E_UNEXPECTED;
                                                    Trace(TRACE_LEVEL_ERROR, "Too many elements in %s UsageList, hr = %!HRESULT!", m_SensorName, hr);
                                                }

                                                hr = HidP_SetUsages(ReportType, UsagePage, LinkCollection+usIdx, pUsageList, &ulNumUsages, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

                                                if (FAILED(hr))
                                                {
                                                    TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);
                                                    Trace(TRACE_LEVEL_ERROR, "Failed to SetUsages = reportingState in %s feature report, hr = %!HRESULT!", m_SensorName, hr);
                                                }
                                                else
                                                {
                                                    switch (pUsageList[usUdx])
                                                    {
                                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_SEL:
                                                            ulRequestedReportingStateEnum = HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_ENUM;
                                                            break;

                                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_SEL:
                                                            ulRequestedReportingStateEnum = HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_ENUM;
                                                            break;

                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);
                                                Trace(TRACE_LEVEL_ERROR, "Failed to UnsetUsages = reportingState in %s feature report, hr = %!HRESULT!", m_SensorName, hr);
                                            }
                                            break;

                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_INTEGRATED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_ATTACHED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_EXTERNAL_SEL:
                                            //used in feature reports, so no action
                                            break;

                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_WAKE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_WAKE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_WAKE_SEL:
                                            Trace(TRACE_LEVEL_ERROR, "Unsupported %s reporting state selector, hr = %!HRESULT!, Usage = %i", m_SensorName, hr, tempUsage);
                                            break;

                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_UNDEFINED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D2_STANDBY_WITH_WAKE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D3_SLEEP_WITH_WAKE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_SEL:
                                            //used in feature reports, so no action
                                            break;

                                        case HID_DRIVER_USAGE_SENSOR_STATE_UNKNOWN_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_READY_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_NOT_AVAILABLE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_NO_DATA_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_INITIALIZING_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_ACCESS_DENIED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_ERROR_SEL:
                                            //used in input and feature reports, so no action
                                            break;

                                        case HID_DRIVER_USAGE_SENSOR_EVENT_UNKNOWN_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_EVENT_STATE_CHANGED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_EVENT_PROPERTY_CHANGED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_EVENT_DATA_UPDATED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_EVENT_POLL_RESPONSE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_EVENT_CHANGE_SENSITIVITY_SEL:
                                            //used in input reports, so no action
                                            break;

                                        default:
                                            Trace(TRACE_LEVEL_ERROR, "Unrecognized %s reporting state selector, hr = %!HRESULT!, Usage = %i", m_SensorName, hr, tempUsage);
                                            break;
                                        }
                                    }
                                }
                                else if (HIDP_STATUS_USAGE_NOT_FOUND == (INT)hr)
                                {
                                    hr = S_OK;
                                }
                                else
                                {
                                    TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);

                                    hr = E_UNEXPECTED;
                                    Trace(TRACE_LEVEL_ERROR, "Failure to get %s button caps type: feature, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                        }
                    }
                    else
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Feature buffer for %s too large, hr = %!HRESULT!", m_SensorName, hr);
                    }
                }
            }
            else
            {
                hr = E_POINTER;
                Trace(TRACE_LEVEL_ERROR, "%s m_pLinkCollectionNodes is null, hr = %!HRESULT!", m_SensorName, hr);
            }
        }
        else
        {
            hr = E_POINTER;
            Trace(TRACE_LEVEL_ERROR, "m_pSensorManager is null, hr = %!HRESULT!", hr);
        }
    }

    if (SUCCEEDED(hr) && (TRUE == fPowerStateSelectorSupported))
    {
        if (nullptr != m_pSensorManager) 
        {
            if (nullptr != m_pSensorManager->m_pLinkCollectionNodes)
            {
                if (m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren > 0)
                {
                    if ((uReportSize < READ_BUFFER_SIZE) && (*pNumUsages <= MAX_NUM_HID_USAGES))
                    {
                        if (SUCCEEDED(hr))
                        {
                            for (USHORT usIdx = 1; usIdx <= m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren; usIdx++)
                            {
                                ULONG ulNumUsages = *pNumUsages;

                                hr = HidP_GetUsages(ReportType, UsagePage, LinkCollection+usIdx, pUsageList, &ulNumUsages, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

                                if (SUCCEEDED(hr))
                                {
                                    for (USHORT usUdx = 0; usUdx < ulNumUsages; usUdx++)
                                    {
                                        USAGE tempUsage = pUsageList[usUdx];

                                        switch(pUsageList[usUdx])
                                        {
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_SEL:
                                            hr = HidP_UnsetUsages(ReportType, UsagePage, LinkCollection+usIdx, pUsageList, pNumUsages, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);
                                            if (SUCCEEDED(hr))
                                            {
                                                if ((SENSOR_POWER_STATE_FULL_POWER == ulPowerState) && (ulNumUsages < MAX_NUM_HID_USAGES))
                                                {
                                                    pUsageList[usUdx] = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_SEL;
                                                    Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Power State = FULL_POWER", m_SensorName);
                                                }
                                                else if ((SENSOR_POWER_STATE_LOW_POWER == ulPowerState) && (ulNumUsages < MAX_NUM_HID_USAGES))
                                                {
                                                    pUsageList[usUdx] = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_SEL;
                                                    Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Power State = LOW_POWER", m_SensorName);
                                                }
                                                else if ((SENSOR_POWER_STATE_POWER_OFF == ulPowerState) && (ulNumUsages < MAX_NUM_HID_USAGES))
                                                {
                                                    pUsageList[usUdx] = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_SEL;
                                                    Trace(TRACE_LEVEL_INFORMATION, "Requesting %s Power State = POWER_OFF", m_SensorName);
                                                }
                                                else
                                                {
                                                    hr = E_UNEXPECTED;
                                                    Trace(TRACE_LEVEL_ERROR, "Too many elements in %s UsageList, hr = %!HRESULT!", m_SensorName, hr);
                                                }

                                                hr = HidP_SetUsages(ReportType, UsagePage, LinkCollection+usIdx, pUsageList, &ulNumUsages, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

                                                if (FAILED(hr))
                                                {
                                                    TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);
                                                    Trace(TRACE_LEVEL_ERROR, "Failed to SetUsages = reportingState in %s feature report, hr = %!HRESULT!", m_SensorName, hr);
                                                }
                                                else
                                                {
                                                    switch (pUsageList[usUdx])
                                                    {
                                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_SEL:
                                                            ulRequestedPowerStateEnum = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_ENUM;
                                                            break;

                                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_SEL:
                                                            ulRequestedPowerStateEnum = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_ENUM;
                                                            break;

                                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_SEL:
                                                            ulRequestedPowerStateEnum = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_ENUM;
                                                            break;

                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);
                                                Trace(TRACE_LEVEL_ERROR, "Failed to UnsetUsages = reportingState in %s feature report, hr = %!HRESULT!", m_SensorName, hr);
                                            }
                                            break;

                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_INTEGRATED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_ATTACHED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_EXTERNAL_SEL:
                                            //used in feature reports, so no action
                                            break;

                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_WAKE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_WAKE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_WAKE_SEL:
                                            //used in feature reports, so no action
                                            break;

                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_UNDEFINED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D2_STANDBY_WITH_WAKE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D3_SLEEP_WITH_WAKE_SEL:
                                            Trace(TRACE_LEVEL_ERROR, "Unsupported %s power state selector, hr = %!HRESULT!, Usage = %i", m_SensorName, hr, tempUsage);
                                            break;

                                        case HID_DRIVER_USAGE_SENSOR_STATE_UNKNOWN_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_READY_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_NOT_AVAILABLE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_NO_DATA_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_INITIALIZING_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_ACCESS_DENIED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_STATE_ERROR_SEL:
                                            //used in input and feature reports, so no action
                                            break;

                                        case HID_DRIVER_USAGE_SENSOR_EVENT_UNKNOWN_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_EVENT_STATE_CHANGED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_EVENT_PROPERTY_CHANGED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_EVENT_DATA_UPDATED_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_EVENT_POLL_RESPONSE_SEL:
                                        case HID_DRIVER_USAGE_SENSOR_EVENT_CHANGE_SENSITIVITY_SEL:
                                            //used in input reports, so no action
                                            break;

                                        default:
                                            Trace(TRACE_LEVEL_ERROR, "Unrecognized %s power state selector, hr = %!HRESULT!, Usage = %i", m_SensorName, hr, tempUsage);
                                            break;
                                        }
                                    }
                                }
                                else if (HIDP_STATUS_USAGE_NOT_FOUND == (INT)hr)
                                {
                                    hr = S_OK;
                                }
                                else
                                {
                                    TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);

                                    hr = E_UNEXPECTED;
                                    Trace(TRACE_LEVEL_ERROR, "Failure to get %s button caps type: feature, hr = %!HRESULT!", m_SensorName, hr);
                                }
                            }
                        }
                    }
                    else
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Feature buffer for %s too large, hr = %!HRESULT!", m_SensorName, hr);
                    }
                }
            }
            else
            {
                hr = E_POINTER;
                Trace(TRACE_LEVEL_ERROR, "%s m_pLinkCollectionNodes is null, hr = %!HRESULT!", m_SensorName, hr);
            }
        }
        else
        {
            hr = E_POINTER;
            Trace(TRACE_LEVEL_ERROR, "m_pSensorManager is null, hr = %!HRESULT!", hr);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Write the report interval value to the device and then read back what the device has set the report interval to before
        // setting this value for the DeviceProperties and at the API

        const BOOL fExactCopyRequired = TRUE;
        BOOL fIsExactCopy = FALSE;

        hr = SetThenGetSensorPropertiesFromDevice(pFeatureReport, uReportSize, fExactCopyRequired, &fIsExactCopy);

        if (SUCCEEDED(hr))
        {
            if (FALSE == fIsExactCopy)
            {
                Trace(TRACE_LEVEL_ERROR, "%s Feature Report returned is not exact copy, hr = %!HRESULT!", m_SensorName, hr);
            }

            //handle the case where named array selectors are used
            if (nullptr != m_pSensorManager)
            {
                if (nullptr != m_pSensorManager->m_pLinkCollectionNodes)
                {
                    //if (m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren > 0)
                    if (m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren > 0)
                    {
                        if ((uReportSize < READ_BUFFER_SIZE) && (*pNumUsages <= MAX_NUM_HID_USAGES))
                        {
                            if (SUCCEEDED(hr))
                            {
                                for (USHORT usIdx = 1; usIdx <= m_pSensorManager->m_pLinkCollectionNodes[LinkCollection].NumberOfChildren; usIdx++)
                                {
                                    ULONG ulNumUsages = *pNumUsages;

                                    hr = HidP_GetUsages(ReportType, UsagePage, LinkCollection+usIdx, pUsageList, &ulNumUsages, m_pSensorManager->m_pPreparsedData, (PCHAR)pFeatureReport, uReportSize);

                                    if (SUCCEEDED(hr))
                                    {
                                        for (USHORT usUdx = 0; usUdx < ulNumUsages; usUdx++)
                                        {
                                            USAGE tempUsage = pUsageList[usIdx];

                                            switch(pUsageList[usUdx])
                                            {
                                            case 0:
                                                //no entry
                                                break;

                                            //Common sensor properties
                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_SEL:
                                                ulCurrentReportingStateEnum = HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_ENUM;
                                                if (ulRequestedReportingStateEnum == ulCurrentReportingStateEnum)
                                                {
                                                    Trace(TRACE_LEVEL_INFORMATION, "%s Reporting State enum was set correctly to = NO_EVENTS", m_SensorName);
                                                }
                                                else
                                                {
                                                    Trace(TRACE_LEVEL_ERROR, "%s Reporting State enum is not correct = NO_EVENTS, hr = %!HRESULT!", m_SensorName, hr);
                                                }
                                                break;

                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_SEL:
                                                ulCurrentReportingStateEnum = HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_ENUM;
                                                if (ulRequestedReportingStateEnum == ulCurrentReportingStateEnum)
                                                {
                                                    Trace(TRACE_LEVEL_INFORMATION, "%s Reporting State enum was set correctly to = ALL_EVENTS", m_SensorName);
                                                }
                                                else
                                                {
                                                    Trace(TRACE_LEVEL_ERROR, "%s Reporting State enum is not correct = ALL_EVENTS, hr = %!HRESULT!", m_SensorName, hr);
                                                }
                                                break;


                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_SEL:
                                                ulCurrentPowerStateEnum = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_ENUM;
                                                if (ulRequestedPowerStateEnum == ulCurrentPowerStateEnum)
                                                {
                                                    Trace(TRACE_LEVEL_INFORMATION, "%s Power State enum was set correctly to = FULL_POWER", m_SensorName);
                                                }
                                                else
                                                {
                                                    Trace(TRACE_LEVEL_ERROR, "%s Power State enum is not correct = FULL_POWER, hr = %!HRESULT!", m_SensorName, hr);
                                                }
                                                break;

                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_SEL:
                                                ulCurrentPowerStateEnum = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_ENUM;
                                                if (ulRequestedPowerStateEnum == ulCurrentPowerStateEnum)
                                                {
                                                    Trace(TRACE_LEVEL_INFORMATION, "%s Power State enum was set correctly to = LOW_POWER", m_SensorName);
                                                }
                                                else
                                                {
                                                    Trace(TRACE_LEVEL_ERROR, "%s Power State enum is not correct = LOW_POWER, hr = %!HRESULT!", m_SensorName, hr);
                                                }
                                                break;

                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_SEL:
                                                ulCurrentPowerStateEnum = HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_ENUM;
                                                if (ulRequestedPowerStateEnum == ulCurrentPowerStateEnum)
                                                {
                                                    Trace(TRACE_LEVEL_INFORMATION, "%s Power State enum was set correctly to = POWER_OFF", m_SensorName);
                                                }
                                                else
                                                {
                                                    Trace(TRACE_LEVEL_ERROR, "%s Power State enum is not correct = POWER_OFF, hr = %!HRESULT!", m_SensorName, hr);
                                                }
                                                break;

                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_WAKE_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_WAKE_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_WAKE_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_UNDEFINED_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D2_STANDBY_WITH_WAKE_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D3_SLEEP_WITH_WAKE_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_STATE_UNKNOWN_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_STATE_READY_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_STATE_NOT_AVAILABLE_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_STATE_NO_DATA_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_STATE_INITIALIZING_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_STATE_ACCESS_DENIED_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_STATE_ERROR_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_INTEGRATED_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_ATTACHED_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_EXTERNAL_SEL:
                                                //used in feature reports - no action
                                                break;

                                            case HID_DRIVER_USAGE_SENSOR_EVENT_UNKNOWN_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_EVENT_STATE_CHANGED_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_EVENT_PROPERTY_CHANGED_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_EVENT_DATA_UPDATED_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_EVENT_POLL_RESPONSE_SEL:
                                            case HID_DRIVER_USAGE_SENSOR_EVENT_CHANGE_SENSITIVITY_SEL:
                                                //used in input reports - no action
                                                break;

                                            default:
                                                Trace(TRACE_LEVEL_ERROR, "Unrecognized feature %s selector, hr = %!HRESULT!, Usage = %i", m_SensorName, hr, tempUsage);
                                                break;
                                            }
                                        }
                                    }
                                    else if (HIDP_STATUS_USAGE_NOT_FOUND == (INT)hr)
                                    {
                                        hr = S_OK;
                                    }
                                    else
                                    {
                                        TraceHIDParseError(hr, m_SensorType, ReportType, LinkCollection);

                                        hr = E_UNEXPECTED;
                                        Trace(TRACE_LEVEL_ERROR, "Failure to get %s button caps type: feature, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                }
                            }
                        }
                        else
                        {
                            hr = E_UNEXPECTED;
                            Trace(TRACE_LEVEL_ERROR, "Feature buffer for %s too large, hr = %!HRESULT!", m_SensorName, hr);
                        }
                    }
                }
                else
                {
                    hr = E_POINTER;
                    Trace(TRACE_LEVEL_ERROR, "%s m_pLinkCollectionNodes is null, hr = %!HRESULT!", m_SensorName, hr);
                }
            }
            else
            {
                hr = E_POINTER;
                Trace(TRACE_LEVEL_ERROR, "m_pSensorManager is null, hr = %!HRESULT!", hr);
            }
        }
        else
        {
            Trace(TRACE_LEVEL_ERROR, "Failed to set then get %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
        }

        //handle the case where values are used instead of selectors
        if (SUCCEEDED(hr))
        {
            UCHAR reportID = 0;
            LONG  UsageValue = 0;
            CHAR  UsageArray[HID_FEATURE_REPORT_STRING_MAX_LENGTH*2] = {0};

            if (nullptr != m_pSensorManager)
            {
                if (m_pSensorManager->m_NumMappedSensors > 1)
                {
                    reportID = (UCHAR)(m_StartingFeatureReportID + m_SensorNum); 
                }
            }
            else
            {
                hr = E_POINTER;
                Trace(TRACE_LEVEL_ERROR, "m_pSensorManager is null, hr = %!HRESULT!", hr);
            }

            if (SUCCEEDED(hr))
            {
                USHORT ReportCount = 0;

                USHORT numNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;

                LONG   UnitsExp = 0;
                ULONG  BitSize = 0;

                for(ULONG idx = 0; idx < numNodes; idx++)
                {
                    if (reportID == m_FeatureValueCapsNodes[idx].ReportID)
                    {
                        UsagePage = m_FeatureValueCapsNodes[idx].UsagePage;
                        Usage = m_FeatureValueCapsNodes[idx].NotRange.Usage;
                        ReportCount = m_FeatureValueCapsNodes[idx].ReportCount;
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
                            Usage = Usage & 0x0FFF; //remove the data modifier
                            UsageValue = ExtractValueFromUsageUValue(m_FeatureValueCapsNodes[idx].LogicalMin, BitSize, UsageUValue);

                            switch(Usage)
                            {

                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE:
                                ulCurrentReportingState = UsageValue;
                                if (ulRequestedReportingState == ulCurrentReportingState)
                                {
                                    if (FALSE == ulCurrentReportingState)
                                    {
                                        Trace(TRACE_LEVEL_INFORMATION, "%s Reporting State value was set correctly to = NO_EVENTS", m_SensorName);
                                    }
                                    else
                                    {
                                        Trace(TRACE_LEVEL_INFORMATION, "%s Reporting State value was set correctly to = ALL_EVENTS", m_SensorName);
                                    }
                                }
                                else
                                {
                                    if (FALSE == ulCurrentReportingState)
                                    {
                                        Trace(TRACE_LEVEL_ERROR, "%s Power State value is not correct = NO_EVENTS, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                    else
                                    {
                                        Trace(TRACE_LEVEL_ERROR, "%s Power State value is not correct = ALL_EVENTS, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                }
                                break;

                            case HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE:
                                ulCurrentPowerState = UsageValue;
                                if (ulRequestedPowerState == ulCurrentPowerState)
                                {
                                    if (SENSOR_POWER_STATE_POWER_OFF == ulCurrentPowerState)
                                    {
                                        Trace(TRACE_LEVEL_INFORMATION, "%s Power State value was set correctly to = POWER_OFF", m_SensorName);
                                    }
                                    else if (SENSOR_POWER_STATE_LOW_POWER == ulCurrentPowerState)
                                    {
                                        Trace(TRACE_LEVEL_INFORMATION, "%s Power State value was set correctly to = LOW_POWER", m_SensorName);
                                    }
                                    else if (SENSOR_POWER_STATE_FULL_POWER == ulCurrentPowerState)
                                    {
                                        Trace(TRACE_LEVEL_INFORMATION, "%s Power State value was set correctly to = FULL_POWER", m_SensorName);
                                    }
                                    else
                                    {
                                        Trace(TRACE_LEVEL_INFORMATION, "%s Power State value was set correctly to = UNKNOWN", m_SensorName);
                                    }
                                }
                                else
                                {
                                    if (SENSOR_POWER_STATE_POWER_OFF == ulCurrentPowerState)
                                    {
                                        Trace(TRACE_LEVEL_ERROR, "%s Power State value is not correct = POWER_OFF, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                    else if (SENSOR_POWER_STATE_LOW_POWER == ulCurrentPowerState)
                                    {
                                        Trace(TRACE_LEVEL_ERROR, "%s Power State value is not correct = LOW_POWER, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                    else if (SENSOR_POWER_STATE_LOW_POWER == ulCurrentPowerState)
                                    {
                                        Trace(TRACE_LEVEL_ERROR, "%s Power State value is not correct = FULL_POWER, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                    else
                                    {
                                        Trace(TRACE_LEVEL_ERROR, "%s Power State value is not correct = UNKNOWN, hr = %!HRESULT!", m_SensorName, hr);
                                    }
                                }
                                break;

                            default:
                                break;
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
    }

    if (SUCCEEDED(hr))
    {
        hr = S_OK;
        m_fSensorPropertiesPreviouslyUpdated = TRUE;
    }

    return hr;
 }


HRESULT CSensor::GetSensorPropertiesFromDevice(
        _In_ UCHAR reportID, 
        _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
        _In_ UINT uReportSize)
{
    HRESULT hr = E_UNEXPECTED;

    if (nullptr != m_pSensorManager)
    {
        if (nullptr != m_pSensorManager->m_pSensorDDI)
        {
            hr = m_pSensorManager->m_pSensorDDI->GetFeatureReport(reportID, pFeatureReport, uReportSize, m_SensorName);
        }
        else
        {
            hr = E_POINTER;
            Trace(TRACE_LEVEL_ERROR, "m_pSensorDDI is NULL, hr = %!HRESULT!", hr);
        }
    }
    else
    {
        hr = E_POINTER;
        Trace(TRACE_LEVEL_ERROR, "m_pSensorManager is NULL, hr = %!HRESULT!", hr);
    }


    return hr;
}

HRESULT CSensor::SetThenGetSensorPropertiesFromDevice(
        _In_reads_bytes_(READ_BUFFER_SIZE) BYTE* pFeatureReport, 
        _In_ UINT uReportSize,
        _In_ BOOL fExactCopyRequired,
        _Out_ BOOL* pfIsExactCopy)
{
    HRESULT hr = S_OK;
    BOOL fHasContent = FALSE;
    BOOL fIsExactCopy = TRUE;
    const DWORD dwMaxTries = FEATURE_REPORT_MAX_RETRIES;
    DWORD dwRetryCount = 0;
    UCHAR reportID = 0;

    BYTE* pRecvReport = nullptr;
    BYTE* pFeatureReportCopy = nullptr;

    try 
    {
        pRecvReport = new BYTE[READ_BUFFER_SIZE];
    }
    catch(...)
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for pRecvReport, hr = %!HRESULT!", hr);

        if (nullptr != pRecvReport) 
        {
            delete[] pRecvReport;
        }
    }
    try 
    {
        pFeatureReportCopy = new BYTE[READ_BUFFER_SIZE];
    }
    catch(...)
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "Failed to allocate memory for pFeatureReportCopy, hr = %!HRESULT!", hr);

        if (nullptr != pFeatureReportCopy) 
        {
            delete[] pFeatureReportCopy;
        }
    }

    if (SUCCEEDED(hr))
    {
        if (nullptr != m_pSensorManager)
        {
            if (nullptr != m_pSensorManager->m_pSensorDDI)
            {
                if (nullptr != m_pSensorManager->m_pSensorDDI->m_pHidIoctlRequest)
                {
                    if (uReportSize <= MAX_REPORT_SIZE)
                    {
                        if (nullptr != pFeatureReport)
                        {
                            do
                            {
                                //calculate actual report size
                                USHORT numValueNodes = m_pSensorManager->m_HidCaps.NumberFeatureValueCaps;

                                ULONG  ulBitSize = 0;
                                ULONG  ulReportCount = 0;
                                ULONG  ulActualReportSize = 1; //account for the ReportID

                                for (ULONG idx = 0; idx < numValueNodes; idx++)
                                {
                                    if (pFeatureReport[0] == m_FeatureValueCapsNodes[idx].ReportID)
                                    {
                                        ulReportCount = m_FeatureValueCapsNodes[idx].ReportCount;
                                        ulBitSize = m_FeatureValueCapsNodes[idx].BitSize;

                                        ulActualReportSize += ulReportCount * (ulBitSize / 8);
                                    }
                                }

                                USHORT numButtonNodes = m_pSensorManager->m_HidCaps.NumberFeatureButtonCaps;

                                USAGE uCurrentLinkUsage = 0;
                                USAGE uPreviousLinkUsage = 0;

                                for (ULONG idx = 0; idx < numButtonNodes; idx++)
                                {
                                    if (pFeatureReport[0] == m_FeatureButtonCapsNodes[idx].ReportID)
                                    {
                                        uCurrentLinkUsage = m_FeatureButtonCapsNodes[idx].LinkUsage;

                                        if (uCurrentLinkUsage != uPreviousLinkUsage)
                                        {
                                            ulActualReportSize += 1; //an NAry/Selector
                                            uPreviousLinkUsage = uCurrentLinkUsage;
                                        }
                                    }
                                }

                                fHasContent = FALSE;
                                fIsExactCopy = TRUE;

                                reportID = pFeatureReport[0];

                                ZeroMemory(pRecvReport, uReportSize);
                                ZeroMemory(pFeatureReportCopy, uReportSize);

                                //copy the feature report into a new report
                                CopyMemory(pFeatureReportCopy, pFeatureReport, uReportSize);

                                // Set the feature report
                                Trace(TRACE_LEVEL_INFORMATION, "Setting %s Feature Report", m_SensorName);

                                hr = m_pSensorManager->m_pSensorDDI->m_pHidIoctlRequest->CreateAndSendIoctlRequest(
                                        (BYTE*)pRecvReport,
                                        uReportSize, 
                                        IOCTL_HID_SET_FEATURE, 
                                        (BYTE*)pFeatureReportCopy);

                                if (SUCCEEDED(hr))
                                {
                                    Trace(TRACE_LEVEL_INFORMATION, "Getting %s Feature Report", m_SensorName);

                                    hr = m_pSensorManager->m_pSensorDDI->m_pHidIoctlRequest->CreateAndSendIoctlRequest(
                                            (BYTE*)pFeatureReportCopy,
                                            uReportSize, 
                                            IOCTL_HID_GET_FEATURE,
                                            NULL);
                                }
                                else
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed while setting %s Feature Report, hr = %!HRESULT!", m_SensorName, hr);
                                }

                                if (SUCCEEDED(hr))
                                {
                                    for (DWORD idx = 0; idx < ulActualReportSize; idx++)
                                    {
                                        if (pFeatureReportCopy[idx] != 0)
                                        {
                                            fHasContent = TRUE;
                                        }

                                        UCHAR ucFeatureReportByte = pFeatureReport[idx];
                                        UCHAR ucFeatureReportCopyByte = pFeatureReportCopy[idx];

                                        if (ucFeatureReportCopyByte != ucFeatureReportByte)
                                        {
                                            fIsExactCopy = FALSE;
                                        }
                                    }

                                    if (FALSE == fExactCopyRequired)
                                    {
                                        fIsExactCopy = TRUE;
                                    }

                                    if (TRUE == fHasContent)
                                    {
                                        if (pFeatureReportCopy[0] != reportID)
                                        {
                                            hr = E_UNEXPECTED;
                                            Trace(TRACE_LEVEL_WARNING, "%s Set size = %02i, 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", m_SensorName, ulActualReportSize, pFeatureReport[0], pFeatureReport[1], pFeatureReport[2], pFeatureReport[3], 
                                                                                                                                    pFeatureReport[4], pFeatureReport[5], pFeatureReport[6], pFeatureReport[7], 
                                                                                                                                    pFeatureReport[8], pFeatureReport[9], pFeatureReport[10], pFeatureReport[11],
                                                                                                                                    pFeatureReport[12], pFeatureReport[13], pFeatureReport[14], pFeatureReport[15]);
                                            Trace(TRACE_LEVEL_WARNING, "%s Get size = %02i, 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", m_SensorName, ulActualReportSize, pFeatureReportCopy[0], pFeatureReportCopy[1], pFeatureReportCopy[2], pFeatureReportCopy[3], 
                                                                                                                                    pFeatureReportCopy[4], pFeatureReportCopy[5], pFeatureReportCopy[6], pFeatureReportCopy[7], 
                                                                                                                                    pFeatureReportCopy[8], pFeatureReportCopy[9], pFeatureReportCopy[10], pFeatureReportCopy[11],
                                                                                                                                    pFeatureReportCopy[12], pFeatureReportCopy[13], pFeatureReportCopy[14], pFeatureReportCopy[15]);
                                            Trace(TRACE_LEVEL_ERROR, "Feature Report read for %s Report ID does not match expected reportID so retrying, retry count = %i, hr = %!HRESULT!", m_SensorName, dwRetryCount, hr);
                                        }
                                    }
                                    else
                                    {
                                        Trace(TRACE_LEVEL_WARNING, "%s Set size = %02i, 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", m_SensorName, ulActualReportSize, pFeatureReport[0], pFeatureReport[1], pFeatureReport[2], pFeatureReport[3], 
                                                                                                                                pFeatureReport[4], pFeatureReport[5], pFeatureReport[6], pFeatureReport[7], 
                                                                                                                                pFeatureReport[8], pFeatureReport[9], pFeatureReport[10], pFeatureReport[11],
                                                                                                                                pFeatureReport[12], pFeatureReport[13], pFeatureReport[14], pFeatureReport[15]);
                                        Trace(TRACE_LEVEL_WARNING, "%s Get size = %02i, 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", m_SensorName, ulActualReportSize, pFeatureReportCopy[0], pFeatureReportCopy[1], pFeatureReportCopy[2], pFeatureReportCopy[3], 
                                                                                                                                pFeatureReportCopy[4], pFeatureReportCopy[5], pFeatureReportCopy[6], pFeatureReportCopy[7], 
                                                                                                                                pFeatureReportCopy[8], pFeatureReportCopy[9], pFeatureReportCopy[10], pFeatureReportCopy[11],
                                                                                                                                pFeatureReportCopy[12], pFeatureReportCopy[13], pFeatureReportCopy[14], pFeatureReportCopy[15]);
                                        Trace(TRACE_LEVEL_ERROR, "Feature Report read for %s is empty so retrying, retry count = %i, hr = %!HRESULT!", m_SensorName, dwRetryCount, hr);
                                    }
                                }
                                else
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Getting Feature Report read from %s using m_pHidIoctlRequest failed so retrying, retry count = %i, hr = %!HRESULT!", m_SensorName, dwRetryCount, hr);
                                }

                                if ((TRUE == fExactCopyRequired) && (FALSE == fIsExactCopy))
                                {
                                    Trace(TRACE_LEVEL_WARNING, "%s Set size = %02i, 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", m_SensorName, ulActualReportSize, pFeatureReport[0], pFeatureReport[1], pFeatureReport[2], pFeatureReport[3], 
                                                                                                                            pFeatureReport[4], pFeatureReport[5], pFeatureReport[6], pFeatureReport[7], 
                                                                                                                            pFeatureReport[8], pFeatureReport[9], pFeatureReport[10], pFeatureReport[11],
                                                                                                                            pFeatureReport[12], pFeatureReport[13], pFeatureReport[14], pFeatureReport[15]);
                                    Trace(TRACE_LEVEL_WARNING, "                    16..31: 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", pFeatureReport[16], pFeatureReport[17], pFeatureReport[18], pFeatureReport[19], 
                                                                                                                            pFeatureReport[20], pFeatureReport[21], pFeatureReport[22], pFeatureReport[23], 
                                                                                                                            pFeatureReport[24], pFeatureReport[25], pFeatureReport[26], pFeatureReport[27],
                                                                                                                            pFeatureReport[28], pFeatureReport[29], pFeatureReport[30], pFeatureReport[31]);
                                    Trace(TRACE_LEVEL_WARNING, "                    32..47: 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", pFeatureReport[32], pFeatureReport[33], pFeatureReport[34], pFeatureReport[35], 
                                                                                                                            pFeatureReport[36], pFeatureReport[37], pFeatureReport[38], pFeatureReport[39], 
                                                                                                                            pFeatureReport[40], pFeatureReport[41], pFeatureReport[42], pFeatureReport[43],
                                                                                                                            pFeatureReport[44], pFeatureReport[45], pFeatureReport[46], pFeatureReport[47]);
                                    Trace(TRACE_LEVEL_WARNING, "                    48..63: 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x]", pFeatureReport[48], pFeatureReport[49], pFeatureReport[50], pFeatureReport[51], 
                                                                                                                            pFeatureReport[52], pFeatureReport[53], pFeatureReport[54], pFeatureReport[55], 
                                                                                                                            pFeatureReport[56], pFeatureReport[57], pFeatureReport[58], pFeatureReport[59],
                                                                                                                            pFeatureReport[60], pFeatureReport[61], pFeatureReport[62], pFeatureReport[63]);

                                    Trace(TRACE_LEVEL_WARNING, "%s Get size = %02i, 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", m_SensorName, ulActualReportSize, pFeatureReportCopy[0], pFeatureReportCopy[1], pFeatureReportCopy[2], pFeatureReportCopy[3], 
                                                                                                                            pFeatureReportCopy[4], pFeatureReportCopy[5], pFeatureReportCopy[6], pFeatureReportCopy[7], 
                                                                                                                            pFeatureReportCopy[8], pFeatureReportCopy[9], pFeatureReportCopy[10], pFeatureReportCopy[11],
                                                                                                                            pFeatureReportCopy[12], pFeatureReportCopy[13], pFeatureReportCopy[14], pFeatureReportCopy[15]);
                                    Trace(TRACE_LEVEL_WARNING, "                    16..31: 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", pFeatureReportCopy[16], pFeatureReportCopy[17], pFeatureReportCopy[18], pFeatureReportCopy[19], 
                                                                                                                            pFeatureReportCopy[20], pFeatureReportCopy[21], pFeatureReportCopy[22], pFeatureReportCopy[23], 
                                                                                                                            pFeatureReportCopy[24], pFeatureReportCopy[25], pFeatureReportCopy[26], pFeatureReportCopy[27],
                                                                                                                            pFeatureReportCopy[28], pFeatureReportCopy[29], pFeatureReportCopy[30], pFeatureReportCopy[31]);
                                    Trace(TRACE_LEVEL_WARNING, "                    32..47: 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", pFeatureReportCopy[32], pFeatureReportCopy[33], pFeatureReportCopy[34], pFeatureReportCopy[35], 
                                                                                                                            pFeatureReportCopy[36], pFeatureReportCopy[37], pFeatureReportCopy[38], pFeatureReportCopy[39], 
                                                                                                                            pFeatureReportCopy[40], pFeatureReportCopy[41], pFeatureReportCopy[42], pFeatureReportCopy[43],
                                                                                                                            pFeatureReportCopy[44], pFeatureReportCopy[45], pFeatureReportCopy[46], pFeatureReportCopy[47]);
                                    Trace(TRACE_LEVEL_WARNING, "                    48..63: 0x[%02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x %02x ...]", pFeatureReportCopy[48], pFeatureReportCopy[49], pFeatureReportCopy[50], pFeatureReportCopy[51], 
                                                                                                                            pFeatureReportCopy[52], pFeatureReportCopy[53], pFeatureReportCopy[54], pFeatureReportCopy[55], 
                                                                                                                            pFeatureReportCopy[56], pFeatureReportCopy[57], pFeatureReportCopy[58], pFeatureReportCopy[59],
                                                                                                                            pFeatureReportCopy[60], pFeatureReportCopy[61], pFeatureReportCopy[62], pFeatureReportCopy[63]);

                                    Trace(TRACE_LEVEL_WARNING, "Feature Report read for %s is not an exact copy so retrying, retry count = %i, hr = %!HRESULT!", m_SensorName, dwRetryCount, hr);
                                }

                                dwRetryCount++;

                            } while ((dwRetryCount < dwMaxTries) && ((FALSE == fHasContent) || (FALSE == fIsExactCopy)));

                            if ((dwRetryCount > dwMaxTries) && (FALSE == fHasContent))
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Getting Feature Report from %s failed after %i tries, hr = %!HRESULT!", m_SensorName, dwRetryCount, hr);
                            }
                        }
                        else
                        {
                            hr = E_POINTER;
                            Trace(TRACE_LEVEL_ERROR, "pFeatureReport for %s is NULL, hr = %!HRESULT!", m_SensorName, hr);
                        }
                    }
                    else
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Feature Report size from %s is too large = %i, hr = %!HRESULT!", m_SensorName, uReportSize, hr);
                    }
                }
                else
                {
                    hr = E_POINTER;
                    Trace(TRACE_LEVEL_ERROR, "m_pHidIoctlRequest is NULL, hr = %!HRESULT!", hr);
                }
            }
            else
            {
                hr = E_POINTER;
                Trace(TRACE_LEVEL_ERROR, "m_pSensorDDI is NULL, hr = %!HRESULT!", hr);
            }
        }
        else
        {
            hr = E_POINTER;
            Trace(TRACE_LEVEL_ERROR, "m_pSensorManager is NULL, hr = %!HRESULT!", hr);
        }

        if (SUCCEEDED(hr))
        {
            //copy the returned feature report into the original report
            CopyMemory(pFeatureReport, pFeatureReportCopy, uReportSize);
            *pfIsExactCopy = fIsExactCopy;
        }
    }

#pragma warning(suppress: 6001) //uninitialized memory
    if (nullptr != pRecvReport)
    {
        delete[] pRecvReport;
    }
#pragma warning(suppress: 6001) //uninitialized memory
    if (nullptr != pFeatureReportCopy)
    {
        delete[] pFeatureReportCopy;
    }

    return hr;
}

VOID CSensor::ReportCommonInputReportDescriptorConditions(
        _In_ BOOL fSensorStateSelectorSupported,
        _In_ BOOL fEventTypeSelectorSupported,
        _In_ BOOL fSensorStateSupported,
        _In_ BOOL fEventTypeSupported 
        )
{
    UNREFERENCED_PARAMETER(fSensorStateSelectorSupported);
    UNREFERENCED_PARAMETER(fEventTypeSelectorSupported);
    UNREFERENCED_PARAMETER(fSensorStateSupported);
    UNREFERENCED_PARAMETER(fEventTypeSupported);

    if (FALSE == m_fInformedCommonInputReportConditions)
    {
        if (FALSE == m_fInitialDataReceived)
        {
            Trace(TRACE_LEVEL_INFORMATION, "%s Initial data not yet received", m_SensorName);
        }
        else
        {
            //sensor state checks
            if ((FALSE == fSensorStateSelectorSupported) && (FALSE == fSensorStateSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "No Sensor State definition used for %s in Input report", m_SensorName);
            }
            if ((FALSE == fSensorStateSelectorSupported) && (TRUE == fSensorStateSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Deprecated Sensor State/Status definition used for %s in Input report", m_SensorName);
            }
            if ((TRUE == fSensorStateSelectorSupported) && (TRUE == fSensorStateSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Redundant Sensor State/Status definitions used for %s in Input report", m_SensorName);
            }

            //event type checks
            if ((FALSE == fEventTypeSelectorSupported) && (FALSE == fEventTypeSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "No Event Type definition used for %s in Input report", m_SensorName);
            }
            if ((FALSE == fEventTypeSelectorSupported) && (TRUE == fEventTypeSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Deprecated Event Type definition used for %s in Input report", m_SensorName);
            }
            if ((TRUE == fEventTypeSelectorSupported) && (TRUE == fEventTypeSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Redundant Event Type definitions used for %s in Input report", m_SensorName);
            }

            m_fInformedCommonInputReportConditions = TRUE;
        }
    }


    return;
}

VOID CSensor::ReportCommonFeatureReportDescriptorConditions(
        _In_ BOOL fFeatureReportSupported,
        _In_ BOOL fReportingStateSelectorSupported,
        _In_ BOOL fPowerStateSelectorSupported,
        _In_ BOOL fSensorStatusSelectorSupported,
        _In_ BOOL fConnectionTypeSelectorSupported,
        _In_ BOOL fReportingStateSupported,
        _In_ BOOL fPowerStateSupported,
        _In_ BOOL fSensorStatusSupported,
        _In_ BOOL fConnectionTypeSupported,
        _In_ BOOL fReportIntervalSupported,
        _In_ BOOL fGlobalSensitivitySupported,
        _In_ BOOL fGlobalMaximumSupported,
        _In_ BOOL fGlobalMinimumSupported,
        _In_ BOOL fGlobalAccuracySupported,
        _In_ BOOL fGlobalResolutionSupported,
        _In_ BOOL fMinimumReportIntervalSupported,
        _In_ BOOL fFriendlyNameSupported,
        _In_ BOOL fPersistentUniqueIDSupported,
        _In_ BOOL fManufacturerSupported,
        _In_ BOOL fModelSupported,
        _In_ BOOL fSerialNumberSupported,
        _In_ BOOL fDescriptionSupported
        )
{
        UNREFERENCED_PARAMETER(fReportingStateSelectorSupported);
        UNREFERENCED_PARAMETER(fPowerStateSelectorSupported);
        UNREFERENCED_PARAMETER(fSensorStatusSelectorSupported);
        UNREFERENCED_PARAMETER(fConnectionTypeSelectorSupported);
        UNREFERENCED_PARAMETER(fReportingStateSupported);
        UNREFERENCED_PARAMETER(fPowerStateSupported);
        UNREFERENCED_PARAMETER(fSensorStatusSupported);
        UNREFERENCED_PARAMETER(fConnectionTypeSupported);
        UNREFERENCED_PARAMETER(fReportIntervalSupported);
        UNREFERENCED_PARAMETER(fGlobalSensitivitySupported);
        UNREFERENCED_PARAMETER(fGlobalMaximumSupported);
        UNREFERENCED_PARAMETER(fGlobalMinimumSupported);
        UNREFERENCED_PARAMETER(fGlobalAccuracySupported);
        UNREFERENCED_PARAMETER(fGlobalResolutionSupported);
        UNREFERENCED_PARAMETER(fMinimumReportIntervalSupported);
        UNREFERENCED_PARAMETER(fFriendlyNameSupported);
        UNREFERENCED_PARAMETER(fPersistentUniqueIDSupported);
        UNREFERENCED_PARAMETER(fManufacturerSupported);
        UNREFERENCED_PARAMETER(fModelSupported);
        UNREFERENCED_PARAMETER(fSerialNumberSupported);
        UNREFERENCED_PARAMETER(fDescriptionSupported);

    if (FALSE == m_fInformedCommonFeatureReportConditions)
    {
        if (FALSE == fFeatureReportSupported)
        {
            Trace(TRACE_LEVEL_WARNING, "%s Report Descriptor does not include a Feature report", m_SensorName);
        }
        else
        {
            //connection type checks
            if ((FALSE == fConnectionTypeSelectorSupported) && (FALSE == fConnectionTypeSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "No Connection Type definition used for %s in Feature report", m_SensorName);
            }
            if ((FALSE == fConnectionTypeSelectorSupported) && (TRUE == fConnectionTypeSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Deprecated Connection Type definition used for %s in Feature report", m_SensorName);
            }
            if ((TRUE == fConnectionTypeSelectorSupported) && (TRUE == fConnectionTypeSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Redundant Connection Type definitions used for %s in Feature report", m_SensorName);
            }

            //reporting state checks
            if ((FALSE == fReportingStateSelectorSupported) && (FALSE == fReportingStateSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "No Reporting State definition used for %s in Feature report", m_SensorName);
            }
            if ((FALSE == fReportingStateSelectorSupported) && (TRUE == fReportingStateSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Deprecated Reporting State definition used for %s in Feature report", m_SensorName);
            }
            if ((TRUE == fReportingStateSelectorSupported) && (TRUE == fReportingStateSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Redundant Reporting State definitions used for %s in Feature report", m_SensorName);
            }

            //power state checks
            if ((FALSE == fPowerStateSelectorSupported) && (FALSE == fPowerStateSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "No Power State definition used for %s in Feature report", m_SensorName);
            }
            if ((FALSE == fPowerStateSelectorSupported) && (TRUE == fPowerStateSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Deprecated Power State definition used for %s in Feature report", m_SensorName);
            }
            if ((TRUE == fPowerStateSelectorSupported) && (TRUE == fPowerStateSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Redundant Power State definitions used for %s in Feature report", m_SensorName);
            }

            //power state checks
            if ((FALSE == fSensorStatusSelectorSupported) && (FALSE == fSensorStatusSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "No Sensor State definition used for %s in Feature report", m_SensorName);
            }
            if ((FALSE == fSensorStatusSelectorSupported) && (TRUE == fSensorStatusSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Deprecated Sensor State definition used for %s in Feature report", m_SensorName);
            }
            if ((TRUE == fSensorStatusSelectorSupported) && (TRUE == fSensorStatusSupported))
            {
                Trace(TRACE_LEVEL_WARNING, "Redundant Sensor State definitions used for %s in Feature report", m_SensorName);
            }

            //report interval checks
            if (FALSE == fReportIntervalSupported)
            {
                Trace(TRACE_LEVEL_WARNING, "No Report Interval definition used for %s in Feature report", m_SensorName);
            }

            //change sensitivity checks
            if (TRUE == fGlobalSensitivitySupported)
            {
                Trace(TRACE_LEVEL_WARNING, "Use of Global Change Sensitivity in Feature report for %s is not recommended - consider the Bulk or Datafield forms", m_SensorName);
            }

            //range maximum checks
            if (TRUE == fGlobalMaximumSupported)
            {
                Trace(TRACE_LEVEL_WARNING, "Use of Global Maximum in Feature report for %s is not recommended - consider the Bulk or Datafield forms", m_SensorName);
            }

            //range minimum checks
            if (TRUE == fGlobalMinimumSupported)
            {
                Trace(TRACE_LEVEL_WARNING, "Use of Global Minimum in Feature report for %s is not recommended - consider the Bulk or Datafield forms", m_SensorName);
            }

            //accuracy checks
            if (TRUE == fGlobalAccuracySupported)
            {
                Trace(TRACE_LEVEL_WARNING, "Use of Global Accuracy in Feature report for %s is not recommended - consider the Bulk or Datafield forms", m_SensorName);
            }

            //resolution checks
            if (TRUE == fGlobalResolutionSupported)
            {
                Trace(TRACE_LEVEL_WARNING, "Use of Global Resolution in Feature report for %s is not recommended - consider the Bulk or Datafield forms", m_SensorName);
            }

            //minimum report interval checks
            if (TRUE == fMinimumReportIntervalSupported)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Minimum Report Inteval present in Feature report for %s", m_SensorName);
            }

            //persistent unique ID string checks
            if (TRUE == fPersistentUniqueIDSupported)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Persistent Uniquie ID string is present in Feature report for %s", m_SensorName);
            }

            //manufacturer string checks
            if (TRUE == fManufacturerSupported)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Manufacturer string is present in Feature report for %s", m_SensorName);
            }

            //model string checks
            if (TRUE == fModelSupported)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Model string is present in Feature report for %s", m_SensorName);
            }

            //serial number string checks
            if (TRUE == fSerialNumberSupported)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Serial Number string is present in Feature report for %s", m_SensorName);
            }

            //description string checks
            if (TRUE == fDescriptionSupported)
            {
                Trace(TRACE_LEVEL_INFORMATION, "Description string is present in Feature report for %s", m_SensorName);
            }

        }
    }

    m_fInformedCommonFeatureReportConditions = TRUE;

    return;
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


