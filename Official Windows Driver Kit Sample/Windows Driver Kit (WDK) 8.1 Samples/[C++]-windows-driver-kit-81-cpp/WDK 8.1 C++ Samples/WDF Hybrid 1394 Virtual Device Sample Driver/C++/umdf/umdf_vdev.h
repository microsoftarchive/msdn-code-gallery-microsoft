/*++

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

    umdf_vdev.h

Abstract:

    This module contains the local type definitions for the UMDF 1394 
    virtual device driver sample.

Environment:

    Windows User-Mode Driver Framework (WUDF)

--*/

#pragma once

#ifndef _UMDF_VDEV_H_
#define _UMDF_VDEV_H_

#ifndef ARRAY_SIZE
#define ARRAY_SIZE(x) (sizeof(x) / sizeof(x[0]))
#endif

#include <wudfddi.h>
#include <specstrings.h>

//
// Forward definitions of classes in the other header files.
//

typedef class CUmdfVDev    *PCUmdfVDev;
typedef class CVDevDevice  *PCVDevDevice;

typedef class CVDevSequentialQueue   *PCVDevSequentialQueue;
typedef class CVDevParallelQueue        *PCVDevParallelQueue;

//
// Define the tracing flags.
//

#define WPP_CONTROL_GUIDS \
    WPP_DEFINE_CONTROL_GUID (    \
            Wudf1394VdevTraceGuid, (a155e98e, 147b, 4481, 9f1c, efa63eed7ced), \
                                                                            \
        WPP_DEFINE_BIT(MYDRIVER_ALL_INFO)                                   \
        WPP_DEFINE_BIT(TEST_TRACE_DRIVER)                                   \
        WPP_DEFINE_BIT(TEST_TRACE_DEVICE)                                   \
        WPP_DEFINE_BIT(TEST_TRACE_QUEUE)                                   \
        )

#define WPP_FLAG_LEVEL_LOGGER(flag, level)                                  \
    WPP_LEVEL_LOGGER(flag)

#define WPP_FLAG_LEVEL_ENABLED(flag, level)                                 \
    (WPP_LEVEL_ENABLED(flag) &&                                             \
    WPP_CONTROL(WPP_BIT_ ## flag).Level >= level)

#define WPP_LEVEL_FLAGS_LOGGER(lvl,flags) \
    WPP_LEVEL_LOGGER(flags)

#define WPP_LEVEL_FLAGS_ENABLED(lvl, flags) \
    (WPP_LEVEL_ENABLED(flags) && WPP_CONTROL(WPP_BIT_ ## flags).Level >= lvl)

//
// This comment block is scanned by the trace preprocessor to define our
// Trace function.
//
// begin_wpp config
// FUNC Trace{FLAG=MYDRIVER_ALL_INFO}(LEVEL, MSG, ...);
// FUNC TraceEvents(LEVEL, FLAGS, MSG, ...);
// end_wpp
//
#define MYDRIVER_TRACING_ID      L"Microsoft\\UMDF\\umdf1394vdev"
#define MYDRIVER_CLASS_ID   {0xd202f373, 0x3aae, 0x45ee, {0xae, 0x73, 0x45, 0x76, 0x6c, 0xef, 0x14, 0xb4}}

// begin_wpp config
// Enter();
// USESUFFIX(Enter, "%!FUNC! Enter");
// end_wpp

// begin_wpp config
// Exit();
// USESUFFIX(Exit, "%!FUNC! Exit");
// end_wpp

#define WPP__LOGGER() WPP_LEVEL_LOGGER(MYDRIVER_ALL_INFO)
#define WPP__ENABLED() WPP_LEVEL_ENABLED(MYDRIVER_ALL_INFO)

// begin_wpp config
// FUNC ExitHR(HRESULT);
// USESUFFIX (ExitHR, "%!FUNC! Exit = %!HRESULT!", HRESULT);
// end_wpp

#define WPP_HRESULT_LOGGER(HRESULT) WPP_LEVEL_LOGGER(MYDRIVER_ALL_INFO)
#define WPP_HRESULT_ENABLED(HRESULT) WPP_LEVEL_ENABLED(MYDRIVER_ALL_INFO)

__forceinline 
#ifdef _PREFAST_
__declspec(noreturn)

#endif
VOID
WdfTestNoReturn (
                 VOID)
{
    // do nothing.
}

#define WUDF_TEST_DRIVER_ASSERT(p)  \
{                                   \
    if ( !(p) )                     \
    {                               \
        DebugBreak ();               \
        WdfTestNoReturn ();          \
    }                               \
}

#define SAFE_RELEASE(p) \
{                       \
    if ((p))            \
{                       \
    (p)->Release();     \
    (p) = NULL;         \
}                       \
}


#include "umdf_vdev_com.h"
#include "umdf_vdev_driver.h"
#include "umdf_vdev_device.h"
#include "umdf_vdev_parallelqueue.h"
#include "umdf_vdev_sequentialqueue.h"

#endif

