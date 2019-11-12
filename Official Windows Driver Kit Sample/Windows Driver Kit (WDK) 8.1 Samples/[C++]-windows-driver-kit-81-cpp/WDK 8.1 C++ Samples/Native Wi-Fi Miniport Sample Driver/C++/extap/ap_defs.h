/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_defs.h

Abstract:
    Contains ExtAP specific defines
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-20-2007    Created

Notes:

--*/

#pragma once

#ifndef _AP_DEFS_H
#define _AP_DEFS_H

#define AP11_MAX_IE_BLOB_SIZE   (DOT11_MAX_PDU_SIZE - sizeof(DOT11_MGMT_HEADER) - \
                                   FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements) - 4)


/**
 * Capabilities
 */
#define AP_SCAN_SSID_LIST_MAX_SIZE                          4
#define AP_DESIRED_SSID_LIST_MAX_SIZE                       1
#define AP_PRIVACY_EXEMPTION_LIST_MAX_SIZE                  32
#define AP_STRICTLY_ORDERED_SERVICE_CLASS_IMPLEMENTED       FALSE

/** ExtAP default configurations (hardware independent) */
#define AP_DEFAULT_ENABLED_AUTO_CONFIG          (DOT11_PHY_AUTO_CONFIG_ENABLED_FLAG | DOT11_MAC_AUTO_CONFIG_ENABLED_FLAG)

#define AP_DEFAULT_EXCLUDE_UNENCRYPTED          FALSE

#define AP_DEFAULT_AUTHENTICATION_ALGORITHM     DOT11_AUTH_ALGO_RSNA_PSK

#define AP_DEFAULT_UNICAST_CIPHER_ALGORITHM     DOT11_CIPHER_ALGO_CCMP

#define AP_DEFAULT_MULTICAST_CIPHER_ALGORITHM   AP_DEFAULT_UNICAST_CIPHER_ALGORITHM

#define AP_DEFAULT_DESIRED_PHY_ID_COUNT         1

#define AP_DEFAULT_DESIRED_PHY_ID               DOT11_PHY_ID_ANY

#define AP_DEFAULT_PHY_ID                       0

#define AP_DEFAULT_ADDITIONAL_IE_SIZE           0

#define AP_DEFAULT_ADDITIONAL_IE_DATA           NULL

#define AP_DEFAULT_ENABLE_WPS                   FALSE

#define AP_DEFAULT_ENABLE_BEACON                TRUE

/** 
 * ExtAP default registy info settings, with their ranges
 * a default setting is used if the registry is not present or has invalid value
 */

#define AP_DEFAULT_ALLOWED_ASSOCIATION_COUNT    128
#define AP_MIN_ALLOWED_ASSOCIATION_COUNT        0
#define AP_MAX_ALLOWED_ASSOCIATION_COUNT        255
    
#define AP_DEFAULT_ENABLE_5GHZ                  FALSE

#define AP_DEFAULT_CHANNEL                      11
#define AP_MIN_CHANNEL                          0
#define AP_MAX_CHANNEL                          11
#define AP_MAX_CHANNEL_FOR_11A                  136

#define AP_DEFAULT_FREQUENCY                    136

#define AP_DEFAULT_GROUP_KEY_RENEWAL_INTERVAL   3600        // in seconds
#define AP_MIN_GROUP_KEY_RENEWAL_INTERVAL       60
#define AP_MAX_GROUP_KEY_RENEWAL_INTERVAL       86400

#define AP_DEFAULT_ENABLE_CTS_PROTECTION        FALSE

#define AP_DEFAULT_ENABLE_FRAME_BURST           FALSE

#define AP_DEFAULT_BEACON_PERIOD                100         // in TUs
#define AP_MIN_BEACON_PERIOD                    10
#define AP_MAX_BEACON_PERIOD                    10000

#define AP_DEFAULT_DTIM_PERIOD                  2           // beacon intervals
#define AP_MIN_DTIM_PERIOD                      1
#define AP_MAX_DTIM_PERIOD                      100

#define AP_DEFAULT_RTS_THRESHOLD                2347
#define AP_MIN_RTS_THRESHOLD                    0
#define AP_MAX_RTS_THRESHOLD                    2347

#define AP_DEFAULT_FRAGMENTATION_THRSHOLD       2346
#define AP_MIN_FRAGMENTATION_THRESHOLD          256
#define AP_MAX_FRAGMENTATION_THRESHOLD          2346

#define AP_DEFAULT_SHORT_RETRY_LIMIT            7
#define AP_MIN_SHORT_RETRY_LIMIT                1
#define AP_MAX_SHORT_RETRY_LIMIT                255

#define AP_DEFAULT_LONG_RETRY_LIMIT             4
#define AP_MIN_LONG_RETRY_LIMIT                 1
#define AP_MAX_LONG_RETRY_LIMIT                 255

#define AP_DEFAULT_ENABLE_WMM                   FALSE

/** 
 * Get access to the MP_EXTSTA_PORT from the MP_PORT
 */
#define MP_GET_AP_PORT(_Port)           ((PMP_EXTAP_PORT)(_Port->ChildPort))
#define AP_GET_MP_PORT(_ExtPort)        ((PMP_PORT)(_ExtPort->ParentPort))
#define AP_GET_VNIC(_ExtPort)           (AP_GET_MP_PORT(_ExtPort)->VNic)
#define AP_GET_ADAPTER(_ExtPort)        (AP_GET_MP_PORT(_ExtPort)->Adapter)
#define AP_GET_MP_HANDLE(_ExtPort)      (AP_GET_MP_PORT(_ExtPort)->MiniportAdapterHandle)
#define AP_GET_PORT_NUMBER(_ExtPort)    (AP_GET_MP_PORT(_ExtPort)->PortNumber)

/**
 * Get access to EXTAP components
 */
#define AP_GET_ASSOC_MGR(_ApPort)       (&(_ApPort)->AssocMgr)
#define AP_GET_CONFIG(_ApPort)          (&(_ApPort)->Config)
#define AP_GET_CAPABILITY(_ApPort)      (&(_ApPort)->Capability)
#define AP_GET_REG_INFO(_ApPort)        (&(_ApPort)->RegInfo)

/**
 * Get access to AP settings
 */

/** Settings maintained by association manager */ 
#define AP_GET_SSID(_ApPort)                    (AP_GET_ASSOC_MGR(_ApPort)->Ssid)
#define AP_GET_BSSID(_ApPort)                   (AP_GET_ASSOC_MGR(_ApPort)->Bssid)
#define AP_GET_CAPABILITY_INFO(_ApPort)         (AP_GET_ASSOC_MGR(_ApPort)->Capability)
#define AP_GET_AUTH_ALGO(_ApPort)               (AP_GET_ASSOC_MGR(_ApPort)->AuthAlgorithm)
#define AP_GET_UNICAST_CIPHER_ALGO(_ApPort)     (AP_GET_ASSOC_MGR(_ApPort)->UnicastCipherAlgorithm)
#define AP_GET_MULTICAST_CIPHER_ALGO(_ApPort)   (AP_GET_ASSOC_MGR(_ApPort)->MulticastCipherAlgorithm)
#define AP_GET_OPERATIONAL_RATE_SET(_ApPort)    (AP_GET_ASSOC_MGR(_ApPort)->OperationalRateSet)
#define AP_GET_WPS_ENABLED(_ApPort)             (AP_GET_ASSOC_MGR(_ApPort)->EnableWps)

/** Settings maintained by config */
#define AP_GET_BEACON_PERIOD(_ApPort)           (AP_GET_CONFIG(_ApPort)->BeaconPeriod)
#define AP_GET_DTIM_PERIOD(_ApPort)             (AP_GET_CONFIG(_ApPort)->DTimPeriod)
#define AP_GET_BEACON_IE(_ApPort)               (AP_GET_CONFIG(_ApPort)->AdditionalBeaconIEData)
#define AP_GET_BEACON_IE_SIZE(_ApPort)          (AP_GET_CONFIG(_ApPort)->AdditionalBeaconIESize)
#define AP_GET_PROBE_RESPONSE_IE(_ApPort)       (AP_GET_CONFIG(_ApPort)->AdditionalResponseIEData)
#define AP_GET_PROBE_RESPONSE_IE_SIZE(_ApPort)  (AP_GET_CONFIG(_ApPort)->AdditionalResponseIESize)

// TODO: do we really need this?
/** ExtAP capability */
typedef struct _AP_CAPABIITY
{
    /** Maximum number of SSIDs the NIC can support in OID_DOT11_SCAN_REQUEST */
    ULONG ScanSsidListSize;

    /** Maximum number of desired SSIDs the NIC can support */
    ULONG DesiredSsidListSize;

    /** Maximum number of Ethertype privacy exemptions the NIC can support */
    ULONG PrivacyExemptionListSize;

    /** Maximum number of associations the NIC can support */
    ULONG AssociationTableSize;
    
    // TODO: add other capabilities
} AP_CAPABIITY, *PAP_CAPABIITY;

/**
 * Settings read from the registry
 */
typedef struct _AP_REG_INFO 
{ 
    /** number of allowed associations */
    ULONG   AllowedAssociationCount;

    /** default channel/frequency */
    ULONG   DefaultChannel;

    // TODO: AP Mode (mixed)

    /** group key renewal interval, in seconds */
    ULONG   GroupKeyRenewalInterval;

    // TODO: transmission rate

    /** Beacon period, in TUs */
    ULONG   BeaconPeriod;

    /** DTIM period, in beacon intervals */
    ULONG   DTimPeriod;

    /** RTS threshold */
    ULONG   RtsThreshold;

    /** Fragmentation threshold */
    ULONG   FragmentationThreshold;

    /** Short retry limit */
    ULONG   ShortRetryLimit;

    /** Long retry limit */
    ULONG   LongRetryLimit;

    /** enable 5GHz or not */
    BOOLEAN Enable5GHz;

    /** enable CTS protection */
    BOOLEAN EnableCtsProtection;

    /** enable Frame burst */
    BOOLEAN EnableFrameBurst;

    /** enable WMM */
    BOOLEAN EnableWMM;
} AP_REG_INFO, *PAP_REG_INFO;

/** Define AP states */
typedef enum _AP_STATE
{
    AP_STATE_STOPPED = 0,                   // AP INIT state
    AP_STATE_STARTING,
    AP_STATE_STARTED,
    AP_STATE_STOPPING,
    AP_STATE_INVALID = 0xFFFFFFFF           // When MP is not in AP mode
} AP_STATE, *PAP_STATE;

/** ExtAP port */
typedef struct _MP_EXTAP_PORT
{
    /** parent port */
    PMP_PORT        ParentPort;

    /** ExtAP capability */
    AP_CAPABIITY    Capability;

    /** Registry settings */
    AP_REG_INFO     RegInfo;
    
    /** AP state */
    AP_STATE        State;

    /** 
     * AP reference count. 
     * Indicate the number of external functions 
     * that are accessing the AP port.
     * The AP port cannot be terminated
     * until it reaches zero.
     */
    LONG           RefCount;

    /** current AP configuration */
    AP_CONFIG       Config;

    /** association manager */
    AP_ASSOC_MGR    AssocMgr;

    // TODO: statistics
} MP_EXTAP_PORT, *PMP_EXTAP_PORT;

#endif  // _AP_DEFS_H
