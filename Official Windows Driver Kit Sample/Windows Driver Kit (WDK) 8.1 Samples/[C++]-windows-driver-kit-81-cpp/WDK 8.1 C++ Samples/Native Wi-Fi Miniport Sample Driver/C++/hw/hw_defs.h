/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_defs.h

Abstract:
    Contains hw layer defines
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

#include "TxPacketQ.h"

/** The maximum number of packets we will attempt to reassemble in 
  * parallel before dropping the oldest one. Typical number of associations
  * on a typical AP are 25 and 32 will cover such scenarios nicely. If the
  * Reassembly line size needs to be increased, change the value here.
  */
#define HW_MAX_REASSEMBLY_LINE_SIZE             32


/** Maximum number of MAC states maintained by the HW */
#define HW_MAX_NUMBER_OF_MAC                    3



#define HW_HELPER_PORT_MAC_INDEX                0
#define HW_DEFAULT_PORT_MAC_INDEX               1

#define HW_KEY_MAPPING_KEY_TABLE_SIZE           (HW11_KEY_TABLE_SIZE - DOT11_MAX_NUM_DEFAULT_KEY)
#define HW_PER_STA_KEY_TABLE_SIZE               HW_KEY_MAPPING_KEY_TABLE_SIZE

#define HW_MAX_NUM_DOT11_REG_DOMAINS_VALUE      10
#define HW_MAX_NUM_DIVERSITY_SELECTION_RX_LIST  256

// Overhead of any encrypted packet due to IV. Minimum is 4 for WEP
#define HW_ENCRYPTED_MPDU_MIN_OVERHEAD          4

/*
 * The number of tick intervals in unit of CheckForHang Interval for which the sends have
 * to be stalled before we will ask Ndis to reset the NIC. Since the algorithm is an approximate
 * one, and not synchronized, the actual period might take an extra tick. This, however, should
 * not cause any issues as long as the value is not too big.
 */
#define HW_SENDS_HAVE_STALLED_PERIOD            4

#define HW_MAX_BEACON_PERIOD                    1024
#define HW_LIMIT_BEACON_PERIOD(_Period)		\
	(((_Period) >= HW_MAX_BEACON_PERIOD) ? HW_MAX_BEACON_PERIOD - 1 : _Period)

/** This macro converts the time provided in TU units to Milliseconds */
#define HW_TU_TO_MS(_TimeInTU_)     ((_TimeInTU_ * 1024) / 1000)


#define HW_DUPLICATE_DETECTION_CACHE_LENGTH     8

// 
// If link quality of the AP we are associated with is below this value,
// we will use the lower data rate for communicating with this AP
//
#define HW_LOW_RATE_LINK_QUALITY_THRESHOLD      40 
//
// The lower data rate we would select would be some value below this
// max (IEEE format = 12 Mbps)
//
#define HW_LOW_RATE_MAX_DATA_RATE                24



// Generic callback function for the HW. This can be called into
// by any layer (HW/HVL/Port). The Data parameter is only guaranteed to be
// valid until the callback returns. If the callback function wants access to it later,
// it must copy it locally
typedef NDIS_STATUS (*HW_GENERIC_CALLBACK_FUNC)(
    _In_  PHW                     Hw,
    _In_  PHW_MAC_CONTEXT         MacContext,
    _In_  PVOID                   Data
    );

/**
 * This is information that is loaded from the registry
 */
typedef struct _NIC_REG_INFO {

    /** Override MAC address that has been read from the registry */
	UCHAR					    OverrideAddress[DOT11_ADDRESS_SIZE];
	
	BOOLEAN                     AddressOverrideEnabled;

    /** 
     * Number of RX MPDUs for the receive queue 
     */
    ULONG                       NumRXBuffers;

    /**
     * Maximum number of RX MPDUS to allocate
     */
    ULONG                       NumRXBuffersUpperLimit;
    /**
     * Minimum number of RX MPDUS to allocate
     */
    ULONG                       NumRXBuffersLowerBase;

    /** 
     * Number of TX MSDUs for data queues. For other queues, this is decided 
     * by the hardware layer 
     */
    ULONG                       NumTXBuffers;

    /**
     * Maximum TX data rate. This is in IEEE format
     */
    ULONG                       MaximumTxRate;

    /**
     * When non-zero, enables support for multiple MAC addresses
     */
    ULONG                       MultipleMacAddressEnabled;

    /**
     * Number of packets to process in each RX interrupt
     */
    ULONG                       MaxRxPacketsToProcess;
    
    /**
     * For rate adaptation
     */
    ULONG                       MinPacketsSentForTxRateUpdate;
    ULONG                       TxFailureThresholdForRateFallback;
    ULONG                       TxFailureThresholdForRateIncrease;
    ULONG                       TxFailureThresholdForRoam;
    ULONG                       TxDataRateFallbackSkipLevel;


    ULONG                       BeaconPeriod;
    ULONG                       RTSThreshold;
    ULONG                       FragmentationThreshold; // We dont do fragmentation, so this is not used

} NIC_REG_INFO, *PNIC_REG_INFO;

typedef struct _HW_DUPE_CACHE_ENTRY {
    DOT11_MAC_ADDRESS           Address2;
    UCHAR                       ReceivedGoodMPDU;
    USHORT                      SequenceControl;
} HW_DUPE_CACHE_ENTRY, *PHW_DUPE_CACHE_ENTRY;


typedef struct _NIC_RX_INFO 
{
    /** Lookaside list for RX MSDU structures */
    NPAGED_LOOKASIDE_LIST               RxPacketLookaside;

    /** Lookaside list for RX MPDU structures */
    NPAGED_LOOKASIDE_LIST               RxFragmentLookaside;

    /** Total number of TX MPDU allocated. This is the sum of MPDUs indicated up,
     * plus the MPDUs that are with hardware waiting for receives (NumMPDUAvailable)
     * plus the number of MPDUs in the Unused list (NumMPDUUnused). This value
     * should be between MaxMPDUNum and MinMPDUNum.
     */
    LONG                                NumMPDUAllocated;

    /** Number of MPDU currently available to the hardware for receive */
    LONG                                NumMPDUAvailable;

    /** This is the list of MPDUs that have been submitted to the hardware */
    LIST_ENTRY                          AvailableMPDUList;
    
    //
    // Start of fields related to reassembling fragments 
    //
    /** The total number of Rx MSDUs in the Reassembly line */
    ULONG                               TotalRxMSDUInReassembly;

    /** An array to hold Rx MSDUs that are currently in Reassembly */
    PHW_RX_MSDU                         ReassemblyLine[HW_MAX_REASSEMBLY_LINE_SIZE];

    /** This is the Reassembly Rx MSDU for which we most recently received a fragment */
    PHW_RX_MSDU                         MRUReassemblyRxMSDU;
    
    /*
     * Count down to when we will run through the ReassemblyLine and clear
     * out any expired Rx MSDUs. This is a periodic cleanup mechanism. The
     * unit is the number of interrupts in which we have indicated up
     * frame(s) to the OS
     */
    ULONG                               ReassemblyLineCleanupCountdown;
    // End of fields related to reassembling fragments 

    //
    // Start of fields related to allocation of extra MPDUs when we run low on resources
    //

    /**
     * Linked list of MPDUs that we have to allocated but not give to the hardware
     */
    LIST_ENTRY                          UnusedMPDUList;

    /** The number of Rx MPDU that are in the unused list above */
    LONG                                NumMPDUUnused;
    
    /** 
     * The number of 2 second intervals that have passed since sampling of Rx MPDU list started. This
     * is used to handle cleanup of extra RX MPDUs
     */
    ULONG                               UnusedMPDUListSampleTicks;

    /** The sum of number of Rx MSDU that have been found free in each sampling */
    ULONG                               UnusedMPDUListSampleCount;
    
    // End of fields related to allocation of extra MPDUs when we run low on resources


    //
    // Start of fields  related to duplicate detection
    //
	HW_DUPE_CACHE_ENTRY                 DupePacketCache[2 * HW_DUPLICATE_DETECTION_CACHE_LENGTH];
	UCHAR                               NextDupeCacheIndexData;         // Next index to put entry in for data packets
	UCHAR                               NextDupeCacheIndexOther;        // Next index to put entry in for other packets

    // End of fields related to duplicate detection

#if DBG
    // Stuff we use for debugging purpose
    ULONG                       Debug_BreakOnReceiveCount;
    DOT11_MAC_ADDRESS           Debug_BreakOnReceiveDestinationAddress;
    DOT11_MAC_ADDRESS           Debug_BreakOnReceiveSourceAddress;
    UCHAR                       Debug_BreakOnReceiveType;
    UCHAR                       Debug_BreakOnReceiveSubType;
    BOOLEAN                     Debug_BreakOnReceiveMatchSource;
#endif
        
}NIC_RX_INFO, *PNIC_RX_INFO;


/**
 * This structure holds state for each send queue in the HW layer
 */
typedef struct _HW_TX_QUEUE
{
    /**
     * This is the HAL queue that would be used for sending these packets. The queue type
     * gives an indication to the HAL on what the queue is used for
     */
    HAL_TX_QUEUE_TYPE                   HalQueueType;

    /**
     * This is the HAL queue index that would be used for sending these packets. This
     * identifies the index of the queue for the HAL
     */
    ULONG                               HalQueueIndex;
    
    /**
     * Data structure to queue Packets to be transmitted in. We will queue 
     * when we are running low on HW_TX_MSDUs. This is queued at the entry
     * point of the HW layer
     */
    MP_PACKET_QUEUE                     PendingTxQueue;

    /**
     * Sets the limit on the maximum number of packets that can be queue
     * in the pending queue
     */
    ULONG                               MaxPendingTx;

    /**
     * Array of TX MSDU. A TX MSDU needs to be reserved
     * before the packet can be sent
     */
    HW_TX_MSDU*                         MSDUArray;

    /** Total number of TX MSDU allocated for this queue. Eventhough we 
     * have allocated so many, we would leave 1 empty
     */
    ULONG                               NumMSDUAllocated;

    /** Number of MSDU currently vailable for send */
    LONG                                NumMSDUAvailable;

    /** The Tx MSDU that will be used for preparing the next send */
    LONG                                NextToReserve;
    
    /** The Tx MSDU that will be send completed next */
    LONG                                NextToComplete;

    /** The Tx MSDU that is to be sent out (ready to go) */
    LONG                                NextToSend;

    /** Number of sends that have been submitted to the HAL but not completed */
    ULONG                               NumPending;

    /** Snapshot of previous NextToComplete value */
    LONG                                StalledSendNextToCompleteSnapshot;

    /** Number of ticks since the NextToComplete value has not changed */
    ULONG                               StalledSendTicks;

    /** For tracking */
    MP_DATA_RECORDER                    Tracking_SendRecorder;
}HW_TX_QUEUE, *PHW_TX_QUEUE;


typedef struct _NIC_TX_INFO 
{
    /** Size of the scatter gather list */
    ULONG                               ScatterGatherListSize;

    /** Scatter Gather list buffers pre-allocated for sending. This is split amongst
     * the HW_TX_MSDUs
     */
    PUCHAR                              ScatterGatherListBuffers;

    /** Lookaside list for HW_TX_MPDU structures */
    NPAGED_LOOKASIDE_LIST               TxMPDULookaside;

    /** Lookaside list for MP_TX_MSDU structures */
    NPAGED_LOOKASIDE_LIST               TxPacketLookaside;

    /** Lookaside list for MP_TX_MPDU structures */
    NPAGED_LOOKASIDE_LIST               TxFragmentLookaside;

    /** The queues that we maintain */
    HW_TX_QUEUE                         TxQueue[HW11_NUM_TX_QUEUE];
    
}NIC_TX_INFO, *PNIC_TX_INFO;

//
// If we want to send multiple probes per channel, we can add an
// extra state e.g. ScanStepPerformScan_Pass2 and it will happen
//
typedef enum _NIC_SCAN_STEP
{
	ScanStepSwitchChannel = 0,
	ScanStepPerformScan_Pass1,
	ScanStepMax
} NIC_SCAN_STEP;


/**
 * Maintains the scan state for the hardware
 */
typedef struct _NIC_SCAN_CONTEXT {

    /** The MAC context that is currently scanning */
    PHW_MAC_CONTEXT             MacContext;

    /** Cached copy of the scan request */
    PMP_SCAN_REQUEST            ScanRequest;

	BOOLEAN                     CancelOperation;          // indicates if scan should be stopped
	BOOLEAN						ScanInProgress;
	volatile BOOLEAN            CompleteIndicated;


	NDIS_HANDLE                 Timer_Scan;               // timer for survey scan
	
    ULONG                       ProbeDelay;
    USHORT                      ProbeTXRate;
    ULONG                       ChannelCount;
    PULONG                      ChannelList;
    ULONG                       ChannelTime;
    ULONG                       CurrentChannelIndex;
    ULONG                       CurrentChannel;
    ULONG                       ScanPhyId;
    ULONGLONG                   ChannelSwitchTime;

    NIC_SCAN_STEP               CurrentStep;

    ULONG                       PreScanPhyId;
    UCHAR                       PreScanChannel;
} NIC_SCAN_CONTEXT, *PNIC_SCAN_CONTEXT;


/**
 * This is PHY context that saves PHY state for each context
 */
typedef struct _HW_PHY_CONTEXT
{
    /** 
     * The channel or frequency that should be programmed on the hardware 
     * for this PHY (if this phy is the operating phy)
     */
    UCHAR                       Channel;

    /**
     * The operational rate set for this PHY
     */
    DOT11_RATE_SET              OperationalRateSet;

    /**
     * The rate set that we are permitted to use. Note that we are using one 
     * global value for all our peers.
     */
    DOT11_RATE_SET              ActiveRateSet;
    
}HW_PHY_CONTEXT, *PHW_PHY_CONTEXT;


/**
 * The key information for a MAC_CONTEXT/HW
 */
typedef struct _HW_KEY_ENTRY
{
    /** The actual key data that gets pushed onto the hardware */
    NICKEY                      Key;

    /** The index of this key in the hardware key table */
    UCHAR                       NicKeyIndex;

    /** Index of this key as set on the peer */
    UCHAR                       PeerKeyIndex;

    /** MAC context that owns this key */
    PHW_MAC_CONTEXT             MacContext;
    
    /** When set, we would use software encryption/decryption */
    BOOLEAN                     SoftwareOnly;

    /** CNG Key Handle */
    BCRYPT_KEY_HANDLE           hCNGKey;

    /** Memory used by CNG */
    PVOID                       CNGKeyObject;

    /** Fields used for TX encryption */
    union {
        struct {
            ULONGLONG           PN:48;                  // for CCMP
            ULONGLONG           PN_unused:16;
        };
        struct {
            ULONGLONG           TSC:48;                 // for TKIP
            ULONGLONG           TSC_unused:16;
        };
        struct {
            ULONG               IV:24;                  // for WEP 
            ULONG               IV_unused:8;
        };
    };

    /** Fields used for RX decryption */
    struct {
        ULONGLONG               ReplayCounter:48;       // for CCMP or TKIP
        ULONGLONG               ReplayCounter_unused:16;
    };    
}HW_KEY_ENTRY, *PHW_KEY_ENTRY;


/**
 * This is information about all the peers of the node
 */
typedef struct _HW_PEER_NODE
{
    /** MAC address of this peer node */
    DOT11_MAC_ADDRESS           PeerMac;

    /** Entry is valid */
    BOOLEAN                     Valid:1;

    USHORT                      CapabilityInfo;
    USHORT                      AssociateId;
    
    /** This is the table for keys maintained in software for this peer. These
     * keys are not programmed on the HW. Currently ths is only used for
     * holding default keys for WPA2 adhoc 
     */
    HW_KEY_ENTRY                PrivateKeyTable[DOT11_MAX_NUM_DEFAULT_KEY];

    /** We only support one key mapping key per peer */
    PHW_KEY_ENTRY               KeyMappingKey;
}HW_PEER_NODE, *PHW_PEER_NODE;


/**
 * This is rate adaptation information. For better performance, we should 
 * maintain this information per peer
 */
typedef struct _HW_RATE_INFO
{
    /** Total data send retry count for Tx rate negotiation*/
    LONG                        TotalRetryCount;
    ULONG                       PacketsSentForTxRateCheck;
    USHORT                      PrevTxDataRate;
    USHORT                      TxRateIncreaseWaitCount;
    USHORT                      TxRateIncreaseWaitRequired;

    /** The data rate we should use */
    USHORT                      CurrentTXDataRate;

}HW_RATE_INFO, *PHW_RATE_INFO;

#define HW_ADD_MAC_CONTEXT_REF(_MacContext, _Val)           \
    MPASSERTOP(InterlockedExchangeAdd((PLONG)&((_MacContext)->GenericRefCount), _Val) >= 0)

#define HW_REMOVE_MAC_CONTEXT_REF(_MacContext, _Val)      \
    MPASSERTOP(InterlockedExchangeAdd((PLONG)&((_MacContext)->GenericRefCount), (_Val * -1)) >= 0)

#define HW_ADD_MAC_CONTEXT_SEND_REF(_MacContext, _Val)           \
    MPASSERTOP(InterlockedExchangeAdd((PLONG)&((_MacContext)->SendRefCount), _Val) >= 0)

#define HW_REMOVE_MAC_CONTEXT_SEND_REF(_MacContext, _Val)      \
    MPASSERTOP(InterlockedExchangeAdd((PLONG)&((_MacContext)->SendRefCount), (_Val * -1)) >= 0)

#define HW_ADD_MAC_CONTEXT_RECV_REF(_MacContext, _Val)           \
    MPASSERTOP(InterlockedExchangeAdd((PLONG)&((_MacContext)->RecvRefCount), _Val) >= 0)

#define HW_REMOVE_MAC_CONTEXT_RECV_REF(_MacContext, _Val)      \
    MPASSERTOP(InterlockedExchangeAdd((PLONG)&((_MacContext)->RecvRefCount), (_Val * -1)) >= 0)

// Mac context status flags and manipulation macros
#define HW_MAC_CONTEXT_VALID            0x00000001
#define HW_MAC_CONTEXT_ACTIVE           0x00000002
#define HW_MAC_CONTEXT_ACTIVATING       0x00000004
#define HW_MAC_CONTEXT_PAUSED           0x00000010
#define HW_MAC_CONTEXT_PAUSING          0x00000020
#define HW_MAC_CONTEXT_IN_DOT11_RESET   0x00000040
#define HW_MAC_CONTEXT_LINK_UP          0x00000100

#define HW_SET_MAC_CONTEXT_STATUS(_MacContext, _Flag)    \
    MP_INTERLOCKED_SET_FLAG(&((_MacContext)->Status), _Flag)

#define HW_CLEAR_MAC_CONTEXT_STATUS(_MacContext, _Flag)  \
    MP_INTERLOCKED_CLEAR_FLAG(&((_MacContext)->Status), _Flag)

#define HW_TEST_MAC_CONTEXT_STATUS(_MacContext, _Flag)   \
    MP_TEST_FLAG((_MacContext)->Status, _Flag)

#define HW_MAC_CONTEXT_IS_ACTIVE(_MacContext)            \
    HW_TEST_MAC_CONTEXT_STATUS(_MacContext, HW_MAC_CONTEXT_ACTIVE)

#define HW_MAC_CONTEXT_IS_VALID(_MacContext)            \
    HW_TEST_MAC_CONTEXT_STATUS(_MacContext, HW_MAC_CONTEXT_VALID)

/** When true, we should consider this MAC context when merging stuff on the hw */
#define HW_MAC_CONTEXT_MUST_MERGE(_MacContext)            \
    HW_TEST_MAC_CONTEXT_STATUS(_MacContext, (HW_MAC_CONTEXT_ACTIVE | HW_MAC_CONTEXT_ACTIVATING))

/**
 * When any of these flags are set, the MAC context would not send any packets
 */
#define HW_MAC_CONTEXT_CANNOT_SEND_FLAGS        (HW_MAC_CONTEXT_PAUSED | HW_MAC_CONTEXT_PAUSING | \
                                     HW_MAC_CONTEXT_IN_DOT11_RESET)

/**
 * When any of these flags are set, the MAC context would not process any receive packets
 */
#define HW_MAC_CONTEXT_CANNOT_RECEIVE_FLAGS     (HW_MAC_CONTEXT_PAUSED | HW_MAC_CONTEXT_PAUSING | \
                                     HW_MAC_CONTEXT_IN_DOT11_RESET)

/** 
 * Macro to obtain port number of MAC_CONTEXT
 */
#define HW_MAC_PORT_NO(_MacContext) ((_MacContext)->PortNumber)

/**
 * This is per-port HW state
 */
typedef struct _HW_MAC_CONTEXT
{

    /** The _HW structure encapsulating this MAC context */
    PHW                         Hw;

    /** The _VNIC in the HVL that corresponds to this MAC context */
    PVNIC                       VNic;

    /** Ref count for send packets */
    ULONG                       SendRefCount;

    /** Ref count for receive packets */
    ULONG                       RecvRefCount;

    /** Generic refcounts for anything other than send/receive
     */
    ULONG                       GenericRefCount;

    /** Status to track current state of the MAC context */
    ULONG                       Status;

    /** Current op mode as configured by the upper layer */
    ULONG                       CurrentOpMode;

    /** Port number associated by NDIS for this MAC. This should only be used
     * for logging purposes and not for talking to NDIS 
     */
    NDIS_PORT_NUMBER            PortNumber;
    //
    // MAC STATE
    //
    
    /** MAC address of this MAC */
    DOT11_MAC_ADDRESS           MacAddress;
    
    /** MAC context power management state */
    DOT11_POWER_MGMT_MODE       PowerMgmtMode;

    /** Current BSSID */
    DOT11_MAC_ADDRESS           DesiredBSSID;
    DOT11_BSS_TYPE              BssType;

    DOT11_SSID                  SSID;
    
    /** Beacon period */
    ULONG                       BeaconPeriod;
    ULONG                       AtimWindow;
    ULONGLONG                   LastBeaconTimestamp;
    
    ULONG                       RTSThreshold;
    ULONG                       FragmentationThreshold;
    ULONG                       ShortRetryLimit;
    ULONG                       LongRetryLimit;

    // Key information
    HW_KEY_ENTRY                KeyTable[HW11_KEY_TABLE_SIZE];    // 0-3 is default key, 4+ are key mapping keys
    UCHAR                       DefaultKeyIndex;
    UCHAR                       KeyMappingKeyCount;

    HW_PEER_NODE                PeerTable[HW11_PEER_TABLE_SIZE];
    UCHAR                       PeerNodeCount;

    // The peer node object to use if we cannot find the peer above
    HW_PEER_NODE                DefaultPeer;

    DOT11_AUTH_ALGORITHM        AuthAlgorithm;
    DOT11_CIPHER_ALGORITHM      UnicastCipher;
    DOT11_CIPHER_ALGORITHM      MulticastCipher;

    // Multicast address list
    UCHAR                       MulticastAddressCount;
    UCHAR                       MulticastAddressList[HW11_MAX_MULTICAST_LIST_SIZE][DOT11_ADDRESS_SIZE];
    BOOLEAN                     AcceptAllMulticast;

    // Packet filter set for this MAC
    ULONG                       PacketFilter;
    
    //
    // PHY STATE
    //

    /*
     * Phy ID that should be programmed on the hardware when
     * this MAC_CONTEXT is active
     */
    ULONG                       OperatingPhyId;

    /** Phy ID selected by OS for PHY specific operations */
    ULONG                       SelectedPhyId;  

    /** Software copy of the PHY state associated with this MAC context */
    HW_PHY_CONTEXT              PhyContext[MAX_NUM_PHY_TYPES];

    BOOLEAN                     ShortSlotTimeOptionEnabled;
    BOOLEAN                     CTSToSelfEnabled;
    ULONG                       CurrentRegDomain;

    // The TX data and management rates to use unless we have a better
    // idea
    USHORT                      DefaultTXDataRate;
    USHORT                      DefaultTXMgmtRate;

    // Rate adaptation information. This is maintained globally for the MAC_CONTEXT
    // instead of being per peer.
    HW_RATE_INFO                RateInfo;
    
    
    //
    // Statistics
    //
    DOT11_MAC_FRAME_STATISTICS  UnicastCounters;
    DOT11_MAC_FRAME_STATISTICS  MulticastCounters;

    //
    // Data necessary for operation
    //
    
    /** If there is a pending Join, this would be set */
    VNIC11_GENERIC_CALLBACK_FUNC    PendingJoinCallback;
    PVOID                       JoinContext;
    ULONG                       JoinWaitForBeacon;
    NDIS_HANDLE                 Timer_JoinTimeout;
    ULONG                       JoinFailureTimeout;

    /** Periodic timer for generic purpose. Currently used for rate adaptation */
    NDIS_HANDLE                 Timer_Periodic;
    ULONG                       PeriodicTimerQueued;
    
    /** If there is a pending channel switch OID, this would be set */
    VNIC11_GENERIC_CALLBACK_FUNC    OidChannelSwitchCallback;
    PVOID                       ChannelSwitchContext;

    /** Sequence number of the packets to be sent */
    USHORT                      SequenceNumber;

    /** BSS description for beaconing */
    PUCHAR                      BeaconIEBlob;
    ULONG                       BeaconIEBlobSize;
    PUCHAR                      ProbeResponseIEBlob;
    ULONG                       ProbeResponseIEBlobSize;
    /** Enabled to send probe response */
    BOOLEAN                     BSSStarted; 
    /** Enabled to send beacons */
    BOOLEAN                     BeaconEnabled;  

    VNIC11_GENERIC_CALLBACK_FUNC    PendingScanCallback;
    PVOID                       ScanContext;
    
} HW_MAC_CONTEXT, *PHW_MAC_CONTEXT;

/**
 * MAC state information that is currently programmed on the hardware. This is generally
 * a combination of the state from the various MAC contexts
 */
typedef struct _NIC_MAC_STATE 
{
    /**
     * This is the MAC address that is programmed on the hardware. We are
     * assuming that the physical hardware only supports a single MAC address. Else
     * this should be stored per MAC
     */
    DOT11_MAC_ADDRESS           MacAddress;

    /** BSSID programmed on H/W */
    DOT11_MAC_ADDRESS           Bssid;
    
    /**
     * HW power management state
     */
    DOT11_POWER_MGMT_MODE       PowerMgmtMode;

    ULONG                       MaxTransmitMSDULifetime;
    ULONG                       MaxReceiveLifetime;
    
    // Key table
    PNICKEY                     KeyTable[HW11_KEY_TABLE_SIZE];    // 0-3 is default key, 4+ are key mapping keys
    UCHAR                       KeyMappingKeyCount;

    /** Multicast address list */
    UCHAR                       MulticastAddressCount;
    UCHAR                       MulticastAddressList[HW11_MAX_MULTICAST_LIST_SIZE][DOT11_ADDRESS_SIZE];
    BOOLEAN                     AcceptAllMulticast;
    
    /** Packet filter set on the HW */
    ULONG                       PacketFilter;

    /** Op mode currently programmed on the HW */
    ULONG                       OperationMode;
    DOT11_BSS_TYPE              BssType;
    /** 
     * When we are in safe mode, only one
     * MAC is used. So we hold this state in the HW MAC state
     */
    BOOLEAN                     SafeModeEnabled;

    /**
     * When we are in netmon mode, only one
     * MAC is used. So we hold this state in the HW MAC state
     */
    BOOLEAN                     NetmonModeEnabled;

    /**
     * Set to true when either AP or Adhoc mode & is running a BSS. When
     * true we send probe responses
     */
    BOOLEAN                     BSSStarted;
    
    /**
     * Set to true when in AP/adhoc mode beaconing is enabled
     */
    BOOLEAN                     BeaconEnabled;

    /**
     * Mac context that has started the BSS
     */
    PHW_MAC_CONTEXT             BSSMac;
    LONG                        ActiveBeaconIndex;

    HAL_POWERSAVE_CAP           HalPowerSaveCapability;

} NIC_MAC_STATE, *PNIC_MAC_STATE;


/**
 * PHY state information that is currently programmed on the hardware. This is generally
 * a combination of the state from the various MAC contexts
 */
typedef struct _NIC_PHY_STATE 
{
    // Phy ID that is currently programmed on the hardware
    ULONG                       OperatingPhyId;

    DOT11_DIVERSITY_SUPPORT     DiversitySupport;

    DOT11_SUPPORTED_POWER_LEVELS    SupportedPowerLevels;
    ULONG                       CurrentTxPowerLevel; // 1..8

	PDOT11_REG_DOMAINS_SUPPORT_VALUE    RegDomainsSupportValue;
    ULONG                       DefaultRegDomain;
    
	PDOT11_DIVERSITY_SELECTION_RX_LIST	DiversitySelectionRxList;

    BOOLEAN                     SoftwareRadioOff;
    BOOLEAN                     Debug_SoftwareRadioOff;
    
    NDIS_HANDLE                 Timer_Awake;
    NDIS_HANDLE                 Timer_Doze;
    LONG                        RadioAccessRef;
    BOOLEAN                     RadioStateChangeInProgress;

    /** The work item used to switch channels & PHY */
    NDIS_HANDLE                 PhyProgramWorkItem;
    /** The MAC context that requested the phy programming */
    PHW_MAC_CONTEXT             PhyProgramMacContext;
    HW_GENERIC_CALLBACK_FUNC    PendingPhyProgramCallback;

    /** The PHY/channel number to switch to */
    ULONG                       DestinationPhyId;
    PHW_PHY_CONTEXT             NewPhyContext;
    
} NIC_PHY_STATE, *PNIC_PHY_STATE;

typedef struct _NIC_STATISTICS {

    ULONGLONG				    NumRxNoBuf;     // RX no buffer error
    ULONGLONG                   NumRxError;     // General errors on RX
    ULONGLONG		            NumTxBeaconOk;
    ULONGLONG		            NumTxBeaconErr;	
    ULONGLONG                   NumRxReassemblyError;
	
    DOT11_PHY_FRAME_STATISTICS  PhyCounters[MAX_NUM_PHY_TYPES];

} NIC_STATISTICS, *PNIC_STATISTICS;

/**
 * Holds state for performing software encryption/description
 */
typedef struct _NIC_CRYPTO_STATE {
    BCRYPT_ALG_HANDLE           AlgoHandle;
    ULONG                       KeyObjLen;

    /** Lookaside list for software encryption data */
    NPAGED_LOOKASIDE_LIST       SoftEncryptLookaside;
    
} NIC_CRYPTO_STATE, *PNIC_CRYPTO_STATE;

/** 
 * Data type used for HAL resets
 */
typedef struct _HW_HAL_RESET_PARAMETERS {
    /** When true, this is a full reset */
    BOOLEAN                     FullReset;
} HW_HAL_RESET_PARAMETERS, *PHW_HAL_RESET_PARAMETERS;

/**
 * Reason for disabling/enabling the interrupts. This is only for tracking purpose
 */
typedef enum _HW_ISR_TRACKING_REASON {

    HW_ISR_TRACKING_PAUSE = 1,
    HW_ISR_TRACKING_INTERRUPT,
    HW_ISR_TRACKING_DOT11_RESET,
    HW_ISR_TRACKING_NDIS,
    HW_ISR_TRACKING_START_BSS,
    HW_ISR_TRACKING_RESUME_BSS,
    HW_ISR_TRACKING_SHUTDOWN,
    HW_ISR_TRACKING_HWSTART,
    HW_ISR_TRACKING_HWSTOP,
    HW_ISR_TRACKING_NDIS_RESET,
    HW_ISR_TRACKING_HAL_RESET,
    HW_ISR_TRACKING_CHANNEL,
    HW_ISR_TRACKING_RADIO_STATE,
    HW_ISR_MAX_TRACKING_REASON
}HW_ISR_TRACKING_REASON;



// Adapter status flags and manipulation macros
#define HW_ADAPTER_PAUSED           0x00000001
#define HW_ADAPTER_PAUSING          0x00000002
#define HW_ADAPTER_HALTING          0x00000010
#define HW_ADAPTER_IN_RESET         0x00000020
#define HW_ADAPTER_IN_DOT11_RESET   0x00000040
#define HW_ADAPTER_HAL_IN_RESET     0x00000080
#define HW_ADAPTER_IN_CONTEXT_SWITCH    0x00000100
#define HW_ADAPTER_SURPRISE_REMOVED 0x00000200
#define HW_ADAPTER_IN_LOW_POWER     0x00000400
#define HW_ADAPTER_IN_CHANNEL_SWITCH    0x00000800
#define HW_ADAPTER_RADIO_OFF        0x00001000

#define HW_SET_ADAPTER_STATUS(_HW, _Flag)    \
    MP_INTERLOCKED_SET_FLAG(&((_HW)->Status), _Flag)

#define HW_CLEAR_ADAPTER_STATUS(_HW, _Flag)  \
    MP_INTERLOCKED_CLEAR_FLAG(&((_HW)->Status), _Flag)

#define HW_TEST_ADAPTER_STATUS(_HW, _Flag)   \
    MP_TEST_FLAG((_HW)->Status, _Flag)


/**
 * When any of these flags are set, the hardware would not send any packets
 */
#define HW_CANNOT_SEND_FLAGS        (HW_ADAPTER_PAUSED | HW_ADAPTER_PAUSING | HW_ADAPTER_HALTING |  \
                                     HW_ADAPTER_IN_RESET | HW_ADAPTER_IN_CONTEXT_SWITCH |           \
                                     HW_ADAPTER_IN_DOT11_RESET | HW_ADAPTER_HAL_IN_RESET | \
                                     HW_ADAPTER_SURPRISE_REMOVED | HW_ADAPTER_IN_LOW_POWER | \
                                     HW_ADAPTER_IN_CHANNEL_SWITCH | HW_ADAPTER_RADIO_OFF)

/**
 * When any of these flags are set, the hardware would not process any receive packets
 */
#define HW_CANNOT_RECEIVE_FLAGS     (HW_ADAPTER_PAUSED | HW_ADAPTER_PAUSING | HW_ADAPTER_HALTING |  \
                                     HW_ADAPTER_IN_RESET | HW_ADAPTER_IN_CONTEXT_SWITCH |           \
                                     HW_ADAPTER_IN_DOT11_RESET | HW_ADAPTER_HAL_IN_RESET | \
                                     HW_ADAPTER_SURPRISE_REMOVED | HW_ADAPTER_IN_LOW_POWER| \
                                     HW_ADAPTER_IN_CHANNEL_SWITCH| HW_ADAPTER_RADIO_OFF)

/**
 * When any of these flags are set, the hardware should not be touched. Radio OFf
 * is also in the list because on certain cards/systems, radio off turns off the 
 * bus
 */
#define HW_CANNOT_ACCESS_HARDWARE   (HW_ADAPTER_SURPRISE_REMOVED | HW_ADAPTER_RADIO_OFF)


/**
 * If the number of MAC contexts is 2 (helper and another one) then there
 * are multiple MACs enabled
 */
#define HW_MULTIPLE_MAC_ENABLED(_HW) (!((_HW)->MacContextCount <= 2))

// Hardware lock manipulation macros
#define HW_ACQUIRE_HARDWARE_LOCK(_HW, _AtDispatch)       \
    MP_ACQUIRE_SPINLOCK(_HW->Lock, _AtDispatch)

#define HW_RELEASE_HARDWARE_LOCK(_HW, _AtDispatch)       \
    MP_RELEASE_SPINLOCK(_HW->Lock, _AtDispatch)

/** Use to wait for active calls into the HW to finish on the HW */
#define HW_WAIT_FOR_ACTIVE_OPERATIONS_TO_FINISH(_Hw)      \
    while (_Hw->AsyncFuncRef > 0) {  NdisMSleep(100); }

#define HW_INCREMENT_ACTIVE_OPERATION_REF(_HW)           \
    MPASSERTOP(InterlockedIncrement((PLONG)&((_HW)->AsyncFuncRef)) >= 0)

#define HW_DECREMENT_ACTIVE_OPERATION_REF(_HW)           \
    MPASSERTOP(InterlockedDecrement((PLONG)&((_HW)->AsyncFuncRef)) >= 0)

/** Use to wait for active calls into the HW to finish on the HW */
#define HW_WAIT_FOR_ACTIVE_SENDS_TO_FINISH(_Hw)      \
    while (_Hw->ActiveSendRef > 0) {  NdisMSleep(100); }

#define HW_INCREMENT_ACTIVE_SEND_REF(_HW)           \
    MPASSERTOP(InterlockedIncrement((PLONG)&((_HW)->ActiveSendRef)) >= 0)

#define HW_DECREMENT_ACTIVE_SEND_REF(_HW)           \
    MPASSERTOP(InterlockedDecrement((PLONG)&((_HW)->ActiveSendRef)) >= 0)

typedef struct _HW
{
	/**
	 * The handle by which NDIS recognizes this adapter. This handle needs to be passed
	 * in for many of the calls made to NDIS
	 */
	NDIS_HANDLE			        MiniportAdapterHandle;

	PADAPTER		            Adapter;

    PHVL                        Hvl;

    /** Hardware abstraction layer for WLAN devices */
    PWLAN_HAL                   Hal;

    /** 
     * Status flags maintaining current state of the hardware.
     * This flag is interlocked and send and receive routines would TEST
     * this flag without the lock held. However other operations like
     * Join, Scan, etc would the set the flag with the lock held.
     */
    ULONG                       Status;

    /** Main lock for the hardware layer */
    NDIS_SPIN_LOCK              Lock;

    /** Non-zero when there are any major asynchronous functions running in the HW layer that
     * need to be synchronized against reset/MAC create/delete functions
     */
    LONG                        AsyncFuncRef;
    LONG                        ActiveSendRef;

    /** Interrupt handle registered with NDIS */
    NDIS_HANDLE                 InterruptHandle;
    LONG                        InterruptDisableCount; // > 0 Disabled, else enabled

#if DBG
    /** Tracks the reasons for interrupt disable */
    LONG                        Tracking_InterruptDisable[HW_ISR_MAX_TRACKING_REASON];
#endif

    /** Handle to DMA context for this miniport */
    NDIS_HANDLE                 MiniportDmaHandle;
    NDIS_DEVICE_POWER_STATE     NextDevicePowerState;

    /** Stuff read from the registry */
    NIC_REG_INFO                RegInfo;
    
    /**
     * Non-hardware specific TX related data
     */
    NIC_TX_INFO                 TxInfo;

    /**
     * Non-hardware specific RX related data
     */
    NIC_RX_INFO                 RxInfo;

    /**
     * Scan state of the hardware
     */
    NIC_SCAN_CONTEXT            ScanContext;
    
    /**
     * MAC state currently set on the hardware
     */
    NIC_MAC_STATE               MacState;


    /**
     * PHY state currently set on the hardware
     */
    NIC_PHY_STATE               PhyState;

    /**
     * NIC statistics
     */
    NIC_STATISTICS              Stats;

    /**
     * Per-port state maintained by the HW. Index 0 is the helper port, 
     * port 1 & above are the onces allocated by the upper layer
     */
    HW_MAC_CONTEXT              MacContext[HW_MAX_NUMBER_OF_MAC];
    ULONG                       MacContextCount;

    /**
     * Holds info about encryption/decryption
     */
    NIC_CRYPTO_STATE            CryptoState;
} HW, *PHW;



