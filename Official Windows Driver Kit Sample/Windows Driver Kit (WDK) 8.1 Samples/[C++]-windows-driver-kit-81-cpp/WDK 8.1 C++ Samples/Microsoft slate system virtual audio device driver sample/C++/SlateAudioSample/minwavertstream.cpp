#include <sysvad.h>
#include <limits.h>
#include <ks.h>
#include "simple.h"
#include "minwavert.h"
#include "minwavertstream.h"
#include "UnittestData.h"
#define MINWAVERTSTREAM_POOLTAG 'SRWM'
#define HNSTIME_PER_MILLISECOND 10000

#pragma warning (disable : 4127)

//=============================================================================
// CMiniportWaveRTStream
//=============================================================================

//=============================================================================
#pragma code_seg("PAGE")
CMiniportWaveRTStream::~CMiniportWaveRTStream
( 
    void 
)
/*++

Routine Description:

  Destructor for wavertstream 

Arguments:

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();
    if (NULL != m_pMiniport)
    {
        if (m_bUnregisterStream)
        {
            m_pMiniport->StreamClosed(m_ulPin, this);
            m_bUnregisterStream = FALSE;
        }
        
        m_pMiniport->Release();
        m_pMiniport = NULL;
    }

    if (m_pDpc)
    {
        ExFreePoolWithTag( m_pDpc, MINWAVERTSTREAM_POOLTAG );
        m_pDpc = NULL;
    }

    if (m_pTimer)
    {
        ExFreePoolWithTag( m_pTimer, MINWAVERTSTREAM_POOLTAG );
        m_pTimer = NULL;
    }

    if (m_pbMuted)
    {
        ExFreePoolWithTag( m_pbMuted, MINWAVERTSTREAM_POOLTAG );
        m_pbMuted = NULL;
    }

    if (m_plVolumeLevel)
    {
        ExFreePoolWithTag( m_plVolumeLevel, MINWAVERTSTREAM_POOLTAG );
        m_plVolumeLevel = NULL;
    }

    if (m_plPeakMeter)
    {
        ExFreePoolWithTag( m_plPeakMeter, MINWAVERTSTREAM_POOLTAG );
        m_plPeakMeter = NULL;
    }

    if (m_pWfExt)
    {
        ExFreePoolWithTag( m_pWfExt, MINWAVERTSTREAM_POOLTAG );
        m_pWfExt = NULL;
    }
    if (m_pNotificationTimer)
    {
        KeCancelTimer(m_pNotificationTimer);
        ExFreePoolWithTag(m_pNotificationTimer, MINWAVERTSTREAM_POOLTAG);
    }

    // Since we just cancelled the notification timer, wait for all queued 
    // DPCs to complete before we free the notification DPC.
    //
    KeFlushQueuedDpcs();

    if (m_pNotificationDpc)
    {
        ExFreePoolWithTag( m_pNotificationDpc, MINWAVERTSTREAM_POOLTAG );
    }
    
#ifdef SYSVAD_BTH_BYPASS
    ASSERT(m_ScoOpen == FALSE);
#endif  // SYSVAD_BTH_BYPASS

    DPF_ENTER(("[CMiniportWaveRTStream::~CMiniportWaveRTStream]"));
} // ~CMiniportWaveRTStream

//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS
CMiniportWaveRTStream::Init
( 
    _In_ PCMiniportWaveRT           Miniport_,
    _In_ PPORTWAVERTSTREAM          PortStream_,
    _In_ ULONG                      Pin_,
    _In_ BOOLEAN                    Capture_,
    _In_ PKSDATAFORMAT              DataFormat_,
    _In_ GUID                       SignalProcessingMode
)
/*++

Routine Description:

  Initializes the stream object.

Arguments:

  Miniport_ -

  Pin_ -

  Capture_ -

  DataFormat -

  SignalProcessingMode - The driver uses the signalProcessingMode to configure
    driver and/or hardware specific signal processing to be applied to this new
    stream.

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    PWAVEFORMATEX pWfEx = NULL;
    NTSTATUS ntStatus = STATUS_SUCCESS;

    m_pMiniport = NULL;
    m_ulPin = 0;
    m_bUnregisterStream = FALSE;
    m_bCapture = FALSE;
    m_ulDmaBufferSize = 0;
    m_pDmaBuffer = NULL;
    m_KsState = KSSTATE_STOP;
    m_pTimer = NULL;
    m_pDpc = NULL;
    m_ullPlayPosition = 0;
    m_ullWritePosition = 0;
    m_ullDmaTimeStamp = 0;
    m_hnsElapsedTimeCarryForward = 0;
    m_ulDmaMovementRate = 0;
    m_bLfxEnabled = FALSE;
    m_pbMuted = NULL;
    m_plVolumeLevel = NULL;
    m_plPeakMeter = NULL;
    m_pWfExt = NULL;
    m_ullLinearPosition = 0;
    m_ulContentId = 0;
    m_ulCurrentWritePosition = 0;
    m_IsCurrentWritePositionUpdated = 0;
    
#ifdef SYSVAD_BTH_BYPASS
    m_ScoOpen = FALSE;
#endif  // SYSVAD_BTH_BYPASS

    m_pPortStream = PortStream_;
    InitializeListHead(&m_NotificationList);
    m_ulNotificationIntervalMs = 0;

    m_pNotificationDpc = (PRKDPC)ExAllocatePoolWithTag(
        NonPagedPoolNx,
        sizeof(KDPC),
        MINWAVERTSTREAM_POOLTAG);
    if (!m_pNotificationDpc)
    {
        return STATUS_INSUFFICIENT_RESOURCES;
    }

    m_pNotificationTimer = (PKTIMER)ExAllocatePoolWithTag(
        NonPagedPoolNx,
        sizeof(KTIMER),
        MINWAVERTSTREAM_POOLTAG);
    if (!m_pNotificationTimer)
    {
        return STATUS_INSUFFICIENT_RESOURCES;
    }

    KeInitializeDpc(m_pNotificationDpc, TimerNotifyRT, this);
    KeInitializeTimerEx(m_pNotificationTimer, NotificationTimer);

    pWfEx = GetWaveFormatEx(DataFormat_);
    if (NULL == pWfEx) 
    { 
        return STATUS_UNSUCCESSFUL; 
    }

    m_pMiniport = reinterpret_cast<CMiniportWaveRT*>(Miniport_);
    if (m_pMiniport == NULL)
    {
        return STATUS_INVALID_PARAMETER;
    }
    m_pMiniport->AddRef();
    if (!NT_SUCCESS(ntStatus))
    {
        return ntStatus;
    }
    m_ulPin = Pin_;
    m_bCapture = Capture_;
    m_ulDmaMovementRate = pWfEx->nAvgBytesPerSec;

    m_pDpc = (PRKDPC)ExAllocatePoolWithTag(NonPagedPoolNx, sizeof(KDPC), MINWAVERTSTREAM_POOLTAG);
    if (!m_pDpc)
    {
        return STATUS_INSUFFICIENT_RESOURCES;
    }

    m_pWfExt = (PWAVEFORMATEXTENSIBLE)ExAllocatePoolWithTag(NonPagedPoolNx, sizeof(WAVEFORMATEX) + pWfEx->cbSize, MINWAVERTSTREAM_POOLTAG);
    if (m_pWfExt == NULL)
    {
        return STATUS_INSUFFICIENT_RESOURCES;
    }
    RtlCopyMemory(m_pWfExt, pWfEx, sizeof(WAVEFORMATEX) + pWfEx->cbSize);

    m_pbMuted = (PBOOL)ExAllocatePoolWithTag(NonPagedPoolNx, m_pWfExt->Format.nChannels * sizeof(BOOL), MINWAVERTSTREAM_POOLTAG);
    if (m_pbMuted == NULL)
    {
        return STATUS_INSUFFICIENT_RESOURCES;
    }
    RtlZeroMemory(m_pbMuted, m_pWfExt->Format.nChannels * sizeof(BOOL));

    m_plVolumeLevel = (PLONG)ExAllocatePoolWithTag(NonPagedPoolNx, m_pWfExt->Format.nChannels * sizeof(LONG), MINWAVERTSTREAM_POOLTAG);
    if (m_plVolumeLevel == NULL)
    {
        return STATUS_INSUFFICIENT_RESOURCES;
    }
    RtlZeroMemory(m_plVolumeLevel, m_pWfExt->Format.nChannels * sizeof(LONG));

    m_plPeakMeter = (PLONG)ExAllocatePoolWithTag(NonPagedPoolNx, m_pWfExt->Format.nChannels * sizeof(LONG), MINWAVERTSTREAM_POOLTAG);
    if (m_plPeakMeter == NULL)
    {
        return STATUS_INSUFFICIENT_RESOURCES;
    }
    RtlZeroMemory(m_plPeakMeter, m_pWfExt->Format.nChannels * sizeof(LONG));

    if (m_bCapture)
    {
        DWORD toneFrequency = 0;

        if (!m_pMiniport->IsRenderDevice())
        {
            //
            // Init sine wave generator. To exercise the SignalProcessingMode parameter
            // this sample driver selects the frequency based on the parameter.
            //
            toneFrequency = IsEqualGUID(SignalProcessingMode, AUDIO_SIGNALPROCESSINGMODE_RAW) ? 1000 : 2000;
        }
        else 
        {
            //
            // Loopbacks pins use a different frequency for test validation.
            //
            ASSERT(Pin_ == m_pMiniport->GetLoopbackPinId());
            toneFrequency = 3000; // 3 kHz
        }
        
        ntStatus = m_ToneGenerator.Init(toneFrequency, m_pWfExt);
        if (!NT_SUCCESS(ntStatus))
        {
            return ntStatus;
        }
    }
    else if (!g_DoNotCreateDataFiles)
    {
        //
        // Create an output file for the render data.
        //
        DPF(D_TERSE, ("SaveData %p", &m_SaveData));
        ntStatus = m_SaveData.SetDataFormat(DataFormat_);
        if (NT_SUCCESS(ntStatus))
        {
            ntStatus = m_SaveData.Initialize(m_pMiniport->IsOffloadSupported() ? (Pin_ == m_pMiniport->GetOffloadPinId()) : FALSE);
        }
    
        if (!NT_SUCCESS(ntStatus))
        {
            return ntStatus;
        }
    }

    //
    // Register this stream.
    //
    ntStatus = m_pMiniport->StreamCreated(m_ulPin, this);
    if (NT_SUCCESS(ntStatus))
    {
        m_bUnregisterStream = TRUE;
    }

    return ntStatus;
} // Init

//=============================================================================
#pragma code_seg("PAGE")
STDMETHODIMP_(NTSTATUS)
CMiniportWaveRTStream::NonDelegatingQueryInterface
( 
    _In_ REFIID  Interface,
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
        *Object = PVOID(PUNKNOWN(PMINIPORTWAVERTSTREAM(this)));
    }
    else if (IsEqualGUIDAligned(Interface, IID_IMiniportWaveRTStream))
    {
        *Object = PVOID(PMINIPORTWAVERTSTREAM(this));
    }
    else if (IsEqualGUIDAligned(Interface, IID_IMiniportWaveRTStreamNotification))
    {
        *Object = PVOID(PMINIPORTWAVERTSTREAMNOTIFICATION(this));
    }
    else if (IsEqualGUIDAligned(Interface, IID_IMiniportStreamAudioEngineNode))
    {
        *Object = (PVOID)(IMiniportStreamAudioEngineNode*)this;
    }
    else if (IsEqualGUIDAligned(Interface, IID_IMiniportStreamAudioEngineNode2))
    {
        *Object = (PVOID)(IMiniportStreamAudioEngineNode2*)this;
    }
    else if (IsEqualGUIDAligned(Interface, IID_IDrmAudioStream))
    {
        *Object = (PVOID)(IDrmAudioStream*)this;
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
#pragma code_seg("PAGE")
NTSTATUS CMiniportWaveRTStream::AllocateBufferWithNotification
(
    _In_    ULONG               NotificationCount_,
    _In_    ULONG               RequestedSize_,
    _Out_   PMDL                *AudioBufferMdl_,
    _Out_   ULONG               *ActualSize_,
    _Out_   ULONG               *OffsetFromFirstPage_,
    _Out_   MEMORY_CACHING_TYPE *CacheType_
)
{
    PAGED_CODE();

    ULONG ulBufferDurationMs = 0;

    if ( (0 == RequestedSize_) || (RequestedSize_ < m_pWfExt->Format.nBlockAlign) )
    { 
        return STATUS_UNSUCCESSFUL; 
    }

    RequestedSize_ -= RequestedSize_ % (m_pWfExt->Format.nBlockAlign);
    
    if (!m_bCapture && !g_DoNotCreateDataFiles)
    {
        NTSTATUS ntStatus;
        
        ntStatus = m_SaveData.SetMaxWriteSize(RequestedSize_);    
        if (!NT_SUCCESS(ntStatus))
        {
            return ntStatus;
        }
    }

    PHYSICAL_ADDRESS highAddress;
    highAddress.HighPart = 0;
    highAddress.LowPart = MAXULONG;

    PMDL pBufferMdl = m_pPortStream->AllocatePagesForMdl (highAddress, RequestedSize_);

    if (NULL == pBufferMdl)
    {
        return STATUS_UNSUCCESSFUL;
    }

    // From MSDN: 
    // "Since the Windows audio stack does not support a mechanism to express memory access 
    //  alignment requirements for buffers, audio drivers must select a caching type for mapped
    //  memory buffers that does not impose platform-specific alignment requirements. In other 
    //  words, the caching type used by the audio driver for mapped memory buffers, must not make 
    //  assumptions about the memory alignment requirements for any specific platform.
    //
    //  This method maps the physical memory pages in the MDL into kernel-mode virtual memory. 
    //  Typically, the miniport driver calls this method if it requires software access to the 
    //  scatter-gather list for an audio buffer. In this case, the storage for the scatter-gather 
    //  list must have been allocated by the IPortWaveRTStream::AllocatePagesForMdl or 
    //  IPortWaveRTStream::AllocateContiguousPagesForMdl method. 
    //
    //  A WaveRT miniport driver should not require software access to the audio buffer itself."
    //   
    m_pDmaBuffer = (BYTE*)m_pPortStream->MapAllocatedPages(pBufferMdl, MmCached);

    m_ulDmaBufferSize = RequestedSize_;
    ulBufferDurationMs = (RequestedSize_ * 1000) / m_ulDmaMovementRate;
    m_ulNotificationIntervalMs = ulBufferDurationMs / NotificationCount_;

    *AudioBufferMdl_ = pBufferMdl;
    *ActualSize_ = RequestedSize_;
    *OffsetFromFirstPage_ = 0;
    *CacheType_ = MmCached;

    return STATUS_SUCCESS;
}

//=============================================================================
#pragma code_seg("PAGE")
VOID CMiniportWaveRTStream::FreeBufferWithNotification
(
    _In_        PMDL    Mdl_,
    _In_        ULONG   Size_
)
{
    UNREFERENCED_PARAMETER(Size_);

    PAGED_CODE();

    if (Mdl_ != NULL)
    {
        if (m_pDmaBuffer != NULL)
        {
            m_pPortStream->UnmapAllocatedPages(m_pDmaBuffer, Mdl_);
            m_pDmaBuffer = NULL;
        }
        
        m_pPortStream->FreePagesFromMdl(Mdl_);
    }
    
    m_ulDmaBufferSize = 0;

    return;
}

//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS CMiniportWaveRTStream::RegisterNotificationEvent
(
    _In_ PKEVENT NotificationEvent_
)
{
    UNREFERENCED_PARAMETER(NotificationEvent_);

    PAGED_CODE();

    NotificationListEntry *nleNew = (NotificationListEntry*)ExAllocatePoolWithTag( 
        NonPagedPoolNx,
        sizeof(NotificationListEntry),
        MINWAVERTSTREAM_POOLTAG);
    if (NULL == nleNew)
    {
        return STATUS_INSUFFICIENT_RESOURCES;
    }

    nleNew->NotificationEvent = NotificationEvent_;

    if (!IsListEmpty(&m_NotificationList))
    {
        PLIST_ENTRY leCurrent = m_NotificationList.Flink;
        while (leCurrent != &m_NotificationList)
        {
            NotificationListEntry* nleCurrent = CONTAINING_RECORD( leCurrent, NotificationListEntry, ListEntry);
            if (nleCurrent->NotificationEvent == NotificationEvent_)
            {
                RemoveEntryList( leCurrent );
                ExFreePoolWithTag( nleNew, MINWAVERTSTREAM_POOLTAG );
                return STATUS_UNSUCCESSFUL;
            }

            leCurrent = leCurrent->Flink;
        }
    }

    InsertTailList(&m_NotificationList, &(nleNew->ListEntry));
    
    return STATUS_SUCCESS;
}

//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS CMiniportWaveRTStream::UnregisterNotificationEvent
(
    _In_ PKEVENT NotificationEvent_
)
{
    UNREFERENCED_PARAMETER(NotificationEvent_);

    PAGED_CODE();

    if (!IsListEmpty(&m_NotificationList))
    {
        PLIST_ENTRY leCurrent = m_NotificationList.Flink;
        while (leCurrent != &m_NotificationList)
        {
            NotificationListEntry* nleCurrent = CONTAINING_RECORD( leCurrent, NotificationListEntry, ListEntry);
            if (nleCurrent->NotificationEvent == NotificationEvent_)
            {
                RemoveEntryList( leCurrent );
                ExFreePoolWithTag( nleCurrent, MINWAVERTSTREAM_POOLTAG );
                return STATUS_SUCCESS;
            }

            leCurrent = leCurrent->Flink;
        }
    }

    return STATUS_NOT_FOUND;
}


//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS CMiniportWaveRTStream::GetClockRegister
(
    _Out_ PKSRTAUDIO_HWREGISTER Register_
)
{
    UNREFERENCED_PARAMETER(Register_);

    PAGED_CODE();

    return STATUS_NOT_IMPLEMENTED;
}

//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS CMiniportWaveRTStream::GetPositionRegister
(
    _Out_ PKSRTAUDIO_HWREGISTER Register_
)
{
    UNREFERENCED_PARAMETER(Register_);

    PAGED_CODE();

    return STATUS_NOT_IMPLEMENTED;
}

//=============================================================================
#pragma code_seg("PAGE")
VOID CMiniportWaveRTStream::GetHWLatency
(
    _Out_ PKSRTAUDIO_HWLATENCY  Latency_
)
{
    PAGED_CODE();

    ASSERT(Latency_);

    Latency_->ChipsetDelay = 0;
    Latency_->CodecDelay = 0;
    Latency_->FifoSize = 0;
}

//=============================================================================
#pragma code_seg("PAGE")
VOID CMiniportWaveRTStream::FreeAudioBuffer
(
    _In_opt_    PMDL        Mdl_,
    _In_        ULONG       Size_
)
{
    UNREFERENCED_PARAMETER(Size_);

    PAGED_CODE();
    
    if (Mdl_ != NULL)
    {
        if (m_pDmaBuffer != NULL)
        {
            m_pPortStream->UnmapAllocatedPages(m_pDmaBuffer, Mdl_);
            m_pDmaBuffer = NULL;
        }
        
        m_pPortStream->FreePagesFromMdl(Mdl_);
    }
    
    m_ulDmaBufferSize = 0;
}

//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS CMiniportWaveRTStream::AllocateAudioBuffer
(
    _In_    ULONG                   RequestedSize_,
    _Out_   PMDL                   *AudioBufferMdl_,
    _Out_   ULONG                  *ActualSize_,
    _Out_   ULONG                  *OffsetFromFirstPage_,
    _Out_   MEMORY_CACHING_TYPE    *CacheType_
)
{
    PAGED_CODE();

    if ( (0 == RequestedSize_) || (RequestedSize_ < m_pWfExt->Format.nBlockAlign) )
    { 
        return STATUS_UNSUCCESSFUL; 
    }

    RequestedSize_ -= RequestedSize_ % (m_pWfExt->Format.nBlockAlign);

    PHYSICAL_ADDRESS highAddress;
    highAddress.HighPart = 0;
    highAddress.LowPart = MAXULONG;

    PMDL pBufferMdl = m_pPortStream->AllocatePagesForMdl (highAddress, RequestedSize_);

    if (NULL == pBufferMdl)
    {
        return STATUS_UNSUCCESSFUL;
    }

    // From MSDN: 
    // "Since the Windows audio stack does not support a mechanism to express memory access 
    //  alignment requirements for buffers, audio drivers must select a caching type for mapped
    //  memory buffers that does not impose platform-specific alignment requirements. In other 
    //  words, the caching type used by the audio driver for mapped memory buffers, must not make 
    //  assumptions about the memory alignment requirements for any specific platform.
    //
    //  This method maps the physical memory pages in the MDL into kernel-mode virtual memory. 
    //  Typically, the miniport driver calls this method if it requires software access to the 
    //  scatter-gather list for an audio buffer. In this case, the storage for the scatter-gather 
    //  list must have been allocated by the IPortWaveRTStream::AllocatePagesForMdl or 
    //  IPortWaveRTStream::AllocateContiguousPagesForMdl method. 
    //
    //  A WaveRT miniport driver should not require software access to the audio buffer itself."
    //   
    m_pDmaBuffer = (BYTE*) m_pPortStream->MapAllocatedPages(pBufferMdl, MmCached);

    m_ulDmaBufferSize = RequestedSize_;

    *AudioBufferMdl_ = pBufferMdl;
    *ActualSize_ = RequestedSize_;
    *OffsetFromFirstPage_ = 0;
    *CacheType_ = MmCached;

    return STATUS_SUCCESS;
}

//=============================================================================
#pragma code_seg()
NTSTATUS CMiniportWaveRTStream::GetPosition
(
    _Out_   KSAUDIO_POSITION    *Position_
)
{
    NTSTATUS ntStatus;

#ifdef SYSVAD_BTH_BYPASS
    if (m_ScoOpen)
    {
        ntStatus = GetScoStreamNtStatus();
        IF_FAILED_JUMP(ntStatus, Done);
    }
#endif // SYSVAD_BTH_BYPASS

    if (m_KsState == KSSTATE_RUN)
    {
        //
        // Get the current time and update position.
        //
        LARGE_INTEGER ilQPC = KeQueryPerformanceCounter(NULL);
        UpdatePosition(ilQPC);
    }

    Position_->PlayOffset = m_ullPlayPosition;
    Position_->WriteOffset = m_ullWritePosition;

    ntStatus = STATUS_SUCCESS;
    
#ifdef SYSVAD_BTH_BYPASS
Done:
#endif // SYSVAD_BTH_BYPASS
    return ntStatus;
}

//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS CMiniportWaveRTStream::SetState
(
    _In_    KSSTATE State_
)
{
    PAGED_CODE();

    NTSTATUS        ntStatus        = STATUS_SUCCESS;
    PADAPTERCOMMON  pAdapterComm    = m_pMiniport->GetAdapterCommObj();

    // Spew an event for a pin state change request from portcls
    //Event type: eMINIPORT_PIN_STATE
    //Parameter 1: Current linear buffer position	
    //Parameter 2: Current WaveRtBufferWritePosition	
    //Parameter 3: Pin State 0->KS_STOP, 1->KS_ACQUIRE, 2->KS_PAUSE, 3->KS_RUN 
    //Parameter 4:0
    pAdapterComm->WriteEtwEvent(eMINIPORT_PIN_STATE, 
                                100, // replace with the correct "Current linear buffer position"	
                                m_ulCurrentWritePosition, // replace with the previous WaveRtBufferWritePosition that the drive received	
                                State_, // repalce with the correct "Data length completed"
                                0);  // always zero

    switch (State_)
    {
        case KSSTATE_STOP:
            // Reset DMA
            m_ullPlayPosition = 0;
            m_ullWritePosition = 0;
            m_ullLinearPosition = 0;
            
            // Wait until all work items are completed.
            if (!m_bCapture && !g_DoNotCreateDataFiles)
            {
                m_SaveData.WaitAllWorkItems();
            }

#ifdef SYSVAD_BTH_BYPASS
            if (m_ScoOpen)
            {
                PBTHHFPDEVICECOMMON bthHfpDevice;
                
                ASSERT(m_pMiniport->IsBthHfpDevice());
                bthHfpDevice = m_pMiniport->GetBthHfpDevice(); // weak ref.
                ASSERT(bthHfpDevice != NULL);

                //
                // Close the SCO connection.
                //
                ntStatus = bthHfpDevice->StreamClose();
                if (!NT_SUCCESS(ntStatus))
                {
                    DPF(D_ERROR, ("SetState: KSSTATE_STOP, StreamClose failed, 0x%x", ntStatus));
                }
                
                m_ScoOpen = FALSE;
            }
#endif // SYSVAD_BTH_BYPASS
            break;

        case KSSTATE_ACQUIRE:
#ifdef SYSVAD_BTH_BYPASS
            if (m_pMiniport->IsBthHfpDevice())
            {
                if(m_ScoOpen == FALSE)
                {
                    PBTHHFPDEVICECOMMON bthHfpDevice;

                    bthHfpDevice = m_pMiniport->GetBthHfpDevice(); // weak ref.
                    ASSERT(bthHfpDevice != NULL);

                    //
                    // Open the SCO connection.
                    //
                    ntStatus = bthHfpDevice->StreamOpen();
                    IF_FAILED_ACTION_JUMP(
                        ntStatus,
                        DPF(D_ERROR, ("SetState: KSSTATE_ACQUIRE, StreamOpen failed, 0x%x", ntStatus)),
                        Done);

                    m_ScoOpen = TRUE;
                }
            }
#endif // SYSVAD_BTH_BYPASS
            break;

        case KSSTATE_PAUSE:
            ULONGLONG ullPositionTemp;
            
            // Pause DMA
            if (m_ulNotificationIntervalMs > 0)
            {
                KeCancelTimer(m_pNotificationTimer);
                KeFlushQueuedDpcs(); 
            }

            // This call updates the linear buffer position.
            GetLinearBufferPosition(&ullPositionTemp, NULL);
            break;

        case KSSTATE_RUN:
            // Start DMA
            LARGE_INTEGER ullPerfCounterTemp;
            ullPerfCounterTemp = KeQueryPerformanceCounter(&m_ullPerformanceCounterFrequency);
            m_ullDmaTimeStamp = KSCONVERT_PERFORMANCE_TIME(m_ullPerformanceCounterFrequency.QuadPart, ullPerfCounterTemp);
            m_hnsElapsedTimeCarryForward  = 0;

            if (m_ulNotificationIntervalMs > 0)
            {
                LARGE_INTEGER   delay;
                delay.HighPart  = 0;
                delay.LowPart   = m_ulNotificationIntervalMs * HNSTIME_PER_MILLISECOND * -1;

                KeSetTimerEx
                (
                    m_pNotificationTimer,
                    delay,
                    m_ulNotificationIntervalMs,
                    m_pNotificationDpc
                );
            }

            break;
    }

    m_KsState = State_;

#ifdef SYSVAD_BTH_BYPASS
Done:
#endif  // SYSVAD_BTH_BYPASS
    return ntStatus;
}

//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS CMiniportWaveRTStream::SetFormat
(
    _In_    KSDATAFORMAT    *DataFormat_
)
{
    UNREFERENCED_PARAMETER(DataFormat_);

    PAGED_CODE();

    //if (!m_fCapture && !g_DoNotCreateDataFiles)
    //{
    //    ntStatus = m_SaveData.SetDataFormat(Format);
    //}

    return STATUS_NOT_SUPPORTED;
}

#pragma code_seg()

//=============================================================================
#pragma code_seg()
VOID CMiniportWaveRTStream::UpdatePosition
(
    _In_ LARGE_INTEGER ilQPC
)
{
    // Convert ticks to 100ns units.
    LONGLONG  hnsCurrentTime = KSCONVERT_PERFORMANCE_TIME(m_ullPerformanceCounterFrequency.QuadPart, ilQPC);
    
    // Calculate the time elapsed since the last call to GetPosition() or since the
    // DMA engine started.  Note that the division by 10000 to convert to milliseconds
    // may cause us to lose some of the time, so we will carry the remainder forward 
    // to the next GetPosition() call.
    //
    ULONG TimeElapsedInMS = (ULONG)(hnsCurrentTime - m_ullDmaTimeStamp + m_hnsElapsedTimeCarryForward)/10000;
    
    // Carry forward the remainder of this division so we don't fall behind with our position too much.
    //
    m_hnsElapsedTimeCarryForward = (hnsCurrentTime - m_ullDmaTimeStamp + m_hnsElapsedTimeCarryForward) % 10000;
    
    // Calculate how many bytes in the DMA buffer would have been processed in the elapsed
    // time.  Note that the division by 1000 to convert to milliseconds may cause us to 
    // lose some bytes, so we will carry the remainder forward to the next GetPosition() call.
    //
    // need to divide by 1000 because m_ulDmaMovementRate is average bytes per sec.
    ULONG ByteDisplacement = (m_ulDmaMovementRate * TimeElapsedInMS) / 1000;
    
    if (m_bCapture)
    {
        // Write sine wave to buffer.
        ByteDisplacement %= m_ulDmaBufferSize; // just for not crashing when debugging the driver.
        WriteBytes(ByteDisplacement);
    }
    else if (!g_DoNotCreateDataFiles)
    {
        //
        // Read from buffer and write to a file.
        //
        ByteDisplacement %= m_ulDmaBufferSize; // just for not crashing when debugging the driver.
        ReadBytes(ByteDisplacement);
    }
    
    // Increment the DMA position by the number of bytes displaced since the last
    // call to GetPosition() and ensure we properly wrap at buffer length.
    //
    m_ullPlayPosition = m_ullWritePosition =
        (m_ullWritePosition + ByteDisplacement) % m_ulDmaBufferSize;
    
    // m_ullDmaTimeStamp is updated in both GetPostion and GetLinearPosition calls
    // so m_ullLinearPosition needs to be updated accordingly here
    //
    m_ullLinearPosition += ByteDisplacement;
    
    // Update the DMA time stamp for the next call to GetPosition()
    //
    m_ullDmaTimeStamp = hnsCurrentTime;
}

//=============================================================================
#pragma code_seg()
VOID CMiniportWaveRTStream::WriteBytes
(
    _In_ ULONG ByteDisplacement
)
/*++

Routine Description:

  This function writes the audio buffer using a sine wave generator.

Arguments:

  ByteDisplacement - # of bytes to process.

--*/
{
    ULONG bufferOffset = m_ullLinearPosition % m_ulDmaBufferSize;
    ULONG runWrite = ByteDisplacement;

    //
    // If the run of bytes wraps around, write in two steps.
    //
    if ((bufferOffset + ByteDisplacement) > m_ulDmaBufferSize)
    {
        runWrite = m_ulDmaBufferSize - bufferOffset;
        m_ToneGenerator.GenerateSine(m_pDmaBuffer + bufferOffset, runWrite);
        bufferOffset = 0;
        runWrite = ByteDisplacement - runWrite;
    }

    if (runWrite > 0)
    {
        m_ToneGenerator.GenerateSine(m_pDmaBuffer + bufferOffset, runWrite);
    }
}

//=============================================================================
#pragma code_seg()
VOID CMiniportWaveRTStream::ReadBytes
(
    _In_ ULONG ByteDisplacement
)
/*++

Routine Description:

  This function reads the audio buffer and saves the data in a file.

Arguments:

  ByteDisplacement - # of bytes to process.

--*/
{
    ULONG bufferOffset = m_ullLinearPosition % m_ulDmaBufferSize;
    ULONG runWrite = ByteDisplacement;

    //
    // If the run of bytes wraps around, write in two steps.
    //
    if ((bufferOffset + ByteDisplacement) > m_ulDmaBufferSize)
    {
        runWrite = m_ulDmaBufferSize - bufferOffset;
        m_SaveData.WriteData(m_pDmaBuffer + bufferOffset, runWrite);
        bufferOffset = 0;
        runWrite = ByteDisplacement - runWrite;
    }

    if (runWrite > 0)
    {
        m_SaveData.WriteData(m_pDmaBuffer + bufferOffset, runWrite);
    }
}

//=============================================================================
#pragma code_seg("PAGE")
STDMETHODIMP_(NTSTATUS) 
CMiniportWaveRTStream::SetContentId
(
    _In_  ULONG                   contentId,
    _In_  PCDRMRIGHTS             drmRights
)
/*++

Routine Description:

  Sets DRM content Id for this stream. Also updates the Mixed content Id.

Arguments:

  contentId - new content id

  drmRights - rights for this stream.

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[CMiniportWaveRT::SetContentId]"));

    NTSTATUS    ntStatus;
    ULONG       ulOldContentId = contentId;

    m_ulContentId = contentId;

    //
    // Miniport should create a mixed DrmRights.
    //
    ntStatus = m_pMiniport->UpdateDrmRights();

    //
    // Restore the passed-in content Id.
    //
    if (!NT_SUCCESS(ntStatus))
    {
        m_ulContentId = ulOldContentId;
    }

    //
    // SYSVAD rights each stream seperately to disk. If the rights for this
    // stream indicates that the stream is CopyProtected, stop writing to disk.
    //
    m_SaveData.Disable(drmRights->CopyProtect);

    //
    // From MSDN:
    //
    // This sample doesn't forward protected content, but if your driver uses 
    // lower layer drivers or a different stack to properly work, please see the 
    // following info from MSDN:
    //
    // "Before allowing protected content to flow through a data path, the system
    // verifies that the data path is secure. To do so, the system authenticates
    // each module in the data path beginning at the upstream end of the data path
    // and moving downstream. As each module is authenticated, that module gives
    // the system information about the next module in the data path so that it
    // can also be authenticated. To be successfully authenticated, a module's 
    // binary file must be signed as DRM-compliant.
    //
    // Two adjacent modules in the data path can communicate with each other in 
    // one of several ways. If the upstream module calls the downstream module 
    // through IoCallDriver, the downstream module is part of a WDM driver. In 
    // this case, the upstream module calls the DrmForwardContentToDeviceObject
    // function to provide the system with the device object representing the 
    // downstream module. (If the two modules communicate through the downstream
    // module's COM interface or content handlers, the upstream module calls 
    // DrmForwardContentToInterface or DrmAddContentHandlers instead.)
    //
    // DrmForwardContentToDeviceObject performs the same function as 
    // PcForwardContentToDeviceObject and IDrmPort2::ForwardContentToDeviceObject." 
    //
    // Other supported DRM DDIs for down-level module validation are: 
    // DrmForwardContentToInterfaces and DrmAddContentHandlers.
    //
    // For more information, see MSDN's DRM Functions and Interfaces.
    //

    return ntStatus;
} // SetContentId

#ifdef SYSVAD_BTH_BYPASS

//=============================================================================
#pragma code_seg()
NTSTATUS 
CMiniportWaveRTStream::GetScoStreamNtStatus()
/*++

Routine Description:

  Checks if the Bluetooth SCO HFP connection is up, if not, an error is returned.

Return Value:

  NT status code.

--*/
{
    DPF_ENTER(("[CMiniportWaveRTStream::GetScoStreamNtStatus]"));

    NTSTATUS  ntStatus  = STATUS_INVALID_DEVICE_STATE;
        
    if (m_ScoOpen)
    {
        PBTHHFPDEVICECOMMON bthHfpDevice;
        
        ASSERT(m_pMiniport->IsBthHfpDevice());
        bthHfpDevice = m_pMiniport->GetBthHfpDevice(); // weak ref.
        ASSERT(bthHfpDevice != NULL);

        if (bthHfpDevice->GetStreamStatus())
        {
            ntStatus = STATUS_SUCCESS;
        }
    }

    return ntStatus;        
}
#endif // SYSVAD_BTH_BYPASS

//=============================================================================
#pragma code_seg()
void
TimerNotifyRT
(
    _In_      PKDPC         Dpc,
    _In_opt_  PVOID         DeferredContext,
    _In_opt_  PVOID         SA1,
    _In_opt_  PVOID         SA2
)
{
    UNREFERENCED_PARAMETER(Dpc);
    UNREFERENCED_PARAMETER(SA1);
    UNREFERENCED_PARAMETER(SA2);

    CMiniportWaveRTStream* _this = (CMiniportWaveRTStream*)DeferredContext;
    
    if (NULL == _this)
    {
        return;
    }
    
#ifdef SYSVAD_BTH_BYPASS
    if (_this->m_ScoOpen)
    {
        if (!NT_SUCCESS(_this->GetScoStreamNtStatus()))
        {
            return;
        }
    }
#endif  // SYSVAD_BTH_BYPASS

    if (_this->m_KsState != KSSTATE_RUN)
    {
        return;
    }
    
    PADAPTERCOMMON  pAdapterComm = _this->m_pMiniport->GetAdapterCommObj();

    // Simple buffer underrun detection.
    if (!_this->IsCurrentWaveRTWritePositionUpdated())
    {
        //Event type: eMINIPORT_GLITCH_REPORT
        //Parameter 1: Current linear buffer position 
        //Parameter 2: the previous WaveRtBufferWritePosition that the drive received 
        //Parameter 3: major glitch code: 1:WaveRT buffer is underrun
        //Parameter 4: minor code for the glitch cause
        pAdapterComm->WriteEtwEvent(eMINIPORT_GLITCH_REPORT, 
                                    100,    // replace with the correct "Current linear buffer position"   
                                    _this->GetCurrentWaveRTWritePosition(),
                                    1,      // WaveRT buffer is underrun
                                    0); 
    }

    if (!IsListEmpty(&_this->m_NotificationList))
    {
        PLIST_ENTRY leCurrent = _this->m_NotificationList.Flink;
        while (leCurrent != &_this->m_NotificationList)
        {
            NotificationListEntry* nleCurrent = CONTAINING_RECORD( leCurrent, NotificationListEntry, ListEntry);
            //Event type: eMINIPORT_BUFFER_COMPLETE
            //Parameter 1: Current linear buffer position	
            //Parameter 2: the previous WaveRtBufferWritePosition that the drive received
            //Parameter 3: Data length completed	
            //Parameter 4:0
            pAdapterComm->WriteEtwEvent(eMINIPORT_BUFFER_COMPLETE, 
                                        100, // replace with the correct "Current linear buffer position"	
                                        _this->GetCurrentWaveRTWritePosition(), 	
                                        300, // repalce with the correct "Data length completed"
                                        0);  // always zero

            KeSetEvent(nleCurrent->NotificationEvent, 0, 0);

            leCurrent = leCurrent->Flink;
        }
    }
}
//=============================================================================

