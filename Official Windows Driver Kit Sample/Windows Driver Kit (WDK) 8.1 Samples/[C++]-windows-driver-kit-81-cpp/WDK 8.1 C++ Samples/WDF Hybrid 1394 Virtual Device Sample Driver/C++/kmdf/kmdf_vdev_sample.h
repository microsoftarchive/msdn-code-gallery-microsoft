/*++

Copyright (c)  Microsoft Corporation

Module Name:

kmdf_vdev_sample.h

Abstract:

Base include file for the KMDF stub of the WDF 1394 hybrid driver.

--*/

//
// Sometimes due to the nature of having to map 
// our requests into kernel mode we will have more than one chunk to 
// free in a completion routine.
//
typedef struct _CONTEXT_BUNDLE
{
    PVOID Context0;
    PVOID Context1;
    PVOID Context2;
    PVOID Context3;
}CONTEXT_BUNDLE, *PCONTEXT_BUNDLE;

//
// 80 msecs in units of 100nsecs
//
#define ISOCH_DETACH_TIMEOUT_VALUE  (ULONG)(-100 * 100 * 100 * 100)

//
// IEEE 1212 Directory definition
//
typedef struct _DIRECTORY_INFO {
    union {
        USHORT          DI_CRC;
        USHORT          DI_Saved_Length;
    } u;
    USHORT              DI_Length;
} DIRECTORY_INFO, *PDIRECTORY_INFO;

//
// IEEE 1212 Offset entry definition
//
#pragma warning(disable:4214)  // bit field types other than int warning

typedef struct _OFFSET_ENTRY {
    ULONG               OE_Offset:24;
    ULONG               OE_Key:8;
} OFFSET_ENTRY, *POFFSET_ENTRY;

#pragma warning(default:4214)

#define MAX_LEN_HOST_NAME 32

//
// Structure to identify hybrid device driver across the bus
//
typedef struct _DEVICE_EXTENSION {

    PDEVICE_OBJECT          PortDeviceObject;
    PDEVICE_OBJECT          PhysicalDeviceObject;
    PDEVICE_OBJECT          StackDeviceObject;

    WDFDEVICE               WdfDevice;
    WDFIOTARGET             StackIoTarget;

    WDFQUEUE                IoctlQueue;

    WDFQUEUE                BusResetRequestsQueue;

    WDFSPINLOCK             CromSpinLock;
    WDFSPINLOCK             AsyncSpinLock;
    WDFSPINLOCK             IsochSpinLock;
    WDFSPINLOCK             IsochResourceSpinLock;

    ULONG                   GenerationCount;

    LIST_ENTRY              CromData;
    LIST_ENTRY              AsyncAddressData;
    LIST_ENTRY              IsochDetachData;
    LIST_ENTRY              IsochResourceData;

} DEVICE_EXTENSION, *PDEVICE_EXTENSION;

WDF_DECLARE_CONTEXT_TYPE_WITH_NAME(DEVICE_EXTENSION, GetDeviceContext)


//
// This is used to keep track of dynamic crom calls.
//
typedef struct _CROM_DATA {
    LIST_ENTRY      CromList;
    HANDLE          hCromData;
    PVOID           Buffer;
    PMDL            pMdl;
} CROM_DATA, *PCROM_DATA;

//
// This is used to store data for each async address range.
//
typedef struct _ASYNC_ADDRESS_DATA {
    LIST_ENTRY              AsyncAddressList;
    PDEVICE_EXTENSION       DeviceExtension;
    PVOID                   Buffer;
    ULONG                   nLength;
    ULONG                   nAddressesReturned;
    PADDRESS_RANGE          AddressRange;
    HANDLE                  hAddressRange;
    PMDL                    pMdl;
} ASYNC_ADDRESS_DATA, *PASYNC_ADDRESS_DATA;

#define ISOCH_DETACH_TAG    0xaabbbbaa

//
// This is used to store data needed when calling IsochDetachBuffers.
// We need to store this data seperately for each call to IsochAttachBuffers.
//
typedef struct _ISOCH_DETACH_DATA {
    LIST_ENTRY              IsochDetachList;
    PDEVICE_EXTENSION       DeviceExtension;
    PISOCH_DESCRIPTOR       IsochDescriptor;
    WDFREQUEST              Request;
    PIRP                    newIrp;
    PIRB                    DetachIrb;
    PIRB                    AttachIrb;
    NTSTATUS                AttachStatus;
    KTIMER                  Timer;
    KDPC                    TimerDpc;
    HANDLE                  hResource;
    ULONG                   numIsochDescriptors;
    ULONG                   outputBufferLength;
    ULONG                   bDetach;
} ISOCH_DETACH_DATA, *PISOCH_DETACH_DATA;

//
// This is used to store allocated isoch resources.
// We use this information in case of a surprise removal.
//
typedef struct _ISOCH_RESOURCE_DATA {
    LIST_ENTRY      IsochResourceList;
    HANDLE          hResource;
} ISOCH_RESOURCE_DATA, *PISOCH_RESOURCE_DATA;

typedef struct _DEVICE_LIST {
    CHAR            DeviceName[255];
} DEVICE_LIST, *PDEVICE_LIST;

typedef struct _DEVICE_DATA {
    ULONG           numDevices;
    DEVICE_LIST     deviceList[10];
} DEVICE_DATA, *PDEVICE_DATA;

//
//  kmdf_vdev_api.c
//
NTSTATUS
kmdf1394_GetLocalHostInformation(
                                 IN WDFDEVICE  Device,
                                 IN WDFREQUEST Request,
                                 IN OUT PGET_LOCAL_HOST_INFORMATION GetLocalHostInfo);

_At_(SetLocalHostInfo->nLevel, _In_ _In_range_(SET_LOCAL_HOST_PROPERTIES_GAP_COUNT, SET_LOCAL_HOST_PROPERTIES_MODIFY_CROM))
NTSTATUS
kmdf1394_SetLocalHostProperties (
                                IN WDFDEVICE Device,
                                IN WDFREQUEST Request,
                                IN OUT PSET_LOCAL_HOST_INFORMATION SetLocalHostInfo);

NTSTATUS
kmdf1394_Get1394AddressFromDeviceObject (
                                        IN WDFDEVICE Device,
                                        IN WDFREQUEST Request,
                                        IN OUT PGET_1394_ADDRESS Get1394Address);

NTSTATUS
kmdf1394_Control (
                  IN WDFDEVICE   Device,
                  IN WDFREQUEST  Request);

NTSTATUS
kmdf1394_GetMaxSpeedBetweenDevices (
                                    IN WDFDEVICE Device,
                                    IN WDFREQUEST Request,
                                    IN OUT PGET_MAX_SPEED_BETWEEN_DEVICES MaxSpeedBtwnDevices);

NTSTATUS
kmdf1394_SetDeviceXmitProperties(
                                IN WDFDEVICE Device,
                                IN WDFREQUEST Request,
                                IN PDEVICE_XMIT_PROPERTIES DeviceXmitProperties);

NTSTATUS
kmdf1394_GetConfigurationInformation (
                                    IN WDFDEVICE Device,
                                    IN WDFREQUEST Request);

NTSTATUS
kmdf1394_BusReset (
                IN WDFDEVICE Device,
                IN WDFREQUEST Request,
                IN PULONG fulFlags);

NTSTATUS
kmdf1394_GetGenerationCount (
                            IN WDFDEVICE Device,
                            IN WDFREQUEST Request,
                            IN OUT PULONG GenerationCount);

NTSTATUS
kmdf1394_SendPhyConfigurationPacket (
                                    IN WDFDEVICE Device,
                                    IN WDFREQUEST Request,
                                    IN PPHY_CONFIGURATION_PACKET PhyConfigPacket);

NTSTATUS
kmdf1394_BusResetNotification (
                            IN WDFDEVICE Device,
                            IN WDFREQUEST Request,
                            IN ULONG fulFlags);

NTSTATUS
kmdf1394_SetAddressData (
                        IN WDFDEVICE Device,
                        IN WDFREQUEST Request,
                        IN PSET_ADDRESS_DATA SetAddrData);

void
kmdf1394_BusResetRoutine (
                        IN PVOID Context);

//
// kmdf_vdev_async.c
//

EVT_WDF_REQUEST_COMPLETION_ROUTINE kmdf1394_AsyncLockCompletion;
EVT_WDF_REQUEST_COMPLETION_ROUTINE kmdf1394_AllocateAddressRangeCompletion;
EVT_WDF_REQUEST_COMPLETION_ROUTINE kmdf1394_AsyncStreamCompletion; 
EVT_WDF_REQUEST_COMPLETION_ROUTINE kmdf1394_AsyncReadCompletion;
EVT_WDF_REQUEST_COMPLETION_ROUTINE kmdf1394_AsyncWriteCompletion;

NTSTATUS
kmdf1394_AllocateAddressRange (
                                IN WDFDEVICE DeviceObject,
                                IN WDFREQUEST Irp,
                                IN PALLOCATE_ADDRESS_RANGE AllocateAddrRange);


NTSTATUS
kmdf1394_FreeAddressRange (
                            IN WDFDEVICE Device,
                            IN WDFREQUEST Request,
                            IN HANDLE hAddressRange);

NTSTATUS
kmdf1394_SetAddressData (
                        IN WDFDEVICE Device,
                        IN WDFREQUEST Request,
                        IN PSET_ADDRESS_DATA SetAddrData);

NTSTATUS
kmdf1394_GetAddressData (
                        IN WDFDEVICE Device,
                        IN WDFREQUEST Request,
                        IN OUT PGET_ADDRESS_DATA GetAddrData);

NTSTATUS
kmdf1394_AsyncRead (
                    IN WDFDEVICE Device,
                    IN WDFREQUEST Request,
                    IN OUT PASYNC_READ AsyncRead);

NTSTATUS
kmdf1394_AsyncWrite (
                    IN WDFDEVICE Device,
                    IN WDFREQUEST Request,
                    IN PASYNC_WRITE AsyncWrite);

NTSTATUS
kmdf1394_AsyncLock (
                    IN WDFDEVICE Device,
                    IN WDFREQUEST Request,
                    IN OUT PASYNC_LOCK AsyncLock);


NTSTATUS
kmdf1394_AsyncStream (
                    IN WDFDEVICE Device,
                    IN WDFREQUEST Request,
                    IN OUT PASYNC_STREAM AsyncStream);

//
// isochapi.c
//

NTSTATUS
kmdf1394_IsochAllocateBandwidth (
                                IN WDFDEVICE Device,
                                IN WDFREQUEST Request,
                                IN OUT PISOCH_ALLOCATE_BANDWIDTH IsochAllocateBandwidth);

NTSTATUS
kmdf1394_IsochAllocateChannel (
                                IN WDFDEVICE Device,
                                IN WDFREQUEST Request,
                                IN OUT PISOCH_ALLOCATE_CHANNEL IsochAllocateChannel);

NTSTATUS
kmdf1394_IsochAllocateResources (
                                IN WDFDEVICE Device,
                                IN WDFREQUEST Request,
                                IN PISOCH_ALLOCATE_RESOURCES IsochAllocateResources);

NTSTATUS
kmdf1394_IsochAttachBuffers (
                            IN WDFDEVICE Device,
                            IN WDFREQUEST Request,
                            IN PISOCH_ATTACH_BUFFERS IsochAttachBuffers);

NTSTATUS
kmdf1394_IsochDetachBuffers (
                            IN WDFDEVICE Device,
                            IN WDFREQUEST Request,
                            IN PISOCH_DETACH_BUFFERS IsochDetachBuffers);

NTSTATUS
kmdf1394_IsochFreeBandwidth (
                            IN WDFDEVICE Device,
                            IN WDFREQUEST Request,
                            IN HANDLE hBandwidth);

NTSTATUS
kmdf1394_IsochFreeChannel (
                            IN WDFDEVICE DeviceObject,
                            IN WDFREQUEST Irp,
                            IN ULONG nChannel);

NTSTATUS
kmdf1394_IsochFreeResources (
                            IN WDFDEVICE DeviceObject,
                            IN WDFREQUEST Irp,
                            IN HANDLE hResource);

NTSTATUS
kmdf1394_IsochListen (
                    IN WDFDEVICE Device,
                    IN WDFREQUEST Request,
                    IN PISOCH_LISTEN IsochListen);

NTSTATUS
kmdf1394_IsochQueryCurrentCycleTime (
                                    IN WDFDEVICE Device,
                                    IN WDFREQUEST Request,
                                    OUT PCYCLE_TIME pCurrentCycleTime);

NTSTATUS
kmdf1394_IsochQueryResources (
                            IN WDFDEVICE Device,
                            IN WDFREQUEST Request,
                            IN OUT PISOCH_QUERY_RESOURCES IsochQueryResources);


NTSTATUS
kmdf1394_IsochSetChannelBandwidth (
                                IN WDFDEVICE Device,
                                IN WDFREQUEST Request,
                                IN PISOCH_SET_CHANNEL_BANDWIDTH IsochSetChannelBandwidth);

NTSTATUS
kmdf1394_IsochModifyStreamProperties (
                                    IN WDFDEVICE Device,
                                    IN WDFREQUEST Request,
                                    IN PISOCH_MODIFY_STREAM_PROPERTIES IsochModifyStreamProperties);

NTSTATUS
kmdf1394_IsochStop (
                    IN WDFDEVICE Device,
                    IN WDFREQUEST Request,
                    IN PISOCH_STOP IsochStop);

NTSTATUS
kmdf1394_IsochTalk (
                    IN WDFDEVICE DeviceObject,
                    IN WDFREQUEST Request,
                    IN PISOCH_TALK IsochTalk);

void
kmdf1394_IsochTimeout (
                    IN PKDPC Dpc,
                    IN PISOCH_DETACH_DATA IsochDetachData,
                    IN PVOID SystemArgument1,
                    IN PVOID SystemArgument2);

void
kmdf1394_IsochCallback (
                        IN PDEVICE_EXTENSION DeviceExtension,
                        IN PISOCH_DETACH_DATA IsochDetachData);

void
kmdf1394_IsochCleanup (
                    IN PISOCH_DETACH_DATA IsochDetachData);

//
// util.c
//
NTSTATUS
kmdf1394_SubmitIrpSynch (
                        IN WDFIOTARGET IoTarget,
                        IN WDFREQUEST Request,
                        IN PIRB Irb);

VOID
kmdf1394_SubmitIrpAsyncCompletion (
                                IN WDFREQUEST Request,
                                IN WDFIOTARGET Target,
                                IN PWDF_REQUEST_COMPLETION_PARAMS Params,
                                IN WDFCONTEXT Context);

NTSTATUS
kmdf1394_SubmitIrpAsync (
                        IN WDFIOTARGET IoTarget,
                        IN WDFREQUEST Request,
                        IN WDFMEMORY Memory);

BOOLEAN
kmdf1394_IsOnList (
                PLIST_ENTRY Entry,
                PLIST_ENTRY List);

NTSTATUS
kmdf1394_UpdateGenerationCount (
                                IN WDFDEVICE Device);


//
// 1394vdev.c
//
DRIVER_INITIALIZE DriverEntry;

EVT_WDF_OBJECT_CONTEXT_CLEANUP kmdf1394_EvtDriverCleanup;

//
// ioctl.c
//
EVT_WDF_IO_QUEUE_IO_DEVICE_CONTROL kmdf1394_EvtIoDeviceControl;

//
// pnp.c
//
EVT_WDF_DRIVER_DEVICE_ADD kmdf1394_EvtDeviceAdd;

EVT_WDF_DEVICE_PREPARE_HARDWARE kmdf1394_EvtPrepareHardware;

EVT_WDF_DEVICE_RELEASE_HARDWARE kmdf1394_EvtReleaseHardware;

EVT_WDF_DEVICE_D0_ENTRY kmdf1394_EvtDeviceD0Entry;

EVT_WDF_DEVICE_D0_EXIT kmdf1394_EvtDeviceD0Exit;

EVT_WDF_DEVICE_SELF_MANAGED_IO_CLEANUP kmdf1394_EvtDeviceSelfManagedIoCleanup;

PCHAR
DbgDevicePowerString (
                     IN WDF_POWER_DEVICE_STATE Type);

EVT_WDF_WORKITEM kmdf1394_BusResetRoutineWorkItem;

