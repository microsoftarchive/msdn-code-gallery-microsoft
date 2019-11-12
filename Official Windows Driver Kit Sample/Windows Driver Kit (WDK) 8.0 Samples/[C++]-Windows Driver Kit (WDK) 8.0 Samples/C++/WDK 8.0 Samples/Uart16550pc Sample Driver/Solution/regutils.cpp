/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

_WdfVersionBuild_

Module Name: 

    regutils.cpp

Abstract:

    This module contains the 16550 UART controller's register access functions.
    This controller driver uses the Serial WDF class extension (SerCx).

Environment:

    kernel-mode only

Revision History:

    This code borrows heavily from code in the WDF Serial example.

--*/

// Include WDF.H First
#include <ntddk.h>
#include <wdf.h>
#include "ntddser.h"

// Class Extension includes
#include "SerCx.h"

#include "regutils.h"
#include "device.h"
#include "flow.h"

#include "tracing.h"
#include "regutils.tmh"

NTSTATUS
UartRegConvertAndValidateBaud (
    _In_ ULONG SpeedBPS,
    _Out_ USHORT * DivisorLatch
    )
{
    NTSTATUS status = STATUS_SUCCESS;
    USHORT divisor = 0;
    ULONG realBaud = 0;

    status = UartRegBaudToDivisorLatch(SpeedBPS, &divisor);

    if (NT_SUCCESS(status))
    {
        //
        // Determine if difference between desired and real
        // baud rate is within acceptable tolerance.
        //

        status = UartRegDivisorLatchToBaud(
            divisor,
            &realBaud);

        if (NT_SUCCESS(status))
        {
            ULONG baudDifference;
            
            if (SpeedBPS > realBaud)
            {
                baudDifference = SpeedBPS - realBaud;
            }
            else
            {
                baudDifference = realBaud - SpeedBPS;
            }

            KFLOATING_SAVE kFloatSave;
            status = KeSaveFloatingPointState(&kFloatSave);

            if (NT_SUCCESS(status))
            {
                if((DOUBLE)(baudDifference) / (DOUBLE)SpeedBPS >
                    UartBaudRateErrorTolerance)
                {
                    status = STATUS_INVALID_PARAMETER;
                    TraceMessage(
                        TRACE_LEVEL_ERROR,
                        TRACE_FLAG_CONTROL,
                        "Desired baudrate %lu exceeds error tolerance "
                        "(%.2f%%) - %!STATUS!",
                        SpeedBPS,
                        (UartBaudRateErrorTolerance*100),
                        status);
                }

                KeRestoreFloatingPointState(&kFloatSave);
            }
            else
            {
                TraceMessage(
                    TRACE_LEVEL_ERROR,
                    TRACE_FLAG_CONTROL,
                    "Failure saving floating point state - %!STATUS!",
                    status);
            }
        }
    }

    if (NT_SUCCESS(status))
    {
        *DivisorLatch = divisor;
    }
    
    return status;
}


NTSTATUS
UartRegDivisorLatchToBaud (
    _In_ USHORT DivisorLatchRegs,
    _Out_ ULONG * BaudRate
    )
{
    NTSTATUS status = STATUS_SUCCESS;

    if (DivisorLatchRegs == 0)
    {
        status = STATUS_INVALID_PARAMETER;
    }
    else
    {
        //
        // Baudrate =  SourceClock
        //            --------------
        //             16 * Divisor
        //
        // * add 1/2 denominator to numerator to effectively round 
        //   and prevent dropout
        //

        *BaudRate = (UartSourceClockFrequency + (8 * (ULONG)DivisorLatchRegs)) /
            (16 * (ULONG)DivisorLatchRegs);
    }
    
    return status;
}


NTSTATUS
UartRegBaudToDivisorLatch (
    _In_ ULONG SpeedBPS,
    _Out_ USHORT * DivisorLatch
    )
{
    NTSTATUS status = STATUS_SUCCESS;

    if ((SpeedBPS < UartMinBaudRate) ||
        (SpeedBPS > UartMaxBaudRate))
    {
        status = STATUS_INVALID_PARAMETER;
    }
    else
    {
        //
        // Divisor =   SourceClock
        //           ---------------
        //            16 * Baudrate
        //
        // * add 1/2 denominator to numerator to effectively round 
        //   and prevent dropout
        //

        *DivisorLatch = (USHORT)((UartSourceClockFrequency + (8 * SpeedBPS)) /
            (16 * SpeedBPS));
    }
    
    return status;
}


NTSTATUS
UartRegLCRToStruct (
    _In_ UCHAR LineControlRegister,
    _Out_ PSERIAL_LINE_CONTROL LineControlStruct
    )
{
    // Decodes the LCR and stores data in a SERIAL_LINE_CONTROL struct.

    NTSTATUS status = STATUS_SUCCESS;
    
    switch (LineControlRegister & SERIAL_DATA_MASK)
    {
    case SERIAL_5_DATA:
        LineControlStruct->WordLength = 5;
        break;
    case SERIAL_6_DATA:
        LineControlStruct->WordLength = 6;
        break;
    case SERIAL_7_DATA:
        LineControlStruct->WordLength = 7;
        break;
    case SERIAL_8_DATA:
        LineControlStruct->WordLength = 8;
        break;
    default:
        status = STATUS_INVALID_PARAMETER;
        goto exit;
    }

    switch (LineControlRegister & SERIAL_STOP_MASK)
    {
    case SERIAL_1_STOP:
        LineControlStruct->StopBits = STOP_BIT_1;
        break;
    case SERIAL_1_5_STOP:
        if (LineControlStruct->WordLength == 5)
        {
            LineControlStruct->StopBits = STOP_BITS_1_5;
        }
        else
        {
            LineControlStruct->StopBits = STOP_BITS_2;
        }
        break;
    default:
        status = STATUS_INVALID_PARAMETER;
        goto exit;
    }

    switch (LineControlRegister & SERIAL_PARITY_MASK)
    {
    case SERIAL_NONE_PARITY:
        LineControlStruct->Parity = NO_PARITY;
        break;
    case SERIAL_ODD_PARITY:
        LineControlStruct->Parity = ODD_PARITY;
        break;
    case SERIAL_EVEN_PARITY:
        LineControlStruct->Parity = EVEN_PARITY;
        break;
    case SERIAL_MARK_PARITY:
        LineControlStruct->Parity = MARK_PARITY;
        break;
    case SERIAL_SPACE_PARITY:
        LineControlStruct->Parity = SPACE_PARITY;
        break;
    default:
        status = STATUS_INVALID_PARAMETER;
        goto exit;
    }

exit:

    return status;
}


NTSTATUS
UartRegStructToLCR (
    _In_ PSERIAL_LINE_CONTROL LineControlStruct,
    _Out_ UCHAR * LineControlRegister
    )
{
    // Decodes the SERIAL_LINE_CONTROL struct and stores data in LCR fields.
    UCHAR lcr = 0;
    NTSTATUS status = STATUS_SUCCESS;

    switch (LineControlStruct->WordLength)
    {
    case 5:
        lcr = lcr | SERIAL_5_DATA;
        break;
    case 6:
        lcr = lcr | SERIAL_6_DATA;
        break;
    case 7:
        lcr = lcr | SERIAL_7_DATA;
        break;
    case 8:
        lcr = lcr | SERIAL_8_DATA;
        break;
    default:
        status = STATUS_INVALID_PARAMETER;
        goto exit;
    }

    switch (LineControlStruct->StopBits)
    {
    case STOP_BIT_1:
        lcr = lcr | SERIAL_1_STOP;
        break;
    case STOP_BITS_1_5:
        if (LineControlStruct->WordLength != 5)
        {
            status = STATUS_INVALID_PARAMETER;
            goto exit;
        }
        lcr = lcr | SERIAL_1_5_STOP;
        break;
    case STOP_BITS_2:
        if (LineControlStruct->WordLength == 5)
        {
            status = STATUS_INVALID_PARAMETER;
            goto exit;
        }
        lcr = lcr | SERIAL_2_STOP;
        break;
    default:
        status = STATUS_INVALID_PARAMETER;
        goto exit;
    }

    switch (LineControlStruct->Parity)
    {
    case NO_PARITY:
        lcr = lcr | SERIAL_NONE_PARITY;
        break;
    case ODD_PARITY:
        lcr = lcr | SERIAL_ODD_PARITY;
        break;
    case EVEN_PARITY:
        lcr = lcr | SERIAL_EVEN_PARITY;
        break;
    case MARK_PARITY:
        lcr = lcr | SERIAL_MARK_PARITY;
        break;
    case SPACE_PARITY:
        lcr = lcr | SERIAL_SPACE_PARITY;
        break;
    default:
        status = STATUS_INVALID_PARAMETER;
        goto exit;
    }

    *LineControlRegister = lcr;

exit:

    return status;
}

BOOLEAN
UartRecordInterrupt(
    _In_ WDFDEVICE Device
    )
{
    BOOLEAN servicedAnInterrupt = FALSE;
    UCHAR regInterruptId = 0;
    UCHAR regLineStatus = 0;
    UCHAR regModemStatus = 0;
    UCHAR oldErrorBits = 0;
    PUART_DEVICE_EXTENSION pDevExt;

    FuncEntry(TRACE_FLAG_REGUTIL);

    pDevExt = UartGetDeviceExtension(Device);

    regInterruptId = READ_INTERRUPT_ID_REG(pDevExt, pDevExt->Controller);

    // If an interrupt actually happened, record the registers...
    // While the NO_INTERRUPT_PENDING bit is NOT set,
    while ((regInterruptId & SERIAL_IIR_NO_INTERRUPT_PENDING) == 0)
    {
        BYTE ier = 0x00;
        servicedAnInterrupt = TRUE;
        
        pDevExt->InterruptIdentifier = regInterruptId;

        // TODO: Fix
        //       IIR indicates a single interrupt ordered by priority.
        //       This switch statement ensures that interrupts are not
        //       confused with one another.
        switch (regInterruptId & SERIAL_IIR_MASK)
        {
        case SERIAL_IIR_RLS:
            regLineStatus = READ_LINE_STATUS(pDevExt, pDevExt->Controller);
            oldErrorBits = pDevExt->LineStatus & SERIAL_LSR_ERROR;
            pDevExt->LineStatus = regLineStatus | oldErrorBits;
            break;

        case SERIAL_IIR_RDA:
        case SERIAL_IIR_CTI:
            // Issue flow control
            UartFlowReceiveFull(Device);

            // Clear the RDA interrupt for now. We will
            // read the data in the DPC.
            ier = READ_INTERRUPT_ENABLE(pDevExt, pDevExt->Controller);
            ier &= (~SERIAL_IER_RDA);
            WRITE_INTERRUPT_ENABLE(pDevExt, pDevExt->Controller, ier);
            break;

        case SERIAL_IIR_THR:
            pDevExt->HoldingEmpty = TRUE;

            // Clear the THR interrupt for now
            ier = READ_INTERRUPT_ENABLE(pDevExt, pDevExt->Controller);
            ier &= ~(SERIAL_IER_THR);
            WRITE_INTERRUPT_ENABLE(pDevExt, pDevExt->Controller, ier);
            break;

        case SERIAL_IIR_MS:
            regModemStatus = READ_MODEM_STATUS(pDevExt, pDevExt->Controller);
            pDevExt->ModemStatus = (pDevExt->ModemStatus & SERIAL_MSR_EVENTS) |
                regModemStatus;
            break;

        default:
            break;
        }

        regInterruptId = READ_INTERRUPT_ID_REG(pDevExt, pDevExt->Controller);
    }

    FuncExit(TRACE_FLAG_REGUTIL);

    return servicedAnInterrupt;
}

VOID
UartEvaluateLineStatus(
    _In_ WDFDEVICE Device,
    _Out_ PBOOLEAN CanReceive,
    _Out_ PBOOLEAN CanTransmit,
    _Out_ PBOOLEAN HasErrors
    )
{
    UCHAR regLineStatus = 0;
    UCHAR oldErrorBits = 0;
    PUART_DEVICE_EXTENSION pDevExt = NULL;

    FuncEntry(TRACE_FLAG_REGUTIL);

    pDevExt = UartGetDeviceExtension(Device);

    regLineStatus = READ_LINE_STATUS(pDevExt, pDevExt->Controller);

    if (pDevExt->LineStatus != regLineStatus)
    {
        // LSR changed.
        // If there is an error indicated, binary OR it with the previous error.
        oldErrorBits = pDevExt->LineStatus & SERIAL_LSR_ERROR;
        pDevExt->LineStatus = regLineStatus | oldErrorBits;
    }

    pDevExt->HoldingEmpty = ((pDevExt->LineStatus & SERIAL_LSR_THRE) != 0);
    
    *CanReceive = ((pDevExt->LineStatus & SERIAL_LSR_DR) != 0);
    *CanTransmit = pDevExt->HoldingEmpty && UartFlowCanTransmit(Device);
    *HasErrors = ((pDevExt->LineStatus & SERIAL_LSR_ERROR) != 0);

    FuncExit(TRACE_FLAG_REGUTIL);
}
    

USHORT UartWaitableEvents(
    _In_ WDFDEVICE Device
    )
{
    PUART_DEVICE_EXTENSION pDevExt = NULL;
    USHORT someEvent = 0;

    pDevExt = UartGetDeviceExtension(Device);    

    FuncEntry(TRACE_FLAG_REGUTIL);

    // Note that each if statement here is consistent in that it compares
    // LineStatus to SERIAL_LSR_* and compares ModemStatus to SERIAL_MSR_*.

    if ((pDevExt->LineStatus & SERIAL_LSR_DR) != 0)
        someEvent = someEvent | SERIAL_EV_RXCHAR;

    // SERIAL_EV_RXFLAG is not handled in this function.

    if ((pDevExt->LineStatus & SERIAL_LSR_TEMT) != 0)
        someEvent = someEvent | SERIAL_EV_TXEMPTY;

    if ((pDevExt->LineStatus & SERIAL_LSR_BI) != 0)
        someEvent = someEvent | SERIAL_EV_BREAK;

    if ((pDevExt->LineStatus & SERIAL_LSR_ERROR) != 0)
        someEvent = someEvent | SERIAL_EV_ERR;

    if ((pDevExt->ModemStatus & SERIAL_MSR_DCTS) != 0)
        someEvent = someEvent | SERIAL_EV_CTS;

    if ((pDevExt->ModemStatus & SERIAL_MSR_DDSR) != 0)
        someEvent = someEvent | SERIAL_EV_DSR;

    if ((pDevExt->ModemStatus & SERIAL_MSR_TERI) != 0)
        someEvent = someEvent | SERIAL_EV_RLSD;

    if ((pDevExt->ModemStatus & SERIAL_MSR_RI) != 0)
        someEvent = someEvent | SERIAL_EV_RING;

    // SERIAL_EV_PERR is not possible.
    // SERIAL_EV_RX80FULL is not handled in this function.
    // SERIAL_EV_EVENT1 is not possible.
    // SERIAL_EV_EVENT2 is not possible.

    // Clear the modem status events
    pDevExt->ModemStatus &= ~(SERIAL_MSR_EVENTS);

    FuncExit(TRACE_FLAG_REGUTIL);

    return someEvent;
}
