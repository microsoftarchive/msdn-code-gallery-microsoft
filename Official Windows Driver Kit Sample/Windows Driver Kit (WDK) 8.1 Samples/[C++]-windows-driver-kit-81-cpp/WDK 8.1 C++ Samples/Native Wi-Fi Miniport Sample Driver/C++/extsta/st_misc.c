/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_misc.c

Abstract:
    STA layer utility functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_misc.c to st_misc.c

Notes:

--*/
#include "precomp.h"
#include "st_misc.h"

#if DOT11_TRACE_ENABLED
#include "st_misc.tmh"
#endif

#define PHY_TYPE_BUFFER_SIZE    (sizeof(DOT11_SUPPORTED_PHY_TYPES) + \
                                 sizeof(DOT11_PHY_TYPE) * STA_DESIRED_PHY_MAX_COUNT)

NDIS_STATUS
StaFilterUnsupportedRates(
    _In_ PMP_BSS_ENTRY pAPEntry, 
    _In_ PDOT11_RATE_SET rateSet
    )
{
    NDIS_STATUS     ndisStatus;
    DOT11_RATE_SET  APRateSet = {0};
    ULONG           i;
    ULONG           j;

    //
    // Get the rate set supported by AP from its beacon
    //
    ndisStatus = Dot11GetRateSetFromInfoEle(pAPEntry->pDot11InfoElemBlob,
                                          pAPEntry->InfoElemBlobSize,
                                          FALSE, 
                                          &APRateSet);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
        return ndisStatus;

    //
    // Filter out any rates that are not supported by AP
    //
    i = 0;
    while (i < rateSet->uRateSetLength) 
    {
        for (j = 0; j < APRateSet.uRateSetLength; j++) 
        {
            if ((rateSet->ucRateSet[i] & 0x7f) == (APRateSet.ucRateSet[j] & 0x7f))
                break;
        }

        //
        // remove the rate if it is not in AP's rate set
        //
        if (j == APRateSet.uRateSetLength)
        {
            rateSet->uRateSetLength--;
            rateSet->ucRateSet[i] = rateSet->ucRateSet[rateSet->uRateSetLength];
        }
        else
        {
            i++;
        }
    }

    //
    // We must have at least one good data rate
    //
    if (rateSet->uRateSetLength < 1)
        return NDIS_STATUS_FAILURE;

    return NDIS_STATUS_SUCCESS;
}           

ULONG
StaGetPhyIdByType(
    _In_ PMP_EXTSTA_PORT pStation,
    _In_ DOT11_PHY_TYPE PhyType
    )
{
    ULONG phyId = BasePortGetPhyIdFromType(STA_GET_MP_PORT(pStation), PhyType);

    // If the translation did not succeed, this is an invalid phy id
    if (phyId == DOT11_PHY_ID_ANY)
        return STA_INVALID_PHY_ID;

    return phyId;
}

DOT11_PHY_TYPE
StaGetPhyTypeById(
    _In_ PMP_EXTSTA_PORT pStation,
    _In_ ULONG PhyId
    )
{
    return BasePortGetPhyTypeFromId(STA_GET_MP_PORT(pStation), PhyId);
}


_At_(*ppCurrentIE, _Writes_and_advances_ptr_(*pIESize))
NDIS_STATUS
StaAttachInfraRSNIE(
    _In_    PMP_EXTSTA_PORT pStation, 
    _In_    PMP_BSS_ENTRY pAPEntry, 
    _Inout_ PUCHAR *ppCurrentIE,
    _Inout_ PUSHORT pIESize
    )
{
    USHORT      size;
    NDIS_STATUS ndisStatus;
    UCHAR       SecIELength = 0;
    PUCHAR      SecIEData = NULL;
    RSN_IE_INFO RSNIEInfo;
    UCHAR       IEId;
    ULONG       pmkidIndex = 0;
    BOOLEAN     attachPMKID = FALSE;

    //
    // Get RSNIEInfo from AP
    //
    if (pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA ||
        pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK)
    {
        IEId = DOT11_INFO_ELEMENT_ID_RSN;
        ndisStatus = Dot11GetInfoEle(pAPEntry->pDot11InfoElemBlob,
                                     pAPEntry->InfoElemBlobSize,
                                     DOT11_INFO_ELEMENT_ID_RSN,
                                     &SecIELength,
                                     (PVOID)&SecIEData);

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            return ndisStatus;
        }

        ndisStatus = Dot11ParseRNSIE(SecIEData, RSNA_OUI_PREFIX, SecIELength, &RSNIEInfo);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            return ndisStatus;
        }
    }
    else 
    {
        IEId = DOT11_INFO_ELEMENT_ID_VENDOR_SPECIFIC;
        ndisStatus = Dot11GetWPAIE(pAPEntry->pDot11InfoElemBlob,
                                   pAPEntry->InfoElemBlobSize,
                                   &SecIELength,
                                   (PVOID)&SecIEData);

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            return ndisStatus;
        }

        ndisStatus = Dot11ParseRNSIE(SecIEData, WPA_OUI_PREFIX, SecIELength, &RSNIEInfo);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            return ndisStatus;
        }
    }

    //
    // Our RSN IE will contain 1 group cipher, 1 pairwise cipher, 1 AKM suite, and capability.
    // In addition, WPA IE has 4 bytes WPA_IE_TAG, RNSA IE has PMKIDs.
    //
    size = sizeof(DOT11_INFO_ELEMENT) +                 // IE ID and length
           sizeof(USHORT) +                             // version
           sizeof(ULONG) +                              // group cipher
           sizeof(USHORT) + sizeof(ULONG) +             // pairwise cipher
           sizeof(USHORT) + sizeof(ULONG) +             // AKM suite
           sizeof(USHORT);                              // capability.

    if (RSNIEInfo.OUIPrefix == WPA_OUI_PREFIX)
    {
        size += sizeof(ULONG);                          // WPA IE tag, 0x01f25000
    }
    else if (pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA &&
             pStation->Config.PMKIDCount > 0)
    {
        // walk through PMKIDList and locate a PMKID of which BSSID matchs AP's BSSID
        for (pmkidIndex = 0; pmkidIndex < pStation->Config.PMKIDCount; pmkidIndex++)
        {
            if (NdisEqualMemory(pStation->Config.PMKIDList[pmkidIndex].BSSID, pAPEntry->Dot11BSSID, sizeof(DOT11_MAC_ADDRESS)) == 1)
            {
                attachPMKID = TRUE;
                break;      // there should be at most one pmkid to attach
            }
        }

        if (attachPMKID == TRUE)
        {
            size += sizeof(USHORT) + sizeof(DOT11_PMKID_VALUE);
        }
    }

    ASSERT(size <= 255 + sizeof(DOT11_INFO_ELEMENT));
    if (*pIESize < size)
    {
        return STATUS_BUFFER_TOO_SMALL;
    }
    
    //
    // Set IE ID and length
    //
    ((DOT11_INFO_ELEMENT UNALIGNED *)(*ppCurrentIE))->ElementID = IEId;
    ((DOT11_INFO_ELEMENT UNALIGNED *)(*ppCurrentIE))->Length = (UCHAR)(size - sizeof(DOT11_INFO_ELEMENT));
    *ppCurrentIE += sizeof(DOT11_INFO_ELEMENT);

    //
    // Set WPA_IE_TAG for WPA IE
    //
    if (RSNIEInfo.OUIPrefix == WPA_OUI_PREFIX)
    {
        *((ULONG UNALIGNED *)(*ppCurrentIE)) = WPA_IE_TAG;
        *ppCurrentIE += sizeof(ULONG);
    }

    //
    // Set version
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = 1;
    *ppCurrentIE += sizeof(USHORT);

    //
    // If AP RSN IE did not contain a group cipher, we wont add any
    // fields except version
    //
    if (RSNIEInfo.GroupCipher == NULL)
    {
        //
        // Update remaining IE size.
        //
        *pIESize = *pIESize - size;
        return NDIS_STATUS_SUCCESS;
    }

    //
    // Set group cipher
    //
    *((ULONG UNALIGNED *)(*ppCurrentIE)) = *((ULONG UNALIGNED *)RSNIEInfo.GroupCipher);
    *ppCurrentIE += sizeof(ULONG);

    //
    // Temoporarily set our enabled multicast cipher. We will notify hardware layer of this selection
    // when association is successful.
    //
    pStation->Config.MulticastCipherAlgorithm = Dot11GetGroupCipherFromRSNIEInfo(&RSNIEInfo);

    //
    // Set pairwise cipher
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = 1;
    *ppCurrentIE += sizeof(USHORT);
    *((ULONG UNALIGNED *)(*ppCurrentIE)) = Dot11GetRSNOUIFromCipher(RSNIEInfo.OUIPrefix, 
                                                                  pStation->Config.UnicastCipherAlgorithm);
    *ppCurrentIE += sizeof(ULONG);

    //
    // Set AKM suite
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = 1;
    *ppCurrentIE += sizeof(USHORT);
    *((ULONG UNALIGNED *)(*ppCurrentIE)) = Dot11GetRSNOUIFromAuthAlgo(pStation->Config.AuthAlgorithm);
    *ppCurrentIE += sizeof(ULONG);

    //
    // Set capability. Get the capability from hardware layer.
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = VNic11QueryRSNCapabilityField(STA_GET_VNIC(pStation));
    *ppCurrentIE += sizeof(USHORT);

    //
    // Set PMKID
    //
    if (attachPMKID == TRUE)
    {
        ASSERT(pmkidIndex < pStation->Config.PMKIDCount);

        *((USHORT UNALIGNED *)(*ppCurrentIE)) = 1;  // one pmkid at most to attach
        *ppCurrentIE += sizeof(USHORT);

        NdisMoveMemory(*ppCurrentIE,
                        pStation->Config.PMKIDList[pmkidIndex].PMKID,
                        sizeof(DOT11_PMKID_VALUE));
        *ppCurrentIE += sizeof(DOT11_PMKID_VALUE);
    }

    //
    // Update remaining IE size.
    //
    *pIESize = *pIESize - size;

    return NDIS_STATUS_SUCCESS;
}

_At_(*pIESize, _In_range_(>=, sizeof(DOT11_INFO_ELEMENT) +
                              sizeof(USHORT) +
                              sizeof(ULONG) +
                              sizeof(USHORT) + sizeof(ULONG) +
                              sizeof(USHORT) + sizeof(ULONG) +
                              sizeof(USHORT)))
_At_(*ppCurrentIE, _Writes_and_advances_ptr_(*pIESize))    
NDIS_STATUS
StaAttachAdHocRSNIE(
    _In_    PMP_EXTSTA_PORT pStation, 
    _Inout_ PUCHAR *ppCurrentIE,
    _Inout_ PUSHORT pIESize
    )
{
    USHORT      size;

    ASSERT(pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK);

    //
    // Our RSN IE will contain 1 group cipher, 1 pairwise cipher, 1 AKM suite, and capability.
    //
    size = sizeof(DOT11_INFO_ELEMENT) +                 // IE ID and length
           sizeof(USHORT) +                             // version
           sizeof(ULONG) +                              // group cipher
           sizeof(USHORT) + sizeof(ULONG) +             // pairwise cipher
           sizeof(USHORT) + sizeof(ULONG) +             // AKM suite
           sizeof(USHORT);                              // capability.

    //
    // Set IE ID and length
    //
    ((DOT11_INFO_ELEMENT UNALIGNED *)(*ppCurrentIE))->ElementID = DOT11_INFO_ELEMENT_ID_RSN;
    ((DOT11_INFO_ELEMENT UNALIGNED *)(*ppCurrentIE))->Length = (UCHAR)(size - sizeof(DOT11_INFO_ELEMENT));
    *ppCurrentIE += sizeof(DOT11_INFO_ELEMENT);

    //
    // Set version
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = 1;
    *ppCurrentIE += sizeof(USHORT);

    //
    // Set group cipher
    //
    *((ULONG UNALIGNED *)(*ppCurrentIE)) = Dot11GetRSNOUIFromCipher(RSNA_OUI_PREFIX, 
                                                                  pStation->Config.MulticastCipherAlgorithm);
    *ppCurrentIE += sizeof(ULONG);

    //
    // Set pairwise cipher
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = 1;
    *ppCurrentIE += sizeof(USHORT);
    *((ULONG UNALIGNED *)(*ppCurrentIE)) = Dot11GetRSNOUIFromCipher(RSNA_OUI_PREFIX, 
                                                                  pStation->Config.UnicastCipherAlgorithm);
    *ppCurrentIE += sizeof(ULONG);

    //
    // Set AKM suite
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = 1;
    *ppCurrentIE += sizeof(USHORT);
    *((ULONG UNALIGNED *)(*ppCurrentIE)) = Dot11GetRSNOUIFromAuthAlgo(pStation->Config.AuthAlgorithm);
    *ppCurrentIE += sizeof(ULONG);

    //
    // Set capability. Get the capability from hardware layer.
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = VNic11QueryRSNCapabilityField(STA_GET_VNIC(pStation));
    *ppCurrentIE += sizeof(USHORT);

    //
    // Update remaining IE size.
    //
    *pIESize = *pIESize - size;

    return NDIS_STATUS_SUCCESS;
}

BOOLEAN
StaMatchRSNInfo(
    _In_ PMP_EXTSTA_PORT        pStation,
    _In_ PRSN_IE_INFO    RSNIEInfo
    )
{
    DOT11_CIPHER_ALGORITHM  cipher;
    DOT11_AUTH_ALGORITHM    authAlgo;
    ULONG                   index;

    //
    // Check the RSNIEInfo structure.
    // Only version 1 is supported
    //
    if (RSNIEInfo->Version != 1)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, 
                (" - Reject (AP is running unspported RSNA version %d)\n", 
                RSNIEInfo->Version));
        return FALSE;
    }

    //
    // In adhoc mode, make sure there is exactly one pairwise cipher, one AKM in
    // the IE. Also, the RSN capability should match ours and there should be no PMKID.
    //
    if (pStation->Config.BSSType == dot11_BSS_type_independent)
    {
        if (RSNIEInfo->PairwiseCipherCount != 1)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (AdHoc network does not specify exactly one pairwise cipher)\n"));
            return FALSE;
        }

        if (RSNIEInfo->AKMSuiteCount != 1)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (AdHoc network does not specify exactly one AKM)\n"));
            return FALSE;
        }

        if (RSNIEInfo->PMKIDCount != 0)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (AdHoc network uses PMKID)\n"));
            return FALSE;
        }
    }

    //
    // Check AP's group cipher is one of our enabled multicast ciphers.
    //
    ASSERT(RSNIEInfo->GroupCipherCount <= 1);
    if (RSNIEInfo->GroupCipherCount == 0)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (AP does not specify group cipher)\n"));
        return FALSE;
    }

    cipher = Dot11GetGroupCipherFromRSNIEInfo(RSNIEInfo);
    for (index = 0; index < pStation->Config.MulticastCipherAlgorithmCount; index++)
    {
        if (cipher == pStation->Config.MulticastCipherAlgorithmList[index])
            break;
    }

    if (index == pStation->Config.MulticastCipherAlgorithmCount)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (no matching group cipher)\n"));
        return FALSE;
    }

    //
    // Check AP's pairwise ciphers. At least one must match with our unicast cipher
    //
    for (index = 0; index < RSNIEInfo->PairwiseCipherCount; index++)
    {
        cipher = Dot11GetPairwiseCipherFromRSNIEInfo(RSNIEInfo, (USHORT)index);
        if (cipher == pStation->Config.UnicastCipherAlgorithm)
            break;
    }

    if (index == RSNIEInfo->PairwiseCipherCount)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (no matching pairwise cipher)\n"));
        return FALSE;
    }

    //
    // Check AP's AKM suite list. At least one must match with our auth algorithm
    //
    for (index = 0; index < RSNIEInfo->AKMSuiteCount; index++)
    {
        authAlgo = Dot11GetAKMSuiteFromRSNIEInfo(RSNIEInfo, (USHORT)index);
        if (authAlgo == pStation->Config.AuthAlgorithm)
            break;
    }

    if (index == RSNIEInfo->AKMSuiteCount)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (no matching AKM suite)\n"));
        return FALSE;
    }

    return TRUE;
}

VOID 
StaIndicateDot11Status(
    _In_ PMP_EXTSTA_PORT        pStation,
    _In_  NDIS_STATUS     StatusCode,
    _In_opt_  PVOID           RequestID,
    _In_  PVOID           pStatusBuffer,
    _In_  ULONG           StatusBufferSize
    )
{
    NDIS_STATUS_INDICATION  statusIndication;
    
    NdisZeroMemory(&statusIndication, sizeof(NDIS_STATUS_INDICATION));
    
    //
    // Fill in object header
    //
    statusIndication.Header.Type = NDIS_OBJECT_TYPE_STATUS_INDICATION;
    statusIndication.Header.Revision = NDIS_STATUS_INDICATION_REVISION_1;
    statusIndication.Header.Size = sizeof(NDIS_STATUS_INDICATION);

    // Fill in the port number
    statusIndication.PortNumber = (STA_GET_MP_PORT(pStation))->PortNumber;
    //
    // Fill in the rest of the field
    // 
    statusIndication.StatusCode = StatusCode;
    statusIndication.SourceHandle = STA_GET_MP_PORT(pStation)->MiniportAdapterHandle;
    statusIndication.DestinationHandle = NULL;
    statusIndication.RequestId = RequestID;
    
    statusIndication.StatusBuffer = pStatusBuffer;
    statusIndication.StatusBufferSize = StatusBufferSize;

    MpTrace(COMP_EVENTS, DBG_SERIOUS, ("Port(%d): Indicating NDIS_STATUS_INDICATION 0x%08x\n", statusIndication.PortNumber, StatusCode));

    //
    // Indicate the status to NDIS
    //
    NdisMIndicateStatusEx(
        STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
        &statusIndication
        );
}

VOID
Sta11IndicateStatus(
    _In_  PMP_PORT                Port,
    _In_  NDIS_STATUS             StatusCode,
    _In_  PVOID                   StatusBuffer,
    _In_  ULONG                   StatusBufferSize
    )
{
    // Send the indication up
    StaIndicateDot11Status(MP_GET_STA_PORT(Port), 
        StatusCode, 
        NULL, 
        StatusBuffer, 
        StatusBufferSize
        );
}

VOID
Sta11Notify(
    _In_  PMP_PORT        Port,
    PVOID               Notif
)
{
    PMP_EXTSTA_PORT         staPort = MP_GET_STA_PORT(Port);
    PNOTIFICATION_DATA_HEADER   notifyHeader = (PNOTIFICATION_DATA_HEADER)Notif;

    switch (notifyHeader->Type)
    {
        case NotificationOpRateChange:
        {
            POP_RATE_CHANGE_NOTIFICATION rateNotif = (POP_RATE_CHANGE_NOTIFICATION)Notif;
            
            // When there is a rate change notification, we set ourselves
            // up for doing a link quality indication on the next received packet
            staPort->ConnectContext.UpdateLinkQuality = TRUE;

            // If we are at the lower rate, lets try to roam
            if (rateNotif->LowestRate)
                staPort->ConnectContext.RoamForSendFailures = TRUE;
            break;
        }
        default:
            break;
    }
}

