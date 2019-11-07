/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    stdafx.h

Abstract:

    Contains the precompiled headers that are common accross all GATT Service drivers

--*/

#pragma once

#include "resource.h"
#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif

#define STRSAFE_NO_DEPRECATE

#include <stdio.h>
#include <tchar.h>

#include <atlbase.h>
#include <atlcom.h>
#include <atlcoll.h>
#include <atlstr.h>
#include <math.h>

// This driver is entirely user-mode
_Analysis_mode_(_Analysis_code_type_user_code_);

// Disabling C6211 as drivers do not perform exception handling.
#pragma warning(disable: 6211)

#include <wudfddi.h>
#include <PortableDeviceTypes.h>
#include <PortableDeviceClassExtension.h>
#include <PortableDevice.h>
#include <initguid.h>
#include <propkeydef.h>
#define DEFINE_DEVSVCGUID DEFINE_GUID
#define DEFINE_DEVSVCPROPKEY DEFINE_PROPERTYKEY
#include <DeviceServices.h>
#include <Bthdef.h>
#include <BthLEDef.h>
#include <BluetoothLEApis.h>
#include <setupapi.h>
#include <Dbt.h>
#include <BluetoothApis.h>
#include <BthGuid.h>

extern HINSTANCE g_hInstance;

// Forward class declarations
class WpdObjectEnumeratorContext;
class WpdServiceMethods;
class WpdBaseDriver;
class BthLEDevice;

//
// Generic WPD Method declaration
//

// This method call indicates that an app is being activated
// {47b2ea1b-3b9a-4061-82dd-ab7e6c8f8cfc}
DEFINE_DEVSVCGUID(METHOD_AppActivated, 0x47b2ea1b,0x3b9a,0x4061,0x82, 0xdd, 0xab, 0x7e, 0x6c, 0x8f, 0x8c, 0xfc);
#define NAME_METHOD_AppActivated L"ApplicationActivated"

// This method call indicates that an app is being suspended
// {829e165a-0253-49f7-a598-9049021fb4df}
DEFINE_DEVSVCGUID(METHOD_AppSuspended, 0x829e165a,0x0253,0x49f7,0xa5, 0x98, 0x90, 0x49, 0x02, 0x1f, 0xb4, 0xdf);
#define NAME_METHOD_AppSuspended L"ApplicationSuspended"


//++
//
// This section is copied from wdm.h.  We are not including wdm.h as it conflicts
// with atlbase.h.  This is the only macro we will be using from wdm.h
//
// VOID
// RtlRetrieveUshort (
//     PUSHORT DESTINATION_ADDRESS
//     PUSHORT SOURCE_ADDRESS
//     )
//
// Routine Description:
//
// This macro retrieves a USHORT value from the SOURCE address, avoiding
// alignment faults.  The DESTINATION address is assumed to be aligned.
//
// Arguments:
//
//     DESTINATION_ADDRESS - where to store USHORT value
//     SOURCE_ADDRESS - where to retrieve USHORT value from
//
// Return Value:
//
//     none.
//
//--

#define SHORT_SIZE  (sizeof(USHORT))
#define SHORT_MASK  (SHORT_SIZE - 1)

#if defined(_AMD64_)

#define RtlRetrieveUshort(DEST_ADDRESS,SRC_ADDRESS)                     \
         *(USHORT UNALIGNED *)(DEST_ADDRESS) = *(PUSHORT)(SRC_ADDRESS)

#else

#define RtlRetrieveUshort(DEST_ADDRESS,SRC_ADDRESS)                   \
         if ((ULONG_PTR)SRC_ADDRESS & SHORT_MASK) {                       \
             ((PUCHAR) (DEST_ADDRESS))[0] = ((PUCHAR) (SRC_ADDRESS))[0];  \
             ((PUCHAR) (DEST_ADDRESS))[1] = ((PUCHAR) (SRC_ADDRESS))[1];  \
         }                                                            \
         else {                                                       \
             *((PUSHORT) DEST_ADDRESS) = *((PUSHORT) SRC_ADDRESS);    \
         }                                                            \

#endif

float FORCEINLINE
UshortToFloat(
    _In_ USHORT value
    )
{
    SHORT mantissa = 0;
    CHAR exponent = 0;

    (value & 0x07ff);

    mantissa = (value & 0x07ff);
    exponent = (value >> 12);

    return ((float)mantissa * pow((float)10.0, (float)exponent));    
}


