/*++
    Copyright (c) 2000,2001 Microsoft Corporation

    Module Name:
        ioctl.c

    Abstract:
        Contains routines to support HIDCLASS internal ioctl queries
        for the pen tablet devices.

    Environment:
        Kernel mode

--*/

#include "pch.h"

#define MODULE_ID                       2

#ifdef ALLOC_PRAGMA
  #pragma alloc_text(PAGE, GetDeviceDescriptor)
  #pragma alloc_text(PAGE, GetReportDescriptor)
  #pragma alloc_text(PAGE, GetString)
  #pragma alloc_text(PAGE, GetAttributes)
#endif

/*++
    @doc    EXTERNAL

    @func   NTSTATUS | HbtnInternalIoctl |
            Process the Control IRPs sent to this device.
            <nl>This function cannot be pageable because reads/writes
            can be made at dispatch-level

    @parm   IN PDRIVER_OBJECT | DevObj | Points to the driver object.
    @parm   IN PIRP | Irp | Points to an I/O Request Packet.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS EXTERNAL
HbtnInternalIoctl(
    IN PDEVICE_OBJECT DevObj,
    IN PIRP           Irp
    )
{
    NTSTATUS            status = STATUS_SUCCESS;
    PDEVICE_EXTENSION   devext = GET_MINIDRIVER_DEVICE_EXTENSION(DevObj);
    PIO_STACK_LOCATION  irpsp = IoGetCurrentIrpStackLocation(Irp);

    TEnter(Func,("(DevObj=%p,Irp=%p,IrpSp=%p,Ioctl=%s)\n",
                DevObj, Irp, irpsp,
                LookupName(irpsp->Parameters.DeviceIoControl.IoControlCode,
                           HidIoctlNames)));

    Irp->IoStatus.Information = 0;
    status = IoAcquireRemoveLock(&devext->RemoveLock, Irp);
    if (!NT_SUCCESS(status))
    {
        LogError(ERRLOG_DEVICE_REMOVED,
                 status,
                 UNIQUE_ERRID(0x10),
                 NULL,
                 NULL);
        TWarn(("received IRP after device was removed.\n"));
        Irp->IoStatus.Status = status;
        IoCompleteRequest(Irp, IO_NO_INCREMENT);
    }
    else if (!(devext->dwfHBtn & HBTNF_DEVICE_STARTED))
    {
        IoReleaseRemoveLock(&devext->RemoveLock, Irp);
        status = STATUS_DEVICE_NOT_READY;
        LogError(ERRLOG_DEVICE_NOT_STARTED,
                 status,
                 UNIQUE_ERRID(0x20),
                 NULL,
                 NULL);
        TWarn(("digitizer is not started.\n"));
        Irp->IoStatus.Status = status;
        IoCompleteRequest(Irp, IO_NO_INCREMENT);
    }
    else
    {
        switch(irpsp->Parameters.DeviceIoControl.IoControlCode)
        {
            case IOCTL_HID_GET_DEVICE_DESCRIPTOR:
                status = GetDeviceDescriptor(devext, Irp);
                break;

            case IOCTL_HID_GET_REPORT_DESCRIPTOR:
                status = GetReportDescriptor(devext, Irp);
                break;

            case IOCTL_HID_READ_REPORT:
                status = ReadReport(devext, Irp);
                break;

            case IOCTL_HID_WRITE_REPORT:
                status = OemWriteReport(devext, Irp);
                break;

            case IOCTL_HID_GET_STRING:
                status = GetString(devext, Irp);
                break;

            case IOCTL_HID_GET_DEVICE_ATTRIBUTES:
                status = GetAttributes(devext, Irp);
                break;

            case IOCTL_HID_ACTIVATE_DEVICE:
            case IOCTL_HID_DEACTIVATE_DEVICE:
                status = STATUS_SUCCESS;
                break;

            default:
                status = STATUS_NOT_SUPPORTED;
                LogError(ERRLOG_NOT_SUPPORTED,
                         status,
                         UNIQUE_ERRID(0x30),
                         NULL,
                         NULL);
                TWarn(("unsupported (IOCTL=%x).\n",
                           irpsp->Parameters.DeviceIoControl.IoControlCode));
                break;
        }

        if (status != STATUS_PENDING)
        {
            IoReleaseRemoveLock(&devext->RemoveLock, Irp);
            Irp->IoStatus.Status = status;
            IoCompleteRequest(Irp, IO_NO_INCREMENT);
        }
        else
        {
            IoMarkIrpPending(Irp);
        }
    }

    TExit(Func,("=%x\n", status));
    return status;
}       //HbtnInternalIoctl

/*++
    @doc    INTERNAL

    @func   NTSTATUS | GetDeviceDescriptor |
            Respond to HIDCLASS IOCTL_HID_GET_DEVICE_DESCRIPTOR
            by returning a device descriptor.

    @parm   IN PDEVICE_EXTENSION | DevExt | Points to the device extension.
    @parm   IN PIRP | Irp | Points to an I/O Request Packet.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
GetDeviceDescriptor(
    _In_ PDEVICE_EXTENSION DevExt,
    _In_ PIRP              Irp
    )
{
    NTSTATUS            status = STATUS_SUCCESS;
    PIO_STACK_LOCATION  irpsp = NULL;

    UNREFERENCED_PARAMETER(DevExt);
    PAGED_CODE ();

    irpsp = IoGetCurrentIrpStackLocation(Irp);

    TEnter(Func,("(DevExt=%p,Irp=%p,IrpSp=%p)\n", DevExt, Irp, irpsp));

    if (irpsp->Parameters.DeviceIoControl.OutputBufferLength <
        sizeof(gHidDescriptor))
    {
        status = STATUS_BUFFER_TOO_SMALL;
        LogError(ERRLOG_BUFFER_TOO_SMALL,
                 status,
                 UNIQUE_ERRID(0x40),
                 NULL,
                 NULL);
        TWarn(("output buffer too small (len=%d).\n",
                   irpsp->Parameters.DeviceIoControl.OutputBufferLength));
    }
    else
    {
        RtlCopyMemory(Irp->UserBuffer,
                      &gHidDescriptor,
                      sizeof(gHidDescriptor));

        Irp->IoStatus.Information = sizeof(gHidDescriptor);
        status = STATUS_SUCCESS;
    }

    TExit(Func,("=%x\n", status));
    return status;
}       //GetDeviceDescriptor

/*++
    @doc    INTERNAL

    @func   NTSTATUS | GetReportDescriptor |
            Respond to HIDCLASS IOCTL_HID_GET_REPORT_DESCRIPTOR
            by returning appropriate the report descriptor.

    @parm   IN PDEVICE_EXTENSION | DevExt | Points to the device extension.
    @parm   IN PIRP | Irp | Points to an I/O Request Packet.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
GetReportDescriptor(
    _In_ PDEVICE_EXTENSION DevExt,
    _In_ PIRP              Irp
    )
{
    NTSTATUS            status = STATUS_SUCCESS;
    PIO_STACK_LOCATION  irpsp = NULL;

    UNREFERENCED_PARAMETER(DevExt);
    PAGED_CODE ();

    irpsp = IoGetCurrentIrpStackLocation(Irp);

    TEnter(Func,("(DevExt=%p,Irp=%p,IrpSp=%p\n", DevExt, Irp, irpsp));

    if (irpsp->Parameters.DeviceIoControl.OutputBufferLength <
        gdwcbReportDescriptor)
    {
        status = STATUS_BUFFER_TOO_SMALL;
        LogError(ERRLOG_BUFFER_TOO_SMALL,
                 status,
                 UNIQUE_ERRID(0x50),
                 NULL,
                 NULL);
        TWarn(("output buffer too small (len=%d).\n",
                   irpsp->Parameters.DeviceIoControl.OutputBufferLength));
    }
    else
    {
        RtlCopyMemory(Irp->UserBuffer,
                      gReportDescriptor,
                      gdwcbReportDescriptor);

        Irp->IoStatus.Information = gdwcbReportDescriptor;
        status = STATUS_SUCCESS;
    }

    TExit(Func,("=%x\n", status));
    return status;
}       //GetReportDescriptor

/*++
    @doc    INTERNAL

    @func   NTSTATUS | ReadReport | Read input report.

    @parm   IN PDEVICE_EXTENSION | DevExt | Points to the device extension.
    @parm   IN PIRP | Irp | Points to an I/O Request Packet.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
ReadReport(
    _In_ PDEVICE_EXTENSION DevExt,
    _In_ PIRP              Irp
    )
{
    NTSTATUS            status = STATUS_SUCCESS;
    PIO_STACK_LOCATION  irpsp = NULL;

    irpsp = IoGetCurrentIrpStackLocation(Irp);

    TEnter(Func,("(DevExt=%p,Irp=%p,IrpSp=%p)\n", DevExt, Irp, irpsp));
    TAssert(Irp->UserBuffer != NULL);

    if (irpsp->Parameters.DeviceIoControl.OutputBufferLength !=
        sizeof(HID_INPUT_REPORT))
    {
        status = STATUS_INVALID_BUFFER_SIZE;
        LogError(ERRLOG_INVALID_BUFFER_SIZE,
                 status,
                 UNIQUE_ERRID(0x60),
                 NULL,
                 NULL);
        TWarn(("invalid input report size (len=%d).\n",
                   irpsp->Parameters.DeviceIoControl.OutputBufferLength));
    }
    else
    {
        IoCsqInsertIrp(&DevExt->IrpQueue,
                    Irp,
                    NULL);
        status = STATUS_PENDING;
    }

    TExit(Func,("=%x\n", status));
    return status;
}       //ReadReport

/*++
    @doc    INTERNAL

    @func   NTSTATUS | GetString | Respond to IOCTL_HID_GET_STRING.

    @parm   IN PDEVICE_EXTENSION | DevExt | Points to the device extension.
    @parm   IN PIRP | Irp | Points to an I/O Request Packet.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
GetString(
    _In_ PDEVICE_EXTENSION DevExt,
    _In_ PIRP              Irp
    )
{
    NTSTATUS            status = STATUS_SUCCESS;
    PIO_STACK_LOCATION  irpsp = NULL;
    PWSTR               pwstrID = NULL;
    ULONG_PTR           lenID = 0;
    ULONG_PTR           ulpStringID = 0;

    UNREFERENCED_PARAMETER(DevExt);

    PAGED_CODE();

    irpsp = IoGetCurrentIrpStackLocation(Irp);

    ulpStringID = (ULONG_PTR)(irpsp->Parameters.DeviceIoControl.Type3InputBuffer);
    TEnter(Func,("(DevExt=%p,Irp=%p,IrpSp=%p,StringID=%x)\n",
                DevExt, Irp, irpsp,
                (unsigned int)ulpStringID));

    switch (0xFFFF & ulpStringID)
    {
        case HID_STRING_ID_IMANUFACTURER:
            pwstrID = gpwstrManufacturerID;
            break;

        case HID_STRING_ID_IPRODUCT:
            pwstrID = gpwstrProductID;
            break;

        case HID_STRING_ID_ISERIALNUMBER:
            pwstrID = gpwstrSerialNumber;
            break;

        default:
            pwstrID = NULL;
            break;
    }

    lenID = pwstrID? wcslen(pwstrID)*sizeof(WCHAR) + sizeof(UNICODE_NULL): 0;
    if (pwstrID == NULL)
    {
        status = STATUS_INVALID_PARAMETER;
        LogError(ERRLOG_INVALID_PARAMETER,
                 status,
                 UNIQUE_ERRID(0x70),
                 NULL,
                 NULL);
        TWarn(("invalid string ID (ID=%x).\n",
                   (unsigned int)ulpStringID));
    }
    else if (irpsp->Parameters.DeviceIoControl.OutputBufferLength < lenID)
    {
        status = STATUS_BUFFER_TOO_SMALL;
        LogError(ERRLOG_BUFFER_TOO_SMALL,
                 status,
                 UNIQUE_ERRID(0x80),
                 NULL,
                 NULL);
        TWarn(("output buffer too small (len=%d,need=%d).\n",
                   irpsp->Parameters.DeviceIoControl.OutputBufferLength,
                   (LONG)lenID));
    }
    else
    {
        RtlCopyMemory(Irp->UserBuffer, pwstrID, lenID);

        Irp->IoStatus.Information = lenID;
        status = STATUS_SUCCESS;
    }

    TExit(Func,("=%x (string=%S)\n", status, pwstrID? pwstrID: L"Null"));
    return status;
}       //GetString

/*++
    @doc    INTERNAL

    @func   NTSTATUS | GetAttributes |
            Respond to IOCTL_HID_GET_ATTRIBUTES, by filling
            the HID_DEVICE_ATTRIBUTES struct.

    @parm   IN PDEVICE_EXTENSION | DevExt | Points to the device extension.
    @parm   IN PIRP | Irp | Points to an I/O Request Packet.

    @rvalue SUCCESS | returns STATUS_SUCCESS.
    @rvalue FAILURE | returns NT status code.
--*/

NTSTATUS INTERNAL
GetAttributes(
    _In_ PDEVICE_EXTENSION DevExt,
    _In_ PIRP              Irp
    )
{
    NTSTATUS            status = STATUS_SUCCESS;
    PIO_STACK_LOCATION  irpsp = NULL;

    UNREFERENCED_PARAMETER(DevExt);

    PAGED_CODE();

    irpsp = IoGetCurrentIrpStackLocation(Irp);

    TEnter(Func,("(DevExt=%p,Irp=%p,IrpSp=%p)\n", DevExt, Irp, irpsp));

    if (irpsp->Parameters.DeviceIoControl.OutputBufferLength <
        sizeof(HID_DEVICE_ATTRIBUTES))
    {
        status = STATUS_BUFFER_TOO_SMALL;
        LogError(ERRLOG_BUFFER_TOO_SMALL,
                 status,
                 UNIQUE_ERRID(0x90),
                 NULL,
                 NULL);
        TWarn(("output buffer too small (len=%d).\n",
                   irpsp->Parameters.DeviceIoControl.OutputBufferLength));
    }
    else
    {
        PHID_DEVICE_ATTRIBUTES DevAttrib;

        DevAttrib = (PHID_DEVICE_ATTRIBUTES)Irp->UserBuffer;

        DevAttrib->Size = sizeof(HID_DEVICE_ATTRIBUTES);
        DevAttrib->VendorID = OEM_VENDOR_ID;
        DevAttrib->ProductID = OEM_PRODUCT_ID;
        DevAttrib->VersionNumber = OEM_VERSION_NUM;

        Irp->IoStatus.Information = sizeof(HID_DEVICE_ATTRIBUTES);
        status = STATUS_SUCCESS;
    }

    TExit(Func,("=%x\n", status));
    return status;
}       //GetAttributes

