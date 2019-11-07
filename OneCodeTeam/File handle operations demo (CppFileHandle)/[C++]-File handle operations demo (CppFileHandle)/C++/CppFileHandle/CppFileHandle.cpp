/****************************** Module Header ******************************\
* Module Name:  CppFileHandle.cpp
* Project:      CppFileHandle
* Copyright (c) Microsoft Corporation.
* 
* CppFileHandle demonstrates two typical scenarios of using file handles:
* 
* 1) Enumerate file handles of a process
* 2) Get file name from a file handle
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include <stdio.h>
#include <tchar.h>
#include <windows.h>
#include <psapi.h>
#include <strsafe.h>
#include <assert.h>
#pragma endregion


#define BUFFER_SIZE			512


#pragma region EnumerateFileHandles

#define STATUS_SUCCESS					((NTSTATUS)0x00000000L)
#define STATUS_INFO_LENGTH_MISMATCH		((NTSTATUS)0xc0000004L)


#include <winternl.h>

// Undocumented SYSTEM_INFORMATION_CLASS: SystemHandleInformation
const SYSTEM_INFORMATION_CLASS SystemHandleInformation = 
(SYSTEM_INFORMATION_CLASS)16;

// The NtQuerySystemInformation function and the structures that it returns 
// are internal to the operating system and subject to change from one 
// release of Windows to another. To maintain the compatibility of your 
// application, it is better not to use the function.
typedef NTSTATUS (WINAPI * PFN_NTQUERYSYSTEMINFORMATION)(
	IN SYSTEM_INFORMATION_CLASS SystemInformationClass,
	OUT PVOID SystemInformation,
	IN ULONG SystemInformationLength,
	OUT PULONG ReturnLength OPTIONAL
	);


#define HANDLE_TYPE_FILE				28

// Undocumented structure: SYSTEM_HANDLE_INFORMATION
typedef struct _SYSTEM_HANDLE 
{
	ULONG ProcessId;
	UCHAR ObjectTypeNumber;
	UCHAR Flags;
	USHORT Handle;
	PVOID Object;
	ACCESS_MASK GrantedAccess;
} SYSTEM_HANDLE, *PSYSTEM_HANDLE;

typedef struct _SYSTEM_HANDLE_INFORMATION 
{
	ULONG NumberOfHandles;
	SYSTEM_HANDLE Handles[1];
} SYSTEM_HANDLE_INFORMATION, *PSYSTEM_HANDLE_INFORMATION;


// Undocumented FILE_INFORMATION_CLASS: FileNameInformation
const FILE_INFORMATION_CLASS FileNameInformation = 
(FILE_INFORMATION_CLASS)9;

// The NtQueryInformationFile function and the structures that it returns 
// are internal to the operating system and subject to change from one 
// release of Windows to another. To maintain the compatibility of your 
// application, it is better not to use the function.
typedef NTSTATUS (WINAPI * PFN_NTQUERYINFORMATIONFILE)(
	IN HANDLE FileHandle,
	OUT PIO_STATUS_BLOCK IoStatusBlock,
	OUT PVOID FileInformation,
	IN ULONG Length,
	IN FILE_INFORMATION_CLASS FileInformationClass
	);

// FILE_NAME_INFORMATION contains name of queried file object.
typedef struct _FILE_NAME_INFORMATION {
	ULONG FileNameLength;
	WCHAR FileName[1];
} FILE_NAME_INFORMATION, *PFILE_NAME_INFORMATION;


BOOL GetFileNameFromHandle(HANDLE hFile); 

/*!
* Enumerate file handles of the specified process using undocumented APIs.
* 
* \param pid
* Process Id
*/
DWORD EnumerateFileHandles(ULONG pid)
{
	/////////////////////////////////////////////////////////////////////////
	// Prepare for NtQuerySystemInformation and NtQueryInformationFile.
	// 

	// The functions have no associated import library. You must use the 
	// LoadLibrary and GetProcAddress functions to dynamically link to 
	// ntdll.dll.

	HINSTANCE hNtDll = LoadLibrary(_T("ntdll.dll"));
	assert(hNtDll != NULL);

	PFN_NTQUERYSYSTEMINFORMATION NtQuerySystemInformation = 
		(PFN_NTQUERYSYSTEMINFORMATION)GetProcAddress(hNtDll, 
		"NtQuerySystemInformation");
	assert(NtQuerySystemInformation != NULL);

	PFN_NTQUERYINFORMATIONFILE NtQueryInformationFile = 
		(PFN_NTQUERYINFORMATIONFILE)GetProcAddress(hNtDll, 
		"NtQueryInformationFile");


	/////////////////////////////////////////////////////////////////////////
	// Get system handle information.
	// 

	DWORD nSize = 4096, nReturn;
	PSYSTEM_HANDLE_INFORMATION pSysHandleInfo = (PSYSTEM_HANDLE_INFORMATION)
		HeapAlloc(GetProcessHeap(), 0, nSize);

	// NtQuerySystemInformation does not return the correct required buffer 
	// size if the buffer passed is too small. Instead you must call the 
	// function while increasing the buffer size until the function no longer 
	// returns STATUS_INFO_LENGTH_MISMATCH.
	while (NtQuerySystemInformation(SystemHandleInformation, pSysHandleInfo, 
		nSize, &nReturn) == STATUS_INFO_LENGTH_MISMATCH)
	{
		HeapFree(GetProcessHeap(), 0, pSysHandleInfo);
		nSize += 4096;
		pSysHandleInfo = (SYSTEM_HANDLE_INFORMATION*)HeapAlloc(
			GetProcessHeap(), 0, nSize);
	}


	/////////////////////////////////////////////////////////////////////////
	// Enumerate file handles of the process.
	// 

	DWORD dwFiles = 0;
	
	// Get the handle of the target process. The handle will be used to 
	// duplicate the file handles in the process.
	HANDLE hProcess = OpenProcess(
		PROCESS_DUP_HANDLE | PROCESS_QUERY_INFORMATION, FALSE, pid);
	if (hProcess == NULL)
	{
		_tprintf(_T("OpenProcess failed w/err 0x%08lx\n"), GetLastError());
		return -1;
	}

	for (ULONG i = 0; i < pSysHandleInfo->NumberOfHandles; i++)
	{
		PSYSTEM_HANDLE pHandle = &(pSysHandleInfo->Handles[i]);

		// Check for file handles of the specified process
		if (pHandle->ProcessId == pid && 
			pHandle->ObjectTypeNumber == HANDLE_TYPE_FILE)
		{
			dwFiles++;	// Increase the number of file handles

			// Duplicate the handle in the current process
			HANDLE hCopy;
			if (!DuplicateHandle(hProcess, (HANDLE)pHandle->Handle, 
				GetCurrentProcess(), &hCopy, MAXIMUM_ALLOWED, FALSE, 0))
				continue;

			// Retrieve file name information about the file object.
			IO_STATUS_BLOCK ioStatus;
			PFILE_NAME_INFORMATION pNameInfo = (PFILE_NAME_INFORMATION)
				malloc(MAX_PATH * 2 * 2);
			DWORD dwInfoSize = MAX_PATH * 2 * 2;

			if (NtQueryInformationFile(hCopy, &ioStatus, pNameInfo, 
				dwInfoSize, FileNameInformation) == STATUS_SUCCESS)
			{
				// Get the file name and print it
				WCHAR wszFileName[MAX_PATH + 1];
				StringCchCopyNW(wszFileName, MAX_PATH + 1, 
					pNameInfo->FileName, /*must be WCHAR*/
					pNameInfo->FileNameLength /*in bytes*/ / 2);

				wprintf(L"0x%x:\t%s\n", pHandle->Handle, wszFileName);
			}
			free(pNameInfo);

			CloseHandle(hCopy);
		}
	}

	CloseHandle(hProcess);


	/////////////////////////////////////////////////////////////////////////
	// Clean up.
	// 

	HeapFree(GetProcessHeap(), 0, pSysHandleInfo);

	// Return the number of file handles in the process
	return dwFiles;
}

#pragma endregion


#pragma region GetFileNameFromHandle

/*!
* Get file name from a handle to a file object using a file mapping object. 
* It uses the CreateFileMapping and MapViewOfFile functions to create the 
* mapping. Next, it uses the GetMappedFileName function to obtain the file 
* name. For remote files, it prints the device path received from this 
* function. For local files, it converts the path to use a drive letter and 
* prints this path.
* 
* \param hFile
* File handle
*/
BOOL GetFileNameFromHandle(HANDLE hFile) 
{
	TCHAR szFileName[MAX_PATH + 1];
	HANDLE hFileMap;

	// Get the file size
	DWORD dwFileSizeHi = 0;
	DWORD dwFileSizeLo = GetFileSize(hFile, &dwFileSizeHi); 

	if (dwFileSizeLo == 0 && dwFileSizeHi == 0)
	{
		_tprintf(_T("Cannot map a file with a length of zero\n"));
		return FALSE;
	}

	/////////////////////////////////////////////////////////////////////////
	// Create a file mapping object.
	// 

	// Create a file mapping to get the file name

	hFileMap = CreateFileMapping(hFile, NULL, PAGE_READONLY, 0, 1, NULL);
	if (!hFileMap)
	{
		_tprintf(_T("CreateFileMapping failed w/err 0x%08lx\n"), 
			GetLastError());
		return FALSE;
	}

	void* pMem = MapViewOfFile(hFileMap, FILE_MAP_READ, 0, 0, 1);
	if (!pMem)
	{
		_tprintf(_T("MapViewOfFile failed w/err 0x%08lx\n"), GetLastError());
		CloseHandle(hFileMap);
		return FALSE;
	}


	/////////////////////////////////////////////////////////////////////////
	// Call the GetMappedFileName function to obtain the file name.
	// 

	if (GetMappedFileName(GetCurrentProcess(), pMem, szFileName, MAX_PATH))
	{
		// szFileName contains device file name like:
		// \Device\HarddiskVolume2\Users\JLG\AppData\Local\Temp\HLe6098.tmp
		_tprintf(_T("Device name is %s\n"), szFileName);

		// Translate path with device name to drive letters.
		TCHAR szTemp[BUFFER_SIZE];
		szTemp[0] = '\0';

		// Get a series of null-terminated strings, one for each valid drive 
		// in the system, plus with an additional null character. Each string 
		// is a drive name. e.g. C:\\0D:\\0\0
		if (GetLogicalDriveStrings(BUFFER_SIZE - 1, szTemp)) 
		{
			TCHAR szName[MAX_PATH];
			TCHAR szDrive[3] = _T(" :");
			BOOL bFound = FALSE;
			TCHAR* p = szTemp;

			do
			{
				// Copy the drive letter to the template string
				*szDrive = *p;

				// Look up each device name. For example, given szDrive is C:, 
				// the output szName may be \Device\HarddiskVolume2.
				if (QueryDosDevice(szDrive, szName, MAX_PATH))
				{
					UINT uNameLen = _tcslen(szName);

					if (uNameLen < MAX_PATH)
					{
						// Match the device name e.g. \Device\HarddiskVolume2
						bFound = _tcsnicmp(szFileName, szName, uNameLen) == 0;

						if (bFound)
						{
							// Reconstruct szFileName using szTempFile
							// Replace device path with DOS path
							TCHAR szTempFile[MAX_PATH];
							StringCchPrintf(szTempFile, MAX_PATH, _T("%s%s"), 
								szDrive, szFileName + uNameLen);
							StringCchCopyN(szFileName, MAX_PATH + 1, 
								szTempFile, _tcslen(szTempFile));
						}
					}
				}

				// Go to the next NULL character, i.e. the next drive name.
				while (*p++);

			} while (!bFound && *p); // End of string
		}
	}


	/////////////////////////////////////////////////////////////////////////
	// Clean up the file mapping object.
	// 

	UnmapViewOfFile(pMem);
	CloseHandle(hFileMap);

	_tprintf(_T("File name is %s\n\n"), szFileName);
	return TRUE;
}

#pragma endregion


int _tmain(int argc, _TCHAR* argv[])
{
	// Enumerate file handles of a process using undocumented APIs
	ULONG pid = GetCurrentProcessId();
	DWORD dwFiles = EnumerateFileHandles(pid);

	_tprintf(TEXT("\r\n"));

	// Get file name from file handle using a file mapping object
	HANDLE hFile;
    hFile = CreateFile(TEXT("test.txt"), GENERIC_WRITE | GENERIC_READ,
		0, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hFile == INVALID_HANDLE_VALUE)
    {
        _tprintf(TEXT("CreateFile failed with %d\n"), GetLastError());
        return 0;
    }

	BYTE bWriteBuffer[] = "0123456789"; 
    DWORD dwBytesWritten; 
 
    // Write 11 bytes from the buffer to the file 
    if (!WriteFile(hFile,                // File handle 
        bWriteBuffer,                    // Buffer to be write from 
        sizeof(bWriteBuffer),            // Number of bytes to write 
        &dwBytesWritten,                 // Number of bytes that were written 
        NULL))                           // No overlapped structure 
    { 
        // WriteFile returns FALSE because of some error 
 
        _tprintf(TEXT("Could not write to file w/err 0x%08lx\n"), GetLastError()); 
        CloseHandle(hFile); 
        return 0; 
    } 

	GetFileNameFromHandle(hFile);
	CloseHandle(hFile);

	return 0;
}