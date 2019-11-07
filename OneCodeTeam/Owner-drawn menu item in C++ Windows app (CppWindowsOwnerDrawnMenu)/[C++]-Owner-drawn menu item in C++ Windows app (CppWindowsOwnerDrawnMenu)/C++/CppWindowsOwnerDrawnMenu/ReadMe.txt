========================================================================
    WIN32 APPLICATION : CppWindowsOwnerDrawnMenu Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

If you need complete control over the appearance of a menu item, you can use 
an owner-drawn menu item in your application. This VC++ code sample 
demonstrates creating owner-drawn menu items. The example contains a 
Character menu whose items display regular, bold, italic, and underline texts 
in custom foreground, background and highlight colors.


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the owner-drawn menu item 
sample.

Step1. After you successfully build the sample project in Visual Studio 2008, 
you will get the application: CppWindowsOwnerDrawnMenu.exe. 

Step2. Run the application. The application ontains a Character menu with the 
"Regular", "Bold", "Italic", and "Underline" menu items. The texts on the 
items are displayed in a regular font, in bold, in italic, and in underline 
respectively. The background color is green and the text color is red. 

Step3. Close the application. 


/////////////////////////////////////////////////////////////////////////////
Creation:

Step1. Create a basic VC++ Windows Application

Create a new Visual C++ / Win32 / Win32 Project. Name it as 
CppWindowsOwnerDrawnMenu and set the Application type to Windows application 
in the Application Settings page.

Step2. Add menu items

The menu bar and drop-down menu are defined in the extended menu-template 
resource CppWindowsOwnerDrawnMenu.rc. Because a menu template cannot specify 
owner-drawn items, the Character menu initially contains four text menu items 
with the following strings: "Regular," "Bold," "Italic," and "Underline". The
IDs of the menu items are consecutive numbers.

// Menu-item identifiers for the Character menu 
 
#define IDM_CHARACTER    32771
#define IDM_REGULAR      32772
#define IDM_BOLD         32773
#define IDM_ITALIC       32774
#define IDM_UNDERLINE    32775

Step3. Set the Owner-Drawn Flag

The application's window procedure changes the "Regular," "Bold," "Italic," 
and "Underline" menu items to owner-drawn items when it processes the 
WM_CREATE message. When it receives the WM_CREATE message, the window 
procedure calls the application-defined OnCreate function, which performs the 
following steps for each menu item: 

  1) Allocates an application-defined CHARMENUITEM structure. 
  2) Gets the text of the menu item and saves it in the application-defined 
     CHARMENUITEM structure. 
  3) Creates the font used to display the menu item and saves its handle in 
     the application-defined CHARMENUITEM structure. 
  4) Changes the menu item type to MFT_OWNERDRAW and saves a pointer to the 
     application-defined CHARMENUITEM structure as item data. 

Step4. Process the Creation Event of Owner-Drawn Menu Items

Before the system displays an owner-drawn menu item for the first time, it 
sends the WM_MEASUREITEM message to the window procedure of the window that 
owns the item's menu. This message contains a pointer to a MEASUREITEMSTRUCT 
structure that identifies the item and contains the item data that an 
application may have assigned to it. The window procedure must fill the 
itemWidth and itemHeight members of the structure before returning from 
processing the message. The system uses the information in these members when 
creating the bounding rectangle in which an application draws the menu item. 
It also uses the information to detect when the user chooses the item. 

In this code sample, the OnMeasureItem function processes this message by 
selecting the font for the menu item into a device context and then 
determining the space required to display the menu item text in that font. 
The font and menu item text are both specified by the menu item's 
CHARMENUITEM structure (the structure defined by the application and 
associated with each menu item in OnCreate). The application determines the 
size of the text by using the GetTextExtentPoint32 function.

Step5. Process the Draw Event of Owner-Drawn Menu Items

Whenever a menu item must be drawn (for example, when it is first displayed 
or when the user selects it), the system sends the WM_DRAWITEM message to the 
window procedure of the menu's owner window. This message contains a pointer 
to a DRAWITEMSTRUCT structure, which contains information about the item, 
including the item data that an application may have assigned to it. In 
addition, DRAWITEMSTRUCT contains flags that indicate the state of the item 
(such as whether it is grayed or selected) as well as a bounding rectangle 
and a device context that the application uses to draw the item. 

An application must do the following while processing the WM_DRAWITEM message: 

  1) Determine the type of drawing that is necessary. To do so, check the 
     itemAction member of the DRAWITEMSTRUCT structure. 
  2) Draw the menu item appropriately, using the bounding rectangle and 
     device context obtained from the DRAWITEMSTRUCT structure. The 
     application must draw only within the bounding rectangle. For performance 
     reasons, the system does not clip portions of the image that are drawn 
     outside the rectangle. 
  3) Restore all GDI objects selected for the menu item's device context. 

If the user selects the menu item, the system sets the itemAction member of 
the DRAWITEMSTRUCT structure to the ODA_SELECT value and sets the 
ODS_SELECTED value in the itemState member. This is an application's cue to 
redraw the menu item to indicate that it is selected.

In this code sample, the OnDrawItem function processes the WM_DRAWITEM message 
by displaying the menu item text in the appropriate font. The font and menu 
item text are both specified by the menu item's CHARMENUITEM structure. The 
application selects text and background colors appropriate to the menu item's 
state.

Step6. Free Allocated Resources for Each Owner-drawn Menu Item.

The OnDestroy window procedure processes the WM_DESTROY message to destroy 
fonts and free memory. The application deletes the font and frees the 
application-defined CHARMENUITEM structure associated with each owner-drawn 
menu item.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: Using Menus / Creating Owner-Drawn Menu Items
http://msdn.microsoft.com/en-us/library/ms647558(VS.85).aspx#_win32_Creating_Owner_Drawn_Menu_Items


/////////////////////////////////////////////////////////////////////////////