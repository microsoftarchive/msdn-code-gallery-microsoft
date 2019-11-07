/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_trace.h

Abstract:
    Contains defines useful for tracing in the driver
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

#if DOT11_TRACE_ENABLED

    //
    // Trace framework definition when WPP based tracing is enabled
    //

    #define DBG_SERIOUS            1        // TRACE_LEVEL_CRITICAL/FATAL
    #define DBG_NORMAL             4        // TRACE_LEVEL_INFORMATION
    #define DBG_LOUD               5        // TRACE_LEVEL_VERBOSE
    #define DBG_TRACE              6        // TRACE_LEVEL_RESERVED6

    //
    // These are the various tracing components we can have. The max number is 31
    // so always keep that in mind.
    //
    // WARNING! THIS GUID IS BEING USED BY THE SAMPLE DRIVER.
    // PLEASE GENERATE YOUR OWN UNIQUE GUID AND PLACE BELOW.
    //
    // Use COMP_TESTING when you want to add messages trace through a specific
    // piece of code and not want to see trace from all other components
    //
    #define WPP_CONTROL_GUIDS                                                   \
        WPP_DEFINE_CONTROL_GUID(CtlGuid,(e49b27dd,b2f0,4571,8c6a,3271a3a3a6b9), \
            WPP_DEFINE_BIT(COMP_MISC)                                          \
            WPP_DEFINE_BIT(COMP_INIT_PNP)                                      \
            WPP_DEFINE_BIT(COMP_OID)                                           \
            WPP_DEFINE_BIT(COMP_RECV)                                          \
            WPP_DEFINE_BIT(COMP_SEND)                                          \
            WPP_DEFINE_BIT(COMP_PS_PACKETS)                                    \
            WPP_DEFINE_BIT(COMP_EVENTS)                                        \
            WPP_DEFINE_BIT(COMP_POWER)                                         \
            WPP_DEFINE_BIT(COMP_SCAN)                                          \
            WPP_DEFINE_BIT(COMP_ASSOC)                                         \
            WPP_DEFINE_BIT(COMP_DBG)                                           \
            WPP_DEFINE_BIT(COMP_TESTING)                                       \
            WPP_DEFINE_BIT(COMP_HVL)                                           \
            WPP_DEFINE_BIT(COMP_ASSOC_MGR)                                     \
        )

    #ifdef WPP_COMPID_LEVEL_ENABLED
        #undef WPP_COMPID_LEVEL_ENABLED
    #endif

    #define WPP_COMPID_LEVEL_ENABLED(CTL,LEVEL)             \
        ((WPP_CONTROL(WPP_BIT_ ## CTL).Level >= LEVEL) &&   \
        (WPP_CONTROL(WPP_BIT_ ## CTL).Flags[WPP_FLAG_NO(WPP_BIT_ ## CTL)] & WPP_MASK(WPP_BIT_ ## CTL)))

    #ifndef WPP_COMPID_LEVEL_LOGGER
    #  define WPP_COMPID_LEVEL_LOGGER(CTL,LEVEL)      (WPP_CONTROL(WPP_BIT_ ## CTL).Logger),
    #endif

    //
    // When tracing is enabled MpEntry and MpExit have to be
    // turned off. Tracing cannot understand Preprocessor
    // directives.
    //
    #define MpTrace(_Comp, _Level, Fmt)
    #define MpEntry
    #define MpExit

#else // DOT11_TRACE_ENABLED

#if DBG

    //
    // This is checked build and WPP tracing is off, so using Debug Prints instead.
    // These are slower but some people may still prefer
    // them to tracing thorugh initial debugging of code
    //

    extern ULONG GlobalDebugLevel;
    extern ULONG GlobalDebugComponents;

    //
    // Define the tracing levels
    //
    #define     DBG_OFF                 100 // Never used in a call to MpTrace
    #define     DBG_SERIOUS             10
    #define     DBG_NORMAL              45
    #define     DBG_LOUD                90
    #define     DBG_FUNCTION            95  // MpEntry/Exit
    #define     DBG_TRACE               99  // Never Set GlobalDebugLevel to this

    //
    // Define the tracing components
    //
    #define COMP_MISC                   0x00000001
    #define COMP_INIT_PNP               0x00000002
    #define COMP_OID                    0x00000004
    #define COMP_RECV                   0x00000008
    #define COMP_SEND                   0x00000010
    #define COMP_PS_PACKETS             0x00000020
    #define COMP_EVENTS                 0x00000040
    #define COMP_POWER                  0x00000080
    #define COMP_SCAN                   0x00000100
    #define COMP_DBG                    0x00000200
    #define COMP_TESTING                0x00000400
    #define COMP_ASSOC                  0x00000800
    #define COMP_HVL                    0x00001000
    #define COMP_ASSOC_MGR              0x00002000
    
    #define MpTrace(_Comp, _Level, Fmt)                     \
        if((_Comp & GlobalDebugComponents) && (_Level <= GlobalDebugLevel))  \
            DbgPrint Fmt;

    #define MpEntry                  \
        if (DBG_FUNCTION <= GlobalDebugLevel)   \
            DbgPrint(("==> " __FUNCTION__ "\n"))

    #define MpExit                  \
        if (DBG_FUNCTION <= GlobalDebugLevel)   \
            DbgPrint(("<== " __FUNCTION__ "\n"))

    //
    // Function that reads DebugLevel and Components information from
    // registry
    //
    VOID MpReadGlobalDebugMask(
        NDIS_HANDLE  NdisMiniportDriverHandle
        );

#else  // DBG

//
// WPP Tracing is OFF and this is not a checked build of the driver. All trace messages are disabled
//

#define DBG_OFF
#define DBG_TRACE
#define DBG_SERIOUS
#define DBG_NORMAL
#define DBG_LOUD
#define MpTrace(_Comp, _Level, Fmt)
#define MpEntry
#define MpExit

#endif // !DBG

#endif // !DOT11_TRACE_ENABLED
