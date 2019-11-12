/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    vnic_send.c

Abstract:
    Implements the send processing for the VNIC
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"


#if DOT11_TRACE_ENABLED
#include "vnic_send.tmh"
#endif

__inline
VOID
VNicIncOutstandingSends(    
    _In_  PVNIC                   pVNic,
    _In_  ULONG                   ulNumPkts
    )
{
    ASSERT(VNicIsLocked(pVNic));
    ASSERT(pVNic->lOutstandingSends >= 0);
    
    pVNic->lOutstandingSends += ulNumPkts;
}

__inline
VOID
VNicDecOutstandingSends(    
    _In_  PVNIC                   pVNic,
    _In_  ULONG                   ulNumPkts
    )
{
    ASSERT(VNicIsLocked(pVNic));
    pVNic->lOutstandingSends -= ulNumPkts;
    ASSERT(pVNic->lOutstandingSends >= 0);
}

/*
    This function is called with the VNic locked
    */
VOID
VNicQueueSendRequests(
    _In_  PVNIC                   pVNic,
    _In_  ULONG                   ulNumPkts,
    _In_  PMP_TX_MSDU             PacketList
    )
{
    ASSERT(VNicIsLocked(pVNic));

    MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Queueing %d send packets \n", VNIC_PORT_NO, ulNumPkts));
    
    QueuePktList(&pVNic->TxQueue, ulNumPkts, PacketList);

    return;
}

/*
    NOTE
    This packet gives up the VNIC lock before calling into the hardware. The VNIC state might get
    changed during the course of the call
    */
VOID
VNicSendPktsToHw(    
    _In_ PVNIC                   pVNic,
    _In_ ULONG                   ulNumPkts,
    _In_ PMP_TX_MSDU             PacketList,
    _In_ ULONG                   SendFlags
    )
{
    BOOLEAN fDispatchLevel = SendFlags & NDIS_SEND_FLAGS_DISPATCH_LEVEL ? TRUE : FALSE;
    PMP_TX_MSDU currentPacket = NULL;
    ULONG myCount = 0;

    currentPacket = PacketList;
    while (currentPacket != NULL)
    {
        myCount++;
        currentPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket);
    }
    MPASSERT(myCount == ulNumPkts);

    ASSERT(VNicIsLocked(pVNic));
    ASSERT(VNicIsActive(pVNic));
    
    MpTrace(COMP_HVL, DBG_LOUD, ("VNic(%d): Sending %d packets to the hardware \n", VNIC_PORT_NO, ulNumPkts));

    /*
        We can call the hardware for sending packets. We need to increment the context 
        switch ref count to avoid becoming inactive. The context switch ref count will be 
        decremented when we receive the send completionAlso we need to give up our lock 
        before calling the hardware. 
        */
    VNicIncCtxSRef(pVNic, ulNumPkts, REF_SEND_PKTS);

    // we also need to keep track of the sends outstanding to the hardware
    VNicIncOutstandingSends(pVNic, ulNumPkts);
    
    _Analysis_assume_lock_held_((& pVNic->Lock)->SpinLock);
    VNicUnlockAtDispatch(pVNic, fDispatchLevel);
    
    Hw11SendPackets(pVNic->pvHwContext, PacketList, SendFlags);

    VNicLockAtDispatch(pVNic, fDispatchLevel);
}

/*
    This function releases the lock during its processing
    */
VOID
VNicProcessQueuedPkts(
    _In_ PVNIC pVNic, 
    BOOLEAN fDispatchLevel
    )
{
    ULONG                   ulNumPkts = 0;
    PMP_TX_MSDU             PacketList = NULL;
    BOOLEAN fFailSends = FALSE;
    
    ASSERT(VNicIsLocked(pVNic));
    ASSERT(VNicIsActive(pVNic));
    ASSERT(!PktQueueIsEmpty(&pVNic->TxQueue));

    do
    {
        if (VNicIsInReset(pVNic))
        {
            fFailSends = TRUE;
            ulNumPkts = PktQueueDepth(&pVNic->TxQueue);
            PacketList = DeQueuePktList(&pVNic->TxQueue);
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d) is in reset. Failing %d sends. \n", VNIC_PORT_NO, ulNumPkts));
            break;
        }        

        if (Hw11CanTransmit(pVNic->pvHwContext))
        {
            ulNumPkts = PktQueueDepth(&pVNic->TxQueue);
            PacketList = DeQueuePktList(&pVNic->TxQueue);
            
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): Processing %d queued packets \n", VNIC_PORT_NO, ulNumPkts));

            VNicSendPktsToHw(pVNic, ulNumPkts, PacketList, 0);
        }
        else
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d): The hardware is not yet ready to accept packets\n", VNIC_PORT_NO));
        }
    } while (FALSE);

    if (fFailSends)
    {
        _Analysis_assume_lock_held_((& pVNic->Lock)->SpinLock);

        VNicUnlockAtDispatch(pVNic, fDispatchLevel);
        Port11SendCompletePackets(pVNic->pvPort, PacketList, 0);
        VNicLockAtDispatch(pVNic, fDispatchLevel);
    }    
    
    return;
}


VOID
VNic11SendPackets(
    _In_ PVNIC                   pVNic,
    _In_ PMP_TX_MSDU             PacketList,
    _In_ ULONG                   ulNumPkts,
    _In_ ULONG                   SendFlags
    )
{
    BOOLEAN fDispatchLevel = SendFlags & NDIS_SEND_FLAGS_DISPATCH_LEVEL ? TRUE : FALSE;
    BOOLEAN fFailSends = FALSE;
#if DBG
    PMP_TX_MSDU             currentPacket;
    ULONG                   myCount = 0;

    currentPacket = PacketList;
    while (currentPacket != NULL)
    {
        myCount++;
        currentPacket = MP_TX_MSDU_NEXT_MSDU(currentPacket);
    }

    MPASSERT(myCount == ulNumPkts);
#endif

    VNicLockAtDispatch(pVNic, fDispatchLevel);

    do
    {
        if (VNicIsInReset(pVNic))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNIC(%d) is in reset. Failing %d sends. \n", VNIC_PORT_NO, ulNumPkts));
            fFailSends = TRUE;
            break;
        }
        
        if (VNicIsActive(pVNic) && Hw11CanTransmit(pVNic->pvHwContext) && (0 == PktQueueDepth(&pVNic->TxQueue)))
        {        
            VNicSendPktsToHw(pVNic, ulNumPkts, PacketList, SendFlags);
        }
        else
        {
            /*
                We are either 
                a. not currently active or 
                b. the packets cannot be submitted to the hardware or
                c. there are packets pending in the send queue already
                
                Queue the send requests internally
                */
            VNicQueueSendRequests(pVNic, ulNumPkts, PacketList);
        }
    } while (FALSE);
    
    VNicUnlockAtDispatch(pVNic, fDispatchLevel);

    if (fFailSends)
    {
        Port11SendCompletePackets(pVNic->pvPort, PacketList, SendFlags);
    }
    
    return;
}

BOOLEAN
VNic11CanTransmit(
    _In_  PVNIC                   pVNic
    )
{
    UNREFERENCED_PARAMETER(pVNic);
    
    return TRUE;
}

VOID 
VNic11SendCompletePackets(
    _In_  PVNIC                   pVNic,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   ulNumPkts,
    _In_  ULONG                   SendCompleteFlags
    )
{
    BOOLEAN fDispatchLevel = SendCompleteFlags & NDIS_SEND_COMPLETE_FLAGS_DISPATCH_LEVEL ? TRUE : FALSE;

    MpTrace(COMP_HVL, DBG_LOUD, ("VNic(%d): Send completion called for %d packets \n", VNIC_PORT_NO, ulNumPkts));

    Port11SendCompletePackets(pVNic->pvPort, PacketList, SendCompleteFlags);

    VNicLockAtDispatch(pVNic, fDispatchLevel);
    
    /*
        Remove the context switch ref counts that we added for these packets. Context switches 
        can happen once it goes to zero
        */
    VNicDecCtxSRef(pVNic, ulNumPkts, REF_SEND_PKTS);

    // account for the outstanding sends that have now been completed
    VNicDecOutstandingSends(pVNic, ulNumPkts);
    
    // also handle any sends that had been queued
    if (!PktQueueIsEmpty(&pVNic->TxQueue))
    {
        VNicProcessQueuedPkts(pVNic, fDispatchLevel);
    }

    VNicUnlockAtDispatch(pVNic, fDispatchLevel);    
}

_IRQL_requires_(DISPATCH_LEVEL)
VOID
VNicCancelPendingSends(
    _In_  PVNIC                   pVNic
    )
{
    PMP_TX_MSDU   pendingPackets = NULL;

    ASSERT(VNicIsLocked(pVNic));
    
    do
    {
        if (!PktQueueIsEmpty(&pVNic->TxQueue))
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): Cancelling %d sends\n", VNIC_PORT_NO, PktQueueDepth(&pVNic->TxQueue)));
            
            pendingPackets = DeQueuePktList(&pVNic->TxQueue);

            _Analysis_assume_lock_held_(pVNic->Lock.SpinLock);

            // give up lock before calling into the port
            VNicUnlock(pVNic);            
            Port11SendCompletePackets(pVNic->pvPort, pendingPackets, 0);
            VNicLock(pVNic);
        }
        else
        {
            MpTrace(COMP_HVL, DBG_NORMAL, ("VNic(%d): No sends are pending. Nothing to cancel\n", VNIC_PORT_NO));
        }
    } while (FALSE);    
}
