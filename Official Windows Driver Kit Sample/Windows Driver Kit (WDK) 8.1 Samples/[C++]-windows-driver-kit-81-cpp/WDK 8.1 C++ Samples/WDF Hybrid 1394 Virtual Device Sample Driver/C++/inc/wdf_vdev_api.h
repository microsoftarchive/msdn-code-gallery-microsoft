/*++

Copyright (c) Microsoft Corporation

Module Name:

    wdf_vdev_api.h

Abstract:
       
       Reworking of the old 1394 API header file to fit a WDF hybrid driver stack.

--*/

#pragma once


//
// Disable warnings for nonstandard bit field types.
// It is valid for a bit field to be a type other than an int
// but the W4 option flags and throws an error in this case
//

#if _MSC_VER >= 1200

#pragma warning(push)

#endif

#pragma warning(disable:4214) // nonstandard extension used : bit field types other than int


#ifndef _WDF_VDEV_API_H_
#define _WDF_VDEV_API_H_



#ifdef __cplusplus
extern "C" {
#endif

#define WM_DEVICE_CHANGE                        (WM_USER+301)
#define WM_BUS_RESET                            (WM_USER+302)

#define NOTIFY_DEVICE_CHANGE                    WM_DEVICE_CHANGE
#define NOTIFY_BUS_RESET                        WM_BUS_RESET


//
// flags used for allocateaddressrange
//
#define ASYNC_ALLOC_USE_MDL                     1
#define ASYNC_ALLOC_USE_FIFO                    2
#define ASYNC_ALLOC_USE_NONE                    3


#if !defined (_KMDF1394VDEV_DRIVER_)
//
// Definition of flags for AllocateAddressRange Irb
//
#define BIG_ENDIAN_ADDRESS_RANGE                1

//
// Definition of fulAccessType for AllocateAddressRange
//
#define ACCESS_FLAGS_TYPE_READ                  1
#define ACCESS_FLAGS_TYPE_WRITE                 2
#define ACCESS_FLAGS_TYPE_LOCK                  4
#define ACCESS_FLAGS_TYPE_BROADCAST             8

//
// Definition of fulNotificationOptions for AllocateAddressRange
//
#define NOTIFY_FLAGS_NEVER                      0
#define NOTIFY_FLAGS_AFTER_READ                 1
#define NOTIFY_FLAGS_AFTER_WRITE                2
#define NOTIFY_FLAGS_AFTER_LOCK                 4

//
// Definitions of Lock transaction types
//
#define LOCK_TRANSACTION_MASK_SWAP              1
#define LOCK_TRANSACTION_COMPARE_SWAP           2
#define LOCK_TRANSACTION_FETCH_ADD              3
#define LOCK_TRANSACTION_LITTLE_ADD             4
#define LOCK_TRANSACTION_BOUNDED_ADD            5
#define LOCK_TRANSACTION_WRAP_ADD               6

//
// Definition of fulFlags in Async Read/Write/Lock requests
//

#define ASYNC_FLAGS_NONINCREMENTING             1


//
// flag instucts the port driver to NOT take an int for checking the status
// of this transaction. Always return success...
//

#define ASYNC_FLAGS_NO_STATUS               0x00000002

//
// Definitions of levels of Host controller information
//
#define GET_HOST_UNIQUE_ID                      1
#define GET_HOST_CAPABILITIES                   2
#define GET_POWER_SUPPLIED                      3
#define GET_PHYS_ADDR_ROUTINE                   4
#define GET_HOST_CONFIG_ROM                     5
#define GET_HOST_CSR_CONTENTS                   6
#define GET_HOST_DMA_CAPABILITIES                7


//
// Definitions of capabilities in Host info level 2
//
#define HOST_INFO_PACKET_BASED                  1
#define HOST_INFO_STREAM_BASED                  2
#define HOST_INFO_SUPPORTS_ISOCH_STRIPPING      4
#define HOST_INFO_SUPPORTS_START_ON_CYCLE       8
#define HOST_INFO_SUPPORTS_RETURNING_ISO_HDR    16
#define HOST_INFO_SUPPORTS_ISO_HDR_INSERTION    32


//
// Definitions of Bus Reset flags (used when Bus driver asks Port driver
// to perform a bus reset)
//
#define BUS_RESET_FLAGS_PERFORM_RESET           1
#define BUS_RESET_FLAGS_FORCE_ROOT              2

//
// Definitions of flags for GetMaxSpeedBetweenDevices and
// Get1394AddressFromDeviceObject
//
#define USE_LOCAL_NODE                          1

//
// Definition of flags for BusResetNotification Irb
//
#define REGISTER_NOTIFICATION_ROUTINE           1
#define DEREGISTER_NOTIFICATION_ROUTINE         2
//
// Definitions of Speed flags used throughout 1394 Bus APIs
//
#define SPEED_FLAGS_100                         0x01
#define SPEED_FLAGS_200                         0x02
#define SPEED_FLAGS_400                         0x04
#define SPEED_FLAGS_800                         0x08
#define SPEED_FLAGS_1600                        0x10
#define SPEED_FLAGS_3200                        0x20

#define SPEED_FLAGS_FASTEST                     0x80000000
//
// Definitions of Isoch Allocate Resources flags
//
#define RESOURCE_USED_IN_LISTENING              1
#define RESOURCE_USED_IN_TALKING                2
#define RESOURCE_BUFFERS_CIRCULAR               4
#define RESOURCE_STRIP_ADDITIONAL_QUADLETS      8
#define RESOURCE_TIME_STAMP_ON_COMPLETION       16
#define RESOURCE_SYNCH_ON_TIME                  32
#define RESOURCE_USE_PACKET_BASED               64

//
// Definitions of Isoch Descriptor flags
//
#define DESCRIPTOR_SYNCH_ON_SY                  0x00000001
#define DESCRIPTOR_SYNCH_ON_TAG                 0x00000002
#define DESCRIPTOR_SYNCH_ON_TIME                0x00000004
#define DESCRIPTOR_USE_SY_TAG_IN_FIRST          0x00000008
#define DESCRIPTOR_TIME_STAMP_ON_COMPLETION     0x00000010
#define DESCRIPTOR_PRIORITY_TIME_DELIVERY       0x00000020
#define DESCRIPTOR_HEADER_SCATTER_GATHER        0x00000040

//
// Definitions of Isoch synchronization flags
//
#define SYNCH_ON_SY                             DESCRIPTOR_SYNCH_ON_SY
#define SYNCH_ON_TAG                            DESCRIPTOR_SYNCH_ON_TAG
#define SYNCH_ON_TIME                           DESCRIPTOR_SYNCH_ON_TIME

//
// flags for the SetPortProperties request
//
#define SET_LOCAL_HOST_PROPERTIES_NO_CYCLE_STARTS       0x00000001
#define SET_LOCAL_HOST_PROPERTIES_GAP_COUNT             0x00000002
#define SET_LOCAL_HOST_PROPERTIES_MODIFY_CROM           0x00000003

//
// definition of Flags for SET_LOCAL_HOST_PROPERTIES_MODIFY_CROM
//
#define SLHP_FLAG_ADD_CROM_DATA         0x01
#define SLHP_FLAG_REMOVE_CROM_DATA      0x02

//
// Various Interesting 1394 IEEE 1212 locations
//
#define INITIAL_REGISTER_SPACE_HI               0xffff


#define TOPOLOGY_MAP_LOCATION                   0xf0001000
#define SPEED_MAP_LOCATION                      0xf0002000

typedef struct _ISOCH_RESOURCES
{
    LIST_ENTRY List;
    HANDLE hIsochResource;
}ISOCH_RESOURCES, *PISOCH_RESOURCES;

//
// These are redefines from 1394.h
//
//
// 1394 Cycle Time format
//
typedef struct _CYCLE_TIME {
    ULONG               CL_CycleOffset:12;      // Bits 0-11
    ULONG               CL_CycleCount:13;       // Bits 12-24
    ULONG               CL_SecondCount:7;       // Bits 25-31
} CYCLE_TIME, *PCYCLE_TIME;

//
// 1394 Address Offset format (48 bit addressing)
//
typedef struct _ADDRESS_OFFSET {
    USHORT              Off_High;
    ULONG               Off_Low;
} ADDRESS_OFFSET, *PADDRESS_OFFSET;

//
// 1394 Node Address format
//
typedef struct _NODE_ADDRESS {
    USHORT              NA_Node_Number:6;       // Bits 10-15
    USHORT              NA_Bus_Number:10;       // Bits 0-9
} NODE_ADDRESS, *PNODE_ADDRESS;

//
// 1394 I/O Address format
//
typedef struct _IO_ADDRESS {
    NODE_ADDRESS        IA_Destination_ID;
    ADDRESS_OFFSET      IA_Destination_Offset;
} IO_ADDRESS, *PIO_ADDRESS;

//
// 1394 Self ID packet format
//
typedef struct _SELF_ID {
    ULONG               SID_Phys_ID:6;          // Byte 0 - Bits 0-5
    ULONG               SID_Packet_ID:2;        // Byte 0 - Bits 6-7
    ULONG               SID_Gap_Count:6;        // Byte 1 - Bits 0-5
    ULONG               SID_Link_Active:1;      // Byte 1 - Bit 6
    ULONG               SID_Zero:1;             // Byte 1 - Bit 7
    ULONG               SID_Power_Class:3;      // Byte 2 - Bits 0-2
    ULONG               SID_Contender:1;        // Byte 2 - Bit 3
    ULONG               SID_Delay:2;            // Byte 2 - Bits 4-5
    ULONG               SID_Speed:2;            // Byte 2 - Bits 6-7
    ULONG               SID_More_Packets:1;     // Byte 3 - Bit 0
    ULONG               SID_Initiated_Rst:1;    // Byte 3 - Bit 1
    ULONG               SID_Port3:2;            // Byte 3 - Bits 2-3
    ULONG               SID_Port2:2;            // Byte 3 - Bits 4-5
    ULONG               SID_Port1:2;            // Byte 3 - Bits 6-7
} SELF_ID, *PSELF_ID;

//
// Additional 1394 Self ID packet format (only used when More bit is on)
//
typedef struct _SELF_ID_MORE {
    ULONG               SID_Phys_ID:6;          // Byte 0 - Bits 0-5
    ULONG               SID_Packet_ID:2;        // Byte 0 - Bits 6-7
    ULONG               SID_PortA:2;            // Byte 1 - Bits 0-1
    ULONG               SID_Reserved2:2;        // Byte 1 - Bits 2-3
    ULONG               SID_Sequence:3;         // Byte 1 - Bits 4-6
    ULONG               SID_One:1;              // Byte 1 - Bit 7
    ULONG               SID_PortE:2;            // Byte 2 - Bits 0-1
    ULONG               SID_PortD:2;            // Byte 2 - Bits 2-3
    ULONG               SID_PortC:2;            // Byte 2 - Bits 4-5
    ULONG               SID_PortB:2;            // Byte 2 - Bits 6-7
    ULONG               SID_More_Packets:1;     // Byte 3 - Bit 0
    ULONG               SID_Reserved3:1;        // Byte 3 - Bit 1
    ULONG               SID_PortH:2;            // Byte 3 - Bits 2-3
    ULONG               SID_PortG:2;            // Byte 3 - Bits 4-5
    ULONG               SID_PortF:2;            // Byte 3 - Bits 6-7
} SELF_ID_MORE, *PSELF_ID_MORE;

//
// 1394 Topology Map format
//
typedef struct _TOPOLOGY_MAP {
    USHORT              TOP_Length;             // number of quadlets in map
    USHORT              TOP_CRC;                // 16 bit CRC defined by 1212
    ULONG               TOP_Generation;         // Generation number
    USHORT              TOP_Node_Count;         // Node count
    USHORT              TOP_Self_ID_Count;      // Number of Self IDs
    SELF_ID             TOP_Self_ID_Array[1];    // Array of Self IDs
} TOPOLOGY_MAP, *PTOPOLOGY_MAP;

//
// 1394 Speed Map format
//
typedef struct _SPEED_MAP {
    USHORT              SPD_Length;             // number of quadlets in map
    USHORT              SPD_CRC;                // 16 bit CRC defined by 1212
    ULONG               SPD_Generation;         // Generation number
    UCHAR               SPD_Speed_Code[4032];
} SPEED_MAP, *PSPEED_MAP;

//
// Definitions of the structures that correspond to the Host info levels
//
typedef struct _GET_LOCAL_HOST_INFO1 {
    LARGE_INTEGER       UniqueId;
} GET_LOCAL_HOST_INFO1, *PGET_LOCAL_HOST_INFO1;

typedef struct _GET_LOCAL_HOST_INFO2 {
    ULONG               HostCapabilities;
    ULONG               MaxAsyncReadRequest;
    ULONG               MaxAsyncWriteRequest;
} GET_LOCAL_HOST_INFO2, *PGET_LOCAL_HOST_INFO2;

typedef struct _GET_LOCAL_HOST_INFO3 {
    ULONG               deciWattsSupplied;
    ULONG               Voltage;                    // x10 -> +3.3 == 33
                                                    // +5.0 == 50,+12.0 == 120
                                                    // etc.
} GET_LOCAL_HOST_INFO3, *PGET_LOCAL_HOST_INFO3;

typedef struct _GET_LOCAL_HOST_INFO4 {
    PVOID               PhysAddrMappingRoutine;
    PVOID               Context;
} GET_LOCAL_HOST_INFO4, *PGET_LOCAL_HOST_INFO4;

//
// the caller can set ConfigRomLength to zero, issue the request, which will
// be failed with STATUS_INVALID_BUFFER_SIZE and the ConfigRomLength will be set
// by the port driver to the proper length. The caller can then re-issue the request
// after it has allocated a buffer for the configrom with the correct length
//
typedef struct _GET_LOCAL_HOST_INFO5 {
    PVOID                   ConfigRom;
    ULONG                   ConfigRomLength;
} GET_LOCAL_HOST_INFO5, *PGET_LOCAL_HOST_INFO5;

typedef struct _GET_LOCAL_HOST_INFO6 {
    ADDRESS_OFFSET          CsrBaseAddress;
    ULONG                   CsrDataLength;
    UCHAR                   CsrDataBuffer[1];
} GET_LOCAL_HOST_INFO6, *PGET_LOCAL_HOST_INFO6;

typedef struct _GET_LOCAL_HOST_INFO7
{
    ULONG                   HostDmaCapabilities;
    ULARGE_INTEGER          MaxDmaBufferSize;
} GET_LOCAL_HOST_INFO7, *PGET_LOCAL_HOST_INFO7;


//
// Definitions of the structures that correspond to the Host info levels
//
typedef struct _SET_LOCAL_HOST_PROPS2 {
    ULONG       GapCountLowerBound;
} SET_LOCAL_HOST_PROPS2, *PSET_LOCAL_HOST_PROPS2;

typedef struct _SET_LOCAL_HOST_PROPS3 {
    ULONG       fulFlags;
    HANDLE      hCromData;
    ULONG       nLength;
    UCHAR       Buffer[1];
} SET_LOCAL_HOST_PROPS3, *PSET_LOCAL_HOST_PROPS3;

//
// 1394 Phy Configuration packet format
//
typedef struct _PHY_CONFIGURATION_PACKET {
    ULONG               PCP_Phys_ID:6;          // Byte 0 - Bits 0-5
    ULONG               PCP_Packet_ID:2;        // Byte 0 - Bits 6-7
    ULONG               PCP_Gap_Count:6;        // Byte 1 - Bits 0-5
    ULONG               PCP_Set_Gap_Count:1;    // Byte 1 - Bit 6
    ULONG               PCP_Force_Root:1;       // Byte 1 - Bit 7
    ULONG               PCP_Reserved1:8;        // Byte 2 - Bits 0-7
    ULONG               PCP_Reserved2:8;        // Byte 3 - Bits 0-7
    ULONG               PCP_Inverse;            // Inverse quadlet
} PHY_CONFIGURATION_PACKET, *PPHY_CONFIGURATION_PACKET;

#endif // KDMF1394VDEV_DRIVER

//
// 1394 Add/Remove Virtual Device format
//
typedef struct _VIRT_DEVICE {
    ULONG           fulFlags;
    ULARGE_INTEGER  InstanceID;
    PSTR            DeviceID;
} VIRT_DEVICE, *PVIRT_DEVICE;

//
// struct used to pass in with IOCTL_ALLOCATE_ADDRESS_RANGE
//
typedef struct _ALLOCATE_ADDRESS_RANGE {
    ULONG           fulAllocateFlags;
    ULONG           fulFlags;
    ULONG           nLength;
    ULONG           MaxSegmentSize;
    ULONG           fulAccessType;
    ULONG           fulNotificationOptions;
    ADDRESS_OFFSET  Required1394Offset;
    HANDLE          hAddressRange;
    UCHAR           Data[1];
    LIST_ENTRY     AsyncAddrList;
} ALLOCATE_ADDRESS_RANGE, *PALLOCATE_ADDRESS_RANGE;

//
// struct used to pass in with IOCTL_ASYNC_READ
//
typedef struct _ASYNC_READ {
    ULONG           bGetGeneration;
    IO_ADDRESS      DestinationAddress;
    ULONG           nNumberOfBytesToRead;
    ULONG           nBlockSize;
    ULONG           fulFlags;
    ULONG           ulGeneration;
    UCHAR           Data[1];
} ASYNC_READ, *PASYNC_READ;

//
// struct used to pass in with IOCTL_SET_ADDRESS_DATA
//
typedef struct _SET_ADDRESS_DATA {
    HANDLE          hAddressRange;
    ULONG           nLength;
    ULONG           ulOffset;
    UCHAR           Data[1];
} SET_ADDRESS_DATA, *PSET_ADDRESS_DATA,
    GET_ADDRESS_DATA, *PGET_ADDRESS_DATA;

//
// struct used to pass in with IOCTL_ASYNC_WRITE
//
typedef struct _ASYNC_WRITE {
    ULONG           bGetGeneration;
    IO_ADDRESS      DestinationAddress;
    ULONG           nNumberOfBytesToWrite;
    ULONG           nBlockSize;
    ULONG           fulFlags;
    ULONG           ulGeneration;
    UCHAR           Data[1];
} ASYNC_WRITE, *PASYNC_WRITE;

//
// struct used to pass in with IOCTL_ASYNC_LOCK
//
typedef struct _ASYNC_LOCK {
    ULONG           bGetGeneration;
    IO_ADDRESS      DestinationAddress;
    ULONG           nNumberOfArgBytes;
    ULONG           nNumberOfDataBytes;
    ULONG           fulTransactionType;
    ULONG           fulFlags;
    ULONG           Arguments[2];
    ULONG           DataValues[2];
    ULONG           ulGeneration;
    ULONG           Buffer[2];
} ASYNC_LOCK, *PASYNC_LOCK;

//
// struct used to pass in with IOCTL_ASYNC_STREAM
//
typedef struct _ASYNC_STREAM {
    ULONG           nNumberOfBytesToStream;
    ULONG           fulFlags;
    ULONG           ulTag;
    ULONG           nChannel;
    ULONG           ulSynch;
    ULONG           nSpeed;
    UCHAR           Data[1];
} ASYNC_STREAM, *PASYNC_STREAM;

//
// struct used to pass in with IOCTL_ISOCH_ALLOCATE_BANDWIDTH
//
typedef struct _ISOCH_ALLOCATE_BANDWIDTH {
    ULONG           nMaxBytesPerFrameRequested;
    ULONG           fulSpeed;
    HANDLE          hBandwidth;
    ULONG           BytesPerFrameAvailable;
    ULONG           SpeedSelected;
} ISOCH_ALLOCATE_BANDWIDTH, *PISOCH_ALLOCATE_BANDWIDTH;

//
// struct used to pass in with IOCTL_ISOCH_ALLOCATE_CHANNEL
//
typedef struct _ISOCH_ALLOCATE_CHANNEL {
    LONG           nRequestedChannel;
    ULONG           Channel;
    LARGE_INTEGER   ChannelsAvailable;
} ISOCH_ALLOCATE_CHANNEL, *PISOCH_ALLOCATE_CHANNEL;

//
// struct used to pass in with IOCTL_ISOCH_ALLOCATE_RESOURCES
//
typedef struct _ISOCH_ALLOCATE_RESOURCES {
    ULONG           fulSpeed;
    ULONG           fulFlags;
    ULONG           nChannel;
    ULONG           nMaxBytesPerFrame;
    ULONG           nNumberOfBuffers;
    ULONG           nMaxBufferSize;
    ULONG           nQuadletsToStrip;
    HANDLE          hResource;
} ISOCH_ALLOCATE_RESOURCES, *PISOCH_ALLOCATE_RESOURCES;

//
// struct used to pass in isoch descriptors
//
typedef struct _RING3_ISOCH_DESCRIPTOR {
    ULONG           fulFlags;
    ULONG           ulLength;
    ULONG           nMaxBytesPerFrame;
    ULONG           ulSynch;
    ULONG           ulTag;
    CYCLE_TIME      CycleTime;
    ULONG           bUseCallback;
    ULONG           bAutoDetach;
    UCHAR           Data[1];
} RING3_ISOCH_DESCRIPTOR, *PRING3_ISOCH_DESCRIPTOR;

//
// struct used to pass in with IOCTL_ISOCH_ATTACH_BUFFERS
//
typedef struct _ISOCH_ATTACH_BUFFERS {
    HANDLE                  hResource;
    ULONG                   nNumberOfDescriptors;
    ULONG                   ulBufferSize;
    PULONG                  pIsochDescriptor;
    RING3_ISOCH_DESCRIPTOR  R3_IsochDescriptor[1];
} ISOCH_ATTACH_BUFFERS, *PISOCH_ATTACH_BUFFERS;

//
// struct used to pass in with IOCTL_ISOCH_DETACH_BUFFERS
//
typedef struct _ISOCH_DETACH_BUFFERS {
    HANDLE          hResource;
    ULONG           nNumberOfDescriptors;
    PULONG          pIsochDescriptor;
} ISOCH_DETACH_BUFFERS, *PISOCH_DETACH_BUFFERS;

//
// struct used to pass in with IOCTL_ISOCH_LISTEN
//
typedef struct _ISOCH_LISTEN {
    HANDLE          hResource;
    ULONG           fulFlags;
    CYCLE_TIME      StartTime;
} ISOCH_LISTEN, *PISOCH_LISTEN;

//
// struct used to pass in with IOCTL_ISOCH_QUERY_RESOURCES
//
typedef struct _ISOCH_QUERY_RESOURCES {
    ULONG           fulSpeed;
    ULONG           BytesPerFrameAvailable;
    LARGE_INTEGER   ChannelsAvailable;
} ISOCH_QUERY_RESOURCES, *PISOCH_QUERY_RESOURCES;

//
// struct used to pass in with IOCTL_ISOCH_SET_CHANNEL_BANDWIDTH
//
typedef struct _ISOCH_SET_CHANNEL_BANDWIDTH {
    HANDLE          hBandwidth;
    ULONG           nMaxBytesPerFrame;
} ISOCH_SET_CHANNEL_BANDWIDTH, *PISOCH_SET_CHANNEL_BANDWIDTH;

//
// struct used to pass in with IOCTL_ISOCH_MODIFY_STREAM_PROPERTIES
//
typedef struct _ISOCH_MODIFY_STREAM_PROPERTIES {
    HANDLE            hResource;
    ULARGE_INTEGER    ChannelMask;
    ULONG             fulSpeed;
} ISOCH_MODIFY_STREAM_PROPERTIES, *PISOCH_MODIFY_STREAM_PROPERTIES;

//
// struct used to pass in with IOCTL_ISOCH_STOP
//
typedef struct _ISOCH_STOP {
    HANDLE          hResource;
    ULONG           fulFlags;
} ISOCH_STOP, *PISOCH_STOP;

//
// struct used to pass in with IOCTL_ISOCH_TALK
//
typedef struct _ISOCH_TALK {
    HANDLE          hResource;
    ULONG           fulFlags;
    CYCLE_TIME      StartTime;
} ISOCH_TALK, *PISOCH_TALK;

//
// struct used to pass in with IOCTL_GET_LOCAL_HOST_INFORMATION
//
typedef struct _GET_LOCAL_HOST_INFORMATION {
    ULONG           Status;
    ULONG           nLevel;
    ULONG           ulBufferSize;
    UCHAR           Information[1];
} GET_LOCAL_HOST_INFORMATION, *PGET_LOCAL_HOST_INFORMATION;

//
// struct used to pass in with IOCTL_GET_1394_ADDRESS_FROM_DEVICE_OBJECT
//
typedef struct _GET_1394_ADDRESS {
    ULONG           fulFlags;
    NODE_ADDRESS    NodeAddress;
} GET_1394_ADDRESS, *PGET_1394_ADDRESS;

//
// struct used to pass in with IOCTL_GET_MAX_SPEED_BETWEEN_DEVICES
//
typedef struct _GET_MAX_SPEED_BETWEEN_DEVICES {
    ULONG           fulFlags;
    ULONG           ulNumberOfDestinations;
    HANDLE          hDestinationDeviceObjects[64];
    ULONG           fulSpeed;
} GET_MAX_SPEED_BETWEEN_DEVICES, *PGET_MAX_SPEED_BETWEEN_DEVICES;

//
// struct used to pass in with IOCTL_SET_DEVICE_XMIT_PROPERTIES
//
typedef struct _DEVICE_XMIT_PROPERTIES {
    ULONG           fulSpeed;
    ULONG           fulPriority;
} DEVICE_XMIT_PROPERTIES, *PDEVICE_XMIT_PROPERTIES;

//
// struct used to pass in with IOCTL_SET_LOCAL_HOST_INFORMATION
//
typedef struct _SET_LOCAL_HOST_INFORMATION {
    ULONG           nLevel;
    ULONG           ulBufferSize;
    UCHAR           Information[1];
} SET_LOCAL_HOST_INFORMATION, *PSET_LOCAL_HOST_INFORMATION;

#if !defined (_KMDF1394VDEV_DRIVER_)

typedef struct _DEVICE_LIST 
{
    CHAR            DeviceName[255];
} DEVICE_LIST, *PDEVICE_LIST;

typedef struct _DEVICE_DATA 
{
    ULONG           numDevices;
    DEVICE_LIST     deviceList[10];
} DEVICE_DATA, *PDEVICE_DATA;

#endif // KDMF1394VDEV_DRIVER

#if _MSC_VER >= 1200

#pragma warning(pop)

#else

#endif


#ifdef __cplusplus
}
#endif

#endif // _WDF_VDEV_API_H_
