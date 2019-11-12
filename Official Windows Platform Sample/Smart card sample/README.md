# Smart card sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
## Topics
- Devices and sensors
- Smart Card
- virtual smart card
## Updated
- 11/25/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the <a href="http://msdn.microsoft.com/library/windows/apps/dn263949">
<b>Windows.Devices.SmartCards</b></a> API to work with smart cards and smart card readers programmatically.
</p>
<p>Specifically, this sample shows how to:</p>
<ul>
<li>Create and set up a Trusted Platform Module (TPM) virtual smart card. </li><li>Change a user's personal identification number (PIN) for a physical smart card or TPM virtual smart card.
</li><li>Reset a user's PIN for a physical smart card or TPM virtual smart card. </li><li>Change an admin key (also known as an <i>administrator PIN</i> or <i>unblock PIN</i>) for a physical smart card or TPM virtual smart card.
</li><li>Create a challenge and verify the response for a physical smart card or TPM virtual smart card by using the admin key.
</li><li>Delete a TPM virtual smart card. </li><li>List all smart cards on the device. </li></ul>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>. </p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>. </p>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;For Windows&nbsp;8 app samples, download the <a href="http://go.microsoft.com/fwlink/p/?LinkId=301698">
Windows&nbsp;8 app samples pack</a>. The samples in the Windows&nbsp;8 app samples pack will build and run only on Microsoft Visual Studio&nbsp;2012.</p>
<p></p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://msdn.microsoft.com/library/windows/apps/dn263949"><b>Windows.Devices.SmartCards</b></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows app samples</a>
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
<b>Debug</b> &gt; <b>Start Without Debugging</b>.</p>
<p>To test the sample's scenarios, run the app with the <b>Local Machine</b> setting in Microsoft Visual Studio (not
<b>Simulator</b> or <b>Remote Machine</b>).</p>
<p>To create or delete a TPM virtual smart card, the machine must have a TPM chip installed. To verify this, open
<b>Device Manager</b> and expand <b>Security devices</b>. If a TPM chip is available, a device entry containing the words
<b>Trusted Platform Module</b> will appear.</p>
<p>After the app starts, select scenario 1 to create and provision a TPM virtual smart card. Modify the default values or leave them as-is, and select
<b>Create</b>. Type and confirm a PIN, and select <b>OK</b>. After you successfully run scenario 1, you can test the other scenarios.</p>
</div>
