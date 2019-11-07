# Outlook 2010: Building a C++ Add-In
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Outlook 2010
- Office 2010
## Topics
- Ribbon Extensibility
- C++ add-ins
## Updated
- 08/02/2011
## Description

<h2><strong>Introduction</strong></h2>
<p>Create an unmanaged add-in in C&#43;&#43; that customizes the Microsoft Outlook 2010 user interface. Learn how to customize the ribbon component of the Microsoft Office Fluent user interface, and to add custom form regions to Outlook 2010. This sample accompanies
 the article <a href="http://msdn.microsoft.com/en-us/library/ee941475(office.14).aspx">
Building a C&#43;&#43; Add-in for Outlook 2010</a> in the MSDN Library.</p>
<h2><strong>Description</strong></h2>
<p>Microsoft Outlook has a rich model for building add-ins that can greatly enhance a user's day-to-day experience with Outlook. To build Outlook add-ins, you can use Microsoft Office development tools in Microsoft Visual Studio 2010 (or the Microsoft Visual
 Studio Tools for Office for earlier versions of Visual Studio), which is a set of managed libraries built on top of the Outlook and Microsoft Office object models. Alternatively, you can use unmanaged code (that is, C&#43;&#43;).</p>
<p>This sample creates a C&#43;&#43; add-in and uses two common extensibility mechanisms: a custom ribbon and a form region. With a custom ribbon, you can add UI, such as a button, to the ribbon and allow your add-in to respond to context-based UI events. With a custom
 form region, you can add UI elements, such as a <strong>WebBrowser</strong> control, to Outlook forms.</p>
<p>This sample is intended for intermediate and advanced developers who have experience developing applications in C&#43;&#43;. The accompanying article does not necessarily enumerate in detail common procedures in Visual Studio.</p>
<p style="padding-left:30px"><strong>Note:</strong> This sample uses the Bing API only as an example to provide functionality to a custom user interface. You can connect the custom button in the Outlook inspector ribbon with an API such as the Outlook object
 model. If you use the Bing API in your application, be sure that your code conforms to the currently supported version of Bing.</p>
