/*++

Copyright (c) Microsoft Corporation All Rights Reserved

Module Name:

    minwavert.h

Abstract:

    Definition of wavert miniport class.

--*/

#ifndef _SYSVAD_MINWAVERT_H_
#define _SYSVAD_MINWAVERT_H_

#ifdef SYSVAD_BTH_BYPASS
#include "bthhfpmicwavtable.h"
#endif // SYSVAD_BTH_BYPASS

//=============================================================================
// Referenced Forward
//=============================================================================
class CMiniportWaveRTStream;
typedef CMiniportWaveRTStream *PCMiniportWaveRTStream;

//=============================================================================
// Classes
//=============================================================================
///////////////////////////////////////////////////////////////////////////////
// CMiniportWaveRT
//   
class CMiniportWaveRT : 
    public IMiniportWaveRT,
    public IMiniportAudioEngineNode,
    public IMiniportAudioSignalProcessing,
    public CUnknown
{
private:
    ULONG                               m_ulLoopbackAllocated;
    ULONG                               m_ulSystemAllocated;
    ULONG                               m_ulOffloadAllocated;
    
    ULONG                               m_ulMaxSystemStreams;
    ULONG                               m_ulMaxOffloadStreams;
    ULONG                               m_ulMaxLoopbackStreams;
    // weak ref of running streams.
    PCMiniportWaveRTStream            * m_SystemStreams;
    PCMiniportWaveRTStream            * m_OffloadStreams;
    PCMiniportWaveRTStream            * m_LoopbackStreams;

    BOOL                                m_bGfxEnabled;
    PBOOL                               m_pbMuted;
    PLONG                               m_plVolumeLevel;
    PLONG                               m_plPeakMeter;
    PKSDATAFORMAT_WAVEFORMATEXTENSIBLE  m_pMixFormat;
    PKSDATAFORMAT_WAVEFORMATEXTENSIBLE  m_pDeviceFormat;
    PCFILTER_DESCRIPTOR                 m_FilterDesc;
    eDeviceType                         m_DeviceType;
    PADAPTERCOMMON                      m_pAdapterCommon;
    PIN_DEVICE_FORMATS_AND_MODES *      m_DeviceFormatsAndModes;
    ULONG                               m_DeviceFormatsAndModesCount; 
    USHORT                              m_DeviceMaxChannels;
    PDRMPORT                            m_pDrmPort;
    DRMRIGHTS                           m_MixDrmRights;
    ULONG                               m_ulMixDrmContentId;
    CONSTRICTOR_OPTION                  m_LoopbackProtection;
    union {
        PVOID                           m_DeviceContext;
#ifdef SYSVAD_BTH_BYPASS
        PBTHHFPDEVICECOMMON             m_BthHfpDevice;
#endif  // SYSVAD_BTH_BYPASS
    };
    ULONG                               m_DeviceFlags;

public:
    NTSTATUS ValidateStreamCreate
    (
        _In_ ULONG _Pin, 
        _In_ BOOLEAN _Capture
    );
    
    NTSTATUS StreamCreated
    (
        _In_ ULONG                  _Pin,
        _In_ PCMiniportWaveRTStream _Stream
    );
    
    NTSTATUS StreamClosed
    (
        _In_ ULONG _Pin,
        _In_ PCMiniportWaveRTStream _Stream
    );
    
    NTSTATUS IsFormatSupported
    ( 
        _In_ ULONG          _ulPin, 
        _In_ BOOLEAN        _bCapture,
        _In_ PKSDATAFORMAT  _pDataFormat
    );

    static NTSTATUS GetAttributesFromAttributeList
    (
        _In_ const KSMULTIPLE_ITEM *_pAttributes,
        _In_ size_t _Size,
        _Out_ GUID* _pSignalProcessingMode
    );

protected:
    NTSTATUS UpdateDrmRights
    (
        void
    );
    
    NTSTATUS SetLoopbackProtection
    (
        _In_ CONSTRICTOR_OPTION ulProtectionOption
    );

public:
    DECLARE_STD_UNKNOWN();

    CMiniportWaveRT(
        _In_            PUNKNOWN                                UnknownAdapter,
        _In_            PENDPOINT_MINIPAIR                      MiniportPair,
        _In_opt_        PVOID                                   DeviceContext
        )
        :CUnknown(0),
        m_ulMaxSystemStreams(0),
        m_ulMaxOffloadStreams(0),
        m_ulMaxLoopbackStreams(0),
        m_DeviceType(MiniportPair->DeviceType),
        m_DeviceContext(DeviceContext), 
        m_DeviceMaxChannels(MiniportPair->DeviceMaxChannels),
        m_DeviceFormatsAndModes(MiniportPair->PinDeviceFormatsAndModes),
        m_DeviceFormatsAndModesCount(MiniportPair->PinDeviceFormatsAndModesCount),
        m_DeviceFlags(MiniportPair->DeviceFlags)
    {
        m_pAdapterCommon = (PADAPTERCOMMON)UnknownAdapter; // weak ref.
        
        if (MiniportPair->WaveDescriptor)
        {
            RtlCopyMemory(&m_FilterDesc, MiniportPair->WaveDescriptor, sizeof(m_FilterDesc));
            
            //
            // Get the max # of pin instances.
            //
            if (IsRenderDevice())
            {
                if (IsOffloadSupported())
                {
                    if (m_FilterDesc.PinCount > KSPIN_WAVE_RENDER_SOURCE)
                    {
                        m_ulMaxSystemStreams = m_FilterDesc.Pins[KSPIN_WAVE_RENDER_SINK_SYSTEM].MaxFilterInstanceCount;
                        m_ulMaxOffloadStreams = m_FilterDesc.Pins[KSPIN_WAVE_RENDER_SINK_OFFLOAD].MaxFilterInstanceCount;
                        m_ulMaxLoopbackStreams = m_FilterDesc.Pins[KSPIN_WAVE_RENDER_SINK_LOOPBACK].MaxFilterInstanceCount;
                    }
                }
                else
                {
                    if (m_FilterDesc.PinCount > KSPIN_WAVE_RENDER2_SOURCE)
                    {
                        m_ulMaxSystemStreams = m_FilterDesc.Pins[KSPIN_WAVE_RENDER2_SINK_SYSTEM].MaxFilterInstanceCount;
                        m_ulMaxLoopbackStreams = m_FilterDesc.Pins[KSPIN_WAVE_RENDER2_SINK_LOOPBACK].MaxFilterInstanceCount;
                    }
                }
            }
        }
        
#ifdef SYSVAD_BTH_BYPASS
        if (IsBthHfpDevice())
        {
            if (m_BthHfpDevice != NULL)
            {
                // This ref is released on dtor.
                m_BthHfpDevice->AddRef(); // strong ref.
            }
        }
#endif // SYSVAD_BTH_BYPASS
    }

    ~CMiniportWaveRT();

    IMP_IMiniportWaveRT;
    IMP_IMiniportAudioEngineNode;
    IMP_IMiniportAudioSignalProcessing;
    
    // Friends
    friend class        CMiniportWaveRTStream;
    friend class        CMiniportTopologySimple;
    
    friend NTSTATUS PropertyHandler_WaveFilter(   
                        _In_ PPCPROPERTY_REQUEST      PropertyRequest 
                        );   

public:
    NTSTATUS PropertyHandlerEffectListRequest
    (
        _In_ PPCPROPERTY_REQUEST PropertyRequest
    );    

    NTSTATUS PropertyHandlerProposedFormat
    (
        _In_ PPCPROPERTY_REQUEST PropertyRequest
    );

    NTSTATUS PropertyHandlerProposedFormat2
    (
        _In_ PPCPROPERTY_REQUEST PropertyRequest
    );

    PADAPTERCOMMON GetAdapterCommObj() 
    {
        return m_pAdapterCommon; 
    };

#ifdef SYSVAD_BTH_BYPASS
    NTSTATUS PropertyHandler_BthHfpAudioEffectsDiscoveryEffectsList  
    (
        _In_ PPCPROPERTY_REQUEST PropertyRequest
    );
#endif  // SYSVAD_BTH_BYPASS

    //---------------------------------------------------------------------------------------------------------
    // volume
    //---------------------------------------------------------------------------------------------------------
    NTSTATUS GetVolumeChannelCount
    (
        _Out_  UINT32 * pulChannelCount
    );
    
    NTSTATUS GetVolumeSteppings
    (
        _Out_writes_bytes_(_ui32DataSize)  PKSPROPERTY_STEPPING_LONG _pKsPropStepLong, 
        _In_  UINT32    _ui32DataSize
    );
    
    NTSTATUS GetChannelVolume
    (
        _In_  UINT32    _uiChannel, 
        _Out_  LONG *   _pVolume
    );
    
    NTSTATUS SetChannelVolume
    (
        _In_  UINT32    _uiChannel, 
        _In_  LONG      _Volume
    );

    //-----------------------------------------------------------------------------
    // metering 
    //-----------------------------------------------------------------------------
    NTSTATUS GetPeakMeterChannelCount
    (
        _Out_  UINT32 * pulChannelCount
    );
    
    NTSTATUS GetPeakMeterSteppings
    (
        _Out_writes_bytes_(_ui32DataSize)  PKSPROPERTY_STEPPING_LONG _pKsPropStepLong, 
        _In_  UINT32    _ui32DataSize
    );
    
    NTSTATUS GetChannelPeakMeter
    (
        _In_  UINT32    _uiChannel, 
        _Out_  LONG *   _plPeakMeter
    );

    //-----------------------------------------------------------------------------
    // mute
    //-----------------------------------------------------------------------------
    NTSTATUS GetMuteChannelCount
    (
        _Out_  UINT32 * pulChannelCount
    );
    
    NTSTATUS GetMuteSteppings
    (
        _Out_writes_bytes_(_ui32DataSize)  PKSPROPERTY_STEPPING_LONG _pKsPropStepLong, 
        _In_  UINT32    _ui32DataSize
    );
    
    NTSTATUS GetChannelMute
    (
        _In_  UINT32    _uiChannel, 
        _Out_  BOOL *   _pbMute
    );
    
    NTSTATUS SetChannelMute
    (
        _In_  UINT32    _uiChannel, 
        _In_  BOOL      _bMute
    );

private:
    //-----------------------------------------------------------------------------
    // Supported formats index array:
    //
    // capture endpoint:
    // [0] Pin 1 - wave source.
    //
    // render endpoints:
    // [0] Audio-engine device formats.
    // [1] Pin 0 - Host
    // [2] Pin 1 - Offload
    // [3] Pin 2 - Loopback
    //
    KSDATAFORMAT_WAVEFORMATEXTENSIBLE * GetCapturePinSupportedDeviceFormats()
    {
        ASSERT(m_DeviceFormatsAndModesCount > CAPTURE_DEVICE_FORMATS);
        ASSERT(m_DeviceFormatsAndModes[CAPTURE_DEVICE_FORMATS].WaveFormats != NULL);
        ASSERT(GetCapturePinSupportedDeviceFormatsCount() > 0);
        
        return m_DeviceFormatsAndModes[CAPTURE_DEVICE_FORMATS].WaveFormats;
    }
    
    ULONG GetCapturePinSupportedDeviceFormatsCount()
    {
        ASSERT(m_DeviceFormatsAndModesCount > CAPTURE_DEVICE_FORMATS);
        
        return m_DeviceFormatsAndModes[CAPTURE_DEVICE_FORMATS].WaveFormatsCount;
    }
    
    KSDATAFORMAT_WAVEFORMATEXTENSIBLE * GetAudioEngineSupportedDeviceFormats()
    {
        ASSERT(m_DeviceFormatsAndModesCount > AUDIO_DEVICE_FORMATS);
        ASSERT(m_DeviceFormatsAndModes[AUDIO_DEVICE_FORMATS].WaveFormats != NULL);
        ASSERT(GetAudioEngineSupportedDeviceFormatsCount() > 0);
        
        return m_DeviceFormatsAndModes[AUDIO_DEVICE_FORMATS].WaveFormats;
    }
    
    ULONG GetAudioEngineSupportedDeviceFormatsCount()
    {
        ASSERT(m_DeviceFormatsAndModesCount > AUDIO_DEVICE_FORMATS);
        return m_DeviceFormatsAndModes[AUDIO_DEVICE_FORMATS].WaveFormatsCount;
    }
    
    KSDATAFORMAT_WAVEFORMATEXTENSIBLE * GetHostPinSupportedDeviceFormats()
    {
        ASSERT(m_DeviceFormatsAndModesCount > HOST_DEVICE_FORMATS);
        ASSERT(m_DeviceFormatsAndModes[HOST_DEVICE_FORMATS].WaveFormats != NULL);
        ASSERT(GetHostPinSupportedDeviceFormatsCount() > 0);
        
        return m_DeviceFormatsAndModes[HOST_DEVICE_FORMATS].WaveFormats;
    }
        
    ULONG GetHostPinSupportedDeviceFormatsCount()
    {
        ASSERT(m_DeviceFormatsAndModesCount > HOST_DEVICE_FORMATS);
        
        return m_DeviceFormatsAndModes[HOST_DEVICE_FORMATS].WaveFormatsCount;
    }
    
    KSDATAFORMAT_WAVEFORMATEXTENSIBLE * GetOffloadPinSupportedDeviceFormats()
    {
        ASSERT(m_DeviceFormatsAndModesCount > OFFLOAD_DEVICE_FORMATS);
        ASSERT(m_DeviceFormatsAndModes[OFFLOAD_DEVICE_FORMATS].WaveFormats != NULL);
        ASSERT(GetOffloadPinSupportedDeviceFormatsCount() > 0);
        
        return m_DeviceFormatsAndModes[OFFLOAD_DEVICE_FORMATS].WaveFormats;
    }
        
    ULONG GetOffloadPinSupportedDeviceFormatsCount()
    {
        ASSERT(m_DeviceFormatsAndModesCount > OFFLOAD_DEVICE_FORMATS);
        
        return m_DeviceFormatsAndModes[OFFLOAD_DEVICE_FORMATS].WaveFormatsCount;
    }
    
    KSDATAFORMAT_WAVEFORMATEXTENSIBLE * GetLoopbackPinSupportedDeviceFormats()
    {
        ASSERT(m_DeviceFormatsAndModesCount > LOOPBACK_DEVICE_FORMATS);
        ASSERT(m_DeviceFormatsAndModes[LOOPBACK_DEVICE_FORMATS].WaveFormats != NULL);
        ASSERT(GetLoopbackPinSupportedDeviceFormatsCount() > 0);
        
        return m_DeviceFormatsAndModes[LOOPBACK_DEVICE_FORMATS].WaveFormats;
    }
        
    ULONG GetLoopbackPinSupportedDeviceFormatsCount()
    {
        ASSERT(m_DeviceFormatsAndModesCount > LOOPBACK_DEVICE_FORMATS);
        
        return m_DeviceFormatsAndModes[LOOPBACK_DEVICE_FORMATS].WaveFormatsCount;
    }
    
    //-----------------------------------------------------------------------------
    // Supported modes index array:
    //
    // capture endpoint:
    // [0] Pin 1 - wave source.
    //
    // render endpoints:
    // [0] Audio-engine device formats - no support
    // [1] Pin 0 - Host
    // [2] Pin 1 - Offload
    // [3] Pin 2 - Loopback - no support
    //
    MODE_AND_DEFAULT_FORMAT * GetCapturePinSupportedDeviceModes()
    {
        ASSERT(m_DeviceFormatsAndModesCount > CAPTURE_DEVICE_FORMATS);
        ASSERT(GetCapturePinSupportedDeviceModesCount() > 0);
        
#ifdef SYSVAD_BTH_BYPASS
        if (m_DeviceType == eBthHfpMicDevice)
        {
            ASSERT(m_BthHfpDevice != NULL);
            if (m_BthHfpDevice->IsNRECSupported())
            {
                return BthHfpMicPinSupportedDeviceModesNrec;
            }
            else
            {
                return BthHfpMicPinSupportedDeviceModesNoNrec;
            }
        }
#endif // SYSVAD_BTH_BYPASS
        
        ASSERT(m_DeviceFormatsAndModes[CAPTURE_DEVICE_FORMATS].ModeAndDefaultFormat != NULL);
        return m_DeviceFormatsAndModes[CAPTURE_DEVICE_FORMATS].ModeAndDefaultFormat;
    }
    
    ULONG GetCapturePinSupportedDeviceModesCount()
    {
        ASSERT(m_DeviceFormatsAndModesCount > CAPTURE_DEVICE_FORMATS);
        
#ifdef SYSVAD_BTH_BYPASS
        if (m_DeviceType == eBthHfpMicDevice)
        {
            ASSERT(m_BthHfpDevice != NULL);
            if (m_BthHfpDevice->IsNRECSupported())
            {
                return SIZEOF_ARRAY(BthHfpMicPinSupportedDeviceModesNrec);
            }
            else
            {
                return SIZEOF_ARRAY(BthHfpMicPinSupportedDeviceModesNoNrec);
            }
        }
#endif // SYSVAD_BTH_BYPASS
        
        return m_DeviceFormatsAndModes[CAPTURE_DEVICE_FORMATS].ModeAndDefaultFormatCount;
    }
    
    MODE_AND_DEFAULT_FORMAT * GetHostPinSupportedDeviceModes()
    {
        ASSERT(m_DeviceFormatsAndModesCount > HOST_DEVICE_FORMATS);
        ASSERT(m_DeviceFormatsAndModes[HOST_DEVICE_FORMATS].ModeAndDefaultFormat != NULL);
        ASSERT(GetHostPinSupportedDeviceModesCount() > 0);
        
        return m_DeviceFormatsAndModes[HOST_DEVICE_FORMATS].ModeAndDefaultFormat;
    }
        
    ULONG GetHostPinSupportedDeviceModesCount()
    {
        ASSERT(m_DeviceFormatsAndModesCount > HOST_DEVICE_FORMATS);
        
        return m_DeviceFormatsAndModes[HOST_DEVICE_FORMATS].ModeAndDefaultFormatCount;
    }
    
    MODE_AND_DEFAULT_FORMAT * GetOffloadPinSupportedDeviceModes()
    {
        ASSERT(m_DeviceFormatsAndModesCount > OFFLOAD_DEVICE_FORMATS);
        ASSERT(m_DeviceFormatsAndModes[OFFLOAD_DEVICE_FORMATS].ModeAndDefaultFormat != NULL);
        ASSERT(GetOffloadPinSupportedDeviceModesCount() > 0);
        
        return m_DeviceFormatsAndModes[OFFLOAD_DEVICE_FORMATS].ModeAndDefaultFormat;
    }
        
    ULONG GetOffloadPinSupportedDeviceModesCount()
    {
        ASSERT(m_DeviceFormatsAndModesCount > OFFLOAD_DEVICE_FORMATS);
        
        return m_DeviceFormatsAndModes[OFFLOAD_DEVICE_FORMATS].ModeAndDefaultFormatCount;
    }
    
    BOOL IsRenderDevice()
    {
        return (m_DeviceType == eSpeakerDevice   ||
                m_DeviceType == eSpeakerHpDevice ||
                m_DeviceType == eHdmiDevice      ||
                m_DeviceType == eBthHfpSpeakerDevice) ? TRUE : FALSE;
    }

    BOOL IsOffloadSupported()
    {
        return (m_DeviceFlags & ENDPOINT_OFFLOAD_SUPPORTED) ? TRUE : FALSE;
    }

    ULONG GetSystemPinId()
    {
        ASSERT(IsRenderDevice());
        return IsOffloadSupported() ? KSPIN_WAVE_RENDER_SINK_SYSTEM : KSPIN_WAVE_RENDER2_SINK_SYSTEM;
    }
    
    ULONG GetLoopbackPinId()
    {
        ASSERT(IsRenderDevice());
        return IsOffloadSupported() ? KSPIN_WAVE_RENDER_SINK_LOOPBACK : KSPIN_WAVE_RENDER2_SINK_LOOPBACK;
    }
    
    ULONG GetOffloadPinId()
    {
        ASSERT(IsRenderDevice());
        ASSERT(IsOffloadSupported());
        
        return KSPIN_WAVE_RENDER_SINK_OFFLOAD;
    }

    ULONG GetSourcePinId()
    {
        ASSERT(IsRenderDevice());
        return IsOffloadSupported() ? KSPIN_WAVE_RENDER_SOURCE : KSPIN_WAVE_RENDER2_SOURCE;
    }

#ifdef SYSVAD_BTH_BYPASS
public:
    BOOL IsBthHfpDevice()
    {
        return (m_DeviceType == eBthHfpMicDevice ||
                m_DeviceType == eBthHfpSpeakerDevice) ? TRUE : FALSE;
    }

    // Returns a weak ref to the Bluetooth HFP device.
    PBTHHFPDEVICECOMMON GetBthHfpDevice() 
    {
        PBTHHFPDEVICECOMMON bthHfpDevice = NULL;
        
        if (IsBthHfpDevice())
        {
            if (m_BthHfpDevice != NULL)
            {
                bthHfpDevice = m_BthHfpDevice;
            }
        }
    
        return bthHfpDevice;
    }
#endif // SYSVAD_BTH_BYPASS
};
typedef CMiniportWaveRT *PCMiniportWaveRT;

#endif // _SYSVAD_MINWAVERT_H_

