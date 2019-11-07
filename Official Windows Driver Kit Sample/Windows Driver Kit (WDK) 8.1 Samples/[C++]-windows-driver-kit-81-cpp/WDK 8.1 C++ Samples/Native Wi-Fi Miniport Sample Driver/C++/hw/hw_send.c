/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_send.c

Abstract:
    Implements the send functionality for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"
#include "hw_send.h"
#include "hw_crypto.h"
#include "hw_phy.h"
#include "hw_mac.h"
#include "hw_rate.h"

#if DOT11_TRACE_ENABLED
#include "hw_send.tmh"
#endif

#if 0
// TEST CODE
__inline
void HW_CHECK_TX_MSDU_TIME(PHW_TX_MSDU _Msdu, ULONG _BreakTime)
{
    LARGE_INTEGER               _currentTime;

    UNREFERENCED_PARAMETER(_BreakTime);
    UNREFERENCED_PARAMETER(_Msdu);
    
    NdisGetCurrentSystemTime(&_currentTime); /* Returns in 100 nanoseconds */
    MpTrace(COMP_SEND, DBG_SERIOUS, ("# %4d %s %d\n", _Msdu->TotalMSDULength, (_Msdu->TxSucceeded ? "OK " : "ERR"),
        (ULONG)(_currentTime.QuadPart - _Msdu->SnapshotTime.QuadPart)));
}
#endif

NDIS_STATUS
Hw11InitializeSendEngine(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       i, j, size;
    PHW_TX_QUEUE                currentQueue;
    HAL_TX_QUEUE_SETUP_INFO     txQueueSetupInfo;
    PHW_TX_MSDU                 msdu;
    BOOLEAN                     halQueuesAllocated = FALSE, halQueuesSetup = FALSE;
    BOOLEAN                     lookasideAllocated = FALSE;

    UNREFERENCED_PARAMETER(ErrorValue);
    do
    {
        //
        // Before we allocate the queue structures, etc let us populate
        // some initial values so that we can do some stuff in a loop
        //
        
        // Data queue
        Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE].NumMSDUAllocated = Hw->RegInfo.NumTXBuffers;
        Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE].HalQueueType = HAL_QUEUE_TYPE_DATA;
        Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE].HalQueueIndex = LOW_QUEUE;
        Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE].MaxPendingTx = MAXULONG;

        // Management queue (mainly internal send)
        Hw->TxInfo.TxQueue[HW11_INTERNAL_SEND_QUEUE].NumMSDUAllocated = HW_INTERNAL_SEND_QUEUE_BUFFER_COUNT;
        Hw->TxInfo.TxQueue[HW11_INTERNAL_SEND_QUEUE].HalQueueType = HAL_QUEUE_TYPE_MANAGEMENT;
        Hw->TxInfo.TxQueue[HW11_INTERNAL_SEND_QUEUE].HalQueueIndex = NORMAL_QUEUE;
        Hw->TxInfo.TxQueue[HW11_INTERNAL_SEND_QUEUE].MaxPendingTx = 0;

        // High priority (unused)
        Hw->TxInfo.TxQueue[HW11_UNUSED_QUEUE].NumMSDUAllocated = HW_UNUSED_SEND_QUEUE_BUFFER_COUNT;
        Hw->TxInfo.TxQueue[HW11_UNUSED_QUEUE].HalQueueType = HAL_QUEUE_TYPE_HIGH_PRIORITY;
        Hw->TxInfo.TxQueue[HW11_UNUSED_QUEUE].HalQueueIndex = HIGH_QUEUE;
        Hw->TxInfo.TxQueue[HW11_UNUSED_QUEUE].MaxPendingTx = 0;

        // Beacon queue
        Hw->TxInfo.TxQueue[HW11_BEACON_QUEUE].NumMSDUAllocated = HW_BEACON_QUEUE_BUFFER_COUNT; // This is all that we need
        Hw->TxInfo.TxQueue[HW11_BEACON_QUEUE].HalQueueType = HAL_QUEUE_TYPE_BEACON;
        Hw->TxInfo.TxQueue[HW11_BEACON_QUEUE].HalQueueIndex = BEACON_QUEUE;
        Hw->TxInfo.TxQueue[HW11_BEACON_QUEUE].MaxPendingTx = 0;

        //
        // Allocate content for each of the HW queues
        //
        for (i = 0; i < HW11_NUM_TX_QUEUE; i++)
        {
            currentQueue = &Hw->TxInfo.TxQueue[i];
            
            //
            // Empty pending queues
            //
            MpInitPacketQueue(&(currentQueue->PendingTxQueue));

            //
            // Allocate the TX_MSDU's array
            //
            MP_ALLOCATE_MEMORY(Hw->MiniportAdapterHandle, &currentQueue->MSDUArray, 
                (currentQueue->NumMSDUAllocated * sizeof(HW_TX_MSDU)), HW_MEMORY_TAG);
            if (currentQueue->MSDUArray == NULL)
            {
                *ErrorCode = NDIS_ERROR_CODE_OUT_OF_RESOURCES;
                ndisStatus = NDIS_STATUS_RESOURCES;
                MpTrace(COMP_SEND, DBG_SERIOUS,  ("Failed to allocate the requested number (%d) of TX_MSDUs for queue %d.\n", 
                    currentQueue->NumMSDUAllocated, i
                    ));
                break;
            }
            NdisZeroMemory(currentQueue->MSDUArray, (currentQueue->NumMSDUAllocated * sizeof(HW_TX_MSDU)));

            // We leave one descriptor empty for easy synchronization
            currentQueue->NumMSDUAvailable = currentQueue->NumMSDUAllocated - 1;
            
            currentQueue->NextToSend = 0;   // Start from descriptor 0
            currentQueue->NextToComplete = 0;
            currentQueue->NextToReserve = 0;

            MP_OPEN_RECORDER(Hw->MiniportAdapterHandle, currentQueue->Tracking_SendRecorder);
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Now we allocate the HAL queues
        //
        ndisStatus = HalAllocateTxQueues(Hw->Hal, HW11_NUM_TX_QUEUE);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_SEND, DBG_SERIOUS,  ("Failed to allocate HAL TX queues. Status = 0x%08x\n",
                ndisStatus));
            *ErrorCode = NDIS_ERROR_CODE_OUT_OF_RESOURCES;
            break;
        }
        halQueuesAllocated = TRUE;

        //
        // Allocate TX descriptors
        //
        for (i = 0; i < HW11_NUM_TX_QUEUE; i++)
        {
            txQueueSetupInfo.DescNum = Hw->TxInfo.TxQueue[i].NumMSDUAllocated;
            txQueueSetupInfo.QueueType = Hw->TxInfo.TxQueue[i].HalQueueType;
            
            ndisStatus = HalSetupTxQueue(Hw->Hal, Hw->TxInfo.TxQueue[i].HalQueueIndex, &txQueueSetupInfo);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                *ErrorCode = NDIS_ERROR_CODE_OUT_OF_RESOURCES;
                MpTrace(COMP_SEND, DBG_SERIOUS,  ("Failed to setup the HAL TX queue for queue %d. Status = 0x%08x\n", 
                    i, ndisStatus
                    ));
                break;
            }

            HalResetTxDescs(Hw->Hal, Hw->TxInfo.TxQueue[i].HalQueueIndex);
        }
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // Free the partial allocation
            if (i > 0)
            {
                do
                {
                    i--;
                    HalReleaseTxDescs(Hw->Hal, Hw->TxInfo.TxQueue[i].HalQueueIndex);
                }while (i != 0);
            }
            
            break;
        }
        halQueuesSetup = TRUE;
        
        // For the default queue, we do some extra allocations - SG buffers and coalesce buffers
        currentQueue = &Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE];
        
        // For integer overflow
        ndisStatus = RtlULongMult(currentQueue->NumMSDUAllocated, MP_TX_FRAGMENTS_MAX_COUNT, &size);
        if (ndisStatus != STATUS_SUCCESS)
            break;
        
        ndisStatus = RtlULongMult(size, Hw->TxInfo.ScatterGatherListSize, &size);
        if (ndisStatus != STATUS_SUCCESS)
            break;

        // Allocate SG buffers
        MP_ALLOCATE_MEMORY(
            Hw->MiniportAdapterHandle,
            &Hw->TxInfo.ScatterGatherListBuffers,
            size,
            HW_MEMORY_TAG
            );
        if (Hw->TxInfo.ScatterGatherListBuffers == NULL)
        {
            *ErrorCode = NDIS_ERROR_CODE_OUT_OF_RESOURCES;
            ndisStatus = NDIS_STATUS_RESOURCES;
            MpTrace(COMP_SEND, DBG_SERIOUS,  ("Failed to allocate scatter gather resources\n"));
            break;
        }
        NdisZeroMemory(Hw->TxInfo.ScatterGatherListBuffers, size);

        //
        // Initialize the TX MSDUs for the default queue
        //
        for (i = 0; i < currentQueue->NumMSDUAllocated; i++)
        {
            msdu = &(currentQueue->MSDUArray[i]);

            //
            // Assign the preallocate SG buffer to this MSDU
            //
            msdu->ScatterGatherList = Hw->TxInfo.ScatterGatherListBuffers + 
                                        (i * Hw->TxInfo.ScatterGatherListSize * MP_TX_FRAGMENTS_MAX_COUNT);
            msdu->Hw = Hw;
            msdu->Index = i;
            msdu->QueueID = HW11_DEFAULT_QUEUE;
            
            // Allocate the coalesce buffers
            NdisMAllocateSharedMemory(Hw->MiniportAdapterHandle,
                                      MAX_TX_RX_PACKET_SIZE,
                                      FALSE,
                                      (void **)&msdu->BufferVa,
                                      &msdu->BufferPa);

            if (msdu->BufferVa == NULL)
            {
                ndisStatus=NDIS_STATUS_RESOURCES;
                *ErrorCode = NDIS_ERROR_CODE_OUT_OF_RESOURCES;                
                MpTrace(COMP_SEND, DBG_SERIOUS, ("Allocation of coalesce buffer %d failed\n", i));
                break;
            }
        }
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        for (j = 1; j < HW11_NUM_TX_QUEUE; j++)
        {
            // For the other queue, we only allocate the data buffer
            currentQueue = &Hw->TxInfo.TxQueue[j];
            for (i = 0; i < currentQueue->NumMSDUAllocated; i++)
            {
                msdu = &(currentQueue->MSDUArray[i]);

                msdu->Hw = Hw;
                msdu->Index = i;
                msdu->QueueID = (UCHAR)j;

                NdisMAllocateSharedMemory(Hw->MiniportAdapterHandle,
                                          MAX_TX_RX_PACKET_SIZE,
                                          FALSE,
                                          (void **)&msdu->BufferVa,
                                          &msdu->BufferPa);

                if (msdu->BufferVa == NULL)
                {
                    ndisStatus=NDIS_STATUS_RESOURCES;
                    *ErrorCode = NDIS_ERROR_CODE_OUT_OF_RESOURCES;
                    MpTrace(COMP_SEND, DBG_SERIOUS, ("Allocation of buffer %d for queued %d failed\n", i, j));
                    break;
                }
            }
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                break;
            }
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        //
        // Some basic allocations
        //

        // 
        // Lookaside list for HW_TX_MPDU. These are allocated on the fly as and when
        // we need them. 
        //
        NdisInitializeNPagedLookasideList(
            &Hw->TxInfo.TxMPDULookaside,
            NULL,
            NULL,
            0,
            sizeof(HW_TX_MPDU),
            HW_MEMORY_TAG,
            0
            );

        NdisInitializeNPagedLookasideList(
            &Hw->TxInfo.TxPacketLookaside,
            NULL,
            NULL,
            0,
            sizeof(MP_TX_MSDU),
            HW_MEMORY_TAG,
            0
            );

        NdisInitializeNPagedLookasideList(
            &Hw->TxInfo.TxFragmentLookaside,
            NULL,
            NULL,
            0,
            sizeof(MP_TX_MPDU),
            HW_MEMORY_TAG,
            0
            );
        lookasideAllocated = TRUE;

    } while (FALSE);
    
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // Other allocations
        if (lookasideAllocated)
        {
            NdisDeleteNPagedLookasideList(&Hw->TxInfo.TxFragmentLookaside);
            NdisDeleteNPagedLookasideList(&Hw->TxInfo.TxPacketLookaside);
            NdisDeleteNPagedLookasideList(&Hw->TxInfo.TxMPDULookaside);
        }
        //
        // Free the coalesce and other data buffers
        //
        for (j = 0; j < HW11_NUM_TX_QUEUE; j++)
        {
            currentQueue = &Hw->TxInfo.TxQueue[j];
            if (currentQueue->MSDUArray != NULL)
            {
                for (i = 0; i < currentQueue->NumMSDUAllocated; i++)
                {
                    msdu = &(currentQueue->MSDUArray[i]);

                    if (msdu->BufferVa != NULL)
                    {
                        NdisMFreeSharedMemory(Hw->MiniportAdapterHandle,
                            MAX_TX_RX_PACKET_SIZE,
                            FALSE,
                            (void *)msdu->BufferVa,
                            msdu->BufferPa
                            );
                    }
                }
            }
        }

        // The SG buffers
        if (Hw->TxInfo.ScatterGatherListBuffers)
            MP_FREE_MEMORY(Hw->TxInfo.ScatterGatherListBuffers);

        // Release the HAL data structures    
        if (halQueuesSetup)
        {
            for (i = 0; i < HW11_NUM_TX_QUEUE; i++)
            {
                HalReleaseTxDescs(Hw->Hal, Hw->TxInfo.TxQueue[i].HalQueueIndex);
            }    
        }
        if (halQueuesAllocated)
            HalReleaseTxQueues(Hw->Hal);
        
        // Release the HW data queues
        for (i = 0; i < HW11_NUM_TX_QUEUE; i++)
        {
            currentQueue = &Hw->TxInfo.TxQueue[i];
            if (currentQueue->MSDUArray != NULL)
            {
                MP_FREE_MEMORY(currentQueue->MSDUArray);
            }
            currentQueue->MSDUArray = NULL;

            MP_CLOSE_RECORDER(currentQueue->Tracking_SendRecorder);
            
        }

    }
    
    return ndisStatus;
}

VOID
Hw11TerminateSendEngine(
    _In_  PHW                     Hw
    )
{
    PHW_TX_QUEUE                currentQueue;
    PHW_TX_MSDU                 msdu;
    ULONG                       i, j;

    // Other allocations
    NdisDeleteNPagedLookasideList(&Hw->TxInfo.TxFragmentLookaside);
    NdisDeleteNPagedLookasideList(&Hw->TxInfo.TxPacketLookaside);
    NdisDeleteNPagedLookasideList(&Hw->TxInfo.TxMPDULookaside);

    //
    // Free the coalesce and other data buffers
    //
    for (j = 0; j < HW11_NUM_TX_QUEUE; j++)
    {
        currentQueue = &Hw->TxInfo.TxQueue[j];
        if (currentQueue->MSDUArray != NULL)
        {
            for (i = 0; i < currentQueue->NumMSDUAllocated; i++)
            {
                msdu = &(currentQueue->MSDUArray[i]);

                if (msdu->BufferVa != NULL)
                {
                    NdisMFreeSharedMemory(Hw->MiniportAdapterHandle,
                        MAX_TX_RX_PACKET_SIZE,
                        FALSE,
                        (void *)msdu->BufferVa,
                        msdu->BufferPa
                        );
                }
            }
        }
    }

    // The SG buffers
    if (Hw->TxInfo.ScatterGatherListBuffers)
        MP_FREE_MEMORY(Hw->TxInfo.ScatterGatherListBuffers);

    // Release the HAL data structures    
    for (i = 0; i < HW11_NUM_TX_QUEUE; i++)
    {
        currentQueue = &Hw->TxInfo.TxQueue[i];
        HalReleaseTxDescs(Hw->Hal, currentQueue->HalQueueIndex);
    }    
    HalReleaseTxQueues(Hw->Hal);

    // Release the HW data queues
    for (i = 0; i < HW11_NUM_TX_QUEUE; i++)
    {
        currentQueue = &Hw->TxInfo.TxQueue[i];
        if (currentQueue->MSDUArray != NULL)
        {
            MP_FREE_MEMORY(currentQueue->MSDUArray);
        }
        currentQueue->MSDUArray = NULL;

        MP_CLOSE_RECORDER(currentQueue->Tracking_SendRecorder);        
    }    
  
}

/**
 * This function is called once the Hw has made any and all changes that 
 * were needed to the 802.11 frame to be transmitted. After the scatter gather 
 * operation has been performed, the NIC must NOT make any changes to the 
 * packet as they may not get reflected in the SG Elements.
 *
 */
VOID
HwPerformScatterGather(
    _In_ PHW                      Hw,
    _In_ PHW_TX_MSDU              Msdu
    )
{
    PNET_BUFFER_LIST            NetBufferList;
    PNET_BUFFER                 CurrentNetBuffer;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_FAILURE;
    ULONG                       ulSize;
    
    //
    // Place a ref count on the Scatter Gather allocation count
    // Prevents it from completing the operation prematurely
    //
    HW_INCREMENT_PENDING_TX_MSDU_SG_OP(Msdu);
    
    NetBufferList = MP_TX_MSDU_WRAPPED_NBL(Msdu->MpMsdu);
    Msdu->TotalSGRequested = 0;

    for (CurrentNetBuffer = NET_BUFFER_LIST_FIRST_NB(NetBufferList);
          CurrentNetBuffer != NULL;
          CurrentNetBuffer = NET_BUFFER_NEXT_NB(CurrentNetBuffer))
    {
        //
        // If reset is occuring we must abandon attempts to SG
        //
        if (HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_SEND_FLAGS))
        {
            // We fail this packet right now
            ndisStatus = HwGetAdapterStatus(Hw);
            break;
        }
        
        //
        // One more NetBuffer submitted for scatter gather
        //
        HW_INCREMENT_PENDING_TX_MSDU_SG_OP(Msdu);
        HW_INCREMENT_ACTIVE_OPERATION_REF(Hw);
        
        //
        // Ask NDIS to scatter gather this NetBuffer for us
        //
        if (Msdu->TotalSGRequested < HW_MAX_NUM_OF_FRAGMENTS)
        {
            ndisStatus = RtlULongMult(Hw->TxInfo.ScatterGatherListSize, Msdu->TotalSGRequested, &ulSize);
            if (ndisStatus != STATUS_SUCCESS)
            {
                MpTrace(COMP_SEND, DBG_SERIOUS, ("Overflow when computing SG buffer size\n"));
                break;
            }
            
            //
            // Use the preallocated scatter gather resources
            //
            ndisStatus = NdisMAllocateNetBufferSGList(
                Hw->MiniportDmaHandle,
                CurrentNetBuffer,
                Msdu,
                NDIS_SG_LIST_WRITE_TO_DEVICE,
                (Msdu->ScatterGatherList + ulSize),
                Hw->TxInfo.ScatterGatherListSize
                );
        }
        else
        {
            //
            // We have run out of preallocated resources of scatter gather.
            // We will let OS allocate some on the fly. This is unoptimal
            // but we expect this situation to get hit very rarely.
            //
            ndisStatus = NdisMAllocateNetBufferSGList(
                Hw->MiniportDmaHandle,
                CurrentNetBuffer,
                Msdu,
                NDIS_SG_LIST_WRITE_TO_DEVICE,
                NULL,
                0
                );
        }
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            //
            // Scatter Gather call failed. Remove the ref count added above
            //
            MpTrace(COMP_SEND, DBG_NORMAL,  ("Failed to scatter gather NetBuffer %p\n", CurrentNetBuffer));
            HW_DECREMENT_PENDING_TX_MSDU_SG_OP(Msdu);
            HW_DECREMENT_ACTIVE_OPERATION_REF(Hw);
            break;
        }

        //
        // SG successfully requested. The requst cannot fail now.
        // This number reflects the total number of SG operations that will
        // have successfully completed as well.
        //
        Msdu->TotalSGRequested++;
    }
    
    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        //
        // Successful with requesting scatter gather for all NBs.
        //
        Msdu->ScatterGatherRequested = TRUE;
    }
    else
    {
        Msdu->WaitForSendToComplete = FALSE;
        Msdu->FailedDuringSend = TRUE;
        
        // These packets have failed. But we set the Ready for send flag
        // on the packets irrespective
        HW_TX_MSDU_SET_FLAG(Msdu, HW_TX_MSDU_IS_READY_FOR_SEND);
    }
    
    //
    // Remove the Ref count added at start of SG for this Tx MSDU
    //
    if (HW_DECREMENT_PENDING_TX_MSDU_SG_OP(Msdu) == 0)
    {
        //
        // Scatter gather has been successfully completed
        // for this MSDU.
        //
        HwSGComplete(Msdu, TRUE);
    }
}


VOID
HWProcessSGList(
    _In_  PDEVICE_OBJECT          DeviceObject,
    _In_  PVOID                   Irp,
    _In_  PSCATTER_GATHER_LIST    SGList,
    _In_  PVOID                   Context
    )
{
    PHW_TX_MSDU                 Msdu;
    UNREFERENCED_PARAMETER(DeviceObject);
    UNREFERENCED_PARAMETER(Irp);
    
    Msdu = (PHW_TX_MSDU) Context;
    
    //
    // One more NetBuffer has been scatter gathered successfully
    //
    Msdu->SGElementList[Msdu->SGElementListCount] = SGList;
    Msdu->SGElementListCount++;
    
    //
    // Check if we are done with SG for this NBL
    //
    if (HW_DECREMENT_PENDING_TX_MSDU_SG_OP(Msdu) == 0)
    {
        HwSGComplete(Msdu, TRUE);
    }

    //
    // Remove the HW level count kept on pending SG operations
    //
    HW_DECREMENT_ACTIVE_OPERATION_REF(Msdu->Hw);
}


VOID
HwSGComplete(
    _In_  PHW_TX_MSDU         Msdu,
    _In_  BOOLEAN             DispatchLevel
    )
{
    MPASSERT(Msdu->OutstandingSGAllocationCount == 0);
    if (Msdu->ScatterGatherRequested)
    {
        //
        // All SG operations for this NBL have been completed
        // successfully.
        //

        // This packet is ready for sending
        HW_TX_MSDU_SET_FLAG(Msdu, HW_TX_MSDU_IS_READY_FOR_SEND);
    }
    else
    {
        //
        // Scatter gather has failed
        //
        MPASSERT(Msdu->OutstandingSGAllocationCount == 0);  // Ensure we dont have SG operations pending
        MPASSERT(Msdu->FailedDuringSend == TRUE);
    }

    //
    // Flush the ready packets to the hardware
    //
    HwSubmitReadyMSDUs(Msdu->Hw, &Msdu->Hw->TxInfo.TxQueue[Msdu->QueueID], DispatchLevel);
}

__inline BOOLEAN
HwCanTransmit(
    _In_  PHW_TX_QUEUE            TxQueue
    )
{
    //
    // The requirements to be able to transmit are:
    // 1. HW has available TX_MSDUs for sending
    //
    return ((TxQueue->NumMSDUAvailable > 0) ? TRUE : FALSE);
}

NDIS_STATUS
HwPrepareTxMSDUForSend(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_MAC_HEADER           fragmentHeader;
    PDOT11_MGMT_DATA_MAC_HEADER mgmtdataHeader;
    PHW_MAC_CONTEXT             macContext;
    PHW_PEER_NODE               peerNode = NULL;
    PMP_TX_MPDU                 mpMpdu;
    PHW_TX_MPDU                 hwMpdu;
    PHW_KEY_ENTRY               key = NULL;
    ULONG                       fragmentLength;
    ULONG                       i;
    USHORT                      duration;
    HW_TX_CIPHER_SETTING        cipherSetting;
    UCHAR                       packetType;
    USHORT                      initialTxRate;

    UNREFERENCED_PARAMETER(TxQueue);
    
    // OS currently only sends 1 MPDU per MSDU. The miniport does not
    // do fragmentation on its own. So we only expect to have 1 MPDU per MSDU
    MPASSERT(Msdu->MpduCount == 1);
    
    do
    {
        // 
        // Get the fragment header from the first MPDU. This would also apply for all
        // the MSDUs in the packets
        //
        mpMpdu = HW_TX_MSDU_MPDU_AT(Msdu, 0)->MpMpdu;
        fragmentLength = MP_TX_MPDU_LENGTH(mpMpdu);

        // We shouldnt ever send really small packets through this code path
        MPASSERT(fragmentLength > sizeof(DOT11_MAC_HEADER));

        fragmentHeader = MP_TX_MPDU_DATA(mpMpdu, sizeof(DOT11_MAC_HEADER));        
        if ((fragmentHeader == NULL) || (fragmentLength < sizeof(DOT11_MAC_HEADER)))
        {
            MpTrace(COMP_SEND, DBG_SERIOUS, ("Not enough data in packet to be sent\n"));
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // The MAC context that is trying to send the packet
        macContext = Msdu->MpMsdu->MacContext;

        // Find the peer destination node. If we cannot find an existing one,
        // we use the default peer node
        peerNode = HwFindPeerNode(macContext, fragmentHeader->Address1, FALSE);
        if (peerNode == NULL)
            peerNode = &macContext->DefaultPeer;
        Msdu->PeerNode = peerNode;
        
        // Collect packet information
        Msdu->MulticastDestination = DOT11_IS_MULTICAST(fragmentHeader->Address1);
        packetType = (UCHAR)fragmentHeader->FrameControl.Type;
        if ((packetType == DOT11_FRAME_TYPE_DATA) ||
            (packetType == DOT11_FRAME_TYPE_MANAGEMENT))
        {
            if (fragmentLength < sizeof(DOT11_MGMT_DATA_MAC_HEADER))
            {
                // Bad packet
                MpTrace(COMP_SEND, DBG_SERIOUS, ("Not enough data in management/data packet to be sent\n"));
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }
        }
        
        // Determine if this packet must be encrypted
        cipherSetting = HwDetermineCipherSettings(macContext, peerNode, Msdu, fragmentHeader);
        if (cipherSetting != HW_TX_NEVER_ENCRYPT)
        {    
            // Find the key to be used for the encryption
            key = HwFindEncryptionKey(macContext, peerNode, Msdu, fragmentHeader, cipherSetting);
            if (key == NULL) 
            {
                if (cipherSetting == HW_TX_CAN_ENCRYPT)
                {
                    //
                    // If this is a unicast frame or if the BSSPrivacy is on, reject the frame
                    //
                    if ((!Msdu->MulticastDestination) || 
                        (MP_TEST_FLAG(peerNode->CapabilityInfo, DOT11_CAPABILITY_INFO_PRIVACY)))
                    {
                        // We must encrypt the packet
                        MpTrace(COMP_SEND, DBG_SERIOUS, ("Dropping packet because we are unable to find key to encrypt packet\n"));
                        ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
                        break;
                    }
                }
                // Packet should not be encrypted
                Msdu->SendEncrypted = FALSE;
            }
            else
            {
                // Found a key. Packet should be sent encrypted
                Msdu->SendEncrypted = TRUE;
                Msdu->Key = key;
            }
        }
        else
        {
            Msdu->SendEncrypted = FALSE;
        }

        // Determine the TX rate for this packet
        initialTxRate = HwDetermineStartTxRate(macContext, peerNode, Msdu, fragmentHeader);
        if (cipherSetting == HW_TX_ENCRYPT_IF_KEY_MAPPING_KEY_AVAILABLE)
        {
            //
            // This is an indication by the OS that is sending a 1x packet. We use
            // lower rates for these packets
            //
            initialTxRate = macContext->DefaultTXMgmtRate;
        }
        HwFillTxRateTable(macContext, Msdu, initialTxRate);
    
        Msdu->UseShortPreamble = FALSE;
        Msdu->CTSToSelfEnabled = FALSE;
        Msdu->PsBitSetting = TxDescPsBitUnspecified;

        if (MP_TX_MSDU_IS_PRIVATE(Msdu->MpMsdu))
        {
            // For internal packets, set the PS bit based on the fragment header
            if (fragmentHeader->FrameControl.PwrMgt)
            {
                // PS bit should be set
                Msdu->PsBitSetting = TxDescPsBitSet;
            }
            else
            {
                // PS bit should be clear
                Msdu->PsBitSetting = TxDescPsBitClear;
            }
        }

        // We do not support sending on any PHY
        UNREFERENCED_PARAMETER(MP_TX_MSDU_SEND_CONTEXT(Msdu->MpMsdu)->uPhyId);

        Msdu->PhyId = macContext->OperatingPhyId;
        if (HalGetPhyMIB(Hw->Hal, Msdu->PhyId)->PhyType == dot11_phy_type_erp)
        {
            if (MP_TEST_FLAG(peerNode->CapabilityInfo, DOT11_CAPABILITY_SHORT_PREAMBLE) && 
                (packetType != DOT11_FRAME_TYPE_MANAGEMENT))
                Msdu->UseShortPreamble = TRUE;

            // if the data rate is at 11mbps or less, there is no need to send CTS to self
            if (macContext->CTSToSelfEnabled && (initialTxRate > 22))
                Msdu->CTSToSelfEnabled = TRUE;
        }

        // RTS enable
        if (((fragmentLength + 4) > macContext->RTSThreshold) &&
            (macContext->FragmentationThreshold > macContext->RTSThreshold))
        {
            Msdu->RTSEnabled = TRUE;
        }
        else
        {
            Msdu->RTSEnabled = FALSE;
        }

        Msdu->SendItem.DescNum = 0;
        Msdu->SendItem.firstIterator = (HAL_TX_ITERATOR)(-1);
        Msdu->SendItem.lastIterator = (HAL_TX_ITERATOR)(-1);
        Msdu->TotalMSDULength = 0;

        // Process all the MPDUs in the packet (currently 1)
        for (i = 0; i < HW_TX_MSDU_MPDU_COUNT(Msdu); i++)
        {
            hwMpdu = HW_TX_MSDU_MPDU_AT(Msdu, i);
            mpMpdu = hwMpdu->MpMpdu;

            // We need to fill stuff into the fragment header
            fragmentHeader = MP_TX_MPDU_DATA(mpMpdu, sizeof(DOT11_MAC_HEADER));        
            fragmentLength = MP_TX_MPDU_LENGTH(mpMpdu);
            
            if (key != NULL)
            {
                // This frame would be sent encrypted
                fragmentHeader->FrameControl.WEP = 1;
                
                // Perform TX encryption
                ndisStatus = HwSetupTxCipher(Hw, macContext, Msdu, hwMpdu, key);
                if (ndisStatus != NDIS_STATUS_SUCCESS)
                {
                    MpTrace(COMP_SEND, DBG_SERIOUS, ("Failed to setup TX encryption for \n"));
                    break;
                }

                // Update the length and header since we may have moved the MAC headers
                fragmentLength = MP_TX_MPDU_LENGTH(mpMpdu);
                fragmentHeader = MP_TX_MPDU_DATA(mpMpdu, sizeof(DOT11_MAC_HEADER));        
            }
            else if (!Hw->MacState.SafeModeEnabled)
            {
                // We arent encrypting, so clear the WEP bit
                fragmentHeader->FrameControl.WEP = 0;
            }
            // Fill bits that the OS does not set (We dont do fragmentation or power mgmt yet)
            if (!MP_TX_MSDU_IS_PRIVATE(Msdu->MpMsdu))
            {
                fragmentHeader->FrameControl.MoreFrag = 0;
                fragmentHeader->FrameControl.Retry = 0;
                fragmentHeader->FrameControl.MoreData = 0;
                fragmentHeader->FrameControl.PwrMgt = 0;
            }

            Msdu->TotalMSDULength += fragmentLength;
            
            // Compute and fill the duration into the packet
            duration = 0;
            if (!Msdu->MulticastDestination)
            {
                // Calculate the duration field value
                duration = HwComputeTxTime(
                                Msdu,
                                packetType,
                                fragmentLength,
                                (i == (ULONG)(HW_TX_MSDU_MPDU_COUNT(Msdu) - 1) ? TRUE : FALSE)
                                );
            }
            
            NdisMoveMemory(&fragmentHeader->DurationID, &duration, sizeof(USHORT));


            // Fill the sequence number into the packet
            if ((packetType == DOT11_FRAME_TYPE_DATA) ||
                (packetType == DOT11_FRAME_TYPE_MANAGEMENT))
            {
                mgmtdataHeader = (PDOT11_MGMT_DATA_MAC_HEADER)fragmentHeader;

                mgmtdataHeader->SequenceControl.FragmentNumber = (USHORT)i;
                mgmtdataHeader->SequenceControl.SequenceNumber = (USHORT)macContext->SequenceNumber;                
            }
            
            if (i == (ULONG)(HW_TX_MSDU_MPDU_COUNT(Msdu) - 1))
            {
                // When we reach the last fragment we increment the sequence number
                MP_INCREMENT_LIMIT_UNSAFE(macContext->SequenceNumber, 4096);
            }

        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

    } while (FALSE);

    return ndisStatus;
}


// Lock held
__inline NDIS_STATUS
HwReserveTxResources(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PMP_TX_MSDU             Packet
    )
{
    PHW_TX_MSDU                 reservedMsdu;
    PHW_TX_MPDU                 mpdu;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       i;

    //
    // One more TX_MSDU will now be in use. Decrement available count and 
    // update TX_MSDU fields.
    //
    HW_DECREMENT_AVAILABLE_TX_MSDU(TxQueue);
    reservedMsdu = &TxQueue->MSDUArray[TxQueue->NextToReserve];
    reservedMsdu->MpduCount = 0;

    MpTrace(COMP_SEND, DBG_LOUD, ("Reserved %d on %d\n", TxQueue->NextToReserve, TxQueue->HalQueueIndex));
    
    //
    // Allocate MPDUs
    for (i = 0; i < MP_TX_MSDU_MPDU_COUNT(Packet); i++)
    {
        mpdu = NdisAllocateFromNPagedLookasideList(&(Hw->TxInfo.TxMPDULookaside));
        if (mpdu == NULL)
        {
            MpTrace(COMP_SEND, DBG_SERIOUS, ("Unable to allocate HW_TX_MPDU during TX resource reservation %d\n", i));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        NdisZeroMemory(mpdu, sizeof(HW_TX_MPDU));

        // Save the MPDU in the MSDU list
        reservedMsdu->MpduList[i] = mpdu;
        reservedMsdu->MpduCount++;

        // Link them
        mpdu->MpMpdu = MP_TX_MSDU_MPDU_AT(Packet, i);
    }

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // Free the MPDUs
        for (i = 0; i < reservedMsdu->MpduCount; i++)
        {
            NdisFreeToNPagedLookasideList(&(Hw->TxInfo.TxMPDULookaside), reservedMsdu->MpduList[i]);
        }
        reservedMsdu->MpduCount = 0;
    
        HW_INCREMENT_AVAILABLE_TX_MSDU(TxQueue);

        return ndisStatus;
    }
    
    // Only support single MPDU per MSDU
    MPASSERT(reservedMsdu->MpduCount == 1);        

    // Update the next descriptor to reserve
    MP_INCREMENT_LIMIT_UNSAFE(TxQueue->NextToReserve, (LONG)TxQueue->NumMSDUAllocated);
    
    HW_RESERVE_TX_MSDU(reservedMsdu, Packet);
    
    return NDIS_STATUS_SUCCESS;
}


VOID
HwProcessReservedTxPackets(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  LONG                    StartIndex,
    _In_  ULONG                   NumTxReady,
    _In_  ULONG                   SendFlags
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PHW_TX_MSDU                 hwMsdu;
    ULONG                       i = 0;
    KIRQL                       oldIrql = 0;
    BOOLEAN                     dispatchLevel = NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags);

    if (NumTxReady != 0)
    {
        HW_TX_ACQUIRE_QUEUE_LOCK(Hw, TxQueue, dispatchLevel);

        //
        // Process each of the reserved MSDUs
        //
        for (i = 0; i < NumTxReady; i++)
        {
            hwMsdu = &TxQueue->MSDUArray[(StartIndex + i) % (TxQueue->NumMSDUAllocated)];
            ndisStatus = HwPrepareTxMSDUForSend(Hw, TxQueue, hwMsdu);

            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                //
                // We cannot send this packet. Fail it during SendComplete
                //
                MpTrace(COMP_SEND, DBG_SERIOUS, ("Could not prepare TX MSDU %p for sending\n", hwMsdu));
                hwMsdu->WaitForSendToComplete = FALSE;
                hwMsdu->FailedDuringSend = TRUE;
            }
        }

        HW_TX_RELEASE_QUEUE_LOCK(Hw, TxQueue, dispatchLevel);

        //
        // Now go through our list of packets and request scatter gather for those
        // that need it
        //
        if (!dispatchLevel)
            NDIS_RAISE_IRQL_TO_DISPATCH(&oldIrql);  // Scatter gather must be called at Dispatch
        for (i = 0; i < NumTxReady; i++)
        {
            hwMsdu = &TxQueue->MSDUArray[(StartIndex + i) % (TxQueue->NumMSDUAllocated)];
            
            if (hwMsdu->FailedDuringSend == FALSE)
            {
                // If necessary, call NDIS to request scatter gather resources for this packet
                if (HW_TX_MSDU_IS_INTERNAL(hwMsdu))
                {
                    // For internal packets, we dont do scatter gather it is essentially
                    // ready for seng
                    HW_TX_MSDU_SET_FLAG(hwMsdu, HW_TX_MSDU_IS_READY_FOR_SEND);
                }
                else
                {
                    // We cannot send yet. We need to do scatter gather and send
                    // once SG completes
                    HwPerformScatterGather(Hw, hwMsdu);
                }
            }
            else
            {
                // These packets have failed. But we set the Ready for send flag
                // on the packets irrespective
                HW_TX_MSDU_SET_FLAG(hwMsdu, HW_TX_MSDU_IS_READY_FOR_SEND);
            }        
        }
        if (!dispatchLevel)
            NDIS_LOWER_IRQL(oldIrql, DISPATCH_LEVEL);
    }
    
    //
    // Now we have either finished prepared the packets for sending or have
    // submitted them to NDIS for scatter gather. Now we try to determine if we
    // can submit them to the HAL
    //
    if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_SEND_FLAGS))
    {
        HwSubmitReadyMSDUs(Hw, TxQueue, (BOOLEAN)dispatchLevel);
    }
}

__inline USHORT 
HwComputeDot11TxTime(
	_In_  USHORT                  FrameLength,
	_In_  ULONG                   DataRate,
	_In_  BOOLEAN                 ManagementFrame,
	_In_  BOOLEAN                 ShortPreamble
	)
{
	USHORT			            frameTime;

	if(ManagementFrame || !ShortPreamble)
	{	// long preamble
		frameTime = (USHORT)(144+48+(FrameLength*8*2/DataRate));		
	}
	else
	{	// Short preamble
		frameTime = (USHORT)(72+24+(FrameLength*8*2/DataRate));
	}

	if( ( FrameLength*8*2 % DataRate ) != 0 )
		frameTime ++;
	
	return frameTime;
}


__inline USHORT
HwComputeTxTime(
    _In_  PHW_TX_MSDU             Msdu,
    _In_  DOT11_FRAME_TYPE        FrameType,
    _In_  ULONG                   FragmentLength,
    _In_  BOOLEAN                 LastFragment
    )
{
    BOOLEAN                     isManagement = (FrameType == DOT11_FRAME_TYPE_MANAGEMENT) ? TRUE : FALSE;
    USHORT                      AckCtsTime;
    
    AckCtsTime = HwComputeDot11TxTime(sAckCtsLng / 8, Msdu->TxRateTable[0], isManagement, Msdu->UseShortPreamble);
    if (LastFragment) 
    {
        return (aSifsTime + AckCtsTime);
    }
    else
    {
        return (3 * aSifsTime + 2 * AckCtsTime +  
                     HwComputeDot11TxTime((USHORT)(FragmentLength + sCrcLng), 
                                   Msdu->TxRateTable[0], 
                                   isManagement, 
                                   Msdu->UseShortPreamble
                                   )
                );
    }
}




__inline HW_TX_CIPHER_SETTING
HwDetermineCipherSettings(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_PEER_NODE           PeerNode,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  PDOT11_MAC_HEADER       FragmentHeader
    )
{
    UNREFERENCED_PARAMETER(PeerNode);
    
    if (MacContext->Hw->MacState.SafeModeEnabled)
    {
        // If we are in safe mode we never encrypt
        return HW_TX_NEVER_ENCRYPT;
    }

    //
    // Else, if encryption is enabled determine wheter or not we should be encrypting
    //
    if (MacContext->UnicastCipher != DOT11_CIPHER_ALGO_NONE)
    {
        if (FragmentHeader->FrameControl.Type == DOT11_FRAME_TYPE_DATA)
        {
            // Return the cipher settings from the send context
            switch(MP_TX_MSDU_SEND_CONTEXT(Msdu->MpMsdu)->usExemptionActionType)
            {
                case DOT11_EXEMPT_NO_EXEMPTION:
                    
                    //
                    // We want to encrypt this frame.
                    //
                    return HW_TX_CAN_ENCRYPT;
                    break;

                case DOT11_EXEMPT_ALWAYS:
                    
                    //
                    // We don't encrypt this frame.
                    //
                    return HW_TX_NEVER_ENCRYPT;
                    break;

                case DOT11_EXEMPT_ON_KEY_MAPPING_KEY_UNAVAILABLE:

                    //
                    // We encrypt this frame if and only if a key mapping key is set.
                    //
                    return HW_TX_ENCRYPT_IF_KEY_MAPPING_KEY_AVAILABLE;
                    break;

                default:
                    MPASSERT(FALSE);
                    return HW_TX_CAN_ENCRYPT;
                    break;            
            }
        }
        else if (FragmentHeader->FrameControl.WEP)
        {
            // Management packets that need encryption for cases like shared key authentication
            return HW_TX_CAN_ENCRYPT;
        }
    }             
        

    //
    // All other packets are never encrypted
    return HW_TX_NEVER_ENCRYPT;
}


PHW_KEY_ENTRY
HwFindEncryptionKey(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_PEER_NODE           PeerNode,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  PDOT11_MAC_HEADER       FragmentHeader,
    _In_  HW_TX_CIPHER_SETTING    CipherSettings
    )
{
    PHW_KEY_ENTRY               keyEntry = NULL;
    ULONG                       index = 0;
    NDIS_STATUS                 ndisStatus;

    UNREFERENCED_PARAMETER(PeerNode);
    
    //
    // Find the key that would be used to encrypt the frame if the frame were to be encrypted
    // For unicast frame, search the matching key in the key mapping table first. If not found, 
    // use default key. For multicast frame, only use the default key.    
    //
    if (!Msdu->MulticastDestination)
    {
        ndisStatus = HwFindKeyMappingKeyIndex(MacContext, FragmentHeader->Address1, FALSE, &index);
        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            keyEntry = &MacContext->KeyTable[index];
            return keyEntry;
        }
    }

    if (CipherSettings == HW_TX_ENCRYPT_IF_KEY_MAPPING_KEY_AVAILABLE)
    {
        // We are only supposed to use a key mapping key for encryption. We didnt find 
        // one. Return
        return NULL;
    }

    //
    // Return the corresponding default key
    // 
    keyEntry = &MacContext->KeyTable[MacContext->DefaultKeyIndex];
    if (!keyEntry->Key.Valid)
    {
        //
        // Check if we would be using the private key table
        //
        if ((Msdu->MulticastDestination) &&
            (MacContext->BssType == dot11_BSS_type_independent) &&
            (MacContext->AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK)
            )
        {
            // Check the private key table since we didnt put the TX default 
            // key into the regular key table
            keyEntry = &PeerNode->PrivateKeyTable[MacContext->DefaultKeyIndex];
            if (!keyEntry->Key.Valid)
            {
                return NULL;
            }
        }
        else
        {
            return NULL;
        }
    }
    return keyEntry;
}

NDIS_STATUS
HwSetupTxCipher(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  PHW_TX_MPDU             Mpdu,
    _In_  PHW_KEY_ENTRY           Key
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PNICKEY                     nicKey;
    PMDL                        lastMdl = NULL;
    BOOLEAN                     doSoftwareEncryption = FALSE;
    PNET_BUFFER                 netBuffer = NULL;
    ULONG                       headerSize;
    ULONG                       remainingDataLength = 0, byteCount = 0;
    PUCHAR                      encryptionIV;
    PDOT11_MGMT_DATA_MAC_HEADER fragmentHeader;
    UCHAR                       keyId;
    
    if (HalGetEncryptionCapabilities(Hw->Hal) & HAL_ENCRYPTION_RESERVE_IV_FIELD)
    {
        do
        {
            Mpdu->RetreatedSize = 0;
            Mpdu->MICMdlAdded = FALSE;
            Mpdu->LastMdlByteCount = 0;
            Mpdu->MICMdl = NULL;

            nicKey = &Key->Key;

            //
            // Find the size of header and IV field.
            //
            fragmentHeader = MP_TX_MPDU_DATA(Mpdu->MpMpdu, sizeof(DOT11_MGMT_DATA_MAC_HEADER));
            
            if (fragmentHeader)
            {
                headerSize = (fragmentHeader->FrameControl.FromDS && fragmentHeader->FrameControl.ToDS) ?
                             sizeof(DOT11_DATA_LONG_HEADER) : sizeof(DOT11_DATA_SHORT_HEADER);
            }
            else
            {
                headerSize = 0;
            }

            if (nicKey->AlgoId == DOT11_CIPHER_ALGO_TKIP ||
                nicKey->AlgoId == DOT11_CIPHER_ALGO_CCMP)
            {
                // These ciphers are not supported for internal sends
                MPASSERT(!HW_TX_MSDU_IS_INTERNAL(Msdu));            
                Mpdu->RetreatedSize = 8;
            }
            else
            {
                Mpdu->RetreatedSize = 4;
            }

            //
            // The IV is saved at different locations for internal and external packets
            // identify the location
            //
            if (!HW_TX_MSDU_IS_INTERNAL(Msdu))
            {
                // Packet coming from the OS - We use the backfill
                // for the IV, etc
                netBuffer = MP_TX_MPDU_WRAPPED_NB(Mpdu->MpMpdu);
                            
                //
                // For TKIP, we need to calculate MIC and append it at the end of MSDU.
                // For WPA2PSK adhoc, we need to do software encryption. Also allocate
                // a MIC, but don't do MIC calculation.
                //
                if ((nicKey->AlgoId == DOT11_CIPHER_ALGO_TKIP) ||
                    ((nicKey->AlgoId == DOT11_CIPHER_ALGO_CCMP) &&
                     (Msdu->MulticastDestination) &&
                     (MacContext->BssType == dot11_BSS_type_independent) &&
                     (MacContext->AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK)))
                {
                    
                    // Allocate the MDL for the MIC
                    Mpdu->MICMdl = NdisAllocateMdl(MacContext->Hw->MiniportAdapterHandle, Mpdu->MICData, 8);
                    if (!Mpdu->MICMdl) 
                    {
                        ndisStatus = NDIS_STATUS_RESOURCES;
                        MpTrace(COMP_SEND, DBG_SERIOUS, ("Unable to allocate MDL for MIC\n"));
                        break;
                    }

                    //
                    // Calculate MIC
                    //
                    if (nicKey->AlgoId == DOT11_CIPHER_ALGO_TKIP)
                    {
                        ndisStatus = HwCalculateTxMIC(netBuffer, 0, nicKey->TxMICKey, Mpdu->MICData);
                        if (ndisStatus != NDIS_STATUS_SUCCESS)
                        {
                            MpTrace(COMP_SEND, DBG_SERIOUS, ("Unable to calculate TX MIC\n"));
                            break;
                        }
                    }
                    else
                    {
                        doSoftwareEncryption = TRUE;
                    }

                    //
                    // Chain the new MDL at the end of the NET_BUFFER.
                    //
                    lastMdl = NET_BUFFER_FIRST_MDL(netBuffer);
                    remainingDataLength = NET_BUFFER_DATA_OFFSET(netBuffer) + NET_BUFFER_DATA_LENGTH(netBuffer);
                    byteCount = MmGetMdlByteCount(lastMdl);
                    while(remainingDataLength > byteCount)
                    {
                        remainingDataLength -= byteCount;
                        lastMdl = lastMdl->Next;
                        byteCount = MmGetMdlByteCount(lastMdl);
                    }

                    Mpdu->LastMdlByteCount = byteCount;
                    Mpdu->LastMdl = lastMdl;
                    NdisAdjustMdlLength(lastMdl, remainingDataLength);

                    // Add the MIC Mdl behind the last MDL
                    Mpdu->MICMdl->Next = lastMdl->Next;
                    lastMdl->Next = Mpdu->MICMdl;

                    // Set the flag so that we will clean this up on completion
                    Mpdu->MICMdlAdded = TRUE;
                    NET_BUFFER_DATA_LENGTH(netBuffer) += 8;
                }

                //
                // OS must guarantee 8 available bytes for retreat. WEP IV will use 4 bytes,
                // CCMP/TKIP IV will use 8 bytes.
                //
                MPASSERT(NET_BUFFER_DATA_OFFSET(netBuffer) >= HW11_REQUIRED_BACKFILL_SIZE);
                MPASSERT(NET_BUFFER_FIRST_MDL(netBuffer) == NET_BUFFER_CURRENT_MDL(netBuffer));

                // Note that because we had enough space for retreat and we are at the 
                // first MDL, the retreated portion is still in the first MDL. 
                ndisStatus = NdisRetreatNetBufferDataStart(netBuffer, 
                                Mpdu->RetreatedSize, 
                                0, 
                                NULL
                                );
                if (ndisStatus != NDIS_STATUS_SUCCESS)
                {
                    MpTrace(COMP_SEND, DBG_SERIOUS, ("NdisRetreatNetBufferDataStart failed for TX IV \n"));
                    ndisStatus = NDIS_STATUS_RESOURCES;
                    Mpdu->RetreatedSize = 0;
                    break;
                }
                

            }
            else
            {
                // Internal packets - There is extra backfill in the
                // send buffer. Use that for the IV
                
                MPASSERT(Mpdu->MpMpdu->InternalSendBuffer < Mpdu->MpMpdu->InternalSendDataStart);

                // Move the data start behind and change the total data length
                Mpdu->MpMpdu->InternalSendDataStart -= Mpdu->RetreatedSize;
                Mpdu->MpMpdu->InternalSendLength = Mpdu->MpMpdu->InternalSendLength 
                                                        + Mpdu->RetreatedSize;
            }

            //
            // We move the 802.11 header to make room for encryption IV
            // Note: cannot call NdisMoveMemory since source and destination overlaps.
            //
            fragmentHeader = (PDOT11_MGMT_DATA_MAC_HEADER)
                             (((PCHAR)fragmentHeader) - Mpdu->RetreatedSize);

            RtlMoveMemory(fragmentHeader, 
                          ((PCHAR)fragmentHeader) + Mpdu->RetreatedSize, 
                          headerSize);

            //
            // Now initialize the IV field.
            //
            encryptionIV = Add2Ptr(fragmentHeader, headerSize);

            keyId = Key->PeerKeyIndex;
            if (keyId >= DOT11_MAX_NUM_DEFAULT_KEY)
                keyId = 0;

            ndisStatus = NDIS_STATUS_SUCCESS;

            switch (nicKey->AlgoId)
            {
                case DOT11_CIPHER_ALGO_WEP40:
                case DOT11_CIPHER_ALGO_WEP104:
                    encryptionIV[0] = (UCHAR)(Key->IV & 0xff);
                    encryptionIV[1] = (UCHAR)((Key->IV >> 8) & 0xff);
                    encryptionIV[2] = (UCHAR)((Key->IV >> 16) & 0xff);
                    encryptionIV[3] = (keyId << 6);

                    Key->IV++;

                    break;

                case DOT11_CIPHER_ALGO_CCMP:
                    encryptionIV[0] = (UCHAR)(Key->PN & 0xff);
                    encryptionIV[1] = (UCHAR)((Key->PN >> 8) & 0xff);
                    encryptionIV[2] = 0;
                    encryptionIV[3] = 0x20 | (keyId << 6);
                    encryptionIV[4] = (UCHAR)((Key->PN >> 16) & 0xff);
                    encryptionIV[5] = (UCHAR)((Key->PN >> 24) & 0xff);
                    encryptionIV[6] = (UCHAR)((Key->PN >> 32) & 0xff);
                    encryptionIV[7] = (UCHAR)((Key->PN >> 40) & 0xff);

                    Key->PN++;

                    if (doSoftwareEncryption)
                    {
                        // Not supported for internal sends
                        MPASSERT(!HW_TX_MSDU_IS_INTERNAL(Msdu));
                        
                        ndisStatus = HwEncryptCCMP(Hw, Key->hCNGKey, netBuffer);

                        // Encryption is already done. Hardware does not
                        // need to do it
                        Msdu->SendEncrypted = FALSE;
                    }
                    
                    break;

                case DOT11_CIPHER_ALGO_TKIP:
                    encryptionIV[0] = (UCHAR)((Key->TSC >> 8) & 0xff);
                    encryptionIV[1] = (encryptionIV[0] | 0x20) & 0x7f;
                    encryptionIV[2] = (UCHAR)(Key->TSC & 0xff);
                    encryptionIV[3] = 0x20 | (keyId << 6);
                    encryptionIV[4] = (UCHAR)((Key->TSC >> 16) & 0xff);
                    encryptionIV[5] = (UCHAR)((Key->TSC >> 24) & 0xff);
                    encryptionIV[6] = (UCHAR)((Key->TSC >> 32) & 0xff);
                    encryptionIV[7] = (UCHAR)((Key->TSC >> 40) & 0xff);

                    Key->TSC++;
                    
                    break;

                case DOT11_CIPHER_ALGO_WEP:     // this should not be there.
                default:
                    MPASSERT(FALSE);
                    break;
            }
            
        }while (FALSE);
         
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            if (lastMdl)
            {
                lastMdl->Next = NULL;
                NET_BUFFER_DATA_LENGTH(netBuffer) -= 8;
            }

            if (Mpdu->MICMdl)
            {
                NdisFreeMdl(Mpdu->MICMdl);
                Mpdu->MICMdl = NULL;
            }

            Mpdu->MICMdlAdded = FALSE;
        }

    }

    return ndisStatus;
}

VOID
HwSubmitReadyMSDUs(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    PHW_TX_MSDU                 Msdu;
    NDIS_STATUS                 ndisStatus;

    UNREFERENCED_PARAMETER(DispatchLevel);

    // Note that when a reset is running, it would conflict with this
    // code path. Currently this is protected by the AsyncFunction counter in the HW
    
    //
    // Starting from NextTxMSDUToSend, we will try to send, in a thread-safe manner,
    // as many TX_MSDUs as are ready and possible to send.
    //
    Msdu = &TxQueue->MSDUArray[TxQueue->NextToSend];
    while (HW_TX_MSDU_TEST_FLAG(Msdu, HW_TX_MSDU_IS_READY_FOR_SEND))
    {
        //
        // Ofcourse, multiple threads can be here simultaneouly. They all think this
        // Tx MSDU is the next one to be transmitted and is ready for sending. However, we
        // want only one thread to do the transmission (to avoid multiple sends of same
        // packet). All threads will try to become the sender of this packet through the
        // following Interlocked operation but only one will succeed.
        //
        if (HW_ACQUIRE_TX_MSDU_SEND_OWNERSHIP(Msdu))
        {
            //
            // This TX_MSDU is ready for sending and this thread has won the contention
            // for ownership of sending. Any thread that fails to win contention will
            // abondon trying to acquire it and will leave immediately
            //
            if (!Msdu->FailedDuringSend)
            {
                ndisStatus = HwSubmitMSDU(Hw, TxQueue, Msdu);
                if (ndisStatus == NDIS_STATUS_RESOURCES)
                {
                    // The packet could not be sent at this time. An attempt should
                    // be made to resend this packet again later. Stop sending any
                    // more packets to guarantee order of delivery
                    HW_RELEASE_TX_MSDU_SEND_OWNERSHIP(Msdu);
                    MpTrace(COMP_SEND, DBG_LOUD, ("Not enough resources to submit ready MSDU to HAL\n"));
                    break;
                }

                if (ndisStatus != NDIS_STATUS_SUCCESS)
                {
                    // Packet failed sending
                    Msdu->WaitForSendToComplete = FALSE;
                    Msdu->FailedDuringSend = TRUE;
                    
                }

                // Update the next Tx descriptor to send
                MP_INCREMENT_LIMIT_SAFE(TxQueue->NextToSend, TxQueue->NumMSDUAllocated);
                
            }
            else
            {
                // We skip over failed packets. We dont need to send them
                MP_INCREMENT_LIMIT_SAFE(TxQueue->NextToSend, TxQueue->NumMSDUAllocated);
            }
            
            //
            // Move onto next TX_MSDU to see if it is ready for sending
            //
            Msdu = &TxQueue->MSDUArray[TxQueue->NextToSend];
        }
        else
        {
            break;
        }
    }

    
}


ULONG
HwGetTxFreeDescNum(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue
    )
{
    return TxQueue->NumMSDUAllocated - HalGetTxDescBusyNum(Hw->Hal, TxQueue->HalQueueIndex);
}


//
// Return values:
//  NDIS_STATUS_SUCCESS: The packet was handed to hardware
//  NDIS_STATUS_RESOURCES: The packet could not be handed
//          to hardware due to lack of resources
//  NDIS_STATUS_FAILURE: Send failed due to reasons other
//          than low Hw resources
//
NDIS_STATUS
HwSubmitMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu
    )
{
    BOOLEAN         UseCoalesce;
    ULONG           NumTxDescNeeded, NumTxDescAvailable;
    NDIS_STATUS     ndisStatus;
    BOOLEAN         CanTransmit = FALSE;

    NumTxDescNeeded = HwCountTxResourceNeeded(Msdu, &UseCoalesce);
    NumTxDescAvailable = HwGetTxFreeDescNum(Hw, TxQueue);

    if (NumTxDescAvailable >= NumTxDescNeeded)
    {
        //
        // Enough descriptors to send the packet
        //
        CanTransmit = TRUE;
    }
    else
    {
        //
        // Not enough descriptors to send the NET_BUFFER_LIST as is, 
        // but might be able to transmit by coalescing
        //
        MpTrace(COMP_SEND, DBG_LOUD, ("Coalescing MSDU. Need %d descriptors, %d available\n",
            NumTxDescNeeded,
            NumTxDescAvailable)
            );
            
        NumTxDescNeeded = Msdu->SGElementListCount;   // One Desc per fragment
        if (NumTxDescAvailable > NumTxDescNeeded)
        {
            UseCoalesce = TRUE;
            CanTransmit = TRUE;
        }
    }
    
    if (CanTransmit)
    {
        if (HwAwake(Hw, FALSE) == FALSE)
        {
            // Radio is OFF, drop this packet
            ndisStatus = NDIS_STATUS_DOT11_POWER_STATE_INVALID;
        }
        else
        {
            if (UseCoalesce == FALSE)
            {
                ndisStatus = HwTransmitMSDU(Hw, TxQueue, Msdu, (USHORT)NumTxDescNeeded);
            }
            else 
            {
                ndisStatus = HwTransmitMSDUCoalesce(Hw, TxQueue, Msdu, (USHORT)NumTxDescNeeded);
                if (ndisStatus != NDIS_STATUS_SUCCESS && ndisStatus != NDIS_STATUS_RESOURCES)
                {
                    ndisStatus = NDIS_STATUS_FAILURE;   // Just a failure
                }
            }
        }
    }
    else 
    {
        MpTrace(COMP_TESTING, DBG_LOUD, ("Tx Queued: Needed %d, Available %d\n",
            NumTxDescNeeded,
            NumTxDescAvailable)
            );

        ndisStatus = NDIS_STATUS_RESOURCES;
    }
    
    return ndisStatus;
}




ULONG
HwCountTxResourceNeeded(
    _In_  PHW_TX_MSDU         Msdu,
    _Out_ BOOLEAN             *UseCoalesce
    )
{
    ULONG                   i, TotalDescNeeded = 0;
    USHORT                  usNumberOfMPDU = 0;
    SCATTER_GATHER_LIST    *pFragList;
    PNET_BUFFER             CurrentNetBuffer;
    PNET_BUFFER_LIST        pNetBufferList = MP_TX_MSDU_WRAPPED_NBL(Msdu->MpMsdu);

    if (HW_TX_MSDU_IS_INTERNAL(Msdu))
    {
        // For internal packets, we also use the coalesce buffer for sending
        *UseCoalesce = TRUE;
        return 1;
    }
    
    //
    // Initially assume that we do not need coalescing.
    //
    *UseCoalesce = FALSE;
   
    //
    // Each NB in the NBL could be a MPDU. However the OS 
    // only gives us a single NET_BUFFER in the NET_BUFFER_LIST.
    //
    CurrentNetBuffer = NET_BUFFER_LIST_FIRST_NB(pNetBufferList);
    while (CurrentNetBuffer != NULL)
    {
        pFragList = (PSCATTER_GATHER_LIST) Msdu->SGElementList[usNumberOfMPDU];
        
        if (pFragList->NumberOfElements >= HW_TX_COALESCE_THRESHOLD)
        {
            // It is better to coalesce this MSDU
            if (UseCoalesce != NULL)
                *UseCoalesce = TRUE;

            TotalDescNeeded = Msdu->SGElementListCount;
            goto done;
        }
        else
        {
            TotalDescNeeded += pFragList->NumberOfElements;
        }
        
        for (i = 0; i < pFragList->NumberOfElements; i++)
        {
            // This is the hardware limitation
            if (pFragList->Elements[i].Length <= 4)
            {
                if (UseCoalesce != NULL)
                    *UseCoalesce = TRUE;

                TotalDescNeeded = Msdu->SGElementListCount;
                goto done;
            }
        }
        
        usNumberOfMPDU++;
        CurrentNetBuffer = NET_BUFFER_NEXT_NB(CurrentNetBuffer);
    }

done:
    return TotalDescNeeded;
}



NDIS_STATUS 
HwTransmitMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  USHORT                  NumTxDescNeeded
    )
{
    NDIS_STATUS                 status = NDIS_STATUS_SUCCESS;
    USHORT                      i, j, k;
    PSCATTER_GATHER_LIST        SGList;
    PSCATTER_GATHER_ELEMENT     SGElement;
    ULONG                       mdlDataOffset;
    USHORT                      totalSGElementCount = 0, mdlSkipCount = 0;
    PNET_BUFFER                 currentNetBuffer;
    PUCHAR                      header = NULL;
    ULONG                       length;
    USHORT                      numTxDescUsed;
    HAL_TX_ITERATOR             firstIter, lastIter, descIter;
    HAL_TX_DESC_INFO            descInfo;
    BOOLEAN                     firstSegment;
    PDOT11_MAC_HEADER           macHeader;

    // We may skip some empty MDLs at the start. So the passed in number of
    // TxDescNeeded is ignored and we compute a new one
    UNREFERENCED_PARAMETER(NumTxDescNeeded);

    // Internal packets must be sent via the coalesce code path
    MPASSERT(!HW_TX_MSDU_IS_INTERNAL(Msdu));

    //
    // Fill a TX_DESC for each scatter gather element for each MPDU
    //
    currentNetBuffer = NET_BUFFER_LIST_FIRST_NB(MP_TX_MSDU_WRAPPED_NBL(Msdu->MpMsdu));
    for (i = 0; i < Msdu->SGElementListCount; i++)
    {
        SGList = (PSCATTER_GATHER_LIST) Msdu->SGElementList[i];
        mdlDataOffset = NET_BUFFER_CURRENT_MDL_OFFSET(currentNetBuffer);
        totalSGElementCount = totalSGElementCount + (USHORT)SGList->NumberOfElements;

        NdisQueryMdl(NET_BUFFER_FIRST_MDL(currentNetBuffer),
            &header,
            &length,
            NormalPagePriority
            );
        if (!header)
            return NDIS_STATUS_FAILURE;
        
        for (j = 0; j < (USHORT)SGList->NumberOfElements; j++) 
        {
            //
            // Get the scatter gather element for this TX_DESC
            //
            SGElement = &SGList->Elements[j];
            
            if (mdlDataOffset > 0 && SGElement->Length <= mdlDataOffset)
            {
                mdlDataOffset -= SGElement->Length;
                mdlSkipCount++;
            }
            else
            {
                break;
            }
         }
        currentNetBuffer = NET_BUFFER_NEXT_NB(currentNetBuffer);         
    }

    numTxDescUsed = totalSGElementCount - mdlSkipCount;

    // Reserve TX descriptor chain for this send
    status = HalReserveNextTxDescsForTransmit(Hw->Hal, TxQueue->HalQueueIndex, NumTxDescNeeded, &firstIter);
    if (status != NDIS_STATUS_SUCCESS)
        return status;

    descIter = firstIter;
    lastIter = firstIter;

    // Process each NET_BUFFER
    currentNetBuffer = NET_BUFFER_LIST_FIRST_NB(MP_TX_MSDU_WRAPPED_NBL(Msdu->MpMsdu));
    
    for (i = 0; i < Msdu->SGElementListCount; i++)
    {
        SGList = (PSCATTER_GATHER_LIST) Msdu->SGElementList[i];
        mdlDataOffset = NET_BUFFER_CURRENT_MDL_OFFSET(currentNetBuffer);
        firstSegment = TRUE;
        
        NdisQueryBufferSafe(NET_BUFFER_FIRST_MDL(currentNetBuffer),
            &header,
            &length,
            NormalPagePriority);
        MPASSERT(header != NULL);
        _Analysis_assume_( header != NULL );
        header += mdlDataOffset;
        for (j = 0; j < (USHORT)SGList->NumberOfElements; j++) 
        {
            //
            // Get the scatter gather element for this TX_DESC
            //
            SGElement = &SGList->Elements[j];
            
            if (mdlDataOffset > 0 && SGElement->Length <= mdlDataOffset)
            {
                mdlDataOffset -= SGElement->Length;
            }
            else
            {   
                descInfo.MacHeader = header;
                descInfo.FirstSeg = firstSegment;
                descInfo.LastSeg = (BOOLEAN)(j == (USHORT)SGList->NumberOfElements - 1);
                descInfo.PhysicalAddressLow = SGElement->Address.LowPart + mdlDataOffset;
                descInfo.BufferLen = SGElement->Length - mdlDataOffset;
                descInfo.PacketLen = NET_BUFFER_DATA_LENGTH(currentNetBuffer);
                
                for (k = 0; k < HW_TX_RATE_TABLE_SIZE; k++)
                    descInfo.RateTable[k] = Msdu->TxRateTable[k];
                    
                descInfo.Multicast = Msdu->MulticastDestination;
                descInfo.ShortPreamble = Msdu->UseShortPreamble;
                descInfo.MoreFrag = (BOOLEAN) ((i != Msdu->SGElementListCount - 1) && (j != SGList->NumberOfElements - 1));
                descInfo.RTSEnabled = Msdu->RTSEnabled;
                descInfo.CTSToSelf = Msdu->CTSToSelfEnabled;
                descInfo.RTSCTSRate = 22;           // CTS to self is always 11 Mbpa
                descInfo.NoEncrypt = !Msdu->SendEncrypted;
                if (Msdu->SendEncrypted)
                {
                    // Set the Key Index
                    MPASSERT(Msdu->Key != NULL);
                    descInfo.KeyIndex = Msdu->Key->NicKeyIndex;
                }
                
                descInfo.Broadcast = DOT11_IS_BROADCAST((header 
                                            + FIELD_OFFSET(DOT11_MAC_HEADER, Address1)));
                descInfo.FirstDescIter = firstIter;
                
                // Populate correct packet type
                descInfo.PacketType = HAL_PACKET_DATA;
                macHeader = (PDOT11_MAC_HEADER)descInfo.MacHeader;
                if (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_CONTROL)
                {
                    if (macHeader->FrameControl.Subtype == DOT11_CTRL_SUBTYPE_PS_POLL)
                    {
                        descInfo.PacketType = HAL_PACKET_PS_POLL;
                    }
                }
                else if (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_MANAGEMENT)
                {
                    if (macHeader->FrameControl.Subtype == DOT11_MGMT_SUBTYPE_PROBE_RESPONSE)
                    {
                        descInfo.PacketType = HAL_PACKET_PROBE_RESPONSE;
                    }
                }
                
                descInfo.EOL = FALSE;
                descInfo.PsBitSetting = Msdu->PsBitSetting;

                HalFillTxDescriptor(Hw->Hal, &descInfo, descIter);

                lastIter = descIter;
                HalTxIterMoveNext(Hw->Hal, &descIter);

                //
                // We only want to reduce MdlDataOffset from the first SG element
                //
                mdlDataOffset= 0;
                firstSegment = FALSE;
            }
        }
        
        currentNetBuffer = NET_BUFFER_NEXT_NB(currentNetBuffer);
    }

    MPASSERT(currentNetBuffer == NULL);

    // We need this info for send completion
    Msdu->TotalDescUsed = numTxDescUsed;
    
    MPASSERT(numTxDescUsed <= NumTxDescNeeded);
    
    Msdu->SendItem.DescNum = numTxDescUsed;
    Msdu->SendItem.firstIterator = firstIter;
    Msdu->SendItem.lastIterator = lastIter;

    MP_RECORD_STRING_3(TxQueue->Tracking_SendRecorder, "SEND: %d %p %p", numTxDescUsed, 
        Msdu->SendItem.firstIterator,
        Msdu->SendItem.lastIterator
        );

    // It is in send
    HW_TX_MSDU_SET_FLAG(Msdu, HW_TX_MSDU_IS_IN_SEND);
    
    HW_TIMESTAMP_TX_MSDU(Msdu, TxQueue);    
    HW_INCREMENT_HAL_PENDING_TX(TxQueue);
    HalTransmit(Hw->Hal, &Msdu->SendItem);
    
    return NDIS_STATUS_SUCCESS;
}



NDIS_STATUS
HwCopyNBLToBuffer(
    _In_  PNET_BUFFER_LIST    NetBufferList,
    _Out_writes_bytes_(MAX_TX_RX_PACKET_SIZE) PUCHAR              pDest,
    _Out_ PULONG              BytesCopied
    ) 
{
    NDIS_STATUS     ndisStatus = NDIS_STATUS_SUCCESS;
    PNET_BUFFER     CurrentNetBuffer;
    PMDL            CurrentMdl;
    LONG            CurrLength;
    ULONG           TotalLength;
    PUCHAR          pSrc;
    
    __try
    {
        *BytesCopied = 0;
        NdisZeroMemory(pDest, MAX_TX_RX_PACKET_SIZE);

        for (CurrentNetBuffer = NET_BUFFER_LIST_FIRST_NB(NetBufferList);
             CurrentNetBuffer != NULL;
             CurrentNetBuffer = NET_BUFFER_NEXT_NB(CurrentNetBuffer))
        {
            CurrentMdl = NET_BUFFER_CURRENT_MDL(CurrentNetBuffer);

            pSrc = MmGetSystemAddressForMdlSafe(CurrentMdl, NormalPagePriority);
            if (!pSrc)
            {
                ndisStatus = NDIS_STATUS_RESOURCES;
                __leave;
            }
            
            //
            // For the first MDL with data, we need to skip the free space
            //
            pSrc += NET_BUFFER_CURRENT_MDL_OFFSET(CurrentNetBuffer);
            CurrLength = MmGetMdlByteCount(CurrentMdl) - NET_BUFFER_CURRENT_MDL_OFFSET(CurrentNetBuffer);

            if (CurrLength > 0)
            {
                TotalLength = ((ULONG) CurrLength) + *BytesCopied;
                if ((TotalLength < ((ULONG) CurrLength)) || (TotalLength > HW11_MAX_FRAME_SIZE))
                {
                    MpTrace(COMP_SEND, DBG_SERIOUS, ("%s: The total MSDU size (%d) is greater than max "
                        "allowed (%d)\n", __FUNCTION__, CurrLength + *BytesCopied, HW11_MAX_FRAME_SIZE));
                    
                    ndisStatus = NDIS_STATUS_INVALID_LENGTH;
                    break;  /* break out for loop */
                }
                
                //
                // Copy the data.
                // 
                NdisMoveMemory(pDest, pSrc, CurrLength);
                *BytesCopied += CurrLength;
                pDest += CurrLength;
                
//                MPASSERTMSG("The total MSDU size is greater than max allowed\n",
//                            *BytesCopied <= MP_802_11_MAX_FRAME_SIZE);
            }

            CurrentMdl = NDIS_MDL_LINKAGE(CurrentMdl);
            while (CurrentMdl)
            {
                pSrc = MmGetSystemAddressForMdlSafe(CurrentMdl, NormalPagePriority);
                if (!pSrc)
                {
                    ndisStatus = NDIS_STATUS_RESOURCES;
                    __leave;
                }

                CurrLength = MmGetMdlByteCount(CurrentMdl);

                if (CurrLength > 0)
                {
                    TotalLength = ((ULONG) CurrLength) + *BytesCopied;
                    if ((TotalLength < ((ULONG) CurrLength)) || (TotalLength > HW11_MAX_FRAME_SIZE))
                    {
                        MpTrace(COMP_SEND, DBG_SERIOUS, ("%s: The total MSDU size (%d) is greater than max "
                            "allowed (%d)\n", __FUNCTION__, CurrLength + *BytesCopied, HW11_MAX_FRAME_SIZE));
                        
                        ndisStatus = NDIS_STATUS_INVALID_LENGTH;
                        break;  /* break out while and for loop */
                    }

                    //
                    // Copy the data.
                    //
                    #pragma prefast(suppress:__WARNING_INCORRECT_VALIDATION, "Filed Esp:696")
                    NdisMoveMemory(pDest, pSrc, CurrLength);

                    *BytesCopied += CurrLength;
                    pDest += CurrLength;
                    
//                    MPASSERTMSG("The total MSDU size is greater than max allowed\n",
//                                *BytesCopied <= MP_802_11_MAX_FRAME_SIZE);
                }

                CurrentMdl = NDIS_MDL_LINKAGE(CurrentMdl);
            }
        }
    }
    __finally 
    {
    }

    return ndisStatus;
}


NDIS_STATUS
HwTransmitMSDUCoalesce(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  USHORT                  NumTxDescNeeded
    )
{
    NDIS_STATUS                 status = NDIS_STATUS_SUCCESS;
    ULONG                       bytesCopied;
    PMP_TX_MPDU                 mpMpdu;
    HAL_TX_ITERATOR             firstIter, descIter;
    HAL_TX_DESC_INFO            descInfo;
    USHORT                      i, j;
    ULONG                       dataOffset;
    PDOT11_MAC_HEADER           macHeader;
    //
    // Coalesce all the data in the coalesce buffer
    //
    if (HW_TX_MSDU_IS_INTERNAL(Msdu))
    {
        // Copy the internal buffer into this packet
        mpMpdu = MP_TX_MSDU_MPDU_AT(Msdu->MpMsdu, 0);
        bytesCopied = MP_TX_MPDU_LENGTH(mpMpdu);
        
        NdisMoveMemory(Msdu->BufferVa, MP_TX_MPDU_DATA(mpMpdu, bytesCopied), bytesCopied);
    }
    else
    {
        status = HwCopyNBLToBuffer(MP_TX_MSDU_WRAPPED_NBL(Msdu->MpMsdu),
                       Msdu->BufferVa,
                       &bytesCopied
                       );
    }
    
    if (status != NDIS_STATUS_SUCCESS)
    {
        //
        // Failed to coalesce this packet. We will fail the MP_TX_MSDU
        //
        MpTrace(COMP_SEND, DBG_NORMAL, ("Failed to Coalesce TX_MSDU into single buffer\n"));
        return status;
    }

    // Reserve TX descriptor chain for this send
    status = HalReserveNextTxDescsForTransmit(Hw->Hal, TxQueue->HalQueueIndex, NumTxDescNeeded, &firstIter);
    if (status != NDIS_STATUS_SUCCESS)
    {
        return status;
    }
    
    //
    // Fill a TX_DESC for each scatter gather element for each MPDU
    //
    if (NumTxDescNeeded > 1)
    {
        //
        // We will start from the 2nd MPDU (if present) and will fill in the
        // first one when we are done with all the others. 2nd MPDU means 2nd NB
        // We will also need to skip the data offset (free space) and data length
        // of the first NetBuffer as well.
        //
        MPASSERT(Msdu->MpduCount > 1);
        MPASSERT(!HW_TX_MSDU_IS_INTERNAL(Msdu));
        
        mpMpdu = MP_TX_MSDU_MPDU_AT(Msdu->MpMsdu, 0);
        dataOffset = NET_BUFFER_DATA_LENGTH(MP_TX_MPDU_WRAPPED_NB(mpMpdu));
    }
    else
    {
        //
        // There is only on MPDU so only one TX_DESC needed
        // We will not enter the loop and directly fill the
        // TX_DESC for the first NetBuffer
        //
        dataOffset = 0;
    }

        
    descIter = firstIter;    
    for (i = 1; i < NumTxDescNeeded; i++)
    {
        mpMpdu = MP_TX_MSDU_MPDU_AT(Msdu->MpMsdu, i);

        //
        // Get the next descriptor we will be filling. We leave the first
        // descriptor for later use
        //
        HalTxIterMoveNext(Hw->Hal, &descIter);
     
        descInfo.MacHeader = Msdu->BufferVa + dataOffset;
        descInfo.FirstSeg = TRUE;
        descInfo.LastSeg = TRUE;
        descInfo.PhysicalAddressLow = Msdu->BufferPa.LowPart + dataOffset;
        descInfo.BufferLen = MP_TX_MPDU_LENGTH(mpMpdu);
        descInfo.PacketLen = MP_TX_MPDU_LENGTH(mpMpdu);
        
        for (j = 0; j < HW_TX_RATE_TABLE_SIZE; j++)
            descInfo.RateTable[j] = Msdu->TxRateTable[j];
            
        descInfo.Multicast = Msdu->MulticastDestination;
        descInfo.ShortPreamble = Msdu->UseShortPreamble;
        descInfo.MoreFrag = (BOOLEAN)(i != NumTxDescNeeded - 1);
        descInfo.RTSEnabled = Msdu->RTSEnabled;
        descInfo.CTSToSelf = Msdu->CTSToSelfEnabled;
        descInfo.RTSCTSRate = 22;           // 11 Mbps
        descInfo.NoEncrypt = !Msdu->SendEncrypted;
        if (Msdu->SendEncrypted)
        {
            // Set the Key Index
            MPASSERT(Msdu->Key != NULL);
            descInfo.KeyIndex = Msdu->Key->NicKeyIndex;
        }
        
        descInfo.Broadcast = DOT11_IS_BROADCAST((Msdu->BufferVa 
                                + dataOffset + FIELD_OFFSET(DOT11_MAC_HEADER, Address1)));
        descInfo.FirstDescIter = firstIter;

        // Populate correct packet type
        descInfo.PacketType = HAL_PACKET_DATA;
        macHeader = (PDOT11_MAC_HEADER)descInfo.MacHeader;
        if (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_CONTROL)
        {
            if (macHeader->FrameControl.Subtype == DOT11_CTRL_SUBTYPE_PS_POLL)
            {
                descInfo.PacketType = HAL_PACKET_PS_POLL;
            }
        }
        else if (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_MANAGEMENT)
        {
            if (macHeader->FrameControl.Subtype == DOT11_MGMT_SUBTYPE_PROBE_RESPONSE)
            {
                descInfo.PacketType = HAL_PACKET_PROBE_RESPONSE;
            }
        }

        descInfo.EOL = FALSE;
        descInfo.PsBitSetting = Msdu->PsBitSetting;

        HalFillTxDescriptor(Hw->Hal, &descInfo, descIter);

        //
        // One more TX_DESC used up.
        // Move to the next MPDU
        //
        dataOffset += MP_TX_MPDU_LENGTH(mpMpdu);
    }

    //
    // Now fill out TX_DESC for the first MPDU
    //
    mpMpdu = MP_TX_MSDU_MPDU_AT(Msdu->MpMsdu, 0);
    
    descInfo.MacHeader = Msdu->BufferVa;
    descInfo.FirstSeg = TRUE;
    descInfo.LastSeg = TRUE;
    descInfo.PhysicalAddressLow = Msdu->BufferPa.LowPart;
    descInfo.BufferLen = MP_TX_MPDU_LENGTH(mpMpdu);
    descInfo.PacketLen = MP_TX_MPDU_LENGTH(mpMpdu);
    
    for (j = 0; j < HW_TX_RATE_TABLE_SIZE; j++)
        descInfo.RateTable[j] = Msdu->TxRateTable[j];
        
    descInfo.Multicast = Msdu->MulticastDestination;
    descInfo.ShortPreamble = Msdu->UseShortPreamble;
    descInfo.MoreFrag = (BOOLEAN)(NumTxDescNeeded > 1);
    descInfo.RTSEnabled = Msdu->RTSEnabled;
    descInfo.CTSToSelf = Msdu->CTSToSelfEnabled;
    descInfo.RTSCTSRate = 22;           // 11 Mbps
    descInfo.NoEncrypt = !Msdu->SendEncrypted;
    if (Msdu->SendEncrypted)
    {
        // Set the Key Index
        MPASSERT(Msdu->Key != NULL);
        descInfo.KeyIndex = Msdu->Key->NicKeyIndex;
    }
    descInfo.Broadcast = DOT11_IS_BROADCAST((Msdu->BufferVa + 
                                FIELD_OFFSET(DOT11_MAC_HEADER, Address1)));
    descInfo.FirstDescIter = firstIter;
    // Populate correct packet type
    descInfo.PacketType = HAL_PACKET_DATA;
    macHeader = (PDOT11_MAC_HEADER)descInfo.MacHeader;
    if (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_CONTROL)
    {
        if (macHeader->FrameControl.Subtype == DOT11_CTRL_SUBTYPE_PS_POLL)
        {
            descInfo.PacketType = HAL_PACKET_PS_POLL;
        }
    }
    else if (macHeader->FrameControl.Type == DOT11_FRAME_TYPE_MANAGEMENT)
    {
        if (macHeader->FrameControl.Subtype == DOT11_MGMT_SUBTYPE_PROBE_RESPONSE)
        {
            descInfo.PacketType = HAL_PACKET_PROBE_RESPONSE;
        }
    }
    descInfo.EOL = FALSE;
    descInfo.PsBitSetting = Msdu->PsBitSetting;

    HalFillTxDescriptor(Hw->Hal, &descInfo, descIter);

    // We need this info for send completion
    Msdu->TotalDescUsed = NumTxDescNeeded;

    Msdu->SendItem.DescNum = NumTxDescNeeded;
    Msdu->SendItem.firstIterator = firstIter;
    Msdu->SendItem.lastIterator = descIter;

    MP_RECORD_STRING_3(TxQueue->Tracking_SendRecorder, "SEND: %d %p %p", NumTxDescNeeded, 
        Msdu->SendItem.firstIterator,
        Msdu->SendItem.lastIterator
        );

    // It is in send
    HW_TX_MSDU_SET_FLAG(Msdu, HW_TX_MSDU_IS_IN_SEND);
    
    HW_INCREMENT_HAL_PENDING_TX(TxQueue);
    HalTransmit(Hw->Hal, &Msdu->SendItem);
    
    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
HwTransmitBeacon(
    _In_  PHW                     Hw,
    _In_  PHW_TX_MSDU             Msdu
    )
{
    HAL_TX_ITERATOR             descIterator;
    HAL_TX_DESC_INFO            descInfo; 
    NDIS_STATUS                 ndisStatus;
    BOOLEAN                     autoResend = TRUE;
    UCHAR                       i;
    
    ndisStatus = HalReserveNextTxDescsForTransmit(Hw->Hal, BEACON_QUEUE, 1, &descIterator);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        return ndisStatus;
    }

    if (Hw->MacState.OperationMode == DOT11_OPERATION_MODE_EXTENSIBLE_AP)
    {
        // For AP mode, we create a fresh beacon everytime
        autoResend = FALSE;
    }

    //MpTrace(COMP_TESTING, DBG_SERIOUS, ("BEACON: Transmit Beacon 0x%08x", (ULONG)descIterator));

    descInfo.MacHeader = Msdu->BufferVa;
    descInfo.FirstSeg = TRUE;
    descInfo.LastSeg = TRUE;
    descInfo.PhysicalAddressLow = Msdu->BufferPa.LowPart;
    descInfo.BufferLen = Msdu->TotalMSDULength;
    descInfo.PacketLen = Msdu->TotalMSDULength;
    
    for (i = 0; i < HW_TX_RATE_TABLE_SIZE; i++)
        descInfo.RateTable[i] = Msdu->TxRateTable[i];

    descInfo.Multicast = TRUE;
    descInfo.ShortPreamble = FALSE;
    descInfo.MoreFrag = FALSE;
    descInfo.RTSEnabled = FALSE;
    descInfo.CTSToSelf = FALSE;
    descInfo.RTSCTSRate = 0;
    descInfo.NoEncrypt = TRUE;
    descInfo.Broadcast = TRUE;
    descInfo.FirstDescIter = descIterator;
    descInfo.PacketType = HAL_PACKET_BEACON;
    descInfo.EOL = autoResend ;
    descInfo.PsBitSetting = TxDescPsBitClear;
    descInfo.KeyIndex = 0;

    HalFillTxDescriptor(Hw->Hal, &descInfo, descIterator);  

    Msdu->SendItem.DescNum = 1;
    Msdu->SendItem.firstIterator = descIterator;
    Msdu->SendItem.lastIterator = descIterator;

    HalTransmitBeacon(Hw->Hal, &Msdu->SendItem, autoResend );

    return NDIS_STATUS_SUCCESS;
}

BOOLEAN
HwTxMSDUIsComplete(
    _In_  PHW                     Hw,
    _In_  PHW_TX_MSDU             Msdu
    )
{
    ULONG                       i;
    HAL_TX_DESC_STATUS          currentDescStatus;
    HAL_TX_ITERATOR             descIter;
    BOOLEAN                     msduFailed = FALSE;
    PDOT11_PHY_FRAME_STATISTICS phyStats;

    if (!HW_TX_MSDU_TEST_FLAG(Msdu, HW_TX_MSDU_HAS_SENDER))
    {
        // The packet is not yet been "done" by the TX path. We cannot send
        // complete this yet
        return FALSE;
    }
    
    if (Msdu->FailedDuringSend)
    {
        MPASSERT(Msdu->MpMsdu->MacContext != NULL);
    
        //
        // Failed during send. We can complete this packet
        //
        if (Msdu->MulticastDestination)
            Msdu->MpMsdu->MacContext->MulticastCounters.ullTransmittedFailureFrameCount++;
        else            
            Msdu->MpMsdu->MacContext->UnicastCounters.ullTransmittedFailureFrameCount++;

        return TRUE;
    }
    else if (!HW_TX_MSDU_TEST_FLAG(Msdu, HW_TX_MSDU_IS_IN_SEND))
    {
        // Packet has not yet been submitted to the HAL
        return FALSE;
    }

    MPASSERT(Msdu->WaitForSendToComplete);

    if (HalGetTxDescBusyNum(Hw->Hal, Hw->TxInfo.TxQueue[Msdu->QueueID].HalQueueIndex) > 0)
    {
        if (HalIsSendItemCompleted(Hw->Hal, &Msdu->SendItem))
        {
            Msdu->SucceedMPDUCount = 0;
            phyStats = &Hw->Stats.PhyCounters[Msdu->PhyId];

            MP_RECORD_STRING_2(Hw->TxInfo.TxQueue[Msdu->QueueID].Tracking_SendRecorder, "COMP: %p %p", 
                Msdu->SendItem.firstIterator,
                Msdu->SendItem.lastIterator
                );
            
            descIter = Msdu->SendItem.firstIterator;
            for (i = 0; i < Msdu->TotalDescUsed; i++) 
            {
                HalGetTxDescStatus(Hw->Hal, descIter, &currentDescStatus);
                
                if (currentDescStatus.LastSegment)
                {
                    if (currentDescStatus.TransmitSuccess)
                    {
                        phyStats->ullTransmittedFrameCount++;
                        phyStats->ullTransmittedFragmentCount++;

                        if(Msdu->MulticastDestination)
                        {
                            phyStats->ullMulticastTransmittedFrameCount++;
                        }

                        if(currentDescStatus.RetryCount > 0)
                        {
                            phyStats->ullRetryCount++;
                            if (currentDescStatus.RetryCount > 1)
                            {
                                phyStats->ullMultipleRetryCount++;
                            }
                        }

                    }
                    else
                    {                    
                        phyStats->ullFailedCount++;
                        msduFailed = TRUE;
                    }

                    HwUpdatePeerTxStatistics(Msdu->MpMsdu->MacContext, 
                        Msdu, 
                        currentDescStatus
                        );

                    if(currentDescStatus.RetryCount > 0)
                    {
                        phyStats->ullACKFailureCount += currentDescStatus.RetryCount;
                    }

                    if (Msdu->RTSEnabled)
                    {
                        phyStats->ullRTSFailureCount++;
                        if (currentDescStatus.RtcRetryCount < Msdu->MpMsdu->MacContext->LongRetryLimit)
                        {
                            phyStats->ullRTSSuccessCount++;
                        }
                    }
                }

                HalTxIterMoveNext(Hw->Hal, &descIter);
            }

            if (msduFailed == FALSE)
            {
                Msdu->TxSucceeded = TRUE;

                if (Msdu->MulticastDestination)
                    Msdu->MpMsdu->MacContext->MulticastCounters.ullTransmittedFrameCount++;
                else            
                    Msdu->MpMsdu->MacContext->UnicastCounters.ullTransmittedFrameCount++;
            }

            Msdu->WaitForSendToComplete = FALSE;

            HW_DECREMENT_HAL_PENDING_TX(&Hw->TxInfo.TxQueue[Msdu->QueueID]);            
            HalReleaseTxDesc(Hw->Hal, Msdu->SendItem.firstIterator, Msdu->SendItem.lastIterator, Msdu->SendItem.DescNum);            

            Msdu->TotalDescUsed = 0;

            
            return TRUE;

        }
        
    }
    
    return FALSE;
}

VOID
HwFreeTxResources(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu
    )
{
    PHW_TX_MPDU                 mpdu;
    ULONG                       i;
    PNET_BUFFER                 currentNetBuffer;
    
    UNREFERENCED_PARAMETER(TxQueue);
    
    //
    // Free the SG lists
    //
    for (i = 0; i < Msdu->SGElementListCount; i++)
    {
        mpdu = HW_TX_MSDU_MPDU_AT(Msdu, i);    
        currentNetBuffer = MP_TX_MPDU_WRAPPED_NB(mpdu->MpMpdu);
        MPASSERT(currentNetBuffer != NULL);
        
        NdisMFreeNetBufferSGList(
            Hw->MiniportDmaHandle,
            Msdu->SGElementList[i],
            currentNetBuffer
            );
    }

    //
    // Free the MPDUs
    //
    for (i = 0; i < HW_TX_MSDU_MPDU_COUNT(Msdu); i++)
    {
        mpdu = HW_TX_MSDU_MPDU_AT(Msdu, i);

        // If we retreated for the IV
        if (mpdu->RetreatedSize > 0)
        {
            if (!HW_TX_MSDU_IS_INTERNAL(Msdu))
            {            
                NdisAdvanceNetBufferDataStart(MP_TX_MPDU_WRAPPED_NB(mpdu->MpMpdu), 
                      mpdu->RetreatedSize,
                      FALSE,
                      NULL
                      );
            }
            else
            {
                mpdu->MpMpdu->InternalSendDataStart -= mpdu->RetreatedSize;
                mpdu->MpMpdu->InternalSendLength = mpdu->MpMpdu->InternalSendLength 
                                                    + mpdu->RetreatedSize;
            }
        }

        // If we appended a MIC MDL
        if (mpdu->MICMdlAdded)
        {
            MPASSERT(!HW_TX_MSDU_IS_INTERNAL(Msdu));
            MPASSERT(mpdu->MICMdl != NULL);
            
            NdisAdjustMdlLength(mpdu->LastMdl, mpdu->LastMdlByteCount);
            mpdu->LastMdl->Next = mpdu->MICMdl->Next;

            NET_BUFFER_DATA_LENGTH(MP_TX_MPDU_WRAPPED_NB(mpdu->MpMpdu)) -= 8;
            NdisFreeMdl(mpdu->MICMdl);
        }

        NdisFreeToNPagedLookasideList(&(Hw->TxInfo.TxMPDULookaside), mpdu);    
    }
    

    // Release the MSDU structure
    HW_RELEASE_TX_MSDU(Msdu);

}

VOID
HwCompleteTxMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  BOOLEAN                 Success
    )
{
    if (Success)
        MP_TX_MSDU_STATUS(Msdu->MpMsdu) = NDIS_STATUS_SUCCESS;
    else
        MP_TX_MSDU_STATUS(Msdu->MpMsdu) = NDIS_STATUS_FAILURE;

    // Free the MPDUs assigned for this MSDU
    HwFreeTxResources(Hw, TxQueue, Msdu);

    // Next to complete is incremented
    MP_INCREMENT_LIMIT_UNSAFE(TxQueue->NextToComplete, (LONG)TxQueue->NumMSDUAllocated);
    HW_INCREMENT_AVAILABLE_TX_MSDU(TxQueue);
}


VOID
HwCheckSendQueueForCompletion(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue
    )
{
    PHW_TX_MSDU                 msdu;
    PMP_TX_MSDU                 mpMsduToComplete = NULL;
    PHW_MAC_CONTEXT             currentMacContext = NULL;
    ULONG                       completedCount = 0;
    PMP_TX_MSDU                 currentPacket = NULL;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    LONG                        firstMsduIndex, numMsduReserved = 0;

    HW_TX_ACQUIRE_QUEUE_LOCK(Hw, TxQueue, TRUE);
    msdu = &TxQueue->MSDUArray[TxQueue->NextToComplete];

    if ((TxQueue->NumMSDUAvailable < (LONG)(TxQueue->NumMSDUAllocated - 1)) &&
        (HwTxMSDUIsComplete(Hw, msdu)))
    {
//        HW_CHECK_TX_MSDU_TIME(msdu, 0);
        do
        {
            MpTrace(COMP_SEND, DBG_LOUD, ("Completing %d on %d\n", TxQueue->NextToComplete, TxQueue->HalQueueIndex));
        
            // Add this to the chain of MSDUs to complete to the upper layer
            MP_TX_MSDU_NEXT_MSDU(msdu->MpMsdu) = mpMsduToComplete;
            mpMsduToComplete = msdu->MpMsdu;
            
            currentMacContext = MP_TX_MSDU_MAC_CONTEXT(msdu->MpMsdu);

            // Cleanup and do appropriate completion for the MSDU
            //
            // If the packet failed during send, then we report failure to the OS
            // We do not report failure if the packet failed due to too many retries
            //            
            HwCompleteTxMSDU(Hw, TxQueue, msdu, !msdu->FailedDuringSend);

            // Decrement the recount on the MAC
            HW_REMOVE_MAC_CONTEXT_SEND_REF(currentMacContext, 1);

            // Next to complete has already been incremented
            msdu = &TxQueue->MSDUArray[TxQueue->NextToComplete];

            completedCount++;
        } while ((TxQueue->NumMSDUAvailable < (LONG)(TxQueue->NumMSDUAllocated - 1)) &&
            HwTxMSDUIsComplete(Hw, msdu));

        //
        // Since we completed some packets, reset the send ticks
        //
        TxQueue->StalledSendTicks = 0;
        
        HW_TX_RELEASE_QUEUE_LOCK(Hw, TxQueue, TRUE);

        HwSendCompletePackets(Hw, 
            mpMsduToComplete, 
            completedCount,
            NDIS_SEND_COMPLETE_FLAGS_DISPATCH_LEVEL
            ); 

        HW_TX_ACQUIRE_QUEUE_LOCK(Hw, TxQueue, TRUE);
    }

    // Check if we have any pending packets that should be put on the TX list of this array


    // This is the first index we would use for sending. From this index, upto
    // the numMsduReserved are the MSDUs that were prepared by this code and should
    // be used for sending
    firstMsduIndex = TxQueue->NextToReserve;
    
    while (!MpPacketQueueIsEmpty(&TxQueue->PendingTxQueue) &&
           !HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_SEND_FLAGS) &&
            (numMsduReserved < MAX_SEND_MSDU_TO_PROCESS))
    {
        // Remove a packet from the TX queue
        currentPacket = MP_MSDU_FROM_QUEUE_ENTRY(MpDequeuePacket(&TxQueue->PendingTxQueue));
        
        //
        // Check if there are any sends sitting in the pending queue. If not
        // we can process the new ones for sending. Else we should not to avoid
        // out of order delivery
        //
        if (HwCanTransmit(TxQueue))
        {
            //
            // Lets reserve HW_TX_MSDU for this packet. Reservation guarantees
            // that no one else will use the reserved TX_MSDU and also avoids
            // synchronization issues later
            //
            ndisStatus = HwReserveTxResources(Hw, TxQueue, currentPacket);
            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                numMsduReserved++;
            }

        }
        else
        {
            // Cannot send now
            ndisStatus = NDIS_STATUS_PENDING;
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            //
            // Put it back at the head of the pending sends queue
            //
            MpQueuePacketPriority(&TxQueue->PendingTxQueue, QUEUE_ENTRY_FROM_MP_MSDU(currentPacket));
            break;
        }

    }
    
    HW_TX_RELEASE_QUEUE_LOCK(Hw, TxQueue, TRUE);

    //
    // Process all the packets that we have been queued (this should be called even
    // if we dont have any new packets since it would forward existing packets to the HW)    
    //
    HwProcessReservedTxPackets(Hw, 
        TxQueue,
        firstMsduIndex,
        numMsduReserved,
        NDIS_SEND_FLAGS_DISPATCH_LEVEL
        );
    
}


VOID
HwReinitializeSendQueue(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    ULONG                       i;
    PHW_TX_MSDU                 msdu;
    PVOID                       Caller, CallersCaller;

    //
    // We save the address of the caller in the pool header, for debugging.
    //
    RtlGetCallersAddress(&Caller, &CallersCaller);
    HW_TX_ACQUIRE_QUEUE_LOCK(Hw, TxQueue, DispatchLevel);

    // We leave one descriptor empty for easy synchronization
    TxQueue->NumMSDUAvailable = TxQueue->NumMSDUAllocated - 1;
    
    // Reset the MSDUs
    for (i = 0; i < TxQueue->NumMSDUAllocated; i++)
    {
        msdu = &TxQueue->MSDUArray[i];
            
        msdu->Flags = 0;
        msdu->MpMsdu = NULL;
        msdu->PeerNode = NULL;
        msdu->PhyId = DOT11_PHY_ID_ANY;
        msdu->SGElementListCount = 0;
    }

    TxQueue->StalledSendTicks = 0;
    
    // Reset the HAL queues
    if (HalResetTxDescs(Hw->Hal, TxQueue->HalQueueIndex) != NDIS_STATUS_SUCCESS)
    {
        // The HAL TX queues did not successfully reset. We do not 
        // reset send from the current descriptor
        // MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("HalResetTxDescs for queue %d failed\n", TxQueue->HalQueueIndex));        
    }
    else
    {
        TxQueue->NextToSend = 0;   // Start from descriptor 0
        TxQueue->NextToComplete = 0;
        TxQueue->NextToReserve = 0;
    }

    MP_RECORD_STRING_3(TxQueue->Tracking_SendRecorder, "RSET: %d %p %p", 
        TxQueue->NextToSend,
        Caller,
        CallersCaller
        );

    TxQueue->NumPending = 0;

    HW_TX_RELEASE_QUEUE_LOCK(Hw, TxQueue, DispatchLevel);
}


VOID 
HwSendCompletePackets(
    _In_  PHW                     Hw,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   NumberOfPackets,
    _In_  ULONG                   SendCompleteFlags
    )
{
    PMP_TX_MSDU                 currentPacket, nextPacket;
    PMP_TX_MSDU                 prevPacket = NULL, packetsToComplete = NULL;
    BOOLEAN                     indicateList = FALSE, newCompletion;
    ULONG                       processedCount = 0;
    UNREFERENCED_PARAMETER(NumberOfPackets);
    UNREFERENCED_PARAMETER(Hw);

    currentPacket = PacketList;
    
    while (currentPacket != NULL)
    {
        // Save the next in case we do the complete
        nextPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket);
        MP_TX_MSDU_NEXT_MSDU(currentPacket) = NULL;
        newCompletion = FALSE;
        
        if (prevPacket != NULL)
        {
            // Compare the completion point of this packet with the 
            // previous ones

            if (MP_TX_MSDU_IS_PRIVATE(currentPacket) != MP_TX_MSDU_IS_PRIVATE(prevPacket))
            {
                // One of the two packets is private & the other is not. Complete the set
                // we have formed so far
                newCompletion = TRUE;
            }
            else if (MP_TX_MSDU_MAC_CONTEXT(currentPacket) != MP_TX_MSDU_MAC_CONTEXT(prevPacket))
            {
                // The MAC contexts of the two packets is not the same. Complete the set
                // we have formed so far
                newCompletion = TRUE;
            }
        }

        if (newCompletion == FALSE)
        {
            // This is an optimization. Since this MSDU belongs to the same completion point as 
            // previous one, lets add it to the list now and check if this was the last MSDU of the
            // chain. If yes, we can do the indication and be done
            MP_TX_MSDU_NEXT_MSDU(currentPacket) = packetsToComplete;
            packetsToComplete = currentPacket;
            prevPacket = currentPacket;
            
            processedCount++;

            currentPacket = NULL;  // This way code below wont add this to the list

            if (nextPacket == NULL)
            {
                // This was the last packet in the PacketList, lets complete
                // the chain we have
                indicateList = TRUE;
            }
        }
        else
        {
            // The code below (indicateList == TRUE & newCompletion == TRUE) would append the 
            // currentPacket to the list to the new list
            indicateList = TRUE;
        }

        if (indicateList)
        {
            // Complete the list of packets that we have processed so far to
            // the MAC context we have done so far
            if (MP_TX_MSDU_IS_PRIVATE(packetsToComplete))
            {
                HwSendCompletePrivatePackets(MP_TX_MSDU_MAC_CONTEXT(packetsToComplete), 
                    packetsToComplete, 
                    processedCount,
                    SendCompleteFlags
                    );
            }
            else
            {
                Hvl11SendCompletePackets(MP_TX_MSDU_MAC_CONTEXT(packetsToComplete)->VNic, 
                    packetsToComplete, 
                    processedCount,
                    SendCompleteFlags
                    );
            }

            // Reset state
            indicateList = FALSE;

            // Add the new (different) MSDU to the list we would use going forward
            if (newCompletion)
            {
                // Indicate was done because the completion point was changing
                MPASSERT(currentPacket != NULL);
                processedCount = 1;
                prevPacket = currentPacket;
                packetsToComplete = currentPacket;
            }
            else
            {
                // Indicate was done because we were at the end. Nothing to add
                processedCount = 0;
                prevPacket = NULL;
                packetsToComplete = NULL;
            }
        }

        currentPacket = nextPacket;
    }

    // Complete any remaining packet
    if (packetsToComplete != NULL)
    {
        // Because of the optimization above, this should only get hit as a corner case
        // if the last MSDU is different
        MPASSERT(processedCount == 1);
        
        if (MP_TX_MSDU_IS_PRIVATE(packetsToComplete))
        {
            HwSendCompletePrivatePackets(MP_TX_MSDU_MAC_CONTEXT(packetsToComplete), 
                packetsToComplete, 
                processedCount,
                SendCompleteFlags
                );
        }
        else
        {
            Hvl11SendCompletePackets(MP_TX_MSDU_MAC_CONTEXT(packetsToComplete)->VNic, 
                packetsToComplete, 
                processedCount,
                SendCompleteFlags
                );
        }
    }
}

_IRQL_requires_(DISPATCH_LEVEL)
VOID
HwFlushQueuedTxMSDUs(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    NDIS_STATUS                 ndisStatus = HwGetAdapterStatus(Hw);
    PMP_TX_MSDU                 currentMsdu, mpMsduToComplete = NULL;
    ULONG                       completedCount = 0;
    PHW_MAC_CONTEXT             currentMacContext = NULL;

    // Note: At this point, the interrupt may be running. So
    // stuff here needs to be done with the lock held

    _Analysis_suppress_lock_checking_((& Hw->Lock)->SpinLock)

    while (!MpPacketQueueIsEmpty(&TxQueue->PendingTxQueue))
    {
        // Remove a packet from the TX queue
        currentMsdu = MP_MSDU_FROM_QUEUE_ENTRY(MpDequeuePacket(&TxQueue->PendingTxQueue));
        
        MP_TX_MSDU_NEXT_MSDU(currentMsdu) = mpMsduToComplete;
        mpMsduToComplete = currentMsdu;

        MP_TX_MSDU_STATUS(currentMsdu) = ndisStatus;

        currentMacContext = MP_TX_MSDU_MAC_CONTEXT(currentMsdu);
        HW_REMOVE_MAC_CONTEXT_SEND_REF(currentMacContext, 1);

        completedCount++;

        // The stuff in the PendingTxQueue has not yet been processed
        // so we dont do cleanup on it        
    }

    //
    // Fix the TX_QUEUE so it will be empty.
    //
    TxQueue->PendingTxQueue.Head = NULL;
    TxQueue->PendingTxQueue.Tail = NULL;    
    TxQueue->PendingTxQueue.Count = 0;

    if (mpMsduToComplete)
    {
        //
        // Send Complete the NBLs to NDIS
        //
        HW_TX_RELEASE_QUEUE_LOCK(Hw, TxQueue, DispatchLevel);

        MpTrace(COMP_SEND, DBG_LOUD, ("Flushed %d queued TX MSDU\n", completedCount));

        HwSendCompletePackets(Hw, mpMsduToComplete, completedCount, 0);

        HW_TX_ACQUIRE_QUEUE_LOCK(Hw, TxQueue, DispatchLevel);
    }
}

_IRQL_requires_(DISPATCH_LEVEL)
VOID
HwFlushReservedTxMSDUs(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    PMP_TX_MSDU                 mpMsduToComplete = NULL;
    PHW_TX_MSDU                 currentMsdu;
    ULONG                       completedCount = 0, numTxDesc;
    PHW_MAC_CONTEXT             currentMacContext = NULL;

    // Note: At this point, the interrupt may be running. So
    // stuff here needs to be done with the lock held
        _Analysis_suppress_lock_checking_((& Hw->Lock)->SpinLock)

    // We may have submitted some to the hardware, but we dont know 
    // what state the hardware is currently in. Lets complete them all
    // Free all the MSDUs (total -1 since we leave one spot empty)
    numTxDesc = TxQueue->NumMSDUAllocated - TxQueue->NumMSDUAvailable - 1;  
    while (numTxDesc > 0)
    {
        currentMsdu = &TxQueue->MSDUArray[TxQueue->NextToComplete];

        // Attempt to return the descriptor to the hardware. This is best effort,
        // since we are anyways going to reset later
        if((TxQueue->NumPending > 0) && 
           HW_TX_MSDU_TEST_FLAG(currentMsdu, HW_TX_MSDU_IS_IN_SEND))
        {
            // This MSDU has been previously submitted to the HW, lets 
            // return it to the Hal since we would be resetting and not 
            // all HW can do reset TX individually
            HW_DECREMENT_HAL_PENDING_TX(TxQueue);
            
            HalReleaseTxDesc(Hw->Hal, 
                currentMsdu->SendItem.firstIterator, 
                currentMsdu->SendItem.lastIterator, 
                currentMsdu->SendItem.DescNum
                ); 
        }

        MP_TX_MSDU_NEXT_MSDU(currentMsdu->MpMsdu) = mpMsduToComplete;
        mpMsduToComplete = currentMsdu->MpMsdu;
        
        currentMacContext = MP_TX_MSDU_MAC_CONTEXT(currentMsdu->MpMsdu);

        // Cleanup and do appropriate completion for the MSDU
        HwCompleteTxMSDU(Hw, TxQueue, currentMsdu, FALSE);

        // Decrement the recount on the MAC
        HW_REMOVE_MAC_CONTEXT_SEND_REF(currentMacContext, 1);

        completedCount++;
        numTxDesc--;
    }

    if (mpMsduToComplete != NULL)
    {
        HW_TX_RELEASE_QUEUE_LOCK(Hw, TxQueue, DispatchLevel);

        MpTrace(COMP_SEND, DBG_LOUD, ("Flushed %d reserved TX MSDU\n", completedCount));

        HwSendCompletePackets(Hw, 
            mpMsduToComplete, 
            completedCount,
            0
            );

        HW_TX_ACQUIRE_QUEUE_LOCK(Hw, TxQueue, DispatchLevel);
    }    
}


VOID
HwFlushSendQueue(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    HW_TX_ACQUIRE_QUEUE_LOCK(Hw, TxQueue, DispatchLevel);

    // Flush the queued TX descriptors
    HwFlushQueuedTxMSDUs(Hw, TxQueue, DispatchLevel);
    
    // Flush the descriptors that we have reserved
    HwFlushReservedTxMSDUs(Hw, TxQueue, DispatchLevel);

    HW_TX_RELEASE_QUEUE_LOCK(Hw, TxQueue, DispatchLevel);
}

VOID
HwFlushSendEngine(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    // Complete pending packets from the queue
    HwFlushSendQueue(Hw, &Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE], DispatchLevel);
    HwFlushSendQueue(Hw, &Hw->TxInfo.TxQueue[HW11_INTERNAL_SEND_QUEUE], DispatchLevel);

}

VOID
HwResetSendEngine(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    // Note: At this point, the interrupt may be running. So
    // stuff here needs to be done with the lock held

    // Complete pending packets from the queue
    HwFlushSendEngine(Hw, DispatchLevel);

    // Reset the queues
    if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE) )
    {
        HwReinitializeSendQueue(Hw, &Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE], DispatchLevel);
        HwReinitializeSendQueue(Hw, &Hw->TxInfo.TxQueue[HW11_INTERNAL_SEND_QUEUE], DispatchLevel);
        HwReinitializeSendQueue(Hw, &Hw->TxInfo.TxQueue[HW11_UNUSED_QUEUE], DispatchLevel);
        HwReinitializeSendQueue(Hw, &Hw->TxInfo.TxQueue[HW11_BEACON_QUEUE], DispatchLevel);
    }
}

BOOLEAN
HwArePktsPendingInSendQueue(
    _In_  PHW_TX_QUEUE            TxQueue
    )
{
    if (TxQueue->NumMSDUAvailable < ((LONG)TxQueue->NumMSDUAllocated - 1))
    {
        return TRUE;
    }
    else
    {
        return FALSE;
    }
    
}

BOOLEAN
Hw11ArePktsPending(
    _In_  PHW                     Hw
    )
{
    BOOLEAN                     pktsPending = FALSE;

    // Check the default queue
    pktsPending = HwArePktsPendingInSendQueue(&Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE]);
    if (pktsPending)
    {
        return TRUE;
    }

    // Check the internal send queue
    pktsPending = HwArePktsPendingInSendQueue(&Hw->TxInfo.TxQueue[HW11_INTERNAL_SEND_QUEUE]);
    if (pktsPending)
    {
        return TRUE;
    }

    return FALSE;
}

BOOLEAN
HwCheckSendQueueForHang(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue
    )
{
    BOOLEAN                     isHung = FALSE;

    //
    // If we have pending packets in the HAL and the hardware is not flushing them it
    // maybe due to the timing window between software updating and hardware looking
    // at the TX descriptors
    //
    if (TxQueue->NumPending > 0)
    {
        // There are packets pending
        HalCheckForSends(Hw->Hal, TxQueue->HalQueueIndex, TRUE);
    }

    //
    // If we have packets pending in the HW layer, check if we may be hung
    //
    if (TxQueue->NumMSDUAvailable < 
        ((LONG)TxQueue->NumMSDUAllocated - 1))
    {
        // Check if we suspect we are in a bad state
        if (TxQueue->StalledSendTicks == 0)
        {
            // First time that we are tracking sends (or this has been reset on a send complete)
            // Save the next to complete
            TxQueue->StalledSendNextToCompleteSnapshot 
                 = TxQueue->NextToComplete;

            // Starting to track
            TxQueue->StalledSendTicks++;
        }
        else
        {
            //
            // We have previously seen pending sends. Check if anything has completed
            // since then
            //
            if (TxQueue->StalledSendNextToCompleteSnapshot 
                == TxQueue->NextToComplete)
            {
                // The next to send has not changed.    
                MpTrace(COMP_MISC, DBG_LOUD, ("%d sample periods of NextToComplete have occured\n", 
                    TxQueue->StalledSendTicks));

                //
                // If the number of sends pended on the Hw11 has not changed we will
                // count a tick interval of stalled send period.
                //
                TxQueue->StalledSendTicks++;

                if (TxQueue->StalledSendTicks == (HW_SENDS_HAVE_STALLED_PERIOD - 1))
                {
                    MpTrace(COMP_MISC, DBG_LOUD, ("If one more CheckForHang detects stalled sends, we will reset! Investigate\n"));
#if DEBUG_RESET
                    MPASSERTMSG(
                        "If one more CheckForHang detects stalled sends, we will reset! Investigate\n",
                        FALSE
                        );
#endif
                }
                else if(TxQueue->StalledSendTicks >= HW_SENDS_HAVE_STALLED_PERIOD)
                {
                    MpTrace(COMP_MISC, DBG_SERIOUS, ("Send Engine seems to be stalled. Requesting reset\n"));
                    isHung = TRUE;
                }
            }
            else
            {
                // Some sends have completed. Reset the ticks
                TxQueue->StalledSendTicks = 0;
            }
        }
    }

    return isHung;
}

BOOLEAN
HwCheckForSendHang(
    _In_  PHW                     Hw
    )
{
    BOOLEAN                     isHung = FALSE;

    // Check the default queue
    isHung = HwCheckSendQueueForHang(Hw, &Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE]);
    if (isHung)
    {
        MpTrace(COMP_MISC, DBG_SERIOUS, ("Requesting reset from NDIS\n"));
#if DEBUG_RESET
        MPASSERTMSG("Reset should not occur normally! Investigate\n", FALSE);
#endif
        return TRUE;
    }

    // Check the internal send queue
    isHung = HwCheckSendQueueForHang(Hw, &Hw->TxInfo.TxQueue[HW11_INTERNAL_SEND_QUEUE]);
    if (isHung)
    {
        MpTrace(COMP_MISC, DBG_SERIOUS, ("Requesting reset from NDIS\n"));
#if DEBUG_RESET
        MPASSERTMSG("Reset should not occur normally! Investigate\n", FALSE);    
#endif
        return TRUE;
    }

    return FALSE;
}

PMP_TX_MSDU
HwAllocatePrivatePacket(
    _In_  PHW                     Hw,
    _In_  USHORT                  PacketSize
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_TX_MSDU                 txPacket = NULL;
    PMP_TX_MPDU                 txFragment = NULL;
    PUCHAR                      dataBuffer = NULL;

    do
    {
        // TX Packet
        txPacket = NdisAllocateFromNPagedLookasideList(&(Hw->TxInfo.TxPacketLookaside));
        if (txPacket == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        NdisZeroMemory(txPacket, sizeof(MP_TX_MSDU  ));
        
        // TX Fragment
        txFragment = NdisAllocateFromNPagedLookasideList(&(Hw->TxInfo.TxFragmentLookaside));
        if (txFragment == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        NdisZeroMemory(txFragment, sizeof(MP_TX_MPDU));

        // Data buffer. We allocate extra space for the backfill that the H/W requires
        MP_ALLOCATE_MEMORY(Hw->MiniportAdapterHandle, 
            &dataBuffer, 
            PacketSize + HW11_REQUIRED_BACKFILL_SIZE, 
            HW_MEMORY_TAG
            );
        if (dataBuffer == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // Save this internal send buffer
        txFragment->InternalSendBuffer = dataBuffer;

        // Skip the backfill to get to the data start
        dataBuffer += HW11_REQUIRED_BACKFILL_SIZE;

        txFragment->InternalSendDataStart = dataBuffer;
        txFragment->InternalSendLength = PacketSize;
        MP_TX_MPDU_MSDU(txFragment) = txPacket;

        MP_TX_MSDU_MPDU_AT(txPacket, 0) = txFragment;
        MP_TX_MSDU_MPDU_COUNT(txPacket) = 1;
        txPacket->HardwarePrivate = TRUE;

        // These packets are always exempt from encryption. The HW layer
        // makes an assumption about this too & it wont encrypt the packets sent this way
        MP_TX_MSDU_SEND_CONTEXT(txPacket)->usExemptionActionType = DOT11_EXEMPT_ALWAYS;
        
        // Also must be sent using the current phy
        MP_TX_MSDU_SEND_CONTEXT(txPacket)->uPhyId = DOT11_PHY_ID_ANY;


    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (dataBuffer != NULL)
            MP_FREE_MEMORY(dataBuffer);

        if (txFragment != NULL)
            NdisFreeToNPagedLookasideList(&(Hw->TxInfo.TxFragmentLookaside), 
                txFragment);

        // Delete the packet
        if (txPacket != NULL)
            NdisFreeToNPagedLookasideList(&(Hw->TxInfo.TxPacketLookaside), 
                txPacket);

        txPacket = NULL;
    }

    return txPacket;
}

VOID
HwFreePrivatePacket(
    _In_  PHW                     Hw,
    _In_  PMP_TX_MSDU             Packet
    )
{
    MPASSERT(MP_TX_MSDU_IS_PRIVATE(Packet));
    MP_FREE_MEMORY(MP_TX_MSDU_MPDU_AT(Packet, 0)->InternalSendBuffer);

    NdisFreeToNPagedLookasideList(&(Hw->TxInfo.TxFragmentLookaside), 
        MP_TX_MSDU_MPDU_AT(Packet, 0));

    NdisFreeToNPagedLookasideList(&(Hw->TxInfo.TxPacketLookaside), Packet);
}

VOID
HwSendPrivatePackets(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendFlags
    )
{
    // Only single sends permitted
    MPASSERT(MP_TX_MSDU_NEXT_MSDU(PacketList) == NULL);
    
    // We forward this to the main send routine
    Hw11SendPackets(MacContext, PacketList, SendFlags);
}

VOID 
HwSendCompletePrivatePackets(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   NumberOfPackets,
    _In_  ULONG                   SendCompleteFlags
    )
{
    PMP_TX_MSDU                 currentPacket = PacketList, nextPacket;
    
    UNREFERENCED_PARAMETER(NumberOfPackets);
    UNREFERENCED_PARAMETER(SendCompleteFlags);

    while (currentPacket != NULL)
    {
        nextPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket);
        MP_TX_MSDU_NEXT_MSDU(currentPacket) = NULL;
        
        HwFreePrivatePacket(MacContext->Hw, currentPacket);
        
        currentPacket = nextPacket;
    }
}

NDIS_STATUS
Hw11SendNullPkt(
    _In_ PHW_MAC_CONTEXT          MacContext,
    _In_ BOOLEAN                  PowerSaveBitSet
    )
{
#ifndef PARTIAL_HAL
    PMP_TX_MSDU                 nullPacket = NULL;
    PDOT11_DATA_SHORT_HEADER    packetHeader;
    PUCHAR                      buffer;
    USHORT                      bufferLength = sizeof(DOT11_DATA_SHORT_HEADER);

    MpTrace(COMP_SEND, DBG_LOUD, ("HwMac(%d): Transmitting Null packet with PS bit %s\n", 
        HW_MAC_PORT_NO(MacContext), (PowerSaveBitSet ? "SET" : "CLEAR" )));

    nullPacket = HwAllocatePrivatePacket(MacContext->Hw, bufferLength);
    if (nullPacket == NULL)
    {
        return NDIS_STATUS_RESOURCES;
    }
    
    buffer = MP_TX_MPDU_DATA(MP_TX_MSDU_MPDU_AT(nullPacket, 0), bufferLength);
    NdisZeroMemory(buffer, bufferLength);

    // Populate the fields
    packetHeader = (PDOT11_DATA_SHORT_HEADER)buffer;
    if (!packetHeader)
    {
        return NDIS_STATUS_RESOURCES;
    }

    packetHeader->FrameControl.Type = DOT11_FRAME_TYPE_DATA;
    packetHeader->FrameControl.Subtype = DOT11_DATA_SUBTYPE_NULL;
    packetHeader->FrameControl.ToDS = 1;

    if (PowerSaveBitSet)
        packetHeader->FrameControl.PwrMgt = 1;
    else
        packetHeader->FrameControl.PwrMgt = 0;
    
    NdisMoveMemory(packetHeader->Address1, MacContext->DesiredBSSID, DOT11_ADDRESS_SIZE);
    NdisMoveMemory(packetHeader->Address2, MacContext->MacAddress, DOT11_ADDRESS_SIZE);
    NdisMoveMemory(packetHeader->Address3, MacContext->DesiredBSSID, DOT11_ADDRESS_SIZE);

    packetHeader->SequenceControl.usValue = 0;

    // Total length
    (MP_TX_MSDU_MPDU_AT(nullPacket, 0))->InternalSendLength = bufferLength;

    // Send the packet
    HwSendPrivatePackets(MacContext, nullPacket, 0);
#else
    UNREFERENCED_PARAMETER(MacContext);
    UNREFERENCED_PARAMETER(PowerSaveBitSet);
#endif

    return NDIS_STATUS_SUCCESS;
}

BOOLEAN
Hw11CanTransmit(
    _In_  PHW_MAC_CONTEXT         MacContext
    )
{
    //
    // When the upper layer calls asking whether packets can be sent or not,
    // we only report FALSE if we are in a state where packets would be failed.
    // We do not report for low resources.
    //
    /*
    if (HW_TEST_ADAPTER_STATUS(MacContext->Hw, HW_CANNOT_SEND_FLAGS))
    {
        return FALSE;
    }
    */

    UNREFERENCED_PARAMETER (MacContext);
    
    // BUGBUG: Add comments why we are returning true
    return TRUE;
}

VOID
Hw11SendPackets(
    _In_ PHW_MAC_CONTEXT         MacContext,
    _In_ PMP_TX_MSDU             PacketList,
    _In_ ULONG                   SendFlags
    )
{
    PMP_TX_MSDU                 currentPacket, nextPacket;
    PMP_TX_MSDU                 failedPackets = NULL;
    ULONG                       failedCount = 0;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PHW_TX_QUEUE                currentQueue = NULL;
    LONG                        firstMsduIndex, numMsduReserved = 0;
    BOOLEAN                     dispatchLevel = NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags);
    //
    // Add a send reference to the HW layer. This is tracking the number
    // of operations that are currently pending on the HW
    //
    HW_INCREMENT_ACTIVE_SEND_REF(MacContext->Hw);

    do
    {
        //
        // Check if the packets can be sent or not
        //
        if (HW_TEST_ADAPTER_STATUS(MacContext->Hw, HW_CANNOT_SEND_FLAGS))
        {
            ndisStatus = HwGetAdapterStatus(MacContext->Hw);
            for (currentPacket = PacketList; 
                    currentPacket != NULL; 
                    currentPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket))
            {
                MpTrace(COMP_SEND, DBG_LOUD, ("HW cannot transmit (0x%08x). Dropping send packets with status 0x%08x\n", 
                    MacContext->Hw->Status, ndisStatus));
                MP_TX_MSDU_STATUS(currentPacket) = ndisStatus;

                // The HwSendCompletePackets function needs the MAC context
                MP_TX_MSDU_MAC_CONTEXT(currentPacket) = MacContext;
                failedCount++;
            }
            failedPackets = PacketList;

            break;
        }
        
        currentPacket = PacketList;

        // NOTE: If we used multiple queues for send, the initialization below needs to 
        // be updated to work with an array

        if (MP_TX_MSDU_IS_PRIVATE(PacketList))
        {
            // Use the internal send queue for packets internal to the driver
            currentQueue = &MacContext->Hw->TxInfo.TxQueue[HW11_INTERNAL_SEND_QUEUE];
        }
        else
        {
            // Use the default queue for all other packet types
            currentQueue = &MacContext->Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE];
        }
        
        HW_TX_ACQUIRE_QUEUE_LOCK(MacContext->Hw, currentQueue, dispatchLevel);
        
        // This is the first index we would use for sending. From this index, upto
        // the numMsduReserved are the MSDUs that were prepared by this code and should
        // be used for sending
        firstMsduIndex = currentQueue->NextToReserve;
        
        while (currentPacket != NULL)
        {
            nextPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket);
            MP_TX_MSDU_NEXT_MSDU(currentPacket) = NULL;

            // Save the MAC context so that on send complete we
            // can identify the sending MAC context. Also add a ref to the MAC
            MP_TX_MSDU_MAC_CONTEXT(currentPacket) = MacContext;
            HW_ADD_MAC_CONTEXT_SEND_REF(MacContext, 1);

            
            //
            // Check if there are any sends sitting in the pending queue. If not
            // we can process the new ones for sending. Else we should not to avoid
            // out of order delivery
            //
            if (MpPacketQueueIsEmpty(&currentQueue->PendingTxQueue) &&
                HwCanTransmit(currentQueue))
            {
                //
                // Lets reserve HW_TX_MSDU for this packet. Reservation guarantees
                // that no one else will use the reserved TX_MSDU and also avoids
                // synchronization issues later
                //
                ndisStatus = HwReserveTxResources(MacContext->Hw, currentQueue, currentPacket);
                if (ndisStatus == NDIS_STATUS_SUCCESS)
                {
                    numMsduReserved++;
                }
                else
                {
                    //
                    // If we cannot reserve resources to send this packet, 
                    // put it in the pending queue
                    //
                    ndisStatus = NDIS_STATUS_PENDING;
                }
            }
            else
            {
                // Cannot send now
                ndisStatus = NDIS_STATUS_PENDING;
            }

            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                //
                // Put it on the pending sends queue
                //
                if (currentQueue->PendingTxQueue.Count < currentQueue->MaxPendingTx)
                {
                    MpQueuePacket(&currentQueue->PendingTxQueue, QUEUE_ENTRY_FROM_MP_MSDU(currentPacket));
                }
                else
                {
                    //
                    // There are already too many packets in the pending queue. Or we are
                    // not allowed to queue packets on this queue. Drop the packet
                    //
                    MP_TX_MSDU_NEXT_MSDU(currentPacket) = failedPackets;
                    MP_TX_MSDU_STATUS(currentPacket) = NDIS_STATUS_RESOURCES;                    

                    // This packet would get completed remove the extra MAC ref
                    HW_REMOVE_MAC_CONTEXT_SEND_REF(MacContext, 1);
                    failedPackets = currentPacket;
                    failedCount++;
                }
            }

            currentPacket = nextPacket;
        }

        HW_TX_RELEASE_QUEUE_LOCK(MacContext->Hw, currentQueue, dispatchLevel);
        
        //
        // Process all the packets that we have been queued
        //
        HwProcessReservedTxPackets(MacContext->Hw, 
            currentQueue,
            firstMsduIndex,
            numMsduReserved,
            SendFlags
            );
            
    } while (FALSE);


    if (failedPackets != NULL)
    {
        //
        // Complete the failed packets
        //
        HwSendCompletePackets(MacContext->Hw, 
            PacketList, 
            failedCount, 
            (NDIS_TEST_SEND_AT_DISPATCH_LEVEL(SendFlags) ? NDIS_SEND_COMPLETE_FLAGS_DISPATCH_LEVEL : 0)
            );
    }
    
    //
    // Remove the active send ref
    //
    HW_DECREMENT_ACTIVE_SEND_REF(MacContext->Hw);
}

VOID
HwHandleSendCompleteInterrupt(
    _In_  PHW                     Hw
    )
{
    // Increment the adapter send ref count. This blocks resets from proceeding
    HW_INCREMENT_ACTIVE_SEND_REF(Hw);

    if (HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_SEND_FLAGS))
    {
        //
        // While a context switch or a reset or pause is in progress, we dont want 
        // to do sends to be processed
        //
        HW_DECREMENT_ACTIVE_SEND_REF(Hw);    
        return;
    }

    // Need to check the default data and internal send queue for completion
    HwCheckSendQueueForCompletion(Hw, &Hw->TxInfo.TxQueue[HW11_DEFAULT_QUEUE]);
    HwCheckSendQueueForCompletion(Hw, &Hw->TxInfo.TxQueue[HW11_INTERNAL_SEND_QUEUE]);

    HW_DECREMENT_ACTIVE_SEND_REF(Hw);
}
