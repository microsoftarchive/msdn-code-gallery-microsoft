# C++ app automates Excel (CppAutomateExcel)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Office
## Topics
- Excel
- Automation
## Updated
- 03/02/2012
## Description

<h1><span style="font-family:������">CONSOLE APPLICATION</span> (<span style="font-family:������">CppAutomateExcel</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The CppAutomateExcel example demonstrates how to write VC&#43;&#43; codes to create a<span style="">&nbsp;
</span>Microsoft Excel instance, create a workbook, fill data into a specific range, save the workbook, close the Microsoft Excel application and then clean up unmanaged COM resources.
</p>
<p class="MsoNormal">There are three basic ways you can write VC&#43;&#43; automation codes:
</p>
<p class="MsoNormal">1. Automating Excel using the #import directive and smart pointers
</p>
<p class="MsoNormal">The code in Solution1.h/cpp demonstrates the use of #import to automate Excel. #import (http://msdn.microsoft.com/en-us/library/8etzzkb6.aspx), a new<span style="">&nbsp;
</span>directive that became available with Visual C&#43;&#43; 5.0, creates VC&#43;&#43; &quot;smart pointers&quot; from a specified type library. It is very powerful, but often not recommended because of reference-counting problems that typically occur when used with the
 Microsoft Office applications. Unlike the direct API approach in Solution2.h/cpp, smart pointers enable us to benefit from the type info to early/late bind the object. #import takes care of adding the messy guids to
</p>
<p class="MsoNormal">the project and the COM APIs are encapsulated in custom classes that the #import directive generates.
</p>
<p class="MsoNormal">2. Automating Excel using C&#43;&#43; and the COM APIs </p>
<p class="MsoNormal">The code in Solution2.h/cpp demontrates the use of C/C&#43;&#43; and the COM APIs to automate Excel. The raw automation is much more difficult, but it is sometimes necessary to avoid the overhead with MFC, or problems with #import. Basically,
 you work with such APIs as CoCreateInstance(), and COM interfaces such as IDispatch and IUnknown.
</p>
<p class="MsoNormal">3. Automating Excel with MFC </p>
<p class="MsoNormal">With MFC, Visual C&#43;&#43; Class Wizard can generate &quot;wrapper classes&quot; from the type libraries. These classes simplify the use of the COM servers. Automating Excel with MFC is not covered in this sample.<span style="">&nbsp;
</span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal">The following steps walk through a demonstration of the Excel automation sample that starts a Microsoft Excel instance, creates a workbook, fills data into a specified range, saves the workbook, and quits the Microsoft Excel application
 cleanly. </p>
<p class="MsoNormal">Step1. After you successfully build the sample project in Visual Studio 2010, you will get the application: CppAutomateExcel.exe.
</p>
<p class="MsoNormal">Step2. Open Windows Task Manager (Ctrl&#43;Shift&#43;Esc) to confirm that no Excel.exe is running.
</p>
<p class="MsoNormal">Step3. Run the application. It should print the following content in the console window if no error is thrown.
</p>
<p class="MsoNormal"><span style=""><img src="53396-image.png" alt="" width="576" height="310" align="middle">
</span><span style="">&nbsp; </span></p>
<p class="MsoNormal">Then, you will see two new workbooks in the directory of the application: Sample1.xlsx and Sample2.xlsx. Both workbooks contain a worksheet named &quot;Report&quot;. The worksheet has the following data in the range A2:B6.
</p>
<p class="MsoNormal"><span style=""><img src="53397-image.png" alt="" width="576" height="345" align="middle">
</span></p>
<p class="MsoNormal">Step4. In Windows Task Manager, confirm that the Excel.exe process does not exist, i.e. the Microsoft Excel intance was closed and cleaned up properly.
</p>
<h2><span style="">Using the code</span> </h2>
<h3>A. Automating Excel using the #import directive and smart pointers (Solution1.h/cpp)
</h3>
<p class="MsoNormal">Step1. Import the type library of the target COM server using the #import directive.
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
#import &quot;libid:2DF8D04C-5BFA-101B-BDE5-00AA0044DE52&quot; \
    rename(&quot;RGB&quot;, &quot;MSORGB&quot;)
    rename(&quot;DocumentProperties&quot;, &quot;MSODocumentProperties&quot;)
// [-or-]
//#import &quot;C:\\Program Files\\Common Files\\Microsoft Shared\\OFFICE12\\MSO.DLL&quot; \
//    rename(&quot;RGB&quot;, &quot;MSORGB&quot;)
//    rename(&quot;DocumentProperties&quot;, &quot;MSODocumentProperties&quot;)
using namespace Office;
#import &quot;libid:0002E157-0000-0000-C000-000000000046&quot;
// [-or-]
//#import &quot;C:\\Program Files\\Common Files\\Microsoft Shared\\VBA\\VBA6\\VBE6EXT.OLB&quot;
using namespace VBIDE;
#import &quot;libid:00020813-0000-0000-C000-000000000046&quot; \
    rename(&quot;DialogBox&quot;, &quot;ExcelDialogBox&quot;) \
    rename(&quot;RGB&quot;, &quot;ExcelRGB&quot;) \
    rename(&quot;CopyFile&quot;, &quot;ExcelCopyFile&quot;) \
    rename(&quot;ReplaceText&quot;, &quot;ExcelReplaceText&quot;) \
    no_auto_exclude
// [-or-]
//#import &quot;C:\\Program Files\\Microsoft Office\\Office12\\EXCEL.EXE&quot; \
//    rename(&quot;DialogBox&quot;, &quot;ExcelDialogBox&quot;) \
//    rename(&quot;RGB&quot;, &quot;ExcelRGB&quot;) \
//    rename(&quot;CopyFile&quot;, &quot;ExcelCopyFile&quot;) \
//    rename(&quot;ReplaceText&quot;, &quot;ExcelReplaceText&quot;) \
//    no_auto_exclude

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step2. Build the project. If the build is successful, the compiler generates the .tlh and .tli files that encapsulate the COM server based on the type library specified in the #import directive. It serves as a class wrapper we<span style="">
</span>can now use to create the COM class and access its properties, methods, etc.
</p>
<p class="MsoNormal">Step3. Initializes the COM library on the current thread and identifies the concurrency model as single-thread apartment (STA) by calling CoInitializeEx, or CoInitialize.
</p>
<p class="MsoNormal">Step4. Create the Excel.Application COM object using the smart pointer. The class name is the original interface name (i.e. Excel::_Application) with a &quot;Ptr&quot; suffix. We can use either the constructor of the smart pointer class
 or its CreateInstance method to create the COM object. </p>
<p class="MsoNormal">Step5. Automate the Excel COM object through the smart pointers. In this example, you can find the basic operations in Excel automation like
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp; </span>Create a new Workbook. (i.e. Application.Workbooks.Add)
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp; </span>Get the active Worksheet, and rename it to be &quot;Report&quot;.
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp; </span>Fill data into the worksheet's cells.
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp; </span>Save the workbook as a xlsx file and close it.
</p>
<p class="MsoNormal">Step6. Quit the Excel application. (i.e. Application.Quit())
</p>
<p class="MsoNormal">Step7. It is said that the smart pointers are released automatically, so we do not need to manually release the COM object.
</p>
<p class="MsoNormal">Step8. It is necessary to catch the COM errors if the type library was<span style="">&nbsp;
</span>imported without raw_interfaces_only and when the raw interfaces (e.g. raw_Quit) are not used. For example:
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
#import &quot;XXXX.tlb&quot;
try
{
    spXlApp-&gt;Quit();
}
catch (_com_error &err)
{
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step9. Uninitialize COM for this thread by calling <span class="SpellE">
CoUninitialize</span>. </p>
<h3>B. Automating Excel using C&#43;&#43; and the COM APIs (Solution2.h/cpp) </h3>
<p class="MsoNormal">Step1. Add the automation helper function, AutoWrap. </p>
<p class="MsoNormal">Step2. Initializes the COM library on the current thread and identifies the concurrency model as single-thread apartment (STA) by calling CoInitializeEx, or CoInitialize.
</p>
<p class="MsoNormal">Step3. Get CLSID of the Excel COM server using the API CLSIDFromProgID.
</p>
<p class="MsoNormal">Step4. Start the Excel COM server and get the IDispatch interface using the API CoCreateInstance.
</p>
<p class="MsoNormal">Step5. Automate the Excel COM object with the help of AutoWrap. In this example, you can find the basic operations in Excel automation like
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp; </span>Create a new Workbook. (i.e. Application.Workbooks.Add)
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp; </span>Get the active Worksheet, and rename it to be &quot;Report&quot;.
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp; </span>Fill data into the worksheet's cells.
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp; </span>Save the workbook as a xlsx file and close it.
</p>
<p class="MsoNormal">Step6. Quit the Excel application. (i.e. Application.Quit())
</p>
<p class="MsoNormal">Step7. Release the COM objects. </p>
<p class="MsoNormal">Step8. Uninitialize COM for this thread by calling CoUninitialize.
</p>
<h2>More Information </h2>
<p class="MsoNormal"><a href="http://msdn.microsoft.com/en-us/library/bb149067.aspx">MSDN: Excel 2007 Developer Reference</a>
</p>
<p class="MsoNormal"><a href="http://support.microsoft.com/kb/196776/">Office Automation Using Visual C&#43;&#43;</a>
</p>
<p class="MsoNormal"><a href="http://support.microsoft.com/kb/216686">How to automate Excel from C&#43;&#43; without using MFC or #import</a>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
