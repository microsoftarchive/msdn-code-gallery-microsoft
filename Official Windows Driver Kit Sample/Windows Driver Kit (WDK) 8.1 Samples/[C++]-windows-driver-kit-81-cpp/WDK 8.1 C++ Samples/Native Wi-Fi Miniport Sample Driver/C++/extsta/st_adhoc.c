/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    St_Adhoc.c

Abstract:
    STA layer adhoc connection functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_adhoc.c to st_adhoc.c

Notes:

--*/
#include "precomp.h"
#include "st_aplst.h"
#include "st_adhoc.h"
#include "st_scan.h"
#include "st_send.h"

#if DOT11_TRACE_ENABLED
#include "St_adhoc.tmh"
#endif

//
// The following is how Ad Hoc connection works:
//    1. We have a list of beaconing Ad Hoc stations through active or passive scanning. 
//       Go through the list, find all the stations that match our desired SSID
//       and BSSID list.
//    2. For each station found in step 1, we do the following until the start request succeeds for the 
//       a station.
//        a. Issue a join request using the station's BSSID and SSID. 
//        b. If we get response, do a start request using the station's BSSID, SSID and ATIM.
//    3. If step 2 fails, we start our own Ad Hoc network.
//    4. Indicate connection start and connection complete. At this stage, no station is indicated 
//       as associated.
//    5. When a beacon frame or probe response frame is received from a Ad Hoc station, if it
//       has not been indicated as associated but it matches with our BSSID/SSID, indicate association
//       start and associate complete for the station. If the station has been indicated as associated
//       but its SSID/BSSID do not match with ours, indicate disassociation for the station.
//    6. We have a timer routine that disassociates any station from which no beacon or probe response
//       frame is received for a specified period of time. It also removes the station from the list if 
//       no beacon or probe response is received for another specified period of time.
//

NTSTATUS
StaConnectAdHoc(
    _In_  PMP_EXTSTA_PORT pStation
    )
{
    NDIS_STATUS ndisStatus;

    //
    // Cannot connect when scan is in progress
    //
    if (STA_TEST_SCAN_FLAG(pStation, STA_EXTERNAL_SCAN_IN_PROGRESS))
    {
        MpTrace(COMP_SCAN, DBG_SERIOUS, ("External scan already in progress. Ignoring adhoc connect request\n"));
        return NDIS_STATUS_DOT11_MEDIA_IN_USE;
    }

    //
    // Check the AdHoc state
    //

    NdisAcquireSpinLock(&pStation->AdHocStaInfo.StaInfoLock);
    
    if (pStation->AdHocStaInfo.AdHocState != STA_ADHOC_DISCONNECTED)
    {
        ndisStatus = NDIS_STATUS_INVALID_STATE;
    }
    else
    {
        ndisStatus = NDIS_STATUS_SUCCESS;
        pStation->AdHocStaInfo.AdHocState |= STA_ADHOC_CONNECTION_IN_PROGRESS;
    }

    NdisReleaseSpinLock(&pStation->AdHocStaInfo.StaInfoLock);

    //
    // Schedule a work item to do Ad Hoc connect if we can proceed.
    //

    if (ndisStatus == NDIS_STATUS_SUCCESS) 
    {
        NdisQueueIoWorkItem(
            pStation->AdHocStaInfo.ConnectWorkItem,
            StaConnectAdHocWorkItem,
            pStation        
            );
    }

    return ndisStatus;
}

NTSTATUS
StaDisconnectAdHoc(
    _In_  PMP_EXTSTA_PORT pStation
    )
{
    DOT11_DISASSOCIATION_PARAMETERS     disassocParam;
    BOOLEAN                             cancelled;
    NDIS_STATUS                         ndisStatus;

    //
    // Check the AdHoc state
    //
    NdisAcquireSpinLock(&pStation->AdHocStaInfo.StaInfoLock);
    
    if (pStation->AdHocStaInfo.AdHocState != STA_ADHOC_CONNECTED)
    {
        ndisStatus = NDIS_STATUS_INVALID_STATE;
    }
    else
    {
        ndisStatus = NDIS_STATUS_SUCCESS;
        pStation->AdHocStaInfo.AdHocState |= STA_ADHOC_DISCONNECTION_IN_PROGRESS;

        //
        // Cancel the AdHoc watchdog timer. If we failed to cancel the timer, 
        // wait till the timer counter goes to 0.
        //
        cancelled = NdisCancelTimerObject(pStation->AdHocStaInfo.WatchdogTimer);
        if (!cancelled) 
        {
            while (pStation->AdHocStaInfo.TimerCounter != 0) 
            {
                NdisStallExecution(50);      // 50 us
            }
        }
        else
        {
            NdisInterlockedDecrement(&pStation->AdHocStaInfo.TimerCounter);
        }
    }

    NdisReleaseSpinLock(&pStation->AdHocStaInfo.StaInfoLock);

    if (ndisStatus != NDIS_STATUS_SUCCESS) 
        return ndisStatus;

    //
    // Send every station on the ad hoc network a de-auth message and clear their
    // association states.
    //
    StaClearStaListAssocState(pStation, TRUE);

    //
    // Stop beaconing and receiving data frames.
    //
    StaStopAdHocBeaconing(pStation);
    if (pStation->AdHocStaInfo.BSSDescription)
    {
        MP_FREE_MEMORY(pStation->AdHocStaInfo.BSSDescription);
        pStation->AdHocStaInfo.BSSDescription = NULL;
    }
    
    //
    // Indicate up DISASSOCIATION status 
    //
    MP_ASSIGN_NDIS_OBJECT_HEADER(disassocParam.Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_DISASSOCIATION_PARAMETERS_REVISION_1,
                                 sizeof(DOT11_DISASSOCIATION_PARAMETERS));
    disassocParam.uReason = DOT11_DISASSOC_REASON_OS;
    NdisFillMemory(disassocParam.MacAddr, sizeof(DOT11_MAC_ADDRESS), 0xff);

    StaIndicateDot11Status(pStation, 
                           NDIS_STATUS_DOT11_DISASSOCIATION,
                           NULL,
                           &disassocParam,
                           sizeof(DOT11_DISASSOCIATION_PARAMETERS));

    // Also notify hw about disconnected status
    VNic11NotifyConnectionStatus(
        STA_GET_VNIC(pStation), 
        FALSE, 
        NDIS_STATUS_DOT11_DISASSOCIATION,
        &disassocParam, 
        sizeof(DOT11_DISASSOCIATION_PARAMETERS)
        );    

    NdisAcquireSpinLock(&pStation->AdHocStaInfo.StaInfoLock);
    pStation->AdHocStaInfo.AdHocState = STA_ADHOC_DISCONNECTED;
    NdisReleaseSpinLock(&pStation->AdHocStaInfo.StaInfoLock);

    return ndisStatus;
}

NDIS_STATUS
StaInitializeAdHocStaInfo(
    _In_  PMP_EXTSTA_PORT        pStation
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_TIMER_CHARACTERISTICS  timerChar;               
    BOOLEAN                     lockAllocated = FALSE;

    do
    {
        pStation->AdHocStaInfo.fBeaconing = FALSE;
        pStation->AdHocStaInfo.BSSDescription = NULL;
        pStation->AdHocStaInfo.StaCount = 0;
        pStation->AdHocStaInfo.DeauthStaCount = 0;
        pStation->AdHocStaInfo.AdHocState = STA_ADHOC_DISCONNECTED;
        ndisStatus = MP_ALLOCATE_READ_WRITE_LOCK(&pStation->AdHocStaInfo.StaListLock, STA_GET_MP_PORT(pStation)->MiniportAdapterHandle);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate adhoc read/write lock\n"));
            break;
        }
        lockAllocated = TRUE;
        
        NdisInitializeListHead(&pStation->AdHocStaInfo.StaList);
        NdisAllocateSpinLock(&pStation->AdHocStaInfo.StaInfoLock);

        //
        // Allocate the work item (StaConnectAdHocWorkItem)
        //
        pStation->AdHocStaInfo.ConnectWorkItem = NdisAllocateIoWorkItem(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle);
        if(pStation->AdHocStaInfo.ConnectWorkItem == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate adhoc connect workitem\n"));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        
        NdisInitializeEvent(&pStation->AdHocStaInfo.JoinCompletionEvent);
        NdisInitializeEvent(&pStation->AdHocStaInfo.StartBSSCompletionEvent);
        NdisInitializeEvent(&pStation->AdHocStaInfo.StopBSSCompletionEvent);

        NdisZeroMemory(&timerChar, sizeof(NDIS_TIMER_CHARACTERISTICS));
        
        timerChar.Header.Type = NDIS_OBJECT_TYPE_TIMER_CHARACTERISTICS;
        timerChar.Header.Revision = NDIS_TIMER_CHARACTERISTICS_REVISION_1;
        timerChar.Header.Size = sizeof(NDIS_TIMER_CHARACTERISTICS);
        timerChar.AllocationTag = EXTSTA_MEMORY_TAG;
        
        timerChar.TimerFunction = StaAdHocWatchdogTimerRoutine;
        timerChar.FunctionContext = pStation;

        ndisStatus = NdisAllocateTimerObject(
                        STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
                        &timerChar,
                        &pStation->AdHocStaInfo.WatchdogTimer
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate adhoc watchdog timer\n"));
            break;
        }

        pStation->AdHocStaInfo.TimerCounter = 0;
        pStation->AdHocStaInfo.AsyncFuncCount = 0;
        
    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (pStation->AdHocStaInfo.WatchdogTimer)
        {
            NdisFreeTimerObject(pStation->AdHocStaInfo.WatchdogTimer);
            pStation->AdHocStaInfo.WatchdogTimer = NULL;
        }

        if(pStation->AdHocStaInfo.ConnectWorkItem)
        {
            NdisFreeIoWorkItem(pStation->AdHocStaInfo.ConnectWorkItem);
            pStation->AdHocStaInfo.ConnectWorkItem = NULL;
        }

        if (lockAllocated)
        {
            MP_FREE_READ_WRITE_LOCK(&pStation->AdHocStaInfo.StaListLock);
        }
    }
    
    return ndisStatus;
}

VOID
StaFreeAdHocStaInfo(
    _In_  PMP_EXTSTA_PORT        pStation
    )
{
    if (pStation->AdHocStaInfo.WatchdogTimer)
    {
        NdisFreeTimerObject(pStation->AdHocStaInfo.WatchdogTimer);
        pStation->AdHocStaInfo.WatchdogTimer = NULL;
    }

    if(pStation->AdHocStaInfo.ConnectWorkItem)
    {
        NdisFreeIoWorkItem(pStation->AdHocStaInfo.ConnectWorkItem);
        pStation->AdHocStaInfo.ConnectWorkItem = NULL;
    }
    MP_FREE_READ_WRITE_LOCK(&pStation->AdHocStaInfo.StaListLock);
}

NDIS_STATUS 
StaSaveAdHocStaInfo(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_RX_MPDU        pFragment,
    _In_  PDOT11_BEACON_FRAME pDot11BeaconFrame,
    _In_  ULONG           InfoElemBlobSize
    )
{
    NDIS_STATUS             ndisStatus = NDIS_STATUS_SUCCESS;
    ULONGLONG               HostTimeStamp;
    MP_RW_LOCK_STATE              LockState;
    PSTA_ADHOC_STA_ENTRY    StaEntry = NULL;
    PDOT11_MGMT_HEADER      pMgmtPktHeader;
    PVOID                   pInfoElemBlob = NULL;
    PSTA_ADHOC_STA_INFO     AdHocStaInfo = &pStation->AdHocStaInfo;
    PLIST_ENTRY             pHead = NULL, pEntry = NULL;
    UCHAR                   channel;

    pMgmtPktHeader = (PDOT11_MGMT_HEADER)MP_RX_MPDU_DATA(pFragment);
    NdisGetCurrentSystemTime((PLARGE_INTEGER)&HostTimeStamp);

    __try 
    {

        //
        // Lock the entire list
        //

        MP_ACQUIRE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);

        //
        // Search if we already have the entry for the station on the list.
        //

        pHead = &AdHocStaInfo->StaList;
        pEntry = pHead->Flink;
        while (pEntry != pHead) 
        {
            StaEntry = CONTAINING_RECORD(pEntry, STA_ADHOC_STA_ENTRY, Link);
            if (MP_COMPARE_MAC_ADDRESS(StaEntry->MacAddress, pMgmtPktHeader->SA)) 
                break;

            pEntry = pEntry->Flink;
        }

        if (pEntry == pHead) 
        {
            if (AdHocStaInfo->StaCount >= pStation->RegInfo->AdhocStationMaxCount)
            {
                //
                // We have reached the limit on the number of adhoc networks we would
                // maintain state for. Dont add new ones. When the adhoc watchdog
                // runs, it would reduce this number
                //
                ndisStatus = NDIS_STATUS_RESOURCES;
                StaEntry = NULL;
                __leave;
            }
            
            //
            // Create a new entry for this station.
            //

            MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle, 
                               &StaEntry, 
                               sizeof(STA_ADHOC_STA_ENTRY), 
                               EXTSTA_MEMORY_TAG);
            if (StaEntry == NULL)
            {
                ndisStatus = NDIS_STATUS_RESOURCES;
                __leave;
            }

            //
            // Initialize the new entry
            //

            StaEntry->AllocatedIEBlobSize = 0;
            StaEntry->InfoElemBlobPtr = NULL;           
            StaEntry->InfoElemBlobSize = 0;
            StaEntry->PhyId = STA_DESIRED_PHY_MAX_COUNT;        // set to an invalid Phy ID
            StaEntry->AssocState = dot11_assoc_state_unauth_unassoc;
            NdisMoveMemory(StaEntry->MacAddress, pMgmtPktHeader->SA, sizeof(DOT11_MAC_ADDRESS));
        }

        //
        // Update the information
        //
        if (StaEntry == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            __leave;
        }

        NdisMoveMemory(StaEntry->Dot11BSSID, pMgmtPktHeader->BSSID, sizeof(DOT11_MAC_ADDRESS));

        StaEntry->HostTimestamp = HostTimeStamp;
        StaEntry->BeaconTimestamp = pDot11BeaconFrame->Timestamp;
        StaEntry->BeaconInterval = pDot11BeaconFrame->BeaconInterval;
        StaEntry->Dot11Capability = pDot11BeaconFrame->Capability;
        StaEntry->ProbeRequestsSent = 0;

        //
        // Get channel number at which the frame was received.
        //
        if (Dot11GetChannelForDSPhy((PUCHAR)&pDot11BeaconFrame->InfoElements,
                                    InfoElemBlobSize, 
                                    &channel) != NDIS_STATUS_SUCCESS)
        {
            channel = pFragment->Msdu->Channel;
        }

        if (channel != 0)
        {
            StaEntry->Channel = channel;
        }

        //
        // Increase the information blob size if necessary
        //
        if (StaEntry->AllocatedIEBlobSize < InfoElemBlobSize)
        {
            MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle, 
                               &pInfoElemBlob, 
                               InfoElemBlobSize, 
                               EXTSTA_MEMORY_TAG);
            if (pInfoElemBlob == NULL)
            {
                ndisStatus = NDIS_STATUS_RESOURCES;
                __leave;
            }
                
            //
            // Delete any old blob buffer
            //
            if (StaEntry->InfoElemBlobPtr)
            {
                MP_FREE_MEMORY(StaEntry->InfoElemBlobPtr);   
            }

            StaEntry->InfoElemBlobPtr = pInfoElemBlob;
            StaEntry->AllocatedIEBlobSize = InfoElemBlobSize;
        }
        
        StaEntry->InfoElemBlobSize = InfoElemBlobSize;

        //
        // Update/Save the information element block
        //
        NdisMoveMemory(StaEntry->InfoElemBlobPtr, &pDot11BeaconFrame->InfoElements, InfoElemBlobSize);

        //
        // Add the new adhoc station to our list
        //
        if (pEntry == pHead) 
        {
            InsertTailList(pHead, &StaEntry->Link);
            AdHocStaInfo->StaCount++;
        }

        //
        // Indicate the possible association/disassociation for this StaEntry 
        //

        StaAdHocIndicateAssociation(pStation, StaEntry);
    }
    __finally 
    {
        MP_RELEASE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);

        if (ndisStatus != NDIS_STATUS_SUCCESS) 
        {
            if (pEntry == pHead && StaEntry) 
            {
                if (StaEntry->InfoElemBlobPtr) 
                {
                    MP_FREE_MEMORY(StaEntry->InfoElemBlobPtr);
                }
                
                MP_FREE_MEMORY(StaEntry);
            }
        }
    }

    return ndisStatus;
}

NDIS_STATUS
StaResetAdHocStaInfo(
    _In_  PMP_EXTSTA_PORT       pStation,
    _In_  BOOLEAN        flushStaList
    )
{
    MP_RW_LOCK_STATE              LockState;
    PSTA_ADHOC_STA_ENTRY    StaEntry;
    PSTA_ADHOC_STA_INFO     AdHocStaInfo = &pStation->AdHocStaInfo;
    PLIST_ENTRY             pEntry = NULL;
    BOOLEAN                 cancelled;
    BOOLEAN                 connected;
    BOOLEAN                 connecting;
    DOT11_DISASSOCIATION_PARAMETERS     disassocParam;

    NdisAcquireSpinLock(&pStation->AdHocStaInfo.StaInfoLock);
    
    connected = (BOOLEAN)(pStation->AdHocStaInfo.AdHocState == STA_ADHOC_CONNECTED);
    connecting = (BOOLEAN)(pStation->AdHocStaInfo.AdHocState & STA_ADHOC_CONNECTION_IN_PROGRESS);
    
    pStation->AdHocStaInfo.AdHocState |= STA_ADHOC_RESET_PENDING;

    //
    // If we got a reset while adhoc connect is pending (possible for NdisReset)
    // wait for the adhoc thread to finish
    //
    if (connecting)
    {
        NdisReleaseSpinLock(&pStation->AdHocStaInfo.StaInfoLock);

        //
        // We loop waiting for the connect to finish
        //
        while ((pStation->AdHocStaInfo.AdHocState & STA_ADHOC_CONNECTION_IN_PROGRESS) != 0)
        {
            NdisMSleep(10000);
        }
        
        NdisAcquireSpinLock(&pStation->AdHocStaInfo.StaInfoLock);
    }


    //
    // If we are currently connected, cancel the AdHoc watchdog timer. 
    // If we failed to cancel the timer, wait till the timer counter goes to 0.
    //
    if (connected)
    {
        cancelled = NdisCancelTimerObject(AdHocStaInfo->WatchdogTimer);
        if (!cancelled) 
        {
            while (AdHocStaInfo->TimerCounter != 0) 
            {
                NdisStallExecution(50);      // 50 us
            }
        }
        else
        {
            NdisInterlockedDecrement(&AdHocStaInfo->TimerCounter);
        }
    }

    NdisReleaseSpinLock(&pStation->AdHocStaInfo.StaInfoLock);

    //
    // Free everything on the list if asked to do so.
    //
    if (flushStaList)
    {
        MP_ACQUIRE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);

        while (!IsListEmpty(&AdHocStaInfo->StaList)) 
        {
            pEntry = RemoveHeadList(&AdHocStaInfo->StaList);
            StaEntry = CONTAINING_RECORD(pEntry, STA_ADHOC_STA_ENTRY, Link);
            
            if (StaEntry->InfoElemBlobPtr) 
            {
                MP_FREE_MEMORY(StaEntry->InfoElemBlobPtr);
            }
            
            MP_FREE_MEMORY(StaEntry);
        }

        AdHocStaInfo->StaCount = 0;
        AdHocStaInfo->DeauthStaCount = 0;
        MP_RELEASE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);
    }

    //
    // Stop beaconing
    //
    if (connected)
    {
        StaStopAdHocBeaconing(pStation);
    }
    
    //
    // Also notify hw about disconnected status
    //
    MP_ASSIGN_NDIS_OBJECT_HEADER(disassocParam.Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_DISASSOCIATION_PARAMETERS_REVISION_1,
                                 sizeof(DOT11_DISASSOCIATION_PARAMETERS));
    disassocParam.uReason = DOT11_DISASSOC_REASON_OS;
    NdisFillMemory(disassocParam.MacAddr, sizeof(DOT11_MAC_ADDRESS), 0xff);

    VNic11NotifyConnectionStatus(
        STA_GET_VNIC(pStation), 
        FALSE, 
        NDIS_STATUS_DOT11_DISASSOCIATION,
        &disassocParam, 
        sizeof(DOT11_DISASSOCIATION_PARAMETERS)
        );    

    //
    // Reset state
    //
    NdisAcquireSpinLock(&pStation->AdHocStaInfo.StaInfoLock);  
    pStation->AdHocStaInfo.AdHocState = STA_ADHOC_DISCONNECTED;
    NdisReleaseSpinLock(&pStation->AdHocStaInfo.StaInfoLock);

    return NDIS_STATUS_SUCCESS;
}

void
StaClearStaListAssocState(
    _In_  PMP_EXTSTA_PORT    pStation,
    _In_  BOOLEAN     SendDeauth
    )
{
    MP_RW_LOCK_STATE              LockState;
    PSTA_ADHOC_STA_ENTRY    StaEntry = NULL;
    PSTA_ADHOC_STA_INFO     AdHocStaInfo = &pStation->AdHocStaInfo;
    PLIST_ENTRY             pHead = NULL, pEntry = NULL;
    UCHAR                   Buffer[sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_DEAUTH_FRAME)];
    PDOT11_MGMT_HEADER      MgmtPacket = (PDOT11_MGMT_HEADER)Buffer;
    PDOT11_DEAUTH_FRAME     DeauthFrame;

    //
    // Initialize the de-auth message if we are to send a de-auth message
    //
    if (SendDeauth)
    {
        MgmtPacket->FrameControl.Version = 0x0;
        MgmtPacket->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
        MgmtPacket->FrameControl.Subtype = DOT11_MGMT_SUBTYPE_DEAUTHENTICATION;
        MgmtPacket->FrameControl.ToDS = 0x0;      // Default value for Mgmt frames
        MgmtPacket->FrameControl.FromDS = 0x0;    // Default value for Mgmt frames
        MgmtPacket->FrameControl.MoreFrag = 0x0;  
        MgmtPacket->FrameControl.Retry = 0x0;
        MgmtPacket->FrameControl.PwrMgt = 0x0;
        MgmtPacket->FrameControl.MoreData = 0x0;
        MgmtPacket->FrameControl.WEP = 0x0;       // no WEP
        MgmtPacket->FrameControl.Order = 0x0;     // no order
        MgmtPacket->SequenceControl.usValue = 0;

        NdisMoveMemory(MgmtPacket->SA, 
                       VNic11QueryMACAddress(STA_GET_VNIC(pStation)),
                       DOT11_ADDRESS_SIZE);

        NdisMoveMemory(MgmtPacket->BSSID,
                       VNic11QueryCurrentBSSID(STA_GET_VNIC(pStation)),
                       DOT11_ADDRESS_SIZE);

        DeauthFrame = (PDOT11_DEAUTH_FRAME)Add2Ptr(MgmtPacket, sizeof(DOT11_MGMT_HEADER));
        DeauthFrame->ReasonCode = DOT11_MGMT_REASON_DEAUTH_LEAVE_SS;
    }
    
    MP_ACQUIRE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);

    pHead = &AdHocStaInfo->StaList;
    pEntry = pHead->Flink;
    while (pEntry != pHead) 
    {
        StaEntry = CONTAINING_RECORD(pEntry, STA_ADHOC_STA_ENTRY, Link);
        pEntry = pEntry->Flink;

        //
        // If we are to send de-auth messages, send one to each associated station.
        //
        if (SendDeauth && StaEntry->AssocState == dot11_assoc_state_auth_assoc)
        {
            NdisMoveMemory(MgmtPacket->DA, 
                           StaEntry->MacAddress,
                           DOT11_ADDRESS_SIZE);

            BasePortSendInternalPacket(STA_GET_MP_PORT(pStation), 
                                Buffer, 
                               sizeof(Buffer));

            MpTrace(COMP_ASSOC, DBG_SERIOUS, 
                ("Sent deauth packet to Ad Hoc station: %02X-%02X-%02X-%02X-%02X-%02X\n", 
                StaEntry->MacAddress[0], StaEntry->MacAddress[1], StaEntry->MacAddress[2], 
                StaEntry->MacAddress[3], StaEntry->MacAddress[4], StaEntry->MacAddress[5]));
        }

        StaEntry->AssocState = dot11_assoc_state_unauth_unassoc;
    }

    AdHocStaInfo->DeauthStaCount = 0;
    
    MP_RELEASE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);
}

BOOLEAN
StaAcceptStation(
    _In_  PMP_EXTSTA_PORT pStation,
    _In_  PSTA_ADHOC_STA_ENTRY StaEntry
    )
{
    ULONG           index;
    DOT11_PHY_TYPE  PhyType;
    DOT11_RATE_SET  rateSet = {0};
    NDIS_STATUS     ndisStatus;
    UCHAR           SecIELength;
    PUCHAR          SecIEData;
    RSN_IE_INFO     RSNIEInfo;

    //
    // This function determines if we could accept a station on the same
    // ad hoc network as ours based on its attributes other than SSID/BSSID.
    //

    //
    // Check if the StaEntry is on the exclused MAC list
    //

    for (index = 0; index < pStation->Config.ExcludedMACAddressCount; index++) 
    {
        if (NdisEqualMemory(StaEntry->MacAddress,
                            pStation->Config.ExcludedMACAddressList[index],
                            sizeof(DOT11_MAC_ADDRESS)) == 1)
            break;
    }

    if (index < pStation->Config.ExcludedMACAddressCount)
        return FALSE;

    //
    // Check if the StaEntry matches the desired PHY list.
    //

    PhyType = VNic11DeterminePHYType(STA_GET_VNIC(pStation), 
                                   StaEntry->Dot11Capability,
                                   StaEntry->Channel);

    StaEntry->PhyId = StaGetPhyIdByType(pStation, PhyType);
    if (!StaMatchPhyId(pStation, StaEntry->PhyId))
        return FALSE;

    //
    // Check if all basic data rates are supported.
    //

    ndisStatus = Dot11GetRateSetFromInfoEle(StaEntry->InfoElemBlobPtr,
                                          StaEntry->InfoElemBlobSize,
                                          TRUE, 
                                          &rateSet);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
        return FALSE;

    ndisStatus = VNic11ValidateOperationalRates(STA_GET_VNIC(pStation),
                                              StaEntry->PhyId,
                                              rateSet.ucRateSet,
                                              rateSet.uRateSetLength);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
        return FALSE;

    //
    // Check for Privacy attribute.
    //

    if (StaEntry->Dot11Capability.Privacy && pStation->Config.UnicastCipherAlgorithm == DOT11_CIPHER_ALGO_NONE)
        return FALSE;

    //
    // Check RSNA IE if our auth algo is RSNA_PSK
    //
    if (pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK)
    {
        ndisStatus = Dot11GetInfoEle(StaEntry->InfoElemBlobPtr,
                                     StaEntry->InfoElemBlobSize,
                                     DOT11_INFO_ELEMENT_ID_RSN,
                                     &SecIELength,
                                     (PVOID)&SecIEData);

        if (ndisStatus != NDIS_STATUS_SUCCESS)
            return FALSE;

        ndisStatus = Dot11ParseRNSIE(SecIEData, RSNA_OUI_PREFIX, SecIELength, &RSNIEInfo);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            return FALSE;

        //
        // Check if the station is running RNSA with AKM and ciphers that meet our requirement. 
        //
        if (!StaMatchRSNInfo(pStation, &RSNIEInfo))
            return FALSE;

        //
        // Save station's group cipher
        //
        StaEntry->GroupCipher = Dot11GetGroupCipherFromRSNIEInfo(&RSNIEInfo);
    }

    return TRUE;
}


NDIS_STATUS
StaAdhocJoinCompletionHandler(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   Data
    )
{
    PMP_EXTSTA_PORT             pStation = MP_GET_STA_PORT(Port);
    
    // Set the join completion event
    pStation->AdHocStaInfo.JoinCompletionStatus = *((PNDIS_STATUS)Data);
    NdisSetEvent(&pStation->AdHocStaInfo.JoinCompletionEvent);

    // Release the context switch barrier
    VNic11ReleaseCtxSBarrier(STA_GET_VNIC(pStation));

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
StaAdhocJoinChannelSwitchCompletionHandler(
    _In_ PMP_PORT     Port,
    _In_ PVOID        Data
    )
{
    PMP_EXTSTA_PORT        pStation = MP_GET_STA_PORT(Port);
    NDIS_STATUS            ndisStatus = *((PNDIS_STATUS)Data);

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        VNic11AcquireCtxSBarrier(STA_GET_VNIC(pStation));
        
        // Call Join on the H/W

        //
        // Hardware function handles timer synchronization with this
        // access point
        //
        ndisStatus = VNic11JoinBSS(STA_GET_VNIC(pStation), 
                        pStation->AdHocStaInfo.JoinBSSDescription, 
                        STA11_ADHOC_JOIN_TIMEOUT, 
                        StaAdhocJoinCompletionHandler
                        );
        if (ndisStatus != NDIS_STATUS_PENDING)
        {
            // Failed (Join cannot succeed synchronously)
            VNic11ReleaseCtxSBarrier(STA_GET_VNIC(pStation));
        }
        else
        {
            // This translates to status success
            ndisStatus = NDIS_STATUS_SUCCESS;
        }
    }

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        pStation->AdHocStaInfo.JoinCompletionStatus = *((PNDIS_STATUS)Data);
        NdisSetEvent(&pStation->AdHocStaInfo.JoinCompletionEvent);
    }

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
StaJoinAdHoc (
    _In_  PMP_EXTSTA_PORT pStation,
    _In_  PSTA_ADHOC_STA_ENTRY StaEntry
    )
{
    PMP_BSS_DESCRIPTION     BSSDescription = NULL;
    NDIS_STATUS             ndisStatus = NDIS_STATUS_SUCCESS;

    __try 
    {
        //
        // Notify the H/W about connection start
        //
        StaAdhocIndicateConnectionStart(pStation, StaEntry->Dot11BSSID, TRUE);

        //
        // Allocate a BSS description structure (we allocate 3 bytes more for possible addition of
        // DS parameter IE, that contains channel number).
        //
        
        // Integer overflow
        if ((FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + StaEntry->InfoElemBlobSize) > 
            (FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + StaEntry->InfoElemBlobSize + 3))
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            __leave;
        }

        MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
                           &BSSDescription, 
                           FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + StaEntry->InfoElemBlobSize + 3,
                           MP_MEMORY_TAG);
        if (BSSDescription == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            __leave;
        }
        
        //
        // Initialize the BSS description structure from StaEntry
        //
        NdisMoveMemory(BSSDescription->BSSID, StaEntry->Dot11BSSID, sizeof(DOT11_MAC_ADDRESS));
        BSSDescription->BSSType = dot11_BSS_type_independent;
        BSSDescription->BeaconPeriod = StaEntry->BeaconInterval;
        BSSDescription->Timestamp = StaEntry->BeaconTimestamp;
        BSSDescription->Capability.usValue = StaEntry->Dot11Capability.usValue;
        NdisMoveMemory(BSSDescription->IEBlobs, StaEntry->InfoElemBlobPtr, StaEntry->InfoElemBlobSize);
        // Use the same buffers for beacon and probe responses
        BSSDescription->BeaconIEBlobOffset = 0;
        BSSDescription->BeaconIEBlobSize = StaEntry->InfoElemBlobSize;
        BSSDescription->ProbeIEBlobOffset = 0;
        BSSDescription->ProbeIEBlobSize = StaEntry->InfoElemBlobSize;
        BSSDescription->IEBlobsSize = StaEntry->InfoElemBlobSize;
        
        // Specify the Phy ID, channel to use
        BSSDescription->Channel = StaEntry->Channel;
        BSSDescription->PhyId = StaEntry->PhyId;
            
        //
        // Before we are doing the sync join, which could take a while, check if reset
        // is pending. If so, we quit.
        //

        NdisAcquireSpinLock(&pStation->AdHocStaInfo.StaInfoLock);

        if (pStation->AdHocStaInfo.AdHocState & STA_ADHOC_RESET_PENDING)
            ndisStatus = NDIS_STATUS_REQUEST_ABORTED;

        NdisReleaseSpinLock(&pStation->AdHocStaInfo.StaInfoLock);
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            __leave;

        //
        // Reset an event for signalling the join completion. Then call hardware
        // interface function to perform join request. Unlike infrastructure,
        // we wait synchronously for the join to complete. 
        //
        NdisResetEvent(&pStation->AdHocStaInfo.JoinCompletionEvent);
        pStation->AdHocStaInfo.JoinBSSDescription = BSSDescription;

        // Set the channel & PHY ID before we do the join
        ndisStatus = VNic11SetChannel(STA_GET_VNIC(pStation), 
            BSSDescription->Channel, 
            BSSDescription->PhyId, 
            TRUE,
            StaAdhocJoinChannelSwitchCompletionHandler
            );
        if (ndisStatus == NDIS_STATUS_PENDING)
        {
            // Hardware will start the channel switch and call us back
            // when the switch completes & then we would do a join
        
            // This is success for the channel switch attempt        
            ndisStatus = NDIS_STATUS_SUCCESS;
        }
        else if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            // Start the Join
            ndisStatus = StaAdhocJoinChannelSwitchCompletionHandler(STA_GET_MP_PORT(pStation), &ndisStatus);
        }
        else
        {
            // Failure
            __leave;
        }

        //
        // Wait for the event that signals join completion
        //
        NdisWaitEvent(&pStation->AdHocStaInfo.JoinCompletionEvent, 0);
        ndisStatus = pStation->AdHocStaInfo.JoinCompletionStatus;
    }
    __finally 
    {    
        // Failed, status to the H/W
        StaAdhocIndicateConnectionCompletion(pStation, ndisStatus, TRUE);
        
        if (BSSDescription)
        {
            MP_FREE_MEMORY(BSSDescription);
        }
    }

    return ndisStatus;
}

NDIS_STATUS
Sta11StartBSSCompleteCallback(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   Data
    )
{
    PMP_EXTSTA_PORT             pStation = MP_GET_STA_PORT(Port);
    
    // Set the start completion event
    pStation->AdHocStaInfo.StartBSSCompletionStatus = *((PNDIS_STATUS)Data);
    NdisSetEvent(&pStation->AdHocStaInfo.StartBSSCompletionEvent);

    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
StaStartAdHoc(
    _In_  PMP_EXTSTA_PORT pStation,
    _In_opt_  PSTA_ADHOC_STA_ENTRY StaEntry,
    _In_reads_bytes_opt_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS BSSID
    )
/*++

Routine Description:

    Start AdHoc mode

    The caller should guarantee serialization!!!

Arguments:

    pStation -  The pStation on which the Ad Hoc mode should be started

    StaEntry -  One of the existing pStations in the Ad Hoc network, or
                NULL if it is a new network.

    BSSID -     BSSID of the ad hoc network we are starting when StaEntry is NULL.

Return Value:

--*/
{
    PMP_BSS_DESCRIPTION     BSSDescription = NULL;
    DOT11_RATE_SET          rateSet = {0,0};
    ULONG                   index;
    NDIS_STATUS             ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC                   pNic = STA_GET_VNIC(pStation);
    UCHAR                   channel;
    PUCHAR                  tmpPtr;
    UCHAR                   size;
    USHORT                  ATIMWindow;
    DOT11_CAPABILITY        dot11Capability = {0};
    STA_FHSS_IE             FHSSIE = {0};
    BOOLEAN                 IEPresent;
    DOT11_SSID              dot11SSID;
    PUCHAR                  infoBlobPtr;
    USHORT                  infoBlobSize;
    DOT11_PHY_TYPE          PhyType;
    BOOLEAN                 set;

    __try 
    {
        //
        // Notify the H/W about connection start
        //
        if (StaEntry) 
        {
            StaAdhocIndicateConnectionStart(pStation, StaEntry->Dot11BSSID, TRUE);
        }
        else
        {
            StaAdhocIndicateConnectionStart(pStation, BSSID, TRUE);
        }
        
        //
        // Allocate a BSS description structure with maximum possible IE field.
        //

        MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
                           &BSSDescription, 
                           FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + STA11_MAX_IE_BLOB_SIZE,
                           MP_MEMORY_TAG);
        if (BSSDescription == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            __leave;
        }
        NdisZeroMemory(BSSDescription, FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + STA11_MAX_IE_BLOB_SIZE);

        //
        // Initialize the BSS description structure from StaEntry or our own pStation information.
        //

        BSSDescription->BSSType = dot11_BSS_type_independent;
        BSSDescription->Timestamp = 0;

        if (StaEntry) 
        {
            NdisMoveMemory(BSSDescription->BSSID, StaEntry->Dot11BSSID, sizeof(DOT11_MAC_ADDRESS));
            BSSDescription->BeaconPeriod = StaEntry->BeaconInterval;
            BSSDescription->Timestamp = StaEntry->BeaconTimestamp;
        }
        else 
        {
            NdisMoveMemory(BSSDescription->BSSID, BSSID, sizeof(DOT11_MAC_ADDRESS));
            BSSDescription->BeaconPeriod = (USHORT) VNic11QueryBeaconPeriod(pNic);
            BSSDescription->Timestamp = 0;            
        }

        NdisMoveMemory(BSSDescription->MacAddress, VNic11QueryMACAddress(pNic), sizeof(DOT11_MAC_ADDRESS));

        //
        // Fill the capabilityInformation 
        // 

        dot11Capability.ESS = 0;
        dot11Capability.IBSS = 1;
        dot11Capability.CFPollable = 0;     // CFPollable is always unavailable in AdHoc mode
        dot11Capability.CFPollRequest = 0;
        
        if (StaEntry)
        {
            dot11Capability.Privacy = StaEntry->Dot11Capability.Privacy;
        }
        else
        {
            dot11Capability.Privacy = (pStation->Config.UnicastCipherAlgorithm != DOT11_CIPHER_ALGO_NONE) ? 1 : 0;
        }

        PhyType = VNic11QueryCurrentPhyType(pNic);
        switch (PhyType) 
        {
            case dot11_phy_type_erp:
                set = VNic11QueryShortSlotTimeOptionImplemented(pNic, FALSE);
                if (set)
                {
                    set = VNic11QueryShortSlotTimeOptionEnabled(pNic, FALSE);
                }
                dot11Capability.ShortSlotTime = set ? 1 : 0;

                set = VNic11QueryDsssOfdmOptionImplemented(pNic, FALSE);
                if (set)
                {
                    set = VNic11QueryDsssOfdmOptionEnabled(pNic, FALSE);
                }
                dot11Capability.DSSSOFDM = set ? 1 : 0;

                // fall through

            case dot11_phy_type_hrdsss:
                set = VNic11QueryShortPreambleOptionImplemented(pNic, FALSE);
                dot11Capability.ShortPreamble = set ? 1: 0;

                set = VNic11QueryPbccOptionImplemented(pNic, FALSE);
                dot11Capability.PBCC = set ? 1: 0;

                set = VNic11QueryChannelAgilityPresent(pNic, FALSE);
                if (set)
                {
                    set = VNic11QueryChannelAgilityEnabled(pNic, FALSE);
                }
                dot11Capability.ChannelAgility = set ? 1 : 0;
        }

        BSSDescription->Capability.usValue = dot11Capability.usValue;  // usValue is initialized to 0

        //
        // Set the starting address and size of the beacon blob.
        //
        BSSDescription->BeaconIEBlobOffset = 0;
       
        infoBlobPtr = &BSSDescription->IEBlobs[BSSDescription->BeaconIEBlobOffset];
        infoBlobSize = STA11_MAX_IE_BLOB_SIZE;

        //
        // Add SSID.
        //

        if (StaEntry) 
        {
            ndisStatus = Dot11GetInfoEle(StaEntry->InfoElemBlobPtr,
                                         StaEntry->InfoElemBlobSize,
                                         DOT11_INFO_ELEMENT_ID_SSID,
                                         &size,
                                         &tmpPtr);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
                __leave;

            // extra check for prefast
            if (size > DOT11_SSID_MAX_LENGTH)
            {
                ndisStatus = NDIS_STATUS_INVALID_LENGTH;
                __leave;
            }

            dot11SSID.uSSIDLength = size;
            NdisMoveMemory(dot11SSID.ucSSID, tmpPtr, dot11SSID.uSSIDLength);
        } 
        else
        {

            //
            // Use our desired SSID.
            //

            dot11SSID = pStation->Config.SSID;
        }

        ndisStatus = Dot11AttachElement(&infoBlobPtr,
                                        &infoBlobSize,
                                        DOT11_INFO_ELEMENT_ID_SSID,
                                        (UCHAR)(dot11SSID.uSSIDLength),
                                        dot11SSID.ucSSID);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            __leave;

        //
        // Add basic rate set.
        //

        if (StaEntry) 
        {
            ndisStatus = Dot11GetRateSetFromInfoEle(StaEntry->InfoElemBlobPtr,
                                                  StaEntry->InfoElemBlobSize,
                                                  TRUE, 
                                                  &rateSet);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
                __leave;
        }
        else 
        {
            VNic11QueryBasicRateSet(STA_GET_VNIC(pStation), &rateSet, FALSE);
            for (index = 0; index < rateSet.uRateSetLength; index++)
            {
                rateSet.ucRateSet[index] |= 0x80;
            }
        }

        ndisStatus = Dot11AttachElement(&infoBlobPtr,
                                        &infoBlobSize,
                                        DOT11_INFO_ELEMENT_ID_SUPPORTED_RATES,
                                        (UCHAR)((rateSet.uRateSetLength > 8) ? 8 : rateSet.uRateSetLength),
                                        rateSet.ucRateSet);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            __leave;
        
        if (rateSet.uRateSetLength > (UCHAR)8) 
        {
            ndisStatus = Dot11AttachElement(&infoBlobPtr,
                                            &infoBlobSize,
                                            DOT11_INFO_ELEMENT_ID_EXTD_SUPPORTED_RATES,
                                            (UCHAR)(rateSet.uRateSetLength - 8),
                                            rateSet.ucRateSet + 8);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
                __leave;
        }
    
        //
        // Add PHY specific IEs
        //
        switch (PhyType)
        {
            case dot11_phy_type_erp:
            case dot11_phy_type_hrdsss:
            case dot11_phy_type_dsss:

                //
                // Attach DSSS IE
                //
                
                if (StaEntry) 
                {
                    ndisStatus = Dot11CopyInfoEle(StaEntry->InfoElemBlobPtr,
                                                  StaEntry->InfoElemBlobSize,
                                                  DOT11_INFO_ELEMENT_ID_DS_PARAM_SET,
                                                  &size,
                                                  sizeof(channel),
                                                  &channel);
 
                    IEPresent = (ndisStatus == NDIS_STATUS_SUCCESS && size == sizeof(channel)) ? TRUE : FALSE;
                } 
                else 
                {
                    channel = (UCHAR)VNic11QueryCurrentChannel(pNic, FALSE);
                    IEPresent = TRUE;
                }

                if (IEPresent)
                {
                    ndisStatus = Dot11AttachElement(&infoBlobPtr,
                                                    &infoBlobSize,
                                                    DOT11_INFO_ELEMENT_ID_DS_PARAM_SET,
                                                    sizeof(channel),
                                                    &channel);
                    if (ndisStatus != NDIS_STATUS_SUCCESS)
                        __leave;
                }

                break;

            case dot11_phy_type_fhss:

                //
                // Attach FHSS IE
                //
                
                if (StaEntry) 
                {
                    ndisStatus = Dot11CopyInfoEle(StaEntry->InfoElemBlobPtr,
                                                  StaEntry->InfoElemBlobSize,
                                                  DOT11_INFO_ELEMENT_ID_FH_PARAM_SET,
                                                  &size,
                                                  sizeof(FHSSIE),
                                                  &FHSSIE);
 
                    IEPresent = (ndisStatus == NDIS_STATUS_SUCCESS && size == sizeof(FHSSIE)) ? TRUE: FALSE;
                } 
                else 
                {
                    IEPresent = FALSE;
                }

                if (IEPresent) 
                {
                    ndisStatus = Dot11AttachElement(&infoBlobPtr,
                                                    &infoBlobSize,
                                                    DOT11_INFO_ELEMENT_ID_FH_PARAM_SET,
                                                    sizeof(FHSSIE),
                                                    (PVOID)&FHSSIE);
                    if (ndisStatus != NDIS_STATUS_SUCCESS)
                        __leave;
                }
      
                break;

            case dot11_phy_type_ofdm:
                break;

            case dot11_phy_type_irbaseband:
                break;

            default:
                break;
        }

        BSSDescription->PhyId = StaGetPhyIdByType(pStation, PhyType);
        //
        // Update ATIM window
        //

        if (StaEntry) 
        {
            ndisStatus = Dot11CopyInfoEle(StaEntry->InfoElemBlobPtr,
                                          StaEntry->InfoElemBlobSize,
                                          DOT11_INFO_ELEMENT_ID_IBSS_PARAM_SET,
                                          &size,
                                          sizeof(ATIMWindow),
                                          &ATIMWindow);

            if (ndisStatus != NDIS_STATUS_SUCCESS || size != sizeof(ATIMWindow)) 
            {
                ATIMWindow = 0;
            }
        }
        else
        {
            ATIMWindow = 0;
        }

        ndisStatus = VNic11SetATIMWindow(pNic, (ULONG)ATIMWindow);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            __leave;

        // 
        // Add it into the IE
        //
        ndisStatus = Dot11AttachElement(&infoBlobPtr,
                                        &infoBlobSize,
                                        DOT11_INFO_ELEMENT_ID_IBSS_PARAM_SET,
                                        sizeof(ATIMWindow),
                                        (PVOID)&ATIMWindow);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            __leave;
            

        //
        // If we are running RSNA_PSK, add RSN IE.
        //
        if (pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK)
        {
            //
            // If we are joining an exsiting network, set our group cipher to the one used by
            // the existing network if our group cipher is not specified.
            //
            if (pStation->Config.MulticastCipherAlgorithmCount > 1)
            {
                if (StaEntry)
                {
                    //
                    // If we are joining an exsiting network, set our group cipher to the one used by
                    // the existing network.
                    //
                    pStation->Config.MulticastCipherAlgorithm = StaEntry->GroupCipher;
                }
                else
                {
                    //
                    // If we are creating our own network, set our group cipher to the first one 
                    // in the enabled multicast cipher list.
                    //
                    pStation->Config.MulticastCipherAlgorithm = pStation->Config.MulticastCipherAlgorithmList[0];
                }

                //
                // Tell hardware layer what group cipher we use.
                //
                VNic11SetCipher(STA_GET_VNIC(pStation), FALSE, pStation->Config.MulticastCipherAlgorithm);
            }

            ndisStatus = StaAttachAdHocRSNIE(pStation, 
                                             &infoBlobPtr,
                                             &infoBlobSize);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
                __leave;
        }

        //
        // Add any additional IEs set by IBSS parameter if we still have space.
        //

        if (pStation->Config.AdditionalIESize != 0 && pStation->Config.AdditionalIESize <= infoBlobSize)
        {
            NdisMoveMemory(infoBlobPtr, 
                           pStation->Config.AdditionalIEData,
                           pStation->Config.AdditionalIESize);
            infoBlobPtr += pStation->Config.AdditionalIESize;
            infoBlobSize = infoBlobSize - ((USHORT)pStation->Config.AdditionalIESize);
        }

        BSSDescription->BeaconIEBlobSize = (ULONG)PtrOffset(BSSDescription->IEBlobs, infoBlobPtr);

        //
        // Use the same buffer for the probe response IE blob
        //
        BSSDescription->ProbeIEBlobOffset = BSSDescription->BeaconIEBlobOffset;
        BSSDescription->ProbeIEBlobSize = BSSDescription->BeaconIEBlobSize;

        BSSDescription->IEBlobsSize = BSSDescription->BeaconIEBlobSize;

        // save the BSS description in the adhoc info
        pStation->AdHocStaInfo.BSSDescription = BSSDescription;
        BSSDescription = NULL; // so that the pointer does not get freed
        
        //
        // Start beaconing and receiving data frames.
        //
        ndisStatus = StaStartAdHocBeaconing(pStation);

        ASSERT(NDIS_STATUS_PENDING != ndisStatus);
        
        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            //
            // Set TX data rate and our active PhyId
            //

            VNic11SelectTXDataRate(pNic, &rateSet, 100);
            pStation->Config.ActivePhyId = StaGetPhyIdByType(pStation, PhyType);
        }
        else
        {
            // start beaconing failed. Free up the BSS description we allocated
            MP_FREE_MEMORY(pStation->AdHocStaInfo.BSSDescription);
            pStation->AdHocStaInfo.BSSDescription = NULL;
        }
    }
    __finally 
    {
        if (BSSDescription)
        {
            MP_FREE_MEMORY(BSSDescription);
        }
        StaAdhocIndicateConnectionCompletion(pStation, ndisStatus, TRUE);    
    }

    return ndisStatus;
}


NDIS_STATUS
Sta11StopBSSCompleteCallback(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   Data
    )
{
    PMP_EXTSTA_PORT             pStation = MP_GET_STA_PORT(Port);

    UNREFERENCED_PARAMETER(Data);
    
    // Set the stop completion event
    pStation->AdHocStaInfo.StopBSSCompletionStatus = *((PNDIS_STATUS)Data);
    NdisSetEvent(&pStation->AdHocStaInfo.StopBSSCompletionEvent);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
StaStartAdHocBeaconing(
    _In_  PMP_EXTSTA_PORT pStation
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC pVNic = STA_GET_VNIC(pStation);

    // we should not be already beaconing
    ASSERT(!pStation->AdHocStaInfo.fBeaconing);
    if (pStation->AdHocStaInfo.fBeaconing)
    {
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("AdHoc beaconing is already on. Simply returning \n"));
        return NDIS_STATUS_SUCCESS;
    }
    
    // we should already have the BSS description
    ASSERT(pStation->AdHocStaInfo.BSSDescription);
    if (!pStation->AdHocStaInfo.BSSDescription)
    {
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("No BSS description present. Simply returning \n"));
        return NDIS_STATUS_SUCCESS;
    }
    
    NdisResetEvent(&pStation->AdHocStaInfo.StartBSSCompletionEvent);

    // remember we have an item pending with the VNIC
    InterlockedIncrement(&pStation->AdHocStaInfo.AsyncFuncCount);
    
    ndisStatus = VNic11StartBSS(
                    pVNic, 
                    pStation->AdHocStaInfo.BSSDescription, 
                    Sta11StartBSSCompleteCallback
                    );
    
    if (ndisStatus == NDIS_STATUS_PENDING)
    {
        //
        // Wait for the event that signals start completion
        //
        NdisWaitEvent(&pStation->AdHocStaInfo.StartBSSCompletionEvent, 0);
        ndisStatus = pStation->AdHocStaInfo.StartBSSCompletionStatus;

        if (ndisStatus == NDIS_STATUS_RESET_IN_PROGRESS)
        {
            // The start BSS failed because of a reset. Convert to the
            // appropriate status that the caller understands
            ndisStatus = NDIS_STATUS_REQUEST_ABORTED;
        }
    }

    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        // the beaconing was started
        pStation->AdHocStaInfo.fBeaconing = TRUE;
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("AdHoc beaconing started \n"));
    }
    else
    {
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("AdHoc beaconing failed to start %!x!. Not setting the flag\n", ndisStatus));
    }

    // VNIC has completed our request.
    InterlockedDecrement(&pStation->AdHocStaInfo.AsyncFuncCount);
    
    return ndisStatus;
}

VOID
StaStopAdHocBeaconing(
    _In_  PMP_EXTSTA_PORT pStation
    )
{
    NDIS_STATUS             ndisStatus = NDIS_STATUS_SUCCESS;

    if (pStation->AdHocStaInfo.fBeaconing)
    {
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("AdHoc beaconing is on. Stopping it\n"));
        
        NdisResetEvent(&pStation->AdHocStaInfo.StartBSSCompletionEvent);

        // remember we have an item pending with the VNIC
        InterlockedIncrement(&pStation->AdHocStaInfo.AsyncFuncCount);
        
        ndisStatus = VNic11StopBSS(STA_GET_VNIC(pStation), Sta11StopBSSCompleteCallback);
        if (ndisStatus == NDIS_STATUS_PENDING)
        {
            //
            // Wait for the event that signals stop completion
            //
            NdisWaitEvent(&pStation->AdHocStaInfo.StopBSSCompletionEvent, 0);
            ndisStatus = pStation->AdHocStaInfo.StopBSSCompletionStatus;
        }

        // reset the flag
        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            MpTrace(COMP_ASSOC, DBG_NORMAL, ("AdHoc beaconing stopped \n"));
            pStation->AdHocStaInfo.fBeaconing = FALSE;
        }
        else
        {
            // BUGBUG: Is not resetting the flag the right thing to do???
            MpTrace(COMP_ASSOC, DBG_NORMAL, ("AdHoc beaconing failed to stop %!x!. Not resetting the flag\n", ndisStatus));
            pStation->AdHocStaInfo.fBeaconing = FALSE;
        }

        // VNIC has completed our request.
        InterlockedDecrement(&pStation->AdHocStaInfo.AsyncFuncCount);
    }
    else
    {
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("AdHoc beaconing is not on. Not doing anything\n"));
    }
}

NDIS_STATUS
StaGetMatchingAdHocStaList(
    _In_  PMP_EXTSTA_PORT pStation,
    _Out_ PULONG StaCount,
    _Outptr_result_buffer_maybenull_(*StaCount) PSTA_ADHOC_STA_ENTRY **StaEntryArray
    )
{
    NDIS_STATUS             ndisStatus = NDIS_STATUS_SUCCESS;
    PLIST_ENTRY             pHead = NULL, pEntry = NULL;
    PSTA_ADHOC_STA_ENTRY    StaEntry = NULL;
    MP_RW_LOCK_STATE              LockState;
    UCHAR                   size;
    PUCHAR                  tmpPtr;
    ULONG                   index;
    PDOT11_SSID             SSID;

    *StaEntryArray = NULL;
    *StaCount = 0;

    __try 
    {

        //
        // Only need read access to the list
        //
        
        MP_ACQUIRE_READ_LOCK(&pStation->AdHocStaInfo.StaListLock, &LockState);

        if (pStation->AdHocStaInfo.StaCount == 0 || pStation->Config.IgnoreAllMACAddresses)
            __leave;

        MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
                           StaEntryArray,
                           sizeof(PSTA_ADHOC_STA_ENTRY) * pStation->AdHocStaInfo.StaCount, 
                           MP_MEMORY_TAG);          
        if ((*StaEntryArray) == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            __leave;
        }

        //
        // Go through the adhoc station list and find all the matching stations and 
        // add them to the StaEntry array.
        //

        pHead = &pStation->AdHocStaInfo.StaList;
        pEntry = pHead->Flink;
        while (pEntry != pHead) 
        {

            StaEntry = CONTAINING_RECORD(pEntry, STA_ADHOC_STA_ENTRY, Link);
            pEntry = pEntry->Flink;

            
            //
            // Check if the StaEntry matches one of the BSSID in the BSSID list.
            //

            if (!pStation->Config.AcceptAnyBSSID) 
            {
                for (index = 0; index < pStation->Config.DesiredBSSIDCount; index++)
                {
                    if (NdisEqualMemory(pStation->Config.DesiredBSSIDList[index], 
                                        StaEntry->Dot11BSSID, 
                                        sizeof(DOT11_MAC_ADDRESS)) == 1)
                        break;
                }

                if (index == pStation->Config.DesiredBSSIDCount)
                    continue;
            }

            //
            // Check if the StaEntry matches the desired SSID.
            //

            SSID = &pStation->Config.SSID;
            if (SSID->uSSIDLength > 0) 
            {
                ndisStatus = Dot11GetInfoEle(StaEntry->InfoElemBlobPtr,
                                             StaEntry->InfoElemBlobSize,
                                             DOT11_INFO_ELEMENT_ID_SSID,
                                             &size,
                                             &tmpPtr);
                if (ndisStatus != NDIS_STATUS_SUCCESS)
                    continue;

                if (SSID->uSSIDLength != size || NdisEqualMemory(SSID->ucSSID, tmpPtr, size) == 0)
                    continue;
            }

            //
            // Check for other attributes.
            //
            if (!StaAcceptStation(pStation, StaEntry))
                continue;
            
            //
            // Found a matching station. Allocate memory for it.
            //
            if (sizeof(STA_ADHOC_STA_ENTRY) > 
                    (sizeof(STA_ADHOC_STA_ENTRY) + StaEntry->InfoElemBlobSize))
            {
                ndisStatus = NDIS_STATUS_FAILURE;
                continue;
            }

            MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
                               &tmpPtr,
                               sizeof(STA_ADHOC_STA_ENTRY) + StaEntry->InfoElemBlobSize, 
                               MP_MEMORY_TAG);
            if (tmpPtr == NULL)
            {
                ndisStatus = NDIS_STATUS_RESOURCES;
                continue;
            }

            //
            // Copy the information of the matching station.
            //

            (*StaEntryArray)[*StaCount] = (PSTA_ADHOC_STA_ENTRY)tmpPtr;
            NdisMoveMemory(tmpPtr, StaEntry, sizeof(STA_ADHOC_STA_ENTRY));

            tmpPtr += sizeof(STA_ADHOC_STA_ENTRY);
            (*StaEntryArray)[*StaCount]->InfoElemBlobPtr = tmpPtr;
            NdisMoveMemory(tmpPtr, StaEntry->InfoElemBlobPtr, StaEntry->InfoElemBlobSize);

            (*StaCount)++;
        }
    }
    __finally
    {
        MP_RELEASE_READ_LOCK(&pStation->AdHocStaInfo.StaListLock, &LockState);
    }

    return ndisStatus;
}

VOID
StaConnectAdHocWorkItem(
    PVOID           Context,
    NDIS_HANDLE     NdisIoWorkItemHandle
    )
{
    NDIS_STATUS                             ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_EXTSTA_PORT                                pStation;
    PVNIC                                   pNic;
    ULONG                                   index;
    ULONG                                   StaCount = 0;
    PSTA_ADHOC_STA_ENTRY                   *StaEntryArray = NULL;
    PSTA_ADHOC_STA_ENTRY                    StaEntry = NULL;
    DOT11_MAC_ADDRESS                       newBSSID;
    ULONG                                   PhyId;
    LARGE_INTEGER                           timeoutTime;
    
    UNREFERENCED_PARAMETER(NdisIoWorkItemHandle);

    pStation = (PMP_EXTSTA_PORT)Context;
    pNic = STA_GET_VNIC(pStation);

    __try 
    {

        //
        // Delete all non-persistent WEP keys and clear the association state.
        //

        VNic11DeleteNonPersistentKey(pNic);
        StaClearStaListAssocState(pStation, FALSE);

        //
        // Since we need beacons & probe response for establishing the adhoc network, 
        // update the packet filter on the VNIC. This is a combination of the packet 
        // filter already set by the upper layer with the additional bits set
        //
        VNic11SetPacketFilter(pNic, 
            (STA_GET_MP_PORT(pStation)->PacketFilter | 
             (NDIS_PACKET_TYPE_802_11_DIRECTED_MGMT | NDIS_PACKET_TYPE_802_11_BROADCAST_MGMT)
             )
            );

        //
        // Search for an existing AdHoc network to connect to. If IBSSJoinOnly is set,
        // keep search until one is found or a reset is pending.
        //

        do
        {

            //
            // If this is not the first time we execute this loop, wait for a short while
            // to prevent us from spinning.
            //

            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                NdisMSleep(100000);    // 100 ms.
            }
            else
            {
                //
                // Lets force a periodic scan
                //
                NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
                pStation->ScanContext.PeriodicScanCounter = STA_DEFAULT_SCAN_TICK_COUNT;
                NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            }

            //
            // Take a snapshot of the current AdHoc station list. We can't hold the station list 
            // lock and access the list directly since connect operation is a lengthy process in 
            // which we may call wait functions. 
            //

            if (StaEntryArray) 
            {
                for (index = 0; index < StaCount; index++)
                {
                    MP_FREE_MEMORY(StaEntryArray[index]);
                }

                MP_FREE_MEMORY(StaEntryArray);
                StaEntryArray = NULL;
            }

            ndisStatus = StaGetMatchingAdHocStaList(pStation, &StaCount, &StaEntryArray);
            if (ndisStatus != NDIS_STATUS_SUCCESS || (NULL == StaEntryArray)) 
            {
                ndisStatus = (ndisStatus != NDIS_STATUS_SUCCESS) ? ndisStatus : NDIS_STATUS_FAILURE;
                __leave;
            }

            //
            // Go through the matching station list and try to join the existing Ad Hoc network.
            //

            ndisStatus = STATUS_NOT_FOUND;
            for (index = 0; index < StaCount; index++) 
            {
                StaEntry = StaEntryArray[index];
                if (NULL == StaEntry)
                    continue;

                //
                // Do a synchronize join request with the station's SSID and BSSID.
                //
                ndisStatus = StaJoinAdHoc(pStation, StaEntry);
                if (ndisStatus == NDIS_STATUS_REQUEST_ABORTED)
                    break;
                else if (ndisStatus != NDIS_STATUS_SUCCESS) 
                    continue;

                // sucessfully joined adhoc, check for protection info in next beacon
                pStation->Config.CheckForProtectionInERP = TRUE;
                
                //
                // Start the distributed beacon for Ad Hoc
                //
                ndisStatus = StaStartAdHoc(pStation, StaEntry, NULL);
                if (ndisStatus == NDIS_STATUS_SUCCESS) 
                    break;
            }

            //
            // Check again if reset is pending.
            //
            if (ndisStatus != NDIS_STATUS_REQUEST_ABORTED)
            {
                NdisAcquireSpinLock(&pStation->AdHocStaInfo.StaInfoLock);
                if (pStation->AdHocStaInfo.AdHocState & STA_ADHOC_RESET_PENDING)
                    ndisStatus = NDIS_STATUS_REQUEST_ABORTED;
                NdisReleaseSpinLock(&pStation->AdHocStaInfo.StaInfoLock);
            }

            // We would keep looping trying to setup the adhoc if we were required to 
            // join and havent yet joined one. Else we move out and start our own adhoc
        } while (pStation->Config.IBSSJoinOnly && 
                 ndisStatus != NDIS_STATUS_SUCCESS && 
                 ndisStatus != NDIS_STATUS_REQUEST_ABORTED);

        if (ndisStatus == NDIS_STATUS_REQUEST_ABORTED)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("AdHoc connect: Aborted due to reset\n"));
        }

        //
        // No existing matching AdHoc network found, start our own.
        //

        if (ndisStatus != NDIS_STATUS_SUCCESS && ndisStatus != NDIS_STATUS_REQUEST_ABORTED) 
        {

            //
            // start a new Adhoc cell if our beacon interval is longer than ATIM windows
            // and we have the desired SSID.
            //
            
            if (VNic11QueryBeaconPeriod(pNic) > VNic11QueryATIMWindow(pNic) &&
                pStation->Config.SSID.uSSIDLength > 0)
            {

                //
                // If we have desired BSSID list, use the first BSSID from the list as the
                // BSSID of the new Ad Hoc network. Otherwise, generate a random BSSID. 
                //

                if (!pStation->Config.AcceptAnyBSSID && pStation->Config.DesiredBSSIDCount > 0)
                    NdisMoveMemory(newBSSID, pStation->Config.DesiredBSSIDList[0], sizeof(DOT11_MAC_ADDRESS));
                else
                    Dot11GenerateRandomBSSID((PVOID)VNic11QueryMACAddress(pNic), newBSSID);

                //
                // If our current phy type isn't in the desired phy list, set it to the first phy in
                // the list.
                //

                PhyId = VNic11QueryOperatingPhyId(pNic);
                if (!StaMatchPhyId(pStation, PhyId))
                {
                    PhyId = pStation->Config.DesiredPhyList[0];
                    ASSERT(PhyId != DOT11_PHY_ID_ANY);
                    ndisStatus = VNic11SetOperatingPhyId(pNic, PhyId);
                }
                else 
                {
                    ndisStatus = NDIS_STATUS_SUCCESS;
                }

                //
                // Start a new Ad Hoc network
                //

                if (ndisStatus == NDIS_STATUS_SUCCESS)
                {
                    ndisStatus = StaStartAdHoc(pStation, NULL, newBSSID);
                }
            }
            else
                __leave;
        }

        //
        // Indicate connection start to the OS. If the connection status is not successful, 
        // use all-zeros as the BSSID in the connection start structure.
        //
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            StaAdhocIndicateConnectionStart(pStation, NULL, FALSE);
        }
        else
        {
            StaAdhocIndicateConnectionStart(pStation, *VNic11QueryCurrentBSSID(pNic), FALSE);
        }

        //
        // Indicate connection complete to the OS
        //
        StaAdhocIndicateConnectionCompletion(pStation, ndisStatus, FALSE);

    }
    __finally
    {
        if (StaEntryArray) 
        {
            for (index = 0; index < StaCount; index++)
            {
                if (StaEntryArray[index] != NULL)
                {
                    MP_FREE_MEMORY(StaEntryArray[index]);
                }
            }

            MP_FREE_MEMORY(StaEntryArray);
        }

        NdisAcquireSpinLock(&pStation->AdHocStaInfo.StaInfoLock);

        //
        // Check if reset is pending one last time.
        //
        if (ndisStatus == NDIS_STATUS_SUCCESS &&
            (pStation->AdHocStaInfo.AdHocState & STA_ADHOC_RESET_PENDING))
        {
            ndisStatus = NDIS_STATUS_REQUEST_ABORTED;
        }

        //
        // If connection is successful, start a timer watching for disconnected stations.
        //
        if (ndisStatus == NDIS_STATUS_SUCCESS) 
        {
            NdisInterlockedIncrement(&pStation->AdHocStaInfo.TimerCounter);
            timeoutTime.QuadPart = Int32x32To64((LONG)2000, -10000);
            NdisSetTimerObject(pStation->AdHocStaInfo.WatchdogTimer, timeoutTime, 0, NULL);
            pStation->AdHocStaInfo.AdHocState = STA_ADHOC_CONNECTED;

            //
            // Here we dont need to forward the connection status to the VNIC, it will
            // change itself to be disconnected
            //
        }
        else
        {
            pStation->AdHocStaInfo.AdHocState = STA_ADHOC_DISCONNECTED;
        }
        
        NdisReleaseSpinLock(&pStation->AdHocStaInfo.StaInfoLock);
    }
}


VOID
StaAdhocIndicateConnectionStart(
    _In_  PMP_EXTSTA_PORT         pStation,
    _In_reads_bytes_opt_(DOT11_ADDRESS_SIZE)  DOT11_MAC_ADDRESS       BSSID,
    _In_  BOOLEAN                 Internal
    )
{
    DOT11_CONNECTION_START_PARAMETERS   connStart;

    //
    // If the connection status is not successful, 
    // use all-zeros as the BSSID in the connection start structure.
    //
    MP_ASSIGN_NDIS_OBJECT_HEADER(connStart.Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_CONNECTION_START_PARAMETERS_REVISION_1,
                                 sizeof(DOT11_CONNECTION_START_PARAMETERS));
    connStart.BSSType = dot11_BSS_type_independent;
    if (BSSID != NULL) 
    {
        NdisMoveMemory(connStart.AdhocBSSID, 
            BSSID, 
            sizeof(DOT11_MAC_ADDRESS)
            );
    }
    else
    {
        NdisZeroMemory(connStart.AdhocBSSID, sizeof(DOT11_MAC_ADDRESS));
    }

    connStart.AdhocSSID = pStation->Config.SSID;

    if (!Internal)
    {
        StaIndicateDot11Status(pStation, 
                               NDIS_STATUS_DOT11_CONNECTION_START,
                               NULL,
                               &connStart,
                               sizeof(DOT11_CONNECTION_START_PARAMETERS));
    }
    else
    {
        //
        // Forward the connection status to the hardware
        //
        VNic11NotifyConnectionStatus(
            STA_GET_VNIC(pStation), 
            TRUE, // Start with true
            NDIS_STATUS_DOT11_CONNECTION_START,
            &connStart, 
            sizeof(DOT11_CONNECTION_START_PARAMETERS)
            );    
    }
}


VOID
StaAdhocIndicateConnectionCompletion(
    _In_  PMP_EXTSTA_PORT         pStation,
    _In_  ULONG                   CompletionStatus,
    _In_  BOOLEAN                 Internal
    )
{
    DOT11_CONNECTION_COMPLETION_PARAMETERS   connComp;

    MP_ASSIGN_NDIS_OBJECT_HEADER(connComp.Header, 
         NDIS_OBJECT_TYPE_DEFAULT,
         DOT11_CONNECTION_COMPLETION_PARAMETERS_REVISION_1,
         sizeof(DOT11_CONNECTION_COMPLETION_PARAMETERS)
         );

    switch (CompletionStatus)
    {
        case NDIS_STATUS_SUCCESS:
            connComp.uStatus = DOT11_CONNECTION_STATUS_SUCCESS;
            break;

        case NDIS_STATUS_REQUEST_ABORTED:
            connComp.uStatus = DOT11_CONNECTION_STATUS_CANCELLED;
            break;

        default:
            connComp.uStatus = DOT11_CONNECTION_STATUS_FAILURE;
    }

    if (Internal)
    {
        //
        // Forward the connection status to the hardware
        //
        VNic11NotifyConnectionStatus(
            STA_GET_VNIC(pStation), 
            (CompletionStatus == NDIS_STATUS_SUCCESS) ? TRUE : FALSE, 
            NDIS_STATUS_DOT11_CONNECTION_COMPLETION,
            &connComp, 
            sizeof(DOT11_CONNECTION_COMPLETION_PARAMETERS)
            );
    }
    else
    {
        //
        // And to the OS
        //
        StaIndicateDot11Status(pStation, 
           NDIS_STATUS_DOT11_CONNECTION_COMPLETION,
           NULL,
           &connComp,
           sizeof(DOT11_CONNECTION_COMPLETION_PARAMETERS)
           );
    }

}

void 
StaAdHocIndicateAssociation(
    _In_  PMP_EXTSTA_PORT pStation,
    _In_  PSTA_ADHOC_STA_ENTRY StaEntry
    )
{
    NDIS_STATUS                                 ndisStatus = NDIS_STATUS_SUCCESS;
    UCHAR                                       size;
    PUCHAR                                      tmpPtr;
    DOT11_ASSOCIATION_START_PARAMETERS          assocStart;
    PDOT11_ASSOCIATION_COMPLETION_PARAMETERS    assocComp;
    ULONG                                       assocCompSize;
    DOT11_DISASSOCIATION_PARAMETERS             disassocParam = {0};
    DOT11_BEACON_FRAME UNALIGNED               *beaconFrame;
    BOOLEAN                                     match;
    BOOLEAN                                     connected;
    PSTA_ADHOC_STA_INFO                         AdHocStaInfo = &pStation->AdHocStaInfo;

    //
    // If we are not in the connected state, return.
    //

    NdisAcquireSpinLock(&AdHocStaInfo->StaInfoLock);
    connected = (BOOLEAN)(AdHocStaInfo->AdHocState == STA_ADHOC_CONNECTED);
    NdisReleaseSpinLock(&AdHocStaInfo->StaInfoLock);

    if (!connected)
        return;

    //
    // Check BSSID
    //

    match = (BOOLEAN)(NdisEqualMemory(VNic11QueryCurrentBSSID(STA_GET_VNIC(pStation)), 
                                      StaEntry->Dot11BSSID, 
                                      sizeof(DOT11_MAC_ADDRESS)) == 1);

    //
    // Check SSID
    //

    if (match) 
    {
        ndisStatus = Dot11GetInfoEle(StaEntry->InfoElemBlobPtr,
                                     StaEntry->InfoElemBlobSize,
                                     DOT11_INFO_ELEMENT_ID_SSID,
                                     &size,
                                     &tmpPtr);
        match = (BOOLEAN)(ndisStatus == NDIS_STATUS_SUCCESS &&
                          pStation->Config.SSID.uSSIDLength == size &&
                          NdisEqualMemory(pStation->Config.SSID.ucSSID, tmpPtr, size) == 1);
    }

    //
    // If the station was associated, but now its SSID or BSSID is changed, indicate disassociation.
    // Likewise, if the station was not associated, but its SSID and BSSID match with our current
    // BSSID and SSID, indicate association.
    //

    if (StaEntry->AssocState == dot11_assoc_state_auth_assoc && !match) 
    {

        //
        // Indicate DISASSOCIATION
        //
        MP_ASSIGN_NDIS_OBJECT_HEADER(disassocParam.Header, 
                                     NDIS_OBJECT_TYPE_DEFAULT,
                                     DOT11_DISASSOCIATION_PARAMETERS_REVISION_1,
                                     sizeof(DOT11_DISASSOCIATION_PARAMETERS));

        disassocParam.uReason = DOT11_ASSOC_STATUS_PEER_DISASSOCIATED_START | DOT11_MGMT_REASON_DISASSO_LEAVE_SS;

        NdisMoveMemory(disassocParam.MacAddr, StaEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));

        StaIndicateDot11Status(pStation, 
                               NDIS_STATUS_DOT11_DISASSOCIATION,
                               NULL,
                               &disassocParam,
                               sizeof(DOT11_DISASSOCIATION_PARAMETERS));

        //
        // This station is no longer assocated.
        //

        StaEntry->AssocState = dot11_assoc_state_unauth_unassoc;

        //
        // Delete key mapping key and per-STA key associated with the leaving station.
        //

        VNic11DeleteNonPersistentMappingKey(STA_GET_VNIC(pStation), StaEntry->MacAddress);

        VNic11NotifyConnectionStatus(
            STA_GET_VNIC(pStation), 
            TRUE,                       // Even if we may not have peers, our status is connected
            NDIS_STATUS_DOT11_DISASSOCIATION,
            &disassocParam, 
            sizeof(DOT11_DISASSOCIATION_PARAMETERS)
            );

        MpTrace(COMP_ASSOC, DBG_SERIOUS, 
                ("Ad Hoc station disassociated due to mismatch SSID/BSSID: %02X-%02X-%02X-%02X-%02X-%02X\n", 
                StaEntry->MacAddress[0], StaEntry->MacAddress[1], StaEntry->MacAddress[2], 
                StaEntry->MacAddress[3], StaEntry->MacAddress[4], StaEntry->MacAddress[5]));
    }
    else if (StaEntry->AssocState == dot11_assoc_state_unauth_unassoc && 
             match && 
             StaAcceptStation(pStation, StaEntry))
    {

        //
        // Allocate enough memory for ASSOCIATION_COMPLETE indication. If allocation fails,
        // we skip this beaconing station.
        //

        // Integer overflow
        if ((FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements) + StaEntry->InfoElemBlobSize) > 
                (FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements) +
                StaEntry->InfoElemBlobSize +
                sizeof(ULONG)))
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            return;
        }

        assocCompSize = sizeof(DOT11_ASSOCIATION_COMPLETION_PARAMETERS) +
                        FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements) +
                        StaEntry->InfoElemBlobSize +    // for beacon
                        sizeof(ULONG);                  // for single entry PHY list

        MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
                           &assocComp, 
                           assocCompSize,
                           MP_MEMORY_TAG);
        if (assocComp == NULL) 
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            return;
        }
    
        //
        // Indicate ASSOCIATION_START
        //
        MP_ASSIGN_NDIS_OBJECT_HEADER(assocStart.Header, 
                                     NDIS_OBJECT_TYPE_DEFAULT,
                                     DOT11_ASSOCIATION_START_PARAMETERS_REVISION_1,
                                     sizeof(DOT11_ASSOCIATION_START_PARAMETERS));
        assocStart.uIHVDataOffset = 0;
        assocStart.uIHVDataSize = 0;

        assocStart.SSID = pStation->Config.SSID;
        NdisMoveMemory(assocStart.MacAddr, StaEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));

        StaIndicateDot11Status(pStation, 
                               NDIS_STATUS_DOT11_ASSOCIATION_START,
                               NULL,
                               &assocStart,
                               sizeof(DOT11_ASSOCIATION_START_PARAMETERS));

        //
        // Indicate association start to the HW
        //
        VNic11NotifyConnectionStatus(
            STA_GET_VNIC(pStation), 
            TRUE,
            NDIS_STATUS_DOT11_ASSOCIATION_START,
            &assocStart, 
            sizeof(DOT11_ASSOCIATION_START_PARAMETERS)
            );

        //
        // Indicate ASSOCIATION_COMPLETE
        // 
        MP_ASSIGN_NDIS_OBJECT_HEADER(assocComp->Header, 
                                     NDIS_OBJECT_TYPE_DEFAULT,
                                     DOT11_ASSOCIATION_COMPLETION_PARAMETERS_REVISION_1,
                                     sizeof(DOT11_ASSOCIATION_COMPLETION_PARAMETERS));

        NdisMoveMemory(assocComp->MacAddr, StaEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));
        assocComp->uStatus = 0;

        assocComp->bReAssocReq = FALSE;
        assocComp->bReAssocResp = FALSE;
        assocComp->uAssocReqOffset = 0;
        assocComp->uAssocReqSize = 0;
        assocComp->uAssocRespOffset = 0;
        assocComp->uAssocRespSize = 0;
        
        //
        // Append the beacon information of this beaconing station.
        //

        beaconFrame = (DOT11_BEACON_FRAME UNALIGNED *)
                      Add2Ptr(assocComp, sizeof(DOT11_ASSOCIATION_COMPLETION_PARAMETERS));
        beaconFrame->Timestamp = StaEntry->BeaconTimestamp;
        beaconFrame->BeaconInterval = StaEntry->BeaconInterval; 
        beaconFrame->Capability.usValue = StaEntry->Dot11Capability.usValue;
        NdisMoveMemory((PVOID)&beaconFrame->InfoElements,
                      StaEntry->InfoElemBlobPtr,
                      StaEntry->InfoElemBlobSize);
        assocComp->uBeaconOffset = sizeof(DOT11_ASSOCIATION_COMPLETION_PARAMETERS);
        assocComp->uBeaconSize = FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements) + StaEntry->InfoElemBlobSize;
        assocComp->uIHVDataOffset = 0;
        assocComp->uIHVDataSize = 0;
        
        //
        // Set the auth and cipher algorithm.
        //

        assocComp->AuthAlgo = pStation->Config.AuthAlgorithm;
        assocComp->UnicastCipher = pStation->Config.UnicastCipherAlgorithm;
        assocComp->MulticastCipher = pStation->Config.MulticastCipherAlgorithm;

        //
        // Set the PHY list. It just contains our active phy id.
        //

        assocComp->uActivePhyListOffset = sizeof(DOT11_ASSOCIATION_COMPLETION_PARAMETERS) + 
                                          FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements) +
                                          StaEntry->InfoElemBlobSize;
        assocComp->uActivePhyListSize = sizeof(ULONG);
        *((ULONG UNALIGNED *)Add2Ptr(assocComp, assocComp->uActivePhyListOffset)) = pStation->Config.ActivePhyId;

        assocComp->bFourAddressSupported = FALSE;
        assocComp->bPortAuthorized = FALSE;
        assocComp->DSInfo = DOT11_DS_UNKNOWN;

        assocComp->uEncapTableOffset = 0;
        assocComp->uEncapTableSize = 0;

        //
        // Before informing the OS about the association, inform the HW
        //
        VNic11NotifyConnectionStatus(
            STA_GET_VNIC(pStation), 
            TRUE,
            NDIS_STATUS_DOT11_ASSOCIATION_COMPLETION,
            assocComp, 
            assocCompSize
            );

        //
        // Inform the OS
        //
        StaIndicateDot11Status(pStation, 
                               NDIS_STATUS_DOT11_ASSOCIATION_COMPLETION,
                               NULL,
                               assocComp,
                               assocCompSize);
        //
        // Free the preallocated ASSOCIATION_COMPLETE indication structure.
        //

        MP_FREE_MEMORY(assocComp);

        //
        // This station is assocated.
        //
        StaEntry->AssocState = dot11_assoc_state_auth_assoc;

        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Ad Hoc station associated: %02X-%02X-%02X-%02X-%02X-%02X\n", 
                StaEntry->MacAddress[0], StaEntry->MacAddress[1], StaEntry->MacAddress[2], 
                StaEntry->MacAddress[3], StaEntry->MacAddress[4], StaEntry->MacAddress[5]));
    }
}

void
StaProbeInactiveStation(
    _In_  PMP_EXTSTA_PORT pStation,
    _In_  PSTA_ADHOC_STA_ENTRY StaEntry
    )
{
    char                buffer[256];    // big enough for a probe request message.
    PDOT11_MGMT_HEADER  MgmtPacket = (PDOT11_MGMT_HEADER)buffer;
    NDIS_STATUS         ndisStatus;
    PUCHAR              infoBlobPtr;
    USHORT              infoBlobSize;
    DOT11_RATE_SET      rateSet = {0};

    MgmtPacket->FrameControl.Version = 0x0;
    MgmtPacket->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
    MgmtPacket->FrameControl.Subtype = DOT11_MGMT_SUBTYPE_PROBE_REQUEST;
    MgmtPacket->FrameControl.ToDS = 0x0;      // Default value for Mgmt frames
    MgmtPacket->FrameControl.FromDS = 0x0;    // Default value for Mgmt frames
    MgmtPacket->FrameControl.MoreFrag = 0x0;  
    MgmtPacket->FrameControl.Retry = 0x0;
    MgmtPacket->FrameControl.PwrMgt = 0x0;
    MgmtPacket->FrameControl.MoreData = 0x0;
    MgmtPacket->FrameControl.WEP = 0x0;       // no WEP
    MgmtPacket->FrameControl.Order = 0x0;     // no order
    MgmtPacket->SequenceControl.usValue = 0;

    NdisMoveMemory(MgmtPacket->DA, 
                   StaEntry->MacAddress,
                   DOT11_ADDRESS_SIZE);
    
    NdisMoveMemory(MgmtPacket->SA, 
                   VNic11QueryMACAddress(STA_GET_VNIC(pStation)),
                   DOT11_ADDRESS_SIZE);

    NdisMoveMemory(MgmtPacket->BSSID,
                   VNic11QueryCurrentBSSID(STA_GET_VNIC(pStation)),
                   DOT11_ADDRESS_SIZE);

    //
    // Add SSID to the probe request.
    //
    infoBlobPtr = Add2Ptr(MgmtPacket, DOT11_MGMT_HEADER_SIZE);
    infoBlobSize = sizeof(buffer) - DOT11_MGMT_HEADER_SIZE;
    
    ndisStatus = Dot11AttachElement(&infoBlobPtr,
                                    &infoBlobSize,
                                    DOT11_INFO_ELEMENT_ID_SSID,
                                    (UCHAR)pStation->Config.SSID.uSSIDLength,
                                    pStation->Config.SSID.ucSSID);

    ASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
        return;

    //
    // Add supported rates to the probe request.
    //
    ndisStatus = Dot11GetRateSetFromInfoEle(StaEntry->InfoElemBlobPtr,
                                          StaEntry->InfoElemBlobSize,
                                          TRUE, 
                                          &rateSet);
    ASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
        return;

    ndisStatus = Dot11AttachElement(&infoBlobPtr,
                                    &infoBlobSize,
                                    DOT11_INFO_ELEMENT_ID_SUPPORTED_RATES,
                                    (UCHAR)((rateSet.uRateSetLength > 8) ? 8 : rateSet.uRateSetLength),
                                    rateSet.ucRateSet);
    ASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
        return;
    
    if (rateSet.uRateSetLength > (UCHAR)8) 
    {
        ndisStatus = Dot11AttachElement(&infoBlobPtr,
                                        &infoBlobSize,
                                        DOT11_INFO_ELEMENT_ID_EXTD_SUPPORTED_RATES,
                                        (UCHAR)(rateSet.uRateSetLength - 8),
                                        rateSet.ucRateSet + 8);
        ASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
            return;
    }
        
    BasePortSendInternalPacket(STA_GET_MP_PORT(pStation), 
                        (PUCHAR)MgmtPacket, 
                       sizeof(buffer) - infoBlobSize);

    MpTrace(COMP_ASSOC, DBG_SERIOUS, 
            ("Sent direct probe request to inactive Ad Hoc station: %02X-%02X-%02X-%02X-%02X-%02X\n", 
            StaEntry->MacAddress[0], StaEntry->MacAddress[1], StaEntry->MacAddress[2], 
            StaEntry->MacAddress[3], StaEntry->MacAddress[4], StaEntry->MacAddress[5]));
}

VOID
StaAdHocWatchdogTimerRoutine(
    _In_  PVOID     SystemSpecific1,
    _In_  PVOID     FunctionContext,
    _In_  PVOID     SystemSpecific2,
    _In_  PVOID     SystemSpecific3
    )
{
    PMP_EXTSTA_PORT pStation = (PMP_EXTSTA_PORT )FunctionContext;
    
    ULONGLONG                       currentTime;
    ULONGLONG                       disassocTime;
    ULONGLONG                       removeTime;
    DOT11_DISASSOCIATION_PARAMETERS disassocParam;
    PSTA_ADHOC_STA_INFO             AdHocStaInfo = &pStation->AdHocStaInfo;
    PSTA_ADHOC_STA_ENTRY            StaEntry;
    PLIST_ENTRY                     pHead;
    PLIST_ENTRY                     pEntry;
    MP_RW_LOCK_STATE                      LockState;
    BOOLEAN                         StopTimer;
    ULONG                           index;
    LARGE_INTEGER                   fireUpTime;



    UNREFERENCED_PARAMETER(SystemSpecific1);
    UNREFERENCED_PARAMETER(SystemSpecific2);
    UNREFERENCED_PARAMETER(SystemSpecific3);

    //
    // If we are not in the connected state, return.
    //

    NdisAcquireSpinLock(&AdHocStaInfo->StaInfoLock);
    StopTimer = (BOOLEAN)(AdHocStaInfo->AdHocState != STA_ADHOC_CONNECTED);
    NdisReleaseSpinLock(&AdHocStaInfo->StaInfoLock);

    if (StopTimer)
    {
        NdisInterlockedDecrement(&AdHocStaInfo->TimerCounter);
        return;
    }

    __try
    {
        MP_ACQUIRE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);

        //
        // Calculate the various times. For Ad Hoc, since only one station in the entire network
        // will transmit beacon frame during a beacon interval, the UnreachableDetectionThreshold
        // multiplied by number of stations in the network is used as the actual threshold.
        //

        NdisGetCurrentSystemTime((PLARGE_INTEGER)&currentTime);
        disassocTime = pStation->Config.UnreachableDetectionThreshold;
        disassocTime *= (AdHocStaInfo->StaCount + 1);

        //
        // We enforce a range.
        //
        if (disassocTime < STA_ADHOC_MIN_UNREACHABLE_THRESHOLD)
            disassocTime = STA_ADHOC_MIN_UNREACHABLE_THRESHOLD;
        if (disassocTime > STA_ADHOC_MAX_UNREACHABLE_THRESHOLD)
            disassocTime = STA_ADHOC_MAX_UNREACHABLE_THRESHOLD;

        disassocTime *= 10000;
        
        removeTime = pStation->RegInfo->BSSEntryExpireTime;

        //
        // Go through the Ad Hoc station list. If we have not received a beacon or probe response from
        // an associated station for more than disassocTime, we disassociat the station. If we have not 
        // received a beacon or probe response from any station for more than removeTime, we remove the 
        // station from the list.
        //

        pHead = &AdHocStaInfo->StaList;
        pEntry = pHead->Flink;
        while (pEntry != pHead)
        {

            StaEntry = CONTAINING_RECORD(pEntry, STA_ADHOC_STA_ENTRY, Link);
            pEntry = pEntry->Flink;

            //
            // Disassociate the station if it appears on the exclusion list.
            //

            if (StaEntry->AssocState == dot11_assoc_state_auth_assoc)
            {
                for (index = 0; index < pStation->Config.ExcludedMACAddressCount; index++) 
                {
                    if (NdisEqualMemory(StaEntry->MacAddress,
                                        pStation->Config.ExcludedMACAddressList[index],
                                        sizeof(DOT11_MAC_ADDRESS)) == 1)
                        break;
                }

                if (index < pStation->Config.ExcludedMACAddressCount)
                {
                    //
                    // Indicate DISASSOCIATION
                    //
                    MP_ASSIGN_NDIS_OBJECT_HEADER(disassocParam.Header, 
                                                 NDIS_OBJECT_TYPE_DEFAULT,
                                                 DOT11_DISASSOCIATION_PARAMETERS_REVISION_1,
                                                 sizeof(DOT11_DISASSOCIATION_PARAMETERS));
                    disassocParam.uIHVDataOffset = 0;
                    disassocParam.uIHVDataSize = 0;
                    disassocParam.uReason = DOT11_DISASSOC_REASON_OS;
                    NdisMoveMemory(disassocParam.MacAddr, StaEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));

                    StaIndicateDot11Status(pStation, 
                                           NDIS_STATUS_DOT11_DISASSOCIATION,
                                           NULL,
                                           &disassocParam,
                                           sizeof(DOT11_DISASSOCIATION_PARAMETERS));

                    //
                    // This station is no longer assocated.
                    //

                    StaEntry->AssocState = dot11_assoc_state_unauth_unassoc;

                    //
                    // Delete key mapping key and per-STA key associated with the leaving station.
                    //
                    VNic11DeleteNonPersistentMappingKey(STA_GET_VNIC(pStation), StaEntry->MacAddress);

                    //
                    // Inform the hardware
                    //
                    VNic11NotifyConnectionStatus(
                        STA_GET_VNIC(pStation), 
                        TRUE,                       // Even if we may not have peers, our status is connected
                        NDIS_STATUS_DOT11_DISASSOCIATION,
                        &disassocParam, 
                        sizeof(DOT11_DISASSOCIATION_PARAMETERS)
                        );

                    MpTrace(COMP_ASSOC, DBG_SERIOUS, 
                            ("Ad Hoc station disassociated due to exclusion: %02X-%02X-%02X-%02X-%02X-%02X\n", 
                            StaEntry->MacAddress[0], StaEntry->MacAddress[1], StaEntry->MacAddress[2], 
                            StaEntry->MacAddress[3], StaEntry->MacAddress[4], StaEntry->MacAddress[5]));
                }
            }
            
            //
            // Disassociate the station if we have not received a beacon or probe response from
            // from it for more than disassocTime.
            //

            if (StaEntry->AssocState == dot11_assoc_state_auth_assoc && 
                StaEntry->HostTimestamp + disassocTime < currentTime)
            {
                if (StaEntry->ProbeRequestsSent++ < STA_PROBE_REQUEST_LIMIT)
                {
                    StaProbeInactiveStation(pStation, StaEntry);
                    continue;
                }
                
                //
                // Indicate DISASSOCIATION
                //
                MP_ASSIGN_NDIS_OBJECT_HEADER(disassocParam.Header, 
                                             NDIS_OBJECT_TYPE_DEFAULT,
                                             DOT11_DISASSOCIATION_PARAMETERS_REVISION_1,
                                             sizeof(DOT11_DISASSOCIATION_PARAMETERS));
                disassocParam.uIHVDataOffset = 0;
                disassocParam.uIHVDataSize = 0;
                disassocParam.uReason = DOT11_DISASSOC_REASON_PEER_UNREACHABLE;
                NdisMoveMemory(disassocParam.MacAddr, StaEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));

                StaIndicateDot11Status(pStation, 
                                       NDIS_STATUS_DOT11_DISASSOCIATION,
                                       NULL,
                                       &disassocParam,
                                       sizeof(DOT11_DISASSOCIATION_PARAMETERS));

                //
                // This station is no longer assocated.
                //

                StaEntry->AssocState = dot11_assoc_state_unauth_unassoc;

                //
                // Delete key mapping key and per-STA key associated with the leaving station.
                //

                VNic11DeleteNonPersistentMappingKey(STA_GET_VNIC(pStation), StaEntry->MacAddress);

                //
                // Inform the hardware
                //
                VNic11NotifyConnectionStatus(
                    STA_GET_VNIC(pStation), 
                    TRUE,                       // Even if we may not have peers, our status is connected
                    NDIS_STATUS_DOT11_DISASSOCIATION,
                    &disassocParam, 
                    sizeof(DOT11_DISASSOCIATION_PARAMETERS)
                    );

                MpTrace(COMP_ASSOC, DBG_SERIOUS, 
                        ("Ad Hoc station disassociated due to inactivity: %02X-%02X-%02X-%02X-%02X-%02X\n", 
                        StaEntry->MacAddress[0], StaEntry->MacAddress[1], StaEntry->MacAddress[2], 
                        StaEntry->MacAddress[3], StaEntry->MacAddress[4], StaEntry->MacAddress[5]));
            }

            //
            // If we just received de-auth from the station, it's in a special state. Change it to 
            // dot11_assoc_state_unauth_unassoc after it reaches its waiting period. This prevents 
            // us from re-associating a station in the situation where the station sends a de-auth 
            // frame, then sends a few beacon before finally quits.
            //

            if (StaEntry->AssocState == dot11_assoc_state_zero)
            {
                StaEntry->DeauthWaitingTick++;
                if (StaEntry->DeauthWaitingTick > STA_DEAUTH_WAITING_THRESHOLD)
                {
                    StaEntry->AssocState = dot11_assoc_state_unauth_unassoc;

                    ASSERT(AdHocStaInfo->DeauthStaCount >= 1);
                    AdHocStaInfo->DeauthStaCount--;
                }
            }

            //
            // Remove the station from the list if we have not received a beacon or probe response from
            // from it for more than removeTime.
            //

            if (StaEntry->HostTimestamp + removeTime < currentTime &&
                StaEntry->AssocState == dot11_assoc_state_unauth_unassoc) 
            {
                ASSERT(AdHocStaInfo->StaCount >= 1);
                RemoveEntryList(&StaEntry->Link);
                AdHocStaInfo->StaCount--;

                MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Ad Hoc station removed: %02X-%02X-%02X-%02X-%02X-%02X\n", 
                        StaEntry->MacAddress[0], StaEntry->MacAddress[1], StaEntry->MacAddress[2], 
                        StaEntry->MacAddress[3], StaEntry->MacAddress[4], StaEntry->MacAddress[5]));

                if (StaEntry->InfoElemBlobPtr) 
                {
                    MP_FREE_MEMORY(StaEntry->InfoElemBlobPtr);
                }
                
                MP_FREE_MEMORY(StaEntry);
            }
        }
    }
    __finally
    {
        MP_RELEASE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);
    }

    //
    // Set the timer again.
    //
    fireUpTime.QuadPart =  Int32x32To64((LONG)2000, -10000);
    NdisSetTimerObject(pStation->AdHocStaInfo.WatchdogTimer, fireUpTime, 0, NULL);
}

VOID
StaAdhocProcessMgmtPacket(
    _In_  PMP_EXTSTA_PORT            pStation,
    _In_  PDOT11_MGMT_HEADER  MgmtPacket,
    _In_  ULONG               PacketLength
    )
{
    BOOLEAN                         connected;
    PDOT11_AUTH_FRAME               AuthFrame;
    PSTA_ADHOC_STA_INFO             AdHocStaInfo = &pStation->AdHocStaInfo;
    PSTA_ADHOC_STA_ENTRY            StaEntry;
    PLIST_ENTRY                     pHead;
    PLIST_ENTRY                     pEntry;
    MP_RW_LOCK_STATE                      LockState;
    DOT11_DISASSOCIATION_PARAMETERS disassocParam;

    //
    // If we are not in the connected state, return.
    //

    NdisAcquireSpinLock(&AdHocStaInfo->StaInfoLock);
    connected = (BOOLEAN)(AdHocStaInfo->AdHocState == STA_ADHOC_CONNECTED);
    NdisReleaseSpinLock(&AdHocStaInfo->StaInfoLock);

    if (!connected)
        return;

    //
    // We only handle authentication and de-authentication requests.
    //
    ASSERT(MgmtPacket->FrameControl.Type == DOT11_FRAME_TYPE_MANAGEMENT);

    switch (MgmtPacket->FrameControl.Subtype)
    {
        case DOT11_MGMT_SUBTYPE_AUTHENTICATION:
            
            //
            // We only process open system authentication request. When we receive such a request,
            // if the sender's association state is dot11_assoc_state_auth_assoc, we respond with
            // a success authentication packet.
            //
            
            //
            // Check frame length.
            //
            if (PacketLength < sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_AUTH_FRAME))
                break;

            //
            // Check BSSID and DA
            //
            if (!MP_COMPARE_MAC_ADDRESS(MgmtPacket->BSSID, VNic11QueryCurrentBSSID(STA_GET_VNIC(pStation))) ||
                !MP_COMPARE_MAC_ADDRESS(MgmtPacket->DA, VNic11QueryMACAddress(STA_GET_VNIC(pStation))))
            {
                break;
            }

            //
            // Get auth frame, make sure it's open system auth request.
            //
            AuthFrame = (PDOT11_AUTH_FRAME)Add2Ptr(MgmtPacket, sizeof(DOT11_MGMT_HEADER));
            if (AuthFrame->usAlgorithmNumber != DOT11_AUTH_OPEN_SYSTEM || AuthFrame->usXid != 1)
                break;

            //
            // Go through the StaEntry list to find the sender.
            //
            MP_ACQUIRE_READ_LOCK(&AdHocStaInfo->StaListLock, &LockState);

            pHead = &AdHocStaInfo->StaList;
            pEntry = pHead->Flink;
            while (pEntry != pHead) 
            {
                StaEntry = CONTAINING_RECORD(pEntry, STA_ADHOC_STA_ENTRY, Link);
                pEntry = pEntry->Flink;

                if (MP_COMPARE_MAC_ADDRESS(MgmtPacket->SA, StaEntry->MacAddress))
                {
                    //
                    // We found the sender on the list. If the sender's association state
                    // is dot11_assoc_state_auth_assoc, we send a response.
                    //
                    if (StaEntry->AssocState == dot11_assoc_state_auth_assoc)
                    {
                        //
                        // Reuse the received frame to format the response.
                        //
                        MgmtPacket->SequenceControl.usValue = 0;
                        NdisMoveMemory(MgmtPacket->SA, MgmtPacket->DA, sizeof(DOT11_MAC_ADDRESS));
                        NdisMoveMemory(MgmtPacket->DA, StaEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));
                        AuthFrame->usXid = 2;
                        AuthFrame->usStatusCode = DOT11_FRAME_STATUS_SUCCESSFUL;
                        
                        BasePortSendInternalPacket(STA_GET_MP_PORT(pStation), 
                                            (PUCHAR)MgmtPacket, 
                                           sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_AUTH_FRAME));
                    }

                    break;
                }
            }

            MP_RELEASE_READ_LOCK(&AdHocStaInfo->StaListLock, &LockState);

            break;

        case DOT11_MGMT_SUBTYPE_DEAUTHENTICATION:
            
            //
            // When we receive a deauthentication request, if the sender's association state is 
            // dot11_assoc_state_auth_assoc, we disassociate the sender and change its association
            // state to dot11_assoc_state_zero.
            //
            
            //
            // Check frame length.
            //
            if (PacketLength < sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_DEAUTH_FRAME))
                break;

            //
            // Check BSSID and DA
            //
            if (!MP_COMPARE_MAC_ADDRESS(MgmtPacket->BSSID, VNic11QueryCurrentBSSID(STA_GET_VNIC(pStation))) ||
                !MP_COMPARE_MAC_ADDRESS(MgmtPacket->DA, VNic11QueryMACAddress(STA_GET_VNIC(pStation))))
            {
                break;
            }

            //
            // Go through the StaEntry list to find the sender.
            //
            MP_ACQUIRE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);

            pHead = &AdHocStaInfo->StaList;
            pEntry = pHead->Flink;
            while (pEntry != pHead) 
            {
                StaEntry = CONTAINING_RECORD(pEntry, STA_ADHOC_STA_ENTRY, Link);
                pEntry = pEntry->Flink;

                if (MP_COMPARE_MAC_ADDRESS(MgmtPacket->SA, StaEntry->MacAddress))
                {
                    //
                    // We found the sender on the list. If the sender's association state
                    // is dot11_assoc_state_auth_assoc, we change it to dot11_assoc_state_zero
                    // and indicate disassociation.
                    //
                    if (StaEntry->AssocState == dot11_assoc_state_auth_assoc)
                    {
                        MP_ASSIGN_NDIS_OBJECT_HEADER(disassocParam.Header, 
                                                     NDIS_OBJECT_TYPE_DEFAULT,
                                                     DOT11_DISASSOCIATION_PARAMETERS_REVISION_1,
                                                     sizeof(DOT11_DISASSOCIATION_PARAMETERS));

                        disassocParam.uReason = DOT11_ASSOC_STATUS_PEER_DISASSOCIATED_START | DOT11_MGMT_REASON_DISASSO_LEAVE_SS;
                        NdisMoveMemory(disassocParam.MacAddr, StaEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));

                        StaIndicateDot11Status(pStation, 
                                               NDIS_STATUS_DOT11_DISASSOCIATION,
                                               NULL,
                                               &disassocParam,
                                               sizeof(DOT11_DISASSOCIATION_PARAMETERS));

                        //
                        // Delete key mapping key and per-STA key associated with the leaving station.
                        //

                        VNic11DeleteNonPersistentMappingKey(STA_GET_VNIC(pStation), StaEntry->MacAddress);

                        //
                        // Inform the hardware
                        //
                        VNic11NotifyConnectionStatus(
                            STA_GET_VNIC(pStation), 
                            TRUE,                       // Even if we may not have peers, our status is connected
                            NDIS_STATUS_DOT11_DISASSOCIATION,
                            &disassocParam, 
                            sizeof(DOT11_DISASSOCIATION_PARAMETERS)
                            );

                        //
                        // This station is no longer assocated. We assign dot11_assoc_state_zero
                        // as its association state so that if we receive a beacon from it right away,
                        // we would not re-associate it.
                        //

                        StaEntry->AssocState = dot11_assoc_state_zero;
                        StaEntry->DeauthWaitingTick = 0;

                        ASSERT(AdHocStaInfo->DeauthStaCount < AdHocStaInfo->StaCount);
                        AdHocStaInfo->DeauthStaCount++;

                        MpTrace(COMP_ASSOC, DBG_SERIOUS, 
                                ("Ad Hoc station disassociated due to receiving deauth packet: %02X-%02X-%02X-%02X-%02X-%02X\n", 
                                StaEntry->MacAddress[0], StaEntry->MacAddress[1], StaEntry->MacAddress[2], 
                                StaEntry->MacAddress[3], StaEntry->MacAddress[4], StaEntry->MacAddress[5]));
                    }

                    break;
                }
            }

            MP_RELEASE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);

            break;

        default:
            //
            // All other management packets are ignored.
            //
            break;
    }
}

NDIS_STATUS
StaEnumerateAssociationInfoAdHoc(
    _In_  PMP_EXTSTA_PORT        pStation,
    _Inout_updates_bytes_(TotalLength) PDOT11_ASSOCIATION_INFO_LIST   pAssocInfoList,
    _In_  ULONG           TotalLength
    )
{
    PSTA_ADHOC_STA_ENTRY    StaEntry = NULL;
    PDOT11_ASSOCIATION_INFO_EX  pAssocInfo = NULL;
    ULONG                   EntrySize;
    ULONG                   RemainingBytes = 0;
    NDIS_STATUS             ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                   StaCount = 0;
    PSTA_ADHOC_STA_ENTRY    *StaEntryArray = NULL;
    ULONG                   index;

    pAssocInfo = &(pAssocInfoList->dot11AssocInfo[0]);

    RemainingBytes = TotalLength 
        - FIELD_OFFSET(DOT11_ASSOCIATION_INFO_LIST, dot11AssocInfo);

    do
    {
        //
        // Find adhoc nodes that match our settings
        //
        ndisStatus = StaGetMatchingAdHocStaList(pStation, &StaCount, &StaEntryArray);
        if ((ndisStatus != NDIS_STATUS_SUCCESS) || (NULL == StaEntryArray)) 
        {
            // No adhoc stations found or failed when creating the list
            break;
        }

        for (index = 0; index < StaCount; index++) 
        {
            StaEntry = StaEntryArray[index];

            EntrySize = sizeof(DOT11_ASSOCIATION_INFO_EX);    // Only storing assoc info
            
            if (RemainingBytes >= EntrySize)
            {
                // Store the entry
                NdisZeroMemory(pAssocInfo, sizeof(DOT11_ASSOCIATION_INFO_EX));

                NdisMoveMemory(pAssocInfo->PeerMacAddress, StaEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));
                NdisMoveMemory(pAssocInfo->BSSID, StaEntry->Dot11BSSID, sizeof(DOT11_MAC_ADDRESS));
                pAssocInfo->usCapabilityInformation = StaEntry->Dot11Capability.usValue;
                pAssocInfo->usListenInterval = pStation->Config.ListenInterval;

                pAssocInfo->dot11AssociationState = StaEntry->AssocState;
                if (pAssocInfo->dot11AssociationState == dot11_assoc_state_zero)
                    pAssocInfo->dot11AssociationState = dot11_assoc_state_unauth_unassoc;
                    
                MpTrace(COMP_OID, DBG_LOUD, 
                        ("Assoc State For %02X-%02X-%02X-%02X-%02X-%02X is %d\n", 
                        pAssocInfo->PeerMacAddress[0], pAssocInfo->PeerMacAddress[1], 
                        pAssocInfo->PeerMacAddress[2], pAssocInfo->PeerMacAddress[3], 
                        pAssocInfo->PeerMacAddress[4], pAssocInfo->PeerMacAddress[5], 
                        pAssocInfo->dot11AssociationState));

                // TODO: Supported rates
                
                //
                // We do not keep per station statistics
                //                
                pAssocInfoList->uNumOfEntries++;        
                pAssocInfoList->uTotalNumOfEntries++;
                RemainingBytes -= EntrySize;
                pAssocInfo++;
            }
            else
            {
                // Not enough space to store this entry
                pAssocInfoList->uTotalNumOfEntries++;
                ndisStatus = NDIS_STATUS_BUFFER_OVERFLOW;
                RemainingBytes = 0;
                //
                // We continue walking through the list to determine the total
                // space required for this OID
                //            
            }
        }
    } while (FALSE);
    
    if (StaEntryArray) 
    {
        for (index = 0; index < StaCount; index++)
        {
            MP_FREE_MEMORY(StaEntryArray[index]);
        }

        MP_FREE_MEMORY(StaEntryArray);
    }    

    return ndisStatus;
}

void
StaAdHocReceiveDirectData(
    _In_  PMP_EXTSTA_PORT                    pStation,
    _In_  PDOT11_DATA_SHORT_HEADER    pDataHdr
    )
{
    BOOLEAN                         connected;
    PSTA_ADHOC_STA_INFO             AdHocStaInfo = &pStation->AdHocStaInfo;
    PSTA_ADHOC_STA_ENTRY            StaEntry;
    PLIST_ENTRY                     pHead;
    PLIST_ENTRY                     pEntry;
    MP_RW_LOCK_STATE                      LockState;
    
    //
    // If we are not in the connected state, return.
    //

    NdisAcquireSpinLock(&AdHocStaInfo->StaInfoLock);
    connected = (BOOLEAN)(AdHocStaInfo->AdHocState == STA_ADHOC_CONNECTED);
    NdisReleaseSpinLock(&AdHocStaInfo->StaInfoLock);

    if (!connected)
        return;

    //
    // Find if the station that sent this data frame also just sent us de-auth frame,
    // if so, the station must have tried to re-connect to the ad hoc network. In this
    // case, change its associate state to dot11_assoc_state_unauth_unassoc.
    //

    __try
    {
        MP_ACQUIRE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);

        if (AdHocStaInfo->DeauthStaCount == 0)
            __leave;

        pHead = &AdHocStaInfo->StaList;
        pEntry = pHead->Flink;
        while (pEntry != pHead) 
        {
            StaEntry = CONTAINING_RECORD(pEntry, STA_ADHOC_STA_ENTRY, Link);
            pEntry = pEntry->Flink;

            if (MP_COMPARE_MAC_ADDRESS(pDataHdr->Address2, StaEntry->MacAddress) &&
                StaEntry->AssocState == dot11_assoc_state_zero)
            {
                StaEntry->AssocState = dot11_assoc_state_unauth_unassoc;
                AdHocStaInfo->DeauthStaCount--;
                break;
            }
                
        }
    }
    __finally
    {
        MP_RELEASE_WRITE_LOCK(&AdHocStaInfo->StaListLock, &LockState);
    }
}
