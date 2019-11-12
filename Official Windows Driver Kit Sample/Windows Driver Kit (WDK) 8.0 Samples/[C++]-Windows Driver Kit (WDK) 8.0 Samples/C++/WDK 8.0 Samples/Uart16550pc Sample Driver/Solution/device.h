/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    device.h

Abstract:

    This module contains the 16550 UART controller's device functions.
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

#include "regutils.h"

typedef enum UART_TIMEOUT_WORK {
    TIMEOUT_START = 0,
    TIMEOUT_STOP
};

//
// Uart16550pc Device Context: UART_DEVICE_EXTENSION
//
typedef struct _UART_DEVICE_EXTENSION
{
    // This data structure will be refactored to some extent in a later update
    // Much of these data members is based upon the WDF Serial driver but some
    // will be eliminated.

    //
    // The WDFDEVICE and pointers to the DEVICE_OBJECT and DRIVER_OBJECT
    // associated with this instance of the controller driver.
    //
    WDFDEVICE WdfDevice;
    PDEVICE_OBJECT DeviceObject;
    PDRIVER_OBJECT DriverObject;
    
    //
    // This value indicates whether the device is
    // currently opened.
    //
    BOOLEAN DeviceOpened;
    
    //
    // This value indicates whether the device is
    // currently idle. It is only set to TRUE after
    // EvtDeviceD0EntyPostInterruptsEnabled is 
    // invoked, but is cleared to FALSE before 
    // EvtDeviceD0ExitPreInterruptsDisabled returns.
    //
    BOOLEAN DeviceActive;

    //
    // The base address for the set of device registers
    // of the serial port.
    //
    PUCHAR Controller;

    //
    // This value holds the span (in units of bytes) of the register
    // set controlling this port.  This is constant over the life
    // of the port.
    //
    ULONG SpanOfController;

    //
    // Address space
    //
    ULONG AddressSpace;

    //
    // Read Function Pointer
    //
    PREAD_PORT_UCHAR UartReadUChar;
    
    //
    // Write Function Pointer
    //
    PWRITE_PORT_UCHAR UartWriteUChar;

    //
    // Hold the clock rate input to the serial part.
    //
    ULONG ClockRate;

    //
    // The number of characters to push out if a fifo is present.
    //
    ULONG TxFifoAmount;

    //
    // Set to indicate that it is ok to share interrupts within the device.
    //
    ULONG PermitShare;

    //
    // Points to the interrupt object for used by this device.
    //
    WDFINTERRUPT WdfInterrupt;

    //
    // Translated vector
    //
    ULONG Vector;

    //
    // Translated Irql
    //
    KIRQL Irql;

    //
    // Interrupt mode
    //
    KINTERRUPT_MODE     InterruptMode;

    //
    // Interrupt affinity
    //
    KAFFINITY           Affinity;

    //
    // Set at intialization to indicate that on the current
    // architecture we need to unmap the base register address
    // when we unload the driver.
    //
    BOOLEAN UnMapRegisters;

    //
    // Holds the number of bytes remaining in the current write
    // request.
    //
    // This location is only accessed while at interrupt level.
    //
    ULONG WriteLength;

    //
    // Holds a pointer to the current character to be sent in
    // the current write.
    //
    // This location is only accessed while at interrupt level.
    //
    PUCHAR WriteCurrentChar;

    //
    // This mask will hold the bitmask sent down via the set mask
    // ioctl.  It is used by the interrupt service routine to determine
    // if the occurence of "events" (in the serial drivers understanding
    // of the concept of an event) should be noted.
    //
    ULONG IsrWaitMask;

    //
    // This mask will always be a subset of the IsrWaitMask.  While
    // at device level, if an event occurs that is "marked" as interesting
    // in the IsrWaitMask, the driver will turn on that bit in this
    // history mask.  The driver will then look to see if there is a
    // request waiting for an event to occur.  If there is one, it
    // will copy the value of the history mask into the wait request, zero
    // the history mask, and complete the wait request.  If there is no
    // waiting request, the driver will be satisfied with just recording
    // that the event occured.  If a wait request should be queued,
    // the driver will look to see if the history mask is non-zero.  If
    // it is non-zero, the driver will copy the history mask into the
    // request, zero the history mask, and then complete the request.
    //
    ULONG HistoryMask;

    //
    // This is a pointer to the where the history mask should be
    // placed when completing a wait.  It is only accessed at
    // device level.
    //
    // We have a pointer here to assist us to synchronize completing a wait.
    // If this is non-zero, then we have wait outstanding, and the isr still
    // knows about it.  We make this pointer null so that the isr won't
    // attempt to complete the wait.
    //
    // We still keep a pointer around to the wait request, since the actual
    // pointer to the wait request will be used for the "common" request completion
    // path.
    //
    ULONG *IrpMaskLocation;

    //
    // This mask holds all of the reason that transmission
    // is not proceeding.  Normal transmission can not occur
    // if this is non-zero.
    //
    // This is only written from interrupt level.
    // This could be (but is not) read at any level.
    //
    ULONG TXHolding;

    //
    // This mask holds all of the reason that reception
    // is not proceeding.  Normal reception can not occur
    // if this is non-zero.
    //
    // This is only written from interrupt level.
    // This could be (but is not) read at any level.
    //
    ULONG RXHolding;

    //
    // This holds the reasons that the driver thinks it is in
    // an error state.
    //
    // This is only written from interrupt level.
    // This could be (but is not) read at any level.
    //
    ULONG ErrorWord;

    //
    // This holds the current baud rate for the device.
    //
    ULONG CurrentBaud;

    //
    // This is the number of characters read since the XoffCounter
    // was started.  This variable is only accessed at device level.
    // If it is greater than zero, it implies that there is an
    // XoffCounter ioctl in the queue.
    //
    LONG CountSinceXoff;

    //
    // This ulong is incremented each time something trys to start
    // the execution path that tries to lower the RTS line when
    // doing transmit toggling.  If it "bumps" into another path
    // (indicated by a false return value from queueing a dpc
    // and a TRUE return value tring to start a timer) it will
    // decrement the count.  These increments and decrements
    // are all done at device level.  Note that in the case
    // of a bump while trying to start the timer, we have to
    // go up to device level to do the decrement.
    //
    ULONG CountOfTryingToLowerRTS;

    //
    // This ULONG is used to keep track of the "named" (in ntddser.h)
    // baud rates that this particular device supports.
    //
    ULONG SupportedBauds;

    //
    // This holds the various characters that are used
    // for replacement on errors and also for flow control.
    //
    // They are only set at interrupt level.
    //
    SERIAL_CHARS SpecialChars;

    //
    // This structure holds the handshake and control flow
    // settings for the serial driver.
    //
    // It is only set at interrupt level.  It can be
    // be read at any level with the control lock held.
    //
    SERIAL_HANDFLOW HandFlow;


    //
    // Holds performance statistics that applications can query.
    // Reset on each open.  Only set at device level.
    //
    SERIALPERF_STATS PerfStats;

    //
    // Holds the last written value of the Line Control Register (LCR)
    //
    UCHAR LineControl;

    //
    // Holds the last read value of the Interrupt Enable Register (IER)
    //
    UCHAR InterruptEnableRegister;

    //
    // Holds the last read value of the Line Status Register (LSR)
    //
    UCHAR LineStatus;

    UCHAR ModemStatus;

    UCHAR ReceiveBuffer;

    UCHAR FifoControl;

    UCHAR ModemControl;

    USHORT DivisorLatch;

    //
    // Hold the last read value of the Interrupt Identifier Register (IIR)
    //
    UCHAR InterruptIdentifier;

    //
    // This is only accessed at interrupt level.  It keeps track
    // of whether the holding register is empty.
    //
    BOOLEAN HoldingEmpty;

    //
    // This is only accessed at interrupt level.  It keeps track
    // of the receive flow control condition.
    //
    BOOLEAN ReceiveFlowControlAsserted;

    //
    // If true automatic RX flow control is enabled and calls to 
    // UartFlowReceiveAvailable() and UartFlowReceiveFull() will
    // have no effect, and ReceiveFlowControlAsserted should be
    // ignored.  Automatic RX flow control is necessary for RX DMA.
    //
    BOOLEAN AutoRXFlowControlEnabled;

    //
    // If true automatic TX flow control is enabled.  This is
    // necessary for TX DMA.
    //
    BOOLEAN AutoTXFlowControlEnabled;

    //
    // This variable is only accessed at interrupt level.  Whenever
    // a wait is initiated this variable is set to false.
    // Whenever any kind of character is written it is set to true.
    // Whenever the write queue is found to be empty the code that
    // is processing that completing request will synchonize with the interrupt.
    // If this synchronization code finds that the variable is true and that
    // there is a wait on the transmit queue being empty then it is
    // certain that the queue was emptied and that it has happened since
    // the wait was initiated.
    //
    BOOLEAN EmptiedTransmit;

    //
    // This holds the mask that will be used to mask off unwanted
    // data bits of the received data (valid data bits can be 5,6,7,8)
    // The mask will normally be 0xff.  This is set while the control
    // lock is held since it wouldn't have adverse effects on the
    // isr if it is changed in the middle of reading characters.
    // (What it would do to the app is another question - but then
    // the app asked the driver to do it.)
    //
    UCHAR ValidDataMask;

    //
    // The application can turn on a mode,via the
    // IOCTL_SERIAL_LSRMST_INSERT ioctl, that will cause the
    // serial driver to insert the line status or the modem
    // status into the RX stream.  The parameter with the ioctl
    // is a pointer to a UCHAR.  If the value of the UCHAR is
    // zero, then no insertion will ever take place.  If the
    // value of the UCHAR is non-zero (and not equal to the
    // xon/xoff characters), then the serial driver will insert.
    //
    UCHAR EscapeChar;

    //
    // These two booleans are used to indicate to the isr transmit
    // code that it should send the xon or xoff character.  They are
    // only accessed at open and at interrupt level.
    //
    BOOLEAN SendXonChar;
    BOOLEAN SendXoffChar;

    //
    // This boolean will be true if a 16550 is present *and* enabled.
    //
    BOOLEAN FifoPresent;

    //
    // This value is used to indicate whether a new configuration
    // is outstanding. This, in conjunction with UartApplyConfig, 
    // ensures hardware state is not overwritten when entering/exiting D0.
    //
    BOOLEAN NewConfigOutstanding;

    // This boolean will be set during the file open callback, and 
    // indicates that the driver should reset the FIFOs the first
    // time the device enters D0.
    BOOLEAN ResetFifoOnD0Entry;

    //
    // This is the water mark that the rxfifo should be
    // set to when the fifo is turned on.  This is not the actual
    // value, but the encoded value that goes into the register.
    //
    UCHAR RxFifoTrigger;

    USHORT TxFifoSize;
    USHORT RxFifoSize;

    WDFSPINLOCK DpcSpinLock;

    //
    // DMA objects used to setup DMA and do DMA transfers.  They
    // are created on startup and released on shutdown.
    //
    WDFDMAENABLER TxDmaEnabler;
    WDFDMAENABLER RxDmaEnabler;
    WDFDMATRANSACTION DmaTransmitTransaction;
    WDFDMATRANSACTION DmaReceiveTransaction;

    //
    // WDM DMA Adapter object.  This is only really needed for its
    // ReadDmaCounter routine to query number of bytes actually transferred
    // using DMA, as WDF currently has no way to do this using WDFDMATRANSACTIONs
    //
    PDMA_ADAPTER readAdapter;
    PDMA_ADAPTER writeAdapter;

    //
    // These values denote that DMA has been configured using resources
    // provided by firmware.  If false, the driver will not attempt to 
    // schedule a DMA transfer in that direction
    //
    BOOLEAN DmaTransmitConfigured;
    BOOLEAN DmaReceiveConfigured;

    //
    // These values denote that the driver is using Dma for the current transfer
    //
    BOOLEAN DmaTransmitEnabled;
    BOOLEAN DmaReceiveEnabled;

    //
    // Store information about DMA properties, especially minimum transfer unit
    //
    DMA_ADAPTER_INFO DmaReadInfo;
    DMA_ADAPTER_INFO DmaWriteInfo;

    //
    // The number of bytes to be transferred over DMA.
    //
    ULONG DmaTransmitLength;
    ULONG DmaReceiveLength;

    //
    // Store MDL pointers, for use while buffer is owned by
    // controller driver.
    //
    PMDL TransmitMdl;
    PMDL ReceiveMdl;

    //
    // Buffers retrieved from the class extension are cached here
    // until filled, or I/O is cancelled or timed out.
    //
    SERCX_BUFFER_DESCRIPTOR PIOTransmitBuffer;
    SERCX_BUFFER_DESCRIPTOR PIOReceiveBuffer;

    PUCHAR FIFOBuffer;
    PUCHAR FIFOBufferNextByte;
    ULONG FIFOBufferBytes;
    ULONG FIFOBufferTag;

    //
    // Number of bytes transferred to or from cached buffer
    //
    ULONG TransmitProgress;
    ULONG ReceiveProgress;

    //
    // Locks around cached buffer access
    //
    WDFSPINLOCK TransmitBufferSpinLock;
    WDFSPINLOCK ReceiveBufferSpinLock;

    //
    // Timer used to implement interval timeout while holding receive buffer
    //
    WDFTIMER TimeoutTimer;
    ULONGLONG TimeoutLengthMs;

    // Since DMA cancellation occurs in a separate callback function, we use this flag only in the 
    // DMA receive transfer, timeout, and cancellation paths to let the callback know that DMA was
    // cancelled by an interval timeout
    BOOLEAN TimeoutTimerTimedOut;
}
UART_DEVICE_EXTENSION,*PUART_DEVICE_EXTENSION;

WDF_DECLARE_CONTEXT_TYPE_WITH_NAME(UART_DEVICE_EXTENSION, 
                                   UartGetDeviceExtension);

//
// Uart16550pc Function: UartDeviceCreate
//
NTSTATUS
UartDeviceCreate(
    PWDFDEVICE_INIT DeviceInit
    );

//
// Uart16550pc Event Handler: UartEvtPrepareHardware
//
EVT_WDF_DEVICE_PREPARE_HARDWARE UartEvtPrepareHardware;

//
// Uart16550pc Event Handler: UartEvtReleaseHardware
//
EVT_WDF_DEVICE_RELEASE_HARDWARE UartEvtReleaseHardware;

//
// Uart16550pc Event Handler: UartEvtD0Entry
//
EVT_WDF_DEVICE_D0_ENTRY UartEvtD0Entry;

//
// Uart16550pc Event Handler: UartEvtD0Exit
//
EVT_WDF_DEVICE_D0_EXIT UartEvtD0Exit;

//
// Uart16550pc Event Handler: UartEvtInterruptEnable
//
EVT_WDF_INTERRUPT_ENABLE UartEvtInterruptEnable;

//
// Uart16550pc Event Handler: UartEvtInterruptDisable
//
EVT_WDF_INTERRUPT_DISABLE UartEvtInterruptDisable;

//
// Uart16550pc Event Handler: UartEvtD0EntryPostInterruptsEnabled
//
EVT_WDF_DEVICE_D0_ENTRY_POST_INTERRUPTS_ENABLED UartEvtD0EntryPostInterruptsEnabled;

//
// Uart16550pc Event Handler: UartEvtD0ExitPreInterruptsDisabled
//
EVT_WDF_DEVICE_D0_EXIT_PRE_INTERRUPTS_DISABLED UartEvtD0ExitPreInterruptsDisabled;

//
// Uart16550pc Function: UartEnableInterrupts
//
VOID
UartEnableInterrupts(
    _In_ PUART_DEVICE_EXTENSION pDevExt
    );

//
// Uart16550pc Function: UartReceiveBytesFromRXFIFO
//
NTSTATUS
UartReceiveBytesFromRXFIFO(
    _In_ WDFDEVICE Device,
    _In_ PUART_DEVICE_EXTENSION pDevExt,
    _In_ BOOLEAN canRetrieveBuffer,
    _In_ BOOLEAN canChangeReceiveFlow);

//
// Uart16550pc Function: UartReceiveBytesToSoftwareFIFO
//
VOID
UartReceiveBytesToSoftwareFIFO(
    _In_ WDFDEVICE Device,
    _In_ PUART_DEVICE_EXTENSION pDevExt);

//
// Uart16550pc Function: UartReceiveBytesFromSoftwareFIFO
//
VOID
UartReceiveBytesFromSoftwareFIFO(
    _In_ PUART_DEVICE_EXTENSION pDevExt);

//
// Uart16550pc Function: UartUseDma
//
BOOLEAN
UartUseDma(
    _In_ PUART_DEVICE_EXTENSION pDevExt,
    _In_ WDF_DMA_DIRECTION Direction,
    _In_ size_t Length);

//
// Uart16550pc Function: HasCachedTransmitBuffer
//
BOOLEAN
HasCachedTransmitBuffer(
    _In_ PUART_DEVICE_EXTENSION pDevExt);

//
// Uart16550pc Function: HasCachedReceiveBuffer
//
BOOLEAN
HasCachedReceiveBuffer(
    _In_ PUART_DEVICE_EXTENSION pDevExt);

//
// Uart16550pc Function: GetNextTransmitBufferByte
//
UCHAR GetNextTransmitBufferByte(
    _In_ PUART_DEVICE_EXTENSION pDevExt);

//
// Uart16550pc Function: GetNextReceiveBufferByteAddress
//
PUCHAR GetNextReceiveBufferByteAddress(
    _In_ PUART_DEVICE_EXTENSION pDevExt);

//
// Uart16550pc Function: GetTransmitBufferLength
//
ULONG
GetTransmitBufferLength(
    _In_ PUART_DEVICE_EXTENSION pDevExt);

//
// Uart16550pc Function: GetReceiveBufferLength
//
ULONG
GetReceiveBufferLength(
    _In_ PUART_DEVICE_EXTENSION pDevExt);

//
// Uart16550pc Function: GetMdlByteAddressAt
//
PUCHAR
GetMdlByteAddressAt(
    _In_ PMDL baseMdl,
    _In_ ULONG index);

//
// Uart16550pc Function: GetMdlLength
//
ULONG 
GetMdlLength(
    _In_ PMDL baseMdl);