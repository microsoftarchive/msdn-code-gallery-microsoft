/*++

Copyright (c) Microsoft Corporation All Rights Reserved

Module Name:

    minipairs.h

Abstract:

    Local audio endpoint filter definitions. 

--*/

#ifndef _SYSVAD_MINIPAIRS_H_
#define _SYSVAD_MINIPAIRS_H_

#include "speakertopo.h"
#include "speakertoptable.h"
#include "speakerwavtable.h"

#include "speakerhptopo.h"
#include "speakerhptoptable.h"
#include "speakerhpwavtable.h"

#include "hdmitopo.h"
#include "hdmitoptable.h"
#include "hdmiwavtable.h"

#include "micintopo.h"
#include "micintoptable.h"
#include "micinwavtable.h"

#include "micarraytopo.h"
#include "micarray1toptable.h"
#include "micarray2toptable.h"
#include "micarray3toptable.h"
#include "micarraywavtable.h"
#include "micarray3wavtable.h"

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
* Topology/Wave bridge connection for speaker (internal)             *
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
PHYSICALCONNECTIONTABLE SpeakerTopologyPhysicalConnections =
{
    KSPIN_TOPO_WAVEOUT_SOURCE,  // TopologyIn
    (ULONG)-1,                  // TopologyOut
    (ULONG)-1,                  // WaveIn
    KSPIN_WAVE_RENDER_SOURCE    // WaveOut
};

static
ENDPOINT_MINIPAIR SpeakerMiniports =
{
    eSpeakerDevice,
    L"TopologySpeaker",                   // make sure this name matches with SYSVAD.TopologySpeaker.szPname in the inf's [Strings] section 
    CreateMiniportTopologySYSVAD,
    &SpeakerTopoMiniportFilterDescriptor,
    L"WaveSpeaker",                       // make sure this name matches with SYSVAD.WaveSpeaker.szPname in the inf's [Strings] section
    CreateMiniportWaveRTSYSVAD,
    &SpeakerWaveMiniportFilterDescriptor,
    SPEAKER_DEVICE_MAX_CHANNELS,
    SpeakerPinDeviceFormatsAndModes,
    SIZEOF_ARRAY(SpeakerPinDeviceFormatsAndModes),
    &SpeakerTopologyPhysicalConnections,
    ENDPOINT_OFFLOAD_SUPPORTED
};

/*********************************************************************
* Topology/Wave bridge connection for speaker (external:headphone)   *
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
PHYSICALCONNECTIONTABLE SpeakerHpTopologyPhysicalConnections =
{
    KSPIN_TOPO_WAVEOUT_SOURCE,  // TopologyIn
    (ULONG)-1,                  // TopologyOut
    (ULONG)-1,                  // WaveIn
    KSPIN_WAVE_RENDER_SOURCE    // WaveOut
};

static
ENDPOINT_MINIPAIR SpeakerHpMiniports =
{
    eSpeakerHpDevice,
    L"TopologySpeakerHeadphone",            // make sure this name matches with SYSVAD.TopologySpeakerHeadphone.szPname in the inf's [Strings] section 
    CreateMiniportTopologySYSVAD,
    &SpeakerHpTopoMiniportFilterDescriptor,
    L"WaveSpeakerHeadphone",                // make sure this name matches with SYSVAD.WaveSpeakerHeadphone.szPname in the inf's [Strings] section
    CreateMiniportWaveRTSYSVAD,
    &SpeakerHpWaveMiniportFilterDescriptor,
    SPEAKERHP_DEVICE_MAX_CHANNELS,
    SpeakerHpPinDeviceFormatsAndModes,
    SIZEOF_ARRAY(SpeakerHpPinDeviceFormatsAndModes),
    &SpeakerHpTopologyPhysicalConnections,
    ENDPOINT_OFFLOAD_SUPPORTED
};

/*********************************************************************
* Topology/Wave bridge connection for hdmi endpoint                  *
*                                                                    *
*              +------+                +------+                      *
*              | Wave |                | Topo |                      *
*              |      |                |      |                      *
* System   --->|0    1|---> Loopback   |      |                      *
*              |      |                |      |                      *
*              |     2|--------------->|0    1|---> Line Out         *
*              |      |                |      |                      *
*              +------+                +------+                      *
*********************************************************************/
static
PHYSICALCONNECTIONTABLE HdmiTopologyPhysicalConnections =
{
    KSPIN_TOPO_WAVEOUT_SOURCE,  // TopologyIn
    (ULONG)-1,                  // TopologyOut
    (ULONG)-1,                  // WaveIn
    KSPIN_WAVE_RENDER2_SOURCE   // WaveOut (no offloading)
};

static
ENDPOINT_MINIPAIR HdmiMiniports =
{
    eHdmiDevice,
    L"TopologyHdmi",                   // make sure this name matches with SYSVAD.TopologyHdmi.szPname in the inf's [Strings] section 
    CreateHdmiMiniportTopology,
    &HdmiTopoMiniportFilterDescriptor,
    L"WaveHdmi",                       // make sure this name matches with SYSVAD.WaveHdmi.szPname in the inf's [Strings] section
    CreateMiniportWaveRTSYSVAD,
    &HdmiWaveMiniportFilterDescriptor,
    HDMI_DEVICE_MAX_CHANNELS,
    HdmiPinDeviceFormatsAndModes,
    SIZEOF_ARRAY(HdmiPinDeviceFormatsAndModes),
    &HdmiTopologyPhysicalConnections,
    ENDPOINT_NO_FLAGS
};

//
// Capture miniports.
//

/*********************************************************************
* Topology/Wave bridge connection for mic in                         *
*                                                                    *
*              +------+    +------+                                  *
*              | Topo |    | Wave |                                  *
*              |      |    |      |                                  *
*  Mic in  --->|0    1|===>|0    1|---> Capture Host Pin             *
*              |      |    |      |                                  *
*              +------+    +------+                                  *
*********************************************************************/
static
PHYSICALCONNECTIONTABLE MicInTopologyPhysicalConnections =
{
    (ULONG)-1,                  // TopologyIn
    KSPIN_TOPO_BRIDGE,          // TopologyOut
    KSPIN_WAVE_BRIDGE,          // WaveIn
    (ULONG)-1                   // WaveOut
};

static
ENDPOINT_MINIPAIR MicInMiniports =
{
    eMicInDevice,
    L"TopologyMicIn",                   // make sure this name matches with SYSVAD.TopologyMicIn.szPname in the inf's [Strings] section 
    CreateMiniportTopologySYSVAD,
    &MicInTopoMiniportFilterDescriptor,
    L"WaveMicIn",                       // make sure this name matches with SYSVAD.WaveMicIn.szPname in the inf's [Strings] section
    CreateMiniportWaveRTSYSVAD,
    &MicInWaveMiniportFilterDescriptor,
    MICIN_DEVICE_MAX_CHANNELS,
    MicInPinDeviceFormatsAndModes,
    SIZEOF_ARRAY(MicInPinDeviceFormatsAndModes),
    &MicInTopologyPhysicalConnections,
    ENDPOINT_NO_FLAGS
};

/*********************************************************************
* Topology/Wave bridge connection for mic array  1 (front)           *
*                                                                    *
*              +------+    +------+                                  *
*              | Topo |    | Wave |                                  *
*              |      |    |      |                                  *
*  Mic in  --->|0    1|===>|0    1|---> Capture Host Pin             *
*              |      |    |      |                                  *
*              +------+    +------+                                  *
*********************************************************************/
static
PHYSICALCONNECTIONTABLE MicArray1TopologyPhysicalConnections =
{
    (ULONG)-1,                  // TopologyIn
    KSPIN_TOPO_BRIDGE,          // TopologyOut
    KSPIN_WAVE_BRIDGE,          // WaveIn
    (ULONG)-1                   // WaveOut
};

static
ENDPOINT_MINIPAIR MicArray1Miniports =
{
    eMicArrayDevice1,
    L"TopologyMicArray1",                // make sure this name matches with SYSVAD.TopologyMicArray1.szPname in the inf's [Strings] section 
    CreateMicArrayMiniportTopology,
    &MicArray1TopoMiniportFilterDescriptor,
    L"WaveMicArray1",                    // make sure this name matches with SYSVAD.WaveMicArray1.szPname in the inf's [Strings] section
    CreateMiniportWaveRTSYSVAD,
    &MicArrayWaveMiniportFilterDescriptor,
    MICARRAY_DEVICE_MAX_CHANNELS,
    MicArrayPinDeviceFormatsAndModes,
    SIZEOF_ARRAY(MicArrayPinDeviceFormatsAndModes),
    &MicArray1TopologyPhysicalConnections,
    ENDPOINT_NO_FLAGS
};

/*********************************************************************
* Topology/Wave bridge connection for mic array  2 (back)            *
*                                                                    *
*              +------+    +------+                                  *
*              | Topo |    | Wave |                                  *
*              |      |    |      |                                  *
*  Mic in  --->|0    1|===>|0    1|---> Capture Host Pin             *
*              |      |    |      |                                  *
*              +------+    +------+                                  *
*********************************************************************/
static
PHYSICALCONNECTIONTABLE MicArray2TopologyPhysicalConnections =
{
    (ULONG)-1,                  // TopologyIn
    KSPIN_TOPO_BRIDGE,          // TopologyOut
    KSPIN_WAVE_BRIDGE,          // WaveIn
    (ULONG)-1                   // WaveOut
};

static
ENDPOINT_MINIPAIR MicArray2Miniports =
{
    eMicArrayDevice2,
    L"TopologyMicArray2",                // make sure this name matches with SYSVAD.TopologyMicArray2.szPname in the inf's [Strings] section 
    CreateMicArrayMiniportTopology,
    &MicArray2TopoMiniportFilterDescriptor,
    L"WaveMicArray2",                    // make sure this name matches with SYSVAD.WaveMicArray2.szPname in the inf's [Strings] section
    CreateMiniportWaveRTSYSVAD,
    &MicArrayWaveMiniportFilterDescriptor,
    MICARRAY_DEVICE_MAX_CHANNELS,
    MicArrayPinDeviceFormatsAndModes,
    SIZEOF_ARRAY(MicArrayPinDeviceFormatsAndModes),
    &MicArray2TopologyPhysicalConnections,
    ENDPOINT_NO_FLAGS
};

/*********************************************************************
* Topology/Wave bridge connection for mic array  3 (combined)        *
*                                                                    *
*              +------+    +------+                                  *
*              | Topo |    | Wave |                                  *
*              |      |    |      |                                  *
*  Mic in  --->|0    1|===>|0    1|---> Capture Host Pin             *
*              |      |    |      |                                  *
*              +------+    +------+                                  *
*********************************************************************/
static
PHYSICALCONNECTIONTABLE MicArray3TopologyPhysicalConnections =
{
    (ULONG)-1,                  // TopologyIn
    KSPIN_TOPO_BRIDGE,          // TopologyOut
    KSPIN_WAVE_BRIDGE,          // WaveIn
    (ULONG)-1                   // WaveOut
};

static
ENDPOINT_MINIPAIR MicArray3Miniports =
{
    eMicArrayDevice3,
    L"TopologyMicArray3",                // make sure this name matches with SYSVAD.TopologyMicArray3.szPname in the inf's [Strings] section 
    CreateMicArrayMiniportTopology,
    &MicArray3TopoMiniportFilterDescriptor,
    L"WaveMicArray3",                    // make sure this name matches with SYSVAD.WaveMicArray3.szPname in the inf's [Strings] section
    CreateMiniportWaveRTSYSVAD,
    &MicArray3WaveMiniportFilterDescriptor,
    MICARRAY3_DEVICE_MAX_CHANNELS,
    MicArray3PinDeviceFormatsAndModes,
    SIZEOF_ARRAY(MicArray3PinDeviceFormatsAndModes),
    &MicArray3TopologyPhysicalConnections,
    ENDPOINT_NO_FLAGS
};

//=============================================================================
//
// Render miniport pairs.
//
static
PENDPOINT_MINIPAIR  g_RenderEndpoints[] = 
{
    &SpeakerMiniports,
    &SpeakerHpMiniports,
    &HdmiMiniports,
};

#define g_cRenderEndpoints  (SIZEOF_ARRAY(g_RenderEndpoints))

//=============================================================================
//
// Capture miniport pairs.
//
static
PENDPOINT_MINIPAIR  g_CaptureEndpoints[] = 
{
    &MicInMiniports,
    &MicArray1Miniports,
    &MicArray2Miniports,
    &MicArray3Miniports,
};

#define g_cCaptureEndpoints (SIZEOF_ARRAY(g_CaptureEndpoints))

//=============================================================================
//
// Total miniports = # endpoints * 2 (topology + wave).
//
#define g_MaxMiniports  ((g_cRenderEndpoints + g_cCaptureEndpoints) * 2)

#endif // _SYSVAD_MINIPAIRS_H_

