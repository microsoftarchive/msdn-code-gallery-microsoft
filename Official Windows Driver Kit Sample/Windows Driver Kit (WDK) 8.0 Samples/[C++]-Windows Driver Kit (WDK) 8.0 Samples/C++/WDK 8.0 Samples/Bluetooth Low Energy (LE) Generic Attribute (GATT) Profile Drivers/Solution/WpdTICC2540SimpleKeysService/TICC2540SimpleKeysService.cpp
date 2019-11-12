/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    TICC2540SimpleKeysService.cpp
    
Abstract:

--*/

#include "stdafx.h"

#include "TICC2540SimpleKeysService.tmh"

// Supported events
EventAttributeInfo g_SupportedServiceEvents[] =
{
    {&EVENT_TICC2540SimpleKeysService_KeyPressed, NAME_EVENT_TICC2540SimpleKeysService_KeyPressed},
};

// Event parameters
EventParameterAttributeInfo g_ServiceEventParameters[] =
{
    {&EVENT_TICC2540SimpleKeysService_KeyPressed, &WPD_EVENT_PARAMETER_EVENT_ID,                              VT_CLSID},
    {&EVENT_TICC2540SimpleKeysService_KeyPressed, &EVENT_PARAMETER_TICC2540SimpleKeysService_KeyPressValue,   VT_UI4},
    {&EVENT_TICC2540SimpleKeysService_KeyPressed, &WPD_EVENT_PARAMETER_OBJECT_PARENT_PERSISTENT_UNIQUE_ID,    VT_LPWSTR},
    {&EVENT_TICC2540SimpleKeysService_KeyPressed, &WPD_OBJECT_CONTAINER_FUNCTIONAL_OBJECT_ID,                 VT_LPWSTR},
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

TICC2540SimpleKeysService::TICC2540SimpleKeysService()
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


TICC2540SimpleKeysService::~TICC2540SimpleKeysService()
{
    //
    // This service doesn't support queueing, no events should ever be queued
    //
}


