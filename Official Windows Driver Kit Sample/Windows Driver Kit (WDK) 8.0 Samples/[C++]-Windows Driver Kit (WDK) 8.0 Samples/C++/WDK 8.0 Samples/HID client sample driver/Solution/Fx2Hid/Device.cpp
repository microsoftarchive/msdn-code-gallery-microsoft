/*++
 
Copyright (C) Microsoft Corporation, All Rights Reserved.

Module Name:

	Device.cpp

Abstract:

	This module contains the implementation of the Fx2 HID sample driver's
	device callback object.

Environment:

   Windows User-Mode Driver Framework (WUDF)

--*/
#include "internal.h"
#include "initguid.h"
#include <setupapi.h>
#include <devpkey.h>

#include "device.tmh"

CMyDevice::~CMyDevice(
	)
{
}

HRESULT
CMyDevice::CreateInstance(
	_In_ IWDFDriver *FxDriver,
	_In_ IWDFDeviceInitialize * FxDeviceInit,
	_Out_ PCMyDevice *Device
	)
/*++
 
  Routine Description:

	This method creates and initializs an instance of the driver's 
	device callback object.

  Arguments:

	FxDeviceInit - the settings for the device.

	Device - a location to store the referenced pointer to the device object.

  Return Value:

	Status

--*/
{
	PCMyDevice device = nullptr;
	HRESULT hr;

	//
	// Allocate a new instance of the device class.
	//

	device = new CMyDevice();

	if (nullptr == device)
	{
		return E_OUTOFMEMORY;
	}

	//
	// Initialize the instance.
	//

	hr = device->Initialize(FxDriver, FxDeviceInit);

	if (SUCCEEDED(hr)) 
	{
		*Device = device;
	} 
	else 
	{
		device->Release();
		device = nullptr;
	}

	return hr;
}

HRESULT
CMyDevice::Initialize(
	_In_ IWDFDriver		   * FxDriver,
	_In_ IWDFDeviceInitialize * FxDeviceInit
	)
/*++
 
  Routine Description:

	This method initializes the device callback object and creates the
	partner device object.

	The method should perform any device-specific configuration that:
		*  could fail (these can't be done in the constructor)
		*  must be done before the partner object is created -or-
		*  can be done after the partner object is created and which aren't 
		   influenced by any device-level parameters the parent (the driver
		   in this case) might set.

  Arguments:

	FxDeviceInit - the settings for this device.

  Return Value:

	status.

--*/
{
	IWDFDevice *fxDevice = NULL;

	HRESULT hr = S_OK;

	//
	// TODO: If you're writing a filter driver then indicate that here. 
	//
	// FxDeviceInit->SetFilter();
	//
		
	//
	// TODO: Any per-device initialization which must be done before 
	//	   creating the partner object.
	//

	//
	// Set no locking unless you need an automatic callbacks synchronization
	//

	FxDeviceInit->SetLockingConstraint(None);

	//
	// Create a new FX device object and assign the new callback object to 
	// handle any device level events that occur.
	//

	//
	// QueryIUnknown references the IUnknown interface that it returns
	// (which is the same as referencing the device).  We pass that to 
	// CreateDevice, which takes its own reference if everything works.
	//

	if (SUCCEEDED(hr)) 
	{
		IUnknown *unknown = this->QueryIUnknown();

		hr = FxDriver->CreateDevice(FxDeviceInit, unknown, &fxDevice);

		unknown->Release();
	}

	//
	// If that succeeded then set our FxDevice member variable.
	//

	if (SUCCEEDED(hr)) 
	{
		m_FxDevice = fxDevice;

		//
		// Drop the reference we got from CreateDevice.  Since this object
		// is partnered with the framework object they have the same 
		// lifespan - there is no need for an additional reference.
		//

		fxDevice->Release();
	}

	return hr;
}

HRESULT
CMyDevice::Configure(
	VOID
	)
/*++
 
  Routine Description:

	This method is called after the device callback object has been initialized 
	and returned to the driver.  It would setup the device's queues and their 
	corresponding callback objects.

  Arguments:

	FxDevice - the framework device object for which we're handling events.

  Return Value:

	status

--*/
{   
	HRESULT hr = S_OK;

	hr = CMyReadWriteQueue::CreateInstance(this, &m_ReadWriteQueue);

	if (FAILED(hr))
	{
		return hr;
	}

	//
	// We use default queue for read/write
	//
	
	hr = m_ReadWriteQueue->Configure();

	m_ReadWriteQueue->Release();

	//
	// Create the control queue and configure forwarding for IOCTL requests.
	//

	if (SUCCEEDED(hr)) 
	{
		hr = CMyControlQueue::CreateInstance(this, &m_ControlQueue);

		if (SUCCEEDED(hr)) 
		{
			hr = m_ControlQueue->Configure();
			if (SUCCEEDED(hr)) 
			{
				m_FxDevice->ConfigureRequestDispatching(
								m_ControlQueue->GetFxQueue(),
								WdfRequestDeviceIoControl,
								true
								);
			}
			m_ControlQueue->Release();		 
		}
	}

	//
	// Create a manual I/O queue to hold requests for notification when
	// the switch state changes.
	//

	hr = m_FxDevice->CreateIoQueue(NULL,
								   FALSE,
								   WdfIoQueueDispatchManual,
								   FALSE,
								   FALSE,
								   &m_SwitchChangeQueue);
	

	//
	// Release creation reference as object tree will keep a reference
	//
	
	m_SwitchChangeQueue->Release();

	if (SUCCEEDED(hr)) 
	{
		// TODO:  Expose your device interface GUID here.
		hr = m_FxDevice->CreateDeviceInterface(&GUID_DEVINTERFACE_OSRUSBFX2,
											   NULL);
	}

	//
	// Mark the interface as restricted to allow access to applications bound
	// using device metadata.
	//
	if (SUCCEEDED(hr))
	{
		WDF_PROPERTY_STORE_ROOT RootSpecifier;
		IWDFUnifiedPropertyStoreFactory * pUnifiedPropertyStoreFactory = NULL;
		IWDFUnifiedPropertyStore * pUnifiedPropertyStore = NULL;
		DEVPROP_BOOLEAN isRestricted = DEVPROP_TRUE;

		hr = m_FxDevice->QueryInterface(IID_PPV_ARGS(&pUnifiedPropertyStoreFactory));

		WUDF_TEST_DRIVER_ASSERT(SUCCEEDED(hr));

		RootSpecifier.LengthCb = sizeof(RootSpecifier);
		RootSpecifier.RootClass = WdfPropertyStoreRootClassDeviceInterfaceKey;
		// TODO:  Use your device interface GUID here.
		RootSpecifier.Qualifier.DeviceInterfaceKey.InterfaceGUID = &GUID_DEVINTERFACE_OSRUSBFX2;
		RootSpecifier.Qualifier.DeviceInterfaceKey.ReferenceString = NULL;

		hr = pUnifiedPropertyStoreFactory->RetrieveUnifiedDevicePropertyStore(&RootSpecifier,
																					 &pUnifiedPropertyStore);

		if (SUCCEEDED(hr))
		{
			hr = pUnifiedPropertyStore->SetPropertyData(&DEVPKEY_DeviceInterface_Restricted,
															   0, // Lcid
															   0, // Flags
															   DEVPROP_TYPE_BOOLEAN,
															   sizeof(isRestricted),
															   &isRestricted);
		}

		if (FAILED(hr))
		{
			TraceEvents(TRACE_LEVEL_ERROR, 
						TEST_TRACE_DEVICE, 
						"%!FUNC! Could not set restricted property %!HRESULT!",
						hr
						);
		}

		SAFE_RELEASE(pUnifiedPropertyStoreFactory);
		SAFE_RELEASE(pUnifiedPropertyStore);
	}

	return hr;
}

HRESULT
CMyDevice::QueryInterface(
	_In_ REFIID InterfaceId,
	_Outptr_ PVOID *Object
	)
/*++
 
  Routine Description:

	This method is called to get a pointer to one of the object's callback
	interfaces.  

	Since the skeleton driver doesn't support any of the device events, this
	method simply calls the base class's BaseQueryInterface.

	If the skeleton is extended to include device event interfaces then this 
	method must be changed to check the IID and return pointers to them as
	appropriate.

  Arguments:

	InterfaceId - the interface being requested

	Object - a location to store the interface pointer if successful

  Return Value:

	S_OK or E_NOINTERFACE

--*/
{
	HRESULT hr;

	if (IsEqualIID(InterfaceId, __uuidof(IPnpCallbackHardware)))
	{
		*Object = QueryIPnpCallbackHardware();
		hr = S_OK;	
	}
	else if (IsEqualIID(InterfaceId, __uuidof(IPnpCallback)))
	{
		*Object = QueryIPnpCallback();
		hr = S_OK;	
	}	
	else if (IsEqualIID(InterfaceId, __uuidof(IRequestCallbackRequestCompletion)))
	{
		*Object = QueryIRequestCallbackRequestCompletion();
		hr = S_OK;	
	}
	else
	{
		hr = CUnknown::QueryInterface(InterfaceId, Object);
	}

	return hr;
}

HRESULT
CMyDevice::OnPrepareHardware(
	_In_ IWDFDevice * /* FxDevice */
	)
/*++

Routine Description:

	This routine is invoked to ready the driver
	to talk to hardware. It opens the handle to the 
	device and talks to it using the HID interface.

Arguments:

	FxDevice  : Pointer to the WDF device interface

Return Value:

	HRESULT 

--*/
{
	PWSTR instanceId = NULL;
	DWORD instanceIdCch = 0;
	HRESULT hr = S_OK;

	m_SwitchState.SwitchesAsUChar = 0;

	if (SUCCEEDED(hr))
	{	
		//
		// Find out the instance id buffer size.
		//
		hr = m_FxDevice->RetrieveDeviceInstanceId(NULL, &instanceIdCch);
	}

	if (SUCCEEDED(hr))
	{
		//
		// Allocate the buffer.
		//
		instanceId = new WCHAR[instanceIdCch];
		if (instanceId == NULL)
		{
			hr = E_OUTOFMEMORY;

			TraceEvents(TRACE_LEVEL_ERROR, 
						TEST_TRACE_DEVICE, 
						"%!FUNC! Out of memory"
						);
		}
	}

	if (SUCCEEDED(hr))
	{	
		//
		// Get the instance id.
		//
		hr = m_FxDevice->RetrieveDeviceInstanceId(instanceId, &instanceIdCch);
	}

	if (SUCCEEDED(hr))
	{
		TraceEvents(TRACE_LEVEL_INFORMATION, 
					TEST_TRACE_DEVICE, 
					"%!FUNC! Device instance id %S",
					instanceId
					);
	}
	else
	{
		TraceEvents(TRACE_LEVEL_ERROR, 
					TEST_TRACE_DEVICE, 
					"%!FUNC! Cannot get device instance id %!HRESULT!",
					hr
					);
	}

	if(SUCCEEDED(hr))
	{
		//
		// Create a file object.
		//
		hr = m_FxDevice->CreateWdfFile( NULL, &m_WdfFile );

		if (SUCCEEDED(hr) && nullptr != m_WdfFile)
		{
			//
			// Get our parent driver object.
			//
			m_FxDevice->GetDriver( &m_WdfDriver );

			//
			// Get the default I/O target.  This allows us to send IOCTLs
			// to the HID collection below us in the stack.
			//
			if (nullptr != m_WdfDriver)
			{
				m_FxDevice->GetDefaultIoTarget( &m_WdfIoTarget );
			}
			else
			{
				hr = E_UNEXPECTED;
				Trace(TRACE_LEVEL_ERROR, "Failed during GetDriver(), hr = %!HRESULT!", hr); 
			}

			if (nullptr == m_WdfIoTarget)
			{
				hr = E_UNEXPECTED;
				Trace(TRACE_LEVEL_ERROR, "Failed during GetDefaultIoTarget(), hr = %!HRESULT!", hr); 
			}
		}
		else
		{
			Trace(TRACE_LEVEL_ERROR, "m_WdfFile is NULL, hr = %!HRESULT!", hr); 
		}
	}
	else
	{
		Trace(TRACE_LEVEL_ERROR, "Failed during CreateWdfFile(), hr = %!HRESULT!", hr); 
	}
	
	if (SUCCEEDED(hr))
	{
		//
		// Get the HID capabilities and attributes.
		//
		hr = GetHidCapabilities();
	}

	if (SUCCEEDED(hr))
	{
		//
		// Queue up a pending read for input reports.
		//
		WUDF_TEST_DRIVER_ASSERT(m_Caps.InputReportByteLength == sizeof (m_SwitchStateReport));
		IWDFIoRequest *pWdfRequest;
		//
		// Create child request for the default I/O target
		//
		hr = m_FxDevice->CreateRequest(NULL, m_WdfIoTarget, &pWdfRequest);
		if (SUCCEEDED(hr))
		{
			hr = SendReadRequest(pWdfRequest);
			pWdfRequest->Release();
		}
		else
		{
			Trace(TRACE_LEVEL_ERROR, "Failed to create a request, hr = %!HRESULT!", hr); 
		}
	}

	if (SUCCEEDED(hr))
	{
		//
		// Set the seven segement display and bar graph to indicate that we're done with 
		// prepare hardware.
		//
		hr = IndicateDeviceReady();
	}

	if (FAILED(hr))
	{
		TraceEvents(TRACE_LEVEL_ERROR, 
					TEST_TRACE_DEVICE, 
					"%!FUNC! failed %!HRESULT!",
					hr
					);
	}

	delete[] instanceId;

	return hr;
}

HRESULT
CMyDevice::OnReleaseHardware(
	_In_ IWDFDevice * /* FxDevice */
	)
/*++

Routine Description:

	This routine is invoked when the device is being removed or stopped
	It releases all resources allocated for this device.

Arguments:

	FxDevice - Pointer to the Device object.

Return Value:

	HRESULT - Always succeeds.

--*/
{
	//
	// Release all objects we created.
	//
	if (m_WdfDriver != NULL)
	{
		m_WdfDriver->Release();
		m_WdfDriver = nullptr;
	}
	if (m_WdfFile != NULL)
	{
		m_WdfFile->Close();
		m_WdfFile->Release();
		m_WdfFile = nullptr;
	}
	if (m_WdfIoTarget != nullptr)
	{
		m_WdfIoTarget->Release();
		m_WdfIoTarget = nullptr;
	}
	if (NULL != m_Ppd)
	{
		delete[] (BYTE*)m_Ppd;
		m_Ppd = NULL;
	}
	return S_OK;
}

HRESULT
CMyDevice::GetHidCapabilities(
	void
	)
/*++

Routine Description:

	This routine gets the capabilities and attributes from the HID collection

Arguments:

	None.

Return Value:

	HRESULT

--*/
{
	HID_COLLECTION_INFORMATION info;
	HRESULT hr = S_OK;

	ZeroMemory((BYTE *)&info, sizeof(info));
	hr = CreateAndSendIoctlRequest(NULL, 0, (BYTE*)(&info), sizeof(info), IOCTL_HID_GET_COLLECTION_INFORMATION);

	if (SUCCEEDED(hr))
	{
		m_Ppd = (PHIDP_PREPARSED_DATA)(new BYTE[info.DescriptorSize]);

		if (m_Ppd == NULL)
		{
			hr = E_OUTOFMEMORY;

			TraceEvents(TRACE_LEVEL_ERROR, 
						TEST_TRACE_DEVICE, 
						"%!FUNC! Out of memory"
						);
		}
	}

	if (SUCCEEDED(hr))
	{
		ZeroMemory((BYTE*)m_Ppd, info.DescriptorSize);
		hr = CreateAndSendIoctlRequest(NULL, 0, (BYTE*)m_Ppd, info.DescriptorSize, IOCTL_HID_GET_COLLECTION_DESCRIPTOR);
	}


	if (SUCCEEDED(hr))
	{
		ZeroMemory(&m_Caps, sizeof (m_Caps));

		hr = HidP_GetCaps (m_Ppd, &m_Caps);

		if (FAILED(hr))
		{
			TraceEvents(TRACE_LEVEL_ERROR, 
						TEST_TRACE_DEVICE, 
						"%!FUNC! HidP_GetCaps failed %!HRESULT!",
						hr
						);
		}
	}

	if (SUCCEEDED(hr))
	{
		//
		// Validate the capabilities.
		//
		if (m_Caps.UsagePage != 0xff00 ||
			m_Caps.FeatureReportByteLength != 2)
		{
			hr = E_UNEXPECTED;

			TraceEvents(TRACE_LEVEL_ERROR, 
						TEST_TRACE_DEVICE, 
						"%!FUNC! Capabilities don't match expected usage page 0x%x and feature report length %d %!HRESULT!",
						m_Caps.UsagePage,
						m_Caps.FeatureReportByteLength,
						hr
						);

		}
	}

	return hr;
}

HRESULT
CMyDevice::IndicateDeviceReady(
	VOID
	)
/*++
 
  Routine Description:

	This method lights the period on the device's seven-segment display to 
	indicate that the driver's PrepareHardware method has completed.

  Arguments:

	None

  Return Value:

	Status

--*/
{
	SEVEN_SEGMENT display = {0};
	BAR_GRAPH_STATE barGraph = {0};
	HRESULT hr = S_OK;

	if (SUCCEEDED(hr)) 
	{
		display.Segments |= 0xf7;

		hr = SetSevenSegmentDisplay(&display);
	}

	if (SUCCEEDED(hr))
	{
		barGraph.BarsAsUChar = BARGRAPH_LED_ALL_OFF;

		hr = SetBarGraphDisplay(&barGraph);
	}

	return hr;
}

HRESULT
CMyDevice::GetBarGraphDisplay(
	_In_ PBAR_GRAPH_STATE BarGraphState
	)
/*++
 
  Routine Description:

	This method synchronously retrieves the bar graph display information 
	from the OSR USB-FX2 device.  It uses the buffers in the FxRequest
	to hold the data it retrieves.

  Arguments:

	FxRequest - the request for the bar-graph info.

  Return Value:

	Status

--*/
{
	HRESULT hr = S_OK;
	HIDFX2_FEATURE_REPORT	featureReport;

	//
	// Zero the contents of the buffer.
	//
	BarGraphState->BarsAsUChar = 0;

	//
	// Get the feature
	//
	featureReport.ReportId = BARGRAPH_REPORT_ID;
	featureReport.FeatureData = 0;

	hr = CreateAndSendIoctlRequest(NULL, 0, (BYTE*)&featureReport, sizeof (featureReport), IOCTL_HID_GET_FEATURE);

	if (SUCCEEDED(hr))
	{
		BarGraphState->BarsAsUChar = featureReport.FeatureData;
		TraceEvents(TRACE_LEVEL_INFORMATION, 
					TEST_TRACE_DEVICE, 
					"%!FUNC! Bargraph display value is 0x%x",
					BarGraphState->BarsAsUChar
					);
	}
	else
	{
		TraceEvents(TRACE_LEVEL_ERROR, 
					TEST_TRACE_DEVICE, 
					"%!FUNC! IOCTL_HID_GET_FEATURE failed %!HRESULT!",
					hr
					);
	}

	return hr;
}

HRESULT
CMyDevice::SetBarGraphDisplay(
	_In_ PBAR_GRAPH_STATE BarGraphState
	)
/*++
 
  Routine Description:

	This method synchronously sets the bar graph display on the OSR USB-FX2 
	device using the buffers in the FxRequest as input.

  Arguments:

	FxRequest - the request to set the bar-graph info.

  Return Value:

	Status

--*/
{
	HRESULT hr = S_OK;
	HIDFX2_FEATURE_REPORT	featureReport;
	UCHAR	usage = 0;

	//
	// The usage for set feature for the bargraph display maps directly
	// to the command to be sent down to the device.
	//
	usage = BarGraphState->BarsAsUChar;

	//
	// Set the feature
	//
	featureReport.ReportId = BARGRAPH_REPORT_ID;
	featureReport.FeatureData = usage;

	hr = CreateAndSendIoctlRequest((BYTE*)&featureReport, sizeof (featureReport), NULL, 0, IOCTL_HID_SET_FEATURE);

	if (FAILED(hr))
	{
		TraceEvents(TRACE_LEVEL_ERROR, 
					TEST_TRACE_DEVICE, 
					"%!FUNC! IOCTL_HID_SET_FEATURE failed %!HRESULT!",
					hr
					);
	}

	return hr;
}

HRESULT
CMyDevice::GetSevenSegmentDisplay(
	_In_ PSEVEN_SEGMENT SevenSegment
	)
/*++
 
  Routine Description:

	This method synchronously retrieves the bar graph display information 
	from the OSR USB-FX2 device.  It uses the buffers in the FxRequest
	to hold the data it retrieves.

  Arguments:

	FxRequest - the request for the bar-graph info.

  Return Value:

	Status

--*/
{
	HRESULT hr = S_OK;
	HIDFX2_FEATURE_REPORT	featureReport;

	//
	// Zero the output buffer.
	//
	SevenSegment->Segments = 0;

	//
	// Get the feature
	//
	featureReport.ReportId = SEVEN_SEGMENT_REPORT_ID;
	featureReport.FeatureData = 0;

	hr = CreateAndSendIoctlRequest(NULL, 0, (BYTE*)&featureReport, sizeof (featureReport), IOCTL_HID_GET_FEATURE);

	if (SUCCEEDED(hr))
	{
		SevenSegment->Segments = featureReport.FeatureData;
		TraceEvents(TRACE_LEVEL_INFORMATION, 
					TEST_TRACE_DEVICE, 
					"%!FUNC! Seven segment display value is 0x%x",
					SevenSegment->Segments
					);
	}
	else
	{
		TraceEvents(TRACE_LEVEL_ERROR, 
					TEST_TRACE_DEVICE, 
					"%!FUNC! IOCTL_HID_GET_FEATURE failed %!HRESULT!",
					hr
					);
	}

	return hr;
}

HRESULT
CMyDevice::SetSevenSegmentDisplay(
	_In_ PSEVEN_SEGMENT SevenSegment
	)
/*++
 
  Routine Description:

	This method synchronously sets the bar graph display on the OSR USB-FX2 
	device using the buffers in the FxRequest as input.

  Arguments:

	FxRequest - the request to set the bar-graph info.

  Return Value:

	Status

--*/
{
	HRESULT					hr = S_OK;
	UCHAR					usage = 0;
	HIDFX2_FEATURE_REPORT	featureReport;

	//
	// The usage for set feature for the seven segment display maps directly
	// to the command to be sent down to the device.
	//
	usage = SevenSegment->Segments;

	//
	// Set the feature
	//
	featureReport.ReportId = SEVEN_SEGMENT_REPORT_ID;
	featureReport.FeatureData = usage;

	hr = CreateAndSendIoctlRequest((BYTE*)&featureReport, sizeof (featureReport), NULL, 0, IOCTL_HID_SET_FEATURE);

	if (FAILED(hr))
	{
		TraceEvents(TRACE_LEVEL_ERROR, 
					TEST_TRACE_DEVICE, 
					"%!FUNC! IOCTL_HID_SET_FEATURE failed %!HRESULT!",
					hr
					);
	}

	return hr;
}

HRESULT
CMyDevice::ReadSwitchState(
	_In_ PSWITCH_STATE SwitchState
	)
/*++
 
  Routine Description:

	This method synchronously retrieves the switch state for the device.

  Arguments:

	FxRequest - the request for the bar-graph info.

  Return Value:

	Status

--*/
{
	HRESULT hr = S_OK;

	//
	// The switch state is updated when we get input reports from the
	// HID collection.
	//
	SwitchState->SwitchesAsUChar = m_SwitchState.SwitchesAsUChar;

	TraceEvents(TRACE_LEVEL_INFORMATION, 
				TEST_TRACE_DEVICE, 
				"%!FUNC! m_SwitchState 0x%x",
				m_SwitchState.SwitchesAsUChar
				);


	return hr;
}

	
VOID
CMyDevice::ServiceSwitchChangeQueue(
	_In_ SWITCH_STATE NewState,
	_In_ HRESULT CompletionStatus,
	_In_opt_ IWDFFile *SpecificFile
	)
/*++
 
  Routine Description:

	This method processes switch-state change notification requests as 
	part of reading the OSR device's interrupt pipe.  As each read completes
	this pulls all pending I/O off the switch change queue and completes
	each request with the current switch state.

  Arguments:

	NewState - the state of the switches

	CompletionStatus - all pending operations are completed with this status.

	SpecificFile - if provided only requests for this file object will get
				   completed.

  Return Value:

	None

--*/
{
	IWDFIoRequest *fxRequest;
	HRESULT enumHr = S_OK;

	do 
	{
		HRESULT hr;

		//
		// Get the next request.
		//

		if (NULL != SpecificFile)
		{
			enumHr = m_SwitchChangeQueue->RetrieveNextRequestByFileObject(
											SpecificFile,
											&fxRequest
											);
		}
		else
		{
			enumHr = m_SwitchChangeQueue->RetrieveNextRequest(&fxRequest);
		}

		//
		// If we got one then complete it.
		//

		if (SUCCEEDED(enumHr)) 
		{
			if (SUCCEEDED(CompletionStatus)) 
			{
				IWDFMemory *fxMemory;

				//
				// First copy the result to the request buffer.
				//

				fxRequest->GetOutputMemory(&fxMemory );

				hr = fxMemory->CopyFromBuffer(0, 
											  &NewState, 
											  sizeof(SWITCH_STATE));
				fxMemory->Release();
			}
			else 
			{
				hr = CompletionStatus;
			}

			//
			// Complete the request with the status of the copy (or the completion
			// status if that was an error).
			//

			if (SUCCEEDED(hr)) 
			{
				fxRequest->CompleteWithInformation(hr, sizeof(SWITCH_STATE));
			}
			else
			{
				fxRequest->Complete(hr);
			}

			fxRequest->Release();			
		}
	}
	while (SUCCEEDED(enumHr));
}


HRESULT 
CMyDevice::CreateAndSendIoctlRequest(
	_In_reads_bytes_opt_(InputBufferSize) BYTE* InputBuffer,
	_In_ ULONG InputBufferSize,
	_In_reads_bytes_opt_(OutputBufferSize) BYTE* OutputBuffer,
	_In_ ULONG OutputBufferSize,
	_In_ ULONG IoctlCode
	)
/*++
 
  Routine Description:

	This method sends an IOCTL request down to the HID collection.

  Arguments:

	InputBuffer - Pointer to the data to be sent.

	InputBufferSize - Size of data to be sent.

	OutputBuffer - Pointer to a receive buffer.

	OutputBufferSize - Size of recieve data buffer.

	IoctlCode - Control code to be sent.

  Return Value:

	None

--*/
{
	HRESULT hr = S_OK;

	IWDFMemory		*pOutputMemory = nullptr;
	IWDFMemory		*pInputMemory = nullptr;
	IWDFIoRequest	*pWdfRequest = nullptr;

	if ((nullptr != m_FxDevice) &&
		(nullptr != m_WdfIoTarget) &&
		(nullptr != m_WdfFile) )
	{
		//
		// Create a new request
		//
		hr = m_FxDevice->CreateRequest(NULL, m_FxDevice, &pWdfRequest);

		if (SUCCEEDED(hr))
		{
			// 
			// Create input memory
			//
			if (SUCCEEDED(hr) && (nullptr != InputBuffer))
			{
				hr = m_WdfDriver->CreatePreallocatedWdfMemory(InputBuffer, 
															  InputBufferSize, 
															  NULL,				// no object event callback
															  pWdfRequest,		// request object as parent
															  &pInputMemory); 
			}

			// 
			// Create output memory
			//
			if (SUCCEEDED(hr) && (nullptr != OutputBuffer))
			{
				hr = m_WdfDriver->CreatePreallocatedWdfMemory(OutputBuffer, 
															  OutputBufferSize, 
															  NULL,			// no object event callback
															  pWdfRequest,	// request object as parent
															  &pOutputMemory); 
			}

			if (SUCCEEDED(hr))
			{
				hr = m_WdfIoTarget->FormatRequestForIoctl(pWdfRequest,
														  IoctlCode,
														  m_WdfFile,
														  pInputMemory, 
														  NULL, 
														  pOutputMemory,
														  NULL);
			}

			//
			// Send down the request
			//	
			if (SUCCEEDED(hr))
			{
				hr = pWdfRequest->Send(m_WdfIoTarget, WDF_REQUEST_SEND_OPTION_SYNCHRONOUS, 0);
			}
		
			//
			// This is a synchronous call, so we delete the request object
			// whether it succeeded or failed
			//
			pWdfRequest->DeleteWdfObject();
			pWdfRequest->Release();
			pWdfRequest = nullptr;
		}
	}

	if (nullptr != pInputMemory)
	{
		pInputMemory->Release();
		pInputMemory = nullptr;
	}

	if (nullptr != pOutputMemory)
	{
		pOutputMemory->Release();
		pOutputMemory = nullptr;
	}

	if (FAILED(hr))
	{
		Trace(TRACE_LEVEL_ERROR, "Failed during request, hr = %!HRESULT!", hr); 
	}

	return hr;		
}

HRESULT 
CMyDevice::SendReadRequest(
	_In_ IWDFIoRequest *pWdfRequest
	)
/*++
 
  Routine Description:

	This method sends a synchronous read request down to the HID collection.

  Arguments:

	pWdfRequest - pointer to request object.

  Return Value:

	None

--*/
{
	HRESULT hr = S_OK;

	IWDFMemory		*pOutputMemory = nullptr;
	IRequestCallbackRequestCompletion *pICallback;

	if ((nullptr != m_FxDevice) &&
		(nullptr != m_WdfIoTarget) &&
		(nullptr != m_WdfFile))
	{

		if (SUCCEEDED(hr) && nullptr != pWdfRequest)
		{
			hr = this->QueryInterface( __uuidof(IRequestCallbackRequestCompletion) , (PVOID*)&pICallback );
 
			//
			// Set completion callback and release our reference
			//
			if (SUCCEEDED(hr) && (nullptr != pICallback))
			{
				pWdfRequest->SetCompletionCallback(pICallback, NULL);
				pICallback->Release();
			} 
			else
			{
				Trace(TRACE_LEVEL_ERROR, "Failed during QueryInterface(), hr = %!HRESULT!", hr); 
			}

			// 
			// Create output memory
			//
			if (SUCCEEDED(hr))
			{
				hr = m_WdfDriver->CreatePreallocatedWdfMemory((BYTE *)&m_SwitchStateReport, 
																	sizeof (m_SwitchStateReport), 
																	NULL,			// no object event callback
																	pWdfRequest,	// request object as parent
																	&pOutputMemory); 
			}

			if (SUCCEEDED(hr))
			{
				hr = m_WdfIoTarget->FormatRequestForRead( pWdfRequest,
																m_WdfFile,
																pOutputMemory, 
																NULL, 
																NULL);
			}

			//
			// Send down the request
			//	
			if (SUCCEEDED(hr))
			{
				hr = pWdfRequest->Send(m_WdfIoTarget, 0, 0);
			}
		}
		
	}
	else
	{
		hr = E_UNEXPECTED;
		Trace(TRACE_LEVEL_ERROR, "Invalid member variables, hr = %!HRESULT!", hr); 
	}

	if (nullptr != pOutputMemory)
	{
		pOutputMemory->Release();
		pOutputMemory = nullptr;
	}

	if (FAILED(hr))
	{
		Trace(TRACE_LEVEL_ERROR, "Failed during request, hr = %!HRESULT!", hr); 
	}

	return hr;		
}


HRESULT
CMyDevice::OnD0Entry(
	_In_ IWDFDevice*  pWdfDevice,
	_In_ WDF_POWER_DEVICE_STATE  previousState
	)
{
	UNREFERENCED_PARAMETER(pWdfDevice);
	UNREFERENCED_PARAMETER(previousState);

	return S_OK;
}

HRESULT
CMyDevice::OnD0Exit(
	_In_ IWDFDevice*  pWdfDevice,
	_In_ WDF_POWER_DEVICE_STATE  previousState
	)
{
	UNREFERENCED_PARAMETER(pWdfDevice);
	UNREFERENCED_PARAMETER(previousState);

	return S_OK;
}

void
CMyDevice::OnSurpriseRemoval(
	_In_ IWDFDevice*  pWdfDevice
	)
{
	UNREFERENCED_PARAMETER(pWdfDevice); 
}


HRESULT
CMyDevice::OnQueryRemove(
	_In_ IWDFDevice*  pWdfDevice
	)
{
	UNREFERENCED_PARAMETER(pWdfDevice);
	return S_OK;
}

HRESULT
CMyDevice::OnQueryStop(
	_In_ IWDFDevice*  pWdfDevice
	)
{
	UNREFERENCED_PARAMETER(pWdfDevice);
	return S_OK;
}

/////////////////////////////////////////////////////////////////////////
//
// CReadWriteRequest::OnCompletion
//
//  This method is called by Framework when read request is completed
//  The result is saved in the m_SwitchState
//
/////////////////////////////////////////////////////////////////////////
void CMyDevice::OnCompletion(
		_In_ IWDFIoRequest*					pRequest,
		_In_ IWDFIoTarget*					pIoTarget,
		_In_ IWDFRequestCompletionParams*	CompletionParams,
		_In_ PVOID							Context
		)
{
	UNREFERENCED_PARAMETER(pRequest);
	UNREFERENCED_PARAMETER(pIoTarget);
	UNREFERENCED_PARAMETER(Context);

	HRESULT hr = E_UNEXPECTED;

	if (NULL != CompletionParams)
	{
		hr = CompletionParams->GetCompletionStatus();

		if (SUCCEEDED(hr))
		{
			WDF_REQUEST_TYPE reqType = CompletionParams->GetCompletedRequestType();

			if ((WdfRequestRead == reqType) && 
				(CompletionParams->GetInformation() == sizeof (m_SwitchStateReport)) &&
				(DEP_SWITCHES_REPORT_ID == m_SwitchStateReport.ReportId))
			{
				//
				// Set the new state and complete any pending
				// switch state requests.
				//
				m_SwitchState.SwitchesAsUChar = m_SwitchStateReport.SwitchStateAsByte;
				ServiceSwitchChangeQueue(m_SwitchState, hr, nullptr);
			}

		}
	}
	else
	{
		Trace(TRACE_LEVEL_ERROR, "CompletionParameters is null"); 
	}

	if (SUCCEEDED(hr))
	{
		//
		// Re-send the read request
		//
		IWDFIoRequest2 *pWdfRequest2;
		pRequest->QueryInterface(IID_PPV_ARGS(&pWdfRequest2));
		if (nullptr != pWdfRequest2)
		{
			pWdfRequest2->Reuse(S_OK);
			hr = SendReadRequest(pWdfRequest2);
			pWdfRequest2->Release();

			if (FAILED(hr))
			{
				Trace(TRACE_LEVEL_ERROR, "Failed to send a new request hr = %!HRESULT!", hr); 
			}

		}
		else
		{
			hr = E_UNEXPECTED;
			Trace(TRACE_LEVEL_ERROR, "Failed to reuse the request hr = %!HRESULT!", hr); 
		}


	}

	return;
}




