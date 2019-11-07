//
//    Copyright (C) Microsoft.  All rights reserved.
//
/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Module Name:

    Driver.cpp

Abstract:

    This module contains the implementation for the HID sensor class 
    driver callback class.

--*/

#include "internal.h"
#include "SensorsHIDClassDriver.h" // IDL Generated File
#include "Driver.h"
#include "Device.h"


#include "Driver.tmh"

/////////////////////////////////////////////////////////////////////////
//
// CMyDriver::CMyDriver
//
// Object constructor function
//
/////////////////////////////////////////////////////////////////////////
CMyDriver::CMyDriver()
{
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDriver::OnDeviceAdd
//
// The framework call this function when device is detected. This driver
// creates a device callback object
//
// Parameters:
//      pDriver     - pointer to an IWDFDriver object
//      pDeviceInit - pointer to a device initialization object
//
// Return Values:
//      S_OK: device initialized successfully
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDriver::OnDeviceAdd(
    _In_ IWDFDriver* pDriver,
    _In_ IWDFDeviceInitialize* pDeviceInit
    )
{
    //TODO change the date/time and build strings
    const char COMPILE_DATE[]  = "April 19, 2012"; //__DATE__;
    const char COMPILE_TIME[]  = "08:24AM"; //__TIME__;

    //informational at the start of the run in case there are no other log entries to view
    Trace(TRACE_LEVEL_CRITICAL, "   ");
    Trace(TRACE_LEVEL_CRITICAL, "------------------------------ START ------------------------------------------");
    Trace(TRACE_LEVEL_CRITICAL, "SensorsHIDClassDriver, changed on %s at %s", COMPILE_DATE, COMPILE_TIME);
    Trace(TRACE_LEVEL_CRITICAL, "SensorsHIDClassDriver - Trace log running with TRACE_LEVEL == CRITICAL");
    Trace(TRACE_LEVEL_ERROR, "SensorsHIDClassDriver - Trace log running with TRACE_LEVEL == ERROR");
    Trace(TRACE_LEVEL_WARNING, "SensorsHIDClassDriver - Trace log running with TRACE_LEVEL == WARNING");
    Trace(TRACE_LEVEL_INFORMATION, "SensorsHIDClassDriver - Trace log running with TRACE_LEVEL == INFORMATION");
    Trace(TRACE_LEVEL_VERBOSE, "SensorsHIDClassDriver - Trace log running with TRACE_LEVEL == VERBOSE");

    Trace(TRACE_LEVEL_INFORMATION, "%!FUNC! Entry");

    HRESULT hr = CMyDevice::CreateInstance(pDriver, pDeviceInit);

    return hr;
}
/////////////////////////////////////////////////////////////////////////
//
// CMyDriver::OnInitialize
//
//  The framework calls this function just after loading the driver. The driver
//  can perform any global, device independent intialization in this routine.
//
/////////////////////////////////////////////////////////////////////////
HRESULT CMyDriver::OnInitialize(
    _In_ IWDFDriver* pDriver
    )
{
    UNREFERENCED_PARAMETER(pDriver);

    return S_OK;
}

/////////////////////////////////////////////////////////////////////////
//
// CMyDriver::OnDeinitialize
//
//  The framework calls this function just before de-initializing itself. All
//  WDF framework resources should be released by driver before returning
//  from this call.
//
/////////////////////////////////////////////////////////////////////////
void CMyDriver::OnDeinitialize(
    _In_ IWDFDriver* pDriver
    )
{
    UNREFERENCED_PARAMETER(pDriver);
    return;
}

