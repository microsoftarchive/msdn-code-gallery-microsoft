/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    base_port_send.c

Abstract:
    Implements the send functionality for the base port class
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "port_defs.h"
#include "base_port.h"
#include "base_port_intf.h"
#include "vnic_intf.h"

#if DOT11_TRACE_ENABLED
#include "base_port_send.tmh"
#endif

// Acquires the send token. If the original value was 0, then we successfully acquired the token
__inline
BOOLEAN PORT_ACQUIRE_SEND_TOKEN(PMP_PORT Port)
{
    return (InterlockedExchange(&((Port)->SendToken), 0) == 1 ? TRUE : FALSE);
}

// Wait for the IRQL
__inline 
_IRQL_requires_max_(PASSIVE_LEVEL)
BOOLEAN PORT_WAIT_FOR_SEND_TOKEN(PMP_PORT Port, ULONG SleepCount)
{
    while (TRUE)
    {
        if (PORT_ACQUIRE_SEND_TOKEN(Port))
            return TRUE;    // Got the token

        // If we have exhausted waiting, give up
        if (SleepCount == 0)
            return FALSE;

        // Sleep a while & we will try again
        NdisMSleep(10000);

        --SleepCount;
    }
}

// Release the token
__inline
VOID PORT_RELEASE_SEND_TOKEN(PMP_PORT Port)
{
    MPASSERTOP(InterlockedExchange(&Port->SendToken, 1) == 0);
}

NDIS_STATUS 
BasePortSendEventHandler(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendFlags
    )
{
    UNREFERENCED_PARAMETER(Port);
    UNREFERENCED_PARAMETER(PacketList);
    UNREFERENCED_PARAMETER(SendFlags);

    return NDIS_STATUS_SUCCESS;
}

VOID 
BasePortSendCompleteEventHandler(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendCompleteFlags
    )
{
    UNREFERENCED_PARAMETER(Port);
    UNREFERENCED_PARAMETER(PacketList);
    UNREFERENCED_PARAMETER(SendCompleteFlags);
}

NDIS_STATUS
BasePortTranslateTxNBLsToTxPackets(
    _In_  PMP_PORT                Port,
    _In_  PNET_BUFFER_LIST        NetBufferLists,
    _Out_ PMP_TX_MSDU  *          PacketList
    )
{
    PMP_TX_MSDU                 outputPacketList = NULL;
    PMP_TX_MSDU                 currentPacket, prevPacket = NULL;
    PMP_TX_MPDU                 currentFragment;
    USHORT                      fragmentIndex = 0;
    PNET_BUFFER_LIST            currentNetBufferList = NetBufferLists, nextNetBufferList;
    PNET_BUFFER                 currentNetBuffer;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_EXTSTA_SEND_CONTEXT  osSendContext, mySendContext;  // This is same for ExtAP & ExtSTA

    *PacketList = NULL;

    // Convert each NBL and NB to our structure    
    while (currentNetBufferList != NULL)
    {
        nextNetBufferList = NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList);

        // First the MP_TX_MSDU  
        currentPacket = NdisAllocateFromNPagedLookasideList(&(Port->TxPacketLookaside));
        if (currentPacket == NULL)
        {
            MpTrace(COMP_SEND, DBG_SERIOUS, ("Failed to allocate MP_TX_MSDU   for NET_BUFFER_LIST\n"));        
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        NdisZeroMemory(currentPacket, sizeof(MP_TX_MSDU  ));
        
        // Populate the TX_PACKET
        MP_TX_MSDU_WRAPPED_NBL(currentPacket) = currentNetBufferList;
        // Save the TX_MSDU in the NET_BUFFER_LIST for debugging purpose
        MP_NBL_WRAPPED_TX_MSDU(currentNetBufferList) = currentPacket;

        osSendContext = MP_GET_SEND_CONTEXT(currentNetBufferList);
        mySendContext = MP_TX_MSDU_SEND_CONTEXT(currentPacket);
        NdisMoveMemory(mySendContext, osSendContext, sizeof(DOT11_EXTSTA_SEND_CONTEXT));
        
        if (outputPacketList == NULL)
        {
            outputPacketList = currentPacket;
        }
        else
        {
            MP_TX_MSDU_NEXT_MSDU(prevPacket) = currentPacket;
        }
        // The Next NBL's PACKET would be added after the current NBL's PACKET
        prevPacket = currentPacket;

        // Now we go through the NBs in this NB
        fragmentIndex = 0;
        for (currentNetBuffer = NET_BUFFER_LIST_FIRST_NB(currentNetBufferList);
             currentNetBuffer != NULL;
             currentNetBuffer = NET_BUFFER_NEXT_NB(currentNetBuffer))
        {
            currentFragment = NdisAllocateFromNPagedLookasideList(&(Port->TxFragmentLookaside));
            if (currentFragment == NULL)
            {
                MpTrace(COMP_SEND, DBG_SERIOUS, ("Failed to allocate MP_TX_MPDU     for NET_BUFFER\n"));        
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }
            NdisZeroMemory(currentFragment, sizeof(MP_TX_MPDU    ));

            // Populate the TX_FRAGMENT
            MP_TX_MPDU_WRAPPED_NB(currentFragment) = currentNetBuffer;
            MP_TX_MPDU_MSDU(currentFragment) = currentPacket;

            // Add it to the fragment list of the packet
            MP_TX_MSDU_MPDU_AT(currentPacket, fragmentIndex) = currentFragment;
            fragmentIndex++;
        }
        MP_TX_MSDU_MPDU_COUNT(currentPacket) = fragmentIndex;        
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        currentNetBufferList = nextNetBufferList;
    }


    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (outputPacketList != NULL)
        {
            BasePortFreeTranslatedTxPackets(Port, outputPacketList);
            outputPacketList = NULL;
        }
    }

    *PacketList = outputPacketList;

    return ndisStatus;
}


VOID
BasePortFreeTranslatedTxPackets(
    _In_ PMP_PORT                Port,
    _In_ PMP_TX_MSDU             PacketList
    )
{
    PMP_TX_MSDU                 currentPacket = PacketList;
    ULONG                       i = 0;

    while (currentPacket != NULL)
    {
        // Save the next since we are going to delete this packet
        PacketList = MP_TX_MSDU_NEXT_MSDU(currentPacket);

        // Delete the fragments from this packet
        for (i = 0; i < MP_TX_MSDU_MPDU_COUNT(currentPacket); i++)
        {            
            PMP_TX_MPDU pMPDU = MP_TX_MSDU_MPDU_AT(currentPacket, i);
            NdisFreeToNPagedLookasideList(&(Port->TxFragmentLookaside),                 
                pMPDU);
        }
        
        // Delete the packet
        NdisFreeToNPagedLookasideList(&(Port->TxPacketLookaside), 
            currentPacket);
        
        currentPacket = PacketList;
    }
}

NDIS_STATUS
BasePortGetPortStatus(
    _In_  PMP_PORT                Port
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



VOID
BasePortCompleteFailedPackets(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendCompleteFlags
    )
{
    PMP_TX_MSDU                 currentPacket = PacketList;
    PNET_BUFFER_LIST            currentNetBufferList, prevNetBufferList = NULL;
    PNET_BUFFER_LIST            netBufferListsToComplete = NULL;
    #if DBG
    ULONG ulNumNBLs = 0, ulInternalSends = 0;
    #endif
        

    while (currentPacket != NULL)
    {        
        //
        // No refcount is added yet, so we dont need to remove anything
        //

        if (MP_TX_MSDU_WRAPPED_NBL(currentPacket) == NULL)
        {
            #if DBG
            ulInternalSends++;
            #endif
            // Internal packet submitted from BasePortSendInternalPacket, free the memory
            // we allocated for the buffer
            MP_FREE_MEMORY(MP_TX_MSDU_MPDU_AT(currentPacket, 0)->InternalSendBuffer);
        }
        else
        {
            #if DBG
            ulNumNBLs++;
            #endif
            //
            // There were from the OS. We need to convert back to NBLs and 
            // complete them to the OS
            //

            //
            // Get the NBLs out
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

        currentPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket);
    }

    //
    // Free our packet wrapper structures for all the packets
    //
    BasePortFreeTranslatedTxPackets(Port, PacketList);

    #if DBG
        MpTrace(COMP_SEND, DBG_NORMAL, ("Port(%d): NdisMSendNetBufferListsComplete called with %d NBLs\n", Port->PortNumber, ulNumNBLs));
        MpTrace(COMP_SEND, DBG_NORMAL, ("Port(%d): Completed %d internal sends\n", Port->PortNumber, ulInternalSends));
    #endif        

    if (netBufferListsToComplete != NULL)
    {
        // Complete the NBLs back to NDIS
        NdisMSendNetBufferListsComplete(
            Port->MiniportAdapterHandle,
            netBufferListsToComplete,
            SendCompleteFlags
            );
    }
}

VOID
BasePortFlushQueuedTxPackets(
    _In_  PMP_PORT                Port
    )
{
    PMP_TX_MSDU                 currentPacket, packetListToComplete = NULL;
    ULONG                       count = 0;
    NDIS_STATUS                 completionStatus = BasePortGetPortStatus(Port);

    // Empty the pending TX queue
    MP_ACQUIRE_PORT_LOCK(Port, FALSE);
    while (!MpPacketQueueIsEmpty(&Port->PendingTxQueue))
    {
        currentPacket = MP_MSDU_FROM_QUEUE_ENTRY(MpDequeuePacket(&Port->PendingTxQueue));
        count++;

        MP_TX_MSDU_NEXT_MSDU(currentPacket) = NULL;
        MP_TX_MSDU_STATUS(currentPacket) = completionStatus;

        MP_TX_MSDU_NEXT_MSDU(currentPacket) = packetListToComplete;
        packetListToComplete = currentPacket;
    }
    MP_RELEASE_PORT_LOCK(Port, FALSE);

    if (packetListToComplete != NULL)
    {
        BasePortCompleteFailedPackets(Port, 
            packetListToComplete,
            0
            );
    }    
}

// Called with PORT::Lock held
_IRQL_requires_(DISPATCH_LEVEL)
_Requires_lock_held_((&Port->Lock)->SpinLock)
VOID
BasePortTransmitQueuedPackets(
    _In_  PMP_PORT                Port,
    _In_  ULONG                   SendFlags
    )
{
    PMP_TX_MSDU                 currentPacket, prevPacketToForward = NULL, packetListToForward = NULL;
    ULONG                       count = 0, numPktToFwd = 0;
    NDIS_STATUS                 ndisStatus;
    PMP_TX_MSDU                 packetListToFail = NULL;

    // PORT_LOCK must be held

    // If I am pausing, this flag is set. We would not
    // be processing any pending packets. These packets get flushed on a pause
    
    // then not process these pending packets
    if (MP_TEST_PORT_STATUS(Port, MP_PORT_CANNOT_SEND_MASK))
    {   
        //
        // We dont do anything
        //
        MpTrace(COMP_SEND, DBG_NORMAL, ("Dropping sends as port should not be sending\n"));                
        
        return;
    }

    while ((!MpPacketQueueIsEmpty(&Port->PendingTxQueue) &&
            (count < MAX_SEND_MSDU_TO_PROCESS)))
    {
        //
        // Dequeue the first packet from the list
        //
        currentPacket = MP_MSDU_FROM_QUEUE_ENTRY(MpDequeuePacket(&Port->PendingTxQueue));
        count++;

        // 
        // Let the port pre-process the packets. We only let one
        // packet to get processed at a time
        //
        MP_TX_MSDU_NEXT_MSDU(currentPacket) = NULL;
        MP_TX_MSDU_STATUS(currentPacket) = NDIS_STATUS_SUCCESS;
        ndisStatus = Port11NotifySend(Port, currentPacket, SendFlags);
        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            //
            // Look at the per-packet status values
            //
            ndisStatus = MP_TX_MSDU_STATUS(currentPacket);
            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                //
                // Port is fine with processing these packets. Let us check if we can
                // continue sending this down. We only check with the VNIC
                //
                if (VNic11CanTransmit(PORT_GET_VNIC(Port)) == FALSE)
                {
                    //
                    // Some reason we cannot submit this packet to the
                    // HW yet. Queue it in the pending Tx queue
                    //
                    ndisStatus = NDIS_STATUS_PENDING;

                    //
                    // In this case we indicate the packet to the Port again
                    // the next time we attempt to send. Ensure that the port
                    // can handle that
                    //
                }
            }
        }
        else
        {
            //
            // Port returned PENDING or FAILURE for the packet list. We wont
            // be sending any of these packets
            //
        }
        
        //
        // All the above processing would give us one of the following status codes
        //
        // NDIS_STATUS_SUCCESS - The packets can be processed furthers. In this case
        //                       we forward the packet to the lower layer
        // NDIS_STATUS_PENDING - This packet should not be sent now, but can be sent later
        //                       In this case we requeue the packet and stop processing
        //                       further packets
        // NDIS_STATUS_FAILURE or anything other failure status
        //                     - The packet is not sent and we continue processing
        //                       other packets
        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            numPktToFwd++;
            
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
        else if (ndisStatus == NDIS_STATUS_PENDING)
        {
            //
            // Put the packet back at the head of the packet queue. To avoid out of
            // order delivery we dont go forward and give any more packets to the 
            // lower layer 
            //
            MpQueuePacketPriority(&Port->PendingTxQueue, QUEUE_ENTRY_FROM_MP_MSDU(currentPacket));
            break;
        }
        else
        {
            //
            // Put this packet in the list of packets to be failed
            //
            MpTrace(COMP_SEND, DBG_NORMAL, ("Port or VNic failed sends with status 0x%08x\n", ndisStatus));
            MP_TX_MSDU_STATUS(currentPacket) = ndisStatus;
            MP_TX_MSDU_NEXT_MSDU(currentPacket) = packetListToFail;
            packetListToFail = currentPacket;
        }

    }

    if ((packetListToForward != NULL) || (packetListToFail != NULL))
    {
        // Forward this list to the VNIC
        MP_RELEASE_PORT_LOCK(Port, NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags));
        if (packetListToForward != NULL)
        {
            VNic11SendPackets(PORT_GET_VNIC(Port), packetListToForward, numPktToFwd, SendFlags);
        }
        
        //
        // Complete the failed packets
        //
        if (packetListToFail != NULL)
        {
            BasePortCompleteFailedPackets(Port, 
                packetListToFail,
                NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags) ? NDIS_SEND_COMPLETE_FLAGS_DISPATCH_LEVEL : 0
                );
        }
        
        MP_ACQUIRE_PORT_LOCK(Port, NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags));
    }
}

// Called with PORT::Lock held - This is an optimization since in most cases this would be
// called from BasePortHandleSendNetBufferLists, which already has the lock
_Requires_lock_held_((&Port->Lock)->SpinLock)
_IRQL_requires_(DISPATCH_LEVEL)
VOID
BasePortProcessQueuedPackets(
    _In_  PMP_PORT                Port,
    _In_  ULONG                   SendFlags
    )
{
    // 
    // The logic below is - If we can get the send token, we will process the sends that are 
    // in the queue (through BasePortTransmitQueuedPackets). If we cannot get the send token,
    // we will trigger the deferred send routine to try. Always deferring does not work because
    // we may run into a synchronization window where during 4 way handshake OS sends us a TX 
    // and a KEY back to back & if the TX gets to the H/W after the key, we would end up encrypting
    // the handshake packet when not expected
    //
    if (PORT_ACQUIRE_SEND_TOKEN(Port))
    {
        MpTrace(COMP_TESTING, DBG_NORMAL, ("Port(%d): Processing queued sends\n", Port->PortNumber));
        // Got the token, process sends
        BasePortTransmitQueuedPackets(Port, SendFlags);

        PORT_RELEASE_SEND_TOKEN(Port);
    }
    else
    {
        //
        // Deferred processing is needed
        //
        NdisSetEvent(&Port->DeferredSendTrigger);
    }
}

// Caller of this API needs to ensure that this call returns before
// a pause is initiated
NDIS_STATUS
BasePortSendInternalPacket(
    _In_  PMP_PORT                Port,
    _In_reads_bytes_(PacketLength)  PUCHAR                  PacketData,
    _In_  USHORT                  PacketLength
    )
{
    PMP_TX_MSDU                 txPacket = NULL;
    PMP_TX_MPDU                 txFragment = NULL;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR                      dataBuffer = NULL;   
    // This can be optimized to not do the additional allocation

    do
    {
        // TX Packet
        txPacket = NdisAllocateFromNPagedLookasideList(&(Port->TxPacketLookaside));
        if (txPacket == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        NdisZeroMemory(txPacket, sizeof(MP_TX_MSDU  ));
        
        // TX Fragment
        txFragment = NdisAllocateFromNPagedLookasideList(&(Port->TxFragmentLookaside));
        if (txFragment == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        NdisZeroMemory(txFragment, sizeof(MP_TX_MPDU));

        // Data buffer. We allocate extra space for the backfill that the H/W requires
        MP_ALLOCATE_MEMORY(Port->MiniportAdapterHandle, 
            &dataBuffer, 
            PacketLength + HW11_REQUIRED_BACKFILL_SIZE, 
            EXTSTA_MEMORY_TAG
            );
        if (dataBuffer == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        // Save this internal send buffer
        txFragment->InternalSendBuffer = dataBuffer;

        // Skip the backfill to get to the data start and 
        // Copy the raw packet data into the TX_FRAGMENT structure
        dataBuffer += HW11_REQUIRED_BACKFILL_SIZE;
        NdisMoveMemory(dataBuffer, PacketData, PacketLength);
        
        txFragment->InternalSendDataStart = dataBuffer;
        txFragment->InternalSendLength = PacketLength;
        MP_TX_MPDU_MSDU(txFragment) = txPacket;

        MP_TX_MSDU_MPDU_AT(txPacket, 0) = txFragment;
        MP_TX_MSDU_MPDU_COUNT(txPacket) = 1;

        // These packets are always exempt from encryption. The HW layer
        // makes an assumption about this too & it wont encrypt the packets sent this way
        MP_TX_MSDU_SEND_CONTEXT(txPacket)->usExemptionActionType = DOT11_EXEMPT_ALWAYS;
        
        // Also must be sent using the current phy
        MP_TX_MSDU_SEND_CONTEXT(txPacket)->uPhyId = DOT11_PHY_ID_ANY;
        
        // Increment the refcount for number of packets that are
        // pending
        PORT_INCREMENT_PNP_REFCOUNT(Port);        

        // We can alternately put this into the PendingTxQueue and 
        // let StaProcessQueuedTxPackets do the work (instead of
        // calling VNic directly)

        // Give the packet to the VNIC for sending
        VNic11SendPackets(PORT_GET_VNIC(Port), txPacket, 1, 0);
        
    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (dataBuffer != NULL)
            MP_FREE_MEMORY(dataBuffer);

        if (txFragment != NULL)
            NdisFreeToNPagedLookasideList(&(Port->TxFragmentLookaside), 
                txFragment);

        // Delete the packet
        if (txPacket != NULL)
            NdisFreeToNPagedLookasideList(&(Port->TxPacketLookaside), 
                txPacket);
    }

    return ndisStatus;
}

VOID
BasePortHandleSendNetBufferLists(
    _In_  PMP_PORT                Port,
    _In_  PNET_BUFFER_LIST        NetBufferLists,
    _In_  ULONG                   SendFlags
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_STATUS                 failedCompletionStatus = NDIS_STATUS_FAILURE;
    PMP_TX_MSDU                 currentPacket;
    PNET_BUFFER_LIST            currentNetBufferList, nextNetBufferList, failedNBLs = NULL;
    #if DBG
    ULONG ulNumNBLs = 0;
    #endif

    // We do all our send processing with the lock held
    MP_ACQUIRE_PORT_LOCK(Port, NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags));
    
    do
    {
        //
        // Check for global states under which we cannot be sending packets
        //
        if (MP_TEST_PORT_STATUS(Port, MP_PORT_CANNOT_SEND_MASK))
        {            
            MP_RELEASE_PORT_LOCK(Port, NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags));
            MpTrace(COMP_SEND, DBG_NORMAL, ("Sends failed as port is not in a valid send state\n"));

            // We fail all the packets right here
            failedNBLs = NetBufferLists;
            failedCompletionStatus = BasePortGetPortStatus(Port);
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
                MpTrace(COMP_SEND, DBG_NORMAL, ("Unable to translate TX NBLs to TX MSDUs. Dropping packet\n"));                
            }
            else
            {
                #if DBG
                ulNumNBLs++;
                #endif
                //
                // Queue this packet at the end of our send queue. We will empty the
                // queue in one shot later
                //
                MpQueuePacket(&Port->PendingTxQueue, QUEUE_ENTRY_FROM_MP_MSDU(currentPacket));
            }

            currentNetBufferList = nextNetBufferList;            
        }

        #if DBG
            MpTrace(COMP_SEND, DBG_NORMAL, ("Port(%d): Trigger BasePortTransmitQueuedPackets with %d NBLs\n", Port->PortNumber, ulNumNBLs));
        #endif
        
        //
        // Now attempt to process the queued packets
        //
        BasePortProcessQueuedPackets(Port, SendFlags);
        
        // Release the port lock
        MP_RELEASE_PORT_LOCK(Port, NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags));

    } while (FALSE);


    if (failedNBLs != NULL)
    {
        #if DBG
        ULONG ulNumFailedNBLs = 0;
        #endif
        
        // We just complete the NBLs back to NDIS
        for(currentNetBufferList = failedNBLs;
            currentNetBufferList != NULL;
            currentNetBufferList = NET_BUFFER_LIST_NEXT_NBL(currentNetBufferList))
        {
            #if DBG
            ulNumFailedNBLs++;
            #endif
            NET_BUFFER_LIST_STATUS(currentNetBufferList) = failedCompletionStatus;
        }

        #if DBG
            MpTrace(COMP_SEND, DBG_NORMAL, ("Port(%d): NdisMSendNetBufferListsComplete called with %d NBLs\n", Port->PortNumber, ulNumFailedNBLs));
        #endif
        
        NdisMSendNetBufferListsComplete(
            Port->MiniportAdapterHandle,
            failedNBLs,
            (NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags) ? NDIS_SEND_COMPLETE_FLAGS_DISPATCH_LEVEL : 0)
            );

    }


}

VOID
BasePortSendCompletePackets(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendCompleteFlags
    )
{
    PMP_TX_MSDU                 currentPacket = PacketList, tempNextPacket;
    PNET_BUFFER_LIST            currentNetBufferList, prevNetBufferList = NULL;
    PNET_BUFFER_LIST            netBufferListsToComplete = NULL;
    BOOLEAN                     nonInternalsCompleted = FALSE;
    #if DBG
    ULONG ulNumNBLs = 0, ulInternalSends = 0;
    #endif
    
    UNREFERENCED_PARAMETER(SendCompleteFlags);

    while (currentPacket != NULL)
    {
        if (MP_TX_MSDU_WRAPPED_NBL(currentPacket) == NULL)
        {
            #if DBG
            ulInternalSends++;
            #endif
            
            // Internal packet submitted from BasePortSendInternalPacket, free the memory
            // we allocated for the buffer
            MP_FREE_MEMORY(MP_TX_MSDU_MPDU_AT(currentPacket, 0)->InternalSendBuffer);
        }
        else
        {
            #if DBG
            ulNumNBLs++;
            #endif
            //
            // There were from the OS. First we indicate the completions to the ports and
            // then We need to convert back to NBLs and complete them to the OS
            //

            //
            // Give the packet to the port. Since we dont know what the next
            // packet type is & whether the port should process it or not, we 
            // give a NULL here
            //
            tempNextPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket);
            MP_TX_MSDU_NEXT_MSDU(currentPacket) = NULL;
            
            Port11NotifySendComplete(Port, currentPacket, SendCompleteFlags);

            //
            // Restore the next
            //
            MP_TX_MSDU_NEXT_MSDU(currentPacket) = tempNextPacket;
            
            //
            // Get the NBLs out
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

            nonInternalsCompleted = TRUE;
        }

        //
        // Remove the refcount added during the send
        //
        PORT_DECREMENT_PNP_REFCOUNT(Port);        

        currentPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket);
    }

    //
    // Free our packet wrapper structures for all the packets
    //
    BasePortFreeTranslatedTxPackets(Port, PacketList);

    #if DBG
        MpTrace(COMP_SEND, DBG_NORMAL, ("Port(%d): NdisMSendNetBufferListsComplete called with %d NBLs\n", Port->PortNumber, ulNumNBLs));
        MpTrace(COMP_SEND, DBG_NORMAL, ("Port(%d): Completed %d internal sends\n", Port->PortNumber, ulInternalSends));
    #endif
        

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
    // Check to determine if we have pending TX packets queued and whether
    // we should process them
    //
    
    //
    // Currently we have a primitive mechanism to avoid stack overflow:
    //      PortSend
    //      VNic/HwSend
    //      PortSendComplete
    //      PortSend
    //      VNic/HwSend
    //      ..
    //
    // We avoid by saying if we were sending OS packets
    // only then we send more down, else we dont
    //
    if (!MpPacketQueueIsEmpty(&Port->PendingTxQueue) && nonInternalsCompleted)
    {
        //
        // We have pending packets that need to get get processed either inline or deferred
        //
        MP_ACQUIRE_PORT_LOCK(Port, NDIS_TEST_SEND_COMPLETE_AT_DISPATCH_LEVEL(SendCompleteFlags));

        // Process pending packets queue. We are at dispatch because of the lock, but
        // we only forward the value we got from the OS
        BasePortProcessQueuedPackets(Port, 
            NDIS_TEST_SEND_COMPLETE_AT_DISPATCH_LEVEL(SendCompleteFlags) ? NDIS_SEND_FLAGS_DISPATCH_LEVEL
                : 0
            );
        
        MP_RELEASE_PORT_LOCK(Port, NDIS_TEST_SEND_COMPLETE_AT_DISPATCH_LEVEL(SendCompleteFlags));
    }


}


VOID 
BasePortDeferredSendThread(
    _In_ PVOID                    Context
    )
{
    PMP_PORT    port = (PMP_PORT)Context;
    BOOLEAN     trigger = FALSE;
    
    MpTrace(COMP_SEND, DBG_NORMAL, ("Port(%d): Send worker thread started\n", port->PortNumber));

    while (!MP_TEST_PORT_STATUS(port, MP_PORT_HALTING))
    {
        // Wait for some time for the send to be queued. We either get triggered or we timeout and see
        // that we had missed the trigger
        trigger = NdisWaitEvent(&port->DeferredSendTrigger, MP_SEND_THREAD_SLEEP_TIME);

        // If we either got triggered or we timed out and there are packets in the queue, process them.
        // If we miss something, we will come back to this eventually
        if (trigger || !MpPacketQueueIsEmpty(&port->PendingTxQueue))
        {
            // Lets try to acquire the send token. If we cannot, we will give up and try later
            if (PORT_WAIT_FOR_SEND_TOKEN(port, MP_SEND_TOKEN_WAIT_COUNT))
            {
                MpTrace(COMP_TESTING, DBG_NORMAL, ("Port(%d): Processing deferred sends\n", port->PortNumber));

                // We got the token, lets try to send queued packets
                NdisResetEvent(&port->DeferredSendTrigger);
                MP_ACQUIRE_PORT_LOCK(port, FALSE);
                BasePortTransmitQueuedPackets(port, 0);        
                MP_RELEASE_PORT_LOCK(port, FALSE);

                // Release the token
                PORT_RELEASE_SEND_TOKEN(port);
            }
            else
            {
                // Didnt get the token, lets keep waiting. We dont reset the event
            }
        }
        
        // If the port is halting, we would bail out, else we go back to waiting
    } 

    MpTrace(COMP_SEND, DBG_NORMAL, ("Port(%d): Send worker thread is exiting\n", port->PortNumber));
    
    PsTerminateSystemThread(STATUS_SUCCESS);
}
