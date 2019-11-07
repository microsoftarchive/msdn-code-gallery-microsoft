# Modal and modeless dialogs in MFC (MFCDialog)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- MFC
## Topics
- Dialog
## Updated
- 03/04/2012
## Description

<h1><span style="font-family:������">MICROSOFT FOUNDATION CLASS LIBRARY</span> (<span style="font-family:������">MFCDialog</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The MFCDialog example demonstrates the creation of modal and modeless dialog boxes in MFC.<span style="">
</span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style=""><img src="53705-image.png" alt="" width="420" height="287" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal"><span style="">If you click the ��Show Modal Dialog��, a dialog pops up. You can��t manipulate the main form unless you close the modal dialog.
</span></p>
<p class="MsoNormal"><span style=""><img src="53706-image.png" alt="" width="576" height="355" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal"><span style="">If you click the ��Show Modeless Dialog�� button, a modeless dialog pops up. You can manipulate the main dialog without having to close the modeless dialog.
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal">A. Creating Modal Dialog Boxes<span style="">&nbsp; </span>
</p>
<p class="MsoNormal">To create a modal dialog box, call either of the two public constructors declared in CDialog. Next, call the dialog object's DoModal member function to display the dialog box and manage interaction with it until the user chooses OK or
 Cancel. This management by DoModal is what makes the dialog box modal. For modal dialog boxes, DoModal loads the dialog resource.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
    CMFCDialogDlg dlg;
    m_pMainWnd = &dlg;
    INT_PTR nResponse = dlg.DoModal();
    if (nResponse == IDOK)
    {
        // TODO: Place code here to handle when the dialog is
        //  dismissed with OK
    }
    else if (nResponse == IDCANCEL)
    {
        // TODO: Place code here to handle when the dialog is
        //  dismissed with Cancel
    }

</pre>
<pre id="codePreview" class="cplusplus">
    CMFCDialogDlg dlg;
    m_pMainWnd = &dlg;
    INT_PTR nResponse = dlg.DoModal();
    if (nResponse == IDOK)
    {
        // TODO: Place code here to handle when the dialog is
        //  dismissed with OK
    }
    else if (nResponse == IDCANCEL)
    {
        // TODO: Place code here to handle when the dialog is
        //  dismissed with Cancel
    }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<p class="MsoNormal">B. Creating Modeless Dialog Boxes </p>
<p class="MsoNormal">For a modeless dialog box, you must provide your own public constructor in your dialog class. To create a modeless dialog box, call your public constructor and then call the dialog object's Create member function to load the dialog resource.
 You can call Create either during or after the constructor call. If the dialog resource has the property WS_VISIBLE, the dialog box appears immediately. If not, you must call its ShowWindow member function.<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
CMFCDialogDlg::CMFCDialogDlg(CWnd* pParent /*=NULL*/)
    : CDialog(CMFCDialogDlg::IDD, pParent)
{
    m_hIcon = AfxGetApp()-&gt;LoadIcon(IDR_MAINFRAME);
}

</pre>
<pre id="codePreview" class="cplusplus">
CMFCDialogDlg::CMFCDialogDlg(CWnd* pParent /*=NULL*/)
    : CDialog(CMFCDialogDlg::IDD, pParent)
{
    m_hIcon = AfxGetApp()-&gt;LoadIcon(IDR_MAINFRAME);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<h2>More Information<span style=""> </span></h2>
<p class="MsoListParagraphCxSpFirst" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/b8tas481.aspx">MSDN: Creating Modal Dialog Boxes
</a><span style="">&nbsp;</span> </p>
<p class="MsoListParagraphCxSpLast" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/hf0yazk7.aspx">MSDN: Creating Modeless Dialog Boxes
</a><span style="">&nbsp;</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
