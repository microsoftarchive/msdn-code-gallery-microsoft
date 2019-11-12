/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

Module Name:

    Internal.h

Abstract:

    This module contains the local type definitions for the sensors service
    driver.

--*/

#pragma once

#include <windows.h>
#include <atlbase.h>
#include <atlcom.h>
#include <atlcoll.h>
#include <atlstr.h>
#include <atlcomtime.h>
#include <strsafe.h>

_Analysis_mode_(_Analysis_code_type_user_driver_);  // Macro letting the compiler know this is not a kernel driver (this will help surpress needless warnings)

// Common WPD and WUDF headers
#include <devioctl.h>
#include <initguid.h>
#include <propkeydef.h>
#include <propvarutil.h>
#include "PortableDeviceTypes.h"
#include "PortableDeviceClassExtension.h"
#include "PortableDevice.h"

// Headers for Sensor specific defines and WpdCommands
#include "Sensors.h"
#include <SensorsClassExtension.h>

// Headers to include float limits
#include "float.h"

// Headers to include tracing
#include "Trace.h"

// Device Interface GUID used for Radio Management communication
// TODO: Create a new GUID
// {3BA2B796-B3F0-4225-9F8E-62A38E444208}
DEFINE_GUID(GUID_DEVINTERFACE_GPS_RADIO_MANAGEMENT,
    0x3ba2b796, 0xb3f0, 0x4225, 0x9f, 0x8e, 0x62, 0xa3, 0x8e, 0x44, 0x42, 0x8);

#define IOCTL_INDEX                         0x800
#define FILE_DEVICE_GPS_RADIO_MANAGEMENT    0x64900 // TODO: Pick a semi-unique value

#define IOCTL_GPS_RADIO_MANAGEMENT_GET_RADIO_STATE CTL_CODE(    \
    FILE_DEVICE_GPS_RADIO_MANAGEMENT,                           \
    IOCTL_INDEX,                                                \
    METHOD_BUFFERED,                                            \
    FILE_READ_ACCESS)

#define IOCTL_GPS_RADIO_MANAGEMENT_GET_PREVIOUS_RADIO_STATE CTL_CODE(    \
    FILE_DEVICE_GPS_RADIO_MANAGEMENT,                           \
    IOCTL_INDEX + 1,                                            \
    METHOD_BUFFERED,                                            \
    FILE_READ_ACCESS)

#define IOCTL_GPS_RADIO_MANAGEMENT_SET_RADIO_STATE CTL_CODE(    \
    FILE_DEVICE_GPS_RADIO_MANAGEMENT,                           \
    IOCTL_INDEX + 2,                                            \
    METHOD_BUFFERED,                                            \
    FILE_WRITE_ACCESS)

#define IOCTL_GPS_RADIO_MANAGEMENT_SET_PREVIOUS_RADIO_STATE CTL_CODE(    \
    FILE_DEVICE_GPS_RADIO_MANAGEMENT,                           \
    IOCTL_INDEX + 3,                                            \
    METHOD_BUFFERED,                                            \
    FILE_WRITE_ACCESS)

#define PROP_STORE_KEY_RADIO_STATE L"SENSOR_PROPERTY_RADIO_STATE"
#define PROP_STORE_KEY_PREVIOUS_RADIO_STATE L"SENSOR_PROPERTY_PREVIOUS_RADIO_STATE"


// IO map
const DWORD64 SHUTDOWN_IN_PROGRESS                          = 0x01;
const DWORD64 PROCESSING_IPNPCALLBACK                       = 0x02;
const DWORD64 PROCESSING_IPNPCALLBACKHARDWARE               = 0x04;
const DWORD64 PROCESSING_IFILECALLBACKCLEANUP               = 0x08;
const DWORD64 PROCESSING_IQUEUECALLBACKDEVICEIOCONTROL      = 0x10;
const DWORD64 PROCESSING_IREQUESTCALLBACKREQUESTCOMPLETION  = 0x20;
const DWORD64 PROCESSING_ISENSORDRIVER                      = 0x40;
const DWORD64 PROCESSING_ISENSOREVENT                       = 0x80;
const DWORD64 PROCESSING_IPNPCALLBACKSELFMANAGEDIO          = 0x100;
const DWORD64 PROCESSING_IN_PROGRESS                        = 0xFFFFFFFE;

///////////////////////////////////////////////////////////////////
// Common macro expansions that are used throughout the project
#define SAFE_RELEASE(p)     {if ((p)) { (p)->Release(); (p) = NULL; }}
#define ARRAY_SIZE(x)       (sizeof(x) / sizeof(x[0]))

///////////////////////////////////////////////////////////////////
// This comment block is scanned by the trace preprocessor to define our
// Trace function.
//
// begin_wpp config
// FUNC Trace{FLAG=MYDRIVER_ALL_INFO}(LEVEL, MSG, ...);
// end_wpp
//

enum SensorType 
{
    Collection = -2,
    SensorTypeNone = -1,
    Geolocation = 0,
    FirstSensorType = Geolocation,
    LastSensorType = Geolocation,
    SensorTypeUnknown,
};

enum SensorReportingState
{
    SENSOR_REPORTING_STATE_NO_EVENTS = 0,
    SENSOR_REPORTING_STATE_ALL_EVENTS,
};

enum SensorPowerState
{
    SENSOR_POWER_STATE_POWER_OFF = 0,
    SENSOR_POWER_STATE_LOW_POWER,
    SENSOR_POWER_STATE_FULL_POWER,
};

class CSensor; //forward declaration

typedef CAtlList< CSensor* > SENSOR_LIST;
typedef CAtlMap< ULONG, CComBSTR > SENSOR_ID_MAP;
typedef CAtlMap< ULONG, SensorType > SENSOR_TYPE_MAP;

const ULONG MAX_NUM_DATA_FIELDS = 32; //for use with CLIENT_ENTRY change sensitivity
typedef struct _CLIENT_ENTRY
{
    FLOAT       fltClientChangeSensitivity[MAX_NUM_DATA_FIELDS];
    ULONG       ulClientReportInterval;
    ULONG       ulClientLocationDesiredAccuracy;

} CLIENT_ENTRY, *PCLIENT_ENTRY;

typedef CAtlMap< IWDFFile*, CLIENT_ENTRY > CLIENT_MAP;

typedef struct _SUBSCRIBER_ENTRY
{
    BOOL        fSubscribed;

} SUBSCRIBER_ENTRY, *PSUBSCRIBER_ENTRY;

typedef CAtlMap< IWDFFile*, SUBSCRIBER_ENTRY > SUBSCRIBER_MAP;

typedef struct _PROPERTYKEY_DWVALUE_PAIR 
{
    PROPERTYKEY propkeyProperty; 
    DWORD dwPropertyValue;

} PROPERTYKEY_DWVALUE_PAIR, *PPROPERTYKEY_DWVALUE_PAIR;


//NOTE: following is specific to the Geolocation sample
#include <radiomgr.h>

// Module defines
const unsigned char MODULE_NAME[] = "SensorsGeolocationDriverSample.dll"; //VER_ORIGINALFILENAME_STR

// Default values
const unsigned short DEFAULT_DEVICE_MODEL_VALUE[]   = L"Geolocation Driver Sample";
const unsigned short DEFAULT_SERIAL_NUMBER[]        = L"57E940BB-A3ED-491B-9722-A605392668E4";
const unsigned short DEFAULT_MANUFACTURER[]         = L"Microsoft Corporation";

const FLOAT DEFAULT_MIN_CHANGE_SENSITIVITY          = 1.0F;
const FLOAT DEFAULT_MAX_CHANGE_SENSITIVITY          = 100.0F;    
const ULONG DEFAULT_MIN_REPORT_INTERVAL             = 16; //mS
const ULONG DEFAULT_MAX_REPORT_INTERVAL             = 600000; //mS
const ULONG DEFAULT_SLEEP_REPORT_INTERVAL           = 0;

// Read/Write defines
const ULONG  INITIAL_DATA_POLL_MAX_RETRIES  = 5;
const ULONG  FEATURE_REPORT_MAX_RETRIES     = 10;

const ULONG  DEVICE_POLL_TIMEOUT = 15; //mS

const FLOAT  CHANGE_SENSITIVITY_NOT_SET = -1.0F; //invalid value

// sensor defines
const ULONG  DESCRIPTOR_MAX_LENGTH                  = (126*2);
const ULONG  SENSOR_ID_APPENDIX_MAX_LENGTH          = 9;

//
// WUDF power policy settings
//

const WDF_POWER_POLICY_S0_IDLE_CAPABILITIES SENSOR_POWER_POLICY_S0_IDLE_CAPABILITIES    = IdleCannotWakeFromS0;

// Set delay timeout value. This specifies the time
// delay between WDF detecting the device is idle
// and WDF requesting a Dx power transition on the 
// device's behalf.
const ULONG SENSOR_POWER_POLICY_IDLE_TIMEOUT                                            = 100;

// Opt-in to D3Cold to allow the platform to remove 
// power when the device is idle and enters D3.
const WDF_TRI_STATE SENSOR_POWER_POLICY_EXCLUDE_D3_COLD                                 = WdfFalse;

