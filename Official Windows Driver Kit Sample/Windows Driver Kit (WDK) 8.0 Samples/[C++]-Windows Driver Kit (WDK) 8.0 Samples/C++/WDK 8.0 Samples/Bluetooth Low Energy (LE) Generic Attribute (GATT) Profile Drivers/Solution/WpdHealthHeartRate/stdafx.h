/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    stdafx.h

Abstract:

    Contains the precompiled header for the Health HeartRate Service driver

--*/

#pragma once

//
// Include the common headers
//
#include "..\stdafx.h"

#define MYDRIVER_TRACING_ID      L"Microsoft\\Bluetooth\\WpdHealthHeartRateService"

#define WPP_CONTROL_GUIDS \
    WPP_DEFINE_CONTROL_GUID(WpdHealthHeartRateServiceCtlGuid,(72707F43,F5B5,4799,9913,3353F4039520), \
        WPP_DEFINE_BIT(TRACE_FLAG_ALL)                                      \
        WPP_DEFINE_BIT(TRACE_FLAG_DEVICE)                                   \
        WPP_DEFINE_BIT(TRACE_FLAG_DRIVER)                                   \
        WPP_DEFINE_BIT(TRACE_FLAG_QUEUE)                                    \
        )

//
// BEGIN HealthHeartRateService definition
//
#define DEVICE_PROTOCOL_NAME            L"Health HeartRate Service"
#define DEVICE_FRIENDLY_NAME            L"Bluetooth GATT Health HeartRate Service"
#define SERVICE_OBJECT_ID               L"BTHLEGATT_HealthHeartRateService"
#define SERVICE_PERSISTENT_UNIQUE_ID    L"BTHLEGATT_HealthHeartRateService_{00001809-0000-1000-8000-00805F9B34FB}"
#define SERVICE_OBJECT_NAME_VALUE       L"BTHLEGATT_HealthHeartRateService"
#define SERVICE_HUMAN_READABLE_NAME     L"Bluetooth GATT Health HeartRate Service"
#define SERVICE_VERSION                 L"1.0"

#define HEALTH_HEARTRATE_SERVICE_UUID         0x180D
#define HEARTRATE_MEASUREMENT_CHAR_UUID       0x2A37

//
// Health HeartRate Service
// {00001809-0000-1000-8000-00805f9b34fb}
DEFINE_DEVSVCGUID(SERVICE_HealthHeartRateService, HEALTH_HEARTRATE_SERVICE_UUID, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb);

// HeartRate measurements characteristic
// {00001809-0000-1000-8000-00805f9b34fb}.1 
DEFINE_DEVSVCPROPKEY(SERVICE_HealthHeartRateService_HeartRateMeasurement, HEALTH_HEARTRATE_SERVICE_UUID, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb, 1);
#define NAME_SERVICE_HealthHeartRateService_HeartRateMeasurement L"HeartRateMeasurement"


// This method submits a ReadMeasurement request.
// {0b6b15e1-1ec4-4dde-881a-cfc3e0a7a5c7}
DEFINE_DEVSVCGUID(METHOD_ReadHeartRateMeasurement, 0x0b6b15e1,0x1ec4,0x4dde, 0x88, 0x1a, 0xcf, 0xc3, 0xe0, 0xa7, 0xa5, 0xc7);
#define NAME_METHOD_ReadHeartRateMeasurement L"ReadHeartRateMeasurement"

// Heart Rate Measurement parameter
// {0b6b15e1-1ec4-4dde-881a-cfc3e0a7a5c7}.2
DEFINE_DEVSVCPROPKEY(METHOD_PARAMETER_HealthHeartRateService_Measurement_Result,    0x0b6b15e1,0x1ec4,0x4dde, 0x88, 0x1a, 0xcf, 0xc3, 0xe0, 0xa7, 0xa5, 0xc7, 2);
DEFINE_DEVSVCPROPKEY(METHOD_PARAMETER_HealthHeartRateService_Measurement_TimeStamp, 0x0b6b15e1,0x1ec4,0x4dde, 0x88, 0x1a, 0xcf, 0xc3, 0xe0, 0xa7, 0xa5, 0xc7, 3);
DEFINE_DEVSVCPROPKEY(METHOD_PARAMETER_HealthHeartRateService_Measurement_Rate,      0x0b6b15e1,0x1ec4,0x4dde, 0x88, 0x1a, 0xcf, 0xc3, 0xe0, 0xa7, 0xa5, 0xc7, 4);

BOOLEAN FORCEINLINE
IsHealthHeartRateServiceUuid(BTH_LE_UUID uuid)
{   
    BTH_LE_UUID correctUuid = {0};
    
    correctUuid.IsShortUuid = TRUE;
    correctUuid.Value.ShortUuid = HEALTH_HEARTRATE_SERVICE_UUID;

    return IsBthLEUuidMatch(uuid, correctUuid);
}


BOOLEAN FORCEINLINE
IsHeartRateMeasurementCharacteristic(BTH_LE_UUID uuid)
{
    BTH_LE_UUID correctUuid = {0};
    
    correctUuid.IsShortUuid = TRUE;
    correctUuid.Value.ShortUuid = HEARTRATE_MEASUREMENT_CHAR_UUID;
    
    return IsBthLEUuidMatch(uuid, correctUuid);
}
//
// END HealthHeartRateService definition
//

// Forward class declarations
class HealthHeartRateService;
class HealthHeartRateServiceContent;

typedef HealthHeartRateService WpdGattService;
typedef HealthHeartRateServiceContent WpdGattServiceContent;
#define SERVICE_GattService SERVICE_HealthHeartRateService


//
// Includes
//
#include "helpers.h"
#include "WpdBluetoothGattServiceDriver.h"
#include "AbstractDeviceContent.h"
#include "AbstractGattService.h"
#include "HealthHeartRateService.h"
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
#include "HealthHeartRateServiceContent.h"

