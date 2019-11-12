/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    helper_port_main.h

Abstract:
    Contains helper port internal defines
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

/** Used to hold state about miniport pause */
typedef struct _MP_MINIPORT_PAUSE_PARAMETERS
{
    /** The pause parameters provided by NDIS */
    PNDIS_MINIPORT_PAUSE_PARAMETERS NdisParameters;
    
    /** The event that is fired when the pause is completed */
    NDIS_EVENT              CompleteEvent;

}MP_MINIPORT_PAUSE_PARAMETERS, *PMP_MINIPORT_PAUSE_PARAMETERS;

/** Used to hold state about miniport restart */
typedef struct _MP_MINIPORT_RESTART_PARAMETERS
{
    /** The restart parameters provided by NDIS */
    PNDIS_MINIPORT_RESTART_PARAMETERS NdisParameters;
    
    /** The event that is fired when the restart is completed */
    NDIS_EVENT              CompleteEvent;

}MP_MINIPORT_RESTART_PARAMETERS, *PMP_MINIPORT_RESTART_PARAMETERS;


/** Used to hold state about ndis reset */
typedef struct _MP_NDIS_RESET_PARAMETERS
{
    PBOOLEAN                AddressingReset;

    NDIS_STATUS             ResetStatus;    

    /** The event that is fired when the reset is completed */
    NDIS_EVENT              CompleteEvent;
}MP_NDIS_RESET_PARAMETERS, *PMP_NDIS_RESET_PARAMETERS;


/** Used to hold state about power OID */
typedef struct _MP_POWER_PARAMETERS
{
    /** The device state to transition to */
    NDIS_DEVICE_POWER_STATE NewDeviceState;
    
    /** The event that is fired when the oid processing is completed */
    NDIS_EVENT              CompleteEvent;

}MP_POWER_PARAMETERS, *PMP_POWER_PARAMETERS;


/** Used to hold state about port pause */
typedef struct _MP_PORT_PAUSE_PARAMETERS
{
    /** The port that needs to be paused */
    PMP_PORT                PortToPause;

    /** The event that is fired when the pause is completed */
    NDIS_EVENT              CompleteEvent;

}MP_PORT_PAUSE_PARAMETERS, *PMP_PORT_PAUSE_PARAMETERS;


/** Used to hold state about port restart */
typedef struct _MP_PORT_RESTART_PARAMETERS
{
    /** The port that needs to be restarted */
    PMP_PORT                PortToRestart;

    /** The event that is fired when the restart is completed */
    NDIS_EVENT              CompleteEvent;

}MP_PORT_RESTART_PARAMETERS, *PMP_PORT_RESTART_PARAMETERS;

/** Used to hold state about port terminate */
typedef struct _MP_TERMINATE_PORT_PARAMETERS
{
    /** The port that needs to be terminated */
    PMP_PORT                PortToTerminate;

    /** The event that is fired when the terminate is completed */
    NDIS_EVENT              CompleteEvent;
}MP_TERMINATE_PORT_PARAMETERS, *PMP_TERMINATE_PORT_PARAMETERS;

