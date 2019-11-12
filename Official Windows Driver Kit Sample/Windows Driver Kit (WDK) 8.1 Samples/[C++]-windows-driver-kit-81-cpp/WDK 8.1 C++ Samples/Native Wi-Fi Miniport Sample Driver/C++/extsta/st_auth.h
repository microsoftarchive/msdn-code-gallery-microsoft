/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_auth.h

Abstract:
    STA layer authentication frame processing functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_auth.h to st_auth.h
    
Notes:

--*/
#ifndef _STATION_AUTHENTICATION_H_
#define _STATION_AUTHENTICATION_H_

/**
 * Starts exchanging authentication packets with the access point. 
 * 
 * \param pStation          STATION structure
 * \param pAPEntry          APEntry to authenticate with
 * \param AuthenticateTimeout  Max time to wait for authentication completion,
 *                             expressed in 100 nanoseconds
 * \return NDIS_STATUS  If the function fails, it must complete association process
 *                      before returning.
 * \sa StaAuthenticateComplete
 */
NDIS_STATUS
StaAuthenticate(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  ULONG           AuthenticateTimeout
    );

/**
 * Called to complete an authentication attempt. This is generally called on receiving
 * authentication response (depending on authentication type) or on timeout
 * 
 * \param pStation          STATION structure
 * \param Status            Status code for the authentication attempt. If the
 *                          status is zero, the authentication is considered a 
 *                          success and association step initiated
 * \sa 
 */
VOID
StaAuthenticateComplete(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  ULONG           Status
    );

/**
 * Timer to trigger authentication timeout
 */
NDIS_TIMER_FUNCTION StaAuthenticateTimer;

/**
 * Timeout call back in case of timeout waiting for authentication response. This 
 * can also be called from the reset routine to expedite association process 
 * completion. This means it may be called at IRQL_PASSIVE or IRQL_DISPATCH
 *
 * \param param             STATION structure
 */
VOID
StaAuthenticateTimeoutCallback(
    _In_  PMP_EXTSTA_PORT                        pStation
    );

/**
 * Sends a deauthentication packet to an access point. The caller is responsible
 * for determining if the packet must be sent or not, changing states and
 * indicating status
 * 
 * \param pStation          STATION structure
 * \param pAPEntry          APEntry to deauthenticate with
 * \param AssociateTimeout  Reason to use in the deauthenticate request
 * \return NDIS_STATUS  NDIS_STATUS from the send attempt
 */
NDIS_STATUS
StaSendDeauthentication(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  USHORT          ReasonCode
    );


/**
 * Starts exchanging 802.11 authentication packets with the access point
 * 
 * \param pStation          STATION structure
 * \param pAPEntry          APEntry to authenticate with
 * \param AuthenticateTimeout  Max time to wait for authentication completion,
 *                             expressed in 100 nanoseconds
 * \return NDIS_STATUS  If the function fails, it must complete association process
 *                      before returning.
 * \sa StaAuthenticateComplete
 */
NDIS_STATUS
StaStart80211Authentication(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  ULONG           AuthenticateTimeout
    );

/**
 * Timeout call back in case of timeout waiting for authentication response. This 
 * can also be called from the reset routine to expedite association process 
 * completion. This means it may be called at IRQL_PASSIVE or IRQL_DISPATCH
 *
 * \param param             STATION structure
 */
VOID
StaCancel80211Authentication(
    _In_  PMP_EXTSTA_PORT                        pStation
    );

/**
 * Invoked on receiving an authentication packet from an access point. This
 * function is used for 802.11 authentication
 * 
 * \param pStation          STATION structure
 * \param pPacketBuffer     Authentication packet contents
 * \param PacketLength      Length of authentication packet
 */
VOID
StaProcess80211AuthPacket(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_reads_bytes_(PacketLength)  PUCHAR                          pPacketBuffer,
    _In_  ULONG                           PacketLength
    );

/**
 * Allocates and populate the first 802.11 authentication request packet. 
 * 
 * \param pStation              STATION structure
 * \param pAPEntry              The APEntry this authentication request packet is for
 * \param ppAuthPacket          Returns pointer to authentication request packet
 * \param pAuthPacketLength     Authentication request packet length
 * \return NDIS_STATUS
 * \sa StaFree80211AuthReqPacket
 */
NDIS_STATUS
StaCreate80211AuthReqPacket(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _Outptr_result_bytebuffer_(*pAuthPacketLength) PUCHAR*         ppAuthPacket,
    _Out_ PUSHORT         pAuthPacketLength
    );

/**
 * Populate the shared key authentication challenge response packet
 * 
 * \param pStation              STATION structure
 * \param pAPEntry              The APEntry this authentication request packet is for
 * \param pPacketBuffer         Pre-allocated buffer for the auth packet with challenge
 *                              text already in place
 * \sa StaCreate80211AuthReqPacket and StaFree80211AuthReqPacket
 */
void
StaFillSharedKeyAuthChallengResPacket(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_reads_bytes_(sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_AUTH_FRAME))  PUCHAR          pPacketBuffer
    );

/**
 * Frees a previously allocate 802.11 authentication request packet
 * 
 * \param pStation              STATION structure
 * \param pAPEntry              The APEntry this authentication request packet was for
 * \param pAuthPacket           Pointer to authentication request packet
 * \param AuthPacketLength      Authentication request packet length
 * \sa StaCreate80211AuthReqPacket
 */

VOID
StaFree80211AuthReqPacket(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_ PUCHAR          pAuthPacket,
    _In_ USHORT          AuthPacketLength
    );


#endif // _STATION_AUTHENTICATION_H_
