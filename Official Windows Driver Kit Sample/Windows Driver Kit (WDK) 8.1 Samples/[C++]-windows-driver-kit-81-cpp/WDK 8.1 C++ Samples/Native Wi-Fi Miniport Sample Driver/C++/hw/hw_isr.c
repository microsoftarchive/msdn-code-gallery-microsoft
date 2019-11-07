/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_isr.c

Abstract:
    Implements interrupt handling functions for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"
#include "hw_send.h"
#include "hw_recv.h"
#include "hw_isr.h"
#include "hw_mac.h"

#if DOT11_TRACE_ENABLED
#include "hw_isr.tmh"
#endif

/** This structure is passed to the function that sets the beacon IE */
typedef struct _HW_SET_BEACON_IE_PARAMS
{
    PHW_MAC_CONTEXT         HwMac;
    PVOID                   BeaconIEBlob;
    ULONG                   BeaconIEBlobSize;

} HW_SET_BEACON_IE_PARAMS, *PHW_SET_BEACON_IE_PARAMS;

NDIS_STATUS
HwRegisterInterrupt(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    )
{
    NDIS_STATUS                 ndisStatus;
    NDIS_MINIPORT_INTERRUPT_CHARACTERISTICS     InterruptChars;
    ULONG                       ndisVersion;

    NdisZeroMemory(&InterruptChars, sizeof(NDIS_MINIPORT_INTERRUPT_CHARACTERISTICS));

    InterruptChars.Header.Type = NDIS_OBJECT_TYPE_MINIPORT_INTERRUPT;
    InterruptChars.Header.Revision = NDIS_MINIPORT_INTERRUPT_REVISION_1;
    InterruptChars.Header.Size = sizeof(NDIS_MINIPORT_INTERRUPT_CHARACTERISTICS);

    InterruptChars.InterruptHandler = HWInterrupt;

    *ErrorCode = NDIS_STATUS_SUCCESS;
    *ErrorValue = 0;

    ndisVersion = NdisGetVersion();
    if (ndisVersion <= MP_NDIS_VERSION_NEEDS_COMPATIBILITY)
    {
        // Use the old interrupt DPC that does not support receive side throttling
        InterruptChars.InterruptDpcHandler = HWInterruptDPCNoRST;
    }
    else
    {
        // Use the new interrupt DPC routine that supports receive side throttling
        InterruptChars.InterruptDpcHandler = HWInterruptDPC;
    }
    
    InterruptChars.DisableInterruptHandler = HWDisableInterrupt;
    InterruptChars.EnableInterruptHandler = HWEnableInterrupt;

    ndisStatus = NdisMRegisterInterruptEx(
                    Hw->MiniportAdapterHandle,
                    Hw,
                    &InterruptChars,
                    &Hw->InterruptHandle
                    );
    if(ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to register interrupt with NDIS. Status = 0x%x\n", ndisStatus));
        *ErrorCode = NDIS_ERROR_CODE_INTERRUPT_CONNECT;
        *ErrorValue = ERRLOG_REGISTER_INTERRUPT_FAILED;
    }
    else
    {
        // Pass the interrupt handle to the HAL
        HalSetInterruptHandle(Hw->Hal, Hw->InterruptHandle);
    }

    return ndisStatus;
}

VOID
HwDeregisterInterrupt(
    _In_  PHW                     Hw
    )
{
    // Pass the interrupt handle to the HAL
    HalSetInterruptHandle(Hw->Hal, NULL);
    
    NdisMDeregisterInterruptEx(Hw->InterruptHandle);
}

__inline VOID
HwClearInterrupt(
    _In_  PHW                     Hw
    )
{
    HalClearInterrupt(Hw->Hal);
    
    // We clear the RDU bit (for next EnableInterrupt call)
    HalInterlockedAndIntrMask(Hw->Hal, (ULONG)(~(HAL_ISR_RX_DS_UNAVAILABLE)));
}

__inline BOOLEAN
HwInterruptEnabled(
    _In_  PHW                     Hw
    )
{
    return HalIsInterruptEnabled(Hw->Hal);
}

// The function that actually disable interrupts. This should not called from outside
// otherwise we wont be able to track the interrupts
_Function_class_(MINIPORT_SYNCHRONIZE_INTERRUPT)
__inline BOOLEAN
HwInternalDisableInterrupt(
    _In_  PHW                     Hw
    )
{
    NdisInterlockedIncrement(&Hw->InterruptDisableCount);
    HalDisableInterrupt(Hw->Hal);
    return TRUE;
}

// The functions that actually enables interrupts. This should not called from outside
// otherwise we wont be able to track the interrupts
_Function_class_(MINIPORT_SYNCHRONIZE_INTERRUPT)
__inline BOOLEAN
HwInternalEnableInterrupt(
    _In_  PHW                     Hw
    )
{
    // If multiple threads have disabled interrupts, enable them only when the
    // final one enables it
    if (NdisInterlockedDecrement(&Hw->InterruptDisableCount) == 0)
    {
        HalEnableInterrupt(Hw->Hal);
    }
    return TRUE;
}


// This is called from multiple places and we cannot just change this to call 
// NdisMSynchronizeWithInterruptEx
VOID
HwDisableInterrupt(
    _In_  PHW                     Hw,
    _In_  HW_ISR_TRACKING_REASON  Reason
    )
{
    UNREFERENCED_PARAMETER(Reason);
#if DBG
    NdisInterlockedIncrement(&Hw->Tracking_InterruptDisable[Reason]);
#endif

    HwInternalDisableInterrupt(Hw);
}

_Function_class_(MINIPORT_SYNCHRONIZE_INTERRUPT)
VOID
HwDisableInterruptWithSync(
    _In_  PHW                     Hw,
    _In_  HW_ISR_TRACKING_REASON  Reason
    )
{
    UNREFERENCED_PARAMETER(Reason);
#if DBG
    NdisInterlockedIncrement(&Hw->Tracking_InterruptDisable[Reason]);
#endif

    NdisMSynchronizeWithInterruptEx(
        Hw->InterruptHandle,
        0,
        (MINIPORT_SYNCHRONIZE_INTERRUPT_HANDLER)HwInternalDisableInterrupt,
        (PVOID)Hw
        );    
}


VOID
HwEnableInterrupt(
    _In_  PHW                     Hw,
    _In_  HW_ISR_TRACKING_REASON  Reason
    )
{
    UNREFERENCED_PARAMETER(Reason);
#if DBG
    NdisInterlockedDecrement(&Hw->Tracking_InterruptDisable[Reason]);
#endif

    HwInternalEnableInterrupt(Hw);
}

VOID
HwEnableInterruptWithSync(
    _In_  PHW                     Hw,
    _In_  HW_ISR_TRACKING_REASON  Reason
    )
{
    UNREFERENCED_PARAMETER(Reason);
#if DBG
    NdisInterlockedDecrement(&Hw->Tracking_InterruptDisable[Reason]);
#endif

    NdisMSynchronizeWithInterruptEx(
        Hw->InterruptHandle,
        0,
        (MINIPORT_SYNCHRONIZE_INTERRUPT_HANDLER)HwInternalEnableInterrupt,
        (PVOID)Hw
        );    
}

BOOLEAN
HwInterruptRecognized(
    _In_  PHW                     Hw,
    _Out_ PBOOLEAN                QueueMiniportHandleInterrupt
    )
{
    ULONG                       isr, deviceIsr;
    ULONG                       halIsrMask;

    *QueueMiniportHandleInterrupt = FALSE;
    
    //
    // Read from ISR value from the hardware
    //
    HalReadIsr(Hw->Hal, &isr, &deviceIsr);
    halIsrMask = HalGetIntrMask(Hw->Hal);

    //
    // If surprise removal has occured,set the flag
    //
    if (isr == HAL_ISR_SURPRISE_REMOVED)
    {
        HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_SURPRISE_REMOVED);
        *QueueMiniportHandleInterrupt = FALSE;
        return FALSE;
    }

    //
    // Time critical job handling
    //
    if (Hw->MacState.BSSStarted)
    {    
        HwProcessBeaconInterrupt(Hw, isr);
		
        if (isr & HAL_ISR_ATIM_END)
        {
            //
            // The ATIM window has passed away.
            //
            HwProcessATIMWindowInterrupt(Hw);
        }
    }

    //
    // HAL specific ISR processing
    // 
    HalProcessIsr(Hw->Hal, deviceIsr);

    if (isr & (HAL_ISR_RX_DS_UNAVAILABLE| HAL_ISR_RX_FIFO_OVERFLOW))
    {
        Hw->Stats.NumRxNoBuf++;
    }

    if (isr & halIsrMask)
    {
        if (isr == HAL_ISR_BEACON_INTERRUPT)
        {
            //
            // If this is only a beacon interrupt,no DPC is needed.
            // Beacons have already been handled at DIRQL
            //
            *QueueMiniportHandleInterrupt = FALSE;
        }
        else 
        {
            //
            // We need a DPC to handle this interrupt
            //
            *QueueMiniportHandleInterrupt = TRUE;
        }

        return TRUE;
    }
    else 
    {
        return FALSE;
    }
}


/**
 * Called by NDIS when an interrupt occurs that may belong this this miniport
 * 
 * \param MiniportInterruptContext          The Interrupt Context registered with NDIS
 * \param pbQueueMiniportHandleInterrupt    Set to true by this function if we want to
 * queue the Handle Interrupt DPC
 * \param TargetProcessors                  Ununsed
 * \return TRUE if the interrupt was generated by this adapter
 * \sa MPHandleInterrupt
 */
BOOLEAN 
HWInterrupt(
    NDIS_HANDLE             MiniportInterruptContext,
    PBOOLEAN                QueueMiniportHandleInterrupt,
    PULONG                  TargetProcessors
    )
{
    PHW                         hw = (PHW)MiniportInterruptContext;
    BOOLEAN                     interruptRecognized;

    *TargetProcessors = 0;

    do
    {        
        //
        // If the NIC is in low power state, this cannot be our interrupt
        //
        if (HW_TEST_ADAPTER_STATUS(hw, HW_ADAPTER_IN_LOW_POWER))
        {
            *QueueMiniportHandleInterrupt = FALSE;
            interruptRecognized = FALSE;
            break;
        }

        //
        // If the Interrupts are not already disabled and the HAL did generate an interrupt
        // claim this interrupt
        //
        if (HwInterruptEnabled(hw))
        {
            interruptRecognized = HwInterruptRecognized(hw, QueueMiniportHandleInterrupt);
            
            if (interruptRecognized)
            {
                //
                // This hardware has generated an interrupt
                //
                if (*QueueMiniportHandleInterrupt)
                {
                    // 
                    // The driver requires a DPC to handle this interrupt.
                    // Disable other interrupts so we can handle this one.
                    // This will be re-enabled in MpHandleInterrupt routine.
                    //
                    HwDisableInterrupt(hw, HW_ISR_TRACKING_INTERRUPT);
                }
                
                //
                // Clear this interrupt so that we do not wrongly keep claiming
                // this same interrupt. Also clear the 
                //
                HwClearInterrupt(hw);
            }
        }
        else
        {
            //
            // Interrupts are disabled so we are not the source of this interrupt
            //
            interruptRecognized = FALSE;
            *QueueMiniportHandleInterrupt = FALSE;
        }
    } while(FALSE);

    return interruptRecognized;
}



BOOLEAN
HwInterruptComplete(
    PHW                     Hw
    )
{
    BOOLEAN                 moreReceivesAvailable = FALSE;
    
    //
    // If receives are available, we would temporarily enable the
    // RDU interrupt to ensure that we get interrupt again. Keeping this
    // interrupt on all the time would cause problems with Reset/Pause 
    // scenarios where we cannot empty the receive descriptors, and
    // so may keep getting interrupted
    //
    if (HwIsReceiveAvailable(Hw, TRUE) && !HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_RECEIVE_FLAGS))
    {
        //
        // New receives are available, but we didnt indicate
        // them in this interrupt. Enable RDU in IntrMask. This would be 
        // reset by the ClearInterrupt routine
        //
        HalInterlockedOrIntrMask(Hw->Hal, HAL_ISR_RX_DS_UNAVAILABLE);
        moreReceivesAvailable = TRUE;
    }

#if 0 
    
    //
    // If we are supposed to turn off the RF due to power management, do
    // it now
    //
    if (pNic->ManagementInfo.PowerMgmtMode.dot11PowerMode == dot11_power_mode_powersave)
    {
        if (pNic->ManagementInfo.bSleepOnInterruptComplete)
        {
            // Sleep
            NdisGetCurrentSystemTime(&CurrentTime);
            CurrentTime.QuadPart /= 10;

            if (pNic->ManagementInfo.WakeupTime > (ULONGLONG)CurrentTime.QuadPart)
            {
                HwDoze(pNic, 
                    (ULONG)(pNic->ManagementInfo.WakeupTime - (ULONGLONG)CurrentTime.QuadPart),
                    FALSE
                    );
            }
            
            pNic->ManagementInfo.bSleepOnInterruptComplete = FALSE;
        }
    }
#endif

    return moreReceivesAvailable;
}


/**
 * Called by NDIS when the miniport claims an interrupt and requests this DPC to
 * be queued during a previous call to MPISR
 * 
 * \param MiniportInterruptContext    The HW context for this miniport
 * \sa MPISR
 */
VOID
HWInterruptDPCNoRST(
    _In_  NDIS_HANDLE             MiniportInterruptContext,
    _In_  PVOID                   MiniportDpcContext,
    _In_  PVOID                   ReceiveThrottleParameters,
    _In_  PVOID                   NdisReserved2
    )
{
    PHW                         hw = (PHW)MiniportInterruptContext;

    // This code runs on Vista
    UNREFERENCED_PARAMETER(ReceiveThrottleParameters);
    UNREFERENCED_PARAMETER(NdisReserved2);
    UNREFERENCED_PARAMETER(MiniportDpcContext);

    //
    // Ensure that the HW is awake before we do this
    // 
    HalPowerSaveStateWakeup(hw->Hal);

    //
    // The ordering of Send Complete and Receive handling is
    // significant. Depending on successful send completion of
    // certain packets like PS-POLL, we expect to receive
    // buffered frames from another machine.
    // For wireless medium, we care about successful sends having
    // occurred before we receive particular frames.
    //
    HwHandleSendCompleteInterrupt(hw);

    //
    // Perform receives
    //
    HwHandleReceiveInterrupt(hw, hw->RegInfo.MaxRxPacketsToProcess);

    //
    // More packets may be available for processing. However, due 
    // to lack of buffers, hardware may not raise any new receive interrupts.
    // We inform the hardware of this possibility so that if needed it 
    // can enable the appropriate interrupt
    //
    HwInterruptComplete(hw);

    
    //
    // Enable back the interrupts. These were disabled in HWInterrupt routine.
    //
    HwEnableInterruptWithSync(hw, HW_ISR_TRACKING_INTERRUPT);

    //
    // Restore the power save settings on the hardware
    //
    HalPowerSaveStateRestore(hw->Hal);
}


/**
 * Called by NDIS when the miniport claims an interrupt and requests this DPC to
 * be queued during a previous call to MPISR (or by NDIS as part of RST)
 * 
 * \param MiniportInterruptContext    The HW context for this miniport
 * \sa MPISR
 */
VOID
HWInterruptDPC(
    NDIS_HANDLE             MiniportInterruptContext,
    PVOID                   MiniportDpcContext,
    PVOID                   NdisReserved1,
    PVOID                   NdisReserved2
    )
{
    PHW                         hw = (PHW)MiniportInterruptContext;
    PNDIS_RECEIVE_THROTTLE_PARAMETERS   throttleParameters 
            = (PNDIS_RECEIVE_THROTTLE_PARAMETERS)NdisReserved1;
    ULONG                       maxNblsToIndicate = hw->RegInfo.MaxRxPacketsToProcess;
    BOOLEAN                     moreNblsPending;

    UNREFERENCED_PARAMETER(NdisReserved2);
    UNREFERENCED_PARAMETER(MiniportDpcContext);

    //
    // Ensure that the HW is awake before we do this
    // 
    HalPowerSaveStateWakeup(hw->Hal);

    //
    // The ordering of Send Complete and Receive handling is
    // significant. Depending on successful send completion of
    // certain packets like PS-POLL, we expect to receive
    // buffered frames from another machine.
    // For wireless medium, we care about successful sends having
    // occurred before we receive particular frames.
    //
    HwHandleSendCompleteInterrupt(hw);

    //
    // Perform receives using the OS provided limit for number of packets 
    // to indicate
    //
    if (throttleParameters->MaxNblsToIndicate < maxNblsToIndicate)
    {
        // OS has a lower limit on the number of packets we can indicate. Use
        // that instead of our value
        maxNblsToIndicate = throttleParameters->MaxNblsToIndicate;
    }
    HwHandleReceiveInterrupt(hw, maxNblsToIndicate);

    //
    // More packets may be available for processing. However, due 
    // to lack of buffers, hardware may not raise any new receive interrupts.
    // We inform the hardware of this possibility so that if needed it 
    // can enable the appropriate interrupt
    //
    moreNblsPending = HwInterruptComplete(hw);

    if (moreNblsPending && (throttleParameters->MaxNblsToIndicate != ULONG_MAX))
    {
        //
        // Receive side throttling is enabled in the OS. If we have receive packets
        // pending, let the OS requeue the DPC for us without us 
        // enabling the interrupt
        //
        throttleParameters->MoreNblsPending = TRUE;
    }
    else
    {
        //
        // Either RST is not enabled or we dont have packets pending in the H/W.
        // Enable back the interrupts. These were disabled in HWInterrupt routine.
        //
        throttleParameters->MoreNblsPending = FALSE;
        HwEnableInterruptWithSync(hw, HW_ISR_TRACKING_INTERRUPT);
    }

    //
    // Restore the power save settings on the hardware
    //
    HalPowerSaveStateRestore(hw->Hal);
}


/**
 * Called by NDIS to Enable Interrupts on this miniport
 * 
 * \param MiniportInterruptContext  The Interrupt context for this miniport
 * \sa MPDisableInterrupt
 */
VOID
HWEnableInterrupt(
    NDIS_HANDLE             MiniportInterruptContext
    )
{
    PHW                         Hw = (PHW)MiniportInterruptContext;

    HwEnableInterrupt(Hw, HW_ISR_TRACKING_NDIS);
}


/**
 * Called by NDIS to disable interrupts on this miniport
 * 
 * \param MiniportInterruptContext  The Interrupt context for this miniport
 * \sa MPEnableInterrupt
 */
VOID
HWDisableInterrupt(
    NDIS_HANDLE             MiniportInterruptContext
    )
{
    PHW                         Hw = (PHW)MiniportInterruptContext;

    HwDisableInterrupt(Hw, HW_ISR_TRACKING_NDIS);
}

VOID
HwProcessATIMWindowInterrupt(
    _In_  PHW                     Hw
    )
{
    UNREFERENCED_PARAMETER(Hw);
    
    // Power save in adhoc mode not supported
}

_Function_class_(MINIPORT_SYNCHRONIZE_INTERRUPT)
__inline BOOLEAN
HwInternalSetBeaconIE(
    _In_  PHW_SET_BEACON_IE_PARAMS BeaconParams
    )
{
    HwSetBeaconIE(BeaconParams->HwMac, BeaconParams->BeaconIEBlob, BeaconParams->BeaconIEBlobSize);
    return TRUE;
}


VOID
HwSetBeaconIEWithSync(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PVOID                   pBeaconIEBlob,
    _In_  ULONG                   uBeaconIEBlobSize
    )
{
    HW_SET_BEACON_IE_PARAMS       beaconParams;

    NdisZeroMemory(&beaconParams, sizeof(HW_SET_BEACON_IE_PARAMS));

    beaconParams.HwMac = HwMac;
    beaconParams.BeaconIEBlob = pBeaconIEBlob;
    beaconParams.BeaconIEBlobSize = uBeaconIEBlobSize;

    // Call the real function at IRQL dispatch
    NdisMSynchronizeWithInterruptEx(
        HwMac->Hw->InterruptHandle,
        0,
        (MINIPORT_SYNCHRONIZE_INTERRUPT_HANDLER)HwInternalSetBeaconIE,
        (PVOID)&beaconParams
        );    

}

// Called at device IRQL
VOID
HwProcessBeaconInterrupt(
    _In_  PHW                     Hw,
    _In_  ULONG                   Isr
    )
{
    HAL_TX_ITERATOR             descIter;
    HAL_TX_DESC_STATUS          descStatus;
    PHW_MAC_CONTEXT             macContext = Hw->MacState.BSSMac;

    do
    {
        // Since we may miss beacon done interrupt, we check this
        // without checking the ISR
        if (HalGetTxDescBusyNum(Hw->Hal, BEACON_QUEUE) == 0)
            break;
            
        HalGetNextToCheckTxDescIterator(Hw->Hal, BEACON_QUEUE, &descIter);
        HalGetTxDescStatus(Hw->Hal, descIter, &descStatus);            

        //MpTrace(COMP_TESTING, DBG_SERIOUS, ("BEACON: Checking Beacon 0x%08x", (ULONG)descIter));

        if (descStatus.SendPending)
        {
            break;
        }   

        //MpTrace(COMP_TESTING, DBG_SERIOUS, ("BEACON: Beacon Completed 0x%08x", (ULONG)descIter));
        
        if (descStatus.TransmitSuccess)
        {
            Hw->Stats.NumTxBeaconOk++;

            if (macContext->BssType == dot11_BSS_type_independent)
                HalEnableAdHocCoordinator(Hw->Hal, TRUE);
        }
        else
        {
            Hw->Stats.NumTxBeaconErr++;

            if (macContext->BssType == dot11_BSS_type_independent)
                HalEnableAdHocCoordinator(Hw->Hal, FALSE);
        }
        
        HalReleaseTxDesc(Hw->Hal, descIter, descIter, 1);            
    } while (FALSE);

    if (Isr & HAL_ISR_BEACON_INTERRUPT)
    {
        // This interrupt is raised whenever we are about to send a beacon.

        // Setup the next beacon to be transmitted
        HwSetupBeacon(Hw, macContext);
    }
}

