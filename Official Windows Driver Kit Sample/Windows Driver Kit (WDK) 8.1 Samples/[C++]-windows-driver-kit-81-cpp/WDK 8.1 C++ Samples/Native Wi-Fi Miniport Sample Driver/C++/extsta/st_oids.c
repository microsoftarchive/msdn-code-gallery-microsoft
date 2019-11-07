/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_oids.c

Abstract:
    Implements the OID handling for the Station layer layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "st_oids.h"
#include "st_adhoc.h"
#include "st_scan.h"
#include "st_conn.h"
#include "st_aplst.h"

#if DOT11_TRACE_ENABLED
#include "st_oids.tmh"
#endif


VOID
StaInitializeStationConfig(
    _In_  PMP_EXTSTA_PORT         Station
    )
{
    PSTA_CURRENT_CONFIG         config = &Station->Config;

    //
    // free PrivacyExemptionList
    //
    if (config->PrivacyExemptionList)
    {
        MP_FREE_MEMORY(Station->Config.PrivacyExemptionList);
    }

    //
    // free IBSS params
    //
    if (config->AdditionalIEData)
    {
        MP_FREE_MEMORY(config->AdditionalIEData);
    }

    //
    // Any dynamically allocated structure must be freed prior to this point.
    //
    NdisZeroMemory(config, sizeof(STA_CURRENT_CONFIG));

    config->BSSType = dot11_BSS_type_infrastructure;
    // Wildcard SSID
    config->SSID.uSSIDLength = 0;

    // Setup to accept an BSSID
    config->DesiredBSSIDList[0][0] = 0xFF;
    config->DesiredBSSIDList[0][1] = 0xFF;
    config->DesiredBSSIDList[0][2] = 0xFF;
    config->DesiredBSSIDList[0][3] = 0xFF;
    config->DesiredBSSIDList[0][4] = 0xFF;
    config->DesiredBSSIDList[0][5] = 0xFF;
    config->DesiredBSSIDCount = 1;
    config->AcceptAnyBSSID = TRUE;

    //
    // Reset other configuration parameters.
    //
    config->UnreachableDetectionThreshold = 2000;
    config->ExcludeUnencrypted = FALSE;

    config->AID = 0;
    config->ValidAID = FALSE;
    config->ListenInterval = STA_LISTEN_INTERVAL_DEFAULT;

    config->MediaStreamingEnabled = FALSE;
    config->UnicastUseGroupEnabled = TRUE;
    config->HiddenNetworkEnabled = FALSE;
    config->SafeModeEnabled = FALSE;

    //
    // Reset desired PHY ID list
    //
    config->DesiredPhyCount = 1;
    config->DesiredPhyList[0] = DOT11_PHY_ID_ANY;
    config->ActivePhyId = DOT11_PHY_ID_ANY;
    
    //
    // This is scan specific setting 
    //
    Station->ScanContext.SSIDInProbeRequest = TRUE;         // True until we go to low power state

    //
    // Clear all statistics
    //
    NdisZeroMemory(&Station->Statistics, sizeof(STA_STATS));
}

VOID
StaResetStationConfig(
    _In_  PMP_EXTSTA_PORT         Station
    )
{
    //
    // Set default auth and cipher algorithms
    //
    StaSetDefaultAuthAlgo(Station);

    //
    // Reset power management state on hardware
    //
    StaSetPowerSavingLevel(
        Station,
        DOT11_POWER_SAVING_NO_POWER_SAVING
        );
}

NDIS_STATUS
StaGetAlgorithmPair(
    _In_    PMP_PORT                Port,
    _In_    DOT11_BSS_TYPE          BssType,
    _In_    STA_QUERY_ALGO_PAIR_FUNC    QueryFunction,
    _Outptr_result_maybenull_
            PDOT11_AUTH_CIPHER_PAIR *AlgoPairs,
    _Out_   PULONG                  NumAlgoPairs
    )
{
    DOT11_AUTH_CIPHER_PAIR_LIST CipherPairList;
    PDOT11_AUTH_CIPHER_PAIR_LIST    FullPairList;
    NDIS_STATUS                 ndisStatus;
    ULONG                       size;

    *AlgoPairs = NULL;
    *NumAlgoPairs = 0;

    //
    // First get the total size of the algorithm pair list.
    //
    ndisStatus = (*QueryFunction)(Port, BssType, &CipherPairList, sizeof(CipherPairList));
    if (ndisStatus != NDIS_STATUS_SUCCESS && ndisStatus != NDIS_STATUS_BUFFER_OVERFLOW)
    {
        return ndisStatus;
    }

    // Integer overflow
    if (FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs) > 
            FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs) + 
            CipherPairList.uTotalNumOfEntries * sizeof(DOT11_AUTH_CIPHER_PAIR))
    {
        return NDIS_STATUS_FAILURE;
    }

    size = FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs) +
           CipherPairList.uTotalNumOfEntries * sizeof(DOT11_AUTH_CIPHER_PAIR);

    MP_ALLOCATE_MEMORY(Port->MiniportAdapterHandle, 
                       &FullPairList,
                       size,
                       EXTSTA_MEMORY_TAG
                       );
    if (FullPairList == NULL)
    {
        return NDIS_STATUS_RESOURCES;
    }

    //
    // Get the size of the list and copy the algorithm pair list data. Note that we over-allocated a little
    // bit for convenience.
    //

    ndisStatus = (*QueryFunction)(Port, BssType, FullPairList, size);
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS && FullPairList->uTotalNumOfEntries == FullPairList->uNumOfEntries);

    *AlgoPairs = (PDOT11_AUTH_CIPHER_PAIR) FullPairList;
    *NumAlgoPairs = FullPairList->uNumOfEntries;

    //
    // Copy the algorithm pair to the beginning of the allocated buffer. Note that we cannot
    // use NdisMoveMemory as the source and destination overlap.
    //
    RtlMoveMemory(FullPairList,
                  FullPairList->AuthCipherPairs,
                  FullPairList->uNumOfEntries * sizeof(DOT11_AUTH_CIPHER_PAIR));

    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
StaQuerySupportedUnicastAlgorithmPairCallback(
    _In_  PMP_PORT                Port,
    _In_  DOT11_BSS_TYPE          BssType,
    _Out_writes_bytes_(TotalLength)
          PDOT11_AUTH_CIPHER_PAIR_LIST    AuthCipherList,
    _In_ _In_range_(sizeof(DOT11_AUTH_CIPHER_PAIR_LIST) - sizeof(DOT11_AUTH_CIPHER_PAIR), ULONG_MAX)  
          ULONG                   TotalLength
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       bytesNeeded = 0;
    ULONG                       count;
    PVNIC                       vnic = Port->VNic;
    BOOLEAN                     WEP40Implemented = VNic11WEP40Implemented(vnic);
    BOOLEAN                     WEP104Implemented = VNic11WEP104Implemented(vnic);
    BOOLEAN                     TKIPImplemented = VNic11TKIPImplemented(vnic);
    BOOLEAN                     CCMPImplemented = VNic11CCMPImplemented(vnic, BssType);

    do
    {
        count = 1;
        if (WEP40Implemented) 
        {
            count++;
            if (BssType == dot11_BSS_type_infrastructure)
                count++;
        }

        if (WEP104Implemented)
        {
            count++;
            if (BssType == dot11_BSS_type_infrastructure)
                count++;
        }

        if (WEP40Implemented || WEP104Implemented)
        {
            count++;
            if (BssType == dot11_BSS_type_infrastructure)
                count++;
        }

        if (TKIPImplemented && (BssType == dot11_BSS_type_infrastructure))
        {
            count += 4;
        }

        if (CCMPImplemented)
        {
            count++;
            if (BssType == dot11_BSS_type_infrastructure)
                count += 3;
        }

        // Ensure enough space for one entry (though this would
        // get saved as part of the DOT11_AUTH_CIPHER_PAIR_LIST structure
        // itself)
        bytesNeeded = FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs) +
                      count * sizeof(DOT11_AUTH_CIPHER_PAIR);
        
        AuthCipherList->uNumOfEntries = 0;
        AuthCipherList->uTotalNumOfEntries = count;

        if (TotalLength < bytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;            
            break;
        }

        AuthCipherList->uNumOfEntries = count;
        
        count = 0;
        AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_OPEN;
        AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_NONE;

        if (WEP40Implemented)
        {
            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_OPEN;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP40;

            if (BssType == dot11_BSS_type_infrastructure)
            {
                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_SHARED_KEY;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP40;
            }
        }

        if (WEP104Implemented) 
        {
            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_OPEN;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP104;

            if (BssType == dot11_BSS_type_infrastructure)
            {
                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_SHARED_KEY;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP104;
            }
        }

        if (WEP40Implemented || WEP104Implemented)
        {
            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_OPEN;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP;

            if (BssType == dot11_BSS_type_infrastructure)
            {
                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_SHARED_KEY;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP;
            }
        }

        if (TKIPImplemented && BssType == dot11_BSS_type_infrastructure)
        {
            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_TKIP;

            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA_PSK;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_TKIP;

            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_TKIP;

            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA_PSK;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_TKIP;
        }

        if (CCMPImplemented)
        {
            if (BssType == dot11_BSS_type_infrastructure)
            {
                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_CCMP;

                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA_PSK;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_CCMP;

                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_CCMP;
            }

            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA_PSK;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_CCMP;
        }

    } while(FALSE);

    return ndisStatus;
}


NDIS_STATUS
StaQuerySupportedMulticastAlgorithmPairCallback(
    _In_  PMP_PORT                Port,
    _In_  DOT11_BSS_TYPE          BssType,
    _Out_writes_bytes_(TotalLength)
          PDOT11_AUTH_CIPHER_PAIR_LIST    AuthCipherList,
    _In_ _In_range_(sizeof(DOT11_AUTH_CIPHER_PAIR_LIST) - sizeof(DOT11_AUTH_CIPHER_PAIR), ULONG_MAX)
          ULONG                   TotalLength
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       bytesNeeded = 0;
    ULONG                       count;
    PVNIC                       vnic = Port->VNic;
    BOOLEAN                     WEP40Implemented = VNic11WEP40Implemented(vnic);
    BOOLEAN                     WEP104Implemented = VNic11WEP104Implemented(vnic);
    BOOLEAN                     TKIPImplemented = VNic11TKIPImplemented(vnic);
    BOOLEAN                     CCMPImplemented = VNic11CCMPImplemented(vnic, BssType);

    do
    {
        count = 1;

        if (WEP40Implemented) 
        {
            count++;
            if (BssType == dot11_BSS_type_infrastructure)
            {
                count++;
                if (TKIPImplemented || CCMPImplemented)
                    count += 4;
            }
        }

        if (WEP104Implemented) 
        {
            count++;
            if (BssType == dot11_BSS_type_infrastructure)
            {
                count++;
                if (TKIPImplemented || CCMPImplemented)
                    count += 4;
            }
        }

        if (WEP40Implemented || WEP104Implemented)
        {
            count++;
            if (BssType == dot11_BSS_type_infrastructure)
                count++;
        }

        if (TKIPImplemented && BssType == dot11_BSS_type_infrastructure)
        {
            count += 4;
        }

        if (CCMPImplemented)
        {
            count++;
            if (BssType == dot11_BSS_type_infrastructure)
                count += 3;
        }

        // Ensure enough space for one entry (though this would
        // get saved as part of the DOT11_AUTH_CIPHER_PAIR_LIST structure
        // itself)
        bytesNeeded = FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs) +
                      count * sizeof(DOT11_AUTH_CIPHER_PAIR);
        
        AuthCipherList->uNumOfEntries = 0;
        AuthCipherList->uTotalNumOfEntries = count;

        if (TotalLength < bytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;            
            break;
        }

        AuthCipherList->uNumOfEntries = count;

        count = 0;
        AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_OPEN;
        AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_NONE;

        if (WEP40Implemented)
        {
            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_OPEN;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP40;

            if (BssType == dot11_BSS_type_infrastructure)
            {
                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_SHARED_KEY;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP40;
 
                if (TKIPImplemented || CCMPImplemented) 
                {
                    count++;
                    AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA;
                    AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP40;

                    count++;
                    AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA_PSK;
                    AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP40;

                    count++;
                    AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA;
                    AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP40;

                    count++;
                    AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA_PSK;
                    AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP40;
                }
            }
        }

        if (WEP104Implemented) 
        {
            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_OPEN;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP104;

            if (BssType == dot11_BSS_type_infrastructure)
            {
                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_SHARED_KEY;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP104;

                if (TKIPImplemented || CCMPImplemented)
                {
                    count++;
                    AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA;
                    AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP104;

                    count++;
                    AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA_PSK;
                    AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP104;

                    count++;
                    AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA;
                    AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP104;

                    count++;
                    AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA_PSK;
                    AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP104;
                }
            }
        }

        if (WEP40Implemented || WEP104Implemented)
        {
            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_OPEN;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP;

            if (BssType == dot11_BSS_type_infrastructure)
            {
                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_80211_SHARED_KEY;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_WEP;
            }
        }

        if (TKIPImplemented && BssType == dot11_BSS_type_infrastructure)
        {
            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_TKIP;

            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA_PSK;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_TKIP;

            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_TKIP;

            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA_PSK;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_TKIP;
        }

        if (CCMPImplemented)
        {
            if (BssType == dot11_BSS_type_infrastructure)
            {
                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_CCMP;

                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_WPA_PSK;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_CCMP;

                count++;
                AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA;
                AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_CCMP;
            }

            count++;
            AuthCipherList->AuthCipherPairs[count].AuthAlgoId = DOT11_AUTH_ALGO_RSNA_PSK;
            AuthCipherList->AuthCipherPairs[count].CipherAlgoId = DOT11_CIPHER_ALGO_CCMP;
        }

    } while(FALSE);

    return ndisStatus;
}

    
NDIS_STATUS
StaQueryExtStaCapability(
    _In_  PMP_PORT                Port,
    _Out_ PDOT11_EXTSTA_CAPABILITY   Dot11ExtStaCap
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC                       vnic = Port->VNic;

    do
    {
        MP_ASSIGN_NDIS_OBJECT_HEADER(Dot11ExtStaCap->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_EXTSTA_CAPABILITY_REVISION_1,
            sizeof(DOT11_EXTSTA_CAPABILITY));

        Dot11ExtStaCap->uScanSSIDListSize = 4;     // minimum required.
        Dot11ExtStaCap->uDesiredBSSIDListSize = STA_DESIRED_BSSID_MAX_COUNT;
        Dot11ExtStaCap->uDesiredSSIDListSize = 1;
        Dot11ExtStaCap->uExcludedMacAddressListSize = STA_EXCLUDED_MAC_ADDRESS_MAX_COUNT;
        Dot11ExtStaCap->uPrivacyExemptionListSize = 32;
        Dot11ExtStaCap->uKeyMappingTableSize = VNic11KeyMappingKeyTableSize(vnic);
        Dot11ExtStaCap->uDefaultKeyTableSize = VNic11DefaultKeyTableSize(vnic);
        Dot11ExtStaCap->uWEPKeyValueMaxLength = VNic11WEP104Implemented(vnic) ? 
                                                 104 / 8 : (VNic11WEP40Implemented(vnic) ? 40 / 8 : 0);
        Dot11ExtStaCap->uPMKIDCacheSize = STA_PMKID_MAX_COUNT;
        Dot11ExtStaCap->uMaxNumPerSTADefaultKeyTables = VNic11PerStaKeyTableSize(vnic);

    } while(FALSE);

    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryActivePhyList(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_PHY_ID_LIST), ULONG_MAX) 
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_PHY_ID_LIST          phyIdList;
    
    do
    {
        *BytesWritten = 0;
        *BytesNeeded = 0;

        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        phyIdList = (PDOT11_PHY_ID_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(phyIdList->Header, 
                                     NDIS_OBJECT_TYPE_DEFAULT,
                                     DOT11_PHY_ID_LIST_REVISION_1,
                                     sizeof(DOT11_PHY_ID_LIST));
                                     
        //
        // Our NIC only supports one active PHY at a time.
        //
        phyIdList->uTotalNumOfEntries = 1;
        
        //
        // If the buffer is not big enough, simply return error.
        //
        if (InformationBufferLength < (FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId) 
                                        + phyIdList->uTotalNumOfEntries * sizeof(ULONG))
            )
        {
            phyIdList->uNumOfEntries = 0;
            return NDIS_STATUS_BUFFER_OVERFLOW;
        }

        //
        // Copy the desired PHY list.
        //
        phyIdList->uNumOfEntries = 1;
        phyIdList->dot11PhyId[0] = Station->Config.ActivePhyId;


    } while (FALSE);


    *BytesWritten = phyIdList->uNumOfEntries * sizeof(ULONG) + 
                    FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId);
        
    *BytesNeeded = phyIdList->uTotalNumOfEntries * sizeof(ULONG) +
                   FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId);

    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryDesiredBSSIDList(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_BSSID_LIST) - sizeof(DOT11_MAC_ADDRESS), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_BSSID_LIST           dot11BSSIDList;
    
    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        dot11BSSIDList = (PDOT11_BSSID_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(dot11BSSIDList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_BSSID_LIST_REVISION_1,
            sizeof(DOT11_BSSID_LIST)
            );

        dot11BSSIDList->uTotalNumOfEntries = Station->Config.DesiredBSSIDCount;
        
        // Integer overflow
        if (FIELD_OFFSET(DOT11_BSSID_LIST, BSSIDs) > 
                FIELD_OFFSET(DOT11_BSSID_LIST, BSSIDs) + 
                Station->Config.DesiredBSSIDCount * sizeof(DOT11_MAC_ADDRESS))
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            break;
        }
        
        *BytesNeeded = Station->Config.DesiredBSSIDCount * sizeof(DOT11_MAC_ADDRESS)
                        + FIELD_OFFSET(DOT11_BSSID_LIST, BSSIDs);

        if (InformationBufferLength < *BytesNeeded)
        {
            dot11BSSIDList->uNumOfEntries = 0;
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        NdisMoveMemory(
            dot11BSSIDList->BSSIDs, 
            &(Station->Config.DesiredBSSIDList),
            sizeof(DOT11_MAC_ADDRESS) * Station->Config.DesiredBSSIDCount
            );

        dot11BSSIDList->uNumOfEntries = Station->Config.DesiredBSSIDCount;
        
    } while(FALSE);
    
    *BytesWritten = dot11BSSIDList->uNumOfEntries * sizeof(DOT11_BSSID_LIST) + 
        FIELD_OFFSET(DOT11_BSSID_LIST, BSSIDs);
        
    *BytesNeeded = dot11BSSIDList->uTotalNumOfEntries * sizeof(DOT11_BSSID_LIST) +
        FIELD_OFFSET(DOT11_BSSID_LIST, BSSIDs);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryDesiredPhyList(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_PHY_ID_LIST) - sizeof(ULONG), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_PHY_ID_LIST          phyIdList;
    
    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        phyIdList = (PDOT11_PHY_ID_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(phyIdList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_PHY_ID_LIST_REVISION_1,
            sizeof(DOT11_PHY_ID_LIST)
            );

        phyIdList->uTotalNumOfEntries = Station->Config.DesiredPhyCount;

        // Integer overflow
        if (FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId) > 
                FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId) + 
                Station->Config.DesiredPhyCount * sizeof(ULONG))
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            break;
        }
        
        *BytesNeeded = Station->Config.DesiredPhyCount * sizeof(ULONG)
                        + FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId);

        if (InformationBufferLength < *BytesNeeded)
        {
            phyIdList->uNumOfEntries = 0;
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        NdisMoveMemory(
            phyIdList->dot11PhyId, 
            &(Station->Config.DesiredPhyList),
            sizeof(ULONG) * Station->Config.DesiredPhyCount
            );

        phyIdList->uNumOfEntries = Station->Config.DesiredPhyCount;
        
    } while(FALSE);
    
    *BytesWritten = phyIdList->uNumOfEntries * sizeof(ULONG) + 
        FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId);
        
    *BytesNeeded = phyIdList->uTotalNumOfEntries * sizeof(ULONG) +
        FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryDesiredSsidList(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten) 
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_SSID_LIST) - sizeof(DOT11_SSID), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_SSID_LIST            dot11SSIDList;
    
    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        dot11SSIDList = (PDOT11_SSID_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(dot11SSIDList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_SSID_LIST_REVISION_1,
            sizeof(DOT11_SSID_LIST)
            );
        dot11SSIDList->uTotalNumOfEntries = 1;

        // Ensure enough space for one entry (though this would
        // get saved as part of the DOT11_SSID_LIST structure
        // itself)
        *BytesNeeded = 1 * sizeof(DOT11_SSID)
                        + FIELD_OFFSET(DOT11_SSID_LIST, SSIDs);

        if (InformationBufferLength < *BytesNeeded)
        {
            dot11SSIDList->uNumOfEntries = 0;
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        NdisMoveMemory(
            dot11SSIDList->SSIDs, 
            &(Station->Config.SSID),
            sizeof(DOT11_SSID)
            );

        dot11SSIDList->uNumOfEntries = 1;
        
    } while(FALSE);
    
    *BytesWritten = dot11SSIDList->uNumOfEntries * sizeof(DOT11_SSID) + 
        FIELD_OFFSET(DOT11_SSID_LIST, SSIDs);
        
    *BytesNeeded = dot11SSIDList->uTotalNumOfEntries * sizeof(DOT11_SSID) +
        FIELD_OFFSET(DOT11_SSID_LIST, SSIDs);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryEnabledAuthenticationAlgorithm(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_AUTH_ALGORITHM_LIST) - sizeof(DOT11_AUTH_ALGORITHM), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_AUTH_ALGORITHM_LIST  authAlgoList = NULL;
    
    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        authAlgoList = (PDOT11_AUTH_ALGORITHM_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(authAlgoList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_AUTH_ALGORITHM_LIST_REVISION_1,
            sizeof(DOT11_AUTH_ALGORITHM_LIST));

        authAlgoList->uTotalNumOfEntries = 1;

        // Ensure enough space for one entry (though this would
        // get saved as part of the DOT11_AUTH_ALGORITHM_LIST structure
        // itself)
        *BytesNeeded =  FIELD_OFFSET(DOT11_AUTH_ALGORITHM_LIST, AlgorithmIds)
                        + 1 * sizeof(DOT11_AUTH_ALGORITHM);
        
        if (InformationBufferLength < *BytesNeeded)
        {
            authAlgoList->uNumOfEntries = 0;
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;            
            break;
        }

        authAlgoList->uNumOfEntries = 1;
        
        authAlgoList->AlgorithmIds[0] = Station->Config.AuthAlgorithm;

    } while(FALSE);
    
    *BytesWritten = authAlgoList->uNumOfEntries * sizeof(DOT11_AUTH_ALGORITHM) + 
        FIELD_OFFSET(DOT11_AUTH_ALGORITHM_LIST, AlgorithmIds);
        
    *BytesNeeded = authAlgoList->uTotalNumOfEntries * sizeof(DOT11_AUTH_ALGORITHM) +
        FIELD_OFFSET(DOT11_AUTH_ALGORITHM_LIST, AlgorithmIds);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryEnabledMulticastCipherAlgorithm(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_CIPHER_ALGORITHM_LIST) - sizeof(DOT11_CIPHER_ALGORITHM), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_CIPHER_ALGORITHM_LIST    authCipherList = NULL;
    
    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        authCipherList = (PDOT11_CIPHER_ALGORITHM_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(authCipherList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_CIPHER_ALGORITHM_LIST_REVISION_1,
            sizeof(DOT11_CIPHER_ALGORITHM_LIST));

        authCipherList->uTotalNumOfEntries = Station->Config.MulticastCipherAlgorithmCount;

        // Integer overflow
        if (FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds) > 
                FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds) + 
                Station->Config.MulticastCipherAlgorithmCount * sizeof(DOT11_CIPHER_ALGORITHM))
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            break;
        }
        
        *BytesNeeded = FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds) +
                        Station->Config.MulticastCipherAlgorithmCount * sizeof(DOT11_CIPHER_ALGORITHM);
        
        if (InformationBufferLength < *BytesNeeded)
        {
            authCipherList->uNumOfEntries = 0;
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;            
            break;
        }

        authCipherList->uNumOfEntries = Station->Config.MulticastCipherAlgorithmCount;

        NdisMoveMemory(authCipherList->AlgorithmIds,
            Station->Config.MulticastCipherAlgorithmList,
            Station->Config.MulticastCipherAlgorithmCount * sizeof(DOT11_CIPHER_ALGORITHM)
            );

    } while(FALSE);
    
    *BytesWritten = authCipherList->uNumOfEntries * sizeof(DOT11_CIPHER_ALGORITHM) + 
        FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds);
        
    *BytesNeeded = authCipherList->uTotalNumOfEntries * sizeof(DOT11_CIPHER_ALGORITHM) +
        FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryEnabledUnicastCipherAlgorithm(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_CIPHER_ALGORITHM_LIST) - sizeof(DOT11_CIPHER_ALGORITHM), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_CIPHER_ALGORITHM_LIST    authCipherList = NULL;
    
    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        authCipherList = (PDOT11_CIPHER_ALGORITHM_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(authCipherList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_CIPHER_ALGORITHM_LIST_REVISION_1,
            sizeof(DOT11_CIPHER_ALGORITHM_LIST));
            
        authCipherList->uTotalNumOfEntries = 1;

        // Ensure enough space for one entry (though this would
        // get saved as part of the DOT11_CIPHER_ALGORITHM_LIST structure
        // itself)
        *BytesNeeded = FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds) +
                      1 * sizeof(DOT11_CIPHER_ALGORITHM);
        
        if (InformationBufferLength < *BytesNeeded)
        {
            authCipherList->uNumOfEntries = 0;
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;            
            break;
        }

        authCipherList->uNumOfEntries = 1;
        authCipherList->AlgorithmIds[0] = Station->Config.UnicastCipherAlgorithm;

    } while(FALSE);
    
    *BytesWritten = authCipherList->uNumOfEntries * sizeof(DOT11_CIPHER_ALGORITHM) + 
        FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds);
        
    *BytesNeeded = authCipherList->uTotalNumOfEntries * sizeof(DOT11_CIPHER_ALGORITHM) +
        FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQuerySupportedMulticastAlgorithmPair(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_AUTH_CIPHER_PAIR_LIST) - sizeof(DOT11_AUTH_CIPHER_PAIR), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_AUTH_CIPHER_PAIR_LIST    authCipherList = NULL;
    
    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        authCipherList = (PDOT11_AUTH_CIPHER_PAIR_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(authCipherList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_AUTH_CIPHER_PAIR_LIST_REVISION_1,
            sizeof(DOT11_AUTH_CIPHER_PAIR_LIST));

        authCipherList->uNumOfEntries = 0;    
        authCipherList->uTotalNumOfEntries = 0;    

        ndisStatus = StaQuerySupportedMulticastAlgorithmPairCallback(
                        STA_GET_MP_PORT(Station), 
                        Station->Config.BSSType,
                        authCipherList, 
                        InformationBufferLength
                        );

    } while(FALSE);
    
    *BytesWritten = authCipherList->uNumOfEntries * sizeof(DOT11_AUTH_CIPHER_PAIR) + 
        FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs);
        
    *BytesNeeded = authCipherList->uTotalNumOfEntries * sizeof(DOT11_AUTH_CIPHER_PAIR) +
        FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQuerySupportedUnicastAlgorithmPair(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_AUTH_CIPHER_PAIR_LIST) - sizeof(DOT11_AUTH_CIPHER_PAIR), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_AUTH_CIPHER_PAIR_LIST    authCipherList = NULL;
    
    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        authCipherList = (PDOT11_AUTH_CIPHER_PAIR_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(authCipherList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_AUTH_CIPHER_PAIR_LIST_REVISION_1,
            sizeof(DOT11_AUTH_CIPHER_PAIR_LIST));

        authCipherList->uNumOfEntries = 0;    
        authCipherList->uTotalNumOfEntries = 0;    

        ndisStatus = StaQuerySupportedUnicastAlgorithmPairCallback(
                        STA_GET_MP_PORT(Station), 
                        Station->Config.BSSType,
                        authCipherList, 
                        InformationBufferLength
                        );

    } while(FALSE);
    
    *BytesWritten = authCipherList->uNumOfEntries * sizeof(DOT11_AUTH_CIPHER_PAIR) + 
        FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs);
        
    *BytesNeeded = authCipherList->uTotalNumOfEntries * sizeof(DOT11_AUTH_CIPHER_PAIR) +
        FIELD_OFFSET(DOT11_AUTH_CIPHER_PAIR_LIST, AuthCipherPairs);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaEnumerateAssociationInformation(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_ASSOCIATION_INFO_LIST), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_ASSOCIATION_INFO_LIST    assocInfoList;
    
    do
    {

        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        assocInfoList = (PDOT11_ASSOCIATION_INFO_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(assocInfoList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_ASSOCIATION_INFO_LIST_REVISION_1,
            sizeof(DOT11_ASSOCIATION_INFO_LIST));
            
        assocInfoList->uNumOfEntries = 0;        
        assocInfoList->uTotalNumOfEntries = 0;

        if (Station->Config.BSSType == dot11_BSS_type_infrastructure) 
        {
            ndisStatus = StaEnumerateAssociationInfoInfra(
                Station,
                assocInfoList,
                InformationBufferLength                
                );
        }
        else if (Station->Config.BSSType == dot11_BSS_type_independent) 
        {
            ndisStatus = StaEnumerateAssociationInfoAdHoc(
                Station,
                assocInfoList,
                InformationBufferLength
                );
        }
        else
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            break;
        }

    } while(FALSE);

    *BytesWritten = assocInfoList->uTotalNumOfEntries * sizeof(DOT11_ASSOCIATION_INFO_EX) + 
        FIELD_OFFSET(DOT11_ASSOCIATION_INFO_LIST, dot11AssocInfo);
        
    *BytesNeeded = assocInfoList->uTotalNumOfEntries * sizeof(DOT11_ASSOCIATION_INFO_EX) +
        FIELD_OFFSET(DOT11_ASSOCIATION_INFO_LIST, dot11AssocInfo);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryExcludedMACAddressList(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_MAC_ADDRESS_LIST) - sizeof(DOT11_MAC_ADDRESS), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_MAC_ADDRESS_LIST     dot11MacAddrList;
    
    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        dot11MacAddrList = (PDOT11_MAC_ADDRESS_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(dot11MacAddrList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_MAC_ADDRESS_LIST_REVISION_1,
            sizeof(DOT11_MAC_ADDRESS_LIST));

        dot11MacAddrList->uTotalNumOfEntries = Station->Config.ExcludedMACAddressCount;

        // Integer overflow
        if (FIELD_OFFSET(DOT11_MAC_ADDRESS_LIST, MacAddrs) > 
                FIELD_OFFSET(DOT11_MAC_ADDRESS_LIST, MacAddrs) + 
                Station->Config.ExcludedMACAddressCount * sizeof(DOT11_MAC_ADDRESS))
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            break;
        }

        //
        // Ensure enough space to return complete list
        //
        *BytesNeeded = Station->Config.ExcludedMACAddressCount 
                            * sizeof(DOT11_MAC_ADDRESS)
                        + FIELD_OFFSET(DOT11_MAC_ADDRESS_LIST, MacAddrs);
                        
        if (InformationBufferLength < *BytesNeeded)
        {
            dot11MacAddrList->uNumOfEntries = 0;
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        NdisMoveMemory(
            dot11MacAddrList->MacAddrs, 
            &(Station->Config.ExcludedMACAddressList),
            sizeof(DOT11_MAC_ADDRESS) * Station->Config.ExcludedMACAddressCount
            );

        dot11MacAddrList->uNumOfEntries = Station->Config.ExcludedMACAddressCount;
    } while(FALSE);
    
    *BytesWritten = dot11MacAddrList->uNumOfEntries * sizeof(DOT11_MAC_ADDRESS_LIST) + 
        FIELD_OFFSET(DOT11_MAC_ADDRESS_LIST, MacAddrs);
        
    *BytesNeeded = dot11MacAddrList->uTotalNumOfEntries * sizeof(DOT11_MAC_ADDRESS_LIST) +
        FIELD_OFFSET(DOT11_MAC_ADDRESS_LIST, MacAddrs);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryIBSSParameters(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_IBSS_PARAMS), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_IBSS_PARAMS          dot11IBSSParams;

    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        dot11IBSSParams = (PDOT11_IBSS_PARAMS)InformationBuffer;
        
        MP_ASSIGN_NDIS_OBJECT_HEADER(((PDOT11_IBSS_PARAMS)InformationBuffer)->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_IBSS_PARAMS_REVISION_1,
            sizeof(DOT11_IBSS_PARAMS));


        *BytesNeeded = sizeof(DOT11_IBSS_PARAMS) + Station->Config.AdditionalIESize;
        if (InformationBufferLength < *BytesNeeded)
        {
            *BytesWritten = 0;
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        dot11IBSSParams->bJoinOnly = Station->Config.IBSSJoinOnly;
        dot11IBSSParams->uIEsLength = Station->Config.AdditionalIESize;
        dot11IBSSParams->uIEsOffset = sizeof(DOT11_IBSS_PARAMS);

        if (Station->Config.AdditionalIESize > 0)
        {
            NdisMoveMemory(Add2Ptr(dot11IBSSParams, sizeof(DOT11_IBSS_PARAMS)),
                           Station->Config.AdditionalIEData,
                           Station->Config.AdditionalIESize);
        }                  

        *BytesWritten = *BytesNeeded;
    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryAssociationParameters(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_ASSOCIATION_PARAMS), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_ASSOCIATION_PARAMS   dot11AssocParams;

    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        dot11AssocParams = (PDOT11_ASSOCIATION_PARAMS)InformationBuffer;
        
        MP_ASSIGN_NDIS_OBJECT_HEADER(((PDOT11_ASSOCIATION_PARAMS)InformationBuffer)->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_ASSOCIATION_PARAMS_REVISION_1,
            sizeof(DOT11_ASSOCIATION_PARAMS));


        *BytesNeeded = sizeof(DOT11_ASSOCIATION_PARAMS) + Station->Config.AdditionalIESize;
        if (InformationBufferLength < *BytesNeeded)
        {
            *BytesWritten = 0;
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        NdisMoveMemory(dot11AssocParams->BSSID, Station->Config.AssocIEBSSID, DOT11_ADDRESS_SIZE);
        dot11AssocParams->uAssocRequestIEsLength = Station->Config.AdditionalIESize;
        dot11AssocParams->uAssocRequestIEsOffset = sizeof(DOT11_ASSOCIATION_PARAMS);

        if (Station->Config.AdditionalIESize > 0)
        {
            NdisMoveMemory(Add2Ptr(dot11AssocParams, sizeof(DOT11_ASSOCIATION_PARAMS)),
                           Station->Config.AdditionalIEData,
                           Station->Config.AdditionalIESize);
        }                  

        *BytesWritten = *BytesNeeded;
    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryPMKIDList(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_PMKID_LIST) - sizeof(DOT11_PMKID_ENTRY), ULONG_MAX)
          ULONG                 InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_PMKID_LIST           PMKIDList;
    
    do
    {
        *BytesWritten = 0;
        *BytesNeeded = 0;

        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        PMKIDList = (PDOT11_PMKID_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(PMKIDList->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_PMKID_LIST_REVISION_1,
            sizeof(DOT11_PMKID_LIST));
            
        PMKIDList->uTotalNumOfEntries = Station->Config.PMKIDCount;

        // Integer overflow
        if (FIELD_OFFSET(DOT11_PMKID_LIST, PMKIDs) > 
                FIELD_OFFSET(DOT11_PMKID_LIST, PMKIDs) + 
                Station->Config.PMKIDCount * sizeof(DOT11_PMKID_ENTRY))
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            PMKIDList->uNumOfEntries = 0;
            break;
        }

        //
        // If the buffer is not big enough, simply return error.
        //
        if (InformationBufferLength < (FIELD_OFFSET(DOT11_PMKID_LIST, PMKIDs) 
                + Station->Config.PMKIDCount * sizeof(DOT11_PMKID_ENTRY)))
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            PMKIDList->uNumOfEntries = 0;
            break;
        }

        //
        // Copy the PMKID list.
        //
        PMKIDList->uNumOfEntries = Station->Config.PMKIDCount;
        NdisMoveMemory(PMKIDList->PMKIDs,
                       Station->Config.PMKIDList,
                       Station->Config.PMKIDCount * sizeof(DOT11_PMKID_ENTRY));

    } while (FALSE);

    *BytesWritten = PMKIDList->uNumOfEntries * sizeof(DOT11_PMKID_ENTRY) + 
                    FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId);
        
    *BytesNeeded = PMKIDList->uTotalNumOfEntries * sizeof(DOT11_PMKID_ENTRY) +
                   FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryPrivacyExemptionList(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)\
          PVOID                   InformationBuffer,
    _In_ _In_range_((sizeof(DOT11_PRIVACY_EXEMPTION_LIST) - sizeof(DOT11_PRIVACY_EXEMPTION)), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                     ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_PRIVACY_EXEMPTION_LIST   list;
    PDOT11_PRIVACY_EXEMPTION_LIST   privacyExemptionList = Station->Config.PrivacyExemptionList;

    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        list = (PDOT11_PRIVACY_EXEMPTION_LIST)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(list->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_PRIVACY_EXEMPTION_LIST_REVISION_1,
            sizeof(DOT11_PRIVACY_EXEMPTION_LIST));
            
        //
        // If we don't have privacy exemption list, simply return success with number of entries set to 0.
        //
        list->uNumOfEntries = 0;
        if (!privacyExemptionList || !privacyExemptionList->uNumOfEntries) 
        {
            list->uTotalNumOfEntries = 0;
            ndisStatus = NDIS_STATUS_SUCCESS;
            break;
        }

        // Integer overflow
        if (FIELD_OFFSET(DOT11_PRIVACY_EXEMPTION_LIST, PrivacyExemptionEntries) > 
                FIELD_OFFSET(DOT11_PRIVACY_EXEMPTION_LIST, PrivacyExemptionEntries) + 
                privacyExemptionList->uNumOfEntries * sizeof(DOT11_PRIVACY_EXEMPTION))
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            break;
        }

        //
        // Check if we have enough space to copy all lists. If not, simply fail the request.
        // we don't copy partial list.
        //
        list->uTotalNumOfEntries = privacyExemptionList->uNumOfEntries;
        
        if (InformationBufferLength < FIELD_OFFSET(DOT11_PRIVACY_EXEMPTION_LIST, PrivacyExemptionEntries) +
                          privacyExemptionList->uNumOfEntries * sizeof(DOT11_PRIVACY_EXEMPTION))
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        //
        // We have enough space, copy all lists.
        //
        list->uNumOfEntries = privacyExemptionList->uNumOfEntries;
        NdisMoveMemory(list->PrivacyExemptionEntries,
                       privacyExemptionList->PrivacyExemptionEntries,
                       privacyExemptionList->uNumOfEntries * sizeof(DOT11_PRIVACY_EXEMPTION));

                
    } while(FALSE);

    *BytesWritten = FIELD_OFFSET(DOT11_PRIVACY_EXEMPTION_LIST, PrivacyExemptionEntries) +
                    list->uNumOfEntries * sizeof(DOT11_PRIVACY_EXEMPTION);
        
    *BytesNeeded = FIELD_OFFSET(DOT11_PRIVACY_EXEMPTION_LIST, PrivacyExemptionEntries) +
                   list->uTotalNumOfEntries * sizeof(DOT11_PRIVACY_EXEMPTION);
        
    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryDot11Statistics(
    _In_  PMP_EXTSTA_PORT         Station,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(sizeof(DOT11_STATISTICS), ULONG_MAX)
          ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_ PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_STATISTICS           dot11Stats;

    do
    {
        NdisZeroMemory(InformationBuffer, InformationBufferLength);

        dot11Stats = (PDOT11_STATISTICS)InformationBuffer;

        MP_ASSIGN_NDIS_OBJECT_HEADER(dot11Stats->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_STATISTICS_REVISION_1,
            sizeof(DOT11_STATISTICS));


        ndisStatus = VNic11QueryDot11Statistics(
                        STA_GET_VNIC(Station),
                        dot11Stats,
                        InformationBufferLength,
                        BytesWritten,
                        BytesNeeded
                        );
        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            // Excluded counts are kept by the station
            dot11Stats->MacUcastCounters.ullWEPExcludedCount = Station->Statistics.ullUcastWEPExcludedCount;
            dot11Stats->MacMcastCounters.ullWEPExcludedCount = Station->Statistics.ullMcastWEPExcludedCount;
        }

        MPASSERT(dot11Stats->Header.Revision == DOT11_STATISTICS_REVISION_1   &&
               dot11Stats->Header.Size == sizeof(DOT11_STATISTICS)     &&
               dot11Stats->Header.Type == NDIS_OBJECT_TYPE_DEFAULT);

    } while(FALSE);

    return ndisStatus;
}


VOID
StaSetDefaultAuthAlgo(
    _In_  PMP_EXTSTA_PORT         Station
    )
{
    PSTA_CURRENT_CONFIG         config = &Station->Config;
    PVNIC                       vnic = STA_GET_MP_PORT(Station)->VNic;

    //
    // Set the current authentication algorithm depending on bss type and hardware cipher implemented.
    //
    if (VNic11CCMPImplemented(vnic, config->BSSType) || VNic11TKIPImplemented(vnic))
    {
        if (config->BSSType == dot11_BSS_type_infrastructure)
        {
            config->AuthAlgorithm = DOT11_AUTH_ALGO_RSNA;
        }
        else if (VNic11CCMPImplemented(vnic, config->BSSType))
        {
            config->AuthAlgorithm = DOT11_AUTH_ALGO_RSNA_PSK;
        }
        else
        {
            config->AuthAlgorithm = DOT11_AUTH_ALGO_80211_OPEN;
        }
    }
    else
    {
        config->AuthAlgorithm = DOT11_AUTH_ALGO_80211_OPEN;
    }

    //
    // Set the default cipher depending on the new authentication algorithm selected.
    //
    StaSetDefaultCipher(Station);
}

VOID
StaSetDefaultCipher(
    _In_  PMP_EXTSTA_PORT         Station
    )
{
    PVNIC                       vnic = STA_GET_MP_PORT(Station)->VNic;
    PSTA_CURRENT_CONFIG         config = &Station->Config;
    BOOLEAN                     WEP40Implemented = VNic11WEP40Implemented(vnic);
    BOOLEAN                     WEP104Implemented = VNic11WEP104Implemented(vnic);
    BOOLEAN                     TKIPImplemented = VNic11TKIPImplemented(vnic);
    BOOLEAN                     CCMPImplemented = VNic11CCMPImplemented(vnic, Station->Config.BSSType);
    ULONG                       index = 0;

    switch (config->AuthAlgorithm)
    {
        case DOT11_AUTH_ALGO_80211_OPEN:
            if (WEP104Implemented || WEP40Implemented)
            {
                config->UnicastCipherAlgorithm = DOT11_CIPHER_ALGO_WEP;
                config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_WEP;
            }
            else 
            {
                config->UnicastCipherAlgorithm = DOT11_CIPHER_ALGO_NONE;
            }

            if (WEP104Implemented)
            {
                config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_WEP104;
            }

            if (WEP40Implemented)
            {
                config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_WEP40;
            }

            config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_NONE;

            break;

        case DOT11_AUTH_ALGO_80211_SHARED_KEY:
            MPASSERT(WEP104Implemented || WEP40Implemented);
            MPASSERT(Station->Config.BSSType == dot11_BSS_type_infrastructure);

            config->UnicastCipherAlgorithm = DOT11_CIPHER_ALGO_WEP;
            config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_WEP;

            if (WEP104Implemented)
            {
                config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_WEP104;
            }

            if (WEP40Implemented)
            {
                config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_WEP40;
            }

            break;

        case DOT11_AUTH_ALGO_WPA:
        case DOT11_AUTH_ALGO_WPA_PSK:
        case DOT11_AUTH_ALGO_RSNA:
            MPASSERT(Station->Config.BSSType == dot11_BSS_type_infrastructure);
            // fall through

        case DOT11_AUTH_ALGO_RSNA_PSK:
            MPASSERT(TKIPImplemented || CCMPImplemented);
            if (CCMPImplemented)
            {
                config->UnicastCipherAlgorithm = DOT11_CIPHER_ALGO_CCMP;
            }
            else 
            {
                MPASSERT(Station->Config.BSSType == dot11_BSS_type_infrastructure);
                config->UnicastCipherAlgorithm = DOT11_CIPHER_ALGO_TKIP;
            }

            if (CCMPImplemented)
            {
                config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_CCMP;
            }

            if (Station->Config.BSSType == dot11_BSS_type_infrastructure)
            {
                if (TKIPImplemented)
                {
                    config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_TKIP;
                }
                
                if (WEP104Implemented)
                {
                    config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_WEP104;
                }

                if (WEP40Implemented)
                {
                    config->MulticastCipherAlgorithmList[index++] = DOT11_CIPHER_ALGO_WEP40;
                }
            }

            break;

        default:
            MPASSERT(FALSE);
            return;
    }

    config->MulticastCipherAlgorithmCount = index;
    if (index > 1)
    {
        config->MulticastCipherAlgorithm = DOT11_CIPHER_ALGO_NONE;
    }
    else
    {
        config->MulticastCipherAlgorithm = config->MulticastCipherAlgorithmList[0];
    }
    
    VNic11SetCipher(vnic, TRUE, config->UnicastCipherAlgorithm);
    VNic11SetCipher(vnic, FALSE, config->MulticastCipherAlgorithm);
}




NDIS_STATUS
StaSetPacketFilter(
    _In_  PMP_EXTSTA_PORT         Station,
    _In_  ULONG                   PacketFilter
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       VNicPacketFilter;

    do
    {
        //
        // Any bits not supported?
        //
        if (PacketFilter & ~NDIS_PACKET_TYPE_ALL_802_11_FILTERS)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // The packet filter we set on the VNIC is a super set of the
        // packet filter set by the OS. The hardware filters out packets that
        // the miniport internally  & the OS are not interested in and then
        // we do software filtering of packets the OS is not interested in
        //
        VNicPacketFilter = PacketFilter | 
                            NDIS_PACKET_TYPE_802_11_DIRECTED_MGMT |
                            NDIS_PACKET_TYPE_802_11_BROADCAST_MGMT |
                            NDIS_PACKET_TYPE_802_11_MULTICAST_MGMT |
                            NDIS_PACKET_TYPE_802_11_ALL_MULTICAST_MGMT;
        
        ndisStatus = VNic11SetPacketFilter(
                        STA_GET_VNIC(Station),
                        VNicPacketFilter
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Save the new packet filter value
        //
        STA_GET_MP_PORT(Station)->PacketFilter = PacketFilter;

    } while(FALSE);

    return(ndisStatus);
}



_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaScanRequest(
    _In_  PMP_EXTSTA_PORT         Station,
    _In_reads_(InformationBufferLength)
          PVOID                     InformationBuffer,
    _In_  ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
          PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_SCAN_REQUEST_V2      dot11ScanRequest = InformationBuffer;

    UNREFERENCED_PARAMETER(BytesNeeded);
    
    do
    {
        *BytesRead = 0;
        
        if (STA_NIC_POWER_STATE_IS_OFF(Station))
        {
            MpTrace(COMP_SCAN, DBG_SERIOUS, ("Nic is currently turned off\n"));
            ndisStatus = NDIS_STATUS_DOT11_POWER_STATE_INVALID;
            break;
        }

        *BytesRead = sizeof(DOT11_SCAN_REQUEST_V2);
        
        //
        // Validate the input parameter
        //
        ndisStatus = BasePortValidateScanRequest(STA_GET_MP_PORT(Station), dot11ScanRequest);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        if (dot11ScanRequest->uNumOfdot11SSIDs == 0)
        {
            MpTrace(COMP_SCAN, DBG_SERIOUS, ("No SSID found in the scan data\n"));
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        //
        // Pass the scan request to the station
        //
        ndisStatus = StaStartScan(
                        Station,
                        STA_GET_MP_PORT(Station)->PendingOidRequest->RequestId,
                        dot11ScanRequest,
                        InformationBufferLength
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
    } while(FALSE);

    return ndisStatus;
}

NDIS_STATUS
StaChannelSwitchCompletionHandler(
    _In_ PMP_PORT     Port,
    _In_ PVOID        Data
    )
{
    NDIS_STATUS                 completeStatus = *((PNDIS_STATUS)Data);
    
    // The previous pending channel switch OID has finished. Complete
    // the OID to the OS
    Port11CompletePendingOidRequest(Port, completeStatus);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
StaSetCurrentChannel(
    _In_  PMP_EXTSTA_PORT         Station,
    _In_  ULONG                   Channel
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if(STA_NIC_POWER_STATE_IS_OFF(Station))
        {
            ndisStatus = NDIS_STATUS_DOT11_POWER_STATE_INVALID;
            break;
        }

        //
        // A scan operation is in progress. Setting the channel could interfere
        // with the scanning process. We will not honor this request.
        //
        if (STA_DOT11_SCAN_IN_PROGRESS(Station))
        {
            ndisStatus = NDIS_STATUS_DOT11_MEDIA_IN_USE;
            break;
        }

        // If autoconfig is enabled, we dont allow this to be set
        if (STA_GET_MP_PORT(Station)->AutoConfigEnabled & DOT11_PHY_AUTO_CONFIG_ENABLED_FLAG)
        {
            ndisStatus = NDIS_STATUS_DOT11_AUTO_CONFIG_ENABLED;
            break;
        }

        ndisStatus = VNic11SetChannel(STA_GET_VNIC(Station), 
                        Channel, 
                        VNic11QuerySelectedPhyId(STA_GET_VNIC(Station)),
                        TRUE, 
                        StaChannelSwitchCompletionHandler
                        );
        
    } while(FALSE);

    return ndisStatus;
}

NDIS_STATUS
StaSetCurrentFrequency(
    _In_  PMP_EXTSTA_PORT         Station,
    _In_  ULONG                   Frequency
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if(STA_NIC_POWER_STATE_IS_OFF(Station))
        {
            ndisStatus = NDIS_STATUS_DOT11_POWER_STATE_INVALID;
            break;
        }

        //
        // A scan operation is in progress. Setting the channel could interfere
        // with the scanning process. We will not honor this request.
        //
        if (STA_DOT11_SCAN_IN_PROGRESS(Station))
        {
            ndisStatus = NDIS_STATUS_DOT11_MEDIA_IN_USE;
            break;
        }

        // If autoconfig is enabled, we dont allow this to be set
        if (STA_GET_MP_PORT(Station)->AutoConfigEnabled & DOT11_PHY_AUTO_CONFIG_ENABLED_FLAG)
        {
            ndisStatus = NDIS_STATUS_DOT11_AUTO_CONFIG_ENABLED;
            break;
        }
        
        // All H/W calls set channel
        ndisStatus = VNic11SetChannel(STA_GET_VNIC(Station), 
                        Frequency, 
                        VNic11QuerySelectedPhyId(STA_GET_VNIC(Station)),
                        FALSE,
                        StaChannelSwitchCompletionHandler
                        );
    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetCipherDefaultKey(
    _In_    PMP_EXTSTA_PORT         Station,
    _Inout_updates_to_(InformationBufferLength, *BytesRead)
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_CIPHER_DEFAULT_KEY_VALUE defaultKey;
    
    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        defaultKey = (PDOT11_CIPHER_DEFAULT_KEY_VALUE)InformationBuffer;
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(defaultKey->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_CIPHER_DEFAULT_KEY_VALUE_REVISION_1,
                sizeof(DOT11_CIPHER_DEFAULT_KEY_VALUE)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        *BytesNeeded = FIELD_OFFSET(DOT11_CIPHER_DEFAULT_KEY_VALUE, ucKey) + 
                       defaultKey->usKeyLength;
        if (InformationBufferLength < *BytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        //
        // Check the validity of the defaultKey
        //
        if (defaultKey->uKeyIndex >= VNic11DefaultKeyTableSize(STA_GET_VNIC(Station))) 
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // Check cipher algorithm
        //

        switch (defaultKey->AlgorithmId)
        {
            case DOT11_CIPHER_ALGO_CCMP:
                if (!VNic11CCMPImplemented(STA_GET_VNIC(Station), Station->Config.BSSType))
                    ndisStatus = NDIS_STATUS_INVALID_DATA;

                break;

            case DOT11_CIPHER_ALGO_TKIP:
                if (!VNic11TKIPImplemented(STA_GET_VNIC(Station)))
                    ndisStatus = NDIS_STATUS_INVALID_DATA;

                break;

            case DOT11_CIPHER_ALGO_WEP:
                if (!VNic11WEP40Implemented(STA_GET_VNIC(Station)) && 
                    !VNic11WEP104Implemented(STA_GET_VNIC(Station)))
                    ndisStatus = NDIS_STATUS_INVALID_DATA;

                break;

            case DOT11_CIPHER_ALGO_WEP40:
                if (!VNic11WEP40Implemented(STA_GET_VNIC(Station)) || 
                    (defaultKey->usKeyLength != 40 / 8))
                    ndisStatus = NDIS_STATUS_INVALID_DATA;

                break;

            case DOT11_CIPHER_ALGO_WEP104:
                if (!VNic11WEP104Implemented(STA_GET_VNIC(Station)) || 
                    (defaultKey->usKeyLength != 104 / 8))
                    ndisStatus = NDIS_STATUS_INVALID_DATA;

                break;

            default:
                ndisStatus = NDIS_STATUS_INVALID_DATA;
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Set HW default key
        //
        ndisStatus = VNic11SetDefaultKey(STA_GET_VNIC(Station),
                        defaultKey->MacAddr,
                        defaultKey->uKeyIndex, 
                        defaultKey->bStatic,
                        defaultKey->AlgorithmId,
                        defaultKey->bDelete ? 0 : defaultKey->usKeyLength,
                        defaultKey->ucKey
                        );

        *BytesRead = *BytesNeeded;
    } while(FALSE);
    
    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetCipherKeyMappingKey(
    _In_    PMP_EXTSTA_PORT         Station,
    _Inout_updates_to_(InformationBufferLength, *BytesRead)
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_BYTE_ARRAY           keyData;
    PDOT11_CIPHER_KEY_MAPPING_KEY_VALUE keyMappingKeys;
    ULONG                       keysListLength;
    ULONG                       keySize;
    
    do
    {
        *BytesRead = 0;

        keyData = (PDOT11_BYTE_ARRAY)InformationBuffer;
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(keyData->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_CIPHER_KEY_MAPPING_KEY_VALUE_BYTE_ARRAY_REVISION_1,
                sizeof(DOT11_BYTE_ARRAY)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        *BytesNeeded = FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer) + 
                       keyData->uNumOfBytes;
        if (InformationBufferLength < *BytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        keyMappingKeys = (PDOT11_CIPHER_KEY_MAPPING_KEY_VALUE)keyData->ucBuffer;
        keysListLength = keyData->uNumOfBytes;

        while (keysListLength >= FIELD_OFFSET(DOT11_CIPHER_KEY_MAPPING_KEY_VALUE, ucKey))
        {
            keySize = FIELD_OFFSET(DOT11_CIPHER_KEY_MAPPING_KEY_VALUE, ucKey) + 
                        keyMappingKeys->usKeyLength;
            if (keysListLength < keySize)
            {
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }

            //
            // Check cipher algorithm
            //
            switch (keyMappingKeys->AlgorithmId)
            {
                case DOT11_CIPHER_ALGO_CCMP:
                    if (!VNic11CCMPImplemented(STA_GET_VNIC(Station), Station->Config.BSSType))
                        ndisStatus = NDIS_STATUS_INVALID_DATA;

                    break;

                case DOT11_CIPHER_ALGO_TKIP:
                    if (!VNic11TKIPImplemented(STA_GET_VNIC(Station)))
                        ndisStatus = NDIS_STATUS_INVALID_DATA;

                    break;

                case DOT11_CIPHER_ALGO_WEP:
                    if (!VNic11WEP40Implemented(STA_GET_VNIC(Station)) && 
                        !VNic11WEP104Implemented(STA_GET_VNIC(Station)))
                        ndisStatus = NDIS_STATUS_INVALID_DATA;

                    break;

                case DOT11_CIPHER_ALGO_WEP40:
                    if (!VNic11WEP40Implemented(STA_GET_VNIC(Station)) || 
                        (keyMappingKeys->usKeyLength != 40 / 8))
                        ndisStatus = NDIS_STATUS_INVALID_DATA;

                    break;

                case DOT11_CIPHER_ALGO_WEP104:
                    if (!VNic11WEP104Implemented(STA_GET_VNIC(Station)) || 
                        (keyMappingKeys->usKeyLength != 104 / 8))
                        ndisStatus = NDIS_STATUS_INVALID_DATA;

                    break;

                default:
                    ndisStatus = NDIS_STATUS_INVALID_DATA;
            }

            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                break;
            }

            ndisStatus = VNic11SetKeyMappingKey(STA_GET_VNIC(Station),
                            keyMappingKeys->PeerMacAddr,
                            keyMappingKeys->Direction,
                            keyMappingKeys->bStatic,
                            keyMappingKeys->AlgorithmId,
                            keyMappingKeys->bDelete ? 0 : keyMappingKeys->usKeyLength,
                            keyMappingKeys->ucKey
                            );

            keysListLength -= keySize;
            keyMappingKeys = Add2Ptr(keyMappingKeys, keySize);
        }

        //
        // Return success only if all data are consumed.
        //
        if (keysListLength != 0)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
        }

        *BytesRead = *BytesNeeded;
    } while(FALSE);

    return ndisStatus;
}



NDIS_STATUS
StaConnectRequest(
    _In_  PMP_EXTSTA_PORT         Station
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    
    do
    {
        //
        // If a reset is in progress on the NIC, fail this request
        //
        if (MP_TEST_PORT_STATUS(STA_GET_MP_PORT(Station), (MP_PORT_IN_RESET)))
        {
            MpTrace(COMP_OID, DBG_SERIOUS,  ("Connection failed as a reset is in progress on this adapter\n"));
            return NDIS_STATUS_RESET_IN_PROGRESS;
        }

        //
        // If a halt is in progress on the NIC, fail this request
        //
        if (MP_TEST_PORT_STATUS(STA_GET_MP_PORT(Station), (MP_PORT_HALTING)))
        {
            MpTrace(COMP_OID, DBG_SERIOUS,  ("Connection failed as this adapter is halting\n"));
            return NDIS_STATUS_DOT11_MEDIA_IN_USE;
        }      
        
        //
        // If the NIC is paused, fail this request
        //
        if (MP_TEST_PORT_STATUS(STA_GET_MP_PORT(Station), (MP_PORT_PAUSED | MP_PORT_PAUSING)))
        {
            MpTrace(COMP_OID, DBG_SERIOUS,  ("Connection failed as this adapter is pausing\n"));
            return NDIS_STATUS_DOT11_MEDIA_IN_USE;
        }      

        //
        // If NIC is in off power state, start it
        //
        if (STA_NIC_POWER_STATE_IS_OFF(Station))
        {
            MpTrace(COMP_OID, DBG_SERIOUS,  ("Connect failed as hardware is in power off state\n"));
            ndisStatus = NDIS_STATUS_DOT11_POWER_STATE_INVALID;
            break;
        }

        if (Station->Config.BSSType == dot11_BSS_type_independent)
        {
            ndisStatus = StaConnectAdHoc(Station);
            
            if ((ndisStatus == NDIS_STATUS_SUCCESS) || (ndisStatus == NDIS_STATUS_PENDING))
            {
                STA_GET_MP_PORT(Station)->CurrentOpState = OP_STATE;
            }
        }
        else
        {
            ndisStatus = StaConnectInfra(Station);
            
            if ((ndisStatus == NDIS_STATUS_SUCCESS) || (ndisStatus == NDIS_STATUS_PENDING))
            {
                STA_GET_MP_PORT(Station)->CurrentOpState = OP_STATE;
            }
        }

    } while(FALSE);

    return ndisStatus;
}

NDIS_STATUS
StaSetDesiredBSSType(
    _In_  PMP_EXTSTA_PORT         Station,
    _In_  ULONG                   BSSType
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        if ((BSSType != dot11_BSS_type_independent) && 
            (BSSType != dot11_BSS_type_infrastructure) && 
            (BSSType != dot11_BSS_type_any))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // For our implementation, the BSS type any corresponds to
        // infrastructure
        //
        if (BSSType == dot11_BSS_type_any)
            BSSType = dot11_BSS_type_infrastructure;
        
        if (Station->Config.BSSType != (DOT11_BSS_TYPE)BSSType)
        {
            Station->Config.BSSType = BSSType;

            VNic11SetCurrentBSSType(STA_GET_VNIC(Station), (DOT11_BSS_TYPE)BSSType);

            if (Station->Config.PowerSavingLevel != DOT11_POWER_SAVING_NO_POWER_SAVING)
            {
                //
                // If the new type is infra, we need to make sure the configured power
                // saving level is in agreement with what's in HW layer. If the new type
                // is IBSS, we need to make sure power saving mode is off in HW layer.
                //
                StaSetPowerSavingLevel(Station, 
                    (BSSType == dot11_BSS_type_infrastructure ? Station->Config.PowerSavingLevel :
                        DOT11_POWER_SAVING_NO_POWER_SAVING)
                    );
                    
            }
            
            //
            // Set default auth and cipher algorithms based on the new bss type
            //
            StaSetDefaultAuthAlgo(Station);
        }

    }while (FALSE);

    return ndisStatus;
}                

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetDesiredBSSIDList(
    _Inout_        
                PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead)
                PVOID                   InformationBuffer,
    _In_        ULONG                   InformationBufferLength,
    _Out_       PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
                PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_BSSID_LIST           dot11BSSIDList = NULL;
    
    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        dot11BSSIDList = (PDOT11_BSSID_LIST)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(dot11BSSIDList->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_BSSID_LIST_REVISION_1,
                sizeof(DOT11_BSSID_LIST)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        if (dot11BSSIDList->uNumOfEntries > 0)
        {
            //
            // Ensure enough space for all the entries
            //
            *BytesNeeded = dot11BSSIDList->uNumOfEntries * sizeof(DOT11_MAC_ADDRESS) +
                FIELD_OFFSET(DOT11_BSSID_LIST, BSSIDs);
            
            if (InformationBufferLength < (*BytesNeeded))
            {
                ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
                break;
            }
        }

        // Can only support STA_DESIRED_BSSID_MAX_COUNT
        // BSSIDs
        //
        if (dot11BSSIDList->uNumOfEntries > STA_DESIRED_BSSID_MAX_COUNT)
        {
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }
        Station->Config.AcceptAnyBSSID = FALSE;

        NdisMoveMemory(&(Station->Config.DesiredBSSIDList),
            &(dot11BSSIDList->BSSIDs[0]),
            dot11BSSIDList->uNumOfEntries * sizeof(DOT11_MAC_ADDRESS)
            );

        Station->Config.DesiredBSSIDCount = dot11BSSIDList->uNumOfEntries;

        //
        // If only the broadcast MAC is present, accept any BSSID
        //
        if ((dot11BSSIDList->uNumOfEntries == 1) &&
            (ETH_IS_BROADCAST(dot11BSSIDList->BSSIDs[0]) == TRUE)
            )
        {
            Station->Config.AcceptAnyBSSID = TRUE;
        }

        *BytesRead =  FIELD_OFFSET(DOT11_BSSID_LIST, BSSIDs)
                        + dot11BSSIDList->uNumOfEntries * sizeof(DOT11_MAC_ADDRESS);

        
    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetDesiredPhyList(
    _Inout_ PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead)
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_PHY_ID_LIST          phyIdList;
    ULONG                       index;
    DOT11_SUPPORTED_PHY_TYPES   supportedPhyTypes;
    BOOLEAN                     anyPhyId = FALSE;

    do
    {
        *BytesRead = FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId);
        *BytesNeeded = 0;

        phyIdList = (PDOT11_PHY_ID_LIST)InformationBuffer;
        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(phyIdList->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_PHY_ID_LIST_REVISION_1,
                sizeof(DOT11_PHY_ID_LIST)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // If the list is too long or too short, simply return error.
        //
        if ((phyIdList->uNumOfEntries < 1) || (phyIdList->uNumOfEntries > STA_DESIRED_PHY_MAX_COUNT))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Verify length/number of entries match up
        *BytesNeeded = phyIdList->uNumOfEntries * sizeof(ULONG) +
                        FIELD_OFFSET(DOT11_PHY_ID_LIST, dot11PhyId);

        if (InformationBufferLength < *BytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        *BytesRead = *BytesNeeded;
        
        //
        // Make sure we support all the PHY IDs in the list. Since we are using
        // sequential PHY IDs, the logic below works
        //
        VNic11QuerySupportedPHYTypes(STA_GET_VNIC(Station), 0, &supportedPhyTypes);
        for (index = 0; index < phyIdList->uNumOfEntries; index++)
        {
            if (phyIdList->dot11PhyId[index] == DOT11_PHY_ID_ANY)
            {
                anyPhyId = TRUE;
            }
            else if (phyIdList->dot11PhyId[index] >= supportedPhyTypes.uTotalNumOfEntries) 
            {
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        //
        // Copy the desired PHY list.
        //
        if (anyPhyId)
        {
            Station->Config.DesiredPhyCount = 1;
            Station->Config.DesiredPhyList[0] = DOT11_PHY_ID_ANY;
        }
        else
        {
            Station->Config.DesiredPhyCount = phyIdList->uNumOfEntries;
            NdisMoveMemory(Station->Config.DesiredPhyList,
                phyIdList->dot11PhyId,
                Station->Config.DesiredPhyCount * sizeof(ULONG));
        }

        //
        // Pass to hardware
        //
        ndisStatus = VNic11SetDesiredPhyIdList(
                        STA_GET_VNIC(Station),
                        Station->Config.DesiredPhyList,
                        Station->Config.DesiredPhyCount
                        );
    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetDesiredSSIDList(
    _Inout_ PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead) 
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_SSID_LIST            dot11SSIDList = NULL;
    
    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        dot11SSIDList = (PDOT11_SSID_LIST)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(dot11SSIDList->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_SSID_LIST_REVISION_1,
                sizeof(DOT11_SSID_LIST)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Must have atleast one SSID in the list
        if (dot11SSIDList->uNumOfEntries < 1)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Verify length/number of entries match up
        *BytesNeeded = dot11SSIDList->uNumOfEntries * sizeof(DOT11_SSID) +
            FIELD_OFFSET(DOT11_SSID_LIST, SSIDs);
        if (InformationBufferLength < (*BytesNeeded))
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        // Only support Single SSID (also reported in MAX table
        // size)
        if (dot11SSIDList->uNumOfEntries != 1)
        {
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }

        // Copy the data locally
        NdisMoveMemory(&(Station->Config.SSID),
            &(dot11SSIDList->SSIDs[0]),
            sizeof(DOT11_SSID)
            );

        *BytesRead =  FIELD_OFFSET(DOT11_SSID_LIST, SSIDs)
                        + 1 * sizeof(DOT11_SSID);
        
    } while(FALSE);

    return ndisStatus;
}

NDIS_STATUS
StaDisconnectRequest(
    _In_  PMP_EXTSTA_PORT         Station
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    
    do
    {
        if (Station->Config.BSSType == dot11_BSS_type_independent)
        {
            ndisStatus = StaDisconnectAdHoc(Station);
            
            STA_GET_MP_PORT(Station)->CurrentOpState = INIT_STATE;
        }
        else
        {
            ndisStatus = StaDisconnectInfra(Station);
            
            STA_GET_MP_PORT(Station)->CurrentOpState = INIT_STATE;
        }

    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetEnabledAuthenticationAlgorithm(
    _Inout_ PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead)
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_AUTH_ALGORITHM_LIST  authAlgoList = NULL;
    
    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        authAlgoList = (PDOT11_AUTH_ALGORITHM_LIST)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(authAlgoList->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_AUTH_ALGORITHM_LIST_REVISION_1,
                sizeof(DOT11_AUTH_ALGORITHM_LIST)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Only support one authentication algorithm
        if (authAlgoList->uNumOfEntries != 1)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Verify length/number of entries match up
        *BytesNeeded = authAlgoList->uNumOfEntries * sizeof(DOT11_AUTH_ALGORITHM) +
            FIELD_OFFSET(DOT11_AUTH_ALGORITHM_LIST, AlgorithmIds);
        if (InformationBufferLength < *BytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        *BytesRead = FIELD_OFFSET(DOT11_AUTH_ALGORITHM_LIST, AlgorithmIds) +
                      1 * sizeof(DOT11_AUTH_ALGORITHM);

        // 
        // Check if we support the specified auth algorithm.
        //
        switch (authAlgoList->AlgorithmIds[0])
        {
            case DOT11_AUTH_ALGO_80211_OPEN:
                break;

            case DOT11_AUTH_ALGO_80211_SHARED_KEY:
                if (!VNic11WEP104Implemented(STA_GET_VNIC(Station)) && !VNic11WEP40Implemented(STA_GET_VNIC(Station)) ||
                    Station->Config.BSSType == dot11_BSS_type_independent)
                {
                    ndisStatus = NDIS_STATUS_INVALID_DATA;
                }

                break;

            case DOT11_AUTH_ALGO_WPA:
            case DOT11_AUTH_ALGO_WPA_PSK:
            case DOT11_AUTH_ALGO_RSNA:
                if (Station->Config.BSSType == dot11_BSS_type_independent)
                {
                    ndisStatus = NDIS_STATUS_INVALID_DATA;
                }

                // fall through

            case DOT11_AUTH_ALGO_RSNA_PSK:
                if (!VNic11CCMPImplemented(STA_GET_VNIC(Station), Station->Config.BSSType) && 
                    (!VNic11TKIPImplemented(STA_GET_VNIC(Station)) || 
                     (Station->Config.BSSType == dot11_BSS_type_independent)))
                {
                    ndisStatus = NDIS_STATUS_INVALID_DATA;
                }

                break;

            default:
                ndisStatus = NDIS_STATUS_INVALID_DATA;
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        // Copy the data locally
        if (Station->Config.AuthAlgorithm != authAlgoList->AlgorithmIds[0])
        {
            Station->Config.AuthAlgorithm = authAlgoList->AlgorithmIds[0];

            // reload enabled unicast and multicast cipher based on current bss type and auth algo.
            StaSetDefaultCipher(Station);
        }
    
        //
        // Tell HW layer of the auth algorithm
        //
        VNic11SetAuthentication(STA_GET_VNIC(Station), Station->Config.AuthAlgorithm);

        // We dont need to process anything just yet
        // store it and we will use it when the connect
        // request comes in       
    } while(FALSE);

    return ndisStatus;
}



BOOLEAN
StaValidateMulticastAuthCipherPair(
    _In_  PMP_EXTSTA_PORT         Station,
    _In_  DOT11_AUTH_ALGORITHM    AuthAlgo,
    _In_  DOT11_CIPHER_ALGORITHM  CipherAlgo
    )
{
    BOOLEAN                     WEP40Implemented = VNic11WEP40Implemented(STA_GET_VNIC(Station));
    BOOLEAN                     WEP104Implemented = VNic11WEP104Implemented(STA_GET_VNIC(Station));
    BOOLEAN                     TKIPImplemented = VNic11TKIPImplemented(STA_GET_VNIC(Station));
    BOOLEAN                     CCMPImplemented = VNic11CCMPImplemented(STA_GET_VNIC(Station), Station->Config.BSSType);

    switch (AuthAlgo)
    {
        case DOT11_AUTH_ALGO_80211_OPEN:
            return (BOOLEAN)((CipherAlgo == DOT11_CIPHER_ALGO_WEP && (WEP40Implemented || WEP104Implemented)) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_WEP104 && WEP104Implemented) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_WEP40 && WEP40Implemented) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_NONE));

        case DOT11_AUTH_ALGO_80211_SHARED_KEY:
            return (BOOLEAN)((CipherAlgo == DOT11_CIPHER_ALGO_WEP && (WEP40Implemented || WEP104Implemented)) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_WEP104 && WEP104Implemented) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_WEP40 && WEP40Implemented));

        case DOT11_AUTH_ALGO_WPA:
        case DOT11_AUTH_ALGO_WPA_PSK:
        case DOT11_AUTH_ALGO_RSNA:
        case DOT11_AUTH_ALGO_RSNA_PSK:
            return (BOOLEAN)((CipherAlgo == DOT11_CIPHER_ALGO_TKIP && TKIPImplemented) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_CCMP && CCMPImplemented) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_WEP104 && WEP104Implemented) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_WEP40 && WEP40Implemented));

        default:
            MPASSERT(FALSE);
    }

    return FALSE;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetEnabledMulticastCipherAlgorithm(
    _Inout_ PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead)
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_CIPHER_ALGORITHM_LIST    cipherAlgoList = NULL;
    ULONG                       index;

    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        cipherAlgoList = (PDOT11_CIPHER_ALGORITHM_LIST)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(cipherAlgoList->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_CIPHER_ALGORITHM_LIST_REVISION_1,
                sizeof(DOT11_CIPHER_ALGORITHM_LIST)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Must have atleast one entry in the list
        if (cipherAlgoList->uNumOfEntries < 1)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Verify length/number of entries match up
        *BytesNeeded = cipherAlgoList->uNumOfEntries * sizeof(DOT11_CIPHER_ALGORITHM) +
                       FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds);

        if (InformationBufferLength < *BytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        *BytesRead = FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds) +
                        cipherAlgoList->uNumOfEntries * sizeof(DOT11_CIPHER_ALGORITHM);

        // Only support no more than STA_MULTICAST_CIPHER_MAX_COUNT cipher algorithms
        if (cipherAlgoList->uNumOfEntries > STA_MULTICAST_CIPHER_MAX_COUNT)
        {
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }

        //
        // Check if we can support each of the cipher algorithms given current auth algorithm.
        //
        
        for (index = 0; index < cipherAlgoList->uNumOfEntries; index++)
        {
            //
            // Check if we can support the cipher algorithms given current auth algorithm.
            //
            if (!StaValidateMulticastAuthCipherPair(Station, 
                    Station->Config.AuthAlgorithm, 
                    cipherAlgoList->AlgorithmIds[index]))
            {
                ndisStatus = NDIS_STATUS_INVALID_DATA;
                break;
            }
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)        
        {
            break;
        }

        //
        // If there is only one enabled multicast cipher, we known exactly what the 
        // multicast cipher will be. Program the hardware. Otherwise, we have to wait
        // until we know which multicast cipher will be used among those enabled.
        //
        if (cipherAlgoList->uNumOfEntries == 1)
        {
            Station->Config.MulticastCipherAlgorithm = cipherAlgoList->AlgorithmIds[0];
            VNic11SetCipher(STA_GET_VNIC(Station), FALSE, cipherAlgoList->AlgorithmIds[0]);
        }
        else
        {
            Station->Config.MulticastCipherAlgorithm = DOT11_CIPHER_ALGO_NONE;
            VNic11SetCipher(STA_GET_VNIC(Station), FALSE, DOT11_CIPHER_ALGO_NONE);
        }

        // Copy the data locally
        Station->Config.MulticastCipherAlgorithmCount = cipherAlgoList->uNumOfEntries;
        for (index = 0; index < cipherAlgoList->uNumOfEntries; index++)
        {
            Station->Config.MulticastCipherAlgorithmList[index] = cipherAlgoList->AlgorithmIds[index];
        }
    
    } while(FALSE);

    return ndisStatus;
}


BOOLEAN
StaValidateUnicastAuthCipherPair(
    _In_  PMP_EXTSTA_PORT         Station,
    _In_  DOT11_AUTH_ALGORITHM    AuthAlgo,
    _In_  DOT11_CIPHER_ALGORITHM  CipherAlgo
    )
{
    BOOLEAN                     WEP40Implemented = VNic11WEP40Implemented(STA_GET_VNIC(Station));
    BOOLEAN                     WEP104Implemented = VNic11WEP104Implemented(STA_GET_VNIC(Station));
    BOOLEAN                     TKIPImplemented = VNic11TKIPImplemented(STA_GET_VNIC(Station));
    BOOLEAN                     CCMPImplemented = VNic11CCMPImplemented(STA_GET_VNIC(Station), Station->Config.BSSType);

    switch (AuthAlgo)
    {
        case DOT11_AUTH_ALGO_80211_OPEN:
            return (BOOLEAN)((CipherAlgo == DOT11_CIPHER_ALGO_WEP && (WEP40Implemented || WEP104Implemented)) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_WEP104 && WEP104Implemented) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_WEP40 && WEP40Implemented) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_NONE));

        case DOT11_AUTH_ALGO_80211_SHARED_KEY:
            return (BOOLEAN)((CipherAlgo == DOT11_CIPHER_ALGO_WEP && (WEP40Implemented || WEP104Implemented)) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_WEP104 && WEP104Implemented) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_WEP40 && WEP40Implemented));

        case DOT11_AUTH_ALGO_WPA:
        case DOT11_AUTH_ALGO_WPA_PSK:
        case DOT11_AUTH_ALGO_RSNA:
        case DOT11_AUTH_ALGO_RSNA_PSK:
            return (BOOLEAN)((CipherAlgo == DOT11_CIPHER_ALGO_TKIP && TKIPImplemented) ||
                             (CipherAlgo == DOT11_CIPHER_ALGO_CCMP && CCMPImplemented));

        default:
            MPASSERT(FALSE);

    }

    return FALSE;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetEnabledUnicastCipherAlgorithm(
    _Inout_ PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead)
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_CIPHER_ALGORITHM_LIST    cipherAlgoList = NULL;
    
    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        cipherAlgoList = (PDOT11_CIPHER_ALGORITHM_LIST)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(cipherAlgoList->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_CIPHER_ALGORITHM_LIST_REVISION_1,
                sizeof(DOT11_CIPHER_ALGORITHM_LIST)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // We only support one cipher algorithm
        if (cipherAlgoList->uNumOfEntries != 1)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Verify length/number of entries match up
        *BytesNeeded = cipherAlgoList->uNumOfEntries * sizeof(DOT11_CIPHER_ALGORITHM) +
                       FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds);

        if (InformationBufferLength < *BytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        *BytesRead = FIELD_OFFSET(DOT11_CIPHER_ALGORITHM_LIST, AlgorithmIds) +
                      1 * sizeof(DOT11_CIPHER_ALGORITHM);

        //
        // Check if we can support the cipher algorithms given current auth algorithm.
        //
        if (!StaValidateUnicastAuthCipherPair(Station, 
                Station->Config.AuthAlgorithm, 
                cipherAlgoList->AlgorithmIds[0]))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        VNic11SetCipher(STA_GET_VNIC(Station), TRUE, cipherAlgoList->AlgorithmIds[0]);

        // Copy the data locally
        Station->Config.UnicastCipherAlgorithm = cipherAlgoList->AlgorithmIds[0];
    
    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetExcludedMACAddressList(
    _Inout_ PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead)
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_MAC_ADDRESS_LIST     dot11MacAddrList = NULL;
    PSTA_INFRA_CONNECT_CONTEXT  connectContext = &(Station->ConnectContext);
    PMP_BSS_ENTRY              apEntry = NULL;
    ULONG                       i;
    PSTA_CURRENT_CONFIG         config = &(Station->Config);
    BOOLEAN                     disconnect = FALSE;

    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        dot11MacAddrList = (PDOT11_MAC_ADDRESS_LIST)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(dot11MacAddrList->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_MAC_ADDRESS_LIST_REVISION_1,
                sizeof(DOT11_MAC_ADDRESS_LIST)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        if (dot11MacAddrList->uNumOfEntries > 0)
        {
            //
            // Ensure enough space for all the entries
            //
            *BytesNeeded = dot11MacAddrList->uNumOfEntries * sizeof(DOT11_MAC_ADDRESS) +
                FIELD_OFFSET(DOT11_MAC_ADDRESS_LIST, MacAddrs);
            
            if (InformationBufferLength < (*BytesNeeded))
            {
                ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
                break;
            }
        }

        // Can only support STA_EXCLUDED_MAC_ADDRESS_MAX_COUNT
        // MAC addresses
        //
        if (dot11MacAddrList->uNumOfEntries > STA_EXCLUDED_MAC_ADDRESS_MAX_COUNT)
        {
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }

        config->IgnoreAllMACAddresses = FALSE;
        
        NdisMoveMemory(&(config->ExcludedMACAddressList),
            &(dot11MacAddrList->MacAddrs[0]),
            dot11MacAddrList->uNumOfEntries * sizeof(DOT11_MAC_ADDRESS)
            );

        config->ExcludedMACAddressCount = dot11MacAddrList->uNumOfEntries;

        //
        // If only the broadcast MAC is present, ignore all MAC addresses
        //
        if ((dot11MacAddrList->uNumOfEntries == 1) &&
            (ETH_IS_BROADCAST(dot11MacAddrList->MacAddrs[0]) == TRUE)
            )
        {
            config->IgnoreAllMACAddresses = TRUE;
        }

        *BytesRead =  FIELD_OFFSET(DOT11_MAC_ADDRESS_LIST, MacAddrs)
                        + dot11MacAddrList->uNumOfEntries * sizeof(DOT11_MAC_ADDRESS);

        // If the access point we are currently
        // associated matches this MAC address, begin disassociation
        // process
        STA_INCREMENT_REF(Station->ConnectContext.AsyncFuncCount);
        NdisAcquireSpinLock(&(connectContext->Lock));

        if ((Station->ConnectContext.ConnectState == CONN_STATE_READY_TO_ROAM) &&
            (Station->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED)
            )
        {
        
            //
            // Get the AP entry
            //
            apEntry = connectContext->ActiveAP;
            MPASSERT(apEntry != NULL);     // SYNC:

            // Check if our current BSSID is in the new excluded MAC address list
            if (config->IgnoreAllMACAddresses)
            {
                disconnect = TRUE;
            }
            else
            {
                disconnect = FALSE;        

                // Walk through the excluded MAC address list
                for (i = 0; i < config->ExcludedMACAddressCount; i++)
                {
                    if (MP_COMPARE_MAC_ADDRESS(apEntry->MacAddress,
                            config->ExcludedMACAddressList[i]) == TRUE)
                    {
                        disconnect = TRUE;
                    }
                }
            }
            
            if (disconnect)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, (" - Disconnecting from excluded MAC AP\n"));
                connectContext->ActiveAP = NULL;

                //
                // Set state to disconnected. Then, when we wake up, we would
                // perform a roam
                //
                connectContext->AssociateState = ASSOC_STATE_NOT_ASSOCIATED;

                NdisReleaseSpinLock(&(connectContext->Lock));
                MpTrace(COMP_ASSOC, DBG_LOUD, ("Low power with Active connection\n"));

                apEntry->AssocState = dot11_assoc_state_unauth_unassoc;    

                StaDisconnect(
                    Station, 
                    apEntry, 
                    ASSOC_STATE_ASSOCIATED
                    );
                
                // Release the ref on the AP entry
                STA_DECREMENT_REF(apEntry->RefCount);
            }
            else
            {
                NdisReleaseSpinLock(&(connectContext->Lock));                
            }
        }
        else
        {
            NdisReleaseSpinLock(&(connectContext->Lock));
        }

        STA_DECREMENT_REF(Station->ConnectContext.AsyncFuncCount);
        
    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetIBSSParameters(
    _Inout_ PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead)
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_IBSS_PARAMS          dot11IBSSParams = NULL;
    PVOID                       tmpBuf = NULL;

    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        if (InformationBufferLength < sizeof(DOT11_IBSS_PARAMS))
        {
            *BytesNeeded = sizeof(DOT11_IBSS_PARAMS);
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        dot11IBSSParams = (PDOT11_IBSS_PARAMS)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(dot11IBSSParams->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_IBSS_PARAMS_REVISION_1,
                sizeof(DOT11_IBSS_PARAMS)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;            
        }

        //
        // Verify IE blob length
        //
        *BytesNeeded = dot11IBSSParams->uIEsOffset + dot11IBSSParams->uIEsLength;
        if (InformationBufferLength < *BytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }
        
        if (dot11IBSSParams->uIEsLength > 0)
        {
            MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(Station)->MiniportAdapterHandle, 
                &tmpBuf,
                dot11IBSSParams->uIEsLength,
                EXTSTA_MEMORY_TAG);

            if (tmpBuf == NULL) 
            {
                *BytesRead = sizeof(DOT11_IBSS_PARAMS);
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }

            NdisMoveMemory(tmpBuf, 
                Add2Ptr(dot11IBSSParams, dot11IBSSParams->uIEsOffset),
                dot11IBSSParams->uIEsLength);
        }

        if (Station->Config.AdditionalIEData)
        {
            MP_FREE_MEMORY(Station->Config.AdditionalIEData);
        }

        Station->Config.IBSSJoinOnly = dot11IBSSParams->bJoinOnly;
        Station->Config.AdditionalIESize = dot11IBSSParams->uIEsLength;
        Station->Config.AdditionalIEData = tmpBuf;

        *BytesRead = *BytesNeeded;

    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetAssociationParameters(
    _Inout_ PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead) 
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_ASSOCIATION_PARAMS   dot11AssocParams = NULL;
    PVOID                       tmpBuf = NULL;

    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        if (InformationBufferLength < sizeof(DOT11_ASSOCIATION_PARAMS))
        {
            *BytesNeeded = sizeof(DOT11_ASSOCIATION_PARAMS);
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        dot11AssocParams = (PDOT11_ASSOCIATION_PARAMS)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(dot11AssocParams->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_ASSOCIATION_PARAMS_REVISION_1,
                sizeof(DOT11_ASSOCIATION_PARAMS)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;            
        }

        //
        // Verify IE blob length
        //
        *BytesNeeded = dot11AssocParams->uAssocRequestIEsOffset + dot11AssocParams->uAssocRequestIEsLength;
        if (InformationBufferLength < *BytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }
        
        if (dot11AssocParams->uAssocRequestIEsLength > 0)
        {
            MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(Station)->MiniportAdapterHandle, 
                &tmpBuf,
                dot11AssocParams->uAssocRequestIEsLength,
                EXTSTA_MEMORY_TAG);

            if (tmpBuf == NULL) 
            {
                *BytesRead = sizeof(DOT11_ASSOCIATION_PARAMS);
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }

            NdisMoveMemory(tmpBuf, 
                Add2Ptr(dot11AssocParams, dot11AssocParams->uAssocRequestIEsOffset),
                dot11AssocParams->uAssocRequestIEsLength);
        }

        if (Station->Config.AdditionalIEData)
        {
            MP_FREE_MEMORY(Station->Config.AdditionalIEData);
        }

        // Save the parameters
        NdisMoveMemory(Station->Config.AssocIEBSSID, dot11AssocParams->BSSID, DOT11_ADDRESS_SIZE);
        Station->Config.AdditionalIESize = dot11AssocParams->uAssocRequestIEsLength;
        Station->Config.AdditionalIEData = tmpBuf;

        *BytesRead = *BytesNeeded;

    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetPMKIDList(
    _Inout_ PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead) 
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_PMKID_LIST           PMKIDList;
    ULONG                       index1, index2;

    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        PMKIDList = (PDOT11_PMKID_LIST)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(PMKIDList->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_PMKID_LIST_REVISION_1,
                sizeof(DOT11_PMKID_LIST)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // If the list is too long or too short, simply return error.
        //
        if (PMKIDList->uNumOfEntries > STA_PMKID_MAX_COUNT || PMKIDList->uNumOfEntries < 1)
        {
            *BytesRead = FIELD_OFFSET(DOT11_PMKID_LIST, PMKIDs);
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }

        // Verify length/number of entries match up
        *BytesNeeded = PMKIDList->uNumOfEntries * sizeof(DOT11_PMKID_ENTRY) +
                       FIELD_OFFSET(DOT11_PMKID_LIST, PMKIDs);

        if (InformationBufferLength < *BytesNeeded)
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }
        
        *BytesRead = *BytesNeeded;

        //
        // Copy the PMKID list.
        //
        Station->Config.PMKIDCount = PMKIDList->uNumOfEntries;
        NdisMoveMemory(Station->Config.PMKIDList,
            PMKIDList->PMKIDs,
            Station->Config.PMKIDCount * sizeof(DOT11_PMKID_ENTRY));

        //
        // Make sure all BSSID specified in the list are in our desired BSSID list.
        //
        if (!Station->Config.AcceptAnyBSSID)
        {
            for (index1 = 0; index1 < Station->Config.PMKIDCount; index1++)
            {
                for (index2 = 0; index2 < Station->Config.DesiredBSSIDCount; index2++)
                {
                    if (MP_COMPARE_MAC_ADDRESS(Station->Config.PMKIDList[index1].BSSID,
                                               Station->Config.DesiredBSSIDList[index2]))
                    {
                        break;
                    }
                }

                //
                // BSSID of PMKID is not in our desired BSSID list. delete the PMKID.
                //
                if (index2 == Station->Config.DesiredBSSIDCount)
                {
                    if (index1 != Station->Config.PMKIDCount - 1)
                    {
                        //
                        // This is not the last entry, copy the last entry to replace this one.
                        //
                        NdisMoveMemory(Station->Config.PMKIDList + index1,
                                       Station->Config.PMKIDList + Station->Config.PMKIDCount - 1,
                                       sizeof(DOT11_PMKID_ENTRY));

                        //
                        // Have to check the current index again.
                        //
                        index1--;
                    }

                    Station->Config.PMKIDCount--;
                }
            }
        }
    } while(FALSE);

    return ndisStatus;
}


NDIS_STATUS
StaSetPowerSavingLevel(
    _In_  PMP_EXTSTA_PORT         Station,
    _In_  ULONG                   PowerSavingLevel
    )
{
    DOT11_POWER_MGMT_MODE       mode;
    NDIS_STATUS                 ndisStatus;

    // if device is in connected state and AP sent an invalid AID, 
    // we can't do power save. simplely return success here.
    // no need to configure power mgmt mode to no power saving as
    // it's already done in StaReceiveAssociationResponse when an 
    // invalid AID was received.
    if (Station->ConnectContext.ConnectState == CONN_STATE_READY_TO_ROAM &&
        Station->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED &&
        Station->Config.ValidAID == FALSE)
    {
        return NDIS_STATUS_SUCCESS;
    }

    //
    // Initialize the DOT11_POWER_MGMT_MODE structure depending on specified power level.
    //

    switch (PowerSavingLevel) 
    {
        case DOT11_POWER_SAVING_FAST_PSP:
            mode.dot11PowerMode = dot11_power_mode_powersave;
            mode.uPowerSaveLevel = DOT11_POWER_SAVE_LEVEL_FAST_PSP;
            mode.usListenInterval = STA_LISTEN_INTERVAL_LOW_PS_MODE;
            mode.usAID = Station->Config.AID;
            mode.bReceiveDTIMs = TRUE;
            break;

        case DOT11_POWER_SAVING_MAX_PSP:
        case DOT11_POWER_SAVING_MAXIMUM_LEVEL:
            mode.dot11PowerMode = dot11_power_mode_powersave;
            mode.uPowerSaveLevel = DOT11_POWER_SAVE_LEVEL_MAX_PSP;
            mode.usListenInterval = Station->Config.ListenInterval;
            mode.usAID = Station->Config.AID;
            mode.bReceiveDTIMs = TRUE;
            break;

        case DOT11_POWER_SAVING_NO_POWER_SAVING:
            mode.dot11PowerMode = dot11_power_mode_active;
            mode.usAID = 0;
            break;

        default:
            return NDIS_STATUS_INVALID_DATA;
    }

    //
    // Pass the request to hardware.
    //
    
    ndisStatus = VNic11SetPowerMgmtMode(STA_GET_VNIC(Station), &mode);
    if (ndisStatus == NDIS_STATUS_SUCCESS)
        Station->Config.PowerSavingLevel = PowerSavingLevel;

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetPrivacyExemptionList(
    _Inout_ PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(InformationBufferLength, *BytesRead)
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_PRIVACY_EXEMPTION_LIST   newList;
    PDOT11_PRIVACY_EXEMPTION_LIST   currentList = Station->Config.PrivacyExemptionList;
    ULONG                           size;

    do
    {
        *BytesRead = 0;
        *BytesNeeded = 0;

        newList = (PDOT11_PRIVACY_EXEMPTION_LIST)InformationBuffer;

        if (!MP_VERIFY_NDIS_OBJECT_HEADER_DEFAULT(newList->Header, 
                NDIS_OBJECT_TYPE_DEFAULT,
                DOT11_PRIVACY_EXEMPTION_LIST_REVISION_1,
                sizeof(DOT11_PRIVACY_EXEMPTION_LIST)))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        *BytesNeeded = FIELD_OFFSET(DOT11_PRIVACY_EXEMPTION_LIST, PrivacyExemptionEntries) +
                       newList->uNumOfEntries * sizeof(DOT11_PRIVACY_EXEMPTION);
        if (InformationBufferLength < (*BytesNeeded))
        {
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        //
        // Check if we already have the buffer allocated for storing privacy exemption list.
        // If we don't, or if the buffer isn't big enough, allocate a buffer.
        //

        size = FIELD_OFFSET(DOT11_PRIVACY_EXEMPTION_LIST, PrivacyExemptionEntries) +
               newList->uNumOfEntries * sizeof(DOT11_PRIVACY_EXEMPTION);

        if (!currentList || 
            (currentList && (currentList->uTotalNumOfEntries < newList->uNumOfEntries))) 
        {
            MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(Station)->MiniportAdapterHandle, 
                               &currentList,
                               size,
                               EXTSTA_MEMORY_TAG);
            if (currentList == NULL) 
            {
                *BytesRead = FIELD_OFFSET(DOT11_PRIVACY_EXEMPTION_LIST, PrivacyExemptionEntries);
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }

            currentList->uTotalNumOfEntries = newList->uNumOfEntries;
        }

        //
        // Copy the new privacy exemption list
        //

        currentList->uNumOfEntries = newList->uNumOfEntries;
        if (newList->uNumOfEntries)
        {
            NdisMoveMemory(currentList->PrivacyExemptionEntries,
                newList->PrivacyExemptionEntries,
                newList->uNumOfEntries * sizeof(DOT11_PRIVACY_EXEMPTION));
        }

        //
        // If new buffer is allocated, free the existing buffer if any.
        //

        if (Station->Config.PrivacyExemptionList && 
            (Station->Config.PrivacyExemptionList != currentList))
        {
            MP_FREE_MEMORY(Station->Config.PrivacyExemptionList);
        }

        //
        // Set the new buffer as the one for the current privacy exemption list.
        //

        Station->Config.PrivacyExemptionList = currentList;

        *BytesRead = size;

    } while(FALSE);

    return ndisStatus;
}


VOID
StaResetStep1(
    _In_  PMP_EXTSTA_PORT         Station
    )
{
    // Stop new ones and wait for existings scans to finish
    StaStopScan(Station);

    if (Station->Config.BSSType == dot11_BSS_type_independent)
    {
        //
        // If we are running in Ad Hoc mode, disconnect from the ad hoc network. 
        //
        if (STA_GET_MP_PORT(Station)->CurrentOpState == OP_STATE) 
        {
            StaDisconnectAdHoc(Station);
        }
    }
    else
    {
        //
        // Reset infrastructure connection
        //
        StaResetConnection(Station, TRUE);
    }
    
    // Reset the state we had maintained about for roaming
    StaResetRoamState(Station);

    // Reset the AdHoc station Info but do not clear AdHoc station list.
    StaResetAdHocStaInfo(Station, FALSE);

    // We dont clear the BSS list on a reset 
    StaInitializeStationConfig(Station);

}

VOID
StaResetStep2(
    _In_  PMP_EXTSTA_PORT         Station
    )
{
    StaResetStationConfig(Station);

    StaStartPeriodicScan(Station);
}

NDIS_STATUS
StaDot11ResetHandler(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   NdisOidRequest
    )    
{
    PMP_EXTSTA_PORT             station = MP_GET_STA_PORT(Port);
    PNDIS_OID_REQUEST           oidRequest = (PNDIS_OID_REQUEST)NdisOidRequest;
    PDOT11_RESET_REQUEST        dot11ResetRequest = oidRequest->DATA.METHOD_INFORMATION.InformationBuffer;
    PDOT11_STATUS_INDICATION    dot11StatusIndication = oidRequest->DATA.METHOD_INFORMATION.InformationBuffer;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     mutexAcquired = FALSE;

    // Set the state to be in reset
    MP_ACQUIRE_PORT_LOCK(STA_GET_MP_PORT(station), FALSE);
    MP_SET_PORT_STATUS(STA_GET_MP_PORT(station), MP_PORT_IN_RESET);
    MP_RELEASE_PORT_LOCK(STA_GET_MP_PORT(station), FALSE);

    do
    {    
        // Call the VNIC to reset its state. This would ensure that any operation
        // that we could be waiting on would get completed back to us
        ndisStatus = VNic11Dot11Reset(STA_GET_VNIC(station), dot11ResetRequest);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("VNic Reset failed with status 0x%08x\n", ndisStatus));
            break;
        }

        // The rest of the operations are serialized between pause, reset and set op mode
        PORT_ACQUIRE_PNP_MUTEX(STA_GET_MP_PORT(station));
        mutexAcquired = TRUE;

        //
        // Clean up station state
        //
        StaResetStep1(station);
        
        //
        // Notify VNIC that we may now start sending stuff its way
        //    
        ndisStatus = VNic11Dot11ResetComplete(STA_GET_VNIC(station));
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("VNic ResetComplete failed with status 0x%08x\n", ndisStatus));
            // Continuing
        }

        //
        // The base port to reset
        //
        BasePortResetPort(STA_GET_MP_PORT(station), dot11ResetRequest);

        //
        // Restore station state
        //
        StaResetStep2(station);

    } while (FALSE);

    //
    // Clear the reset bits
    //
    MP_ACQUIRE_PORT_LOCK(STA_GET_MP_PORT(station), FALSE);
    MP_CLEAR_PORT_STATUS(STA_GET_MP_PORT(station), MP_PORT_IN_RESET);
    MP_RELEASE_PORT_LOCK(STA_GET_MP_PORT(station), FALSE);

    if (mutexAcquired)
    {
        PORT_RELEASE_PNP_MUTEX(STA_GET_MP_PORT(station));
    }

    //
    // Complete the reset request with appropriate status to NDIS
    //
    dot11StatusIndication->uStatusType = DOT11_STATUS_RESET_CONFIRM;
    dot11StatusIndication->ndisStatus = ndisStatus;
    oidRequest->DATA.METHOD_INFORMATION.BytesWritten = sizeof(DOT11_STATUS_INDICATION);
    
    return ndisStatus;
}

NDIS_STATUS
StaResetRequest(
    _In_  PMP_EXTSTA_PORT         Station,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_RESET_REQUEST        dot11ResetRequest = NdisOidRequest->DATA.METHOD_INFORMATION.InformationBuffer;

    //
    // First make sure the input buffer is large enough to
    // hold a RESET_CONFIRM
    //
    if (NdisOidRequest->DATA.METHOD_INFORMATION.OutputBufferLength < sizeof(DOT11_STATUS_INDICATION)) 
    {
        NdisOidRequest->DATA.METHOD_INFORMATION.BytesNeeded = sizeof(DOT11_STATUS_INDICATION);
        return NDIS_STATUS_INVALID_LENGTH;
    }
    
    //
    // Validate the buffer length
    //
    if (NdisOidRequest->DATA.METHOD_INFORMATION.InputBufferLength < sizeof(DOT11_RESET_REQUEST)) 
    {
        NdisOidRequest->DATA.METHOD_INFORMATION.BytesNeeded = sizeof(DOT11_RESET_REQUEST);
        return NDIS_STATUS_INVALID_LENGTH;
    }
    
    //
    // Validate the buffer
    //
    switch (dot11ResetRequest->dot11ResetType) {
        case dot11_reset_type_phy:
        case dot11_reset_type_mac:
        case dot11_reset_type_phy_and_mac:
            break;

        default:
            return NDIS_STATUS_INVALID_DATA;
    }
    
    NdisOidRequest->DATA.METHOD_INFORMATION.BytesRead = sizeof(DOT11_RESET_REQUEST);

    ndisStatus = StaDot11ResetHandler(
        STA_GET_MP_PORT(Station),
        NdisOidRequest
        );
    
    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaEnumerateBSSList(
    _In_  PMP_PORT                Port,
    _Inout_updates_bytes_to_(OutputBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_ _In_range_(<=, OutputBufferLength)
          ULONG                   InputBufferLength,
    _In_ _In_range_(>=, FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer) + sizeof(DOT11_COUNTRY_OR_REGION_STRING))
    ULONG                   OutputBufferLength,
    _Out_ PULONG                  BytesRead,
    _Out_ PULONG                  BytesWritten,
    _Out_when_invalid_ndis_length_
          PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_BYTE_ARRAY           dot11ByteArray;
    DOT11_COUNTRY_OR_REGION_STRING  countryRegionString;

    do
    {
        *BytesWritten = 0;
        *BytesNeeded = 0;

        //
        // Check enough space for the 3 fields of the DOT11_BYTE_ARRAY
        //
        if (OutputBufferLength < FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer))
        {
            *BytesNeeded = sizeof(DOT11_BYTE_ARRAY);
            ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
            break;
        }

        if (InputBufferLength < sizeof(DOT11_COUNTRY_OR_REGION_STRING))
        {
            // Unspecified country/region string
            NdisZeroMemory(countryRegionString, sizeof(DOT11_COUNTRY_OR_REGION_STRING));
        }
        else
        {
            // Copy the country/region string from the OS
#pragma warning(suppress: 6385) // PREFast is getting confused about the readable size of InformationBuffer
            NdisMoveMemory(countryRegionString, InformationBuffer, sizeof(DOT11_COUNTRY_OR_REGION_STRING));
        }

        dot11ByteArray = (PDOT11_BYTE_ARRAY)InformationBuffer;

        dot11ByteArray->uNumOfBytes = 0;
        dot11ByteArray->uTotalNumOfBytes = 0;

        // Base port copies the BSS list
        ndisStatus = BasePortCopyBSSList(
                        Port,
                        countryRegionString,
                        MP_GET_STA_PORT(Port)->RegInfo->BSSEntryExpireTime,
                        dot11ByteArray,
                        OutputBufferLength
                        );

        *BytesRead = sizeof(DOT11_COUNTRY_OR_REGION_STRING);

        *BytesWritten = dot11ByteArray->uNumOfBytes + 
            FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer);
            
        *BytesNeeded = dot11ByteArray->uTotalNumOfBytes +
            FIELD_OFFSET(DOT11_BYTE_ARRAY, ucBuffer);
    } while(FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryRecvSensitivityList(
    _In_  PMP_EXTSTA_PORT         Station,
    _Inout_updates_bytes_to_(OutputBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_  ULONG                   InputBufferLength,
    _In_  ULONG                   OutputBufferLength,
    _Out_ PULONG                  BytesRead,
    _Out_ PULONG                  BytesWritten,
    _Out_when_invalid_ndis_length_
          PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_RECV_SENSITIVITY_LIST    dot11RecvSensitivityList = InformationBuffer;
    ULONG                       maxEntries;

    do
    {
        if (InputBufferLength < FIELD_OFFSET(DOT11_RECV_SENSITIVITY_LIST, dot11RecvSensitivity))
        {
            *BytesNeeded = sizeof(DOT11_RECV_SENSITIVITY_LIST);
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }

        if (OutputBufferLength < FIELD_OFFSET(DOT11_RECV_SENSITIVITY_LIST, dot11RecvSensitivity))
        {
            *BytesNeeded = sizeof(DOT11_RECV_SENSITIVITY_LIST);
            ndisStatus = NDIS_STATUS_INVALID_LENGTH;
            break;
        }

        maxEntries = ((OutputBufferLength - 
                        (FIELD_OFFSET(DOT11_RECV_SENSITIVITY_LIST, dot11RecvSensitivity))) 
                            / sizeof(DOT11_RECV_SENSITIVITY));


        ndisStatus = VNic11QueryRecvSensitivityList(
                        STA_GET_VNIC(Station),
                        maxEntries,
                        dot11RecvSensitivityList
                        );

        *BytesRead = FIELD_OFFSET(DOT11_RECV_SENSITIVITY_LIST, uNumOfEntries);
        
        *BytesWritten = FIELD_OFFSET(DOT11_RECV_SENSITIVITY_LIST, dot11RecvSensitivity) 
                            + (sizeof(DOT11_RECV_SENSITIVITY) * dot11RecvSensitivityList->uNumOfEntries);
        *BytesNeeded = FIELD_OFFSET(DOT11_RECV_SENSITIVITY_LIST, dot11RecvSensitivity) 
                            + (sizeof(DOT11_RECV_SENSITIVITY) * dot11RecvSensitivityList->uTotalNumOfEntries);
    } while(FALSE);

    return ndisStatus;
}

NDIS_STATUS
Sta11SetOperationMode(
    _In_  PMP_PORT                Port,
    _In_  ULONG                   OpMode
    )
{
    if (OpMode == DOT11_OPERATION_MODE_NETWORK_MONITOR)
    {
        // Disable autoconfig when in Netmon mode
        Port->AutoConfigEnabled = 0;
    }
    else
    {
        // By default autoconfig is enabled
        Port->AutoConfigEnabled = DOT11_PHY_AUTO_CONFIG_ENABLED_FLAG 
            | DOT11_MAC_AUTO_CONFIG_ENABLED_FLAG;
    }

    return NDIS_STATUS_SUCCESS;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaQueryInformation(
    _In_  PMP_PORT                Port,
    _In_  NDIS_OID                Oid,
    _Out_writes_bytes_to_(InformationBufferLength, *BytesWritten)
          PVOID                   InformationBuffer,
    _In_  ULONG                   InformationBufferLength,
    _Out_ PULONG                  BytesWritten,
    _Out_when_invalid_ndis_length_
          PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       ulongInfo = 0;
    BOOLEAN                     boolInfo = FALSE;

    // Initialize the result
    *BytesWritten = 0;
    *BytesNeeded = 0;

    UNREFERENCED_PARAMETER(InformationBuffer);
    UNREFERENCED_PARAMETER(InformationBufferLength);

    MpTrace(COMP_OID, DBG_TRACE,  ("Querying OID: 0x%08x\n", Oid));

    //
    // Assume OID succeeds by default. Failure cases will set it as failure.
    //
    ndisStatus = NDIS_STATUS_SUCCESS;

    
    switch (Oid)
    {
        case OID_GEN_CURRENT_PACKET_FILTER:
            {
                ulongInfo = Port->PacketFilter;
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;

        case OID_GEN_TRANSMIT_QUEUE_LENGTH: // Port
            {
                ndisStatus = NDIS_STATUS_NOT_RECOGNIZED;
            }
            break;
    
        case OID_DOT11_ACTIVE_PHY_LIST:
            {
                ndisStatus = StaQueryActivePhyList(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ATIM_WINDOW:
            {
                ulongInfo = VNic11QueryATIMWindow(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_AUTO_CONFIG_ENABLED:
            {
                ulongInfo = Port->AutoConfigEnabled;
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }        
            break;
            
        case OID_DOT11_CIPHER_DEFAULT_KEY_ID:
            {
                ulongInfo = VNic11QueryDefaultKeyId(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_CURRENT_PHY_ID:
            {
                ulongInfo = VNic11QuerySelectedPhyId(Port->VNic);
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_DESIRED_BSS_TYPE:
            {
                ulongInfo = MP_GET_STA_PORT(Port)->Config.BSSType;
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_DESIRED_BSSID_LIST:
            {
                ndisStatus = StaQueryDesiredBSSIDList(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DESIRED_PHY_LIST:
            {
                ndisStatus = StaQueryDesiredPhyList(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DESIRED_SSID_LIST:
            {
                ndisStatus = StaQueryDesiredSsidList(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_AUTHENTICATION_ALGORITHM:
            {
                ndisStatus = StaQueryEnabledAuthenticationAlgorithm(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_MULTICAST_CIPHER_ALGORITHM:
            {
                ndisStatus = StaQueryEnabledMulticastCipherAlgorithm(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_UNICAST_CIPHER_ALGORITHM:
            {
                ndisStatus = StaQueryEnabledUnicastCipherAlgorithm(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENUM_ASSOCIATION_INFO:
            {
                ndisStatus = StaEnumerateAssociationInformation(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_EXCLUDE_UNENCRYPTED:
            {
                boolInfo = MP_GET_STA_PORT(Port)->Config.ExcludeUnencrypted;
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_EXCLUDED_MAC_ADDRESS_LIST:
            {
                ndisStatus = StaQueryExcludedMACAddressList(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_EXTSTA_CAPABILITY:
            {
                ndisStatus = StaQueryExtStaCapability(Port, 
                                (PDOT11_EXTSTA_CAPABILITY)InformationBuffer
                                );
                                
                *BytesWritten = sizeof(DOT11_EXTSTA_CAPABILITY);                
            }
            break;
            
        case OID_DOT11_HARDWARE_PHY_STATE:
            {
                boolInfo = VNic11QueryHardwarePhyState(Port->VNic);
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);

            }
            break;
            
        case OID_DOT11_HIDDEN_NETWORK_ENABLED:
            {
                boolInfo = MP_GET_STA_PORT(Port)->Config.HiddenNetworkEnabled;
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_IBSS_PARAMS:
            {
                ndisStatus = StaQueryIBSSParameters(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_MEDIA_STREAMING_ENABLED:
            {
                boolInfo = MP_GET_STA_PORT(Port)->Config.MediaStreamingEnabled;
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_PMKID_LIST:
            {
                ndisStatus = StaQueryPMKIDList(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }        
            break;
            
        case OID_DOT11_POWER_MGMT_REQUEST:
            {
                ulongInfo = MP_GET_STA_PORT(Port)->Config.PowerSavingLevel; 
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
            
        case OID_DOT11_PRIVACY_EXEMPTION_LIST:
            {
                ndisStatus = StaQueryPrivacyExemptionList(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
        
            break;
            
        case OID_DOT11_SAFE_MODE_ENABLED:
            {
                boolInfo = MP_GET_STA_PORT(Port)->Config.SafeModeEnabled;
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;
            
        case OID_DOT11_STATISTICS:
            {
                ndisStatus = StaQueryDot11Statistics(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_SUPPORTED_MULTICAST_ALGORITHM_PAIR:
            {
                ndisStatus = StaQuerySupportedMulticastAlgorithmPair(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_SUPPORTED_UNICAST_ALGORITHM_PAIR:
            {
                ndisStatus = StaQuerySupportedUnicastAlgorithmPair(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }            break;
            
        case OID_DOT11_UNICAST_USE_GROUP_ENABLED:
            {
                boolInfo = MP_GET_STA_PORT(Port)->Config.UnicastUseGroupEnabled;
                NdisMoveMemory(InformationBuffer, &boolInfo, sizeof(BOOLEAN));
                *BytesWritten = sizeof(BOOLEAN);
            }
            break;

        case OID_DOT11_UNREACHABLE_DETECTION_THRESHOLD:
            {
                ulongInfo = MP_GET_STA_PORT(Port)->Config.UnreachableDetectionThreshold;
                NdisMoveMemory(InformationBuffer, &ulongInfo, sizeof(ULONG));
                *BytesWritten = sizeof(ULONG);
            }
            break;
                
        case OID_DOT11_ASSOCIATION_PARAMS:
            {
                ndisStatus = StaQueryAssociationParameters(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesWritten,
                                BytesNeeded
                                );
            }
            break;

        default:
            // Not recognized OIDs go to the BasePort for processing
            ndisStatus = NDIS_STATUS_NOT_RECOGNIZED;
            break;
    }


    MpTrace(COMP_OID, DBG_NORMAL,  ("ExtSTA OID query completed! Port %p, OID 0x%08x, ndisStatus = 0x%08x\n",
                Port, Oid, ndisStatus));

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaSetInformation(
    _Inout_ PMP_PORT                Port,
    _In_    NDIS_OID                Oid,
    _In_reads_bytes_(InformationBufferLength)
            PVOID                   InformationBuffer,
    _In_    ULONG                   InformationBufferLength,
    _Out_   PULONG                  BytesRead,
    _Out_when_invalid_ndis_length_
            PULONG                  BytesNeeded
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       ulongInfo = 0;
    BOOLEAN                     boolInfo = FALSE;

//    MpEntry;

    *BytesRead = 0;
    *BytesNeeded = 0;

    MpTrace(COMP_OID, DBG_TRACE,  ("Setting OID: 0x%08x\n", Oid));
    
    //
    // Assume OID succeeds by default. Failure cases will set it as failure.
    //
    ndisStatus = NDIS_STATUS_SUCCESS;

    switch (Oid)
    {
        case OID_GEN_CURRENT_PACKET_FILTER:
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                *BytesRead = sizeof(ULONG);
                ndisStatus = StaSetPacketFilter(MP_GET_STA_PORT(Port), ulongInfo);
            }
            break;
            
        case OID_DOT11_SCAN_REQUEST:
            {
                ndisStatus = StaScanRequest(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CURRENT_CHANNEL:
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                *BytesRead = sizeof(ULONG);
                ndisStatus = StaSetCurrentChannel(MP_GET_STA_PORT(Port), ulongInfo);
            }
            break;
            
        case OID_DOT11_CURRENT_FREQUENCY:   // VNic
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                *BytesRead = sizeof(ULONG);
                ndisStatus = StaSetCurrentFrequency(MP_GET_STA_PORT(Port), ulongInfo);
            }
            break;
            
        case OID_DOT11_ATIM_WINDOW:
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                *BytesRead = sizeof(ULONG);
                ndisStatus = VNic11SetATIMWindow(Port->VNic, ulongInfo);
            }
            break;
            
        case OID_DOT11_AUTO_CONFIG_ENABLED:
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                *BytesRead = sizeof(ULONG);
                Port->AutoConfigEnabled = ulongInfo;
            }
            break;
            
        case OID_DOT11_CIPHER_DEFAULT_KEY:
            {
                ndisStatus = StaSetCipherDefaultKey(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CIPHER_DEFAULT_KEY_ID:
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));

                //
                // Check the Key ID range
                //
                if (ulongInfo > VNic11DefaultKeyTableSize(Port->VNic))
                {
                    ndisStatus = NDIS_STATUS_INVALID_DATA;
                }
                else
                {
                    ndisStatus = VNic11SetDefaultKeyId(Port->VNic, ulongInfo);
                }
            }
            break;
            
        case OID_DOT11_CIPHER_KEY_MAPPING_KEY:
            {
                ndisStatus = StaSetCipherKeyMappingKey(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_CONNECT_REQUEST:
            {
                ndisStatus = StaConnectRequest(MP_GET_STA_PORT(Port));
            }
            break;
            
        case OID_DOT11_CURRENT_PHY_ID:
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                *BytesRead = sizeof(ULONG);
                VNic11SetSelectedPhyId(Port->VNic, ulongInfo);
            }        
            break;
            
        case OID_DOT11_DESIRED_BSS_TYPE:
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(DOT11_BSS_TYPE));
                *BytesRead = sizeof(DOT11_BSS_TYPE);
                ndisStatus = StaSetDesiredBSSType(MP_GET_STA_PORT(Port), ulongInfo);
            }
            break;
            
        case OID_DOT11_DESIRED_BSSID_LIST:
            {
                ndisStatus = StaSetDesiredBSSIDList(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DESIRED_PHY_LIST:
            {
                ndisStatus = StaSetDesiredPhyList(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DESIRED_SSID_LIST:
            {
                ndisStatus = StaSetDesiredSSIDList(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_DISCONNECT_REQUEST:
            {
                ndisStatus = StaDisconnectRequest(MP_GET_STA_PORT(Port));
            }
            break;
            
        case OID_DOT11_ENABLED_AUTHENTICATION_ALGORITHM:
            {
                ndisStatus = StaSetEnabledAuthenticationAlgorithm(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_MULTICAST_CIPHER_ALGORITHM:
            {
                ndisStatus = StaSetEnabledMulticastCipherAlgorithm(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_ENABLED_UNICAST_CIPHER_ALGORITHM:
            {
                ndisStatus = StaSetEnabledUnicastCipherAlgorithm(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_EXCLUDE_UNENCRYPTED:
            {
                NdisMoveMemory(&boolInfo, InformationBuffer, sizeof(BOOLEAN));
                *BytesRead = sizeof(BOOLEAN);
                MP_GET_STA_PORT(Port)->Config.ExcludeUnencrypted = boolInfo;
            }
            break;
            
        case OID_DOT11_EXCLUDED_MAC_ADDRESS_LIST:
            {
                ndisStatus = StaSetExcludedMACAddressList(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_FLUSH_BSS_LIST:
            {
                Mp11FlushBSSList(Port->Adapter, Port);
                // Clear the last scan time
                MP_GET_STA_PORT(Port)->ScanContext.LastScanTime = 0;
                ndisStatus = NDIS_STATUS_SUCCESS;
            }
            break;
            
        case OID_DOT11_HIDDEN_NETWORK_ENABLED:
            {
                NdisMoveMemory(&boolInfo, InformationBuffer, sizeof(BOOLEAN));
                *BytesRead = sizeof(BOOLEAN);
                MP_GET_STA_PORT(Port)->Config.HiddenNetworkEnabled = boolInfo;
            }
            break;
            
        case OID_DOT11_IBSS_PARAMS:
            {
                ndisStatus = StaSetIBSSParameters(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_MEDIA_STREAMING_ENABLED:
            {
                //
                // Save the setting. Next time we do a unsolicited periodic scan,
                // we will check this value and not do the scan
                //
                NdisMoveMemory(&boolInfo, InformationBuffer, sizeof(BOOLEAN));
                *BytesRead = sizeof(BOOLEAN);
                MP_GET_STA_PORT(Port)->Config.MediaStreamingEnabled = boolInfo;
            }
            break;
            
        case OID_DOT11_PMKID_LIST:
            {
                ndisStatus = StaSetPMKIDList(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_POWER_MGMT_REQUEST:
            {
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                *BytesRead = sizeof(ULONG);
                ndisStatus = StaSetPowerSavingLevel(MP_GET_STA_PORT(Port), ulongInfo);
            }        
            break;
            
        case OID_DOT11_PRIVACY_EXEMPTION_LIST:
            {
                ndisStatus = StaSetPrivacyExemptionList(MP_GET_STA_PORT(Port),
                                InformationBuffer,
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;
            
        case OID_DOT11_SAFE_MODE_ENABLED:
            {
                NdisMoveMemory(&boolInfo, InformationBuffer, sizeof(BOOLEAN));
                *BytesRead = sizeof(BOOLEAN);
                MP_GET_STA_PORT(Port)->Config.SafeModeEnabled = boolInfo;
                ndisStatus = VNic11SetSafeModeOption(Port->VNic, boolInfo);
            }
            break;
            
        case OID_DOT11_UNICAST_USE_GROUP_ENABLED:
            {
                NdisMoveMemory(&boolInfo, InformationBuffer, sizeof(BOOLEAN));
                *BytesRead = sizeof(BOOLEAN);
                MP_GET_STA_PORT(Port)->Config.UnicastUseGroupEnabled = boolInfo;
            }
            break;
            
        case OID_DOT11_UNREACHABLE_DETECTION_THRESHOLD:
            {    
                NdisMoveMemory(&ulongInfo, InformationBuffer, sizeof(ULONG));
                *BytesRead = sizeof(ULONG);
                MP_GET_STA_PORT(Port)->Config.UnreachableDetectionThreshold = ulongInfo;
            }
            break;

        case OID_DOT11_ASSOCIATION_PARAMS:
            {
                ndisStatus = StaSetAssociationParameters(MP_GET_STA_PORT(Port), 
                                InformationBuffer, 
                                InformationBufferLength,
                                BytesRead,
                                BytesNeeded
                                );
            }
            break;

        default:
            // Not recognized OIDs go to the BasePort for processing
            ndisStatus = NDIS_STATUS_NOT_RECOGNIZED;
            break;
    }


    MpTrace(COMP_OID, DBG_NORMAL,  ("ExtSTA OID set completed! Port %p, OID 0x%08x, ndisStatus = 0x%08x\n",
                Port, Oid, ndisStatus));
    
//    MpExit;
    return ndisStatus;
}


NDIS_STATUS
StaQuerySetInformation(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    )
{
    NDIS_OID                oid = NdisOidRequest->DATA.METHOD_INFORMATION.Oid;
    NDIS_STATUS             ndisStatus;

    MpTrace(COMP_OID, DBG_TRACE,  ("Querying/Setting OID: 0x%08x\n", oid));


    //
    // Assume OID succeeds by default. Failure cases will set it as failure.
    //
    ndisStatus = NDIS_STATUS_SUCCESS;
    NdisOidRequest->DATA.METHOD_INFORMATION.BytesWritten = 0;
    NdisOidRequest->DATA.METHOD_INFORMATION.BytesRead = 0;
    NdisOidRequest->DATA.METHOD_INFORMATION.BytesNeeded = 0;
    
    switch(oid)
    {
        case OID_DOT11_RESET_REQUEST:
            {
                ndisStatus = StaResetRequest(
                                MP_GET_STA_PORT(Port),
                                NdisOidRequest
                                );
            }
            break;

        case OID_DOT11_ENUM_BSS_LIST:
            {
                ndisStatus = StaEnumerateBSSList(
                                Port,
                                NdisOidRequest->DATA.METHOD_INFORMATION.InformationBuffer,
                                NdisOidRequest->DATA.METHOD_INFORMATION.InputBufferLength,
                                NdisOidRequest->DATA.METHOD_INFORMATION.OutputBufferLength,
                                (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesRead,
                                (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesWritten,
                                (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesNeeded
                                );
            }
            break;

        case OID_DOT11_RECV_SENSITIVITY_LIST:
            {
                ndisStatus = StaQueryRecvSensitivityList(
                                MP_GET_STA_PORT(Port),
                                NdisOidRequest->DATA.METHOD_INFORMATION.InformationBuffer,
                                NdisOidRequest->DATA.METHOD_INFORMATION.InputBufferLength,
                                NdisOidRequest->DATA.METHOD_INFORMATION.OutputBufferLength,
                                (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesRead,
                                (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesWritten,
                                (PULONG)&NdisOidRequest->DATA.METHOD_INFORMATION.BytesNeeded
                                );
            }
            break;

        default:
            ndisStatus = NDIS_STATUS_NOT_RECOGNIZED;
            break;
    }

    return ndisStatus;
}


_At_(Attr->ExtSTAAttributes, _Post_notnull_)
NDIS_STATUS
Sta11Fill80211Attributes(
    _In_  PMP_PORT                Port,
    _Inout_ PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    )
{
    DOT11_EXTSTA_CAPABILITY     ExtStaCap;
    NDIS_STATUS                 ndisStatus;

    // We support ExtSTA and Netmon mode
    Attr->OpModeCapability |= (DOT11_OPERATION_MODE_EXTENSIBLE_STATION |
                                DOT11_OPERATION_MODE_NETWORK_MONITOR);


    MP_ALLOCATE_MEMORY(Port->MiniportAdapterHandle, 
                       &Attr->ExtSTAAttributes,
                       sizeof(DOT11_EXTSTA_ATTRIBUTES),
                       PORT_MEMORY_TAG);
    if (Attr->ExtSTAAttributes == NULL)
    {
        return NDIS_STATUS_RESOURCES;
    }

    NdisZeroMemory(Attr->ExtSTAAttributes, sizeof(DOT11_EXTSTA_ATTRIBUTES));
    
    //
    // First part of the attribute is the same as the capability. Get it
    // from Sta11QueryExtStaCapability.
    //

    ndisStatus = StaQueryExtStaCapability(Port, &ExtStaCap);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        return ndisStatus;
    }

    MP_ASSIGN_NDIS_OBJECT_HEADER(Attr->ExtSTAAttributes->Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_EXTSTA_ATTRIBUTES_REVISION_1,
                                 sizeof(DOT11_EXTSTA_ATTRIBUTES));

    Attr->ExtSTAAttributes->uScanSSIDListSize = ExtStaCap.uScanSSIDListSize;
    Attr->ExtSTAAttributes->uDesiredBSSIDListSize = ExtStaCap.uDesiredBSSIDListSize;
    Attr->ExtSTAAttributes->uDesiredSSIDListSize = ExtStaCap.uDesiredSSIDListSize;
    Attr->ExtSTAAttributes->uExcludedMacAddressListSize = ExtStaCap.uExcludedMacAddressListSize;
    Attr->ExtSTAAttributes->uPrivacyExemptionListSize = ExtStaCap.uPrivacyExemptionListSize;
    Attr->ExtSTAAttributes->uKeyMappingTableSize = ExtStaCap.uKeyMappingTableSize;
    Attr->ExtSTAAttributes->uDefaultKeyTableSize = ExtStaCap.uDefaultKeyTableSize;
    Attr->ExtSTAAttributes->uWEPKeyValueMaxLength = ExtStaCap.uWEPKeyValueMaxLength;
    Attr->ExtSTAAttributes->uPMKIDCacheSize = ExtStaCap.uPMKIDCacheSize;
    Attr->ExtSTAAttributes->uMaxNumPerSTADefaultKeyTables = ExtStaCap.uMaxNumPerSTADefaultKeyTables;
    Attr->ExtSTAAttributes->bStrictlyOrderedServiceClassImplemented = FALSE;

    //
    // Safe mode enabled
    //
    Attr->ExtSTAAttributes->bSafeModeImplemented = TRUE;

    //
    // 11d stuff.
    //
    Attr->ExtSTAAttributes->uNumSupportedCountryOrRegionStrings = 0;
    Attr->ExtSTAAttributes->pSupportedCountryOrRegionStrings = NULL;
    
    do
    {
        //
        // Get unicast algorithm pair list for infrastructure
        //
        ndisStatus = StaGetAlgorithmPair(Port,
                        dot11_BSS_type_infrastructure,
                        StaQuerySupportedUnicastAlgorithmPairCallback,
                        &Attr->ExtSTAAttributes->pInfraSupportedUcastAlgoPairs,
                        &Attr->ExtSTAAttributes->uInfraNumSupportedUcastAlgoPairs
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            break;

        //
        // Get multicast algorithm pair list for infrastructure
        //
        ndisStatus = StaGetAlgorithmPair(Port,
                        dot11_BSS_type_infrastructure,
                        StaQuerySupportedMulticastAlgorithmPairCallback,
                        &Attr->ExtSTAAttributes->pInfraSupportedMcastAlgoPairs,
                        &Attr->ExtSTAAttributes->uInfraNumSupportedMcastAlgoPairs
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            break;

        //
        // Get unicast algorithm pair list for ad hoc
        //
        ndisStatus = StaGetAlgorithmPair(Port,
                        dot11_BSS_type_independent,
                        StaQuerySupportedUnicastAlgorithmPairCallback,
                        &Attr->ExtSTAAttributes->pAdhocSupportedUcastAlgoPairs,
                        &Attr->ExtSTAAttributes->uAdhocNumSupportedUcastAlgoPairs
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            break;

        //
        // Get multicast algorithm pair list for ad hoc
        //
        ndisStatus = StaGetAlgorithmPair(Port,
                        dot11_BSS_type_independent,
                        StaQuerySupportedMulticastAlgorithmPairCallback,
                        &Attr->ExtSTAAttributes->pAdhocSupportedMcastAlgoPairs,
                        &Attr->ExtSTAAttributes->uAdhocNumSupportedMcastAlgoPairs
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            break;

    }    while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        Sta11Cleanup80211Attributes(Port, Attr);
    }
    
    return ndisStatus;
}


VOID
Sta11Cleanup80211Attributes(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    )
{
    UNREFERENCED_PARAMETER(Port);
    
    if (Attr->ExtSTAAttributes)
    {
        if (Attr->ExtSTAAttributes->pSupportedCountryOrRegionStrings)
        {
            MP_FREE_MEMORY(Attr->ExtSTAAttributes->pSupportedCountryOrRegionStrings);
        }
        if (Attr->ExtSTAAttributes->pInfraSupportedUcastAlgoPairs)
        {
            MP_FREE_MEMORY(Attr->ExtSTAAttributes->pInfraSupportedUcastAlgoPairs);
        }

        if (Attr->ExtSTAAttributes->pInfraSupportedMcastAlgoPairs)
        {
            MP_FREE_MEMORY(Attr->ExtSTAAttributes->pInfraSupportedMcastAlgoPairs);
        }

        if (Attr->ExtSTAAttributes->pAdhocSupportedUcastAlgoPairs)
        {
            MP_FREE_MEMORY(Attr->ExtSTAAttributes->pAdhocSupportedUcastAlgoPairs);
        }

        if (Attr->ExtSTAAttributes->pAdhocSupportedMcastAlgoPairs)
        {
            MP_FREE_MEMORY(Attr->ExtSTAAttributes->pAdhocSupportedMcastAlgoPairs);
        }

        MP_FREE_MEMORY(Attr->ExtSTAAttributes);

        Attr->ExtSTAAttributes = NULL;
    }

}

NDIS_STATUS
Sta11OidHandler(
    _In_  PMP_PORT                Port,
    _In_  PNDIS_OID_REQUEST       NdisOidRequest
    )
{

    NDIS_STATUS                 ndisStatus = NDIS_STATUS_NOT_SUPPORTED;

    switch(NdisOidRequest->RequestType)
    {
        case NdisRequestQueryInformation:
        case NdisRequestQueryStatistics:
            ndisStatus = StaQueryInformation(
                            Port,
                            NdisOidRequest->DATA.QUERY_INFORMATION.Oid,
                            NdisOidRequest->DATA.QUERY_INFORMATION.InformationBuffer,
                            NdisOidRequest->DATA.QUERY_INFORMATION.InformationBufferLength,
                            (PULONG)&NdisOidRequest->DATA.QUERY_INFORMATION.BytesWritten,
                            (PULONG)&NdisOidRequest->DATA.QUERY_INFORMATION.BytesNeeded
                            );
            break;

        case NdisRequestSetInformation:
            ndisStatus = StaSetInformation(
                            Port,
                            NdisOidRequest->DATA.SET_INFORMATION.Oid,
                            NdisOidRequest->DATA.SET_INFORMATION.InformationBuffer,
                            NdisOidRequest->DATA.SET_INFORMATION.InformationBufferLength,
                            (PULONG)&NdisOidRequest->DATA.SET_INFORMATION.BytesRead,
                            (PULONG)&NdisOidRequest->DATA.SET_INFORMATION.BytesNeeded
                            );
            break;

        case NdisRequestMethod:
            ndisStatus = StaQuerySetInformation(
                            Port,
                            NdisOidRequest
                            );
            break;


        default:
            ndisStatus = NDIS_STATUS_NOT_RECOGNIZED;
            break;
    }

    if (ndisStatus == NDIS_STATUS_NOT_RECOGNIZED)
    {
        // Let the base port process it
        ndisStatus = BasePortOidHandler(Port, NdisOidRequest);
    }
    else if ((ndisStatus != NDIS_STATUS_SUCCESS) && (ndisStatus != NDIS_STATUS_PENDING))
    {
        // OID has failed here
        MpTrace(COMP_OID, DBG_NORMAL, ("NDIS_OID_REQUEST failed in ExtSTA Port. Status = 0x%08x\n", 
            ndisStatus));
    }
    
    return ndisStatus;
}


VOID
StaPowerSleep(
    _In_  PMP_EXTSTA_PORT         Station
    )
{
    PSTA_INFRA_CONNECT_CONTEXT  pConnectContext = &(Station->ConnectContext);
    PMP_BSS_ENTRY      pAPEntry = NULL;
    
    //
    // When going to sleep, we should have been paused and have already
    // stopped periodic scanning. Ensure that that is the case
    //
    MPASSERT(STA_TEST_SCAN_FLAG(Station, STA_STOP_PERIODIC_SCAN));

    //
    // We are going to sleep. Unless we are connecting to a hidden network, we wont
    // add the SSID to the probe request
    //
    if (Station->Config.HiddenNetworkEnabled)
    {
        // Hidden network, add SSID
        Station->ScanContext.SSIDInProbeRequest = TRUE;
    }
    else
    {
        // Not hidden network, dont add SSID
        Station->ScanContext.SSIDInProbeRequest = FALSE;
    }
    
    //
    // If we are associated with an AP, we would disassociate
    // and later when we wake up, we would perform a roam
    //
    STA_INCREMENT_REF(Station->ConnectContext.AsyncFuncCount);    
    NdisAcquireSpinLock(&(pConnectContext->Lock));

    if ((Station->ConnectContext.ConnectState == CONN_STATE_READY_TO_ROAM) &&
        (Station->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED)
        )
    {
        //
        // Get the AP entry
        //
        pAPEntry = pConnectContext->ActiveAP;
        pConnectContext->ActiveAP = NULL;

        //
        // Set state to disconnected. Then, when we wake up, we would
        // perform a roam
        //
        pConnectContext->AssociateState = ASSOC_STATE_NOT_ASSOCIATED;

        //
        // Save the association channel of this AP & we would do a scan
        // on that channel first and attempt to roam quickly
        //
        Station->ConnectContext.AssociationChannel = pAPEntry->Channel;
        Station->ConnectContext.AttemptFastRoam = TRUE;
        
        NdisReleaseSpinLock(&(pConnectContext->Lock));
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Low power with Active connection\n"));

        pAPEntry->AssocState = dot11_assoc_state_unauth_unassoc;    

        // This would perform a disconnect and cause a disassociation indication to the OS
        StaDisconnect(
            Station, 
            pAPEntry, 
            ASSOC_STATE_ASSOCIATED
            );

        //
        // Clear its timestamp, so that we dont assume its around 
        // without a (fast) scan on resume
        //
        pAPEntry->HostTimestamp = 0;

        // Release the ref on the AP entry
        STA_DECREMENT_REF(pAPEntry->RefCount);
    }
    else
    {
        //
        // We are not connected. Do a scan soon after resume to get the most updated
        // scan list ready for the OS/user
        //
        Station->ScanContext.ScanSeverity = SCAN_HIGH_SEVERITY;
        
        NdisReleaseSpinLock(&(pConnectContext->Lock));
    }
    STA_DECREMENT_REF(Station->ConnectContext.AsyncFuncCount);    
}

VOID
StaPowerWakeup(
    _In_  PMP_EXTSTA_PORT         Station
    )
{
    // We dont do anything yet on resume
    UNREFERENCED_PARAMETER(Station);   
}

NDIS_STATUS
Sta11SetPower(
    _In_  PMP_PORT                Port,
    _In_  NDIS_DEVICE_POWER_STATE NewDevicePowerState
    )
{
    if (NewDevicePowerState == NdisDeviceStateD0)
    {
        // Waking up
        StaPowerWakeup(MP_GET_STA_PORT(Port));
    }
    else
    {
        // Going to Sleeping
        StaPowerSleep(MP_GET_STA_PORT(Port));
    }

    return NDIS_STATUS_SUCCESS;
}

