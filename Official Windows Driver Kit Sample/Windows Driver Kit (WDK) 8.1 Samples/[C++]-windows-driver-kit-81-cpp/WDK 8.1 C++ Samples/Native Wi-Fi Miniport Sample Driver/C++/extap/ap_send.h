/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_send.h

Abstract:
    ExtAP send processing functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    10-15-2007    Created

Notes:

--*/


#define MAX_SEND_MSDU_TO_PROCESS             5

NDIS_STATUS
ApSendMgmtPacket(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_reads_bytes_(MgmtPacketSize) PUCHAR MgmtPacket,
    _In_ USHORT MgmtPacketSize
    );

/** 
 * Called with PORT::Lock held
 */
NDIS_STATUS 
Ap11SendEventHandler(
    _In_ PMP_PORT Port,
    _In_ PMP_TX_MSDU   PacketList,
    _In_ ULONG SendFlags
    );

/**
 * Handle send completion event
 */
VOID 
Ap11SendCompleteEventHandler(
    _In_ PMP_PORT Port,
    _In_ PMP_TX_MSDU   PacketList,
    _In_ ULONG SendCompleteFlags
    );

