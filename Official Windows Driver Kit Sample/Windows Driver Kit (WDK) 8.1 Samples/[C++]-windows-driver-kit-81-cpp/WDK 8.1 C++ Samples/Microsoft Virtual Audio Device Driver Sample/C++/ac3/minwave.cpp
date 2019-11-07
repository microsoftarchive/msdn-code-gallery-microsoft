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
#include "ac3.h"
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
    _In_        ULONG                       PinId,
    _In_        PKSDATARANGE                ClientDataRange,
    _In_        PKSDATARANGE                MyDataRange,
    _In_        ULONG                       OutputBufferLength,
    _Out_writes_bytes_to_opt_(OutputBufferLength, *ResultantFormatLength)
                PVOID                       ResultantFormat,
    _Out_       PULONG                      ResultantFormatLength 
)
/*++

Routine Description:

  The DataRangeIntersection function determines the highest quality 
  intersection of two data ranges.

Arguments:

  PinId -           Pin for which data intersection is being determined. 

  ClientDataRange - Pointer to KSDATARANGE structure which contains the data 
                    range submitted by client in the data range intersection 
                    property request. 

  MyDataRange -         Pin's data range to be compared with client's data 
                        range. In this case we actually ignore our own data 
                        range, because we know that we only support one range.

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
    UNREFERENCED_PARAMETER(MyDataRange);

    PAGED_CODE();
    WORD    wFormatTag = WAVE_FORMAT_PCM;

    // PCM is supported on all host pins but AC3 is allowed only on the SPDIF host pins
    if (IsEqualGUIDAligned(ClientDataRange->SubFormat, 
            KSDATAFORMAT_SUBTYPE_DOLBY_AC3_SPDIF))
    {
        wFormatTag = WAVE_FORMAT_DOLBY_AC3_SPDIF;

        if ((KSPIN_WAVE_SPDIF_CAPTURE_HOST != PinId) &&
            (KSPIN_WAVE_SPDIF_RENDER_HOST != PinId))
        {
            return STATUS_NO_MATCH;
        }
    }
    else
    if (!IsEqualGUIDAligned(ClientDataRange->SubFormat,
            KSDATAFORMAT_SUBTYPE_PCM) &&
        !IsEqualGUIDAligned(ClientDataRange->SubFormat,
            KSDATAFORMAT_SUBTYPE_WILDCARD))
    {
        return STATUS_NO_MATCH;
    }

    if (!IsEqualGUIDAligned(ClientDataRange->MajorFormat, 
            KSDATAFORMAT_TYPE_AUDIO) &&
        !IsEqualGUIDAligned(ClientDataRange->MajorFormat, 
            KSDATAFORMAT_TYPE_WILDCARD))
    {
        return STATUS_NO_MATCH;
    }

    if (IsEqualGUIDAligned(ClientDataRange->Specifier, 
            KSDATAFORMAT_SPECIFIER_WAVEFORMATEX) ||
        IsEqualGUIDAligned(ClientDataRange->Specifier,
            KSDATAFORMAT_SPECIFIER_WILDCARD))
    {
        *ResultantFormatLength = sizeof(KSDATAFORMAT_WAVEFORMATEX);
    }
    else if (IsEqualGUIDAligned(ClientDataRange->Specifier, 
        KSDATAFORMAT_SPECIFIER_DSOUND))
    {
        *ResultantFormatLength = sizeof(KSDATAFORMAT_DSOUND);
    }
    else
    {
        return STATUS_NO_MATCH;
    }

    // Validate return buffer size, if the request is only for the
    // size of the resultant structure, return it now.
    //
    if (!OutputBufferLength || !ResultantFormat) 
    {
        *ResultantFormatLength = sizeof(KSDATAFORMAT_WAVEFORMATEX);
        return STATUS_BUFFER_OVERFLOW;
    } 
    else if (OutputBufferLength < sizeof(KSDATAFORMAT_WAVEFORMATEX)) 
    {
        return STATUS_BUFFER_TOO_SMALL;
    }
    
    PKSDATAFORMAT_WAVEFORMATEX  resultantFormatWFX;
    PWAVEFORMATEX               pWaveFormatEx;

    resultantFormatWFX = (PKSDATAFORMAT_WAVEFORMATEX) ResultantFormat;

    // Return the best (only) available format.
    //
    resultantFormatWFX->DataFormat.FormatSize   = *ResultantFormatLength;
    resultantFormatWFX->DataFormat.Flags        = 0;
    resultantFormatWFX->DataFormat.SampleSize   = 4; // must match nBlockAlign
    resultantFormatWFX->DataFormat.Reserved     = 0;

    resultantFormatWFX->DataFormat.MajorFormat  = KSDATAFORMAT_TYPE_AUDIO;
    INIT_WAVEFORMATEX_GUID(&resultantFormatWFX->DataFormat.SubFormat,
        WAVE_FORMAT_DOLBY_AC3_SPDIF );

    // Extra space for the DSound specifier
    //
    if (IsEqualGUIDAligned(ClientDataRange->Specifier,
            KSDATAFORMAT_SPECIFIER_DSOUND))
    {
        PKSDATAFORMAT_DSOUND        resultantFormatDSound;
        resultantFormatDSound = (PKSDATAFORMAT_DSOUND)    ResultantFormat;

        resultantFormatDSound->DataFormat.Specifier = 
            KSDATAFORMAT_SPECIFIER_DSOUND;

        // DSound format capabilities are not expressed 
        // this way in KS, so we express no capabilities. 
        //
        resultantFormatDSound->BufferDesc.Flags = 0 ;
        resultantFormatDSound->BufferDesc.Control = 0 ;

        pWaveFormatEx = &resultantFormatDSound->BufferDesc.WaveFormatEx;
    }
    else  // WAVEFORMATEX or WILDCARD (WAVEFORMATEX)
    {
        resultantFormatWFX->DataFormat.Specifier = 
            KSDATAFORMAT_SPECIFIER_WAVEFORMATEX;

        pWaveFormatEx = (PWAVEFORMATEX)((PKSDATAFORMAT)resultantFormatWFX + 1);
    }
    pWaveFormatEx->wFormatTag      = wFormatTag;
    pWaveFormatEx->nChannels       = 2;
    pWaveFormatEx->nSamplesPerSec  = 48000;
    pWaveFormatEx->wBitsPerSample  = 16;
    pWaveFormatEx->cbSize          = 0;
    pWaveFormatEx->nBlockAlign     = 4;
    pWaveFormatEx->nAvgBytesPerSec = 192000;

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

    m_MaxOutputStreams      = MAX_OUTPUT_STREAMS;
    m_MaxInputStreams       = MAX_INPUT_STREAMS;
    m_MaxTotalStreams       = MAX_TOTAL_STREAMS;

    m_MinChannels           = MIN_CHANNELS;
    m_MaxChannelsPcm        = MAX_CHANNELS;

    m_MinBitsPerSamplePcm   = MIN_BITS_PER_SAMPLE;
    m_MaxBitsPerSamplePcm   = MAX_BITS_PER_SAMPLE;
    m_MinSampleRatePcm      = MIN_SAMPLE_RATE;
    m_MaxSampleRatePcm      = MAX_SAMPLE_RATE;

    RtlZeroMemory(m_arInstanceCounts, sizeof(m_arInstanceCounts));
    
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

    // Check if have available streams
    if (m_arInstanceCounts[Pin] != 0)
    {
        ntStatus = STATUS_INSUFFICIENT_RESOURCES;
    }

    // Determine if the format is valid.
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = ValidateFormat(Pin, DataFormat);
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
        m_arInstanceCounts[Pin] = 1;

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
CMiniportWaveCyclic::ValidateFormat
(
    IN  ULONG               nPin,
    IN  PKSDATAFORMAT       pDataFormat 
)
/*++

Routine Description:

  Validates that the given dataformat is valid. This overwrites BaseWave's
  ValidateFormat and includes checks for AC3 format.

Arguments:

  pDataFormat - The dataformat for validation.

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[CMiniportWaveCyclic::ValidateFormat]"));

    NTSTATUS        ntStatus = STATUS_INVALID_PARAMETER;
    PWAVEFORMATEX   pwfx;

    pwfx = GetWaveFormatEx(pDataFormat);
    if (pwfx)
    {
        if (IS_VALID_WAVEFORMATEX_GUID(&pDataFormat->SubFormat))
        {
            USHORT wfxID = EXTRACT_WAVEFORMATEX_ID(&pDataFormat->SubFormat);

            if ((wfxID == WAVE_FORMAT_PCM) ||
                ((wfxID == WAVE_FORMAT_DOLBY_AC3_SPDIF) &&
                ((nPin == KSPIN_WAVE_SPDIF_CAPTURE_HOST) || (nPin == KSPIN_WAVE_SPDIF_RENDER_HOST))))
            {
                if
                (
                    (pDataFormat->FormatSize == sizeof(KSDATAFORMAT_WAVEFORMATEX)) &&
                    (pwfx->cbSize == 0)                              &&
                    (pwfx->nChannels <= MAX_CHANNELS )           &&
                    (pwfx->wBitsPerSample >= MIN_BITS_PER_SAMPLE)&&
                    (pwfx->wBitsPerSample <= MAX_BITS_PER_SAMPLE)&&
                    (pwfx->nSamplesPerSec >= MIN_SAMPLE_RATE)    &&
                    (pwfx->nSamplesPerSec <= MAX_SAMPLE_RATE)
                )
                {
                    ntStatus = STATUS_SUCCESS;
                }
                else
                {
                    DPF(D_TERSE, ("Invalid format"));
                }
            }
            else
            {
                DPF(D_TERSE, ("Invalid format tag"));
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
        m_pMiniportLocal->m_arInstanceCounts[m_ulPin] = 0;
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
#pragma code_seg()

