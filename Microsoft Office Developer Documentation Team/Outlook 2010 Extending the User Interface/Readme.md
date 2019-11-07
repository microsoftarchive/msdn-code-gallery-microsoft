# Outlook 2010: Extending the User Interface
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- Outlook 2010
- Office 2010
## Topics
- Ribbon Extensibility
- customizing the user interface
## Updated
- 08/01/2011
## Description

<h2><strong>Introduction</strong></h2>
<p>Learn how to programmatically extend the Microsoft Office Fluent user interface (UI) to customize the UI in Microsoft Outlook 2010. The Fluent UI in Microsoft Office 2010 includes the Office Fluent ribbon, menus, and Microsoft Office Backstage view. The
 code sample illustrates how to customize the ribbon, the context menus, and Backstage view, and examines specific issues that apply to the Outlook 2010 UI. This sample accompanies the article
<a href="http://msdn.microsoft.com/en-us/library/ee692172(office.14).aspx">Extending the User Interface in Outlook 2010</a> in the MSDN Library.</p>
<h2><strong>Description</strong></h2>
<p>The sample add-in demonstrates how to customize the ribbons, menus, context menus, and Backstage view in Outlook 2010. Developed in Visual Studio 2008 Tools for Office, the add-in adds ribbon controls, a custom menu, context menu items, and Backstage view
 controls, and then displays a message box when the user clicks the control or menu item. The add-in provides a visual element for every entry point in the Outlook 2010 UI that you can customize by using Fluent UI extensibility.</p>
<p>The sample add-in has some additional features that illustrate how to manage certain problem areas in the Outlook 2010 UI. For example, suppose that you want to show a custom group in the ribbon for received e-mail items only. The sample add-in displays
 the custom ribbon tab only when the selected item in the Outlook explorer is a received mail item or when the received mail item is displayed in an inspector. Although that might seem to be a trivial task at first, it is actually a complex problem because
 Outlook can display multiple explorer or inspector windows, and your code must be able to respond appropriately. For example, in two open explorer windows, you might have to hide the custom tab in the window that has a meeting selected, but display the custom
 tab in the window that has a received mail item selected. Once you understand how the sample add-in works, you can use the wrapper classes from the sample to build your own solution that can coordinate the display of your command UI in multiple Outlook windows.</p>
