# C++/CLI wrapper for .NET assembly (CppCLINETAssemblyWrapper)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- Library
## Topics
- Interop
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>DYNAMIC LINK LIBRARY : CppCLINETAssemblyWrapper Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
The code in this file declares the C&#43;&#43; wrapper class CSSimpleObjectWrapper for <br>
the .NET class CSSimpleObject defined in the .NET class library <br>
CSClassLibrary. Your native C&#43;&#43; application can include this wrapper class <br>
and link to the DLL to indirectly call the .NET class.<br>
<br>
&nbsp;CppCallNETAssemblyWrapper (a native C&#43;&#43; application)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;--&gt;<br>
&nbsp; &nbsp; &nbsp;CppCLINETAssemblyWrapper (this C&#43;&#43;/CLI wrapper)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;--&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;CSClassLibrary (a .NET assembly)<br>
<br>
</p>
<h3>Sample Relation:</h3>
<p style="font-family:Courier New">(The relationship between the current sample and the rest samples in
<br>
Microsoft All-In-One Code Framework <a target="_blank" href="http://1code.codeplex.com)">
http://1code.codeplex.com)</a><br>
<br>
CppCLINETAssemblyWrapper -&gt; CSClassLibrary<br>
The C&#43;&#43;/CLI sample module CppCLINETAssemblyWrapper wraps the .NET class <br>
defined in the C# class library CSClassLibrary. The wrapper class can be <br>
called by any native C&#43;&#43; applications to indirectly interoperate with the <br>
.NET class.<br>
<br>
</p>
<h3>Implementation:</h3>
<p style="font-family:Courier New"><br>
Step1. Create a Visual C&#43;&#43; / CLR / Class Library project named <br>
CppCLINETAssemblyWrapper in Visual Studio 2008. The project wizard generates <br>
a default empty C&#43;&#43;/CLI class:<br>
<br>
&nbsp; &nbsp;namespace CppCLINativeDllWrapper {<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;public ref class Class1<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;{<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;// TODO: Add your methods for this class here.<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;};<br>
&nbsp; &nbsp;}<br>
<br>
Step2. Reference the C# class library CSClassLibrary in the C&#43;&#43;/CLI class <br>
library project.<br>
<br>
Step3. Configure the C&#43;&#43;/CLI class library to export symbols. The symbols <br>
can be imported and called by native C&#43;&#43; applications.<br>
<br>
Add CPPCLINETASSEMBLYWRAPPER_EXPORTS to the preprocessor definitions of the <br>
project (see the C/C&#43;&#43; / Preprocessor page in the project Properties dialog). <br>
All files within this DLL are compiled with the symbol <br>
CPPCLINETASSEMBLYWRAPPER_EXPORTS. This symbol should not be defined on any <br>
project that uses this DLL. <br>
<br>
In the header file CppCLINETAssemblyWrapper.h, add the following definitions:<br>
<br>
&nbsp; &nbsp;#ifdef CPPCLINETASSEMBLYWRAPPER_EXPORTS<br>
&nbsp; &nbsp;#define SYMBOL_DECLSPEC __declspec(dllexport)<br>
&nbsp; &nbsp;#else<br>
&nbsp; &nbsp;#define SYMBOL_DECLSPEC&nbsp;&nbsp;&nbsp;&nbsp;__declspec(dllimport)<br>
&nbsp; &nbsp;#endif<br>
<br>
Any other project whose source files include this header file see <br>
SYMBOL_DECLSPEC classes and functions as being imported from a DLL, whereas <br>
this DLL sees symbols defined with this macro as being exported.<br>
<br>
Because the header file may be included by any other native C&#43;&#43; project, the <br>
file should only contain native C&#43;&#43; types, includes and keywords.<br>
<br>
Step4. Design the C&#43;&#43; wrapper class CSSimpleObjectWrapper for the .NET class <br>
CSimpleObject defined in the C# class library CSClassLibrary.<br>
<br>
In the header file CppCLINETAssemblyWrapper.h, declare the class.<br>
<br>
&nbsp; &nbsp;class SYMBOL_DECLSPEC CSSimpleObjectWrapper<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp;public:<br>
&nbsp; &nbsp; &nbsp; &nbsp;CSSimpleObjectWrapper(void);<br>
&nbsp; &nbsp; &nbsp; &nbsp;virtual ~CSSimpleObjectWrapper(void);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Property<br>
&nbsp; &nbsp; &nbsp; &nbsp;float get_FloatProperty(void);<br>
&nbsp; &nbsp; &nbsp; &nbsp;void set_FloatProperty(float fVal);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Method<br>
&nbsp; &nbsp; &nbsp; &nbsp;HRESULT ToString(PWSTR pszBuffer, DWORD dwSize);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Static method<br>
&nbsp; &nbsp; &nbsp; &nbsp;static int GetStringLength(PCWSTR pszString);<br>
<br>
&nbsp; &nbsp;private:<br>
&nbsp; &nbsp; &nbsp; &nbsp;void *m_impl;<br>
&nbsp; &nbsp;};<br>
<br>
The class contains a native C&#43;&#43; generic pointer (void *m_impl;) to the <br>
wrapped .NET object. It is initialized in the constructor <br>
CSSimpleObjectWrapper(void);, and the wrapped object is freed in the <br>
desctructor (virtual ~CSSimpleObjectWrapper(void);).<br>
<br>
&nbsp; &nbsp;CSSimpleObjectWrapper::CSSimpleObjectWrapper(void)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Instantiate the C# class CSSimpleObject.<br>
&nbsp; &nbsp; &nbsp; &nbsp;CSSimpleObject ^ obj = gcnew CSSimpleObject();<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Pin the CSSimpleObject .NET object, and record the address of the
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// pinned object in m_impl. <br>
&nbsp; &nbsp; &nbsp; &nbsp;m_impl = GCHandle::ToIntPtr(GCHandle::Alloc(obj)).ToPointer();
<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp;CSSimpleObjectWrapper::~CSSimpleObjectWrapper(void)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Get the GCHandle associated with the pinned object based on its
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// address, and free the GCHandle. At this point, the CSSimpleObject
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// object is eligible for GC.<br>
&nbsp; &nbsp; &nbsp; &nbsp;GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));<br>
&nbsp; &nbsp; &nbsp; &nbsp;h.Free();<br>
&nbsp; &nbsp;}<br>
<br>
The public member methods of CSSimpleObjectWrapper wraps those of the C# class <br>
CSSimpleObject. They redirects the calls to CSSimpleObject through the <br>
CSSimpleObject object pointed by m_impl. Type marshaling takes place between <br>
the managed and native code.<br>
<br>
&nbsp; &nbsp;float CSSimpleObjectWrapper::get_FloatProperty(void)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Get the pinned CSSimpleObject object from its memory address.<br>
&nbsp; &nbsp; &nbsp; &nbsp;GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));<br>
&nbsp; &nbsp; &nbsp; &nbsp;CSSimpleObject ^ obj = safe_cast&lt;CSSimpleObject^&gt;(h.Target);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Redirect the call to the corresponding property of the wrapped
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// CSSimpleObject object.<br>
&nbsp; &nbsp; &nbsp; &nbsp;return obj-&gt;FloatProperty;<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp;void CSSimpleObjectWrapper::set_FloatProperty(float fVal)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Get the pinned CSSimpleObject object from its memory address.<br>
&nbsp; &nbsp; &nbsp; &nbsp;GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));<br>
&nbsp; &nbsp; &nbsp; &nbsp;CSSimpleObject ^ obj = safe_cast&lt;CSSimpleObject^&gt;(h.Target);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Redirect the call to the corresponding property of the wrapped
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// CSSimpleObject object.<br>
&nbsp; &nbsp; &nbsp; &nbsp;obj-&gt;FloatProperty = fVal;<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp;HRESULT CSSimpleObjectWrapper::ToString(PWSTR pszBuffer, DWORD dwSize)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Get the pinned CSSimpleObject object from its memory address.<br>
&nbsp; &nbsp; &nbsp; &nbsp;GCHandle h = GCHandle::FromIntPtr(IntPtr(m_impl));<br>
&nbsp; &nbsp; &nbsp; &nbsp;CSSimpleObject ^ obj = safe_cast&lt;CSSimpleObject^&gt;(h.Target);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;String ^ str;<br>
&nbsp; &nbsp; &nbsp; &nbsp;HRESULT hr;<br>
&nbsp; &nbsp; &nbsp; &nbsp;try<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Redirect the call to the corresponding method of the wrapped
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// CSSimpleObject object.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;str = obj-&gt;ToString();<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;catch (Exception ^ e)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;hr = Marshal::GetHRForException(e);<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (SUCCEEDED(hr))<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Convert System::String to PCWSTR.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;marshal_context ^ context = gcnew marshal_context();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;PCWSTR pszStr = context-&gt;marshal_as&lt;const wchar_t*&gt;(str);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;hr = StringCchCopy(pszBuffer, dwSize, pszStr == NULL ? L&quot;&quot; : pszStr);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;delete context; // This will also free the memory pointed by pszStr<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;return hr;<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp;int CSSimpleObjectWrapper::GetStringLength(PCWSTR pszString)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;return CSSimpleObject::GetStringLength(gcnew String(pszString));<br>
&nbsp; &nbsp;}<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
Using C&#43;&#43; Interop (Implicit PInvoke)<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/2x8kf7zx.aspx">http://msdn.microsoft.com/en-us/library/2x8kf7zx.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
