/*++

Copyright (c) 1997-2000  Microsoft Corporation All Rights Reserved

Module Name:

    ac3.h

Abstract:

    Node and Pin numbers for ac3 sample.

--*/

#ifndef _MSVAD_AC3_H_
#define _MSVAD_AC3_H_

// Pin properties.
#define MAX_OUTPUT_STREAMS          1       // Number of capture streams.
#define MAX_INPUT_STREAMS           1       // Number of render streams.
#define MAX_TOTAL_STREAMS           MAX_OUTPUT_STREAMS + MAX_INPUT_STREAMS                      


// To keep the code simple assume device supports only 48KHz, 16-bit, stereo (PCM and AC3)
#define MIN_CHANNELS                2       // Min Channels.
#define MAX_CHANNELS                2       // Max Channels.
#define MIN_BITS_PER_SAMPLE         16      // Min Bits Per Sample
#define MAX_BITS_PER_SAMPLE         16      // Max Bits Per Sample
#define MIN_SAMPLE_RATE             48000   // Min Sample Rate
#define MAX_SAMPLE_RATE             48000   // Max Sample Rate

// Wave pins
enum 
{
    KSPIN_WAVE_CAPTURE_SINK = 0,
    KSPIN_WAVE_CAPTURE_SOURCE,
    KSPIN_WAVE_RENDER_SINK, 
    KSPIN_WAVE_RENDER_SOURCE,
    KSPIN_WAVE_SPDIF_CAPTURE_JACK,
    KSPIN_WAVE_SPDIF_CAPTURE_HOST,
    KSPIN_WAVE_SPDIF_RENDER_HOST,
    KSPIN_WAVE_SPDIF_RENDER_JACK
};

// Wave Topology nodes.
enum 
{
    KSNODE_WAVE_ADC = 0,
    KSNODE_WAVE_DAC
};

// topology pins.
enum
{
    KSPIN_TOPO_WAVEOUT_SOURCE = 0,
    KSPIN_TOPO_SYNTHOUT_SOURCE,
    KSPIN_TOPO_SYNTHIN_SOURCE,
    KSPIN_TOPO_MIC_SOURCE,
    KSPIN_TOPO_LINEOUT_DEST,
    KSPIN_TOPO_WAVEIN_DEST
};

// topology nodes.
enum
{
    KSNODE_TOPO_WAVEOUT_VOLUME = 0,
    KSNODE_TOPO_WAVEOUT_MUTE,
    KSNODE_TOPO_SYNTHOUT_VOLUME,
    KSNODE_TOPO_SYNTHOUT_MUTE,
    KSNODE_TOPO_MIC_VOLUME,
    KSNODE_TOPO_SYNTHIN_VOLUME,
    KSNODE_TOPO_LINEOUT_MIX,
    KSNODE_TOPO_LINEOUT_VOLUME,
    KSNODE_TOPO_WAVEIN_MUX
};

#endif

