===============================================================================
    MICROSOFT FOUNDATION CLASS LIBRARY : MFCCOMClient Project Overview
===============================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

As we discussed in the sample CppCOMClient, there are basically three ways to 
create and access a COM object in a native client. MFC is one of them. This 
sample demonstrates the following two things:

1. The creation and access of a COM object (more specifically, an ATL out-of-
process COM object in this sample) using MFC and the class wrapper generated 
by the VC++ class wizard.

CATLExeSimpleObjectWrapper.h contains the wrapper class of the COM server;
MFCCreateCOMPage.h/cpp has the codes that use the wrapper class to create and 
access a COM server ATLExeCOMServer; IDD_CREATECOM_PAGE is the property page  
in the dialog that demonstrates the COM client.

2. The creation and access of an ActiveX control using MFC to add the control
to the dialog template.

mfcactivexctrl.h/cpp has the wrapper class of the ActiveX control that is 
inserted into the dialog IDD_ACTIVEXCTRL_PAGE. MFCActiveXCtrlPage.h/cpp 
corresponds to the dialog resource.


/////////////////////////////////////////////////////////////////////////////
Project Relation:

MFCCOMClient -> ATLExeCOMServer
MFCCOMClient is the client application of the COM server ATLExeCOMServer.

MFCCOMClient -> MFCActiveX
MFCCOMClient hosts the ActiveX control MFCActiveX.


/////////////////////////////////////////////////////////////////////////////
Build:

MFCCOMClient depends on ATLExeCOMServer and MFCActiveX. To build and run 
MFCCOMClient successfully, please make sure ATLExeCOMServer and MFCActiveX 
are built and registered rightly.


/////////////////////////////////////////////////////////////////////////////
Creation:

A. Creating the project

Step1. Create a MFC dialog-based project named MFCCOMClient.

Step2. Build the UI of the demo.

 2.1 Add a Tab Control to the main dialog and add a CTabCtrl member, 
 m_tabCtrl, to be bound to the control. 
 
 2.2 Insert two dialog resources IDD_CREATECOM_PAGE, IDD_ACTIVEXCTRL_PAGE and
 set their Style as Child, Border as None. Use the class wizard to create two 
 classes, CMFCCreateCOMPage and CMFCActiveXCtrlPage for the dialog resources
 respectively. 
 
 2.3 In the OnInitDialog method of the main dialog class CMFCCOMClientDlg, 
 setup the tab control, m_tabCtrl, by inserting two tab items and show 
 IDD_CREATECOM_PAGE and IDD_ACTIVEXCTRL_PAGE in them.

B. Creating a COM object using MFC (MFCCreateCOMPage.h/cpp)

Step1. Create the wrapper class to encapsulate the COM class.

 1.1 Right-click the MFCCOMClient project and select Add / Add Class. Double-
 click "MFC Class From TypeLib". 
 
 1.2 Find the type library of ATLExeCOMServer in Add Class From Typelib 
 Wizard, and add the interface ISimpleObject from the Interfaces column 
 to the column of Generated classes. Rename the resulting class and file as 
 CATLExeSimpleObjectWrapper, then click Finish.

This creates the file CATLExeSimpleObjectWrapper.h with the class 
CATLExeSimpleObjectWrapper that encapsulate the COM interface 
ATLExeCOMServer.ISimpleObject.

Step2. Create and access the COM object using the wrapper class.

 2.1 In the class CMFCCreateCOMPage, create a new thread (AfxBeginThread) and
 initialize COM for the thread by calling CoInitializeEx, or CoInitialize.

 2.2 Create an instance of the wrapper class CATLExeSimpleObjectWrapper and 
 use its CreateDispatch() method to create the actual COM object.
 
 2.3 Access the COM object's methods and properties. In order to catch the 
 errors in the execution, add a try {...} catch (COleException *e) {} block.

 2.4 The wrapper class handles the release of the object for us, so we do not
 need to care about it.
 
 2.5 Uninitialize COM for this thread by calling CoUninitialize.

C. Hosting an ActiveX control (MFCActiveXCtrlPage.h/cpp)

Step1. On the dialog IDD_ACTIVEXCTRL_PAGE in which we will add the ActiveX 
control, right-click the mouse and select Insert ActiveX Control.

Step2. Find "MFCActiveX Control" in the Insert ActiveX Control dialog and add
it. A new control appears on IDD_ACTIVEXCTRL_PAGE. Stretch and position the 
control appropriately.

Step3. Right-click the control, and select Add Variable. In Add Member 
Variable Wizard, set Variable type as CMFCActiveXCtrl, enter m_OcxActiveXCtrl
in Variable name and click Finish. This generates the class CMFCActiveXCtrl 
that wraps the ActiveX control in mfcactivexctrl.h/cpp, and adds a variable, 
CMFCActiveXCtrl m_OcxActiveXCtrl, to the class CMFCActiveXCtrlPage.

Step4. Access the properties and methods of the ActiveX control like this:

	FLOAT fProp = m_OcxActiveXCtrl.GetFloatProperty();

Step5. To access the events of the control (e.g. FloatPropertyChanging in 
MFCActiveX), right-click the ActiveX control on IDD_ACTIVEXCTRL_PAGE and 
select Add Event Handler. Rename the function handler of 
FloatPropertyChanging to be FloatPropertyChangingMFCActiveXCtrl and click 
Add and Edit. In the event handler, pop up a message box to allow selecting 
OK or Cancel, then pass the user's selection back to the control through the
Cancel parameter.

	void CMFCActiveXCtrlPage::FloatPropertyChangingMFCActiveXCtrl(
		float NewValue, BOOL* Cancel)
	{
		CString strMessage;
		strMessage.Format(
			_T("FloatProperty is being changed to %f"), NewValue);

		// OK or cancel the change of FloatProperty
		*Cancel = (IDCANCEL == MessageBox(strMessage,
			_T("MFCActiveX!FloatPropertyChanging"), MB_OKCANCEL));
	}


/////////////////////////////////////////////////////////////////////////////
References:

COM Programming by Example: Using MFC, ActiveX, ATL, ADO, and COM+ 
By John E. Swanke
http://www.amazon.com/COM-Programming-Example-ActiveX-CD-ROM/dp/1929629036

Viewing and Adding ActiveX Controls to a Dialog Box
http://msdn.microsoft.com/en-us/library/6e37cay9.aspx

Using MFC to Host a WebBrowser Control
http://msdn.microsoft.com/en-us/library/aa752046.aspx

Using ActiveX Controls Example: Insert Internet Explorer into your Dialogs
By Hazem Nasereddin
http://www.codeproject.com/KB/COM/webbrowser.aspx


/////////////////////////////////////////////////////////////////////////////
