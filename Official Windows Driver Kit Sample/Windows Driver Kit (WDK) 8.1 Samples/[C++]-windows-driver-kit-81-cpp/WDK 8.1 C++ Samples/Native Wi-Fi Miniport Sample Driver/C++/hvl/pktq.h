#pragma once

typedef struct _PKT_QUEUE
{
    PQUEUE_ENTRY        Head;
    PQUEUE_ENTRY        Tail;
    ULONG               Count;
} PKT_QUEUE, *PPKT_QUEUE;


#define VERIFY_QUEUE(pPktQueue)           \
    ASSERT(!(pPktQueue->Head == NULL && pPktQueue->Count != 0));   \
    ASSERT(!(pPktQueue->Head != NULL && pPktQueue->Count == 0));   \
    ASSERT(!(pPktQueue->Head == NULL && pPktQueue->Tail != NULL)); \
    ASSERT(!(pPktQueue->Head != NULL && pPktQueue->Tail == NULL));

//
// If Next is no longer the first link in the packet list, change these macros
//
#define GET_QUEUE_ENTRY_FROM_PKT(_PKT)      \
    (PQUEUE_ENTRY)(&_PKT->QueueEntry)
#define GET_PKT_FROM_QUEUE_ENTRY(_QueueEntry)      \
    (PMP_TX_MSDU)(CONTAINING_RECORD(_QueueEntry, MP_TX_MSDU, QueueEntry))


__inline
BOOLEAN
PktQueueIsEmpty(
    PPKT_QUEUE pPktQueue
    )
{
    VERIFY_QUEUE(pPktQueue);
    return (pPktQueue->Head?FALSE:TRUE);
}

__inline
ULONG
PktQueueDepth(
    PPKT_QUEUE    pPktQueue
    )
{
    VERIFY_QUEUE(pPktQueue);
    return pPktQueue->Count;
}


__inline
VOID
InitPktQueue(
    PPKT_QUEUE       pPktQueue
    )
{
    NdisZeroMemory(pPktQueue, sizeof(PKT_QUEUE));
}

__inline
VOID
DeInitPktQueue(
    PPKT_QUEUE       pPktQueue
    )
{
    //
    // For debug versions of the driver, assert if a leak is detected.
    //
    if (!PktQueueIsEmpty(pPktQueue))
    {
        MPASSERTMSG("Attempt to deinitialize a NBL queue that is not empty\n", FALSE);
    }
}

__inline
VOID
QueuePkt(
    PPKT_QUEUE       pPktQueue,
    PMP_TX_MSDU      pPkt
    )
{
    PQUEUE_ENTRY pQueueEntry;
    VERIFY_QUEUE(pPktQueue);

    pQueueEntry = GET_QUEUE_ENTRY_FROM_PKT(pPkt);
    pQueueEntry->Next = NULL;
    
    //
    // If queue is empty, the head is NULL. New head should be this entry
    //
    if (PktQueueIsEmpty(pPktQueue) || (NULL == pPktQueue->Tail))
    {
        pPktQueue->Head = pQueueEntry;
    }
    else
    {
        //
        // If not empty, then there must be a tail. Add the NBL to next of Tail
        //
        pPktQueue->Tail->Next = pQueueEntry;
    }
    
    //
    // Make the NBL the new tail
    //
    pPktQueue->Tail = pQueueEntry;
    pPktQueue->Count++;
}

__inline
VOID
QueuePktList(
    PPKT_QUEUE       pPktQueue,
    ULONG            ulNumPkts,
    PMP_TX_MSDU      pPktList
    )
{
    PMP_TX_MSDU   currentPacket = NULL;

    UNREFERENCED_PARAMETER(ulNumPkts);
    VERIFY_QUEUE(pPktQueue);

    currentPacket = pPktList;
    if (pPktQueue->Head == NULL)
    {
        //
        // If queue is empty, the head is NULL. New head should be the start of this list
        //
        pPktQueue->Head = GET_QUEUE_ENTRY_FROM_PKT(currentPacket);
        pPktQueue->Tail = GET_QUEUE_ENTRY_FROM_PKT(currentPacket);
        pPktQueue->Count = 1;

        // Below we would add the next packet onwards
        currentPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket);
    }

    // Add the remaining packets to the tail of the queue
    while (currentPacket != NULL)
    {
        pPktQueue->Tail->Next = GET_QUEUE_ENTRY_FROM_PKT(currentPacket);
        pPktQueue->Tail = GET_QUEUE_ENTRY_FROM_PKT(currentPacket);
        
        pPktQueue->Count++;
        currentPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket);        
    }

    pPktQueue->Tail->Next = NULL;
}


/**
 * Removes a packet from the head of the Queue. Call to make sure that PktQueue is
 * not empty before attempting to dequeue a packet.
 * 
 * \param pPktQueue         The Queue to insert the packet in
 * \return The packet at the head.
 */
__inline
PMP_TX_MSDU  
DeQueuePkt(
    _In_  PPKT_QUEUE   pPktQueue
    )
{
    PQUEUE_ENTRY pNext, pHead;
    VERIFY_QUEUE(pPktQueue);
    
    MPASSERTMSG("Attempt to DeQueue from an Empty Packet Queue!\n", pPktQueue->Head);
    
    pHead = pPktQueue->Head;
    pNext = pPktQueue->Head->Next;
    pPktQueue->Head = pNext;
    if (pNext == NULL)
    {
        //
        // The queue is now emty as a result of dequeue.
        // Reset some variables
        //
        pPktQueue->Tail = NULL;
    }
    
    pPktQueue->Count--;

    //
    // We want to return only One NBL to the caller. Since NBL are chained
    // together by the Next Pointer, we should set it to NULL.
    //
    pHead->Next = NULL;

    return GET_PKT_FROM_QUEUE_ENTRY(pHead);
}


/**
    Dequeue all the packets from the packet queue
    */
__inline
PMP_TX_MSDU  
DeQueuePktList(
    _In_  PPKT_QUEUE   pPktQueue
    )
{
    PQUEUE_ENTRY        currentEntry;
    PMP_TX_MSDU         pktList = NULL, prevPacket = NULL, currentPacket = NULL;
    
    VERIFY_QUEUE(pPktQueue);
    
    MPASSERTMSG("Attempt to DeQueue from an Empty Packet Queue!\n", pPktQueue->Head);

    currentEntry = pPktQueue->Head;
    
    while (currentEntry != NULL)
    {
        currentPacket = GET_PKT_FROM_QUEUE_ENTRY(currentEntry);

        if (pktList == NULL)
            pktList = currentPacket;
        else
            MP_TX_MSDU_NEXT_MSDU(prevPacket) = currentPacket;
            
        prevPacket = currentPacket;

        currentEntry = currentEntry->Next;        
    }

    if (NULL == currentPacket)
    {
        MPASSERT(currentPacket != NULL);
        return NULL;
    }

    MP_TX_MSDU_NEXT_MSDU(currentPacket) = NULL;
    
    //
    // The queue is now emty as a result of dequeue.
    // Reset some variables
    //
    pPktQueue->Head = NULL;
    pPktQueue->Tail = NULL;
    pPktQueue->Count = 0;

    return pktList;
}

