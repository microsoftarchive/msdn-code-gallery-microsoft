/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_send.c

Abstract:
    ExtAP send processing functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    10-15-2007    Created

Notes:

--*/
#include "precomp.h"

#if DOT11_TRACE_ENABLED
#include "ap_send.tmh"
#endif

// Caller of this API needs to ensure that this call returns before
// a pause is initiated
NDIS_STATUS
ApSendMgmtPacket(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(MgmtPacketSize) PUCHAR MgmtPacket,
    _In_ USHORT MgmtPacketSize
    )
{
    return BasePortSendInternalPacket(AP_GET_MP_PORT(ApPort), MgmtPacket, MgmtPacketSize);
}

/** 
 * Called with PORT::Lock held
 */
NDIS_STATUS 
Ap11SendEventHandler(
    _In_ PMP_PORT Port,
    _In_ PMP_TX_MSDU   PacketList,
    _In_ ULONG SendFlags
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PAP_ASSOC_MGR assocMgr = AP_GET_ASSOC_MGR(MP_GET_AP_PORT(Port));
    PMP_TX_MSDU   currentPacket;
    PDOT11_MAC_HEADER currentPacketMacHeader;
    PMAC_HASH_ENTRY macEntry;
    PAP_STA_ENTRY staEntry;
    DOT11_ASSOCIATION_STATE assocState;
    DOT11_FRAME_CLASS frameClass;
    MP_RW_LOCK_STATE lockState;
    //
    // The following boolean is used to improve the performance w.r.t. lock
    //
    BOOLEAN holdReadLock = FALSE;
    
    UNREFERENCED_PARAMETER(SendFlags);
    
    // 
    // Reference AP port first
    //
    ApRefPort(MP_GET_AP_PORT(Port));
    
    do
    {
        // 
        // Only process packets when AP is started
        //
        if (ApGetState(MP_GET_AP_PORT(Port)) != AP_STATE_STARTED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            break;
        }
        
        // 
        // Process each of the packets internally
        //

        for (currentPacket = PacketList;
             currentPacket != NULL;
             currentPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket)
             )
        {
            currentPacketMacHeader = (PDOT11_MAC_HEADER)MP_TX_MPDU_DATA(MP_TX_MSDU_MPDU_AT(PacketList, 0), sizeof(DOT11_MAC_HEADER));

            if (NULL == currentPacketMacHeader)
            {
                DbgBreakPoint();
            }
            
            //
            // Broadcast packet is always allowed
            //
            if (DOT11_IS_BROADCAST(&currentPacketMacHeader->Address1))
            {
                continue;
            }
            
            //
            // Need to hold a read lock on the Mac table to check the 
            // association state of the destination station.
            // We will update statistics in send completion events.
            //
            if (!holdReadLock)
            {
                MP_ACQUIRE_READ_LOCK(&assocMgr->MacHashTableLock, &lockState);
                holdReadLock = TRUE;
            }
            
            //
            // Lookup the station entry from Mac table if it exists
            //
            macEntry = LookupMacHashTable(
                        &assocMgr->MacHashTable, 
                        &currentPacketMacHeader->Address1
                        );

            // 
            // Get the station association state
            //
            if (macEntry != NULL)
            {
                staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);
                assocState = ApGetStaAssocState(staEntry);
            }
            else
            {
                assocState = dot11_assoc_state_unauth_unassoc;
            }

            // 
            // Get frame class
            //
            frameClass = Dot11GetFrameClass(&currentPacketMacHeader->FrameControl);

            switch(frameClass)
            {
                case DOT11_FRAME_CLASS_1:
                    //
                    // Class 1 frames are always allowed
                    //
                    break;

                case DOT11_FRAME_CLASS_2:
                    //
                    // Class 2 frames are allowed if the station is authenticated
                    //
                    if (dot11_assoc_state_unauth_unassoc == assocState)
                    {
                        MP_TX_MSDU_STATUS(currentPacket) = NDIS_STATUS_INVALID_STATE;
                    }
                    break;

                case DOT11_FRAME_CLASS_3:
                    //
                    // Class 3 frames are allowed if the station is associated
                    //
                    if (assocState != dot11_assoc_state_auth_assoc)
                    {
                        MP_TX_MSDU_STATUS(currentPacket) = NDIS_STATUS_INVALID_STATE;
                    }
                    break;

                default:
                    MP_TX_MSDU_STATUS(currentPacket) = NDIS_STATUS_INVALID_STATE;
            }
        }

        //
        // Release the read lock
        //
        if (holdReadLock)
        {
            MP_RELEASE_READ_LOCK(&assocMgr->MacHashTableLock, &lockState);
        }
        
    } while (FALSE);

    // 
    // Dereference AP port
    //
    ApDerefPort(MP_GET_AP_PORT(Port));
    
    return ndisStatus;
}

/**
 * Handle send completion event
 */
VOID 
Ap11SendCompleteEventHandler(
    _In_ PMP_PORT Port,
    _In_ PMP_TX_MSDU   PacketList,
    _In_ ULONG SendCompleteFlags
    )
{
    PAP_ASSOC_MGR assocMgr = AP_GET_ASSOC_MGR(MP_GET_AP_PORT(Port));
    PMP_TX_MSDU   currentPacket;
    PDOT11_MAC_HEADER currentPacketMacHeader;
    PMAC_HASH_ENTRY macEntry;
    PAP_STA_ENTRY staEntry;
    MP_RW_LOCK_STATE lockState;
    
    UNREFERENCED_PARAMETER(SendCompleteFlags);
    
    // 
    // Reference AP port first
    //
    ApRefPort(MP_GET_AP_PORT(Port));
    
    do
    {
        // 
        // Only process send completion when AP is started
        //
        if (ApGetState(MP_GET_AP_PORT(Port)) != AP_STATE_STARTED)
        {
            break;
        }
        
        // 
        // Process each of the packets internally
        // Need to hold a read lock on the Mac table.
        // We don't hold a write lock because we can tolerate
        // inconsistency in statistics to achieve better performance.
        //

        MP_ACQUIRE_READ_LOCK(&assocMgr->MacHashTableLock, &lockState);
        
        for (currentPacket = PacketList;
             currentPacket != NULL;
             currentPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket)
             )
        {
            currentPacketMacHeader = (PDOT11_MAC_HEADER)MP_TX_MPDU_DATA(MP_TX_MSDU_MPDU_AT(PacketList, 0), sizeof(DOT11_MAC_HEADER));

            if (NULL == currentPacketMacHeader)
            {
                DbgBreakPoint();
            }
            
            //
            // Broadcast packet is ignored
            //
            if (DOT11_IS_BROADCAST(&currentPacketMacHeader->Address1))
            {
                continue;
            }
            
            //
            // Lookup the station entry from Mac table if it exists
            //
            macEntry = LookupMacHashTable(
                        &assocMgr->MacHashTable, 
                        &currentPacketMacHeader->Address1
                        );

            if (NULL == macEntry)
            {
                //
                // The station has left.
                //
                continue;
            }

            //
            // Get the station entry
            //
            staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);

            if (NDIS_STATUS_SUCCESS == MP_TX_MSDU_STATUS(currentPacket))
            {
                staEntry->Statistics.ullTxPacketSuccessCount++;
            }
            else
            {
                staEntry->Statistics.ullTxPacketFailureCount++;
            }
            
        }

        //
        // Release the read lock
        //
        MP_RELEASE_READ_LOCK(&assocMgr->MacHashTableLock, &lockState);
        
    } while (FALSE);

    // 
    // Dereference AP port
    //
    ApDerefPort(MP_GET_AP_PORT(Port));
    
}

#if 0

BOOLEAN
ApCanTransmit(
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    UNREFERENCED_PARAMETER(ApPort);
    
    //
    // TODO: If we are in the middle of a scan, this send must be queued
    //

    return TRUE;
}

NDIS_STATUS
ApGetPortStatus(
    _In_ PMP_PORT Port
    )
{
    NDIS_STATUS ndisStatus;

    if (MP_TEST_PORT_STATUS(Port, MP_PORT_PAUSED))
        ndisStatus = NDIS_STATUS_PAUSED;
    else if (MP_TEST_PORT_STATUS(Port, MP_PORT_PAUSING))
        ndisStatus = NDIS_STATUS_PAUSED;
    else if (MP_TEST_PORT_STATUS(Port, MP_PORT_IN_RESET))
        ndisStatus = NDIS_STATUS_RESET_IN_PROGRESS;
    else if (MP_TEST_PORT_STATUS(Port, MP_PORT_HALTING))
        ndisStatus = NDIS_STATUS_CLOSING;
    else
        ndisStatus = NDIS_STATUS_FAILURE;       // return a generic error

    return ndisStatus;
}

// Called with PORT::Lock held
VOID
ApProcessQueuedTxPackets(
    _In_ PMP_PORT Port,
    _In_ ULONG SendFlags
    )
{
    BOOLEAN forwardToHardware;
    PMP_TX_MSDU   currentPacket, prevPacketToForward = NULL, packetListToForward = NULL;
    ULONG count = 0;

    // PORT_LOCK must be held

    // TODO: If I am pausing, this flag is set. We would not
    // be processing any pending packets. Ensure that these packets get flushed on a pause
    
    // then not process these pending packets. So then
    if (MP_TEST_PORT_STATUS(Port, MP_PORT_CANNOT_SEND_MASK))
    {   
        // We dont do anything
        return;
    }

    while ((!MpPacketQueueIsEmpty(&Port->PendingTxQueue) &&
            (count < MAX_SEND_MSDU_TO_PROCESS)))
    {
        //
        // Dequeue the first packet from the list
        //
        currentPacket = MpDequeuePacket(&Port->PendingTxQueue);
        count++;
        
        //
        // Now determine what to do with this TX_PACKET. We either would 
        // pass it to the hardware or hold it (if we are scanning, or we have
        // previously pended packets, etc)
        //
        if (ApCanTransmit(MP_GET_AP_PORT(Port)) &&
            VNic11CanTransmit(PORT_GET_VNIC(Port)))
        {
            //
            // Can send this packet to the hardware
            //
            forwardToHardware = TRUE;
        }
        else
        {
            //
            // Some reason we cannot submit this packet to the
            // HW yet. Queue it in the pending Tx queue
            //
            forwardToHardware = FALSE;
        }

        if (forwardToHardware)
        {
            // Add this to the end of the chain we are forwarding to the HW
            if (packetListToForward == NULL)
            {
                packetListToForward = currentPacket;
            }
            else
            {
                MP_TX_MSDU_NEXT_MSDU(prevPacketToForward) = currentPacket;
            }
            prevPacketToForward = currentPacket;

            //
            // Increment the counter for the number of packets we have submitted 
            // to the hardware. This would block the port from pausing, etc
            //
            PORT_INCREMENT_PNP_REFCOUNT(Port);
        }
        else
        {
            //
            // Put the packet back at the head of the packet queue
            //
            MpQueuePacketPriority(&Port->PendingTxQueue, currentPacket);
            break;
        }
    }

    if (packetListToForward != NULL)
    {
        // Forward this list to the VNIC
        MP_RELEASE_PORT_LOCK(Port, NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags));
        VNic11SendPackets(PORT_GET_VNIC(Port), packetListToForward, SendFlags);
        MP_ACQUIRE_PORT_LOCK(Port, NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags));
    }
}


VOID 
Ap11SendCompleteHandler(
    _In_ PMP_PORT Port,
    _In_ PMP_TX_MSDU   PacketList,
    _In_ ULONG SendCompleteFlags
    )
{
    PMP_TX_MSDU   currentPacket = PacketList;
    PNET_BUFFER_LIST currentNetBufferList, prevNetBufferList = NULL;
    PNET_BUFFER_LIST netBufferListsToComplete = NULL;

    UNREFERENCED_PARAMETER(SendCompleteFlags);

    while (currentPacket != NULL)
    {
        // Save the next since we are going to delete this packet
        PacketList = MP_TX_MSDU_NEXT_MSDU(currentPacket);
        MP_TX_MSDU_NEXT_MSDU(currentPacket) = NULL;

        //
        // Remove the refcount added during the send
        //
        PORT_DECREMENT_PNP_REFCOUNT(Port);        

        if (MP_TX_MSDU_WRAPPED_NBL(currentPacket) == NULL)
        {
            // Internal packet submitted from BasePortSendInternalPacket, free our structures
            MP_FREE_MEMORY(MP_TX_MSDU_MPDU_AT(currentPacket, 0)->InternalSendData);
        }
        else
        {
            //
            // There were from the OS. We need to convert back to NBLs and complete them to
            // the OS
            //
            currentNetBufferList = currentPacket->NetBufferList;
            NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList) = NULL;
            NET_BUFFER_LIST_STATUS(currentNetBufferList) = currentPacket->Status;

            if (netBufferListsToComplete == NULL)
            {
                netBufferListsToComplete = currentNetBufferList;
            }
            else
            {
                NET_BUFFER_LIST_NEXT_NBL(prevNetBufferList) = currentNetBufferList;
            }
            
            prevNetBufferList = currentNetBufferList;
        }
        
        currentPacket = PacketList;
    }

    // Free our packet wrapper structures
    BasePortFreeTranslatedTxPackets(Port, PacketList);

    if (netBufferListsToComplete != NULL)
    {
        // Complete the NBLs back to NDIS
        NdisMSendNetBufferListsComplete(
            Port->MiniportAdapterHandle,
            netBufferListsToComplete,
            SendCompleteFlags
            );
    }

    //
    // Quick check to determine if we have pending TX packets queued and whether
    // we should process them
    //
    if (!MpPacketQueueIsEmpty(&Port->PendingTxQueue))
    {
        //
        // We have pending packets that may need to get get processed
        //
        MP_ACQUIRE_PORT_LOCK(Port, NDIS_TEST_SEND_COMPLETE_AT_DISPATCH_LEVEL(SendCompleteFlags));

        // Process pending packets queue
        ApProcessQueuedTxPackets(
            Port, 
            (NDIS_TEST_SEND_COMPLETE_AT_DISPATCH_LEVEL(SendCompleteFlags) ? NDIS_SEND_FLAGS_DISPATCH_LEVEL : 0)
            );
        MP_RELEASE_PORT_LOCK(Port, NDIS_TEST_SEND_COMPLETE_AT_DISPATCH_LEVEL(SendCompleteFlags));
    }

}

VOID 
Ap11SendNBLHandler(
    _In_ PMP_PORT Port,
    _In_ PNET_BUFFER_LIST NetBufferLists,
    _In_ ULONG SendFlags
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_STATUS failedCompletionStatus = NDIS_STATUS_FAILURE;
    PMP_TX_MSDU   currentPacket;
    PNET_BUFFER_LIST currentNetBufferList, nextNetBufferList, failedNBLs = NULL;

    // We do all our send processing with the lock held
    MP_ACQUIRE_PORT_LOCK(Port, NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags));
    
    do
    {
        //
        // Check if we are in a valid send state
        //
        if (MP_TEST_PORT_STATUS(Port, MP_PORT_CANNOT_SEND_MASK))
        {            
            MP_RELEASE_PORT_LOCK(Port, NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags));
            MpTrace(COMP_SEND, DBG_NORMAL, ("Sends failed as port is not in a valid send state\n"));
            failedNBLs = NetBufferLists;
            failedCompletionStatus = ApGetPortStatus(Port);
            break;
        }

        // We process each NBL individually
        currentNetBufferList = NetBufferLists;

        while (currentNetBufferList != NULL)
        {
            //
            // Cache a reference to the next NBL right away. We will process one NBL
            // at a time so the CurrentNBL next pointer will be set to NULL.
            //
            nextNetBufferList = NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList);
            NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList) = NULL;

            // Translate this single NBL to the MP_TX_MSDU  
            ndisStatus = BasePortTranslateTxNBLsToTxPackets(
                            Port,
                            currentNetBufferList,
                            &currentPacket
                            );
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                // Add this to the failed list
                NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList) = failedNBLs;
                failedNBLs = currentNetBufferList;
                failedCompletionStatus = ndisStatus;
            }

            //
            // Queue this packet at the end of our send queue. We will empty the
            // queue in one shot later
            //
            MpQueuePacket(&Port->PendingTxQueue, currentPacket);

            currentNetBufferList = nextNetBufferList;
            
        }

        //
        // Now process all the queued packets (lock still held to ensure that we
        // dont let pause, etc to complete without completing the pending packets)
        //
        ApProcessQueuedTxPackets(Port, SendFlags);
        
        // Release the port lock
        MP_RELEASE_PORT_LOCK(Port, NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags));

    } while (FALSE);


    if (failedNBLs != NULL)
    {
        // We just complete the NBLs back to NDIS
        for(currentNetBufferList = failedNBLs;
            currentNetBufferList != NULL;
            currentNetBufferList = NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList))
        {
            NET_BUFFER_LIST_STATUS(currentNetBufferList) = failedCompletionStatus;
        }

        NdisMSendNetBufferListsComplete(
            Port->MiniportAdapterHandle,
            failedNBLs,
            (NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags) ? NDIS_SEND_COMPLETE_FLAGS_DISPATCH_LEVEL : 0)
            );

    }
}
#endif
