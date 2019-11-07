# Spell checking provider sample
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- COM
## Topics
- Services
## Updated
- 10/17/2013
## Description

<div id="mainSection">
<p>This sample demonstrates how to create a sample spell checking provider that conforms to the Windows
<a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh869852">Spell Checking API</a>. It is registered with Windows and can then be used in text controls if selected by the user in the
<b>Language Control Panel</b>. </p>
<p>This sample works in conjunction with the <a href="http://go.microsoft.com/fwlink/p/?linkid=242818">
Spell checking client sample</a>.</p>
<p>The following documentation provides general guidance for the APIs used in this sample:</p>
<ul>
<li><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh869852">Spell Checking API</a>
</li><li><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh869853">Spell Checking API Reference</a>
</li></ul>
<p></p>
<p class="note"><b>Note</b>&nbsp;&nbsp;The Windows Samples Gallery contains a variety of code samples that exercise the various new programming models, platforms, features, and components available in Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. These downloadable samples
 are provided as compressed ZIP files that contain a Visual Studio solution (SLN) file for the sample, along with the source files, assets, resources, and metadata necessary to successfully compile and run the sample. For more information about the programming
 models, platforms, languages, and APIs demonstrated in this sample, please refer to the guidance, tutorials, and reference topics provided in the Windows&nbsp;8.1 documentation available in the Windows Developer Center. This sample is provided as-is in order to
 indicate or demonstrate the functionality of the programming models and feature APIs for Windows&nbsp;8.1 and/or Windows Server&nbsp;2012&nbsp;R2. Please provide feedback on this sample!</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>.</p>
<h3><a id="related_topics"></a>Related topics</h3>
<dl><dt><a href="http://msdn.microsoft.com/en-us/library/windows/desktop/hh869852">Spell Checking API</a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?linkid=242818">Spell checking client sample</a>
</dt></dl>
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
<p></p>
<ol>
<li>Start Visual Studio&nbsp;2013 and select <b>File</b> &gt; <b>Open</b> &gt; <b>Project/Solution</b>.
</li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio&nbsp;2013 Solution (.sln) file.
</li><li>Press F7 (or F6 for Visual Studio&nbsp;2013) or use <b>Build</b> &gt; <b>Build Solution</b> to build the sample.
</li></ol>
<p></p>
<h3>Run the sample</h3>
<p>The sample must be installed as a spell checking provider by performing the following steps.</p>
<ol>
<li>Create a folder named &quot;SampleSpellingProvider&quot; under &quot;C:\Program Files\&quot;. </li><li>Copy the SampleProvider.dll you've built to &quot;C:\Program Files\SampleSpellingProvider\&quot;.
</li><li>Run one of the reg files provided with this sample:
<ul>
<li>currentuser.reg if you want to install for the current user. </li><li>localmachine.reg if you want to install for all users. </li></ul>
<p class="note"><b>Note</b>&nbsp;&nbsp;The reg files contain the path &quot;c:\\Program Files\\SampleSpellingProvider\\SampleSpellingProvider.dll&quot;. Edit it appropriately if you placed your .dll in a different location.</p>
</li><li>Open the desktop <b>Control Panel</b>, select <b>Clock, Language, and Region</b> &gt;
<b>Language</b>, and double-click <b>English</b> in the list of languages. </li><li>Select &quot;Sample Spell Checker&quot; as the default spell checker for English. </li></ol>
<p></p>
<p>The sample spell checking provider will now be used as the English spell checker by Windows controls and any clients of the spell checking API. You can install the
<a href="http://go.microsoft.com/fwlink/p/?linkid=242818">spell checking client sample</a> to exercise this provider sample.</p>
</div>
