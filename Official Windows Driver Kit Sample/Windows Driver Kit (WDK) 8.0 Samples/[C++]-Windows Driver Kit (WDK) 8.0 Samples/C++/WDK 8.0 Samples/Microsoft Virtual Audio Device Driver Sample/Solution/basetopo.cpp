/*++

Copyright (c) 1997-2000  Microsoft Corporation All Rights Reserved

Module Name:

    basetopo.cpp

Abstract:

    Implementation of topology miniport. This the base class for 
    all MSVAD samples

--*/

#pragma warning (disable : 4127)

#include <msvad.h>
#include "common.h"
#include "basetopo.h"

//=============================================================================
#pragma code_seg("PAGE")
CMiniportTopologyMSVAD::CMiniportTopologyMSVAD
(
    void
)
/*++

Routine Description:

  Topology miniport constructor

Arguments:

Return Value:

  void

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[%s]",__FUNCTION__));

    m_AdapterCommon = NULL;
    m_FilterDescriptor = NULL;
} // CMiniportTopologyMSVAD

CMiniportTopologyMSVAD::~CMiniportTopologyMSVAD
(
    void
)
/*++

Routine Description:

  Topology miniport destructor

Arguments:

Return Value:

  void

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[%s]",__FUNCTION__));

    if (m_AdapterCommon)
    {
        m_AdapterCommon->Release();
    }
} // ~CMiniportTopologyMSVAD

//=============================================================================
NTSTATUS
CMiniportTopologyMSVAD::DataRangeIntersection
( 
    _In_        ULONG                   PinId,
    _In_        PKSDATARANGE            ClientDataRange,
    _In_        PKSDATARANGE            MyDataRange,
    _In_        ULONG                   OutputBufferLength,
    _Out_writes_bytes_to_opt_(OutputBufferLength, *ResultantFormatLength)
                PVOID                   ResultantFormat,
    _Out_       PULONG                  ResultantFormatLength 
)
/*++

Routine Description:

  The DataRangeIntersection function determines the highest 
  quality intersection of two data ranges. Topology miniport does nothing.

Arguments:

  PinId - Pin for which data intersection is being determined. 

  ClientDataRange - Pointer to KSDATARANGE structure which contains the data range 
                    submitted by client in the data range intersection property 
                    request

  MyDataRange - Pin's data range to be compared with client's data range

  OutputBufferLength - Size of the buffer pointed to by the resultant format 
                       parameter

  ResultantFormat - Pointer to value where the resultant format should be 
                    returned

  ResultantFormatLength - Actual length of the resultant format that is placed 
                          at ResultantFormat. This should be less than or equal 
                          to OutputBufferLength

Return Value:

  NT status code.

--*/
{
    UNREFERENCED_PARAMETER(PinId);
    UNREFERENCED_PARAMETER(ClientDataRange);
    UNREFERENCED_PARAMETER(MyDataRange);
    UNREFERENCED_PARAMETER(OutputBufferLength);
    UNREFERENCED_PARAMETER(ResultantFormat);
    UNREFERENCED_PARAMETER(ResultantFormatLength);

    PAGED_CODE();

    DPF_ENTER(("[%s]",__FUNCTION__));

    return (STATUS_NOT_IMPLEMENTED);
} // DataRangeIntersection

//=============================================================================
NTSTATUS
CMiniportTopologyMSVAD::GetDescription
( 
    _Out_ PPCFILTER_DESCRIPTOR *  OutFilterDescriptor 
)
/*++

Routine Description:

  The GetDescription function gets a pointer to a filter description. 
  It provides a location to deposit a pointer in miniport's description 
  structure. This is the placeholder for the FromNode or ToNode fields in 
  connections which describe connections to the filter's pins

Arguments:

  OutFilterDescriptor - Pointer to the filter description. 

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(OutFilterDescriptor);

    DPF_ENTER(("[%s]",__FUNCTION__));

    *OutFilterDescriptor = m_FilterDescriptor;

    return (STATUS_SUCCESS);
} // GetDescription

//=============================================================================
NTSTATUS
CMiniportTopologyMSVAD::Init
( 
    IN  PUNKNOWN                UnknownAdapter_,
    IN  PPORTTOPOLOGY           Port_ 
)
/*++

Routine Description:

  Initializes the topology miniport.

Arguments:

  UnknownAdapter -

  Port_ - Pointer to topology port

Return Value:

  NT status code.

--*/
{
    UNREFERENCED_PARAMETER(Port_);

    PAGED_CODE();

    ASSERT(UnknownAdapter_);
    ASSERT(Port_);

    DPF_ENTER(("[CMiniportTopologyMSVAD::Init]"));

    NTSTATUS                    ntStatus;

    ntStatus = 
        UnknownAdapter_->QueryInterface
        ( 
            IID_IAdapterCommon,
            (PVOID *) &m_AdapterCommon
        );
    if (NT_SUCCESS(ntStatus))
    {
        m_AdapterCommon->MixerReset();
    }

    if (!NT_SUCCESS(ntStatus))
    {
        // clean up AdapterCommon
        if (m_AdapterCommon)
        {
            m_AdapterCommon->Release();
            m_AdapterCommon = NULL;
        }
    }

    return ntStatus;
} // Init

//=============================================================================
NTSTATUS
CMiniportTopologyMSVAD::PropertyHandlerBasicSupportVolume
(
    IN  PPCPROPERTY_REQUEST     PropertyRequest
)
/*++

Routine Description:

  Handles BasicSupport for Volume nodes.

Arguments:
    
  PropertyRequest - property request structure

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    NTSTATUS                    ntStatus = STATUS_SUCCESS;
    ULONG                       cbFullProperty = 
        sizeof(KSPROPERTY_DESCRIPTION) +
        sizeof(KSPROPERTY_MEMBERSHEADER) +
        sizeof(KSPROPERTY_STEPPING_LONG);

    if (PropertyRequest->ValueSize >= (sizeof(KSPROPERTY_DESCRIPTION)))
    {
        PKSPROPERTY_DESCRIPTION PropDesc = 
            PKSPROPERTY_DESCRIPTION(PropertyRequest->Value);

        PropDesc->AccessFlags       = KSPROPERTY_TYPE_ALL;
        PropDesc->DescriptionSize   = cbFullProperty;
        PropDesc->PropTypeSet.Set   = KSPROPTYPESETID_General;
        PropDesc->PropTypeSet.Id    = VT_I4;
        PropDesc->PropTypeSet.Flags = 0;
        PropDesc->MembersListCount  = 1;
        PropDesc->Reserved          = 0;

        // if return buffer can also hold a range description, return it too
        if(PropertyRequest->ValueSize >= cbFullProperty)
        {
            // fill in the members header
            PKSPROPERTY_MEMBERSHEADER Members = 
                PKSPROPERTY_MEMBERSHEADER(PropDesc + 1);

            Members->MembersFlags   = KSPROPERTY_MEMBER_STEPPEDRANGES;
            Members->MembersSize    = sizeof(KSPROPERTY_STEPPING_LONG);
            Members->MembersCount   = 1;
            Members->Flags          = KSPROPERTY_MEMBER_FLAG_BASICSUPPORT_MULTICHANNEL;

            // fill in the stepped range
            PKSPROPERTY_STEPPING_LONG Range = 
                PKSPROPERTY_STEPPING_LONG(Members + 1);

            Range->Bounds.SignedMaximum = 0x00000000;      //   0 dB
            Range->Bounds.SignedMinimum = -96 * 0x10000;   // -96 dB
            Range->SteppingDelta        = 0x08000;         //  .5 dB
            Range->Reserved             = 0;

            // set the return value size
            PropertyRequest->ValueSize = cbFullProperty;
        } 
        else
        {
            PropertyRequest->ValueSize = sizeof(KSPROPERTY_DESCRIPTION);
        }
    } 
    else if(PropertyRequest->ValueSize >= sizeof(ULONG))
    {
        // if return buffer can hold a ULONG, return the access flags
        PULONG AccessFlags = PULONG(PropertyRequest->Value);

        PropertyRequest->ValueSize = sizeof(ULONG);
        *AccessFlags = KSPROPERTY_TYPE_ALL;
    }
    else
    {
        PropertyRequest->ValueSize = 0;
        ntStatus = STATUS_BUFFER_TOO_SMALL;
    }

    return ntStatus;
} // PropertyHandlerBasicSupportVolume

//=============================================================================
NTSTATUS
CMiniportTopologyMSVAD::PropertyHandlerCpuResources
( 
    IN  PPCPROPERTY_REQUEST     PropertyRequest 
)
/*++

Routine Description:

  Processes KSPROPERTY_AUDIO_CPURESOURCES

Arguments:
    
  PropertyRequest - property request structure

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[%s]",__FUNCTION__));

    NTSTATUS ntStatus = STATUS_INVALID_DEVICE_REQUEST;

    if (PropertyRequest->Verb & KSPROPERTY_TYPE_GET)
    {
        ntStatus = ValidatePropertyParams(PropertyRequest, sizeof(ULONG));
        if (NT_SUCCESS(ntStatus))
        {
            *(PLONG(PropertyRequest->Value)) = KSAUDIO_CPU_RESOURCES_NOT_HOST_CPU;
            PropertyRequest->ValueSize = sizeof(LONG);
        }
    }
    else if (PropertyRequest->Verb & KSPROPERTY_TYPE_BASICSUPPORT)
    {
        ntStatus = 
            PropertyHandler_BasicSupport
            ( 
                PropertyRequest, 
                KSPROPERTY_TYPE_GET | KSPROPERTY_TYPE_BASICSUPPORT,
                VT_ILLEGAL
            );
    }

    return ntStatus;
} // PropertyHandlerCpuResources

//=============================================================================
NTSTATUS                            
CMiniportTopologyMSVAD::PropertyHandlerGeneric
(
    IN  PPCPROPERTY_REQUEST     PropertyRequest
)
/*++

Routine Description:

  Handles all properties for this miniport.

Arguments:

  PropertyRequest - property request structure

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    NTSTATUS                    ntStatus = STATUS_INVALID_DEVICE_REQUEST;

    switch (PropertyRequest->PropertyItem->Id)
    {
        case KSPROPERTY_AUDIO_VOLUMELEVEL:
            ntStatus = PropertyHandlerVolume(PropertyRequest);
            break;
        
        case KSPROPERTY_AUDIO_CPU_RESOURCES:
            ntStatus = PropertyHandlerCpuResources(PropertyRequest);
            break;

        case KSPROPERTY_AUDIO_MUTE:
            ntStatus = PropertyHandlerMute(PropertyRequest);
            break;

        case KSPROPERTY_AUDIO_MUX_SOURCE:
            ntStatus = PropertyHandlerMuxSource(PropertyRequest);
            break;

        case KSPROPERTY_AUDIO_DEV_SPECIFIC:
            ntStatus = PropertyHandlerDevSpecific(PropertyRequest);
            break;

        default:
            DPF(D_TERSE, ("[PropertyHandlerGeneric: Invalid Device Request]"));
    }

    return ntStatus;
} // PropertyHandlerGeneric

//=============================================================================
NTSTATUS                            
CMiniportTopologyMSVAD::PropertyHandlerMute
(
    IN  PPCPROPERTY_REQUEST     PropertyRequest
)
/*++

Routine Description:

  Property handler for KSPROPERTY_AUDIO_MUTE

Arguments:

  PropertyRequest - property request structure

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[%s]",__FUNCTION__));

    NTSTATUS                    ntStatus;
    PBOOL                       pfMute;

    if (PropertyRequest->Verb & KSPROPERTY_TYPE_BASICSUPPORT)
    {
        ntStatus = 
            PropertyHandler_BasicSupport
            (
                PropertyRequest,
                KSPROPERTY_TYPE_ALL,
                VT_BOOL
            );
    }
    else
    {
        ntStatus = 
            ValidatePropertyParams
            (   
                PropertyRequest, 
                sizeof(BOOL), 
                sizeof(LONG)
            );
        if (NT_SUCCESS(ntStatus))
        {
            // If the channel index is needed, it is supplied in the Instance parameter
            // LONG lChannel = * PLONG (PropertyRequest->Instance);
            //
            pfMute   = PBOOL (PropertyRequest->Value);

            if (PropertyRequest->Verb & KSPROPERTY_TYPE_GET)
            {
                *pfMute = 
                    m_AdapterCommon->MixerMuteRead
                    (
                        PropertyRequest->Node
                    );
                PropertyRequest->ValueSize = sizeof(BOOL);
            }
            else if (PropertyRequest->Verb & KSPROPERTY_TYPE_SET)
            {
                m_AdapterCommon->MixerMuteWrite
                (
                    PropertyRequest->Node, 
                    *pfMute
                );
            }
        }
        else
        {
            DPF(D_TERSE, ("[%s - ntStatus=0x%08x]",__FUNCTION__,ntStatus));
        }
    }

    return ntStatus;
} // PropertyHandlerMute

//=============================================================================
NTSTATUS                            
CMiniportTopologyMSVAD::PropertyHandlerMuxSource
(
    IN  PPCPROPERTY_REQUEST     PropertyRequest
)
/*++

Routine Description:

  PropertyHandler for KSPROPERTY_AUDIO_MUX_SOURCE.

Arguments:

  PropertyRequest - property request structure

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[%s]",__FUNCTION__));

    NTSTATUS                    ntStatus = STATUS_INVALID_DEVICE_REQUEST;

    //
    // Validate node
    // This property is only valid for WAVEIN_MUX node.
    //
    // TODO if (WAVEIN_MUX == PropertyRequest->Node)
    {
        if (PropertyRequest->ValueSize >= sizeof(ULONG))
        {
            PULONG pulMuxValue = PULONG(PropertyRequest->Value);
            
            if (PropertyRequest->Verb & KSPROPERTY_TYPE_GET)
            {
                *pulMuxValue = m_AdapterCommon->MixerMuxRead();
                PropertyRequest->ValueSize = sizeof(ULONG);
                ntStatus = STATUS_SUCCESS;
            }
            else if (PropertyRequest->Verb & KSPROPERTY_TYPE_SET)
            {
                m_AdapterCommon->MixerMuxWrite(*pulMuxValue);
                ntStatus = STATUS_SUCCESS;
            }
            else if (PropertyRequest->Verb & KSPROPERTY_TYPE_BASICSUPPORT)
            {
                ntStatus = 
                    PropertyHandler_BasicSupport
                    ( 
                        PropertyRequest, 
                        KSPROPERTY_TYPE_ALL,
                        VT_I4
                    );
            }
        }
        else
        {
            DPF(D_TERSE, ("[PropertyHandlerMuxSource - Invalid parameter]"));
            ntStatus = STATUS_INVALID_PARAMETER;
        }
    }

    return ntStatus;
} // PropertyHandlerMuxSource

//=============================================================================
NTSTATUS
CMiniportTopologyMSVAD::PropertyHandlerVolume
(
    IN  PPCPROPERTY_REQUEST     PropertyRequest     
)
/*++

Routine Description:

  Property handler for KSPROPERTY_AUDIO_VOLUMELEVEL

Arguments:

  PropertyRequest - property request structure

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[%s]",__FUNCTION__));

    NTSTATUS ntStatus = STATUS_INVALID_DEVICE_REQUEST;
    LONG     lChannel;
    PULONG   pulVolume;

    if (PropertyRequest->Verb & KSPROPERTY_TYPE_BASICSUPPORT)
    {
        ntStatus = PropertyHandlerBasicSupportVolume(PropertyRequest);
    }
    else
    {
        ntStatus = 
            ValidatePropertyParams
            (
                PropertyRequest, 
                sizeof(ULONG),  // volume value is a ULONG
                sizeof(LONG)    // instance is the channel number
            );
        if (NT_SUCCESS(ntStatus))
        {
            lChannel = * (PLONG (PropertyRequest->Instance));
            pulVolume = PULONG (PropertyRequest->Value);

            if (PropertyRequest->Verb & KSPROPERTY_TYPE_GET)
            {
                *pulVolume = 
                    m_AdapterCommon->MixerVolumeRead
                    (
                        PropertyRequest->Node, 
                        lChannel
                    );
                PropertyRequest->ValueSize = sizeof(ULONG);                
            }
            else if (PropertyRequest->Verb & KSPROPERTY_TYPE_SET)
            {
                m_AdapterCommon->MixerVolumeWrite
                (
                    PropertyRequest->Node, 
                    lChannel, 
                    *pulVolume
                );
            }
        }
        else
        {
            DPF(D_TERSE, ("[%s - ntStatus=0x%08x]",__FUNCTION__,ntStatus));
        }
    }

    return ntStatus;
} // PropertyHandlerVolume


//=============================================================================
NTSTATUS                            
CMiniportTopologyMSVAD::PropertyHandlerDevSpecific
(
    IN  PPCPROPERTY_REQUEST     PropertyRequest
)
/*++

Routine Description:

  Property handler for KSPROPERTY_AUDIO_DEV_SPECIFIC

Arguments:

  PropertyRequest - property request structure

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[%s]",__FUNCTION__));

    NTSTATUS ntStatus=STATUS_SUCCESS;

    if (PropertyRequest->Verb & KSPROPERTY_TYPE_BASICSUPPORT)
    {
        if( DEV_SPECIFIC_VT_BOOL == PropertyRequest->Node )
        {
            ntStatus = PropertyHandler_BasicSupport(PropertyRequest,KSPROPERTY_TYPE_ALL,VT_BOOL);
        }
        else
        {
            ULONG ExpectedSize = sizeof( KSPROPERTY_DESCRIPTION ) + 
                                 sizeof( KSPROPERTY_MEMBERSHEADER ) + 
                                 sizeof( KSPROPERTY_BOUNDS_LONG );
            DWORD ulPropTypeSetId;

            if( DEV_SPECIFIC_VT_I4 == PropertyRequest->Node )
            {
                ulPropTypeSetId = VT_I4;
            }
            else if ( DEV_SPECIFIC_VT_UI4 == PropertyRequest->Node )
            {
                ulPropTypeSetId = VT_UI4;
            }
            else
            {
                ulPropTypeSetId = VT_ILLEGAL;
                ntStatus = STATUS_INVALID_PARAMETER;
            }

            if( NT_SUCCESS(ntStatus))
            {
                if ( !PropertyRequest->ValueSize )
                {
                    PropertyRequest->ValueSize = ExpectedSize;
                    ntStatus = STATUS_BUFFER_OVERFLOW;
                } 
                else if (PropertyRequest->ValueSize >= sizeof(KSPROPERTY_DESCRIPTION))
                {
                    // if return buffer can hold a KSPROPERTY_DESCRIPTION, return it
                    //
                    PKSPROPERTY_DESCRIPTION PropDesc = PKSPROPERTY_DESCRIPTION(PropertyRequest->Value);

                    PropDesc->AccessFlags       = KSPROPERTY_TYPE_ALL;
                    PropDesc->DescriptionSize   = ExpectedSize;
                    PropDesc->PropTypeSet.Set   = KSPROPTYPESETID_General;
                    PropDesc->PropTypeSet.Id    = ulPropTypeSetId;
                    PropDesc->PropTypeSet.Flags = 0;
                    PropDesc->MembersListCount  = 0;
                    PropDesc->Reserved          = 0;

                    if ( PropertyRequest->ValueSize >= ExpectedSize )
                    {
                        // Extra information to return
                        PropDesc->MembersListCount  = 1;

                        PKSPROPERTY_MEMBERSHEADER MembersHeader = ( PKSPROPERTY_MEMBERSHEADER )( PropDesc + 1);
                        MembersHeader->MembersFlags = KSPROPERTY_MEMBER_RANGES;
                        MembersHeader->MembersCount  = 1;
                        MembersHeader->MembersSize   = sizeof( KSPROPERTY_BOUNDS_LONG );
                        MembersHeader->Flags = 0;

                        PKSPROPERTY_BOUNDS_LONG PeakMeterBounds = (PKSPROPERTY_BOUNDS_LONG)( MembersHeader + 1);
                        if(VT_I4 == ulPropTypeSetId )
                        {
                            PeakMeterBounds->SignedMinimum = 0;
                            PeakMeterBounds->SignedMaximum = 0x7fffffff;
                        }
                        else
                        {
                            PeakMeterBounds->UnsignedMinimum = 0;
                            PeakMeterBounds->UnsignedMaximum = 0xffffffff;
                        }

                        // set the return value size
                        PropertyRequest->ValueSize = ExpectedSize;
                    }
                    else
                    {
                        // No extra information to return.
                        PropertyRequest->ValueSize = sizeof(KSPROPERTY_DESCRIPTION);
                    }

                    ntStatus = STATUS_SUCCESS;
                } 
                else if (PropertyRequest->ValueSize >= sizeof(ULONG))
                {
                    // if return buffer can hold a ULONG, return the access flags
                    //
                    *(PULONG(PropertyRequest->Value)) = KSPROPERTY_TYPE_ALL;

                    PropertyRequest->ValueSize = sizeof(ULONG);
                    ntStatus = STATUS_SUCCESS;                    
                }
                else
                {
                    PropertyRequest->ValueSize = 0;
                    ntStatus = STATUS_BUFFER_TOO_SMALL;
                }
            }
        }
    }
    else
    {
        // switch on node id
        switch( PropertyRequest->Node )
        {
        case DEV_SPECIFIC_VT_BOOL:
            {
                PBOOL pbDevSpecific;

                ntStatus = ValidatePropertyParams(PropertyRequest, sizeof(BOOL), 0);

                if (NT_SUCCESS(ntStatus))
                {
                    pbDevSpecific   = PBOOL (PropertyRequest->Value);

                    if (PropertyRequest->Verb & KSPROPERTY_TYPE_GET)
                    {
                        *pbDevSpecific = m_AdapterCommon->bDevSpecificRead();
                        PropertyRequest->ValueSize = sizeof(BOOL);
                    }
                    else if (PropertyRequest->Verb & KSPROPERTY_TYPE_SET)
                    {
                        m_AdapterCommon->bDevSpecificWrite(*pbDevSpecific);
                    }
                    else
                    {
                        ntStatus = STATUS_INVALID_PARAMETER;
                    }
                }
            }
            break;
        case DEV_SPECIFIC_VT_I4:
            {
                INT* piDevSpecific;

                ntStatus = ValidatePropertyParams(PropertyRequest, sizeof(int), 0);

                if (NT_SUCCESS(ntStatus))
                {
                    piDevSpecific   = PINT (PropertyRequest->Value);

                    if (PropertyRequest->Verb & KSPROPERTY_TYPE_GET)
                    {
                        *piDevSpecific = m_AdapterCommon->iDevSpecificRead();
                        PropertyRequest->ValueSize = sizeof(int);
                    }
                    else if (PropertyRequest->Verb & KSPROPERTY_TYPE_SET)
                    {
                        m_AdapterCommon->iDevSpecificWrite(*piDevSpecific);
                    }
                    else
                    {
                        ntStatus = STATUS_INVALID_PARAMETER;
                    }
                }
            }
            break;
        case DEV_SPECIFIC_VT_UI4:
            {
                UINT* puiDevSpecific;

                ntStatus = ValidatePropertyParams(PropertyRequest, sizeof(UINT), 0);

                if (NT_SUCCESS(ntStatus))
                {
                    puiDevSpecific   = PUINT (PropertyRequest->Value);

                    if (PropertyRequest->Verb & KSPROPERTY_TYPE_GET)
                    {
                        *puiDevSpecific = m_AdapterCommon->uiDevSpecificRead();
                        PropertyRequest->ValueSize = sizeof(UINT);
                    }
                    else if (PropertyRequest->Verb & KSPROPERTY_TYPE_SET)
                    {
                        m_AdapterCommon->uiDevSpecificWrite(*puiDevSpecific);
                    }
                    else
                    {
                        ntStatus = STATUS_INVALID_PARAMETER;
                    }
                }
            }
            break;
        default:
            ntStatus = STATUS_INVALID_PARAMETER;
            break;
        }


        if( !NT_SUCCESS(ntStatus))
        {
            DPF(D_TERSE, ("[%s - ntStatus=0x%08x]",__FUNCTION__,ntStatus));
        }
    }

    return ntStatus;
} // PropertyHandlerDevSpecific



#pragma code_seg()

