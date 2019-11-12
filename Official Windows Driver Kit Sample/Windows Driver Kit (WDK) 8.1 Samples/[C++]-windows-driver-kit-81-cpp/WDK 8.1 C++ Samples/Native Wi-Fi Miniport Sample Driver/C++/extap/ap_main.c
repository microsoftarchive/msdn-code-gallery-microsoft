/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_main.c

Abstract:
    Implements the PNP handling for the ExtAP layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-21-2007    Created

Notes:

--*/

#include "precomp.h"
#include "ap_oids.h"

#if DOT11_TRACE_ENABLED
#include "ap_main.tmh"
#endif



NDIS_STATUS
Ap11LoadRegistryInformation(
    _In_                NDIS_HANDLE       MiniportAdapterHandle,
    _In_opt_            NDIS_HANDLE       ConfigurationHandle,
    _Outptr_result_maybenull_ PVOID*      RegistryInformation
    )
{
    UNREFERENCED_PARAMETER(MiniportAdapterHandle);
    UNREFERENCED_PARAMETER(ConfigurationHandle);

    *RegistryInformation = NULL;
    
    return NDIS_STATUS_SUCCESS;
}

VOID
Ap11FreeRegistryInformation(
    _In_  PVOID                 RegistryInformation
    )
{
    UNREFERENCED_PARAMETER(RegistryInformation);
}

NDIS_STATUS
Ap11AllocatePort(
    _In_ NDIS_HANDLE MiniportAdapterHandle,
    _In_ PADAPTER Adapter,
    _In_ PMP_PORT Port
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_EXTAP_PORT newExtApPort = NULL;

    UNREFERENCED_PARAMETER(Adapter);

    do
    {
        // Allocate the AP port specific structure
        MP_ALLOCATE_MEMORY(
            MiniportAdapterHandle, 
            &newExtApPort, 
            sizeof(MP_EXTAP_PORT), 
            EXTAP_MEMORY_TAG
            );
        
        if (NULL == newExtApPort)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Port(%u): Failed to allocate %d bytes for extap port.\n",
                                    Port->PortNumber,
                                    sizeof(MP_EXTAP_PORT)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // Clear everything
        NdisZeroMemory(newExtApPort, sizeof(MP_EXTAP_PORT));

        // set the state
        newExtApPort->State = AP_STATE_INVALID;

        // Save pointer to the parent port & vice versa
        Port->ChildPort = newExtApPort;
        newExtApPort->ParentPort = Port;

        // Set op mode and op state value
        // The initial op mode is extsta
        Port->CurrentOpMode = DOT11_OPERATION_MODE_EXTENSIBLE_AP;
        Port->CurrentOpState = INIT_STATE;
    } while (FALSE);
    
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (newExtApPort != NULL)
        {
            Ap11FreePort(Port);
        }
    }

    return ndisStatus;
}


VOID
Ap11FreePort(
    _In_ PMP_PORT Port
    )
{
    PMP_EXTAP_PORT extApPort = MP_GET_AP_PORT(Port);

    // AP shall be stopped 
    MPASSERT(AP_STATE_INVALID == extApPort->State || AP_STATE_STOPPED == extApPort->State);
    
    // Free ExtAP port
    MP_FREE_MEMORY(extApPort);

}

NDIS_STATUS
Ap11InitializePort(
    _In_ PMP_PORT Port,
    _In_ PVOID RegistryInformation
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_EXTAP_PORT extApPort = MP_GET_AP_PORT(Port);
    // remember the initialized components so that we can rollback when error occurs
    BOOLEAN deinitHWCapability = FALSE;
    BOOLEAN deinitRegInfo = FALSE;
    BOOLEAN deinitConfig = FALSE;
    BOOLEAN deinitAssocMgr = FALSE;

    MPASSERT(AP_STATE_INVALID == extApPort->State);
    UNREFERENCED_PARAMETER(RegistryInformation);
    
    do
    {
        // get hardware capability
        ndisStatus = ApInitializeCapability(extApPort);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Port(%u): Failed to get hardware capability. \n",
                                        Port->PortNumber));
            break;
        }
        deinitHWCapability = TRUE;
        
        // Read registry settings
        // This call will not fail
        ApInitializeRegInfo(extApPort);

        
        deinitRegInfo = TRUE;
        
        // initialize configuration
        ndisStatus = ApInitializeConfig(
            AP_GET_CONFIG(extApPort),
            extApPort
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Port(%u): Failed to initialize AP configurations. \n",
                                        Port->PortNumber));
            break;
        }
        deinitConfig = TRUE;

        // initialize association manager
        ndisStatus = ApInitializeAssocMgr(
            AP_GET_ASSOC_MGR(extApPort),
            extApPort
            );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Port(%u): Failed to initialize association manager. \n",
                                        Port->PortNumber));
            break;
        }
        deinitAssocMgr = TRUE;

        // Setup the handler functions
        Port->RequestHandler = Ap11OidHandler;
        Port->DirectRequestHandler = Ap11DirectOidHandler;
        Port->SendEventHandler = Ap11SendEventHandler;
        Port->SendCompleteEventHandler = Ap11SendCompleteEventHandler;
        Port->ReceiveEventHandler = Ap11ReceiveHandler;
        Port->ReturnEventHandler = BasePortReturnEventHandler;

        // initial reference count
        extApPort->RefCount = 1;
        
        // AP is not started yet
        extApPort->State = AP_STATE_STOPPED;
    } while (FALSE);
    
    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // Clean up
        if (deinitAssocMgr)
        {
            ApDeinitializeAssocMgr(AP_GET_ASSOC_MGR(extApPort));
        }

        if (deinitConfig)
        {
            ApDeinitializeConfig(AP_GET_CONFIG(extApPort));
        }

        if (deinitRegInfo)
        {
            ApDeinitializeRegInfo(AP_GET_REG_INFO(extApPort));
        }
        
        if (deinitHWCapability)
        {
            ApDeinitializeCapability(AP_GET_CAPABILITY(extApPort));
        }
    }

    return ndisStatus;
}

VOID
Ap11TerminatePort(
    _In_ PMP_PORT Port
    )
{
    PMP_EXTAP_PORT extApPort = MP_GET_AP_PORT(Port);

    // AP shall be stopped already
    if (extApPort->State != AP_STATE_STOPPED)
    {
        StopExtAp(extApPort);
    }
    
    MPASSERT(AP_STATE_STOPPED == extApPort->State);

    // deinitialize all components
    ApDeinitializeAssocMgr(AP_GET_ASSOC_MGR(extApPort));
   
    ApDeinitializeConfig(AP_GET_CONFIG(extApPort));

    ApDeinitializeRegInfo(AP_GET_REG_INFO(extApPort));
    
    ApDeinitializeCapability(AP_GET_CAPABILITY(extApPort));
    
    extApPort->State = AP_STATE_INVALID;
    
}

/**
 * After Pause returns, the upper layer will guarantee 
 * the packet receive and send handlers will not be invoked.
 * There might be OID calls if the port will not be terminated.
 * The port needs to guarantee it will not call vnic to send
 * packets.
 */
NDIS_STATUS
Ap11PausePort(
    _In_ PMP_PORT Port
    )
{
    // We shall still allows control to go through
    // base port will block all traffic
    // TODO: anything else ExtAP needs to do?

    PauseExtAp(MP_GET_AP_PORT(Port));
    
    return BasePortPausePort(Port);
}

NDIS_STATUS
Ap11RestartPort(
    _In_ PMP_PORT Port
    )
{
    // TODO: anything else ExtAP needs to do?
    
    RestartExtAp(MP_GET_AP_PORT(Port));
    
    return BasePortRestartPort(Port);
}

NDIS_STATUS
Ap11NdisResetStart(
    _In_ PMP_PORT Port
    )
{
    // TODO: Implement this
    UNREFERENCED_PARAMETER(Port);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
Ap11NdisResetEnd(
    _In_ PMP_PORT Port
    )
{
    // TODO: Implement this
    UNREFERENCED_PARAMETER(Port);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
Ap11SetPower(
    _In_  PMP_PORT              Port,
    _In_  NDIS_DEVICE_POWER_STATE NewDevicePowerState
    )
{
    NDIS_STATUS ndisStatus = NDIS_STATUS_SUCCESS;
    PMP_EXTAP_PORT extApPort = MP_GET_AP_PORT(Port);

    if (NdisDeviceStateD0 != NewDevicePowerState)
    {
        //
        // We are going to sleep, inform the VNic
        //
        if (extApPort->State == AP_STATE_STARTED)
        {
            ndisStatus = StopExtApBss(extApPort);
        }

    }

    return ndisStatus;
}


VOID
Ap11IndicateStatus(
    _In_  PMP_PORT              Port,
    _In_  NDIS_STATUS           StatusCode,
    _In_  PVOID                 StatusBuffer,
    _In_  ULONG                 StatusBufferSize
    )
{
    PMP_EXTAP_PORT extApPort = MP_GET_AP_PORT(Port);

    switch (StatusCode)
    {
        case NDIS_STATUS_DOT11_PHY_STATE_CHANGED:
        {
            PDOT11_PHY_STATE_PARAMETERS phyStateParameters = (PDOT11_PHY_STATE_PARAMETERS)StatusBuffer;
            DOT11_STOP_AP_PARAMETERS    stopApParameters;
            DOT11_CAN_SUSTAIN_AP_PARAMETERS sustainApParameters;
            
            // Handle the phy state change notification
            if ((phyStateParameters->bHardwarePhyState == FALSE) || (phyStateParameters->bSoftwarePhyState == FALSE))
            {
                // Check if this is the first radio off or was already turned off
                if (NdisInterlockedIncrement((LONG*)&extApPort->Config.CannotSustainApRef) == 1)
                {
                    // Radio has been turned off. Indicate a cannot sustain to the upper layer
                    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, 
                            ("Port(%u): Indicating stop AP notification", AP_GET_PORT_NUMBER(extApPort)));

                    // We stopped the AP, now indicate the cannot sustain notification to the upper layer
                    NdisZeroMemory(&stopApParameters, sizeof(DOT11_STOP_AP_PARAMETERS));
                    stopApParameters.Header.Revision = DOT11_STOP_AP_PARAMETERS_REVISION_1;
                    stopApParameters.Header.Type = NDIS_OBJECT_TYPE_DEFAULT;
                    stopApParameters.Header.Size = sizeof(DOT11_STOP_AP_PARAMETERS);

                    stopApParameters.ulReason = DOT11_STOP_AP_REASON_IHV_START;
                    
                    ApIndicateDot11Status(
                        MP_GET_AP_PORT(Port),
                        NDIS_STATUS_DOT11_STOP_AP,
                        NULL,
                        &stopApParameters,
                        sizeof(DOT11_STOP_AP_PARAMETERS)
                        );
                }
                else
                {
                    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, 
                            ("Port(%u): Extra radio OFF notification", AP_GET_PORT_NUMBER(extApPort)));
                }
            }
            else
            { 
                // Radio has been turned on. If we have finished radio ons for all the radio off's, then
                // we can go back to claiming that we can sustain the AP. In case the radio off was never done, so
                // the ref is non-relevant
                if (extApPort->Config.CannotSustainApRef > 0)
                {
                    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, 
                            ("Port(%u): Radio switched on", AP_GET_PORT_NUMBER(extApPort)));
                    if (NdisInterlockedDecrement((LONG*)&extApPort->Config.CannotSustainApRef) == 0)
                    {
                        MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, 
                                ("Port(%u): Indicating can sustain AP notification", AP_GET_PORT_NUMBER(extApPort)));
                        // All radio off's have been fixed. We can now do a can sustain notification
                        NdisZeroMemory(&sustainApParameters, sizeof(DOT11_CAN_SUSTAIN_AP_PARAMETERS));
                        sustainApParameters.Header.Revision = DOT11_CAN_SUSTAIN_AP_PARAMETERS_REVISION_1;
                        sustainApParameters.Header.Type = NDIS_OBJECT_TYPE_DEFAULT;
                        sustainApParameters.Header.Size = sizeof(DOT11_CAN_SUSTAIN_AP_PARAMETERS);
                        
                        sustainApParameters.ulReason = DOT11_CAN_SUSTAIN_AP_REASON_IHV_START;
                        
                        ApIndicateDot11Status(
                            MP_GET_AP_PORT(Port),
                            NDIS_STATUS_DOT11_CAN_SUSTAIN_AP,
                            NULL,
                            &sustainApParameters,
                            sizeof(DOT11_CAN_SUSTAIN_AP_PARAMETERS)
                            );
                    }
                }
                else
                {
                    MpTrace(COMP_ASSOC_MGR, DBG_NORMAL, 
                            ("Port(%u): Extra radio ON notification", AP_GET_PORT_NUMBER(extApPort)));
                }
            }          
        }

        default:
            break;
    }

    ApIndicateDot11Status(
        MP_GET_AP_PORT(Port),
        StatusCode,
        NULL,
        StatusBuffer,
        StatusBufferSize
        );
}

