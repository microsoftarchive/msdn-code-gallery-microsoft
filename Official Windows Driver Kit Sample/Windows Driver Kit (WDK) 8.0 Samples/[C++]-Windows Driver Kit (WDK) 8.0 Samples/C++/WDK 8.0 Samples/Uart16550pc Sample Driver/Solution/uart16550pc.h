/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    uart16550pc.h

Abstract:

    This module contains the 16550 UART controller's DDI functions.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

--*/

#pragma once

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>

// Class Extension includes
#include "SerCx.h"

/////////////////////////////////////////////////
//
// Resource and descriptor definitions.
//
/////////////////////////////////////////////////

#include "reshub.h"

//
// Serial peripheral bus descriptor
//

#include "pshpack1.h"

typedef struct _PNP_UART_SERIAL_BUS_DESCRIPTOR {
    PNP_SERIAL_BUS_DESCRIPTOR SerialBusDescriptor;
    ULONG BaudRate;
    USHORT RxBufferSize;
    USHORT TxBufferSize;
    UCHAR Parity;
    UCHAR SerialLinesEnabled;
    // follwed by optional Vendor Data
    // followed by resource name string
} PNP_UART_SERIAL_BUS_DESCRIPTOR, *PPNP_UART_SERIAL_BUS_DESCRIPTOR;

#include "poppack.h"

#define UART_SERIAL_BUS_TYPE               0x03
#define UART_SERIAL_FLAG_FLOW_CTL_NONE     (0x0000 << 0)
#define UART_SERIAL_FLAG_FLOW_CTL_HW       (0x0001 << 0)
#define UART_SERIAL_FLAG_FLOW_CTL_XONXOFF  (0x0002 << 0)
#define UART_SERIAL_FLAG_FLOW_CTL_MASK     (0x0003 << 0)
#define UART_SERIAL_FLAG_STOP_BITS_0       (0x0000 << 2)
#define UART_SERIAL_FLAG_STOP_BITS_1       (0x0001 << 2)
#define UART_SERIAL_FLAG_STOP_BITS_1_5     (0x0002 << 2)
#define UART_SERIAL_FLAG_STOP_BITS_2       (0x0003 << 2)
#define UART_SERIAL_FLAG_STOP_BITS_MASK    (0x0003 << 2)
#define UART_SERIAL_FLAG_DATA_BITS_5       (0x0000 << 4)
#define UART_SERIAL_FLAG_DATA_BITS_6       (0x0001 << 4)
#define UART_SERIAL_FLAG_DATA_BITS_7       (0x0002 << 4)
#define UART_SERIAL_FLAG_DATA_BITS_8       (0x0003 << 4)
#define UART_SERIAL_FLAG_DATA_BITS_9       (0x0004 << 4)
#define UART_SERIAL_FLAG_DATA_BITS_MASK    (0x0007 << 4)
#define UART_SERIAL_FLAG_BIG_ENDIAN        (0x0001 << 7)
#define UART_SERIAL_PARITY_NONE            0x00
#define UART_SERIAL_PARITY_EVEN            0x01
#define UART_SERIAL_PARITY_ODD             0x02
#define UART_SERIAL_PARITY_MARK            0x03
#define UART_SERIAL_PARITY_SPACE           0x04
#define UART_SERIAL_LINES_DCD              (0x0001 << 2)
#define UART_SERIAL_LINES_RI               (0x0001 << 3)
#define UART_SERIAL_LINES_DSR              (0x0001 << 4)
#define UART_SERIAL_LINES_DTR              (0x0001 << 5)
#define UART_SERIAL_LINES_CTS              (0x0001 << 6)
#define UART_SERIAL_LINES_RTS              (0x0001 << 7)

//
// Uart16550pc Function: UartInitContext
//
NTSTATUS
UartInitContext(
    _In_ WDFDEVICE Device
    );

//
// Uart16550pc Event Handler: UartEvtFileOpen
//
EVT_SERCX_FILEOPEN UartEvtFileOpen;

//
// Uart16550pc Event Handler: UartEvtFileClose
//
EVT_SERCX_FILECLOSE UartEvtFileClose;

//
// Uart16550pc Event Handler: UartEvtFileCleanup
//
EVT_SERCX_FILECLEANUP UartEvtFileCleanup;

//
// Uart16550pc Event Handler: UartEvtTransmit
//
EVT_SERCX_TRANSMIT UartEvtTransmit;

//
// Uart16550pc Event Handler: UartEvtReceive
//
EVT_SERCX_RECEIVE UartEvtReceive;

//
// Uart16550pc Event Handler: UartEvtTransmitCancel
//
EVT_SERCX_TRANSMIT_CANCEL UartEvtTransmitCancel;

//
// Uart16550pc Event Handler: UartEvtReceiveCancel
//
EVT_SERCX_RECEIVE_CANCEL UartEvtReceiveCancel;

//
// Uart16550pc Event Handler: UartEvtWaitmask
//
EVT_SERCX_WAITMASK UartEvtWaitmask;

//
// Uart16550pc Event Handler: UartEvtPurge
//
EVT_SERCX_PURGE UartEvtPurge;

//
// Uart16550pc Event Handler: UartEvtControl
//
EVT_SERCX_CONTROL UartEvtControl;

//
// Uart16550pc Event Handler: UartEvtApplyConfig
//
EVT_SERCX_APPLY_CONFIG UartEvtApplyConfig;

//
// Uart16550pc Function: UartGetDescriptorFromConnectionParameters
//
NTSTATUS
UartGetDescriptorFromConnectionParameters(
    _In_ PVOID ConnectionParameters,
    _Out_ PPNP_UART_SERIAL_BUS_DESCRIPTOR * Descriptor
    );

//
// Uart16550pc Function: UartApplyConfig
//
NTSTATUS
UartApplyConfig(
    _In_ WDFDEVICE Device,
    _In_ PPNP_UART_SERIAL_BUS_DESCRIPTOR Descriptor
    );

//
// Uart16550pc DMA Event Handler: UartEvtTransmitProgramDma
//
EVT_WDF_PROGRAM_DMA UartEvtTransmitProgramDma;

//
// Uart16550pc DMA Event Handler: UartEvtReceiveProgramDma
//
EVT_WDF_PROGRAM_DMA UartEvtReceiveProgramDma;

//
// Uart16550pc DMA Event Handler: UartEvtTransmitDmaTransferComplete
//
EVT_WDF_DMA_TRANSACTION_DMA_TRANSFER_COMPLETE UartEvtTransmitDmaTransferComplete;

//
// Uart16550pc DMA Event Handler: UartEvtReceiveDmaTransferComplete
//
EVT_WDF_DMA_TRANSACTION_DMA_TRANSFER_COMPLETE UartEvtReceiveDmaTransferComplete;

//
// Uart16550pc Function: UartTransmitDmaCancelCleanup
//
NTSTATUS
UartTransmitDmaCancelCleanup(
    _In_ WDFDEVICE Device,
    _In_ ULONG BytesTransferred
    );

//
// Uart16550pc Function: UartReceiveDmaCancelCleanup
//
NTSTATUS
UartReceiveDmaCancelCleanup(
    _In_ WDFDEVICE Device,
    _In_ ULONG BytesTransferred
    );

//
// Uart16550pc Event Handler: UartIntervalTimeoutTimer;
//
EVT_WDF_TIMER UartIntervalTimeoutTimer;

//
// Uart16550pc Function: UartIOCTLtoString
//
PCHAR
UartIOCTLtoString(
    _In_ ULONG        IoControlCode
    );
