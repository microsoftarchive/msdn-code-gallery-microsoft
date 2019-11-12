# UI Automation fragment provider sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Win32
## Topics
- Automation
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample shows how an application can use Microsoft UI Automation to provide programmatic access to the elements in a UI fragment (in this case, a custom list box control) that is hosted in a window (<b>HWND</b>). The control itself has been kept simple.
 It does not support scrolling and therefore an arbitrary limit has been set on the number of items it can contain. For convenience, list items are stored in a deque (from the Standard Template Library).
</p>
<p>The fragment consists of the root element (a list box) and its children (the list items). The UI Automation provider for the list box supports only one control pattern, the
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee671285">Selection</a> pattern. The provider for the list items implements the
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee671286">SelectionItem</a> control pattern.</p>
<p>To see the UI Automation provider at work, run the application and then open Inspect (an accessibility testing tool available from the Windows Software Development Kit (SDK)). You can see the application and its controls in the Control View. Clicking on
 the control or on the list items in the Inspect tree causes the provider's methods to be called, and the values returned are displayed in the Properties pane. (Note that some values are retrieved from the default
<b>HWND</b> provider, not this custom implementation.) You can also select an item in the list by using the
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee671286">SelectionItem</a> control pattern in Inspect.</p>
<p class="note"><b>Warning</b>&nbsp;&nbsp;This sample requires Microsoft Visual Studio&nbsp;2013 and will not compile in Microsoft Visual Studio Express&nbsp;2013 for Windows.</p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3><a id="related_topics"></a>Related topics</h3>
<dl><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee671355"><b>ISelectionProvider</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee671349"><b>ISelectionItemProvider</b></a>
</dt><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee671596">UI Automation Provider Programmer's Guide</a>
</dt></dl>
<h3>Related technologies</h3>
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/ee684009">UI Automation</a>
<h3>Operating system requirements</h3>
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
<h3>Build the sample</h3>
<p>To build this sample:</p>
<ol>
<li>Start Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample. Go to the C&#43;&#43; directory and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>Press F7 (or F6 for Visual Studio&nbsp;2013) or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample.
</li></ol>
<p></p>
<h3>Run the sample</h3>
<p>To run this sample after building it, go to the installation folder for this sample with Windows Explorer and run UIAFragmentProvider.exe from the
<i>&lt;install_root&gt;</i>\UI Automation Fragment Provider Sample\C&#43;&#43;\Debug folder.
</p>
<p>To run this sample from Microsoft Visual Studio, press the F5 key to run with debugging enabled, or Ctrl&#43;F5 to run without debugging enabled. Alternatively, select
<b>Start Debugging</b> or <b>Start Without Debugging</b> from the <b>Debug</b> menu.</p>
</div>
