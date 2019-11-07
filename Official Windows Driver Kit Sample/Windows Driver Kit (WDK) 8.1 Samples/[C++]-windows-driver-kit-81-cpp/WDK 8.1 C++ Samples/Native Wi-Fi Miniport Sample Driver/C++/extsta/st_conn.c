/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_conn.c

Abstract:
    STA layer infrastructure connection/roaming functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_conn.c to st_conn.c
    
Notes:

--*/
#include "precomp.h"
#include "st_conn.h"
#include "st_aplst.h"
#include "st_auth.h"
#include "st_assoc.h"
#include "st_scan.h"
#include "st_oids.h"

#if DOT11_TRACE_ENABLED
#include "st_conn.tmh"
#endif


NDIS_STATUS
StaInitializeConnectionContext(
    _In_  PMP_EXTSTA_PORT        pStation
    )
{
    PSTA_INFRA_CONNECT_CONTEXT  pConnectContext = &(pStation->ConnectContext);
    NDIS_TIMER_CHARACTERISTICS  timerChar;               
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    do
    {
        NdisZeroMemory(&timerChar, sizeof(NDIS_TIMER_CHARACTERISTICS));
            
        // Authentication timeout timer
        timerChar.Header.Type = NDIS_OBJECT_TYPE_TIMER_CHARACTERISTICS;
        timerChar.Header.Revision = NDIS_TIMER_CHARACTERISTICS_REVISION_1;
        timerChar.Header.Size = sizeof(NDIS_TIMER_CHARACTERISTICS);
        timerChar.AllocationTag = EXTSTA_MEMORY_TAG;
        
        timerChar.TimerFunction = StaAuthenticateTimer;
        timerChar.FunctionContext = pStation;

        ndisStatus = NdisAllocateTimerObject(
                        STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
                        &timerChar,
                        &(pConnectContext->Timer_AuthenticateTimeout)
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate authentication timeout timer\n"));
            break;
        }

        // Association timeout timer
        timerChar.TimerFunction = StaAssociateTimer;
        timerChar.FunctionContext = pStation;

        ndisStatus = NdisAllocateTimerObject(
                        STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
                        &timerChar,
                        &(pConnectContext->Timer_AssociateTimeout)
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate association timeout timer\n"));
            break;
        }

        NdisAllocateSpinLock(&(pConnectContext->Lock));

        NDIS_INIT_MUTEX(&(pConnectContext->DisconnectGate));        // Initialized to set
        
        pConnectContext->AssociateState = ASSOC_STATE_NOT_ASSOCIATED;
        pConnectContext->ConnectState = CONN_STATE_DISCONNECTED;
        pConnectContext->PreviousConnectState = CONN_STATE_DISCONNECTED;
        pConnectContext->RoamForSendFailures = FALSE;
        pConnectContext->UpdateLinkQuality = FALSE;

        //
        // Allocate minimum required buffer for association completion. We use this
        // for association completion
        //
        MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle, &(pConnectContext->pAssocFailBuffer), 
                            sizeof(DOT11_ASSOCIATION_COMPLETION_PARAMETERS), EXTSTA_MEMORY_TAG);
        if (pConnectContext->pAssocFailBuffer == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate association failure buffer\n"));
            break;
        }

        // For association we need directed mgmt packets. Set the appropriate packet filter
        ndisStatus = VNic11SetPacketFilter(STA_GET_VNIC(pStation), 
                        NDIS_PACKET_TYPE_802_11_DIRECTED_MGMT | STA_GET_MP_PORT(pStation)->PacketFilter
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to set default packet filter\n"));
            break;
        }
            
    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        StaFreeConnectionContext(pStation);
    }
    
    return ndisStatus;
}

VOID
StaFreeConnectionContext(
    _In_  PMP_EXTSTA_PORT        pStation
    )
{
    if (pStation->ConnectContext.Timer_AuthenticateTimeout != NULL)
    {
        NdisFreeTimerObject(pStation->ConnectContext.Timer_AuthenticateTimeout);
        pStation->ConnectContext.Timer_AuthenticateTimeout = NULL;
    }

    if (pStation->ConnectContext.Timer_AssociateTimeout != NULL)
    {
        NdisFreeTimerObject(pStation->ConnectContext.Timer_AssociateTimeout);
        pStation->ConnectContext.Timer_AssociateTimeout = NULL;
    }

    // Free the default association failure buffer
    if (pStation->ConnectContext.pAssocFailBuffer != NULL)
    {
        MP_FREE_MEMORY(pStation->ConnectContext.pAssocFailBuffer);
        pStation->ConnectContext.pAssocFailBuffer = NULL;
    }
}


//
// Multiple threads may be present here
// 
VOID
StaResetConnection(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  BOOLEAN         bDisconnect
    )
{
    PSTA_INFRA_CONNECT_CONTEXT    pConnectContext = &(pStation->ConnectContext);
    PMP_BSS_ENTRY      pAPEntry = NULL;
    STA_ASSOC_STATE     AssociateStateAtReset = ASSOC_STATE_NOT_ASSOCIATED;
    BOOLEAN             bTimerCancelled = FALSE;

    //
    // Handle simultaneous reset/halt, etc
    //
    NDIS_WAIT_FOR_MUTEX(&(pConnectContext->DisconnectGate));
    
    //
    // Only one reset routine gets in here at a time
    //

    NdisAcquireSpinLock(&(pConnectContext->Lock));
    
    //
    // Check current state so that we can do cleanup
    //
    if (pConnectContext->ConnectState > CONN_STATE_IN_RESET)
    {
        pConnectContext->PreviousConnectState = pConnectContext->ConnectState;
        pConnectContext->ConnectState = CONN_STATE_IN_RESET;        // Let everyone know we are in reset
    }
    NdisReleaseSpinLock(&(pConnectContext->Lock));

    //
    // To expedite reset, we trigger timeout routines 
    //
    bTimerCancelled = NdisCancelTimerObject(pStation->ConnectContext.Timer_AssociateTimeout);
    if (bTimerCancelled)
    {
        //
        // Invoke the association timeout routine
        //
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Trigger early associate timeout\n"));
        StaAssociateTimeoutCallback(pStation);
    }
    
    bTimerCancelled = NdisCancelTimerObject(pStation->ConnectContext.Timer_AuthenticateTimeout);
    if (bTimerCancelled)
    {
        //
        // Invoke the authentication timeout routine
        //
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Trigger early authenticate timeout\n"));
        StaAuthenticateTimeoutCallback(pStation);
    }

    //
    // Wait for all asynchronous threads to complete. This waits for connection
    // attempt to complete
    //
    while (pConnectContext->AsyncFuncCount > 0)
    {
        NdisMSleep(10000);
    }

    NdisAcquireSpinLock(&(pConnectContext->Lock));

    //
    // Get the AP entry
    //
    pAPEntry = pConnectContext->ActiveAP;
    pConnectContext->ActiveAP = NULL;

    //
    // Set state to disconnected
    //
    AssociateStateAtReset = pConnectContext->AssociateState;    
    pConnectContext->PreviousConnectState = CONN_STATE_DISCONNECTED;
    pConnectContext->ConnectState = CONN_STATE_DISCONNECTED;
    pConnectContext->AssociateState = ASSOC_STATE_NOT_ASSOCIATED;

    if (AssociateStateAtReset == ASSOC_STATE_ASSOCIATED)
    {
        //
        // We were connected when reset completed and we did not fail the connection. 
        // Issue a disconnect (can be changed to just a disconnect status indication)
        //
        NdisReleaseSpinLock(&(pConnectContext->Lock));
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Reset with Active connection\n"));

        MPASSERT(pAPEntry != NULL);     // SYNC:
        
        pAPEntry->AssocState = dot11_assoc_state_unauth_unassoc;    

        //
        // If we are halting we dont want to do a clean disconnect
        // 
        if (bDisconnect)
        {
            StaDisconnect(
                pStation, 
                pAPEntry, 
                AssociateStateAtReset
                );
        }
        
        // We were connected when we started
        STA_DECREMENT_REF(pAPEntry->RefCount);
    }
    else
    {    
        NdisReleaseSpinLock(&(pConnectContext->Lock));
    }

    //
    // Other reset/cleanup routines can now proceed
    //
    NDIS_RELEASE_MUTEX(&(pConnectContext->DisconnectGate));
}

NDIS_STATUS
StaIndicateConnectionStart(
    _In_  PMP_EXTSTA_PORT        pStation
    )
{
    DOT11_CONNECTION_START_PARAMETERS   connectionStartParameters;

    //
    // Indicate the connection start status indication
    //
    NdisZeroMemory(&connectionStartParameters, sizeof(DOT11_CONNECTION_START_PARAMETERS));
    MP_ASSIGN_NDIS_OBJECT_HEADER(connectionStartParameters.Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_CONNECTION_START_PARAMETERS_REVISION_1,
                                 sizeof(DOT11_CONNECTION_START_PARAMETERS));
    connectionStartParameters.BSSType = pStation->Config.BSSType;

    MpTrace(COMP_ASSOC, DBG_LOUD, ("Connection Start Status Indication\n"));

    StaIndicateDot11Status(
        pStation,
        NDIS_STATUS_DOT11_CONNECTION_START,
        NULL,
        &connectionStartParameters,
        sizeof(DOT11_CONNECTION_START_PARAMETERS)
        );

    //
    // Forward the connection status to the hardware
    //
    VNic11NotifyConnectionStatus(
        STA_GET_VNIC(pStation), 
        FALSE, 
        NDIS_STATUS_DOT11_CONNECTION_START,
        &connectionStartParameters, 
        sizeof(DOT11_CONNECTION_START_PARAMETERS)
        );    

    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
StaIndicateConnectionCompletion(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  ULONG           CompletionStatus
    )
{
    DOT11_CONNECTION_COMPLETION_PARAMETERS   connectionCompleteParameters;

    //
    // Indicate the connection complete status indication
    //
    NdisZeroMemory(&connectionCompleteParameters, sizeof(DOT11_CONNECTION_COMPLETION_PARAMETERS));
    MP_ASSIGN_NDIS_OBJECT_HEADER(connectionCompleteParameters.Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_CONNECTION_COMPLETION_PARAMETERS_REVISION_1,
                                 sizeof(DOT11_CONNECTION_COMPLETION_PARAMETERS));
    connectionCompleteParameters.uStatus = CompletionStatus;

    //
    // Forward the connection status to the hardware
    //
    VNic11NotifyConnectionStatus(
        STA_GET_VNIC(pStation), 
        (CompletionStatus == 0) ? TRUE : FALSE, 
        NDIS_STATUS_DOT11_CONNECTION_COMPLETION,
        &connectionCompleteParameters, 
        sizeof(DOT11_CONNECTION_COMPLETION_PARAMETERS)
        );

    //
    // And to the OS
    //
    StaIndicateDot11Status(
        pStation,
        NDIS_STATUS_DOT11_CONNECTION_COMPLETION,
        NULL,
        &connectionCompleteParameters,
        sizeof(DOT11_CONNECTION_COMPLETION_PARAMETERS)
        );

    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
StaIndicateRoamingStart(
    _In_  PMP_EXTSTA_PORT         pStation,
    _In_  ULONG                   RoamingReason,
    _In_  BOOLEAN                 CurrentAssociated
    )
{
    DOT11_ROAMING_START_PARAMETERS   roamingStartParameters;

    //
    // Indicate the connection start status indication
    //
    NdisZeroMemory(&roamingStartParameters, sizeof(DOT11_ROAMING_START_PARAMETERS));
    MP_ASSIGN_NDIS_OBJECT_HEADER(roamingStartParameters.Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_ROAMING_START_PARAMETERS_REVISION_1,
                                 sizeof(DOT11_ROAMING_START_PARAMETERS));
    roamingStartParameters.uRoamingReason = RoamingReason;
    
    MpTrace(COMP_ASSOC, DBG_LOUD, ("Roaming Start Status Indication\n"));

    //
    // Forard the status to the OS
    //
    StaIndicateDot11Status(
        pStation,
        NDIS_STATUS_DOT11_ROAMING_START,
        NULL,
        &roamingStartParameters,
        sizeof(DOT11_ROAMING_START_PARAMETERS)
        );

    //
    // Forward the roaming status to the hardware
    //
    VNic11NotifyConnectionStatus(
        STA_GET_VNIC(pStation), 
        CurrentAssociated, 
        NDIS_STATUS_DOT11_ROAMING_START,
        &roamingStartParameters, 
        sizeof(DOT11_ROAMING_START_PARAMETERS)
        );    

    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
StaIndicateRoamingCompletion(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  ULONG           CompletionStatus
    )
{
    DOT11_ROAMING_COMPLETION_PARAMETERS   roamingCompleteParameters;

    //
    // Indicate the connection complete status indication
    //
    NdisZeroMemory(&roamingCompleteParameters, sizeof(DOT11_ROAMING_COMPLETION_PARAMETERS));
    MP_ASSIGN_NDIS_OBJECT_HEADER(roamingCompleteParameters.Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_ROAMING_COMPLETION_PARAMETERS_REVISION_1,
                                 sizeof(DOT11_ROAMING_COMPLETION_PARAMETERS));
    roamingCompleteParameters.uStatus = CompletionStatus;


    //
    // Forward the roaming completion status to the hardware
    //
    VNic11NotifyConnectionStatus(
        STA_GET_VNIC(pStation), 
        (CompletionStatus == 0) ? TRUE : FALSE, 
        NDIS_STATUS_DOT11_ROAMING_COMPLETION,
        &roamingCompleteParameters, 
        sizeof(DOT11_ROAMING_COMPLETION_PARAMETERS)
        );

    //
    // And to the OS
    //
    StaIndicateDot11Status(
        pStation,
        NDIS_STATUS_DOT11_ROAMING_COMPLETION,
        NULL,
        &roamingCompleteParameters,
        sizeof(DOT11_ROAMING_COMPLETION_PARAMETERS)
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
StaIndicateAssociationStart(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry
    )
{
    DOT11_ASSOCIATION_START_PARAMETERS  assocStartParameters;
    MpEntry;

    //
    // Indicate the association start status indication
    //
    NdisZeroMemory(&assocStartParameters, sizeof(DOT11_ASSOCIATION_START_PARAMETERS));
    MP_ASSIGN_NDIS_OBJECT_HEADER(assocStartParameters.Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_ASSOCIATION_START_PARAMETERS_REVISION_1,
                                 sizeof(DOT11_ASSOCIATION_START_PARAMETERS));
    assocStartParameters.uIHVDataOffset = 0;
    assocStartParameters.uIHVDataSize = 0;

    // We only supporting single SSID
    NdisMoveMemory(&(assocStartParameters.SSID), &(pStation->Config.SSID), sizeof(DOT11_SSID));
    
    // Copy the MAC address from the AP entry
    NdisAcquireSpinLock(&(pAPEntry->Lock));
    NdisMoveMemory(&(assocStartParameters.MacAddr), &(pAPEntry->MacAddress), sizeof(DOT11_MAC_ADDRESS));
    NdisReleaseSpinLock(&(pAPEntry->Lock));

    StaIndicateDot11Status(
        pStation,
        NDIS_STATUS_DOT11_ASSOCIATION_START,
        NULL,
        &assocStartParameters,
        sizeof(DOT11_ASSOCIATION_START_PARAMETERS)
        );

    //
    // After informing the OS about the association start, inform the HW
    //
    VNic11NotifyConnectionStatus(
        STA_GET_VNIC(pStation), 
        FALSE,
        NDIS_STATUS_DOT11_ASSOCIATION_START,
        &assocStartParameters, 
        sizeof(DOT11_ASSOCIATION_START_PARAMETERS)
        );

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
StaIndicateAssociationCompletion(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  DOT11_ASSOC_STATUS Status
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR              pStatusBuffer = NULL;
    ULONG               BufferLength = 0;
    BOOLEAN             bUseBackupBuffer = FALSE;
    PDOT11_ASSOCIATION_COMPLETION_PARAMETERS    pCompletionParameters = NULL;
    PUCHAR              pTempPtr = NULL;
    ULONG               CurrentOffset = sizeof(DOT11_ASSOCIATION_COMPLETION_PARAMETERS);
    ULONG               IHVDataOffset;
    
    MpEntry;
    
    NdisAcquireSpinLock(&(pAPEntry->Lock));

    //
    // Determine length of buffer to use for filling completion
    // parameters.
    //
    BufferLength = sizeof(DOT11_ASSOCIATION_COMPLETION_PARAMETERS) +
        pAPEntry->InfoElemBlobSize + FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements) +
        sizeof(ULONG);                           // PHYID + 

    if (pAPEntry->pAssocRequest != NULL)
    {
        ASSERT(pAPEntry->AssocRequestLength > DOT11_MGMT_HEADER_SIZE);
        BufferLength += pAPEntry->AssocRequestLength - DOT11_MGMT_HEADER_SIZE;
    }

    if (pAPEntry->pAssocResponse != NULL)
    {
        ASSERT(pAPEntry->AssocResponseLength > DOT11_MGMT_HEADER_SIZE);
        BufferLength += pAPEntry->AssocResponseLength - DOT11_MGMT_HEADER_SIZE;
    }

    // This is info we are passing to the HW
    IHVDataOffset = BufferLength;
    BufferLength += sizeof(ULONG);  // Association ID
    
    MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle, &pStatusBuffer, BufferLength, EXTSTA_MEMORY_TAG);
    if (pStatusBuffer == NULL)
    {
        // 
        // Allocation failed. However we must indicate the completion status. 
        // We will use the preallocated (default sized) buffer for indicating 
        // association completion
        //
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Unable to allocate completion buffer. Using backup buffer\n"));
        bUseBackupBuffer = TRUE;
        pStatusBuffer = pStation->ConnectContext.pAssocFailBuffer;
        BufferLength = sizeof(DOT11_ASSOCIATION_COMPLETION_PARAMETERS);
        ndisStatus = NDIS_STATUS_RESOURCES;
    }

    NdisZeroMemory(pStatusBuffer, BufferLength);
    
    pCompletionParameters = (PDOT11_ASSOCIATION_COMPLETION_PARAMETERS)pStatusBuffer;
    MP_ASSIGN_NDIS_OBJECT_HEADER(pCompletionParameters->Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_ASSOCIATION_COMPLETION_PARAMETERS_REVISION_1,
                                 sizeof(DOT11_ASSOCIATION_COMPLETION_PARAMETERS));
    NdisMoveMemory(pCompletionParameters->MacAddr, pAPEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));

    pCompletionParameters->uStatus = Status;
    if (Status == 0)
    {
        //
        // Fill success state parameters
        //
        pCompletionParameters->AuthAlgo = pStation->Config.AuthAlgorithm;     // ENCRYPT:
        pCompletionParameters->UnicastCipher = pStation->Config.UnicastCipherAlgorithm;
        pCompletionParameters->MulticastCipher = pStation->Config.MulticastCipherAlgorithm;
        pCompletionParameters->bFourAddressSupported = FALSE;       // No support for 4 address detection
        pCompletionParameters->bPortAuthorized = FALSE;
        pCompletionParameters->DSInfo = DOT11_DS_UNKNOWN;
        pCompletionParameters->uEncapTableOffset = 0;
        pCompletionParameters->uEncapTableSize = 0;

        pCompletionParameters->bReAssocReq = FALSE;                             // ROAM:
        pCompletionParameters->bReAssocResp = FALSE;    

    }

    if (bUseBackupBuffer == FALSE)
    {
        // 
        // We have a big enough buffer for all the information
        //

        //
        // First add PHY ID
        //
        pCompletionParameters->uActivePhyListOffset = CurrentOffset;
        pCompletionParameters->uActivePhyListSize = sizeof(ULONG);
        pTempPtr = pStatusBuffer + CurrentOffset;
        *((ULONG UNALIGNED *)pTempPtr) = pStation->Config.ActivePhyId;

        CurrentOffset += sizeof(ULONG);

        //
        // Beacon (or probe response if we dont have a beacon)
        // We copy both the fixed & variable IEs of the beacon/probe response so the length &
        // pointer are adjusted below
        //
        pCompletionParameters->uBeaconOffset = CurrentOffset;
        pCompletionParameters->uBeaconSize = pAPEntry->InfoElemBlobSize + FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements);
        pTempPtr = pStatusBuffer + CurrentOffset;
        NdisMoveMemory(pTempPtr, 
            pAPEntry->pDot11InfoElemBlob - FIELD_OFFSET(DOT11_BEACON_FRAME, InfoElements), 
            pCompletionParameters->uBeaconSize
            );
        CurrentOffset += pCompletionParameters->uBeaconSize;

        //
        // Association request
        //
        if (pAPEntry->pAssocRequest != NULL)
        {
            pCompletionParameters->uAssocReqOffset = CurrentOffset;
            pCompletionParameters->uAssocReqSize = pAPEntry->AssocRequestLength - DOT11_MGMT_HEADER_SIZE;
        
            pTempPtr = pStatusBuffer + CurrentOffset;
            NdisMoveMemory(pTempPtr, 
                pAPEntry->pAssocRequest + DOT11_MGMT_HEADER_SIZE, 
                pCompletionParameters->uAssocReqSize);
            CurrentOffset += pCompletionParameters->uAssocReqSize;
        }

        //
        // Association response
        //
        if (pAPEntry->pAssocResponse != NULL)
        {
            pCompletionParameters->uAssocRespOffset = CurrentOffset;
            pCompletionParameters->uAssocRespSize = pAPEntry->AssocResponseLength - DOT11_MGMT_HEADER_SIZE;
        
            pTempPtr = pStatusBuffer + CurrentOffset;
            NdisMoveMemory(pTempPtr, 
                pAPEntry->pAssocResponse + DOT11_MGMT_HEADER_SIZE, 
                pCompletionParameters->uAssocRespSize);
            CurrentOffset += pCompletionParameters->uAssocRespSize;
        }

        //
        // Fill in the Association ID in the IHV data portion
        //
        if (Status == 0)
        {
            pTempPtr = pStatusBuffer + IHVDataOffset;
            *((ULONG UNALIGNED *)pTempPtr) = pStation->Config.AID;
            
            pCompletionParameters->uIHVDataOffset = IHVDataOffset;
            pCompletionParameters->uIHVDataSize = sizeof(ULONG);

            CurrentOffset += sizeof(ULONG);
        }
    }

    //
    // We can free the association request and response structures if present
    //
    if (pAPEntry->pAssocRequest != NULL)
    {
        StaFreeAssociateRequestPacket(
            pStation, 
            pAPEntry, 
            pAPEntry->pAssocRequest, 
            pAPEntry->AssocRequestLength
            );
        pAPEntry->pAssocRequest = NULL;
        pAPEntry->AssocRequestLength = 0;
    }

    if (pAPEntry->pAssocResponse != NULL)
    {
        MP_FREE_MEMORY(pAPEntry->pAssocResponse);
        pAPEntry->pAssocResponse = NULL;
        pAPEntry->AssocResponseLength = 0;
    }
    
    NdisReleaseSpinLock(&(pAPEntry->Lock));


    //
    // Before informing the OS about the association, inform the HW
    //
    VNic11NotifyConnectionStatus(
        STA_GET_VNIC(pStation), 
        (Status == 0) ? TRUE : FALSE,
        NDIS_STATUS_DOT11_ASSOCIATION_COMPLETION,
        pStatusBuffer, 
        BufferLength
        );

    VNic11ReleaseCtxSBarrier(STA_GET_VNIC(pStation));

    // Now indicate the status indicaiton to the OS
    StaIndicateDot11Status(
        pStation,
        NDIS_STATUS_DOT11_ASSOCIATION_COMPLETION,
        NULL,
        pStatusBuffer,
        BufferLength
        );

    //
    // Free the status buffer if we allocated it
    //
    if (bUseBackupBuffer == FALSE)
    {
        MP_FREE_MEMORY(pStatusBuffer);
        pStatusBuffer = NULL;
    }

    //
    // Let the caller know if we indicated complete indication or partial
    //
    return ndisStatus;
}

VOID
StaIndicateDisassociation(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  ULONG           Reason
    )
{
    DOT11_DISASSOCIATION_PARAMETERS disassocParameters;
    MpEntry;

    NdisZeroMemory(&disassocParameters, sizeof(DOT11_DISASSOCIATION_PARAMETERS));
    MP_ASSIGN_NDIS_OBJECT_HEADER(disassocParameters.Header, 
                                 NDIS_OBJECT_TYPE_DEFAULT,
                                 DOT11_DISASSOCIATION_PARAMETERS_REVISION_1,
                                 sizeof(DOT11_DISASSOCIATION_PARAMETERS));
    disassocParameters.uIHVDataOffset = 0;
    disassocParameters.uIHVDataSize = 0;

    // Copy the MAC address from the AP entry
    NdisAcquireSpinLock(&(pAPEntry->Lock));
    NdisMoveMemory(&(disassocParameters.MacAddr), &(pAPEntry->MacAddress), sizeof(DOT11_MAC_ADDRESS));
    NdisReleaseSpinLock(&(pAPEntry->Lock));

    disassocParameters.uReason = Reason;

    StaIndicateDot11Status(
        pStation,
        NDIS_STATUS_DOT11_DISASSOCIATION,
        NULL,
        &disassocParameters,
        sizeof(DOT11_DISASSOCIATION_PARAMETERS)
        );

    // Indicate disassociation to the HW
    VNic11NotifyConnectionStatus(
        STA_GET_VNIC(pStation), 
        FALSE,
        NDIS_STATUS_DOT11_DISASSOCIATION,
        &disassocParameters, 
        sizeof(DOT11_DISASSOCIATION_PARAMETERS)
        );

}


NDIS_STATUS
StaConnectStart(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  BOOLEAN         FirstAttempt
    )
{
    NDIS_STATUS     ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_BSS_ENTRY  pAPEntry = NULL;
    ULONG           CompletionStatus = DOT11_CONNECTION_STATUS_IHV_START;   // Default failure status
    ULONG           origPowerSavingLevel;
    BOOLEAN         bScanForAP = FALSE;
    BOOLEAN         bIndicateComplete = FALSE;

    //
    // We dont want reset/halt to wait while we are still running. This refcount
    // wont be removed until connect complete is called. Reset/halt routine will wait
    // for this count to go to zero before returning to the OS
    //
    STA_INCREMENT_REF(pStation->ConnectContext.AsyncFuncCount);

    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
    MPASSERT(pStation->ConnectContext.AssociateState == ASSOC_STATE_NOT_ASSOCIATED);  // SYNC:

    if (FirstAttempt)
    {
        //
        // We wont connect if OS scan is pending
        //
        if (STA_TEST_SCAN_FLAG(pStation, STA_EXTERNAL_SCAN_IN_PROGRESS))
        {
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));        
            STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);

            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("External scan already in progress. Ignoring connect request\n"));
            return NDIS_STATUS_DOT11_MEDIA_IN_USE;
        }
    
        //
        // The first time, proceed only if we are disconnected
        //
        if (pStation->ConnectContext.ConnectState != CONN_STATE_DISCONNECTED)
        {
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));        
            STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);

            MPASSERT(pStation->ConnectContext.ConnectState == CONN_STATE_IN_RESET);
            
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Connecting while connected/in reset\n"));

            //
            // Exit without doing any thing
            //
            return NDIS_STATUS_RESET_IN_PROGRESS;
        }
    }
    else
    {
        //
        // Proceed only if we are not in reset/disconnected
        //
        if (pStation->ConnectContext.ConnectState != CONN_STATE_READY_TO_CONNECT)
        {
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));        
            STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);

            MpTrace(COMP_ASSOC, DBG_LOUD, ("Connecting while not expected to connect\n"));

            //
            // Exit without doing any thing
            //
            return NDIS_STATUS_FAILURE;
        }
    }
    
    do
    {
        //
        // On a connect request, we are ready to start the connection
        //
        pStation->ConnectContext.AssociateState = ASSOC_STATE_READY_TO_ASSOCIATE;
        pStation->ConnectContext.PreviousConnectState = pStation->ConnectContext.ConnectState;
        pStation->ConnectContext.ConnectState = CONN_STATE_READY_TO_CONNECT;

        //
        // If we are currently scanning, stop the scan. This is best attempt effort
        // If the OS requests a new scan we may let it continue (forced). 
        // We only check this when we are connecting in context of NdisRequest.
        // After that we are always called from ScanComplete and our scan in 
        // progress check would always be true
        // 
        if (FirstAttempt && (STA_TEST_SCAN_FLAG(pStation, STA_PERIODIC_SCAN_IN_PROGRESS) == TRUE))
        {
        
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

            MpTrace(COMP_ASSOC, DBG_LOUD, ("Cancelling pending periodic scan to connect\n"));

            //
            // First time called in the context of NdisRequest at passive.
            // We wait for scan to complete
            //
            StaWaitForPeriodicScan(pStation);
        }
        else
        {
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        }

        //
        // If we are in power saving mode, temporarily set to active mode.
        //
        origPowerSavingLevel = pStation->Config.PowerSavingLevel;
        if (origPowerSavingLevel != DOT11_POWER_SAVING_NO_POWER_SAVING)
        {
            StaSetPowerSavingLevel(pStation, DOT11_POWER_SAVING_NO_POWER_SAVING);
            //
            // Save the original level back so that on complete we
            // restore it
            //
            pStation->Config.PowerSavingLevel = origPowerSavingLevel;
        }

        //
        // Get list of access points that we can associate with
        //
        ndisStatus = StaGetCandidateAPList(
            pStation,
            FALSE                // Find as many APs as possible
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Unable to obtain candidate access points list\n"));
            bScanForAP = TRUE;
            break;
        }

        //
        // Any other failures are because we couldnt associate with any access point
        //
        CompletionStatus = DOT11_CONNECTION_STATUS_CANDIDATE_LIST_EXHAUSTED;

        if (FirstAttempt)
        {
            if (pStation->ConnectContext.CandidateAPCount == 0)
            {
                //
                // Either havent found any access points yet or not
                // able to find a matching access point. We will queue
                // the periodic scan routine to attempt to find APs
                // and connect
                //
                MpTrace(COMP_ASSOC, DBG_LOUD, ("Did not find any candidate access points\n"));
                bScanForAP = TRUE;
                break;
            }
        }
        // We have an AP we can connect to or we have already scanned and still didnt find AP
        // in which case we would just complete the connection and let the OS try another profile        
        bScanForAP = FALSE;
        
        //
        // Connection start status indication
        //
        StaIndicateConnectionStart(pStation);        
        bIndicateComplete = TRUE;
        
        //
        // Initiate association with the first access point in the candidate
        // access points list
        //
        pStation->ConnectContext.CurrentCandidateAPIndex = 0;

        while (pStation->ConnectContext.CurrentCandidateAPIndex < pStation->ConnectContext.CandidateAPCount)
        {
            pAPEntry = pStation->ConnectContext.CandidateAPList[pStation->ConnectContext.CurrentCandidateAPIndex];

            //
            // Did the state change while we were doing something else?
            //
            NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
            if (pStation->ConnectContext.ConnectState < CONN_STATE_READY_TO_CONNECT)
            {
                //
                // We got a reset, etc while we were attempting to connect
                //
                NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
                
                // Abort (we wont even indicate connection complete)
                CompletionStatus = DOT11_CONNECTION_STATUS_CANCELLED;

                MpTrace(COMP_ASSOC, DBG_LOUD, ("Connection aborted due to reset, etc\n"));
                break;
            }
            pStation->ConnectContext.AssociateState = ASSOC_STATE_READY_TO_ASSOCIATE;
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            
            ndisStatus = StaStartAssociationProcess(pStation, pAPEntry);
            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                //
                // Association with first access point started. It
                // will complete asynchronously
                //
                bIndicateComplete = FALSE;
                break;
            }
            //
            // Association start failed. If we cannot find a new access point
            // we will complete the connection attempt with failure
            //

            // Association with current access points failed. Move to the next access point (if present)
            pStation->ConnectContext.CurrentCandidateAPIndex++;

        } 
        
    }while (FALSE);

    if (bScanForAP == TRUE)
    {
        MPASSERT(bIndicateComplete == FALSE);  // Exclusive (else ref/complete status not protected)
        
        //
        // We were unable to find an access point to associate with.
        // Lets scan for the access point.
        //
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        pStation->ConnectContext.AssociateState = ASSOC_STATE_NOT_ASSOCIATED;
        MPASSERT(pStation->ConnectContext.ConnectState != CONN_STATE_READY_TO_ROAM);
        
        if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
        {
            //
            // Do a scan routine to find an access point
            // to connect with (not for roaming)
            //
            StaForceInternalScan(pStation, FALSE);
        }
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        
        //
        // Remove the extra refcount we had added for connect. The scan
        // may not run so cannot require it to decrement the ref count
        //
        STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);
    }
    else if (bIndicateComplete == TRUE)
    {
        //
        // We have indicated connection start. Indicate 
        // connection completion status (would also remove the extra 
        // asynchronous function ref to let reset continue)
        //
        StaConnectComplete(pStation, CompletionStatus);
    }
    
    return ndisStatus;
}


VOID
StaConnectComplete(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  ULONG           CompletionStatus
    )
{
    ULONG               i = 0;
    BOOLEAN             bResetConnectState = TRUE;
    BOOLEAN             bCompleteRoam = FALSE;

    //
    // Reset Aid if connection failed.
    //
    if (CompletionStatus != DOT11_CONNECTION_STATUS_SUCCESS)
    {
        pStation->Config.AID = 0;
    }

    //
    // We were set to active mode to do the connection. But if we are supposed to 
    // be in power saving mode, restore it. This needs to be done before we indicate
    // connection complete for synchronization reason.
    //
    if (pStation->Config.PowerSavingLevel != DOT11_POWER_SAVING_NO_POWER_SAVING)
    {
        StaSetPowerSavingLevel(pStation, pStation->Config.PowerSavingLevel);
    }

    //
    // Release references from all the access points from the candidate APs (if we were associated
    // the active access point would have have an extra reference)
    //
    for (i = 0; i < pStation->ConnectContext.CandidateAPCount; i++)
    {
        STA_DECREMENT_REF(pStation->ConnectContext.CandidateAPList[i]->RefCount);
    }
    pStation->ConnectContext.CandidateAPCount = 0;

    //
    // Update association state
    //        
    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
    if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
    {
        if (pStation->ConnectContext.ConnectState == CONN_STATE_READY_TO_CONNECT)
        {
            // Indicate connection completion status
            bCompleteRoam = FALSE;
        }
        else
        {
            // Indicate roaming completion status
            bCompleteRoam = TRUE;
        }
        
        if (CompletionStatus != DOT11_CONNECTION_STATUS_SUCCESS)
        {
            //
            // Association failed
            //
            pStation->ConnectContext.AssociateState = ASSOC_STATE_NOT_ASSOCIATED;
        }
        else
        {
            //
            // Association succeed. Next time we need to scan to roam, our scan
            // can start quicker
            //
            pStation->ScanContext.RoamingScanGap = STA_PERIODIC_SCAN_SHORT_GAP;
        }

        //
        // Once we have indicate a connection start/complete status indication
        // we cannot indicate another one until we reset. Any new connection
        // we do must be a roam indication
        //
        pStation->ConnectContext.PreviousConnectState = pStation->ConnectContext.ConnectState;
        pStation->ConnectContext.ConnectState = CONN_STATE_READY_TO_ROAM;
    }
    else
    {
        //
        // We are being reset or disconnected. Figure out if we 
        // are completing a roam or a connect
        //
        if (pStation->ConnectContext.PreviousConnectState == CONN_STATE_READY_TO_CONNECT)
        {
            // Indicate connection completion status
            bCompleteRoam = FALSE;
        }
        else
        {
            // Indicate roaming completion status
            bCompleteRoam = TRUE;
        }
        
        // The connect state will be cleared by the reset routine
        bResetConnectState = FALSE;
        
    }
    NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

    //
    // Indicate completion status
    //
    if (bCompleteRoam)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Roaming Complete Status Indication %d\n", CompletionStatus));
        StaIndicateRoamingCompletion(pStation, CompletionStatus);
    }
    else
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Connection Complete Status Indication %d\n", CompletionStatus));
        StaIndicateConnectionCompletion(pStation, CompletionStatus);

        if (bResetConnectState)
        {
            if (CompletionStatus != DOT11_CONNECTION_STATUS_SUCCESS)
            {
                //
                // If we do a connect complete status indication with failure,
                // its over from the OS's point of view and it will use a fresh 
                // user profile and send us a new connect request. Any further  
                // roaming status indications would ignored by the OS. 
                // We set ourselves to CONN_STATE_DISCONNECTED and not do
                // any roaming
                //
                NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
                if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
                {
                    pStation->ConnectContext.ConnectState = CONN_STATE_DISCONNECTED;
                }
                NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            }
        }
    }
    
    //
    // Connect/roam process is done
    //
    STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);
}

NDIS_STATUS
StaConnectInfra(
    _In_  PMP_EXTSTA_PORT        pStation
    )
{
    return StaConnectStart(pStation, TRUE);
}


NDIS_STATUS
StaDisconnect(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  STA_ASSOC_STATE OriginalState
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;

    //
    // SYNC: This is a temporary check to verify that we havent yet lost
    // the access point ref. This assert has synchronization issues
    //
    MPASSERT(pAPEntry->RefCount > 1);

    pAPEntry->AssocState = dot11_assoc_state_unauth_unassoc;
    
    if (OriginalState > ASSOC_STATE_WAITING_FOR_ASSOCIATE)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Disconnect will disassociate\n"));

        //
        // Send disassociation to the access point
        //
        ndisStatus = StaSendDisassociation(
            pStation, 
            pAPEntry, 
            DOT11_MGMT_REASON_DISASSO_LEAVE_SS
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Failed sending disassociation packet\n"));
            // This status may get overwritten when we send the deauthentication
        }
    }
    
    if (OriginalState > ASSOC_STATE_WAITING_FOR_AUTHENTICATE)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Disconnect will deauthenticate\n"));
        
        //
        // Send deauthentication to the access point
        //
        ndisStatus = StaSendDeauthentication(
            pStation, 
            pAPEntry, 
            DOT11_MGMT_REASON_DEAUTH_LEAVE_SS
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Failed sending deauthenticate packet\n"));
        }        
    }

    if (OriginalState == ASSOC_STATE_ASSOCIATED)
    {
        //
        // We only indicate disconnect if we were associated and are being reset or
        // being disconnected
        //

        //
        // Indicate disconnect status
        //
        StaIndicateDisassociation(
            pStation, 
            pAPEntry, 
            DOT11_DISASSOC_REASON_OS
            );
    }

    return ndisStatus;
}

NDIS_STATUS
StaDisconnectInfra(
    _In_  PMP_EXTSTA_PORT        pStation
    )
{
    PSTA_INFRA_CONNECT_CONTEXT    pConnectContext = &(pStation->ConnectContext);

    NdisAcquireSpinLock(&(pConnectContext->Lock));
    //
    // Only need to proceed if we are not already reset, etc
    //
    if (pConnectContext->ConnectState >= CONN_STATE_READY_TO_CONNECT)
    {
        //
        // We are either connected or connecting due to roaming. If 
        // connecting, we need to wait for the connection process
        // to end.
        //
        NdisReleaseSpinLock(&(pConnectContext->Lock));
        StaResetConnection(pStation, TRUE);
    }
    else
    {
        // Either already disconnected or in reset, we dont 
        // proceed
        NdisReleaseSpinLock(&(pConnectContext->Lock));
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Ignored bogus disconnect because we are in reset or disconnected\n"));
        return NDIS_STATUS_FAILURE;
    }

    return NDIS_STATUS_SUCCESS;
}

// Connection lock must NOT be held
NDIS_STATUS
StaStartAssociationProcess(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;

    //
    // Set up for starting the association process
    //
    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));    

    if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
    {
        MPASSERT(pStation->ConnectContext.AssociateState == ASSOC_STATE_READY_TO_ASSOCIATE);
        //
        // Save the access point being used for the association
        //
        pStation->ConnectContext.ActiveAP = pAPEntry;
        
        //
        // Add an extra refcounts to the active AP
        //
        STA_INCREMENT_REF(pAPEntry->RefCount);
    }
    else
    {
        // Reset, etc
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Start Association process failed because in reset\n"));
        return NDIS_STATUS_FAILURE;        
    }

    //
    // We are now in the process of associating
    //
    pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
    pAPEntry->AssocState = dot11_assoc_state_unauth_unassoc;
    
    NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

    MpTrace(COMP_ASSOC, DBG_LOUD, ("Initiating association with AP: %02X-%02X-%02X-%02X-%02X-%02X\n", 
        pAPEntry->Dot11BSSID[0], pAPEntry->Dot11BSSID[1], pAPEntry->Dot11BSSID[2], 
        pAPEntry->Dot11BSSID[3], pAPEntry->Dot11BSSID[4], pAPEntry->Dot11BSSID[5]));

    //
    // Indicate association start status indication
    //
    ndisStatus = StaIndicateAssociationStart(pStation, pAPEntry);
    if (ndisStatus == NDIS_STATUS_SUCCESS)
    {
        //
        // After this point, we will indicate associate complete, connect
        // complete irrespective of whether we are in reset or not
        //

        //
        // First step for our association is to 
        // synchronize timers with the AP
        //
        ndisStatus = StaJoin(
            pStation,
            pAPEntry,
            STA_JOIN_FAILURE_TIMEOUT
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Join failed synchronously\n"));
            
            // Indicate association complete status indication. Do not call
            // StaCompleteAssociationProcess here since it may call us back to attempt
            // association with a different candidate AP results in a 
            // recursive stack like:
            //
            //      :
            //      StaCompleteAssociationProcess
            //      StaStartAssociationProcess
            //      StaCompleteAssociationProcess
            //      StaStartAssociationProcess
            //      StaConnectInfra
            //

            //
            // Indicate association completion status
            //
            StaIndicateAssociationCompletion(pStation, pAPEntry, DOT11_ASSOC_STATUS_SYSTEM_ERROR);

            NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
            pStation->ConnectContext.ActiveAP = NULL;
            pStation->ConnectContext.AssociateState = ASSOC_STATE_READY_TO_ASSOCIATE; 
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

            //
            // Remove the extra refcount from the AP
            //
            STA_DECREMENT_REF(pAPEntry->RefCount);

            //
            // Caller will reattempt to associate
            //
        }
    }
    else
    {
        //
        // Remove the extra (in connection) refcount
        //
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        pStation->ConnectContext.ActiveAP = NULL;
        pStation->ConnectContext.AssociateState = ASSOC_STATE_READY_TO_ASSOCIATE; 
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        STA_DECREMENT_REF(pAPEntry->RefCount);
    }
    
    return ndisStatus;
}

VOID
StaCompleteAssociationProcess(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  ULONG           Status
    )
{
    PMP_BSS_ENTRY      pAPEntry = NULL;
    NDIS_STATUS         ndisStatus = NDIS_STATUS_FAILURE;
    ULONG               CompletionStatus = DOT11_CONNECTION_STATUS_IHV_START;
    BOOLEAN             bTryNextAP = TRUE;

    pAPEntry = pStation->ConnectContext.ActiveAP;   // It cannot be null at this time
    MPASSERT(pAPEntry);
    
    //
    // Indicate the association complete status indication
    //
    ndisStatus = StaIndicateAssociationCompletion(pStation, pAPEntry, Status);
    // We currently ignore return status from status indication

    //
    // If failure, attempt to begin association with next access point.
    //
    if (Status != 0)
    {
        MpTrace(COMP_ASSOC, DBG_NORMAL, ("Association process has failed with status 0x%x\n", Status));
        
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        
        if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, ("We will continue attempting to connect\n"));
        
            //
            // Connection failed. But we can still attempt to associate
            //
            bTryNextAP = TRUE;
            pStation->ConnectContext.ActiveAP = NULL;
            CompletionStatus = DOT11_CONNECTION_STATUS_CANDIDATE_LIST_EXHAUSTED;
            pStation->ConnectContext.AssociateState = ASSOC_STATE_READY_TO_ASSOCIATE;   // For next association attempt
        }
        else
        {
            MpTrace(COMP_ASSOC, DBG_LOUD, ("We give up attempting to connect\n"));

            //
            // Connection process has to be cancelled because of reset or
            // disconnect. Dont try the next AP
            //
            bTryNextAP = FALSE;
            pStation->ConnectContext.AssociateState = ASSOC_STATE_NOT_ASSOCIATED;   // Reset to not associated
            CompletionStatus = DOT11_CONNECTION_STATUS_CANCELLED;
        }
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

        //
        // We didnt succeed associating the AP. Remove the connected refcount and
        // update association cost
        //
        pAPEntry->AssocCost += STA_ASSOC_COST_FAILED_CONNECT;
        pAPEntry->AssocState = dot11_assoc_state_unauth_unassoc;
        STA_DECREMENT_REF(pAPEntry->RefCount);
        ndisStatus = NDIS_STATUS_FAILURE;
        
        if (bTryNextAP == TRUE)
        {
            //
            // Get the next access point
            //
            pStation->ConnectContext.CurrentCandidateAPIndex++;
            
            while (pStation->ConnectContext.CurrentCandidateAPIndex < pStation->ConnectContext.CandidateAPCount)
            {
                pAPEntry = pStation->ConnectContext.CandidateAPList[pStation->ConnectContext.CurrentCandidateAPIndex];

                ndisStatus = StaStartAssociationProcess(pStation, pAPEntry);
                
                if (ndisStatus == NDIS_STATUS_SUCCESS)
                {
                    // Association with access point started. It
                    // will complete asynchronously (function exit below)
                    break;
                }
                
                //
                // This connection failed, try the next access point in the list
                //
                NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
                if (pStation->ConnectContext.ConnectState < CONN_STATE_READY_TO_CONNECT)
                {
                    //
                    // We got a reset, etc while we were attempting to connect
                    //
                    NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

                    // Cancel
                    CompletionStatus = DOT11_CONNECTION_STATUS_CANCELLED;            

                    MpTrace(COMP_ASSOC, DBG_LOUD, ("Connection aborted due to reset, etc\n"));            
                    break;
                }
                NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

                // We will loop through and try next AP in the list

                // Association with current access point failed.
                pStation->ConnectContext.CurrentCandidateAPIndex++;
            }

            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                //
                // Association process has been restarted, no need
                // to indicate connection completion
                //
                return;
            }
        }
    }
    else
    {
        //
        // Association successful
        //
        CompletionStatus = DOT11_CONNECTION_STATUS_SUCCESS;
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        MPASSERT(pStation->ConnectContext.AssociateState == ASSOC_STATE_RECEIVED_ASSOCIATE);
        pStation->ConnectContext.AssociateState = ASSOC_STATE_ASSOCIATED;
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
    }
    
    //    
    // Complete connection
    //
    StaConnectComplete(pStation, CompletionStatus);
}

NDIS_STATUS
StaInfraJoinCompletionHandler(
    _In_ PMP_PORT           Port,
    _In_ PVOID              Data
    )
{    
    PMP_EXTSTA_PORT         pStation = MP_GET_STA_PORT(Port);
    NDIS_STATUS             ndisStatus = *((PNDIS_STATUS)Data);

    //
    // Handle reset/disconnect coming in before the join complete
    //
    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
    
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        MpTrace(COMP_ASSOC, DBG_SERIOUS, ("Join failed asynchronously with status 0x%x\n", ndisStatus));
    
        //
        // If join has failed (very likely because of timeout),
        // we give up on the association attempts with this access points
        //
        MPASSERT(pStation->ConnectContext.AssociateState == ASSOC_STATE_WAITING_FOR_JOIN);
        pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;

        if (ndisStatus == NDIS_STATUS_FILE_NOT_FOUND)
        {
            //
            // Join failed because we didnt receive a beacon from the AP. This means
            // we havent seen this AP, lower the last seen time. We dont
            // expire it yet, just lower it so that if we try again, we would this
            // this AP, but wont try it too many times
            //
            pStation->ConnectContext.ActiveAP->HostTimestamp = 
                pStation->ConnectContext.ActiveAP->HostTimestamp - (pStation->RegInfo->BSSEntryExpireTime
                + STA_BSS_ENTRY_EXPIRE_TIME_MIN);
        }
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

        //
        // Irrespective of whether join was cancelled or failed, we do complete
        // the association process
        //
        StaCompleteAssociationProcess(pStation, DOT11_ASSOC_STATUS_SYSTEM_ERROR);
    }
    else
    {
        //
        // Join has succeeded. Check if we go to the next stage
        //
        if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
        {
            MPASSERT(pStation->ConnectContext.AssociateState == ASSOC_STATE_WAITING_FOR_JOIN);
            pStation->ConnectContext.AssociateState = ASSOC_STATE_JOINED;        
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

            MpTrace(COMP_ASSOC, DBG_LOUD, ("Join successful\n"));

            //
            // Join has succeeded, authenticate with the access point
            //
            ndisStatus = StaAuthenticate(
                pStation,
                pStation->ConnectContext.ActiveAP,
                STA_AUTHENTICATE_FAILURE_TIMEOUT
                );
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                //
                // Authentication cannot be started. The StaAuthenticate
                // found would have already completed the association process.
                // We dont do anything
                //
            }
        }
        else
        {
            //
            // Join aborted
            //
            pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;

            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Join aborted\n"));

            StaCompleteAssociationProcess(pStation, DOT11_ASSOC_STATUS_CANCELLED);
        }
    }

    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
StaInfraJoinChannelSwitchCompletionHandler(
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
        ndisStatus = VNic11JoinBSS(
            STA_GET_VNIC(pStation),
            pStation->ConnectContext.JoinBSSDescription,
            pStation->ConnectContext.JoinFailureTimeout,
            StaInfraJoinCompletionHandler
            );
    }

    // Delete the join description
    if (pStation->ConnectContext.JoinBSSDescription)
    {
        MP_FREE_MEMORY(pStation->ConnectContext.JoinBSSDescription);
        pStation->ConnectContext.JoinBSSDescription = NULL;
    }

    if (ndisStatus == NDIS_STATUS_PENDING)
    {
        // Hardware will start the join and call us back
        // when the join completes or times out

        // This is success for the join attempt        
        ndisStatus = NDIS_STATUS_SUCCESS;
    }
    else 
    {
        // This implementation does not support Join succeeding 
        // synchronously
        MPASSERT(ndisStatus != NDIS_STATUS_SUCCESS);

        // Invoke the Join completion
        StaInfraJoinCompletionHandler(
            Port,
            &ndisStatus
            );
    }

    return ndisStatus;
}

// Caller handles completion in case of failure
NDIS_STATUS
StaJoin(
    _In_  PMP_EXTSTA_PORT        pStation,
    _In_  PMP_BSS_ENTRY  pAPEntry,
    _In_  ULONG           JoinFailureTimeout
    )
{
    NDIS_STATUS         ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_BSS_DESCRIPTION BSSDescription = NULL;

    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
    if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
    {
        MPASSERT(pStation->ConnectContext.AssociateState == ASSOC_STATE_STARTED_ASSOCIATION);
        pStation->ConnectContext.AssociateState = ASSOC_STATE_WAITING_FOR_JOIN;
    }
    else
    {
        // Disconnect/Reset request came in
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Disconnect/reset while attempting to join\n"));
        
        return NDIS_STATUS_FAILURE;
    }
    NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

    NdisAcquireSpinLock(&(pAPEntry->Lock));

    do
    {
        //
        // Allocate a BSS description structure (we allocate 3 bytes more for possible addition of
        // DS parameter IE, that contains channel number).
        //
        
        // Integer overflow
        if ((FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + pAPEntry->InfoElemBlobSize) > 
            (FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + pAPEntry->InfoElemBlobSize + 3))
        {
            ndisStatus = NDIS_STATUS_FAILURE;
            break;
        }

        MP_ALLOCATE_MEMORY(STA_GET_MP_PORT(pStation)->MiniportAdapterHandle,
                           &BSSDescription, 
                           FIELD_OFFSET(MP_BSS_DESCRIPTION, IEBlobs) + pAPEntry->InfoElemBlobSize + 3,
                           MP_MEMORY_TAG);
        if (BSSDescription == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        
        //
        // Initialize the BSS description structure from StaEntry
        //
        NdisMoveMemory(BSSDescription->BSSID, pAPEntry->Dot11BSSID, sizeof(DOT11_MAC_ADDRESS));
        NdisMoveMemory(BSSDescription->MacAddress, pAPEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));
        BSSDescription->BSSType = dot11_BSS_type_infrastructure;
        BSSDescription->BeaconPeriod = pAPEntry->BeaconInterval;
        BSSDescription->Timestamp = pAPEntry->BeaconTimestamp;
        BSSDescription->Capability.usValue = pAPEntry->Dot11Capability.usValue;
        NdisMoveMemory(BSSDescription->IEBlobs, pAPEntry->pDot11InfoElemBlob, pAPEntry->InfoElemBlobSize);
        // Use the same buffers for beacon and probe responses
        BSSDescription->BeaconIEBlobOffset = 0;
        BSSDescription->BeaconIEBlobSize = pAPEntry->InfoElemBlobSize;
        BSSDescription->ProbeIEBlobOffset = 0;
        BSSDescription->ProbeIEBlobSize = pAPEntry->InfoElemBlobSize;
        
        BSSDescription->IEBlobsSize = pAPEntry->InfoElemBlobSize;
        
        // Specify the Phy ID, channel to use
        BSSDescription->Channel = pAPEntry->Channel;
        BSSDescription->PhyId = pAPEntry->PhyId;

        // Save the channel of this association
        pStation->ConnectContext.AssociationChannel = pAPEntry->Channel;
    } while (FALSE);        
    
    //
    // The VNic11JoinInfra function must make a copy of the 
    // AP entry information if it needed to use it. We can
    // unlock the structure
    //
    NdisReleaseSpinLock(&(pAPEntry->Lock));

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        return ndisStatus;
    }

    // Save these for later use
    pStation->ConnectContext.JoinFailureTimeout = JoinFailureTimeout;
    pStation->ConnectContext.JoinBSSDescription = BSSDescription;

    // Set the channel & PHY ID
    ndisStatus = VNic11SetChannel(STA_GET_VNIC(pStation), 
        BSSDescription->Channel, 
        BSSDescription->PhyId, 
        TRUE,
        StaInfraJoinChannelSwitchCompletionHandler
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
        ndisStatus = StaInfraJoinChannelSwitchCompletionHandler(STA_GET_MP_PORT(pStation), &ndisStatus);
    }
    else
    {
        // Channel switch has failed synchronously. We return to the caller who would
        // handle the association completion, try with next
        // access point, etc
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        MPASSERT(pStation->ConnectContext.AssociateState == ASSOC_STATE_WAITING_FOR_JOIN);
        pStation->ConnectContext.AssociateState = ASSOC_STATE_STARTED_ASSOCIATION;
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
    }

    return ndisStatus;
}

NDIS_STATUS
StaEnumerateAssociationInfoInfra(
    _In_  PMP_EXTSTA_PORT        pStation,
    _Out_ PDOT11_ASSOCIATION_INFO_LIST   pAssocInfoList,
    _In_  ULONG           TotalLength
    )
{
    PMP_BSS_ENTRY      pAPEntry = NULL;
    PDOT11_ASSOCIATION_INFO_EX  pAssocInfo = NULL;

    UNREFERENCED_PARAMETER(TotalLength);    // One entry is in assocInfoList itself
    
    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
    if (pStation->ConnectContext.ActiveAP != NULL)
    {
        // We have an entry saved. Increment refcount
        pAPEntry = pStation->ConnectContext.ActiveAP;
        
        STA_INCREMENT_REF(pAPEntry->RefCount);
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));        
    }
    else
    {
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

        // No information to fill
        pAssocInfoList->uNumOfEntries = 0;
        pAssocInfoList->uTotalNumOfEntries = 0;

        return NDIS_STATUS_SUCCESS;
    }

    pAssocInfo = &(pAssocInfoList->dot11AssocInfo[0]);

    NdisZeroMemory(pAssocInfo, sizeof(DOT11_ASSOCIATION_INFO_EX));

    NdisMoveMemory(pAssocInfo->PeerMacAddress, pAPEntry->MacAddress, sizeof(DOT11_MAC_ADDRESS));
    NdisMoveMemory(pAssocInfo->BSSID, pAPEntry->Dot11BSSID, sizeof(DOT11_MAC_ADDRESS));
    pAssocInfo->usCapabilityInformation = pAPEntry->Dot11Capability.usValue;
    pAssocInfo->usListenInterval = pStation->Config.ListenInterval;

    pAssocInfo->usAssociationID = pAPEntry->AssocID;
    pAssocInfo->dot11AssociationState = pAPEntry->AssocState;
    pAssocInfo->liAssociationUpTime.QuadPart = pAPEntry->AssociationUpTime.QuadPart;    

    // TODO: Supported rates

    //
    // We do not keep per station statistics
    //
    
    pAssocInfoList->uNumOfEntries = 1;
    pAssocInfoList->uTotalNumOfEntries = 1;
    
    // Remove the extra refcount
    STA_DECREMENT_REF(pAPEntry->RefCount);

    return NDIS_STATUS_SUCCESS;
}

// LOCK HELD
BOOLEAN
StaCanRoam(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    //
    // We dont roam for IBSS
    //
    if (pStation->Config.BSSType == dot11_BSS_type_independent)
    {
        return FALSE;
    }

    if (pStation->ConnectContext.ConnectState >= CONN_STATE_READY_TO_CONNECT)
    {
        if (pStation->ConnectContext.AssociateState == ASSOC_STATE_NOT_ASSOCIATED)
        {
            // We can associate, but are not currently associated.
            // We will attempt to roam
            return TRUE;
        }
        else if (pStation->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED)
        {
            //
            // Check if we perfomed the scan because we wanted to roam. We dont
            // check beacon time as that would check probably succeed since we just
            // performed a scan on other channels
            //
            if (pStation->ScanContext.ScanSeverity >=  SCAN_FIND_BETTER_AP)
            {
                return TRUE;
            }
            else
            {
                return FALSE;
            }
        }
    }

    //
    // Not required to connect or have good connection
    //
    return FALSE;
}



NDIS_STATUS
StaPerformRoam(
    _In_  PMP_EXTSTA_PORT        pStation
    )
{
    NDIS_STATUS     ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_BSS_ENTRY  pAPEntry = NULL;
    ULONG           CompletionStatus = DOT11_CONNECTION_STATUS_IHV_START;   // Default failure status
    ULONG           origPowerSavingLevel;
    BOOLEAN         bIndicateComplete = FALSE;
    BOOLEAN         bMustRoam = FALSE;
    BOOLEAN         bStrictFiltering = FALSE;   // Probably not connected, find all APs we can find
    ULONG           RoamingReason = DOT11_ROAMING_REASON_ASSOCIATION_LOST;
    ULONG           i;

    //
    // We dont want reset/halt to wait while we are still running. This refcount
    // wont be removed until connect complete is called. Reset/halt routine will wait
    // for this count to go to zero before returning to the OS
    //
    STA_INCREMENT_REF(pStation->ConnectContext.AsyncFuncCount);

    NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));

    //
    // Proceed only if we are not in reset/disconnected
    //
    if (pStation->ConnectContext.ConnectState != CONN_STATE_READY_TO_ROAM)
    {
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));        
        STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);

        //
        // Exit without doing any thing
        //
        return NDIS_STATUS_FAILURE;
    }

    if (pStation->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED)
    {
        //
        // Add ref to currently associate AP so that it does not go away and we
        // can use it for comparison
        //
        pAPEntry = pStation->ConnectContext.ActiveAP;
        STA_INCREMENT_REF(pAPEntry->RefCount);

        //
        // If we must roam, even if we do not find a better AP, we will 
        // send a disconnect request to the OS
        //
        if (pStation->ScanContext.ScanSeverity >= SCAN_FIND_USABLE_AP)
            bMustRoam = TRUE;
        bStrictFiltering = !bMustRoam;  // If we must roam, dont do stricter filter for candidate APs
    }
    NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));        
    
    do
    {
        //
        // Get list of access points that we can associate with
        //
        ndisStatus = StaGetCandidateAPList(
            pStation,
            bStrictFiltering
            );
        if ((ndisStatus != NDIS_STATUS_SUCCESS) || 
            ((bMustRoam == FALSE) &&
             ((pStation->ConnectContext.CandidateAPCount == 0) ||
              (pStation->ConnectContext.CandidateAPList[0] == pAPEntry))
            )
           )
        {
            //
            // Second check above (bMustRoam ...) checks that if we dont need to roam, we dont
            // proceed forward with spurious roaming status indications if we are going
            // to the same AP. We only proceed if we find a better access point
            //
            
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Unable to obtain better access points list\n"));
            
            //
            // Remove all the extra references that were added
            //

            //
            // The one we added to the AP earlier
            //
            if (pAPEntry != NULL)
            {
                STA_DECREMENT_REF(pAPEntry->RefCount);
            }

            //
            // Candidate AP list contains extra ref count
            //
            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                for (i = 0; i < pStation->ConnectContext.CandidateAPCount; i++)
                {
                    // CandidateAPList entries are not NULL due to CandidateAPCount
                    if (pStation->ConnectContext.CandidateAPList[i] != NULL)
                    {
                        STA_DECREMENT_REF(pStation->ConnectContext.CandidateAPList[i]->RefCount);
                    }
                }
            }
            
            //
            // Remove function ref count
            //            
            STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);        
            break;
        }

        //
        // We may have changed state by now, check
        //
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
        if (pStation->ConnectContext.ConnectState == CONN_STATE_READY_TO_ROAM)
        {
            if (pStation->ConnectContext.AssociateState == ASSOC_STATE_ASSOCIATED)
            {
                MPASSERT(pAPEntry != NULL);
                pStation->ConnectContext.AssociateState = ASSOC_STATE_READY_TO_ASSOCIATE;
                NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));

                //
                // We are going to roam. Send disconnect packet to the AP we are currently
                // associated with
                //
                StaSendDeauthentication(pStation, pAPEntry, DOT11_MGMT_REASON_UPSPEC_REASON);
                RoamingReason = DOT11_ROAMING_REASON_BETTER_AP_FOUND;

                //
                // Indicate disassociation to let the OS know we have disassociated. The OS
                // will give us some time to roam or would reset us
                //
                StaIndicateDisassociation(pStation, pAPEntry, DOT11_ASSOC_STATUS_UNREACHABLE);

                //
                // We are going to disconnect from this AP. Next time we associate, we want to give 
                // this AP lower preference for an association. This helps avoid thrashing between
                // APs
                //
                pAPEntry->AssocCost += STA_ASSOC_COST_ROAM_DISCONNECT;

                //
                // Decrement the extra connected refcount on this AP
                //
                STA_DECREMENT_REF(pAPEntry->RefCount);
            }
            else
            {
                pStation->ConnectContext.AssociateState = ASSOC_STATE_READY_TO_ASSOCIATE;
                NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            }

            //
            // This ref is no longer necessary. FYI: APEntry may not be null even if
            // current AssociateState is not ASSOC_STATE_ASSOCIATED (eg. remotely deauth)
            //
            if (pAPEntry != NULL)
            {
                STA_DECREMENT_REF(pAPEntry->RefCount);
            }
        }
        else
        {
            //
            // Reset or Disconnected
            //
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Reset while trying to find APs to associate with\n"));
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            
            //
            // Remove all the extra references that were added
            //

            //
            // On the AP that we added earlier
            //
            if (pAPEntry != NULL)
            {
                STA_DECREMENT_REF(pAPEntry->RefCount);
            }

            //
            // Candidate AP list contains extra ref count
            //
            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                for (i = 0; i < pStation->ConnectContext.CandidateAPCount; i++)
                {
                    STA_DECREMENT_REF(pStation->ConnectContext.CandidateAPList[i]->RefCount);
                }
            }
            
            //
            // Remove function ref count
            //            
            STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);
            
            break;
        }

        if ((pStation->ConnectContext.CandidateAPCount == 0) && 
            (pAPEntry != NULL))
        {
            // If we just indicated a disassociate & we havent yet found a new AP, it could
            // be because didnt do a full scan (ScanSeverity  == SCAN_REDISCOVER_CURRENT_AP), lets
            // not do the roaming indications
            MpTrace(COMP_ASSOC, DBG_LOUD, ("Continuing attempt to find AP to associate to\n"));
            STA_DECREMENT_REF(pStation->ConnectContext.AsyncFuncCount);
            break;
        }

        //
        // If we are in power saving mode, temporarily set to active mode.
        //
        origPowerSavingLevel = pStation->Config.PowerSavingLevel;
        if (origPowerSavingLevel != DOT11_POWER_SAVING_NO_POWER_SAVING)
        {
            StaSetPowerSavingLevel(pStation, DOT11_POWER_SAVING_NO_POWER_SAVING);
            //
            // Save the original level back so that on complete we
            // restore it
            //            
            pStation->Config.PowerSavingLevel = origPowerSavingLevel;
        }

        //
        // Any other failures are because we couldnt associate with any access point
        //
        CompletionStatus = DOT11_CONNECTION_STATUS_CANDIDATE_LIST_EXHAUSTED;
        
        //
        // Roaming start status indication. We have always broken connectivity by this time
        //
        StaIndicateRoamingStart(pStation, RoamingReason, FALSE);
        bIndicateComplete = TRUE;
        
        //
        // Initiate association with the first access point in the candidate
        // access points list
        //
        pStation->ConnectContext.CurrentCandidateAPIndex = 0;

        while (pStation->ConnectContext.CurrentCandidateAPIndex < pStation->ConnectContext.CandidateAPCount)
        {
            pAPEntry = pStation->ConnectContext.CandidateAPList[pStation->ConnectContext.CurrentCandidateAPIndex];

            //
            // Did the state change while we were doing something else?
            //
            NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
            if (pStation->ConnectContext.ConnectState != CONN_STATE_READY_TO_ROAM)
            {
                //
                // We got a reset, etc while we were attempting to connect
                //
                NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
                
                // Abort (we wont even indicate connection complete)
                CompletionStatus = DOT11_CONNECTION_STATUS_CANCELLED;

                MpTrace(COMP_ASSOC, DBG_LOUD, ("Connection aborted due to reset, etc\n"));
                break;
            }
            pStation->ConnectContext.AssociateState = ASSOC_STATE_READY_TO_ASSOCIATE;
            NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
            
            ndisStatus = StaStartAssociationProcess(pStation, pAPEntry);
            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                //
                // Association with first access point started. It
                // will complete asynchronously
                //
                bIndicateComplete = FALSE;
                break;
            }
            //
            // Association start failed. If we cannot find a new access point
            // we will complete the connection attempt with failure
            //

            // Association with current access points failed. Move to the next access point (if present)
            pStation->ConnectContext.CurrentCandidateAPIndex++;

        } 
        
    }while (FALSE);

    if (bIndicateComplete == TRUE)
    {
        //
        // We have indicated connection start. Indicate 
        // connection completion status (would also remove the extra 
        // asynchronous function ref to let reset continue)
        //
        StaConnectComplete(pStation, CompletionStatus);
    }
    
    return ndisStatus;
}



// LOCK HELD
_Requires_lock_held_((&pStation->ConnectContext.Lock)->SpinLock)
_IRQL_requires_(DISPATCH_LEVEL)
VOID
StaRoamStart(
    _In_  PMP_EXTSTA_PORT            pStation
    )
{
    if (StaCanRoam(pStation))
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Attempting roam\n"));
        
        //
        // Initiate the connection (need to release lock)
        //
        NdisReleaseSpinLock(&(pStation->ConnectContext.Lock));
        StaPerformRoam(pStation);
        NdisAcquireSpinLock(&(pStation->ConnectContext.Lock));
    }
}

VOID 
StaResetRoamState(
    _In_  PMP_EXTSTA_PORT        pStation
    )
{
    PLIST_ENTRY         pHead = NULL, pEntry = NULL;
    PMP_BSS_ENTRY      pStaBSSEntry = NULL;
    MP_RW_LOCK_STATE          LockState;
    PMP_BSS_LIST                pDiscoveredAPList = NULL;

    // NOTE: If we support multiple STA, we should reset the roam state only 
    // for BSS entries that match my profile
    
    // Clear fast scan state
    pStation->ConnectContext.AttemptFastRoam = FALSE;

    //
    // Reset the priority values on all AP entries
    //

    pDiscoveredAPList = Mp11QueryAndRefBSSList(
                            STA_GET_MP_PORT(pStation)->Adapter, 
                            STA_GET_MP_PORT(pStation),
                            &LockState
                            );
    if (pDiscoveredAPList == NULL)
    {
        MpTrace(COMP_ASSOC, DBG_LOUD, ("Unable to obtain BSS list to reset the roam state\n"));
    }
    else
    {
        pHead = &(pDiscoveredAPList->List);
        pEntry = pHead->Flink;
        while(pEntry != pHead) 
        {
            pStaBSSEntry = CONTAINING_RECORD(pEntry, MP_BSS_ENTRY, Link);
            pEntry = pEntry->Flink;

            pStaBSSEntry->AssocCost = 0;
            pStaBSSEntry->LowQualityCount = 0;
        }

        Mp11ReleaseBSSListRef(STA_GET_MP_PORT(pStation)->Adapter, 
            pDiscoveredAPList, 
            &LockState
            );
    }
    
    //
    // Reset other roam state
    //
    pStation->ScanContext.RoamingScanGap = STA_PERIODIC_SCAN_SHORT_GAP;
    pStation->ScanContext.ScanSeverity = SCAN_LOW_SEVERITY;
}
