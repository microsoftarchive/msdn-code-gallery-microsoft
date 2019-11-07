# GDC 2013 Windows Developer Content Direct3D Game Templates
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Direct3D
## Topics
- Graphics
- Graphics and 3D
## Updated
- 11/22/2013
## Description

<h1>Introduction</h1>
<p>&nbsp;</p>
<p><span style="font-size:small">The&nbsp;Windows Content Team&nbsp;GDC 2013 Direct3D game templates are a set of Visual Studio project templates for Windows Store Direct3D apps.&nbsp;The GDC 2013 versions of the content incorporate code from these templates
</span><span style="font-size:small">in the coding examples.</span></p>
<p><span style="font-size:small">Note: Due to reported compatibility issues, we have removed the Visual Studio 2013 version of this template. The
<a href="http://go.microsoft.com/fwlink/?LinkID=287055">WDC DirectX game learning template</a> is designed for Visual Studio 2013, Windows 8.1, and DirectX 11.2. It also includes new helper classes and has&nbsp;<a href="http://msdn.microsoft.com/library/windows/apps/dn481529.aspx">supporting
 documentation</a>. If you prefer to use this template instead, you can create a GDC 2013 game template project in Visual Studio 2012 and import it into Visual Studio 2013.</span></p>
<p><span style="font-size:small">The ZIP file includes a single VSIX with three templates:</span></p>
<ul>
<li><span style="font-size:small"><strong>DirectX game (CoreWindow)</strong>. This template constructs a basic CoreWindow view
</span><span style="font-size:small">provider with a game-development friendly layout that demonstrates DirectX 11.1</span>
<span style="font-size:small">and C&#43;&#43; coding best practices for Windows Store apps.</span>
</li><li><span style="font-size:small"><strong>DirectX game (XAML)</strong>. This template uses the XAML framework with&nbsp; SwapChainBackgroundPanel
</span><span style="font-size:small">to display Direct3D 11.1 rendering output. It demonstrates DirectX 11.1 and C&#43;&#43;
</span><span style="font-size:small">coding best practices for Windows Store apps with XAML.</span>
</li><li><span style="font-size:small"><strong>DirectX game (XAML and CoreWindow)</strong>. This template creates two swap chains &ndash; one with
</span><span style="font-size:small">a CoreWindow view provider and the other with the XAML framework and
</span><span style="font-size:small">SwapChainBackgroundPanel &ndash; and allows you to dynamically switch between the two
</span><span style="font-size:small">based on game state or user input.</span> </li></ul>
<p>&nbsp;</p>
<p><span style="font-size:small">The following GDC 2013 documentation incorporates these</span>
<span style="font-size:small">samples:</span></p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<br>
<span style="font-size:small"><a href="http://go.microsoft.com/fwlink/?LinkId=288796">Prepare your Windows Store DirectX game programming environment</a>&nbsp;</span></p>
<p><span style="font-size:small"><a href="http://go.microsoft.com/fwlink/?LinkId=288797">Port from DirectX 9 to Windows Store</a>&nbsp;</span></p>
<p><span style="font-size:small"><a href="http://go.microsoft.com/fwlink/?LinkId=288798">Port from OpenGL ES 2.0 to Direct3D 11.1</a>&nbsp;</span></p>
<p>&nbsp;</p>
<h1><span>Using the templates</span></h1>
<p><br>
<span style="color:#000000; font-family:Segoe UI; font-size:small">To use these templates, download the ZIP file and open it. Extract the file<strong> GDC2013GameTemplates.vsix</strong> and double-click on it. This will install the templates. If Visual Studio
 2012 was open when you ran the VSIX installer, close it and re-open it. When you go to FILE-&gt;New Project, the three templates will appear under the Visual C&#43;&#43; project node.&nbsp;</span></p>
<p><span style="font-size:small"><strong><span style="color:#000000; font-family:Segoe UI">NOTE</span></strong><span style="color:#000000; font-family:Segoe UI">&nbsp;&nbsp;&nbsp;The VSIX installer must be extracted from the archive before installation. The
 SLN and VCXPROJ files exist only to meet validation requirements and can be ignored.</span></span></p>
<p><em><img id="78676" src="78676-gdc13_dx_gme_templates.png" alt="" width="700" height="477"></em></p>
<p>&nbsp;</p>
<h1><span>Source Code Files</span></h1>
<ul>
<li><em>GDC2013GameTemplates_VS2012.vsix</em> </li></ul>
<h1><em>&nbsp;</em></h1>
