/*++

Copyright (c) Microsoft Corporation All Rights Reserved

Module Name:

    kshelper.h

Abstract:

    Helper functions for sysvad

--*/
#ifndef _SYSVAD_KSHELPER_H_
#define _SYSVAD_KSHELPER_H_

#include <portcls.h>
#include <ksdebug.h>

PWAVEFORMATEX                   
GetWaveFormatEx
(
    _In_  PKSDATAFORMAT           pDataFormat
);

NTSTATUS                        
ValidatePropertyParams
(
    _In_ PPCPROPERTY_REQUEST      PropertyRequest, 
    _In_ ULONG                    cbValueSize,
    _In_ ULONG                    cbInstanceSize = 0 
);

NTSTATUS                        
PropertyHandler_BasicSupport
(
    _In_  PPCPROPERTY_REQUEST     PropertyRequest,
    _In_  ULONG                   Flags,
    _In_  DWORD                   PropTypeSetId
);

NTSTATUS
PropertyHandler_BasicSupportVolume
(
    _In_  PPCPROPERTY_REQUEST   PropertyRequest,
    _In_  ULONG                 MaxChannels
);

NTSTATUS
PropertyHandler_BasicSupportMute
(
    _In_  PPCPROPERTY_REQUEST   PropertyRequest,
    _In_  ULONG                 MaxChannels
);

NTSTATUS
PropertyHandler_BasicSupportPeakMeter2
(
    _In_  PPCPROPERTY_REQUEST   PropertyRequest,
    _In_  ULONG                 MaxChannels
);

NTSTATUS
PropertyHandler_CpuResources
( 
    _In_  PPCPROPERTY_REQUEST     PropertyRequest 
);

NTSTATUS
PropertyHandler_Volume
(
    _In_  PADAPTERCOMMON        AdapterCommon,
    _In_  PPCPROPERTY_REQUEST   PropertyRequest,
    _In_  ULONG                 MaxChannels
);

NTSTATUS                            
PropertyHandler_Mute
(
    _In_  PADAPTERCOMMON        AdapterCommon,
    _In_  PPCPROPERTY_REQUEST   PropertyRequest,
    _In_  ULONG                 MaxChannels
);

NTSTATUS
PropertyHandler_PeakMeter2
(
    _In_  PADAPTERCOMMON        AdapterCommon,
    _In_  PPCPROPERTY_REQUEST   PropertyRequest,
    _In_  ULONG                 MaxChannels
);

#endif  // _SYSVAD_KSHELPER_H_

