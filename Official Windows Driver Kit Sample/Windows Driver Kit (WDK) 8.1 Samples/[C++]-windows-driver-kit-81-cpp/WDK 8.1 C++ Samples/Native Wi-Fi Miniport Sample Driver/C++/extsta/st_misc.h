/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_misc.h

Abstract:
    STA layer utility functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_misc.h to st_misc.h

Notes:

--*/
#ifndef _STATION_MISC_H
#define _STATION_MISC_H

#define STA_INVALID_PHY_ID          0x80000000U


ULONG
StaGetPhyIdByType(
    _In_ PMP_EXTSTA_PORT pStation,
    _In_ DOT11_PHY_TYPE PhyType
    );

DOT11_PHY_TYPE
StaGetPhyTypeById(
    _In_ PMP_EXTSTA_PORT pStation,
    _In_ ULONG PhyId
    );

NDIS_STATUS
StaFilterUnsupportedRates(
    _In_ PMP_BSS_ENTRY pAPEntry, 
    _In_ PDOT11_RATE_SET rateSet
    );


_At_(*ppCurrentIE, _Writes_and_advances_ptr_(*pIESize))
NDIS_STATUS
StaAttachInfraRSNIE(
    _In_ PMP_EXTSTA_PORT pStation, 
    _In_ PMP_BSS_ENTRY pAPEntry, 
    _Inout_ PUCHAR *ppCurrentIE,
    _Inout_ PUSHORT pIESize
    );

_At_(*pIESize, _In_range_(>=, sizeof(DOT11_INFO_ELEMENT) +
                              sizeof(USHORT) +
                              sizeof(ULONG) +
                              sizeof(USHORT) + sizeof(ULONG) +
                              sizeof(USHORT) + sizeof(ULONG) +
                              sizeof(USHORT)))
_At_(*ppCurrentIE, _Writes_and_advances_ptr_(*pIESize))
NDIS_STATUS
StaAttachAdHocRSNIE(
    _In_    PMP_EXTSTA_PORT pStation, 
    _Inout_ PUCHAR *ppCurrentIE,
    _Inout_ PUSHORT pIESize
    );

BOOLEAN
StaMatchRSNInfo(
    _In_ PMP_EXTSTA_PORT        pStation,
    _In_ PRSN_IE_INFO    RSNIEInfo
    );

VOID 
StaIndicateDot11Status(
    _In_ PMP_EXTSTA_PORT        pStation,
    _In_  NDIS_STATUS     StatusCode,
    _In_opt_  PVOID           RequestID,
    _In_  PVOID           pStatusBuffer,
    _In_  ULONG           StatusBufferSize
    );

VOID
Sta11Notify(
    _In_  PMP_PORT        Port,
    PVOID               Notif
);

#endif // _STATION_MAIN_H
