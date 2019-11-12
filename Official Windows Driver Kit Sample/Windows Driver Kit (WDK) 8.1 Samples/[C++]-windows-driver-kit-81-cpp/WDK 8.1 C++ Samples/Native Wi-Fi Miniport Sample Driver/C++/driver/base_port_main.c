/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    base_port_main.c

Abstract:
    Implements some of the functionality for the base port class
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"
#include "port_defs.h"
#include "base_port.h"
#include "base_port_intf.h"
#include "vnic_intf.h"
#include "glb_utils.h"

#if DOT11_TRACE_ENABLED
#include "base_port_main.tmh"
#endif

_Success_(return == NDIS_STATUS_SUCCESS)
NDIS_STATUS
BasePortAllocatePort(
    _In_  PADAPTER                Adapter,
    _In_  MP_PORT_TYPE            PortType,
    _Outptr_result_nullonfailure_ PMP_PORT*     Port
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_PORT                    newPort = NULL;

    *Port = NULL;

    do
    {
        // Allocate a PORT data structure
        MP_ALLOCATE_MEMORY(Adapter->MiniportAdapterHandle, &newPort, sizeof(MP_PORT), PORT_MEMORY_TAG);
        if (newPort == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate %d bytes for port\n",
                                 sizeof(MP_PORT)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // Clear everything
        NdisZeroMemory(newPort, sizeof(MP_PORT));

        // Allocate memory for fields inside the PORT structure
        NdisAllocateSpinLock(&(newPort->Lock));
        NDIS_INIT_MUTEX(&(newPort->ResetPnpMutex));

        // The VNIC
        ndisStatus = VNic11Allocate(Adapter->MiniportAdapterHandle, &(newPort->VNic), newPort);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }

        // Save the passed data into the PORT
        newPort->MiniportAdapterHandle = Adapter->MiniportAdapterHandle;
        newPort->Adapter = Adapter;
        newPort->PortType = PortType;

        newPort->CurrentOpMode = DOT11_OPERATION_MODE_UNKNOWN;
        newPort->CurrentOpState = INIT_STATE;

        // We start in paused state
        MP_SET_PORT_STATUS(newPort, MP_PORT_PAUSED);
        newPort->PauseCount = 1;   

        // Setup the default handler functions
        newPort->RequestHandler = BasePortOidHandler;
        newPort->DirectRequestHandler = BasePortDirectOidHandler;
        newPort->SendEventHandler = BasePortSendEventHandler;
        newPort->SendCompleteEventHandler = BasePortSendCompleteEventHandler;
        newPort->ReceiveEventHandler = BasePortReceiveEventHandler;
        newPort->ReturnEventHandler = BasePortReturnEventHandler;

        newPort->AutoConfigEnabled = DOT11_PHY_AUTO_CONFIG_ENABLED_FLAG 
                                        | DOT11_MAC_AUTO_CONFIG_ENABLED_FLAG;

        // Return the newly created structure to the caller
        *Port = newPort;

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (newPort != NULL)
        {
            BasePortFreePort(newPort);
        }
    }

    return ndisStatus;

}

VOID
BasePortFreePort(
    _In_ __drv_freesMem(Mem) PMP_PORT                Port
    )
{
    if (Port->VNic != NULL)
    {
        VNic11Free(Port->VNic);
    }
    NdisFreeSpinLock(&(Port->Lock));
    
    MP_FREE_MEMORY(Port);

}

NDIS_STATUS
BasePortInitializePort(
    _In_  PMP_PORT                Port,
    _In_  PHVL                    Hvl
    )
{   
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    NET_BUFFER_LIST_POOL_PARAMETERS     nblPoolParameters;
    BOOLEAN lookasideAllocated = FALSE, packetQueueInitialized = FALSE;
    BOOLEAN vnicInitialized = FALSE;

    do
    {
        //
        // Allocate the Tx related data structures
        //
        MpInitPacketQueue(&Port->PendingTxQueue);
        packetQueueInitialized = TRUE;            

        // Lookaside list for MP_TX_MSDU  
        NdisInitializeNPagedLookasideList(
            &Port->TxPacketLookaside,
            NULL,
            NULL,
            0,
            sizeof(MP_TX_MSDU  ),
            PORT_MEMORY_TAG,
            0
            );

        // Lookaside list for MP_TX_MPDU    
        NdisInitializeNPagedLookasideList(
            &Port->TxFragmentLookaside,
            NULL,
            NULL,
            0,
            sizeof(MP_TX_MPDU    ),
            PORT_MEMORY_TAG,
            0
            );

        lookasideAllocated = TRUE;

        Port->SendToken = 1;
        
        NdisInitializeEvent(&Port->DeferredSendTrigger);
        
        //
        // Allocate the RX related data structures
        //

        //
        // Allocate the NBL Pool
        //
        NdisZeroMemory(&nblPoolParameters, sizeof(NET_BUFFER_LIST_POOL_PARAMETERS));
        nblPoolParameters.Header.Type = NDIS_OBJECT_TYPE_DEFAULT;
        nblPoolParameters.Header.Revision = NET_BUFFER_LIST_POOL_PARAMETERS_REVISION_1;
        nblPoolParameters.Header.Size = sizeof(NET_BUFFER_LIST_POOL_PARAMETERS);
        nblPoolParameters.fAllocateNetBuffer = TRUE;
        nblPoolParameters.ContextSize = 0;
        nblPoolParameters.PoolTag = PORT_MEMORY_TAG;
        nblPoolParameters.DataSize = 0;
        
        Port->RxNetBufferListPool = NdisAllocateNetBufferListPool(
            Port->MiniportAdapterHandle,
            &nblPoolParameters
            );
        if (Port->RxNetBufferListPool == NULL)
        {
            MpTrace(COMP_RECV, DBG_SERIOUS,  ("Failed to allocate NetBufferList Pool\n"));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        MP_CLEAR_PORT_STATUS(Port, MP_PORT_HALTING);

        //
        // Finally we initialize the VNIC
        //
        ndisStatus = VNic11Initialize(Port->VNic, Hvl, Port->Adapter->Hw, Port->PortType, Port->PortNumber);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to initialize VNic. Status = 0x%08x\n\n", ndisStatus));
            break;
        }
        vnicInitialized = TRUE;

        // Create the send worker thread. This would serialize the submission of sends to the H/W. 
        // Since the worker thread does not send any traffic we dont need to create the thread for it
        if (Port->PortType != HELPER_PORT)
        {
            ndisStatus = MpCreateThread(
                BasePortDeferredSendThread,
                Port,
                0,
                &Port->DeferredSendThread
                );
            if (ndisStatus != NDIS_STATUS_SUCCESS)
            {
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to create send processing worker thread. Status = 0x%08x\n\n", ndisStatus));
                break;
            }
    }        

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (vnicInitialized)
        {
            VNic11Terminate(Port->VNic);
        }
        
        if (Port->RxNetBufferListPool)
        {
            NdisFreeNetBufferListPool(Port->RxNetBufferListPool);
            Port->RxNetBufferListPool = NULL;
        }

        if (Port->DeferredSendThread != NULL)
        {
            MP_SET_PORT_STATUS(Port, MP_PORT_HALTING);

            // Wait for the send thread to exit
            NdisSetEvent(&Port->DeferredSendTrigger);
            
            KeWaitForSingleObject(Port->DeferredSendThread, 
                Executive, 
                KernelMode, 
                FALSE, 
                NULL
                );
        
            ObDereferenceObject(Port->DeferredSendThread);    
            Port->DeferredSendThread = NULL;
        }    

        if (lookasideAllocated)
        {
            NdisDeleteNPagedLookasideList(&Port->TxPacketLookaside);

            NdisDeleteNPagedLookasideList(&Port->TxFragmentLookaside);
        }

        if (packetQueueInitialized)
        {
            MpDeinitPacketQueue(&Port->PendingTxQueue);
        }

        
    }

    return ndisStatus;
}

VOID
BasePortTerminatePort(
    _In_  PMP_PORT                Port
    )
{
    NDIS_STATUS                 ndisStatus;
    
    MP_SET_PORT_STATUS(Port, MP_PORT_HALTING);
    if (Port->DeferredSendThread != NULL)
    {
        // Wait for the send thread to exit
        NdisSetEvent(&Port->DeferredSendTrigger);
        
        ndisStatus = KeWaitForSingleObject(Port->DeferredSendThread, 
                        Executive, 
                        KernelMode, 
                        FALSE, 
                        NULL
                        );

        MPASSERT(NT_SUCCESS(ndisStatus));
        ObDereferenceObject(Port->DeferredSendThread);    
        Port->DeferredSendThread = NULL;
    }    
    
    VNic11Terminate(Port->VNic);

    if (Port->RxNetBufferListPool)
    {
        NdisFreeNetBufferListPool(Port->RxNetBufferListPool);
        Port->RxNetBufferListPool = NULL;
    }

    // Delete the lookaside list
    NdisDeleteNPagedLookasideList(&Port->TxPacketLookaside);

    NdisDeleteNPagedLookasideList(&Port->TxFragmentLookaside);

    MpDeinitPacketQueue(&Port->PendingTxQueue);
}


NDIS_STATUS
BasePortPausePort(
    _In_  PMP_PORT                Port
    )
{
    NDIS_STATUS                 ndisStatus;

    // Flush pending Tx packets
    BasePortFlushQueuedTxPackets(Port);

    // When we pause a port, we pause the Mac context. This
    // automatically waits for packets submitted to the hardware
    // to complete
    ndisStatus = VNic11Pause(PORT_GET_VNIC(Port));
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
BasePortRestartPort(
    _In_  PMP_PORT                Port
    )
{
    NDIS_STATUS                 ndisStatus;

    // DO NOT TOUCH THE PAUSE flags OR COUNT or MUTEX, these are handled
    // elsewhere

    // When we restart a port, we restart the HW
    ndisStatus = VNic11Restart(PORT_GET_VNIC(Port));
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);
    
    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
BasePortResetPort(
    _In_  PMP_PORT                Port,
    _In_  PDOT11_RESET_REQUEST    Dot11ResetRequest
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(Dot11ResetRequest);
    
    // Reset my state
    Port->CurrentOpState = INIT_STATE;

    Port->AutoConfigEnabled = DOT11_PHY_AUTO_CONFIG_ENABLED_FLAG 
                                    | DOT11_MAC_AUTO_CONFIG_ENABLED_FLAG;

    // Flush all the TX packets that are in the queue
    BasePortFlushQueuedTxPackets(Port);
    return ndisStatus;
}

VOID
BasePortInvalidatePort(
    _In_  PMP_PORT                Port
    )
{
    // Set the special EMPTY_PORT type
    Port->PortType = EMPTY_PORT;
    Port->CurrentOpMode = DOT11_OPERATION_MODE_UNKNOWN;
    Port->CurrentOpState = INIT_STATE;
    
    // Setup the default handler functions for this port
    Port->RequestHandler = BasePortOidHandler;
    Port->DirectRequestHandler = BasePortDirectOidHandler;
    Port->SendEventHandler = BasePortSendEventHandler;
    Port->SendCompleteEventHandler = BasePortSendCompleteEventHandler;
    Port->ReceiveEventHandler = BasePortReceiveEventHandler;
    Port->ReturnEventHandler = BasePortReturnEventHandler;
}
