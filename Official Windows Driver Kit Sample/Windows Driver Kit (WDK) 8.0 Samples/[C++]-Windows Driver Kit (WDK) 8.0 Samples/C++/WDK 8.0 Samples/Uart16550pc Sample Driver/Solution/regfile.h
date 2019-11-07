/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    regfile.h

Abstract:

    This module contains the 16550 UART controller's register file #defines.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

    These macros borrow heavily from code in the WDF Serial example.

--*/

#pragma once

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>

// Class Extension includes
#include "SerCx.h"

//
// This value - which could be redefined at compile
// time, define the stride between registers
//
#if !defined(UART_REGISTER_STRIDE)
#define UART_REGISTER_STRIDE 1
#endif

//
// Offsets from the base register address of the
// various registers for the 8250 family of UARTS.
//
#define RECEIVE_BUFFER_REGISTER    ((ULONG)((0x00)*UART_REGISTER_STRIDE))
#define TRANSMIT_HOLDING_REGISTER  ((ULONG)((0x00)*UART_REGISTER_STRIDE))
#define INTERRUPT_ENABLE_REGISTER  ((ULONG)((0x01)*UART_REGISTER_STRIDE))
#define INTERRUPT_IDENT_REGISTER   ((ULONG)((0x02)*UART_REGISTER_STRIDE))
#define FIFO_CONTROL_REGISTER      ((ULONG)((0x02)*UART_REGISTER_STRIDE))
#define LINE_CONTROL_REGISTER      ((ULONG)((0x03)*UART_REGISTER_STRIDE))
#define MODEM_CONTROL_REGISTER     ((ULONG)((0x04)*UART_REGISTER_STRIDE))
#define LINE_STATUS_REGISTER       ((ULONG)((0x05)*UART_REGISTER_STRIDE))
#define MODEM_STATUS_REGISTER      ((ULONG)((0x06)*UART_REGISTER_STRIDE))
#define DIVISOR_LATCH_LSB          ((ULONG)((0x00)*UART_REGISTER_STRIDE))
#define DIVISOR_LATCH_MSB          ((ULONG)((0x01)*UART_REGISTER_STRIDE))
#define SERIAL_REGISTER_SPAN       ((ULONG)(7*UART_REGISTER_STRIDE))

//
// If we have an interrupt status register this is its assumed
// length.
//
#define UART_STATUS_LENGTH       ((ULONG)(1*UART_REGISTER_STRIDE))

//
// Bitmask definitions for accessing the 8250 device registers.
//

//
// These bits define the number of data bits trasmitted in
// the Serial Data Unit (SDU - Start,data, parity, and stop bits)
//
#define SERIAL_DATA_LENGTH_5 0x00
#define SERIAL_DATA_LENGTH_6 0x01
#define SERIAL_DATA_LENGTH_7 0x02
#define SERIAL_DATA_LENGTH_8 0x03


//
// These masks define the interrupts that can be enabled or disabled.
//
//
// This interrupt is used to notify that there is new incomming
// data available.  The SERIAL_RDA interrupt is enabled by this bit.
//
#define SERIAL_IER_RDA   0x01

//
// This interrupt is used to notify that there is space available
// in the transmitter for another character.  The SERIAL_THR
// interrupt is enabled by this bit.
//
#define SERIAL_IER_THR   0x02

//
// This interrupt is used to notify that some sort of error occured
// with the incomming data.  The SERIAL_RLS interrupt is enabled by
// this bit.
#define SERIAL_IER_RLS   0x04

//
// This interrupt is used to notify that some sort of change has
// taken place in the modem control line.  The SERIAL_MS interrupt is
// enabled by this bit.
//
#define SERIAL_IER_MS    0x08


//
// These masks define the values of the interrupt identification
// register.  The low bit must be clear in the interrupt identification
// register for any of these interrupts to be valid.  The interrupts
// are defined in priority order, with the highest value being most
// important.  See above for a description of what each interrupt
// implies.
//
#define SERIAL_IIR_RLS      0x06
#define SERIAL_IIR_RDA      0x04
#define SERIAL_IIR_CTI      0x0c
#define SERIAL_IIR_THR      0x02
#define SERIAL_IIR_MS       0x00
#define SERIAL_IIR_MASK     0x0f

//
// This bit mask get the value of the high two bits of the
// interrupt id register.  If this is a 16550 class chip
// these bits will be a one if the fifo's are enbled, otherwise
// they will always be zero.
//
#define SERIAL_IIR_FIFOS_ENABLED 0xc0

//
// If the low bit is logic one in the interrupt identification register
// this implies that *NO* interrupts are pending on the device.
//
#define SERIAL_IIR_NO_INTERRUPT_PENDING 0x01


//
// Use these bits to detect removal of serial card for Stratus implementation
//
#define SERIAL_IIR_MUST_BE_ZERO 0x30


//
// These masks define access to the fifo control register.
//

//
// Enabling this bit in the fifo control register will turn
// on the fifos.  If the fifos are enabled then the high two
// bits of the interrupt id register will be set to one.  Note
// that this only occurs on a 16550 class chip.  If the high
// two bits in the interrupt id register are not one then
// we know we have a lower model chip.
//
//
#define SERIAL_FCR_ENABLE     ((UCHAR)0x01)
#define SERIAL_FCR_RCVR_RESET ((UCHAR)0x02)
#define SERIAL_FCR_TXMT_RESET ((UCHAR)0x04)

//
// This set of values define the high water marks (when the
// interrupts trip) for the receive fifo.
//
#define SERIAL_1_BYTE_HIGH_WATER   ((UCHAR)0x00)
#define SERIAL_4_BYTE_HIGH_WATER   ((UCHAR)0x40)
#define SERIAL_8_BYTE_HIGH_WATER   ((UCHAR)0x80)
#define SERIAL_14_BYTE_HIGH_WATER  ((UCHAR)0xc0)

//
// These values define the default FIFO sizes until
// the actual settings can be read from ACPI.
//

#define SERIAL_TX_FIFO_SIZE_DEFAULT  16
#define SERIAL_RX_FIFO_SIZE_DEFAULT  16

//
// This value defines the size of the softare FIFO
//
#define SERIAL_SOFTWARE_FIFO_SIZE 32
#define SERIAL_SOFTWARE_FIFO_TAG 'CPRU'

//
// These masks define access to the line control register.
//

//
// This defines the bit used to control the definition of the "first"
// two registers for the 8250.  These registers are the input/output
// register and the interrupt enable register.  When the DLAB bit is
// enabled these registers become the least significant and most
// significant bytes of the divisor value.
//
#define SERIAL_LCR_DLAB     0x80

//
// This defines the bit used to control whether the device is sending
// a break.  When this bit is set the device is sending a space (logic 0).
//
// Most protocols will assume that this is a hangup.
//
#define SERIAL_LCR_BREAK    0x40

//
// These defines are used to set the line control register.
//
#define SERIAL_5_DATA       ((UCHAR)0x00)
#define SERIAL_6_DATA       ((UCHAR)0x01)
#define SERIAL_7_DATA       ((UCHAR)0x02)
#define SERIAL_8_DATA       ((UCHAR)0x03)
#define SERIAL_DATA_MASK    ((UCHAR)0x03)

#define SERIAL_1_STOP       ((UCHAR)0x00)
#define SERIAL_1_5_STOP     ((UCHAR)0x04) // Only valid for 5 data bits
#define SERIAL_2_STOP       ((UCHAR)0x04) // Not valid for 5 data bits
#define SERIAL_STOP_MASK    ((UCHAR)0x04)

#define SERIAL_NONE_PARITY  ((UCHAR)0x00)
#define SERIAL_ODD_PARITY   ((UCHAR)0x08)
#define SERIAL_EVEN_PARITY  ((UCHAR)0x18)
#define SERIAL_MARK_PARITY  ((UCHAR)0x28)
#define SERIAL_SPACE_PARITY ((UCHAR)0x38)
#define SERIAL_PARITY_MASK  ((UCHAR)0x38)

//
// These masks define access the modem control register.
//

//
// This bit controls the data terminal ready (DTR) line.  When
// this bit is set the line goes to logic 0 (which is then inverted
// by normal hardware).  This is normally used to indicate that
// the device is available to be used.  Some odd hardware
// protocols (like the kernel debugger) use this for handshaking
// purposes.
//
#define SERIAL_MCR_DTR            0x01

//
// This bit controls the ready to send (RTS) line.  When this bit
// is set the line goes to logic 0 (which is then inverted by the normal
// hardware).  This is used for hardware handshaking.  It indicates that
// the hardware is ready to send data and it is waiting for the
// receiving end to set clear to send (CTS).
//
#define SERIAL_MCR_RTS            0x02

//
// This bit is used for general purpose output.
//
#define SERIAL_MCR_OUT1           0x04

//
// This bit is used for general purpose output.
//
#define SERIAL_MCR_OUT2           0x08

//
// This bit controls the loopback testing mode of the device.  Basically
// the outputs are connected to the inputs (and vice versa).
//
#define SERIAL_MCR_LOOP           0x10

//
// This bit enables auto flow control on a TI TL16C550C/TL16C550CI
//

#define SERIAL_MCR_TL16C550CAFE   0x20


//
// These masks define access to the line status register.  The line
// status register contains information about the status of data
// transfer.  The first five bits deal with receive data and the
// last two bits deal with transmission.  An interrupt is generated
// whenever bits 1 through 4 in this register are set.
//

//
// This bit is the data ready indicator.  It is set to indicate that
// a complete character has been received.  This bit is cleared whenever
// the receive buffer register has been read.
//
#define SERIAL_LSR_DR       0x01

//
// This is the overrun indicator.  It is set to indicate that the receive
// buffer register was not read befor a new character was transferred
// into the buffer.  This bit is cleared when this register is read.
//
#define SERIAL_LSR_OE       0x02

//
// This is the parity error indicator.  It is set whenever the hardware
// detects that the incoming serial data unit does not have the correct
// parity as defined by the parity select in the line control register.
// This bit is cleared by reading this register.
//
#define SERIAL_LSR_PE       0x04

//
// This is the framing error indicator.  It is set whenever the hardware
// detects that the incoming serial data unit does not have a valid
// stop bit.  This bit is cleared by reading this register.
//
#define SERIAL_LSR_FE       0x08

//
// This is the break interrupt indicator.  It is set whenever the data
// line is held to logic 0 for more than the amount of time it takes
// to send one serial data unit.  This bit is cleared whenever the
// this register is read.
//
#define SERIAL_LSR_BI       0x10

#define SERIAL_LSR_ERROR    0x1E

//
// This is the transmit holding register empty indicator.  It is set
// to indicate that the hardware is ready to accept another character
// for transmission.  This bit is cleared whenever a character is
// written to the transmit holding register.
//
#define SERIAL_LSR_THRE     0x20

//
// This bit is the transmitter empty indicator.  It is set whenever the
// transmit holding buffer is empty and the transmit shift register
// (a non-software accessable register that is used to actually put
// the data out on the wire) is empty.  Basically this means that all
// data has been sent.  It is cleared whenever the transmit holding or
// the shift registers contain data.
//
#define SERIAL_LSR_TEMT     0x40

//
// This bit indicates that there is at least one error in the fifo.
// The bit will not be turned off until there are no more errors
// in the fifo.
//
#define SERIAL_LSR_FIFOERR  0x80


//
// These masks are used to access the modem status register.
// Whenever one of the first four bits in the modem status
// register changes state a modem status interrupt is generated.
//

//
// This bit is the delta clear to send.  It is used to indicate
// that the clear to send bit (in this register) has *changed*
// since this register was last read by the CPU.
//
#define SERIAL_MSR_DCTS     0x01

//
// This bit is the delta data set ready.  It is used to indicate
// that the data set ready bit (in this register) has *changed*
// since this register was last read by the CPU.
//
#define SERIAL_MSR_DDSR     0x02

//
// This is the trailing edge ring indicator.  It is used to indicate
// that the ring indicator input has changed from a low to high state.
//
#define SERIAL_MSR_TERI     0x04

//
// This bit is the delta data carrier detect.  It is used to indicate
// that the data carrier bit (in this register) has *changed*
// since this register was last read by the CPU.
//
#define SERIAL_MSR_DDCD     0x08

//
// This bit contains the (complemented) state of the clear to send
// (CTS) line.
//
#define SERIAL_MSR_CTS      0x10

//
// This bit contains the (complemented) state of the data set ready
// (DSR) line.
//
#define SERIAL_MSR_DSR      0x20

//
// This bit contains the (complemented) state of the ring indicator
// (RI) line.
//
#define SERIAL_MSR_RI       0x40

//
// This bit contains the (complemented) state of the data carrier detect
// (DCD) line.
//
#define SERIAL_MSR_DCD      0x80

//
// The following bits in the modem status register are used
// to indicate a register value change.
//
#define SERIAL_MSR_EVENTS   (SERIAL_MSR_DCTS | \
                             SERIAL_MSR_DDSR | \
                             SERIAL_MSR_TERI | \
                             SERIAL_MSR_RI)

//
// This should be more than enough space to hold then
// numeric suffix of the device name.
//
#define DEVICE_NAME_DELTA 20


//
// Up to 16 Ports Per card.  However for sixteen
// port cards the interrupt status register must me
// the indexing kind rather then the bitmask kind.
//
//
#define SERIAL_MAX_PORTS_INDEXED (16)
#define SERIAL_MAX_PORTS_NONINDEXED (8)

typedef struct _UART_CONFIG_DATA {
    PHYSICAL_ADDRESS    ControllerRaw;
    PHYSICAL_ADDRESS    ControllerTranslated;
    ULONG               SpanOfController;
    ULONG               AddressSpace;
    ULONG               VectorTranslated;
    ULONG               IrqlTranslated;
    KAFFINITY           Affinity;

    ULONG               ClockRate;
    ULONG               DisablePort;
    ULONG               ForceFifoEnable;
    ULONG               RxFIFO;
    ULONG               TxFIFO;
    //ULONG               PermitShare;
    //ULONG               PermitSystemWideShare;
    //ULONG               LogFifo;
    KINTERRUPT_MODE     InterruptMode;
    ULONG               TL16C550CAFC;
    PCM_PARTIAL_RESOURCE_DESCRIPTOR DmaResourceDescriptor[2];
}
UART_CONFIG_DATA,*PUART_CONFIG_DATA;


//
// This structure contains configuration data, much of which
// is read from the registry.
//
typedef struct _SERIAL_FIRMWARE_DATA {
    PDRIVER_OBJECT  DriverObject;
    ULONG           ControllersFound;
    ULONG           ForceFifoEnableDefault;
    ULONG           DebugLevel;
    ULONG           ShouldBreakOnEntry;
    ULONG           RxFIFODefault;
    ULONG           TxFIFODefault;
    ULONG           PermitShareDefault;
    ULONG           PermitSystemWideShare;
    ULONG           LogFifoDefault;
    ULONG           UartRemovalDetect;
    UNICODE_STRING  Directory;
    UNICODE_STRING  NtNameSuffix;
    UNICODE_STRING  DirectorySymbolicName;
    LIST_ENTRY      ConfigList;
} SERIAL_FIRMWARE_DATA,*PSERIAL_FIRMWARE_DATA;

//
// Default xon/xoff characters.
//
#define SERIAL_DEF_XON 0x11
#define SERIAL_DEF_XOFF 0x13

//
// Reasons that recption may be held up.
//
#define SERIAL_RX_DTR       ((ULONG)0x01)
#define SERIAL_RX_XOFF      ((ULONG)0x02)
#define SERIAL_RX_RTS       ((ULONG)0x04)
#define SERIAL_RX_DSR       ((ULONG)0x08)

//
// Reasons that transmission may be held up.
//
#define SERIAL_TX_CTS       ((ULONG)0x01)
#define SERIAL_TX_DSR       ((ULONG)0x02)
#define SERIAL_TX_DCD       ((ULONG)0x04)
#define SERIAL_TX_XOFF      ((ULONG)0x08)
#define SERIAL_TX_BREAK     ((ULONG)0x10)

//
// These values are used by the routines that can be used
// to complete a read (other than interval timeout) to indicate
// to the interval timeout that it should complete.
//
#define SERIAL_COMPLETE_READ_CANCEL ((LONG)-1)
#define SERIAL_COMPLETE_READ_TOTAL ((LONG)-2)
#define SERIAL_COMPLETE_READ_COMPLETE ((LONG)-3)

//
// These are default values that shouldn't appear in the registry
//
#define SERIAL_BAD_VALUE ((ULONG)-1)

//
// Uart16550pc Function Type: PREAD_PORT_UCHAR
//
typedef
UCHAR
(*PREAD_PORT_UCHAR)(
    _In_reads_(_Inexpressible_(Offset)) PUCHAR BaseAddress,
    _In_ ULONG Offset
    );

//
// Uart16550pc Function Type: PWRITE_PORT_UCHAR
//
typedef
VOID
(*PWRITE_PORT_UCHAR)(
    _In_reads_(_Inexpressible_(Offset)) PUCHAR BaseAddress,
    _In_ ULONG Offset,
    _In_ UCHAR Value
    );


UCHAR
UartReadPortUChar(
    _In_reads_(_Inexpressible_(Offset)) PUCHAR BaseAddress,
    _In_ ULONG Offset
    );

VOID
UartWritePortUChar(
    _In_reads_(_Inexpressible_(Offset)) PUCHAR BaseAddress,
    _In_ ULONG Offset,
    _In_ UCHAR y
    );

UCHAR
UartReadRegisterUChar(
    _In_reads_(_Inexpressible_(Offset)) PUCHAR BaseAddress,
    _In_ ULONG Offset
    );

VOID
UartWriteRegisterUChar (
    _In_reads_(_Inexpressible_(Offset)) PUCHAR BaseAddress,
    _In_ ULONG Offset,
    _In_ UCHAR y
    );

//
// Uart16550pc Function: UartMapHardwareResources
//
NTSTATUS
UartMapHardwareResources(
    _In_ WDFDEVICE Device,
    _In_ WDFCMRESLIST Resources,
    _In_ WDFCMRESLIST ResourcesTranslated,
    _Out_ PUART_CONFIG_DATA PConfig
    );

//
// Uart16550pc Function: UartGetMappedAddress
//
PVOID
UartGetMappedAddress(
    _In_ PHYSICAL_ADDRESS IoAddress,
    _In_ ULONG NumberOfBytes,
    _In_ ULONG AddressSpace,
    _Out_ PBOOLEAN MappedAddress
    );

//
// Uart16550pc Function: UartInitController
//
NTSTATUS
UartInitController(
    _In_ WDFDEVICE Device,
    _In_ PUART_CONFIG_DATA PConfig
    );

//
// Uart16550pc Function: UartSaveStatePreInterruptsDisabled
//
NTSTATUS
UartSaveStatePreInterruptsDisabled(
    _In_ WDFDEVICE Device
    );

//
// Uart16550pc Function: UartSaveState
//
NTSTATUS
UartSaveState(
    _In_ WDFDEVICE Device
    );

//
// Uart16550pc Function: UartRestoreState
//
NTSTATUS
UartRestoreState(
    _In_ WDFDEVICE Device
    );

//
// Uart16550pc Function: UartRestoreStatePostInterruptsEnabled
//
NTSTATUS
UartRestoreStatePostInterruptsEnabled(
    _In_ WDFDEVICE Device
    );

//
// Uart16550pc Function: UartInitCachedBuffers
//
NTSTATUS
UartInitCachedBuffers(
    _In_ WDFDEVICE Device
    );

//
// Uart16550pc Function: UartInitDMA
//
NTSTATUS
UartInitDMA(
    _In_ WDFDEVICE Device,
    _In_ PUART_CONFIG_DATA PConfig,
    _In_ ULONG DmaResourceCount
    );