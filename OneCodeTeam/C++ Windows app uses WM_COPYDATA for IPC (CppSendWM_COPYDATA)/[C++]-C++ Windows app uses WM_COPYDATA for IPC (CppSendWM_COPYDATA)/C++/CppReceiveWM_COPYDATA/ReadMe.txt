=============================================================================
        WIN32 APPLICATION : CppReceiveWM_COPYDATA Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

Inter-process Communication (IPC) based on the Windows message WM_COPYDATA is 
a mechanism for exchanging data among Windows applications in the local 
machine. The receiving application must be a Windows application. The data 
being passed must not contain pointers or other references to objects not 
accessible to the application receiving the data. While WM_COPYDATA is being 
sent, the referenced data must not be changed by another thread of the 
sending process. The receiving application should consider the data read-only. 
If the receiving application must access the data after SendMessage returns, 
it needs to copy the data into a local buffer.

This code sample demonstrates receiving a custom data structure (MY_STRUCT) 
from the sending application (CppSendWM_COPYDATA) by handling WM_COPYDATA 
messages.


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the WM_COPYDATA samples.

Step1. After you successfully build the CppSendWM_COPYDATA and 
CppReceiveWM_COPYDATA sample projects in Visual Studio 2008, you will get the 
applications: CppSendWM_COPYDATA.exe and CppReceiveWM_COPYDATA.exe. 

Step2. Run CppSendWM_COPYDATA.exe and CppReceiveWM_COPYDATA.exe. In 
CppSendWM_COPYDATA, input the Number and Message fields.

  Number: 123456
  Message: Hello World

Then click the SendMessage button. The number and the message will be sent 
to CppReceiveWM_COPYDATA through a WM_COPYDATA message. When 
CppReceiveWM_COPYDATA receives the message, the application extracts the 
number and the message, and display them in the window.


/////////////////////////////////////////////////////////////////////////////
Sample Relation:
(The relationship between the current sample and the rest samples in 
Microsoft All-In-One Code Framework http://1code.codeplex.com)

CppSendWM_COPYDATA -> CppReceiveWM_COPYDATA
CppSendWM_COPYDATA sends data to CppReceiveWM_COPYDATA through the  
WM_COPYDATA message.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. In DiagProc or WndProc, capture and handle the WM_COPYDATA message

	HANDLE_MSG (hWnd, WM_COPYDATA, OnCopyData);
	
	BOOL OnCopyData(HWND hWnd, HWND hwndFrom, PCOPYDATASTRUCT pcds)
	{  }

2. In OnCopyData, get the COPYDATASTRUCT struct from lparam of the 
WM_COPYDATA message, and fetch the data struct (MY_STRUCT) from 
COPYDATASTRUCT.lpData.

The receiving application should consider the data (COPYDATASTRUCT->lpData) 
read-only. It is valid only during the processing of the message. The 
receiving application should not free the memory referenced by the paramter. 
If the receiving application must access the data after SendMessage returns, 
it must copy the data into a local buffer. 

	MY_STRUCT myStruct;
	memcpy_s(&myStruct, sizeof(myStruct), pcds->lpData, pcds->cbData);

4. Display the data in the window.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: WM_COPYDATA Message
http://msdn.microsoft.com/en-us/library/ms649011.aspx

MSDN: Using Data Copy
http://msdn.microsoft.com/en-us/library/ms649009.aspx


/////////////////////////////////////////////////////////////////////////////
