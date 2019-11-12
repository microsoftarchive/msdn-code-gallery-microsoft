# C++/CLI wrapper for native DLL (CppCLINativeDllWrapper)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- .NET Framework
## Topics
- Interop
## Updated
- 08/22/2012
## Description

<h1><span style="font-family:������">DYNAMIC LINK LIBRARY </span>(<span class="SpellE"><span style="font-family:������">CppCLINativeDllWrapper</span></span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
This C&#43;&#43;/CLI code sample demonstrates making C&#43;&#43;/CLI wrapper classes for a native C&#43;&#43; DLL module that allow you to call from any .NET code to the classes and functions exported by the module.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span>&nbsp; </span><span class="SpellE">CSCallNativeDllWrapper</span>/<span class="SpellE">VBCallNativeDllWrapper</span> (any .NET clients)<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>--&gt;<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span><span class="SpellE">CppCLINativeDllWrapper</span> (this C&#43;&#43;/CLI wrapper)<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>--&gt;<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span>
<span class="SpellE">CppDynamicLinkLibrary</span> (a native C&#43;&#43; DLL module)</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
In this code sample, the <span class="SpellE">CSimpleObjectWrapper</span> class wraps the native C&#43;&#43; class
<span class="SpellE">CSimpleObject</span>, and the <span class="SpellE">NativeMethods</span> class wraps the global functions exported by CppDynamicLinkLibrary.dll.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
The interoperability features supported by Visual C&#43;&#43;/CLI offer a particular advantage over other .NET languages when it comes to interoperating with native modules. Apart from the traditional explicit P/Invoke, C&#43;&#43;/CLI allows implicit P/Invoke, also known
 as C&#43;&#43; <span class="SpellE">Interop</span>, or It Just Work (IJW), which mixes managed code and native code almost invisibly. The feature provides better type safety, easier coding, greater performance, and is more forgiving if the native API is modified.
 You can use the technology to build .NET wrappers for native C&#43;&#43; classes and functions if their source code is available, and allow any .NET clients to access the native C&#43;&#43; classes and functions through the wrappers.<span>
</span></p>
<h2>Using the Code</h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Step1. Create a Visual C&#43;&#43; / CLR / Class Library project named
<span class="SpellE">CppCLINativeDllWrapper</span> in Visual Studio 2010. The project wizard generates a default empty C&#43;&#43;/CLI class:
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">&nbsp;</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Step2. Reference the native C&#43;&#43; DLL
<span class="SpellE">CppDynamicLinkLibrary</span>. </span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><span>&nbsp;
</span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Option1.
 Link the LIB file of the DLL by entering the LIB file name in<span>&nbsp;&nbsp; </span>
Project Properties / Linker / Input / Additional Dependencies. We can<span>&nbsp;&nbsp;
</span>configure the search path of the LIB file in Project <span class="SpellE">
roperties</span> / Linker /<span>&nbsp;&nbsp; </span>General / Additional Library Directories.
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><span>&nbsp;
</span>Option2. Select References from the Project's shortcut menu. On the<span>&nbsp;&nbsp;
</span>Property Pages dialog box, expand the Common Properties node, select<span>&nbsp;&nbsp;
</span>References, and then select the Add New Reference... button. The Add<span>&nbsp;&nbsp;
</span>Reference dialog box is displayed. This dialog lists all the libraries <span class="GramE">
that<span>&nbsp; </span>you</span> can reference. The Projects tab lists all the projects in the current<span>&nbsp;&nbsp;
</span>solution and any libraries they contain. If the <span class="SpellE">CppDynamicLinkLibrary</span><span>&nbsp;&nbsp;
</span>project is in the current solution, select <span class="SpellE">CppDynamicLinkLibrary</span> and click<span>&nbsp;&nbsp;
</span>OK in the Projects tab. </span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Step3. Including the header file that declares the functions and classes of the DLL.
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">You can configure the search path of the header file in Project Properties / C/C&#43;&#43; / General / Additional Include Directories.
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Step4. Design the C&#43;&#43;/CLI wrapper class
<span class="SpellE">CSimpleObjectWrapper</span> for the native C&#43;&#43; class <span class="SpellE">
CSimpleObject</span>. </span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">&nbsp;</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">The class wraps an instance of the native C&#43;&#43; class
<span class="SpellE">CSimpleObject</span>. The instance is pointed by the private member variable
<span class="SpellE">m_impl</span>. It is initialized in the constructor <span class="SpellE">
<span class="GramE">CSimpleObjectWrapper</span></span><span class="GramE">(</span>void);, and is freed in the
<span class="SpellE">desctructor</span> (virtual ~<span class="SpellE">CSimpleObjectWrapper</span>(void);) and the
<span class="SpellE">finalizer</span> (!<span class="SpellE">CSimpleObjectWrapper</span>(void);).
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">&nbsp;</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">The public member properties and methods of
<span class="SpellE">CSimpleObjectWrapper</span> <span class="GramE">wraps</span> those of the native C&#43;&#43; class
<span class="SpellE">CSimpleObject</span>. They <span class="GramE">redirects</span> the calls to
<span class="SpellE">CSimpleObject</span> through the <span class="SpellE">CSimpleObject</span> instance pointed by
<span class="SpellE">m_impl</span>. Type marshaling takes place between the managed and native code.
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">&nbsp;</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Step5. Design the C&#43;&#43;/CLI wrapper class
<span class="SpellE">NativeMethods</span> for the <span class="GramE">functions
<span><span>&nbsp;</span></span>exported</span> by the native C&#43;&#43; DLL module. </span>
</h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">All methods in
<span class="SpellE">NativeMethods</span> are declared as static for the global functions exported by
<span class="SpellE">CppDynamicLinkLibrary</span>. They redirect calls to the native DLL.</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
</span></h2>
<p class="MsoNormal"><span>&nbsp;</span></p>
<h2>More Information</h2>
<p class="MsoListParagraphCxSpFirst"><span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/2x8kf7zx.aspx">Using C&#43;&#43;
<span class="SpellE">Interop</span> (Implicit <span class="SpellE">PInvoke</span>)</a></p>
<p class="MsoListParagraphCxSpLast"><span style="font-family:Symbol"><span>��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/ms235281.aspx">How to: Wrap Native Class for Use by C#</a></p>
<p class="MsoNormal">&nbsp;</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
