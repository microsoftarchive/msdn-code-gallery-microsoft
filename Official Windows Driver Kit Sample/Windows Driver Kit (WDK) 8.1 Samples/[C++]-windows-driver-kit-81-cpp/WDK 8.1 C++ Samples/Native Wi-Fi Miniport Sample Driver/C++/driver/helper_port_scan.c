/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    helper_port_scan.c

Abstract:
    Implements the scan functionality for the helper port
    
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
#include "helper_port_scan.h"
#include "vnic_intf.h"
#include "glb_utils.h"

#if DOT11_TRACE_ENABLED
#include "helper_port_scan.tmh"
#endif


NDIS_STATUS
HelperPortInitializeScanContext(
    _In_  PMP_HELPER_PORT         HelperPort
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NDIS_TIMER_CHARACTERISTICS  timerChar;               
    ULONG                       i;
    do
    {
        HelperPort->ScanContext.ActiveScanParameters = NULL;

        // Allocate the power save timeout call back
        NdisZeroMemory(&timerChar, sizeof(NDIS_TIMER_CHARACTERISTICS));
        
        timerChar.Header.Type = NDIS_OBJECT_TYPE_TIMER_CHARACTERISTICS;
        timerChar.Header.Revision = NDIS_TIMER_CHARACTERISTICS_REVISION_1;
        timerChar.Header.Size = sizeof(NDIS_TIMER_CHARACTERISTICS);
        timerChar.AllocationTag = PORT_MEMORY_TAG;
        
        timerChar.TimerFunction = HelperPortScanTimer;
        timerChar.FunctionContext = HelperPort;

        ndisStatus = NdisAllocateTimerObject(
                        HELPPORT_GET_MP_PORT(HelperPort)->MiniportAdapterHandle,
                        &timerChar,
                        &HelperPort->ScanContext.Timer_Scan
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate helper port scan timer\n"));
            break;
        }

        //
        // Get list of channels we would scan
        //
        ndisStatus = HelperPortCreateScanChannelList(HelperPort);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to create helper port scan list\n"));
            break;
        }        

        // Initialize the preallocated scan parameter structures
        for (i = 0; i < MP_MAX_NUMBER_OF_PORT; i++)
        {
            NdisZeroMemory(&HelperPort->ScanContext.ScanParameters[i], sizeof(MP_SCAN_PARAMETERS));
            
            HelperPort->ScanContext.ScanParameters[i].State = SCAN_EMPTY_REQUEST;
            HelperPort->ScanContext.ScanParameters[i].UsageCount = 0;
        }

        // To maintain the scan list, we need to receive all beacons and probe responses. Set the
        // appropriate packet filter
        VNic11SetPacketFilter(HELPPORT_GET_VNIC(HelperPort), 
            NDIS_PACKET_TYPE_802_11_BROADCAST_MGMT | NDIS_PACKET_TYPE_802_11_DIRECTED_MGMT
            );
        
    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (HelperPort->ScanContext.Timer_Scan)
        {
            NdisFreeTimerObject(HelperPort->ScanContext.Timer_Scan);
            HelperPort->ScanContext.Timer_Scan = NULL;
        }
    
        for (i = 0; i < HW11_MAX_PHY_COUNT; i++)
        {
            if (HelperPort->ScanContext.ScanChannels[i].ChannelList != NULL)
            {
                MP_FREE_MEMORY(HelperPort->ScanContext.ScanChannels[i].ChannelList);
                HelperPort->ScanContext.ScanChannels[i].ChannelList = NULL;
            }
        }

    }
    
    return ndisStatus;
}

VOID
HelperPortTerminateScanContext(
    _In_  PMP_HELPER_PORT         HelperPort
    )
{
    PMP_SCAN_PARAMETERS         scanParameters = NULL;

    ULONG                       i;

    // There can be scan requests around if we were waiting for exclusive access
    // for a request and it did not get satisfied
    if (HelperPort->ScanContext.ParametersCount != 0)
    {
        // We may have scan requests structures allocated, waiting for exclusive access
        // but that is never going to get satisfied. Free the requests
        for (i = 0; i < MP_MAX_NUMBER_OF_PORT; i++)
        {
            scanParameters = &HelperPort->ScanContext.ScanParameters[i];
            if (scanParameters->State != SCAN_EMPTY_REQUEST)
            {
                // The only condition in which this is OK is when we are waiting for 
                // an exclusive access
                MPASSERT(scanParameters->UsageCount == 1);
                HelperPortScanParametersReleaseRef(HelperPort, scanParameters);
            }
        }        
    }

    if (HelperPort->ScanContext.Timer_Scan)
    {
        NdisFreeTimerObject(HelperPort->ScanContext.Timer_Scan);
        HelperPort->ScanContext.Timer_Scan = NULL;
    }

    for (i = 0; i < HW11_MAX_PHY_COUNT; i++)
    {
        if (HelperPort->ScanContext.ScanChannels[i].ChannelList != NULL)
        {
            MP_FREE_MEMORY(HelperPort->ScanContext.ScanChannels[i].ChannelList);
            HelperPort->ScanContext.ScanChannels[i].ChannelList = NULL;
        }
    }
}

VOID
HelperPortNotify(
    _In_  PMP_PORT        Port,
    PVOID               Notif
    )
{
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    PNOTIFICATION_DATA_HEADER   notifyHeader = (PNOTIFICATION_DATA_HEADER)Notif;

    switch (notifyHeader->Type)
    {
        case NotificationOpLinkState:
        {
            POP_LINK_STATE_NOTIFICATION pMediaNotif = (POP_LINK_STATE_NOTIFICATION)Notif;
            
            MP_ACQUIRE_PORT_LOCK(Port, FALSE);
            
            if (pMediaNotif->MediaConnected)
            {
                MpTrace(COMP_SCAN, DBG_NORMAL, ("Incrementing connect count for CONNECT. current = %d\n", helperPort->ScanContext.MediaConnectedCount));
                MPASSERT(helperPort->ScanContext.MediaConnectedCount < MP_MAX_NUMBER_OF_PORT);
                helperPort->ScanContext.MediaConnectedCount++;
            }
            else
            {
                MpTrace(COMP_SCAN, DBG_NORMAL, ("Decrementing connect count for DISCONNECT. current = %d\n", helperPort->ScanContext.MediaConnectedCount));
                MPASSERT(helperPort->ScanContext.MediaConnectedCount > 0);
                helperPort->ScanContext.MediaConnectedCount--;
            }
            
            MP_RELEASE_PORT_LOCK(Port, FALSE);
            break;
        }
        default:
            break;
    }
}

NDIS_STATUS
HelperPortCreateScanChannelList(
    _In_  PMP_HELPER_PORT         HelperPort
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       phyId, channelListIndex = 0;
    PDOT11_SUPPORTED_PHY_TYPES  supportedPhyTypes;
    UCHAR                       buffer[(sizeof(DOT11_SUPPORTED_PHY_TYPES) + 
                                    sizeof(DOT11_PHY_TYPE) * HW11_MAX_PHY_COUNT)];  
    PMP_SCAN_CHANNEL_LIST       currentChannelList;
    DOT11_PHY_TYPE              bgScanPhy;

    do
    {
        //
        // Get supported PHY types.
        //
        supportedPhyTypes = (PDOT11_SUPPORTED_PHY_TYPES) buffer;
        supportedPhyTypes->uNumOfEntries = 0;
        VNic11QuerySupportedPHYTypes(HELPPORT_GET_VNIC(HelperPort), 
            HW11_MAX_PHY_COUNT, 
            supportedPhyTypes
            );

        //
        // For devices supporting both b & g phys, we only scan on g. Check if
        // we need to do this here
        //
        bgScanPhy = dot11_phy_type_hrdsss;
        for (phyId = 0; phyId < supportedPhyTypes->uNumOfEntries; phyId++)
        {
            if (supportedPhyTypes->dot11PHYType[phyId] == dot11_phy_type_erp)
            {
                // Support g, no need to scan b
                bgScanPhy = dot11_phy_type_erp;
                break;
            }
        }        

        //
        // Go through the list to see if there is a phy type we scan for
        //
        for (phyId = 0; phyId < supportedPhyTypes->uNumOfEntries; phyId++)
        {
            // We only scan on g(or b) & a phy
            if ((supportedPhyTypes->dot11PHYType[phyId] == bgScanPhy) ||
                (supportedPhyTypes->dot11PHYType[phyId] == dot11_phy_type_ofdm))
            {
                // Query for the channel list
                currentChannelList = &HelperPort->ScanContext.ScanChannels[channelListIndex];

                // Start with a zero length buffer to determine the size
                currentChannelList->ChannelCount = 0;
                ndisStatus = VNic11QuerySupportedChannels(HELPPORT_GET_VNIC(HelperPort), 
                                    phyId,
                                    &currentChannelList->ChannelCount, 
                                    NULL
                                    );
                if (ndisStatus != NDIS_STATUS_BUFFER_TOO_SHORT)
                {
                    MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Unable to query size of the channel list for phy ID %d\n", phyId));    
                    // We skip this PHY
                    ndisStatus = NDIS_STATUS_SUCCESS;
                    break;
                }
                
                MP_ALLOCATE_MEMORY(HELPPORT_GET_MP_PORT(HelperPort)->MiniportAdapterHandle,
                    &currentChannelList->ChannelList,
                    currentChannelList->ChannelCount * sizeof(ULONG),
                    PORT_MEMORY_TAG
                    );
                if (currentChannelList->ChannelList == NULL)
                {
                    MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Unable to allocate memory for the channel list for phy ID %d\n", phyId));
                    ndisStatus = NDIS_STATUS_RESOURCES;
                    break;
                }

                // Query again
                ndisStatus = VNic11QuerySupportedChannels(HELPPORT_GET_VNIC(HelperPort), 
                                    phyId,
                                    &currentChannelList->ChannelCount, 
                                    currentChannelList->ChannelList
                                    );
                if (ndisStatus != NDIS_STATUS_SUCCESS)
                {
                    MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Unable to query channel list for phy ID %d\n", phyId));
                    break;
                }

                currentChannelList->PhyId = phyId;
                
                // Populated one set of channels
                channelListIndex++;
            }

            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                break;
            }
        }
    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        for (channelListIndex = 0; channelListIndex < HW11_MAX_PHY_COUNT; channelListIndex++)
        {
            if (HelperPort->ScanContext.ScanChannels[channelListIndex].ChannelList != NULL)
            {
                MP_FREE_MEMORY(HelperPort->ScanContext.ScanChannels[channelListIndex].ChannelList);
                HelperPort->ScanContext.ScanChannels[channelListIndex].ChannelList = NULL;
            }
        }
    }

    return ndisStatus;
}

VOID
HelperPortScanParametersReleaseRef(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters
    )
{
    if (MP_DECREMENT_SCAN_PARAMETER_REF(ScanParameters) == 0)
    {
        // Free the channel list
        if (ScanParameters->VNicScanRequest.ChannelList != NULL)
        {
            MP_FREE_MEMORY(ScanParameters->VNicScanRequest.ChannelList );
            ScanParameters->VNicScanRequest.ChannelList = NULL;
        }

        // Release the structure
        HelperPort->ScanContext.ParametersCount--;    
        ScanParameters->State = SCAN_EMPTY_REQUEST;
    }
}

NDIS_STATUS
HelperPortHandleScan(
    _In_  PMP_PORT                Port,
    _In_  PMP_PORT                RequestingPort,
    _In_  PMP_SCAN_REQUEST        ScanRequest,
    _In_  PORT11_GENERIC_CALLBACK_FUNC    CompletionHandler
    )
{
    PMP_SCAN_PARAMETERS         scanParameters = NULL;
    // If we cannot find an empty slot, we return media in use
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_DOT11_MEDIA_IN_USE; 
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    ULONG                       i;

    do
    {
        // Add this scan request to the pending scan list
        MP_ACQUIRE_PORT_LOCK(Port, FALSE);

        // Find a structure we can use
        for (i = 0; i < MP_MAX_NUMBER_OF_PORT; i++)
        {
            if (helperPort->ScanContext.ScanParameters[i].State == SCAN_EMPTY_REQUEST)
            {
                scanParameters = &helperPort->ScanContext.ScanParameters[i];
                helperPort->ScanContext.ParametersCount++;

                MP_INCREMENT_SCAN_PARAMETER_REF(scanParameters);
                
                // Save the passed in information
                scanParameters->RequestingPort = RequestingPort;
                scanParameters->PortScanRequest = ScanRequest;
                scanParameters->CompletionHandler = CompletionHandler;

                // Scan would start at the first channel of the first phy
                scanParameters->NextChannelIndex = 0;
                scanParameters->CurrentPhyIndex = 0;

                // Queue it for processing
                scanParameters->CancelScan = 0;
                scanParameters->State = SCAN_QUEUED_FOR_PROCESSING;
                
                ndisStatus = NDIS_STATUS_SUCCESS;
                break;
            }
        }
        MP_RELEASE_PORT_LOCK(Port, FALSE);

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_SCAN, DBG_SERIOUS, ("Unable to save scan parameters\n"));    
            break;
        }

        // Let the scan request proceed
        HelperPortProcessPendingScans(helperPort);
    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // Free stuff that we may have allocated
        if (scanParameters != NULL)
        {
            // This cannot be left in our list
            HelperPortScanParametersReleaseRef(helperPort, scanParameters);
        }
    }
    
    return ndisStatus;
}


VOID
HelperPortCancelScan(
    _In_  PMP_PORT                Port,
    _In_  PMP_PORT                RequestingPort
    )
{
    ULONG                       i;
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    PMP_SCAN_PARAMETERS         scanParameters = NULL;
    MP_SCAN_STATE               preCancelState = SCAN_EMPTY_REQUEST;
    NDIS_STATUS                 ndisStatus;
    BOOLEAN                     timerCancelled = FALSE;

    // Add this scan request to the list
    MP_ACQUIRE_PORT_LOCK(Port, FALSE);

    // Search for the scan request in our list
    for (i = 0; i < MP_MAX_NUMBER_OF_PORT; i++)
    {
        if ((helperPort->ScanContext.ScanParameters[i].State != SCAN_EMPTY_REQUEST) && 
            (helperPort->ScanContext.ScanParameters[i].State != SCAN_COMPLETED) && 
            (helperPort->ScanContext.ScanParameters[i].RequestingPort == RequestingPort))
        {
            // The scan request from this port is in the queue
            scanParameters = &helperPort->ScanContext.ScanParameters[i];
            scanParameters->CancelScan = TRUE;

            // Add a refcount to ensure that the structure does not get deleted on us
            MP_INCREMENT_SCAN_PARAMETER_REF(scanParameters);

            preCancelState = scanParameters->State;

            // Save the previous state (for tracking)
            scanParameters->TrackingPreCancelState = preCancelState;
            
            if (preCancelState == SCAN_QUEUED_FOR_PROCESSING)
            {
                // This request is not yet activated for processing.
                // Remove the request from the pending scan list. This is done right now
                // with the lock held so that the request does not get requeued
                scanParameters->State = SCAN_REQUEST_IN_USE;
                MpTrace(COMP_SCAN, DBG_NORMAL, ("Canceling unprocessed scan request\n"));
            }
            else if (preCancelState == SCAN_EXCLUSIVE_ACCESS_QUEUED)
            {
                // We are unsure if the exclusive access request would
                // be granted or not. It would be granted if the cancel was part of
                // a pause or something. It would not be granted if this was
                // a halt
                MpTrace(COMP_SCAN, DBG_NORMAL, ("Canceling scan request waiting for exclusive access\n"));
            }

            break;
        }
    }

    MP_RELEASE_PORT_LOCK(Port, FALSE);

    if (scanParameters == NULL)
    {
        // No scan to cancel.
        return;
    }

    if (preCancelState != SCAN_QUEUED_FOR_PROCESSING)
    {
        // NOTE: Since we added the ref, we know that the ScanParameters buffer is available

        // If we have the timer running, force fire the timer
        timerCancelled = NdisCancelTimerObject(helperPort->ScanContext.Timer_Scan);
        if (timerCancelled == TRUE)
        {
            // We cancelled the timer, so we would need to invoke the complete ourselves
            MpTrace(COMP_SCAN, DBG_NORMAL, ("Canceling scan request waiting for scan timer\n"));
            HelperPortScanTimerCallback(helperPort);
        }
        else
        {
            // We could be waiting for exclusive access
            if (preCancelState == SCAN_EXCLUSIVE_ACCESS_QUEUED)
            {
                // We would complete the scan here. Because of the cancel flag, the
                // exclusive access routine would not proceed with the scan 
                // if it got called
                ndisStatus = NDIS_STATUS_REQUEST_ABORTED;

                MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(helperPort), FALSE);
                
                // Clear the active pointer
                helperPort->ScanContext.ActiveScanParameters = NULL;
                scanParameters->State = SCAN_COMPLETED;

                MP_RELEASE_PORT_LOCK(HELPPORT_GET_MP_PORT(helperPort), FALSE);

                // Now perform the indication
                HelperPortIndicateScanCompletion(helperPort, scanParameters, &ndisStatus);

                // We dont remove the reference here. We wait for the ex or the scan
                // to do it
            }

            // Ask the HW to cancel the scan (if it has it)
            VNic11CancelScan(HELPPORT_GET_VNIC(helperPort));
        }

        // Now wait for the scan complete to get indicated
        while (scanParameters->State != SCAN_COMPLETED)
        {
            MpTrace(COMP_SCAN, DBG_NORMAL, ("Waiting for scan operation to complete\n"));
            NdisMSleep(20 * 1000);
        }

        // This lock is acquired to ensure that the free here does not conflict with an
        // in progress scan completion
        MP_ACQUIRE_PORT_LOCK(Port, FALSE);
        HelperPortScanParametersReleaseRef(helperPort, scanParameters);
        MP_RELEASE_PORT_LOCK(Port, FALSE);

        // If there is a different second scan pending, process it
        HelperPortProcessPendingScans(helperPort);
    }
    else
    {
        // This scan was never started, we need to complete the scan request & we are done
        ndisStatus = NDIS_STATUS_REQUEST_ABORTED;
        HelperPortIndicateScanCompletion(helperPort, scanParameters, &ndisStatus);

        MP_ACQUIRE_PORT_LOCK(Port, FALSE);        
        scanParameters->State = SCAN_COMPLETED;        
        HelperPortScanParametersReleaseRef(helperPort, scanParameters);
        MP_RELEASE_PORT_LOCK(Port, FALSE);        
    }

}

VOID
HelperPortProcessPendingScans(
    _In_  PMP_HELPER_PORT         HelperPort
    )
{
    ULONG                       i;
    PMP_SCAN_PARAMETERS         scanParameters = NULL;
    BOOLEAN                     processScan = FALSE;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    
    // Add this scan request to the list
    MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

    if (HelperPort->ScanContext.ActiveScanParameters != NULL)
    {
        // There is already a scan in progress. We dont do anything here
    }
    else
    {
        // Search for a scan request in our list
        for (i = 0; i < MP_MAX_NUMBER_OF_PORT; i++)
        {
            if (HelperPort->ScanContext.ScanParameters[i].State == SCAN_QUEUED_FOR_PROCESSING)
            {
                // Found a scan request, we will queue it up
                scanParameters = &HelperPort->ScanContext.ScanParameters[i];
                break;
            }
        }
    }

    // If there is an active scan, check if we should perform the scan
    if (scanParameters != NULL)
    {
        // Check if we should perform this scan
        processScan = HelperPortShouldPerformScan(HelperPort, scanParameters);
        if (processScan)
        {
            // Set this as the active scan
            HelperPort->ScanContext.ActiveScanParameters = scanParameters;
            scanParameters->State = SCAN_STARTED;            
        }
        else
        {
            // We will successfully complete this scan without processing
            scanParameters->State = SCAN_REQUEST_IN_USE;
        }
    }

    MP_RELEASE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

    if (scanParameters == NULL)
    {
        // Nothing to process, we are done
        return;
    }

    if (processScan)
    {
        // Start the scan
        HelperPortStartScanProcess(HelperPort, scanParameters);
    }
    else
    {
        // There is a scan request that wont do a scan for, complete it without
        // actually processing it
        ndisStatus = NDIS_STATUS_SUCCESS;
        MpTrace(COMP_SCAN, DBG_NORMAL, ("Vetoed a scan request\n"));
        
        // Now perform the indication
        HelperPortIndicateScanCompletion(HelperPort, scanParameters, &ndisStatus);

        // We reacquire the lock to ensure that the cancel does not conflict
        // with this completion
        MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

        // The scan is done
        scanParameters->State = SCAN_COMPLETED;
        HelperPortScanParametersReleaseRef(HelperPort, scanParameters);
        MP_RELEASE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);
    }
}        

VOID
HelperPortIndicateScanCompletion(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters,
    _In_  PNDIS_STATUS            CompletionStatus
    )
{
    UNREFERENCED_PARAMETER(HelperPort);
    
    // Call the completion handler
    ScanParameters->CompletionHandler(ScanParameters->RequestingPort, CompletionStatus);
}

BOOLEAN
HelperPortShouldPerformScan(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters
    )
{
    ULONGLONG                   currentSystemTime;
    ULONGLONG                   scanGapSeconds = HelperPort->RegInfo->InterScanTime;
    ULONGLONG                   acceptableScanTime;

    if (MP_TEST_PORT_STATUS(HELPPORT_GET_MP_PORT(HelperPort), MP_PORT_CANNOT_SCAN_MASK))
    {
        // Cannot scan when in this state
        return FALSE;
    }
    
    if ((ScanParameters->CancelScan) || 
        (ScanParameters->State != SCAN_QUEUED_FOR_PROCESSING))
    {
        // Scan cancelled
        return FALSE;
    }

    if (MP_TEST_FLAG(ScanParameters->PortScanRequest->Dot11ScanRequest->dot11ScanType, dot11_scan_type_forced))
    {
        // Forced scan. We will do it
        return TRUE;
    }

    if (MP_TEST_FLAG(ScanParameters->PortScanRequest->ScanRequestFlags, MP_SCAN_REQUEST_OS_ISSUED))
    {
        // OS issued scan, we will do it
        return TRUE;
    }

    // Potentially, we should check if this scan request has a non-zero SSID

    //
    // To avoid too many scans, lets check if we have scanned recently.
    //
    NdisGetCurrentSystemTime((PLARGE_INTEGER)&currentSystemTime);
    acceptableScanTime = HelperPort->ScanContext.LastScanTime +             
         scanGapSeconds * 10000000; // Convert seconds to 100nS

    if (acceptableScanTime > currentSystemTime)
    {
        //
        // Scanned recently. Dont scan again
        //
        return FALSE;
    }
    else
    {
        return TRUE;
    }
}


    
NDIS_STATUS
HelperPortScanExAccessCallback(
    _In_  PMP_PORT                Port, 
    _In_  PVOID                   Ctx
    )
{
    PMP_SCAN_PARAMETERS         scanParameters = (PMP_SCAN_PARAMETERS)Ctx;
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    BOOLEAN                     performScan = TRUE;
    
    // The cancellation here is handled specially. Since we cannot abort
    // a request for exclusive access, its possible that our exclusive
    // access request gets honored after we have cancelled a scan

    HelperPortExclusiveAccessGranted(helperPort);
    
    MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(helperPort), FALSE);
    if ((scanParameters->State != SCAN_EXCLUSIVE_ACCESS_QUEUED) ||
        (scanParameters->CancelScan))
    {
        // The request has been cancelled and would get completed
        MpTrace(COMP_SCAN, DBG_NORMAL, ("Ignored exclusive access for cancelled/old scan request %p\n", scanParameters));
        performScan = FALSE;
    }
    else
    {
        // Move forward with the scan
        scanParameters->State = SCAN_EXCLUSIVE_ACCESS_ACQUIRED;
    }
    MP_RELEASE_PORT_LOCK(HELPPORT_GET_MP_PORT(helperPort), FALSE);

    if (performScan)
    {
        HelperPortStartPartialScan(helperPort, scanParameters);
    }
    else
    {
        // Release exclusive access back to the HW
        HelperPortReleaseExclusiveAccess(helperPort);

        // Free the scan request
        HelperPortScanParametersReleaseRef(helperPort, scanParameters);
    }
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS 
HelperPortScanCompleteCallback(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   Data
    )
{
    PMP_HELPER_PORT             helperPort = MP_GET_HELPPORT(Port);
    NDIS_STATUS                 completionStatus = *((PNDIS_STATUS)Data);
    PMP_SCAN_PARAMETERS         scanParameters 
                                    = helperPort->ScanContext.ActiveScanParameters;
     // Complete the partial scan
    HelperPortCompletePartialScan(helperPort, scanParameters, &completionStatus);

    return NDIS_STATUS_SUCCESS;
}

VOID
HelperPortScanTimerCallback(
    _In_  PMP_HELPER_PORT         HelperPort
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_SCAN_PARAMETERS         scanParameters;
    BOOLEAN                     queueExAccess = TRUE;
    
    MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);
    MPASSERT(HelperPort->ScanContext.ActiveScanParameters != NULL);
    scanParameters = HelperPort->ScanContext.ActiveScanParameters;
    
    if (scanParameters->CancelScan)
    {
        // Scan is being cancelled, dont need to queue exclusive access
        MpTrace(COMP_SCAN, DBG_NORMAL, ("Ignored scan timer for cancelled/old scan request %p\n", scanParameters));
        queueExAccess = FALSE;
    }
    else
    {
        scanParameters->State = SCAN_EXCLUSIVE_ACCESS_QUEUED;
    }
    MP_RELEASE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

    if (queueExAccess)
    {
        //
        // Queue an exclusive access. The scan would be done in there
        //
        ndisStatus = HelperPortRequestExclusiveAccess(HelperPort,
            HelperPortScanExAccessCallback, 
            HelperPort->ScanContext.ActiveScanParameters,
            FALSE
            );

        if ((ndisStatus != NDIS_STATUS_SUCCESS) && (ndisStatus != NDIS_STATUS_PENDING))
        {
            // Exclusive access request was rejected
            MpTrace(COMP_SCAN, DBG_SERIOUS, ("Exclusive access request in scan timer failed. Status = 0x%08x\n", ndisStatus));
            
            HelperPortCompleteScanProcess(
                HelperPort,
                scanParameters,
                &ndisStatus
            );
            
        }
        else if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            // the function completed synchronously, call the callback ourselves
            HelperPortScanExAccessCallback(HELPPORT_GET_MP_PORT(HelperPort), HelperPort->ScanContext.ActiveScanParameters);
        }
    }
    else
    {
        // Abort the scan
        ndisStatus = NDIS_STATUS_REQUEST_ABORTED;
        HelperPortCompleteScanProcess(
            HelperPort,
            scanParameters,
            &ndisStatus
        );
    }
}

VOID
HelperPortScanTimer(
    PVOID                   SystemSpecific1,
    PVOID                   FunctionContext,
    PVOID                   SystemSpecific2,
    PVOID                   SystemSpecific3
    )
{
    PMP_HELPER_PORT             helperPort = (PMP_HELPER_PORT)FunctionContext;
    
    UNREFERENCED_PARAMETER(SystemSpecific1);
    UNREFERENCED_PARAMETER(SystemSpecific2);
    UNREFERENCED_PARAMETER(SystemSpecific3);

    HelperPortScanTimerCallback(helperPort);
}

VOID
HelperPortStartScanProcess(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       scanChannelCount = 0;
    ULONG                       i;
    
    do
    {
        MpTrace(COMP_SCAN, DBG_NORMAL, ("Starting the scan process of %p for port %p\n", 
            ScanParameters, ScanParameters->RequestingPort));

        //
        // For optimal scanning, we specify the list of channels
        // that the HW should use if the port hasnt specified any.
        // Note that for ExtSTA the channels in the PhyTypeInfo 
        // structures of the DOT11_SCAN_REQUEST are invalid. So we dont
        // need to consider those
        //
        if (ScanParameters->PortScanRequest->ChannelCount != 0)
        {
            // Use the list of channels specified by the port
            scanChannelCount = ScanParameters->PortScanRequest->ChannelCount;
        }
        else
        {        
            if (HelperPort->ScanContext.MediaConnectedCount > 0)
            {
                // A port is connected (STA associated/AP started/Adhoc running)
                // We dont scan all channels in one shot. We do multiple partial
                // scans

                // The scan type determines the number of channels we scan at a time
                if (ScanParameters->PortScanRequest->Dot11ScanRequest->dot11ScanType & dot11_scan_type_active)
                    scanChannelCount = HelperPort->RegInfo->ActiveScanChannelCount;
                else
                    scanChannelCount = HelperPort->RegInfo->PassiveScanChannelCount;
                    
                MpTrace(COMP_SCAN, DBG_NORMAL, ("Link Up scan will scan atmost %d channels at a time\n",
                    scanChannelCount));
            }
            else
            {
                // We can scan each phy in one scan. Find the maximum number of
                // channels in a phy & use that as our scan channels limit
                scanChannelCount = 0;
                for (i = 0; i < HW11_MAX_PHY_COUNT; i++)
                {
                    if (HelperPort->ScanContext.ScanChannels[i].ChannelCount > scanChannelCount)
                    {
                        scanChannelCount = HelperPort->ScanContext.ScanChannels[i].ChannelCount;
                    }
                }            
                MpTrace(COMP_SCAN, DBG_NORMAL, ("Link Down scan will scan upto %d channels at a time\n",
                    scanChannelCount));
            }
        }

        ScanParameters->MaxChannelCount = scanChannelCount;
        
        //
        // Create a channel buffer that we would give to the lower layer
        //
        MP_ALLOCATE_MEMORY(
            HELPPORT_GET_MP_PORT(HelperPort)->MiniportAdapterHandle,
            &ScanParameters->VNicScanRequest.ChannelList,
            scanChannelCount * sizeof(ULONG),
            PORT_MEMORY_TAG
            );
        
        if (ScanParameters->VNicScanRequest.ChannelList == NULL)
        {
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        
        //
        // Save scan start time
        //
        NdisGetCurrentSystemTime((PLARGE_INTEGER)&HelperPort->ScanContext.LastScanTime);        

        ScanParameters->VNicScanRequest.Dot11ScanRequest = ScanParameters->PortScanRequest->Dot11ScanRequest;
        ScanParameters->VNicScanRequest.ScanRequestBufferLength = 
            ScanParameters->PortScanRequest->ScanRequestBufferLength;

        MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);
        if (ScanParameters->CancelScan)
        {
            MpTrace(COMP_SCAN, DBG_NORMAL, ("Aborting scan start for a cancelled scan request\n"));        
            ndisStatus = NDIS_STATUS_REQUEST_ABORTED;
        }
        else
        {
            // We will queue an exclusive access request
            ScanParameters->State = SCAN_EXCLUSIVE_ACCESS_QUEUED;
        }
        MP_RELEASE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        //
        // Queue an exclusive access. The scan would be done in there
        //
        ndisStatus = HelperPortRequestExclusiveAccess(HelperPort, 
            HelperPortScanExAccessCallback, 
            ScanParameters,
            FALSE
            );

        if (NDIS_STATUS_SUCCESS == ndisStatus)
        {
            // the function completed synchronously, call the callback ourselves
            HelperPortScanExAccessCallback(HELPPORT_GET_MP_PORT(HelperPort), ScanParameters);
        }
        else if (ndisStatus == NDIS_STATUS_PENDING)
        {
            // Pending is same as success
            if (ndisStatus == NDIS_STATUS_PENDING)
            {
                ndisStatus = NDIS_STATUS_SUCCESS;
            }
        }
        else
        {
            // The exclusive access request failed
            MpTrace(COMP_SCAN, DBG_SERIOUS, ("Exclusive access request for scan start failed. Status = 0x%08x\n", ndisStatus)); 
        }
    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        HelperPortCompleteScanProcess(
            HelperPort,
            ScanParameters,
            &ndisStatus
        );
    }

}

VOID
HelperPortCompleteScanProcess(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters,
    _In_  PNDIS_STATUS            CompletionStatus
    )
{
    MpTrace(COMP_SCAN, DBG_NORMAL, ("Completed the scan process of %p for port %p\n", 
        ScanParameters, ScanParameters->RequestingPort));

    MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);
    
    // Clear the active pointer
    MPASSERT(ScanParameters == HelperPort->ScanContext.ActiveScanParameters);
    HelperPort->ScanContext.ActiveScanParameters = NULL;
    ScanParameters->State = SCAN_REQUEST_IN_USE;

    MP_RELEASE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

    // Now perform the indication
    HelperPortIndicateScanCompletion(HelperPort, ScanParameters, CompletionStatus);

    // We reacquire the lock to ensure that the cancel does not conflict
    // with this completion
    MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);
    
    // The scan is done
    ScanParameters->State = SCAN_COMPLETED;
    HelperPortScanParametersReleaseRef(HelperPort, ScanParameters);
    
    MP_RELEASE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

    // If another scan is queued, run it now
    HelperPortProcessPendingScans(HelperPort);
}

// Scans a set of channels. Called with exclusive access held. Also only called when we have a channel
// to scan.
VOID
HelperPortStartPartialScan(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       i, currentChannelCount;
    PMP_SCAN_CHANNEL_LIST       currentChannelList;

    do
    {
        MpTrace(COMP_SCAN, DBG_LOUD, ("Starting partial scan of %p\n", ScanParameters));
    
        MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);
        // Check if the request should be cancelled
        if (ScanParameters->CancelScan)
        {
            MpTrace(COMP_SCAN, DBG_NORMAL, ("Aborting partial scan for cancelled/old scan request %p\n", ScanParameters));
            ndisStatus = NDIS_STATUS_REQUEST_ABORTED;
        }
        else
        {
            // We are going to send this on the hardware
            ScanParameters->State = SCAN_HARDWARE_SCANNING;
        }
        MP_RELEASE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        // Determine the channels to scan. If specified by the port, use those, else
        // use our
        if (ScanParameters->PortScanRequest->ChannelCount != 0)
        {
            currentChannelCount = ScanParameters->PortScanRequest->ChannelCount;
            
            //
            // Populate the channels for this scan with what the port requested
            //
            ScanParameters->VNicScanRequest.ChannelCount = currentChannelCount;
            for (i = 0; i < currentChannelCount; i++)
            {
                ScanParameters->VNicScanRequest.ChannelList[i] = ScanParameters->PortScanRequest->ChannelList[i];
            }
            ScanParameters->VNicScanRequest.PhyId = ScanParameters->PortScanRequest->PhyId;
        }
        else
        {
            currentChannelList = &HelperPort->ScanContext.ScanChannels[ScanParameters->CurrentPhyIndex];
            // We must have atleast one channel in the current phy index that we can scan
            MPASSERT(currentChannelList->ChannelCount > ScanParameters->NextChannelIndex);

            // Determine the number of channels to use for this scan
            currentChannelCount = ScanParameters->MaxChannelCount;

            if ((ScanParameters->NextChannelIndex + currentChannelCount) > 
                    currentChannelList->ChannelCount)
            {
                // We have fewer remaining channels that our MaxChannelCount, adjust the channel count
                currentChannelCount = currentChannelList->ChannelCount 
                                            - ScanParameters->NextChannelIndex;
            }
            
            //
            // Populate the channels for this scan
            //
            ScanParameters->VNicScanRequest.ChannelCount = currentChannelCount;
            for (i = 0; i < currentChannelCount; i++)
            {
                ScanParameters->VNicScanRequest.ChannelList[i] = currentChannelList->ChannelList[ScanParameters->NextChannelIndex + i];
            }
            ScanParameters->VNicScanRequest.PhyId = currentChannelList->PhyId;

            // Next time we start scan at the next channel
            ScanParameters->NextChannelIndex = ScanParameters->NextChannelIndex + currentChannelCount;
        }

        ndisStatus = VNic11StartScan(HELPPORT_GET_VNIC(HelperPort), 
                        &ScanParameters->VNicScanRequest,
                        HelperPortScanCompleteCallback
                        );
        
        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            // the function completed synchronously - call the callback ourselves
            HelperPortScanCompleteCallback(HELPPORT_GET_MP_PORT(HelperPort), &ndisStatus);
        }
        else if (NDIS_STATUS_PENDING != ndisStatus)
        {
            MpTrace(COMP_SCAN, DBG_SERIOUS, ("VNic11StartScan failed for channel index %d\n",
                        ScanParameters->NextChannelIndex - currentChannelCount));
            break;
        }
    }
    while (FALSE);   

    if (ndisStatus != NDIS_STATUS_PENDING && ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // We call the complete function
        HelperPortCompletePartialScan(HelperPort, ScanParameters, &ndisStatus);
    }
}

// Called when scan on a set of channels is completed
VOID
HelperPortCompletePartialScan(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters,
    _In_  PNDIS_STATUS            CompletionStatus
    )
{
    LARGE_INTEGER               rescheduleTime;
    BOOLEAN                     requeueScan = TRUE;
    ULONG                       i;

    // release exclusive access
    HelperPortReleaseExclusiveAccess(HelperPort);

    // Determine the next step for scan
    MP_ACQUIRE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

    if (ScanParameters->PortScanRequest->ChannelCount != 0)
    {
        // All port specified channels are scanned in one shot
        requeueScan = FALSE;
    }
    else
    {
        // Determine if we are done with all the channels on current PHY
        if (ScanParameters->NextChannelIndex >= 
                HelperPort->ScanContext.ScanChannels[ScanParameters->CurrentPhyIndex].ChannelCount)
        {
            // We are done with all phys on current channel, check if there is another
            // phy we need to scan

            requeueScan = FALSE;    // Start with a negative
            for (i = ScanParameters->CurrentPhyIndex + 1; i < HW11_MAX_PHY_COUNT; i++)
            {
                if (HelperPort->ScanContext.ScanChannels[i].ChannelCount > 0)
                {
                    // Found a phy to scan we will requeue the scan.
                    requeueScan = TRUE;

                    // Start from first channel on this new phy
                    ScanParameters->CurrentPhyIndex = i;
                    ScanParameters->NextChannelIndex = 0;
                }
            }
        }
    }
    
    if (ScanParameters->CancelScan)
    {
        // Cancelled
        requeueScan = FALSE;
    }
    
    if (requeueScan == FALSE)
    {
        // We will queuing the scan timer
        ScanParameters->State = SCAN_WAITING_FOR_TIMER;
    }

    if ((*CompletionStatus) != NDIS_STATUS_SUCCESS)
    {
        // Partial scan failed
        MpTrace(COMP_SCAN, DBG_NORMAL, ("Partial scan failed. Status = 0x%08x\n", (*CompletionStatus)));
        
        requeueScan = FALSE;
    }
    
    MP_RELEASE_PORT_LOCK(HELPPORT_GET_MP_PORT(HelperPort), FALSE);

    MpTrace(COMP_SCAN, DBG_LOUD, ("Completed partial scan of %p\n", ScanParameters));

    if (requeueScan)
    {
        if (HelperPort->ScanContext.MediaConnectedCount > 0)
        {    
            // Queue the timer for another scan. We try to schedule this after one
            // context switch interval
            rescheduleTime.QuadPart = Int32x32To64(HelperPort->RegInfo->ScanRescheduleTime, -10000);
        }
        else
        {
            // Noboby is connected yet, Queue the timer for another scan
            rescheduleTime.QuadPart = Int32x32To64(MP_SCAN_RESCHEDULE_TIME_NOT_CONNECTED, -10000);
        }
        
        NdisSetTimerObject(HelperPort->ScanContext.Timer_Scan, rescheduleTime, 0, NULL);
    }
    else
    {
        // We have finished scanning all channels, call the scan completion
        HelperPortCompleteScanProcess(
            HelperPort,
            ScanParameters,
            CompletionStatus
        );
    }
}

