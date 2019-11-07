/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    Private.h  - Private header file for the S5933DK1 driver.

Abstract:

Environment:

    Kernel mode

--*/

#if !defined(_S5933DK1_H_)
#define _S5933DK1_H_

// ----------------------------------------------------------------------------
// Hardware related defines and equates
// ----------------------------------------------------------------------------

// ----------------------------------------------------------------------------
// S5933DK1 ISA card operation register addresses (p. 3-67)
//
// The sense of mailbox transfers is the opposite from the S5933: an S5933
// "inbound" mailbox is an add-on "outbound" mailbox. The sense of DMA
// transfers is the same: an S5933 "read" moves data from memory to the S5933,
// and an add-on "read" moves data from the S5933 to the add-on.
// ----------------------------------------------------------------------------
#define AIMB1    ((PULONG) 0x300)
#define AIMB2    ((PULONG) 0x304)
#define AIMB3    ((PULONG) 0x308)
#define AIMB4    ((PULONG) 0x30C)
#define AOMB1    ((PULONG) 0x310)
#define AOMB2    ((PULONG) 0x314)
#define AOMB3    ((PULONG) 0x318)
#define AOMB4    ((PULONG) 0x31C)
#define AFIFO    ((PULONG) 0x320)
#define AMWAR    ((PULONG) 0x324)
#define APTA     ((PULONG) 0x328)       // pass-through address
#define APTD     ((PULONG) 0x32C)       // pass-through data
#define AMRAR    ((PULONG) 0x330)
#define AMBEF    ((PULONG) 0x334)
#define AINT     ((PULONG) 0x338)
#define AGCSTS   ((PULONG) 0x33C)

#define AMWTC    ((PULONG) 0x718)
#define AMRTC    ((PULONG) 0x71C)

//
// Bits in GCSTS register (p. 3.77 ff.):
//
#define GCSTS_WFIFO_FULL     0x00000001 // add-on to PCI FIFO is full
#define GCSTS_WFIFO_4PLUS    0x00000002 // at least 4 empty words in add-on to PCI FIFO
#define GCSTS_WFIFO_EMPTY    0x00000004 // add-on to PCI FIFO completely empty
#define GCSTS_RFIFO_FULL     0x00000008 // PCI to add-on FIFO is full
#define GCSTS_RFIFO_4PLUS    0x00000010 // at least 4 valid words in PCI to add-on FIFO
#define GCSTS_RFIFO_EMPTY    0x00000020 // PCI to add-on is completely empty
#define GCSTS_RTCZ           0x00000040 // read (PCI to add-on) transfer count is zero
#define GCSTS_WTCZ           0x00000080 // write (add-on to PCI) transfer count is zero

//                           0x00000F00 // reserved (always zero)
#define GCSTS_BIST_CODE      0x0000F000 // results of built-in self test
#define GCSTS_NVRAM_ADDRESS  0x00FF0000 // address/data port for NVRAM access

//                           0x01000000 // reserved (always zero)
#define GCSTS_OFIFO_RESET    0x02000000 // reset (mark empty) PCI to add-on FIFO
#define GCSTS_IFIFO_RESET    0x04000000 // reset (mark full) add-on to PCI FIFO
#define GCSTS_MAILBOX_RESET  0x08000000 // reset mailbox flags
#define GCSTS_RESET          0x0E000000 // all reset flags

#define GCSTS_TC_ENABLE      0x10000000 // use transfer counts for addon-initiated DMA



#define INT_INTERRUPT_MASK   0x003F0000 // mask for all interrupt flags

// ----------------------------------------------------------------------------
// Software related defines and equates
// ----------------------------------------------------------------------------

//
// The device extension.
//
typedef struct _RDK_DEVICE_EXTENSION {

    WDFDEVICE           Device;               // a WDFDEVICE handle
    WDFREQUEST          CurrentRequest;       // a WDFREQUEST handle
    WDFWORKITEM         Worker;               // a WDFWORKITEM handle

    PUCHAR              Port300Base;          // I/O port base address
    ULONG               Port300Count;         // Number of assigned ports
    BOOLEAN             Port300Mapped;        // TRUE if mapped port addr

    PUCHAR              Port718Base;          // I/O port base address
    ULONG               Port718Count;         // Number of assigned ports
    BOOLEAN             Port718Mapped;        // TRUE if mapped port addr

} RDK_DEVICE_EXTENSION, * PRDK_DEVICE_EXTENSION;

WDF_DECLARE_CONTEXT_TYPE_WITH_NAME(RDK_DEVICE_EXTENSION, AmccIsaGetDevExt)

//
// The workitem structure and its DMA derivative.
//
typedef struct _DMA_WORKITEM {

    PUCHAR   Buffer;             // data buffer address
    size_t   nbytes;             // remaining byte count
    size_t   numxfer;            // number transferred so far
    BOOLEAN  isread;             // TRUE if read, FALSE if write

} DMA_WORKITEM, * PDMA_WORKITEM;


struct _WORKITEM;
typedef BOOLEAN (*WORKITEM_CALLBACK) (struct _WORKITEM * Item );

typedef struct _WORKITEM {

    WDFREQUEST          Request;   // complete this Request when done
    WORKITEM_CALLBACK   Callback;  // callback routine
    PRDK_DEVICE_EXTENSION   DevExt;    // device extension

    union {
        DMA_WORKITEM    DmaWork;      // dma workitem subtype.
                                   // other subtypes may be inserted here
    } u;

} WORKITEM, * PWORKITEM;

WDF_DECLARE_CONTEXT_TYPE_WITH_NAME(WORKITEM, AmccIsaGetWorkItem)

// ----------------------------------------------------------------------------
// Function prototypes
// ----------------------------------------------------------------------------

EVT_WDF_DEVICE_PREPARE_HARDWARE AmccIsaEvtDevicePrepareHardware;
EVT_WDF_DEVICE_RELEASE_HARDWARE AmccIsaEvtDeviceReleaseHardware;

EVT_WDF_IO_QUEUE_IO_DEVICE_CONTROL AmccIsaEvtIoDeviceControl;

EVT_WDF_OBJECT_CONTEXT_CLEANUP AmccIsaDeviceContextCleanup;

VOID
AmccIsaFreeDeviceResources(
    _In_ PRDK_DEVICE_EXTENSION DeviceExtension
    );

EVT_WDF_WORKITEM AmccIsaWorker;

NTSTATUS
AmccIsaResetDevice(
    _In_ PRDK_DEVICE_EXTENSION  DevExt
    );

NTSTATUS
AmccIsaReadWriteDma(
    _In_ PRDK_DEVICE_EXTENSION  DevExt,
    _In_ WDFREQUEST         Request,
    _In_ BOOLEAN            isRead
    );

BOOLEAN
ReadDmaCallback(
    _In_ PWORKITEM  Item
    );

BOOLEAN
WriteDmaCallback(
    _In_ PWORKITEM  Item
    );

#endif //_S5933DK1_H_


