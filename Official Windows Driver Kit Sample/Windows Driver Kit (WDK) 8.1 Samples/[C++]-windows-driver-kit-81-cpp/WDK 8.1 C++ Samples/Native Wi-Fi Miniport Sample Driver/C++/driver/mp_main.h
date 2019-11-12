/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_main.h

Abstract:
    Contains defines relevant for NDIS entry points into the driver
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once


//
// Handlers for Entry Points from NDIS
//


MINIPORT_UNLOAD DriverUnload;

MINIPORT_SET_OPTIONS MPSetOptions;

MINIPORT_SEND_NET_BUFFER_LISTS MPSendNetBufferLists;

MINIPORT_CANCEL_SEND MPCancelSendNetBufferLists;

MINIPORT_RETURN_NET_BUFFER_LISTS MPReturnNetBufferLists;

MINIPORT_CHECK_FOR_HANG MPCheckForHang;

MINIPORT_RESET MPReset;

MINIPORT_OID_REQUEST MPOidRequest;

MINIPORT_CANCEL_OID_REQUEST MPCancelOidRequest;

MINIPORT_DIRECT_OID_REQUEST MPDirectOidRequest;

MINIPORT_CANCEL_DIRECT_OID_REQUEST MPCancelDirectOidRequest;

NDIS_STATUS
MpGetAdapterStatus(
    _In_  PADAPTER                Adapter
    );

