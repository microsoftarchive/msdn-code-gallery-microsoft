/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    stdafx.h

Abstract:

    Contains the precompiled header for the Health BloodPressure Service driver

--*/

#pragma once

#include "..\stdafx.h"

#define MYDRIVER_TRACING_ID      L"Microsoft\\Bluetooth\\WpdHealthBloodPressureService"

#define WPP_CONTROL_GUIDS \
    WPP_DEFINE_CONTROL_GUID(WpdHealthBloodPressureServiceCtlGuid,(72707F43,F5B5,4799,9913,3353F4039520), \
        WPP_DEFINE_BIT(TRACE_FLAG_ALL)                                      \
        WPP_DEFINE_BIT(TRACE_FLAG_DEVICE)                                   \
        WPP_DEFINE_BIT(TRACE_FLAG_DRIVER)                                   \
        WPP_DEFINE_BIT(TRACE_FLAG_QUEUE)                                    \
        )

//
// BEGIN HealthBloodPressureService definition
//
#define DEVICE_PROTOCOL_NAME            L"Health BloodPressure Service"
#define DEVICE_FRIENDLY_NAME            L"Bluetooth GATT Health BloodPressure Service"
#define SERVICE_OBJECT_ID               L"BTHLEGATT_HealthBloodPressureService"
#define SERVICE_PERSISTENT_UNIQUE_ID    L"BTHLEGATT_HealthBloodPressureService_{00001810-0000-1000-8000-00805F9B34FB}"
#define SERVICE_OBJECT_NAME_VALUE       L"BTHLEGATT_HealthBloodPressureService"
#define SERVICE_HUMAN_READABLE_NAME     L"Bluetooth GATT Health Blood Pressure Service"
#define SERVICE_VERSION                 L"1.0"

#define HEALTH_BloodPressure_SERVICE_UUID         0x1810
#define BloodPressure_MEASUREMENT_CHAR_UUID       0x2A35

//
// Health BloodPressure Service
// {00001810-0000-1000-8000-00805f9b34fb}
DEFINE_DEVSVCGUID(SERVICE_HealthBloodPressureService, HEALTH_BloodPressure_SERVICE_UUID, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb);

// BloodPressure measurements characteristic
// {00001810-0000-1000-8000-00805f9b34fb}.1 
DEFINE_DEVSVCPROPKEY(SERVICE_HealthBloodPressureService_Measurement, HEALTH_BloodPressure_SERVICE_UUID, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb, 1);
#define NAME_SERVICE_HealthBloodPressureService_BloodPressureMeasurement L"BloodPressureMeasurement"


// This event contains an integer parameter containing the value of the blood pressure measurement
// {0b6b15e1-1ec4-4dde-881a-cfc3e0a7a5c7}
DEFINE_DEVSVCGUID(EVENT_HealthBloodPressureService_Measurement, 0x0b6b15e1,0x1ec4,0x4dde, 0x88, 0x1a, 0xcf, 0xc3, 0xe0, 0xa7, 0xa5, 0xc7);
#define NAME_EVENT_HealthBloodPressureService_Measurement L"BloodPressureMeasurement"

// Blood Pressure Measurement event parameters
// {0b6b15e1-1ec4-4dde-881a-cfc3e0a7a5c7}.2
DEFINE_DEVSVCPROPKEY(EVENT_PARAMETER_HealthBloodPressureService_Measurement_TimeStamp,              0x0b6b15e1,0x1ec4,0x4dde, 0x88, 0x1a, 0xcf, 0xc3, 0xe0, 0xa7, 0xa5, 0xc7, 2);
DEFINE_DEVSVCPROPKEY(EVENT_PARAMETER_HealthBloodPressureService_Measurement_Type,                   0x0b6b15e1,0x1ec4,0x4dde, 0x88, 0x1a, 0xcf, 0xc3, 0xe0, 0xa7, 0xa5, 0xc7, 3);
DEFINE_DEVSVCPROPKEY(EVENT_PARAMETER_HealthBloodPressureService_Measurement_Systolic,               0x0b6b15e1,0x1ec4,0x4dde, 0x88, 0x1a, 0xcf, 0xc3, 0xe0, 0xa7, 0xa5, 0xc7, 4);
DEFINE_DEVSVCPROPKEY(EVENT_PARAMETER_HealthBloodPressureService_Measurement_Diastolic,              0x0b6b15e1,0x1ec4,0x4dde, 0x88, 0x1a, 0xcf, 0xc3, 0xe0, 0xa7, 0xa5, 0xc7, 5);
DEFINE_DEVSVCPROPKEY(EVENT_PARAMETER_HealthBloodPressureService_Measurement_MeanArterialPressure,   0x0b6b15e1,0x1ec4,0x4dde, 0x88, 0x1a, 0xcf, 0xc3, 0xe0, 0xa7, 0xa5, 0xc7, 6);


BOOLEAN FORCEINLINE
IsHealthBloodPressureServiceUuid(BTH_LE_UUID uuid)
{   
    BTH_LE_UUID correctUuid = {0};
    
    correctUuid.IsShortUuid = TRUE;
    correctUuid.Value.ShortUuid = HEALTH_BloodPressure_SERVICE_UUID;

    return IsBthLEUuidMatch(uuid, correctUuid);
}


BOOLEAN FORCEINLINE
IsBloodPressureMeasurementCharacteristic(BTH_LE_UUID uuid)
{
    BTH_LE_UUID correctUuid = {0};
    
    correctUuid.IsShortUuid = TRUE;
    correctUuid.Value.ShortUuid = BloodPressure_MEASUREMENT_CHAR_UUID;
    
    return IsBthLEUuidMatch(uuid, correctUuid);
}
//
// END HealthBloodPressureService definition
//

// Forward class declarations
class HealthBloodPressureService;
class HealthBloodPressureServiceContent;


typedef HealthBloodPressureService WpdGattService;
typedef HealthBloodPressureServiceContent WpdGattServiceContent;
#define SERVICE_GattService SERVICE_HealthBloodPressureService


//
// Includes
//
#include "helpers.h"
#include "WpdBluetoothGattServiceDriver.h"
#include "AbstractDeviceContent.h"
#include "AbstractGattService.h"
#include "HealthBloodPressureService.h"
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
#include "HealthBloodPressureServiceContent.h"

