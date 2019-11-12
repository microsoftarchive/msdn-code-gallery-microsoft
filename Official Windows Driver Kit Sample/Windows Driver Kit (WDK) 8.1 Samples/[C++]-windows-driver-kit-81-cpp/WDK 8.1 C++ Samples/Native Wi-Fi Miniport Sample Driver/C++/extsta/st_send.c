/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    st_send.c

Abstract:
    STA layer send processing functions
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    08-01-2005    Created
    01-15-2006    Renamed sta_send.c to st_send.c

Notes:

--*/
#include "precomp.h"
#include "st_scan.h"
#include "st_send.h"

#if DOT11_TRACE_ENABLED
#include "st_send.tmh"
#endif

BOOLEAN
StaCanTransmit(
    _In_  PMP_EXTSTA_PORT        pStation
    )
{
    //
    // If we are in the middle of a scan, this send must be queued
    //
    if (STA_TEST_SCAN_FLAG(pStation, (STA_COMPLETE_PERIODIC_SCAN | STA_COMPLETE_EXTERNAL_SCAN)))
        return TRUE;    // Currently since scans are happening in a different context this is OK.
                        // However if the HVL was removed we would need to return false here

    return TRUE;
}


/* See prototype for documentation */
NDIS_STATUS 
Sta11SendEventHandler(
    _In_  PMP_PORT                Port,
    _In_  PMP_TX_MSDU             PacketList,
    _In_  ULONG                   SendFlags
    )
{
    UNREFERENCED_PARAMETER(PacketList);
    UNREFERENCED_PARAMETER(SendFlags);

    //
    // If we can send packets are this time, we are OK.
    //
    if (!StaCanTransmit(MP_GET_STA_PORT(Port)))
    {
        return NDIS_STATUS_PENDING;
    }

    return NDIS_STATUS_SUCCESS;
}
