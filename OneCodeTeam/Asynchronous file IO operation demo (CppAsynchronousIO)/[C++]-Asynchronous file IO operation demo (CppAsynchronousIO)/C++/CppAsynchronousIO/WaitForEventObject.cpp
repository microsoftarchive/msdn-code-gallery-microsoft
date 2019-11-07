/****************************** Module Header ******************************\
* Module Name:  WaitForEventObject.cpp
* Project:      CppAsynchronousIO
* Copyright (c) Microsoft Corporation.
* 
* AsyncIOWaitForEventObject demonstrates receiving completed asynchronous 
* I/O request notifications by waiting for the event kernel object attached 
* to the OVERLAPPED struct in each I/O request. It allows multiple 
* simultaneous I/O requests against a single device. It also allows one 
* thread to issue an I/O request and another thread to process it.
* 
* If you want to perform multiple asynchronous device I/O requests 
* simultaneously, you must create a separate event object for each request, 
* initialize the hEvent member in each request's OVERLAPPED structure, and 
* then call ReadFile or WriteFile. When you reach the point in your code at 
* which you need to synchronize with the completion of the I/O request, 
* simply call WaitForMultipleObjects, passing in the event handles associated 
* with each outstanding I/O request's OVERLAPPED structures. With this scheme, 
* you can easily and reliably perform multiple asynchronous device I/O 
* operations simultaneously and use the same device object. 
* 
* Typically, a real-life application has a loop that waits for I/O requests 
* to complete. As each request completes, the thread performs the desired 
* task, queues another asynchronous I/O request, and loops back around, 
* waiting for more I/O requests to complete.
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


void ReportResult(DWORD dwBytesTransferred, BYTE* bBuffer, LPSTR pszType)
{
	printf("%d bytes were %s: %s\n", dwBytesTransferred, pszType, bBuffer);
}

void ReportError(DWORD dwError)
{
	// Note: if dwError is 0xc0000011, it means STATUS_END_OF_FILE
	_tprintf(_T("AsyncIOWaitForEventObject failed w/err 0x%08lx\n"), dwError);
}

BOOL RemoveFromEventArray(HANDLE hEvents[], int nSize, int nIndex) 
{
	if (nIndex < 0 || nSize <= nIndex)
		return FALSE;

	for (int i = nIndex + 1; i < nSize; i++)
	{
		hEvents[i - 1] = hEvents[i];
	}
	return TRUE;
}


void AsyncIOWaitForEventObject(void)
{
	HANDLE hFile;
	TCHAR szFileName[] = _T("MSDN.tmp");
	DWORD dwError;


	/////////////////////////////////////////////////////////////////////////
	// Create/open a file for read and write asynchronously.
	// 

	_tprintf(_T("Open the file %s\n"), szFileName);

	hFile = CreateFile(szFileName,		// Name of the file
		GENERIC_WRITE | GENERIC_READ,	// Open for writing and reading
		0,								// Do not share
		NULL,							// Default security
		OPEN_ALWAYS,					// Always open
		// The file must be opened for asynchronous I/O by using the 
		// FILE_FLAG_OVERLAPPED flag.
		FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, 
		NULL);

	if (hFile == INVALID_HANDLE_VALUE)
	{
		_tprintf(_T("Could not open file w/err 0x%08lx\n"), GetLastError());
		return;
	}


	/////////////////////////////////////////////////////////////////////////
	// Perform multiple asynchronous device I/O requests simultaneously.
	// 

	// A queue of handles for events that are attached to the I/O requests
	HANDLE hReadWriteEvents[10] = { 0 };
	int nEvents = 0;

	// 
	// Issue an asynchronous Write command
	// 

	// The last member of the OVERLAPPED structure, hEvent, identifies an 
	// event kernel object. You must create this event object by calling 
	// CreateEvent. When an asynchronous I/O request completes, the device 
	// driver checks to see whether the hEvent member of the OVERLAPPED 
	// structure is NULL. If hEvent is not NULL, the driver signals the event 
	// by calling SetEvent. 
	OVERLAPPED oWrite = { 0 };
	oWrite.OffsetHigh = 0;
	oWrite.Offset = 0;
	oWrite.hEvent = CreateEvent(NULL, TRUE, FALSE, _T("WriteEvent"));

	BYTE bWriteBuffer[] = "0123456789";	// 11 bytes (including '\0')
	DWORD dwBytesWritten;

	// Issue the Write command 
	BOOL bWriteDone = WriteFile(hFile, bWriteBuffer, 11, &dwBytesWritten, &oWrite);
	dwError = GetLastError();

	if (bWriteDone)
	{
		// If WriteFile returns TRUE, it indicates that the I/O request was 
		// performed synchronously. At this moment, dwBytesWritten is 
		// meaningful. See http://support.microsoft.com/kb/156932

		ReportResult(dwBytesWritten, bWriteBuffer, "written");
	}
	else if (dwError != ERROR_IO_PENDING)
	{
		// Error occurred when issuing the asynchronous IO request.
		ReportError(dwError);
	}
	else
	{
		// If the I/O request was NOT performed synchronously (WriteFile 
		// returns FALSE), and GetLastError() == ERROR_IO_PENDING, the I/O is 
		// being performed asynchronously. At this moment, dwBytesWritten is 
		// meaningless.

		hReadWriteEvents[nEvents] = oWrite.hEvent;
		nEvents++;
	}

	// 
	// Issue an asynchronous Read command
	// 

	OVERLAPPED oRead = { 0 };
	oRead.OffsetHigh = 0;
	oRead.Offset = 0;
	oRead.hEvent = CreateEvent(NULL, TRUE, FALSE, _T("ReadEvent"));

	BYTE bReadBuffer[100];
	DWORD dwBytesRead;

	// Issue the Read command
	BOOL bReadDone = ReadFile(hFile, bReadBuffer, 100, &dwBytesRead, &oRead);
	dwError = GetLastError();

	if (bReadDone)
	{
		// If ReadFile returns TRUE, it indicates that the I/O request was 
		// performed synchronously. At this moment, dwBytesWritten is 
		// meaningful. See http://support.microsoft.com/kb/156932

		ReportResult(dwBytesRead, bReadBuffer, "read");
	}
	else if (dwError != ERROR_IO_PENDING)
	{
		// Error occurred when issuing the asynchronous IO request.
		ReportError(dwError);
	}
	else
	{
		// If the I/O request was NOT performed synchronously (ReadFile  
		// returns FALSE), and GetLastError() == ERROR_IO_PENDING, the I/O is 
		// being performed asynchronously. At this moment, dwBytesWritten is 
		// meaningless.

		hReadWriteEvents[nEvents] = oRead.hEvent;
		nEvents++;
	}

	// 
	// Wait for and handle the completion of any asynchronous read/write 
	// commands in a loop. Typically, a real-life application has a loop that 
	// waits for I/O requests to complete. As each request completes, the 
	// thread performs the desired task, queues another asynchronous I/O 
	// request, and loops back around, waiting for more I/O requests to 
	// complete.
	// 

	while (nEvents > 0)
	{
		DWORD dwReturn = WaitForMultipleObjects(nEvents, hReadWriteEvents, 
			FALSE, INFINITE);
		int nEventId = dwReturn - WAIT_OBJECT_0;

		// Handle the result for the signalled asynchronous IO

		if (hReadWriteEvents[nEventId] == oWrite.hEvent)
		{
			// The asynchronous write operation is done

			dwError = oWrite.Internal;
			dwBytesWritten = oWrite.InternalHigh;

			if (SUCCEEDED(dwError))
			{
				bWriteDone = TRUE;
				ReportResult(dwBytesWritten, bWriteBuffer, "written");
			}
			else
			{
				SetLastError(dwError);
				ReportError(dwError);
			}
		}
		else if (hReadWriteEvents[nEventId] == oRead.hEvent)
		{
			// The asynchronous read operation is done

			dwError = oRead.Internal;
			dwBytesRead = oRead.InternalHigh;

			if (SUCCEEDED(dwError))
			{
				bReadDone = TRUE;
				ReportResult(dwBytesRead, bReadBuffer, "read");
			}
			else
			{
				SetLastError(dwError);
				ReportError(dwError);
			}
		}

		// Remove the signaled event object
		if (RemoveFromEventArray(hReadWriteEvents, nEvents, nEventId))
			nEvents--;
	}


	/////////////////////////////////////////////////////////////////////////
	// Close the file.
	// 

	CloseHandle(hFile);
}