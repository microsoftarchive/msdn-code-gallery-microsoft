# Voice Commands Quickstart
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Speech
- Windows Phone 8.1
## Topics
- Speech
- Text to Speech
- Voice Comands
## Updated
- 01/23/2015
## Description

<div id="mainSection">
<div class="clsServerSDKContent">
<h1><span style="font-size:10px">This sample demonstrates how to use voice commands with Cortana in your Windows Phone Store app.</span><a id="gallery_samples.voice_commands_quickstart_gallery"></a></h1>
</div>
<p>The sample demonstrates these features and aspects:&nbsp;</p>
<ul>
<li>Authoring and configuring your Voice Command Definition (VCD) file. </li><li>Installing the VCD file on app launch. </li><li>Handling your app being activated by a voice command. </li><li>Determining whether the voice command that activated your app was actually spoken, or whether it was typed in as text.
</li><li>Navigating to a page in your app based on parameters in a voice command. </li><li>Using a phrase topic to allow dictation to be part of a voice command. </li><li>Using a phrase topic with a subject (in this case, movie titles) in a voice command, to further refine the relevance of speech recognition results.
</li><li>Using text-to-speech (TTS) to give audible feedback about the voice command; but only if the voice command was spoken.
</li><li>Programmatically redefining a phrase list.&nbsp; </li></ul>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;Building this sample requires Windows&nbsp;8.1 and Microsoft Visual Studio&nbsp;2013 with Update 2 or later.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Visual Studio&nbsp;2013, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301697">
Visual Studio&nbsp;2013</a>. After you install Visual Studio&nbsp;2013, update your installation with Update 2 or later.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="https://msdn.microsoft.com/en-us/library/dn630430.aspx">Quickstart: Voice commands (XAML)</a>
</dt><dt><a href="https://msdn.microsoft.com/en-us/library/windows/apps/xaml/dn792133.aspx">How to dynamically modify Voice Command Definition (VCD) phrase lists (XAML)</a>
</dt><dt><a href="https://msdn.microsoft.com/en-us/library/windows/apps/dn722330.aspx">Quickstart: Voice commands (HTML)</a>
</dt><dt><a href="https://msdn.microsoft.com/en-us/library/windows/apps/dn747872.aspx">How to dynamically modify Voice Command Definition (VCD) phrase lists (HTML)</a>
</dt><dt><a href="https://msdn.microsoft.com/en-us/library/dn722331.aspx"><strong>Voice command elements and attributes</strong></a>
</dt></dl>
<h2>Operating system requirements</h2>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>None supported </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>None supported </dt></td>
</tr>
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample<span style="font-size:10px">&nbsp;</span></h2>
<ol>
<li>Start Visual Studio Express&nbsp;2013 for Windows --&gt; and select <strong>File</strong> &gt;
<strong>Open</strong> &gt; <strong>Project/Solution</strong>. </li><li>Go to the directory in which you unzipped the sample. Go to the directory named for the sample, and double-click the Visual Studio Express&nbsp;2013 for Windows Solution (.sln) file.
</li><li>Press F7 or use <strong>Build</strong> &gt; <strong>Build Solution</strong> to build the sample.&nbsp;
</li></ol>
<h2>Run the sample</h2>
<p>To debug the app and then run it, press F5 or use <strong>Debug</strong> &gt; <strong>
Start Debugging</strong>. To run the app without debugging, press Ctrl&#43;F5 or use <strong>
Debug</strong> &gt; <strong>Start Without Debugging</strong>.</p>
<p>The app has now been launched, so it has installed the voice commands.</p>
<p>Press the <strong>Search</strong> button to launch Cortana, or press and hold <strong>
Search</strong> to speak right away.</p>
<p>You may have turned on Cortana when you set up your phone. If you did, you'll see her when you press the
<strong>Search</strong> button. If you press the <strong>Search</strong> button and see the Bing image of the day, that means Cortana's not on. To turn on Cortana, in the App list, tap
<strong>Settings</strong>, swipe over to <strong>Applications</strong>, tap <strong>
Cortana</strong>, and then turn on <strong>Cortana</strong>.</p>
<p>At Cortana, tap <strong>See more</strong> then scroll down to <strong>Apps</strong> and tap
<strong>Voice Commands Quickstart</strong> to see what you can say. Tap the microphone and speak. Try saying &quot;Quickstart, show sports section&quot;.</p>
<p>Go to the <strong>How to use</strong> page in the app and tap <strong>Add more sections</strong>. Now two new newspaper sections have been added: &quot;entertainment&quot; and &quot;weather&quot;. So you can now say &quot;Quickstart, show entertainment section&quot;.</p>
<p>Also, try saying &quot;Quickstart, text Avery I'm running late&quot; (to see the short message phrase topic in action) or &quot;Quickstart, play Gone With The Wind&quot; (to demo the phrase topic with a search scenario with the subject movies).</p>
</div>
