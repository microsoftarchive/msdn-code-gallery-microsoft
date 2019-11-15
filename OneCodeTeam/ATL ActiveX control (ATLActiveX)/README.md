# ATL ActiveX control (ATLActiveX)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- COM
- ATL
## Topics
- ActiveX
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>ACTIVE TEMPLATE LIBRARY : ATLActiveX Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
ATLActiveX demonstrates an ActiveX control written in Active Template Library <br>
(ATL) and Windows Template Library (WTL). WTL extends ATL and provides a set <br>
of classes for controls, dialogs, frame windows, GDI objects, and more. In <br>
the example, we will go through the basic steps of changing the UI and adding <br>
properties, methods, and events to the control.<br>
<br>
ATLActiveX exposes the following items:<br>
<br>
1. An ATL ActiveX control short-named ATLActiveX.<br>
<br>
Program ID: ATLActiveX.ATLActiveXCtrl<br>
CLSID_ATLActiveX: 5A5C9ED1-ECC2-47F7-8015-A304D0DB8EF8<br>
DIID_IATLActiveXCtrl: 299F0541-E940-450C-8726-DC3C1E5A2421<br>
DIID__IATLActiveXCtrlEvents: DF48D1B5-FFE2-42A1-8E66-D3B96E16A9B4<br>
LIBID_MFCActiveXLib: 3A5B4610-4796-4C96-BEB5-AA1B83198B9A<br>
<br>
Dialogs:<br>
&nbsp; &nbsp;// The main dialog of the control<br>
&nbsp; &nbsp;IDD_MAINDIALOG<br>
<br>
Properties:<br>
&nbsp; &nbsp;// With both get and set accessor methods<br>
&nbsp; &nbsp;FLOAT FloatProperty<br>
<br>
Methods:<br>
&nbsp; &nbsp;// HelloWorld returns a BSTR &quot;HelloWorld&quot;<br>
&nbsp; &nbsp;BSTR HelloWorld(void);<br>
&nbsp; &nbsp;// GetProcessThreadID outputs the running process ID and thread ID<br>
&nbsp; &nbsp;void GetProcessThreadID(LONG* pdwProcessId, LONG* pdwThreadId);<br>
<br>
Events:<br>
&nbsp; &nbsp;// FloatPropertyChanging is fired before new value is set to the <br>
&nbsp; &nbsp;// FloatProperty property. The Cancel parameter allows the client to
<br>
&nbsp; &nbsp;// cancel the change of FloatProperty.<br>
&nbsp; &nbsp;void FloatPropertyChanging(FLOAT NewValue, VARIANT_BOOL* Cancel);<br>
<br>
</p>
<h3>Implementation:</h3>
<p style="font-family:Courier New"><br>
A. Creating the project<br>
<br>
Step1. Create a Visual C&#43;&#43; / ATL / ATL Project named ATLActiveX in Visual <br>
Studio 2008.<br>
<br>
Step2. In the page &quot;Application Settings&quot; of ATL Project Wizard, select the
<br>
server type as Dynamic-link library (DLL), and allow merging of proxy/stub <br>
code.<br>
<br>
B. Adding an ATL Control<br>
<br>
Step1. In Class View, right-click the ATLActiveX project. Point to Add on the <br>
shortcut menu, and click Add Class. The Add Class dialog box appears. In the <br>
ATL folder, double-click ATL Control, which starts the ATL Control Wizard.<br>
<br>
Step2. In the Names page of the ATL Control Wizard, type &quot;ATLActiveXCtrl&quot; as
<br>
the short name.<br>
<br>
Step3. In the Options page of the wizard, select the Connection points check <br>
box. This enables the support for events in the control. Also select the <br>
Licensed check box to add support for returning a license key to containers. <br>
Leave the default values of the rest settings in the Options page.<br>
<br>
Step4. The Interfaces page of the wizard specifies the interfaces that the &nbsp;<br>
control will support. Leave the default interfaces in this example.<br>
<br>
Step5. In the Appearance page of the wizard, select the Insertable check box <br>
to make the control insertable, which means it can be embedded into <br>
applications that support embedded objects, such as Excel or Word.<br>
<br>
Step6. Click Stock Properties to open the Stock Properties page. Under Not <br>
supported, scroll down the list of possible stock properties. Double-click <br>
BackColor and Enabled to move them to the Supported list.<br>
<br>
Step7. Click Finish to complete the options for the control. As the wizard <br>
created the control, several code changes and file additions occurred:<br>
<br>
&nbsp; &nbsp;ATLActiveXCtrl.h&nbsp;&nbsp;&nbsp;&nbsp;Contains most of the implementation of ATLActiveXCtrl<br>
&nbsp; &nbsp;ATLActiveXCtrl.cpp&nbsp;&nbsp;&nbsp;&nbsp;Contains the remaining parts of ATLActiveXCtrl<br>
&nbsp; &nbsp;ATLActiveXCtrl.rgs&nbsp;&nbsp;&nbsp;&nbsp;Contains the registry script used to register the
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;control<br>
&nbsp; &nbsp;ATLActiveXCtrl.htm&nbsp;&nbsp;&nbsp;&nbsp;A web page containing a reference to the newly
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;created control<br>
&nbsp; &nbsp;ATLActiveX.idl&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Changed to include details of the new control<br>
&nbsp; &nbsp;ATLActiveX.cpp&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Added the new control to the object map<br>
<br>
C. Changing the UI of the ActiveX Control using WTL<br>
<br>
By default, the control's drawing code displays a square and the text <br>
&quot;ATL 8.0 : ATLActiveXCtrl&quot;. In order to change the UI of the control, we can
<br>
either draw the UI in the OnDraw override <br>
(see: <a target="_blank" href="http://msdn.microsoft.com/en-us/library/hbd0h07f.aspx),">
http://msdn.microsoft.com/en-us/library/hbd0h07f.aspx),</a> or write a <br>
dialog-based ActiveX control (see: <a target="_blank" href="&lt;a target=" href="http://support.microsoft.com/kb/175503">
http://support.microsoft.com/kb/175503</a>).'&gt;<a target="_blank" href="http://support.microsoft.com/kb/175503">http://support.microsoft.com/kb/175503</a>).
<br>
Writing a dialog-based control allows you to take advantage of the Visual C&#43;&#43; <br>
resource editor to lay out contained controls or to reuse existing MFC dialog <br>
box templates and Visual Basic forms. Here is the detailed step list:<br>
<br>
Step1. Declare the class CComDlgCtrl, based on CComControl, which is derived <br>
from CDialogImpl instead of CWindowImpl. (See CComDlgCtrl.h)<br>
<br>
Step2. Derive the ActiveX control, ATLActiveXCtrl, from this new class, <br>
instead of CComControl. <br>
<br>
&nbsp; &nbsp;class ATL_NO_VTABLE CATLActiveXCtrl :<br>
&nbsp; &nbsp;...<br>
&nbsp; &nbsp; &nbsp; &nbsp;public CComDlgCtrl&lt;CATLActiveXCtrl&gt;, // Replaced CComControl<br>
&nbsp; &nbsp;...<br>
<br>
Step3. Create a dialog template with the ID: IDD_MAINDIALOG and make sure its <br>
default properties are set to be Border = None, Style = Child, <br>
System Menu = False, Visible = True.<br>
<br>
Step4. Update class definition to identify this resource.<br>
<br>
&nbsp; &nbsp;class ATL_NO_VTABLE CATLActiveXCtrl :<br>
&nbsp; &nbsp;...<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp;public:<br>
&nbsp; &nbsp; &nbsp; &nbsp;enum { IDD = IDD_MAINDIALOG };<br>
&nbsp; &nbsp;...<br>
<br>
Step5. Declare and implement a message handler for WM_INITDIALOG, and remove <br>
the CHAIN_MSG_MAP entry in message map.<br>
<br>
&nbsp; &nbsp;BEGIN_MSG_MAP(CATLActiveXCtrl)<br>
&nbsp; &nbsp; &nbsp; &nbsp;MESSAGE_HANDLER(WM_INITDIALOG, OnInitDialog)<br>
&nbsp; &nbsp; &nbsp; &nbsp;// CHAIN_MSG_MAP(CComControl&lt;CATLActiveXCtrl&gt;)<br>
&nbsp; &nbsp;END_MSG_MAP()<br>
<br>
&nbsp; &nbsp;LRESULT OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, <br>
&nbsp; &nbsp; &nbsp; &nbsp;BOOL& bHandled)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;InPlaceActivate( OLEIVERB_UIACTIVATE );<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Perform any dialog initialization<br>
&nbsp; &nbsp; &nbsp; &nbsp;return 0;<br>
&nbsp; &nbsp;}<br>
<br>
Step6. Remove the OnDraw declaration and implementation. <br>
<br>
Step7. Depending on how you created the control, you need to set the <br>
m_bWindowOnly variable to 1 in your control's constructor to force the <br>
control to be non-Windowless. <br>
<br>
&nbsp; &nbsp;CATLActiveXCtrl()<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;m_bWindowOnly = 1;<br>
&nbsp; &nbsp;}<br>
<br>
Step8. In order to handle tabbing and other navigation keys correctly, <br>
override IOleInPlaceActiveObjectImpl::TranslateAccelerator. <br>
<br>
&nbsp; &nbsp;IFACEMETHOD(TranslateAccelerator)(MSG *pMsg)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;if ((pMsg-&gt;message &lt; WM_KEYFIRST || <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pMsg-&gt;message &gt; WM_KEYLAST)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&& (pMsg-&gt;message &lt; WM_MOUSEFIRST ||
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pMsg-&gt;message &gt; WM_MOUSELAST))<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return S_FALSE;<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;return (IsDialogMessage(pMsg)) ? S_OK : S_FALSE;<br>
&nbsp; &nbsp;}<br>
<br>
Step9. Add the command handlers for the controls in the dialog resource. The <br>
designer auto-generates the code for the commands in ATLActiveXCtrl.h/cpp. <br>
For example, <br>
<br>
&nbsp; &nbsp;BEGIN_MSG_MAP(CATLActiveXCtrl)<br>
&nbsp; &nbsp; &nbsp; &nbsp;COMMAND_HANDLER(IDC_MSGBOX_BN, BN_CLICKED, OnBnClickedMsgboxBn)<br>
&nbsp; &nbsp;END_MSG_MAP()<br>
<br>
&nbsp; &nbsp;LRESULT CATLActiveXCtrl::OnBnClickedMsgboxBn(WORD /*wNotifyCode*/, <br>
&nbsp; &nbsp; &nbsp; &nbsp;WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;TCHAR szMessage[256];<br>
&nbsp; &nbsp; &nbsp; &nbsp;GetDlgItemText(IDC_MSGBOX_EDIT, szMessage, 256);<br>
&nbsp; &nbsp; &nbsp; &nbsp;MessageBox(szMessage, _T(&quot;HelloWorld&quot;), MB_ICONINFORMATION | MB_OK);<br>
&nbsp; &nbsp; &nbsp; &nbsp;return 0;<br>
&nbsp; &nbsp;}<br>
<br>
D. Adding Properties to the ActiveX Control<br>
<br>
Step1. Right-click IATLActiveXCtrl in Class View of the ATLActiveX project. <br>
Click Add / Add Property on the shortcut menu. <br>
<br>
Step2. In the Add Property Wizard, specify the property type as FLOAT and set <br>
the property name as FloatProperty. Select both Get function and Put function. <br>
ATLActiveXCtrl therefore exposes FloatProperty with the get&put accessor<br>
methods: get_FloatProperty, put_FloatProperty.<br>
<br>
Step3. Add a float field, m_fField, to the class CATLActiveXCtrl:<br>
<br>
&nbsp; &nbsp;protected:<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Used by FloatProperty<br>
&nbsp; &nbsp; &nbsp; &nbsp;float m_fField;<br>
<br>
Implement the get&put accessor methods of FloatProperty to access m_fField.<br>
<br>
&nbsp; &nbsp;IFACEMETHODIMP CATLActiveXCtrl::get_FloatProperty(FLOAT* pVal)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;*pVal = m_fField;<br>
&nbsp; &nbsp; &nbsp; &nbsp;return S_OK;<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp;IFACEMETHODIMP CATLActiveXCtrl::put_FloatProperty(FLOAT newVal)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;m_fField = newVal;<br>
&nbsp; &nbsp; &nbsp; &nbsp;return S_OK;<br>
&nbsp; &nbsp;}<br>
<br>
E. Adding Methods to the ActiveX Control<br>
<br>
Step1. In Class View, find the interface IATLActiveXCtrl. Right-click it and <br>
select Add / Add Method on the shortcut menu.<br>
<br>
Step2. In the Add Method Wizard, specify the method name as HelloWorld. Add <br>
a parameter whose parameter attributes = retval, parameter type = BSTR*, <br>
and parameter name = pRet.<br>
<br>
Step3. Write the body of HelloWorld as this:<br>
<br>
&nbsp; &nbsp;IFACEMETHODIMP CATLActiveXCtrl::HelloWorld(BSTR* pRet)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Allocate memory for the string: <br>
&nbsp; &nbsp; &nbsp; &nbsp;*pRet = ::SysAllocString(L&quot;HelloWorld&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (pRet == NULL)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return E_OUTOFMEMORY;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// The client is now responsible for freeing pbstr<br>
&nbsp; &nbsp; &nbsp; &nbsp;return S_OK;<br>
&nbsp; &nbsp;}<br>
<br>
With the almost same steps, the method GetProcessThreadID is added to get the<br>
executing process ID and thread ID.<br>
<br>
HRESULT GetProcessThreadID([out] LONG* pdwProcessId, [out] LONG* pdwThreadId);<br>
<br>
F. Adding Events to the ActiveX Control<br>
<br>
The Connection points option in B/Step3 is the prerequisite for the control <br>
to supporting events.<br>
<br>
Step1. In Class View, expand ATLActiveX / ATLActiveXLib to display<br>
_IATLActiveXCtrlEvents.<br>
<br>
Step2. Right-click _IATLActiveXCtrlEvents. On the shortcut menu, click Add, &nbsp;<br>
and then click Add Method.<br>
<br>
Step3. Select a Return Type of void, enter FloatPropertyChanging in the<br>
Method name box, and add an [in] parameter FLOAT NewValue, and an [in, out] <br>
parameter VARIANT_BOOL* Cancel. After clicking Finish, _IATLActiveXCtrlEvents <br>
dispinterface in the ATLActiveX.idl file should look like this: <br>
<br>
&nbsp; &nbsp;dispinterface _IATLActiveXCtrlEvents<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;properties:<br>
&nbsp; &nbsp; &nbsp; &nbsp;methods:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;[id(1), helpstring(&quot;method FloatPropertyChanging&quot;)] void
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;FloatPropertyChanging(<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;[in] FLOAT NewValue, [in,out] VARIANT_BOOL* Cancel);<br>
&nbsp; &nbsp;};<br>
<br>
Step4. Generate the type library by rebuilding the project or by <br>
right-clicking the ATLActiveX.idl file in Solution Explorer and clicking<br>
Compile on the shortcut menu. Please note: We must compile the IDL file <br>
BEFORE setting up a connection point.<br>
<br>
Step5. Use the Implement Connection Point Wizard to implement the Connection<br>
Point interface: In Class View, right-click the component's implementation <br>
class CATLActiveXCtrl. On the shortcut menu, click Add, and then click <br>
Add Connection Point. Select _IATLActiveXCtrlEvents from the Source <br>
Interfaces list and double-click it to add it to the Implement connection <br>
points column. Click Finish. A proxy class for the connection point will be <br>
generated (ie. CProxy_IATLActiveXCtrlEvents in this example) in the file <br>
_IATLActiveXCtrlEvents_CP.h. This also creates a function with the name<br>
Fire_[eventname] that can be called to raise events in the client. <br>
<br>
Step6. Fire the event in put_FloatProperty:<br>
<br>
&nbsp; &nbsp;IFACEMETHODIMP CATLActiveXCtrl::put_FloatProperty(FLOAT newVal)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Fire the event, FloatPropertyChanging<br>
&nbsp; &nbsp; &nbsp; &nbsp;VARIANT_BOOL cancel = VARIANT_FALSE; <br>
&nbsp; &nbsp; &nbsp; &nbsp;Fire_FloatPropertyChanging(newVal, &cancel);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (cancel == VARIANT_FALSE)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;m_fField = newVal;&nbsp;&nbsp;&nbsp;&nbsp;// Save the new value<br>
&nbsp; &nbsp; &nbsp; &nbsp;} // else, do nothing<br>
&nbsp; &nbsp; &nbsp; &nbsp;return S_OK;<br>
&nbsp; &nbsp;}<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: ATL Tutorial<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/599w5e7x.aspx">http://msdn.microsoft.com/en-us/library/599w5e7x.aspx</a><br>
<br>
Creating ActiveX Components in C&#43;&#43;<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms974283.aspx">http://msdn.microsoft.com/en-us/library/ms974283.aspx</a><br>
<br>
Create an ActiveX control using ATL that you can use from Fox, Excel, VB6, VB.Net<br>
<a target="_blank" href="http://blogs.msdn.com/calvin_hsia/archive/2006/08/28/729165.aspx">http://blogs.msdn.com/calvin_hsia/archive/2006/08/28/729165.aspx</a><br>
<br>
A Complete Scriptable ActiveX Web Control Tutorial Using ATL<br>
<a target="_blank" href="http://www.codeguru.com/cpp/com-tech/atl/tutorials/article.php/c14599">http://www.codeguru.com/cpp/com-tech/atl/tutorials/article.php/c14599</a><br>
<br>
How To Write a Dialog-based ActiveX Control Using ATL<br>
<a target="_blank" href="http://support.microsoft.com/kb/175503">http://support.microsoft.com/kb/175503</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
