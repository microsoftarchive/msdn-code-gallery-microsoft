# Contact manager API sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- NEW IN RTM
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>Demonstrates the functionality of the API of the <a href="http://msdn.microsoft.com/library/windows/apps/br225002">
<b>Windows.ApplicationModel.Contacts</b></a> namespace. </p>
<p>The sample has these scenarios:</p>
<ul>
<li><b>Show contact card</b>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/dn263588"><b>ContactManager.ShowContactCard(Contact, Rect, Placement)</b></a> API to query the contact. Then, uses
<a href="http://msdn.microsoft.com/library/windows/apps/br224849"><b>Contact</b></a> properties and other object properties of the
<a href="http://msdn.microsoft.com/library/windows/apps/br225002"><b>Windows.ApplicationModel.Contacts</b></a> namespace.</p>
</li><li><b>Show contact card with delay loaded data</b>
<p>Uses the <a href="http://msdn.microsoft.com/library/windows/apps/dn263597"><b>ContactManager.ShowDelayLoadedContactCard</b></a> API to obtain a
<a href="http://msdn.microsoft.com/library/windows/apps/dn297400"><b>ContactCardDelayedDataLoader</b></a> object. Then, calls
<a href="http://msdn.microsoft.com/library/windows/apps/dn297413"><b>ContactCardDelayedDataLoader.SetData</b></a> to update the contact card with the full set of
<a href="http://msdn.microsoft.com/library/windows/apps/br224849"><b>Contact</b></a> data.</p>
</li></ul>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>. </p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>. </p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><b>Reference</b> </dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224849"><b>Contact</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn297400"><b>ContactCardDelayedDataLoader</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br225002"><b>Windows.ApplicationModel.Contacts</b></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br225014"><b>Windows.ApplicationModel.Contacts.Provider</b></a>
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
