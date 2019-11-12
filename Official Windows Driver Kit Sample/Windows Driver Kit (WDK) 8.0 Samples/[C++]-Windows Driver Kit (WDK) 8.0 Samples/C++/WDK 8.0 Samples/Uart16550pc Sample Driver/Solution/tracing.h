/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

Module Name:

    tracing.h

Abstract:

    This module contains the private tracing definitions for the 
    Serial (UART) software loopback driver.

Environment:

    kernel mode only

Revision History:


--*/

#ifndef _TRACING_H_
#define _TRACING_H_

typedef struct _WDF_DRIVER_CONFIG *PWDF_DRIVER_CONFIG;

//
// Tracing Definitions:
//
// Control GUID: 
// {D89680CC-0701-47cf-B42F-891ED0597BEE}

#define WPP_CONTROL_GUIDS                           \
    WPP_DEFINE_CONTROL_GUID(                        \
        Uart16550pcTraceGuid,                       \
        (D89680CC,0701,47cf,B42F,891ED0597BEE),     \
        WPP_DEFINE_BIT(TRACE_FLAG_TRANSMIT)         \
        WPP_DEFINE_BIT(TRACE_FLAG_RECEIVE)          \
        WPP_DEFINE_BIT(TRACE_FLAG_CONTROL)          \
        WPP_DEFINE_BIT(TRACE_FLAG_INIT)             \
        WPP_DEFINE_BIT(TRACE_FLAG_UNLOAD)           \
        WPP_DEFINE_BIT(TRACE_FLAG_INTERRUPT)        \
        WPP_DEFINE_BIT(TRACE_FLAG_WORKERDPC)        \
        WPP_DEFINE_BIT(TRACE_FLAG_REGUTIL)          \
        WPP_DEFINE_BIT(TRACE_FLAG_FLOWCONTROL)      \
        WPP_DEFINE_BIT(TRACE_FLAG_FILE)             \
        )

#define WPP_LEVEL_FLAGS_LOGGER(level,flags) WPP_LEVEL_LOGGER(flags)
#define WPP_LEVEL_FLAGS_ENABLED(level, flags) (WPP_LEVEL_ENABLED(flags) && WPP_CONTROL(WPP_BIT_ ## flags).Level >= level)

// begin_wpp config
// FUNC FuncEntry{LEVEL=TRACE_LEVEL_VERBOSE}(FLAGS);
// FUNC FuncExit{LEVEL=TRACE_LEVEL_VERBOSE}(FLAGS);
// USEPREFIX(FuncEntry, "%!STDPREFIX! [%!FUNC!] --> entry");
// USEPREFIX(FuncExit, "%!STDPREFIX! [%!FUNC!] <--");
// end_wpp

#endif // _TRACING_H_
