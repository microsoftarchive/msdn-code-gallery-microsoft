/*++

Copyright (c) 1997-2000  Microsoft Corporation All Rights Reserved

Module Name:

    simple.h

Abstract:

    Node and Pin numbers for simple sample.

--*/

#ifndef _MSVAD_SIMPLE_H_
#define _MSVAD_SIMPLE_H_

// Pin properties.
#define MAX_INPUT_STREAMS           1       // Number of capture streams.

// PCM Info
#define MIN_CHANNELS                2       // Min Channels.
#define MAX_CHANNELS_PCM            2       // Max Channels.
#define MIN_BITS_PER_SAMPLE_PCM     16      // Min Bits Per Sample
#define MAX_BITS_PER_SAMPLE_PCM     16      // Max Bits Per Sample
#define MIN_SAMPLE_RATE             8000    // Min Sample Rate
#define MAX_SAMPLE_RATE             48000   // Max Sample Rate

// Wave pins
enum 
{
    KSPIN_WAVE_BRIDGE = 0,
    KSPIN_WAVE_HOST
};

// Wave Topology nodes.
enum 
{
    KSNODE_WAVE_ADC = 0
};

// topology pins.
enum
{
    KSPIN_TOPO_MIC_ELEMENTS,
    KSPIN_TOPO_BRIDGE
};

// topology nodes.
enum
{
    KSNODE_TOPO_VOLUME,
    KSNODE_TOPO_MUTE
};

#endif

