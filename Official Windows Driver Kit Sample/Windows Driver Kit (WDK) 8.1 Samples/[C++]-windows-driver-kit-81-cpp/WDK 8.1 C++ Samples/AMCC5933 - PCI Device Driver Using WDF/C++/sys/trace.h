/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.
    
Module Name:

    TRACE.h

Abstract:

    Header file for the debug tracing related function defintions and macros.

Environment:

    Kernel mode

--*/

#define WPP_CHECK_FOR_NULL_STRING  //to prevent exceptions due to NULL strings

#if 0 // kind of noisy...turn on for WPP problems
#if DBG
#define WppDebug(a,b) DbgPrint b;
#endif
#endif

#define WPP_LEVEL_FLAGS_LOGGER(lvl,flags) \
           WPP_LEVEL_LOGGER(flags)
               
#define WPP_LEVEL_FLAGS_ENABLED(lvl, flags) \
           (WPP_LEVEL_ENABLED(flags) && WPP_CONTROL(WPP_BIT_ ## flags).Level >= lvl)

#define WPP_CONTROL_GUIDS \
    WPP_DEFINE_CONTROL_GUID( Amcc5933TraceGuid,                          \
                             (3942B8CC, 55A4, 47AC, 9FCE, A53F7366DF76), \
                             WPP_DEFINE_BIT(AMCC_TRACE_INIT)             \
                             WPP_DEFINE_BIT(AMCC_TRACE_SHUTDOWN)         \
                             WPP_DEFINE_BIT(AMCC_TRACE_IO)               \
                           )

// Uncomment the following line if you want to get see traces in the debugger
//#define WPP_DEBUG(b) DbgPrint("AMCC-PCI:"),DbgPrint b, DbgPrint("\n")
