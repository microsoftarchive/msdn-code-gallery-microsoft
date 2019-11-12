/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_intf.h

Abstract:
    Contains interfaces into the ExtSTA port
    
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


_At_(Attr->ExtSTAAttributes, _Post_notnull_)
NDIS_STATUS
Sta11Fill80211Attributes(
    _In_  PMP_PORT                Port,
    _Inout_ PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    );

VOID
Sta11Cleanup80211Attributes(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    );


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Sta11LoadRegistryInformation(
    _In_      NDIS_HANDLE             MiniportAdapterHandle,
    _In_opt_  NDIS_HANDLE             ConfigurationHandle,
    _Out_     PVOID*                  RegistryInformation
    );

VOID
Sta11FreeRegistryInformation(
    _In_opt_  PVOID                   RegistryInformation
    );

NDIS_STATUS
Sta11AllocatePort(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _In_  PADAPTER                Adapter,
    _In_  PMP_PORT                Port
    );

VOID
Sta11FreePort(
    _In_  PMP_PORT                Port
    );


NDIS_STATUS
Sta11InitializePort(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   RegistryInformation
    );

VOID
Sta11TerminatePort(
    _In_  PMP_PORT                Port
    );

NDIS_STATUS
Sta11PausePort(
    _In_  PMP_PORT                Port
    );

NDIS_STATUS
Sta11RestartPort(
    _In_  PMP_PORT                Port
    );

NDIS_STATUS
Sta11NdisResetStart(
    _In_  PMP_PORT                Port
    );

NDIS_STATUS
Sta11NdisResetEnd(
    _In_  PMP_PORT                Port
    );

NDIS_STATUS
Sta11SetPower(
    _In_  PMP_PORT                Port,
    _In_  NDIS_DEVICE_POWER_STATE NewDevicePowerState
    );

/*
 * This is the OID handler for a station. It handles most of the station specific 
 * OIDs and forwards the rest up to the BasePort for processing.
 * 
 * Look at Port11HandleOidRequest for information about when this function is
 * called
 */
NDIS_STATUS
Sta11OidHandler(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    );

/*
 * This is the send notifications handler for a station. It can return
 * NDIS_STATUS_PENDING if the station is currently scanning or in the middle of 
 * a roam. Else it would permit the packets go through
 * 
 * Look at Port11NotifySend for information about when this function is
 * called
 */
NDIS_STATUS
Sta11SendEventHandler(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendFlags
    );

NDIS_STATUS
Sta11SetOperationMode(
    _In_  PMP_PORT                Port,
    _In_  ULONG                   OpMode
    );

/*
 * This is the receive notifications handler for a station. It looks at
 * management packets to update association state. It also looks at data packets
 * and does filtering based on privacy settings. If the packet is mal-formed
 * or not correctly ciphered, this function can return a false to block
 * the packet from getting indicated up to the OS
 * 
 * Look at Port11NotifyReceive for information about when this function is
 * called
 */
NDIS_STATUS
Sta11ReceiveEventHandler(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    );


VOID
Sta11IndicateStatus(
    _In_  PMP_PORT                Port,
    _In_  NDIS_STATUS             StatusCode,
    _In_  PVOID                   StatusBuffer,
    _In_  ULONG                   StatusBufferSize
    );

NDIS_STATUS
Sta11ScanComplete(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   Data
    );

VOID
Sta11Notify(
    _In_  PMP_PORT        Port,
    PVOID               Notif
);


