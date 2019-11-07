/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_send.h

Abstract:
    Contains defines used for send functionality 
    in the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

/*
 * The maximum number of Dot11 fragments we expect to practically see in a NBL 
 * Worst case, this value can go up to 9 and theoretically go up to 16
 */
#define HW_MAX_NUM_OF_FRAGMENTS                 MIN(MP_TX_FRAGMENTS_MAX_COUNT, 4)

#define HW_TX_COALESCE_THRESHOLD                8

/** Number of buffers for beacons */
#define HW_BEACON_QUEUE_BUFFER_COUNT            1
/** Number of buffers for Probe response, etc */
#define HW_INTERNAL_SEND_QUEUE_BUFFER_COUNT     16
#define HW_UNUSED_SEND_QUEUE_BUFFER_COUNT       2


#define sAckCtsLng		112		// bits in ACK and CTS frames
#define sCrcLng			4		// octets for crc32 (FCS, ICV)
#define aSifsTime		10
#define sMacHdrLng		24		// octets in data header, no WEP


/** We can have per queue-lock, but we currently dont use them */
#define HW_TX_ACQUIRE_QUEUE_LOCK(_Hw, _HwQueue, _Dispatch)  \
    HW_ACQUIRE_HARDWARE_LOCK(_Hw, _Dispatch)
#define HW_TX_RELEASE_QUEUE_LOCK(_Hw, _HwQueue, _Dispatch)  \
    HW_RELEASE_HARDWARE_LOCK(_Hw, _Dispatch)

/** Since we are not using per queue-lock, this semantic is used
 * when we are holding a queue lock & want to acquire the HW lock
 */
#define HW_ACQUIRE_HARDWARE_LOCK_WITH_QUEUE_LOCK_HELD(_Hw)
#define HW_RELEASE_HARDWARE_LOCK_WITH_QUEUE_LOCK_HELD(_Hw)

typedef enum _HW_TX_CIPHER_SETTING{
    HW_TX_CAN_ENCRYPT,
    HW_TX_ENCRYPT_IF_KEY_MAPPING_KEY_AVAILABLE,
    HW_TX_NEVER_ENCRYPT
} HW_TX_CIPHER_SETTING, *PHW_TX_CIPHER_SETTING;

#define HW_INCREMENT_AVAILABLE_TX_MSDU(_HwQueue)        \
    NdisInterlockedIncrement((PLONG)&((_HwQueue)->NumMSDUAvailable))
#define HW_DECREMENT_AVAILABLE_TX_MSDU(_HwQueue)        \
    NdisInterlockedDecrement((PLONG)&((_HwQueue)->NumMSDUAvailable))

#define HW_INCREMENT_PENDING_TX_MSDU_SG_OP(_Txd)    \
    ((UCHAR)NdisInterlockedIncrement((PLONG)&((_Txd)->OutstandingSGAllocationCount)))
#define HW_DECREMENT_PENDING_TX_MSDU_SG_OP(_Txd)    \
    ((UCHAR)NdisInterlockedDecrement((PLONG)&((_Txd)->OutstandingSGAllocationCount)))


#define HW_INCREMENT_HAL_PENDING_TX(_HwQueue)       \
    NdisInterlockedIncrement((PLONG)&((_HwQueue)->NumPending))

#define HW_DECREMENT_HAL_PENDING_TX(_HwQueue)       \
    NdisInterlockedDecrement((PLONG)&((_HwQueue)->NumPending))

//
// Various flags used for the synchronization of the TX_MSDU list
//
// DO NOT ADD ANY OTHER BITS TO THESE FLAGS (ie. do not add any other flags)
/** The Tx MSDU is free and can used to transmit a packet */
#define     HW_TX_MSDU_IS_FREE                  0x00000000
/** This Tx MSDU is reserved for a packet and should not be used for any other packet */
#define     HW_TX_MSDU_IS_RESERVED              0x00000001
/** The Tx MSDU is setup for send */
#define     HW_TX_MSDU_IS_READY_FOR_SEND        0x00000002
/** This Tx MSDU has a thread owner that will transmit it when the turn comes */
#define     HW_TX_MSDU_HAS_SENDER               0x00000004
/** This Tx Desc has been passed to the hardware for transmission */
#define     HW_TX_MSDU_IS_IN_SEND               0x00000008
// DO NOT ADD ANY OTHER BITS TO THESE FLAGS (ie. do not add any other flags)

/**
 * This Macro tries to obtain ownership of sending a tx MSDU for the current thread.
 * If the ownership is successfullly obtained, the macro returns true. Else a
 * false value is returned. This macro is internal to the send path only.
 * 
 * FOR THIS TO WORK, DO NOT ADD ANY OTHER BITS TO THE FLAGS
 */
#define HW_ACQUIRE_TX_MSDU_SEND_OWNERSHIP(_TxDesc)                          \
    (InterlockedCompareExchange(											\
                (PLONG)&(_TxDesc->Flags),									        \
                (HW_TX_MSDU_IS_RESERVED | HW_TX_MSDU_IS_READY_FOR_SEND | HW_TX_MSDU_HAS_SENDER),    \
                (HW_TX_MSDU_IS_RESERVED | HW_TX_MSDU_IS_READY_FOR_SEND)	    \
                ) == (HW_TX_MSDU_IS_RESERVED | HW_TX_MSDU_IS_READY_FOR_SEND))

#define HW_RELEASE_TX_MSDU_SEND_OWNERSHIP(_TxDesc)                          \
    InterlockedExchange((PLONG)&(_TxDesc->Flags), (HW_TX_MSDU_IS_RESERVED | HW_TX_MSDU_IS_READY_FOR_SEND))

#define HW_TX_MSDU_SET_FLAG(_Txd, _Flag)    MP_INTERLOCKED_SET_FLAG(&(_Txd->Flags), _Flag)
#define HW_TX_MSDU_TEST_FLAG(_Txd, _Flag)   MP_TEST_FLAG((_Txd->Flags), _Flag)
#define HW_TX_MSDU_CLEAR_FLAG(_Txd, _Flag)  MP_INTERLOCKED_CLEAR_FLAG(&(_Txd->Flags), _Flag)


/**
 * Initializes a newly reserved HW_TX_MPDU
 */
#define HW_RESERVE_TX_MSDU(_Msdu, _Packet)                  \
{                                                           \
    MPASSERT(_Msdu->Flags == 0);                            \
    HW_TX_MSDU_SET_FLAG(_Msdu, HW_TX_MSDU_IS_RESERVED);     \
    _Msdu->MpMsdu = _Packet;                                \
    _Msdu->SGElementListCount = 0;                          \
    _Msdu->WaitForSendToComplete = TRUE;                    \
    _Msdu->FailedDuringSend = FALSE;                        \
    _Msdu->TxSucceeded = FALSE;                             \
    _Msdu->ScatterGatherRequested = FALSE;                  \
}


/**
 * Release a previously reserved HW_TX_MPDU
 */
#define HW_RELEASE_TX_MSDU(_Msdu)                           \
{                                                           \
    _Msdu->MpduCount = 0;                                   \
    _Msdu->Flags = 0;                                       \
    _Msdu->MpMsdu = NULL;                                   \
    _Msdu->PeerNode = NULL;                                 \
    _Msdu->PhyId = DOT11_PHY_ID_ANY;                        \
    _Msdu->SGElementListCount = 0;                          \
    _Msdu->Key = NULL;                                      \
}

#define HW_TIMESTAMP_TX_MSDU(_Msdu, _TxQueue)               \
{                                                           \
    NdisGetCurrentSystemTime(&_Msdu->SnapshotTime);         \
}

#define HW_CHECK_TX_MSDU_TIME(_Msdu, _BreakTime)            \
{                                                           \
    LARGE_INTEGER               _currentTime;               \
    NdisGetCurrentSystemTime(&_currentTime);   \
    DbgPrint("# %4d %s %d\n", _Msdu->TotalMSDULength, (_Msdu->TxSucceeded ? "OK " : "ERR"), \
        (_currentTime.QuadPart - _Msdu->SnapshotTime.QuadPart));                \
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
    );



/**
 * This function is called when Scatter Gather operation for a TX_MSDU has
 * been completed, either successfully or with failures. This function
 * determines whether SG operations were successful or not and then calls
 * the hardware with appropriate status.
 * 
 * \param pTxd              The Tx MSDU which was being scatter gathered
 * \param DispatchLevel     TRUE if IRQL is at dispatch level
 */
VOID
HwSGComplete(
    _In_  PHW_TX_MSDU         Msdu,
    _In_  BOOLEAN             DispatchLevel
    );

__inline BOOLEAN
HwCanTransmit(
    _In_  PHW_TX_QUEUE            TxQueue
    );

NDIS_STATUS
HwPrepareTxMSDUForSend(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu
    );


// Lock held
__inline NDIS_STATUS
HwReserveTxResources(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PMP_TX_MSDU             Packet
    );


VOID
HwProcessReservedTxPackets(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  LONG                    StartIndex,
    _In_  ULONG                   NumTxReady,
    _In_  ULONG                   SendFlags
    );


__inline USHORT
HwComputeTxTime(
    _In_  PHW_TX_MSDU             Msdu,
    _In_  DOT11_FRAME_TYPE        FrameType,
    _In_  ULONG                   FragmentLength,
    _In_  BOOLEAN                 LastFragment
    );



__inline HW_TX_CIPHER_SETTING
HwDetermineCipherSettings(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_PEER_NODE           PeerNode,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  PDOT11_MAC_HEADER       FragmentHeader
    );


PHW_KEY_ENTRY
HwFindEncryptionKey(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_PEER_NODE           PeerNode,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  PDOT11_MAC_HEADER       FragmentHeader,
    _In_  HW_TX_CIPHER_SETTING    CipherSettings
    );

NDIS_STATUS
HwSetupTxCipher(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  PHW_TX_MPDU             Mpdu,
    _In_  PHW_KEY_ENTRY           Key
    );

VOID
HwSubmitReadyMSDUs(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  BOOLEAN                 DispatchLevel
    );


ULONG
HwGetTxFreeDescNum(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue
    );


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
    );




ULONG
HwCountTxResourceNeeded(
    _In_  PHW_TX_MSDU         Msdu,
    _Out_ BOOLEAN             *UseCoalesce
    );



NDIS_STATUS 
HwTransmitMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  USHORT                  NumTxDescNeeded
    );



NDIS_STATUS
HwCopyNBLToBuffer(
    _In_  PNET_BUFFER_LIST    NetBufferList,
    _Out_writes_bytes_(MAX_TX_RX_PACKET_SIZE) PUCHAR              pDest,
    _Out_ PULONG              BytesCopied
    );


NDIS_STATUS
HwTransmitMSDUCoalesce(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  USHORT                  NumTxDescNeeded
    );

NDIS_STATUS
HwTransmitBeacon(
    _In_  PHW                     Hw,
    _In_  PHW_TX_MSDU             Msdu
    );

BOOLEAN
HwTxMSDUIsComplete(
    _In_  PHW                     Hw,
    _In_  PHW_TX_MSDU             Msdu
    );

VOID
HwFreeTxResources(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu
    );

VOID
HwCompleteTxMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  PHW_TX_MSDU             Msdu,
    _In_  BOOLEAN                 Success
    );


VOID
HwCheckSendQueueForCompletion(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue
    );


VOID
HwReinitializeSendQueue(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  BOOLEAN                 DispatchLevel
    );

VOID 
HwSendCompletePackets(
    _In_  PHW                     Hw,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   NumberOfPackets,
    _In_  ULONG                   SendCompleteFlags
    );

_IRQL_requires_(DISPATCH_LEVEL)
VOID
HwFlushQueuedTxMSDUs(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  BOOLEAN                 DispatchLevel
    );

_IRQL_requires_(DISPATCH_LEVEL)
VOID
HwFlushReservedTxMSDUs(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  BOOLEAN                 DispatchLevel
    );


VOID
HwFlushSendQueue(
    _In_  PHW                     Hw,
    _In_  PHW_TX_QUEUE            TxQueue,
    _In_  BOOLEAN                 DispatchLevel
    );

VOID
HwFlushSendEngine(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 DispatchLevel
    );

VOID
HwResetSendEngine(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 DispatchLevel
    );

BOOLEAN
HwCheckForSendHang(
    _In_  PHW                     Hw
    );


PMP_TX_MSDU
HwAllocatePrivatePacket(
    _In_  PHW                     Hw,
    _In_  USHORT                  PacketSize
    );

VOID
HwFreePrivatePacket(
    _In_  PHW                     Hw,
    _In_  PMP_TX_MSDU             Packet
    );    

VOID
HwSendPrivatePackets(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendFlags
    );

VOID 
HwSendCompletePrivatePackets(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   NumberOfPackets,
    _In_  ULONG                   SendCompleteFlags
    );

VOID
HwHandleSendCompleteInterrupt(
    _In_  PHW                     Hw
    );


