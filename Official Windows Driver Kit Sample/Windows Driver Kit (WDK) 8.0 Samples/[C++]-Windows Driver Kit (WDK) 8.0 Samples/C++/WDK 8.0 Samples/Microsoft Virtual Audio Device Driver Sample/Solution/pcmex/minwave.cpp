/*++

Copyright (c) 1997-2000  Microsoft Corporation All Rights Reserved

Module Name:

    minwave.cpp

Abstract:

    Implementation of wavecyclic miniport.

--*/

#pragma warning (disable : 4127)

#include <msvad.h>
#include <common.h>
#include "pcmex.h"
#include "minwave.h"
#include "wavtable.h"

#pragma code_seg("PAGE")

//=============================================================================
// CMiniportWaveCyclic
//=============================================================================

//=============================================================================
NTSTATUS
CreateMiniportWaveCyclicMSVAD
( 
    OUT PUNKNOWN *              Unknown,
    IN  REFCLSID,
    IN  PUNKNOWN                UnknownOuter OPTIONAL,
    _When_((PoolType & NonPagedPoolMustSucceed) != 0,
       __drv_reportError("Must succeed pool allocations are forbidden. "
			 "Allocation failures cause a system crash"))
    IN  POOL_TYPE               PoolType 
)
/*++

Routine Description:

  Create the wavecyclic miniport.

Arguments:

  Unknown - 

  RefClsId -

  UnknownOuter -

  PoolType -

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(Unknown);

    STD_CREATE_BODY(CMiniportWaveCyclic, Unknown, UnknownOuter, PoolType);
}

//=============================================================================
CMiniportWaveCyclic::~CMiniportWaveCyclic
( 
    void 
)
/*++

Routine Description:

  Destructor for wavecyclic miniport

Arguments:

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[CMiniportWaveCyclic::~CMiniportWaveCyclic]"));
} // ~CMiniportWaveCyclic


//=============================================================================
STDMETHODIMP_(NTSTATUS)
CMiniportWaveCyclic::DataRangeIntersection
( 
    _In_        ULONG           PinId,
    _In_        PKSDATARANGE    ClientDataRange,
    _In_        PKSDATARANGE    MyDataRange,
    _In_        ULONG           OutputBufferLength,
    _Out_writes_bytes_to_opt_(OutputBufferLength, *ResultantFormatLength)
                PVOID           ResultantFormat,
    _Out_       PULONG          ResultantFormatLength 
)
/*++

Routine Description:

  The DataRangeIntersection function determines the highest quality 
  intersection of two data ranges.  Note that the driver assumes that
  all values within the minimum and maximum values for sample rate, 
  bit depth, and channel count are valid as opposed to just discrete
  values.

Arguments:

  PinId -           Pin for which data intersection is being determined. 

  ClientDataRange - Pointer to KSDATARANGE structure which contains the data 
                    range submitted by client in the data range intersection 
                    property request. 

  MyDataRange -         Pin's data range to be compared with client's data 
                        range.

  OutputBufferLength -  Size of the buffer pointed to by the resultant format 
                        parameter. 

  ResultantFormat -     Pointer to value where the resultant format should be 
                        returned. 

  ResultantFormatLength -   Actual length of the resultant format placed in 
                            ResultantFormat. This should be less than or equal 
                            to OutputBufferLength. 

  Return Value:

    NT status code.

--*/
{
    UNREFERENCED_PARAMETER(PinId);

    PAGED_CODE();

    // This code is the same as AC97 sample intersection handler.
    //

    // Check the size of output buffer. Note that we are returning
    // WAVEFORMATPCMEX.
    //
    if (!OutputBufferLength || !ResultantFormat) 
    {
        *ResultantFormatLength = sizeof(KSDATAFORMAT) + sizeof(WAVEFORMATPCMEX);
        return STATUS_BUFFER_OVERFLOW;
    } 
    
    if (OutputBufferLength < (sizeof(KSDATAFORMAT) + sizeof(WAVEFORMATPCMEX))) 
    {
        return STATUS_BUFFER_TOO_SMALL;
    }

#pragma warning(push)
    // CMiniportWaveCyclic::DataRangeIntersection's annotation on ResultantFormat is Out_opt_ with buffer 
    // length OutputBufferLength. If we get to this point, then ResultantFormat != NULL and buffer length 
    // OutputBufferLength is larger enough to hold data of size KSDATAFORMAT, so supress the warning.
    //
#pragma warning(disable:6386)
    // Fill in the structure the datarange structure.
    //
    RtlCopyMemory(ResultantFormat, MyDataRange, sizeof(KSDATAFORMAT));
#pragma warning (pop)

    // Modify the size of the data format structure to fit the WAVEFORMATPCMEX
    // structure.
    //
    ((PKSDATAFORMAT)ResultantFormat)->FormatSize =
        sizeof(KSDATAFORMAT) + sizeof(WAVEFORMATPCMEX);

    // Append the WAVEFORMATPCMEX structure
    //
    PWAVEFORMATPCMEX pWfxExt = 
        (PWAVEFORMATPCMEX)((PKSDATAFORMAT)ResultantFormat + 1);

    // Ensure that the returned channel count falls within our range of
    // supported channel counts.
    pWfxExt->Format.nChannels = 
        (WORD)min(((PKSDATARANGE_AUDIO) ClientDataRange)->MaximumChannels, 
                    ((PKSDATARANGE_AUDIO) MyDataRange)->MaximumChannels);


    // Ensure that the returned sample rate falls within the supported range
    // of sample rates from our data range.
    if((((PKSDATARANGE_AUDIO) ClientDataRange)->MaximumSampleFrequency <
        ((PKSDATARANGE_AUDIO) MyDataRange)->MinimumSampleFrequency) ||
        (((PKSDATARANGE_AUDIO) ClientDataRange)->MinimumSampleFrequency >
        ((PKSDATARANGE_AUDIO) MyDataRange)->MaximumSampleFrequency))
    {
        DPF(D_TERSE, ("[No intersection in sample rate ranges]"));
        return STATUS_NO_MATCH;
    }
    pWfxExt->Format.nSamplesPerSec = 
        min(((PKSDATARANGE_AUDIO) ClientDataRange)->MaximumSampleFrequency,
            ((PKSDATARANGE_AUDIO) MyDataRange)->MaximumSampleFrequency);

    // Ensure that the returned bits per sample is in the supported
    // range of bit depths from our data range.
    if((((PKSDATARANGE_AUDIO) ClientDataRange)->MaximumBitsPerSample <
        ((PKSDATARANGE_AUDIO) MyDataRange)->MinimumBitsPerSample) ||
        (((PKSDATARANGE_AUDIO) ClientDataRange)->MinimumBitsPerSample >
        ((PKSDATARANGE_AUDIO) MyDataRange)->MaximumBitsPerSample))
    {
        DPF(D_TERSE, ("[No intersection in bits per sample ranges]"));
        return STATUS_NO_MATCH;
    }
    pWfxExt->Format.wBitsPerSample = 
        (WORD)min(((PKSDATARANGE_AUDIO) ClientDataRange)->MaximumBitsPerSample,
                    ((PKSDATARANGE_AUDIO) MyDataRange)->MaximumBitsPerSample);

    // Fill in the rest of the format
    pWfxExt->Format.nBlockAlign = 
        (pWfxExt->Format.wBitsPerSample * pWfxExt->Format.nChannels) / 8;
    pWfxExt->Format.nAvgBytesPerSec = 
        pWfxExt->Format.nSamplesPerSec * pWfxExt->Format.nBlockAlign;
    pWfxExt->Format.cbSize = 22;
    pWfxExt->Samples.wValidBitsPerSample = pWfxExt->Format.wBitsPerSample;
    pWfxExt->SubFormat = KSDATAFORMAT_SUBTYPE_PCM;
    pWfxExt->Format.wFormatTag = WAVE_FORMAT_EXTENSIBLE;

    // Determine the appropriate channel config to use.
    switch(pWfxExt->Format.nChannels)
    {
        case 1:
            pWfxExt->dwChannelMask = KSAUDIO_SPEAKER_MONO;
            break;
        case 2:
            pWfxExt->dwChannelMask = KSAUDIO_SPEAKER_STEREO;
            break;
        case 4:
            // Since there are two 4-channel speaker configs, make sure
            // the appropriate one is used.  If neither is set, default
            // to KSAUDIO_SPEAKER_QUAD.
            if(m_ChannelConfig.ActiveSpeakerPositions == KSAUDIO_SPEAKER_SURROUND)
            {
                pWfxExt->dwChannelMask = KSAUDIO_SPEAKER_SURROUND;
            }
            else
            {
                pWfxExt->dwChannelMask = KSAUDIO_SPEAKER_QUAD;
            }
            break;
        case 6:
            // Since there are two 6-channel speaker configs, make sure
            // the appropriate one is used.  If neither is set, default
            // to KSAUDIO_SPEAKER_5PIONT1_SURROUND.
            if(m_ChannelConfig.ActiveSpeakerPositions == KSAUDIO_SPEAKER_5POINT1)
            {
                pWfxExt->dwChannelMask = KSAUDIO_SPEAKER_5POINT1;
            }
            else
            {
                pWfxExt->dwChannelMask = KSAUDIO_SPEAKER_5POINT1_SURROUND;
            }
            break;
        case 8:
            // Since there are two 8-channel speaker configs, make sure
            // the appropriate one is used.  If neither is set, default
            // to KSAUDIO_SPEAKER_7POINT1_SURROUND.
            if(m_ChannelConfig.ActiveSpeakerPositions == KSAUDIO_SPEAKER_7POINT1)
            {
                pWfxExt->dwChannelMask = KSAUDIO_SPEAKER_7POINT1;
            }
            else
            {
                pWfxExt->dwChannelMask = KSAUDIO_SPEAKER_7POINT1_SURROUND;
            }
            break;
        default:
            // Unsupported channel count.
            return STATUS_NO_MATCH;
    }
    
    // Now overwrite also the sample size in the ksdataformat structure.
    ((PKSDATAFORMAT)ResultantFormat)->SampleSize = pWfxExt->Format.nBlockAlign;
    
    // That we will return.
    //
    *ResultantFormatLength = sizeof(KSDATAFORMAT) + sizeof(WAVEFORMATPCMEX);
    
    return STATUS_SUCCESS;
} // DataRangeIntersection

//=============================================================================
STDMETHODIMP_(NTSTATUS)
CMiniportWaveCyclic::GetDescription
( 
    _Out_ PPCFILTER_DESCRIPTOR * OutFilterDescriptor 
)
/*++

Routine Description:

  The GetDescription function gets a pointer to a filter description. 
  It provides a location to deposit a pointer in miniport's description 
  structure. This is the placeholder for the FromNode or ToNode fields in 
  connections which describe connections to the filter's pins. 

Arguments:

  OutFilterDescriptor - Pointer to the filter description. 

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(OutFilterDescriptor);

    return 
        CMiniportWaveCyclicMSVAD::GetDescription(OutFilterDescriptor);
} // GetDescription

//=============================================================================
STDMETHODIMP_(NTSTATUS)
CMiniportWaveCyclic::Init
( 
    _In_  PUNKNOWN                UnknownAdapter_,
    _In_  PRESOURCELIST           ResourceList_,
    _In_  PPORTWAVECYCLIC         Port_ 
)
/*++

Routine Description:

  The Init function initializes the miniport. Callers of this function 
  should run at IRQL PASSIVE_LEVEL

Arguments:

  UnknownAdapter - A pointer to the Iuknown interface of the adapter object. 

  ResourceList - Pointer to the resource list to be supplied to the miniport 
                 during initialization. The port driver is free to examine the 
                 contents of the ResourceList. The port driver will not be 
                 modify the ResourceList contents. 

  Port - Pointer to the topology port object that is linked with this miniport. 

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(UnknownAdapter_);
    ASSERT(Port_);

    NTSTATUS                    ntStatus;

    DPF_ENTER(("[CMiniportWaveCyclic::Init]"));

    m_MaxOutputStreams                      = MAX_OUTPUT_STREAMS;
    m_MaxInputStreams                       = MAX_INPUT_STREAMS;
    m_MaxTotalStreams                       = MAX_TOTAL_STREAMS;

    m_MinChannels                           = MIN_CHANNELS;
    m_MaxChannelsPcm                        = MAX_CHANNELS_PCM;

    m_MinBitsPerSamplePcm                   = MIN_BITS_PER_SAMPLE_PCM;
    m_MaxBitsPerSamplePcm                   = MAX_BITS_PER_SAMPLE_PCM;
    m_MinSampleRatePcm                      = MIN_SAMPLE_RATE;
    m_MaxSampleRatePcm                      = MAX_SAMPLE_RATE;

    m_ChannelConfig.ActiveSpeakerPositions  = KSAUDIO_SPEAKER_STEREO;
    
    ntStatus =
        CMiniportWaveCyclicMSVAD::Init
        (
            UnknownAdapter_,
            ResourceList_,
            Port_
        );
    if (NT_SUCCESS(ntStatus))
    {
        // Set filter descriptor.
        m_FilterDescriptor = &MiniportFilterDescriptor;

        m_fCaptureAllocated = FALSE;
        m_fRenderAllocated = FALSE;
    }

    return ntStatus;
} // Init

//=============================================================================
_Use_decl_annotations_
STDMETHODIMP_(NTSTATUS)
CMiniportWaveCyclic::NewStream
( 
    PMINIPORTWAVECYCLICSTREAM * OutStream,
    PUNKNOWN                OuterUnknown,
    POOL_TYPE               PoolType,
    ULONG                   Pin,
    BOOLEAN                 Capture,
    PKSDATAFORMAT           DataFormat,
    PDMACHANNEL *           OutDmaChannel,
    PSERVICEGROUP *         OutServiceGroup 
)
/*++

Routine Description:

  The NewStream function creates a new instance of a logical stream 
  associated with a specified physical channel. Callers of NewStream should 
  run at IRQL PASSIVE_LEVEL.

Arguments:

  OutStream -

  OuterUnknown -

  PoolType - 

  Pin - 

  Capture - 

  DataFormat -

  OutDmaChannel -

  OutServiceGroup -

Return Value:

  NT status code.

--*/
{
    UNREFERENCED_PARAMETER(PoolType);

    PAGED_CODE();

    ASSERT(OutStream);
    ASSERT(DataFormat);
    ASSERT(OutDmaChannel);
    ASSERT(OutServiceGroup);

    DPF_ENTER(("[CMiniportWaveCyclic::NewStream]"));

    NTSTATUS                    ntStatus = STATUS_SUCCESS;
    PCMiniportWaveCyclicStream  stream = NULL;

    // Check if we have enough streams.
    if (Capture)
    {
        if (m_fCaptureAllocated)
        {
            DPF(D_TERSE, ("[Only one capture stream supported]"));
            ntStatus = STATUS_INSUFFICIENT_RESOURCES;
        }
    }
    else
    {
        if (m_fRenderAllocated)
        {
            DPF(D_TERSE, ("[Only one render stream supported]"));
            ntStatus = STATUS_INSUFFICIENT_RESOURCES;
        }
    }

    // Determine if the format is valid.
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = ValidateFormat(DataFormat);
    }

    // Instantiate a stream. Stream must be in
    // NonPagedPool because of file saving.
    //
    if (NT_SUCCESS(ntStatus))
    {
        stream = new (NonPagedPool, MSVAD_POOLTAG) 
            CMiniportWaveCyclicStream(OuterUnknown);

        if (stream)
        {
            stream->AddRef();

            ntStatus = 
                stream->Init
                ( 
                    this,
                    Pin,
                    Capture,
                    DataFormat
                );
        }
        else
        {
            ntStatus = STATUS_INSUFFICIENT_RESOURCES;
        }
    }

    if (NT_SUCCESS(ntStatus))
    {
        if (Capture)
        {
            m_fCaptureAllocated = TRUE;
        }
        else
        {
            m_fRenderAllocated = TRUE;
        }

        *OutStream = PMINIPORTWAVECYCLICSTREAM(stream);
        (*OutStream)->AddRef();
        
        *OutDmaChannel = PDMACHANNEL(stream);
        (*OutDmaChannel)->AddRef();

        *OutServiceGroup = m_ServiceGroup;
        (*OutServiceGroup)->AddRef();

        // The stream, the DMA channel, and the service group have
        // references now for the caller.  The caller expects these
        // references to be there.
    }

    // This is our private reference to the stream.  The caller has
    // its own, so we can release in any case.
    //
    if (stream)
    {
        stream->Release();
    }
    
    return ntStatus;
} // NewStream

//=============================================================================
STDMETHODIMP_(NTSTATUS)
CMiniportWaveCyclic::NonDelegatingQueryInterface
( 
    _In_         REFIID  Interface,
    _COM_Outptr_ PVOID * Object 
)
/*++

Routine Description:

  QueryInterface

Arguments:

  Interface - GUID

  Object - interface pointer to be returned.

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(Object);

    if (IsEqualGUIDAligned(Interface, IID_IUnknown))
    {
        *Object = PVOID(PUNKNOWN(PMINIPORTWAVECYCLIC(this)));
    }
    else if (IsEqualGUIDAligned(Interface, IID_IMiniport))
    {
        *Object = PVOID(PMINIPORT(this));
    }
    else if (IsEqualGUIDAligned(Interface, IID_IMiniportWaveCyclic))
    {
        *Object = PVOID(PMINIPORTWAVECYCLIC(this));
    }
    else
    {
        *Object = NULL;
    }

    if (*Object)
    {
        // We reference the interface for the caller.

        PUNKNOWN(*Object)->AddRef();
        return STATUS_SUCCESS;
    }

    return STATUS_INVALID_PARAMETER;
} // NonDelegatingQueryInterface

//=============================================================================
NTSTATUS                            
CMiniportWaveCyclic::PropertyHandlerGeneric
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

    DPF_ENTER(("[CMiniportWaveCyclic::PropertyHandlerChannelConfig]"));

    ASSERT(PropertyRequest);
    ASSERT(PropertyRequest->PropertyItem);

    NTSTATUS    ntStatus = STATUS_INVALID_DEVICE_REQUEST;

    switch (PropertyRequest->PropertyItem->Id)
    {
        case KSPROPERTY_AUDIO_CHANNEL_CONFIG:
            // This miniport will handle channel config property
            // requests.
            ntStatus = PropertyHandlerChannelConfig(PropertyRequest);
            break;
        default:
            // All other property requests are handled by the base
            // miniport.
            ntStatus = 
                CMiniportWaveCyclicMSVAD::PropertyHandlerGeneric
                (
                    PropertyRequest
                );
    }

    return ntStatus;
} // PropertyHandlerGeneric

//=============================================================================
NTSTATUS                    
CMiniportWaveCyclic::PropertyHandlerChannelConfig
(
    IN  PPCPROPERTY_REQUEST     PropertyRequest
)
/*++

Routine Description:

  Handles the KSPROPERTY_AUDIO_CHANNEL_CONFIG property requests.

Arguments:

  PropertyRequest - property request structure

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    NTSTATUS                    ntStatus = STATUS_INVALID_DEVICE_REQUEST;

    DPF_ENTER(("[CMiniportWaveCyclic::PropertyHandlerChannelConfig]"));

    // Validate the property request structure.
    ntStatus = ValidatePropertyParams(PropertyRequest, sizeof(KSAUDIO_CHANNEL_CONFIG), 0);
    if(!NT_SUCCESS(ntStatus))
    {
        return ntStatus;
    }

    // Get the KSAUDIO_CHANNEL_CONFIG structure.
    KSAUDIO_CHANNEL_CONFIG *value = static_cast<KSAUDIO_CHANNEL_CONFIG*>(PropertyRequest->Value);

    if (PropertyRequest->Verb & KSPROPERTY_TYPE_GET)
    {
        // Store the current channel config in the return structure.
        *value = m_ChannelConfig;
        PropertyRequest->ValueSize = sizeof(KSAUDIO_CHANNEL_CONFIG);
    }
    else if (PropertyRequest->Verb & KSPROPERTY_TYPE_SET)
    {
        // Limit the channel mask based on the maximum supported channel
        // count.
        //
        switch(value->ActiveSpeakerPositions)
        {
            case KSAUDIO_SPEAKER_MONO:
                break;
            case KSAUDIO_SPEAKER_STEREO:
                if (m_MaxChannelsPcm >= 2)
                {
                    value->ActiveSpeakerPositions = KSAUDIO_SPEAKER_STEREO;
                    break;
                }
                return STATUS_NOT_SUPPORTED;
            case KSAUDIO_SPEAKER_QUAD:
                if (m_MaxChannelsPcm >= 4)
                {
                    value->ActiveSpeakerPositions = KSAUDIO_SPEAKER_QUAD;
                    break;
                }
                return STATUS_NOT_SUPPORTED;
            case KSAUDIO_SPEAKER_SURROUND:
                if (m_MaxChannelsPcm >= 4)
                {
                    value->ActiveSpeakerPositions = KSAUDIO_SPEAKER_SURROUND;
                    break;
                }
                return STATUS_NOT_SUPPORTED;
            case KSAUDIO_SPEAKER_5POINT1:
                if (m_MaxChannelsPcm >= 6)
                {
                    value->ActiveSpeakerPositions = KSAUDIO_SPEAKER_5POINT1;
                    break;
                }
                return STATUS_NOT_SUPPORTED;
            case KSAUDIO_SPEAKER_5POINT1_SURROUND:
                if (m_MaxChannelsPcm >= 6)
                {
                    value->ActiveSpeakerPositions = KSAUDIO_SPEAKER_5POINT1_SURROUND;
                    break;
                }
                return STATUS_NOT_SUPPORTED;
            case KSAUDIO_SPEAKER_7POINT1_SURROUND:
                if (m_MaxChannelsPcm >= 8)
                {
                    value->ActiveSpeakerPositions = KSAUDIO_SPEAKER_7POINT1_SURROUND;
                    break;
                }
                return STATUS_NOT_SUPPORTED;
            case KSAUDIO_SPEAKER_7POINT1:
                if (m_MaxChannelsPcm >= 8)
                {
                    value->ActiveSpeakerPositions = KSAUDIO_SPEAKER_7POINT1;
                    break;
                }
                return STATUS_NOT_SUPPORTED;
            default:
                DPF(D_TERSE, ("[Channel Mask not supported]"));
                return STATUS_NOT_SUPPORTED;
        }

        // Store the new channel mask.
        m_ChannelConfig = *value;
    }
    
    return ntStatus;
} // PropertyHandlerChannelConfig

//=============================================================================
NTSTATUS
CMiniportWaveCyclic::ValidateFormat
( 
    IN  PKSDATAFORMAT           pDataFormat 
)
/*++

Routine Description:

  Validates that the given dataformat is valid. This is for supporting 
  WAVEFORMATEXTENSIBLE.

Arguments:

  pDataFormat - The dataformat for validation.

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(pDataFormat);

    DPF_ENTER(("[CMiniportWaveCyclicMSVAD::ValidateFormat]"));

    NTSTATUS                    ntStatus;
    PWAVEFORMATEX               pwfx;

    // Let the default Validator handle the request.
    //
    ntStatus = CMiniportWaveCyclicMSVAD::ValidateFormat(pDataFormat);
    if (NT_SUCCESS(ntStatus))
    {
        return ntStatus;
    }

    // If the format is not known check for WAVEFORMATEXTENSIBLE.
    //
    pwfx = GetWaveFormatEx(pDataFormat);
    if (pwfx)
    {
        if (IS_VALID_WAVEFORMATEX_GUID(&pDataFormat->SubFormat))
        {
            USHORT wfxID = EXTRACT_WAVEFORMATEX_ID(&pDataFormat->SubFormat);

            switch (wfxID)
            {
                // This is for WAVE_FORMAT_EXTENSIBLE support.
                //
                case WAVE_FORMAT_PCM:
                {
                    switch (pwfx->wFormatTag)
                    {
                        case WAVE_FORMAT_EXTENSIBLE:
                        {
                            PWAVEFORMATEXTENSIBLE   pwfxExt = 
                                (PWAVEFORMATEXTENSIBLE) pwfx;
                            ntStatus = ValidateWfxExt(pwfxExt);
                            break;
                        }
                    }
                    break;
                }
                

                default:
                    DPF(D_TERSE, ("Invalid format EXTRACT_WAVEFORMATEX_ID!"));
                    break;
            }
        }
        else
        {
            DPF(D_TERSE, ("Invalid pDataFormat->SubFormat!") );
        }
    }

    return ntStatus;
} // ValidateFormat

//=============================================================================
NTSTATUS
CMiniportWaveCyclic::ValidateWfxExt
(
    IN  PWAVEFORMATEXTENSIBLE   pWfxExt
)
/*++

Routine Description:

  Given a waveformatextensible, verifies that the format is in device
  datarange.  Note that the driver assumes that
  all values within the minimum and maximum values for sample rate, 
  bit depth, and channel count are valid as opposed to just discrete
  values.

Arguments:

  pWfxExt - wave format extensible structure

Return Value:
    
    NT status code.

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[CMiniportWaveCyclic::ValidateWfxExtPcm]"));

    // First verify that the subformat is OK
    //
    if (pWfxExt)
    {
        if(IsEqualGUIDAligned(pWfxExt->SubFormat, KSDATAFORMAT_SUBTYPE_PCM))
        {
            PWAVEFORMATEX           pWfx = (PWAVEFORMATEX) pWfxExt;

            // Then verify that the format parameters are supported.
            if
            (
                pWfx                                                &&
                (pWfx->cbSize == sizeof(WAVEFORMATEXTENSIBLE) - 
                    sizeof(WAVEFORMATEX))                           &&
                (pWfx->nChannels >= m_MinChannels)                  &&
                (pWfx->nChannels <= m_MaxChannelsPcm)               &&
                (pWfx->nSamplesPerSec >= m_MinSampleRatePcm)        &&
                (pWfx->nSamplesPerSec <= m_MaxSampleRatePcm)        &&
                (pWfx->wBitsPerSample >= m_MinBitsPerSamplePcm)     &&
                (pWfx->wBitsPerSample <= m_MaxBitsPerSamplePcm)
            )
            {
                return STATUS_SUCCESS;
            }
        }
    }

    DPF(D_TERSE, ("Invalid PCM format"));

    return STATUS_INVALID_PARAMETER;
} // ValidateWfxExtPcm

//=============================================================================
// CMiniportWaveStreamCyclicSimple
//=============================================================================

//=============================================================================
CMiniportWaveCyclicStream::~CMiniportWaveCyclicStream
( 
    void 
)
/*++

Routine Description:

  Destructor for wavecyclicstream 

Arguments:

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[CMiniportWaveCyclicStream::~CMiniportWaveCyclicStream]"));

    if (NULL != m_pMiniportLocal)
    {
        if (m_fCapture)
        {
            m_pMiniportLocal->m_fCaptureAllocated = FALSE;
        }
        else
        {
            m_pMiniportLocal->m_fRenderAllocated = FALSE;
        }
    }
} // ~CMiniportWaveCyclicStream

//=============================================================================
NTSTATUS
CMiniportWaveCyclicStream::Init
( 
    IN PCMiniportWaveCyclic         Miniport_,
    IN ULONG                        Pin_,
    IN BOOLEAN                      Capture_,
    IN PKSDATAFORMAT                DataFormat_
)
/*++

Routine Description:

  Initializes the stream object. Allocate a DMA buffer, timer and DPC

Arguments:

  Miniport_ -

  Pin_ -

  Capture_ -

  DataFormat -

  DmaChannel_ -

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    m_pMiniportLocal = Miniport_;

    return 
        CMiniportWaveCyclicStreamMSVAD::Init
        (
            Miniport_,
            Pin_,
            Capture_,
            DataFormat_
        );
} // Init

//=============================================================================
STDMETHODIMP_(NTSTATUS)
CMiniportWaveCyclicStream::NonDelegatingQueryInterface
( 
    _In_         REFIID  Interface,
    _COM_Outptr_ PVOID * Object 
)
/*++

Routine Description:

  QueryInterface

Arguments:

  Interface - GUID

  Object - interface pointer to be returned

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(Object);

    if (IsEqualGUIDAligned(Interface, IID_IUnknown))
    {
        *Object = PVOID(PUNKNOWN(PMINIPORTWAVECYCLICSTREAM(this)));
    }
    else if (IsEqualGUIDAligned(Interface, IID_IMiniportWaveCyclicStream))
    {
        *Object = PVOID(PMINIPORTWAVECYCLICSTREAM(this));
    }
    else if (IsEqualGUIDAligned(Interface, IID_IDmaChannel))
    {
        *Object = PVOID(PDMACHANNEL(this));
    }
    else
    {
        *Object = NULL;
    }

    if (*Object)
    {
        PUNKNOWN(*Object)->AddRef();
        return STATUS_SUCCESS;
    }

    return STATUS_INVALID_PARAMETER;
} // NonDelegatingQueryInterface

//=============================================================================
NTSTATUS
PropertyHandler_Wave
( 
    IN PPCPROPERTY_REQUEST      PropertyRequest 
)
/*++

Routine Description:

  Redirects property request to miniport object

Arguments:

  PropertyRequest  

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(PropertyRequest);

    DPF_ENTER(("[PropertyHandler_Wave]"));

    return ((PCMiniportWaveCyclic)
        (PropertyRequest->MajorTarget))->PropertyHandlerGeneric
        (
            PropertyRequest
        );
} // PropertyHandler_Topology
#pragma code_seg()

