/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    vnic_recv.c

Abstract:
    Implements the receive processing for the VNIC
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"

#if DOT11_TRACE_ENABLED
#include "vnic_recv.tmh"
#endif

VOID 
VNic11IndicateReceivePackets(
    _In_  PVNIC                   pVNic,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReceiveFlags
    )
{
    Port11IndicateReceivePackets(pVNic->pvPort, PacketList, ReceiveFlags);
}

VOID 
VNic11ReturnPackets(
    _In_  PVNIC                   pVNic,
    _In_  PMP_RX_MSDU             PacketList,
    _In_  ULONG                   ReturnFlags
    )
{
    Hw11ReturnPackets(pVNic->pvHwContext, PacketList, ReturnFlags);
}

