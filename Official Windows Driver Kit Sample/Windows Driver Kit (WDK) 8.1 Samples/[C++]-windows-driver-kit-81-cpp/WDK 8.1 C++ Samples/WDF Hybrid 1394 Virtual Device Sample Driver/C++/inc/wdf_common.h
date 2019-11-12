/*++

Copyright (c) Microsoft Corporation, All Rights Reserved

Module Name:

   wdf_common.h

Abstract:

    This module contains the common declarations shared by UMDF, KDMF drivers
    and user applications for the 1394 vdev hybrid device sample.

Environment:

    user and kernel


--*/

#pragma once

#ifndef _WDF_COMMON_H_
#define _WDF_COMMON_H_

#ifdef __cplusplus
extern "C" {
#endif
    
    // {0xabfaaa91, 0x4a19, 0x4119, 0x88, 0x31, 0xcc, 0x80, 0x22, 0x9f, 0x48, 0x14}
    DEFINE_GUID (GUID_UMDF_VDEV_HYBRID, 0xabfaaa91, 0x4a19, 0x4119, 
        0x88, 0x31, 0xcc, 0x80, 0x22, 0x9f, 0x48, 0x14);

    // {0x15a86bbf, 0x52c5, 0x4e77, 0xa9, 0xf1, 0xe5, 0x4, 0xbd, 0xeb, 0xbf, 0xa7}
    DEFINE_GUID (GUID_KMDF_VDEV, 0x15a86bbf, 0x52c5, 0x4e77, 
        0xa9, 0xf1, 0xe5, 0x4, 0xbd, 0xeb, 0xbf, 0xa7);

    //-----------------------------------------------------------------------------
    // 4127 -- Conditional Expression is Constant warning
    //-----------------------------------------------------------------------------

#define WHILE(a) \
    while(__pragma(warning(disable:4127)) a __pragma(warning(disable:4127)))

    //
    // Memory pool tag
    //
#define POOLTAG_KMDF_VDEV   'mkvV'
#define POOLTAG_UMDF_VDEV 'muvV'


    //
    // Define the IOCTLS for driver(s) and applications to use.
    //
#define DIAG1394_IOCTL_INDEX                            0x0800


#define IOCTL_ALLOCATE_ADDRESS_RANGE                    \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 0,       \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_FREE_ADDRESS_RANGE                        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 1,       \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ASYNC_READ        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 2,       \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ASYNC_WRITE       \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 3,       \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ASYNC_LOCK        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 4,       \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_ALLOCATE_BANDWIDTH     \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 5,       \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_ALLOCATE_CHANNEL     \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 6,       \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_ALLOCATE_RESOURCES    \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 7,       \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_ATTACH_BUFFERS     \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 8,       \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_DETACH_BUFFERS    \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 9,       \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_FREE_BANDWIDTH     \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 10,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_FREE_CHANNEL         \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 11,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_FREE_RESOURCES    \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 12,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_LISTEN    \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 13,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_QUERY_CURRENT_CYCLE_TIME        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 14,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_QUERY_RESOURCES     \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 15,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_SET_CHANNEL_BANDWIDTH      \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 16,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_STOP      \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 17,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_TALK      \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 18,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_GET_LOCAL_HOST_INFORMATION     \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 19,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_GET_1394_ADDRESS_FROM_DEVICE_OBJECT      \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 20,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_CONTROL       \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 21,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_GET_MAX_SPEED_BETWEEN_DEVICES        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 22,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_SET_DEVICE_XMIT_PROPERTIES       \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 23,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_GET_CONFIGURATION_INFORMATION        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 24,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_BUS_RESET       \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 25,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_GET_GENERATION_COUNT        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 26,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_SEND_PHY_CONFIGURATION_PACKET      \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 27,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_BUS_RESET_NOTIFICATION    \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 28,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ASYNC_STREAM        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 29,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_SET_LOCAL_HOST_INFORMATION  \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 30,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_SET_ADDRESS_DATA        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 40,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_GET_ADDRESS_DATA        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 41,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_BUS_RESET_NOTIFY      \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 50,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_GET_DIAG_VERSION      \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 51,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)

#define IOCTL_ISOCH_MODIFY_STREAM_PROPERTIES        \
            CTL_CODE( FILE_DEVICE_UNKNOWN,  \
            DIAG1394_IOCTL_INDEX + 52,      \
            METHOD_BUFFERED,                \
            FILE_ANY_ACCESS)


#ifdef __cplusplus
}
#endif

#endif // #ifndef _WDF_COMMON_H_
