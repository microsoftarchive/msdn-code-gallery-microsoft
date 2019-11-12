////////////////////////////////////////////////////////////////////////////////////////////////////
//
//   Copyright (c) 2012 Microsoft Corporation.  All Rights Reserved.
//
//   Module Name:
//      Framework_WFPSamplerCalloutDriver.cpp
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

#include "Framework_WFPSamplerCalloutDriver.h"   /// .
#include "Framework_Include.h"                   /// .
#include "Framework_WFPSamplerCalloutDriver.tmh" /// $(OBJ_PATH)\$(O)\

extern "C"
{
   _IRQL_requires_(PASSIVE_LEVEL)
   _IRQL_requires_same_
   _Function_class_(DRIVER_INITIALIZE)
   DRIVER_INITIALIZE DriverEntry;
};

PDEVICE_OBJECT         g_pWDMDevice            = 0;
NDIS_POOL_DATA*        g_pNDISPoolData         = 0;
BOOLEAN                g_calloutsRegistered    = FALSE;
HANDLE                 g_bfeSubscriptionHandle = 0;
SERIALIZATION_LIST     g_bsiSerializationList  = {0};
WFPSAMPLER_DEVICE_DATA g_WFPSamplerDeviceData  = {0};
DEVICE_EXTENSION       g_deviceExtension       = {0};
WDFDRIVER              g_WDFDriver;
WDFDEVICE              g_WDFDevice;

VOID PrvBasicStreamInjectionSerializationListInitialization()
{
   KeInitializeSpinLock(&(g_bsiSerializationList.spinlock));

   InitializeListHead(&(g_bsiSerializationList.listHead));

   g_bsiSerializationList.numEntries = 0;

   return;
}

/**
 @private_function="PrvFwpmBfeStateSubscribeChanges"
 
   Purpose:  Subscribe to notifications of changes to the BFE service's state.                  <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF550062.aspx             <br>
*/
_Success_(return == STATUS_SUCCESS)
NTSTATUS PrvFwpmBfeStateSubscribeChanges()
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PrvFwpmBfeStateSubscribeChanges()\n");

#endif /// DBG
   
   NTSTATUS status = STATUS_SUCCESS;

   status = FwpmBfeStateSubscribeChanges(g_pWDMDevice,
                                         SubscriptionBFEStateChangeCallback,
                                         &g_WFPSamplerDeviceData,
                                         &g_bfeSubscriptionHandle);
   if(status != STATUS_SUCCESS)
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PrvFwpmBfeStateSubscribeChanges : FwpmBfeStateSubscribeChanges() [status: %#x]\n",
                 status);

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PrvFwpmBfeStateSubscribeChanges() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @private_function="PrvWFPSamplerDeviceDataPopulate"
 
   Purpose:  Register the  global WFP objects such as:                                          <br>
                Callouts                                                                        <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref:                                                                                    <br>
*/
_IRQL_requires_(PASSIVE_LEVEL)
_IRQL_requires_same_
_Check_return_
_Success_(return == STATUS_SUCCESS)
NTSTATUS PrvWFPSamplerFwpObjectsAddGlobal(_In_ WFPSAMPLER_DEVICE_DATA* pDeviceData)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PrvWFPSamplerFwpObjectsAddGlobal()\n");

#endif /// DBG
   
   UNREFERENCED_PARAMETER(pDeviceData);

   NT_ASSERT(pDeviceData);

   NTSTATUS status = STATUS_SUCCESS;

   if(g_calloutsRegistered == FALSE)
   {
      status = KrnlHlprExposedCalloutsRegister();
      HLPR_BAIL_ON_FAILURE(status);

      g_calloutsRegistered = TRUE;
   }

   HLPR_BAIL_LABEL:

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PrvWFPSamplerFwpObjectsAddGlobal() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @private_function="PrvWFPSamplerDeviceDataPopulate"
 
   Purpose:  Populate the deviceData with the BFE engineHandle and various injection handles.   <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF550061.aspx             <br>
*/
_IRQL_requires_(PASSIVE_LEVEL)
_IRQL_requires_same_
_Check_return_
_Success_(return == STATUS_SUCCESS)
NTSTATUS PrvWFPSamplerDeviceDataPopulate(_Inout_ WFPSAMPLER_DEVICE_DATA* pDeviceData)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PrvWFPSamplerDeviceDataPopulate()\n");

#endif /// DBG
   
   NT_ASSERT(pDeviceData);

   NTSTATUS status = STATUS_SUCCESS;

   if(FwpmBfeStateGet() == FWPM_SERVICE_RUNNING)
   {
      status = KrnlHlprFwpmSessionCreateEngineHandle(&(pDeviceData->pEngineHandle));
      HLPR_BAIL_ON_FAILURE(status);
   }

#pragma warning(push)
#pragma warning(disable: 6388) /// all injection Handles will be 0

   status = KrnlHlprInjectionHandleDataCreate(&(pDeviceData->pIPv4InboundInjectionHandles),
                                              AF_INET,
                                              FWP_DIRECTION_INBOUND);
   HLPR_BAIL_ON_FAILURE(status);

   status = KrnlHlprInjectionHandleDataCreate(&(pDeviceData->pIPv4OutboundInjectionHandles),
                                              AF_INET,
                                              FWP_DIRECTION_OUTBOUND);
   HLPR_BAIL_ON_FAILURE(status);

   status = KrnlHlprInjectionHandleDataCreate(&(pDeviceData->pIPv6InboundInjectionHandles),
                                              AF_INET6,
                                              FWP_DIRECTION_INBOUND);
   HLPR_BAIL_ON_FAILURE(status);

   status = KrnlHlprInjectionHandleDataCreate(&(pDeviceData->pIPv6OutboundInjectionHandles),
                                              AF_INET6,
                                              FWP_DIRECTION_OUTBOUND);
   HLPR_BAIL_ON_FAILURE(status);

   status = KrnlHlprInjectionHandleDataCreate(&(pDeviceData->pInboundInjectionHandles),
                                              AF_UNSPEC,
                                              FWP_DIRECTION_INBOUND);
   HLPR_BAIL_ON_FAILURE(status);

   status = KrnlHlprInjectionHandleDataCreate(&(pDeviceData->pOutboundInjectionHandles),
                                              AF_UNSPEC,
                                              FWP_DIRECTION_OUTBOUND);
   HLPR_BAIL_ON_FAILURE(status);

#pragma warning(pop)

   HLPR_BAIL_LABEL:

   if(status != STATUS_SUCCESS)
   {
      if(pDeviceData->pOutboundInjectionHandles)
         KrnlHlprInjectionHandleDataDestroy(&(pDeviceData->pOutboundInjectionHandles));

      if(pDeviceData->pInboundInjectionHandles)
         KrnlHlprInjectionHandleDataDestroy(&(pDeviceData->pInboundInjectionHandles));

      if(pDeviceData->pIPv6OutboundInjectionHandles)
         KrnlHlprInjectionHandleDataDestroy(&(pDeviceData->pIPv6OutboundInjectionHandles));

      if(pDeviceData->pIPv6InboundInjectionHandles)
         KrnlHlprInjectionHandleDataDestroy(&(pDeviceData->pIPv6InboundInjectionHandles));

      if(pDeviceData->pIPv4OutboundInjectionHandles)
         KrnlHlprInjectionHandleDataDestroy(&(pDeviceData->pIPv4OutboundInjectionHandles));

      if(pDeviceData->pIPv4InboundInjectionHandles)
         KrnlHlprInjectionHandleDataDestroy(&(pDeviceData->pIPv4InboundInjectionHandles));

      if(pDeviceData->pEngineHandle &&
         *(pDeviceData->pEngineHandle))
         KrnlHlprFwpmSessionReleaseHandle(pDeviceData->pEngineHandle);
   }

#if DBG

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_INFO_LEVEL,
                 " <--- PrvWFPSamplerDeviceDataPopulate()\n");
   
#endif /// DBG

   return status;
}

/**
 @private_function="PrvDriverDeviceAdd"
 
   Purpose:  Add the device and configure initial WFP callout driver settings.                  <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF545841.aspx             <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF546090.aspx             <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF545926.aspx             <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF546942.aspx             <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF545854.aspx             <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF546050.aspx             <br>
*/
_IRQL_requires_(PASSIVE_LEVEL)
_IRQL_requires_same_
_Check_return_
_Success_(return == STATUS_SUCCESS)
NTSTATUS PrvDriverDeviceAdd(_In_ WDFDRIVER* pWDFDriver)
{
#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> PrvDriverDeviceAdd()\n");

#endif /// DBG
   
   NT_ASSERT(pWDFDriver);

   NTSTATUS              status         = STATUS_SUCCESS;
   PWDFDEVICE_INIT       pWDFDeviceInit = 0;
   WDF_OBJECT_ATTRIBUTES attributes     = {0};

   WDF_OBJECT_ATTRIBUTES_INIT(&attributes);

   attributes.EvtCleanupCallback = EventCleanupDeviceObject;

   pWDFDeviceInit = WdfControlDeviceInitAllocate(*pWDFDriver,
                                                 &SDDL_DEVOBJ_KERNEL_ONLY);
   if(pWDFDeviceInit == 0)
   {
      status = STATUS_UNSUCCESSFUL;

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PrvDriverDeviceAdd : WdfControlDeviceInitAllocate() [status: %#x]\n",
                 status);

      HLPR_BAIL;      
   }

   WdfDeviceInitSetDeviceType(pWDFDeviceInit,
                              FILE_DEVICE_NETWORK);

   status = WdfDeviceCreate(&pWDFDeviceInit,
                            &attributes,
                            &g_WDFDevice);
   if(status != STATUS_SUCCESS)
   {
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! PrvDriverDeviceAdd : WdfDeviceCreate() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   g_pWDMDevice = WdfDeviceWdmGetDeviceObject(g_WDFDevice);
   HLPR_BAIL_ON_NULL_POINTER_WITH_STATUS(g_pWDMDevice,
                                         status);

   status = RegisterPowerStateChangeCallback(&g_deviceExtension);
   HLPR_BAIL_ON_FAILURE(status);

#pragma warning(push)
#pragma warning(disable: 6388) /// g_pNDISPoolData will be 0

   status = KrnlHlprNDISPoolDataCreate(&g_pNDISPoolData);
   HLPR_BAIL_ON_FAILURE(status);

#pragma warning(pop)

   PrvFwpmBfeStateSubscribeChanges();

   status = PrvWFPSamplerDeviceDataPopulate(&g_WFPSamplerDeviceData);
   HLPR_BAIL_ON_FAILURE(status);

   status = PrvWFPSamplerFwpObjectsAddGlobal(&g_WFPSamplerDeviceData);
   HLPR_BAIL_ON_FAILURE(status);

   PrvBasicStreamInjectionSerializationListInitialization();

   WdfControlFinishInitializing(g_WDFDevice);

   HLPR_BAIL_LABEL:

   if(status != STATUS_SUCCESS)
   {
      if(pWDFDeviceInit)
          WdfDeviceInitFree(pWDFDeviceInit);
   }

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " <--- PrvDriverDeviceAdd() [status: %#x]\n",
              status);

#endif /// DBG
   
   return status;
}

/**
 @framework_function="DriverEntry"
 
   Purpose:  Entry point for the driver.                                                        <br>
                                                                                                <br>
   Notes:                                                                                       <br>
                                                                                                <br>
   MSDN_Ref: HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF540807.aspx             <br>
             HTTP://MSDN.Microsoft.com/En-US/Library/Windows/Hardware/FF547175.aspx             <br>
*/
_IRQL_requires_same_
_Function_class_(DRIVER_INITIALIZE)
NTSTATUS DriverEntry(_In_ PDRIVER_OBJECT pDriverObject,
                     _In_ PUNICODE_STRING pRegistryPath)
{
   WPP_INIT_TRACING(pDriverObject,
                    pRegistryPath);

#if DBG
   
   DbgPrintEx(DPFLTR_IHVNETWORK_ID,
              DPFLTR_INFO_LEVEL,
              " ---> DriverEntry()\n");

#endif /// DBG
   
   NT_ASSERT(pDriverObject);
   NT_ASSERT(pRegistryPath);

   NTSTATUS              status = STATUS_SUCCESS;
   WDF_DRIVER_CONFIG     driverConfig;
   WDF_OBJECT_ATTRIBUTES attributes;

   WDF_DRIVER_CONFIG_INIT(&driverConfig,
                          WDF_NO_EVENT_CALLBACK);

   ExInitializeDriverRuntime(DrvRtPoolNxOptIn);

   driverConfig.DriverInitFlags |= WdfDriverInitNonPnpDriver;
   driverConfig.EvtDriverUnload  = EventDriverUnload;

   WDF_OBJECT_ATTRIBUTES_INIT(&attributes);

   attributes.EvtCleanupCallback = EventCleanupDriverObject;

   status = WdfDriverCreate(pDriverObject,
                            pRegistryPath,
                            &attributes,
                            &driverConfig,
                            &g_WDFDriver);
   if(status != STATUS_SUCCESS)
   {
      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_ERROR_LEVEL,
                 " !!!! DriverEntry : WdfDriverCreate() [status: %#x]\n",
                 status);

      HLPR_BAIL;
   }

   status = PrvDriverDeviceAdd(&g_WDFDriver);
   HLPR_BAIL_ON_FAILURE(status);

   HLPR_BAIL_LABEL:

#if DBG

      DbgPrintEx(DPFLTR_IHVNETWORK_ID,
                 DPFLTR_INFO_LEVEL,
                 " <--- DriverEntry() [status: %#x]\n",
                 status);
   
#endif /// DBG

   if(status != STATUS_SUCCESS)
      WPP_CLEANUP(pDriverObject);

   return status;
}
