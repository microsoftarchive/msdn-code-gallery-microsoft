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

    This module contains the local type definitions for the HID sensor class driver.

--*/

#pragma once

//disabling warning regarding unary & operator possibly returning unaligned in sensor property handling helper methods
#pragma warning(disable:4366)


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

// Headers for hid specific defines
#include "hidusage.h"
extern "C" 
{
#include "hidsdi.h"
}
#include "hidclass.h"

// Headers to include float limits
#include "float.h"

// Headers to include tracing
#include "Trace.h"

const DWORD64 SHUTDOWN_IN_PROGRESS                          = 0x01;
const DWORD64 PROCESSING_IPNPCALLBACK                       = 0x02;
const DWORD64 PROCESSING_IPNPCALLBACKHARDWARE               = 0x04;
const DWORD64 PROCESSING_IFILECALLBACKCLEANUP               = 0x08;
const DWORD64 PROCESSING_IQUEUECALLBACKDEVICEIOCONTROL      = 0x10;
const DWORD64 PROCESSING_IREQUESTCALLBACKREQUESTCOMPLETION  = 0x20;
const DWORD64 PROCESSING_ISENSORDRIVER                      = 0x40;
const DWORD64 PROCESSING_ISENSOREVENT                       = 0x80;
const DWORD64 PROCESSING_IN_PROGRESS                        = 0xFE;

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
    Accelerometer = 0,
    FirstSensorType = Accelerometer,
    AmbientLight, 
    Presence,
    Compass,
    Gyrometer,
    Inclinometer,
    Barometer,
    Hygrometer,
    Thermometer,
    Potentiometer,
    Distance,
    Switch,
    Voltage,
    Current,
    Power,
    Frequency,
    Orientation,
    Custom,
    Generic,
    Unsupported,
    LastSensorType = Unsupported, 
};

enum SensorPowerState
{
    SENSOR_POWER_STATE_POWER_OFF = 0,
    SENSOR_POWER_STATE_LOW_POWER,
    SENSOR_POWER_STATE_FULL_POWER,
};

enum SensorEventType
{
    SENSOR_EVENT_TYPE_UNKNOWN = 0,
    SENSOR_EVENT_TYPE_STATE_CHANGED,
    SENSOR_EVENT_TYPE_PROPERTY_CHANGED,
    SENSOR_EVENT_TYPE_DATA_UPDATED,
    SENSOR_EVENT_TYPE_POLL_RESPONSE,
    SENSOR_EVENT_TYPE_CHANGE_SENSITIVITY,
};

class CSensor; //forward declaration

typedef CAtlList< CSensor* > SENSOR_LIST;
typedef CAtlMap< ULONG, CComBSTR > SENSOR_ID_MAP;
typedef CAtlMap< ULONG, SensorType > SENSOR_TYPE_MAP;
typedef CAtlMap< ULONG, ULONG > SENSOR_USAGE_MAP;
typedef CAtlMap< ULONG, USHORT > SENSOR_LINKS_MAP;

const ULONG MAX_NUM_DATA_FIELDS = 32; //for use with CLIENT_ENTRY change sensitivity
typedef struct _CLIENT_ENTRY
{
    FLOAT       fltClientChangeSensitivity[MAX_NUM_DATA_FIELDS];
    ULONG       ulClientReportInterval;

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
    DWORD       dwPropertyValue;

} PROPERTYKEY_DWVALUE_PAIR, *PPROPERTYKEY_DWVALUE_PAIR;

typedef struct _PROPERTYKEY_USVALUE_PAIR 
{
    PROPERTYKEY propkeyProperty; 
    USHORT      usPropertyValue;

} PROPERTYKEY_USVALUE_PAIR, *PPROPERTYKEY_USVALUE_PAIR;

// Module defines
const unsigned char MODULE_NAME[] = "SensorsHIDDriverSample.dll"; //VER_ORIGINALFILENAME_STR

// Default values
const unsigned short DEFAULT_DEVICE_MODEL_VALUE[]   = L"HID Sensor Class Device";

// Read/Write defines
const ULONG  READ_BUFFER_SIZE = 64;
const ULONG  MAX_REPORT_SIZE  = 64;
const ULONG  PENDING_READ_COUNT = 1;

const ULONG  INITIAL_DATA_POLL_MAX_RETRIES  = 1;
const ULONG  FEATURE_REPORT_MAX_RETRIES     = 2;

const ULONG  DEVICE_POLL_TIMEOUT = 15; //mS

const ULONG DEVICE_SYNCHRONOUS_REQUEST_FLAGS        = WDF_REQUEST_SEND_OPTION_SYNCHRONOUS | WDF_REQUEST_SEND_OPTION_TIMEOUT;
const LONGLONG DEVICE_SYNCHRONOUS_REQUEST_TIMEOUT   = -10000000; // 1s, in 100-ns intervals

const FLOAT  CHANGE_SENSITIVITY_NOT_SET = -1.0F; //invalid value

// HID defines
const ULONG  HID_USB_DESCRIPTOR_MAX_LENGTH          = (126*2);
const ULONG  HID_USB_SENSOR_ID_APPENDIX_MAX_LENGTH  = 9;
const ULONG  HID_IN_REPORT_OFFSET                   = 1;
const DWORD  USB_LEADING_BYTE                       = 1;
const ULONG  HID_FEATURE_REPORT_STRING_MAX_LENGTH   = 33;
const ULONG  HID_SIGN_FACTOR                        = 65536;

// Character page defines
const ULONG  CP_ASCII                               = 20127;

// auxilliary Math limits defines
#ifndef USHORT_MAX
#define USHORT_MAX 65535
#endif

//sensor category usages from HID sensor page specification
//NOTE: These are "defines" rather than "const" in order to simply coordination with the firmware
////////////////////////////////////////////////////////////////////////////////////
//
// Sensor-specific HID definitions
//
// These #defines are used in within this driver. They are similar to the #defines
// below that show the definitions from the device side. These are NOT used for 
// HID Report Descriptors
//
////////////////////////////////////////////////////////////////////////////////////

#define HID_DRIVER_USAGE_PAGE_SENSOR                                                    0x0020

//sensor category usages
#define HID_DRIVER_USAGE_SENSOR_TYPE_COLLECTION                                         0x0001
//sensor category biometric
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_BIOMETRIC                                      0x0010
#define HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_PRESENCE                                 0x0011
#define HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_PROXIMITY                                0x0012
#define HID_DRIVER_USAGE_SENSOR_TYPE_BIOMETRIC_TOUCH                                    0x0013
//sensor category electrical
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_ELECTRICAL                                     0x0020
#define HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_CAPACITANCE                             0x0021
#define HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_CURRENT                                 0x0022
#define HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_POWER                                   0x0023
#define HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_INDUCTANCE                              0x0024
#define HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_RESISTANCE                              0x0025
#define HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_VOLTAGE                                 0x0026
#define HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_POTENTIOMETER                           0x0027
#define HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_FREQUENCY                               0x0028
#define HID_DRIVER_USAGE_SENSOR_TYPE_ELECTRICAL_PERIOD                                  0x0029
//sensor category environmental
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_ENVIRONMENTAL                                  0x0030
#define HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_ATMOSPHERIC_PRESSURE                 0x0031
#define HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_HUMIDITY                             0x0032
#define HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_TEMPERATURE                          0x0033
#define HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_WIND_DIRECTION                       0x0034
#define HID_DRIVER_USAGE_SENSOR_TYPE_ENVIRONMENTAL_WIND_SPEED                           0x0035
//sensor category light
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_LIGHT                                          0x0040
#define HID_DRIVER_USAGE_SENSOR_TYPE_LIGHT_AMBIENTLIGHT                                 0x0041
#define HID_DRIVER_USAGE_SENSOR_TYPE_LIGHT_CONSUMER_INFRARED                            0x0042
//sensor category location
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_LOCATION                                       0x0050
#define HID_DRIVER_USAGE_SENSOR_TYPE_LOCATION_BROADCAST                                 0x0051
#define HID_DRIVER_USAGE_SENSOR_TYPE_LOCATION_DEAD_RECKONING                            0x0052
#define HID_DRIVER_USAGE_SENSOR_TYPE_LOCATION_GPS                                       0x0053
#define HID_DRIVER_USAGE_SENSOR_TYPE_LOCATION_LOOKUP                                    0x0054
#define HID_DRIVER_USAGE_SENSOR_TYPE_LOCATION_OTHER                                     0x0055
#define HID_DRIVER_USAGE_SENSOR_TYPE_LOCATION_STATIC                                    0x0056
#define HID_DRIVER_USAGE_SENSOR_TYPE_LOCATION_TRIANGULATION                             0x0057
//sensor category mechanical
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_MECHANICAL                                     0x0060
#define HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH                          0x0061
#define HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_BOOLEAN_SWITCH_ARRAY                    0x0062
#define HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_MULTIVAL_SWITCH                         0x0063
#define HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_FORCE                                   0x0064
#define HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_PRESSURE                                0x0065
#define HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_STRAIN                                  0x0066
#define HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_SCALE_WEIGHT                            0x0067
#define HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_VIBRATOR                                0x0068
#define HID_DRIVER_USAGE_SENSOR_TYPE_MECHANICAL_HALL_EFFECT_SWITCH                      0x0069
//sensor category motion
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_MOTION                                         0x0070
#define HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_ACCELEROMETER_1D                            0x0071
#define HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_ACCELEROMETER_2D                            0x0072
#define HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_ACCELEROMETER_3D                            0x0073
#define HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_1D                                0x0074
#define HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_2D                                0x0075
#define HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER_3D                                0x0076
#define HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_MOTION_DETECTOR                             0x0077
#define HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_SPEEDOMETER                                 0x0078
#define HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_ACCELEROMETER                               0x0079
#define HID_DRIVER_USAGE_SENSOR_TYPE_MOTION_GYROMETER                                   0x007A
//sensor category orientation
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_ORIENTATION                                    0x0080
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_1D                             0x0081
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_2D                             0x0082
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS_3D                             0x0083
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_INCLINOMETER_1D                        0x0084
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_INCLINOMETER_2D                        0x0085
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_INCLINOMETER_3D                        0x0086
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DISTANCE_1D                            0x0087
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DISTANCE_2D                            0x0088
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DISTANCE_3D                            0x0089
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DEVICE_ORIENTATION                     0x008A
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_COMPASS                                0x008B
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_INCLINOMETER                           0x008C
#define HID_DRIVER_USAGE_SENSOR_TYPE_ORIENTATION_DISTANCE                               0x008D
//sensor category scanner
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_SCANNER                                        0x0090
#define HID_DRIVER_USAGE_SENSOR_TYPE_SCANNER_RFID                                       0x0091
#define HID_DRIVER_USAGE_SENSOR_TYPE_SCANNER_BARCODE                                    0x0092
#define HID_DRIVER_USAGE_SENSOR_TYPE_SCANNER_NFC                                        0x0093
//sensor category time
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_TIME                                           0x00A0
#define HID_DRIVER_USAGE_SENSOR_TYPE_TIME_ALARM                                         0x00A1
#define HID_DRIVER_USAGE_SENSOR_TYPE_TIME_RTC                                           0x00A2
//sensor category other
#define HID_DRIVER_USAGE_SENSOR_CATEGORY_OTHER                                          0x00E0
#define HID_DRIVER_USAGE_SENSOR_TYPE_OTHER_CUSTOM                                       0x00E1
#define HID_DRIVER_USAGE_SENSOR_TYPE_OTHER_GENERIC                                      0x00E2
#define HID_DRIVER_USAGE_SENSOR_TYPE_OTHER_GENERIC_ENUMERATOR                           0x00E3

//unit usages
#define HID_DRIVER_USAGE_SENSOR_UNITS_NOT_SPECIFIED                                     0x00
//TODO change these to single hex number according to byte ordering in spec
#define HID_DRIVER_USAGE_SENSOR_UNITS_LUX                                               0xE1,0x00,0x00,0x01 // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_KELVIN                                            0x01,0x00,0x01,0x00 // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_FAHRENHEIT                                        0x03,0x00,0x01,0x00 // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_PASCAL                                            0xF1,0xE1           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_NEWTON                                            0x11,0xE1           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_METERS_PER_SECOND                                 0x11,0xF0           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_METERS_PER_SEC_SQRD                               0x11,0xE0           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_FARAD                                             0xE1,0x4F,0x20,0x00 // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_AMPERE                                            0x01,0x00,0x10,0x00 // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_WATT                                              0x21,0xD1           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_HENRY                                             0x21,0xE1,0xE0,0x00 // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_OHM                                               0x21,0xD1,0xE0,0x00 // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_VOLT                                              0x21,0xD1,0xF0,0x00 // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_HERTZ                                             0x01,0xF0           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEGREES                                           0x14                // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEGREES_PER_SECOND                                0x14,0xF0           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEGREES_PER_SEC_SQRD                              0x14,0xE0           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_RADIANS                                           0x12                // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_RADIANS_PER_SECOND                                0x12,0xF0           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_RADIANS_PER_SEC_SQRD                              0x12,0xE0           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_SECOND                                            0x01,0x10           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_GAUSS                                             0x01,0xE1,0xF0,0x00 // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_GRAM                                              0x01,0x01           // Unit
#define HID_DRIVER_USAGE_SENSOR_UNITS_CENTIMETER                                        0x11                // Unit
#ifdef DEFINE_NON_HID_UNITS
#define HID_DRIVER_USAGE_SENSOR_UNITS_CELSIUS              “Use Unit(Kelvin) and subtract 273.15”
#define HID_DRIVER_USAGE_SENSOR_UNITS_KILOGRAM             “Use Unit(gram) and UnitExponent(0x03)”
#define HID_DRIVER_USAGE_SENSOR_UNITS_METER                “Use Unit(centimeter) and UnitExponent(0x02)”
#define HID_DRIVER_USAGE_SENSOR_UNITS_BAR                  “Use Unit(Pascal) and UnitExponent(0x05)”
#define HID_DRIVER_USAGE_SENSOR_UNITS_KNOT                 “Use Unit(m/s) and multiply by 1852/3600”
#define HID_DRIVER_USAGE_SENSOR_UNITS_PERCENT              “Use Unit(Not_Specified)”
#define HID_DRIVER_USAGE_SENSOR_UNITS_G                    “Use Unit(m/s2) and divide by 9.8”
#define HID_DRIVER_USAGE_SENSOR_UNITS_MILLISECOND          “Use Unit(second) and UnitExponent(0x0D)”
#define HID_DRIVER_USAGE_SENSOR_UNITS_MILLIGAUSS           “Use Unit(Gauss) and UnitExponent(0x0D)”
#endif
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_LUX                                    0x01
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KELVIN                                 0x02
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_CELSIUS                                0x03
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_PASCAL                                 0x04
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_NEWTON                                 0x05
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_METERS_PER_SECOND                      0x06
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KILOGRAM                               0x07
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_METER                                  0x08
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_METERS_PER_SEC_SQRD                    0x09
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_FARAD                                  0x0A
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_AMPERE                                 0x0B
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_WATT                                   0x0C
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_HENRY                                  0x0D
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_OHM                                    0x0E
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_VOLT                                   0x0F
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_HERTZ                                  0x10
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_BAR                                    0x11
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_ANTI_CLOCKWISE                 0x12
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_CLOCKWISE                      0x13
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREE                                 0x14
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_DEGREES_PER_SECOND                     0x15
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_KNOT                                   0x16
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_PERCENT                                0x17
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_SECOND                                 0x18
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLISECOND                            0x19
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_G                                      0x1A
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_BYTES                                  0x1B
#define HID_DRIVER_USAGE_SENSOR_UNITS_DEPRECATED_MILLIGAUSS                             0x1C

//data type usages modifiers
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_NONE                                           0x0000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_ABS                         0x1000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_MAX                                            0x2000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_MIN                                            0x3000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_ACCURACY                                       0x4000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_RESOLUTION                                     0x5000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_THRESHOLD_HIGH                                 0x6000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_THRESHOLD_LOW                                  0x7000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_CALIBRATION_OFFSET                             0x8000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_CALIBRATION_MULTIPLIER                         0x9000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_REPORT_INTERVAL                                0xA000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_FREQUENCY_MAX                                  0xB000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_PERIOD_MAX                                     0xC000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_RANGE_PCT                   0xD000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_CHANGE_SENSITIVITY_REL_PCT                     0xE000 //US
#define HID_DRIVER_USAGE_SENSOR_DATA_MOD_VENDOR_RESERVED                                0xF000 //US

//state usages
#define HID_DRIVER_USAGE_SENSOR_STATE                                                   0x0201 // NAry
//state selectors
#define HID_DRIVER_USAGE_SENSOR_STATE_UNKNOWN_SEL                                       0x0800 // Sel
#define HID_DRIVER_USAGE_SENSOR_STATE_READY_SEL                                         0x0801 // Sel
#define HID_DRIVER_USAGE_SENSOR_STATE_NOT_AVAILABLE_SEL                                 0x0802 // Sel
#define HID_DRIVER_USAGE_SENSOR_STATE_NO_DATA_SEL                                       0x0803 // Sel
#define HID_DRIVER_USAGE_SENSOR_STATE_INITIALIZING_SEL                                  0x0804 // Sel
#define HID_DRIVER_USAGE_SENSOR_STATE_ACCESS_DENIED_SEL                                 0x0805 // Sel
#define HID_DRIVER_USAGE_SENSOR_STATE_ERROR_SEL                                         0x0806 // Sel
//state enums
#define HID_DRIVER_USAGE_SENSOR_STATE_UNKNOWN_ENUM                                      0x01 // Enum
#define HID_DRIVER_USAGE_SENSOR_STATE_READY_ENUM                                        0x02 // Enum
#define HID_DRIVER_USAGE_SENSOR_STATE_NOT_AVAILABLE_ENUM                                0x03 // Enum
#define HID_DRIVER_USAGE_SENSOR_STATE_NO_DATA_ENUM                                      0x04 // Enum
#define HID_DRIVER_USAGE_SENSOR_STATE_INITIALIZING_ENUM                                 0x05 // Enum
#define HID_DRIVER_USAGE_SENSOR_STATE_ACCESS_DENIED_ENUM                                0x06 // Enum
#define HID_DRIVER_USAGE_SENSOR_STATE_ERROR_ENUM                                        0x07 // Enum
//state deprecated enums
#define HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_UNKNOWN                                0x00
#define HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_NOT_AVAILABLE                          0x01
#define HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_READY                                  0x02
#define HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_NO_DATA                                0x03
#define HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_INITIALIZING                           0x04
#define HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_ACCESS_DENIED                          0x05
#define HID_DRIVER_USAGE_SENSOR_STATE_DEPRECATED_ERROR                                  0x06

//event usages
#define HID_DRIVER_USAGE_SENSOR_EVENT                                                   0x0202 // NAry
//event selectors
#define HID_DRIVER_USAGE_SENSOR_EVENT_UNKNOWN_SEL                                       0x0810 // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_STATE_CHANGED_SEL                                 0x0811 // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_PROPERTY_CHANGED_SEL                              0x0812 // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_DATA_UPDATED_SEL                                  0x0813 // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_POLL_RESPONSE_SEL                                 0x0814 // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_CHANGE_SENSITIVITY_SEL                            0x0815 // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_MAX_REACHED_SEL                                   0x0816 // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_MIN_REACHED_SEL                                   0x0817 // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_UPWARD_SEL                   0x0818 // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_HIGH_THESHOLD_CROSS_ABOVE_SEL        HID_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_UPWARD_SEL
#define HID_DRIVER_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_DOWNWARD_SEL                 0x0819 // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_BELOW_SEL       HID_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_DOWNWARD_SEL
#define HID_DRIVER_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_UPWARD_SEL                    0x081A // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_ABOVE_SEL        HID_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_UPWARD_SEL
#define HID_DRIVER_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_DOWNWARD_SEL                  0x081B // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_BELOW_SEL        HID_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_DOWNWARD_SEL
#define HID_DRIVER_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_UPWARD_SEL                   0x081C // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_ABOVE_SEL       HID_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_UPWARD_SEL
#define HID_DRIVER_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_DOWNWARD_SEL                 0x081D // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_BELOW_SEL       HID_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_DOWNWARD_SEL
#define HID_DRIVER_USAGE_SENSOR_EVENT_PERIOD_EXCEEDED_SEL                               0x081E // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_FREQUENCY_EXCEEDED_SEL                            0x081F // Sel
#define HID_DRIVER_USAGE_SENSOR_EVENT_COMPLEX_TRIGGER_SEL                               0x0820 // Sel
//event enums
#define HID_DRIVER_USAGE_SENSOR_EVENT_UNKNOWN_ENUM                                      0x01 // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_STATE_CHANGED_ENUM                                0x02 // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_PROPERTY_CHANGED_ENUM                             0x03 // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_DATA_UPDATED_ENUM                                 0x04 // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_POLL_RESPONSE_ENUM                                0x05 // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_CHANGE_SENSITIVITY_ENUM                           0x06 // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_MAX_REACHED_ENUM                                  0x07 // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_MIN_REACHED_ENUM                                  0x08 // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_UPWARD_ENUM                  0x09 // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_HIGH_THESHOLD_CROSS_ABOVE_ENUM   HID_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_UPWARD_ENUM
#define HID_DRIVER_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_DOWNWARD_ENUM                0x0A // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_BELOW_ENUM  HID_USAGE_SENSOR_EVENT_HIGH_THRESHOLD_CROSS_DOWNWARD_ENUM
#define HID_DRIVER_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_UPWARD_ENUM                   0x0B // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_ABOVE_ENUM   HID_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_UPWARD_ENUM
#define HID_DRIVER_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_DOWNWARD_ENUM                 0x0C // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_BELOW_ENUM   HID_USAGE_SENSOR_EVENT_LOW_THRESHOLD_CROSS_DOWNWARD_ENUM
#define HID_DRIVER_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_UPWARD_ENUM                  0x0D // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_ABOVE_ENUM  HID_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_UPWARD_ENUM
#define HID_DRIVER_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_DOWNWARD_ENUM                0x0E // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_BELOW_ENUM  HID_USAGE_SENSOR_EVENT_ZERO_THRESHOLD_CROSS_DOWNWARD_ENUM
#define HID_DRIVER_USAGE_SENSOR_EVENT_PERIOD_EXCEEDED_ENUM                              0x0F // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_FREQUENCY_EXCEEDED_ENUM                           0x10 // Enum
#define HID_DRIVER_USAGE_SENSOR_EVENT_COMPLEX_TRIGGER_ENUM                              0x11 // Enum
//event deprecated enums
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_UNKNOWN                                0x00
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_STATE_CHANGED                          0x01
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_PROPERTY_CHANGED                       0x02
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_DATA_UPDATE                            0x03
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_POLL_RESPONSE                          0x04
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_CHANGE_SENSITIVITY                     0x05
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_MAX_REACHED                            0x06
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_MIN_REACHED                            0x07
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_HIGH_THRESHHOLD_CROSS_ABOVE            0x08
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_HIGH_THRESHHOLD_CROSS_BELOW            0x09
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_LOW_THRESHHOLD_CROSS_ABOVE             0x0A
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_LOW_THRESHHOLD_CROSS_BELOW             0x0B
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_ZERO_THRESHOLD_CROSS_ABOVE             0x0C
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_ZERO_THRESHOLD_CROSS_BELOW             0x0D
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_PERIOD_EXCEEDED                        0x0E
#define HID_DRIVER_USAGE_SENSOR_EVENT_DEPRECATED_FREQUENCY_EXCEEDED                     0x0F

//property usages (get/set feature report)
#define HID_DRIVER_USAGE_SENSOR_PROPERTY                                                0x0300
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_FRIENDLY_NAME                                  0x0301
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_PERSISTENT_UNIQUE_ID                           0x0302
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_STATUS                                  0x0303
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_MINIMUM_REPORT_INTERVAL                        0x0304
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_MANUFACTURER                            0x0305
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_MODEL                                   0x0306
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_SERIAL_NUMBER                           0x0307
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_DESCRIPTION                             0x0308
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_CONNECTION_TYPE                         0x0309 // NAry
//begin connection type selectors
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_INTEGRATED_SEL              0x0830 // Sel
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_ATTACHED_SEL                0x0831 // Sel
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_EXTERNAL_SEL                0x0832 // Sel
//end connection type selectors
//begin connection type enums
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_INTEGRATED_ENUM             0x01 // Enum
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_ATTACHED_ENUM               0x02 // Enum
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_CONNECTION_TYPE_PC_EXTERNAL_ENUM               0x03 // Enum
//end connection type enums
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SENSOR_DEVICE_PATH                             0x030A
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_HARDWARE_REVISION                              0x030B
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_FIRMWARE_VERSION                               0x030C
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_RELEASE_DATE                                   0x030D
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORT_INTERVAL                                0x030E
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_ABS                         0x030F
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_RANGE_PCT                   0x0310
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_CHANGE_SENSITIVITY_REL_PCT                     0x0311
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_ACCURACY                                       0x0312
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_RESOLUTION                                     0x0313
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_RANGE_MAXIMUM                                  0x0314
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_RANGE_MINIMUM                                  0x0315
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE                                0x0316 // NAry
//begin reporting state selectors
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_SEL                  0x0840 // Sel
#define HID_DRIVER_USAGE_REPORTING_STATE_ON_NONE_SEL       HID_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_SEL
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_SEL                 0x0841 // Sel
#define HID_DRIVER_USAGE_REPORTING_STATE_ON_ALL_SEL        HID_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_SEL
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_SEL           0x0842 // Sel
#define HID_DRIVER_USAGE_REPORTING_STATE_ON_THRESHOLD_SEL  HID_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_SEL
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_WAKE_SEL             0x0843 // Sel
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_WAKE_SEL            0x0844 // Sel
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_WAKE_SEL      0x0845 // Sel
//end reporting state selectors
//begin reporting state enums
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_ENUM                 0x01 // Enum
#define HID_DRIVER_USAGE_REPORTING_STATE_ON_NONE_ENUM      HID_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_ENUM
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_ENUM                0x02 // Enum
#define HID_DRIVER_USAGE_REPORTING_STATE_ON_ALL_ENUM       HID_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_ENUM
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_ENUM          0x03 // Enum
#define HID_DRIVER_USAGE_REPORTING_STATE_ON_THRESHOLD_ENUM HID_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_ENUM
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_NO_EVENTS_WAKE_ENUM            0x04 // Enum
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_ALL_EVENTS_WAKE_ENUM           0x05 // Enum
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_REPORTING_STATE_THRESHOLD_EVENTS_WAKE_ENUM     0x06 // Enum
//end reporting state enums
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SAMPLING_RATE                                  0x0317
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_RESPONSE_CURVE                                 0x0318
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE                                    0x0319 // NAry
//begin power state selectors
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_UNDEFINED_SEL                      0x0850 // Sel
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_SEL                  0x0851 // Sel
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_SEL                   0x0852 // Sel
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D2_STANDBY_WITH_WAKE_SEL           0x0853 // Sel
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D3_SLEEP_WITH_WAKE_SEL             0x0854 // Sel
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_SEL                   0x0855 // Sel
//end power state selectors
//begin power state enums
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_UNDEFINED_ENUM                     0x01 // Enum
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D0_FULL_POWER_ENUM                 0x02 // Enum
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D1_LOW_POWER_ENUM                  0x03 // Enum
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D2_STANDBY_WITH_WAKE_ENUM          0x04 // Enum
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D3_SLEEP_WITH_WAKE_ENUM            0x05 // Enum
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_POWER_STATE_D4_POWER_OFF_ENUM                  0x06 // Enum
//end power state enums


//data type location
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION                                           0x0400
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_DESIRED_ACCURACY                          0x0401
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ALTITUDE_ANTENNA_SEALEVEL                 0x0402
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_DIFFERENTIAL_REFERENCE_STATION_ID         0x0403
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ALTITIDE_ELIPSOID_ERROR                   0x0404
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ALTITIDE_ELIPSOID                         0x0405
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ALTITUDE_SEALEVEL_ERROR                   0x0406
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ALTITUDE_SEALEVEL                         0x0407
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_DGPS_DATA_AGE                             0x0408
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ERROR_RADIUS                              0x0409
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_FIX_QUALITY                               0x040A // NAry
//begin fix quality selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_QUALITY_NO_FIX                                 0x0870 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_QUALITY_GPS                                    0x0871 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_QUALITY_DGPS                                   0x0872 // Sel
//end fix quality selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_FIX_TYPE                                  0x040B // NAary
//begin fix type selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_TYPE_NO_FIX                                    0x0880 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_TYPE_GPS_SPS_MODE_FIX_VALID                    0x0881 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_TYPE_DGPS_SPS_MODE_FIX_VALID                   0x0882 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_TYPE_GPS_PPS_MODE_FIX_VALID                    0x0883 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_TYPE_REAL_TIME_KINEMATIC                       0x0884 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_TYPE_FLOAT_RTK                                 0x0885 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_TYPE_ESTIMATED_DEAD_RECKONING                  0x0886 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_TYPE_MANUAL_INPUT_MODE                         0x0887 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_FIX_TYPE_SIMULATOR_MODE                            0x0888 // Sel
//end fix type selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_GEOIDAL_SEPARATION                        0x040C
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_GPS_OPERATION_MODE                        0x040D // NAry
//begin gps operation mode selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_GPS_OP_MODE_MANUAL                                 0x0890 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_GPS_OP_MODE_AUTOMATIC                              0x0891 // Sel
//end gps operation mode selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_GPS_SELECTION_MODE                        0x040E // NAry
//begin gps selection mode selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_GPS_SEL_MODE_AUTONOMOUS                            0x08A0 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_GPS_SEL_MODE_DGPS                                  0x08A1 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_GPS_SEL_MODE_ESTIMATED_DEAD_RECKONING              0x08A2 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_GPS_SEL_MODE_MANUAL_INPUT                          0x08A3 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_GPS_SEL_MODE_SIMULATOR                             0x08A4 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_GPS_SEL_MODE_DATA_NOT_VALID                        0x08A5 // Sel
//end gps selection mode selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_GPS_STATUS                                0x040F // NAry
//begin gps status selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_GPS_STATUS_DATA_VALID                              0x08B0 // Sel
#define HID_DRIVER_USAGE_SENSOR_DATA_GPS_STATUS_DATA_NOT_VALID                          0x08B1 // Sel
//end gps status selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_POSITION_DILUTION_OF_PRECISION            0x0410
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_HORIZONTAL_DILUTION_OF_PRECISION          0x0411
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_VERTICAL_DILUTION_OF_PRECISION            0x0412
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_LATITUDE                                  0x0413
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_LONGITUDE                                 0x0414
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_TRUE_HEADING                              0x0415
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_MAGNETIC_HEADING                          0x0416
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_MAGNETIC_VARIATION                        0x0417
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SPEED                                     0x0418
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW                        0x0419
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW_AZIMUTH                0x041A
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW_ELEVATION              0x041B
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW_ID                     0x041C
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW_PRNS                   0x041D
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_IN_VIEW_STN_RATIO              0x041E
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_USED_COUNT                     0x041F
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_SATELLITES_USED_PRNS                      0x0420
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_NMEA_SENTENCE                             0x0421
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ADDRESS_LINE_1                            0x0422
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_ADDRESS_LINE_2                            0x0423
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_CITY                                      0x0424
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_STATE_OR_PROVINCE                         0x0425
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_COUNTRY_OR_REGION                         0x0426
#define HID_DRIVER_USAGE_SENSOR_DATA_LOCATION_POSTAL_CODE                               0x0427
//property usages (get/set feature report)
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_LOCATION                                       0x042A
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_LOCATION_DESIRED_ACCURACY                      0x042B // NAry
//begin location desired accuracy selectors
#define HID_DRIVER_USAGE_SENSOR_DESIRED_ACCURACY_DEFAULT                                0x0860 // Sel
#define HID_DRIVER_USAGE_SENSOR_DESIRED_ACCURACY_HIGH                                   0x0861 // Sel
#define HID_DRIVER_USAGE_SENSOR_DESIRED_ACCURACY_MEDIUM                                 0x0862 // Sel
#define HID_DRIVER_USAGE_SENSOR_DESIRED_ACCURACY_LOW                                    0x0863 // Sel
//end location desired accuracy selectors

//data type environmental
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL                                      0x0430
#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_ATMOSPHERIC_PRESSURE                 0x0431
#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_REFERENCE_PRESSURE                   0x0432
#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_RELATIVE_HUMIDITY                    0x0433
#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_TEMPERATURE                          0x0434
#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_WIND_DIRECTION                       0x0435
#define HID_DRIVER_USAGE_SENSOR_DATA_ENVIRONMENTAL_WIND_SPEED                           0x0436
//property usages (get/set feature report)
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_ENVIRONMENTAL                                  0x0440
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_ENVIRONMENTAL_REFERENCE_PRESSURE               0x0441

//data type motion
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION                                             0x0450
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_STATE                                       0x0451
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ACCELERATION                                0x0452
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ACCELERATION_X_AXIS                         0x0453
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ACCELERATION_Y_AXIS                         0x0454
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ACCELERATION_Z_AXIS                         0x0455
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY                            0x0456
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_X_AXIS                     0x0457
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Y_AXIS                     0x0458
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_VELOCITY_Z_AXIS                     0x0459
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_POSITION                            0x045A
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_POSITION_X_AXIS                     0x045B
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_POSITION_Y_AXIS                     0x045C
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_ANGULAR_POSITION_Z_AXIS                     0x045D
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_SPEED                                       0x045E
#define HID_DRIVER_USAGE_SENSOR_DATA_MOTION_INTENSITY                                   0x045F

//data type orientation
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION                                        0x0470
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING                       0x0471
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING_X                     0x0472
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING_Y                     0x0473
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_HEADING_Z                     0x0474
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_MAGNETIC_NORTH             0x0475
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_COMPENSATED_TRUE_NORTH                 0x0476
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_NORTH                         0x0477
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TRUE_NORTH                             0x0478
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_DISTANCE                               0x0479
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_DISTANCE_X                             0x047A
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_DISTANCE_Y                             0x047B
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_DISTANCE_Z                             0x047C
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_DISTANCE_OUT_OF_RANGE                  0x047D
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TILT                                   0x047E
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TILT_X                                 0x047F
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TILT_Y                                 0x0480
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_TILT_Z                                 0x0481
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_ROTATION_MATRIX                        0x0482
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_QUATERNION                             0x0483
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX                          0x0484
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_X_AXIS                   0x0485
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Y_AXIS                   0x0486
#define HID_DRIVER_USAGE_SENSOR_DATA_ORIENTATION_MAGNETIC_FLUX_Z_AXIS                   0x0487

//data type mechanical
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL                                         0x0490
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_BOOLEAN_SWITCH_STATE                    0x0491
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_BOOLEAN_SWITCH_ARRAY_STATES             0x0492
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_MULTIVAL_SWITCH_VALUE                   0x0493
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_FORCE                                   0x0494
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_ABSOLUTE_PRESSURE                       0x0495
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_GAUGE_PRESSURE                          0x0496
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_STRAIN                                  0x0497
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_WEIGHT                                  0x0498
//property usages (get/set feature report)
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_MECHANICAL                                     0x04A0
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_MECHANICAL_VIBRATION_STATE                     0x04A1
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_VIBRATION_SPEED_FORWARD                 0x04A2
#define HID_DRIVER_USAGE_SENSOR_DATA_MECHANICAL_VIBRATION_SPEED_BACKWARD                0x04A3

//data type biometric
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC                                          0x04B0
#define HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC_HUMAN_PRESENCE                           0x04B1
#define HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC_HUMAN_PROXIMITY_RANGE                    0x04B2
#define HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC_HUMAN_PROXIMITY_OUT_OF_RANGE             0x04B3
#define HID_DRIVER_USAGE_SENSOR_DATA_BIOMETRIC_HUMAN_TOUCH_STATE                        0x04B4

//data type light sensor
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT                                              0x04D0
#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_ILLUMINANCE                                  0x04D1
#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_COLOR_TEMPERATURE                            0x04D2
#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY                                 0x04D3
#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY_X                               0x04D4
#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CHROMATICITY_Y                               0x04D5
#define HID_DRIVER_USAGE_SENSOR_DATA_LIGHT_CONSUMER_IR_SENTENCE                         0x04D6
//property usages (get/set feature report)
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_LIGHT                                          0x04E0
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_LIGHT_CONSUMER_IR_SENTENCE_SEND                0x04E1

//data type scanner
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_SCANNER                                            0x04F0
#define HID_DRIVER_USAGE_SENSOR_DATA_SCANNER_RFID_TAG                                   0x04F1
#define HID_DRIVER_USAGE_SENSOR_DATA_SCANNER_NFC_SENTENCE_RECEIVE                       0x04F2
//property usages (get/set feature report)
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SCANNER                                        0x04F8
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_SCANNER_NFC_SENTENCE_SEND                      0x04F9

//data type electrical
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL                                         0x0500
#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_CAPACITANCE                             0x0501
#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_CURRENT                                 0x0502
#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_POWER                                   0x0503
#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_INDUCTANCE                              0x0504
#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_RESISTANCE                              0x0505
#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_VOLTAGE                                 0x0506
#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_FREQUENCY                               0x0507
#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_PERIOD                                  0x0508
#define HID_DRIVER_USAGE_SENSOR_DATA_ELECTRICAL_PERCENT_OF_RANGE                        0x0509

//data type time
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME                                               0x0520
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_YEAR                                          0x0521
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_MONTH                                         0x0522
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_DAY                                           0x0523
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_DAY_OF_WEEK                                   0x0524
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_HOUR                                          0x0525
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_MINUTE                                        0x0526
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_SECOND                                        0x0527
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_MILISECOND                                    0x0528
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_TIMESTAMP                                     0x0529
#define HID_DRIVER_USAGE_SENSOR_DATA_TIME_JULIAN_DAY_OF_YEAR                            0x052A
//property usages (get/set feature report)
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME                                           0x0530
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME_TIME_ZONE_OFFSET_FROM_UTC                 0x0531
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME_TIME_ZONE_NAME                            0x0532
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME_DAYLIGHT_SAVINGS_TIME_OBSERVED            0x0533
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME_TIME_TRIM_ADJUSTMENT                      0x0534
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_TIME_ARM_ALARM                                 0x0535

//data type custom
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM                                             0x0540
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_USAGE                                       0x0541
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_BOOLEAN_ARRAY                               0x0542
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE                                       0x0543
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_1                                     0x0544
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_2                                     0x0545
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_3                                     0x0546
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_4                                     0x0547
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_5                                     0x0548
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_6                                     0x0549
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_7                                     0x054A
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_8                                     0x054B
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_9                                     0x054C
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_10                                    0x054D
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_11                                    0x054E
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_12                                    0x054F
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_13                                    0x0550
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_14                                    0x0551
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_15                                    0x0552
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_16                                    0x0553
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_17                                    0x0554
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_18                                    0x0555
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_19                                    0x0556
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_20                                    0x0557
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_21                                    0x0558
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_22                                    0x0559
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_23                                    0x055A
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_24                                    0x055B
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_25                                    0x055C
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_26                                    0x055D
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_27                                    0x055E
#define HID_DRIVER_USAGE_SENSOR_DATA_CUSTOM_VALUE_28                                    0x055F

//data type generic
//data field usages (input report)
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC                                            0x0560
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_GUID_OR_PROPERTYKEY                        0x0561
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_CATEGORY_GUID                              0x0562
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_TYPE_GUID                                  0x0563
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_EVENT_PROPERTYKEY                          0x0564
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_PROPERTY_PROPERTYKEY                       0x0565
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_DATAFIELD_PROPERTYKEY                      0x0566
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_EVENT                                      0x0567
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_PROPERTY                                   0x0568
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_DATAFIELD                                  0x0569
#define HID_DRIVER_USAGE_SENSOR_DATA_ENUMERATOR_TABLE_ROW_INDEX                         0x056A
#define HID_DRIVER_USAGE_SENSOR_DATA_ENUMERATOR_TABLE_ROW_COUNT                         0x056B
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_GUID_OR_PROPERTYKEY_KIND                   0x056C // NAry
//begin GorPK kind selectors
#define HID_DRIVER_USAGE_SENSOR_GORPK_KIND_CATEGORY                                     0x08D0 // Sel
#define HID_DRIVER_USAGE_SENSOR_GORPK_KIND_TYPE                                         0x08D1 // Sel
#define HID_DRIVER_USAGE_SENSOR_GORPK_KIND_EVENT                                        0x08D2 // Sel
#define HID_DRIVER_USAGE_SENSOR_GORPK_KIND_PROPERTY                                     0x08D3 // Sel
#define HID_DRIVER_USAGE_SENSOR_GORPK_KIND_DATAFIELD                                    0x08D4 // Sel
//end GorPK kind selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_GUID                                       0x056D
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_PROPERTYKEY                                0x056E
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_TOP_LEVEL_COLLECTION_ID                    0x056F
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_REPORT_ID                                  0x0570
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_REPORT_ITEM_POSITION_INDEX                 0x0571
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_FIRMWARE_VARTYPE                           0x0572 // NAry
//begin firmware vartype selectors
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_NULL                                0x0900 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_BOOL                                0x0901 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_UI1                                 0x0902 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_I1                                  0x0903 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_UI2                                 0x0904 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_I2                                  0x0905 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_UI4                                 0x0906 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_I4                                  0x0907 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_UI8                                 0x0908 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_I8                                  0x0909 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_R4                                  0x090A // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_R8                                  0x090B // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_WSTR                                0x090C // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_STR                                 0x090D // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_CLSID                               0x090E // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_VECTOR_VT_UI1                       0x090F // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16E0                               0x0910 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16E1                               0x0911 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16E2                               0x0912 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16E3                               0x0913 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16E4                               0x0914 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16E5                               0x0915 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16E6                               0x0916 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16E7                               0x0917 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16E8                               0x0918 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16E9                               0x0919 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16EA                               0x091A // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16EB                               0x091B // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16EC                               0x091C // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16ED                               0x091D // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16EE                               0x091E // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F16EF                               0x091F // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32E0                               0x0920 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32E1                               0x0921 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32E2                               0x0922 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32E3                               0x0923 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32E4                               0x0924 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32E5                               0x0925 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32E6                               0x0926 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32E7                               0x0927 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32E8                               0x0928 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32E9                               0x0929 // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32EA                               0x092A // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32EB                               0x092B // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32EC                               0x092C // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32ED                               0x092D // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32EE                               0x092E // Sel
#define HID_DRIVER_USAGE_SENSOR_FIRMWARE_VARTYPE_VT_F32EF                               0x092F // Sel
//end firmware vartype selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_UNIT_OF_MEASURE                            0x0573 // NAry
//begin unit of measure selectors
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_NOT_SPECIFIED                              0x0940 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_LUX                                        0x0941 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_DEGREES_KELVIN                             0x0942 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_DEGREES_CELSIUS                            0x0943 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_PASCAL                                     0x0944 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_NEWTON                                     0x0945 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_METERS_PER_SECOND                          0x0946 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_KILOGRAM                                   0x0947 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_METER                                      0x0948 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_METERS_PER_SEC_SQRD                        0x0949 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_FARAD                                      0x094A // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_AMPERE                                     0x094B // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_WATT                                       0x094C // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_HENRY                                      0x094D // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_OHM                                        0x094E // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_VOLT                                       0x094F // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_HERTZ                                      0x0950 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_BAR                                        0x0951 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_DEGREES_ANTI_CLOCKWISE                     0x0952 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_DEGREES_CLOCKWISE                          0x0953 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_DEGREES                                    0x0954 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_DEGREES_PER_SECOND                         0x0955 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_DEGREES_PER_SEC_SQRD                       0x0956 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_KNOT                                       0x0957 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_PERCENT                                    0x0958 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_SECOND                                     0x0959 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_MILLISECOND                                0x095A // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_G                                          0x095B // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_BYTES                                      0x095C // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_MILLIGAUSS                                 0x095D // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_UNIT_BITS                                       0x095E // Sel
//end unit of measure selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_UNIT_EXPONENT                              0x0574 // NAry
//begin unit exponent selectors
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_0                                      0x0970 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_1                                      0x0971 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_2                                      0x0972 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_3                                      0x0973 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_4                                      0x0974 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_5                                      0x0975 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_6                                      0x0976 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_7                                      0x0977 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_8                                      0x0978 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_9                                      0x0979 // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_A                                      0x097A // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_B                                      0x097B // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_C                                      0x097C // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_D                                      0x097D // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_E                                      0x097E // Sel
#define HID_DRIVER_USAGE_SENSOR_GENERIC_EXPONENT_F                                      0x097F // Sel
//end unit exponent selectors
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_REPORT_SIZE                                0x0575
#define HID_DRIVER_USAGE_SENSOR_DATA_GENERIC_REPORT_COUNT                               0x0576
//property usages (get/set feature report)
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_GENERIC                                        0x0580
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_ENUMERATOR_TABLE_ROW_INDEX                     0x0581
#define HID_DRIVER_USAGE_SENSOR_PROPERTY_ENUMERATOR_TABLE_ROW_COUNT                     0x0582



#define MAX_NUM_HID_USAGES     6

////////////////////////////////////////////////////////////////////////////////////
//
// Sensor-specific HID definitions
//
// These following #include files are used in the HID device firmware. For the 
// purposes of this driver, the "hid_sensor_spec_macros.h" are used to compile
// "hid_sensors_spec_report_descriptors.h" which are example HID Report Descriptors
// for the various supported sensors.
//
// These #defines are compliant with the HID Sensor Usages document HUTRR39b.pdf
// available from usbhid.org

// NOTE: the are the augmented definitions from the following document:
// "HID Sensor Usages - Annotations for Windows HID Sensor Class Driver"
//
////////////////////////////////////////////////////////////////////////////////////

#include "hid_sensor_spec_macros.h"
#include "hid_sensor_spec_report_descriptors.h"

////////////////////////////////////////////////////////////////////////////////////



