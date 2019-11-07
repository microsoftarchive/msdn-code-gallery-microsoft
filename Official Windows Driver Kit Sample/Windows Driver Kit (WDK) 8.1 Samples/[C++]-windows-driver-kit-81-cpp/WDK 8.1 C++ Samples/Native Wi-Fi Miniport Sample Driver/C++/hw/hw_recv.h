/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_recv.h

Abstract:
    Contains defines used for receive functionality 
    in the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

/*
 * The number of Reassembly Rx MSDUs that are currently in use before
 * we start expiring old receive Rx MSDUs if they have exceeded the
 * RxLifeTime set by the OS.
 */ 
#define HW_REASSEMBLY_LINE_LOW_RESOURCES_THRESHOLD   30

/*
 * Countdown before which the Reassembly Cleanup algorithm to kick in. The unit
 * is the number of times receive interrupt handling routine has run and indicated
 * something up to the OS.
 */
#define HW_REASSEMBLY_CLEANUP_COUNTDOWN_VALUE         250


/*
 * When the number of Rx MSDU available to the miniport falls below this
 * value, the miniport starts indicating low resources with packets indicated
 * to port. At this point, the upper layer driver will copy the data
 * indicated and return it immediately so that the next received packets can
 * be indicated using this Rx MSDU. This helps avoid packet loss due to lack of
 * resources to indicate packets up in.
 */
#define HW_RX_MSDU_LOW_RESOURCE_THRESHOLD        4

#define HW_RX_MSDU_GROW_POOL_SIZE                1


#define HW_INCREMENT_AVAILABLE_RX_MPDU(_Hw)          NdisInterlockedIncrement((PLONG)&_Hw->RxInfo.NumMPDUAvailable)
#define HW_DECREMENT_AVAILABLE_RX_MPDU(_Hw)          NdisInterlockedDecrement((PLONG)&_Hw->RxInfo.NumMPDUAvailable)
#define HW_ADD_TO_AVAILABLE_RX_MPDU(_Hw, NumToAdd)   InterlockedExchangeAdd((PLONG)&_Hw->RxInfo.NumMPDUAvailable, NumToAdd);

#define HW_INCREMENT_UNUSED_RX_MPDU(_Hw)            NdisInterlockedIncrement((PLONG)&_Hw->RxInfo.NumMPDUUnused)
#define HW_DECREMENT_UNUSED_RX_MPDU(_Hw)            NdisInterlockedDecrement((PLONG)&_Hw->RxInfo.NumMPDUUnused)

#define HW_INCREMENT_TOTAL_RX_MPDU_ALLOCATED(_Hw)    NdisInterlockedIncrement((PLONG)&_Hw->RxInfo.NumMPDUAllocated)
#define HW_DECREMENT_TOTAL_RX_MPDU_ALLOCATED(_Hw)    NdisInterlockedDecrement((PLONG)&_Hw->RxInfo.NumMPDUAllocated)
#define HW_ADD_TO_TOTAL_RX_MPDU_ALLOCATED(_Hw, NumToAdd)      InterlockedExchangeAdd((PLONG)&_Hw->RxInfo.NumMPDUAllocated, NumToAdd);

/*
 * \internal
 * Matches an Rx MSDUs in Reassembly with the provided sequence number and MAC
 * address. If both the values match, the macro returns true, else returns
 * false
 */
#define HW_REASSEMBLY_RX_MSDU_MATCH(_ReassemblyRxMSDU, _SequenceNumber, _MacAddress)    \
    ((_ReassemblyRxMSDU->SequenceNumber == _SequenceNumber) &&                 \
      (MP_COMPARE_MAC_ADDRESS(                                              \
            _ReassemblyRxMSDU->PeerAddress,                                 \
            _MacAddress)))


/** Access the timestamp field of the MSDU */
#define HW_GET_RX_EXPIRATION_TIME(_Msdu)   _Msdu->ExpirationTime

/** Placed the time when MSDU will expire */
#define HW_SET_RX_EXPIRATION_TIME(_Msdu, _LifeTime)         \
{                                                                   \
    KeQueryTickCount((PLARGE_INTEGER)&(_Msdu->ExpirationTime));     \
    _Msdu->ExpirationTime += _LifeTime;          \
}


/**
 * Initializes a newly allocate HW_RX_MPDU
 */
#define HW_INITIALIZE_RX_MPDU(_Mpdu, _Hw)    \
{                                                           \
    NdisZeroMemory(_Mpdu, sizeof(HW_RX_MPDU));              \
                                                            \
    /* Set the appropriate pointers in the HW & MP MPDU */  \
    _Mpdu->MpMpdu = &_Mpdu->PrivateMpMpdu;                  \
    _Mpdu->Hw = _Hw;                                        \
    _Mpdu->MpMpdu->HwMpdu = _Mpdu;                          \
}

/**
 * Reinitialize a returned HW_RX_MPDU
 */
#define HW_REINITIALIZE_RX_MPDU(_Mpdu)    \
{                                                           \
    _Mpdu->DataLength = 0;                                  \
    _Mpdu->PhyId = DOT11_PHY_ID_ANY;                        \
    _Mpdu->Channel = 0;                                     \
    _Mpdu->RawPacket = FALSE;                               \
    _Mpdu->Encrypted = FALSE;                               \
    _Mpdu->MulticastDestination = FALSE;                    \
    _Mpdu->MacContext = NULL;                               \
    _Mpdu->Key = NULL;                                      \
}


/** We can have a special receive lock, but we currently dont use them */
#define HW_RX_ACQUIRE_LOCK(_Hw, _Dispatch)  \
    HW_ACQUIRE_HARDWARE_LOCK(_Hw, _Dispatch)
#define HW_RX_RELEASE_LOCK(_Hw, _Dispatch)  \
    HW_RELEASE_HARDWARE_LOCK(_Hw, _Dispatch)

// If this is a broadcast packet, the counters get updated on the default MAC
#define HW_MAC_CONTEXT_FOR_RX_STATISTICS(_Hw, _Mpdu)    \
    ((_Mpdu->MacContext != NULL) ? _Mpdu->MacContext : &_Hw->MacContext[0])


__inline PHW_RX_MSDU
HwAllocateRxMSDU(
    _In_  PHW                     Hw
    );

__inline VOID
HwFreeRxMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu
    );
    
BOOLEAN
HwCheckPhyParameters(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );

// Returns TRUE if this fragment is duplicate and should be dropped. FALSE otherwise
// Also if AddToTable is true, this entry is added to the duplicates table, else its not
BOOLEAN
HwMPDUIsDuplicate(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu,
    _In_  BOOLEAN                 IsGoodPacket
    );



// Called for Data and Management packets on 
BOOLEAN
HwCheckForDuplicateMPDU(
    _In_  PHW                     Hw,
    _In_reads_bytes_(sizeof(DOT11_MGMT_DATA_MAC_HEADER))  PUCHAR                  PacketBuffer,
    _In_  BOOLEAN                 IsGoodPacket
    );

BOOLEAN
HwCheckMacParameters(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );

// Lock held
BOOLEAN
HwIsReceiveAvailable(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 DispatchLevel
    );

PHW_RX_MPDU
HwGetReceivedMPDU(
    _In_  PHW                     Hw
    );


// Called at Dispatch
NDIS_STATUS
HwProcessReceivedMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );


// Returns TRUE if we do software decryption (Even if decryption gave ICV errors, etc)
BOOLEAN
HwRxDecrypt(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );

// Failure only for catastrophic issues
NDIS_STATUS
HwProcessRxCipher(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );

PHW_KEY_ENTRY
HwFindDecryptionKey(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );

NDIS_STATUS
HwIdentifyReceiveMac(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );

NDIS_STATUS
HwFilterMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );

NDIS_STATUS
HwPrepareReceivedMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );

NDIS_STATUS
HwSecurityCheck(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu
    );

NDIS_STATUS
HwPrepareReceivedMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu
    );


// Lock held (or called from Initialize)
// Used to return a RxMPDU to the HAL for filling
__inline VOID
HwSubmitRxMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );

VOID
HwReturnRxMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu,
    _In_  BOOLEAN                 DispatchLevel
    );

NDIS_STATUS
HwGrowMPDUPool(
    _In_  PHW                     Hw,
    _In_  ULONG                   NumToAllocate
    );

VOID
HwReturnRxMSDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu,
    _In_  BOOLEAN                 DispatchLevel
    );

VOID
HwIndicateReceivedMSDUs(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             MsduList,
    _In_  ULONG                   ReceiveFlags
    );

PMP_RX_MSDU
HwMapHwRxMSDUToMpRxMSDU(
    _In_  PHW_RX_MSDU             Msdu
    );

VOID
HwIndicateMSDUOnMACContext(
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PHW_RX_MSDU             Msdu,
    _In_  ULONG                   ReceiveFlags
    );



/**
 * Internal helper function to help with reassembly. For each fragment received
 * belonging to an Rx MSDU, this function will add it to the appropriate position.
 * If the fragment cannot be added for any reason, this function will return
 * the appropriate error code.
 * 
 * \param pMpRxd            The Rx MSDU in which the fragments are to be assembled
 * \param pNicFragment      The fragment we just received
 * \param pFragmentHdr      The 802.11 header of the received fragment
 * \param usFragmentSize    The size, in bytes, of the received fragment
 * \param ucFragNumber      The 802.11 fragment number of this frag
 * \return NDIS_STATUS_SUCCESS if frag added successfully and the reassembly
 * operation got completed when the frag was added. Will return NDIS_STATUS_PENDING
 * if frag was added successfully but we have still not received all MPDUs for
 * this MSDU. Else, a failure code is returned.
 * 
 * \sa MpHandleReceiveInterrupt, MpPrepareRxMSDUForIndication
 */
__inline NDIS_STATUS
HwAddRxMPDUToMSDU(
    _In_  PHW_RX_MSDU                 Msdu,
    _In_  PHW_RX_MPDU                 Mpdu,
    _In_  PDOT11_MGMT_DATA_MAC_HEADER FragmentHeader,
    _In_  UCHAR                       FragNumber
    );




/**
 * Called when the miniport determines that a received fragment (MPDU) is part of
 * a bigger MSDU and we have to wait till all MPDU's have been received for this
 * miniport.
 * 
 * \param pAdapter      The adapter context for this miniport
 * \param pMpRxd        The Rx MSDU in which all the fragments will be assembled for this MSDU
 * \param DispatchLevel TRUE if current IRQL is DISPATCH
 * \return NDIS_STATUS_SUCCESS if added successfully, else failure
 * \sa MpRemoveMSDUFromReassemblyLine
 */
NDIS_STATUS
HwAddPartialMSDUToReassemblyLine(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu,
    _In_  BOOLEAN                 DispatchLevel
    );




/**
 * This function searches for an Rx MSDU in the reassembly line given the
 * sequence number and the Fragment Hdr (to match MAC address) of an 802.11 packet
 * 
 * \param pAdapter          The adapter context for this miniport
 * \param pFragmentHdr      802.11 header of a received fragment for which we need a match
 * \param usSequenceNumber  The Sequence Number we will use to match the RX_MSDU
 * \param 
 * \return 
 * \sa 
 */
__inline PHW_RX_MSDU
HwFindPartialMSDUInReassemblyLine(
    _In_  PHW                     Hw,
    _In_  PDOT11_MGMT_DATA_MAC_HEADER FragmentHdr, 
    _In_  USHORT                  SequenceNumber
    );



/**
 * Called when an Rx MSDU previously added to Reassembly line has to be removed. The MSDU
 * may have been assembled successfully or an unrecoverable error may have caused us
 * to drop all fragments in this MSDU
 * 
 * \param pAdapter      The adapter context for this miniport
 * \param pMpRxd        The Rx MSDU to be dropped
 * \sa MpAddPartialMSDUToReassemblyLine
 */
VOID
HwRemoveMSDUFromReassemblyLine(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MSDU             Msdu
    );

/**
 * This function goes through the Reassembly line and drops all Rx MSDUs that
 * have we were not able to assemble in the RX_LIFETIME of the packet. This
 * function needs to be run periodically to help get rid of any orphan Rx MSDUs in
 * the reassembly line that can never be completed.
 * 
 * \param pAdapter          The adapter context for this miniport
 * \param DispatchLevel     TRUE if IRQL is DISPATCH
 * \sa MpAddPartialMSDUToReassemblyLine
 */
__inline VOID
HwExpireReassemblingMSDUs(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 DispatchLevel
    );

VOID
HwFlushMSDUReassemblyLine(
    _In_  PHW                     Hw
    );

PHW_RX_MSDU
HwBuildMSDUForMPDU(
    _In_  PHW                     Hw,
    _In_  PHW_RX_MPDU             Mpdu
    );

VOID
HwHandleReceiveInterrupt(
    _In_  PHW                     Hw,
    _In_  ULONG                   MaxNblsToIndicate
    );

VOID
HwResetReceiveEngine(
    _In_  PHW                     Hw,
    _In_  BOOLEAN                 DispatchLevel
    );

VOID
HwWaitForPendingReceives(
    _In_  PHW                     Hw,
    _In_opt_ PHW_MAC_CONTEXT      MacContext
    );

