# Windows modal and modeless dialogs in C++ (CppWindowsDialog)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- Windows SDK
## Topics
- Windows Dialog
## Updated
- 03/01/2012
## Description

<h1><span style="font-family:������">WIN32 APPLICATION </span>(<span style="font-family:������">CppWindowsDialog</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The CppWindowsDialog example demonstrates the skeleton of registered and created window, based on a dialog resource defined in a resource script. It<span style="">
</span>also shows the steps to show a modal or a modeless dialog.<span style="">&nbsp;
</span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style=""><img src="53116-image.png" alt="" width="481" height="311" align="middle">
</span><span style=""></span></p>
<p class="MsoListParagraph" style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">When you click ��Show Modal Dialog��: A Modal Dialog will pop up. Note that you can��t access the CppWinodwsDialog form unless you close the Modal Dialog.
</span></p>
<p class="MsoNormal"><span style=""><img src="53117-image.png" alt="" width="490" height="335" align="middle">
</span><span style=""></span></p>
<p class="MsoListParagraph" style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">When you click ��Show Modeless Dialog��: A Modeless Dialog will pop up. Note that you can access the CppWindowsDialog form again, and you can create the second Modeless Dialog form without having to close the previous one.
</span></p>
<p class="MsoNormal"><span style=""><img src="53118-image.png" alt="" width="576" height="384" align="middle">
</span><span style=""></span></p>
<p class="MsoNormal"></p>
<h2>Using the code</h2>
<p class="MsoNormal">Step1. Create a Visual C&#43;&#43; / Win32 / Win32 Project named CppWindowsDialog. In the project wizard, specify the application type as Windows application.
</p>
<p class="MsoNormal">Step2. In the Resource View, delete the Accelerator and Menu resource as they are not necessary in this simple dialog example.
</p>
<p class="MsoNormal">Step3. Rename the default dialog resource IDD_ABOUTBOX as IDD_MAINDIALOG. It serves as the main dialog of the windows application. Open IDD_MAINDIALOG in the designer, and set the following properties accordingly.
</p>
<p class="MsoNormal"><span style="background:#D9D9D9"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Caption = CppWindowsDialog </span></p>
<p class="MsoNormal"><span style="background:#D9D9D9"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Center = True </span></p>
<p class="MsoNormal"><span style="background:#D9D9D9"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Class Name = CPPWINDOWSDIALOG<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>// The class name of the custom dialog </span></p>
<p class="MsoNormal"><span style="background:#D9D9D9"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Minimize Box = True </span></p>
<p class="MsoNormal">Step4. Open the source file CppWindowsDialog.cpp, delete the About callback:
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step5. In _tWinMain, remove the lines: </p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
    HACCEL hAccelTable;
    hAccelTable = LoadAccelerators(hInstance, 
        MAKEINTRESOURCE(IDC_CPPWINDOWSDIALOG));

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">and remove the line<span style="">: </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">as we do not use accelerator in this example. </p>
<p class="MsoNormal">Step6. In MyRegisterClass, set wcex.cbWndExtra = DLGWINDOWEXTRA (See the article
<a href="http://blogs.msdn.com/oldnewthing/archive/2003/11/13/55662.aspx">http://blogs.msdn.com/oldnewthing/archive/2003/11/13/55662.aspx</a> for the reason). Set wcex.lpszMenuName = 0 as we do not use menus in dialog. Last, Set wcex.hbrBackground = (HBRUSH)(COLOR_BTNFACE&#43;1);
</p>
<p class="MsoNormal">Step7. In InitInstance, change the line </p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">to </p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
hWnd = CreateDialog(hInst, MAKEINTRESOURCE(IDD_MAINDIALOG), 0, 0);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step8. In WndProc, handle windows messages with the help of Message Forwarding<span style="">
</span>Macros in <span class="SpellE">windowsx.h</span> header. The Message Cracker Wizard created by Hern��n Di
<span class="SpellE">Pietro</span> helps to generate the prototype of message processing functions.
<a href="http://www.codeproject.com/KB/winsdk/msgcrackwizard.aspx">http://www.codeproject.com/KB/winsdk/msgcrackwizard.aspx</a><span style="">
</span></p>
<p class="MsoNormal">Step9. Create a Modal Dialog Box<span style="">. </span></p>
<p class="MsoNormal">You create a modal dialog box by using the DialogBox function. You must specify the identifier or name of a dialog box template resource and a pointer to the dialog box procedure. The DialogBox function loads the template, displays the
 dialog box, and processes all <span class="GramE">user</span> input until the user closes the dialog box.
</p>
<p class="MsoNormal">Step10. Create a Modeless Dialog Box<span style="">. </span>
</p>
<p class="MsoNormal">You create a modeless dialog box by using the CreateDialog function, specifying the identifier or name of a dialog box template resource and a pointer to the dialog box procedure. CreateDialog loads the template, creates the dialog box,
 and optionally displays it. Your application is responsible for retrieving and dispatching user input messages to the dialog box procedure.
</p>
<p class="MsoNormal">Step11. Enable Visual Style<span style="">. </span></p>
<p class="MsoNormal">To add a resource manifest that will result in the application being drawn with winxp visual styles, open the project's properties from the 'project' menu by selecting properties. Select 'linker - Manifest File' from the tree control
 on the left of your project properties dialog; the following string<span style="">
</span>should be added to the 'Additional Manifest Dependencies' field in the right hand panel:
</p>
<p class="MsoNormal"><span class="GramE"><span style="background:#D9D9D9">type</span></span><span style="background:#D9D9D9">='win32'
</span><span style="background:#D9D9D9"></span></p>
<p class="MsoNormal"><span class="GramE"><span style="background:#D9D9D9">name</span></span><span style="background:#D9D9D9">='<span class="SpellE">Microsoft.Windows.Common</span>-Controls' version='6.0.0.0'
</span></p>
<p class="MsoNormal"><span class="SpellE"><span class="GramE"><span style="background:#D9D9D9">processorArchitecture</span></span></span><span style="background:#D9D9D9">='x86'
</span><span style="background:#D9D9D9"></span></p>
<p class="MsoNormal"><span class="SpellE"><span class="GramE"><span style="background:#D9D9D9">publicKeyToken</span></span></span><span style="background:#D9D9D9">='6595b64144ccf1df' language='*'</span><span style="background:#D9D9D9">
</span></p>
<p class="MsoNormal"><span style=""></span></p>
<h2>More Information</h2>
<p class="MsoListParagraphCxSpFirst" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/ms644996.aspx">MSDN: Using Dialog Boxes</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/ms633574.aspx">MSDN: About Window Classes</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://www.codeproject.com/KB/winsdk/msgcrackwizard.aspx">Message Cracker Wizard for Win32 SDK Developer</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://winapi.foosyerdoos.org.uk/info/user_cntrls.php">Creating Windows and User Controls</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://winapi.foosyerdoos.org.uk/info/common_cntrls.php.">Creating Common Controls</a>
</p>
<p class="MsoListParagraphCxSpLast"></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
