//-----------------------------------------------------------------------
// <copyright file="Common.h" company="Microsoft">
//      Copyright (c) 2006 Microsoft Corporation. All rights reserved.
// </copyright>
//
// Module:
//      Common.h
//
// Description:
//
//-----------------------------------------------------------------------


#pragma once


#include <windows.h>

#include <atlbase.h>
extern CComModule _Module; // Required by ATLCOM.h
#include <atlcom.h>


#include "Driverspecs.h"
_Analysis_mode_(_Analysis_code_type_user_driver_); // Macro letting the compiler know this is not a kernel driver (this will help surpress needless warnings)


// One forward-declare that pretty much everyone is going to need to know about
class CWSSDevice;
typedef CComObject<CWSSDevice> *WSSDevicePtr;

// The following are custom WPD commands that can be sent to the driver
// Base GUID: 6BDE5134-3988-498A-98BA-841607FECBE7
const PROPERTYKEY CUSTOM_COMMAND_INVERT_COLORS             = {0x6BDE5134, 0x3988, 0x498A, 0x98, 0xBA, 0x84, 0x16, 0x07, 0xFE, 0xCB, 0xE7, 1};
const PROPERTYKEY CUSTOM_COMMAND_INVERT_COLORS_SETVALUE    = {0x6BDE5134, 0x3988, 0x498A, 0x98, 0xBA, 0x84, 0x16, 0x07, 0xFE, 0xCB, 0xE7, 2}; // [VT_UI4]
const PROPERTYKEY CUSTOM_COMMAND_CHANGE_SOMETHING          = {0x6BDE5134, 0x3988, 0x498A, 0x98, 0xBA, 0x84, 0x16, 0x07, 0xFE, 0xCB, 0xE7, 3};
const PROPERTYKEY CUSTOM_COMMAND_CHANGE_SOMETHING_SETVALUE = {0x6BDE5134, 0x3988, 0x498A, 0x98, 0xBA, 0x84, 0x16, 0x07, 0xFE, 0xCB, 0xE7, 4}; // [VT_UI4]
