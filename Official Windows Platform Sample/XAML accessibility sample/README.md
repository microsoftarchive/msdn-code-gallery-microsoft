# XAML accessibility sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- XAML
- Windows Runtime
## Topics
- User Interface
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample shows you how to add basic accessibility support to your app. </p>
<p>Specifically, this sample covers:</p>
<ul>
<li>Automation properties set at the app level. This is typically done using the <a href="http://msdn.microsoft.com/library/windows/apps/br209081">
<b>AutomationProperties</b></a> attached properties. For example, you can set the accessible name that is crucial to many assistive technology scenarios by setting
<a href="http://msdn.microsoft.com/library/windows/apps/hh759770"><b>AutomationProperties.Name</b></a>.
</li><li>Tab sequence, which includes checking and using the existing layout-derived tab sequence, and deliberately modifying that sequence by setting
<a href="http://msdn.microsoft.com/library/windows/apps/br209461"><b>TabIndex</b></a>.
</li><li>Overriding the <a href="http://msdn.microsoft.com/library/windows/apps/br209185">
<b>AutomationPeer</b></a> class to report specific information about a control. This includes information about the name, role and value of the control from an accessibility framework perspective. The peer implements one of the automation patterns, by inheriting
 and implementing a pattern interface and its members. Automation clients can then query for available patterns and invoke automation APIs using that pattern.
</li><li>Adding accessibility support to the items in a data-bound custom <a href="http://msdn.microsoft.com/library/windows/apps/br242878">
<b>ListView</b></a> by overriding the <a href="http://msdn.microsoft.com/library/windows/apps/br242840">
<b>PrepareContainerForItemOverride</b></a> method to set automation properties. </li></ul>
<p></p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh452680">Accessibility in apps using C&#43;&#43;, C#, or Visual Basic</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/jj134090">Guidelines and checklist for accessibility</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209081"><b>AutomationProperties</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242840"><b>ItemsControl.PrepareContainerForItemOverride</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br229583">Roadmap for C# and Visual Basic</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209179"><b>Windows.UI.Xaml.Automation</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br242563"><b>Windows.UI.Xaml.Automation.Peers</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br209225"><b>Windows.UI.Xaml.Automation.Provider</b></a>
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
</tbody>
</table>
<h2>Build the sample</h2>
<ol>
<li>
<p>Start Visual Studio and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.</p>
</li><li>
<p>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.</p>
</li><li>
<p>Press F7 or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample.</p>
</li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <b>Debug</b> &gt; <b>Start Debugging</b>. To run the app without debugging, press Ctrl&#43;F5 or use
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
</div>
