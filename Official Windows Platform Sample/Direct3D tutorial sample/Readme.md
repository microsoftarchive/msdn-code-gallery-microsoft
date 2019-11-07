# Direct3D tutorial sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- DirectX
- Windows 8.1
- Windows Phone 8.1
## Topics
- Audio and video
- universal app
## Updated
- 04/02/2014
## Description

<div id="mainSection">
<p>This sample is a five-lesson tutorial that provides an intro to the Direct3D API, and which introduces the concepts and code used in many of the other DirectX samples.
</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info about how to build apps
 that target Windows and Windows Phone with Visual Studio, see <a href="http://msdn.microsoft.com/library/windows/apps/dn609832">
Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>Specifically, this sample covers:</p>
<ul>
<li>Basic initialization of DirectX and Direct3D by using the Windows Runtime </li><li>Creating meshes </li><li>Implementing pixel and vertex shaders </li><li>Rendering 3D geometry </li><li>Techniques for basic lighting and texturing </li></ul>
<p></p>
<p>These lessons build upon each other, from configuring DirectX for your C&#43;&#43; app to texturing primitives and adding effects.
</p>
<ol>
<li>Lesson1.Basics. This tutorial sample sets up DirectX resources in a C&#43;&#43; app. </li><li>Lesson2.Triangles. This tutorial sample creates a 3D cube from a mesh using a vertex shader.
</li><li>Lesson3.Cubes. This tutorial sample applies basic vertex lighting and coloring to the cube primitive created in Lesson 2.
</li><li>Lesson4.Textures. This tutorial sample loads DDS textures and applies them to a 3D primitive by using the cube created in Lesson 3. It also introduces a simple dot-product lighting model, where the cube surfaces are lighter or darker based on their distance
 and angle relative to a light source. </li><li>Lesson5.Components. This tutorial sample takes the concepts from the previous four lessons and demonstrates how to separate them into separate code objects for reuse.
</li></ol>
<p></p>
<p>This sample is written in C&#43;&#43; and requires basic experience with graphics programming concepts.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Microsoft Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh465137">An introduction to 3D graphics with DirectX</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dd370987">Direct2D graphics</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/ff476080">Direct3D 11 graphics</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/bb205169">DXGI reference</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229580">Getting started: create a aview with DirectX</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
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
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<p></p>
<ol>
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <b>File</b> &gt; <b>Open</b> &gt;
<b>Project/Solution</b>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory named for the sample and double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Follow the steps for the version of the sample you want:
<ul>
<li>
<p>To build the Windows version of the sample:</p>
<ol>
<li>Select the Windows version of any of the lessons in <b>Solution Explorer</b>.
<p>For example, select <b>Lesson5.Components.Windows</b>, which selects the individual .vcxproj file for lesson 5, Lesson5.Components\Windows\Lesson5.Components.Windows.vcxproj.</p>
</li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build Lesson5.Components.Windows</b>.
<p>This builds lesson 5 separately from the solution.</p>
</li></ol>
</li><li>
<p>To build the Windows Phone version of the sample:</p>
<ol>
<li>Select the Windows Phone version of any of the lessons in <b>Solution Explorer</b>.
<p>For example, select <b>Lesson5.Components.WindowsPhone</b>, which selects the individual .vcxproj file for lesson 5, Lesson5.Components\WindowsPhone\Lesson5.Components.WindowsPhone.vcxproj.</p>
</li><li>Press Ctrl&#43;Shift&#43;B, or use <b>Build</b> &gt; <b>Build Solution</b>, or use <b>
Build</b> &gt; <b>Build Lesson5.Components.WindowsPhone</b>.
<p>This builds lesson 5 separately from the solution.</p>
</li></ol>
</li></ul>
</li></ol>
<p></p>
<h2>Run the sample</h2>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><b>Deploying the sample</b></p>
<ul>
<li>
<p>To deploy the built Windows version of the sample:</p>
<ol>
<li>Select the Windows version of any of the lessons in <b>Solution Explorer</b>. For example, select
<b>Lesson5.Components.Windows</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b>, or use <b>Build</b> &gt; <b>Deploy Lesson5.Components.Windows</b>.
</li></ol>
</li><li>
<p>To deploy the built Windows Phone version of the sample:</p>
<ol>
<li>Select the Windows Phone version of any of the lessons in <b>Solution Explorer</b>. For example, select
<b>Lesson5.Components.WindowsPhone</b>. </li><li>Use <b>Build</b> &gt; <b>Deploy Solution</b>, or use <b>Build</b> &gt; <b>Deploy Lesson5.Components.WindowsPhone</b>.
</li></ol>
</li></ul>
<p><b>Deploying and running the sample</b></p>
<ul>
<li>
<p>To deploy and run the Windows version of the sample:</p>
<ol>
<li>Right-click the Windows version of any of the lessons in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. For example, right-click <b>Lesson5.Components.Windows</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li><li>
<p>To deploy and run the Windows Phone version of the sample:</p>
<ol>
<li>Right-click the Windows Phone version of any of the lessons in <b>Solution Explorer</b> and select
<b>Set as StartUp Project</b>. For example, right-click <b>Lesson5.Components.WindowsPhone</b> and select
<b>Set as StartUp Project</b>. </li><li>To debug the sample and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the sample without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>. </li></ol>
</li></ul>
</div>
