/****************************** Module Header ******************************\
* Module Name:  CppAsynchronousIO.cpp
* Project:      CppAsynchronousIO
* Copyright (c) Microsoft Corporation.
* 
* CppAsynchronousIO demonstrates the asynchronous file I/O operations. A 
* thread performing asynchronous file I/O sends an I/O request to the kernel 
* by calling an appropriate function. If the request is accepted by the 
* kernel, the calling thread continues processing another job until the 
* kernel signals to the thread that the I/O operation is complete. It then 
* interrupts its current job and processes the data from the I/O operation as 
* necessary.
* 
* To access a device asynchronously, you must first open the device by 
* calling CreateFile, specifying the FILE_FLAG_OVERLAPPED flag in the 
* dwFlagsAndAttributes parameter. This flag notifies the system that you 
* intend to access the device asynchronously.
* 
* To queue an I/O request for a device driver, you use the ReadFile and 
* WriteFile with an OVERLAPPED structure.
* 
* To receive completed I/O request notifications, Windows offers four 
* different methods:
* 
* 1. Signaling a device kernel object: Not useful for performing multiple 
* simultaneous I/O requests against a single device. Allows one thread to 
* issue an I/O request and another thread to process it. (See 
* WaitForDeviceObject)
* 
* 2. Signaling an event kernel object: Allows multiple simultaneous I/O 
* requests against a single device. Allows one thread to issue an I/O request 
* and another thread to process it. (See: WaitForEventObject and 
* GetOverlappedResult)
* 
* 3. Using alertable I/O: Allows multiple simultaneous I/O requests against a 
* single device. The thread that issued an I/O request must also process it. 
* (See: AlertableIO)
* 
* 4. Using I/O completion ports: Allows multiple simultaneous I/O requests 
* against a single device. Allows one thread to issue an I/O request and 
* another thread to process it. This technique is highly scalable and has the 
* most flexibility. (See IoCompletionPort)
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


void AsyncIOWaitForDeviceObject(void);
void AsyncIOWaitForEventObject(void);
void AsyncIOGetOverlappedResult(void);
void AlertableIO(void);
void IoCompletionPort(void);


int _tmain(int argc, _TCHAR* argv[])
{
	_putts(_T("AsyncIOWaitForDeviceObject"));

	// Receive completed asynchronous I/O request notifications by waiting 
	// for the device kernel object. It's not useful for performing multiple 
	// simultaneous I/O requests against a single device, but it allows one 
	// thread to issue an I/O request and another thread to process it.
	AsyncIOWaitForDeviceObject();

	_putts(_T("\nAsyncIOWaitForEventObject"));

	// Receive completed asynchronous I/O request notifications by waiting 
	// for the event kernel object attached to the OVERLAPPED struct in each 
	// I/O request. It allows multiple simultaneous I/O requests against a 
	// single device. It also allows one thread to issue an I/O request and 
	// another thread to process it.
	AsyncIOWaitForEventObject();

	_putts(_T("\nAsyncIOGetOverlappedResult"));

	// Receive completed asynchronous I/O request notifications by calling 
	// GetOverlappedResult. It allows one thread to issue an I/O request and 
	// another thread to process it.
	AsyncIOGetOverlappedResult();

	_putts(_T("\nAlertableIO"));

	// Alertable I/O the method by which application threads process 
	// asynchronous I/O requests only when they are in an alertable state. 
	// It allows multiple simultaneous I/O requests against a single device. 
	// The thread that issued an I/O request must also process it.
	AlertableIO();

	_putts(_T("\nIoCompletionPort"));

	// An I/O completion port allows multiple simultaneous I/O requests 
	// against a single device. It also allows one thread to issue an I/O 
	// request and another thread to process it. This technique is highly 
	// scalable and has the most flexibility. 
	IoCompletionPort();

	return 0;
}