/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_data_defs.h

Abstract:
    Contains hw layer defines for data traffic
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

#define HW_MIC_LENGTH           8

// Do not change this mapping
#define HW_TX_RATE_TABLE_SIZE   HAL_TX_RATE_TABLE_SIZE
#if HW_TX_RATE_TABLE_SIZE != HAL_TX_RATE_TABLE_SIZE
#error Must HW_TX_RATE_TABLE_SIZE == HAL_TX_RATE_TABLE_SIZE
#endif


#define HW_TX_MSDU_MPDU_AT(_Msdu, _i)   ((_Msdu)->MpduList[_i])
#define HW_TX_MSDU_MPDU_COUNT(_Msdu)    ((_Msdu)->MpduCount)

#define HW_TX_MSDU_IS_INTERNAL(_Msdu)   MP_TX_MSDU_IS_INTERNAL((_Msdu)->MpMsdu)

// Hardware structures to hold MSDU related data for sending
typedef struct _HW_TX_MSDU 
{
    /** Use for queuing the packets in internal structures */
    QUEUE_ENTRY                 QueueEntry;

    /** Corresponding MP_TX_MSDU */
    PMP_TX_MSDU                 MpMsdu;

    /** HW structure pointer */
    PHW                         Hw;

    /** Index of this entry in the descriptor array */
    ULONG                       Index;

    /** Software queue type that this MSDU is held in */
    UCHAR                       QueueID;

    /** Pointer to the peer node. This enables us to maintain per-peer statistics */
    PHW_PEER_NODE               PeerNode;

    PHW_TX_MPDU                 MpduList[MP_TX_FRAGMENTS_MAX_COUNT];
    _Field_range_(0, MP_TX_FRAGMENTS_MAX_COUNT-1) USHORT                      MpduCount;

    /** Flags for maintaining state */
    ULONG                       Flags;

    /** Time stamp when this MSDU */
    LARGE_INTEGER               SnapshotTime;                   
    
    /*
     * Preallocated Scatter Gather resource for this Tx MSDU.
     * \warning Do not use this field to access the SG Elements. Use SGElementList instead.
     */
    PUCHAR                      ScatterGatherList;

    /** Count of SG requests that have been submitted to OS for this TX_MSDU */
    USHORT                      TotalSGRequested;
    
    /*
     * The number of SGElement List in the array. If scatter gather is
     * successfully completed, this number is also the total number
     * of NetBuffers (MPDU) in this NetBufferList (MSDU)
     */
    USHORT                      SGElementListCount;
    
    /** The array of SGList corresponding to each NetBuffer in the Tx MSDU */
    PVOID                       SGElementList[MP_TX_FRAGMENTS_MAX_COUNT];

    /** The number of Scatter gather operations outstanding on this MSDU */
    UCHAR                       OutstandingSGAllocationCount;
    
    /** This buffer is used for different purposes for different queues. For default/data queues,
     * this is used as a coalesce buffer. For beacons, this is used as a beacon data
     * queue. Currently we only track this on the HW_TX_MSDU
     */
	PUCHAR						BufferVa;
	NDIS_PHYSICAL_ADDRESS		BufferPa;

    /** The rates to use for sending. A 0 in the table means ignored rate */
    USHORT                      TxRateTable[HW_TX_RATE_TABLE_SIZE];

    BOOLEAN                     WaitForSendToComplete:1;
    BOOLEAN                     FailedDuringSend:1;
    BOOLEAN                     TxSucceeded:1;
    BOOLEAN                     MulticastDestination:1;
    BOOLEAN                     ScatterGatherRequested:1;
    BOOLEAN                     UseShortPreamble:1;
    BOOLEAN                     RTSEnabled:1;
    BOOLEAN                     CTSToSelfEnabled:1;
    BOOLEAN                     SendEncrypted:1;

    ULONG                       PhyId;
    ULONG                       TotalMSDULength;
    
    ULONG                       TotalDescUsed;
    ULONG                       SucceedMPDUCount;
    PHW_KEY_ENTRY               Key;

    HAL_TX_DESC_PS_BIT_ENUM     PsBitSetting;
        
    HAL_SEND_ITEM               SendItem;

} HW_TX_MSDU, *PHW_TX_MSDU;

// Hardware
typedef struct _HW_TX_MPDU 
{
    /** The MP_MPDU for this HW_MPDU. This is used to get to the NET_BUFFER, etc
     * for the send
     */
    PMP_TX_MPDU                 MpMpdu;

    /** Encryption related data */
    UCHAR                       RetreatedSize;
    ULONG                       LastMdlByteCount;
    BOOLEAN                     MICMdlAdded;
    PMDL                        MICMdl;    
    PMDL                        LastMdl;
    UCHAR                       MICData[HW_MIC_LENGTH];

} HW_TX_MPDU, *PHW_TX_MPDU;

typedef struct _HW_RX_MSDU
{
    /** When receiving/returning multiple this is the next pointer */
    PHW_RX_MSDU                 Next;

    PHW_RX_MPDU                 MpduList[MP_RX_FRAGMENTS_MAX_COUNT];
    _Field_range_(0, MP_RX_FRAGMENTS_MAX_COUNT-1) USHORT                      MpduCount;  

    /** For reassembling fragments */
    USHORT                      ReassemblyLinePosition;
    USHORT                      SequenceNumber;
    DOT11_MAC_ADDRESS           PeerAddress;
    ULONGLONG                   ExpirationTime;

    /** Total length of the MSDU packet. The dot11 header
     * from only the first MPDU is included in there.
     */
    ULONG                       DataLength;
        
    /** The pointer to the corresponding MP_RX_MSDU */
    PMP_RX_MSDU                 MpMsdu;
    /** Preallocated MpMsdu structure */
    MP_RX_MSDU                  PrivateMpMsdu;
    
} HW_RX_MSDU, *PHW_RX_MSDU;

typedef struct _HW_RX_MPDU
{
    /** Used to chain the RX_MPDUs into a list */
    LIST_ENTRY                  ListEntry;

    /** Actual data pointer. This may be offset from the BufferVa depending on
     * encryption settings, fragmentation, etc
     */
    PUCHAR                      DataStart;
        
    /** Length of data starting from DataStart */
    ULONG                       DataLength;

    /** Complete data buffer that is passed to the hardware */
    PVOID                       BufferVa;
    NDIS_PHYSICAL_ADDRESS       BufferPa;

    HAL_RX_DESC_STATUS          DescStatus;
    HAL_RX_ITERATOR             DescIter;
    
    /** The receive context for this MPDU */

    //
    // When the RawPacket bit is set, the integrity of the packet is not
    // guaranteed
    // 
    BOOLEAN                     RawPacket:1;
    BOOLEAN                     Encrypted:1;
    BOOLEAN                     MulticastDestination:1; // Multicast or Broadcast
    UCHAR                       LinkQuality;

    /** This is the rate in IEEE format (units of 500 kilobits per second (Kbps)) */
    USHORT                      Rate;

    ULONG                       PhyId;
    ULONG                       Channel;

    LONG                        RSSI;
    
    ULONGLONG                   Timestamp;
    ULONGLONG                   FrameNumber;

    /** MAC context of the node that received this. If this is NULL, the MPDU
     * needs to be indicated to all the MACs
     */
    PHW_MAC_CONTEXT             MacContext;

    /** If found, the key that was used to encrypt this packet */
    PHW_KEY_ENTRY               Key;

    /** Pointer to the corresponding MP_RX_MPDU */
    PMP_RX_MPDU                 MpMpdu;
    /** Preallocated MpMpdu structure. Should not be accessed through MpMpdu pointer */
    MP_RX_MPDU                  PrivateMpMpdu;

    /** MiniportSharedMemoryAllocateComplete is called with MiniportAdapterContext.
     * However we need  the HW context. To simplify this, we save the HW context
     * here
     */
    PHW                         Hw;
} HW_RX_MPDU, *PHW_RX_MPDU;


