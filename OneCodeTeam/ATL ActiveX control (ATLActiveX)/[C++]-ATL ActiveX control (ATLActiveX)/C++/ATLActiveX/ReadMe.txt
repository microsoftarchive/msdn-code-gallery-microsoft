=============================================================================
         ACTIVE TEMPLATE LIBRARY : ATLActiveX Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

ATLActiveX demonstrates an ActiveX control written in Active Template Library 
(ATL) and Windows Template Library (WTL). WTL extends ATL and provides a set 
of classes for controls, dialogs, frame windows, GDI objects, and more. In 
the example, we will go through the basic steps of changing the UI and adding 
properties, methods, and events to the control.

ATLActiveX exposes the following items:

1. An ATL ActiveX control short-named ATLActiveX.

Program ID: ATLActiveX.ATLActiveXCtrl
CLSID_ATLActiveX: 5A5C9ED1-ECC2-47F7-8015-A304D0DB8EF8
DIID_IATLActiveXCtrl: 299F0541-E940-450C-8726-DC3C1E5A2421
DIID__IATLActiveXCtrlEvents: DF48D1B5-FFE2-42A1-8E66-D3B96E16A9B4
LIBID_MFCActiveXLib: 3A5B4610-4796-4C96-BEB5-AA1B83198B9A

Dialogs:
    // The main dialog of the control
    IDD_MAINDIALOG

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
    // FloatProperty property. The Cancel parameter allows the client to 
    // cancel the change of FloatProperty.
    void FloatPropertyChanging(FLOAT NewValue, VARIANT_BOOL* Cancel);


/////////////////////////////////////////////////////////////////////////////
Implementation:

A. Creating the project

Step1. Create a Visual C++ / ATL / ATL Project named ATLActiveX in Visual 
Studio 2008.

Step2. In the page "Application Settings" of ATL Project Wizard, select the 
server type as Dynamic-link library (DLL), and allow merging of proxy/stub 
code.

B. Adding an ATL Control

Step1. In Class View, right-click the ATLActiveX project. Point to Add on the 
shortcut menu, and click Add Class. The Add Class dialog box appears. In the 
ATL folder, double-click ATL Control, which starts the ATL Control Wizard.

Step2. In the Names page of the ATL Control Wizard, type "ATLActiveXCtrl" as 
the short name.

Step3. In the Options page of the wizard, select the Connection points check 
box. This enables the support for events in the control. Also select the 
Licensed check box to add support for returning a license key to containers. 
Leave the default values of the rest settings in the Options page.

Step4. The Interfaces page of the wizard specifies the interfaces that the  
control will support. Leave the default interfaces in this example.

Step5. In the Appearance page of the wizard, select the Insertable check box 
to make the control insertable, which means it can be embedded into 
applications that support embedded objects, such as Excel or Word.

Step6. Click Stock Properties to open the Stock Properties page. Under Not 
supported, scroll down the list of possible stock properties. Double-click 
BackColor and Enabled to move them to the Supported list.

Step7. Click Finish to complete the options for the control. As the wizard 
created the control, several code changes and file additions occurred:

    ATLActiveXCtrl.h	Contains most of the implementation of ATLActiveXCtrl
    ATLActiveXCtrl.cpp	Contains the remaining parts of ATLActiveXCtrl
    ATLActiveXCtrl.rgs	Contains the registry script used to register the 
                        control
    ATLActiveXCtrl.htm	A web page containing a reference to the newly 
                        created control
    ATLActiveX.idl		Changed to include details of the new control
    ATLActiveX.cpp		Added the new control to the object map

C. Changing the UI of the ActiveX Control using WTL

By default, the control's drawing code displays a square and the text 
"ATL 8.0 : ATLActiveXCtrl". In order to change the UI of the control, we can 
either draw the UI in the OnDraw override 
(see: http://msdn.microsoft.com/en-us/library/hbd0h07f.aspx), or write a 
dialog-based ActiveX control (see: http://support.microsoft.com/kb/175503). 
Writing a dialog-based control allows you to take advantage of the Visual C++ 
resource editor to lay out contained controls or to reuse existing MFC dialog 
box templates and Visual Basic forms. Here is the detailed step list:

Step1. Declare the class CComDlgCtrl, based on CComControl, which is derived 
from CDialogImpl instead of CWindowImpl. (See CComDlgCtrl.h)

Step2. Derive the ActiveX control, ATLActiveXCtrl, from this new class, 
instead of CComControl. 

    class ATL_NO_VTABLE CATLActiveXCtrl :
    ...
        public CComDlgCtrl<CATLActiveXCtrl>, // Replaced CComControl
    ...

Step3. Create a dialog template with the ID: IDD_MAINDIALOG and make sure its 
default properties are set to be Border = None, Style = Child, 
System Menu = False, Visible = True.

Step4. Update class definition to identify this resource.

    class ATL_NO_VTABLE CATLActiveXCtrl :
    ...
    {
    public:
        enum { IDD = IDD_MAINDIALOG };
    ...

Step5. Declare and implement a message handler for WM_INITDIALOG, and remove 
the CHAIN_MSG_MAP entry in message map.

    BEGIN_MSG_MAP(CATLActiveXCtrl)
        MESSAGE_HANDLER(WM_INITDIALOG, OnInitDialog)
        // CHAIN_MSG_MAP(CComControl<CATLActiveXCtrl>)
    END_MSG_MAP()

    LRESULT OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, 
        BOOL& bHandled)
    {
        InPlaceActivate( OLEIVERB_UIACTIVATE );
        // Perform any dialog initialization
        return 0;
    }

Step6. Remove the OnDraw declaration and implementation. 

Step7. Depending on how you created the control, you need to set the 
m_bWindowOnly variable to 1 in your control's constructor to force the 
control to be non-Windowless. 

    CATLActiveXCtrl()
    {
        m_bWindowOnly = 1;
    }

Step8. In order to handle tabbing and other navigation keys correctly, 
override IOleInPlaceActiveObjectImpl::TranslateAccelerator. 

    IFACEMETHOD(TranslateAccelerator)(MSG *pMsg)
    {
        if ((pMsg->message < WM_KEYFIRST || 
            pMsg->message > WM_KEYLAST)
            && (pMsg->message < WM_MOUSEFIRST || 
            pMsg->message > WM_MOUSELAST))
        {
            return S_FALSE;
        }
        return (IsDialogMessage(pMsg)) ? S_OK : S_FALSE;
    }

Step9. Add the command handlers for the controls in the dialog resource. The 
designer auto-generates the code for the commands in ATLActiveXCtrl.h/cpp. 
For example, 

    BEGIN_MSG_MAP(CATLActiveXCtrl)
        COMMAND_HANDLER(IDC_MSGBOX_BN, BN_CLICKED, OnBnClickedMsgboxBn)
    END_MSG_MAP()

    LRESULT CATLActiveXCtrl::OnBnClickedMsgboxBn(WORD /*wNotifyCode*/, 
        WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
    {
        TCHAR szMessage[256];
        GetDlgItemText(IDC_MSGBOX_EDIT, szMessage, 256);
        MessageBox(szMessage, _T("HelloWorld"), MB_ICONINFORMATION | MB_OK);
        return 0;
    }

D. Adding Properties to the ActiveX Control

Step1. Right-click IATLActiveXCtrl in Class View of the ATLActiveX project. 
Click Add / Add Property on the shortcut menu. 

Step2. In the Add Property Wizard, specify the property type as FLOAT and set 
the property name as FloatProperty. Select both Get function and Put function. 
ATLActiveXCtrl therefore exposes FloatProperty with the get&put accessor
methods: get_FloatProperty, put_FloatProperty.

Step3. Add a float field, m_fField, to the class CATLActiveXCtrl:

    protected:
        // Used by FloatProperty
        float m_fField;

Implement the get&put accessor methods of FloatProperty to access m_fField.

    IFACEMETHODIMP CATLActiveXCtrl::get_FloatProperty(FLOAT* pVal)
    {
        *pVal = m_fField;
        return S_OK;
    }

    IFACEMETHODIMP CATLActiveXCtrl::put_FloatProperty(FLOAT newVal)
    {
        m_fField = newVal;
        return S_OK;
    }

E. Adding Methods to the ActiveX Control

Step1. In Class View, find the interface IATLActiveXCtrl. Right-click it and 
select Add / Add Method on the shortcut menu.

Step2. In the Add Method Wizard, specify the method name as HelloWorld. Add 
a parameter whose parameter attributes = retval, parameter type = BSTR*, 
and parameter name = pRet.

Step3. Write the body of HelloWorld as this:

    IFACEMETHODIMP CATLActiveXCtrl::HelloWorld(BSTR* pRet)
    {
        // Allocate memory for the string: 
        *pRet = ::SysAllocString(L"HelloWorld");
        if (pRet == NULL)
            return E_OUTOFMEMORY;

        // The client is now responsible for freeing pbstr
        return S_OK;
    }

With the almost same steps, the method GetProcessThreadID is added to get the
executing process ID and thread ID.

HRESULT GetProcessThreadID([out] LONG* pdwProcessId, [out] LONG* pdwThreadId);

F. Adding Events to the ActiveX Control

The Connection points option in B/Step3 is the prerequisite for the control 
to supporting events.

Step1. In Class View, expand ATLActiveX / ATLActiveXLib to display
_IATLActiveXCtrlEvents.

Step2. Right-click _IATLActiveXCtrlEvents. On the shortcut menu, click Add,  
and then click Add Method.

Step3. Select a Return Type of void, enter FloatPropertyChanging in the
Method name box, and add an [in] parameter FLOAT NewValue, and an [in, out] 
parameter VARIANT_BOOL* Cancel. After clicking Finish, _IATLActiveXCtrlEvents 
dispinterface in the ATLActiveX.idl file should look like this: 

    dispinterface _IATLActiveXCtrlEvents
    {
        properties:
        methods:
            [id(1), helpstring("method FloatPropertyChanging")] void 
            FloatPropertyChanging(
            [in] FLOAT NewValue, [in,out] VARIANT_BOOL* Cancel);
    };

Step4. Generate the type library by rebuilding the project or by 
right-clicking the ATLActiveX.idl file in Solution Explorer and clicking
Compile on the shortcut menu. Please note: We must compile the IDL file 
BEFORE setting up a connection point.

Step5. Use the Implement Connection Point Wizard to implement the Connection
Point interface: In Class View, right-click the component's implementation 
class CATLActiveXCtrl. On the shortcut menu, click Add, and then click 
Add Connection Point. Select _IATLActiveXCtrlEvents from the Source 
Interfaces list and double-click it to add it to the Implement connection 
points column. Click Finish. A proxy class for the connection point will be 
generated (ie. CProxy_IATLActiveXCtrlEvents in this example) in the file 
_IATLActiveXCtrlEvents_CP.h. This also creates a function with the name
Fire_[eventname] that can be called to raise events in the client. 

Step6. Fire the event in put_FloatProperty:

    IFACEMETHODIMP CATLActiveXCtrl::put_FloatProperty(FLOAT newVal)
    {
        // Fire the event, FloatPropertyChanging
        VARIANT_BOOL cancel = VARIANT_FALSE; 
        Fire_FloatPropertyChanging(newVal, &cancel);

        if (cancel == VARIANT_FALSE)
        {
            m_fField = newVal;	// Save the new value
        } // else, do nothing
        return S_OK;
    }


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: ATL Tutorial
http://msdn.microsoft.com/en-us/library/599w5e7x.aspx

Creating ActiveX Components in C++
http://msdn.microsoft.com/en-us/library/ms974283.aspx

Create an ActiveX control using ATL that you can use from Fox, Excel, VB6, VB.Net
http://blogs.msdn.com/calvin_hsia/archive/2006/08/28/729165.aspx

A Complete Scriptable ActiveX Web Control Tutorial Using ATL
http://www.codeguru.com/cpp/com-tech/atl/tutorials/article.php/c14599

How To Write a Dialog-based ActiveX Control Using ATL
http://support.microsoft.com/kb/175503


/////////////////////////////////////////////////////////////////////////////