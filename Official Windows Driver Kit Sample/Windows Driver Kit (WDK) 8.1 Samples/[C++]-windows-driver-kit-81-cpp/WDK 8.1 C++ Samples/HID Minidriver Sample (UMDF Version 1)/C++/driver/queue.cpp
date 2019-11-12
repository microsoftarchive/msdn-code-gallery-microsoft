/*++
 
Copyright (C) Microsoft Corporation, All Rights Reserved.

Module Name:

    Queue.cpp

Abstract:

    This module contains the implementation of the driver's
    queue callback object.

Environment:

   Windows User-Mode Driver Framework (WUDF)

--*/

#include "internal.h"
#include "winioctl.h"
#include "queue.h"

#if defined(EVENT_TRACING)
#include "queue.tmh"
#endif


//
// This is the default report descriptor for the virtual Hid device returned
// by the mini driver in response to IOCTL_HID_GET_REPORT_DESCRIPTOR.
//
HID_REPORT_DESCRIPTOR           G_DefaultReportDescriptor[] = {
    0x06,0x00, 0xFF,                // USAGE_PAGE (Vender Defined Usage Page)
    0x09,0x01,                      // USAGE (Vendor Usage 0x01)
    0xA1,0x01,                      // COLLECTION (Application)
    0x85,CONTROL_FEATURE_REPORT_ID,    // REPORT_ID (1)
    0x09,0x01,                         // USAGE (Vendor Usage 0x01)
    0x15,0x00,                         // LOGICAL_MINIMUM(0)
    0x26,0xff, 0x00,                   // LOGICAL_MAXIMUM(255)
    0x75,0x08,                         // REPORT_SIZE (0x08)
    //0x95,FEATURE_REPORT_SIZE_CB,       // REPORT_COUNT 
    0x96,(FEATURE_REPORT_SIZE_CB & 0xff), (FEATURE_REPORT_SIZE_CB >> 8), // REPORT_COUNT 
    0xB1,0x00,                         // FEATURE (Data,Ary,Abs)
    0x09,0x01,                         // USAGE (Vendor Usage 0x01)
    0x75,0x08,                         // REPORT_SIZE (0x08)
    //0x95,INPUT_REPORT_SIZE_CB,       // REPORT_COUNT 
    0x96,(INPUT_REPORT_SIZE_CB & 0xff), (INPUT_REPORT_SIZE_CB >> 8), // REPORT_COUNT 
    0x81,0x00,                         // INPUT (Data,Ary,Abs)
    0x09,0x01,                         // USAGE (Vendor Usage 0x01)
    0x75,0x08,                         // REPORT_SIZE (0x08)
    //0x95,OUTPUT_REPORT_SIZE_CB,      // REPORT_COUNT 
    0x96,(OUTPUT_REPORT_SIZE_CB & 0xff), (OUTPUT_REPORT_SIZE_CB >> 8), // REPORT_COUNT 
    0x91,0x00,                         // OUTPUT (Data,Ary,Abs)
    0xC0,                           // END_COLLECTION
};

//
// This is the default HID descriptor returned by the mini driver
// in response to IOCTL_HID_GET_DEVICE_DESCRIPTOR. The size
// of report descriptor is currently the size of G_DefaultReportDescriptor.
//

HID_DESCRIPTOR              G_DefaultHidDescriptor = {
    0x09,   // length of HID descriptor
    0x21,   // descriptor type == HID  0x21
    0x0100, // hid spec release
    0x00,   // country code == Not Specified
    0x01,   // number of HID class descriptors
    { 0x22,   // report descriptor type 0x22
    sizeof(G_DefaultReportDescriptor) }  // total length of report descriptor
};

HRESULT
CMyQueue::CreateInstance(
    _In_ PCMyDevice Device,
    _Out_ CMyQueue **Queue
    )
/*++
 
  Routine Description:

    This method creates and initializs an instance of the driver's 
    queue callback object.

  Arguments:

    FxDeviceInit - the settings for the device.

    Queue - a location to store the referenced pointer to the queue object.

  Return Value:

    Status

--*/
{
    CMyQueue *queue;

    HRESULT hr = S_OK;

    //
    // Allocate a new instance of the device class.
    //

    queue = new CMyQueue(Device);

    if (NULL == queue)
    {
        hr = E_OUTOFMEMORY;
    }

    //
    // Initialize the instance.
    //

    if (SUCCEEDED(hr)) 
    {
        hr = queue->Initialize(Device->GetFxDevice());
    }

    if (SUCCEEDED(hr)) 
    {
        queue->AddRef();
        *Queue = queue;
    }

    if (NULL != queue)
    {
        queue->Release();
    }

    return hr;
}

HRESULT
CMyQueue::QueryInterface(
    _In_ REFIID InterfaceId,
    _Out_ PVOID *Object
    )
/*++
 
  Routine Description:

    This method is called to get a pointer to one of the object's callback
    interfaces.  

  Arguments:

    InterfaceId - the interface being requested

    Object - a location to store the interface pointer if successful

  Return Value:

    S_OK or E_NOINTERFACE

--*/
{
    HRESULT hr;

    if(IsEqualIID(InterfaceId, __uuidof(IQueueCallbackDeviceIoControl))) 
    {
        hr = S_OK;
        AddRef();
        *Object = static_cast<IQueueCallbackDeviceIoControl *>(this);
    } 
    else 
    {
        hr = CUnknown::QueryInterface(InterfaceId, Object);
    }

    return hr;
}

HRESULT
CMyQueue::Initialize(
    _In_ IWDFDevice *FxDevice
    )
/*++
 
  Routine Description:

    This method initializes the queue callback object.  Any operations which
    need to be performed before the caller can use the callback object, but 
    which couldn't be done in the constructor becuase they could fail would
    be placed here.

  Arguments:

    FxDevice - the device which this Queue is for.

  Return Value:

    status.

--*/
{
    IWDFIoQueue     *fxQueue;
    HRESULT hr;

    //
    // Create the framework queue
    //
    IUnknown *unknown = QueryIUnknown();
    hr = FxDevice->CreateIoQueue(
                        unknown,
                        TRUE,                        // bDefaultQueue
                        WdfIoQueueDispatchParallel, 
                        FALSE,                       // bPowerManaged
                        TRUE,                        // bAllowZeroLengthRequests
                        &fxQueue 
                        );
    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR, 
            "%!FUNC!: Could not create default I/O queue, %!hresult!",
            hr
            );
    }

    if (SUCCEEDED(hr)) 
    {
        m_FxQueue = fxQueue;

        //
        // m_FxQueue is kept as a Weak reference to framework Queue object to avoid 
        // circular reference. This object's lifetime is contained within 
        // framework Queue object's lifetime
        //
        
        fxQueue->Release();
    }

    unknown->Release();

    return hr;
}

VOID
STDMETHODCALLTYPE
CMyQueue::OnDeviceIoControl(
    _In_ IWDFIoQueue *FxQueue,
    _In_ IWDFIoRequest *FxRequest,
    _In_ ULONG ControlCode,
    _In_ SIZE_T InputBufferSizeInBytes,
    _In_ SIZE_T OutputBufferSizeInBytes
    )
/*++

Routine Description:

    DeviceIoControl dispatch routine

Aruments:
    
    FxQueue - Framework Queue instance
    FxRequest - Framework Request  instance
    ControlCode - IO Control Code
    InputBufferSizeInBytes - Lenth of input buffer
    OutputBufferSizeInBytes - Lenth of output buffer

    Always succeeds DeviceIoIoctl
Return Value:

    VOID

--*/
{
    UNREFERENCED_PARAMETER(FxQueue);
    UNREFERENCED_PARAMETER(InputBufferSizeInBytes);
    UNREFERENCED_PARAMETER(OutputBufferSizeInBytes);
    
    HRESULT hr = S_OK;
    BOOLEAN completeRequest = TRUE;
    IWDFIoRequest2 *fxRequest2;

    hr = FxRequest->QueryInterface(IID_PPV_ARGS(&fxRequest2));
    if (FAILED(hr)){
        FxRequest->Complete(hr);
        return;
    }
    fxRequest2->Release();

    switch (ControlCode)
    {
    case IOCTL_HID_GET_DEVICE_DESCRIPTOR:   // METHOD_NEITHER
        //
        // Retrieves the device's HID descriptor.
        //
        hr = GetHidDescriptor(fxRequest2);
        break;
    
    case IOCTL_HID_GET_DEVICE_ATTRIBUTES:    // METHOD_NEITHER
        //
        //Retrieves a device's attributes in a HID_DEVICE_ATTRIBUTES structure.
        //
        hr = GetDeviceAttributes(fxRequest2);
        break;

    case IOCTL_HID_GET_REPORT_DESCRIPTOR:    // METHOD_NEITHER
        //
        //Obtains the report descriptor for the HID device.
        //
        hr = GetReportDescriptor(fxRequest2);
        break;

    case IOCTL_HID_READ_REPORT:    // METHOD_NEITHER
        //
        // Returns a report from the device into a class driver-supplied 
        // buffer. 
        //
        hr = ReadReport(fxRequest2, &completeRequest);
        break;

    case IOCTL_UMDF_HID_GET_FEATURE:       // METHOD_NEITHER

        hr = GetFeature(fxRequest2);
        break;

    case IOCTL_UMDF_HID_GET_INPUT_REPORT:  // METHOD_NEITHER

        hr = GetInputReport(fxRequest2);
        break;

    case IOCTL_UMDF_HID_SET_FEATURE:       // METHOD_NEITHER

        hr = SetFeature(fxRequest2);
        break;

    case IOCTL_UMDF_HID_SET_OUTPUT_REPORT: // METHOD_NEITHER

        hr = SetOutputReport(fxRequest2);
        break;

    case IOCTL_HID_WRITE_REPORT:           // METHOD_NEITHER
        //
        // Transmits a class driver-supplied report to the device.
        //
        hr = WriteReport(fxRequest2);
        break;

    case IOCTL_HID_GET_STRING:                      // METHOD_NEITHER     
    case IOCTL_HID_GET_INDEXED_STRING:              // METHOD_OUT_DIRECT
        //
        // Transmits a class driver-supplied report to the device.
        //
        hr = GetString(fxRequest2);
        break;

    case IOCTL_HID_SEND_IDLE_NOTIFICATION_REQUEST:  // METHOD_NEITHER
        //
        // This has the USBSS Idle notification callback. If the lower driver
        // can handle it (e.g. USB stack can handle it) then pass it down
        // otherwise complete it here as not inplemented. For a virtual
        // device, idling is not needed.
        //
        // Not implemented. fall through...
        //
    case IOCTL_HID_ACTIVATE_DEVICE:                 // METHOD_NEITHER
    case IOCTL_HID_DEACTIVATE_DEVICE:               // METHOD_NEITHER
    case IOCTL_GET_PHYSICAL_DESCRIPTOR:             // METHOD_OUT_DIRECT 
        //
        // Not implemented. fall through...
        //
    default:
        hr = HRESULT_FROM_NT(STATUS_NOT_IMPLEMENTED);
        break;
    }

    //
    // Complete the request. Information value has already been set by request
    // handlers.
    //
    if (completeRequest) {
        fxRequest2->Complete(hr);
    }

    return;
}

HRESULT
CMyQueue::GetHidDescriptor(
    _In_ IWDFIoRequest2 *FxRequest
    )
/*++

Routine Description:

    This routines returns HID descriptor.

Arguments:

    Request - Handle to request object

Return Value:

    HRESULT

--*/
{
    HRESULT             hr = S_OK;
    size_t              bytesToCopy = 0;
    IWDFMemory *memory = NULL;

    //
    // Retrieve buffer
    //
    hr = FxRequest->RetrieveOutputMemory(&memory);
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, "RetrieveOutputMemory failed %!hresult!\n", hr);
        goto exit;        
    }

    //
    // Copy hard-coded HID Descriptor to request memory.
    //
    bytesToCopy = G_DefaultHidDescriptor.bLength;
    if (bytesToCopy == 0) 
    {
        hr = HRESULT_FROM_NT(STATUS_DATA_ERROR);
        Trace(TRACE_LEVEL_ERROR, "%!FUNC! desriptor length is zero %!hresult!\n", 
            hr);
        goto exit;        
    }

    hr = memory->CopyFromBuffer(0, 
                                (PVOID) &G_DefaultHidDescriptor,
                                bytesToCopy);
    
    if (FAILED(hr)) 
    {
        Trace(TRACE_LEVEL_ERROR, "%!FUNC! Buffer copy failed %!hresult!\n", hr);
        goto exit;;
    }

    //
    // Report how many bytes were copied
    //
    FxRequest->SetInformation(bytesToCopy);

exit:
    SAFE_RELEASE(memory);

    return hr;
}

HRESULT
CMyQueue::GetReportDescriptor(
    _In_ IWDFIoRequest2 *FxRequest
    )
/*++

Routine Description

    Handles IOCTL_UMDF_HID_GET_REPORT_DESCRIPTOR

Arguments:

Return Value:

    NT status value

--*/
{
    HRESULT     hr = S_OK;
    size_t      bytesToCopy = 0;
    IWDFMemory *memory = NULL;

    hr = FxRequest->RetrieveOutputMemory(&memory);
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, "RetrieveOutputMemory failed %!hresult!\n", hr);
        goto exit;        
    }

    bytesToCopy = memory->GetSize();
    if (bytesToCopy != sizeof(G_DefaultReportDescriptor)) {
        hr = E_INVALIDARG;
        Trace(TRACE_LEVEL_ERROR, "Incorrect buffer size received %!hresult!\n", hr);
        goto exit;        
    }

    hr = memory->CopyFromBuffer(0, 
                                (PVOID) &G_DefaultReportDescriptor,
                                sizeof(G_DefaultReportDescriptor));
    if (FAILED(hr)) 
    {
        Trace(TRACE_LEVEL_ERROR, "%!FUNC! Buffer copy failed %!hresult!\n", hr);
        goto exit;;
    }

    //
    // Report how many bytes were copied
    //
    FxRequest->SetInformation(sizeof(G_DefaultReportDescriptor));

exit:
    
    SAFE_RELEASE(memory);

    return hr;
}

HRESULT
CMyQueue::GetDeviceAttributes(
    _In_ IWDFIoRequest2 *FxRequest
    )
/*++

Routine Description:

    Finds the HID descriptor and copies it into the buffer provided by the 
    Request.

Arguments:

    Device - Handle to WDF Device Object

    Request - Handle to request object

Return Value:

    NT status code.

--*/
{
    HRESULT             hr = S_OK;
    IWDFMemory *memory = NULL;
    PVOID buffer;
    SIZE_T bufferSizeCb;

    hr = FxRequest->RetrieveOutputMemory(&memory);
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, "RetrieveOutputMemory failed %!hresult!\n", hr);
        return hr;        
    }

    buffer = memory->GetDataBuffer(&bufferSizeCb);
    memory->Release();

    if (bufferSizeCb != sizeof (HID_DEVICE_ATTRIBUTES)) 
    {
        hr = HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! Insufficient HID attributes buffer size %!hresult!\n", hr);
        return hr;
    }

    CopyMemory(buffer, &m_Device->m_Attributes, sizeof (HID_DEVICE_ATTRIBUTES));
    
    //
    // Report how many bytes were copied
    //
    FxRequest->SetInformation(sizeof (HID_DEVICE_ATTRIBUTES));

    return hr;
}

HRESULT
CMyQueue::ReadReport(
    _In_ IWDFIoRequest2 *FxRequest,
    _Out_ BOOLEAN* CompleteRequest
    )
{
    HRESULT hr;
    PTP_TIMER timer = NULL;

    //
    // start the timer if not already started
    //
    timer = m_Device->m_ManualQueue->GetTimer();
    if (IsThreadpoolTimerSet(timer) == FALSE) 
    {     
        FILETIME dueTime;
        Trace(TRACE_LEVEL_INFORMATION, "*** Setting the timer ***\n");

        *reinterpret_cast<PLONGLONG>(&dueTime) = 
            -static_cast<LONGLONG>(MILLI_SECOND_TO_NANO100(1000));
        
        SetThreadpoolTimer(timer,
                           &dueTime,
                           5000,  // ms periodic
                           0      // optional delay in ms
                           );
    }

    //
    // forward the request to manual queue
    //
    hr = FxRequest->ForwardToIoQueue(m_Device->m_ManualQueue->GetQueue());
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! ForwardToIoQueue returned failure %!hresult!\n", hr);
        *CompleteRequest = TRUE;
    }
    else {
        *CompleteRequest = FALSE;
    }

    return hr;
}

HRESULT
CMyQueue::GetFeature(
    _In_ IWDFIoRequest2 *FxRequest
    )
/*++

Routine Description:

    Handles IOCTL_UMDF_HID_GET_FEATURE for all the collection.


Arguments:

    Request - Pointer to Request Packet.

Return Value:

    NT status code.

--*/
{
    HRESULT hr;
    IWDFMemory *memory = NULL;
    SIZE_T inBufferCb, outBufferCb;
    ULONG reportSize;
    PVOID inBuffer;
    UCHAR reportId;
    PUCHAR reportBuffer = NULL;
    PMY_DEVICE_ATTRIBUTES myAttributes;

    // *************************************************************************
    // IOCTL_UMDF_HID_GET_FEATURE
    // Input report ID:
    //     UMDF driver retrieves report ID associated with this collection 
    //     by retrieving the buffer containing report ID by calling 
    //     IWDFRequest::GetInputMemory() or RetrieveInputMemory. 
    // Output report buffer and length: 
    //     UMDF driver calls IWDFRequest::GetOutputMemory() to get memory 
    //     buffer and its length. The driver fills the buffer with feature data. 
    //
    // *************************************************************************

    Trace(TRACE_LEVEL_INFORMATION, "GetFeature\n");

    //
    // Get input report ID
    //
    hr = FxRequest->RetrieveInputMemory(&memory);
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, "RetrieveINputMemory failed %!hresult!\n", hr);
        return hr;        
    }

    inBuffer = memory->GetDataBuffer(&inBufferCb);
    SAFE_RELEASE(memory);
    
    if (inBufferCb < sizeof(reportId)) 
    {
        hr = HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! Insufficient buffer size %!hresult!\n", hr);
        return hr;
    }

    reportId = *(PUCHAR)inBuffer;
    if (reportId != CONTROL_COLLECTION_REPORT_ID)
    {
        //
        // If collection ID is not for control collection then handle
        // this request just as you would for a regular collection.
        //
        hr = HRESULT_FROM_NT(STATUS_INVALID_PARAMETER);
        Trace(TRACE_LEVEL_INFORMATION, 
            "Unexpected request, %!hresult!\n", hr);
        return hr;
    }

    //
    // Get output buffer
    //
    hr = FxRequest->RetrieveOutputMemory(&memory);
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, "RetrieveOutputMemory failed %!hresult!\n", hr);
        return hr;        
    }
    reportBuffer = (PUCHAR) memory->GetDataBuffer(&outBufferCb);
    SAFE_RELEASE(memory);
    
    //
    // Since output buffer is for write only (no read allowed by UMDF in output
    // buffer), any read from output buffer would be reading garbage), so don't 
    // let app embed custom control code in output buffer. The minidriver can 
    // support multiple features using separate report ID instead of using 
    // custom control code. Since this is targeted at report ID 1, we know it
    // is a request for getting attributes.
    //
    reportSize = sizeof(MY_DEVICE_ATTRIBUTES) + 1;  // +1 for report ID
    if (outBufferCb < reportSize)
    {
        hr = HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! Insufficient report buffer size %!hresult!\n", hr);
        return hr;
    }

    //
    // Since this device has one report ID, hidclass would pass on the report
    // ID in the buffer (it wouldn't if report descriptor did not have any report
    // ID). However, since UMDF allows only writes to an output buffer, we can't 
    // "read" the report ID from "output" buffer. There is no need to read the 
    // report ID since we get it other way as shown above, however this is 
    // something to keep in mind.
    //
    reportBuffer++;   // skip the report ID
    myAttributes = (PMY_DEVICE_ATTRIBUTES) reportBuffer;
    myAttributes->ProductID = m_Device->m_Attributes.ProductID;
    myAttributes->VendorID = m_Device->m_Attributes.VendorID;
    myAttributes->VersionNumber = m_Device->m_Attributes.VersionNumber;

    //
    // Report how many bytes were copied
    //
    FxRequest->SetInformation(reportSize);
    hr = S_OK;
    
    return hr;
}

HRESULT
CMyQueue::SetFeature(
    _In_ IWDFIoRequest2 *FxRequest
    )
/*++

Routine Description:

    Handles IOCTL_UMDF_HID_SET_FEATURE for all the collection.
    For control collection (custom defined collection) it handles
    the user-defined control codes for sideband communication

Arguments:

    Request - Pointer to Request Packet.

Return Value:

    NT status code.

--*/
{
    HRESULT hr;
    IWDFMemory *memory = NULL;
    SIZE_T inBufferCb, reportSize;
    UCHAR reportId; 
    SIZE_T pOutBufferSize;
    PHIDMINI_CONTROL_INFO controlInfo = NULL;
    
    // *************************************************************************
    // IOCTL_UMDF_HID_SET_FEATURE
    // Input report ID:
    //     UMDF driver retrieves report ID associated with this collection 
    //     through pOutBufferSize parameter passed to a call to 
    //     IWDFRequest::GetDeviceIoControlParameters DDI.
    // Input report buffer and length: 
    //     UMDF driver calls IWDFRequest::GetInputMemory() to get memory 
    //     buffer and its length. The buffer contains feature report data that
    //     driver transfers to the device. 
    //
    // *************************************************************************

    Trace(TRACE_LEVEL_INFORMATION, "SetFeature\n");

    //
    // Get report ID if needed.
    //
    FxRequest->GetDeviceIoControlParameters(NULL, NULL, &pOutBufferSize);
    reportId = (UCHAR) pOutBufferSize;

    if (reportId != CONTROL_COLLECTION_REPORT_ID)
    {
        //
        // If collection ID is not for control collection then handle
        // this request just as you would for a regular collection.
        //
        hr = HRESULT_FROM_NT(STATUS_INVALID_PARAMETER);
        Trace(TRACE_LEVEL_INFORMATION, 
            "Unknown request, %!hresult!\n", hr);
        return hr;
    }

    //
    // Get input report buffer. 
    //
    hr = FxRequest->RetrieveInputMemory(&memory);
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, "RetrieveInputMemory failed %!hresult!\n", hr);
        return hr;        
    }
    controlInfo = (PHIDMINI_CONTROL_INFO)memory->GetDataBuffer(&inBufferCb);
    SAFE_RELEASE(memory);

    //
    // before touching control code make sure buffer is big enough.
    //
    reportSize = sizeof(HIDMINI_CONTROL_INFO);
    Trace(TRACE_LEVEL_INFORMATION, "Expected report size: %I64d\n", reportSize);
    if (inBufferCb < reportSize) 
    {
        hr = HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! Unexpected buffer size %I64d, expected %I64d %!hresult!\n", 
            inBufferCb, reportSize, hr);
        return hr;
    }

    switch(controlInfo->ControlCode) 
    {
    case HIDMINI_CONTROL_CODE_SET_ATTRIBUTES:
        //
        // Store the device attributes in device extension
        //
        m_Device->m_Attributes.ProductID = controlInfo->u.Attributes.ProductID;
        m_Device->m_Attributes.VendorID = controlInfo->u.Attributes.VendorID;
        m_Device->m_Attributes.VersionNumber = controlInfo->u.Attributes.VersionNumber;

        //
        // set status and information
        //
        FxRequest->SetInformation(reportSize);
        hr = S_OK;

        break;

    case HIDMINI_CONTROL_CODE_DUMMY1:
        Trace(TRACE_LEVEL_INFORMATION,
            "Control Code HIDMINI_CONTROL_CODE_DUMMY1\n");
        hr = HRESULT_FROM_NT(STATUS_NOT_IMPLEMENTED);
        break;

    case HIDMINI_CONTROL_CODE_DUMMY2:
        Trace(TRACE_LEVEL_INFORMATION, 
            "Control Code HIDMINI_CONTROL_CODE_DUMMY2\n");
        hr = HRESULT_FROM_NT(STATUS_NOT_IMPLEMENTED);
        break;

    default:
        Trace(TRACE_LEVEL_INFORMATION, "Unknown control Code\n");
        hr = HRESULT_FROM_NT(STATUS_NOT_IMPLEMENTED);
        break;
    }

    return hr;
}

HRESULT
CMyQueue::GetInputReport(
    _In_ IWDFIoRequest2 *FxRequest
    )
/*++

Routine Description:

    Handles IOCTL_UMDF_HID_GET_INPUT_REPORT for all the collection.

Arguments:

    Request - Pointer to Request Packet.

Return Value:

    NT status code.

--*/
{
    HRESULT hr;
    IWDFMemory *memory = NULL;
    SIZE_T inBufferCb, outBufferCb;
    ULONG reportSize;
    PVOID inBuffer;
    UCHAR reportId;
    PHIDMINI_INPUT_REPORT reportBuffer = NULL;

    // *************************************************************************
    // IOCTL_UMDF_HID_GET_INPUT_REPORT
    // Input report ID:
    //     UMDF driver retrieves report ID associated with this collection 
    //     by retrieving the buffer containing report ID by calling 
    //     IWDFRequest::RetrieveInputMemory(). 
    // Output report buffer and length: 
    //     UMDF driver calls IWDFRequest::RetrieveOutputMemory() to get memory 
    //     buffer and its length. The driver fills the buffer with feature data. 
    //
    // *************************************************************************

    Trace(TRACE_LEVEL_INFORMATION, "GetInputReport\n");

    //
    // Get input report ID
    //
    hr = FxRequest->RetrieveInputMemory(&memory);
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, "RetrieveInputMemory failed %!hresult!\n", hr);
        return hr;        
    }

    inBuffer = memory->GetDataBuffer(&inBufferCb);
    SAFE_RELEASE(memory);
    
    if (inBufferCb < sizeof(reportId)) 
    {
        hr = HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! Insufficient buffer size %!hresult!\n", hr);
        return hr;
    }

    reportId = *(PUCHAR)inBuffer;
    if (reportId != CONTROL_COLLECTION_REPORT_ID)
    {
        //
        // If collection ID is not for control collection then handle
        // this request just as you would for a regular collection.
        //
        hr = HRESULT_FROM_NT(STATUS_INVALID_PARAMETER);
        Trace(TRACE_LEVEL_INFORMATION, 
            "Unexpected request, %!hresult!\n", hr);
        return hr;
    }

    //
    // Get output buffer
    //
    hr = FxRequest->RetrieveOutputMemory(&memory);
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, "RetrieveOutputMemory failed %!hresult!\n", hr);
        return hr;        
    }
    reportBuffer = (PHIDMINI_INPUT_REPORT) memory->GetDataBuffer(&outBufferCb);
    SAFE_RELEASE(memory);
    
    reportSize = sizeof(HIDMINI_INPUT_REPORT);
    if (outBufferCb < reportSize)
    {
        hr = HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! Insufficient report buffer size %!hresult!\n", hr);
        return hr;
    }

    reportBuffer->ReportId = CONTROL_COLLECTION_REPORT_ID;
    reportBuffer->Data = m_OutputReport;

    Trace(TRACE_LEVEL_INFORMATION, "Returning %d in input report\n", m_OutputReport);

    //
    // Report how many bytes were copied
    //
    FxRequest->SetInformation(reportSize);
    hr = S_OK;
    
    return hr;
}


HRESULT
CMyQueue::SetOutputReport(
    _In_ IWDFIoRequest2 *FxRequest
    )
/*++

Routine Description:

    Handles IOCTL_UMDF_HID_SET_OUTPUT_REPORT for all the collection.

Arguments:

    Request - Pointer to Request Packet.

Return Value:

    NT status code.

--*/
{
    HRESULT hr;
    IWDFMemory *memory = NULL;
    SIZE_T inBufferCb, reportSize;
    UCHAR reportId; 
    SIZE_T pOutBufferSize;
    PHIDMINI_OUTPUT_REPORT reportBuffer = NULL;
    
    // *************************************************************************
    // IOCTL_UMDF_HID_SET_OUTPUT_REPORT
    // Input report ID:
    //     UMDF driver retrieves report ID associated with this collection 
    //     through pOutBufferSize parameter passed to a call to 
    //     IWDFRequest::GetDeviceIoControlParameters DDI.
    // Input report buffer and length: 
    //     UMDF driver calls IWDFRequest::GetInputMemory() to get memory 
    //     buffer and its length. The buffer contains feature report data that
    //     driver transfers to the device. 
    //
    // *************************************************************************

    Trace(TRACE_LEVEL_INFORMATION, "SetOutputReport\n");

    //
    // Get report ID if needed.
    //
    FxRequest->GetDeviceIoControlParameters(NULL, NULL, &pOutBufferSize);
    reportId = (UCHAR) pOutBufferSize;

    if (reportId != CONTROL_COLLECTION_REPORT_ID)
    {
        //
        // If collection ID is not for control collection then handle
        // this request just as you would for a regular collection.
        //
        hr = HRESULT_FROM_NT(STATUS_INVALID_PARAMETER);
        Trace(TRACE_LEVEL_INFORMATION, 
            "Unknown request, %!hresult!\n", hr);
        return hr;
    }

    //
    // Get input report buffer. 
    //
    hr = FxRequest->RetrieveInputMemory(&memory);
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, "RetrieveInputMemory failed %!hresult!\n", hr);
        return hr;        
    }
    
    reportBuffer = (PHIDMINI_OUTPUT_REPORT)memory->GetDataBuffer(&inBufferCb);
    SAFE_RELEASE(memory);

    //
    // before touching buffer make sure buffer is big enough.
    //
    reportSize = sizeof(PHIDMINI_OUTPUT_REPORT);
    Trace(TRACE_LEVEL_INFORMATION, "Expected report size: %I64d\n", reportSize);
    if (inBufferCb < reportSize) 
    {
        hr = HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! Unexpected buffer size %I64d, expected %I64d %!hresult!\n", 
            inBufferCb, reportSize, hr);
        return hr;
    }

    m_OutputReport = reportBuffer->Data;
    Trace(TRACE_LEVEL_INFORMATION, "Received %d in output report\n", reportBuffer->Data);

    //
    // Report how many bytes were copied
    //
    FxRequest->SetInformation(reportSize);
    hr = S_OK;

    return hr;
}


HRESULT
CMyQueue::WriteReport(
    _In_ IWDFIoRequest2 *FxRequest
    )
/*++

Routine Description:

    Handles IOCTL_HID_WRITE_REPORT all the collection.

Arguments:

    Request - Pointer to  Request Packet.

Return Value:

    NT status code.

--*/
{
    HRESULT hr;
    IWDFMemory *memory = NULL;
    SIZE_T inBufferCb, reportSize;
    UCHAR reportId; 
    SIZE_T pOutBufferSize;
    PHIDMINI_OUTPUT_REPORT outputReport = NULL;
    
    // *************************************************************************
    // IOCTL_HID_WRITE_REPORT
    // Input report ID:
    //     UMDF driver retrieves report ID associated with this collection 
    //     through pOutBufferSize parameter passed to a call to 
    //     IWDFRequest::GetDeviceIoControlParameters DDI.
    // Input report buffer and length: 
    //     UMDF driver calls IWDFRequest::GetInputMemory() to get memory 
    //     buffer and its length. The buffer contains feature report data that
    //     driver transfers to the device. 
    //
    // *************************************************************************

    Trace(TRACE_LEVEL_INFORMATION, "WriteReport\n");

    //
    // Get report ID if needed.
    //
    FxRequest->GetDeviceIoControlParameters(NULL, NULL, &pOutBufferSize);
    reportId = (UCHAR) pOutBufferSize;

    if (reportId != CONTROL_COLLECTION_REPORT_ID)
    {
        //
        // If collection ID is not for control collection then handle
        // this request just as you would for a regular collection.
        //
        hr = HRESULT_FROM_NT(STATUS_INVALID_PARAMETER);
        Trace(TRACE_LEVEL_INFORMATION, 
            "Unknown request, %!hresult!\n", hr);
        return hr;
    }

    //
    // Get input report buffer. 
    //
    hr = FxRequest->RetrieveInputMemory(&memory);
    if (FAILED(hr)) {
        Trace(TRACE_LEVEL_ERROR, "RetrieveINputMemory failed %!hresult!\n", hr);
        return hr;        
    }
    outputReport = (PHIDMINI_OUTPUT_REPORT) memory->GetDataBuffer(&inBufferCb);
    SAFE_RELEASE(memory);

    //
    // make sure buffer is big enough.
    //
    reportSize = sizeof(HIDMINI_OUTPUT_REPORT);  
    if (inBufferCb < reportSize) 
    {
        hr =  HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! Unexpected buffer size %!hresult!\n", hr);
        return hr;
    }

    //
    // Store the device data in device extension. 
    //
    m_Device->m_DeviceData = outputReport->Data;

    //
    // set status and information
    //
    FxRequest->SetInformation(reportSize);
    hr = S_OK;

    return hr;
}

HRESULT
CMyQueue::GetString(
    _In_ IWDFIoRequest2 *FxRequest
    )
/*++

Routine Description:

    Handles IOCTL_HID_GET_INDEXED_STRING and IOCTL_HID_GET_STRING.

Arguments:

    Request - Pointer to Request Packet.

Return Value:

    NT status code.

--*/
{
    HRESULT hr;
    IWDFMemory *memory = NULL;
    SIZE_T inBufferCb, outBufferCb, stringSizeCb;
    PVOID inBuffer, outBuffer;
    ULONG languageId, stringIndex, stringId;
    BOOLEAN isIndexedString = FALSE;
    ULONG ioctl;
    PWSTR string = NULL;

    // *************************************************************************
    // IOCTL_HID_GET_INDEXED_STRING/IOCTL_HID_GET_STRING
    // Input:
    //     UMDF driver calls IWDFRequest::GetInputMemory() to get the buffer 
    //     that contains an INT value that describes the string to be retrieved.
    //     The most significant two bytes of the INT value contain the language
    //     ID (for example, a value of 1033 indicates English). 
    //     The least significant two bytes of the INT value contain the string 
    //     index in cae of IOCTL_HID_GET_INDEXED_STRING. 
    //     In case of IOCTL_HID_GET_STRING, the two least significant bytes 
    //     contain one of the following three constant values: 
    //     HID_STRING_ID_IMANUFACTURER, HID_STRING_ID_IPRODUCT,
    //     HID_STRING_ID_ISERIALNUMBER.
    // Output:
    //     UMDF driver calls IWDFRequest::GetOutputMemory() to get the output 
    //     buffer and fill it with the retrieved string (a NULL-terminated wide 
    //     character string). 
    //
    // *************************************************************************

    Trace(TRACE_LEVEL_INFORMATION, "GetIndexedString\n");

    //
    // Get input report buffer. 
    //
    hr = FxRequest->RetrieveInputMemory(&memory);
    if (FAILED(hr)) 
    {
        Trace(TRACE_LEVEL_ERROR, "RetrieveINputMemory failed %!hresult!\n", hr);
        return hr;        
    }
    inBuffer = memory->GetDataBuffer(&inBufferCb);
    SAFE_RELEASE(memory);

    //
    // make sure buffer is big enough.
    //
    if (inBufferCb < sizeof(ULONG)) 
    {
        hr =  HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! Unexpected input buffer size %!hresult!\n", hr);
        return hr;
    }

    FxRequest->GetDeviceIoControlParameters(&ioctl, NULL, NULL);
    if (ioctl == IOCTL_HID_GET_INDEXED_STRING) 
    {
        isIndexedString = TRUE;
    }

    //
    // The most significant two bytes of the INT value contain the language
    // ID (for example, a value of 1033 indicates English). 
    //
    languageId = (*(PULONG)inBuffer) >> 16;
    UNREFERENCED_PARAMETER(languageId);

    if (isIndexedString)
    {
        //
        // The least significant two bytes of the INT value contain the string index.
        //
        stringIndex = ((*(PULONG)inBuffer) & 0x0ffff);
        if (stringIndex != VHIDMINI_DEVICE_STRING_INDEX) 
        {
            hr = E_INVALIDARG;
            Trace(TRACE_LEVEL_ERROR, "%!FUNC! Unexpected string index %!hresult!\n", hr);
            return hr;
        }

        stringSizeCb = sizeof(VHIDMINI_DEVICE_STRING);
        string = VHIDMINI_DEVICE_STRING;
    }
    else 
    {
        //
        // The least significant two bytes of the INT value contain the string Id.
        //
        stringId = ((*(PULONG)inBuffer) & 0x0ffff);

        switch (stringId){
        case HID_STRING_ID_IMANUFACTURER:
            stringSizeCb = sizeof(VHIDMINI_MANUFACTURER_STRING);
            string = VHIDMINI_MANUFACTURER_STRING;
            break;
        case HID_STRING_ID_IPRODUCT:
            stringSizeCb = sizeof(VHIDMINI_PRODUCT_STRING);
            string = VHIDMINI_PRODUCT_STRING;
            break;
        case HID_STRING_ID_ISERIALNUMBER:
            stringSizeCb = sizeof(VHIDMINI_SERIAL_NUMBER_STRING);
            string = VHIDMINI_SERIAL_NUMBER_STRING;
            break;
        default:
            stringSizeCb = 0;
            string = NULL;
            break;
        }
    }

    //
    // Get output buffer 
    //
    hr = FxRequest->RetrieveOutputMemory(&memory);
    if (FAILED(hr)) 
    {
        Trace(TRACE_LEVEL_ERROR, "RetrieveOutputMemory failed %!hresult!\n", hr);
        return hr;        
    }
    outBuffer = (PUCHAR) memory->GetDataBuffer(&outBufferCb);
    SAFE_RELEASE(memory);

    if (outBufferCb < stringSizeCb) 
    {
        hr = HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
        Trace(TRACE_LEVEL_ERROR, 
            "%!FUNC! Unexpected output buffer size %!hresult!\n", hr);
        return hr;
    }

    CopyMemory(outBuffer, string, stringSizeCb);

    //
    // set status and information
    //
    FxRequest->SetInformation(stringSizeCb);
    hr = S_OK;

    return hr;
}

HRESULT
CMyManualQueue::CreateInstance(
    _In_ PCMyDevice Device,
    _Out_ CMyManualQueue **Queue
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
    CMyManualQueue *queue;

    HRESULT hr = S_OK;

    //
    // Allocate a new instance of the device class.
    //
    queue = new CMyManualQueue(Device);

    if (NULL == queue)
    {
        hr = E_OUTOFMEMORY;
    }

    //
    // Initialize the instance.
    //
    if (SUCCEEDED(hr)) 
    {
        hr = queue->Initialize(Device->GetFxDevice());
    }

    if (SUCCEEDED(hr)) 
    {
        queue->AddRef();
        *Queue = queue;
    }

    if (NULL != queue)
    {
        queue->Release();
    }

    return hr;
}

HRESULT
CMyManualQueue::QueryInterface(
    _In_ REFIID InterfaceId,
    _Out_ PVOID *Object
    )
/*++
 
  Routine Description:

    This method is called to get a pointer to one of the object's callback
    interfaces.  

  Arguments:

    InterfaceId - the interface being requested

    Object - a location to store the interface pointer if successful

  Return Value:

    S_OK or E_NOINTERFACE

--*/
{
    HRESULT hr;

    if (IsEqualIID(InterfaceId, __uuidof(IObjectCleanup))) {
        AddRef();
        *Object = static_cast<IObjectCleanup *>(this);
        hr = S_OK; 
    }
    else 
    {
        hr = CUnknown::QueryInterface(InterfaceId, Object);
    }

    return hr;
}

HRESULT
CMyManualQueue::Initialize(
    _In_ IWDFDevice *FxDevice
    )
/*++
 
  Routine Description:

    This method initializes the device callback object.  Any operations which
    need to be performed before the caller can use the callback object, but 
    which couldn't be done in the constructor becuase they could fail would
    be placed here.

  Arguments:

    FxDevice - the device which this Queue is for.

  Return Value:

    status.

--*/
{
    IWDFIoQueue     *fxQueue;
    HRESULT hr;

    //
    // Create the framework queue
    //
    IUnknown *unknown = QueryIUnknown();
    hr = FxDevice->CreateIoQueue(
                        unknown,
                        FALSE,                        // bDefaultQueue
                        WdfIoQueueDispatchManual, 
                        FALSE,                       // bPowerManaged
                        TRUE,                        // bAllowZeroLengthRequests
                        &fxQueue 
                        );
    if (FAILED(hr))
    {
        Trace(
            TRACE_LEVEL_ERROR, 
            "%!FUNC!: Could not create manual I/O queue, %!hresult!",
            hr
            );
    }

    if (SUCCEEDED(hr)) 
    {
        m_FxQueue = fxQueue;
        
        fxQueue->Release();
    }

    unknown->Release();

    if (SUCCEEDED(hr))
    {
        //
        // create a threadpool timer
        //
        m_Timer = CreateThreadpoolTimer(_TimerCallback,
                                        this,
                                        NULL  // use default threadpool
                                        );
        if (m_Timer == NULL) {
            hr = E_OUTOFMEMORY;
            Trace(TRACE_LEVEL_ERROR, 
                "Failed to allocate timer %!hresult!", hr);
        }           
    }

    return hr;
}

VOID 
CMyManualQueue::_TimerCallback(
    _Inout_      PTP_CALLBACK_INSTANCE Instance,
    _Inout_      PVOID Context,
    _Inout_      PTP_TIMER Timer
    )
{
    PCMyManualQueue This = (PCMyManualQueue) Context;
    HRESULT hr;
    IWDFIoRequest *fxRequest = NULL;
    IWDFMemory *memory = NULL;
    PVOID buffer;
    SIZE_T bufferSizeCb;
    ULONG readReportSizeCb = sizeof(HIDMINI_INPUT_REPORT);
    PHIDMINI_INPUT_REPORT readReport;
    
    UNREFERENCED_PARAMETER(Instance);
    UNREFERENCED_PARAMETER(Timer);

    Trace(TRACE_LEVEL_ERROR, "_TimerCallback CMyQueue 0x%p\n", This);

    //
    // see if we have a request in manual queue
    //
    hr = This->GetQueue()->RetrieveNextRequest(&fxRequest);
    if (SUCCEEDED(hr)) {
        IWDFIoRequest2 *fxRequest2;
        
        Trace(TRACE_LEVEL_INFORMATION, "retrieved read request from manual queue CMyManualQueue 0x%p\n", This);

        hr = fxRequest->QueryInterface(IID_PPV_ARGS(&fxRequest2));
        if (FAILED(hr)){
            Trace(TRACE_LEVEL_ERROR, "QueryInterface failed %!hresult!", hr);
            fxRequest->Complete(hr);
            fxRequest->Release();
            return;
        }
        fxRequest2->Release();

        Trace(TRACE_LEVEL_INFORMATION, "EffectiveIoType: %d\n", fxRequest2->GetEffectiveIoType());

        hr = fxRequest2->RetrieveOutputMemory(&memory);
        if (FAILED(hr)) {
            Trace(TRACE_LEVEL_ERROR, "RetrieveINputMemory failed %!hresult!", hr);
            fxRequest2->Complete(hr);
            fxRequest2->Release();
            return;
        }

        buffer = memory->GetDataBuffer(&bufferSizeCb);
        memory->Release();
        
        if (bufferSizeCb < readReportSizeCb) 
        {
            hr = HRESULT_FROM_NT(STATUS_INVALID_BUFFER_SIZE);
            Trace(TRACE_LEVEL_ERROR, 
                "%!FUNC! Insufficient read report buffer size %!hresult!", hr);
        }
        else
        {
            //
            //Create input report
            //
            readReport = (PHIDMINI_INPUT_REPORT)buffer;
            readReport->ReportId = CONTROL_FEATURE_REPORT_ID;
            readReport->Data = This->m_Device->m_DeviceData;
            
            //
            // Report how many bytes were copied
            //
            fxRequest2->SetInformation(readReportSizeCb);
            hr = S_OK;
        }

        fxRequest2->Complete(hr);
        fxRequest2->Release();
    }

    return;
}

void
CMyManualQueue::OnCleanup(
    IN IWDFObject*  pWdfObject
    )
{
    UNREFERENCED_PARAMETER(pWdfObject);
    
    if (m_Timer != NULL) {
        //
        // Prevent new queues. If pftDueTime parameter is NULL, the timer object
        // will cease to queue new callbacks (but callbacks already queued will
        // still occur).
        //
        SetThreadpoolTimer(
            m_Timer,  
            NULL,  // PFILETIME pftDueTime,
            0,     // DWORD msPeriod,
            0      // DWORD msWindowLength
            );

        //
        // wait for timer callback. 
        //
        WaitForThreadpoolTimerCallbacks(m_Timer,
                                        FALSE    // donot cancel pending 
                                        );
        //
        // close timer callback
        //
        CloseThreadpoolTimer(m_Timer);

        m_Timer = NULL;
    }
}

