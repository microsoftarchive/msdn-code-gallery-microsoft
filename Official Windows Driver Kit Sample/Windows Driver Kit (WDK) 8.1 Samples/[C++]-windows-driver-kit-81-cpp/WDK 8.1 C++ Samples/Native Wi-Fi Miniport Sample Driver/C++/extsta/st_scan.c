/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_scan.c

Abstract:
    STA layer OS/internal scan functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_scan.c to st_scan.c

Notes:

--*/
#include "precomp.h"
#include "st_aplst.h"
#include "st_scan.h"
#include "st_conn.h"

#if DOT11_TRACE_ENABLED
#include "st_scan.tmh"
#endif

NDIS_STATUS
StaInitializeScanContext(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       ScanRequestLength = 0;
    PDOT11_SCAN_REQUEST_V2      pInternalScanRequestBuffer = NULL;
    PLONG                       pInternalScanChannelList = NULL;
    NDIS_TIMER_CHARACTERISTICS  timerChar;  

    do
    {
        NdisZeroMemory(&timerChar, sizeof(NDIS_TIMER_CHARACTERISTICS));
        
        timerChar.Header.Type = NDIS_OBJECT_TYPE_TIMER_CHARACTERISTICS;
        timerChar.Header.Revision = NDIS_TIMER_CHARACTERISTICS_REVISION_1;
        timerChar.Header.Size = sizeof(NDIS_TIMER_CHARACTERISTICS);
        timerChar.AllocationTag = EXTSTA_MEMORY_TAG;
        
        timerChar.TimerFunction = StaPeriodicScanCallback;
        timerChar.FunctionContext = pStation;

        ndisStatus = NdisAllocateTimerObject(
                        STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
                        &timerChar,
                        &(pStation->ScanContext.Timer_PeriodicScan)
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate periodic scan timer\n"));
            break;
        }

        NdisZeroMemory(&pStation->ScanContext.InternalScanRequest, sizeof(MP_SCAN_REQUEST));
        
        //
        // Determine the total length of the scan request
        //
        ScanRequestLength = sizeof(DOT11_SCAN_REQUEST_V2) +                 // The basic structure
             + sizeof(DOT11_SSID)                                           // 1 SSID
             ;                                                              // No IE's or RequestIDs
            
        MP_ALLOCATE_MEMORY(
            STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
            &pInternalScanRequestBuffer,
            ScanRequestLength,
            EXTSTA_MEMORY_TAG
            );

        if (pInternalScanRequestBuffer == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        NdisZeroMemory(pInternalScanRequestBuffer, ScanRequestLength);    

        // Allocate the channels buffer for internal scans
        MP_ALLOCATE_MEMORY(
            STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
            &pInternalScanChannelList,
            sizeof(ULONG),
            EXTSTA_MEMORY_TAG
            );

        if (pInternalScanChannelList == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        NdisZeroMemory(pInternalScanChannelList, sizeof(ULONG));    

        //
        // Fill defaults into the internal scan request buffer
        //
        pInternalScanRequestBuffer->dot11BSSType = dot11_BSS_type_infrastructure;
        pInternalScanRequestBuffer->dot11BSSID[0] = 0xFF;
        pInternalScanRequestBuffer->dot11BSSID[1] = 0xFF;
        pInternalScanRequestBuffer->dot11BSSID[2] = 0xFF;
        pInternalScanRequestBuffer->dot11BSSID[3] = 0xFF;
        pInternalScanRequestBuffer->dot11BSSID[4] = 0xFF;
        pInternalScanRequestBuffer->dot11BSSID[5] = 0xFF;
        
        pInternalScanRequestBuffer->dot11ScanType = dot11_scan_type_auto;
        pInternalScanRequestBuffer->bRestrictedScan = FALSE;
        
        pInternalScanRequestBuffer->udot11SSIDsOffset = 0;
        pInternalScanRequestBuffer->uNumOfdot11SSIDs = 0;

        pInternalScanRequestBuffer->bUseRequestIE = FALSE;
        pInternalScanRequestBuffer->uNumOfRequestIDs = 0;

        pInternalScanRequestBuffer->uPhyTypeInfosOffset = sizeof(DOT11_SSID);   // Phy Info follows SSID
                
        pInternalScanRequestBuffer->uNumOfPhyTypeInfos = 0;
        // Phy info structures are all zeroed out

        pInternalScanRequestBuffer->uIEsOffset = 0;
        pInternalScanRequestBuffer->uIEsLength = 0;


        //
        // Save it. We will use this for all internal scan requests
        //
        pStation->ScanContext.InternalScanRequest.Dot11ScanRequest = pInternalScanRequestBuffer;
        pStation->ScanContext.InternalScanRequest.ScanRequestBufferLength = ScanRequestLength;

        pStation->ScanContext.InternalScanRequest.ChannelList = pInternalScanChannelList;

        //
        // Periodic scans are disabled until restart
        //
        pStation->ScanContext.PeriodicScanDisableCount = 1;
    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (pStation->ScanContext.Timer_PeriodicScan != NULL)
        {
            NdisFreeTimerObject(pStation->ScanContext.Timer_PeriodicScan);
            pStation->ScanContext.Timer_PeriodicScan = NULL;
        }

        if (pInternalScanChannelList != NULL)
        {
            MP_FREE_MEMORY(pInternalScanChannelList);
            pStation->ScanContext.InternalScanRequest.ChannelList = NULL;
        }

        if (pInternalScanRequestBuffer != NULL)
        {
            MP_FREE_MEMORY(pInternalScanRequestBuffer);
            pStation->ScanContext.InternalScanRequest.Dot11ScanRequest = NULL;
        }
        pStation->ScanContext.InternalScanRequest.ScanRequestBufferLength = 0;        
    }

    
    return ndisStatus;
}

VOID
StaFreeScanContext(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    // Periodic scan is already stopped and cleaned up
    if (pStation->ScanContext.Timer_PeriodicScan != NULL)
    {
        NdisFreeTimerObject(pStation->ScanContext.Timer_PeriodicScan);
        pStation->ScanContext.Timer_PeriodicScan = NULL;
    }
    
    if (pStation->ScanContext.InternalScanRequest.ChannelList != NULL)
    {
        MP_FREE_MEMORY(pStation->ScanContext.InternalScanRequest.ChannelList);
        pStation->ScanContext.InternalScanRequest.ChannelList = NULL;
    }
    
    if (pStation->ScanContext.InternalScanRequest.Dot11ScanRequest != NULL)
    {
        MP_FREE_MEMORY(pStation->ScanContext.InternalScanRequest.Dot11ScanRequest);
        pStation->ScanContext.InternalScanRequest.Dot11ScanRequest = NULL;
    }

    pStation->ScanContext.InternalScanRequest.ScanRequestBufferLength = 0;        
}

VOID
StaCheckPMKIDCandidate(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    NDIS_STATUS         ndisStatus;
    ULONGLONG           time;
    PMP_BSS_ENTRY      APEntry;
    UCHAR               SecIELength = 0;
    PUCHAR              SecIEData = NULL;
    RSN_IE_INFO         RSNIEInfo;
    ULONG               Count, Size;
    ULONG               index1, index2;
    BOOLEAN             NeedIndicateAgain;
    UCHAR               Buffer[sizeof(DOT11_PMKID_CANDIDATE_LIST_PARAMETERS) + 
                               STA_PMKID_MAX_COUNT * sizeof(DOT11_BSSID_CANDIDATE)];

    PDOT11_PMKID_CANDIDATE_LIST_PARAMETERS  PMKIDParam;
    PDOT11_BSSID_CANDIDATE                  PMKIDCandidate;

    __try 
    {
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));        
        
        //
        // We must be connected and our auth algorithm must be RSNA.   
        //
        if (pStation->Config.AuthAlgorithm != DOT11_AUTH_ALGO_RSNA ||
            pStation->ConnectContext.ConnectState != CONN_STATE_READY_TO_ROAM ||
            pStation->ConnectContext.AssociateState != ASSOC_STATE_ASSOCIATED)
        {
            __leave;
        }

        //
        // If we just recently checked PMKID candidate, simply return.
        //
        NdisGetCurrentSystemTime((PLARGE_INTEGER)&time);
        if ((time - pStation->PMKIDCache.CheckingTime) / 10000000 < STA_PMKID_CHECK_INTERVAL)
        {
            __leave;
        }

        pStation->PMKIDCache.CheckingTime = time;

        //
        // Get all the AP we can connect to.
        //
        ndisStatus = StaGetCandidateAPList(pStation, TRUE);

        //
        // If we don't get any AP, quit.
        //
        if (ndisStatus != NDIS_STATUS_SUCCESS || pStation->ConnectContext.CandidateAPCount == 0)
        {
            __leave;
        }

        //
        // Check if we really need to indicate the PMKID candidates. We don't have to if any
        // candidates are already in our PMKID cache.
        //
        Count = pStation->ConnectContext.CandidateAPCount;
        if (Count > STA_PMKID_MAX_COUNT)
            Count = STA_PMKID_MAX_COUNT;

        if (Count == pStation->PMKIDCache.Count) 
        {
            NeedIndicateAgain = FALSE;
            for (index1 = 0; index1 < Count; index1++)
            {
                APEntry = pStation->ConnectContext.CandidateAPList[index1];
                PMKIDCandidate = pStation->PMKIDCache.Candidate;
                for (index2 = 0; index2 < pStation->PMKIDCache.Count; index2++, PMKIDCandidate++)
                {
                    if (NdisEqualMemory(APEntry->Dot11BSSID, PMKIDCandidate->BSSID, sizeof(DOT11_MAC_ADDRESS)) == 1)
                    {
                        break;
                    }
                }

                if (index2 == pStation->PMKIDCache.Count)
                {
                    NeedIndicateAgain = TRUE;
                    break;
                }
            }
        }
        else
        {
            NeedIndicateAgain = TRUE;
        }

        if (!NeedIndicateAgain)
        {
            //
            // Remove the extra refs that the get candidate list adds
            //
            for (index1 = 0; index1 < pStation->ConnectContext.CandidateAPCount; index1++)
            {
                STA_DECREMENT_REF(pStation->ConnectContext.CandidateAPList[index1]->RefCount);
            }
            __leave;
        }

        //
        // We will have to indicate the PMKID candidate again. Build the candidate list.
        //
        PMKIDCandidate = pStation->PMKIDCache.Candidate;
        for (index1 = 0; index1 < Count; index1++)
        {
            APEntry = pStation->ConnectContext.CandidateAPList[index1];

            NdisDprAcquireSpinLock(&(APEntry->Lock)); // Lock AP entry

            //
            // Get the RSN Capability. 
            //
            ndisStatus = Dot11GetInfoEle(APEntry->pDot11InfoElemBlob,
                                         APEntry->InfoElemBlobSize,
                                         DOT11_INFO_ELEMENT_ID_RSN,
                                         &SecIELength,
                                         (PVOID)&SecIEData);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                NdisDprReleaseSpinLock(&(APEntry->Lock));
                continue;
            }

            ndisStatus = Dot11ParseRNSIE(SecIEData, RSNA_OUI_PREFIX, SecIELength, &RSNIEInfo);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                NdisDprReleaseSpinLock(&(APEntry->Lock));            
                continue;
            }
            
            //
            // Copy the PMKID candidate.
            //

            NdisMoveMemory(PMKIDCandidate->BSSID, APEntry->Dot11BSSID, sizeof(DOT11_MAC_ADDRESS));
            NdisDprReleaseSpinLock(&(APEntry->Lock));
            
            PMKIDCandidate->uFlags = (RSNIEInfo.Capability & RSNA_CAPABILITY_PRE_AUTH) ? 
                                     DOT11_PMKID_CANDIDATE_PREAUTH_ENABLED : 0;
            PMKIDCandidate++;
        }

        pStation->PMKIDCache.Count = (ULONG)(PMKIDCandidate - pStation->PMKIDCache.Candidate);

        //
        // Remove the extra refs that the get candidate list adds
        //
        for (index1 = 0; index1 < pStation->ConnectContext.CandidateAPCount; index1++)
        {
            STA_DECREMENT_REF(pStation->ConnectContext.CandidateAPList[index1]->RefCount);
        }

        //
        // Build DOT11_PMKID_CANDIDATE_LIST_PARAMETERS structure for indication.
        //
        PMKIDParam = (PDOT11_PMKID_CANDIDATE_LIST_PARAMETERS)Buffer;
        MP_ASSIGN_NDIS_OBJECT_HEADER(PMKIDParam->Header, 
                                     NDIS_OBJECT_TYPE_DEFAULT,
                                     DOT11_PMKID_CANDIDATE_LIST_PARAMETERS_REVISION_1,
                                     sizeof(DOT11_PMKID_CANDIDATE_LIST_PARAMETERS));
        PMKIDParam->uCandidateListSize = pStation->PMKIDCache.Count;
        PMKIDParam->uCandidateListOffset = sizeof(DOT11_PMKID_CANDIDATE_LIST_PARAMETERS);

        Size = pStation->PMKIDCache.Count * sizeof(DOT11_BSSID_CANDIDATE);
        NdisMoveMemory(Add2Ptr(PMKIDParam, sizeof(DOT11_PMKID_CANDIDATE_LIST_PARAMETERS)),
                       pStation->PMKIDCache.Candidate,
                       Size);

        StaIndicateDot11Status(pStation, 
                                       NDIS_STATUS_DOT11_PMKID_CANDIDATE_LIST,
                                       NULL,
                                       Buffer,
                                       sizeof(DOT11_PMKID_CANDIDATE_LIST_PARAMETERS) + Size);
    }
    __finally
    {
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
    }
}

NDIS_STATUS
StaStartScan(
    _In_  PMP_EXTSTA_PORT            pStation,
    _In_  PVOID               ScanRequestID,
    _In_  PDOT11_SCAN_REQUEST_V2 pDot11ScanRequest,
    _In_ ULONG               ScanRequestBufferLength
    )
{
    NDIS_STATUS             ndisStatus;
    PDOT11_SCAN_REQUEST_V2  pExternalScanRequest = NULL;    // The scan request structure we would modify
                                                            // and use internally

    //
    // If a reset is in progress on the NIC, fail this request
    //
    if (MP_TEST_PORT_STATUS(STA_GET_MP_PORT(pStation), (MP_PORT_IN_RESET)))
    {
        MpTrace(COMP_OID, DBG_SERIOUS,  ("Scan failed as a reset is in progress on this adapter\n"));
        return NDIS_STATUS_RESET_IN_PROGRESS;
    }

    //
    // If a halt is in progress on the NIC, fail this request
    //
    if (MP_TEST_PORT_STATUS(STA_GET_MP_PORT(pStation), (MP_PORT_HALTING)))
    {
        MpTrace(COMP_OID, DBG_SERIOUS,  ("Scan failed as this adapter is halting\n"));
        return NDIS_STATUS_DOT11_MEDIA_IN_USE;
    }      
    
    //
    // If the NIC is paused, fail this request
    //
    if (MP_TEST_PORT_STATUS(STA_GET_MP_PORT(pStation), (MP_PORT_PAUSED | MP_PORT_PAUSING)))
    {
        MpTrace(COMP_OID, DBG_SERIOUS,  ("Scan failed as this adapter is pausing\n"));
        return NDIS_STATUS_DOT11_MEDIA_IN_USE;
    }


    if (STA_TEST_SCAN_FLAG(pStation, STA_EXTERNAL_SCAN_IN_PROGRESS))
    {
        MpTrace(COMP_SCAN, DBG_SERIOUS, ("External scan already in progress. Ignoring new request\n"));
        return NDIS_STATUS_DOT11_MEDIA_IN_USE;
    }

    do
    {
        //
        // Using the connect context lock to synchronize between 
        // scanning and connecting
        //
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));

        //
        // This stops any new internal scans from getting submitted to the
        // hardware portion
        //
        STA_SET_SCAN_FLAG(pStation, STA_EXTERNAL_SCAN_IN_PROGRESS);

        //
        // Check state of current internal scans
        //
        if (STA_TEST_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_IN_PROGRESS) == TRUE)
        {
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

            //
            // The internal scan has been submitted to hardware. Check if the OS 
            // is asking for a forced scan. If it is, abort the internal scan
            // and do the OS initiated scan, else we ignore the OS scan request
            //
            if (!(pDot11ScanRequest->dot11ScanType & dot11_scan_type_forced))
            {
                //
                // Not a forced scan, we skip the OS scan since we are scanning 
                // anyways
                //
                ndisStatus = NDIS_STATUS_DOT11_MEDIA_IN_USE;
                MpTrace(COMP_SCAN, DBG_LOUD, ("Not a forced scan. Skipping OS scan due to scanning\n"));
                break;
            }

            //
            // Internal scan has been submitted to hardware. Cancel it (and wait 
            // for it to cancel). Note that we do not stop the DPC from running
            // again. It continues running, but it wont do a scan.
            //
            StaWaitForPeriodicScan(pStation);

            NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));        
        }

        //
        // If we are performing an association, we skip the scan 
        // if its not forced. This would happen if the perdiodic scan 
        // decided to initiated roaming/connection and the OS sent down
        // a scan request
        //
        if ((pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT) &&
            ((pStation->ConnectContext.AssociateState != ASSOC_STATE_NOT_ASSOCIATED) &&
             (pStation->ConnectContext.AssociateState != ASSOC_STATE_ASSOCIATED))
           )
        {
            if (pDot11ScanRequest->dot11ScanType != dot11_scan_type_forced)
            {
                //
                // Not a forced scan, we skip the OS scan since we are associating
                //
                NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
                ndisStatus = NDIS_STATUS_DOT11_MEDIA_IN_USE;
                MpTrace(COMP_SCAN, DBG_LOUD, ("Not a forced scan. Skipping OS scan due to association\n"));
                break;
            }
        }

        MpTrace(COMP_SCAN, DBG_LOUD, ("Processing OS requested scan\n"));
        //
        // Next scan completion is for external scan
        //
        
        STA_SET_SCAN_FLAG(pStation, STA_COMPLETE_EXTERNAL_SCAN);
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

        // Allocate a new scan request structure that we modify and cache internally
        MP_ALLOCATE_MEMORY(
            STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
            &pExternalScanRequest,
            ScanRequestBufferLength,
            EXTSTA_MEMORY_TAG
            );
        
        if (pExternalScanRequest == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        
        NdisMoveMemory(
            pExternalScanRequest,
            pDot11ScanRequest,
            ScanRequestBufferLength
            );

        //
        // Save some state
        //
        NdisZeroMemory(&pStation->ScanContext.ExternalScanRequest, sizeof(MP_SCAN_REQUEST));
        
        pStation->ScanContext.ExternalScanRequest.Dot11ScanRequest = pExternalScanRequest;
        pStation->ScanContext.ExternalScanRequest.ScanRequestBufferLength = ScanRequestBufferLength;
        pStation->ScanContext.ScanRequestID = ScanRequestID;
        pStation->ScanContext.ExternalScanRequest.ScanRequestFlags = MP_SCAN_REQUEST_OS_ISSUED;
        
        //
        // If desired we would modify the scan request buffer here. We dont do anything for external
        // requests
        //


        //
        // Save scan start time
        //
        NdisGetCurrentSystemTime((PLARGE_INTEGER)&pStation->ScanContext.LastScanTime);
        
        ndisStatus = Mp11Scan(
            STA_GET_MP_PORT(pStation)->Adapter,
            STA_GET_MP_PORT(pStation),
            &pStation->ScanContext.ExternalScanRequest,
            Sta11ScanComplete
            );

    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        STA_CLEAR_SCAN_FLAG(pStation, (STA_EXTERNAL_SCAN_IN_PROGRESS | STA_COMPLETE_EXTERNAL_SCAN));
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        
        if (pExternalScanRequest != NULL)
            MP_FREE_MEMORY(pExternalScanRequest);

        pStation->ScanContext.ExternalScanRequest.Dot11ScanRequest = NULL;
    }
    
    return ndisStatus; 
}

VOID
StaScanComplete(
    _In_  PMP_PORT                Port,
    _In_  NDIS_STATUS             NdisStatus
    )
{
    PMP_EXTSTA_PORT             pStation = MP_GET_STA_PORT(Port);
    LARGE_INTEGER               fireTime;
    PVOID                       completeID;
    
    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));

    if (STA_TEST_SCAN_FLAG(pStation, STA_COMPLETE_PERIODIC_SCAN))
    {
        //
        // This is the completion of a periodic scan. 
        //
        STA_CLEAR_SCAN_FLAG(pStation, STA_COMPLETE_PERIODIC_SCAN);

        //
        // Run our "roaming" code to determine if we need to roam/connect
        //
        StaPeriodicScanComplete(pStation, NdisStatus);

        if (!STA_TEST_SCAN_FLAG(pStation, STA_STOP_PERIODIC_SCAN))
        {
            //
            // Permitted to requeue
            //
            STA_SET_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_QUEUED);

            if (((pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT) &&
                (pStation->ConnectContext.AssociateState == ASSOC_STATE_NOT_ASSOCIATED)) ||
                (pStation->ScanContext.ScanSeverity >= SCAN_REDISCOVER_CURRENT_AP))
            {
                // Not connected when we should be, attempt to run the scan routine again quickly
                fireTime.QuadPart = Int32x32To64((LONG)STA_SHORT_SCAN_TIMER_INTERVAL, -10000);
            }
            else
            {
                fireTime.QuadPart = Int32x32To64((LONG)STA_DEFAULT_SCAN_TIMER_INTERVAL, -10000);
            }
            NdisSetTimerObject(pStation->ScanContext.Timer_PeriodicScan, fireTime, 0, NULL);                
        }

        //
        // The clear is intentionally after the complete. In the complete, we
        // may roam. The Connect/Scan OID routines would wait for this flag
        // to clear, thus ensuring that nobody else would attempt to change
        // channels as the same time as they
        //
        STA_CLEAR_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_IN_PROGRESS);        
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));        
    }
    else
    {
        STA_CLEAR_SCAN_FLAG(pStation, STA_COMPLETE_EXTERNAL_SCAN);
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        
        //
        // This is the completion of a scan request from the OS
        //
        MPASSERT(pStation->ScanContext.ExternalScanRequest.Dot11ScanRequest != NULL);

        MP_FREE_MEMORY(pStation->ScanContext.ExternalScanRequest.Dot11ScanRequest);
        pStation->ScanContext.ExternalScanRequest.Dot11ScanRequest = NULL;
        completeID = pStation->ScanContext.ScanRequestID;

        pStation->ScanContext.ScanRequestID = NULL;

        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));        
        // Clear the flag before indicating the status
        STA_CLEAR_SCAN_FLAG(pStation, STA_EXTERNAL_SCAN_IN_PROGRESS);
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

        StaIndicateDot11Status(pStation, 
            NDIS_STATUS_DOT11_SCAN_CONFIRM,
            completeID,
            &NdisStatus,
            sizeof(NDIS_STATUS)
            );

    }

    //
    // Just completed a scan. Perfect time to check new PMKID
    // candidates. This applies to RNSA (WPA2) only.
    //
    if (pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA)
    {
        StaCheckPMKIDCandidate(pStation);
    }

    if (pStation->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED)
    {
        pStation->Config.CheckForProtectionInERP = TRUE;
    }
}

NDIS_STATUS
Sta11ScanComplete(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   Data
    )
{
    NDIS_STATUS                 Status = *(NDIS_STATUS*)Data;

    // Call the completion handler
    StaScanComplete(Port, Status);

    return NDIS_STATUS_SUCCESS;
}

// LOCK HELD
BOOLEAN
StaInternalScanPermitted(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    // Dont queue if OS scan is asking for a scan or we are not supposed to scan
    if (STA_TEST_SCAN_FLAG(pStation, STA_EXTERNAL_SCAN_IN_PROGRESS | STA_STOP_PERIODIC_SCAN))
    {
        return FALSE;
    }
    
    //
    // Do not queue scan if we are in reset. ResetEnd will
    // queue it
    //
    if (MP_TEST_PORT_STATUS(STA_GET_MP_PORT(pStation), (MP_PORT_IN_RESET)))
    {
        return FALSE;
    }

    //
    // If media streaming, only perform the scan if we do not have an active
    // connection
    //    
    if (pStation->Config.MediaStreamingEnabled == TRUE)
    {
        //
        // Media streaming is on. Can only scan if we are not connected
        //
        if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
        {
            if (pStation->ConnectContext.AssociateState == ASSOC_STATE_NOT_ASSOCIATED)
            {
                // Expected to be connected, but am not
                return TRUE;
            }
        }
        else
        {
            // Not connected
            return TRUE;
        }

        if (pStation->ScanContext.ScanSeverity >= SCAN_FIND_USABLE_AP)
        {
            // Lost connection
            return TRUE;
        }
        
        return FALSE;
    }

    //
    // If we are in the middle of attempting to associate, dont scan
    //
    if ((pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT) &&
        ((pStation->ConnectContext.AssociateState != ASSOC_STATE_NOT_ASSOCIATED) &&
         (pStation->ConnectContext.AssociateState != ASSOC_STATE_ASSOCIATED))
       )
    {
        return FALSE;
    }

    //
    // If we are in netmon mode, disable scanning
    //
    if (STA_GET_MP_PORT(pStation)->CurrentOpMode == DOT11_OPERATION_MODE_NETWORK_MONITOR)
    {
        return FALSE;
    }
    
    //
    // Additional conditions we can check for are whether or not the
    // medium is busy or not for us to perform a scan
    //

    return TRUE;
}


// LOCK HELD
BOOLEAN
StaInternalScanRequired(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    ULONGLONG               ullCurrentSystemTime;
    ULONGLONG               ullOldestBeaconTime;
    ULONGLONG               ullAcceptableScanTime;
    PMP_BSS_ENTRY          pAPEntry;
    BOOLEAN                 bScanRequired = FALSE;
    // Depending on whether we are scanning for updates or scanning
    // for roaming/connecting, we choose appropriate interscan gap
    ULONGLONG               ScanGapSeconds = STA_PERIODIC_SCAN_NORMAL_GAP;
    PCHAR                   pScanReason = NULL;        // For debug purposes only

    NdisGetCurrentSystemTime((PLARGE_INTEGER)&ullCurrentSystemTime);

    do
    {
        //
        // We scan if:
        //  1. we are not connected and should be connected
        //  2. if we are connected, but need to roam 
        //  3. to periodically collect environment information
        //
        pStation->ScanContext.ScanSeverity = SCAN_LOW_SEVERITY;
        
        if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
        {
            if (pStation->ConnectContext.AssociateState == ASSOC_STATE_NOT_ASSOCIATED)
            {
                //
                // Not connected when we should be connected. We will scan and attempt
                // to connect
                //
                bScanRequired = TRUE;
                ScanGapSeconds = 0; // Ignore the scan gaps, we must scan now
                pStation->ScanContext.ScanSeverity = SCAN_FIND_USABLE_AP;
                pScanReason = "Scan/Roam required because we are not associated but should be\n";
                break;
            }
            else if ((pStation->ConnectContext.ConnectState == CONN_STATE_READY_TO_ROAM) &&
                     (pStation->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED)
                    )
            {
                //
                // We are associated. The AP is good
                //

                //
                // Check the time that we last received a beacon from the access point
                // we are associated with. If its been long, we roam
                //
                pAPEntry = pStation->ConnectContext.ActiveAP;
                MPASSERT(pAPEntry);
                
                NdisDprAcquireSpinLock(&(pAPEntry->Lock));      // Called with lock held = DISPATCH

                pAPEntry->NoBeaconRoamTime = pAPEntry->BeaconInterval * pStation->RegInfo->LostAPRoamBeaconCount;
                if (pAPEntry->NoBeaconRoamTime < STA_MIN_INFRA_UNREACHABLE_TIMEOUT)
                {
                    //
                    // Beacon interval is so short that we may think we lost 
                    // connectivity when we are scanning. Adjust for that
                    //
                    pAPEntry->NoBeaconRoamTime = STA_MIN_INFRA_UNREACHABLE_TIMEOUT;
                }
                
                ullOldestBeaconTime = ullCurrentSystemTime;
                if (pAPEntry->NoBeaconRoamTime <= ullOldestBeaconTime)
                    ullOldestBeaconTime -= pAPEntry->NoBeaconRoamTime;

                if (pAPEntry->HostTimestamp < ullOldestBeaconTime)
                {
                    //
                    // Determine if we need to drop connectivity
                    //
                    ullOldestBeaconTime = ullCurrentSystemTime - STA_MAX_INFRA_UNREACHABLE_TIMEOUT;
                    if (pAPEntry->HostTimestamp < ullOldestBeaconTime)
                    {
                        //
                        // Havent received a beacon for a long while. If we do not find
                        // a better AP, we must assume we are disconnected
                        //
                        pStation->ScanContext.ScanSeverity = SCAN_FIND_USABLE_AP;

                        //
                        // Reset its timestamp to be zero, so that we do not
                        // report this AP to the OS if we dont receive a new beacon from it
                        //
                        pAPEntry->HostTimestamp = 0;
                    }
                    else
                    {
                        // Scan for the AP & if we dont find it, keep trying
                        pStation->ScanContext.ScanSeverity = SCAN_REDISCOVER_CURRENT_AP;
                    }
                    NdisDprReleaseSpinLock(&(pAPEntry->Lock));

                    //
                    // Havent received a beacon from this AP for a while
                    // lets try to roam
                    //

                    // Attempt to find the current AP again by a very directed scan
                    pStation->ConnectContext.AttemptFastRoam = TRUE;
                    
                    bScanRequired = TRUE;
                    ScanGapSeconds = 0; // Ignore the scan gaps, we must scan now
                    pScanReason = "Scan/Roam required because we havent received a beacon for a while\n";
                    break;
                }

                //
                // Check the number of beacons that we have received with 
                // RSSI/LinkQuality value below threshold
                //
                if (pAPEntry->LowQualityCount > pStation->RegInfo->RSSIRoamBeaconCount)
                {
                    NdisDprReleaseSpinLock(&(pAPEntry->Lock));

                    //
                    // Have received a sequence of low strength beacons
                    // from this AP. Lets check if we can find a better AP
                    // and if yes roam
                    //
                    bScanRequired = TRUE;
                    ScanGapSeconds = pStation->ScanContext.RoamingScanGap;
                    pStation->ScanContext.ScanSeverity = SCAN_FIND_BETTER_AP;
                    pScanReason = "Scan/Roam required because we have low signal strength\n";
                    break;
                }

                //
                // Need to scan and roam due send failures reported by hardware
                //
                if (pStation->ConnectContext.RoamForSendFailures == TRUE)
                {
                    NdisDprReleaseSpinLock(&(pAPEntry->Lock));

                    // reset RoamForSendFailures so next time it will not try to scan/roam again
                    pStation->ConnectContext.RoamForSendFailures = FALSE;
                    
                    bScanRequired = TRUE;
                    ScanGapSeconds = pStation->ScanContext.RoamingScanGap;
                    pStation->ScanContext.ScanSeverity = SCAN_FIND_BETTER_AP;
                    pScanReason = "Scan/Roam required because there were too many retries for send\n";
                    break;
                }

                //
                // If some AP characteristics (eg. channel) has changed, that would 
                // cause us to do a fresh association.
                //
                if (StaHasAPConfigurationChanged(pStation))
                {                    
                    NdisDprReleaseSpinLock(&(pAPEntry->Lock));
                    bScanRequired = TRUE;
                    ScanGapSeconds = 0;
                    pStation->ScanContext.ScanSeverity = SCAN_FIND_USABLE_AP;
                    pScanReason = "Scan/Roam required because there has been a change in the AP's configuration\n";
                    break;
                }

                NdisDprReleaseSpinLock(&(pAPEntry->Lock));
            }
        }

        //
        // Check if we are due for doing a periodic scan. We dont need to do a periodic scan for infrastructure. 
        // It causes unnecessary perf hit as we move to a different channel, etc. We do need the scan
        // for adhoc
        //
        if (pStation->Config.BSSType == dot11_BSS_type_independent)
        {
            if (pStation->ScanContext.PeriodicScanCounter++ >= STA_DEFAULT_SCAN_TICK_COUNT)
            {
                //
                // Not scanned for specified duration. Lets scan
                //
                bScanRequired = TRUE;
                break;
            }
        }

        // If there any other reason we need to scan?
        if (pStation->ScanContext.ScanSeverity == SCAN_HIGH_SEVERITY)
        {
            bScanRequired = TRUE;
            break;
        }
    } while (FALSE);

    if (bScanRequired)
    {
        //
        // To avoid too many unnecessary scans, lets check if we have scanned recently.
        // If yes, we delay our scan for a while
        //
        ullAcceptableScanTime = pStation->ScanContext.LastScanTime +             
             ScanGapSeconds * 10000000; // Convert seconds to 100nS

        if (ullAcceptableScanTime > ullCurrentSystemTime)
        {
            //
            // Scanned recently. Dont scan again
            //
            bScanRequired = FALSE;
        }
        else
        {
            //
            // We are going to scan. Increase the scan gap we use during roaming
            //
            pStation->ScanContext.RoamingScanGap 
                = 2 * pStation->ScanContext.RoamingScanGap;
            if (pStation->ScanContext.RoamingScanGap > STA_PERIODIC_SCAN_NORMAL_GAP)
            {
                pStation->ScanContext.RoamingScanGap = STA_PERIODIC_SCAN_SHORT_GAP;
            }

            //
            // Log if scanning due to roaming needs
            //
            if (pScanReason != NULL)
                MpTrace(COMP_ASSOC, DBG_NORMAL, ("%s\n", pScanReason));
       }
    }
    
    return bScanRequired;
}


VOID
StaStartPeriodicScan(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    LARGE_INTEGER               fireTime;
    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));

    // Check if its OK to enable scans
    if (NdisInterlockedDecrement((LONG *)&pStation->ScanContext.PeriodicScanDisableCount) == 0)
    {
        STA_CLEAR_SCAN_FLAG(pStation, STA_STOP_PERIODIC_SCAN);
        STA_SET_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_QUEUED);

        //
        // Reset the scan counter
        //
        pStation->ScanContext.PeriodicScanCounter = 0;

        //
        // If we are not connected
        //
        if ((pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT) &&
            (pStation->ConnectContext.AssociateState == ASSOC_STATE_NOT_ASSOCIATED)
            )
        {
            // MpTrace(COMP_SCAN, DBG_NORMAL, ("Reenabling FAST periodic scans\n"));
            //
            // Scan quickly
            //
            fireTime.QuadPart = Int32x32To64((LONG)STA_FORCED_PERIODIC_SCAN_INTERVAL, -10000);
            NdisSetTimerObject(pStation->ScanContext.Timer_PeriodicScan, fireTime, 0, NULL);
        }
        else
        {        
            // MpTrace(COMP_SCAN, DBG_NORMAL, ("Reenabling SLOW periodic scans\n"));
            //
            // Queue the periodic scan request
            //
            fireTime.QuadPart = Int32x32To64((LONG)STA_DEFAULT_SCAN_TIMER_INTERVAL, -10000);
            NdisSetTimerObject(pStation->ScanContext.Timer_PeriodicScan, fireTime, 0, NULL);
        }
    }
    else
    {
        MpTrace(COMP_SCAN, DBG_NORMAL, ("Skipped reenabling of periodic scans due to non-zero disable count\n"));
    }
    NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));   
}

VOID
StaStopPeriodicScan(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    BOOLEAN                 ScanTimerCancelled;

    // MUST BE CALLED BELOW DISPATCH
    
    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));

    // 
    // Another periodic scan disable has been called
    //
    NdisInterlockedIncrement((LONG *)&pStation->ScanContext.PeriodicScanDisableCount);
    
    //
    // Stop the scan from requeuing itself
    //
    STA_SET_SCAN_FLAG(pStation, STA_STOP_PERIODIC_SCAN);
    
    //
    // Cancel any scans that may be queued by not yet run
    //
    ScanTimerCancelled = NdisCancelTimerObject(pStation->ScanContext.Timer_PeriodicScan);
    if (ScanTimerCancelled)
    {
        STA_CLEAR_SCAN_FLAG(pStation, (STA_PERIODIC_SCAN_IN_PROGRESS | STA_PERIODIC_SCAN_QUEUED));
    }

    //
    // Cancel any scans that are already running
    //
    if (STA_TEST_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_IN_PROGRESS))
    {
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

        //
        // Wait for the periodic scan to finish
        //
        StaWaitForPeriodicScan(pStation);
    }
    else
    {
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
    }    
}

//
// Waits for the current periodic scan to complete. It cancels the scan
// and also waits for the routine to complete executing
//
VOID
StaWaitForPeriodicScan(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    while (TRUE)
    {
        //
        // Cancel scanning. If periodic scan request
        // had not yet been submitted, we would return without 
        // the scan completed. Hence we use this loop
        //
        Mp11CancelScan(STA_GET_MP_PORT(pStation)->Adapter, STA_GET_MP_PORT(pStation));

        if (!STA_TEST_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_IN_PROGRESS))
        {
            //
            // Its done
            //
            break;
        }        
    }
}

VOID
StaStopExternalScan(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    // Wait for the OS scan to finish
    if (STA_TEST_SCAN_FLAG(pStation, STA_EXTERNAL_SCAN_IN_PROGRESS))
    {
        // We cannot proceed while scan is in progress
        Mp11CancelScan(STA_GET_MP_PORT(pStation)->Adapter, STA_GET_MP_PORT(pStation));
    }
}


VOID
StaStopScan(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    // Periodic scans (first to ensure that completion of an OS scan does not trigger a
    // periodic scan)
    StaStopPeriodicScan(pStation);

    // Any remaining OS scans
    StaStopExternalScan(pStation);
}

// LOCK HELD
VOID
StaForceInternalScan(
    _In_  PMP_EXTSTA_PORT            pStation,
    _In_  BOOLEAN             bScanToRoam
    )
{
    BOOLEAN                 ScanTimerCancelled;
    LARGE_INTEGER           fireTime;

    UNREFERENCED_PARAMETER(bScanToRoam);
    
    //
    // If we are allowed to queue a periodic scan and its not already running
    //
    if (!STA_TEST_SCAN_FLAG(pStation, (STA_STOP_PERIODIC_SCAN | STA_PERIODIC_SCAN_IN_PROGRESS)))
    {
        //
        // The periodic scan runs at big intervals. To make it
        // run now, lets try to cancel it and if successful
        // queue it again with short interval. If we cannot cancel it,
        // its either already running or isnt supposed to run
        //        
        ScanTimerCancelled = NdisCancelTimerObject(pStation->ScanContext.Timer_PeriodicScan);
        if (ScanTimerCancelled)
        {
            // Not touching the flags since I requeue
            fireTime.QuadPart = Int32x32To64((LONG)STA_FORCED_PERIODIC_SCAN_INTERVAL, -10000);
            NdisSetTimerObject(pStation->ScanContext.Timer_PeriodicScan, 
                fireTime, 
                0, 
                NULL
                );
        }
    }
}


// LOCK HELD
_Requires_lock_held_((&pStation->ConnectContext.Lock)->SpinLock)
_IRQL_requires_(DISPATCH_LEVEL)
VOID
StaPeriodicScanComplete(
    _In_  PMP_EXTSTA_PORT            pStation,
    _In_  NDIS_STATUS         ndisStatus
    )
{
    if (ndisStatus == NDIS_STATUS_REQUEST_ABORTED)
    {
        //
        // Scan request had aborted
        //
        return;
    }

    if ((pStation->ConnectContext.ConnectState == CONN_STATE_READY_TO_CONNECT) &&
        (pStation->ConnectContext.AssociateState == ASSOC_STATE_NOT_ASSOCIATED))
    {
        //
        // This is a scan complete after a connect request. We havent yet
        // tried to connect. Initiate the connection process
        //
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        StaConnectStart(pStation, FALSE);
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        return;
    }
    else
    {
        if (pStation->ConnectContext.ConnectState == CONN_STATE_READY_TO_ROAM)
        {
            //
            // Check if we can roam. If yes, lets attempt to roam
            //
            StaRoamStart(pStation);
        }
    }

}

//
// This is running periodically (when we are not reset or stopping)
//
VOID
StaPeriodicScanCallback(
    PVOID  SystemSpecific1,
    PVOID  FunctionContext,
    PVOID  SystemSpecific2,
    PVOID  SystemSpecific3
    )
{
    PMP_EXTSTA_PORT                pStation = (PMP_EXTSTA_PORT)FunctionContext;
    NDIS_STATUS             ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_SCAN_REQUEST_V2  pInternalScanRequest = NULL;
    PDOT11_SSID             pSSID;
    BOOLEAN                 bRequeueScan = FALSE;
    LARGE_INTEGER           nextFireTime;

    UNREFERENCED_PARAMETER(SystemSpecific1);
    UNREFERENCED_PARAMETER(SystemSpecific2);
    UNREFERENCED_PARAMETER(SystemSpecific3);

    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
    STA_CLEAR_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_QUEUED);
    STA_SET_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_IN_PROGRESS);

    do
    {
        //
        // Check if we NEED to do a scan
        //
        if (StaInternalScanRequired(pStation) == FALSE)
        {
            STA_CLEAR_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_IN_PROGRESS);
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            bRequeueScan = TRUE;
            break;
        }

        //
        // Check if we can do a scan
        //
        if (StaInternalScanPermitted(pStation) == FALSE)
        {
            STA_CLEAR_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_IN_PROGRESS);
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            bRequeueScan = TRUE;
            MpTrace(COMP_SCAN, DBG_LOUD, ("Periodic scan required but not permitted at this time\n"));
            break;
        }

        //
        // Next scan complete would be for a periodic scan
        //
        STA_SET_SCAN_FLAG(pStation, STA_COMPLETE_PERIODIC_SCAN);

        MpTrace(COMP_ASSOC, DBG_LOUD, ("Initiating periodic scan\n"));

        //
        // Reset scan tick count
        //
        pStation->ScanContext.PeriodicScanCounter = 0;
        
        //
        // Copy the SSID from the current config into the internal
        // scan request buffer
        //
        pInternalScanRequest = pStation->ScanContext.InternalScanRequest.Dot11ScanRequest;
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

        //
        // If we are scanning because we want to find a better AP, ask for a
        // forced scan. This is to ensure that the scan does not get masked
        //
        if (pStation->ScanContext.ScanSeverity >= SCAN_FIND_BETTER_AP )
        {
            pInternalScanRequest->dot11ScanType = dot11_scan_type_auto | dot11_scan_type_forced;
        }
        else
        {
            pInternalScanRequest->dot11ScanType = dot11_scan_type_auto;
        }

        //
        // Check if we should attempt to connect without doing a 
        // full scan. This is mainly used for fast reconnection
        // on resume
        //
        if (pStation->ConnectContext.AttemptFastRoam)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Attempt fast reconnect by scanning channel %d first\n", 
                pStation->ConnectContext.AssociationChannel));

            pStation->ConnectContext.AttemptFastRoam = FALSE;

            // Ask the HW to scan a specific channel
            pStation->ScanContext.InternalScanRequest.ChannelCount = 1;
            pStation->ScanContext.InternalScanRequest.ChannelList[0] = pStation->ConnectContext.AssociationChannel;

            // This is a forced scan
            pInternalScanRequest->dot11ScanType = dot11_scan_type_auto | dot11_scan_type_forced;
        }
        else
        {
            // Scan all possible channels
            pStation->ScanContext.InternalScanRequest.ChannelCount = 0;
        }

        if (pStation->ScanContext.SSIDInProbeRequest)
        {
            pSSID = (PDOT11_SSID) pInternalScanRequest->ucBuffer;
            NdisMoveMemory(pSSID, &(pStation->Config.SSID), sizeof(DOT11_SSID));
            pInternalScanRequest->uNumOfdot11SSIDs = 1;
        }
        else
        {
            //
            // Insert the broadcast SSID
            //
            NdisZeroMemory(pInternalScanRequest->ucBuffer, sizeof(DOT11_SSID));
            pInternalScanRequest->uNumOfdot11SSIDs = 1;
        }

        //
        // Save scan time
        //
        NdisGetCurrentSystemTime((PLARGE_INTEGER)&pStation->ScanContext.LastScanTime);

        //
        // Start the scan
        //
        ndisStatus = Mp11Scan(
            STA_GET_MP_PORT(pStation)->Adapter,
            STA_GET_MP_PORT(pStation),
            &pStation->ScanContext.InternalScanRequest,
            Sta11ScanComplete
            );

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            bRequeueScan = TRUE;
            // Flag is cleared below where we have the lock
        }
    } while (FALSE);

    //
    // We were not able to scan for some reason. Check if we
    // should reschedule ourselves for later scanning
    //
    if (bRequeueScan == TRUE)
    {
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        if (!STA_TEST_SCAN_FLAG(pStation, STA_STOP_PERIODIC_SCAN))
        {
            //
            // Permitted to requeue
            //
            STA_SET_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_QUEUED);
            STA_CLEAR_SCAN_FLAG(pStation, (STA_PERIODIC_SCAN_IN_PROGRESS | STA_COMPLETE_PERIODIC_SCAN));

            if ((pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT) &&
                (pStation->ConnectContext.AssociateState == ASSOC_STATE_NOT_ASSOCIATED))
            {
                // Not connected when we should be, attempt to run the scan routine again quickly
                nextFireTime.QuadPart = Int32x32To64((LONG)STA_SHORT_SCAN_TIMER_INTERVAL, -10000);
            }
            else
            {
                nextFireTime.QuadPart = Int32x32To64((LONG)STA_DEFAULT_SCAN_TIMER_INTERVAL, -10000);
            }
            NdisSetTimerObject(pStation->ScanContext.Timer_PeriodicScan, 
                nextFireTime, 
                0, 
                NULL
                );
        }
        else
        {
            STA_CLEAR_SCAN_FLAG(pStation, (STA_PERIODIC_SCAN_IN_PROGRESS | STA_COMPLETE_PERIODIC_SCAN));
        }
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
    }
}
