/*++

Copyright (c) 1997-2000  Microsoft Corporation All Rights Reserved

Module Name:

    wavtable.h

Abstract:

    Declaration of wave miniport tables.

--*/

#ifndef _MSVAD_WAVTABLE_H_
#define _MSVAD_WAVTABLE_H_

//=============================================================================
// Defines
//=============================================================================

#define STATIC_KSDATAFORMAT_SUBTYPE_DOLBY_AC3_SPDIF\
    DEFINE_WAVEFORMATEX_GUID(WAVE_FORMAT_DOLBY_AC3_SPDIF)
DEFINE_GUIDSTRUCT("00000092-0000-0010-8000-00aa00389b71", KSDATAFORMAT_SUBTYPE_DOLBY_AC3_SPDIF);
#define KSDATAFORMAT_SUBTYPE_DOLBY_AC3_SPDIF DEFINE_GUIDNAMED(KSDATAFORMAT_SUBTYPE_DOLBY_AC3_SPDIF)


//=============================================================================
static
KSDATARANGE_AUDIO PinDataRangesStream[] =
{
    {
        {
            sizeof(KSDATARANGE_AUDIO),
            0,
            0,
            0,
            STATICGUIDOF(KSDATAFORMAT_TYPE_AUDIO),
            STATICGUIDOF(KSDATAFORMAT_SUBTYPE_PCM),
            STATICGUIDOF(KSDATAFORMAT_SPECIFIER_WAVEFORMATEX)
        },
        MAX_CHANNELS,           
        MIN_BITS_PER_SAMPLE,    
        MAX_BITS_PER_SAMPLE,    
        MIN_SAMPLE_RATE,            
        MAX_SAMPLE_RATE             
    },
    // For AC3 support, add a new datarange. 
    // Note that in Vista, this handles all compressed SPDIF formats
    //
    {
        {
            sizeof(KSDATARANGE_AUDIO),
            0,
            0,
            0,
            STATICGUIDOF(KSDATAFORMAT_TYPE_AUDIO),
            STATICGUIDOF(KSDATAFORMAT_SUBTYPE_DOLBY_AC3_SPDIF),
            STATICGUIDOF(KSDATAFORMAT_SPECIFIER_WAVEFORMATEX)
        },
        MAX_CHANNELS,           
        MIN_BITS_PER_SAMPLE,    
        MAX_BITS_PER_SAMPLE,    
        MIN_SAMPLE_RATE,
        MAX_SAMPLE_RATE
    }
};

static
PKSDATARANGE PinDataRangePointersStream[] =
{
    PKSDATARANGE(&PinDataRangesStream[0])
};

static
PKSDATARANGE PinDataRangePointersSPDIFStream[] =
{
    PKSDATARANGE(&PinDataRangesStream[0]),  // PCM formats
    PKSDATARANGE(&PinDataRangesStream[1])   // Compressed SPDIF formats
};

//=============================================================================
static
KSDATARANGE PinDataRangesBridge[] =
{
    {
        sizeof(KSDATARANGE),
        0,
        0,
        0,
        STATICGUIDOF(KSDATAFORMAT_TYPE_AUDIO),
        STATICGUIDOF(KSDATAFORMAT_SUBTYPE_ANALOG),
        STATICGUIDOF(KSDATAFORMAT_SPECIFIER_NONE)
    },
    {
        sizeof(KSDATARANGE),
        0,
        0,
        0,
        STATICGUIDOF(KSDATAFORMAT_TYPE_AUDIO),
        STATICGUIDOF(KSDATAFORMAT_SUBTYPE_AC3_AUDIO),
        STATICGUIDOF(KSDATAFORMAT_SPECIFIER_NONE)
    }
};

static
PKSDATARANGE PinDataRangePointersBridge[] =
{
    &PinDataRangesBridge[0]
};

static
PKSDATARANGE PinDataRangePointersSPDIFJacks[] =
{
    &PinDataRangesBridge[1]
};


//=============================================================================
static
PCPIN_DESCRIPTOR MiniportPins[] =
{
    // Wave In Streaming Pin (Capture) KSPIN_WAVE_CAPTURE_SINK
    {
        MAX_OUTPUT_STREAMS,
        MAX_OUTPUT_STREAMS,
        0,
        NULL,
        {
            0,
            NULL,
            0,
            NULL,
            SIZEOF_ARRAY(PinDataRangePointersStream),
            PinDataRangePointersStream,
            KSPIN_DATAFLOW_OUT,
            KSPIN_COMMUNICATION_SINK,
            &KSCATEGORY_AUDIO,
            &KSAUDFNAME_RECORDING_CONTROL,
            0
        }
    },
    
    // Wave In Bridge Pin (Capture - From Topology) KSPIN_WAVE_CAPTURE_SOURCE
    {
        0,
        0,
        0,
        NULL,
        {
            0,
            NULL,
            0,
            NULL,
            SIZEOF_ARRAY(PinDataRangePointersBridge),
            PinDataRangePointersBridge,
            KSPIN_DATAFLOW_IN,
            KSPIN_COMMUNICATION_NONE,
            &KSCATEGORY_AUDIO,
            NULL,
            0
        }
    },
  
    // Wave Out Streaming Pin (Renderer) KSPIN_WAVE_RENDER_SINK
    {
        MAX_INPUT_STREAMS,
        MAX_INPUT_STREAMS, 
        0,
        NULL,
        {
            0,
            NULL,
            0,
            NULL,
            SIZEOF_ARRAY(PinDataRangePointersStream),
            PinDataRangePointersStream,
            KSPIN_DATAFLOW_IN,
            KSPIN_COMMUNICATION_SINK,
            &KSCATEGORY_AUDIO,
            NULL,
            0
        }
    },
  
    // Wave Out Bridge Pin (Renderer) KSPIN_WAVE_RENDER_SOURCE
    {
        0,
        0,
        0,
        NULL,
        {
            0,
            NULL,
            0,
            NULL,
            SIZEOF_ARRAY(PinDataRangePointersBridge),
            PinDataRangePointersBridge,
            KSPIN_DATAFLOW_OUT,
            KSPIN_COMMUNICATION_NONE,
            &KSCATEGORY_AUDIO,
            NULL,
            0
        }
    },


    // SPDIF In Bridge Pin (Capturer) KSPIN_WAVE_SPDIF_CAPTURE_JACK
    {
        0,
        0,
        0,
        NULL,
        {
            0,
            NULL,
            0,
            NULL,
            SIZEOF_ARRAY(PinDataRangePointersSPDIFJacks),
            PinDataRangePointersSPDIFJacks,
            KSPIN_DATAFLOW_IN,
            KSPIN_COMMUNICATION_NONE,
            &KSNODETYPE_SPDIF_INTERFACE,
            NULL,
            0
        }
    },

    // SPDIF In Streaming Pin (Capturer) KSPIN_WAVE_SPDIF_CAPTURE_HOST
    {
        1,
        1,
        0,
        NULL,
        {
            0,
            NULL,
            0,
            NULL,
            SIZEOF_ARRAY(PinDataRangePointersSPDIFStream),
            PinDataRangePointersSPDIFStream,
            KSPIN_DATAFLOW_OUT,
            KSPIN_COMMUNICATION_SINK,
            &KSCATEGORY_AUDIO,
            NULL,
            0
        }
    },

    // SPDIF Out Streaming Pin (Renderer) KSPIN_WAVE_SPDIF_RENDER_HOST
    {
        1,
        1,
        0,
        NULL,
        {
            0,
            NULL,
            0,
            NULL,
            SIZEOF_ARRAY(PinDataRangePointersSPDIFStream),
            PinDataRangePointersSPDIFStream,
            KSPIN_DATAFLOW_IN,
            KSPIN_COMMUNICATION_SINK,
            &KSCATEGORY_AUDIO,
            NULL,
            0
        }
    },


    // SPDIF Out Bridge Pin (Renderer) KSPIN_WAVE_SPDIF_RENDER_JACK
    {
        0,
        0,
        0,
        NULL,
        {
            0,
            NULL,
            0,
            NULL,
            SIZEOF_ARRAY(PinDataRangePointersSPDIFJacks),
            PinDataRangePointersSPDIFJacks,
            KSPIN_DATAFLOW_OUT,
            KSPIN_COMMUNICATION_NONE,
            &KSNODETYPE_SPDIF_INTERFACE,
            NULL,
            0
        }
    },
};

//=============================================================================
static
PCNODE_DESCRIPTOR MiniportNodes[] =
{
    // KSNODE_WAVE_ADC
    {
        0,                      // Flags
        NULL,                   // AutomationTable
        &KSNODETYPE_ADC,        // Type
        NULL                    // Name
    },
    // KSNODE_WAVE_DAC
    {
        0,                      // Flags
        NULL,                   // AutomationTable
        &KSNODETYPE_DAC,        // Type
        NULL                    // Name
    }
};


//=============================================================================
static
PCCONNECTION_DESCRIPTOR MiniportConnections[] =
{
    { PCFILTER_NODE,        KSPIN_WAVE_CAPTURE_SOURCE,      KSNODE_WAVE_ADC,    1 },    
    { KSNODE_WAVE_ADC,      0,                              PCFILTER_NODE,      KSPIN_WAVE_CAPTURE_SINK },    

    { PCFILTER_NODE,        KSPIN_WAVE_RENDER_SINK,         KSNODE_WAVE_DAC,    1 },    
    { KSNODE_WAVE_DAC,      0,                              PCFILTER_NODE,      KSPIN_WAVE_RENDER_SOURCE },    

    { PCFILTER_NODE,        KSPIN_WAVE_SPDIF_CAPTURE_JACK,  PCFILTER_NODE,      KSPIN_WAVE_SPDIF_CAPTURE_HOST},

    { PCFILTER_NODE,        KSPIN_WAVE_SPDIF_RENDER_HOST,   PCFILTER_NODE,      KSPIN_WAVE_SPDIF_RENDER_JACK}
};

//=============================================================================
static
PCFILTER_DESCRIPTOR MiniportFilterDescriptor =
{
    0,                                  // Version
    NULL,                               // AutomationTable
    sizeof(PCPIN_DESCRIPTOR),           // PinSize
    SIZEOF_ARRAY(MiniportPins),         // PinCount
    MiniportPins,                       // Pins
    sizeof(PCNODE_DESCRIPTOR),          // NodeSize
    SIZEOF_ARRAY(MiniportNodes),        // NodeCount
    MiniportNodes,                      // Nodes
    SIZEOF_ARRAY(MiniportConnections),  // ConnectionCount
    MiniportConnections,                // Connections
    0,                                  // CategoryCount
    NULL                                // Categories - NULL->use defaults (AUDIO RENDER CAPTURE)
};

#endif
