/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    regutils.h

Abstract:

    This module contains the 16550 UART controller driver's register utilities.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

    This code borrows heavily from code in the WDF Serial example.

--*/

#pragma once

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>
#include "ntddser.h"

// Class Extension includes
#include "SerCx.h"

#include "regfile.h"

//
// Source clock used to generate bus clock.
//
const ULONG UartSourceClockFrequency = 1843200;
const ULONG UartMinBaudRate = 2;       // source / (16 * MAXUSHORT)
const ULONG UartMaxBaudRate = 115200;  // source / (16 * 1)
const DOUBLE UartBaudRateErrorTolerance = .05;

//-----------------------------------------------------------------------------
// 4127 -- Conditional Expression is Constant warning
//-----------------------------------------------------------------------------
#define WHILE(constant) \
__pragma(warning(suppress: 4127)) while(constant)


//
// Sets the divisor latch register.  The divisor latch register
// is used to control the baud rate of the 8250.
//
// As with all of these routines it is assumed that it is called
// at a safe point to access the hardware registers.  In addition
// it also assumes that the data is correct.
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
// DesiredDivisor - The value to which the divisor latch register should
//                  be set.
//
#define WRITE_DIVISOR_LATCH(Extension, BaseAddress,DesiredDivisor)           \
do                                                                \
{                                                                 \
    PUCHAR Address = BaseAddress;                                 \
    SHORT Divisor = DesiredDivisor;                               \
    UCHAR LineControl;                                            \
    LineControl = Extension->UartReadUChar(Address, LINE_CONTROL_REGISTER); \
    Extension->UartWriteUChar(                                             \
        Address, LINE_CONTROL_REGISTER,                            \
        (UCHAR)(LineControl | SERIAL_LCR_DLAB)                    \
        );                                                        \
    Extension->UartWriteUChar(                                             \
        Address, DIVISOR_LATCH_LSB,                                \
        (UCHAR)(Divisor & 0xff)                                   \
        );                                                        \
    Extension->UartWriteUChar(                                             \
        Address, DIVISOR_LATCH_MSB,                                \
        (UCHAR)((Divisor & 0xff00) >> 8)                          \
        );                                                        \
    Extension->UartWriteUChar(                                             \
        Address, LINE_CONTROL_REGISTER,                            \
        LineControl                                               \
        );                                                        \
} WHILE (0)

//
// Reads the divisor latch register.  The divisor latch register
// is used to control the baud rate of the 8250.
//
// As with all of these routines it is assumed that it is called
// at a safe point to access the hardware registers.  In addition
// it also assumes that the data is correct.
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
// DesiredDivisor - A pointer to the 2 byte word which will contain
//                  the value of the divisor.
//
#define READ_DIVISOR_LATCH(Extension, BaseAddress,PDesiredDivisor)           \
do                                                                \
{                                                                 \
    PUCHAR Address = BaseAddress;                                 \
    PUSHORT PDivisor = PDesiredDivisor;                            \
    UCHAR LineControl;                                            \
    UCHAR Lsb;                                                    \
    UCHAR Msb;                                                    \
    LineControl = Extension->UartReadUChar(Address, LINE_CONTROL_REGISTER); \
    Extension->UartWriteUChar(                                             \
        Address, LINE_CONTROL_REGISTER,                            \
        (UCHAR)(LineControl | SERIAL_LCR_DLAB)                    \
        );                                                        \
    Lsb = Extension->UartReadUChar(Address, DIVISOR_LATCH_LSB);             \
    Msb = Extension->UartReadUChar(Address, DIVISOR_LATCH_MSB);             \
    *PDivisor = Lsb;                                              \
    *PDivisor = *PDivisor | (((USHORT)Msb) << 8);                 \
    Extension->UartWriteUChar(                                             \
        Address, LINE_CONTROL_REGISTER,                            \
        LineControl                                               \
        );                                                        \
} WHILE (0)

//
// This macro reads the interrupt enable register.
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
#define READ_INTERRUPT_ENABLE(Extension, BaseAddress)                     \
    (Extension->UartReadUChar((BaseAddress), INTERRUPT_ENABLE_REGISTER))

//
// This macro writes the interrupt enable register.
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
// Values - The values to write to the interrupt enable register.
//
#define WRITE_INTERRUPT_ENABLE(Extension, BaseAddress,Values)                \
do                                                                \
{                                                                 \
    Extension->UartWriteUChar(                                             \
        BaseAddress, INTERRUPT_ENABLE_REGISTER,                    \
        Values                                                    \
        );                                                        \
} WHILE (0)

//
// This macro disables all interrupts on the hardware.
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
//
#define DISABLE_ALL_INTERRUPTS(Extension, BaseAddress)       \
do                                                \
{                                                 \
    WRITE_INTERRUPT_ENABLE(Extension, BaseAddress,0);        \
} WHILE (0)

//
// This macro enables all interrupts on the hardware.
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
//
#define ENABLE_ALL_INTERRUPTS(Extension, BaseAddress)        \
do                                                \
{                                                 \
                                                  \
    WRITE_INTERRUPT_ENABLE(                       \
        (Extension), (BaseAddress),                            \
        (UCHAR)(SERIAL_IER_RDA | SERIAL_IER_THR | \
                SERIAL_IER_RLS | SERIAL_IER_MS)   \
        );                                        \
                                                  \
} WHILE (0)

//
// This macro reads the interrupt identification register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
// Note that this routine potententially quites a transmitter
// empty interrupt.  This is because one way that the transmitter
// empty interrupt is cleared is to simply read the interrupt id
// register.
//
//
#define READ_INTERRUPT_ID_REG(Extension, BaseAddress)                          \
    (Extension->UartReadUChar((BaseAddress), INTERRUPT_IDENT_REGISTER))

//
// This macro reads the modem control register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
//
#define READ_MODEM_CONTROL(Extension, BaseAddress)                          \
    (Extension->UartReadUChar((BaseAddress), MODEM_CONTROL_REGISTER))

//
// This macro reads the modem status register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
//
#define READ_MODEM_STATUS(Extension, BaseAddress)                          \
    (Extension->UartReadUChar((BaseAddress), MODEM_STATUS_REGISTER))

//
// This macro reads a value out of the receive buffer
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
//
#define READ_RECEIVE_BUFFER(Extension, BaseAddress)                          \
    (Extension->UartReadUChar((BaseAddress), RECEIVE_BUFFER_REGISTER))

//
// This macro reads the line status register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
//
#define READ_LINE_STATUS(Extension, BaseAddress)                          \
    (Extension->UartReadUChar((BaseAddress), LINE_STATUS_REGISTER))

//
// This macro writes the line control register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
//
#define WRITE_LINE_CONTROL(Extension, BaseAddress,NewLineControl)           \
do                                                               \
{                                                                \
    Extension->UartWriteUChar(                                            \
        (BaseAddress), LINE_CONTROL_REGISTER,                     \
        (NewLineControl)                                         \
        );                                                       \
} WHILE (0)

//
// This macro reads the line control register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
//
#define READ_LINE_CONTROL(Extension, BaseAddress)           \
    (Extension->UartReadUChar((BaseAddress), LINE_CONTROL_REGISTER))


//
// This macro writes to the transmit register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
// TransmitChar - The character to send down the wire.
//
//
#define WRITE_TRANSMIT_HOLDING(Extension, BaseAddress,TransmitChar)       \
do                                                             \
{                                                              \
    Extension->UartWriteUChar(                                          \
        (BaseAddress), TRANSMIT_HOLDING_REGISTER,               \
        (TransmitChar)                                         \
        );                                                     \
} WHILE (0)

//
// This macro writes to the transmit FIFO register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
// TransmitChars - Pointer to the characters to send down the wire.
//
// TxN - number of charactes to send.
//
//
#define WRITE_TRANSMIT_FIFO_HOLDING(Extension, BaseAddress,TransmitChars,TxN)  \
do                                                             \
{                                                              \
    WRITE_PORT_BUFFER_UCHAR(                                   \
        (BaseAddress), TRANSMIT_HOLDING_REGISTER,               \
        (TransmitChars),                                       \
        (TxN)                                                  \
        );                                                     \
} WHILE (0)

//
// This macro writes to the control register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
// ControlValue - The value to set the fifo control register too.
//
//
#define WRITE_FIFO_CONTROL(Extension, BaseAddress,ControlValue)           \
do                                                             \
{                                                              \
    Extension->UartWriteUChar(                                          \
        (BaseAddress), FIFO_CONTROL_REGISTER,                   \
        (ControlValue)                                         \
        );                                                     \
} WHILE (0)

//
// This macro writes to the modem control register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located.
//
// ModemControl - The control bits to send to the modem control.
//
//
#define WRITE_MODEM_CONTROL(Extension, BaseAddress,ModemControl)          \
do                                                             \
{                                                              \
    Extension->UartWriteUChar(                                          \
        (BaseAddress), MODEM_CONTROL_REGISTER,                  \
        (ModemControl)                                         \
        );                                                     \
} WHILE (0)

#define WRITE_INTERRUPT_STATUS(Extension, BaseAddress,Status)  \
do                                                               \
{                                                                \
       Extension->UartWriteUChar(BaseAddress, 0, Status);                    \
} WHILE (0)


//
// This macro reads the interrupt status register
//
// Arguments:
//
// BaseAddress - A pointer to the address from which the hardware
//               device registers are located. BaseAddress is gotten
//               from PSERIAL_MULTIPORT_DISPATCH->InterruptStatus which
//               already has the complete address
//
// AddressSpace - Flag indicating where port is located, MMIO or IO
//                space
//
//
#define READ_INTERRUPT_STATUS(Extension, BaseAddress)  \
                      Extension->UartReadUChar(BaseAddress, 0))

NTSTATUS
UartRegDivisorLatchToBaud (
    _In_ USHORT DivisorLatchRegs,
    _Out_ ULONG * BaudRate
    );

NTSTATUS
UartRegBaudToDivisorLatch (
    _In_ ULONG SpeedBPS,
    _Out_ USHORT * DivisorLatch
    );

NTSTATUS
UartRegConvertAndValidateBaud (
    _In_ ULONG SpeedBPS,
    _Out_ USHORT * DivisorLatch
    );

NTSTATUS
UartRegLCRToStruct (
    _In_ UCHAR LineControlRegister,
    _Out_ PSERIAL_LINE_CONTROL LineControlStruct
    );

NTSTATUS
UartRegStructToLCR (
    _In_ PSERIAL_LINE_CONTROL LineControlStruct,
    _Out_ UCHAR * LineControlRegister
    );

BOOLEAN
UartRecordInterrupt(
    _In_ WDFDEVICE Device
    );

VOID
UartEvaluateLineStatus(
    _In_ WDFDEVICE Device,
    _Out_ PBOOLEAN CanReceive,
    _Out_ PBOOLEAN CanTransmit,
    _Out_ PBOOLEAN HasErrors
    );

USHORT UartWaitableEvents(
    _In_ WDFDEVICE Device
    );
