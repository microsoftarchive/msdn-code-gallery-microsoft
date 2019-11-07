/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    scan_glb_defs.h

Abstract:
    Scan data related defines common to the complete driver

Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once


/**
 * Holds information about each BSS we have found.
 * This is updated on a beacon/probe response in the context of our
 * receive handler. A linked list of these entries is maintained
 * by the station to keep track of the discovered BSS
 */
typedef struct _MP_BSS_ENTRY 
{
    /** List entry linkage */
    LIST_ENTRY                  Link;

    /** Reference count to keep track of number of users of the AP entry.
     * When the entry is added to the linked list, this starts at 1. Association
     * process would add an extra reference to keep the entry around while
     * it is still associating/associated.
     */
    LONG                        RefCount;

    /**
     * Spinlock to protect modification of beacon/information element pointers while
     * other threads may be using it
     */
    NDIS_SPIN_LOCK              Lock;

    DOT11_PHY_TYPE              Dot11PhyType;

    ULONG                       PhyId;

    /** RSSI for the beacon/probe */
    LONG                        RSSI;

    /** Link quality */
    ULONG                       LinkQuality;

    ULONG                       ChannelCenterFrequency;
    
    DOT11_BSS_TYPE              Dot11BSSType;

    DOT11_MAC_ADDRESS           Dot11BSSID;

    DOT11_MAC_ADDRESS           MacAddress;

    USHORT                      BeaconInterval;

    ULONGLONG                   BeaconTimestamp;

    ULONGLONG                   HostTimestamp;

    DOT11_CAPABILITY            Dot11Capability;
    
    ULONG                       MaxBeaconFrameSize;
    PUCHAR                      pDot11BeaconFrame;      // Beacon frame starting after management header
    ULONG                       BeaconFrameSize;

    ULONG                       MaxProbeFrameSize;
    PUCHAR                      pDot11ProbeFrame;      // Probe response frame starting after management header
    ULONG                       ProbeFrameSize;

    PUCHAR                      pDot11InfoElemBlob;    // Pointer to the information elements in pDot11BeaconFrame
                                                       // or pDot11ProbeFrame (whichever one is the latest)
    ULONG                       InfoElemBlobSize;      // Length of information element blob

    UCHAR                       Channel;    // Valid only if it is non-zero

    /** 
     * The OS needs to be given the association request 
     * packet information on a association completion. This 
     * information is cached in this structure
     */
    _Field_size_(AssocRequestLength) 
    PUCHAR                      pAssocRequest;
    USHORT                      AssocRequestLength;     // Includes header

    /** 
     * The OS needs to be given the association response
     * packet information on a association completion. This 
     * information is cached in this structure
     */
    USHORT                      AssocResponseLength;    // Includes header
    _Field_size_(AssocResponseLength) 
    PUCHAR                      pAssocResponse;

    /** Association ID */
    USHORT                      AssocID;

    /** Association state. Just keeps state and not lock protected */
    DOT11_ASSOCIATION_STATE     AssocState;

    /** Timestamp when we obtained the association */
    LARGE_INTEGER               AssociationUpTime;

    /** 
     * Cost for roaming purpose. A lower number is better. We dont
     * rank of APs based on this, but use higher number this for rejecting 
     * some AP
     */
    LONG                        AssocCost;

    /**
     * Time in 100 ns after which we assume connectivity is lost
     * with this AP
     */
    ULONGLONG                   NoBeaconRoamTime;

    /**
     * Number of contiguous beacons which had signal strength lower than
     * our roaming threshold
     */
    ULONG                       LowQualityCount;

} MP_BSS_ENTRY, * PMP_BSS_ENTRY;


/**
 * Holds information about BSS we have discovered.
 */
typedef struct _MP_BSS_LIST 
{
    /** 
     * Linked list of discovered BSS
     * This list must not be modified/read without acquiring the 
     * ListLock
     */
    LIST_ENTRY                  List;

    /** 
     * Lock we need before we adding/removing entries from the 
     * discovered BSS list. This will be acquired for read by
     * routines that are not modifying the chain and acquired 
     * for write by routines that will be removing entries or
     * adding entries to the chain
     */
    MP_READ_WRITE_LOCK          ListLock;

    /** Maximum number of BSS we will keep track of */
    ULONG                       MaxNumOfBSSEntries;

    /** Number of BSSs we have discovered */
    ULONG                       NumOfBSSEntries;

}MP_BSS_LIST, *PMP_BSS_LIST;
