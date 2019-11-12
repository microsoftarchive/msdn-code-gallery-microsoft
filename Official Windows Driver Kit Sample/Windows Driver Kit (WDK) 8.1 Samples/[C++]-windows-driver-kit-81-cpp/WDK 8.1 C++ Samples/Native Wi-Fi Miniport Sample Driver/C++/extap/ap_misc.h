/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_misc.h

Abstract:
    Contains ExtAP internal supporting functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    10-10-2007    Created

Notes:

--*/

#pragma once

#ifndef _AP_MISC_H
#define _AP_MISC_H

/** 
 * DOT11 Frame Class
 * See definitions in 802.11 standard.
 */
typedef enum _DOT11_FRAME_CLASS
{
    DOT11_FRAME_CLASS_1,
    DOT11_FRAME_CLASS_2,
    DOT11_FRAME_CLASS_3,
    DOT11_FRAME_CLASS_INVALID
} DOT11_FRAME_CLASS, *PDOT11_FRAME_CLASS;

DOT11_FRAME_CLASS
FORCEINLINE
Dot11GetFrameClass(
    _In_ PDOT11_FRAME_CTRL FrameControl
    )
{
    DOT11_FRAME_CLASS frameClass = DOT11_FRAME_CLASS_INVALID;

    switch(FrameControl->Type)
    {
        case DOT11_FRAME_TYPE_CONTROL:
            switch(FrameControl->Subtype)
            {
                case DOT11_CTRL_SUBTYPE_RTS:
                case DOT11_CTRL_SUBTYPE_CTS:
                case DOT11_CTRL_SUBTYPE_ACK:
                case DOT11_CTRL_SUBTYPE_CFE_CFA:
                case DOT11_CTRL_SUBTYPE_CFE:
                    frameClass = DOT11_FRAME_CLASS_1;
                    break;
                    
                case DOT11_CTRL_SUBTYPE_PS_POLL:
                    frameClass = DOT11_FRAME_CLASS_3;
                    break;
            }
            break;

        case DOT11_FRAME_TYPE_MANAGEMENT:
            switch(FrameControl->Subtype)
            {
                case DOT11_MGMT_SUBTYPE_PROBE_REQUEST:
                case DOT11_MGMT_SUBTYPE_PROBE_RESPONSE:
                case DOT11_MGMT_SUBTYPE_BEACON:
                case DOT11_MGMT_SUBTYPE_AUTHENTICATION:
                case DOT11_MGMT_SUBTYPE_DEAUTHENTICATION:
                case DOT11_MGMT_SUBTYPE_ATIM:
                    frameClass = DOT11_FRAME_CLASS_1;
                    break;
                    
                case DOT11_MGMT_SUBTYPE_ASSOCIATION_REQUEST:
                case DOT11_MGMT_SUBTYPE_ASSOCIATION_RESPONSE:
                case DOT11_MGMT_SUBTYPE_REASSOCIATION_REQUEST:
                case DOT11_MGMT_SUBTYPE_REASSOCIATION_RESPONSE:
                    case DOT11_MGMT_SUBTYPE_DISASSOCIATION:
                    frameClass = DOT11_FRAME_CLASS_2;
                    break;
            }
            break;

        case DOT11_FRAME_TYPE_DATA:
            if (FrameControl->ToDS == 0 && FrameControl->FromDS == 0)
            {
                frameClass = DOT11_FRAME_CLASS_1;
            }
            else
            {
                frameClass = DOT11_FRAME_CLASS_3;
            }
            break;
    }

    return frameClass;
}

/** Hardware related */

/** Get hardware capability */
NDIS_STATUS
ApInitializeCapability(
    _In_ PMP_EXTAP_PORT ApPort
    );

/** Deinitialize hardware capability */
VOID
ApDeinitializeCapability(
    _In_ PAP_CAPABIITY Capability
    );

/** Registry settings */
/** 
 * Read AP settings from registry
 * A default value is used if
 *     1. The registry vlaue is not present, or
 *     2. The registry value is not valid based on the hardware capability
 */
VOID
ApInitializeRegInfo(
    _In_ PMP_EXTAP_PORT ApPort
    );

/** Deinitialize registry info */
VOID
ApDeinitializeRegInfo(
    _In_ PAP_REG_INFO RegInfo
    );

/** commonly used supporting functions */

/** Get AP state */
AP_STATE
FORCEINLINE
ApGetState(
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    return (AP_STATE)InterlockedExchangeAdd(
                (LONG *)&ApPort->State,
                0
                );
}

/** Set AP state, return the old state */
AP_STATE
FORCEINLINE
ApSetState(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ AP_STATE NewState
    )
{
    return (AP_STATE)InterlockedExchange(
                (LONG *)&ApPort->State,
                (LONG)NewState
                );
}

/** Reference AP port */
LONG
FORCEINLINE
ApRefPort(
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    return InterlockedIncrement(&ApPort->RefCount);
}

/** Dereference AP port */
LONG
FORCEINLINE
ApDerefPort(
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    return InterlockedDecrement(&ApPort->RefCount);
}

/** Dot11 status indication */
// TODO: shall we share the function with station
VOID 
ApIndicateDot11Status(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ NDIS_STATUS StatusCode,
    _In_opt_ PVOID RequestID,
    _In_ PVOID StatusBuffer,
    _In_ ULONG StatusBufferSize
    );


/** 
 * Functions that can be invoked before AP port is created 
 */

/** Query supported unicast auth/cipher algorithm pairs */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
ApQuerySupportedUnicastAlgoPairs(
    _In_ PMP_PORT Port,
    _Out_writes_opt_(PairCount) PDOT11_AUTH_CIPHER_PAIR AuthCipherPairs,
    _In_ ULONG PairCount,
    _Out_ PULONG PairCountWritten,
    _Out_when_invalid_ndis_length_ PULONG PairCountNeeded
    );

/** Query supported multicast auth/cipher algorithm pairs */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
ApQuerySupportedMulticastAlgoPairs(
    _In_ PMP_PORT Port,
    _Out_opt_ PDOT11_AUTH_CIPHER_PAIR AuthCipherPairs,
    _In_ ULONG BufferLength,
    _Out_ PULONG BytesWritten,
    _Out_ PULONG BytesNeeded
    );

/** Validate auth algorithm */
BOOLEAN
ApValidateAuthAlgo(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ DOT11_AUTH_ALGORITHM AuthAlgo
    );

/** Validate unicate auth/cipher algorithms */
BOOLEAN
ApValidateUnicastAuthCipherPair(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ DOT11_AUTH_ALGORITHM AuthAlgo,
    _In_ DOT11_CIPHER_ALGORITHM CipherAlgo
    );

/** Validate multicate auth/cipher algorithms */
BOOLEAN
ApValidateMulticastAuthCipherPair(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ DOT11_AUTH_ALGORITHM AuthAlgo,
    _In_ DOT11_CIPHER_ALGORITHM CipherAlgo
    );

/** 
 * Calculate required list size 
 * Return FALSE if overflow
 */
BOOLEAN
FORCEINLINE
GetRequiredListSize(
    _In_ ULONG ElemOffset,
    _In_ ULONG ElemSize,
    _In_ ULONG ElemCount,
    _Out_ PULONG RequiredSize
    )
{
    ElemCount = (ElemCount == 0) ? 1 : ElemCount;
    
    if (RtlULongMult(ElemSize, ElemCount, RequiredSize) != STATUS_SUCCESS)
    {
        return FALSE;
    }

    if (RtlULongAdd(*RequiredSize, ElemOffset, RequiredSize) != STATUS_SUCCESS)
    {
        return FALSE;
    }

    return TRUE;
}

/**
 * Validate the size of a SSID list.
 * Return NDIS_STATUS_INVALID_LENGTH if the list size is less than the needed size.
 * Return NDIS_STATUS_SUCCESS is returned if the list size is equal to or greater than the needed size.
 * SizeNeeded contains the required size in these cases.
 * Return NDIS_STATUS_INVALID_DATA if size calculation overflows.
 */
NDIS_STATUS
ValiateSsidListSize(
    _In_ PDOT11_SSID_LIST SsidList,
    _In_ ULONG ListSize,
    _Out_ PULONG SizeNeeded
    );

/**
 * Validate the size of a privacy exemption list.
 * Return NDIS_STATUS_INVALID_LENGTH if the list size is less than the needed size.
 * Return NDIS_STATUS_SUCCESS is returned if the list size is equal to or greater than the needed size.
 * SizeNeeded contains the required size in these cases.
 * Return NDIS_STATUS_INVALID_DATA if size calculation overflows.
 */
NDIS_STATUS
ValiatePrivacyExemptionListSize(
    _In_ PDOT11_PRIVACY_EXEMPTION_LIST PrivacyExemptionList,
    _In_ ULONG ListSize,
    _Out_ PULONG SizeNeeded
    );

/**
 * Validate the size of a auth algorithm list.
 * Return NDIS_STATUS_INVALID_LENGTH if the list size is less than the needed size.
 * Return NDIS_STATUS_SUCCESS is returned if the list size is equal to or greater than the needed size.
 * SizeNeeded contains the required size in these cases.
 * Return NDIS_STATUS_INVALID_DATA if size calculation overflows.
 */
NDIS_STATUS
ValiateAuthAlgorithmListSize(
    _In_ PDOT11_AUTH_ALGORITHM_LIST AuthAlgorithmList,
    _In_ ULONG ListSize,
    _Out_ PULONG SizeNeeded
    );

/**
 * Validate the size of a cipher algorithm list.
 * Return NDIS_STATUS_INVALID_LENGTH if the list size is less than the needed size.
 * Return NDIS_STATUS_SUCCESS is returned if the list size is equal to or greater than the needed size.
 * SizeNeeded contains the required size in these cases.
 * Return NDIS_STATUS_INVALID_DATA if size calculation overflows.
 */
NDIS_STATUS
ValiateCipherAlgorithmListSize(
    _In_ PDOT11_CIPHER_ALGORITHM_LIST CipherAlgorithmList,
    _In_ ULONG ListSize,
    _Out_ PULONG SizeNeeded
    );

/**
 * Validate the size of a cipher default key.
 * Return NDIS_STATUS_INVALID_LENGTH if the key size is less than the needed size.
 * Return NDIS_STATUS_SUCCESS is returned if the key size is equal to or greater than the needed size.
 * SizeNeeded contains the required size in these cases.
 * Return NDIS_STATUS_INVALID_DATA if size calculation overflows.
 */
NDIS_STATUS
ValiateCipherDefaultKeySize(
    _In_ PDOT11_CIPHER_DEFAULT_KEY_VALUE CipherDefaultKey,
    _In_ ULONG KeySize,
    _Out_ PULONG SizeNeeded
    );


/**
 * Validate the size of a DOT11_BYTE_ARRAY.
 * Return NDIS_STATUS_INVALID_LENGTH if the buffer size is less than the needed size.
 * Return NDIS_STATUS_SUCCESS otherwise.
 */
NDIS_STATUS
ValidateDot11ByteArray(
    _In_ PDOT11_BYTE_ARRAY pDot11ByteArray,
    _In_ ULONG ArraySize,
    _Out_ PULONG SizeNeeded
    );

/**
 * Validate the size of a PHY ID list.
 * Return NDIS_STATUS_INVALID_LENGTH if the list size is less than the needed size.
 * Return NDIS_STATUS_SUCCESS is returned if the list size is equal to or greater than the needed size.
 * SizeNeeded contains the required size in these cases.
 * Return NDIS_STATUS_INVALID_DATA if size calculation overflows.
 */
NDIS_STATUS
ValiatePhyIdListSize(
    _In_ PDOT11_PHY_ID_LIST PhyIdList,
    _In_ ULONG ListSize,
    _Out_ PULONG SizeNeeded
    );

/**
 * Validate the size of additional IEs.
 * Return NDIS_STATUS_INVALID_LENGTH if the IE size is less than the needed size.
 * Return NDIS_STATUS_SUCCESS is returned if the IE size is equal to or greater than the needed size.
 * SizeNeeded contains the required size in these cases.
 * Return NDIS_STATUS_INVALID_DATA if size calculation overflows.
 */
NDIS_STATUS
ValiateAdditionalIeSize(
    _In_ PDOT11_ADDITIONAL_IE IeData,
    _In_ ULONG IeSize,
    _Out_ PULONG SizeNeeded
    );

/** Commonly used MACROs */

/** Set BytesNeeded to the needed buffer size and bail with corresponding NDIS status */
#define SET_NEEDED_BUFFER_SIZE_AND_BREAK(NeededSize) \
        { \
            *BytesNeeded = (NeededSize); \
            ndisStatus = NDIS_STATUS_INVALID_LENGTH; \
            break; \
        }

/**
 * Validate supported cipher algorithm
 */
BOOLEAN
IsSupportedCipher(
    _In_ PMP_EXTAP_PORT ApPort,
    _In_ ULONG AlgorithmId,
    _In_ USHORT KeyLength
    );

VOID 
ApIndicateFrequencyAdoped(
    _In_ PMP_EXTAP_PORT ApPort
    );

#endif  // _AP_MISC_H
