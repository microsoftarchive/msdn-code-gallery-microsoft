# MFC app invokes COM server and ActiveX control (MFCCOMClient)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- COM
- MFC
## Topics
- COM Client
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>MICROSOFT FOUNDATION CLASS LIBRARY : MFCCOMClient Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
As we discussed in the sample CppCOMClient, there are basically three ways to <br>
create and access a COM object in a native client. MFC is one of them. This <br>
sample demonstrates the following two things:<br>
<br>
1. The creation and access of a COM object (more specifically, an ATL out-of-<br>
process COM object in this sample) using MFC and the class wrapper generated <br>
by the VC&#43;&#43; class wizard.<br>
<br>
CATLExeSimpleObjectWrapper.h contains the wrapper class of the COM server;<br>
MFCCreateCOMPage.h/cpp has the codes that use the wrapper class to create and <br>
access a COM server ATLExeCOMServer; IDD_CREATECOM_PAGE is the property page &nbsp;<br>
in the dialog that demonstrates the COM client.<br>
<br>
2. The creation and access of an ActiveX control using MFC to add the control<br>
to the dialog template.<br>
<br>
mfcactivexctrl.h/cpp has the wrapper class of the ActiveX control that is <br>
inserted into the dialog IDD_ACTIVEXCTRL_PAGE. MFCActiveXCtrlPage.h/cpp <br>
corresponds to the dialog resource.<br>
<br>
</p>
<h3>Project Relation:</h3>
<p style="font-family:Courier New"><br>
MFCCOMClient -&gt; ATLExeCOMServer<br>
MFCCOMClient is the client application of the COM server ATLExeCOMServer.<br>
<br>
MFCCOMClient -&gt; MFCActiveX<br>
MFCCOMClient hosts the ActiveX control MFCActiveX.<br>
<br>
</p>
<h3>Build:</h3>
<p style="font-family:Courier New"><br>
MFCCOMClient depends on ATLExeCOMServer and MFCActiveX. To build and run <br>
MFCCOMClient successfully, please make sure ATLExeCOMServer and MFCActiveX <br>
are built and registered rightly.<br>
<br>
</p>
<h3>Creation:</h3>
<p style="font-family:Courier New"><br>
A. Creating the project<br>
<br>
Step1. Create a MFC dialog-based project named MFCCOMClient.<br>
<br>
Step2. Build the UI of the demo.<br>
<br>
2.1 Add a Tab Control to the main dialog and add a CTabCtrl member, <br>
m_tabCtrl, to be bound to the control. <br>
<br>
2.2 Insert two dialog resources IDD_CREATECOM_PAGE, IDD_ACTIVEXCTRL_PAGE and<br>
set their Style as Child, Border as None. Use the class wizard to create two <br>
classes, CMFCCreateCOMPage and CMFCActiveXCtrlPage for the dialog resources<br>
respectively. <br>
<br>
2.3 In the OnInitDialog method of the main dialog class CMFCCOMClientDlg, <br>
setup the tab control, m_tabCtrl, by inserting two tab items and show <br>
IDD_CREATECOM_PAGE and IDD_ACTIVEXCTRL_PAGE in them.<br>
<br>
B. Creating a COM object using MFC (MFCCreateCOMPage.h/cpp)<br>
<br>
Step1. Create the wrapper class to encapsulate the COM class.<br>
<br>
1.1 Right-click the MFCCOMClient project and select Add / Add Class. Double-<br>
click &quot;MFC Class From TypeLib&quot;. <br>
<br>
1.2 Find the type library of ATLExeCOMServer in Add Class From Typelib <br>
Wizard, and add the interface ISimpleObject from the Interfaces column <br>
to the column of Generated classes. Rename the resulting class and file as <br>
CATLExeSimpleObjectWrapper, then click Finish.<br>
<br>
This creates the file CATLExeSimpleObjectWrapper.h with the class <br>
CATLExeSimpleObjectWrapper that encapsulate the COM interface <br>
ATLExeCOMServer.ISimpleObject.<br>
<br>
Step2. Create and access the COM object using the wrapper class.<br>
<br>
2.1 In the class CMFCCreateCOMPage, create a new thread (AfxBeginThread) and<br>
initialize COM for the thread by calling CoInitializeEx, or CoInitialize.<br>
<br>
2.2 Create an instance of the wrapper class CATLExeSimpleObjectWrapper and <br>
use its CreateDispatch() method to create the actual COM object.<br>
<br>
2.3 Access the COM object's methods and properties. In order to catch the <br>
errors in the execution, add a try {...} catch (COleException *e) {} block.<br>
<br>
2.4 The wrapper class handles the release of the object for us, so we do not<br>
need to care about it.<br>
<br>
2.5 Uninitialize COM for this thread by calling CoUninitialize.<br>
<br>
C. Hosting an ActiveX control (MFCActiveXCtrlPage.h/cpp)<br>
<br>
Step1. On the dialog IDD_ACTIVEXCTRL_PAGE in which we will add the ActiveX <br>
control, right-click the mouse and select Insert ActiveX Control.<br>
<br>
Step2. Find &quot;MFCActiveX Control&quot; in the Insert ActiveX Control dialog and add<br>
it. A new control appears on IDD_ACTIVEXCTRL_PAGE. Stretch and position the <br>
control appropriately.<br>
<br>
Step3. Right-click the control, and select Add Variable. In Add Member <br>
Variable Wizard, set Variable type as CMFCActiveXCtrl, enter m_OcxActiveXCtrl<br>
in Variable name and click Finish. This generates the class CMFCActiveXCtrl <br>
that wraps the ActiveX control in mfcactivexctrl.h/cpp, and adds a variable, <br>
CMFCActiveXCtrl m_OcxActiveXCtrl, to the class CMFCActiveXCtrlPage.<br>
<br>
Step4. Access the properties and methods of the ActiveX control like this:<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;FLOAT fProp = m_OcxActiveXCtrl.GetFloatProperty();<br>
<br>
Step5. To access the events of the control (e.g. FloatPropertyChanging in <br>
MFCActiveX), right-click the ActiveX control on IDD_ACTIVEXCTRL_PAGE and <br>
select Add Event Handler. Rename the function handler of <br>
FloatPropertyChanging to be FloatPropertyChangingMFCActiveXCtrl and click <br>
Add and Edit. In the event handler, pop up a message box to allow selecting <br>
OK or Cancel, then pass the user's selection back to the control through the<br>
Cancel parameter.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;void CMFCActiveXCtrlPage::FloatPropertyChangingMFCActiveXCtrl(<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;float NewValue, BOOL* Cancel)<br>
&nbsp;&nbsp;&nbsp;&nbsp;{<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CString strMessage;<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;strMessage.Format(<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_T(&quot;FloatProperty is being changed to %f&quot;), NewValue);<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// OK or cancel the change of FloatProperty<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;*Cancel = (IDCANCEL == MessageBox(strMessage,<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_T(&quot;MFCActiveX!FloatPropertyChanging&quot;), MB_OKCANCEL));<br>
&nbsp;&nbsp;&nbsp;&nbsp;}<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
COM Programming by Example: Using MFC, ActiveX, ATL, ADO, and COM&#43; <br>
By John E. Swanke<br>
<a target="_blank" href="http://www.amazon.com/COM-Programming-Example-ActiveX-CD-ROM/dp/1929629036">http://www.amazon.com/COM-Programming-Example-ActiveX-CD-ROM/dp/1929629036</a><br>
<br>
Viewing and Adding ActiveX Controls to a Dialog Box<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/6e37cay9.aspx">http://msdn.microsoft.com/en-us/library/6e37cay9.aspx</a><br>
<br>
Using MFC to Host a WebBrowser Control<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/aa752046.aspx">http://msdn.microsoft.com/en-us/library/aa752046.aspx</a><br>
<br>
Using ActiveX Controls Example: Insert Internet Explorer into your Dialogs<br>
By Hazem Nasereddin<br>
<a target="_blank" href="http://www.codeproject.com/KB/COM/webbrowser.aspx">http://www.codeproject.com/KB/COM/webbrowser.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
