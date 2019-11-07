/*++

Copyright (c) 1997-2000  Microsoft Corporation All Rights Reserved

Module Name:

    common.cpp

Abstract:

    Implementation of the AdapterCommon class. 

--*/

#pragma warning (disable : 4127)

// Disable the 28204 warning due to annotation mismatches between DDK headers
// for QueryInterface().
//
#pragma warning(disable:28204)

#include <msvad.h>
#include "common.h"
#include "hw.h"
#include "savedata.h"

#define HNS_PER_MS              10000
#define INSTANTIATE_INTERVAL_MS 10000   // 10 seconds between instantiate and uninstantiate

//-----------------------------------------------------------------------------
// Externals
//-----------------------------------------------------------------------------

PSAVEWORKER_PARAM               CSaveData::m_pWorkItems = NULL;
PDEVICE_OBJECT                  CSaveData::m_pDeviceObject = NULL;

typedef
NTSTATUS
(*PMSVADMINIPORTCREATE)
(
    _Out_       PUNKNOWN *,
    _In_        REFCLSID,
    _In_opt_    PUNKNOWN,
    _In_        POOL_TYPE
);

NTSTATUS
CreateMiniportWaveCyclicMSVAD
( 
    OUT PUNKNOWN *,
    IN  REFCLSID,
    IN  PUNKNOWN,
    _When_((PoolType & NonPagedPoolMustSucceed) != 0,
       __drv_reportError("Must succeed pool allocations are forbidden. "
			 "Allocation failures cause a system crash"))
    IN  POOL_TYPE PoolType
);

NTSTATUS
CreateMiniportTopologyMSVAD
( 
    OUT PUNKNOWN *,
    IN  REFCLSID,
    IN  PUNKNOWN,
    _When_((PoolType & NonPagedPoolMustSucceed) != 0,
       __drv_reportError("Must succeed pool allocations are forbidden. "
			 "Allocation failures cause a system crash"))
    IN  POOL_TYPE PoolType
);

//=============================================================================
// Helper Routines
//=============================================================================
//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS
InstallSubdevice
( 
    _In_        PDEVICE_OBJECT          DeviceObject,
    _In_opt_    PIRP                    Irp,
    _In_        PWSTR                   Name,
    _In_        REFGUID                 PortClassId,
    _In_        REFGUID                 MiniportClassId,
    _In_opt_    PMSVADMINIPORTCREATE    MiniportCreate,
    _In_opt_    PUNKNOWN                UnknownAdapter,
    _In_opt_    PRESOURCELIST           ResourceList,
    _Out_opt_   PUNKNOWN *              OutPortUnknown,
    _Out_opt_   PUNKNOWN *              OutMiniportUnknown
)
{
/*++

Routine Description:

    This function creates and registers a subdevice consisting of a port       
    driver, a minport driver and a set of resources bound together.  It will   
    also optionally place a pointer to an interface on the port driver in a    
    specified location before initializing the port driver.  This is done so   
    that a common ISR can have access to the port driver during 
    initialization, when the ISR might fire.                                   

Arguments:

    DeviceObject - pointer to the driver object

    Irp - pointer to the irp object.

    Name - name of the miniport. Passes to PcRegisterSubDevice
 
    PortClassId - port class id. Passed to PcNewPort.

    MiniportClassId - miniport class id. Passed to PcNewMiniport.

    MiniportCreate - pointer to a miniport creation function. If NULL, 
                     PcNewMiniport is used.

    UnknownAdapter - pointer to the adapter object. 
                     Used for initializing the port.

    ResourceList - pointer to the resource list.

    PortInterfaceId - GUID that represents the port interface.
       
    OutPortInterface - pointer to store the port interface

    OutPortUnknown - pointer to store the unknown port interface.

Return Value:

    NT status code.

--*/
    PAGED_CODE();

    ASSERT(DeviceObject);
    ASSERT(Name);

    NTSTATUS                    ntStatus;
    PPORT                       port = NULL;
    PUNKNOWN                    miniport = NULL;
     
    DPF_ENTER(("[InstallSubDevice %S]", Name));

    // Create the port driver object
    //
    ntStatus = PcNewPort(&port, PortClassId);

    // Create the miniport object
    //
    if (NT_SUCCESS(ntStatus))
    {
        if (MiniportCreate)
        {
            ntStatus = 
                MiniportCreate
                ( 
                    &miniport,
                    MiniportClassId,
                    NULL,
                    NonPagedPool 
                );
        }
        else
        {
            ntStatus = 
                PcNewMiniport
                (
                    (PMINIPORT *) &miniport, 
                    MiniportClassId
                );
        }
    }

    // Init the port driver and miniport in one go.
    //
    if (NT_SUCCESS(ntStatus))
    {
#pragma warning(push)
        // IPort::Init's annotation on ResourceList requires it to be non-NULL.  However,
        // for dynamic devices, we may no longer have the resource list and this should
        // still succeed.
        //
#pragma warning(disable:6387)
        ntStatus = 
            port->Init
            ( 
                DeviceObject,
                Irp,
                miniport,
                UnknownAdapter,
                ResourceList 
            );
#pragma warning(pop)

        if (NT_SUCCESS(ntStatus))
        {
            // Register the subdevice (port/miniport combination).
            //
            ntStatus = 
                PcRegisterSubdevice
                ( 
                    DeviceObject,
                    Name,
                    port 
                );
        }
    }

    // Deposit the port interfaces if it's needed.
    //
    if (NT_SUCCESS(ntStatus))
    {
        if (OutPortUnknown)
        {
            ntStatus = 
                port->QueryInterface
                ( 
                    IID_IUnknown,
                    (PVOID *)OutPortUnknown 
                );
        }

        if (OutMiniportUnknown)
        {
            ntStatus = 
                miniport->QueryInterface
                ( 
                    IID_IUnknown,
                    (PVOID *) OutMiniportUnknown 
                );
        }
    }

    if (port)
    {
        port->Release();
    }

    if (miniport)
    {
        miniport->Release();
    }

    return ntStatus;
} // InstallSubDevice

//=============================================================================
#pragma code_seg("PAGE")
IO_WORKITEM_ROUTINE InstantiateTimerWorkRoutine;
void InstantiateTimerWorkRoutine
(
    _In_ DEVICE_OBJECT *_DeviceObject, 
    _In_opt_ PVOID _Context
)
{
	PAGED_CODE();

    UNREFERENCED_PARAMETER(_DeviceObject);

    PADAPTERCOMMON  pCommon = (PADAPTERCOMMON) _Context;

    if (NULL == pCommon)
    {
        // This is completely unexpected, assert here.
        //
        ASSERT(pCommon);
        return;
    }

    // Loop through various states of instantiated and
    // plugged in.
    //
    if (pCommon->IsInstantiated() && pCommon->IsPluggedIn())
    {
        pCommon->UninstantiateDevices();
    }
    else if (pCommon->IsInstantiated() && !pCommon->IsPluggedIn())
    {
        pCommon->Plugin();
    }
    else if (!pCommon->IsInstantiated())
    {
        pCommon->InstantiateDevices();
        pCommon->Unplug();
    }

    // Free the work item that was allocated in the DPC.
    //
	pCommon->FreeInstantiateWorkItem();
}

//=============================================================================
#pragma code_seg()
KDEFERRED_ROUTINE InstantiateTimerNotify;
void InstantiateTimerNotify
(
    IN  PKDPC                   Dpc,
    IN  PVOID                   DeferredContext,
    IN  PVOID                   SA1,
    IN  PVOID                   SA2
)
/*++

Routine Description:

  Dpc routine. This simulates an interrupt service routine to simulate
  a jack plug/unplug.

Arguments:

  Dpc - the Dpc object

  DeferredContext - Pointer to a caller-supplied context to be passed to
                    the DeferredRoutine when it is called

  SA1 - System argument 1

  SA2 - System argument 2

Return Value:

  NT status code.

--*/
{
    UNREFERENCED_PARAMETER(Dpc);
    UNREFERENCED_PARAMETER(SA1);
    UNREFERENCED_PARAMETER(SA2);

    PIO_WORKITEM    pWorkItem = NULL;
    PADAPTERCOMMON  pCommon = (PADAPTERCOMMON) DeferredContext;

    if (NULL == pCommon)
    {
        // This is completely unexpected, assert here.
        //
        ASSERT(pCommon);
        return;
    }

    // Queue a work item to run at PASSIVE_LEVEL so we can call
    // PortCls in order to register or unregister subdevices.
    //
    pWorkItem = IoAllocateWorkItem(pCommon->GetDeviceObject());
    if (NULL != pWorkItem)
    {
        // Store the work item in the adapter common object and queue it.
        //
        if (NT_SUCCESS(pCommon->SetInstantiateWorkItem(pWorkItem)))
        {
            IoQueueWorkItem(pWorkItem, InstantiateTimerWorkRoutine, DelayedWorkQueue, DeferredContext);
        }
        else
        {
            // If we failed to stash the work item in the common adapter object,
            // free it now or else we'll leak it.
            //
            IoFreeWorkItem(pWorkItem);
        }
    }
} // InstantiateTimerNotify


//=============================================================================
// Classes
//=============================================================================

///////////////////////////////////////////////////////////////////////////////
// CAdapterCommon
//   

class CAdapterCommon : 
    public IAdapterCommon,
    public IAdapterPowerManagement,
    public CUnknown    
{
    private:
        PUNKNOWN                m_pPortWave;            // Port Wave Interface
        PUNKNOWN                m_pMiniportWave;        // Miniport Wave Interface
        PUNKNOWN                m_pPortTopology;        // Port Mixer Topology Interface
        PUNKNOWN                m_pMiniportTopology;    // Miniport Mixer Topology Interface
        PSERVICEGROUP           m_pServiceGroupWave;
        PDEVICE_OBJECT          m_pDeviceObject;      
        DEVICE_POWER_STATE      m_PowerState;        
        PCMSVADHW               m_pHW;                  // Virtual MSVAD HW object
        PKTIMER                 m_pInstantiateTimer;    // Timer object
        PRKDPC                  m_pInstantiateDpc;      // Deferred procedure call object
        BOOL                    m_bInstantiated;        // Flag indicating whether or not subdevices are exposed
        BOOL                    m_bPluggedIn;           // Flag indicating whether or not a jack is plugged in
        PIO_WORKITEM            m_pInstantiateWorkItem; // Work Item for instantiate timer callback

        //=====================================================================
        // Helper routines for managing the states of topologies being exposed
        STDMETHODIMP_(NTSTATUS) ExposeMixerTopology();
        STDMETHODIMP_(NTSTATUS) ExposeWaveTopology();
        STDMETHODIMP_(NTSTATUS) UnexposeMixerTopology();
        STDMETHODIMP_(NTSTATUS) UnexposeWaveTopology();
        STDMETHODIMP_(NTSTATUS) ConnectTopologies();
        STDMETHODIMP_(NTSTATUS) DisconnectTopologies();

    public:
        //=====================================================================
        // Default CUnknown
        DECLARE_STD_UNKNOWN();
        DEFINE_STD_CONSTRUCTOR(CAdapterCommon);
        ~CAdapterCommon();

        //=====================================================================
        // Default IAdapterPowerManagement
        IMP_IAdapterPowerManagement;

        //=====================================================================
        // IAdapterCommon methods                                               
        STDMETHODIMP_(NTSTATUS) Init
        (   
            IN  PDEVICE_OBJECT  DeviceObject
        );

        STDMETHODIMP_(PDEVICE_OBJECT)   GetDeviceObject(void);
        STDMETHODIMP_(NTSTATUS)         InstantiateDevices(void);
        STDMETHODIMP_(NTSTATUS)         UninstantiateDevices(void);
        STDMETHODIMP_(NTSTATUS)         Plugin(void);
        STDMETHODIMP_(NTSTATUS)         Unplug(void);
        STDMETHODIMP_(PUNKNOWN *)       WavePortDriverDest(void);

        STDMETHODIMP_(void)     SetWaveServiceGroup
        (   
            IN  PSERVICEGROUP   ServiceGroup
        );

        STDMETHODIMP_(BOOL)     bDevSpecificRead();

        STDMETHODIMP_(void)     bDevSpecificWrite
        (
            IN  BOOL            bDevSpecific
        );
        STDMETHODIMP_(INT)      iDevSpecificRead();

        STDMETHODIMP_(void)     iDevSpecificWrite
        (
            IN  INT             iDevSpecific
        );
        STDMETHODIMP_(UINT)     uiDevSpecificRead();

        STDMETHODIMP_(void)     uiDevSpecificWrite
        (
            IN  UINT            uiDevSpecific
        );

        STDMETHODIMP_(BOOL)     MixerMuteRead
        (
            IN  ULONG           Index
        );

        STDMETHODIMP_(void)     MixerMuteWrite
        (
            IN  ULONG           Index,
            IN  BOOL            Value
        );

        STDMETHODIMP_(ULONG)    MixerMuxRead(void);

        STDMETHODIMP_(void)     MixerMuxWrite
        (
            IN  ULONG           Index
        );

        STDMETHODIMP_(void)     MixerReset(void);

        STDMETHODIMP_(LONG)     MixerVolumeRead
        ( 
            IN  ULONG           Index,
            IN  LONG            Channel
        );

        STDMETHODIMP_(void)     MixerVolumeWrite
        ( 
            IN  ULONG           Index,
            IN  LONG            Channel,
            IN  LONG            Value 
        );

        STDMETHODIMP_(BOOL)     IsInstantiated() { return m_bInstantiated; };

        STDMETHODIMP_(BOOL)     IsPluggedIn() { return m_bPluggedIn; }

        STDMETHODIMP_(NTSTATUS) SetInstantiateWorkItem
        (
            _In_ __drv_aliasesMem   PIO_WORKITEM    WorkItem
        );

        STDMETHODIMP_(NTSTATUS) FreeInstantiateWorkItem();

        //=====================================================================
        // friends

        friend NTSTATUS         NewAdapterCommon
        ( 
            OUT PADAPTERCOMMON * OutAdapterCommon, 
            IN  PRESOURCELIST   ResourceList 
        );
};

//-----------------------------------------------------------------------------
// Functions
//-----------------------------------------------------------------------------

//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS
NewAdapterCommon
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

  Creates a new CAdapterCommon

Arguments:

  Unknown - 

  UnknownOuter -

  PoolType

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(Unknown);

    STD_CREATE_BODY_
    ( 
        CAdapterCommon, 
        Unknown, 
        UnknownOuter, 
        PoolType,      
        PADAPTERCOMMON 
    );
} // NewAdapterCommon

//=============================================================================
CAdapterCommon::~CAdapterCommon
( 
    void 
)
/*++

Routine Description:

  Destructor for CAdapterCommon.

Arguments:

Return Value:

  void

--*/
{
    PAGED_CODE();

    DPF_ENTER(("[CAdapterCommon::~CAdapterCommon]"));

    if (m_pInstantiateTimer)
    {
        KeCancelTimer(m_pInstantiateTimer);
        ExFreePoolWithTag(m_pInstantiateTimer, MSVAD_POOLTAG);
    }

    // Since we just cancelled the  the instantiate timer, wait for all 
    // queued DPCs to complete before we free the instantiate DPC.
    //
    KeFlushQueuedDpcs();

    if (m_pInstantiateDpc)
    {
        ExFreePoolWithTag( m_pInstantiateDpc, MSVAD_POOLTAG );
        // Should also ensure that this destructor is synchronized
        // with the instantiate timer DPC and work item.
        //
    }

    if (m_pHW)
    {
        delete m_pHW;
    }

    CSaveData::DestroyWorkItems();

    if (m_pMiniportWave)
    {
        m_pMiniportWave->Release();
        m_pMiniportWave = NULL;
    }

    if (m_pPortWave)
    {
        m_pPortWave->Release();
        m_pPortWave = NULL;
    }

    if (m_pMiniportTopology)
    {
        m_pMiniportTopology->Release();
        m_pMiniportTopology = NULL;
    }

    if (m_pPortTopology)
    {
        m_pPortTopology->Release();
        m_pPortTopology = NULL;
    }

    if (m_pServiceGroupWave)
    {
        m_pServiceGroupWave->Release();
    }
} // ~CAdapterCommon  

//=============================================================================
#pragma code_seg()
STDMETHODIMP_(PDEVICE_OBJECT)   
CAdapterCommon::GetDeviceObject
(
    void
)
/*++

Routine Description:

  Returns the deviceobject

Arguments:

Return Value:

  PDEVICE_OBJECT

--*/
{
    return m_pDeviceObject;
} // GetDeviceObject

//=============================================================================
#pragma code_seg("PAGE")
NTSTATUS
CAdapterCommon::Init
( 
    IN  PDEVICE_OBJECT          DeviceObject 
)
/*++

Routine Description:

    Initialize adapter common object.

Arguments:

    DeviceObject - pointer to the device object

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(DeviceObject);

    NTSTATUS        ntStatus = STATUS_SUCCESS;

    DPF_ENTER(("[CAdapterCommon::Init]"));

    m_pDeviceObject     = DeviceObject;
    m_PowerState        = PowerDeviceD0;
    m_pPortWave         = NULL;
    m_pMiniportWave     = NULL;
    m_pPortTopology     = NULL;
    m_pMiniportTopology = NULL;
    m_pInstantiateTimer = NULL;
    m_pInstantiateDpc   = NULL;
    m_bInstantiated     = FALSE;
    m_bPluggedIn        = FALSE;
    m_pInstantiateWorkItem = NULL;

    // Initialize HW.
    // 
    m_pHW = new (NonPagedPool, MSVAD_POOLTAG)  CMSVADHW;
    if (!m_pHW)
    {
        DPF(D_TERSE, ("Insufficient memory for MSVAD HW"));
        ntStatus = STATUS_INSUFFICIENT_RESOURCES;
    }
    else
    {
        m_pHW->MixerReset();
    }

    CSaveData::SetDeviceObject(DeviceObject);   //device object is needed by CSaveData

    // Allocate DPC for instantiation timer.
    //
    if (NT_SUCCESS(ntStatus))
    {
        m_pInstantiateDpc = (PRKDPC)
            ExAllocatePoolWithTag
            (
                NonPagedPool,
                sizeof(KDPC),
                MSVAD_POOLTAG
            );
        if (!m_pInstantiateDpc)
        {
            DPF(D_TERSE, ("[Could not allocate memory for DPC]"));
            ntStatus = STATUS_INSUFFICIENT_RESOURCES;
        }
    }

    // Allocate timer for instantiation.
    //
    if (NT_SUCCESS(ntStatus))
    {
        m_pInstantiateTimer = (PKTIMER)
            ExAllocatePoolWithTag
            (
                NonPagedPool,
                sizeof(KTIMER),
                MSVAD_POOLTAG
            );
        if (!m_pInstantiateTimer)
        {
            DPF(D_TERSE, ("[Could not allocate memory for Timer]"));
            ntStatus = STATUS_INSUFFICIENT_RESOURCES;
        }
    }

    // Initialize instantiation timer and DPC.
    //
    if (NT_SUCCESS(ntStatus))
    {
        KeInitializeDpc(m_pInstantiateDpc, InstantiateTimerNotify, this);
        KeInitializeTimerEx(m_pInstantiateTimer, NotificationTimer);

#ifdef _ENABLE_INSTANTIATION_INTERVAL_
        // Set the timer to expire every INSTANTIATE_INTERVAL_MS milliseconds.
        //
        LARGE_INTEGER   liInstantiateInterval = {0};
        liInstantiateInterval.QuadPart = -1 * INSTANTIATE_INTERVAL_MS * HNS_PER_MS;
        KeSetTimerEx
        (
            m_pInstantiateTimer,
            liInstantiateInterval,
            INSTANTIATE_INTERVAL_MS,
            m_pInstantiateDpc
        );
#endif
    }

    return ntStatus;
} // Init

//=============================================================================
STDMETHODIMP_(void)
CAdapterCommon::MixerReset
( 
    void 
)
/*++

Routine Description:

  Reset mixer registers from registry.

Arguments:

Return Value:

  void

--*/
{
    PAGED_CODE();
    
    if (m_pHW)
    {
        m_pHW->MixerReset();
    }
} // MixerReset

//=============================================================================
STDMETHODIMP
CAdapterCommon::NonDelegatingQueryInterface
( 
    _In_         REFIID                      Interface,
    _COM_Outptr_ PVOID *                     Object 
)
/*++

Routine Description:

  QueryInterface routine for AdapterCommon

Arguments:

  Interface - 

  Object -

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();

    ASSERT(Object);

    if (IsEqualGUIDAligned(Interface, IID_IUnknown))
    {
        *Object = PVOID(PUNKNOWN(PADAPTERCOMMON(this)));
    }
    else if (IsEqualGUIDAligned(Interface, IID_IAdapterCommon))
    {
        *Object = PVOID(PADAPTERCOMMON(this));
    }
    else if (IsEqualGUIDAligned(Interface, IID_IAdapterPowerManagement))
    {
        *Object = PVOID(PADAPTERPOWERMANAGEMENT(this));
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
STDMETHODIMP_(void)
CAdapterCommon::SetWaveServiceGroup
( 
    IN PSERVICEGROUP            ServiceGroup 
)
/*++

Routine Description:


Arguments:

Return Value:

  NT status code.

--*/
{
    PAGED_CODE();
    
    DPF_ENTER(("[CAdapterCommon::SetWaveServiceGroup]"));
    
    if (m_pServiceGroupWave)
    {
        m_pServiceGroupWave->Release();
    }

    m_pServiceGroupWave = ServiceGroup;

    if (m_pServiceGroupWave)
    {
        m_pServiceGroupWave->AddRef();
    }
} // SetWaveServiceGroup

//=============================================================================
STDMETHODIMP_(NTSTATUS)
CAdapterCommon::InstantiateDevices
( 
    void 
)
/*++

Routine Description:

  Instantiates the wave and topology ports and exposes them.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    PAGED_CODE();

    NTSTATUS ntStatus = STATUS_SUCCESS;

    if (m_bInstantiated)
    {
        return STATUS_SUCCESS;
    }

    // If the mixer topology port is not exposed, create and expose it.
    //
    ntStatus = ExposeMixerTopology();

    // Create and expose the wave topology.
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = ExposeWaveTopology();
    }

    // Register the physical connection between wave and mixer topologies.
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = ConnectTopologies();
    }

    if (NT_SUCCESS(ntStatus))
    {
        m_bInstantiated = TRUE;
        m_bPluggedIn = TRUE;
    }

    return ntStatus;
} // InstantiateDevices

//=============================================================================
STDMETHODIMP_(NTSTATUS)
CAdapterCommon::UninstantiateDevices
( 
    void 
)
/*++

Routine Description:

  Uninstantiates the wave and topology ports.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    PAGED_CODE();

    NTSTATUS ntStatus = STATUS_SUCCESS;

    // Check if we're already uninstantiated
    //
    if (!m_bInstantiated)
    {
        return ntStatus;
    }

    // Unregister the physical connection between wave and mixer topologies
    // and unregister/unexpose the wave topology. This is the same as being 
    // unplugged.
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = Unplug();
    }

    // Unregister the topo port
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = UnexposeMixerTopology();
    }

    if (NT_SUCCESS(ntStatus))
    {
        m_bInstantiated = FALSE;
        m_bPluggedIn = FALSE;
    }

    return ntStatus;
} // UninstantiateDevices

//=============================================================================
STDMETHODIMP_(NTSTATUS)
CAdapterCommon::Plugin
( 
    void 
)
/*++

Routine Description:

  Called in response to jacks being plugged in.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    PAGED_CODE();

    NTSTATUS ntStatus = STATUS_SUCCESS;

    if (!m_bInstantiated)
    {
        return STATUS_INVALID_DEVICE_STATE;
    }

    if (m_bPluggedIn)
    {
        return ntStatus;
    }

    // Create and expose the wave topology.
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = ExposeWaveTopology();
    }

    // Register the physical connection between wave and mixer topologies.
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = ConnectTopologies();
    }

    if (NT_SUCCESS(ntStatus))
    {
        m_bPluggedIn = TRUE;
    }

    return ntStatus;
} // Plugin

//=============================================================================
STDMETHODIMP_(NTSTATUS)
CAdapterCommon::Unplug
( 
    void 
)
/*++

Routine Description:

  Called in response to jacks being unplugged.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    PAGED_CODE();

    NTSTATUS ntStatus = STATUS_SUCCESS;

    if (!m_bInstantiated)
    {
        return STATUS_INVALID_DEVICE_STATE;
    }

    if (!m_bPluggedIn)
    {
        return ntStatus;
    }

    // Unregister the physical connection between wave and mixer topologies.
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = DisconnectTopologies();
    }

    // Unregister and destroy the wave port
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = UnexposeWaveTopology();
    }

    if (NT_SUCCESS(ntStatus))
    {
        m_bPluggedIn = FALSE;
    }

    return ntStatus;
} // Unplug

STDMETHODIMP_(NTSTATUS) 
CAdapterCommon::ExposeMixerTopology
(
    void
)
/*++

Routine Description:

  Creates and registers the mixer topology.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    NTSTATUS ntStatus = STATUS_SUCCESS;

    PAGED_CODE();

    if (m_pPortTopology)
    {
        return ntStatus;
    }

    ntStatus = InstallSubdevice( 
        m_pDeviceObject,
        NULL,
        L"Topology",
        CLSID_PortTopology,
        CLSID_PortTopology, 
        CreateMiniportTopologyMSVAD,
        PUNKNOWN(PADAPTERCOMMON(this)),
        NULL,
        &m_pPortTopology,
        &m_pMiniportTopology);

    return ntStatus;
}

STDMETHODIMP_(NTSTATUS) 
CAdapterCommon::ExposeWaveTopology
(
    void
)
/*++

Routine Description:

  Creates and registers wave topology.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    NTSTATUS ntStatus = STATUS_SUCCESS;

    PAGED_CODE();

    if (m_pPortWave)
    {
        return ntStatus;
    }

    ntStatus = InstallSubdevice( 
        m_pDeviceObject,
        NULL,
        L"Wave",
        CLSID_PortWaveCyclic,
        CLSID_PortWaveCyclic,   
        CreateMiniportWaveCyclicMSVAD,
        PUNKNOWN(PADAPTERCOMMON(this)),
        NULL,
        &m_pPortWave,
        &m_pMiniportWave);

    return ntStatus;
}

STDMETHODIMP_(NTSTATUS) 
CAdapterCommon::UnexposeMixerTopology
(
    void
)
/*++

Routine Description:

  Unregisters and releases the mixer topology.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    NTSTATUS                        ntStatus = STATUS_SUCCESS;
    PUNREGISTERSUBDEVICE            pUnregisterSubdevice = NULL;

    PAGED_CODE();

    if (NULL == m_pPortTopology)
    {
        return ntStatus;
    }

    // Get the IUnregisterSubdevice interface.
    //
    ntStatus = m_pPortTopology->QueryInterface( IID_IUnregisterSubdevice,
                                                (PVOID *)&pUnregisterSubdevice);

    // Unregister the topo port.
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = pUnregisterSubdevice->UnregisterSubdevice(
            m_pDeviceObject,
            m_pPortTopology);

        // Release the IUnregisterSubdevice interface.
        //
        pUnregisterSubdevice->Release();

        // At this point, we're done with the mixer topology and 
        // the miniport.
        //
        if (NT_SUCCESS(ntStatus))
        {
            m_pPortTopology->Release();
            m_pPortTopology = NULL;

            m_pMiniportTopology->Release();
            m_pMiniportTopology = NULL;
        }
    }

    return ntStatus;
}

STDMETHODIMP_(NTSTATUS)
CAdapterCommon::UnexposeWaveTopology
(
    void
)
/*++

Routine Description:

  Unregisters and releases the wave topology.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    NTSTATUS                        ntStatus = STATUS_SUCCESS;
    PUNREGISTERSUBDEVICE            pUnregisterSubdevice = NULL;

    PAGED_CODE();

    if (NULL == m_pPortWave)
    {
        return ntStatus;
    }

    // Get the IUnregisterSubdevice interface.
    //
    ntStatus = m_pPortWave->QueryInterface( IID_IUnregisterSubdevice,
                                                (PVOID *)&pUnregisterSubdevice);

    // Unregister the wave port.
    //
    if (NT_SUCCESS(ntStatus))
    {
        ntStatus = pUnregisterSubdevice->UnregisterSubdevice(
            m_pDeviceObject,
            m_pPortWave);
    
        // Release the IUnregisterSubdevice interface.
        //
        pUnregisterSubdevice->Release();

        // At this point, we're done with the mixer topology and 
        // the miniport.
        //
        if (NT_SUCCESS(ntStatus))
        {
            m_pPortWave->Release();
            m_pPortWave = NULL;

            m_pMiniportWave->Release();
            m_pMiniportWave = NULL;
        }
    }
    return ntStatus;
}

STDMETHODIMP_(NTSTATUS)
CAdapterCommon::ConnectTopologies
(
    void
)
/*++

Routine Description:

  Connects the bridge pins between the wave and mixer topologies.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    NTSTATUS ntStatus = STATUS_SUCCESS;

    PAGED_CODE();

    // Connect the capture path.
    //
    if ((TopologyPhysicalConnections.ulTopologyOut != (ULONG)-1) &&
        (TopologyPhysicalConnections.ulWaveIn != (ULONG)-1))
    {
        ntStatus = PcRegisterPhysicalConnection( 
            m_pDeviceObject,
            m_pPortTopology,
            TopologyPhysicalConnections.ulTopologyOut,
            m_pPortWave,
            TopologyPhysicalConnections.ulWaveIn);
    }

    // Connect the render path.
    //
    if (NT_SUCCESS(ntStatus))
    {
        if ((TopologyPhysicalConnections.ulWaveOut != (ULONG)-1) &&
            (TopologyPhysicalConnections.ulTopologyIn != (ULONG)-1))
        {
            ntStatus =
                PcRegisterPhysicalConnection
                ( 
                    m_pDeviceObject,
                    m_pPortWave,
                    TopologyPhysicalConnections.ulWaveOut,
                    m_pPortTopology,
                    TopologyPhysicalConnections.ulTopologyIn
                );
        }
    }

    return ntStatus;
}

STDMETHODIMP_(NTSTATUS)
CAdapterCommon::DisconnectTopologies
(
    void
)
/*++

Routine Description:

  Disconnects the bridge pins between the wave and mixer topologies.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    NTSTATUS                        ntStatus    = STATUS_SUCCESS;
    NTSTATUS                        ntStatus2   = STATUS_SUCCESS;
    PUNREGISTERPHYSICALCONNECTION   pUnregisterPhysicalConnection = NULL;

    PAGED_CODE();

    //
    // Get the IUnregisterPhysicalConnection interface
    //
    ntStatus = m_pPortTopology->QueryInterface( IID_IUnregisterPhysicalConnection,
                                                (PVOID *)&pUnregisterPhysicalConnection);
    if (NT_SUCCESS(ntStatus))
    {
        // 
        // Remove the render physical connection
        //
        if ((TopologyPhysicalConnections.ulWaveOut != (ULONG)-1) &&
            (TopologyPhysicalConnections.ulTopologyIn != (ULONG)-1))
        {
            ntStatus = pUnregisterPhysicalConnection->UnregisterPhysicalConnection(
                m_pDeviceObject,
                m_pPortWave,
                TopologyPhysicalConnections.ulWaveOut,
                m_pPortTopology,
                TopologyPhysicalConnections.ulTopologyIn);

            if(!NT_SUCCESS(ntStatus))
            {
                DPF(D_TERSE, ("DisconnectTopologies: UnregisterPhysicalConnection(render) failed, 0x%x", ntStatus));
            }
        }

        //
        // Remove the capture physical connection
        //
        if ((TopologyPhysicalConnections.ulTopologyOut != (ULONG)-1) &&
            (TopologyPhysicalConnections.ulWaveIn != (ULONG)-1))
        {
            ntStatus2 = pUnregisterPhysicalConnection->UnregisterPhysicalConnection(
                m_pDeviceObject,
                m_pPortTopology,
                TopologyPhysicalConnections.ulTopologyOut,
                m_pPortWave,
                TopologyPhysicalConnections.ulWaveIn);
            
            if(!NT_SUCCESS(ntStatus2))
            {
                DPF(D_TERSE, ("DisconnectTopologies: UnregisterPhysicalConnection(capture) failed, 0x%x", ntStatus2));
                if (NT_SUCCESS(ntStatus))
                {
                    ntStatus = ntStatus2;
                }
            }
        }
    }

    SAFE_RELEASE(pUnregisterPhysicalConnection);
    
    return ntStatus;
}

#pragma code_seg()
//=============================================================================
STDMETHODIMP_(NTSTATUS)		
CAdapterCommon::SetInstantiateWorkItem
(
    _In_ __drv_aliasesMem PIO_WORKITEM WorkItem
)
/*++

Routine Description:

  Sets the work item that will be called to instantiate or 
  uninstantiate topologies.

Arguments:

  PIO_WORKITEM - [in] work item that was previously allocated.

Return Value:

  NTSTATUS

--*/
{
    // Make sure there isn't already a work item allocated.
    //
    if ( m_pInstantiateWorkItem != NULL )
    {
        return STATUS_INVALID_DEVICE_STATE;
    }

    // Stash the work item to be free'd after the work routine is called.
    //
    m_pInstantiateWorkItem = WorkItem;

    return STATUS_SUCCESS;
}

#pragma code_seg("PAGE")
//=============================================================================
STDMETHODIMP_(NTSTATUS)
CAdapterCommon::FreeInstantiateWorkItem()
/*++

Routine Description:

  Frees a work item that was called to instantiate or 
  uninstantiate topologies.

Arguments:

Return Value:

  NTSTATUS

--*/
{
    PAGED_CODE();

    // Make sure we actually have a work item set.
    //
    if ( m_pInstantiateWorkItem == NULL )
    {
        return STATUS_INVALID_DEVICE_STATE;
    }

    // Go ahead and free the work item.
    //
    IoFreeWorkItem( m_pInstantiateWorkItem );
    m_pInstantiateWorkItem = NULL;

    return STATUS_SUCCESS;
}

//=============================================================================
STDMETHODIMP_(PUNKNOWN *)
CAdapterCommon::WavePortDriverDest
( 
    void 
)
/*++

Routine Description:

  Returns the wave port.

Arguments:

Return Value:

  PUNKNOWN : pointer to waveport

--*/
{
    PAGED_CODE();

    return &m_pPortWave;
} // WavePortDriverDest

#pragma code_seg()

//=============================================================================
STDMETHODIMP_(BOOL)
CAdapterCommon::bDevSpecificRead()
/*++

Routine Description:

  Fetch Device Specific information.

Arguments:

  N/A

Return Value:

    BOOL - Device Specific info

--*/
{
    if (m_pHW)
    {
        return m_pHW->bGetDevSpecific();
    }

    return FALSE;
} // bDevSpecificRead

//=============================================================================
STDMETHODIMP_(void)
CAdapterCommon::bDevSpecificWrite
(
    IN  BOOL                    bDevSpecific
)
/*++

Routine Description:

  Store the new value in the Device Specific location.

Arguments:

  bDevSpecific - Value to store

Return Value:

  N/A.

--*/
{
    if (m_pHW)
    {
        m_pHW->bSetDevSpecific(bDevSpecific);
    }
} // DevSpecificWrite

//=============================================================================
STDMETHODIMP_(INT)
CAdapterCommon::iDevSpecificRead()
/*++

Routine Description:

  Fetch Device Specific information.

Arguments:

  N/A

Return Value:

    INT - Device Specific info

--*/
{
    if (m_pHW)
    {
        return m_pHW->iGetDevSpecific();
    }

    return 0;
} // iDevSpecificRead

//=============================================================================
STDMETHODIMP_(void)
CAdapterCommon::iDevSpecificWrite
(
    IN  INT                    iDevSpecific
)
/*++

Routine Description:

  Store the new value in the Device Specific location.

Arguments:

  iDevSpecific - Value to store

Return Value:

  N/A.

--*/
{
    if (m_pHW)
    {
        m_pHW->iSetDevSpecific(iDevSpecific);
    }
} // iDevSpecificWrite

//=============================================================================
STDMETHODIMP_(UINT)
CAdapterCommon::uiDevSpecificRead()
/*++

Routine Description:

  Fetch Device Specific information.

Arguments:

  N/A

Return Value:

    UINT - Device Specific info

--*/
{
    if (m_pHW)
    {
        return m_pHW->uiGetDevSpecific();
    }

    return 0;
} // uiDevSpecificRead

//=============================================================================
STDMETHODIMP_(void)
CAdapterCommon::uiDevSpecificWrite
(
    IN  UINT                    uiDevSpecific
)
/*++

Routine Description:

  Store the new value in the Device Specific location.

Arguments:

  uiDevSpecific - Value to store

Return Value:

  N/A.

--*/
{
    if (m_pHW)
    {
        m_pHW->uiSetDevSpecific(uiDevSpecific);
    }
} // uiDevSpecificWrite

//=============================================================================
STDMETHODIMP_(BOOL)
CAdapterCommon::MixerMuteRead
(
    IN  ULONG                   Index
)
/*++

Routine Description:

  Store the new value in mixer register array.

Arguments:

  Index - node id

Return Value:

    BOOL - mixer mute setting for this node

--*/
{
    if (m_pHW)
    {
        return m_pHW->GetMixerMute(Index);
    }

    return 0;
} // MixerMuteRead

//=============================================================================
STDMETHODIMP_(void)
CAdapterCommon::MixerMuteWrite
(
    IN  ULONG                   Index,
    IN  BOOL                    Value
)
/*++

Routine Description:

  Store the new value in mixer register array.

Arguments:

  Index - node id

  Value - new mute settings

Return Value:

  NT status code.

--*/
{
    if (m_pHW)
    {
        m_pHW->SetMixerMute(Index, Value);
    }
} // MixerMuteWrite

//=============================================================================
STDMETHODIMP_(ULONG)
CAdapterCommon::MixerMuxRead() 
/*++

Routine Description:

  Return the mux selection

Arguments:

  Index - node id

  Value - new mute settings

Return Value:

  NT status code.

--*/
{
    if (m_pHW)
    {
        return m_pHW->GetMixerMux();
    }

    return 0;
} // MixerMuxRead

//=============================================================================
STDMETHODIMP_(void)
CAdapterCommon::MixerMuxWrite
(
    IN  ULONG                   Index
)
/*++

Routine Description:

  Store the new mux selection

Arguments:

  Index - node id

  Value - new mute settings

Return Value:

  NT status code.

--*/
{
    if (m_pHW)
    {
        m_pHW->SetMixerMux(Index);
    }
} // MixerMuxWrite

//=============================================================================
STDMETHODIMP_(LONG)
CAdapterCommon::MixerVolumeRead
( 
    IN  ULONG                   Index,
    IN  LONG                    Channel
)
/*++

Routine Description:

  Return the value in mixer register array.

Arguments:

  Index - node id

  Channel = which channel

Return Value:

    Byte - mixer volume settings for this line

--*/
{
    if (m_pHW)
    {
        return m_pHW->GetMixerVolume(Index, Channel);
    }

    return 0;
} // MixerVolumeRead

//=============================================================================
STDMETHODIMP_(void)
CAdapterCommon::MixerVolumeWrite
( 
    IN  ULONG                   Index,
    IN  LONG                    Channel,
    IN  LONG                    Value
)
/*++

Routine Description:

  Store the new value in mixer register array.

Arguments:

  Index - node id

  Channel - which channel

  Value - new volume level

Return Value:

    void

--*/
{
    if (m_pHW)
    {
        m_pHW->SetMixerVolume(Index, Channel, Value);
    }
} // MixerVolumeWrite

//=============================================================================
STDMETHODIMP_(void)
CAdapterCommon::PowerChangeState
( 
    _In_  POWER_STATE             NewState 
)
/*++

Routine Description:


Arguments:

  NewState - The requested, new power state for the device. 

Return Value:

    void

--*/
{
    DPF_ENTER(("[CAdapterCommon::PowerChangeState]"));

    // is this actually a state change??
    //
    if (NewState.DeviceState != m_PowerState)
    {
        // switch on new state
        //
        switch (NewState.DeviceState)
        {
            case PowerDeviceD0:
            case PowerDeviceD1:
            case PowerDeviceD2:
            case PowerDeviceD3:
                m_PowerState = NewState.DeviceState;

                DPF
                ( 
                    D_VERBOSE, 
                    ("Entering D%d", ULONG(m_PowerState) - ULONG(PowerDeviceD0)) 
                );

                break;
    
            default:
            
                DPF(D_VERBOSE, ("Unknown Device Power State"));
                break;
        }
    }
} // PowerStateChange

//=============================================================================
_Use_decl_annotations_
STDMETHODIMP_(NTSTATUS)
CAdapterCommon::QueryDeviceCapabilities
( 
    PDEVICE_CAPABILITIES    PowerDeviceCaps 
)
/*++

Routine Description:

    Called at startup to get the caps for the device.  This structure provides 
    the system with the mappings between system power state and device power 
    state.  This typically will not need modification by the driver.         

Arguments:

  PowerDeviceCaps - The device's capabilities. 

Return Value:

  NT status code.

--*/
{
    UNREFERENCED_PARAMETER(PowerDeviceCaps);

    DPF_ENTER(("[CAdapterCommon::QueryDeviceCapabilities]"));

    return (STATUS_SUCCESS);
} // QueryDeviceCapabilities

//=============================================================================
STDMETHODIMP_(NTSTATUS)
CAdapterCommon::QueryPowerChangeState
( 
    _In_  POWER_STATE             NewStateQuery 
)
/*++

Routine Description:

  Query to see if the device can change to this power state 

Arguments:

  NewStateQuery - The requested, new power state for the device

Return Value:

  NT status code.

--*/
{
    UNREFERENCED_PARAMETER(NewStateQuery);

    DPF_ENTER(("[CAdapterCommon::QueryPowerChangeState]"));

    return STATUS_SUCCESS;
} // QueryPowerChangeState

