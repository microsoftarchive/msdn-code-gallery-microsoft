/*++

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

    Internal.h

Abstract:

    This module contains the local type definitions for the SPB 
    accelerometer driver.

--*/

#ifndef _INTERNAL_H_
#define _INTERNAL_H_

#pragma once

#define UMDF_USING_NTSTATUS

#include <windows.h>
#include <winternl.h>
#include <ntstatus.h>
#include <atlbase.h>
#include <atlcom.h>
#include <atlcoll.h>
#include <atlstr.h>
#include <gpio.h>

_Analysis_mode_(_Analysis_code_type_user_driver_);  // Macro letting the compiler know this is not a kernel driver (this will help surpress needless warnings)

// Common WPD, UMDF, and WDM headers
#include <devioctl.h>
#include <initguid.h>
#include <propkeydef.h>
#include <propvarutil.h>
#include "PortableDeviceTypes.h"
#include "PortableDeviceClassExtension.h"
#include "PortableDevice.h"
#include "acpiioct.h"

// Headers for Sensor specific defines and WpdCommands
#include "Sensors.h"
#include <SensorsClassExtension.h>

// Headers for internal interfaces
#include "SensorDevice.h"
#include "Request.h"

// TODO: Remove these
#include "SpbRequest.h"
#include "AccelerometerDevice.h"

// Resource hub defines
#define RESHUB_USE_HELPER_ROUTINES
#include "reshub.h"

#include "Trace.h"

// One forward-declare that pretty much everyone is going to need to know about
class CMyDevice;

//
// Test properties
//

//2f808247-7cdb-4319-bf5b-e16ab67f7344
DEFINE_GUID(SENSOR_PROPERTY_SPB_TEST_GUID,              \
    0X2F808247, 0X7CDB, 0X4319, 0XBF, 0X5B, 0XE1, 0X6A, 0XB6, 0X7F, 0X73, 0X44);
DEFINE_PROPERTYKEY(SENSOR_PROPERTY_TEST_REGISTER,   \
    0X2F808247, 0X7CDB, 0X4319, 0XBF, 0X5B, 0XE1, 0X6A, 0XB6, 0X7F, 0X73, 0X44, 2); //[VT_UI4]
DEFINE_PROPERTYKEY(SENSOR_PROPERTY_TEST_DATA_SIZE,  \
    0X2F808247, 0X7CDB, 0X4319, 0XBF, 0X5B, 0XE1, 0X6A, 0XB6, 0X7F, 0X73, 0X44, 3); //[VT_UI4]
DEFINE_PROPERTYKEY(SENSOR_PROPERTY_TEST_DATA,       \
    0X2F808247, 0X7CDB, 0X4319, 0XBF, 0X5B, 0XE1, 0X6A, 0XB6, 0X7F, 0X73, 0X44, 4); //[VT_VECTOR|VT_UI1]

///////////////////////////////////////////////////////////////////
// Common macro expansions that are used throughout the project
#define SAFE_RELEASE(p)     {if ((p)) { (p)->Release(); (p) = nullptr; }}
#define ARRAY_SIZE(x)       (sizeof(x) / sizeof(x[0]))

#endif // _INTERNAL_H_
