/*++

Copyright (c) Microsoft Corporation

Module Name:

    kmdf_vdev_debug.h

Abstract:

--*/

#define _DRIVERNAME_  "KMDF1394VDEV"

#define WPP_CONTROL_GUIDS \
    WPP_DEFINE_CONTROL_GUID(Kmdf1394Vdev,(549F0E03, 9907, 483b, BC93, DB482567BE4A),  \
        WPP_DEFINE_BIT(TRACE_FLAG_DRIVER) \
        WPP_DEFINE_BIT(TRACE_FLAG_PNP)    \
        WPP_DEFINE_BIT(TRACE_FLAG_API)    \
        WPP_DEFINE_BIT(TRACE_FLAG_IOCTL)  \
        WPP_DEFINE_BIT(TRACE_FLAG_ASYNC)  \
        WPP_DEFINE_BIT(TRACE_FLAG_ISOCH)  \
        WPP_DEFINE_BIT(TRACE_FLAG_UTILS) )
// {549F0E03-9907-483b-BC93-DB482567BE4A}

#define WPP_LEVEL_FLAGS_LOGGER(lvl,flags) WPP_LEVEL_LOGGER(flags)
#define WPP_LEVEL_FLAGS_ENABLED(lvl,flags) (WPP_LEVEL_ENABLED(flags) && WPP_CONTROL(WPP_BIT_ ## flags).Level >= lvl)

// begin_wpp config
// Enter();
// USESUFFIX(Enter, "%!FUNC! Enter");
// end_wpp

// begin_wpp config
// Exit();
// USESUFFIX(Exit, "%!FUNC! Exit");
// end_wpp

#define WPP__LOGGER() WPP_LEVEL_LOGGER(TRACE_FLAG_DRIVER)
#define WPP__ENABLED() WPP_LEVEL_ENABLED(TRACE_FLAG_DRIVER)

// begin_wpp config
// FUNC ExitS(STATUS);
// USESUFFIX (ExitS, "%!FUNC! Exit = %!STATUS!", STATUS);
// end_wpp

#define WPP_STATUS_LOGGER(STATUS) WPP_LEVEL_LOGGER(TRACE_FLAG_DRIVER)
#define WPP_STATUS_ENABLED(STATUS) WPP_LEVEL_ENABLED(TRACE_FLAG_DRIVER)


