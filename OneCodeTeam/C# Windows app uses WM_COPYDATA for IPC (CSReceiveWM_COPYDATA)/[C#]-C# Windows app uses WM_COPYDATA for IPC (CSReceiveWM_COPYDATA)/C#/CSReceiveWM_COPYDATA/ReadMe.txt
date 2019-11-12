=============================================================================
        WINDOWS APPLICATION : CSReceiveWM_COPYDATA Project Overview
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

This code sample demonstrates receiving a custom data structure (MyStruct) 
from the sending application (CSSendWM_COPYDATA) by handling WM_COPYDATA 
messages.


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the WM_COPYDATA samples.

Step1. After you successfully build the CSSendWM_COPYDATA and 
CSReceiveWM_COPYDATA sample projects in Visual Studio 2008, you will get the 
applications: CSSendWM_COPYDATA.exe and CSReceiveWM_COPYDATA.exe. 

Step2. Run CSSendWM_COPYDATA.exe and CSReceiveWM_COPYDATA.exe. In 
CSSendWM_COPYDATA, input the Number and Message fields.

  Number: 123456
  Message: Hello World

Then click the SendMessage button. The number and the message will be sent 
to CSReceiveWM_COPYDATA through a WM_COPYDATA message. When 
CSReceiveWM_COPYDATA receives the message, the application extracts the 
number and the message, and display them in the window.


/////////////////////////////////////////////////////////////////////////////
Sample Relation:
(The relationship between the current sample and the rest samples in 
Microsoft All-In-One Code Framework http://1code.codeplex.com)

CSSendWM_COPYDATA -> CSReceiveWM_COPYDATA
CSSendWM_COPYDATA sends data to CSReceiveWM_COPYDATA through the WM_COPYDATA 
message.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. Override the WndProc method in the Windows Form.

    protected override void WndProc(ref Message m)
    {
    }

2. Handle the WM_COPYDATA message in WndProc 

    if (m.Msg == WM_COPYDATA)
    {
    }
        
3. Get the COPYDATASTRUCT struct from lParam of the WM_COPYDATA message, and 
fetch the data (MyStruct object) from COPYDATASTRUCT.lpData.

    // Get the COPYDATASTRUCT struct from lParam.
    COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));

    // If the size matches
    if (cds.cbData == Marshal.SizeOf(typeof(MyStruct)))
    {
        // Marshal the data from the unmanaged memory block to a 
        // MyStruct managed struct.
        MyStruct myStruct = (MyStruct)Marshal.PtrToStructure(cds.lpData, 
            typeof(MyStruct));
    }

4. Display the data in the form.


/////////////////////////////////////////////////////////////////////////////
References:

WM_COPYDATA Message
http://msdn.microsoft.com/en-us/library/ms649011.aspx


/////////////////////////////////////////////////////////////////////////////
