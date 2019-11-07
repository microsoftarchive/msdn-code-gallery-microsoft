/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hw_main.c

Abstract:
    Implements initialization/PNP routines for the HW layer
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/
#include "precomp.h"
#include "hw_main.h"
#include "hw_isr.h"
#include "hw_oids.h"
#include "hw_phy.h"
#include "hw_mac.h"
#include "hw_crypto.h"

#if DOT11_TRACE_ENABLED
#include "hw_main.tmh"
#endif



MP_REG_ENTRY HWRegTable[] = {
    {
        NDIS_STRING_CONST("*ReceiveBuffers"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, NumRXBuffers),
        sizeof(ULONG),
        HW11_DEF_RX_MSDUS,  
        HW11_MIN_RX_MSDUS,  
        HW11_MAX_RX_MSDUS
    },
    {
        NDIS_STRING_CONST("*TransmitBuffers"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, NumTXBuffers),
        sizeof(ULONG),
        HW11_DEF_TX_MSDUS,  
        HW11_MIN_TX_MSDUS,  
        HW11_MAX_TX_MSDUS
    },
    {
        NDIS_STRING_CONST("RadioOff"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,PhyState),
        FIELD_OFFSET(NIC_PHY_STATE, SoftwareRadioOff),
        sizeof(BOOLEAN),
        0,                              // Default = On
        0,  
        1
    },
    {
        NDIS_STRING_CONST("MaximumTxRate"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, MaximumTxRate),
        sizeof(ULONG),
        48,
        2,                              // Minimum = 1 Mbps
        HW11_MAX_DATA_RATE
    },
    {
        NDIS_STRING_CONST("MultipleMacAddressEnabled"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, MultipleMacAddressEnabled),
        sizeof(ULONG),
        1,
        0,
        1
    },
    {
        NDIS_STRING_CONST("MaxRxPacketsToProcess"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, MaxRxPacketsToProcess),
        sizeof(ULONG),
        NUM_RECV_PACKETS_TO_PROCESS_DEFAULT,
        NUM_RECV_PACKETS_TO_PROCESS_MIN,
        NUM_RECV_PACKETS_TO_PROCESS_MAX
    },
    {
        NDIS_STRING_CONST("MinPacketsSentForTxRateUpdate"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, MinPacketsSentForTxRateUpdate),
        sizeof(ULONG),
        30,
        10,
        500
    },

    {
        NDIS_STRING_CONST("TxFailureThresholdForRateFallback"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, TxFailureThresholdForRateFallback),
        sizeof(ULONG),
        125,
        50,
        200
     },

    {
        NDIS_STRING_CONST("TxFailureThresholdForRateIncrease"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, TxFailureThresholdForRateIncrease),
        sizeof(ULONG),
        35,
        0,
        50
    },

    {
        NDIS_STRING_CONST("TxFailureThresholdForRoam"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, TxFailureThresholdForRoam),
        sizeof(ULONG),
        350,
        300,
        600
    },

    {
        NDIS_STRING_CONST("TxDataRateFallbackSkipLevel"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, TxDataRateFallbackSkipLevel),
        sizeof(ULONG),
        3,
        1,
        6
    },

    {
        NDIS_STRING_CONST("RTSThreshold"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, RTSThreshold),
        sizeof(ULONG),
        2347,
        0,
        2347
    },

    {
        NDIS_STRING_CONST("FragmentationThreshold"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, FragmentationThreshold),
        sizeof(ULONG),
        2346,
        256,
        2346
    },

    {
        NDIS_STRING_CONST("BeaconPeriod"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, BeaconPeriod),
        sizeof(ULONG),
        100,
        100,
        500
    },

    {
        NDIS_STRING_CONST("NumRXBuffersUpperLimit"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, NumRXBuffersUpperLimit),
        sizeof(ULONG),
        HW11_MAX_RX_MSDUS,
        HW11_MIN_RX_MSDUS,
        HW11_MAX_RX_MSDUS
    },

    {
        NDIS_STRING_CONST("NumRXBuffersLowerBase"), 
        NdisParameterInteger,
        FIELD_OFFSET(HW,RegInfo),
        FIELD_OFFSET(NIC_REG_INFO, NumRXBuffersLowerBase),
        sizeof(ULONG),
        HW11_MIN_RX_MSDUS,
        HW11_MIN_RX_MSDUS,
        HW11_MAX_RX_MSDUS
    }
    
};

#define HW_NUM_REG_PARAMS       (((ULONG)sizeof (HWRegTable)) / ((ULONG)sizeof(MP_REG_ENTRY)))


NDIS_STATUS
Hw11Allocate(
    _In_  NDIS_HANDLE             MiniportAdapterHandle,
    _Outptr_result_maybenull_ PHW*          Hw,
    _In_  PADAPTER                Adapter
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PHW                         newHw = NULL;
    ULONG                       size;
    NDIS_TIMER_CHARACTERISTICS  timerChar;               

    *Hw = NULL;
    do
    {
        // Allocate a HW data structure
        MP_ALLOCATE_MEMORY(MiniportAdapterHandle, &newHw, sizeof(HW), HW_MEMORY_TAG);
        if (newHw == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate %d bytes for HW\n",
                                 sizeof(HW)));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // Clear everything
        NdisZeroMemory(newHw, sizeof(HW));

        // We start in the PAUSED state
        HW_SET_ADAPTER_STATUS(newHw, HW_ADAPTER_PAUSED);
        newHw->InterruptDisableCount = 1;       // Since we are paused, we want the interrupts to be disabled
#if DBG
        NdisInterlockedIncrement(&newHw->Tracking_InterruptDisable[HW_ISR_TRACKING_PAUSE]);
#endif
        
        // Allocate memory for fields inside the HW structure
        size = sizeof(DOT11_REG_DOMAINS_SUPPORT_VALUE) + 
               (HW_MAX_NUM_DOT11_REG_DOMAINS_VALUE - 1) * sizeof(DOT11_REG_DOMAIN_VALUE);
        MP_ALLOCATE_MEMORY(MiniportAdapterHandle,
                           &(newHw->PhyState.RegDomainsSupportValue),
                           size,
                           HW_MEMORY_TAG);
        if (newHw->PhyState.RegDomainsSupportValue == NULL)
        {
            MpTrace(COMP_INIT_PNP,
                    DBG_SERIOUS,
                    ("Failed to allocate memory for RegDomainsSupportValue\n"));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        NdisZeroMemory(newHw->PhyState.RegDomainsSupportValue, size);

        size = sizeof(DOT11_DIVERSITY_SELECTION_RX_LIST) +
              (HW_MAX_NUM_DIVERSITY_SELECTION_RX_LIST - 1) * sizeof(DOT11_DIVERSITY_SELECTION_RX);
        MP_ALLOCATE_MEMORY(MiniportAdapterHandle,
                           &(newHw->PhyState.DiversitySelectionRxList),
                           size, 
                           HW_MEMORY_TAG);
        if (newHw->PhyState.DiversitySelectionRxList == NULL) 
        {
            MpTrace(COMP_INIT_PNP,
                    DBG_SERIOUS,
                    ("Failed to allocate memory for DiversitySelectionRxList\n"));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }
        NdisZeroMemory(newHw->PhyState.DiversitySelectionRxList, size);        

        NdisZeroMemory(&timerChar, sizeof(NDIS_TIMER_CHARACTERISTICS));
        
        // Allocate the power save wake timer
        timerChar.Header.Type = NDIS_OBJECT_TYPE_TIMER_CHARACTERISTICS;
        timerChar.Header.Revision = NDIS_TIMER_CHARACTERISTICS_REVISION_1;
        timerChar.Header.Size = sizeof(NDIS_TIMER_CHARACTERISTICS);
        timerChar.AllocationTag = HW_MEMORY_TAG;
        
        timerChar.TimerFunction = HwAwakeTimer;
        timerChar.FunctionContext = newHw;

        ndisStatus = NdisAllocateTimerObject(
                        MiniportAdapterHandle,
                        &timerChar,
                        &newHw->PhyState.Timer_Awake
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate power save awake timer\n"));
            break;
        }

        // Allocate the power save sleep timer
        timerChar.TimerFunction = HwDozeTimer;
        timerChar.FunctionContext = newHw;

        ndisStatus = NdisAllocateTimerObject(
                        MiniportAdapterHandle,
                        &timerChar,
                        &newHw->PhyState.Timer_Doze
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate power save doze timer\n"));
            break;
        }
        
        // Allocate the scan timer
        timerChar.TimerFunction = HwScanTimer;
        timerChar.FunctionContext = newHw;

        ndisStatus = NdisAllocateTimerObject(
                        MiniportAdapterHandle,
                        &timerChar,
                        &newHw->ScanContext.Timer_Scan
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate scan timer\n"));
            break;
        }

        newHw->PhyState.PhyProgramWorkItem = NdisAllocateIoWorkItem(MiniportAdapterHandle);
        if(newHw->PhyState.PhyProgramWorkItem == NULL)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to allocate channel switch work item\n"));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        // The hardware lock
        NdisAllocateSpinLock(&newHw->Lock);

        ndisStatus = HwInitializeCrypto(newHw);
        if (ndisStatus != NDIS_STATUS_SUCCESS) 
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("HwInitializeCrypto failed. Status = 0x%08x\n", ndisStatus));
            break;
        }

        // Save the Adapter pointer in the HW
        newHw->Adapter = Adapter;
        newHw->MiniportAdapterHandle = MiniportAdapterHandle;

        // Return the newly created structure to the caller
        *Hw = newHw;

    } while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        if (newHw != NULL)
        {
            Hw11Free(newHw);
        }
    }

    return ndisStatus;
}


VOID
Hw11Free(
    _In_  PHW                     Hw
    )
{
    HwTerminateCrypto(Hw);

    NdisFreeSpinLock(&Hw->Lock);

    if (Hw->PhyState.PhyProgramWorkItem != NULL)
    {
        NdisFreeIoWorkItem(Hw->PhyState.PhyProgramWorkItem);
        Hw->PhyState.PhyProgramWorkItem = NULL;
    }

    if (Hw->ScanContext.Timer_Scan)
    {
        NdisFreeTimerObject(Hw->ScanContext.Timer_Scan);
        Hw->ScanContext.Timer_Scan = NULL;
    }

    if (Hw->PhyState.Timer_Doze)
    {
        NdisFreeTimerObject(Hw->PhyState.Timer_Doze);
        Hw->PhyState.Timer_Doze = NULL;
    }
    
    if (Hw->PhyState.Timer_Awake)
    {
        NdisFreeTimerObject(Hw->PhyState.Timer_Awake);
        Hw->PhyState.Timer_Awake = NULL;
    }
    
    if (Hw->PhyState.RegDomainsSupportValue)
    {
        MP_FREE_MEMORY(Hw->PhyState.RegDomainsSupportValue);
        Hw->PhyState.RegDomainsSupportValue = NULL;
    }

    if (Hw->PhyState.DiversitySelectionRxList)
    {
        MP_FREE_MEMORY(Hw->PhyState.DiversitySelectionRxList);
        Hw->PhyState.DiversitySelectionRxList = NULL;
    }
    if (Hw->Hal != NULL)
    {
        HalFreeNic(Hw->Hal);
        Hw->Hal = NULL;
    }
    
    MP_FREE_MEMORY(Hw);
}


VOID
Hw11ReadRegistryConfiguration(
    _In_        PHW                     Hw,
    _In_opt_    NDIS_HANDLE             ConfigurationHandle
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    PUCHAR                      NetworkAddress;
    UINT                        Length;

    // TODO: These should be read from the registry
    Hw->RegInfo.RTSThreshold = 2347;
    Hw->RegInfo.FragmentationThreshold = 2346;
    Hw->RegInfo.BeaconPeriod = 100;

    Hw->RegInfo.NumRXBuffersUpperLimit = HW11_MAX_RX_MSDUS;
    Hw->RegInfo.NumRXBuffersLowerBase = HW11_MIN_RX_MSDUS;
    
    //
    // read all the non-hardware specific registry values 
    //
    MpReadRegistry((PVOID)Hw, ConfigurationHandle, HWRegTable, HW_NUM_REG_PARAMS);

    //
    // Read NetworkAddress registry value. Use it as the current address if any.
    //
    if (ConfigurationHandle != NULL) 
    {
        NdisReadNetworkAddress(&ndisStatus,
                               (void **)&NetworkAddress,
                               &Length,
                               ConfigurationHandle);

        //
        // If there is a valid NetworkAddress override in registry,use it 
        //
        if ((ndisStatus == NDIS_STATUS_SUCCESS) && (Length == sizeof(DOT11_MAC_ADDRESS))) 
        {
            // Dont use multicast/broadcast or 00* addresses
            if (!ETH_IS_MULTICAST(NetworkAddress) && !ETH_IS_BROADCAST(NetworkAddress)) 
            {
                if ((NetworkAddress[0] == 0x00) &&
                    (NetworkAddress[1] == 0x00) &&
                    (NetworkAddress[2] == 0x00) &&
                    (NetworkAddress[3] == 0x00) &&
                    (NetworkAddress[4] == 0x00) &&
                    (NetworkAddress[5] == 0x00)) 
                {   
                    // Network addr = 00 00 00 00 00 00
                    Hw->RegInfo.AddressOverrideEnabled = FALSE;
                }
                else if ((NetworkAddress[0] & 0x02) != 0x02)
                {
                    // Not a locally administered address
                    Hw->RegInfo.AddressOverrideEnabled = FALSE;
                }
                else
                {
                    ETH_COPY_NETWORK_ADDRESS(Hw->RegInfo.OverrideAddress,NetworkAddress);
                    Hw->RegInfo.AddressOverrideEnabled = TRUE;
                }
            }
        }

        ndisStatus = NDIS_STATUS_SUCCESS;

    }

}

#ifdef EXPORT_DRIVER_HAL

// If the HAL is an export driver, then we would invoke this API from the export driver
DECLSPEC_IMPORT NDIS_STATUS HalDriverCreateWLANHal(
    NDIS_HANDLE miniportAdapterHandle, 
    USHORT vendorId, 
    USHORT deviceId, 
    USHORT revisionId, 
    PWLAN_HAL *hal
    );

#endif



NDIS_STATUS
Hw11FindNic(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    )
{
    ULONG                       size;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    UCHAR                       buffer[HW11_PCI_CONFIG_BUFFER_LENGTH];
    PPCI_COMMON_CONFIG          pciConfig = (PPCI_COMMON_CONFIG) buffer;

    *ErrorCode = NDIS_STATUS_SUCCESS;
    *ErrorValue = 0;

    //
    // Make sure adapter is present on the bus.
    // If present,verify the PCI configuration values.
    //

    do
    {
        // Load the PCI config information into our local buffer
        size = NdisMGetBusData(Hw->MiniportAdapterHandle,
                    PCI_WHICHSPACE_CONFIG,
                    FIELD_OFFSET(PCI_COMMON_CONFIG,VendorID),
                    pciConfig,
                    HW11_PCI_CONFIG_BUFFER_LENGTH
                    );
        
        if (size != HW11_PCI_CONFIG_BUFFER_LENGTH) 
        {
            MpTrace(COMP_INIT_PNP,
                    DBG_SERIOUS,
                    ("NdisReadPciSlotInformation failed. Number of bytes of PCI config info returned is %d\n", size));

            *ErrorCode = NDIS_ERROR_CODE_ADAPTER_NOT_FOUND;
            *ErrorValue = ERRLOG_READ_PCI_SLOT_FAILED;
            ndisStatus = NDIS_STATUS_ADAPTER_NOT_FOUND;
            break;
        }

        //
        // We have read the hardware ID, etc. Create the appropriate HAL
        // object that corresponds to our hardware. This allocates the HAL
        // structure and populates the HAL function pointers
        //
#ifdef EXPORT_DRIVER_HAL
        // Invoke the export driver
        ndisStatus = HalDriverCreateWLANHal(Hw->MiniportAdapterHandle, 
                        pciConfig->VendorID, 
                        pciConfig->DeviceID, 
                        pciConfig->RevisionID, 
                        &Hw->Hal
                        );

#else
        // Invoke the HAL library
        ndisStatus = HalCreateWLANHal(Hw->MiniportAdapterHandle, 
                        pciConfig->VendorID, 
                        pciConfig->DeviceID, 
                        pciConfig->RevisionID, 
                        &Hw->Hal
                        );
#endif
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP,
                    DBG_SERIOUS,
                    ("HalCreateWLANHal failed. Status = 0x%08x\n", ndisStatus));

            *ErrorCode = NDIS_ERROR_CODE_ADAPTER_NOT_FOUND;
            *ErrorValue = ERRLOG_VENDOR_DEVICE_MISMATCH;

            break;
        }

        //
        // Let the HAL layer read the registry. This may depend on the NIC
        // and hence we do this here after we have found the NIC and create the HAL
        // and not during the first Hw11ReadRegistryConfiguration call
        //
        ndisStatus = HalReadRegistryConfiguration(Hw->Hal);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            // Read registry should only fail for test purposes. If registry data is invalid the 
            // driver should load the defaults and NOT fail driver initialize        
            MpTrace(COMP_INIT_PNP,
                    DBG_SERIOUS,
                    ("HalReadRegistryConfiguration failed. Status = 0x%08x\n", ndisStatus));

            MPASSERT(ndisStatus == NDIS_STATUS_RESOURCES);
        }

        //
        // Allow the HAL to look at PCI config information. This may also modify the
        // HAL
        //
        ndisStatus = HalParsePciConfiguration(Hw->Hal, buffer, size);    
        if (ndisStatus != NDIS_STATUS_SUCCESS) 
        {
            MpTrace(COMP_INIT_PNP, 
                    DBG_SERIOUS, 
                    ("HalParsePciConfiguration failed. Status = 0x%08x\n", ndisStatus));

            *ErrorCode = NDIS_ERROR_CODE_UNSUPPORTED_CONFIGURATION;
            *ErrorValue = ERRLOG_INVALID_PCI_CONFIGURATION;

            break;                
        }

    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // The only thing done so far is create the WLAN hal
        if (Hw->Hal != NULL)
        {
            HalFreeNic(Hw->Hal);
            Hw->Hal = NULL;
        }

    }
    
    return ndisStatus;
}

NDIS_STATUS
Hw11DiscoverNicResources(
    _In_  PHW                     Hw,
    _In_  PNDIS_RESOURCE_LIST     ResList,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    )
{
    ULONG                       index;
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     resPort = FALSE, resInterrupt = FALSE, resMemory = FALSE;
    PCM_PARTIAL_RESOURCE_DESCRIPTOR resDesc;

    *ErrorCode = NDIS_STATUS_SUCCESS;
    *ErrorValue = 0;
    
    do
    {
        for(index=0; index < ResList->Count; index++)
        {
            resDesc = &ResList->PartialDescriptors[index];

            //
            // Notify Hw11 about the resources found
            //
            ndisStatus = HalAddAdapterResource(Hw->Hal, resDesc);
            
            //
            // If the resource was successfully accepted, remember its type
            //
            if (ndisStatus == NDIS_STATUS_SUCCESS)
            {
                switch(resDesc->Type)
                {
                    case CmResourceTypePort:
                        resPort = TRUE;
                        break;

                    case CmResourceTypeInterrupt:
                        resInterrupt = TRUE;
                        break;

                    case CmResourceTypeMemory:
                        resMemory = TRUE;
                        break;
                }
            }
            else if(ndisStatus != NDIS_STATUS_NOT_ACCEPTED)
            {
                //
                // This is an unrecoverable error. The Hw11 probably came across a resource
                // that caused fatal error while being mapped or used. Details about the
                // the failure should be in the event log. Bail out.
                //
                MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to add adapter resource. Status = 0x%08x\n", ndisStatus));
                break;
            }
        }

        //
        // Make sure that the hardware has found its port, interrupt and memory resources.
        // If any of them is missing, fail initialization
        //
        if(!resInterrupt || !resMemory)
        {
            ndisStatus = NDIS_STATUS_RESOURCE_CONFLICT;
            *ErrorCode = NDIS_ERROR_CODE_RESOURCE_CONFLICT;
            
            if(!resPort)
                *ErrorValue = ERRLOG_NO_IO_RESOURCE;
            else if(!resInterrupt)
                *ErrorValue = ERRLOG_NO_INTERRUPT_RESOURCE;
            else 
                *ErrorValue = ERRLOG_NO_MEMORY_RESOURCE;            
            break;
        }
    } while(FALSE);

    return ndisStatus;


}

NDIS_STATUS
Hw11ReadNicInformation(
    _In_  PHW                     Hw
    )
{
    UNREFERENCED_PARAMETER(Hw);

    //
    // Dont need anything    
    //
    
    return NDIS_STATUS_SUCCESS;
}


NDIS_STATUS
Hw11Initialize(
    _In_  PHW                     Hw,
    _In_  PHVL                    Hvl,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    Hw->Hvl = Hvl;

    *ErrorCode = NDIS_STATUS_SUCCESS;
    *ErrorValue = 0;

    do
    {
        //
        // Initialize the HAL layer
        //
        ndisStatus = HalInitialize(Hw->Hal);
        if (ndisStatus != NDIS_STATUS_SUCCESS) 
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("HalInitialize failed. Status = 0x%08x\n", ndisStatus));
            break;
        }
        
        //
        // Read the HW capabilities
        //
        HalGetPowerSaveCapabilities(Hw->Hal, &Hw->MacState.HalPowerSaveCapability);

        //
        // Reset our state to its initial value
        //        
        HwResetSoftwareMacState(Hw);     // Resets the software data
        HwResetSoftwarePhyState(Hw);     // Resets the software data
        NdisZeroMemory(&Hw->Stats, sizeof(NIC_STATISTICS));

        //
        // Clear any stale state from the hardware
        //
        ndisStatus = HwClearNicState(Hw);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("HwClearNicState failed. Status = 0x%08x\n", ndisStatus));
            break;
        }

        //
        // Program our new state on the hardware
        //
        ndisStatus = HwSetNicState(Hw);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("HwSetNicState failed. Status = 0x%08x\n", ndisStatus));
            break;
        }

        //
        // Initialize the scatter gather DMA with NDIS for send. This also allocates stuff
        // for receive shared memory allocation
        //
        ndisStatus = HwInitializeScatterGatherDma(Hw, ErrorCode, ErrorValue);
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            break;
        }


    } while (FALSE);


    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {       
        //
        // Deregister the DMA from NDIS
        //
        if (Hw->MiniportDmaHandle != NULL)
        {
            NdisMDeregisterScatterGatherDma(Hw->MiniportDmaHandle);
        }
    }
    
    return ndisStatus;
}


VOID
Hw11Terminate(
    _In_  PHW                     Hw
    )
{
    //
    // Deregister the DMA from NDIS
    //
    if (Hw->MiniportDmaHandle != NULL)
    {
        NdisMDeregisterScatterGatherDma(Hw->MiniportDmaHandle);
    }

    //
    // Cancel all other timers (these are all stopped already)
    //
    MPASSERT(NdisCancelTimerObject(Hw->ScanContext.Timer_Scan) == FALSE);
    MPASSERT(NdisCancelTimerObject(Hw->PhyState.Timer_Doze) == FALSE);
    MPASSERT(NdisCancelTimerObject(Hw->PhyState.Timer_Awake) == FALSE);

}

VOID
Hw11Shutdown(
    _In_  PHW                     Hw,
    _In_  NDIS_SHUTDOWN_ACTION    ShutdownAction
    )
{
    UNREFERENCED_PARAMETER(ShutdownAction);

    //
    // No I/O if device has been surprise removed
    //
    if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
    {
        //
        // Disable Interrupts only if adapter has not been removed
        //
//        MpTrace(COMP_TESTING, DBG_SERIOUS, ("Hw11Shutdown \n"));
        HwDisableInterrupt(Hw, HW_ISR_TRACKING_SHUTDOWN);

        //
        // Issue a halt to the HAL. HAL should go into a known state
        // and shut off power to the antenna. If surprise removal has
        // occurred, we will not do this.
        //
        HalHaltNic(Hw->Hal);
    }
}

VOID
Hw11DevicePnPEvent(
    _In_  PHW                     Hw,
    _In_  PNET_DEVICE_PNP_EVENT   NetDevicePnPEvent
    )
{
    if (NetDevicePnPEvent->DevicePnPEvent == NdisDevicePnPEventSurpriseRemoved)
    {
        //
        // If hardware has been surprise removed,remember that.
        // We have to make sure we do not try to do any I/O on ports
        // if device has been surprise removed.
        //
        HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
        HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_SURPRISE_REMOVED);
        HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

        // Wait for interrupt, etc functions to finish
        HW_WAIT_FOR_ACTIVE_OPERATIONS_TO_FINISH(Hw);
        
        // Wait for active sends to be finish
        HW_WAIT_FOR_ACTIVE_SENDS_TO_FINISH(Hw);

        //
        // We must stop advertising sending beacons and probes.
        // as the hardware is no longer present
        //
        Hw->MacState.BeaconEnabled = FALSE;
        Hw->MacState.BSSStarted = FALSE;
        
        //
        // Clear the send queue. Since the hardware is gone, we would not get
        // any more send completes
        //
        HwResetSendEngine(Hw, FALSE);

        //
        // Any place where we are reading registers and making major
        // decisions we should consider protecting against FFFF for surprise 
        // removal case
        //

        // TODO: Notify the HAL about surprise remove
    }
}


NDIS_STATUS
Hw11Start(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    BOOLEAN                     interruptRegistered = FALSE, hardwareStarted = FALSE;
    
    //
    // Disable interrupts
    // We should disable interrupts before the are registered.
    // They will be enabled right at the end of Initialize
    //
    HwDisableInterrupt(Hw, HW_ISR_TRACKING_HWSTART);

    do
    {
        //
        // Register interrupt with NDIS
        //
        ndisStatus = HwRegisterInterrupt(
                        Hw,
                        ErrorCode, 
                        ErrorValue
                        );
        if (ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to register interrupt with NDIS\n"));
            break;
        }
        interruptRegistered = TRUE;
        
        Hw->PhyState.Debug_SoftwareRadioOff = Hw->PhyState.SoftwareRadioOff;
        if (Hw->PhyState.SoftwareRadioOff)
        {
            HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_RADIO_OFF);            
            HalSetRFPowerState(Hw->Hal, RF_SHUT_DOWN);
            MpTrace(COMP_INIT_PNP, DBG_NORMAL, ("Radio is switched off. Not starting hardware/enabling interrupts\n"));
            break;
        }
        else
        {
            HalSetRFPowerState(Hw->Hal, RF_ON);
        }

        //
        // Start the HAL. If anything fails after this point,
        // we must issue a Halt to the HAL before returning
        // from initialize
        //
        ndisStatus = HalStart(Hw->Hal, FALSE);
        if(ndisStatus != NDIS_STATUS_SUCCESS)
        {
            MpTrace(COMP_INIT_PNP, DBG_SERIOUS, ("Failed to start HAL successfully.\n"));
            break;
        }
        hardwareStarted = TRUE;
        
        // Enable the interrupts on the hardware
        HwEnableInterrupt(Hw, HW_ISR_TRACKING_HWSTART);
                
    }while (FALSE);

    if (ndisStatus != NDIS_STATUS_SUCCESS)
    {
        // Disable interrupts and deregister them first
        HwDisableInterrupt(Hw, HW_ISR_TRACKING_HWSTART);
        
        if (interruptRegistered)
            HwDeregisterInterrupt(Hw);

        if (hardwareStarted)
        {
            HalStop(Hw->Hal);
            HalHaltNic(Hw->Hal);
        }
    }

    return ndisStatus;
}

// First disable interrupts and then deregister interrupts
VOID
Hw11Stop(
    _In_  PHW                     Hw,
    _In_  NDIS_HALT_ACTION        HaltAction
    )
{
    UNREFERENCED_PARAMETER(HaltAction);
    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_HALTING);
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

    //
    // Deregister interrupts. We must disable them before deregistering interrupts
    //
//    MpTrace(COMP_TESTING, DBG_SERIOUS, ("Hw11Stop \n"));    
    HwDisableInterrupt(Hw, HW_ISR_TRACKING_HWSTOP);
    HwDeregisterInterrupt(Hw);

    // There must be no pending operations at this time
    MPASSERT(Hw->AsyncFuncRef == 0);
    
    //
    // Ensure that we have stop beaconing
    //
    Hw->MacState.BeaconEnabled = FALSE;
    Hw->MacState.BSSStarted = FALSE;

    //
    // Flush all the MSDU in the reassembly line
    //
    HwFlushMSDUReassemblyLine(Hw);

    // Stop everything in the H/W (We may restart things for context switches, etc)
    HalStop(Hw->Hal);
}

VOID
Hw11Halt(
    _In_  PHW                     Hw
    )
{
    if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
    {
        // Turn off everything on the hardware
        HalHaltNic(Hw->Hal);
    }
}    


NDIS_STATUS
Hw11SelfTest(
    _In_  PHW                     Hw
    )
{
    UNREFERENCED_PARAMETER(Hw);

    return NDIS_STATUS_SUCCESS;
}

BOOLEAN
Hw11CheckForHang(
    _In_  PHW                     Hw
    )
{
    BOOLEAN                     needReset = FALSE;

    do
    {
        //
        // Check if sends are hung
        //
        needReset = HwCheckForSendHang(Hw);

#if 0
        //
        // Sample the usage of Rx MSDU list. Will be used during MpReturnPackets to determine
        // if we need to shrink the Rx MSDU list.
        //
        if (Hw->RxInfo.UnusedMPDUListSampleTicks < HW_RX_MSDU_LIST_SAMPLING_PERIOD)
        {
            Hw->RxInfo.UnusedMPDUListSampleTicks++;
            Hw->RxInfo.UnusedMPDUListSampleCount += Hw->RxInfo->NumMPDUUnused;
        }
        else
        {
            MpTrace(COMP_MISC, DBG_LOUD, ("Percentage of Under Utilization = %d\n", 
                      ((Hw->RxInfo.UnusedMPDUListSampleCount * 100)  / (Hw->RxInfo.UnusedMPDUListSampleTicks * Hw->RxInfo->NumMPDUAllocated))));
            Hw->RxInfo.UnusedMPDUListSampleTicks = 0;
            Hw->RxInfo.UnusedMPDUListSampleCount = 0;
        }
#endif
    } while (FALSE);

    return needReset;
}

//
// Reset Step 1 - Cleanup any "pending" operations
//
VOID
Hw11NdisResetStep1(
    _In_  PHW                     Hw
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;

    // if we are being reset because our sends are hung, we do not want to assert. however
    // if sends are not the cause, we do want to assert.
    if (!Hw11ArePktsPending(Hw))
    {
        MPASSERT(FALSE);
    }
        
    //
    // Set state as in reset
    //
    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_IN_RESET);
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

    //
    // Now cleanup everything
    //

    // Cancel scan (if it is running)
    HwCancelScan(Hw);
    
    // Wait for pending operations in the hardware to finish
    HW_WAIT_FOR_ACTIVE_OPERATIONS_TO_FINISH(Hw);

    // Wait for active sends to be finish
    HW_WAIT_FOR_ACTIVE_SENDS_TO_FINISH(Hw);

    // Disable interrupts
    HwDisableInterrupt(Hw, HW_ISR_TRACKING_NDIS_RESET);

    ndisStatus = HalResetStart(Hw->Hal);
    MPASSERT(ndisStatus == NDIS_STATUS_SUCCESS);

    HalStop(Hw->Hal);    

    HwFlushSendEngine(Hw, FALSE);
    
    // Dont wait for pending receives here. They may be stuck in protocols on
    // sends & the sends may be queued in the ports. That would cause a deadlock
}

//
// Reset Step 2 - Reset the hardware state
//
VOID
Hw11NdisResetStep2(
    _In_  PHW                     Hw
    )
{
    // Reset the send and receive engine 
    // (first waiting to ensure that indicated receives have returned)
    HwWaitForPendingReceives(Hw, NULL);
    HwResetSendEngine(Hw, FALSE);
    HwResetReceiveEngine(Hw, FALSE);
    // Remove old keys, etc
    HwClearNicState(Hw);
    
    // Reset our MAC & PHY state
    HwResetSoftwareMacState(Hw);
    HwResetSoftwarePhyState(Hw);
}

//
// Reset Step 3 - Restart the hardware
//
NDIS_STATUS
Hw11NdisResetStep3(
    _In_  PHW                     Hw,
    _Out_ PBOOLEAN                AddressingReset
    )
{
    *AddressingReset = FALSE;

    HalStart(Hw->Hal, TRUE);
    
    // Push the new state on the hardware
    HwSetNicState(Hw);

    HalResetEnd(Hw->Hal);
    
    // Renable the interrupts
    HwEnableInterrupt(Hw, HW_ISR_TRACKING_NDIS_RESET);
    
    HW_CLEAR_ADAPTER_STATUS(Hw, HW_ADAPTER_IN_RESET);

    // TODO: Should we return NDIS_STATUS_HARD_ERRORS if we are hung
    
    return NDIS_STATUS_SUCCESS;
}

_IRQL_requires_(PASSIVE_LEVEL)
NDIS_STATUS
HwResetHAL(
    _In_  PHW                     Hw,
    _In_  PHW_HAL_RESET_PARAMETERS ResetParams,
    _In_  BOOLEAN                 DispatchLevel
    )
{
    UNREFERENCED_PARAMETER(ResetParams);
    UNREFERENCED_PARAMETER(DispatchLevel);

    MPASSERT(!DispatchLevel);

    // Since we wait, we cannot be called at dispatch    
    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_HAL_IN_RESET);
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

    // Wait for active send threads to finish. We dont wait
    // for anything else on an HAL reset since some of those
    // operations themselves may be causing the reset (Eg. channel
    // switch of a scan)
    HW_WAIT_FOR_ACTIVE_SENDS_TO_FINISH(Hw);
    HwDisableInterrupt(Hw, HW_ISR_TRACKING_HAL_RESET);

    if (ResetParams->FullReset)
    {
        // Perform a full reset of the HW
        HalResetStart(Hw->Hal);

        HalStop(Hw->Hal);    

        // Reset the send and receive engine
        HwWaitForPendingReceives(Hw, NULL);
        HwResetSendEngine(Hw, FALSE);
        HwResetReceiveEngine(Hw, FALSE);
        // Remove old keys, etc
        HwClearNicState(Hw);
        
        // Reset our MAC & PHY state
        HwResetSoftwareMacState(Hw);
        HwResetSoftwarePhyState(Hw);

        HalStart(Hw->Hal, TRUE);
        
        // Push the new state on the hardware
        HwSetNicState(Hw);

        HalResetEnd(Hw->Hal);
    }
    else
    {
        // TODO: Currently we are overloading the HalSwitchChannel API for doing a HalReset
        HalSwitchChannel(Hw->Hal, 
            Hw->PhyState.OperatingPhyId,
            HalGetPhyMIB(Hw->Hal, Hw->PhyState.OperatingPhyId)->Channel, 
            FALSE
            );

        HwResetReceiveEngine(Hw, FALSE);
        HwResetSendEngine(Hw, FALSE);
        HalStartReceive(Hw->Hal);
    }
    HwEnableInterrupt(Hw, HW_ISR_TRACKING_HAL_RESET);

    HW_CLEAR_ADAPTER_STATUS(Hw, HW_ADAPTER_HAL_IN_RESET);

    return NDIS_STATUS_SUCCESS;
}

NDIS_STATUS
Hw11CtxSStart(
    _In_  PHW                     Hw
    )
{
    //
    // Set the in progress flag. This would stop any new receives
    // from getting processed.
    //
    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_IN_CONTEXT_SWITCH);
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

    MpTrace(COMP_MISC, DBG_SERIOUS, ("H/W context switch started"));

    // 
    // Wait for any current send/receive interrupt handlers to finish. New ones
    // may increment the counter but would abort once they find the above flag
    // set
    //
    HW_WAIT_FOR_ACTIVE_OPERATIONS_TO_FINISH(Hw);
    HW_WAIT_FOR_ACTIVE_SENDS_TO_FINISH(Hw);
    
    return NDIS_STATUS_SUCCESS;
}

VOID
Hw11CtxSComplete(
    _In_  PHW                     Hw
    )
{
    //
    // Clear the in progress flag. This would let receive 
    //
    MpTrace(COMP_MISC, DBG_SERIOUS, ("H/W context switch completed"));
    
    HW_CLEAR_ADAPTER_STATUS(Hw, HW_ADAPTER_IN_CONTEXT_SWITCH);    
    return;
}

NDIS_STATUS
Hw11Pause(
    _In_  PHW                     Hw
    )
{
    //
    // Set the in progress flag. This would stop any new receives
    // from getting processed.
    //
    HW_ACQUIRE_HARDWARE_LOCK(Hw, FALSE);
    HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_PAUSING);
    HW_RELEASE_HARDWARE_LOCK(Hw, FALSE);

    // 
    // Wait for any current send/receive interrupt handlers to finish. New ones
    // may increment the counter but would abort once they find the above flag
    // set
    //
    HW_WAIT_FOR_ACTIVE_OPERATIONS_TO_FINISH(Hw);

    // Wait for active sends to be finish
    HW_WAIT_FOR_ACTIVE_SENDS_TO_FINISH(Hw);

    // For performance reason we also disable the interrupt
//    MpTrace(COMP_TESTING, DBG_SERIOUS, ("Hw11Pause \n"));    
    HwDisableInterrupt(Hw, HW_ISR_TRACKING_PAUSE);

    HW_SET_ADAPTER_STATUS(Hw, HW_ADAPTER_PAUSED);
    HW_CLEAR_ADAPTER_STATUS(Hw, HW_ADAPTER_PAUSING);

    return NDIS_STATUS_SUCCESS;
}

VOID
Hw11Restart(
    _In_  PHW                     Hw
    )
{
    //
    // Clear the paused flag
    //
    HW_CLEAR_ADAPTER_STATUS(Hw, HW_ADAPTER_PAUSED);

    //
    // Reenable the disabled interrupts
    //
    
    //
    // If receives are available, we would temporarily enable the
    // RDU interrupt to ensure that we get interrupt again. Keeping this
    // interrupt on all the time would cause problems with Reset/Pause 
    // scenarios where we cannot empty the receive descriptors, and
    // so may keep getting interrupted
    //
    if (!HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE) && HwIsReceiveAvailable(Hw, FALSE))
    {
        //
        // New receives are available, but we didnt indicate
        // them in this interrupt. Enable RDU in IntrMask. This would be 
        // reset by the ClearInterrupt routine
        //
        HalInterlockedOrIntrMask(Hw->Hal, HAL_ISR_RX_DS_UNAVAILABLE);
    }    
//    MpTrace(COMP_TESTING, DBG_SERIOUS, ("Hw11Restart \n"));
    HwEnableInterrupt(Hw, HW_ISR_TRACKING_PAUSE);
}


NDIS_STATUS
HwInitializeScatterGatherDma(
    _In_  PHW                     Hw,
    _Out_ NDIS_ERROR_CODE*        ErrorCode,
    _Out_ PULONG                  ErrorValue
    )
{
    NDIS_STATUS                 ndisStatus;
    NDIS_SG_DMA_DESCRIPTION     DmaDescription;

    *ErrorCode = NDIS_STATUS_SUCCESS;
    *ErrorValue = 0;

    do
    {
        NdisZeroMemory(&DmaDescription, sizeof(DmaDescription));

        DmaDescription.Header.Type = NDIS_OBJECT_TYPE_SG_DMA_DESCRIPTION;
        DmaDescription.Header.Revision = NDIS_SG_DMA_DESCRIPTION_REVISION_1;
        DmaDescription.Header.Size = sizeof(NDIS_SG_DMA_DESCRIPTION);
        
        DmaDescription.Flags = 0;
        // The FCS does get DMA into OS buffers so we need to give extra space for that
        DmaDescription.MaximumPhysicalMapping = HW11_MAX_FRAME_SIZE + 4;
        
        // Send
        DmaDescription.ProcessSGListHandler = HWProcessSGList;

        // Receive
        DmaDescription.SharedMemAllocateCompleteHandler = HWAllocateComplete;

        ndisStatus = NdisMRegisterScatterGatherDma(
                        Hw->MiniportAdapterHandle,
                        &DmaDescription,
                        &Hw->MiniportDmaHandle
                        );
        if (ndisStatus == NDIS_STATUS_SUCCESS)
        {
            Hw->TxInfo.ScatterGatherListSize = DmaDescription.ScatterGatherListSize;
        }
        else
        {
            MpTrace(COMP_SEND, DBG_SERIOUS,  ("Failed to register SG DMA with NDIS\n"));
            *ErrorCode = NDIS_ERROR_CODE_OUT_OF_RESOURCES;
            *ErrorValue = ERRLOG_OUT_OF_SG_RESOURCES;
        }
    } while(FALSE);
    
    return ndisStatus;
}

/**
 * Clears all the software MAC state
 */
VOID
HwResetSoftwareMacState(
    _In_  PHW                     Hw
    )
{
    UCHAR                       index;

    //
    // If we have read a Network address from the registry we use that, else
    // we use the "real" hardware address
    //
    if (Hw->RegInfo.AddressOverrideEnabled)
    {
        // Use the value read from the registry
        NdisMoveMemory(Hw->MacState.MacAddress, Hw->RegInfo.OverrideAddress, DOT11_ADDRESS_SIZE);
    }
    else
    {
        // Use the value from the hardware
        NdisMoveMemory(Hw->MacState.MacAddress, HalGetPermanentAddress(Hw->Hal), DOT11_ADDRESS_SIZE);
    }
    
    Hw->MacState.PowerMgmtMode.dot11PowerMode = dot11_power_mode_active;

    Hw->MacState.MaxTransmitMSDULifetime = 512;
    Hw->MacState.MaxReceiveLifetime = 512;

    // Invalidate all the keys
    for (index = 0; index < HW11_KEY_TABLE_SIZE; index++)
    {
        Hw->MacState.KeyTable[index] = NULL;
    }
    Hw->MacState.KeyMappingKeyCount = 0;
    
    Hw->MacState.MulticastAddressCount = 0;
    Hw->MacState.AcceptAllMulticast = FALSE;

    Hw->MacState.PacketFilter = NDIS_PACKET_TYPE_802_11_BROADCAST_MGMT 
                                | NDIS_PACKET_TYPE_802_11_DIRECTED_MGMT;

    // Default we are in ExtSTA mode
    Hw->MacState.OperationMode = DOT11_OPERATION_MODE_EXTENSIBLE_STATION;
    
    Hw->MacState.SafeModeEnabled = FALSE;
    
    Hw->MacState.BSSStarted = FALSE;
    Hw->MacState.BeaconEnabled = FALSE;
    Hw->MacState.BSSMac = NULL;
    Hw->MacState.ActiveBeaconIndex = -1;

    // Start with scan completed set to be true
    Hw->ScanContext.CompleteIndicated = TRUE;
}

/**
 * Clears all the software PHY state
 */
VOID
HwResetSoftwarePhyState(
    _In_  PHW                     Hw
    )
{
    ULONG                       i;
    
    // Default we always pick PHY 0
    Hw->PhyState.OperatingPhyId = 0;

    Hw->PhyState.DiversitySupport = dot11_diversity_support_dynamic;

    Hw->PhyState.SupportedPowerLevels.uNumOfSupportedPowerLevels = 4;
    Hw->PhyState.SupportedPowerLevels.uTxPowerLevelValues[0] = 10;
    Hw->PhyState.SupportedPowerLevels.uTxPowerLevelValues[1] = 20;
    Hw->PhyState.SupportedPowerLevels.uTxPowerLevelValues[2] = 30;
    Hw->PhyState.SupportedPowerLevels.uTxPowerLevelValues[3] = 50;
    Hw->PhyState.CurrentTxPowerLevel = 1;    // 1 based

    Hw->PhyState.RegDomainsSupportValue->uNumOfEntries = 7;
    Hw->PhyState.RegDomainsSupportValue->uTotalNumOfEntries = HW_MAX_NUM_DOT11_REG_DOMAINS_VALUE;
    for (i = 0; i < Hw->PhyState.RegDomainsSupportValue->uNumOfEntries; i++)
        Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[i].uRegDomainsSupportIndex = i;

    Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[0].uRegDomainsSupportValue = DOT11_REG_DOMAIN_OTHER;
    Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[1].uRegDomainsSupportValue = DOT11_REG_DOMAIN_FCC;
    Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[2].uRegDomainsSupportValue = DOT11_REG_DOMAIN_DOC;
    Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[3].uRegDomainsSupportValue = DOT11_REG_DOMAIN_ETSI;
    Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[4].uRegDomainsSupportValue = DOT11_REG_DOMAIN_SPAIN;
    Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[5].uRegDomainsSupportValue = DOT11_REG_DOMAIN_FRANCE;
    Hw->PhyState.RegDomainsSupportValue->dot11RegDomainValue[6].uRegDomainsSupportValue = DOT11_REG_DOMAIN_MKK;

    // NOTE: For regulatory compliance, this must be loaded either from the hardware 
    // or read from the OS
    Hw->PhyState.DefaultRegDomain = DOT11_REG_DOMAIN_FCC;

    Hw->PhyState.DiversitySelectionRxList->uNumOfEntries = 2; 
    Hw->PhyState.DiversitySelectionRxList->uTotalNumOfEntries = HW_MAX_NUM_DIVERSITY_SELECTION_RX_LIST;
    Hw->PhyState.DiversitySelectionRxList->dot11DiversitySelectionRx[0].uAntennaListIndex = 1;
    Hw->PhyState.DiversitySelectionRxList->dot11DiversitySelectionRx[0].bDiversitySelectionRX = TRUE;
    Hw->PhyState.DiversitySelectionRxList->dot11DiversitySelectionRx[1].uAntennaListIndex = 2;
    Hw->PhyState.DiversitySelectionRxList->dot11DiversitySelectionRx[1].bDiversitySelectionRX = TRUE;

}


/**
 * Clears the hardware state
 */
NDIS_STATUS
HwClearNicState(
    _In_  PHW                     Hw
    )
{
    UCHAR                       index;

    //
    // Shouldnt be looking at the MAC/PHY state stored in hardware. We just 
    // want to clear everything from the hardware
    //

    //
    // Remove the keys from the hardware
    //
    for (index = 0; index < HW11_KEY_TABLE_SIZE; index++)
    {
        HalRemoveKeyEntry(Hw->Hal, index);
    }

    return NDIS_STATUS_SUCCESS;
}


/**
 * Programs the software state onto the hardware
 */
NDIS_STATUS
HwSetNicState(
    _In_  PHW                     Hw
    )
{
    // If hardware shouldnt be accessed, return
    if (HW_TEST_ADAPTER_STATUS(Hw, HW_CANNOT_ACCESS_HARDWARE))
        return NDIS_STATUS_SUCCESS;
        
    //
    // Ensure the H/W is in the correct state
    //
    HalSetOperationMode(Hw->Hal, Hw->MacState.OperationMode);
    
    //
    // Set the MAC address
    //
    HalSetMacAddress(Hw->Hal, Hw->MacState.MacAddress);

    HalSetBssId(Hw->Hal, Hw->MacState.MacAddress);

    HalSetPacketFilter(Hw->Hal, Hw->MacState.PacketFilter);

    //
    // The interrupt mask
    //
    HalSetIntrMask(Hw->Hal, 
        (HAL_ISR_TX_DONE | HAL_ISR_RX_DONE | HAL_ISR_BEACON_INTERRUPT | HAL_ISR_ATIM_END)
        );

    return NDIS_STATUS_SUCCESS;
}


