/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    ioctls.cpp

Abstract:

    This module contains the 16550 UART controller's IOCTL implementations.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

--*/

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>
#include "ntddser.h"

// Class Extension includes
#include "SerCx.h"

#include "ioctls.h"
#include "regfile.h"
#include "regutils.h"
#include "device.h"
#include "flow.h"

#include "tracing.h"
#include "ioctls.tmh"


////////////////////////
// Internal Functions //
////////////////////////

//
// Internal Function: UartCtlSetBaudRate
//
VOID
UartCtlSetBaudRate(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status = STATUS_UNSUCCESSFUL;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PSERIAL_BAUD_RATE pBaudRate = NULL;
    USHORT DivisorLatchRegs = 0;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveInputBuffer(Request, 
                    sizeof(SERIAL_BAUD_RATE), 
                    (PVOID*)(& pBaudRate), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve input buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        status = UartRegConvertAndValidateBaud(
            pBaudRate->BaudRate, 
            &DivisorLatchRegs);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_CONTROL,
                "Failed to convert and validate baudrate %lu - "
                "%!STATUS!",
                pBaudRate->BaudRate,
                status);
        }
    }

    if (NT_SUCCESS(status))
    {        
        // Acquires the interrupt lock and writes the Divisor Latch.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        pDevExt->CurrentBaud = pBaudRate->BaudRate;
        WRITE_DIVISOR_LATCH(pDevExt, pDevExt->Controller, DivisorLatchRegs);

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);
    
    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlGetBaudRate
//
VOID
UartCtlGetBaudRate(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status = STATUS_UNSUCCESSFUL;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PSERIAL_BAUD_RATE pBaudRate = NULL;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveOutputBuffer(Request, 
                    sizeof(SERIAL_BAUD_RATE), 
                    (PVOID*)(& pBaudRate), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve output buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        // Acquires the interrupt lock and retrieves the current baud rate.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        pBaudRate->BaudRate = pDevExt->CurrentBaud;
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        WdfRequestSetInformation(Request, sizeof(SERIAL_BAUD_RATE));
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}


//
// Internal Function: UartCtlGetModemControl
//
VOID
UartCtlGetModemControl(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PULONG pBuffer;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveOutputBuffer(Request, 
                    sizeof(ULONG), 
                    (PVOID*)(&pBuffer), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve output buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        // Acquires the interrupt lock and reads the modem control register.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        *pBuffer = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        WdfRequestSetInformation(Request, sizeof(ULONG));
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}


//
// Internal Function: UartCtlSetModemControl
//
VOID
UartCtlSetModemControl(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PULONG pBuffer;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveInputBuffer(Request, 
                    sizeof(ULONG), 
                    (PVOID*)(&pBuffer), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve input buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        // Acquires the interrupt lock and writes the modem control register.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, (UCHAR)*pBuffer);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlSetFifoControl
//
VOID
UartCtlSetFifoControl(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status = STATUS_UNSUCCESSFUL;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    UCHAR fifoControlRegister = 0;
    UCHAR fifoControlRegisterSaved = 0;
    PUCHAR pFifoControl = NULL;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveInputBuffer(Request, 
                    sizeof(UCHAR), 
                    (PVOID*)(& pFifoControl), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve input buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        // This value is not verified.  This is as specified in the documentation.
        fifoControlRegister = *pFifoControl;

        // Save the value of the FCR to be written, with the reset bits unset.
        fifoControlRegisterSaved = fifoControlRegister & 
                        (~(SERIAL_FCR_RCVR_RESET | SERIAL_FCR_TXMT_RESET));

        // Acquires the interrupt lock and writes the FCR.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        WRITE_FIFO_CONTROL(pDevExt, pDevExt->Controller, fifoControlRegister);
        pDevExt->FifoControl = fifoControlRegisterSaved;
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlSetLineControl
//
VOID
UartCtlSetLineControl(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status = STATUS_UNSUCCESSFUL;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    UCHAR lineControlRegister = 0;
    PSERIAL_LINE_CONTROL pLineControl = NULL;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveInputBuffer(Request, 
                    sizeof(SERIAL_LINE_CONTROL), 
                    (PVOID*)(& pLineControl), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve input buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        status = UartRegStructToLCR(pLineControl, &lineControlRegister);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_CONTROL,
                "Failed to calculate LCR from SERIAL_LINE_CONTROL %p  - "
                "%!STATUS!",
                pLineControl,
                status);
        }
    }

    if (NT_SUCCESS(status))
    {
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        // Set line control, save break setting
        lineControlRegister = lineControlRegister | 
            (READ_LINE_CONTROL(pDevExt, pDevExt->Controller) & SERIAL_LCR_BREAK);
        WRITE_LINE_CONTROL(pDevExt, pDevExt->Controller, lineControlRegister);

        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlGetLineControl
//
VOID
UartCtlGetLineControl(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status = STATUS_UNSUCCESSFUL;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    UCHAR lineControlRegister = 0;
    PSERIAL_LINE_CONTROL pLineControl = NULL;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveOutputBuffer(Request, 
                    sizeof(SERIAL_LINE_CONTROL), 
                    (PVOID*)(& pLineControl), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve output buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        // Acquires the interrupt lock and reads the LCR.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        lineControlRegister = READ_LINE_CONTROL(pDevExt, pDevExt->Controller);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);   

        status = UartRegLCRToStruct(lineControlRegister, pLineControl);

        if (!NT_SUCCESS(status))
        {
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_CONTROL,
                "Failed to calculate SERIAL_LINE_CONTROL from LCR %c  - "
                "%!STATUS!",
                lineControlRegister,
                status);
        }
    }

    if (NT_SUCCESS(status))
    {
        WdfRequestSetInformation(Request, sizeof(SERIAL_LINE_CONTROL));
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlSetChars
//
VOID
UartCtlSetChars(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status = STATUS_UNSUCCESSFUL;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PSERIAL_CHARS pSpecialChars = NULL;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveInputBuffer(Request, 
                    sizeof(SERIAL_CHARS), 
                    (PVOID*)(& pSpecialChars), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve input buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        //
        // Software flow control and in-band signaling
        // have not been implemented in this sample. Characters
        // are not checked for valid range or values.
        //

        pDevExt->SpecialChars.EofChar = pSpecialChars->EofChar;
        pDevExt->SpecialChars.ErrorChar = pSpecialChars->ErrorChar;
        pDevExt->SpecialChars.BreakChar = pSpecialChars->BreakChar;
        pDevExt->SpecialChars.EventChar = pSpecialChars->EventChar;
        pDevExt->SpecialChars.XonChar = pSpecialChars->XonChar;
        pDevExt->SpecialChars.XoffChar = pSpecialChars->XoffChar;
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlGetChars
//
VOID
UartCtlGetChars(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status = STATUS_UNSUCCESSFUL;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PSERIAL_CHARS pSpecialChars = NULL;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveOutputBuffer(Request, 
                    sizeof(SERIAL_CHARS), 
                    (PVOID*)(& pSpecialChars), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve output buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        *pSpecialChars = pDevExt->SpecialChars;

        WdfRequestSetInformation(Request, sizeof(SERIAL_CHARS));
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlSetDtr
//
VOID
UartCtlSetDtr(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    UCHAR regModemControl;
    NTSTATUS status = STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    if ((pDevExt->HandFlow.ControlHandShake & SERIAL_DTR_MASK) ==
        SERIAL_DTR_HANDSHAKE)
    {
        status = STATUS_INVALID_PARAMETER;
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "DTR cannot be set when automatic DTR flow control is used - "
            "%!STATUS!",
            status);
    }

    if (NT_SUCCESS(status))
    {
        // Acquires the interrupt lock and sets the MCR.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        regModemControl = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);
        regModemControl |= SERIAL_MCR_DTR;
        WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlClrDtr
//
VOID
UartCtlClrDtr(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    UCHAR regModemControl;
    NTSTATUS status = STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    if ((pDevExt->HandFlow.ControlHandShake & SERIAL_DTR_MASK) ==
        SERIAL_DTR_HANDSHAKE)
    {
        status = STATUS_INVALID_PARAMETER;
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "DTR cannot be cleared when automatic DTR flow control is used - "
            "%!STATUS!",
            status);
    }

    if (NT_SUCCESS(status))
    {
        // Acquires the interrupt lock and sets the MCR.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        regModemControl = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);
        regModemControl &= ~SERIAL_MCR_DTR;
        WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlSetRts
//
VOID
UartCtlSetRts(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    UCHAR regModemControl;
    NTSTATUS status = STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    if (((pDevExt->HandFlow.FlowReplace & SERIAL_RTS_MASK) ==
        SERIAL_RTS_HANDSHAKE) ||
        ((pDevExt->HandFlow.FlowReplace & SERIAL_RTS_MASK) ==
        SERIAL_TRANSMIT_TOGGLE))
    {
        status = STATUS_INVALID_PARAMETER;
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "RTS cannot be set when automatic RTS flow control or "
            "transmit toggling is used - %!STATUS!",
            status);
    }

    if (NT_SUCCESS(status))
    {
        // Acquires the interrupt lock and sets the MCR.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        regModemControl = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);
        regModemControl |= SERIAL_MCR_RTS;
        WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlClrRts
//
VOID
UartCtlClrRts(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    UCHAR regModemControl;
    NTSTATUS status = STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    if (((pDevExt->HandFlow.FlowReplace & SERIAL_RTS_MASK) ==
        SERIAL_RTS_HANDSHAKE) ||
        ((pDevExt->HandFlow.FlowReplace & SERIAL_RTS_MASK) ==
        SERIAL_TRANSMIT_TOGGLE))
    {
        status = STATUS_INVALID_PARAMETER;
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "RTS cannot be cleared when automatic RTS flow control or "
            "transmit toggling is used - %!STATUS!",
            status);
    }

    if (NT_SUCCESS(status))
    {
        // Acquires the interrupt lock and sets the MCR.
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        regModemControl = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);
        regModemControl &= ~SERIAL_MCR_RTS;
        WRITE_MODEM_CONTROL(pDevExt, pDevExt->Controller, regModemControl);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlSetXoff
//
VOID
UartCtlSetXoff(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;

    UNREFERENCED_PARAMETER(Device);
    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    //
    // Software flow control and in-band signaling 
    // have not been implemented in this sample.
    //

    status = STATUS_NOT_IMPLEMENTED;

    TraceMessage(TRACE_LEVEL_WARNING,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlSetXon
//
VOID
UartCtlSetXon(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;

    UNREFERENCED_PARAMETER(Device);
    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    //
    // Software flow control and in-band signaling 
    // have not been implemented in this sample.
    //

    status = STATUS_NOT_IMPLEMENTED;

    TraceMessage(TRACE_LEVEL_WARNING,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlSetBreakOn
//
VOID
UartCtlSetBreakOn(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;

    UNREFERENCED_PARAMETER(Device);
    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

     //
    // Software flow control and in-band signaling 
    // have not been implemented in this sample.
    //

    status = STATUS_NOT_IMPLEMENTED;

    TraceMessage(TRACE_LEVEL_WARNING,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlSetBreakOff
//
VOID
UartCtlSetBreakOff(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;

    UNREFERENCED_PARAMETER(Device);
    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    //
    // Software flow control and in-band signaling 
    // have not been implemented in this sample.
    //

    status = STATUS_NOT_IMPLEMENTED;

    TraceMessage(TRACE_LEVEL_WARNING,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlGetHandflow
//
VOID
UartCtlGetHandflow(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status = STATUS_UNSUCCESSFUL;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PSERIAL_HANDFLOW pHandFlow = NULL;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

     status = WdfRequestRetrieveOutputBuffer(Request, 
                    sizeof(SERIAL_HANDFLOW), 
                    (PVOID*)(& pHandFlow), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve output buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        *pHandFlow = pDevExt->HandFlow;

        WdfRequestSetInformation(Request, sizeof(SERIAL_HANDFLOW));
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlSetHandflow
//
VOID
UartCtlSetHandflow(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status = STATUS_UNSUCCESSFUL;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PSERIAL_HANDFLOW pHandFlow = NULL;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveInputBuffer(Request, 
                    sizeof(SERIAL_HANDFLOW), 
                    (PVOID*)(& pHandFlow), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve input buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        //
        // Make sure there are no invalid bits set in
        // the control and handshake
        //

        if ((pHandFlow->ControlHandShake & SERIAL_CONTROL_INVALID) ||
            (pHandFlow->FlowReplace & SERIAL_FLOW_INVALID))
        {
            status = STATUS_INVALID_PARAMETER;
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_CONTROL,
                "Invalid bit in SERIAL_HANDFLOW %p - "
                "%!STATUS!",
                pHandFlow,
                status);
        }
    }

    if (NT_SUCCESS(status))
    {
        //
        // Software flow control and in-band signaling 
        // have not been implemented in this sample.
        //

        if ((pHandFlow->ControlHandShake & SERIAL_DSR_SENSITIVITY) ||
            (pHandFlow->ControlHandShake & SERIAL_ERROR_ABORT) ||
            (pHandFlow->ControlHandShake & SERIAL_DCD_HANDSHAKE) ||
            (pHandFlow->FlowReplace & SERIAL_AUTO_TRANSMIT) ||
            (pHandFlow->FlowReplace & SERIAL_AUTO_RECEIVE) ||
            (pHandFlow->FlowReplace & SERIAL_ERROR_CHAR) ||
            (pHandFlow->FlowReplace & SERIAL_NULL_STRIPPING) ||
            (pHandFlow->FlowReplace & SERIAL_BREAK_CHAR) ||
            (pHandFlow->FlowReplace & SERIAL_XOFF_CONTINUE) ||
            ((pHandFlow->FlowReplace & SERIAL_RTS_MASK) == 
                SERIAL_TRANSMIT_TOGGLE))
        {
            status = STATUS_NOT_IMPLEMENTED;
            TraceMessage(
                TRACE_LEVEL_WARNING,
                TRACE_FLAG_CONTROL,
                "Specified SERIAL_HANDFLOW %p has not been implemented - "
                "%!STATUS!",
                pHandFlow,
                status);
        }
    }

    if (NT_SUCCESS(status))
    {
        //
        // Make sure the DTR mode is valid
        //

        if ((pHandFlow->ControlHandShake & SERIAL_DTR_MASK) ==
            SERIAL_DTR_MASK)
        {
            status = STATUS_INVALID_PARAMETER;
            TraceMessage(
                TRACE_LEVEL_ERROR,
                TRACE_FLAG_CONTROL,
                "Cannot set handflow with invalid DTR mode %lu - "
                "%!STATUS!",
                pHandFlow->ControlHandShake,
                status);
        }
    }

    if (NT_SUCCESS(status))
    {
        WdfSpinLockAcquire(pDevExt->ReceiveBufferSpinLock);
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        BOOLEAN newFlowControl;
        BOOLEAN prevFlowControl = UsingRXFlowControl(pDevExt);

        pDevExt->HandFlow = *pHandFlow;

        newFlowControl = UsingRXFlowControl(pDevExt);
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        // Empty software FIFO before changing flow control
        if (newFlowControl != prevFlowControl)
        {
            if (!newFlowControl)
            {
                if (pDevExt->FIFOBufferBytes > 0)
                {
                    // If switching from flow control to no flow control,
                    // read bytes from the software FIFO to ring buffer before
                    // asserting flow control lines.

                    // Shouldn't have a cached buffer and bytes in the software FIFO
                    NT_ASSERT(!HasCachedReceiveBuffer(pDevExt));
                
                    // Using a new status variable so the IOCTL doesn't fail if
                    // the driver can't read the software FIFO to SerCx ring buffer, which
                    // may happen after the file has closed.
                    NTSTATUS fifoStatus = SerCxRetrieveReceiveBuffer(Device, SERIAL_SOFTWARE_FIFO_SIZE, &pDevExt->PIOReceiveBuffer);

                    // Read bytes from software FIFO and return the buffer
                    if (NT_SUCCESS(fifoStatus))
                    {
                        // Read the software FIFO bytes into the ring buffer.
                        // This function won't return the buffer.
                        UartReceiveBytesFromSoftwareFIFO(pDevExt);

                        // The software FIFO has been read out and should be empty now.
                        NT_ASSERT(pDevExt->FIFOBufferBytes == 0);

                        fifoStatus = SerCxProgressReceive(
                            Device, 
                            pDevExt->ReceiveProgress, 
                            SerCxStatusSuccess);

                        if (!NT_SUCCESS(fifoStatus))
                        {
                            TraceMessage(
                                TRACE_LEVEL_ERROR,
                                TRACE_FLAG_CONTROL,
                                "%!FUNC! Failed to return buffer - %!STATUS!",
                                fifoStatus);
                            NT_ASSERTMSG("SerCxProgressReceive shouldn't fail", NT_SUCCESS(fifoStatus));
                        }

                        pDevExt->PerfStats.ReceivedCount += pDevExt->ReceiveProgress;
                        pDevExt->ReceiveProgress = 0;
                        pDevExt->PIOReceiveBuffer.Buffer = NULL;
                    }
                    else
                    {
                        TraceMessage(
                            TRACE_LEVEL_WARNING,
                            TRACE_FLAG_CONTROL,
                            "SerCxRetrieveReceiveBuffer failed - %!STATUS!",
                            fifoStatus);
                    }
                }
            }
            else
            {
                // If switching from no flow control to flow control,
                // the software FIFO should already be empty.
                NT_ASSERT(pDevExt->FIFOBufferBytes == 0);
            }
        }

        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);

        // If software FIFO empty, re-assert flow control
        // Automatic flow control MUST be re-enabled here if it's being used.
        if (pDevExt->FIFOBufferBytes == 0)
        {
            UartFlowReceiveAvailable(Device);
        }        
        
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);
        WdfSpinLockRelease(pDevExt->ReceiveBufferSpinLock);
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlGetModemstatus
//
VOID
UartCtlGetModemstatus(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;
    UNREFERENCED_PARAMETER(Device);
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PULONG pBuffer;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

     status = WdfRequestRetrieveOutputBuffer(Request, 
                    sizeof(ULONG), 
                    (PVOID*)(& pBuffer), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve output buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        *pBuffer = pDevExt->ModemStatus;
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        WdfRequestSetInformation(Request, sizeof(ULONG));
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlGetDtrrts
//
VOID
UartCtlGetDtrrts(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PULONG pBuffer;
    UCHAR regModemControl;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveOutputBuffer(Request, 
                    sizeof(ULONG), 
                    (PVOID*)(& pBuffer), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve output buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        regModemControl = READ_MODEM_CONTROL(pDevExt, pDevExt->Controller);
        regModemControl &= SERIAL_DTR_STATE | SERIAL_RTS_STATE;

        *pBuffer = regModemControl;

        WdfRequestSetInformation(Request, sizeof(ULONG));
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlGetCommstatus
//
VOID
UartCtlGetCommstatus(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PSERIAL_STATUS pStat = NULL;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

     status = WdfRequestRetrieveOutputBuffer(Request, 
                    sizeof(SERIAL_STATUS), 
                    (PVOID*)(& pStat), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve output buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        WdfSpinLockAcquire(pDevExt->DpcSpinLock);

        pStat->Errors = pDevExt->ErrorWord;
        pDevExt->ErrorWord = 0;

        WdfSpinLockRelease(pDevExt->DpcSpinLock);

        //
        // Software flow control and in-band signaling 
        // have not been implemented in this samples. Parameters
        // such as HoldReasons, AmountInIn/OutQueue, and
        // WaitForImmediate have not been populated.
        //

        WdfRequestSetInformation(Request, sizeof(SERIAL_STATUS));
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlGetProperties
//
VOID
UartCtlGetProperties(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;
    PSERIAL_COMMPROP pProps = NULL;
    ULONG bufferSize;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveOutputBuffer(Request, 
                    sizeof(SERIAL_COMMPROP), 
                    (PVOID*)(& pProps), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve output buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        RtlZeroMemory(pProps, sizeof(SERIAL_COMMPROP));

        pProps->PacketLength = sizeof(SERIAL_COMMPROP);
        pProps->PacketVersion = 2;
        pProps->ServiceMask = SERIAL_SP_SERIALCOMM;
        pProps->MaxTxQueue = 0;
        pProps->MaxRxQueue = MAXLONG;

        pProps->MaxBaud = UartMaxBaudRate;
        pProps->SettableBaud = SERIAL_BAUD_USER;

        pProps->ProvSubType = SERIAL_SP_UNSPECIFIED;

        pProps->ProvCapabilities = 
            SERIAL_PCF_DTRDSR |
            SERIAL_PCF_RTSCTS |
            SERIAL_PCF_CD     |
            SERIAL_PCF_TOTALTIMEOUTS |
            SERIAL_PCF_INTTIMEOUTS;

        pProps->SettableParams = 
            SERIAL_SP_PARITY |
            SERIAL_SP_BAUD |
            SERIAL_SP_DATABITS |
            SERIAL_SP_STOPBITS |
            SERIAL_SP_HANDSHAKING |
            SERIAL_SP_PARITY_CHECK |
            SERIAL_SP_CARRIER_DETECT;


        pProps->SettableData = 
            SERIAL_DATABITS_5 |
            SERIAL_DATABITS_6 |
            SERIAL_DATABITS_7 |
            SERIAL_DATABITS_8;

        pProps->SettableStopParity =
            SERIAL_STOPBITS_10 |
            SERIAL_STOPBITS_15 |
            SERIAL_STOPBITS_20 |
            SERIAL_PARITY_NONE |
            SERIAL_PARITY_ODD  |
            SERIAL_PARITY_EVEN |
            SERIAL_PARITY_MARK |
            SERIAL_PARITY_SPACE;
        
        SerCxGetRingBufferUtilization(Device, NULL, &bufferSize);

        pProps->CurrentTxQueue = 0;
        pProps->CurrentRxQueue = bufferSize;

        WdfRequestSetInformation(Request, sizeof(SERIAL_COMMPROP));
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlXoffCounter
//
VOID
UartCtlXoffCounter(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;

    UNREFERENCED_PARAMETER(Device);
    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    //
    // Software flow control and in-band signaling 
    // have not been implemented in this sample.
    //

    status = STATUS_NOT_IMPLEMENTED;

    TraceMessage(TRACE_LEVEL_WARNING,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlLsrmstInsert
//
VOID
UartCtlLsrmstInsert(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;

    UNREFERENCED_PARAMETER(Device);
    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    //
    // Software flow control and in-band signaling 
    // have not been implemented in this sample.
    //

    status = STATUS_NOT_IMPLEMENTED;

    TraceMessage(TRACE_LEVEL_WARNING,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlGetStats
//
VOID
UartCtlGetStats(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);
    PSERIALPERF_STATS pStats = NULL;

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    status = WdfRequestRetrieveOutputBuffer(Request, 
                    sizeof(SERIALPERF_STATS), 
                    (PVOID*)(& pStats), 
                    NULL);

    if (!NT_SUCCESS(status))
    {
        TraceMessage(
            TRACE_LEVEL_ERROR,
            TRACE_FLAG_CONTROL,
            "Failed to retrieve output buffer for WDFREQUEST %p - "
            "%!STATUS!",
            Request,
            status);
    }

    if (NT_SUCCESS(status))
    {
        WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
        *pStats = pDevExt->PerfStats;
        WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

        WdfRequestSetInformation(Request, sizeof(SERIALPERF_STATS));
    }

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlClearStats
//
VOID
UartCtlClearStats(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    PUART_DEVICE_EXTENSION pDevExt = UartGetDeviceExtension(Device);

    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);
  
    WdfInterruptAcquireLock(pDevExt->WdfInterrupt);
    RtlZeroMemory(&pDevExt->PerfStats, sizeof(SERIALPERF_STATS));
    WdfInterruptReleaseLock(pDevExt->WdfInterrupt);

    TraceMessage(TRACE_LEVEL_INFORMATION,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}

//
// Internal Function: UartCtlImmediateChar
//
VOID
UartCtlImmediateChar(
    _In_ WDFDEVICE Device, 
    _In_ WDFREQUEST Request,
    _In_ size_t OutputBufferLength,
    _In_ size_t InputBufferLength
    )
{
    NTSTATUS status;

    UNREFERENCED_PARAMETER(Device);
    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(InputBufferLength);

    FuncEntry(TRACE_FLAG_CONTROL);

    //
    // Interleaving an immediate character into the write path
    // has not been implemented in this sample.
    //

    status = STATUS_NOT_IMPLEMENTED;

    TraceMessage(TRACE_LEVEL_WARNING,
                    TRACE_FLAG_CONTROL,
                    "WdfRequestComplete( %!HANDLE! => %!STATUS! )",
                    Request,
                    status);
    WdfRequestComplete(Request, status);

    FuncExit(TRACE_FLAG_CONTROL);
}