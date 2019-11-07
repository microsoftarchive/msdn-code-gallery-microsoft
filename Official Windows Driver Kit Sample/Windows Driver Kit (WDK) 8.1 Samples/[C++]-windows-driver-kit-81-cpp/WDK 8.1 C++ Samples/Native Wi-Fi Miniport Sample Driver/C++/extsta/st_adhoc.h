/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_adhoc.h

Abstract:
    STA layer adhoc functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_adhoc.h to st_adhoc.h

Notes:

--*/
#ifndef _STATION_ADHOC_H
#define _STATION_ADHOC_H

#define STA11_ADHOC_JOIN_TIMEOUT                20              /* 20 beacons */
#define STA_ADHOC_MIN_UNREACHABLE_THRESHOLD     5000            /* 5 seconds */
#define STA_ADHOC_MAX_UNREACHABLE_THRESHOLD     30000           /* 30 seconds */
#define STA_DEAUTH_WAITING_THRESHOLD            3               /* 3 ticks, or 6 seconds */
#define STA_PROBE_REQUEST_LIMIT                 2               /* 2 probe requests */

/**
 * Information Element to be put in beacon for FHSS phy
 */
typedef struct STA_FHSS_IE {
    USHORT dot11DwellTime;
    UCHAR dot11HopSet;
    UCHAR dot11HopPattern;
    UCHAR dot11HopIndex;
} STA_FHSS_IE, *PSTA_FHSS_IE;

/**
 * Initializes the AdHoc station info
 * 
 * \param pStation  station pointer
 */
NDIS_STATUS
StaInitializeAdHocStaInfo(
    _In_  PMP_EXTSTA_PORT        pStation
    );

/**
 * Free the AdHoc station info
 * 
 * \param pStation  station pointer
 */
VOID
StaFreeAdHocStaInfo(
    _In_  PMP_EXTSTA_PORT        pStation
    );

NTSTATUS
StaConnectAdHoc(
    _In_  PMP_EXTSTA_PORT pStation
    );

NTSTATUS
StaDisconnectAdHoc(
    _In_  PMP_EXTSTA_PORT pStation
    );
    
NDIS_STATUS 
StaSaveAdHocStaInfo(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_RX_MPDU        pFragment,
    _In_  PDOT11_BEACON_FRAME pDot11BeaconFrame,
    _In_  ULONG           InfoElemBlobSize
    );

NDIS_STATUS
StaResetAdHocStaInfo(
    _In_  PMP_EXTSTA_PORT       pStation,
    _In_  BOOLEAN        flushStaList
    );

void
StaClearStaListAssocState(
    _In_  PMP_EXTSTA_PORT    pStation,
    _In_  BOOLEAN     SendDeauth
    );

NDIS_IO_WORKITEM_FUNCTION StaConnectAdHocWorkItem;

void 
StaAdHocReceiveDirectData(
    _In_  PMP_EXTSTA_PORT                    pStation,
    _In_  PDOT11_DATA_SHORT_HEADER    pDataHdr
    );

VOID
StaAdhocIndicateConnectionStart(
    _In_  PMP_EXTSTA_PORT         pStation,
    _In_reads_bytes_opt_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       BSSID,
    _In_  BOOLEAN                 Internal
    );

VOID
StaAdhocIndicateConnectionCompletion(
    _In_  PMP_EXTSTA_PORT         pStation,
    _In_  ULONG                   CompletionStatus,
    _In_  BOOLEAN                 Internal
    );

void 
StaAdHocIndicateAssociation(
    _In_  PMP_EXTSTA_PORT pStation,
    _In_  PSTA_ADHOC_STA_ENTRY StaEntry
    );

NDIS_STATUS
StaFlushAdHocStaList(
    _In_  PMP_EXTSTA_PORT         pStation
    );

NDIS_TIMER_FUNCTION StaAdHocWatchdogTimerRoutine;

VOID
StaAdhocProcessMgmtPacket(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_  PDOT11_MGMT_HEADER              MgmtPacket,
    _In_  ULONG                           PacketLength
    );

VOID
StaStopAdHocBeaconing(
    _In_  PMP_EXTSTA_PORT                        pStation
    );

NDIS_STATUS
StaEnumerateAssociationInfoAdHoc(
    _In_  PMP_EXTSTA_PORT        pStation,
     _Inout_updates_bytes_(TotalLength) PDOT11_ASSOCIATION_INFO_LIST   pAssocInfoList,
    _In_  ULONG           TotalLength
    );

NDIS_STATUS
StaStartAdHocBeaconing(
    _In_  PMP_EXTSTA_PORT pStation
    );

VOID
StaStopAdHocBeaconing(
    _In_  PMP_EXTSTA_PORT pStation
    );


#endif
