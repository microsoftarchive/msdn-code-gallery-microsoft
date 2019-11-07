/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_assocmgr.c

Abstract:
    Implements the association manager for ExtAP
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-24-2007    Created

Notes:

--*/
#include "precomp.h"
    
#if DOT11_TRACE_ENABLED
#include "ap_assocmgr.tmh"
#endif

/** DOT11 broadcast address */
DOT11_MAC_ADDRESS Dot11BroadcastAddress = {0XFF, 0XFF, 0XFF, 0XFF, 0XFF, 0XFF};


// Default Privacy Exemption List
DOT11_PRIVACY_EXEMPTION_LIST
DefaultPrivacyExemptionList =
{
    // Header
    {
        NDIS_OBJECT_TYPE_DEFAULT,                   // Type
        DOT11_PRIVACY_EXEMPTION_LIST_REVISION_1,    // Revision
        sizeof(DOT11_PRIVACY_EXEMPTION_LIST)        // Size
    },

    // uNumOfEntries
    0,

    // uTotalNumOfEntries
    0,

    // PrivacyExemptionEntries
    {0}
};

PDOT11_PRIVACY_EXEMPTION_LIST pDefaultPrivacyExemptionList = &DefaultPrivacyExemptionList;

/** Internal supporting functions */
NDIS_STATUS
AmMatchRsnInfo(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PRSN_IE_INFO pRsnIeInfo,
    _Out_ PUSHORT   pStatusCode
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    USHORT  statusCode = DOT11_FRAME_STATUS_SUCCESSFUL;
    DOT11_CIPHER_ALGORITHM  cipher;
    DOT11_AUTH_ALGORITHM    authAlgo;
    ULONG                   index;

    do {
        if (pRsnIeInfo->Version != 1)
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            statusCode = DOT11_FRAME_STATUS_UNSUPPORTED_RSN_IE_VERSION;
            break;
        }

        //
        // check group cipher
        //
        cipher = Dot11GetGroupCipherFromRSNIEInfo(pRsnIeInfo);
        if (cipher != AssocMgr->MulticastCipherAlgorithm)
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            statusCode = DOT11_FRAME_STATUS_INVALID_GROUP_CIPHER;
            break;
        }

        //
        // check unicast cipher
        //
        for (index = 0; index < pRsnIeInfo->PairwiseCipherCount; index++)
        {
            cipher = Dot11GetPairwiseCipherFromRSNIEInfo(pRsnIeInfo, (USHORT)index);
            if (cipher == AssocMgr->UnicastCipherAlgorithm)
                break;
        }
        if (index == pRsnIeInfo->PairwiseCipherCount)
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            statusCode = DOT11_FRAME_STATUS_INVALID_PAIRWISE_CIPHER;
            break;
        }

        //
        // check AKM suite
        //
        for (index = 0; index < pRsnIeInfo->AKMSuiteCount; index++)
        {
            authAlgo = Dot11GetAKMSuiteFromRSNIEInfo(pRsnIeInfo, (USHORT)index);
            if (authAlgo == AssocMgr->AuthAlgorithm)
                break;
        }
        if (index == pRsnIeInfo->AKMSuiteCount)
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            statusCode = DOT11_FRAME_STATUS_INVALID_AKMP;
            break;
        }

    }
    while(FALSE);

    *pStatusCode = statusCode;
    return ndisStatus;
}

/** Enable WPS */
VOID
AmEnableWps(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    AssocMgr->EnableWps = TRUE;
    
    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): WPS is enabled.",
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
}

/** Disable WPS */
VOID
AmDisableWps(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    AssocMgr->EnableWps = FALSE;

    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): WPS is disabled.",
                AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
}

/** Get peer info */
VOID
AmGetPeerInfo(
    _In_ PAP_STA_ENTRY StaEntry,
    _Out_ PDOT11_PEER_INFO PeerInfo
    )
{
    // clear association info
    NdisZeroMemory(PeerInfo, sizeof(DOT11_PEER_INFO));

    // MAC address
    RtlCopyMemory(
        PeerInfo->MacAddress,
        StaEntry->MacHashEntry.MacKey,
        sizeof(DOT11_MAC_ADDRESS)
        );

    // capability information
    PeerInfo->usCapabilityInformation = StaEntry->CapabilityInformation.usValue;

    // auth/cipher
    PeerInfo->AuthAlgo = StaEntry->AuthAlgo;
    PeerInfo->UnicastCipherAlgo = StaEntry->UnicastCipher;
    PeerInfo->MulticastCipherAlgo = StaEntry->MulticastCipher;

    // WPS enabled
    PeerInfo->bWpsEnabled = StaEntry->WpsEnabled;
    
    // listen interval
    PeerInfo->usListenInterval = StaEntry->ListenInterval;

    // supported rates
    RtlCopyMemory(
        PeerInfo->ucSupportedRates,
        StaEntry->SupportedRateSet.ucRateSet,
        sizeof(UCHAR) * StaEntry->SupportedRateSet.uRateSetLength
        );

    // association ID
    PeerInfo->usAssociationID = StaEntry->Aid;

    // association state
    PeerInfo->AssociationState = StaEntry->AssocState;

    // power mode
    PeerInfo->PowerMode = StaEntry->PowerMode;

    // association up time
    PeerInfo->liAssociationUpTime = StaEntry->AssocUpTime;

    // statistics
    RtlCopyMemory(
        &PeerInfo->Statistics, 
        &StaEntry->Statistics, 
        sizeof(DOT11_PEER_STATISTICS)
        );
}

/** 
 * MAC hash table enum callback function to get peer information
 * The caller must hold a read lock and make sure the buffer is big enough.
 */
BOOLEAN
AmGetPeerInfoCallback(
    _In_ PMAC_HASH_TABLE Table,
    _In_ PMAC_HASH_ENTRY MacEntry,
    _In_ PVOID CallbackCtxt
    )
{
    PDOT11_PEER_INFO_LIST peerInfoList = (PDOT11_PEER_INFO_LIST)CallbackCtxt;
    PAP_STA_ENTRY staEntry = CONTAINING_RECORD(MacEntry, AP_STA_ENTRY, MacHashEntry);

    UNREFERENCED_PARAMETER(Table);

    MPASSERT(peerInfoList->uNumOfEntries < peerInfoList->uTotalNumOfEntries);
    
    // get association info
    AmGetPeerInfo(staEntry, &peerInfoList->PeerInfo[peerInfoList->uNumOfEntries++]);

    return TRUE;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmAllocateStaEntry(
    _In_ const PAP_ASSOC_MGR AssocMgr,
    _In_ const DOT11_MAC_ADDRESS * StaMacAddr,
    _Outptr_ PAP_STA_ENTRY * StaEntry
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PAP_STA_ENTRY staEntry = NULL;
    NDIS_TIMER_CHARACTERISTICS  timerChar;               

    do
    {
        // 
        // Allocate the structure
        //
        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(AssocMgr->ApPort), 
            &staEntry, 
            sizeof(AP_STA_ENTRY), 
            EXTAP_MEMORY_TAG
            );
        
        if (NULL == staEntry)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for station info.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        sizeof(AP_STA_ENTRY)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // 
        // clear everything
        //
        NdisZeroMemory(staEntry, sizeof(AP_STA_ENTRY));

        //
        // initialize association timer
        //
        NdisZeroMemory(&timerChar, sizeof(NDIS_TIMER_CHARACTERISTICS));
        
        timerChar.Header.Type = NDIS_OBJECT_TYPE_TIMER_CHARACTERISTICS;
        timerChar.Header.Revision = NDIS_TIMER_CHARACTERISTICS_REVISION_1;
        timerChar.Header.Size = sizeof(NDIS_TIMER_CHARACTERISTICS);
        timerChar.AllocationTag = EXTAP_MEMORY_TAG;
        
        timerChar.TimerFunction = AmStaAssocTimeoutCallback;
        timerChar.FunctionContext = staEntry;

        ndisStatus = NdisAllocateTimerObject(
                        AP_GET_MP_HANDLE(AssocMgr->ApPort),
                        &timerChar,
                        &staEntry->AssocTimer
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Failed to allocate station association timer."));
            break;
        }

        // 
        // initialize MAC hash entry
        //
        InitalizeMacHashEntry(
            &staEntry->MacHashEntry, 
            StaMacAddr
            );

        //
        // set pointer to the association manager
        //
        staEntry->AssocMgr = AssocMgr;
        
        // 
        // set an invalid AID
        //
        staEntry->Aid = AP_INVALID_AID;
        
        // 
        // initialize reference count
        //
        staEntry->RefCount = 1;

        // TODO: anything else to do?

        *StaEntry = staEntry;
    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MP_FREE_MEMORY(staEntry);
    }

    return ndisStatus;
}

/** 
 * MAC hash table enum callback function to free station entry
 * The caller must hold a write lock.
 */
BOOLEAN
AmRemoveStaEntryCallback(
    _In_ PMAC_HASH_TABLE Table,
    _In_ PMAC_HASH_ENTRY MacEntry,
    _In_ PVOID CallbackCtxt
    )
{
    LIST_ENTRY * head = (LIST_ENTRY *)CallbackCtxt;         // list of removed station entries

    // remove the entry from the table
    RemoveMacHashEntry(Table, MacEntry);

    // insert it into the list
    InsertTailList(head, &MacEntry->Linkage);

    return TRUE;
}

VOID
AmFreeAllStaEntries(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    LIST_ENTRY head;                        // list of stations to free
    LIST_ENTRY * entry;
    PMAC_HASH_ENTRY macEntry;
    PAP_STA_ENTRY staEntry;
    MP_RW_LOCK_STATE lockState;
    
    MPASSERT(AP_ASSOC_MGR_STATE_STOPPING == AssocMgr->State);

    //
    // Initialize the list head
    //
    InitializeListHead(&head);

    // 
    // Acquire a write lock on the MAC hash table
    //
    MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);

    // 
    // Remove all remaining entries
    //
    EnumMacEntry(
        &AssocMgr->MacHashTable, 
        AmRemoveStaEntryCallback,
        &head
        );

    // 
    // Release lock
    //
    MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);

    //
    // Free all entries
    //
    while(!IsListEmpty(&head))
    {
        entry = RemoveHeadList(&head);
        macEntry = CONTAINING_RECORD(entry, MAC_HASH_ENTRY, Linkage);
        staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);

        //
        // Deref the station.
        // The station entry is freed when ref count reaches zero.
        //
        ApDerefSta(staEntry);
    }
}

/** Filter out unsupported rates */
VOID
AmFilterUnsupportedRates(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_RATE_SET StaRateSet
    )
{
    ULONG i, j;

    //
    // Filter out any rates that are not supported by AP
    //
    i = 0;
    while (i < StaRateSet->uRateSetLength) 
    {
        for (j = 0; j < AssocMgr->OperationalRateSet.uRateSetLength; j++) 
        {
            if ((StaRateSet->ucRateSet[i] & 0x7f) == (AssocMgr->OperationalRateSet.ucRateSet[j] & 0x7f))
                break;
        }

        //
        // remove the rate if it is not in AP's rate set
        //
        if (j == AssocMgr->OperationalRateSet.uRateSetLength)
        {
            StaRateSet->uRateSetLength--;
            StaRateSet->ucRateSet[i] = StaRateSet->ucRateSet[StaRateSet->uRateSetLength];
        }
        else
        {
            i++;
        }
    }

}

/** Initialize Association Manager */
NDIS_STATUS
ApInitializeAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_TIMER_CHARACTERISTICS  timerChar;               
        
    do
    {
        // 
        // association manager must not be initialized already
        //
        MPASSERT(AP_ASSOC_MGR_STATE_NOT_INITIALIZED == AssocMgr->State);
        
        if (AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Association manager is already initialized.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        // 
        // set the pointer to the port
        //
        AssocMgr->ApPort = ApPort;

        // 
        // initialize RW lock for MAC hash table
        //
        ndisStatus = MP_ALLOCATE_READ_WRITE_LOCK(&AssocMgr->MacHashTableLock, AP_GET_MP_HANDLE(AssocMgr->ApPort));
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Failed to allocate read write lock for association manager.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        //
        // initialize station inactive timer
        //
        NdisZeroMemory(&timerChar, sizeof(NDIS_TIMER_CHARACTERISTICS));
        
        timerChar.Header.Type = NDIS_OBJECT_TYPE_TIMER_CHARACTERISTICS;
        timerChar.Header.Revision = NDIS_TIMER_CHARACTERISTICS_REVISION_1;
        timerChar.Header.Size = sizeof(NDIS_TIMER_CHARACTERISTICS);
        timerChar.AllocationTag = EXTAP_MEMORY_TAG;
        
        timerChar.TimerFunction = AmStaInactiveTimeoutCallback;
        timerChar.FunctionContext = AssocMgr;

        ndisStatus = NdisAllocateTimerObject(
                        AP_GET_MP_HANDLE(AssocMgr->ApPort),
                        &timerChar,
                        &AssocMgr->StaInactiveTimer
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Failed to allocate station inactive timer."));
            MP_FREE_READ_WRITE_LOCK(&AssocMgr->MacHashTableLock);
            break;
        }

        //
        // initialize events for handling asynchronous BSS start/stop calls
        //
        NdisInitializeEvent(&AssocMgr->StartBSSCompletionEvent);
        NdisInitializeEvent(&AssocMgr->StopBSSCompletionEvent);

        NdisInitializeEvent(&AssocMgr->SetChannelCompletionEvent);
        
        //
        // set inactive timer counter to zero
        //
        AssocMgr->StaInactiveTimerCounter = 0;
        
        // 
        // initialize MAC table
        //
        InitMacHashTable(&AssocMgr->MacHashTable);

        // 
        // AID table shall all be 0s
        //
        
        // TODO: anything needs to be done here?

        AssocMgr->State = AP_ASSOC_MGR_STATE_STOPPED;

        // 
        // set defaults
        //
        AmSetDefault(AssocMgr);

    } while (FALSE);
    
    return ndisStatus;
}

/** Deinitialize Association Manager */
VOID
ApDeinitializeAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    MPASSERT(AssocMgr != NULL);

    do
    {
        // 
        // association manager must be in stopped state
        //
        MPASSERT(AP_ASSOC_MGR_STATE_STOPPED == AssocMgr->State);
        
        if (AssocMgr->State != AP_ASSOC_MGR_STATE_STOPPED)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Cannot deinitialize association manager when it is not in stopped state.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        // 
        // clean up first
        //
        AmCleanup(AssocMgr);

        // 
        // deinit MAC table
        //
        DeinitializeMacHashTable(&AssocMgr->MacHashTable);

        //
        // Free the inactivity timer
        //
        if (AssocMgr->StaInactiveTimer)
        {
            NdisFreeTimerObject(AssocMgr->StaInactiveTimer);
            AssocMgr->StaInactiveTimer = NULL;
        }
            
        // 
        // free locks
        //
        MP_FREE_READ_WRITE_LOCK(&AssocMgr->MacHashTableLock);

        AssocMgr->State = AP_ASSOC_MGR_STATE_NOT_INITIALIZED;
    } while (FALSE);
    
}

/** Start Association Manager */
NDIS_STATUS
ApStartAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    USHORT i;

    MPASSERT(AssocMgr != NULL);

    do
    {
        // association shall be in stopped state
        MPASSERT(AP_ASSOC_MGR_STATE_STOPPED == AssocMgr->State);

        if (AssocMgr->State != AP_ASSOC_MGR_STATE_STOPPED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Association manager is not in stopped state when a starting request is received.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        AssocMgr->State = AP_ASSOC_MGR_STATE_STARTING;
            
        // TODO: validate AP settings

        if ( AssocMgr->bUseDefaultAlgorithms )
        {
            AmSetDefaultAuthAndCipherAlgorithms( AssocMgr );
        }

        //
        // Reset AID
        //
        for (i = 0; i < AP_AID_TABLE_SIZE; i++)
        {
            AssocMgr->AidTable[i] = 0;
        }

        AssocMgr->StaInactiveTimerCounter = 0;
        
        AssocMgr->State = AP_ASSOC_MGR_STATE_STARTED;

        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Association manager started.",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
    } while (FALSE);
    
    return ndisStatus;
}

/**
 * Stop Association Manager
 * 1. Stop accepting new association requests
 * 2. Disassociate all stations and send corresponding indications
 * 3. Doesn't have to wait for the pending association request decision from IM
 * 4. Cancel on-going scan and wait for it to complete
 */
VOID
ApStopAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    BOOLEAN timerCancelled = FALSE;
    MPASSERT(AssocMgr != NULL);

    do
    {
        // Do nothing if association manager is not in started or starting state
        MPASSERT(AP_ASSOC_MGR_STATE_STARTED == AssocMgr->State || AP_ASSOC_MGR_STATE_STARTING == AssocMgr->State);
        
        if (AssocMgr->State != AP_ASSOC_MGR_STATE_STARTED && AssocMgr->State != AP_ASSOC_MGR_STATE_STARTING)
        {
            break;
        }

        AssocMgr->State = AP_ASSOC_MGR_STATE_STOPPING;

        // TODO: stop association manager

        // 
        // Disassociate all stations
        //
        AmDisassociatePeerRequest(
            AssocMgr, 
            &Dot11BroadcastAddress, 
            DOT11_MGMT_REASON_UPSPEC_REASON             // TODO: a better reason code
            );

        //
        // Station inactive timer counter shall be zero
        //
        MPASSERT(0 == AssocMgr->StaInactiveTimerCounter);

        if (AssocMgr->StaInactiveTimerCounter != 0)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Station inactive timer counter is not zero.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

            AssocMgr->StaInactiveTimerCounter = 0;
        }            

        //
        // Stop the periodic timer
        //
        timerCancelled = NdisCancelTimerObject(AssocMgr->StaInactiveTimer);
        if (timerCancelled)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_LOUD, ("Port(%u): Station inactive timer cancelled successfully.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
        }
        
        //
        // Cancel on-going scan
        //
        if (ApGetScanState(AssocMgr))
        {
            Mp11CancelScan(AP_GET_ADAPTER(AssocMgr->ApPort), AP_GET_MP_PORT(AssocMgr->ApPort));

            // 
            // Wait for scan to complete
            //
            while (ApGetScanState(AssocMgr))
            {
                NdisMSleep(10000);
            }
        }
        
        AssocMgr->State = AP_ASSOC_MGR_STATE_STOPPED;

        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Association manager stopped.",
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
    } while (FALSE);

}

/** Restart Association Manager */
NDIS_STATUS
ApRestartAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    UNREFERENCED_PARAMETER(AssocMgr);

    // TODO: anything to do?

    return NDIS_STATUS_SUCCESS;
}

/**
 * Pause Association Manager
 * 1. Cancel on-going scan and wait for it to complete
 */
NDIS_STATUS
ApPauseAssocMgr(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    if (ApGetScanState(AssocMgr))
    {
        //
        // Cancel on-going scan
        //
        Mp11CancelScan(AP_GET_ADAPTER(AssocMgr->ApPort), AP_GET_MP_PORT(AssocMgr->ApPort));

        // 
        // Wait for scan to complete
        //
        while (ApGetScanState(AssocMgr))
        {
            NdisMSleep(10000);
        }
    }

    // TODO: anything else?
    
    return NDIS_STATUS_SUCCESS;
}


/** 
 * Set association manager to its default based on the hardware capability 
 * and registry settings
 */
VOID
AmSetDefault(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    BOOLEAN set;
    DOT11_PHY_TYPE phyType;
    PVNIC vnic = AP_GET_VNIC(AssocMgr->ApPort);

    
    MPASSERT(AssocMgr != NULL);

    // Setting default SSID
    RtlZeroMemory(
        &(AssocMgr->Ssid),
        sizeof(AssocMgr->Ssid)
        );

    // default BSSID is the hardware address
    RtlCopyMemory(
        AssocMgr->Bssid,
        VNic11QueryMACAddress(AP_GET_VNIC(AssocMgr->ApPort)),
        sizeof(DOT11_MAC_ADDRESS)
        );
    

    AssocMgr->AuthAlgorithm = AP_DEFAULT_AUTHENTICATION_ALGORITHM;

    AssocMgr->UnicastCipherAlgorithm = AP_DEFAULT_UNICAST_CIPHER_ALGORITHM;

    AssocMgr->MulticastCipherAlgorithm = AP_DEFAULT_MULTICAST_CIPHER_ALGORITHM;

    AssocMgr->bUseDefaultAlgorithms = TRUE;

    AssocMgr->ExcludeUnencrypted = AP_DEFAULT_EXCLUDE_UNENCRYPTED;

    AssocMgr->PrivacyExemptionList = pDefaultPrivacyExemptionList;

    AssocMgr->EnableWps = AP_DEFAULT_ENABLE_WPS;

    // Use something other than the default to force the change
    // to be persisted in the default layer.
    AssocMgr->BeaconEnabled = !(AP_DEFAULT_ENABLE_BEACON);

    // Persist the default Beacon enabled flag in the VNic layer
    // BUG: 203538 - This should be set
    (VOID)AmSetApBeaconMode( AssocMgr , AP_DEFAULT_ENABLE_BEACON );

    // TODO: add PHY
    
    // TODO: add HW capability logic

    // TODO: capability
    NdisZeroMemory(&AssocMgr->Capability, sizeof(DOT11_CAPABILITY));
    AssocMgr->Capability.ESS = 1;
    
    // privacy
    AssocMgr->Capability.Privacy = (AssocMgr->UnicastCipherAlgorithm != DOT11_CIPHER_ALGO_NONE) ? 1 : 0;
    
    // PHY type
    phyType = VNic11QueryCurrentPhyType(vnic);
    switch (phyType) 
    {
        case dot11_phy_type_erp:
            set = VNic11QueryShortSlotTimeOptionImplemented(vnic, FALSE);
            if (set)
            {
                set = VNic11QueryShortSlotTimeOptionEnabled(vnic, FALSE);
            }
            AssocMgr->Capability.ShortSlotTime = set ? 1 : 0;
    
            set = VNic11QueryDsssOfdmOptionImplemented(vnic, FALSE);
            if (set)
            {
                set = VNic11QueryDsssOfdmOptionEnabled(vnic, FALSE);
            }
            AssocMgr->Capability.DSSSOFDM = set ? 1 : 0;
    
            // fall through
    
        case dot11_phy_type_hrdsss:
            set = VNic11QueryShortPreambleOptionImplemented(vnic, FALSE);
            AssocMgr->Capability.ShortPreamble = set ? 1: 0;
    
            set = VNic11QueryPbccOptionImplemented(vnic, FALSE);
            AssocMgr->Capability.PBCC = set ? 1: 0;
    
            set = VNic11QueryChannelAgilityPresent(vnic, FALSE);
            if (set)
            {
                set = VNic11QueryChannelAgilityEnabled(vnic, FALSE);
            }
            AssocMgr->Capability.ChannelAgility = set ? 1 : 0;
    }
        
    // TODO: OperationalRateSet
    VNic11QueryOperationalRateSet(
        AP_GET_VNIC(AssocMgr->ApPort),
        &AssocMgr->OperationalRateSet,
        FALSE
        );

}


/** 
 * Set association manager to its defaults after a Reset OID
 */
VOID
AmRestoreDefault(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    AssocMgr->AuthAlgorithm = AP_DEFAULT_AUTHENTICATION_ALGORITHM;

    AssocMgr->UnicastCipherAlgorithm = AP_DEFAULT_UNICAST_CIPHER_ALGORITHM;

    AssocMgr->MulticastCipherAlgorithm = AP_DEFAULT_MULTICAST_CIPHER_ALGORITHM;

    AssocMgr->bUseDefaultAlgorithms = TRUE;

    AmSetDefaultAuthAndCipherAlgorithms( AssocMgr );


    AssocMgr->ExcludeUnencrypted = AP_DEFAULT_EXCLUDE_UNENCRYPTED;


    AmCleanupPrivacyExemptionList( AssocMgr );

    AmCleanupDesiredSsidList( AssocMgr );

    AmSetWpsEnabled( AssocMgr , AP_DEFAULT_ENABLE_WPS );

}

/**
 * Set default cipher based on the auth algorithm and hardware capability
 */
VOID
AmSetDefaultCipher(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    PVNIC vnic = AP_GET_VNIC(AssocMgr->ApPort);
    BOOLEAN WEP40Implemented = VNic11WEP40Implemented(vnic);
    BOOLEAN WEP104Implemented = VNic11WEP104Implemented(vnic);
    BOOLEAN CCMPImplemented = VNic11CCMPImplemented(vnic, dot11_BSS_type_infrastructure);

    switch (AssocMgr->AuthAlgorithm)
    {
        case DOT11_AUTH_ALGO_80211_OPEN:
            if (WEP104Implemented || WEP40Implemented)
            {
                AssocMgr->UnicastCipherAlgorithm = DOT11_CIPHER_ALGO_WEP;
                AssocMgr->MulticastCipherAlgorithm = DOT11_CIPHER_ALGO_WEP;
            }
            else 
            {
                AssocMgr->UnicastCipherAlgorithm = DOT11_CIPHER_ALGO_NONE;
                AssocMgr->MulticastCipherAlgorithm = DOT11_CIPHER_ALGO_NONE;
            }
            break;

        case DOT11_AUTH_ALGO_WPA:
        case DOT11_AUTH_ALGO_WPA_PSK:
        case DOT11_AUTH_ALGO_RSNA:
        case DOT11_AUTH_ALGO_RSNA_PSK:
            MPASSERT(VNic11TKIPImplemented(vnic) || CCMPImplemented);
            if (CCMPImplemented)
            {
                AssocMgr->UnicastCipherAlgorithm = DOT11_CIPHER_ALGO_CCMP;
                AssocMgr->MulticastCipherAlgorithm = DOT11_CIPHER_ALGO_CCMP;
            }
            else 
            {
                AssocMgr->UnicastCipherAlgorithm = DOT11_CIPHER_ALGO_TKIP;
                AssocMgr->MulticastCipherAlgorithm = DOT11_CIPHER_ALGO_TKIP;
            }
            break;

        default:
            MPASSERT(FALSE);
            return;
    }

    //
    // Set ciphers to VNIC
    //
    VNic11SetCipher(vnic, TRUE, AssocMgr->UnicastCipherAlgorithm);
    VNic11SetCipher(vnic, FALSE, AssocMgr->MulticastCipherAlgorithm);
}

/**
 * Set default auth and cipher algorithms in VNIC
 */
VOID
AmSetDefaultAuthAndCipherAlgorithms
(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{

    MPASSERT(AP_DEFAULT_AUTHENTICATION_ALGORITHM == AssocMgr->AuthAlgorithm);

    VNic11SetAuthentication
    (
        AP_GET_VNIC(AssocMgr->ApPort), 
        AssocMgr->AuthAlgorithm
    );

    AmSetDefaultCipher( AssocMgr );

    return;
}

/** 
 * Clean up Association Manager Privacy Exemption List
 */
VOID
AmCleanupPrivacyExemptionList(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    MPASSERT(AssocMgr->PrivacyExemptionList);

    if (AssocMgr->PrivacyExemptionList != pDefaultPrivacyExemptionList)
    {
        MP_FREE_MEMORY(AssocMgr->PrivacyExemptionList);
        AssocMgr->PrivacyExemptionList = pDefaultPrivacyExemptionList;
    }
}

/** 
 * Clean up Association Manager Desired SSID List
 */
VOID
AmCleanupDesiredSsidList(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    // Setting default SSID
    RtlZeroMemory(
        &(AssocMgr->Ssid),
        sizeof(AssocMgr->Ssid)
        );
}


/** 
 * Clean up Association Manager 
 * Shall be called after ApStopAssocMgr()
 * All clients must have been disassociated at this point
 */
VOID
AmCleanup(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    MPASSERT(AssocMgr != NULL);

    do
    {
        // association shall be in stopped state
        MPASSERT(AP_ASSOC_MGR_STATE_STOPPED == AssocMgr->State);
        
        if (AssocMgr->State != AP_ASSOC_MGR_STATE_STOPPED)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Cannot cleanup association manager when it is not in stopped state.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }
        
        // TODO: free memory blocks
        
        AmCleanupPrivacyExemptionList( AssocMgr );

    } while (FALSE);
    
}

/**
 * Get an available AID
 * Each AID is represented as a bit in the AID table.
 * The bit is set to 1 if an AID is used.
 */
USHORT
AmAllocateAid(
    _In_ PAP_ASSOC_MGR AssocMgr
    )
{
    // TODO: hold a lock?
    USHORT aid = AP_INVALID_AID;
    USHORT i;
    UCHAR *byteFound = NULL;
    UCHAR byteMask = 1;

    // first find the UCHAR that has bit with value 0
    for (i = 0; i < AP_AID_TABLE_SIZE; i++)
    {
        if (AssocMgr->AidTable[i] != AP_AID_TABLE_UNIT_MASK)
        {
            byteFound = &AssocMgr->AidTable[i];
            break;
        }
    }

    if (byteFound != NULL)
    {
        aid = i * AP_AID_TABLE_UNIT_SIZE;

        // find the position of the first bit that is 0
        while (TRUE)
        {
            aid++;
            
            if ((*byteFound & byteMask) == 0)
            {
                // find the first bit that is 0, set it to 1
                *byteFound |= byteMask;
                break;
            }

            byteMask <<= 1;
        }
    }

    MPASSERT(aid > 0);
    MPASSERT(aid != AP_INVALID_AID);
    MPASSERT(aid <= AP_MAX_AID);

    // add AID header
    aid |= AP_AID_HEADER;
    
    return aid;
}

/** Free an AID */
VOID
AmFreeAid(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ USHORT Aid
    )
{
    // TODO: hold a lock?
    UCHAR byteMask = 1;

    // remove AID header
    Aid &= ~(AP_AID_HEADER);

    MPASSERT(Aid > 0 && Aid <= AP_MAX_AID);
    if (Aid > 0 && Aid <= AP_MAX_AID)
    {
        // prepare the byte mask
        byteMask <<= (Aid - 1) % AP_AID_TABLE_UNIT_SIZE;

        // update the UCHAR that has the corresponding bit for the AID
        AssocMgr->AidTable[(Aid - 1) / AP_AID_TABLE_UNIT_SIZE] &= ~byteMask;
    }        
}

/**
 * Internal functions for OIDs
 */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmQuerySsid(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_SSID_LIST SsidList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    
    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);

    do
    {
        // only return one SSID
        if (InformationBufferLength < sizeof(DOT11_SSID_LIST))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_SSID_LIST));
        }

        if ( AssocMgr->Ssid.uSSIDLength )
        {
            SsidList->uNumOfEntries = 
                SsidList->uTotalNumOfEntries = 1;

            RtlCopyMemory(
                    &SsidList->SSIDs[0],
                    &AssocMgr->Ssid,
                    sizeof(DOT11_SSID)
                    );
        }
        else
        {
            SsidList->uNumOfEntries = 
                SsidList->uTotalNumOfEntries = 0;

            RtlZeroMemory(
                    &SsidList->SSIDs[0],
                    sizeof(DOT11_SSID)
                    );
        }

        *BytesWritten = sizeof(DOT11_SSID_LIST);
        
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
AmSetSsid(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_SSID_LIST SsidList
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN bOk = FALSE;

    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);
        
    do
    {
        // SSID can only be set when association manager is stopped.
        MPASSERT(AP_ASSOC_MGR_STATE_STOPPED == AssocMgr->State);

        bOk =
        (
            (1 == SsidList->uNumOfEntries) &&
            (1 == SsidList->uTotalNumOfEntries) &&
            (1 <= SsidList->SSIDs[0].uSSIDLength) &&
            (DOT11_SSID_MAX_LENGTH >= SsidList->SSIDs[0].uSSIDLength)
        ) ? TRUE : FALSE;
        if (!bOk)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Invalid desired SSID."));
            break;
        }

        if (AssocMgr->State != AP_ASSOC_MGR_STATE_STOPPED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Association manager is not in stopped state when setting desired SSID.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        // copy SSID
        RtlCopyMemory(
                &AssocMgr->Ssid,
                &SsidList->SSIDs[0],
                sizeof(DOT11_SSID)
                );
        
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
AmSetBssid(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ DOT11_MAC_ADDRESS * Bssid
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);
        
    do
    {
        // BSSID can only be set when association manager is stopped.
        MPASSERT(AP_ASSOC_MGR_STATE_STOPPED == AssocMgr->State);
        if (AssocMgr->State != AP_ASSOC_MGR_STATE_STOPPED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Association manager is not in stopped state when setting BSSID.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        // copy BSSID
        RtlCopyMemory(
                AssocMgr->Bssid,
                *Bssid,
                sizeof(DOT11_MAC_ADDRESS)
                );
        
    } while (FALSE);

    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmQueryAuthAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_AUTH_ALGORITHM_LIST EnabledAuthenticationAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    
    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);

    do
    {
        // only return one auth algorithm
        if (InformationBufferLength < sizeof(DOT11_AUTH_ALGORITHM_LIST))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_AUTH_ALGORITHM_LIST));
        }
    
        EnabledAuthenticationAlgorithm->uNumOfEntries = 
            EnabledAuthenticationAlgorithm->uTotalNumOfEntries = 1;

        EnabledAuthenticationAlgorithm->AlgorithmIds[0] = AssocMgr->AuthAlgorithm;

        *BytesWritten = sizeof(DOT11_AUTH_ALGORITHM_LIST);
    } while (FALSE);

    return ndisStatus;
}




NDIS_STATUS
AmSetAuthAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_AUTH_ALGORITHM_LIST EnabledAuthenticationAlgorithm
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);
        
    do
    {
        // Auth algorithm can only be set when association manager is stopped.
        MPASSERT(AP_ASSOC_MGR_STATE_STOPPED == AssocMgr->State);
        if (AssocMgr->State != AP_ASSOC_MGR_STATE_STOPPED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Association manager is not in stopped state when setting auth algorithm.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        //
        // Only supports one auth algorithm
        //
        if (EnabledAuthenticationAlgorithm->uNumOfEntries != 1)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // Validate auth algorithm
        //
        if (!ApValidateAuthAlgo(
                AssocMgr->ApPort, 
                EnabledAuthenticationAlgorithm->AlgorithmIds[0]
                ))
        {
            //
            // Invalid auth algorithm
            //
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, 
                    ("Port(%u): Invalid auth algorithm (%u).",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                        EnabledAuthenticationAlgorithm->AlgorithmIds[0]));
            break;
        }

        //
        // Set it to VNIC
        //
        VNic11SetAuthentication(
            AP_GET_VNIC(AssocMgr->ApPort), 
            EnabledAuthenticationAlgorithm->AlgorithmIds[0]
            );
        
        AssocMgr->AuthAlgorithm = EnabledAuthenticationAlgorithm->AlgorithmIds[0];

        //
        // Set default ciphers
        //
        AmSetDefaultCipher(AssocMgr);

        AssocMgr->bUseDefaultAlgorithms = FALSE;

    } while (FALSE);

    return ndisStatus;
}


_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmQueryUnicastCipherAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_CIPHER_ALGORITHM_LIST EnabledUnicastCipherAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    
    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);

    do
    {
        // only return one unicast cipher algorithm
        if (InformationBufferLength < sizeof(DOT11_CIPHER_ALGORITHM_LIST))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_CIPHER_ALGORITHM_LIST));
        }
        
        EnabledUnicastCipherAlgorithm->uNumOfEntries = 
            EnabledUnicastCipherAlgorithm->uTotalNumOfEntries = 1;

        EnabledUnicastCipherAlgorithm->AlgorithmIds[0] = AssocMgr->UnicastCipherAlgorithm;

        *BytesWritten = sizeof(DOT11_CIPHER_ALGORITHM_LIST);
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
AmSetUnicastCipherAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_CIPHER_ALGORITHM_LIST EnabledUnicastCipherAlgorithm
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);
        
    do
    {
        // 
        // Unicast cipher algorithm can only be set when association manager is stopped.
        //
        MPASSERT(AP_ASSOC_MGR_STATE_STOPPED == AssocMgr->State);
        if (AssocMgr->State != AP_ASSOC_MGR_STATE_STOPPED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Association manager is not in stopped state when setting unicast cipher algorithm.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        //
        // Only supports one cipher algorithm
        //
        if (EnabledUnicastCipherAlgorithm->uNumOfEntries != 1)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // Validate auth/cipher pair
        //
        if (!ApValidateUnicastAuthCipherPair(
                AssocMgr->ApPort, 
                AssocMgr->AuthAlgorithm, 
                EnabledUnicastCipherAlgorithm->AlgorithmIds[0]
                ))
        {
            //
            // Invalid cipher algorithm
            //
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, 
                    ("Port(%u): Invalid unicast cipher algorithm (%u) for auth algorithm (%u).",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                        EnabledUnicastCipherAlgorithm->AlgorithmIds[0], AssocMgr->AuthAlgorithm));
            break;
        }

        //
        // Set it to VNIC
        //
        VNic11SetCipher(
            AP_GET_VNIC(AssocMgr->ApPort), 
            TRUE,               // is unicast
            EnabledUnicastCipherAlgorithm->AlgorithmIds[0]
            );
        
        AssocMgr->UnicastCipherAlgorithm = EnabledUnicastCipherAlgorithm->AlgorithmIds[0];
        
        // update Capability info
        AssocMgr->Capability.Privacy = (AssocMgr->UnicastCipherAlgorithm != DOT11_CIPHER_ALGO_NONE) ? 1 : 0;

    } while (FALSE);

    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmQueryMulticastCipherAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_CIPHER_ALGORITHM_LIST EnabledMulticastCipherAlgorithm,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    
    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);

    do
    {
        // only return one multicast cipher algorithm
        if (InformationBufferLength < sizeof(DOT11_CIPHER_ALGORITHM_LIST))
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(sizeof(DOT11_CIPHER_ALGORITHM_LIST));
        }
        
        EnabledMulticastCipherAlgorithm->uNumOfEntries = 
            EnabledMulticastCipherAlgorithm->uTotalNumOfEntries = 1;

        EnabledMulticastCipherAlgorithm->AlgorithmIds[0] = AssocMgr->MulticastCipherAlgorithm;

        *BytesWritten = sizeof(DOT11_CIPHER_ALGORITHM_LIST);
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
AmSetMulticastCipherAlgorithm(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_CIPHER_ALGORITHM_LIST EnabledMulticastCipherAlgorithm
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);
        
    do
    {
        // Multicast cipher algorithm can only be set when association manager is stopped.
        MPASSERT(AP_ASSOC_MGR_STATE_STOPPED == AssocMgr->State);
        if (AssocMgr->State != AP_ASSOC_MGR_STATE_STOPPED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Association manager is not in stopped state when setting multicast cipher algorithm.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        //
        // Only supports one cipher algorithm
        //
        if (EnabledMulticastCipherAlgorithm->uNumOfEntries != 1)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // Validate auth/cipher pair
        //
        if (!ApValidateMulticastAuthCipherPair(
                AssocMgr->ApPort, 
                AssocMgr->AuthAlgorithm, 
                EnabledMulticastCipherAlgorithm->AlgorithmIds[0]
                ))
        {
            //
            // Invalid cipher algorithm
            //
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, 
                    ("Port(%u): Invalid multicast cipher algorithm (%u) for auth algorithm (%u).",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                        EnabledMulticastCipherAlgorithm->AlgorithmIds[0], AssocMgr->AuthAlgorithm));
            break;
        }

        //
        // Set it to VNIC
        //
        VNic11SetCipher(
            AP_GET_VNIC(AssocMgr->ApPort), 
            FALSE,              // is multicast
            EnabledMulticastCipherAlgorithm->AlgorithmIds[0]
            );
        
        AssocMgr->MulticastCipherAlgorithm = EnabledMulticastCipherAlgorithm->AlgorithmIds[0];
        
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
AmSetOperationalRateSet(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_RATE_SET OperationalRateSet
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);
        
    do
    {
        // Operational rate set can only be set when association manager is stopped.
        MPASSERT(AP_ASSOC_MGR_STATE_STOPPED == AssocMgr->State);
        if (AssocMgr->State != AP_ASSOC_MGR_STATE_STOPPED)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, ("Port(%u): Association manager is not in stopped state when setting operational rate set.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        // copy rate set
        RtlCopyMemory(
                &AssocMgr->OperationalRateSet,
                OperationalRateSet,
                sizeof(DOT11_RATE_SET)
                );
        
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
AmSetExcludeUnencrypted(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ BOOLEAN ExcludeUnencrypted
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    //
    // No need to hold the lock.
    // This is can only be set before AP starts.
    //
    AssocMgr->ExcludeUnencrypted = ExcludeUnencrypted;

    return ndisStatus;
}

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmQueryPrivacyExemptionList(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_PRIVACY_EXEMPTION_LIST PrivacyExemptionList,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredSize = 0; 

    do
    {
        ndisStatus = ValiatePrivacyExemptionListSize(
                        AssocMgr->PrivacyExemptionList, 
                        InformationBufferLength, 
                        &requiredSize
                        );

        if (NDIS_STATUS_INVALID_LENGTH == ndisStatus)
        {
            // 
            // the buffer is not big enough
            //
            *BytesNeeded = requiredSize;
            break;
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // 
            // this should not happen
            //
            MPASSERT(FALSE);
            break;
        }

        // 
        // the buffer is big enough, copy the list
        //
        RtlCopyMemory(
                PrivacyExemptionList,
                AssocMgr->PrivacyExemptionList,
                requiredSize
                );

        *BytesWritten = requiredSize;
    } while (FALSE);
    
    return ndisStatus;
}

NDIS_STATUS
AmSetPrivacyExemptionList(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_PRIVACY_EXEMPTION_LIST PrivacyExemptionList
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG tmpExemptionListSize = 0;
    PDOT11_PRIVACY_EXEMPTION_LIST tmpExemptionList = NULL;

    do
    {
        //
        // Get the required list size first
        //
        if (!GetRequiredListSize(
                FIELD_OFFSET(DOT11_PRIVACY_EXEMPTION_LIST, PrivacyExemptionEntries), 
                sizeof(DOT11_PRIVACY_EXEMPTION), 
                PrivacyExemptionList->uNumOfEntries, 
                &tmpExemptionListSize
                ))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        //
        // Allocate memory for the new list
        //
        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(AssocMgr->ApPort), 
            &tmpExemptionList, 
            tmpExemptionListSize, 
            EXTAP_MEMORY_TAG
            );
        
        if (NULL == tmpExemptionList)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for the exemption list.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        tmpExemptionListSize));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        //
        // Copy the list
        //
        RtlCopyMemory(
                tmpExemptionList,
                PrivacyExemptionList,
                tmpExemptionListSize
                );

        //
        // No need to hold the lock.
        // This is can only be set before AP starts.
        //
        AmCleanupPrivacyExemptionList( AssocMgr );

        AssocMgr->PrivacyExemptionList = tmpExemptionList;

    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS
AmSetWpsEnabled(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ BOOLEAN WpsEnabled
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);

    // WPS can be enabled any time
    if (WpsEnabled && !AssocMgr->EnableWps)
    {
        // WPS needs to be enabled
        // Beacon and probe response shall be handled differently
        AmEnableWps(AssocMgr);
        AssocMgr->EnableWps = TRUE;
    }
    else if (!WpsEnabled && AssocMgr->EnableWps)
    {
        // WPS needs to be disabled
        // Beacon and probe response shall be handled differently
        AmDisableWps(AssocMgr);
        AssocMgr->EnableWps = FALSE;
    }
    
    return ndisStatus;
}

NDIS_STATUS
AmSetApBeaconMode(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ BOOLEAN BeaconEnabled
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC vnic = AP_GET_VNIC(AssocMgr->ApPort);

    MPASSERT(AssocMgr->State != AP_ASSOC_MGR_STATE_NOT_INITIALIZED);

    BeaconEnabled = BeaconEnabled?TRUE:FALSE;

    while (BeaconEnabled  != AssocMgr->BeaconEnabled)
    {
        // VNic Layer needs to be informed of change.
        ndisStatus = VNic11SetBeaconEnabledFlag( vnic, BeaconEnabled );
        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            break;
        }

        // persist the value locally.
        AssocMgr->BeaconEnabled = BeaconEnabled;

        break;
    }

    return ndisStatus;
}


/** enum peer information */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmEnumPeerInfo(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _Out_ PDOT11_PEER_INFO_LIST PeerInfo,
    _In_ ULONG InformationBufferLength,
    _Out_ PULONG BytesWritten,
    _Out_when_invalid_ndis_length_ PULONG BytesNeeded
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    MP_RW_LOCK_STATE lockState;
    ULONG staEntryCount = 0;
    ULONG requiredSize = 0;
    
    do
    {
        // acquire the read lock on the MAC hash table
        MP_ACQUIRE_READ_LOCK(&AssocMgr->MacHashTableLock, &lockState);

        // get station entry count
        staEntryCount = GetMacHashTableEntryCount(&AssocMgr->MacHashTable);

        // calculate the required size
        if (!GetRequiredListSize(
                FIELD_OFFSET(DOT11_PEER_INFO_LIST, PeerInfo), 
                sizeof(DOT11_PEER_INFO), 
                staEntryCount, 
                &requiredSize
                ))
        {
            // overflow should not happen
            MPASSERT(FALSE);
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        if (InformationBufferLength < requiredSize)
        {
            SET_NEEDED_BUFFER_SIZE_AND_BREAK(requiredSize);
        }

        // the buffer is large enough to hold all information
        PeerInfo->uTotalNumOfEntries = staEntryCount;
        PeerInfo->uNumOfEntries = 0;

        // enumerate all station entries and get association info
        EnumMacEntry(
            &AssocMgr->MacHashTable, 
            AmGetPeerInfoCallback, 
            PeerInfo
            );

        // all association information is obtained
        MPASSERT(PeerInfo->uNumOfEntries == PeerInfo->uTotalNumOfEntries);
        
        *BytesWritten = requiredSize;
    } while (FALSE);

    // release the read lock
    MP_RELEASE_READ_LOCK(&AssocMgr->MacHashTableLock, &lockState);
    
    return ndisStatus;
}

NDIS_STATUS
AmSetStaPortState(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_MAC_ADDRESS PeerMacAddr,
    _In_ BOOLEAN PortOpen
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_INVALID_STATE;
    PMAC_HASH_ENTRY macEntry;
    PAP_STA_ENTRY staEntry;
    MP_RW_LOCK_STATE lockState;

    //
    // Acquire read lock on the Mac table
    //
    MP_ACQUIRE_READ_LOCK(&AssocMgr->MacHashTableLock, &lockState);

    // 
    // Remove a specific station
    //
    macEntry = LookupMacHashTable(
                &AssocMgr->MacHashTable, 
                PeerMacAddr
                );

    if (macEntry != NULL)
    {
        //
        // Change port state
        //
        staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);
        if (ApGetStaAssocState(staEntry) == dot11_assoc_state_auth_assoc)
        {
            //
            // Port state is only meaningful after the state is associated.
            //
            ApSetStaPortState(
                staEntry, 
                PortOpen ? STA_PORT_STATE_OPEN : STA_PORT_STATE_CLOSED
                );

            ndisStatus = NDIS_STATUS_SUCCESS;
        }
    }

    //
    // Release the read lock
    //
    MP_RELEASE_READ_LOCK(&AssocMgr->MacHashTableLock, &lockState);
    
    return ndisStatus;
}

/**
 * Scan complete callback
 */
NDIS_STATUS
ApScanComplete(
    _In_ PMP_PORT Port,
    _In_ PVOID Data
    )
{
    NDIS_STATUS ndisStatus = *(NDIS_STATUS*)Data;
    PAP_ASSOC_MGR assocMgr = AP_GET_ASSOC_MGR(MP_GET_AP_PORT(Port));
    MP_RW_LOCK_STATE lockState;
    PVOID scanRequestId = NULL;
    
    //
    // TODO: use another lock?
    //
    MP_ACQUIRE_WRITE_LOCK(&assocMgr->MacHashTableLock, &lockState);

    MPASSERT(assocMgr->ScanRequest.Dot11ScanRequest != NULL);

    if (assocMgr->ScanRequest.Dot11ScanRequest != NULL)
    {
        MP_FREE_MEMORY(assocMgr->ScanRequest.Dot11ScanRequest);
        assocMgr->ScanRequest.Dot11ScanRequest = NULL;
        scanRequestId = assocMgr->ScanRequestId;
        assocMgr->ScanRequestId = NULL;
    }
    
    MP_RELEASE_WRITE_LOCK(&assocMgr->MacHashTableLock, &lockState);
    
    //
    // Reset scan state
    //
    if (!ApSetScanState(assocMgr, FALSE))
    {
        //
        // This shouldn't happen
        //
        MPASSERT(FALSE);
    }

    ApIndicateDot11Status(
        assocMgr->ApPort, 
        NDIS_STATUS_DOT11_SCAN_CONFIRM,
        scanRequestId,
        &ndisStatus,
        sizeof(NDIS_STATUS)
        );
    
    return NDIS_STATUS_SUCCESS;
}

/** 
 * MAC hash table enum callback function to check whether a station
 * is in the process of connecting to the AP.
 * The caller must hold a write lock.
 */
BOOLEAN
AmPortStateStaEntryCallback(
    _In_ PMAC_HASH_TABLE Table,
    _In_ PMAC_HASH_ENTRY MacEntry,
    _In_ PVOID CallbackCtxt
    )
{
    PAP_STA_ENTRY staEntry = CONTAINING_RECORD(MacEntry, AP_STA_ENTRY, MacHashEntry);
    BOOLEAN * connectionInProcess = (BOOLEAN *)CallbackCtxt;

    UNREFERENCED_PARAMETER(Table);
    
    if (*connectionInProcess)
    {
        return FALSE;
    }
    else if (ApGetStaPortState(staEntry) != STA_PORT_STATE_OPEN)
    {
        //
        // Presence of the station entry indicates authentication started.
        // The connection completes when the station port state is set to open.
        //
        *connectionInProcess = TRUE;
        
        //
        // TODO: timeout for port state update?
        //

        return FALSE;
    }

    return TRUE;
}

/**
 * Handle scan request
 * Ignore non-forced scan request if a station is connecting to AP
 */
NDIS_STATUS
AmScanRequest(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PVOID ScanRequestId,
    _In_ PDOT11_SCAN_REQUEST_V2 ScanRequest,
    _In_ ULONG ScanRequestBufferLength
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_INVALID_STATE;
    MP_RW_LOCK_STATE lockState;
    BOOLEAN connectionInProcess = FALSE;
    PDOT11_SCAN_REQUEST_V2 localScanRequest = NULL;

    //
    // Acquire read lock on the Mac table
    //
    MP_ACQUIRE_READ_LOCK(&AssocMgr->MacHashTableLock, &lockState);

    // 
    // Enum station entries to check whether a connection is in process
    //
    EnumMacEntry(
        &AssocMgr->MacHashTable, 
        AmPortStateStaEntryCallback,
        &connectionInProcess
        );

    //
    // Release the read lock
    //
    MP_RELEASE_READ_LOCK(&AssocMgr->MacHashTableLock, &lockState);

    do
    {
        if (ApGetScanState(AssocMgr))
        {
            MpTrace(COMP_SCAN, DBG_SERIOUS, ("Port(%u): External scan already in progress. Ignoring new request.",
                                    AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            ndisStatus = NDIS_STATUS_DOT11_MEDIA_IN_USE;
            break;
        }
        
        if (connectionInProcess && ScanRequest->dot11ScanType != dot11_scan_type_forced)
        {
            MpTrace(COMP_OID, DBG_SERIOUS,  ("Port(%u): Non-forced scan is ignored as a connection is in progress on this adapter.",
                                    AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            ndisStatus = NDIS_STATUS_DOT11_MEDIA_IN_USE;
            break;
        }

        //
        // Make a local copy of the scan request
        //
        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(AssocMgr->ApPort), 
            &localScanRequest, 
            ScanRequestBufferLength, 
            EXTAP_MEMORY_TAG
            );
        
        if (NULL == localScanRequest)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for scan request.",
                                    AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                    ScanRequestBufferLength));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        
        //
        // Set scan state
        //
        if (ApSetScanState(AssocMgr, TRUE))
        {
            //
            // Somebody sets the scan state already
            // This shouldn't happen
            //
            MPASSERT(FALSE);
            break;
        }

        //
        // Save scan request
        //
        RtlCopyMemory(
            localScanRequest,
            ScanRequest,
            ScanRequestBufferLength
            );

        //
        // TODO: use another lock?
        //
        MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
        
        if (AssocMgr->ScanRequest.Dot11ScanRequest)
        {
            // Shouldn't happen
            MPASSERT(FALSE);
            MP_FREE_MEMORY(AssocMgr->ScanRequest.Dot11ScanRequest);
        }

        NdisZeroMemory(&AssocMgr->ScanRequest, sizeof(MP_SCAN_REQUEST));
        
        AssocMgr->ScanRequest.Dot11ScanRequest = localScanRequest;
        AssocMgr->ScanRequest.ScanRequestBufferLength = ScanRequestBufferLength;
        AssocMgr->ScanRequestId = ScanRequestId;

        MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);

        ndisStatus = Mp11Scan(
            AP_GET_ADAPTER(AssocMgr->ApPort),
            AP_GET_MP_PORT(AssocMgr->ApPort),
            &AssocMgr->ScanRequest,
            ApScanComplete
            );

        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            //
            // Don't free the local scan request
            // It will be freed when scan completes.
            //
            localScanRequest = NULL;
        }
        else
        {
            //
            // TODO: use another lock?
            //
            MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
            
            MPASSERT(AssocMgr->ScanRequest.Dot11ScanRequest == localScanRequest);
            
            AssocMgr->ScanRequest.Dot11ScanRequest = NULL;
            
            MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);

            //
            // Reset scan state
            //
            if (!ApSetScanState(AssocMgr, FALSE))
            {
                //
                // This shouldn't happen
                //
                MPASSERT(FALSE);
            }
        }
    } while (FALSE);

    if (localScanRequest != NULL)
    {
        MP_FREE_MEMORY(localScanRequest);
    }
    
    return ndisStatus;
}

NDIS_STATUS
AmDisassociatePeerRequest(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_MAC_ADDRESS PeerMacAddr,
    _In_ USHORT Reason
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    LIST_ENTRY head;                // list of stations to disassociate
    LIST_ENTRY * entry;
    PMAC_HASH_ENTRY macEntry;
    PAP_STA_ENTRY staEntry;
    MP_RW_LOCK_STATE lockState;
    PUCHAR pucMacAddr = (PUCHAR) PeerMacAddr;

    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): disassociate STA %02X-%02X-%02X-%02X-%02X-%02X. Reason = 0x%08x.",
                AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                pucMacAddr[0],
                pucMacAddr[1],
                pucMacAddr[2],
                pucMacAddr[3],
                pucMacAddr[4],
                pucMacAddr[5],
                Reason
                ));

    //
    // Initialize the list head
    //
    InitializeListHead(&head);

    //
    // Acquire write lock on the Mac table
    //
    MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);

    if (DOT11_IS_BROADCAST(PeerMacAddr))
    {
        //
        // Remove all stations
        //
        EnumMacEntry(
            &AssocMgr->MacHashTable, 
            AmRemoveStaEntryCallback,
            &head
            );
    }
    else
    {
        // 
        // Remove a specific station
        //
        macEntry = RemoveFromMacHashTable(
                    &AssocMgr->MacHashTable, 
                    PeerMacAddr
                    );

        if (macEntry != NULL)
        {
            //
            // Insert it to the list of stations to disassociate
            //
            InsertTailList(&head, &macEntry->Linkage);
        }
    }

    //
    // Release the write lock
    //
    MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
    
    //
    // Disassociate all stations
    //
    while(!IsListEmpty(&head))
    {
        entry = RemoveHeadList(&head);
        macEntry = CONTAINING_RECORD(entry, MAC_HASH_ENTRY, Linkage);
        staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);
    
        AmDeauthenticateSta(
            AssocMgr, 
            staEntry, 
            DOT11_DISASSOC_REASON_OS, 
            TRUE,                                       // Send deauth frame 
            Reason
            );
    }

    return ndisStatus;
}

/*
 * This function is called inside a direct call.
 * The caller must ensure the AP port stays valid.
 * The function must handle potential concurrent accesses
 * to shared resources.
 */
NDIS_STATUS
AmAssociationDecision(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_INCOMING_ASSOC_DECISION AssocDecision
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PMAC_HASH_ENTRY macEntry = NULL;
    PAP_STA_ENTRY staEntry = NULL;

    MP_RW_LOCK_STATE lockState;
    

    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Get association decision for STA %02X-%02X-%02X-%02X-%02X-%02X. Accept = %!BOOLEAN!, Reason = 0x%08x.",
                AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                AssocDecision->PeerMacAddr[0],
                AssocDecision->PeerMacAddr[1],
                AssocDecision->PeerMacAddr[2],
                AssocDecision->PeerMacAddr[3],
                AssocDecision->PeerMacAddr[4],
                AssocDecision->PeerMacAddr[5],
                AssocDecision->bAccept,
                AssocDecision->usReasonCode
                ));
    
    //
    // acquire a write lock on Mac table because we may need to update it
    //
    MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
    
    //
    // Lookup the station entry from Mac table
    //
    macEntry = LookupMacHashTable(
                &AssocMgr->MacHashTable, 
                &AssocDecision->PeerMacAddr
                );

    if (NULL == macEntry)
    {
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): STA is not in the peer table.",
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
        
        ndisStatus = NDIS_STATUS_INVALID_DATA;
    }
    else
    {
        staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);

        //
        // Cache the decision.
        // This call is made by OS in the same thread of NDIS_STATUS_DOT11_INCOMING_ASSOC_REQUEST_RECEIVED
        // indication.
        // We have to process the decision after NDIS_STATUS_DOT11_INCOMING_ASSOC_REQUEST_RECEIVED indication returns,
        // so that we can send NDIS_STATUS_DOT11_INCOMING_ASSOC_COMPLETION indication.
        // If we process the decision here and send NDIS_STATUS_DOT11_INCOMING_ASSOC_COMPLETION indication, 
        // the nested indications will result in a deadlock.
        //
        staEntry->AcceptAssoc = AssocDecision->bAccept;
        staEntry->Reason = AssocDecision->usReasonCode;

        // BUGBUG - handle OS specified IEs in assoc response after Windows 7
        MPASSERT( 0 == AssocDecision->uAssocResponseIEsLength );
    }
    
    MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);

    return ndisStatus;
}

/**
 * Process the association decision made by OS.
 */
NDIS_STATUS
AmProcessAssociationDecision(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ BOOLEAN reAssociation,
    _In_reads_bytes_(AssocReqFrameSize) PDOT11_MGMT_HEADER AssocReqFrame,
    _In_ USHORT AssocReqFrameSize,
    _In_ PDOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS AssocCompletePara,
    _In_ ULONG AssocCompleteParaSize
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR assocRespFrame = NULL;                   // association response
    USHORT assocRespFrameSize = 0;
    BOOLEAN sendResponse = FALSE;
    PMAC_HASH_ENTRY macEntry = NULL;
    PAP_STA_ENTRY staEntry = NULL;
    BOOLEAN releaseMacTableLock = FALSE;
    MP_RW_LOCK_STATE lockState;
    // NDIS indication
    BOOLEAN indicateAssocComplete = FALSE;
    ULONG errorStatus = 0;                          // success

    MPASSERT(AssocReqFrame != NULL);
    MPASSERT(AssocCompletePara != NULL);
    
    do
    {
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Process association decision for STA %02X-%02X-%02X-%02X-%02X-%02X. Reassociation = %!BOOLEAN!.",
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                    AssocReqFrame->SA[0],
                    AssocReqFrame->SA[1],
                    AssocReqFrame->SA[2],
                    AssocReqFrame->SA[3],
                    AssocReqFrame->SA[4],
                    AssocReqFrame->SA[5],
                    reAssociation
                    ));
        
        //
        // acquire a write lock on Mac table because we may need to update it
        //
        MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
        releaseMacTableLock = TRUE;
        
        //
        // Lookup the station entry from Mac table
        //
        macEntry = LookupMacHashTable(
                    &AssocMgr->MacHashTable, 
                    &AssocReqFrame->SA
                    );

        if (NULL == macEntry)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);

        MPASSERT(dot11_assoc_state_auth_unassoc == staEntry->AssocState);
        if (staEntry->AssocState != dot11_assoc_state_auth_unassoc)
        {
            ndisStatus = NDIS_STATUS_INVALID_STATE;
            break;
        }

        indicateAssocComplete = TRUE;

        // 
        // Create association response frame
        //
        if (staEntry->AcceptAssoc)
        {
            ndisStatus = AmCreateAssocRespFrame(
                            AssocMgr, 
                            staEntry, 
                            reAssociation,
                            AssocReqFrame, 
                            AssocReqFrameSize, 
                            DOT11_FRAME_STATUS_SUCCESSFUL, 
                            &assocRespFrame, 
                            &assocRespFrameSize
                            );
        }
        else
        {
            ndisStatus = AmCreateAssocRespFrame(
                            AssocMgr, 
                            staEntry, 
                            reAssociation,
                            AssocReqFrame, 
                            AssocReqFrameSize, 
                            staEntry->Reason, 
                            &assocRespFrame, 
                            &assocRespFrameSize
                            );
        }

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to create association response frame. Status = 0x%08x.", 
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        ndisStatus));

            errorStatus = ndisStatus;
            break;
        }

        sendResponse = TRUE;
        
        //
        // update station entry
        //
        if (staEntry->AcceptAssoc)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): STA is associated.",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)
                        ));

            staEntry->AssocState = dot11_assoc_state_auth_assoc;

            //
            // Reset statistics
            //
            NdisZeroMemory(&staEntry->Statistics, sizeof(DOT11_PEER_STATISTICS));

            // 
            // Association up time
            //
            NdisGetCurrentSystemTime(&staEntry->AssocUpTime);

            //
            // Start inactive timer
            //
            AmStartStaInactiveTimer(AssocMgr);
        }
        else
        {
            // Otherwise, leave it at the unassociated state
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): STA is not associated.",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)
                        ));
        }
    } while (FALSE);

    
    //
    // Prepare association complete indication
    //

    // 
    // Association complete indication
    //
    if (indicateAssocComplete)
    {
        if (!staEntry->AcceptAssoc)
        {
            //
            // OS rejects the association.
            // Set the status code.
            //
            errorStatus = staEntry->Reason;
        }
        
        // 
        // Can be a success or failure.
        // If it is failure, it must be an error from OS
        //
        if (errorStatus != 0)
        {
            AmPrepareAssocCompletePara(
                AssocMgr, 
                NULL,                   // no STA entry is required for a failure
                &AssocReqFrame->SA, 
                errorStatus, 
                DOT11_ASSOC_ERROR_SOURCE_OS,
                reAssociation,          // reassociation
                NULL,                   // no association request frame
                0,
                NULL,                   // no association response frame
                0,
                AssocCompletePara,
                &AssocCompleteParaSize
                );
        }
        else
        {
            //
            // Mac headers of the request and response frames
            // are not needed.
            //
            AmPrepareAssocCompletePara(
                AssocMgr, 
                staEntry,               
                &AssocReqFrame->SA, 
                errorStatus, 
                DOT11_ASSOC_ERROR_SOURCE_OS,
                reAssociation,          // reassociation
                Add2Ptr(AssocReqFrame, DOT11_MGMT_HEADER_SIZE),
                AssocReqFrameSize - DOT11_MGMT_HEADER_SIZE,
                Add2Ptr(assocRespFrame, DOT11_MGMT_HEADER_SIZE),
                assocRespFrameSize - DOT11_MGMT_HEADER_SIZE,
                AssocCompletePara,
                &AssocCompleteParaSize
                );
        }
    }

    
    // 
    // release locks
    //
    if (releaseMacTableLock)
    {
        MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
    }

    //
    // Send management frames
    //
    if (sendResponse)
    {
        //
        // Send association response
        //
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Send association response frame.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

        ndisStatus = ApSendMgmtPacket(
                        AssocMgr->ApPort, 
                        assocRespFrame, 
                        assocRespFrameSize
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // TODO: do we disassociate?
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to send association response frame. Status = 0x%08x.", 
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        ndisStatus));
        }

        MP_FREE_MEMORY(assocRespFrame);
        assocRespFrame = NULL;
    }

    //
    // Send NDIS indications
    //
    if (indicateAssocComplete)
    {
        ApIndicateDot11Status(
            AssocMgr->ApPort, 
            NDIS_STATUS_DOT11_INCOMING_ASSOC_COMPLETION, 
            NULL,                   // no request ID
            AssocCompletePara, 
            AssocCompleteParaSize
            );
    }

    return ndisStatus;
}


/**
 * Internal functions for station management
 */

/**
 * Processing station authentication frame
 * Receving authentication for a station that is already associated/authenticated
 * will put it in unauthenticated/unassociated state.
 */
VOID 
AmProcessStaAuthentication(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(PacketSize) PDOT11_MGMT_HEADER MgmtPacket,
    _In_ USHORT PacketSize
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC vnic = AP_GET_VNIC(AssocMgr->ApPort);
    PDOT11_AUTH_FRAME authFrame = NULL;
    PUCHAR authRespFrame = NULL;                // auth response
    USHORT authRespFrameSize = 0;
    USHORT statusCode = DOT11_FRAME_STATUS_SUCCESSFUL;
    BOOLEAN sendResponse = FALSE;
    PMAC_HASH_ENTRY macEntry = NULL;
    PAP_STA_ENTRY staEntry = NULL;
    BOOLEAN releaseMacTableLock = FALSE;
    MP_RW_LOCK_STATE lockState = {0};
    /** NDIS indications */
    DOT11_DISASSOCIATION_PARAMETERS disassocPara;
    BOOLEAN indicateDisassoc = FALSE;
    DOT11_INCOMING_ASSOC_STARTED_PARAMETERS assocStartPara = {0};
    BOOLEAN indicateAssocStart = FALSE;
    PDOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS assocCompletePara = NULL;
    ULONG assocCompleteParaSize = 0;
    BOOLEAN indicateAssocComplete = FALSE;
    UCHAR errorSource = DOT11_ASSOC_ERROR_SOURCE_OS;    // error source for association complete indication
    ULONG errorStatus = 0;
    
    // only handle authentication
    MPASSERT(DOT11_FRAME_TYPE_MANAGEMENT == MgmtPacket->FrameControl.Type);
    MPASSERT(DOT11_MGMT_SUBTYPE_AUTHENTICATION == MgmtPacket->FrameControl.Subtype);
    MPASSERT(PacketSize >= DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_AUTH_FRAME));
    
    do
    {
        //
        // Check BSSID and DA
        //
        if (!MP_COMPARE_MAC_ADDRESS(MgmtPacket->BSSID, VNic11QueryCurrentBSSID(vnic)) ||
            !MP_COMPARE_MAC_ADDRESS(MgmtPacket->DA, VNic11QueryMACAddress(vnic)))
        {
            break;
        }

        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Process authentication request for STA %02X-%02X-%02X-%02X-%02X-%02X.",
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                    MgmtPacket->SA[0],
                    MgmtPacket->SA[1],
                    MgmtPacket->SA[2],
                    MgmtPacket->SA[3],
                    MgmtPacket->SA[4],
                    MgmtPacket->SA[5]
                    ));
        
        //
        // Acquire a write lock on Mac table because we may need to update it
        //
        MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
        
        //
        // Remove the station entry from Mac table if it exists
        //
        macEntry = RemoveFromMacHashTable(
                    &AssocMgr->MacHashTable, 
                    &MgmtPacket->SA
                    );

        MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
        
        if (macEntry != NULL)
        {
            staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);

            //
            // We need to deauthenticate the station
            //
            AmDeauthenticateSta(
                AssocMgr, 
                staEntry, 
                DOT11_DISASSOC_REASON_OS,           // TODO: a better reason code
                FALSE,                              // Don't need to send deauth frame
                0
                );

            //
            // Station entry is destroyed after the ref count reaches zero
            //
            staEntry = NULL;
        }

        //
        // Now there is no entry for the station.
        // Let's start from the beginning.
        //

        //
        // Acquire a write lock on Mac table because we may need to update it
        //
        MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
        releaseMacTableLock = TRUE;
        
        //
        // Get Auth Frame
        //
        authFrame = (PDOT11_AUTH_FRAME)Add2Ptr(MgmtPacket, DOT11_MGMT_HEADER_SIZE);

        // 
        // We shall send response even if authentication fails
        //
        sendResponse = TRUE;

        //
        // Check the associated/associating station limit
        //
        if (GetMacHashTableEntryCount(&AssocMgr->MacHashTable) >= AP_STA_MAX_ENTRIES_DEFAULT)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Deny request because too many stations are associated/associating.",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

            statusCode = DOT11_FRAME_STATUS_ASSOC_DENIED_AP_BUSY;

            // indicate association start and complete
            indicateAssocStart = indicateAssocComplete = TRUE;
            errorSource = DOT11_ASSOC_ERROR_SOURCE_OS;
            errorStatus = (ULONG)NDIS_STATUS_RESOURCES;                 // TODO: a better status code?
            break;
        }

        //
        // Check authentication algorithm, we only accept open system authentication
        //
        if (authFrame->usAlgorithmNumber != DOT11_AUTH_OPEN_SYSTEM)
        {
            statusCode = DOT11_FRAME_STATUS_UNSUPPORTED_AUTH_ALGO; 

            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Invalid authentication algorithm %u.",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                        authFrame->usAlgorithmNumber));

            //
            // Indicate association start and complete
            //
            indicateAssocStart = indicateAssocComplete = TRUE;
            errorSource = DOT11_ASSOC_ERROR_SOURCE_REMOTE;
            errorStatus = statusCode;
            break;
        }

        //
        // Check transaction sequence number
        //
        if (authFrame->usXid != 1)
        {
            statusCode = DOT11_FRAME_STATUS_INVALID_AUTH_XID;

            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Invalid authentication XID %u.",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                        authFrame->usXid));

            // 
            // Indicate association start and complete
            //
            indicateAssocStart = indicateAssocComplete = TRUE;
            errorSource = DOT11_ASSOC_ERROR_SOURCE_REMOTE;
            errorStatus = statusCode;
            break;
        }

        //
        // Open system authentication always succeeds
        //

        // 
        // Create a new station entry
        //
        if (AmAllocateStaEntry(
                AssocMgr, 
                &MgmtPacket->SA,
                &staEntry
                ) != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Couldn't add sta entry.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

            statusCode = DOT11_FRAME_STATUS_FAILURE;                // TODO: a better status code?

            // 
            // Indicate association start and complete
            //
            indicateAssocStart = indicateAssocComplete = TRUE;
            errorSource = DOT11_ASSOC_ERROR_SOURCE_OS;
            errorStatus = (ULONG)NDIS_STATUS_RESOURCES;             // TODO: Is it the right way?
            break;
        }

        //
        // Authentication succeeds.
        // Send association start indication after receiving association request or timing out.
        //
        staEntry->AssocState = dot11_assoc_state_auth_unassoc;
        
        //
        // Add the station entry to the Mac table
        //
        InsertMacHashTable(&AssocMgr->MacHashTable, &staEntry->MacHashEntry);

        //
        // Start association timer
        //
        AmStartStaAssocTimer(staEntry);
        
    } while (FALSE);

    if (sendResponse)
    {
        //
        // Create auth response frame, can be a success or failure
        //
        AmCreateAuthRespFrame(
            AssocMgr, 
            staEntry, 
            MgmtPacket, 
            PacketSize, 
            statusCode, 
            &authRespFrame, 
            &authRespFrameSize
            );

        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Create authentication response. StatusCode = 0x%08x.", 
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort), statusCode));
    }
    else
    {
        // 
        // Do nothing if cannot create auth response frame.
        // Let the timeout precedure clean up the stale entry.
        //
    }

    //
    // Prepare association start and complete indications
    //
    if (indicateAssocStart)
    {
        // 
        // Must be a failure case and complete shall be indicated.
        //
        MPASSERT(indicateAssocComplete);

        AmPrepareAssocStartPara(
            &MgmtPacket->SA, 
            &assocStartPara
            );
    }

    if (indicateAssocComplete)
    {
        // 
        // Allocate association complete parameters
        //
        if (AmAllocAssocCompletePara(
                AssocMgr, 
                0,              // no association request frame 
                0,              // no association response frame
                &assocCompletePara,
                &assocCompleteParaSize
                ) != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to allocate association completion parameters.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            
            // 
            // Cannot create association complete parameter. Don't send any indication.
            //
            indicateAssocStart = indicateAssocComplete = FALSE;
        }
        else
        {
            // 
            // Fill in information
            //
            AmPrepareAssocCompletePara(
                AssocMgr, 
                NULL,               // no STA entry is required
                &MgmtPacket->SA, 
                errorStatus, 
                errorSource, 
                FALSE,              // not reassociation
                NULL,               // no association request frame
                0,
                NULL,
                0,                  // no association response frame
                assocCompletePara,
                &assocCompleteParaSize
                );
        }
    }
    
    // 
    // Release locks
    //
    if (releaseMacTableLock)
    {
        MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
    }

    //
    // Send management frames
    //
    if (authRespFrame)
    {
        //
        // Send authentication response
        //
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Send authentication response frame.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

        ndisStatus = ApSendMgmtPacket(
                        AssocMgr->ApPort, 
                        authRespFrame, 
                        authRespFrameSize
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to send authentication response frame. Status = 0x%08x.", 
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        ndisStatus));
        }

        MP_FREE_MEMORY(authRespFrame);
        authRespFrame = NULL;
    }

    //
    // Send NDIS indications
    //
    if (indicateDisassoc)
    {
        ApIndicateDot11Status(
            AssocMgr->ApPort, 
            NDIS_STATUS_DOT11_DISASSOCIATION, 
            NULL,                   // no request ID
            &disassocPara, 
            sizeof(disassocPara)
            );
    }

    if (indicateAssocStart)
    {
        ApIndicateDot11Status(
            AssocMgr->ApPort, 
            NDIS_STATUS_DOT11_INCOMING_ASSOC_STARTED, 
            NULL,                   // no request ID
            &assocStartPara, 
            sizeof(assocStartPara)
            );
    }

    if (indicateAssocComplete)
    {
        ApIndicateDot11Status(
            AssocMgr->ApPort, 
            NDIS_STATUS_DOT11_INCOMING_ASSOC_COMPLETION, 
            NULL,                   // no request ID
            assocCompletePara, 
            assocCompleteParaSize
            );
    }

    //
    // Free indications
    //
    if (assocCompletePara)
    {
        MP_FREE_MEMORY(assocCompletePara);
    }
    
}

/**
 * Processing station deauthentication frame
 */
VOID 
AmProcessStaDeauthentication(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(PacketSize) PDOT11_MGMT_HEADER MgmtPacket,
    _In_ _In_range_(DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_DEAUTH_FRAME), USHORT_MAX)  USHORT PacketSize
    )
{
    PVNIC vnic = AP_GET_VNIC(AssocMgr->ApPort);
    PDOT11_DEAUTH_FRAME deauthFrame;
    ULONG reasonCode;
    PMAC_HASH_ENTRY macEntry = NULL;
    PAP_STA_ENTRY staEntry = NULL;
    MP_RW_LOCK_STATE lockState;

    UNREFERENCED_PARAMETER(PacketSize);
    
    // only handle deauth
    MPASSERT(DOT11_FRAME_TYPE_MANAGEMENT == MgmtPacket->FrameControl.Type);
    MPASSERT(DOT11_MGMT_SUBTYPE_DEAUTHENTICATION == MgmtPacket->FrameControl.Subtype);
    MPASSERT(PacketSize >= DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_DEAUTH_FRAME));
    
    do
    {
        //
        // Check BSSID and DA
        //
        if (!MP_COMPARE_MAC_ADDRESS(MgmtPacket->BSSID, VNic11QueryCurrentBSSID(vnic)) ||
            !MP_COMPARE_MAC_ADDRESS(MgmtPacket->DA, VNic11QueryMACAddress(vnic)))
        {
            break;
        }

        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Process deauthentication request for STA %02X-%02X-%02X-%02X-%02X-%02X.",
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                    MgmtPacket->SA[0],
                    MgmtPacket->SA[1],
                    MgmtPacket->SA[2],
                    MgmtPacket->SA[3],
                    MgmtPacket->SA[4],
                    MgmtPacket->SA[5]
                    ));
        
        //
        // acquire a write lock on Mac table because we may need to update it
        //
        MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
        
        //
        // Remove the station entry from Mac table if it exists
        //
        macEntry = RemoveFromMacHashTable(
                    &AssocMgr->MacHashTable, 
                    &MgmtPacket->SA
                    );

        MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);

        if (NULL == macEntry)
        {
            // 
            // The station is not in our station table.
            // Ignore the deauth.
            //
            break;
        }
        
        //
        // Get Deauth Frame
        //
        deauthFrame = (PDOT11_DEAUTH_FRAME)Add2Ptr(MgmtPacket, DOT11_MGMT_HEADER_SIZE);

        // 
        // Get the reason 
        //
        reasonCode = DOT11_DISASSOC_REASON_PEER_DEAUTHENTICATED;
        reasonCode += deauthFrame->ReasonCode;
        
        staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);

        // 
        // We need to deauth the station
        //
        AmDeauthenticateSta(
            AssocMgr, 
            staEntry, 
            reasonCode,
            FALSE,                      // Don't send deauth frame
            0
            );
        
    } while (FALSE);

}

/**
 * Processing station association request frame
 */
VOID 
AmProcessStaAssociation(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ BOOLEAN reAssociation,
    _In_reads_bytes_(PacketSize) PDOT11_MGMT_HEADER MgmtPacket,
    _In_ _In_range_(DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_REASSOC_REQUEST_FRAME), USHORT_MAX) USHORT PacketSize
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC vnic = AP_GET_VNIC(AssocMgr->ApPort);
    PDOT11_ASSOC_REQUEST_FRAME assocRequestFrame = NULL;
    PUCHAR assocRespFrame = NULL;                   // association response
    USHORT assocRespFrameSize = 0;
    USHORT statusCode = DOT11_FRAME_STATUS_SUCCESSFUL;
    BOOLEAN sendResponse = FALSE;
    USHORT reasonCode = DOT11_MGMT_REASON_UPSPEC_REASON;
    BOOLEAN sendDeauth = FALSE;
    PMAC_HASH_ENTRY macEntry = NULL;
    PAP_STA_ENTRY staEntry = NULL;
    BOOLEAN releaseMacTableLock = FALSE;
    MP_RW_LOCK_STATE lockState;
    DOT11_SSID ssid = {0};
    DOT11_RATE_SET rateSet = {0};
    PUCHAR ieBlobPtr;
    USHORT ieBlobSize;
    PUCHAR rsnIePtr;
    UCHAR  rsnIeSize;
    RSN_IE_INFO rsnIeInfo;
    /** NDIS indications */
    DOT11_INCOMING_ASSOC_STARTED_PARAMETERS assocStartPara;
    BOOLEAN indicateAssocStart = FALSE;
    PDOT11_INCOMING_ASSOC_REQUEST_RECEIVED_PARAMETERS assocReqPara = NULL;
    ULONG assocReqParaSize = 0;
    BOOLEAN indicateAssocReqReceived = FALSE;
    PDOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS assocCompletePara = NULL;
    ULONG assocCompleteParaSize = 0;
    BOOLEAN indicateAssocComplete = FALSE;
    UCHAR errorSource = DOT11_ASSOC_ERROR_SOURCE_OS;    // error source for association complete indication
    ULONG errorStatus = 0;
    USHORT frameSubType = reAssociation?DOT11_MGMT_SUBTYPE_REASSOCIATION_REQUEST:DOT11_MGMT_SUBTYPE_ASSOCIATION_REQUEST;
    USHORT expectedFrameSize = reAssociation?sizeof(DOT11_REASSOC_REQUEST_FRAME):sizeof(DOT11_ASSOC_REQUEST_FRAME);

    // only handle association
    // or reassociation
    MPASSERT(DOT11_FRAME_TYPE_MANAGEMENT == MgmtPacket->FrameControl.Type);
    MPASSERT(frameSubType == MgmtPacket->FrameControl.Subtype);
    MPASSERT(PacketSize >= DOT11_MGMT_HEADER_SIZE + expectedFrameSize);
    
    UNREFERENCED_PARAMETER(frameSubType);

    do
    {
        //
        // Check BSSID and DA
        //
        if (!MP_COMPARE_MAC_ADDRESS(MgmtPacket->BSSID, VNic11QueryCurrentBSSID(vnic)) ||
            !MP_COMPARE_MAC_ADDRESS(MgmtPacket->DA, VNic11QueryMACAddress(vnic)))
        {
            break;
        }

        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Process association request for STA %02X-%02X-%02X-%02X-%02X-%02X. Reassociation = %!BOOLEAN!",
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                    MgmtPacket->SA[0],
                    MgmtPacket->SA[1],
                    MgmtPacket->SA[2],
                    MgmtPacket->SA[3],
                    MgmtPacket->SA[4],
                    MgmtPacket->SA[5],
                    reAssociation
                    ));
        
        //
        // Acquire a write lock on Mac table because we may need to update it
        //
        MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
        releaseMacTableLock = TRUE;
        
        //
        // Lookup the station entry from Mac table if it exists
        //
        macEntry = LookupMacHashTable(
                    &AssocMgr->MacHashTable, 
                    &MgmtPacket->SA
                    );

        if (macEntry != NULL)
        {
            staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);

            //
            // Stop association timer because we've received the association request.
            //
            AmStopStaAssocTimer(staEntry);
            
            //
            // Do we need to disassociate the station first?
            //
            if (ApGetStaAssocState(staEntry) == dot11_assoc_state_auth_assoc)
            {
                MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Need to disassociate the STA that is already in associated state.",
                            AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
                
                //
                // We need to disassociate the station.
                // The steps are the following:
                // 1. Reference the station entry so that it won't be destroyed.
                // 2. Release lock.
                // 3. Disassociation
                // 4. De-reference the station entry (the entry may be destroyted).
                // 5. Acquire lock.
                // 6. Search for station entry
                //
                
                ApRefSta(staEntry);

                MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
                
                AmDisassociateSta(
                    AssocMgr, 
                    staEntry, 
                    DOT11_DISASSOC_REASON_OS            // TODO: a better reason code
                    );

                ApDerefSta(staEntry);

                MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);

                //
                // Note:
                // The station entry can be removed because of timeout cleanup
                // or OS issueing Reset and/or Disassociation.
                // However, a new entry will NOT be added because received packet
                // processing is serialized, i.e. packets are processed one by one.
                //
                macEntry = LookupMacHashTable(
                            &AssocMgr->MacHashTable, 
                            &MgmtPacket->SA
                            );
                
                if (macEntry != NULL)
                {
                    staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);
                }
                else
                {
                    staEntry = NULL;
                }
            }
        }

        if (NULL == macEntry)
        {
            // 
            // The station is not authenticated yet, send a deauth.
            //
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Send deauthentication frame to STA not authenticated yet.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

            sendDeauth = TRUE;
            reasonCode = DOT11_MGMT_REASON_CLASS2_ERROR;

            // 
            // Don't indication association start because the station is not authenticated yet.
            //
            
            break;
        }
        
        if (ApGetStaAssocState(staEntry) != dot11_assoc_state_auth_unassoc)
        {
            MPASSERT(FALSE);

            // 
            // The station is not authenticated yet, send a deauth.
            //
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Send deauthentication frame to STA not authenticated yet.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

            sendDeauth = TRUE;
            reasonCode = DOT11_MGMT_REASON_CLASS2_ERROR;

            // 
            // Don't indication association start because the station is not authenticated yet.
            //
            
            break;
        }

        // 
        // We must prepare assoc completion indication at this time.
        // Allocate the maximum buffer for the completion.
        //
        ndisStatus = AmAllocAssocCompletePara(
                        AssocMgr, 
                        DOT11_MAX_MSDU_SIZE, 
                        DOT11_MAX_MSDU_SIZE, 
                        &assocCompletePara, 
                        &assocCompleteParaSize
                        );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Failed to allocate association completion parameter. Status = 0x%08x.",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                        ndisStatus));
            break;
        }
        
        // 
        // Need to send assoc start indication
        // 
        indicateAssocStart = TRUE;

        // 
        // Send association complete if associations is not accepted by miniport.
        //
        indicateAssocComplete = TRUE;           
        
        //
        // Get association Frame
        // Note that reassociation request frame contains association request frame.
        //
        assocRequestFrame = (PDOT11_ASSOC_REQUEST_FRAME)Add2Ptr(MgmtPacket, DOT11_MGMT_HEADER_SIZE);

        //
        // Get IE blob
        //
        ieBlobPtr = (PUCHAR)assocRequestFrame + expectedFrameSize;
        ieBlobSize = PacketSize - DOT11_MGMT_HEADER_SIZE - expectedFrameSize;
        
        //
        // Match SSID
        //
        ndisStatus = Dot11CopySSIDFromMemoryBlob(
                ieBlobPtr, 
                ieBlobSize, 
                &ssid
                );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            sendResponse = TRUE;
            statusCode = DOT11_FRAME_STATUS_INVALID_IE;        // TODO: a better status code?

            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Failed to get SSID from IE blob. Status = 0x%08x.",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                        ndisStatus
                        ));

            // 
            // For assoc complete indication
            //
            errorSource = DOT11_ASSOC_ERROR_SOURCE_OTHER;
            errorStatus = statusCode;
            break;
        }

        if (ssid.uSSIDLength != AssocMgr->Ssid.uSSIDLength ||
            memcmp(ssid.ucSSID, AssocMgr->Ssid.ucSSID, ssid.uSSIDLength) != 0
            )
        {
            sendResponse = TRUE;
            statusCode = DOT11_FRAME_STATUS_INVALID_IE;        // TODO: a better status code?

            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): SSID doesn't match.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

            // 
            // For assoc complete indication
            //
            errorSource = DOT11_ASSOC_ERROR_SOURCE_OTHER;
            errorStatus = statusCode;
            break;
        }
            
        //
        // TODO: match capability 
        //
        if (assocRequestFrame->Capability.ESS != 1 ||
            assocRequestFrame->Capability.IBSS != 0
            )
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Capability doesn't match. ESS = %u, IBSS = %u.", 
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                        assocRequestFrame->Capability.ESS,
                        assocRequestFrame->Capability.IBSS));

            sendResponse = TRUE;
            statusCode = DOT11_FRAME_STATUS_UNSUPPORTED_CAPABILITIES;

            // 
            // For assoc complete indication
            //
            errorSource = DOT11_ASSOC_ERROR_SOURCE_OTHER;
            errorStatus = statusCode;
            break;
        }
            
        //
        // Match rate set
        //
        ndisStatus = Dot11GetRateSetFromInfoEle(
                ieBlobPtr, 
                ieBlobSize, 
                FALSE,          // all rates
                &rateSet
                );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Failed to rate set from IE blob. Status = 0x%08x.",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                        ndisStatus));

            sendResponse = TRUE;
            statusCode = DOT11_FRAME_STATUS_INVALID_IE;

            // 
            // For assoc complete indication
            //
            errorSource = DOT11_ASSOC_ERROR_SOURCE_OTHER;
            errorStatus = statusCode;
            break;
        }

        //
        // Filter out the rates that are not supported by AP
        //
        AmFilterUnsupportedRates(
            AssocMgr, 
            &rateSet
            );

        if (0 == rateSet.uRateSetLength)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): No matching rate.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

            sendResponse = TRUE;
            statusCode = DOT11_FRAME_STATUS_ASSOC_DENIED_DATA_RATE_SET;

            // 
            // For assoc complete indication
            //
            errorSource = DOT11_ASSOC_ERROR_SOURCE_OTHER;
            errorStatus = statusCode;
            break;
        }

        //
        // set the default auth/cipher in staEntry
        //
        staEntry->AuthAlgo = DOT11_AUTH_ALGO_80211_OPEN;
        if (assocRequestFrame->Capability.Privacy != 0)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Choosing WEP.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

            // assuming WEP if Privacy bit is set
            staEntry->UnicastCipher = DOT11_CIPHER_ALGO_WEP;
            staEntry->MulticastCipher = DOT11_CIPHER_ALGO_WEP;
        }
        else
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Choosing None.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

            staEntry->UnicastCipher = DOT11_CIPHER_ALGO_NONE;
            staEntry->MulticastCipher = DOT11_CIPHER_ALGO_NONE;
        }

        //
        // Check the WPA2 Auth/Cipher in IE blob
        // TODO: check WPA IE and set auth/cipher in staEntry as well
        //
        if (AssocMgr->AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Choosing RSNA PSK.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

            if( Dot11GetInfoEle(
                    ieBlobPtr,
                    ieBlobSize,
                    DOT11_INFO_ELEMENT_ID_RSN,
                    &rsnIeSize,
                    &rsnIePtr
                    ) == NDIS_STATUS_SUCCESS)
            {

                MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): RSN IE present.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

                //
                // WPA2 IE is present, parse it and ensure it is supported
                //
                ndisStatus = Dot11ParseRNSIE(rsnIePtr, RSNA_OUI_PREFIX, rsnIeSize, &rsnIeInfo);

                if (ndisStatus != NDIS_STATUS_SUCCESS)
                {
                    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Failed to parse RSN IE. Status = 0x%08x.", 
                                AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                ndisStatus));

                    sendResponse = TRUE;
                    statusCode = DOT11_FRAME_STATUS_INVALID_IE;

                    // 
                    // For assoc complete indication
                    //
                    errorSource = DOT11_ASSOC_ERROR_SOURCE_OTHER;
                    errorStatus = statusCode;
                    break;
                }
                
                ndisStatus = AmMatchRsnInfo(
                        AssocMgr,
                        &rsnIeInfo,
                        &statusCode
                        );
                
                if (ndisStatus != NDIS_STATUS_SUCCESS)
                {
                    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): RSN IE mismatch. Status = 0x%08x.", 
                                AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                ndisStatus));

                    sendResponse = TRUE;

                    // 
                    // For assoc complete indication
                    //
                    errorSource = DOT11_ASSOC_ERROR_SOURCE_OTHER;
                    errorStatus = statusCode;
                    break;

                }

                //
                // copy auth/cipher from AssocMgr to staEntry as they match
                //
                staEntry->AuthAlgo = AssocMgr->AuthAlgorithm;
                staEntry->UnicastCipher = AssocMgr->UnicastCipherAlgorithm;
                staEntry->MulticastCipher = AssocMgr->MulticastCipherAlgorithm;
            }
            else
            {
                MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): No RSN IE present.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

                //
                // no WPA2 IE is present, only accept Open/None when WPS is enabled.
                // do not check Privacy bit, it may be 1 (from Vista SP1+WUR client).
                //
                if (!AssocMgr->EnableWps)
                {
                    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Cannot accept open in non-WCN case.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

                    sendResponse = TRUE;
                    statusCode = DOT11_FRAME_STATUS_UNSUPPORTED_AUTH_ALGO;

                    // 
                    // For assoc complete indication
                    //
                    errorSource = DOT11_ASSOC_ERROR_SOURCE_OTHER;
                    errorStatus = statusCode;
                    break;
                }
            }
        }

        // Allocate association request received parameters.
        //
        ndisStatus = AmAllocAssocReqPara(
                        AssocMgr, 
                        PacketSize - DOT11_MGMT_HEADER_SIZE, 
                        &assocReqPara, 
                        &assocReqParaSize
                        );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Failed to allocate association request parameters. Status = 0x%08x.",
                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                        ndisStatus));
            break;
        }

        indicateAssocReqReceived = TRUE;

        //
        // Initialize association decision.
        // It will be overwritten by the decision made by OS.
        // The initial values are used for the error case.
        //
        staEntry->AcceptAssoc = FALSE;
        staEntry->Reason = DOT11_FRAME_STATUS_FAILURE;

        // 
        // In this case, we will not send response and association completion indication.
        // The response is sent after receving OID_DOT11_INCOMING_ASSOCIATION_DECISION.
        //
        sendResponse = FALSE;
        indicateAssocComplete = FALSE;

        //
        // Miniport decides to accept the association.
        // Update station entry
        //

        // TODO: update station entry
        staEntry->Aid = AmAllocateAid(AssocMgr);
        staEntry->ListenInterval = assocRequestFrame->usListenInterval;
        staEntry->CapabilityInformation = assocRequestFrame->Capability;
        RtlCopyMemory(
            &staEntry->SupportedRateSet,
            &rateSet,
            sizeof(DOT11_RATE_SET)
            );
        
    } while (FALSE);

    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): First phase completed. StatusCode = 0x%08x.", AP_GET_PORT_NUMBER(AssocMgr->ApPort), statusCode));

    if (sendResponse)
    {
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Create association response with failure.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

        //
        // Create association response, it must be a failure
        //
        AmCreateAssocRespFrame(
            AssocMgr, 
            staEntry, 
            reAssociation,
            MgmtPacket, 
            PacketSize, 
            statusCode, 
            &assocRespFrame, 
            &assocRespFrameSize
            );
    }

    //
    // Prepare association indications
    //

    // 
    // Association start indication
    //
    if (indicateAssocStart)
    {
        AmPrepareAssocStartPara(
            &MgmtPacket->SA, 
            &assocStartPara
            );
    }
    
    // 
    // Association request received indication
    //
    if (indicateAssocReqReceived)
    {
        //
        // Mac header of the request frame is not needed.
        //
        AmPrepareAssocReqPara(
            AssocMgr, 
            &MgmtPacket->SA,
            reAssociation,
            Add2Ptr(MgmtPacket, DOT11_MGMT_HEADER_SIZE), 
            PacketSize - DOT11_MGMT_HEADER_SIZE, 
            assocReqPara,
            &assocReqParaSize
            );
    }
    
    // 
    // Association complete indication
    //
    if (indicateAssocComplete)
    {
        // 
        // Must be a failure
        //
        MPASSERT(errorStatus != 0);
        AmPrepareAssocCompletePara(
            AssocMgr, 
            NULL,                   // no STA entry is required for a failure
            &MgmtPacket->SA, 
            errorStatus, 
            errorSource,
            reAssociation,
            NULL,                   // no association request frame
            0,
            NULL,                   // no association response frame
            0,
            assocCompletePara,
            &assocCompleteParaSize
            );
    }

    
    // 
    // Release locks
    //
    if (releaseMacTableLock)
    {
        MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
    }

    //
    // Send management frames
    //
    if (assocRespFrame)
    {
        //
        // Send association response
        //
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Sending association response frame.", AP_GET_PORT_NUMBER(AssocMgr->ApPort)));

        ndisStatus = ApSendMgmtPacket(
                        AssocMgr->ApPort, 
                        assocRespFrame, 
                        assocRespFrameSize
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to send association response frame. Status = 0x%08x.", 
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        ndisStatus));
        }

        MP_FREE_MEMORY(assocRespFrame);
        assocRespFrame = NULL;
    }

    if (sendDeauth)
    {
        //
        // Send deauth frame
        //
        AmSendDeauthenticationFrame(
            AssocMgr, 
            &MgmtPacket->SA, 
            reasonCode
            );
    }

    //
    // Send NDIS indications
    //
    if (indicateAssocStart)
    {
        ApIndicateDot11Status(
            AssocMgr->ApPort, 
            NDIS_STATUS_DOT11_INCOMING_ASSOC_STARTED, 
            NULL,                   // no request ID
            &assocStartPara, 
            sizeof(assocStartPara)
            );
    }

    if (indicateAssocReqReceived)
    {
        MPASSERT(!indicateAssocComplete);
            
        ApIndicateDot11Status(
            AssocMgr->ApPort, 
            NDIS_STATUS_DOT11_INCOMING_ASSOC_REQUEST_RECEIVED, 
            NULL,                   // no request ID
            assocReqPara, 
            assocReqParaSize
            );

        //
        // OS will make the direct OID call OID_DOT11_INCOMING_ASSOCIATION_DECISION
        // in the status indication thread.
        // So after the status indication returns, the association decision shall be
        // cached in the station entry already. We need to process it now.
        //
        ndisStatus = AmProcessAssociationDecision(
                AssocMgr, 
                reAssociation,
                MgmtPacket,
                PacketSize,
                assocCompletePara,
                assocCompleteParaSize
                );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // 
            // Log error
            //
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to process association decision. Status = 0x%08x.", 
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        ndisStatus));
        }
    }

    if (indicateAssocComplete)
    {
        ApIndicateDot11Status(
            AssocMgr->ApPort, 
            NDIS_STATUS_DOT11_INCOMING_ASSOC_COMPLETION, 
            NULL,                   // no request ID
            assocCompletePara, 
            assocCompleteParaSize
            );
    }

    //
    // Free indications
    //
    if (assocReqPara)
    {
        MP_FREE_MEMORY(assocReqPara);
    }
    
    if (assocCompletePara)
    {
        MP_FREE_MEMORY(assocCompletePara);
    }

}

/**
 * Processing station disassociation frame
 */
VOID 
AmProcessStaDisassociation(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(PacketSize) PDOT11_MGMT_HEADER MgmtPacket,
    _In_ _In_range_(DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_DISASSOC_FRAME), USHORT_MAX) USHORT PacketSize
    )
{
    PVNIC vnic = AP_GET_VNIC(AssocMgr->ApPort);
    PDOT11_DISASSOC_FRAME disassocFrame;
    ULONG reasonCode;
    PMAC_HASH_ENTRY macEntry = NULL;
    PAP_STA_ENTRY staEntry = NULL;
    BOOLEAN releaseMacTableLock = FALSE;
    MP_RW_LOCK_STATE lockState;

    UNREFERENCED_PARAMETER(PacketSize);
    
    // only handle association
    MPASSERT(DOT11_FRAME_TYPE_MANAGEMENT == MgmtPacket->FrameControl.Type);
    MPASSERT(DOT11_MGMT_SUBTYPE_DISASSOCIATION == MgmtPacket->FrameControl.Subtype);
    MPASSERT(PacketSize >= DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_DISASSOC_FRAME));
    
    do
    {
        //
        // Check BSSID and DA
        //
        if (!MP_COMPARE_MAC_ADDRESS(MgmtPacket->BSSID, VNic11QueryCurrentBSSID(vnic)) ||
            !MP_COMPARE_MAC_ADDRESS(MgmtPacket->DA, VNic11QueryMACAddress(vnic)))
        {
            break;
        }

        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Process disassociation request for STA %02X-%02X-%02X-%02X-%02X-%02X.",
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                    MgmtPacket->SA[0],
                    MgmtPacket->SA[1],
                    MgmtPacket->SA[2],
                    MgmtPacket->SA[3],
                    MgmtPacket->SA[4],
                    MgmtPacket->SA[5]
                    ));
        
        //
        // acquire a write lock on Mac table because we may need to update it
        //
        MP_ACQUIRE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
        releaseMacTableLock = TRUE;
        
        //
        // Lookup the station entry from Mac table if it exists
        //
        macEntry = LookupMacHashTable(
                    &AssocMgr->MacHashTable, 
                    &MgmtPacket->SA
                    );

        if (NULL == macEntry)
        {
            // 
            // The station is not in our station table.
            // Ignore the disassociation.
            //
            break;
        }
        
        //
        // Get Disassoc Frame
        //
        disassocFrame = (PDOT11_DISASSOC_FRAME)Add2Ptr(MgmtPacket, DOT11_MGMT_HEADER_SIZE);

        // 
        // Get the reason 
        //
        reasonCode = DOT11_DISASSOC_REASON_PEER_DISASSOCIATED;
        reasonCode += disassocFrame->usReasonCode;
        
        staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);

        // 
        // We need to disassociate the station.
        // The steps are the following:
        // 1. Reference the station entry
        // 2. Release lock.
        // 3. Disassociate the station
        // 4. Dereference the station entry
        //

        ApRefSta(staEntry);
        
        MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
        releaseMacTableLock = FALSE;
        
        AmDisassociateSta(
            AssocMgr, 
            staEntry, 
            reasonCode
            );

        ApDerefSta(staEntry);
    } while (FALSE);

    // 
    // Release locks
    //
    if (releaseMacTableLock)
    {
        MP_RELEASE_WRITE_LOCK(&AssocMgr->MacHashTableLock, &lockState);
    }

}

/**
 * Association related management frames
 */

/** 
 * Fill common part of a management frame.
 * The frame is allocated by caller.
 */
VOID
AmFillMgmtFrameHeader(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PDOT11_MGMT_HEADER MgmtFrame,
    _In_ DOT11_MGMT_SUBTYPE MgmtSubtype,
    _In_ PDOT11_MAC_ADDRESS DestAddr
    )
{
    
    //
    // Fill the MAC header
    //
    MgmtFrame->FrameControl.Version = 0x0;
    MgmtFrame->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
    MgmtFrame->FrameControl.Subtype = MgmtSubtype;
    MgmtFrame->FrameControl.ToDS = 0x0;         // Default value for Mgmt frames
    MgmtFrame->FrameControl.FromDS = 0x0;       // Default value for Mgmt frames
    MgmtFrame->FrameControl.MoreFrag = 0x0;  
    MgmtFrame->FrameControl.Retry = 0x0;
    MgmtFrame->FrameControl.PwrMgt = 0x0;
    MgmtFrame->FrameControl.MoreData = 0x0;
    MgmtFrame->FrameControl.WEP = 0x0;          // no WEP
    MgmtFrame->FrameControl.Order = 0x0;        // no order
    
    RtlCopyMemory(
        MgmtFrame->DA, 
        *DestAddr,
        sizeof(DOT11_MAC_ADDRESS)  
        );
    
    RtlCopyMemory(
        MgmtFrame->SA, 
        VNic11QueryMACAddress(AP_GET_VNIC(AssocMgr->ApPort)),
        sizeof(DOT11_MAC_ADDRESS)
        );
    
    RtlCopyMemory(
        MgmtFrame->BSSID,
        AssocMgr->Bssid,
        sizeof(DOT11_MAC_ADDRESS)
        );
}

/** Create authentication response frame */
NDIS_STATUS
AmCreateAuthRespFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_opt_ PAP_STA_ENTRY StaEntry,
    _In_ PDOT11_MGMT_HEADER ReceivedMgmtPacket,
    _In_ USHORT ReceivedPacketSize,
    _In_ USHORT StatusCode,
    _Outptr_result_bytebuffer_(*AuthRespFrameSize) PUCHAR * AuthRespFrame,
    _Out_ PUSHORT AuthRespFrameSize
    )
{
    PDOT11_MGMT_HEADER mgmtFrame = NULL;
    PDOT11_AUTH_FRAME authFrame = NULL;
    PDOT11_AUTH_FRAME receivedAuthFrame;
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR packetBuffer = NULL;
    USHORT packetSize;

    do
    {
        if (!StaEntry)
        {
            ndisStatus = NDIS_STATUS_INVALID_PARAMETER;
            break;
        }
        
        if (ReceivedPacketSize < DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_AUTH_FRAME))
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }
        
        packetSize = DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_AUTH_FRAME);

        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(AssocMgr->ApPort), 
            &packetBuffer, 
            packetSize, 
            EXTAP_MEMORY_TAG
            );
        
        if (NULL == packetBuffer)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to allocate authentication frame (%u bytes).", 
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        packetSize));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        NdisZeroMemory(packetBuffer, packetSize);

        mgmtFrame = (PDOT11_MGMT_HEADER)packetBuffer;

        //
        // Fill the MAC header
        //
        AmFillMgmtFrameHeader(
            AssocMgr,
            mgmtFrame, 
            DOT11_MGMT_SUBTYPE_AUTHENTICATION, 
            &StaEntry->MacHashEntry.MacKey 
            );

        receivedAuthFrame = (PDOT11_AUTH_FRAME)Add2Ptr(ReceivedMgmtPacket, DOT11_MGMT_HEADER_SIZE);
        authFrame = (PDOT11_AUTH_FRAME)(packetBuffer + DOT11_MGMT_HEADER_SIZE);
        
        authFrame->usAlgorithmNumber = receivedAuthFrame->usAlgorithmNumber;
        authFrame->usXid = 2;
        authFrame->usStatusCode = StatusCode;

        //
        // Output parameters
        //
        *AuthRespFrame = packetBuffer;
        *AuthRespFrameSize = packetSize;
        
        packetBuffer = NULL;        // ensure the frame will not be freed
    } while (FALSE);
    
    if (packetBuffer)
    {
        MP_FREE_MEMORY(packetBuffer);
    }

    return ndisStatus;
}
    
/** Create association/reassociation response frame */
_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
AmCreateAssocRespFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PAP_STA_ENTRY StaEntry,
    _In_ BOOLEAN reAssociation,
    _In_ PDOT11_MGMT_HEADER ReceivedMgmtPacket,
    _In_ USHORT ReceivedPacketSize,
    _In_ USHORT StatusCode,
    _Outptr_result_bytebuffer_(*AssocRespFrameSize) PUCHAR * AssocRespFrame,
    _Out_ PUSHORT AssocRespFrameSize
    )
{
    PDOT11_MGMT_HEADER mgmtFrame = NULL;
    PDOT11_ASSOC_RESPONSE_FRAME assocResponseFrame = NULL;
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR packetBuffer = NULL;
    USHORT packetSize;
    PUCHAR currentIe = NULL;
    USHORT ieSize = AP11_MAX_IE_BLOB_SIZE;
    USHORT frameSize = reAssociation?sizeof(DOT11_REASSOC_REQUEST_FRAME):sizeof(DOT11_ASSOC_REQUEST_FRAME);
    USHORT respFrameSize = reAssociation?sizeof(DOT11_REASSOC_RESPONSE_FRAME):sizeof(DOT11_ASSOC_RESPONSE_FRAME);
    USHORT subType = reAssociation?DOT11_MGMT_SUBTYPE_REASSOCIATION_RESPONSE:DOT11_MGMT_SUBTYPE_ASSOCIATION_RESPONSE;

    UNREFERENCED_PARAMETER(ReceivedMgmtPacket);

    do
    {
        if (ReceivedPacketSize < DOT11_MGMT_HEADER_SIZE + frameSize)
        {
            ndisStatus = NDIS_STATUS_INVALID_DATA;
            break;
        }

        // Need to append IE
        packetSize = DOT11_MGMT_HEADER_SIZE + respFrameSize + ieSize;

        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(AssocMgr->ApPort), 
            &packetBuffer, 
            packetSize, 
            EXTAP_MEMORY_TAG
            );
        
        if (NULL == packetBuffer)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to allocate association response frame (%u bytes).", 
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        packetSize));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        NdisZeroMemory(packetBuffer, packetSize);

        mgmtFrame = (PDOT11_MGMT_HEADER)packetBuffer;

        //
        // Fill the MAC header
        //
        AmFillMgmtFrameHeader(
            AssocMgr, 
            mgmtFrame, 
            subType, 
            &StaEntry->MacHashEntry.MacKey
            );
        
        assocResponseFrame = (PDOT11_ASSOC_RESPONSE_FRAME)(packetBuffer + DOT11_MGMT_HEADER_SIZE);
        
        assocResponseFrame->Capability = StaEntry->CapabilityInformation;
        assocResponseFrame->usAID = StaEntry->Aid;
        assocResponseFrame->usStatusCode = StatusCode;

        //
        // Add IEs
        //
        currentIe = (PUCHAR)assocResponseFrame + sizeof(DOT11_ASSOC_RESPONSE_FRAME);
        
        // 
        // Add supported rates
        //
        ndisStatus = Dot11AttachElement(
                &currentIe,
                &ieSize,
                DOT11_INFO_ELEMENT_ID_SUPPORTED_RATES,
                (UCHAR)((StaEntry->SupportedRateSet.uRateSetLength > 8) ? 8 : StaEntry->SupportedRateSet.uRateSetLength),
                StaEntry->SupportedRateSet.ucRateSet
                );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to populate supported rates in association response.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
            break;
        }

        //
        // Add the extended rate set if needed
        //
        if (StaEntry->SupportedRateSet.uRateSetLength > (UCHAR)8) 
        {
            ndisStatus = Dot11AttachElement(
                &currentIe,
                &ieSize,
                DOT11_INFO_ELEMENT_ID_EXTD_SUPPORTED_RATES,
                (UCHAR)(StaEntry->SupportedRateSet.uRateSetLength - 8),
                (StaEntry->SupportedRateSet.ucRateSet + 8)
                );
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to add extended rates in association response.",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort)));
                break;
            }
        }

        //
        // TODO: add other IEs
        //
        
        //
        // Adjust packet length for IE space we did not use.  
        // We may have empty space here that we dont want send out in the packet. 
        //
        packetSize = packetSize - ieSize;

        //
        // Output parameters
        //
        *AssocRespFrame = packetBuffer;
        *AssocRespFrameSize = packetSize;
        
        packetBuffer = NULL;        // ensure the frame will not be freed
    } while (FALSE);
    
    if (packetBuffer)
    {
        MP_FREE_MEMORY(packetBuffer);
    }

    return ndisStatus;
}

/** Create disassociation frame */
NDIS_STATUS
AmCreateDisassocFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS DestAddr,
    _In_ USHORT ReasonCode,
    _Outptr_result_bytebuffer_(*DisassocFrameSize) PUCHAR * DisassocFrame,
    _Out_ PUSHORT DisassocFrameSize
    )
{
    PDOT11_MGMT_HEADER mgmtFrame = NULL;
    PDOT11_DISASSOC_FRAME disassocFrame = NULL;
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR packetBuffer = NULL;
    USHORT packetSize;

    do
    {
        packetSize = DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_DISASSOC_FRAME);

        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(AssocMgr->ApPort), 
            &packetBuffer, 
            packetSize, 
            EXTAP_MEMORY_TAG
            );
        
        if (NULL == packetBuffer)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to allocate disassociation frame (%u bytes).",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        packetSize));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        NdisZeroMemory(packetBuffer, packetSize);

        mgmtFrame = (PDOT11_MGMT_HEADER)packetBuffer;

        //
        // Fill the MAC header
        //
        AmFillMgmtFrameHeader(
            AssocMgr, 
            mgmtFrame, 
            DOT11_MGMT_SUBTYPE_DISASSOCIATION, 
            DestAddr
            );
        
        disassocFrame = (PDOT11_DISASSOC_FRAME)(packetBuffer + DOT11_MGMT_HEADER_SIZE);
        disassocFrame->usReasonCode = ReasonCode;

        //
        // Output parameters
        //
        *DisassocFrame = packetBuffer;
        *DisassocFrameSize = packetSize;
        
        packetBuffer = NULL;        // ensure the frame will not be freed
    } while (FALSE);
    
    if (packetBuffer)
    {
        MP_FREE_MEMORY(packetBuffer);
    }

    return ndisStatus;
}

/** Create deauthentication frame */
NDIS_STATUS
AmCreateDeauthFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS DestAddr,
    _In_ USHORT ReasonCode,
    _Outptr_result_bytebuffer_(*DeauthFrameSize) PUCHAR * DeauthFrame,
    _Out_ PUSHORT DeauthFrameSize
    )
{
    PDOT11_MGMT_HEADER mgmtFrame = NULL;
    PDOT11_DEAUTH_FRAME deauthFrame = NULL;
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR packetBuffer = NULL;
    USHORT packetSize;

    do
    {
        packetSize = DOT11_MGMT_HEADER_SIZE + sizeof(DOT11_DEAUTH_FRAME);

        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(AssocMgr->ApPort), 
            &packetBuffer, 
            packetSize, 
            EXTAP_MEMORY_TAG
            );
        
        if (NULL == packetBuffer)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to allocate deauthentication frame (%u bytes).",
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        packetSize));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        NdisZeroMemory(packetBuffer, packetSize);

        mgmtFrame = (PDOT11_MGMT_HEADER)packetBuffer;

        //
        // Fill the MAC header
        //
        AmFillMgmtFrameHeader(
            AssocMgr, 
            mgmtFrame, 
            DOT11_MGMT_SUBTYPE_DEAUTHENTICATION, 
            DestAddr
            );
        
        deauthFrame = (PDOT11_DEAUTH_FRAME)(packetBuffer + DOT11_MGMT_HEADER_SIZE);
        deauthFrame->ReasonCode = ReasonCode;

        //
        // Output parameters
        //
        *DeauthFrame = packetBuffer;
        *DeauthFrameSize = packetSize;
        
        packetBuffer = NULL;        // ensure the frame will not be freed
    } while (FALSE);
    
    if (packetBuffer)
    {
        MP_FREE_MEMORY(packetBuffer);
    }

    return ndisStatus;
}


/** 
 * Send disassociation frame. 
 * Shall not invoke it in a lock.
 */
NDIS_STATUS
AmSendDisassociationFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS DestAddr,
    _In_ USHORT ReasonCode
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR mgmtFrame = NULL;
    USHORT mgmtFrameSize;
    PUCHAR pucMacAddr = (PUCHAR) DestAddr;

    do
    {
        ndisStatus = AmCreateDisassocFrame(
                        AssocMgr, 
                        DestAddr, 
                        ReasonCode, 
                        &mgmtFrame, 
                        &mgmtFrameSize
                        );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
        
        //
        // Send the disassociation packet
        //
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Send disassociation frame to STA %02X-%02X-%02X-%02X-%02X-%02X. Reason = 0x%08x.",
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                    pucMacAddr[0],
                    pucMacAddr[1],
                    pucMacAddr[2],
                    pucMacAddr[3],
                    pucMacAddr[4],
                    pucMacAddr[5],
                    ReasonCode
                    ));

        ndisStatus = ApSendMgmtPacket(
                        AssocMgr->ApPort, 
                        mgmtFrame, 
                        mgmtFrameSize
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to send disassociation frame. Status = 0x%08x.", 
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        ndisStatus));
        }

    } while (FALSE);
    
    if (mgmtFrame)
    {
        MP_FREE_MEMORY(mgmtFrame);
    }

    return ndisStatus;
}

/** 
 * Send deauthentication frame.
 * Shall not invoke it in a lock.
 */
NDIS_STATUS
AmSendDeauthenticationFrame(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS DestAddr,
    _In_ USHORT ReasonCode
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR mgmtFrame = NULL;
    USHORT mgmtFrameSize;
    PUCHAR pucMacAddr = (PUCHAR) DestAddr;

    do
    {
        ndisStatus = AmCreateDeauthFrame(
                        AssocMgr, 
                        DestAddr, 
                        ReasonCode, 
                        &mgmtFrame, 
                        &mgmtFrameSize
                        );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Send the disassociation packet
        //
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Send deauthentication frame to STA %02X-%02X-%02X-%02X-%02X-%02X. Reason = 0x%08x.",
                    AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                    pucMacAddr[0],
                    pucMacAddr[1],
                    pucMacAddr[2],
                    pucMacAddr[3],
                    pucMacAddr[4],
                    pucMacAddr[5],
                    ReasonCode
                    ));
        
        ndisStatus = ApSendMgmtPacket(
                        AssocMgr->ApPort, 
                        mgmtFrame, 
                        mgmtFrameSize
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to send deauthentication frame. Status = 0x%08x.", 
                                        AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                        ndisStatus));
        }

    } while (FALSE);
    
    if (mgmtFrame)
    {
        MP_FREE_MEMORY(mgmtFrame);
    }

    return ndisStatus;
}


/** 
 * Disassociate a station. 
 * The caller must ensure the station entry is valid during this call.
 * The caller should not hold any lock.
 */
VOID
AmDisassociateSta(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PAP_STA_ENTRY StaEntry,
    _In_ ULONG Reason
    )
{
    DOT11_DISASSOCIATION_PARAMETERS disassocPara;

    UNREFERENCED_PARAMETER(AssocMgr);
    UNREFERENCED_PARAMETER(StaEntry);
    UNREFERENCED_PARAMETER(Reason);

    do
    {
        //
        // Stop association timer.
        // This a no-op if the timer is not started.
        //
        AmStopStaAssocTimer(StaEntry);
        
        //
        // Check the current state of the station.
        // The state can change because no lock is held.
        // Set the new state to disassociated if the current state is associated.
        //
        if (ApCheckAndSetStaAssocState(
                StaEntry,
                dot11_assoc_state_auth_unassoc,
                dot11_assoc_state_auth_assoc
                ) != dot11_assoc_state_auth_assoc)
        {
            // 
            // The state is not associated. Do nothing.
            //
            break;
        }

        //
        // The above state check ensures the station will not be
        // disassociated twice.
        //

        //
        // Stop inactive timer for this station
        //
        AmStopStaInactiveTimer(AssocMgr);
        
        //
        // TODO: clear cached packets
        // Packets may be cached in HW layer because of power save.
        // TODO: This cannot be called in a lock?
        //

        // Free AID
        if (StaEntry->Aid != AP_INVALID_AID)
        {
            AmFreeAid(AssocMgr, StaEntry->Aid);
            StaEntry->Aid = AP_INVALID_AID;
        }

        //
        // TODO: Send disassociation frame
        //
        if (DOT11_ASSOC_STATUS_DISASSOCIATED_BY_OS == Reason)
        {
            // TODO: send disassociation frame as appropriate
        }

        //
        // Prepare disassociation indication
        //
        AmPrepareDisassocPara(
            &StaEntry->MacHashEntry.MacKey, 
            Reason, 
            &disassocPara
            );
        
        //
        // Send NDIS indication
        //
        ApIndicateDot11Status(
            AssocMgr->ApPort, 
            NDIS_STATUS_DOT11_DISASSOCIATION, 
            NULL,                   // no request ID
            &disassocPara, 
            sizeof(disassocPara)
            );

        // 
        // Add trace
        //
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, 
                ("Port(%u): Station %02X-%02X-%02X-%02X-%02X-%02X disassociated due to reason 0x%08x.", 
                AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                StaEntry->MacHashEntry.MacKey[0], 
                StaEntry->MacHashEntry.MacKey[1], 
                StaEntry->MacHashEntry.MacKey[2], 
                StaEntry->MacHashEntry.MacKey[3], 
                StaEntry->MacHashEntry.MacKey[4], 
                StaEntry->MacHashEntry.MacKey[5], 
                Reason));
    } while (FALSE);
}

/** 
 * Deauthenicate a station. 
 * This shall be called only after a station 
 * is authenticated.
 * The station entry must be removed from the
 * Mac table already.
 * So this function will be invoked only once
 * for each station entry.
 */
VOID
AmDeauthenticateSta(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ PAP_STA_ENTRY StaEntry,
    _In_ ULONG Reason,
    _In_ BOOLEAN SendDeauthFrame,
    _In_ USHORT Dot11Reason 
    )
{
    //
    // Disassociate the station first.
    // This is a no-op if the station is not associated
    //
    AmDisassociateSta(
        AssocMgr, 
        StaEntry, 
        Reason
        );
    
    if (SendDeauthFrame)
    {
        //
        // Send deauth frame
        //
        AmSendDeauthenticationFrame(
            AssocMgr, 
            &StaEntry->MacHashEntry.MacKey, 
            Dot11Reason
            );
    }

    //
    // Set station state
    //
    ApSetStaAssocState(
        StaEntry, 
        dot11_assoc_state_unauth_unassoc
        );

    // 
    // Add trace
    //
    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, 
            ("Port(%u): Station %02X-%02X-%02X-%02X-%02X-%02X deauthenticated due to reason 0x%08x.", 
            AP_GET_PORT_NUMBER(AssocMgr->ApPort),
            StaEntry->MacHashEntry.MacKey[0], 
            StaEntry->MacHashEntry.MacKey[1], 
            StaEntry->MacHashEntry.MacKey[2], 
            StaEntry->MacHashEntry.MacKey[3], 
            StaEntry->MacHashEntry.MacKey[4], 
            StaEntry->MacHashEntry.MacKey[5], 
            Dot11Reason));
    
    // 
    // dereference the entry, may result in deleting the entry
    //
    ApDerefSta(StaEntry);

}

/** Supporting functions for NDIS indications */

/** Allocate association completion parameters */
NDIS_STATUS
AmAllocAssocCompletePara(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ USHORT AssocReqFrameSize,
    _In_ USHORT AssocRespFrameSize,
    _Outptr_ PDOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS * AssocCompletePara,
    _Out_ PULONG AssocCompleteParaSize
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredSize = 0;
    PAP_CONFIG Config;
    ULONG beaconFrameSize;
    
    Config = AP_GET_CONFIG(AssocMgr->ApPort);
    beaconFrameSize = Config->ApBeaconIESize + FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);

    requiredSize = sizeof(DOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS)    // data structure itself
                    + AssocReqFrameSize                                     // association request frame size, can be zero
                    + AssocRespFrameSize                                    // association response frame size, can be zero
                    + beaconFrameSize                                       // beacon frame size
                    + sizeof(ULONG)                                         // active PHY
                    ;

    MP_ALLOCATE_MEMORY(
        AP_GET_MP_HANDLE(AssocMgr->ApPort), 
        AssocCompletePara, 
        requiredSize, 
        EXTAP_MEMORY_TAG
        );

    if (NULL == *AssocCompletePara)
    {
        MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to allocate %u bytes for association complete parameters.", 
                                    AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                    requiredSize));
        ndisStatus = NDIS_STATUS_RESOURCES;
    }
    else
    {
        NdisZeroMemory(*AssocCompletePara, requiredSize);
        *AssocCompleteParaSize = requiredSize;
    }

    return ndisStatus;
}

/** Allocate  association request received parameters */
NDIS_STATUS
AmAllocAssocReqPara(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_ USHORT AssocReqFrameSize,
    _Outptr_ PDOT11_INCOMING_ASSOC_REQUEST_RECEIVED_PARAMETERS * AssocReqPara,
    _Out_ PULONG AssocReqParaSize
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG requiredSize = 0;
    
    requiredSize = sizeof(DOT11_INCOMING_ASSOC_REQUEST_RECEIVED_PARAMETERS) // data structure itself
                    + AssocReqFrameSize                                     // association request frame size, can be zero
                    ;

    MP_ALLOCATE_MEMORY(
        AP_GET_MP_HANDLE(AssocMgr->ApPort), 
        AssocReqPara, 
        requiredSize, 
        EXTAP_MEMORY_TAG
        );

    if (NULL == *AssocReqPara)
    {
        MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to allocate %u bytes for association request received parameters.", 
                                    AP_GET_PORT_NUMBER(AssocMgr->ApPort),
                                    requiredSize));
        ndisStatus = NDIS_STATUS_RESOURCES;
    }
    else
    {
        NdisZeroMemory(*AssocReqPara, requiredSize);
        *AssocReqParaSize = requiredSize;
    }

    return ndisStatus;
}


/**
 * Prepare information for association start indication.
 * The buffer is allocated by the caller.
 */
VOID
AmPrepareAssocStartPara(
    _In_ PDOT11_MAC_ADDRESS StaMacAddr,
    _Out_ PDOT11_INCOMING_ASSOC_STARTED_PARAMETERS AssocStartPara
    )
{
    // 
    // NDIS header
    //
    MP_ASSIGN_NDIS_OBJECT_HEADER(
        AssocStartPara->Header,
        NDIS_OBJECT_TYPE_DEFAULT,
        DOT11_INCOMING_ASSOC_STARTED_PARAMETERS_REVISION_1,
        sizeof(DOT11_INCOMING_ASSOC_STARTED_PARAMETERS)
        );
    
    // 
    // Copy Mac address
    //
    RtlCopyMemory(
        AssocStartPara->PeerMacAddr,
        *StaMacAddr,
        sizeof(DOT11_MAC_ADDRESS)
        );

}

/**
 * Prepare information for association request received indication.
 * The buffer is allocated by the caller. 
 * The caller must make sure the buffer is big enough to hold all data.
 */
VOID
AmPrepareAssocReqPara(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS StaMacAddr,
    _In_ BOOLEAN Reassociation,
    _In_reads_bytes_(RequestFrameSize) PUCHAR RequestFrame,
    _In_ USHORT RequestFrameSize,
    _Out_ PDOT11_INCOMING_ASSOC_REQUEST_RECEIVED_PARAMETERS AssocReqPara,
    _Inout_ PULONG AssocReqParaSize
    )
{
    ULONG requiredAssocReqParaSize = 0;

    UNREFERENCED_PARAMETER(AssocMgr);
    
    //
    // Calculate the size of the association request received parameters
    //
    requiredAssocReqParaSize = sizeof(DOT11_INCOMING_ASSOC_REQUEST_RECEIVED_PARAMETERS) + RequestFrameSize;

    MPASSERT(*AssocReqParaSize >= requiredAssocReqParaSize);
    
    // 
    // Clear everything
    //
    NdisZeroMemory(AssocReqPara, requiredAssocReqParaSize);

    *AssocReqParaSize = requiredAssocReqParaSize;

    //
    // Fill in information
    //

    // 
    // NDIS header
    //
    MP_ASSIGN_NDIS_OBJECT_HEADER(
        AssocReqPara->Header,
        NDIS_OBJECT_TYPE_DEFAULT,
        DOT11_INCOMING_ASSOC_REQUEST_RECEIVED_PARAMETERS_REVISION_1,
        sizeof(DOT11_INCOMING_ASSOC_REQUEST_RECEIVED_PARAMETERS)
        );

    // 
    // Station Mac address
    //
    RtlCopyMemory(
        AssocReqPara->PeerMacAddr,
        *StaMacAddr,
        sizeof(DOT11_MAC_ADDRESS)
        );

    // 
    // Reassociation?
    //
    AssocReqPara->bReAssocReq = Reassociation;

    // 
    // Request frame follows the data structure immediately
    // 
    AssocReqPara->uAssocReqSize = RequestFrameSize;
    AssocReqPara->uAssocReqOffset = sizeof(DOT11_INCOMING_ASSOC_REQUEST_RECEIVED_PARAMETERS);

    //
    // Copy association request
    //
    RtlCopyMemory(
        Add2Ptr(AssocReqPara, AssocReqPara->uAssocReqOffset),
        RequestFrame,
        RequestFrameSize
        );

}


/**
 * Prepare information for association complete indication.
 * The buffer is allocated by the caller.
 * The caller must make sure the buffer is big enough to hold all data.
 */
VOID
AmPrepareAssocCompletePara(
    _In_ PAP_ASSOC_MGR AssocMgr,
    _In_opt_ PAP_STA_ENTRY StaEntry,
    _In_reads_bytes_(DOT11_ADDRESS_SIZE) PDOT11_MAC_ADDRESS StaMacAddr,
    _In_ ULONG Status,
    _In_ UCHAR ErrorSource,
    _In_ BOOLEAN Reassociation,
    _In_reads_bytes_opt_(RequestFrameSize) PUCHAR RequestFrame,
    _In_ USHORT RequestFrameSize,
    _In_reads_bytes_opt_(ResponseFrameSize) PUCHAR ResponseFrame,
    _In_ USHORT ResponseFrameSize,
    _Out_ PDOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS AssocCompletePara,
    _Inout_ PULONG AssocCompleteParaSize
    )
{
    ULONG requiredAssocCompleteParaSize = 0;
    USHORT activePhyListSize;
    ULONG* phyId;
    PAP_CONFIG ApConfig;
    ULONG  beaconFrameSize;

    do
    {
        //
        // TODO: only support one active PHY?
        // 
        activePhyListSize = sizeof(ULONG);

        ApConfig = &AssocMgr->ApPort->Config;
        beaconFrameSize = ApConfig->ApBeaconIESize + FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);

        //
        // Calculate the size of the association completion parameters
        //
        requiredAssocCompleteParaSize = sizeof(DOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS) + 
                                RequestFrameSize + 
                                ResponseFrameSize +
                                activePhyListSize +
                                beaconFrameSize
                                ;

        MPASSERT(*AssocCompleteParaSize >= requiredAssocCompleteParaSize);
        
        // 
        // Store the pointer in station entry
        //
        *AssocCompleteParaSize = requiredAssocCompleteParaSize;

        // 
        // Clear everything
        //
        NdisZeroMemory(AssocCompletePara, requiredAssocCompleteParaSize);

        //
        // Fill in information
        //

        // 
        // NDIS header
        //
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            AssocCompletePara->Header,
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS_REVISION_1,
            sizeof(DOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS)
            );

        // 
        // Status
        //
        AssocCompletePara->uStatus = Status;

        // 
        // Error source
        //
        AssocCompletePara->ucErrorSource = ErrorSource;
        
        // 
        // Station Mac address
        //
        RtlCopyMemory(
            AssocCompletePara->PeerMacAddr,
            *StaMacAddr,
            sizeof(DOT11_MAC_ADDRESS)
            );

        // 
        // Reassociation?
        //
        AssocCompletePara->bReAssocReq = AssocCompletePara->bReAssocResp = Reassociation;

        // 
        // The following only applies to an successful association
        //
        if (Status != 0)
        {
            break;
        }
        // if the Status is 0 then these pointers must be valid
        if (StaEntry == NULL || RequestFrame == NULL || ResponseFrame == NULL)
        {
            MPASSERT(StaEntry != NULL);
            MPASSERT(RequestFrame != NULL);
            MPASSERT(ResponseFrame != NULL);
            return;
        }
    
        // 
        // Request frame follows the data structure immediately
        //
        MPASSERT(RequestFrameSize != 0);
        AssocCompletePara->uAssocReqSize = RequestFrameSize;
        AssocCompletePara->uAssocReqOffset = sizeof(DOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS);

        RtlCopyMemory(
            Add2Ptr(AssocCompletePara, AssocCompletePara->uAssocReqOffset),
            RequestFrame,
            RequestFrameSize
            );

        // 
        // Response frame follows request frame
        //
        MPASSERT(ResponseFrameSize != 0);
        AssocCompletePara->uAssocRespSize = ResponseFrameSize;
        AssocCompletePara->uAssocRespOffset = AssocCompletePara->uAssocReqOffset + RequestFrameSize;

        RtlCopyMemory(
            Add2Ptr(AssocCompletePara, AssocCompletePara->uAssocRespOffset),
            ResponseFrame,
            ResponseFrameSize
            );

        // 
        // Active PHY list follows response frame
        //
        AssocCompletePara->uActivePhyListSize = activePhyListSize;
        AssocCompletePara->uActivePhyListOffset = AssocCompletePara->uAssocRespOffset + ResponseFrameSize;

        phyId = (ULONG*)Add2Ptr(AssocCompletePara, AssocCompletePara->uActivePhyListOffset);
        (*phyId) = VNic11QueryOperatingPhyId(AP_GET_VNIC(AssocMgr->ApPort));

        //
        // Beacon frames
        //
        AssocCompletePara->uBeaconSize = beaconFrameSize;
        AssocCompletePara->uBeaconOffset = AssocCompletePara->uActivePhyListOffset + activePhyListSize;
           
        RtlCopyMemory(
            Add2Ptr(AssocCompletePara, AssocCompletePara->uBeaconOffset + FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements)),
            ApConfig->ApBeaconIEData,
            ApConfig->ApBeaconIESize
            );

        MPASSERT(StaEntry != NULL);
        AssocCompletePara->AuthAlgo = StaEntry->AuthAlgo;
        AssocCompletePara->UnicastCipher = StaEntry->UnicastCipher;
        AssocCompletePara->MulticastCipher = StaEntry->MulticastCipher;
        
    } while (FALSE);

}

/**
 * Prepare in information for disassociation indication.
 * The buffer is allocated by the caller.
 */
VOID
AmPrepareDisassocPara(
    _In_ PDOT11_MAC_ADDRESS StaMacAddr,
    _In_ ULONG Reason,
    _Out_ PDOT11_DISASSOCIATION_PARAMETERS DisassocPara
    )
{
    // 
    // NDIS header
    //
    MP_ASSIGN_NDIS_OBJECT_HEADER(
        DisassocPara->Header,
        NDIS_OBJECT_TYPE_DEFAULT,
        DOT11_DISASSOCIATION_PARAMETERS_REVISION_1,
        sizeof(DOT11_DISASSOCIATION_PARAMETERS)
        );
    
    // 
    // Copy Mac address
    //
    RtlCopyMemory(
        DisassocPara->MacAddr,
        *StaMacAddr,
        sizeof(DOT11_MAC_ADDRESS)
        );

    // 
    // Reason
    //
    DisassocPara->uReason = Reason;

    // 
    // IHV specific
    //
    DisassocPara->uIHVDataSize = DisassocPara->uIHVDataOffset = 0;
}

/**
 * Timer related functions
 */

/** 
 * MAC hash table enum callback function for inactive timeout.
 * An entry is removed from the MAC hash table if its inactive
 * time exceeds the predefined limit.
 * The caller must hold a write lock.
 */
BOOLEAN
AmInactiveStaEntryCallback(
    _In_ PMAC_HASH_TABLE Table,
    _In_ PMAC_HASH_ENTRY MacEntry,
    _In_ PVOID CallbackCtxt
    )
{
    LIST_ENTRY * head = (LIST_ENTRY *)CallbackCtxt;         // list of removed station entries
    PAP_STA_ENTRY staEntry = CONTAINING_RECORD(MacEntry, AP_STA_ENTRY, MacHashEntry);

    if (ApIncrementStaInactiveTime(staEntry) >= AP_NO_ACTIVITY_TIME)
    {
        // remove the entry from the table
        RemoveMacHashEntry(Table, MacEntry);

        // insert it into the list
        InsertTailList(head, &MacEntry->Linkage);
    }

    return TRUE;
}


/**
 * Timeout callback for the station inactive timer.
 *
 * \param param PAP_ASSOC_MGR
 */
VOID
AmStaInactiveTimeoutCallback(
    PVOID SystemSpecific1,
    PVOID param,
    PVOID SystemSpecific2,
    PVOID SystemSpecific3 
    )
{
    PAP_ASSOC_MGR assocMgr = (PAP_ASSOC_MGR)param;
    LIST_ENTRY head;                // list of stations to deauthenticate
    LIST_ENTRY * entry;
    PMAC_HASH_ENTRY macEntry;
    PAP_STA_ENTRY staEntry;
    MP_RW_LOCK_STATE lockState;
    
    UNREFERENCED_PARAMETER(SystemSpecific1);
    UNREFERENCED_PARAMETER(SystemSpecific2);
    UNREFERENCED_PARAMETER(SystemSpecific3);

    //
    // Initialize the list head
    //
    InitializeListHead(&head);

    //
    // Acquire write lock on the Mac table
    //
    MP_ACQUIRE_WRITE_LOCK(&assocMgr->MacHashTableLock, &lockState);

    //
    // increment inactive time for all stations
    // The stations whose inactive time exceeds the limit
    // are removed from the MAC table and insert in the list to deauthenticate
    //
    EnumMacEntry(
        &assocMgr->MacHashTable, 
        AmInactiveStaEntryCallback,
        &head
        );

    //
    // Release the write lock
    //
    MP_RELEASE_WRITE_LOCK(&assocMgr->MacHashTableLock, &lockState);
    
    //
    // Deauthenticate all stations in the list
    //
    while(!IsListEmpty(&head))
    {
        entry = RemoveHeadList(&head);
        macEntry = CONTAINING_RECORD(entry, MAC_HASH_ENTRY, Linkage);
        staEntry = CONTAINING_RECORD(macEntry, AP_STA_ENTRY, MacHashEntry);
    
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Deauthentication STA %02X-%02X-%02X-%02X-%02X-%02X because it has been inactive for a long time.",
                AP_GET_PORT_NUMBER(assocMgr->ApPort),
                staEntry->MacHashEntry.MacKey[0],
                staEntry->MacHashEntry.MacKey[1],
                staEntry->MacHashEntry.MacKey[2],
                staEntry->MacHashEntry.MacKey[3],
                staEntry->MacHashEntry.MacKey[4],
                staEntry->MacHashEntry.MacKey[5]
                ));
    
        AmDeauthenticateSta(
            assocMgr, 
            staEntry, 
            DOT11_DISASSOC_REASON_PEER_UNREACHABLE, 
            TRUE,                                       // Send deauth frame 
            DOT11_MGMT_REASON_INACTIVITY
            );
    }
    
}

/**
 * Timeout callback for the association timer.
 * The ref count of the station must be incremented
 * when the timer is scheduled.
 *
 * \param param PAP_STA_ENTRY
 */
VOID
AmStaAssocTimeoutCallback(
    PVOID SystemSpecific1,
    PVOID param,
    PVOID SystemSpecific2,
    PVOID SystemSpecific3 
    )
{
    PAP_STA_ENTRY staEntry = (PAP_STA_ENTRY)param;
    PAP_ASSOC_MGR assocMgr = staEntry->AssocMgr;
    BOOLEAN deauthSta = FALSE;
    MP_RW_LOCK_STATE lockState;
    /** NDIS indications */
    DOT11_INCOMING_ASSOC_STARTED_PARAMETERS assocStartPara;
    PDOT11_INCOMING_ASSOC_COMPLETION_PARAMETERS assocCompletePara = NULL;
    ULONG assocCompleteParaSize = 0;
    
    UNREFERENCED_PARAMETER(SystemSpecific1);
    UNREFERENCED_PARAMETER(SystemSpecific2);
    UNREFERENCED_PARAMETER(SystemSpecific3);

    //
    // Acquire write lock on the Mac table
    //
    MP_ACQUIRE_WRITE_LOCK(&assocMgr->MacHashTableLock, &lockState);
    
    //
    // Check the waiting for association request flag first
    //
    if (InterlockedExchange(
            &staEntry->WaitingForAssocReq,
            STA_NOT_WAITING_FOR_ASSOC_REQ
            ) == STA_WAITING_FOR_ASSOC_REQ)
    {
        //
        // Timeout for association request.
        // We need to remove the station from the MAC table
        // and deauth it
        //
        RemoveMacHashEntry(
            &assocMgr->MacHashTable, 
            &staEntry->MacHashEntry
            );
        deauthSta = TRUE;
    }

    //
    // Release the write lock
    //
    MP_RELEASE_WRITE_LOCK(&assocMgr->MacHashTableLock, &lockState);

    if (deauthSta)
    {
        //
        // Deauth the station
        //
        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL,("Port(%u): Deauthentication STA %02X-%02X-%02X-%02X-%02X-%02X because of association request timeout.",
                    AP_GET_PORT_NUMBER(assocMgr->ApPort),
                    staEntry->MacHashEntry.MacKey[0],
                    staEntry->MacHashEntry.MacKey[1],
                    staEntry->MacHashEntry.MacKey[2],
                    staEntry->MacHashEntry.MacKey[3],
                    staEntry->MacHashEntry.MacKey[4],
                    staEntry->MacHashEntry.MacKey[5]
                    ));
        
        AmDeauthenticateSta(
            assocMgr, 
            staEntry, 
            DOT11_DISASSOC_REASON_OS,                   // This reason code is ignored because the station is not associated yet. 
            TRUE,                                       // Send deauth frame 
            DOT11_MGMT_REASON_AUTH_NOT_VALID
            );

        //
        // Send back to back association start and association complete indications to OS
        //
        
        // 
        // Allocate association complete parameters
        //
        if (AmAllocAssocCompletePara(
                assocMgr, 
                0,              // no association request frame 
                0,              // no association response frame
                &assocCompletePara,
                &assocCompleteParaSize
                ) != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC_MGR, DBG_SERIOUS, ("Port(%u): Failed to allocate association completion parameters.",
                                        AP_GET_PORT_NUMBER(assocMgr->ApPort)));
        }
        else
        {
            // 
            // Fill in association complete information
            //
            AmPrepareAssocCompletePara(
                assocMgr, 
                NULL,               // no STA entry is required
                &staEntry->MacHashEntry.MacKey, 
                DOT11_FRAME_STATUS_FAILURE,         // TODO: a better status code?
                DOT11_ASSOC_ERROR_SOURCE_REMOTE, 
                FALSE,              // not reassociation
                NULL,               // no association request frame
                0,
                NULL,
                0,                  // no association response frame
                assocCompletePara,
                &assocCompleteParaSize
                );

            //
            // Prepare association start indication
            //
            AmPrepareAssocStartPara(
                &staEntry->MacHashEntry.MacKey, 
                &assocStartPara
                );

            // 
            // Association start indication
            //
            ApIndicateDot11Status(
                assocMgr->ApPort, 
                NDIS_STATUS_DOT11_INCOMING_ASSOC_STARTED, 
                NULL,                   // no request ID
                &assocStartPara, 
                sizeof(assocStartPara)
                );

            //
            // Association complete indication
            //
            ApIndicateDot11Status(
                assocMgr->ApPort, 
                NDIS_STATUS_DOT11_INCOMING_ASSOC_COMPLETION, 
                NULL,                   // no request ID
                assocCompletePara, 
                assocCompleteParaSize
                );
            
            //
            // Free indications
            //
            MP_FREE_MEMORY(assocCompletePara);
        }
    }
    
    //
    // Deref the station
    //
    ApDerefSta(staEntry);
}

