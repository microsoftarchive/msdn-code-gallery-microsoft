# Window subclassing demo (CppWindowsSubclassing)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- Windows SDK
## Topics
- Windows UI
- Subclassing
## Updated
- 03/01/2012
## Description

<h1><span style="font-family:������">WIN32 APPLICATION</span> (<span style="font-family:������">CppWindowsSubclassing</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">If a control or a window does almost everything you want, but you need a few more features, you can change or add features to the original control by subclassing it. A subclass can have all the features of an existing class as well as
 any additional features you want to give it. </p>
<p class="MsoNormal">Two subclassing rules apply to subclassing in Win32. </p>
<p class="MsoListParagraphCxSpFirst" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Subclassing is allowed only within a process. An application cannot subclass a window or class that belongs to another process.
</p>
<p class="MsoListParagraphCxSpLast" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>The subclassing process may not use the original window procedure address directly.
</p>
<p class="MsoNormal">There are two approaches to window subclassing. </p>
<p class="MsoNormal"><b style=""><span style="">1. </span>Subclassing Controls Prior to ComCtl32.dll version 6
</b></p>
<p class="MsoNormal">The first is usable by most windows operating systems (Windows 2000, XP and later). You can put a control in a subclass and store user data within a control. You do this when you use versions of ComCtl32.dll prior to version 6 which ships
 with Microsoft Windows XP. There are some disadvantages in creating subclasses with earlier versions of ComCtl32.dll.
</p>
<p class="MsoListParagraphCxSpFirst" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>The window procedure can only be replaced once. </p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>It is difficult to remove a subclass after it is created. </p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Associating private data with a window is inefficient. </p>
<p class="MsoListParagraphCxSpLast" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>To call the next procedure in a subclass chain, you cannot cast the old window procedure and call it, you must call it using CallWindowProc.
</p>
<p class="MsoNormal">To make a new control it is best to start with one of the Windows common controls and extend it to fit a particular need. To extend a control, create<span style="">
</span>a control and replace its existing window procedure with a new one. The new procedure intercepts the control's messages and either acts on them or passes them to the original procedure for default processing. Use the SetWindowLong or SetWindowLongPtr
 function to replace the WNDPROC of the control. </p>
<p class="MsoNormal"><b style="">2. Subclassing Controls Using ComCtl32.dll version 6
</b></p>
<p class="MsoNormal">The second is only usable with a minimum operating system of Windows XP since it relies on ComCtl32.dll version 6. ComCtl32.dll version 6 supplied with Windows XP contains four functions that make creating subclasses easier and eliminate
 the disadvantages previously discussed. The new functions encapsulate the management involved with multiple sets of reference data, therefore the developer can focus on programming features and not on managing subclasses. The subclassing functions are:
</p>
<p class="MsoListParagraphCxSpFirst" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>SetWindowSubclass </p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>GetWindowSubclass </p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>RemoveWindowSubclass </p>
<p class="MsoListParagraphCxSpLast" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>DefSubclassProc.<span style="">&nbsp; </span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style="">The main form is: </span></p>
<p class="MsoNormal"><span style=""><img src="53098-image.png" alt="" width="352" height="332" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal"><span style="">If you click one of the tow <span style="">
&nbsp;</span>��Subclass�� buttons, the form changes as follows: </span></p>
<p class="MsoNormal"><span style=""><img src="53099-image.png" alt="" width="352" height="332" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal"><span style="">If you right click on the <span style="">&nbsp;</span>button , a message box pops up.
</span></p>
<p class="MsoNormal"><span style=""><img src="53100-image.png" alt="" width="284" height="154" align="middle">
</span><span style=""></span></p>
<h2><span style="">Using the code </span></h2>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step1. According to the CppWindowsDialog example, build up the main dialog for use in this example.
</span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">IDD_MAINDIALOG - The main dialog, having a button with the caption &quot;Right-click the button&quot;, which serves as the target of subclassing.
</span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step2. To demonstrate subclassing Controls Prior to ComCtl32.dll version 6, add the buttons IDC_SUBCLASS_BN and IDC_UNSUBCLASS_BN, and the new button procedure with the prototype:
</span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Inside NewBtnProc, customize the control's behaviors for the proper messages, get the old button procedure and call CallWindowProc to invoke the old behavior of the control.
</span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>1) Subclass (OnSubclass) </span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Get the handle of the control to be subclassed -&gt; Subclass the button control (SetWindowLongPtr, GWLP_WNDPROC) -&gt; Store the original, default window procedure of the button as the button control's user data (SetWindowLongPtr, GWLP_USERDATA)
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
void OnSubclass(HWND hWnd)
{
    // Get the handle of the control to be subclassed, and subclass it by 
    // using SetWindowLongPtr with GWLP_WNDPROC or using the SubclassWindow 
    // macro defined in windowsx.h.
    HWND hButton = GetDlgItem(hWnd, IDC_BUTTON);
    WNDPROC OldBtnProc = reinterpret_cast&lt;WNDPROC&gt;(SetWindowLongPtr(
        hButton, GWLP_WNDPROC, reinterpret_cast&lt;LONG_PTR&gt;(NewBtnProc)));
    if (OldBtnProc == NULL)
    {
        ReportError(L&quot;SetWindowLongPtr in OnSubclass&quot;, GetLastError());
        return;
    }


    // Store the original, default window procedure of the button as the 
    // button control's user data.
    SetWindowLongPtr(hButton, GWLP_USERDATA, reinterpret_cast&lt;LONG_PTR&gt;(OldBtnProc));


    // Invalidate the button control so that WM_PAINT is sent to it and the 
    // new paint of the button can be shown immediately.
    RECT rc;
    if (GetClientRect(hButton, &rc))
    {
        InvalidateRect(hButton, &rc, TRUE);
    }


    // Update the UI.
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SUBCLASS), FALSE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNSUBCLASS), TRUE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFESUBCLASS), FALSE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFEUNSUBCLASS), FALSE);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>2) Unsubclass (OnUnsubclass) </span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Get the handle of the control that was subclassed -&gt; Retrieve the previously stored original button window procedure (GetWindowLongPtr, GWLP_USERDATA) -&gt; Replace the current handler with the old one (setWindowLongPtr, GWLP_WNDPROC)
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
void OnUnsubclass(HWND hWnd)
{
    // Get the handle of the control that was subclassed, and unsubclass it 
    // by retrieving the previously stored original button window procedure 
    // and replacing the current handler with the old one.
    HWND hButton = GetDlgItem(hWnd, IDC_BUTTON);
    LONG_PTR OldBtnProc = GetWindowLongPtr(hButton, GWLP_USERDATA);
    if (0 == SetWindowLongPtr(hButton, GWLP_WNDPROC, OldBtnProc))
    {
        ReportError(L&quot;SetWindowLongPtr in OnUnsubclass&quot;, GetLastError());
    }


    // Invalidate the button control so that WM_PAINT is sent to it and the 
    // new paint of the button can be shown immediately.
    RECT rc;
    if (GetClientRect(hButton, &rc))
    {
        InvalidateRect(hButton, &rc, TRUE);
    }


    // Update the UI.
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SUBCLASS), TRUE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNSUBCLASS), FALSE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFESUBCLASS), TRUE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFEUNSUBCLASS), FALSE);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Step3. To demonstrate subclassing Controls Using ComCtl32.dll version 6, add the buttons IDC_SAFESUBCLASS_BN and IDC_SAFEUNSUBCLASS_BN, and the new button procedure with the prototype:
</span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">Every time a message is received by the new window procedure, a subclass ID and reference data are included. Inside NewSafeBtnProc, customize the control's behaviors for the proper messages, and call DefSubclassProc to invoke the default behavior
 of the control. Additionally, You must remove your window subclass before the window being subclassed is destroyed. This is typically done either by removing the subclass once your temporary need has passed, or if you are installing a permanent subclass, by
 inserting a call to RemoveWindowSubclass inside the subclass procedure itself: </span>
</p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
      case WM_NCDESTROY:


      // You must remove your window subclass before the window being 
      // subclassed is destroyed. This is typically done either by removing 
      // the subclass once your temporary need has passed, or if you are 
      // installing a permanent subclass, by inserting a call to 
      // RemoveWindowSubclass inside the subclass procedure itself:


      if (!RemoveWindowSubclass(hButton, NewSafeBtnProc, uIdSubclass))
      {
          ReportError(L&quot;RemoveWindowSubclass in handling WM_NCDESTROY&quot;);
      }


      return DefSubclassProc(hButton, message, wParam, lParam);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">The application also needs to link to comctl32.lib (Project Properties / Linker / Input / Additional Dependencies), and includes the comctl32.h header file.
</span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>1) Subclass (OnSafeSubclass) </span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Get the handle of the control to be subclassed -&gt; Subclass the button control (SetWindowSubclass)
</span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>2) Unsubclass (OnSafeUnsubclass) </span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style=""><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Get the handle of the control that was subclassed -&gt; Unsubclass the control (RemoveWindowSubclass)
</span></p>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:������"></span></p>
<h2>More Information </h2>
<p class="MsoNormal" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:������"></span></p>
<p class="MsoListParagraphCxSpFirst" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://msdn.microsoft.com/en-us/library/bb773183.aspx">MSDN: Subclassing Controls</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://winapi.foosyerdoos.org.uk/info/sub_superclass.php">Subclassing and Superclassing</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://blogs.msdn.com/oldnewthing/archive/2009/05/07/9592397.aspx">When you subclass a window, it's the original window procedure of the window you subclass you have to call when you want to call
 the original window procedure</a> </span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://blogs.msdn.com/oldnewthing/archive/2003/11/11/55653.aspx">Safer subclassing</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://msdn.microsoft.com/en-us/library/ms997565.aspx">MSDN: Safe Subclassing in Win32</a>
</span></p>
<p class="MsoListParagraphCxSpLast" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-family:������"><a href="http://www.codeproject.com/KB/DLL/subhook.aspx.">Cross Process Subclassing</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
