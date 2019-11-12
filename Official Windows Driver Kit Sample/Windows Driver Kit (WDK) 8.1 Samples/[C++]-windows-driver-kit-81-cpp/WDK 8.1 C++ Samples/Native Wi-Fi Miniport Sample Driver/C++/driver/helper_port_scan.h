/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    helper_port_scan.h

Abstract:
    Contains helper port scan specific defines
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#pragma once

NDIS_STATUS
HelperPortInitializeScanContext(
    _In_  PMP_HELPER_PORT         HelperPort
    );

VOID
HelperPortTerminateScanContext(
    _In_  PMP_HELPER_PORT         HelperPort
    );

NDIS_STATUS
HelperPortCreateScanChannelList(
    _In_  PMP_HELPER_PORT         HelperPort
    );

VOID
HelperPortScanParametersReleaseRef(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters
    );

VOID
HelperPortProcessPendingScans(
    _In_  PMP_HELPER_PORT         HelperPort
    );

VOID
HelperPortIndicateScanCompletion(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters,
    _In_  PNDIS_STATUS            CompletionStatus
    );

BOOLEAN
HelperPortShouldPerformScan(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters
    );
    
NDIS_STATUS
HelperPortScanExAccessCallback(
    _In_  PMP_PORT                Port, 
    _In_  PVOID                   Ctx
    );

NDIS_STATUS 
HelperPortScanCompleteCallback(
    _In_  PMP_PORT                Port,
    _In_  PVOID                   Data
    );

VOID
HelperPortScanTimerCallback(
    _In_  PMP_HELPER_PORT         HelperPort
    );

NDIS_TIMER_FUNCTION HelperPortScanTimer;

VOID
HelperPortStartScanProcess(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters
    );


VOID
HelperPortCompleteScanProcess(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters,
    _In_  PNDIS_STATUS            CompletionStatus
    );

VOID
HelperPortStartPartialScan(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters
    );
    
VOID
HelperPortCompletePartialScan(
    _In_  PMP_HELPER_PORT         HelperPort,
    _In_  PMP_SCAN_PARAMETERS     ScanParameters,
    _In_  PNDIS_STATUS            CompletionStatus
    );

