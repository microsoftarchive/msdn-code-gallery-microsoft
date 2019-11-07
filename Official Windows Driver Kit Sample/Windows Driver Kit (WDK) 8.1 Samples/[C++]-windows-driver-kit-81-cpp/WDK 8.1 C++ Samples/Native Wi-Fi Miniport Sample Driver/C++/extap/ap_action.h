/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    ap_action.h

Abstract:
    ExtAP actions such as start, stop
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    10-01-2007    Created
    
Notes:

--*/
#pragma once
    
#ifndef _AP_ACTION_H
#define _AP_ACTION_H

/** Start AP */
NDIS_STATUS
StartExtAp(
    _In_ PMP_EXTAP_PORT ApPort
    );

/** VNic Start BSS completion handler */
NDIS_STATUS
Ap11StartBSSCompleteCallback(
    _In_ PMP_PORT Port,
    _In_ PVOID Data
    );

/** Stop AP */
NDIS_STATUS
StopExtAp(
    _In_ PMP_EXTAP_PORT ApPort
    );

/** Stop Ext AP BSS */
NDIS_STATUS
StopExtApBss(
    _In_ PMP_EXTAP_PORT ApPort
    );


/** VNic stop BSS completion handler */
NDIS_STATUS
Ap11StopBSSCompleteCallback(
    _In_ PMP_PORT Port,
    _In_ PVOID Data
    );

/** Restart AP */
NDIS_STATUS
RestartExtAp(
    _In_ PMP_EXTAP_PORT ApPort
    );

/** Pause AP */
NDIS_STATUS
PauseExtAp(
    _In_ PMP_EXTAP_PORT ApPort
    );

NDIS_STATUS
Ap11SetChannelCompleteCallback(
    _In_ PMP_PORT Port,
    _In_ PVOID Data
    );

NDIS_STATUS
ConstructAPIEBlob(
    _In_ PMP_EXTAP_PORT ApPort,
    PUCHAR ieBlobPtr,
    BOOLEAN bResponseIE,
    USHORT *pIEBlobSize,
    USHORT *pbytesWritten
    );

#endif // _AP_ACTION_H

