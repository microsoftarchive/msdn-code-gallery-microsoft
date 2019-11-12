/*++

Copyright (c) Microsoft Corporation All Rights Reserved

Module Name:

    Common.h

Abstract:
    
    CAdapterCommon class declaration.

--*/

#ifndef _SYSVAD_COMMON_H_
#define _SYSVAD_COMMON_H_

//=============================================================================
// Macros
//=============================================================================

#define UNREFERENCED_VAR(status) \
    status = status 

//-------------------------------------------------------------------------
// Description:
//
// If the condition evaluates to TRUE, jump to the given label.
//
// Parameters:
//
//      condition - [in] Code that fits in if statement
//      label - [in] label to jump if condition is met
//
#define IF_TRUE_JUMP(condition, label)                          \
    if(condition)                                               \
    {                                                           \
        goto label;                                             \
    }

//-------------------------------------------------------------------------
// Description:
//
// If the condition evaluates to TRUE, perform the given statement
// then jump to the given label.
//
// Parameters:
//
//      condition - [in] Code that fits in if statement
//      action - [in] action to perform in body of if statement
//      label - [in] label to jump if condition is met
//
#define IF_TRUE_ACTION_JUMP(condition, action, label)           \
    if(condition)                                               \
    {                                                           \
        action;                                                 \
        goto label;                                             \
    }
    
//-------------------------------------------------------------------------
// Description:
//
// If the ntStatus is not NT_SUCCESS, perform the given statement then jump to
// the given label.
//
// Parameters:
//
//      ntStatus - [in] Value to check
//      action - [in] action to perform in body of if statement
//      label - [in] label to jump if condition is met
//
#define IF_FAILED_ACTION_JUMP(ntStatus, action, label)          \
        if (!NT_SUCCESS(ntStatus))                              \
        {                                                       \
            action;                                             \
            goto label;                                         \
        }
    
//-------------------------------------------------------------------------
// Description:
//
// If the ntStatus passed is not NT_SUCCESS, jump to the given label.
//
// Parameters:
//
//      ntStatus - [in] Value to check
//      label - [in] label to jump if condition is met
//
#define IF_FAILED_JUMP(ntStatus, label)                         \
    if(!NT_SUCCESS(ntStatus))                                   \
    {                                                           \
        goto label;                                             \
    }

#define SAFE_RELEASE(p) {if (p) { (p)->Release(); (p) = nullptr; } }

// JACKDESC_RGB(r, g, b) 
#define JACKDESC_RGB(r, g, b) \
    ((COLORREF)((r << 16) | (g << 8) | (b)))

// Min/Max defines.
#define MIN(x, y) ((x) < (y) ? (x) : (y))
#define MAX(x, y) ((x) > (y) ? (x) : (y))

#define MINWAVERT_POOLTAG   'RWNM'
#define MINTOPORT_POOLTAG   'RTNM'
#define MINADAPTER_POOLTAG  'uAyS'

typedef enum
{
    eSpeakerDevice = 0,
    eSpeakerHpDevice,
    eHdmiDevice,
    eMicInDevice,
    eMicArrayDevice1,
    eMicArrayDevice2,
    eMicArrayDevice3,
    eBthHfpSpeakerDevice,
    eBthHfpMicDevice,
    eMaxDeviceType
    
} eDeviceType;

//
// Signal processing modes and default formats structs.
//
typedef struct _MODE_AND_DEFAULT_FORMAT {
    GUID            Mode;
    KSDATAFORMAT*   DefaultFormat;
} MODE_AND_DEFAULT_FORMAT, *PMODE_AND_DEFAULT_FORMAT;

//
// Used to specify the device formats supported on a specific pin.
//
typedef struct _PIN_DEVICE_FORMATS_AND_MODES 
{
    KSDATAFORMAT_WAVEFORMATEXTENSIBLE * WaveFormats;
    ULONG                               WaveFormatsCount;
    
    MODE_AND_DEFAULT_FORMAT *           ModeAndDefaultFormat;
    ULONG                               ModeAndDefaultFormatCount;

} PIN_DEVICE_FORMATS_AND_MODES, *PPIN_DEVICE_FORMATS_AND_MODES;

// forward declaration.
typedef struct _ENDPOINT_MINIPAIR *PENDPOINT_MINIPAIR;

// both wave & topology miniport create function prototypes have this form:
typedef HRESULT (*PFNCREATEMINIPORT)(
    _Out_           PUNKNOWN                              * Unknown,
    _In_            REFCLSID,
    _In_opt_        PUNKNOWN                                UnknownOuter,
    _When_((PoolType & NonPagedPoolMustSucceed) != 0,
       __drv_reportError("Must succeed pool allocations are forbidden. "
			 "Allocation failures cause a system crash"))
    _In_            POOL_TYPE                               PoolType, 
    _In_            PUNKNOWN                                UnknownAdapter,
    _In_opt_        PVOID                                   DeviceContext,
    _In_            PENDPOINT_MINIPAIR                      MiniportPair
);


#define ENDPOINT_NO_FLAGS               0x00000000
#define ENDPOINT_OFFLOAD_SUPPORTED      0x00000001

//
// Endpoint miniport pair (wave/topology) descriptor.
//
typedef struct _ENDPOINT_MINIPAIR 
{
    eDeviceType                     DeviceType;

    // Topology miniport.
    PWSTR                           TopoName;               // make sure this name matches with SYSVAD.<TopoName>.szPname in the inf's [Strings] section
    PFNCREATEMINIPORT               TopoCreateCallback;
    PCFILTER_DESCRIPTOR*            TopoDescriptor;

    // Wave RT miniport.
    PWSTR                           WaveName;               // make sure this name matches with SYSVAD.<WaveName>.szPname in the inf's [Strings] section
    PFNCREATEMINIPORT               WaveCreateCallback;
    PCFILTER_DESCRIPTOR*            WaveDescriptor;

    USHORT                          DeviceMaxChannels;
    PIN_DEVICE_FORMATS_AND_MODES*   PinDeviceFormatsAndModes;
    ULONG                           PinDeviceFormatsAndModesCount;
    
    // Miniport physical connections.
    PHYSICALCONNECTIONTABLE*        PhysicalConnections;

    // General endpoint flags (one of more ENDPOINT_<flag-type>, see above)
    ULONG                           DeviceFlags;
} ENDPOINT_MINIPAIR;

//=============================================================================
// Defines
//=============================================================================

DEFINE_GUID(IID_IAdapterCommon,
0x7eda2950, 0xbf9f, 0x11d0, 0x87, 0x1f, 0x0, 0xa0, 0xc9, 0x11, 0xb5, 0x44);

//=============================================================================
// Interfaces
//=============================================================================

///////////////////////////////////////////////////////////////////////////////
// IAdapterCommon
//
DECLARE_INTERFACE_(IAdapterCommon, IUnknown)
{
    STDMETHOD_(NTSTATUS,        Init) 
    ( 
        THIS_
        _In_  PDEVICE_OBJECT      DeviceObject 
    ) PURE;

    STDMETHOD_(PDEVICE_OBJECT,  GetDeviceObject)
    (
        THIS
    ) PURE;
    
    STDMETHOD_(PDEVICE_OBJECT,  GetPhysicalDeviceObject)
    (
        THIS
    ) PURE;

    STDMETHOD_(WDFDEVICE,       GetWdfDevice)
    (
        THIS
    ) PURE;

    STDMETHOD_(VOID,            SetWaveServiceGroup) 
    ( 
        THIS_
        _In_ PSERVICEGROUP        ServiceGroup 
    ) PURE;

    STDMETHOD_(BOOL,            bDevSpecificRead)
    (
        THIS_
    ) PURE;

    STDMETHOD_(VOID,            bDevSpecificWrite)
    (
        THIS_
        _In_  BOOL                bDevSpecific
    );

    STDMETHOD_(INT,             iDevSpecificRead)
    (
        THIS_
    ) PURE;

    STDMETHOD_(VOID,            iDevSpecificWrite)
    (
        THIS_
        _In_  INT                 iDevSpecific
    );

    STDMETHOD_(UINT,            uiDevSpecificRead)
    (
        THIS_
    ) PURE;

    STDMETHOD_(VOID,            uiDevSpecificWrite)
    (
        THIS_
        _In_  UINT                uiDevSpecific
    );

    STDMETHOD_(BOOL,            MixerMuteRead)
    (
        THIS_
        _In_  ULONG               Index,
        _In_  ULONG               Channel
    ) PURE;

    STDMETHOD_(VOID,            MixerMuteWrite)
    (
        THIS_
        _In_  ULONG               Index,
        _In_  ULONG               Channel,
        _In_  BOOL                Value
    );

    STDMETHOD_(ULONG,           MixerMuxRead)
    (
        THIS
    );

    STDMETHOD_(VOID,            MixerMuxWrite)
    (
        THIS_
        _In_  ULONG               Index
    );

    STDMETHOD_(LONG,            MixerVolumeRead) 
    ( 
        THIS_
        _In_  ULONG               Index,
        _In_  ULONG               Channel
    ) PURE;

    STDMETHOD_(VOID,            MixerVolumeWrite) 
    ( 
        THIS_
        _In_  ULONG               Index,
        _In_  ULONG               Channel,
        _In_  LONG                Value 
    ) PURE;
    
    STDMETHOD_(LONG,            MixerPeakMeterRead) 
    ( 
        THIS_
        _In_  ULONG               Index,
        _In_  ULONG               Channel
    ) PURE;

    STDMETHOD_(VOID,            MixerReset) 
    ( 
        THIS 
    ) PURE;

    STDMETHOD_(NTSTATUS,        WriteEtwEvent) 
    ( 
        THIS_ 
        _In_ EPcMiniportEngineEvent    miniportEventType,
        _In_ ULONGLONG  ullData1,
        _In_ ULONGLONG  ullData2,
        _In_ ULONGLONG  ullData3,
        _In_ ULONGLONG  ullData4
    ) PURE;

    STDMETHOD_(VOID,            SetEtwHelper) 
    ( 
        THIS_
        PPORTCLSETWHELPER _pPortClsEtwHelper
    ) PURE;
    
    STDMETHOD_(NTSTATUS,        InstallSubdevice)
    ( 
        _In_opt_        PIRP                                    Irp,
        _In_            PWSTR                                   Name,
        _In_            REFGUID                                 PortClassId,
        _In_            REFGUID                                 MiniportClassId,
        _In_opt_        PFNCREATEMINIPORT                       MiniportCreate,
        _In_opt_        PVOID                                   DeviceContext,
        _In_            PENDPOINT_MINIPAIR                      MiniportPair,
        _In_opt_        PRESOURCELIST                           ResourceList,
        _In_            REFGUID                                 PortInterfaceId,
        _Out_opt_       PUNKNOWN                              * OutPortInterface,
        _Out_opt_       PUNKNOWN                              * OutPortUnknown
    );    

    STDMETHOD_(NTSTATUS,        UnregisterSubdevice)
    (
        THIS_
        _In_opt_   PUNKNOWN     UnknownPort
    );

    STDMETHOD_(NTSTATUS,        ConnectTopologies)
    (
        THIS_
        _In_ PUNKNOWN                   UnknownTopology,
        _In_ PUNKNOWN                   UnknownWave,
        _In_ PHYSICALCONNECTIONTABLE*   PhysicalConnections
    );

    STDMETHOD_(NTSTATUS,        DisconnectTopologies)
    (
        THIS_
        _In_ PUNKNOWN                   UnknownTopology,
        _In_ PUNKNOWN                   UnknownWave,
        _In_ PHYSICALCONNECTIONTABLE*   PhysicalConnections
    );

    STDMETHOD_(NTSTATUS,        InstallEndpointFilters)
    (
        THIS_
        _In_opt_    PIRP                Irp, 
        _In_        PENDPOINT_MINIPAIR  MiniportPair,
        _In_opt_    PVOID               DeviceContext,
        _Out_opt_   PUNKNOWN *          UnknownTopology,
        _Out_opt_   PUNKNOWN *          UnknownWave
    );

    STDMETHOD_(NTSTATUS,        RemoveEndpointFilters)
    (
        THIS_
        _In_        PENDPOINT_MINIPAIR  MiniportPair,
        _In_opt_    PUNKNOWN            UnknownTopology,
        _In_opt_    PUNKNOWN            UnknownWave
    );

#ifdef SYSVAD_BTH_BYPASS
    STDMETHOD_(NTSTATUS,        InitBthScoBypass)();
       
    STDMETHOD_(VOID,            CleanupBthScoBypass)();
#endif // SYSVAD_BTH_BYPASS

};

typedef IAdapterCommon *PADAPTERCOMMON;

//=============================================================================
// Function Prototypes
//=============================================================================
NTSTATUS
NewAdapterCommon
( 
    _Out_       PUNKNOWN *              Unknown,
    _In_        REFCLSID,
    _In_opt_    PUNKNOWN                UnknownOuter,
    _When_((PoolType & NonPagedPoolMustSucceed) != 0,
        __drv_reportError("Must succeed pool allocations are forbidden. "
			    "Allocation failures cause a system crash"))
    _In_        POOL_TYPE               PoolType 
);


#ifdef SYSVAD_BTH_BYPASS

// Event callback definition.
typedef VOID (*PFNEVENTNOTIFICATION)(
    _In_opt_    PVOID   Context
);

DEFINE_GUID(IID_IBthHfpDeviceCommon,
0x576b824a, 0x5248, 0x47b1, 0x82, 0xc5, 0xe4, 0x7b, 0xa7, 0xe2, 0xaf, 0x2b);

//=============================================================================
// Interfaces
//=============================================================================

///////////////////////////////////////////////////////////////////////////////
// IAdapterCommon
//
DECLARE_INTERFACE_(IBthHfpDeviceCommon, IUnknown)
{
    STDMETHOD_(BOOL,                IsVolumeSupported)
    (
        THIS_
    ) PURE;
    
    STDMETHOD_(PKSPROPERTY_VALUES,  GetVolumeSettings)
    (
        THIS_
        _Out_ PULONG    Size 
    ) PURE;

    STDMETHOD_(LONG,                GetSpeakerVolume)
    (
        THIS_
    ) PURE;

    STDMETHOD_(NTSTATUS,            SetSpeakerVolume)
    (
        THIS_
        _In_ ULONG      Volume
    ) PURE;
    
    STDMETHOD_(LONG,                GetMicVolume)
    (
        THIS_
    ) PURE;

    STDMETHOD_(NTSTATUS,            SetMicVolume)
    (
        THIS_
        _In_ ULONG      Volume
    ) PURE;

    STDMETHOD_(BOOL,                GetConnectionStatus)
    (
        THIS_
    ) PURE;
    
    STDMETHOD_(NTSTATUS,            Connect)
    (
        THIS_
    ) PURE;

    STDMETHOD_(NTSTATUS,            Disconnect)
    (
        THIS_
    ) PURE;
    
    STDMETHOD_(BOOL,                GetStreamStatus)
    (
        THIS_
    ) PURE;

    STDMETHOD_(NTSTATUS,            StreamOpen)
    (
        THIS_
    ) PURE;

    STDMETHOD_(NTSTATUS,            StreamClose)
    (
        THIS_
    ) PURE;

    STDMETHOD_(GUID,                GetContainerId)
    (
        THIS_
    ) PURE;

    STDMETHOD_(VOID,                SetSpeakerVolumeHandler)
    (
        THIS_
        _In_opt_    PFNEVENTNOTIFICATION    EventHandler,
        _In_opt_    PVOID                   EventHandlerContext
    ) PURE;
    
    STDMETHOD_(VOID,                SetSpeakerConnectionStatusHandler)
    (
        THIS_
        _In_opt_    PFNEVENTNOTIFICATION    EventHandler,
        _In_opt_    PVOID                   EventHandlerContext
    ) PURE;
    
    STDMETHOD_(VOID,                SetMicVolumeHandler)
    (
        THIS_
        _In_opt_    PFNEVENTNOTIFICATION    EventHandler,
        _In_opt_    PVOID                   EventHandlerContext
    ) PURE;
    
    STDMETHOD_(VOID,                SetMicConnectionStatusHandler)
    (
        THIS_
        _In_opt_    PFNEVENTNOTIFICATION    EventHandler,
        _In_opt_    PVOID                   EventHandlerContext
    ) PURE;

    STDMETHOD_(BOOL,                IsNRECSupported)
    (
        THIS_
    ) PURE;
    
    STDMETHOD_(BOOL,                GetNRECDisableStatus)
    (
        THIS_
    ) PURE;
};
typedef IBthHfpDeviceCommon *PBTHHFPDEVICECOMMON;

#endif // SYSVAD_BTH_BYPASS

#endif  //_SYSVAD_COMMON_H_

