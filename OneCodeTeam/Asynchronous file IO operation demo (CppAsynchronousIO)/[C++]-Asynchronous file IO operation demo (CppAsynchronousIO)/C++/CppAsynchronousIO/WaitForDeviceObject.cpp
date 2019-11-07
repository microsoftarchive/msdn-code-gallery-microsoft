/****************************** Module Header ******************************\
* Module Name:  WaitForDeviceObject.cpp
* Project:      CppAsynchronousIO
* Copyright (c) Microsoft Corporation.
* 
* AsyncIOWaitForDeviceObject demonstrates receiving completed asynchronous 
* I/O request notifications by waiting for the device kernel object. It's not 
* useful for performing multiple simultaneous I/O requests against a single 
* device. The solution allows one thread to issue an I/O request and another 
* thread to process it.
* 
* In Windows, a device kernel object can be used for thread synchronization, 
* so the object can either be in a signaled or nonsignaled state. The 
* ReadFile and WriteFile functions set the device kernel object to the 
* nonsignaled state just before queuing the I/O request. When the device 
* driver completes the request, the driver sets the device kernel object to 
* the signaled state. A thread can determine whether an asynchronous I/O 
* request has completed by calling either WaitForSingleObject or 
* WaitForMultipleObjects. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region "Includes"
#include <stdio.h>
#include <tchar.h>
#include <assert.h>
#include <windows.h>
#pragma endregion


void AsyncIOWaitForDeviceObject(void)
{
	HANDLE hFile;
	TCHAR szFileName[] = _T("MSDN.tmp");
	DWORD dwError;


	/////////////////////////////////////////////////////////////////////////
	// Write file asynchronously and receive the completed asynchronous I/O 
	// request notifications by waiting for the device kernel object.
	// 

	// 1. Create/open a file for write asynchronously

	_tprintf(_T("Create the file %s\n"), szFileName);

	hFile = CreateFile(szFileName,		// Name of the file
		GENERIC_WRITE,					// Open for writing
		0,								// Do not share
		NULL,							// Default security
		CREATE_ALWAYS,					// Overwrite existing
		// The file must be opened for asynchronous I/O by using the 
		// FILE_FLAG_OVERLAPPED flag.
		FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, 
		NULL);
	
	if (hFile == INVALID_HANDLE_VALUE)
    {
        _tprintf(_T("Could not create file w/err 0x%08lx\n"), GetLastError());
        return;
    }

	// 2. Initialize the OVERLAPPED structure
	// To avoid the confusion of multiple asynchronous calls to the same 
	// object, all asynchronous I/O requests must specify the starting file 
	// offset in the OVERLAPPED structure. OffsetHigh and Offset indicate 
	// 64-bit offset in the file where you want the I/O operation to begin.

	OVERLAPPED oWrite = { 0 };
	oWrite.OffsetHigh = 0;
	oWrite.Offset = 0;

	// 3. Issue the Write command to write 100 bytes at the offset specified 
	// in OVERLAPPED.OffsetHigh/Offset to the file from the buffer

	BYTE bWriteBuffer[] = "0123456789";	// 11 bytes (including '\0')
	DWORD dwBytesWritten;
	BOOL bWriteDone = WriteFile(hFile, bWriteBuffer, 11, &dwBytesWritten, 
		&oWrite);

	// If WriteFile returns TRUE, it indicates that the I/O request was  
	// performed synchronously. At this moment, dwBytesWritten is meaningful.
	// See http://support.microsoft.com/kb/156932

	// Else

	// If the I/O request was NOT performed synchronously (WriteFile returns 
	// FALSE), check to see whether an error occurred or whether the I/O is 
	// being performed asynchronously. (GetLastError() == ERROR_IO_PENDING)
	// At this moment, dwBytesWritten is meaningless.
	dwError = GetLastError();
	if (!bWriteDone && (dwError == ERROR_IO_PENDING)) 
	{
		// The I/O is being performed asynchronously

		// There are many things a program can do while waiting for async 
		// operations to complete, such as queuing additional operations, or 
		// doing background work.
		// ......

		// Wait on the device object for the async I/O operation to complete
		WaitForSingleObject(hFile, INFINITE);
		
		// oWrite.Internal contains the I/O error
		// oWrite.InternalHigh contains the number of bytes transferred
		dwError = oWrite.Internal;
		dwBytesWritten = oWrite.InternalHigh;
		
		if (SUCCEEDED(dwError))
		{
			bWriteDone = TRUE;
		}
		else
		{
			SetLastError(dwError);
		}
	}

	// 4. Handle the result
	// The write operation was finished successfully or failed with an error
	if (bWriteDone) 
	{
		// dwBytesWritten contains the number of written bytes
		// bWriteBuffer contains the written data
		
		// Print the result
		printf("%d bytes were written: %s\n", dwBytesWritten, bWriteBuffer);
	}
	else
	{
		// An error occurred; see dwError

		// Print the error
		_tprintf(_T("AsyncIOWaitForDeviceObject(Write) failed w/err 0x%08lx\n"), 
			dwError);
	}

	// 5. Close the file
	CloseHandle(hFile);


	/////////////////////////////////////////////////////////////////////////
	// Read file asynchronously and receive the completed asynchronous I/O 
	// request notifications by waiting for the device kernel object.
	// 

	// 1. Open the file for read asynchronously

	_tprintf(_T("Open the file %s\n"), szFileName);

	hFile = CreateFile(szFileName,		// Name of the file
		GENERIC_READ,					// Open for reading
		FILE_SHARE_READ,				// Share reading
		NULL,							// Default security
		OPEN_EXISTING,					// Open existing
		// The file must be opened for asynchronous I/O by using the 
		// FILE_FLAG_OVERLAPPED flag.
		FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, 
		NULL);

	if (hFile == INVALID_HANDLE_VALUE)
    {
        _tprintf(_T("Could not open file w/err 0x%08lx\n"), GetLastError());
        return;
    }

	// 2. Initialize the OVERLAPPED structure
	// To avoid the confusion of multiple asynchronous calls to the same 
	// object, all asynchronous I/O requests must specify the starting file 
	// offset in the OVERLAPPED structure. OffsetHigh and Offset indicate 
	// 64-bit offset in the file where you want the I/O operation to begin.

	OVERLAPPED oRead = { 0 };
	oRead.OffsetHigh = 0;
	oRead.Offset = 0;

	// 3. Issue the Read command to read 100 bytes at the offset specified 
	// in OVERLAPPED.OffsetHigh/Offset from the file to the buffer

	BYTE bReadBuffer[100];
	DWORD dwBytesRead;
	BOOL bReadDone = ReadFile(hFile, bReadBuffer, 100, &dwBytesRead, &oRead);

	// If ReadFile returns TRUE, it indicates that the I/O request was  
	// performed synchronously. At this moment, dwBytesRead is meaningful.
	// See http://support.microsoft.com/kb/156932

	// Else

	// If the I/O request was NOT performed synchronously (ReadFile returns 
	// FALSE), check to see whether an error occurred or whether the I/O is 
	// being performed asynchronously. (GetLastError() == ERROR_IO_PENDING)
	// At this moment, dwBytesRead is meaningless.
	dwError = GetLastError();
	if (!bReadDone && (dwError == ERROR_IO_PENDING)) 
	{
		// The I/O is being performed asynchronously

		// There are many things a program can do while waiting for async 
		// operations to complete, such as queuing additional operations, or 
		// doing background work.
		// ......

		// Wait on the device object for the async I/O operation to complete
		WaitForSingleObject(hFile, INFINITE);
		
		// oRead.Internal contains the I/O error
		// oRead.InternalHigh contains the number of bytes transferred
		dwError = oRead.Internal;
		dwBytesRead = oRead.InternalHigh;
		
		if (SUCCEEDED(dwError))
		{
			bReadDone = TRUE;
		}
		else
		{
			SetLastError(dwError);
		}
	}

	// 4. Handle the result
	// The read operation was finished successfully or failed with an error
	if (bReadDone) 
	{
		// dwBytesRead contains the number of read bytes
		// bBuffer contains the read data
		
		// Print the result
		printf("%d bytes were read: %s\n", dwBytesRead, bReadBuffer);
	}
	else
	{
		// An error occurred; see dwError

		// Print the error
		_tprintf(_T("AsyncIOWaitForDeviceObject(Read) failed w/err 0x%08lx\n"), 
			dwError);
	}

	// 5. Close the file
	CloseHandle(hFile);
}