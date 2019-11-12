/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    mp_oids.h

Abstract:
    Contains the defines necessay for handling OIDs for the MP layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#pragma once

VOID
MpQuerySupportedOidsList(
    _Inout_ PNDIS_OID            *SupportedOidList,
    _Inout_ PULONG               SupportedOidListLength
    );



