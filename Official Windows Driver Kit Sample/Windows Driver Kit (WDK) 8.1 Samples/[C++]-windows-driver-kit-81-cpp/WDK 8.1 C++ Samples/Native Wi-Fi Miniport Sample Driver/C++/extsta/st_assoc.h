/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_assoc.h

Abstract:
    STA layer association frame processing functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_assoc.h to st_assoc.h
    
Notes:

--*/
#ifndef _STATION_ASSOCIATION_H_
#define _STATION_ASSOCIATION_H_

/**
 * Allocates and populate the association request packet. 
 * 
 * \param pStation              STATION structure
 * \param pAPEntry              The APEntry this association request packet is for
 * \param ppAssocPacket         Returns pointer to association request packet
 * \param pAssocPacketLength    Association request packet length
 * \return NDIS_STATUS
 * \sa StaFreeAssociateRequestPacket
 */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaCreateAssociateRequestPacket(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _Outptr_result_buffer_to_(
        STA11_MAX_IE_BLOB_SIZE + sizeof(DOT11_MGMT_HEADER) +  sizeof(DOT11_ASSOC_REQUEST_FRAME),
        *pAssocPacketLength)
    _Result_nullonfailure_
          PUCHAR* ppAssocPacket,
    _Out_ PUSHORT         pAssocPacketLength
    );

/**
 * Frees a previously allocate association request packet
 * 
 * \param pStation              STATION structure
 * \param pAPEntry              The APEntry this association request packet was for
 * \param pAssocPacket          Pointer to association request packet
 * \param AssocPacketLength     Association request packet's data length. The memory
 *                              allocated may be larger than this length
 * \sa StaCreateAssociateRequestPacket
 */
VOID
StaFreeAssociateRequestPacket(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_ PUCHAR          pAssocPacket,
    _In_ USHORT          AssocPacketLength
    );


/**
 * Starts exchanging association packets with the access point. 
 * 
 * \param pStation          STATION structure
 * \param pAPEntry          APEntry to associate with
 * \param AssociateTimeout  Max time to wait for association completion, expressed 
 *                          in 100 nanoseconds
 * \return NDIS_STATUS  If the function fails, it must complete association process
 *                      before returning.
 * \sa StaAssociationComplete
 */
NDIS_STATUS
StaAssociate(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  ULONG           AssociateTimeout
    );

/**
 * Called to complete an association attempt. This is generally called on receiving
 * association response or on timeout
 * 
 * \param pStation          STATION structure
 * \param Status            Status code for the association attempt. If the
 *                          status is zero, the association is considered a 
 *                          success
 * \sa 
 */
VOID
StaAssociateComplete(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  ULONG           Status
    );

/**
 * Timer to trigger association timeout
 */
NDIS_TIMER_FUNCTION StaAssociateTimer;

/**
 * Timeout call back in case association has timed out. This can also be called
 * from the reset routine to expedite association process completion. This means
 * it may be called at IRQL_PASSIVE or IRQL_DISPATCH
 *
 * \param param             STATION structure
 */

VOID
StaAssociateTimeoutCallback(
    PMP_EXTSTA_PORT             Station
    );


/**
 * Sends a disassociation packet to an access point. The caller is responsible
 * for determining if the packet must be sent or not, changing states and
 * indicating status
 * 
 * \param pStation          STATION structure
 * \param pAPEntry          APEntry to disassociate with
 * \param AssociateTimeout  Reason to use in the disassociation request
 * \return NDIS_STATUS  NDIS_STATUS from the send attempt
 */
NDIS_STATUS
StaSendDisassociation(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  USHORT          ReasonCode
    );



#endif // _STATION_ASSOCIATION_H_
