/*++
    Copyright (c) 2000,2001 Microsoft Corporation

    Module Name:
        HidBtn.h

    Abstract:
        Contains definitions of all constants and data types for the
        serial pen hid driver.

    Environment:
        Kernel mode

--*/

#ifndef _HIDBTN_H
#define _HIDBTN_H

//
// Constants
//
#define HBTN_POOL_TAG                   'ntbH'

// dwfHBtn flag values
#define HBTNF_DEVICE_STARTED            0x00000001

//
// Macros
//
#define GET_MINIDRIVER_DEVICE_EXTENSION(DO) \
    ((PDEVICE_EXTENSION)(((PHID_DEVICE_EXTENSION)(DO)->DeviceExtension)->MiniDeviceExtension))
#define GET_NEXT_DEVICE_OBJECT(DO)          \
    (((PHID_DEVICE_EXTENSION)(DO)->DeviceExtension)->NextDeviceObject)
#define GET_PDO(DO)                         \
    (((PHID_DEVICE_EXTENSION)(DO)->DeviceExtension)->PhysicalDeviceObject)
//#define ARRAYSIZE(a)    (sizeof(a)/sizeof(a[0]))

//
// Type Definitions
//
typedef struct _DEVICE_EXTENSION
{
    ULONG          dwfHBtn;             //flags
    PDEVICE_OBJECT self;                //my device object
    PDEVICE_OBJECT LowerDevObj;         //lower device object
    IO_REMOVE_LOCK RemoveLock;          //to protect IRP_MN_REMOVE_DEVICE
    LIST_ENTRY     PendingIrpList;
    OEM_EXTENSION  OemExtension;
    KSPIN_LOCK     QueueLock;           //lock for cancel safe queue
    KSPIN_LOCK     DataLock;            //lock for protecting device extension access.
    IO_CSQ         IrpQueue;           //cancel safe queue
    
} DEVICE_EXTENSION, *PDEVICE_EXTENSION;


#define GET_DEV_EXT(csq) \
	CONTAINING_RECORD(csq, DEVICE_EXTENSION, IrpQueue)

//
// Function prototypes
//

// hidbtn.c
DRIVER_INITIALIZE DriverEntry;

_Dispatch_type_(IRP_MJ_CREATE)
_Dispatch_type_(IRP_MJ_CLOSE)
DRIVER_DISPATCH HbtnCreateClose;

DRIVER_ADD_DEVICE HbtnAddDevice;

DRIVER_UNLOAD HbtnUnload;

// pnp.c
_Dispatch_type_(IRP_MJ_PNP)
DRIVER_DISPATCH HbtnPnp;


_Dispatch_type_(IRP_MJ_POWER)
DRIVER_DISPATCH HbtnPower;


NTSTATUS INTERNAL
SendSyncIrp(
    _In_ PDEVICE_OBJECT DevObj,
    _In_ PIRP           Irp,
    _In_ BOOLEAN        fCopyToNext
    );

IO_COMPLETION_ROUTINE IrpCompletion;

// ioctl.c
_Dispatch_type_(IRP_MJ_INTERNAL_DEVICE_CONTROL)
DRIVER_DISPATCH HbtnInternalIoctl;

_Dispatch_type_(IRP_MJ_SYSTEM_CONTROL)
DRIVER_DISPATCH HbtnSystemControl;


NTSTATUS INTERNAL
GetDeviceDescriptor(
    _In_ PDEVICE_EXTENSION DevExt,
    _In_ PIRP              Irp
    );

NTSTATUS INTERNAL
GetReportDescriptor(
    _In_ PDEVICE_EXTENSION DevExt,
    _In_ PIRP              Irp
    );

NTSTATUS INTERNAL
ReadReport(
    _In_ PDEVICE_EXTENSION DevExt,
    _In_ PIRP              Irp
    );

NTSTATUS INTERNAL
GetString(
    _In_ PDEVICE_EXTENSION DevExt,
    _In_ PIRP              Irp
    );

NTSTATUS INTERNAL
GetAttributes(
    _In_ PDEVICE_EXTENSION DevExt,
    _In_ PIRP              Irp
    );

// oembtn.c
NTSTATUS INTERNAL
OemAddDevice(
    _In_ PDEVICE_EXTENSION devext
    );

NTSTATUS INTERNAL
OemStartDevice(
    _In_ PDEVICE_EXTENSION devext,
    _In_ PIRP              Irp
    );

NTSTATUS INTERNAL
OemRemoveDevice(
    _In_ PDEVICE_EXTENSION devext,
    _In_ PIRP              Irp
    );

NTSTATUS INTERNAL
OemWriteReport(
    _In_ PDEVICE_EXTENSION devext,
    _In_ PIRP Irp
    );

_At_((PDEVICE_EXTENSION)csq, _In_)
VOID
HbtnInsertIrp (
    _In_ PIO_CSQ   csq,
    _In_ PIRP      Irp
    );

VOID
HbtnRemoveIrp(
    _In_  PIO_CSQ csq,
    _In_  PIRP    Irp
    );

_At_((PDEVICE_EXTENSION)csq, _In_)
PIRP
HbtnPeekNextIrp(
    _In_  PIO_CSQ csq,
    _In_  PIRP    Irp,
    _In_  PVOID  PeekContext
    );

_At_((PDEVICE_EXTENSION)csq, _In_)
_Acquires_lock_(GET_DEV_EXT(csq)->QueueLock)
_IRQL_raises_(DISPATCH_LEVEL)
_IRQL_requires_max_(DISPATCH_LEVEL)
VOID
HbtnAcquireLock(
    _In_                              PIO_CSQ csq,
    _Out_ _At_(*Irql, _IRQL_saves_)   PKIRQL  Irql
    );

_At_((PDEVICE_EXTENSION)csq, _In_)
_Releases_lock_(GET_DEV_EXT(csq)->QueueLock)
_IRQL_requires_(DISPATCH_LEVEL)
VOID
HbtnReleaseLock(
    _In_                              PIO_CSQ csq,
    _In_ _IRQL_restores_              KIRQL   Irql
    );

_At_((PDEVICE_EXTENSION)pCsq, _In_)
VOID
HbtnCompleteCancelledIrp(
    _In_  PIO_CSQ             pCsq,
    _In_  PIRP                Irp
    );


#endif  //ifndef _HIDBTN_H

