========================================================================
    WIN32 APPLICATION : CppWindowsCommonControls Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

CppWindowsCommonControls contains simple examples of how to create common  
controls defined in comctl32.dll. The controls include Animation, ComboBoxEx, 
Updown, Header, MonthCal, DateTimePick, ListView, TreeView, Tab, Tooltip, IP 
Address, Statusbar, Progress Bar, Toolbar, Trackbar, and SysLink.


/////////////////////////////////////////////////////////////////////////////
Creation:

First, build up the dialogs for use in this example according to the 
CppWindowsDialog example. Then link the application to comctl32.lib in the 
project's property page / Linker / Input / Additional Dependencies. Because 
the sample needs to work on Windows XP and Windows 2003 apart from Windows 
Vista, change the default target version to _WIN32_WINNT_WINXP in the 
targetver.h.

	#ifndef WINVER
	#define WINVER _WIN32_WINNT_WINXP		// Originally 0x0600
	#endif

	#ifndef _WIN32_WINNT
	#define _WIN32_WINNT _WIN32_WINNT_WINXP	// Originally 0x0600
	#endif 

A. Animation Control

Step1. Add the AVI resource IDR_UPLOAD_AVI according to the KB article:

How to create a resource .dll file that contains an AVI file 
http://support.microsoft.com/kb/178199

Step2. In OnInitAnimationDialog, load and register animation control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_ANIMATE_CLASS;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step3. Create the animation control.

	RECT rc = { 20, 20, 280, 60 };
	HWND hAnimate = CreateWindowEx(0, ANIMATE_CLASS, 0, 
		ACS_TIMER | ACS_AUTOPLAY | ACS_TRANSPARENT | WS_CHILD | WS_VISIBLE, 
		rc.left, rc.top, rc.right, rc.bottom, 
		hWnd, (HMENU)IDC_ANIMATION, hInst, 0);

Step4. Open and play the AVI clip in the animation control.

	SendMessage(hAnimate, ACM_OPEN, (WPARAM)0, 
		(LPARAM)MAKEINTRESOURCE(IDR_UPLOAD_AVI));
	SendMessage(hAnimate, ACM_PLAY, (WPARAM)-1, 
		MAKELONG(/*from frame*/0, /*to frame*/-1));

B. ComboBoxEx Control

Step1. In OnInitComboBoxExDialog, load and register ComboBoxEx control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_USEREX_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create the ComboBoxEx control.

	RECT rc = { 20, 20, 280, 100 };
	HWND hComboEx = CreateWindowEx(0, WC_COMBOBOXEX, 0, 
		CBS_DROPDOWN | WS_CHILD | WS_VISIBLE, 
		rc.left, rc.top, rc.right, rc.bottom, 
		hWnd, (HMENU)IDC_COMBOBOXEX, hInst, 0);

Step3. Create an image list and associate the image list with the ComboBoxEx 
control. (ImageList_Create, CBEM_SETIMAGELIST)

Step4. Add some items with image to the ComboBoxEx common control. 
(COMBOBOXEXITEM, CBEM_INSERTITEM)

Step5. Destroy the image list on close. (ImageList_Destroy)

C. Up-Down Control

Step1. In OnInitUpdownDialog, load and register Updown control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_UPDOWN_CLASS;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create an Updown control and an Edit control. (CreateWindowEx)

Step3. Attach the Updown control to its 'buddy' edit control. (UDM_SETBUDDY)

D. Header Control

Step1. In OnInitHeaderDialog, load and register Header control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_WIN95_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create an Header control. (CreateWindowEx)

Step3. Resize the header control to fit the client rectangle. 
(HDM_LAYOUT, HDLAYOUT, SetWindowPos)

Step4. Add Header items (HDITEM, HDM_INSERTITEM)

Step5. In OnHeaderSize of the window, resize the Header control accordingly.
(HDM_LAYOUT, HDLAYOUT, SetWindowPos)

E. Month Calendar Control

Step1. In OnInitMonthCalDialog, load and register MonthCal control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_DATE_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create a Month Calendar control. (CreateWindowEx)

F. Date and Time Picker Control

Step1. In OnInitDateTimePickDialog, load and register DateTimePick control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_DATE_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create a DateTimePick control. (CreateWindowEx)

G. List View Control

Step1. In OnInitListviewDialog, load and register Listview control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_LISTVIEW_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create a Listview control. (CreateWindowEx)

Step3. Set up and attach image lists to list view common control. 
(ImageList_Create, ImageList_AddIcon, ListView_SetImageList)

Step4. Add items to the the list view common control. 
(LVITEM, LVM_INSERTITEM)

Step5. In OnListviewSize of the window, resize and re-arrange the Listview 
control to fit the client rectangle. (MoveWindow, LVM_ARRANGE)

Step6. In OnListviewClose, free up the image list resources. 
(ImageList_Destroy)

H. Tree View Control

Step1. OnInitTreeviewDialog, load and register Treeview control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_TREEVIEW_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create a Treeview control. (CreateWindowEx)

Step3. Set up and attach image lists to tree view common control. 
(ImageList_Create, ImageList_AddIcon, TreeView_SetImageList)

Step4. Add items to the tree view common control. 
(TVITEM, TVINSERTSTRUCT, TVM_INSERTITEM)

Step5. In OnTreeviewSize of the window, resize the Treeview control to fit 
the client rectangle. (MoveWindow)

Step6. In OnTreeviewClose, free up the image list resources. 
(TreeView_GetImageList, ImageList_Destroy)

I. Tab Control

Step1. In OnInitTabControlDialog, load and register Tab control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_TAB_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create a Tab control. (CreateWindowEx)

Step3. Add items to the tab common control. (TCITEM, TCM_INSERTITEM)

Step4. In OnTabSize of the window, resize the Tab control to fit the client 
rectangle. (MoveWindow)

J. Tooltip Control

Step1. In OnInitTooltipDialog, load and register Tooltip control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_WIN95_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create a button control and a tooltip control. Note that a tooltip 
control should not have the WS_CHILD style, nor should it have an id, 
otherwise its behavior will be adversely affected, eg. tooltips displayed in 
wrong place or not at all. (CreateWindowEx)

Step3. Associate the tooltip with the button control. (TOOLINFO, TTM_ADDTOOL)

K. IP Address Control

Step1. In OnInitIPAddressDialog, load and register IPAddress control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_INTERNET_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create the IPAddress control. (CreateWindowEx)

L. Status Bar Control

Step1. In OnInitStatusbarDialog, load and register StatusBar control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_BAR_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create the status bar control. (CreateWindowEx)

Step3. Establish the number of partitions or 'parts' the status bar will 
have, their actual dimensions will be set in the parent window's WM_SIZE 
handler. (SB_SETPARTS)

Step4. Put some texts into each part of the status bar and setup each part.
(SB_SETTEXT)

Step5. In OnStatusbarSize, partition the statusbar to keep the ratio of the 
sizes of its parts constant. Resize statusbar so it's always same width as 
parent's client area. (SB_SETPARTS, WM_SIZE)

M. Progress Bar Control

Step1. In OnInitProgressDialog, load and register Progress Bar control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_PROGRESS_CLASS;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create the progress bar control. (CreateWindowEx)

Step3. Set the progress bar position. (PBM_SETPOS)

N. Toolbar Control

Step1. In OnInitToolbarDialog, load and register Toolbar control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_BAR_CLASSES;
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create the toolbar control. (CreateWindowEx)

Step3. Setup and add buttons to Toolbar. 

  3.1 Send the TB_BUTTONSTRUCTSIZE to the toolbar control. If an application 
  uses the CreateWindowEx function to create the toolbar, the application 
  must send this message to the toolbar before sending the TB_ADDBITMAP or 
  TB_ADDBUTTONS message. The CreateToolbarEx function automatically sends 
  TB_BUTTONSTRUCTSIZE, and the size of the TBBUTTON structure is a parameter 
  of the function.
  
  3.2 Add images (TBADDBITMAP, TB_ADDBITMAP)
  
  3.3 Add buttons (TBBUTTON, TB_ADDBUTTONS)
  
Step4. Tell the toolbar to resize itself, and show it. (TB_AUTOSIZE)

O. Trackbar control

Step1. In OnInitTrackbarDialog, load and register Trackbar control class. 

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_WIN95_CLASSES; // Or ICC_PROGRESS_CLASS
	if (!InitCommonControlsEx(&iccx))
		return FALSE;

Step2. Create the Trackbar control. (CreateWindowEx)

Step3. Set Trackbar range. (TBM_SETRANGE)

P. SysLink Control

Step1. In OnInitSysLinkDialog, load and register SysLink control class.

	INITCOMMONCONTROLSEX iccx;
	iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
	iccx.dwICC = ICC_LINK_CLASS;
	if (!InitCommonControlsEx(&iccx))
		return FALSE; 

Step2. Create the SysLink control. The SysLink control supports the anchor 
tag(<a>) along with the attributes HREF and ID. (CreateWindowEx) For example,

	HWND hLink = CreateWindowEx(0, WC_LINK, 
		_T("All-In-One Code Framework\n") \
		_T("<A HREF=\"http://cfx.codeplex.com\">Home</A> ") \
		_T("and <A ID=\"idBlog\">Blog</A>"), 
        WS_VISIBLE | WS_CHILD | WS_TABSTOP, 
		rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_SYSLINK, hInst, NULL);

Step3. In OnSysLinkNotify, capture the notifications associated with SysLink 
controls: NM_CLICK (syslink) and (for links that can be activated by the 
Enter key) NM_RETURN. If the link is a HREF, get its url from 
NMLINK.LITEM.szUrl, otherwise, get the ID from NMLINK.LITEM.szID.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: About Window Classes 
http://msdn.microsoft.com/en-us/library/ms633574.aspx

Creating Common Controls
http://winapi.foosyerdoos.org.uk/info/common_cntrls.php

MSDN: Animation Control
http://msdn.microsoft.com/en-us/library/bb761881.aspx

MSDN: ComboBoxEx Control Reference
http://msdn.microsoft.com/en-us/library/bb775740.aspx

MSDN: Up-Down Control
http://msdn.microsoft.com/en-us/library/bb759880.aspx

MSDN: Header Control
http://msdn.microsoft.com/en-us/library/bb775239.aspx

MSDN: Month Calendar Control Reference
http://msdn.microsoft.com/en-us/library/bb760917.aspx

MSDN: Date and Time Picker
http://msdn.microsoft.com/en-us/library/bb761727.aspx

MSDN: List View
http://msdn.microsoft.com/en-us/library/bb774737.aspx

MSDN: Tree View
http://msdn.microsoft.com/en-us/library/bb759988.aspx

MSDN: Tab
http://msdn.microsoft.com/en-us/library/bb760548.aspx

MSDN: ToolTip
http://msdn.microsoft.com/en-us/library/bb760246.aspx

MSDN: IP Address Control
http://msdn.microsoft.com/en-us/library/bb761374.aspx

MSDN: Status Bar
http://msdn.microsoft.com/en-us/library/bb760726.aspx

MSDN: Progress Bar
http://msdn.microsoft.com/en-us/library/bb760818.aspx

MSDN: Toolbar
http://msdn.microsoft.com/en-us/library/bb760435.aspx

MSDN: Trackbar
http://msdn.microsoft.com/en-us/library/bb760145.aspx

MSDN: SysLink
http://msdn.microsoft.com/en-us/library/bb760704.aspx


/////////////////////////////////////////////////////////////////////////////
