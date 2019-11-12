/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_assoc.c

Abstract:
    STA layer association frame processing functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_assoc.c to st_assoc.c

Notes:

--*/
#include "precomp.h"
#include "st_assoc.h"
#include "st_conn.h"
#include "st_auth.h"
#include "st_adhoc.h"
#include "st_scan.h"
#include "st_send.h"

#if DOT11_TRACE_ENABLED
#include "st_assoc.tmh"
#endif

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
StaCreateAssociateRequestPacket(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _Outptr_result_buffer_to_(
        STA11_MAX_IE_BLOB_SIZE + sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_ASSOC_REQUEST_FRAME),
        *pAssocPacketLength)
	_Result_nullonfailure_
          PUCHAR* ppAssocPacket,
    _Out_ PUSHORT         pAssocPacketLength
    )
{
    PDOT11_MGMT_HEADER  pMgmtMacHeader = NULL;
    PDOT11_ASSOC_REQUEST_FRAME   pDot11AssocRequest = NULL;
    USHORT              IESize;
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pCurrentIE = NULL;
    DOT11_RATE_SET      RateSet;
    BOOLEAN             set;
    PVNIC               pNic = STA_GET_VNIC(pStation);

    IESize = STA11_MAX_IE_BLOB_SIZE;                    // SSID + Rates IE
    *pAssocPacketLength = sizeof(DOT11_MGMT_HEADER) + 
            sizeof(DOT11_ASSOC_REQUEST_FRAME) +
            IESize;

    MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle, ppAssocPacket, *pAssocPacketLength, EXTSTA_MEMORY_TAG);
    if (*ppAssocPacket == NULL)
    {
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to allocate association request packet\n"));
        return NDIS_STATUS_RESOURCES;
    }
    NdisZeroMemory(*ppAssocPacket, *pAssocPacketLength);    
    
    pMgmtMacHeader = (PDOT11_MGMT_HEADER)*ppAssocPacket;

    //
    // Fill the MAC header
    //
    pMgmtMacHeader->FrameControl.Version = 0x0;
    pMgmtMacHeader->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
    pMgmtMacHeader->FrameControl.Subtype = DOT11_MGMT_SUBTYPE_ASSOCIATION_REQUEST;
    pMgmtMacHeader->FrameControl.ToDS = 0x0;      // Default value for Mgmt frames
    pMgmtMacHeader->FrameControl.FromDS = 0x0;    // Default value for Mgmt frames
    pMgmtMacHeader->FrameControl.MoreFrag = 0x0;  
    pMgmtMacHeader->FrameControl.Retry = 0x0;
    pMgmtMacHeader->FrameControl.PwrMgt = 0x0;
    pMgmtMacHeader->FrameControl.MoreData = 0x0;
    pMgmtMacHeader->FrameControl.WEP = 0x0;       // no WEP
    pMgmtMacHeader->FrameControl.Order = 0x0;     // no order

    memcpy(pMgmtMacHeader->DA, 
           pAPEntry->MacAddress,
           DOT11_ADDRESS_SIZE
           );

    memcpy(pMgmtMacHeader->SA, 
           VNic11QueryMACAddress(pNic),
           DOT11_ADDRESS_SIZE
           );

    memcpy(pMgmtMacHeader->BSSID,
           pAPEntry->Dot11BSSID,
           DOT11_ADDRESS_SIZE
           );
           
    pDot11AssocRequest = (PDOT11_ASSOC_REQUEST_FRAME)((*ppAssocPacket) + sizeof(DOT11_MGMT_HEADER));

    do
    {
        //
        // Capabilities
        //
        NdisZeroMemory(&(pDot11AssocRequest->Capability), sizeof(DOT11_CAPABILITY));

        pDot11AssocRequest->Capability.ESS = 1;
        pDot11AssocRequest->Capability.IBSS = 0;
        pDot11AssocRequest->Capability.CFPollable = (VNic11QueryCFPollable(pNic) ? 1 : 0);
        pDot11AssocRequest->Capability.CFPollRequest = 0;

        pDot11AssocRequest->Capability.Privacy = 
            (pStation->Config.UnicastCipherAlgorithm != DOT11_CIPHER_ALGO_NONE) ? 1 : 0;

        switch (VNic11QueryCurrentPhyType(pNic)) 
        {
            case dot11_phy_type_erp:
                set = VNic11QueryShortSlotTimeOptionImplemented(pNic, FALSE);
                if (set)
                {
                    set = VNic11QueryShortSlotTimeOptionEnabled(pNic, FALSE);
                }
                pDot11AssocRequest->Capability.ShortSlotTime = set ? 1 : 0;
                
                set = VNic11QueryDsssOfdmOptionImplemented(pNic, FALSE);
                if (set)
                {
                    set = VNic11QueryDsssOfdmOptionEnabled(pNic, FALSE);
                }
                pDot11AssocRequest->Capability.DSSSOFDM = set ? 1 : 0;

                // fall through

            case dot11_phy_type_hrdsss:
                // Enable/verify short preamble
                set = VNic11QueryShortPreambleOptionImplemented(pNic, FALSE);
                pDot11AssocRequest->Capability.ShortPreamble = set ? 1: 0;

                set = VNic11QueryPbccOptionImplemented(pNic, FALSE);
                pDot11AssocRequest->Capability.PBCC = set ? 1: 0;

                set = VNic11QueryChannelAgilityPresent(pNic, FALSE);
                if (set)
                {
                    set = VNic11QueryChannelAgilityEnabled(pNic, FALSE);
                }
                pDot11AssocRequest->Capability.ChannelAgility = set ? 1 : 0;
        }

        pDot11AssocRequest->usListenInterval = pStation->Config.ListenInterval;

        pCurrentIE = ((PUCHAR)pDot11AssocRequest) + sizeof(DOT11_ASSOC_REQUEST_FRAME);
        
        //
        // Fill the SSID
        //
#pragma warning(suppress: 28020) // PREFast bug comparing IESize and uSSIDLength
        ndisStatus = Dot11AttachElement(
                &pCurrentIE,
                &IESize,
                DOT11_INFO_ELEMENT_ID_SSID,
                (UCHAR)pStation->Config.SSID.uSSIDLength,
                pStation->Config.SSID.ucSSID
                );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to populate SSID in association request\n"));
            break;
        }

        //
        // Fill the supported rates
        //
        VNic11QueryOperationalRateSet(pNic, &RateSet, FALSE);
        ndisStatus = StaFilterUnsupportedRates(pAPEntry, &RateSet);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to find compatible operational rates\n"));
            break;
        }

        ndisStatus = Dot11AttachElement(
                &pCurrentIE,
                &IESize,
                DOT11_INFO_ELEMENT_ID_SUPPORTED_RATES,
                (UCHAR)((RateSet.uRateSetLength > 8) ? 8 : RateSet.uRateSetLength),
                RateSet.ucRateSet
                );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to populate supported rates in association request\n"));
            break;
        }

        //
        // Add the extended rate set if needed
        //
        if (RateSet.uRateSetLength > (UCHAR)8) 
        {
            ndisStatus = Dot11AttachElement(
                &pCurrentIE,
                &IESize,
                DOT11_INFO_ELEMENT_ID_EXTD_SUPPORTED_RATES,
                (UCHAR)(RateSet.uRateSetLength - 8),
                (RateSet.ucRateSet + 8)
                );
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to add extended rates in association request\n"));
                break;
            }
        }

        //
        // RSNA/WPA IE
        //
        if (pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA ||
            pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_RSNA_PSK ||
            pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_WPA ||
            pStation->Config.AuthAlgorithm == DOT11_AUTH_ALGO_WPA_PSK)
        {
            ndisStatus = StaAttachInfraRSNIE(pStation, 
                                             pAPEntry, 
                                             &pCurrentIE,
                                             &IESize);
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to add RSN IE in association request\n"));
                break;
            }
        }

        //
        // Add the association params IE
        //
        if (pStation->Config.AdditionalIESize > 0)
        {
            // Check that BSSID matches
            if (ETH_IS_BROADCAST(pStation->Config.AssocIEBSSID) ||
                MP_COMPARE_MAC_ADDRESS(pAPEntry->Dot11BSSID, pStation->Config.AssocIEBSSID))
            {
                // BSSID matches, add the IE
                if (pStation->Config.AdditionalIESize < IESize)
                {
                    NdisMoveMemory(pCurrentIE, pStation->Config.AdditionalIEData, pStation->Config.AdditionalIESize);
                    pCurrentIE += pStation->Config.AdditionalIESize;
                    IESize -= (USHORT)pStation->Config.AdditionalIESize;
                }
                else
                {
                    MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to add additional IE in association request\n"));
                    // This is not a fatal error
                }
            }
        }

        //
        // Adjust packet length for IE space we did not use. Eg. if the SSID is not 32 bytes long, 
        // we have empty space here that we dont want send out in the packet. 
        //
        *pAssocPacketLength = *pAssocPacketLength - IESize;
        

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MP_FREE_MEMORY(*ppAssocPacket);
        *ppAssocPacket = NULL;
    }
    
    return ndisStatus;
}

VOID
StaFreeAssociateRequestPacket(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_ PUCHAR          pAssocPacket,
    _In_ USHORT          AssocPacketLength
    )
{
    UNREFERENCED_PARAMETER(pStation);
    UNREFERENCED_PARAMETER(pAPEntry);
    UNREFERENCED_PARAMETER(AssocPacketLength);

    MP_FREE_MEMORY(pAssocPacket);
}

NDIS_STATUS
StaAssociate(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  ULONG           AssociateTimeout
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pAssocPacket = NULL;
    USHORT              AssocPacketLength;
    ULONG               StatusCode = 0;
    LARGE_INTEGER       timeoutTime;
    
    MpEntry;

    do
    {
        //
        // Create the associate packet
        //
        ndisStatus = StaCreateAssociateRequestPacket(
            pStation,
            pAPEntry,
            &pAssocPacket,
            &AssocPacketLength
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            //
            // Complete the association process
            //
            StatusCode = ndisStatus;
            NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
            if (pStation->ConnectContext.AssociateState == ASSOC_STATE_RECEIVED_AUTHENTICATE)
            {
                //
                // Reset ourselves to ready to original state
                //
                pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
            }
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

            break;
        }

        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        if ((pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT) &&
            (pStation->ConnectContext.AssociateState == ASSOC_STATE_RECEIVED_AUTHENTICATE))
        {
            //
            // Packet will be sent means we will be waiting for packet response
            //
            pStation->ConnectContext.AssociateState = ASSOC_STATE_WAITING_FOR_ASSOCIATE;

            //
            // Save association request packet inside the AP entry. This will be freed
            // on association completion indication
            //
            NdisDprAcquireSpinLock(&(pAPEntry->Lock));
            pAPEntry->pAssocRequest = pAssocPacket;
            pAPEntry->AssocRequestLength = AssocPacketLength;
            NdisDprReleaseSpinLock(&(pAPEntry->Lock));

            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        }
        else
        {
            //
            // Reset, disconnect, deauthenticated by AP after authentication succeeded
            //
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Reset/Disconnect/Deauth while starting association\n"));

            //
            // Assoc packet pointer isnt saved, free it
            //
            StaFreeAssociateRequestPacket(
                pStation, 
                pAPEntry, 
                pAssocPacket, 
                AssocPacketLength
                );

            //
            // Abort association 
            //
            StatusCode = (ULONG)STATUS_CANCELLED;

            break;
        }

        //
        // Send the association request packet
        //
        ndisStatus = BasePortSendInternalPacket(
            STA_GET_MP_PORT(pStation),
             pAssocPacket,
            AssocPacketLength
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to send Association Request packet\n"));
            StatusCode = ndisStatus;

            NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
            if (pStation->ConnectContext.AssociateState == ASSOC_STATE_WAITING_FOR_AUTHENTICATE)
            {
                //
                // Reset ourselves to ready to original state
                //
                pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
            }
            
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            
            break;
        }
        else
        {
            // Add an extra refcount for the association timer
            STA_INCREMENT_REF(pStation->ConnectContext.AsyncFuncCount);

            //
            // Set the timeout timer for associate failure case. 
            //
            timeoutTime.QuadPart = Int32x32To64((LONG)AssociateTimeout, -10000);
            NdisSetTimerObject(pStation->ConnectContext.Timer_AssociateTimeout, 
                timeoutTime, 
                0, 
                NULL
                );
        }
        
        //
        // We dont let the hardware pend the request since we will be 
        // freeing the packet
        //
        MPASSERT(ndisStatus != NDIS_STATUS_PENDING);
    }while (FALSE);

    //
    // Fail the association if we failed inline, etc
    //
    if (StatusCode != 0)
    {
        StaAssociateComplete(pStation, DOT11_ASSOC_STATUS_SYSTEM_ERROR);
    }
    
    return ndisStatus;
}


VOID
StaAssociateComplete(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  ULONG           Status
    )
{
    MpEntry;

    //
    // Complete the Association
    //
    StaCompleteAssociationProcess(pStation, Status);    
}

// Can be called at both passive and dispatch
VOID
StaAssociateTimeoutCallback(
    PMP_EXTSTA_PORT             Station
    )
{
    //
    // Cancel the associate operation if it has not already completed
    //
    NdisAcquireSpinLock(&(Station->ConnectContext.Lock));    
    if (Station->ConnectContext.AssociateState == ASSOC_STATE_WAITING_FOR_ASSOCIATE)
    {
        Station->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
        NdisReleaseSpinLock(&(Station->ConnectContext.Lock));

        MpTrace(COMP_ASSOC, DBG_NORMAL, ("Cancelled the association Operation as it took too long\n"));
        
        StaAssociateComplete(Station, DOT11_ASSOC_STATUS_UNREACHABLE);
    }
    else if (Station->ConnectContext.AssociateState == ASSOC_STATE_REMOTELY_DEAUTHENTICATED)
    {
        //
        // While we were waiting for an associate response packet, we got a deauth
        // We will complete the association attempt with failure
        //
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("Deauthentication while waiting for association response\n"));

        // Put us back to default state
        Station->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
        NdisReleaseSpinLock(&(Station->ConnectContext.Lock));

        //
        // Complete the association process with failure
        //
        StaAssociateComplete(Station, DOT11_ASSOC_STATUS_PEER_DEAUTHENTICATED_START | Station->ConnectContext.DeAuthReason);
    }
    else
    {
        //
        // The association has already completed. dont do anything
        //
        NdisReleaseSpinLock(&(Station->ConnectContext.Lock));
        
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Association completed/cancelled before timeout\n"));
    }

    // Remove the extra refcount added for the timer
    STA_DECREMENT_REF(Station->ConnectContext.AsyncFuncCount);    
}

VOID
StaAssociateTimer(
    PVOID            SystemSpecific1,
    PVOID			 param,
    PVOID            SystemSpecific2,
    PVOID            SystemSpecific3 
    )
{
    PMP_EXTSTA_PORT    pStation = (PMP_EXTSTA_PORT)param;

	UNREFERENCED_PARAMETER(SystemSpecific1);
	UNREFERENCED_PARAMETER(SystemSpecific2);
	UNREFERENCED_PARAMETER(SystemSpecific3);

    StaAssociateTimeoutCallback(pStation);
}

VOID 
StaReceiveAssociationResponse(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_  PMP_RX_MPDU                    pFragment,
    _In_  ULONG                           TotalLength
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pPacketBuffer;
    PDOT11_MGMT_HEADER  pMgmtHeader;
    PDOT11_ASSOC_RESPONSE_FRAME   pDot11AssocFrame;
    USHORT              StatusCode;
    USHORT              AID = 0;
    PMP_BSS_ENTRY      pAPEntry = pStation->ConnectContext.ActiveAP;
    BOOLEAN             bTimerCancelled, bSetTxDataRate = FALSE;
    DOT11_RATE_SET      rateSet = {0};

    pPacketBuffer = MP_RX_MPDU_DATA(pFragment);

    //
    // Ref to make sure reset/halt does not leave while we are still working
    //
    STA_INCREMENT_REF(pStation->ConnectContext.AsyncFuncCount);
        
    NdisDprAcquireSpinLock(&(pStation->ConnectContext.Lock));
    if (pStation->ConnectContext.AssociateState == ASSOC_STATE_WAITING_FOR_ASSOCIATE)
    {
        if (pStation->ConnectContext.ConnectState < CONN_STATE_READY_TO_CONNECT)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Reset/Disconnect before association completed\n"));
            
            //
            // Reset/disconnect, etc. We dont process this associate packet. Eventually, timeout
            // will happen and cleanup
            //
            NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));
            STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);
            return;
        }

        do
        {
            if (TotalLength < (sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_ASSOC_RESPONSE_FRAME)))
            {
                ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
                MpTrace(COMP_ASSOC, DBG_LOUD, ("Association response packet too short\n"));
                break;
            }

            pMgmtHeader = (PDOT11_MGMT_HEADER)pPacketBuffer;

            //
            // Check that is a packet from the AP we are interested in
            //
            if (!MP_COMPARE_MAC_ADDRESS(pMgmtHeader->SA, pAPEntry->MacAddress) ||
                !MP_COMPARE_MAC_ADDRESS(pMgmtHeader->BSSID, pAPEntry->Dot11BSSID) ||
                !MP_COMPARE_MAC_ADDRESS(pMgmtHeader->DA, VNic11QueryMACAddress(STA_GET_VNIC(pStation))))
            {
                ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
                MpTrace(COMP_ASSOC, DBG_LOUD, ("Association response packet not for me\n"));
                break;
            }

        } while (FALSE);

        if (ndisStatus != NDIS_STATUS_NOT_ACCEPTED)
        {
            //
            // This was a valid response to our association request
            // Complete the association with appropriate status
            //        
            pDot11AssocFrame = (PDOT11_ASSOC_RESPONSE_FRAME)(pPacketBuffer + sizeof(DOT11_MGMT_HEADER));

            //
            // Get association status code from the packet
            //
            StatusCode = pDot11AssocFrame->usStatusCode;

            if (StatusCode == DOT11_FRAME_STATUS_SUCCESSFUL)
            {
                //
                // Association attempt succeeded
                //
                ndisStatus = NDIS_STATUS_SUCCESS;
                MpTrace(COMP_ASSOC, DBG_NORMAL, ("Association response status SUCCESS\n"));

                //
                // Validate AID
                //
                AID = pDot11AssocFrame->usAID;
                if ((AID & 0xc000) != 0xc000) 
                {
                    // Invalid AID. 
                    pStation->Config.ValidAID = FALSE;
                    
                    // Connecting to a non-conformant AP. Continue with association instead of bailing out.
                    MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Association response contains invalid AID %d\n", AID));
                }
                else
                {
                    pStation->Config.ValidAID = TRUE;
                }

                AID  &= ~(0xc000);
                if (AID > 2007) 
                {
                    // AID too big
                    ndisStatus = NDIS_STATUS_FAILURE;
                    MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Association response contains invalid AID %d\n", AID));
                }

                if (ndisStatus == NDIS_STATUS_SUCCESS)
                {
                    //
                    // Get data rate
                    //
                    ndisStatus = Dot11GetRateSetFromInfoEle(
                                    Add2Ptr(pDot11AssocFrame, sizeof(DOT11_ASSOC_RESPONSE_FRAME)),
                                    TotalLength - sizeof(DOT11_MGMT_HEADER) - sizeof(DOT11_ASSOC_RESPONSE_FRAME),
                                    FALSE,
                                    &rateSet);
                }
                
                if (ndisStatus == NDIS_STATUS_SUCCESS)
                {
                    // Association has succeeded
                    MpTrace(COMP_ASSOC, DBG_NORMAL, ("Association ID %d\n", AID));
                    
                    pStation->ConnectContext.AssociateState = ASSOC_STATE_RECEIVED_ASSOCIATE;                
                    pStation->Config.AID = AID;

                    //
                    // Clear non-static WEP keys.
                    //
                    VNic11DeleteNonPersistentKey(STA_GET_VNIC(pStation));

                    bSetTxDataRate = TRUE;

                    //
                    // Set active PhyId
                    //
                    pStation->Config.ActivePhyId = pAPEntry->PhyId;

                    //
                    // Set multicast cipher algorithm if the exact algorithm was not selected.
                    //
                    if (pStation->Config.MulticastCipherAlgorithmCount > 1)
                    {
                        VNic11SetCipher(STA_GET_VNIC(pStation), FALSE, pStation->Config.MulticastCipherAlgorithm);
                    }
                }
                else
                {
                    pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
                    StatusCode = DOT11_FRAME_STATUS_FAILURE;         // Unspecified failure
                }
            }
            else
            {
                // The association attempt failed
                MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Association failed by the access point with status %d\n", StatusCode));
                pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
            }

            //
            // Copy the association response buffer into the AP Entry for completion indication
            //
            NdisDprAcquireSpinLock(&(pAPEntry->Lock));
            pAPEntry->AssocResponseLength = (USHORT)TotalLength;
            MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle, &(pAPEntry->pAssocResponse), TotalLength, EXTSTA_MEMORY_TAG);
            if (pAPEntry->pAssocResponse == NULL)
            {
                MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to save association request packet\n"));
                //
                // We still maintain the association
                //
                pAPEntry->AssocResponseLength = 0;
            }
            else
            {
                //
                // Copy the association response into the packet
                //
                NdisMoveMemory(pAPEntry->pAssocResponse, pPacketBuffer, TotalLength);
            }

            // Save association ID, time, etc
            pAPEntry->AssocID = AID;
            NdisGetCurrentSystemTime(&(pAPEntry->AssociationUpTime));
            pAPEntry->AssocState = dot11_assoc_state_auth_assoc;
            NdisDprReleaseSpinLock(&(pAPEntry->Lock));

            NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));

            if (bSetTxDataRate == TRUE)
            {
                //
                // Set data TX rate
                // TODO: This should be based per peer
                //
                VNic11SelectTXDataRate(STA_GET_VNIC(pStation), 
                    &rateSet, 
                    pFragment->Msdu->LinkQuality
                    );
            }

            //
            // Attempt to cancel the timer
            //
            bTimerCancelled = NdisCancelTimerObject(pStation->ConnectContext.Timer_AssociateTimeout);
            if (bTimerCancelled)
            {
                STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);
            }

            if (StatusCode) 
            {
                StaAssociateComplete(pStation, StatusCode | DOT11_ASSOC_STATUS_ASSOCIATION_RESPONSE_START);
            } 
            else 
            {
                StaAssociateComplete(pStation, DOT11_ASSOC_STATUS_SUCCESS);
            }
        }
        else
        {
            NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));
        }        
    }
    else
    {
        NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Association already timed out\n"));
    }

    STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);    
}


VOID 
StaReceiveDisassociation(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_  PMP_RX_MPDU                    pFragment,
    _In_  ULONG                           TotalLength
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pPacketBuffer;
    PDOT11_DISASSOC_FRAME   pDot11DisassocFrame = NULL;
    PDOT11_MGMT_HEADER  pMgmtHeader;
    PMP_BSS_ENTRY      pAPEntry;
    BOOLEAN             bDisassociate = TRUE;
    
    if (TotalLength < (sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_DISASSOC_FRAME)))
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Disassociation packet too short\n"));
        return ;
    }

    pPacketBuffer = MP_RX_MPDU_DATA(pFragment);

    //
    // Ref to make sure reset/halt does not leave while we are still working
    //
    STA_INCREMENT_REF(pStation->ConnectContext.AsyncFuncCount);

    //
    // Proceed only if we have received association packet
    //
    NdisDprAcquireSpinLock(&(pStation->ConnectContext.Lock));
    if ((pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT) &&
        (pStation->ConnectContext.AssociateState > ASSOC_STATE_WAITING_FOR_ASSOCIATE))
    {        
        pAPEntry = pStation->ConnectContext.ActiveAP;
        pMgmtHeader = (PDOT11_MGMT_HEADER)pPacketBuffer;
        
        //
        // Check that is a packet from the AP we are interested in
        //
        if (!MP_COMPARE_MAC_ADDRESS(pMgmtHeader->SA, pAPEntry->MacAddress) ||
            !MP_COMPARE_MAC_ADDRESS(pMgmtHeader->BSSID, pAPEntry->Dot11BSSID) ||
            !MP_COMPARE_MAC_ADDRESS(pMgmtHeader->DA, VNic11QueryMACAddress(STA_GET_VNIC(pStation))))
        {
            NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Disassociate packet not for me\n"));
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
        }
        else
        {
            pDot11DisassocFrame = (PDOT11_DISASSOC_FRAME)(pPacketBuffer + sizeof(DOT11_MGMT_HEADER));

            MpTrace(COMP_ASSOC, DBG_NORMAL, ("Received disassociation, reason %d - ", pDot11DisassocFrame->usReasonCode));

            //
            // On a Disassociated, we would just send the deauthenticate
            // packet to the access point & move ourselves to ready state
            //
            if (pStation->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, ("After we were associated\n"));        
            
                //
                // We were associated, cleanup state and indicate disassociation
                //
                pStation->ConnectContext.ActiveAP = NULL;
                bDisassociate = TRUE;
                pAPEntry->AssocState = dot11_assoc_state_unauth_unassoc;    // Moving to unauthenticated
                pStation->ConnectContext.AssociateState = ASSOC_STATE_NOT_ASSOCIATED;                
            }
            else
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, ("While we were associating\n"));        
            
                //
                // Receive disassociate while still attempting to associate. Set associate failed
                // so appropriate routine knows to fail the association attempt
                //
                pStation->ConnectContext.AssociateState = ASSOC_STATE_REMOTELY_DISASSOCIATED;
                bDisassociate = FALSE;
            }
            
            NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));

            if (bDisassociate)
            {
                //
                // Send deauthenticate (ignoring status)
                //
                StaSendDeauthentication(
                    pStation, 
                    pAPEntry, 
                    DOT11_MGMT_REASON_UPSPEC_REASON
                    );
                
                // Indicate disassociation
                StaIndicateDisassociation(
                    pStation, 
                    pAPEntry, 
                    DOT11_ASSOC_STATUS_PEER_DISASSOCIATED_START | pDot11DisassocFrame->usReasonCode
                    );

                NdisDprAcquireSpinLock(&(pStation->ConnectContext.Lock));
                if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
                {
                    //
                    // We got disassociated, but we can still connect. We will
                    // queue the periodic scan routine to attempt association
                    //
                    StaForceInternalScan(pStation, TRUE);
                }
                NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));
            }
            
            //
            // Raise cost and remove connection ref from the AP entry and raise cost
            //
            pAPEntry->AssocCost += STA_ASSOC_COST_REMOTE_DISCONNECT;
            STA_DECREMENT_REF(pAPEntry->RefCount);
        }
    }
    else
    {
        NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));
    }
    
    // Done
    STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);    
}

NDIS_STATUS
StaSendDisassociation(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  USHORT          ReasonCode
    )
{
    PDOT11_MGMT_HEADER  pMgmtMacHeader = NULL;
    PDOT11_DISASSOC_FRAME   pDot11DisassocFrame = NULL;
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pPacketBuffer = NULL;
    USHORT              PacketSize;

    MpEntry;
    
    PacketSize = sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_DISASSOC_FRAME);

    MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle, &pPacketBuffer, PacketSize, EXTSTA_MEMORY_TAG);
    if (pPacketBuffer == NULL)
    {
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to allocate disassociation request packet\n"));
        return NDIS_STATUS_RESOURCES;
    }
    NdisZeroMemory(pPacketBuffer, PacketSize);        

    pMgmtMacHeader = (PDOT11_MGMT_HEADER)pPacketBuffer;
    
    //
    // Fill the MAC header
    //
    pMgmtMacHeader->FrameControl.Version = 0x0;
    pMgmtMacHeader->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
    pMgmtMacHeader->FrameControl.Subtype = DOT11_MGMT_SUBTYPE_DISASSOCIATION;
    pMgmtMacHeader->FrameControl.ToDS = 0x0;      // Default value for Mgmt frames
    pMgmtMacHeader->FrameControl.FromDS = 0x0;    // Default value for Mgmt frames
    pMgmtMacHeader->FrameControl.MoreFrag = 0x0;  
    pMgmtMacHeader->FrameControl.Retry = 0x0;
    pMgmtMacHeader->FrameControl.PwrMgt = 0x0;
    pMgmtMacHeader->FrameControl.MoreData = 0x0;
    pMgmtMacHeader->FrameControl.WEP = 0x0;       // no WEP
    pMgmtMacHeader->FrameControl.Order = 0x0;     // no order

    memcpy(pMgmtMacHeader->DA, 
           pAPEntry->MacAddress,
           DOT11_ADDRESS_SIZE
           );

    memcpy(pMgmtMacHeader->SA, 
           VNic11QueryMACAddress(STA_GET_VNIC(pStation)),
           DOT11_ADDRESS_SIZE
           );

    memcpy(pMgmtMacHeader->BSSID,
           pAPEntry->Dot11BSSID,
           DOT11_ADDRESS_SIZE
           );

    pDot11DisassocFrame = (PDOT11_DISASSOC_FRAME)(pPacketBuffer + sizeof(DOT11_MGMT_HEADER));
    pDot11DisassocFrame->usReasonCode = ReasonCode;

    //
    // Send the disassociation request packet
    ndisStatus = BasePortSendInternalPacket(
        STA_GET_MP_PORT(pStation),
         pPacketBuffer,
        PacketSize
        );
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to send disassociation packet\n"));
    }

    // Send Mgmt Packet cannot pend
    MPASSERT(ndisStatus != NDIS_STATUS_PENDING);
    
    if (pPacketBuffer)
    {
        MP_FREE_MEMORY(pPacketBuffer);
    }

    return ndisStatus;
}
