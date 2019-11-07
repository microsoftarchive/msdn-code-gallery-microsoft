========================================================================
    VSTO APPLICATION : CSOfficeRibbonAccessibility Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

This example illustrates how to pinvoke the Microsoft Active Accessibilty 
(MSAA) API to automate Office Ribbon controls. The code calls the following 
APIs,

    AccessibleObjectFromWindow, 
    AccessibleChildren,
    GetRoleText,
    GetStateText,

to display the whole structure of the Office ribbon, including tabs, groups,
and controls. It also shows how to nagivate to a tab and execute button 
function programmatically.


/////////////////////////////////////////////////////////////////////////////
Prerequisties:

*Visual Studio 2010 and Office 2010
or 
*Visual Studio 2008 and Office 2007


/////////////////////////////////////////////////////////////////////////////
Demo:

1. Compile the VSTO project using Visual Studio 2008 or 2010.

2. Open your Office Word 2007 or 2010. You should see a group named 
"CSOfficeRibbonAccessibility" in the Add-Ins ribbon tab. In the group, there 
is a button "Show Ribbon Information".

3. Click the "Show Ribbon Information" button. You will see a windows form 
with the title "Ribbon Information".

4. In the "Tabs" list box, you can see all visible tabs. Clicking any of them 
make the office navigate to that tab.

6. By click the button "List child groups", the form will list all groups in 
the currently selected tab in the "Groups" list box.

7. Select a group and click the button "List child controls". The form will 
list all controls in the currently selected group in the "Controls" list box. 

8. Select a control, and click the button "Execute selected control". the form
will invoke the selected control's function.


/////////////////////////////////////////////////////////////////////////////
Implementation:

A. Creating the project

Step1. Create a VSTO Word 2010 AddIn using Visual Studio 2010. Right click the 
project, and click Add->New Item. In the popup dialog, choose Office tab,  
select the Ribbon (Visual Designer) at the right panel and click OK. In the 
Ribbon designer, drag a button to the Add-ins Tab. Double click the button to 
generate a ribbon control click event handler.

Step2. Right click the project, and click Add->New Item. In the popup dialog, 
choose the Windows Form tab, and add a Form named RibbonInfoForm. In the 
Form designer, drag labels, listboxs, and buttons as the sample shows.

Step3. In the event handler created in step 1, we add code to show the 
RibbonInfoForm form.

	RibbonInfoForm form = new RibbonInfoForm();
    form.Show();

----------------

B. Using the Microsoft Active Accessibility API

Microsoft Active Accessibility (MSAA) is an API for user interface 
accessibility. The Accessibility interface offers a hierarchy of objects 
subordinate to the main window of the Office 2007/2010 applications. You can 
use the API to automate Office ribbon controls. 

NativeMethods.cs declares the P/Invoke signatures for MSAA APIs:
    AccessibleChildren
    AccessibleObjectFromWindow
    GetRoleText
    GetStateText

MSAAHelper.cs provides helper functions that invoke the native MSAA APIs:
    GetAccessibleChildren
    GetAccessibleObjectListByRole
    GetAccessibleObjectFromHandle
    GetAccessibleObjectByNameAndRole
    GetRoleText
    GetStateText

In RibbonInfoForm.cs, the form lists all accessible ribbon tab, group and 
control objects, and execute their default actions. The sample basically does 
the following logic:

  1. Find the top window of the Office application, and get its IAccessible 
     interface. 
  2. Get the IAccessibile interface for the ribbon object.
  3. Recurse through the IAccessibility tree and find the tabs, groups and 
     controls. We use the accessible name and role to identify the tabs, 
     groups and controls. You can use the Inspect tool in the Windows SDK to 
     inspet the accessible name and role of the Office ribbon.
     http://msdn.microsoft.com/en-us/library/dd318521.aspx
  4. Execute the default action (in case of ribbon tabs the default action is 
     switch, for buttons it’s click and for the dropdowns it’s drop).


/////////////////////////////////////////////////////////////////////////////
References:

How to switch ribbon tab programmatically... looks simple?
http://blogs.msdn.com/b/pranavwagh/archive/2008/01/21/how-to-switch-ribbon-tab-programmatically-looks-simple.aspx

Microsoft Active Accessibility
http://msdn.microsoft.com/en-us/library/ms697707.aspx

Accessing the Ribbon with VBA
http://www.wordarticles.com/Shorts/RibbonVBA/RibbonVBADemo.htm

UI Automation using Microsoft Active Accessibility(MSAA)
http://www.codeproject.com/KB/winsdk/MSAA_UI_Automation.aspx


/////////////////////////////////////////////////////////////////////////////
