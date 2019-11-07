/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    stdafx.h

Abstract:

    Contains the precompiled header for the TI CC2540 Simple Keys Service driver

--*/

#pragma once

#include "..\stdafx.h"

#define MYDRIVER_TRACING_ID      L"Microsoft\\Bluetooth\\WpdTICC2540SimpleKeysService"

#define WPP_CONTROL_GUIDS \
    WPP_DEFINE_CONTROL_GUID(WpdTICC2540SimpleKeysServiceCtlGuid,(f0cc34b3,a482,4dc0,b978,b5cf42aec4fd), \
        WPP_DEFINE_BIT(TRACE_FLAG_ALL)                                      \
        WPP_DEFINE_BIT(TRACE_FLAG_DEVICE)                                   \
        WPP_DEFINE_BIT(TRACE_FLAG_DRIVER)                                   \
        WPP_DEFINE_BIT(TRACE_FLAG_QUEUE)                                    \
        )

//
// BEGIN TI CC2540 Simple Key Service definition
//
#define DEVICE_PROTOCOL_NAME            L"Texas Instrument CC2540"
#define DEVICE_FRIENDLY_NAME            L"Texas Instrument CC2540 Mini Development Kit"
#define SERVICE_OBJECT_ID               L"BTHLEGATT_TICC2540SimpleKeysService"
#define SERVICE_PERSISTENT_UNIQUE_ID    L"BTHLEGATT_TICC2540SimpleKeysService_{0000FFE0-0000-1000-8000-00805F9B34FB}"
#define SERVICE_OBJECT_NAME_VALUE       L"BTHLEGATT_TICC2540SimpleKeysService"
#define SERVICE_HUMAN_READABLE_NAME     L"TI CC2540 Simple Keys Service"
#define SERVICE_VERSION                 L"1.0"
#define SIMPLE_KEYS_SERVICE_UUID             0x0000FFE0
#define KEY_PRESS_STATE_CHARACTERISTIC_UUID  0x0000FFE1

//
// Simple Keys Service
// {0000FFE0-0000-1000-8000-00805f9b34fb}
DEFINE_DEVSVCGUID(SERVICE_TICC2540SimpleKeysService, SIMPLE_KEYS_SERVICE_UUID, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb);

// key press state characteristic
// {0000FFE0-0000-1000-8000-00805f9b34fb}.1 
DEFINE_DEVSVCPROPKEY(SERVICE_TICC2540SimpleKeysService_KeyPressState, SIMPLE_KEYS_SERVICE_UUID, 0x0000, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5f, 0x9b, 0x34, 0xfb, 1);
#define NAME_SERVICE_TICC2540SimpleKeysService_KeyPressState L"Key Press State"

// key pressed event
// This event contains an integer parameter containing the value of the key press
// {E1E085FF-97A7-4A4C-839C-7609BF0D873C}
DEFINE_DEVSVCGUID(EVENT_TICC2540SimpleKeysService_KeyPressed, 0xE1E085FF, 0x97A7, 0x4A4C, 0x83, 0x9C, 0x76, 0x09, 0xBF, 0x0D, 0x87, 0x3C);
#define NAME_EVENT_TICC2540SimpleKeysService_KeyPressed L"KeyPressed"

// key pressed value event parameter
// {E1E085FF-97A7-4A4C-839C-7609BF0D873C}.2
DEFINE_DEVSVCPROPKEY(EVENT_PARAMETER_TICC2540SimpleKeysService_KeyPressValue, 0xE1E085FF, 0x97A7, 0x4A4C, 0x83, 0x9C, 0x76, 0x09, 0xBF, 0x0D, 0x87, 0x3C, 2);

BOOLEAN FORCEINLINE
IsTICC2540SimpleKeysServiceUuid(BTH_LE_UUID uuid)
{   
    BTH_LE_UUID correctUuid = {0};
    
    correctUuid.IsShortUuid = TRUE;
    correctUuid.Value.ShortUuid = SIMPLE_KEYS_SERVICE_UUID;

    return IsBthLEUuidMatch(uuid, correctUuid);
}


BOOLEAN FORCEINLINE
IsKeyPressStateCharacteristic(BTH_LE_UUID uuid)
{
    BTH_LE_UUID correctUuid = {0};
    
    correctUuid.IsShortUuid = TRUE;
    correctUuid.Value.ShortUuid = KEY_PRESS_STATE_CHARACTERISTIC_UUID;
    
    return IsBthLEUuidMatch(uuid, correctUuid);
}
//
// END TI CC2540 Simple Key Service definition
//

// Forward class declarations
class TICC2540SimpleKeysService;
class TICC2540SimpleKeysServiceContent;

typedef TICC2540SimpleKeysService WpdGattService;
typedef TICC2540SimpleKeysServiceContent WpdGattServiceContent;
#define SERVICE_GattService SERVICE_TICC2540SimpleKeysService


//
// Includes
//
#include "helpers.h"
#include "WpdBluetoothGattServiceDriver.h"
#include "AbstractDeviceContent.h"
#include "AbstractGattService.h"
#include "TICC2540SimpleKeysService.h"
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
#include "TICC2540SimpleKeysServiceContent.h"

