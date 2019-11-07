////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      Framework_WFPSamplerCalloutDriver.h
//
//   Abstract:
//
//   Author:
//      Dusty Harper      (DHarper)
//
//   Revision History:
//
//      [ Month ][Day] [Year] - [Revision]-[ Comments ]
//      May       01,   2010  -     1.0   -  Creation
//
////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef FRAMEWORK_WFP_SAMPLER_CALLOUT_DRIVER_H
#define FRAMEWORK_WFP_SAMPLER_CALLOUT_DRIVER_H

extern "C"
{
#pragma warning(push)
#pragma warning(disable: 4201) /// NAMELESS_STRUCT_UNION
#pragma warning(disable: 4324) /// STRUCTURE_PADDED

   #include <ntifs.h>                    /// IfsKit\Inc
   #include <ntddk.h>                    /// Inc
   #include <wdf.h>                      /// Inc\WDF\KMDF\1.9
   #include <ndis.h>                     /// Inc
   #include <fwpmk.h>                    /// Inc
   #include <fwpsk.h>                    /// Inc
   #include <netioddk.h>                 /// Inc
   #include <ntintsafe.h>                /// Inc
   #include <ntstrsafe.h>                /// Inc
   #include <stdlib.h>                   /// SDK\Inc\CRT

#pragma warning(pop)
}

#include "Identifiers.h"                     /// ..\Inc
#include "ProviderContexts.h"                /// ..\Inc
#include "HelperFunctions_Include.h"         /// ..\SysLib
#include "HelperFunctions_ExposedCallouts.h" /// .
#include "ClassifyFunctions_Include.h"       /// .
#include "CompletionFunctions_Include.h"     /// .
#include "NotifyFunctions_Include.h"         /// .
#include "SubscriptionFunctions_Include.h"   /// .

#define WFPSAMPLER_CALLOUT_DRIVER_TAG (UINT32)'DCSW'

typedef struct WFPSAMPLER_DEVICE_DATA_
{
   HANDLE*                pEngineHandle;
   INJECTION_HANDLE_DATA* pIPv4InboundInjectionHandles;
   INJECTION_HANDLE_DATA* pIPv4OutboundInjectionHandles;
   INJECTION_HANDLE_DATA* pIPv6InboundInjectionHandles;
   INJECTION_HANDLE_DATA* pIPv6OutboundInjectionHandles;
   INJECTION_HANDLE_DATA* pInboundInjectionHandles;
   INJECTION_HANDLE_DATA* pOutboundInjectionHandles;
}WFPSAMPLER_DEVICE_DATA, *PWFPSAMPLER_DEVICE_DATA;

typedef struct DEVICE_EXTENSION_
{
   VOID*            pRegistration;
   PCALLBACK_OBJECT pCallbackObject;
}DEVICE_EXTENSION, *PDEVICE_EXTENSION;


typedef struct SERIALIZATION_LIST_
{
   KSPIN_LOCK  spinlock;
   LIST_ENTRY  listHead;
   UINT64      numEntries;
}SERIALIZATION_LIST, *PSERIALIZATION_LIST;

extern PDEVICE_OBJECT g_pWDMDevice;

extern WDFDRIVER g_WDFDriver;
extern WDFDEVICE g_WDFDevice;

extern HANDLE g_bfeSubscriptionHandle;

extern WFPSAMPLER_DEVICE_DATA g_WFPSamplerDeviceData;

extern DEVICE_EXTENSION g_deviceExtension;

extern BOOLEAN g_calloutsRegistered;

extern SERIALIZATION_LIST g_bsiSerializationList;

#endif /// FRAMEWORK_WFP_SAMPLER_CALLOUT_DRIVER_H
