# XAML SwapChainPanel DirectX interop sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- XAML
## Topics
- User Interface
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the <a href="http://msdn.microsoft.com/library/windows/apps/dn252834">
<b>SwapChainPanel</b></a> control to include Microsoft DirectX content in your Windows Store app using C&#43;&#43;, C#, or Visual Basic app. This sample uses both Visual&nbsp;C&#43;&#43; component extensions (C&#43;&#43;/CX), C#, and Extensible Application Markup Language (XAML).</p>
<p></p>
<p>This sample show how to:</p>
<ul>
<li>Compose scenes with mixed DirectX and XAML content. </li><li>Implement multithreaded rendering and input handling on a background thread. </li><li>Change swap chain sizes in response to scale-change events. </li><li>Use transparent swap chains and <a href="http://msdn.microsoft.com/library/windows/apps/dn298732">
<b>CompositeMode</b></a> to create a highlighting effect. </li></ul>
<p></p>
<p></p>
<p>The sample demonstrates these tasks:</p>
<ol>
<li>
<p><b>Composition with XAML elements</b></p>
<p>This scenario shows how to rotate a <a href="http://msdn.microsoft.com/library/windows/apps/dn252834">
<b>SwapChainPanel</b></a> showing Microsoft Direct3D content, set its transparency, and compose it with other XAML elements.</p>
</li><li><b>Handle input, render ink strokes, and recognize handwriting</b>
<p>This scenario shows how to handle input, render ink strokes, and recognize handwriting using Direct2D on a background thread.</p>
</li><li><b>Scale DirectX content smoothly and precisely</b>
<p>This scenario shows how to scale DirectX content in a <a href="http://msdn.microsoft.com/library/windows/apps/br209527">
<b>ScrollViewer</b></a> control.</p>
</li><li><b>Use a transparent swap chain</b>
<p>This scenario shows how to use <a href="http://msdn.microsoft.com/library/windows/apps/dn252834">
<b>SwapChainPanel</b></a> for transparent swap chains. A transparent swap chain is placed over XAML text, and the panel's
<a href="http://msdn.microsoft.com/library/windows/apps/dn298732"><b>CompositeMode</b></a> is set to
<a href="http://msdn.microsoft.com/library/windows/apps/dn298530"><b>ElementCompositeMode.MinBlend</b></a>, to enable a highlighter effect.</p>
</li><li><b>Add accessibility support for DirectX content</b>
<p>This scenario shows how to add UI Automation (UIA) accessibility support for DirectX content by creating accessibility peers.</p>
</li></ol>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Roadmaps</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229583">Roadmap for C# and Visual Basic</a>
</dt><dt><b>Samples</b> </dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows app samples</a>
</dt><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn252834"><b>SwapChainPanel</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn298732"><b>CompositeMode</b></a>
</dt></dl>
<h2>Operating system requirements</h2>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>Windows&nbsp;8.1 </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>Windows Server&nbsp;2012&nbsp;R2 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<ol>
<li>Start Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</div>
