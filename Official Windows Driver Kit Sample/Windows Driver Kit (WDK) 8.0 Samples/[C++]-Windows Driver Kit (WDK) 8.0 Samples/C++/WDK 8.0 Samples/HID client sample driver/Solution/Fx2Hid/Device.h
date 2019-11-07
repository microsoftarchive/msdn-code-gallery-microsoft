/*++

Copyright (C) Microsoft Corporation, All Rights Reserved

Module Name:

	Device.h

Abstract:

	This module contains the type definitions for the Fx2Hid
	driver's device callback class.

Environment:

	Windows User-Mode Driver Framework (WUDF)

--*/

#pragma once
#include "internal.h"

//
// FX2 definitions
//

typedef struct {
	UCHAR Segments;
} SEVEN_SEGMENT, *PSEVEN_SEGMENT;

#define BARGRAPH_LED_1_ON		0x1
#define BARGRAPH_LED_2_ON		0x2
#define BARGRAPH_LED_3_ON		0x4
#define BARGRAPH_LED_4_ON		0x8
#define BARGRAPH_LED_5_ON		0x10
#define BARGRAPH_LED_6_ON		0x20
#define BARGRAPH_LED_7_ON		0x40
#define BARGRAPH_LED_8_ON		0x80
#define BARGRAPH_LED_ALL_ON		0xff
#define BARGRAPH_LED_ALL_OFF	0x00

//
// HID-specific definitions from hidusbfx2, which exposes the fx2 device
// as a custom HID collection.
//
#define DEP_SWITCHES_REPORT_ID        3
#define SEVEN_SEGMENT_REPORT_ID       4
#define BARGRAPH_REPORT_ID            5

typedef struct _HIDFX2_INPUT_REPORT {

	//
	//Report ID for the collection
	//
	BYTE ReportId;

	union {
		struct {
			//
			// Individual switches starting from the 
			//  right of the set of switches
			//
			BYTE SwitchBrowser : 1;
			BYTE SwitchBack : 1;
			BYTE SwitchForward : 1;
			BYTE SwitchRefresh : 1;
			BYTE SwitchBookMarks : 1;
			BYTE SwitchMail : 1;
			BYTE SwitchCalculator : 1;
			BYTE Padding : 1;
			
		} ConsumerControlReport;

		struct {
			//
			// Individual switches starting from the 
			//  right of the set of switches
			//
			BYTE Padding : 7;
			BYTE SwitchPowerSleep : 1;
			
		} SystemControlReport;

		//
		// The state of all the switches as a single
		// UCHAR
		//
		
		BYTE SwitchStateAsByte;
	};
}HIDFX2_INPUT_REPORT, *PHIDFX2_INPUT_REPORT;

typedef struct _HIDFX2_FEATURE_REPORT {
	//
	//Report ID for the collection
	//
	BYTE ReportId;

	//
	//one-byte feature data from 7-segment display or bar graph
	//
	BYTE FeatureData;

}HIDFX2_FEATURE_REPORT, *PHIDFX2_FEATURE_REPORT;

//
// Class for the Fx2 HID driver.
//

class CMyDevice : 
	public CUnknown,
	public IPnpCallbackHardware,
	public IPnpCallback,
	public IRequestCallbackRequestCompletion
{
//
// Private data members.
//
private:
	//
	// Framework objects
	//	
	IWDFDevice				*m_FxDevice; // weak reference
	IWDFDriver				*m_WdfDriver;
	IWDFDriverCreatedFile	*m_WdfFile;
	IWDFIoTarget			*m_WdfIoTarget;


	// 
	// Weak reference to the control queue
	//
	PCMyReadWriteQueue		m_ReadWriteQueue;

	// 
	// Weak reference to the control queue
	//
	PCMyControlQueue		m_ControlQueue;

	//
	// Current switch state
	//
	SWITCH_STATE			m_SwitchState;
	
	//
	// Switch state buffer - this might hold the transient value
	// m_SwitchState holds stable value of the switch state
	//
	HIDFX2_INPUT_REPORT		m_SwitchStateReport;

	//
	// A manual queue to hold requests for changes in the I/O switch state.
	//

	IWDFIoQueue				*m_SwitchChangeQueue;

	//
	// HID structures
	//
	PHIDP_PREPARSED_DATA	m_Ppd;			// The opaque parser info describing this device
	HIDP_CAPS				m_Caps;			// The Capabilities of this hid device.

//
// Private methods.
//

private:

	CMyDevice(
		VOID
		) :
		m_FxDevice(nullptr),
		m_WdfDriver(nullptr),
		m_WdfFile(nullptr),
		m_WdfIoTarget(nullptr),
		m_ControlQueue(nullptr),
		m_ReadWriteQueue(nullptr),
		m_SwitchChangeQueue(nullptr),
		m_Ppd(NULL)
	{
		m_SwitchState.SwitchesAsUChar = 0;
		ZeroMemory(&m_SwitchStateReport, sizeof (m_SwitchStateReport));
	}

	~CMyDevice(
		);

	HRESULT
	Initialize(
		_In_ IWDFDriver *FxDriver,
		_In_ IWDFDeviceInitialize *FxDeviceInit
		);

	//
	// Helper methods
	//
   
	HRESULT
	GetHidCapabilities(
		VOID
		);

	HRESULT
	IndicateDeviceReady(
		VOID
		);

	VOID
	ServiceSwitchChangeQueue(
		_In_ SWITCH_STATE NewState,
		_In_ HRESULT CompletionStatus,
		_In_opt_ IWDFFile *SpecificFile
		);

	HRESULT
	CreateAndSendIoctlRequest(
		_In_reads_bytes_opt_(InputBufferSize) BYTE* InputBuffer,
		_In_ ULONG InputBufferSize,
		_In_reads_bytes_opt_(OutputBufferSize) BYTE* OutputBuffer,
		_In_ ULONG OutputBufferSize,
		_In_ ULONG IoctlCode
		);

	HRESULT 
	SendReadRequest(
		_In_ IWDFIoRequest *pWdfRequest
		);

//
// Public methods
//
public:

	//
	// The factory method used to create an instance of this driver.
	//
	
	static
	HRESULT
	CreateInstance(
		_In_ IWDFDriver *FxDriver,
		_In_ IWDFDeviceInitialize *FxDeviceInit,
		_Out_ PCMyDevice *Device
		);

	IWDFDevice *
	GetFxDevice(
		VOID
		)
	{
		return m_FxDevice;
	}

	HRESULT
	Configure(
		VOID
		);

	IPnpCallback *
	QueryIPnpCallback(
		VOID
		)
	{
		AddRef();
		return static_cast<IPnpCallback *>(this);
	}

	IPnpCallbackHardware *
	QueryIPnpCallbackHardware(
		VOID
		)
	{
		AddRef();
		return static_cast<IPnpCallbackHardware *>(this);
	}

	IRequestCallbackRequestCompletion *
	QueryIRequestCallbackRequestCompletion(
		VOID
		)
	{
		AddRef();
		return static_cast<IRequestCallbackRequestCompletion *>(this);
	}

	HRESULT
	GetBarGraphDisplay(
		_In_ PBAR_GRAPH_STATE BarGraphState
		);

	HRESULT
	SetBarGraphDisplay(
		_In_ PBAR_GRAPH_STATE BarGraphState
		);

	HRESULT
	GetSevenSegmentDisplay(
		_In_ PSEVEN_SEGMENT SevenSegment
		);

	HRESULT
	SetSevenSegmentDisplay(
		_In_ PSEVEN_SEGMENT SevenSegment
		);

	HRESULT
	ReadSwitchState(
		_In_ PSWITCH_STATE SwitchState
		);

	IWDFIoQueue *
	GetSwitchChangeQueue(
		VOID
		)
	{
		return m_SwitchChangeQueue;
	}
	
	PSWITCH_STATE
	GetCurrentSwitchState(
		VOID
		)
	{
		return &m_SwitchState;
	}


//
// COM methods
//
public:

	//
	// IUnknown methods.
	//

	virtual
	ULONG
	STDMETHODCALLTYPE
	AddRef(
		VOID
		)
	{
		return __super::AddRef();
	}

	_At_(this, __drv_freesMem(object))
	virtual
	ULONG
	STDMETHODCALLTYPE
	Release(
		VOID
	   )
	{
		return __super::Release();
	}

	virtual
	HRESULT
	STDMETHODCALLTYPE
	QueryInterface(
		_In_ REFIID InterfaceId,
		_Outptr_ PVOID *Object
		);

	//
	// IPnpCallbackHardware
	//

	virtual
	HRESULT
	STDMETHODCALLTYPE
	OnPrepareHardware(
			_In_ IWDFDevice *FxDevice
			);

	virtual
	HRESULT
	STDMETHODCALLTYPE
	OnReleaseHardware(
		_In_ IWDFDevice *FxDevice
		);

	
	//
	// IPnpCallback
	//
	virtual	
	HRESULT
	STDMETHODCALLTYPE
	OnD0Entry(
		_In_ IWDFDevice*  pWdfDevice,
		_In_ WDF_POWER_DEVICE_STATE  previousState
	);
	
	virtual	
	HRESULT
	STDMETHODCALLTYPE
	OnD0Exit(
		_In_ IWDFDevice*  pWdfDevice,
		_In_ WDF_POWER_DEVICE_STATE  previousState
	);

	virtual
	void
	STDMETHODCALLTYPE
	OnSurpriseRemoval(
		_In_ IWDFDevice*  pWdfDevice
		);

	virtual
	HRESULT
	STDMETHODCALLTYPE
	OnQueryRemove(
		_In_ IWDFDevice*  pWdfDevice
		);

	virtual
	HRESULT
	STDMETHODCALLTYPE
	OnQueryStop(
		_In_ IWDFDevice*  pWdfDevice
		);	

	//
	// IRequestCallbackRequestCompletion
	//
	virtual 
	void 
	STDMETHODCALLTYPE 
	OnCompletion(
		_In_ IWDFIoRequest*					pRequest,
		_In_ IWDFIoTarget*					pIoTarget,
		_In_ IWDFRequestCompletionParams*	CompletionParams,
		_In_ PVOID							Context
		);

};

