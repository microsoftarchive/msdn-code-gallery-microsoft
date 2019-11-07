/****************************** Module Header ******************************\
Module Name:  CppVhdAPI.cpp
Project:      CppVhdAPI (VHD API demo)
Copyright (c) Microsoft Corporation.

Demonstrates various VHD API usage, such as VHD creation, attaching, 
detaching and getting and setting disk information.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include <windows.h>
#include <stdio.h>
#define DEFIND_GUID
#include <initguid.h>
#include <virtdisk.h>
#pragma comment(lib, "VirtDisk.lib")


#define PHYS_PATH_LEN 1024+1
GUID GUID_TEST = {12345678-1234-5678-1234-000000000000};
GUID zGuid = GUID_NULL;


void PrintErrorMessage(ULONG ErrorId)
{
    PVOID Message = NULL;
    FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | 
        FORMAT_MESSAGE_FROM_SYSTEM |
        FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL,
        ErrorId,
        0,
        (LPWSTR)&Message,
        16,
        NULL);

    wprintf(L"%s\n", Message);
    LocalFree(Message);
}

void usage()
{
    printf("CppVhdAPI.exe -[cxaomdgpe] -f <vhdfile> -s <size>\n");
    printf("-c CreateVirtualDisk............input: -f <vhd file name>, -s <size in MB>\n");
    printf("-a AttachVirtualDisk............input: -f <vhd file name>\n");
    printf("-d DetachVirtualDisk............input: -f <vhd file name>\n");
    printf("-g GetVirtualDiskInformation....input: -f <vhd file name>\n");
    printf("-p GetVirtualDiskPhysicalPath...input: -f <vhd file name> -- note: must be attached\n");
    printf("-e SetVirtualDiskInformation....input: -f <vhd file name>, -u <new GUID>\n");
    printf("Examples:\n");
    printf("  Create a 3.6 Gb VHD named 'mytest.vhd'\n");
    printf("CppVhdAPI.exe -c -f c:\\testdir\\mytest.vhd -s 3600\n\n");
    printf("  Attach a VHD named 'mytest.vhd'\n");
    printf("CppVhdAPI.exe -a -f c:\\testdir\\mytest.vhd\n\n");
    printf("  Set VHD GUID 'mytest.vhd'\n");
    printf("CppVhdAPI.exe -e -f c:\\testdir\\mytest.vhd -u {12345678-1234-5678-1234-000000000000}\n");
}

BOOL ValidateActionAndParameters(wchar_t action, PCWSTR pszFilename, ULONG sizeInMb, PCWSTR pszGuid)
{
    HRESULT hr;

    switch (action)
    {
    case L'c':  // CreateVirtualDisk
    case L'x':  // ExpandVirtualDisk
        {
            // Validate file name and size
            return (wcslen(pszFilename) && (sizeInMb));
        }

    case L'e':  // SetVirtualDiskInformation    
        if (wcslen(pszFilename) && wcslen(pszGuid))
        {
            // Validate filename and size
            hr = CLSIDFromString(pszGuid, &zGuid);
            if (SUCCEEDED(hr))
            {
                printf("failed to convert %ws to GUID, err %d\n", pszGuid, GetLastError());
                return FALSE;
            }
            return TRUE;
        } 
        else
        {
            return FALSE;
        }
        break;

    case L'a':  // AttachVirtualDisk            
    case L'o':  // CompactVirtualDisk           
    case L'd':  // DetachVirtualDisk            
    case L'g':  // GetVirtualDiskInformation    
    case L'p':  // GetVirtualDiskPhysicalPath   
    case L'm':  // MergeVirtualDisk             
        { 
            // Validate filename only
            return (wcslen(pszFilename));
        }

    default:
        return FALSE;
    }
    return FALSE;
}

BOOL CreateVHD_Fixed(PCWSTR pszVhdPath, ULONG sizeInMB)
{
    BOOL bRet = FALSE;
    HANDLE hvhd;
    CREATE_VIRTUAL_DISK_PARAMETERS  params;
    VIRTUAL_DISK_ACCESS_MASK        mask;
    VIRTUAL_STORAGE_TYPE            vst =
    {
        VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
        VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
    };

    wprintf(L"CreateVHD_Fixed %s, size (MB) %d\n", pszVhdPath, sizeInMB);

    params.Version1.UniqueId            = GUID_NULL;
    params.Version1.BlockSizeInBytes    = 0;
    params.Version1.MaximumSize		    = sizeInMB * 1024 * 1024;
    params.Version1.ParentPath		    = NULL;
    params.Version1.SourcePath          = NULL;
    params.Version1.SectorSizeInBytes   = 512;
    params.Version					    = CREATE_VIRTUAL_DISK_VERSION_1;
    mask                                = VIRTUAL_DISK_ACCESS_CREATE;

    DWORD ret = CreateVirtualDisk(&vst, 
        pszVhdPath, 
        mask, 
        NULL, 
        // To create a dynamic disk, use CREATE_VIRTUAL_DISK_FLAG_NONE instead.
        CREATE_VIRTUAL_DISK_FLAG_FULL_PHYSICAL_ALLOCATION, 
        0, 
        &params, 
        NULL, 
        &hvhd);

    if (ret == ERROR_SUCCESS)
    {
        bRet = TRUE;
    }
    else
    {
        bRet = FALSE;
        printf("failed to create vdisk...err 0x%x\n", ret);
        PrintErrorMessage(GetLastError());
    }

    if (INVALID_HANDLE_VALUE != hvhd)
    {
        CloseHandle(hvhd);
    }

    return bRet;
}

BOOL OpenAndAttachVHD(PCWSTR pszVhdPath)
{
    BOOL bRet = FALSE;
    HANDLE hVhd = INVALID_HANDLE_VALUE;
    DWORD ret;
    OPEN_VIRTUAL_DISK_PARAMETERS oparams;
    ATTACH_VIRTUAL_DISK_PARAMETERS iparams;
    VIRTUAL_STORAGE_TYPE            vst =
    {
        VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
        VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
    };

    wprintf(L"OpenAndAttachVHD %s\n", pszVhdPath);

    oparams.Version = OPEN_VIRTUAL_DISK_VERSION_1;
    oparams.Version1.RWDepth = OPEN_VIRTUAL_DISK_RW_DEPTH_DEFAULT;

    iparams.Version = ATTACH_VIRTUAL_DISK_VERSION_1;

    ret = OpenVirtualDisk(&vst, pszVhdPath,
        VIRTUAL_DISK_ACCESS_ATTACH_RW | VIRTUAL_DISK_ACCESS_GET_INFO | VIRTUAL_DISK_ACCESS_DETACH,
        OPEN_VIRTUAL_DISK_FLAG_NONE, &oparams, &hVhd);

    if (ERROR_SUCCESS == ret) 
    {
        printf("success opening vdisk...\n");
        ret = AttachVirtualDisk(hVhd, NULL, 
            ATTACH_VIRTUAL_DISK_FLAG_PERMANENT_LIFETIME, 0, &iparams, NULL);

        if (ERROR_SUCCESS == ret) 
        {
            printf("success attaching vdisk...\n");
        }
        else
        {
            printf("failed to attach vdisk...err 0x%x\n", ret);
            PrintErrorMessage(GetLastError());
            bRet = FALSE;
        }
    }
    else
    {
        printf("failed to open vdisk...err 0x%x\n", ret);
        PrintErrorMessage(GetLastError());
        bRet = FALSE;
    }

    if (INVALID_HANDLE_VALUE != hVhd)
    {
        CloseHandle(hVhd);
    }

    return bRet;
}

BOOL OpenAndDetachVHD(PCWSTR pszVhdPath)
{
    BOOL bRet = FALSE;
    DWORD ret;
    DETACH_VIRTUAL_DISK_FLAG Flags;
    HANDLE hVhd = INVALID_HANDLE_VALUE;
    OPEN_VIRTUAL_DISK_PARAMETERS oparams;
    VIRTUAL_STORAGE_TYPE            vst =
    {
        VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
        VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
    };

    wprintf(L"OpenAndDetachVHD %s\n", pszVhdPath);

    oparams.Version = OPEN_VIRTUAL_DISK_VERSION_1;
    oparams.Version1.RWDepth = OPEN_VIRTUAL_DISK_RW_DEPTH_DEFAULT;

    ret = OpenVirtualDisk(&vst, pszVhdPath,
        VIRTUAL_DISK_ACCESS_DETACH,
        OPEN_VIRTUAL_DISK_FLAG_NONE, NULL /*&oparams*/, &hVhd);

    if (ERROR_SUCCESS == ret) 
    {
        printf("success opening vdisk...\n");
        Flags = DETACH_VIRTUAL_DISK_FLAG_NONE;
        ret = DetachVirtualDisk(hVhd, Flags, 0);
        if (ERROR_SUCCESS == ret)
        {
            printf("success detaching vdisk...\n");
        }
        else
        {
            printf("failed to detach vdisk... %d\n", ret);
            PrintErrorMessage(GetLastError());
            bRet = FALSE;
        }
    }
    else
    {
        printf("failed to open vdisk...err %d\n", ret);
        PrintErrorMessage(GetLastError());
        bRet = FALSE;
    }

    if (INVALID_HANDLE_VALUE != hVhd)
    {
        CloseHandle(hVhd);
    }

    return bRet;
}

BOOL OpenAndCompactVHD(PCWSTR pszVhdPath)
{
    BOOL bRet = FALSE;
    DWORD ret;
    HANDLE hVhd = INVALID_HANDLE_VALUE;
    OPEN_VIRTUAL_DISK_PARAMETERS oparams;
    COMPACT_VIRTUAL_DISK_PARAMETERS parameters;
    COMPACT_VIRTUAL_DISK_FLAG flags = COMPACT_VIRTUAL_DISK_FLAG_NONE;

    VIRTUAL_STORAGE_TYPE            vst =
    {
        VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
        VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
    };

    wprintf(L"OpenAndCompactVHD %s\n", pszVhdPath);

    oparams.Version = OPEN_VIRTUAL_DISK_VERSION_1;
    oparams.Version1.RWDepth = OPEN_VIRTUAL_DISK_RW_DEPTH_DEFAULT;

    ret = OpenVirtualDisk(&vst, pszVhdPath,
        VIRTUAL_DISK_ACCESS_METAOPS,
        OPEN_VIRTUAL_DISK_FLAG_NONE, &oparams, &hVhd);

    if (ERROR_SUCCESS == ret)
    {
        printf("success opening vdisk...\n");

        parameters.Version = COMPACT_VIRTUAL_DISK_VERSION_1;
        parameters.Version1.Reserved = 0;

        ret = CompactVirtualDisk(hVhd, 
            COMPACT_VIRTUAL_DISK_FLAG_NONE, 
            &parameters,
            0);

        if (ERROR_SUCCESS == ret)
        {
            printf("success expanding vdisk...\n");
        }
        else
        {
            printf("failed to expand vdisk... %d\n", ret);
            PrintErrorMessage(GetLastError());
            bRet = FALSE;
        }
    }
    else
    {
        printf("failed to open vdisk...err %d\n", ret);
        PrintErrorMessage(GetLastError());
        bRet = FALSE;
    }

    if (INVALID_HANDLE_VALUE != hVhd)
    {
        CloseHandle(hVhd);
    }

    return bRet;
}

// Expanding a virtual disk requires that the virtual disk be detached during 
// the operation.
BOOL OpenAndExpandVHD(PCWSTR pszVhdPath, ULONG newSizeMB)
{
    BOOL bRet = FALSE;
    DWORD ret;
    HANDLE hVhd = INVALID_HANDLE_VALUE;
    EXPAND_VIRTUAL_DISK_PARAMETERS xparams;
    VIRTUAL_STORAGE_TYPE            vst =
    {
        VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
        VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
    };

    wprintf(L"OpenAndExpandVHD %s, new size (MB) %d\n", pszVhdPath, newSizeMB);

    ret = OpenVirtualDisk(&vst, pszVhdPath,
        VIRTUAL_DISK_ACCESS_METAOPS,
        OPEN_VIRTUAL_DISK_FLAG_NONE, NULL, &hVhd);

    if (ERROR_SUCCESS == ret) 
    {
        printf("success opening vdisk...\n");
        xparams.Version = EXPAND_VIRTUAL_DISK_VERSION_1;
        xparams.Version1.NewSize = newSizeMB * 1024 * 1024;

        ret = ExpandVirtualDisk(hVhd, EXPAND_VIRTUAL_DISK_FLAG_NONE, &xparams, 0);

        if (ERROR_SUCCESS == ret) 
        {
            printf("success expanding vdisk...\n");
        }
        else
        {
            printf("failed to expand vdisk... %d\n", ret);
            PrintErrorMessage(GetLastError());
            bRet = FALSE;
        }
    }
    else
    {
        printf("failed to open vdisk...err %d\n", ret);
        PrintErrorMessage(GetLastError());
        bRet = FALSE;
    }

    if (INVALID_HANDLE_VALUE != hVhd)
    {
        CloseHandle(hVhd);
    }

    return bRet;
}

BOOL OpenAndMergeVHD(PCWSTR pszVhdPath)
{
    BOOL bRet = FALSE;
    DWORD ret;
    HANDLE hVhd;
    MERGE_VIRTUAL_DISK_PARAMETERS mparms;
    OPEN_VIRTUAL_DISK_PARAMETERS oparms;
    MERGE_VIRTUAL_DISK_FLAG flags = MERGE_VIRTUAL_DISK_FLAG_NONE;
    VIRTUAL_STORAGE_TYPE            vst =
    {
        VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
        VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
    };

    wprintf(L"OpenAndMergeVHD %s\n", pszVhdPath);

    oparms.Version = OPEN_VIRTUAL_DISK_VERSION_1;
    oparms.Version1.RWDepth = 2;

    ret = OpenVirtualDisk(&vst, pszVhdPath,
        VIRTUAL_DISK_ACCESS_METAOPS | VIRTUAL_DISK_ACCESS_GET_INFO,
        OPEN_VIRTUAL_DISK_FLAG_NONE, &oparms, &hVhd);

    if (ERROR_SUCCESS == ret) 
    {
        printf("success opening vdisk...\n");

        mparms.Version = MERGE_VIRTUAL_DISK_VERSION_1;
        mparms.Version1.MergeDepth = oparms.Version1.RWDepth - 1; //MERGE_VIRTUAL_DISK_DEFAULT_MERGE_DEPTH;

        ret = MergeVirtualDisk(hVhd, flags, &mparms, NULL);

        if (ERROR_SUCCESS == ret) 
        {
            printf("success merging vdisk...\n");
            bRet = TRUE;
        }
        else 
        {
            printf("failed to expand vdisk... %d\n", ret);
            PrintErrorMessage(GetLastError());
            bRet = FALSE;
        }
    } 
    else
    {
        printf("failed to open vdisk...err %d\n", ret);
        PrintErrorMessage(GetLastError());
        bRet = FALSE;
    }

    if (INVALID_HANDLE_VALUE != hVhd)
    {
        CloseHandle(hVhd);
    }

    return bRet;
}

int OpenAndGetVHDInfo(PCWSTR pszVhdPath, PCWSTR pszGuid)
{
    BOOL bRet = FALSE;
    DWORD ret;
    HANDLE hVhd;
    GET_VIRTUAL_DISK_INFO Info;
    ULONG InfoSize;
    ULONG SizeUsed;
    VIRTUAL_STORAGE_TYPE            vst =
    {
        VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
        VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
    };

    wprintf(L"OpenAndGetVHDInfo %s\n", pszVhdPath);

    ret = OpenVirtualDisk(&vst, pszVhdPath, 
        VIRTUAL_DISK_ACCESS_ALL, OPEN_VIRTUAL_DISK_FLAG_NONE, 
        NULL, &hVhd);

    if (ERROR_SUCCESS == ret)
    {
        printf("success opening vdisk...\n");
        InfoSize = (ULONG)sizeof(GET_VIRTUAL_DISK_INFO);
        Info.Version = GET_VIRTUAL_DISK_INFO_SIZE;
        ret = GetVirtualDiskInformation(hVhd,
            &InfoSize,
            &Info,
            &SizeUsed);

        if (ret == ERROR_SUCCESS)
        {
            printf("success getting virtual disk size info\n");
            printf("Info.Size.VirtualSize  (bytes) = %I64d (dec)\n", Info.Size.VirtualSize);
            printf("Info.Size.PhysicalSize (bytes) = %I64d (dec)\n", Info.Size.PhysicalSize);
            printf("Info.Size.BlockSize    (bytes) = %d (dec)\n", Info.Size.BlockSize);
            printf("Info.Size.SectorSize   (bytes) = %d (dec)\n", Info.Size.SectorSize);
            bRet = TRUE;
        } 
        else
        {
            printf("failed to get virtual disk size info %d\n", ret);
            PrintErrorMessage(GetLastError());
        }

        InfoSize = (ULONG)sizeof(GET_VIRTUAL_DISK_INFO);
        Info.Version = GET_VIRTUAL_DISK_INFO_IDENTIFIER;
        ret = GetVirtualDiskInformation(hVhd,
            &InfoSize,
            &Info,
            &SizeUsed);

        if (ret == ERROR_SUCCESS) 
        {
            StringFromCLSID(Info.Identifier, (LPOLESTR *) &pszGuid);
            printf("success getting virtual disk ID info\n");
            wprintf(L"Info.Identifier  (GUID) = %s\n", pszGuid);
            bRet = TRUE;
        }
        else
        {
            printf("failed to get virtual disk ID info %d\n", ret);
            PrintErrorMessage(GetLastError());
        }
    } 
    else
    {
        printf("failed to open vdisk...err %d\n", ret);
        PrintErrorMessage(GetLastError());
    }

    if (INVALID_HANDLE_VALUE != hVhd)
    {
        CloseHandle(hVhd);
    }

    return bRet;
}

int OpenAndSetVHDInfo(PCWSTR pszVhdPath, PCWSTR pszGuid, GUID *Guid)
{
    BOOL bRet = FALSE;
    DWORD ret;
    HANDLE hVhd;
    ULONG InfoSize;
    SET_VIRTUAL_DISK_INFO SetInfo;
    VIRTUAL_STORAGE_TYPE            vst =
    {
        VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
        VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
    };

    wprintf(L"OpenAndSetVHDInfo %s GUID %s\n", pszVhdPath, pszGuid);

    ret = OpenVirtualDisk(&vst, pszVhdPath, 
        VIRTUAL_DISK_ACCESS_ALL, OPEN_VIRTUAL_DISK_FLAG_NONE, 
        NULL, &hVhd);

    if (ERROR_SUCCESS == ret) 
    {
        printf("success opening vdisk...\n");

        SetInfo.Version = SET_VIRTUAL_DISK_INFO_IDENTIFIER;
        InfoSize = sizeof(SetInfo);
        SetInfo.UniqueIdentifier = zGuid; //*Guid;

        ret = SetVirtualDiskInformation(hVhd, &SetInfo);
        if (ret == ERROR_SUCCESS) 
        {
            printf("success setting vhd info\n");
        }
        else
        {
            printf("failed to set vhd info %d\n", ret);
            PrintErrorMessage(GetLastError());
        }
    }
    else
    {
        printf("failed to open vdisk...err %d\n", ret);
        PrintErrorMessage(GetLastError());
    }

    if (INVALID_HANDLE_VALUE != hVhd) 
    {
        CloseHandle(hVhd);
    }

    return bRet;
}

BOOL OpenAndGetPhysVHD(PCWSTR pszVhdPath, PWSTR pszPhysicalDiskPath)
{    
    BOOL bRet = FALSE;
    HANDLE hVhd = INVALID_HANDLE_VALUE;
    DWORD ret;
    OPEN_VIRTUAL_DISK_PARAMETERS oparams;
    ATTACH_VIRTUAL_DISK_PARAMETERS iparams;
    VIRTUAL_STORAGE_TYPE            vst =
    {
        VIRTUAL_STORAGE_TYPE_DEVICE_VHD,
        VIRTUAL_STORAGE_TYPE_VENDOR_MICROSOFT
    };

    wprintf(L"OpenAndGetPhysVHD %s\n", pszVhdPath);

    oparams.Version = OPEN_VIRTUAL_DISK_VERSION_1;
    oparams.Version1.RWDepth = OPEN_VIRTUAL_DISK_RW_DEPTH_DEFAULT;

    iparams.Version = ATTACH_VIRTUAL_DISK_VERSION_1;

    ret = OpenVirtualDisk(&vst, pszVhdPath,
        VIRTUAL_DISK_ACCESS_ATTACH_RW | VIRTUAL_DISK_ACCESS_GET_INFO | VIRTUAL_DISK_ACCESS_DETACH,
        OPEN_VIRTUAL_DISK_FLAG_NONE, &oparams, &hVhd);

    if (ERROR_SUCCESS == ret) 
    {
        ULONG sizePhysicalDisk;
        printf("success opening vdisk...\n");
        memset(pszPhysicalDiskPath, 0, sizeof (wchar_t) * PHYS_PATH_LEN);
        sizePhysicalDisk = (PHYS_PATH_LEN * sizeof(wchar_t)) * 256;
        ret = GetVirtualDiskPhysicalPath(hVhd, &sizePhysicalDisk, pszPhysicalDiskPath);
        if (ERROR_SUCCESS == ret)
        {
            wprintf(L"success getting physical path %s vhdname\n", pszPhysicalDiskPath);
            bRet = TRUE;
        }
        else
        {
            printf("failed to get vhd physical info %d\n", ret);
            PrintErrorMessage(GetLastError());
        }
    } 
    else 
    {
        printf("failed to open vdisk...err 0x%x\n", ret);
        PrintErrorMessage(GetLastError());
    }

    if (INVALID_HANDLE_VALUE != hVhd) 
    {
        CloseHandle(hVhd);
    }

    return bRet;
}

int wmain(int argc, wchar_t *argv[])
{
    wchar_t cmd = 0;
    wchar_t action = 0;
    ULONG sizeInMb = 0;
    wchar_t szFilename[132] = {0};
    wchar_t szGuid[132] = {0};
    wchar_t szPhysicalDiskPath[PHYS_PATH_LEN * 256];

    for (int i = 1; i < argc; i++)
    {
        cmd = towlower(argv[i][1]);
        //wprintf(L"cmd = %c\n", cmd);
        switch (cmd) 
        {
        case L'c': // CreateVirtualDisk            - CreateVHD_Fixed(PWSTR vhdpath, ULONG SizeMB)
        case L'a': // AttachVirtualDisk            - OpenAndAttachVHDVHD(PWSTR vhdpath)
        case L'o': // CompactVirtualDisk           - OpenAndCompactVHD(PWSTR pszVhdPath)
        case L'd': // DetachVirtualDisk            - OpenAndDetach(PWSTR vhdpath)
        case L'x': // ExpandVirtualDisk            - OpenAndExpandVHD(PWSTR pszVhdPath, ULONG NewSizeMB)
        case L'g': // GetVirtualDiskInformation    - GetVHDInfo(HANDLE VhdHandle) - input=path
        case L'p': // GetVirtualDiskPhysicalPath   - needs API
        case L'm': // MergeVirtualDisk             - MergeVhd(HANDLE hVhd) <-- needs open, input=path
        case L'e': // SetVirtualDiskInformation    - SetVHDInfo(HANDLE VhdHandle, PWSTR ParentPath, GUID *Guid)
            action = cmd;
            break;
        case L'f':
            //wprintf(L"f: %s\n", &argv[i][3]);
            wcscpy_s(szFilename, _countof(szFilename), &argv[i][3]);
            break;
        case L's':
            //wprintf(L"s: %s\n", &argv[i][3]);
            swscanf_s(&argv[i][3], L"%d", &sizeInMb);
            break;
        case L'u':
            //wprintf(L"u: %s\n", &argv[i][3]);
            wcscpy_s(szGuid, _countof(szGuid), &argv[i][3]);
            break;
        case L'h':
        case L'?':
            usage();
            goto _test_exit;
            break;
        default:
            break;
        }
    }

    if (!ValidateActionAndParameters(action, szFilename, sizeInMb, szGuid)) 
    {
        printf("invalid command %c\n", action);
        usage();
        return -1;
    }

    wprintf(L"Command = %c, Filename = %s, SizeMB = %d, Guid = %s\n", action, szFilename, sizeInMb, szGuid);
    switch (action)
    {
    case L'c':
        printf("CreateVirtualDisk\n");
        CreateVHD_Fixed(szFilename, sizeInMb);
        break;
    case L'a':
        printf("AttachVirtualDisk\n");
        OpenAndAttachVHD(szFilename);
        break;
    case L'o':
        printf("CompactVirtualDisk\n");
        OpenAndCompactVHD(szFilename);
        break;
    case L'd':
        printf("DetachVirtualDisk\n");
        OpenAndDetachVHD(szFilename);
        break;
    case L'x':
        printf("ExpandVirtualDisk\n");
        OpenAndExpandVHD(szFilename, sizeInMb);
        break;
    case L'g':
        printf("GetVirtualDiskInformation\n");
        OpenAndGetVHDInfo(szFilename, szGuid);
        break;
    case L'p':
        printf("GetVirtualDiskPhysicalPath\n");
        OpenAndGetPhysVHD(szFilename, szPhysicalDiskPath);
        break;
    case L'm':
        printf("MergeVirtualDisk\n");
        OpenAndMergeVHD(szFilename);
        break;
    case L'e':
        printf("SetVirtualDiskInformation\n");
        OpenAndSetVHDInfo(szFilename, szGuid, &zGuid);
        break;
    default:
        wprintf(L"Unknown command %c\n", action);
        break;
    }

_test_exit:

    return 0;
}