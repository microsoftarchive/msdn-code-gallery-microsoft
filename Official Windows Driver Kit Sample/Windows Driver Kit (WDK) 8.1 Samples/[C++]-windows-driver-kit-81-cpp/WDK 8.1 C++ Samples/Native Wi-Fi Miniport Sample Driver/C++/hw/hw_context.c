/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_context.c

Abstract:
    Implements for MAC context management for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"
#include "hw_context.h"
#include "hw_isr.h"
#include "hw_main.h"
#include "hw_mac.h"
#include "hw_phy.h"
#include "hw_rate.h"

#if DOT11_TRACE_ENABLED
#include "hw_context.tmh"
#endif



NDIS_STATUS
Hw11AllocateMACContext(
    _In_  PHW                     Hw,
    _Outptr_result_maybenull_ PHW_MAC_CONTEXT* MacContext,
    _In_  PVNIC                   VNic,
    _In_  NDIS_PORT_NUMBER        PortNumber    
    )
{
    UCHAR                       i;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_FAILURE;
    *MacContext = NULL;
    
    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        if (!HW_MAC_CONTEXT_IS_VALID(&Hw->MacContext[i]))
        {
            ndisStatus = HwInitializeMACContext(Hw, &Hw->MacContext[i], VNic, i);
            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                // Return this MAC to the caller
                Hw->MacContextCount++;
                HW_SET_MAC_CONTEXT_STATUS(&Hw->MacContext[i], HW_MAC_CONTEXT_VALID | HW_MAC_CONTEXT_PAUSED);
                Hw->MacContext[i].PortNumber = PortNumber;

                *MacContext = &Hw->MacContext[i];            
            }
            // Success of failure, we break out
            break;
        }
    }
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

    if (*MacContext == NULL)
    {
        MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate MAC context in HW for VNIC\n"));
    }
    
    return ndisStatus;
}

VOID
Hw11FreeMACContext(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    )
{
    BOOLEAN                     isActive = FALSE;
    
    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    if (HW_TEST_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_ACTIVE))
    {
        isActive = TRUE;
    }

    if (isActive)
    {
        // We are still active, we need to clear our state from the HW before bailing out
        HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

        if (MacContext->BSSStarted)
        {
            // Stop beaconing
            HwStopBSS(Hw);
        }
            
        // delete all the keys that are currently set on the hardware
        HwDeleteAllKeysFromHw(MacContext);

        HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    }
    
    MacContext->Status = 0;
    
    HwTerminateMACContext(Hw, MacContext);

    Hw->MacContextCount--;
    
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);
}


NDIS_STATUS
Hw11EnableMACContext(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     fAlreadyActive = FALSE;

    MpTrace(COMP_MISC, DBG_NORMAL, ("HwMac(%d): Hw11EnableMACContext called\n", HW_MAC_PORT_NO(MacContext)));

    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    if (HW_TEST_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_ACTIVE))
    {
        fAlreadyActive = TRUE;
    }
    else
    {
        HW_SET_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_ACTIVATING);
    }
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

    if (fAlreadyActive)
    {
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("MAC context is already active. Returning.\n"));
        return NDIS_STATUS_SUCCESS;
    }

    //
    // Program the MAC context on the hardware
    //
    ndisStatus = HwSetMACContextOnNic(Hw, MacContext);
    
    // Do I need this lock?
    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    HW_SET_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_ACTIVE);
    HW_CLEAR_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_ACTIVATING);
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

    return ndisStatus;
}

NDIS_STATUS
Hw11DisableMACContext(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     notActive = FALSE;

    MpTrace(COMP_MISC, DBG_NORMAL, ("HwMac(%d): Hw11DisableMACContext called\n", HW_MAC_PORT_NO(MacContext)));

    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    if (!HW_TEST_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_ACTIVE))
    {
        // This can happen when we have been terminated and the MAC is
        // being disabled after the terminate finished
        notActive = TRUE;
    }
    HW_CLEAR_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_ACTIVE);
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

    if (notActive)
    {
        return ndisStatus;
    }
    
    // Wait for the refcount to go down to 0
    while (MacContext->GenericRefCount > 0)
        NdisMSleep(1000);

    // Wait for the refcount to go down to 0
    while (MacContext->SendRefCount > 0)
        NdisMSleep(1000);

    //
    // Remove this MAC context from the hardware
    //
    ndisStatus = HwClearMACContextFromNic(Hw, MacContext);
    
    return ndisStatus;
}


NDIS_STATUS
Hw11PauseMACContext(
    _In_  PHW_MAC_CONTEXT         MacContext
    )
{
    HW_ACQUIRE_HARDWARE_LOCK(MacContext->Hw, FALSE);
    HW_SET_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_PAUSING);
    HW_RELEASE_HARDWARE_LOCK(MacContext->Hw, FALSE);

    MpTrace(COMP_MISC, DBG_NORMAL, ("HwMac(%d): Hw11PauseMACContext called\n", HW_MAC_PORT_NO(MacContext)));

    //
    // Wait for the periodic timer to finish. It would see the pausing
    // flag and give up
    //
    while (MacContext->PeriodicTimerQueued != 0)
    {
        if (NdisCancelTimerObject(MacContext->Timer_Periodic) == TRUE)
        {
            //
            // Periodic timer is done
            //
            MacContext->PeriodicTimerQueued = 0;
        }
        else
        {
            // Sleep a bit and try again
            NdisMSleep(1000);
        }
    }

    // Wait for the refcount to go down to 0
    while (MacContext->GenericRefCount > 0)
        NdisMSleep(1000);

    // Wait for the refcount to go down to 0
    while (MacContext->SendRefCount > 0)
        NdisMSleep(1000);

    // Wait for the refcount to go down to 0
    HwWaitForPendingReceives(MacContext->Hw, MacContext);

    HW_ACQUIRE_HARDWARE_LOCK(MacContext->Hw, FALSE);
    HW_SET_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_PAUSED);
    HW_CLEAR_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_PAUSING);
    HW_RELEASE_HARDWARE_LOCK(MacContext->Hw, FALSE);

    return NDIS_STATUS_SUCCESS;
}

VOID
Hw11RestartMACContext(
    _In_  PHW_MAC_CONTEXT         MacContext
    )
{
    LARGE_INTEGER               periodicTimerDuration;

    MpTrace(COMP_MISC, DBG_NORMAL, ("HwMac(%d): Hw11RestartMACContext called\n", HW_MAC_PORT_NO(MacContext)));

    HW_ACQUIRE_HARDWARE_LOCK(MacContext->Hw, FALSE);
    HW_CLEAR_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_PAUSED);
    HW_RELEASE_HARDWARE_LOCK(MacContext->Hw, FALSE);

    // Queue the periodic time
    periodicTimerDuration.QuadPart = HW_CONTEXT_PERIODIC_TIMER_DURATION * -1 * 10000;

    // We dont add a ref for the periodic timer. Instead we set a flag that its queued
    MacContext->PeriodicTimerQueued = 1;
    NdisSetTimerObject(MacContext->Timer_Periodic, periodicTimerDuration, 0, NULL);
}


// Called with Lock held
NDIS_STATUS
HwInitializeMACContext(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PVNIC                   VNic,
    _In_  ULONG                   ContextId
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    DOT11_RESET_REQUEST         resetRequest;
    NDIS_TIMER_CHARACTERISTICS  timerChar;   

    do
    {
        NdisZeroMemory(MacContext, sizeof(HW_MAC_CONTEXT));

        // Program the default values
        MacContext->Hw = Hw;
        MacContext->VNic = VNic;

        // Default everything comes up in ExtSTA mode. This does not
        // get changed on a reset but on a set op mode
        MacContext->CurrentOpMode = DOT11_OPERATION_MODE_EXTENSIBLE_STATION;

        NdisZeroMemory(&timerChar, sizeof(NDIS_TIMER_CHARACTERISTICS));
        
        timerChar.Header.Type = NDIS_OBJECT_TYPE_TIMER_CHARACTERISTICS;
        timerChar.Header.Revision = NDIS_TIMER_CHARACTERISTICS_REVISION_1;
        timerChar.Header.Size = sizeof(NDIS_TIMER_CHARACTERISTICS);

        timerChar.AllocationTag = HW_MEMORY_TAG;
        
        // Allocate the Join timeout call back
        timerChar.TimerFunction = HwJoinTimeoutTimer;
        timerChar.FunctionContext = MacContext;

        ndisStatus = NdisAllocateTimerObject(
                        Hw->MiniportAdapterHandle,
                        &timerChar,
                        &MacContext->Timer_JoinTimeout
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate join timer for MAC context\n"));
            break;
        }

        // Allocate the periodic timeout call back
        timerChar.TimerFunction = HwMACContextPeriodicTimer;
        timerChar.FunctionContext = MacContext;

        ndisStatus = NdisAllocateTimerObject(
                        Hw->MiniportAdapterHandle,
                        &timerChar,
                        &MacContext->Timer_Periodic
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate periodic timer for MAC context\n"));
            break;
        }        

        //
        // Assign a MAC address to this context.
        //        
        NdisMoveMemory(MacContext->MacAddress, 
            Hw->MacState.MacAddress,
            DOT11_ADDRESS_SIZE
            );

        if (Hw->RegInfo.MultipleMacAddressEnabled)
        {
            // Let the HW assign a separate MAC address to this port. If it HW supports 
            // multiple MAC addresses, it would provide us a new one for each context. 
            // Else it would use the default
            ndisStatus = HalAssignMACAddress(Hw->Hal, ContextId, MacContext->MacAddress);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to assign MAC address for MAC context\n"));
                break;
            }        
        }
        
        //
        // Reset the MAC context state
        //
        NdisZeroMemory(&resetRequest, sizeof(DOT11_RESET_REQUEST));
        resetRequest.dot11ResetType = dot11_reset_type_phy_and_mac;
        
        resetRequest.bSetDefaultMIB = TRUE;

        // Before we enable it, we reset its contents
        HwResetMACContext(MacContext, &resetRequest);
        
    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // Cleanup
        HwTerminateMACContext(Hw, MacContext);
    }
    
    return ndisStatus;
}

// Called with lock held
VOID
HwTerminateMACContext(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    )
{
    ULONG   i;
    UNREFERENCED_PARAMETER(Hw);
    
    MPASSERT(MacContext->GenericRefCount == 0);

    //
    // Delete any keys we may be holding
    //
    for (i = 0; i < HW11_PEER_TABLE_SIZE; i++)
    {
        // function checks for validity
        HwFreePeerNode(MacContext, &MacContext->PeerTable[i]);
    }
    HwFreePeerNode(MacContext, &MacContext->DefaultPeer);

    if (MacContext->Timer_Periodic)
    {
        NdisFreeTimerObject(MacContext->Timer_Periodic);
        MacContext->Timer_Periodic = NULL;
    }

    if (MacContext->Timer_JoinTimeout)
    {
        NdisFreeTimerObject(MacContext->Timer_JoinTimeout);
        MacContext->Timer_JoinTimeout = NULL;
    }
    
    if (MacContext->BeaconIEBlob != NULL)
    {
        MP_FREE_MEMORY(MacContext->BeaconIEBlob);
    }

    if (MacContext->ProbeResponseIEBlob != NULL)
    {
        MP_FREE_MEMORY(MacContext->ProbeResponseIEBlob);
    }
    
    NdisZeroMemory(MacContext, sizeof(HW_MAC_CONTEXT));
}

//
// Dot11Reset Step 1 - Cleanup any "pending" operations
//
VOID
HwDot11ResetStep1(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest    
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PHW                         hw = MacContext->Hw;

    UNREFERENCED_PARAMETER(Dot11ResetRequest);

    HW_ACQUIRE_HARDWARE_LOCK(hw, FALSE);
    HW_SET_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_IN_DOT11_RESET);    
    HW_SET_ADAPTER_STATUS(hw, HW_ADAPTER_IN_DOT11_RESET);
    HW_RELEASE_HARDWARE_LOCK(hw, FALSE);

    // Wait for pending operations in the hardware to finish
    HW_WAIT_FOR_ACTIVE_OPERATIONS_TO_FINISH(hw);

    // Wait for active sends to be finish
    HW_WAIT_FOR_ACTIVE_SENDS_TO_FINISH(hw);

    // Wait for pending operations and receives in the MAC contexts to get completed. We dont wait
    // for sends
    while (MacContext->GenericRefCount > 0)
        NdisMSleep(1000);

    // If we have started a BSS, stop beaconing
    if (MacContext->BSSStarted)
        HwStopBSS(hw);

    // Disable interrupts
//    MpTrace(COMP_TESTING, DBG_SERIOUS, ("HwDot11ResetStep1 \n"));
    HwDisableInterrupt(hw, HW_ISR_TRACKING_DOT11_RESET);

    ndisStatus = HalResetStart(hw->Hal);
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);

    HalStop(hw->Hal);    

    // Flush sends and wait for pending receives from this mac context
    HwFlushSendEngine(hw, FALSE);
    HwWaitForPendingReceives(hw, MacContext);
}

//
// Dot11Reset Step 2 - Reset our state
//
NDIS_STATUS
HwDot11ResetStep2(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest    
    )
{
    PHW                         hw = MacContext->Hw;

    MpTrace(COMP_MISC, DBG_NORMAL, ("HwMac(%d): Reseting state\n", HW_MAC_PORT_NO(MacContext)));

    // Reset the send and receive engine
    HwResetSendEngine(hw, FALSE);
    HwResetReceiveEngine(hw, FALSE);

    // Remove keys from hardware
    HwDeleteAllKeysFromHw(MacContext);
    
    // Reset HW MAC & PHY state
    HwResetSoftwareMacState(hw);
    HwResetSoftwarePhyState(hw);
    NdisZeroMemory(&hw->Stats, sizeof(NIC_STATISTICS));

    // Reset the MAC context
    HW_ACQUIRE_HARDWARE_LOCK(MacContext->Hw, FALSE);    
    HwResetMACContext(MacContext, Dot11ResetRequest);
    HW_RELEASE_HARDWARE_LOCK(MacContext->Hw, FALSE);    

    MpTrace(COMP_MISC, DBG_NORMAL, ("HwMac(%d): Reapplying new state\n", HW_MAC_PORT_NO(MacContext)));

    //
    // Restart everything
    //
    HalStart(hw->Hal, TRUE);
    
    // Push the new state on the hardware
    HwSetNicState(hw);

    HalResetEnd(hw->Hal);

    MpTrace(COMP_MISC, DBG_NORMAL, ("HwMac(%d): Setting MAC Context\n", HW_MAC_PORT_NO(MacContext)));

    //
    // Now program this MAC context on the HW
    //
    HwSetMACContextOnNic(hw, MacContext);
    
    // Renable the interrupts
//    MpTrace(COMP_TESTING, DBG_SERIOUS, ("HwDot11ResetStep2 \n"));    
    HwEnableInterruptWithSync(hw, HW_ISR_TRACKING_DOT11_RESET);
    
    HW_CLEAR_ADAPTER_STATUS(hw, HW_ADAPTER_IN_DOT11_RESET);
    HW_CLEAR_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_IN_DOT11_RESET);    

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
Hw11Dot11Reset(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest    
    )
{
    MpTrace(COMP_MISC, DBG_NORMAL, ("HwMac(%d): Hw11Dot11Reset called\n", HW_MAC_PORT_NO(MacContext)));
    
    // The reset is a 2 step process
    HwDot11ResetStep1(MacContext, Dot11ResetRequest);
    return HwDot11ResetStep2(MacContext, Dot11ResetRequest);
}

//
// Resets the software state but does not program the hardware
//
NDIS_STATUS
HwResetMACContext(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest    
    )
{
    UCHAR                       index;
    PNICPHYMIB                  phyMib;
    DOT11_MAC_ADDRESS           broadcastAddress = {0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF};

    //
    // Load the default MAC address
    //
    if ((Dot11ResetRequest->dot11ResetType == dot11_reset_type_phy_and_mac) || 
        (Dot11ResetRequest->dot11ResetType == dot11_reset_type_mac))
    {
        // We dont let the OS change the MAC address
//        NdisMoveMemory(MacContext->MacAddress, Dot11ResetRequest->dot11MacAddress, DOT11_ADDRESS_SIZE);
    }

    HW_CLEAR_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_LINK_UP);
    MacContext->PowerMgmtMode.dot11PowerMode = dot11_power_mode_active;

    //
    // Default BSSID
    //
    NdisMoveMemory(MacContext->DesiredBSSID, broadcastAddress, DOT11_ADDRESS_SIZE);
    
    MacContext->BssType = dot11_BSS_type_infrastructure;
    MacContext->SSID.uSSIDLength = 0;
    
    MacContext->BeaconPeriod = MacContext->Hw->RegInfo.BeaconPeriod;
    MacContext->AtimWindow = 0;         // Set when in PS mode
    MacContext->LastBeaconTimestamp = 0;
    
    MacContext->RTSThreshold = MacContext->Hw->RegInfo.RTSThreshold;
    MacContext->FragmentationThreshold = MacContext->Hw->RegInfo.FragmentationThreshold;

    MacContext->ShortRetryLimit = HalGetShortRetryLimit(MacContext->Hw->Hal);
    MacContext->LongRetryLimit = HalGetLongRetryLimit(MacContext->Hw->Hal);

    // Invalidate all the keys
    for (index = 0; index < HW11_KEY_TABLE_SIZE; index++)
    {
        MPASSERT(MacContext->KeyTable[index].hCNGKey == NULL);
        HwFreeKey(&MacContext->KeyTable[index]);
    }
    MacContext->DefaultKeyIndex = 0;
    MacContext->KeyMappingKeyCount = 0;

    // Clear the peer node count
    for (index = 0; index < HW11_PEER_TABLE_SIZE; index++)
    {
        HwFreePeerNode(MacContext, &MacContext->PeerTable[index]);
    }
    MacContext->PeerNodeCount = 0;

    // Default peer is valid
    HwFreePeerNode(MacContext, &MacContext->DefaultPeer);
    MacContext->DefaultPeer.Valid = TRUE;
    ETH_COPY_NETWORK_ADDRESS(MacContext->DefaultPeer.PeerMac, broadcastAddress);

    MacContext->AuthAlgorithm = DOT11_AUTH_OPEN_SYSTEM;
    MacContext->UnicastCipher = DOT11_CIPHER_ALGO_NONE;
    MacContext->MulticastCipher = DOT11_CIPHER_ALGO_NONE;
    MacContext->MulticastAddressCount = 0;
    MacContext->AcceptAllMulticast = FALSE;

    // We do not reset the packet filter here. The Port would set this
    // MacContext->PacketFilter = NDIS_PACKET_TYPE_802_11_BROADCAST_MGMT 
    //                            | NDIS_PACKET_TYPE_802_11_DIRECTED_MGMT;

    MacContext->OperatingPhyId = 0;
    MacContext->SelectedPhyId = 0;

    // Reset all the PHY parameters to their default value
    for (index = 0; index < MAX_NUM_PHY_TYPES; index++)
    {
        phyMib = HalGetPhyMIB(MacContext->Hw->Hal, index);

        // Ensure that all the parameters are reset here

        // Reset the channel to the channel currently set on the HAL structure
        MacContext->PhyContext[index].Channel = phyMib->Channel;

        // Also get the operation rate set
        NdisMoveMemory(&(MacContext->PhyContext[index].OperationalRateSet), &(phyMib->OperationalRateSet), sizeof(DOT11_RATE_SET));

        // This is also the current active rate set
        NdisMoveMemory(&(MacContext->PhyContext[index].ActiveRateSet), &(phyMib->OperationalRateSet), sizeof(DOT11_RATE_SET));        
    }

    MacContext->ShortSlotTimeOptionEnabled = FALSE;
    MacContext->CTSToSelfEnabled = FALSE;

    // Load the default reg domain
    MacContext->CurrentRegDomain = MacContext->Hw->PhyState.DefaultRegDomain;
    
    MacContext->DefaultTXDataRate = 22; // Default data rate = 11 Mbps
    MacContext->DefaultTXMgmtRate = HalGetBeaconRate(MacContext->Hw->Hal, MacContext->OperatingPhyId);

    // Clear the rate information
    NdisZeroMemory(&MacContext->RateInfo, sizeof(HW_RATE_INFO));
    MacContext->RateInfo.CurrentTXDataRate = MacContext->DefaultTXDataRate;

    MacContext->JoinContext = NULL;
    MacContext->ChannelSwitchContext = NULL;
    MacContext->ScanContext = NULL;
    
    // Reset statistics
    NdisZeroMemory(&MacContext->UnicastCounters, sizeof(DOT11_MAC_FRAME_STATISTICS));
    NdisZeroMemory(&MacContext->MulticastCounters, sizeof(DOT11_MAC_FRAME_STATISTICS));

    // Reset general data
    if (MacContext->BeaconIEBlob != NULL)
    {
        MP_FREE_MEMORY(MacContext->BeaconIEBlob);
        MacContext->BeaconIEBlob = NULL;
    }
    MacContext->BeaconIEBlobSize = 0;
    if (MacContext->ProbeResponseIEBlob != NULL)
    {
        MP_FREE_MEMORY(MacContext->ProbeResponseIEBlob);
        MacContext->ProbeResponseIEBlob = NULL;
    }
    MacContext->ProbeResponseIEBlobSize = 0;

    // We always come up as enabled. The upper layer would disable it if it wants to
    MacContext->BeaconEnabled = TRUE;
    MacContext->BSSStarted = FALSE;

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwSetMACContextOnNic(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    )
{
    ULONG PhyId = 0, index = 0;
    PHW_KEY_ENTRY KeyEntry = NULL;

    //
    // This function is called each time a MAC context becomes active. We need to take
    // the config from the MAC context and set it on the HW. If a MAC is already active,
    // we need to merge the config with the existing hardware config, else
    // we can just update the existing programmed value
    //
    if (HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
    {
        // HW should not be touched
        return NDIS_STATUS_SUCCESS;
    }

    HwUpdateBSSType(Hw, MacContext);
    MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Set the BSS type %d \n", MacContext->BssType));
    
    // Op Mode
    HwUpdateOperationMode(Hw, MacContext);

    // Program this MAC context's phy state on the HW
    PhyId = MacContext->OperatingPhyId;
    HwProgramPhy(MacContext->Hw, 
        MacContext, 
        PhyId, 
        &MacContext->PhyContext[PhyId],
        NULL
        );

    // Update the link state    
    HwUpdateLinkState(MacContext->Hw, MacContext);
    
    // set the BSSID 
    HwUpdateBSSID(Hw, MacContext);
    
    // beacon interval
    Hw11SetBeaconPeriod(MacContext, MacContext->BeaconPeriod, TRUE);

    // packet filter
    HwUpdatePacketFilter(Hw, MacContext);

    // Multicast list
    HwUpdateMulticastAddressList(Hw, MacContext);

    // encryption
    HwUpdateUnicastCipher(MacContext->Hw, MacContext);
    for (index = 0; index < HW11_KEY_TABLE_SIZE; index++)
    {
        KeyEntry = &MacContext->KeyTable[index];
        if (KeyEntry->Key.Valid)
        {
            MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Adding key entry (index = %d)\n", index));
            HwAddKeyEntry(MacContext->Hw, KeyEntry);
        }
    }
    
    // If BSS is started, enable it on the H/W
    if (MacContext->BSSStarted)
    {
        // Start BSS on the H/W
        HwResumeBSS(Hw, MacContext, MacContext->BeaconEnabled);
        MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Resummed BSS on the hardware.\n"));
    }
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwClearMACContextFromNic(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext
    )
{
    UNREFERENCED_PARAMETER(Hw);

    if (HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
    {
        // HW should not be touched
        return NDIS_STATUS_SUCCESS;
    }
    
    //
    // This function is called each time a MAC context becomes inactive. We should
    // unset settings from the hardware. Example - remove keys, disable beaconing in
    // Adhoc or AP mode, etc.
    //
    if (MacContext->BSSStarted)
    {
        // Pause BSS (we may already be in no-beacon mode)
        HwPauseBSS(Hw);
    }
        
    // delete all the keys that are currently set on the hardware
    HwDeleteAllKeysFromHw(MacContext);

    // Disable this specific MAC context from the hardware
    HwUpdateUnicastCipher(Hw, MacContext);
    
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwUpdateBSSDescription(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  BOOLEAN                 BSSStart
    )
{
    NDIS_STATUS                 ndisStatus;
    PHW_PEER_NODE               peerNode;
    PUCHAR                      remoteRatesIE;
    UCHAR                       remoteRatesIELength;
    PDOT11_RATE_SET             localRateSet;
    PNICPHYMIB                  phyMib;
    PUCHAR                      ieBlob = NULL;
    ULONG                       ieBlobSize;

    // Both IE blobs must be set (can be set to the same value)
    MPASSERT(BSSDescription->BeaconIEBlobSize > 0);
    MPASSERT(BSSDescription->ProbeIEBlobSize > 0);

    if (BSSStart)
    {
        MPASSERT(MP_COMPARE_MAC_ADDRESS(HwMac->MacAddress, BSSDescription->MacAddress));
        
        if (HwMac->BeaconIEBlob != NULL)
        {
            MP_FREE_MEMORY(HwMac->BeaconIEBlob);
            HwMac->BeaconIEBlobSize = 0;
        }

        if (HwMac->ProbeResponseIEBlob != NULL)
        {
            MP_FREE_MEMORY(HwMac->ProbeResponseIEBlob);
            HwMac->ProbeResponseIEBlobSize = 0;
        }
        
        // We are starting a BSS. Store the information in the default peer node
        peerNode = &HwMac->DefaultPeer;

        // Copy the BSS beacon information into local buffer
        MP_ALLOCATE_MEMORY(HwMac->Hw->MiniportAdapterHandle, 
            &HwMac->BeaconIEBlob, 
            BSSDescription->BeaconIEBlobSize, 
            HW_MEMORY_TAG
            );
        if (HwMac->BeaconIEBlob == NULL)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to allocate space to store beacon information\n"));
            return NDIS_STATUS_RESOURCES;
        }

        // Copy the BSS probe response information into local buffer
        MP_ALLOCATE_MEMORY(HwMac->Hw->MiniportAdapterHandle, 
            &HwMac->ProbeResponseIEBlob, 
            BSSDescription->ProbeIEBlobSize, 
            HW_MEMORY_TAG
            );
        if (HwMac->ProbeResponseIEBlob == NULL)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to allocate space to store probe response information\n"));
            MP_FREE_MEMORY(HwMac->BeaconIEBlob);
            HwMac->BeaconIEBlob = NULL;
            return NDIS_STATUS_RESOURCES;
        }

        NdisMoveMemory(HwMac->BeaconIEBlob, 
            &BSSDescription->IEBlobs[BSSDescription->BeaconIEBlobOffset], 
            BSSDescription->BeaconIEBlobSize
            );
        HwMac->BeaconIEBlobSize = BSSDescription->BeaconIEBlobSize;

        NdisMoveMemory(HwMac->ProbeResponseIEBlob, 
            &BSSDescription->IEBlobs[BSSDescription->ProbeIEBlobOffset], 
            BSSDescription->ProbeIEBlobSize
            );
        HwMac->ProbeResponseIEBlobSize = BSSDescription->ProbeIEBlobSize;
        
    }
    else
    {
        MPASSERT(!MP_COMPARE_MAC_ADDRESS(HwMac->MacAddress, BSSDescription->MacAddress));
        
        // Allocate or find a peer node structure to use for this. 
        // The TRUE is needed for Adhoc/AP mode
        peerNode = HwFindPeerNode(HwMac, BSSDescription->MacAddress, TRUE);
        if (peerNode == NULL)
        {   
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to reserve space for peer node information\n"));
            return NDIS_STATUS_RESOURCES;
        }
    }
    // Some of the information gets loaded into the MAC context
    HwMac->BssType = BSSDescription->BSSType;
    
    NdisMoveMemory(HwMac->DesiredBSSID, BSSDescription->BSSID, sizeof(DOT11_MAC_ADDRESS));

    // The use settings from the beacon IE blob 
    ieBlob = &BSSDescription->IEBlobs[BSSDescription->BeaconIEBlobOffset];
    ieBlobSize = BSSDescription->BeaconIEBlobSize;
    
    // Copy the SSID
    ndisStatus = Dot11CopySSIDFromMemoryBlob(
        ieBlob,
        ieBlobSize,
        &HwMac->SSID
        );
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        //
        // A required IE is missing. Fail the request.
        //
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Mandatory SSID Info Element not found in BSS description\n"));
        return NDIS_STATUS_INVALID_DATA;
    }
    
    HwMac->BeaconPeriod = HW_LIMIT_BEACON_PERIOD(BSSDescription->BeaconPeriod);
    HwMac->LastBeaconTimestamp = BSSDescription->Timestamp;
    
    // Some stuff goes into the peer node fields
    peerNode->CapabilityInfo = BSSDescription->Capability.usValue;

    //
    // If called has not selected PhyID/Channel, use the current one
    //
    if (BSSDescription->Channel == 0)
    {
        //
        // Channel/PhyID 
        //
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("Channel/PhyID not specified for join. Using current values\n"));
        BSSDescription->PhyId = HwMac->OperatingPhyId;
        BSSDescription->Channel = HwMac->PhyContext[BSSDescription->PhyId].Channel;
    }

    //
    // For management packets, pick the lowest basic data rate supported between the AP and us as the
    // rate for management packets
    //
    phyMib = HalGetPhyMIB(HwMac->Hw->Hal, BSSDescription->PhyId);
    localRateSet = &phyMib->BasicRateSet; // TODO: Should this happen with MAC context specific rate set?
    HwMac->DefaultTXMgmtRate = HalGetBeaconRate(HwMac->Hw->Hal, BSSDescription->PhyId);

    ndisStatus = Dot11GetInfoEle(
        ieBlob,
        ieBlobSize,
        DOT11_INFO_ELEMENT_ID_SUPPORTED_RATES,
        &remoteRatesIELength,
        &remoteRatesIE
        );
    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        if (!HwSelectLowestMatchingRate(
                remoteRatesIE,
                remoteRatesIELength,
                localRateSet,
                &HwMac->DefaultTXMgmtRate
                ))
        {
            //
            // Look in the extended rates set
            //
            ndisStatus = Dot11GetInfoEle(
                ieBlob,
                ieBlobSize,
                DOT11_INFO_ELEMENT_ID_EXTD_SUPPORTED_RATES,
                &remoteRatesIELength,
                &remoteRatesIE
                );
            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                if (!HwSelectLowestMatchingRate(
                        remoteRatesIE,
                        remoteRatesIELength,
                        localRateSet,
                        &HwMac->DefaultTXMgmtRate
                        ))
                {
                    // 
                    // Didnt find any prefered data rate, select the default
                    //
                    HwMac->DefaultTXMgmtRate = HalGetBeaconRate(HwMac->Hw->Hal, BSSDescription->PhyId);
                }
            }
        }
    }

    return NDIS_STATUS_SUCCESS;
}

VOID
Hw11StopBSS(
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    // Stop BSS
    HwStopBSS(HwMac->Hw);
    
    HwMac->BSSStarted = FALSE;
}

NDIS_STATUS
Hw11StartBSS(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription
    )
{
    NDIS_STATUS                 ndisStatus;

    //
    // Check if we are in the middle of an operation that does not
    // let us start a BSS. Any operation that blocks a receive blocks
    // us from starting a BSS
    //
    HW_ACQUIRE_HARDWARE_LOCK(HwMac->Hw, FALSE);
    if (HW_TEST_ADAPTER_STATUS(HwMac->Hw, HW_CANNOT_RECEIVE_FLAGS))
    {
        // We cannot perform the start
        HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Start BSS failed because HW is in invalid state 0x%08x\n", HwMac->Hw->Status));
        return HwGetAdapterStatus(HwMac->Hw);
    }

    //
    // We should fail the start request if the radio is OFF
    //
    if (HW_TEST_ADAPTER_STATUS(HwMac->Hw, HW_CANNOT_ACCESS_HARDWARE))
    {
        // We cannot perform the start
        HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Start BSS failed because hardware is not available\n"));
        return HwGetAdapterStatus(HwMac->Hw);
    }
    
    HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);

    do
    {
        //
        // Copy the information from the BSS to our buffer
        //
        ndisStatus = HwUpdateBSSDescription(HwMac, BSSDescription, TRUE);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        // BSS has been started
        HwMac->BSSStarted = TRUE;

        // Pass this to the hardware
        ndisStatus = HwStartBSS(HwMac->Hw, HwMac, HwMac->BeaconEnabled);
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
Hw11JoinBSS(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_BSS_DESCRIPTION     BSSDescription,
    _In_  ULONG                   JoinFailureTimeout,
    _In_  VNIC11_GENERIC_CALLBACK_FUNC    CompletionHandler,
    _In_  PVOID                   JoinContext
    )
{
    NDIS_STATUS                 ndisStatus;
    ULONG                       origPhyId;
    UCHAR                       origChannel;

    //
    // Check if we are in the middle of an operation that does not
    // let us do a join. Any operation that blocks a receive blocks
    // us from joining
    //
    HW_ACQUIRE_HARDWARE_LOCK(HwMac->Hw, FALSE);
    if (HW_TEST_ADAPTER_STATUS(HwMac->Hw, HW_CANNOT_RECEIVE_FLAGS))
    {
        // We cannot perform the join
        HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Join failed because HW is in invalid state 0x%08x\n", HwMac->Hw->Status));
        return HwGetAdapterStatus(HwMac->Hw);
    }

    if (HW_TEST_ADAPTER_STATUS(HwMac->Hw, HW_CANNOT_ACCESS_HARDWARE))
    {
        // We cannot perform the join
        HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Join failed because adapter is not available\n"));
        return HwGetAdapterStatus(HwMac->Hw);
    }
    
    // Add a ref count to the MAC context to avoid it from getting context
    // switched, etc
    HW_ADD_MAC_CONTEXT_REF(HwMac, 1);

    // Save the completion handler
    HwMac->PendingJoinCallback = CompletionHandler;
    HwMac->JoinContext = JoinContext;
    HwMac->JoinFailureTimeout = JoinFailureTimeout;
    
    HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);

    // Now the actual the join operation
    do
    {
        //
        // Copy the information from the BSS to our buffer
        //
        ndisStatus = HwUpdateBSSDescription(HwMac, BSSDescription, FALSE);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // Clear this
            HwMac->PendingJoinCallback = NULL;
            break;
        }

        //
        // If the current Phy ID or Channel does not match the one specified in
        // the BSS description, change the channel
        //
        if ((HwMac->OperatingPhyId != BSSDescription->PhyId) ||
            (HwMac->PhyContext[BSSDescription->PhyId].Channel != BSSDescription->Channel))
        {
            origPhyId = HwMac->OperatingPhyId;
            origChannel = HwMac->PhyContext[HwMac->OperatingPhyId].Channel;
            
            // Need to switch. Update our local phy state and program the HW
            HwMac->OperatingPhyId = BSSDescription->PhyId;
            HwMac->PhyContext[BSSDescription->PhyId].Channel = BSSDescription->Channel;

            ndisStatus = HwProgramPhy(HwMac->Hw, 
                            HwMac, 
                            BSSDescription->PhyId,
                            &HwMac->PhyContext[BSSDescription->PhyId],
                            HwJoinChannelSwitchComplete
                            );
            if (ndisStatus != NDIS_STATUS_PENDING)
            {
                MPASSERT(ndisStatus != NDIS_STATUS_SUCCESS);
                HwMac->PendingJoinCallback = NULL;

                // Restore original values
                HwMac->OperatingPhyId = origPhyId;
                HwMac->PhyContext[HwMac->OperatingPhyId].Channel = origChannel;
            }
        }
        else
        {
            HwJoinChannelSwitchComplete(HwMac->Hw, HwMac, NULL);
            ndisStatus = NDIS_STATUS_PENDING;
        }

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_PENDING)
    {
        HW_REMOVE_MAC_CONTEXT_REF(HwMac, 1);
    }

    return ndisStatus;
}

NDIS_STATUS 
HwJoinChannelSwitchComplete(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_opt_  PVOID                   Data
    )
{
    LARGE_INTEGER               joinFailureInMs;

    UNREFERENCED_PARAMETER(Hw);
    UNREFERENCED_PARAMETER(Data);
    
    //
    // Channel switch for the join is done. Now wait for the join
    //
    HwPrepareToJoinBSS(MacContext->Hw, MacContext);
    
    // Timeout passed in is in Beacon Periods. For relative time, the set timer 
    // function requires a negative timeout in 100nanoseconds.
    joinFailureInMs.QuadPart = HW_TU_TO_MS(MacContext->JoinFailureTimeout * MacContext->BeaconPeriod);
    joinFailureInMs.QuadPart = joinFailureInMs.QuadPart * -1 * 10000;

    // Add a ref for the async function and set the timer        
    HW_INCREMENT_ACTIVE_OPERATION_REF(Hw);
    NdisSetTimerObject(MacContext->Timer_JoinTimeout, joinFailureInMs, 0, NULL);

    // Set the bit that we are waiting for join
    HW_WAIT_FOR_BSS_JOIN(MacContext);

    return NDIS_STATUS_SUCCESS;
}

VOID
HwJoinBSSComplete(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_opt_  PHW_RX_MPDU             Mpdu,
    _In_  NDIS_STATUS             Status
    )
{
    VNIC11_GENERIC_CALLBACK_FUNC    completionHandler = NULL;
    PVOID                           completeContext = NULL;

    UNREFERENCED_PARAMETER(Mpdu);

    MpTrace(COMP_ASSOC, DBG_NORMAL, ("Join completed with status 0x%08x\n", Status));
    
    HW_ACQUIRE_HARDWARE_LOCK(HwMac->Hw, FALSE);
    completionHandler = HwMac->PendingJoinCallback;
    completeContext = HwMac->JoinContext;

    HwMac->PendingJoinCallback = NULL;
    HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);
    MPASSERT(completeContext != NULL);

    // Invoke the completion handler
    completionHandler(HwMac->VNic, completeContext, &Status);

    // Remove the extra ref
    HW_REMOVE_MAC_CONTEXT_REF(HwMac, 1);
}

VOID
HwJoinTimeoutTimer(
    PVOID                   SystemSpecific1,
    PVOID                   FunctionContext,
    PVOID                   SystemSpecific2,
    PVOID                   SystemSpecific3
    )
{
    PHW_MAC_CONTEXT             macContext = (PHW_MAC_CONTEXT)FunctionContext;

    UNREFERENCED_PARAMETER(SystemSpecific1);
    UNREFERENCED_PARAMETER(SystemSpecific2);
    UNREFERENCED_PARAMETER(SystemSpecific3);
    
    //
    // Cancel the join operation if it has not already completed
    //
    if (HW_STOP_WAITING_FOR_JOIN(macContext))
    {
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("Cancelled the Join Operation as it took too long\n"));
        HwJoinBSSComplete(macContext, NULL, NDIS_STATUS_FILE_NOT_FOUND);
    }

    // Remove the Async function ref
    HW_DECREMENT_ACTIVE_OPERATION_REF(macContext->Hw);
}

VOID
Hw11NotifyEventConnectionState(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  BOOLEAN                 Connected,
    _In_  ULONG                   StatusType,
    _In_  PVOID                   StatusBuffer,
    _In_  ULONG                   StatusBufferSize,
    _In_  BOOLEAN                 fProgramHardware
    )
{
    PHW_PEER_NODE               peerNode;
    PDOT11_ASSOCIATION_START_PARAMETERS         startParams;
    PDOT11_ASSOCIATION_COMPLETION_PARAMETERS    completionParams;
    PDOT11_DISASSOCIATION_PARAMETERS            disassocParams;
    ULONG                       associateID;

    UNREFERENCED_PARAMETER(StatusBufferSize);
    UNREFERENCED_PARAMETER(fProgramHardware);

    //
    // Inform the HW update change in connect state for this MAC
    //
    if (Connected)
    {
        HW_SET_MAC_CONTEXT_STATUS(HwMac, HW_MAC_CONTEXT_LINK_UP);
    }
    else
    {
        HW_CLEAR_MAC_CONTEXT_STATUS(HwMac, HW_MAC_CONTEXT_LINK_UP);
    }
    
    HwUpdateLinkState(HwMac->Hw, HwMac);

    switch (StatusType)
    {
        case NDIS_STATUS_DOT11_ASSOCIATION_START:
            {
                // Attempt to allocate a peer node data structure
                startParams = (PDOT11_ASSOCIATION_START_PARAMETERS)StatusBuffer;
                peerNode = HwFindPeerNode(HwMac, startParams->MacAddr, TRUE);
                if (peerNode == NULL)
                {
                    MpTrace(COMP_ASSOC, DBG_NORMAL, ("Unable to allocate state for new association peer\n"));
                    break;
                }
            }
            break;
            
        case NDIS_STATUS_DOT11_ASSOCIATION_COMPLETION:
            {
                // Set the Association ID on the hardware
                completionParams = (PDOT11_ASSOCIATION_COMPLETION_PARAMETERS)StatusBuffer;
                if (Connected && (completionParams->uIHVDataOffset != 0))
                {
                    associateID = *((ULONG*)((PUCHAR)StatusBuffer + completionParams->uIHVDataOffset));
                }
                else
                {
                    associateID = 0;
                }
                
                peerNode = HwFindPeerNode(HwMac, completionParams->MacAddr, FALSE);
                if (peerNode != NULL)
                {
                    // If the peer node is NULL, then we are reseting or something.
                    peerNode->AssociateId = (USHORT)associateID;
                    HwUpdateAssociateId(HwMac->Hw, peerNode->AssociateId);
                }
                else
                {
                    MpTrace(COMP_ASSOC, DBG_NORMAL, ("Unable to find peer node on peer association completion\n"));
                }
            }
            break;

        case NDIS_STATUS_DOT11_DISASSOCIATION:
            {
                disassocParams = (PDOT11_DISASSOCIATION_PARAMETERS)StatusBuffer;

                // Invalidate the node that we just found
                peerNode = HwFindPeerNode(HwMac, disassocParams->MacAddr, FALSE);
                if (peerNode != NULL)
                {
                    // This peer is no longer "active"
                    HwFreePeerNode(HwMac, peerNode);

                    // Clear the associate ID from the HW
                    HwUpdateAssociateId(HwMac->Hw, 0);
                }
            }
            break;

        default:
            break;
        
    }

 }

NDIS_STATUS
Hw11StartScan(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PMP_SCAN_REQUEST        ScanRequest,
    _In_  VNIC11_GENERIC_CALLBACK_FUNC    CompletionHandler,
    _In_  PVOID                   ScanContext
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    //
    // Check if we are in the middle of an operation that does not
    // let us do a scan. Any operation that blocks a receive blocks
    // us from scan
    //
    HW_ACQUIRE_HARDWARE_LOCK(HwMac->Hw, FALSE);
    if (HW_TEST_ADAPTER_STATUS(HwMac->Hw, HW_CANNOT_RECEIVE_FLAGS | HW_CANNOT_SEND_FLAGS))
    {
        // We cannot perform the scan
        HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);
        return HwGetAdapterStatus(HwMac->Hw);
    }

    // Add a ref count to the MAC context to avoid it from getting context
    // switched, etc
    HW_ADD_MAC_CONTEXT_REF(HwMac, 1);

    //
    // Save the scan completion handler and the scan context
    //
    HwMac->PendingScanCallback = CompletionHandler;
    HwMac->ScanContext = ScanContext;

    HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);

    //
    // Pass the scan to the HW
    //
    ndisStatus = HwStartScan(HwMac->Hw, HwMac, ScanRequest);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        HwMac->PendingScanCallback = NULL;

        // Remove the extra ref
        HW_REMOVE_MAC_CONTEXT_REF(HwMac, 1);
    }
    else 
    {
        // return pending since we are going to callback
        ndisStatus = NDIS_STATUS_PENDING;
    }
    
    return ndisStatus;
}


VOID
HwScanComplete(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  NDIS_STATUS             Status
    )
{
    VNIC11_GENERIC_CALLBACK_FUNC    completionHandler = NULL;
    PVOID                           completeContext = NULL;

    MpTrace(COMP_SCAN, DBG_NORMAL, ("Scan completed with status 0x%08x\n", Status));
    
    HW_ACQUIRE_HARDWARE_LOCK(HwMac->Hw, FALSE);
    completionHandler = HwMac->PendingScanCallback;
    completeContext = HwMac->ScanContext;

    HwMac->PendingScanCallback = NULL;
    HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);
    MPASSERT(completeContext != NULL);

    // Invoke the completion handler
    completionHandler(HwMac->VNic, completeContext, &Status);

    // Remove the extra ref
    HW_REMOVE_MAC_CONTEXT_REF(HwMac, 1);
}

VOID
Hw11CancelScan(
    _In_  PHW_MAC_CONTEXT         HwMac
    )
{
    HwCancelScan(HwMac->Hw);
}

VOID
HwMACContextPeriodicTimer(
    PVOID                   SystemSpecific1,
    PVOID                   FunctionContext,
    PVOID                   SystemSpecific2,
    PVOID                   SystemSpecific3
    )
{
    PHW_MAC_CONTEXT             macContext = (PHW_MAC_CONTEXT)FunctionContext;
    BOOLEAN                     requeue = TRUE;
    LARGE_INTEGER               periodicTimerDuration;

    UNREFERENCED_PARAMETER(SystemSpecific1);
    UNREFERENCED_PARAMETER(SystemSpecific2);
    UNREFERENCED_PARAMETER(SystemSpecific3);

    do
    {
        MPASSERT(macContext->PeriodicTimerQueued != 0);

        // Dont continue if we are pausing, etc
        if (HW_TEST_MAC_CONTEXT_STATUS(macContext, (HW_MAC_CONTEXT_PAUSED | HW_MAC_CONTEXT_PAUSING)))
        {
            MpTrace(COMP_MISC, DBG_NORMAL, ("Aborting MAC context periodic timer\n"));
            requeue = FALSE;
            break;
        }

        // Update the TX data rate. 
        HwUpdateTxDataRate(macContext);

    }while (FALSE);

    if (requeue)
    {
        MpTrace(COMP_MISC, DBG_LOUD, ("Requeuing MAC context periodic timer\n"));
        
        // Requeue the periodic time
        periodicTimerDuration.QuadPart = Int32x32To64(HW_CONTEXT_PERIODIC_TIMER_DURATION, -10000);

        // We dont add a ref for the periodic timer. Instead we set a flag that its queued. This is
        // because we dont want to wait for this timer when we are being deactivated, etc
        macContext->PeriodicTimerQueued = 1;
        NdisSetTimerObject(macContext->Timer_Periodic, periodicTimerDuration, 0, NULL);
    }
    else
    {
        // Dont queue again
        macContext->PeriodicTimerQueued = 0;
    }

}

NDIS_STATUS
Hw11SetBeaconEnabledFlag(    
    _In_  PHW_MAC_CONTEXT         HwMac,
    BOOLEAN                     BeaconEnabled,
    BOOLEAN                     fProgramHardware
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    if (BeaconEnabled == HwMac->BeaconEnabled)
    {
        // Do nothing
        MpTrace(COMP_MISC, DBG_LOUD, ("HwMac(%d): Ignoring Beacon Enabled flag with no change\n", HwMac->PortNumber));
        return NDIS_STATUS_SUCCESS;
    }

    // We dont want a context switch to happen while we are doing this
    HW_INCREMENT_ACTIVE_OPERATION_REF(HwMac->Hw);
    
    HwMac->BeaconEnabled = BeaconEnabled;

    if (fProgramHardware && (HwMac->BSSStarted == TRUE))
    {
        if (BeaconEnabled)
        {
            // Start BSS
            MpTrace(COMP_MISC, DBG_LOUD, ("HwMac(%d): Starting BSS beaconing\n", HwMac->PortNumber));
            HwEnableBSSBeacon(HwMac->Hw, HwMac);
        }
        else
        {
            // Stop BSS
            MpTrace(COMP_MISC, DBG_LOUD, ("HwMac(%d): Stopping BSS beaconing\n", HwMac->PortNumber));
            HwDisableBSSBeacon(HwMac->Hw);
        }
        
    }
    else
    {
        // Just update the local state
        MpTrace(COMP_MISC, DBG_LOUD, ("HwMac(%d): Updated Beacon Enabled flag to %s WITH__out programming hardware\n", 
            HwMac->PortNumber,
            (BeaconEnabled ? "TRUE" : "FALSE")
            ));
    }

    HW_DECREMENT_ACTIVE_OPERATION_REF(HwMac->Hw);

    return ndisStatus;
}

NDIS_STATUS 
Hw11SetBeaconIE(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PVOID                   pBeaconIEBlob,
    _In_  ULONG                   uBeaconIEBlobSize,
    _In_  BOOLEAN                 fProgramHardware
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR                      oldBeaconIEBlob = NULL;
    PUCHAR                      newBeaconIEBlob = NULL;

    //
    // The ISR can be reading the beacon IE at this time. Process
    // this OID at device IRQL
    //
    if (HwMac->BeaconIEBlob != NULL)
    {
        // Save the old beacon since we would be replacing it
        oldBeaconIEBlob = HwMac->BeaconIEBlob;
    }
    
    MP_ALLOCATE_MEMORY(HwMac->Hw->MiniportAdapterHandle, 
        &newBeaconIEBlob, 
        uBeaconIEBlobSize, 
        HW_MEMORY_TAG
        );
    if (newBeaconIEBlob == NULL)
    {
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to allocate space to store beacon information\n"));
        return NDIS_STATUS_RESOURCES;
    }
    NdisMoveMemory(newBeaconIEBlob, pBeaconIEBlob, uBeaconIEBlobSize);
    
    // Raise to device IRQL before updating the Beacon IE blob. This will
    // ensure that if the HwProcessBeaconInterrupt is in progress it will get the 
    // correct value
    HwSetBeaconIEWithSync(HwMac, newBeaconIEBlob, uBeaconIEBlobSize);

    if (fProgramHardware)
    {
        // Update the beacon on the hardware. 
        HwSetupBeacon(HwMac->Hw, HwMac);
    }
    
    // Free the old beacon buffer. It has been replaced with the new beacon buffer
    if (oldBeaconIEBlob)
        MP_FREE_MEMORY(oldBeaconIEBlob);

    return ndisStatus;
}

NDIS_STATUS 
Hw11SetProbeResponseIE(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PVOID                   pResponseIEBlob,
    _In_  ULONG                   uResponseIEBlobSize,
    _In_  BOOLEAN                 fProgramHardware
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    // This doesnt need to be programmed on the hardware. It gets picked up
    UNREFERENCED_PARAMETER(fProgramHardware);
    
    HW_ACQUIRE_HARDWARE_LOCK(HwMac->Hw, FALSE);
    
    do
    {
        if (HwMac->ProbeResponseIEBlob != NULL)
        {
            MP_FREE_MEMORY(HwMac->ProbeResponseIEBlob);
        }
        HwMac->ProbeResponseIEBlobSize = 0;
        
        MP_ALLOCATE_MEMORY(HwMac->Hw->MiniportAdapterHandle, 
            &HwMac->ProbeResponseIEBlob, 
            uResponseIEBlobSize, 
            HW_MEMORY_TAG
            );
        if (HwMac->ProbeResponseIEBlob == NULL)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to allocate space to store probe response information\n"));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        NdisMoveMemory(HwMac->ProbeResponseIEBlob, pResponseIEBlob, uResponseIEBlobSize);
        HwMac->ProbeResponseIEBlobSize = uResponseIEBlobSize;      
    }while (FALSE);
        
    HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);

    return ndisStatus;
}

