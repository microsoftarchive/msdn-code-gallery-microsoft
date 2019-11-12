/*++

Copyright (c) Microsoft Corporation.  All rights reserved.

    THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
    KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
    PURPOSE.

Module Name:

    testvhid.c

Environment:

    user mode only

Author:

--*/

#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <setupapi.h>
#include <hidsdi.h>
#include <TCHAR.h>
#include "common.h"
#include <time.h>
#include <dontuse.h>

//
// These are the default device attributes set in the driver
// which are used to identify the device.
//
#define HIDMINI_DEFAULT_PID              0xFEED
#define HIDMINI_DEFAULT_VID              0xBEEF

//
// These are the device attributes returned by the mini driver in response
// to IOCTL_HID_GET_DEVICE_ATTRIBUTES.
//
#define HIDMINI_TEST_PID              0xDEAF
#define HIDMINI_TEST_VID              0xFEED
#define HIDMINI_TEST_VERSION          0x0505


//
// Function prototypes
//
BOOLEAN
GetFeature(
    HANDLE file
    );

BOOLEAN
SetFeature(
    HANDLE file
    );

BOOLEAN
GetInputReport(
    HANDLE file
    );

BOOLEAN
SetOutputReport(
    HANDLE file
    );

BOOL
SearchMatchingHwID (
    _In_ HDEVINFO            DeviceInfoSet,
    _In_ PSP_DEVINFO_DATA    DeviceInfoData
    );

BOOL
OpenDeviceInterface (
    _In_       HDEVINFO                    HardwareDeviceInfo,
    _In_       PSP_DEVICE_INTERFACE_DATA   DeviceInterfaceData,
    _In_       HANDLE*                     File
    );

BOOLEAN
CheckIfOurDevice(
    HANDLE file
    );

BOOLEAN
ReadInputData(
    _In_ HANDLE file
    );

BOOLEAN
WriteOutputData(
    _In_ HANDLE file
    );

BOOLEAN
GetIndexedString(
    HANDLE File
    );

BOOLEAN
GetStrings(
    HANDLE File
    );

//
// Implementation
//
INT __cdecl
main(
    _In_ ULONG argc,
    _In_reads_(argc) PCHAR argv[]
    )
{
    HDEVINFO hardwareDeviceInfo;
    SP_DEVICE_INTERFACE_DATA deviceInterfaceData;
    SP_DEVINFO_DATA devInfoData;
    GUID hidguid;
    int i;
    HANDLE file = INVALID_HANDLE_VALUE;
    BOOLEAN found = FALSE;
    BOOLEAN bSuccess = FALSE;


    UNREFERENCED_PARAMETER(argc);
    UNREFERENCED_PARAMETER(argv);


    srand( (unsigned)time( NULL ) );

    HidD_GetHidGuid(&hidguid);

    hardwareDeviceInfo =
            SetupDiGetClassDevs ((LPGUID)&hidguid,
                                            NULL,
                                            NULL, // Define no
                                            (DIGCF_PRESENT |
                                            DIGCF_INTERFACEDEVICE));

    if (INVALID_HANDLE_VALUE == hardwareDeviceInfo){
        printf("SetupDiGetClassDevs failed: %x\n", GetLastError());
        return 1;
    }

    deviceInterfaceData.cbSize = sizeof (SP_DEVICE_INTERFACE_DATA);

    devInfoData.cbSize = sizeof(SP_DEVINFO_DATA);

    //
    // Enumerate devices of this interface class
    //
    printf("\n....looking for our HID device (with UP=0xFF00 "
                "and Usage=0x01)\n");

    for(i=0; SetupDiEnumDeviceInterfaces (hardwareDeviceInfo,
                            0, // No care about specific PDOs
                            (LPGUID)&hidguid,
                            i, //
                            &deviceInterfaceData);
                            i++ ){

        //
        // Open the device interface and Check if it is our device
        // by matching the Usage page and Usage from Hid_Caps.
        // If this is our device then send the hid request.
        //
        if (OpenDeviceInterface(hardwareDeviceInfo, 
                                &deviceInterfaceData,
                                &file)){
            found = TRUE;
            break;
        }

        //
        //device was not found so loop around.
        //
    }

    if (found) {
        printf("...sending control request to our device\n");

        //
        // Get/Set feature loopback 
        //
        bSuccess = GetFeature(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }
        
        bSuccess = SetFeature(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }

        bSuccess = GetFeature(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }

        //
        // Get/Set report loopback
        //
        bSuccess = GetInputReport(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }

        bSuccess = SetOutputReport(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }

        bSuccess = GetInputReport(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }

        //
        // Read/Write report loopback 
        //
        bSuccess = ReadInputData(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }

        bSuccess = WriteOutputData(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }

        bSuccess = ReadInputData(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }

        //
        // Get Strings
        //
        bSuccess = GetIndexedString(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }

        bSuccess = GetStrings(file);
        if (bSuccess == FALSE) {
            goto cleanup;
        }

    }
    else {
        printf("Failure: Could not find our HID device \n");
    }

cleanup:

    if (found && bSuccess == FALSE) {
        printf("****** Failure: one or more commands to device failed *******\n");
    }

    SetupDiDestroyDeviceInfoList (hardwareDeviceInfo);

    if (file != INVALID_HANDLE_VALUE) {
        CloseHandle(file);
    }

    return (bSuccess ? 0 : 1);
}


BOOL
OpenDeviceInterface (
    _In_ HDEVINFO hardwareDeviceInfo,
    _In_ PSP_DEVICE_INTERFACE_DATA deviceInterfaceData,
    _In_ HANDLE* File
    )
{
    PSP_DEVICE_INTERFACE_DETAIL_DATA    deviceInterfaceDetailData = NULL;
    DWORD predictedLength = 0;
    DWORD requiredLength = 0;
    HANDLE file;
    BOOL deviceFound = FALSE;

    *File = INVALID_HANDLE_VALUE;
    SetupDiGetDeviceInterfaceDetail(
                            hardwareDeviceInfo,
                            deviceInterfaceData,
                            NULL, // probing so no output buffer yet
                            0, // probing so output buffer length of zero
                            &requiredLength,
                            NULL
                            ); // not interested in the specific dev-node

    predictedLength = requiredLength;

    deviceInterfaceDetailData =
         (PSP_DEVICE_INTERFACE_DETAIL_DATA) malloc (predictedLength);

    if (!deviceInterfaceDetailData)
    {
        printf("Error: OpenDeviceInterface: malloc failed\n");
        return FALSE;
    }

    deviceInterfaceDetailData->cbSize =
                    sizeof (SP_DEVICE_INTERFACE_DETAIL_DATA);

    if (!SetupDiGetDeviceInterfaceDetail(
                            hardwareDeviceInfo,
                            deviceInterfaceData,
                            deviceInterfaceDetailData,
                            predictedLength,
                            &requiredLength,
                            NULL))
    {
        printf("Error: SetupDiGetInterfaceDeviceDetail failed\n");
        free (deviceInterfaceDetailData);
        return FALSE;
    }

    file = CreateFile ( deviceInterfaceDetailData->DevicePath,
                            GENERIC_READ | GENERIC_WRITE,
                            0, // FILE_SHARE_READ | FILE_SHARE_READ |
                            NULL, // no SECURITY_ATTRIBUTES structure
                            OPEN_EXISTING, // No special create flags
                            0, // No special attributes
                            NULL); // No template file

    if (INVALID_HANDLE_VALUE == file) {
        printf("Error: CreateFile failed: %d\n", GetLastError());
        free (deviceInterfaceDetailData);
        return FALSE;
    }

    if (CheckIfOurDevice(file)){
        deviceFound  = TRUE;
        *File = file;
    }
    else {
        CloseHandle(file);
    }

    free (deviceInterfaceDetailData);

    return deviceFound;
}


BOOLEAN
CheckIfOurDevice(
    HANDLE file)
{
    PHIDP_PREPARSED_DATA Ppd; // The opaque parser info describing this device
    HIDP_CAPS                       Caps; // The Capabilities of this hid device.
    USAGE                               MyUsagePage = 0xff00;
    USAGE                               MyUsage = 0x0001;
    HIDD_ATTRIBUTES attr; // Device attributes

    if (!HidD_GetAttributes(file, &attr))
    {
        printf("Error: HidD_GetAttributes failed \n");
        return FALSE;
    }

    printf("Device Attributes - PID: 0x%x, VID: 0x%x \n", attr.ProductID, attr.VendorID);
    if ((attr.VendorID != HIDMINI_DEFAULT_VID) || (attr.ProductID != HIDMINI_DEFAULT_PID))
    {
        printf("Device attributes doesn't match the sample \n");
        return FALSE;
    }

    if (!HidD_GetPreparsedData (file, &Ppd))
    {
        printf("Error: HidD_GetPreparsedData failed \n");
        return FALSE;
    }

    if (!HidP_GetCaps (Ppd, &Caps))
    {
        printf("Error: HidP_GetCaps failed \n");
        HidD_FreePreparsedData (Ppd);
        return FALSE;
    }

    if ((Caps.UsagePage == MyUsagePage) && (Caps.Usage == MyUsage)){
        printf("Success: Found my device.. \n");
        return TRUE;

    }

    return FALSE;

}

BOOLEAN
GetFeature(
    HANDLE file
    )
{
    PMY_DEVICE_ATTRIBUTES myDevAttributes = NULL;
    ULONG bufferSize;
    PUCHAR buffer;
    BOOLEAN bSuccess;

    //
    // Allocate memory equal to 1 byte for report ID + size of  
    // feature report. Buffer size for get feature should be atleast equal to 
    // the size of feature report.
    //
    bufferSize = FEATURE_REPORT_SIZE_CB + 1;
    buffer = (PUCHAR) malloc (bufferSize);
    if (!buffer )
    {
        printf("malloc failed\n");
        return FALSE;
    }

    ZeroMemory(buffer, bufferSize);

    //
    // Fill the first byte with report ID of control collection
    //
    buffer[0] = CONTROL_COLLECTION_REPORT_ID;

    //
    // Send Hid control code thru HidD_GetFeature API
    //
    bSuccess = HidD_GetFeature(file,  // HidDeviceObject,
                                   buffer,    // ReportBuffer,
                                   bufferSize // ReportBufferLength
                                   );
    if (!bSuccess)
    {
        printf("failed HidD_GetFeature\n");
    }
    else
    {
        myDevAttributes = (PMY_DEVICE_ATTRIBUTES) (buffer + 1); // +1 to skip report id

        printf("Received following feature attributes from device: \n"
               "    VendorID: 0x%x, \n"
               "    ProductID: 0x%x, \n"
               "    VersionNumber: 0x%x\n",
               myDevAttributes->VendorID,
               myDevAttributes->ProductID,
               myDevAttributes->VersionNumber);
    }

    free(buffer);
    return bSuccess;
}

BOOLEAN
SetFeature(
    HANDLE file
    )
{
    PHIDMINI_CONTROL_INFO  controlInfo = NULL;
    PMY_DEVICE_ATTRIBUTES myDevAttributes = NULL;
    ULONG bufferSize;
    BOOLEAN bSuccess;

    //
    // Allocate memory equal to HIDMINI_CONTROL_INFO
    //
    bufferSize = sizeof(HIDMINI_CONTROL_INFO);
    controlInfo = (PHIDMINI_CONTROL_INFO) malloc (bufferSize);
    if (!controlInfo )
    {
        printf("malloc failed\n");
        return FALSE;
    }

    ZeroMemory(controlInfo, bufferSize);

    //
    // Fill the control structure with the report ID of the control collection and
    // the control code.
    //
    controlInfo->ReportId = CONTROL_COLLECTION_REPORT_ID;
    controlInfo->ControlCode = HIDMINI_CONTROL_CODE_SET_ATTRIBUTES;
    myDevAttributes = (PMY_DEVICE_ATTRIBUTES)&controlInfo->u.Attributes;
    myDevAttributes->VendorID = HIDMINI_TEST_VID;
    myDevAttributes->ProductID = HIDMINI_TEST_PID;
    myDevAttributes->VersionNumber = HIDMINI_TEST_VERSION;

    //
    // Set feature
    //
    bSuccess = HidD_SetFeature(file,         // HidDeviceObject,
                         controlInfo,  // ReportBuffer,
                         bufferSize    // ReportBufferLength
                         );
    if (!bSuccess)
    {
        printf("failed HidD_SetFeature \n");
    }
    else
    {
        printf("Set following feature attributes on device: \n"
               "    VendorID: 0x%x, \n"
               "    ProductID: 0x%x, \n"
               "    VersionNumber: 0x%x\n",
               myDevAttributes->VendorID,
               myDevAttributes->ProductID,
               myDevAttributes->VersionNumber);
    }

    free(controlInfo);

    return bSuccess;
}

BOOLEAN
GetInputReport(
    HANDLE file
    )
{
    ULONG bufferSize;
    PUCHAR buffer;
    BOOLEAN bSuccess;

    //
    // Allocate memory 
    //
    bufferSize = sizeof(HIDMINI_INPUT_REPORT);
    buffer = (PUCHAR) malloc (bufferSize);
    if (!buffer )
    {
        printf("malloc failed\n");
        return FALSE;
    }

    ZeroMemory(buffer, bufferSize);

    //
    // Fill the first byte with report ID of collection
    //
    buffer[0] = CONTROL_COLLECTION_REPORT_ID;

    //
    // Send Hid control code 
    //
    bSuccess = HidD_GetInputReport(file,  // HidDeviceObject,
                               buffer,    // ReportBuffer,
                               bufferSize // ReportBufferLength
                               );
    if (!bSuccess)
    {
        printf("failed HidD_GetInputReport\n");
    }
    else
    {
        printf("Received following data in input report: %d\n", 
           ((PHIDMINI_INPUT_REPORT) buffer)->Data); 
    }

    free(buffer);
    return bSuccess;
}


BOOLEAN
SetOutputReport(
    HANDLE file
    )
{
    ULONG bufferSize;
    PHIDMINI_OUTPUT_REPORT buffer;
    BOOLEAN bSuccess;

    //
    // Allocate memory 
    //
    bufferSize = sizeof(HIDMINI_OUTPUT_REPORT);
    buffer = (PHIDMINI_OUTPUT_REPORT) malloc (bufferSize);
    if (!buffer )
    {
        printf("malloc failed\n");
        return FALSE;
    }

    ZeroMemory(buffer, bufferSize);

    //
    // Fill the report
    //
    buffer->ReportId = CONTROL_COLLECTION_REPORT_ID;
    buffer->Data = (UCHAR) (rand() % UCHAR_MAX);

    //
    // Send Hid control code 
    //
    bSuccess = HidD_SetOutputReport(file,  // HidDeviceObject,
                               buffer,    // ReportBuffer,
                               bufferSize // ReportBufferLength
                               );
    if (!bSuccess)
    {
        printf("failed HidD_SetOutputReport\n");
    }
    else
    {
        printf("Set following data in output report: %d\n", 
           ((PHIDMINI_OUTPUT_REPORT) buffer)->Data); 
    }

    free(buffer);
    return bSuccess;
}

BOOLEAN
ReadInputData(
    _In_ HANDLE file
    )
{
    PHIDMINI_INPUT_REPORT report;
    ULONG bufferSize;
    BOOL bSuccess;
    DWORD bytesRead;

    //
    // Allocate memory 
    //
    bufferSize = sizeof(HIDMINI_INPUT_REPORT);  
    report = (PHIDMINI_INPUT_REPORT) malloc (bufferSize);
    if (!report )
    {
        printf("malloc failed\n");
        return FALSE;
    }

    ZeroMemory(report, bufferSize);

    report->ReportId = CONTROL_COLLECTION_REPORT_ID;

    //
    // get input data. 
    //
    bSuccess = ReadFile(
              file,        // HANDLE hFile,
              report,      // LPVOID lpBuffer,
              bufferSize,  // DWORD nNumberOfBytesToRead,
              &bytesRead,  // LPDWORD lpNumberOfBytesRead,
              NULL         // LPOVERLAPPED lpOverlapped
            );

    if (!bSuccess)
    {
        printf("failed ReadFile \n");
    }
    else
    {
        printf("Read following byte from device: %d\n", 
            report->Data);
    }

    free(report);

    return (BOOLEAN) bSuccess;
}

BOOLEAN
WriteOutputData(
    _In_ HANDLE file
    )
{
    PHIDMINI_OUTPUT_REPORT outputReport;
    ULONG outputReportSize;
    BOOL bSuccess;
    DWORD bytesWritten;

    //
    // Allocate memory for outtput report
    //
    outputReportSize = sizeof(HIDMINI_OUTPUT_REPORT);
    outputReport = (PHIDMINI_OUTPUT_REPORT) malloc (outputReportSize);
    if (!outputReport )
    {
        printf("malloc failed\n");
        return FALSE;
    }

    ZeroMemory(outputReport, outputReportSize);

    outputReport->ReportId = CONTROL_COLLECTION_REPORT_ID;
    outputReport->Data = (UCHAR) (rand() % UCHAR_MAX);

    //
    // Wrute output data. 
    //
    bSuccess = WriteFile(
              file,        // HANDLE hFile,
              (PVOID) outputReport,      // LPVOID lpBuffer,
              outputReportSize,  // DWORD nNumberOfBytesToRead,
              &bytesWritten,  // LPDWORD lpNumberOfBytesRead,
              NULL         // LPOVERLAPPED lpOverlapped
            );

    if (!bSuccess)
    {
        printf("failed WriteFile \n");
    }
    else
    {
        printf("Wrote following byte to device: %d\n", 
            outputReport->Data);
    }

    free(outputReport);

    return (BOOLEAN) bSuccess;
}

BOOLEAN
GetIndexedString(
    HANDLE File
    )
{
    BOOLEAN bSuccess;
    BYTE* buffer;
    ULONG bufferLength;

    bufferLength = MAXIMUM_STRING_LENGTH;
    buffer = malloc(bufferLength);
    if (!buffer )
    {
        printf("malloc failed\n");
        return FALSE;
    }

    ZeroMemory(buffer, bufferLength);

    bSuccess = HidD_GetIndexedString(
        File,
        VHIDMINI_DEVICE_STRING_INDEX,  // IN ULONG  StringIndex,
        (PVOID) buffer,  //OUT PVOID  Buffer,
        bufferLength // IN ULONG  BufferLength
        ) ;

    if (!bSuccess)
    {
        printf("failed WriteFile \n");
    }
    else
    {
        printf("Indexed string: %S\n", (PWSTR) buffer);
    }

    free(buffer);

    return bSuccess;
}

BOOLEAN
GetStrings(
    HANDLE File
    )
{
    BOOLEAN bSuccess;
    BYTE* buffer;
    ULONG bufferLength;

    bufferLength = MAXIMUM_STRING_LENGTH;
    buffer = malloc(bufferLength);
    if (!buffer )
    {
        printf("malloc failed\n");
        return FALSE;
    }

    ZeroMemory(buffer, bufferLength);
    bSuccess = HidD_GetProductString(
        File,
        (PVOID) buffer,  //OUT PVOID  Buffer,
        bufferLength // IN ULONG  BufferLength
        );

    if (!bSuccess)
    {
        printf("failed HidD_GetProductString \n");
        goto exit;
    }
    else
    {
        printf("Product string: %S\n", (PWSTR) buffer);
    }

    ZeroMemory(buffer, bufferLength);
    bSuccess = HidD_GetSerialNumberString(
        File,
        (PVOID) buffer,  //OUT PVOID  Buffer,
        bufferLength // IN ULONG  BufferLength
        );

    if (!bSuccess)
    {
        printf("failed HidD_GetSerialNumberString \n");
        goto exit;
    }
    else
    {
        printf("Serial number string: %S\n", (PWSTR) buffer);
    }

    ZeroMemory(buffer, bufferLength);
    bSuccess = HidD_GetManufacturerString(
        File,
        (PVOID) buffer,  //OUT PVOID  Buffer,
        bufferLength // IN ULONG  BufferLength
        );

    if (!bSuccess)
    {
        printf("failed HidD_GetManufacturerString \n");
        goto exit;
    }
    else
    {
        printf("Manufacturer string: %S\n", (PWSTR) buffer);
    }

exit:
    
    free(buffer);

    return bSuccess;
}    


