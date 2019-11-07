/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    stdafx.h

Abstract:

    Contains the precompiled header for the Health Thermometer Service driver

--*/

#pragma once

#include "..\stdafx.h"

#define MYDRIVER_TRACING_ID      L"Microsoft\\Bluetooth\\WpdHealthThermometerService"

#define WPP_CONTROL_GUIDS \
    WPP_DEFINE_CONTROL_GUID(WpdHealthThermometerServiceCtlGuid,(72707F43,F5B5,4799,9913,3353F4039520), \
        WPP_DEFINE_BIT(TRACE_FLAG_ALL)                                      \
        WPP_DEFINE_BIT(TRACE_FLAG_DEVICE)                                   \
        WPP_DEFINE_BIT(TRACE_FLAG_DRIVER)                                   \
        WPP_DEFINE_BIT(TRACE_FLAG_QUEUE)                                    \
        )

//
// BEGIN HealthThermometerService definition
//
#define DEVICE_PROTOCOL_NAME            L"Health Thermometer Service"
#define DEVICE_FRIENDLY_NAME            L"Bluetooth GATT Health Thermometer Service"
#define SERVICE_OBJECT_ID               L"BTHLEGATT_HealthThermometerService"
#define SERVICE_PERSISTENT_UNIQUE_ID    L"BTHLEGATT_HealthThermometerService_{00001809-0000-1000-8000-00805F9B34FB}"
#define SERVICE_OBJECT_NAME_VALUE       L"BTHLEGATT_HealthThermometerService"
#define SERVICE_HUMAN_READABLE_NAME     L"Bluetooth GATT Health Thermometer Service"
#define SERVICE_VERSION                 L"1.0"

#define HEALTH_THERMOMETER_SERVICE_UUID         0x1809
#define TEMPERATURE_MEASUREMENT_CHAR_UUID       0x2A1C
#define TEMPERATURE_TYPE_CHAR_UUID              0x2A1D

//
// Health Thermometer Service
// {00001809-0000-1000-8000-00805f9b34fb}
DEFINE_DEVSVCGUID(SERVICE_HealthThermometerService, HEALTH_THERMOMETER_SERVICE_UUID, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb);

// Temperature measurements characteristic
// {00001809-0000-1000-8000-00805f9b34fb}.1 
DEFINE_DEVSVCPROPKEY(SERVICE_HealthThermometerService_TemperatureMeasurement, TEMPERATURE_MEASUREMENT_CHAR_UUID, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb, 1);
#define NAME_SERVICE_HealthThermometerService_TemperatureMeasurement L"TemperatureMeasurement"


// This event contains a timestamp parameter and an integer value containing the temperature measurement
// {2835789E-3F7E-45FA-9728-F9EF85164FA7}
DEFINE_DEVSVCGUID(EVENT_HealthThermometerService_TemperatureMeasurement, 0x2835789E, 0x3F7E, 0x45FA, 0x97, 0x28, 0xF9, 0xEF, 0x85, 0x16, 0x4F, 0xA7);
#define NAME_EVENT_HealthThermometerService_TemperatureMeasurement L"TemperatureMeasurement"


// Temperature Measurement parameter
// {2835789E-3F7E-45FA-9728-F9EF85164FA7}.2
DEFINE_DEVSVCPROPKEY(EVENT_PARAMETER_HealthTemperatureService_Measurement_TimeStamp, 0x2835789E, 0x3F7E, 0x45FA, 0x97, 0x28, 0xF9, 0xEF, 0x85, 0x16, 0x4F, 0xA7, 2);
DEFINE_DEVSVCPROPKEY(EVENT_PARAMETER_HealthTemperatureService_Measurement_Value,     0x2835789E, 0x3F7E, 0x45FA, 0x97, 0x28, 0xF9, 0xEF, 0x85, 0x16, 0x4F, 0xA7, 3);


BOOLEAN FORCEINLINE
IsHealthThermometerServiceUuid(BTH_LE_UUID uuid)
{   
    BTH_LE_UUID correctUuid = {0};
    
    correctUuid.IsShortUuid = TRUE;
    correctUuid.Value.ShortUuid = HEALTH_THERMOMETER_SERVICE_UUID;

    return IsBthLEUuidMatch(uuid, correctUuid);
}


BOOLEAN FORCEINLINE
IsTemperatureMeasurementCharacteristic(BTH_LE_UUID uuid)
{
    BTH_LE_UUID correctUuid = {0};
    
    correctUuid.IsShortUuid = TRUE;
    correctUuid.Value.ShortUuid = TEMPERATURE_MEASUREMENT_CHAR_UUID;
    
    return IsBthLEUuidMatch(uuid, correctUuid);
}
//
// END HealthThermometerService definition
//

// Forward class declarations
class HealthThermometerService;
class HealthThermometerServiceContent;

typedef HealthThermometerService WpdGattService;
typedef HealthThermometerServiceContent WpdGattServiceContent;
#define SERVICE_GattService SERVICE_HealthThermometerService


//
// Includes
//
#include "helpers.h"
#include "WpdBluetoothGattServiceDriver.h"
#include "AbstractDeviceContent.h"
#include "AbstractGattService.h"
#include "HealthThermometerService.h"
#include "BthLEDeviceContent.h"
#include "BthLEDevice.h"
#include "WpdObjectEnum.h"
#include "WpdObjectProperties.h"
#include "WpdCapabilities.h"
#include "WpdServiceCapabilities.h"
#include "WpdServiceMethods.h"
#include "WpdService.h"
#include "WpdBaseDriver.h"
#include "driver.h"
#include "device.h"
#include "queue.h"
#include "WppTracing.h"
#include "HealthThermometerServiceContent.h"


