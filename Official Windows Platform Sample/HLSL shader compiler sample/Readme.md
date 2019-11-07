# HLSL shader compiler sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- XAML
- DirectX
- Windows Runtime
## Topics
- HLSL compiler
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to use HLSL compiler APIs such as <a href="http://msdn.microsoft.com/library/windows/apps/dd607324">
<b>D3DCompile</b></a> from within Windows Store apps. It also demonstrates a new feature in the compiler,
<a href="direct3d11.direct3d_11_2_features#link">HLSL shader linking</a>, which allows linking of precompiled shader libraries to form complete shaders at run time. For more info about how to use shader linking, see
<a href="http://msdn.microsoft.com/library/windows/apps/dn466359">Using shader linking</a>.
</p>
<p>This sample uses these APIs:</p>
<ul>
<li><a href="http://msdn.microsoft.com/library/windows/apps/dn280341"><b>D3DCreateLinker</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn280558"><b>ID3D11Linker</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn280560"><b>ID3D11Linker::Link</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn280342"><b>D3DLoadModule</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn280564"><b>ID3D11ModuleInstance</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn280608"><b>ID3D11Module::CreateInstance</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn280535"><b>ID3D11FunctionLinkingGraph</b></a>
</li><li><a href="http://msdn.microsoft.com/library/windows/apps/dn280340"><b>D3DCreateFunctionLinkingGraph</b></a>
</li></ul>
<p></p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8.1 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=243667">
Windows&nbsp;8.1 app samples pack</a>. The samples in the Windows&nbsp;8.1 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2013.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn280558"><b>ID3D11Linker</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn280342"><b>D3DLoadModule</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn280564"><b>ID3D11ModuleInstance</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn280340"><b>D3DCreateFunctionLinkingGraph</b></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkId=243667">Windows 8.1 app samples</a>
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
<p></p>
<ol>
<li>Start Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample. </li></ol>
<p></p>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </p>
</div>
