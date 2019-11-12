/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_recv.h

Abstract:
    AP layer receive processing functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    10-08-2007    Created

Notes:

--*/
#ifndef _AP_RECV_H_
#define _AP_RECV_H_

#include <packon.h>
#include <packoff.h>

/**
 * Processing management packet
 */
NDIS_STATUS
ApReceiveMgmtPacket(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PMP_RX_MPDU     Fragment,
    _In_ USHORT FragmentSize
    );

/**
 * Processing authentication
 */
VOID 
ApReceiveAuthentication(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    );

/**
 * Processing deauthentication
 */
VOID 
ApReceiveDeauthentication(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    );

/**
 * Processing association request
 */
VOID 
ApReceiveAssociationRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    );

/**
 * Processing reassociation request
 */
VOID 
ApReceiveReassociationRequest(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    );

/**
 * Processing disassociation
 */
VOID 
ApReceiveDisassociation(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    );

#endif // _AP_RECV_H_
