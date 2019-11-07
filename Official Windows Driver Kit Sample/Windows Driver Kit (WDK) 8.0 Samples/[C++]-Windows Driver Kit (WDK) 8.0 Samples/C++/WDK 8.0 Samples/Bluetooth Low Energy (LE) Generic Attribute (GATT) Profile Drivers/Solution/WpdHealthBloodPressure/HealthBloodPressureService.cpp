/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    HealthBloodPressureService.cpp
    
Abstract:

--*/

#include "stdafx.h"

#include "HealthBloodPressureService.tmh"

// Supported events
EventAttributeInfo g_SupportedServiceEvents[] =
{
    {&EVENT_HealthBloodPressureService_Measurement, NAME_EVENT_HealthBloodPressureService_Measurement},
};

// Event parameters
EventParameterAttributeInfo g_ServiceEventParameters[] =
{
    {&EVENT_HealthBloodPressureService_Measurement, &WPD_EVENT_PARAMETER_EVENT_ID,                                                  VT_CLSID},
    {&EVENT_HealthBloodPressureService_Measurement, &EVENT_PARAMETER_HealthBloodPressureService_Measurement_TimeStamp,              VT_UI8},        
    {&EVENT_HealthBloodPressureService_Measurement, &EVENT_PARAMETER_HealthBloodPressureService_Measurement_Type,                   VT_LPWSTR},
    {&EVENT_HealthBloodPressureService_Measurement, &EVENT_PARAMETER_HealthBloodPressureService_Measurement_Systolic,               VT_R4},
    {&EVENT_HealthBloodPressureService_Measurement, &EVENT_PARAMETER_HealthBloodPressureService_Measurement_Diastolic,              VT_R4},
    {&EVENT_HealthBloodPressureService_Measurement, &EVENT_PARAMETER_HealthBloodPressureService_Measurement_MeanArterialPressure,   VT_R4},
    {&EVENT_HealthBloodPressureService_Measurement, &WPD_EVENT_PARAMETER_OBJECT_PARENT_PERSISTENT_UNIQUE_ID,                        VT_LPWSTR},
    {&EVENT_HealthBloodPressureService_Measurement, &WPD_OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID,                                     VT_LPWSTR},
};

// Supported Methods
MethodAttributeInfo g_SupportedServiceMethods[] = 
{
    {NULL, NULL, 0},
};

MethodParameterAttributeInfo g_ServiceMethodParameters[] = 
{
    {NULL, NULL, 0, WPD_PARAMETER_USAGE_RETURN, WPD_PARAMETER_ATTRIBUTE_FORM_UNSPECIFIED , 0, NULL},
};

HealthBloodPressureService::HealthBloodPressureService()
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

HealthBloodPressureService::~HealthBloodPressureService()
{
    //
    // This service doesn't support queueing, no events should ever be queued
    //
}




