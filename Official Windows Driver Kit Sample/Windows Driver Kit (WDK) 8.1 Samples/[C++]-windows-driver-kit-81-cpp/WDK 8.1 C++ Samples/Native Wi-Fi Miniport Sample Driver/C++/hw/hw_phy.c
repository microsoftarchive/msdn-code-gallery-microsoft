/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_phy.c

Abstract:
    Implements the phy functionality for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"
#include "hw_phy.h"
#include "hw_isr.h"
#include "hw_main.h"
#include "hw_mac.h"

#if DOT11_TRACE_ENABLED
#include "hw_phy.tmh"
#endif

BOOLEAN
HwQueryShortSlotTimeOptionImplemented(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    PNICPHYMIB                  phyMib;

    phyMib = HalGetPhyMIB(Hw->Hal, PhyId);

    //
    // Depending on phy type, we report support
    //
    if (phyMib->PhyType == dot11_phy_type_erp)
        return TRUE;
    else 
        return FALSE;
}


BOOLEAN
HwQueryDsssOfdmOptionImplemented(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    UNREFERENCED_PARAMETER(Hw);
    UNREFERENCED_PARAMETER(PhyId);

    //
    // Always returning false
    //
    return FALSE;
}


BOOLEAN
HwQueryShortPreambleOptionImplemented(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    PNICPHYMIB                  phyMib;

    phyMib = HalGetPhyMIB(Hw->Hal, PhyId);

    //
    // Depending on phy type, we report support
    //
    if (phyMib->PhyType == dot11_phy_type_erp)
        return TRUE;
    else 
        return FALSE;
}


BOOLEAN
HwQueryPbccOptionImplemented(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    UNREFERENCED_PARAMETER(Hw);
    UNREFERENCED_PARAMETER(PhyId);

    //
    // Not implemented, so always returning false
    //
    return FALSE;
}


BOOLEAN
HwQueryChannelAgilityPresent(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    UNREFERENCED_PARAMETER(Hw);
    UNREFERENCED_PARAMETER(PhyId);

    //
    // Always return false
    //
    return FALSE;
}


BOOLEAN
HwQueryNicPowerState(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    UNREFERENCED_PARAMETER(PhyId);

    //
    // We dont do per-PHY radio control. So return global state
    //
    return !Hw->PhyState.SoftwareRadioOff;

}


BOOLEAN
HwQueryHardwarePhyState(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    UNREFERENCED_PARAMETER(Hw);
    UNREFERENCED_PARAMETER(PhyId);
    
    // NOTE: Always TRUE since we dont support hardware setting to turn off the radio
    return TRUE;
}

BOOLEAN
HwQuerySoftwarePhyState(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    UNREFERENCED_PARAMETER(PhyId);

    return !Hw->PhyState.SoftwareRadioOff;
}



NDIS_STATUS
HwQueryDiversitySelectionRX(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_DIVERSITY_SELECTION_RX_LIST Dot11DiversitySelectionRXList
    )
{
    USHORT    Index;

    UNREFERENCED_PARAMETER(PhyId);
    
    if( MaxEntries < Hw->PhyState.DiversitySelectionRxList->uNumOfEntries )
    {
        Dot11DiversitySelectionRXList->uTotalNumOfEntries 
            = Hw->PhyState.DiversitySelectionRxList->uNumOfEntries;
        Dot11DiversitySelectionRXList->uNumOfEntries = MaxEntries;
        for( Index=0; Index<MaxEntries; Index++ )
        {
            Dot11DiversitySelectionRXList->dot11DiversitySelectionRx[Index].uAntennaListIndex 
                = Hw->PhyState.DiversitySelectionRxList->dot11DiversitySelectionRx[Index].uAntennaListIndex;
            Dot11DiversitySelectionRXList->dot11DiversitySelectionRx[Index].bDiversitySelectionRX 
                = Hw->PhyState.DiversitySelectionRxList->dot11DiversitySelectionRx[Index].bDiversitySelectionRX;
        }
        return NDIS_STATUS_BUFFER_OVERFLOW;
    }
    else
    {
        Dot11DiversitySelectionRXList->uNumOfEntries = Hw->PhyState.DiversitySelectionRxList->uNumOfEntries;
        Dot11DiversitySelectionRXList->uNumOfEntries = MaxEntries;
        for( Index=0; Index<Hw->PhyState.DiversitySelectionRxList->uNumOfEntries; Index++ )
        {
            Dot11DiversitySelectionRXList->dot11DiversitySelectionRx[Index].uAntennaListIndex 
                = Hw->PhyState.DiversitySelectionRxList->dot11DiversitySelectionRx[Index].uAntennaListIndex;
            Dot11DiversitySelectionRXList->dot11DiversitySelectionRx[Index].bDiversitySelectionRX 
                = Hw->PhyState.DiversitySelectionRxList->dot11DiversitySelectionRx[Index].bDiversitySelectionRX;
        }
    
        return NDIS_STATUS_SUCCESS;                        
    }
}


NDIS_STATUS
HwQueryRegDomainsSupportValue(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  ULONG                   MaxEntries,
    _Out_ PDOT11_REG_DOMAINS_SUPPORT_VALUE    Dot11RegDomainsSupportValue
    )
{
    USHORT    Index;

    UNREFERENCED_PARAMETER(PhyId);

    if( MaxEntries < Hw->PhyState.RegDomainsSupportValue->uNumOfEntries )
    {
        Dot11RegDomainsSupportValue->uTotalNumOfEntries 
            = Hw->PhyState.RegDomainsSupportValue->uNumOfEntries;
        Dot11RegDomainsSupportValue->uNumOfEntries = MaxEntries;
        for( Index=0; Index<MaxEntries; Index++ )
        {
            Dot11RegDomainsSupportValue->dot11RegDomainValue[Index].uRegDomainsSupportIndex 
                = Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[Index].uRegDomainsSupportIndex;
            Dot11RegDomainsSupportValue->dot11RegDomainValue[Index].uRegDomainsSupportValue 
                = Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[Index].uRegDomainsSupportValue;
        }
        return NDIS_STATUS_BUFFER_OVERFLOW;
    }
    else
    {
        Dot11RegDomainsSupportValue->uNumOfEntries 
            = Hw->PhyState.RegDomainsSupportValue->uNumOfEntries;
        Dot11RegDomainsSupportValue->uNumOfEntries = MaxEntries;
        for( Index=0; Index<Hw->PhyState.DiversitySelectionRxList->uNumOfEntries; Index++ )
        {
            Dot11RegDomainsSupportValue->dot11RegDomainValue[Index].uRegDomainsSupportIndex 
                = Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[Index].uRegDomainsSupportIndex;
            Dot11RegDomainsSupportValue->dot11RegDomainValue[Index].uRegDomainsSupportValue 
                = Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[Index].uRegDomainsSupportValue;
        }
    
        return NDIS_STATUS_SUCCESS;                        
    }
}


LONG
HwQueryMinRSSI(
    _In_  PHW                     Hw,
    _In_  ULONG                   DataRate
    )
{
    UNREFERENCED_PARAMETER(Hw);
    UNREFERENCED_PARAMETER(DataRate);

    // NOTE: RSSI typically does change with Data rate
    return -95;
}


LONG
HwQueryMaxRSSI(
    _In_  PHW                     Hw,
    _In_  ULONG                   DataRate
    )
{
    UNREFERENCED_PARAMETER(Hw);
    UNREFERENCED_PARAMETER(DataRate);

    // NOTE: RSSI typically does change with Data rate
    return -45;
}


NDIS_STATUS
HwQuerySupportedDataRatesValue(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _Out_ PDOT11_SUPPORTED_DATA_RATES_VALUE_V2    Dot11SupportedDataRatesValue
    )
{
    PNICPHYMIB                  phyMib;
    ULONG                       index;

    phyMib = HalGetPhyMIB(Hw->Hal, PhyId);
    for (index = 0; index < MAX_NUM_SUPPORTED_RATES_V2; index++)
    {
        Dot11SupportedDataRatesValue->ucSupportedTxDataRatesValue[index] 
        = phyMib->SupportedDataRatesValue.ucSupportedTxDataRatesValue[index];
        Dot11SupportedDataRatesValue->ucSupportedRxDataRatesValue[index] 
        = phyMib->SupportedDataRatesValue.ucSupportedRxDataRatesValue[index];
    }

    return NDIS_STATUS_SUCCESS;
}


ULONG
HwQueryCCAModeSupported(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    PNICPHYMIB                  phyMib;

    phyMib = HalGetPhyMIB(Hw->Hal, PhyId);

    if (phyMib->PhyType == dot11_phy_type_dsss ||
        phyMib->PhyType == dot11_phy_type_hrdsss ||
        phyMib->PhyType == dot11_phy_type_erp)
    {
        return DOT11_CCA_MODE_CS_ONLY;
    }
    else
    {
        return 0;
    }
}


ULONG
HwQueryCurrentTXPowerLevel(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    return HalGetTXPowerLevel(Hw->Hal, PhyId);
}


DOT11_DIVERSITY_SUPPORT
HwQueryDiversitySupport(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    UNREFERENCED_PARAMETER(PhyId);
    
    // NOTE: This can be per phy
    return Hw->PhyState.DiversitySupport;
}


ULONG
HwQueryEDThreshold(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    PNICPHYMIB                  phyMib;
    
    phyMib = HalGetPhyMIB(Hw->Hal, PhyId);

    if (phyMib->PhyType == dot11_phy_type_dsss ||
        phyMib->PhyType == dot11_phy_type_hrdsss ||
        phyMib->PhyType == dot11_phy_type_erp)
    {
        // NOTE: Hardcoded value is being used here
        return (ULONG)-65;
    }
    else
    {
        return 0;
    }
}



ULONG
HwQueryFrequencyBandsSupported(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    PNICPHYMIB                  phyMib;
    
    phyMib = HalGetPhyMIB(Hw->Hal, PhyId);

    if (phyMib->PhyType == dot11_phy_type_ofdm)
    {
        return DOT11_FREQUENCY_BANDS_LOWER | DOT11_FREQUENCY_BANDS_MIDDLE;
    }
    else
    {
        return 0;
    }
}


BOOLEAN
HwQueryMultiDomainCapabilityImplemented(
    _In_  PHW                     Hw
    )
{
    UNREFERENCED_PARAMETER(Hw);
    return FALSE;
}


DOT11_TEMP_TYPE
HwQueryTempType(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    UNREFERENCED_PARAMETER(Hw);
    UNREFERENCED_PARAMETER(PhyId);

    //
    // Our hardware can work in the range of 0 to 70 degrees C. Since there is no such range in 
    // DOT11_TEMP_TYPE, we return dot11_temp_type_1 (0C - 40C).
    //
    return dot11_temp_type_1;
}


NDIS_STATUS
HwPersistRadioPowerState(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 RadioOff
    )
{
    NDIS_CONFIGURATION_OBJECT       ConfigObject;
    NDIS_HANDLE                     RegistryConfigurationHandle = NULL;
    NDIS_CONFIGURATION_PARAMETER    Parameter;
    NDIS_STRING                     RegName = NDIS_STRING_CONST("RadioOff");
    NDIS_STATUS                     ndisStatus;

    ConfigObject.Header.Type = NDIS_OBJECT_TYPE_CONFIGURATION_OBJECT;
    ConfigObject.Header.Revision = NDIS_CONFIGURATION_OBJECT_REVISION_1;
    ConfigObject.Header.Size = sizeof( NDIS_CONFIGURATION_OBJECT );
    ConfigObject.NdisHandle = Hw->MiniportAdapterHandle;
    ConfigObject.Flags = 0;

    ndisStatus = NdisOpenConfigurationEx(
                    &ConfigObject,
                    &RegistryConfigurationHandle
                    );

    if ((ndisStatus == NDIS_STATUS_SUCCESS) && (RegistryConfigurationHandle != NULL))
    {
        NdisZeroMemory(&Parameter, sizeof(NDIS_CONFIGURATION_PARAMETER));

        Parameter.ParameterData.IntegerData = (RadioOff ? 1 : 0);
        Parameter.ParameterType = NdisParameterInteger;
        
        NdisWriteConfiguration(&ndisStatus,
            RegistryConfigurationHandle,
            &RegName,
            &Parameter
            );
    }
    
    //
    // Close the handle to the registry
    //
    if (RegistryConfigurationHandle)
    {
        NdisCloseConfiguration(RegistryConfigurationHandle);
    }

    return ndisStatus;
}
NDIS_STATUS
HwSetNicPowerState(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  BOOLEAN                 PowerOn
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    HW_HAL_RESET_PARAMETERS     resetParams;

    //
    // Our RF cannot be selectively turned on/off. We turn on or turn off all the phys
    // anytime this OID is set
    //
    UNREFERENCED_PARAMETER(PhyId);
    //
    // Check if we are already in the specified state.
    //
    if (Hw->PhyState.SoftwareRadioOff == !PowerOn)
    {
        // No need to take status indications if we are
        // already in the correct state
        return NDIS_STATUS_SUCCESS;
    }

    if (!PowerOn)
    {
        //
        // Going to power save. Get everything into a stable state
        //
        
        //
        // If a scan is in progress, cancel scan
        //
        if (Hw->ScanContext.ScanInProgress)
        {
            HwCancelScan(Hw);
        }

        //
        // Before we turn off the radio, we reset the h/w. 
        // This is to ensure that there isnt any pending operation sitting 
        // on the hardware
        //
        NdisZeroMemory(&resetParams, sizeof(HW_HAL_RESET_PARAMETERS));
        HwResetHAL(Hw, &resetParams, FALSE);

        //
        // Disable the interrupt
        //
        HwDisableInterruptWithSync(Hw, HW_ISR_TRACKING_RADIO_STATE);

        HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
        HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_RADIO_OFF);
        HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);
        
    }
    
    //
    // Update Radio state
    //

    // Set flag so other threads (power save, scan timer, etc) wont change
    // state behind us
    Hw->PhyState.RadioStateChangeInProgress = TRUE;

    // We wait for ever for other threads to not reach this state
    while (Hw->PhyState.RadioAccessRef != 0);

    //
    // Set the Radio state
    //
    if (PowerOn)
    {
        Hw->PhyState.Debug_SoftwareRadioOff = FALSE;
        HalSetRFPowerState(Hw->Hal, RF_ON);
        Hw->PhyState.SoftwareRadioOff = FALSE;        
    }
    else
    {
        Hw->PhyState.SoftwareRadioOff = TRUE;
        HalSetRFPowerState(Hw->Hal, RF_SHUT_DOWN);
        Hw->PhyState.Debug_SoftwareRadioOff = TRUE;
    }

    Hw->PhyState.RadioStateChangeInProgress = FALSE;

    if (PowerOn)
    {
        // Clear the radio off bit
        HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
        HW_CLEAR_ADAPTER_STATUS(Hw, HW_ADAPTER_RADIO_OFF);
        HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);
    
        // Re-reset the H/W to get it into a good state
        NdisZeroMemory(&resetParams, sizeof(HW_HAL_RESET_PARAMETERS));
        resetParams.FullReset = TRUE;
        HwResetHAL(Hw, &resetParams, FALSE);

        //
        // Reenable the interrupt
        //
        HwEnableInterruptWithSync(Hw, HW_ISR_TRACKING_RADIO_STATE);
    }

    //
    // Save the new radio state in the registry
    //
    ndisStatus = HwPersistRadioPowerState(Hw, Hw->PhyState.SoftwareRadioOff);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_OID, DBG_SERIOUS, ("Unable to persist new radio state in the registry\n"));
        return ndisStatus;
    }

    //
    // Report the new power state to the OS
    //    
    HwIndicatePhyPowerState(
        Hw,
        DOT11_PHY_ID_ANY
        );
    return ndisStatus;

}


// Called at PASSIVE, DISPATCH & DIRQL
// Returns TRUE if at the end of the call, the new state of the RF is the requested
// state, returns FALSE otherwise
BOOLEAN
HwSetRFState(
    _In_  PHW                     Hw,
    _In_  UCHAR	                NewRFState
    )
{
    BOOLEAN                     result = TRUE;
    
    do
    {
        if (InterlockedIncrement(&Hw->PhyState.RadioAccessRef) == 1)
        {
            if (Hw->PhyState.RadioStateChangeInProgress)
            {
                // Radio state is being changed, dont proceed
                result = FALSE;
                break;
            }
            
            if ((NewRFState == RF_ON) || (NewRFState == RF_SLEEP))
            {
                if (Hw->PhyState.SoftwareRadioOff == TRUE)
                {
                    // Radio is OFF, dont change RF state
                    result = FALSE;
                    break;
                }
            }
            
            HalSetRFPowerState(Hw->Hal, NewRFState);
        }
        else
        {
            // Another thread is changing RF/Radio state,
            // dont modify. We bail out and the caller can decide if it wants to
            // reattempt (we dont wait since we may be called at high IRQL, etc)
            MpTrace(COMP_POWER, DBG_LOUD, ("Unable to set RF/Radio state as something else is updating it\n"));
            result = FALSE;
        }
    }while (FALSE);

    InterlockedDecrement(&Hw->PhyState.RadioAccessRef);
    
    return result;
}

// Called at PASSIVE or DISPATCH
BOOLEAN
HwSetRFOn(
    _In_  PHW                     Hw,
	_In_ UCHAR                    MaxRetries
	)
{
    UCHAR   RetryCount = 0;
	
    //
    // This is at dispatch, dont spin for long
    //
    while (RetryCount < MaxRetries)
    {
        if (HwSetRFState(Hw, RF_ON) == FALSE)
        {
            // If radio is OFF, hw awake can fail. It can also fail
            // if something else is changing radio state, in which case, we try
            // try again
            if (Hw->PhyState.SoftwareRadioOff == TRUE)
            {
                // Failed because radio was OFF
                break;
            }
            else
            {
                //
                // Something else is toggling RF state
                //
                NdisStallExecution(10); // Stall for 10 microseconds
            }
        }
        else
        {
            // Hw awake OK
            return TRUE;
        }
        RetryCount++;
    }

    return FALSE;
}

BOOLEAN
HwAwake(
    _In_  PHW                     Hw,
	_In_  BOOLEAN                 DeviceIRQL
	)
{
    BOOLEAN                     Canceled;
    
    if (HalGetRFPowerState(Hw->Hal) != RF_ON) 
    {
        MpTrace(COMP_POWER, DBG_LOUD, (" *** RF ON\n"));

        if (!DeviceIRQL)
        {
            //
            // Try to cancel timer in case we are not called by timer this time
            //
            Canceled = NdisCancelTimerObject(Hw->PhyState.Timer_Awake);
            if (Canceled)
            {
                MpTrace(COMP_POWER, DBG_LOUD, ("Power ON timer cancelled\n"));
            }

            // Enable RF and retry 3 times if something else is using it
            return HwSetRFOn(Hw, 3);
        }
        else
        {
            return HwSetRFState(Hw, RF_ON);
        }
    }
    
    return TRUE;


}

VOID
HwAwakeTimer(
    PVOID                   SystemSpecific1,
    PVOID                   FunctionContext,
    PVOID                   SystemSpecific2,
    PVOID                   SystemSpecific3
    )
{
    PHW                         hw = (PHW)FunctionContext;

    UNREFERENCED_PARAMETER(SystemSpecific1);
    UNREFERENCED_PARAMETER(SystemSpecific2);
    UNREFERENCED_PARAMETER(SystemSpecific3);

    // If radio is OFF, the callback wont turn on the radio
    HwAwake(hw, FALSE);    
}

VOID
HwDozeTimer(
    PVOID                   SystemSpecific1,
    PVOID                   FunctionContext,
    PVOID                   SystemSpecific2,
    PVOID                   SystemSpecific3
    )
{
    UNREFERENCED_PARAMETER(SystemSpecific1);
    UNREFERENCED_PARAMETER(SystemSpecific2);
    UNREFERENCED_PARAMETER(SystemSpecific3);

    UNREFERENCED_PARAMETER(FunctionContext);
}

NDIS_STATUS
HwValidateChannel(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  UCHAR                   Channel
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    
    // Validate that we can set this channel (regulatory compliance)
    ndisStatus = HalValidateChannel(Hw->Hal, PhyId, Channel);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_MISC, DBG_SERIOUS, ("Hal failed channel/frequency validation\n"));
        return ndisStatus;
    }

    return ndisStatus;
}

NDIS_STATUS
HwSetChannel(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  UCHAR                   Channel
    )
{
    // Must only be called for the active phy
    MPASSERT(PhyId == Hw->PhyState.OperatingPhyId);

    // When setting the channel, we dont check if we are not already on that 
    // channel. This is because this may be called after setting a PhyID and
    // that does not necessarily set the channel

    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_IN_CHANNEL_SWITCH);
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

    // Wait for active send threads to finish. We dont wait
    // for anything else on an HAL reset since some of those
    // operations themselves may be causing the reset (Eg. channel
    // switch of a scan)
    HW_WAIT_FOR_ACTIVE_SENDS_TO_FINISH(Hw);
//    MpTrace(COMP_TESTING, DBG_SERIOUS, ("HwSetChannel \n"));    
    HwDisableInterrupt(Hw, HW_ISR_TRACKING_CHANNEL);

    // Flush the sends
    HwFlushSendEngine(Hw, FALSE);

    HalSwitchChannel(Hw->Hal, 
        PhyId,
        Channel, 
        FALSE
        );

    HwResetSendEngine(Hw, FALSE);
    HwResetReceiveEngine(Hw, FALSE);
    HalStartReceive(Hw->Hal);
//    MpTrace(COMP_TESTING, DBG_SERIOUS, ("HwSetChannel \n"));    
    HwEnableInterrupt(Hw, HW_ISR_TRACKING_CHANNEL);

    HW_CLEAR_ADAPTER_STATUS(Hw, HW_ADAPTER_IN_CHANNEL_SWITCH);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwSetOperatingPhyId(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    // Update the PhyID on the hardware. We cannot check if OperatingPhyID == PhyID
    // since on a reset the value in the PhyState is cleared but the value is not
    // programmed on the hardware until later
    HalSetOperatingPhyId(Hw->Hal, PhyId);
    Hw->PhyState.OperatingPhyId = PhyId;

    return ndisStatus;
}

NDIS_STATUS
HwSetPhyContext(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId,
    _In_  PHW_PHY_CONTEXT         PhyContext
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        MpTrace(COMP_MISC, DBG_LOUD, ("Programming channel %d on phy %d\n", PhyContext->Channel, PhyId));
        
        // Set the operating phy ID
        ndisStatus = HwSetOperatingPhyId(Hw, PhyId);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_MISC, DBG_SERIOUS, ("Unable to set operating phyID to %d. Error = 0x%08x\n",
                        PhyId, ndisStatus));
            break;
        }

        // Set the channel on the hardware
        ndisStatus = HwSetChannel(Hw,
                        PhyId, 
                        PhyContext->Channel
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_MISC, DBG_SERIOUS, ("Unable to set channel rate Set. Error = 0x%08x\n",
                        ndisStatus));
            break;
        }

    } while (FALSE);

    return ndisStatus;
}

VOID
HwPhyProgramWorkItem(
    PVOID                   Context,
    NDIS_HANDLE             NdisIoWorkItemHandle
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PHW                         hw = (PHW)Context;
    HW_GENERIC_CALLBACK_FUNC    completionHandler = NULL;
    PHW_PHY_CONTEXT             newPhyContext = NULL;
    PHW_MAC_CONTEXT             completeContext = NULL;

    if (NULL == hw)
    {
        MPASSERT(hw);
        return;
    }

    newPhyContext = hw->PhyState.NewPhyContext;

    UNREFERENCED_PARAMETER(NdisIoWorkItemHandle);

    ndisStatus = HwSetPhyContext(hw, 
                    hw->PhyState.DestinationPhyId, 
                    newPhyContext
                    );

    // Complete the phy programming
    completionHandler = hw->PhyState.PendingPhyProgramCallback;
    completeContext = hw->PhyState.PhyProgramMacContext;
    hw->PhyState.PendingPhyProgramCallback = NULL;

    // Remove the extra ref. We do this before the workitem call since otherwise
    // if from the completion handler context the upper layer calls back into us, we 
    // could deadlock (Hw11CtxSStart is one function that can deadlock)
    HW_DECREMENT_ACTIVE_OPERATION_REF(hw);

    // Invoke the completion handler
    completionHandler(hw, completeContext, &ndisStatus);
}

// Programs the parameters from the PhyContext on the HW for the specified PhyID.
// The PhyId is also made active. If CompletionCallback is NULL, the call completes
// synchronously. If this is not NULL, the completion handler is always called even 
// in case of a failure
NDIS_STATUS
HwProgramPhy(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  ULONG                   PhyId,
    _In_  PHW_PHY_CONTEXT         PhyContext,
    _In_opt_  HW_GENERIC_CALLBACK_FUNC    CompletionCallback
    )    
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_PENDING;
    UCHAR                       i ,numActive = 0;
    BOOLEAN                     programmingDone = FALSE;
    
    do
    {
        //
        // if there are multiple active MACs program the channel only for the first one
        //
        for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
        {
            if (HW_MAC_CONTEXT_MUST_MERGE(&Hw->MacContext[i]))
            {
                numActive++;
            }
        }

        if (numActive > 1)
        {
            // These are multiple MACs active. 
            MpTrace(COMP_MISC, DBG_NORMAL, ("Multiple MACs active. Not programming new phy setting\n"));

            // The new MAC should be programming the H/W on the current channel itself
            MPASSERT((Hw->PhyState.OperatingPhyId == PhyId) &&
                      (HalGetOperatingPhyMIB(Hw->Hal)->Channel == PhyContext->Channel));

            // Success
            ndisStatus = NDIS_STATUS_SUCCESS;
            programmingDone = TRUE;
            break;
        }

        if (HwAwake(Hw, FALSE) != TRUE)
        {        
            // Phy is OFF, we cannot finish this operation
            MpTrace(COMP_MISC, DBG_SERIOUS, ("Unable to activate PHY, not programming new phy settings\n"));
            ndisStatus = NDIS_STATUS_DOT11_POWER_STATE_INVALID;
            programmingDone = TRUE;
            break;
        }
        
        // We could optimize here by checking if we are already on the target Phy or Channel
        // and bailing out here

        // Only one phy programming call is permitted
        MPASSERT(Hw->PhyState.PendingPhyProgramCallback == NULL);

        if (CompletionCallback != NULL)
        {
            // Save the MAC context, etc
            Hw->PhyState.PhyProgramMacContext = HwMac;
            Hw->PhyState.PendingPhyProgramCallback = CompletionCallback;
            Hw->PhyState.DestinationPhyId = PhyId;
            Hw->PhyState.NewPhyContext = PhyContext;

            // Add a ref since we are going to do an async operation
            HW_INCREMENT_ACTIVE_OPERATION_REF(Hw);

            // Caller is OK with asynchronous behavior
            NdisQueueIoWorkItem(
                Hw->PhyState.PhyProgramWorkItem,
                HwPhyProgramWorkItem,
                Hw
                );

            // Completed asynchronously
            ndisStatus = NDIS_STATUS_PENDING;
        }
        else
        {
            // Caller expect synchronous behavior. Do the work inline
            ndisStatus = HwSetPhyContext(Hw, 
                            PhyId,
                            PhyContext
                            );
        }
        
    }while (FALSE);


    if (programmingDone)
    {        
        if (CompletionCallback != NULL)
        {
            // Call the completion handler
            CompletionCallback(Hw, HwMac, &ndisStatus);

            // Already called the complete
            ndisStatus = NDIS_STATUS_PENDING;
        }
        else
        {
            // Caller expects synchronous behavior
            // we are done
        }
    }

    return ndisStatus;
}

