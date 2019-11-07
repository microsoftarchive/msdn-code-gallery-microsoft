=============================================================================
    MICROSOFT FOUNDATION CLASS LIBRARY : MFCTaskDialog Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The CTaskDialog class replaces the standard Windows message box and has 
additional functionality such as new controls to gather information from the 
user. This class is in the MFC library in Visual Studio 2010. The CTaskDialog 
is available starting with Windows Vista. Earlier versions of Windows cannot 
display the CTaskDialog object. Use CTaskDialog::IsSupported to determine at 
runtime whether the current user can display the task dialog box. The 
standard Windows message box is still supported in Visual Studio 2010.

This sample demonstrates the usages of CTaskDialog:

1. Basic usages
2. A relatively complete usuage of most controls on CTaskDialog
3. Progress bar and marquee progress bar on CTaskDialog
4. MessageBox usage
5. Navigation usage


/////////////////////////////////////////////////////////////////////////////
Prerequisite:

1. To run the sample, the minimum required operating system is Windows Vista.
2. To compile the sample, you need Visual Studio 2010.
3. The CTaskDialog is available only when you build your application by using 
the Unicode library.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

A. Add #include "afxtaskdialog.h" after the list of includes.

B. Determine at runtime whether the current system can display the task 
dialog box by calling CTaskDialog::IsSupported(). See the 
CMFCTaskDialogApp::InitInstance method in MFCTaskDialog.cpp.

C. Set the content, main instruction, title and footer of TaskDialog.

	CString strContent = _T("This is an important message to the user.");
	CString strMainInstruction = _T("Important!\nPlease read!");
	CString strTitle = _T("Basic Usage 1");

	CTaskDialog dlg(strContent, strMainInstruction, strTitle, 
		TDCBF_YES_BUTTON | TDCBF_NO_BUTTON, 
		TDF_ENABLE_HYPERLINKS | TDF_ALLOW_DIALOG_CANCELLATION);

D. Add command button controls to TaskDialog

	// a) Load with code
	dlg.AddCommandControl(IDS_COMMAND1, _T("First command link option"));
	dlg.AddCommandControl(IDS_COMMAND2, _T("Second command link option"));
	// [-or-]
	// b) Load from string resource
	//dlg.LoadCommandControls(IDS_COMMAND1, IDS_COMMAND2);

IDS_COMMAND1 and IDS_COMMAND2 are string resources added into the String 
Table in Resource View.

E. Add radio button controls to TaskDialog

	// a) Load with code
	dlg.AddRadioButton(IDS_RADIO1, _T("First possible radio"));
	dlg.AddRadioButton(IDS_RADIO2, _T("Second possible radio"));
	// [-or-]
	// b) Load from string resource
	//dlg.LoadRadioButtons(IDS_RADIO1, IDS_RADIO2);

IDS_RADIO1 and IDS_RADIO2 are string resources added into the String Table in 
Resource View.

F. Set icons on TaskDialog

	dlg.SetFooterIcon(TD_INFORMATION_ICON);
	dlg.SetMainIcon(TD_WARNING_ICON);

G. Add an expansion area of TaskDialog

	dlg.SetExpansionArea(
		_T("Supplementary to the user\ntyped over two lines."), 
		_T("Get some additional information."), 
		_T("Hide the additional information."));

H. Add a verification checkbox on TaskDialog

	dlg.SetVerificationCheckboxText(_T("Remember the user's settings."));
	dlg.SetVerificationCheckbox(TRUE);

I. Add a progress bar on TaskDialog

CTaskDialog must be created with the TDF_SHOW_PROGRESS_BAR option:

	CTaskDialog dlg(strContent, strMainInstruction, strTitle, 
		TDCBF_YES_BUTTON | TDCBF_NO_BUTTON, 
		TDF_ENABLE_HYPERLINKS | TDF_SHOW_PROGRESS_BAR);

Call CTaskDialog::SetProgressBarRange to set the range of the progress bar.
Call CTaskDialog::SetProgressBarState to set the status (PBST_ERROR or 
PBST_PAUSED or PBST_NORMAL). 
Call CTaskDialog::SetProgressBarPosition to set the current position of the 
progress bar.

J. Add a marquee progress bar on TaskDialog

To add a marquee progress bar on TaskDialog, you simply need to add the 
TDF_SHOW_MARQUEE_PROGRESS_BAR option while creating CTaskDialog object, and 
call CTaskDialog::SetProgressBarMarquee.

	CTaskDialog dlg(strContent, strMainInstruction, strTitle, 
		TDCBF_YES_BUTTON | TDCBF_NO_BUTTON, 
		TDF_ENABLE_HYPERLINKS | TDF_SHOW_MARQUEE_PROGRESS_BAR);
	dlg.SetProgressBarMarquee(TRUE , 2);

	
M. Navigation usage

One CTaskDialog can call CTaskDialog::NavigateTo to transfer the focus to 
another CTaskDialog.

K. Display TaskDiaog and get the results

You can call CTaskDialog::DoModal() to display the task dialog as a modal 
dialog. The application then waits for the user to close the dialog box.

The CTaskDialog closes when the user selects a common button, a command link 
control, or closes the CTaskDialog. The return value is the identifier that 
indicates how the user closed the dialog box. 

	CTaskDialog::GetSelectedCommandControlID() returns the selected command 
	  button control ID. 
	CTaskDialog::GetSelectedRadioButtonID() returns the selected radio button.
	CTaskDialog::GetVerificationCheckboxState() retrieves the state of the 
	  verification check box

L. Simplify the creation of TaskDialog

CTaskDialog::ShowDialog is able to creating a simple CTaskDialog object and 
display it in one call.

	INT_PTR nResult = CTaskDialog::ShowDialog(
		_T("Do you like the MFCTaskDialog sample?"), _T(""), 
		_T("MessageBox Usage"), 0, 0, TDCBF_YES_BUTTON | TDCBF_NO_BUTTON);


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: CTaskDialog Class
http://msdn.microsoft.com/en-us/library/dd293651(VS.100).aspx

Walkthrough: Adding a CTaskDialog to an Application
http://msdn.microsoft.com/en-us/library/dd465289(VS.100).aspx

CTaskDialog: an alternative to the simple message box!
http://blogs.msdn.com/vcblog/archive/2008/12/04/ctaskdialog-an-alternative-to-the-simple-message-box.aspx


/////////////////////////////////////////////////////////////////////////////