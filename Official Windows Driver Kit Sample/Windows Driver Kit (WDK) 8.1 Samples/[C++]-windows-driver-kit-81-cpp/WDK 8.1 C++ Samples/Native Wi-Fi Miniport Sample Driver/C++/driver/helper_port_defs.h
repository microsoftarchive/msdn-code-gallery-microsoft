/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    helper_port_defs.h

Abstract:
    Contains helper port specific defines
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

#define MP_GET_HELPPORT(_Port)            ((PMP_HELPER_PORT)(_Port->ChildPort))
#define HELPPORT_GET_MP_PORT(_HelpPort)   ((PMP_PORT)(_HelpPort->ParentPort))
#define HELPPORT_GET_VNIC(_HelpPort)      (HELPPORT_GET_MP_PORT(_HelpPort)->VNic)

/** Maximum number of BSSs we will store in our BSS entries list */
#define MP_BSS_ENTRY_MAX_ENTRIES_DEFAULT       128
#define MP_BSS_ENTRY_MAX_ENTRIES_MIN           16
#define MP_BSS_ENTRY_MAX_ENTRIES_MAX           512

/**
 * Time duration after it was created at which a BSS entry is 
 * considered to have expired
 */
#define MP_BSS_ENTRY_EXPIRE_TIME_DEFAULT       750000000       // 75 seconds
#define MP_BSS_ENTRY_EXPIRE_TIME_MIN           150000000       // 15 seconds
#define MP_BSS_ENTRY_EXPIRE_TIME_MAX          2000000000       // 200 seconds

/**
 * Link quality value (0-100 which if we go below for a sequence of beacons,
 * would suggest that we have a poor signal
 */
#define MP_POOR_LINK_QUALITY_THRESHOLD_DEFAULT   15
#define MP_POOR_LINK_QUALITY_THRESHOLD_MIN       5
#define MP_POOR_LINK_QUALITY_THRESHOLD_MAX       80


/** Maximum number of channels that we scan for when doing a passive scan & are not connected*/
#define MP_SCAN_SET_CHANNEL_COUNT_PASSIVE_DEFAULT      2
#define MP_SCAN_SET_CHANNEL_COUNT_PASSIVE_MIN          1
#define MP_SCAN_SET_CHANNEL_COUNT_PASSIVE_MAX          10

/** Maximum number of channels that we scan for when doing an active scan & are not connected*/
#define MP_SCAN_SET_CHANNEL_COUNT_ACTIVE_DEFAULT       3   // We can scan more channels when in active
#define MP_SCAN_SET_CHANNEL_COUNT_ACTIVE_MIN           1
#define MP_SCAN_SET_CHANNEL_COUNT_ACTIVE_MAX           10


/** Interval between partial scan timer (in milliseconds) */
#define MP_SCAN_RESCHEDULE_TIME_MS_DEFAULT             300
#define MP_SCAN_RESCHEDULE_TIME_MS_MIN                 10
#define MP_SCAN_RESCHEDULE_TIME_MS_MAX                 1000

#define MP_SCAN_RESCHEDULE_TIME_NOT_CONNECTED          100

/** Interval between mutiple scan (in seconds) */
#define MP_INTER_SCAN_TIME_DEFAULT                     30
#define MP_INTER_SCAN_TIME_MIN                         10
#define MP_INTER_SCAN_TIME_MAX                         1000



/** State of each scan parameters structure */
typedef enum _MP_SCAN_STATE
{
    SCAN_EMPTY_REQUEST,

    SCAN_REQUEST_IN_USE,        /** Request is in some transition state */
    
    SCAN_QUEUED_FOR_PROCESSING, /** Queued, but not yet started */

    SCAN_STARTED,               /** Started scanning */

    SCAN_EXCLUSIVE_ACCESS_QUEUED,/** Waiting for exclusive access */

    SCAN_EXCLUSIVE_ACCESS_ACQUIRED,/** Acquired exclusive access */

    SCAN_HARDWARE_SCANNING,     /** Submitted to hardware */

    SCAN_WAITING_FOR_TIMER,     /** Waiting for timer for next scan */

    SCAN_COMPLETED              /** Scan complete indicated */
}MP_SCAN_STATE, *PMP_SCAN_STATE;

// Scan parameter structure refcount 
#define MP_INCREMENT_SCAN_PARAMETER_REF(_ScanParam)\
    NdisInterlockedIncrement(&(_ScanParam->UsageCount))

#define MP_DECREMENT_SCAN_PARAMETER_REF(_ScanParam)\
    NdisInterlockedDecrement(&(_ScanParam->UsageCount))


/** Used to hold state about each port's scan request */
typedef struct _MP_SCAN_PARAMETERS
{
    /** Port that requested the scan */
    PMP_PORT                RequestingPort;
    
    /** Original scan request from the port */
    PMP_SCAN_REQUEST        PortScanRequest;
    
    /** Scan completion callback into the port */
    PORT11_GENERIC_CALLBACK_FUNC    CompletionHandler;

    /** Current state of the scan */
    MP_SCAN_STATE           State;

    /** Set to true when the scan needs to be cancelled */
    BOOLEAN                 CancelScan;

    MP_SCAN_STATE           TrackingPreCancelState;

    /** If non-zero, this scan request is in use */
    LONG                    UsageCount;

    /** The PHY we are currently scanning on */
    ULONG                   CurrentPhyIndex;
    
    /** The next channel to process once the current partial scan is done. This is
     * on the current phy 
     */
    ULONG                   NextChannelIndex;
    
    /** Maximum number of channels to scan at a time */
    ULONG                   MaxChannelCount;
    
    /** The scan request that is passed to the VNic */
    MP_SCAN_REQUEST         VNicScanRequest;
} MP_SCAN_PARAMETERS, *PMP_SCAN_PARAMETERS;

/**
 * Hold list of channels to scan on each PHY
 */
typedef struct _MP_SCAN_CHANNEL_LIST
{
    /** The PHY ID */
    ULONG                   PhyId;

    /** Number of channels in the channel list */
    ULONG                   ChannelCount;

    /** Actual list of channel to scan on this PHY */
    PULONG                  ChannelList;

}MP_SCAN_CHANNEL_LIST, *PMP_SCAN_CHANNEL_LIST;


/**
 * State we maintain for holding 
 */
typedef struct _MP_SCAN_CONTEXT
{
    /** The scan request that we are currently processing */
    PMP_SCAN_PARAMETERS         ActiveScanParameters;

    /** The time at which we started the last scan (periodic or OS scan) */
    ULONGLONG                   LastScanTime;

    /** Current list of scan requests. There is no relationship betwee
     * index into this array and port number. Since each port would have
     * max 1 scan, we use use the MP_MAX_NUMBER_OF_PORT for the size of 
     * cached scan requests array
     */
    MP_SCAN_PARAMETERS          ScanParameters[MP_MAX_NUMBER_OF_PORT];
    LONG                        ParametersCount;

    /** The full list of channels supported/enabled by the HW*/
    MP_SCAN_CHANNEL_LIST        ScanChannels[HW11_MAX_PHY_COUNT];

    /** Since we split the upper layer scan into multiple scan requests
     * to the HW, this timer is used to schedule the next partial scan 
     */     
    NDIS_HANDLE                 Timer_Scan;

    /* 
        Holds the number of media connected notifications
        */
    ULONG                       MediaConnectedCount;

}MP_SCAN_CONTEXT, *PMP_SCAN_CONTEXT;

/**
 * Registry configuration information
 */
typedef struct _MP_HELPER_REG_INFO
{
    /** Link quality for low threshold */
    ULONG                       RSSILinkQualityThreshold;

    /** Time in 100ns, to hold a BSS entry even after beacons are missed */
    ULONG                       BSSEntryExpireTime;

    /** Max number of BSS entries to cache */
    ULONG                       BSSEntryMaxCount;

    ULONG                       ActiveScanChannelCount; // MP_SCAN_SET_CHANNEL_COUNT_ACTIVE

    ULONG                       PassiveScanChannelCount;// MP_SCAN_SET_CHANNEL_COUNT_PASSIVE

    ULONG                       ScanRescheduleTime; // MP_SCAN_RESCHEDULE_TIME_MS

    ULONG                       InterScanTime;      // MP_INTER_SCAN_TIME

} MP_HELPER_REG_INFO, *PMP_HELPER_REG_INFO;


typedef struct _MP_HELPER_PORT
{
    /** Pointer back to the parent port of this port */
    PMP_PORT                    ParentPort;

    /** Information about BSS */
	MP_BSS_LIST                 BSSList;

    NDIS_DEVICE_POWER_STATE     DevicePowerState;
    
    /** Scan state */
    MP_SCAN_CONTEXT             ScanContext;

    /** Pointer to helper port registry info */
    PMP_HELPER_REG_INFO         RegInfo;

    LONG                        Tracking_ExclusiveAccessAcquireCount;
    LONG                        Tracking_ExclusiveAccessReleaseCount;
    LONG                        Tracking_ExclusiveAccessGrantCount;

    LONG                        Debug_ExclusiveAccessCount;
} MP_HELPER_PORT, *PMP_HELPER_PORT;

NDIS_STATUS 
HelperPortPauseMiniport(
    _In_        PMP_HELPER_PORT                 HelperPort,
    _In_        BOOLEAN                         IsInternal,
    _In_opt_    PNDIS_MINIPORT_PAUSE_PARAMETERS MiniportPauseParameters
    );

NDIS_STATUS 
HelperPortRestartMiniport(
    _In_        PMP_HELPER_PORT                     HelperPort,
    _In_        BOOLEAN                             IsInternal,
    _In_opt_    PNDIS_MINIPORT_RESTART_PARAMETERS   MiniportRestartParameters
    );

NDIS_STATUS 
HelperPortPausePort(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_PORT                PortToPause    
    );

NDIS_STATUS 
HelperPortRestartPort(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_PORT                PortToRestart
    );

NDIS_STATUS 
HelperPortNdisReset(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PBOOLEAN                AddressingReset
    );

NDIS_STATUS 
HelperPortSetPower(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  NDIS_DEVICE_POWER_STATE NewDeviceState
    );


NDIS_STATUS
HelperPortRequestExclusiveAccess(
    _In_  PMP_HELPER_PORT       HelperPort,
    _In_  PORT11_GENERIC_CALLBACK_FUNC CallbackFunction,
    _In_  PVOID                 Context,
    _In_  BOOLEAN               PnPOperation
    );
        
VOID
HelperPortReleaseExclusiveAccess(
    _In_  PMP_HELPER_PORT       HelperPort
    );
    
VOID
HelperPortExclusiveAccessGranted(
    _In_  PMP_HELPER_PORT       HelperPort
    );

VOID
HelperPortDelegateExclusiveAccess(
    _In_  PMP_HELPER_PORT       HelperPort,
    _In_  PMP_PORT              PortToActivate
    );


