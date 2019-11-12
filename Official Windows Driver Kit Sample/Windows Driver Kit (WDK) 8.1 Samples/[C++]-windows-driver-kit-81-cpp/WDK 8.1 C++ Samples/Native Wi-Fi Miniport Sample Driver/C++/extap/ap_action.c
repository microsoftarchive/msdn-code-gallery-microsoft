/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_action.c

Abstract:
    Implements ExtAP actions such as start, stop
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    10-01-2007    Created

Notes:

--*/
#include "precomp.h"
    
#if DOT11_TRACE_ENABLED
#include "ap_action.tmh"
#endif

//
// ported from StaAttachAdHocRSNIE
//
_At_(*pIESize, _Pre_ _In_range_(>=, sizeof(DOT11_INFO_ELEMENT) +
        sizeof(USHORT) +
        sizeof(ULONG) + 
        sizeof(USHORT) + sizeof(ULONG) +
        sizeof(USHORT) + sizeof(ULONG) +
        sizeof(USHORT)))
_At_(*ppCurrentIE, _Writes_and_advances_ptr_(*pIESize))
NDIS_STATUS
ApAttachRSNIE(
    _In_ PMP_EXTAP_PORT ApPort,
    _Inout_ PUCHAR *ppCurrentIE,
    _Inout_ PUSHORT pIESize        
    )
{
    USHORT      size;
    PVNIC vnic = AP_GET_VNIC(ApPort);

    ASSERT(AP_GET_AUTH_ALGO(ApPort) == DOT11_AUTH_ALGO_RSNA_PSK);

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
    *((ULONG UNALIGNED *)(*ppCurrentIE)) = Dot11GetRSNOUIFromCipher(RSNA_OUI_PREFIX, AP_GET_MULTICAST_CIPHER_ALGO(ApPort));
    *ppCurrentIE += sizeof(ULONG);

    //
    // Set pairwise cipher
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = 1;
    *ppCurrentIE += sizeof(USHORT);
    *((ULONG UNALIGNED *)(*ppCurrentIE)) = Dot11GetRSNOUIFromCipher(RSNA_OUI_PREFIX, AP_GET_UNICAST_CIPHER_ALGO(ApPort));
    *ppCurrentIE += sizeof(ULONG);

    //
    // Set AKM suite
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = 1;
    *ppCurrentIE += sizeof(USHORT);
    *((ULONG UNALIGNED *)(*ppCurrentIE)) = Dot11GetRSNOUIFromAuthAlgo(AP_GET_AUTH_ALGO(ApPort));
    *ppCurrentIE += sizeof(ULONG);

    //
    // Set capability. Get the capability from hardware layer.
    //
    *((USHORT UNALIGNED *)(*ppCurrentIE)) = VNic11QueryRSNCapabilityField(vnic);
    *ppCurrentIE += sizeof(USHORT);

    //
    // Update remaining IE size.
    //
    *pIESize = *pIESize - size;

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
ConstructAPIEBlob(
    _In_ PMP_EXTAP_PORT ApPort,
    PUCHAR ieBlobPtr,
    BOOLEAN bResponseIE,
    USHORT *pIEBlobSize,
    USHORT *pbytesWritten
)
{
    PUCHAR ieBlobStart = ieBlobPtr;
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    DOT11_PHY_TYPE phyType;
    UCHAR channel;
    PVNIC vnic = AP_GET_VNIC(ApPort);
    PVOID pAdditionalIEData = NULL;
    ULONG uAdditionalIESize;
    BOOLEAN isPreferredChannel;

    *pbytesWritten = 0;
    
    do
    {
        //
        // Add SSID.
        //

        ndisStatus = Dot11AttachElement(
                        &ieBlobPtr,
                        pIEBlobSize,
                        DOT11_INFO_ELEMENT_ID_SSID,
                        (UCHAR)(AP_GET_SSID(ApPort).uSSIDLength),
                        AP_GET_SSID(ApPort).ucSSID
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): Failed to attach SSID IE. Status = 0x%08x.", 
                                    AP_GET_PORT_NUMBER(ApPort),
                                    ndisStatus));
            break;
        }

        //
        // Add basic rate set.
        //

        ndisStatus = Dot11AttachElement(
                        &ieBlobPtr,
                        pIEBlobSize,
                        DOT11_INFO_ELEMENT_ID_SUPPORTED_RATES,
                        (UCHAR)((AP_GET_OPERATIONAL_RATE_SET(ApPort).uRateSetLength > 8) ? 8 : AP_GET_OPERATIONAL_RATE_SET(ApPort).uRateSetLength),
                        AP_GET_OPERATIONAL_RATE_SET(ApPort).ucRateSet
                        );
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): Failed to attach rate set IE. Status = 0x%08x.", 
                                    AP_GET_PORT_NUMBER(ApPort),
                                    ndisStatus));
            break;
        }

        if (AP_GET_OPERATIONAL_RATE_SET(ApPort).uRateSetLength > (UCHAR)8) 
        {
            ndisStatus = Dot11AttachElement(
                            &ieBlobPtr,
                            pIEBlobSize,
                            DOT11_INFO_ELEMENT_ID_EXTD_SUPPORTED_RATES,
                            (UCHAR)(AP_GET_OPERATIONAL_RATE_SET(ApPort).uRateSetLength - 8),
                            AP_GET_OPERATIONAL_RATE_SET(ApPort).ucRateSet + 8
                            );
            
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): Failed to attach rate set IE. Status = 0x%08x.", 
                                    AP_GET_PORT_NUMBER(ApPort),
                                    ndisStatus));
                break;
            }
        }

        //
        // Add PHY specific IEs
        //

        phyType = VNic11QueryCurrentPhyType(vnic);
        channel = (UCHAR)VNic11QueryPreferredChannel(vnic, &isPreferredChannel);

        if (!isPreferredChannel)
        {
            channel = 11;

        }
        
        switch (phyType)
        {
            case dot11_phy_type_erp:
            case dot11_phy_type_hrdsss:
            case dot11_phy_type_dsss:

                //
                // Attach DSSS IE
                //
                
                ndisStatus = Dot11AttachElement(
                                &ieBlobPtr,
                                pIEBlobSize,
                                DOT11_INFO_ELEMENT_ID_DS_PARAM_SET,
                                sizeof(channel),
                                &channel
                                );
                
                if (ndisStatus != NDIS_STATUS_SUCCESS)
                {
                    MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): Failed to attach DSSS IE. Status = 0x%08x.", 
                                        AP_GET_PORT_NUMBER(ApPort),
                                        ndisStatus));
                    break;
                }

                break;

            case dot11_phy_type_fhss:

                //
                // Attach FHSS IE
                //
#if 0                
                if (StaEntry) 
                {
                    ndisStatus = Dot11CopyInfoEle(StaEntry->InfoElemBlobPtr,
                                                  StaEntry->InfoElemBlobSize,
                                                  DOT11_INFO_ELEMENT_ID_FH_PARAM_SET,
                                                  &size,
                                                  sizeof(FHSSIE),
                                                  &FHSSIE);

                    iePresent = (ndisStatus == NDIS_STATUS_SUCCESS && size == sizeof(FHSSIE)) ? TRUE: FALSE;
                } 
                else 
                {
                    iePresent = FALSE;
                }

                if (iePresent) 
                {
                    ndisStatus = Dot11AttachElement(&ieBlobPtr,
                                                    pIEBlobSize,
                                                    DOT11_INFO_ELEMENT_ID_FH_PARAM_SET,
                                                    sizeof(FHSSIE),
                                                    (PVOID)&FHSSIE);
                    if (ndisStatus != NDIS_STATUS_SUCCESS)
                        __leave;
                }
      
#endif
                break;
            case dot11_phy_type_ofdm:
                break;

            case dot11_phy_type_irbaseband:
                break;

            default:
                break;
        }

#if 0
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

            if (ndisStatus == NDIS_STATUS_SUCCESS && size == sizeof(ATIMWindow)) 
            {
                ndisStatus = VNic11SetATIMWindow(vnic, (ULONG)ATIMWindow);
                if (ndisStatus != NDIS_STATUS_SUCCESS)
                    __leave;
            }
        }
#endif

        //
        // If we are running RSNA_PSK, add RSN IE.
        //
        if (DOT11_AUTH_ALGO_RSNA_PSK == AP_GET_AUTH_ALGO(ApPort))
        {
            //
            // Tell hardware layer what group cipher we use.
            //
            VNic11SetCipher(vnic, FALSE, AP_GET_MULTICAST_CIPHER_ALGO(ApPort));

            ndisStatus = ApAttachRSNIE(
                 ApPort, 
                 &ieBlobPtr,
                 pIEBlobSize
                 );
            if (ndisStatus != NDIS_STATUS_SUCCESS)
                break;
        }

        //
        // Add any additional IEs if we still have space.
        // No need to hold a lock on the AP configuration because OIDs are serialized.
        //
        if (! bResponseIE)
        {
            // include additional beacon IE
            pAdditionalIEData = AP_GET_BEACON_IE(ApPort);
            uAdditionalIESize = AP_GET_BEACON_IE_SIZE(ApPort);
        }
        else
        {
            // include probe response IE
            pAdditionalIEData = AP_GET_PROBE_RESPONSE_IE(ApPort);
            uAdditionalIESize = AP_GET_PROBE_RESPONSE_IE_SIZE(ApPort);
        }

        if (pAdditionalIEData != NULL && 
            uAdditionalIESize != 0 &&
            uAdditionalIESize <= *pIEBlobSize
            )
        {
            NdisMoveMemory(
                ieBlobPtr, 
                pAdditionalIEData,
                uAdditionalIESize
                );
            ieBlobPtr += uAdditionalIESize;
            *pIEBlobSize = *pIEBlobSize - ((USHORT)uAdditionalIESize);
        }

        *pbytesWritten = (USHORT)PtrOffset(ieBlobStart, ieBlobPtr);
    } while (FALSE);

    return ndisStatus;
}

/** Start AP */
NDIS_STATUS
StartExtAp(
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC vnic = AP_GET_VNIC(ApPort);
    PMP_BSS_DESCRIPTION bssDescription = NULL;
//    DOT11_CAPABILITY dot11Capability = {0};
//    BOOLEAN set;
//    DOT11_PHY_TYPE phyType;
    PUCHAR ieBlobPtr;
    USHORT ieBlobSize, blobBytesWritten = 0;
    PUCHAR ieBlobPtr2;
//    PUCHAR tmpPtr;
//    UCHAR size;
//    DOT11_RATE_SET rateSet;
//    ULONG index;
    UCHAR channel;
//    USHORT ATIMWindow;
//    STA_FHSS_IE FHSSIE = {0};
//    BOOLEAN iePresent;
    BOOLEAN notifyCompletion = FALSE;           // need to notify VNIC that the connection is complete
    BOOLEAN CtxSBarrierAcquired = FALSE;
    BOOLEAN preferredChannelSet;
    
    MPASSERT(ApGetState(ApPort) == AP_STATE_STOPPED);
    do 
    {
        //
        // Set AP start to starting
        //
        ApSetState(ApPort, AP_STATE_STARTING);
        

        //
        // start association manager
        //
        ndisStatus = ApStartAssocMgr(AP_GET_ASSOC_MGR(ApPort));

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to start association manager. Status = 0x%08x.", 
                                AP_GET_PORT_NUMBER(ApPort), ndisStatus));
            break;
        }

        //
        // Allocate a BSS description structure with maximum possible IE field.
        //

        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(ApPort),
            &bssDescription, 
            FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + AP11_MAX_IE_BLOB_SIZE * 2,
            EXTAP_MEMORY_TAG
            );
        
        if (NULL == bssDescription)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for BSS description.",
                                AP_GET_PORT_NUMBER(ApPort),
                                FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + AP11_MAX_IE_BLOB_SIZE * 2));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        NdisZeroMemory(bssDescription, FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + AP11_MAX_IE_BLOB_SIZE * 2);

        //
        // Initialize the BSS description structure
        //

        bssDescription->BSSType = dot11_BSS_type_infrastructure;
        bssDescription->Timestamp = 0;

        //
        // set BSSID
        //
        NdisMoveMemory(
            bssDescription->BSSID, 
            AP_GET_BSSID(ApPort), 
            sizeof(DOT11_MAC_ADDRESS)
            );

        NdisMoveMemory(
            bssDescription->MacAddress, 
            VNic11QueryMACAddress(vnic), 
            sizeof(DOT11_MAC_ADDRESS)
            );

        //
        // set beacon period
        //
        bssDescription->BeaconPeriod = (USHORT)AP_GET_CONFIG(ApPort)->BeaconPeriod;
        
        //
        // Fill the capabilityInformation 
        // 

        bssDescription->Capability.usValue = AP_GET_CAPABILITY_INFO(ApPort).usValue;

        //
        // Set the starting address and size of the beacon IE blobs
        //
        bssDescription->BeaconIEBlobOffset = 0;
       
        ieBlobPtr = &bssDescription->IEBlobs[bssDescription->BeaconIEBlobOffset];
        ieBlobSize = AP11_MAX_IE_BLOB_SIZE;

        ndisStatus = ConstructAPIEBlob(ApPort, ieBlobPtr, FALSE, &ieBlobSize, &blobBytesWritten);

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): ConstructAPIEBlob for Beacon failed. Status = 0x%08x.", 
                                    AP_GET_PORT_NUMBER(ApPort),
                                    ndisStatus));
            break;
        }
        
        bssDescription->BeaconIEBlobSize = blobBytesWritten;

        //
        // save a copy of the IE blob
        //
        MP_ALLOCATE_MEMORY(
            AP_GET_MP_HANDLE(ApPort),
            &ieBlobPtr2, 
            blobBytesWritten,
            EXTAP_MEMORY_TAG
            );
        if (ieBlobPtr2 != NULL)
        {
            NdisMoveMemory(
                ieBlobPtr2,
                ieBlobPtr,
                blobBytesWritten
                );
            CfgSaveBeaconIe(
                AP_GET_CONFIG(ApPort),
                ieBlobPtr2,
                blobBytesWritten
                );
        }

        //
        // Set the starting address and size of the probe response IE blobs
        //
        bssDescription->ProbeIEBlobOffset = bssDescription->BeaconIEBlobSize;
       
        ieBlobPtr = &bssDescription->IEBlobs[bssDescription->ProbeIEBlobOffset];
        ieBlobSize = AP11_MAX_IE_BLOB_SIZE;

        ndisStatus = ConstructAPIEBlob(ApPort, ieBlobPtr, TRUE, &ieBlobSize, &blobBytesWritten);
        
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): ConstructAPIEBlob for Probe Response failed. Status = 0x%08x.", 
                                    AP_GET_PORT_NUMBER(ApPort),
                                    ndisStatus));
            break;
        }
        
        bssDescription->ProbeIEBlobSize = blobBytesWritten;
        bssDescription->IEBlobsSize = bssDescription->ProbeIEBlobSize + bssDescription->BeaconIEBlobSize;

        //
        // Notify VNIC that we need hardware access 
        //
        VNic11NotifyConnectionStatus(
            vnic,
            TRUE,                   // connected
            NDIS_STATUS_DOT11_CONNECTION_START,
            NULL,                   // status buffer
            0                       // status buffer size
            );

        //
        // Need to notify VNIC that connection is complete
        //
        notifyCompletion = TRUE;
        
        //
        // Set the AP channel
        //
        channel = (UCHAR)VNic11QueryPreferredChannel(vnic, &preferredChannelSet);

        // Set the default channel of AP to 11, this is fine because AP is only supported on 802.11b/g
        if (!preferredChannelSet)
        {
            channel = 11;

        }
        NdisResetEvent(&AP_GET_ASSOC_MGR(ApPort)->SetChannelCompletionEvent);
        // TODO: Need to get the PHY also
        ndisStatus = VNic11SetChannel(vnic, channel, 0, TRUE, Ap11SetChannelCompleteCallback);
        if (ndisStatus == NDIS_STATUS_PENDING)
        {
            //
            // Wait for the event that signals start completion
            //
            NdisWaitEvent(&AP_GET_ASSOC_MGR(ApPort)->SetChannelCompletionEvent, 0);

            ndisStatus = AP_GET_ASSOC_MGR(ApPort)->SetChannelCompletionStatus;
        }

        if (NDIS_STATUS_SUCCESS != ndisStatus)
        {
            MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): Set channel failed. Status = 0x%08x.", 
                                    AP_GET_PORT_NUMBER(ApPort),
                                    ndisStatus));
            break;
        }

        // let the OS know the channel we are working on
        ApIndicateFrequencyAdoped(ApPort);

        VNic11AcquireCtxSBarrier(vnic);
        CtxSBarrierAcquired = TRUE;
        
        //
        // Start beaconing and receiving data frames.
        //
        NdisResetEvent(&AP_GET_ASSOC_MGR(ApPort)->StartBSSCompletionEvent);

        ndisStatus = VNic11StartBSS(vnic, bssDescription, Ap11StartBSSCompleteCallback);
        if (ndisStatus == NDIS_STATUS_PENDING)
        {
            //
            // Wait for the event that signals start completion
            //
            NdisWaitEvent(&AP_GET_ASSOC_MGR(ApPort)->StartBSSCompletionEvent, 0);

            ndisStatus = AP_GET_ASSOC_MGR(ApPort)->StartBSSCompletionStatus;
        }

        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            //
            // Set TX data rate and our active PhyId
            //
            VNic11SelectTXDataRate(vnic, &(AP_GET_OPERATIONAL_RATE_SET(ApPort)), 100);
            // TODO: get active Phy ID
        }
    } while (FALSE);
    
    if (bssDescription != NULL)
    {
        MP_FREE_MEMORY(bssDescription);
    }

    if (CtxSBarrierAcquired)
    {
        VNic11ReleaseCtxSBarrier(vnic);
    }
    
    if (notifyCompletion)
    {
        //
        // Notify VNIC that we don't need hardware access any more
        //
        VNic11NotifyConnectionStatus(
            vnic,
            (ndisStatus == NDIS_STATUS_SUCCESS) ? TRUE : FALSE,
            NDIS_STATUS_DOT11_CONNECTION_COMPLETION,
            NULL,                   // status buffer
            0                       // status buffer size
            );
    }
        
    if (NDIS_STATUS_SUCCESS == ndisStatus)
    {
        // 
        // AP is started
        //
        ApSetState(ApPort, AP_STATE_STARTED);
        
        MpTrace(COMP_OID, DBG_NORMAL, 
                ("Port(%u): AP successfully started.", AP_GET_PORT_NUMBER(ApPort)));

        // 
        // Set the base port to OP mode
        //
        AP_GET_MP_PORT(ApPort)->CurrentOpState = OP_STATE;
    }
    else
    {
        // 
        // Stop association manager
        //
        ApStopAssocMgr(AP_GET_ASSOC_MGR(ApPort));

        MpTrace(COMP_OID, DBG_NORMAL, 
                ("Port(%u): AP failed to start. Status = 0x%08x.", AP_GET_PORT_NUMBER(ApPort), ndisStatus));

        // 
        // AP is not started
        //
        ApSetState(ApPort, AP_STATE_STOPPED);
    }
    
    return ndisStatus;
}

NDIS_STATUS
Ap11StartBSSCompleteCallback(
    _In_ PMP_PORT Port,
    _In_ PVOID Data
    )
{
    PMP_EXTAP_PORT apPort = MP_GET_AP_PORT(Port);

    // Set the start completion event
    AP_GET_ASSOC_MGR(apPort)->StartBSSCompletionStatus = *((PNDIS_STATUS)Data);
    NdisSetEvent(&AP_GET_ASSOC_MGR(apPort)->StartBSSCompletionEvent);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
Ap11SetChannelCompleteCallback(
    _In_ PMP_PORT Port,
    _In_ PVOID Data
    )
{
    PMP_EXTAP_PORT apPort = MP_GET_AP_PORT(Port);
    
    // Set the set channel completion event
    AP_GET_ASSOC_MGR(apPort)->SetChannelCompletionStatus = *((PNDIS_STATUS)Data);
    NdisSetEvent(&AP_GET_ASSOC_MGR(apPort)->SetChannelCompletionEvent);

    return NDIS_STATUS_SUCCESS;
}


/** Stop AP */
NDIS_STATUS
StopExtAp(
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    MPASSERT(ApGetState(ApPort) == AP_STATE_STARTED);
    
    // 
    // Set AP state to stopping so that no packets will be processed
    //

    ApSetState(ApPort, AP_STATE_STOPPING);

    //
    // stop beacon
    //
    ndisStatus = StopExtApBss(ApPort);

    // 
    // Wait for AP reference count to reach 1
    //
    while (ApPort->RefCount > 1)
    {
        NdisMSleep(10000);
    }
    
    //
    // stop association manager
    //
    ApStopAssocMgr(&ApPort->AssocMgr);

    //
    // Wait for DPCs to complete
    // This ensures all timeout callbacks return
    //
    KeFlushQueuedDpcs();
    
    ApSetState(ApPort, AP_STATE_STOPPED);
    
    MpTrace(COMP_OID, DBG_NORMAL, 
            ("Port(%u): AP successfully stopped.", AP_GET_PORT_NUMBER(ApPort)));

    // 
    // Set the base port to INIT mode
    //
    AP_GET_MP_PORT(ApPort)->CurrentOpState = INIT_STATE;
    
    return ndisStatus;
}


/** Stop Ext AP BSS */
NDIS_STATUS
StopExtApBss(
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PVNIC vnic = AP_GET_VNIC(ApPort);

    NdisResetEvent(&AP_GET_ASSOC_MGR(ApPort)->StopBSSCompletionEvent);

    ndisStatus = VNic11StopBSS(vnic, Ap11StopBSSCompleteCallback);
    if (ndisStatus == NDIS_STATUS_PENDING)
    {
        //
        // Wait for the event that signals stop completion
        //
        NdisWaitEvent(&AP_GET_ASSOC_MGR(ApPort)->StopBSSCompletionEvent, 0);
        ndisStatus = NDIS_STATUS_SUCCESS;
    }    

    return ndisStatus;
}


NDIS_STATUS
Ap11StopBSSCompleteCallback(
    _In_ PMP_PORT Port,
    _In_ PVOID Data
    )
{
    PMP_EXTAP_PORT apPort = MP_GET_AP_PORT(Port);

    UNREFERENCED_PARAMETER(Data);
    
    // Set the stop completion event
    NdisSetEvent(&AP_GET_ASSOC_MGR(apPort)->StopBSSCompletionEvent);

    return NDIS_STATUS_SUCCESS;
}


/** Restart AP */
NDIS_STATUS
RestartExtAp(
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    ndisStatus = ApRestartAssocMgr(AP_GET_ASSOC_MGR(ApPort));

    return ndisStatus;
}

/** Pause AP */
NDIS_STATUS
PauseExtAp(
    _In_ PMP_EXTAP_PORT ApPort
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;

    ndisStatus = ApPauseAssocMgr(AP_GET_ASSOC_MGR(ApPort));

    return ndisStatus;
}

VOID
Ap11Notify(
    _In_  PMP_PORT        Port,
    PVOID               Notif
)
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_EXTAP_PORT ApPort = MP_GET_AP_PORT(Port);
    PNOTIFICATION_DATA_HEADER pHdr = (PNOTIFICATION_DATA_HEADER)Notif;
    PUCHAR beaconIEBlobPtr = NULL, probeIEBlobPtr = NULL;
    USHORT beaconIEBlobSizeAllocated = 0, beaconIEBlobSize = 0;
    USHORT probeIEBlobSizeAllocated = 0, probeIEBlobSize = 0;
    UCHAR channel;
    PVNIC vnic = AP_GET_VNIC(ApPort);
    BOOLEAN isPreferredChannel;
    BOOLEAN bBeaconIESet = FALSE;

    // if AP is not running, we needn't do anything
    if (ApGetState(ApPort) != AP_STATE_STARTED)
    {
        return;
    }

    // BUGBUG:Synchronize this function with AP start/stop
    switch (pHdr->Type)
    {
        case NotificationOpChannel:
        {
            if (VNic11QueryPreferredChannel(vnic, &isPreferredChannel) == VNic11QueryCurrentChannel(vnic, FALSE))
            {
                // No need to do anything here
                MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): AP is already running on preferred channel.",
                                    AP_GET_PORT_NUMBER(ApPort)));
                
                break;
            }

            //
            // Update the beacon in the hardware
            //
            MP_ALLOCATE_MEMORY(
                AP_GET_MP_HANDLE(ApPort),
                &beaconIEBlobPtr, 
                AP11_MAX_IE_BLOB_SIZE,
                EXTAP_MEMORY_TAG
                );
            
            if (NULL == beaconIEBlobPtr)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for beacon IEs.",
                                    AP_GET_PORT_NUMBER(ApPort),
                                    AP11_MAX_IE_BLOB_SIZE));
                break;
            }
            
            NdisZeroMemory(beaconIEBlobPtr, AP11_MAX_IE_BLOB_SIZE);

            beaconIEBlobSizeAllocated = AP11_MAX_IE_BLOB_SIZE;
            ndisStatus = ConstructAPIEBlob(ApPort, beaconIEBlobPtr, FALSE, &beaconIEBlobSizeAllocated, &beaconIEBlobSize);
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
               MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): ConstructAPIEBlob for beacon failed. Status = 0x%08x.", 
                                    AP_GET_PORT_NUMBER(ApPort),
                                    ndisStatus));
               break;
            }

            //
            // Update the probe response in the hardware
            //
            MP_ALLOCATE_MEMORY(
                 AP_GET_MP_HANDLE(ApPort),
                 &probeIEBlobPtr, 
                 AP11_MAX_IE_BLOB_SIZE,
                 EXTAP_MEMORY_TAG
                 );
             
            if (NULL == probeIEBlobPtr)
            {
                MpTrace(COMP_OID, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for probe response IEs.",
                                     AP_GET_PORT_NUMBER(ApPort),
                                     AP11_MAX_IE_BLOB_SIZE));
                break;
            }
             
            NdisZeroMemory(probeIEBlobPtr, AP11_MAX_IE_BLOB_SIZE);
            
            probeIEBlobSizeAllocated = AP11_MAX_IE_BLOB_SIZE;
            ndisStatus = ConstructAPIEBlob(ApPort, probeIEBlobPtr, TRUE, &probeIEBlobSizeAllocated, &probeIEBlobSize);
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): ConstructAPIEBlob for probe response failed. Status = 0x%08x.", 
                                     AP_GET_PORT_NUMBER(ApPort),
                                     ndisStatus));
                break;
            }

            // 
            // Disassociate all stations before shifting channel
            //
            AmDisassociatePeerRequest(
                AP_GET_ASSOC_MGR(ApPort), 
                &Dot11BroadcastAddress, 
                DOT11_MGMT_REASON_UPSPEC_REASON             // TODO: a better reason code
                );

            // Set beacon IE
            ndisStatus = VNic11SetBeaconIE(Port->VNic, beaconIEBlobPtr, beaconIEBlobSize);
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): VNic11SetBeaconIE failed. Status = 0x%08x.", 
                                    AP_GET_PORT_NUMBER(ApPort),
                                    ndisStatus));
                break;
            }
            bBeaconIESet = TRUE;

            // Set probe response IE
            ndisStatus = VNic11SetProbeResponseIE(Port->VNic, probeIEBlobPtr, probeIEBlobSize);
            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                MpTrace(COMP_OID, DBG_NORMAL, ("Port(%u): VNic11SetProbeResponseIE failed. Status = 0x%08x.", 
                                     AP_GET_PORT_NUMBER(ApPort),
                                     ndisStatus));
                break;
            }
            
            channel = (UCHAR)VNic11QueryPreferredChannel(vnic, &isPreferredChannel);
                
            NdisResetEvent(&AP_GET_ASSOC_MGR(ApPort)->SetChannelCompletionEvent);

            ndisStatus = VNic11SetChannel(vnic, channel, 0, TRUE, Ap11SetChannelCompleteCallback);
            if (ndisStatus == NDIS_STATUS_PENDING)
            {
                //
                // Wait for the event that signals start completion
                //
                NdisWaitEvent(&AP_GET_ASSOC_MGR(ApPort)->SetChannelCompletionEvent, 0);

                ndisStatus = AP_GET_ASSOC_MGR(ApPort)->SetChannelCompletionStatus;
            }

            if (NDIS_STATUS_SUCCESS != ndisStatus)
            {
                break;
            }

            // let the OS know the channel we are working on
            ApIndicateFrequencyAdoped(ApPort);
        
        }
        break;

        default:
            break;
    };

    if (beaconIEBlobPtr != NULL)
    {
        if (TRUE == bBeaconIESet)
        {
            // save the beacon IE for assoc completion params
            CfgSaveBeaconIe(&ApPort->Config, beaconIEBlobPtr, beaconIEBlobSize);
        }
        else
        {
            MP_FREE_MEMORY(beaconIEBlobPtr);
        }
    }

    if (probeIEBlobPtr != NULL)
    {
        MP_FREE_MEMORY(probeIEBlobPtr);
    }
    
}

