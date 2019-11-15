# MFC CTaskDialog sample (MFCTaskDialog)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- MFC
## Topics
- Controls
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>MICROSOFT FOUNDATION CLASS LIBRARY : MFCTaskDialog Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
The CTaskDialog class replaces the standard Windows message box and has <br>
additional functionality such as new controls to gather information from the <br>
user. This class is in the MFC library in Visual Studio 2010. The CTaskDialog <br>
is available starting with Windows Vista. Earlier versions of Windows cannot <br>
display the CTaskDialog object. Use CTaskDialog::IsSupported to determine at <br>
runtime whether the current user can display the task dialog box. The <br>
standard Windows message box is still supported in Visual Studio 2010.<br>
<br>
This sample demonstrates the usages of CTaskDialog:<br>
<br>
1. Basic usages<br>
2. A relatively complete usuage of most controls on CTaskDialog<br>
3. Progress bar and marquee progress bar on CTaskDialog<br>
4. MessageBox usage<br>
5. Navigation usage<br>
<br>
</p>
<h3>Prerequisite:</h3>
<p style="font-family:Courier New"><br>
1. To run the sample, the minimum required operating system is Windows Vista.<br>
2. To compile the sample, you need Visual Studio 2010.<br>
3. The CTaskDialog is available only when you build your application by using <br>
the Unicode library.<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
A. Add #include &quot;afxtaskdialog.h&quot; after the list of includes.<br>
<br>
B. Determine at runtime whether the current system can display the task <br>
dialog box by calling CTaskDialog::IsSupported(). See the <br>
CMFCTaskDialogApp::InitInstance method in MFCTaskDialog.cpp.<br>
<br>
C. Set the content, main instruction, title and footer of TaskDialog.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;CString strContent = _T(&quot;This is an important message to the user.&quot;);<br>
&nbsp;&nbsp;&nbsp;&nbsp;CString strMainInstruction = _T(&quot;Important!\nPlease read!&quot;);<br>
&nbsp;&nbsp;&nbsp;&nbsp;CString strTitle = _T(&quot;Basic Usage 1&quot;);<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;CTaskDialog dlg(strContent, strMainInstruction, strTitle,
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TDCBF_YES_BUTTON | TDCBF_NO_BUTTON,
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TDF_ENABLE_HYPERLINKS | TDF_ALLOW_DIALOG_CANCELLATION);<br>
<br>
D. Add command button controls to TaskDialog<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;// a) Load with code<br>
&nbsp;&nbsp;&nbsp;&nbsp;dlg.AddCommandControl(IDS_COMMAND1, _T(&quot;First command link option&quot;));<br>
&nbsp;&nbsp;&nbsp;&nbsp;dlg.AddCommandControl(IDS_COMMAND2, _T(&quot;Second command link option&quot;));<br>
&nbsp;&nbsp;&nbsp;&nbsp;// [-or-]<br>
&nbsp;&nbsp;&nbsp;&nbsp;// b) Load from string resource<br>
&nbsp;&nbsp;&nbsp;&nbsp;//dlg.LoadCommandControls(IDS_COMMAND1, IDS_COMMAND2);<br>
<br>
IDS_COMMAND1 and IDS_COMMAND2 are string resources added into the String <br>
Table in Resource View.<br>
<br>
E. Add radio button controls to TaskDialog<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;// a) Load with code<br>
&nbsp;&nbsp;&nbsp;&nbsp;dlg.AddRadioButton(IDS_RADIO1, _T(&quot;First possible radio&quot;));<br>
&nbsp;&nbsp;&nbsp;&nbsp;dlg.AddRadioButton(IDS_RADIO2, _T(&quot;Second possible radio&quot;));<br>
&nbsp;&nbsp;&nbsp;&nbsp;// [-or-]<br>
&nbsp;&nbsp;&nbsp;&nbsp;// b) Load from string resource<br>
&nbsp;&nbsp;&nbsp;&nbsp;//dlg.LoadRadioButtons(IDS_RADIO1, IDS_RADIO2);<br>
<br>
IDS_RADIO1 and IDS_RADIO2 are string resources added into the String Table in <br>
Resource View.<br>
<br>
F. Set icons on TaskDialog<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;dlg.SetFooterIcon(TD_INFORMATION_ICON);<br>
&nbsp;&nbsp;&nbsp;&nbsp;dlg.SetMainIcon(TD_WARNING_ICON);<br>
<br>
G. Add an expansion area of TaskDialog<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;dlg.SetExpansionArea(<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_T(&quot;Supplementary to the user\ntyped over two lines.&quot;),
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_T(&quot;Get some additional information.&quot;),
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_T(&quot;Hide the additional information.&quot;));<br>
<br>
H. Add a verification checkbox on TaskDialog<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;dlg.SetVerificationCheckboxText(_T(&quot;Remember the user's settings.&quot;));<br>
&nbsp;&nbsp;&nbsp;&nbsp;dlg.SetVerificationCheckbox(TRUE);<br>
<br>
I. Add a progress bar on TaskDialog<br>
<br>
CTaskDialog must be created with the TDF_SHOW_PROGRESS_BAR option:<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;CTaskDialog dlg(strContent, strMainInstruction, strTitle,
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TDCBF_YES_BUTTON | TDCBF_NO_BUTTON,
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TDF_ENABLE_HYPERLINKS | TDF_SHOW_PROGRESS_BAR);<br>
<br>
Call CTaskDialog::SetProgressBarRange to set the range of the progress bar.<br>
Call CTaskDialog::SetProgressBarState to set the status (PBST_ERROR or <br>
PBST_PAUSED or PBST_NORMAL). <br>
Call CTaskDialog::SetProgressBarPosition to set the current position of the <br>
progress bar.<br>
<br>
J. Add a marquee progress bar on TaskDialog<br>
<br>
To add a marquee progress bar on TaskDialog, you simply need to add the <br>
TDF_SHOW_MARQUEE_PROGRESS_BAR option while creating CTaskDialog object, and <br>
call CTaskDialog::SetProgressBarMarquee.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;CTaskDialog dlg(strContent, strMainInstruction, strTitle,
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TDCBF_YES_BUTTON | TDCBF_NO_BUTTON,
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TDF_ENABLE_HYPERLINKS | TDF_SHOW_MARQUEE_PROGRESS_BAR);<br>
&nbsp;&nbsp;&nbsp;&nbsp;dlg.SetProgressBarMarquee(TRUE , 2);<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;<br>
M. Navigation usage<br>
<br>
One CTaskDialog can call CTaskDialog::NavigateTo to transfer the focus to <br>
another CTaskDialog.<br>
<br>
K. Display TaskDiaog and get the results<br>
<br>
You can call CTaskDialog::DoModal() to display the task dialog as a modal <br>
dialog. The application then waits for the user to close the dialog box.<br>
<br>
The CTaskDialog closes when the user selects a common button, a command link <br>
control, or closes the CTaskDialog. The return value is the identifier that <br>
indicates how the user closed the dialog box. <br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;CTaskDialog::GetSelectedCommandControlID() returns the selected command
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;button control ID. <br>
&nbsp;&nbsp;&nbsp;&nbsp;CTaskDialog::GetSelectedRadioButtonID() returns the selected radio button.<br>
&nbsp;&nbsp;&nbsp;&nbsp;CTaskDialog::GetVerificationCheckboxState() retrieves the state of the
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;verification check box<br>
<br>
L. Simplify the creation of TaskDialog<br>
<br>
CTaskDialog::ShowDialog is able to creating a simple CTaskDialog object and <br>
display it in one call.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;INT_PTR nResult = CTaskDialog::ShowDialog(<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_T(&quot;Do you like the MFCTaskDialog sample?&quot;), _T(&quot;&quot;),
<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_T(&quot;MessageBox Usage&quot;), 0, 0, TDCBF_YES_BUTTON | TDCBF_NO_BUTTON);<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: CTaskDialog Class<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/dd293651(VS.100).aspx">http://msdn.microsoft.com/en-us/library/dd293651(VS.100).aspx</a><br>
<br>
Walkthrough: Adding a CTaskDialog to an Application<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/dd465289(VS.100).aspx">http://msdn.microsoft.com/en-us/library/dd465289(VS.100).aspx</a><br>
<br>
CTaskDialog: an alternative to the simple message box!<br>
<a target="_blank" href="http://blogs.msdn.com/vcblog/archive/2008/12/04/ctaskdialog-an-alternative-to-the-simple-message-box.aspx">http://blogs.msdn.com/vcblog/archive/2008/12/04/ctaskdialog-an-alternative-to-the-simple-message-box.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
