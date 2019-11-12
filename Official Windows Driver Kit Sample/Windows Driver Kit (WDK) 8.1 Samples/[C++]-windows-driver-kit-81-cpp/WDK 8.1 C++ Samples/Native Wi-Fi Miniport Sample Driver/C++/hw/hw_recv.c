/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_recv.c

Abstract:
    Implements the receive functionality for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"
#include "hw_recv.h"
#include "hw_crypto.h"
#include "hw_context.h"
#include "hw_mac.h"

#if DOT11_TRACE_ENABLED
#include "hw_recv.tmh"
#endif

NDIS_STATUS
Hw11InitializeReceiveEngine(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     descAllocated = FALSE;
    ULONG                       i;
    PHW_RX_MPDU                 mpdu = NULL;

    *ErrorCode = NDIS_STATUS_SUCCESS;
    *ErrorValue = 0;

    do
    {
        NdisInitializeListHead(&Hw->RxInfo.AvailableMPDUList);
        NdisInitializeListHead(&Hw->RxInfo.UnusedMPDUList);

        // The lookaside list for the Rx MPDUs
        NdisInitializeNPagedLookasideList(&Hw->RxInfo.RxFragmentLookaside,
            NULL,
            NULL,
            0,
            sizeof(HW_RX_MPDU),
            HW_MEMORY_TAG,
            0);

        // Lookaside list for the RX MSDUs
        NdisInitializeNPagedLookasideList(&Hw->RxInfo.RxPacketLookaside,
            NULL,
            NULL,
            0,
            sizeof(HW_RX_MSDU),
            HW_MEMORY_TAG,
            0);

        // Allocate the HAL Rx descriptors
        ndisStatus = HalAllocateRxDescs(Hw->Hal, Hw->RegInfo.NumRXBuffers);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_SEND, DBG_SERIOUS,  ("Failed to allocate the HAL RX descriptors. Status = 0x%08x\n", 
                ndisStatus
                ));
            *ErrorCode = NDIS_ERROR_CODE_OUT_OF_RESOURCES;                
            break;
        }
        descAllocated = TRUE;

        HalResetRxDescs(Hw->Hal);

        // Now we preallocate a whole bunch of RX MSDUs and place them in our list
        for (i = 0; i < Hw->RegInfo.NumRXBuffers; i++)
        {
            // Allocate an MPDU structure
            mpdu = NdisAllocateFromNPagedLookasideList(&Hw->RxInfo.RxFragmentLookaside);
            if (mpdu == NULL)
            {
                MpTrace(COMP_SEND, DBG_NORMAL, ("Unable to allocate HW_RX_MPDU %d\n", i));
                ndisStatus = NDIS_STATUS_RESOURCES;
                *ErrorCode = NDIS_ERROR_CODE_OUT_OF_RESOURCES;                
                break;
            }
            // Initialize the data structure
            HW_INITIALIZE_RX_MPDU(mpdu, Hw);

            //
            // Allocate the shared memory buffer for this MPDU            
            // This implementation is wasting memory. We can be
            // more efficient by allocating shared memory in larger
            // chunks and managing them ourselves
            //
            NdisMAllocateSharedMemory(Hw->MiniportAdapterHandle,
                MAX_TX_RX_PACKET_SIZE,
                FALSE,
                (void **)&mpdu->BufferVa,
                &mpdu->BufferPa
                );
            if ((!mpdu->BufferVa) || (mpdu->BufferPa.QuadPart == 0))
            {
                ndisStatus=NDIS_STATUS_RESOURCES;
                *ErrorCode = NDIS_ERROR_CODE_OUT_OF_RESOURCES;
                MpTrace(COMP_SEND, DBG_SERIOUS, ("Allocation of RX buffer %d failed\n", i));
                NdisFreeToNPagedLookasideList(&Hw->RxInfo.RxFragmentLookaside, mpdu);
                break;
            }

            HW_INCREMENT_TOTAL_RX_MPDU_ALLOCATED(Hw);
            
            // Return this to the HW
            HwSubmitRxMPDU(Hw, mpdu);

        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // If we have managed to allocate atleast the minimum number of RX MPDU's we
            // are OK with failure
            if (Hw->RxInfo.NumMPDUAllocated >= (LONG)Hw->RegInfo.NumRXBuffersLowerBase)
            {
                // This is fine, we work only with these many MPDUs
                MpTrace(COMP_SEND, DBG_SERIOUS, ("Able to allocate only %d HW_RX_MPDUs\n", Hw->RxInfo.NumMPDUAllocated));
                ndisStatus = NDIS_STATUS_SUCCESS;
            }
            else
            {
                MpTrace(COMP_SEND, DBG_SERIOUS, ("Unable to allocate minimum required number of HW_RX_MPDU\n"));
                break;
            }
        }        

#if DBG
        // Initialize some debugging state
        Hw->RxInfo.Debug_BreakOnReceiveCount = 0;
        Hw->RxInfo.Debug_BreakOnReceiveType = DOT11_FRAME_TYPE_DATA;
        Hw->RxInfo.Debug_BreakOnReceiveSubType = DOT11_DATA_SUBTYPE_DATA;
        Hw->RxInfo.Debug_BreakOnReceiveMatchSource = FALSE;
        NdisMoveMemory(Hw->RxInfo.Debug_BreakOnReceiveDestinationAddress, Hw->MacState.MacAddress, DOT11_ADDRESS_SIZE);
        NdisMoveMemory(Hw->RxInfo.Debug_BreakOnReceiveSourceAddress, Hw->MacState.MacAddress, DOT11_ADDRESS_SIZE);

#endif
        
    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (descAllocated)
            Hw11TerminateReceiveEngine(Hw);
    }


    return ndisStatus;
}

VOID
Hw11TerminateReceiveEngine(
    _In_  PHW                     Hw
    )
{
    PHW_RX_MPDU                 mpdu = NULL;

    MPASSERT((Hw->RxInfo.NumMPDUAvailable + Hw->RxInfo.NumMPDUUnused) == (Hw->RxInfo.NumMPDUAllocated));
    
    // 
    // Merge unused queue into available queue so we only need one resource free code
    //
    while (!IsListEmpty(&Hw->RxInfo.UnusedMPDUList))
    {
        mpdu = (PHW_RX_MPDU)RemoveHeadList(&Hw->RxInfo.UnusedMPDUList);
        InsertTailList(&Hw->RxInfo.AvailableMPDUList, &mpdu->ListEntry);

        HW_DECREMENT_UNUSED_RX_MPDU(Hw);
        HW_INCREMENT_AVAILABLE_RX_MPDU(Hw);
    }

    //
    // Free the whole list
    //
    while (Hw->RxInfo.NumMPDUAvailable > 0)
    {
        mpdu = (PHW_RX_MPDU)RemoveHeadList(&Hw->RxInfo.AvailableMPDUList);
        HW_DECREMENT_AVAILABLE_RX_MPDU(Hw);

        NdisMFreeSharedMemory(Hw->MiniportAdapterHandle,
            MAX_TX_RX_PACKET_SIZE,
            FALSE,
            mpdu->BufferVa,
            mpdu->BufferPa
            );

        NdisFreeToNPagedLookasideList(&Hw->RxInfo.RxFragmentLookaside, mpdu);        
    }
    
    HalReleaseRxDescs(Hw->Hal);

    NdisDeleteNPagedLookasideList(&Hw->RxInfo.RxFragmentLookaside);
    NdisDeleteNPagedLookasideList(&Hw->RxInfo.RxPacketLookaside);
}


__inline PHW_RX_MSDU
HwAllocateRxMSDU(
    _In_  PHW                     Hw
    )
{
    PHW_RX_MSDU                 msdu = NULL;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    
    do
    {
        // Allocate the MSDU structure. We use the lookaside list
        msdu = NdisAllocateFromNPagedLookasideList(&Hw->RxInfo.RxPacketLookaside);
        if (msdu == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            MpTrace(COMP_SEND, DBG_NORMAL, ("Unable to allocate HW_RX_MSDU\n"));
            break;
        }

        NdisZeroMemory(msdu, sizeof(HW_RX_MSDU));
        
        // Set the appropriate pointers in the HW & MP MSDU
        msdu->MpMsdu = &msdu->PrivateMpMsdu;
        msdu->MpMsdu->HwMsdu = msdu;

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (msdu)
        {        
            NdisFreeToNPagedLookasideList(&Hw->RxInfo.RxFragmentLookaside, msdu);
            msdu = NULL;
        }
    }

    return msdu;
}

__inline VOID
HwFreeRxMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu
    )
{
    NdisFreeToNPagedLookasideList(&Hw->RxInfo.RxFragmentLookaside, Msdu);
}

BOOLEAN
HwCheckPhyParameters(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    //
    // Save the current PHY parameters into this packet
    //
    Mpdu->PhyId = Hw->PhyState.OperatingPhyId;
    Mpdu->Timestamp = Mpdu->DescStatus.TimeStamp;
    Mpdu->Rate = Mpdu->DescStatus.Rate;
    Mpdu->RSSI = HalGetRSSI(Hw->Hal, &Mpdu->DescStatus, (UCHAR *)Mpdu->DataStart); 
    Mpdu->LinkQuality = (UCHAR)HalGetCalibratedRSSI(Hw->Hal, &Mpdu->DescStatus, (UCHAR *)Mpdu->DataStart); 
    Mpdu->Channel = HalGetPhyMIB(Hw->Hal, Mpdu->PhyId)->Channel;

    //
    // Update PHY statistics
    //
    if (Mpdu->DescStatus.CRCError)
    {
        // FCS error count
        Hw->Stats.PhyCounters[Mpdu->PhyId].ullFCSErrorCount++;
        return FALSE;
    }

    // Hardware specific errors
    if (Mpdu->DescStatus.FifoOverflow || Mpdu->DescStatus.HardwareError)
    {
        // Incomplete packet
        Hw->Stats.NumRxError++;
        return FALSE;
    }

    // This is an indication of a bad receive
    if (Mpdu->DescStatus.Length < 4)
    {
        Hw->Stats.NumRxError++;
        return FALSE;
    }

    // Fragment is good from the PHY point
    Hw->Stats.PhyCounters[Mpdu->PhyId].ullReceivedFragmentCount++;
    MPASSERT(Hw->PhyState.Debug_SoftwareRadioOff == FALSE);
    
    return TRUE;
}

// Returns TRUE if this fragment is duplicate and should be dropped. FALSE otherwise
BOOLEAN
HwMPDUIsDuplicate(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu,
    _In_  BOOLEAN                 IsGoodPacket
    )
{
    PDOT11_MAC_HEADER           macHeader = (PDOT11_MAC_HEADER)Mpdu->DataStart;

    //
    // Reject packets which are not correct length
    //
    switch(macHeader->FrameControl.Type)
    {
    case DOT11_FRAME_TYPE_MANAGEMENT:
    case DOT11_FRAME_TYPE_DATA:
        if (Mpdu->DataLength < sizeof(DOT11_MGMT_DATA_MAC_HEADER))
            return TRUE;    // Drop the packet

        // We dont cache broadcast/multicast packet information
        if (Mpdu->MulticastDestination)
            return FALSE;
            
        //
        // Additional packet filtering - duplication detection
        //
        if (HwCheckForDuplicateMPDU(Hw, Mpdu->DataStart, IsGoodPacket))
        {
            // Update duplicate counter
            Hw->Stats.PhyCounters[Mpdu->PhyId].ullFrameDuplicateCount++;        
            return TRUE;    // Drop the packet
        }

        break;
        
    case DOT11_FRAME_TYPE_CONTROL:    
        break;
        
    default:
        //
        // Reserved packet are filtered out
        //
        return TRUE;    // Drop the packet
    }

    // Not duplicate
    return FALSE;
}



// Called for Data and Management packets only
BOOLEAN
HwCheckForDuplicateMPDU(
    _In_  PHW                     Hw,
    _In_reads_bytes_(sizeof(DOT11_MGMT_DATA_MAC_HEADER))  PUCHAR                  PacketBuffer,
    _In_  BOOLEAN                 IsGoodPacket
    )
{
    UCHAR                               index;
    PDOT11_MGMT_DATA_MAC_HEADER         packetHeader = (PDOT11_MGMT_DATA_MAC_HEADER)PacketBuffer;
    UCHAR                               startIndex, endIndex;

    //
    // For duplicate detection, we compare the <Address 2, sequence-number, fragment-number> tuple to
    // the last received value. If all match, this is a retransmitted packet. However we 
    // determine if we should drop this or not based on whether we have indicated a good MPDU
    // before or not. If this is a bad version of this MPDU (CRC error or something), we drop
    // it. If this is a good one and we have already received a good version of this MPDU before
    // (no-CRC error, etc) we drop this one. Else we indicate this up (because all previous duplicates
    // were corrupted packets)
    //

    //
    // A bunch of probe responses come in during scans, etc. This could cause us
    // to indicate up retry data packets. We handle this by separating cached entries for data
    // packets from other packet types
    //
    if (packetHeader->FrameControl.Type == DOT11_FRAME_TYPE_DATA)
    {
        startIndex = 0;
        endIndex = HW_DUPLICATE_DETECTION_CACHE_LENGTH;
    }
    else
    {
        startIndex = HW_DUPLICATE_DETECTION_CACHE_LENGTH;
        endIndex = 2 * HW_DUPLICATE_DETECTION_CACHE_LENGTH;
    }    

    //
    // The duplicate's cache is indexed using addresses
    //
    for (index = startIndex; index < endIndex; index++)
    {
        if (MP_COMPARE_MAC_ADDRESS(Hw->RxInfo.DupePacketCache[index].Address2, packetHeader->Address2))
        {
            if (packetHeader->FrameControl.Retry)
            {
                //
                // This is a retry if same sequence control
                //
                if (Hw->RxInfo.DupePacketCache[index].SequenceControl == packetHeader->SequenceControl.usValue)
                {
                    // This is a retransmission. Check if we should drop it
                    if (!IsGoodPacket)
                        return TRUE;        // Duplicate & it is a bad MPDU, drop it

                    if (Hw->RxInfo.DupePacketCache[index].ReceivedGoodMPDU)
                        return TRUE;        // Duplicate & we have already received a good MPDU, drop this

                    Hw->RxInfo.DupePacketCache[index].ReceivedGoodMPDU = TRUE;
                    return FALSE;           // Duplicate, but havent received a good one before, dont drop
                }
            }

            //
            // Save latest sequence number & whether we have received a good or bad MPDU
            //
            Hw->RxInfo.DupePacketCache[index].SequenceControl = packetHeader->SequenceControl.usValue;
            Hw->RxInfo.DupePacketCache[index].ReceivedGoodMPDU = IsGoodPacket;

            return FALSE;
        }
    }

    //
    // Store the tuple information from this packet
    //

    //
    // This address is not cached. We just add it to the next index in the cache
    //
    if (packetHeader->FrameControl.Type == DOT11_FRAME_TYPE_DATA)
    {
        index = ((Hw->RxInfo.NextDupeCacheIndexData + 1) % HW_DUPLICATE_DETECTION_CACHE_LENGTH);
        Hw->RxInfo.NextDupeCacheIndexData = index;
    }
    else
    {
        index = ((Hw->RxInfo.NextDupeCacheIndexOther + 1) % HW_DUPLICATE_DETECTION_CACHE_LENGTH)
                    + HW_DUPLICATE_DETECTION_CACHE_LENGTH;
        Hw->RxInfo.NextDupeCacheIndexOther = index;
    }
    
    NdisMoveMemory(Hw->RxInfo.DupePacketCache[index].Address2, 
        packetHeader->Address2, 
        sizeof(DOT11_MAC_ADDRESS)
        );
    Hw->RxInfo.DupePacketCache[index].SequenceControl = packetHeader->SequenceControl.usValue;
    Hw->RxInfo.DupePacketCache[index].ReceivedGoodMPDU = IsGoodPacket;

    return FALSE;
}

BOOLEAN
HwCheckMacParameters(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    PNICKEY                     key;
    PHW_MAC_CONTEXT             macContext = HW_MAC_CONTEXT_FOR_RX_STATISTICS(Hw, Mpdu);

    if (Mpdu->DescStatus.ICVError)
    {
        if (Mpdu->Encrypted)
        {
            //
            // For MAC counters, we shouldnt count duplicates, filter those 
            // counters out
            //
            if (HwMPDUIsDuplicate(Hw, Mpdu, FALSE))
            {
                // Drop and dont count
                return FALSE;
            }
        
            if (Mpdu->Key != NULL)
            {
                key = &Mpdu->Key->Key;
                if (key->Valid)
                {
                    //
                    // Decryption failed even though we think we have
                    // a valid key. This counts as an ICV error
                    //
                    switch (key->AlgoId)
                    {
                        case DOT11_CIPHER_ALGO_CCMP:
                            if (Mpdu->MulticastDestination)
                            {
                                macContext->MulticastCounters.ullCCMPDecryptErrors++;
                            }
                            else
                            {
                                macContext->UnicastCounters.ullCCMPDecryptErrors++;
                            }
                            break;
                            
                        case DOT11_CIPHER_ALGO_TKIP:
                            if (Mpdu->MulticastDestination)
                            {
                                macContext->MulticastCounters.ullTKIPICVErrorCount++;
                            }
                            else
                            {
                                macContext->UnicastCounters.ullTKIPICVErrorCount++;
                            }
                            break;
                        
                        case DOT11_CIPHER_ALGO_WEP104:
                        case DOT11_CIPHER_ALGO_WEP40:
                            if (Mpdu->MulticastDestination)
                            {
                                macContext->MulticastCounters.ullWEPICVErrorCount++;
                            }
                            else
                            {
                                macContext->UnicastCounters.ullWEPICVErrorCount++;
                            }
                            break;
                            
                        default:
                            break;
                    }
                }
                else
                {
                    //
                    // Decryption failed and the key we have is invalid. This happens 
                    // when the hardware uses a default key for decrypting a packet, but
                    // the default key was not set
                    //
                    if (Mpdu->MulticastDestination)
                    {
                        macContext->MulticastCounters.ullWEPUndecryptableCount++;
                    }
                    else
                    {
                        macContext->UnicastCounters.ullWEPUndecryptableCount++;
                    }
                }
            }
            else
            {
                //
                // No key found - Our hardware may still have done the decryption (using a bogus
                // default key id) but we would not have found the key it used
                //
                if (Mpdu->MulticastDestination)
                {
                    macContext->MulticastCounters.ullWEPUndecryptableCount++;
                }
                else
                {
                    macContext->UnicastCounters.ullWEPUndecryptableCount++;
                }
            }
            if (Mpdu->MulticastDestination)
            {
                macContext->MulticastCounters.ullDecryptFailureCount++;
            }
            else
            {
                macContext->UnicastCounters.ullDecryptFailureCount++;
            }
        }
        
        return FALSE;
    }

    if (!(Mpdu->DescStatus.FirstSegment && Mpdu->DescStatus.LastSegment))
    {
        if (Mpdu->MulticastDestination)
        {
            macContext->MulticastCounters.ullReceivedFailureFrameCount++;
        }
        else
        {
            macContext->UnicastCounters.ullReceivedFailureFrameCount++;
        }

        // Fragments spanning multiple descriptors are not supported
        return FALSE;
    }

    //
    // Drop any duplicate frames
    //
    return !HwMPDUIsDuplicate(Hw, Mpdu, TRUE);
}

BOOLEAN
HwIsReceiveAvailable(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    PHW_RX_MPDU                 mpdu = NULL;
    BOOLEAN                     recvAvailable = FALSE;

    //
    // The Available fragment list needs to be protected with a lock since it
    // gets accessed by the return code paths also
    //
    HW_RX_ACQUIRE_LOCK(Hw, DispatchLevel);
    if (!IsListEmpty(&Hw->RxInfo.AvailableMPDUList))
    {
        MPASSERT(Hw->RxInfo.NumMPDUAvailable > 0);

        // Get the next fragment that we expect to be filled by the hardware
        mpdu = (PHW_RX_MPDU)MP_PEEK_LIST_HEAD(&Hw->RxInfo.AvailableMPDUList);

        // Check with the HAL if this RX_MPDU is ready for receive indication
        if (HalIsRxDescReady(Hw->Hal, mpdu->DescIter))
        {
            recvAvailable = TRUE;
        }
    }
    else
    {
        //
        // There cannot be a receive frame, since hardware has no RX_MPDU free 
        // available to use.
        //
    }
    HW_RX_RELEASE_LOCK(Hw, DispatchLevel);

    return recvAvailable;
}

PHW_RX_MPDU
HwGetReceivedMPDU(
    _In_  PHW                     Hw
    )
{
    PHW_RX_MPDU                 mpdu = NULL, unusedMpdu;

    HW_RX_ACQUIRE_LOCK(Hw, TRUE);
    
    // One less descriptor available to the hardware. 
    HW_DECREMENT_AVAILABLE_RX_MPDU(Hw);
    
    mpdu = (PHW_RX_MPDU)RemoveHeadList(&Hw->RxInfo.AvailableMPDUList);

    // Copy the status of this receive into our local copy
    HalReserveRxDesc(Hw->Hal, mpdu->DescIter);
    HalGetRxStatus(Hw->Hal, mpdu->DescIter, &mpdu->DescStatus);

    //
    // This hardware RX_DESC is now free. If we have Unused MPDU, we can assign this
    // MPDU to such a frag so that we can immediately receive a frame that comes
    // along and not have to wait for a return to occur
    //
    while (Hw->RxInfo.NumMPDUAvailable < (LONG)Hw->RegInfo.NumRXBuffers)
    {
        // Nothing is available
        if (IsListEmpty(&Hw->RxInfo.UnusedMPDUList))
            break;
        
        unusedMpdu = (PHW_RX_MPDU)RemoveHeadList(&Hw->RxInfo.UnusedMPDUList);
        HW_DECREMENT_UNUSED_RX_MPDU(Hw);

        // Return the descriptor held by this MPDU to the hardware
        HwSubmitRxMPDU(Hw, unusedMpdu);
    }

    // This MPDU should be owned by the OS
    MPASSERT(mpdu->DescStatus.ReceiveFinished);
    
    // We no longer need the lock. We have pulled the MPDU out of the available list,
    // and can now process it individually
    HW_RX_RELEASE_LOCK(Hw, TRUE);

    // Fill some default information about the packet
    mpdu->DataLength = mpdu->DescStatus.Length;
    mpdu->DataStart = mpdu->BufferVa;
    

    return mpdu;
}


// Called at Dispatch
NDIS_STATUS
HwProcessReceivedMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    PDOT11_MAC_HEADER           macHeader = (PDOT11_MAC_HEADER)Mpdu->DataStart;
    PHW_MAC_CONTEXT             macContext = Mpdu->MacContext;
    PDOT11_MGMT_DATA_MAC_HEADER mgmtHeader;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(Hw);
    
    if (macContext == NULL)
    {
        // If we havent identified the single MAC this
        // packet should go to, we dont do much processing
#if 0
        //
        // During a scan operation, for beacon and probe response frames, check its timestamp. 
        // If it's not out-dated, save the channel number at which it was received. 
        // Otherwise, drop the frame.
        //
        if (Hw->ScanContext.ScanInProgress &&
            (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_MANAGEMENT) &&
            ((macHeader->FrameControl.Type == DOT11_MGMT_SUBTYPE_BEACON) || 
             (macHeader->FrameControl.Type == DOT11_MGMT_SUBTYPE_BEACON)))
        {            
            if (Mpdu->Timestamp < Hw->ScanContext.ChannelSwitchTime)
            {
                return NDIS_STATUS_NOT_ACCEPTED;
            }

            //
            // Update the channel number
            //
            Mpdu->Channel = Hw->ScanContext.CurrentChannel;
        }
#endif
        switch(macHeader->FrameControl.Type)
        {
            case DOT11_FRAME_TYPE_MANAGEMENT:
                {
                    switch (macHeader->FrameControl.Subtype)
                    {
                        case DOT11_MGMT_SUBTYPE_PROBE_REQUEST:
                            {
#ifndef PARTIAL_HAL
                                // Check if we are running in AP or IBSS mode.
                                // If yes & we are active, respond to this probe request 
                                macContext = Hw->MacState.BSSMac;
                                if ((Hw->MacState.BSSStarted) && 
                                    (macContext != NULL) &&
                                    (HW_MAC_CONTEXT_IS_ACTIVE(macContext)) &&
                                    (macContext->BSSStarted))
                                {
                                    // This MAC must be active
                                    ndisStatus = HwProcessProbeRequest(Hw, macContext, Mpdu);
                                    if (ndisStatus != NDIS_STATUS_SUCCESS)
                                    {
                                        MpTrace(COMP_MISC, DBG_NORMAL, ("Failed to respond to probe request\n"));
                                    }
                                }
#endif
                            }
                            break;
                    }
                }
        }
        return NDIS_STATUS_SUCCESS;
    }

    // Check if there is any operation that needs to be done on this MPDU
    switch(macHeader->FrameControl.Type)
    {
        case DOT11_FRAME_TYPE_MANAGEMENT:
            {
                mgmtHeader = (PDOT11_MGMT_DATA_MAC_HEADER)Mpdu->DataStart;
                switch (macHeader->FrameControl.Subtype)
                {
                    case DOT11_MGMT_SUBTYPE_PROBE_REQUEST:
                        {
#ifndef PARTIAL_HAL
                            // Check if we are running in AP or IBSS mode.
                            // If yes & we are active, respond                            
                            if ((macContext->BSSStarted) && 
                                (HW_MAC_CONTEXT_IS_ACTIVE(macContext)))
                            {
                                // This MAC must be active
                                ndisStatus = HwProcessProbeRequest(Hw, macContext, Mpdu);
                            }
#endif
                        }
                        break;
                        
                    case DOT11_MGMT_SUBTYPE_BEACON:
                        {
                            // Check if we are waiting for a join to this AP
                            // If yes, indicate join completion
                            if (macContext->JoinWaitForBeacon &&
                                MP_COMPARE_MAC_ADDRESS(mgmtHeader->Address3, macContext->DesiredBSSID) &&
                                HW_STOP_WAITING_FOR_JOIN(macContext))
                            {
                                //
                                // We have received the beacon that synchronises us with the BSS.
                                // We will complete the pending Join request
                                //
                                if (NdisCancelTimerObject(macContext->Timer_JoinTimeout) == TRUE)
                                {
                                    // Timer was cancelled, remove the async operation ref
                                    HW_DECREMENT_ACTIVE_OPERATION_REF(Hw);
                                }

                                // Inform the MAC context that the Join has completed
                                HwJoinBSSComplete(macContext, Mpdu, NDIS_STATUS_SUCCESS);
                            }
                            // TODO:
                            // Check if power save is enabled. If yes,
                            // check if there are buffered packets
                        }
                        break;

                    default:
                        break;
                }
            }
            break;

        case DOT11_FRAME_TYPE_DATA:
            {
                // TODO: check is something needs to be done for power save
                // If in AP mode, check peer table. If in STA mode
                // check for PS/More bit

                // Update rate adaptation table based on received packet
                // data rate. Plus note that if we do this based                
                // on MPDU, someone can spoof and cause our adaptation
                // table to get messed up
            }
            break;


        default:
            break;
    }

    return ndisStatus;
}


// Returns TRUE if we do software decryption (Even if decryption gave ICV errors, etc)
BOOLEAN
HwRxDecrypt(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    PHW_MAC_CONTEXT             currentMacContext;
    ULONG                       i, keyId;
    PDOT11_DATA_SHORT_HEADER    dataFragmentHeader;
    PHW_PEER_NODE               peerNode;
    NDIS_STATUS                 decryptStatus;
    PHW_KEY_ENTRY               decryptKey;

    // We only handle it for data packets
    dataFragmentHeader = (PDOT11_DATA_SHORT_HEADER)Mpdu->DataStart;
    if (dataFragmentHeader->FrameControl.Type != DOT11_FRAME_TYPE_DATA)
    {
        return FALSE;
    }

    // Information about synchronization of MacContext is given in HwFindDecryptionKey
    for (i = HW_DEFAULT_PORT_MAC_INDEX ; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        currentMacContext = &Hw->MacContext[i];
        if (!HW_MAC_CONTEXT_IS_VALID(currentMacContext) || !HW_MAC_CONTEXT_IS_ACTIVE(currentMacContext))
        {
            continue;
        }

        // The only case in which we do software decryption today is for
        // multicast packets for Adhoc WPA2
        if ((currentMacContext->BssType == dot11_BSS_type_independent) &&
            (currentMacContext->AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK) &&
            (dataFragmentHeader->FrameControl.FromDS == 0) &&
            (dataFragmentHeader->FrameControl.ToDS == 0) &&
            (Mpdu->MulticastDestination) &&
            (MP_COMPARE_MAC_ADDRESS(dataFragmentHeader->Address3, currentMacContext->DesiredBSSID)))
        {
            // Lets look for the peer MAC address
            peerNode = HwFindPeerNode(currentMacContext, dataFragmentHeader->Address2, FALSE);

            // Check if we can find a valid Key ID in the peer (to & from DS = 0, so its a short header)
            keyId = *((PUCHAR)Add2Ptr(dataFragmentHeader, sizeof(DOT11_DATA_SHORT_HEADER) + 3));
            keyId = (keyId >> 6);
            
            if (peerNode && peerNode->PrivateKeyTable[keyId].Key.Valid)
            {
                // Found the key to use for decryption
                decryptKey = &peerNode->PrivateKeyTable[keyId];

                // If this is CCMP, we would be doing software decryption
                if (decryptKey->Key.AlgoId == DOT11_CIPHER_ALGO_CCMP)
                {
                    decryptStatus = HwDecryptCCMP(decryptKey->hCNGKey,
                                        Mpdu->DataStart,
                                        Mpdu->DataLength
                                        );
                    // It is decrypted. If there was a decrypt failure, 
                    // we still continue with the ICV error flag set
                    Mpdu->DescStatus.Decrypted = 1;
                    if (decryptStatus != NDIS_STATUS_SUCCESS)
                        Mpdu->DescStatus.ICVError = 1;
                    
                    // We assume that if we found a valid matching key, we 
                    // are OK. This would break if two MACs connect to the same PEER and each
                    // of them has a unique key for the same key ID
                    Mpdu->Key = decryptKey;

                    return TRUE;
                }
            }
        }
    }

    return FALSE;
}

// Failure only for catastrophic issues like corrupted packet
NDIS_STATUS
HwProcessRxCipher(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    PDOT11_MAC_HEADER           macHeader;
    BOOLEAN                     softwareDecrypted = FALSE;
    PHW_KEY_ENTRY               decryptKey;
    ULONG                       headerSize, ivSize, overhead;
    PUCHAR                      encryptionIV;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    MPASSERT(Hw->MacState.SafeModeEnabled == FALSE);
    
    macHeader = (PDOT11_MAC_HEADER)Mpdu->DataStart;

    if (macHeader->FrameControl.WEP == 1)
    {
        headerSize = (macHeader->FrameControl.FromDS && macHeader->FrameControl.ToDS) ?
                     sizeof(DOT11_DATA_LONG_HEADER) : sizeof(DOT11_DATA_SHORT_HEADER);
                     
        //
        // If the MPDU is smaller than minimum encrypted data packet size,
        // then there is no point going ahead
        //
        if (Mpdu->DataLength < (headerSize + HW_ENCRYPTED_MPDU_MIN_OVERHEAD))
        {
            // Not enough data for MGMT/DATA packet + IV/ICV. Drop this packet
            // without further processing
            MpTrace(COMP_RECV, DBG_LOUD, ("Encrypted MPDU data length less than minimum required\n"));
            return NDIS_STATUS_INVALID_PACKET;
        }

        //
        // Check on whether we need to do software decryption
        //
        if (Mpdu->DescStatus.Decrypted == FALSE)
        {
            // Packet received is encrypted and the hardware has not decrypted it. 
            // Lets check if we should do software decryption        
            softwareDecrypted = HwRxDecrypt(Hw, Mpdu);

            // The above function changes the Decrypted flag, so dont merge if/else
            // below
        }

        if (Mpdu->DescStatus.Decrypted == TRUE)
        {
            // The hardware (or software) has decrypted this packet. It would not have removed 
            // the IV and ICV/MIC fields. We would manually remove it
            Mpdu->Encrypted = TRUE;

            // Next look for the key that was used for decrypting this packet
            if (softwareDecrypted)
            {
                decryptKey = Mpdu->Key;
            }
            else
            {
                decryptKey = HwFindDecryptionKey(Hw, Mpdu);    

                // Some hardware would claim to have decrypted the packet even if they
                // havent found the key. Check again for the case that we do software decryption

                if ((decryptKey == NULL) &&
                    (Hw->MacState.BssType == dot11_BSS_type_independent) &&
                    (Mpdu->MulticastDestination))
                {
                    // Check for software decryption
                    softwareDecrypted = HwRxDecrypt(Hw, Mpdu);
                    if (softwareDecrypted)
                        decryptKey = Mpdu->Key;
                }
            }

            // Clear the WEP flag
            macHeader->FrameControl.WEP = 0;

            if (decryptKey && decryptKey->Key.Valid)
            {
                //
                // Find the IVSize and total overhead due to encryption and CRC.
                //
                encryptionIV = Add2Ptr(macHeader, headerSize);
                switch (decryptKey->Key.AlgoId)
                {
                    case DOT11_CIPHER_ALGO_TKIP:

                        // 
                        // 8 bytes IV, 4 byte ICV. The per-MSDU MIC is removed after frame 
                        // reassembling and MIC failure checking.
                        //
                        ivSize = 8;
                        overhead = 12;
                        if (Mpdu->DataLength < (headerSize + overhead))
                        {
                            MpTrace(COMP_RECV, DBG_LOUD, ("Encrypted TKIP MPDU data length less than minimum required\n"));
                            ndisStatus = NDIS_STATUS_INVALID_PACKET;
                        }
                        else
                        {
                            Mpdu->FrameNumber = (((ULONGLONG)encryptionIV[0]) << 8) |
                                                 ((ULONGLONG)encryptionIV[2]) |
                                                 (((ULONGLONG)encryptionIV[4]) << 16) |
                                                 (((ULONGLONG)encryptionIV[5]) << 24) |
                                                 (((ULONGLONG)encryptionIV[6]) << 32) |
                                                 (((ULONGLONG)encryptionIV[7]) << 40);
                        }
                        break;

                    case DOT11_CIPHER_ALGO_CCMP:

                        // 
                        // 8 bytes IV, 8 bytes MIC. 
                        //
                        ivSize = 8;
                        overhead = 16;
                        if (Mpdu->DataLength < (headerSize + overhead))
                        {
                            MpTrace(COMP_RECV, DBG_LOUD, ("Encrypted CCMP MPDU data length less than minimum required\n"));
                            ndisStatus = NDIS_STATUS_INVALID_PACKET;
                        }
                        else
                        {
                            Mpdu->FrameNumber = ((ULONGLONG)encryptionIV[0]) |
                                                (((ULONGLONG)encryptionIV[1]) << 8) |
                                                (((ULONGLONG)encryptionIV[4]) << 16) |
                                                (((ULONGLONG)encryptionIV[5]) << 24) |
                                                (((ULONGLONG)encryptionIV[6]) << 32) |
                                                (((ULONGLONG)encryptionIV[7]) << 40);
                        }
                        break;

                    case DOT11_CIPHER_ALGO_WEP104:
                    case DOT11_CIPHER_ALGO_WEP40:

                        // 
                        // 4 bytes IV, 4 bytes ICV. 
                        //
                        ivSize = 4;
                        overhead = 8;
                        if (Mpdu->DataLength < (headerSize + overhead))
                        {
                            MpTrace(COMP_RECV, DBG_LOUD, ("Encrypted WEP MPDU data length less than minimum required\n"));
                            ndisStatus = NDIS_STATUS_INVALID_PACKET;
                        }                        
                        break;

                    default:
                        MPASSERT(FALSE);
                        ivSize = 0;
                        overhead = 0;
                        break;
                }

                if (ndisStatus == NDIS_STATUS_SUCCESS)
                {
                    //
                    // Copy the header. It is probably the most efficient way to get rid of the IV field.
                    // Note: cannot call NdisMoveMemory since source and destination overlaps.
                    //
                    RtlMoveMemory(((PCHAR)macHeader) + ivSize, macHeader, headerSize);
                }
                
                //
                // Set the data size and offset.
                //
                Mpdu->DataStart += ivSize;
                Mpdu->DataLength -= overhead;
                Mpdu->Encrypted = TRUE;
                Mpdu->Key = decryptKey;
            }
            else
            {
                // 
                // A key if used for decryption was wrong (or is no longer valid)
                //
                Mpdu->Key = decryptKey;
                Mpdu->DescStatus.ICVError = 1;
            }

        }
        else
        {
            // Neither could decrypt the packet
            Mpdu->Encrypted = TRUE;
        }
    }
    else
    {
        Mpdu->Encrypted = FALSE;
    }

    return ndisStatus;
}

PHW_KEY_ENTRY
HwFindDecryptionKey(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    NDIS_STATUS                 ndisStatus;
    PHW_MAC_CONTEXT             currentMacContext;
    ULONG                       i, keyIndex;
    PDOT11_MGMT_DATA_MAC_HEADER fragmentHeader;
    PHW_KEY_ENTRY               keyUsed = NULL;
    USHORT                      headerSize;
    BOOLEAN                     doBSSIDMatching = FALSE;
    PUCHAR                      bssid = NULL;

    // Should be only called for packets that have been decrypted
    MPASSERT(Mpdu->Key == NULL);    // Software decrypted packets shouldnt come here
    MPASSERT(Mpdu->Encrypted);
    MPASSERT(Mpdu->DescStatus.Decrypted);

    fragmentHeader = (PDOT11_MGMT_DATA_MAC_HEADER)Mpdu->DataStart;

    //
    // Synchronization Note: The receive handler function would have incremented 
    // the HW AsyncCount. This AsyncCount stops the Pause routines
    // from proceeding while the receive is running. Unless we are paused, 
    // the Mac context cannot be deleted. So we are OK here. 
    //
    // Note that a MAC context can become invalid after we have assigned a receive to it. 
    // We would still indicate the packet to that MAC
    //

    for (i = HW_DEFAULT_PORT_MAC_INDEX ; i < HW_MAX_NUMBER_OF_MAC; i++)
    {
        currentMacContext = &Hw->MacContext[i];
        doBSSIDMatching = FALSE;

        // We only look for valid MACs. A MAC context can be invalid 
        if (!HW_MAC_CONTEXT_IS_VALID(currentMacContext) || 
            HW_TEST_MAC_CONTEXT_STATUS(currentMacContext, HW_MAC_CONTEXT_CANNOT_RECEIVE_FLAGS))
        {
            continue;
        }

        if (!HW_MAC_CONTEXT_IS_ACTIVE(currentMacContext))
        {
            // The current MAC context is not active (or is being deactivated). But it maybe 
            // that the hardware has already decrypted the packet using a previously set key
            // If we find a matching key for this MAC, we only accept this key if we 
            // find the BSSID matches
            doBSSIDMatching = TRUE;
        }

        //
        // Lets try to use the keys to determine if an encrypted packet was decrypted
        // using one of the keys that this MAC set on the hardware
        //
        if (currentMacContext->UnicastCipher != DOT11_CIPHER_ALGO_NONE)
        {
            //
            // First we check if we have a key mapping key in this MAC context
            // that matches this node
            //
            if (!Mpdu->MulticastDestination)
            {                    
                ndisStatus = HwFindKeyMappingKeyIndex(currentMacContext, 
                                fragmentHeader->Address2, 
                                FALSE, 
                                &keyIndex
                                );

                if (ndisStatus == NDIS_STATUS_SUCCESS)
                {
                    // We found a key mapping 
                    keyUsed = &currentMacContext->KeyTable[keyIndex];
                }

                ndisStatus = NDIS_STATUS_SUCCESS;   // Reset status
            }

            //
            // Else, look for the default Key ID that was used for decryption
            //
            if (keyUsed == NULL)
            {
                headerSize = (fragmentHeader->FrameControl.FromDS && fragmentHeader->FrameControl.ToDS)?
                                sizeof(DOT11_DATA_LONG_HEADER) : sizeof(DOT11_DATA_SHORT_HEADER);
            
                keyIndex = *((PUCHAR)Add2Ptr(fragmentHeader, headerSize + 3));
                keyIndex = keyIndex >> 6;

                //
                // Determine if the key at this index from this MAC programmed on the hardware at
                // this same index
                //
                if ((currentMacContext->KeyTable[keyIndex].Key.Valid) && 
                    (currentMacContext->KeyTable[keyIndex].NicKeyIndex == keyIndex))
                {
                    // Yes. This key is set at the key index in the hardware at this key index
                    keyUsed = &currentMacContext->KeyTable[keyIndex];

                    // If we select a default key, we perform BSSID matching. This is to handle the
                    // two conditions:
                    // 1. Two MAC contexts are using the same default key ID. They may not be active
                    //    at the same time, but since we dont consider active/inactive when 
                    //    finding keys, we do BSSID matching
                    // 2. Two MAC contexts are active, with the first one in the list having default key 
                    //    at 0 and the second one using key mapping keys. Since packets with Key mapping 
                    //    keys can point to index 0 as their default key, we have to ensure that we 
                    //    dont incorrectly map this received packet to the incorrect context
                    doBSSIDMatching = TRUE;
                }
            }

            // If we found a key, lets check if we need to do BSSID matching
            if (keyUsed != NULL)
            {
                if (doBSSIDMatching)
                {
                    bssid = Dot11GetBSSID(Mpdu->DataStart, Mpdu->DataLength);
                    if (bssid != NULL)
                    {
                        if (!MP_COMPARE_MAC_ADDRESS(currentMacContext->DesiredBSSID, bssid))
                        {
                            // BSSID does not match, we dont select this MAC
                            keyUsed = NULL;
                        }
                    }
                    else
                    {
                        MPASSERT(bssid);    // These are all data or management packets that must have a BSSID
                        keyUsed = NULL;
                    }
                }
            }

            if (keyUsed != NULL)
            {
                // Found the key
                break;
            }
        }

    }

    return keyUsed;
}

NDIS_STATUS
HwIdentifyReceiveMac(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    PHW_MAC_CONTEXT             currentMacContext;
    ULONG                       i;
    PUCHAR                      mpduBssid;

    //
    // The code below is needed because we do not currently have unique MAC 
    // addresses per context. If we enable unique MAC addresses on the hardware
    // we can do the split based on MAC addresses & dont need to rely on
    // keys
    //

    //
    // If this is called after software decryption, we may have
    // the key already set
    //
    if (Mpdu->Key != NULL)
    {
        // Someone has already determined the key that is used for
        // encrypting this packet. We just need to look at the Mac context
        
        currentMacContext = Mpdu->Key->MacContext;

        // If the Mac context is already set, then we should
        // have the same one, else something went really wrong
        MPASSERT((Mpdu->MacContext == NULL) || (currentMacContext == Mpdu->MacContext));

        Mpdu->MacContext = currentMacContext;
        return NDIS_STATUS_SUCCESS;
    }

    MPASSERT(Mpdu->MacContext == NULL);

    // If there is only a single (non-helper) MAC active, we indicate the packet
    // on it
    if (!HW_MULTIPLE_MAC_ENABLED(Hw))
    {
        // The context at index 1 is the one we should use
        currentMacContext = &Hw->MacContext[HW_DEFAULT_PORT_MAC_INDEX];
        MPASSERT(HW_MAC_CONTEXT_IS_VALID(currentMacContext));
        Mpdu->MacContext = currentMacContext;
        return NDIS_STATUS_SUCCESS;        
    }

    // If we do not find any specific node to indicate this packet to. Indicate the
    // packet to all the MACs that are valid
    Mpdu->MacContext = NULL;

    // Information about synchronization of MacContext is given in HwFindDecryptionKey
    // We walk through the list of MAC contexts (ignoring MAC 0)
    for (i = HW_DEFAULT_PORT_MAC_INDEX ; i < HW_MAX_NUMBER_OF_MAC; i++)        
    {
        currentMacContext = &Hw->MacContext[i];
        if (!HW_MAC_CONTEXT_IS_VALID(currentMacContext) ||
            HW_TEST_MAC_CONTEXT_STATUS(currentMacContext, HW_MAC_CONTEXT_CANNOT_RECEIVE_FLAGS)
            )
        {
            continue;
        }
        
        //
        // Try to do BSSID based matching
        //
        mpduBssid = Dot11GetBSSID(Mpdu->DataStart, Mpdu->DataLength);
        if (mpduBssid != NULL)
        {
            if (DOT11_IS_UNICAST(currentMacContext->DesiredBSSID) &&
                MP_COMPARE_MAC_ADDRESS(mpduBssid, currentMacContext->DesiredBSSID))
            {
                // If BSSID matches, indicate the packet on this MAC

                // If we have 2 MACs that connect to a single BSSID, there is an issue 
                // here, only one of them would see the BSSID
                Mpdu->MacContext = currentMacContext;
                break;
#if 0
                macHeader = (PDOT11_MAC_HEADER)Mpdu->DataStart;

                // For data packets, we consider BSSID matching to be good enough since
                // we only one MAC can establish a connection to a BSSID 
                if (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_DATA)
                {
                    Mpdu->MacContext = currentMacContext;
                    break;
                }

                // For management packets BSSID matching is applied only for unicast packets.
                // Broadcast packets are indicated to everyone
                if (!Mpdu->MulticastDestination)
                {
                    Mpdu->MacContext = currentMacContext;
                    break;
                }
#endif
            }
        }
    }

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwFilterMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    PDOT11_MAC_HEADER           macHeader = (PDOT11_MAC_HEADER)Mpdu->DataStart;
    ULONG                       packetFilter;
    BOOLEAN                     addressMatch;
    PHW_MAC_CONTEXT             currentMacContext;
    ULONG                       i;
    PDOT11_MAC_ADDRESS          multicastAddressList;
    ULONG                       multicastAddressListCount;

    if (Mpdu->MacContext != NULL)
    {
        // If the MAC context is NULL, we use its packet filter & MAC address
        packetFilter = Mpdu->MacContext->PacketFilter;
        addressMatch = MP_COMPARE_MAC_ADDRESS(Mpdu->MacContext->MacAddress, macHeader->Address1);
    }
    else
    {
        // Else we use the HW packet filter and check if we have any MAC address that matches
        packetFilter = Hw->MacState.PacketFilter;
        addressMatch = FALSE;

        // Here we start from MAC context 0 since it can also receive probe responses as directed
        // packets
        for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)        
        {
            currentMacContext = &Hw->MacContext[i];
            if (HW_MAC_CONTEXT_IS_VALID(currentMacContext))
            {
                // Check if this MAC has this address
                addressMatch = MP_COMPARE_MAC_ADDRESS(currentMacContext->MacAddress, macHeader->Address1);
                if (addressMatch)
                    break;
            }
        }
    }

    switch(macHeader->FrameControl.Type)
    {
    case DOT11_FRAME_TYPE_MANAGEMENT:
        if (addressMatch && (packetFilter & NDIS_PACKET_TYPE_802_11_DIRECTED_MGMT))
            return NDIS_STATUS_SUCCESS;
        
        if (Mpdu->DescStatus.Broadcast && (packetFilter & NDIS_PACKET_TYPE_802_11_BROADCAST_MGMT))
            return NDIS_STATUS_SUCCESS;

        if (Mpdu->DescStatus.Multicast  && (packetFilter & (NDIS_PACKET_TYPE_802_11_MULTICAST_MGMT | 
                                                           NDIS_PACKET_TYPE_802_11_ALL_MULTICAST_MGMT)))
            return NDIS_STATUS_SUCCESS;

        if (packetFilter & NDIS_PACKET_TYPE_802_11_PROMISCUOUS_MGMT)
        {
            Hw->Stats.PhyCounters[Mpdu->PhyId].ullPromiscuousReceivedFragmentCount++;
            return NDIS_STATUS_SUCCESS;
        }

        //
        // We are dropping this packet. Check if we should indicate it upto the helper port
        //
        if ((macHeader->FrameControl.Subtype == DOT11_MGMT_SUBTYPE_BEACON) ||
            (macHeader->FrameControl.Subtype == DOT11_MGMT_SUBTYPE_PROBE_RESPONSE))
        {
            //
            // The MAC context currently (if any) flagged to get this does not want it. 
            // So only the helper port gets this, change the MAC context in the MPDU. We may 
            // have updated statistics for the old MAC context, but that is OK.
            //
            Mpdu->MacContext = &Hw->MacContext[HW_HELPER_PORT_MAC_INDEX];
            return NDIS_STATUS_SUCCESS;
        }

        break;
        
    case DOT11_FRAME_TYPE_CONTROL:
        if (addressMatch && (packetFilter & NDIS_PACKET_TYPE_802_11_DIRECTED_CTRL))
            return NDIS_STATUS_SUCCESS;

        if (Mpdu->DescStatus.Broadcast && (packetFilter & NDIS_PACKET_TYPE_802_11_BROADCAST_CTRL))
            return NDIS_STATUS_SUCCESS;

        if (packetFilter & NDIS_PACKET_TYPE_802_11_PROMISCUOUS_CTRL)
        {
            Hw->Stats.PhyCounters[Mpdu->PhyId].ullPromiscuousReceivedFragmentCount++;
            return NDIS_STATUS_SUCCESS;
        }        

        //
        // Note: no multicast control frame.
        //

        break;
        
    case DOT11_FRAME_TYPE_DATA:
        if (addressMatch && (packetFilter & NDIS_PACKET_TYPE_DIRECTED))
            return NDIS_STATUS_SUCCESS;

        if (Mpdu->DescStatus.Broadcast && (packetFilter & NDIS_PACKET_TYPE_BROADCAST))
            return NDIS_STATUS_SUCCESS;

        if (Mpdu->DescStatus.Multicast && (packetFilter & NDIS_PACKET_TYPE_ALL_MULTICAST))
            return NDIS_STATUS_SUCCESS;

        if (Mpdu->DescStatus.Multicast && (packetFilter & NDIS_PACKET_TYPE_MULTICAST))
        {
            // Perform filtering for multicast addresses
            if (Mpdu->MacContext)
            {
                multicastAddressListCount = Mpdu->MacContext->MulticastAddressCount;
                multicastAddressList = Mpdu->MacContext->MulticastAddressList;
            }
            else
            {
                multicastAddressListCount = Hw->MacState.MulticastAddressCount;
                multicastAddressList = Hw->MacState.MulticastAddressList;
            }
            
            for (i = 0; i < multicastAddressListCount; i++)
            {
                if (MP_COMPARE_MAC_ADDRESS(macHeader->Address1, multicastAddressList[i]))
                {
                    return NDIS_STATUS_SUCCESS;
                }
            }
            return NDIS_STATUS_NOT_ACCEPTED;
        }

        if (packetFilter & (NDIS_PACKET_TYPE_PROMISCUOUS | 
                                          NDIS_PACKET_TYPE_802_11_RAW_DATA))
        {
            Hw->Stats.PhyCounters[Mpdu->PhyId].ullPromiscuousReceivedFragmentCount++;
            return NDIS_STATUS_SUCCESS;
        }

        break;
        
    default:
        //
        // Reserved packet should always be filtered
        //
        return NDIS_STATUS_NOT_ACCEPTED;
    }

    return NDIS_STATUS_NOT_ACCEPTED;
}

NDIS_STATUS
HwPrepareReceivedMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_MAC_HEADER           macHeader;
    BOOLEAN                     goodMpdu = FALSE;
    
    do
    {
#if DBG
        if (Hw->RxInfo.Debug_BreakOnReceiveCount > 0)
        {
            // For debugging purpose
            if (MpDebugMatchPacketType(
                    Mpdu->DataStart,
                    Hw->RxInfo.Debug_BreakOnReceiveType,
                    Hw->RxInfo.Debug_BreakOnReceiveSubType,
                    &Hw->RxInfo.Debug_BreakOnReceiveDestinationAddress,
                    (Hw->RxInfo.Debug_BreakOnReceiveMatchSource ? &Hw->RxInfo.Debug_BreakOnReceiveSourceAddress
                        : NULL)
                    ))
            {
                if (Hw->RxInfo.Debug_BreakOnReceiveCount)
                {
                    DbgPrint("Receive packet matching Rx BreakOnReceive filter %p\n", Mpdu->DataStart);
                    Hw->RxInfo.Debug_BreakOnReceiveCount--;
                    DbgBreakPoint();
                }
            }
        }
#endif

        // The unicast bit is easy
        if (Mpdu->DescStatus.Unicast)
            Mpdu->MulticastDestination = FALSE;
        else        
            Mpdu->MulticastDestination = TRUE;

        // Check phy level state
        goodMpdu = HwCheckPhyParameters(Hw, Mpdu);

        // For some modes, we dont do a lot of work
        if (Hw->MacState.NetmonModeEnabled)
        {
            // If we are in netmon mode, we dont need to do much preparation
            Mpdu->RawPacket = TRUE;

            // Sometimes in netmon mode, we get partial packets that we shouldnt
            // indicate up. Drop those else the upper layers may crash
            if (Mpdu->DataLength < (4 + sizeof(DOT11_MAC_HEADER)))
            {
                ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            }
            
            // Indicate on MAC 1
            MPASSERT(HW_MAC_CONTEXT_IS_VALID(&Hw->MacContext[HW_DEFAULT_PORT_MAC_INDEX]));
            Mpdu->MacContext = &Hw->MacContext[HW_DEFAULT_PORT_MAC_INDEX];
            break;
        }

        if (!goodMpdu)
        {
            // This packet is bad at the PHY layer. Drop it now
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            break;
        }
        else
        {
            // The hardware includes the FCS in the DataLength. Adjust for that
            Mpdu->DataLength -= 4;
            if (Mpdu->DataLength < sizeof(DOT11_MAC_HEADER))
            {
                // Too short a packet. We just reject it
                ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
                break;
            }

            if (Mpdu->DataLength > MAX_TX_RX_PACKET_SIZE)
            {
                // Too large a packet. We again reject it
                ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
                break;
            }
        }

        //
        // Packet Integrity: From this point on, its verified that packets are
        // are atleast MAC_HEAER size
        //
        if (Hw->MacState.SafeModeEnabled)
        {
            // And set the encrypted bit based on whether or not the WEP bit is set
            // in the received packet
            macHeader = (PDOT11_MAC_HEADER)Mpdu->DataStart;
            Mpdu->Encrypted = (macHeader->FrameControl.WEP == 1) ? TRUE: FALSE;

            // Rest of the filtering happens as before
        }
        else
        {
            // Do anything that is relevant for ciphers
            ndisStatus = HwProcessRxCipher(Hw, Mpdu);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                // Something went wrong when deciphering the packet. We wont try
                // to identify the MAC
                MpTrace(COMP_RECV, DBG_LOUD, ("Dropped RX MPDU as HwProcessRxCipher failed with status 0x%08x\n", ndisStatus));
                break;
            }
        }

        // Try to identify the MAC context that the packet must get indicated on
        ndisStatus = HwIdentifyReceiveMac(Hw, Mpdu);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // This is a bad packet that should not be indicate to any MACs
            MpTrace(COMP_RECV, DBG_LOUD, ("Dropped RX MPDU as HwIdentifyReceiveMac failed with status 0x%08x\n", ndisStatus));
            break;
        }


        // Check MAC level state information about the MPDU
        goodMpdu = HwCheckMacParameters(Hw, Mpdu);
        if (!goodMpdu)
        {
            // This packet is bad at the MAC layer. Drop it now
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            break;
        }

        //
        // Packet Integrity: From this point on, its verified that management and
        // data packets are atleast the minimum size required. Control packet length
        // must be verified
        //
        

        // Check if the HW layer wants to respond to this packet, etc
        // This is done before filtering since the HW layer may want to
        // respond to some packets that dont need to be indicated up
        ndisStatus = HwProcessReceivedMPDU(Hw, Mpdu);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // Mpdu should not be processed
            MpTrace(COMP_RECV, DBG_LOUD, ("Dropped RX MPDU as HwProcessReceivedMPDU failed with status 0x%08x\n", ndisStatus));
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            break;            
        }

        // Filter the non-important packets out
        ndisStatus = HwFilterMPDU(Hw, Mpdu);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
HwSecurityCheck(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PHW_RX_MPDU                 firstMpdu, currentMpdu;
    PHW_KEY_ENTRY               key;
    PHW_MAC_CONTEXT             macContext;
    UCHAR                       receivedMIC[HW_MIC_LENGTH];
    UCHAR                       calculatedMIC[HW_MIC_LENGTH];
    ULONG                       length, mpduIndex, offset, micIndex;
    ULONG                       usedLength;

    UNREFERENCED_PARAMETER(Hw);
    //
    // This function does the MIC failure checking and replay counter checking.
    // If either one is done by hardware, its respective portion can be skipped.
    //

    //
    // Get the key that decrypted this MSDU. If we did not decrypt this MSDU, as
    // indicated by a NULL key, simply return success.
    //
    MPASSERT(Msdu->MpduCount >= 1);
    _Analysis_assume_(Msdu->MpduCount >= 1);

    firstMpdu = Msdu->MpduList[0];
    key = firstMpdu->Key;
    if (key == NULL)
    {
        return NDIS_STATUS_SUCCESS;
    }

    // This should be skipped for safe mode and netmon mode
    MPASSERT((Hw->MacState.SafeModeEnabled == FALSE) && (Hw->MacState.NetmonModeEnabled == FALSE));

    // Since we have a key, we should have MAC context
    MPASSERT(firstMpdu->MacContext != NULL);
    macContext = firstMpdu->MacContext;

    if (key->Key.AlgoId == DOT11_CIPHER_ALGO_TKIP)
    {
        //
        // If the MSDU was decrypted by TKIP, check MIC and replay counter.
        // First, get the MIC field. Note that MIC could span across two or more MPDUs.
        //
        
        length = Msdu->DataLength;
        MPASSERT(length > HW_MIC_LENGTH);

        //
        // Find the MPDU that contains (at least the first byte of) MIC.
        // This is the MPDU which contains the 8th byte from the end of the MSDU
        //
        mpduIndex = 0;
        currentMpdu = firstMpdu;
        while ((length - currentMpdu->DataLength) >= HW_MIC_LENGTH)
        {
            length = length - currentMpdu->DataLength;
            mpduIndex++;
            currentMpdu = Msdu->MpduList[mpduIndex];
        }

        //
        // Found that MDL. Calculate the offset of MIC in the MDL. Copy MIC to our buffer.
        //
        offset = length - HW_MIC_LENGTH;
        usedLength = 0;
        for (micIndex = 0; micIndex < HW_MIC_LENGTH; micIndex++)
        {
            receivedMIC[micIndex] = *((PUCHAR)Add2Ptr(currentMpdu->DataStart, offset));
            offset++;
            usedLength++;

            if ((offset >= currentMpdu->DataLength) && (usedLength < HW_MIC_LENGTH))
            {
                // The rest of the MIC is in following MPDUs
                
                // Adjust the length of the MPDU to remove the MIC
                currentMpdu->DataLength -= usedLength;
                usedLength = 0;

                // Move to the next MPDU
                mpduIndex++;
                currentMpdu = Msdu->MpduList[mpduIndex];
                offset = 0;
            }
        }
        // Adjust the length of the MPDU to remove the MIC
        currentMpdu->DataLength -= usedLength;
        

        //
        // Shorten the total data by size of MIC. Then calculate the MIC based on
        // receiving data and Rx MIC key.
        //
        Msdu->DataLength -= HW_MIC_LENGTH;

        ndisStatus = HwCalculateRxMIC(Msdu, 0, key->Key.RxMICKey, calculatedMIC);
        MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);

        //
        // Compare the received MIC vs. calculated MIC. If there is mismatch, indicated
        // MIC failure.
        //
        if (NdisEqualMemory(receivedMIC, calculatedMIC, HW_MIC_LENGTH) != 1)
        {
        
            HwIndicateMICFailure(macContext,
                (PDOT11_MGMT_DATA_MAC_HEADER)firstMpdu->DataStart,
                ((key->PeerKeyIndex < DOT11_MAX_NUM_DEFAULT_KEY) ? key->PeerKeyIndex : 0),
                ((key->PeerKeyIndex < DOT11_MAX_NUM_DEFAULT_KEY) ? TRUE : FALSE)
                );

            MpTrace(COMP_RECV, DBG_SERIOUS, ("TKIP MIC failure detected\n"));

            if (firstMpdu->MulticastDestination)
            {
                macContext->MulticastCounters.ullTKIPLocalMICFailures++;
            }
            else
            {
                macContext->UnicastCounters.ullTKIPLocalMICFailures++;
            }
            return NDIS_STATUS_NOT_ACCEPTED;
        }

        //
        // Replay counter checking for TKIP. This is done after MIC verification. 
        // Two things are checked:
        //   1. The frame number of the first fragment in a MSDU must be greater than
        //      the replay counter for the key that decrypted the MSDU.
        //   2. The frame number of the all fragments in a MSDU must be monotonically
        //      increased, but not necessarily sequential (unlike in CCMP).
        //
        if (firstMpdu->FrameNumber <= key->ReplayCounter)
        {
            MpTrace(COMP_RECV, DBG_SERIOUS, ("TKIP Replay counter failed for first MPDU\n"));

            if (firstMpdu->MulticastDestination)
            {
                macContext->MulticastCounters.ullTKIPReplays++;
            }
            else
            {
                macContext->UnicastCounters.ullTKIPReplays++;
            }
            
            return NDIS_STATUS_NOT_ACCEPTED;
        }

        for (mpduIndex = 0; mpduIndex < (ULONG)Msdu->MpduCount - 1; mpduIndex++)
        {
            if (Msdu->MpduList[mpduIndex]->FrameNumber >= Msdu->MpduList[mpduIndex + 1]->FrameNumber)
            {
                MpTrace(COMP_RECV, DBG_SERIOUS, ("TKIP Replay counter failed for middle MPDU\n"));
                if (firstMpdu->MulticastDestination)
                {
                    macContext->MulticastCounters.ullTKIPReplays++;
                }
                else
                {
                    macContext->UnicastCounters.ullTKIPReplays++;
                }            
                return NDIS_STATUS_NOT_ACCEPTED;
            }
        }

        //
        // Passed replay counter check. Update the replay counter in the key to frame number of
        // the last fragment in the MSDU.
        //

        // MPDU Count > 0
        key->ReplayCounter = Msdu->MpduList[Msdu->MpduCount - 1]->FrameNumber;
    }
    else if (key->Key.AlgoId == DOT11_CIPHER_ALGO_CCMP)
    {
        //
        // If the MSDU was decrypted by CCMP, check replay counter.
        // Two things are checked:
        //   1. The frame number of the first fragment in a MSDU must be greater than
        //      the replay counter for the key that decrypted the MSDU.
        //   2. The frame number of the all fragments in a MSDU must be sequential.
        //
        if (firstMpdu->FrameNumber <= key->ReplayCounter)
        {
            MpTrace(COMP_RECV, DBG_SERIOUS, ("CCMP Replay counter failed for first MPDU\n"));

            if (firstMpdu->MulticastDestination)
            {
                macContext->MulticastCounters.ullCCMPReplays++;
            }
            else
            {
                macContext->UnicastCounters.ullCCMPReplays++;
            }
            
            return NDIS_STATUS_NOT_ACCEPTED;
        }

        for (mpduIndex = 0; mpduIndex < (ULONG)Msdu->MpduCount - 1; mpduIndex++)
        {
            if ((Msdu->MpduList[mpduIndex]->FrameNumber + 1) != Msdu->MpduList[mpduIndex + 1]->FrameNumber)
            {
                MpTrace(COMP_RECV, DBG_SERIOUS, ("TKIP Replay counter failed for middle MPDU\n"));
                if (firstMpdu->MulticastDestination)
                {
                    macContext->MulticastCounters.ullCCMPReplays++;
                }
                else
                {
                    macContext->UnicastCounters.ullCCMPReplays++;
                }            
                return NDIS_STATUS_NOT_ACCEPTED;
            }
        }

        //
        // Passed replay counter check. Update the replay counter in the key to frame number of
        // the last fragment in the MSDU.
        //
        key->ReplayCounter = Msdu->MpduList[Msdu->MpduCount - 1]->FrameNumber;

    }

    // Update the decryption counters
    if (firstMpdu->MulticastDestination)
    {
        macContext->MulticastCounters.ullDecryptSuccessCount++;
    }
    else
    {
        macContext->UnicastCounters.ullDecryptSuccessCount++;
    }

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
HwPrepareReceivedMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PHW_RX_MPDU                 firstMpdu;
    PDOT11_MAC_HEADER           macHeader;
    UCHAR                       headerSize;
    ULONG                       totalMsduLength, i;
    PHW_MAC_CONTEXT             macContext;

    do
    {
        //
        // Compute the real length of the MSDU
        //
        firstMpdu = Msdu->MpduList[0];

        // The length of the packet header
        macHeader = (PDOT11_MAC_HEADER)firstMpdu->DataStart;
        if ((macHeader->FrameControl.Type == DOT11_FRAME_TYPE_DATA) ||
                 (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_MANAGEMENT))
        {
            if ((macHeader->FrameControl.ToDS == 1) && (macHeader->FrameControl.FromDS == 1))
                headerSize = DOT11_DATA_LONG_HEADER_SIZE;
            else
                headerSize = DOT11_DATA_SHORT_HEADER_SIZE;        
        }
        else
        {
            // Control
            headerSize = sizeof(DOT11_MAC_HEADER);
        }

        // For fragmented packets, remove the headers from second MPDU
        // onwards. The first MPDU does not get modified
        totalMsduLength = firstMpdu->DataLength;

        for (i = 1; i < Msdu->MpduCount; i++)
        {
            // For the total length of the MSDU we are NOT including the
            // fragment header length the MPDUs
            totalMsduLength = totalMsduLength + Msdu->MpduList[i]->DataLength 
                                - headerSize;

            // Adjust the MPDU's data start & data length
            Msdu->MpduList[i]->DataStart += headerSize;
            Msdu->MpduList[i]->DataLength -= headerSize;
            
        }
        Msdu->DataLength = totalMsduLength;

        //
        // Do security checking. This includes
        // MIC failure checking and replay counter checking.
        //
        ndisStatus = HwSecurityCheck(Hw, Msdu);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        // Update the frame statistics
        macContext = HW_MAC_CONTEXT_FOR_RX_STATISTICS(Hw, firstMpdu);     
        if (firstMpdu->MulticastDestination)
        {
            macContext->MulticastCounters.ullReceivedFrameCount++;
        }
        else
        {
            macContext->UnicastCounters.ullReceivedFrameCount++;
        }
        Hw->Stats.PhyCounters[firstMpdu->PhyId].ullReceivedFrameCount++;

        ndisStatus = NDIS_STATUS_SUCCESS;
    }while (FALSE);

    return ndisStatus;
}


// Lock held (or called from Initialize)
// Used to return a RxMPDU to the HAL for filling
__inline VOID
HwSubmitRxMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    //
    // Return this to the HAL
    //
    HalReturnRxDesc(Hw->Hal,
        MAX_TX_RX_PACKET_SIZE,
        (PUCHAR)Mpdu->BufferVa,
        Mpdu->BufferPa,
        &Mpdu->DescIter
        );

    // Add this to the RX queue
    InsertTailList(&Hw->RxInfo.AvailableMPDUList, &Mpdu->ListEntry);

    HW_INCREMENT_AVAILABLE_RX_MPDU(Hw);
}
VOID
HwReturnRxMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    BOOLEAN                     addToUnusedList = TRUE;

    HW_REINITIALIZE_RX_MPDU(Mpdu);
    
    HW_RX_ACQUIRE_LOCK(Hw, DispatchLevel);    
    if (Hw->RxInfo.NumMPDUAvailable >= (LONG)Hw->RegInfo.NumRXBuffers)
    {
#if 0 
        ULONG                       percentMPDUListUnunsed;
        //
        // Check if we need to shrink the Rx MSDU list.
        // We have been sampling how many Rx MSDUs remain unused at every
        // check for hang interval. If the average number of free Rx MSDUs remain
        // above a threshold for a statistically significant time interval,
        // we will free this Rx MSDU in order to shrink the MSDU list.
        //
        if (Hw->RxInfo.UnusedMPDUListSampleTicks > HW_RX_MSDU_LIST_SAMPLING_PERIOD)
        {
            
            //
            // Determine the percentage of Rx MSDU list that has remained underutilized
            // for last RxMSDUListSampleTicks time interval
            //
            percentMPDUListUnunsed = 
                      ((Hw->RxInfo.NumMPDUUnused * 100)  / (Hw->RxInfo.UnusedMPDUListSampleTicks * Hw->RxInfo->NumMPDUAllocated))));

            //
            // Restart sampling for the next sampling period
            //
            Hw->RxInfo.UnusedMPDUListSampleTicks = 0;
            Hw->RxInfo.UnusedMPDUListSampleCount = 0;
            
            if (percentMPDUListUnunsed > HW_RX_MSDU_LIST_UNDER_UTILIZATION_THRESHOLD)
            {
                MpTrace(COMP_RECV, DBG_NORMAL,  ("Shrinking the Rx MPDU pool. Current Size: %d   Percentage Under Utilization: %d\n",
                                Hw->RxInfo->NumMPDUAllocated, percentMPDUListUnunsed));
                //
                // The threshold was exceeded, lets free this MSDU
                //

                //
                // We will free the last fragment in this Rx MSDU and return
                // the rest of the fragments to the Hw11 for reuse
                //
                Hw11FreeFragment(
                    pAdapter->pNic,
                    pMpRxd->Fragments[--pMpRxd->FragmentCount]
                    );
                NextNetBufferList = NET_BUFFER_LIST_NEXT_NBL(CurrentNetBufferList);
                MpDropRxMSDU(pAdapter, pMpRxd, DispatchLevel);
                MpFreeRxMSDU(pAdapter, pMpRxd);
                MP_DECREMENT_TOTAL_RX_MSDU_ALLOCATED(pAdapter);
                addToUnusedList = FALSE;
            }


        }
#endif
        if (addToUnusedList)
        {
            //
            // If the MPDUs available to HAL is at the maximum posible value already
            // in we cannot add any right now. We put this in the Unused list and
            // would transfer it later when we remove some from the HAL (on a receive
            // indication)
            // 
            InsertTailList(&Hw->RxInfo.UnusedMPDUList, &Mpdu->ListEntry);
            HW_INCREMENT_UNUSED_RX_MPDU(Hw);
        }
    }
    else 
    {
        //
        // Return the MPDU to the HAL
        //
        HwSubmitRxMPDU(Hw, Mpdu);
    }
    HW_RX_RELEASE_LOCK(Hw, DispatchLevel);
}

NDIS_STATUS
HwGrowMPDUPool(
    _In_  PHW                     Hw,
    _In_  ULONG                   NumToAllocate
    )
{
    ULONG                       i;
    PHW_RX_MPDU                 mpdu;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    for (i = 0; i < NumToAllocate; i++) 
    {
        // Allocate an MPDU structure
        mpdu = NdisAllocateFromNPagedLookasideList(&Hw->RxInfo.RxFragmentLookaside);
        if (mpdu == NULL)
        {
            MpTrace(COMP_SEND, DBG_NORMAL, ("Unable to allocate extra HW_RX_MPDU %d\n", i));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        // Initialize the data structure
        HW_INITIALIZE_RX_MPDU(mpdu, Hw);

        // Increment the adapter ref count. This blocks resets, pauses from proceeding
        HW_INCREMENT_ACTIVE_OPERATION_REF(Hw);

        // Call the Async allocate handler. This calls HWAllocateComplete
        if ((ndisStatus = NdisMAllocateSharedMemoryAsyncEx(
                Hw->MiniportDmaHandle,
                MAX_TX_RX_PACKET_SIZE,
                FALSE,
                mpdu
                )) == NDIS_STATUS_FAILURE)
        {
            ndisStatus=NDIS_STATUS_RESOURCES;
            MpTrace(COMP_SEND, DBG_SERIOUS, ("Allocation of Extra RX buffer %d failed\n", i));
            NdisFreeToNPagedLookasideList(&Hw->RxInfo.RxFragmentLookaside, mpdu);
            
            // Remove the ref
            HW_DECREMENT_ACTIVE_OPERATION_REF(Hw);
            break;
        }

    }
    
    return ndisStatus;
}

VOID 
HWAllocateComplete(
    _In_  NDIS_HANDLE             MiniportAdapterContext,
    _In_  PVOID                   VirtualAddress,
    _In_  PNDIS_PHYSICAL_ADDRESS  PhysicalAddress,
    _In_  ULONG                   Length,
    _In_  PVOID                   Context
    )
{
    PHW_RX_MPDU                 mpdu;
    PHW                         hw;

    UNREFERENCED_PARAMETER(Length);
    
    mpdu = (PHW_RX_MPDU)Context;

    // We need the hardware context for the completion and not the AdapterContext
    // which is passed into the SharedMemoryComplete function
    UNREFERENCED_PARAMETER(MiniportAdapterContext);
    hw = mpdu->Hw;
    
    if (VirtualAddress == NULL) 
    {
        MpTrace(COMP_SEND, DBG_SERIOUS, ("Allocation of Extra RX buffer failed\n"));
        NdisFreeToNPagedLookasideList(&hw->RxInfo.RxFragmentLookaside, mpdu);
        HW_DECREMENT_ACTIVE_OPERATION_REF(hw);
    }
    else 
    {
        mpdu->BufferVa = VirtualAddress;
        mpdu->BufferPa = *PhysicalAddress;

        // Increment the counter for total number of MPDU allocated
        HW_INCREMENT_TOTAL_RX_MPDU_ALLOCATED(hw);

        // Return the MPDU. This may either go to the hardware
        // or the unused list depending on how many are free
        HwReturnRxMPDU(hw, mpdu, FALSE); // NOT Called at DISPATCH

        // Remove the ref
        HW_DECREMENT_ACTIVE_OPERATION_REF(hw);
    }
}



VOID
HwReturnRxMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    ULONG                       i;

    // Return the MPDUs    
    for (i = 0; i < Msdu->MpduCount; i++)
    {
        HwReturnRxMPDU(Hw, Msdu->MpduList[i], DispatchLevel);
    }

    Msdu->MpduCount = 0;

    HwFreeRxMSDU(Hw, Msdu);
}

VOID
HwIndicateReceivedMSDUs(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             MsduList,
    _In_  ULONG                   ReceiveFlags
    )
{
    PHW_RX_MSDU                 currentMsdu, nextMsdu;
    PHW_RX_MPDU                 firstMpdu;
    PDOT11_MAC_HEADER           macHeader;
    PHW_MAC_CONTEXT             macContext;
    BOOLEAN                     returnMSDU = TRUE;
    ULONG                       i;

    currentMsdu = MsduList;
    while (currentMsdu != NULL)
    {
        nextMsdu = currentMsdu->Next;
        currentMsdu->Next = NULL;

        // If we indicate with status resources, the MSDU
        // wont be returned, else it would be
        returnMSDU = TRUE;
        
        firstMpdu = currentMsdu->MpduList[0];
        macHeader = (PDOT11_MAC_HEADER)firstMpdu->DataStart;

        // If a packet is a management beacon or probe response packet, 
        // it must be indicated to the HELPER_PORT mac context
        if ((macHeader->FrameControl.Type == DOT11_FRAME_TYPE_MANAGEMENT) &&
            ((macHeader->FrameControl.Subtype == DOT11_MGMT_SUBTYPE_BEACON) ||
             (macHeader->FrameControl.Subtype == DOT11_MGMT_SUBTYPE_PROBE_RESPONSE)))
        {
            // All management packet are indicated to helper port. Indication
            // is with status resources since we dont expect the helper port to ever
            // indicate these packets up to the OS
            HwIndicateMSDUOnMACContext(
                &Hw->MacContext[HW_HELPER_PORT_MAC_INDEX],
                currentMsdu, 
                ReceiveFlags | NDIS_RECEIVE_FLAGS_RESOURCES
                );
        }

        //
        // Some packets only need to be indicated to the helper port. Lets check
        // if this is one of them
        //
        if (firstMpdu->MacContext != &Hw->MacContext[HW_HELPER_PORT_MAC_INDEX])
        {
            if (firstMpdu->MacContext != NULL)
            {
                // Mac context has been identified
                macContext = firstMpdu->MacContext;
                
                MPASSERT(HW_MAC_CONTEXT_IS_VALID(macContext));

                // Indicate this to the specific MAC
                HwIndicateMSDUOnMACContext(
                    macContext,
                    currentMsdu, 
                    ReceiveFlags
                    );

                // The MSDU would be returned from the return handler
                returnMSDU = FALSE;
            }
            else
            {
                // Indicate the MSDU to all the valid MACs. Everyone gets the packet
                // with status resources
                
                for (i = HW_DEFAULT_PORT_MAC_INDEX; i < HW_MAX_NUMBER_OF_MAC; i++)
                {
                    macContext = &Hw->MacContext[i];

                    if (HW_MAC_CONTEXT_IS_VALID(macContext))
                    {
                        // Indicate this the specific MAC
                        HwIndicateMSDUOnMACContext(
                            macContext,
                            currentMsdu, 
                            ReceiveFlags | NDIS_RECEIVE_FLAGS_RESOURCES
                            );
                    }
                }
            }
        }
        
        // We return the MSDU if either we indicate with resources
        // or the caller set the flag that it should be indicated with 
        // status resources
        if ((returnMSDU == TRUE) ||
            (NDIS_TEST_RECEIVE_CANNOT_PEND(ReceiveFlags)))
        {
            // The MSDU is done. Return
            HwReturnRxMSDU(
                Hw, 
                currentMsdu, 
                (NDIS_TEST_RECEIVE_AT_DISPATCH_LEVEL(ReceiveFlags) ? TRUE : FALSE)
                );
        }

        currentMsdu = nextMsdu;
    }
}

PMP_RX_MSDU
HwMapHwRxMSDUToMpRxMSDU(
    _In_  PHW_RX_MSDU             Msdu
    )
{
    PMP_RX_MSDU                 mpMsdu;
    PMP_RX_MPDU                 mpMpdu;
    PHW_RX_MPDU                 mpdu;
    ULONG                       i;

    // The MP_RX_MSDU is preallocated as part of the HW_RX_MSDU
    mpMsdu = Msdu->MpMsdu;

    for (i = 0; i < Msdu->MpduCount; i++)
    {
        // Populate the MP_RX_MPDU
        mpdu = Msdu->MpduList[i];
        mpMpdu = mpdu->MpMpdu;
        mpMpdu->Data = mpdu->DataStart;
        mpMpdu->DataLength = mpdu->DataLength;
        mpMpdu->Msdu = mpMsdu;
        
        // Add it to the list
        mpMsdu->MpduList[i] = mpMpdu;
    }
    mpMsdu->MpduCount = Msdu->MpduCount;

    // Copy the receive context from the first MPDU
    mpdu = Msdu->MpduList[0];

    mpMsdu->RecvContext.lRSSI = mpdu->RSSI;
    mpMsdu->RecvContext.ucDataRate = (UCHAR)mpdu->Rate;
    mpMsdu->RecvContext.uPhyId = mpdu->PhyId;
    mpMsdu->RecvContext.uReceiveFlags = (mpdu->RawPacket ? DOT11_RECV_FLAG_RAW_PACKET : 0);
    mpMsdu->RecvContext.usNumberOfMPDUsReceived = mpMsdu->MpduCount;
    mpMsdu->Channel = (UCHAR)mpdu->Channel;
    mpMsdu->RecvContext.uChCenterFrequency = HwChannelToFrequency(mpMsdu->Channel);
    mpMsdu->LinkQuality = mpdu->LinkQuality;
    mpMsdu->Flags = (mpdu->Encrypted ? MP_RX_MSDU_FLAG_ENCRYPTED : 0);

    mpMsdu->Next = NULL;

    return mpMsdu;
}

VOID
HwIndicateMSDUOnMACContext(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_RX_MSDU             Msdu,
    _In_  ULONG                   ReceiveFlags
    )
{
    PMP_RX_MSDU                 mpMsdu;

    mpMsdu = HwMapHwRxMSDUToMpRxMSDU(Msdu);

    // Add a ref count so that context dont change
    // while packets are pending on the MAC
    HW_ADD_MAC_CONTEXT_RECV_REF(MacContext, 1);

    // We only indicate packets on running ports. Else we may
    // end up indicating packets up to ports that are in the middle of
    // opmode change, etc
    if (!HW_TEST_MAC_CONTEXT_STATUS(MacContext, HW_MAC_CONTEXT_CANNOT_SEND_FLAGS))
    {
        // Call Hvl
        Hvl11IndicateReceivePackets(MacContext->VNic,
            mpMsdu,
            ReceiveFlags
            );
    }
    else
    {
        // Cannot indicate this up
        if (NDIS_TEST_RECEIVE_CAN_PEND(ReceiveFlags))
        {
            // Return the packet back to the HW
            Hw11ReturnPackets(MacContext, 
                mpMsdu, 
                (NDIS_TEST_RECEIVE_AT_DISPATCH_LEVEL(ReceiveFlags) ? 
                    NDIS_RETURN_FLAGS_DISPATCH_LEVEL: 0)
                );
        }
    }

    // If the packet cannot pend at the above layer remove the
    // the refcount that blocks context switches, pauses, etc
    if (NDIS_TEST_RECEIVE_CANNOT_PEND(ReceiveFlags))
        HW_REMOVE_MAC_CONTEXT_RECV_REF(MacContext, 1);  
}


__inline NDIS_STATUS
HwAddRxMPDUToMSDU(
    _In_  PHW_RX_MSDU                 Msdu,
    _In_  PHW_RX_MPDU                 Mpdu,
    _In_  PDOT11_MGMT_DATA_MAC_HEADER FragmentHeader,
    _In_  UCHAR                       FragNumber
    )
{
    NDIS_STATUS     ndisStatus = NDIS_STATUS_PENDING;
    
    do
    {
        //
        // If this is a duplicate fragment, drop it
        //
        if (Msdu->MpduList[FragNumber] != NULL)
        {
            MpTrace(COMP_RECV, DBG_LOUD,  ("Duplicate fragment received\n"));
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            break;
        }
        
        //
        // If a fragment in this 802.11 packet was lost, fail the entire packet
        //
        if (FragNumber > Msdu->MpduCount)
        {
            MpTrace(COMP_RECV, DBG_LOUD,  ("A fragment in the packet was lost. Dropping packet\n"));
            ndisStatus = NDIS_STATUS_FAILURE;
            break;
        }
        
        //
        // Is this the last fragment in the frame, we are done
        //
        if (FragmentHeader->FrameControl.MoreFrag == 0)
        {
            //
            // Tell the caller that the Rx MSDU is now complete.
            //
            ndisStatus = NDIS_STATUS_SUCCESS;
        }
        else
        {
            if(FragmentHeader->FrameControl.Type != DOT11_FRAME_TYPE_DATA)
            {
                //
                // Only data frames can be fragmented. Drop the frame if it is suspicious.
                //
                MpTrace(COMP_RECV, DBG_LOUD,  ("Error! A non-data frame was received which is fragmented!\n"));
                ndisStatus = NDIS_STATUS_FAILURE;
                break;
            } else
            {
                ndisStatus = NDIS_STATUS_PENDING;
            }
        }

        //
        // This is a good fragment. Add it to the MSDU
        //
        Msdu->MpduList[FragNumber] = Mpdu;
        Msdu->MpduCount++;
        
    } while(FALSE);
    
    return ndisStatus;
}


NDIS_STATUS
HwAddPartialMSDUToReassemblyLine(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    USHORT                      i;

    //
    // If we are running low on reassembly line space,
    // expire some ReceiveLifeTime packets
    //
    if (Hw->RxInfo.TotalRxMSDUInReassembly >= HW_REASSEMBLY_LINE_LOW_RESOURCES_THRESHOLD)
    {
        HwExpireReassemblingMSDUs(Hw, DispatchLevel);
    }

    for(i = 0; i < HW_MAX_REASSEMBLY_LINE_SIZE; i++)
    {
        if(Hw->RxInfo.ReassemblyLine[i] == NULL)
        {
            //
            // Found an empty spot for this Rx MSDU in the assembly line
            //
            Hw->RxInfo.ReassemblyLine[i] = Msdu;
            Hw->RxInfo.TotalRxMSDUInReassembly++;
            
            //
            // store position index in the Rx MSDU & Timestamp this MSDU
            //
            Msdu->ReassemblyLinePosition = i;
            HW_SET_RX_EXPIRATION_TIME(Msdu, Hw->MacState.MaxReceiveLifetime);
            
            //
            // The Most Recently Used Assembly Rx MSDU becomes this one
            // This gives us an optimization by avoiding search
            // as we expect next receive to belong to this MSDU
            //
            Hw->RxInfo.MRUReassemblyRxMSDU = Msdu;
            
            return NDIS_STATUS_SUCCESS;
        }
    }

    return NDIS_STATUS_RESOURCES;
}


__inline PHW_RX_MSDU
HwFindPartialMSDUInReassemblyLine(
    _In_  PHW                     Hw,
    _In_  PDOT11_MGMT_DATA_MAC_HEADER FragmentHdr, 
    _In_  USHORT                  SequenceNumber
    )
{
    PHW_RX_MSDU                 msdu;
    USHORT                      i;
    
    //
    // First check the MRU Reassembly MSDU.
    // Make sure there is some Rx MSDU in reassembly
    //
    if (Hw->RxInfo.TotalRxMSDUInReassembly == 0)
    {
        return NULL;
    }
    else
    {
        msdu = Hw->RxInfo.MRUReassemblyRxMSDU;
    }
    if (msdu && HW_REASSEMBLY_RX_MSDU_MATCH(msdu, 
                SequenceNumber, &FragmentHdr->Address2))
    {
        //
        // Match found. Optimal search.
        //
        return msdu;
    }
    else
    {
        //
        // We will have to go through the reassembly line and
        // search manually
        //
        for(i = 0; i < HW_MAX_REASSEMBLY_LINE_SIZE; i++)
        {
            msdu = Hw->RxInfo.ReassemblyLine[i];
            if (msdu && HW_REASSEMBLY_RX_MSDU_MATCH(msdu, 
                        SequenceNumber, &FragmentHdr->Address2))
            {
                //
                // Found the match through manual search
                // Set this as the MRU and return it
                //
                MpTrace(COMP_RECV, DBG_LOUD,  ("*** UNOPTIMAL SRCH ***\n"));
                Hw->RxInfo.MRUReassemblyRxMSDU = msdu;
                return msdu;
            }
        }

        return NULL;
    }
}



VOID
HwRemoveMSDUFromReassemblyLine(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu
    )
{
    //
    // The position of this Rx MSDU in the Reassembly line is
    // stored in the MSDU. Use that to quickly remove the
    // Rx MSDU from reassembly line
    //
    MPASSERT(Hw->RxInfo.ReassemblyLine[Msdu->ReassemblyLinePosition] == Msdu);
    Hw->RxInfo.ReassemblyLine[Msdu->ReassemblyLinePosition] = NULL;
    Hw->RxInfo.TotalRxMSDUInReassembly--;

    //
    // If this is the MRU Reassembly MSDU, clear that
    //
    if (Hw->RxInfo.MRUReassemblyRxMSDU == Msdu)
        Hw->RxInfo.MRUReassemblyRxMSDU = NULL;

    MPASSERT(Msdu->Next == NULL);
}


__inline VOID
HwExpireReassemblingMSDUs(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    ULONG                       i, numReassemblies;
    PHW_RX_MSDU                 msdu;
    LARGE_INTEGER               currentTickCount;

    numReassemblies = Hw->RxInfo.TotalRxMSDUInReassembly;   // for optimization
    
    //
    // Get the current time
    //
    KeQueryTickCount(&currentTickCount);
    
    for(i=0; ((numReassemblies > 0) && (i < HW_REASSEMBLY_LINE_LOW_RESOURCES_THRESHOLD)); i++)
    {
        if ((msdu = Hw->RxInfo.ReassemblyLine[i]) != NULL)
        {
            //
            // If this packet has exceeded the MaxRxLifetime, expire it
            //
            if (HW_GET_RX_EXPIRATION_TIME(msdu) < (ULONGLONG)currentTickCount.QuadPart)
            {
                MpTrace(COMP_RECV, DBG_NORMAL,  ("Expiring Rx MSDU with Seq: %d\n", msdu->SequenceNumber));
                HwRemoveMSDUFromReassemblyLine(Hw, msdu);
                HwReturnRxMSDU(Hw, msdu, DispatchLevel);
                Hw->Stats.NumRxReassemblyError++;
            }

            //
            // We found one more Reassembly.
            //
            numReassemblies--;
        }
    }
}

VOID
HwFlushMSDUReassemblyLine(
    _In_  PHW                     Hw
    )
{
    ULONG                       i;
    PHW_RX_MSDU                 msdu;
    
    for(i=0; i < HW_MAX_REASSEMBLY_LINE_SIZE; i++)
    {
        if ((msdu = Hw->RxInfo.ReassemblyLine[i]) != NULL)
        {
            HwRemoveMSDUFromReassemblyLine(Hw, msdu);
            HwReturnRxMSDU(Hw, msdu, FALSE);
            Hw->Stats.NumRxReassemblyError++;
        }
    }
}

PHW_RX_MSDU
HwBuildMSDUForMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PHW_RX_MSDU                 msdu = NULL;
    PDOT11_MGMT_DATA_MAC_HEADER fragmentHeader;
    DOT11_SEQUENCE_CONTROL      sequenceControl;
    
    //
    // If we are in safe mode or in netmon mode, we dont need to
    // do any reassembly. We just need to convert the MPDU to MSDU
    // and we are done
    //
    if (Hw->MacState.NetmonModeEnabled || Hw->MacState.SafeModeEnabled)
    {
        // Create a MSDU and we are done
        msdu = HwAllocateRxMSDU(Hw);
        if (msdu == NULL)
        {
            // Unable to allocate a MSDU
            MpTrace(COMP_RECV, DBG_LOUD,  ("Unable to allocate RX MSDU for received fragment\n"));
            HwReturnRxMPDU(Hw, Mpdu, TRUE);
            return NULL;    // NDIS_STATUS_RESOURCES
        }

        // Populate the MPDU into the MSDU
        msdu->MpduList[0] = Mpdu;
        msdu->MpduCount++;

        return msdu;
    }

    //
    // Else we attempt fragment reassembly
    //
    fragmentHeader = (PDOT11_MGMT_DATA_MAC_HEADER)Mpdu->DataStart;

    //
    // If this is a new fragment for a new frame, we need an
    // Rx MSDU for it. We need sequence number to determine this.
    // Control frames will not have Sequence Control field.
    // 
    if (fragmentHeader->FrameControl.Type != DOT11_FRAME_TYPE_CONTROL)
    {
        NdisMoveMemory(&sequenceControl.usValue, &fragmentHeader->SequenceControl, 2);
    }
    else
    {
        sequenceControl.usValue = 0;
    }

    if (sequenceControl.FragmentNumber == 0)
    {
        //
        // This is a new fragment. Allocate an Rx MSDU for it.
        //
        msdu = HwAllocateRxMSDU(Hw);
        if (msdu == NULL)
        {
            // Unable to allocate a MSDU
            MpTrace(COMP_RECV, DBG_LOUD,  ("Unable to allocate RX MSDU for received fragment\n"));
            HwReturnRxMPDU(Hw, Mpdu, TRUE);
            return NULL;    // NDIS_STATUS_RESOURCES;
        }
        
        //
        // Add the received fragment to this MSDU.
        //
        ndisStatus = HwAddRxMPDUToMSDU(msdu, Mpdu, fragmentHeader,(UCHAR)sequenceControl.FragmentNumber);
        if(ndisStatus == NDIS_STATUS_PENDING)
        {
            MPASSERT(fragmentHeader->FrameControl.Type == DOT11_FRAME_TYPE_DATA);

            //
            // We expect more fragments to arrive for this MSDU.
            // Save the MAC address in MSDU. Need to avoid confusion if two
            // different stations transmit MSDUs with the same sequence number.
            //
            NdisMoveMemory(
                &msdu->PeerAddress,
                &fragmentHeader->Address2,
                sizeof(DOT11_MAC_ADDRESS)
                );
                
            //
            // Also save the sequence number in the MSDU
            //
            msdu->SequenceNumber = sequenceControl.SequenceNumber;
            
            //
            // Add this Rx MSDU into the Reassembly Line
            //
            if (HwAddPartialMSDUToReassemblyLine(Hw, msdu, TRUE) != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_RECV, DBG_LOUD, ("Failed to find a place in Reassembly line. Dropping frame\n"));

                // Return the MSDU
                HwReturnRxMSDU(Hw, msdu, TRUE);
                Hw->Stats.NumRxReassemblyError++;                
                return NULL; // NDIS_STATUS_RESOURCES;
            }

            //
            // More fragments expected. Its already added to the list
            //
            return NULL; // NDIS_STATUS_PENDING;
        }
        else if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_RECV, DBG_LOUD,  ("Dropping duplicate or out of order fragment\n"));
            Hw->Stats.NumRxReassemblyError++;
            // Free the newly allocated MSDU
            HwReturnRxMSDU(Hw, msdu, TRUE);

            // And the current MPDU that we failed to add to the MSDU
            HwReturnRxMPDU(Hw, Mpdu, TRUE);
            
            return NULL; // FAILURE;
        }
        else
        {
            //
            // Fragment is the only one 
            //
            return msdu;
        }
    }
    else
    {
        //
        // This is potentially a fragment belonging to an Rx MSDU in reassembly
        // Find the Rx MSDU it belongs to.
        //
        msdu = HwFindPartialMSDUInReassemblyLine(Hw, fragmentHeader, sequenceControl.SequenceNumber);
        if (msdu == NULL)
        {
            //
            // There is no Rx MSDU is reassembly for this fragment. Return it.
            // This implies first fragment was never received successfully
            // or that there is a malicious station on the BSS
            //
            MpTrace(COMP_RECV, DBG_LOUD,  ("Could not find Rx MSDU for seq num: %d\n", sequenceControl.SequenceNumber));
            HwReturnRxMPDU(Hw, Mpdu, TRUE);
            Hw->Stats.NumRxReassemblyError++;
            return NULL; // NDIS_STATUS_NOT_ACCEPTED;
        }
        
        //
        // Add this fragment to the MSDU
        //
        ndisStatus = HwAddRxMPDUToMSDU(msdu, Mpdu, fragmentHeader, (UCHAR)sequenceControl.FragmentNumber);
        switch(ndisStatus)
        {
            case NDIS_STATUS_SUCCESS:
                //
                // The MSDU is complete. We will be indicating this Rx MSDU up.
                // Remove from reassembly line
                //
                HwRemoveMSDUFromReassemblyLine(Hw, msdu);
                return msdu;

            case NDIS_STATUS_PENDING:
                //
                // There are more fragments to come. We dont indicate this up
                //
                return NULL;

            case NDIS_STATUS_NOT_ACCEPTED:
                //
                // This fragment could not be accepted. Return it to hardware.
                //
                MpTrace(COMP_RECV, DBG_LOUD,  ("Dropping the fragment received\n"));
                HwReturnRxMPDU(Hw, Mpdu, TRUE);
                return NULL;

            case NDIS_STATUS_FAILURE:
                //
                // The Rx MSDU has failed. We need to drop the entire frame.
                //
                MpTrace(COMP_RECV, DBG_LOUD,  ("Dropping the entire 802.11 frame in reassembly\n"));
                HwRemoveMSDUFromReassemblyLine(Hw, msdu);
                HwReturnRxMSDU(Hw, msdu, TRUE);

                // Free the MPDU also. Its not yet added to the MSDU
                HwReturnRxMPDU(Hw, Mpdu, TRUE);
                Hw->Stats.NumRxReassemblyError++;
                return NULL;

            default:
                MPASSERT(FALSE);
        }

        return NULL;
    }
    
}

VOID
HwWaitForPendingReceives(
    _In_  PHW                     Hw,
    _In_opt_ PHW_MAC_CONTEXT      MacContext
    )
{
    UCHAR                       i;

    //
    // Wait for receives to return
    //
    if (MacContext != NULL)
    {
        // Only wait for receives from this MAC_CONTEXT to return
        while (MacContext->RecvRefCount > 0)
            NdisMSleep(1000);
    }
    else
    {
        for (i = 0; i < HW_MAX_NUMBER_OF_MAC; i++)
        {
            while (Hw->MacContext[i].RecvRefCount > 0)
                NdisMSleep(1000);
        }
    }
}

VOID
HwResetReceiveEngine(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    PHW_RX_MPDU                 mpdu;
    LONG                        loopCount = 0;

    if (HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE) )
    {
        return;
    }

    // Note that we may be reset while we have packets pending at the
    // upper layer. That is OK.

    // Note: At this point, the interrupt may be running. So
    // stuff here needs to be done with the lock held
    HW_RX_ACQUIRE_LOCK(Hw, DispatchLevel);    

    //
    // Reset the descriptors from the hardware
    //
    if (HalResetRxDescs(Hw->Hal) != NDIS_STATUS_SUCCESS)
    {
        HW_RX_RELEASE_LOCK(Hw, DispatchLevel); 
        // MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("HalResetRxDescs failed\n"));
        return;
    }
    
    //
    // Resubmit each of the available MPDUs to the hardware again
    //
    mpdu = (PHW_RX_MPDU)MP_PEEK_LIST_HEAD(&(Hw->RxInfo.AvailableMPDUList));    
    while (mpdu)
    {
        if ((PVOID)mpdu == (PVOID)&Hw->RxInfo.AvailableMPDUList)
        {
            // Reached the end of the list
            break;
        }
        
        //
        // Return this to the HAL
        //
        HalReturnRxDesc(Hw->Hal,
            MAX_TX_RX_PACKET_SIZE,
            (PUCHAR)mpdu->BufferVa,
            mpdu->BufferPa,
            &mpdu->DescIter
            );

        mpdu = (PHW_RX_MPDU)MP_PEEK_LIST_HEAD(&mpdu->ListEntry);
        loopCount++;
    }
    MPASSERT(Hw->RxInfo.NumMPDUAvailable == loopCount);
    
    HW_RX_RELEASE_LOCK(Hw, DispatchLevel);    
}

    
VOID
HwHandleReceiveInterrupt(
    _In_  PHW                     Hw,
    _In_  ULONG                   MaxNblsToIndicate
    )
{
    ULONG                       numMSDUToIndicate = 0;
    PHW_RX_MPDU                 mpdu;
    PHW_RX_MSDU                 msdu, msduToIndicate = NULL, lastMsduToIndicate = NULL;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    //
    // We have to make sure that the we can receive right now. If a Pause or reset
    // is in progress, we should not indicate packets up to the protocol anymore.
    // Since this function is not protected by a spinlock (for perf reasons), we have
    // to use Ref Counts to make sure that this DPC is not running if Pause or reset
    // etc. is running.
    //
    HW_INCREMENT_ACTIVE_OPERATION_REF(Hw);

    if (HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_RECEIVE_FLAGS))
    {
        //
        // While a context switch or a reset or pause is in progress, we dont want 
        // to do receive indications.
        //
        HW_DECREMENT_ACTIVE_OPERATION_REF(Hw);    
        return;
    }
    
    while (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_RECEIVE_FLAGS) && 
           (numMSDUToIndicate < MaxNblsToIndicate) &&
           (Hw->RxInfo.NumMPDUAvailable > 0) &&
           (HwIsReceiveAvailable(Hw, TRUE))
           )
    {

        //
        // Get the receive MPDU 
        //
        mpdu = HwGetReceivedMPDU(Hw);
        MPASSERT(mpdu != NULL);

       
        // Perform processing on this MPDU so that it is ready for receive indication
        ndisStatus = HwPrepareReceivedMPDU(Hw, mpdu);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            HwReturnRxMPDU(Hw, mpdu, TRUE);
            continue;
        }

        // This is a good packet. Lets create an MSDU for this MPDU
        msdu = HwBuildMSDUForMPDU(Hw, mpdu);
        if (msdu == NULL)
        {
            // Either this is part of a partial MSDU or was a bad
            // MPDU
            continue;
        }

        // Now we have an MSDU, lets prepare the MSDU for indication
        ndisStatus = HwPrepareReceivedMSDU(Hw, msdu);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            HwReturnRxMSDU(Hw, msdu, TRUE);
            continue;
        }

        // This needs to be indicated up to the ports
        numMSDUToIndicate++;
        if (msduToIndicate == NULL)
            msduToIndicate = msdu;
        else
            lastMsduToIndicate->Next = msdu;

        lastMsduToIndicate = msdu;

    }

    //
    // Periodically, run through the list of Rx MSDUs in reassembly and drop
    // any which have expired. This is an effort to get rid of any orphan
    // Rx MSDUs stuck in reassembly. Watch Dog timer solution is an overkill
    // and CheckForHang can cause sync issues on multiproc machines.
    //
    if (Hw->RxInfo.ReassemblyLineCleanupCountdown-- == 0)
    {
        HwExpireReassemblingMSDUs(Hw, TRUE);
        Hw->RxInfo.ReassemblyLineCleanupCountdown = HW_REASSEMBLY_CLEANUP_COUNTDOWN_VALUE;
    }    

    if (numMSDUToIndicate > 0)
    {
        // We are ready to indicate this MSDU to the upper layer

        //
        // If the number of free descriptors is getting low, indicate this
        // packet with low resources set.
        //
        if (Hw->RxInfo.NumMPDUAvailable < HW_RX_MSDU_LOW_RESOURCE_THRESHOLD)
        {
            HwIndicateReceivedMSDUs(Hw, 
                msduToIndicate, 
                NDIS_RECEIVE_FLAGS_DISPATCH_LEVEL | NDIS_RECEIVE_FLAGS_RESOURCES
                );

            //
            // And grow the size of the MPDU pool
            //
            
            //
            // We must not exceed the upper limit on Num Rx MSDU set by User
            //
            if (Hw->RxInfo.NumMPDUAllocated < (LONG)Hw->RegInfo.NumRXBuffersUpperLimit)
            {
                //
                // Return status is not important.
                //
                HwGrowMPDUPool(
                    Hw,
                    MIN(HW_RX_MSDU_GROW_POOL_SIZE, (Hw->RegInfo.NumRXBuffersUpperLimit - Hw->RxInfo.NumMPDUAllocated))
                    );
            }
            
            //
            // Start the sampling again. This disables the shrinking algorithm.
            // We don't want to shrink while Rx MSDU resources are running low
            //
            Hw->RxInfo.UnusedMPDUListSampleTicks = 0;
            Hw->RxInfo.UnusedMPDUListSampleCount = 0;
        }
        else
        {
            HwIndicateReceivedMSDUs(Hw, 
                msduToIndicate, 
                NDIS_RECEIVE_FLAGS_DISPATCH_LEVEL
                );
        }
    }

    HW_DECREMENT_ACTIVE_OPERATION_REF(Hw);    
}


/**
 * This function is called by NDIS when the protocol above returns NetBufferLists
 * previously indicated by this miniport.
 * 
 * \param MiniportAdapterContext    The adapter context for this miniport
 * \param NetBufferLists            The NBLs that was previously indicated to NDIS
 * \param ReturnFlags               Flags for return information (dispatch level, etc)
 * \sa Hw11ReturnFragment
 */
VOID 
Hw11ReturnPackets(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReturnFlags
    )
{
    PMP_RX_MSDU                 currentMsdu = PacketList;
    PHW_RX_MSDU                 hwMsdu;
    
    while (currentMsdu != NULL)
    {
        // Save the next since we are going to delete this packet
        PacketList = MP_RX_MSDU_NEXT_MSDU(currentMsdu);
        MP_RX_MSDU_NEXT_MSDU(currentMsdu) = NULL;

        hwMsdu = currentMsdu->HwMsdu;

        HwReturnRxMSDU(MacContext->Hw, 
            hwMsdu, 
            (NDIS_TEST_RETURN_AT_DISPATCH_LEVEL(ReturnFlags) ? TRUE : FALSE)
            );
        
        // Remove the ref added on indication
        HW_REMOVE_MAC_CONTEXT_RECV_REF(MacContext, 1);

        currentMsdu = PacketList;
    }
}





