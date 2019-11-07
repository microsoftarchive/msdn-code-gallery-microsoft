=============================================================================
         WIN32 APPLICATION : CppSendWM_COPYDATA Project Overview
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

This code sample demonstrates sending a custom data structure (MY_STRUCT) to 
the receiving Windows application (CppReceiveWM_COPYDATA) by using 
SendMessage(WM_COPYDATA). If the data structure fails to be passed, the 
application displays the error code for diagnostics. A typical error code is 
0x5 (Access is denied) caused by User Interface Privilege Isolation (UIPI). 
UIPI prevents processes from sending selected window messages and other USER 
APIs to processes running with higher integrity. When the receiving 
application (CppReceiveWM_COPYDATA) runs at an integrity level higher than 
this sending application, you will see the "SendMessage(WM_COPYDATA) failed 
w/err 0x00000005" error message.


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

1. Find the handle of the receiving window (FindWindow)

2. Prepare the COPYDATASTRUCT struct with the data to be sent. 
(COPYDATASTRUCT)

3. Send the COPYDATASTRUCT struct through the WM_COPYDATA message to the 
receiving window.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: WM_COPYDATA Message
http://msdn.microsoft.com/en-us/library/ms649011.aspx

MSDN: Using Data Copy
http://msdn.microsoft.com/en-us/library/ms649009.aspx


/////////////////////////////////////////////////////////////////////////////
