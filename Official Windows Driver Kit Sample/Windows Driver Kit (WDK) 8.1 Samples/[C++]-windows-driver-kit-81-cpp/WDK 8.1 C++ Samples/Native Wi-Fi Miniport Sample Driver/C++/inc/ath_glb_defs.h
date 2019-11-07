/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ath_glb_defs.h

Abstract:
    Hardware (Atheros) specific define common to the complete driver

Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

#define HW11_MAJOR_DRIVER_VERSION   1
#define HW11_MINOR_DRIVER_VERSION   0


/** Vendor specific driver version */
#define HW11_DRIVER_VERSION     

/**
 * Highest byte is the NIC byte plus three vendor bytes, they are normally
 * obtained from the NIC
 */
#define MP_VENDOR_ID                 0x00FFFFFF


// Pool tags for this driver
#define MP_MEMORY_TAG           'MltA'  // MP Layer
#define PORT_MEMORY_TAG         'PltA'  // Port
#define EXTSTA_MEMORY_TAG       'SltA'  // ExtSTA
#define EXTAP_MEMORY_TAG        'AltA'  // ExtAP
#define HVL_MEMORY_TAG          'VltA'  // HVL/VNIC
#define HW_MEMORY_TAG           'HltA'  // HW


/** The time interval in which NDIS should call MiniportCheckForHang handler. See NdisMSetAttributesEx for details */
#define HW11_CHECK_FOR_HANG_TIME_IN_SECONDS   6     // Increased CheckForHang time since our DOT11_RESET can
                                                    // take more than 2 seconds

/** Specifies the miniport attributes that are passed to NDIS */
#define HW11_NDIS_MINIPORT_ATTRIBUTES   NDIS_MINIPORT_ATTRIBUTES_BUS_MASTER    

/** Specifies the I/O bus interface type of the NIC, which usually is the type of I/O bus on which the NIC is connected */
#define HW11_BUS_INTERFACE_TYPE NdisInterfacePci

#define HW11_PCI_CONFIG_BUFFER_LENGTH	    PCI_COMMON_HDR_LENGTH

/** The link rate supported by this hardware (in 500 kbps) */
#define HW11_MAX_DATA_RATE      108

#define HW11_RX_BUFFER_SPACE    2400

#define HW11_MAX_FRAME_SIZE     (DOT11_MAX_PDU_SIZE - 4)
#define HW11_MIN_FRAME_SIZE     DOT11_MIN_PDU_SIZE
#define HW11_MAXIMUM_LOOKAHEAD  (DOT11_MIN_PDU_SIZE - (sizeof(DOT11_DATA_SHORT_HEADER) + 12))
#define HW11_MAX_MULTICAST_LIST_SIZE    32
#define HW11_REQUIRED_BACKFILL_SIZE     8

/** Maximum number of phy's supported */
#define HW11_MAX_PHY_COUNT      5

/** Minimum number of Rx MSDU that we can allocate or shrink to */
#define HW11_MIN_RX_MSDUS       64
/** Maximum number of Rx MSDUs that we will ever allocate or grow to */
#define HW11_MAX_RX_MSDUS       128
/** Default number of Rx MSDUs to start with */
#define HW11_DEF_RX_MSDUS       64

/** Minimum number of Tx MSDUs that need to be initialized with */
#define HW11_MIN_TX_MSDUS       1
/** Maximum number of Rx MSDUs that we can allocate */
#define HW11_MAX_TX_MSDUS       64
/** Default number of Tx MSDUs to allocate and initialize miniport with */
#define HW11_DEF_TX_MSDUS       64

/** Number of keys that can be held in the key table */
#define HW11_KEY_TABLE_SIZE     16

/** Number of peers we can keey track of */
#define HW11_PEER_TABLE_SIZE    16

#define HW11_MAX_TX_ANTENNA     2

#define HW11_MAX_RX_ANTENNA     2

#define HW11_VENDOR_DESCRIPTION "Atheros"

/** Maintain 4 queues in the driver. Three of these are used, one for data and management traffic
 * from ports, one for traffic from within the HW and one for beacons. The fourth is unused, but
 * since the HW requires it, we allocate it
 */
#define HW11_NUM_TX_QUEUE       4

/**
 * The purpose of each of the TX queues
 */
typedef enum _HW11_TX_QUEUE_TYPE
{
    HW11_DEFAULT_QUEUE = 0,
    HW11_INTERNAL_SEND_QUEUE = 1,
    HW11_UNUSED_QUEUE = 2,
    HW11_BEACON_QUEUE = 3
}HW11_TX_QUEUE_TYPE;

