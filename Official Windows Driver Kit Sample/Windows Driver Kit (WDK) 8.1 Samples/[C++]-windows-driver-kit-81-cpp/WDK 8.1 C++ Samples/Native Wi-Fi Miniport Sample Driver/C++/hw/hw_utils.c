/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_utils.c

Abstract:
    Implements helper routines for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "hw_utils.h"
#include "hw_phy.h"
#include "hw_mac.h"

#if DOT11_TRACE_ENABLED
#include "hw_utils.tmh"
#endif

NDIS_STATUS
HwGetAdapterStatus(
    _In_  PHW                     Hw
    )
{
    NDIS_STATUS ndisStatus;

    if (HW_TEST_ADAPTER_STATUS(Hw, HW_ADAPTER_PAUSED))
        ndisStatus = NDIS_STATUS_PAUSED;
    else if (HW_TEST_ADAPTER_STATUS(Hw, HW_ADAPTER_PAUSING))
        ndisStatus = NDIS_STATUS_PAUSED;
    else if (HW_TEST_ADAPTER_STATUS(Hw, HW_ADAPTER_IN_RESET))
        ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
    else if (HW_TEST_ADAPTER_STATUS(Hw, HW_ADAPTER_HALTING))
        ndisStatus = NDIS_STATUS_CLOSING;
    else if (HW_TEST_ADAPTER_STATUS(Hw, HW_ADAPTER_SURPRISE_REMOVED))
        ndisStatus = NDIS_STATUS_ADAPTER_REMOVED;
    else if (HW_TEST_ADAPTER_STATUS(Hw, HW_ADAPTER_RADIO_OFF))
        ndisStatus = NDIS_STATUS_DOT11_POWER_STATE_INVALID;
    else
        ndisStatus = NDIS_STATUS_FAILURE;       // return a generc error

    return ndisStatus;
}

VOID
HwFreePeerNode(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PHW_PEER_NODE           PeerNode
    )
{
    ULONG   i = 0;

    if (PeerNode->Valid)
    {
        for (i = 0; i < DOT11_MAX_NUM_DEFAULT_KEY; i++)
        {
            // Remove any private key that may still be around
            if (PeerNode->PrivateKeyTable[i].Key.Valid)
            {
                HwFreeKey(&PeerNode->PrivateKeyTable[i]);
            }
        }
        PeerNode->Valid = FALSE;
        PeerNode->AssociateId = 0;
        HwMac->PeerNodeCount--;
    }
}

PHW_PEER_NODE
HwFindPeerNode(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  BOOLEAN                 Allocate
    )
{
    UCHAR                       index;
    UCHAR                       emptyIndex;
    PHW_PEER_NODE               peerNode = NULL;

    //
    // Search the peer key table to find the matching peer node
    //
    emptyIndex = HW11_PEER_TABLE_SIZE;
    
    for (index = 0; index < HW11_PEER_TABLE_SIZE; index++)
    {
        if ((HwMac->PeerTable[index].Valid) && 
            (MP_COMPARE_MAC_ADDRESS(HwMac->PeerTable[index].PeerMac, MacAddr)))
        {
            //
            // Found the matching peer node
            //
            peerNode = &HwMac->PeerTable[index];
            break;
        }

        //
        // This is temporary for the case that we dont find the value
        //
        if (!HwMac->PeerTable[index].Valid && emptyIndex == HW11_PEER_TABLE_SIZE)
        {
            emptyIndex = index;
        }
    }

    if ((index == HW11_PEER_TABLE_SIZE) && Allocate)
    {
        //
        // We did not find a per-STA key with matching MacAddr. 
        //
        if (emptyIndex == HW11_PEER_TABLE_SIZE)
        {
            //
            // If we are asked to add a peer but the table is full, fail the request.
            //
            return NULL;
        }

        //
        // The location to add the peer is found
        //
        index = emptyIndex;
        peerNode = &HwMac->PeerTable[index];

        //
        // Copy the MAC address into the node structure
        //
        ETH_COPY_NETWORK_ADDRESS(peerNode->PeerMac, MacAddr);
        peerNode->Valid = TRUE;
        HwMac->PeerNodeCount++; // New node added        
    }

    return peerNode;
}

NDIS_STATUS
HwFindKeyMappingKeyIndex(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       MacAddr,
    _In_  BOOLEAN                 Allocate,
    _Out_ PULONG                  ReturnIndex
    )
{
    UCHAR                       index;
    UCHAR                       emptyIndex, remainingKeyCount;
    PHW_KEY_ENTRY               keyEntry = NULL;

    //
    // Search the key mapping table to find either a matching MacAddr or an empty key entry.
    //
    emptyIndex = 0;

    // We will walk the list below until we find the key or an empty key index. For optimization
    // we will only look at KeyMappingKeyCount + 1 number of valid entries. The 1 is added so
    // that we can find an empty space
    remainingKeyCount = HwMac->KeyMappingKeyCount + 1;
    
    for (index = DOT11_MAX_NUM_DEFAULT_KEY; (index < HW11_KEY_TABLE_SIZE) && (remainingKeyCount > 0); index++)
    {
        keyEntry = &HwMac->KeyTable[index];
        if (keyEntry->Key.Valid)
        { 
            if (MP_COMPARE_MAC_ADDRESS(keyEntry->Key.MacAddr, MacAddr))
            {
                *ReturnIndex = index;
                return NDIS_STATUS_SUCCESS;
            }

            remainingKeyCount--;
        }

        if (!keyEntry->Key.Valid && emptyIndex == 0)
        {
            emptyIndex = index;
            remainingKeyCount--;    // Remove the extra 1 we have added
        }
    }

    //
    // Not found, can we allocate a new one?
    //
    if (Allocate)
    {
        if (emptyIndex == 0)
        {
            //
            // No space to allocate new one
            //
            return NDIS_STATUS_RESOURCES;
        }
        
        //
        // Use this entry
        //
        keyEntry = &HwMac->KeyTable[emptyIndex];        
        ETH_COPY_NETWORK_ADDRESS(keyEntry->Key.MacAddr, MacAddr);
        keyEntry->MacContext = HwMac;
        
        *ReturnIndex = emptyIndex;
        
        return NDIS_STATUS_SUCCESS;
    }


    return NDIS_STATUS_FAILURE;
}


ULONG64
HwDataRateToLinkSpeed(
    UCHAR  rate
    )
{
    return ((ULONG64)500000) * rate;
}

UCHAR
HwLinkSpeedToDataRate(
    ULONG64 linkSpeed
    )
{
    return (UCHAR)(linkSpeed / 500000);
}

BOOLEAN
HwSelectLowestMatchingRate(
    PUCHAR          remoteRatesIE,
    UCHAR           remoteRatesIELength,
    PDOT11_RATE_SET localRateSet,
    PUSHORT         SelectedRate
    )
{
    UCHAR           indexHwRates;
    UCHAR           indexAPRates;

    for (indexHwRates = 0; indexHwRates < localRateSet->uRateSetLength; indexHwRates++)
    {
        for (indexAPRates = 0; indexAPRates < remoteRatesIELength; indexAPRates++)
        {
            if (localRateSet->ucRateSet[indexHwRates] == (remoteRatesIE[indexAPRates] & 0x7f))
            {
                *SelectedRate = localRateSet->ucRateSet[indexHwRates];
                return TRUE;
            }
        }
    }

    return FALSE;
}


VOID
HwIndicateLinkSpeed(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  USHORT                  LinkSpeed
    )
{
    NDIS_LINK_STATE         linkState;
    
    NdisZeroMemory(&linkState, sizeof(NDIS_LINK_STATE));
    
    //
    // Fill in object headers
    //
    linkState.Header.Revision = NDIS_LINK_STATE_REVISION_1;
    linkState.Header.Type = NDIS_OBJECT_TYPE_DEFAULT;
    linkState.Header.Size = sizeof(NDIS_LINK_STATE);

    //
    // Link state buffer
    //
    linkState.MediaConnectState = MediaConnectStateConnected;
    linkState.MediaDuplexState = MediaDuplexStateHalf;
    linkState.RcvLinkSpeed = HwDataRateToLinkSpeed((UCHAR)LinkSpeed);
    linkState.XmitLinkSpeed = HwDataRateToLinkSpeed((UCHAR)LinkSpeed);
   
    Hvl11IndicateStatus(
        HwMac->VNic,
        NDIS_STATUS_LINK_STATE,
        &linkState,
        sizeof(NDIS_LINK_STATE)
        );
    
}

VOID
HwIndicateMICFailure(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PDOT11_DATA_SHORT_HEADER Header,
    _In_  ULONG                   KeyId,
    _In_  BOOLEAN                 IsDefaultKey
    )
{
    DOT11_TKIPMIC_FAILURE_PARAMETERS    param;

    MP_ASSIGN_NDIS_OBJECT_HEADER(param.Header, 
        NDIS_OBJECT_TYPE_DEFAULT,
        DOT11_TKIPMIC_FAILURE_PARAMETERS_REVISION_1,
        sizeof(DOT11_TKIPMIC_FAILURE_PARAMETERS)
        );

    param.bDefaultKeyFailure = IsDefaultKey;
    param.uKeyIndex = KeyId;
    NdisMoveMemory(param.PeerMac, Header->Address2, sizeof(DOT11_MAC_ADDRESS));

    Hvl11IndicateStatus(
        HwMac->VNic,
        NDIS_STATUS_DOT11_TKIPMIC_FAILURE,
        &param,
        sizeof(DOT11_TKIPMIC_FAILURE_PARAMETERS)
        );
}


VOID
HwIndicatePhyPowerState(
    _In_  PHW                     Hw,
    _In_  ULONG                   PhyId
    )
{
    PHW_MAC_CONTEXT             currentMacContext;
    ULONG                       i;

    DOT11_PHY_STATE_PARAMETERS  phyStateParams;
    
    NdisZeroMemory(&phyStateParams, sizeof(DOT11_PHY_STATE_PARAMETERS));
    
    //
    // Fill in object header
    //
    phyStateParams.Header.Type = NDIS_OBJECT_TYPE_DEFAULT;
    phyStateParams.Header.Revision = DOT11_PHY_STATE_PARAMETERS_REVISION_1;
    phyStateParams.Header.Size = sizeof(DOT11_PHY_STATE_PARAMETERS);

    //
    // Phy state buffer
    //
    phyStateParams.uPhyId = PhyId;
    phyStateParams.bHardwarePhyState = HwQueryHardwarePhyState(Hw, PhyId);
    phyStateParams.bSoftwarePhyState = HwQuerySoftwarePhyState(Hw, PhyId);
   
    //
    // Indicate the phy power state on all the valid MAC contexts
    //
    for (i = HW_DEFAULT_PORT_MAC_INDEX ; i < HW_MAX_NUMBER_OF_MAC; i++)        
    {
        currentMacContext = &Hw->MacContext[i];
        if (HW_MAC_CONTEXT_IS_VALID(currentMacContext))
        {
            Hvl11IndicateStatus(
                currentMacContext->VNic,
                NDIS_STATUS_DOT11_PHY_STATE_CHANGED,
                &phyStateParams,
                sizeof(DOT11_PHY_STATE_PARAMETERS)
                );
        }
    }
}





ULONG DSSS_Freq_Channel[] = {
    0,
    2412,
    2417,
    2422,
    2427,
    2432,
    2437,
    2442,
    2447,
    2452,
    2457,
    2462,
    2467,
    2472,
    2484
};

ULONG
HwChannelToFrequency(
    _In_  UCHAR               Channel
    )
{
    if (Channel >= 15)
    {
        // 11a channel
        return 5000 + 5 * Channel;
    }
    else
    {
        return DSSS_Freq_Channel[Channel];
    }
}

NDIS_STATUS
HwTranslateChannelFreqToLogicalID(
    _In_reads_(NumChannels)  PULONG  ChannelFreqList,
    _In_  ULONG                   NumChannels,
    _Out_writes_(NumChannels) PULONG  LogicalChannelList
    )
{
    ULONG i, freqDiffFromBase;
    NDIS_STATUS     ndisStatus = NDIS_STATUS_SUCCESS;

    NdisZeroMemory(LogicalChannelList, NumChannels * sizeof(ULONG));
    
    for (i=0; i<NumChannels; i++)
    {
        //
        // Validate that frequences are correct
        //
        if (ChannelFreqList[i] == 2484)
        {
            LogicalChannelList[i] = 14;
            continue;
        }
        else if (ChannelFreqList[i] >= 2412 && ChannelFreqList[i] <= 2472)
        {
            freqDiffFromBase = ChannelFreqList[i] - 2412 + 5;
        }
        else if (ChannelFreqList[i] >= 5000 && ChannelFreqList[i] <= 6000)
        {
            freqDiffFromBase = ChannelFreqList[i] - 2412;
        }
        else
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        if (freqDiffFromBase % 5 != 0)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        LogicalChannelList[i] = freqDiffFromBase / 5;
    }

    return ndisStatus;
}
