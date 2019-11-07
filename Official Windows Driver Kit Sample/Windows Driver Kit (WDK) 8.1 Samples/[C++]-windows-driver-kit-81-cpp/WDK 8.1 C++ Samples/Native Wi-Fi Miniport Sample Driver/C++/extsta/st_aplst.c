/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    St_aplst.c

Abstract:
    STA layer BSS list functionality
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created

Notes:

--*/
#include "precomp.h"
#include "st_aplst.h"
#include "st_adhoc.h"

#if DOT11_TRACE_ENABLED
#include "St_aplst.tmh"
#endif

VOID 
StaReceiveBeacon(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_  PMP_RX_MPDU                    pFragment,
    _In_  ULONG                           TotalLength
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pPacketBuffer;
    PDOT11_BEACON_FRAME pDot11BeaconFrame;
    ULONG               uOffsetOfInfoElemBlob =
                            FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements) + sizeof(DOT11_MGMT_HEADER);
    ULONG               uInfoElemBlobSize = 0;

    pPacketBuffer = MP_RX_MPDU_DATA(pFragment);
    do
    {
        // 
        // Drop if its doesnt contain atleast the
        // fixed size portion (DOT11_BEACON_FRAME)
        //
        if (uOffsetOfInfoElemBlob > TotalLength)
        {
            break;
        }

        pDot11BeaconFrame = (PDOT11_BEACON_FRAME)(pPacketBuffer + sizeof(DOT11_MGMT_HEADER));
        
        // Validate information elements blob
        ndisStatus = Dot11GetInfoBlobSize(
            pPacketBuffer,
            TotalLength,
            uOffsetOfInfoElemBlob,
            &uInfoElemBlobSize
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        if (pDot11BeaconFrame->Capability.IBSS) {
            ndisStatus = StaSaveAdHocStaInfo(
                pStation,
                pFragment,
                pDot11BeaconFrame,
                uInfoElemBlobSize
                );
        }
            
        ndisStatus = StaProcessBeaconForConfigInfo(
            pStation,
            pFragment,
            (PUCHAR)&pDot11BeaconFrame->InfoElements,
            TotalLength
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
    } while (FALSE);
}

BOOLEAN
StaHasAPConfigurationChanged(
    _In_  PMP_EXTSTA_PORT                        pStation
    )
{
    if (pStation->ConnectContext.ActiveAP->Channel != pStation->ConnectContext.AssociationChannel)
    {
        //
        // Channel has changed. Lets do a fresh association
        //
        MpTrace(COMP_ASSOC, DBG_LOUD, ("AP channel changed from %d to %d\n", 
            pStation->ConnectContext.AssociationChannel,
            pStation->ConnectContext.ActiveAP->Channel));

        return TRUE;
    }

    //
    // Currently we dont check any other parameters.
    // We can check the SSID. If the AP was stopped/restarted
    // the SSID may have changed and we may still see the beacon
    // but the AP has lost all our state. It should send us a 
    // disassociate, but it may not
    //
    
    return FALSE;
}

VOID
StaCheckForProtectionInERP(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_reads_bytes_(infoBlobLength)  PUCHAR          pInfoBlob,
    _In_  ULONG           infoBlobLength
    )
{
    NDIS_STATUS     status = NDIS_STATUS_SUCCESS;
    UCHAR           erpIELength = 0;
    PUCHAR          erpIEBuf = NULL;

    status = Dot11GetInfoEle(
                pInfoBlob,
                infoBlobLength,
                DOT11_INFO_ELEMENT_ID_ERP,
                &erpIELength,
                &erpIEBuf
                );

    if (status != NDIS_STATUS_SUCCESS || erpIELength != 1)    // ERP IE length has to be 1
    {
        VNic11SetCTSToSelfOption(STA_GET_VNIC(pStation), FALSE);
    }
    else
    {
        if (((DOT11_ERP_IE*)erpIEBuf)->UseProtection == 1)
        {
            VNic11SetCTSToSelfOption(STA_GET_VNIC(pStation), TRUE);
        }
        else
        {
            VNic11SetCTSToSelfOption(STA_GET_VNIC(pStation), FALSE);
        }
    }
}

NDIS_STATUS 
StaProcessBeaconForConfigInfo(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_  PMP_RX_MPDU                    pFragment,
    _In_reads_bytes_(TotalLength)  PUCHAR                          pInfoBlob,
    _In_  ULONG                           TotalLength
    )
{
    NDIS_STATUS         status = NDIS_STATUS_SUCCESS;
    BOOLEAN             beaconIsFromAP = FALSE;

    do
    {
        if (pStation->Config.CheckForProtectionInERP == FALSE &&
            pStation->ConnectContext.UpdateLinkQuality == FALSE)
        {
            break;
        }
            
        NdisDprAcquireSpinLock(&(pStation->ConnectContext.Lock));
        if (pStation->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED &&
            NdisEqualMemory(pStation->ConnectContext.ActiveAP->Dot11BSSID,
                            ((DOT11_MGMT_HEADER*)MP_RX_MPDU_DATA(pFragment))->SA,
                            sizeof(DOT11_MAC_ADDRESS)))
        {
            beaconIsFromAP = TRUE;
        }
        NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));

        if (beaconIsFromAP == FALSE)
        {
            break;
        }

        // look for UseProtection bit in beacon's ERP IE
        if (pStation->Config.CheckForProtectionInERP == TRUE)
        {
            StaCheckForProtectionInERP(pStation, pInfoBlob, TotalLength);
            pStation->Config.CheckForProtectionInERP = FALSE;
        }

        // indicate link quality indication to the os
        if (pStation->ConnectContext.UpdateLinkQuality == TRUE &&
            pStation->Config.BSSType == dot11_BSS_type_infrastructure)
        {
            UCHAR                           buffer[sizeof(DOT11_LINK_QUALITY_PARAMETERS) + sizeof(DOT11_LINK_QUALITY_ENTRY)];
            ULONG                           bufferLength = sizeof(buffer);
            DOT11_LINK_QUALITY_PARAMETERS*  pLinkQualityParams = (DOT11_LINK_QUALITY_PARAMETERS*)&buffer[0];
            DOT11_LINK_QUALITY_ENTRY*       pEntry = (DOT11_LINK_QUALITY_ENTRY*)&buffer[sizeof(DOT11_LINK_QUALITY_PARAMETERS)];

            // reset the variable
            pStation->ConnectContext.UpdateLinkQuality = FALSE;

            // initialize indication buffer
            NdisZeroMemory(&buffer[0], bufferLength);

            MP_ASSIGN_NDIS_OBJECT_HEADER(pLinkQualityParams->Header, 
                                         NDIS_OBJECT_TYPE_DEFAULT,
                                         DOT11_LINK_QUALITY_PARAMETERS_REVISION_1,
                                         sizeof(DOT11_LINK_QUALITY_PARAMETERS));

            pLinkQualityParams->uLinkQualityListSize = 1;
            pLinkQualityParams->uLinkQualityListOffset = sizeof(DOT11_LINK_QUALITY_PARAMETERS);

            // previous NdisZeroMemory already set pEntry->PeerMacAddr to all 0x00, which
            // means the link quality is for current network
            pEntry->ucLinkQuality = pFragment->Msdu->LinkQuality;

            StaIndicateDot11Status(pStation, 
                                   NDIS_STATUS_DOT11_LINK_QUALITY,
                                   NULL,
                                   &buffer[0],
                                   bufferLength);
        }
    } while (FALSE);
    
    return status;
}



BOOLEAN
StaMatchAPSSID(
    _In_  PMP_BSS_ENTRY                  pAPEntry,
    _In_  PSTA_CURRENT_CONFIG             pConfig
    )
{
    DOT11_SSID      APSSID, *pMatchSSID;
    NDIS_STATUS     ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN         bIsAcceptable = TRUE;  // Default accept
    ULONG           i;
    ULONG           uOffsetOfInfoElemBlob = FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);

    do
    {
        // Zero length DesiredSSID is the wildcard SSID. That would match any AP and
        // we dont need to compare                      
        if (pConfig->SSID.uSSIDLength != 0)
        {
            // Get the SSID from the Beacon IE's
            if (pAPEntry->BeaconFrameSize > uOffsetOfInfoElemBlob)
            {
                ndisStatus = Dot11CopySSIDFromMemoryBlob(
                    pAPEntry->pDot11BeaconFrame + uOffsetOfInfoElemBlob,
                    pAPEntry->BeaconFrameSize - uOffsetOfInfoElemBlob,
                    &APSSID
                    );

                if (ndisStatus != NDIS_STATUS_SUCCESS)
                {
                    // No SSID IE in the AP entry. We assume this is a bad AP and
                    // reject it
                    MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (No SSID IE)\n"));
                    bIsAcceptable = FALSE;
                    break;
                }        

                pMatchSSID = &APSSID;
            }
            else
            {
                pMatchSSID = NULL;
            }
            
            if ((pMatchSSID == NULL) ||
                Dot11IsHiddenSSID(pMatchSSID->ucSSID, pMatchSSID->uSSIDLength))
            {
                if (pAPEntry->ProbeFrameSize > uOffsetOfInfoElemBlob)
                {
                    // Hidden SSID. See if we have an ssid from a probe response
                    ndisStatus = Dot11CopySSIDFromMemoryBlob(
                        pAPEntry->pDot11ProbeFrame + uOffsetOfInfoElemBlob,
                        pAPEntry->ProbeFrameSize - uOffsetOfInfoElemBlob,
                        &APSSID
                        );
                    
                    if (ndisStatus != NDIS_STATUS_SUCCESS)
                    {
                        // No SSID IE in the AP entry. We assume this is a bad AP and
                        // reject it
                        MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (No SSID IE)\n"));
                        bIsAcceptable = FALSE;
                        break;
                    }        

                    // Use this for matching
                    pMatchSSID = &APSSID;
                    MpTrace(COMP_ASSOC, DBG_LOUD, (" - Using probe response SSID\n"));
                }
            }
            
            // Check that SSID matches (Note: case sensitive comparison)
            if ((pMatchSSID == NULL) ||
                (pMatchSSID->uSSIDLength != pConfig->SSID.uSSIDLength) ||
                (NdisEqualMemory(pMatchSSID->ucSSID, pConfig->SSID.ucSSID, pMatchSSID->uSSIDLength) != TRUE))
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (mismatched SSID)\n"));
                bIsAcceptable = FALSE;
                break;
            }
        }
        
        // Check the excluded MAC address list
        if (pConfig->IgnoreAllMACAddresses)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (Ignore all MAC addresses)\n"));
            bIsAcceptable = FALSE;
            break;
        }
        else
        {
            bIsAcceptable = TRUE;        

            // Walk through the excluded MAC address list
            for (i = 0; i < pConfig->ExcludedMACAddressCount; i++)
            {
                if (MP_COMPARE_MAC_ADDRESS(pAPEntry->MacAddress,
                        pConfig->ExcludedMACAddressList[i]) == TRUE)
                {
                    bIsAcceptable = FALSE;
                    break;
                }
            }

            if (bIsAcceptable == FALSE)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (Excluded MAC addresses)\n"));
                break;
            }
        }

        // Check the desired BSSID list
        if (pConfig->AcceptAnyBSSID == FALSE)
        {
            bIsAcceptable = FALSE;        

            // Walk through the desired BSSID list
            for (i = 0; i < pConfig->DesiredBSSIDCount; i++)
            {
                if (MP_COMPARE_MAC_ADDRESS(pAPEntry->Dot11BSSID,
                        pConfig->DesiredBSSIDList[i]) == TRUE)
                {
                    // This is an acceptable BSSID
                    bIsAcceptable = TRUE;
                    break;
                }
            }

            if (bIsAcceptable == FALSE)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (Not in desired BSSID list)\n"));
                break;
            }
        }
        
        // The SSID matches
        bIsAcceptable = TRUE;
        break;
        
    }while (FALSE);

    return bIsAcceptable;
}

BOOLEAN
StaMatchPhyId(
    _In_  PMP_EXTSTA_PORT    pStation,
    _In_  ULONG       PhyId
    )
{
    ULONG                       index;

    ASSERT(PhyId != DOT11_PHY_ID_ANY);

    if (PhyId == DOT11_PHY_ID_ANY || PhyId == STA_INVALID_PHY_ID)
    {
        return FALSE;
    }

    if (pStation->Config.DesiredPhyList[0] == DOT11_PHY_ID_ANY)
        return TRUE;

    //
    // Check if the PHY ID is in our desired PHY ID list
    //
    for (index = 0; index < pStation->Config.DesiredPhyCount; index++)
    {
        if (PhyId == pStation->Config.DesiredPhyList[index])
            return TRUE;
    }

    MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (Not in desired PHY list)\n"));
    return FALSE;
}

BOOLEAN
StaMatchAPDataRates(
    _In_  PMP_BSS_ENTRY                  pAPEntry,
    _In_  PMP_EXTSTA_PORT                        pStation
    )
{
    NDIS_STATUS     ndisStatus = NDIS_STATUS_SUCCESS;
    DOT11_RATE_SET  rateSet = {0};
    BOOLEAN         bIsAcceptable = TRUE;  // Default accept
    
    do
    {
        ndisStatus = Dot11GetRateSetFromInfoEle(
                pAPEntry->pDot11InfoElemBlob,
                pAPEntry->InfoElemBlobSize,
                TRUE, 
                &rateSet
                );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (No Supported Rates IE)\n"));
            bIsAcceptable = FALSE;
            break;
        }

        //
        // Check that all the basic data rates required by the 
        // access point are supported by us
        // 
        ndisStatus = VNic11ValidateOperationalRates(
            STA_GET_VNIC(pStation),
            pAPEntry->PhyId,
            rateSet.ucRateSet,
            rateSet.uRateSetLength
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (Mismatched data rate)\n"));
            bIsAcceptable = FALSE;
            break;
        }
            
        bIsAcceptable = TRUE;
    }while (FALSE);

    return bIsAcceptable;
}

BOOLEAN
StaMatchAPSecurity(
    _In_  PMP_BSS_ENTRY                  pAPEntry,
    _In_  PMP_EXTSTA_PORT                        pStation
    )
{
    BOOLEAN                 bIsAcceptable = FALSE;
    UCHAR                   SecIELength = 0;
    PUCHAR                  SecIEData = NULL;
    RSN_IE_INFO             RSNIEInfo;
    NDIS_STATUS             ndisStatus;

    __try
    {
        //
        // Privacy bit
        //
        if (pStation->Config.UnicastCipherAlgorithm == DOT11_CIPHER_ALGO_NONE)
        {
            if (pAPEntry->Dot11Capability.Privacy)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (Privacy bit set)\n"));
                __leave;
            }
        }
        else if (pStation->Config.UnicastCipherAlgorithm == DOT11_CIPHER_ALGO_WEP)
        {
            //
            // If ExcludeUnencrypted is false, we associate with an AP even if 
            // it is not beaconing privacy bit
            //
            if (pStation->Config.ExcludeUnencrypted == TRUE)
            {
                if (!pAPEntry->Dot11Capability.Privacy)
                {
                    MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (Privacy bit clear)\n"));
                    __leave;
                }
            }
        }
        else
        {
            if (!pAPEntry->Dot11Capability.Privacy)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (Privacy bit clear)\n"));
                __leave;
            }
        }

        //
        // Check RSNA (WPA2) or WPA
        //
        if (pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA ||
            pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK)
        {
            ndisStatus = Dot11GetInfoEle(pAPEntry->pDot11InfoElemBlob,
                                         pAPEntry->InfoElemBlobSize,
                                         DOT11_INFO_ELEMENT_ID_RSN,
                                         &SecIELength,
                                         (PVOID)&SecIEData);

            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (AP not RSNA enabled)\n"));
                __leave;
            }

            ndisStatus = Dot11ParseRNSIE(SecIEData, RSNA_OUI_PREFIX, SecIELength, &RSNIEInfo);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (AP contains invalid RSN IE)\n"));
                __leave;
            }
        }
        else if (pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_WPA ||
                 pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_WPA_PSK)
        {
            ndisStatus = Dot11GetWPAIE(pAPEntry->pDot11InfoElemBlob,
                                       pAPEntry->InfoElemBlobSize,
                                       &SecIELength,
                                       (PVOID)&SecIEData);

            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (AP not WPA enabled)\n"));
                __leave;
            }

            ndisStatus = Dot11ParseRNSIE(SecIEData, WPA_OUI_PREFIX, SecIELength, &RSNIEInfo);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (AP contains invalid WPA IE)\n"));
                __leave;
            }
        }
        else
        {
            bIsAcceptable = TRUE;
            __leave;
        }

        //
        // Check if the AP is running RNSA/WPA with AKM and ciphers that meet our requirement. 
        //
        bIsAcceptable = StaMatchRSNInfo(pStation, &RSNIEInfo);
    }
    __finally
    {
    }

    return bIsAcceptable;
}

//
// AP entry must be locked against change
//
BOOLEAN
StaMatchAPEntry(
    _In_  PMP_BSS_ENTRY                  pAPEntry,
    _In_  PMP_EXTSTA_PORT                        pStation
    )
{
    PSTA_CURRENT_CONFIG  pConfig = &(pStation->Config);

    MpTrace(COMP_ASSOC, DBG_LOUD, ("Matching AP: %02X-%02X-%02X-%02X-%02X-%02X", 
        pAPEntry->Dot11BSSID[0], pAPEntry->Dot11BSSID[1], pAPEntry->Dot11BSSID[2], 
        pAPEntry->Dot11BSSID[3], pAPEntry->Dot11BSSID[4], pAPEntry->Dot11BSSID[5]));
    
    // Ignore entries ready for deletion
    if (pAPEntry->RefCount < 1)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (Deleting)\n"));
        return FALSE;
    }

    // BSS Type
    if (pConfig->BSSType != pAPEntry->Dot11BSSType)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, (" - Reject (mismatched BSS)\n"));
        return FALSE;
    }

    // Check AP
    if (StaMatchAPSSID(pAPEntry, pConfig) == FALSE)
    {
        return FALSE;
    }

    // Check PHY type
    if (StaMatchPhyId(pStation, pAPEntry->PhyId) == FALSE)
    {
        return FALSE;
    }

    // Match data rates
    if (StaMatchAPDataRates(pAPEntry, pStation) == FALSE)
    {
        return FALSE;
    }

    // Match WEP/WPA/WPA2 capabilities
    if (StaMatchAPSecurity(pAPEntry, pStation) == FALSE)
    {
        return FALSE;
    }

    MpTrace(COMP_ASSOC, DBG_LOUD, (" - Accepted, RSSI %d\n", pAPEntry->RSSI));
    
    // We can use this AP
    return TRUE;
}

NDIS_STATUS
StaRankCandidateAPList(
    _In_  PMP_EXTSTA_PORT                        pStation
    )
{
    PMP_BSS_ENTRY *APList;
    PMP_BSS_ENTRY  tmpAP;
    ULONG           APCount;
    ULONG           i, j;

    APList = pStation->ConnectContext.CandidateAPList;
    APCount = pStation->ConnectContext.CandidateAPCount;
    if (APCount < 2)
        return NDIS_STATUS_SUCCESS;

    //
    // Order all the candidate APs according to RSSI. 
    //

    for (i = 0; i < APCount - 1; i++) 
    {
        for (j = i + 1; j < APCount; j++)
        {
            if (APList[i]->LinkQuality < APList[j]->LinkQuality)
            {
                tmpAP = APList[i];
                APList[i] = APList[j];
                APList[j] = tmpAP;
            }
        }
    }

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
StaGetCandidateAPList(
    _In_  PMP_EXTSTA_PORT         pStation,
    _In_  BOOLEAN                 bStrictFiltering
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    MP_RW_LOCK_STATE            LockState;
    PMP_BSS_ENTRY               pAPEntry = NULL;
    PLIST_ENTRY                 pHead = NULL, pEntry = NULL;
    PMP_BSS_LIST                pDiscoveredAPList = NULL;
    ULONGLONG                   ullOldestAllowedEntry;

    //
    // Determine the expiry time we will use for determining if we
    // we will be picking an access point or not    
    //
    NdisGetCurrentSystemTime((PLARGE_INTEGER)&ullOldestAllowedEntry);
    if (bStrictFiltering)
    {
        //
        // We get stricter when determining which APs have expired, only accepting
        // APs we have seen in/after the last scan
        //
        ullOldestAllowedEntry = pStation->ScanContext.LastScanTime;
    }
    else
    {
        //
        // We will be more relaxed in picking stale APs
        //
        if (pStation->RegInfo->BSSEntryExpireTime <= ullOldestAllowedEntry)
            ullOldestAllowedEntry -= pStation->RegInfo->BSSEntryExpireTime;
    }

    pStation->ConnectContext.CandidateAPCount = 0;

    pDiscoveredAPList = Mp11QueryAndRefBSSList(
                            STA_GET_MP_PORT(pStation)->Adapter, 
                            STA_GET_MP_PORT(pStation),
                            &LockState
                            );
    if (pDiscoveredAPList == NULL)
    {
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to obtain BSS list\n"));
        return NDIS_STATUS_FAILURE;
    }

    pHead = &(pDiscoveredAPList->List);
    pEntry = pHead->Flink;
    //
    // Try to find as many access points as we can
    //
    while((pEntry != pHead) && 
          (pStation->ConnectContext.CandidateAPCount < STA_CANDIDATE_AP_MAX_COUNT)
          )
    {
        pAPEntry = CONTAINING_RECORD(pEntry, MP_BSS_ENTRY, Link);
        pEntry = pEntry->Flink;

        NdisDprAcquireSpinLock(&(pAPEntry->Lock)); // Lock AP entry
        
        //
        // Ignore stale entries. We do periodic scanning, so it
        // an AP is not reasonably fresh, we dont accept it
        //
        if (pAPEntry->HostTimestamp < ullOldestAllowedEntry)
        {
            NdisDprReleaseSpinLock(&(pAPEntry->Lock));
            continue;
        }

        //
        // If we have failed/dropped association with this AP too many
        // times, we wont roam to it. This mainly helps us ensure
        // that we dont keep jumping between APs
        //
        if ((bStrictFiltering) && (pAPEntry->AssocCost > STA_ASSOC_COST_MAX_DONT_CONNECT))
        {
            // If the AP we are connected to goes away and all others 
            // are costly, we could lose connectivity. The roaming caller
            // needs to ensure that in such case, StrictFiltering is off
            NdisDprReleaseSpinLock(&(pAPEntry->Lock));
            continue;
        }

        //
        // Match the AP information with our AP/roaming filter
        //
        if (StaMatchAPEntry(
            pAPEntry,
            pStation
            ) == TRUE)
        {
            // Add a refcount so that we can ensure that the access point
            // does not go away while we rank/use it. Note we still have the
            // list lock, so the entry hasnt yet been deleted or modified
            if (NdisInterlockedIncrement(&(pAPEntry->RefCount)) > 1)
            {
                pStation->ConnectContext.CandidateAPList[pStation->ConnectContext.CandidateAPCount] = pAPEntry;
                pStation->ConnectContext.CandidateAPCount++;
            }
            else
            {
                // This entry maybe going away, dont use it
                NdisInterlockedDecrement(&(pAPEntry->RefCount));
            }
        }
        
        NdisDprReleaseSpinLock(&(pAPEntry->Lock));
    }

    //
    // Now reorder APs in our candidate list to have the most 
    // preferred AP first
    //
    ndisStatus = StaRankCandidateAPList(pStation);

    Mp11ReleaseBSSListRef(STA_GET_MP_PORT(pStation)->Adapter, 
        pDiscoveredAPList, 
        &LockState
        );

    return ndisStatus;
}
