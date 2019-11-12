========================================================================
    CONSOLE APPLICATION : CppAsynchronousIO Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

CppAsynchronousIO demonstrates the asynchronous file I/O operations. A 
thread performing asynchronous file I/O sends an I/O request to the kernel by 
calling an appropriate function. If the request is accepted by the kernel, 
the calling thread continues processing another job until the kernel signals 
to the thread that the I/O operation is complete. It then interrupts its 
current job and processes the data from the I/O operation as necessary.


/////////////////////////////////////////////////////////////////////////////
Project Relation:

CppAsynchronousIO - CppSynchronousIO
CppAsynchronousIO shows asynchronous I/O operations and CppSynchronousIO 
demonstrates synchronous I/O.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. Create/open a file for reading and writing asynchronously by calling 
CreateFile, specifying the FILE_FLAG_OVERLAPPED flag in the 
dwFlagsAndAttributes parameter. 

2. To queue an I/O request for a device driver, call ReadFile and WriteFile 
with an OVERLAPPED structure.

3. To receive completed I/O request notifications, Windows offers four 
different methods:

 a) Signaling a device kernel object: Not useful for performing multiple 
 simultaneous I/O requests against a single device. Allows one thread to 
 issue an I/O request and another thread to process it. (See 
 WaitForDeviceObject)

 b) Signaling an event kernel object: Allows multiple simultaneous I/O 
 requests against a single device. Allows one thread to issue an I/O request 
 and another thread to process it. (See: WaitForEventObject and 
 GetOverlappedResult)

 c) Using alertable I/O: Allows multiple simultaneous I/O requests against a 
 single device. The thread that issued an I/O request must also process it. 
 (See: AlertableIO)

 d) Using I/O completion ports: Allows multiple simultaneous I/O requests 
 against a single device. Allows one thread to issue an I/O request and 
 another thread to process it. This technique is highly scalable and has the 
 most flexibility. (See IoCompletionPort)


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Synchronous and Asynchronous I/O
http://msdn.microsoft.com/en-us/library/aa365683.aspx

Windows via C/C++, Fifth Edition. Basics of Asynchronous Device I/O
http://msdn.microsoft.com/en-us/library/cc500408.aspx


/////////////////////////////////////////////////////////////////////////////
