/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_rate.c

Abstract:
    Implements basic rate adaptation logic for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"
#include "hw_rate.h"
#include "vnic_intf.h"

#if DOT11_TRACE_ENABLED
#include "hw_rate.tmh"
#endif




USHORT
HwSelectTXDataRate(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PDOT11_RATE_SET         RemoteRateSet,
    _In_  ULONG                   LinkQuality
    )
{
    PDOT11_RATE_SET             operationalRateSet;
    PDOT11_RATE_SET             activeRateSet;
    ULONG                       indexLocalRates;
    ULONG                       indexRemoteRates;
    USHORT                      maximumRate;
    OP_RATE_CHANGE_NOTIFICATION rateNotification;

    // The active rate set structure needs to be protected
    HW_ACQUIRE_HARDWARE_LOCK(HwMac->Hw, FALSE);
    
    //
    // Select the best rate supported by both AP and us. 
    //
    operationalRateSet = &(HwMac->PhyContext[HwMac->OperatingPhyId].OperationalRateSet);
    activeRateSet = &(HwMac->PhyContext[HwMac->OperatingPhyId].ActiveRateSet);

    activeRateSet->uRateSetLength = 0;
    
    //
    // We merge the rates from the AP and the client to create the 
    // currently active data rates table
    //
    for (indexLocalRates = 0; indexLocalRates < operationalRateSet->uRateSetLength; indexLocalRates++) 
    {
        for (indexRemoteRates = 0; indexRemoteRates < RemoteRateSet->uRateSetLength; indexRemoteRates++)
        {
            if (operationalRateSet->ucRateSet[indexLocalRates] == (RemoteRateSet->ucRateSet[indexRemoteRates] & 0x7f))
            {

                activeRateSet->ucRateSet[activeRateSet->uRateSetLength] = 
                    operationalRateSet->ucRateSet[indexLocalRates];
                activeRateSet->uRateSetLength++;
                break;
            }
        }
    }

    //
    // If we dont find any rates, we will stick with our management packet rate
    //
    if (activeRateSet->uRateSetLength == 0)
    {
        activeRateSet->ucRateSet[0] = (UCHAR)HalGetBeaconRate(HwMac->Hw->Hal, HwMac->OperatingPhyId);
        activeRateSet->uRateSetLength = 1;
    }
    else if (activeRateSet->uRateSetLength > 1)
    {
        // bubble sort data rates in ascending order
        INT     i, j;
        UCHAR   temp;
        for (i = activeRateSet->uRateSetLength - 1; i >= 0; i--)
        {
            for (j = 1; j <= i; j++)
            {
                if (activeRateSet->ucRateSet[j - 1] > 
                    activeRateSet->ucRateSet[j])
                {
                    temp = activeRateSet->ucRateSet[j - 1];
                    activeRateSet->ucRateSet[j - 1] = 
                        activeRateSet->ucRateSet[j];
                    activeRateSet->ucRateSet[j] = temp;
                }
            }
        }
    }

    //
    // Now that the active rate set is populated and updated, select
    // the best rate
    //
    
    //
    // Determine what the maximum TX rate should be
    //
    maximumRate = HW11_MAX_DATA_RATE;  // Start with max supported by HW
    if (LinkQuality < HW_LOW_RATE_LINK_QUALITY_THRESHOLD)
    {
        //
        // The link quality is low, we will go for the lower start rate instead of 
        // the max supported. The hardware does rate fallback, so we pick
        // something that would cause the least number of retrys
        //
        maximumRate = HW_LOW_RATE_MAX_DATA_RATE;
    }

    // 
    // Check if this is greater than max permitted value read from the registry
    //
    if (maximumRate > HwMac->Hw->RegInfo.MaximumTxRate)
    {
        maximumRate = (USHORT)HwMac->Hw->RegInfo.MaximumTxRate;
    }

    //
    // Now find the best matching rate between us and the AP
    //
    for (indexLocalRates = activeRateSet->uRateSetLength - 1; 
         indexLocalRates > 0;
         indexLocalRates--) 
    {
        //
        // Check for a rate supported by both us and the AP that is below the max we prefer to use
        //
        if (activeRateSet->ucRateSet[indexLocalRates] <= maximumRate)
        {
            //
            // Found the rate we want to use
            //
            break;
        }
    }

    HwMac->DefaultTXDataRate = activeRateSet->ucRateSet[indexLocalRates];

    // Update our rate info table. In our current implementation this is not-per peer
    HwMac->RateInfo.CurrentTXDataRate = HwMac->DefaultTXDataRate;

    MpTrace(COMP_ASSOC, DBG_LOUD, ("TX Data rate: %d\n", HwMac->DefaultTXDataRate));

    
    HW_RELEASE_HARDWARE_LOCK(HwMac->Hw, FALSE);

    //
    // Indicate NDIS_STATUS_LINK_STATE to inform the OS about the new 
    // link speed
    //
    HwIndicateLinkSpeed(HwMac, HwMac->DefaultTXDataRate);    

    // Indicate change of link speed to the port so that it 
    // can take any necessary action
    rateNotification.Header.Type = NotificationOpRateChange;
    rateNotification.Header.Size = sizeof(OP_RATE_CHANGE_NOTIFICATION);
    rateNotification.Header.pSourceVNic = NULL;
    
    rateNotification.NewRate = HwMac->DefaultTXDataRate;
    rateNotification.OldRate = 0;
    
    rateNotification.LowestRate = FALSE;
    
    VNic11Notify(HwMac->VNic, &rateNotification);

    return (UCHAR)HwMac->DefaultTXDataRate;
}


USHORT
HwDetermineStartTxRate(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_PEER_NODE           PeerNode,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  PDOT11_MAC_HEADER       FragmentHeader
    )
{
    UNREFERENCED_PARAMETER(PeerNode);
    UNREFERENCED_PARAMETER(Msdu);

    // We currently use very simple logic for determining the TX rate
    if ((FragmentHeader->FrameControl.Type != DOT11_FRAME_TYPE_DATA) ||
        (DOT11_IS_MULTICAST(FragmentHeader->Address1)) ||
        (MP_TX_MSDU_IS_PRIVATE(Msdu->MpMsdu)))
    {
        // Send private packets, non-data and multicast/broadcast packets at
        // lowest rate
        return MacContext->DefaultTXMgmtRate;
    }
    else
    {
        // Use the current rate
        return MacContext->RateInfo.CurrentTXDataRate;
    }
}

VOID
HwUpdatePeerTxStatistics(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  HAL_TX_DESC_STATUS      DescStatus
    )
{
    PDOT11_MAC_HEADER           fragmentHeader;
    
    do
    {
        if (!HW_TEST_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_LINK_UP))
        {
            // Only update statistics when device is connected
            break;
        }

        fragmentHeader = MP_TX_MPDU_DATA((HW_TX_MSDU_MPDU_AT(Msdu, 0))->MpMpdu, sizeof(DOT11_MAC_HEADER));
        if (fragmentHeader == NULL)
        {
            break;
        }

        if (fragmentHeader->FrameControl.Type != DOT11_FRAME_TYPE_DATA)
        {
            // only update retry statistics for control, management, and unicast data packets
            break;
        }

        if (DOT11_IS_MULTICAST(fragmentHeader->Address1))
        {
            // Ignore multicast packets too. This wont happen for station mode, but can for AP
            break;
        }
        
        // We are not performing per peer rate adaptation. We are only maintaining overall statistics
        InterlockedExchangeAdd(&MacContext->RateInfo.TotalRetryCount, DescStatus.RetryCount);

        // This function is called for every packet sent
        InterlockedIncrement((LONG*)&MacContext->RateInfo.PacketsSentForTxRateCheck);
    } while (FALSE);

}

/*
    Tx rate adaptation algorithm:
    Preconditions to update tx data rate:
            1) device is in connect state; 
            2)device has sent at least 100 data packets

    retransmitPercentage = (toal retry) / (total packets sent)

    if retransmit percentage <= threshold for rate increase which default to 35%
        if not sending at the highest rate
            if previously came down from higher rate and wait count < wait required
                increase wait count and exit
            else
                if not previously came down from higher rate
                    reset wait required
                else leave wait required unchanged
                increase current tx rate to next higher rate, reset all other counters and exit

    if retransmit percentage >= threshold for roam which default to 350%
        if already send at the lowest rate
            try to roam to a better AP, reset all counters and exit
        else if there are at least fallback skip (which default to 3) lower data rates available
            reduce current tx rate to third lower data rate, reset all counters and exit
        else if current sending at second lowest rate
            set current tx rate to the lowest value, reset all counters and exit
        else fall through to following rate fallback handler

    if retransmit percentage >= threshold for rate fallback which default to 125%
    and not sending at the lowest rate
        if previously came up from lower rate
            double current wait required
        else reset wait required
        reduce current tx rate to next lower value, reset all other counters and exit
*/
VOID 
HwUpdateTxDataRate(
    _In_  PHW_MAC_CONTEXT         MacContext
    )
{
    PDOT11_RATE_SET             activeRateSet;
    PHW_RATE_INFO               rateInfo = &MacContext->RateInfo;     // We are not doing per peer rate adaptation
    ULONG                       rateIndex = 0;
    USHORT                      prevTxRate = 0;
    ULONG                       totalRetry = rateInfo->TotalRetryCount;
    ULONG                       totalSend = rateInfo->PacketsSentForTxRateCheck;
    ULONG                       retransmitPercentage = 0;
    OP_RATE_CHANGE_NOTIFICATION rateNotification;

    if (HW_TEST_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_CANNOT_SEND_FLAGS) ||
        !HW_TEST_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_LINK_UP))
    {
        // MAC context wont be sending to a peer. There isnt anything for us to do
        return;
    }

    // Check the send threshold
    if (totalSend < MacContext->Hw->RegInfo.MinPacketsSentForTxRateUpdate)
    {
        return;
    }

    // reset counters
    InterlockedExchange(&rateInfo->TotalRetryCount, 0);
    InterlockedExchange((LONG*)&rateInfo->PacketsSentForTxRateCheck, 0);

    // The active rate set structure needs to be protected
    HW_ACQUIRE_HARDWARE_LOCK(MacContext->Hw, FALSE);

    do
    {
        activeRateSet = &(MacContext->PhyContext[MacContext->OperatingPhyId].ActiveRateSet);
        for (rateIndex = 0; rateIndex < activeRateSet->uRateSetLength; rateIndex++)
        {
            if (rateInfo->CurrentTXDataRate == activeRateSet->ucRateSet[rateIndex])
            {
                prevTxRate = rateInfo->CurrentTXDataRate;
                break;
            }
        }    

        // prevTxRate equals 0 means rateInfo->CurrentTXDataRate 
        // is not in activeRateSet->ucRateSet list
        if (prevTxRate == 0)
        {   
            MpTrace(COMP_MISC, DBG_LOUD, ("Current TX data rate is not in active rate set\n"));
            break;
        }

        retransmitPercentage = totalRetry * 100 / totalSend;

        MpTrace(COMP_TESTING, DBG_TRACE, ("%s: retransmitPercentage=%d(%d/%d), waitCount=%d, "
            "waitRequired=%d, prevRate=%d\n", 
            __FUNCTION__, 
            retransmitPercentage, totalRetry, totalSend, 
            rateInfo->TxRateIncreaseWaitCount, 
            rateInfo->TxRateIncreaseWaitRequired, 
            rateInfo->PrevTxDataRate));

        if (retransmitPercentage <= MacContext->Hw->RegInfo.TxFailureThresholdForRateIncrease)
        {
            // Consider going up
            
            if (rateIndex < activeRateSet->uRateSetLength - 1)
            {
                if ((rateInfo->PrevTxDataRate == activeRateSet->ucRateSet[rateIndex + 1]) &&
                    (rateInfo->TxRateIncreaseWaitCount + 1 < rateInfo->TxRateIncreaseWaitRequired))
                {
                    // I just came down from the rate above me, i dont want to go up yet due to WaitRequired
                    rateInfo->TxRateIncreaseWaitCount++;
                    
                    MpTrace(COMP_TESTING, DBG_TRACE, ("%s: WAIT before increasing Tx rate. retransmitPercentage=%d(%d/%d), "
                        "waitCount=%d, waitRequired=%d, prevRate=%d\n",
                        __FUNCTION__,
                        retransmitPercentage, totalRetry, totalSend, rateInfo->TxRateIncreaseWaitCount, 
                        rateInfo->TxRateIncreaseWaitRequired, rateInfo->PrevTxDataRate));
                }
                else
                {
                    // 1. I came down rate above me and WaitCount >= WaitRequired
                    // 2. I came up from rate below me, no need to wait for additional time
                    if (rateInfo->PrevTxDataRate != activeRateSet->ucRateSet[rateIndex + 1])
                    {
                        // Case 2 above
                        rateInfo->TxRateIncreaseWaitRequired = 1;
                    }
                    else
                    {
                        // Case 1 above
                        /** don't reset TxRateIncreaseWaitRequired as we need to double */
                        /** the value if fallback to the current rate again */
                    }
                    rateInfo->PrevTxDataRate = rateInfo->CurrentTXDataRate;
                    rateInfo->TxRateIncreaseWaitCount = 0;

                    MpTrace(COMP_TESTING, DBG_TRACE, ("%s: increasing Tx data rate from %d to %d. "
                        "retransmitPercentage = %d(%d/%d), waitCount=%d, waitRequired=%d, prevRate=%d\n",
                        __FUNCTION__, 
                        rateInfo->CurrentTXDataRate, activeRateSet->ucRateSet[rateIndex + 1],
                        retransmitPercentage, totalRetry, totalSend,
                        rateInfo->TxRateIncreaseWaitCount, 
                        rateInfo->TxRateIncreaseWaitRequired, 
                        rateInfo->PrevTxDataRate));

                    rateInfo->CurrentTXDataRate = activeRateSet->ucRateSet[rateIndex + 1];
                }
            }
            break;
        }

        if (retransmitPercentage >= MacContext->Hw->RegInfo.TxFailureThresholdForRoam)
        {
            ULONG   rateSkipLevel = MacContext->Hw->RegInfo.TxDataRateFallbackSkipLevel;
            
            // Really large retry count, consider roaming
            
            if (rateIndex == 0)
            {
                // I am sending at the lowest rate. Either roam or disconnect.
                MpTrace(COMP_TESTING, DBG_TRACE, ("%s: too many retransmit happened while "
                    "transmitting at the lowest data rate. Attempting to roam. "
                    "retransmitPercentage=%d(%d/%d), waitCount=%d, waitRequired=%d, prevRate=%d", 
                    __FUNCTION__,
                    retransmitPercentage, totalRetry, totalSend, 
                    rateInfo->TxRateIncreaseWaitCount,
                    rateInfo->TxRateIncreaseWaitRequired, 
                    rateInfo->PrevTxDataRate));

                // Our packets arent going through, jump up to using best data rate &
                // hopefully our packets will go through
                rateInfo->CurrentTXDataRate = activeRateSet->ucRateSet[activeRateSet->uRateSetLength - 1];

                // reset counters
                rateInfo->PrevTxDataRate = 0;
                rateInfo->TxRateIncreaseWaitRequired = 1;
                rateInfo->TxRateIncreaseWaitCount = 0;
                break;
            }
            else if (rateIndex >= rateSkipLevel)
            {
                // try to go down three rates
                MpTrace(COMP_TESTING, DBG_TRACE, ("%s: too many retransmit happened. Reducing Tx rate from %d to %d.\n",
                        __FUNCTION__, 
                        rateInfo->CurrentTXDataRate,
                        activeRateSet->ucRateSet[rateIndex - rateSkipLevel]));

                rateInfo->CurrentTXDataRate = activeRateSet->ucRateSet[rateIndex - rateSkipLevel];
                rateInfo->PrevTxDataRate = 0;
                rateInfo->TxRateIncreaseWaitRequired = 1;
                rateInfo->TxRateIncreaseWaitCount = 0;
                break;
            }
            else if (rateIndex != 1)
            {
                // set tx rate to the lowest rate
                MpTrace(COMP_TESTING, DBG_TRACE, ("%s: too many retransmit happened. Reducing Tx rate from "
                        "%d to lowest %d.\n",
                        __FUNCTION__, 
                        rateInfo->CurrentTXDataRate,
                        activeRateSet->ucRateSet[0]));

                rateInfo->CurrentTXDataRate = activeRateSet->ucRateSet[0];
                rateInfo->PrevTxDataRate = 0;
                rateInfo->TxRateIncreaseWaitRequired = 1;
                rateInfo->TxRateIncreaseWaitCount = 0;
                break;
            }
            else
            {
                // rateIndex equals to 1. this is the same as lowering tx rate by 1 level
                // which is what rate fallback handler does. fall through.
            }
        }

        if ((retransmitPercentage >= MacContext->Hw->RegInfo.TxFailureThresholdForRateFallback) && (rateIndex > 0))
        {
            // consider going down. no need to wait for additional time
            
            if (rateInfo->PrevTxDataRate == activeRateSet->ucRateSet[rateIndex - 1])
            {
                rateInfo->TxRateIncreaseWaitRequired *= 2;
            }
            else
            {
                rateInfo->TxRateIncreaseWaitRequired = 1;
            }
            rateInfo->PrevTxDataRate = rateInfo->CurrentTXDataRate;
            rateInfo->TxRateIncreaseWaitCount = 0;
            
            MpTrace(COMP_TESTING, DBG_TRACE, ("%s: reducing Tx data rate from %d to %d. "
                "retransmitPercentage=%d(%d/%d), waitCount=%d, waitRequired=%d, prevRate=%d\n",
                __FUNCTION__, 
                rateInfo->CurrentTXDataRate, 
                activeRateSet->ucRateSet[rateIndex - 1],
                retransmitPercentage, totalRetry, totalSend, 
                rateInfo->TxRateIncreaseWaitCount, 
                rateInfo->TxRateIncreaseWaitRequired,
                rateInfo->PrevTxDataRate));

            rateInfo->CurrentTXDataRate = activeRateSet->ucRateSet[rateIndex - 1];
            break;
        }
    } while (FALSE);

    HW_RELEASE_HARDWARE_LOCK(MacContext->Hw, FALSE);

    if (prevTxRate != rateInfo->CurrentTXDataRate)
    {
        // Indicate new link speed
        HwIndicateLinkSpeed(MacContext, rateInfo->CurrentTXDataRate);    

        // Indicate change of link speed to the port so that it 
        // can take any necessary action
        rateNotification.Header.Type = NotificationOpRateChange;
        rateNotification.Header.Size = sizeof(OP_RATE_CHANGE_NOTIFICATION);
        rateNotification.Header.pSourceVNic = NULL;

        rateNotification.NewRate = rateInfo->CurrentTXDataRate;
        rateNotification.OldRate = prevTxRate;

        if (rateIndex == 0)
            rateNotification.LowestRate = TRUE;
        else
            rateNotification.LowestRate = FALSE;

        VNic11Notify(MacContext->VNic, &rateNotification);
    }
}


VOID
HwFillTxRateTable(
    _In_  PHW_MAC_CONTEXT         HwMac,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  USHORT                  InitialTxRate
    )
{
    SHORT                       i;
    USHORT                      currentRate, rateTableIndex = ((USHORT)-1);
    PDOT11_RATE_SET             rateTable = NULL;

    // Initialize with 0, except for the first entry. This will be returned if we 
    // cannot fill the table properly
    for (i = 0; i < HW_TX_RATE_TABLE_SIZE; i++)
        Msdu->TxRateTable[i] = 0;            
    Msdu->TxRateTable[0] = InitialTxRate;

    HW_ACQUIRE_HARDWARE_LOCK_WITH_QUEUE_LOCK_HELD(HwMac->Hw);

    // If we are post association, we will be using the active rate set, else
    // we will be using the operational rate set
    if ((HwMac->PhyContext[HwMac->OperatingPhyId].ActiveRateSet.uRateSetLength == 0) ||
        (!HW_TEST_MAC_CONTEXT_STATUS(HwMac, HW_MAC_CONTEXT_LINK_UP)))
    {
        // Active Rate table not yet populated, use the operational rate table
        if (HwMac->PhyContext[HwMac->OperatingPhyId].OperationalRateSet.uRateSetLength == 0)
        {
            // Both are NULL
            HW_RELEASE_HARDWARE_LOCK_WITH_QUEUE_LOCK_HELD(HwMac->Hw);
            return;
        }
        else
        {
            // Operational rate table
            rateTable = &HwMac->PhyContext[HwMac->OperatingPhyId].OperationalRateSet;
        }
    }
    else
    {   
        // Connected with a valid active rate table. We will use that
        rateTable = &HwMac->PhyContext[HwMac->OperatingPhyId].ActiveRateSet;
    }
    
    // Find the rate index in our rate table (going backwards)
    for (i = ((SHORT)rateTable->uRateSetLength - 1); i >= 0 ; i--)
    {
        if (rateTable->ucRateSet[i] == InitialTxRate)
        {
            rateTableIndex = i;
            break;
        }
    }

    if (rateTableIndex == ((USHORT)-1))
    {
        //
        // We didnt find the initial rate in our table. Only first entry will be used, other are 0 & 
        // HAL will handle it appropriately
        //
        HW_RELEASE_HARDWARE_LOCK_WITH_QUEUE_LOCK_HELD(HwMac->Hw);
        MpTrace(COMP_SEND, DBG_LOUD, ("Unable to find current rate %d in active rates table\n", InitialTxRate));
        return;
    }

    // Fill the table with the rates we want the HAL to use. First entry contains the start rate
    // and following contain the retry rates. If we reach the bottom of our table, we reuse
    // the lowest rate
    currentRate = InitialTxRate;
    for (i = 1; i < HW_TX_RATE_TABLE_SIZE; i++)
    {
        if (rateTableIndex > 0)
        {
            // Next lower rate from the table
            rateTableIndex--;
            currentRate = rateTable->ucRateSet[rateTableIndex];    
        }
        
        Msdu->TxRateTable[i] = currentRate;
    }
    HW_RELEASE_HARDWARE_LOCK_WITH_QUEUE_LOCK_HELD(HwMac->Hw);
    
    return;
}

