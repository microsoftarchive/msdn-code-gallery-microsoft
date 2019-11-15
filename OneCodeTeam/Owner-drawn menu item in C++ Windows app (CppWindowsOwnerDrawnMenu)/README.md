# Owner-drawn menu item in C++ Windows app (CppWindowsOwnerDrawnMenu)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- Windows UI
## Topics
- Menu
- OwnerDrawn
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>WIN32 APPLICATION : CppWindowsOwnerDrawnMenu Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
If you need complete control over the appearance of a menu item, you can use <br>
an owner-drawn menu item in your application. This VC&#43;&#43; code sample <br>
demonstrates creating owner-drawn menu items. The example contains a <br>
Character menu whose items display regular, bold, italic, and underline texts <br>
in custom foreground, background and highlight colors.<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
The following steps walk through a demonstration of the owner-drawn menu item <br>
sample.<br>
<br>
Step1. After you successfully build the sample project in Visual Studio 2008, <br>
you will get the application: CppWindowsOwnerDrawnMenu.exe. <br>
<br>
Step2. Run the application. The application ontains a Character menu with the <br>
&quot;Regular&quot;, &quot;Bold&quot;, &quot;Italic&quot;, and &quot;Underline&quot; menu items. The texts on the
<br>
items are displayed in a regular font, in bold, in italic, and in underline <br>
respectively. The background color is green and the text color is red. <br>
<br>
Step3. Close the application. <br>
<br>
</p>
<h3>Creation:</h3>
<p style="font-family:Courier New"><br>
Step1. Create a basic VC&#43;&#43; Windows Application<br>
<br>
Create a new Visual C&#43;&#43; / Win32 / Win32 Project. Name it as <br>
CppWindowsOwnerDrawnMenu and set the Application type to Windows application <br>
in the Application Settings page.<br>
<br>
Step2. Add menu items<br>
<br>
The menu bar and drop-down menu are defined in the extended menu-template <br>
resource CppWindowsOwnerDrawnMenu.rc. Because a menu template cannot specify <br>
owner-drawn items, the Character menu initially contains four text menu items <br>
with the following strings: &quot;Regular,&quot; &quot;Bold,&quot; &quot;Italic,&quot; and &quot;Underline&quot;. The<br>
IDs of the menu items are consecutive numbers.<br>
<br>
// Menu-item identifiers for the Character menu <br>
<br>
#define IDM_CHARACTER &nbsp; &nbsp;32771<br>
#define IDM_REGULAR &nbsp; &nbsp; &nbsp;32772<br>
#define IDM_BOLD &nbsp; &nbsp; &nbsp; &nbsp; 32773<br>
#define IDM_ITALIC &nbsp; &nbsp; &nbsp; 32774<br>
#define IDM_UNDERLINE &nbsp; &nbsp;32775<br>
<br>
Step3. Set the Owner-Drawn Flag<br>
<br>
The application's window procedure changes the &quot;Regular,&quot; &quot;Bold,&quot; &quot;Italic,&quot;
<br>
and &quot;Underline&quot; menu items to owner-drawn items when it processes the <br>
WM_CREATE message. When it receives the WM_CREATE message, the window <br>
procedure calls the application-defined OnCreate function, which performs the <br>
following steps for each menu item: <br>
<br>
&nbsp;1) Allocates an application-defined CHARMENUITEM structure. <br>
&nbsp;2) Gets the text of the menu item and saves it in the application-defined <br>
&nbsp; &nbsp; CHARMENUITEM structure. <br>
&nbsp;3) Creates the font used to display the menu item and saves its handle in <br>
&nbsp; &nbsp; the application-defined CHARMENUITEM structure. <br>
&nbsp;4) Changes the menu item type to MFT_OWNERDRAW and saves a pointer to the <br>
&nbsp; &nbsp; application-defined CHARMENUITEM structure as item data. <br>
<br>
Step4. Process the Creation Event of Owner-Drawn Menu Items<br>
<br>
Before the system displays an owner-drawn menu item for the first time, it <br>
sends the WM_MEASUREITEM message to the window procedure of the window that <br>
owns the item's menu. This message contains a pointer to a MEASUREITEMSTRUCT <br>
structure that identifies the item and contains the item data that an <br>
application may have assigned to it. The window procedure must fill the <br>
itemWidth and itemHeight members of the structure before returning from <br>
processing the message. The system uses the information in these members when <br>
creating the bounding rectangle in which an application draws the menu item. <br>
It also uses the information to detect when the user chooses the item. <br>
<br>
In this code sample, the OnMeasureItem function processes this message by <br>
selecting the font for the menu item into a device context and then <br>
determining the space required to display the menu item text in that font. <br>
The font and menu item text are both specified by the menu item's <br>
CHARMENUITEM structure (the structure defined by the application and <br>
associated with each menu item in OnCreate). The application determines the <br>
size of the text by using the GetTextExtentPoint32 function.<br>
<br>
Step5. Process the Draw Event of Owner-Drawn Menu Items<br>
<br>
Whenever a menu item must be drawn (for example, when it is first displayed <br>
or when the user selects it), the system sends the WM_DRAWITEM message to the <br>
window procedure of the menu's owner window. This message contains a pointer <br>
to a DRAWITEMSTRUCT structure, which contains information about the item, <br>
including the item data that an application may have assigned to it. In <br>
addition, DRAWITEMSTRUCT contains flags that indicate the state of the item <br>
(such as whether it is grayed or selected) as well as a bounding rectangle <br>
and a device context that the application uses to draw the item. <br>
<br>
An application must do the following while processing the WM_DRAWITEM message: <br>
<br>
&nbsp;1) Determine the type of drawing that is necessary. To do so, check the <br>
&nbsp; &nbsp; itemAction member of the DRAWITEMSTRUCT structure. <br>
&nbsp;2) Draw the menu item appropriately, using the bounding rectangle and <br>
&nbsp; &nbsp; device context obtained from the DRAWITEMSTRUCT structure. The <br>
&nbsp; &nbsp; application must draw only within the bounding rectangle. For performance
<br>
&nbsp; &nbsp; reasons, the system does not clip portions of the image that are drawn
<br>
&nbsp; &nbsp; outside the rectangle. <br>
&nbsp;3) Restore all GDI objects selected for the menu item's device context. <br>
<br>
If the user selects the menu item, the system sets the itemAction member of <br>
the DRAWITEMSTRUCT structure to the ODA_SELECT value and sets the <br>
ODS_SELECTED value in the itemState member. This is an application's cue to <br>
redraw the menu item to indicate that it is selected.<br>
<br>
In this code sample, the OnDrawItem function processes the WM_DRAWITEM message <br>
by displaying the menu item text in the appropriate font. The font and menu <br>
item text are both specified by the menu item's CHARMENUITEM structure. The <br>
application selects text and background colors appropriate to the menu item's <br>
state.<br>
<br>
Step6. Free Allocated Resources for Each Owner-drawn Menu Item.<br>
<br>
The OnDestroy window procedure processes the WM_DESTROY message to destroy <br>
fonts and free memory. The application deletes the font and frees the <br>
application-defined CHARMENUITEM structure associated with each owner-drawn <br>
menu item.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: Using Menus / Creating Owner-Drawn Menu Items<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms647558(VS.85).aspx#_win32_Creating_Owner_Drawn_Menu_Items">http://msdn.microsoft.com/en-us/library/ms647558(VS.85).aspx#_win32_Creating_Owner_Drawn_Menu_Items</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
