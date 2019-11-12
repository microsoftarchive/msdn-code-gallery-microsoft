/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_defs.h

Abstract:
    Contains miniport layer defines
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

extern NDIS_HANDLE GlobalDriverHandle;
extern NDIS_HANDLE GlobalDriverContext;


#define MP_MAJOR_NDIS_VERSION   6
#define MP_MINOR_NDIS_VERSION   20

// Adapter status flags and manipulation macros
#define MP_ADAPTER_PAUSED       0x00000001
#define MP_ADAPTER_PAUSING      0x00000002
#define MP_ADAPTER_HALTING      0x00000010
#define MP_ADAPTER_IN_RESET     0x00000020
#define MP_ADAPTER_REMOVING     0x00000040
#define MP_ADAPTER_SURPRISE_REMOVED 0x00000100

// Currently if any of the flags are set, we cannot send
#define MP_ADAPTER_CANNOT_SEND_MASK     0xFFFFFFF

#define MP_SET_ADAPTER_STATUS(_Adapter, _Flag)    \
    MP_INTERLOCKED_SET_FLAG(&(_Adapter->Status), _Flag)

#define MP_CLEAR_ADAPTER_STATUS(_Adapter, _Flag)  \
    MP_INTERLOCKED_CLEAR_FLAG(&(_Adapter->Status), _Flag)

#define MP_TEST_ADAPTER_STATUS(_Adapter, _Flag)   \
    MP_TEST_FLAG(_Adapter->Status, _Flag)
    

// Adapter lock manipulation macros
#define MP_ACQUIRE_ADAPTER_LOCK(_Adapter, _AtDispatch)       \
    NdisAcquireSpinLock(&(_Adapter->Lock))

#define MP_RELEASE_ADAPTER_LOCK(_Adapter, _AtDispatch)       \
    NdisReleaseSpinLock(&(_Adapter->Lock))


// Adapter's port refcount 
#define MP_INCREMENT_PORTLIST_REFCOUNT(_Adapter)\
    MPASSERTOP(NdisInterlockedIncrement(&(_Adapter->PortRefCount)) > 0)

#define MP_DECREMENT_PORTLIST_REFCOUNT(_Adapter)\
    MPASSERTOP(NdisInterlockedDecrement(&(_Adapter->PortRefCount)) >= 0)

// Just a busy wait for PortList refcount to go to 0
#define MP_WAIT_FOR_PORTLIST_REFCOUNT(_Adapter) \
    MP_VERIFY_PASSIVE_IRQL();                   \
    while (_Adapter->PortRefCount != 0) NdisMSleep(10);

typedef struct _ADAPTER
{
   
    /**
     * The handle by which NDIS recognizes this adapter. This handle needs to be passed
     * in for many of the calls made to NDIS
     */
    NDIS_HANDLE			        MiniportAdapterHandle;

    /**
     * HW structure associated with this Adapter
     */
    PHW                         Hw;

    /**
     * HVL structure
     */
    PHVL                        Hvl;

    /**
     * The helper port
     */
    PMP_PORT                    HelperPort;

    /**
     * List to hold the ports created on this adapter
     */
    _Field_size_part_(MP_MAX_NUMBER_OF_PORT, NumberOfPorts) 
        MP_PORT*                    PortList[MP_MAX_NUMBER_OF_PORT];    // Public
    ULONG                       NumberOfPorts;                      // Public

    /** Flag maintaining state about the ADAPTER. It is used for synchronization */
    ULONG                       Status;     // PnpFlag;  // Public

    /** Refcount tracking whether any component is using a port. While this is
     * non-zero, the port list cannot be modified */
    LONG                        PortRefCount;                       // Public
    
    /** Lock used to protect the ADAPTER (and HVL) data */
    NDIS_SPIN_LOCK              Lock;                               // Public

    /** Registry settings for port */
    PVOID                       PortRegInfo;
    
    /** Last processed/pending request. Maintained for tracking purpose */
    PNDIS_OID_REQUEST           Tracking_LastRequest;
    NDIS_OID                    Tracking_LastOid;

#if DBG
    NDIS_OID                    Debug_BreakOnOid;
    ULONG                       Debug_BreakOnReset;
#endif

    /** Work item for handling OIDs */
    NDIS_HANDLE                 OidWorkItem;
    /** Oid request information for requests that are processed deferred */
    PNDIS_OID_REQUEST           DeferredOidRequest;

} ADAPTER, *PADAPTER;


