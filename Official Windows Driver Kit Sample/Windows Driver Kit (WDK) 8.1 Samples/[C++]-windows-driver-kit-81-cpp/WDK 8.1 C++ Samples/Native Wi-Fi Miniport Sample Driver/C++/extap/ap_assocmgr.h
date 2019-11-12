/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_assocmgr.h

Abstract:
    ExtAP association definitions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-10-2007    Created
    
Notes:

--*/
#pragma once
    
#ifndef _AP_ASSOCMGR_H
#define _AP_ASSOCMGR_H

/** Forward declarations */
typedef struct _MP_EXTAP_PORT      MP_EXTAP_PORT, *PMP_EXTAP_PORT;

/** Maximum number of stations we will cache state about */
#define AP_STA_MAX_ENTRIES_DEFAULT              32
#define AP_STA_MAX_ENTRIES_MIN                  16
#define AP_STA_MAX_ENTRIES_MAX                  64

/** Time to wait for association request from station after auth success (in number of milliseconds) */
#define AP_ASSOCIATION_REQUEST_TIMEOUT          250

/** Time in which the station has no activity we assume we have lost connectivity to that station (in number of seconds) */
#define AP_NO_ACTIVITY_TIME                     1800
/** The interval of the station inactive timer, in milliseconds */
#define AP_STA_INACTIVE_TIMER_INTERVAL          1000        

#define AP_MAX_AID                              2007
#define AP_INVALID_AID                          0xFFFF
#define AP_AID_TABLE_UNIT_SIZE                  8          
#define AP_AID_TABLE_UNIT_MASK                  0xFF
#define AP_AID_TABLE_SIZE                       (AP_MAX_AID/AP_AID_TABLE_UNIT_SIZE + 1)
#define AP_AID_HEADER                           0xC000


extern DOT11_MAC_ADDRESS Dot11BroadcastAddress;

/** Association Manager state */
typedef enum _AP_ASSOC_MGR_STATE
{
    AP_ASSOC_MGR_STATE_NOT_INITIALIZED = 0,
    AP_ASSOC_MGR_STATE_STARTING,
    AP_ASSOC_MGR_STATE_STARTED,
    AP_ASSOC_MGR_STATE_STOPPING,
    AP_ASSOC_MGR_STATE_STOPPED
} AP_ASSOC_MGR_STATE, *PAP_ASSOC_MGR_STATE;

/**
 * ExtAP association manager
 */
typedef struct _AP_ASSOC_MGR
{
    /** ExtAP port */
    PMP_EXTAP_PORT          ApPort;
    
    /** state of Association Manager */
    AP_ASSOC_MGR_STATE      State;
    
    /**
     * Hash table for the stations
     */
    MAC_HASH_TABLE          MacHashTable;

    /** 
     * Lock we need before we adding/removing entries from the 
     * hash table. This will be acquired for read by
     * routines that are not modifying the table and acquired 
     * for write by routines that will be removing entries or
     * adding entries to the table.
     */
    MP_READ_WRITE_LOCK      MacHashTableLock;

    /** AID table, a bit for each AID */
    UCHAR                   AidTable[AP_AID_TABLE_SIZE];
    
    /** 
     * Association related configurations
     * A lock is NOT required when updating/querying these configurations
     */

    /** SSID that we advertise (we only support one SSID) */
    DOT11_SSID              Ssid;

    /** AP BSSID */
    DOT11_MAC_ADDRESS       Bssid;

    /** Capability information */
    DOT11_CAPABILITY        Capability;
    
    /** Currently enabled authentication algorithm */
    DOT11_AUTH_ALGORITHM    AuthAlgorithm;  

    /** Currently enabled unicast cipher algorithm */
    DOT11_CIPHER_ALGORITHM  UnicastCipherAlgorithm;  

    /** Currently enabled multicast cipher algorithm */
    DOT11_CIPHER_ALGORITHM  MulticastCipherAlgorithm;  

    /** Use default auth cipher algorithms **/
    BOOLEAN bUseDefaultAlgorithms;

    /** Operational rate set */
    DOT11_RATE_SET          OperationalRateSet;

    /** Current setting related to acceptance of unencrypted data */
    BOOLEAN                 ExcludeUnencrypted;
    PDOT11_PRIVACY_EXEMPTION_LIST   PrivacyExemptionList;

    /** Enable WPS */
    BOOLEAN                 EnableWps;

    /** Beacon Mode */
    BOOLEAN                 BeaconEnabled;

    /** Scan related data */
    /** Scan in process */
    LONG                    ScanInProcess;

    /** Local copy of scan request */
    MP_SCAN_REQUEST         ScanRequest;
    
    /** Scan request ID */
    PVOID                   ScanRequestId;
    
    /** 
     * Station inactive timer 
     * When the timer fires, the inactive time of each
     * station is incremented by 1.
     */
    NDIS_HANDLE              StaInactiveTimer;

    /**
     * This is actually the count of the associated
     * stations because each station is going to 
     * increase the counter by 1 when it is associated
     * and decrement by 1 when it is disassociated.
     */
    LONG                    StaInactiveTimerCounter;

    /** For signalling the completion of a synchronous start request */
    NDIS_STATUS             StartBSSCompletionStatus;
    NDIS_EVENT              StartBSSCompletionEvent;
    NDIS_EVENT              StopBSSCompletionEvent;

    NDIS_STATUS             SetChannelCompletionStatus;
    NDIS_EVENT              SetChannelCompletionEvent;
         
} AP_ASSOC_MGR, *PAP_ASSOC_MGR;

/**
 * Station port state
 */
typedef enum _STA_PORT_STATE
{
    STA_PORT_STATE_INVALID = 0,
    STA_PORT_STATE_OPEN,
    STA_PORT_STATE_CLOSED
} STA_PORT_STATE, *PSTA_PORT_STATE;

/**
 * Tracks the state of a station
 */
typedef struct _AP_STA_ENTRY
{
    /** 
     * MAC hash entry.
     * This is used for hash table operations.
     */
    MAC_HASH_ENTRY          MacHashEntry;

    /** 
     * Pointer to the association manager
     * where the station is managed.
     */
    PAP_ASSOC_MGR           AssocMgr;
    
    /** Capability information */
    DOT11_CAPABILITY        CapabilityInformation;

    /** Listen interval */
    USHORT                  ListenInterval;

    /** Supported rates */
    DOT11_RATE_SET          SupportedRateSet;
    
    /** Current association state of the station */
    DOT11_ASSOCIATION_STATE AssocState;

    /** Current association ID */
    USHORT                  Aid;

    /** Power mode */
    DOT11_POWER_MODE        PowerMode;
    
    /** Auth algorithm */
    DOT11_AUTH_ALGORITHM    AuthAlgo;

    /** Unicast cipher algorithm */
    DOT11_CIPHER_ALGORITHM  UnicastCipher;

    /** Multicast cipher algorithm */
    DOT11_CIPHER_ALGORITHM  MulticastCipher;

    /** WPS enabled */
    BOOLEAN                 WpsEnabled;
    
    /** Association timer */
    NDIS_HANDLE             AssocTimer;

    /** Waiting for association request */
    LONG                    WaitingForAssocReq;

    /** 
     * Association up time, i.e. timestamp at which association is completed with success.
     * Timestamp value is returned by NdisGetCurrentSystemTime
     */
    LARGE_INTEGER           AssocUpTime;

    /** Statistics */
    DOT11_PEER_STATISTICS   Statistics;

    /** 
     * Station reference count. 
     * Indicate the number of external functions 
     * that are accessing the station entry.
     * The reference count is 1 when an entry is created.
     * It is deleted when the reference count reaches zero.
     */
    LONG                    RefCount;

#if 0
    /** Buffer for association complete indication */
    PDOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS AssocCompletePara;
    ULONG                   AssocCompleteParaSize;

    /** Received association request */
    PDOT11_MGMT_HEADER      AssocReqFrame;
    USHORT                  AssocReqFrameSize;
#endif
    /** Association decision */
    BOOLEAN                 AcceptAssoc;
    USHORT                  Reason;

    /** 
     * Inactive time. 
     * Indicates how long the station has been inactive, in seconds.
     */
    LONG                    InactiveTime;
    
    /** 
     * Port state
     * This is used to decide whether a non-forced scan
     * shall be allowed or not.
     */
    STA_PORT_STATE          PortState; 
} AP_STA_ENTRY, *PAP_STA_ENTRY;

/** Association Manager Functions invoked by other components */

/**
 * The following functions are defined for upper layer components.
 * Caller must handle synchronization.
 */
 
/** Initialize Association Manager */
NDIS_STATUS
ApInitializeAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PMP_EXTAP_PORT ApPort
    );

/** Deinitialize Association Manager */
VOID
ApDeinitializeAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr
    );

/** Start Association Manager */
NDIS_STATUS
ApStartAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr
    );

/**
 * Stop Association Manager
 * 1. Stop accepting new association requests
 * 2. Disassociate all stations and send corresponding indications
 * 3. Doesn't have to wait for the pending association request decision from IM
 * 4. Cancel on-going scan and wait for it to complete
 */
VOID
ApStopAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr
    );

/** Restart Association Manager */
NDIS_STATUS
ApRestartAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr
    );

/**
 * Pause Association Manager
 * 1. Cancel on-going scan and wait for it to complete
 */
NDIS_STATUS
ApPauseAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr
    );

/** Internal functions called by other association manager functions */

/**
 * Station entry related functions
 */

/**
 * Allocate a station entry
 */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmAllocateStaEntry(
    _In_ const PAP_ASSOC_MGR AssocMgr,
    _In_ const DOT11_MAC_ADDRESS * StaMacAddr,
    _Outptr_ PAP_STA_ENTRY * StaEntry
    );

/**
 * Free a station entry
 */
VOID
FORCEINLINE
AmFreeStaEntry(
    _In_ PAP_STA_ENTRY StaEntry
    )
{
    if (StaEntry->AssocTimer)
    {
        NdisFreeTimerObject(StaEntry->AssocTimer);
        StaEntry->AssocTimer = NULL;
    }
    
    MP_FREE_MEMORY(StaEntry);
}

/** 
 * Reference a station entry 
 * Must be called when the station is still in the MAC table.
 * The caller must synchronize the access.
 */
LONG
FORCEINLINE
ApRefSta(
    _In_ PAP_STA_ENTRY StaEntry
    )
{
    return InterlockedIncrement(&StaEntry->RefCount);
}

/** 
 * Dereference a station entry 
 * Can be called anywhere. The caller must ensure the reference count is 
 * greater than 0.
 */
LONG
FORCEINLINE
ApDerefSta(
    _In_ PAP_STA_ENTRY StaEntry
    )
{
    LONG refCount = InterlockedDecrement(&StaEntry->RefCount);

    if (0 == refCount)
    {
        AmFreeStaEntry(StaEntry);
    }

    return refCount;
}

/** 
 * Get scan state.
 */
LONG
FORCEINLINE
ApGetScanState(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    return InterlockedExchangeAdd(
                &AssocMgr->ScanInProcess,
                0
                );
}

/** 
 * Set scan state.
 */
LONG
FORCEINLINE
ApSetScanState(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ LONG NewScanState
    )
{
    return InterlockedExchange(
                &AssocMgr->ScanInProcess,
                NewScanState
                );
}

/** 
 * Get the association state of a station.
 */
DOT11_ASSOCIATION_STATE
FORCEINLINE
ApGetStaAssocState(
    _In_ PAP_STA_ENTRY StaEntry
    )
{
    return (DOT11_ASSOCIATION_STATE)InterlockedExchangeAdd(
                (LONG *)&StaEntry->AssocState,
                0
                );
}

/** 
 * Set the association state of a station.
 */
DOT11_ASSOCIATION_STATE
FORCEINLINE
ApSetStaAssocState(
    _In_ PAP_STA_ENTRY StaEntry,
    _In_ DOT11_ASSOCIATION_STATE NewAssocState
    )
{
    return (DOT11_ASSOCIATION_STATE)InterlockedExchange(
                (LONG *)&StaEntry->AssocState,
                (LONG)NewAssocState
                );
}

/** 
 * Get the port state of a station.
 */
STA_PORT_STATE
FORCEINLINE
ApGetStaPortState(
    _In_ PAP_STA_ENTRY StaEntry
    )
{
    return (STA_PORT_STATE)InterlockedExchangeAdd(
                (LONG *)&StaEntry->PortState,
                0
                );
}

/** 
 * Set the port state of a station.
 */
STA_PORT_STATE
FORCEINLINE
ApSetStaPortState(
    _In_ PAP_STA_ENTRY StaEntry,
    _In_ STA_PORT_STATE NewPortState
    )
{
    return (STA_PORT_STATE)InterlockedExchange(
                (LONG *)&StaEntry->PortState,
                (LONG)NewPortState
                );
}

/**
 * Reset station inactive time.
 * Return the original inactive time.
 */
LONG
FORCEINLINE
ApResetStaInactiveTime(
    _In_ PAP_STA_ENTRY StaEntry
    )
{
    return InterlockedExchange(
            &StaEntry->InactiveTime,
            0
            );
}

/**
 * Increment station inactive time by 1.
 * Return the original inactive time.
 */
LONG
FORCEINLINE
ApIncrementStaInactiveTime(
    _In_ PAP_STA_ENTRY StaEntry
    )
{
    return InterlockedIncrement(
            &StaEntry->InactiveTime
            );
}

/** 
 * Check and set the association state of a station.
 * The new state is set only if the old state matches
 * the given state.
 */
DOT11_ASSOCIATION_STATE
FORCEINLINE
ApCheckAndSetStaAssocState(
    _In_ PAP_STA_ENTRY StaEntry,
    _In_ DOT11_ASSOCIATION_STATE NewAssocState,
    _In_ DOT11_ASSOCIATION_STATE OldAssocState
    )
{
    return (DOT11_ASSOCIATION_STATE)InterlockedCompareExchange(
                (LONG *)&StaEntry->AssocState,
                (LONG)NewAssocState,
                (LONG)OldAssocState
                );
}

/**
 * Association manager related functions
 */

/** 
 * Set association manager to its default based on the hardware capability 
 * and registry settings
 */
VOID
AmSetDefault(
    _In_ PAP_ASSOC_MGR AssocMgr
    );

/** 
 * Set association manager to its defaults after a Reset OID
 */
VOID
AmRestoreDefault(
    _In_ PAP_ASSOC_MGR AssocMgr
    );

/**
 * Set default cipher based on the auth algorithm and hardware capability
 */
VOID
AmSetDefaultCipher(
    _In_ PAP_ASSOC_MGR AssocMgr
    );

/**
 * Set default auth and cipher algorithms in VNIC
 */
VOID
AmSetDefaultAuthAndCipherAlgorithms
(
    _In_ PAP_ASSOC_MGR AssocMgr
    );


/** 
 * Clean up Association Manager Privacy Exemption List
 */
VOID
AmCleanupPrivacyExemptionList(
    _In_ PAP_ASSOC_MGR AssocMgr
    );


/** 
 * Clean up Association Manager Desired SSID List
 */
VOID
AmCleanupDesiredSsidList(
    _In_ PAP_ASSOC_MGR AssocMgr
    );


/** 
 * Clean up Association Manager 
 * Shall be called after ApStopAssocMgr()
 * All clients must have been disassociated at this point
 */
VOID
AmCleanup(
    _In_ PAP_ASSOC_MGR AssocMgr
    );

/**
 * Internal functions for OIDs
 */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmQuerySsid(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_SSID_LIST SsidList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    );

NDIS_STATUS
AmSetSsid(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_SSID_LIST SsidList
    );

VOID
FORCEINLINE    
AmQueryBssid(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ DOT11_MAC_ADDRESS * Bssid
    )
{
    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);
    
    RtlCopyMemory(
            *Bssid,
            AssocMgr->Bssid,
            sizeof(DOT11_MAC_ADDRESS)
            );
}

NDIS_STATUS
AmSetBssid(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ DOT11_MAC_ADDRESS * Bssid
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmQueryAuthAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_AUTH_ALGORITHM_LIST EnabledAuthenticationAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    );

NDIS_STATUS
AmSetAuthAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_AUTH_ALGORITHM_LIST EnabledAuthenticationAlgorithm
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmQueryUnicastCipherAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_CIPHER_ALGORITHM_LIST EnabledUnicastCipherAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    );

NDIS_STATUS
AmSetUnicastCipherAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_CIPHER_ALGORITHM_LIST EnabledUnicastCipherAlgorithm
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmQueryMulticastCipherAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_CIPHER_ALGORITHM_LIST EnabledMulticastCipherAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    );

NDIS_STATUS
AmSetMulticastCipherAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_CIPHER_ALGORITHM_LIST EnabledMulticastCipherAlgorithm
    );

VOID
FORCEINLINE
AmQueryOperationalRateSet(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_RATE_SET OperationalRateSet
    )
{
    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);
    
    RtlCopyMemory(
            OperationalRateSet,
            &AssocMgr->OperationalRateSet,
            sizeof(DOT11_RATE_SET)
            );
}

NDIS_STATUS
AmSetOperationalRateSet(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_RATE_SET OperationalRateSet
    );

VOID
FORCEINLINE
AmQueryExcludeUnencrypted(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ BOOLEAN * ExcludeUnencrypted
    )
{
    *ExcludeUnencrypted = AssocMgr->ExcludeUnencrypted;
}

NDIS_STATUS
AmSetExcludeUnencrypted(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ BOOLEAN ExcludeUnencrypted
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmQueryPrivacyExemptionList(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_PRIVACY_EXEMPTION_LIST PrivacyExemptionList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    );

NDIS_STATUS
AmSetPrivacyExemptionList(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_PRIVACY_EXEMPTION_LIST PrivacyExemptionList
    );

VOID
FORCEINLINE    
AmQueryWpsEnabled(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ BOOLEAN * WpsEnabled
    )
{
    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);
    
    *WpsEnabled = AssocMgr->EnableWps;
}

NDIS_STATUS
AmSetWpsEnabled(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ BOOLEAN WpsEnabled
    );


VOID
FORCEINLINE    
AmQueryApBeaconMode(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ BOOLEAN * BeaconEnabled
    )
{
    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);
    
    *BeaconEnabled = AssocMgr->BeaconEnabled;
}

NDIS_STATUS
AmSetApBeaconMode(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ BOOLEAN BeaconEnabled
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmEnumPeerInfo(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_PEER_INFO_LIST PeerInfo,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    );

NDIS_STATUS
AmDisassociatePeerRequest(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_MAC_ADDRESS PeerMacAddr,
    _In_ USHORT Reason
    );

NDIS_STATUS
AmSetStaPortState(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_MAC_ADDRESS PeerMacAddr,
    _In_ BOOLEAN PortOpen
    );

NDIS_STATUS
AmScanRequest(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PVOID ScanRequestId,
    _In_ PDOT11_SCAN_REQUEST_V2 ScanRequest,
    _In_ ULONG ScanRequestBufferLength
    );

/*
 * This function is called inside a direct call.
 * The caller must ensure the AP port stays valid.
 * The function must handle potential concurrent accesses
 * to shared resources.
 */
NDIS_STATUS
AmAssociationDecision(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_INCOMING_ASSOC_DECISION AssocDecision
    );

/*
 * Process the association decision made by OS.
 */
NDIS_STATUS
AmProcessAssociationDecision(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ BOOLEAN reAssociation,
    _In_reads_bytes_(AssocReqFrameSize) PDOT11_MGMT_HEADER AssocReqFrame,
    _In_ USHORT AssocReqFrameSize,
    _In_ PDOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS AssocCompletePara,
    _In_ ULONG AssocCompleteParaSize
    );

/**
 * Internal functions for station management
 */

/**
 * Processing station authentication frame
 */
VOID 
AmProcessStaAuthentication(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(PacketSize) PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    );

/**
 * Processing station deauthentication frame
 */
VOID 
AmProcessStaDeauthentication(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(PacketSize) PDOT11_MGMT_HEADER MgmtPacket,
    _In_ _In_range_(DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_DEAUTH_FRAME), USHORT_MAX) USHORT PacketSize
    );

/**
 * Processing station association request frame
 */
VOID 
AmProcessStaAssociation(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ BOOLEAN reAssociation,
    _In_reads_bytes_(PacketSize) PDOT11_MGMT_HEADER MgmtPacket,
    _In_ _In_range_(DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_REASSOC_REQUEST_FRAME), USHORT_MAX)USHORT PacketSize
    );


/**
 * Processing station disassociation frame
 */
VOID 
AmProcessStaDisassociation(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(PacketSize) PDOT11_MGMT_HEADER MgmtPacket,
    _In_ _In_range_(DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_DISASSOC_FRAME), USHORT_MAX) USHORT PacketSize
    );

/**
 * Association related management frames
 */

/** Create authentication response frame */
NDIS_STATUS
AmCreateAuthRespFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_opt_ PAP_STA_ENTRY StaEntry,
    _In_ PDOT11_MGMT_HEADER ReceivedMgmtPacket,
    _In_ USHORT ReceivedPacketSize,
    _In_ USHORT StatusCode,
    _Outptr_result_bytebuffer_(*AuthRespFrameSize) PUCHAR * AuthRespFrame,
    _Out_ PUSHORT AuthRespFrameSize
    );
    
/** Create association/reassociation response frame */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmCreateAssocRespFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PAP_STA_ENTRY StaEntry,
    _In_ BOOLEAN reAssociation,
    _In_ PDOT11_MGMT_HEADER ReceivedMgmtPacket,
    _In_ USHORT ReceivedPacketSize,
    _In_ USHORT StatusCode,
    _Outptr_result_bytebuffer_(*AssocRespFrameSize) PUCHAR * AssocRespFrame,
    _Out_ PUSHORT AssocRespFrameSize
    );

/** Create disassociation frame */
NDIS_STATUS
AmCreateDisassocFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS DestAddr,
    _In_ USHORT ReasonCode,
    _Outptr_result_bytebuffer_(*DisassocFrameSize) PUCHAR * DisassocFrame,
    _Out_ PUSHORT DisassocFrameSize
    );

/** Create deauthentication frame */
NDIS_STATUS
AmCreateDeauthFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS DestAddr,
    _In_ USHORT ReasonCode,
    _Outptr_result_bytebuffer_(*DeauthFrameSize) PUCHAR * DeauthFrame,
    _Out_ PUSHORT DeauthFrameSize
    );

/** 
 * Send disassociation frame. 
 * Shall not invoke it in a lock.
 */
NDIS_STATUS
AmSendDisassociationFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS DestAddr,
    _In_ USHORT ReasonCode
    );

/** 
 * Send deauthentication frame. 
 * Shall not invoke it in a lock.
 */
NDIS_STATUS
AmSendDeauthenticationFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS DestAddr,
    _In_ USHORT ReasonCode
    );

/** 
 * Disassociate a station. 
 * The caller must ensure the station entry is valid during this call.
 * The caller should not hold any lock.
 */
VOID
AmDisassociateSta(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PAP_STA_ENTRY StaEntry,
    _In_ ULONG Reason
    );

/** 
 * Deauthenicate a station. 
 * This shall be called only after a station 
 * is authenticated.
 * The station entry must be removed from the
 * Mac table already.
 * So this function will be invoked only once
 * for each station entry.
 */
VOID
AmDeauthenticateSta(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PAP_STA_ENTRY StaEntry,
    _In_ ULONG Reason,
    _In_ BOOLEAN SendDeauthFrame,
    _In_ USHORT Dot11Reason 
    );

/** Supporting functions for NDIS indications */

/** Allocate association completion parameters */
NDIS_STATUS
AmAllocAssocCompletePara(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ USHORT AssocReqFrameSize,
    _In_ USHORT AssocRespFrameSize,
    _Outptr_ PDOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS * AssocCompletePara,
    _Out_ PULONG AssocCompleteParaSize
    );

/** Allocate  association request received parameters */
NDIS_STATUS
AmAllocAssocReqPara(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ USHORT AssocReqFrameSize,
    _Outptr_ PDOT11_INCOMING_ASSOC_REQUEST_RECEIVED_PARAMETERS * AssocReqPara,
    _Out_ PULONG AssocReqParaSize
    );

/**
 * Prepare information for association start indication.
 * The buffer is allocated by the caller.
 */
VOID
AmPrepareAssocStartPara(
    _In_ PDOT11_MAC_ADDRESS StaMacAddr,
    _Out_ PDOT11_INCOMING_ASSOC_STARTED_PARAMETERS AssocStartPara
    );

/**
 * Prepare information for association request received indication.
 * The buffer is allocated by the caller.
 * The caller must make sure the buffer is big enough to hold all data.
 */
VOID
AmPrepareAssocReqPara(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS StaMacAddr,
    _In_ BOOLEAN Reassociation,
    _In_reads_bytes_(RequestFrameSize) PUCHAR RequestFrame,
    _In_ USHORT RequestFrameSize,
    _Out_ PDOT11_INCOMING_ASSOC_REQUEST_RECEIVED_PARAMETERS AssocReqPara,
    _Inout_ PULONG AssocReqParaSize
    );

/**
 * Prepare information for association complete indication.
 * The buffer is allocated by the caller.
 * The caller must make sure the buffer is big enough to hold all data.
 */
VOID
AmPrepareAssocCompletePara(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_opt_ PAP_STA_ENTRY StaEntry,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS StaMacAddr,
    _In_ ULONG Status,
    _In_ UCHAR ErrorSource,
    _In_ BOOLEAN Reassociation,
    _In_reads_bytes_opt_(RequestFrameSize) PUCHAR RequestFrame,
    _In_ USHORT RequestFrameSize,
    _In_reads_bytes_opt_(ResponseFrameSize) PUCHAR ResponseFrame,
    _In_ USHORT ResponseFrameSize,
    _Out_ PDOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS AssocCompletePara,
    _Inout_ PULONG AssocCompleteParaSize
    );

/**
 * Prepare information for disassociation indication.
 * The buffer is allocated by the caller.
 */
VOID
AmPrepareDisassocPara(
    _In_ PDOT11_MAC_ADDRESS StaMacAddr,
    _In_ ULONG Reason,
    _Out_ PDOT11_DISASSOCIATION_PARAMETERS DisassocPara
    );

/**
 * Timer related functions
 */

/**
 * Start station inactive timer
 * This function first increments the counter.
 * If the counter is 1, it sets the timer.
 */
VOID
FORCEINLINE
AmStartStaInactiveTimer(
    _In_ PAP_ASSOC_MGR AssocMgr
)
{
    LARGE_INTEGER               fireTime;
    if (InterlockedIncrement(&AssocMgr->StaInactiveTimerCounter) == 1)
    {
        //
        // This is the first one to start the timer.
        // Start the periodic timer
        //
        fireTime.QuadPart = Int32x32To64((LONG)AP_STA_INACTIVE_TIMER_INTERVAL, -10000);
        NdisSetTimerObject(
            AssocMgr->StaInactiveTimer,
            fireTime,
            AP_STA_INACTIVE_TIMER_INTERVAL, 
            NULL
            );
    }
}

/**
 * Stop station inactive timer
 * This function first decrements the counter.
 * If the counter is zero, it cancels the timer.
 */
VOID
FORCEINLINE
AmStopStaInactiveTimer(
    _In_ PAP_ASSOC_MGR AssocMgr
)
{
    LARGE_INTEGER  delayedFireTime;
    
    if (InterlockedDecrement(&AssocMgr->StaInactiveTimerCounter) == 0)
    {
        //
        // This is the last one to stop the timer.
        // Reschedule the periodic timer to not run for a long time while
        //
        delayedFireTime.QuadPart = Int32x32To64((LONG)MAXLONG, -10000);
        NdisSetTimerObject(
            AssocMgr->StaInactiveTimer,
            delayedFireTime,
            0, 
            NULL
            );        
    }
}

/**
 * Timeout callback for the station inactive timer.
 *
 * \param param PAP_ASSOC_MGR
 */
NDIS_TIMER_FUNCTION AmStaInactiveTimeoutCallback;

#define STA_WAITING_FOR_ASSOC_REQ       1
#define STA_NOT_WAITING_FOR_ASSOC_REQ   0

/**
 * Start the association timer for a station.
 */
VOID
FORCEINLINE
AmStartStaAssocTimer(
    PAP_STA_ENTRY StaEntry
    )
{
    LARGE_INTEGER fireTime;

    //
    // Reference the station first
    //
    ApRefSta(StaEntry);

    //
    // Set the waiting for association request flag
    //
    InterlockedExchange(
        &StaEntry->WaitingForAssocReq,
        STA_WAITING_FOR_ASSOC_REQ
        );
    
    //
    // Start timer
    //
    fireTime.QuadPart = Int32x32To64((LONG)AP_ASSOCIATION_REQUEST_TIMEOUT, -10000);
    NdisSetTimerObject(StaEntry->AssocTimer, 
        fireTime, 
        0, 
        NULL
        );
}

/**
 * Stop the association timer for a station.
 * It is a no-op if the timer is not started.
 */
VOID
FORCEINLINE
AmStopStaAssocTimer(
    PAP_STA_ENTRY StaEntry
    )
{
    BOOLEAN timerCancelled = FALSE;
    
    //
    // Clear the waiting for association request flag
    //
    if (InterlockedExchange(
            &StaEntry->WaitingForAssocReq,
            STA_NOT_WAITING_FOR_ASSOC_REQ
            ) == STA_WAITING_FOR_ASSOC_REQ)
    {
        //
        // Stop periodic timer
        //
        timerCancelled = NdisCancelTimerObject(StaEntry->AssocTimer);
        if (timerCancelled)
        {
            //
            // Need to deref the station
            //
            ApDerefSta(StaEntry);
        }
    }
}

/**
 * Timeout callback for the association timer.
 * The ref count of the station must be incremented
 * when the timer is scheduled.
 *
 * \param param PAP_STA_ENTRY
 */
NDIS_TIMER_FUNCTION AmStaAssocTimeoutCallback;

#endif // _AP_ASSOCMGR_H


