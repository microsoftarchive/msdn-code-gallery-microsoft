# MFC ActiveX control (MFCActiveX)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- COM
- MFC
- Windows SDK
## Topics
- ActiveX
## Updated
- 03/04/2012
## Description

<h1><span style="font-family:������">ACTIVEX CONTROL DLL</span> (<span style="font-family:������">MFCActiveX</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">MFCActiveX demonstrates an ActiveX control written in Microsoft Foundation Classes (MFC). ActiveX controls (formerly known as OLE controls) are small program building blocks that can work in a variety of different containers, ranging
 from software development tools to end-user productivity tools. For example, it can be used to create distributed applications that work over the Internet through web browsers. ActiveX controls can be written in MFC, ATL, C&#43;&#43;, C#, Borland Delphi and Visual
 Basic. In this sample, we focus on writing an ActiveX control using MFC. We will go through the basic steps of
</p>
<p class="MsoNormal">adding a main dialog, properties, methods, and events to the control.
<span style=""></span></p>
<h2><span style="">Compiling the code </span></h2>
<p class="MsoNormal"><span style="">1. run Visual Studio as administrator because the control needs to be registered into HKCR.
</span></p>
<p class="MsoNormal"><span style="">2. Be sure to build the MFCActiveX project using the Release configuration!
</span></p>
<h2>Using the Code</h2>
<h3><span style="">A. Creating the project </span></h3>
<p class="MsoNormal">Step1. Create a Visual C&#43;&#43; / MFC / MFC ActiveX Control project named MFCActiveX in Visual Studio 2008.
</p>
<p class="MsoNormal">Step2. <span class="GramE">In the page &quot;Control Settings&quot;, select &quot;Create control based on&quot; as STATIC.</span> Under &quot;Additional features&quot;, check &quot;Activates when visible&quot; and &quot;Flicker-free
 activation&quot;, and un-check &quot;Has an <span class="GramE">About</span> box dialog&quot;.
</p>
<h3><span style="">B. Adding a main dialog to the control </span></h3>
<p class="MsoNormal">Step1. In Resource View, insert a new dialog resource and change the control ID to IDD_MAINDIALOG.
</p>
<p class="MsoNormal">Step2. Change the default properties of the dialog to Border - None, Style - Child, System Menu - False, Visible - True.
</p>
<p class="MsoNormal">Step3. Create a class for the dialog, by right clicking on the dialog and selecting Add Class. Name the class CMainDialog, with the base class CDialog.
</p>
<p class="MsoNormal">Step4. Add the member variable m_MainDialog of the type CMainDialog to the class CMFCActiveXCtrl.
</p>
<p class="MsoNormal">Step5. Select the class CMFCActiveXCtrl in Class View. In the Properties sheet, select the Messages icon. Add OnCreate for the WM_CREATE message.
</p>
<p class="MsoNormal">Step6. Open MFCActiveXCtrl.cpp, and add the following code to the OnCreate method to create the main dialog.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
int CMFCActiveXCtrl::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (COleControl::OnCreate(lpCreateStruct) == -1)
        return -1;


    // To create the main dialog
    m_MainDialog.Create(IDD_MAINDIALOG, this);


    return 0;
}

</pre>
<pre id="codePreview" class="cplusplus">
int CMFCActiveXCtrl::OnCreate(LPCREATESTRUCT lpCreateStruct)
{
    if (COleControl::OnCreate(lpCreateStruct) == -1)
        return -1;


    // To create the main dialog
    m_MainDialog.Create(IDD_MAINDIALOG, this);


    return 0;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step7. Add the following code to the OnDraw method to size the main dialog window and fill the background.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
void CMFCActiveXCtrl::OnDraw(
            CDC* pdc, const CRect& rcBounds, const CRect& rcInvalid)
{
    if (!pdc)
        return;


    // To size the main dialog window and fill the background
    m_MainDialog.MoveWindow(rcBounds, TRUE);
    CBrush brBackGnd(TranslateColor(AmbientBackColor()));
    pdc-&gt;FillRect(rcBounds, &brBackGnd);


    DoSuperclassPaint(pdc, rcBounds);
}

</pre>
<pre id="codePreview" class="cplusplus">
void CMFCActiveXCtrl::OnDraw(
            CDC* pdc, const CRect& rcBounds, const CRect& rcInvalid)
{
    if (!pdc)
        return;


    // To size the main dialog window and fill the background
    m_MainDialog.MoveWindow(rcBounds, TRUE);
    CBrush brBackGnd(TranslateColor(AmbientBackColor()));
    pdc-&gt;FillRect(rcBounds, &brBackGnd);


    DoSuperclassPaint(pdc, rcBounds);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h3><span style="">C. Adding Properties to the ActiveX control </span></h3>
<p class="MsoNormal">Step1. In Class View, expand the element MFCActiveXLib. Right click on _DMFCActiveX, and click on Add, Add Property. In the Add Property Wizard dialog, select FLOAT for Property type, and enter &quot;FloatProperty&quot; for property name.
 Select &quot;Get/Set methods&quot; to create the methods GetFloatProperty and SetFloatProperty.
</p>
<p class="MsoNormal">Step2. In the class CMFCActiveXCtrl, add a member variable, FLOAT m_fField. In the class's contructor, set the variable's default value to 0.0f.<b><span style="">
</span></b></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
CMFCActiveXCtrl::CMFCActiveXCtrl() : m_fField(0.0f)
{
    InitializeIIDs(&IID_DMFCActiveX, &IID_DMFCActiveXEvents);
    // TODO: Initialize your control's instance data here.
}

</pre>
<pre id="codePreview" class="cplusplus">
CMFCActiveXCtrl::CMFCActiveXCtrl() : m_fField(0.0f)
{
    InitializeIIDs(&IID_DMFCActiveX, &IID_DMFCActiveXEvents);
    // TODO: Initialize your control's instance data here.
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step3. Associate the Get/Set methods of FloatProperty with m_fField.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
FLOAT CMFCActiveXCtrl::GetFloatProperty(void)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());


    // TODO: Add your dispatch handler code here
    return this-&gt;m_fField;
}


void CMFCActiveXCtrl::SetFloatProperty(FLOAT newVal)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());


    // TODO: Add your property handler code here
    
    // Fire the event, FloatPropertyChanging
    VARIANT_BOOL cancel = VARIANT_FALSE; 
    FloatPropertyChanging(newVal, &cancel);


    if (cancel == VARIANT_FALSE)
    {
        m_fField = newVal;    // Save the new value
        SetModifiedFlag();


        // Display the new value in the control UI
        CString strFloatProp;
        strFloatProp.Format(_T(&quot;%f&quot;), m_fField);
        m_MainDialog.m_StaticFloatProperty.SetWindowTextW(strFloatProp);
    }
    // else, do nothing.
}

</pre>
<pre id="codePreview" class="cplusplus">
FLOAT CMFCActiveXCtrl::GetFloatProperty(void)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());


    // TODO: Add your dispatch handler code here
    return this-&gt;m_fField;
}


void CMFCActiveXCtrl::SetFloatProperty(FLOAT newVal)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());


    // TODO: Add your property handler code here
    
    // Fire the event, FloatPropertyChanging
    VARIANT_BOOL cancel = VARIANT_FALSE; 
    FloatPropertyChanging(newVal, &cancel);


    if (cancel == VARIANT_FALSE)
    {
        m_fField = newVal;    // Save the new value
        SetModifiedFlag();


        // Display the new value in the control UI
        CString strFloatProp;
        strFloatProp.Format(_T(&quot;%f&quot;), m_fField);
        m_MainDialog.m_StaticFloatProperty.SetWindowTextW(strFloatProp);
    }
    // else, do nothing.
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h3><span style="">D. Adding Methods to the ActiveX control </span></h3>
<p class="MsoNormal">In Class View, expand the element MFCActiveXLib. Right click on _DMFCActiveX, and click on Add, Add Method. In the Add Method Wizard dialog, select BSTR for the return type, and enter &quot;HelloWorld&quot; for Method name.<b><span style="">
</span></b></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
BSTR CMFCActiveXCtrl::HelloWorld(void)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());


    CString strResult;
    strResult = _T(&quot;HelloWorld&quot;);
    
    return strResult.AllocSysString();
}

</pre>
<pre id="codePreview" class="cplusplus">
BSTR CMFCActiveXCtrl::HelloWorld(void)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());


    CString strResult;
    strResult = _T(&quot;HelloWorld&quot;);
    
    return strResult.AllocSysString();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">With the almost same steps, the method GetProcessThreadID is added to get theexecuting process ID and thread ID:
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
void CMFCActiveXCtrl::GetProcessThreadID(LONG* pdwProcessId, LONG* pdwThreadId)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());


    // TODO: Add your dispatch handler code here
    *pdwProcessId = GetCurrentProcessId();
    *pdwThreadId = GetCurrentThreadId();
}

</pre>
<pre id="codePreview" class="cplusplus">
void CMFCActiveXCtrl::GetProcessThreadID(LONG* pdwProcessId, LONG* pdwThreadId)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());


    // TODO: Add your dispatch handler code here
    *pdwProcessId = GetCurrentProcessId();
    *pdwThreadId = GetCurrentThreadId();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h3><span style="">E. Adding Events to the ActiveX control </span></h3>
<p class="MsoNormal">Step1. In Class View, right click on CMFCActiveXCtrl, select Add,
<span class="GramE">Add</span> Event. In the Add Event Wizard, enter &quot;FloatPropertyChanging&quot; for Event name and add<b><span style="">
</span></b>two parameters: FLOAT <span class="SpellE">NewValue</span>, VARIANT_BOOL* Cancel.
</p>
<p class="MsoNormal">Step2. The event &quot;FloatPropertyChanging&quot; is fired in SetFloatProperty:
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
void CMFCActiveXCtrl::SetFloatProperty(FLOAT newVal)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());


    // TODO: Add your property handler code here
    
    // Fire the event, FloatPropertyChanging
    VARIANT_BOOL cancel = VARIANT_FALSE; 
    FloatPropertyChanging(newVal, &cancel);


    if (cancel == VARIANT_FALSE)
    {
        m_fField = newVal;    // Save the new value
        SetModifiedFlag();


        // Display the new value in the control UI
        CString strFloatProp;
        strFloatProp.Format(_T(&quot;%f&quot;), m_fField);
        m_MainDialog.m_StaticFloatProperty.SetWindowTextW(strFloatProp);
    }
    // else, do nothing.
}

</pre>
<pre id="codePreview" class="cplusplus">
void CMFCActiveXCtrl::SetFloatProperty(FLOAT newVal)
{
    AFX_MANAGE_STATE(AfxGetStaticModuleState());


    // TODO: Add your property handler code here
    
    // Fire the event, FloatPropertyChanging
    VARIANT_BOOL cancel = VARIANT_FALSE; 
    FloatPropertyChanging(newVal, &cancel);


    if (cancel == VARIANT_FALSE)
    {
        m_fField = newVal;    // Save the new value
        SetModifiedFlag();


        // Display the new value in the control UI
        CString strFloatProp;
        strFloatProp.Format(_T(&quot;%f&quot;), m_fField);
        m_MainDialog.m_StaticFloatProperty.SetWindowTextW(strFloatProp);
    }
    // else, do nothing.
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><span style="">&nbsp;</span></span>More Information
</h2>
<p class="MsoListParagraphCxSpFirst" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/ms968497.aspx">The ABCs of MFC ActiveX Controls</a>
</p>
<p class="MsoListParagraphCxSpLast" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://www.codeproject.com/KB/COM/CompleteActiveX.aspx">A Complete ActiveX Web Control Tutorial By David
<span class="SpellE">Marcionek</span></a> </p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
