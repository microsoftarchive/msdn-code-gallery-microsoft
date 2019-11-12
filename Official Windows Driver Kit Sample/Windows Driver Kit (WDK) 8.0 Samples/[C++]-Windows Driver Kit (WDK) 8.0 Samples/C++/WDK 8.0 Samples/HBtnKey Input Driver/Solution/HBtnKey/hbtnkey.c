/*++
    Copyright (c) 2000,2002 Microsoft Corporation

    Module Name:
        HBtnKey.c

    Abstract:
        Contains OEM specific functions.

    Environment:
        Kernel mode

--*/

#include "pch.h"
#include <kbdmou.h>
#define MODULE_ID                       3
UCHAR gReportDescriptor[] = {
    //
    // Tablet PC Button Report
    //
    0x05, 0x01,                 // USAGE_PAGE (Generic Desktop)             0
    0x09, 0x09,                 // USAGE (TabletPC Buttons)                 2
    0xa1, 0x01,                 // COLLECTION (Application)                 4
    0x85, REPORTID_BTN,         //   REPORT_ID (Btn)                        6
    0x05, 0x09,                 //   USAGE_PAGE (Button)                    8
    0x19, 0x01,                 //   USAGE_MINIMUM (Button 1)               10
    0x29, 0x0c,                 //   USAGE_MAXIMUM (Button 12)              12
    0x15, 0x00,                 //   LOGICAL_MINIMUM (0)                    14
    0x25, 0x01,                 //   LOGICAL_MAXIMUM (1)                    16
    0x75, 0x01,                 //   REPORT_SIZE (1)                        18
    0x95, 0x0c,                 //   REPORT_COUNT (12)                      20
    0x81, 0x02,                 //   INPUT (Data,Var,Abs)                   22
    0x95, 0x14,                 //   REPORT_COUNT (20)                      24
    0x81, 0x03,                 //   INPUT (Cnst,Var,Abs)                   26
    0xc0,                       // END_COLLECTION                           28/29
    //
    // SAS Report
    //
    0x05, 0x01,                 // USAGE_PAGE (Generic Desktop)             0
    0x09, 0x06,                 // USAGE (Keyboard)                         2
    0xa1, 0x01,                 // COLLECTION (Application)                 4
    0x85, REPORTID_SAS,         //   REPORT_ID (SAS)                        6
    0x05, 0x07,                 //   USAGE_PAGE (Key Codes)                 8
    0x09, 0x4c,                 //   USAGE(Del Key)                         10
    0x09, 0xe0,                 //   USAGE(Left Ctrl Key)                   12
    0x09, 0xe2,                 //   USAGE(Left Alt Key)                    14
    0x15, 0x00,                 //   LOGICAL_MINIMUM (0)                    16
    0x25, 0x01,                 //   LOGICAL_MAXIMUM (1)                    18
    0x75, 0x01,                 //   REPORT_SIZE (1)                        20
    0x95, 0x03,                 //   REPORT_COUNT (3)                       22
    0x81, 0x02,                 //   INPUT (Data,Var,Abs)                   24
    0x95, 0x1d,                 //   REPORT_COUNT (29)                      26
    0x81, 0x03,                 //   INPUT (Cnst,Var,Abs)                   28
    0xc0,                       // END_COLLECTION                           30/31
#ifdef SIMULATE_KBD
    //
    // Keyboard Report
    //
    0x05, 0x01,                 // USAGE_PAGE (Generic Desktop)             0
    0x09, 0x06,                 // USAGE (Keyboard)                         2
    0xa1, 0x01,                 // COLLECTION (Application)                 4
    0x85, REPORTID_KBD,         //   REPORT_ID (Kbd)                        6
    0x05, 0x07,                 //   USAGE_PAGE (Key Codes)                 8
    0x75, 0x01,                 //   REPORT_SIZE (1)                        10
    0x95, 0x08,                 //   REPORT_COUNT (8)                       12
    0x15, 0x00,                 //   LOGICAL_MINIMUM (0)                    14
    0x25, 0x01,                 //   LOGICAL_MAXIMUM (1)                    16
    0x19, 0xe0,                 //   USAGE_MINIMUM (224)                    18
    0x29, 0xe7,                 //   USAGE_MAXIMUM (231)                    20
    0x81, 0x02,                 //   INPUT (Data,Var,Abs)                   22
    0x75, 0x08,                 //   REPORT_SIZE (8)                        24
    0x95, 0x06,                 //   REPORT_COUNT (6)                       26
    0x15, 0x00,                 //   LOGICAL_MINIMUM (0)                    28
    0x25, 0xff,                 //   LOGICAL_MAXIMUM (255)                  30
    0x19, 0x00,                 //   USAGE_MINIMUM (0)                      32
    0x29, 0xff,                 //   USAGE_MAXIMUM (255)                    34
    0x81, 0x00,                 //   INPUT (Data,Ary,Abs)                   36
    0x85, REPORTID_LED,         //   REPORT_ID (LED)                        38
    0x05, 0x08,                 //   USAGE_PAGE (LEDs)                      40
    0x75, 0x01,                 //   REPORT_SIZE (1)                        42
    0x95, 0x03,                 //   REPORT_COUNT (3)                       44
    0x09, 0x03,                 //   USAGE (Scroll Lock)                    46
    0x09, 0x01,                 //   USAGE (Num Lock)                       48
    0x09, 0x02,                 //   USAGE (Caps Lock)                      50
    0x91, 0x02,                 //   OUTPUT (Data,Var,Abs)                  52
    0x95, 0x05,                 //   REPORT_COUNT (5)                       54
    0x91, 0x03,                 //   OUTPUT (Cnst,Var,Abs)                  56
    0xc0                        // END_COLLECTION                           58/59
#endif
};
ULONG gdwcbReportDescriptor = sizeof(gReportDescriptor);

HID_DESCRIPTOR gHidDescriptor =
{
    sizeof(HID_DESCRIPTOR),             //bLength
    HID_HID_DESCRIPTOR_TYPE,            //bDescriptorType
    HID_REVISION,                       //bcdHID
    0,                                  //bCountry - not localized
    1,                                  //bNumDescriptors
    {                                   //DescriptorList[0]
        HID_REPORT_DESCRIPTOR_TYPE,     //bReportType
        sizeof(gReportDescriptor)       //wReportLength
    }
};

PWSTR gpwstrManufacturerID = L"Microsoft";
PWSTR gpwstrProductID = L"TabletPC Key Buttons";
PWSTR gpwstrSerialNumber = L"0";

//
// Local function prototypes.
//
NTSTATUS INTERNAL
SendSyncIoctl(
    _In_  PDEVICE_EXTENSION devext,
    _In_  ULONG             IoControlCode,
    _In_reads_bytes_opt_(dwcbOutputBuffer) PVOID InputBuffer,
    _In_  ULONG             dwcbInputBuffer,
    _Out_writes_bytes_opt_(dwcbOutputBuffer) PVOID OutputBuffer,
    _In_  ULONG             dwcbOutputBuffer
    );

NTSTATUS INTERNAL
EnableKbdPort(
    _In_ PDEVICE_EXTENSION devext,
    _In_ PIRP              irp,
    _In_ BOOLEAN           fEnable
    );

VOID EXTERNAL
KbdPortCallback(
    _In_  PDEVICE_OBJECT       devobj,
    _In_  PKEYBOARD_INPUT_DATA InputDataStart,
    _In_  PKEYBOARD_INPUT_DATA InputDataEnd,
    _Out_ PULONG               pdwcConsumed
    );

_Success_(return)
BOOLEAN INTERNAL
SendButtonReport(
    _In_  PDEVICE_EXTENSION devext,
    _In_  USHORT            wMakeCode,
    _In_  USHORT            wFlags,
    _Out_ PHID_INPUT_REPORT Report
    );

#ifdef SIMULATE_KBD
_Check_return_
BOOLEAN INTERNAL
SendKbdReport(
    _In_  PDEVICE_EXTENSION devext,
    _In_  USHORT            wMakeCode,
    _In_  USHORT            wFlags,
    _Out_ PHID_INPUT_REPORT Report
    );
#endif

#ifdef ALLOC_PRAGMA
  #pragma alloc_text(PAGE, OemAddDevice)
  #pragma alloc_text(PAGE, OemStartDevice)
  #pragma alloc_text(PAGE, OemRemoveDevice)
  #pragma alloc_text(PAGE, SendSyncIoctl)
  #pragma alloc_text(PAGE, EnableKbdPort)
#endif  //ifdef ALLOC_PRAGMA

/*++
    @doc    INTERNAL

    @func   NTSTATUS | OemAddDevice | OEM specific AddDevice code.

    @parm   IN PDEVICE_EXTENSION | devext | Points to the device extension.

    @rvalue SUCCESS | Returns STATUS_SUCCESS.
    @rvalue FAILURE | Returns NT status code.
--*/

NTSTATUS INTERNAL
OemAddDevice(
    _In_ PDEVICE_EXTENSION devext
    )
{
    NTSTATUS        status = STATUS_SUCCESS;
    CONNECT_DATA    ConnectData = {0};

    PAGED_CODE ();
    TEnter(Func,("(devext=%p)\n", devext));

    ConnectData.ClassDeviceObject = devext->self;
#pragma warning(disable:4152)  //nonstandard extension, function/data pointer conversion
    ConnectData.ClassService = KbdPortCallback;
#pragma warning(default:4152)
    status = SendSyncIoctl(devext,
                           IOCTL_INTERNAL_KEYBOARD_CONNECT,
                           &ConnectData,
                           sizeof(ConnectData),
                           NULL,
                           0);
    if (NT_SUCCESS(status))
    {
        devext->dwfHBtn |= HBTNF_KBD_CONNECTED;
    }

    TExit(Func,("=%x\n", status));
    return status;
}       //OemAddDevice

/*++
    @doc    INTERNAL

    @func   NTSTATUS | OemStartDevice |
            Get the device information and attempt to initialize a
            configuration for a device.  If we cannot identify this as a
            valid HID device or configure the device, our start device
            function is failed.

    @parm   IN PDEVICE_EXTENSION | devext | Points to the device extension.
    @parm   IN PIRP | Irp | Points to an I/O request packet.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
OemStartDevice(
    _In_ PDEVICE_EXTENSION devext,
    _In_ PIRP              Irp
    )
{
    NTSTATUS status = STATUS_SUCCESS;

    PAGED_CODE();
    TEnter(Func,("(devext=%p,Irp=%p)\n", devext, Irp));

    status = EnableKbdPort(devext, Irp, TRUE);

    TExit(Func,("=%x\n", status));
    return status;
}       //OemStartDevice

/*++
    @doc    INTERNAL

    @func   VOID | OemRemoveDevice | FDO Remove routine

    @parm   IN PDEVICE_EXTENSION | devext | Points to the device extension.
    @parm   IN PIRP | Irp | Points to an I/O request packet.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
OemRemoveDevice(
    _In_ PDEVICE_EXTENSION devext,
    _In_ PIRP              Irp
    )
{
    NTSTATUS status = STATUS_SUCCESS;

    PAGED_CODE();
    TEnter(Func,("(devext=%p)\n", devext));

    status = EnableKbdPort(devext, Irp, FALSE);

    TExit(Func,("=%x\n", status));
    return status;
}       //OemRemoveDevice

/*++
    @doc    INTERNAL

    @func   NTSTATUS | SendSyncIoctl | Send a synchronous internal ioctl
            to the keyboard port driver.

    @parm   IN PDEVICE_EXTENSION | devext | Points to the device extension.
    @parm   IN PVOID | InputBuffer | Optional input buffer.
    @parm   IN ULONG | dwcbInputBuffer | Input buffer length.
    @parm   OUT PVOID | OutputBuffer | Optional output buffer.
    @parm   IN ULONG | dwcbOutputBuffer | Output buffer length.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
SendSyncIoctl(
    _In_  PDEVICE_EXTENSION devext,
    _In_  ULONG             IoControlCode,
    _In_reads_bytes_opt_(dwcbOutputBuffer) PVOID InputBuffer,
    _In_  ULONG             dwcbInputBuffer,
    _Out_writes_bytes_opt_(dwcbOutputBuffer) PVOID OutputBuffer,
    _In_  ULONG             dwcbOutputBuffer
    )
{
    NTSTATUS        status = STATUS_SUCCESS;
    PIRP            irp = NULL;
    KEVENT          event = {0};
    IO_STATUS_BLOCK iosb = {0};

    PAGED_CODE();
    TEnter(Func,("(devext=%p,IoControlCode=%x,InputBuffer=%p,InLen=%d,OutputBuffer=%p,OutLen=%d)\n",
                devext, IoControlCode, InputBuffer, dwcbInputBuffer,
                OutputBuffer, dwcbOutputBuffer));

    KeInitializeEvent(&event, NotificationEvent, FALSE);
    irp = IoBuildDeviceIoControlRequest(IoControlCode,
                                        devext->LowerDevObj,
                                        InputBuffer,
                                        dwcbInputBuffer,
                                        OutputBuffer,
                                        dwcbOutputBuffer,
                                        TRUE,
                                        &event,
                                        &iosb);
    if (irp != NULL)
    {
        status = IoCallDriver(devext->LowerDevObj, irp);
        if (status == STATUS_PENDING)
        {
            status = KeWaitForSingleObject(&event,
                                           Executive,
                                           KernelMode,
                                           FALSE,
                                           NULL);
        }

        if (status == STATUS_SUCCESS)
        {
            status = iosb.Status;
        }
    }
    else
    {
        status = STATUS_INSUFFICIENT_RESOURCES;
        LogError(ERRLOG_INSUFFICIENT_RESOURCES,
                 status,
                 UNIQUE_ERRID(0x10),
                 NULL,
                 NULL);
        TErr(("Failed to build ioctl IRP.\n"));
    }

    TExit(Func,("=%x\n", status));
    return status;
}       //SendSyncIoctl

/*++
    @doc    INTERNAL

    @func   NTSTATUS | EnableKbdPort | Enable or disable keyboard port.

    @parm   IN PDEVICE_EXTENSION | devext | Points to the device extension.
    @parm   IN PIRP | irp | Points to an I/O request packet.
    @parm   IN BOOLEAN | fEnable | TRUE if enable, otherwise disable.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
EnableKbdPort(
    _In_ PDEVICE_EXTENSION devext,
    _In_ PIRP              irp,
    _In_ BOOLEAN           fEnable
    )
{
    NTSTATUS            status = STATUS_SUCCESS;
    PIO_STACK_LOCATION  irpspNext = IoGetNextIrpStackLocation(irp);
    NTSTATUS            PrevStatus = irp->IoStatus.Status;
    ULONG_PTR           PrevInfo = irp->IoStatus.Information;

    PAGED_CODE();
    TEnter(Func,("(devext=%p,irp=%p,fEnable=%x)\n", devext, irp, fEnable));

    RtlZeroMemory(irpspNext, sizeof(*irpspNext));
    irpspNext->MajorFunction = fEnable? IRP_MJ_CREATE: IRP_MJ_CLOSE;
    status = SendSyncIrp(devext->LowerDevObj, irp, FALSE);
    if (NT_SUCCESS(status))
    {
        irp->IoStatus.Status = PrevStatus;
        irp->IoStatus.Information = PrevInfo;
    }
    else
    {
        LogError(ERRLOG_OPEN_KBDPORT_FAILED,
                 status,
                 UNIQUE_ERRID(0x20),
                 NULL,
                 NULL);
        TErr(("Failed to send Create IRP to keyboard port driver (status=%x).\n",
                    status));
    }

    TExit(Func,("=%x\n", status));
    return status;
}       //EnableKbdPort

/*++
    @doc    EXTERNAL

    @func   VOID | KbdPortCallback | Callback from keyboard port driver.

    @parm   IN PDEVICE_OBJECT | DevObj | Points to my pdo.
    @parm   IN PKEYBOARD_INPUT_DATA | InputDataStart | Points to the start
            of the input data queue.
    @parm   IN PKEYBOARD_INPUT_DATA | InputDataEnd | Points to the end
            of the input data queue.
    @parm   OUT PULONG | pdwcConsumed | To hold the number of input data
            consumed by this call.

    @rvalue None.

    @note   This routine is called at DISPATCH_LEVEL.
--*/

VOID EXTERNAL
KbdPortCallback(
    _In_  PDEVICE_OBJECT       DevObj,
    _In_  PKEYBOARD_INPUT_DATA InputDataStart,
    _In_  PKEYBOARD_INPUT_DATA InputDataEnd,
    _Out_ PULONG               pdwcConsumed
    )
{
    PDEVICE_EXTENSION   devext = GET_MINIDRIVER_DEVICE_EXTENSION(DevObj);
    ULONG               dwcDataInQueue = (ULONG)(((PCHAR)InputDataEnd-(PCHAR)InputDataStart)/
                                   sizeof(KEYBOARD_INPUT_DATA));
    PLIST_ENTRY         plist = NULL;
    PIRP                PendingIrp = NULL;
    LIST_ENTRY          listHead = {0};

    TEnter(Func,("(devobj=%p,InputStart=%p,InputEnd=%p,pdwcConsumed=%p,cConsumed=%d)\n",
                DevObj, InputDataStart, InputDataEnd, pdwcConsumed,
                *pdwcConsumed));

    *pdwcConsumed = 0;
    InitializeListHead(&listHead);
    while (dwcDataInQueue > 0)
    {
        
        PendingIrp = IoCsqRemoveNextIrp(&devext->IrpQueue, NULL);
        if(NULL !=  PendingIrp)
        {
            if (SendButtonReport(devext,
                                 InputDataStart->MakeCode,
                                 InputDataStart->Flags,
                                 (PHID_INPUT_REPORT)PendingIrp->UserBuffer))

            {
                
                PendingIrp->IoStatus.Information = sizeof(HID_INPUT_REPORT);
                PendingIrp->IoStatus.Status = STATUS_SUCCESS;
                InsertTailList(&listHead, &PendingIrp->Tail.Overlay.ListEntry);
            }
            else
            {
                //
                // We are not sending this one, so put the irp back in queue.
                //
                IoCsqInsertIrp(&devext->IrpQueue,
                    PendingIrp,
                    NULL);
            }

            InputDataStart++;
            dwcDataInQueue--;
            (*pdwcConsumed)++;
        }
        else
        {
            TWarn(("No pending ReadReport irp.\n"));
            break;
        }
    }   

    while (!IsListEmpty(&listHead))
    {
        plist = RemoveHeadList(&listHead);
        PendingIrp = CONTAINING_RECORD(plist, IRP, Tail.Overlay.ListEntry);
        IoCompleteRequest(PendingIrp, IO_NO_INCREMENT);
        IoReleaseRemoveLock(&devext->RemoveLock, PendingIrp);
    }

    TExit(Func,("=! (cConsumed=%d)\n", *pdwcConsumed));
    return;
}       //KbdPortCallback

/*++
    @doc    INTERNAL

    @func   BOOLEAN | SendButtonReport | Send a button report.

    @parm   IN PDEVICE_EXTENSION | DevExt | Points to the device extension.
    @parm   IN USHORT | wMakeCode | Specifies the keyboard make code.
    @parm   IN USHORT | wFlags | Specifies whether the make code is a make
            or break event.
    @parm   OUT PHID_INPUT_REPORT | Report | Points to the input report.

    @rvalue SUCCESS | Returns TRUE - has sent a report.
    @rvalue FAILURE | Returns FALSE - has not sent a report.

    @note   This routine is always called at DISPATCH_LEVEL.
--*/

_Success_(return)
BOOLEAN INTERNAL
SendButtonReport(
    _In_  PDEVICE_EXTENSION devext,
    _In_  USHORT            wMakeCode,
    _In_  USHORT            wFlags,
    _Out_ PHID_INPUT_REPORT Report
    )
{
    BOOLEAN fSendReport = FALSE;
    #define MAKECODE_SASBUTTON  0x58

    TEnter(Func,("(devext=%p,MakeCode=%x,Flags=%x,Report=%p)\n",
                devext, wMakeCode, wFlags, Report));

    KeAcquireSpinLockAtDpcLevel(&devext->DataLock);

    if (wMakeCode == MAKECODE_SASBUTTON)
    {
        Report->ReportID = REPORTID_SAS;
        Report->SASReport.bKeys = (wFlags & KEY_BREAK)? 0: SASF_CAD;
        fSendReport = TRUE;
        TInfo(("SAS %s Event\n", (wFlags & KEY_BREAK)? "Break": "Make"));
    }
    else
    {
        static struct {
            USHORT wMakeCode;
            ULONG  dwButtonID;
        } ButtonTable[] =
        {
            0x3b,   0x00000001,     //F1
            0x3c,   0x00000002,     //F2
            0x3d,   0x00000004,     //F3
            0x3e,   0x00000008,     //F4
            0x3f,   0x00000010,     //F5
            0x40,   0x00000020,     //F6
            0x41,   0x00000040,     //F7
            0x42,   0x00000080,     //F8
            0x43,   0x00000100,     //F9
            0x44,   0x00000200,     //F10
            0x57,   0x00000400,     //F11
            0x58,   0x00000800,     //F12
            0,      0,
        };
        ULONG i;        

        for (i = 0; ButtonTable[i].dwButtonID != 0; ++i)
        {
            if (wMakeCode == ButtonTable[i].wMakeCode)
            {
                if (wFlags & KEY_BREAK)
                {
                    devext->OemExtension.dwLastButtons &=
                        ~ButtonTable[i].dwButtonID;
                }
                else
                {
                    devext->OemExtension.dwLastButtons |=
                        ButtonTable[i].dwButtonID;
                }
                Report->ReportID = REPORTID_BTN;
                Report->BtnReport.dwButtonStates =
                    devext->OemExtension.dwLastButtons;
                fSendReport = TRUE;
                TInfo( ("Button Event (Buttons=%x)\n",
                              Report->BtnReport.dwButtonStates));
                break;
            }
        }

#ifdef SIMULATE_KBD
        if (!fSendReport)
        {
            fSendReport = SendKbdReport(devext, wMakeCode, wFlags, Report);
        }
#endif
    }
    KeReleaseSpinLockFromDpcLevel(&devext->DataLock);
    TExit(Func,("=%x\n", fSendReport));
    return fSendReport;
}       //SendButtonReport

#ifdef SIMULATE_KBD
/*++
    @doc    INTERNAL

    @func   BOOLEAN | SendKbdReport | Send a keyboard report.

    @parm   IN PDEVICE_EXTENSION | DevExt | Points to the device extension.
    @parm   IN USHORT | wMakeCode | Specifies the keyboard make code.
    @parm   IN USHORT | wFlags | Specifies whether the make code is a make
            or break event.
    @parm   OUT PHID_INPUT_REPORT | Report | Points to the input report.

    @rvalue SUCCESS | Returns TRUE - has sent a report.
    @rvalue FAILURE | Returns FALSE - has not sent a report.

    @note   This routine is always called at DISPATCH_LEVEL.
--*/

_Check_return_
BOOLEAN INTERNAL
SendKbdReport(
    _In_  PDEVICE_EXTENSION devext,
    _In_  USHORT            wMakeCode,
    _In_  USHORT            wFlags,
    _Out_ PHID_INPUT_REPORT Report
    )
{
    BOOLEAN fSendReport = FALSE;
    typedef struct _USAGEMAP
    {
        UCHAR bCode;
        UCHAR bUsage;
    } USAGEMAP, *PUSAGEMAP;
    static USAGEMAP E0CodeTable[] =
    {
        0x52, 0x49,     //Insert
        0x47, 0x4a,     //Home
        0x49, 0x4b,     //PageUp
        0x53, 0x4c,     //Delete
        0x4f, 0x4d,     //End
        0x51, 0x4e,     //PageDown
        0x4d, 0x4f,     //Right
        0x4b, 0x50,     //Left
        0x50, 0x51,     //Down
        0x48, 0x52,     //Up
        0x35, 0x54,     //KeyPad '/'
        0x1c, 0x58,     //KeyPad Enter
        0x5d, 0x65,     //Application
        0x5b, 0xe3,     //Left GUI
        0x1d, 0xe4,     //Right Ctrl
        0x38, 0xe6,     //Right Alt
        0x5c, 0xe7,     //Right GUI
        0x00, 0x00
    };
    static UCHAR UsageTable[] =
    {
//         0     1     2     3     4     5     6     7
//         8     9     a     b     c     d     e     f
/*0x00*/0x00, 0x29, 0x1e, 0x1f, 0x20, 0x21, 0x22, 0x23,
/*0x08*/0x24, 0x25, 0x26, 0x27, 0x2d, 0x2e, 0x2a, 0x2b,
/*0x10*/0x14, 0x1a, 0x08, 0x15, 0x17, 0x1c, 0x18, 0x0c,
/*0x18*/0x12, 0x13, 0x2f, 0x30, 0x28, 0xe0, 0x04, 0x16,
/*0x20*/0x07, 0x09, 0x0a, 0x0b, 0x0d, 0x0e, 0x0f, 0x33,
/*0x28*/0x34, 0x35, 0xe1, 0x31, 0x1d, 0x1b, 0x06, 0x19,
/*0x30*/0x05, 0x11, 0x10, 0x36, 0x37, 0x38, 0xe5, 0x55,
/*0x38*/0xe2, 0x2c, 0x39, 0x3a, 0x3b, 0x3c, 0x3d, 0x3e,
/*0x40*/0x3f, 0x40, 0x41, 0x42, 0x43, 0x53, 0x47, 0x5f,
/*0x48*/0x60, 0x61, 0x56, 0x5c, 0x5d, 0x5e, 0x57, 0x59,
/*0x50*/0x5a, 0x5b, 0x62, 0x63, 0x00, 0x00, 0x00, 0x44,
/*0x58*/0x45
    };
    
    ULONG i = 0;
    UCHAR bUsage = 0x00;

    TEnter(Func,("(devext=%p,MakeCode=%x,Flags=%x,Report=%p)\n",
                devext, wMakeCode, wFlags, Report));

    TInfo(("MakeCode=%x,Flags=%x\n", wMakeCode, wFlags));
    if (wFlags & KEY_E1)
    {
        TAssert(wMakeCode == 0x1d);
        devext->dwfHBtn |= HBTNF_KBD_E1;
        TInfo( ("Kbd E1\n"));
    }
    else if (wFlags & KEY_E0)
    {
        if (!(devext->dwfHBtn & HBTNF_KBD_E0) &&
            ((wMakeCode == 0x2a) || (wMakeCode == 0x37)))
        {
            devext->dwfHBtn |= HBTNF_KBD_E0;
            TInfo( ("Kbd E0\n"));
        }
        else if ((devext->dwfHBtn & HBTNF_KBD_E0) &&
                 ((wMakeCode == 0x2a) || (wMakeCode == 0x37)))
        {
            devext->dwfHBtn &= ~HBTNF_KBD_E0;
            bUsage = 0x46;
            fSendReport = TRUE;
            TInfo( ("Kbd PrintScreen\n"));
        }
        else
        {
            
            for (i = 0; E0CodeTable[i].bCode != 0; ++i)
            {
                if (wMakeCode == E0CodeTable[i].bCode)
                {
                    bUsage = E0CodeTable[i].bUsage;
                    fSendReport = TRUE;
                    break;
                }
            }

            if (!fSendReport)
            {
                devext->dwfHBtn &= ~HBTNF_KBD_E0;
                TWarn(("Failed to find %x in E0CodeTable!\n",
                           wMakeCode));
            }
        }
    }
    else if ((devext->dwfHBtn & HBTNF_KBD_E1) && (wMakeCode == 0x45))
    {
        devext->dwfHBtn &= ~HBTNF_KBD_E1;
        bUsage = 0x48;
        fSendReport = TRUE;
        TInfo( ("Kbd Pause\n"));
    }
    else
    {
        devext->dwfHBtn &= ~HBTNF_KBD_E1;
        if (wMakeCode < ARRAYSIZE(UsageTable))
        {
            bUsage = UsageTable[wMakeCode];
            if (bUsage != 0)
            {
                fSendReport = TRUE;
            }
        }

        if (!fSendReport)
        {
            TWarn(("Failed to find %x in MakeCodeTable!\n",
                       wMakeCode));
        }
    }

    if (fSendReport)
    {
        if ((bUsage >= 0xe0) && (bUsage <= 0xe7))
        {
            UCHAR bMask;

            bMask = 1 << (bUsage - 0xe0);
            if (wFlags & KEY_BREAK)
            {
                devext->OemExtension.KbdReport.bModifiers &= ~bMask;
            }
            else if (devext->OemExtension.KbdReport.bModifiers & bMask)
            {
                //
                // This is a repeat, ignore it.
                //
                fSendReport = FALSE;
            }
            else
            {
                devext->OemExtension.KbdReport.bModifiers |= bMask;
            }
        }
        else
        {
            fSendReport = FALSE;
            if (wFlags & KEY_BREAK)
            {
                for (i = 0;
                     i < ARRAYSIZE(devext->OemExtension.KbdReport.abKeys);
                     ++i)
                {
                    if (bUsage == devext->OemExtension.KbdReport.abKeys[i])
                    {
                        devext->OemExtension.KbdReport.abKeys[i] = 0;
                        fSendReport = TRUE;
                        break;
                    }
                }

                if (!fSendReport)
                {
                    TWarn(("Failed to find make slot for %x!\n", bUsage));
                }
            }
            else
            {
                BOOLEAN fRepeat = FALSE;

                for (i = 0;
                     i < ARRAYSIZE(devext->OemExtension.KbdReport.abKeys);
                     ++i)
                {
                    if (bUsage == devext->OemExtension.KbdReport.abKeys[i])
                    {
                        //
                        // This is a repeat make, ignore it.
                        //
                        fRepeat = TRUE;
                        break;
                    }
                }

                if (!fRepeat)
                {
                    //
                    // Find an empty slot to put it in.
                    //
                    for (i = 0;
                         i < ARRAYSIZE(devext->OemExtension.KbdReport.abKeys);
                         ++i)
                    {
                        if (devext->OemExtension.KbdReport.abKeys[i] == 0)
                        {
                            devext->OemExtension.KbdReport.abKeys[i] = bUsage;
                            fSendReport = TRUE;
                            break;
                        }
                    }
                }

                if (!fRepeat && !fSendReport)
                {
                    TWarn(("Failed to find empty slot for %x!\n", bUsage));
                }
            }
        }

        if (fSendReport)
        {
            Report->ReportID = REPORTID_KBD;
            Report->KbdReport = devext->OemExtension.KbdReport;
            TInfo( ("Kbd Event (Usage=%x)\n", bUsage));
        }
    }

    TExit(Func,("=%x\n", fSendReport));
    return fSendReport;
}       //SendKbdReport

/*++
    @doc    INTERNAL

    @func   NTSTATUS | OemWriteReport | Write output report.

    @parm   IN PDEVICE_EXTENSION | devext | Points to the device extension.
    @parm   IN PIRP | Irp | Points to an I/O Request Packet.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
OemWriteReport(
    _In_ PDEVICE_EXTENSION devext,
    _In_ PIRP              Irp
    )
{
    NTSTATUS            status = STATUS_SUCCESS;
    PIO_STACK_LOCATION  irpsp = IoGetCurrentIrpStackLocation(Irp);
    PHID_XFER_PACKET    XferPacket = (PHID_XFER_PACKET)Irp->UserBuffer;

    TEnter(Func,("(devext=%p,Irp=%p,IrpSp=%p)\n", devext, Irp, irpsp));

    if (irpsp->Parameters.DeviceIoControl.InputBufferLength !=
        sizeof(HID_XFER_PACKET))
    {
        status = STATUS_INVALID_BUFFER_SIZE;
        LogError(ERRLOG_INVALID_BUFFER_SIZE,
                 status,
                 UNIQUE_ERRID(0x80),
                 NULL,
                 NULL);
        TWarn(("invalid transfer packet size (len=%d).\n",
                   irpsp->Parameters.DeviceIoControl.InputBufferLength));
    }
    else if (XferPacket->reportBufferLen != sizeof(HID_OUTPUT_REPORT))
    {
        status = STATUS_INVALID_BUFFER_SIZE;
        LogError(ERRLOG_INVALID_BUFFER_SIZE,
                 status,
                 UNIQUE_ERRID(0x90),
                 NULL,
                 NULL);
        TWarn(("invalid output report size (len=%d).\n",
                   XferPacket->reportBufferLen));
    }
    else if (!(devext->dwfHBtn & HBTNF_DEVICE_STARTED))
    {
        status = STATUS_DEVICE_NOT_READY ;
        LogError(ERRLOG_DEVICE_NOT_STARTED,
                 status,
                 UNIQUE_ERRID(0xa0),
                 NULL,
                 NULL);
        TWarn(("digitizer is not started.\n"));
    }
    else if (XferPacket->reportId != REPORTID_LED)
    {
        status = STATUS_INVALID_PARAMETER;
        LogError(ERRLOG_INVALID_PARAMETER,
                 status,
                 UNIQUE_ERRID(0xb0),
                 NULL,
                 NULL);
        TWarn(("invalid output report ID (ReportID=%d).\n",
                   XferPacket->reportId));
    }
    else
    {
        KEYBOARD_INDICATOR_PARAMETERS LEDStates;

        LEDStates.UnitId = 0;
        LEDStates.LedFlags = ((PHID_OUTPUT_REPORT)XferPacket->reportBuffer)->
                                LEDReport.bLEDStates;
        status = SendSyncIoctl(devext,
                               IOCTL_KEYBOARD_SET_INDICATORS,
                               &LEDStates,
                               sizeof(LEDStates),
                               NULL,
                               0);
    }

    TExit(Func,("=%x\n", status));
    return status;
}       //OemWriteReport
#else
/*++
    @doc    INTERNAL

    @func   NTSTATUS | OemWriteReport | Write output report.

    @parm   IN PDEVICE_EXTENSION | devext | Points to the device extension.
    @parm   IN PIRP | Irp | Points to an I/O Request Packet.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
OemWriteReport(
    _In_ PDEVICE_EXTENSION devext,
    _In_ PIRP              Irp
    )
{
    NTSTATUS status = STATUS_SUCCESS;

    UNREFERENCED_PARAMETER(Irp);
    UNREFERENCED_PARAMETER(devext);

    TEnter(Func,("(devext=%p,Irp=%p)\n", devext, Irp));


    status = STATUS_NOT_SUPPORTED;
    TWarn(("unsupported WriteReport.\n"));

    TExit(Func,("=%x\n", status));
    return status;
}       //OemWriteReport
#endif

