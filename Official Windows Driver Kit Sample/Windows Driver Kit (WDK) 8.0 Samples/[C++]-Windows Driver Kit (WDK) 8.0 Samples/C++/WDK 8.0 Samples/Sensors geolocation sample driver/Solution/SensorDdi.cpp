//
//    Copyright (C) Microsoft.  All rights reserved.
//
/*++
 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Module Name:

    SensorDDI.cpp

Abstract:

      This module implements the ISensorDriver interface which is used
      by the Sensor Class Extension.
--*/


#include "internal.h"
#include "SensorManager.h"
#include "SensorDDI.h"

#include "devpkey.h"
#include <strsafe.h>

#include "Sensor.h"
#include "Geolocation.h"


#include "SensorDDI.tmh"

const PROPERTYKEY g_SupportedCommonProperties[] =
{
    WPD_OBJECT_ID,
    WPD_OBJECT_PERSISTENT_UNIQUE_ID,
    WPD_OBJECT_PARENT_ID,
    WPD_OBJECT_NAME,
    WPD_OBJECT_FORMAT,
    WPD_OBJECT_CONTENT_TYPE,
    WPD_OBJECT_CAN_DELETE
};

const PROPERTYKEY g_SupportedDeviceProperties[] =
{
    WPD_DEVICE_FIRMWARE_VERSION,
    WPD_DEVICE_POWER_LEVEL,
    WPD_DEVICE_POWER_SOURCE,
    WPD_DEVICE_PROTOCOL,
    WPD_DEVICE_MODEL,
    WPD_DEVICE_SERIAL_NUMBER,
    WPD_DEVICE_SUPPORTS_NON_CONSUMABLE,
    WPD_DEVICE_MANUFACTURER,
    WPD_DEVICE_FRIENDLY_NAME,
    WPD_DEVICE_TYPE,
    WPD_FUNCTIONAL_OBJECT_CATEGORY
};


/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::CSensorDDI()
//
// Constructor.
//
/////////////////////////////////////////////////////////////////////////
CSensorDDI::CSensorDDI()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::~CSensorDDI()
//
// Destructor
//
/////////////////////////////////////////////////////////////////////////
CSensorDDI::~CSensorDDI()
{

}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::InitSensorDevice
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::InitSensorDevice(_In_ IWDFDevice* pWdfDevice)
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = (nullptr != pWdfDevice) ? S_OK : E_UNEXPECTED;

    // NOTE: This is where the hardware should be configured for first (or restarted) use

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::DeInitSensorDevice
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::DeInitSensorDevice()
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    // NOTE: Any device configuration should be stored here and reused on InitSensorDevice

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::UpdateSensorValues
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::UpdateSensorPropertyValues(_In_ LPWSTR wszObjectID, _In_ BOOL fSettableOnly)
{
    UNREFERENCED_PARAMETER(fSettableOnly);

    HRESULT             hr = S_OK;
    CSensor*            pSensor = NULL;  
    SensorType          sensType = SensorTypeNone;
    DWORD               sensIndex = 0;

    hr = FindSensorTypeFromObjectID((LPWSTR)wszObjectID, &sensType, &sensIndex);

    if (SUCCEEDED(hr))
    {
        if (m_pSensorManager->m_pSensorList.GetCount() > 0)
        {
            hr = GetSensorObject(sensIndex, &pSensor);
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "No sensors found, hr = %!HRESULT!", hr);
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            CGeolocation* pGeolocation = nullptr;

            switch (sensType)
            {
            case Geolocation:
                pGeolocation = static_cast<CGeolocation*>(pSensor);
                hr = pGeolocation->UpdateGeolocationPropertyValues();
                break;

            default:
                hr = E_UNEXPECTED;
                break;
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get pSensor, hr = %!HRESULT!", hr);
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::UpdateSensorValues
//
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::UpdateSensorDataFieldValues(_In_ LPWSTR wszObjectID)
{
    HRESULT             hr = S_OK;
    CSensor*            pSensor = nullptr;  
    SensorType          sensType = SensorTypeNone;
    DWORD               sensIndex = 0;

    hr = FindSensorTypeFromObjectID((LPWSTR)wszObjectID, &sensType, &sensIndex);

    if (SUCCEEDED(hr))
    {
        hr = GetSensorObject(sensIndex, &pSensor);

        if (FAILED(hr))
        {
            Trace(TRACE_LEVEL_ERROR, "Device is not a supported sensor, hr = %!HRESULT!", hr);
        }
    }

    if (SUCCEEDED(hr))
    {
        CGeolocation* pGeolocation = nullptr;

        switch (sensType)
        {
        case Geolocation:
            pGeolocation = static_cast<CGeolocation*>(pSensor);
            hr = pGeolocation->UpdateGeolocationDataFieldValues();
            break;

        default:
            hr = E_UNEXPECTED;
            break;
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::RequestDeviceInfo
//
// Synchronously request the report descriptor, which contains
// the sensor type.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::RequestDeviceInfo(
            _Inout_ SensorType* pSensType,
            _Inout_updates_(DESCRIPTOR_MAX_LENGTH) LPWSTR pwszManufacturer,
            _Inout_updates_(DESCRIPTOR_MAX_LENGTH) LPWSTR pwszProduct,
            _Inout_updates_(DESCRIPTOR_MAX_LENGTH) LPWSTR pwszSerialNumber,
            _Inout_updates_(DESCRIPTOR_MAX_LENGTH) LPWSTR pwszDeviceID)
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    if (SUCCEEDED(hr))
    {
        ULONG idx = 0;
        // This is a list of the sensors supported by this driver. It takes the form of:
        // ULONG SensorList[] = { USAGE_SENSOR1, USAGE_SENSOR2, ..., USAGE_SENSORn };
        ULONG SensorList[] = { USAGE_GEOLOCATION_SENSOR };
        ULONG numNodes = ARRAYSIZE(SensorList);

        if ((SUCCEEDED(hr)) && (numNodes > 0))
        {
            *pSensType = Collection;

            Trace(TRACE_LEVEL_CRITICAL, "Sensor collection present with %i sensors", numNodes);

            for (idx = 0; idx < numNodes; idx++)
            {
                switch (SensorList[idx])
                {
                case USAGE_GEOLOCATION_SENSOR:
                    m_pSensorManager->m_AvailableSensorsTypes[idx] = Geolocation;
                    Trace(TRACE_LEVEL_CRITICAL, "Sensor %i is Geolocation", idx+1);
                    break;

                default:
                    break;
                }
            }

            if (m_pSensorManager->m_AvailableSensorsTypes.GetCount() != (numNodes))
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Invalid sensor type discovered in a collection node, hr = %!HRESULT!", hr);
            }
        }
    }
   
    if (SUCCEEDED(hr))
    {
        wcscpy_s(pwszSerialNumber, DESCRIPTOR_MAX_LENGTH, DEFAULT_SERIAL_NUMBER);
        Trace(TRACE_LEVEL_CRITICAL, "Device Serial Number = %ls", pwszSerialNumber);

        wcscpy_s(pwszDeviceID, DESCRIPTOR_MAX_LENGTH, DEFAULT_SERIAL_NUMBER);
        Trace(TRACE_LEVEL_CRITICAL, "Device ID            = %ls", pwszDeviceID);
    }

    if (SUCCEEDED(hr))
    {
        wcscpy_s(pwszManufacturer, DESCRIPTOR_MAX_LENGTH, DEFAULT_MANUFACTURER);
        Trace(TRACE_LEVEL_CRITICAL, "Device Manufacturer  = %ls", pwszManufacturer);
    }

    if (SUCCEEDED(hr))
    {
        wcscpy_s(pwszProduct, DESCRIPTOR_MAX_LENGTH, DEFAULT_DEVICE_MODEL_VALUE);
        Trace(TRACE_LEVEL_CRITICAL, "Device Product       = %ls", pwszProduct);
    }

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetSupportedSensorObjects
//
// Routine Description:
//
//  This method is called by Sensor Class Extension during the initialize procedure to get 
//  the list of sensor objects and their supported properties.
//  
// Arguments:
//
//  ppPortableDeviceValuesCollection - A double IPortableDeviceValuesCollection pointer
//                                     that receives the set of Sensor property values
//
// Return Value:
//
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetSupportedSensorObjects(
        _Out_ IPortableDeviceValuesCollection** ppPortableDeviceValuesCollection
        )
{
    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        // CoCreate a collection to store the sensor object identifiers.
        hr = CoCreateInstance(CLSID_PortableDeviceValuesCollection,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_PPV_ARGS(ppPortableDeviceValuesCollection));

        CComBSTR objectId;
        CComPtr<IPortableDeviceKeyCollection> spKeys;
        CComPtr<IPortableDeviceValues> spValues;

        POSITION pos = m_pSensorManager->m_AvailableSensorsIDs.GetStartPosition();

        while( NULL != pos)
        {
            objectId = m_pSensorManager->m_AvailableSensorsIDs.GetNextValue(pos);


            if (hr == S_OK)
            {
                hr = OnGetSupportedProperties(objectId, &spKeys);
            }

            if (hr == S_OK)
            {
#pragma warning(suppress: 6387) //nullptr is invalid parameter - use intentional
                hr = OnGetPropertyValues( nullptr, objectId, spKeys, &spValues );
            }

            if (hr == S_OK)
            {
                hr = (*ppPortableDeviceValuesCollection)->Add(spValues);
            }

            spKeys.Release();
            spValues.Release();
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Exit, hr = %!HRESULT!", hr);

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetSupportedProperties
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to get the list of supported properties
//  for a particular Sensor.
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys are being requested
//  ppKeys - An IPortableDeviceKeyCollection to be populated with supported PROPERTYKEYs
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetSupportedProperties(
        _In_  LPWSTR pwszObjectID,
        _Out_ IPortableDeviceKeyCollection** ppKeys
        )
{
    Trace(TRACE_LEVEL_VERBOSE, "Get Properties supported by sensor %ls", pwszObjectID);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        // CoCreate a collection to store the supported property keys.
        hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_PPV_ARGS(ppKeys));

        // Add supported property keys for the specified object to the collection
        if (SUCCEEDED(hr))
        {
            hr = AddSupportedPropertyKeys(pwszObjectID, *ppKeys);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_VERBOSE, "Properties supported by sensor %ls returned, hr = %!HRESULT!", pwszObjectID, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetSupportedDataFields
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to get the list of supported data fields
//  for a particular Sensor.
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys are being requested
//  ppKeys - An IPortableDeviceKeyCollection to be populated with supported PROPERTYKEYs
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetSupportedDataFields(
        _In_  LPWSTR wszObjectID,
        _Out_ IPortableDeviceKeyCollection** ppKeys
        )
{
    Trace(TRACE_LEVEL_VERBOSE, "Get Datafields supported by sensor %ls", wszObjectID);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        // CoCreate a collection to store the supported property keys.
        hr = CoCreateInstance(CLSID_PortableDeviceKeyCollection,
                                  NULL,
                                  CLSCTX_INPROC_SERVER,
                                  IID_PPV_ARGS(ppKeys));

        // Add supported property keys for the specified object to the collection
        if (hr == S_OK && ppKeys != NULL)
        {
            hr = AddSupportedDataFieldKeys(wszObjectID, *ppKeys);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_VERBOSE, "Datafields supported by sensor %ls returned, hr = %!HRESULT!", wszObjectID, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetSupportedEvents
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to get the list of supported events
//  for a particular Sensor.
//  
// Arguments:
//
// ObjectID - String that represents the object whose supported property keys are being requested
//  ppSupportedEvents - A set of GUIDs that represent the supported events
//  pulEventCount - Count of the number of events supported
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetSupportedEvents(
        _In_  LPWSTR ObjectID,
        _Out_ GUID **ppSupportedEvents,
        _Out_ ULONG *pulEventCount
        )
{
    Trace(TRACE_LEVEL_INFORMATION, "Get Events supported by sensor %ls", ObjectID);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        SensorType sensType = SensorTypeNone;
        DWORD sensIndex = 0;

        hr = FindSensorTypeFromObjectID((LPWSTR)ObjectID, &sensType, &sensIndex);

        if (SUCCEEDED(hr))
        {
            //Return two GUIDs
            GUID* pBuf = (GUID*)CoTaskMemAlloc(sizeof(GUID) * 2);

            if( pBuf != NULL )
            {
                *pBuf       = SENSOR_EVENT_DATA_UPDATED;
                *(pBuf + 1) = SENSOR_EVENT_STATE_CHANGED;

                *ppSupportedEvents = pBuf;
                *pulEventCount = 2;
            }
            else
            {
                hr = E_OUTOFMEMORY;

                *ppSupportedEvents = NULL;
                *pulEventCount = 0;
            }
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_INFORMATION, "Events supported by sensor %ls returned, hr = %!HRESULT!", ObjectID, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetProperties
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to get Sensor property values 
//  for a particular Sensor.
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnGetProperties(
        _In_  IWDFFile* appId,
        _In_  LPWSTR ObjectID,
        _In_  IPortableDeviceKeyCollection* pKeys,
        _Out_ IPortableDeviceValues** ppValues
        )
{
    Trace(TRACE_LEVEL_VERBOSE, "Get Properties supported by sensor %ls for Client %p", ObjectID, appId);

    UNREFERENCED_PARAMETER(appId);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        if ((ObjectID == NULL) ||
            (pKeys       == NULL) ||
            (ppValues     == NULL))
        {
            hr = E_INVALIDARG;
        }

        if (SUCCEEDED(hr))
        {
            hr = OnGetPropertyValues( appId, ObjectID, pKeys, ppValues );
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_VERBOSE, "Properties supported by sensor %ls returned for Client %p, hr = %!HRESULT!", ObjectID, appId, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetRadioState
//
// Routine Description:
//
//  This method is called for Radio Management IO and gets the current
//  radio state.
//
// Arguments:
//
//  pRequest - The IO Request
//  fPreviousState - If to get the previous radio state instead of the
//                   current radio state.
//
// Return Value:
//  Status
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::OnGetRadioState(
        _In_ IWDFIoRequest* pRequest,
        _In_ bool fPreviousState
        )
{
    HRESULT hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

#if (NTDDI_VERSION >= NTDDI_WIN8)
    if (SUCCEEDED(hr))
    {
        CComPtr<IWDFMemory> spOutputMemory = nullptr;
        pRequest->GetOutputMemory(&spOutputMemory);
        if (nullptr != spOutputMemory)
        {
            SIZE_T  cbBufferOut  = 0;
            void* pOutBuffer = spOutputMemory->GetDataBuffer(&cbBufferOut);
            if (sizeof(DEVICE_RADIO_STATE) == cbBufferOut && nullptr != pOutBuffer)
            {
                CSensor* pSensor = nullptr;
                hr = GetGeolocationSensorObject(&pSensor);
                if (SUCCEEDED(hr) && nullptr != pSensor)
                {
                    CGeolocation* pGeolocation = (CGeolocation*)pSensor;

                    if (fPreviousState)
                    {
                        *((DEVICE_RADIO_STATE*)pOutBuffer) = (DEVICE_RADIO_STATE)pGeolocation->m_ulPreviousGeolocationRadioState;
                    }
                    else
                    {
                        *((DEVICE_RADIO_STATE*)pOutBuffer) = (DEVICE_RADIO_STATE)pGeolocation->m_ulCurrentGeolocationRadioState;
                    }

                    Trace(TRACE_LEVEL_VERBOSE, "%!FUNC! Got radio state = %d", *((DEVICE_RADIO_STATE*)pOutBuffer));
                }
            }
        }
    }
#else
    UNREFERENCED_PARAMETER(pRequest);
    UNREFERENCED_PARAMETER(fPreviousState);
#endif

    ExitProcessing(PROCESSING_ISENSORDRIVER);
    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnGetDataFields
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to get Sensor data fields
//  for a particular Sensor.
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
HRESULT CSensorDDI:: OnGetDataFields(
        _In_  IWDFFile* appId,
        _In_  LPWSTR wszObjectID,
        _In_  IPortableDeviceKeyCollection* pKeys,
        _Out_ IPortableDeviceValues** ppValues
        )
{
    UNREFERENCED_PARAMETER(appId);

    Trace(TRACE_LEVEL_VERBOSE, "Get Datafields supported by sensor %ls for Client %p", wszObjectID, appId);


    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        if ((wszObjectID == NULL) ||
            (pKeys       == NULL) ||
            (ppValues     == NULL))
        {
            hr = E_INVALIDARG;
        }

        if (SUCCEEDED(hr))
        {
            // CoCreate a collection to store the property values.
            hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(ppValues));

            // Read the specified properties on the specified object and add the property values to the collection.
            if (hr == S_OK)
            {
                DWORD       cKeys       = 0;
                IPortableDeviceValues*  pValues = *ppValues; //fakes this call: hr = GetDataValuesForObject(wszObjectID, pKeys, *ppValues);

                hr = pKeys->GetCount(&cKeys);

                if (hr == S_OK)
                {
                    CAtlStringW strObjectID = wszObjectID;
                    CComBSTR objectId;
            
                    POSITION posID = m_pSensorManager->m_AvailableSensorsIDs.GetStartPosition();
                    POSITION posType = m_pSensorManager->m_AvailableSensorsTypes.GetStartPosition();
                    SensorType sensType = SensorTypeNone;
                    DWORD sensIndex = 0;
                    BOOL fPollFailed = FALSE;

                    while( NULL != posID)
                    {
                        objectId = m_pSensorManager->m_AvailableSensorsIDs.GetNextValue(posID);
                        sensType = m_pSensorManager->m_AvailableSensorsTypes.GetNextValue(posType);

                        if (strObjectID.CompareNoCase(objectId) == 0)
                        {
                            CSensor* pSensor = nullptr;

                            hr = FindSensorTypeFromObjectID((LPWSTR)wszObjectID, &sensType, &sensIndex);

                            if (SUCCEEDED(hr))
                            {
                                hr = GetSensorObject(sensIndex, &pSensor);

                                if (SUCCEEDED(hr) && (nullptr != pSensor))
                                {
                                    Trace(TRACE_LEVEL_INFORMATION, "Client %p Requesting Datafield Values for %s", appId, pSensor->m_SensorName);

                                    if ((FALSE == pSensor->m_fReportingState) || (FALSE == pSensor->m_fInitialDataReceived))
                                    {
                                        {
                                            CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSection); //make this thread safe
                                
                                            //issue a poll to the device for this sensor and wait for updated data from sensor
                                            pSensor->m_fSensorUpdated = FALSE;
                                            //NOTE: m_nNotifyOnSensorUpdate should be the Sensor Number n = {1..k}
                                            //this is derived from sensIndex = {0..k-1} so 1 is added indicating it is
                                            //a valid sensor (anything > 0)
                                            m_nNotifyOnSensorUpdate = sensIndex+1;
                                        }

                                        const DWORD dwInitialDataMaxTries = INITIAL_DATA_POLL_MAX_RETRIES;
                                        DWORD dwInitialDataTryCount = 0;

                                        do
                                        {
                                            fPollFailed = FALSE;

                                            Trace(TRACE_LEVEL_INFORMATION, "Sending poll request to device to update Datafield Values for %s", pSensor->m_SensorName);

                                            hr = UpdateSensorDataFieldValues(m_pSensorManager->m_AvailableSensorsIDs[sensIndex]);

                                            if (FAILED(hr))
                                            {
                                                Trace(TRACE_LEVEL_ERROR, "Failed to request input report from %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                            }
                                            else
                                            {
                                                const DWORD dwDataPollTimeLimit = DEVICE_POLL_TIMEOUT; //mS

                                                ULONGLONG ullInitialEventTime = GetTickCount64();
                                                ULONGLONG ulCurrentEventTime = 0;
                                                ULONG ulTimePassed = 0;
                                                ULONG ulYieldCount = 0;

                                                do //spin here until dwDataPollTimeLimit has passed;
                                                {
                                                    Yield();
                                                    ulYieldCount++;

                                                    ulCurrentEventTime = GetTickCount64();
                                                    ulTimePassed = (ULONG)(ulCurrentEventTime - ullInitialEventTime); //time passed in milliseconds

                                                } while ((FALSE == pSensor->m_fSensorUpdated) && (ulTimePassed < dwDataPollTimeLimit));

                                                if (ulTimePassed > dwDataPollTimeLimit)
                                                {
                                                    fPollFailed = TRUE;
                                                    dwInitialDataTryCount++;

                                                    //do not fail - instead, fall through and get the latest value from the API
                                                    Trace(TRACE_LEVEL_ERROR, "Failed to receive poll response from %s within %i mS, failed at %i mS and %i yields, hr = %!HRESULT!", pSensor->m_SensorName, dwDataPollTimeLimit, ulTimePassed, ulYieldCount, hr);
                                                }
                                                else
                                                {
                                                    if (TRUE == pSensor->m_fSensorUpdated) 
                                                    {
                                                        pSensor->m_fInitialDataReceived = TRUE;
                                                        Trace(TRACE_LEVEL_INFORMATION, "Received polled datafield update for %s after %i mS, hr = %!HRESULT!", pSensor->m_SensorName, ulTimePassed, hr);
                                                    }
                                                }
                                            }

                                        } while ((FALSE == pSensor->m_fInitialDataReceived) && (dwInitialDataTryCount < dwInitialDataMaxTries));

                                        if (FALSE == pSensor->m_fInitialDataReceived)
                                        {
                                            hr = E_UNEXPECTED;
                                            Trace(TRACE_LEVEL_ERROR, "Failed to receive initial data from %s after %i tries, hr = %!HRESULT!", pSensor->m_SensorName, dwInitialDataTryCount, hr);
                                        }
                                    }
                                }
                                else
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to get pSensor, hr = %!HRESULT!", hr);
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;

                                if (TRUE == wcscmp(L"DEVICE", wszObjectID))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", wszObjectID, hr);
                                }
                                else
                                {
                                    //WPD is calling us, so ignore trace
                                }
                            }


                            if (SUCCEEDED(hr))
                            {
                                for (DWORD dwIndex = 0; dwIndex < cKeys; dwIndex++)
                                {
                                    PROPERTYKEY Key = WPD_PROPERTY_NULL;
                                    hr = pKeys->GetAt(dwIndex, &Key);

                                    if (hr == S_OK)
                                    {
                                        PROPVARIANT var;
                                        PropVariantInit( &var );

                                        // Preset the property value to 'error not supported'.  The actual value
                                        // will replace this value, if read from the device.
                                        hr = pValues->SetErrorValue(Key, HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED));
                            
                                        if (SUCCEEDED(hr))
                                        {
                                            if (nullptr != pSensor)
                                            {
                                                if (SUCCEEDED(pSensor->GetDataField(Key, &var)))
                                                {
                                                    hr = pValues->SetValue(Key, &var);
                                                }
                                                else
                                                {
                                                    Trace(TRACE_LEVEL_ERROR, "Failed to get datafield %!GUID!-%i value from %s", &Key.fmtid, Key.pid, pSensor->m_SensorName);
                                                }
                                            }
                                            else
                                            {
                                                hr = E_POINTER;
                                                Trace(TRACE_LEVEL_ERROR, "pSensor == NULL before getting datafield %!GUID!-%i value from %s, hr = %!HRESULT!", &Key.fmtid, Key.pid, pSensor->m_SensorName, hr);
                                            }
                                        }
                                        else
                                        {
                                            hr = E_UNEXPECTED;
                                            Trace(TRACE_LEVEL_ERROR, "Failed on idx = %i for SetErrorValue for %!GUID!-%i value from %s, hr = %!HRESULT!", dwIndex, &Key.fmtid, Key.pid, pSensor->m_SensorName, hr);
                                        }

                                        // Free any allocated resources
                                        PropVariantClear( &var );
                                    }
                                    else
                                    {
                                        Trace(TRACE_LEVEL_ERROR, "Failed to update datafield %!GUID!-%i value from %s, hr = %!HRESULT!", &Key.fmtid, Key.pid, pSensor->m_SensorName, hr);
                                    }
                                }

                                if (SUCCEEDED(hr))
                                {
                                    if (FALSE == pSensor->m_fReportingState)
                                    {
                                        if (FALSE == fPollFailed)
                                        {
                                            Trace(TRACE_LEVEL_INFORMATION, "Returned poll-updated datafield values for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                        }
                                        else
                                        {
                                            Trace(TRACE_LEVEL_INFORMATION, "Returned current datafield values for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                        }
                                    }
                                    else
                                    {
                                        Trace(TRACE_LEVEL_INFORMATION, "Returned async-updated datafield values for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                    }
                                }
                                else
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed while updating datafield values for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                }
                            }
                            else
                            {
                                if (nullptr != pSensor)
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed while polling datafield values for %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                                }
                                else
                                {
                                    Trace(TRACE_LEVEL_ERROR, "pSensor was NULL while polling datafield values, hr = %!HRESULT!", hr);
                                }
                            }
                        }

                        if (sensType == SensorTypeNone)
                        {
                            hr = E_UNEXPECTED;
                            Trace(TRACE_LEVEL_ERROR, "Failed because sensType == SensorTypeNone, hr = %!HRESULT!", hr);
                        }
                    }
                }

                //*ppValues = pValues; //fakes return of this call: hr = GetDataValuesForObject(wszObjectID, pKeys, *ppValues);
            }
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_VERBOSE, "Datafields supported by sensor %ls returned for Client %p, hr = %!HRESULT!", wszObjectID, appId, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnSetProperties
//
// Routine Description:
//
//  This method is called by Sensor Class Extension to set Sensor properties
//  for a particular Sensor.
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys 
//             are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnSetProperties(
        _In_ IWDFFile* appId,
        _In_ LPWSTR ObjectID,
        _In_ IPortableDeviceValues* pValues,
        _Out_ IPortableDeviceValues** ppResults
        )
{
    Trace(TRACE_LEVEL_VERBOSE, "Set Properties on sensor %ls for Client %p", ObjectID, appId);

    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        if ((ObjectID == NULL) ||
            (pValues       == NULL) ||
            (ppResults     == NULL))
        {
            hr = E_INVALIDARG;
        }

        // Update our cache
        if(SUCCEEDED(hr))
        {
            DWORD           cValues     = 0;
            BOOL            fHasError   = FALSE;
            SensorType      sType       = SensorTypeNone;
            DWORD           sIndex      = 0;
            CSensor*        pSensor     = nullptr;

            // CoCreate a collection to store the property set operation results.
            hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                      NULL,
                                      CLSCTX_INPROC_SERVER,
                                      IID_PPV_ARGS(ppResults));

            // Get the sensor type 
            if( hr == S_OK )
            {        
                hr = FindSensorTypeFromObjectID(ObjectID, &sType, &sIndex);
            }

            if (SUCCEEDED(hr) && (sType != SensorTypeNone))
            {
                hr = GetSensorObject(sIndex, &pSensor);
            }
            else
            {
                hr = E_UNEXPECTED;

                if (TRUE == wcscmp(L"DEVICE", ObjectID))
                {
                    Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", ObjectID, hr);
                }
                else
                {
                    //WPD is calling us, so ignore trace
                }
            }

            // Set the property values on the specified object
            if ((hr == S_OK) && (nullptr != pSensor))
            {
                hr = pValues->GetCount(&cValues);

                if (hr == S_OK)
                {

                    for (DWORD dwIndex = 0; dwIndex < cValues; dwIndex++)
                    {
                        PROPERTYKEY Key = WPD_PROPERTY_NULL;
                        PROPVARIANT var;

                        PropVariantInit( &var );

                        hr = pValues->GetAt(dwIndex, &Key, &var);

                        if(hr == S_OK)
                        {
                            // Check if this is a supported settable property
                            if(TRUE == IsSettablePropertyKey(sIndex, Key))
                            {

                                FLOAT clientChangeSensitivities[MAX_NUM_DATA_FIELDS] = {0};
                                ULONG clientReportInterval = 0;
                                ULONG clientLocationDesiredAccuracy = 0;

                                if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_CURRENT_REPORT_INTERVAL))
                                {
                                    //set the report interval for this client in the client report interval list
                                    hr = FindClientFromAppID( pSensor, appId, &clientReportInterval, &clientLocationDesiredAccuracy, clientChangeSensitivities);

                                    if (SUCCEEDED(hr))
                                    {
                                        if (VT_UI4 == var.vt)
                                        {
                                            if (0 == var.uintVal)
                                            {
                                                //set the report interval for this sensor client to 0 to indicate "use the default"
                                                pSensor->m_pClientMap[appId].ulClientReportInterval = 0;
                                            }
                                            else if (var.uintVal >= pSensor->m_ulDefaultMinimumReportInterval)
                                            {
                                                pSensor->m_pClientMap[appId].ulClientReportInterval = var.uintVal;
                                            }
                                            else if (var.vt == VT_EMPTY)
                                            {
                                                //Do not allow deletion of the property
                                                hr = (*ppResults)->SetErrorValue(Key, E_ACCESSDENIED);
                                                fHasError = TRUE;
                                            }
                                            else
                                            {
                                                hr = (*ppResults)->SetErrorValue(Key, E_INVALIDARG);
                                                fHasError = TRUE;
                                            }

                                            if (SUCCEEDED(hr) && (FALSE == fHasError))
                                            {
                                                hr = SelectClientReportInterval(pSensor); 
                                            }
                                        }
                                        else
                                        {
                                            hr = E_UNEXPECTED;
                                            Trace(TRACE_LEVEL_ERROR, "var.vt is not == VT_UI4, hr = %!HRESULT!", hr);

                                            hr = (*ppResults)->SetErrorValue(Key, E_INVALIDARG);
                                            fHasError = TRUE;
                                        }
                                    }
                                }
                                else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_LOCATION_DESIRED_ACCURACY))
                                {
                                    //set the report interval for this client in the client report interval list
                                    hr = FindClientFromAppID( pSensor, appId, &clientReportInterval, &clientLocationDesiredAccuracy, clientChangeSensitivities);

                                    if (SUCCEEDED(hr))
                                    {
                                        if (VT_UI4 == var.vt)
                                        {
                                            if (0 == var.uintVal)
                                            {
                                                //set the location desired accuracy for this sensor client to 0 to indicate "use the default"
                                                pSensor->m_pClientMap[appId].ulClientLocationDesiredAccuracy = LOCATION_DESIRED_ACCURACY_DEFAULT;
                                            }
                                            else if (var.uintVal <= LOCATION_DESIRED_ACCURACY_HIGH)
                                            {
                                                if (var.uintVal > LOCATION_DESIRED_ACCURACY_DEFAULT)
                                                {
                                                    pSensor->m_pClientMap[appId].ulClientLocationDesiredAccuracy = var.uintVal;
                                                }
                                            }
                                            else if (var.vt == VT_EMPTY)
                                            {
                                                //Do not allow deletion of the property
                                                hr = (*ppResults)->SetErrorValue(Key, E_ACCESSDENIED);
                                                fHasError = TRUE;
                                            }
                                            else
                                            {
                                                hr = (*ppResults)->SetErrorValue(Key, E_INVALIDARG);
                                                fHasError = TRUE;
                                            }

                                            if (SUCCEEDED(hr) && (FALSE == fHasError))
                                            {
                                                hr = SelectClientLocationDesiredAccuracy(pSensor); 
                                            }
                                        }
                                        else
                                        {
                                            hr = E_UNEXPECTED;
                                            Trace(TRACE_LEVEL_ERROR, "var.vt is not == VT_UI4, hr = %!HRESULT!", hr);

                                            hr = (*ppResults)->SetErrorValue(Key, E_INVALIDARG);
                                            fHasError = TRUE;
                                        }
                                    }
                                }
                                else if (TRUE == IsEqualPropertyKey(Key, SENSOR_PROPERTY_CHANGE_SENSITIVITY))
                                {
                                    PROPERTYKEY pkDfKey = {0};
                                    PROPERTYKEY pkDvKey = {0};
                                    DWORD cDfVals = 0;
                                    DWORD cDfKeys = 0;
                                    PROPVARIANT val;

                                    CComPtr<IPortableDeviceValues> spDfVals = nullptr;

                                    if ((VT_UNKNOWN == var.vt) && (var.punkVal != nullptr))
                                    {
                                        IUnknown* pUnknown = var.punkVal;

                                        hr = pUnknown->QueryInterface(IID_PPV_ARGS(&spDfVals));

                                        if (SUCCEEDED(hr) && (nullptr != spDfVals))
                                        {
                                            hr = spDfVals->GetCount(&cDfVals);
                                        }
                                        else
                                        {
                                            hr = E_UNEXPECTED;
                                            Trace(TRACE_LEVEL_ERROR, "spDfValsSource == nullptr or failed, hr = %!HRESULT!", hr);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = pSensor->m_spSupportedSensorDataFields->GetCount(&cDfKeys);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            hr = FindClientFromAppID( pSensor, appId, &clientReportInterval, &clientLocationDesiredAccuracy, clientChangeSensitivities);
                                        }

                                        if (SUCCEEDED(hr))
                                        {
                                            for (DWORD dwDvIdx = 0; dwDvIdx < cDfVals; dwDvIdx++)
                                            {
                                                DWORD dwDkIdx = 0;

                                                PropVariantInit( &val );

                                                //get the key/value from the value values
                                                hr = spDfVals->GetAt(dwDvIdx, &pkDvKey, &val);

                                                if (SUCCEEDED(hr))
                                                {
                                                    HRESULT hrError = E_UNEXPECTED;

                                                    if (V_VT(&val) == VT_R4 || V_VT(&val) == VT_R8 || V_VT(&val) == VT_UI4)
                                                    {
                                                        float fltVal;

                                                        if (V_VT(&val) == VT_UI4)
                                                        {
                                                            fltVal = V_VT(&val) == static_cast<float>(V_UI4(&val));
                                                        }
                                                        else
                                                        {
                                                            fltVal = V_VT(&val) == VT_R8 ? static_cast<float>(V_R8(&val)) : V_R4(&val);
                                                        }
                                                        if (fltVal < 0.0F)
                                                        {
                                                            hr = (*ppResults)->SetErrorValue(Key, E_INVALIDARG);
                                                            fHasError = TRUE;
                                                        }
                                                        else
                                                        {
                                                            //find the value key in the supported data fields
                                                            for (dwDkIdx = 0; dwDkIdx < cDfKeys; dwDkIdx++)
                                                            {
                                                                hr = pSensor->m_spSupportedSensorDataFields->GetAt(dwDkIdx, &pkDfKey);

                                                                if (SUCCEEDED(hr))
                                                                {
                                                                    if (IsEqualPropertyKey(pkDfKey, pkDvKey))
                                                                    {
                                                                        pSensor->m_pClientMap[appId].fltClientChangeSensitivity[dwDkIdx] = fltVal;

                                                                        hrError = S_OK;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (V_VT(&val) == VT_NULL) 
                                                    {
                                                        //find the value key in the supported data fields
                                                        for (dwDkIdx = 0; dwDkIdx < cDfKeys; dwDkIdx++)
                                                        {
                                                            hr = pSensor->m_spSupportedSensorDataFields->GetAt(dwDkIdx, &pkDfKey);

                                                            if (SUCCEEDED(hr))
                                                            {
                                                                if (IsEqualPropertyKey(pkDfKey, pkDvKey))
                                                                {
                                                                    pSensor->m_pClientMap[appId].fltClientChangeSensitivity[dwDkIdx] = CHANGE_SENSITIVITY_NOT_SET;

                                                                    hrError = S_OK;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }

                                                    if (FAILED(hrError))
                                                    {
                                                        hr = E_UNEXPECTED;
                                                        Trace(TRACE_LEVEL_ERROR, "pkDvKey not found or pkDfVal invalid arg, hr = %!HRESULT!", hr);

                                                        hr = (*ppResults)->SetErrorValue(Key, E_INVALIDARG);
                                                        fHasError = TRUE;
                                                    }
                                                }
                                            }

                                            if (SUCCEEDED(hr))
                                            {
                                                hr = SelectClientChangeSensitivity(pSensor);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        hr = E_UNEXPECTED;
                                        Trace(TRACE_LEVEL_ERROR, "var.vt is not == VT_UNKNOWN, hr = %!HRESULT!", hr);

                                        hr = (*ppResults)->SetErrorValue(Key, E_INVALIDARG);
                                        fHasError = TRUE;
                                    }
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;
                                Trace(TRACE_LEVEL_ERROR, "Property not found, hr = %!HRESULT!", hr);

                                hr = (*ppResults)->SetErrorValue(Key, HRESULT_FROM_WIN32(ERROR_NOT_FOUND));
                                fHasError = TRUE;
                            }
                        }

                        PropVariantClear(&var);
                
                        if( FAILED( hr ) )
                        {
                            break;
                        }
                    }
                }

                // Since we have set failures for the property set operations we must let the application
                // know by returning S_FALSE. This will instruct the application to look at the
                // property set operation results for failure values.
                if( (hr == S_OK) && (TRUE == fHasError) )
                {
                    hr = S_FALSE;
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failed to get pSensor, hr = %!HRESULT!", hr);
            }

            //return hr;
        }

        // If it returns S_OK, Send the values down to the device
        // S_FALSE return indicates that the property is not supported
        if( S_OK == hr )
        {
            hr = UpdateSensorPropertyValues(ObjectID, TRUE);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    Trace(TRACE_LEVEL_VERBOSE, "Properties on sensor %ls were set for Client %p, hr = %!HRESULT!", ObjectID, appId, hr);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnSetRadioState
//
// Routine Description:
//  This method is called for Radio Management IO to set the radio state
//
// Arguments:
//  pRequest - The IO request
//  fPreviousState - If the previous radio state should be set instead
//                   of the current state.
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::OnSetRadioState(
        _In_ IWDFIoRequest* pRequest,
        _In_ bool fPreviousState
        )
{
    HRESULT hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

#if (NTDDI_VERSION >= NTDDI_WIN8)
    if (SUCCEEDED(hr))
    {
        CComPtr<IWDFMemory> spInputMemory;
        pRequest->GetInputMemory(&spInputMemory);
        SIZE_T  cbBufferIn  = 0;
        void* pInBuffer = spInputMemory->GetDataBuffer(&cbBufferIn);

        if (nullptr != pInBuffer &&
            sizeof(DEVICE_RADIO_STATE) == cbBufferIn &&
            *((DEVICE_RADIO_STATE*)pInBuffer) <= DRS_RADIO_MAX
            )
        {
            CSensor* pSensor = nullptr;
            hr = GetGeolocationSensorObject(&pSensor);
            if (SUCCEEDED(hr) && nullptr != pSensor)
            {
                if (fPreviousState)
                {
                    pSensor->m_ulPreviousGeolocationRadioState = *((DEVICE_RADIO_STATE*)pInBuffer);
                }
                else
                {
                    pSensor->m_ulRequiredGeolocationRadioState = *((DEVICE_RADIO_STATE*)pInBuffer);
                }

                Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Radio state set to = %d", *((DEVICE_RADIO_STATE*)pInBuffer));

                hr = UpdateSensorPropertyValues(pSensor->GetSensorObjectID(), TRUE);
            }
        }
        else
        {
            hr = E_INVALIDARG;
        }
    }
#else
    UNREFERENCED_PARAMETER(pRequest);
    UNREFERENCED_PARAMETER(fPreviousState);
#endif

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnClientConnect
//
// Routine Description:
//
//  This method is called by Sensor Class Extension when a client app connects
//  to a Sensor
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys 
//             are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnClientConnect(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{
CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSectionConnectDisconnect);


    HRESULT     hr      = S_OK;
    SensorType  sType   = SensorTypeNone;
    ULONG       sIndex  = 0;
    CSensor*    pSensor = nullptr;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        hr = FindSensorTypeFromObjectID(pwszObjectID, &sType, &sIndex);

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sIndex, &pSensor);
        }
        else
        {
            hr = E_UNEXPECTED;

            if (TRUE == wcscmp(L"DEVICE", pwszObjectID))
            {
                Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", pwszObjectID, hr);
            }
            else
            {
                //WPD is calling us, so ignore trace
            }
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! for Client %p to %s", pClientFile, pSensor->m_SensorName);

            BOOL fClientIsPresent = FALSE;

            hr = CheckForClient(pSensor, pClientFile, &fClientIsPresent);

            //do not allow client to connect more than one time
            //however, if a client is already connected
            //this must return S_OK so that the class extension
            //believes the request to connect has been honored
            if (SUCCEEDED(hr))
            {
                if (FALSE == fClientIsPresent)
                {
                    size_t cClients = 0;
                    CLIENT_ENTRY entry;

                    entry.ulClientReportInterval = 0; //default
                    entry.ulClientLocationDesiredAccuracy = LOCATION_DESIRED_ACCURACY_DEFAULT;
                    for (DWORD idx = 0; idx < MAX_NUM_DATA_FIELDS; idx++)
                    {
                        entry.fltClientChangeSensitivity[idx] = CHANGE_SENSITIVITY_NOT_SET;
                    }

                    pSensor->m_pClientMap[pClientFile] = entry;

                    cClients = pSensor->m_pClientMap.GetCount();

                    if (1 == cClients)
                    {
#if (NTDDI_VERSION >= NTDDI_WIN8)
                        if (DRS_RADIO_ON == pSensor->m_ulCurrentGeolocationRadioState)
                        {
#endif
                            Trace(TRACE_LEVEL_INFORMATION, "Calling StopIdle so the device will remain in D0 as long as there are connected clients and the radio is on");
                            pSensor->m_spWdfDevice2->StopIdle(FALSE);
#if (NTDDI_VERSION >= NTDDI_WIN8)
                        }
#endif
                    }

                    Trace(TRACE_LEVEL_INFORMATION, "Client %p connected to %s, Client count now = %i", pClientFile, pSensor->m_SensorName, (ULONG)cClients);

                    if (0 == cClients-1)
                    {
                        DWORD cDfKeys = 0;
                        pSensor->m_spSupportedSensorDataFields->GetCount(&cDfKeys);

                        pSensor->m_ulLowestClientReportInterval = pSensor->m_ulDefaultCurrentReportInterval;

                        for (DWORD dwIdx = 0; dwIdx < MAX_NUM_DATA_FIELDS; dwIdx++)
                        {
                            pSensor->m_fltLowestClientChangeSensitivities[dwIdx] = pSensor->m_fltDefaultChangeSensitivity;
                        }

                    }

                    if (SUCCEEDED(hr))
                    {
                        if(cClients >= 1)
                        {
                            hr = FindSensorTypeFromObjectID(pwszObjectID, &sType, &sIndex);

                            if (SUCCEEDED(hr))
                            {
                                hr = SelectClientReportInterval(pSensor);

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to update report interval, hr = %!HRESULT!", hr);
                                }
                                else
                                {
                                    hr = SelectClientLocationDesiredAccuracy(pSensor);
                                }

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to update location desired accuracy, hr = %!HRESULT!", hr);
                                }
                            }
                            else
                            {
                                hr = E_UNEXPECTED;

                                if (TRUE == wcscmp(L"DEVICE", pwszObjectID))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", pwszObjectID, hr);
                                }
                                else
                                {
                                    //WPD is calling us, so ignore trace
                                }
                            }

                            if (SUCCEEDED(hr))
                            {
                                hr = SelectClientChangeSensitivity(pSensor);

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to update change sensitivity, hr = %!HRESULT!", hr);
                                }
                            }

                            if (SUCCEEDED(hr) && 1 == cClients)
                            {
                                pSensor->m_ulPowerState = SENSOR_POWER_STATE_LOW_POWER;

                                hr = UpdateSensorPropertyValues(pwszObjectID, TRUE);
                            }

                            // Poll the sensors for initial data values
                            if(SUCCEEDED(hr))
                            {
                                hr = UpdateSensorDataFieldValues(m_pSensorManager->m_AvailableSensorsIDs[sIndex]);

                                if (FAILED(hr))
                                {
                                    Trace(TRACE_LEVEL_ERROR, "Failed to request input report, hr = %!HRESULT!", hr);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Trace(TRACE_LEVEL_ERROR, "Client is already connected, hr = %!HRESULT!", hr);
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failure while trying to connect client, hr = %!HRESULT!", hr);
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get pSensor, hr = %!HRESULT!", hr);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnClientDisconnect
//
// Routine Description:
//
//  This method is called by Sensor Class Extension when a client app disconnects
//  from a Sensor
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys 
//             are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnClientDisconnect(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{
CComCritSecLock<CComAutoCriticalSection> scopeLock(m_CriticalSectionConnectDisconnect);

    HRESULT     hr      = S_OK;
    SensorType  sType   = SensorTypeNone;
    ULONG       sIndex  = 0;
    CSensor*    pSensor = nullptr;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        CComPtr<IPortableDeviceValues> spReportInterval = NULL;
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                              NULL,
                              CLSCTX_INPROC_SERVER,
                              IID_PPV_ARGS(&spReportInterval));

        if (SUCCEEDED(hr))
        {
            hr = FindSensorTypeFromObjectID(pwszObjectID, &sType, &sIndex);
        }

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sIndex, &pSensor);
        }
        else
        {
            hr = E_UNEXPECTED;

            if (TRUE == wcscmp(L"DEVICE", pwszObjectID))
            {
                Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", pwszObjectID, hr);
            }
            else
            {
                //WPD is calling us, so ignore trace
            }
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! for Client %p from %s", pClientFile, pSensor->m_SensorName);

            BOOL fClientIsPresent = FALSE;
            size_t cClients = pSensor->m_pClientMap.GetCount();
                        
            hr = CheckForClient( pSensor, pClientFile, &fClientIsPresent);

            if (SUCCEEDED(hr) && (TRUE == fClientIsPresent))
            {
                if (SUCCEEDED(hr))
                {
                    if (cClients > 0)
                    {
                        //find this client in the list of clients and remove from the three client lists
                        hr = RemoveClient( pSensor, pClientFile );

                        if (SUCCEEDED(hr))
                        {
                            hr = CheckForClient( pSensor, pClientFile, &fClientIsPresent);

                            if (TRUE == fClientIsPresent)
                            {
                                Trace(TRACE_LEVEL_ERROR, "Failed to remove client %p from %s, hr = %!HRESULT!", pClientFile, pSensor->m_SensorName, hr);
                            }
                            else
                            {
                                Trace(TRACE_LEVEL_INFORMATION, "Client %p successfully removed from %s, Client Count now = %i, hr = %!HRESULT!", pClientFile, pSensor->m_SensorName, (DWORD)cClients-1, hr);
                            }
                        }
                        else
                        {
                            Trace(TRACE_LEVEL_ERROR, "Client %p has already been removed from %s, hr = %!HRESULT!", pClientFile, pSensor->m_SensorName, hr);
                        }

                        cClients = pSensor->m_pClientMap.GetCount();
                        
                        if (SUCCEEDED(hr))
                        {
                            hr = SelectClientReportInterval(pSensor);
                        }

                        if (SUCCEEDED(hr))
                        {
                            hr = SelectClientLocationDesiredAccuracy(pSensor);
                        }

                        if (SUCCEEDED(hr))
                        {
                            hr = SelectClientChangeSensitivity(pSensor);
                        }

                        if (SUCCEEDED(hr))
                        {
                            if (0 == cClients)
                            {
                                pSensor->m_fReportingState = FALSE;
                                pSensor->m_ulPowerState = SENSOR_POWER_STATE_POWER_OFF;

#if (NTDDI_VERSION >= NTDDI_WIN8)
                                if (DRS_RADIO_ON == pSensor->m_ulCurrentGeolocationRadioState)
                                {
#endif
                                    Trace(TRACE_LEVEL_INFORMATION, "Calling ResumeIdle so the device will remain in Dx as long as there are no connected clients or the radio is off");
                                    pSensor->m_spWdfDevice2->ResumeIdle();
#if (NTDDI_VERSION >= NTDDI_WIN8)
                                }
#endif

                                Trace(TRACE_LEVEL_INFORMATION, "All Clients have been removed from %s, hr = %!HRESULT!", pSensor->m_SensorName, hr);
                            }

                            hr = UpdateSensorPropertyValues(pwszObjectID, TRUE);
                        }
                    }
                    else
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Attempt to reduce client count below 0 on %s, AppID %p", pSensor->m_SensorName, pClientFile);
                    }
                }

                if (SUCCEEDED(hr) && (NULL != m_pSensorManager))
                {
                    size_t cSensors = m_pSensorManager->m_pSensorList.GetCount();
                    BOOL fHasClients = FALSE;
                    cClients = 0;

                    // see if any sensor has at least one client
                    for (sIndex = 0; sIndex < cSensors; sIndex++)
                    {
                        pSensor = nullptr;

                        hr = GetSensorObject(sIndex, &pSensor);

                        if (SUCCEEDED(hr) && (nullptr != pSensor))
                        {
                            cClients = pSensor->m_pClientMap.GetCount();

                            if (0 != cClients)
                            {
                                fHasClients = TRUE;
                            }
                        }
                        else
                        {
                            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
                        }
                    }

                    // if no clients, then allow for device disconnect
                    if (SUCCEEDED(hr) && (FALSE == fHasClients))
                    {
                        DeInitSensorDevice();
                    }
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Attempt to remove non-existent client, AppID* %u, Client Count %u", (DWORD)pClientFile, (ULONG)cClients);
            }
        }
        else
        {
            hr = E_POINTER;
            Trace(TRACE_LEVEL_ERROR, "Sensor not found, hr = %!HRESULT!", hr);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnClientSubscribeToEvents
//
// Routine Description:
//
//  This method is called by Sensor Class Extension when a client subscribes to
//  events by calling SetEventSink
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys 
//             are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnClientSubscribeToEvents(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{
    UNREFERENCED_PARAMETER(pClientFile);
    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        // Set the Client Status : denotes the status of event subscribed clients
        CSensor* pSensor = nullptr;
        SensorType sType;
        DWORD sIndex;

        hr = FindSensorTypeFromObjectID(pwszObjectID, &sType, &sIndex);

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sIndex, &pSensor);
        }
        else
        {
            hr = E_UNEXPECTED;

            if (TRUE == wcscmp(L"DEVICE", pwszObjectID))
            {
                Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", pwszObjectID, hr);
            }
            else
            {
                //WPD is calling us, so ignore trace
            }
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! for Client %p to %s", pClientFile, pSensor->m_SensorName);

            hr = pSensor->Subscribe(pClientFile);

        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
        }

        if(SUCCEEDED(hr) && (nullptr != pSensor))
        {
            size_t cSubscriberCount = pSensor->m_pSubscriberMap.GetCount();

            Trace(TRACE_LEVEL_INFORMATION, "Client %p Subscribed to %s, Subscriber Count = %i", pClientFile, pSensor->m_SensorName, (DWORD)cSubscriberCount);

            if (cSubscriberCount == 1)
            {
                pSensor->m_fReportingState = TRUE;
                pSensor->m_ulPowerState = SENSOR_POWER_STATE_FULL_POWER;

                pSensor->m_ulEventCount = 0;

                hr = UpdateSensorPropertyValues(pwszObjectID, TRUE);

                if (FAILED(hr))
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed to update property values, hr = %!HRESULT!", hr);
                }
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to subscribe to sensor events, hr = %!HRESULT!", hr);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnClientUnsubscribeFromEvents
//
// Routine Description:
//
//  This method is called by Sensor Class Extension when a client unsubscribes from
//  events by calling SetEventSink(NULL)
//  
// Arguments:
//
//  ObjectID - String that represents the object whose supported property keys 
//             are being requested
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnClientUnsubscribeFromEvents(
        _In_ IWDFFile* pClientFile,
        _In_ LPWSTR pwszObjectID
        )
{

    UNREFERENCED_PARAMETER(pClientFile);
    HRESULT hr = S_OK;

    hr = EnterProcessing(PROCESSING_ISENSORDRIVER);

    if (SUCCEEDED(hr))
    {
        // Set the Client Status : denotes the status of event subscribed clients
        CSensor* pSensor = nullptr;
        SensorType sType;
        DWORD sIndex;

        hr = FindSensorTypeFromObjectID(pwszObjectID, &sType, &sIndex);

        if (SUCCEEDED(hr))
        {
            hr = GetSensorObject(sIndex, &pSensor);
        }
        else
        {
            hr = E_UNEXPECTED;

            if (TRUE == wcscmp(L"DEVICE", pwszObjectID))
            {
                Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", pwszObjectID, hr);
            }
            else
            {
                //WPD is calling us, so ignore trace
            }
        }

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! for Client %p from %s", pClientFile, pSensor->m_SensorName);

            hr = pSensor->Unsubscribe(pClientFile);

        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
        }

        if(SUCCEEDED(hr))
        {
            size_t cSubscriberCount = pSensor->m_pSubscriberMap.GetCount();

            Trace(TRACE_LEVEL_INFORMATION, "Subscriber %p Removed from %s, Subscriber Count = %i", pClientFile, pSensor->m_SensorName, (DWORD)cSubscriberCount);


            if (0 == cSubscriberCount)
            {
                Trace(TRACE_LEVEL_INFORMATION, "All Subscribers removed from %s, Subscriber Count = %i", pSensor->m_SensorName, (DWORD)cSubscriberCount);
                pSensor->m_fReportingState = FALSE;
                pSensor->m_ulPowerState = SENSOR_POWER_STATE_LOW_POWER;

                hr = UpdateSensorPropertyValues(pwszObjectID, TRUE);
            }

            if (FAILED(hr))
            {
                Trace(TRACE_LEVEL_ERROR, "Failed to update property values, hr = %!HRESULT!", hr);
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failure during attempt to unsubscribe from events, hr = %!HRESULT!", hr);
        }

    } // processing in progress

    ExitProcessing(PROCESSING_ISENSORDRIVER);

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
// CSensorDDI::OnProcessWpdMessage
//
// Routine Description:
//
//  
// Arguments:
//
// 
//
// Return Value:
//  Status
// 
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI:: OnProcessWpdMessage(
        _In_ IUnknown* pPortableDeviceValuesParamsUnknown,
        _In_ IUnknown* pPortableDeviceValuesResultsUnknown
        )
{
    UNREFERENCED_PARAMETER(pPortableDeviceValuesParamsUnknown);
    UNREFERENCED_PARAMETER(pPortableDeviceValuesResultsUnknown);

    HRESULT hr = S_OK;

    return hr;
}

/////////////////////////////////////////////////////////////////////////
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
//  The driver should:
//  - Return all requested property values in WPD_PROPERTY_OBJECT_PROPERTIES_PROPERTY_VALUES.  If any property read failed, the corresponding value should be
//      set to type VT_ERROR with the 'scode' member holding the HRESULT reason for the failure.
//  - S_OK should be returned if all properties were read successfully.
//  - S_FALSE should be returned if any property read failed.
//  - Any error return indicates that the driver did not fill in any results, and the caller will
//      not attempt to unpack any property values.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::OnGetPropertyValues(
        _In_ IWDFFile* appId,
        _In_ LPWSTR  wszObjectID,
        _In_ IPortableDeviceKeyCollection* pKeys,
        _Out_ IPortableDeviceValues** ppValues )
{
    HRESULT hr          = S_OK;


    // CoCreate a collection to store the property values.
    hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                NULL,
                                CLSCTX_INPROC_SERVER,
                                IID_PPV_ARGS(ppValues));

    // Read the specified properties on the specified object and add the property values to the collection.
    if (hr == S_OK && ppValues != NULL)
    {
        DWORD       cKeys       = 0;
        BOOL        fError      = FALSE;
        IPortableDeviceValues*  pValues = *ppValues; //Fakes this call: hr = GetPropertyValuesForObject(wszObjectID, pKeys, *ppValues);

        CSensor*            pSensor = NULL;  
        SensorType          sensType = SensorTypeNone;
        DWORD               sensIndex = 0;

        CGeolocation*       pGeolocation = NULL;

        if ((wszObjectID == NULL) ||
            (pKeys       == NULL) ||
            (pValues     == NULL))
        {
            hr = E_INVALIDARG;
        }

        if (SUCCEEDED(hr))
        {
            hr = pKeys->GetCount(&cKeys);

            if (hr == S_OK)
            {
                hr = FindSensorTypeFromObjectID((LPWSTR)wszObjectID, &sensType, &sensIndex);

                if (SUCCEEDED(hr))
                {
                    hr = GetSensorObject(sensIndex, &pSensor);

                    if ((NULL != pSensor) && (SUCCEEDED(hr)))
                    {
                        //depending on where OnGetPropertyValues() is being called from, appId may not be valid
                        if (nullptr != appId)
                        {
                            Trace(TRACE_LEVEL_INFORMATION, "Client %p Requesting Property Values for %s", appId, pSensor->m_SensorName);
                        }
                        else
                        {
                            Trace(TRACE_LEVEL_VERBOSE, "Sensor %ls is being asked for Property Values for %s", wszObjectID, pSensor->m_SensorName);
                        }

                        switch (sensType)
                        {
                        case Geolocation:
                            pGeolocation = (CGeolocation*) pSensor;
                            hr = pGeolocation->GetPropertyValuesForGeolocationObject(wszObjectID, pKeys, pValues);
                            break;

                        default:
                            break;
                        }
                    }
                    else
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
                    }
                }
                else
                {
                    hr = E_UNEXPECTED;

                    if (TRUE == wcscmp(L"DEVICE", wszObjectID))
                    {
                        Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", wszObjectID, hr);
                    }
                    else
                    {
                        //WPD is calling us, so ignore trace
                    }
                }

            }
        }

        hr = (FALSE == fError) ? hr : S_FALSE;

        *ppValues = pValues; //Fakes return from this call: hr = GetPropertyValuesForObject(wszObjectID, pKeys, *ppValues);

    }


    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to copy PROPERTYKEYs from one collection to the other.
//
//  The parameters sent to us are:
//  pSourceKeys - The source collection
//  pTargetKeys - The target collection
//
//  The driver should:
//  Copy PROPERTYKEYs from the source collection to the target.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::CopyPropertyKeys(
    _In_ IPortableDeviceKeyCollection *pSourceKeys,
    _In_ IPortableDeviceKeyCollection *pTargetKeys)
{
    HRESULT hr = S_OK;
    DWORD cKeys = 0;
    PROPERTYKEY key = {0};

    if (NULL != pSourceKeys && NULL != pTargetKeys)
    {
        hr = pSourceKeys->GetCount(&cKeys);
        
        if (SUCCEEDED(hr))
        {
            for (DWORD dwIndex = 0; dwIndex < cKeys; ++dwIndex)
            {
                hr = pSourceKeys->GetAt(dwIndex, &key);
                
                if (SUCCEEDED(hr))
                {
                    hr = pTargetKeys->Add(key);
                }
            }
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to determine the client index and
//  the client-desired settings for settable properties.
//
//  The parameters sent to us are:
//
//  The driver should:
//  Return the client index, the client change sensitivity and the client report interval.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::FindClientFromAppID(_In_ CSensor* pSensor,
                               _In_ IWDFFile* appID, 
                               _Out_ ULONG* pClientReportInterval, 
                               _Out_ ULONG* pClientLocationDesiredAccuracy,
                               _Inout_updates_(MAX_NUM_DATA_FIELDS) FLOAT* pClientChangeSensitivities)
{
    HRESULT     hr = E_POINTER;

    if ( (nullptr != pSensor) &&
         (nullptr != pClientReportInterval) &&
         (nullptr != pClientChangeSensitivities) 
         )
    {
        DWORD cDfKeys = 0; 

        hr = pSensor->m_spSupportedSensorDataFields->GetCount(&cDfKeys);

        if (SUCCEEDED(hr))
        {
            hr = HRESULT_FROM_WIN32(ERROR_NOT_FOUND);

            if (cDfKeys > 0)
            {
                CLIENT_ENTRY entry;

                if (pSensor->m_pClientMap.Lookup(appID, entry) == FALSE)
                {
                    Trace(TRACE_LEVEL_ERROR, "Failed to find client %p in %s, hr = %!HRESULT!", appID, pSensor->m_SensorName, hr);
                }
                else
                {
                    *pClientReportInterval = pSensor->m_pClientMap[appID].ulClientReportInterval;
                    *pClientLocationDesiredAccuracy = pSensor->m_pClientMap[appID].ulClientLocationDesiredAccuracy;
                    for (DWORD dwDataField = 0; dwDataField < MAX_NUM_DATA_FIELDS; dwDataField++)
                    {
                        pClientChangeSensitivities[dwDataField] = pSensor->m_pClientMap[appID].fltClientChangeSensitivity[dwDataField];
                    }

                    hr = S_OK;
                }
            }
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to check for a connected client.
//
//  The parameters sent to us are:
//
//  The driver should:
//  Return a BOOL indicating whether this client is connected.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::CheckForClient(_In_ CSensor* pSensor, _In_ IWDFFile* appID, _Out_ BOOL* pfClientPresent )
{
    HRESULT hr = S_OK;
    
    *pfClientPresent = FALSE;

    if (nullptr != pSensor)
    {
        CLIENT_ENTRY entry; 

        if (pSensor->m_pClientMap.Lookup(appID, entry) == FALSE)
        {
            //Trace(TRACE_LEVEL_INFORMATION, "Client %p was not found in the %s client list, %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
        else
        {
            *pfClientPresent = TRUE;
            //Trace(TRACE_LEVEL_INFORMATION, "Client %p was found in the %s client list, %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to remove a connected client.
//
//  The parameters sent to us are:
//
//  The driver should:
//  Return the client index.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::RemoveClient(_In_ CSensor* pSensor, _In_ IWDFFile* appID )
{
    HRESULT hr = S_OK;
    
    if (nullptr != pSensor)
    {

        if (pSensor->m_pClientMap.RemoveKey(appID) == FALSE)
        {
            //Entry is not in client list or could not be removed
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Client %p not found in the %s client list, hr = %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to check for a subscribed client.
//
//  The parameters sent to us are:
//
//  The driver should:
//  Return a BOOL indicating whether this client is subscribed.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::CheckForSubscriber(_In_ CSensor* pSensor, _In_ IWDFFile* appID, _Out_ BOOL* pfClientIsSubscribed )
{
    HRESULT hr = S_OK;
    
    *pfClientIsSubscribed = FALSE;

    if (nullptr != pSensor)
    {
        SUBSCRIBER_ENTRY entry; 

        if (pSensor->m_pSubscriberMap.Lookup(appID, entry) == FALSE)
        {
            //Trace(TRACE_LEVEL_INFORMATION, "Client %p was not found in the %s subscriber list, %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
        else
        {
            *pfClientIsSubscribed = pSensor->m_pSubscriberMap[appID].fSubscribed;
            //Trace(TRACE_LEVEL_INFORMATION, "Client %p was found in the %s subscriber list, %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to remove a subscribed client.
//
//  The parameters sent to us are:
//
//  The driver should:
//  Return the client index
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::RemoveSubscriber(_In_ CSensor* pSensor, _In_ IWDFFile* appID )
{
    HRESULT hr = S_OK;
    
    if (nullptr != pSensor)
    {

        if (pSensor->m_pSubscriberMap.RemoveKey(appID) == FALSE)
        {
            //Entry is not in client list or could not be removed
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Client %p not found in the %s subscriber list, hr = %!HRESULT!", appID, pSensor->m_SensorName, hr);
        }
    }
    else
    {
        hr = E_POINTER;
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to determine the sensor type and
//  index from the object ID.
//
//  The parameters sent to us are:
//  Key - A PROPERTYKEY
//
//  The driver should:
//  Return the sensor type and the sensor index.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::FindSensorTypeFromObjectID(_In_ LPWSTR pwszObjectID, 
                                   _Out_ SensorType* pSensorType,
                                   _Out_ DWORD* pSensorIdx)
{
    HRESULT hr = E_POINTER;
    CAtlStringW strObjectID = pwszObjectID;
    CComBSTR objectId;

    if ( (nullptr != pSensorType) &&
         (nullptr != pSensorIdx) )
    {
        if (nullptr != m_pSensorManager)
        {
            DWORD cSensorIDs = (DWORD)m_pSensorManager->m_AvailableSensorsIDs.GetCount();
            DWORD cSensorTypes = (DWORD)m_pSensorManager->m_AvailableSensorsTypes.GetCount();

            if ((cSensorIDs < 1) || (cSensorTypes < 1))
            {
                hr = E_UNEXPECTED;
            }
            else
            {
                hr = HRESULT_FROM_WIN32(ERROR_NOT_FOUND);

                POSITION posID = m_pSensorManager->m_AvailableSensorsIDs.GetStartPosition();
                POSITION posType = m_pSensorManager->m_AvailableSensorsTypes.GetStartPosition();
                SensorType sensType = SensorTypeNone;
                DWORD idx = 0;

                while(NULL != posID)
                {
                    objectId = m_pSensorManager->m_AvailableSensorsIDs.GetNextValue(posID);
                    sensType = m_pSensorManager->m_AvailableSensorsTypes.GetNextValue(posType);

                    if (strObjectID.CompareNoCase(objectId) == 0)
                    {
                        *pSensorType = sensType;
                        *pSensorIdx = idx;
                        hr = S_OK;

                        break;
                    }

                    idx++;
                }

                if (SUCCEEDED(hr))
                {
                    if (sensType == SensorTypeNone)
                    {
                        hr = E_UNEXPECTED;
                    }
                }
            }
        }
        else
        {
            hr = E_UNEXPECTED;
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to determine if the given PROPERTYKEY is a settable
//  property of the SENSOR object.
//
//  The parameters sent to us are:
//  Key - A PROPERTYKEY
//
//  The driver should:
//  Return TRUE if the PROPERTYKEY is supported; FALSE otherwise.
//
/////////////////////////////////////////////////////////////////////////
BOOL CSensorDDI::IsSettablePropertyKey(_In_ DWORD sIndex, _In_ PROPERTYKEY Key)
{
    HRESULT hr = S_OK;
    CSensor*   pSensor = nullptr;

    CComPtr<IPortableDeviceKeyCollection> spSensorProperties;
    
    BOOL fResult = FALSE;

    hr = GetSensorObject(sIndex, &pSensor);

    if (SUCCEEDED(hr) && (nullptr != pSensor))
    {
        pSensor->GetSettableProperties(&spSensorProperties);
    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
    }

    if (SUCCEEDED(hr))
    {
        fResult = FindPropertyKey(Key, spSensorProperties);
    }

    return fResult;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to determine if the given PROPERTYKEY can be
//  found in the given collection
//
//  The parameters sent to us are:
//  Key - A PROPERTYKEY
//  pKeys - The collection to search in.
//
//  The driver should:
//  Return TRUE if the PROPERTYKEY is found; FALSE otherwise.
//
/////////////////////////////////////////////////////////////////////////
BOOL CSensorDDI::FindPropertyKey(_In_ PROPERTYKEY Key, _In_ IPortableDeviceKeyCollection *pKeys)
{
    HRESULT hr = S_OK;

    DWORD cKeys = 0;
    PROPERTYKEY tempKey;
    BOOL fResult = FALSE;

    if (NULL != pKeys)
    {
        hr = pKeys->GetCount(&cKeys);
        
        if (SUCCEEDED(hr))
        {
            for (DWORD dwIndex = 0; dwIndex < cKeys; ++dwIndex)
            {
                hr = pKeys->GetAt(dwIndex, &tempKey);
                
                if (SUCCEEDED(hr))
                {
                    if (IsEqualPropertyKey(tempKey, Key))
                    {
                        fResult = TRUE;
                        break;
                    }
                }
            }
        }
    }

    return fResult;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate common PROPERTYKEYs found on ALL objects.
//
//  The parameters sent to us are:
//  pKeys - An IPortableDeviceKeyCollection to be populated with PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the ALL objects.
//
/////////////////////////////////////////////////////////////////////////
VOID CSensorDDI::AddCommonPropertyKeys(
    _In_ IPortableDeviceKeyCollection* pKeys)
{
    if (pKeys != NULL)
    {
        for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_SupportedCommonProperties); dwIndex++)
        {
            pKeys->Add(g_SupportedCommonProperties[dwIndex] );
        }
    }
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate common PROPERTYKEYs found on the DEVICE object.
//
//  The parameters sent to us are:
//  pKeys - An IPortableDeviceKeyCollection to be populated with PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the DEVICE object.
//
/////////////////////////////////////////////////////////////////////////
VOID CSensorDDI::AddDevicePropertyKeys(
    _In_ IPortableDeviceKeyCollection* pKeys)
{
    if (pKeys != NULL)
    {
        for (DWORD dwIndex = 0; dwIndex < ARRAYSIZE(g_SupportedDeviceProperties); dwIndex++)
        {
            pKeys->Add(g_SupportedDeviceProperties[dwIndex] );
        }
    }
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate common PROPERTYKEYs found on sensor objects.
//
//  The parameters sent to us are:
//  pKeys - An IPortableDeviceKeyCollection to be populated with PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the sensor objects.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::AddSensorPropertyKeys(
    _In_ SensorType sensType,
    _In_ DWORD sensIndex,
    _In_ IPortableDeviceKeyCollection* pKeys)
{
    UNREFERENCED_PARAMETER(sensType);

    HRESULT hr = S_OK;

    CComPtr<IPortableDeviceKeyCollection> spSensorProperties;
    
    if (NULL != pKeys)
    {
        // Add the supported properties
        CSensor* pSensor = nullptr;

        hr = GetSensorObject(sensIndex, &pSensor);

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            hr = pSensor->GetSupportedProperties(&spSensorProperties);
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
        }

        if (SUCCEEDED(hr))
        {
            hr = CopyPropertyKeys(spSensorProperties, pKeys);
        }
    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "pKeys or pSensorManager is null, hr = %!HRESULT!", hr);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate common PROPERTYKEYs found on sensor objects.
//
//  The parameters sent to us are:
//  pKeys - An IPortableDeviceKeyCollection to be populated with PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the sensor objects.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::AddSensorDataFieldKeys(
       _In_ SensorType sensType,
       _In_ DWORD sensIndex,
       _In_ IPortableDeviceKeyCollection* pKeys)
{    
    UNREFERENCED_PARAMETER(sensType);

    HRESULT hr = S_OK;

    CComPtr<IPortableDeviceKeyCollection> spSensorDataFields;

    if (NULL != pKeys)
    {
        // Add the supported datafields
        CSensor* pSensor = nullptr;

        hr = GetSensorObject(sensIndex, &pSensor);

        if (SUCCEEDED(hr) && (nullptr != pSensor))
        {
            hr = pSensor->GetSupportedDataFields(&spSensorDataFields);
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "Failed to get sensor object, hr = %!HRESULT!", hr);
        }
        
        if (SUCCEEDED(hr))
        {
            hr = CopyPropertyKeys(spSensorDataFields, pKeys);
        }
    }
    else
    {
        hr = E_UNEXPECTED;
        Trace(TRACE_LEVEL_ERROR, "pKeys or pSensorManager is null, hr = %!HRESULT!", hr);
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate supported PROPERTYKEYs found on objects.
//
//  The parameters sent to us are:
//  wszObjectID - the object whose supported property keys are being requested
//  pKeys - An IPortableDeviceKeyCollection to be populated with supported PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the specified object.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::AddSupportedPropertyKeys(
    _In_ LPWSTR                        wszObjectID,
    _In_ IPortableDeviceKeyCollection*  pKeys)
{
    HRESULT hr = S_OK;
    CAtlStringW strObjectID = wszObjectID;
  
    // Add Common PROPERTYKEYs for ALL WPD objects
    AddCommonPropertyKeys(pKeys);

    if (strObjectID.CompareNoCase(WPD_DEVICE_OBJECT_ID) == 0)
    {
        // Add the PROPERTYKEYs for the 'DEVICE' object
        AddDevicePropertyKeys(pKeys);
    }

    // Add the PROPERTYKEYs for the 'SENSOR' object
    SensorType sType;
    DWORD sIndex;

    hr = FindSensorTypeFromObjectID(wszObjectID, &sType, &sIndex);

    if(SUCCEEDED(hr))
    {
        hr = AddSensorPropertyKeys(sType, sIndex, pKeys);
    }
    else
    {
        hr = E_UNEXPECTED;

        if (TRUE == wcscmp(L"DEVICE", wszObjectID))
        {
            Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", wszObjectID, hr);
        }
        else
        {
            //WPD is calling us, so ignore trace
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to populate supported PROPERTYKEYs found on data objects.
//
//  The parameters sent to us are:
//  wszObjectID - the object whose supported property keys are being requested
//  pKeys - An IPortableDeviceKeyCollection to be populated with supported PROPERTYKEYs
//
//  The driver should:
//  Add PROPERTYKEYs pertaining to the specified object.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::AddSupportedDataFieldKeys(
    _In_  LPWSTR                        wszObjectID,
    _In_ IPortableDeviceKeyCollection*  pKeys)
{
    HRESULT     hr          = S_OK;
    CAtlStringW strObjectID = wszObjectID;

    // Add the DATAFIELD keys for the 'SENSOR' object
    SensorType sType;
    DWORD sIndex;

    hr = FindSensorTypeFromObjectID(wszObjectID, &sType, &sIndex);

    if(SUCCEEDED(hr))
    {
        hr = AddSensorDataFieldKeys(sType, sIndex, pKeys);
    }
    else
    {
        hr = E_UNEXPECTED;

        if (TRUE == wcscmp(L"DEVICE", wszObjectID))
        {
            Trace(TRACE_LEVEL_ERROR, "Device is %ws not a supported sensor, hr = %!HRESULT!", wszObjectID, hr);
        }
        else
        {
            //WPD is calling us, so ignore trace
        }
    }
    
    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to update choose a client Report Interval.
//
//  The parameters sent to us are:
//  pSensor - the sensor to use
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::SelectClientReportInterval(
        _In_ CSensor* pSensor)
{
    HRESULT hr = S_OK;

    if (pSensor == NULL)
    {
        hr = E_INVALIDARG;
        return hr;
    }

    if (SUCCEEDED(hr))
    {
        ULONG shortestReportInterval = UINT_MAX;
        ULONG clientReportInterval = 0;
        size_t cClients = pSensor->m_pClientMap.GetCount();
        IWDFFile* pClient = nullptr;
        POSITION posClient = nullptr;
        CLIENT_ENTRY entry;
        BOOL fReportIntervalChosen = FALSE;

        if (cClients == 0)
        {
            pSensor->m_fReportingState = FALSE;
            pSensor->m_ulPowerState = SENSOR_POWER_STATE_POWER_OFF;

            pSensor->m_ulLowestClientReportInterval = pSensor->m_ulDefaultCurrentReportInterval;
        }
        else
        {
            posClient = pSensor->m_pClientMap.GetStartPosition();

            //find the lowest value in the client report interval list
            while (posClient != NULL)
            {
                pClient = pSensor->m_pClientMap.GetKeyAt(posClient);
                pSensor->m_pClientMap.Lookup(pClient, entry);
                clientReportInterval = pSensor->m_pClientMap[pClient].ulClientReportInterval;
                if (0 == clientReportInterval)
                {
                    //exclude this client from report interval calculations;
                }
                else if ((clientReportInterval > 0) && (clientReportInterval < shortestReportInterval))
                {
                    fReportIntervalChosen = TRUE;
                    shortestReportInterval = clientReportInterval;
                }

                pSensor->m_pClientMap.GetNextKey(posClient);
            }

            //if no clients have specified a report interval, then use the default
            if (UINT_MAX == shortestReportInterval)
            {
                shortestReportInterval = pSensor->m_ulDefaultCurrentReportInterval;
            }

            //range check to be sure is not less than Min Report Interval
            if (shortestReportInterval < pSensor->m_ulDefaultMinimumReportInterval)
            {
                shortestReportInterval = pSensor->m_ulDefaultMinimumReportInterval;
            }

            pSensor->m_ulLowestClientReportInterval = shortestReportInterval;

            if (TRUE == fReportIntervalChosen)
            {
                pSensor->m_fReportingState = TRUE;
                pSensor->m_ulPowerState = SENSOR_POWER_STATE_FULL_POWER;
            }
            else
            {
                pSensor->m_fReportingState = FALSE;
                pSensor->m_ulPowerState = SENSOR_POWER_STATE_LOW_POWER;
            }

            pSensor->CheckLongReportIntervalTimer();
        }
    }

    return hr;
}


/////////////////////////////////////////////////////////////////////////
//
//  This method is called to update choose a client location desired accuracy.
//
//  The parameters sent to us are:
//  pSensor - the sensor to use
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::SelectClientLocationDesiredAccuracy(
        _In_ CSensor* pSensor)
{
    HRESULT hr = S_OK;

    if (pSensor == NULL)
    {
        hr = E_INVALIDARG;
        return hr;
    }

    if (SUCCEEDED(hr))
    {
        ULONG shortestLocationDesiredAccuracy = UINT_MAX;
        ULONG clientLocationDesiredAccuracy = 0;
        size_t cClients = pSensor->m_pClientMap.GetCount();
        IWDFFile* pClient = nullptr;
        POSITION posClient = nullptr;
        CLIENT_ENTRY entry;

        if (cClients == 0)
        {
            pSensor->m_ulLowestClientLocationDesiredAccuracy = LOCATION_DESIRED_ACCURACY_DEFAULT;
        }
        else
        {
            posClient = pSensor->m_pClientMap.GetStartPosition();

            //find the lowest value in the client report interval list
            while (posClient != NULL)
            {
                pClient = pSensor->m_pClientMap.GetKeyAt(posClient);
                pSensor->m_pClientMap.Lookup(pClient, entry);
                clientLocationDesiredAccuracy = pSensor->m_pClientMap[pClient].ulClientLocationDesiredAccuracy;
                if (0 == clientLocationDesiredAccuracy)
                {
                    //exclude this client from report interval calculations;
                }
                else if ((clientLocationDesiredAccuracy > 0) && (clientLocationDesiredAccuracy < shortestLocationDesiredAccuracy))
                {
                    shortestLocationDesiredAccuracy = clientLocationDesiredAccuracy;
                }

                pSensor->m_pClientMap.GetNextKey(posClient);
            }

            //if no clients have specified a location desired accuracy, then use the default
            if (UINT_MAX == shortestLocationDesiredAccuracy)
            {
                shortestLocationDesiredAccuracy = LOCATION_DESIRED_ACCURACY_DEFAULT;
            }

            //range check to be sure is less than Max Location Desired Accuracy
            if (shortestLocationDesiredAccuracy > LOCATION_DESIRED_ACCURACY_HIGH)
            {
                shortestLocationDesiredAccuracy = LOCATION_DESIRED_ACCURACY_DEFAULT;
            }

            pSensor->m_ulLowestClientLocationDesiredAccuracy = shortestLocationDesiredAccuracy;
        }
    }

    return hr;
}

/////////////////////////////////////////////////////////////////////////
//
//  This method is called to choose a client Change Sensitivity.
//
//  The parameters sent to us are:
//  pSensor - the sensor to use
//
//
/////////////////////////////////////////////////////////////////////////
HRESULT CSensorDDI::SelectClientChangeSensitivity(
        _In_ CSensor* pSensor)
{
    HRESULT hr = S_OK;

    if (pSensor == NULL)
    {
        hr = E_INVALIDARG;
        return hr;
    }

    // Update our cache
    if(SUCCEEDED(hr))
    {
        CComPtr<IPortableDeviceValues> spDfVals = nullptr;
        FLOAT clientChangeSensitivity = 0;
        FLOAT lowestChangeSensitivity = FLT_MAX;
        size_t cClients = pSensor->m_pClientMap.GetCount();
        IWDFFile* pClient = nullptr;
        POSITION posClient = nullptr;
        CLIENT_ENTRY entry;

        PROPERTYKEY pkDfKey;
        DWORD cDfKeys = 0;

        // CoCreate a collection to store the property set operation results.
        hr = CoCreateInstance(CLSID_PortableDeviceValues,
                                    NULL,
                                    CLSCTX_INPROC_SERVER,
                                    IID_PPV_ARGS(&spDfVals));

        if (SUCCEEDED(hr))
        {
            hr = pSensor->m_spSupportedSensorDataFields->GetCount(&cDfKeys);
        }

        if (SUCCEEDED(hr))
        {
            for (ULONG uDfIdx = 1; uDfIdx < cDfKeys; uDfIdx++) //ignore the timestamp, the first datafield
            {
                pSensor->m_spSupportedSensorDataFields->GetAt(uDfIdx, &pkDfKey);

                if (SUCCEEDED(hr))
                {
                    if (cClients == 0)
                    {
                        pSensor->m_fltLowestClientChangeSensitivities[uDfIdx] = pSensor->m_fltDefaultChangeSensitivity;
                    }
                    else
                    {
                        lowestChangeSensitivity = FLT_MAX;

                        posClient = pSensor->m_pClientMap.GetStartPosition();

                        //find the lowest value in the client change sensitivity list
                        while (posClient != NULL)
                        {
                            pClient = pSensor->m_pClientMap.GetKeyAt(posClient);
                            pSensor->m_pClientMap.Lookup(pClient, entry);
                        
                            if (cClients > 0)
                            {
                                clientChangeSensitivity = pSensor->m_pClientMap[pClient].fltClientChangeSensitivity[uDfIdx];

                                if ( clientChangeSensitivity < 0.0F )
                                {
                                    //exclude this client from change sensitivity calculations
                                }
                                else if (( clientChangeSensitivity >= 0.0F) && ( clientChangeSensitivity < lowestChangeSensitivity ))
                                {
                                    lowestChangeSensitivity = clientChangeSensitivity;
                                }
                            }

                            pSensor->m_pClientMap.GetNextKey(posClient);
                        }

                        //if no clients have a specified change sensitivity, then use the default
                        if (FLT_MAX == lowestChangeSensitivity)
                        {
                            lowestChangeSensitivity = pSensor->m_fltDefaultChangeSensitivity;
                        }

                        //range check to be sure is not less than 0.0F
                        if (lowestChangeSensitivity < 0.0F)
                        {
                            lowestChangeSensitivity = pSensor->m_fltDefaultChangeSensitivity;
                        }

                        pSensor->m_fltLowestClientChangeSensitivities[uDfIdx] = lowestChangeSensitivity;
                    }
                }
            }
        }
    }

    return hr;
}

HRESULT CSensorDDI::GetSensorObject(
        _In_ ULONG ulIndex, 
        _Out_ CSensor** ppSensor)
{
    HRESULT hr = S_OK;

    if (SUCCEEDED(hr))
    {
        if (nullptr != m_pSensorManager)
        {
            POSITION pos = NULL;

            pos = m_pSensorManager->m_pSensorList.FindIndex(ulIndex);

            if (NULL != pos)
            {
                *ppSensor = m_pSensorManager->m_pSensorList.GetAt(pos);

                if (nullptr == *ppSensor)
                {
                    hr = E_UNEXPECTED;
                    Trace(TRACE_LEVEL_ERROR, "Failed to get sensor, hr = %!HRESULT!", hr);
                }
            }
            else
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failed to find sensor position, hr = %!HRESULT!", hr);
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "pSensorManger is null, hr = %!HRESULT!", hr);
        }
    }

    return hr;
}

HRESULT CSensorDDI::GetGeolocationSensorObject(
        _Out_ CSensor** ppSensor)
{
    HRESULT hr = S_OK;
    *ppSensor = nullptr;

    if (SUCCEEDED(hr))
    {
        if (nullptr != m_pSensorManager)
        {
            for (DWORD i = 0; i < m_pSensorManager->m_pSensorList.GetCount(); i++)
            {
                POSITION pos = m_pSensorManager->m_pSensorList.FindIndex(i);
                if (nullptr != pos)
                {
                    CSensor* pSensorTemp = m_pSensorManager->m_pSensorList.GetAt(pos);

                    if (nullptr == pSensorTemp)
                    {
                        hr = E_UNEXPECTED;
                        Trace(TRACE_LEVEL_ERROR, "Failed to get sensor, hr = %!HRESULT!", hr);
                        break;
                    }
                    else
                    {
                        if (Geolocation == pSensorTemp->GetSensorType())
                        {
                            *ppSensor = pSensorTemp;
                        }
                    }
                }
                else
                {
                    hr = E_UNEXPECTED;
                    Trace(TRACE_LEVEL_ERROR, "pos is null, hr = %!HRESULT!", hr);
                    break;
                }
            }

            if (nullptr == *ppSensor)
            {
                hr = E_UNEXPECTED;
                Trace(TRACE_LEVEL_ERROR, "Failed to find sensor position, hr = %!HRESULT!", hr);
            }
        }
        else
        {
            hr = E_UNEXPECTED;
            Trace(TRACE_LEVEL_ERROR, "pSensorManger is null, hr = %!HRESULT!", hr);
        }
    }

    return hr;
}

inline HRESULT CSensorDDI::EnterProcessing(DWORD64 dwControlFlag)
{
    return m_pSensorManager->EnterProcessing(dwControlFlag);
}

inline void CSensorDDI::ExitProcessing(DWORD64 dwControlFlag)
{
    m_pSensorManager->ExitProcessing(dwControlFlag);
}

