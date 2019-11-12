# C# app P/Invokes a native DLL (CSPInvokeDll)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- .NET Framework
## Topics
- Interop
- P/Invoke
## Updated
- 08/22/2012
## Description

<h2><span style="font-size:14.0pt; line-height:115%">CONSOLE APPLICATION </span><span style="font-size:14.0pt; line-height:115%">(</span><span style="font-size:14.0pt; line-height:115%">CSPInvokeDll</span><span style="font-size:14.0pt; line-height:115%">)
</span></h2>
<h2>Introduction</h2>
<p class="MsoNormal">Platform Invocation Services (P/Invoke) in .NET allows managed code to call unmanaged functions that are implemented and exported in unmanaged DLLs. This VC# code sample demonstrates using P/Invoke to call the functions exported by the
 native DLLs: CppDynamicLinkLibrary.dll, user32.dll and msvcrt.dll.<span style="">
</span></p>
<h2>Running the Sample</h2>
<h2><span style=""><img src="65135-image.png" alt="" width="576" height="377" align="middle">
</span><span style=""></span></h2>
<h2>Using the Code<span style=""> </span></h2>
<h3>A. P/Invoke functions exposed from a native C&#43;&#43; DLL module. </h3>
<p class="MsoNormal">Step1. Declare the methods as having an implementation from a DLL export.
</p>
<p class="MsoNormal">First, declare the method with the <span style="font-size:10.0pt; line-height:115%; font-family:&quot;Courier New&quot;; color:blue">
static</span> and <span style="font-size:10.0pt; line-height:115%; font-family:&quot;Courier New&quot;; color:blue">
extern</span> C# keywords. Next, attach the DllImport attribute to the method. The DllImport attribute allows us to specify the name of the DLL that contains the method. The general practice is to name the C# method the same as the exported method, but we can<span style="">
</span>also use a different name for the C# method. Specify custom marshaling information for the method's parameters and return value, which will override the .NET Framework default marshaling.
</p>
<p class="MsoNormal">For example, </p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
[DllImport(&quot;CppDynamicLinkLibrary.dll&quot;, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
internal static extern int GetStringLength1(string str);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">These tools can help your write the correct P/Invoke declartions.
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Dumpbin: View the export table of a DLL </p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>PInvoke.NET: PInvoke.net is primarily a wiki, allowing developers to find,
<span style="">e</span>dit and add PInvoke* signatures, user-defined types, and any other info
<span style="">r</span>elated to calling Win32 and other unmanaged APIs from managed code such
<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>as C# or VB.NET.<span style="">&nbsp;&nbsp;&nbsp; </span></p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>PInvoke Interop Assistant: It is a toolkit that helps developers to <span style="">
e</span>fficiently convert from C to managed P/Invoke signatures or verse visa. </p>
<p class="MsoNormal">Step2. Call the methods through the PInvoke signatures. For example:
</p>
<h3>B. P/Invoke C&#43;&#43; classes exposed from a native C&#43;&#43; DLL module. </h3>
<p class="MsoNormal">There is no easy way to call the classes in a native C&#43;&#43; DLL module through P/Invoke.
<a href="http://go.microsoft.com/?linkid=9729423.">Visual C&#43;&#43; Team Blog</a> introduced a solution, but it is complicated<span style="">.
</span></p>
<p class="MsoNormal">The recommended way of calling native C&#43;&#43; class from .NET are:
</p>
<p class="MsoNormal"><span style="">&nbsp; </span>1) use a C&#43;&#43;/CLI class library to wrap the native C&#43;&#43; module, and your .NET
<span style="">&nbsp;</span>code class the C&#43;&#43;/CLI wrapper class to indirectly access the native C&#43;&#43;
<span style="">&nbsp;</span>class. </p>
<p class="MsoNormal"><span style="">&nbsp; </span>2) convert the native C&#43;&#43; module to be a COM server and expose the native
<span style="">&nbsp;</span>C&#43;&#43; class through a COM interface. Then, the .NET code can access the
<span style="">&nbsp;</span>class through .NET-COM interop. </p>
<h3>C. Unload the native DLL module. </h3>
<p class="MsoNormal">You can unload the DLL by first calling GetModuleHandle to get the handle of the module and then calling FreeLibrary to unload it.
<span style=""></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
[DllImport(&quot;kernel32.dll&quot;, CharSet = CharSet.Auto, SetLastError = true)]
static extern IntPtr GetModuleHandle(string moduleName);
[DllImport(&quot;kernel32.dll&quot;, CharSet = CharSet.Auto, SetLastError = true)]
[return: MarshalAs(UnmanagedType.Bool)]
static extern bool FreeLibrary(IntPtr hModule);
// Unload the DLL by calling GetModuleHandle and FreeLibrary. 
if (!FreeLibrary(GetModuleHandle(moduleName)))
{
    Console.WriteLine(&quot;FreeLibrary failed w/err {0}&quot;, 
    Marshal.GetLastWin32Error());
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<h2>More Information</h2>
<p class="MsoListParagraphCxSpFirst" style="margin-bottom:0cm; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:NSimSun"></span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; text-indent:5.0pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-size:9.5pt; font-family:NSimSun"><a href="http://msdn.microsoft.com/en-us/library/aa288468.aspx">MSDN: Platform Invoke Tutorial</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; text-indent:5.0pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-size:9.5pt; font-family:NSimSun"><a href="http://msdn.microsoft.com/en-us/library/aa719104.aspx">MSDN: Using P/Invoke to Call Unmanaged APIs from Your Managed Classes</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; text-indent:5.0pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-size:9.5pt; font-family:NSimSun"><a href="http://msdn.microsoft.com/en-us/magazine/cc164123.aspx">MSDN: Calling Win32 DLLs in C# with P/Invoke</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; text-indent:5.0pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-size:9.5pt; font-family:NSimSun"><a href="http://www.pinvoke.net/">PInvoke.NET</a>
</span></p>
<p class="MsoListParagraphCxSpMiddle" style="margin-bottom:0cm; margin-bottom:.0001pt; text-indent:5.0pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="font-size:9.5pt; font-family:NSimSun"><a href="http://www.codeplex.com/clrinterop">PInvoke Interop Assistant</a>
</span></p>
<p class="MsoListParagraphCxSpLast"></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
