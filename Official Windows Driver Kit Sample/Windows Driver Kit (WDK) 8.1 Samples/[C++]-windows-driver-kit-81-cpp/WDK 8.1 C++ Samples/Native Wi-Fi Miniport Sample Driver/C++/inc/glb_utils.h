/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    glb_utils.h

Abstract:
    Contains utility functions used by the whole driver
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

ULONG
MpReadRegistry(
    _In_        PVOID                   StructureStart,
    _In_opt_    NDIS_HANDLE             ConfigurationHandle,
    _In_        PMP_REG_ENTRY           RegKeyList,
    _In_        ULONG                   NumKeys
    );

NDIS_STATUS
Dot11CopyMdlToBuffer(
    _Inout_updates_(_Inexpressible_("Varies")) PNDIS_BUFFER * ppMdlChain,
    ULONG uOffset,
    PVOID pvBuffer,
    ULONG uBytesToCopy,
    PULONG puLastWalkedMdlOffset,
    PULONG puBytesCopied
    );

NDIS_STATUS
Dot11GetDataFromMdlChain(
    _Inout_updates_(_Inexpressible_("Varies")) PNDIS_BUFFER * ppMdlChain,
    ULONG uOffset,
    ULONG uBytesNeeded,
    PVOID pvStorage,
    PULONG puLastWalkedMdlOffset,
    PUCHAR * ppucReturnBuf
    );

NDIS_STATUS
Dot11GetMacHeaderFromNB(
    _In_  PNET_BUFFER                     NetBuffer,
    _In_  PDOT11_MAC_HEADER*              ppDot11MacHeader
    );

PVOID
Dot11GetNetBufferData(
    _In_  PNET_BUFFER                     NetBuffer,
    _In_  ULONG                           BytesNeeded
    );



#define WPA_IE_TAG      0x01f25000
#define WCN_IE_TAG      0x04f25000

//
// Dot11ValidateInfoBlob and Dot11ValidatePacketInfoBlob
//  Validate the information blob. Dot11ValidateInfoBlob works
//  on a contiguous buffer while Dot11ValidatePacketInfoBlob
//  works on a MDL chain.
//
NDIS_STATUS
Dot11ValidateInfoBlob(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob
    );

NDIS_STATUS
Dot11GetInfoBlobSize(
    _In_reads_bytes_(PacketLength)  PUCHAR                  pPacketBuffer,
    _In_  ULONG                   PacketLength,
    _In_  ULONG                   OffsetOfInfoElemBlob,
    _Out_ PULONG                  pInfoElemBlobSize
    );

NDIS_STATUS
Dot11ValidatePacketInfoBlob(
    _In_ PNDIS_BUFFER pMdlHead,
    _In_ ULONG uOffsetOfInfoEleBlob
    );

//
// Dot11GetInfoEle and Dot11GetInfoEleFromPacket
//      Look up a particular information element in the
//      information blob.
//
// Dot11GetInfoEle works on a contiguous buffer while
// Dot11GetInfoEleFromPacket works on a MDL chain
//
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11GetInfoEle(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob,
    _In_ UCHAR ucInfoId,
    _Out_ PUCHAR pucLength,
    _Outptr_result_bytebuffer_(*pucLength) PVOID * ppvInfoEle
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11GetInfoEleFromPacket(
    _In_    PNDIS_BUFFER pMdlHead,
    _In_    ULONG uOffsetOfInfoEleBlob,
    _In_    UCHAR ucInfoId,
    _In_    UCHAR ucMaxLength,
    _Out_   PUCHAR pucLength,
    _Inout_ PVOID * ppvInfoEle
    );

NDIS_STATUS
Dot11CopySSIDFromPacket(
    _In_ PNDIS_BUFFER pMdlHead,
    _In_ ULONG uOffsetOfInfoEleBlob,
    _Inout_ PDOT11_SSID pDot11SSID
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11GetChannelForDSPhy(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob,
    _Out_ PUCHAR pucChannel
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11CopySSIDFromMemoryBlob(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob,
    _Out_ PDOT11_SSID pDot11SSID
    );

PUCHAR
Dot11GetBSSID(
    _In_reads_bytes_(fragmentLength) PUCHAR fragmentHeader,
    _In_ ULONG fragmentLength
    );

_Success_(return == NDIS_STATUS_SUCCESS)
_At_(*pusBlobSize, _In_range_(>=, 2U + ucElementLength))
_At_(*ppucBlob, _Writes_and_advances_ptr_(*pusBlobSize))
NDIS_STATUS
Dot11AttachElement(   
    _Inout_ PUCHAR *ppucBlob,
    _Inout_ USHORT *pusBlobSize,        
    _In_    UCHAR ucElementId,
    _In_    UCHAR ucElementLength,
    _In_reads_bytes_(ucElementLength)
            PUCHAR pucElementInfo
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11CopyInfoEle(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob,
    _In_ UCHAR ucInfoId,
    _Out_ PUCHAR pucLength,
    _In_ ULONG uBufSize,
    _Out_ PVOID pvInfoEle
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11GetWPAIE(
    _In_reads_bytes_(uSizeOfBlob) PUCHAR pucInfoBlob,
    _In_ ULONG uSizeOfBlob,
    _Out_ PUCHAR pucLength,
    _Outptr_result_bytebuffer_(*pucLength) PVOID * ppvInfoEle
    );

BOOLEAN
Dot11IsHiddenSSID(
    _In_reads_bytes_(SSIDLength) PUCHAR               SSID,
    _In_ ULONG                SSIDLength
    );

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
Dot11GetRateSetFromInfoEle(
    _In_reads_bytes_(InfoElemBlobSize)  PUCHAR InfoElemBlobPtr,
    _In_  ULONG InfoElemBlobSize,
    _In_  BOOLEAN basicRateOnly,
    _Inout_ PDOT11_RATE_SET rateSet
    );

VOID
Dot11GenerateRandomBSSID(
    _In_reads_bytes_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS MACAddr,
    _Out_writes_bytes_(DOT11_ADDRESS_SIZE) DOT11_MAC_ADDRESS BSSID
    );

NDIS_STATUS
Dot11ParseRNSIE(
    _In_reads_bytes_(RSNIELength) PUCHAR RSNIEData,
    _In_ ULONG OUIPrefix,
    _In_ UCHAR RSNIELength,
    _Out_ PRSN_IE_INFO RSNIEInfo
    );

DOT11_CIPHER_ALGORITHM
Dot11GetGroupCipherFromRSNIEInfo(
    _In_ PRSN_IE_INFO RSNIEInfo
    );

DOT11_CIPHER_ALGORITHM
Dot11GetPairwiseCipherFromRSNIEInfo(
    _In_ PRSN_IE_INFO RSNIEInfo,
    _In_ USHORT index
    );

DOT11_AUTH_ALGORITHM
Dot11GetAKMSuiteFromRSNIEInfo(
    _In_ PRSN_IE_INFO RSNIEInfo,
    _In_ USHORT index
    );

ULONG
Dot11GetRSNOUIFromCipher(
    _In_ ULONG prefix,
    _In_ DOT11_CIPHER_ALGORITHM cipher
    );

ULONG
Dot11GetRSNOUIFromAuthAlgo(
    _In_ DOT11_AUTH_ALGORITHM algo
    );

NDIS_STATUS
MpCreateThread(
    _In_ PKSTART_ROUTINE StartFunc,
    _In_ PVOID Context,
    _In_ KPRIORITY Priority,
    _Outptr_ PKTHREAD* Thread
    );
