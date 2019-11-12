/*++

Copyright (c) Microsoft Corporation All Rights Reserved

Module Name:

    bthhfpminipairs.h

Abstract:

    Bluetooth HFP audio endpoint filter definitions. 

--*/

#ifndef _SYSVAD_BTHHFPMINIPAIRS_H_
#define _SYSVAD_BTHHFPMINIPAIRS_H_

#include "bthhfpspeakertopo.h"
#include "bthhfpspeakertoptable.h"
#include "bthhfpspeakerwavtable.h"

#include "bthhfpmictopo.h"
#include "bthhfpmictoptable.h"
#include "bthhfpmicwavtable.h"

NTSTATUS
CreateMiniportWaveRTSYSVAD
( 
    _Out_       PUNKNOWN *,
    _In_        REFCLSID,
    _In_opt_    PUNKNOWN,
    _In_        POOL_TYPE,
    _In_        PUNKNOWN,
    _In_opt_    PVOID,
    _In_        PENDPOINT_MINIPAIR
);

NTSTATUS
CreateMiniportTopologySYSVAD
( 
    _Out_       PUNKNOWN *,
    _In_        REFCLSID,
    _In_opt_    PUNKNOWN,
    _In_        POOL_TYPE,
    _In_        PUNKNOWN,
    _In_opt_    PVOID,
    _In_        PENDPOINT_MINIPAIR
);

//
// Render miniports.
//

/*********************************************************************
* Topology/Wave bridge connection for BTH-HFP speaker                *
*                                                                    *
*              +------+                +------+                      *
*              | Wave |                | Topo |                      *
*              |      |                |      |                      *
* System   --->|0    2|---> Loopback   |      |                      *
*              |      |                |      |                      *
* Offload  --->|1    3|--------------->|0    1|---> Line Out         *
*              |      |                |      |                      *
*              +------+                +------+                      *
*********************************************************************/
static
PHYSICALCONNECTIONTABLE BthHfpSpeakerTopologyPhysicalConnections =
{
    KSPIN_TOPO_WAVEOUT_SOURCE,  // TopologyIn
    (ULONG)-1,                  // TopologyOut
    (ULONG)-1,                  // WaveIn
    KSPIN_WAVE_RENDER_SOURCE    // WaveOut
};

static
ENDPOINT_MINIPAIR BthHfpSpeakerMiniports =
{
    eBthHfpSpeakerDevice,
    L"TopologyBthHfpSpeaker",               // make sure this name matches with SYSVAD.TopologyBthHfpSpeaker.szPname in the inf's [Strings] section 
    CreateMiniportTopologySYSVAD,
    &BthHfpSpeakerTopoMiniportFilterDescriptor,
    L"WaveBthHfpSpeaker",                   // make sure this name matches with SYSVAD.WaveBthHfpSpeaker.szPname in the inf's [Strings] section
    CreateMiniportWaveRTSYSVAD,
    &BthHfpSpeakerWaveMiniportFilterDescriptor,
    1,
    BthHfpSpeakerPinDeviceFormatsAndModes,
    SIZEOF_ARRAY(BthHfpSpeakerPinDeviceFormatsAndModes),
    &BthHfpSpeakerTopologyPhysicalConnections,
    ENDPOINT_OFFLOAD_SUPPORTED
};

//
// Capture miniports.
//

/*********************************************************************
* Topology/Wave bridge connection for BTH-HFP mic                    *
*                                                                    *
*              +------+    +------+                                  *
*              | Topo |    | Wave |                                  *
*              |      |    |      |                                  *
*  Mic in  --->|0    1|===>|0    1|---> Capture Host Pin             *
*              |      |    |      |                                  *
*              +------+    +------+                                  *
*********************************************************************/
static
PHYSICALCONNECTIONTABLE BthHfpMicTopologyPhysicalConnections =
{
    (ULONG)-1,                  // TopologyIn
    KSPIN_TOPO_BRIDGE,          // TopologyOut
    KSPIN_WAVE_BRIDGE,          // WaveIn
    (ULONG)-1                   // WaveOut
};

static
ENDPOINT_MINIPAIR BthHfpMicMiniports =
{
    eBthHfpMicDevice,
    L"TopologyBthHfpMic",                   // make sure this name matches with SYSVAD.TopologyBthHfpMic.szPname in the inf's [Strings] section 
    CreateMiniportTopologySYSVAD,
    &BthHfpMicTopoMiniportFilterDescriptor,
    L"WaveBthHfpMic",                       // make sure this name matches with SYSVAD.WaveBthHfpMic.szPname in the inf's [Strings] section
    CreateMiniportWaveRTSYSVAD,
    &BthHfpMicWaveMiniportFilterDescriptor,
    1,
    BthHfpMicPinDeviceFormatsAndModes,
    SIZEOF_ARRAY(BthHfpMicPinDeviceFormatsAndModes),
    &BthHfpMicTopologyPhysicalConnections,
    ENDPOINT_NO_FLAGS
};

//=============================================================================
//
// Render miniport pairs.
//
static
PENDPOINT_MINIPAIR  g_BthHfpRenderEndpoints[] = 
{
    &BthHfpSpeakerMiniports
};

#define g_cBthHfpRenderEndpoints  (SIZEOF_ARRAY(g_BthHfpRenderEndpoints))
C_ASSERT(g_cBthHfpRenderEndpoints == 1);

//=============================================================================
//
// Capture miniport pairs.
//
static
PENDPOINT_MINIPAIR  g_BthHfpCaptureEndpoints[] = 
{
    &BthHfpMicMiniports
};

#define g_cBthHfpCaptureEndpoints (SIZEOF_ARRAY(g_BthHfpCaptureEndpoints))
C_ASSERT(g_cBthHfpCaptureEndpoints == 1);

//=============================================================================
//
// Total miniports = # endpoints * 2 (topology + wave).
//
#define g_MaxBthHfpMiniports  ((g_cBthHfpRenderEndpoints + g_cBthHfpCaptureEndpoints) * 2)

#endif // _SYSVAD_BTHHFPMINIPAIRS_H_

