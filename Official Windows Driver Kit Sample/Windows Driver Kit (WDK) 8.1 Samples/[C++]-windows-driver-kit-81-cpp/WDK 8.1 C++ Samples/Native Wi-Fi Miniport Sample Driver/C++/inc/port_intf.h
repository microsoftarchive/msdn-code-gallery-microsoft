/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    port_intf.h

Abstract:
    Contains interfaces into the port
    
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

#define DEFAULT_NDIS_PORT_NUMBER    0
#define HELPER_PORT_PORT_NUMBER     0xFFFFFFFF

// Macro to verify if this port number is allocated/"registered" with NDIS
#define IS_ALLOCATED_PORT_NUMBER(_PortNumber)   (_PortNumber != DEFAULT_NDIS_PORT_NUMBER && _PortNumber != HELPER_PORT_PORT_NUMBER)



NDIS_STATUS
Port11Fill80211Attributes(
    _In_  PADAPTER                Adapter,
    _Inout_ PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    );

VOID
Port11Cleanup80211Attributes(
    _In_  PADAPTER                Adapter,
    _In_  PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES attr
    );

NDIS_STATUS
Port11LoadRegistryInformation(
    _In_                        PADAPTER      Adapter,
    _In_opt_                    NDIS_HANDLE   ConfigurationHandle,
    _Outptr_result_maybenull_   PVOID*        RegistryInformation
    );

VOID
Port11FreeRegistryInformation(
    _In_ __drv_freesMem(Mem)   PVOID          RegistryInformation
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Port11AllocatePort(
    _In_  PADAPTER                  Adapter,
    _In_  MP_PORT_TYPE              PortType,
    _Outptr_result_nullonfailure_   PMP_PORT*     Port
    );

VOID
Port11FreePort(
    _In_  PMP_PORT                Port,
    _In_  BOOLEAN                 FreeBasePort
    );

NDIS_STATUS
Port11InitializePort(
    _In_  PMP_PORT                Port,
    _In_  PHVL                    Hvl,
    _In_  PVOID                   RegistryInformation
    );

VOID
Port11TerminatePort(
    _In_  PMP_PORT                Port,
    _In_  BOOLEAN                 TerminateBasePort
    );

PMP_PORT
Port11TranslatePortNumberToPort(
    _In_  PADAPTER                Adapter,
    _In_  NDIS_PORT_NUMBER        PortNumber
    );

VOID
Port11RemovePortFromAdapterList(
    _In_ PMP_PORT                 PortToRemove
    );
    
NDIS_STATUS
Port11PausePort(
    _In_  PMP_PORT                Port,
    _In_  BOOLEAN                 IsInternal
    );

NDIS_STATUS
Port11RestartPort(
    _In_  PMP_PORT                Port,
    _In_  BOOLEAN                 IsInternal
    );

NDIS_STATUS
Port11NdisResetStart(
    _In_  PMP_PORT                Port
    );

NDIS_STATUS
Port11NdisResetEnd(
    _In_  PMP_PORT                Port
    );

NDIS_STATUS
Port11SetPower(
    _In_  PMP_PORT                Port,
    _In_  NDIS_DEVICE_POWER_STATE NewDevicePowerState
    );

/**
 * This is called from the MP layer when NDIS sends an OID request on a port using 
 * MiniportOidRequest. The call gets forwarded to the port specific OID request handler
 * for processing. The port specific handler may process the OID or may pass the
 * request to the BasePortOidHandler function in the base port to process some of the
 * OIDs.
 *
 */
NDIS_STATUS
Port11HandleOidRequest(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    );

VOID
Port11CompletePendingOidRequest(
    _In_  PMP_PORT                Port,
    _In_  NDIS_STATUS             NdisStatus
    );


/**
 * This is called from the MP layer when NDIS sends an OID request on a port using 
 * MiniportDirectOidRequest. The call gets forwarded to the port specific OID request handler
 * for processing
 *
 */
NDIS_STATUS
Port11HandleDirectOidRequest(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    );

VOID
Port11CompletePendingDirectOidRequest(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest,
    _In_  NDIS_STATUS             NdisStatus
    );

/**
 * This is called from the MP layer when NDIS calls MiniportSend to send packets on this port.
 * This gets forwarded to the BasePortHandleSendNetBufferLists handler. 
 * 
 * See BasePortHandleSendNetBufferLists for more information
 */
VOID
Port11HandleSendNetBufferLists(
    _In_  PMP_PORT                Port,
    _In_  PNET_BUFFER_LIST        NetBufferLists,
    _In_  ULONG                   SendFlags
    );

/**
 * This is called by the base port for all OS submitted send packets to let a port
 * modify/reject packets before they are sent to the hardware. This is
 * called from the context of Port11HandleSendNetBufferLists or Port11SendCompletePackets.
 *
 * This function is invoked with the PORT Lock held.
 *
 * See BasePortHandleSendNetBufferLists for information on when this is called
 */
NDIS_STATUS
Port11NotifySend(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendFlags
    );

/**
 * This is called by the HW when send packets are completed. This gets forwarded to 
 * the BasePortSendCompletePackets handler. 
 *
 * See BasePortSendCompletePackets for more information on what happens when this is called
 */

VOID 
Port11SendCompletePackets(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendCompleteFlags
    );

/**
 * This is called by the base port on completion of all OS submitted send packets. This
 * allows a port to undo any actions it may have taken on the packets in Port11NotifySend.
 * This is called in the context of Port11SendCompletePackets
 * 
 * See BasePortSendCompletePackets for more information on when this is called 
 */
VOID
Port11NotifySendComplete(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendCompleteFlags
    );

/**
 * This is called by the HW when receive packets are available for indication.
 * This gets forwarded to the BasePortIndicateReceivePackets handler. 
 *
 * See BasePortIndicateReceivePackets for more information on what happens 
 * when this is called
 */
VOID
Port11IndicateReceivePackets(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    );

/**
 * This is called by the base port when it received packet indications from the HW.
 * This allows a port to reject any packets that it believes should not be indicated to the
 * OS. This is called in the context of Port11IndicateReceivePackets
 * 
 * See BasePortIndicateReceivePackets for more information on when this is called 
 */
NDIS_STATUS
Port11NotifyReceive(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    );

/**
 * This is called by the MP layer when packets receive indicated on this port are returned 
 * by NDIS. This gets forwarded to the BasePortHandleReturnNetBufferLists handler.
 *
 * See BasePortHandleReturnNetBufferLists for more information on what happens 
 * when this is called
 */
VOID
Port11HandleReturnNetBufferLists(
    _In_  PMP_PORT                Port,
    _In_  PNET_BUFFER_LIST        NetBufferLists,
    _In_  ULONG                   ReturnFlags
    );

/**
 * This is called by the base port on the return of all packets receive indicated to the OS. 
 * This allows a port to undo any actions it may have taken on the packets in 
 * Port11NotifyReceive. This is called in the context of Port11HandleReturnNetBufferLists
 * 
 * See BasePortHandleReturnNetBufferLists for more information on when this is called 
 */
VOID
Port11NotifyReturn(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReturnFlags
    );

VOID
Port11IndicateStatus(
    _In_  PMP_PORT                Port,
    _In_  NDIS_STATUS             StatusCode,
    _In_  PVOID                   StatusBuffer,
    _In_  ULONG                   StatusBufferSize
    );

VOID
Port11Notify(
    _In_  PMP_PORT            Port,
    _In_  PVOID               Notif
);
