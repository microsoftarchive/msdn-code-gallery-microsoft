/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    HealthHeartRateService.cpp
    
Abstract:

--*/

#include "stdafx.h"

#include "HealthHeartRateService.tmh"

// Supported events
EventAttributeInfo g_SupportedServiceEvents[] =
{
    {NULL, NULL},
};

// Event parameters
EventParameterAttributeInfo g_ServiceEventParameters[] =
{
    {NULL, NULL, 0},
};

// Supported Methods
MethodAttributeInfo g_SupportedServiceMethods[] = 
{
    {&METHOD_ReadHeartRateMeasurement, NAME_METHOD_ReadHeartRateMeasurement, WPD_COMMAND_ACCESS_READWRITE},
};

MethodParameterAttributeInfo g_ServiceMethodParameters[] = 
{
    {&METHOD_ReadHeartRateMeasurement, &METHOD_PARAMETER_HealthHeartRateService_Measurement_Result,                VT_BOOL,    WPD_PARAMETER_USAGE_RETURN, WPD_PARAMETER_ATTRIBUTE_FORM_UNSPECIFIED, 0, L"Result" },
    {&METHOD_ReadHeartRateMeasurement, &METHOD_PARAMETER_HealthHeartRateService_Measurement_TimeStamp,             VT_UI8,     WPD_PARAMETER_USAGE_OUT,    WPD_PARAMETER_ATTRIBUTE_FORM_UNSPECIFIED, 1, L"TimeStamp" },
    {&METHOD_ReadHeartRateMeasurement, &METHOD_PARAMETER_HealthHeartRateService_Measurement_Rate,                  VT_UI4,     WPD_PARAMETER_USAGE_OUT,    WPD_PARAMETER_ATTRIBUTE_FORM_UNSPECIFIED, 2, L"Rate" }, 
};

typedef struct _HEARTRATE_MEASUREMENT {
    LIST_ENTRY  ListEntry;
    
    ULONGLONG   TimeStamp;
    ULONG       Rate;
}HEARTRATE_MEASUREMENT;

HealthHeartRateService::HealthHeartRateService()
{

    RequestFilename =                   SERVICE_OBJECT_ID;
    m_pDevice =                         NULL;
    m_hEventSync =                      NULL;
    
    m_SupportedServiceEvents =          g_SupportedServiceEvents;
    m_SupportedServiceEventCount =      ARRAYSIZE(g_SupportedServiceEvents);
    m_ServiceEventParameters =          g_ServiceEventParameters;
    m_ServiceEventParameterCount =      ARRAYSIZE(g_ServiceEventParameters);
    
    m_SupportedServiceMethods =         g_SupportedServiceMethods;
    m_SupportedServiceMethodCount =     ARRAYSIZE(g_SupportedServiceMethods);
    m_ServiceMethodParameters =         g_ServiceMethodParameters;
    m_ServiceMethodParameterCount =     ARRAYSIZE(g_ServiceMethodParameters);  
}


HealthHeartRateService::~HealthHeartRateService()
{
    //
    // On destruction, we must drain the queued objects
    //    
    HEARTRATE_MEASUREMENT * pMeasurement = NULL;    
    
    EnterCriticalSection(&m_EventQueueCS);

    while(!IsListEmpty(&m_EventQueueHead))
    {
        PLIST_ENTRY ple = NULL;
        ple = RemoveHeadList(&m_EventQueueHead);
        pMeasurement = CONTAINING_RECORD(ple, HEARTRATE_MEASUREMENT, ListEntry);
        delete pMeasurement;
    }

    LeaveCriticalSection(&m_EventQueueCS);    
}


HRESULT HealthHeartRateService::OnMethodInvoke(
    _In_    REFGUID                Method,
    _In_    IPortableDeviceValues* pParams,
    _Out_   IPortableDeviceValues* pResults)
{
    HRESULT hr = S_OK;
    
    UNREFERENCED_PARAMETER(pParams);
    
    if (IsEqualGUID(METHOD_ReadHeartRateMeasurement, Method))
    {
        HEARTRATE_MEASUREMENT * pMeasurement = NULL;
        
        //
        // For simplicity of the implementation of this sample, all methods are invoked 
        // on a seperate thread.  As a result, if no measurements are available, 
        // we simply wait for one to be available, then complete the method.
        //

        bool listEmpty = false;

        EnterCriticalSection(&m_EventQueueCS);
        
        if (IsListEmpty(&m_EventQueueHead)) 
        {
            listEmpty = true;
            ResetEvent(m_hEventSync);
        }

        LeaveCriticalSection(&m_EventQueueCS);

        //
        // if there are no events to process then we wait for a device event
        //
        if (listEmpty)
        {
            WaitForSingleObject(m_hEventSync, INFINITE);
        }
        
        //
        // The postcondition here is that either the queue was not empty,
        // or that an event was received.
        // IsListEmpty should always evaluate to FALSE at this point
        //
        
        EnterCriticalSection(&m_EventQueueCS);

        if (!IsListEmpty(&m_EventQueueHead))
        {
            PLIST_ENTRY ple = NULL;
            ple = RemoveHeadList(&m_EventQueueHead);
            pMeasurement = CONTAINING_RECORD(ple, HEARTRATE_MEASUREMENT, ListEntry);                    
        }

        LeaveCriticalSection(&m_EventQueueCS);

        if (NULL == pMeasurement)
        {
            hr = E_FAIL;
            CHECK_HR(hr, "No measurement available");
        }

        if (SUCCEEDED(hr))
        {
            //
            // Set the timestamp
            //
            if (SUCCEEDED(hr))
            {
                hr = pResults->SetUnsignedLargeIntegerValue(METHOD_PARAMETER_HealthHeartRateService_Measurement_TimeStamp, pMeasurement->TimeStamp);
                CHECK_HR(hr, "Failed to add METHOD_PARAMETER_HealthHeartRateService_Measurement_TimeStamp");
            }

            //
            // Set the measurement Rate
            //
            if (SUCCEEDED(hr))
            {
                hr = pResults->SetUnsignedIntegerValue(METHOD_PARAMETER_HealthHeartRateService_Measurement_Rate, pMeasurement->Rate);
                CHECK_HR(hr, "Failed to add METHOD_PARAMETER_HealthHeartRateService_Measurement_Rate");
            }

            delete pMeasurement;
            pMeasurement = NULL;
        }

        pResults->SetUnsignedLargeIntegerValue(METHOD_PARAMETER_HealthHeartRateService_Measurement_Result, SUCCEEDED(hr) ? true : false);
            
        CHECK_HR(hr, "METHOD_ReadHeartRateMeasurement failed");
    }
    else
    {
        hr = HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED);
        CHECK_HR(hr, "Unknown method %ws received",CComBSTR(Method));
    }

    return hr;
}


VOID
HealthHeartRateService::HeartRateMeasurementEvent(
    _In_ BTH_LE_GATT_EVENT_TYPE EventType,
    _In_ PVOID EventOutParameter
    )
{
    HRESULT hr = S_OK;
    PBLUETOOTH_GATT_VALUE_CHANGED_EVENT ValueChangedEventParameters = NULL;
    HEARTRATE_MEASUREMENT * pMeasurement = NULL;

    if (CharacteristicValueChangedEvent != EventType) {
        return;
    }

    ValueChangedEventParameters = (PBLUETOOTH_GATT_VALUE_CHANGED_EVENT)EventOutParameter;    

    //
    // Our value is at least 1
    //
    if (0 == ValueChangedEventParameters->CharacteristicValue->DataSize) {
        hr = E_FAIL;
        CHECK_HR(hr, "Invalid data size");
    }

    if (SUCCEEDED(hr))
    {
        pMeasurement = new HEARTRATE_MEASUREMENT;
        if (NULL == pMeasurement)
        {
            hr = E_FAIL;
            CHECK_HR(hr, "Failed to allocate the blood pressure meansurement");
        }
    }

    if (SUCCEEDED(hr))
    {
        ZeroMemory(pMeasurement, sizeof(*pMeasurement));

        InitializeListHead(&pMeasurement->ListEntry);
    }

    if (SUCCEEDED(hr))
    {
        //
        // if the first bit is set, then the value is the next 2 bytes.  If it is clear, the value is in the next byte
        //
        if (0x01 == (ValueChangedEventParameters->CharacteristicValue->Data[0] & 0x01)) {

            USHORT temp;

            if (3 > ValueChangedEventParameters->CharacteristicValue->DataSize) {
                hr = E_FAIL;
                CHECK_HR(hr, "Invalid data size");
            }

            RtlRetrieveUshort(&temp, &ValueChangedEventParameters->CharacteristicValue->Data[1]);

            pMeasurement->Rate = temp;
        } else {

            if (2 > ValueChangedEventParameters->CharacteristicValue->DataSize) {
                hr = E_FAIL;
                CHECK_HR(hr, "Invalid data size");
            }

            pMeasurement->Rate = ValueChangedEventParameters->CharacteristicValue->Data[1];
        }

        TraceEvents(TRACE_LEVEL_INFORMATION, TRACE_FLAG_DEVICE, "Received a value change event, new value [%x]", pMeasurement->Rate);
    }
    
    //
    // Get the timestamp
    //
    if (SUCCEEDED(hr))
    {
        FILETIME fTime;

        GetSystemTimeAsFileTime(&fTime);

        ConvertFileTimeToUlonglong(&fTime, &pMeasurement->TimeStamp);
        
    }  

    if (SUCCEEDED(hr))
    {    
        //
        // Now Queue the measurement
        //
        EnterCriticalSection(&m_EventQueueCS);

        InsertTailList(&m_EventQueueHead, &pMeasurement->ListEntry);
        pMeasurement = NULL;

        LeaveCriticalSection(&m_EventQueueCS);

        //
        // Now signal the event
        //
        SetEvent(m_hEventSync);
    }
    
    //
    // Cleanup
    //
    if (NULL != pMeasurement)
    {
        delete pMeasurement;
        pMeasurement = NULL;
    }
}






