/*++

Copyright (c) 1997-2000  Microsoft Corporation All Rights Reserved

Module Name:

    Msvad.h

Abstract:

    Header file for common stuff.

--*/

#ifndef _MSVAD_H_
#define _MSVAD_H_

#include <portcls.h>
#include <stdunk.h>
#include <ksdebug.h>
#include "kshelper.h"

//=============================================================================
// Defines
//=============================================================================

// Version number. Revision numbers are specified for each sample.
#define MSVAD_VERSION               1

// Revision number.
#define MSVAD_REVISION              0

// Product Id
// {5B722BF8-F0AB-47ee-B9C8-8D61D31375A1}
#define STATIC_PID_MSVAD\
    0x5b722bf8, 0xf0ab, 0x47ee, 0xb9, 0xc8, 0x8d, 0x61, 0xd3, 0x13, 0x75, 0xa1
DEFINE_GUIDSTRUCT("5B722BF8-F0AB-47ee-B9C8-8D61D31375A1", PID_MSVAD);
#define PID_MSVAD DEFINE_GUIDNAMED(PID_MSVAD)

// Pool tag used for MSVAD allocations
#define MSVAD_POOLTAG               'DVSM'  

// Debug module name
#define STR_MODULENAME              "MSVAD: "

// Debug utility macros
#define D_FUNC                      4
#define D_BLAB                      DEBUGLVL_BLAB
#define D_VERBOSE                   DEBUGLVL_VERBOSE        
#define D_TERSE                     DEBUGLVL_TERSE          
#define D_ERROR                     DEBUGLVL_ERROR          
#define DPF                         _DbgPrintF
#define DPF_ENTER(x)                DPF(D_FUNC, x)

// Channel orientation
#define CHAN_LEFT                   0
#define CHAN_RIGHT                  1
#define CHAN_MASTER                 (-1)

// Dma Settings.
#define DMA_BUFFER_SIZE             0x16000

#define KSPROPERTY_TYPE_ALL         KSPROPERTY_TYPE_BASICSUPPORT | \
                                    KSPROPERTY_TYPE_GET | \
                                    KSPROPERTY_TYPE_SET
                                    
// Specific node numbers for vadsampl.sys
#define DEV_SPECIFIC_VT_BOOL 9
#define DEV_SPECIFIC_VT_I4   10
#define DEV_SPECIFIC_VT_UI4  11

// Safe release.
#define SAFE_RELEASE(p) {if (p) { (p)->Release(); (p) = nullptr; } }

//=============================================================================
// Typedefs
//=============================================================================

// Connection table for registering topology/wave bridge connection
typedef struct _PHYSICALCONNECTIONTABLE
{
    ULONG       ulTopologyIn;
    ULONG       ulTopologyOut;
    ULONG       ulWaveIn;
    ULONG       ulWaveOut;
} PHYSICALCONNECTIONTABLE, *PPHYSICALCONNECTIONTABLE;

// This is the structure of the portclass FDO device extension Nt has created
// for us.  We keep the adapter common object here.
struct IAdapterCommon;
typedef struct _PortClassDeviceContext              // 32       64      Byte offsets for 32 and 64 bit architectures
{
    ULONG_PTR m_pulReserved1[2];                    // 0-7      0-15    First two pointers are reserved.
    PDEVICE_OBJECT m_DoNotUsePhysicalDeviceObject;  // 8-11     16-23   Reserved pointer to our Physical Device Object (PDO).
    PVOID m_pvReserved2;                            // 12-15    24-31   Reserved pointer to our Start Device function.
    PVOID m_pvReserved3;                            // 16-19    32-39   "Out Memory" according to DDK.  
    IAdapterCommon* m_pCommon;                      // 20-23    40-47   Pointer to our adapter common object.
    PVOID m_pvUnused1;                              // 24-27    48-55   Unused space.
    PVOID m_pvUnused2;                              // 28-31    56-63   Unused space.

    // Anything after above line should not be used.
    // This actually goes on for (64*sizeof(ULONG_PTR)) but it is all opaque.
} PortClassDeviceContext;

//=============================================================================
// Externs
//=============================================================================

// Physical connection table. Defined in mintopo.cpp for each sample
extern PHYSICALCONNECTIONTABLE TopologyPhysicalConnections;

// Generic topology handler
extern NTSTATUS PropertyHandler_Topology
( 
    IN  PPCPROPERTY_REQUEST     PropertyRequest 
);

// Generic wave port handler
extern NTSTATUS PropertyHandler_Wave
(
    IN  PPCPROPERTY_REQUEST     PropertyRequest
);

// Default WaveFilter automation table.
// Handles the GeneralComponentId request.
extern NTSTATUS PropertyHandler_WaveFilter
(
    IN PPCPROPERTY_REQUEST PropertyRequest
);

#endif

