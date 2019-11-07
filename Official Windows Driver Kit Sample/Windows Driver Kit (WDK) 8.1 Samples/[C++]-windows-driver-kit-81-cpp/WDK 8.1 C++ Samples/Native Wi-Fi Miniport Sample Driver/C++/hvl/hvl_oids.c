/*++

Copyright (c) Microsoft Corporation. All rights reserved.

Module Name:
    hvl_oids.c

Abstract:
    Implements the OID handling for the Hvl
    
Revision History:
      When        What
    ----------    ----------------------------------------------
    09-04-2007    Created

Notes:

--*/

#include "precomp.h"

#if DOT11_TRACE_ENABLED
#include "hvl_oids.tmh"
#endif

#define  NUM_SUPPORTED_VWIFI_COMBINATIONS   1

NDIS_STATUS
Hvl11Fill80211Attributes(
    _In_  PHVL                    Hvl,
    _Out_ PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    )
{
    NDIS_STATUS                 ndisStatus = NDIS_STATUS_SUCCESS;
    ULONG                       vwifiAttrSize = 0;
    PDOT11_VWIFI_ATTRIBUTES VWiFiAttribs = NULL;
    PDOT11_VWIFI_COMBINATION_V2 pCombination = NULL;
    
    do
    {
        vwifiAttrSize = FIELD_OFFSET(DOT11_VWIFI_ATTRIBUTES, Combinations) + 
                    NUM_SUPPORTED_VWIFI_COMBINATIONS * DOT11_SIZEOF_VWIFI_COMBINATION_REVISION_2;

        
        MP_ALLOCATE_MEMORY(
            Hvl->MiniportAdapterHandle, 
            &Attr->VWiFiAttributes,
            vwifiAttrSize,
            MP_MEMORY_TAG
            );
        
        if (NULL == Attr->VWiFiAttributes)
        {
            MpTrace(COMP_OID, DBG_SERIOUS, ("Failed to allocate %d bytes for VWiFi capability.\n",
                                 vwifiAttrSize));
            ndisStatus = NDIS_STATUS_RESOURCES;
            break;
        }

        VWiFiAttribs = Attr->VWiFiAttributes;
        
        NdisZeroMemory(VWiFiAttribs, vwifiAttrSize);
        
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            VWiFiAttribs->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_VWIFI_ATTRIBUTES_REVISION_1,
            sizeof(DOT11_VWIFI_ATTRIBUTES));

        VWiFiAttribs->uTotalNumOfEntries = NUM_SUPPORTED_VWIFI_COMBINATIONS;


        pCombination = (PDOT11_VWIFI_COMBINATION_V2)&VWiFiAttribs->Combinations[0];

        
        // support for Infra-SoftAP
        MP_ASSIGN_NDIS_OBJECT_HEADER(
            pCombination->Header, 
            NDIS_OBJECT_TYPE_DEFAULT,
            DOT11_VWIFI_COMBINATION_REVISION_2,
            DOT11_SIZEOF_VWIFI_COMBINATION_REVISION_2);

        pCombination->uNumInfrastructure = 1;
        pCombination->uNumSoftAP = 1;

        
    }while (FALSE);

    return ndisStatus;
}

VOID
Hvl11Cleanup80211Attributes(
    _In_  PHVL                    Hvl,
    _In_  PNDIS_MINIPORT_ADAPTER_NATIVE_802_11_ATTRIBUTES Attr
    )
{    
    UNREFERENCED_PARAMETER(Hvl);

    if (Attr->VWiFiAttributes)
    {
        MP_FREE_MEMORY(Attr->VWiFiAttributes);
    }
}

