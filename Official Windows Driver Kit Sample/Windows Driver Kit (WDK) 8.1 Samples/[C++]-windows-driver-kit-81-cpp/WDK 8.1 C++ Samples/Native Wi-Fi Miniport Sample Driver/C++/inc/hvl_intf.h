/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hvl_intf.h

Abstract:
    Contains interfaces into the HVL
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

//
// Forward declaration
//
typedef struct _ADAPTER         ADAPTER, *PADAPTER;
typedef struct _HVL             HVL, *PHVL;
typedef struct _HW              HW, *PHW;
typedef struct _MP_PORT         MP_PORT, *PMP_PORT;
typedef struct _VNIC            VNIC, *PVNIC;
typedef struct _HW_MAC_CONTEXT  HW_MAC_CONTEXT, *PHW_MAC_CONTEXT;

NDIS_STATUS
Hvl11Allocate(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _Outptr_result_maybenull_ PHVL*         ppHvl,
    _In_  PADAPTER                Adapter
    );


VOID
Hvl11Free(
    _In_ _Post_ptr_invalid_ PHVL                    Hvl
    );

NDIS_STATUS
Hvl11Initialize(
    _In_  PHVL                    Hvl,
    _In_  PHW                     Hw
    );

VOID
Hvl11Terminate(
    _In_  PHVL                    Hvl
    );

NDIS_STATUS
Hvl11Fill80211Attributes(
    _In_  PHVL                    Hvl,
    _Out_ PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    );

VOID
Hvl11Cleanup80211Attributes(
    _In_  PHVL                    Hvl,
    _In_  PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    );

VOID
Hvl11EnableContextSwitches(
    _In_  PHVL                    Hvl
    );

VOID
Hvl11ActivatePort(
    _In_  PHVL                    Hvl,
    _In_  PVNIC                   VNic
    );

NDIS_STATUS
Hvl11AllocateMACContext(
    _In_  PHVL                    Hvl,
    _Out_ PHW_MAC_CONTEXT*        MacContext,
    _In_  PVNIC                   VNic
    );

VOID
Hvl11FreeMACContext(
    _In_  PHVL                    Hvl,
    _In_  PHW_MAC_CONTEXT         MacContext
    );


VOID 
Hvl11SendCompletePackets(
    _In_  PVNIC                   VNic,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   NumPkts,
    _In_  ULONG                   SendCompleteFlags
    );

VOID
Hvl11IndicateReceivePackets(
    _In_  PVNIC                   VNic,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    );

NDIS_STATUS
Hvl11RegisterVNic(
    _In_  PHVL    Hvl,
    _In_  PVNIC   pVNic               
    );

NDIS_STATUS
Hvl11DeregisterVNic(
    _In_  PHVL    Hvl,
    _In_  PVNIC   pVNic               
    );
NDIS_STATUS
Hvl11RegisterHelperPort(
    _In_  PHVL    pHvl,
    _In_  PVNIC   pVNic               
    );

NDIS_STATUS
Hvl11DeregisterHelperPort(
    _In_  PHVL    pHvl,
    _In_  PVNIC   pVNic               
    );

NDIS_STATUS
Hvl11RequestExAccess(
    _In_ PHVL Hvl, 
    _In_ PVNIC VNic,
    _In_ BOOLEAN fPnPOperation
    );

NDIS_STATUS
Hvl11ReleaseExAccess(
    PHVL Hvl, 
    PVNIC VNic
    );

VOID
Hvl11BlockTimedCtxS(_Inout_ PHVL Hvl);

VOID
Hvl11UnblockTimedCtxS(_Inout_ PHVL Hvl);

VOID
Hvl11IndicateStatus(
    _In_  PVNIC                   VNic,
    _In_  NDIS_STATUS             StatusCode,
    _In_  PVOID                   StatusBuffer,
    _In_  ULONG                   StatusBufferSize
    );

VOID
Hvl11Notify(
    PHVL                    Hvl,
    PVOID                   pvNotif
    );

