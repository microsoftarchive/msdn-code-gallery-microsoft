========================================================================
    ACTIVEX CONTROL DLL : MFCActiveX Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

MFCActiveX demonstrates an ActiveX control written in Microsoft Foundation 
Classes (MFC). ActiveX controls (formerly known as OLE controls) are small 
program building blocks that can work in a variety of different containers, 
ranging from software development tools to end-user productivity tools. For 
example, it can be used to create distributed applications that work over 
the Internet through web browsers. ActiveX controls can be written in MFC, 
ATL, C++, C#, Borland Delphi and Visual Basic. In this sample, we focus on 
writing an ActiveX control using MFC. We will go through the basic steps of 
adding a main dialog, properties, methods, and events to the control.

MFCActiveX exposes the following items:

1. A MFC ActiveX control short-named MFCActiveX.

Program ID: MFCACTIVEX.MFCActiveXCtrl.1
CLSID_MFCActiveX: E389AD6C-4FB6-47AF-B03A-A5A5C6B2B820
DIID__DMFCActiveX: 0327DD42-7A9E-415B-B9A0-4AEEE1A3319E
DIID__DMFCActiveXEvents: 97B9B2F3-E95A-49D4-ACA3-E2A181424FD8
LIBID_MFCActiveXLib: DFFC673C-E5FE-4D0D-99CA-6FB4BDCF0A50

Dialogs:
// The main dialog of the control
IDD_MAINDIALOG
// The property page of the control
IDD_PROPPAGE_MFCACTIVEX

Properties:
// With both get and set accessor methods
FLOAT FloatProperty

Methods:
// HelloWorld returns a BSTR "HelloWorld"
BSTR HelloWorld(void);
// GetProcessThreadID outputs the running process ID and thread ID
void GetProcessThreadID(LONG* pdwProcessId, LONG* pdwThreadId);

Events:
// FloatPropertyChanging is fired before new value is set to the 
// FloatProperty property. The Cancel parameter allows the client to cancel  
// the change of FloatProperty.
void FloatPropertyChanging(FLOAT NewValue, VARIANT_BOOL* Cancel);


/////////////////////////////////////////////////////////////////////////////
Project Relation:

MFCCOMClient -> MFCActiveX
MFCCOMClient demonstrates the use of the MFC ActiveX control. 

MFCActiveX - CSActiveX - VBActiveX
These samples expose the same UI and the same set of properties, methods, and
events, but they are implemented in different languages.


/////////////////////////////////////////////////////////////////////////////
Build:

To build MFCActiveX, 1. run Visual Studio as administrator because the 
control needs to be registered into HKCR. 2. Be sure to build the MFCActiveX 
project using the Release configuration!


/////////////////////////////////////////////////////////////////////////////
Creation:

A. Creating the project

Step1. Create a Visual C++ / MFC / MFC ActiveX Control project named 
MFCActiveX in Visual Studio 2008.

Step2. In the page "Control Settings", select "Create control based on" as 
STATIC. Under "Additional features", check "Activates when visible" and 
"Flicker-free activation", and un-check "Has an About box dialog".

B. Adding a main dialog to the control

Step1. In Resource View, insert a new dialog resource and change the control 
ID to IDD_MAINDIALOG.

Step2. Change the default properties of the dialog to Border - None, 
Style - Child, System Menu - False, Visible - True.

Step3. Create a class for the dialog, by right clicking on the dialog and 
selecting Add Class. Name the class CMainDialog, with the base class CDialog.

Step4. Add the member variable m_MainDialog of the type CMainDialog to the 
class CMFCActiveXCtrl.

Step5. Select the class CMFCActiveXCtrl in Class View. In the Properties 
sheet, select the Messages icon. Add OnCreate for the WM_CREATE message. 

Step6. Open MFCActiveXCtrl.cpp, and add the following code to the OnCreate 
method to create the main dialog.

	m_MainDialog.Create(IDD_MAINDIALOG, this);

Step7. Add the following code to the OnDraw method to size the main dialog 
window and fill the background.

	m_MainDialog.MoveWindow(rcBounds, TRUE);
	CBrush brBackGnd(TranslateColor(AmbientBackColor()));
	pdc->FillRect(rcBounds, &brBackGnd);

C. Adding Properties to the ActiveX control

Step1. In Class View, expand the element MFCActiveXLib. Right click on 
_DMFCActiveX, and click on Add, Add Property. In the Add Property Wizard 
dialog, select FLOAT for Property type, and enter "FloatProperty" for 
property name. Select "Get/Set methods" to create the methods 
GetFloatProperty and SetFloatProperty.

Step2. In the class CMFCActiveXCtrl, add a member variable, 
FLOAT m_FloatField. In the class's contructor, set the variable's default 
value to 0.0f.

Step3. Associate the Get/Set methods of FloatProperty with m_FloatField.

D. Adding Methods to the ActiveX control

Step3. In Class View, expand the element MFCActiveXLib. Right click on 
_DMFCActiveX, and click on Add, Add Method. In the Add Method Wizard 
dialog, select BSTR for the return type, and enter "HelloWorld" for Method 
name.

With the almost same steps, the method GetProcessThreadID is added to get the
executing process ID and thread ID:

	void GetProcessThreadID(LONG* pdwProcessId, LONG* pdwThreadId);

E. Adding Events to the ActiveX control

Step1. In Class View, right click on CMFCActiveXCtrl, select Add, Add Event. 
In the Add Event Wizard, enter "FloatPropertyChanging" for Event name and add
two parameters: FLOAT NewValue, VARIANT_BOOL* Cancel. 

Step2. The event "FloatPropertyChanging" is fired in SetFloatProperty:

	void CMFCActiveXCtrl::SetFloatProperty(FLOAT newVal)
	{
		AFX_MANAGE_STATE(AfxGetStaticModuleState());

		// Fire the event, FloatPropertyChanging
		VARIANT_BOOL cancel = VARIANT_FALSE; 
		FloatPropertyChanging(newVal, &cancel);

		if (cancel == VARIANT_FALSE)
		{
			m_fField = newVal;	// Save the new value
			SetModifiedFlag();

			// Display the new value in the control UI
			CString strFloatProp;
			strFloatProp.Format(_T("%f"), m_fField);
			m_MainDialog.m_StaticFloatProperty.SetWindowTextW(strFloatProp);
		}
		// else, do nothing.
	}


/////////////////////////////////////////////////////////////////////////////
References:

The ABCs of MFC ActiveX Controls
http://msdn.microsoft.com/en-us/library/ms968497.aspx

A Complete ActiveX Web Control Tutorial By David Marcionek
http://www.codeproject.com/KB/COM/CompleteActiveX.aspx


/////////////////////////////////////////////////////////////////////////////
