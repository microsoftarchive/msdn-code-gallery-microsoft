/*++

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

    Internal.h

Abstract:

    This module contains the local type definitions for the UMDF 
    driver.

Environment:

    Windows User-Mode Driver Framework (WUDF)

--*/

#pragma once

#ifndef ARRAY_SIZE
#define ARRAY_SIZE(x) (sizeof(x) / sizeof(x[0]))
#endif

#define UMDF_USING_NTSTATUS
#include <ntstatus.h>

//
// Include the WUDF headers
//

#include "wudfddi.h"

//
// Use specstrings for in/out annotation of function parameters.
//

#include "specstrings.h"

//
// Forward definitions of classes in the other header files.
//

typedef class CMyDriver *PCMyDriver;
typedef class CMyDevice *PCMyDevice;
typedef class CMyQueue  *PCMyQueue;
typedef class CMyManualQueue  *PCMyManualQueue;

//
// Define the tracing flags.
//
#if !defined(EVENT_TRACING)
    
    //
    // TODO: These defines are missing in evntrace.h
    // in some DDK build environments (XP).
    //
#if !defined(TRACE_LEVEL_NONE)
  #define TRACE_LEVEL_NONE        0
  #define TRACE_LEVEL_CRITICAL    1
  #define TRACE_LEVEL_FATAL       1
  #define TRACE_LEVEL_ERROR       2
  #define TRACE_LEVEL_WARNING     3
  #define TRACE_LEVEL_INFORMATION 4
  #define TRACE_LEVEL_VERBOSE     5
  #define TRACE_LEVEL_RESERVED6   6
  #define TRACE_LEVEL_RESERVED7   7
  #define TRACE_LEVEL_RESERVED8   8
  #define TRACE_LEVEL_RESERVED9   9
#endif
    
    VOID
    Trace(
        _In_ ULONG   DebugPrintLevel,
        _In_ PCSTR   DebugMessage,
        ...
        );
    
#define WPP_INIT_TRACING(_ID_)
#define WPP_CLEANUP()
    
#else

#define WPP_CONTROL_GUIDS                                                   \
    WPP_DEFINE_CONTROL_GUID(                                                \
        MyDriverTraceControl, (83cdcaa5,ce52,43f2,aa2d,5f5a84e22213),       \
                                                                            \
        WPP_DEFINE_BIT(MYDRIVER_ALL_INFO)                                   \
        )

#define WPP_FLAG_LEVEL_LOGGER(flag, level)                                  \
    WPP_LEVEL_LOGGER(flag)

#define WPP_FLAG_LEVEL_ENABLED(flag, level)                                 \
    (WPP_LEVEL_ENABLED(flag) &&                                             \
     WPP_CONTROL(WPP_BIT_ ## flag).Level >= level)

#endif

//
// This comment block is scanned by the trace preprocessor to define our
// Trace function.
//
// begin_wpp config
// FUNC Trace{FLAG=MYDRIVER_ALL_INFO}(LEVEL, MSG, ...);
// end_wpp
//

//
// Driver specific #defines
//

#define MYDRIVER_TRACING_ID      L"Microsoft\\UMDF\\wudfvhidmini"
#define MYDRIVER_COM_DESCRIPTION L"UMDF HID minidriver"
#define MYDRIVER_CLASS_ID        {0x522d8dbc, 0x520d, 0x4d7e, {0x8f, 0x53, 0x92, 0x0e, 0x5c, 0x86, 0x7e, 0x6c}}
#define _DRIVER_NAME_            "WudfVhidmini: "
//
// Include the type specific headers.
//
// 
// Hidport.h in DDK\wdm path references KM objects as well so redefine them 
// approrpiately to compile this UMDF driver. They are not used in UMDF driver.
//
#define PDRIVER_OBJECT  PVOID
#define PDEVICE_OBJECT  PVOID
#define PUNICODE_STRING PVOID
#include "hidport.h"  // located in $(DDK_INC_PATH)/wdm

#include "strsafe.h"

//
// Ioctl definitions
//
#include "winioctl.h"
#include "common.h"
#include "comsup.h"
#include "driver.h"
#include "device.h"
#include "queue.h"


__forceinline 
#ifdef _PREFAST_
__declspec(noreturn)
#endif
VOID
WdfTestNoReturn(
    VOID
    )
{
    // do nothing.
}

#define WUDF_SAMPLE_DRIVER_ASSERT(p)  \
{                                   \
    if ( !(p) )                     \
    {                               \
        DebugBreak();               \
        WdfTestNoReturn();          \
    }                               \
}

#define SAFE_RELEASE(p)     {if ((p)) { (p)->Release(); (p) = NULL; }}

//
// Macro for updating IOCTL type to use METHOD_NEITHER.
// Least significamt 2 bits (0-1) are used for IoType
// Just changing IoType would cause conflict with other HID ioctls, so we
// update Iotcl function code by bit-adding IoType to function code.
//
#define TO_TEMP_IOCTL(_IOCTL_)  (_IOCTL_ | ((_IOCTL_ & 0x3ff) << 2))
#define TO_METHOD_NEITHER(_IOCTL_)    \
    ((TO_TEMP_IOCTL(_IOCTL_) & (~3)) | METHOD_NEITHER)




