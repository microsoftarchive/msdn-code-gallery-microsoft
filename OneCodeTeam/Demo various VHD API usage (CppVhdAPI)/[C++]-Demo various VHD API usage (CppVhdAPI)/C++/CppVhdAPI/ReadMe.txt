=============================================================================
       CONSOLE APPLICATION : CppVhdAPI Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The code sample demonstrates various VHD API usage, such as VHD creation, 
attaching, detaching and getting and setting disk information.


/////////////////////////////////////////////////////////////////////////////
Prerequisites:

The code sample must be run on Windows 7 / Windows Server 2008 R2 or newer.  
The minimum supported client of the VHD APIs is Windows 7; and the minimum 
supported server of the VHD APIs is Windows Server 2008 R2.


/////////////////////////////////////////////////////////////////////////////
Demo:

    CppVhdAPI.exe -[cxaomdgpe] -f <vhdfile> -s <size>
    -c CreateVirtualDisk............input: -f <vhd file name>, -s <size in MB>
    -a AttachVirtualDisk............input: -f <vhd file name>
    -d DetachVirtualDisk............input: -f <vhd file name>
    -g GetVirtualDiskInformation....input: -f <vhd file name>
    -p GetVirtualDiskPhysicalPath...input: -f <vhd file name> -- note: must be attached
    -e SetVirtualDiskInformation....input: -f <vhd file name>, -u <new GUID>

    Examples:
      Create a 3.6 Gb VHD named 'mytest.vhd'
    CppVhdAPI.exe -c -f c:\testdir\mytest.vhd -s 3600

      Attach a VHD named 'mytest.vhd'
    CppVhdAPI.exe -a -f c:\testdir\mytest.vhd
    After the virtual disk is attached, you can find the disk in Disk 
    Management MMC (Microsoft Management Console).  You can initialize the 
    disk and partition it.

      Set VHD GUID 'mytest.vhd'
    CppVhdAPI.exe -e -f c:\testdir\mytest.vhd -u {12345678-1234-5678-1234-000000000000}

      Detach a VHD named 'mytest.vhd'
    CppVhdAPI.exe -d -f c:\testdir\mytest.vhd
    After the virtual disk is detached, the disk will disappear in Disk 
    Management MMC (Microsoft Management Console).


/////////////////////////////////////////////////////////////////////////////
Implementation:

The main logic of this code is:

1. parse and validate input parameters
2. execute command and parameters resulting in the action to the VHD.  In 
this code sample, the usage of the following APIs are demoed.

    CreateVirtualDisk
        - Creates a virtual hard disk (VHD) image file

    OpenVirtualDisk
        - Opens a virtual hard disk (VHD) for use

    AttachVirtualDisk
        - Attaches a virtual hard disk (VHD) by locating an appropriate VHD 
        provider to accomplish the attachment.

    DetachVirtualDisk
        - Detaches a virtual hard disk (VHD) by locating an appropriate VHD 
        provider to accomplish the operation.

    GetVirtualDiskInformation 
        - Retrieves information about a virtual hard disk (VHD).

    GetVirtualDiskPhysicalPath
        - Retrieves the path to the physical device object that contains a 
        virtual hard disk (VHD).

    SetVirtualDiskInformation
        - Sets information about a virtual hard disk (VHD).


/////////////////////////////////////////////////////////////////////////////
References:

The location for complete information on VHDs is:
http://msdn.microsoft.com/en-us/library/dd323684(v=VS.85).aspx

The Virtual Disk API In Windows 7
http://msdn.microsoft.com/en-us/magazine/dd569754.aspx


/////////////////////////////////////////////////////////////////////////////
