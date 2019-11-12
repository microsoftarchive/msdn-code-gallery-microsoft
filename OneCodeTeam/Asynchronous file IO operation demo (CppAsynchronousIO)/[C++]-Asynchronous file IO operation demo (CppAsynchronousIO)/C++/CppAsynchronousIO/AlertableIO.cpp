/****************************** Module Header ******************************\
* Module Name:  AlertableIO.cpp
* Project:      CppAsynchronousIO
* Copyright (c) Microsoft Corporation.
* 
* Alertable I/O is the method by which application threads process 
* asynchronous I/O requests only when they are in an alertable state. It 
* allows multiple simultaneous I/O requests against a single device. The 
* thread that issued an I/O request must also process it because alertable 
* I/O returns the result of the I/O request only to the thread that initiated 
* it. I/O completion ports do not have this limitation.
* 
* More readings:
* http://msdn.microsoft.com/en-us/library/aa363772.aspx
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


BYTE bWriteBuffer[] = "0123456789";
BYTE bReadBuffer[100];


void WINAPI CompletedWriteRoutine(DWORD dwErr, DWORD cbBytesWritten, 
								  LPOVERLAPPED lpOverLap) 
{
	printf("%d bytes were written: %s\n", cbBytesWritten, bWriteBuffer);
}


void WINAPI CompletedReadRoutine(DWORD dwErr, DWORD cbBytesRead, 
								 LPOVERLAPPED lpOverLap) 
{
	printf("%d bytes were read: %s\n", cbBytesRead, bReadBuffer);
}


void AlertableIO(void)
{
	HANDLE hFile;
	TCHAR szFileName[] = _T("MSDN.tmp");


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
	// Perform multiple asynchronous alertable device I/O requests 
	// simultaneously.
	// 

	//
	// Issue an alertable Write request
	// 

	OVERLAPPED oWrite = { 0 };
	oWrite.OffsetHigh = 0;
	oWrite.Offset = 0;

	if (!WriteFileEx(hFile, bWriteBuffer, 11, &oWrite, 
		CompletedWriteRoutine))
	{
		_tprintf(_T("WriteFileEx failed w/err 0x%08lx\n"), GetLastError());
	}

	//
	// Issue an alertable Read request
	// 

	OVERLAPPED oRead = { 0 };
	oRead.OffsetHigh = 0;
	oRead.Offset = 0;

	if (!ReadFileEx(hFile, bReadBuffer, 100, &oRead, 
		CompletedReadRoutine))
	{
		_tprintf(_T("ReadFileEx failed w/err 0x%08lx\n"), GetLastError());
	}


	/////////////////////////////////////////////////////////////////////////
	// Put the thread in an alertable state. 
	// 

	// A thread can enter alertable state by calling one of the following 
	// functions with the appropriate flags:
	// a) SleepEx 
	// b) WaitForSingleObjectEx 
	// c) WaitForMultipleObjectsEx 
	// d) SignalObjectAndWait 
	// e) MsgWaitForMultipleObjectsEx 

	_putts(_T("Sleep for 10 seconds in an alertable state"));
	SleepEx(10000, TRUE);


	/////////////////////////////////////////////////////////////////////////
	// Close the file.
	// 

	CloseHandle(hFile);
}