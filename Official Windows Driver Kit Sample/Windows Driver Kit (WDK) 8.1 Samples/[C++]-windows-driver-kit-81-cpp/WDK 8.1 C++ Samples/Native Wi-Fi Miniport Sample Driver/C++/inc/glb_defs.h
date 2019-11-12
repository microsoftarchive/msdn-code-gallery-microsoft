/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    glb_defs.h

Abstract:
    Contains global defines for the whole driver
    
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
typedef struct _HW_MAC_CONTEXT  HW_MAC_CONTEXT, *PHW_MAC_CONTEXT;

typedef enum _MP_PORT_TYPE
{
    HELPER_PORT,
    EXTSTA_PORT,
    EXTAP_PORT,
    EMPTY_PORT
} MP_PORT_TYPE, *PMP_PORT_TYPE;

/** NDIS_MEDIUM of the driver */
#define MP_MEDIA_TYPE           NdisMediumNative802_11

/** Physical medium of the driver */
#define MP_PHYSICAL_MEDIA_TYPE  NdisPhysicalMediumNative802_11

/** Defines Major version for the OID_DOT11_OPERATION_MODE_CAPABILITY */
#define MP_OPERATION_MODE_CAPABILITY_MAJOR_VERSION              2

/** Defines Minor version for the OID_DOT11_OPERATION_MODE_CAPABILITY */
#define MP_OPERATION_MODE_CAPABILITY_MINOR_VERSION              0

/** NdisGetVersion value for Vista SP1/Server 2008 */
#define MP_NDIS_VERSION_VISTASP1_SRV2008      0x00060001

/** NDIS version equal to or below this need to be in back compat mode */
#define MP_NDIS_VERSION_NEEDS_COMPATIBILITY   MP_NDIS_VERSION_VISTASP1_SRV2008

// Shorthand macro for common SAL annotation pattern used in this driver.
// In the pattern, a ULONG* is only set when an input buffer was of insufficient
// length. It is set to hold the number of bytes required for the input buffer.
#define _Out_when_invalid_ndis_length_   _Pre_notnull_ _Pre_writable_size_(1) \
                                    _When_(return == NDIS_STATUS_INVALID_LENGTH || \
                                           return == NDIS_STATUS_BUFFER_OVERFLOW, \
                                        _Post_valid_)

/** Structure used to read information from the registry */
typedef struct _MP_REG_ENTRY
{
    NDIS_STRING                 RegName;                // variable name text
    UCHAR                       Type;                   // NdisParameterInteger/NdisParameterHexInteger
    UINT                        StructOffset;           // Offset of structure from main object of this layer
    UINT                        FieldOffset;            // offset inside the structure to save this data in
    UINT                        FieldSize;              // size (in bytes) of the field
    UINT                        Default;                // default value to use
    UINT                        Min;                    // minimum value allowed
    UINT                        Max;                    // maximum value allowed
} MP_REG_ENTRY, *PMP_REG_ENTRY;


#define RSNA_OUI_PREFIX             0xac0f00
#define WPA_OUI_PREFIX              0xf25000

#define RSNA_CIPHER_WEP40           0x01000000
#define RSNA_CIPHER_TKIP            0x02000000
#define RSNA_CIPHER_CCMP            0x04000000
#define RSNA_CIPHER_WEP104          0x05000000

#define RSNA_AKM_RSNA               0x01000000
#define RSNA_AKM_RSNA_PSK           0x02000000

#define RSNA_CAPABILITY_PRE_AUTH    0x01
#define RSNA_CAPABILITY_NO_PAIRWISE 0x02

//
// This structure is used for extracting RSN IE in beacon or probe response frame.
// Both RSN (WPA2) and WPA share the same structure. However, for WPA, PMKIDCount
// is also 0 and PKMID NULL.
// 
typedef struct _RSN_IE_INFO
{
    ULONG   OUIPrefix;
    USHORT  Version;
    USHORT  GroupCipherCount;
    USHORT  PairwiseCipherCount;
    USHORT  AKMSuiteCount;
    USHORT  Capability;
    USHORT  PMKIDCount;
    PUCHAR  GroupCipher;
    PUCHAR  PairwiseCipher;
    PUCHAR  AKMSuite;
    PUCHAR  PMKID;
} RSN_IE_INFO, *PRSN_IE_INFO;


typedef struct _QUEUE_ENTRY
{
    struct _QUEUE_ENTRY *Next;
} QUEUE_ENTRY, *PQUEUE_ENTRY;

/** Structure holds for passing BSS information among the various layer */
typedef struct _MP_BSS_DESCRIPTION
{
    DOT11_BSS_TYPE  BSSType;

    DOT11_MAC_ADDRESS BSSID;

    DOT11_MAC_ADDRESS MacAddress;

    USHORT          BeaconPeriod;

    ULONGLONG       Timestamp;

    DOT11_CAPABILITY Capability;

    ULONG           PhyId;
    UCHAR           Channel;    // Valid only if it is non-zero

    // Size of the beacon IEs blob
    ULONG           BeaconIEBlobSize;
    // Offset of the start of beacon IEs in the IEBlobs
    ULONG           BeaconIEBlobOffset;

    // Size of the probe response IEs blob
    ULONG           ProbeIEBlobSize;
    // Offset of the start of probe response IEs in the IE Blobs
    ULONG           ProbeIEBlobOffset;

    // Space to hold beacon and probe response IE information
    ULONG           IEBlobsSize;
    UCHAR           IEBlobs[1];              // Must be the last field.

} MP_BSS_DESCRIPTION, *PMP_BSS_DESCRIPTION;


/** Scan request was initiated by an OS OID call */
#define MP_SCAN_REQUEST_OS_ISSUED   0x00000001

/** Structure used for passing scan requests to the driver */
typedef struct _MP_SCAN_REQUEST
{
    /** The original scan request from the OS */
    PDOT11_SCAN_REQUEST_V2      Dot11ScanRequest;
    ULONG                       ScanRequestFlags;
    ULONG                       ScanRequestBufferLength;

    /** The list of channels to scan & on which PHY*/
    PLONG                       ChannelList;
    ULONG                       ChannelCount;
    ULONG                       PhyId;
}MP_SCAN_REQUEST, *PMP_SCAN_REQUEST;

/**
 * Common header structure for management packets and short data packets
 */
typedef struct DOT11_DATA_SHORT_HEADER DOT11_MGMT_DATA_MAC_HEADER, *PDOT11_MGMT_DATA_MAC_HEADER;

ULONG
FASTCALL
MpInterlockedSetClearBits (
    _Inout_ PULONG               Flags,
    _In_ ULONG                    SetFlag,
    _In_ ULONG                    ClearFlag
    );

/*
 * Increments the value of pCurrentValue by 1 and ensures that it does
 * not reach (and cross) the WrapLimit.
 *
 * \return Returns the original value of pCurrentValue
 */
LONG 
MpInterlockedIncrementWrapped(
    PLONG   pCurrentValue, 
    LONG    WrapLimit
    );


#define MP_INTERLOCKED_SET_FLAG(_Flags, _SetFlag) \
    InterlockedOr((PLONG)(_Flags), (_SetFlag))

#define MP_INTERLOCKED_CLEAR_FLAG(_Flags, _ClearFlag) \
    InterlockedAnd((PLONG)(_Flags), ~(_ClearFlag))

#define MP_INTERLOCKED_SET_CLEAR_FLAG(_Flags, _SetFlag, _ClearFlag) \
    MpInterlockedSetClearBits((PLONG)(_Flags), (_SetFlag), (_ClearFlag))

#define MP_TEST_FLAG(_Flags, _TestFlag) (((_Flags) & (_TestFlag)) != 0)
#define MP_TEST_ALL_FLAGS(_Flags, _TestFlags)   (((_Flags) & (_TestFlags)) == (_TestFlags))


#define MP_SET_FLAG(_Flags, _SetFlag)       ((_Flags) |= (_SetFlag))   
#define MP_CLEAR_FLAG(_Flags, _ClearFlag)   ((_Flags) &= ~(_ClearFlag))
#define MP_CLEAR_ALL_FLAGS(_Flags)          ((_Flags) = 0)

// Increment, but return to zero if u reach/cross the limit
#define MP_INCREMENT_LIMIT_UNSAFE(_Counter, _Limit)         \
{                                           \
    _Counter++;                             \
    if (_Counter >= _Limit)                 \
        _Counter = 0;                       \
}

// Increment, but return to zero if u reach/cross the limit
#define MP_INCREMENT_LIMIT_SAFE(_Counter, _Limit)         \
    MpInterlockedIncrementWrapped((PLONG)&_Counter, (LONG)_Limit)


/** Returns success of the MAC addresses are equal, else failure */
#define MP_COMPARE_MAC_ADDRESS(_MacAddr1, _MacAddr2)    \
    ((RtlCompareMemory(_MacAddr1, _MacAddr2, sizeof(DOT11_MAC_ADDRESS)) == sizeof(DOT11_MAC_ADDRESS)) ? TRUE : FALSE)

#define MP_ACQUIRE_SPINLOCK(_Lock, _DispatchLevel)          \
{                                                           \
    if (_DispatchLevel)                                     \
    {                                                       \
        NdisDprAcquireSpinLock(&(_Lock));                   \
    }                                                       \
    else                                                    \
    {                                                       \
        NdisAcquireSpinLock(&(_Lock));                      \
    }                                                       \
}

#define MP_RELEASE_SPINLOCK(_Lock, _DispatchLevel)          \
{                                                           \
    if (_DispatchLevel)                                     \
    {                                                       \
        NdisDprReleaseSpinLock(&(_Lock));                   \
    }                                                       \
    else                                                    \
    {                                                       \
        NdisReleaseSpinLock(&(_Lock));                      \
    }                                                       \
}

#define MP_PEEK_LIST_HEAD(_ListHead)                        \
    ((_ListHead)->Flink)

//
//  Handy macros for doing pointer arithmetic
//
#ifndef Add2Ptr
#define Add2Ptr(P,I) ((PVOID)((PUCHAR)(P) + (I)))
#endif

#ifndef PtrOffset
#define PtrOffset(B,O) ((ULONG)((ULONG_PTR)(O) - (ULONG_PTR)(B)))
#endif

#ifndef MIN
    #define MIN(a, b)                  ((a) < (b)? a : b)
#endif  // MIN
#ifndef MAX
    #define MAX(a, b)                  ((a) > (b)? a : b)
#endif  // MAX

//
// Unicast address
//
#define DOT11_IS_UNICAST(_Address) (!ETH_IS_MULTICAST(_Address) && !ETH_IS_BROADCAST(_Address))

//
// Broadcast address
//
#define DOT11_IS_BROADCAST(_Address) (ETH_IS_BROADCAST(_Address))

//
// Multicast address
//
#define DOT11_IS_MULTICAST(_Address) (ETH_IS_MULTICAST(_Address))

//
// Macros for assigning and verifying NDIS_OBJECT_HEADER
//
#define MP_ASSIGN_NDIS_OBJECT_HEADER(_header, _type, _revision, _size) \
    (_header).Type = _type; \
    (_header).Revision = _revision; \
    (_header).Size = _size; 

// 
// With NDIS 6.0 header versioning, the driver should allow higher versions
// of structures to be set. This macro verifies that for sets the version is atleast
// the expected one and size is greater or equal to required
//
#define MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(_header, _type, _revision, _size) \
    (((_header).Type == _type) && \
     ((_header).Revision >= _revision) && \
     ((_header).Size >= _size))

//
// This macro performs exact matching of the NDIS_OBJECT_HEADER
//
#define MP_VERIFY_NDIS_OBJECT_HEADER_PERFECT(_header, _type, _revision, _size) \
    (((_header).Type == _type) && \
     ((_header).Revision == _revision) && \
     ((_header).Size == _size))


// Generic callback function into the port. This can be called into
// by any layer (HW/HVL/VNic). The Data parameter is only guaranteed to be
// valid until the callback returns. If the callback function wants access to it later,
// it must copy it locally
typedef NDIS_STATUS (*PORT11_GENERIC_CALLBACK_FUNC)(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   Data
    );


// Generic callback function into the VNIC. This can be called into
// by any layer (HW/HVL/Port). The Data parameter is only guaranteed to be
// valid until the callback returns. If the callback function wants access to it later,
// it must copy it locally
typedef NDIS_STATUS (*VNIC11_GENERIC_CALLBACK_FUNC)(
    _In_  PVNIC                   VNic,
    _In_  PVOID                   Ctx,
    _In_  PVOID                   Data
    );

