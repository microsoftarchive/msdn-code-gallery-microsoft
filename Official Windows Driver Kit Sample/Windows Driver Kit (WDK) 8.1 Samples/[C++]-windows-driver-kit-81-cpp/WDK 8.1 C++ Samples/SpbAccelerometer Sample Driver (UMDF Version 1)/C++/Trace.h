/*++

// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

    Trace.h

Abstract:

    This module contains the tracing information for the SPB 
    accelerometer driver.

--*/

#ifndef _TRACE_H_
#define _TRACE_H_

//
// Tracing Information
//

#define MYDRIVER_TRACING_ID L"Microsoft\\Sensor\\SpbAccelerometerDriver"

// Tracing Definitions:
//
// Control GUID: 
// (DA1A372F,74C1,4F2B,B3A5,632F486F72A3)
#define WPP_CONTROL_GUIDS                             \
    WPP_DEFINE_CONTROL_GUID(                          \
        SpbAccelerometerTraceGuid,                    \
        (DA1A372F,74C1,4F2B,B3A5,632F486F72A3),       \
        WPP_DEFINE_BIT(MYDRIVER_ALL_INFO)             \
        )

#define WPP_FLAG_LEVEL_LOGGER(flag, level)            \
    WPP_LEVEL_LOGGER(flag)

#define WPP_FLAG_LEVEL_ENABLED(flag, level)           \
    (WPP_LEVEL_ENABLED(flag) &&                       \
        WPP_CONTROL(WPP_BIT_ ## flag).Level >= level  \
        )

//
// This comment block is scanned by the trace preprocessor to define our
// Trace function.
//
// begin_wpp config
// FUNC Trace{FLAG=MYDRIVER_ALL_INFO}(LEVEL, MSG, ...);
// FUNC FuncEntry{FLAG=MYDRIVER_ALL_INFO, LEVEL=TRACE_LEVEL_VERBOSE}(...);
// FUNC FuncExit{FLAG=MYDRIVER_ALL_INFO, LEVEL=TRACE_LEVEL_VERBOSE}(...);
// USEPREFIX(Trace, "%!STDPREFIX! [%!FUNC!] ");
// USEPREFIX(FuncEntry, "%!STDPREFIX! [%!FUNC!] --> entry");
// USEPREFIX(FuncExit, "%!STDPREFIX! [%!FUNC!] <--");
// end_wpp
//

#endif // _TRACE_H_