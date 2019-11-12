/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_intf.h

Abstract:
    Contains interfaces into the ExtAP port
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-20-2007    Created

Notes:

--*/
#pragma once

#ifndef _EXTAP_INTF_H
#define _EXTAP_INTF_H

//
// Forward declaration
//
typedef struct _ADAPTER         ADAPTER, *PADAPTER;
typedef struct _HVL             HVL, *PHVL;
typedef struct _HW              HW, *PHW;
typedef struct _MP_PORT         MP_PORT, *PMP_PORT;
typedef struct _VNIC            VNIC, *PVNIC;


NDIS_STATUS
Ap11Fill80211Attributes(
    _In_  PMP_PORT              Port,
    _Inout_ PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    );

VOID
Ap11Cleanup80211Attributes(
    _In_  PMP_PORT              Port,
    _In_  PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    );

NDIS_STATUS
Ap11LoadRegistryInformation(
    _In_                      NDIS_HANDLE           MiniportAdapterHandle,
    _In_opt_                  NDIS_HANDLE           ConfigurationHandle,
    _Outptr_result_maybenull_ PVOID*                RegistryInformation
    );

VOID
Ap11FreeRegistryInformation(
    _In_  PVOID                 RegistryInformation
    );

NDIS_STATUS
Ap11AllocatePort(
	_In_  NDIS_HANDLE           MiniportAdapterHandle,
    _In_  PADAPTER              Adapter,
    _In_  PMP_PORT              Port
    );

VOID
Ap11FreePort(
    _In_  PMP_PORT              Port
    );


NDIS_STATUS
Ap11InitializePort(
    _In_  PMP_PORT              Port,
    _In_  PVOID                 RegistryInformation
    );

VOID
Ap11TerminatePort(
    _In_  PMP_PORT              Port
    );

NDIS_STATUS
Ap11PausePort(
    _In_  PMP_PORT              Port
    );

NDIS_STATUS
Ap11RestartPort(
    _In_  PMP_PORT              Port
    );

NDIS_STATUS
Ap11NdisResetStart(
    _In_  PMP_PORT              Port
    );

NDIS_STATUS
Ap11NdisResetEnd(
    _In_  PMP_PORT              Port
    );

NDIS_STATUS
Ap11SetPower(
    _In_  PMP_PORT              Port,
    _In_  NDIS_DEVICE_POWER_STATE NewDevicePowerState
    );
    
NDIS_STATUS
Ap11OidHandler(
    _In_  PMP_PORT              Port,
    _In_  PNDIS_OID_REQUEST     NdisOidRequest
    );


NDIS_STATUS 
Ap11DirectOidHandler(
    _In_  PMP_PORT              Port,
    _In_  PNDIS_OID_REQUEST     OidRequest
    );

VOID 
Ap11SendCompleteHandler(
    _In_  PMP_PORT              Port,
    _In_  PMP_TX_MSDU           PacketList,
    _In_  ULONG                 SendCompleteFlags
    );

VOID 
Ap11SendNBLHandler(
    _In_  PMP_PORT              Port,
    _In_  PNET_BUFFER_LIST      NetBufferLists,
    _In_  ULONG                 SendFlags
    );

NDIS_STATUS 
Ap11ReceiveHandler(
    _In_  PMP_PORT              Port,
    _In_  PMP_RX_MSDU           PacketList,
    _In_  ULONG                 ReceiveFlags
    );

VOID
Ap11IndicateStatus(
    _In_  PMP_PORT              Port,
    _In_  NDIS_STATUS           StatusCode,
    _In_  PVOID                 StatusBuffer,
    _In_  ULONG                 StatusBufferSize
    );

VOID
Ap11Notify(
    _In_  PMP_PORT        Port,
    PVOID               Notif
);

#endif  // _EXTAP_INTF_H
