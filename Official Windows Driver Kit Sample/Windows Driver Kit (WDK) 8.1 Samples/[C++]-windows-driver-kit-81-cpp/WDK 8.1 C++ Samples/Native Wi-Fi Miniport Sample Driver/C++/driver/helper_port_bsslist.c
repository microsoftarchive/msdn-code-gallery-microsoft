/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    helper_port_bsslist.c

Abstract:
    Implements BSS list management functionality for the helper port
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "port_defs.h"
#include "base_port_intf.h"
#include "helper_port_defs.h"
#include "helper_port_intf.h"
#include "helper_port_bsslist.h"
#include "vnic_intf.h"
#include "glb_utils.h"

#if DOT11_TRACE_ENABLED
#include "helper_port_bsslist.tmh"
#endif


NDIS_STATUS
HelperPortInitializeBSSList(
    _In_  PMP_HELPER_PORT         HelperPort
    )
{
    PMP_BSS_LIST                BSSList = &(HelperPort->BSSList);
    NDIS_STATUS                 ndisStatus;
    
    ndisStatus = MP_ALLOCATE_READ_WRITE_LOCK(&(BSSList->ListLock), HELPPORT_GET_MP_PORT(HelperPort)->MiniportAdapterHandle);
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        return ndisStatus;
   }

    BSSList->MaxNumOfBSSEntries = HelperPort->RegInfo->BSSEntryMaxCount;
    BSSList->NumOfBSSEntries = 0;
    NdisInitializeListHead(&BSSList->List);

    return NDIS_STATUS_SUCCESS;
}

VOID
HelperPortTerminateBSSList(
    _In_  PMP_HELPER_PORT         HelperPort
    )
{
    PMP_BSS_LIST                BSSList = &(HelperPort->BSSList);

    MPASSERT(IsListEmpty(&(BSSList->List)));
    
    MP_FREE_READ_WRITE_LOCK(&(BSSList->ListLock));
}

PMP_BSS_LIST
HelperPortQueryAndRefBSSList(
    _In_  PMP_PORT                Port,
    _In_  PMP_PORT                RequestingPort,
    _Out_ PMP_RW_LOCK_STATE       LockState
    )
{
    PMP_HELPER_PORT     HelperPort = MP_GET_HELPPORT(Port);

    UNREFERENCED_PARAMETER(RequestingPort);

    // Acquire read lock on the BSS list
    MP_ACQUIRE_READ_LOCK(&(HelperPort->BSSList.ListLock), LockState);

    return &(HelperPort->BSSList);
}

VOID
HelperPortReleaseBSSListRef(
    _In_  PMP_PORT                Port,
    _In_  PMP_BSS_LIST            BSSList,
    _Out_  PMP_RW_LOCK_STATE       LockState
    )
{
    UNREFERENCED_PARAMETER(Port);

    // Release the read ref
    MP_RELEASE_READ_LOCK(&(BSSList->ListLock), LockState);
}

NDIS_STATUS
HelperPortFlushBSSList(
    _In_ PMP_PORT                 Port
    )
{
    MP_RW_LOCK_STATE          LockState;
    PLIST_ENTRY         pListEntry;
    PMP_BSS_ENTRY      pBSSEntry = NULL;
    LONG                APRefCount;
    LIST_ENTRY          TempList;
    PMP_HELPER_PORT     HelperPort = MP_GET_HELPPORT(Port);
    PMP_BSS_LIST       pDiscoveredBSSList = &(HelperPort->BSSList);

    //
    // Entries that are currently in use (eg for connection)
    // we cannot flush and instead would put in the temporary queue
    //
    InitializeListHead(&TempList);
    
    MP_ACQUIRE_WRITE_LOCK(&(HelperPort->BSSList.ListLock), &LockState);
        
    while (!IsListEmpty(&(pDiscoveredBSSList->List)))
    {
        pListEntry = RemoveHeadList(&(pDiscoveredBSSList->List));
        pBSSEntry = CONTAINING_RECORD(pListEntry, MP_BSS_ENTRY, Link);

        APRefCount = NdisInterlockedDecrement(&(pBSSEntry->RefCount));
        if (APRefCount == 0)
        {        
            NdisAcquireSpinLock(&(pBSSEntry->Lock));
            MPASSERT(pBSSEntry->pAssocRequest == NULL);
            MPASSERT(pBSSEntry->pAssocResponse == NULL);
            
            if (pBSSEntry->pDot11BeaconFrame != NULL)
            {
                MP_FREE_MEMORY(pBSSEntry->pDot11BeaconFrame);
                pBSSEntry->pDot11BeaconFrame = NULL;
                pBSSEntry->BeaconFrameSize = 0;
                pBSSEntry->MaxBeaconFrameSize= 0;
            }

            if (pBSSEntry->pDot11ProbeFrame != NULL)
            {
                MP_FREE_MEMORY(pBSSEntry->pDot11ProbeFrame);
                pBSSEntry->pDot11ProbeFrame = NULL;
                pBSSEntry->ProbeFrameSize = 0;
                pBSSEntry->MaxProbeFrameSize= 0;
            }
            
            pBSSEntry->pDot11InfoElemBlob = NULL;
            pBSSEntry->InfoElemBlobSize = 0;            
            NdisReleaseSpinLock(&(pBSSEntry->Lock));
            
            MP_FREE_MEMORY(pBSSEntry);        
        }
        else
        {
            // Restore refcount and save for adding back to list
            NdisInterlockedIncrement(&(pBSSEntry->RefCount));
            InsertTailList(&TempList, pListEntry);
        }
    }
    pDiscoveredBSSList->NumOfBSSEntries = 0;

    //
    // Restore entries that are in use
    //
    while (!IsListEmpty(&TempList))
    {
        pListEntry = RemoveHeadList(&TempList);

        InsertTailList(&(pDiscoveredBSSList->List), pListEntry);
        pDiscoveredBSSList->NumOfBSSEntries++;
    }

    // Since our scan list is flushed, also clear the last scan time
    HelperPort->ScanContext.LastScanTime = 0;
    
    MP_RELEASE_WRITE_LOCK(&(HelperPort->BSSList.ListLock), &LockState);

    return NDIS_STATUS_SUCCESS;
}


#if DBG
NDIS_STATUS
HelperPortCheckForExtraRef(
    _In_ PMP_HELPER_PORT         HelperPort
    )
{
    MP_RW_LOCK_STATE          LockState;
    PLIST_ENTRY         pListEntry;
    PMP_BSS_ENTRY      pBSSEntry = NULL;
    PMP_BSS_LIST       pDiscoveredBSSList = &(HelperPort->BSSList);
    LIST_ENTRY          TempList;

    //
    // Entries that are currently in use (eg for connection)
    // we cannot flush and instead would put in the temporary queue
    //
    InitializeListHead(&TempList);
    
    MP_ACQUIRE_WRITE_LOCK(&(HelperPort->BSSList.ListLock), &LockState);
        
    while (!IsListEmpty(&(pDiscoveredBSSList->List)))
    {
        pListEntry = RemoveHeadList(&(pDiscoveredBSSList->List));
        pBSSEntry = CONTAINING_RECORD(pListEntry, MP_BSS_ENTRY, Link);

        //
        // Check that the BSS entry does not have an extra ref count
        //
        MPASSERT(pBSSEntry->RefCount <= 1);
        InsertTailList(&TempList, pListEntry);
    }
    pDiscoveredBSSList->NumOfBSSEntries = 0;

    //
    // Restore entries that are in use
    //
    while (!IsListEmpty(&TempList))
    {
        pListEntry = RemoveHeadList(&TempList);

        InsertTailList(&(pDiscoveredBSSList->List), pListEntry);
        pDiscoveredBSSList->NumOfBSSEntries++;
    }
    
    MP_RELEASE_WRITE_LOCK(&(HelperPort->BSSList.ListLock), &LockState);

    return NDIS_STATUS_SUCCESS;
}
#endif

NDIS_STATUS 
HelperPortReceiveEventHandler(
    _In_  PMP_PORT                Port,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_MAC_HEADER   packetHeader;
    PMP_RX_MPDU         rxFragment;
    USHORT              fragmentSize;
    PMP_RX_MSDU         currentPacket = PacketList;

    UNREFERENCED_PARAMETER(ReceiveFlags);
    
    // Process each of the packets internally
    while (currentPacket != NULL)
    {
        // We only accept 1 fragment
        rxFragment = MP_RX_MSDU_MPDU_AT(currentPacket, 0);    
        packetHeader = MP_RX_MPDU_DATA(rxFragment);
        fragmentSize = (USHORT)MP_RX_MPDU_LENGTH(rxFragment);
        
        switch(packetHeader->FrameControl.Type)
        {
            case DOT11_FRAME_TYPE_MANAGEMENT:
                //
                // Process management packet
                //
                ndisStatus = HelperPortReceiveMgmtPacket(
                    MP_GET_HELPPORT(Port),
                    rxFragment,
                    fragmentSize
                    );
                break;
                
            default:
                break;
        }

        //
        // Set the status. This determines if this packet should
        // be forwarded up or not
        //
        MP_RX_MSDU_STATUS(currentPacket) = ndisStatus;

        // Next packet
        currentPacket = MP_RX_MSDU_NEXT_MSDU(currentPacket);
    }

    return NDIS_STATUS_SUCCESS;
}





NDIS_STATUS
HelperPortReceiveMgmtPacket(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_RX_MPDU             pNicFragment,
    _In_  USHORT                  FragmentSize
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_MGMT_HEADER  pMgmtPktHeader;
    
    // This is a management packet, reject if size isnt right
    if (FragmentSize < sizeof(DOT11_MGMT_HEADER))
    {
        return NDIS_STATUS_NOT_ACCEPTED;
    }

    pMgmtPktHeader = MP_RX_MPDU_DATA(pNicFragment);
    switch(pMgmtPktHeader->FrameControl.Subtype)
    {
        case DOT11_MGMT_SUBTYPE_BEACON:
        case DOT11_MGMT_SUBTYPE_PROBE_RESPONSE:
            HelperPortReceiveBeacon(
                HelperPort,
                pNicFragment,
                FragmentSize
                );
            break;

        default:
            break;
    }

    return ndisStatus;    
}


VOID 
HelperPortReceiveBeacon(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_RX_MPDU             pFragment,
    _In_  ULONG                   TotalLength
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

        // Get/Validate beacon is okay
        pDot11BeaconFrame = (PDOT11_BEACON_FRAME)(pPacketBuffer + sizeof(DOT11_MGMT_HEADER));
        ndisStatus = HelperPortValidateBeacon(pDot11BeaconFrame);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

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

        // Save information from this beacon into our BSS list
        ndisStatus = HelperPortSaveBSSInformation(
            HelperPort,
            pFragment,
            pDot11BeaconFrame,
            (uInfoElemBlobSize + FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements))    // Info elements + header
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }
    } while (FALSE);
}




__inline NDIS_STATUS HelperPortValidateBeacon(
    _In_  PDOT11_BEACON_FRAME     pDot11BeaconFrame
    )
{
    UNREFERENCED_PARAMETER(pDot11BeaconFrame);
    return NDIS_STATUS_SUCCESS;
}


// Write lock acquired
__inline VOID
HelperPortAddBSSEntry(
    _In_  PMP_BSS_LIST           pBSSList,
    _In_  PMP_BSS_ENTRY          pBSSEntry
    )
{
    InsertTailList(&(pBSSList->List), &(pBSSEntry->Link));
    pBSSList->NumOfBSSEntries++;
}

// Write lock acquired
__inline VOID
HelperPortRemoveBSSEntry(
    _In_  PMP_BSS_LIST           pBSSList,
    _In_  PMP_BSS_ENTRY          pBSSEntry
    )
{
    RemoveEntryList(&(pBSSEntry->Link));
    pBSSList->NumOfBSSEntries--;
}

// Read Lock must be held
PMP_BSS_ENTRY
HelperPortFindBSSEntry(
    _In_  PMP_BSS_LIST           pBSSList,
    _In_  const unsigned char *   srcAddress
    )
{
    PLIST_ENTRY         pHead = NULL, pEntry = NULL;
    PMP_BSS_ENTRY      pBSSEntry = NULL;
    BOOLEAN             bFound = FALSE;

    pHead = &(pBSSList->List);
    pEntry = pHead->Flink;
    while(pEntry != pHead) 
    {
        pBSSEntry = CONTAINING_RECORD(pEntry, MP_BSS_ENTRY, Link);
        pEntry = pEntry->Flink;

        NdisAcquireSpinLock(&(pBSSEntry->Lock));
        if (MP_COMPARE_MAC_ADDRESS(pBSSEntry->MacAddress, srcAddress)) 
        {
            NdisReleaseSpinLock(&(pBSSEntry->Lock));
            bFound = TRUE;
            break;
        }
        NdisReleaseSpinLock(&(pBSSEntry->Lock));
    }

    return ((bFound)? pBSSEntry: NULL);

}

// Write lock on the list must be held
// Returns an entry that can be expired. The caller frees it
PMP_BSS_ENTRY
HelperPortExpireBSSEntry(
    _In_  PMP_BSS_LIST           pBSSList,
    _In_  ULONGLONG               ullMaxActiveTime,
    _In_  ULONGLONG               ullExpireTimeStamp
    )
{
    PLIST_ENTRY         pHead = NULL, pEntry = NULL;
    PMP_BSS_ENTRY      pBSSEntry = NULL;
    BOOLEAN             bFound = FALSE;

    //
    // We can expire an entry that has been around for longer 
    // than this time
    //
    if (ullMaxActiveTime <= ullExpireTimeStamp)
        ullExpireTimeStamp -= ullMaxActiveTime;

    pHead = &(pBSSList->List);
    pEntry = pHead->Flink;
    while(pEntry != pHead) 
    {
        pBSSEntry = CONTAINING_RECORD(pEntry, MP_BSS_ENTRY, Link);
        pEntry = pEntry->Flink;

        NdisAcquireSpinLock(&(pBSSEntry->Lock));

        //
        // If the entry is older than we expected and its not in
        // use, we can expire it
        //
        if (pBSSEntry->HostTimestamp < ullExpireTimeStamp)
        {
            if (NdisInterlockedDecrement(&(pBSSEntry->RefCount)) == 0)
            {
                MpTrace(COMP_SCAN, DBG_LOUD, ("Expiring AP: %02X-%02X-%02X-%02X-%02X-%02X\n", 
                    pBSSEntry->Dot11BSSID[0], pBSSEntry->Dot11BSSID[1], pBSSEntry->Dot11BSSID[2], 
                    pBSSEntry->Dot11BSSID[3], pBSSEntry->Dot11BSSID[4], pBSSEntry->Dot11BSSID[5]));

                MPASSERT(pBSSEntry->pAssocRequest == NULL);
                MPASSERT(pBSSEntry->pAssocResponse == NULL);
                
                //
                // This is the entry we can expire. Remove it from the list.
                //
                NdisReleaseSpinLock(&(pBSSEntry->Lock));
                HelperPortRemoveBSSEntry(pBSSList, pBSSEntry);
                
                bFound = TRUE;
                break;
            }
            else
            {
                // Someone is using the entry, we cannot remove/delete this. Add back
                // a ref and we will delete later on

                // This is subobtimal. Ideally the last person to decrement
                // refcount should delete the entry and not us. Modify to remove
                // the entry from the list, decrement its refcount and only free
                // the memory if the refcount has gone to zero
                NdisInterlockedIncrement(&(pBSSEntry->RefCount));
            }
        }
        NdisReleaseSpinLock(&(pBSSEntry->Lock));
    }

    if (bFound != TRUE)
    {
        pBSSEntry = NULL;
    }
    
    return pBSSEntry;
}


NDIS_STATUS 
HelperPortInsertBSSEntry(
    _In_  PMP_HELPER_PORT        HelperPort,
    _In_  PMP_RX_MPDU        pFragment,
    _In_  PDOT11_BEACON_FRAME pDot11BeaconFrame,
    _In_  ULONG           BeaconDataLength
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    ULONGLONG           ullHostTimeStamp;
    MP_RW_LOCK_STATE          LockState;
    PMP_BSS_ENTRY      pBSSEntry = NULL;
    PDOT11_MGMT_HEADER  pMgmtPktHeader;
    BOOLEAN             bNewAp = FALSE;
    PMP_BSS_LIST       pDiscoveredBSSList = &(HelperPort->BSSList);

    pMgmtPktHeader = (PDOT11_MGMT_HEADER)MP_RX_MPDU_DATA(pFragment);
    NdisGetCurrentSystemTime((PLARGE_INTEGER)&ullHostTimeStamp);

    //
    // We acquire the write lock as we are adding entries to the list
    //
    MP_ACQUIRE_WRITE_LOCK(&(HelperPort->BSSList.ListLock), &LockState);

    do
    {
        //
        // Check again if this entry already exists in the list. This is to handle
        // if the AP was added since we first checked (possible if the
        // flush routine was running)
        //
        pBSSEntry = HelperPortFindBSSEntry(
            pDiscoveredBSSList,
            pMgmtPktHeader->SA
            );

        if (pBSSEntry == NULL)
        {
            bNewAp = TRUE;      // New AP
            //
            // We havent found this AP yet, we would add it to the list
            //
            if (pDiscoveredBSSList->NumOfBSSEntries >= pDiscoveredBSSList->MaxNumOfBSSEntries)
            {
                //
                // We need to replace an entry thats in the list
                //
                pBSSEntry = HelperPortExpireBSSEntry(
                    pDiscoveredBSSList,
                    HelperPort->RegInfo->BSSEntryExpireTime,
                    ullHostTimeStamp
                    );

                if (pBSSEntry != NULL)
                {
                    //
                    // Add initial in-use refcount
                    //
                    pBSSEntry->RefCount = 1;
                }
                
                //
                // Dont zero out the AP entry so that we can
                // reuse the info element blob
                //
            }
            else
            {
                //
                // Create a new entry for this AP
                //
                MP_ALLOCATE_MEMORY(HELPPORT_GET_MP_PORT(HelperPort)->MiniportAdapterHandle, 
                    &pBSSEntry, 
                    sizeof(MP_BSS_ENTRY), 
                    PORT_MEMORY_TAG
                    );
                if (pBSSEntry != NULL)
                {
                    //
                    // Initialize the new entry
                    //
                    NdisZeroMemory(pBSSEntry, sizeof(MP_BSS_ENTRY));
                    
                    pBSSEntry->RefCount = 1; // Add initial in-use refcount
                    NdisAllocateSpinLock(&(pBSSEntry->Lock));
                }
            }

            if (pBSSEntry == NULL)
            {
                MpTrace(COMP_SCAN, DBG_SERIOUS, ("Not enough space to add AP: %02X-%02X-%02X-%02X-%02X-%02X\n",
                    pMgmtPktHeader->SA[0], pMgmtPktHeader->SA[1], pMgmtPktHeader->SA[2], 
                    pMgmtPktHeader->SA[3], pMgmtPktHeader->SA[4], pMgmtPktHeader->SA[5]));
            
                ndisStatus = NDIS_STATUS_RESOURCES;
                break;
            }

            //
            // This Entry is not yet in the list
            //
            
            // We will be updating the beacon & probe response frames
            pBSSEntry->BeaconFrameSize = 0;
            pBSSEntry->ProbeFrameSize = 0;
            
            pBSSEntry->AssocCost = 0;

            NdisMoveMemory(
                pBSSEntry->Dot11BSSID,
                pMgmtPktHeader->BSSID,
                sizeof(DOT11_MAC_ADDRESS)
                );

            NdisMoveMemory(
                pBSSEntry->MacAddress,
                pMgmtPktHeader->SA,
                sizeof(DOT11_MAC_ADDRESS)
                );
        }

        // Update the information in this BSS entry (either new or reused entry)
        ndisStatus = HelperPortUpdateBSSEntry(
            HelperPort, 
            pBSSEntry, 
            pFragment, 
            pDot11BeaconFrame, 
            BeaconDataLength
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Add the new BSS to our list
        //
        if (bNewAp)
        {
            MpTrace(COMP_SCAN, DBG_LOUD, ("AP %02X-%02X-%02X-%02X-%02X-%02X at channel: %d (%d)\n", 
                    pBSSEntry->Dot11BSSID[0], pBSSEntry->Dot11BSSID[1], pBSSEntry->Dot11BSSID[2], 
                    pBSSEntry->Dot11BSSID[3], pBSSEntry->Dot11BSSID[4], pBSSEntry->Dot11BSSID[5],
                    pBSSEntry->Channel, pFragment->Msdu->Channel));

            HelperPortAddBSSEntry(pDiscoveredBSSList, pBSSEntry);
        }

        //
        // Note: If any code is added below here, remember to remove entry
        // from the list
        //
    } while (FALSE);

    MP_RELEASE_WRITE_LOCK(&(HelperPort->BSSList.ListLock), &LockState);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // Free the new entry we may have created
        if ((bNewAp) && (pBSSEntry != NULL))
        {
            if (pBSSEntry->pDot11BeaconFrame != NULL)
            {
                MP_FREE_MEMORY(pBSSEntry->pDot11BeaconFrame);
                pBSSEntry->pDot11BeaconFrame = NULL;
                pBSSEntry->BeaconFrameSize = 0;
                pBSSEntry->MaxBeaconFrameSize= 0;
            }

            if (pBSSEntry->pDot11ProbeFrame != NULL)
            {
                MP_FREE_MEMORY(pBSSEntry->pDot11ProbeFrame);
                pBSSEntry->pDot11ProbeFrame = NULL;
                pBSSEntry->ProbeFrameSize = 0;
                pBSSEntry->MaxProbeFrameSize= 0;
            }
                        
            pBSSEntry->pDot11InfoElemBlob = NULL;                
            pBSSEntry->InfoElemBlobSize = 0;
            MP_FREE_MEMORY(pBSSEntry);
        }
    }

    return ndisStatus;
}

// BSS list lock acquired & called at Dispatch
NDIS_STATUS 
HelperPortUpdateBSSEntry(
    _In_  PMP_HELPER_PORT        HelperPort,
    _In_  PMP_BSS_ENTRY  pBSSEntry,
    _In_  PMP_RX_MPDU        pFragment,
    _In_  PDOT11_BEACON_FRAME pDot11BeaconPRFrame,
    _In_  ULONG           BeaconPRDataLength
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PDOT11_MGMT_HEADER  pMgmtPktHeader;
    ULONGLONG           ullHostTimeStamp;
    PVOID               pSavedBeaconPRBuffer = NULL;
    ULONG               uOffsetOfInfoElemBlob = FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);
    UCHAR               channel;
    DOT11_PHY_TYPE      PhyType;

    pMgmtPktHeader = (PDOT11_MGMT_HEADER)MP_RX_MPDU_DATA(pFragment);
    NdisGetCurrentSystemTime((PLARGE_INTEGER)&ullHostTimeStamp);
    
    do
    {
        // 
        // Modifying data in the AP entry
        //
        NdisDprAcquireSpinLock(&(pBSSEntry->Lock));

        if (pDot11BeaconPRFrame->Capability.IBSS)
        {
            pBSSEntry->Dot11BSSType = dot11_BSS_type_independent;
        }
        else
        {   
            pBSSEntry->Dot11BSSType = dot11_BSS_type_infrastructure;
        }
        
        //
        // Adhoc station can leave adhoc cell and create a new cell. SoftAPs
        // can move. This means the BSSID can change
        //
        NdisMoveMemory(
            pBSSEntry->Dot11BSSID,
            pMgmtPktHeader->BSSID,
            sizeof(DOT11_MAC_ADDRESS)
            );

        pBSSEntry->HostTimestamp = ullHostTimeStamp;
        pBSSEntry->BeaconTimestamp = pDot11BeaconPRFrame->Timestamp;
        pBSSEntry->BeaconInterval = pDot11BeaconPRFrame->BeaconInterval;
        pBSSEntry->Dot11Capability = pDot11BeaconPRFrame->Capability;
        pBSSEntry->RSSI = pFragment->Msdu->RecvContext.lRSSI;
        pBSSEntry->LinkQuality = pFragment->Msdu->LinkQuality;
        pBSSEntry->ChannelCenterFrequency 
            = pFragment->Msdu->RecvContext.uChCenterFrequency;

        //
        // If signal strength was below our threshold, catch that
        //
        if (pBSSEntry->LinkQuality < HelperPort->RegInfo->RSSILinkQualityThreshold)
        {
            pBSSEntry->LowQualityCount++;
        }
        else
        {
            pBSSEntry->LowQualityCount = 0;
        }

#if 0
        if (pBSSEntry->AssocState == dot11_assoc_state_auth_assoc)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Received beacon from associated AP: %02X-%02X-%02X-%02X-%02X-%02X\n",
                pMgmtPktHeader->SA[0], pMgmtPktHeader->SA[1], pMgmtPktHeader->SA[2], 
                pMgmtPktHeader->SA[3], pMgmtPktHeader->SA[4], pMgmtPktHeader->SA[5]));
        }
#endif

        //
        // Get channel number at which the frame was received.
        //
        if (Dot11GetChannelForDSPhy(Add2Ptr(pDot11BeaconPRFrame, uOffsetOfInfoElemBlob),
                                    BeaconPRDataLength - uOffsetOfInfoElemBlob, 
                                    &channel) != NDIS_STATUS_SUCCESS)
        {
            channel = pFragment->Msdu->Channel;
        }

        if (channel != 0)
        {
            pBSSEntry->Channel = channel;
        }

        //
        // Get PhyType and PhyId
        //
        PhyType = VNic11DeterminePHYType(HELPPORT_GET_VNIC(HelperPort), 
                                       pBSSEntry->Dot11Capability,
                                       pBSSEntry->Channel);
        if (pBSSEntry->Dot11PhyType != PhyType)
        {
            pBSSEntry->Dot11PhyType = PhyType;
            pBSSEntry->PhyId = BasePortGetPhyIdFromType(HELPPORT_GET_MP_PORT(HelperPort), PhyType);
        }

        if (pMgmtPktHeader->FrameControl.Subtype == DOT11_MGMT_SUBTYPE_BEACON)
        {
            //
            // Increase the beacon frame size if necessary
            //
            if (pBSSEntry->MaxBeaconFrameSize < BeaconPRDataLength)
            {
                MP_ALLOCATE_MEMORY(HELPPORT_GET_MP_PORT(HelperPort)->MiniportAdapterHandle, 
                    &pSavedBeaconPRBuffer, 
                    BeaconPRDataLength,
                    PORT_MEMORY_TAG
                    );
                    
                if (pSavedBeaconPRBuffer == NULL)
                {
                    //
                    // Unable to allocate memory for information elements.
                    // If this is a new AP entry, we wont be adding it to the list.
                    // For existing entries, we end up ignoring the new IE blob
                    //
                    ndisStatus = NDIS_STATUS_RESOURCES;
                    NdisDprReleaseSpinLock(&(pBSSEntry->Lock));
                    break;
                }
                //
                // Delete any old blob buffer
                //
                if (pBSSEntry->pDot11BeaconFrame != NULL)
                {
                    MP_FREE_MEMORY(pBSSEntry->pDot11BeaconFrame);
                }

                pBSSEntry->pDot11BeaconFrame = pSavedBeaconPRBuffer;
                pBSSEntry->MaxBeaconFrameSize = BeaconPRDataLength;
            }

            // Update the beacon
            pBSSEntry->BeaconFrameSize = BeaconPRDataLength;
            
            // Also save this as the IE blob pointer
            pBSSEntry->InfoElemBlobSize = BeaconPRDataLength - uOffsetOfInfoElemBlob;
            pBSSEntry->pDot11InfoElemBlob = (PUCHAR)pBSSEntry->pDot11BeaconFrame + uOffsetOfInfoElemBlob;
            
            //
            // Update/Save the beacon information element block
            //
            NdisMoveMemory(
                pBSSEntry->pDot11BeaconFrame,
                pDot11BeaconPRFrame,
                BeaconPRDataLength
                );
        }
    
        if (pMgmtPktHeader->FrameControl.Subtype == DOT11_MGMT_SUBTYPE_PROBE_RESPONSE)
        {
            //
            // Increase the probe response frame size if necessary
            //
            if (pBSSEntry->MaxProbeFrameSize < BeaconPRDataLength)
            {
                MP_ALLOCATE_MEMORY(HELPPORT_GET_MP_PORT(HelperPort)->MiniportAdapterHandle, 
                    &pSavedBeaconPRBuffer, 
                    BeaconPRDataLength,
                    PORT_MEMORY_TAG
                    );
                    
                if (pSavedBeaconPRBuffer == NULL)
                {
                    //
                    // Unable to allocate memory for information elements.
                    // If this is a new AP entry, we wont be adding it to the list.
                    // For existing entries, we end up ignoring the new IE blob
                    //
                    ndisStatus = NDIS_STATUS_RESOURCES;
                    NdisDprReleaseSpinLock(&(pBSSEntry->Lock));
                    break;
                }
                //
                // Delete any old blob buffer
                //
                if (pBSSEntry->pDot11ProbeFrame != NULL)
                {
                    MP_FREE_MEMORY(pBSSEntry->pDot11ProbeFrame);
                }

                pBSSEntry->pDot11ProbeFrame = pSavedBeaconPRBuffer;
                pBSSEntry->MaxProbeFrameSize = BeaconPRDataLength;
            }
            
            pBSSEntry->ProbeFrameSize = BeaconPRDataLength;

            // Also save this as the IE blob pointer
            pBSSEntry->InfoElemBlobSize = BeaconPRDataLength - uOffsetOfInfoElemBlob;
            pBSSEntry->pDot11InfoElemBlob = (PUCHAR)pBSSEntry->pDot11ProbeFrame + uOffsetOfInfoElemBlob;

            //
            // Update/Save the beacon information element block
            //
            NdisMoveMemory(
                pBSSEntry->pDot11ProbeFrame,
                pDot11BeaconPRFrame,
                BeaconPRDataLength
                );

        }

#if 0
        if (pBSSEntry->AssocState == dot11_assoc_state_auth_assoc)
        {
            MpTrace(COMP_SCAN, DBG_LOUD, ("Received %d for AP %02X-%02X-%02X-%02X-%02X-%02X \n", 
                    pMgmtPktHeader->FrameControl.Subtype,
                    pBSSEntry->Dot11BSSID[0], pBSSEntry->Dot11BSSID[1], pBSSEntry->Dot11BSSID[2], 
                    pBSSEntry->Dot11BSSID[3], pBSSEntry->Dot11BSSID[4], pBSSEntry->Dot11BSSID[5]));
        }
#endif

        //
        // Done with our modification of the AP entry
        //
        NdisDprReleaseSpinLock(&(pBSSEntry->Lock));
        
    } while (FALSE);

    return ndisStatus;
}

NDIS_STATUS 
HelperPortSaveBSSInformation(
    _In_  PMP_HELPER_PORT        HelperPort,
    _In_  PMP_RX_MPDU        pFragment,
    _In_  PDOT11_BEACON_FRAME pDot11BeaconFrame,
    _In_  ULONG           BeaconDataLength
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    MP_RW_LOCK_STATE          LockState;
    PMP_BSS_ENTRY      pBSSEntry = NULL;
    PDOT11_MGMT_HEADER  pMgmtPktHeader;
    PMP_BSS_LIST       pDiscoveredBSSList = &(HelperPort->BSSList);

    pMgmtPktHeader = (PDOT11_MGMT_HEADER)MP_RX_MPDU_DATA(pFragment);

    //
    // In most cases, we would be updating information about this
    // AP in our list. For this reason, we only acquire read lock
    // for most cases
    //
    MP_ACQUIRE_READ_LOCK(&(HelperPort->BSSList.ListLock), &LockState);

    //
    // See if this entry already exists in the list
    //
    pBSSEntry = HelperPortFindBSSEntry(
        pDiscoveredBSSList,
        pMgmtPktHeader->SA
        );

    if (pBSSEntry == NULL)
    {
        //
        // Entry does not exist, we are adding information about a new BSS
        //

        MP_RELEASE_READ_LOCK(&(HelperPort->BSSList.ListLock), &LockState);

        ndisStatus = HelperPortInsertBSSEntry(
            HelperPort, 
            pFragment, 
            pDot11BeaconFrame, 
            BeaconDataLength
            );
        
    }
    else
    {
        //
        // Entry already exists, we are just updating
        //
        
        ndisStatus = HelperPortUpdateBSSEntry(
            HelperPort, 
            pBSSEntry,
            pFragment, 
            pDot11BeaconFrame, 
            BeaconDataLength
            );  // Read lock still held

        MP_RELEASE_READ_LOCK(&(HelperPort->BSSList.ListLock), &LockState);
    }

    return ndisStatus;
}


