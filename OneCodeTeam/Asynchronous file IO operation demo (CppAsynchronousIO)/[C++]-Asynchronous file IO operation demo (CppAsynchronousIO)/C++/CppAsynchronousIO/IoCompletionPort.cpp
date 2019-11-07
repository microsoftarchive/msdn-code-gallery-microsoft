/****************************** Module Header ******************************\
* Module Name:  IoCompletionPort.cpp
* Project:      CppAsynchronousIO
* Copyright (c) Microsoft Corporation.
* 
* I/O completion ports provide an efficient threading model for processing 
* multiple asynchronous I/O requests on a multiprocessor system. When a 
* process creates an I/O completion port, the system creates an associated 
* queue object for requests whose sole purpose is to service these requests. 
* Processes that handle many concurrent asynchronous I/O requests can do so 
* more quickly and efficiently by using I/O completion ports in conjunction 
* with a pre-allocated thread pool than by creating threads at the time they 
* receive an I/O request.
* 
* An I/O completion port allows multiple simultaneous I/O requests against 
* a single device. It also allows one thread to issue an I/O request and  
* another thread to process it. This technique is highly scalable and has the  
* most flexibility. Alertable I/Os allow asynchronous I/O requests to be 
* fulfilled on a single thread. In contrast, completion ports allow any 
* thread to perform asynchronous I/O and have the results processed by an 
* arbitrary thread. A completion port is a kernel object that you can 
* associate with a number of file handles. Once a file handle is associated 
* with a completion port the results of any asynchronous I/O requests, known 
* as completion packets, are queued to the completion port and can be 
* dequeued by any available thread in the process. 
* 
* The main challenge with completion ports is getting your head around the 
* confusing API. There are a few operations involved in using a completion 
* port:
* 
*   Creating a completion port (CreateIoCompletionPort)
*   Associating file handles (CreateIoCompletionPort)
*   Dequeuing completion packets (GetQueuedCompletionStatus)
*   Optionally queuing your own completion packets (PostQueuedCompletionStatus)
*   Creating completion port threads (CreateThread)
* 
* More readings: 
* http://msdn.microsoft.com/en-us/library/aa365198.aspx
* http://weblogs.asp.net/kennykerr/archive/2008/01/03/parallel-programming-with-c-part-4-i-o-completion-ports.aspx
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


#define BUFFER_SIZE				100

#define MSDNFILE_COMPKEY		1000
#define IO_READ					1001
#define IO_WRITE				1002


/*!
* The IOCompletionPort class wraps the functions of I/O completion port.
*/
class IOCompletionPort
{
private:
	HANDLE m_hIOCP;

public:

	IOCompletionPort() : m_hIOCP(NULL)
	{
	}

	~IOCompletionPort()
	{
		Close();
	}

	/*!
	* Create a new completion port that is not associated with any I/O 
	* devices, and set the maximum number of threads that the operating 
	* system can allow to concurrently process I/O completion packets for 
	* the I/O completion port.
	* 
	* \param dwNumberOfConcurrentThreads
	* The maximum number of threads that the operating system can allow to 
	* concurrently process I/O completion packets for the I/O completion 
	* port. If this parameter is zero, the system allows as many concurrently 
	* running threads as there are processors in the system. 
	*/	
	BOOL Create(DWORD dwNumberOfConcurrentThreads = 0)
	{
		m_hIOCP = CreateIoCompletionPort(INVALID_HANDLE_VALUE, NULL, 0, 
			dwNumberOfConcurrentThreads);
		return (m_hIOCP != NULL);
	}

	BOOL Close()
	{
		// Ensure that the handle of the I/O completion port is closed
		if (m_hIOCP != NULL && CloseHandle(m_hIOCP))
		{
			m_hIOCP = NULL;
			return TRUE;
		}
		return FALSE;
	}

	/*!
	* Associate a device with the port. The system appends this information 
	* to the completion port's device list. To have the results of 
	* asynchronous I/O requests queued to the completion port you need to 
	* associate the file handles with the completion port. 
	* 
	* When an asynchronous I/O request for a device completes, the system 
	* checks to see whether the device is associated with a completion port 
	* and, if it is, the system appends the completed I/O request entry to 
	* the end of the completion port's I/O completion queue. 
	* 
	* \param hDevice
	* An open device handle to be associated with the I/O completion port. 
	* The handle has to have been opened for overlapped I/O completion.
	* 
	* \param CompKey
	* A value that has meaning to you; the operating system does not care  
	* what you pass here. We may use this parameter to differentiate the 
	* different devices associated to the completion port. One completion key 
	* stands for a device handle. The value is included in the completion 
	* packet for any I/O requests for the given file handle.
	*/
	BOOL AssociateDevice(HANDLE hDevice, ULONG_PTR CompKey)
	{
		HANDLE h = CreateIoCompletionPort(hDevice, m_hIOCP, CompKey, 0);
		return h == m_hIOCP;
	}

	/*!
	* Queue your own completion packets. Although completion packets are 
	* normally queued by the operating system as asynchronous I/O requests 
	* are completed, you can also queue your own completion packets. This is 
	* achieved using the PostQueuedCompletionStatus function. 
	* 
	* \param CompKey
	* The value to be returned through the lpCompletionKey parameter of the 
	* GetQueuedCompletionStatus function.
	* 
	* \param dwNumBytes
	* The value to be returned through the lpNumberOfBytesTransferred 
	* parameter of the GetQueuedCompletionStatus function.
	* 
	* \param po
	* The value to be returned through the lpOverlapped parameter of the 
	* GetQueuedCompletionStatus function.
	*/
	BOOL QueuePacket(ULONG_PTR CompKey, DWORD dwNumBytes = 0, 
		OVERLAPPED* po = NULL)
	{
		return PostQueuedCompletionStatus(m_hIOCP, dwNumBytes, CompKey, po);
	}

	/*!
	* Attempts to dequeue an I/O completion packet from the specified I/O 
	* completion port. If there is no completion packet queued, the function 
	* waits for a pending I/O operation associated with the completion port 
	* to complete. You can dequeue completion packets on any thread in the 
	* process that created the completion port. All that the thread needs is 
	* the port handle.
	* 
	* \param pCompKey
	* A pointer to a variable that receives the completion key value 
	* associated with the file handle whose I/O operation has completed. A 
	* completion key is a per-file key that is specified in a call to 
	* AssociateDevice.
	* 
	* \param pdwNumBytes
	* A pointer to a variable that receives the number of bytes transferred 
	* during an I/O operation that has completed.
	* 
	* \param ppo
	* A pointer to a variable that receives the address of the OVERLAPPED 
	* structure that was specified when the completed I/O operation was 
	* started. 
	* 
	* \param dwMilliseconds
	* The number of milliseconds that the caller is willing to wait for a 
	* completion packet to appear at the completion port. If a completion 
	* packet does not appear within the specified time, the function times 
	* out, returns FALSE, and sets *lpOverlapped to NULL.
	*/
	BOOL DequeuePacket(ULONG_PTR* pCompKey, PDWORD pdwNumBytes, 
		OVERLAPPED** ppo, DWORD dwMilliseconds = INFINITE)
	{
		return GetQueuedCompletionStatus(m_hIOCP, pdwNumBytes, pCompKey, ppo, 
			dwMilliseconds);
	}
};


/*!
* The OVERLAPPEDEX structures appends some members to the VERLAPPED structure
*/
struct OVERLAPPEDEX : OVERLAPPED
{
public:
	OVERLAPPEDEX(int nType = 0, BYTE* pbBuffer = NULL, int nSize = 0)
	{
		Internal = InternalHigh = 0;   
		Offset = OffsetHigh = 0;
		hEvent = NULL;
		Type = nType;

		if (pbBuffer && nSize > 0)
		{
			memcpy_s(Buffer, BUFFER_SIZE, pbBuffer, nSize);
		}
	}

	// Append some members to the old OVERLAPPED structure
	int Type;
	BYTE Buffer[BUFFER_SIZE];
};


/*!
* The thread procedure that dequeues the I/O completion packets from the I/O 
* completion port object passed in through lpParam, and process the 
* completion packets.
* 
* \param lpParam
* The address of the I/O completion port object.
*/
DWORD WINAPI IOCompletionThread(LPVOID lpParam)
{
	IOCompletionPort* pPort = (IOCompletionPort*)lpParam;

	ULONG_PTR compKey = 0;
	DWORD dwBytesTransferred = 0;
	OVERLAPPEDEX* po = NULL;

	while (pPort->DequeuePacket(&compKey, &dwBytesTransferred, 
		(OVERLAPPED **)&po, 2000 /* timeout: 2 seconds */))
    {
        if (dwBytesTransferred == 0 && compKey == 0 && NULL == po)
        {
            break;
        }
		else if (compKey != MSDNFILE_COMPKEY)
		{
			// Ignore completion packet for other devices
			continue;
		}

		// Process completion packet.
		
		//Sleep(2000); // Observe the thread processing the completion packet

		switch (po->Type)
		{
		case IO_READ:
			printf("%d bytes were read: %s\n", dwBytesTransferred, 
				po->Buffer);
			break;
		case IO_WRITE:
			printf("%d bytes were written: %s\n", dwBytesTransferred, 
				po->Buffer);
			break;
		}
    }

	_tprintf(_T("DequeuePacket failed w/err 0x%08lx\n"), GetLastError());

	return 0;
}


void IoCompletionPort(void)
{
	/////////////////////////////////////////////////////////////////////////
	// Create an I/O completion port.
	// 

	// Determine how many threads in the thread pool. The threads will 
	// dequeue and process the completion packets. You may want to experiment 
	// with different thread counts, or concurrency values, based on your 
	// application's completion processing. For example if you cannot avoid 
	// making some blocking calls on your completion port threads you may want 
	// to increase the concurrency to allow more threads to be scheduled.

	// The general value of the thread count is the system's processor count.
	SYSTEM_INFO sysInfo = { 0 };
	GetNativeSystemInfo(&sysInfo);
	const DWORD dwThreadCount = sysInfo.dwNumberOfProcessors; 

	IOCompletionPort port;
	if (!port.Create(dwThreadCount))
	{
		_tprintf(_T(
			"The creation of I/O completion port failed w/err 0x%08lx\n"), 
			GetLastError());
		return;
	}

	_tprintf(_T("An I/O completion port is created\nIt allows %d threads") \
		_T(" to concurrently process I/O completion packets\n"), 
		dwThreadCount);


	/////////////////////////////////////////////////////////////////////////
	// Open the target device, and associate the device with the I/O 
	// completion port.
	// 

	HANDLE hFile;
	TCHAR szFileName[] = _T("MSDN.tmp");


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

	// Associate the file with the I/O completion port
	port.AssociateDevice(hFile, MSDNFILE_COMPKEY);


	/////////////////////////////////////////////////////////////////////////
	// Issue multiple asynchronous device I/O requests simultaneously.
	// 

	// Issue an asynchronous Write command
	OVERLAPPEDEX oWrite(IO_WRITE, (BYTE *)"0123456789", 11);
	BOOL bWriteDone = WriteFile(hFile, oWrite.Buffer, 11, NULL, &oWrite);
	// Omit error checking ...

	// Issue an asynchronous Read command
	OVERLAPPEDEX oRead(IO_READ);
	BOOL bReadDone = ReadFile(hFile, oRead.Buffer, BUFFER_SIZE, NULL, &oRead);
	// Omit error checking ...


	/////////////////////////////////////////////////////////////////////////
	// Create and run completion port threads.
	// 

	// Construct the thread pool
	HANDLE* hThreads = new HANDLE[dwThreadCount];
	for (DWORD i = 0; i < dwThreadCount; ++i)
	{
		// The threads run CompletionThread
		hThreads[i] = CreateThread(0, 0, IOCompletionThread, &port, 0, NULL);
	}

	// Wait for the quit of the threads
	WaitForMultipleObjects(dwThreadCount, hThreads, TRUE, INFINITE);

	// Close the threads and perform the cleanup
	for (DWORD i = 0; i < dwThreadCount; ++i)
	{
		CloseHandle(hThreads[i]);
	}
	delete[] hThreads;


	/////////////////////////////////////////////////////////////////////////
	// Close handles.
	// 

	CloseHandle(hFile);
	// I/O completion port is closed in IOCompletionPort's destructor.
}