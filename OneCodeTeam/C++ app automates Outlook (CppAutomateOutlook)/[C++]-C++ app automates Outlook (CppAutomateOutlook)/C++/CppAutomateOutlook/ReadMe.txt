========================================================================
    CONSOLE APPLICATION : CppAutomateOutlook Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The CppAutomateOutlook example demonstrates how to write VC++ code to 
automate Microsoft Outlook to log on with your profile, enumerate contacts, 
send a mail, log off, close the Microsoft Outlook application and then clean 
up unmanaged COM resources. 

There are three basic ways you can write VC++ automation codes:

1. Automating Outlook using the #import directive and smart pointers 

The code in Solution1.h/cpp demonstrates the use of #import to automate
Outlook. #import (http://msdn.microsoft.com/en-us/library/8etzzkb6.aspx), a 
new directive that became available with Visual C++ 5.0, creates VC++ "smart 
pointers" from a specified type library. It is very powerful, but often not 
recommended because of reference-counting problems that typically occur when 
used with the Microsoft Office applications. Unlike the direct API approach 
in Solution2.h/cpp, smart pointers enable us to benefit from the type info to 
early/late bind the object. #import takes care of adding the messy guids to 
the project and the COM APIs are encapsulated in custom classes that the 
#import directive generates.

2. Automating Outlook using C++ and the COM APIs

The code in Solution2.h/cpp demontrates the use of C/C++ and the COM APIs to 
automate Outlook. The raw automation is much more difficult, but it is  
sometimes necessary to avoid the overhead with MFC, or problems with #import. 
Basically, you work with such APIs as CoCreateInstance(), and COM interfaces 
such as IDispatch and IUnknown.

3. Automating Outlook with MFC

With MFC, Visual C++ Class Wizard can generate "wrapper classes" from the 
type libraries. These classes simplify the use of the COM servers. Automating 
Outlook with MFC is not covered in this sample.


/////////////////////////////////////////////////////////////////////////////
Prerequisite:

You must run this code sample on a computer that has Microsoft Outlook 2007 
installed.


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the Outlook automation 
sample that starts a Microsoft Outlook instance, logs on with your profile, 
enumerates the contact items, creates and sends a new mail item, logs off and 
quits the Microsoft Outlook application cleanly.

Step1. After you successfully build the sample project in Visual Studio 2008, 
you will get the application: CppAutomateOutlook.exe.

Step2. Open Windows Task Manager (Ctrl+Shift+Esc) to confirm that no 
outlook.exe is running. 

Step3. Run the application. It should print the following content in the 
console window if no error is thrown.

  Outlook.Application is started
  User logs on ...
  Please press ENTER to continue when Outlook is ready.

Outlook would ask you to input your profile and password. When Outlook is 
ready, press ENTER in the console window of CppAutomateOutlook. The 
application will then enumerate your contacts and print the contacts:

  Enumerate the contact items
  <the email address of your contacts and the name of your discussion lists>

Next, CppAutomateOutlook automates Outlook to create and display or send a 
new mail item. 

  Create and send a new mail item

In the new mail item, the To line is set as codefxf@microsoft.com, which is 
the feedback channel of All-In-One Code Framework. The Subject is set to 
"Feedback of All-In-One Code Framework" and the email body shows "Feedback:" 
in bold.

After you input your feedback and click the Send button, the mail item is 
sent and CppAutomateOutlook automates Outlook to log off the current profile 
and quit itself.

  Log off and quit the Outlook application

Step4. In Windows Task Manager, confirm that the outlook.exe process does not 
exist, i.e. the Microsoft Outlook intance was closed and cleaned up properly.


/////////////////////////////////////////////////////////////////////////////
Project Relation:

CppAutomateOutlook - CSAutomateOutlook - VBAutomateOutlook

These examples automate Microsoft Outlook to do the same thing in different 
programming languages.


/////////////////////////////////////////////////////////////////////////////
Creation:

A. Automating Outlook using the #import directive and smart pointers 
(Solution1.h/cpp)

Step1. Import the type library of the target COM server using the #import 
directive. 

	#import "libid:2DF8D04C-5BFA-101B-BDE5-00AA0044DE52" \
		rename("RGB", "MSORGB") \
		rename("DocumentProperties", "MSODocumentProperties")
	// [-or-]
	//#import "C:\\Program Files\\Common Files\\Microsoft Shared\\OFFICE12\\MSO.DLL" \
	//	rename("RGB", "MSORGB") \
	//	rename("DocumentProperties", "MSODocumentProperties")

	using namespace Office;

	#import "progid:Outlook.Application" \
		rename("CopyFile", "OutlookCopyFile") \
		rename("PlaySound", "OutlookPlaySound")
	// [-or-]
	//#import "libid:00062FFF-0000-0000-C000-000000000046" \
	//	rename("CopyFile", "OutlookCopyFile") \
	//	rename("PlaySound", "OutlookPlaySound")
	// [-or-]
	//#import "C:\\Program Files\\Microsoft Office\\Office12\\MSOUTL.OLB"	\
	//	rename("CopyFile", "OutlookCopyFile") \
	//	rename("PlaySound", "OutlookPlaySound")

Step2. Build the project. If the build is successful, the compiler generates 
the .tlh and .tli files that encapsulate the COM server based on the type 
library specified in the #import directive. It serves as a class wrapper we
can now use to create the COM class and access its properties, methods, etc.

Step3. Initializes the COM library on the current thread and identifies the 
concurrency model as single-thread apartment (STA) by calling CoInitializeEx, 
or CoInitialize.

Step4. Create the Outlook.Application COM object using the smart pointer. The 
class name is the original interface name (i.e. Outlook::_Application) with a 
"Ptr" suffix. We can use either the constructor of the smart pointer class or 
its CreateInstance method to create the COM object.

Step5. Automate the Outlook COM object through the smart pointers. In this 
example, you can find the basic operations in Outlook automation like 

	Start Microsoft Outlook and log on with your profile.
	Enumerate the contact items.
	Create and display/send a new mail item.
	User logs off.

Step6. Quit the Outlook application. (i.e. Application.Quit())

Step7. It is said that the smart pointers are released automatically, so we 
do not need to manually release the COM objects.

Step8. It is necessary to catch the COM errors if the type library was  
imported without raw_interfaces_only and when the raw interfaces 
(e.g. raw_Quit) are not used. For example:

	#import "XXXX.tlb"
	try
	{
		spOutookApp->Quit();
	}
	catch (_com_error &err)
	{
	}

Step9. Uninitialize COM for this thread by calling CoUninitialize.

-----------------------------------------------------------------------------

B. Automating Outlook using C++ and the COM APIs (Solution2.h/cpp)

Step1. Add the automation helper function, AutoWrap.

Step2. Initializes the COM library on the current thread and identifies the 
concurrency model as single-thread apartment (STA) by calling CoInitializeEx, 
or CoInitialize.

Step3. Get CLSID of the Outlook COM server using the API CLSIDFromProgID.

Step4. Start the Outlook COM server and get the IDispatch interface using  
the API CoCreateInstance.

Step5. Automate the Outlook COM object with the help of AutoWrap. In this 
example, you can find the basic operations in Outlook automation like 

	Start Microsoft Outlook and log on with your profile.
	Create and send a new mail item.
	User logs off.

Step6. Quit the Outlook application. (i.e. Application.Quit())

Step7. Release the COM objects.

Step8. Uninitialize COM for this thread by calling CoUninitialize.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Outlook 2007 Developer Reference
http://msdn.microsoft.com/en-us/library/bb177050.aspx

How to use an Outlook Object Model from Visual C++ by using a #import 
statement
http://support.microsoft.com/kb/259298

Importing Type Libraries
http://www.codeproject.com/KB/tips/importtlbs.aspx


/////////////////////////////////////////////////////////////////////////////
