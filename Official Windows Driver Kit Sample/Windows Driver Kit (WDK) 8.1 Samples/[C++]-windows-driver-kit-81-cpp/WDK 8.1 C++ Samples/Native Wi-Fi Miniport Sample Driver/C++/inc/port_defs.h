/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    port_defs.h

Abstract:
    Contains defines for the port structure. This should be used only
    by components that need to look inside the port structure
    
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once
#include "TxPacketQ.h"

//
// Forward declaration
//
typedef struct _ADAPTER         ADAPTER, *PADAPTER;
typedef struct _HVL             HVL, *PHVL;
typedef struct _HW              HW, *PHW;
typedef struct _VNIC            VNIC, *PVNIC;


/*
 * Look at Port11HandleOidRequest for information about this function
 */
typedef NDIS_STATUS (*PORT11_OID_REQUEST_FUNC)(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       OidRequest
    );

/*
 * Look at Port11NotifySend for information about this function
 */
typedef NDIS_STATUS (*PORT11_SEND_EVENT_FUNC)(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendFlags
    );

/*
 * Look at Port11NotifySendComplete for information about this function
 */
typedef VOID (*PORT11_SEND_COMPLETE_EVENT_FUNC)(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendCompleteFlags
    );

/*
 * Look at Port11NotifyReceive for information about this function
 */
typedef NDIS_STATUS (*PORT11_RECEIVE_EVENT_FUNC)(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    );

/*
 * Look at Port11NotifyReturn for information about this function
 */
typedef VOID (*PORT11_RETURN_EVENT_FUNC)(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReturnFlags
    );

typedef enum _MP_PORT_OP_STATE
{
    INIT_STATE = 0,
    OP_STATE
} MP_PORT_OP_STATE, *PMP_PORT_OP_STATE;

/** Contains configuration information read from the registry for each of the port */
typedef struct _MP_PORT_REG_INFO
{
    /** Helper port's info */
    PVOID                       HelperPortRegInfo;
    /** EXTSTA port's info */
    PVOID                       ExtStaRegInfo;
    /** EXTAP port's info */
    PVOID                       ExtAPRegInfo;
}MP_PORT_REG_INFO, *PMP_PORT_REG_INFO;

// Port status flags and manipulation macros
#define MP_PORT_PAUSED                  0x00000001
#define MP_PORT_PAUSING                 0x00000002
#define MP_PORT_HALTING                 0x00000010
#define MP_PORT_IN_RESET                0x00000020

// If any of the flags are set, we cannot send
#define MP_PORT_CANNOT_SEND_MASK        (MP_PORT_PAUSED | MP_PORT_PAUSING | MP_PORT_HALTING | \
                                            MP_PORT_IN_RESET)

// If any of these flags are set, we cannot scan
#define MP_PORT_CANNOT_SCAN_MASK        (MP_PORT_PAUSED | MP_PORT_PAUSING | MP_PORT_HALTING | \
                                            MP_PORT_IN_RESET)
    
#define MP_SET_PORT_STATUS(_Port, _Flag)    \
    MP_SET_FLAG(_Port->Status, _Flag)

#define MP_CLEAR_PORT_STATUS(_Port, _Flag)  \
    MP_CLEAR_FLAG(_Port->Status, _Flag)

#define MP_TEST_PORT_STATUS(_Port, _Flag)   \
    MP_TEST_FLAG(_Port->Status, _Flag)


// port lock manipulation macros
#define MP_ACQUIRE_PORT_LOCK(_Port, _AtDispatch)         \
    NdisAcquireSpinLock(&(_Port->Lock))

#define MP_RELEASE_PORT_LOCK(_Port, _AtDispatch)         \
    NdisReleaseSpinLock(&(_Port->Lock))

#define PORT_GET_VNIC(_Port)    (_Port->VNic)

#define PORT_ACQUIRE_PNP_MUTEX(_Port)    \
    NDIS_WAIT_FOR_MUTEX(&(_Port->ResetPnpMutex))
#define PORT_RELEASE_PNP_MUTEX(_Port)    \
    NDIS_RELEASE_MUTEX(&(_Port->ResetPnpMutex))

// Port refcount relevant for pause/restart
#define PORT_INCREMENT_PNP_REFCOUNT(_Port)\
    MPASSERTOP(NdisInterlockedIncrement(&(_Port->PnpRefCount)) >= 0)

#define PORT_ADD_PNP_REFCOUNT(_Port, _Ref)\
    MPASSERTOP(InterlockedExchangeAdd(&(_Port->PnpRefCount), (LONG)_Ref) >= 0)

#define PORT_DECREMENT_PNP_REFCOUNT(_Port)\
    MPASSERTOP(NdisInterlockedDecrement(&(_Port->PnpRefCount)) >= 0)

// Just a busy wait for PortList refcount to go to 0
#define PORT_WAIT_FOR_PNP_REFCOUNT(_Port) \
    MP_VERIFY_PASSIVE_IRQL();                   \
    while (_Port->PnpRefCount != 0) NdisMSleep(10);




typedef struct _MP_PORT
{
	/**
	 * The handle by which NDIS recognizes this adapter. This handle needs to be passed
	 * in for many of the calls made to NDIS
	 */
	NDIS_HANDLE			        MiniportAdapterHandle;

    MP_PORT_TYPE                PortType;   // Used by Port, ChildPort & MP
    NDIS_PORT_NUMBER            PortNumber; // NDIS allocate port number

	PADAPTER		            Adapter;
	PVNIC                       VNic;       // Used by HVL

    /** Flag maintaining state about the PORT. It is used for synchronization */
    ULONG                       Status;     // PnpFlag Used by HW, Adapter, HVL

    /** Refcount tracking operations that would block Pause/Restart, etc operations */
    LONG                        PnpRefCount;// Used by Port/MP

    /** Lock used to protect the PORT data */
    NDIS_SPIN_LOCK              Lock;       // Used by Port & Child Port

    /** Data for the specific port (eg. for helper port, the scan state, for 
     * ExtSTA port the ExtSTA state machine, etc)
     */
    PVOID                       ChildPort;

    /** Lock to synchronize Reset, Pause and Restart and Op mode change events */
    NDIS_MUTEX                  ResetPnpMutex;

    /** Number of times Pause/Restarts we are processing/have started */
    ULONG                       PauseCount;

    /* Oid Request handler for the port (from NDIS) */
    PORT11_OID_REQUEST_FUNC     RequestHandler;

    /* Direct Oid Request handler for the port (from NDIS) */
    PORT11_OID_REQUEST_FUNC     DirectRequestHandler;
    
    /* Notified when there is a packet to be sent. Invoked from Port11NotifySend*/
    PORT11_SEND_EVENT_FUNC      SendEventHandler;
    
    /* Notified when there is a packet that has been send completed. Invoked
     * from Port11NotifySendComplete */
    PORT11_SEND_COMPLETE_EVENT_FUNC   SendCompleteEventHandler;
    
    /* Notified when there is a packet that has been received. Invoked from
     * Port11NotifyReceive */
    PORT11_RECEIVE_EVENT_FUNC   ReceiveEventHandler;

    /* Notified when there is a packet that has been returned. Invoked from
     * Port11NotifyReturn */
    PORT11_RETURN_EVENT_FUNC    ReturnEventHandler;
    
    /** Current operation mode for the port */
    ULONG                       CurrentOpMode;  

    /** Current operation state for the port */
    MP_PORT_OP_STATE            CurrentOpState;

    /** Current autoconfig setting */
    ULONG                       AutoConfigEnabled;

    /** Currently pending OID request */
    PNDIS_OID_REQUEST           PendingOidRequest;
    
    /** Oid request information for requests that are processed deferred */
    PNDIS_OID_REQUEST           DeferredOidRequest;
    PORT11_GENERIC_CALLBACK_FUNC    DeferredOidHandler;

    // ==> TX RELATED DATA STRUCTURES
    
    /** Lookaside list for TX packet structures */
    NPAGED_LOOKASIDE_LIST       TxPacketLookaside;

    /** Lookaside list for TX fragment structures */
    NPAGED_LOOKASIDE_LIST       TxFragmentLookaside;

    MP_PACKET_QUEUE             PendingTxQueue;

    /** Thread used to push sends down */
    PKTHREAD                    DeferredSendThread;
    /** Trigger for deferred send */
    NDIS_EVENT                  DeferredSendTrigger;

    /** Token that any thread must have before it can dequeue packets to send to the VNIC.
     * The token must be acquired before the sends are dequeued from PendingTxQueue and
     * must be held until the sends have been submitted to the VNIC. If the token cannot
     * be acquired, a thread may trigger the DeferredSendThread to process the sends
     */
    LONG                        SendToken;
    
    // <== TX RELATED DATA STRUCTURES

    // ==> RX RELATED DATA STRUCTURES

    /** Receive packet filter */
    ULONG                       PacketFilter;
    
    /** The Pool for allocating NET_BUFFER_LIST & NET_BUFFER from for receive indication */
    NDIS_HANDLE                 RxNetBufferListPool;
    
    // <== RX RELATED DATA STRUCTURES

} MP_PORT, *PMP_PORT;


