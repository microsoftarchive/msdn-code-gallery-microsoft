/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    TxPacketQ.h

Abstract:
    Packet queuing macros    
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

typedef struct _MP_PACKET_QUEUE
{
    PQUEUE_ENTRY        Head;
    PQUEUE_ENTRY        Tail;
    ULONG               Count;
} MP_PACKET_QUEUE, *PMP_PACKET_QUEUE;


#define MP_VERIFY_QUEUE(PacketQueue)           \
    MPASSERT(!(PacketQueue->Head == NULL && PacketQueue->Count != 0));   \
    MPASSERT(!(PacketQueue->Head != NULL && PacketQueue->Count == 0));   \
    MPASSERT(!(PacketQueue->Head == NULL && PacketQueue->Tail != NULL)); \
    MPASSERT(!(PacketQueue->Head != NULL && PacketQueue->Tail == NULL));

#define QUEUE_ENTRY_FROM_MP_MSDU(_Msdu)      \
    (PQUEUE_ENTRY)(&_Msdu->QueueEntry)
#define MP_MSDU_FROM_QUEUE_ENTRY(_QueueEntry)      \
    (PMP_TX_MSDU)(CONTAINING_RECORD(_QueueEntry, MP_TX_MSDU, QueueEntry))

#define QUEUE_ENTRY_FROM_HW_MSDU(_Msdu)      \
    (PQUEUE_ENTRY)(&_Msdu->QueueEntry)
#define HW_MSDU_FROM_QUEUE_ENTRY(_QueueEntry)      \
    (PHW_TX_MSDU)(CONTAINING_RECORD(_QueueEntry, HW_TX_MSDU, QueueEntry))


__inline BOOLEAN
MpPacketQueueIsEmpty(
    PMP_PACKET_QUEUE PacketQueue
    )
{
    MP_VERIFY_QUEUE(PacketQueue);
    return (PacketQueue->Head == NULL) ? TRUE : FALSE;
}


__inline ULONG
MpPacketQueueDepth(
    PMP_PACKET_QUEUE    PacketQueue
    )
{
    MP_VERIFY_QUEUE(PacketQueue);
    return PacketQueue->Count;
}


__inline VOID
MpInitPacketQueue(
    PMP_PACKET_QUEUE       PacketQueue
    )
{
    NdisZeroMemory(PacketQueue, sizeof(MP_PACKET_QUEUE));
}

__inline VOID
MpDeinitPacketQueue(
    PMP_PACKET_QUEUE       PacketQueue
    )
{
    //
    // For debug versions of the driver, assert if a leak is detected.
    //
    if (!MpPacketQueueIsEmpty(PacketQueue))
    {
        MPASSERTMSG("Attempt to deinitialize a PACKET queue that is not empty\n", FALSE);
    }
}

__inline VOID
MpQueuePacket(
    PMP_PACKET_QUEUE       PacketQueue,
    PQUEUE_ENTRY           Packet
    )
{
    PQUEUE_ENTRY pQueueEntry;
    MP_VERIFY_QUEUE(PacketQueue);

    pQueueEntry = Packet;
    pQueueEntry->Next = NULL;
    
    //
    // If queue is empty, the head is NULL. New head should be this entry
    //
    if (MpPacketQueueIsEmpty(PacketQueue) || (NULL == PacketQueue->Tail))
    {
        PacketQueue->Head = pQueueEntry;
    }
    else
    {
        //
        // If not empty, then there must be a tail. Add the PACKET to next of Tail
        //
        PacketQueue->Tail->Next = pQueueEntry;
    }
    
    //
    // Make the PACKET the new tail
    //
    PacketQueue->Tail = pQueueEntry;
    PacketQueue->Count++;
}


__inline VOID
MpQueuePacketPriority(
    PMP_PACKET_QUEUE       PacketQueue,
    PQUEUE_ENTRY           Packet
    )
{
    PQUEUE_ENTRY pQueueEntry;
    MP_VERIFY_QUEUE(PacketQueue);

    pQueueEntry = Packet;
    pQueueEntry->Next = PacketQueue->Head;
    PacketQueue->Head = pQueueEntry;
    if (PacketQueue->Tail == NULL)
        PacketQueue->Tail = pQueueEntry;
    PacketQueue->Count++;
}


/**
 * Removes a PACKET from the head of the Queue. Call to make sure that PacketQueue is
 * not empty before attempting to dequeue a PACKET.
 * 
 * \param PacketQueue         The Queue to insert the PACKET in
 * \return The PACKET at the head.
 */
__inline PQUEUE_ENTRY  
MpDequeuePacket(
    _In_  PMP_PACKET_QUEUE   PacketQueue
    )
{
    PQUEUE_ENTRY pNext, pHead;
    MP_VERIFY_QUEUE(PacketQueue);
    
    MPASSERTMSG("Attempt to DeQueue from an Empty Packet Queue!\n", PacketQueue->Head);
    
    pHead = PacketQueue->Head;
    pNext = PacketQueue->Head->Next;
    PacketQueue->Head = pNext;
    if (pNext == NULL)
    {
        //
        // The queue is now emty as a result of dequeue.
        // Reset some variables
        //
        PacketQueue->Tail = NULL;
    }
    
    PacketQueue->Count--;

    //
    // We want to return only One PACKET to the caller. Since PACKET are chained
    // together by the Next Pointer, we should set it to NULL.
    //
    pHead->Next = NULL;

    return pHead;
}
