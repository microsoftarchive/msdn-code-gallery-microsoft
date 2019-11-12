#pragma once

VOID
VNicProcessQueuedPkts(
    _In_ PVNIC pVNic, 
    BOOLEAN fDispatchLevel
    );

_IRQL_requires_(DISPATCH_LEVEL)
VOID
VNicCancelPendingSends(
    _In_  PVNIC                   pVNic
    );
