/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_auth.c

Abstract:
    STA layer authentication frame processing functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_auth.c to st_auth.c
    
Notes:

--*/
#include "precomp.h"
#include "st_auth.h"
#include "st_conn.h"
#include "st_assoc.h"
#include "st_adhoc.h"
#include "st_scan.h"
#include "st_send.h"

#if DOT11_TRACE_ENABLED
#include "st_auth.tmh"
#endif

NDIS_STATUS
StaAuthenticate(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  ULONG           AuthenticateTimeout
    )
{
    MpEntry;
    //
    // Only supporting 802.11 (Open and shared key) Authentication as of now
    // Note that WPA/RSNA authentication also starts with 802.11 open system
    // authentication.
    //
    return StaStart80211Authentication(
        pStation,
        pAPEntry,
        AuthenticateTimeout
        );
}


VOID
StaAuthenticateComplete(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  ULONG           Status
    )
{
    NDIS_STATUS         ndisStatus;
    
    MpEntry;
    
    if (Status != 0)
    {
        //
        // Authentication failed. Association attempt with this access point
        // has failed. Complete the authentication
        //

        //
        // Complete association process
        //
        StaCompleteAssociationProcess(pStation, Status);
    }
    else
    {
        MPASSERT(pStation->ConnectContext.ActiveAP != NULL);
        
        //
        // Update the authentication state on the access point
        //
        pStation->ConnectContext.ActiveAP->AssocState = dot11_assoc_state_auth_unassoc;
        
        //
        // Start association with this access point
        //
        ndisStatus = StaAssociate(
            pStation,
            pStation->ConnectContext.ActiveAP,
            STA_ASSOCIATE_FAILURE_TIMEOUT
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            //
            // If association cannot be started. It is responsible
            // for completing the associaiton
        }
    }
}

VOID
StaAuthenticateTimer(
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

    StaAuthenticateTimeoutCallback(pStation);
}

VOID
StaAuthenticateTimeoutCallback(
    _In_  PMP_EXTSTA_PORT                        pStation
    )
{
    //
    // Cancel the 802.11 Authentication process
    //
    StaCancel80211Authentication(pStation);

}

VOID 
StaReceiveAuthentication(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_  PMP_RX_MPDU                    pFragment,
    _In_  ULONG                           TotalLength
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pPacketBuffer;

    pPacketBuffer = MP_RX_MPDU_DATA(pFragment);
    do
    {
        if (TotalLength < (sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_AUTH_FRAME)))
        {
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Authentication packet too short\n"));
            break;
        }

        //
        // Pass to 802.11 authentication packet handler for infrastructure if we are running in
        // infra mode. Otherwise, pass to adhoc module.
        //
        if (pStation->Config.BSSType == dot11_BSS_type_infrastructure)
        {
            StaProcess80211AuthPacket(
                pStation, 
                pPacketBuffer, 
                TotalLength
                );
        }
        else
        {
            StaAdhocProcessMgmtPacket(
                pStation, 
                (PDOT11_MGMT_HEADER)pPacketBuffer, 
                TotalLength
                );
        }
            
    }while (FALSE);
}

VOID 
StaReceiveDeauthentication(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_  PMP_RX_MPDU                    pFragment,
    _In_  ULONG                           TotalLength
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pPacketBuffer;
    PDOT11_DEAUTH_FRAME pDot11DeauthFrame = NULL;
    PDOT11_MGMT_HEADER  pMgmtHeader;
    PMP_BSS_ENTRY      pAPEntry;
    BOOLEAN             bDisassociate = FALSE;
    
    if (TotalLength < (sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_DEAUTH_FRAME)))
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Deauthentication packet too short\n"));
        return;
    }

    pPacketBuffer = MP_RX_MPDU_DATA(pFragment);

    //
    // If we are running in adhoc mode, pass the message to adhoc module and we are done.
    //
    if (pStation->Config.BSSType == dot11_BSS_type_independent)
    {
        StaAdhocProcessMgmtPacket(
            pStation, 
            (PDOT11_MGMT_HEADER)pPacketBuffer, 
            TotalLength
            );

        return;
    } 

    //
    // Ref to make sure reset/halt does not leave while we are still working
    //
    STA_INCREMENT_REF(pStation->ConnectContext.AsyncFuncCount);
    
    //
    // Proceed if we are currently authenticated
    //
    NdisDprAcquireSpinLock(&(pStation->ConnectContext.Lock));
    if ((pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT) &&
        (pStation->ConnectContext.AssociateState > ASSOC_STATE_WAITING_FOR_AUTHENTICATE))
    {
        pAPEntry = pStation->ConnectContext.ActiveAP;
        MPASSERT(pAPEntry);

        pMgmtHeader = (PDOT11_MGMT_HEADER)pPacketBuffer;

        //
        // Check if this is a packet from the AP we are interested in
        //
        if (!MP_COMPARE_MAC_ADDRESS(pMgmtHeader->SA, pAPEntry->MacAddress) ||
            !MP_COMPARE_MAC_ADDRESS(pMgmtHeader->BSSID, pAPEntry->Dot11BSSID) ||
            !MP_COMPARE_MAC_ADDRESS(pMgmtHeader->DA, VNic11QueryMACAddress(STA_GET_VNIC(pStation))))
        {
            NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));
            ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Deauthentication packet not for me\n"));
        }
        else
        {
            pDot11DeauthFrame = (PDOT11_DEAUTH_FRAME)(pPacketBuffer + sizeof(DOT11_MGMT_HEADER));

            MpTrace(COMP_ASSOC, DBG_NORMAL, ("Received deauth, reason %d - ", pDot11DeauthFrame->ReasonCode));

            if (pStation->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED)
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, ("after we were associated\n"));        
            
                //
                // We were associated, cleanup state and indicate disassociation
                //
                pStation->ConnectContext.ActiveAP = NULL;
                bDisassociate = TRUE;
                pAPEntry->AssocState = dot11_assoc_state_unauth_unassoc;
                pStation->ConnectContext.AssociateState = ASSOC_STATE_NOT_ASSOCIATED;
            }
            else
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, ("while we were associating\n"));        
            
                //
                // Receive deauth while still attempting to associate. Set associate failed
                // so appropriate routine knows to fail the association attempt
                //
                pStation->ConnectContext.AssociateState = ASSOC_STATE_REMOTELY_DEAUTHENTICATED;
                pStation->ConnectContext.DeAuthReason = pDot11DeauthFrame->ReasonCode;
                bDisassociate = FALSE;
            }
            
            NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));

            if (bDisassociate)
            {
                //
                // Indicate disassociation
                //
                StaIndicateDisassociation(
                    pStation, 
                    pAPEntry, 
                    DOT11_ASSOC_STATUS_PEER_DEAUTHENTICATED_START | pDot11DeauthFrame->ReasonCode
                    );

                NdisDprAcquireSpinLock(&(pStation->ConnectContext.Lock));
                if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
                {
                    //
                    // We got deauthenticated, but we can still connect. We will
                    // queue the periodic scan routine to attempt association
                    //
                    StaForceInternalScan(pStation, TRUE);
                }
                NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));

                //
                // Raise cost and remove connected refcount from the AP entry
                //
                pAPEntry->AssocCost += STA_ASSOC_COST_REMOTE_DISCONNECT;
                STA_DECREMENT_REF(pAPEntry->RefCount);
            }
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
StaSendDeauthentication(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  USHORT          ReasonCode
    )
{
    PDOT11_MGMT_HEADER  pMgmtMacHeader = NULL;
    PDOT11_DEAUTH_FRAME pDot11DeauthFrame = NULL;
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pPacketBuffer = NULL;
    USHORT              PacketSize;

    MpEntry;

    PacketSize = sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_DEAUTH_FRAME);

    MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle, &pPacketBuffer, PacketSize, EXTSTA_MEMORY_TAG);
    if (pPacketBuffer == NULL)
    {
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to allocate deauthentication packet\n"));
        return NDIS_STATUS_RESOURCES;
    }

    NdisZeroMemory(pPacketBuffer, PacketSize);
    
    pMgmtMacHeader = (PDOT11_MGMT_HEADER)pPacketBuffer;
    
    //
    // Fill the MAC header
    //
    pMgmtMacHeader->FrameControl.Version = 0x0;
    pMgmtMacHeader->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
    pMgmtMacHeader->FrameControl.Subtype = DOT11_MGMT_SUBTYPE_DEAUTHENTICATION;
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

    pDot11DeauthFrame = (PDOT11_DEAUTH_FRAME)(pPacketBuffer + sizeof(DOT11_MGMT_HEADER));
    pDot11DeauthFrame->ReasonCode = ReasonCode;

    //
    // Send the deauthentication packet
    ndisStatus = BasePortSendInternalPacket(
        STA_GET_MP_PORT(pStation),
        pPacketBuffer,
        PacketSize
        );
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to send deauthentication packet\n"));
    }

    // Send Mgmt Packet cannot pend
    MPASSERT(ndisStatus != NDIS_STATUS_PENDING);
    
    if (pPacketBuffer)
    {
        MP_FREE_MEMORY(pPacketBuffer);
    }

    return ndisStatus;
}


//====================================================
// 802.11 (OPEN and SHARED KEY) AUTHENTICATION
//====================================================

NDIS_STATUS
StaStart80211Authentication(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  ULONG           AuthenticateTimeout
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pAuthPacket = NULL;
    USHORT              AuthPacketLength;
    ULONG               StatusCode = 0;
    LARGE_INTEGER       timeoutTime;

    do
    {
        //
        // Create the authenticate packet
        //
        ndisStatus = StaCreate80211AuthReqPacket(
            pStation,
            pAPEntry,
            &pAuthPacket,
            &AuthPacketLength
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // Will complete the association process
            StatusCode = ndisStatus;

            NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
            if (pStation->ConnectContext.AssociateState == ASSOC_STATE_JOINED)
            {
                pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
            }
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            break;
        }

        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
        {
            //
            // Sending packet meaning we will be waiting for packet response (seq number 2)
            //
            MPASSERT(pStation->ConnectContext.AssociateState == ASSOC_STATE_JOINED);
            pStation->ConnectContext.AssociateState = ASSOC_STATE_WAITING_FOR_AUTHENTICATE;
            pStation->ConnectContext.ExpectedAuthSeqNumber = 2;
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        }
        else
        {
            //
            // Reset, disconnect, etc
            //
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Reset/Disconnect while starting authentication\n"));

            //
            // Dont proceed with authentication 
            //
            StatusCode = (ULONG)STATUS_CANCELLED;
            break;
        }

        //
        // Send the authentication packet
        //
        ndisStatus = BasePortSendInternalPacket(
            STA_GET_MP_PORT(pStation),
            pAuthPacket,
            AuthPacketLength
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to send authentication request packet\n"));
            StatusCode = ndisStatus;

            NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
            if (pStation->ConnectContext.AssociateState == ASSOC_STATE_WAITING_FOR_AUTHENTICATE)
            {
                // Reset ourselves to original state
                pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
            }            
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            
            break;
        }
        else
        {
            // Add an extra refcount for the authentication timer
            STA_INCREMENT_REF(pStation->ConnectContext.AsyncFuncCount);

            //
            // Set the timeout timer for authenticate failure case. 
            //
            timeoutTime.QuadPart = Int32x32To64((LONG)AuthenticateTimeout, -10000);
            NdisSetTimerObject(pStation->ConnectContext.Timer_AuthenticateTimeout, 
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
    // We free the authentication packet as soon as we have handed it to
    // the hardware
    //
    if (pAuthPacket != NULL)
    {
        StaFree80211AuthReqPacket(
            pStation,
            pAPEntry,
            pAuthPacket,
            AuthPacketLength
            );

        pAuthPacket = NULL;
    }

    //
    // Fail the authentication if we failed inline, etc
    //
    if (StatusCode != 0)
    {
        StaAuthenticateComplete(pStation, DOT11_ASSOC_STATUS_SYSTEM_ERROR);
    }

    return ndisStatus;
}

VOID
StaCancel80211Authentication(
    _In_  PMP_EXTSTA_PORT                        pStation
    )
{
    //
    // Cancel the authenticate operation if it has not already completed
    //
    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
    if (pStation->ConnectContext.AssociateState == ASSOC_STATE_WAITING_FOR_AUTHENTICATE)
    {
        pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

        MpTrace(COMP_ASSOC, DBG_NORMAL, ("Cancelled the authenticate Operation as it took too long\n"));
        StaAuthenticateComplete(pStation, DOT11_ASSOC_STATUS_UNREACHABLE);
    }
    else
    {
        //
        // The authenticate already completed. dont do anything
        //
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Authentication completed/cancelled before timeout\n"));
    }
    
    // Remove the extra refcount added for the timer
    STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);    
}

VOID
StaProcess80211AuthPacket(
    _In_  PMP_EXTSTA_PORT                        pStation,
    _In_reads_bytes_(PacketLength)  PUCHAR                          pPacketBuffer,
    _In_  ULONG                           PacketLength
    )
{
    NDIS_STATUS         ndisStatus;
    PDOT11_AUTH_FRAME   pDot11AuthFrame;
    USHORT              StatusCode;
    BOOLEAN             bTimerCancelled;
    PDOT11_MGMT_HEADER  pMgmtHeader;
    PMP_BSS_ENTRY      pAPEntry = NULL;
    BOOLEAN             bMatch;
    BOOLEAN             bMoreAuthFrame = FALSE;

    //
    // Caller has already verified length that we have enough data for
    // mgmt header & auth status
    //
    UNREFERENCED_PARAMETER(PacketLength);

    //
    // Only proceed if we are waiting for a authentication response & not timed out
    // or something
    //
    ndisStatus = NDIS_STATUS_NOT_ACCEPTED;
    StatusCode = DOT11_FRAME_STATUS_FAILURE;

    __try 
    {
        //
        // Ref to make sure reset/halt does not leave while we are still working
        //
        STA_INCREMENT_REF(pStation->ConnectContext.AsyncFuncCount);

        NdisDprAcquireSpinLock(&(pStation->ConnectContext.Lock));
        
        if (pStation->ConnectContext.AssociateState == ASSOC_STATE_WAITING_FOR_AUTHENTICATE)
        {
            if (pStation->ConnectContext.ConnectState < CONN_STATE_READY_TO_CONNECT)
            {
                //
                // Reset/disconnect, etc. We dont process this auth packet. Eventually, timeout
                // will happen and cleanup
                //
                MpTrace(COMP_ASSOC, DBG_LOUD, ("Reset/Disconnect before authentication completed\n"));
                __leave;
            }

            pAPEntry = pStation->ConnectContext.ActiveAP;
            pMgmtHeader = (PDOT11_MGMT_HEADER)pPacketBuffer;
            
            //
            // Check that is a packet from the AP we are interested in
            //
            if (!MP_COMPARE_MAC_ADDRESS(pMgmtHeader->SA, pAPEntry->MacAddress) ||
                !MP_COMPARE_MAC_ADDRESS(pMgmtHeader->BSSID, pAPEntry->Dot11BSSID) ||
                !MP_COMPARE_MAC_ADDRESS(pMgmtHeader->DA, VNic11QueryMACAddress(STA_GET_VNIC(pStation))))
            {
                MpTrace(COMP_ASSOC, DBG_LOUD, ("Authentication packet not for me\n"));
                __leave;
            }

            pDot11AuthFrame = (PDOT11_AUTH_FRAME)(pPacketBuffer + sizeof(DOT11_MGMT_HEADER));
            
            // Validate auth algorithm and Sequence number
            switch (pStation->Config.AuthAlgorithm) 
            {
                case DOT11_AUTH_ALGO_80211_OPEN:
                case DOT11_AUTH_ALGO_WPA:
                case DOT11_AUTH_ALGO_WPA_PSK:
                case DOT11_AUTH_ALGO_RSNA:
                case DOT11_AUTH_ALGO_RSNA_PSK:
                    bMatch = (BOOLEAN)(pDot11AuthFrame->usAlgorithmNumber == DOT11_AUTH_OPEN_SYSTEM);
                    break;

                case DOT11_AUTH_ALGO_80211_SHARED_KEY:
                    bMatch = (BOOLEAN)(pDot11AuthFrame->usAlgorithmNumber == DOT11_AUTH_SHARED_KEY);
                    break;

                default:
                    bMatch = FALSE;
            }

            if (!bMatch || pDot11AuthFrame->usXid != pStation->ConnectContext.ExpectedAuthSeqNumber)
            {
                MpTrace(COMP_ASSOC, DBG_NORMAL, ("Invalid Authentication algorithm/sequence\n"));
                __leave;
            }

            //
            // This was a valid response to our authentication request
            // Complete the authentication process with appropriate status
            //

            //
            // Get authentication status code from the packet
            //
            StatusCode = pDot11AuthFrame->usStatusCode;

            if (StatusCode == DOT11_FRAME_STATUS_SUCCESSFUL)
            {
                if (pDot11AuthFrame->usAlgorithmNumber == DOT11_AUTH_SHARED_KEY &&
                    pDot11AuthFrame->usXid == 2)
                {
                    //
                    // If this is shared key auth frame with sequence number 2, we are not done yet.
                    // Reuse the received the packet to fill the challenge response authenticate packet.
                    //
                    StaFillSharedKeyAuthChallengResPacket(pStation, pAPEntry, pPacketBuffer);
                    bMoreAuthFrame = TRUE;
                    pStation->ConnectContext.ExpectedAuthSeqNumber = 4;
                }
                else
                {
                    // Authentication succeeded
                    MpTrace(COMP_ASSOC, DBG_NORMAL, ("Authentication response SUCCESS\n"));
                    pStation->ConnectContext.AssociateState = ASSOC_STATE_RECEIVED_AUTHENTICATE;
                    ndisStatus = NDIS_STATUS_SUCCESS;
                }

            }
            else
            {
                // The authentication attempt failed
                MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Authentication failed by the access point with status %d\n", StatusCode));
                pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
                ndisStatus = NDIS_STATUS_FAILURE;                
            }
        }
        else
        {
            // Timeout already
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Authentication already timed out\n"));
        }
    }    
    __finally 
    {
        NdisDprReleaseSpinLock(&(pStation->ConnectContext.Lock));

        if (bMoreAuthFrame) 
        {
            //
            // Send the challenge response authentication packet. 
            // We don't care if the send fails. If it does, the timeout routine will do
            // the necessary cleanup.
            //
            BasePortSendInternalPacket(
                STA_GET_MP_PORT(pStation),
                pPacketBuffer,
                (USHORT)PacketLength
                );
        }
        else if (ndisStatus != NDIS_STATUS_NOT_ACCEPTED) 
        {
            //
            // Attempt to cancel the timer
            //
            bTimerCancelled = NdisCancelTimerObject(pStation->ConnectContext.Timer_AuthenticateTimeout);
            if (bTimerCancelled)
            {
                STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);
            }
                    
            // Complete
            if (StatusCode) 
            {
                StaAuthenticateComplete(pStation, StatusCode | DOT11_ASSOC_STATUS_ASSOCIATION_RESPONSE_START);
            } 
            else 
            {
                StaAuthenticateComplete(pStation, DOT11_ASSOC_STATUS_SUCCESS);
            }
        }

        STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);
    }
}

NDIS_STATUS
StaCreate80211AuthReqPacket(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _Outptr_result_bytebuffer_(*pAuthPacketLength) PUCHAR*         ppAuthPacket,
    _Out_ PUSHORT         pAuthPacketLength
    )
{
    PDOT11_MGMT_HEADER  pMgmtMacHeader = NULL;
    PDOT11_AUTH_FRAME   pDot11AuthFrame = NULL;

    // 
    // We are only supporting 802.11 (open system and shared key)
    //
    *pAuthPacketLength = sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_AUTH_FRAME);

    MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle, ppAuthPacket, *pAuthPacketLength, EXTSTA_MEMORY_TAG);
    if (*ppAuthPacket == NULL)
    {
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to allocate authentication request packet\n"));
        return NDIS_STATUS_RESOURCES;
    }
    NdisZeroMemory(*ppAuthPacket, *pAuthPacketLength);    

    pMgmtMacHeader = (PDOT11_MGMT_HEADER)*ppAuthPacket;

    //
    // Fill the MAC header
    //
    pMgmtMacHeader->FrameControl.Version = 0x0;
    pMgmtMacHeader->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
    pMgmtMacHeader->FrameControl.Subtype = DOT11_MGMT_SUBTYPE_AUTHENTICATION;
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

    pDot11AuthFrame = (PDOT11_AUTH_FRAME)((*ppAuthPacket) + sizeof(DOT11_MGMT_HEADER));
    switch (pStation->Config.AuthAlgorithm) 
    {
        case DOT11_AUTH_ALGO_80211_OPEN:
        case DOT11_AUTH_ALGO_WPA:
        case DOT11_AUTH_ALGO_WPA_PSK:
        case DOT11_AUTH_ALGO_RSNA:
        case DOT11_AUTH_ALGO_RSNA_PSK:
            pDot11AuthFrame->usAlgorithmNumber = (USHORT)(DOT11_AUTH_OPEN_SYSTEM);
            break;

        case DOT11_AUTH_ALGO_80211_SHARED_KEY:
            pDot11AuthFrame->usAlgorithmNumber = (USHORT)(DOT11_AUTH_SHARED_KEY);
            break;

        default:
            ASSERT(0);
    }

    pDot11AuthFrame->usXid = 1;
    pDot11AuthFrame->usStatusCode = (USHORT)0;

    return NDIS_STATUS_SUCCESS;
}

void
StaFillSharedKeyAuthChallengResPacket(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_reads_bytes_(sizeof(DOT11_MGMT_HEADER) + sizeof(DOT11_AUTH_FRAME))  PUCHAR          pPacketBuffer
    )
{
    PDOT11_MGMT_HEADER  pMgmtMacHeader;
    PDOT11_AUTH_FRAME   pDot11AuthFrame;   

    //
    // Fill the MAC header
    //
    pMgmtMacHeader = (PDOT11_MGMT_HEADER)pPacketBuffer;
    pMgmtMacHeader->FrameControl.Version = 0x0;
    pMgmtMacHeader->FrameControl.Type = DOT11_FRAME_TYPE_MANAGEMENT;
    pMgmtMacHeader->FrameControl.Subtype = DOT11_MGMT_SUBTYPE_AUTHENTICATION;
    pMgmtMacHeader->FrameControl.ToDS = 0x0;      // Default value for Mgmt frames
    pMgmtMacHeader->FrameControl.FromDS = 0x0;    // Default value for Mgmt frames
    pMgmtMacHeader->FrameControl.MoreFrag = 0x0;  
    pMgmtMacHeader->FrameControl.Retry = 0x0;
    pMgmtMacHeader->FrameControl.PwrMgt = 0x0;
    pMgmtMacHeader->FrameControl.MoreData = 0x0;
    pMgmtMacHeader->FrameControl.WEP = 0x1;       // WEP on
    pMgmtMacHeader->FrameControl.Order = 0x0;     // no order
    pMgmtMacHeader->SequenceControl.usValue = 0;

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

    //
    // Fill the Auth frame
    //
    pDot11AuthFrame = Add2Ptr(pMgmtMacHeader, sizeof(DOT11_MGMT_HEADER));
    pDot11AuthFrame->usAlgorithmNumber = (USHORT)(DOT11_AUTH_SHARED_KEY);
    pDot11AuthFrame->usXid = 3;
    pDot11AuthFrame->usStatusCode = (USHORT)0;
}

VOID
StaFree80211AuthReqPacket(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_ PUCHAR          pAuthPacket,
    _In_ USHORT          AuthPacketLength
    )
{
    UNREFERENCED_PARAMETER(pStation);
    UNREFERENCED_PARAMETER(pAPEntry);
    UNREFERENCED_PARAMETER(AuthPacketLength);

    MP_FREE_MEMORY(pAuthPacket);
}

//====================================================
// 802.11 (OPEN and SHARED KEY) AUTHENTICATION
//====================================================

