/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    HealthThermometerService.cpp
    
Abstract:

--*/

#include "stdafx.h"

#include "HealthThermometerService.tmh"

// Supported events
EventAttributeInfo g_SupportedServiceEvents[] =
{
    {&EVENT_HealthThermometerService_TemperatureMeasurement, NAME_EVENT_HealthThermometerService_TemperatureMeasurement},
};

// Event parameters
EventParameterAttributeInfo g_ServiceEventParameters[] =
{
    {&EVENT_HealthThermometerService_TemperatureMeasurement, &WPD_EVENT_PARAMETER_EVENT_ID,                                      VT_CLSID},
    {&EVENT_HealthThermometerService_TemperatureMeasurement, &EVENT_PARAMETER_HealthTemperatureService_Measurement_TimeStamp,    VT_UI8},        
    {&EVENT_HealthThermometerService_TemperatureMeasurement, &EVENT_PARAMETER_HealthTemperatureService_Measurement_Value,        VT_R4},
    {&EVENT_HealthThermometerService_TemperatureMeasurement, &WPD_EVENT_PARAMETER_OBJECT_PARENT_PERSISTENT_UNIQUE_ID,            VT_LPWSTR},
    {&EVENT_HealthThermometerService_TemperatureMeasurement, &WPD_OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID,                         VT_LPWSTR},
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

HealthThermometerService::HealthThermometerService()
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


HealthThermometerService::~HealthThermometerService()
{
    //
    // This service doesn't support queueing, no events should ever be queued
    //   
}


